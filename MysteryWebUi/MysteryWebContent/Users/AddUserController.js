app.controller("AddUserDialogController", ['MysteryDownloader', '$window', '$translate', '$uibModalInstance', '$location',
    function (MysteryDownloader, $window, $translate, $uibModalInstance, $location) {
        var me = this;

        me.name = "";
        me.email = "";
        me.password = "";
        
        me.name_placeholder = $translate.instant('USER.FULLNAME');
        me.email_placeholder = $translate.instant('COMMON.EMAIL');
        me.password_placeholder = $translate.instant('COMMON.PASSWORD');

        me.addUser = function () {
            MysteryDownloader.post(
                'AddUser',
                { fullname: me.name, email: me.email, password: me.password },
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
                });

            $uibModalInstance.close();
        };

        me.cancel = function () {
            $uibModalInstance.close();
        }
        
        
    }
]);


app.controller("AddUserController", ['MysteryDownloader', '$window', '$translate', '$uibModal',
    function (MysteryDownloader, $window, $translate, $uibModal) {
        var me = this;
        me.showAddModal = function () {
            $uibModal.open({
                animation: true,
                templateUrl: 'MysteryWebContent/Users/AddUser.html',
                controller: 'AddUserDialogController as c',
            });
        };

        me.user = MysteryDownloader.user;
    }]);