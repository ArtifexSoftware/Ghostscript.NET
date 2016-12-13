//
// GetInkCoverageSample.cs
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

namespace Ghostscript.NET.Samples
{
    public class GetInkCoverageSample : ISample
    {
        public void Start()
        {
            // sample #1
            CheckAllPagesInkCoverage();

            // sample #2
            CheckInkCoverageForPagesThreeToSix();
        }

        private void CheckAllPagesInkCoverage()
        {
            string inputFile = @"E:\gss_test\mixed_test.pdf";

            Dictionary<int, GhostscriptPageInkCoverage> pages = GhostscriptPdfInfo.GetInkCoverage(inputFile);

            string outputTextTemplate = "Page {0} ink coverage -> C:{1}% / M:{2}% / Y:{3}% / K:{4}%";

            foreach (KeyValuePair<int, GhostscriptPageInkCoverage> kvp in pages)
            {
                GhostscriptPageInkCoverage pic = kvp.Value;

                Console.WriteLine(outputTextTemplate, pic.Page, pages[pic.Page].C, pages[pic.Page].M, pages[pic.Page].Y, pages[pic.Page].K);
            }
        }

        private void CheckInkCoverageForPagesThreeToSix()
        {
            string inputFile = @"E:\gss_test\mixed_test.pdf";
            Dictionary<int, GhostscriptPageInkCoverage> pages = GhostscriptPdfInfo.GetInkCoverage(inputFile, 3, 6);

            string outputTextTemplate = "Page {0} ink coverage -> C:{1}% / M:{2}% / Y:{3}% / K:{4}%";

            Console.WriteLine(outputTextTemplate, 3, pages[3].C, pages[3].M, pages[3].Y, pages[3].K);
            Console.WriteLine(outputTextTemplate, 4, pages[4].C, pages[4].M, pages[4].Y, pages[4].K);
            Console.WriteLine(outputTextTemplate, 5, pages[5].C, pages[5].M, pages[5].Y, pages[5].K);
            Console.WriteLine(outputTextTemplate, 6, pages[6].C, pages[6].M, pages[6].Y, pages[6].K);
        }
    }
}
