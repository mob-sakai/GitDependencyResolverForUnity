using System.IO;
using System.Text;
using UnityEngine;

namespace Coffee.GitDependencyResolver
{
    internal static class GitUtils
    {
        private static readonly StringBuilder s_sbError = new StringBuilder();
        private static readonly StringBuilder s_sbOutput = new StringBuilder();

        private static bool IsGitRunning { get; set; }

        private delegate void GitCommandCallback(bool success, string output);

        public static bool ClonePackage(PackageMeta package, string clonePath)
        {
            Directory.CreateDirectory(clonePath);
            var args = string.Format("clone --depth=1 --branch {0} --single-branch {1} {2}", package.rev, package.url, clonePath);
            return ExecuteGitCommand(args);
        }

        static bool ExecuteGitCommand(string args, GitCommandCallback callback = null, bool waitForExit = true)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                Arguments = args,
                CreateNoWindow = true,
                FileName = "git",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            var launchProcess = System.Diagnostics.Process.Start(startInfo);
            if (launchProcess == null || launchProcess.HasExited || launchProcess.Id == 0)
            {
                Debug.LogError("No 'git' executable was found. Please install Git on your system and restart Unity");
                if (callback != null)
                    callback(false, "");
            }
            else
            {
                //Add process callback.
                IsGitRunning = true;
                s_sbError.Length = 0;
                s_sbOutput.Length = 0;
                launchProcess.OutputDataReceived += (sender, e) => s_sbOutput.AppendLine(e.Data ?? "");
                launchProcess.ErrorDataReceived += (sender, e) => s_sbError.AppendLine(e.Data ?? "");
                launchProcess.Exited += (sender, e) =>
                {
                    IsGitRunning = false;
                    var success = 0 == launchProcess.ExitCode;
                    if (!success)
                    {
                        Debug.LogErrorFormat("Error: git {0}\n\n{1}", args, s_sbError);
                    }

                    if (callback != null)
                        callback(success, s_sbOutput.ToString());
                };

                launchProcess.BeginOutputReadLine();
                launchProcess.BeginErrorReadLine();
                launchProcess.EnableRaisingEvents = true;

                if (waitForExit)
                {
                    launchProcess.WaitForExit();
                }
            }

            return launchProcess.HasExited
                ? launchProcess.ExitCode == 0
                : true;
        }
    }
}
