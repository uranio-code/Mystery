app.controller("ContentPropertyEditController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location', 'MysteryFormService',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location, MysteryFormService) {
        var me = this;
        me.content = $scope.content;
        me.property_name = $scope.property_name;

        me.init = function (property_data) {
            me.data = property_data;
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
        };

        if (angular.isDefined(me.content) && angular.isDefined(me.property_name)) {
            MysteryDownloader.loadContent(me.content, function (content) {
                if (angular.isUndefined(content.propertiesUi))
                    return;
                me.init(content.propertiesUi[me.property_name]);
            });
        }

        me.cancel = function () {
            me.current_value = me.data.content[me.data.name];
        };

        me.adjustTextareaHeight = function (e, id) {
            var textarea = document.getElementById(id);
            textarea.style.setProperty('height', e.target.offsetHeight + 'px');
            textarea.style.setProperty('overflow', 'auto');
            textarea.style.setProperty('resize', 'vertical');
        };

        me.adjustTextareaHeightDynamic = function (id) {
            var textarea = document.getElementById(id);
            var resize = function (element) {
                element.style.setProperty('height', textarea.scrollHeight + "px");
            };
            textarea.addEventListener("keyup", resize(textarea), false);
            textarea.addEventListener("keydown", resize(textarea), false);
        };

        me.getTemplateUrl = function (view_template_url) {
            if (view_template_url === 'MysteryWebContent/MysteryContent/Properties/SingleReference.html')
                return 'MysteryWebContent/MysteryContent/Properties/Edit/SingleReferenceEdit.html';
            if (view_template_url === 'MysteryWebContent/MysteryContent/Properties/StringProperty.html')
                return 'MysteryWebContent/MysteryContent/Properties/Edit/StringPropertyEdit.html';
            if (view_template_url === 'MysteryWebContent/MysteryContent/Properties/TextProperty.html')
                return 'MysteryWebContent/MysteryContent/Properties/Edit/TextPropertyEdit.html';
            return view_template_url;
        };
    }
]);