//
// GhostscriptDevice.cs
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
using System.Collections.Generic;
using System.Reflection;
using System.Drawing;
using Ghostscript.NET.Processor;

namespace Ghostscript.NET
{

    #region GhostscriptOptionalSwitch

    public enum GhostscriptOptionalSwitch
    {
        Include,
        Omit
    }

    #endregion

    #region GhostscriptBooleanSwitch

    public enum GhostscriptBooleanSwitch
    {
        [GhostscriptSwitchValue("true")]
        True,
        [GhostscriptSwitchValue("false")]
        False
    }

    #endregion

    public class GhostscriptDevice
    {

        #region Private variables

        private object _device;

        #endregion

        #region Constructor

        public GhostscriptDevice()
        {
            this.CustomSwitches = new List<string>();
            this.InputFiles = new List<string>();
            this.Page = new GhostscriptDevicePageSwitches();
            this.Pdf = new GhostscriptDevicePdfSwitches();
            this.Interaction = new GhostscriptDeviceInteractionSwitches();
            this.Other = new GhostscriptDeviceOtherSwitches();
        }

        #endregion

        #region Device

        [GhostscriptSwitch("-sDEVICE={0}")]
        public virtual object Device
        {
            get { return _device; }
            set { _device = value; }
        }

        #endregion

        #region PostScript

        public string PostScript { get; set; }
        
        #endregion

        #region InputFiles

        public List<string> InputFiles { get; internal set; }

        #endregion

        #region CustomSwitches

        public List<string> CustomSwitches { get; internal set; }

        #endregion

        #region OutputPath

        public string OutputPath { get; set; }

        #endregion

        #region Page

        public GhostscriptDevicePageSwitches Page { get; internal set; }

        #endregion

        #region Pdf

        public GhostscriptDevicePdfSwitches Pdf { get; internal set; }

        #endregion

        #region Interaction

        public GhostscriptDeviceInteractionSwitches Interaction { get; internal set; }

        #endregion

        #region Other

        public GhostscriptDeviceOtherSwitches Other { get; internal set; }

        #endregion

        #region GetSwitches

        public string[] GetSwitches()
        {
            List<string> parameters = new List<string>();
            parameters.Add("-ghostscript.net");

            List<object> switchesHolders = new List<object>();
            switchesHolders.Add(this);
            switchesHolders.Add(this.Page);
            switchesHolders.Add(this.Pdf);
            switchesHolders.Add(this.Interaction);
            switchesHolders.Add(this.Other);

            foreach (object switchesHolder in switchesHolders)
            {
                PropertyInfo[] properties = switchesHolder.GetType().GetProperties();
                foreach (PropertyInfo pi in properties)
                {
                    object[] attributes = pi.GetCustomAttributes(typeof(GhostscriptSwitchAttribute), true);

                    if (attributes.Length > 0)
                    {
                        GhostscriptSwitchAttribute attribute = attributes[0] as GhostscriptSwitchAttribute;
                        string switchName = attribute.Name;

                        object value = pi.GetValue(switchesHolder, null);

                        if (value != null)
                        {
                            Type valueType = value.GetType();

                            if (valueType == typeof(GhostscriptOptionalSwitch))
                            {
                                GhostscriptOptionalSwitch optionalSwitch = (GhostscriptOptionalSwitch)value;

                                if (optionalSwitch == GhostscriptOptionalSwitch.Include)
                                {
                                    parameters.Add(switchName);
                                }
                            }
                            else if (valueType.IsEnum)
                            {
                                object[] enumAttributes = valueType.GetMember(value.ToString())[0].GetCustomAttributes(true);

                                if (enumAttributes.Length > 0)
                                {
                                    GhostscriptSwitchValueAttribute valueAttribute = enumAttributes[0] as GhostscriptSwitchValueAttribute;

                                    parameters.Add(string.Format(switchName, valueAttribute.Value));
                                }
                            }
                            else if (valueType == typeof(Color))
                            {
                                Color color = (Color)value;

                                string hexColor = "16#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");

                                parameters.Add(string.Format(switchName, hexColor));
                            }
                            else if (valueType == typeof(GhostscriptImageDeviceResolution))
                            {
                                GhostscriptImageDeviceResolution res = value as GhostscriptImageDeviceResolution;
                                parameters.Add(string.Format(switchName, res.X, res.Y));
                            }
                            else
                            {
                                parameters.Add(string.Format(switchName, value));
                            }
                        }
                    }
                }

            }

            foreach (string customSwitch in this.CustomSwitches)
            {
                parameters.Add(customSwitch);
            }

            parameters.Add("-sOutputFile=" + this.OutputPath);
            if (!string.IsNullOrWhiteSpace(this.PostScript))
            {
                parameters.Add("-c");
                parameters.Add(this.PostScript);
            }

            parameters.Add("-f");

            foreach (string inputFile in this.InputFiles)
            {
                parameters.Add(inputFile);
            }

            return parameters.ToArray();
        }

        #endregion

        #region Process

        public void Process()
        {
            this.Process(null);
        }

        #endregion

        #region Process - stdIO_callback

        public void Process(GhostscriptStdIO stdIO_callback)
        {
            this.Process(GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL), 
                         true,
                         stdIO_callback); 
        }

        #endregion

        #region Process - ghostscriptVersion, fromMemory, stdIO_callback

        public void Process(GhostscriptVersionInfo ghostscriptVersion, bool fromMemory, GhostscriptStdIO stdIO_callback)
        {
            if (ghostscriptVersion == null)
            {
                throw new ArgumentNullException("ghostscriptVersion");
            }

            using (GhostscriptProcessor processor = new GhostscriptProcessor(ghostscriptVersion, fromMemory))
            {
                processor.StartProcessing(this.GetSwitches(), stdIO_callback);
            }
        }

        #endregion

    }
}
