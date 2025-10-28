//
// LinuxBase.h.cs
// This file is part of Microsoft.WinAny.Helper library
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
