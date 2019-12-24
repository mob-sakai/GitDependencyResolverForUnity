using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Coffee.PackageManager.DependencyResolver
{
	public class PackageMeta
	{
		public string name { get; private set; }
		internal SemVersion version { get; private set; }
		public string branch { get; private set; }
		public string path { get; private set; }
		public PackageMeta [] dependencies { get; private set; }

		public static PackageMeta FromPackageJson (string filePath)
		{
			try
			{
				if(!File.Exists(filePath))
				{
					return null;
				}
				
				Dictionary<string, object> dict = Json.Deserialize (File.ReadAllText (filePath)) as Dictionary<string, object>;
				PackageMeta meta = new PackageMeta () { path = Path.GetDirectoryName(filePath), };
				object obj;

				if (dict.TryGetValue ("name", out obj))
				{
					meta.name = obj as string;
				}

				if (dict.TryGetValue ("version", out obj))
				{
					meta.version = obj as string;
				}

                if (dict.TryGetValue("dependencies", out obj))
                {
                    meta.dependencies = (dict["dependencies"] as Dictionary<string, object>)
                        .Select(x => PackageMeta.FromNameAndUrl(x.Key, (string)x.Value))
                        .ToArray();
                }
                else
                {
                    meta.dependencies = new PackageMeta[0];
                }
				return meta;
			}
			catch
			{
				return null;
			}
		}

		public static PackageMeta FromPackageDir (string dir)
		{
			return FromPackageJson(dir + "/package.json");
		}

		public static PackageMeta FromNameAndUrl (string name, string url)
		{
			PackageMeta meta = new PackageMeta () { name = name, dependencies = new PackageMeta [0] };
			Match m = Regex.Match (url, "([^#]*)#?(.*)");
			if (m.Success)
			{
				string first = m.Groups [1].Value;
				string second = m.Groups [2].Value;

				if (first.Contains ("://"))
				{
					meta.path = first;
					meta.branch = 0 < second.Length ? second : "HEAD";
                    SemVersion version;
                    if (!SemVersion.TryParse(meta.branch, out version))
                        version = null;//new SemVersion(0); //empty version
                    meta.version = version;
                    //meta.version = meta.branch;
                }
				else
				{
					meta.version = first;
				}
			}
			return meta;
		}
	}
}