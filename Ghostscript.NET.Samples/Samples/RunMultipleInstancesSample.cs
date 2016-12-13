//
// RunMultipleInstancesSample.cs
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
using System.Threading;

namespace Ghostscript.NET.Samples
{
    public class RunMultipleInstancesSample : ISample
    {
        private GhostscriptVersionInfo _gs_verssion_info = GhostscriptVersionInfo.GetLastInstalledVersion();

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(Instance1));
            ThreadPool.QueueUserWorkItem(new WaitCallback(Instance2));
        }

        private void Process(string input, string output, int startPage, int endPage)
        {
            // make sure that constructor 'fromMemory' option is set to true if 
            // you want to run multiple instances of the Ghostscript
            Ghostscript.NET.Processor.GhostscriptProcessor processor = new Processor.GhostscriptProcessor(_gs_verssion_info, true);
            processor.StartProcessing(CreateTestArgs(input, output, startPage, endPage), new ConsoleStdIO(true, true, true));
        }

        private void Instance1(object target)
        {
            // export pdf pages to images
            Process(@"E:\mc-1.pdf", @"E:\_pdf_out\a_test-%03d.png", 1, 100);
        }

        private void Instance2(object target)
        {
            // export pdf pages to images
            Process(@"E:\mc-2.pdf", @"E:\_pdf_out\b_test-%03d.png", 1, 100);
        }

        private string[] CreateTestArgs(string inputPath, string outputPath, int pageFrom, int pageTo)
        {
            List<string> gsArgs = new List<string>();

            gsArgs.Add("-q");
            gsArgs.Add("-dSAFER");
            gsArgs.Add("-dBATCH");
            gsArgs.Add("-dNOPAUSE");
            gsArgs.Add("-dNOPROMPT");
            gsArgs.Add(@"-sFONTPATH=" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts));
            gsArgs.Add("-dFirstPage=" + pageFrom.ToString());
            gsArgs.Add("-dLastPage=" + pageTo.ToString());
            gsArgs.Add("-sDEVICE=png16m");
            gsArgs.Add("-r72");
            gsArgs.Add("-sPAPERSIZE=a4");
            gsArgs.Add("-dNumRenderingThreads=" + Environment.ProcessorCount.ToString());
            gsArgs.Add("-dTextAlphaBits=4");
            gsArgs.Add("-dGraphicsAlphaBits=4");
            gsArgs.Add(@"-sOutputFile=" + outputPath);
            gsArgs.Add(@"-f" + inputPath);

            return gsArgs.ToArray();
        }
    }
}
