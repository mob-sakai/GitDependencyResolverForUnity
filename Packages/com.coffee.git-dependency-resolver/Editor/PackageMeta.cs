using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Coffee.GitDependencyResolver
{
    internal class PackageMeta
    {
#if NETSTANDARD
        const RegexOptions k_RegOption = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
#else
        const RegexOptions k_RegOption = RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture;
#endif
        private static readonly Regex s_PackageUrlReg =
            new Regex(
                @"^(git\+)?" +
                @"(?<url>[^#?]*)" +
                @"(#(?<rev>.*))?",
                k_RegOption);

        private static readonly Regex s_IsGitReg =
            new Regex(
                @"^(git\\+)?" +
                @"(git@|git://|http://|https://|ssh://)",
                k_RegOption);

        public string name { get; private set; }
        internal SemVersion version { get; private set; }
        public string rev { get; private set; }
        public string url { get; private set; }
        public PackageMeta[] dependencies { get; private set; }

        private PackageMeta()
        {
            name = "";
            url = "";
            rev = "";
            version = new SemVersion(0);
            dependencies = new PackageMeta [0];
        }

        public static PackageMeta FromPackageJson(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }

                string dir = Path.GetDirectoryName(filePath);
                Dictionary<string, object> dict = Json.Deserialize(File.ReadAllText(filePath)) as Dictionary<string, object>;
                PackageMeta package = new PackageMeta() {url = dir,};


                object obj;

                if (dict.TryGetValue("name", out obj))
                {
                    package.name = obj as string;
                }

                if (dict.TryGetValue("version", out obj))
                {
                    package.version = obj as string;
                }

                if (dict.TryGetValue("dependencies", out obj))
                {
                    package.dependencies = (obj as Dictionary<string, object>)
                        .Select(x => FromNameAndUrl(x.Key, (string) x.Value))
                        .ToArray();
                }
                else
                {
                    package.dependencies = new PackageMeta[0];
                }

                return package;
            }
            catch
            {
                return null;
            }
        }

        public static PackageMeta FromPackageDir(string dir)
        {
            var package = FromPackageJson(dir + "/package.json");
            package.SetVersion(package.rev);
            return package;
        }

        public static PackageMeta FromNameAndUrl(string name, string url)
        {
            PackageMeta package = new PackageMeta() {name = name};

            // Non git package.
            var isGit = s_IsGitReg.IsMatch(url);
            if (!isGit)
            {
                package.SetVersion(url);
                return package;
            }

            // No package url
            Match m = s_PackageUrlReg.Match(url);
            if (!m.Success) return package;

            package.url = m.Groups["url"].Value;
            package.rev = m.Groups["rev"].Value;

            // Get version from revision/branch/tag
            package.SetVersion(package.rev);

            return package;
        }

        public string GetPackagePath(string clonePath)
        {
            return clonePath;
        }

        private void SetVersion(string ver)
        {
            SemVersion v;
            if (SemVersion.TryParse(ver, out v) && version < v)
                version = v;
        }

        public IEnumerable<PackageMeta> GetAllDependencies()
        {
            return dependencies;
        }

        public string GetDirectoryName()
        {
            return string.Format("{0}@{1}", name, version);
        }

        public override string ToString()
        {
            return string.Format("{0}@{1} ({2})", name, version, rev);
        }
    }
}
