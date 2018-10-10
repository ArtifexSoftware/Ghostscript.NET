//
// StreamHelper.cs
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

namespace Ghostscript.NET
{
    internal class StreamHelper
    {

        #region GetStreamExtension

        public static string GetStreamExtension(Stream stream)
        {
            // https://github.com/awslabs/aws-sdk-xamarin/blob/master/AWS.XamarinSDK/AWSSDK_Core/Amazon.Runtime/Internal/Util/HashStream.cs
            // Amazon.Runtime.Internal.Util.HashStream.Position {set; get;} throw NotSupportedException. First check CanSeek and default to PDF file extension
            if (!stream.CanSeek)
            {
                return ".pdf";
            }

            if (stream.Length < 4)
            {
                throw new InvalidDataException("Less than 4 bytes found in stream.");
            }

            stream.Position = 0;

            byte[] test = new byte[4];

            stream.Read(test, 0, 4);

            stream.Position = 0;

            string extension = string.Empty;

            if (test[0] == 0x25 && test[1] == 0x21) // standard ps or eps signature
            {
                extension = ".ps";

                if (stream.Length > 23)
                {
                    test = new byte[23];
                    stream.Read(test, 0, 23);

                    stream.Position = 0;

                    string tmp = System.Text.Encoding.ASCII.GetString(test);

                    if (tmp.ToUpper().Contains("EPS"))
                    {
                        extension = ".eps";
                    }
                }
            }
            else if (test[0] == 0xc5 && test[1] == 0xd0 && test[2] == 0xd3 && test[3] == 0xc6) // eps with preview header signature / magic number (always C5D0D3C6)
            {
                extension = ".eps";
            }
            else if (test[0] == 0x25 && test[1] == 0x50 && test[2] == 0x44 && test[3] == 0x46) // pdf signature
            {
                extension = ".pdf";
            }
            else
            {
                // try to search for pdf signature once again
                // this time look into all first 32 bytes as I run into pdf's that has extra bytes 
                // at the beginning of the pdf file before the actual signature

                stream.Position = 0;

                if (stream.Length > 32)
                {
                    test = new byte[32];
                    stream.Read(test, 0, 32);

                    stream.Position = 0;

                    if (BufferHelper.IndexOf(test, new byte[] { 0x25, 0x50, 0x44, 0x46}) > -1)
                    {
                        extension = ".pdf";
                    }
                }
                
                if(string.IsNullOrWhiteSpace(extension))
                {
                    // we didn't find pdf marker within first 32 bytes, read whole stream and search for pdf marker anywhere
                    BinaryReader reader = new BinaryReader(stream);
                    test = reader.ReadBytes((int)stream.Length);

                    stream.Position = 0;

                    if (BufferHelper.IndexOf(test, new byte[] { 0x25, 0x50, 0x44, 0x46 }) > -1)
                    {
                        extension = ".pdf";
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new FormatException("Stream format is not valid! Please make sure it's PDF, PS or EPS.");
            }

            return extension;
        }

        #endregion

        #region WriteToTemporaryFile

        public static string WriteToTemporaryFile(Stream stream)
        {
            string extension = GetStreamExtension(stream);

            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);

            // https://github.com/awslabs/aws-sdk-xamarin/blob/master/AWS.XamarinSDK/AWSSDK_Core/Amazon.Runtime/Internal/Util/HashStream.cs
            // Amazon.Runtime.Internal.Util.HashStream.Position {set; get;} throw NotSupportedException.
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            using (FileStream fs = File.Create(path))
            {
                CopyStream(stream, fs);
            }

            return path;
        }

        #endregion

        #region CopyStream

        public static void CopyStream(Stream input, Stream output)
        {
            int size = (input.CanSeek) ? Math.Min((int)(input.Length - input.Position), 0x2000) : 0x2000;

            byte[] buffer = new byte[size];

            int n;

            do
            {
                n = input.Read(buffer, 0, buffer.Length);
                output.Write(buffer, 0, n);
            } while (n != 0);
        }

        #endregion

    }
}
