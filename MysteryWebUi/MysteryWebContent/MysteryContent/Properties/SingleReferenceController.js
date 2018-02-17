
app.controller("SingleReferenceController", ['MysteryDownloader', '$scope', '$uibModal', function (MysteryDownloader, $scope, $uibModal) {
    var me = this;
    me.originalUser = { guid: '', ReferenceText: '' };
    me.selected_content = { guid: '', ReferenceText: '' };
    me.loading = true;
    me.service_url = '';
    me.selected = false;

    me.avalible = [];
    me.cp = null;

    me.init = function (cp) {
        me.cp = cp;
        if (cp.data.can_edit) {
            me.service_url =
            'ContentEditService/SingleReferenceSuggestions/' +
            cp.data.content.ContentType + '/' +
            cp.data.content.tiny_guid + '/' +
            cp.data.name;
            MysteryDownloader.get(me.service_url, function (data) {
                me.avalible = data.output;
            });
        };
        if (cp.data.content[cp.data.name] !==null)
        {
            me.selected_content = cp.data.content[cp.data.name];
            me.selected = false;
            me.originalUser = me.selected_content;
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
        if (me.selected_content == null) {
            me.selected_content = me.originalUser;
            me.cp.current_value = { guid: me.originalUser.guid, ContentType: me.originalUser.ContentType };
        }
        else{
            me.originalUser = me.selected_content;
        }
        me.cp.commit();
    }

    me.cancelSelection = function () {
        me.selected_content = me.originalUser;
        me.cp.cancel();
    }

}]);