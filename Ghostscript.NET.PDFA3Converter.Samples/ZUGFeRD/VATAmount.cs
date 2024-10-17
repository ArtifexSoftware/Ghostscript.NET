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

	public class VATAmount
	{

		public VATAmount(decimal basis, decimal calculated, string categoryCode) : base()
		{
			this.basis = basis;
			this.calculated = calculated;
			this.categoryCode = categoryCode;
		}

		internal decimal basis, calculated, applicablePercent;

		internal string categoryCode;

		public virtual decimal getApplicablePercent()
		{

			return applicablePercent;
		}
		public virtual VATAmount setApplicablePercent(decimal value)
		{
			this.applicablePercent = value;
			return this;
		}


		public virtual decimal getBasis()
		{

			return basis;
		}
		public virtual VATAmount setBasis(decimal value)
		{
			this.basis = Math.Round(value, 2, MidpointRounding.AwayFromZero);
			return this;

		}





		public virtual decimal getCalculated() {
			return calculated;
		}


		public virtual VATAmount setCalculated(decimal value) {

			this.calculated = value;
			return this;
		}


		/// 
		/// @deprecated Use <seealso cref="getCategoryCode() instead"/> 
		/// <returns> String with category code </returns>
		[Obsolete("Use <seealso cref=\"getCategoryCode() instead\"/>")]
		public virtual string getDocumentCode()
		{

			return categoryCode;
		}
		public virtual VATAmount setDocumentCode(string value)

		{
			this.categoryCode = value;
			return this;
		}



		public virtual string getCategoryCode()
		{

			return categoryCode;
		}
		public virtual VATAmount setCategoryCode(String value)
		{
			this.categoryCode = value;
			return this;
		}



		public virtual VATAmount add(VATAmount v)
		{
			return new VATAmount(basis + v.getBasis(), calculated + v.getCalculated(), this.categoryCode);
		}

		public virtual VATAmount subtract(VATAmount v)
		{
			return new VATAmount(basis - v.getBasis(), calculated - v.getCalculated(), this.categoryCode);
		}

	}
}