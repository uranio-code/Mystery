app.controller("MysteryMainController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$rootScope', '$location',
        function ($scope, MysteryDownloader, $translate, $uibModal, $rootScope, $location) {
            var me = this;
            $rootScope.$on("$routeChangeSuccess", function (event, next, current) {
                if (angular.isUndefined(next))
                    return;
                if (angular.isUndefined(next.params))
                    return;
                if (angular.isUndefined(next.params.tiny_guid))
                    return;
                if (angular.isUndefined(next.params.Extra))
                    return;
                if (next.params.Extra !== "ContentView")
                    return;
                var tiny_guid = next.params.tiny_guid;

                var path_array = $location.path().split("/");
                me.content = {
                    ContentType: path_array[path_array.length - 2],
                    tiny_guid: tiny_guid,
                    authorized: true,
                };

                MysteryDownloader.loadContent(me.content, function (content) {
                    if (content !== null) {
                        $rootScope.$broadcast("content_view_event", me.content);
                        MysteryDownloader.post('SessionService/RegisterAccess',
                            {
                                tiny_guid: tiny_guid,
                                content_type_name: content.ContentType,
                            });
                    }
                });
            });
        }]);