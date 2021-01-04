using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Coffee.GitDependencyResolver
{
    internal static class DirUtils
    {
        public static void Delete(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public static void Move(string srcDir, string dstDir, Func<string, bool> pred = null)
        {
            Create(dstDir);

            // Move directories.
            foreach (var name in Directory.GetDirectories(srcDir)
                .Select(x => Path.GetFileName(x))
                .Where(x => pred == null || pred(x)))
                Directory.Move(Path.Combine(srcDir, name), Path.Combine(dstDir, name));

            // Move files.
            foreach (var name in Directory.GetFiles(srcDir)
                .Select(x => Path.GetFileName(x))
                .Where(x => pred == null || pred(x)))
                File.Move(Path.Combine(srcDir, name), Path.Combine(dstDir, name));
        }

        public static void Copy(string srcDir, string dstDir, Func<string, bool> pred = null)
        {
            Create(dstDir);

            // Move directories.
            foreach (var name in Directory.GetDirectories(srcDir)
                .Select(x => Path.GetFileName(x))
                .Where(x => pred == null || pred(x)))
                DirectoryCopy(Path.Combine(srcDir, name), Path.Combine(dstDir, name), true);

            // Move files.
            DirectoryInfo dir = new DirectoryInfo(srcDir);
            foreach (FileInfo file in dir.GetFiles()
                .Where(f => pred == null || pred(f.Name)))
            {
                string tempPath = Path.Combine(dstDir, file.Name);
                file.CopyTo(tempPath, false);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            
            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);        

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static void Create(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
