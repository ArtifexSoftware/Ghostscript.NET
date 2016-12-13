//
// DeviceUsageSample.cs
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

namespace Ghostscript.NET.Samples
{
    public class DeviceUsageSample : ISample
    {
        public void Start()
        {
            // sample #1:
            this.Export_Second_And_Third_Pdf_Page_As_24bit_Png();

            // sample #2:
            this.Export_Second_And_Third_Pdf_Page_As_Grayscale_Png();
            
            // sample #3:
            this.Export_First_And_Second_Pdf_Page_As_Color_Jpeg();
            
            // sample #4:
            this.Export_First_And_Second_Pdf_Page_As_Grayscale_Jpeg();
        }

        private void Export_Second_And_Third_Pdf_Page_As_24bit_Png()
        {
            GhostscriptPngDevice dev = new GhostscriptPngDevice(GhostscriptPngDeviceType.Png16m);
            dev.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.ResolutionXY = new GhostscriptImageDeviceResolution(96, 96);
            dev.InputFiles.Add(@"E:\gss_test\indispensable.pdf");
            dev.Pdf.FirstPage = 2;
            dev.Pdf.LastPage = 4;
            dev.CustomSwitches.Add("-dDOINTERPOLATE");
            dev.OutputPath = @"E:\gss_test\output\indispensable_color_page_%03d.png";
            dev.Process();
        }

        private void Export_Second_And_Third_Pdf_Page_As_Grayscale_Png()
        {
            GhostscriptPngDevice dev = new GhostscriptPngDevice(GhostscriptPngDeviceType.PngGray);
            dev.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.ResolutionXY = new GhostscriptImageDeviceResolution(96, 96);
            dev.InputFiles.Add(@"E:\gss_test\indispensable.pdf");
            dev.Pdf.FirstPage = 2;
            dev.Pdf.LastPage = 4;
            dev.OutputPath = @"E:\gss_test\output\indispensable_gray_page_%03d.png";
            dev.Process();
        }

        private void Export_First_And_Second_Pdf_Page_As_Color_Jpeg()
        {
            GhostscriptJpegDevice dev = new GhostscriptJpegDevice(GhostscriptJpegDeviceType.Jpeg);
            dev.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.ResolutionXY = new GhostscriptImageDeviceResolution(96, 96);
            dev.JpegQuality = 80;
            dev.InputFiles.Add(@"E:\gss_test\indispensable.pdf");
            dev.Pdf.FirstPage = 2;
            dev.Pdf.LastPage = 4;
            dev.OutputPath = @"E:\gss_test\output\indispensable_color_page_%03d.jpeg";
            dev.Process();
        }

        private void Export_First_And_Second_Pdf_Page_As_Grayscale_Jpeg()
        {
            GhostscriptJpegDevice dev = new GhostscriptJpegDevice(GhostscriptJpegDeviceType.JpegGray);
            dev.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.ResolutionXY = new GhostscriptImageDeviceResolution(96, 96);
            dev.JpegQuality = 80;
            dev.InputFiles.Add(@"E:\gss_test\indispensable.pdf");
            dev.Pdf.FirstPage = 2;
            dev.Pdf.LastPage = 4;
            dev.OutputPath = @"E:\gss_test\output\indispensable_gray_page_%03d.jpeg";
            dev.Process();
        }
    }
}
