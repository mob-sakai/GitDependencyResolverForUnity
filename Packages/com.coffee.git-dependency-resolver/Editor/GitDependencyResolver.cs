using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("GitDependencyResolver.EditorTests")]

namespace Coffee.GitDependencyResolver
{
    [InitializeOnLoad]
    internal static class Resolver
    {
        const StringComparison Ordinal = StringComparison.Ordinal;
        const string k_LogHeader = "<b><color=#2E8B57>[GitResolver]</color></b> ";

        [System.Diagnostics.Conditional("GDR_LOG")]
        static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(k_LogHeader + format, args);
        }

        static void Error(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(k_LogHeader + format, args);
        }

        static Resolver()
        {
            Log("Init");

            EditorApplication.projectChanged += StartResolve;
            StartResolve();
        }

        private static PackageMeta[] GetInstalledPackages()
        {
            return Directory.GetDirectories("./Library/PackageCache")
                .Concat(Directory.GetDirectories("./Packages"))
                .Select(PackageMeta.FromPackageDir) // Convert to PackageMeta
                .Concat(new[] {PackageMeta.FromPackageJson("./Packages/manifest.json")})
                .Where(x => x != null) // Skip null
                .ToArray();
        }

        /// <summary>
        /// Uninstall unused packages (for auto-installed packages)
        /// </summary>
        private static void UninstallUnusedPackages()
        {
            Log("Find for unused automatically installed packages");
            var needToCheck = true;
            while (needToCheck)
            {
                needToCheck = false;

                // Collect all dependencies.
                var allDependencies = GetInstalledPackages()
                    .SelectMany(x => x.GetAllDependencies()) // Get all dependencies
                    .ToArray();

                PackageMeta[] autoInstalledPackages = Directory.GetDirectories("./Packages")
                    .Where(x => Path.GetFileName(x).StartsWith(".", Ordinal)) // Directory name starts with '.'. This is 'auto-installed package'
                    .Select(PackageMeta.FromPackageDir) // Convert to PackageMeta
                    .Where(x => x != null) // Skip null
                    .ToArray();

                var used = autoInstalledPackages
                    .Where(x => allDependencies.Any(y => y.name == x.name)) // Depended from other packages
                    .GroupBy(x => x.name) // Grouped by package name
                    .Select(x => x.OrderByDescending(y => y.version).First()) // Latest package
                    .ToArray();

                // Collect unused pakages.
                var unused = autoInstalledPackages
                    .Except(used) // Exclude used packages
                    .ToArray();

                var sb = new StringBuilder();
                sb.AppendLine("############## UninstallUnusedPackages ##############");
                sb.AppendLine("\n[ allDependencies ] ");
                allDependencies.ToList().ForEach(p => sb.AppendLine(p.ToString()));

                sb.AppendLine("\n[ autoInstalledPackages ] ");
                autoInstalledPackages.ToList().ForEach(p => sb.AppendLine(p.ToString()));

                sb.AppendLine("\n[ unusedPackages ] ");
                unused.ToList().ForEach(p => sb.AppendLine(p.ToString()));
                Log(sb.ToString());

                // Uninstall unused packages and re-check.
                foreach (var p in unused)
                {
                    needToCheck = true;
                    Log("Uninstall the unused package '{0}@{1}'", p.name, p.version);
                    PackageMeta.GitUnlock(p);
                    FileUtil.DeleteFileOrDirectory(p.repository);
                }
            }
        }

        private static void StartResolve()
        {
            var needToRefresh = false;
            var needToCheck = true;

            Log("Start dependency resolution.");
            PackageMeta.LoadGitLock();
            AssetDatabase.StartAssetEditing();
            while (needToCheck)
            {
                // Uninstall unused packages (for auto-installed packages)
                UninstallUnusedPackages();

                // Collect all installed packages.
                PackageMeta[] installedPackages = GetInstalledPackages();

                // Collect all dependencies.
                var dependencies = installedPackages
                    .SelectMany(x => x.GetAllDependencies()) // Get all dependencies
                    .Where(x => !string.IsNullOrEmpty(x.repository)) // path (url) is available
                    .ToArray();

                // Check all dependencies.
                var requestedPackages = new List<PackageMeta>();
                foreach (var dependency in dependencies)
                {
                    // Is the depended package installed already?
                    var isInstalled = installedPackages
                        .Concat(requestedPackages)
                        .Any(x => dependency.name == x.name && (dependency.version != null && dependency.version <= x.version || x.version == null));

                    if (isInstalled) continue;

                    // Install the depended package later.
                    requestedPackages.RemoveAll(x => dependency.name == x.name);
                    requestedPackages.Add(dependency);
                }

                var sb = new StringBuilder();
                sb.AppendLine("############## StartResolve ##############");
                sb.AppendLine("\n[ installedPackages ] ");
                installedPackages.ToList().ForEach(p => sb.AppendLine(p.ToString()));

                sb.AppendLine("\n[ dependencies ] ");
                dependencies.ToList().ForEach(p => sb.AppendLine(p.ToString()));

                sb.AppendLine("\n[ requestedPackages ] ");
                requestedPackages.ToList().ForEach(p => sb.AppendLine(p.ToString()));
                Log(sb.ToString());

                // No packages is requested to install.
                if (requestedPackages.Count == 0)
                    break;

                // Install all requested packages.
                for (var i = 0; i < requestedPackages.Count; i++)
                {
                    PackageMeta package = requestedPackages[i];
                    DirUtils.Create(Path.Combine("Temp", "GitDependencies"));
                    var clonePath = Path.Combine(Path.Combine("Temp", "GitDependencies"), Path.GetFileName(Path.GetTempFileName()));

                    EditorUtility.DisplayProgressBar("Clone Package", string.Format("Cloning {0}@{1}", package.name, package.version), i / (float) requestedPackages.Count);
                    Log("Cloning '{0}@{1}' ({2}, {3})", package.name, package.version, package.revision, package.path);
                    var success = GitUtils.ClonePackage(package, clonePath);

                    // Check cloned
                    if (!success)
                    {
                        needToCheck = false;
                        Error("Failed to install a package '{0}@{1}': Clone failed.", package.name, package.version);
                        continue;
                    }

                    // Check package path (query)
                    var pkgPath = package.GetPackagePath(clonePath);
                    if (!Directory.Exists(pkgPath))
                    {
                        Error("Failed to install a package '{0}@{1}': The package is not found. {2}/{3}", package.name, package.version, clonePath, package.path);
                        needToCheck = false;
                        continue;
                    }

                    var newPackage = PackageMeta.FromPackageDir(pkgPath);
                    if (newPackage == null)
                    {
                        Error("Failed to install a package '{0}@{1}': package.json is not found. {2}/{3}", package.name, package.version, clonePath, package.path);
                        needToCheck = false;
                        continue;
                    }
                    if (newPackage.name != package.name)
                    {
                        Error("Failed to install a package '{0}@{1}': Different package name. {2} != {3}", package.name, package.version, newPackage.name, package.name);
                        needToCheck = false;
                        continue;
                    }

                    // Install as an embed package
                    var installPath = "Packages/." + newPackage.GetDirectoryName();
                    DirUtils.Delete(installPath);
                    DirUtils.Create(installPath);
                    DirUtils.Move(pkgPath, installPath, p => p != ".git");

                    Log("A package '{0}@{1}' has been installed.", package.name, package.version);
                    needToRefresh = true;
                    AssetDatabase.ImportAsset(installPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ImportRecursive);
                    PackageMeta.GitLock(package);
                }

                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.StopAssetEditing();
            PackageMeta.SaveGitLock();

            Log("Dependency resolution complete. refresh = {0}", needToRefresh);
            if (needToRefresh)
            {
                // Recompile the packages
                AssetDatabase.Refresh();
            }
        }
    }
}
