// Copyright (C) 2024 Artifex Software, Inc.
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
// along with Ghostscript.NET. If not, see <https://www.gnu.org/licenses/agpl-3.0.en.html>
//
// Alternative licensing terms are available from the licensor.
// For commercial licensing, see <https://www.artifex.com/> or contact
// Artifex Software, Inc., 39 Mesa Street, Suite 108A, San Francisco,
// CA 94129, USA, for further information.

using System;
using System.IO;

namespace Ghostscript.NET.PDFA3Converter.Samples
{
    internal static class SamplePaths
    {
        public static string ResolveGhostscriptDll()
        {
            GhostscriptVersionInfo version = GhostscriptVersionInfo.GetPreferredVersion();
            return version.DllPath;
        }

        public static string GetBlankPdf()
        {
            string[] candidates =
            {
                Path.Combine(AppContext.BaseDirectory, "Samples", "blank.pdf"),
                Path.GetFullPath(Path.Combine("Samples", "blank.pdf"))
            };

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException("Could not find Samples/blank.pdf.");
        }

        public static string GetOutputPath(string fileName)
        {
            return Path.Combine(AppContext.BaseDirectory, fileName);
        }
    }
}
