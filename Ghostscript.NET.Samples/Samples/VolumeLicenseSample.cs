//
// VolumeLicenseSample.cs
// This file is part of Ghostscript.NET.Samples project
//
// Author: Artifex Software Inc.
// Copyright (c) 2026 by Artifex Software Inc. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;

namespace Ghostscript.NET.Samples
{
    /// <summary>
    /// Converts one Office file with each key in a local <c>TestFiles/office.keys</c> file
    /// (or <c>GHOSTSCRIPT_NET_OFFICE_KEY_FILE</c> / <c>GHOSTSCRIPT_NET_OFFICE_KEY</c>)
    /// and prints SOPIPE status. The key file is gitignored.
    /// </summary>
    public class VolumeLicenseSample : ISample
    {
        public void Start()
        {
            Environment.SetEnvironmentVariable("GHOSTSCRIPT_NET_OFFICE_ACTIVITY_VERBOSE", "1");

            GhostscriptVersionInfo pdl;
            if (!GhostscriptVersionInfo.TryGetGhostPdlVersion(out pdl))
            {
                Console.WriteLine(GhostscriptOffice.CommercialLicenseRequiredMessage);
                Console.WriteLine("Skipping VolumeLicenseSample: no licensed GhostPDL library was found.");
                return;
            }

            List<KeyEntry> keys = LoadKeys();
            if (keys.Count == 0)
            {
                Console.WriteLine(
                    "Skipping VolumeLicenseSample: create TestFiles/office.keys " +
                    "(Name=key lines) or set GHOSTSCRIPT_NET_OFFICE_KEY / GHOSTSCRIPT_NET_OFFICE_KEY_FILE.");
                return;
            }

            string inputPath = FindSampleOfficeFile();
            if (inputPath == null)
            {
                Console.WriteLine("Skipping VolumeLicenseSample: no .doc/.docx test file found.");
                return;
            }

            Console.WriteLine("Using GhostPDL: " + pdl.DllPath);
            Console.WriteLine("Input: " + inputPath);
            Console.WriteLine("Keys: " + keys.Count);

            string outputDir = Path.GetFullPath("Output");
            Directory.CreateDirectory(outputDir);

            try
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    KeyEntry entry = keys[i];
                    Console.WriteLine();
                    Console.WriteLine("=== " + entry.Name + " (" + (i + 1) + "/" + keys.Count + ") ===");

                    GhostscriptOffice.ClearUnlock();
                    GhostscriptOffice.Unlock(entry.Key);

                    string pdfPath = Path.Combine(outputDir, "VolumeLicenseSample-" + SafeFileName(entry.Name) + ".pdf");
                    if (File.Exists(pdfPath))
                        File.Delete(pdfPath);

                    GhostscriptOffice.ConvertToPdf(inputPath, pdfPath, pdl);

                    int flags = GhostscriptOffice.LastFlags;
                    bool volume = (flags & GhostscriptOffice.KeyFlagVolumeLicense) != 0;
                    bool disableSo = (flags & GhostscriptOffice.KeyFlagDisableSo) != 0;
                    Console.WriteLine("PDF: " + pdfPath);
                    Console.WriteLine("Message: " + GhostscriptOffice.LastMessage);
                    Console.WriteLine("key_ok: " + GhostscriptOffice.LastKeyOk);
                    Console.WriteLine("flags: " + flags + " (0x" + flags.ToString("X") + ")");
                    Console.WriteLine("product: " + GhostscriptOffice.LastProduct);
                    Console.WriteLine("documents: " + GhostscriptOffice.LastDocumentCount);
                    Console.WriteLine("Volume license: " + (volume ? "yes" : "no"));
                    Console.WriteLine("Disable SO: " + (disableSo ? "yes" : "no"));

                    if (volume && !disableSo && GhostscriptOffice.LastDocumentCount > 0)
                    {
                        Console.WriteLine("Flushing activity...");
                        GhostscriptOffice.FlushActivity();
                    }
                    else
                    {
                        Console.WriteLine("Activity monitoring was not started.");
                    }
                }
            }
            finally
            {
                GhostscriptOffice.ClearUnlock();
            }
        }

        struct KeyEntry
        {
            public string Name;
            public string Key;
        }

        static List<KeyEntry> LoadKeys()
        {
            List<KeyEntry> keys = new List<KeyEntry>();
            string path = FindKeyFile();
            if (path != null)
            {
                Console.WriteLine("Key file: " + path);
                foreach (string raw in File.ReadAllLines(path))
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
                        continue;

                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                    {
                        keys.Add(new KeyEntry { Name = "Key" + (keys.Count + 1), Key = line });
                        continue;
                    }

                    string name = line.Substring(0, eq).Trim();
                    string key = line.Substring(eq + 1).Trim();
                    if (name.Length == 0 || key.Length == 0)
                        continue;

                    keys.Add(new KeyEntry { Name = name, Key = key });
                }

                return keys;
            }

            string envKey = Environment.GetEnvironmentVariable("GHOSTSCRIPT_NET_OFFICE_KEY");
            if (!string.IsNullOrWhiteSpace(envKey))
                keys.Add(new KeyEntry { Name = "GHOSTSCRIPT_NET_OFFICE_KEY", Key = envKey.Trim() });

            return keys;
        }

        static string FindKeyFile()
        {
            string env = Environment.GetEnvironmentVariable("GHOSTSCRIPT_NET_OFFICE_KEY_FILE");
            if (!string.IsNullOrWhiteSpace(env) && File.Exists(env))
                return Path.GetFullPath(env);

            string dir = AppContext.BaseDirectory;
            for (int i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
            {
                string candidate = Path.Combine(dir, "TestFiles", "office.keys");
                if (File.Exists(candidate))
                    return candidate;

                candidate = Path.Combine(dir, "Ghostscript.NET.Samples", "TestFiles", "office.keys");
                if (File.Exists(candidate))
                    return candidate;

                dir = Path.GetDirectoryName(dir);
            }

            return null;
        }

        static string SafeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        private static string FindSampleOfficeFile()
        {
            string[] names = { "OfficeSample.doc", "OfficeSample.docx" };
            string dir = AppContext.BaseDirectory;

            for (int i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
            {
                foreach (string name in names)
                {
                    string candidate = Path.Combine(dir, "TestFiles", name);
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }

                    candidate = Path.Combine(dir, name);
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }

                dir = Path.GetDirectoryName(dir);
            }

            return null;
        }
    }
}
