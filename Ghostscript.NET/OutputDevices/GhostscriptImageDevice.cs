//
// GhostscriptImageDevice.cs
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

namespace Ghostscript.NET
{

    #region GhostscriptImageDeviceAlphaBits

    public enum GhostscriptImageDeviceAlphaBits
    {
        /// <summary>
        /// 1.
        /// </summary>
        [GhostscriptSwitchValue("1")]
        V_1,
        /// <summary>
        /// 2.
        /// </summary>
        [GhostscriptSwitchValue("2")]
        V_2,
        /// <summary>
        /// 4.
        /// </summary>
        [GhostscriptSwitchValue("4")]
        V_4
    }

    #endregion

    #region GhostscriptImageDeviceResolution

    public class GhostscriptImageDeviceResolution
    {

        #region Constructor

        public GhostscriptImageDeviceResolution(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region X

        public int? X { get; set; }

        #endregion

        #region Y

        public int? Y { get; set; }

        #endregion

    }

    #endregion

    public class GhostscriptImageDevice : GhostscriptDevice
    {
        #region Constructor

        public GhostscriptImageDevice()
        {
            this.Other.Safer = GhostscriptOptionalSwitch.Include;
            this.Interaction.Batch = GhostscriptOptionalSwitch.Include;
            this.Interaction.NoPause = GhostscriptOptionalSwitch.Include;
        }

        #endregion

        #region Resolution

        /// <summary>
        /// This option sets the resolution of the output file in dots per inch. The default value if you don't specify this options is usually 72 dpi.
        /// </summary>
        [GhostscriptSwitch("-r{0}")]
        public int? Resolution { get; set; }

        #endregion

        #region ResolutionXY

        /// <summary>
        /// This option sets the resolution of the output file in dots per inch. The default value if you don't specify this options is usually 72 dpi.
        /// </summary>
        [GhostscriptSwitch("-r{0}x{1}")]
        public GhostscriptImageDeviceResolution ResolutionXY { get; set; }

        #endregion

        #region TextAlphaBits

        /// <summary>
        /// These option control the use of subsample antialiasing. 
        /// Their use is highly recommended for producing high quality rasterizations of the input files. 
        /// The size of the subsampling box n should be 4 for optimum output, but smaller values can be used for faster rendering. 
        /// Antialiasing is enabled separately for text and graphics content.
        /// </summary>
        [GhostscriptSwitch("-dTextAlphaBits={0}")]
        public GhostscriptImageDeviceAlphaBits? TextAlphaBits { get; set; }

        #endregion

        #region GraphicsAlphaBits

        /// <summary>
        /// These option control the use of subsample antialiasing. 
        /// Their use is highly recommended for producing high quality rasterizations of the input files. 
        /// The size of the subsampling box n should be 4 for optimum output, but smaller values can be used for faster rendering. 
        /// Antialiasing is enabled separately for text and graphics content.
        /// </summary>
        [GhostscriptSwitch("-dGraphicsAlphaBits={0}")]
        public GhostscriptImageDeviceAlphaBits? GraphicsAlphaBits { get; set; }

        #endregion
    }
}
