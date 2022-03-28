using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace HeadElement.ServerPrerendering.Test.Fixtures;

public class MetaElementsSet
{
    public IReadOnlyDictionary<string, IHtmlMetaElement> MetaElements { get; }

    public IHtmlMetaElement Description => this.MetaElements[nameof(this.Description)];

    public IHtmlMetaElement ContentType => this.MetaElements[nameof(this.ContentType)];

    public IHtmlMetaElement ThemeColorLight => this.MetaElements[nameof(this.ThemeColorLight)];

    public IHtmlMetaElement ThemeColorDark => this.MetaElements[nameof(this.ThemeColorDark)];

    public IHtmlMetaElement OgType => this.MetaElements[nameof(this.OgType)];

    public IHtmlMetaElement UnknownName => this.MetaElements[nameof(this.UnknownName)];


    public MetaElementsSet()
    {
        this.MetaElements = new Dictionary<string, IHtmlMetaElement>
        {
            [nameof(this.Description)] = CreateMetaElement(@"<meta name=""description"" content=""Hello, World."" />"),
            [nameof(this.ContentType)] = CreateMetaElement(@"<meta http-equiv=""content-type"" content=""text/html; charset=utf-8"" />"),
            [nameof(this.ThemeColorLight)] = CreateMetaElement(@"<meta name=""theme-color"" media=""(prefers-color-scheme: light)"" content=""white""/>"),
            [nameof(this.ThemeColorDark)] = CreateMetaElement(@"<meta name=""theme-color"" media=""(prefers-color-scheme: dark)"" content=""black""/>"),
            [nameof(this.OgType)] = CreateMetaElement(@"<meta property=""og:type"" content=""website""/>"),
            [nameof(this.UnknownName)] = CreateMetaElement(@"<meta name=""unknown"" content=""unknown content""/>"),
        };
    }

    public void ForEach(Action<IHtmlMetaElement> action)
    {
        foreach (var metaElement in this.MetaElements.Values)
        {
            action.Invoke(metaElement);
        }
    }

    private static IHtmlMetaElement CreateMetaElement(string content)
    {
        var parser = new HtmlParser();
        using var doc = parser.ParseDocument(
            "<html><head>" +
            content +
            "</head></html>");
        var meta = doc.QuerySelector("meta") as IHtmlMetaElement;
        if (meta == null) throw new Exception("A meta element was not found.");
        return meta;
    }
}
