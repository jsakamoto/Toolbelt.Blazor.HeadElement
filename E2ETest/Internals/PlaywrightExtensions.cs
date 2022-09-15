using Microsoft.Playwright;

namespace HeadElement.E2ETest.Internals;

public static class PlaywrightExtensions
{
    public static async ValueTask WaitForAsync(this IPage page, Func<ValueTask<bool>> predictAsync)
    {
        var canceller = new CancellationTokenSource(millisecondsDelay: 5000);
        do
        {
            if (await predictAsync()) return;
            await Task.Delay(100);
        } while (!canceller.IsCancellationRequested);
        throw new OperationCanceledException(canceller.Token);
    }

    public static async ValueTask WaitForBlazorHasBeenStarted(this IPage page)
    {
        await page.WaitForConsoleMessageAsync(new() { Predicate = message => message.Text == "Blazor has been started." });
    }

    public static async ValueTask WaitForTitleAsync(this IPage page, string expectedTitle)
    {
        await page.WaitForAsync(async () => await page.TitleAsync() == expectedTitle);
    }

    public static async ValueTask ClickHomeAsync(this IPage page)
    {
        await page.ClickAsync("a.navbar-brand");
        await page.WaitForSelectorAsync("//h1[text()='Hello, world!']");
    }

    public static async ValueTask ClickCounterAsync(this IPage page)
    {
        await page.ClickAsync("a[href=counter]");
        await page.WaitForSelectorAsync("//h1[text()='Counter']");
    }

    public static async ValueTask ClickFetchDataAsync(this IPage page)
    {
        await page.ClickAsync("a[href=fetchdata]");
        await page.WaitForSelectorAsync("//h1[text()='Weather forecast']");
    }
}
