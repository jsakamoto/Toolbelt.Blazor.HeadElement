namespace Toolbelt.Head.MetaTag {

    interface MetaElement {
        n: string;
        p: string;
        c: string;
    }

    const x = 'property';
    const y = 'meta[name],meta[property]';

    const d = document;
    const h = d.head;
    const q = (s: string) => Array.from(h.querySelectorAll(s) as NodeListOf<HTMLMetaElement>);
    const e = (m: HTMLMetaElement, a: MetaElement) => a.n != '' ? m.name === a.n : m.getAttribute(x) === a.p;
    const r = (m: HTMLMetaElement) => h.removeChild(m);

    export const set = (as: MetaElement[]) => {
        as.forEach(a => {
            let meta = q('meta').find(m => e(m, a));
            let n: HTMLMetaElement | null = null;
            if (typeof (meta) === 'undefined') {
                meta = d.createElement('meta');
                n = meta;
            }
            if (a.n != '') meta.name = a.n;
            if (a.p != '') meta.setAttribute(x, a.p);
            meta.content = a.c;
            if (n !== null) h.appendChild(n);
        });
    }

    export const reset = (as: MetaElement[]) => {
        set(as);
        q(y).filter(m => !as.some(a => e(m, a))).forEach(r);
    }

    export const del = (as: MetaElement[]) => as.forEach(a => q(y).filter(m => e(m, a)).forEach(r));

    export const query = () => JSON.parse((q('script[type="text/default-meta-elements"]').pop() as any || { text: 'null' }).text) || q(y).map(m => (p => ({ p: p || '', n: m.name || '', c: m.content || '' }))(m.getAttribute(x)));
}