export class LinkElementsSet {

    public linkElements: { [key: string]: HTMLLinkElement };

    public get canonical() { return this.linkElements["canonical"]; }
    public get prev() { return this.linkElements["prev"]; }
    public get next() { return this.linkElements["next"]; }
    public get icon() { return this.linkElements["icon"]; }
    public get iconWithSizes() { return this.linkElements["iconWithSizes"]; }
    public get alternateWithType() { return this.linkElements["alternateWithType"]; }
    public get alternateWithMedia() { return this.linkElements["alternateWithMedia"]; }
    public get preload() { return this.linkElements["preload"]; }
    public get preloadWithMedia() { return this.linkElements["preloadWithMedia"]; }
    public get preloadWithType() { return this.linkElements["preloadWithType"]; }
    public get unknown() { return this.linkElements["unknown"]; }

    constructor() {
        this.linkElements = {
            "canonical": this.createLink({ rel: "canonical", href: "https://example.com/" }),
            "prev": this.createLink({ rel: "prev", href: "https://example.com/prev" }),
            "next": this.createLink({ rel: "next", href: "https://example.com/next" }),

            "icon": this.createLink({ rel: "icon", href: "https://example.com/favicon.ico" }),
            "iconWithSizes": this.createLink({ rel: "icon", href: "https://example.com/favicon16blue.png", sizes: "16" }),

            "alternateWithType": this.createLink({ rel: "alternate", href: "https://example.com/index.pdf", type: "application/pdf" }),
            "alternateWithMedia": this.createLink({ rel: "alternate", href: "https://example.com/mobile.html", media: "(min-width: 600px)" }),

            "preload": this.createLink({ rel: "preload", href: "https://example.com/foo.css" }),
            "preloadWithMedia": this.createLink({ rel: "preload", href: "https://example.com/bar.js", media: "(min-width: 600px)" }),
            "preloadWithType": this.createLink({ rel: "preload", href: "https://example.com/fizz.woff2", as: "font", type: "font/woff2" }),

            "unknown": this.createLink({ rel: "unknown", href: "https://example.com/" }),
        };
    }

    public forEach(action: (link: HTMLLinkElement) => void): void {
        for (const link of Object.values(this.linkElements)) {
            action(link);
        }
    }

    private createLink(props: any): HTMLLinkElement {
        const baseLinkElement = { sizes: '', media: '', type: '' };
        const newLinkElement = Object.assign(new HTMLLinkElementMock(), baseLinkElement);
        return Object.assign(newLinkElement, props);
    }
}

class HTMLLinkElementMock {
    public getAttribute(name: string): string | null {
        return (this as any)[name] || null;
    }
}