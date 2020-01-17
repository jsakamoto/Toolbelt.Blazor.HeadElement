using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toolbelt.Blazor.HeadElement
{
    public interface IHeadElementHelper
    {
        ValueTask SetTitleAsync(string title);

        ValueTask<string> GetTitleAsync();

        ValueTask SetDefaultTitleAsync(string defaultTitle);

        ValueTask<string> GetDefaultTitleAsync();

        ValueTask SetMetaElementsAsync(params MetaElement[] elements);

        ValueTask<IEnumerable<MetaElement>> GetDefaultMetaElementsAsync();

        ValueTask RemoveMetaElementsAsync(params MetaElement[] elements);

        ValueTask SetLinkElementsAsync(params LinkElement[] elements);

        ValueTask<IEnumerable<LinkElement>> GetDefaultLinkElementsAsync();

        ValueTask RemoveLinkElementsAsync(params LinkElement[] elements);
    }
}
