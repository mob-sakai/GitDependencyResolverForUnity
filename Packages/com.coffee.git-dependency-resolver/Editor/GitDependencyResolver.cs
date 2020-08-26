using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Coffee.GitDependencyResolver
{
    [InitializeOnLoad]
    internal static class Resolver
    {
        const System.StringComparison Ordinal = System.StringComparison.Ordinal;

        static Resolver()
        {
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
            bool needToCheck = true;
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

                // Collect unused pakages.
                var unusedPackages = autoInstalledPackages
                        .Where(x => Path.GetFileName(x.path).StartsWith(".", Ordinal)) // Directory name starts with '.'. This is 'auto-installed package'
                        .Where(x => !allDependencies.Any(y => y.name == x.name && (y.version == null || y.version == x.version))) // No depended from other packages
                    ;

                // Uninstall unused packages and re-check.
                foreach (var p in unusedPackages)
                {
                    needToCheck = true;
                    Debug.LogFormat("[Resolver] Uninstall unused package {0}:{1} from {2}", p.name, p.version, p.path);
                    FileUtil.DeleteFileOrDirectory(p.url);
                }
            }
        }

        private static void StartResolve()
        {
            var needToRefresh = false;
            var needToCheck = true;

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
                    .Where(x => !string.IsNullOrEmpty(x.url)) // path (url) is available
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

                // No packages is requested to install.
                if (requestedPackages.Count == 0)
                    break;

                // Install all requested packages.
                for (var i = 0; i < requestedPackages.Count; i++)
                {
                    PackageMeta package = requestedPackages[i];
                    var clonePath = Path.GetTempFileName() + "_";

                    EditorUtility.DisplayProgressBar("Clone Package", string.Format("Cloning {0}@{1}", package.name, package.version), i / (float) requestedPackages.Count);
                    var success = GitUtils.ClonePackage(package, clonePath);

                    // Check cloned
                    if (!success)
                    {
                        needToCheck = false;
                        continue;
                    }

                    // Check package path (query)
                    var pkgPath = package.GetPackagePath(clonePath);
                    PackageMeta newPackage = PackageMeta.FromPackageDir(pkgPath);
                    if (!Directory.Exists(pkgPath) || newPackage == null || newPackage.name != package.name)
                    {
                        needToCheck = false;
                        continue;
                    }

                    // Install as an embed package
                    var installPath = "Packages/." + newPackage.GetDirectoryName();
                    DirUtils.Delete(installPath);
                    DirUtils.Create(installPath);
                    DirUtils.Move(pkgPath, installPath, p => p != ".git");
                    AssetDatabase.ImportAsset(installPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ImportRecursive);

                    needToRefresh = true;
                }

                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.StopAssetEditing();

            if (needToRefresh)
            {
                // Recompile the packages
                AssetDatabase.Refresh();
            }
        }
    }
}
