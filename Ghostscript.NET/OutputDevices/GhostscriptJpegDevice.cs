//
// GhostscriptJpegDevice.cs
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
using System.Drawing;

namespace Ghostscript.NET
{

    #region GhostscriptJpegDeviceType

    public enum GhostscriptJpegDeviceType
    {
        /// <summary>
        /// Produce color JPEG files.
        /// </summary>
        [GhostscriptSwitchValue("jpeg")]
        Jpeg,
        /// <summary>
        /// Produce grayscale JPEG files.
        /// </summary>
        [GhostscriptSwitchValue("jpeggray")]
        JpegGray
    }

    #endregion

    public class GhostscriptJpegDevice : GhostscriptImageDevice
    {

        #region Constructor

        public GhostscriptJpegDevice() : this(GhostscriptJpegDeviceType.Jpeg) { }

        #endregion

        #region Constructor - deviceType

        public GhostscriptJpegDevice(GhostscriptJpegDeviceType deviceType)
        {
            this.Device = deviceType;
        }

        #endregion

        #region Device

        public new GhostscriptJpegDeviceType Device
        {
            get
            {
                return (GhostscriptJpegDeviceType)base.Device;
            }
            set
            {
                base.Device = value;
            }
        }

        #endregion

        #region JpegQuality

        /// <summary>
        /// (integer from 0 to 100, default 75)
        /// Set the quality level value according to the widely used IJG quality scale, which balances the extent of compression 
        /// against the fidelity of the image when reconstituted. Lower values drop more information from the image to achieve 
        /// higher compression, and therefore have lower quality when reconstituted.
        /// </summary>
        [GhostscriptSwitch("-dJPEGQ={0}")]
        public int? JpegQuality { get; set; }

        #endregion

        #region QualityFactor

        /// <summary>
        /// (float from 0.0 to 1.0).
        /// Adobe's QFactor quality scale, which you may use in place of JPEGQ above. The QFactor scale is used by PostScript's
        /// DCTEncode filter but is nearly unheard-of elsewhere.
        /// </summary>
        [GhostscriptSwitch("-dQFactor={0}")]
        public float? QualityFactor { get; set; }

        #endregion

        #region Process

        public static void Process(GhostscriptJpegDeviceType deviceType, string[] inputFiles, string outputPath, GhostscriptStdIO stdIO_callback)
        {
            GhostscriptJpegDevice dev = new GhostscriptJpegDevice(deviceType);
            dev.InputFiles.AddRange(inputFiles);
            dev.OutputPath = outputPath;
            dev.Process(stdIO_callback);
        }

        #endregion

    }

}