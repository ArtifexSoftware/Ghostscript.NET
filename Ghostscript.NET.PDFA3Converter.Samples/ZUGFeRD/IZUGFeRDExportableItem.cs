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
namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{
	public interface IZUGFeRDExportableItem
	{
		IZUGFeRDExportableProduct getProduct();

		/// <summary>
		/// item level discounts </summary>
		/// <returns> array of the discounts on a single item
		/// 
		/// IZUGFeRDAllowanceCharge[] getItemAllowances() {
		/// return null;
		/// }
		/// 
		/// **
		/// item level price additions </returns>
		/// <returns> array of the additional charges on the item
		/// 
		/// IZUGFeRDAllowanceCharge[] getItemCharges() {
		/// return null;
		/// }
		///  </returns>
		/// <summary>
		///*
		/// BT 132 (issue https://github.com/ZUGFeRD/mustangproject/issues/247)
		/// @return
		/// </summary>
		string getBuyerOrderReferencedDocumentLineID()
		{

			return null;

		}


		/// <summary>
		/// The price of one item excl. taxes
		/// </summary>
		/// <returns> The price of one item excl. taxes </returns>
		decimal getPrice();

		decimal getValue()
		{

			return getPrice();

		}
		/// <summary>
		/// how many get billed
		/// </summary>
		/// <returns> the quantity of the item </returns>
		decimal getQuantity();

		/// <summary>
		/// how many items units per price
		/// </summary>
		/// <returns> item units per price </returns>
		decimal getBasisQuantity()
		{
			return 1m;
		}

		/// <summary>
		///*
		/// the ID of an additionally referenced document for this item </summary>
		/// <returns> the id as string </returns>
		string getAdditionalReferencedDocumentID()
		{
			return null;
		}


		/// <summary>
		///*
		/// allows to specify multiple(!) referenced documents along with e.g. their typecodes
		/// @return
		/// </summary>
		/*IReferencedDocument[] ReferencedDocuments
		{
			get
			{
				return null;
			}
		}*/
		/// <summary>
		///*
		/// descriptive texts </summary>
		/// <returns> an array of strings of item specific "includedNotes", text values </returns>
		string[] getNotes()
		{

			return null;

		}



		/// <summary>
		///*
		/// specifies the item level delivery period (there is also one on document level),
		/// this will be included in a BillingSpecifiedPeriod element </summary>
		/// <returns> the beginning of the delivery period </returns>
		DateTime? getDetailedDeliveryPeriodFrom()
		{

			return null;

		}

		/// <summary>
		///*
		/// specifies the item level delivery period (there is also one on document level),
		/// this will be included in a BillingSpecifiedPeriod element </summary>
		/// <returns> the end of the delivery period </returns>
		DateTime? getDetailedDeliveryPeriodTo()
		{
			return null;

		}

	}
}