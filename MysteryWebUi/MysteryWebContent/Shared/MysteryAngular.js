

//If you wish to debug an application with this information then you should open up a debug console in the browser 
//then call this method directly in this console:

//angular.reloadWithDebugInfo();

app.config(['$compileProvider', function ($compileProvider) {
    $compileProvider.debugInfoEnabled(false);
    //strange it should work but it crashes... bu
    //$compileProvider.commentDirectivesEnabled(false);
    //$compileProvider.cssClassDirectivesEnabled(false);
}]);



app.config(['$translateProvider', function ($translateProvider) {

    angular.forEach(dicts, function (value, key) {
        $translateProvider.translations(key, value)
    });
    // add translation table
    $translateProvider
        .preferredLanguage('en')
        .useSanitizeValueStrategy('sanitize');
}]);

app.config(['$routeProvider', '$locationProvider',
    function ($routeProvider, $locationProvider) {
        MysteryRoutes.forEach(function (route) {
            route.controllerAs = 'rc';
            $routeProvider.when(route.when, route);
        });

        $routeProvider.otherwise({
            redirectTo: '/start'
        });

        $locationProvider.html5Mode(true);

    }]);




app.config(['$provide', function ($provide) {

    $provide.decorator('filterFilter', [
      '$delegate','ContentMem',
      function filterDecorator($delegate,ContentMem) {

          // store the original filter
          var originalFilter = $delegate;

          // return our filter
          return filterAvoidRecursion;

          function filterAvoidRecursion(array, expression, comparator, anyPropertyKey) {
              //do we need filters?
              if (angular.isUndefined(array))
                  return null;
              if (!array)
                  return [];

              //let's avoid recursion by creating a copy of the given array
              array = Mystery.limitedCopyContent(array);
              
              var filter_result = originalFilter(array, expression, comparator, anyPropertyKey);
              //yet we want the completed objects
              filter_result = ContentMem.recostructData(filter_result);

              return filter_result;
          }

      }

    ]);

}]);

//a directive for Metis menu
app.directive('ngMetis', ["$timeout", function($timeout) {
        
    
    var directive = {
        restrict: 'A',
        link: function(scope, el, atts) {
                
            $timeout(function () {
                var menu_argument = null;
                if (angular.isDefined(atts.ngMetis) && atts.ngMetis.toString() !== "")
                    menu_argument = angular.fromJson(atts.ngMetis);
                angular.element(el).metisMenu(menu_argument);
            }, 0,false);             
        }
    };
        
    return directive;        
}]);

//a directive to have focus on elements
app.directive('focusMe',['$timeout', function ($timeout) {
    return {
        link: function (scope, element, attrs) {
            scope.$watch(attrs.focusMe, function (value) {
                if (value === true) {
                    //calling $timeout as the element which want focus could be still hidden
                    $timeout(function () {
                        element[0].focus();
                        scope[attrs.focusMe] = false;
                    });
                }
            });
        }
    };
}]);

//puting this back because is used in angular-translate
angular.lowercase = function (string) { return angular.isString(string) ? string.toLowerCase() : string; };

