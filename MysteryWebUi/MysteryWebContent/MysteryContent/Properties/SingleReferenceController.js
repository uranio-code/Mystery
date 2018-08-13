
app.controller("SingleReferenceController", ['MysteryDownloader', '$scope', '$uibModal', function (MysteryDownloader, $scope, $uibModal) {
    var me = this;
    me.original_selection = { guid: '', ReferenceText: '' };
    me.selected_content = { guid: '', ReferenceText: '' };
    me.loading = true;
    me.service_url = '';
    me.selected = false;

    me.avalible = [];
    me.cp = null;

    me.getSuggestionsUlr = function () {
        return 'ContentEditService/SingleReferenceSuggestions/' +
            me.cp.data.content.ContentType + '/' +
            me.cp.data.content.tiny_guid + '/' +
            me.cp.data.name;
    }

    me.loadDataFromPropertyControl = function () {
        if (me.cp.data.can_edit) {
            me.service_url = me.getSuggestionsUlr();

            MysteryDownloader.get(me.service_url, function (data) {
                me.avalible = data.output;
            });
        };
        if (me.cp.data.content[me.cp.data.name] !== null) {
            me.selected_content = me.cp.data.content[me.cp.data.name];
            me.selected = false;
            me.original_selection = me.selected_content;
        }
    }

    //anysync loading
    me.getAvalible = function (search_string) {
        if (search_string && angular.isString(search_string)) {
            return MysteryDownloader.getPromise(me.service_url + '?st=' + encodeURI(search_string), function (data) { return data.output; });
        }
        else {
            return MysteryDownloader.getPromise(me.service_url, function (data) { return data.output; });
        }
    }

    me.init = function (cp) {
        me.cp = cp;
        if (angular.isDefined(cp.data)) {
            me.loadDataFromPropertyControl();
        }
        else {
            //property controller have no data yet, in this cases the function
            //initWithContent will be called when the content is loaded
            //we add ourself to it
            var current_initWithContent = me.cp.initWithContent;
            me.cp.initWithContent = function (content, property_name) {
                current_initWithContent(content, property_name);
                me.loadDataFromPropertyControl();
            }

        }
        
        
    };


    me.removeSelection = function () {
        me.selected_content = { guid: '', ReferenceText: '' }
        me.selected = false;
        me.cp.current_value = null;
    };

    me.onSelect = function (item, model, label) {
        me.selected_content = item;
        me.selected = false;
        me.cp.current_value = { guid: me.selected_content.guid, ContentType: me.selected_content.ContentType };
    }

    me.confirmSelection = function () {
        if (me.selected_content === null) {
            me.selected_content = me.original_selection;
            me.cp.current_value = { guid: me.original_selection.guid, ContentType: me.original_selection.ContentType };
        }
        else{
            me.original_selection = me.selected_content;
        }
        me.cp.commit();
    }

    me.cancelSelection = function () {
        me.selected_content = me.original_selection;
        me.cp.cancel();
    }

}]);