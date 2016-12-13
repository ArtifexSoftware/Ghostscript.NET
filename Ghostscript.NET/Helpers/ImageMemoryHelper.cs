//
// ImageMemoryHelper.cs
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
    internal class ImageMemoryHelper
    {
        #region Set24bppRgbImageColor

        public unsafe static void Set24bppRgbImageColor(IntPtr image, int width, int height, byte r, byte g, byte b)
        {
            byte* ptr = (byte*)image;
            int stride = (((width * 3) + 3) & ~3);

            int padding = stride - (width * 3);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    *ptr++ = r;
                    *ptr++ = g;
                    *ptr++ = b;
                }

                ptr+=padding;
            }
        }

        #endregion

        #region CopyImagePartFrom

        public static void CopyImagePartFrom(IntPtr src, IntPtr dest, int x, int y, int width, int height, int stride, int bytesPerPixel)
        {
            int destStride = (((width * bytesPerPixel) + 3) & ~3); 

            int srcTop = y;
            int destTop = 0;
            int srcBottom = y + height - 1;
            int posSrcTop = 0;
            int posDestTop = 0;

            while (srcTop <= srcBottom)
            {
                posSrcTop = (srcTop * (stride)) + (x * bytesPerPixel);
                posDestTop = (destTop * (destStride));

                wdm.MoveMemory(new IntPtr((long)dest + posDestTop), new IntPtr((long)src + posSrcTop), (uint)(width * bytesPerPixel));

                srcTop++;
                destTop++;
            }
        }

        #endregion

        #region CopyImagePartTo

        public static void CopyImagePartTo(IntPtr dest, IntPtr src, int x, int y, int width, int height, int stride, int bytesPerPixel)
        {
            int partStride = (((width * bytesPerPixel) + 3) & ~3); 

            int destTop = y;
            int srcTop = 0;
            int destBottom = y + height - 1;
            int posDestTop = 0;
            int posSrcTop = 0;

            while (destTop <= destBottom)
            {
                posDestTop = (destTop * stride) + (x * bytesPerPixel);
                posSrcTop = (srcTop * partStride);

                wdm.MoveMemory(new IntPtr((long)dest + posDestTop), new IntPtr((long)src + posSrcTop), (uint)(width * bytesPerPixel));

                destTop++;
                srcTop++;
            }
        }

        #endregion

        #region FlipImageVertically

        public static void FlipImageVertically(IntPtr src, IntPtr dest, int height, int stride)
        {
            int size = height * stride;

            var buffer = new byte[size];
            Marshal.Copy(src, buffer, 0, size);

            byte[] row = new byte[stride];

            int top = 0;
            int bottom = height - 1;
            int posTop;
            int posBottom;

            while (top <= bottom)
            {
                posTop = top * stride;
                posBottom = bottom * stride;

                Array.Copy(buffer, posTop, row, 0, stride);
                Array.Copy(buffer, posBottom, buffer, posTop, stride);
                Array.Copy(row, 0, buffer, posBottom, stride);

                top++;
                bottom--;
            }

            Marshal.Copy(buffer, 0, dest, size);
        }

        #endregion
    }
}
