//
// GhostscriptOffice.cs
// This file is part of Ghostscript.NET library
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
using System.Text;
using Ghostscript.NET.Processor;

namespace Ghostscript.NET
{
    /// <summary>
    /// Microsoft Office / OpenDocument support via GhostPDL (SmartOffice).
    /// Ghostscript itself cannot open these files; GhostPDL converts them
    /// internally (Office → PDF) then runs the Ghostscript pipeline.
    /// </summary>
    public static class GhostscriptOffice
    {
        /// <summary>
        /// Artifex contact page for a commercial Ghostscript.NET license (Office / SmartOffice).
        /// </summary>
        public const string CommercialLicenseUrl = "https://artifex.com/contact/ghostscript";

        /// <summary>
        /// Message shown when Office files are used without a commercial Ghostscript.NET license
        /// (no SmartOffice-enabled GhostPDL library).
        /// </summary>
        public static string CommercialLicenseRequiredMessage
        {
            get
            {
                return
                    "Microsoft Office files are a commercial Ghostscript.NET feature and are not included in the open-source (AGPL) package. " +
                    "To convert Word, Excel, PowerPoint, and related files, obtain a commercial Ghostscript.NET license from Artifex: " +
                    CommercialLicenseUrl + " " +
                    "Licensed customers receive the SmartOffice-enabled GhostPDL native library (Ghostscript.NET.Office).";
            }
        }

        /// <summary>
        /// Filename extensions handled by the GhostPDL SmartOffice language.
        /// </summary>
        public static readonly string[] SupportedExtensions = new[]
        {
            ".doc", ".docx",
            ".xls", ".xlsx",
            ".ppt", ".pptx",
            ".odt", ".ods", ".odp",
            ".rtf", ".csv",
            ".hwp", ".hwpx"
        };

        /// <summary>
        /// Returns true when <paramref name="path"/> has an Office / OpenDocument extension.
        /// </summary>
        public static bool IsOfficeFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return IsOfficeExtension(Path.GetExtension(path));
        }

        /// <summary>
        /// Returns true when <paramref name="extension"/> is a supported Office extension.
        /// </summary>
        public static bool IsOfficeExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return false;
            }

            if (extension[0] != '.')
            {
                extension = "." + extension;
            }

            for (int i = 0; i < SupportedExtensions.Length; i++)
            {
                if (string.Equals(SupportedExtensions[i], extension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true when any path in <paramref name="paths"/> is an Office file.
        /// </summary>
        public static bool ContainsOfficeFile(IEnumerable<string> paths)
        {
            if (paths == null)
            {
                return false;
            }

            foreach (string path in paths)
            {
                if (IsOfficeFile(path))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Rewrites processor arguments so existing Ghostscript samples can open Office files:
        /// full paths, and <c>-dSAFER</c> replaced with <c>-dNOSAFER</c> (SmartOffice must read the input).
        /// </summary>
        internal static string[] PrepareProcessorArgs(string[] args)
        {
            List<string> result = new List<string>(args.Length + 1);
            bool hasNoSafer = false;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (IsSaferSwitch(arg))
                {
                    if (!hasNoSafer)
                    {
                        result.Add("-dNOSAFER");
                        hasNoSafer = true;
                    }
                    continue;
                }

                if (IsNoSaferSwitch(arg))
                {
                    if (!hasNoSafer)
                    {
                        result.Add("-dNOSAFER");
                        hasNoSafer = true;
                    }
                    continue;
                }

                if (IsOfficeFile(arg))
                {
                    result.Add(TryGetFullPath(arg));
                    continue;
                }

                if (arg != null && arg.StartsWith("-sOutputFile=", StringComparison.OrdinalIgnoreCase))
                {
                    string path = arg.Substring("-sOutputFile=".Length);
                    result.Add("-sOutputFile=" + TryGetFullPath(path));
                    continue;
                }

                result.Add(arg);
            }

            if (!hasNoSafer)
            {
                int insertAt = result.Count > 0 ? 1 : 0;
                result.Insert(insertAt, "-dNOSAFER");
            }

            return result.ToArray();
        }

        private static bool IsSaferSwitch(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return false;
            }

            return string.Equals(arg, "-dSAFER", StringComparison.OrdinalIgnoreCase)
                || arg.StartsWith("-dSAFER=", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNoSaferSwitch(string arg)
        {
            return !string.IsNullOrEmpty(arg)
                && string.Equals(arg, "-dNOSAFER", StringComparison.OrdinalIgnoreCase);
        }

        private static string TryGetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return path;
            }
        }

        /// <summary>
        /// Converts an Office file to PDF using GhostPDL.
        /// Input and output paths are resolved to absolute paths (required by SmartOffice).
        /// </summary>
        public static void ConvertToPdf(string inputPath, string outputPath)
        {
            ConvertToPdf(inputPath, outputPath, null);
        }

        /// <summary>
        /// Converts an Office file to PDF using the given GhostPDL library.
        /// </summary>
        public static void ConvertToPdf(string inputPath, string outputPath, GhostscriptVersionInfo pdlVersion)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                throw new ArgumentNullException("inputPath");
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentNullException("outputPath");
            }

            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException("Could not find input file.", inputPath);
            }

            if (pdlVersion == null)
            {
                pdlVersion = GhostscriptVersionInfo.GetGhostPdlVersion();
            }
            else if (!pdlVersion.IsGhostPdl)
            {
                throw new GhostscriptPdlLibraryNotFoundException();
            }

            string inputFull = Path.GetFullPath(inputPath);
            string outputFull = Path.GetFullPath(outputPath);

            string outputDirectory = Path.GetDirectoryName(outputFull);
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using (GhostscriptProcessor processor = new GhostscriptProcessor(pdlVersion, false))
            {
                processor.Process(new[]
                {
                    "-ghostscript.net",
                    "-dNOPAUSE",
                    "-dBATCH",
                    "-dNOPROMPT",
                    "-sDEVICE=pdfwrite",
                    "-sOutputFile=" + outputFull,
                    "-f",
                    inputFull
                });
            }

            if (!File.Exists(outputFull) || new FileInfo(outputFull).Length == 0)
            {
                throw new GhostscriptPdlLibraryNotFoundException(
                    GhostscriptOffice.CommercialLicenseRequiredMessage +
                    " The library at '" + pdlVersion.DllPath + "' did not convert '" + inputFull + "' to PDF. " +
                    "Use the licensed GhostPDL/SmartOffice native library, not standard Ghostscript.");
            }
        }

        /// <summary>
        /// Converts an Office file to a temporary PDF and returns that path.
        /// The caller owns the file and should delete it when finished.
        /// </summary>
        public static string ConvertToTemporaryPdf(string inputPath)
        {
            return ConvertToTemporaryPdf(inputPath, null);
        }

        /// <summary>
        /// Converts an Office file to a temporary PDF using the given GhostPDL library.
        /// </summary>
        public static string ConvertToTemporaryPdf(string inputPath, GhostscriptVersionInfo pdlVersion)
        {
            string outputPath = Path.Combine(Path.GetTempPath(), "gsnet-office-" + Guid.NewGuid().ToString("N") + ".pdf");
            ConvertToPdf(inputPath, outputPath, pdlVersion);
            return outputPath;
        }

        /// <summary>
        /// Tries to infer an Office file extension from a stream's magic bytes.
        /// </summary>
        internal static bool TryDetectOfficeExtension(Stream stream, out string extension)
        {
            extension = null;

            if (stream == null || !stream.CanSeek || stream.Length < 8)
            {
                return false;
            }

            long original = stream.Position;
            try
            {
                stream.Position = 0;
                byte[] header = new byte[Math.Min(4096, (int)stream.Length)];
                int read = stream.Read(header, 0, header.Length);
                if (read < 8)
                {
                    return false;
                }

                // OLE compound document (.doc / .xls / .ppt)
                if (header[0] == 0xD0 && header[1] == 0xCF && header[2] == 0x11 && header[3] == 0xE0 &&
                    header[4] == 0xA1 && header[5] == 0xB1 && header[6] == 0x1A && header[7] == 0xE1)
                {
                    extension = ".doc";
                    return true;
                }

                // ZIP-based OOXML / ODF
                if (header[0] == (byte)'P' && header[1] == (byte)'K')
                {
                    string ascii = Encoding.ASCII.GetString(header, 0, read);
                    if (ascii.IndexOf("word/", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        extension = ".docx";
                        return true;
                    }
                    if (ascii.IndexOf("xl/", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        extension = ".xlsx";
                        return true;
                    }
                    if (ascii.IndexOf("ppt/", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        extension = ".pptx";
                        return true;
                    }
                    if (ascii.IndexOf("opendocument.text", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        extension = ".odt";
                        return true;
                    }
                    if (ascii.IndexOf("opendocument.spreadsheet", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        extension = ".ods";
                        return true;
                    }
                    if (ascii.IndexOf("opendocument.presentation", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        extension = ".odp";
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                stream.Position = original;
            }
        }
    }
}
