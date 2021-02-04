//
// DynamicNativeLibrary.cs
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
// 
// Parts of this source code are ported from C to C# by Josip Habjan
//
// The Original Code is MemoryModule.c 
// (https://github.com/fancycode/MemoryModule/blob/master/MemoryModule.c);
// and it's under Mozilla Public License Version 1.1 
// (http://www.mozilla.org/MPL/) 
// The Initial Developer of the Original Code is Joachim Bauch
// Copyright (C) 2004-2012 Joachim Bauch (mail@joachim-bauch.de). 

using System;
using System.Runtime.InteropServices;

namespace Microsoft.WinAny.Interop
{
    /// <summary>
    /// Class that helps you to load and use native/unmanaged dynamic-link libraries dinamically.
    /// It provides ability to load libraries from the memory or disk. 
    /// It's compatibile with x86 and x64 libraries.
    /// </summary>
    public unsafe class DynamicNativeLibrary : IDisposable
    {

        #region Private variables

        private IntPtr  _loadedModuleHandle;
        private bool    _loadedFromMemory;
        private bool    _disposed = false;

        private uint[,,] _protectionFlags = new uint[2, 2, 2]
                    {
                        { /* not executable */ {WinNT.PAGE_NOACCESS, WinNT.PAGE_WRITECOPY}, {WinNT.PAGE_READONLY, WinNT.PAGE_READWRITE}, }, 
                        { /* executable */ {WinNT.PAGE_EXECUTE, WinNT.PAGE_EXECUTE_WRITECOPY}, {WinNT.PAGE_EXECUTE_READ, WinNT.PAGE_EXECUTE_READWRITE}, },
                    };

        #endregion

        #region Constructor - fileName

        /// <summary>
        /// Initializes a new instance of the NativeLibrary class from a native module stored on disk.
        /// </summary>
        /// <param name="lpLibFileName">Native module file name.</param>
        public DynamicNativeLibrary(string fileName)
        {
            _loadedModuleHandle = WinBase.LoadLibrary(fileName);

            if (_loadedModuleHandle == IntPtr.Zero)
                throw new Exception("Module could not be loaded.");

            _loadedFromMemory = false;
        }

        #endregion

        #region Constructor - buffer

        /// <summary>
        /// Initializes a new instance of the NativeLibrary class from a native module byte array.
        /// </summary>
        /// <param name="buffer">Native module byte array.</param>
        public DynamicNativeLibrary(byte[] buffer)
        {
            _loadedModuleHandle = MemoryLoadLibrary(buffer);

            if (_loadedModuleHandle == IntPtr.Zero)
                throw new Exception("Module could not be loaded.");

            _loadedFromMemory = true;
        }

        #endregion

        #region Destructor

        ~DynamicNativeLibrary()
        {
            Dispose(false);
        }

        #endregion

        #region Dispose

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Dispose - disposing

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // free managed resources
                }

                if (_loadedModuleHandle != IntPtr.Zero)
                {
                    if (_loadedFromMemory)
                    {
                        this.MemoryFreeLibrary(_loadedModuleHandle);
                    }
                    else
                    {
                        WinBase.FreeLibrary(_loadedModuleHandle);
                    }

                    _loadedModuleHandle = IntPtr.Zero;
                }

                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region MemoryLoadLibrary

        /// <summary>
        /// Loads the specified native module from a byte array into the address space of the calling process.
        /// </summary>
        /// <param name="data">Native module byte array.</param>
        /// <returns>If the function succeeds, the return value is a handle to the module.</returns>
        private IntPtr MemoryLoadLibrary(byte[] data)
        {
            fixed (byte* ptr_data = data)
            {
                WinNT.IMAGE_DOS_HEADER* dos_header = (WinNT.IMAGE_DOS_HEADER*)ptr_data;

                if (dos_header->e_magic != WinNT.IMAGE_DOS_SIGNATURE)
                {
                    throw new NotSupportedException();
                }

                byte* ptr_old_header;
                uint old_header_oh_sizeOfImage;
                uint old_header_oh_sizeOfHeaders;
                int image_nt_headers_Size;
                IntPtr old_header_oh_imageBase;

                if (Environment.Is64BitProcess)
                {
                    WinNT.IMAGE_NT_HEADERS64* old_header = (WinNT.IMAGE_NT_HEADERS64*)(ptr_data + dos_header->e_lfanew);
                    if (old_header->Signature != WinNT.IMAGE_NT_SIGNATURE)
                    {
                        throw new NotSupportedException();
                    }

                    old_header_oh_sizeOfImage = old_header->OptionalHeader.SizeOfImage;
                    old_header_oh_sizeOfHeaders = old_header->OptionalHeader.SizeOfHeaders;
                    old_header_oh_imageBase = old_header->OptionalHeader.ImageBase;
                    ptr_old_header = (byte*)old_header;

                    image_nt_headers_Size = sizeof(WinNT.IMAGE_NT_HEADERS64);
                }
                else
                {
                    WinNT.IMAGE_NT_HEADERS32* old_header = (WinNT.IMAGE_NT_HEADERS32*)(ptr_data + dos_header->e_lfanew);
                    if (old_header->Signature != WinNT.IMAGE_NT_SIGNATURE)
                    {
                        throw new NotSupportedException();
                    }

                    old_header_oh_sizeOfImage = old_header->OptionalHeader.SizeOfImage;
                    old_header_oh_sizeOfHeaders = old_header->OptionalHeader.SizeOfHeaders;
                    old_header_oh_imageBase = old_header->OptionalHeader.ImageBase;
                    ptr_old_header = (byte*)old_header;

                    image_nt_headers_Size = sizeof(WinNT.IMAGE_NT_HEADERS32);
                }

                IntPtr codeBase = IntPtr.Zero;

                if (!Environment.Is64BitProcess)
                {
                    codeBase = WinBase.VirtualAlloc(old_header_oh_imageBase, old_header_oh_sizeOfImage, WinNT.MEM_RESERVE, WinNT.PAGE_READWRITE);
                }

                if (codeBase == IntPtr.Zero)
                    codeBase = WinBase.VirtualAlloc(IntPtr.Zero, old_header_oh_sizeOfImage, WinNT.MEM_RESERVE, WinNT.PAGE_READWRITE);

                if (codeBase == IntPtr.Zero)
                    return IntPtr.Zero;

                MEMORY_MODULE* memory_module = (MEMORY_MODULE*)Marshal.AllocHGlobal(sizeof(MEMORY_MODULE));
                memory_module->codeBase = (byte*)codeBase;
                memory_module->numModules = 0;
                memory_module->modules = null;
                memory_module->initialized = 0;

                WinBase.VirtualAlloc(codeBase, old_header_oh_sizeOfImage, WinNT.MEM_COMMIT, WinNT.PAGE_READWRITE);

                IntPtr headers = WinBase.VirtualAlloc(codeBase, old_header_oh_sizeOfHeaders, WinNT.MEM_COMMIT, WinNT.PAGE_READWRITE);


                // copy PE header to code
                memory.memcpy((byte*)headers, (byte*)dos_header, dos_header->e_lfanew + old_header_oh_sizeOfHeaders);
               
                memory_module->headers = &((byte*)(headers))[dos_header->e_lfanew];

                if (Environment.Is64BitProcess)
                {
                    WinNT.IMAGE_NT_HEADERS64* mm_headers_64 = (WinNT.IMAGE_NT_HEADERS64*)(memory_module->headers);
                    mm_headers_64->OptionalHeader.ImageBase = codeBase;
                }
                else
                {
                    WinNT.IMAGE_NT_HEADERS32* mm_headers_32 = (WinNT.IMAGE_NT_HEADERS32*)(memory_module->headers);
                    mm_headers_32->OptionalHeader.ImageBase = codeBase;
                }

                this.CopySections(ptr_data, ptr_old_header, memory_module);

                ulong locationDelta = (ulong)((ulong)codeBase - (ulong)old_header_oh_imageBase);

                if (locationDelta != 0)
                {
                    this.PerformBaseRelocation(memory_module, locationDelta);
                }

                if (!this.BuildImportTable(memory_module))
                {
                    goto error;
                }

                this.FinalizeSections(memory_module);

                if (!this.CallDllEntryPoint(memory_module, WinNT.DLL_PROCESS_ATTACH))
                {
                    goto error;
                }

                return (IntPtr)memory_module;

            error:
                MemoryFreeLibrary((IntPtr)memory_module);
                return IntPtr.Zero;
            }
        }

        #endregion

        #region CopySections

        /// <summary>
        /// Copies sections from a native module file block to the new memory location.
        /// </summary>
        /// <param name="ptr_data">Pointer to a native module byte array.</param>
        /// <param name="ptr_old_headers">Pointer to a source native module headers.</param>
        /// <param name="memory_module">Pointer to a memory module.</param>
        private void CopySections(byte* ptr_data, byte* ptr_old_headers, MEMORY_MODULE* memory_module)
        {
            byte* codeBase = memory_module->codeBase;
            WinNT.IMAGE_SECTION_HEADER* section = WinNT.IMAGE_FIRST_SECTION(memory_module->headers);

            ushort numberOfSections;
            uint sectionAlignment;

            if (Environment.Is64BitProcess)
            {
                WinNT.IMAGE_NT_HEADERS64* new_headers = (WinNT.IMAGE_NT_HEADERS64*)memory_module->headers;
                numberOfSections = new_headers->FileHeader.NumberOfSections;

                WinNT.IMAGE_NT_HEADERS64* old_headers = (WinNT.IMAGE_NT_HEADERS64*)ptr_old_headers;
                sectionAlignment = old_headers->OptionalHeader.SectionAlignment;
            }
            else
            {
                WinNT.IMAGE_NT_HEADERS32* new_headers = (WinNT.IMAGE_NT_HEADERS32*)memory_module->headers;
                numberOfSections = new_headers->FileHeader.NumberOfSections;

                WinNT.IMAGE_NT_HEADERS32* old_headers = (WinNT.IMAGE_NT_HEADERS32*)ptr_old_headers;
                sectionAlignment = old_headers->OptionalHeader.SectionAlignment;
            }

            uint index;
            byte* dest;

            for (index = 0; index < numberOfSections; index++, section++)
            {
                if (section->SizeOfRawData == 0)
                {
                    if (sectionAlignment > 0)
                    {
                        dest = (byte*)WinBase.VirtualAlloc((IntPtr)(codeBase + section->VirtualAddress), sectionAlignment, WinNT.MEM_COMMIT, WinNT.PAGE_READWRITE);
                        section->PhysicalAddress = (uint)dest;
                        memory.memset(dest, 0, sectionAlignment);
                    }

                    continue;
                }

                // commit memory block and copy data from dll
                dest = (byte*)WinBase.VirtualAlloc((IntPtr)(codeBase + section->VirtualAddress), section->SizeOfRawData, WinNT.MEM_COMMIT, WinNT.PAGE_READWRITE);
                memory.memcpy(dest, ptr_data + section->PointerToRawData, section->SizeOfRawData);

                section->PhysicalAddress = (uint)dest;
            }
        }

        #endregion

        #region PerformBaseRelocation

        /// <summary>
        /// Adjusts base address of the imported data.
        /// </summary>
        /// <param name="memory_module">Pointer to a memory module.</param>
        /// <param name="delta">Adjustment delta value.</param>
        private void PerformBaseRelocation(MEMORY_MODULE* memory_module, ulong delta)
        {
            WinNT.IMAGE_DATA_DIRECTORY* directory = this.GET_HEADER_DIRECTORY(memory_module, WinNT.IMAGE_DIRECTORY_ENTRY_BASERELOC);

            if (directory->Size > 0)
            {
                WinNT.IMAGE_BASE_RELOCATION* relocation = (WinNT.IMAGE_BASE_RELOCATION*)(memory_module->codeBase + directory->VirtualAddress);

                int sizeOfBaseRelocation = sizeof(WinNT.IMAGE_BASE_RELOCATION);

                int index;

                for (; relocation->VirtualAddress > 0; )
                {
                    byte* dest = (byte*)(memory_module->codeBase + relocation->VirtualAddress);
                    ushort* relInfo = (ushort*)((byte*)relocation + sizeOfBaseRelocation);

                    for (index = 0; index < ((relocation->SizeOfBlock - sizeOfBaseRelocation) / 2); index++, relInfo++)
                    {
                        uint* patchAddrHL32;
                        ulong* patchAddrHL64;

                        uint type, offset;

                        // the upper 4 bits define the type of relocation
                        type = (uint)(*relInfo >> 12);

                        // the lower 12 bits define the offset
                        offset = (uint)(*relInfo & 0xfff);

                        switch (type)
                        {
                            case WinNT.IMAGE_REL_BASED_ABSOLUTE:
                                break;

                            case WinNT.IMAGE_REL_BASED_HIGHLOW:
                                patchAddrHL32 = (uint*)((uint)dest + offset);
                                *patchAddrHL32 += (uint)delta;
                                break;


                            case WinNT.IMAGE_REL_BASED_DIR64:
                                patchAddrHL64 = (ulong*)((ulong)dest + offset);
                                *patchAddrHL64 += delta;
                                break;

                            default:
                                break;
                        }
                    }

                    relocation = (WinNT.IMAGE_BASE_RELOCATION*)((byte*)relocation + relocation->SizeOfBlock);
                }
            }
        }

        #endregion

        #region BuildImportTable

        /// <summary>
        /// Loads required dlls and adjust function table of the imports.
        /// </summary>
        /// <param name="memory_module">Pointer to a memory module.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        private bool BuildImportTable(MEMORY_MODULE* memory_module)
        {
            bool result = true;

            WinNT.IMAGE_DATA_DIRECTORY* directory = this.GET_HEADER_DIRECTORY(memory_module, WinNT.IMAGE_DIRECTORY_ENTRY_IMPORT);

            if (directory->Size > 0)
            {
                WinNT.IMAGE_IMPORT_DESCRIPTOR* importDesc = (WinNT.IMAGE_IMPORT_DESCRIPTOR*)(memory_module->codeBase + directory->VirtualAddress);

                for (; importDesc->Name != 0; importDesc++)
                {
                    IntPtr* thunkRef;
                    IntPtr* funcRef;

                    string moduleName = Marshal.PtrToStringAnsi((IntPtr)(memory_module->codeBase + importDesc->Name));
                    IntPtr handle = WinBase.LoadLibrary(moduleName);

                    if (handle == IntPtr.Zero)
                    {
                        result = false;
                        break;
                    }

                    int size_of_pointer = sizeof(IntPtr);

                    memory_module->modules = (IntPtr*)memory.realloc((byte*)memory_module->modules,
                                                                     (uint)((memory_module->numModules) * size_of_pointer),
                                                                     (uint)((memory_module->numModules + 1) * size_of_pointer));


                    if (memory_module->modules == null)
                    {
                        result = false;
                        break;
                    }

                    memory_module->modules[memory_module->numModules++] = handle;

                    if (importDesc->Characteristics != 0)
                    {
                        thunkRef = (IntPtr*)(memory_module->codeBase + importDesc->Characteristics);
                        funcRef = (IntPtr*)(memory_module->codeBase + importDesc->FirstThunk);
                    }
                    else
                    {
                        thunkRef = (IntPtr*)(memory_module->codeBase + importDesc->FirstThunk);
                        funcRef = (IntPtr*)(memory_module->codeBase + importDesc->FirstThunk);
                    }

                    for (; *thunkRef != IntPtr.Zero; thunkRef++, funcRef++)
                    {
                        if (WinNT.IMAGE_SNAP_BY_ORDINAL(thunkRef))
                        {
                            *funcRef = WinBase.GetProcAddress(handle, (byte*)WinNT.IMAGE_ORDINAL(thunkRef));
                        }
                        else
                        {
                            WinNT.IMAGE_IMPORT_BY_NAME* thunkData = (WinNT.IMAGE_IMPORT_BY_NAME*)(memory_module->codeBase + (ulong)*thunkRef);
                            //string procName = Marshal.PtrToStringAnsi((IntPtr)(byte*)(thunkData) + 2);
                            IntPtr a = (IntPtr)(byte*)(thunkData);
                            string procName = Marshal.PtrToStringAnsi(new IntPtr(a.ToInt64() + 2));
                            *funcRef = WinBase.GetProcAddress(handle, procName);
                        }

                        if (*funcRef == IntPtr.Zero)
                        {
                            result = false;
                            break;
                        }
                    }

                    if (!result)
                        break;
                }
            }

            return result;
        }

        #endregion

        #region FinalizeSections

        /// <summary>
        /// Marks memory pages depending on section headers and release sections that are marked as "discardable".
        /// </summary>
        /// <param name="memory_module">Pointer to a memory module.</param>
        private void FinalizeSections(MEMORY_MODULE* memory_module)
        {
            WinNT.IMAGE_SECTION_HEADER* section = WinNT.IMAGE_FIRST_SECTION(memory_module->headers); ;

            ushort number_of_sections;
            uint size_of_initialized_data;
            uint size_of_uninitialized_data;

            long image_offset = 0;

            if (Environment.Is64BitProcess)
            {
                WinNT.IMAGE_NT_HEADERS64* headers = (WinNT.IMAGE_NT_HEADERS64*)memory_module->headers;
                number_of_sections = headers->FileHeader.NumberOfSections;
                size_of_initialized_data = headers->OptionalHeader.SizeOfInitializedData;
                size_of_uninitialized_data = headers->OptionalHeader.SizeOfUninitializedData;

                image_offset = (long)((ulong)headers->OptionalHeader.ImageBase & 0xffffffff00000000);
            }
            else
            {
                WinNT.IMAGE_NT_HEADERS32* headers = (WinNT.IMAGE_NT_HEADERS32*)memory_module->headers;
                number_of_sections = headers->FileHeader.NumberOfSections;
                size_of_initialized_data = headers->OptionalHeader.SizeOfInitializedData;
                size_of_uninitialized_data = headers->OptionalHeader.SizeOfUninitializedData;
            }

            for (int i = 0; i < number_of_sections; i++, section++)
            {
                uint protect, oldProtect, rawDataSize;
                uint executable = Convert.ToUInt32((section->Characteristics & WinNT.IMAGE_SCN_MEM_EXECUTE) != 0);
                uint readable = Convert.ToUInt32((section->Characteristics & WinNT.IMAGE_SCN_MEM_READ) != 0);
                uint writeable = Convert.ToUInt32((section->Characteristics & WinNT.IMAGE_SCN_MEM_WRITE) != 0);

                if ((section->Characteristics & WinNT.IMAGE_SCN_MEM_DISCARDABLE) != 0)
                {
                    // section is not needed any more and can safely be freed
                    WinBase.VirtualFree((IntPtr)(void*)((long)section->PhysicalAddress | (long)image_offset), section->SizeOfRawData, WinNT.MEM_DECOMMIT);
                    continue;
                }

                protect = _protectionFlags[executable, readable, writeable];

                if ((section->Characteristics & WinNT.IMAGE_SCN_MEM_NOT_CACHED) != 0)
                    protect |= WinNT.PAGE_NOCACHE;

                // determine size of region
                rawDataSize = section->SizeOfRawData;

                if (rawDataSize == 0)
                {
                    if ((section->Characteristics & WinNT.IMAGE_SCN_CNT_INITIALIZED_DATA) != 0)
                        rawDataSize = size_of_initialized_data;

                    else if ((section->Characteristics & WinNT.IMAGE_SCN_CNT_UNINITIALIZED_DATA) != 0)
                        rawDataSize = size_of_uninitialized_data;
                }

                if (rawDataSize > 0)
                {
                    // change memory access flags
                    WinBase.VirtualProtect((IntPtr)(void*)((long)section->PhysicalAddress | (long)image_offset), rawDataSize, protect, &oldProtect);
                }
            }
        }

        #endregion

        #region CallDllEntryPoint

        /// <summary>
        /// Calls module entry point.
        /// </summary>
        /// <param name="memory_module">Pointer to a memory module.</param>
        /// <param name="fdwReason"></param>
        /// <returns>If the function succeeds or if there is no entry point, the return value is true.</returns>
        private bool CallDllEntryPoint(MEMORY_MODULE* memory_module, uint fdwReason)
        {
            uint addressOfEntryPoint;

            if (Environment.Is64BitProcess)
            {
                WinNT.IMAGE_NT_HEADERS64* headers = (WinNT.IMAGE_NT_HEADERS64*)memory_module->headers;
                addressOfEntryPoint = headers->OptionalHeader.AddressOfEntryPoint;
            }
            else
            {
                WinNT.IMAGE_NT_HEADERS32* headers = (WinNT.IMAGE_NT_HEADERS32*)memory_module->headers;
                addressOfEntryPoint = headers->OptionalHeader.AddressOfEntryPoint;
            }

            if (addressOfEntryPoint != 0)
            {
                IntPtr dllEntry = (IntPtr)(memory_module->codeBase + addressOfEntryPoint);

                if (dllEntry == IntPtr.Zero)
                {
                    return false;
                }

                DllEntryProc dllEntryProc = (DllEntryProc)Marshal.GetDelegateForFunctionPointer(dllEntry, typeof(DllEntryProc));

                if (dllEntryProc((IntPtr)memory_module->codeBase, fdwReason, 0))
                {
                    if (fdwReason == WinNT.DLL_PROCESS_ATTACH)
                    {
                        memory_module->initialized = 1;
                    }
                    else if (fdwReason == WinNT.DLL_PROCESS_DETACH)
                    {
                        memory_module->initialized = 0;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region MemoryFreeLibrary

        /// <summary>
        /// Deattach from the process and do a cleanup.
        /// </summary>
        /// <param name="hModule">Pointer to a memory module.</param>
        private void MemoryFreeLibrary(IntPtr hModule)
        {
            if (hModule == IntPtr.Zero)
                return;

            MEMORY_MODULE* memory_module = (MEMORY_MODULE*)hModule;

            if (memory_module != null)
            {
                if (memory_module->initialized != 0)
                {
                    this.CallDllEntryPoint(memory_module, WinNT.DLL_PROCESS_DETACH);
                }

                if (memory_module->modules != null)
                {
                    // free previously opened libraries
                    for (int index = 0; index < memory_module->numModules; index++)
                    {
                        if (memory_module->modules[index] != IntPtr.Zero)
                        {
                            WinBase.FreeLibrary(memory_module->modules[index]);
                        }
                    }

                    Marshal.FreeHGlobal((IntPtr)memory_module->modules);
                }

                if ((IntPtr)memory_module->codeBase != IntPtr.Zero)
                {
                    // release memory of library
                    WinBase.VirtualFree((IntPtr)memory_module->codeBase, 0, WinNT.MEM_RELEASE);
                }

                Marshal.FreeHGlobal((IntPtr)memory_module);
            }
        }

        #endregion

        #region GetDelegateForFunction

        /// <summary>
        /// Retrieves a delegate of an exported function or variable from loaded module.
        /// </summary>
        /// <param name="procName">The function or variable name.</param>
        /// <param name="delegateType">The type of the delegate to be returned.</param>
        /// <returns>A function instance.</returns>
        public Delegate GetDelegateForFunction(string procName, Type delegateType)
        {
            IntPtr procAddress = this.GetProcAddress(procName);

            if (procAddress != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer(procAddress, delegateType);
            }

            return null;
        }

        #endregion

        #region GetDelegateForFunction


        /// <summary>
        /// Retrieves a delegate of an exported function or variable from loaded module.
        /// </summary>
        /// <typeparam name="T">Delegate type.</typeparam>
        /// <param name="procName">The function or variable name.</param>
        /// <returns>A function instance.</returns>
        public T GetDelegateForFunction<T>(string procName)
        {
            return (T)(object)GetDelegateForFunction(procName, typeof(T));
        }

        #endregion

        #region GetProcAddress

        /// <summary>
        /// Retrieves the address of an exported function or variable from loaded module.
        /// </summary>
        /// <param name="procName">The function or variable name.</param>
        /// <returns>
        /// If the function succeeds, the return value is the address of the exported function or variable.
        /// If the function fails, the return value is IntPtr.Zero.
        /// </returns>
        private IntPtr GetProcAddress(string procName)
        {
            if (_loadedModuleHandle == IntPtr.Zero)
                return IntPtr.Zero;

            if (!_loadedFromMemory)
            {
                return WinBase.GetProcAddress(_loadedModuleHandle, procName);
            }

            MEMORY_MODULE* memory_module = (MEMORY_MODULE*)_loadedModuleHandle;

            byte* codeBase = memory_module->codeBase;

            int idx = -1;
            uint i;

            uint* nameRef;
            ushort* ordinal;

            
            WinNT.IMAGE_DATA_DIRECTORY* directory = this.GET_HEADER_DIRECTORY(memory_module, WinNT.IMAGE_DIRECTORY_ENTRY_EXPORT);

            if (directory->Size == 0)
                // no export table found
                return IntPtr.Zero;

            WinNT.IMAGE_EXPORT_DIRECTORY* exports = (WinNT.IMAGE_EXPORT_DIRECTORY*)(codeBase + directory->VirtualAddress);

            if (exports->NumberOfNames == 0 || exports->NumberOfFunctions == 0)
                // DLL doesn't export anything
                return IntPtr.Zero;

            // search function name in list of exported names
            nameRef = (uint*)(codeBase + exports->AddressOfNames);
            ordinal = (ushort*)(codeBase + exports->AddressOfNameOrdinals);

            for (i = 0; i < exports->NumberOfNames; i++, nameRef++, ordinal++)
            {
                IntPtr procNameHandle = (IntPtr)((byte*)((ulong)codeBase + *nameRef));
                string testProcName = Marshal.PtrToStringAnsi(procNameHandle);

                if (testProcName == procName)
                {
                    idx = *ordinal;
                    break;
                }
            }

            if (idx == -1)
                // exported symbol not found
                return IntPtr.Zero;

            if ((uint)idx > exports->NumberOfFunctions)
                // name <-> ordinal number don't match
                return IntPtr.Zero;

            // AddressOfFunctions contains the RVAs to the "real" functions
            //return (IntPtr)((uint)codeBase + *(uint*)((uint)codeBase + exports->AddressOfFunctions + (idx * 4)));
            return (IntPtr)(codeBase + *(uint*)(codeBase + exports->AddressOfFunctions + (idx * 4)));
        }

        #endregion

        #region GET_HEADER_DIRECTORY

        private WinNT.IMAGE_DATA_DIRECTORY* GET_HEADER_DIRECTORY(MEMORY_MODULE* memory_module, uint index)
        {
            if (Environment.Is64BitProcess)
            {
                WinNT.IMAGE_NT_HEADERS64* headers = (WinNT.IMAGE_NT_HEADERS64*)memory_module->headers;
                return (WinNT.IMAGE_DATA_DIRECTORY*)(&headers->OptionalHeader.DataDirectory[index]);
            }
            else
            {
                WinNT.IMAGE_NT_HEADERS32* headers = (WinNT.IMAGE_NT_HEADERS32*)memory_module->headers;
                return (WinNT.IMAGE_DATA_DIRECTORY*)(&headers->OptionalHeader.DataDirectory[index]);
            }
        }

        #endregion

        #region MEMORY_MODULE

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct MEMORY_MODULE
        {
            public byte* headers;
            public byte* codeBase;
            public IntPtr* modules;
            public int numModules;
            public int initialized;
        }

        #endregion

        #region DllEntryProc

        private delegate bool DllEntryProc(IntPtr hinstDll, uint fdwReason, uint lpReserved);

        #endregion

    }
}
