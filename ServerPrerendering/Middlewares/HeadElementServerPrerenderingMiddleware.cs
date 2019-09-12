using System.IO;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html;
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

        public async Task InvokeAsync(HttpContext context, IHeadElementHelperStore internalStore)
        {
            using var memStream = new MemoryStream();
            var orgStream = context.Response.Body;
            context.Response.Body = memStream;

            await _next(context);

            if (context.Response.ContentType.StartsWith("text/html") && internalStore.Title != null)
            {
                memStream.Seek(0, SeekOrigin.Begin);
                var parser = new HtmlParser();
                using var doc = parser.ParseDocument(memStream);
                internalStore.DefaultTitle = doc.Title ?? "";
                doc.Title = internalStore.Title;

                var script = doc.CreateElement("script");
                script.TextContent = internalStore.DefaultTitle;
                script.SetAttribute("type", "text/default-title");
                doc.Head.AppendChild(script);

                memStream.SetLength(0);
                var encoding = Encoding.UTF8;
                using var writer = new StreamWriter(memStream, bufferSize: -1, leaveOpen: true, encoding: encoding) { AutoFlush = true };
                doc.ToHtml(writer, new PrettyMarkupFormatter());
            }

            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(orgStream);
        }
    }
}
