var Toolbelt;
(function (Toolbelt) {
    var Head;
    (function (Head) {
        const selectorForMata = 'meta[name],meta[property],meta[http-equiv]';
        const selectorForLinks = 'link';
        const selectorForScript = 'script[type="text/default-';
        const property = 'property';
        const href = 'href';
        const undef = 'undefined';
        const d = document;
        const head = d.head;
        function q(selector) { return Array.from(head.querySelectorAll(selector)); }
        const crealeElem = (tagName) => d.createElement(tagName);
        const removeChild = (m) => head.removeChild(m);
        const removeMeta = (m) => { removeChild(m); if (m.httpEquiv === 'refresh')
            window.stop(); };
        const getAttr = (e, attrName) => e.getAttribute(attrName);
        const setAttr = (e, attrName, value) => e.setAttribute(attrName, value);
        const sameMeta = (m, a) => a.n !== '' ? m.name === a.n : (a.h !== '' ? m.httpEquiv === a.h : getAttr(m, property) === a.p);
        const linkComparer = {
            canonical: () => true,
            prev: () => true,
            next: () => true,
            icon: (m, a) => ('' + m.sizes) === a.s,
            alternate: (m, a) => m.type === a.p && m.media === a.m,
            preload: (m, a) => getAttr(m, href) === a.h && m.media === a.m,
        };
        const sameLink = (m, a) => m.rel === a.r && ((linkComparer[a.r] || ((m, a) => getAttr(m, href) === a.h))(m, a));
        Head.Title = {
            set: (t) => { d.title = t; },
            query: () => (q(selectorForScript + 'title"]').pop() || { text: d.title }).text
        };
        Head.MetaTag = {
            set: (args) => {
                args.forEach(arg => {
                    let meta = q('meta').find(m => sameMeta(m, arg));
                    let n = null;
                    if (typeof meta === undef) {
                        meta = crealeElem('meta');
                        n = meta;
                    }
                    if (arg.h !== '')
                        meta.httpEquiv = arg.h;
                    if (arg.p !== '')
                        setAttr(meta, property, arg.p);
                    if (arg.n !== '')
                        meta.name = arg.n;
                    meta.content = arg.c;
                    if (n !== null)
                        head.appendChild(n);
                });
            },
            reset: (args) => {
                q(selectorForMata).filter(m => !args.some(arg => sameMeta(m, arg))).forEach(removeMeta);
                Head.MetaTag.set(args);
            },
            del: (args) => args.forEach(arg => q(selectorForMata).filter(m => sameMeta(m, arg)).forEach(removeMeta)),
            query: () => JSON.parse((q(selectorForScript + 'meta-elements"]').pop() || { text: 'null' }).text) || q(selectorForMata).map(m => (p => ({ p: p || '', n: m.name || '', h: m.httpEquiv || '', c: m.content || '' }))(getAttr(m, property)))
        };
        Head.LinkTag = {
            set: (args) => {
                args.forEach(arg => {
                    let link = q('link').find(m => sameLink(m, arg));
                    let newLink = null;
                    if (typeof link === undef) {
                        link = crealeElem('link');
                        newLink = link;
                    }
                    [
                        ['rel', arg.r], [href, arg.h], ['sizes', arg.s], ['type', arg.p], ['title', arg.t], ['media', arg.m], ['as', arg.a]
                    ].forEach(prop => {
                        if (prop[1] === '')
                            link.removeAttribute(prop[0]);
                        else if (getAttr(link, prop[0]) !== prop[1])
                            setAttr(link, prop[0], prop[1]);
                    });
                    if (newLink !== null)
                        head.appendChild(newLink);
                });
            },
            reset: (args) => {
                Head.LinkTag.set(args);
                q(selectorForLinks).filter(m => !args.some(arg => sameLink(m, arg))).forEach(removeChild);
            },
            del: (args) => args.forEach(a => {
                q(selectorForLinks).filter(m => sameLink(m, a)).forEach(removeChild);
            }),
            query: () => JSON.parse((q(selectorForScript + 'link-elements"]').pop() || { text: 'null' }).text) || q(selectorForLinks).map(m => ({
                r: m.rel, h: getAttr(m, href), s: '' + m.sizes, p: m.type, t: m.title, m: m.media, a: m.as
            }))
        };
    })(Head = Toolbelt.Head || (Toolbelt.Head = {}));
})(Toolbelt || (Toolbelt = {}));
//# sourceMappingURL=script.js.map