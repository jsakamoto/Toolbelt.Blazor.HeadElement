namespace Toolbelt.Blazor.HeadElement.Internals
{
    public class HeadElementHelperStore : IHeadElementHelperStore
    {
        public string DefaultTitle { get; set; }

        public string Title { get; set; }

        public string UrlLastTitleSet { get; set; }
    }
}
