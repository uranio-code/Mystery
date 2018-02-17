app.directive('mysteryBooleanValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/BooleanPropertyCell.html'
    };
});
app.directive('mysteryDateValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/DatePropertyCell.html'
    };
});
app.directive('mysteryDoubleValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/DoublePropertyCell.html'
    };
});
app.directive('mysteryLongValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/LongPropertyCell.html'
    };
});
app.directive('mysteryMultiReferenceValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/MultiReferenceCell.html'
    };
});
app.directive('mysterySingleFileValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/SingleFileCell.html'
    };
});
app.directive('mysterySingleReferenceValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/SingleReferenceCell.html'
    };
});
app.directive('mysteryStringValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/StringPropertyCell.html'
    };
});
app.directive('mysteryTextValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/TextPropertyCell.html'
    };
});
app.directive('mysteryTextNonModalValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/TextPropertyNonModalCell.html'
    };
});
app.directive('mysteryDropDownValue', function () {
    return {
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/Properties/DropDownPropertyCell.html'
    };
});