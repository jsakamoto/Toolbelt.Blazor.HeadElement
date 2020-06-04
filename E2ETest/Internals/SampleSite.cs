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

        private Process dotnetCLI;

        private readonly ManualResetEventSlim ListeningWaiter = new ManualResetEventSlim(initialState: false);

        public SampleSite(int listenPort, string projectSubFolder)
        {
            this.ListenPort = listenPort;
            this.ProjectSubFolder = projectSubFolder;
        }

        public string GetUrl() => $"http://localhost:{ListenPort}";

        public void Start()
        {
            if (dotnetCLI != null) return;

            var workDir = AppDomain.CurrentDomain.BaseDirectory;
            while (!Directory.GetDirectories(workDir, "_SampleSites").Any()) workDir = Path.GetDirectoryName(workDir);
            workDir = Path.Combine(workDir, "_SampleSites", ProjectSubFolder);

            File.AppendAllText(@"c:\work\log.txt", $"{DateTime.Now:HH:mm:ss} {workDir}\n");

            dotnetCLI = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --urls {GetUrl()}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = workDir
                },
            };
            dotnetCLI.OutputDataReceived += Process_OutputDataReceived;

            dotnetCLI.Start();
            dotnetCLI.BeginOutputReadLine();
            dotnetCLI.BeginErrorReadLine();

            var timedOut = !ListeningWaiter.Wait(millisecondsTimeout: 10000);
            if (timedOut) throw new TimeoutException("\"dotnet run\" did not respond \"Now listening on: http://\".");
            Thread.Sleep(200);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data?.Contains("Now listening on: http://") == true) ListeningWaiter.Set();
        }

        public void Stop()
        {
            if (dotnetCLI != null)
            {
                //dotnetCLI.OutputDataReceived -= Process_OutputDataReceived;
                if (!dotnetCLI.HasExited) dotnetCLI.Kill();
                dotnetCLI.Dispose();
            }
            ListeningWaiter.Dispose();
        }
    }
}
