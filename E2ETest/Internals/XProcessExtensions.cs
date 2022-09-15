using NUnit.Framework;
using Toolbelt.Diagnostics;

namespace HeadElement.E2ETest.Internals;

internal static class XProcessExtensions
{
    public static async ValueTask ExitCodeIsAsync(this XProcess process, int expectedExitCode)
    {
        await process.WaitForExitAsync();
        process.ExitCode.Is(code => code == 0, process.Output);
    }
}
