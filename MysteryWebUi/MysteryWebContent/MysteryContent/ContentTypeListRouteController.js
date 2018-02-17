app.controller("ContentTypeListRouteController", ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
    function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
        var me = this;
        var path_array = $location.path().split("/")
        me.content_type_name = path_array[path_array.length - 1]
        me.content_type_table_options = {
            content_type_name: me.content_type_name,
            contents_source_promise: function () {
                return MysteryDownloader.getPromise(
                    'ContentTypeService/Contents?content_type_name=' + me.content_type_name,
                    function (data) {
                        return data.output;
                    });
            },
        }
    }
]);