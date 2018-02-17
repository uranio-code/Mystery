app.controller("dmsFolderViewController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$rootScope', '$timeout',
        function ($scope, MysteryDownloader, $translate, $uibModal, $rootScope, $timeout) {
            var me = this;
            me.folder = null;
            me.documents = [];
            me.displayed_documents = [];
            me.DragAndDropVisible = false;
            me.paths = [];
            me.path_loading = true;
            me.init = function (folder) {
                me.folder = MysteryDownloader.loadContent(folder, function (folder) {
                    me.documents = folder.versions;
                    me.displayed_documents = folder.versions;
                    angular.forEach(me.documents, function (doc, index) {
                        MysteryDownloader.loadContent(doc);
                    });
                });
                //breadcrumb
                MysteryDownloader.get('MysteryDMSService/paths/' + folder.ContentType + '/' + folder.tiny_guid, function (response) {
                    if (angular.isUndefined(response.output) ||
                        response.output === null ||
                        angular.isUndefined(response.output.paths))
                        return;
                    me.paths = response.output.paths;
                    me.path_loading = false;
                    angular.forEach(me.paths, function (path, key) {
                        angular.forEach(path, function (node, key) {
                            //we make sure all paths are loaded
                            MysteryDownloader.loadContent(node);
                        });
                    })
                });
            };

            me.tabs = [
                {
                    name: "documents",
                    label: "DMS.FOLDER.DOCUMENTS",
                    templateUrl: 'DMS/Folder/Documents.html',
                    active: true,
                    icon_class: "fa fa-file-text",
                },
                {
                    name: "metadata",
                    label: "DMS.FOLDER.PERMISSIONS",
                    templateUrl: 'DMS/Folder/Permissions.html',
                    active: false,
                    icon_class: "fa fa-lock",
                },
                {
                    name: "history",
                    label: "DMS.FOLDER.HISTORY",
                    templateUrl: 'DMS/Folder/History.html',
                    active: false,
                    icon_class: "fa fa-history",
                },
                {
                    name: "comments",
                    label: "DMS.FOLDER.COMMENTS",
                    templateUrl: 'DMS/Folder/Comments.html',
                    active: false,
                    icon_class: "fa fa-wechat",
                },
            ];

            me.active_tab = me.tabs.filter(function (tab) { return tab.active; })[0];

            me.selectTab = function (tab) {
                if(me.active_tab !== null) me.active_tab.active = false;
                tab.active = true;
                me.active_tab = tab;
            };


            me.document_table_options = {
                content_type_name: 'DMSVersion',
                contents_source_promise: function () {
                    return MysteryDownloader.getPromise('MysteryDMSService/GetFolderDocumentVersions?folder=' + me.folder.tiny_guid,
                        function (data) {
                            return data.output;
                        });
                },
            };

            //drop zone
            me.dropped_file = null;
            me.onUpload = function (event, data, previewId, index) {
                me.dropped_file = MysteryDownloader.parseRespose(data.response).output[0];
                if (me.dropped_file === null)
                    return;
                MysteryDownloader.post('DMSCreateNewDocumentFromFile',
                    { file: me.dropped_file, tiny_folder_guid: me.folder.tiny_guid},
                    function (data) {
                        //TBD maybe we want to go for it? but for multi maybe not..
                        me.active_tab = null;
                        $timeout(function(){me.selectTab(me.tabs[0])});//going for docs
                    });
            };
            //to be passed to the linker function
            $scope.onUpload = me.onUpload;

            me.toggleDragAndDrop = function () {
                me.DragAndDropVisible = !me.DragAndDropVisible;
            }


        }]);