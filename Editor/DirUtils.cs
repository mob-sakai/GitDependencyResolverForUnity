﻿using System;
using System.IO;
using System.Linq;

namespace Coffee.GitDependencyResolver
{
    /// <summary>
    /// Utility for directory operations.
    /// </summary>
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
                .Select(Path.GetFileName)
                .Where(x => pred == null || pred(x)))
                Directory.Move(Path.Combine(srcDir, name), Path.Combine(dstDir, name));

            // Move files.
            foreach (var name in Directory.GetFiles(srcDir)
                .Select(Path.GetFileName)
                .Where(x => pred == null || pred(x)))
                File.Move(Path.Combine(srcDir, name), Path.Combine(dstDir, name));
        }

        public static void Create(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
