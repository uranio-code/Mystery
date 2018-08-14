app.controller("ContentViewController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        var path_array = $location.path().split("/");
        me.content = {
            ContentType: path_array[path_array.length - 2],
            tiny_guid: path_array[path_array.length - 1],
            authorized: true, //this will became false if we are not authorized after the download
        };

        me.content = MysteryDownloader.loadContent(me.content);
    }
]);

app.directive('mysteryPropertyView', function () {
    return {
        scope: {
            property_name: '=propertyName',
            content: '=content',
        },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/PropertyView.html'
    };
});

app.controller("ContentPropertyViewController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location', 'MysteryFormService',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location, MysteryFormService) {
        var me = this;
        me.content = $scope.content;
        me.property_name = $scope.property_name;

        me.init = function (property_data) {
            me.data = property_data;
            me.mouseover = false;
            me.editing = false;
            me.input_focus = false;
            me.current_value = me.data.content[me.data.name]
        }

        me.initWithContent = function (content, property_name) {
            me.content = content;
            me.property_name = property_name;
            MysteryDownloader.loadContent(me.content, function (content) {
                if (angular.isUndefined(content.propertiesUi))
                    return;
                me.init(content.propertiesUi[me.property_name]);
            });
        }

        if (angular.isDefined(me.content) && angular.isDefined(me.property_name)) {
            MysteryDownloader.loadContent(me.content, function (content) {
                if (angular.isUndefined(content.propertiesUi))
                    return;
                me.init(content.propertiesUi[me.property_name]);
            });
        }


        me.showEdit = function () {
            if (angular.isUndefined(me.data) || angular.isUndefined(me.data.can_edit))
                return false;
            return me.data.can_edit && me.editing;
        }

        me.exitEditMode = function () {
            me.editing = false;
            //even if the mouse is over the control we want act as it is no longer
            me.mouseover = false;
            me.input_focus = false;
            MysteryFormService.deactivate(me);

        }

        me.enterEditMode = function () {
            me.editing = true;
            me.input_focus = true;
            MysteryFormService.activate(me);
        }

        //to be used when the property control is a sigle form
        //do not use for a form with multiple field or each one will be posted and applying only the last value in the interface
        me.commit = function () {
            me.posting = true;
            var input = {
                guid: me.data.content.tiny_guid,
                ContentType: me.data.content.ContentType,
            };
            input[me.data.name] = me.current_value;
            MysteryDownloader.post('PropertyEditAction', input, function (data) {
                me.posting = false;
                me.exitEditMode();
            });
        }

        //to be called whe you have multiple property control in 1 form
        //to apply the selected value to the content
        me.setContentPropertyValue = function () {
            me.content[me.data.name] = me.current_value;
        }

        me.cancel = function () {
            me.current_value = me.data.content[me.data.name];
            me.exitEditMode();
        }

        me.adjustTextareaHeight = function (e, id) {
            var textarea = document.getElementById(id);
            textarea.style.setProperty('height', e.target.offsetHeight + 'px');
            textarea.style.setProperty('overflow', 'auto');
            textarea.style.setProperty('resize', 'vertical');
        }

        me.adjustTextareaHeightDynamic = function (id) {
            var textarea = document.getElementById(id);
            var resize = function (element) {
                element.style.setProperty('height', textarea.scrollHeight + "px");
            };
            textarea.addEventListener("keyup", resize(textarea), false);
            textarea.addEventListener("keydown", resize(textarea), false);
        }
    }
]);