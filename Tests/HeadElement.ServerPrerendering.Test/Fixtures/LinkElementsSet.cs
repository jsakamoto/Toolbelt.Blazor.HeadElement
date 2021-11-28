using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace HeadElement.ServerPrerendering.Test.Fixtures;

public class LinkElementsSet
{
    public IReadOnlyDictionary<string, IHtmlLinkElement> LinkElements { get; }

    public IHtmlLinkElement Canonical => this.LinkElements[nameof(this.Canonical)];

    public IHtmlLinkElement Prev => this.LinkElements[nameof(this.Prev)];

    public IHtmlLinkElement Next => this.LinkElements[nameof(this.Next)];

    public IHtmlLinkElement Icon => this.LinkElements[nameof(this.Icon)];

    public IHtmlLinkElement IconWithSizes => this.LinkElements[nameof(this.IconWithSizes)];

    public IHtmlLinkElement AlternateWithType => this.LinkElements[nameof(this.AlternateWithType)];

    public IHtmlLinkElement AlternateWithMedia => this.LinkElements[nameof(this.AlternateWithMedia)];

    public IHtmlLinkElement Preload => this.LinkElements[nameof(this.Preload)];

    public IHtmlLinkElement PreloadWithMedia => this.LinkElements[nameof(this.PreloadWithMedia)];

    public IHtmlLinkElement PreloadWithType => this.LinkElements[nameof(this.PreloadWithType)];

    public IHtmlLinkElement Unknown => this.LinkElements[nameof(this.Unknown)];

    public LinkElementsSet()
    {
        this.LinkElements = new Dictionary<string, IHtmlLinkElement>
        {
            [nameof(this.Canonical)] = CreateLinkElement(@"<link rel=""canonical"" href=""https://example.com/"" />"),
            [nameof(this.Prev)] = CreateLinkElement(@"<link rel=""prev"" href=""https://example.com/prev"" />"),
            [nameof(this.Next)] = CreateLinkElement(@"<link rel=""next"" href=""https://example.com/next"" />"),

            [nameof(this.Icon)] = CreateLinkElement(@"<link rel=""icon"" href=""https://example.com/favicon.ico"" />"),
            [nameof(this.IconWithSizes)] = CreateLinkElement(@"<link rel=""icon"" href=""https://example.com/favicon16blue.png"" sizes=""16"" />"),

            [nameof(this.AlternateWithType)] = CreateLinkElement(@"<link rel=""alternate"" href=""https://example.com/index.pdf"" type=""application/pdf"" />"),
            [nameof(this.AlternateWithMedia)] = CreateLinkElement(@"<link rel=""alternate"" href=""https://example.com/mobile.html"" media=""(min-width: 600px)"" />"),

            [nameof(this.Preload)] = CreateLinkElement(@"<link rel=""preload"" href=""https://example.com/foo.css"" />"),
            [nameof(this.PreloadWithMedia)] = CreateLinkElement(@"<link rel=""preload"" href=""https://example.com/bar.js"" media=""(min-width: 600px)"" />"),
            [nameof(this.PreloadWithType)] = CreateLinkElement(@"<link rel=""preload"" href=""https://example.com/fizz.woff2"" as=""font"" type=""font/woff2"" />"),

            [nameof(this.Unknown)] = CreateLinkElement(@"<link rel=""unknown"" href=""https://example.com/"" />"),
        };
    }

    public void ForEach(Action<IHtmlLinkElement> action)
    {
        foreach (var linkElement in this.LinkElements.Values)
        {
            action.Invoke(linkElement);
        }
    }

    private static IHtmlLinkElement CreateLinkElement(string content)
    {
        var parser = new HtmlParser();
        using var doc = parser.ParseDocument(
            "<html><head>" +
            content +
            "</head></html>");
        var link = doc.QuerySelector("link") as IHtmlLinkElement;
        if (link == null) throw new Exception("link element was not found.");
        return link;
    }

}
