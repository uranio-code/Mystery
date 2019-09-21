var Mystery = {}; //hello!
//the magic of mystery keeping 1 reference to all object with guid is great
//yet sometime we have problem is recursion, for example tree nodes with parent and children 
//during search or json conversion
Mystery.CopyContentDept = 3;
Mystery.limitedCopyContent = function (content, current_level) {
    if (angular.isUndefined(current_level))
        current_level = 0;

    if (current_level > Mystery.CopyContentDept)
        return null;
    if (angular.isUndefined(content))
        return content;

    if (angular.isArray(content)) {
        return content.map(function (x) { return Mystery.limitedCopyContent(x, current_level + 1);})
    }

    if (!angular.isObject(content)||angular.isDate(content))
        return content;
    
    var result = {};
    angular.forEach(content, function (value, key) {
        result[key] = Mystery.limitedCopyContent(value, current_level + 1);
    });

    return result;
}

//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size
$(function () {
    $(window).bind("load resize", function () {
        var topOffset = 50;
        var width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        var height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        if (height < 1) height = 1;
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }
    });

    var url = window.location;
    // var element = $('ul.nav a').filter(function() {
    //     return this.href == url;
    // }).addClass('active').parent().parent().addClass('in').parent();
    var element = $('ul.nav a').filter(function () {
        return this.href === url;
    }).addClass('active').parent();

    while (true) {
        if (element.is('li')) {
            element = element.parent().addClass('in').parent();
        } else {
            break;
        }
    }
});
