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
	public interface IExportableTransaction
	{
		/**
			 * appears in /rsm:CrossIndustryDocument/rsm:HeaderExchangedDocument/ram:Name
			 *
			 * @return Name of document
			 */
		String getDocumentName();

		/**
		 *
		 *
		 * @return Code number of Document type, e.g. "380" for invoiceF
		 *	String getDocumentCode() {
			return DocumentCodeTypeConstants.INVOICE;
		}*/


		/**
		 * Number, typically invoice number of the invoice
		 *
		 * @return invoice number
		 */
		String getNumber();

		/**
		 * the date when the invoice was created
		 *
		 * @return when the invoice was created
		 */
		DateTime getIssueDate();

		/**
		 * when the invoice is to be paid
		 *
		 * @return when the invoice is to be paid
		 */
		DateTime getDueDate();

		String getContractReferencedDocument();

		/**
		 * the sender of the invoice
		 *
		 * @return the contact person at the supplier side
		 */
		IZUGFeRDExportableTradeParty getSender();

		/**
		 * subject of the document e.g. invoice and order
		 * number as human readable text
		 *
		 * @return string with document subject
		 */
		String getSubjectNote();

        /*
			IZUGFeRDAllowanceCharge[] getZFAllowances() {
				return null;
			}


			IZUGFeRDAllowanceCharge[] getZFCharges() {
				return null;
			}


			IZUGFeRDAllowanceCharge[] getZFLogisticsServiceCharges() {
				return null;
			}
		*/

        IZUGFeRDExportableItem[] getZFItems();


        /**
		 * the recipient
		 *
		 * @return the recipient of the invoice
		 */
        IZUGFeRDExportableTradeParty getRecipient();


		/**
		 * the creditors payment informations
		 * @deprecated use getTradeSettlement
		 * @return an array of IZUGFeRDTradeSettlementPayment
		 */
		IZUGFeRDTradeSettlementPayment[] getTradeSettlementPayment();

		/**
		 * the payment information for any payment means
		 *
		 * @return an array of IZUGFeRDTradeSettlement
		 */
		IZUGFeRDTradeSettlement[] getTradeSettlement();

		/**
		 * Tax ID (not VAT ID) of the sender
		 *
		 * @return Tax ID (not VAT ID) of the sender
		 */
		String getOwnTaxID();

		/**
		 * VAT ID (Umsatzsteueridentifikationsnummer) of the sender
		 *
		 * @return VAT ID (Umsatzsteueridentifikationsnummer) of the sender
		 */
		String getOwnVATID();

		/**
		 * supplier identification assigned by the costumer
		 *
		 * @return the sender's identification
		 */
		String getOwnForeignOrganisationID();


        /**
		 * get delivery date
		 *
		 * @return the day the goods have been delivered
		 */
        DateTime getDeliveryDate();

		/**
		 * get main invoice currency used on the invoice
		 *
		 * @return three character currency of this invoice
		 */
		String getCurrency();

		/**
		 * get payment term descriptional text e.g. Bis zum 22.10.2015 ohne Abzug
		 *
		 * @return get payment terms
		 */
		String getPaymentTermDescription();

		/**
		 * get payment terms. if set, getPaymentTermDescription() and getDueDate() are
		 * ignored
		 *
		 * @return the IZUGFeRDPaymentTerms of the invoice
		 **/
		IZUGFeRDPaymentTerms getPaymentTerms();

		/**
		 * returns if a rebate agreements exists
		 *
		 * @return true if a agreement exists
		 */
		bool rebateAgreementExists();

		/**
		 * get reference document number typically used for Invoice Corrections Will be added as IncludedNote in comfort profile
		 *
		 * @return the ID of the document this document refers to
		 */
		String getReferenceNumber();

		/**
		 * consignee identification (identification of the organisation the goods are shipped to [assigned by the costumer])
		 *
		 * @return the sender's identification
		 */
		String getShipToOrganisationID();

		/**
		 * consignee name (name of the organisation the goods are shipped to)
		 *
		 * @return the consignee's organisation name
		 */
		String getShipToOrganisationName();

		/**
		 * consignee street address (street of the organisation the goods are shipped to)
		 *
		 * @return consignee street address
		 */
		String getShipToStreet();

		/**
		 * consignee street postal code (postal code of the organisation the goods are shipped to)
		 *
		 * @return consignee postal code
		 */
		String getShipToZIP();

		/**
		 * consignee city (city of the organisation the goods are shipped to)
		 *
		 * @return the consignee's city
		 */
		String getShipToLocation();

		/**
		 * consignee two digit country code (country code of the organisation the goods are shipped to)
		 *
		 * @return the consignee's two character country iso code
		 */
		String getShipToCountry();

		/**
		 * get the ID of the SellerOrderReferencedDocument, which sits in the ApplicableSupplyChainTradeAgreement/ApplicableHeaderTradeAgreement
		 *
		 * @return the ID of the document
		 */
		String getSellerOrderReferencedDocumentID();

		/**
		 * get the ID of the BuyerOrderReferencedDocument, which sits in the ApplicableSupplyChainTradeAgreement
		 *
		 * @return the ID of the document
		 */
		String getBuyerOrderReferencedDocumentID();

		/**
		 * get the ID of the preceding invoice, which is e.g. to be corrected if this is a correction
		 *
		 * @return the ID of the document
		 */
		String getInvoiceReferencedDocumentID();

		DateTime getInvoiceReferencedIssueDate();

		/**
		 * get the issue timestamp of the BuyerOrderReferencedDocument, which sits in the ApplicableSupplyChainTradeAgreement
		 *
		 * @return the IssueDateTime in format CCYY-MM-DDTHH:MM:SS
		 */
		String getBuyerOrderReferencedDocumentIssueDateTime();

		/**
		 * get the TotalPrepaidAmount located in SpecifiedTradeSettlementMonetarySummation (v1) or SpecifiedTradeSettlementHeaderMonetarySummation (v2)
		 *
		 * @return the total sum (incl. VAT) of prepayments, i.e. the difference between GrandTotalAmount and DuePayableAmount
		 */
		decimal getTotalPrepaidAmount();

        /***
		 * delivery address, i.e. ram:ShipToTradeParty (only supported for zf2)
		 * @return the IZUGFeRDExportableTradeParty delivery address
		 */

        TradeParty getDeliveryAddress();

		/***
		 * specifies the document level delivery period, will be included in a BillingSpecifiedPeriod element
		 * @return the beginning of the delivery period
		 */
		DateTime getDetailedDeliveryPeriodFrom();

		/***
		 * specifies the document level delivery period, will be included in a BillingSpecifiedPeriod element
		 * @return the end of the delivery period
		 */
		DateTime getDetailedDeliveryPeriodTo();

		/**
		 * get additional referenced documents acccording to BG-24 XRechnung (Rechnungsbegruendende Unterlagen),
		 * i.e. ram:ApplicableHeaderTradeAgreement/ram:AdditionalReferencedDocument
		 *
		 * @return a array of objects from class FileAttachment
		 
		FileAttachment[] getAdditionalReferencedDocuments() {
			return null;
		}
*/

		/***
		 * additional text description
		 * @return an array of strings of document wide "includedNotes" (descriptive text values)
		 */
		String[] getNotes();

		String getSpecifiedProcuringProjectName();

		String getSpecifiedProcuringProjectID();
    }

    public class ExportableTransaction : IExportableTransaction
    {
		/**
			 * appears in /rsm:CrossIndustryDocument/rsm:HeaderExchangedDocument/ram:Name
			 *
			 * @return Name of document
			 */
		public String getDocumentName()
		{
			return "RECHNUNG";
		}


        /**
		 *
		 *
		 * @return Code number of Document type, e.g. "380" for invoiceF
		 *	String getDocumentCode() {
			return DocumentCodeTypeConstants.INVOICE;
		}*/


        /**
		 * Number, typically invoice number of the invoice
		 *
		 * @return invoice number
		 */
        public String getNumber()
		{
			return null;
		}


        /**
		 * the date when the invoice was created
		 *
		 * @return when the invoice was created
		 */
        public DateTime getIssueDate()
		{
			return DateTime.MinValue;
		}




        /**
		 * when the invoice is to be paid
		 *
		 * @return when the invoice is to be paid
		 */
        public DateTime getDueDate()
		{
			return DateTime.MinValue;
		}

        public String getContractReferencedDocument()
		{
			return null;
		}


        /**
		 * the sender of the invoice
		 *
		 * @return the contact person at the supplier side
		 */
        public IZUGFeRDExportableTradeParty getSender()
		{
			return null;
		}


		/**
		 * subject of the document e.g. invoice and order
		 * number as human readable text
		 *
		 * @return string with document subject
		 */
		public String getSubjectNote()
		{
			return null;
		}

        /*
			IZUGFeRDAllowanceCharge[] getZFAllowances() {
				return null;
			}


			IZUGFeRDAllowanceCharge[] getZFCharges() {
				return null;
			}


			IZUGFeRDAllowanceCharge[] getZFLogisticsServiceCharges() {
				return null;
			}
		*/

        public IZUGFeRDExportableItem[] getZFItems()
		{
			return null;
        }


        /**
		 * the recipient
		 *
		 * @return the recipient of the invoice
		 */
        public IZUGFeRDExportableTradeParty getRecipient()
        {
            return null;
        }


        /**
		 * the creditors payment informations
		 * @deprecated use getTradeSettlement
		 * @return an array of IZUGFeRDTradeSettlementPayment
		 */
        public IZUGFeRDTradeSettlementPayment[] getTradeSettlementPayment()
		{
			return null;
		}
        /**
		 * the payment information for any payment means
		 *
		 * @return an array of IZUGFeRDTradeSettlement
		 */
        public IZUGFeRDTradeSettlement[] getTradeSettlement()
		{
			return null;
		}


        /**
		 * Tax ID (not VAT ID) of the sender
		 *
		 * @return Tax ID (not VAT ID) of the sender
		 */
        public String getOwnTaxID()
		{
			if (getSender() != null)
			{
				return getSender().getTaxID();
			}
			else
			{
				return null;
			}
		}


        /**
		 * VAT ID (Umsatzsteueridentifikationsnummer) of the sender
		 *
		 * @return VAT ID (Umsatzsteueridentifikationsnummer) of the sender
		 */
        public String getOwnVATID()
		{
			if (getSender() != null)
			{
				return getSender().getVATID();
			}
			else
			{
				return null;
			}
		}


        /**
		 * supplier identification assigned by the costumer
		 *
		 * @return the sender's identification
		 */
        public String getOwnForeignOrganisationID()
		{
			return null;
		}




		/**
		 * get delivery date
		 *
		 * @return the day the goods have been delivered
		 */
		public DateTime getDeliveryDate()
		{
			return DateTime.MinValue;
		}


        /**
		 * get main invoice currency used on the invoice
		 *
		 * @return three character currency of this invoice
		 */
        public String getCurrency()
		{
			return "EUR";
		}


        /**
		 * get payment term descriptional text e.g. Bis zum 22.10.2015 ohne Abzug
		 *
		 * @return get payment terms
		 */
        public String getPaymentTermDescription()
		{
			return null;
		}

        /**
		 * get payment terms. if set, getPaymentTermDescription() and getDueDate() are
		 * ignored
		 *
		 * @return the IZUGFeRDPaymentTerms of the invoice
		 **/
        public IZUGFeRDPaymentTerms getPaymentTerms()
		{
			return null;
		}

        /**
		 * returns if a rebate agreements exists
		 *
		 * @return true if a agreement exists
		 */
        public bool rebateAgreementExists()
		{
			return false;
		}

        /**
		 * get reference document number typically used for Invoice Corrections Will be added as IncludedNote in comfort profile
		 *
		 * @return the ID of the document this document refers to
		 */
        public String getReferenceNumber()
		{
			return null;
		}


		/**
		 * consignee identification (identification of the organisation the goods are shipped to [assigned by the costumer])
		 *
		 * @return the sender's identification
		 */
		public String getShipToOrganisationID()
		{
			return null;
		}


		/**
		 * consignee name (name of the organisation the goods are shipped to)
		 *
		 * @return the consignee's organisation name
		 */
		public String getShipToOrganisationName()
		{
			return null;
		}


		/**
		 * consignee street address (street of the organisation the goods are shipped to)
		 *
		 * @return consignee street address
		 */
		public String getShipToStreet()
		{
			return null;
		}


		/**
		 * consignee street postal code (postal code of the organisation the goods are shipped to)
		 *
		 * @return consignee postal code
		 */
		public String getShipToZIP()
		{
			return null;
		}


		/**
		 * consignee city (city of the organisation the goods are shipped to)
		 *
		 * @return the consignee's city
		 */
		public String getShipToLocation()
		{
			return null;
		}


		/**
		 * consignee two digit country code (country code of the organisation the goods are shipped to)
		 *
		 * @return the consignee's two character country iso code
		 */
		public String getShipToCountry()
		{
			return null;
		}


		/**
		 * get the ID of the SellerOrderReferencedDocument, which sits in the ApplicableSupplyChainTradeAgreement/ApplicableHeaderTradeAgreement
		 *
		 * @return the ID of the document
		 */
		public String getSellerOrderReferencedDocumentID()
		{
			return null;
		}
		/**
		 * get the ID of the BuyerOrderReferencedDocument, which sits in the ApplicableSupplyChainTradeAgreement
		 *
		 * @return the ID of the document
		 */
		public String getBuyerOrderReferencedDocumentID()
		{
			return null;
		}

		/**
		 * get the ID of the preceding invoice, which is e.g. to be corrected if this is a correction
		 *
		 * @return the ID of the document
		 */
		public String getInvoiceReferencedDocumentID()
		{
			return null;
		}

		public DateTime getInvoiceReferencedIssueDate() { return DateTime.MinValue; }
		/**
		 * get the issue timestamp of the BuyerOrderReferencedDocument, which sits in the ApplicableSupplyChainTradeAgreement
		 *
		 * @return the IssueDateTime in format CCYY-MM-DDTHH:MM:SS
		 */
		public String getBuyerOrderReferencedDocumentIssueDateTime()
		{
			return null;
		}


		/**
		 * get the TotalPrepaidAmount located in SpecifiedTradeSettlementMonetarySummation (v1) or SpecifiedTradeSettlementHeaderMonetarySummation (v2)
		 *
		 * @return the total sum (incl. VAT) of prepayments, i.e. the difference between GrandTotalAmount and DuePayableAmount
		 */
		public decimal getTotalPrepaidAmount()
		{
			return decimal.Zero;
		}


		/***
		 * delivery address, i.e. ram:ShipToTradeParty (only supported for zf2)
		 * @return the IZUGFeRDExportableTradeParty delivery address
		 */

		public TradeParty getDeliveryAddress()
		{
			return null;
		}


		/***
		 * specifies the document level delivery period, will be included in a BillingSpecifiedPeriod element
		 * @return the beginning of the delivery period
		 */
		public DateTime getDetailedDeliveryPeriodFrom()
		{
			return DateTime.MinValue;
		}

		/***
		 * specifies the document level delivery period, will be included in a BillingSpecifiedPeriod element
		 * @return the end of the delivery period
		 */
		public DateTime getDetailedDeliveryPeriodTo()
		{
			return DateTime.MinValue;
		}

		/**
		 * get additional referenced documents acccording to BG-24 XRechnung (Rechnungsbegruendende Unterlagen),
		 * i.e. ram:ApplicableHeaderTradeAgreement/ram:AdditionalReferencedDocument
		 *
		 * @return a array of objects from class FileAttachment
		 
		FileAttachment[] getAdditionalReferencedDocuments() {
			return null;
		}
*/

		/***
		 * additional text description
		 * @return an array of strings of document wide "includedNotes" (descriptive text values)
		 */
		public String[] getNotes()
		{
			return null;
		}

		public String getSpecifiedProcuringProjectName()
		{
			return null;
		}

		public String getSpecifiedProcuringProjectID()
		{
			return null;
		}
	}
}