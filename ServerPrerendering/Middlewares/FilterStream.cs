using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.HeadElement.Middlewares
{
    internal delegate Task WriteAsyncInvoker(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

    internal delegate Task FlushAsyncInvoker(CancellationToken cancellationToken);

    internal class FilterStream : Stream
    {
        private readonly HttpContext HttpContext;

        private readonly IHeadElementHelperStore InternalStore;

        internal readonly MemoryStream MemoryStream;

        internal readonly Stream OriginalStream;

        private bool _IsCaptured;

        private WriteAsyncInvoker WriteAsyncInvoker;

        private FlushAsyncInvoker FlushAsyncInvoker;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanTimeout => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public FilterStream(HttpContext httpContext, IHeadElementHelperStore internalStore)
        {
            this.HttpContext = httpContext;
            this.InternalStore = internalStore;
            this.MemoryStream = new MemoryStream();
            this.OriginalStream = this.HttpContext.Response.Body;
            this.HttpContext.Response.Body = this;

            this.WriteAsyncInvoker = DefaultWriteAsyncInvoker;
            this.FlushAsyncInvoker = DefaultFlushAsyncInvoker;
        }

        public void RevertResponseBodyHooking()
        {
            this.HttpContext.Response.Body = this.OriginalStream;
        }

        private Task DefaultWriteAsyncInvoker(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return RebindInvokers().WriteAsync(buffer, offset, count, cancellationToken);
        }

        private Task DefaultFlushAsyncInvoker(CancellationToken cancellationToken)
        {
            return RebindInvokers().FlushAsync(cancellationToken);
        }

        private Stream RebindInvokers()
        {
            _IsCaptured = (HttpContext.Response.ContentType?.StartsWith("text/html") == true && (
                InternalStore.Title != null ||
                InternalStore.MetaElementCommands.Count > 0 ||
                InternalStore.LinkElementCommands.Count > 0
            ));
            var stream = _IsCaptured ? MemoryStream : OriginalStream;
            this.WriteAsyncInvoker = stream.WriteAsync;
            this.FlushAsyncInvoker = stream.FlushAsync;
            return stream;
        }

        internal bool IsCaptured() => _IsCaptured;

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return WriteAsyncInvoker(buffer, offset, count, cancellationToken);
        }

        public override void Flush() => throw new NotSupportedException();

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return FlushAsyncInvoker(cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.MemoryStream.Dispose();
        }
    }
}
