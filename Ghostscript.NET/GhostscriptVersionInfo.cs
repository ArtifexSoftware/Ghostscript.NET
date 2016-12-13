//
// GhostscriptVersionInfo.cs
// This file is part of Ghostscript.NET library
//
// Author: Josip Habjan (habjan@gmail.com, http://www.linkedin.com/in/habjan) 
// Copyright (c) 2013-2016 by Josip Habjan. All rights reserved.
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
using Microsoft.Win32;

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
        {
            _version = version;
            _dllPath = dllPath;
            _libPath = libPath;
            _licenseType = licenseType;
        }

        #endregion

        public GhostscriptVersionInfo(string customDllPath)
        {
            _version = new Version();
            _dllPath = customDllPath;
            _libPath = string.Empty;
            _licenseType = GhostscriptLicense.GPL;
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

        #region ToString

        /// <summary>
        /// Returns GhostscriptVersionInfo string.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Licence: {0}, Version: {1}, Dll: {2}, Lib: {3}", _licenseType, _version, _dllPath, _libPath);
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

        #region GetLastInstalledVersion

        /// <summary>
        /// Gets top installed Ghostscript version.
        /// </summary>
        public static GhostscriptVersionInfo GetLastInstalledVersion()
        {
            return GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL | GhostscriptLicense.Artifex, GhostscriptLicense.GPL);
        }

        #endregion

        #region GetLastInstalledVersion

        /// <summary>
        /// Gets top installed Ghostscript version.
        /// </summary>
        /// <param name="licenseType">Serch for the specific Ghostscript version based on the Ghostscript license.</param>
        /// <param name="licensePriority">If there are both license types installed, which one should have the prilorty.</param>
        /// <returns>GhostscriptVersionInfo object of the last installed Ghostscript version based on priority license.</returns>
        public static GhostscriptVersionInfo GetLastInstalledVersion(GhostscriptLicense licenseType, GhostscriptLicense licensePriority)
        {
            // gets installed Ghostscript versions list
            List<GhostscriptVersionInfo> gsVerList = GetInstalledVersions(licenseType);

            // cache the list count
            int versionsCount = gsVerList.Count;

            // check if there is only 1 version of the Ghostscript installed
            // if yes, then we don't need a deeper search
            if (versionsCount == 1)
            {
                // simply return the first one
                return gsVerList[0];
            }
            else if (versionsCount > 1)
            {
                // get the first one
                GhostscriptVersionInfo lastGsVer = gsVerList[0];

                // loop through all others
                for (int index = 1; index < versionsCount; index++)
                {
                    // get one from the list
                    GhostscriptVersionInfo gs = gsVerList[index];

                    // compare if it's a newer version
                    if (gs.Version > lastGsVer.Version)
                    {
                        // check if this version has license with larger priority
                        if (gs.LicenseType == licensePriority)
                        {
                            // set top version
                            lastGsVer = gsVerList[index];
                        }
                    }
                }

                // return top GhostscriptVersionInfo instance
                return lastGsVer;
            }

            // inform the user that we didn't find Ghostscript installed on this system
            throw new GhostscriptLibraryNotInstalledException();
        }

        #endregion

        #region IsGhostscriptInstalled

        /// <summary>
        /// Gets if the Ghostscript is installed on the local system.
        /// </summary>
        public static bool IsGhostscriptInstalled
        {
            get
            {
                // check for any, GPL or AFPL version
                return GetInstalledVersions(GhostscriptLicense.GPL | GhostscriptLicense.AFPL | GhostscriptLicense.Artifex).Count > 0;
            }
        }

        #endregion

    }
}
