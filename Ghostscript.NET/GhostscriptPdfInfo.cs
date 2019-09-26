//
// GhostscriptPdfInfo.cs
// This file is part of Ghostscript.NET library
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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Ghostscript.NET.Processor;

namespace Ghostscript.NET
{

    /// <summary>
    /// Class that helps us to get various information about the PDF file.
    /// </summary>
    public class GhostscriptPdfInfo
    {

        #region GetInkCoverage - stream

        /// <summary>
        /// Returns Ink coverage for all pages.
        /// The result is ink coverage for the CMYK inks, separately for each single page (for RGB colors, it does a silent conversion to CMYK color space internally).
        /// This function is supported only in Ghostscript v9.05 or newer.
        /// </summary>
        /// <param name="stream">Stream representing PDF document.</param>
        /// <returns>A dictionary of a page numbers with the ink coverage.</returns>
        public static Dictionary<int, GhostscriptPageInkCoverage> GetInkCoverage(Stream stream)
        {
            GhostscriptVersionInfo gvi = GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL);
            return GetInkCoverage(stream, 0, 0, gvi);
        }

        #endregion

        #region GetInkCoverage - stream, versionInfo

        /// <summary>
        /// Returns Ink coverage for all pages.
        /// The result is ink coverage for the CMYK inks, separately for each single page (for RGB colors, it does a silent conversion to CMYK color space internally).
        /// This function is supported only in Ghostscript v9.05 or newer.
        /// </summary>
        /// <param name="stream">Stream representing PDF document.</param>
        /// <param name="versionInfo">GhostscriptVersionInfo instance that tells which Ghostscript library to use.</param>
        /// <returns>A dictionary of a page numbers with the ink coverage.</returns>
        public static Dictionary<int, GhostscriptPageInkCoverage> GetInkCoverage(Stream stream, GhostscriptVersionInfo versionInfo)
        {
            GhostscriptVersionInfo gvi = GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL);
            return GetInkCoverage(stream, 0, 0, versionInfo);
        }

        #endregion

        #region GetInkCoverage - stream, firstPage, lastPage

        /// <summary>
        /// Returns Ink coverage for specified page range.
        /// The result is ink coverage for the CMYK inks, separately for each single page (for RGB colors, it does a silent conversion to CMYK color space internally).
        /// This function is supported only in Ghostscript v9.05 or newer.
        /// </summary>
        /// <param name="stream">Stream representing PDF document.</param>
        /// <param name="firstPage">Designated start page of the document. Pages of all documents in PDF collections are numbered sequentionally.</param>
        /// <param name="lastPage">Designated end page of the document. Pages of all documents in PDF collections are numbered sequentionally.</param>
        /// <returns>Dictionary of page numbers with ink coverage.</returns>
        public static Dictionary<int, GhostscriptPageInkCoverage> GetInkCoverage(Stream stream, int firstPage, int lastPage)
        {
            GhostscriptVersionInfo gvi = GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL);
            return GetInkCoverage(stream, firstPage, lastPage, gvi);
        }

        #endregion

        #region GetInkCoverage - stream, firstPage, lastPage, versionInfo

        /// <summary>
        /// Returns Ink coverage for specified page range.
        /// The result is ink coverage for the CMYK inks, separately for each single page (for RGB colors, it does a silent conversion to CMYK color space internally).
        /// This function is supported only in Ghostscript v9.05 or newer.
        /// </summary>
        /// <param name="stream">Stream representing PDF document.</param>
        /// <param name="firstPage">Designated start page of the document. Pages of all documents in PDF collections are numbered sequentionally.</param>
        /// <param name="lastPage">Designated end page of the document. Pages of all documents in PDF collections are numbered sequentionally.</param>
        /// <param name="versionInfo">GhostscriptVersionInfo instance that tells which Ghostscript library to use.</param>
        /// <returns>A dictionary of a page numbers with the ink coverage.</returns>
        public static Dictionary<int, GhostscriptPageInkCoverage> GetInkCoverage(Stream stream, int firstPage, int lastPage, GhostscriptVersionInfo versionInfo)
        {
            FileCleanupHelper cleanupHelper = new FileCleanupHelper();

            try
            {
                string path = StreamHelper.WriteToTemporaryFile(stream);
                cleanupHelper.Add(path);

                return GetInkCoverage(path, firstPage, lastPage, versionInfo);
            }
            finally
            {
                cleanupHelper.Cleanup();
            }
        }

        #endregion
        
        #region GetInkCoverage - path

        /// <summary>
        /// Returns Ink coverage for all pages.
        /// The result is ink coverage for the CMYK inks, separately for each single page (for RGB colors, it does a silent conversion to CMYK color space internally).
        /// This function is supported only in Ghostscript v9.05 or newer.
        /// </summary>
        /// <param name="path">PDF file path.</param>
        /// <returns>A dictionary of a page numbers with the ink coverage.</returns>
        public static Dictionary<int, GhostscriptPageInkCoverage> GetInkCoverage(string path)
        {
            GhostscriptVersionInfo gvi = GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL);
            return GetInkCoverage(path, 0, 0, gvi);
        }

        #endregion

        #region GetInkCoverage - path, versionInfo

        /// <summary>
        /// Returns Ink coverage for all pages.
        /// The result is ink coverage for the CMYK inks, separately for each single page (for RGB colors, it does a silent conversion to CMYK color space internally).
        /// This function is supported only in Ghostscript v9.05 or newer.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="versionInfo">GhostscriptVersionInfo instance that tells which Ghostscript library to use.</param>
        /// <returns>A dictionary of a page numbers with the ink coverage.</returns>
        public static Dictionary<int, GhostscriptPageInkCoverage> GetInkCoverage(string path, GhostscriptVersionInfo versionInfo)
        {
            GhostscriptVersionInfo gvi = GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL);
            return GetInkCoverage(path, 0, 0, versionInfo);
        }

        #endregion

        #region GetInkCoverage - path, firstPage, lastPage

        /// <summary>
        /// Returns Ink coverage for specified page range.
        /// The result is ink coverage for the CMYK inks, separately for each single page (for RGB colors, it does a silent conversion to CMYK color space internally).
        /// This function is supported only in Ghostscript v9.05 or newer.
        /// </summary>
        /// <param name="path">PDF file path.</param>
        /// <param name="firstPage">Designated start page of the document. Pages of all documents in PDF collections are numbered sequentionally.</param>
        /// <param name="lastPage">Designated end page of the document. Pages of all documents in PDF collections are numbered sequentionally.</param>
        /// <returns>A dictionary of a page numbers with the ink coverage.</returns>
        public static Dictionary<int, GhostscriptPageInkCoverage> GetInkCoverage(string path, int firstPage, int lastPage)
        {
            GhostscriptVersionInfo gvi = GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL);
            return GetInkCoverage(path, firstPage, lastPage, gvi);
        }

        #endregion

        #region GetInkCoverage - path, firstPage, lastPage, versionInfo

        /// <summary>
        /// Returns Ink coverage for specified page range.
        /// The result is ink coverage for the CMYK inks, separately for each single page (for RGB colors, it does a silent conversion to CMYK color space internally).
        /// This function is supported only in Ghostscript v9.05 or newer.
        /// </summary>
        /// <param name="path">PDF file path.</param>
        /// <param name="firstPage">Designated start page of the document. Pages of all documents in PDF collections are numbered sequentionally.</param>
        /// <param name="lastPage">Designated end page of the document. Pages of all documents in PDF collections are numbered sequentionally.</param>
        /// <param name="versionInfo">GhostscriptVersionInfo instance that tells which Ghostscript library to use.</param>
        /// <returns>A dictionary of a page numbers with the ink coverage.</returns>
        public static Dictionary<int, GhostscriptPageInkCoverage> GetInkCoverage(string path, int firstPage, int lastPage, GhostscriptVersionInfo versionInfo)
        {
            GhostscriptPipedOutput gsPipedOutput = new GhostscriptPipedOutput();
            string outputPipeHandle = "%handle%" + int.Parse(gsPipedOutput.ClientHandle).ToString("X2");

            List<string> switches = new List<string>();
            switches.Add("-empty");
            switches.Add("-q");

            if (firstPage != 0 && lastPage != 0)
            {
                switches.Add("-dFirstPage=" + firstPage.ToString());
                switches.Add("-dLastPage=" + lastPage.ToString());
            }

            switches.Add("-o" + outputPipeHandle);
            switches.Add("-sDEVICE=inkcov");
            switches.Add(path);

            GhostscriptProcessor proc = new GhostscriptProcessor(versionInfo, false);
            proc.StartProcessing(switches.ToArray(), null);

            byte[] data = gsPipedOutput.Data;

            gsPipedOutput.Dispose(); gsPipedOutput = null;

            string output = Encoding.ASCII.GetString(data);

            if (output.Length > 0)
            {
                Dictionary<int, GhostscriptPageInkCoverage> result = new Dictionary<int, GhostscriptPageInkCoverage>();

                string[] outputLines = output.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                int pageNumber = firstPage == 0 ? 1 : firstPage;

                foreach(string line in outputLines)
                {
                    GhostscriptPageInkCoverage pic = new GhostscriptPageInkCoverage();
                    pic.Page = pageNumber;
                    pic.IsValid = false;

                    string[] lineParts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (lineParts.Length == 6)
                    {
                        pic.C = Math.Round(float.Parse(lineParts[0], CultureInfo.InvariantCulture) * 100, 4);
                        pic.M = Math.Round(float.Parse(lineParts[1], CultureInfo.InvariantCulture) * 100, 4);
                        pic.Y = Math.Round(float.Parse(lineParts[2], CultureInfo.InvariantCulture) * 100, 4);
                        pic.K = Math.Round(float.Parse(lineParts[3], CultureInfo.InvariantCulture) * 100, 4);

                        if (lineParts[5] == "OK")
                        {
                            pic.IsValid = true;
                        }
                    }

                    result.Add(pageNumber, pic);

                    pageNumber++;
                }

                return result;
            }
            else
            {
                return null; 
            }
        }

        #endregion

    }

    #region GhostscriptPageInkCoverage

    /// <summary>
    /// Ink coverage.
    /// </summary>
    public class GhostscriptPageInkCoverage
    {
        /// <summary>
        /// Gets page number.
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Gets percentage of Cyan color coverage.
        /// </summary>
        public double C { get; set; }
        /// <summary>
        /// Gets percentage of Magenta color coverage.
        /// </summary>
        public double M { get; set; }
        /// <summary>
        /// Gets percentage of Yellow color coverage.
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Gets percentage of Black color coverage.
        /// </summary>
        public double K { get; set; }
        /// <summary>
        /// Gets if the ink coverage values are valid.
        /// </summary>
        public bool IsValid { get; set; }
    }

    #endregion

}
