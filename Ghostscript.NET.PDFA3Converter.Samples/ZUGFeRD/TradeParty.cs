using System.Collections.Generic;

namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{

	/// <summary>
	///*
	/// A organisation, i.e. usually a company
	/// </summary>
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @JsonIgnoreProperties(ignoreUnknown = true) public class TradeParty implements IZUGFeRDExportableTradeParty
	public class TradeParty : IZUGFeRDExportableTradeParty
	{

		protected internal string name, zip, street, location, country;
		protected internal string taxID = null, vatID = null;
		protected internal string ID = null;
		protected internal string additionalAddress = null;
		protected internal IList<BankDetails> bankDetails = new List<BankDetails>();
		//protected internal IList<IZUGFeRDTradeSettlementDebit> debitDetails = new List<IZUGFeRDTradeSettlementDebit>();
		protected internal Contact contact = null;
		//	protected internal LegalOrganisation legalOrg = null;

		/// <summary>
		/// Default constructor.
		/// Probably a bad idea but might be needed by jackson or similar
		/// </summary>
		public TradeParty()
		{

		}


		/// <summary>
		/// </summary>
		/// <param name="name"> of the company </param>
		/// <param name="street"> street and number (use setAdditionalAddress for more parts) </param>
		/// <param name="zip"> postcode of the company </param>
		/// <param name="location"> city of the company </param>
		/// <param name="country"> two letter ISO code </param>
		public TradeParty(string name, string street, string zip, string location, string country)
		{
			this.name = name;
			this.street = street;
			this.zip = zip;
			this.location = location;
			this.country = country;

		}
		/// <summary>
		/// XML parsing constructor </summary>
		/// <param name="nodes"> the nodelist returned e.g. from xpath </param>
		//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
		//ORIGINAL LINE: public TradeParty(NodeList nodes)

		public string getID()
		{
			return ID;
		}

		/// <summary>
		/// if it's a customer, this can e.g. be the customer ID
		/// </summary>
		/// <param name="ID"> customer/seller number </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty setID(string ID)
		{
			this.ID = ID;
			return this;
		}

		/// <summary>
		///*
		/// (optional) a named contact person </summary>
		/// <seealso cref="Contact"/>
		/// <param name="c"> the named contact person </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty setContact(Contact c)
		{
			this.contact = c;
			return this;
		}

		/// <summary>
		///*
		/// required (for senders, if payment is not debit): the BIC and IBAN </summary>
		/// <param name="s"> bank credentials </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty addBankDetails(BankDetails s)
		{
			bankDetails.Add(s);
			return this;
		}

		/// <summary>
		/// (optional)
		/// </summary>
		/// <param name="debitDetail"> </param>
		/// <returns> fluent setter </returns>
		/*public virtual TradeParty addDebitDetails(IZUGFeRDTradeSettlementDebit debitDetail)
		{
			debitDetails.add(debitDetail);
			return this;
		}*/
		/*
			public override IZUGFeRDLegalOrganisation getLegalOrganisation()
			{
				return legalOrg;
			}

			public virtual TradeParty setLegalOrganisation(LegalOrganisation legalOrganisation)
			{
				legalOrg = legalOrganisation;
				return this;
			}
		*/
		public virtual IList<BankDetails> getBankDetails()
		{
			return bankDetails;
		}

		/// <summary>
		///*
		/// a general tax ID </summary>
		/// <param name="taxID"> tax number of the organisation </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty addTaxID(string taxID)
		{
			this.taxID = taxID;
			return this;
		}

		/// <summary>
		///*
		/// the USt-ID </summary>
		/// <param name="vatID"> Ust-ID </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty addVATID(string vatID)
		{
			this.vatID = vatID;
			return this;
		}

		public string getVATID()
		{
			return vatID;
		}

		public virtual TradeParty setVATID(string VATid)
		{
			this.vatID = VATid;
			return this;
		}

		public string getTaxID()
		{
			return taxID;
		}

		public virtual TradeParty setTaxID(string tax)
		{
			this.taxID = tax;
			return this;
		}

		public virtual string getName()
		{
			return name;
		}

		/// <summary>
		///*
		/// required, usually done in the constructor: the complete name of the organisation </summary>
		/// <param name="name"> complete legal name </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty setName(string name)
		{
			this.name = name;
			return this;
		}


		public virtual string getZIP()
		{
			return zip;
		}

		/// <summary>
		///*
		/// usually set in the constructor, required for recipients in german invoices: postcode </summary>
		/// <param name="zip"> postcode </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty setZIP(string zip)
		{
			this.zip = zip;
			return this;
		}

		public string getStreet()
		{
			return street;
		}

		/// <summary>
		///*
		/// usually set in constructor, required in germany, street and house number </summary>
		/// <param name="street"> street name and number </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty setStreet(string street)
		{
			this.street = street;
			return this;
		}

		public string getLocation()
		{
			return location;
		}

		/// <summary>
		///*
		/// usually set in constructor, usually required in germany, the city of the organisation </summary>
		/// <param name="location"> city </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty setLocation(string location)
		{
			this.location = location;
			return this;
		}

		public string getCountry()
		{
			return country;
		}

		/// <summary>
		///*
		/// two-letter ISO code of the country </summary>
		/// <param name="country"> two-letter-code </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty setCountry(string country)
		{
			this.country = country;
			return this;
		}


		public virtual string getVatID()
		{
			return vatID;
		}

		public IZUGFeRDExportableContact getContact()
		{
			return contact;
		}

		public virtual IZUGFeRDTradeSettlement[]? getAsTradeSettlement()
		{
			if (bankDetails.Count == 0)
			{
				return null;
			}
			return bankDetails.ToArray();
		}


		public string getAdditionalAddress()
		{
			return additionalAddress;
		}


		/// <summary>
		///*
		/// additional parts of the address, e.g. which floor.
		/// Street address will become "lineOne", this will become "lineTwo" </summary>
		/// <param name="additionalAddress"> additional address description </param>
		/// <returns> fluent setter </returns>
		public virtual TradeParty setAdditionalAddress(string additionalAddress)
		{
			this.additionalAddress = additionalAddress;
			return this;
		}


	}
}