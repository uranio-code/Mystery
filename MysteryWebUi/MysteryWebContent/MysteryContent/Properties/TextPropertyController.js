app.controller("mysteryTextPropertyModal",
    ['MysteryDownloader', '$scope', '$uibModalInstance',
        function (MysteryDownloader, $scope, $uibModalInstance) {
            var me = this;
            me.cp = $uibModalInstance.cp;
            me.cp.current_value = me.cp.data.content[me.cp.data.name];
            me.calling_controller = $uibModalInstance.calling_controller;
            me.modal_title = me.cp.data.content.ReferenceText + " " + me.cp.data.label;

            me.cancel = function () {
                me.calling_controller.cancel();
                $uibModalInstance.close();
            };

            me.commit = function () {
                me.cp.commit();
                $uibModalInstance.close();
            };
        }
    ]
);

app.controller("mysteryTextPropertyController",
    ['MysteryDownloader', '$scope', '$uibModal',
        function (MysteryDownloader, $scope, $uibModal) {
            var me = this;
            me.cp = null;

            me.init = function (cp) {
                me.cp = cp;
            };

            me.showModal = function () {
                var modal_instance = $uibModal.open({
                    animation: true,
                    templateUrl: 'MysteryWebContent/MysteryContent/Properties/TextPropertyModal.html',
                    controller: 'mysteryTextPropertyModal as c',
                    backdrop: false,
                    keyboard: false
                });
                modal_instance.cp = me.cp;
                modal_instance.calling_controller = me;
                return modal_instance;
            };

            
            me.cancel = function () {
                me.cp.cancel();
                me.init(me.cp);
            };

            me.enterEditMode = me.showModal;

        }]);