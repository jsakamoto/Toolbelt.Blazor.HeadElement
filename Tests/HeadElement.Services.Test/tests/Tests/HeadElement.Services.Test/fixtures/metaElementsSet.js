"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MetaElementsSet = void 0;
class MetaElementsSet {
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
    get description() { return this.metaElements["description"]; }
    get contentType() { return this.metaElements["contentType"]; }
    get themeColorLight() { return this.metaElements["themeColorLight"]; }
    get themeColorDark() { return this.metaElements["themeColorDark"]; }
    get ogType() { return this.metaElements["ogType"]; }
    forEach(action) {
        for (const meta of Object.values(this.metaElements)) {
            action(meta);
        }
    }
    createMeta(props) {
        return Object.assign(new HTMLMetaElementMock(), props);
    }
}
exports.MetaElementsSet = MetaElementsSet;
class HTMLMetaElementMock {
    constructor() {
        this.name = '';
        this.content = '';
        this.httpEquiv = '';
        this.media = '';
    }
    getAttribute(name) {
        return this[name] || null;
    }
}
//# sourceMappingURL=metaElementsSet.js.map