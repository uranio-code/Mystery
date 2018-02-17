app.controller("dmsStartPageController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$rootScope', '$q',
        function ($scope, MysteryDownloader, $translate, $uibModal, $rootScope, $q) {
            var me = this;

            me.last_version_options = {
                content_type_name: 'DMSVersion',
                contents_source_promise: function () {
                    return MysteryDownloader.getPromise('GetLastDMSVersions',
                        function (data) { return data.output.contents; });
                },
            };

        }]);