app.controller("dmsVersionCreatedHistoryEntryController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        me.data = $scope.entry;

        //something odd is going on with translate here
        // if we leave it in the html template I hit a recursion limit eccess
        //this is probaly due the translation service wathing all the data used in templates
        me.loaded = { who: false, what: false };
        me.who = null;
        me.message = "";

        me.onLoaded = function (element_name) {
            me.loaded[element_name] = true;
            if (me.loaded.who && me.loaded.what) {
                me.message = $translate.instant("DMS.VERSION.VERSION_CREATED_LOG", { who: me.who, what: me.data.history_message_data.what })
            }
        }

        me.who = MysteryDownloader.loadContent({ guid: me.data.history_message_data.who, ContentType: 'User' },
            function (content) { me.onLoaded("who"); });
        me.what = MysteryDownloader.loadContent(me.data.history_message_data.what,
            function (content) { me.onLoaded("what"); });


    }]);