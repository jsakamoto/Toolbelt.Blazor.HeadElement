using System.Threading.Tasks;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.HeadElement
{
    public static class HeadElementHelperExtensions
    {
        public static async ValueTask SetMetaByNameAsync(this IHeadElementHelper helper, string name, string content)
        {
            await helper.SetMetaElementAsync(new MetaEntry
            {
                KeyType = MetaEntryKeyType.Name,
                Key = name,
                Content = content
            });
        }

        public static async ValueTask SetMetaByPropertyAsync(this IHeadElementHelper helper, string property, string content)
        {
            await helper.SetMetaElementAsync(new MetaEntry
            {
                KeyType = MetaEntryKeyType.Property,
                Key = property,
                Content = content
            });
        }

        public static async ValueTask RemoveMetaByNameAsync(this IHeadElementHelper helper, string name)
        {
            await helper.RemoveMetaElementAsync(new MetaEntry
            {
                KeyType = MetaEntryKeyType.Name,
                Key = name,
                Content = null
            });
        }

        public static async ValueTask RemoveMetaByPropertyAsync(this IHeadElementHelper helper, string property)
        {
            await helper.RemoveMetaElementAsync(new MetaEntry
            {
                KeyType = MetaEntryKeyType.Property,
                Key = property,
                Content = null
            });
        }
    }
}
