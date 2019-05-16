using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Coffee.PackageManager
{
	[InitializeOnLoad]
	public static class GitDependencyResolver
	{
		static GitDependencyResolver ()
		{
			EditorApplication.projectChanged += StartResolve;
			StartResolve ();
		}

		/// <summary>
		/// Uninstall unused packages (for auto-installed packages)
		/// </summary>
		static void UninstallUnusedPackages ()
		{
			bool check = true;
			while (check)
			{
				check = false;

				// Collect all dependencies.
				var allDependencies = Directory.GetDirectories ("./Library/PackageCache")
					.Concat (Directory.GetDirectories ("./Packages"))
					.Select (PackageMeta.FromPackageDir)            // Convert to PackageMeta
					.Where (x => x != null)                         // Skip null
					.SelectMany (x => x.dependencies)               // Get all dependencies
					.ToArray ();

				// Collect unused pakages.
				var unusedPackages = Directory.GetDirectories ("./Packages")
					.Where (x => Path.GetFileName (x).StartsWith ("."))         // Directory name starts with '.'. This is 'auto-installed package'
					.Select (PackageMeta.FromPackageDir)                        // Convert to PackageMeta
					.Where (x => x != null)                                     // Skip null
					.Where (x => allDependencies.All (y => y.name != x.name))   // No depended from other packages
					.ToArray ();

				// Uninstall unused packages and re-check.
				foreach (var p in unusedPackages)
				{
					check = true;
					Debug.LogFormat ("[Resolver] Uninstall unused package: {0} from {1}", p.name, p.path);
					FileUtil.DeleteFileOrDirectory (p.path);
				}
			}
		}


		static void StartResolve ()
		{
			// Uninstall unused packages (for auto-installed packages)
			UninstallUnusedPackages ();

			// Collect all installed pakages.
			var installedPackages = Directory.GetDirectories ("./Library/PackageCache")
				.Concat (Directory.GetDirectories ("./Packages"))
				.Select (PackageMeta.FromPackageDir)    // Convert to PackageMeta
				.Where (x => x != null)                 // Skip null
				.ToArray ();

			// Collect all dependencies.
			var dependencies = installedPackages
				.SelectMany (x => x.dependencies)				// Get all dependencies
				.Where (x => !string.IsNullOrEmpty (x.path));	// path (url) is available

			List<PackageMeta> requestedPackages = new List<PackageMeta> ();

			// Check all dependencies.
			foreach (var dependency in dependencies)
			{
				// Is the depended package installed already?
				bool isInstalled = installedPackages
						.Concat (requestedPackages)
						.Any (x => dependency.name == x.name && dependency.version <= x.version);

				// Install the depended package later.
				if (!isInstalled)
				{
					Debug.LogFormat ("[Resolver] A dependency package is requested: {0}", dependency.name);
					requestedPackages.RemoveAll (x => dependency.name == x.name);
					requestedPackages.Add (dependency);
				}
			}

			// No packages is requested to install.
			if (requestedPackages.Count == 0)
				return;

			// Install all requested packages.
			for (int i = 0; i < requestedPackages.Count; i++)
			{
				PackageMeta meta = requestedPackages [i];
				EditorUtility.DisplayProgressBar ("Add Package", "Cloning: " + meta.name, i / (float)requestedPackages.Count);
				Debug.LogFormat ("[Resolver] A package is cloning: {0}", meta.name);
				bool success = GitUtils.ClonePackage (meta);
				if (!success)
				{
					Debug.LogFormat ("[Resolver] Failed to clone: {0}", meta.name);
					break;
				}
			}

			// Recompile the packages
			EditorUtility.ClearProgressBar ();
			EditorApplication.delayCall += AssetDatabase.Refresh;
		}
	}
}
