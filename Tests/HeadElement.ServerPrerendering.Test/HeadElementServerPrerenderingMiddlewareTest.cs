using HeadElement.ServerPrerendering.Test.Fixtures;
using NUnit.Framework;
using Toolbelt.Blazor.HeadElement;
using static Toolbelt.Blazor.HeadElement.Middlewares.HeadElementServerPrerenderingMiddleware;

namespace HeadElement.ServerPrerendering.Test;

public class HeadElementServerPrerenderingMiddlewareTest
{
    private readonly static LinkElementsSet LinkElementsSet = new();

    private readonly static MetaElementsSet MetaElementsSet = new();

    [Test]
    public void SameLink_Canonical_Test()
    {
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.Canonical;
            SameLink(link, new(rel: "canonical", href: "https://example.com/")).Is(shouldBeSame);
            SameLink(link, new(rel: "canonical", href: "https://example.com/foo")).Is(shouldBeSame);
            SameLink(link, new(rel: "canonical", href: "https://example.jp/")).Is(shouldBeSame);
        });
    }

    [Test]
    public void SameLink_Prev_Test()
    {
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.Prev;
            SameLink(link, new(rel: "prev", href: "https://example.com/prev")).Is(shouldBeSame);
            SameLink(link, new(rel: "prev", href: "https://example.com/foo")).Is(shouldBeSame);
        });
    }

    [Test]
    public void SameLink_Next_Test()
    {
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.Next;
            SameLink(link, new(rel: "next", href: "https://example.com/next")).Is(shouldBeSame);
            SameLink(link, new(rel: "next", href: "https://example.com/foo")).Is(shouldBeSame);
        });
    }

    [Test]
    public void SameLink_Icon_Test()
    {
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.Icon;
            SameLink(link, new(rel: "icon", href: "https://example.com/count0.ico")).Is(shouldBeSame);
        });
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.IconWithSizes;
            SameLink(link, new(rel: "icon", href: "https://example.com/favicon16red.png", sizes: "16")).Is(shouldBeSame);
            SameLink(link, new(rel: "icon", href: "https://example.com/favicon32blue.png", sizes: "32")).IsFalse();
        });
    }

    [Test]
    public void SameLink_Alternate_Test()
    {
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.AlternateWithMedia;
            SameLink(link, new(rel: "alternate", href: "https://example.com/phone.html", media: "(min-width: 600px)")).Is(shouldBeSame);
            SameLink(link, new(rel: "alternate", href: "https://example.com/mobile.html", media: "(min-width: 1200px)")).IsFalse();
        });
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.AlternateWithType;
            SameLink(link, new(rel: "alternate", href: "https://example.com/mobile.pdf", type: "application/pdf")).Is(shouldBeSame);
            SameLink(link, new(rel: "alternate", href: "https://example.com/index.pdf", type: "plain/text")).IsFalse();
        });
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.AlternateWithHreflang;
            SameLink(link, new(rel: "alternate", href: "https://example.com/localized/", hreflang: "ja")).Is(shouldBeSame, $"failed at {link.ToHtml()}");
            SameLink(link, new(rel: "alternate", href: "https://example.com/ja", hreflang: "ja-jp")).IsFalse($"failed at {link.ToHtml()}");
        });
    }

    [Test]
    public void SameLink_Preload_Test()
    {
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.Preload;
            SameLink(link, new(rel: "preload", href: "https://example.com/foo.css", @as: "stylesheet", type: "text/css")).Is(shouldBeSame);
            SameLink(link, new(rel: "preload", href: "https://example.com/foo.css", media: "(min-width: 1200px)")).IsFalse();
        });
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.PreloadWithMedia;
            SameLink(link, new(rel: "preload", href: "https://example.com/bar.js", media: "(min-width: 600px)")).Is(shouldBeSame);
            SameLink(link, new(rel: "preload", href: "https://example.com/bar.js", media: "(min-width: 1200px)")).IsFalse();
            SameLink(link, new(rel: "preload", href: "https://example.com/bar.js")).IsFalse();
        });
        LinkElementsSet.ForEach(link =>
        {
            var shouldBeSame = link == LinkElementsSet.PreloadWithType;
            SameLink(link, new(rel: "preload", href: "https://example.com/fizz.woff2", @as: "woff2", type: "application/octetstream")).Is(shouldBeSame);
            SameLink(link, new(rel: "preload", href: "https://example.com/fizz.woff2", @as: "font", type: "font/woff2", media: "(min-width: 600px)")).IsFalse();
        });
    }

    [Test]
    public void SameMeta_Description_Test()
    {
        MetaElementsSet.ForEach(meta =>
        {
            var shouldBeSame = meta == MetaElementsSet.Description;
            SameMeta(meta, MetaElement.ByName("description", "Hello, World.")).Is(shouldBeSame);
            SameMeta(meta, MetaElement.ByName("description", "Nice to meet you.")).Is(shouldBeSame);
        });
    }

    [Test]
    public void SameMeta_ThemeColor_Test()
    {
        MetaElementsSet.ForEach(meta =>
        {
            var shouldBeSame = meta == MetaElementsSet.ThemeColorLight;
            SameMeta(meta, MetaElement.ByName("theme-color", media: "(prefers-color-scheme: light)", content: "white")).Is(shouldBeSame);
            SameMeta(meta, MetaElement.ByName("theme-color", media: "(prefers-color-scheme: light)", content: "red")).Is(shouldBeSame);
        });
    }

    [Test]
    public void SameMeta_ContentType_Test()
    {
        MetaElementsSet.ForEach(meta =>
        {
            var shouldBeSame = meta == MetaElementsSet.ContentType;
            SameMeta(meta, MetaElement.ByHttpEquiv("content-type", content: "text/html; charset=utf-8")).Is(shouldBeSame);
            SameMeta(meta, MetaElement.ByHttpEquiv("content-type", content: "text/html; charset=iso2022jp")).Is(shouldBeSame);
        });
    }

    [Test]
    public void SameMeta_OgType_Test()
    {
        MetaElementsSet.ForEach(meta =>
        {
            var shouldBeSame = meta == MetaElementsSet.OgType;
            SameMeta(meta, MetaElement.ByProp("og:type", content: "website")).Is(shouldBeSame);
            SameMeta(meta, MetaElement.ByProp("og:type", content: "article")).Is(shouldBeSame);
        });
    }
}
