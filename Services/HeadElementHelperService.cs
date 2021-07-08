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
            this._NavigationManager.LocationChanged += _NavigationManager_LocationChanged;
        }

        public void Dispose()
        {
            _NavigationManager.LocationChanged -= _NavigationManager_LocationChanged;
        }

        private async ValueTask<bool> EnsureScriptEnabledAsync()
        {
            if (_ScriptEnabled || _JS == null) return _ScriptEnabled;

            await _EnsureScriptSyncer.WaitAsync();
            try
            {
                if (_ScriptEnabled) return _ScriptEnabled;

                var script = Options.DisableClientScriptAutoInjection ? "0" :
                    $"new Promise(r=>((d,t,s)=>(h=>h.querySelector(t+`[src=\"${{s}}\"]`)?r():(e=>(e.src=s,e.onload=r,h.appendChild(e)))(d.createElement(t)))(d.head))(document,'script','_content/Toolbelt.Blazor.HeadElement.Services/script.min.js'))";
                await _JS.InvokeVoidAsync("eval", script);

                _ScriptEnabled = true;
            }
            catch { }
            finally { _EnsureScriptSyncer.Release(); }
            return _ScriptEnabled;
        }

        private async ValueTask<T> InvokeJSAsync<T>(string identifier, params object[] args)
        {
            if (await EnsureScriptEnabledAsync())
            {
                return await _JS.InvokeAsync<T>(identifier, args);
            }
            return default;
        }

        private void _NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            ResetIfNeededAsync().ConfigureAwait(false);
        }

        private async ValueTask ResetIfNeededAsync()
        {
            await _ResetSyncer.WaitAsync();
            try
            {
                await GetDefaultsAsync();

                if (_Store.UrlLastSet == NormalizeUrl(_NavigationManager.Uri)) return;

                var urlLastSet = _Store.UrlLastSet;
                var normalizeUrl = NormalizeUrl(_NavigationManager.Uri);

                _Store.MetaElementCommands.Clear();
                _Store.LinkElementCommands.Clear();

                if (_Store.DefaultTitle != null)
                {
                    await ResetTitleAsync();
                }
                if (_Store.DefaultMetaElements != null)
                {
                    await ResetMetaElementsAsync();
                }
                if (_Store.DefaultLinkElements != null)
                {
                    await ResetLinkElementsAsync();
                }

                _Store.UrlLastSet = NormalizeUrl(_NavigationManager.Uri);
            }
            finally { _ResetSyncer.Release(); }
        }

        private async ValueTask GetDefaultsAsync()
        {
            await GetDefaultTitleAsync();
            await GetDefaultMetaElementsAsync();
            await GetDefaultLinkElementsAsync();
        }

        public ValueTask<string> GetTitleAsync() => new(_Store.Title);

        public async ValueTask SetTitleAsync(string title)
        {
            await _Syncer.WaitAsync();
            try
            {
                await ResetIfNeededAsync();
                _Store.Title = title;
                await InvokeJSAsync<object>(NST + "set", title);
            }
            finally { _Syncer.Release(); }
        }

        private async ValueTask ResetTitleAsync()
        {
            _Store.Title = _Store.DefaultTitle;
            await InvokeJSAsync<object>(NST + "set", _Store.DefaultTitle);
        }

        private string NormalizeUrl(string uri)
        {
            var absoluteUri = _NavigationManager.ToAbsoluteUri(uri);
            return absoluteUri.AbsolutePath.TrimEnd('/') + absoluteUri.Query;
        }

        public ValueTask SetDefaultTitleAsync(string defaultTitle)
        {
            _Store.DefaultTitle = defaultTitle;
            return new ValueTask();
        }

        public async ValueTask<string> GetDefaultTitleAsync()
        {
            if (_Store.DefaultTitle == null)
            {
                _Store.DefaultTitle = await InvokeJSAsync<string>(NST + "query");
            }
            return _Store.DefaultTitle;
        }

        public async ValueTask<IEnumerable<MetaElement>> GetDefaultMetaElementsAsync()
        {
            if (_Store.DefaultMetaElements == null)
            {
                _Store.DefaultMetaElements = await InvokeJSAsync<MetaElement[]>(NSM + "query");
            }
            return _Store.DefaultMetaElements;
        }

        public async ValueTask SetMetaElementsAsync(params MetaElement[] elements)
        {
            await _Syncer.WaitAsync();
            try
            {
                await ResetIfNeededAsync();
                await InvokeJSAsync<object>(NSM + "set", new object[] { elements });

                var commands = elements.Select(elem => new MetaElementCommand { Operation = MetaElementOperations.Set, Element = elem });
                foreach (var command in commands)
                {
                    _Store.MetaElementCommands.Add(command);
                }
            }
            finally { _Syncer.Release(); }
        }

        public async ValueTask RemoveMetaElementsAsync(params MetaElement[] elements)
        {
            await _Syncer.WaitAsync();
            try
            {
                await ResetIfNeededAsync();
                await InvokeJSAsync<object>(NSM + "del", new object[] { elements });

                var commands = elements.Select(elem => new MetaElementCommand { Operation = MetaElementOperations.Remove, Element = elem });
                foreach (var command in commands)
                {
                    _Store.MetaElementCommands.Add(command);
                }
            }
            finally { _Syncer.Release(); }
        }

        private async ValueTask ResetMetaElementsAsync()
        {
            await InvokeJSAsync<object>(NSM + "reset", _Store.DefaultMetaElements);
        }

        public async ValueTask<IEnumerable<LinkElement>> GetDefaultLinkElementsAsync()
        {
            if (_Store.DefaultLinkElements == null)
            {
                _Store.DefaultLinkElements = await InvokeJSAsync<LinkElement[]>(NSL + "query");
            }
            return _Store.DefaultLinkElements;
        }

        public async ValueTask SetLinkElementsAsync(params LinkElement[] elements)
        {
            await _Syncer.WaitAsync();
            try
            {
                await ResetIfNeededAsync();
                await InvokeJSAsync<object>(NSL + "set", new object[] { elements });

                var commands = elements.Select(elem => new LinkElementCommand { Operation = LinkElementOperations.Set, Element = elem });
                foreach (var command in commands)
                {
                    _Store.LinkElementCommands.Add(command);
                }
            }
            finally { _Syncer.Release(); }
        }

        public async ValueTask RemoveLinkElementsAsync(params LinkElement[] elements)
        {
            await _Syncer.WaitAsync();
            try
            {
                await ResetIfNeededAsync();
                await InvokeJSAsync<object>(NSL + "del", new object[] { elements });

                var commands = elements.Select(elem => new LinkElementCommand { Operation = LinkElementOperations.Remove, Element = elem });
                foreach (var command in commands)
                {
                    _Store.LinkElementCommands.Add(command);
                }
            }
            finally { _Syncer.Release(); }
        }

        private async ValueTask ResetLinkElementsAsync()
        {
            await InvokeJSAsync<object>(NSL + "reset", _Store.DefaultLinkElements);
        }
    }
}
