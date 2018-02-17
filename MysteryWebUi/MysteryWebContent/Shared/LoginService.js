var MysteryLoginModalCreator = function ($uibModal) {
    return $uibModal.open({
        animation: true,
        templateUrl: 'MysteryWebContent/Login/Login.html',
        controller: 'mysteryLoginController as c',
        backdrop: false,
        keyboard: false
    });
};

app.factory("MysteryLogin", ['$http', '$uibModal', function ($http, $uibModal) {
    var me = this;
    me.modalInstance = null;

    me.showModal = function (login_call_back) {
        me.modalInstance = MysteryLoginModalCreator($uibModal);
        me.modalInstance.login_call_back = function () {
            login_call_back();
            me.modalInstance = null;
        };
    };

    me.login = function (login_call_back) {
        if (me.modalInstance === null) {
            me.showModal(login_call_back);
        }
        else {
            var current_call_back = me.modalInstance.login_call_back;
            me.modalInstance.login_call_back = function () {
                login_call_back();
                current_call_back();
            };
        }
    };

    return me;
}]);