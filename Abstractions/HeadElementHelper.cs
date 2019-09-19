using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toolbelt.Blazor.HeadElement
{
    public abstract class HeadElementHelper
    {
        public abstract ValueTask SetTitleAsync(string title);

        public abstract ValueTask<string> GetTitleAsync();

        public abstract ValueTask SetDefaultTitleAsync(string defaultTitle);

        public abstract ValueTask<string> GetDefaultTitleAsync();

        public abstract ValueTask<IEnumerable<MetaEntry>> GetDefaultMetaElementsAsync();

        public abstract ValueTask SetMetaElementAsync(MetaEntry entry);

        public abstract ValueTask RemoveMetaElementAsync(MetaEntry entry);

        public async ValueTask SetMetaByNameAsync(string name, string content)
        {
            await this.SetMetaElementAsync(new MetaEntry
            {
                KeyType = MetaEntryKeyType.Name,
                Key = name,
                Content = content
            });
        }

        public async ValueTask SetMetaByPropertyAsync(string property, string content)
        {
            await this.SetMetaElementAsync(new MetaEntry
            {
                KeyType = MetaEntryKeyType.Property,
                Key = property,
                Content = content
            });
        }

        public async ValueTask RemoveMetaByNameAsync(string name)
        {
            await this.RemoveMetaElementAsync(new MetaEntry
            {
                KeyType = MetaEntryKeyType.Name,
                Key = name,
                Content = null
            });
        }

        public async ValueTask RemoveMetaByPropertyAsync(string property)
        {
            await this.RemoveMetaElementAsync(new MetaEntry
            {
                KeyType = MetaEntryKeyType.Property,
                Key = property,
                Content = null
            });
        }
    }
}
