"use strict";
var Toolbelt;
(function (Toolbelt) {
    var Head;
    (function (Head) {
        var _a, _b;
        const searchParam = ((_b = (_a = document.currentScript) === null || _a === void 0 ? void 0 : _a.getAttribute('src')) === null || _b === void 0 ? void 0 : _b.split('?')[1]) || '';
        const url = ['./script.module.min.js', searchParam].filter(v => v != '').join('?');
        Head.ready = import(url).then(m => {
            Object.assign(Head, m.Toolbelt.Head);
        });
    })(Head = Toolbelt.Head || (Toolbelt.Head = {}));
})(Toolbelt || (Toolbelt = {}));
