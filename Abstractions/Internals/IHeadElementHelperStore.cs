namespace Toolbelt.Blazor.HeadElement.Internals
{
    public interface IHeadElementHelperStore
    {
        string DefaultTitle { get; set; }

        string Title { get; set; }

        string UrlLastTitleSet { get; set; }
    }
}
