app.directive('mysteryShareContent', function () {
    return {
        scope: { content: '=content' },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Actions/ShareContentButton.html'
    };
});

app.controller("ShareContentModalController",
    ['MysteryDownloader', '$scope', '$uibModalInstance',
        function (MysteryDownloader, $scope, $uibModalInstance) {
            var me = this;
            me.email_body = '';
            me.id = 'share_textarea';
            me.content = $uibModalInstance.content;
            me.posting = false;
            me.receivers = [];
            me.cancel = function () {
                $uibModalInstance.close();
            };

            me.send = function () {
                if (me.posting)
                    return;
                me.posting = true;
                var post_input = {
                    content_tiny_guid: me.content.tiny_guid,
                    content_type: me.content.ContentType,
                    receivers: me.receivers.map(function (element) { return element.tiny_guid }),
                    email_body: me.email_body,
                };
                MysteryDownloader.post('ShareContentAction', post_input,
                    function (data) {
                        me.posting = false;
                        $uibModalInstance.close();
                    });
            };

            me.adjustTextareaHeightDynamic = function (email_body) {
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

app.controller("ShareContentButtonController",
    ['MysteryDownloader', '$scope', '$uibModal',
        function (MysteryDownloader, $scope, $uibModal) {
            var me = this;
            me.content = $scope.content;
            var modal_options = {
                animation : true,
                templateUrl: 'MysteryWebContent/MysteryContent/Actions/ShareContentModal.html',
                controller: 'ShareContentModalController as c',
            };
            me.click = function () {
                var modalInstance = $uibModal.open(modal_options);
                modalInstance.content = MysteryDownloader.loadContent(me.content);
            };
        }
    ]
);

app.controller("ShareContentLogController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        me.data = $scope.entry;

        me.message = $translate.instant("COMMON.SHARED_LOG",
            {
                from: me.data.history_message_data.from,
                to: me.data.history_message_data.to.map(function (x) { return x.ReferenceText; }).join(),
            });

    }]);

