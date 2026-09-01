//
// ProcessorSample.cs
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
using System.Collections.Generic;
using Ghostscript.NET.Processor;

namespace Ghostscript.NET.Samples
{
    public class SendToPrinterSample : ISample
    {
        public void Start()
        {
            // mswinpr2 + "Microsoft Print to PDF" opens a Save dialog and never returns,
            // which keeps Ghostscript.NET.Samples from exiting.
            string runPrinter = Environment.GetEnvironmentVariable("GHOSTSCRIPT_NET_RUN_PRINTER");
            if (!string.Equals(runPrinter, "1", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Skipping SendToPrinterSample (would wait on a printer/save dialog). Set GHOSTSCRIPT_NET_RUN_PRINTER=1 to run it.");
                return;
            }

            string printerName = "Microsoft Print to PDF";
            string inputFile = SampleFiles.TryGet("SendToPrinterSample.pdf", out string pdf)
                ? pdf
                : @"..\..\..\TestFiles\SendToPrinterSample.pdf";

            using (GhostscriptProcessor processor = new GhostscriptProcessor())
            {
                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dPrinted");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOSAFER");
                switches.Add("-dNumCopies=1");
                switches.Add("-sDEVICE=mswinpr2");
                switches.Add("-sOutputFile=%printer%" + printerName);
                switches.Add("-f");
                switches.Add(inputFile);

                processor.StartProcessing(switches.ToArray(), null);
            }
        }
    }
}
