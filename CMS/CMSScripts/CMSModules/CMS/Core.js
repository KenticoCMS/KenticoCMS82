cmsdefine(['CMS/Context'], function (Context) {
    'use strict';

    var Core = function (data, defaultData) {
        this.ctx = new Context(data, defaultData);
    };

    return Core;
})