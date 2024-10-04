namespace Ghostscript.NET.PDFConverter.Samples.ZUGFeRD
{
	public interface IExportableTransaction
	{
		/**
			 * appears in /rsm:CrossIndustryDocument/rsm:HeaderExchangedDocument/ram:Name
			 *
			 * @return Name of document
			 */
		String getDocumentName()
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
		String getNumber()
		{
			return null;
		}


		/**
		 * the date when the invoice was created
		 *
		 * @return when the invoice was created
		 */
		DateTime? getIssueDate()
		{
			return null;
		}




		/**
		 * when the invoice is to be paid
		 *
		 * @return when the invoice is to be paid
		 */
		DateTime? getDueDate()
		{
			return null;
		}

		String getContractReferencedDocument()
		{
			return null;
		}


		/**
		 * the sender of the invoice
		 *
		 * @return the contact person at the supplier side
		 */
		IZUGFeRDExportableTradeParty getSender()
		{
			return null;
		}


		/**
		 * subject of the document e.g. invoice and order
		 * number as human readable text
		 *
		 * @return string with document subject
		 */
		String getSubjectNote()
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
		IZUGFeRDTradeSettlementPayment[]? getTradeSettlementPayment()
		{
			return null;
		}
		/**
		 * the payment information for any payment means
		 *
		 * @return an array of IZUGFeRDTradeSettlement
		 */
		IZUGFeRDTradeSettlement[]? getTradeSettlement()
		{
			return null;
		}


		/**
		 * Tax ID (not VAT ID) of the sender
		 *
		 * @return Tax ID (not VAT ID) of the sender
		 */
		String getOwnTaxID()
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
		String getOwnVATID()
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
		String getOwnForeignOrganisationID()
		{
			return null;
		}




		/**
		 * get delivery date
		 *
		 * @return the day the goods have been delivered
		 */
		DateTime? getDeliveryDate();


		/**
		 * get main invoice currency used on the invoice
		 *
		 * @return three character currency of this invoice
		 */
		String getCurrency()
		{
			return "EUR";
		}


		/**
		 * get payment term descriptional text e.g. Bis zum 22.10.2015 ohne Abzug
		 *
		 * @return get payment terms
		 */
		String getPaymentTermDescription()
		{
			return null;
		}

		/**
		 * get payment terms. if set, getPaymentTermDescription() and getDueDate() are
		 * ignored
		 *
		 * @return the IZUGFeRDPaymentTerms of the invoice
		 **/
		IZUGFeRDPaymentTerms getPaymentTerms()
		{
			return null;
		}

		/**
		 * returns if a rebate agreements exists
		 *
		 * @return true if a agreement exists
		 */
		bool rebateAgreementExists()
		{
			return false;
		}

		/**
		 * get reference document number typically used for Invoice Corrections Will be added as IncludedNote in comfort profile
		 *
		 * @return the ID of the document this document refers to
		 */
		String getReferenceNumber()
		{
			return null;
		}


		/**
		 * consignee identification (identification of the organisation the goods are shipped to [assigned by the costumer])
		 *
		 * @return the sender's identification
		 */
		String getShipToOrganisationID()
		{
			return null;
		}


		/**
		 * consignee name (name of the organisation the goods are shipped to)
		 *
		 * @return the consignee's organisation name
		 */
		String getShipToOrganisationName()
		{
			return null;
		}


		/**
		 * consignee street address (street of the organisation the goods are shipped to)
		 *
		 * @return consignee street address
		 */
		String getShipToStreet()
		{
			return null;
		}


		/**
		 * consignee street postal code (postal code of the organisation the goods are shipped to)
		 *
		 * @return consignee postal code
		 */
		String getShipToZIP()
		{
			return null;
		}


		/**
		 * consignee city (city of the organisation the goods are shipped to)
		 *
		 * @return the consignee's city
		 */
		String getShipToLocation()
		{
			return null;
		}


		/**
		 * consignee two digit country code (country code of the organisation the goods are shipped to)
		 *
		 * @return the consignee's two character country iso code
		 */
		String getShipToCountry()
		{
			return null;
		}


		/**
		 * get the ID of the SellerOrderReferencedDocument, which sits in the ApplicableSupplyChainTradeAgreement/ApplicableHeaderTradeAgreement
		 *
		 * @return the ID of the document
		 */
		String getSellerOrderReferencedDocumentID()
		{
			return null;
		}
		/**
		 * get the ID of the BuyerOrderReferencedDocument, which sits in the ApplicableSupplyChainTradeAgreement
		 *
		 * @return the ID of the document
		 */
		String getBuyerOrderReferencedDocumentID()
		{
			return null;
		}

		/**
		 * get the ID of the preceding invoice, which is e.g. to be corrected if this is a correction
		 *
		 * @return the ID of the document
		 */
		String getInvoiceReferencedDocumentID()
		{
			return null;
		}

		DateTime? getInvoiceReferencedIssueDate() { return null; }
		/**
		 * get the issue timestamp of the BuyerOrderReferencedDocument, which sits in the ApplicableSupplyChainTradeAgreement
		 *
		 * @return the IssueDateTime in format CCYY-MM-DDTHH:MM:SS
		 */
		String getBuyerOrderReferencedDocumentIssueDateTime()
		{
			return null;
		}


		/**
		 * get the TotalPrepaidAmount located in SpecifiedTradeSettlementMonetarySummation (v1) or SpecifiedTradeSettlementHeaderMonetarySummation (v2)
		 *
		 * @return the total sum (incl. VAT) of prepayments, i.e. the difference between GrandTotalAmount and DuePayableAmount
		 */
		decimal getTotalPrepaidAmount()
		{
			return decimal.Zero;
		}


		/***
		 * delivery address, i.e. ram:ShipToTradeParty (only supported for zf2)
		 * @return the IZUGFeRDExportableTradeParty delivery address
		 */

		IZUGFeRDExportableTradeParty getDeliveryAddress()
		{
			return null;
		}


		/***
		 * specifies the document level delivery period, will be included in a BillingSpecifiedPeriod element
		 * @return the beginning of the delivery period
		 */
		DateTime? getDetailedDeliveryPeriodFrom()
		{
			return null;
		}

		/***
		 * specifies the document level delivery period, will be included in a BillingSpecifiedPeriod element
		 * @return the end of the delivery period
		 */
		DateTime? getDetailedDeliveryPeriodTo()
		{
			return null;
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
		String[] getNotes()
		{
			return null;
		}

		String getSpecifiedProcuringProjectName()
		{
			return null;
		}

		String getSpecifiedProcuringProjectID()
		{
			return null;
		}
	}
}