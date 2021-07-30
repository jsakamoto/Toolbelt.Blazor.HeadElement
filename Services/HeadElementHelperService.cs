using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.HeadElement
{
    public class HeadElementHelperService : IHeadElementHelper, IDisposable
    {
        internal readonly HeadElementHelperServiceOptions Options = new HeadElementHelperServiceOptions();

        private readonly IJSRuntime _JS;

        private readonly NavigationManager _NavigationManager;

        private readonly IHeadElementHelperStore _Store;

        private const string NSM = "Toolbelt.Head.MetaTag.";

        private const string NSL = "Toolbelt.Head.LinkTag.";

        private const string NST = "Toolbelt.Head.Title.";

        private bool _ScriptEnabled = false;

        private readonly SemaphoreSlim _EnsureScriptSyncer = new(1, 1);

        private readonly SemaphoreSlim _ResetSyncer = new(1, 1);

        private readonly SemaphoreSlim _Syncer = new(1, 1);

        internal HeadElementHelperService(IServiceProvider services)
        {
            this._JS = services.GetService<IJSRuntime>();
            this._NavigationManager = services.GetRequiredService<NavigationManager>();
            this._Store = services.GetRequiredService<IHeadElementHelperStore>();
            this._NavigationManager.LocationChanged += this._NavigationManager_LocationChanged;
        }

        public void Dispose()
        {
            this._NavigationManager.LocationChanged -= this._NavigationManager_LocationChanged;
        }

        private async ValueTask<bool> EnsureScriptEnabledAsync()
        {
            if (this._ScriptEnabled || this._JS == null) return this._ScriptEnabled;

            await this._EnsureScriptSyncer.WaitAsync();
            try
            {
                if (this._ScriptEnabled) return this._ScriptEnabled;

                var version = this.GetType().Assembly.GetName().Version;
                var script = this.Options.DisableClientScriptAutoInjection ? "0" :
                    $"new Promise(r=>((d,t,s)=>(h=>h.querySelector(t+`[src=\"${{s}}\"]`)?r():(e=>(e.src=s,e.onload=r,h.appendChild(e)))(d.createElement(t)))(d.head))(document,'script','_content/Toolbelt.Blazor.HeadElement.Services/script.min.js?v={version}'))";
                await this._JS.InvokeVoidAsync("eval", script);

                this._ScriptEnabled = true;
            }
            catch { }
            finally { this._EnsureScriptSyncer.Release(); }
            return this._ScriptEnabled;
        }

        private async ValueTask<T> InvokeJSAsync<T>(string identifier, params object[] args)
        {
            if (await this.EnsureScriptEnabledAsync())
            {
                return await this._JS.InvokeAsync<T>(identifier, args);
            }
            return default;
        }

        private void _NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            this.ResetIfNeededAsync().ConfigureAwait(false);
        }

        private async ValueTask ResetIfNeededAsync()
        {
            await this._ResetSyncer.WaitAsync();
            try
            {
                await this.GetDefaultsAsync();

                if (this._Store.UrlLastSet == this.NormalizeUrl(this._NavigationManager.Uri)) return;

                var urlLastSet = this._Store.UrlLastSet;
                var normalizeUrl = this.NormalizeUrl(this._NavigationManager.Uri);

                this._Store.MetaElementCommands.Clear();
                this._Store.LinkElementCommands.Clear();

                if (this._Store.DefaultTitle != null)
                {
                    await this.ResetTitleAsync();
                }
                if (this._Store.DefaultMetaElements != null)
                {
                    await this.ResetMetaElementsAsync();
                }
                if (this._Store.DefaultLinkElements != null)
                {
                    await this.ResetLinkElementsAsync();
                }

                this._Store.UrlLastSet = this.NormalizeUrl(this._NavigationManager.Uri);
            }
            finally { this._ResetSyncer.Release(); }
        }

        private async ValueTask GetDefaultsAsync()
        {
            await this.GetDefaultTitleAsync();
            await this.GetDefaultMetaElementsAsync();
            await this.GetDefaultLinkElementsAsync();
        }

        public ValueTask<string> GetTitleAsync() => new(this._Store.Title);

        public async ValueTask SetTitleAsync(string title)
        {
            await this._Syncer.WaitAsync();
            try
            {
                await this.ResetIfNeededAsync();
                this._Store.Title = title;
                await this.InvokeJSAsync<object>(NST + "set", title);
            }
            finally { this._Syncer.Release(); }
        }

        private async ValueTask ResetTitleAsync()
        {
            this._Store.Title = this._Store.DefaultTitle;
            await this.InvokeJSAsync<object>(NST + "set", this._Store.DefaultTitle);
        }

        private string NormalizeUrl(string uri)
        {
            var absoluteUri = this._NavigationManager.ToAbsoluteUri(uri);
            return absoluteUri.AbsolutePath.TrimEnd('/') + absoluteUri.Query;
        }

        public ValueTask SetDefaultTitleAsync(string defaultTitle)
        {
            this._Store.DefaultTitle = defaultTitle;
            return new ValueTask();
        }

        public async ValueTask<string> GetDefaultTitleAsync()
        {
            if (this._Store.DefaultTitle == null)
            {
                this._Store.DefaultTitle = await this.InvokeJSAsync<string>(NST + "query");
            }
            return this._Store.DefaultTitle;
        }

        public async ValueTask<IEnumerable<MetaElement>> GetDefaultMetaElementsAsync()
        {
            if (this._Store.DefaultMetaElements == null)
            {
                this._Store.DefaultMetaElements = await this.InvokeJSAsync<MetaElement[]>(NSM + "query");
            }
            return this._Store.DefaultMetaElements;
        }

        public async ValueTask SetMetaElementsAsync(params MetaElement[] elements)
        {
            await this._Syncer.WaitAsync();
            try
            {
                await this.ResetIfNeededAsync();
                await this.InvokeJSAsync<object>(NSM + "set", new object[] { elements });

                var commands = elements.Select(elem => new MetaElementCommand { Operation = MetaElementOperations.Set, Element = elem });
                foreach (var command in commands)
                {
                    this._Store.MetaElementCommands.Add(command);
                }
            }
            finally { this._Syncer.Release(); }
        }

        public async ValueTask RemoveMetaElementsAsync(params MetaElement[] elements)
        {
            await this._Syncer.WaitAsync();
            try
            {
                await this.ResetIfNeededAsync();
                await this.InvokeJSAsync<object>(NSM + "del", new object[] { elements });

                var commands = elements.Select(elem => new MetaElementCommand { Operation = MetaElementOperations.Remove, Element = elem });
                foreach (var command in commands)
                {
                    this._Store.MetaElementCommands.Add(command);
                }
            }
            finally { this._Syncer.Release(); }
        }

        private async ValueTask ResetMetaElementsAsync()
        {
            await this.InvokeJSAsync<object>(NSM + "reset", this._Store.DefaultMetaElements);
        }

        public async ValueTask<IEnumerable<LinkElement>> GetDefaultLinkElementsAsync()
        {
            if (this._Store.DefaultLinkElements == null)
            {
                this._Store.DefaultLinkElements = await this.InvokeJSAsync<LinkElement[]>(NSL + "query");
            }
            return this._Store.DefaultLinkElements;
        }

        public async ValueTask SetLinkElementsAsync(params LinkElement[] elements)
        {
            await this._Syncer.WaitAsync();
            try
            {
                await this.ResetIfNeededAsync();
                await this.InvokeJSAsync<object>(NSL + "set", new object[] { elements });

                var commands = elements.Select(elem => new LinkElementCommand { Operation = LinkElementOperations.Set, Element = elem });
                foreach (var command in commands)
                {
                    this._Store.LinkElementCommands.Add(command);
                }
            }
            finally { this._Syncer.Release(); }
        }

        public async ValueTask RemoveLinkElementsAsync(params LinkElement[] elements)
        {
            await this._Syncer.WaitAsync();
            try
            {
                await this.ResetIfNeededAsync();
                await this.InvokeJSAsync<object>(NSL + "del", new object[] { elements });

                var commands = elements.Select(elem => new LinkElementCommand { Operation = LinkElementOperations.Remove, Element = elem });
                foreach (var command in commands)
                {
                    this._Store.LinkElementCommands.Add(command);
                }
            }
            finally { this._Syncer.Release(); }
        }

        private async ValueTask ResetLinkElementsAsync()
        {
            await this.InvokeJSAsync<object>(NSL + "reset", this._Store.DefaultLinkElements);
        }
    }
}
