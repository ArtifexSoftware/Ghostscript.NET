//
// AddWatermarkSample.cs
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
using System.Diagnostics;
using Ghostscript.NET;
using Ghostscript.NET.Processor;

namespace Ghostscript.NET.Samples
{
    public class AddWatermarkSample : ISample
    {

        private const string POSTSCRIPT_APPEND_WATERMARK = @"
            /watermarkText { (WATERMARK TEXT) } def
            /watermarkFont { /Helvetica-Bold 72 selectfont } def
            /watermarkColor { .75 setgray } def
            /watermarkAngle { 45 } def

            /pageWidth { currentpagedevice /PageSize get 0 get } def
            /pageHeight { currentpagedevice /PageSize get 1 get } def
			
            <<
	            /EndPage {
		            2 eq { pop false } 
		            {
			            gsave
			            watermarkFont
			            watermarkColor
			            pageWidth .5 mul pageHeight .5 mul translate
			            0 0 moveto
			            watermarkText false charpath flattenpath pathbbox
			            4 2 roll pop pop
			            0 0 moveto
			            watermarkAngle rotate
			            -.5 mul exch -.5 mul exch
			            rmoveto
			            watermarkText show
			            grestore 
			            true 
		            } ifelse
	            } bind
            >> setpagedevice";

        public void Start()
        {
            // sample #1:
            this.Extract_Pdf_Pages_As_Png_And_Add_Watermark();

            // sample #2:
            this.Add_Watermark_To_PDF_Document();
        }

        private void Extract_Pdf_Pages_As_Png_And_Add_Watermark()
        {
            GhostscriptPngDevice dev = new GhostscriptPngDevice(GhostscriptPngDeviceType.Png16m);
            dev.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.ResolutionXY = new GhostscriptImageDeviceResolution(96, 96);
            dev.InputFiles.Add(@"E:\gss_test\indispensable.pdf");
            dev.Pdf.FirstPage = 2;
            dev.Pdf.LastPage = 4;
            dev.PostScript = POSTSCRIPT_APPEND_WATERMARK;
            dev.OutputPath = @"E:\gss_test\output\indispensable_color_page_%03d.png";
            dev.Process();
        }

        private void Add_Watermark_To_PDF_Document()
        {
            string inputFile = @"E:\gss_test\test.pdf";
            string outputFile = @"E:\gss_test\output\test-watermarked.pdf";

            List<string> switches = new List<string>();
            switches.Add(string.Empty);

            // set required switches
            switches.Add("-dBATCH");
            switches.Add("-dNOPAUSE");
            switches.Add("-dNOPAUSE");
            switches.Add("-sDEVICE=pdfwrite");
            switches.Add("-sOutputFile=" + outputFile);
            switches.Add("-c");
            switches.Add(POSTSCRIPT_APPEND_WATERMARK);
            switches.Add("-f");
            switches.Add(inputFile);

            // create a new instance of the GhostscriptProcessor
            using (GhostscriptProcessor processor = new GhostscriptProcessor())
            {
                // start processing pdf file
                processor.StartProcessing(switches.ToArray(), null);
            }

            // show new pdf
            Process.Start(outputFile);
        }

    }
}
