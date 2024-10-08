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
using System.Collections.Generic;
using Ghostscript.NET;
using Ghostscript.NET.Samples;

Console.WriteLine("Ghostscript.NET Samples");

if (!GhostscriptVersionInfo.IsGhostscriptInstalled)
{
    throw new Exception("You don't have Ghostscript installed on this machine!");
}

List<ISample> samples = new()
{
    //new GetInkCoverageSample(),
    //new ProcessorSample1(),
    //new ProcessorSample2(),
    //new FindInstalledGhostscriptVersionsSample(),
    //new RunMultipleInstancesSample(),
    //new ViewerSample(),
    //new RasterizerSample1(),
    //new RasterizerSample2(),
    //new AddWatermarkSample(),
    //new DeviceUsageSample(),
    //new PipedOutputSample(),
    //new SendToPrinterSample()
};

foreach (ISample sample in samples)
{
    sample.Start();
    Console.WriteLine($"Sample '{sample.GetType().Name}' run successful!");
}

Console.ReadLine();
