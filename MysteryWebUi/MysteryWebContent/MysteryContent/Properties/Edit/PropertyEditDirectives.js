app.directive('mysteryPropertyEdit', function () {
    return {
        scope: {
            property_name: '=propertyName',
            content: '=content',
        },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/Edit/PropertyEdit.html'
    };
});
app.directive('mysterySingleReferenceEditValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/Edit/SingleReferenceEditCell.html'
    };
});
app.directive('mysteryStringEditValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/Edit/StringPropertyEditCell.html'
    };
});
app.directive('mysteryTextEditValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/Edit/TextPropertyEditCell.html'
    };
});
