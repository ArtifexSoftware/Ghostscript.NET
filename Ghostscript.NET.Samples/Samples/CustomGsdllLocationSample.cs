//
// CustomGsdllLocationSample.cs
// This file is part of Ghostscript.NET.Samples project
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
using System.Collections.Generic;
using Ghostscript.NET.Interpreter;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Rasterizer;
using Ghostscript.NET.Viewer;

namespace Ghostscript.NET.Samples
{
    public class CustomGsdllLocationSample : ISample
    {
        public void Start()
        {
            // For users who distribute their gsdll32.dll or gsdll64.dll with their application: Ghostscript.NET by default 
            // peeks into the registry to collect all installed Ghostscript locations. If you want to use ghostscript dll from
            // a custom location, it's recommended to do something like this:

            GhostscriptVersionInfo gvi = new GhostscriptVersionInfo(@"e:\dummyfolder\myapplication\gsdll32.dll");

            // and then pass that GhostscriptVersionInfo to the required constructor or method

            // sample #1
            GhostscriptProcessor proc = new GhostscriptProcessor(gvi);

            // sample #2
            GhostscriptRasterizer rast = new GhostscriptRasterizer();
            rast.Open("test.pdf", gvi, true);

            // sample #3
            GhostscriptViewer view = new GhostscriptViewer();
            view.Open("test.pdf", gvi, true);
        }
    }
}
