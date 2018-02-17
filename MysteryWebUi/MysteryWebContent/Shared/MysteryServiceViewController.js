app.controller("MysteryServiceViewController",['$scope', 'MysteryDownloader', '$translate', '$uibModal',
    function ($scope, MysteryDownloader, $translate, $uibModal) {
        var me = this;
        me.url = '';
        me.input = {};
        me.output = '';
        me.loading = false;

        me.invokeGet = function ()
        {
            me.loading = true;
            MysteryDownloader.get(me.url, function (data) {
                me.output = angular.toJson(data.output,true);
                me.loading = false;
            });
        }

}]);