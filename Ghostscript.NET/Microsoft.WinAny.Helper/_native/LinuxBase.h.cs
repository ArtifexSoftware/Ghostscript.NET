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
using System.Runtime.InteropServices;

namespace Microsoft.WinAny
{
    internal static class LinuxBase
    {
        [DllImport("libdl.so.2")]
        public static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl.so.2")]
        public static extern bool dlclose(IntPtr handle);

        [DllImport("libdl.so.2")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport("libdl.so.2")]
        public static extern IntPtr dlerror();

        // Flags for dlopen
        public const int RTLD_LAZY = 0x00001;
        public const int RTLD_NOW = 0x00002;
        public const int RTLD_GLOBAL = 0x00100;
        public const int RTLD_LOCAL = 0x00000;
        public const int RTLD_NODELETE = 0x01000;
        public const int RTLD_NOLOAD = 0x00004;
        public const int RTLD_DEEPBIND = 0x00008;

        // Default flags for loading libraries
        public const int DEFAULT_FLAGS = RTLD_LAZY | RTLD_GLOBAL;
    }
}
