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
namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{

	public class Profile
	{
		protected internal string name, id;

		/// <summary>
		///*
		/// Contruct </summary>
		/// <param name="name"> human readable name of the profile, also used as basis to detemine the XMP Name </param>
		/// <param name="ID"> XML Guideline ID </param>
		public Profile(string name, string ID)
		{
			this.name = name;
			this.id = ID;
		}

		/// <summary>
		///*
		/// gets the name </summary>
		/// <returns> the name of the profile </returns>
		public virtual string getName()
		{
			return name;
		}

		/// <summary>
		///*
		/// get guideline id </summary>
		/// <returns> the XML Guideline ID of the profile </returns>
		public virtual string getID()
		{
			return id;
		}

		/// <summary>
		///*
		/// if the profile is embedded in PDF we need RDF metadata </summary>
		/// <returns> the XMP name string of the profile </returns>
		public virtual string getXMPName()
		{
			if (name.Equals("BASICWL"))
			{
				return "BASIC WL";
			}
			else if (name.Equals("EN16931"))
			{
				return "EN 16931";
			}
			else
			{
				return name;
			}
		}
	}
}