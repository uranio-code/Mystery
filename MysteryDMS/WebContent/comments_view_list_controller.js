app.directive('dmsVersionCommentsViewList', function () {
    return {
        scope: { content: '=content' },
        restrict: 'E',
        templateUrl: 'DMS/comments_view_list.html'
    };
});

app.controller("dms_version_comments_view_list_controller", ['$scope', 'MysteryDownloader', '$translate', '$uibModal','$rootScope', 
    function ($scope, MysteryDownloader, $translate, $uibModal, $rootScope) {
        var me = this;
        //let's get the content, we might need it
        me.content = $scope.content;
        me.comments = [];
        me.comment_body = '';
        me.posting = false;

        me.commit = function () {
            if (me.comment_body === '') {
                return;
            } else {
                if (me.posting)
                    return;
                me.posting = true;
                MysteryDownloader.post('DMSAddCommentAction',
                    { dms_version_guid: me.content.tiny_guid, comment_text: me.comment_body },
                    function (data) {
                        me.posting = false;
                    });
                me.comment_body = '';
            }
        };

        MysteryDownloader.post(
            'DMSGetComments', { tiny_guid: me.content.tiny_guid, ContentType: me.content.ContentType },
            function (data) {
                if (angular.isDefined(data.output) && data.output!== null) {
                    me.comments = data.output.contents;
                }
            });

        //when a version get a new comment we show it
        $rootScope.$on("new_content", function (event, data) {
            //content?
            //if (angular.isUndefined(data) || angular.isUndefined(data.guid))
            //    return;
            //DMSComment?
            if (angular.isUndefined(data.ContentType) || data.ContentType !== 'DMSComment')
                return;
            //for this version?
            if (angular.isUndefined(data.parent_dms_content) || angular.isUndefined(data.parent_dms_content.guid))
                return;
            if (data.parent_dms_content.guid !== me.content.guid)
                return;
            //our!
            me.comments.unshift(data);
        });

        me.adjustTextareaHeight = function (id) {
            var textarea = document.getElementById(id);
            textarea.style.setProperty('height', "32px");
        };

        me.adjustTextareaHeightDynamic = function (id) {
            var textarea = document.getElementById(id);
            var resize = function (element) {
                element.style.setProperty('height', textarea.scrollHeight + "px");
            };
            textarea.addEventListener("keyup", resize(textarea), false);
            textarea.addEventListener("keydown", resize(textarea), false);
        };
    }
]);