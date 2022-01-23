using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
#if ENABLE_JSMODULE
        , IAsyncDisposable
#endif
    {
        private delegate ValueTask<T> ScriptInvoker<T>(string identifier, params object[] args);

        internal readonly HeadElementHelperServiceOptions Options = new HeadElementHelperServiceOptions();

        private readonly IJSRuntime _JS;

        private readonly NavigationManager _NavigationManager;

        private readonly IHeadElementHelperStore _Store;

        private const string NSM = "Toolbelt.Head.MetaTag.";

        private const string NSL = "Toolbelt.Head.LinkTag.";

        private const string NST = "Toolbelt.Head.Title.";

        private bool _ScriptEnabled = false;

        private bool _DetachedLocationChangedHandler = false;

        private readonly SemaphoreSlim _EnsureScriptSyncer = new(1, 1);

        private readonly SemaphoreSlim _ResetSyncer = new(1, 1);

        private readonly SemaphoreSlim _Syncer = new(1, 1);

        internal HeadElementHelperService(IServiceProvider services)
        {
            this._JS = services.GetRequiredService<IJSRuntime>();
            this._NavigationManager = services.GetRequiredService<NavigationManager>();
            this._Store = services.GetRequiredService<IHeadElementHelperStore>();
            this._NavigationManager.LocationChanged += this._NavigationManager_LocationChanged;
        }

        private string GetVersionText()
        {
            var assembly = this.GetType().Assembly;
            var version = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? assembly.GetName().Version?.ToString() ?? "";
            return version;
        }

#if ENABLE_JSMODULE
        private IJSObjectReference? _JSModule = null;

        private async ValueTask<ScriptInvoker<T>?> EnsureScriptEnabledAsync<T>()
        {
            if (this._JS == null) return null;
            if (!this._ScriptEnabled)
            {
                await this._EnsureScriptSyncer.WaitAsync();
                try
                {
                    if (!this._ScriptEnabled)
                    {
                        if (!this.Options.DisableClientScriptAutoInjection)
                        {
                            var scriptPath = "./_content/Toolbelt.Blazor.HeadElement.Services/script.module.min.js";

                            // Add version string for refresh token only when navigator is online.
                            // (If the app runs on the offline mode, the module url with query parameters might cause the "resource not found" error.)
                            const string moduleScript = "export function isOnLine(){ return navigator.onLine; }";
                            await using var inlineJsModule = await this._JS.InvokeAsync<IJSObjectReference>("import", "data:text/javascript;charset=utf-8," + Uri.EscapeDataString(moduleScript));
                            var isOnLine = await inlineJsModule.InvokeAsync<bool>("isOnLine");

                            if (isOnLine) scriptPath += $"?v={this.GetVersionText()}";

                            this._JSModule = await this._JS.InvokeAsync<IJSObjectReference>("import", scriptPath);
                        }
                        else
                        {
                            try
                            {
                                const string moduleScript = "export function ready(){ return Toolbelt.Blazor.HotKeys.ready; }";
                                await using var inlineJsModule = await this._JS.InvokeAsync<IJSObjectReference>("import", "data:text/javascript;charset=utf-8," + Uri.EscapeDataString(moduleScript));

                                await inlineJsModule.InvokeVoidAsync("ready");
                            }
                            catch { }
                        }
                        this._ScriptEnabled = true;
                    }
                }
                catch { }
                finally { this._EnsureScriptSyncer.Release(); }
            }

            return
                this._ScriptEnabled == false ? null :
                this._JSModule != null ? this._JSModule.InvokeAsync<T> :
                this._JS != null ? this._JS.InvokeAsync<T> : null;
        }
#else
        private async ValueTask<ScriptInvoker<T>?> EnsureScriptEnabledAsync<T>()
        {
            if (this._JS == null) return null;
            if (!this._ScriptEnabled)
            {
                await this._EnsureScriptSyncer.WaitAsync();
                try
                {
                    if (!this._ScriptEnabled)
                    {
                        if (!this.Options.DisableClientScriptAutoInjection)
                        {
                            // Add version string for refresh token only when navigator is online.
                            // (If the app runs on the offline mode, the module url with query parameters might cause the "resource not found" error.)
                            var jsInProcRuntime = this._JS as IJSInProcessRuntime;
                            var isOnLine = jsInProcRuntime?.Invoke<bool>("eval", "navigator.onLine") ?? false;
                            var versionQuery = isOnLine ? $"?v={this.GetVersionText()}" : "";

                            var scriptPath = "./_content/Toolbelt.Blazor.HeadElement.Services/script.min.js";
                            await this._JS.InvokeVoidAsync("eval", "new Promise(r=>((d,t,s,v)=>(h=>h.querySelector(t+`[src^=\"${s}\"]`)?r():(e=>(e.src=(s+v),e.onload=r,h.appendChild(e)))(d.createElement(t)))(d.head))(document,'script','" + scriptPath + "','" + versionQuery + "'))");
                        }
                        try { await this._JS.InvokeVoidAsync("eval", "Toolbelt.Head.ready"); } catch { }
                        this._ScriptEnabled = true;
                    }
                }
                catch { }
                finally { this._EnsureScriptSyncer.Release(); }
            }
            return
                this._ScriptEnabled == false ? null :
                this._JS != null ? this._JS.InvokeAsync<T> :
                null;
        }
#endif

        private async ValueTask<T?> InvokeJSAsync<T>(string identifier, params object[] args)
        {
            var invoker = await this.EnsureScriptEnabledAsync<T>();
            if (invoker == null) return default;
            return await invoker.Invoke(identifier, args);
        }

        private void _NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
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

        public ValueTask<string> GetTitleAsync() => new(this._Store.Title ?? "");

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
            await this.InvokeJSAsync<object>(NST + "set", this._Store.DefaultTitle ?? "");
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
            return this._Store.DefaultTitle ?? "";
        }

        public async ValueTask<IEnumerable<MetaElement>> GetDefaultMetaElementsAsync()
        {
            if (this._Store.DefaultMetaElements == null)
            {
                this._Store.DefaultMetaElements = await this.InvokeJSAsync<MetaElement[]>(NSM + "query");
            }
            return this._Store.DefaultMetaElements ?? Enumerable.Empty<MetaElement>();
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
            await this.InvokeJSAsync<object>(NSM + "reset", this._Store.DefaultMetaElements ?? Enumerable.Empty<MetaElement>());
        }

        public async ValueTask<IEnumerable<LinkElement>> GetDefaultLinkElementsAsync()
        {
            if (this._Store.DefaultLinkElements == null)
            {
                this._Store.DefaultLinkElements = await this.InvokeJSAsync<LinkElement[]>(NSL + "query");
            }
            return this._Store.DefaultLinkElements ?? Enumerable.Empty<LinkElement>();
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
            await this.InvokeJSAsync<object>(NSL + "reset", this._Store.DefaultLinkElements ?? Enumerable.Empty<LinkElement>());
        }

        private void DetachLocationChangedHandler()
        {
            if (this._DetachedLocationChangedHandler) return;

            this._NavigationManager.LocationChanged -= this._NavigationManager_LocationChanged;
            this._DetachedLocationChangedHandler = true;
        }

        public void Dispose()
        {
            this.DetachLocationChangedHandler();
        }

#if ENABLE_JSMODULE
        public async ValueTask DisposeAsync()
        {
            this.DetachLocationChangedHandler();
            if (this._JSModule != null) { await this._JSModule.DisposeAsync(); }
        }
#endif
    }
}
