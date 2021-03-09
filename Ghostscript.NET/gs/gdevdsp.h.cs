//
// gdevdsp.h.cs
// This file is part of Ghostscript.NET library
//
// Author: Josip Habjan (habjan@gmail.com, http://www.linkedin.com/in/habjan) 
// Copyright (c) 2013-2016 by Josip Habjan. All rights reserved.
//
// Author ported parts of this code from AFPL Ghostscript. 
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

    #region gdevdsp

    public class gdevdsp
    {
        public const int DISPLAY_VERSION_MAJOR_V3 = 3;
        public const int DISPLAY_VERSION_MINOR_V3 = 0;

        public const int DISPLAY_VERSION_MAJOR_V2 = 2;
        public const int DISPLAY_VERSION_MINOR_V2 = 0;

        public const long DISPLAY_COLORS_MASK = 0x8000fL;
        public const long DISPLAY_ALPHA_MASK = 0x00f0L;
        public const long DISPLAY_DEPTH_MASK = 0xff00L;
        public const long DISPLAY_ENDIAN_MASK = 0x00010000L;
        public const long DISPLAY_FIRSTROW_MASK = 0x00020000L;
        public const long DISPLAY_555_MASK = 0x00040000L;
        public const long DISPLAY_ROW_ALIGN_MASK = 0x00700000L;
    }

    #endregion

    /* The display format is set by a combination of the following bitfields */

    #region DISPLAY_FORMAT_COLOR

    /// <summary>
    /// Define the color space alternatives.
    /// </summary>
    public enum DISPLAY_FORMAT_COLOR : long
    {
        DISPLAY_COLORS_NATIVE = (1 << 0),
        DISPLAY_COLORS_GRAY = (1 << 1),
        DISPLAY_COLORS_RGB = (1 << 2),
        DISPLAY_COLORS_CMYK = (1 << 3),
        DISPLAY_COLORS_SEPARATION = (1 << 19),
    }

    #endregion

    #region DISPLAY_FORMAT_ALPHA

    /// <summary>
    /// Define whether alpha information, or an extra unused bytes is included
    /// DISPLAY_ALPHA_FIRST and DISPLAY_ALPHA_LAST are not implemented
    /// </summary>
    public enum DISPLAY_FORMAT_ALPHA : long
    {
        DISPLAY_ALPHA_NONE   = (0<<4),
        DISPLAY_ALPHA_FIRST  = (1<<4),
        DISPLAY_ALPHA_LAST   = (1<<5),
        DISPLAY_UNUSED_FIRST = (1<<6),	/* e.g. Mac xRGB */
        DISPLAY_UNUSED_LAST  = (1<<7)	/* e.g. Windows BGRx */
    }

    #endregion

    #region DISPLAY_FORMAT_DEPTH

    /// <summary>
    /// Define the depth per component for DISPLAY_COLORS_GRAY,
    /// DISPLAY_COLORS_RGB and DISPLAY_COLORS_CMYK,
    /// or the depth per pixel for DISPLAY_COLORS_NATIVE
    /// DISPLAY_DEPTH_2 and DISPLAY_DEPTH_12 have not been tested.
    /// </summary>
    public enum DISPLAY_FORMAT_DEPTH : long
    {
        DISPLAY_DEPTH_1 = (1 << 8),
        DISPLAY_DEPTH_2 = (1 << 9),
        DISPLAY_DEPTH_4 = (1 << 10),
        DISPLAY_DEPTH_8 = (1 << 11),
        DISPLAY_DEPTH_12 = (1 << 12),
        DISPLAY_DEPTH_16 = (1 << 13)
        /* unused (1<<14) */
        /* unused (1<<15) */
    }

    #endregion

    #region DISPLAY_FORMAT_ENDIAN

    /// <summary>
    /// Define whether Red/Cyan should come first,
    /// or whether Blue/Black should come first
    /// </summary>
    public enum DISPLAY_FORMAT_ENDIAN
    {
        DISPLAY_BIGENDIAN = (0 << 16),	/* Red/Cyan first */
        DISPLAY_LITTLEENDIAN = (1 << 16)	/* Blue/Black first */
    }

    #endregion

    #region DISPLAY_FORMAT_FIRSTROW

    /// <summary>
    /// Define whether the raster starts at the top or bottom of the bitmap
    /// </summary>
    public enum DISPLAY_FORMAT_FIRSTROW
    {
        DISPLAY_TOPFIRST = (0 << 17),	/* Unix, Mac */
        DISPLAY_BOTTOMFIRST = (1 << 17)	/* Windows */
    }

    #endregion

    #region DISPLAY_FORMAT_555

    /// <summary>
    /// Define whether packing RGB in 16-bits should use 555
    /// or 565 (extra bit for green)
    /// </summary>
    public enum DISPLAY_FORMAT_555
    {
        DISPLAY_NATIVE_555 = (0 << 18),
        DISPLAY_NATIVE_565 = (1 << 18)
    }

    #endregion

    #region DISPLAY_FORMAT_ROW_ALIGN

    /// <summary>
    /// Define the row alignment, which must be equal to or greater than
    /// the size of a pointer.
    /// The default (DISPLAY_ROW_ALIGN_DEFAULT) is the size of a pointer,
    /// 4 bytes (DISPLAY_ROW_ALIGN_4) on 32-bit systems or 8 bytes
    /// (DISPLAY_ROW_ALIGN_8) on 64-bit systems.
    /// </summary>
    public enum DISPLAY_FORMAT_ROW_ALIGN
    {
        DISPLAY_ROW_ALIGN_DEFAULT = (0 << 20),
        /* DISPLAY_ROW_ALIGN_1 = (1<<20), */
        /* not currently possible */
        /* DISPLAY_ROW_ALIGN_2 = (2<<20), */
        /* not currently possible */
        DISPLAY_ROW_ALIGN_4 = (3 << 20),
        DISPLAY_ROW_ALIGN_8 = (4 << 20),
        DISPLAY_ROW_ALIGN_16 = (5 << 20),
        DISPLAY_ROW_ALIGN_32 = (6 << 20),
        DISPLAY_ROW_ALIGN_64 = (7 << 20)
    }

    #endregion

    #region display_callback_v3

    /// <summary>
    /// Display device callback structure.
    /// 
    /// Note that for Windows, the display callback functions are
    /// cdecl, not stdcall.  This differs from those in iapi.h.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class display_callback_v3 : display_callback
    {
        /// <summary>
        /// Added in V3 */
        /// If non NULL, then this gives the callback provider a chance to
        /// a) be informed of and b) control the bandheight used by the
        /// display device. If a call to allocate the page mode bitmap fails
        /// (either an internal allocation or a display_memalloc call), then
        /// Ghostscript will look for the presence of a
        /// display_rectangle_request callback. If it exists, then it will
        /// attempt to use retangle request mode.
        /// 
        /// As part of this, it will pick an appropriate bandheight. If
        /// this callback exists, it will be called so the callback provider
        /// can know (and, optionally, tweak) the bandheight to be used.
        /// This is purely for performance. The callback should only ever
        /// *reduce* the bandheight given here.
        /// 
        /// Return the adjusted bandheight (or 0 for no change).
        /// </summary>
        public display_adjust_band_height display_adjust_band_height;

        /// <summary>
        /// Ask the callback for a rectangle to render (and a block to render
        /// it in). Each subsequent call tells the caller that any previous
        /// call has finished. To signal 'no more rectangles' return with
        /// *w or *h = 0.
        /// 
        /// On entry: *raster and *plane_raster are set to the standard
        ///   values. All other values are undefined.
        /// On return: *memory should point to a block of memory to use.
        ///   Pixel (*ox,*oy) is the first pixel represented in that block.
        ///   *raster = the number of bytes difference between the address of
        ///   component 0 of Pixel(*ox,*oy) and the address of component 0 of
        ///   Pixel(*ox,1+*oy).
        ///   *plane_raster = the number of bytes difference between the
        ///   address of component 0 of Pixel(*ox,*oy) and the address of
        ///   component 1 of Pixel(*ox,*oy), if in planar mode, 0 otherwise.
        ///   *x, *y, *w, *h = rectangle requested within that memory block.
        /// </summary>
        public display_rectangle_request display_rectangle_request;

    }

    #endregion


    #region display_callback

    /// <summary>
    /// Display device callback structure.
    /// 
    /// Note that for Windows, the display callback functions are
    /// cdecl, not stdcall.  This differs from those in iapi.h.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class display_callback
    {
        /// <summary>
        /// Size of this structure
        /// Used for checking if we have been handed a valid structure
        /// </summary>
        public int size;

        /// <summary>
        /// Major version of this structure
        /// The major version number will change if this structure changes.
        /// </summary>
        public int version_major;

        /// <summary>
        /// Minor version of this structure 
        /// The minor version number will change if new features are added
        /// without changes to this structure.  For example, a new color
        /// format.
        /// </summary>
        public int version_minor;

        /// <summary>
        /// New device has been opened 
        /// This is the first event from this device.
        /// </summary>
        public display_open_callback display_open;

        /// <summary>
        /// Device is about to be closed. 
        /// Device will not be closed until this function returns. 
        /// </summary>
        public display_preclose_callback display_preclose;

        /// <summary>
        /// Device has been closed. 
        /// This is the last event from this device. 
        /// </summary>
        public display_close_callback display_close;


        /// <summary>
        /// Device is about to be resized. 
        /// Resize will only occur if this function returns 0. 
        /// raster is byte count of a row.
        /// </summary>
        public display_presize_callback display_presize;

        /// <summary>
        /// Device has been resized. 
        /// New pointer to raster returned in pimage
        /// </summary>
        public display_size_callback display_size;

        /// <summary>
        /// flushpage
        /// </summary>
        public display_sync_callback display_sync;

        /// <summary>
        /// showpage 
        /// If you want to pause on showpage, then don't return immediately
        /// </summary>
        public display_page_callback display_page;

        /// <summary>
        /// Notify the caller whenever a portion of the raster is updated.
        /// This can be used for cooperative multitasking or for
        /// progressive update of the display.
        /// This function pointer may be set to NULL if not required.
        /// </summary>
        public display_update_callback display_update;

        /// <summary>
        /// Allocate memory for bitmap 
        /// This is provided in case you need to create memory in a special
        /// way, e.g. shared.  If this is NULL, the Ghostscript memory device
        /// allocates the bitmap. This will only called to allocate the
        /// image buffer. The first row will be placed at the address
        /// returned by display_memalloc.
        /// </summary>
        public display_memalloc_callback display_memalloc;

        /// <summary>
        /// Free memory for bitmap 
        /// If this is NULL, the Ghostscript memory device will free the bitmap
        /// </summary>
        public display_memfree_callback display_memfree;

        /// <summary>
        /// Added in V2 
        /// When using separation color space (DISPLAY_COLORS_SEPARATION),
        /// give a mapping for one separation component.
        /// This is called for each new component found.
        /// It may be called multiple times for each component.
        /// It may be called at any time between display_size
        /// and display_close.
        /// The client uses this to map from the separations to CMYK
        /// and hence to RGB for display.
        /// GS must only use this callback if version_major >= 2.
        /// The unsigned short c,m,y,k values are 65535 = 1.0.
        /// This function pointer may be set to NULL if not required.
        /// </summary>
        public display_separation_callback display_separation;
    }

    #endregion
}
