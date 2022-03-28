using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Http;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.HeadElement.Middlewares
{
    public class HeadElementServerPrerenderingMiddleware
    {
        private readonly RequestDelegate _next;

        public HeadElementServerPrerenderingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, IHeadElementHelperStore store)
        {
            var filter = new FilterStream(context, store);
            try
            {
                await this._next(context);

                if (filter.IsCaptured())
                {
                    filter.MemoryStream.Seek(0, SeekOrigin.Begin);
                    var parser = new HtmlParser();
                    using var doc = parser.ParseDocument(filter.MemoryStream);

                    var indentText = doc.Head?.Descendents<IText>()
                        .Select(t => t.Data.Trim('\r', '\n'))
                        .Where(t => Regex.IsMatch(t, @"^[\t ]+$"))
                        .OrderByDescending(t => t.Length)
                        .FirstOrDefault() ?? "";

                    SetDocumentTitle(store, doc, indentText);
                    this.SetMetaElements(store, doc, indentText);
                    this.SetLinkElements(store, doc, indentText);

                    doc.Head?.AppendChild(doc.CreateTextNode("\n"));

                    filter.MemoryStream.SetLength(0);
                    var encoding = Encoding.UTF8;
                    using var writer = new StreamWriter(filter.MemoryStream, bufferSize: -1, leaveOpen: true, encoding: encoding) { AutoFlush = true };
                    // WORKAROUND: The HtmlMarkupFormatter class in AngleSharp doesn't append slash for closing tag, so I implemented custom version of it.
                    doc.ToHtml(writer, new CustomHtmlMarkupFormatter());

                    filter.MemoryStream.Seek(0, SeekOrigin.Begin);
                    await filter.MemoryStream.CopyToAsync(filter.OriginalStream);
                }
            }
            finally
            {
                filter.RevertResponseBodyHooking();
            }
        }

        private static void SetDocumentTitle(IHeadElementHelperStore store, IHtmlDocument doc, string indentText)
        {
            if (store.Title == null) return;

            store.DefaultTitle = doc.Title ?? "";
            doc.Title = store.Title;

            SaveDefault(doc, indentText, store.DefaultTitle, "text/default-title");
        }

        private void SetMetaElements(IHeadElementHelperStore store, IHtmlDocument doc, string indentText)
        {
            if (store.MetaElementCommands.Count == 0) return;

            var metaTags = doc.Head?.QuerySelectorAll("meta[name],meta[property],meta[http-equiv]").Cast<IHtmlMetaElement>().ToList() ?? new List<IHtmlMetaElement>();
            SaveDefault(doc, indentText, metaTags);

            foreach (var cmd in store.MetaElementCommands)
            {
                var e = (cmd.Element ??= new MetaElement());
                var meta = metaTags.FirstOrDefault(m => SameMeta(m, e));

                if (cmd.Operation == MetaElementOperations.Set)
                {
                    if (meta == null)
                    {
                        meta = CreateAndAddToHead<IHtmlMetaElement>(doc, indentText);
                        metaTags.Add(meta);

                        if (cmd.Element.Name != "")
                            meta.Name = cmd.Element.Name;
                        if (cmd.Element.Property != "")
                            meta.SetAttribute("property", cmd.Element.Property);
                        if (cmd.Element.HttpEquiv != "")
                            meta.HttpEquivalent = cmd.Element.HttpEquiv;
                        if (cmd.Element.Media != "")
                            meta.SetAttribute("media", cmd.Element.Media);
                    }
                    meta.Content = cmd.Element.Content;
                }
                else if (cmd.Operation == MetaElementOperations.Remove)
                {
                    if (meta != null)
                    {
                        doc.Head?.RemoveChild(meta);
                        metaTags.Remove(meta);
                    }
                }
            }
        }

        private void SetLinkElements(IHeadElementHelperStore store, IHtmlDocument doc, string indentText)
        {
            if (store.LinkElementCommands.Count == 0) return;

            var linkTags = doc.Head?.QuerySelectorAll("link").Cast<IHtmlLinkElement>().ToList() ?? new List<IHtmlLinkElement>();
            SaveDefault(doc, indentText, linkTags);

            foreach (var cmd in store.LinkElementCommands)
            {
                var e = cmd.Element ?? new LinkElement();
                var link = linkTags.FirstOrDefault(m => SameLink(m, e));

                if (cmd.Operation == LinkElementOperations.Set)
                {
                    if (link == null)
                    {
                        link = CreateAndAddToHead<IHtmlLinkElement>(doc, indentText);
                        linkTags.Add(link);
                    }
                    link.Relation = e.Rel;
                    link.Href = e.Href;
                    foreach (var (name, value) in new[] {
                        ("sizez", e.Sizes),
                        ("type", e.Type),
                        ("title", e.Title),
                        ("media", e.Media),
                        ("as", e.As),
                        ("crossorigin", e.CrossOrigin),
                        ("hreflang", e.Hreflang),
                        ("imagesizes", e.ImageSizes),
                        ("imagesrcset", e.ImageSrcset),
                    })
                    {
                        if (string.IsNullOrEmpty(value)) link.RemoveAttribute(name);
                        else link.SetAttribute(name, value);
                    }
                    link.IsDisabled = e.Disabled;
                }
                else if (cmd.Operation == LinkElementOperations.Remove && link != null)
                {
                    doc.Head?.RemoveChild(link);
                    linkTags.Remove(link);
                }
            }
        }

        internal static bool SameMeta(IHtmlMetaElement m, MetaElement a)
        {
            return
                ((m.GetAttribute("media") ?? "") == a.Media) &&

                ((a.Name != "" && a.Name == m.Name) ||
                (a.Property != "" && a.Property == m.GetAttribute("property")) ||
                (a.HttpEquiv != "" && a.HttpEquiv == m.HttpEquivalent));
        }

        internal static bool SameLink(IHtmlLinkElement m, LinkElement a)
        {
            return m.Relation == a.Rel && (a.Rel switch
            {
                "canonical" => true,
                "prev" => true,
                "next" => true,
                "icon" => (m.Sizes?.ToString() ?? "") == (a.Sizes ?? ""),
                "alternate" => (m.Type ?? "") == (a.Type ?? "") && (m.Media ?? "") == (a.Media ?? "") && ((m.GetAttribute("hreflang") ?? "") == (a.Hreflang ?? "")),
                "preload" => Href(m.Href) == (a.Href ?? "") && (m.Media ?? "") == (a.Media ?? ""),
                _ => Href(m.Href) == (a.Href ?? "")
            });
        }

        private static TElement CreateAndAddToHead<TElement>(IHtmlDocument doc, string indentText) where TElement : IElement
        {
            doc.Head?.AppendChild(doc.CreateTextNode("\n" + indentText));
            var element = doc.CreateElement<TElement>();
            doc.Head?.AppendChild(element);
            return element;
        }

        private static string Href(string? href) { return Regex.Replace(href ?? "", "^about:///", ""); }

        private static string Stringify(object? obj)
        {
            if (obj == null) return "";
            else if (obj is string str)
                return string.IsNullOrEmpty(str) ? "" : JsonSerializer.Serialize(str ?? "");
            else if (obj is bool b)
                return b ? "!0" : "";
            else return obj.ToString() ?? "";
        }

        private static void SaveDefault(IHtmlDocument doc, string indentText, IEnumerable<IHtmlLinkElement> linkTags)
        {
            SaveDefault(doc, indentText, linkTags, "text/default-link-elements", m => new[] {
                Stringify(m.Relation),
                Stringify(Href(m.Href)),
                Stringify(m.Sizes?.ToString()),
                Stringify(m.Type ),
                Stringify(m.Title),
                Stringify(m.Media),
                Stringify(m.GetAttribute("as")),
                Stringify(m.CrossOrigin),
                Stringify(m.GetAttribute("hreflang")),
                Stringify(m.GetAttribute("imagesizes")),
                Stringify(m.GetAttribute("imagesrcset")),
                Stringify(m.IsDisabled)
            });
        }

        private static void SaveDefault(IHtmlDocument doc, string indentText, IEnumerable<IHtmlMetaElement> metaTags)
        {
            SaveDefault(doc, indentText, metaTags, "text/default-meta-elements", m => new[] {
                Stringify(m.GetAttribute("property")),
                Stringify(m.Name),
                Stringify(m.HttpEquivalent),
                Stringify(m.GetAttribute("media") ?? ""),
                Stringify(m.Content)
            });
        }

        private static void SaveDefault<T>(IHtmlDocument doc, string indentText, IEnumerable<T> elements, string saveName, Func<T, string[]> converter)
        {
            var serializedElements = elements.Select(m =>
            {
                var a = string.Join(",", converter(m));
                return "[" + a.TrimEnd(',') + "]";
            });
            var serializedText = "[" + string.Join(",", serializedElements) + "]";
            SaveDefault(doc, indentText, serializedText, saveName);
        }

        public static void SaveDefault(IHtmlDocument doc, string indentText, string text, string type)
        {
            var script = CreateAndAddToHead<IHtmlScriptElement>(doc, indentText);
            script.TextContent = text;
            script.SetAttribute("type", type);
            doc.Head?.AppendChild(script);
        }
    }
}
