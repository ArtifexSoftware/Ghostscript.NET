//
// WinNT.h.cs
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
    internal static unsafe class WinNT
    {

        #region Helpers

        #region IMAGE_FIRST_SECTION

        public static IMAGE_SECTION_HEADER* IMAGE_FIRST_SECTION(byte* ptr_image_nt_headers)
        {
            if (Environment.Is64BitProcess)
            {
                IMAGE_NT_HEADERS64* image_nt_headers = (IMAGE_NT_HEADERS64*)ptr_image_nt_headers;
                return (IMAGE_SECTION_HEADER*)((long)image_nt_headers +
                                               (long)Marshal.OffsetOf(typeof(IMAGE_NT_HEADERS64), "OptionalHeader") +
                                               image_nt_headers->FileHeader.SizeOfOptionalHeader);
            }
            else
            {
                IMAGE_NT_HEADERS32* image_nt_headers = (IMAGE_NT_HEADERS32*)ptr_image_nt_headers;
                return (IMAGE_SECTION_HEADER*)((long)image_nt_headers +
                                               (long)Marshal.OffsetOf(typeof(IMAGE_NT_HEADERS32), "OptionalHeader") +
                                               image_nt_headers->FileHeader.SizeOfOptionalHeader);
            }
        }

        #endregion

        #region IMAGE_SNAP_BY_ORDINAL

        public static bool IMAGE_SNAP_BY_ORDINAL(IntPtr* ordinal)
        {
            if (Environment.Is64BitProcess)
            {
                return ((((ulong)*ordinal) & IMAGE_ORDINAL_FLAG64) != 0);
            }
            else
            {
                return ((((uint)*ordinal) & IMAGE_ORDINAL_FLAG32) != 0);
            }
        }

        #endregion

        #region IMAGE_ORDINAL

        public static ulong IMAGE_ORDINAL(IntPtr* ordinal)
        {
            return ((ulong)*ordinal & 0xffff);
        }

        #endregion

        #endregion

        #region Constants

        public const uint IMAGE_DOS_SIGNATURE                   = 0x5A4D;      // MZ
        public const uint IMAGE_OS2_SIGNATURE                   = 0x454E;      // NE
        public const uint IMAGE_OS2_SIGNATURE_LE                = 0x454C;      // LE
        public const uint IMAGE_VXD_SIGNATURE                   = 0x454C;      // LE
        public const uint IMAGE_NT_SIGNATURE                    = 0x00004550;  // PE00

        public const int IMAGE_SIZEOF_SHORT_NAME               = 8;

        public const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES      = 16;

        public const ulong IMAGE_ORDINAL_FLAG64                = 0x8000000000000000;
        public const uint  IMAGE_ORDINAL_FLAG32                = 0x80000000;

        public const uint IMAGE_SCN_TYPE_NO_PAD                = 0x00000008;  // Reserved.

        public const uint IMAGE_SCN_CNT_CODE                   = 0x00000020;  // Section contains code.
        public const uint IMAGE_SCN_CNT_INITIALIZED_DATA       = 0x00000040;  // Section contains initialized data.
        public const uint IMAGE_SCN_CNT_UNINITIALIZED_DATA     = 0x00000080;  // Section contains uninitialized data.

        public const uint IMAGE_SCN_LNK_OTHER                  = 0x00000100;  // Reserved.
        public const uint IMAGE_SCN_LNK_INFO                   = 0x00000200;  // Section contains comments or some other type of information.

        public const uint IMAGE_SCN_LNK_REMOVE                 = 0x00000800;  // Section contents will not become part of image.
        public const uint IMAGE_SCN_LNK_COMDAT                 = 0x00001000;  // Section contents comdat.

        public const uint IMAGE_SCN_NO_DEFER_SPEC_EXC          = 0x00004000;  // Reset speculative exceptions handling bits in the TLB entries for this section.
        public const uint IMAGE_SCN_GPREL                      = 0x00008000;  // Section content can be accessed relative to GP
        public const uint IMAGE_SCN_MEM_FARDATA                = 0x00008000;

        public const uint IMAGE_SCN_MEM_PURGEABLE              = 0x00020000;
        public const uint IMAGE_SCN_MEM_16BIT                  = 0x00020000;
        public const uint IMAGE_SCN_MEM_LOCKED                 = 0x00040000;
        public const uint IMAGE_SCN_MEM_PRELOAD                = 0x00080000;

        public const uint IMAGE_SCN_ALIGN_1BYTES               = 0x00100000;  //
        public const uint IMAGE_SCN_ALIGN_2BYTES               = 0x00200000;  //
        public const uint IMAGE_SCN_ALIGN_4BYTES               = 0x00300000;  //
        public const uint IMAGE_SCN_ALIGN_8BYTES               = 0x00400000;  //
        public const uint IMAGE_SCN_ALIGN_16BYTES              = 0x00500000;  // Default alignment if no others are specified.
        public const uint IMAGE_SCN_ALIGN_32BYTES              = 0x00600000;  //
        public const uint IMAGE_SCN_ALIGN_64BYTES              = 0x00700000;  //
        public const uint IMAGE_SCN_ALIGN_128BYTES             = 0x00800000;  //
        public const uint IMAGE_SCN_ALIGN_256BYTES             = 0x00900000;  //
        public const uint IMAGE_SCN_ALIGN_512BYTES             = 0x00A00000;  //
        public const uint IMAGE_SCN_ALIGN_1024BYTES            = 0x00B00000;  //
        public const uint IMAGE_SCN_ALIGN_2048BYTES            = 0x00C00000;  //
        public const uint IMAGE_SCN_ALIGN_4096BYTES            = 0x00D00000;  //
        public const uint IMAGE_SCN_ALIGN_8192BYTES            = 0x00E00000;  //
        // Unused                                    0x00F00000;
        public const uint IMAGE_SCN_ALIGN_MASK                 = 0x00F00000;

        public const uint IMAGE_SCN_LNK_NRELOC_OVFL            = 0x01000000;  // Section contains extended relocations.
        public const uint IMAGE_SCN_MEM_DISCARDABLE            = 0x02000000;  // Section can be discarded.
        public const uint IMAGE_SCN_MEM_NOT_CACHED             = 0x04000000;  // Section is not cachable.
        public const uint IMAGE_SCN_MEM_NOT_PAGED              = 0x08000000;  // Section is not pageable.
        public const uint IMAGE_SCN_MEM_SHARED                 = 0x10000000;  // Section is shareable.
        public const uint IMAGE_SCN_MEM_EXECUTE                = 0x20000000;  // Section is executable.
        public const uint IMAGE_SCN_MEM_READ                   = 0x40000000;  // Section is readable.
        public const uint IMAGE_SCN_MEM_WRITE                  = 0x80000000;  // Section is writeable.

        public const uint PAGE_NOACCESS                     = 0x01;     
        public const uint PAGE_READONLY                     = 0x02;     
        public const uint PAGE_READWRITE                    = 0x04;    
        public const uint PAGE_WRITECOPY                    = 0x08;     
        public const uint PAGE_EXECUTE                      = 0x10;    
        public const uint PAGE_EXECUTE_READ                 = 0x20;     
        public const uint PAGE_EXECUTE_READWRITE            = 0x40;     
        public const uint PAGE_EXECUTE_WRITECOPY            = 0x80;     
        public const uint PAGE_GUARD                        = 0x100;     
        public const uint PAGE_NOCACHE                      = 0x200;     
        public const uint PAGE_WRITECOMBINE                 = 0x400;

        public const uint MEM_COMMIT                        = 0x1000;     
        public const uint MEM_RESERVE                       = 0x2000;     
        public const uint MEM_DECOMMIT                      = 0x4000;     
        public const uint MEM_RELEASE                       = 0x8000;     
        public const uint MEM_FREE                          = 0x10000;     
        public const uint MEM_PRIVATE                       = 0x20000;     
        public const uint MEM_MAPPED                        = 0x40000;     
        public const uint MEM_RESET                         = 0x80000;     
        public const uint MEM_TOP_DOWN                      = 0x100000;     
        public const uint MEM_WRITE_WATCH                   = 0x200000;     
        public const uint MEM_PHYSICAL                      = 0x400000;     
        public const uint MEM_ROTATE                        = 0x800000;     
        public const uint MEM_LARGE_PAGES                   = 0x20000000;
        public const uint MEM_4MB_PAGES                     = 0x80000000;
        public const uint MEM_IMAGE                         = SEC_IMAGE;

        public const uint SEC_FILE                          = 0x800000;     
        public const uint SEC_IMAGE                         = 0x1000000;     
        public const uint SEC_PROTECTED_IMAGE               = 0x2000000;  
        public const uint SEC_RESERVE                       = 0x4000000;     
        public const uint SEC_COMMIT                        = 0x8000000;     
        public const uint SEC_NOCACHE                       = 0x10000000;     
        public const uint SEC_WRITECOMBINE                  = 0x40000000;     
        public const uint SEC_LARGE_PAGES                   = 0x80000000;

        public const int WRITE_WATCH_FLAG_RESET            = 0x01;

        // Directory Entries

        public const int IMAGE_DIRECTORY_ENTRY_EXPORT              = 0;   // Export Directory
        public const int IMAGE_DIRECTORY_ENTRY_IMPORT              = 1;   // Import Directory
        public const int IMAGE_DIRECTORY_ENTRY_RESOURCE            = 2;   // Resource Directory
        public const int IMAGE_DIRECTORY_ENTRY_EXCEPTION           = 3;   // Exception Directory
        public const int IMAGE_DIRECTORY_ENTRY_SECURITY            = 4;   // Security Directory
        public const int IMAGE_DIRECTORY_ENTRY_BASERELOC           = 5;   // Base Relocation Table
        public const int IMAGE_DIRECTORY_ENTRY_DEBUG               = 6;   // Debug Directory
        public const int IMAGE_DIRECTORY_ENTRY_ARCHITECTURE        = 7;   // Architecture Specific Data
        public const int IMAGE_DIRECTORY_ENTRY_GLOBALPTR           = 8;   // RVA of GP
        public const int IMAGE_DIRECTORY_ENTRY_TLS                 = 9;   // TLS Directory
        public const int IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG         = 10;   // Load Configuration Directory
        public const int IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT        = 11;   // Bound Import Directory in headers
        public const int IMAGE_DIRECTORY_ENTRY_IAT                 = 12;   // Import Address Table
        public const int IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT        = 13;   // Delay Load Import Descriptors
        public const int IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR      = 14;   // COM Runtime descriptor

        public const int IMAGE_REL_BASED_ABSOLUTE              = 0;
        public const int IMAGE_REL_BASED_HIGH                  = 1;
        public const int IMAGE_REL_BASED_LOW                   = 2;
        public const int IMAGE_REL_BASED_HIGHLOW               = 3;
        public const int IMAGE_REL_BASED_HIGHADJ               = 4;
        public const int IMAGE_REL_BASED_MIPS_JMPADDR          = 5;
        public const int IMAGE_REL_BASED_MIPS_JMPADDR16        = 9;
        public const int IMAGE_REL_BASED_IA64_IMM64            = 9;
        public const int IMAGE_REL_BASED_DIR64                 = 10;


        public const uint DLL_PROCESS_ATTACH                  = 1;    
        public const uint DLL_THREAD_ATTACH                   = 2;    
        public const uint DLL_THREAD_DETACH                   = 3;    
        public const uint DLL_PROCESS_DETACH                  = 0;

        /* These are the settings of the Machine field. */
        public const ushort IMAGE_FILE_MACHINE_UNKNOWN = 0;
        public const ushort IMAGE_FILE_MACHINE_I860 = 0x014d;
        public const ushort IMAGE_FILE_MACHINE_I386 = 0x014c;
        public const ushort IMAGE_FILE_MACHINE_R3000 = 0x0162;
        public const ushort IMAGE_FILE_MACHINE_R4000 = 0x0166;
        public const ushort IMAGE_FILE_MACHINE_R10000 = 0x0168;
        public const ushort IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x0169;
        public const ushort IMAGE_FILE_MACHINE_ALPHA = 0x0184;
        public const ushort IMAGE_FILE_MACHINE_SH3 = 0x01a2;
        public const ushort IMAGE_FILE_MACHINE_SH3DSP = 0x01a3;
        public const ushort IMAGE_FILE_MACHINE_SH3E = 0x01a4;
        public const ushort IMAGE_FILE_MACHINE_SH4 = 0x01a6;
        public const ushort IMAGE_FILE_MACHINE_SH5 = 0x01a8;
        public const ushort IMAGE_FILE_MACHINE_ARM = 0x01c0;
        public const ushort IMAGE_FILE_MACHINE_THUMB = 0x01c2;
        public const ushort IMAGE_FILE_MACHINE_ARMNT = 0x01c4;
        public const ushort IMAGE_FILE_MACHINE_ARM64 = 0xaa64;
        public const ushort IMAGE_FILE_MACHINE_AM33 = 0x01d3;
        public const ushort IMAGE_FILE_MACHINE_POWERPC = 0x01f0;
        public const ushort IMAGE_FILE_MACHINE_POWERPCFP = 0x01f1;
        public const ushort IMAGE_FILE_MACHINE_IA64 = 0x0200;
        public const ushort IMAGE_FILE_MACHINE_MIPS16 = 0x0266;
        public const ushort IMAGE_FILE_MACHINE_ALPHA64 = 0x0284;
        public const ushort IMAGE_FILE_MACHINE_MIPSFPU = 0x0366;
        public const ushort IMAGE_FILE_MACHINE_MIPSFPU16 = 0x0466;
        public const ushort IMAGE_FILE_MACHINE_AXP64 = IMAGE_FILE_MACHINE_ALPHA64;
        public const ushort IMAGE_FILE_MACHINE_TRICORE = 0x0520;
        public const ushort IMAGE_FILE_MACHINE_CEF = 0x0cef;
        public const ushort IMAGE_FILE_MACHINE_EBC = 0x0ebc;
        public const ushort IMAGE_FILE_MACHINE_AMD64 = 0x8664;
        public const ushort IMAGE_FILE_MACHINE_M32R = 0x9041;
        public const ushort IMAGE_FILE_MACHINE_CEE = 0xc0ee;

        #endregion

        #region Structures

        #region IMAGE_DOS_HEADER

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_DOS_HEADER                         // DOS .EXE header
        {
            public ushort e_magic;                      // Magic number
            public ushort e_cblp;                       // Bytes on last page of file
            public ushort e_cp;                         // Pages in file
            public ushort e_crlc;                       // Relocations
            public ushort e_cparhdr;                    // Size of header in paragraphs
            public ushort e_minalloc;                   // Minimum extra paragraphs needed
            public ushort e_maxalloc;                   // Maximum extra paragraphs needed
            public ushort e_ss;                         // Initial (relative) SS value
            public ushort e_sp;                         // Initial SP value
            public ushort e_csum;                       // Checksum
            public ushort e_ip;                         // Initial IP value
            public ushort e_cs;                         // Initial (relative) CS value
            public ushort e_lfarlc;                     // File address of relocation table
            public ushort e_ovno;                       // Overlay number
            public fixed ushort e_res[4];               // Reserved ushorts
            public ushort e_oemid;                      // OEM identifier (for e_oeminfo)
            public ushort e_oeminfo;                    // OEM information; e_oemid specific
            public fixed ushort e_res2[10];             // Reserved ushorts
            public uint e_lfanew;                       // File address of new exe header
        }

        #endregion

        #region IMAGE_NT_HEADERS32

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_NT_HEADERS32
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
        }

        #endregion

        #region IMAGE_NT_HEADERS64

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_NT_HEADERS64
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER64 OptionalHeader;
        }

        #endregion

        #region IMAGE_FILE_HEADER

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        }

        #endregion

        #region IMAGE_OPTIONAL_HEADER32

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public uint BaseOfData;
            public IntPtr ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public uint SizeOfStackReserve;
            public uint SizeOfStackCommit;
            public uint SizeOfHeapReserve;
            public uint SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            public fixed ulong DataDirectory[IMAGE_NUMBEROF_DIRECTORY_ENTRIES];
        }

        #endregion

        #region IMAGE_OPTIONAL_HEADER64

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public IntPtr ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public ulong SizeOfStackReserve;
            public ulong SizeOfStackCommit;
            public ulong SizeOfHeapReserve;
            public ulong SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            public fixed ulong DataDirectory[IMAGE_NUMBEROF_DIRECTORY_ENTRIES];
        }

        #endregion

        #region IMAGE_DATA_DIRECTORY

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_DATA_DIRECTORY
        {
            public uint VirtualAddress;
            public uint Size;
        }

        #endregion

        #region IMAGE_SECTION_HEADER

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_SECTION_HEADER
        {
            public fixed byte Name[IMAGE_SIZEOF_SHORT_NAME];
            public uint PhysicalAddress;
            public uint VirtualAddress;
            public uint SizeOfRawData;
            public uint PointerToRawData;
            public uint PointerToRelocations;
            public uint PointerToLinenumbers;
            public ushort NumberOfRelocations;
            public ushort NumberOfLinenumbers;
            public uint Characteristics;
        }

        #endregion

        #region IMAGE_BASE_RELOCATION

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_BASE_RELOCATION
        {
            public uint VirtualAddress;
            public uint SizeOfBlock;
        }

        #endregion

        #region IMAGE_IMPORT_DESCRIPTOR

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_IMPORT_DESCRIPTOR
        {
            public uint Characteristics;                
            public uint TimeDateStamp;                  
            public uint ForwarderChain;                 
            public uint Name;
            public uint FirstThunk;
        }

        #endregion

        #region IMAGE_IMPORT_BY_NAME

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_IMPORT_BY_NAME
        {
            public ushort Hint;
            public fixed byte Name[1];
        }

        #endregion

        #region IMAGE_EXPORT_DIRECTORY

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_EXPORT_DIRECTORY
        {
            public uint Characteristics;
            public uint TimeDateStamp;
            public ushort MajorVersion;
            public ushort MinorVersion;
            public uint Name;
            public uint Base;
            public uint NumberOfFunctions;
            public uint NumberOfNames;
            public uint AddressOfFunctions;     // RVA from base of image
            public uint AddressOfNames;         // RVA from base of image
            public uint AddressOfNameOrdinals;  // RVA from base of image
        }

        #endregion

        #endregion

    }
}
