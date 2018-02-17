app.directive('mysteryLogin', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/Login/Login.html'
    };
});
var loginCallBack = function () {
    console.warn("nothing to do");
};
app.controller("mysteryLoginController", ['MysteryDownloader', '$window', '$translate', '$uibModalInstance', '$cookies',
    function (MysteryDownloader, $window, $translate, $uibModalInstance, $cookies) {
        var me = this;
        me.email = angular.isDefined($cookies.get("Loging.email"))?String($cookies.get("Loging.email")):"";
        me.password = angular.isDefined($cookies.get("Loging.password"))?String($cookies.get("Loging.password")):"";
        me.remeberMe = angular.isDefined($cookies.get("Loging.REMEMBER_ME"));
        
        me.email_placeholder = $translate.instant('COMMON.EMAIL');
        me.password_placeholder = $translate.instant('COMMON.PASSWORD');
        me.RememberMeLabel = $translate.instant('COMMON.REMEMBER_ME');
        
        function popup(url) {
            var width = 525,
                height = 525,
                screenX = window.screenX,
                screenY = window.screenY,
                outerWidth = window.outerWidth,
                outerHeight = window.outerHeight;

            var left = screenX + Math.max(outerWidth - width, 0) / 2;
            var top = screenY + Math.max(outerHeight - height, 0) / 2;

            var features = [
                        "width=" + width,
                        "height=" + height,
                        "top=" + top,
                        "left=" + left,
                        "status=no",
                        "resizable=yes",
                        "toolbar=no",
                        "menubar=no",
                        "scrollbars=yes"];
            var popup = window.open(url, "oauth", features.join(","));
            if (!popup) {
                console.warn("failed to pop up auth window");
            }

            popup.focus();
        }

        me.loginLiveId = function () {
            MysteryDownloader.get('InstanceService/getLiveIDConfiguration', function (appInfo) {
                var url =
                  "https://login.microsoftonline.com/common/oauth2/v2.0/authorize" +
                  "?client_id=" + appInfo.clientId +
                  "&tenant=common" +
                  "&response_mode=query" +
                  "&scope=" + encodeURIComponent(appInfo.scopes) +
                  "&response_type=code" +
                  "&redirect_uri=" + encodeURIComponent(appInfo.redirectUri);
                loginCallBack = function () {
                    $uibModalInstance.login_call_back();
                    $uibModalInstance.close();
                };
                popup(url);
            });

        };


        me.login = function () {
            if (me.remeberMe)
            {
                $cookies.put("Loging.email", me.email);
                $cookies.put("Loging.password", me.password);
                $cookies.put("Loging.REMEMBER_ME", me.remeberMe);
            }
            else
            {
                $cookies.put("Loging.email", "");
                $cookies.put("Loging.password", "");
                $cookies.remove("Loging.REMEMBER_ME");
            };
            

            MysteryDownloader.post('SessionService/login', { email: me.email, password: me.password },
                function (data) {
                    $uibModalInstance.login_call_back();
                    $uibModalInstance.close();
                });
        };

    }
]);