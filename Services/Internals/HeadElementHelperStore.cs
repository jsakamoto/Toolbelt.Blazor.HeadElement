using System.Collections.Generic;

namespace Toolbelt.Blazor.HeadElement.Internals
{
    public class HeadElementHelperStore : IHeadElementHelperStore
    {
        public string DefaultTitle { get; set; }

        public string Title { get; set; }

        public string UrlLastSet { get; set; }

        public IEnumerable<MetaEntry> DefaultMetaElements { get; set; }

        public IList<MetaEntryCommand> MetaEntryCommands { get; } = new List<MetaEntryCommand>();
    }
}
