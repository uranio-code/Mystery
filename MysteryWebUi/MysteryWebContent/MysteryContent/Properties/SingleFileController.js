app.directive('mysterySingleFileInput', function() {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var ele = angular.element(element)
            var options = {
                uploadUrl: "FileService/PostNewFile", // server upload action
                uploadAsync: true,
                maxFileCount: 1,
            };
            if (angular.isDefined(attrs.showUpload))
                attrs.showUpload = (attrs.showUpload == "true" || attrs.showUpload == "1")
            angular.merge(options, attrs);
            ele.fileinput(options);
            ele.on('fileuploaded', scope.onUpload);
            //ele.on('filebatchselected', function(event, files) {
            //    // trigger upload method immediately after files are selected
            //    ele.fileinput('upload');
            //});
        }
    };
});
app.controller("SingleFileController", ['MysteryDownloader', '$scope', '$uibModal', function (MysteryDownloader, $scope, $uibModal) {
    var me = this;
    me.cp = null;
    me.init = function (cp) {
        me.cp = cp;
    };
    me.data = {};
    
    me.onUpload = function (event, data, previewId, index) {
        me.data = MysteryDownloader.parseRespose(data.response).output[0];
        me.cp.current_value = me.data;
        me.cp.commit();
    };
    //to be passed to the linker function
    $scope.onUpload = me.onUpload;

    me.remove = function () {
        me.cp.current_value = null;
        me.cp.commit();
    }
}]);