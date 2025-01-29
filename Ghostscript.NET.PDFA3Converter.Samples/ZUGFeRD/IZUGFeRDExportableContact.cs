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

	public interface IZUGFeRDExportableContact
	{

		/// <summary>
		/// customer identification assigned by the seller
		/// </summary>
		/// <returns> customer identification </returns>
		string getID()
		{
			return null;
		}


		/// <summary>
		/// First and last name of the recipient
		/// </summary>
		/// <returns> First and last name of the recipient </returns>
		string? getName()
		{
			return null;
		}

		string getPhone()
		{
			return null;
		}

		string getEMail()
		{
			return null;
		}

		string getFax()
		{
			return null;
		}


		/// <summary>
		/// Postal code of the recipient
		/// </summary>
		/// <returns> Postal code of the recipient </returns>
		string getZIP()
		{
			return null;
		}


		/// <summary>
		/// VAT ID (Umsatzsteueridentifikationsnummer) of the contact
		/// </summary>
		/// <returns> VAT ID (Umsatzsteueridentifikationsnummer) of the contact </returns>
		string getVATID()
		{
			return null;
		}


		/// <summary>
		/// two-letter country code of the contact
		/// </summary>
		/// <returns> two-letter iso country code of the contact </returns>
		string getCountry()
		{
			return null;
		}


		/// <summary>
		/// Returns the city of the contact
		/// </summary>
		/// <returns> Returns the city of the recipient </returns>
		string getLocation()
		{
			return null;
		}


		/// <summary>
		/// Returns the street address (street+number) of the contact
		/// </summary>
		/// <returns> street address (street+number) of the contact </returns>
		string getStreet()
		{
			return null;
		}

		/// <summary>
		/// returns additional address information which is display in xml tag "LineTwo"
		/// </summary>
		/// <returns> additional address information </returns>
		string getAdditionalAddress()
		{
			return null;
		}

	}
}