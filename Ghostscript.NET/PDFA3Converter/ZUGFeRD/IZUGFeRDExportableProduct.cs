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
namespace Ghostscript.NET.PDFA3Converter.ZUGFeRD
{
	public interface IZUGFeRDExportableProduct
	{

		/// 
		/// <summary>
		/// HUR	hour
		/// KGM	kilogram
		/// KTM	kilometre
		/// KWH	kilowatt hour
		/// LS	lump sum
		/// LTR	litre
		/// MIN	minute
		/// MMK	square millimetre
		/// MMT	millimetre
		/// MTK	square metre
		/// MTQ	cubic metre
		/// MTR	metre
		/// NAR	number of articles
		/// NPR	number of pairs
		/// P1	percent
		/// SET	set
		/// TNE	tonne (metric ton)
		/// WEE	week
		/// </summary>
		/// <returns> a UN/ECE rec 20 unit code see https://www.unece.org/fileadmin/DAM/cefact/recommendations/rec20/rec20_rev3_Annex2e.pdf </returns>
		string getUnit();

		/// <summary>
		/// Short name of the product
		/// </summary>
		/// <returns> Short name of the product </returns>
		string getName();

		/// <summary>
		/// long description of the product
		/// </summary>
		/// <returns> long description of the product </returns>
		string getDescription();

		/// <summary>
		/// Get the ID that had been assigned by the seller to
		/// identify the product
		/// </summary>
		/// <returns> seller assigned product ID </returns>
		string getSellerAssignedID();

		/// <summary>
		/// Get the ID that had been assigned by the buyer to
		/// identify the product
		/// </summary>
		/// <returns> buyer assigned product ID </returns>
		string getBuyerAssignedID();

		/// <summary>
		/// VAT percent of the product (e.g. 19, or 5.1 if you like)
		/// </summary>
		/// <returns> VAT percent of the product </returns>
		decimal getVATPercent();

		bool getIntraCommunitySupply();

		bool getReverseCharge();

		string getTaxCategoryCode();

        string getTaxExemptionReason();
	}

    public class ZUGFeRDExportableProduct : IZUGFeRDExportableProduct
    {

        /// 
        /// <summary>
        /// HUR	hour
        /// KGM	kilogram
        /// KTM	kilometre
        /// KWH	kilowatt hour
        /// LS	lump sum
        /// LTR	litre
        /// MIN	minute
        /// MMK	square millimetre
        /// MMT	millimetre
        /// MTK	square metre
        /// MTQ	cubic metre
        /// MTR	metre
        /// NAR	number of articles
        /// NPR	number of pairs
        /// P1	percent
        /// SET	set
        /// TNE	tonne (metric ton)
        /// WEE	week
        /// </summary>
        /// <returns> a UN/ECE rec 20 unit code see https://www.unece.org/fileadmin/DAM/cefact/recommendations/rec20/rec20_rev3_Annex2e.pdf </returns>
        public string getUnit()
		{
			return null;
        }

        /// <summary>
        /// Short name of the product
        /// </summary>
        /// <returns> Short name of the product </returns>
        public string getName()
		{
            return null;
        }

        /// <summary>
        /// long description of the product
        /// </summary>
        /// <returns> long description of the product </returns>
        public string getDescription()
		{
            return null;
        }

        /// <summary>
        /// Get the ID that had been assigned by the seller to
        /// identify the product
        /// </summary>
        /// <returns> seller assigned product ID </returns>
        public string getSellerAssignedID()
        {
            return null;
        }

        /// <summary>
        /// Get the ID that had been assigned by the buyer to
        /// identify the product
        /// </summary>
        /// <returns> buyer assigned product ID </returns>
        public string getBuyerAssignedID()
        {
            return null;
        }

        /// <summary>
        /// VAT percent of the product (e.g. 19, or 5.1 if you like)
        /// </summary>
        /// <returns> VAT percent of the product </returns>
        public decimal getVATPercent()
		{
			return decimal.MinValue;
		}

        public bool getIntraCommunitySupply()
        {
            return false;
        }

        public bool getReverseCharge()
        {
            return false;
        }

        public string getTaxCategoryCode()
        {
            /*	if (isIntraCommunitySupply())
				{
					return "K"; // "K"; // within europe
				}
				else if (isReverseCharge())
				{
					return "AE"; // "AE"; // to out of europe...
				}
				else if (getVATPercent().compareTo(decimal.ZERO) == 0)
				{
					return "Z"; // "Z"; // zero rated goods
				}
				else
				{*/
            return "S"; // "S"; // one of the "standard" rates (not
                        // neccessarily a rate, even a deducted VAT
                        // is standard calculation)
                        //}
        }

        public string getTaxExemptionReason()
        {
            /*	if (isIntraCommunitySupply())
				{
					return "Intra-community supply";
				}
				else if (isReverseCharge())
				{
					return "Reverse Charge";
				}*/
            return null;
        }
    }
}