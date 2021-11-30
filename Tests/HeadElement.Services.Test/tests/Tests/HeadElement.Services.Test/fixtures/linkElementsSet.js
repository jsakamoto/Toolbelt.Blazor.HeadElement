"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.LinkElementsSet = void 0;
class LinkElementsSet {
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
    get canonical() { return this.linkElements["canonical"]; }
    get prev() { return this.linkElements["prev"]; }
    get next() { return this.linkElements["next"]; }
    get icon() { return this.linkElements["icon"]; }
    get iconWithSizes() { return this.linkElements["iconWithSizes"]; }
    get alternateWithType() { return this.linkElements["alternateWithType"]; }
    get alternateWithMedia() { return this.linkElements["alternateWithMedia"]; }
    get preload() { return this.linkElements["preload"]; }
    get preloadWithMedia() { return this.linkElements["preloadWithMedia"]; }
    get preloadWithType() { return this.linkElements["preloadWithType"]; }
    get unknown() { return this.linkElements["unknown"]; }
    forEach(action) {
        for (const link of Object.values(this.linkElements)) {
            action(link);
        }
    }
    createLink(props) {
        const baseLinkElement = { sizes: '', media: '', type: '' };
        const newLinkElement = Object.assign(new HTMLLinkElementMock(), baseLinkElement);
        return Object.assign(newLinkElement, props);
    }
}
exports.LinkElementsSet = LinkElementsSet;
class HTMLLinkElementMock {
    getAttribute(name) {
        return this[name] || null;
    }
}
