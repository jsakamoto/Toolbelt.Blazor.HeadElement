# Blazor Head Element Helper [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.Blazor.HeadElement.svg)](https://www.nuget.org/packages/Toolbelt.Blazor.HeadElement/)

## Summary

This component and services allows you to **change the title of document, "meta" elements such as OGP, on your Blazor app**.

This package supports both seiver-side Blazor and client-side Blazor WebAssembly app.

And also supports server-side pre-rendering on your server-side Blazor app.

### NOTICE! - This is "Preview" Release

This package is **"preview" release** at this time.

Some implementations of this packages depends on internals of ASP.NET Core Components.

~~Especially, server-side pre-rendering is very slow and stressfull, because **it captures all requests and buffering all responses on memory.**~~  
_19 Sep, 2019 - Fixed this issue at v.0.0.1-preview2._

## How to use

### Installation

1. Add package to your project like this.

```shell
dotnet add package Toolbelt.Blazor.HeadElement --version 0.0.2-preview2.0.2
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

3. Open `Toolbelt.Blazor.HeadElement` namespace in `_Imports.razor` file.

```
@* This is "_Imports.razor" *@
...
@using Toolbelt.Blazor.HeadElement
```

### A. Change the title of document

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


### C. Server-side pre-rendering support

If you want to get srever-side pre-rendering support, do this.

1. Add `Toolbelt.Blazor.HeadElement.ServerPrerendering` package to your project like this.

```shell
dotnet add package Toolbelt.Blazor.HeadElement.ServerPrerendering --version 0.0.1-preview2.0.2
```

2. Register "Head Element Server Prerendering" middleware at your server-side Blazor app's `Startup`, before `appUseStaticFiles()`.

```csharp
using Toolbelt.Blazor.Extensions.DependencyInjection; // <- Add this, and...

public class Startup
{
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
