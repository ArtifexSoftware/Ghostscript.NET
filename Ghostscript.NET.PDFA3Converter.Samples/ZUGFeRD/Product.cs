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
	
	/// <summary>
	///*
	/// describes a product, good or service used in an invoice item line
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonIgnoreProperties(ignoreUnknown = true) public class Product implements org.mustangproject.ZUGFeRD.IZUGFeRDExportableProduct
	public class Product : IZUGFeRDExportableProduct
	{
		protected internal string unit, name, description, sellerAssignedID, buyerAssignedID;
		protected internal decimal VATPercent;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods of the current type:
		protected internal bool isReverseCharge_Conflict = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods of the current type:
		protected internal bool isIntraCommunitySupply_Conflict = false;

		/// <summary>
		///*
		/// default constructor </summary>
		/// <param name="name"> product short name </param>
		/// <param name="description"> product long name </param>
		/// <param name="unit"> a two/three letter UN/ECE rec 20 unit code, e.g. "C62" for piece </param>
		/// <param name="VATPercent"> product vat rate </param>
		public Product(string name, string description, string unit, decimal VATPercent)
		{
			this.unit = unit;
			this.name = name;
			this.description = description;
			this.VATPercent = VATPercent;
		}


		/// <summary>
		///*
		/// empty constructor
		/// just for jackson etc
		/// </summary>
		public Product()
		{

		}


		public virtual string getSellerAssignedID()
		{
			return sellerAssignedID;
		}

		/// <summary>
		///*
		/// how the seller identifies this type of product </summary>
		/// <param name="sellerAssignedID"> a unique String </param>
		/// <returns> fluent setter </returns>
		public virtual Product setSellerAssignedID(string sellerAssignedID)
		{
			this.sellerAssignedID = sellerAssignedID;
			return this;
		}

		public virtual string getBuyerAssignedID()
		{
			return buyerAssignedID;
		}

		/// <summary>
		///*
		/// if the buyer provided an ID how he refers to this product </summary>
		/// <param name="buyerAssignedID"> a string the buyer provided </param>
		/// <returns> fluent setter </returns>
		public virtual Product setBuyerAssignedID(string buyerAssignedID)
		{
			this.buyerAssignedID = buyerAssignedID;
			return this;
		}

		public  bool isReverseCharge()
		{
			return isReverseCharge_Conflict;
		}

		public  bool isIntraCommunitySupply()
		{
			return isIntraCommunitySupply_Conflict;
		}

		/// <summary>
		///*
		/// sets reverse charge(=delivery to outside EU) </summary>
		/// <returns> fluent setter </returns>
		public virtual Product setReverseCharge()
		{
			isReverseCharge_Conflict = true;
			setVATPercent(decimal.Zero);
			return this;
		}


		/// <summary>
		///*
		/// sets intra community supply(=delivery outside the country inside the EU) </summary>
		/// <returns> fluent setter </returns>
		public virtual Product setIntraCommunitySupply()
		{
			isIntraCommunitySupply_Conflict = true;
			setVATPercent(decimal.Zero);
			return this;
		}

		public  string getUnit()
		{
			return unit;
		}

		/// <summary>
		///*
		/// sets a UN/ECE rec 20 or 21 code which unit the product ships in, e.g. C62=piece </summary>
		/// <param name="unit"> 2-3 letter UN/ECE rec 20 or 21 </param>
		/// <returns> fluent setter </returns>
		public virtual Product setUnit(string unit)
		{
			this.unit = unit;
			return this;
		}

		public  string getName()
		{
			return name;
		}

		/// <summary>
		/// name of the product </summary>
		/// <param name="name"> short name </param>
		/// <returns> fluent setter </returns>
		public virtual Product setName(string name)
		{
			this.name = name;
			return this;
		}

		public  string getDescription()
		{
			return description;
		}

		/// <summary>
		/// description of the product (required) </summary>
		/// <param name="description"> long name </param>
		/// <returns> fluent setter </returns>
		public virtual Product setDescription(string description)
		{
			this.description = description;
			return this;
		}

		public  decimal getVATPercent()
		{
			return VATPercent;
		}

		/// <summary>
		///**
		/// VAT rate of the product </summary>
		/// <param name="VATPercent"> vat rate of the product </param>
		/// <returns> fluent setter </returns>
		public virtual Product setVATPercent(decimal VATPercent)
		{
			this.VATPercent = VATPercent;
			return this;
		}
	}
}
