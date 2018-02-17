app.factory("MysteryFormService", function () {
    var me = this;
    me.forms = [];
    me.register = function (form) {
        if (form == null) return;
        me.forms.push(form);
    };
    me.active = null;
    me.activate = function (form) {
        if (form!=null && me.forms.indexOf(form) < 0) me.register(form);
        if (me.active !== null && me.active !== form) {
            if (angular.isDefined(me.active.commit)) me.active.commit();
            if (angular.isDefined(me.active.exitEditMode)) me.active.exitEditMode();
        }
        me.active = form;
    }

    me.deactivate = function (form) {
        if (me.forms.indexOf(form) < 0) me.register(form);
        if (me.active === form) {
            me.active = null;
        }
    }
    return me;
});