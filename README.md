# Blazor Head Element Helper [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.Blazor.HeadElement.svg)](https://www.nuget.org/packages/Toolbelt.Blazor.HeadElement/)

## Summary

This components and services allows you to **change the title of document, "meta" elements such as OGP, and "link" elements such as canonical url, favicon etc. on your Blazor app**.

This package supports both seiver-side Blazor and client-side Blazor WebAssembly app.

And also supports server-side pre-rendering on your server-side Blazor app.

See also following the live demonstration sites.

- [Blazor WebAssembly (Client-side Blazor) edition](https://demo-blazor-headelement-wasm.azurewebsites.net/)
- [Blazor Server (Server-side Blazor) edition](https://demo-blazor-headelement.azurewebsites.net/)

### Notice

Blazor since .NET 5.0 Preview 8 also started to provide `Title`, `Link`, and `Meta` components by `Microsoft.AspNetCore.Components.Web.Extensions` NuGet package.

However, I'll continue to provide this package and keep to maintain, because this library still has some advantages.

To know what differences are there between this library and Microsoft.AspNetCore.Components.Web.Extensions 5.0 preview 8, please see the following table.

Feature                               | This library            | .NET5 Preview 8  
--------------------------------------|-------------------------|--------------------------------------------  
Server Pre-Rendering                  | Supported.              | Not Supported, yet.  
Respecting pre-rendered title         | Yes.                    | No. It is never recovered if any components override it.  
Overriding pre-rendered meta or link  | Can handle it properly. | Just append it. it may cause duplication.  
Canceling "meta http-equiv=refresh"   | Works well.             | There is no support.  
Using it as a service, not components | Supported.              | Not Supported.  
To Including helper JavaScript        | Automatic.              | Required manually.


## How to use

### Installation

1. Add package to your project like this.

```shell
dotnet add package Toolbelt.Blazor.HeadElement
```

2. Register "Head Element Helper" service at your Blazor app's `Startup`.

```csharp
using Toolbelt.Blazor.Extensions.DependencyInjection; // <- Add this, and...

public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddHeadElementHelper(); // <- Add this.
    ...
```

If the project is Blazor WebAssembly App v.3.2+, you should do it in `Program` class, instead.

```csharp
using Toolbelt.Blazor.Extensions.DependencyInjection; // <- Add this, and...
...
public class Program
{
  public static async Task Main(string[] args)
  {
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    ...
    builder.Services.AddHeadElementHelper(); // <- Add this.
    ...
```


3. Open `Toolbelt.Blazor.HeadElement` namespace in `_Imports.razor` file.

```
@* This is "_Imports.razor" *@
...
@using Toolbelt.Blazor.HeadElement
```

### A. Change the title of the document

3. Markup `<Title>` component in your .razor file.

```html
@* This is "Pages/Counter.razor" *@
@page "/counter"

<Title>Counter(@currentCount) - Server Side App</Title>
```

The title of document will be changed.

![fig1](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.HeadElement/master/.assets/fig1.png)

### B. Change "meta" elements

You can also add or override "meta" elements at runtime dynamically using `<Meta>` component like this.

```html
@* This is "Pages/Counter.razor" *@
@page "/counter"

<Meta Property="ogp:title" Content="Counter" />
```

_**Note:**_ You can also use native attribute names (lower and kebab case. ex: "http-equiv") instead of Razor component parameter names (pascal case).

### C. Change "link" elements

You can add or override "link" elements at runtime dynamically using `<Link>` component like this.

```html
@* This is "Pages/Counter.razor" *@
@page "/counter"

<Link Rel="icon" Href="@($"/favicons/{GetFaviconName()}")" />
```

![fig3](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.HeadElement/master/.assets/fig3.gif)

_**Note:**_ You can also use native attribute names (lower and kebab case) instead of Razor component parameter names (pascal case).

### D. IHeadElementHelper

You can do these tasks by using `IHeadElementHelper` service instead of using `<Title>`, `<Meta>`, and `<Link>` components.

You can get the `IHeadElementHelper` service instnace by "Dependency Injection" mechanism.

```csharp
@inject IHeadElementHelper HeadElementHelper
@using static Toolbelt.Blazor.HeadElement.MetaElement
...
@code {
  protected override async Task OnInitializedAsync()
  {
    await HeadElementHelper.SetTitleAsync("Wow!");
    await HeadElementHelper.SetMetaElementsAsync(
      ByName("description", "Foo bar..."),
      ByProp("og:title", "WoW!")
    );
    await HeadElementHelper.SetLinkElementsAsync(
      new LinkElement("canonical", "https://foo.com/bar")
    );
    ...
```

## Server-side pre-rendering support

If you want to get srever-side pre-rendering support, do this.

1. Add `Toolbelt.Blazor.HeadElement.ServerPrerendering` package to your project like this.

```shell
dotnet add package Toolbelt.Blazor.HeadElement.ServerPrerendering
```

2. Register "Head Element Server Prerendering" middleware at your server-side Blazor app's `Startup`, before `app.UseStaticFiles()`.

```csharp
using Toolbelt.Blazor.Extensions.DependencyInjection; // <- Add this, and...
...
public class Startup
{
  ...
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddHeadElementHelper(); // <!- Don't miss this line, and...
    ...

  public void Configure(IApplicationBuilder app)
  {
    app.UseHeadElementServerPrerendering(); // <- Add this.
    ...
    app.UseStaticFiles()
    ...
```

![fig2](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.HeadElement/master/.assets/fig2.png)

## License

[Mozilla Public License Version 2.0](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.HeadElement/master/LICENSE)
