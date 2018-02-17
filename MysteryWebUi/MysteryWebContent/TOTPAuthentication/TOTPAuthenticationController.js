app.controller("TOTPAuthenticationController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location', '$rootScope', '$q',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location, $rootScope, $q) {
        var me = this;
        me.loading = true;
        me.code = '';
        me.check_done = false;
        MysteryDownloader.get('UserService/TOTPAuthenticationKey', function (data) {
            me.loading = false;
            me.image_url = "https://chart.googleapis.com/chart?chs=100x100&chld=M%7C0&cht=qr&chl=otpauth%3A%2F%2Ftotp%2F" + data.output.instance_name + "%3A" + data.output.upn + "%3Fsecret%3D" + data.output.key;
        });
        me.submit = function () {
            MysteryDownloader.post('UserService/TOTPAuthenticationCode', { code: me.code }, function (data) {
                me.check_done = true;
                me.check_result = data.output;
            });
        };
    }]);