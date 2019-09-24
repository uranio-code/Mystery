app.directive('mysteryNavigation', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/Shared/NavBar/NavBar.html'
    };
});

app.controller("NavBarController", [
    '$scope', 'MysteryDownloader', '$translate', '$uibModal', 'MysteryInstance', '$q', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, MysteryInstance, $q, $location) {

        var me = this;
        me.mystery_instance = {};
        me.search_text = '';
        me.logo_path = '';
        me.user = '';

        MysteryDownloader.get('InstanceService/getInstance', function (data) {
            me.mystery_instance = data.output;
            if (angular.isDefined(me.mystery_instance.logo) && me.mystery_instance.logo!==null)
                me.logo_path = 'InstanceService/getInstanceLogo';
            else
                me.logo_path = 'MysteryWebContent/Shared/NavBar/clomax-transparent.png';
        });

        me.downloader = MysteryDownloader;
     
        me.login = function () {
            MysteryDownloader.get('SessionService/LoggedEcho', function (data) {
            });
            me.user = MysteryDownloader.user;
        };

        me.logout = MysteryDownloader.logout;

        me.user = MysteryDownloader.user;

        me.applications = [];
        MysteryDownloader.get('ApplicationsService/getApplications', function (data) {
            var result = [];
            //we add null elements to have dividers
            for (var e in data.output) {
                var app = data.output[e];
                if(!app.active)
                    continue;
                result = result.concat([data.output[e], null]);
            }
            //at least until we don't have the more
            result.pop();
            me.applications = result;
        });

        me.search = function (search_text) {
            var deferred = $q.defer();

            MysteryDownloader.post(
                'InstanceService/Search',
                { text: search_text, max_result: 10 },
                function (data) {
                    deferred.resolve(data.output);
                }
                );

            return deferred.promise;
        };

        me.onSelect = function (item) {
            $location.path(item.url);
        }
    }
]);