//
// memory.cs
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
    internal static unsafe class memory
    {
        /// <summary>
        /// Copies bytes between buffers.
        /// </summary>
        /// <param name="dest">New buffer.</param>
        /// <param name="src">Buffer to copy from.</param>
        /// <param name="size">Number of characters to copy.</param>
        public static void memcpy(byte* dest, byte* src, uint count)
        {
            for (uint i = 0; i < count; i++)
            {
                *(dest + i) = *(src + i);
            }
        }

        /// <summary>
        /// Sets buffers to a specified character.
        /// </summary>
        /// <param name="dest">Pointer to destination.</param>
        /// <param name="c">Character to set.</param>
        /// <param name="count">Number of characters.</param>
        public static void memset(byte* dest, byte c, uint count)
        {
            for (uint i = 0; i < count; i++)
            {
                *dest = c;
            }
        }

        /// <summary>
        /// Reallocate memory blocks.
        /// </summary>
        /// <param name="memblock">Pointer to previously allocated memory block.</param>
        /// <param name="size">Previously allocated memory block size.</param>
        /// <param name="newsize">New size in bytes.</param>
        /// <returns></returns>
        public static byte* realloc(byte* memblock, uint size, uint newsize)
        {
            byte* newMemBlock = (byte*)Marshal.AllocHGlobal((int)newsize).ToPointer();

            memcpy(newMemBlock, memblock, size);

            Marshal.FreeHGlobal(new IntPtr(memblock));

            return newMemBlock;
        }
    }
}
