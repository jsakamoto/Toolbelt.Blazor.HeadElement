namespace Toolbelt.Blazor.HeadElement
{
    public class HeadElementHelperServiceOptions
    {
#if ENABLE_JSMODULE
        [System.Obsolete("The \"DisableClientScriptAutoInjection\" option is no longer effective in .net 5.0.")]
#endif
        public bool DisableClientScriptAutoInjection { get; set; }
    }
}
