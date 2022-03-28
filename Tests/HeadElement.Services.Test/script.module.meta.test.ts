import { Toolbelt } from "../../Services/script.module";
import * as assert from "assert";
import { describe } from "mocha";
import { MetaElementsSet } from "./fixtures/metaElementsSet";
import { MetaElementProps } from "./fixtures/metaElementProps";

describe('MetaTag', function () {
    const metaElementsSet = new MetaElementsSet();
    const sameMeta = Toolbelt.Head.MetaTag.sameMeta;

    function createMeta(props: MetaElementProps): Toolbelt.Head.MetaElement {
        const baseMetaElement = { n:'', p:'', h:'', m:'', c:'' };
        const newMetaElement = Object.assign({}, baseMetaElement);
        return Object.assign(newMetaElement, props);
    }

    it('sameMeta for description', function () {
        metaElementsSet.forEach(meta => {
            const shouldBeSame = meta === metaElementsSet.description;
            assert.equal(sameMeta(meta, createMeta({ n: 'description', c: 'Hello, World.' })), shouldBeSame);
            assert.equal(sameMeta(meta, createMeta({ n: 'description', c: 'Nice to meet you.' })), shouldBeSame);
        });
    })

    it('sameMeta for themeColor', function () {
        metaElementsSet.forEach(meta => {
            const shouldBeSame = meta === metaElementsSet.themeColorLight;
            assert.equal(sameMeta(meta, createMeta({ n: 'theme-color', m: '(prefers-color-scheme: light)', c: 'white' })), shouldBeSame);
            assert.equal(sameMeta(meta, createMeta({ n: 'theme-color', m: '(prefers-color-scheme: light)', c: 'red' })), shouldBeSame);
        });
    })

    it('sameMeta for content-type', function () {
        metaElementsSet.forEach(meta => {
            const shouldBeSame = meta === metaElementsSet.contentType;
            assert.equal(sameMeta(meta, createMeta({ h: 'content-type', c: 'text/html; charset=utf-8' })), shouldBeSame);
            assert.equal(sameMeta(meta, createMeta({ h: 'content-type', c: 'text/csv; charset=utf-8' })), shouldBeSame);
        });
    })

    it('sameMeta for og:type', function () {
        metaElementsSet.forEach(meta => {
            const shouldBeSame = meta === metaElementsSet.ogType;
            assert.equal(sameMeta(meta, createMeta({ p: 'og:type', c: 'website' })), shouldBeSame);
            assert.equal(sameMeta(meta, createMeta({ p: 'og:type', c: 'article' })), shouldBeSame);
        });
    })
});