using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html;
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
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IHeadElementHelperStore store)
        {
            var filter = new FilterStream(context, store);

            await _next(context);

            if (filter.IsCaptured())
            {
                filter.MemoryStream.Seek(0, SeekOrigin.Begin);
                var parser = new HtmlParser();
                using var doc = parser.ParseDocument(filter.MemoryStream);

                SetDocumentTitle(store, doc);
                SetMetaElements(store, doc);
                SetLinkElements(store, doc);

                filter.MemoryStream.SetLength(0);
                var encoding = Encoding.UTF8;
                using var writer = new StreamWriter(filter.MemoryStream, bufferSize: -1, leaveOpen: true, encoding: encoding) { AutoFlush = true };
                doc.ToHtml(writer, new PrettyMarkupFormatter());

                filter.MemoryStream.Seek(0, SeekOrigin.Begin);
                await filter.MemoryStream.CopyToAsync(filter.OriginalStream);
            }
        }

        public static void SaveDefault(IHtmlDocument doc, string text, string type)
        {
            var script = doc.CreateElement("script");
            script.TextContent = text;
            script.SetAttribute("type", type);
            doc.Head.AppendChild(script);
        }

        public static void SaveDefault<T>(IHtmlDocument doc, T defaultElements, string type)
        {
            SaveDefault(doc, JsonSerializer.Serialize(defaultElements), type);
        }

        private static void SetDocumentTitle(IHeadElementHelperStore store, AngleSharp.Html.Dom.IHtmlDocument doc)
        {
            if (store.Title == null) return;

            store.DefaultTitle = doc.Title ?? "";
            doc.Title = store.Title;

            SaveDefault(doc, store.DefaultTitle, "text/default-title");
        }

        private void SetMetaElements(IHeadElementHelperStore store, IHtmlDocument doc)
        {
            if (store.MetaElementCommands.Count == 0) return;

            var metaTags = doc.Head.QuerySelectorAll("meta[name],meta[property]").Cast<IHtmlMetaElement>().ToList();

            var metaElements = metaTags.Select(m => new MetaElement
            {
                Name = m.Name ?? "",
                Property = m.GetAttribute("property") ?? "",
                Content = m.Content
            });
            SaveDefault(doc, metaElements, "text/default-meta-elements");

            foreach (var cmd in store.MetaElementCommands)
            {
                var meta = metaTags.FirstOrDefault(m => (cmd.Element.Name != "" && cmd.Element.Name == m.Name) || (cmd.Element.Property != "" && cmd.Element.Property == m.GetAttribute("property")));

                if (cmd.Operation == MetaElementOperations.Set)
                {
                    if (meta == null)
                    {
                        meta = doc.CreateElement("meta") as IHtmlMetaElement;
                        if (cmd.Element.Name != "")
                            meta.Name = cmd.Element.Name;
                        if (cmd.Element.Property != "")
                            meta.SetAttribute("property", cmd.Element.Property);
                        doc.Head.AppendChild(meta);
                        metaTags.Add(meta);
                    }
                    meta.Content = cmd.Element.Content;
                }
                else if (cmd.Operation == MetaElementOperations.Remove)
                {
                    if (meta != null)
                    {
                        doc.Head.RemoveChild(meta);
                        metaTags.Remove(meta);
                    }
                }
            }
        }


        private void SetLinkElements(IHeadElementHelperStore store, IHtmlDocument doc)
        {
            static string Href(string href) { return Regex.Replace(href ?? "", "^about:///", ""); }
            static bool SameLink(IHtmlLinkElement m, LinkElement a)
            {
                return m.Relation == a.Rel && (
                    (new[] { "canonical", "prev", "next" }.Contains(a.Rel)) ||
                    (a.Rel == "icon" && (m.Sizes?.ToString() ?? "") == a.Sizes) ||
                    (a.Rel == "alternate" && m.Type == a.Type && m.Media == a.Media) ||
                    (Href(m.Href) == a.Href)
                );
            };

            if (store.LinkElementCommands.Count == 0) return;

            var linkTags = doc.Head.QuerySelectorAll("link").Cast<IHtmlLinkElement>().ToList();

            var linkElements = linkTags.Select(m => new LinkElement
            {
                Rel = m.Relation ?? "",
                Href = Href(m.Href),
                Sizes = m.Sizes?.ToString() ?? "",
                Type = m.Type ?? "",
                Title = m.Title ?? "",
                Media = m.Media ?? ""
            });
            SaveDefault(doc, linkElements, "text/default-link-elements");

            foreach (var cmd in store.LinkElementCommands)
            {
                var e = cmd.Element;
                var link = linkTags.FirstOrDefault(m => SameLink(m, e));

                if (cmd.Operation == LinkElementOperations.Set)
                {
                    if (link == null)
                    {
                        link = doc.CreateElement("link") as IHtmlLinkElement;
                        doc.Head.AppendChild(link);
                        linkTags.Add(link);
                    }
                    link.Relation = e.Rel;
                    link.Href = e.Href;
                    foreach (var prop in new[] { ("sizez", e.Sizes), ("type", e.Type), ("title", e.Title), ("media", e.Media) })
                    {
                        if (string.IsNullOrEmpty(prop.Item2)) link.RemoveAttribute(prop.Item1);
                        else link.SetAttribute(prop.Item1, prop.Item2);
                    }
                }
                else if (cmd.Operation == LinkElementOperations.Remove && link != null)
                {
                    doc.Head.RemoveChild(link);
                    linkTags.Remove(link);
                }
            }
        }
    }
}
