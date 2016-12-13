//
// GhostscriptProcessorInternalStdIOHandler.cs
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

namespace Ghostscript.NET.Processor
{
    internal class GhostscriptProcessorInternalStdIOHandler : GhostscriptStdIO
    {
        private StdInputEventHandler _input;
        private StdOutputEventHandler _output;
        private StdErrorEventHandler _error;

        public GhostscriptProcessorInternalStdIOHandler(StdInputEventHandler input, StdOutputEventHandler output, StdErrorEventHandler error) : base(true, true, true) 
        {
            _input = input;
            _output = output;
            _error = error;
        }

        public override void StdIn(out string input, int count)
        {
            _input(out input, count);
        }

        public override void StdOut(string output)
        {
            _output(output);
        }

        public override void StdError(string error)
        {
            _error(error);
        }
    }
}
