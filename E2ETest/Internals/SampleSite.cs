using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Toolbelt;
using Toolbelt.Diagnostics;

namespace HeadElement.E2ETest
{
    public class SampleSite
    {
        private readonly int ListenPort;

        private readonly string ProjectSubFolder;

        private readonly string TargetFramework;

        private XProcess dotnetCLI;

        private readonly ManualResetEventSlim ListeningWaiter = new ManualResetEventSlim(initialState: false);

        public SampleSite(int listenPort, string projectSubFolder, string targetFramework)
        {
            this.ListenPort = listenPort;
            this.ProjectSubFolder = projectSubFolder;
            this.TargetFramework = targetFramework;
        }

        public string GetUrl() => $"http://localhost:{this.ListenPort}";

        internal string GetUrl(string subPath) => this.GetUrl() + "/" + subPath.TrimStart('/');

        public async ValueTask<SampleSite> StartAsync()
        {
            if (this.dotnetCLI != null) return this;

            var solutionDir = FileIO.FindContainerDirToAncestor("*.sln");
            var workDir = Path.Combine(solutionDir, "_SampleSites", this.ProjectSubFolder);

            this.dotnetCLI = XProcess.Start("dotnet", $"run --urls {this.GetUrl()} -f {this.TargetFramework}", workDir);
            var success = await this.dotnetCLI.WaitForOutputAsync(output => output.Contains("Now listening on: http://"), millsecondsTimeout: 15000);
            if (!success) throw new TimeoutException("\"dotnet run\" did not respond \"Now listening on: http://\".\r\n" + this.dotnetCLI.Output);

            Thread.Sleep(200);
            return this;
        }

        public void Stop()
        {
            this.dotnetCLI?.Dispose();
            this.ListeningWaiter.Dispose();
        }
    }
}
