var Toolbelt;
(function (Toolbelt) {
    var Head;
    (function (Head) {
        var MetaTag;
        (function (MetaTag) {
            const x = 'property';
            const y = 'meta[name],meta[property]';
            const d = document;
            const h = d.head;
            const q = (s) => Array.from(h.querySelectorAll(s));
            const e = (m, a) => a.t === 0 ? m.name === a.k : m.getAttribute(x) === a.k;
            const r = (m) => h.removeChild(m);
            MetaTag.set = (a) => {
                let meta = q('meta').find(m => e(m, a));
                let n = null;
                if (typeof (meta) === 'undefined') {
                    meta = d.createElement('meta');
                    n = meta;
                }
                if (a.t === 0)
                    meta.name = a.k;
                else
                    meta.setAttribute(x, a.k);
                meta.content = a.c;
                if (n !== null)
                    h.appendChild(n);
            };
            MetaTag.reset = (as) => {
                as.forEach(MetaTag.set);
                q(y).filter(m => !as.some(a => e(m, a))).forEach(r);
            };
            MetaTag.del = (a) => q(y).filter(m => e(m, a)).forEach(r);
            MetaTag.query = () => JSON.parse((q('script[type="text/default-meta-elements"]').pop() || { text: 'null' }).text) || q(y).map(m => (p => ({ t: p === null ? 0 : 1, k: p || m.name, c: m.content }))(m.getAttribute(x)));
        })(MetaTag = Head.MetaTag || (Head.MetaTag = {}));
    })(Head = Toolbelt.Head || (Toolbelt.Head = {}));
})(Toolbelt || (Toolbelt = {}));
//# sourceMappingURL=script.js.map