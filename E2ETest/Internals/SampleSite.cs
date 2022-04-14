using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeadElement.E2ETest.Internals;
using Toolbelt;
using Toolbelt.Diagnostics;
using static Toolbelt.Diagnostics.XProcess;

namespace HeadElement.E2ETest
{
    public class SampleSite
    {
        private readonly int ListenPort;

        private readonly string ProjectSubFolder;

        private readonly string TargetFramework;

        private readonly bool Published;

        private XProcess dotnetCLI;

        public SampleSite(int listenPort, string projectSubFolder, string targetFramework, bool published = false)
        {
            this.ListenPort = listenPort;
            this.ProjectSubFolder = projectSubFolder;
            this.TargetFramework = targetFramework;
            this.Published = published;
        }

        public string GetUrl() => $"http://localhost:{this.ListenPort}";

        internal string GetUrl(string subPath) => this.GetUrl() + "/" + subPath.TrimStart('/');

        public async ValueTask<SampleSite> StartAsync()
        {
            if (this.dotnetCLI != null) return this;

            var solutionDir = FileIO.FindContainerDirToAncestor("*.sln");
            var workDir = Path.Combine(solutionDir, "_SampleSites", this.ProjectSubFolder);

            if (this.Published)
            {
                var publishDir = Path.Combine($"{workDir}/bin/Release/{this.TargetFramework}/publish/wwwroot".Split('/'));

                await Start("dotnet", $"tool restore", workDir).ExitCodeIsAsync(0);
                await Start("dotnet", $"publish -c:Release -f:{this.TargetFramework} -p:UsingBrowserRuntimeWorkload=false -p:BlazorEnableCompression=false", workDir).ExitCodeIsAsync(0);
                this.dotnetCLI = Start("dotnet", $"serve -p:{this.ListenPort} -d:\"{publishDir}\"", workDir);
            }
            else
            {
                this.dotnetCLI = Start("dotnet", $"run --urls {this.GetUrl()} -f {this.TargetFramework}", workDir);
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
        }
    }
}
