app.factory("MysteryInstance", [
    'MysteryDownloader', '$http','$translate', '$uibModal',
    function (MysteryDownloader, $http, $translate, $uibModal) {
        var me = this;
        me.session_id = "unkwon";
        me.guid = '';
        me.username = '';
        me.mystery_instance = {};

        MysteryDownloader.get('SessionService/ID', function (data) {
            me.session_id = data;
        });

        MysteryDownloader.get('InstanceService/getInstance', function (data) {
            me.mystery_instance = data.output.content;
        });


    
        me.changeLanguage = function (langKey) {
            $translate.use(langKey);
        };

        return me;

}]);