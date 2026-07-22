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
        }

        #endregion

        public GhostscriptVersionInfo(string customDllPath)
        {
            _version = new Version();
            _dllPath = customDllPath;
            _libPath = string.Empty;
            _licenseType = GhostscriptLicense.GPL;
            _source = GhostscriptDiscoverySource.Custom;
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

        #region ToString

        /// <summary>
        /// Returns GhostscriptVersionInfo string.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Licence: {0}, Version: {1}, Source: {2}, Dll: {3}, Lib: {4}", _licenseType, _version, _source, _dllPath, _libPath);
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
