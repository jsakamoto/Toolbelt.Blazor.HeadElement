using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Toolbelt.Blazor.HeadElement.Internals;
using Microsoft.Extensions.DependencyInjection;

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

        private SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

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

            await SemaphoreSlim.WaitAsync();
            try
            {
                if (_ScriptEnabled) return _ScriptEnabled;

                var script = Options.DisableClientScriptAutoInjection ? "0" :
                    $"new Promise(r=>((d,t,s)=>(h=>h.querySelector(t+`[src=\"${{s}}\"]`)?r():(e=>(e.src=s,e.onload=r,h.appendChild(e)))(d.createElement(t)))(d.head))(document,'script','_content/Toolbelt.Blazor.HeadElement.Services/script.min.js'))";
                await _JS.InvokeVoidAsync("eval", script);

                _ScriptEnabled = true;
            }
            catch { }
            finally { SemaphoreSlim.Release(); }
            return _ScriptEnabled;
        }

        private async ValueTask<T> InvokeJSAsync<T>(string identifier, params object[] args)
        {
            if (await EnsureScriptEnabledAsync())
            {
                return await _JS.InvokeAsync<T>(identifier, args);
            }
            return default(T);
        }

        public ValueTask SetTitleAsync(string title) => SetTitleCoreAsync(title, delay: true);

        public ValueTask<string> GetTitleAsync() => new ValueTask<string>(_Store.Title);

        private void _NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            _Store.MetaElementCommands.Clear();

            if (_Store.UrlLastSet != e.Location)
            {
                if (_Store.DefaultTitle != null)
                {
                    var _ = SetTitleCoreAsync(_Store.DefaultTitle, delay: false);
                }
                if (_Store.DefaultMetaElements != null)
                {
                    var _ = ResetMetaElementsAsync();
                }
                if (_Store.DefaultLinkElements != null)
                {
                    var _ = ResetLinkElementsAsync();
                }
            }
        }

        private async ValueTask SetTitleCoreAsync(string title, bool delay)
        {
            await GetDefaultTitleAsync();

            if (delay)
            {
                await Task.Delay(1);
            }

            _Store.Title = title;
            _Store.UrlLastSet = _NavigationManager.Uri;

            await InvokeJSAsync<object>(NST + "set", title);
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
            await GetDefaultMetaElementsAsync();
            await Task.Delay(1);
            await InvokeJSAsync<object>(NSM + "set", new object[] { elements });

            var commands = elements.Select(elem => new MetaElementCommand { Operation = MetaElementOperations.Set, Element = elem });
            foreach (var command in commands)
            {
                _Store.MetaElementCommands.Add(command);
            }
            _Store.UrlLastSet = _NavigationManager.Uri;
        }

        public async ValueTask RemoveMetaElementsAsync(params MetaElement[] elements)
        {
            await GetDefaultMetaElementsAsync();
            await Task.Delay(1);
            await InvokeJSAsync<object>(NSM + "del", new object[] { elements });

            var commands = elements.Select(elem => new MetaElementCommand { Operation = MetaElementOperations.Remove, Element = elem });
            foreach (var command in commands)
            {
                _Store.MetaElementCommands.Add(command);
            }
            _Store.UrlLastSet = _NavigationManager.Uri;
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
            await GetDefaultLinkElementsAsync();
            await Task.Delay(1);
            await InvokeJSAsync<object>(NSL + "set", new object[] { elements });

            var commands = elements.Select(elem => new LinkElementCommand { Operation = LinkElementOperations.Set, Element = elem });
            foreach (var command in commands)
            {
                _Store.LinkElementCommands.Add(command);
            }
            _Store.UrlLastSet = _NavigationManager.Uri;
        }


        public async ValueTask RemoveLinkElementsAsync(params LinkElement[] elements)
        {
            await GetDefaultLinkElementsAsync();
            await Task.Delay(1);
            await InvokeJSAsync<object>(NSL + "del", new object[] { elements });

            var commands = elements.Select(elem => new LinkElementCommand { Operation = LinkElementOperations.Remove, Element = elem });
            foreach (var command in commands)
            {
                _Store.LinkElementCommands.Add(command);
            }
            _Store.UrlLastSet = _NavigationManager.Uri;
        }

        private async ValueTask ResetLinkElementsAsync()
        {
            await InvokeJSAsync<object>(NSL + "reset", _Store.DefaultLinkElements);
        }
    }
}
