app.directive('mysteryHistory', function () {
    return {
        scope: {
            entries: '=entries',
            serviceUrl: '=serviceUrl',
            header: '=header',
        },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/History/History.html'
    };
});

app.controller("mysteryHistoryController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        me.data = $scope.entries;
        me.service_url = angular.isDefined($scope.serviceUrl) ? $scope.serviceUrl : '';
        if (angular.isDefined($scope.header)) {
            me.header = $translate.instant($scope.header);
        }
        else
            me.header = $translate.instant("COMMON.HISTORY_HEADING");
        
        
        //if we have a service url contents shall always come from there
        if (me.service_url.length > 0) {
            MysteryDownloader.get(me.service_url,
            function (data) {
                me.data = data.output;
            });
        }


    }]);
