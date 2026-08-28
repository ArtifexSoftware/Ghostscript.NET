//
// SampleFiles.cs
// This file is part of Ghostscript.NET.Samples project
//
// Author: Artifex Software Inc.
// Copyright (c) 2026 by Artifex Software Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;

namespace Ghostscript.NET.Samples
{
    internal static class SampleFiles
    {
        public static string Get(string fileName)
        {
            string path;
            if (TryGet(fileName, out path))
            {
                return path;
            }

            throw new FileNotFoundException("Sample file not found: " + fileName);
        }

        public static bool TryGet(string fileName, out string path)
        {
            path = null;

            foreach (string directory in GetSearchDirectories())
            {
                string candidate = Path.Combine(directory, fileName);
                if (File.Exists(candidate))
                {
                    path = Path.GetFullPath(candidate);
                    return true;
                }
            }

            return false;
        }

        public static string OutputDirectory
        {
            get
            {
                string directory = Path.Combine(AppContext.BaseDirectory, "Output");
                Directory.CreateDirectory(directory);
                return directory;
            }
        }

        private static IEnumerable<string> GetSearchDirectories()
        {
            yield return Path.Combine(AppContext.BaseDirectory, "TestFiles");
            yield return Path.GetFullPath("TestFiles");

            string directory = AppContext.BaseDirectory;
            for (int i = 0; i < 6 && !string.IsNullOrEmpty(directory); i++)
            {
                yield return Path.Combine(directory, "TestFiles");
                directory = Path.GetDirectoryName(directory);
            }
        }
    }
}
