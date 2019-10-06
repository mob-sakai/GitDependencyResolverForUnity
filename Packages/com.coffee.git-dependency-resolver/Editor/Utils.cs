using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Coffee.PackageManager.DependencyResolver
{
	internal static class GitUtils
	{
		static readonly StringBuilder s_sbError = new StringBuilder ();
		static readonly StringBuilder s_sbOutput = new StringBuilder ();

		public static bool IsGitRunning { get; private set; }

		public delegate void GitCommandCallback (bool success, string output);

		public static bool ClonePackage (PackageMeta pi)
		{
			var outpath = "Packages/." + pi.name;
			if (Directory.Exists (outpath))
			{
				FileUtil.DeleteFileOrDirectory (outpath);
			}

			string args = string.Format ("clone --depth=1 --branch {0} --single-branch {1} {2}", pi.branch, pi.path, outpath);
			ExecuteGitCommand (args,
			(success, _) =>
			{
				if (success)
				{
					FileUtil.DeleteFileOrDirectory (outpath + "/.git");
				}
			},
			true);

			return Directory.Exists (outpath);
		}

		static WaitWhile ExecuteGitCommand (string args, GitCommandCallback callback, bool waitForExit = false)
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

			var launchProcess = System.Diagnostics.Process.Start (startInfo);
			if (launchProcess == null || launchProcess.HasExited == true || launchProcess.Id == 0)
			{
				Debug.LogError ("No 'git' executable was found. Please install Git on your system and restart Unity");
				callback (false, "");
			}
			else
			{
				//Add process callback.
				IsGitRunning = true;
				s_sbError.Length = 0;
				s_sbOutput.Length = 0;
				launchProcess.OutputDataReceived += (sender, e) => s_sbOutput.AppendLine (e.Data ?? "");
				launchProcess.ErrorDataReceived += (sender, e) => s_sbError.AppendLine (e.Data ?? "");
				launchProcess.Exited += (sender, e) =>
				{
					IsGitRunning = false;
					bool success = 0 == launchProcess.ExitCode;
					if (!success)
					{
						Debug.LogErrorFormat ("Error: git {0}\n\n{1}", args, s_sbError);
					}
					callback (success, s_sbOutput.ToString ());
				};

				launchProcess.BeginOutputReadLine ();
				launchProcess.BeginErrorReadLine ();
				launchProcess.EnableRaisingEvents = true;

				if (waitForExit)
				{
					launchProcess.WaitForExit ();
				}
			}

			return new WaitWhile (() => IsGitRunning);
		}
	}
}
