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
namespace Ghostscript.NET.PDFA3Converter.ZUGFeRD
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
		string getBuyerOrderReferencedDocumentLineID();


		/// <summary>
		/// The price of one item excl. taxes
		/// </summary>
		/// <returns> The price of one item excl. taxes </returns>
		decimal getPrice();

		decimal getValue();
		/// <summary>
		/// how many get billed
		/// </summary>
		/// <returns> the quantity of the item </returns>
		decimal getQuantity();

		/// <summary>
		/// how many items units per price
		/// </summary>
		/// <returns> item units per price </returns>
		decimal getBasisQuantity();

		/// <summary>
		///*
		/// the ID of an additionally referenced document for this item </summary>
		/// <returns> the id as string </returns>
		string getAdditionalReferencedDocumentID();


        /*/// <summary>
        ///*
        /// allows to specify multiple(!) referenced documents along with e.g. their typecodes
        /// @return
        /// </summary>
        IReferencedDocument[] ReferencedDocuments
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
        string[] getNotes();



        /// <summary>
        ///*
        /// specifies the item level delivery period (there is also one on document level),
        /// this will be included in a BillingSpecifiedPeriod element </summary>
        /// <returns> the beginning of the delivery period </returns>
        DateTime getDetailedDeliveryPeriodFrom();

        /// <summary>
        ///*
        /// specifies the item level delivery period (there is also one on document level),
        /// this will be included in a BillingSpecifiedPeriod element </summary>
        /// <returns> the end of the delivery period </returns>
        DateTime getDetailedDeliveryPeriodTo();

	}

    public class ZUGFeRDExportableItem : IZUGFeRDExportableItem
    {
        public IZUGFeRDExportableProduct getProduct()
        {
            return null;
        }

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
        public string getBuyerOrderReferencedDocumentLineID()
        {

            return null;

        }


        /// <summary>
        /// The price of one item excl. taxes
        /// </summary>
        /// <returns> The price of one item excl. taxes </returns>
        public decimal getPrice()
        {
            return decimal.MinValue;
        }

        public decimal getValue()
        {

            return getPrice();

        }
        /// <summary>
        /// how many get billed
        /// </summary>
        /// <returns> the quantity of the item </returns>
        public decimal getQuantity()
        {
            return decimal.MinValue;
        }

        /// <summary>
        /// how many items units per price
        /// </summary>
        /// <returns> item units per price </returns>
        public decimal getBasisQuantity()
        {
            return 1m;
        }

        /// <summary>
        ///*
        /// the ID of an additionally referenced document for this item </summary>
        /// <returns> the id as string </returns>
        public string getAdditionalReferencedDocumentID()
        {
            return null;
        }


        /*/// <summary>
        ///*
        /// allows to specify multiple(!) referenced documents along with e.g. their typecodes
        /// @return
        /// </summary>
        IReferencedDocument[] ReferencedDocuments
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
        public string[] getNotes()
        {

            return null;

        }



        /// <summary>
        ///*
        /// specifies the item level delivery period (there is also one on document level),
        /// this will be included in a BillingSpecifiedPeriod element </summary>
        /// <returns> the beginning of the delivery period </returns>
        public DateTime getDetailedDeliveryPeriodFrom()
        {

            return DateTime.MinValue;

        }

        /// <summary>
        ///*
        /// specifies the item level delivery period (there is also one on document level),
        /// this will be included in a BillingSpecifiedPeriod element </summary>
        /// <returns> the end of the delivery period </returns>
        public DateTime getDetailedDeliveryPeriodTo()
        {
            return DateTime.MinValue;

        }

    }
}