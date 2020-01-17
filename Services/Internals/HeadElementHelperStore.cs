using System.Collections.Generic;

namespace Toolbelt.Blazor.HeadElement.Internals
{
    public class HeadElementHelperStore : IHeadElementHelperStore
    {
        public string DefaultTitle { get; set; }

        public string Title { get; set; }

        public string UrlLastSet { get; set; }

        public IEnumerable<MetaElement> DefaultMetaElements { get; set; }

        public IList<MetaElementCommand> MetaElementCommands { get; } = new List<MetaElementCommand>();

        public IEnumerable<LinkElement> DefaultLinkElements { get; set; }

        public IList<LinkElementCommand> LinkElementCommands { get; } = new List<LinkElementCommand>();
    }
}
