export class MetaElementsSet {

    public metaElements: { [key: string]: HTMLMetaElement };

    public get description() { return this.metaElements["description"]; }
    public get contentType() { return this.metaElements["contentType"]; }
    public get themeColorLight() { return this.metaElements["themeColorLight"]; }
    public get themeColorDark() { return this.metaElements["themeColorDark"]; }
    public get ogType() { return this.metaElements["ogType"]; }

    constructor() {
        this.metaElements = {
            "description": this.createMeta({ name: "description", content: "Hello, World." }),
            "contentType": this.createMeta({ httpEquiv: "content-type", content: "text/html; charset=utf-8" }),
            "themeColorLight": this.createMeta({ name: "theme-color", media: "(prefers-color-scheme: light)", content: "white" }),
            "themeColorDark": this.createMeta({ name: "theme-color", media: "(prefers-color-scheme: dark)", content: "black" }),
            "ogType": this.createMeta({ property: "og:type", content: "website" }),
            "unknownName": this.createMeta({ name: "unknown", content: "unknown content" }),
        };
    }

    public forEach(action: (meta: HTMLMetaElement) => void): void {
        for (const meta of Object.values(this.metaElements)) {
            action(meta);
        }
    }

    private createMeta(props: any): HTMLMetaElement {
        return Object.assign(new HTMLMetaElementMock(), props);
    }
}

class HTMLMetaElementMock {
    public name = '';
    public content = '';
    public httpEquiv = '';
    public media = '';
    public getAttribute(name: string): string | null {
        return (this as any)[name] || null;
    }
}