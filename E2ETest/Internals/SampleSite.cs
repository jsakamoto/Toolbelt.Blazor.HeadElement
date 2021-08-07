using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace HeadElement.E2ETest
{
    public class SampleSite
    {
        private readonly int ListenPort;

        private readonly string ProjectSubFolder;

        private readonly string TargetFramework;

        private Process dotnetCLI;

        private readonly ManualResetEventSlim ListeningWaiter = new ManualResetEventSlim(initialState: false);

        public SampleSite(int listenPort, string projectSubFolder, string targetFramework)
        {
            this.ListenPort = listenPort;
            this.ProjectSubFolder = projectSubFolder;
            this.TargetFramework = targetFramework;
        }

        public string GetUrl() => $"http://localhost:{this.ListenPort}";

        internal string GetUrl(string subPath) => this.GetUrl() + "/" + subPath.TrimStart('/');

        public SampleSite Start()
        {
            if (this.dotnetCLI != null) return this;

            var workDir = AppDomain.CurrentDomain.BaseDirectory;
            while (!Directory.GetDirectories(workDir, "_SampleSites").Any()) workDir = Path.GetDirectoryName(workDir);
            workDir = Path.Combine(workDir, "_SampleSites", this.ProjectSubFolder);

            this.dotnetCLI = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --urls {this.GetUrl()} -f {this.TargetFramework}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = workDir
                },
            };
            this.dotnetCLI.OutputDataReceived += this.Process_OutputDataReceived;

            this.dotnetCLI.Start();
            this.dotnetCLI.BeginOutputReadLine();
            this.dotnetCLI.BeginErrorReadLine();

            var timedOut = !this.ListeningWaiter.Wait(millisecondsTimeout: 15000);
            if (timedOut) throw new TimeoutException("\"dotnet run\" did not respond \"Now listening on: http://\".");
            Thread.Sleep(200);
            return this;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data?.Contains("Now listening on: http://") == true) this.ListeningWaiter.Set();
        }

        public void Stop()
        {
            if (this.dotnetCLI != null)
            {
                //dotnetCLI.OutputDataReceived -= Process_OutputDataReceived;
                if (!this.dotnetCLI.HasExited) this.dotnetCLI.Kill();
                this.dotnetCLI.Dispose();
            }
            this.ListeningWaiter.Dispose();
        }
    }
}
