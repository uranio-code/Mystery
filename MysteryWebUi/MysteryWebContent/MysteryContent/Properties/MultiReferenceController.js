
app.controller("MultiReferenceController", ['MysteryDownloader', '$scope', '$uibModal', function (MysteryDownloader, $scope, $uibModal) {
    var me = this;
    me.original_contents = [];
    me.selected_contents = [];// each on the form: { guid: 'xxxx', ReferenceText: 'the xxx' }
    me.loading = true;
    me.service_url = '';
    me.typing_text = '';
    me.avalible = [];
    me.cp = null;

    me.init = function (cp) {
        me.cp = cp;
        me.service_url = 
            'ContentEditService/MultiReferenceSuggestions/' +
            cp.data.content.ContentType + '/' +
            cp.data.content.tiny_guid + '/'+
            cp.data.name;
        MysteryDownloader.get(me.service_url, function (data) {
            me.avalible = data.output;
        });
        if (cp.data.content[cp.data.name] !== null && cp.data.content[cp.data.name].length > 0) {
            me.selected_contents = cp.data.content[cp.data.name];
            me.original_contents = cp.data.content[cp.data.name];
        }
        else {
            me.selected_contents = [];
        }     
    };

    me.removeSelection = function (to_remove_element) {
        me.selected_contents = me.selected_contents.filter(function (element) { return to_remove_element.guid !== element.guid; });
        me.cp.current_value = me.selected_contents;
    };

    me.removeAllItems = function () {
        me.cp.current_value = [];
        me.selected_contents = [];
    }

    me.onSelect = function (item) {
        if (me.selected_contents.filter(function (element) { return item.guid === element.guid; }).length === 0) {
            me.selected_contents.push({ guid: item.guid, ContentType: item.ContentType, ReferenceText: item.ReferenceText });
            me.original_contents = me.original_contents.filter(function (element) { return item.guid != element.guid });
        }
        me.typing_text = '';
        me.cp.current_value = me.selected_contents;
    }
    
    me.confirmAllSelections = function () {
        if (me.selected_contents == null) {
            me.selected_contents = me.original_contents;
            me.cp.current_value = me.selected_contents;
        }
        else {
            me.original_contents = me.selected_contents;
        }
        me.cp.commit();
    };

    me.cancelSelection = function () {
        me.selected_contents = me.original_contents;
        me.cp.current_value = me.original_contents;
        me.cp.cancel();
    }

}]);