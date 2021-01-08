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
        /** as */
        a: string;
    }

    const selectorForMata = 'meta[name],meta[property],meta[http-equiv]';
    const selectorForLinks = 'link';
    const selectorForScript = 'script[type="text/default-';
    const property = 'property';
    const href = 'href';
    const undef = 'undefined';

    const d = document;
    const head = d.head;

    function q<T>(selector: string) { return Array.from(head.querySelectorAll(selector)) as any[] as T[]; }
    const crealeElem = (tagName: string) => d.createElement(tagName) as any;

    const removeChild = (m: HTMLMetaElement | HTMLLinkElement) => head.removeChild(m);
    const removeMeta = (m: HTMLMetaElement) => { removeChild(m); if (m.httpEquiv === 'refresh') window.stop(); }
    const getAttr = (e: HTMLElement, attrName: string) => e.getAttribute(attrName);
    const setAttr = (e: HTMLElement, attrName: string, value: string) => e.setAttribute(attrName, value);
    const sameMeta = (m: HTMLMetaElement, a: MetaElement) => a.n !== '' ? m.name === a.n : (a.h !== '' ? m.httpEquiv === a.h : getAttr(m, property) === a.p);
    const linkComparer: { [key: string]: (m: HTMLLinkElement, a: LinkElement) => boolean } = {
        canonical: () => true,
        prev: () => true,
        next: () => true,
        icon: (m, a) => ('' + m.sizes) === a.s,
        alternate: (m, a) => m.type === a.p && m.media === a.m,
        preload: (m, a) => getAttr(m, href) === a.h && m.media === a.m,
    };
    const sameLink = (m: HTMLLinkElement, a: LinkElement) => m.rel === a.r && (
        (linkComparer[a.r] || ((m, a) => getAttr(m, href) === a.h))(m, a)
    );

    export const Title = {

        set: (t: string) => { d.title = t; },

        query: () => (q<HTMLScriptElement>(selectorForScript + 'title"]').pop() || { text: d.title }).text
    }

    export const MetaTag = {

        set: (args: MetaElement[]) => {
            args.forEach(arg => {
                let meta = q<HTMLMetaElement>('meta').find(m => sameMeta(m, arg));
                let n: HTMLMetaElement | null = null;
                if (typeof meta === undef) {
                    meta = crealeElem('meta');
                    n = meta;
                }
                if (arg.h !== '') meta.httpEquiv = arg.h;
                if (arg.p !== '') setAttr(meta, property, arg.p);
                if (arg.n !== '') meta.name = arg.n;
                meta.content = arg.c;
                if (n !== null) head.appendChild(n);
            });
        },

        reset: (args: MetaElement[]) => {
            q<HTMLMetaElement>(selectorForMata).filter(m => !args.some(arg => sameMeta(m, arg))).forEach(removeMeta);
            MetaTag.set(args);
        },

        del: (args: MetaElement[]) => args.forEach(arg => q<HTMLMetaElement>(selectorForMata).filter(m => sameMeta(m, arg)).forEach(removeMeta)),

        query: () => JSON.parse((q<HTMLScriptElement>(selectorForScript + 'meta-elements"]').pop() || { text: 'null' }).text) || q<HTMLMetaElement>(selectorForMata).map(m => (p => ({ p: p || '', n: m.name || '', h: m.httpEquiv || '', c: m.content || '' }))(getAttr(m, property)))
    }

    export const LinkTag = {

        set: (args: LinkElement[]) => {
            args.forEach(arg => {
                let link = q<HTMLLinkElement>('link').find(m => sameLink(m, arg));
                let newLink: HTMLLinkElement | null = null;
                if (typeof link === undef) {
                    link = crealeElem('link');
                    newLink = link;
                }
                [
                    ['rel', arg.r], [href, arg.h], ['sizes', arg.s], ['type', arg.p], ['title', arg.t], ['media', arg.m], ['as', arg.a]
                ].forEach(prop => {
                    if (prop[1] === '') link.removeAttribute(prop[0]);
                    else if (getAttr(link, prop[0]) !== prop[1]) setAttr(link, prop[0], prop[1]);
                });
                if (newLink !== null) head.appendChild(newLink);
            });
        },

        reset: (args: LinkElement[]) => {
            LinkTag.set(args);
            q<HTMLLinkElement>(selectorForLinks).filter(m => !args.some(arg => sameLink(m, arg))).forEach(removeChild);
        },

        del: (args: LinkElement[]) => args.forEach(a => {
            q<HTMLLinkElement>(selectorForLinks).filter(m => sameLink(m, a)).forEach(removeChild)
        }),

        query: () =>
            JSON.parse(
                (q<HTMLScriptElement>(selectorForScript + 'link-elements"]').pop() || { text: 'null' }).text
            ) || q<HTMLLinkElement>(selectorForLinks).map(m => ({
                r: m.rel, h: getAttr(m, href), s: '' + m.sizes, p: m.type, t: m.title, m: m.media, a: m.as
            }))
    }
}