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
using System.IO;
using System.Drawing;

namespace Ghostscript.NET.Samples
{
    public class ProcessorSample1 : ISample
    {
        public void Start()
        {
            string inputFile = @"E:\gss_test\test.pdf";
            string outputFile = @"E:\gss_test\output\page-%03d.png";

            int pageFrom = 1;
            int pageTo = 50;

            using (GhostscriptProcessor ghostscript = new GhostscriptProcessor())
            {
                ghostscript.Processing += new GhostscriptProcessorProcessingEventHandler(ghostscript_Processing);

                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dSAFER");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOPROMPT");
                switches.Add("-dFirstPage=" + pageFrom.ToString());
                switches.Add("-dLastPage=" + pageTo.ToString());
                switches.Add("-sDEVICE=png16m");
                switches.Add("-r96");
                switches.Add("-dTextAlphaBits=4");
                switches.Add("-dGraphicsAlphaBits=4");
                switches.Add(@"-sOutputFile=" + outputFile);
                switches.Add(@"-f");
                switches.Add(inputFile);

                ghostscript.Process(switches.ToArray());
            }
        }

        void ghostscript_Processing(object sender, GhostscriptProcessorProcessingEventArgs e)
        {
            Console.WriteLine(e.CurrentPage.ToString() + " / " + e.TotalPages.ToString());
        }

        private void Start2()
        {
            string inputFile = @"E:\__test_data\i1.pdf";

            GhostscriptPipedOutput gsPipedOutput = new GhostscriptPipedOutput();

            string outputPipeHandle = "%handle%" + int.Parse(gsPipedOutput.ClientHandle).ToString("X2");

            using (GhostscriptProcessor processor = new GhostscriptProcessor())
            {
                //"C:\Program Files\gs\gs9.15\bin\gswin64.exe" -sDEVICE=tiff24nc -r300 -dNOPAUSE -dBATCH -sOutputFile="Invoice 1_%03ld.tiff" "Invoice 1.pdf"
            
                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dQUIET");
                switches.Add("-dSAFER");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOPROMPT");
                switches.Add("-dPrinted");
                //switches.Add("-sDEVICE=pdfwrite");
                switches.Add("-sDEVICE=tiff24nc");
                switches.Add("-sOutputFile=" + outputPipeHandle);
                switches.Add("-f");
                switches.Add(inputFile);

                try
                {
                    processor.Process(switches.ToArray());

                    byte[] rawDocumentData = gsPipedOutput.Data;
                    var memStream = new MemoryStream(rawDocumentData);
                    //var image = new Bitmap(memStream);
                    //image.Save(@"Invocie 1.tiff");
                    //if (writeToDatabase)
                    //{
                    //    Database.ExecSP("add_document", rawDocumentData);
                    //}
                    //else if (writeToDisk)
                    //{
                    //    File.WriteAllBytes(@"E:\gss_test\output\test_piped_output.pdf", rawDocumentData);
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    gsPipedOutput.Dispose();
                    gsPipedOutput = null;
                }
            }
        }
    }
}
