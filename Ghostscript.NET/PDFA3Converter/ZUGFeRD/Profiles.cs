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
namespace Ghostscript.NET.PDFA3Converter.ZUGFeRD
{

	public class Profiles
	{
		private static readonly Dictionary<string, Profile> zf2Map = new Dictionary<string, Profile>
	{
		{"MINIMUM", new Profile("MINIMUM", "urn:factur-x.eu:1p0:minimum")},
		 {"BASICWL", new Profile("BASICWL", "urn:factur-x.eu:1p0:basicwl")},
		 {"BASIC", new Profile("BASIC", "urn:cen.eu:en16931:2017#compliant#urn:factur-x.eu:1p0:basic")},
		 {"EN16931", new Profile("EN16931", "urn:cen.eu:en16931:2017")},
		 {"EXTENDED", new Profile("EXTENDED", "urn:cen.eu:en16931:2017#conformant#urn:factur-x.eu:1p0:extended")},
		 {"XRECHNUNG", new Profile("XRECHNUNG", "urn:cen.eu:en16931:2017#compliant#urn:xoev-de:kosit:standard:xrechnung_2.1")}
	};

		private static readonly Dictionary<string, Profile> zf1Map = new Dictionary<string, Profile>
	{

		{"BASIC", new Profile("BASIC", "urn:ferd:CrossIndustryDocument:invoice:1p0:basic")},
		{"COMFORT", new Profile("COMFORT", "urn:ferd:CrossIndustryDocument:invoice:1p0:comfort")},
		{"EXTENDED", new Profile("EXTENDED", "urn:ferd:CrossIndustryDocument:invoice:1p0:extended")}
	};


		public static Profile getByName(string name, int version)
		{
			Profile result = null;
			if (version == 1)
			{
				result = zf1Map[name.ToUpper()];
			}
			else
			{
				result = zf2Map[name.ToUpper()];
			}
			if (result == null)
			{
				throw new Exception("Profile not found");
			}
			return result;
		}
		public static Profile getByName(string name)
		{
			return getByName(name, 2);
		}

	}
}