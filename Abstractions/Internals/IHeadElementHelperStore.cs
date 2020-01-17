using System.Collections.Generic;

namespace Toolbelt.Blazor.HeadElement.Internals
{
    public interface IHeadElementHelperStore
    {
        string UrlLastSet { get; set; }

        string DefaultTitle { get; set; }

        string Title { get; set; }

        IEnumerable<MetaElement> DefaultMetaElements { get; set; }

        IList<MetaElementCommand> MetaElementCommands { get; }

        IEnumerable<LinkElement> DefaultLinkElements { get; set; }

        IList<LinkElementCommand> LinkElementCommands { get; }
    }
}
