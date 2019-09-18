using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.HeadElement
{
    public class HeadElementHelper : IHeadElementHelper, IDisposable
    {
        private readonly IJSRuntime _JS;

        private readonly NavigationManager _NavigationManager;

        private readonly IHeadElementHelperStore _Store;

        private const string NS = "Toolbelt.Head.MetaTag.";

        public HeadElementHelper(IJSRuntime js, NavigationManager navigationManager, IHeadElementHelperStore internalStore)
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

        public ValueTask SetTitleAsync(string title) => SetTitleCoreAsync(title, delay: true);

        public ValueTask<string> GetTitleAsync() => new ValueTask<string>(_Store.Title);

        private void _NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            _Store.MetaEntryCommands.Clear();

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
            var encodedTitle = title.Replace("'", "\\'"); // TODO:
            try { await _JS.InvokeVoidAsync("eval", $"document.title='{encodedTitle}'"); } catch (Exception) { }
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
                try { _Store.DefaultTitle = await _JS.InvokeAsync<string>("eval", "(document.head.querySelector('script[type=\"text/default-title\"]')||{text:document.title}).text"); }
                catch { }
            }
            return _Store.DefaultTitle;
        }

        public async ValueTask<IEnumerable<MetaEntry>> GetDefaultMetaElementsAsync()
        {
            if (_Store.DefaultMetaElements == null)
            {
                try { _Store.DefaultMetaElements = await _JS.InvokeAsync<MetaEntry[]>(NS + "query"); } catch { }
            }
            return _Store.DefaultMetaElements;
        }

        public async ValueTask SetMetaElementAsync(MetaEntry metaEntry)
        {
            await GetDefaultMetaElementsAsync();
            await Task.Delay(1);

            try { await _JS.InvokeVoidAsync(NS + "set", metaEntry); } catch { }

            _Store.MetaEntryCommands.Add(new MetaEntryCommand { Operation = MetaEntryOperations.Set, Entry = metaEntry });
            _Store.UrlLastSet = _NavigationManager.Uri;
        }

        public async ValueTask RemoveMetaElementAsync(MetaEntry metaEntry)
        {
            await GetDefaultMetaElementsAsync();
            await Task.Delay(1);

            try { await _JS.InvokeVoidAsync(NS + "del", metaEntry); } catch { }

            _Store.MetaEntryCommands.Add(new MetaEntryCommand { Operation = MetaEntryOperations.Remove, Entry = metaEntry });
            _Store.UrlLastSet = _NavigationManager.Uri;
        }

        private async ValueTask ResetMetaElementsAsync()
        {
            try { await _JS.InvokeVoidAsync(NS + "reset", _Store.DefaultMetaElements); } catch { }
        }
    }
}
