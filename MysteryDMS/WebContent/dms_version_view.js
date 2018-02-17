app.controller("dmsVersionViewController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$rootScope', '$timeout',
        function ($scope, MysteryDownloader, $translate, $uibModal, $rootScope, $timeout) {
            var me = this;
            me.paths = [];
            me.path_loading = true;
            me.content = null;

            me.init = function (content) {
                me.content = content;
                MysteryDownloader.get('MysteryDMSService/paths/' + content.ContentType + '/' + content.tiny_guid, function (response) {
                    me.path_loading = false;
                    if (angular.isUndefined(response.output) ||
                       response.output === null ||
                       angular.isUndefined(response.output.paths))
                        return;
                    me.paths = response.output.paths;
                    angular.forEach(me.paths, function (path, key) {
                        angular.forEach(path, function (node, key) {
                            //we make sure all paths are loaded
                            MysteryDownloader.loadContent(node);
                        });
                    })
                });
            }

            me.tabs = [
                {
                    name: "metadata",
                    label: "DMS.VERSION.METADATA",
                    templateUrl: 'DMS/Version/MetaData.html',
                    active: true,
                    icon_class: "fa fa-file-text",
                },
                {
                    name: "history",
                    label: "DMS.VERSION.HISTORY",
                    templateUrl: 'DMS/Version/History.html',
                    active: false,
                    icon_class: "fa fa-history",
                },
                {
                    name: "comments",
                    label: "DMS.VERSION.COMMENTS",
                    templateUrl: 'DMS/Version/Comments.html',
                    active: false,
                    icon_class: "fa fa-wechat",
                },
                {
                    name: "preview",
                    label: "DMS.VERSION.PREVIEW",
                    templateUrl: 'DMS/Version/Preview.html',
                    active: false,
                    icon_class: "fa fa-eye",
                },
                {
                    name: "workflow",
                    label: "DMS.VERSION.WORKFLOW",
                    templateUrl: 'DMS/Version/Workflow.html',
                    active: false,
                    icon_class: "fa fa-cog fa-spin",
                },
            ];

            me.active_tab = me.tabs.filter(function (tab) { return tab.active; })[0];

            me.selectTab = function (tab) {
                if (me.active_tab !== null) me.active_tab.active = false;
                tab.active = true;
                me.active_tab = tab;
            };

        }]);