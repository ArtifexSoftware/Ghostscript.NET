//
// OfficeSupportSample.cs
// This file is part of Ghostscript.NET.Samples project
//
// Author: Artifex Software Inc.
// Copyright (c) 2026 by Artifex Software Inc. All rights reserved.
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
using Ghostscript.NET.Processor;
using Ghostscript.NET.Rasterizer;
using SkiaSharp;

namespace Ghostscript.NET.Samples
{
    /// <summary>
    /// Office files require a licensed GhostPDL library (<c>gpdldll64.dll</c> / <c>libgpdl.so</c>),
    /// not standard Ghostscript. Place the DLL from Ghostscript.NET.Office in the app folder
    /// or set <c>GHOSTPDL_DLL</c>.
    /// </summary>
    public class OfficeSupportSample : ISample
    {
        public void Start()
        {
            GhostscriptVersionInfo pdl;
            if (!GhostscriptVersionInfo.TryGetGhostPdlVersion(out pdl))
            {
                Console.WriteLine(GhostscriptOffice.CommercialLicenseRequiredMessage);
                Console.WriteLine("Skipping OfficeSupportSample: no licensed GhostPDL library was found.");
                return;
            }

            string inputPath = FindSampleOfficeFile();
            if (inputPath == null)
            {
                Console.WriteLine("Skipping OfficeSupportSample: no .doc/.docx test file found.");
                return;
            }

            Console.WriteLine("Using GhostPDL: " + pdl.DllPath);
            Console.WriteLine("Input: " + inputPath);

            string outputDir = Path.GetFullPath("Output");
            Directory.CreateDirectory(outputDir);

            string pdfPath = Path.Combine(outputDir, "OfficeSupportSample.pdf");
            GhostscriptOffice.ConvertToPdf(inputPath, pdfPath, pdl);
            Console.WriteLine("PDF: " + pdfPath);

            using (GhostscriptProcessor processor = GhostscriptProcessor.CreateForInput(inputPath))
            {
                processor.Process(new[]
                {
                    "-ghostscript.net",
                    "-dNOPAUSE",
                    "-dBATCH",
                    "-sDEVICE=png16m",
                    "-r96",
                    "-sOutputFile=" + Path.Combine(outputDir, "OfficeSupportSample-%03d.png"),
                    "-f",
                    Path.GetFullPath(inputPath)
                });
            }

            using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(inputPath);
                Console.WriteLine("Page count: " + rasterizer.PageCount);

                if (rasterizer.PageCount > 0)
                {
                    SKBitmap page = rasterizer.GetPage(96, 1);
                    if (page != null)
                    {
                        string pngPath = Path.Combine(outputDir, "OfficeSupportSample-rasterizer.png");
                        using (SKImage image = SKImage.FromBitmap(page))
                        using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                        using (FileStream stream = File.OpenWrite(pngPath))
                        {
                            data.SaveTo(stream);
                        }

                        Console.WriteLine("Rasterizer page 1: " + pngPath);
                    }
                }
            }
        }

        private static string FindSampleOfficeFile()
        {
            string[] names = { "OfficeSample.doc", "OfficeSample.docx" };
            string dir = AppContext.BaseDirectory;

            for (int i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
            {
                foreach (string name in names)
                {
                    string candidate = Path.Combine(dir, "TestFiles", name);
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }

                    candidate = Path.Combine(dir, name);
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }

                dir = Path.GetDirectoryName(dir);
            }

            return null;
        }
    }
}
