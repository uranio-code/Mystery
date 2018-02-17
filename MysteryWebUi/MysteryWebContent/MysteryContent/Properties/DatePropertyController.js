app.controller("mysteryDatePropertyController", ['MysteryDownloader', function (MysteryDownloader) {
    var me = this;
    me.dt = new Date();
    me.format = "dd MMMM yyyy";

    me.today = function () {
        me.dt = new Date();
    };

    me.clear = function () {
        me.dt = null;
    };

    me.init = function (cp) {
        me.cp = cp;
        if (cp.data.content[cp.data.name]) { me.dt = Date.parse(cp.data.content[cp.data.name]) }
        else { me.clear() }
    }

    me.minDate = new Date(1950, 1, 1);
    me.maxDate = new Date(2020, 5, 22);

    me.open = function () {
        me.status.opened = true;
    };

    me.setDate = function (year, month, day) {
        me.dt = new Date(year, month, day);
    };

    me.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    me.status = {
        opened: false
    };

    me.commit = function () {
        //js apply time zones.. nice but not with dates
        me.cp.current_value = 
            me.dt.getFullYear().toString()+'-'+
            (me.dt.getMonth()+1).toString()+'-'+
            me.dt.getDate();
        me.cp.commit();
    };

    me.cancel = function () {
        me.cp.cancel();
        me.init(me.cp);
    }

}]);