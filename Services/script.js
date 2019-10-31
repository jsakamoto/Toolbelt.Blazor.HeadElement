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
            const e = (m, a) => a.n != '' ? m.name === a.n : m.getAttribute(x) === a.p;
            const r = (m) => h.removeChild(m);
            MetaTag.set = (as) => {
                as.forEach(a => {
                    let meta = q('meta').find(m => e(m, a));
                    let n = null;
                    if (typeof (meta) === 'undefined') {
                        meta = d.createElement('meta');
                        n = meta;
                    }
                    if (a.n != '')
                        meta.name = a.n;
                    if (a.p != '')
                        meta.setAttribute(x, a.p);
                    meta.content = a.c;
                    if (n !== null)
                        h.appendChild(n);
                });
            };
            MetaTag.reset = (as) => {
                MetaTag.set(as);
                q(y).filter(m => !as.some(a => e(m, a))).forEach(r);
            };
            MetaTag.del = (as) => as.forEach(a => q(y).filter(m => e(m, a)).forEach(r));
            MetaTag.query = () => JSON.parse((q('script[type="text/default-meta-elements"]').pop() || { text: 'null' }).text) || q(y).map(m => (p => ({ p: p || '', n: m.name || '', c: m.content || '' }))(m.getAttribute(x)));
        })(MetaTag = Head.MetaTag || (Head.MetaTag = {}));
    })(Head = Toolbelt.Head || (Toolbelt.Head = {}));
})(Toolbelt || (Toolbelt = {}));
//# sourceMappingURL=script.js.map