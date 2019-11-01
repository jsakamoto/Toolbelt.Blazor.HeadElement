using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.HeadElement
{
    public class HeadElementHelperService : IHeadElementHelper, IDisposable
    {
        private readonly IJSRuntime _JS;

        private readonly NavigationManager _NavigationManager;

        private readonly IHeadElementHelperStore _Store;

        private const string NS = "Toolbelt.Head.MetaTag.";

        private const string NST = "Toolbelt.Head.Title.";

        private bool _ScriptEnabled = false;

        private SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public HeadElementHelperService(IJSRuntime js, NavigationManager navigationManager, IHeadElementHelperStore internalStore)
        {
            this._JS = js;
            this._NavigationManager = navigationManager;
            this._Store = internalStore;
            this._NavigationManager.LocationChanged += _NavigationManager_LocationChanged;
        }

        public void Dispose()
        {
            _NavigationManager.LocationChanged -= _NavigationManager_LocationChanged;
        }

        private async ValueTask<bool> EnsureScriptEnabledAsync()
        {
            if (_ScriptEnabled) return _ScriptEnabled;

            await SemaphoreSlim.WaitAsync();
            try
            {
                if (_ScriptEnabled) return _ScriptEnabled;
                var script = $"new Promise(r=>((d,t,s)=>(h=>h.querySelector(t+`[src=\"${{s}}\"]`)?r():(e=>(e.src=s,e.onload=r,h.appendChild(e)))(d.createElement(t)))(d.head))(document,'script','_content/Toolbelt.Blazor.HeadElement.Services/script.min.js'))";
                await _JS.InvokeVoidAsync("eval", script);
                _ScriptEnabled = true;
            }
            catch { }
            finally { SemaphoreSlim.Release(); }
            return _ScriptEnabled;
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

            if (await EnsureScriptEnabledAsync())
            {
                await _JS.InvokeVoidAsync(NST + "set", title);
            }
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
                if (await EnsureScriptEnabledAsync())
                {
                    _Store.DefaultTitle = await _JS.InvokeAsync<string>(NST + "query");
                }
            }
            return _Store.DefaultTitle;
        }

        public async ValueTask<IEnumerable<MetaElement>> GetDefaultMetaElementsAsync()
        {
            if (_Store.DefaultMetaElements == null)
            {
                if (await EnsureScriptEnabledAsync())
                {
                    _Store.DefaultMetaElements = await _JS.InvokeAsync<MetaElement[]>(NS + "query");
                }
            }
            return _Store.DefaultMetaElements;
        }

        public async ValueTask SetMetaElementsAsync(params MetaElement[] elements)
        {
            await GetDefaultMetaElementsAsync();
            await Task.Delay(1);

            if (await EnsureScriptEnabledAsync())
            {
                await _JS.InvokeVoidAsync(NS + "set", new object[] { elements });
            }

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

            if (await EnsureScriptEnabledAsync())
            {
                await _JS.InvokeVoidAsync(NS + "del", new object[] { elements });
            }

            var commands = elements.Select(elem => new MetaElementCommand { Operation = MetaElementOperations.Remove, Element = elem });
            foreach (var command in commands)
            {
                _Store.MetaElementCommands.Add(command);
            }
            _Store.UrlLastSet = _NavigationManager.Uri;
        }

        private async ValueTask ResetMetaElementsAsync()
        {
            if (await EnsureScriptEnabledAsync())
            {
                await _JS.InvokeVoidAsync(NS + "reset", _Store.DefaultMetaElements);
            }
        }
    }
}
