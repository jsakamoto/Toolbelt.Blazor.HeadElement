﻿using HeadElement.E2ETest.Internals;
using Toolbelt;
using Toolbelt.Diagnostics;
using static Toolbelt.Diagnostics.XProcess;

namespace HeadElement.E2ETest;

public class SampleSite
{
    private readonly int ListenPort;

    private readonly string ProjectSubFolder;

    private readonly string TargetFramework;

    private readonly bool Published;

    private readonly bool DisableScriptInjection;

    private XProcess? dotnetCLI;

    private WorkDirectory? WorkDir;

    public SampleSite(int listenPort, string projectSubFolder, string targetFramework, bool published = false, bool disableScriptInjection = false)
    {
        this.ListenPort = listenPort;
        this.ProjectSubFolder = projectSubFolder;
        this.TargetFramework = targetFramework;
        this.Published = published;
        this.DisableScriptInjection = disableScriptInjection;
    }

    public string GetUrl() => $"http://localhost:{this.ListenPort}";

    internal string GetUrl(string subPath) => this.GetUrl() + "/" + subPath.TrimStart('/');

    public async ValueTask<SampleSite> StartAsync()
    {
        if (this.dotnetCLI != null) return this;

        var solutionDir = FileIO.FindContainerDirToAncestor("*.sln");
        var sampleSiteDir = Path.Combine(solutionDir, "_SampleSites");
        this.WorkDir = WorkDirectory.CreateCopyFrom(sampleSiteDir, arg => arg.Name is (not "obj" and not "bin"));
        var projDir = Path.Combine(this.WorkDir, this.ProjectSubFolder);

        if (this.Published)
        {
            var publishDir = Path.Combine($"{projDir}/bin/Release/{this.TargetFramework}/publish/wwwroot".Split('/'));

            await Start("dotnet", $"tool restore", projDir).ExitCodeIsAsync(0);
            await Start("dotnet", $"publish -c:Release -f:{this.TargetFramework} -p:UsingBrowserRuntimeWorkload=false -p:BlazorEnableCompression=false", projDir).ExitCodeIsAsync(0);
            this.dotnetCLI = Start("dotnet", $"serve -p:{this.ListenPort} -d:\"{publishDir}\"", projDir);
        }
        else
        {
            var args = new List<string> {
                $"run --urls {this.GetUrl()}",
                $"-f {this.TargetFramework}"
            };
            if (this.DisableScriptInjection) { args.Add("--DisableClientScriptAutoInjection true"); }
            this.dotnetCLI = Start("dotnet", string.Join(" ", args), projDir);
        }

        var success = await this.dotnetCLI.WaitForOutputAsync(output => output.Contains(this.GetUrl()), millsecondsTimeout: 15000);
        if (!success)
        {
            try { this.dotnetCLI.Dispose(); } catch { }
            var output = this.dotnetCLI.Output;
            this.dotnetCLI = null;
            throw new TimeoutException($"\"dotnet run\" did not respond \"Now listening on: {this.GetUrl()}\".\r\n" + output);
        }

        Thread.Sleep(200);
        return this;
    }

    public void Stop()
    {
        this.dotnetCLI?.Dispose();
        this.WorkDir?.Dispose();
    }
}
