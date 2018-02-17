app.controller("deleteContentController", ['MysteryDownloader', '$window', '$translate', '$uibModalInstance','$location',
function (MysteryDownloader, $window, $translate, $uibModalInstance,$location) {
        var me = this;
        me.content = $uibModalInstance.content;
        me.action = $uibModalInstance.action;
        if (angular.isDefined($uibModalInstance.go_away_after_deletion))
            me.go_away_after_deletion = $uibModalInstance.go_away_after_deletion;
        else
            me.go_away_after_deletion = true;


        me.message = $translate.instant('COMMON.DELETE_MESSAGE', { title: me.content.ReferenceText });

        me.cancelClick = function () {
            $uibModalInstance.close();
        };

        me.okClick = function () {
            MysteryDownloader.post(
                'ContentService/ExecuteAction', {
                    guid: me.content.tiny_guid,
                    content_type_name: me.content.ContentType,
                    action_name: 'delete'
                },
                function (data) {
                    if (me.go_away_after_deletion)
                        $location.path(data.output.next_url.url);
                    $uibModalInstance.close();
                })
        };

    }]);