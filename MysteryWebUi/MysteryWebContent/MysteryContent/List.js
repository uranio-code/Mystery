app.directive('mysteryContentList', function () {
    return {
        scope: {
            options:'=options',
        },
        restrict: 'E',
        templateUrl: 'MysteryWebContent/MysteryContent/List.html'
    };
});

app.controller("MysteryContentListController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location','$rootScope','$q',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location, $rootScope,$q) {
        var me = this;
        //translations
        me.search_placeholder = $translate.instant('COMMON.SEARCH') + '...';
        
        //default options
        me.options = {
            columns: [],
            contents: [],
            contents_source_promise: function () {
                var deferred = $q.defer();
                deferred.resolve(me.options.contents);
                return deferred.promise;
            },
            content_type_name: '',

        };

        //we take the given options and we override the default ones
        angular.extend(me.options, $scope.options);

        //we update the options object with the result by extending back
        angular.extend($scope.options, me.options);

        //$scope.options is the object commining from the parent scope 
        //me.options is my new internal object
        //they are distict but all their properties are poiting to the same objects


        me.service_url = angular.isDefined($scope.serviceUrl) ? $scope.serviceUrl : '';
        me.contents_field = angular.isDefined($scope.contentsField) ? $scope.contentsField : '';
        me.columns = angular.isDefined($scope.columns) ? $scope.columns : [];
        me.loading = true;
        me.displayedCollection = [];

        me.displayContents = function (contents) {
            if (angular.isUndefined(contents) || contents === null)
                contents = [];
            me.options.contents.length = 0;
            me.options.contents = me.options.contents.concat(contents);
            me.displayedCollection = [].concat(me.options.contents);
            me.loading = false;
        };
       
        
        me.options.contents_source_promise().then(me.displayContents);
        
        
        //we have (or will have) the contents, it is time to understand which columns we need
        if (me.options.columns.length > 0) {
            //columns are give, perfect, we don't need anything else
        }
        else if (me.options.content_type_name.length > 0) {
            //we ask the content type service to tell the typical columns for this type
            MysteryDownloader.get('ContentTypeService/ContentTypeTable/' + me.options.content_type_name,
            function (data) {
                me.options.columns = data.output;
                me.createGetters();
            });
        }

        //something could delete a content any time! if that happens let's take it out from our list
        $rootScope.$on("content_deleted", function (event, data) {
            if(angular.isUndefined(data)||angular.isUndefined(data.guid))
                return;
            me.options.contents = options.contents.filter(function (item) {
                return item.guid !== data.guid;
            });
        });

        //sorting getters
        me.createGetters = function () {
            me.getters = {};
            angular.forEach(me.options.columns, function (col, key) {
                me.getters[col.name] = function (x) {
                    var result = x[col.name + '_sort_value'];
                    return result;
                };
            });
        };

        me.createGetters();

        me.click = function (content) {
            if (!angular.isUndefined(content.url))
                $location.path(content.url);
        };


    }
]);