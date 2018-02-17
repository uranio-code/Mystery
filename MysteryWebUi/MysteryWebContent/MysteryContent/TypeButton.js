app.controller("TypeButtonController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal','$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        me.data = {
            panel_type: 'primary',
            icon: 'fa-square',
            details: 'DETAILS',
            url: ''
        };

        me.init = function (json_data) {
            angular.merge(me.data ,angular.fromJson(json_data));
        };

        
    }
]);