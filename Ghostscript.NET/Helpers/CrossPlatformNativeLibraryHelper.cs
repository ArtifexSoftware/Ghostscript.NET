// Copyright (C) 2025 Artifex Software, Inc.
//
// This file is part of Ghostscript.NET.
//
// Ghostscript.NET is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// Ghostscript.NET is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
// details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Ghostscript.NET. If not, see 
// <https://www.gnu.org/licenses/agpl-3.0.en.html>
//
// Alternative licensing terms are available from the licensor.
// For commercial licensing, see <https://www.artifex.com/> or contact
// Artifex Software, Inc., 39 Mesa Street, Suite 108A, San Francisco,
// CA 94129, USA, for further information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.WinAny;

namespace Ghostscript.NET
{
    /// <summary>
    /// Cross-platform helper for native library operations.
    /// </summary>
    internal static class CrossPlatformNativeLibraryHelper
    {
        /// <summary>
        /// Gets the current operating system platform.
        /// </summary>
        public static OSPlatform CurrentPlatform
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return OSPlatform.Windows;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    return OSPlatform.Linux;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return OSPlatform.OSX;
                else
                    return OSPlatform.Windows;
            }
        }

        /// <summary>
        /// Gets the appropriate library file extension for the current platform.
        /// </summary>
        public static string LibraryExtension
        {
            get
            {
                if (CurrentPlatform == OSPlatform.Windows)
                    return ".dll";
                else if (CurrentPlatform == OSPlatform.Linux)
                    return ".so";
                else if (CurrentPlatform == OSPlatform.OSX)
                    return ".dylib";
                else
                    return ".dll";
            }
        }

        /// <summary>
        /// Gets the expected Ghostscript library name for the current platform.
        /// </summary>
        public static string GetGhostscriptLibraryName(bool is64Bit)
        {
            if (CurrentPlatform == OSPlatform.Windows)
                return is64Bit ? "gsdll64.dll" : "gsdll32.dll";
            else if (CurrentPlatform == OSPlatform.Linux)
                return "libgs.so";
            else if (CurrentPlatform == OSPlatform.OSX)
                return "libgs.dylib";
            else
                return is64Bit ? "gsdll64.dll" : "gsdll32.dll";
        }

        /// <summary>
        /// Checks if a native library is compatible with the current process architecture.
        /// </summary>
        /// <param name="libraryPath">Path to the native library.</param>
        /// <returns>True if the library is compatible.</returns>
        public static bool IsLibraryCompatible(string libraryPath)
        {
            if (!File.Exists(libraryPath))
                return false;

            if (CurrentPlatform == OSPlatform.Windows)
                return NativeLibraryHelper.Is64BitLibrary(libraryPath) == Environment.Is64BitProcess;
            else if (CurrentPlatform == OSPlatform.Linux)
                return true;
            else if (CurrentPlatform == OSPlatform.OSX)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets common installation paths for Ghostscript on the current platform.
        /// </summary>
        public static string[] GetCommonInstallationPaths()
        {
            if (CurrentPlatform == OSPlatform.Windows)
                return new string[]
                {
                    @"C:\Program Files\gs",
                    @"C:\Program Files (x86)\gs",
                    @"C:\gs"
                };
            else if (CurrentPlatform == OSPlatform.Linux)
                return new string[]
                {
                    "/usr/lib",
                    "/usr/lib/x86_64-linux-gnu",
                    "/usr/lib64",
                    "/usr/local/lib",
                    "/usr/local/lib64",
                    "/opt/ghostscript/lib",
                    "/opt/ghostscript/lib64"
                };
            else if (CurrentPlatform == OSPlatform.OSX)
                return new string[]
                {
                    "/usr/local/lib",
                    "/opt/homebrew/lib",
                    "/opt/ghostscript/lib"
                };
            else
                return new string[0];
        }

        /// <summary>
        /// Searches for Ghostscript library in common installation paths.
        /// </summary>
        /// <param name="is64Bit">Whether to look for 64-bit library.</param>
        /// <returns>Path to found library or null if not found.</returns>
        public static string FindGhostscriptLibrary(bool is64Bit)
        {
            string libraryName = GetGhostscriptLibraryName(is64Bit);
            
            foreach (string basePath in GetCommonInstallationPaths())
            {
                if (Directory.Exists(basePath))
                {
                    // Search in the base directory
                    string libraryPath = Path.Combine(basePath, libraryName);
                    if (File.Exists(libraryPath) && IsLibraryCompatible(libraryPath))
                    {
                        return libraryPath;
                    }

                    // Search in subdirectories
                    try
                    {
                        string[] subdirs = Directory.GetDirectories(basePath);
                        foreach (string subdir in subdirs)
                        {
                            libraryPath = Path.Combine(subdir, "bin", libraryName);
                            if (File.Exists(libraryPath) && IsLibraryCompatible(libraryPath))
                            {
                                return libraryPath;
                            }

                            libraryPath = Path.Combine(subdir, "lib", libraryName);
                            if (File.Exists(libraryPath) && IsLibraryCompatible(libraryPath))
                            {
                                return libraryPath;
                            }
                        }
                    }
                    catch
                    {
                        // Ignore directory access errors
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the appropriate error message for library compatibility issues.
        /// </summary>
        public static string GetCompatibilityErrorMessage()
        {
            if (CurrentPlatform == OSPlatform.Windows)
                return Environment.Is64BitProcess ?
                        "You are using native Ghostscript library (gsdll32.dll) compiled for 32bit systems in a 64bit process. You need to use gsdll64.dll. " +
                        "64bit native Ghostscript library can be downloaded from http://www.ghostscript.com/download/gsdnld.html" :
                        "You are using native Ghostscript library (gsdll64.dll) compiled for 64bit systems in a 32bit process. You need to use gsdll32.dll. " +
                        "32bit native Ghostscript library can be downloaded from http://www.ghostscript.com/download/gsdnld.html";
            else if (CurrentPlatform == OSPlatform.Linux)
                return "Ghostscript library (libgs.so) is not compatible with the current process architecture. " +
                           "Please ensure you have the correct version of libgs.so installed.";
            else if (CurrentPlatform == OSPlatform.OSX)
                return "Ghostscript library (libgs.so) is not compatible with the current process architecture. " +
                           "Please ensure you have the correct version of libgs.so installed.";
            else
                return "Ghostscript library is not compatible with the current process architecture.";
        }
    }
}
