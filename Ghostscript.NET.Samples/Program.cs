//
// Program.cs
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
	class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ghostscript.NET Samples");

            if (!GhostscriptVersionInfo.IsGhostscriptInstalled)
            {
                throw new Exception("You don't have Ghostscript installed on this machine!");
            }

            ISample sample;

            //sample = new GetInkCoverageSample();
            sample = new ProcessorSample1();
            //sample = new ProcessorSample2();
            //sample = new FindInstalledGhostscriptVersionsSample();
            //sample = new RunMultipleInstancesSample();
            //sample = new ViewerSample();
            //sample = new RasterizerSample1();
            //sample = new RasterizerSample2();
            //sample = new AddWatermarkSample();
            //sample = new DeviceUsageSample();
            //sample = new PipedOutputSample();
            //sample = new SendToPrinterSample();
            //sample = new RasterizerCropSample();

            sample.Start();

            Console.ReadLine();
        }
    }
}
