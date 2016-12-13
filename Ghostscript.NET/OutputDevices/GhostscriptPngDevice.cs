//
// GhostscriptPngDevice.cs
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

    #region GhostscriptPngDeviceType

    public enum GhostscriptPngDeviceType
    {
        /// <summary>
        /// 24bit RGB color.
        /// </summary>
        [GhostscriptSwitchValue("png16m")]
        Png16m,
        /// <summary>
        /// Transparency support.
        /// </summary>
        [GhostscriptSwitchValue("pngalpha")]
        PngAlpha,
        /// <summary>
        /// Grayscale output.
        /// </summary>
        [GhostscriptSwitchValue("pnggray")]
        PngGray,
        /// <summary>
        /// 8-bit color.
        /// </summary>
        [GhostscriptSwitchValue("png256")]
        Png256,
        /// <summary>
        /// 4-bit color.
        /// </summary>
        [GhostscriptSwitchValue("png16")]
        Png16,
        /// <summary>
        /// Black-and-white only.
        /// </summary>
        [GhostscriptSwitchValue("pngmono")]
        PngMono,
        /// <summary>
        /// Black-and-white, but the output is formed from an internal 8 bit grayscale rendering which is then error diffused and converted down to 1bpp.
        /// </summary>
        [GhostscriptSwitchValue("pngmonod")]
        PngMonoD
    }

    #endregion

    #region GhostscriptPngDeviceMinFeatureSize

    public enum GhostscriptPngDeviceMinFeatureSize
    {
        [GhostscriptSwitchValue("0")]
        V_0,
        [GhostscriptSwitchValue("1")]
        V_1,
        [GhostscriptSwitchValue("2")]
        V_2,
        [GhostscriptSwitchValue("3")]
        V_3,
        [GhostscriptSwitchValue("4")]
        V_4
    }

    #endregion

    public class GhostscriptPngDevice : GhostscriptImageDevice
    {

        #region Constructor

        public GhostscriptPngDevice() : this(GhostscriptPngDeviceType.Png16m) { }

        #endregion

        #region Constructor - deviceType

        public GhostscriptPngDevice(GhostscriptPngDeviceType deviceType)
        {
            this.Device = deviceType;
        }

        #endregion

        #region Device

        public new GhostscriptPngDeviceType Device
        {
            get
            {
                return (GhostscriptPngDeviceType)base.Device;
            }
            set
            {
                base.Device = value;
            }
        }

        #endregion

        #region DownScaleFactor

        [GhostscriptSwitch("-dDownScaleFactor={0}")]
        public int? DownScaleFactor { get; set; }

        #endregion

        #region MinFeatureSize

        [GhostscriptSwitch("-dMinFeatureSize={0}")]
        public GhostscriptPngDeviceMinFeatureSize? MinFeatureSize { get; set; }

        #endregion

        #region BackgroundColor

        [GhostscriptSwitch("-dBackgroundColor={0}")]
        public Color? BackgroundColor { get; set; }

        #endregion

        #region Process

        public static void Process(GhostscriptPngDeviceType pngDeviceType, string[] inputFiles, string outputPath, GhostscriptStdIO stdIO_callback)
        {
            GhostscriptPngDevice dev = new GhostscriptPngDevice(pngDeviceType);
            dev.InputFiles.AddRange(inputFiles);
            dev.OutputPath = outputPath;
            dev.Process(stdIO_callback);
        }

        #endregion

    }

}