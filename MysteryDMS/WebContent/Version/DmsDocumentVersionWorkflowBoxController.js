app.directive('dmsDocumentVersionWorkflowBox', function () {
    return {
        scope: { content: '=content' },
        restrict: 'E',
        templateUrl: 'DMS/Version/DmsDocumentVersionWorkflowBox.html'
    };
});

app.controller("DmsDocumentVersionWorkflowBoxController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location', 
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        me.version = $scope.content;
        me.versions = [me.version];
        me.active_version = null;
        me.max_previous = 3;
        me.previous = 0;
        me.load_more_previous = false;
        me.restart_from_previous = null;
        me.processRow = function (version) {
            if (version.active)
                me.active_version = version.guid;
            if (version === null)
                return;
            //we have just downloaded a version
            //we have 3 cases:
            //it is the version we are looking at
            //it is a previuos version
            //it is a later version.
            if (version.version_number >= me.version.version_number) {
                if (version.version_number > me.version.version_number)
                    me.versions.push(version);
                //'till we have more recent version we keep downloading
                if (angular.isDefined(version.next_version) && version.next_version !== null) {
                    MysteryDownloader.loadContent(version.next_version, me.processRow);
                }
                else if (angular.isDefined(me.version.previous_version) && me.version.previous_version !== null) {
                    MysteryDownloader.loadContent(me.version.previous_version, me.processRow);
                }
            }
            //older version we don't download all
            if (version.version_number < me.version.version_number) {
                me.versions.unshift(version);
                me.previous += 1;
                if (me.previous === me.max_previous) {
                    if (angular.isDefined(version.previous_version) && version.previous_version !== null) {
                        me.load_more_previous = true;
                        me.restart_from_previous = version;
                    }
                    return;
                }
                if (angular.isDefined(version.previous_version) && version.previous_version !== null) {
                    MysteryDownloader.loadContent(version.previous_version, me.processRow);
                }
            }
        }

        MysteryDownloader.loadContent(me.version, me.processRow);
        me.setActive = function (guid) {            
            me.posting = true;
                        
            MysteryDownloader.post(
                'DMSVersionService/SetActive/' + guid,
                function (data) {
                    me.posting = false;
                })
        }

        
        me.loadMorePrevious = function () {
            me.load_more_previous = false;
            if (me.restart_from_previous === null) {
                return;
            }
            me.previous = 0;
            MysteryDownloader.loadContent(me.restart_from_previous.previous_version, me.processRow);
        }
    }]);