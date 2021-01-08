using System.IO;
using System.Text;
using UnityEngine;

namespace Coffee.GitDependencyResolver
{
    //TODO: It's better to implement it in javascript.
    /// <summary>
    /// Utility for git operations.
    /// </summary>
    internal static class GitUtils
    {
        private static readonly StringBuilder k_SbError = new StringBuilder();
        private static readonly StringBuilder k_SbOutput = new StringBuilder();

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

        private static bool ExecuteGitCommand(string args, GitCommandCallback callback = null, bool waitForExit = true, string dir = ".")
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

            var process = System.Diagnostics.Process.Start(startInfo);
            if (process == null || process.HasExited || process.Id == 0)
            {
                Debug.LogError("No 'git' executable was found. Please install Git on your system and restart Unity");
                if (callback != null)
                    callback(false, "");
                return false;
            }
            else
            {
                //Add process callback.
                k_SbError.Length = 0;
                k_SbOutput.Length = 0;
                process.OutputDataReceived += (sender, e) => k_SbOutput.AppendLine(e.Data ?? "");
                process.ErrorDataReceived += (sender, e) => k_SbError.AppendLine(e.Data ?? "");
                process.Exited += (sender, e) =>
                {
                    var success = 0 == process.ExitCode;
                    if (!success)
                    {
                        Debug.LogErrorFormat("Error: git {0}\n\n{1}", args, k_SbError);
                    }

                    if (callback != null)
                        callback(success, k_SbOutput.ToString());
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.EnableRaisingEvents = true;

                if (waitForExit)
                {
                    process.WaitForExit();
                }
            }

            return !process.HasExited || process.ExitCode == 0;
        }
    }
}
