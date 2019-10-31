using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

                filter.MemoryStream.SetLength(0);
                var encoding = Encoding.UTF8;
                using var writer = new StreamWriter(filter.MemoryStream, bufferSize: -1, leaveOpen: true, encoding: encoding) { AutoFlush = true };
                doc.ToHtml(writer, new PrettyMarkupFormatter());

                filter.MemoryStream.Seek(0, SeekOrigin.Begin);
                await filter.MemoryStream.CopyToAsync(filter.OriginalStream);
            }
        }

        private static void SetDocumentTitle(IHeadElementHelperStore store, AngleSharp.Html.Dom.IHtmlDocument doc)
        {
            if (store.Title == null) return;

            store.DefaultTitle = doc.Title ?? "";
            doc.Title = store.Title;

            var script = doc.CreateElement("script");
            script.TextContent = store.DefaultTitle;
            script.SetAttribute("type", "text/default-title");
            doc.Head.AppendChild(script);
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
            var script = doc.CreateElement("script");
            script.TextContent = JsonSerializer.Serialize(metaElements);
            script.SetAttribute("type", "text/default-meta-elements");
            doc.Head.AppendChild(script);

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
    }
}
