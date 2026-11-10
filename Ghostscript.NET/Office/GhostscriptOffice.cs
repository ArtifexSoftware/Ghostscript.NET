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
    /// <remarks>
    /// An Office job in <c>GhostscriptProcessor</c> does this:
    /// <list type="number">
    /// <item><c>Unlock</c> stores the key string only. C# does not decode it.</item>
    /// <item>The processor creates an anonymous pipe and passes <c>-sSOKEY</c> and
    /// <c>-sSOPIPE</c> (a decimal HANDLE / fd, no quotes).</item>
    /// <item>GhostPDL writes one JSON block and a 0 the first time it opens an
    /// Office file, then one extra 0 per Office file. Nothing is written if no
    /// Office file is opened.</item>
    /// <item>A reader thread consumes those bytes during <c>gsapi_init_with_args</c>.
    /// Each extra 0 can start a volume POST without waiting for GhostPDL to exit.</item>
    /// <item>After <c>gsapi_delete_instance</c> the write handle is closed, the
    /// reader sees EOF, and any remaining count is posted.</item>
    /// </list>
    /// <c>LastMessage</c> and the other <c>Last*</c> properties are the most recent
    /// job. Two processors at once would overwrite them.
    /// </remarks>
    public static class GhostscriptOffice
    {
        /// <summary>Volume-license flag bit (bit 0) reported by GhostPDL.</summary>
        public const int KeyFlagVolumeLicense = 1;

        /// <summary>Disable-SmartOffice flag bit (bit 1) reported by GhostPDL.</summary>
        public const int KeyFlagDisableSo = 2;

        /// <summary>Ghostscript.NET.Office product id in the SOPIPE <c>product</c> field.</summary>
        public const int ProductGsNetOffice = 1;

        /// <summary>All-products id in the SOPIPE <c>product</c> field.</summary>
        public const int ProductAll = 7;

        private static readonly object UnlockSync = new object();
        private static string _unlockKey;
        private static string _lastMessage = string.Empty;
        private static int _lastFlags;
        private static int _lastProduct = -1;
        private static int _lastKeyOk = -1;
        private static int _lastDocumentCount;
        private static string _observeKey;
        private static bool _observeVolume;

        /// <summary>
        /// Artifex contact page for a commercial Ghostscript.NET license (Office / SmartOffice).
        /// </summary>
        public const string CommercialLicenseUrl = "https://artifex.com/contact/ghostscript";

        /// <summary>
        /// Message shown when Office conversion cannot use GhostPDL, or when a key is required for the full document.
        /// </summary>
        public static string CommercialLicenseRequiredMessage
        {
            get
            {
                return
                    "Microsoft Office conversion uses GhostPDL. Ghostscript.NativeAssets includes that library. " +
                    "Call GhostscriptOffice.Unlock with a Ghostscript.NET.Office key for the full document; without a key, only the first 3 pages are converted. " +
                    "License keys: " + CommercialLicenseUrl + ".";
            }
        }

        /// <summary>
        /// Supplies the Ghostscript.NET.Office license key. Stores the trimmed string
        /// only; GhostPDL checks it. Ghostscript.NET adds <c>-sSOKEY</c> automatically.
        /// Without this call, a key-enabled GhostPDL build runs in restricted mode (first 3 pages).
        /// Volume posts use <c>GHOSTSCRIPT_NET_OFFICE_VOLUME_MONITOR_URL</c> when set, otherwise the default volume-monitor URL.
        /// </summary>
        public static void Unlock(string key)
        {
            Unlock(key, null);
        }

        /// <summary>
        /// Supplies the Ghostscript.NET.Office license key and an optional volume-monitor URL.
        /// A non-empty <paramref name="volumeMonitorUrl"/> is used for later volume POSTs.
        /// If it is null or whitespace, <c>GHOSTSCRIPT_NET_OFFICE_VOLUME_MONITOR_URL</c> is used when set,
        /// otherwise <c>https://lp2piszqpd.execute-api.us-east-1.amazonaws.com/usage</c>.
        /// </summary>
        public static void Unlock(string key, string volumeMonitorUrl)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            lock (UnlockSync)
            {
                _unlockKey = key.Trim();
            }

            ActivityMonitor.SetUnlockVolumeMonitorUrl(volumeMonitorUrl);
        }

        /// <summary>
        /// Clears a key previously passed to <see cref="Unlock"/>. Later Office jobs run restricted
        /// until <see cref="Unlock"/> is called again. Also clears an Unlock volume-monitor URL so the
        /// environment variable or the default URL is used again.
        /// </summary>
        public static void ClearUnlock()
        {
            lock (UnlockSync)
            {
                _unlockKey = null;
            }

            ActivityMonitor.SetUnlockVolumeMonitorUrl(null);
        }

        /// <summary>
        /// True when <see cref="Unlock"/> has been given a non-empty key.
        /// Does not validate the key with GhostPDL.
        /// </summary>
        public static bool IsUnlocked
        {
            get
            {
                lock (UnlockSync)
                {
                    return !string.IsNullOrEmpty(_unlockKey);
                }
            }
        }

        /// <summary>
        /// Text from the last Office job's SOPIPE <c>message</c> field.
        /// Empty when GhostPDL wrote no message. A missing leading <c>{ "message"</c>
        /// (restricted mode on the current DLL) is still recovered.
        /// </summary>
        public static string LastMessage
        {
            get
            {
                lock (UnlockSync)
                {
                    return _lastMessage;
                }
            }
        }

        /// <summary>
        /// <c>flags</c> from the last Office job's SOPIPE record, or 0 when the pipe had none.
        /// Bit 0 is <see cref="KeyFlagVolumeLicense"/>. Bit 1 is <see cref="KeyFlagDisableSo"/>.
        /// </summary>
        public static int LastFlags
        {
            get
            {
                lock (UnlockSync)
                {
                    return _lastFlags;
                }
            }
        }

        /// <summary>
        /// <c>product</c> from the last Office job's SOPIPE record.
        /// <see cref="ProductGsNetOffice"/> is Ghostscript.NET.Office, <see cref="ProductAll"/> is every product.
        /// -1 when the pipe had no product field.
        /// </summary>
        public static int LastProduct
        {
            get
            {
                lock (UnlockSync)
                {
                    return _lastProduct;
                }
            }
        }

        /// <summary>
        /// <c>key_ok</c> from the last Office job's SOPIPE record, or -1 when the field was absent.
        /// GhostPDL reports 0 when it rejects the key or when no key was passed. Restricted
        /// mode still converts the first 3 pages. Volume reporting uses <see cref="LastFlags"/>, not this value.
        /// </summary>
        public static int LastKeyOk
        {
            get
            {
                lock (UnlockSync)
                {
                    return _lastKeyOk;
                }
            }
        }

        /// <summary>
        /// Office files GhostPDL counted on the last job (extra zero bytes after the JSON terminator).
        /// </summary>
        public static int LastDocumentCount
        {
            get
            {
                lock (UnlockSync)
                {
                    return _lastDocumentCount;
                }
            }
        }

        /// <summary>
        /// Sends queued volume-license activity and stops the reporters.
        /// Process exit already does this (10 second cap). Call from short-lived tools
        /// that need the POST to finish before they exit.
        /// </summary>
        public static void FlushActivity()
        {
            ActivityMonitor.FlushForExit();
        }

        private static string GetUnlockKey()
        {
            lock (UnlockSync)
            {
                return _unlockKey;
            }
        }

        internal static string ReadOfficeKey(string[] args)
        {
            if (args == null)
            {
                return null;
            }

            string key = null;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (!string.IsNullOrEmpty(arg) &&
                    arg.StartsWith("-sSOKEY=", StringComparison.OrdinalIgnoreCase))
                {
                    key = arg.Substring("-sSOKEY=".Length);
                }
            }

            return string.IsNullOrEmpty(key) ? null : key;
        }

        /// <summary>
        /// Inserts unquoted <c>-sSOPIPE=&lt;handle&gt;</c>. GhostPDL reads a decimal
        /// HANDLE on Windows and a decimal fd on Unix. Quotes would make that fail.
        /// </summary>
        internal static string[] AddSoPipeSwitch(string[] args, string handle)
        {
            string pipeSwitch = "-sSOPIPE=" + handle;
            List<string> result = new List<string>(args);
            for (int i = 0; i < result.Count; i++)
            {
                string arg = result[i];
                if (!string.IsNullOrEmpty(arg) &&
                    arg.StartsWith("-sSOPIPE=", StringComparison.OrdinalIgnoreCase))
                {
                    result[i] = pipeSwitch;
                    return result.ToArray();
                }
            }

            int insertAt = result.Count > 0 ? 1 : 0;
            result.Insert(insertAt, pipeSwitch);
            return result.ToArray();
        }

        /// <summary>
        /// Clears Last* before a new Office job. The reader thread fills them again.
        /// </summary>
        internal static void BeginPipeObservation()
        {
            lock (UnlockSync)
            {
                _lastMessage = string.Empty;
                _lastFlags = 0;
                _lastProduct = -1;
                _lastKeyOk = -1;
                _lastDocumentCount = 0;
                _observeKey = null;
                _observeVolume = false;
            }
        }

        /// <summary>
        /// First 0 on the pipe: JSON status. Does not count a document and does not POST.
        /// </summary>
        internal static void ObservePipeHeader(string key, byte[] header, Action<string> printMessage)
        {
            OfficePipeReport report = OfficePipeReport.Parse(header ?? new byte[0]);
            string message;
            lock (UnlockSync)
            {
                _lastMessage = report.Message ?? string.Empty;
                _lastFlags = report.Flags;
                _lastProduct = report.Product;
                _lastKeyOk = report.KeyOk;
                _lastDocumentCount = 0;
                _observeKey = key;
                _observeVolume = QualifiesForVolume(key, report.Flags, report.Product);
                message = _lastMessage;
            }

            if (printMessage != null && !string.IsNullOrEmpty(message))
            {
                printMessage(message + "\n");
            }
        }

        /// <summary>
        /// Extra 0 after the JSON terminator: one finished Office file.
        /// Starts a reporter for this Ghostscript call on the first qualifying file.
        /// </summary>
        internal static ActivityMonitor ObserveOfficeDocument(ActivityMonitor monitor)
        {
            string key;
            lock (UnlockSync)
            {
                _lastDocumentCount++;
                if (!_observeVolume)
                {
                    return monitor;
                }

                key = _observeKey;
            }

            if (string.IsNullOrEmpty(key))
            {
                return monitor;
            }

            if (monitor == null)
            {
                monitor = ActivityMonitor.Start(key);
            }

            monitor.RecordDocument();
            return monitor;
        }

        private static bool QualifiesForVolume(string key, int flags, int product)
        {
            if (product < 0)
            {
                product = (flags >> 2) & 7;
            }

            bool volume = (flags & KeyFlagVolumeLicense) != 0;
            bool disableSo = (flags & KeyFlagDisableSo) != 0;
            bool productOk = product == ProductGsNetOffice || product == ProductAll;
            return volume && !disableSo && productOk && !string.IsNullOrEmpty(key);
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
        /// full paths, <c>-dSAFER</c> replaced with <c>-dNOSAFER</c> (SmartOffice must read the input),
        /// and <c>-sSOKEY</c> from <see cref="Unlock"/> when a key was stored.
        /// </summary>
        internal static string[] PrepareProcessorArgs(string[] args)
        {
            List<string> result = new List<string>(args.Length + 2);
            bool hasNoSafer = false;
            string unlockKey = GetUnlockKey();
            bool keepCallerSokey = unlockKey == null;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (IsSokeySwitch(arg))
                {
                    if (keepCallerSokey)
                    {
                        result.Add(arg);
                    }

                    continue;
                }

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

            int insertAt = result.Count > 0 ? 1 : 0;

            if (!hasNoSafer)
            {
                result.Insert(insertAt, "-dNOSAFER");
                insertAt++;
            }

            if (unlockKey != null)
            {
                result.Insert(insertAt, "-sSOKEY=" + unlockKey);
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

        private static bool IsSokeySwitch(string arg)
        {
            return !string.IsNullOrEmpty(arg)
                && arg.StartsWith("-sSOKEY=", StringComparison.OrdinalIgnoreCase);
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
