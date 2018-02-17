app.directive('dmsFolderTree', function () {
    return {
        scope: { uid: '=uid',content:'=content' },
        restrict: 'E',
        templateUrl: 'DMS/dms_folder_tree.html'
    };
});
app.directive('dmsFolderTreeNode', function () {
    return {
        scope: { uid: '=uid', content: '=content', },
        restrict: 'E',
        templateUrl: 'DMS/dms_folder_tree_node.html'
    };
});

app.factory("dmsFolderTreeService", ['MysteryDownloader', '$rootScope', function (MysteryDownloader, $rootScope) {
    var me = this;
    me.roots = [];
    me.instanceNavData = function (node) {
        if (angular.isDefined(node.nav_data))//already done
            return;
        MysteryDownloader.loadContent(node, function (content) {
            if (content === null)
                return;
            content.nav_data = {
                open: false,
                can_open: angular.isDefined(content.subfolders) && content.subfolders.length >= 0,
                getSubFolderToRender: function () {
                    if (content.nav_data.open)
                        return content.subfolders;
                    else
                        return [];
                },
            }
        });
    }

    me.refreshNavData = function (node) {
        if (node === null)
            return;
        if (angular.isUndefined(node.nav_data))
        {
            me.instanceNavData(node);
            return;
        }
        node.nav_data.can_open = angular.isDefined(node.subfolders) && node.subfolders.length > 0;
    }

    me.handler = function (event, data) {
        if (angular.isUndefined(data) || angular.isUndefined(data.guid))
            return;
        me.refreshNavData(data);
    }


    $rootScope.$on("content_deleted", me.handler);
    $rootScope.$on("new_content", me.handler);
    $rootScope.$on("changed_content", me.handler);

    me.paths = [];

    me.openToContent = function (event, content) {
        if (angular.isUndefined(content))
            return;
        if (angular.isUndefined(content.ContentType))
            return;
        if (!(content.ContentType === "DMSFolder" || content.ContentType === "DMSVersion"))
            return;
        MysteryDownloader.get('MysteryDMSService/paths/' + content.ContentType + '/' + content.tiny_guid, function (response) {
            if (angular.isUndefined(response.output) ||
                angular.isUndefined(response.output.paths))
                return;
            me.paths = response.output.paths;
            //expand all the nodes in all the paths
            angular.forEach(me.paths, function (path, key) {
                angular.forEach(path, function (node, key) {
                    //we make sure it is loaded
                    MysteryDownloader.loadContent(node, function (content) {
                        //me make sure it has nav data
                        me.instanceNavData(content);
                        //and we open it
                        me.open(content);
                    });
                })
            });
        });
    };

    $rootScope.$on("content_view_event", me.openToContent);

    MysteryDownloader.get('GetDMSFolderTree', function (response) {
        me.roots = response.output.contents;
        angular.forEach(me.roots, function (value, key) {
            me.instanceNavData(value);
        });
    });

    me.open = function (node) {
        node.nav_data.open = true;
        angular.forEach(node.subfolders, function (value, key) {
            me.instanceNavData(value);
        });
    }

    return me;

}]);

app.controller("dmsFolderTreeController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$rootScope', 'dmsFolderTreeService',
        function ($scope, MysteryDownloader, $translate, $uibModal, $rootScope, dmsFolderTreeService) {
            var me = this;
            //content is either given or we will download it
            me.getRoots = function () {
                return dmsFolderTreeService.roots;
            };
            
            me.open = dmsFolderTreeService.open;


        }]);

app.controller("dmsFolderTreeNodeController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$rootScope', 'dmsFolderTreeService',
        function ($scope, MysteryDownloader, $translate, $uibModal, $rootScope, dmsFolderTreeService) {
            var me = this;
            me.node = MysteryDownloader.loadContent($scope.content);

            dmsFolderTreeService.instanceNavData(me.node);
            me.open = dmsFolderTreeService.open;

            me.delete_action = null;
            me.delete_action_download = MysteryDownloader.getPromise('ContentService/' + me.node.ContentType + '/' + me.node.tiny_guid + '/action-info/delete');
            me.delete_action_download.then(function (data) {
                me.delete_action = data.output;
            });

            me.modal_options = {
                animation: true,
                templateUrl: 'MysteryWebContent/MysteryContent/Actions/DeleteContent.html',
                controller: 'deleteContentController as c',
            };

            me.openDeleteModal = function () {
                var modalInstance = $uibModal.open(me.modal_options);
                modalInstance.content = me.node;
                modalInstance.action = me.delete_action;
                modalInstance.go_away_after_deletion = false;
            }

            me.deleteContent = function () {
                //still downloading the action, it will trigger as soon as ready
                if (me.delete_action === null)
                    me.delete_action_download.then(me.openDeleteModal);
                else
                    me.openDeleteModal();
            };
            me.loadPropertyInfoInNodeTitleEditController = function (property_controller) {
                MysteryDownloader.loadContent(me.node, function () {
                    property_controller.init(me.node.propertiesUi['title'])
                });
            };

            me.add_new_folder_action = null;
            MysteryDownloader.get(
                'ContentService/' + me.node.ContentType + '/' + me.node.tiny_guid + '/action-info/DMSCreateNewFolder',
                function (data) {
                    me.add_new_folder_action = data.output;
                });

            me.adding_folder = false;
            me.new_folder_title = '';
            me.add_new_folder = function () {
                if (me.adding_folder) // still waiting previous add to be done
                    return;
                if (me.new_folder_title.length ===0) // we don't add folder without titles.
                    return;
                me.adding_folder = true;
                MysteryDownloader.post('DMSCreateNewFolderFromTree',
                    { tiny_guid: me.node.tiny_guid, new_folder_title: me.new_folder_title },
                    function (data) {
                        //all the new things are taken care by mystery it self, we need only to reset the control.
                        me.new_folder_title = '';
                        me.adding_folder = false;
                    });
            };
            me.add_new_folder_cancel = function () {
                me.new_folder_title = '';
            }

        }]);