//
// ProcessorSample.cs
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
using Ghostscript.NET.Processor;

namespace Ghostscript.NET.Samples
{
    public class ProcessorSample2 : ISample
    {
        public void Start()
        {
            string inputFile = @"E:\gss_test\test.pdf";
            string outputFile = @"E:\gss_test\output\page-%03d.png";

            int pageFrom = 1;
            int pageTo = 50;

            GhostscriptVersionInfo gv = GhostscriptVersionInfo.GetLastInstalledVersion();

            using (GhostscriptProcessor processor = new GhostscriptProcessor(gv, true))
            {
                processor.Processing += new GhostscriptProcessorProcessingEventHandler(processor_Processing);

                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dSAFER");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOPROMPT");
                switches.Add(@"-sFONTPATH=" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts));
                switches.Add("-dFirstPage=" + pageFrom.ToString());
                switches.Add("-dLastPage=" + pageTo.ToString());
                switches.Add("-sDEVICE=png16m");
                switches.Add("-r96");
                switches.Add("-dTextAlphaBits=4");
                switches.Add("-dGraphicsAlphaBits=4");
                switches.Add(@"-sOutputFile=" + outputFile);
                switches.Add(@"-f");
                switches.Add(inputFile);

                // if you dont want to handle stdio, you can pass 'null' value as the last parameter
                LogStdio stdio = new LogStdio();
                processor.StartProcessing(switches.ToArray(), stdio);
            }
        }

        void processor_Processing(object sender, GhostscriptProcessorProcessingEventArgs e)
        {
            Console.WriteLine(e.CurrentPage.ToString() + " / " + e.TotalPages.ToString());
        }

        public class LogStdio : GhostscriptStdIO
        {
            public LogStdio() : base(true, true, true) { }

            public override void StdIn(out string input, int count)
            {
                input = new string('\n', count);
            }

            public override void StdOut(string output)
            {
                // Log.Write(output);
            }

            public override void StdError(string error)
            {
                // Log.Write("Error: " + error);
            }
        }
    }
}
