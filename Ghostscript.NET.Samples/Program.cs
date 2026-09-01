//
// Program.cs
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

using Ghostscript.NET;
using Ghostscript.NET.Samples;
using System;
using System.Collections.Generic;
using System.IO;

Console.WriteLine("Ghostscript.NET Samples");

if (!GhostscriptVersionInfo.IsGhostscriptInstalled)
{
    throw new Exception("Ghostscript was not found. Install Ghostscript or reference Ghostscript.NativeAssets.");
}

Console.WriteLine("Using Ghostscript: " + GhostscriptVersionInfo.GetPreferredVersion().DllPath);

List<ISample> samples = new()
{
    new GetInkCoverageSample(),
    new ProcessorSample1(),
    new ProcessorSample2(),
    new FindInstalledGhostscriptVersionsSample(),
    new RunMultipleInstancesSample(),
    new ViewerSample(),
    new RasterizerSample1(),
    new RasterizerSample2(),
    new AddWatermarkSample(),
    new DeviceUsageSample(),
    new PipedOutputSample(),
    new SendToPrinterSample(),
    new UnicodeTestSample(),
    new OfficeSupportSample()
};

string outputDir = SampleFiles.OutputDirectory;
Directory.CreateDirectory(outputDir);
Directory.CreateDirectory("Output");

int failed = 0;
foreach (ISample sample in samples)
{
    string name = sample.GetType().Name;
    Console.WriteLine();
    Console.WriteLine("--- " + name + " ---");
    try
    {
        sample.Start();
        Console.WriteLine("Sample '" + name + "' completed.");
    }
    catch (Exception ex)
    {
        failed++;
        Console.WriteLine("Sample '" + name + "' failed: " + ex.Message);
    }
}

Console.WriteLine();
Console.WriteLine(failed == 0
    ? "All samples completed. Exiting."
    : failed + " sample(s) failed. Exiting.");
Environment.Exit(failed == 0 ? 0 : 1);
