using System;
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

        private readonly IHeadElementHelperStore _InternalStore;

        public HeadElementHelper(IJSRuntime js, NavigationManager navigationManager, IHeadElementHelperStore internalStore)
        {
            this._JS = js;
            this._NavigationManager = navigationManager;
            this._InternalStore = internalStore;
            this._NavigationManager.LocationChanged += _NavigationManager_LocationChanged;
        }

        public void Dispose()
        {
            _NavigationManager.LocationChanged -= _NavigationManager_LocationChanged;
        }

        public ValueTask SetTitleAsync(string title) => SetTitleCoreAsync(title, delay: true);

        public ValueTask<string> GetTitleAsync() => new ValueTask<string>(_InternalStore.Title);

        private void _NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (_InternalStore.DefaultTitle != null && _InternalStore.UrlLastTitleSet != e.Location)
            {
                var _ = SetTitleCoreAsync(_InternalStore.DefaultTitle, delay: false);
            }
        }

        private async ValueTask SetTitleCoreAsync(string title, bool delay)
        {
            await GetDefaultTitleAsync();

            if (delay)
            {
                await Task.Delay(1);
            }

            _InternalStore.Title = title;
            _InternalStore.UrlLastTitleSet = _NavigationManager.Uri;
            var encodedTitle = title.Replace("'", "\\'"); // TODO:
            try { await _JS.InvokeVoidAsync("eval", $"document.title='{encodedTitle}'"); } catch (Exception) { }
        }

        public ValueTask SetDefaultTitleAsync(string defaultTitle)
        {
            _InternalStore.DefaultTitle = defaultTitle;
            return new ValueTask();
        }

        public async ValueTask<string> GetDefaultTitleAsync()
        {
            if (_InternalStore.DefaultTitle == null)
            {
                try
                {
                    _InternalStore.DefaultTitle = await _JS.InvokeAsync<string>("eval", "(document.head.querySelector('script[type=\"text/default-title\"]')||{text:document.title}).text");
                }
                catch (Exception) { }
            }
            return _InternalStore.DefaultTitle;
        }
    }
}
