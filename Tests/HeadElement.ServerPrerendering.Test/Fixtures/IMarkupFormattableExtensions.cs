using AngleSharp;

namespace HeadElement.ServerPrerendering.Test.Fixtures;

public static class IMarkupFormattableExtensions
{
    public static string ToHtml(this IMarkupFormattable value)
    {
        return value.ToHtml(new AngleSharp.Html.MinifyMarkupFormatter());
    }
}
