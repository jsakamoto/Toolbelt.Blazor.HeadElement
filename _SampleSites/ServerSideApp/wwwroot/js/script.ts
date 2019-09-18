namespace Toolbelt.Head.MetaTag {

    const enum MetaEntryKeyType {
        Name = 0,
        Property = 1
    }

    interface MetaEntry {
        t: MetaEntryKeyType;
        k: string;
        c: string;
    }

    const x = 'property';
    const y = 'meta[name],meta[property]';

    const d = document;
    const h = d.head;
    const q = (s: string) => Array.from(h.querySelectorAll(s) as NodeListOf<HTMLMetaElement>);
    const e = (m: HTMLMetaElement, a: MetaEntry) => a.t === MetaEntryKeyType.Name ? m.name === a.k : m.getAttribute(x) === a.k;
    const r = (m: HTMLMetaElement) => h.removeChild(m);

    export const set = (a: MetaEntry) => {
        let meta = q('meta').find(m => e(m, a));
        let n: HTMLMetaElement | null = null;
        if (typeof (meta) === 'undefined') {
            meta = d.createElement('meta');
            n = meta;
        }
        if (a.t === MetaEntryKeyType.Name) meta.name = a.k;
        else meta.setAttribute(x, a.k);
        meta.content = a.c;
        if (n !== null) h.appendChild(n);
    }

    export const reset = (as: MetaEntry[]) => {
        as.forEach(set);
        q(y).filter(m => !as.some(a => e(m, a))).forEach(r);
    }

    export const del = (a: MetaEntry) => q(y).filter(m => e(m, a)).forEach(r);

    export const query = () => JSON.parse((q('script[type="text/default-meta-elements"]').pop() as any || { text: 'null' }).text) || q(y).map(m => (p => ({ t: p === null ? MetaEntryKeyType.Name : MetaEntryKeyType.Property, k: p || m.name, c: m.content }))(m.getAttribute(x)));
}