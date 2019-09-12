using System.Threading.Tasks;

namespace Toolbelt.Blazor.HeadElement
{
    public interface IHeadElementHelper
    {
        ValueTask SetTitleAsync(string title);
        ValueTask<string> GetTitleAsync();
        ValueTask SetDefaultTitleAsync(string defaultTitle);
        ValueTask<string> GetDefaultTitleAsync();
    }
}
