app.directive('mysteryStartpanel', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/Shared/Start/StartPanel.html'
    };
});

app.controller("StartController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal',
    function ($scope, MysteryDownloader, $translate, $uibModal) {
        var me= this;
        
    }
]);



app.controller("StartPanelController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal',
    function ($scope, MysteryDownloader, $translate, $uibModal) {
        var me = this;
        me.buttons = [];
        MysteryDownloader.get('ContentTypeService/ContentTypeButtons', function (data) {
            me.buttons = data.buttons;
        });
    }
]);