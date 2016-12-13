//
// NativeLibraryHelper.cs
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
using System.IO;
using Microsoft.WinAny;

namespace Ghostscript.NET
{
    /// <summary>
    /// Class that helps us to get various information about native libraries.
    /// </summary>
    internal class NativeLibraryHelper
    {
        /// <summary>
        /// Gets the image file machine type.
        /// </summary>
        /// <param name="path">Native library path.</param>
        /// <returns>Image file machine type.</returns>
        public static ushort GetImageFileMachineType(string path)
        {
            using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return GetImageFileMachineType(fs);
            }
        }

        /// <summary>
        /// Gets the image file machine type.
        /// </summary>
        /// <param name="buffer">Memory buffer representing native library.</param>
        /// <returns>Image file machine type.</returns>
        public static unsafe ushort GetImageFileMachineType(byte[] buffer)
        {
            fixed(byte *ptr = buffer)
            {
                using(UnmanagedMemoryStream ums = new UnmanagedMemoryStream(ptr, buffer.Length))
                {
                    return GetImageFileMachineType(ums);
                }
            }
        }

        /// <summary>
        /// Gets the image file machine type.
        /// </summary>
        /// <param name="srm">Stream representing native library.</param>
        /// <returns>Image file machine type.</returns>
        public static ushort GetImageFileMachineType(Stream srm)
        {
            using (BinaryReader reader = new BinaryReader(srm))
            {
                // seek to the IMAGE_DOS_HEADER->e_lfanew position
                reader.BaseStream.Seek(0x3c, SeekOrigin.Begin);
                // read out the value
                uint e_lfanew = reader.ReadUInt32();
                // seek the the file address of new exe header
                reader.BaseStream.Seek(e_lfanew, SeekOrigin.Begin);
                // read out the signature
                uint signature = reader.ReadUInt32();
                // check if it's a signature we can handle
                if (signature != WinNT.IMAGE_NT_SIGNATURE)
                {
                    return WinNT.IMAGE_FILE_MACHINE_UNKNOWN;
                }

                // read out and return IMAGE_FILE_HEADER->Machine value
                return reader.ReadUInt16();
            }
        }

        /// <summary>
        /// Gets if native library is compiled as 64bit library.
        /// </summary>
        /// <param name="path">Native library path.</param>
        /// <returns>True if native library is compiled as 64 bit library.</returns>
        public static bool Is64BitLibrary(string path)
        {
            ushort machine = GetImageFileMachineType(path);
            return Is64BitMachineValue(machine);
        }

        /// <summary>
        /// Gets if native library is compiled as 64bit library.
        /// </summary>
        /// <param name="buffer">Memory buffer representing native library.</param>
        /// <returns>True if native library is compiled as 64 bit library.</returns>
        public static bool Is64BitLibrary(byte[] buffer)
        {
            ushort machine = GetImageFileMachineType(buffer);
            return Is64BitMachineValue(machine);
        }

        /// <summary>
        /// Gets if machine value represents 64 bit machine.
        /// </summary>
        /// <param name="machine">IMAGE_FILE_HEADER->Machine value.</param>
        private static bool Is64BitMachineValue(ushort machine)
        {
            switch(machine)
            {
                case WinNT.IMAGE_FILE_MACHINE_AMD64:
                case WinNT.IMAGE_FILE_MACHINE_IA64:
                    return true;
                case WinNT.IMAGE_FILE_MACHINE_I386:
                    return false;
                default:
                    return false;
            }
        }

    }
}
