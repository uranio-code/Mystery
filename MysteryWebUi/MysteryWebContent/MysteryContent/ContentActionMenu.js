app.directive('mysteryContentActionMenu', function () {
    return {
        scope: { content: '=content'},
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/ContentActionMenu.html'
    };
});
app.directive('mysteryContentWorkflowActionMenu', function () {
    return {
        scope: { content: '=content' },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/ContentWorkflowActionMenu.html'
    };
});
app.directive('mysteryContentActionButton', function () {
    return {
        scope: { content: '=content', action_name: '=name', action: '=action'},
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/ContentActionButton.html'
    };
});
app.directive('mysteryContentWorkflowActionButton', function () {
    return {
        scope: { content: '=content', action_name: '=name', action: '=action' },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/ContentWorkflowActionButton.html'
    };
});
app.directive('mysteryContentListAction', function () {
    return {
        scope: { content: '=content' },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/ContentListAction.html'
        
    };
});


app.controller("ActionResultModalController", ['$translate', '$uibModalInstance','$location',
    function ($translate, $uibModalInstance, $location) {
        var me = this;
        me.message = $translate.instant($uibModalInstance.result.message);
        var next_url = $uibModalInstance.result.next_url.url;
        me.okClick = function () {
            $uibModalInstance.close();
            $location.path(next_url);
        };
}]);

app.controller("MysteryContentActionMenuController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location','$rootScope',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location, $rootScope) {
        var me = this;
        me.actions = [];
        me.loading = true;
        me.content = $scope.content;
        me.getActions = function () {
            MysteryDownloader.get('ContentService/' + me.content.ContentType + '/' + me.content.tiny_guid + '/actions', function (data) {
                me.actions = data.output;
                me.loading = false;
            });
        };
        me.getActions();
        $rootScope.$on("changed_content", function (event, data) {
            if (angular.isUndefined(data) || angular.isUndefined(data.guid))
                return;
            if (me.content.guid === data.guid) {
                me.getActions();
            }
        });
    
        

    }]);

app.controller("MysteryContentWorkflowActionMenuController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location', '$rootScope',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location, $rootScope) {
        var me = this;
        me.actions = [];
        me.loading = true;
        me.content = $scope.content;
        me.getActions = function () {
            MysteryDownloader.get('ContentService/' + me.content.ContentType + '/' + me.content.tiny_guid + '/workflow_actions', function (data) {
                me.actions = data.output;
                me.loading = false;
            });
        };
        me.getActions();

        $rootScope.$on("changed_content", function (event, data) {
            if (angular.isUndefined(data) || angular.isUndefined(data.guid))
                return;
            if (me.content.guid === data.guid) {
                me.getActions();

            }
        });
    }
]);

app.controller("MysteryContentActionListController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location', '$rootScope',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location, $rootScope) {
        var me = this;
        me.actions = [];
        me.loading = true;
        me.content = $scope.content;
        me.getActions = function () {
            MysteryDownloader.get('ContentService/' + me.content.ContentType + '/' + me.content.tiny_guid + '/list-actions', function (data) {
                me.actions = data.output;
                me.loading = false;
            });
        };
        me.getActions();
        $rootScope.$on("changed_content", function (event, data) {
            if (angular.isUndefined(data) || angular.isUndefined(data.guid))
                return;
            if (me.content.guid === data.guid) {
                me.getActions();
            }
        });
    }]);

app.controller("ContentWorkflowActionButtonController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        me.content = $scope.content;
        me.action_name = $scope.action_name;
        me.logged_user = MysteryDownloader.user;

        //we have 2 ways to instance this control, by giving directly the action
        //from $scope.action or by passing it with $scope.action name
        if (angular.isUndefined($scope.action)) {
            //we have on action data given only its name, we shall for it it
            me.data = {};
            me.loading = true;
            MysteryDownloader.get('ContentService/' + me.content.ContentType + '/' + me.content.tiny_guid + '/workflow-action-info/' + me.action_name, function (data) {
                me.data = data.output;
                me.loading = false;
            });
        }
        else {
            //we have the action data object, we are good to go
            me.data = $scope.action;
            me.loading = false;
            me.action_name = me.data.name;
        }

        me.checkWorkflowVisibility = function () {

            return me.data.authorized;

        };

        me.click = function () {

            //all right, are we a ready to fire or do we display a modal?
            if (angular.isUndefined(me.data.modal) || me.data.modal === null) {
                //fire!
                MysteryDownloader.post('ContentService/ExecuteWorkflowAction', {
                    guid: me.content.tiny_guid,
                    content_type_name: me.content.ContentType,
                    action_name: me.action_name
                },
                    function (data) {
                        if (data.output !== null) {
                            //something to say to the user?
                            if (data.output.message !== null && data.output.message.length > 0) {
                                var modalInstance = $uibModal.open({
                                    animation: true,
                                    templateUrl: 'MysteryWebContent/MysteryContent/Actions/ActionResultModal.html',
                                    controller: 'ActionResultModalController as c',
                                    backdrop: false,
                                    keyboard: false
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
            }
            else {

                //we got a serious action here!
                //let's open a the modal and download the whole content in it
                var modalInstance = $uibModal.open(me.data.modal);
                modalInstance.content = MysteryDownloader.loadContent(me.content);
                modalInstance.action = me.data;


            }
        };
    }]);

app.controller("ContentActionButtonController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        me.content = $scope.content;
        me.action_name = $scope.action_name;
        me.logged_user = MysteryDownloader.user;

        //we have 2 ways to instance this control, by giving directly the action
        //from $scope.action or by passing it with $scope.action name
        if (angular.isUndefined($scope.action))
        {
            //we have on action data given only its name, we shall for it it
            me.data = {};
            me.loading = true;
            MysteryDownloader.get('ContentService/' + me.content.ContentType + '/' + me.content.tiny_guid + '/action-info/' + me.action_name, function (data) {
                me.data = data.output;
                me.loading = false;
            });
        }
        else
        {
            //we have the action data object, we are good to go
            me.data = $scope.action;
            me.loading = false;
            me.action_name = me.data.name;
        }

        me.checkWorkflowVisibility = function () {

            return me.data.authorized;
            
        };

        me.click = function () {

            //all right, are we a ready to fire or do we display a modal?
            if (angular.isUndefined(me.data.modal) || me.data.modal === null) {
                //fire!
                MysteryDownloader.post('ContentService/ExecuteAction', {
                    guid: me.content.tiny_guid,
                    content_type_name: me.content.ContentType,
                    action_name: me.action_name
                },
                    function (data) {
                        if (data.output !== null) {
                            //something to say to the user?
                            if (data.output.message !== null && data.output.message.length > 0) {
                                var modalInstance = $uibModal.open({
                                    animation: true,
                                    templateUrl: 'MysteryWebContent/MysteryContent/Actions/ActionResultModal.html',
                                    controller: 'ActionResultModalController as c',
                                    backdrop: false,
                                    keyboard: false
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
            }
            else {

                //we got a serious action here!
                //let's open a the modal and download the whole content in it
                var modalInstance = $uibModal.open(me.data.modal);
                modalInstance.content = MysteryDownloader.loadContent(me.content);
                modalInstance.action = me.data;
                

            }
        };
    }]);