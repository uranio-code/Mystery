app.controller("mysteryContentDeletedController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        me.data = $scope.entry;

        //something odd is going on with translate here
        // if we leave it in the html template I hit a recursion limit eccess
        //this is probaly due the translation service wathing all the data used in templates
        me.who = MysteryDownloader.loadContent({ guid: me.data.history_message_data.who, ContentType: 'User' });
        me.what = MysteryDownloader.loadContent(me.data.history_message_data.what);
            
        me.message = "";

        //we display the message only after both who and what are loaded
        MysteryDownloader.loadContents([me.who, me.what], function () {
            me.message = $translate.instant("COMMON.DELETED_LOG", { who: me.who, what: me.what })
        });


    }]);