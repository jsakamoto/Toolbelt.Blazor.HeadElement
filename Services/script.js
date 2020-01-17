var Toolbelt;
(function (Toolbelt) {
    var Head;
    (function (Head) {
        const selectorForMata = 'meta[name],meta[property]';
        const selectorForLinks = 'link';
        const selectorForScript = 'script[type="text/default-';
        const property = 'property';
        const href = 'href';
        const undef = 'undefined';
        const d = document;
        const head = d.head;
        const q = (selector) => Array.from(head.querySelectorAll(selector));
        const crealeElem = (tagName) => d.createElement(tagName);
        const removeChild = (m) => head.removeChild(m);
        const getAttr = (e, attrName) => e.getAttribute(attrName);
        const setAttr = (e, attrName, value) => e.setAttribute(attrName, value);
        const sameMeta = (m, a) => a.n !== '' ? m.name === a.n : getAttr(m, property) === a.p;
        const sameLink = (m, a) => m.rel === a.r && ((['canonical', 'prev', 'next'].indexOf(a.r) !== -1) ||
            (a.r === 'icon' && ('' + m.sizes) === a.s) ||
            (a.r === 'alternate' && m.type === a.p && m.media === a.m) ||
            (getAttr(m, href) === a.h));
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
                Head.MetaTag.set(args);
                q(selectorForMata).filter(m => !args.some(arg => sameMeta(m, arg))).forEach(removeChild);
            },
            del: (args) => args.forEach(arg => q(selectorForMata).filter(m => sameMeta(m, arg)).forEach(removeChild)),
            query: () => JSON.parse((q(selectorForScript + 'meta-elements"]').pop() || { text: 'null' }).text) || q(selectorForMata).map(m => (p => ({ p: p || '', n: m.name || '', c: m.content || '' }))(getAttr(m, property)))
        };
        Head.LinkTag = {
            set: (args) => {
                args.forEach(arg => {
                    let link = q('link').find(m => sameLink(m, arg));
                    let n = null;
                    if (typeof link === undef) {
                        link = crealeElem('link');
                        n = link;
                    }
                    [
                        ['rel', arg.r], [href, arg.h], ['sizes', arg.s], ['type', arg.p], ['title', arg.t], ['media', arg.m]
                    ].forEach(prop => {
                        if (prop[1] === '')
                            link.removeAttribute(prop[0]);
                        else if (getAttr(link, prop[0]) !== prop[1])
                            setAttr(link, prop[0], prop[1]);
                    });
                    if (n !== null)
                        head.appendChild(n);
                });
            },
            reset: (args) => {
                Head.LinkTag.set(args);
                q(selectorForLinks).filter(m => !args.some(arg => sameLink(m, arg))).forEach(removeChild);
            },
            del: (args) => args.forEach(a => q(selectorForLinks).filter(m => sameLink(m, a)).forEach(removeChild)),
            query: () => JSON.parse((q(selectorForScript + 'link-elements"]').pop() || { text: 'null' }).text) || q(selectorForLinks).map(m => ({ r: m.rel, h: getAttr(m, href), s: '' + m.sizes, p: m.type, t: m.title, m: m.media }))
        };
    })(Head = Toolbelt.Head || (Toolbelt.Head = {}));
})(Toolbelt || (Toolbelt = {}));
//# sourceMappingURL=script.js.map