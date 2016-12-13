//
// BufferHelper.cs
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

namespace Ghostscript.NET
{
    internal class BufferHelper
    {

        #region IndexOf

        /// <summary>
        /// The Knuth-Morris-Pratt Pattern Matching Algorithm.
        /// </summary>
        public static int IndexOf(byte[] data, byte[] pattern)
        {
            int[] failure = ComputeFailure(pattern);

            int j = 0;

            for (int i = 0; i < data.Length; i++)
            {
                while (j > 0 && pattern[j] != data[i])
                {
                    j = failure[j - 1];
                }

                if (pattern[j] == data[i])
                {
                    j++;
                }

                if (j == pattern.Length)
                {
                    return i - pattern.Length + 1;
                }
            }

            return -1;
        }

        #endregion

        #region ComputeFailure

        private static int[] ComputeFailure(byte[] pattern)
        {
            int[] failure = new int[pattern.Length];

            int j = 0;

            for (int i = 1; i < pattern.Length; i++)
            {
                while (j > 0 && pattern[j] != pattern[i])
                {
                    j = failure[j - 1];
                }

                if (pattern[j] == pattern[i])
                {
                    j++;
                }

                failure[i] = j;
            }

            return failure;
        }

        #endregion

    }
}
