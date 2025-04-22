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

	public class LineCalculator
	{
		private decimal price;
		private decimal priceGross;
		private decimal itemTotalNetAmount;
		private decimal itemTotalVATAmount;
		private decimal allowance = decimal.Zero;
		private decimal charge = decimal.Zero;

		//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
		//ORIGINAL LINE: public LineCalculator(IZUGFeRDExportableItem currentItem)
		public LineCalculator(IZUGFeRDExportableItem currentItem)
		{
			/*
					if (currentItem.getItemAllowances() != null && currentItem.getItemAllowances().length > 0)
					{
			//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			//ORIGINAL LINE: foreach(IZUGFeRDAllowanceCharge allowance @in currentItem.getItemAllowances())

						{
							addAllowance(allowance.getTotalAmount(currentItem));
						}
					}
					if (currentItem.getItemCharges() != null && currentItem.getItemCharges().length > 0)
					{
			//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			//ORIGINAL LINE: foreach(IZUGFeRDAllowanceCharge charge @in currentItem.getItemCharges())

						{
							addCharge(charge.getTotalAmount(currentItem));
						}
					}
			//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			//ORIGINAL LINE: @decimal multiplicator = currentItem.getProduct().getVATPercent().divide(100);
			*/
			decimal multiplicator = currentItem.getProduct().getVATPercent() / 100m;
			priceGross = currentItem.getPrice(); // see https://github.com/ZUGFeRD/mustangproject/issues/159
			price = priceGross - allowance + charge;
			itemTotalNetAmount = Math.Round(price * currentItem.getQuantity() / currentItem.getBasisQuantity(), 2, MidpointRounding.AwayFromZero);
			itemTotalVATAmount = itemTotalNetAmount * multiplicator;

		}


		public virtual decimal getPrice()
		{

			return price;

		}

		public virtual decimal getItemTotalNetAmount()
		{

			return itemTotalNetAmount;

		}

		public virtual decimal getItemTotalVATAmount()
		{
			return itemTotalVATAmount;
		}

		public virtual decimal getItemTotalGrossAmount()
		{
			return itemTotalNetAmount;
		}

		public virtual decimal getPriceGross()
		{
			return priceGross;
		}

		public virtual void addAllowance(decimal b)
		{
			allowance += b;
		}

		public virtual void addCharge(decimal b)
		{
			charge += b;
		}


	}
}