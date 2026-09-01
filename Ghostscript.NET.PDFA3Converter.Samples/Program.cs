// Copyright (C) 2024 Artifex Software, Inc.
//
// This file is part of Ghostscript.NET.
//
// Ghostscript.NET is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// Ghostscript.NET is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
// details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Ghostscript.NET. If not, see 
// <https://www.gnu.org/licenses/agpl-3.0.en.html>
//
// Alternative licensing terms are available from the licensor.
// For commercial licensing, see <https://www.artifex.com/> or contact
// Artifex Software, Inc., 39 Mesa Street, Suite 108A, San Francisco,
// CA 94129, USA, for further information.
using System;
using System.Collections.Generic;


namespace Ghostscript.NET.PDFA3Converter.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ghostscript.NET PDFA3Converter.Samples");

            if (!GhostscriptVersionInfo.IsGhostscriptInstalled)
            {
                throw new Exception("Ghostscript was not found. Install Ghostscript or reference Ghostscript.NativeAssets.");
            }

            Console.WriteLine("Using Ghostscript: " + SamplePaths.ResolveGhostscriptDll());

            int failed = 0;
            ISample[] samples =
            {
                new FacturXWithMustangSample(),
                new FacturXWithZUGFeRDcsharpSample()
            };

            foreach (ISample sample in samples)
            {
                string name = sample.GetType().Name;
                try
                {
                    sample.Start();
                    Console.WriteLine(name + " completed.");
                }
                catch (Exception ex)
                {
                    failed++;
                    Console.WriteLine(name + " failed: " + ex.Message);
                }
            }

            Console.WriteLine(failed == 0
                ? "PDFA3Converter samples completed."
                : failed + " sample(s) failed.");
        }
    }
}
