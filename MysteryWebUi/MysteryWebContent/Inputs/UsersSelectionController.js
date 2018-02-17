app.directive('mysteryUserSelection', function () {
    return {
        scope: { selection: '=selection' },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/Inputs/UsersSelection.html'
    };
});

app.controller("UsersSelectionController", ['MysteryDownloader', '$scope', '$uibModal', function (MysteryDownloader, $scope, $uibModal) {
    var me = this;
    me.selected_contents = $scope.selection;// each on the form: { guid: 'xxxx', ReferenceText: 'the xxx' }
    if (angular.isUndefined(me.selected_contents))
        me.selected_contents = [];

    me.loading = true;
    me.typing_text = '';

    me.avalible = [];

    me.service_url = 'UserService/List';

    MysteryDownloader.get(me.service_url, function (data) {
        me.avalible = data.output;
    });

    me.removeSelection = function (to_remove_element) {
        //we want to edit the array in plance so we manipulate the array coming from the scope
        //to do so we need first to find the element index
        var to_remove_index = null;
        angular.forEach(me.selected_contents, function (value, index) {
            if (to_remove_element.guid == element.guid)
                to_remove_index == index;
        });
        if (to_remove_index != null)
            me.selected_contents.splice(index, 1);
    };

    me.onSelect = function (item) {
        if (me.selected_contents.filter(function (element) { return item.guid == element.guid }).length == 0) {
            me.selected_contents.push(item);
        }
        me.typing_text = '';
    }
    
}]);