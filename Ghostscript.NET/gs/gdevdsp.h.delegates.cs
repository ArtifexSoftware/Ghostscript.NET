//
// gdevdsp.h.delegates.cs
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
using System.Runtime.InteropServices;

namespace Ghostscript.NET
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_open_callback(IntPtr handle, IntPtr device);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_preclose_callback(IntPtr handle, IntPtr device);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_close_callback(IntPtr handle, IntPtr device);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_presize_callback(IntPtr handle, IntPtr device, Int32 width, Int32 height, Int32 raster, UInt32 format);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_size_callback(IntPtr handle, IntPtr device, Int32 width, Int32 height, Int32 raster, UInt32 format, IntPtr pimage);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_sync_callback(IntPtr handle, IntPtr device);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_page_callback(IntPtr handle, IntPtr device, Int32 copies, Int32 flush);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_update_callback(IntPtr handle, IntPtr device, Int32 x, Int32 y, Int32 w, Int32 h);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void display_memalloc_callback(IntPtr handle, IntPtr device, UInt32 size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_memfree_callback(IntPtr handle, IntPtr device, IntPtr mem);

    // added in v2
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int display_separation_callback(IntPtr handle, IntPtr device, Int32 component, String component_name, UInt16 c, UInt16 m, UInt16 y, UInt16 k);

    // added in v3
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_adjust_band_height(IntPtr handle, IntPtr device, Int32 bandheight);

    // added in v3
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int display_rectangle_request(IntPtr handle, IntPtr device, IntPtr memory, Int32 ox, Int32 oy, Int32 raster, Int32 plane_raster, Int32 x, Int32 y, Int32 w, Int32 h);
}
