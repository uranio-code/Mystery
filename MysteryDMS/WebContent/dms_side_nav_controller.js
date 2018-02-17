app.directive('dmsSideNav', function () {
    return {
        scope: { content: '=content' }, 
        restrict: 'E',
        templateUrl: 'DMS/dms_side_nav.html'
    };
});

app.controller("dmsSideNavController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$rootScope', '$location',
        function ($scope, MysteryDownloader, $translate, $uibModal, $rootScope, $location) {
            var me = this;
            me.favorites = [];
            me.users = [];
            me.content = $scope.content;
            MysteryDownloader.get('GetFavorites', function (data) {
                me.favorites = data.output.contents;
            });
            
            $rootScope.$on("new_content", function (event, data) {
                if (angular.isUndefined(data.ContentType) || data.ContentType != 'UserFavorite')
                    return;
                MysteryDownloader.loadContent(data.content_reference, function (content) {
                    me.favorites.push(content);
                });
                
            });

            $rootScope.$on("content_deleted", function (event, data) {
                if (angular.isUndefined(data.ContentType) || data.ContentType != 'UserFavorite')
                    return;
                MysteryDownloader.loadContent(data.content_reference, function (content) {
                    var index = me.favorites.indexOf(content);
                    if (index > -1) {
                        me.favorites.splice(index, 1);
                    }
                });
                
            });

          
        }]);