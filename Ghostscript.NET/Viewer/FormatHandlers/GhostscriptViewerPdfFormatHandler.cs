//
// GhostscriptViewerPdfFormatHandler.cs
// This file is part of Ghostscript.NET library
//
// Author: Josip Habjan (habjan@gmail.com, http://www.linkedin.com/in/habjan) 
// Copyright (c) 2013-2015 by Josip Habjan. All rights reserved.
//
// Author ported some parts of this code from GSView. 
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
using System.Drawing;

namespace Ghostscript.NET.Viewer
{
    internal class GhostscriptViewerPdfFormatHandler : GhostscriptViewerFormatHandler
    {
        #region Private constants

        private const string PDF_TAG = "%GSNET";
        private const string PDF_PAGES_TAG = "%GSNET_VIEWER_PDF_PAGES: ";
        private const string PDF_PAGE_TAG = "%GSNET_VIEWER_PDF_PAGE: ";
        private const string PDF_MEDIA_TAG = "%GSNET_VIEWER_PDF_MEDIA: ";
        private const string PDF_CROP_TAG = "%GSNET_VIEWER_PDF_CROP: ";
        private const string PDF_ROTATE_TAG = "%GSNET_VIEWER_PDF_ROTATE: ";
        private const string PDF_DONE_TAG = "%GSNET_VIEWER_PDF_DONE: ";
        private const string PDF_MARK_TAG = "%GSNET_VIEWER_PDF_MARK: ";

        #endregion

        #region Constructor

        public GhostscriptViewerPdfFormatHandler(GhostscriptViewer viewer) : base(viewer) { }

        #endregion

        #region Initialize

        public override void Initialize()
        {
            // define our routine for preparing to show a page.  
            // This writes out some tags which we capture in the 
            // callback to obtain the page size and orientation. 
            this.Execute(string.Format(@"
                /GSNETViewer_PDFpage {{ 
        	        ({0}) print dup == flush 
        	        pdfgetpage /Page exch store 
                    Page /MediaBox pget 
                    {{ ({1}) print == flush  }} 
        	        if 
                    Page /CropBox pget 
         	        {{ ({2}) print == flush }} 
        	        if 
                    Page /Rotate pget not {{ 0 }} if 
        	        ({3}) print == flush 
                }} def", PDF_PAGE_TAG, PDF_MEDIA_TAG, PDF_CROP_TAG, PDF_ROTATE_TAG));

            // put these in userdict so we can write to them later
            this.Execute(@"
                    /Page null def
                    /Page# 0 def
                    /PDFSave null def
                    /DSCPageCount 0 def
                ");

            // open PDF support dictionaries
            this.Execute(@"
                    GS_PDF_ProcSet begin
                    pdfdict begin");
        }

        #endregion

        #region Open

        public override void Open(string filePath)
        {           
            // open PDF file
            this.Execute(string.Format("({0}) (r) file runpdfbegin", filePath.Replace("\\", "/")));
            
            this.Execute("/FirstPage where { pop FirstPage } { 1 } ifelse");
            this.Execute("/LastPage where { pop LastPage } { pdfpagecount } ifelse");

            // flush stdout and then send PDF page marker to stdout where we capture the page numbers via callback
            this.Execute(string.Format("flush ({0}) print exch =only ( ) print =only (\n) print flush", PDF_PAGES_TAG));

            // fixes problem with the invisible layers
            // if we don't run that code, then optional content groups will be left unmarked and always processed
            this.Execute("process_trailer_attrs\n");
        }

        #endregion

        #region StdInput

        public override void StdInput(out string input, int count)
        {
            input = string.Empty;
        }

        #endregion

        #region StdOutput

        public override void StdOutput(string message)
        {
            if (message.Contains(PDF_TAG))
            {
                int startPos = message.IndexOf(PDF_TAG);
                int endPos = message.IndexOf(": ");

                string tag = message.Substring(startPos, endPos - startPos + 2);
                string rest = message.Substring(endPos + 2, message.Length - endPos - 2);

                switch (tag)
                {
                    case PDF_PAGES_TAG:
                        {
                            string[] pages = rest.Split(new char[] { ' ' });
                            this.FirstPageNumber = int.Parse(pages[0]);
                            this.LastPageNumber = int.Parse(pages[1]);
                        }
                        break;
                    case PDF_PAGE_TAG:
                        {
                            this.CurrentPageNumber = int.Parse(rest);
                            break;
                        }
                    case PDF_MEDIA_TAG:
                        {
                            string[] mb = rest.Split(new char[] { ' ' });
                            this.MediaBox = new GhostscriptRectangle(
                                    float.Parse(mb[0].TrimStart('['), System.Globalization.CultureInfo.InvariantCulture), 
                                    float.Parse(mb[1], System.Globalization.CultureInfo.InvariantCulture), 
                                    float.Parse(mb[2], System.Globalization.CultureInfo.InvariantCulture), 
                                    float.Parse(mb[3].TrimEnd(']'), System.Globalization.CultureInfo.InvariantCulture));
                            break;
                        }
                    case PDF_CROP_TAG:
                        {
                            string[] cb = rest.Split(new char[] { ' ' });
                            this.CropBox = new GhostscriptRectangle(
                                    float.Parse(cb[0].TrimStart('['), System.Globalization.CultureInfo.InvariantCulture),
                                    float.Parse(cb[1], System.Globalization.CultureInfo.InvariantCulture),
                                    float.Parse(cb[2], System.Globalization.CultureInfo.InvariantCulture),
                                    float.Parse(cb[3].TrimEnd(']'), System.Globalization.CultureInfo.InvariantCulture));

                            break;
                        }
                    case PDF_ROTATE_TAG:
                        {
                            int rotate = int.Parse(rest);

                            while (rotate < 0)
                            {
                                rotate += 360;
                            }

                            while (rotate >= 360)
                            {
                                rotate -= 360;
                            }
                                
                            switch (rotate)
                            {
                                case 90:
                                    this.PageOrientation = GhostscriptPageOrientation.Landscape;
                                    break;
                                case 180:
                                    this.PageOrientation = GhostscriptPageOrientation.UpsideDown;
                                    break;
                                case 270:
                                    this.PageOrientation = GhostscriptPageOrientation.Seascape;
                                    break;
                                default:
                                    this.PageOrientation = GhostscriptPageOrientation.Portrait;
                                    break;
                            }

                            break;
                        }
                }
            }
        }

        #endregion

        #region StdError

        public override void StdError(string message)
        {

        }

        #endregion

        #region InitPage

        public override void InitPage(int pageNumber)
        {
            if (pageNumber >= this.FirstPageNumber && pageNumber <= this.LastPageNumber)
            {
                this.Execute(string.Format("{0} GSNETViewer_PDFpage", pageNumber));
            }
            else
            {
                throw new GhostscriptException("Page number is not in pages number range!");
            }
        }

        #endregion

        #region ShowPage

        public override void ShowPage(int pageNumber)
        {
            if (pageNumber >= this.FirstPageNumber && pageNumber <= this.LastPageNumber)
            {
                this.Execute("Page pdfshowpage_init pdfshowpage_finish");
            }
            else
            {
                throw new GhostscriptException("Page number is not in pages number range!");
            }
        }

        #endregion

    }
}
