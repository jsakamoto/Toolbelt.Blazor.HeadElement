using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.HeadElement
{
    public interface IHeadElementHelper
    {
        ValueTask SetTitleAsync(string title);

        ValueTask<string> GetTitleAsync();

        ValueTask SetDefaultTitleAsync(string defaultTitle);

        ValueTask<string> GetDefaultTitleAsync();

        ValueTask<IEnumerable<MetaEntry>> GetDefaultMetaElementsAsync();

        [EditorBrowsable(EditorBrowsableState.Never)]
        ValueTask SetMetaElementAsync(MetaEntry entry);

        [EditorBrowsable(EditorBrowsableState.Never)]
        ValueTask RemoveMetaElementAsync(MetaEntry entry);
    }
}
