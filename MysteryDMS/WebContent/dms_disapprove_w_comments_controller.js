app.controller("dms_disapprove_w_comments_controller",
    ['MysteryDownloader', '$scope', '$uibModalInstance',
        function (MysteryDownloader, $scope, $uibModalInstance) {
            var me = this;
            me.comment_body = '';
            me.id = 'comment_textarea';
            me.content = $uibModalInstance.content;
            me.posting = false;

            me.calling_controller = $uibModalInstance.calling_controller;
            
            me.cancel = function () {
                $uibModalInstance.close();
            };

            me.commit = function () {
                if (me.posting)
                    return;
                me.posting = true;
                MysteryDownloader.post('DMSDisapprove_with_comments',
                    { dms_version_guid: me.content.tiny_guid, comment_text: me.comment_body },
                    function (data) {
                        me.posting = false;
                        $uibModalInstance.close();
                    });
            };
             
            me.adjustTextareaHeightDynamic = function (comment_body) {
                var textarea = document.getElementById(me.id);
                var resize = function (element) {
                    element.style.setProperty('height', textarea.scrollHeight + "px");
                };
                textarea.addEventListener("keyup", resize(textarea), false);
                textarea.addEventListener("keydown", resize(textarea), false);
            }
        }
    ]
);

