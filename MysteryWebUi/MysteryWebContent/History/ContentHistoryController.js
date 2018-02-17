app.directive('mysteryContentHistoryView', function () {
    return {
        scope: { content: '=content' },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/History/ContentHistoryView.html'
    };
});

app.controller("ContentHistoryRouteController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        var path_array = $location.path().split("/");
        me.content = {
            ContentType: path_array[path_array.length - 3],
            tiny_guid: path_array[path_array.length - 2],
            authorized: true, //this will became false if we are not authorized after the download
        };
        me.content = MysteryDownloader.loadContent(me.content);
    }
]);
app.controller("ContentHistoryController", ['MysteryDownloader', '$scope',
    function (MysteryDownloader, $scope) {
        var me = this;
        me.content = $scope.content;
        me.entries = [];
        if (me.content !== null) {
            MysteryDownloader.post('ContentService/History', 
                { ContentType: me.content.ContentType, tiny_guid: me.content.tiny_guid },
                function (data) {
                    Array.prototype.push.apply(me.entries, data.output);
                });
        }

    }]);
