using System.Collections.Generic;

namespace Toolbelt.Blazor.HeadElement.Internals
{
    public interface IHeadElementHelperStore
    {
        string UrlLastSet { get; set; }

        string DefaultTitle { get; set; }

        string Title { get; set; }

        IEnumerable<MetaEntry> DefaultMetaElements { get; set; }

        IList<MetaEntryCommand> MetaEntryCommands { get; }
    }
}
