"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Toolbelt = void 0;
var Toolbelt;
(function (Toolbelt) {
    var Head;
    (function (Head) {
        const metaElementName = 'meta';
        const linkElementName = 'link';
        const selectorForMata = 'meta[name],meta[property],meta[http-equiv]';
        const selectorForLinks = linkElementName;
        const selectorForScript = 'script[type="text/default-';
        const property = 'property';
        const href = 'href';
        const nullText = 'null';
        const d = typeof document !== 'undefined' ? document : {};
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
        const fixstr = (str) => str || '';
        Head.Title = {
            set: (t) => { d.title = t; },
            query: () => (q(selectorForScript + 'title"]').pop() || { text: d.title }).text
        };
        Head.MetaTag = {
            set: (args) => {
                args.forEach(arg => {
                    let meta = q(metaElementName).find(m => sameMeta(m, arg)) || null;
                    let n = null;
                    if (meta === null) {
                        meta = crealeElem(metaElementName);
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
            query: () => {
                const defaultMetas = eval((q(selectorForScript + 'meta-elements"]').pop() || { text: nullText }).text) ||
                    q(selectorForMata).map(m => [
                        getAttr(m, property),
                        m.name,
                        m.httpEquiv,
                        m.content
                    ]);
                return defaultMetas.map(a => ({
                    p: fixstr(a[0]),
                    n: fixstr(a[1]),
                    h: fixstr(a[2]),
                    c: fixstr(a[3]),
                }));
            }
        };
        Head.LinkTag = {
            set: (args) => {
                args.forEach(arg => {
                    let link = q(linkElementName).find(m => Head.LinkTag.sameLink(m, arg)) || null;
                    let newLink = null;
                    if (link === null) {
                        link = crealeElem(linkElementName);
                        newLink = link;
                    }
                    [
                        ['rel', arg.r],
                        [href, arg.h],
                        ['sizes', arg.s],
                        ['type', arg.p],
                        ['title', arg.t],
                        ['media', arg.m],
                        ['as', arg.a],
                        ['crossOrigin', arg.co],
                        ['hreflang', arg.hl],
                        ['imageSizes', arg.isz],
                        ['imageSrcset', arg.iss],
                        ['disabled', arg.d],
                    ].forEach(prop => {
                        let attrName = prop[0];
                        let attrVal = prop[1];
                        if (attrVal === true) {
                            setAttr(link, attrName, '');
                        }
                        else if (attrVal === false || attrVal === '') {
                            link.removeAttribute(attrName);
                        }
                        else if (getAttr(link, attrName) !== attrVal) {
                            setAttr(link, attrName, attrVal);
                        }
                    });
                    if (newLink !== null)
                        head.appendChild(newLink);
                });
            },
            reset: (args) => {
                Head.LinkTag.set(args);
                q(selectorForLinks).filter(m => !args.some(arg => Head.LinkTag.sameLink(m, arg))).forEach(removeChild);
            },
            del: (args) => args.forEach(a => {
                q(selectorForLinks).filter(m => Head.LinkTag.sameLink(m, a)).forEach(removeChild);
            }),
            query: () => {
                const defaultLinks = eval((q(selectorForScript + 'link-elements"]').pop() || { text: nullText }).text) ||
                    q(selectorForLinks).map(m => [
                        m.rel,
                        getAttr(m, href),
                        '' + m.sizes,
                        m.type,
                        m.title,
                        m.media,
                        m.as,
                        m.crossOrigin || '',
                        m.hreflang,
                        m.imageSizes,
                        m.imageSrcset,
                        m.disabled
                    ]);
                return defaultLinks.map(a => ({
                    r: fixstr(a[0]),
                    h: fixstr(a[1]),
                    s: fixstr(a[2]),
                    p: fixstr(a[3]),
                    t: fixstr(a[4]),
                    m: fixstr(a[5]),
                    a: fixstr(a[6]),
                    co: fixstr(a[7]),
                    hl: fixstr(a[8]),
                    isz: fixstr(a[9]),
                    iss: fixstr(a[10]),
                    d: a[11] || false,
                }));
            },
            sameLink: (m, a) => m.rel === a.r && ((linkComparer[a.r] || ((m, a) => getAttr(m, href) === a.h))(m, a))
        };
    })(Head = Toolbelt.Head || (Toolbelt.Head = {}));
})(Toolbelt = exports.Toolbelt || (exports.Toolbelt = {}));
