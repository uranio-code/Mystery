app.controller("DMSAddUserGroupDialogController", ['MysteryDownloader', '$window', '$translate', '$uibModalInstance', '$location',
    function (MysteryDownloader, $window, $translate, $uibModalInstance, $location) {
        var me = this;

        me.fullname = "";
        
        me.fullname_placeholder = $translate.instant('DMS.USERGROUPS.TITLE');
        
        me.addUserGroup = function () {
            MysteryDownloader.post(
                'DMSAddUserGroup',
                { fullname: me.fullname },
                function (data) {
                    if (data.output !== null) {
                        //something to say to the user?
                        if (data.output.message !== null && data.output.message.length > 0) {
                            var modalInstance = $uibModal.open({
                                animation: true,
                                templateUrl: 'MysteryWebContent/MysteryContent/Actions/ActionResultModal.html',
                                controller: 'ActionResultModalController as c',
                                backdrop: false,
                                keyboard: false,
                            });
                            modalInstance.result = data.output;
                        }
                        else {
                            //ok nothing to say, let'go!
                            if (data.output.next_url !== null)
                                $location.path(data.output.next_url.url);
                        }
                    }
                })

            $uibModalInstance.close();
        }

        me.cancel = function () {
            $uibModalInstance.close();
        }
    }
]);


app.controller("DMSAddUserGroupController", ['MysteryDownloader', '$window', '$translate', '$uibModal',
    function (MysteryDownloader, $window, $translate, $uibModal) {
        var me = this;
        me.showAddModal = function () {
            $uibModal.open({
                animation: true,
                templateUrl: 'DMS/DMSAddUserGroup.html',
                controller: 'DMSAddUserGroupDialogController as c',
            });
        }
    }]);