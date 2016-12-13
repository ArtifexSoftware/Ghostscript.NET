//
// GhostscriptSwitchAttribute.cs
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
    /// <summary>
    /// Represents a GhostscriptSwitch attribute.
    /// </summary>
    public sealed class GhostscriptSwitchAttribute : Attribute
    {

        #region Private variables

        private string _name;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptSwitchAttribute class.
        /// </summary>
        /// <param name="name">The Switch name.</param>
        public GhostscriptSwitchAttribute(string name)
        {
            _name = name;
        }

        #endregion

        #region Name

        /// <summary>
        /// Gets the switch name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        #endregion

    }

    /// <summary>
    /// Represents a GhostscriptSwitchValue attribute.
    /// </summary>
    public sealed class GhostscriptSwitchValueAttribute : Attribute
    {

        #region Private variables

        private string _value;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptSwitchValueAttribute class.
        /// </summary>
        /// <param name="value"></param>
        public GhostscriptSwitchValueAttribute(string value)
        {
            _value = value;
        }

        #endregion

        #region Value

        /// <summary>
        /// Gets the switch value.
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        #endregion

    }
}
