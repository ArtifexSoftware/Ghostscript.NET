//
// RasterizerSample.cs
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// required Ghostscript.NET namespaces
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

namespace Ghostscript.NET.Samples
{

    /// <summary>
    /// GhostscriptRasterizer allows you to rasterize pdf and postscript files into the 
    /// memory. If you want Ghostscript to store files on the disk use GhostscriptProcessor
    /// or one of the GhostscriptDevices (GhostscriptPngDevice, GhostscriptJpgDevice).
    /// </summary>
    public class RasterizerSample2 : ISample
    {
        public void Start()
        {
            int desired_dpi = 96;

            string inputPdfPath = @"E:\gss_test\test.pdf";
            string outputPath = @"E:\gss_test\output\";

            using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
            {
                /* custom switches can be added before the file is opened
                
                rasterizer.CustomSwitches.Add("-dPrinted");
                 
                */

                byte[] buffer = File.ReadAllBytes(inputPdfPath);
                MemoryStream ms = new MemoryStream(buffer);

                rasterizer.Open(ms);

                for (int pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {
                    string pageFilePath = Path.Combine(outputPath, "Page-" + pageNumber.ToString() + ".png");

                    Image img = rasterizer.GetPage(desired_dpi, pageNumber);
                    img.Save(pageFilePath, ImageFormat.Png);

                    Console.WriteLine(pageFilePath);
                }
            }
        }
    }
}

