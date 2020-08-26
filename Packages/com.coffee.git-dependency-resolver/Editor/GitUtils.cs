using System.IO;
using System.Text;
using UnityEngine;

namespace Coffee.GitDependencyResolver
{
    //TODO: It's better to implement it in javascript.
    internal static class GitUtils
    {
        private static readonly StringBuilder s_sbError = new StringBuilder();
        private static readonly StringBuilder s_sbOutput = new StringBuilder();

        private static bool IsGitRunning { get; set; }

        private delegate void GitCommandCallback(bool success, string output);

        public static bool ClonePackage(PackageMeta package, string clonePath)
        {
            Directory.CreateDirectory(clonePath);

            ExecuteGitCommand("init", dir: clonePath);
            ExecuteGitCommand("remote add origin " + package.repository, dir: clonePath);
            if (!string.IsNullOrEmpty(package.path))
            {
                ExecuteGitCommand("config core.sparsecheckout true", dir: clonePath);
                File.WriteAllText(Path.Combine(clonePath, ".git/info/sparse-checkout"), package.path);
            }

            var revision = !string.IsNullOrEmpty(package.hash)
                ? package.hash
                : !string.IsNullOrEmpty(package.revision)
                    ? package.revision
                    : "HEAD";

            return
                ExecuteGitCommand("fetch --depth 1 origin " + revision, dir: clonePath)
                && ExecuteGitCommand("reset --hard FETCH_HEAD", dir: clonePath)
                && ExecuteGitCommand("rev-parse HEAD", (success, output) =>
                {
                    if (!success) return;
                    package.hash = output.Trim();
                }, dir: clonePath);
        }

        static bool ExecuteGitCommand(string args, GitCommandCallback callback = null, bool waitForExit = true, string dir = ".")
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                Arguments = args,
                CreateNoWindow = true,
                FileName = "git",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = dir,
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
