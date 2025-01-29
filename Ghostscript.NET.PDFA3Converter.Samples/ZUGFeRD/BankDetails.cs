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
	/// <summary>
	/// provides e.g. the IBAN to transfer money to :-)
	/// </summary>
	public class BankDetails : IZUGFeRDTradeSettlementPayment
	{
		protected internal string IBAN, BIC, accountName = null;

		public BankDetails(string IBAN, string BIC)
		{
			this.IBAN = IBAN;
			this.BIC = BIC;
		}

		public virtual string getIBAN()
		{
			return IBAN;
		}

		/// <summary>
		/// Sets the IBAN "ID", which means that it only needs to be a way to uniquely
		/// identify the IBAN. Of course you will specify your own IBAN in full length but
		/// if you deduct from a customer's account you may e.g. leave out the first or last
		/// digits so that nobody spying on the invoice gets to know the complete number </summary>
		/// <param name="IBAN"> the "IBAN ID", i.e. the IBAN or parts of it </param>
		/// <returns> fluent setter </returns>
		public virtual BankDetails setIBAN(string IBAN)
		{
			this.IBAN = IBAN;
			return this;
		}

		public virtual string getBIC()
		{
			return BIC;
		}

		/// <summary>
		///*
		/// The bank identifier. Bank name is no longer neccessary in SEPA. </summary>
		/// <param name="BIC"> the bic code </param>
		/// <returns> fluent setter </returns>
		public virtual BankDetails setBIC(string BIC)
		{
			this.BIC = BIC;
			return this;
		}

		/// <summary>
		///*
		///  getOwn... methods will be removed in the future in favor of Tradeparty (e.g. Sender) class
		/// 
		/// </summary>
		[Obsolete]
		public string getOwnBIC()
		{
			return getBIC();
		}

		[Obsolete]
		public string getOwnIBAN()
		{
			return getIBAN();
		}


		/// <summary>
		/// set Holder </summary>
		/// <param name="name"> account name (usually account holder if != sender) </param>
		/// <returns> fluent setter </returns>
		public virtual BankDetails setAccountName(string name)
		{
			accountName = name;
			return this;
		}

		public string getAccountName()
		{
			return accountName;
		}
		public string getSettlementXML()
		{
			string accountNameStr = "";
			if (getAccountName() != null)
			{
				accountNameStr = "<ram:AccountName>" + XMLTools.encodeXML(getAccountName()) + "</ram:AccountName>\n";

			}

			string xml = "			<ram:SpecifiedTradeSettlementPaymentMeans>\n" + "				<ram:TypeCode>58</ram:TypeCode>\n" + "				<ram:Information>SEPA credit transfer</ram:Information>\n" + "				<ram:PayeePartyCreditorFinancialAccount>\n" + "					<ram:IBANID>" + XMLTools.encodeXML(getOwnIBAN()) + "</ram:IBANID>\n";
			xml += accountNameStr;
			xml += "				</ram:PayeePartyCreditorFinancialAccount>\n" + "				<ram:PayeeSpecifiedCreditorFinancialInstitution>\n" + "					<ram:BICID>" + XMLTools.encodeXML(getOwnBIC()) + "</ram:BICID>\n" + "				</ram:PayeeSpecifiedCreditorFinancialInstitution>\n" + "			</ram:SpecifiedTradeSettlementPaymentMeans>\n";
			return xml;
		}



	}
}