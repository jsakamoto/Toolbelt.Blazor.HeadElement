namespace Toolbelt.Head {

    interface MetaElement {
        n: string;
        p: string;
        h: string;
        c: string;
    }

    interface LinkElement {
        /** rel */
        r: string;
        /** href */
        h: string;
        /** sizes */
        s: string;
        /** type */
        p: string;
        /** title */
        t: string;
        /** media */
        m: string;
    }

    const selectorForMata = 'meta[name],meta[property]';
    const selectorForLinks = 'link';
    const selectorForScript = 'script[type="text/default-';
    const property = 'property';
    const httpEquiv = 'http-equiv';
    const href = 'href';
    const undef = 'undefined';

    const d = document;
    const head = d.head;

    const q = (selector: string) => Array.from(head.querySelectorAll(selector)) as any[];
    const crealeElem = (tagName: string) => d.createElement(tagName) as any;

    const removeChild = (m: HTMLMetaElement | HTMLLinkElement) => head.removeChild(m);
    const getAttr = (e: HTMLElement, attrName: string) => e.getAttribute(attrName);
    const setAttr = (e: HTMLElement, attrName: string, value: string) => e.setAttribute(attrName, value);
    const sameMeta = (m: HTMLMetaElement, a: MetaElement) => a.n !== '' ? m.name === a.n : (a.h !== '' ? m.httpEquiv === a.h : getAttr(m, property) === a.p);
    const sameLink = (m: HTMLLinkElement, a: LinkElement) => m.rel === a.r && (
        (['canonical', 'prev', 'next'].indexOf(a.r) !== -1) ||
        (a.r === 'icon' && ('' + m.sizes) === a.s) ||
        (a.r === 'alternate' && m.type === a.p && m.media === a.m) ||
        (getAttr(m, href) === a.h) // rel='stylesheet'
    );

    export const Title = {

        set: (t: string) => { d.title = t; },

        query: () => (q(selectorForScript + 'title"]').pop() || { text: d.title }).text
    }

    export const MetaTag = {

        set: (args: MetaElement[]) => {
            args.forEach(arg => {
                let meta = q('meta').find(m => sameMeta(m, arg));
                let n: HTMLMetaElement | null = null;
                if (typeof meta === undef) {
                    meta = crealeElem('meta');
                    n = meta;
                }
                if (arg.h !== '') setAttr(meta, httpEquiv, arg.h);
                if (arg.p !== '') setAttr(meta, property, arg.p);
                if (arg.n !== '') meta.name = arg.n;
                meta.content = arg.c;
                if (n !== null) head.appendChild(n);
            });
        },

        reset: (args: MetaElement[]) => {
            MetaTag.set(args);
            q(selectorForMata).filter(m => !args.some(arg => sameMeta(m, arg))).forEach(removeChild);
        },

        del: (args: MetaElement[]) => args.forEach(arg => q(selectorForMata).filter(m => sameMeta(m, arg)).forEach(removeChild)),

        query: () => JSON.parse((q(selectorForScript + 'meta-elements"]').pop() || { text: 'null' }).text) || q(selectorForMata).map(m => (p => ({ p: p || '', n: m.name || '', c: m.content || '' }))(getAttr(m, property)))
    }

    export const LinkTag = {

        set: (args: LinkElement[]) => {
            args.forEach(arg => {
                let link = q('link').find(m => sameLink(m, arg));
                let n: HTMLLinkElement | null = null;
                if (typeof link === undef) {
                    link = crealeElem('link');
                    n = link;
                }
                [
                    ['rel', arg.r], [href, arg.h], ['sizes', arg.s], ['type', arg.p], ['title', arg.t], ['media', arg.m]
                ].forEach(prop => {
                    if (prop[1] === '') link.removeAttribute(prop[0]);
                    else if (getAttr(link, prop[0]) !== prop[1]) setAttr(link, prop[0], prop[1]);
                });
                if (n !== null) head.appendChild(n);
            });
        },

        reset: (args: LinkElement[]) => {
            LinkTag.set(args);
            q(selectorForLinks).filter(m => !args.some(arg => sameLink(m, arg))).forEach(removeChild);
        },

        del: (args: LinkElement[]) => args.forEach(a => q(selectorForLinks).filter(m => sameLink(m, a)).forEach(removeChild)),

        query: () => JSON.parse((q(selectorForScript + 'link-elements"]').pop() || { text: 'null' }).text) || q(selectorForLinks).map(m => ({ r: m.rel, h: getAttr(m, href), s: '' + m.sizes, p: m.type, t: m.title, m: m.media }))
    }
}