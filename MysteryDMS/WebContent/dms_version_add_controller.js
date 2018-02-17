app.controller("dms_add_version_modal_controller",
    ['MysteryDownloader', '$scope', '$uibModalInstance',
        function (MysteryDownloader, $scope, $uibModalInstance) {
            var me = this;
            me.folder = $uibModalInstance.content;
            me.posting = false;

            me.calling_controller = $uibModalInstance.calling_controller;
            
            me.cancel = function () {
                $uibModalInstance.close();
            };

            me.commit = function () {
                if (me.posting)
                    return;
                me.posting = true;
                //MysteryDownloader.post('DMSAddCommentAction',
                //    { dms_version_guid: me.content.tiny_guid, comment_text: me.comment_body },
                //    function (data) {
                //        me.posting = false;
                //        $uibModalInstance.close();
                //    });
            };
                
        }
    ]
);

