//
// GhostscriptVersionInfo.cs
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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Ghostscript.NET
{
    /// <summary>
    /// Represents a Ghostscript version information.
    /// </summary>
    public class GhostscriptVersionInfo
    {

        #region Private local variables

        private Version _version;
        private string _dllPath;
        private string _libPath;
        private GhostscriptLicense _licenseType;
        private GhostscriptDiscoverySource _source;
        private GhostscriptNativeKind _nativeKind;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptVersionInfo class.
        /// </summary>
        /// <param name="version">Ghostscript version.</param>
        /// <param name="dllPath">Ghostscript native library path.</param>
        /// <param name="libPath">Ghostscript lib path.</param>
        /// <param name="licenseType">Ghostscript license type.</param>
        public GhostscriptVersionInfo(Version version, string dllPath, string libPath, GhostscriptLicense licenseType)
            : this(version, dllPath, libPath, licenseType, GhostscriptDiscoverySource.System)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptVersionInfo class.
        /// </summary>
        /// <param name="version">Ghostscript version.</param>
        /// <param name="dllPath">Ghostscript native library path.</param>
        /// <param name="libPath">Ghostscript lib path.</param>
        /// <param name="licenseType">Ghostscript license type.</param>
        /// <param name="source">How this native library was located.</param>
        public GhostscriptVersionInfo(Version version, string dllPath, string libPath, GhostscriptLicense licenseType, GhostscriptDiscoverySource source)
        {
            _version = version;
            _dllPath = dllPath;
            _libPath = libPath;
            _licenseType = licenseType;
            _source = source;
            _nativeKind = DetectNativeKind(dllPath);
        }

        #endregion

        public GhostscriptVersionInfo(string customDllPath)
        {
            _version = new Version();
            _dllPath = customDllPath;
            _libPath = string.Empty;
            _licenseType = GhostscriptLicense.GPL;
            _source = GhostscriptDiscoverySource.Custom;
            _nativeKind = DetectNativeKind(customDllPath);
        }

        #region Version

        /// <summary>
        /// Gets Ghostscript version.
        /// </summary>
        public Version Version
        {
            get { return _version; }
        }

        #endregion

        #region DllPath

        /// <summary>
        /// Gets Ghostscript native library path.
        /// </summary>
        public string DllPath
        {
            get { return _dllPath; }
        }

        #endregion

        #region LibPath

        /// <summary>
        /// Gets Ghostscrip lib path.
        /// </summary>
        public string LibPath
        {
            get { return _libPath; }
        }

        #endregion

        #region LicenseType

        /// <summary>
        /// Gets Ghostscript license type.
        /// </summary>
        public GhostscriptLicense LicenseType
        {
            get { return _licenseType; }
        }

        #endregion

        #region Source

        /// <summary>
        /// Gets how this Ghostscript native library was located.
        /// </summary>
        public GhostscriptDiscoverySource Source
        {
            get { return _source; }
        }

        #endregion

        #region NativeKind

        /// <summary>
        /// Gets whether this library is Ghostscript or GhostPDL.
        /// GhostPDL is required for Microsoft Office files.
        /// </summary>
        public GhostscriptNativeKind NativeKind
        {
            get { return _nativeKind; }
        }

        /// <summary>
        /// True when this native library is GhostPDL (<c>gpdldll64.dll</c> / <c>libgpdl.so</c>).
        /// </summary>
        public bool IsGhostPdl
        {
            get { return _nativeKind == GhostscriptNativeKind.GhostPdl; }
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns GhostscriptVersionInfo string.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Licence: {0}, Version: {1}, Kind: {2}, Source: {3}, Dll: {4}, Lib: {5}", _licenseType, _version, _nativeKind, _source, _dllPath, _libPath);
        }

        #endregion

        #region GetInstalledVersions

        /// <summary>
        /// Gets installed Ghostscript versions list.
        /// </summary>
        public static List<GhostscriptVersionInfo> GetInstalledVersions()
        {
            return GetInstalledVersions(GhostscriptLicense.GPL | GhostscriptLicense.AFPL | GhostscriptLicense.Artifex);
        }

        #endregion

        #region GetInstalledVersions

        /// <summary>
        /// Gets installed Ghostscript versions list.
        /// </summary>
        /// <returns>A GhostscriptVersionInfo list of the Ghostscript installations found on the local system.</returns>
        public static List<GhostscriptVersionInfo> GetInstalledVersions(GhostscriptLicense licenseType)
        {
            // Check platform and use appropriate detection method
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetInstalledVersionsWindows(licenseType);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetInstalledVersionsLinux(licenseType);
            }
            else
            {
                // For other platforms, return empty list
                return new List<GhostscriptVersionInfo>();
            }
        }

        /// <summary>
        /// Gets installed Ghostscript versions on Windows systems.
        /// </summary>
        /// <param name="licenseType">Search for the specific Ghostscript version based on the Ghostscript license.</param>
        /// <returns>List of GhostscriptVersionInfo objects.</returns>
        public static List<GhostscriptVersionInfo> GetInstalledVersionsWindows(GhostscriptLicense licenseType)
        {
            // create a search list instance
            List<GhostscriptLicense> licenses = new List<GhostscriptLicense>();

            // check if we need to search for AFPL installations
            if ((licenseType & GhostscriptLicense.AFPL) == GhostscriptLicense.AFPL)
            {
                // yep, add this license in the search list
                licenses.Add(GhostscriptLicense.AFPL);
            }

            // check if we need to search for GPL installations
            if ((licenseType & GhostscriptLicense.GPL) == GhostscriptLicense.GPL)
            {
                // yep, add this license in the search list
                licenses.Add(GhostscriptLicense.GPL);
            }

            // check if we need to search for GPL installations
            if ((licenseType & GhostscriptLicense.Artifex) == GhostscriptLicense.Artifex)
            {
                // yep, add this license in the search list
                licenses.Add(GhostscriptLicense.Artifex);
            }

            // create new return list instance
            List<GhostscriptVersionInfo> versions = new List<GhostscriptVersionInfo>();

            // loop through the search list
            foreach (GhostscriptLicense license in licenses)
            {
                RegistryKey hklm = null;
                RegistryKey rkGs = null;

                // check if we are running in 64 bit process
                if (Environment.Is64BitProcess)
                {
                    // user 64 bit registry key
                    hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                }
                else
                {
                    // user 32 bit registry key
                    hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                }

                // check the license type
                if (license == GhostscriptLicense.AFPL)
                {
                    // get the AFPL registry key
                    rkGs = hklm.OpenSubKey("SOFTWARE\\AFPL Ghostscript\\");
                }
                else if (license == GhostscriptLicense.GPL)
                {
                    // get the GPL registry key
                    rkGs = hklm.OpenSubKey("SOFTWARE\\GPL Ghostscript\\");
                }
                else if (license == GhostscriptLicense.Artifex)
                {
                    rkGs = hklm.OpenSubKey("SOFTWARE\\Artifex Ghostscript\\");
                }

                // check if we found the registry key
                if (rkGs != null)
                {
                    // get this registry key sub-keys
                    // each sub-key represents a version of the installed Ghostscript library
                    string[] subkeys = rkGs.GetSubKeyNames();

                    // loop through all sub-keys
                    for (int index = 0; index < subkeys.Length; index++)
                    {
                        // get the subkey / Ghostscript library version
                        string versionKey = subkeys[index];

                        try
                        {
                            // open the sub key
                            RegistryKey rkVer = rkGs.OpenSubKey(versionKey);
                            // get the Ghostscript native library path
                            string gsdll = rkVer.GetValue("GS_DLL", string.Empty) as string;
                            // get the Ghostscript lib path
                            string gslib = rkVer.GetValue("GS_LIB", string.Empty) as string;

                            bool compatibile = false;

                            // check if we can use this dll in this process
                            if (System.Environment.Is64BitProcess && gsdll.Contains("gsdll64.dll"))
                            {
                                // both process and dll are 64 bit, we can use it
                                compatibile = true;
                            }
                            else if (!System.Environment.Is64BitProcess && gsdll.Contains("gsdll32.dll"))
                            {
                                // both process and dll are 32 bit, we can use it
                                compatibile = true;
                            }

                            if (compatibile)
                            {
                                // put this version in the return list
                                versions.Add(new GhostscriptVersionInfo(new Version(versionKey), gsdll, gslib, license));
                            }
                        }
                        catch { }
                    }
                }
            }

            return versions;
        }

        #endregion

        #region GetInstalledVersions - Linux

        /// <summary>
        /// Gets installed Ghostscript versions on Linux systems.
        /// </summary>
        /// <param name="licenseType">Search for the specific Ghostscript version based on the Ghostscript license.</param>
        /// <returns>List of GhostscriptVersionInfo objects.</returns>
        public static List<GhostscriptVersionInfo> GetInstalledVersionsLinux(GhostscriptLicense licenseType)
        {
            List<GhostscriptVersionInfo> versions = new List<GhostscriptVersionInfo>();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return versions;
            }

            // Library names to search for (in order of preference - newer versions first)
            string[] libraryNames = { "libgs.so.10", "libgs.so.9", "libgs.so" };

            // Search for libgs.so in common locations
            string[] searchPaths = CrossPlatformNativeLibraryHelper.GetCommonInstallationPaths();

            foreach (string basePath in searchPaths)
            {
                if (!Directory.Exists(basePath))
                {
                    continue;
                }

                // Check for libraries directly in basePath
                foreach (string libName in libraryNames)
                {
                    string libPath = Path.Combine(basePath, libName);
                    if (File.Exists(libPath))
                    {
                        TryAddVersion(versions, libPath, basePath, licenseType);
                        break; // Found a library in this path, no need to check other names
                    }
                }

                // Look in subdirectories
                try
                {
                    string[] subdirs = Directory.GetDirectories(basePath);
                    string[] subdirPaths = { "lib", "lib64", "bin" };

                    foreach (string subdir in subdirs)
                    {
                        foreach (string subdirPath in subdirPaths)
                        {
                            string subdirFullPath = Path.Combine(subdir, subdirPath);
                            if (!Directory.Exists(subdirFullPath))
                            {
                                continue;
                            }

                            foreach (string libName in libraryNames)
                            {
                                string possiblePath = Path.Combine(subdirFullPath, libName);
                                if (File.Exists(possiblePath))
                                {
                                    TryAddVersion(versions, possiblePath, subdir, licenseType);
                                    break; // Found a library in this subdirectory, no need to check other names
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore directory access errors
                }
            }

            return versions;
        }

        /// <summary>
        /// Helper method to try adding a version from a library path.
        /// </summary>
        private static void TryAddVersion(List<GhostscriptVersionInfo> versions, string libPath, string basePath, GhostscriptLicense licenseType)
        {
            try
            {
                Version version = GetVersionFromLinuxLibrary(libPath);
                versions.Add(new GhostscriptVersionInfo(
                    version ?? new Version(0, 0),
                    libPath,
                    basePath,
                    licenseType));
            }
            catch
            {
                // If we can't get version info, create a generic version
                versions.Add(new GhostscriptVersionInfo(new Version(0, 0), libPath, basePath, licenseType));
            }
        }

        #endregion

        #region GetVersionFromLinuxLibrary

        /// <summary>
        /// Attempts to get version information from a Linux library.
        /// </summary>
        /// <param name="libraryPath">Path to the library file.</param>
        /// <returns>Version information or null if not found.</returns>
        private static Version GetVersionFromLinuxLibrary(string libraryPath)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "readlink",
                    Arguments = $"-f \"{libraryPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(psi);
                if (process == null)
                    return null;

                string output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();

                int exitCode = process.ExitCode;
                process.Dispose();

                if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
                {
                    return ParseVersionFromString(output);
                }
            }
            catch
            {
                // If all methods fail, return null
            }

            return null;
        }

        /// <summary>
        /// Parses version information from a string.
        /// </summary>
        private static Version ParseVersionFromString(string input)
        {
            try
            {
                // Look for version patterns like "9.54.0", "10.0.0", etc.
                var versionMatch = System.Text.RegularExpressions.Regex.Match(input, @"(\d+)\.(\d+)\.(\d+)");
                if (versionMatch.Success)
                {
                    int major = int.Parse(versionMatch.Groups[1].Value);
                    int minor = int.Parse(versionMatch.Groups[2].Value);
                    int build = int.Parse(versionMatch.Groups[3].Value);
                    return new Version(major, minor, build);
                }

                // Look for simpler version patterns like "9.54", "10.0"
                versionMatch = System.Text.RegularExpressions.Regex.Match(input, @"(\d+)\.(\d+)");
                if (versionMatch.Success)
                {
                    int major = int.Parse(versionMatch.Groups[1].Value);
                    int minor = int.Parse(versionMatch.Groups[2].Value);
                    return new Version(major, minor, 0);
                }

                // Look for single number versions like "9", "10"
                versionMatch = System.Text.RegularExpressions.Regex.Match(input, @"(\d+)");
                if (versionMatch.Success)
                {
                    int major = int.Parse(versionMatch.Groups[1].Value);
                    return new Version(major, 0, 0);
                }
            }
            catch
            {
                // Version parsing failed
            }

            return null;
        }

        #endregion

        #region GetLastInstalledVersion

        /// <summary>
        /// Gets the preferred Ghostscript version (bundled NativeAssets first, then the newest system install).
        /// </summary>
        public static GhostscriptVersionInfo GetLastInstalledVersion()
        {
            return GetPreferredVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL | GhostscriptLicense.Artifex, GhostscriptLicense.GPL);
        }

        #endregion

        #region GetLastInstalledVersion

        /// <summary>
        /// Gets the preferred Ghostscript version (bundled NativeAssets first, then the newest matching system install).
        /// </summary>
        /// <param name="licenseType">Search for the specific Ghostscript version based on the Ghostscript license.</param>
        /// <param name="licensePriority">If there are both license types installed, which one should have the priority.</param>
        /// <returns>GhostscriptVersionInfo for the preferred Ghostscript native library.</returns>
        public static GhostscriptVersionInfo GetLastInstalledVersion(GhostscriptLicense licenseType, GhostscriptLicense licensePriority)
        {
            return GetPreferredVersion(licenseType, licensePriority);
        }

        #endregion

        #region GetPreferredVersion

        /// <summary>
        /// Gets the preferred Ghostscript version (bundled NativeAssets first, then the newest system install).
        /// </summary>
        public static GhostscriptVersionInfo GetPreferredVersion()
        {
            return GetPreferredVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL | GhostscriptLicense.Artifex, GhostscriptLicense.GPL);
        }

        /// <summary>
        /// Gets the preferred Ghostscript version (bundled NativeAssets first, then the newest matching system install).
        /// </summary>
        /// <param name="licenseType">Search for the specific Ghostscript version based on the Ghostscript license.</param>
        /// <param name="licensePriority">If there are both license types installed, which one should have the priority.</param>
        /// <returns>GhostscriptVersionInfo for the preferred Ghostscript native library.</returns>
        public static GhostscriptVersionInfo GetPreferredVersion(GhostscriptLicense licenseType, GhostscriptLicense licensePriority)
        {
            GhostscriptVersionInfo bundled;
            if (TryGetBundledVersion(out bundled))
            {
                return bundled;
            }

            GhostscriptVersionInfo system = TryGetLastInstalledSystemVersion(licenseType, licensePriority);
            if (system != null)
            {
                return system;
            }

            throw new GhostscriptLibraryNotInstalledException();
        }

        /// <summary>
        /// Preferred Ghostscript library for viewing. Falls back to Ghostscript next to GhostPDL,
        /// then GhostPDL itself, so Office files work when only a GhostPDL install is present.
        /// </summary>
        public static GhostscriptVersionInfo GetPreferredVersionOrPdl()
        {
            GhostscriptVersionInfo bundled;
            if (TryGetBundledVersion(out bundled))
            {
                return bundled;
            }

            GhostscriptVersionInfo system = TryGetLastInstalledSystemVersion(
                GhostscriptLicense.GPL | GhostscriptLicense.AFPL | GhostscriptLicense.Artifex,
                GhostscriptLicense.GPL);
            if (system != null)
            {
                return system;
            }

            GhostscriptVersionInfo pdl;
            if (TryGetGhostPdlVersion(out pdl))
            {
                GhostscriptVersionInfo gsBesidePdl;
                if (TryGetGhostscriptBeside(pdl.DllPath, out gsBesidePdl))
                {
                    return gsBesidePdl;
                }

                return pdl;
            }

            throw new GhostscriptLibraryNotInstalledException();
        }

        private static bool TryGetGhostscriptBeside(string nativeLibraryPath, out GhostscriptVersionInfo version)
        {
            version = null;

            if (string.IsNullOrWhiteSpace(nativeLibraryPath))
            {
                return false;
            }

            string directory = Path.GetDirectoryName(nativeLibraryPath);
            string found = FindLibraryInDirectory(directory, GetBundledLibraryNames());
            if (string.IsNullOrEmpty(found) || CrossPlatformNativeLibraryHelper.IsGhostPdlLibrary(found))
            {
                return false;
            }

            if (!CrossPlatformNativeLibraryHelper.IsLibraryCompatible(found))
            {
                return false;
            }

            Version gsVersion = ReadBundledVersionMetadata(directory) ?? ParseVersionFromString(Path.GetFileName(found)) ?? new Version(0, 0);
            version = new GhostscriptVersionInfo(
                gsVersion,
                found,
                directory,
                GhostscriptLicense.Artifex,
                GhostscriptDiscoverySource.Custom);
            return true;
        }

        #endregion

        #region TryGetBundledVersion / GetBundledVersion

        /// <summary>
        /// Tries to locate a Ghostscript native library shipped with the application
        /// (for example via the Ghostscript.NativeAssets package).
        /// </summary>
        /// <param name="version">Located bundled version information when found.</param>
        /// <returns>True when a compatible bundled native library was found.</returns>
        public static bool TryGetBundledVersion(out GhostscriptVersionInfo version)
        {
            version = null;

            string libraryPath = FindBundledLibraryPath();
            if (string.IsNullOrEmpty(libraryPath))
            {
                return false;
            }

            if (!CrossPlatformNativeLibraryHelper.IsLibraryCompatible(libraryPath))
            {
                return false;
            }

            string directory = Path.GetDirectoryName(libraryPath) ?? string.Empty;
            Version gsVersion = ReadBundledVersionMetadata(directory) ?? ParseVersionFromString(Path.GetFileName(libraryPath)) ?? new Version(0, 0);

            version = new GhostscriptVersionInfo(
                gsVersion,
                libraryPath,
                directory,
                GhostscriptLicense.Artifex,
                GhostscriptDiscoverySource.Bundled);

            return true;
        }

        /// <summary>
        /// Gets the Ghostscript native library shipped with the application
        /// (for example via the Ghostscript.NativeAssets package).
        /// </summary>
        /// <returns>GhostscriptVersionInfo for the bundled native library.</returns>
        /// <exception cref="GhostscriptLibraryNotInstalledException">Thrown when no bundled library is found.</exception>
        public static GhostscriptVersionInfo GetBundledVersion()
        {
            GhostscriptVersionInfo version;
            if (TryGetBundledVersion(out version))
            {
                return version;
            }

            throw new GhostscriptLibraryNotInstalledException();
        }

        #endregion

        #region Bundled discovery helpers

        private static GhostscriptVersionInfo TryGetLastInstalledSystemVersion(GhostscriptLicense licenseType, GhostscriptLicense licensePriority)
        {
            List<GhostscriptVersionInfo> gsVerList = GetInstalledVersions(licenseType);
            int versionsCount = gsVerList.Count;

            if (versionsCount == 1)
            {
                return gsVerList[0];
            }

            if (versionsCount > 1)
            {
                GhostscriptVersionInfo lastGsVer = gsVerList[0];

                for (int index = 1; index < versionsCount; index++)
                {
                    GhostscriptVersionInfo gs = gsVerList[index];

                    if (gs.Version > lastGsVer.Version)
                    {
                        if (gs.LicenseType == licensePriority)
                        {
                            lastGsVer = gsVerList[index];
                        }
                    }
                }

                return lastGsVer;
            }

            return null;
        }

        private static string FindBundledLibraryPath()
        {
            string baseDirectory = AppContext.BaseDirectory;
            if (string.IsNullOrEmpty(baseDirectory))
            {
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }

            string rid = GetCurrentRuntimeIdentifier();
            string[] libraryNames = GetBundledLibraryNames();

            List<string> probeDirectories = new List<string>();
            AddProbeDirectory(probeDirectories, baseDirectory);
            AddProbeDirectory(probeDirectories, Path.Combine(baseDirectory, "native"));

            if (!string.IsNullOrEmpty(rid))
            {
                AddProbeDirectory(probeDirectories, Path.Combine(baseDirectory, "runtimes", rid, "native"));
                AddProbeDirectory(probeDirectories, Path.Combine(baseDirectory, rid, "native"));
            }

            foreach (string directory in probeDirectories)
            {
                foreach (string libraryName in libraryNames)
                {
                    string candidate = Path.Combine(directory, libraryName);
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        private static void AddProbeDirectory(List<string> directories, string directory)
        {
            if (!string.IsNullOrEmpty(directory) && !directories.Contains(directory))
            {
                directories.Add(directory);
            }
        }

        private static string[] GetBundledLibraryNames()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new[]
                {
                    CrossPlatformNativeLibraryHelper.GetGhostscriptLibraryName(Environment.Is64BitProcess)
                };
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new[] { "libgs.so.10", "libgs.so.9", "libgs.so" };
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new[] { "libgs.dylib", "libgs.so" };
            }

            return new[]
            {
                CrossPlatformNativeLibraryHelper.GetGhostscriptLibraryName(Environment.Is64BitProcess)
            };
        }

        private static string GetCurrentRuntimeIdentifier()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                switch (RuntimeInformation.ProcessArchitecture)
                {
                    case Architecture.X86:
                        return "win-x86";
                    case Architecture.Arm64:
                        return "win-arm64";
                    default:
                        return "win-x64";
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                switch (RuntimeInformation.ProcessArchitecture)
                {
                    case Architecture.Arm64:
                        return "linux-arm64";
                    case Architecture.Arm:
                        return "linux-arm";
                    default:
                        return "linux-x64";
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                switch (RuntimeInformation.ProcessArchitecture)
                {
                    case Architecture.Arm64:
                        return "osx-arm64";
                    default:
                        return "osx-x64";
                }
            }

            return null;
        }

        private static Version ReadBundledVersionMetadata(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return null;
            }

            string versionFile = Path.Combine(directory, "ghostscript.version");
            if (!File.Exists(versionFile))
            {
                return null;
            }

            try
            {
                string text = File.ReadAllText(versionFile).Trim();
                return ParseVersionFromString(text);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region GetPreferredVersionForInput

        /// <summary>
        /// Returns GhostPDL when <paramref name="inputPath"/> is an Office file, otherwise the preferred Ghostscript library.
        /// </summary>
        public static GhostscriptVersionInfo GetPreferredVersionForInput(string inputPath)
        {
            if (GhostscriptOffice.IsOfficeFile(inputPath))
            {
                return GetGhostPdlVersion();
            }

            return GetPreferredVersion();
        }

        #endregion

        #region GhostPDL discovery

        /// <summary>
        /// True when a GhostPDL native library can be located.
        /// </summary>
        public static bool IsGhostPdlInstalled
        {
            get
            {
                GhostscriptVersionInfo version;
                return TryGetGhostPdlVersion(out version);
            }
        }

        /// <summary>
        /// Gets a GhostPDL native library. Throws if none is found.
        /// Office files require GhostPDL, not standard Ghostscript.
        /// </summary>
        public static GhostscriptVersionInfo GetGhostPdlVersion()
        {
            GhostscriptVersionInfo version;
            if (TryGetGhostPdlVersion(out version))
            {
                return version;
            }

            throw new GhostscriptPdlLibraryNotFoundException();
        }

        /// <summary>
        /// Tries to locate a GhostPDL native library.
        /// Search order: <c>GHOSTPDL_DLL</c> / <c>GPDL_DLL</c>, the application folder
        /// (drop-in <c>gpdldll</c> / <c>libgpdl</c> from Ghostscript.NET.Office),
        /// then the same directory as a discovered Ghostscript DLL.
        /// </summary>
        public static bool TryGetGhostPdlVersion(out GhostscriptVersionInfo version)
        {
            version = null;

            string libraryPath = FindGhostPdlLibraryPath();
            if (string.IsNullOrEmpty(libraryPath) || !File.Exists(libraryPath))
            {
                return false;
            }

            if (!CrossPlatformNativeLibraryHelper.IsLibraryCompatible(libraryPath))
            {
                return false;
            }

            string directory = Path.GetDirectoryName(libraryPath) ?? string.Empty;
            Version gsVersion = ReadBundledVersionMetadata(directory) ?? ParseVersionFromString(Path.GetFileName(libraryPath)) ?? new Version(0, 0);

            version = new GhostscriptVersionInfo(
                gsVersion,
                libraryPath,
                directory,
                GhostscriptLicense.Artifex,
                FileIsBundled(libraryPath) ? GhostscriptDiscoverySource.Bundled : GhostscriptDiscoverySource.Custom);

            return true;
        }

        private static string FindGhostPdlLibraryPath()
        {
            string envPath = Environment.GetEnvironmentVariable("GHOSTPDL_DLL");
            if (string.IsNullOrWhiteSpace(envPath))
            {
                envPath = Environment.GetEnvironmentVariable("GPDL_DLL");
            }

            if (!string.IsNullOrWhiteSpace(envPath) && File.Exists(envPath))
            {
                return envPath;
            }

            string[] libraryNames = CrossPlatformNativeLibraryHelper.GetGhostPdlLibraryNames(Environment.Is64BitProcess);

            string bundled = FindLibraryInProbeDirectories(libraryNames);
            if (!string.IsNullOrEmpty(bundled))
            {
                return bundled;
            }

            GhostscriptVersionInfo gs;
            if (TryGetBundledVersion(out gs))
            {
                string besideGs = FindLibraryInDirectory(Path.GetDirectoryName(gs.DllPath), libraryNames);
                if (!string.IsNullOrEmpty(besideGs))
                {
                    return besideGs;
                }
            }

            GhostscriptVersionInfo systemGs = TryGetLastInstalledSystemVersion(
                GhostscriptLicense.GPL | GhostscriptLicense.AFPL | GhostscriptLicense.Artifex,
                GhostscriptLicense.GPL);
            if (systemGs != null)
            {
                string besideGs = FindLibraryInDirectory(Path.GetDirectoryName(systemGs.DllPath), libraryNames);
                if (!string.IsNullOrEmpty(besideGs))
                {
                    return besideGs;
                }
            }

            return null;
        }

        private static string FindLibraryInProbeDirectories(string[] libraryNames)
        {
            string baseDirectory = AppContext.BaseDirectory;
            if (string.IsNullOrEmpty(baseDirectory))
            {
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }

            string rid = GetCurrentRuntimeIdentifier();

            List<string> probeDirectories = new List<string>();
            AddProbeDirectory(probeDirectories, baseDirectory);
            AddProbeDirectory(probeDirectories, Path.Combine(baseDirectory, "native"));

            if (!string.IsNullOrEmpty(rid))
            {
                AddProbeDirectory(probeDirectories, Path.Combine(baseDirectory, "runtimes", rid, "native"));
                AddProbeDirectory(probeDirectories, Path.Combine(baseDirectory, rid, "native"));
            }

            foreach (string directory in probeDirectories)
            {
                string found = FindLibraryInDirectory(directory, libraryNames);
                if (!string.IsNullOrEmpty(found))
                {
                    return found;
                }
            }

            return null;
        }

        private static string FindLibraryInDirectory(string directory, string[] libraryNames)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory) || libraryNames == null)
            {
                return null;
            }

            foreach (string libraryName in libraryNames)
            {
                string candidate = Path.Combine(directory, libraryName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static bool FileIsBundled(string libraryPath)
        {
            string baseDirectory = AppContext.BaseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(baseDirectory) || string.IsNullOrEmpty(libraryPath))
            {
                return false;
            }

            try
            {
                string fullLibrary = Path.GetFullPath(libraryPath);
                string fullBase = Path.GetFullPath(baseDirectory);
                return fullLibrary.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static GhostscriptNativeKind DetectNativeKind(string dllPath)
        {
            return CrossPlatformNativeLibraryHelper.IsGhostPdlLibrary(dllPath)
                ? GhostscriptNativeKind.GhostPdl
                : GhostscriptNativeKind.Ghostscript;
        }

        #endregion

        #region IsGhostscriptInstalled

        /// <summary>
        /// Gets if Ghostscript is available via a bundled native library or a system installation.
        /// </summary>
        public static bool IsGhostscriptInstalled
        {
            get
            {
                GhostscriptVersionInfo bundled;
                if (TryGetBundledVersion(out bundled))
                {
                    return true;
                }

                return GetInstalledVersions(GhostscriptLicense.GPL | GhostscriptLicense.AFPL | GhostscriptLicense.Artifex).Count > 0;
            }
        }

        #endregion

    }
}
