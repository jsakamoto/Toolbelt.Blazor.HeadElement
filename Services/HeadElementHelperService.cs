using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.HeadElement
{
    public class HeadElementHelperService : HeadElementHelper, IDisposable
    {
        private readonly IJSRuntime _JS;

        private readonly NavigationManager _NavigationManager;

        private readonly IHeadElementHelperStore _Store;

        private const string NS = "Toolbelt.Head.MetaTag.";

        private bool _ScriptEnabled = false;

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

        private async ValueTask EnsureScriptEnabledAsync()
        {
            if (_ScriptEnabled) return;

            using var resStream = typeof(HeadElementHelperService).Assembly.GetManifestResourceStream("Toolbelt.Blazor.HeadElement.script.js");
            using var reader = new StreamReader(resStream);
            var scriptText = new StringBuilder();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith(" ")) line = " " + line.TrimStart(' ');
                scriptText.Append(line);
            }

            try
            {
                await _JS.InvokeVoidAsync("eval", scriptText.ToString());
                _ScriptEnabled = true;
            }
            catch { }
        }

        public override ValueTask SetTitleAsync(string title) => SetTitleCoreAsync(title, delay: true);

        public override ValueTask<string> GetTitleAsync() => new ValueTask<string>(_Store.Title);

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

        public override ValueTask SetDefaultTitleAsync(string defaultTitle)
        {
            _Store.DefaultTitle = defaultTitle;
            return new ValueTask();
        }

        public override async ValueTask<string> GetDefaultTitleAsync()
        {
            if (_Store.DefaultTitle == null)
            {
                try { _Store.DefaultTitle = await _JS.InvokeAsync<string>("eval", "(document.head.querySelector('script[type=\"text/default-title\"]')||{text:document.title}).text"); }
                catch { }
            }
            return _Store.DefaultTitle;
        }

        public override async ValueTask<IEnumerable<MetaEntry>> GetDefaultMetaElementsAsync()
        {
            if (_Store.DefaultMetaElements == null)
            {
                try
                {
                    await EnsureScriptEnabledAsync();
                    _Store.DefaultMetaElements = await _JS.InvokeAsync<MetaEntry[]>(NS + "query");
                }
                catch { }
            }
            return _Store.DefaultMetaElements;
        }

        public override async ValueTask SetMetaElementAsync(MetaEntry metaEntry)
        {
            await GetDefaultMetaElementsAsync();
            await Task.Delay(1);

            try
            {
                await EnsureScriptEnabledAsync();
                await _JS.InvokeVoidAsync(NS + "set", metaEntry);
            }
            catch { }

            _Store.MetaEntryCommands.Add(new MetaEntryCommand { Operation = MetaEntryOperations.Set, Entry = metaEntry });
            _Store.UrlLastSet = _NavigationManager.Uri;
        }

        public override async ValueTask RemoveMetaElementAsync(MetaEntry metaEntry)
        {
            await GetDefaultMetaElementsAsync();
            await Task.Delay(1);

            try
            {
                await EnsureScriptEnabledAsync();
                await _JS.InvokeVoidAsync(NS + "del", metaEntry);
            }
            catch { }

            _Store.MetaEntryCommands.Add(new MetaEntryCommand { Operation = MetaEntryOperations.Remove, Entry = metaEntry });
            _Store.UrlLastSet = _NavigationManager.Uri;
        }

        private async ValueTask ResetMetaElementsAsync()
        {
            try
            {
                await EnsureScriptEnabledAsync();
                await _JS.InvokeVoidAsync(NS + "reset", _Store.DefaultMetaElements);
            }
            catch { }
        }
    }
}
