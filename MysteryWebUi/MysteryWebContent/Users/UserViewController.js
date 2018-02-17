app.directive('mysteryUserActivityStream', function () {
    return {
        scope: {
            user: '=user',
        },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/Users/UserActivityStream.html'
    };
});

app.controller("UserViewController", ['MysteryDownloader',
    function (MysteryDownloader) {
        var me = this;
    }]);

app.controller("UserActivityStreamController",['MysteryDownloader','$scope',
    function (MysteryDownloader,$scope) {
        var me = this;
        me.user = $scope.user;
        me.entries = [];
        if (me.user !== null) {
            MysteryDownloader.get('UserService/ActivityStream/' + me.user.tiny_guid, function (data) {
                Array.prototype.push.apply(me.entries, data.output);
            });
        }
        
    }]);
