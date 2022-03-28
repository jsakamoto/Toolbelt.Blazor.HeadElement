"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const script_module_1 = require("../../Services/script.module");
const assert = require("assert");
const mocha_1 = require("mocha");
const linkElementsSet_1 = require("./fixtures/linkElementsSet");
(0, mocha_1.describe)('LinkTag', function () {
    const linkElementsSet = new linkElementsSet_1.LinkElementsSet();
    const sameLink = script_module_1.Toolbelt.Head.LinkTag.sameLink;
    function createLink(props) {
        const baseLinkElement = { r: "", h: "", s: "", p: "", t: "", m: "", a: "", co: "", hl: "", isz: "", iss: "", d: false };
        const newLinkElement = Object.assign({}, baseLinkElement);
        return Object.assign(newLinkElement, props);
    }
    it('sameLink for canonical', function () {
        linkElementsSet.forEach(link => {
            const shouldBeSame = link === linkElementsSet.canonical;
            assert.equal(sameLink(link, createLink({ r: "canonical", h: "https://example.com/" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "canonical", h: "https://example.com/foo" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "canonical", h: "https://example.jp/" })), shouldBeSame);
        });
    });
    it('sameLink for prev', function () {
        linkElementsSet.forEach(link => {
            const shouldBeSame = link === linkElementsSet.prev;
            assert.equal(sameLink(link, createLink({ r: "prev", h: "https://example.com/prev" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "prev", h: "https://example.com/foo" })), shouldBeSame);
        });
    });
    it('sameLink for next', function () {
        linkElementsSet.forEach(link => {
            const shouldBeSame = link === linkElementsSet.next;
            assert.equal(sameLink(link, createLink({ r: "next", h: "https://example.com/next" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "next", h: "https://example.com/foo" })), shouldBeSame);
        });
    });
    it('sameLink for icon', function () {
        linkElementsSet.forEach(link => {
            const shouldBeSame = link === linkElementsSet.icon;
            assert.equal(sameLink(link, createLink({ r: "icon", h: "https://example.com/count0.ico" })), shouldBeSame);
        });
        linkElementsSet.forEach(link => {
            const shouldBeSame = link === linkElementsSet.iconWithSizes;
            assert.equal(sameLink(link, createLink({ r: "icon", h: "https://example.com/favicon16red.png", s: "16" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "icon", h: "https://example.com/favicon32blue.png", s: "32" })), false);
        });
    });
    it('sameLink for alternate', function () {
        linkElementsSet.forEach(link => {
            const shouldBeSame = link === linkElementsSet.alternateWithMedia;
            assert.equal(sameLink(link, createLink({ r: "alternate", h: "https://example.com/phone.html", m: "(min-width: 600px)" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "alternate", h: "https://example.com/mobile.html", m: "(min-width: 1200px)" })), false);
        });
        linkElementsSet.forEach(link => {
            const shouldBeSame = link === linkElementsSet.alternateWithType;
            assert.equal(sameLink(link, createLink({ r: "alternate", h: "https://example.com/mobile.pdf", p: "application/pdf" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "alternate", h: "https://example.com/index.pdf", p: "plain/text" })), false);
        });
        linkElementsSet.forEach(link => {
            const shouldBeSame = link === linkElementsSet.alternateWithHreflang;
            assert.equal(sameLink(link, createLink({ r: "alternate", h: "https://example.com/localized/", hl: "ja" })), shouldBeSame, `fail at ${JSON.stringify(link)}`);
            assert.equal(sameLink(link, createLink({ r: "alternate", h: "https://example.com/ja/", hl: "ja-jp" })), false, `fail at ${JSON.stringify(link)}`);
        });
    });
    it('sameLink for preload', function () {
        linkElementsSet.forEach(link => {
            const shouldBeSame = link === linkElementsSet.preload;
            assert.equal(sameLink(link, createLink({ r: "preload", h: "https://example.com/foo.css", a: "stylesheet", p: "text/css" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "preload", h: "https://example.com/foo.css", m: "(min-width: 1200px)" })), false);
        });
        linkElementsSet.forEach(link => {
            var shouldBeSame = link === linkElementsSet.preloadWithMedia;
            assert.equal(sameLink(link, createLink({ r: "preload", h: "https://example.com/bar.js", m: "(min-width: 600px)" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "preload", h: "https://example.com/bar.js", m: "(min-width: 1200px)" })), false);
            assert.equal(sameLink(link, createLink({ r: "preload", h: "https://example.com/bar.js" })), false);
        });
        linkElementsSet.forEach(link => {
            var shouldBeSame = link === linkElementsSet.preloadWithType;
            assert.equal(sameLink(link, createLink({ r: "preload", h: "https://example.com/fizz.woff2", a: "woff2", p: "application/octetstream" })), shouldBeSame);
            assert.equal(sameLink(link, createLink({ r: "preload", h: "https://example.com/fizz.woff2", a: "font", p: "font/woff2", m: "(min-width: 600px)" })), false);
        });
    });
});
//# sourceMappingURL=script.module.link.test.js.map