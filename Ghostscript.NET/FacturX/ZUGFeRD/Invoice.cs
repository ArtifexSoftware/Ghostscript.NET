/**
 * *********************************************************************
 * <p>
 * Copyright 2018 Jochen Staerk
 * <p>
 * Use is subject to license terms.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at http://www.apache.org/licenses/LICENSE-2.0.
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * <p>
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * <p>
 * **********************************************************************
 */
 using System;
using System.Collections.Generic;
using java.math;

namespace Ghostscript.NET.FacturX.ZUGFeRD
{
    /// <summary>
    ///*
    /// An invoice, with fluent setters </summary>
    /// <seealso cref="IExportableTransaction if you want to implement an interface instead"/>
    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @JsonIgnoreProperties(ignoreUnknown = true) public class Invoice implements IExportableTransaction
    public class Invoice : IExportableTransaction
	{

		protected internal string documentName = null, documentCode = null, number = null, ownOrganisationFullPlaintextInfo = null, referenceNumber = null, shipToOrganisationID = null, shipToOrganisationName = null, shipToStreet = null, shipToZIP = null, shipToLocation = null, shipToCountry = null, buyerOrderReferencedDocumentID = null, invoiceReferencedDocumentID = null, buyerOrderReferencedDocumentIssueDateTime = null, ownForeignOrganisationID = null, ownOrganisationName = null, currency = null, paymentTermDescription = null;
		protected internal DateTime? issueDate = null, dueDate = null;
		protected DateTime? deliveryDate = null;
		protected internal BigDecimal totalPrepaidAmount = null;
		protected internal TradeParty sender = null, recipient = null, deliveryAddress = null;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonDeserialize(contentAs=Item.class) protected java.util.ArrayList<IZUGFeRDExportableItem> ZFItems = null;
		protected internal List<IZUGFeRDExportableItem> ZFItems = null;
		protected internal List<string> notes = null;
		  protected internal string sellerOrderReferencedDocumentID;
		protected internal string contractReferencedDocument = null;
		//protected internal List<FileAttachment> xmlEmbeddedFiles = null;

		protected internal DateTime? detailedDeliveryDateStart = null;
		protected internal DateTime? detailedDeliveryPeriodEnd = null;

		//protected internal List<IZUGFeRDAllowanceCharge> Allowances = new List<IZUGFeRDAllowanceCharge>(), Charges = new List<IZUGFeRDAllowanceCharge>(), LogisticsServiceCharges = new List<IZUGFeRDAllowanceCharge>();
		protected internal IZUGFeRDPaymentTerms paymentTerms = null;
		protected internal DateTime invoiceReferencedIssueDate;
		protected internal string specifiedProcuringProjectID = null;
		protected internal string specifiedProcuringProjectName = null;

	  public Invoice()
	  {
			ZFItems = new List<IZUGFeRDExportableItem>();
			setCurrency("EUR");
	  }
	



public string getDocumentName()
{
			return documentName;
}

	public string getContractReferencedDocument()
	{
			return contractReferencedDocument;
	}

	public virtual Invoice setDocumentName(string documentName)
	{
		this.documentName = documentName;
		return this;
	}

	public string DocumentCode()
	{
			return documentCode;
	}

	public virtual Invoice setDocumentCode(string documentCode)
	{
		this.documentCode = documentCode;
		return this;
	}
/*
	public virtual Invoice embedFileInXML(FileAttachment fa)
	{
		if (xmlEmbeddedFiles == null)
		{
			xmlEmbeddedFiles = new List<>();
		}
		xmlEmbeddedFiles.add(fa);
		return this;
	}

	public override FileAttachment[] getAdditionalReferencedDocuments()
	{
		
			if (xmlEmbeddedFiles == null)
			{
				return null;
			}
			return xmlEmbeddedFiles.toArray(new FileAttachment[0]);
    
		
	}
	*/
	public IZUGFeRDExportableTradeParty getRecipient()
	{
		return recipient;
	}

	/// <summary>
	/// required.
	/// sets the invoice receiving institution = invoicee </summary>
	/// <param name="recipient"> the invoicee organisation </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice setRecipient(TradeParty recipient)
	{
		this.recipient = recipient;
		return this;
	}


	public  string getNumber()
	{
			return number;
	}

	public virtual Invoice setNumber(string number)
	{
		this.number = number;
		return this;
	}


	/***
	 * switch type to invoice correction and refer to document number.
	 * Please note that the quantities need to be negative, if you
	 * e.g. delivered 100 and take 50 back the quantity should be -50 in the
	 * corrected invoice, which will result in negative VAT and a negative payment amount
	 * @param number the invoice number to be corrected
	 * @return this object (fluent setter)
	 */
public virtual Invoice setCorrection(string number)
{
		setInvoiceReferencedDocumentID(number);
		documentCode = "384";
		return this;
}
	public virtual Invoice setCreditNote()
	{
		documentCode = "381";
		return this;
	}

	public string getOwnOrganisationFullPlaintextInfo()
	{
			return ownOrganisationFullPlaintextInfo;
	}

	public virtual Invoice setOwnOrganisationFullPlaintextInfo(string ownOrganisationFullPlaintextInfo)
	{
		this.ownOrganisationFullPlaintextInfo = ownOrganisationFullPlaintextInfo;
		return this;
	}

	public string getReferenceNumber()
	{
			return referenceNumber;
	}

	public virtual Invoice setReferenceNumber(string referenceNumber)
	{
		this.referenceNumber = referenceNumber;
		return this;
	}

	public  string getShipToOrganisationID()
	{
			return shipToOrganisationID;
		
	}

	public virtual Invoice setShipToOrganisationID(string shipToOrganisationID)
	{
		this.shipToOrganisationID = shipToOrganisationID;
		return this;
	}

	public string ShipToOrganisationName()
	{
		
			return shipToOrganisationName;
		
	}

	public virtual Invoice setShipToOrganisationName(string shipToOrganisationName)
	{
		this.shipToOrganisationName = shipToOrganisationName;
		return this;
	}

	public string getShipToStreet()
	{
		
			return shipToStreet;
		
	}


	public virtual Invoice setShipToStreet(string shipToStreet)
	{
		this.shipToStreet = shipToStreet;
		return this;
	}

	public  string getShipToZIP()
	{
		
			return shipToZIP;
		
	}

	public virtual Invoice setShipToZIP(string shipToZIP)
	{
		this.shipToZIP = shipToZIP;
		return this;
	}

	public string ShipToLocation()
	{
		
			return shipToLocation;
		
	}

	public virtual Invoice setShipToLocation(string shipToLocation)
	{
		this.shipToLocation = shipToLocation;
		return this;
	}

	public string ShipToCountry()
	{
		
			return shipToCountry;
		
	}

	public virtual Invoice setShipToCountry(string shipToCountry)
	{
		this.shipToCountry = shipToCountry;
		return this;
	}

	public string BuyerOrderReferencedDocumentID()
	{
		
			return buyerOrderReferencedDocumentID;
		
	}
	public string SellerOrderReferencedDocumentID()
	{
	
			return sellerOrderReferencedDocumentID;
		
	}




public virtual Invoice setSellerOrderReferencedDocumentID(string sellerOrderReferencedDocumentID)
{
	this.sellerOrderReferencedDocumentID = sellerOrderReferencedDocumentID;
	return this;
}
	/// <summary>
	///*
	/// usually the order number </summary>
	/// <param name="buyerOrderReferencedDocumentID"> string with number </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice setBuyerOrderReferencedDocumentID(string buyerOrderReferencedDocumentID)
	{
		this.buyerOrderReferencedDocumentID = buyerOrderReferencedDocumentID;
		return this;
	}

	/// <summary>
	///*
	/// usually in case of a correction the original invoice number </summary>
	/// <param name="invoiceReferencedDocumentID"> string with number </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice setInvoiceReferencedDocumentID(string invoiceReferencedDocumentID)
	{
		this.invoiceReferencedDocumentID = invoiceReferencedDocumentID;
		return this;
	}
	public string getInvoiceReferencedDocumentID()
	{
		
			return invoiceReferencedDocumentID;
		
	}

  public DateTime getInvoiceReferencedIssueDate()
  {
	  
		return invoiceReferencedIssueDate;
	  
  }

  public virtual Invoice setInvoiceReferencedIssueDate(DateTime issueDate)
  {
	  this.invoiceReferencedIssueDate = issueDate;
	  return this;
  }

  public string getBuyerOrderReferencedDocumentIssueDateTime()
  {
	  
			return buyerOrderReferencedDocumentIssueDateTime;
	  
  }

	/// <summary>
	///*
	/// when the order (or whatever reference in BuyerOrderReferencedDocumentID) was issued (@todo switch to date?) </summary>
	/// <param name="buyerOrderReferencedDocumentIssueDateTime">  IssueDateTime in format CCYY-MM-DDTHH:MM:SS </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice setBuyerOrderReferencedDocumentIssueDateTime(string buyerOrderReferencedDocumentIssueDateTime)
	{
		this.buyerOrderReferencedDocumentIssueDateTime = buyerOrderReferencedDocumentIssueDateTime;
		return this;
	}
	public IZUGFeRDExportableTradeParty getSender() {
		return sender;
	}
		public string getOwnTaxID()
	{
			return getSender().getTaxID();
	}


[Obsolete]
public virtual Invoice setOwnTaxID(string ownTaxID)
{
		sender.addTaxID(ownTaxID);
		return this;
}

	public string getOwnVATID()
	{
		
			return getSender().getVATID();
		
	}

	[Obsolete]
	public virtual Invoice setOwnVATID(string ownVATID)
	{
		sender.addVATID(ownVATID);
		return this;
	}

	public string getOwnForeignOrganisationID()
	{
		
			return ownForeignOrganisationID;
		
	}




	public virtual void setZFItems(List<IZUGFeRDExportableItem> ims)
	{
		ZFItems = ims;
	}

	[Obsolete]
	public virtual Invoice setOwnForeignOrganisationID(string ownForeignOrganisationID)
	{
		this.ownForeignOrganisationID = ownForeignOrganisationID;
		return this;
	}

	public string getOwnOrganisationName()
	{
		
			return ownOrganisationName;
		
	}


	[Obsolete]
	public virtual Invoice setOwnOrganisationName(string ownOrganisationName)
	{
		this.ownOrganisationName = ownOrganisationName;
		return this;
	}

	public string getOwnStreet()
	{
		
			return sender.getStreet();
		
	}


	public string getOwnZIP()
	{
		
			return sender.getZIP();
		
	}


	public string getOwnLocation()
	{
		
			return sender.getLocation();
		
	}


	public string getOwnCountry()
	{
		
			return sender.getCountry();
		
	}


	public  string[] getNotes()
	{
		
			if (notes == null)
			{
				return null;
			}
			return notes.ToArray();
		
	}

	public string getCurrency()
	{
		
			return currency;
		
	}


	public virtual Invoice setCurrency(string currency)
	{
		this.currency = currency;
		return this;
	}

	public string getPaymentTermDescription()
	{
		
			return paymentTermDescription;
		
	}

	public virtual Invoice setPaymentTermDescription(string paymentTermDescription)
	{
		this.paymentTermDescription = paymentTermDescription;
		return this;
	}

	public DateTime? getIssueDate()
	{
		
			return issueDate;
		
	}

	public virtual Invoice setIssueDate(DateTime issueDate)
	{
		this.issueDate = issueDate;
		return this;
	}

	public DateTime? getDueDate()
	{
		
			return dueDate;
		
	}

	public virtual Invoice setDueDate(DateTime dueDate)
	{
		this.dueDate = dueDate;
		return this;
	}

	public DateTime? getDeliveryDate()
	{
		
			return deliveryDate;
		
	}

	public virtual Invoice setDeliveryDate(DateTime deliveryDate)
	{
		this.deliveryDate = deliveryDate;
		return this;
	}

	public BigDecimal getTotalPrepaidAmount()
	{
		
			return totalPrepaidAmount;
		
	}

	public virtual Invoice setTotalPrepaidAmount(BigDecimal totalPrepaidAmount)
	{
		this.totalPrepaidAmount = totalPrepaidAmount;
		return this;
	}

	
	/// <summary>
	///*
	/// sets a named sender contact </summary>
	/// @deprecated use setSender 
	/// <seealso cref="Contact"/>
	/// <param name="ownContact"> the sender contact </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice setOwnContact(Contact ownContact)
	{
		this.sender.setContact(ownContact);
		return this;
	}

	/// <summary>
	/// required.
	/// sets the invoicing institution = invoicer </summary>
	/// <param name="sender"> the invoicer </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice setSender(TradeParty sender)
	{
		this.sender = sender;
/*		if ((sender.getBankDetails() != null) && (sender.getBankDetails().size() > 0))
		{
			// convert bankdetails

		}*/
		return this;
	}
/*
	public IZUGFeRDAllowanceCharge[] getZFAllowances()
	{
		
			if (Allowances.isEmpty())
			{
				return null;
			}
			else
			{
		  return Allowances.toArray(new IZUGFeRDAllowanceCharge[0]);
			}
		
	}


	public IZUGFeRDAllowanceCharge[] getZFCharges()
	{
		
			if (Charges.isEmpty())
			{
				return null;
			}
			else
			{
		  return Charges.toArray(new IZUGFeRDAllowanceCharge[0]);
			}
		
	}


	public IZUGFeRDAllowanceCharge[] getZFLogisticsServiceCharges()
	{
		
			if (LogisticsServiceCharges.isEmpty())
			{
				return null;
			}
			else
			{
		  return LogisticsServiceCharges.toArray(new IZUGFeRDAllowanceCharge[0]);
			}
		
	}

*/
	public IZUGFeRDTradeSettlement[]? getTradeSettlement()
	{
		
    
			if (getSender() == null)
			{
				return null;
			}
    
			return ((TradeParty) getSender()).getAsTradeSettlement();
    
		
	}


	public IZUGFeRDPaymentTerms getPaymentTerms()
	{
		
			return paymentTerms;
		
	}

	public virtual Invoice setPaymentTerms(IZUGFeRDPaymentTerms paymentTerms)
	{
		this.paymentTerms = paymentTerms;
		return this;
	}

	public TradeParty getDeliveryAddress()
	{
		
			return deliveryAddress;
		
	}

	/// <summary>
	///*
	/// if the delivery address is not the recipient address, it can be specified here </summary>
	/// <param name="deliveryAddress"> the goods receiving organisation </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice setDeliveryAddress(TradeParty deliveryAddress)
	{
		this.deliveryAddress = deliveryAddress;
		return this;
	}

	public IZUGFeRDExportableItem[] getZFItems()
	{
			return ZFItems.ToArray();

	}


	/// <summary>
	/// required
	/// adds invoice "lines" :-) </summary>
	/// <seealso cref="Item"/>
	/// <param name="item"> the invoice line </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice addItem(IZUGFeRDExportableItem item)
	{
		ZFItems.Add(item);
		return this;
	}


	/// <summary>
	///*
	/// checks if all required items are set in order to be able to export it </summary>
	/// <returns> true if all required items are set </returns>
	public virtual bool Valid
	{
		get
		{
			return (dueDate != null) && (sender != null) && (sender.getTaxID() != null) && (sender.getVATID() != null) && (recipient != null);
			//contact
			//		this.phone = phone;
			//		this.email = email;
			//		this.street = street;
			//		this.zip = zip;
			//		this.location = location;
			//		this.country = country;
		}
	}


/// <summary>
///*
/// adds a document level addition to the price </summary>
/// <seealso cref="Charge"/>
/// <param name="izac"> the charge to be applied </param>
/// <returns> fluent setter </returns>
/*
	public virtual Invoice addCharge(IZUGFeRDAllowanceCharge izac)
	{
		Charges.add(izac);
		return this;
	}
*/
	/// <summary>
	///*
	/// adds a document level rebate </summary>
	/// <seealso cref="Allowance"/>
	/// <param name="izac"> the allowance to be applied </param>
	/// <returns> fluent setter </returns>
/*
	public virtual Invoice addAllowance(IZUGFeRDAllowanceCharge izac)
	{
		Allowances.add(izac);
		return this;
	}
*/
	/// <summary>
	///*
	/// adds the ID of a contract referenced in the invoice </summary>
	/// <param name="s"> the contract number </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice setContractReferencedDocument(string s)
	{
		contractReferencedDocument = s;
		return this;
	}


	/// <summary>
	///*
	/// sets a document level delivery period,
	/// which is optional additional to the mandatory deliverydate
	/// and which will become a BillingSpecifiedPeriod-Element </summary>
	/// <param name="start"> the date of first delivery </param>
	/// <param name="end"> the date of last delivery </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice setDetailedDeliveryPeriod(DateTime start, DateTime end)
	{
		detailedDeliveryDateStart = start;
		detailedDeliveryPeriodEnd = end;

		return this;
	}


	public DateTime? getDetailedDeliveryPeriodFrom()
	{
		
			return detailedDeliveryDateStart;
		
	}

	public  DateTime? getDetailedDeliveryPeriodTo()
	{
		
			return detailedDeliveryPeriodEnd;
		
	}


	/// <summary>
	/*
	/// adds a free text paragraph, which will become a includedNote element </summary>
	/// <param name="text"> freeform UTF8 plain text </param>
	/// <returns> fluent setter </returns>
	public virtual Invoice addNote(string text)
	{
		if (notes == null)
		{
			notes = new List<>();
		}
		notes.add(text);
		return this;
	}*/

	public  string getSpecifiedProcuringProjectID()
	{
		
			return specifiedProcuringProjectID;
		
	}

	public virtual Invoice setSpecifiedProcuringProjectID(string specifiedProcuringProjectID)
	{
		this.specifiedProcuringProjectID = specifiedProcuringProjectID;
		return this;
	}

	public  string getSpecifiedProcuringProjectName()
{
	
			return specifiedProcuringProjectName;
	
}

	public virtual Invoice setSpecifiedProcuringProjectName(string specifiedProcuringProjectName)
	{
		this.specifiedProcuringProjectName = specifiedProcuringProjectName;
		return this;
	}
}
}
