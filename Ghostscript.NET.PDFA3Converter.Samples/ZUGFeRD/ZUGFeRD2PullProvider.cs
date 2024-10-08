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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;


namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{

    public class ZUGFeRD2PullProvider
    {

        //// MAIN CLASS
        protected internal String zugferdDateFormatting = "yyyyMMdd";
        protected internal byte[] zugferdData;
        protected internal IExportableTransaction trans;
        protected internal TransactionCalculator calc;
        private string paymentTermsDescription;
        protected internal Profile profile = Profiles.getByName("EN16931");


        /// <summary>
        /// enables the flag to indicate a test invoice in the XML structure
        /// </summary>
        public void setTest()
        {
        }

        private string vatFormat(decimal value)
        {
            return XMLTools.nDigitFormat(value, 2);
        }

        private string currencyFormat(decimal value)
        {
            return XMLTools.nDigitFormat(value, 2);
        }

        private string priceFormat(decimal value)
        {
            return XMLTools.nDigitFormat(value, 4);
        }

        private string quantityFormat(decimal value)
        {
            return XMLTools.nDigitFormat(value, 4);
        }

        public byte[] getXML()
        {
            return zugferdData;
        }


        public Profile getProfile()
        {
            return profile;
        }

        // @todo check if the two boolean args can be refactored

        /// <summary>
        ///*
        /// returns the UN/CEFACT CII XML for companies(tradeparties), which is actually
        /// the same for ZF1 (v 2013b) and ZF2 (v 2016b) </summary>
        /// <param name="party"> </param>
        /// <param name="isSender"> some attributes are allowed only for senders in certain profiles </param>
        /// <param name="isShipToTradeParty"> some attributes are allowed only for senders or recipients
        /// @return </param>
        protected internal virtual string getTradePartyAsXML(IZUGFeRDExportableTradeParty party, bool isSender, bool isShipToTradeParty)
        {
            string xml = "";
            // According EN16931 either GlobalID or seller assigned ID might be present for BuyerTradeParty
            // and ShipToTradeParty, but not both. Prefer seller assigned ID for now.
            if (party.getID() != null)
            {
                xml += "	<ram:ID>" + XMLTools.encodeXML(party.getID()) + "</ram:ID>\n";
            }
            else if ((party.getGlobalIDScheme() != null) && (party.getGlobalID() != null))
            {
                xml = xml + "           <ram:GlobalID schemeID=\"" + XMLTools.encodeXML(party.getGlobalIDScheme()) + "\">" + XMLTools.encodeXML(party.getGlobalID()) + "</ram:GlobalID>\n";
            }
            xml += "	<ram:Name>" + XMLTools.encodeXML(party.getName()) + "</ram:Name>\n"; //$NON-NLS-2$

            if ((party.getContact() != null) && (isSender || profile == Profiles.getByName("Extended")))
            {
                xml = xml + "<ram:DefinedTradeContact>\n" + "     <ram:PersonName>" + XMLTools.encodeXML(party.getContact().getName()) + "</ram:PersonName>\n";
                if (party.getContact().getPhone() != null)
                {

                    xml = xml + "     <ram:TelephoneUniversalCommunication>\n" + "        <ram:CompleteNumber>" + XMLTools.encodeXML(party.getContact().getPhone()) + "</ram:CompleteNumber>\n" + "     </ram:TelephoneUniversalCommunication>\n";
                }

                if ((party.getContact().getFax() != null) && (profile == Profiles.getByName("Extended")))
                {
                    xml = xml + "     <ram:FaxUniversalCommunication>\n" + "        <ram:CompleteNumber>" + XMLTools.encodeXML(party.getContact().getFax()) + "</ram:CompleteNumber>\n" + "     </ram:FaxUniversalCommunication>\n";
                }
                if (party.getContact().getEMail() != null)
                {

                    xml = xml + "     <ram:EmailURIUniversalCommunication>\n" + "        <ram:URIID>" + XMLTools.encodeXML(party.getContact().getEMail()) + "</ram:URIID>\n" + "     </ram:EmailURIUniversalCommunication>\n";
                }

                xml = xml + "  </ram:DefinedTradeContact>";

            }
            xml += "				<ram:PostalTradeAddress>\n" + "					<ram:PostcodeCode>" + XMLTools.encodeXML(party.getZIP()) + "</ram:PostcodeCode>\n" + "					<ram:LineOne>" + XMLTools.encodeXML(party.getStreet()) + "</ram:LineOne>\n";
            if (party.getAdditionalAddress() != null)
            {
                xml += "				<ram:LineTwo>" + XMLTools.encodeXML(party.getAdditionalAddress()) + "</ram:LineTwo>\n";
            }
            xml += "					<ram:CityName>" + XMLTools.encodeXML(party.getLocation()) + "</ram:CityName>\n" + "					<ram:CountryID>" + XMLTools.encodeXML(party.getCountry()) + "</ram:CountryID>\n" + "				</ram:PostalTradeAddress>\n";
            if ((party.getVATID() != null) && (!isShipToTradeParty))
            {
                xml += "				<ram:SpecifiedTaxRegistration>\n" + "					<ram:ID schemeID=\"VA\">" + XMLTools.encodeXML(party.getVATID()) + "</ram:ID>\n" + "				</ram:SpecifiedTaxRegistration>\n";
            }
            if ((party.getTaxID() != null) && (!isShipToTradeParty))
            {
                xml += "				<ram:SpecifiedTaxRegistration>\n" + "					<ram:ID schemeID=\"FC\">" + XMLTools.encodeXML(party.getTaxID()) + "</ram:ID>\n" + "				</ram:SpecifiedTaxRegistration>\n";

            }
            return xml;

        }


        /// <summary>
        ///*
        /// returns the XML for a charge or allowance on item level </summary>
        /// <param name="allowance"> </param>
        /// <param name="item">
        /// @return </param>
        /*	protected internal virtual string getAllowanceChargeStr(IZUGFeRDAllowanceCharge allowance, IAbsoluteValueProvider item)
            {
                string percentage = "";
                string chargeIndicator = "false";
                if ((allowance.getPercent() != null) && (profile == Profiles.getByName("Extended")))
                {
                    percentage = "<ram:CalculationPercent>" + vatFormat(allowance.getPercent()) + "</ram:CalculationPercent>";
                    percentage += "<ram:BasisAmount>" + item.getValue() + "</ram:BasisAmount>";
                }
                if (allowance.isCharge())
                {
                    chargeIndicator = "true";
                }

                string reason = "";
                if ((allowance.getReason() != null) && (profile == Profiles.getByName("Extended")))
                {
                    // only in extended profile
                    reason = "<ram:Reason>" + XMLTools.encodeXML(allowance.getReason()) + "</ram:Reason>";
                }
                string allowanceChargeStr = "<ram:AppliedTradeAllowanceCharge><ram:ChargeIndicator><udt:Indicator>" + chargeIndicator + "</udt:Indicator></ram:ChargeIndicator>" + percentage + "<ram:ActualAmount>" + priceFormat(allowance.getTotalAmount(item)) + "</ram:ActualAmount>" + reason + "</ram:AppliedTradeAllowanceCharge>";
                return allowanceChargeStr;
            }*/

        public void generateXML(IExportableTransaction trans)
        {
            this.trans = trans;
            this.calc = new TransactionCalculator(trans);

            bool hasDueDate = false;
            String germanDateFormatting = "dd.MM.yyyy";

            string exemptionReason = "";

            if (trans.getPaymentTermDescription() != null)
            {
                paymentTermsDescription = trans.getPaymentTermDescription();
            }

            if ((string.ReferenceEquals(paymentTermsDescription, null)) /*&& (trans.getDocumentCode() != org.mustangproject.ZUGFeRD.model.DocumentCodeTypeConstants.CORRECTEDINVOICE)*/)
            {
                paymentTermsDescription = "Zahlbar ohne Abzug bis " + ((DateTime)trans.getDueDate()).ToString(germanDateFormatting);

            }

            string senderReg = "";

            string rebateAgreement = "";
            if (trans.rebateAgreementExists())
            {
                rebateAgreement = "<ram:IncludedNote>\n" + "		<ram:Content>" + "Es bestehen Rabatt- und Bonusvereinbarungen.</ram:Content>\n" + "<ram:SubjectCode>AAK</ram:SubjectCode>\n" + "</ram:IncludedNote>\n";
            }

            string subjectNote = "";
            if (trans.getSubjectNote() != null)
            {
                subjectNote = "<ram:IncludedNote>\n" + "		<ram:Content>" + XMLTools.encodeXML(trans.getSubjectNote()) + "</ram:Content>\n" + "</ram:IncludedNote>\n";
            }

            string typecode = "380"; // Invoice
            
            string notes = "";
            if (trans.getNotes() != null)
            {
                foreach (string currentNote in trans.getNotes())
                {
                    notes = notes + "<ram:IncludedNote><ram:Content>" + XMLTools.encodeXML(currentNote) + "</ram:Content></ram:IncludedNote>";

                }
            }
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + "<rsm:CrossIndustryInvoice xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:rsm=\"urn:un:unece:uncefact:data:standard:CrossIndustryInvoice:100\"" + " xmlns:ram=\"urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:100\"" + " xmlns:udt=\"urn:un:unece:uncefact:data:standard:UnqualifiedDataType:100\">\n" + "	<rsm:ExchangedDocumentContext>\n" + "		<ram:GuidelineSpecifiedDocumentContextParameter>\n" + "			<ram:ID>" + getProfile().getID() + "</ram:ID>\n" + "		</ram:GuidelineSpecifiedDocumentContextParameter>\n" + "	</rsm:ExchangedDocumentContext>\n" + "	<rsm:ExchangedDocument>\n" + "		<ram:ID>" + XMLTools.encodeXML(trans.getNumber()) + "</ram:ID>\n" + "		<ram:TypeCode>" + typecode + "</ram:TypeCode>\n" + "		<ram:IssueDateTime><udt:DateTimeString format=\"102\">" + ((DateTime)trans.getIssueDate()).ToString(zugferdDateFormatting) + "</udt:DateTimeString></ram:IssueDateTime>\n" + notes + subjectNote + rebateAgreement + senderReg + "	</rsm:ExchangedDocument>\n" + "	<rsm:SupplyChainTradeTransaction>\n";
            int lineID = 0;
            foreach (IZUGFeRDExportableItem currentItem in trans.getZFItems())
            {
                lineID++;
                if (currentItem.getProduct().getTaxExemptionReason() != null)
                {
                    exemptionReason = "<ram:ExemptionReason>" + XMLTools.encodeXML(currentItem.getProduct().getTaxExemptionReason()) + "</ram:ExemptionReason>";
                }
                notes = "";
                if (currentItem.getNotes() != null)
                {
                    foreach (string currentNote in currentItem.getNotes())
                    {
                        notes = notes + "<ram:IncludedNote><ram:Content>" + XMLTools.encodeXML(currentNote) + "</ram:Content></ram:IncludedNote>";

                    }
                }
                LineCalculator lc = new LineCalculator(currentItem);
                xml = xml + "		<ram:IncludedSupplyChainTradeLineItem>\n" + "			<ram:AssociatedDocumentLineDocument>\n" + "				<ram:LineID>" + lineID + "</ram:LineID>\n" + notes + "			</ram:AssociatedDocumentLineDocument>\n" + "			<ram:SpecifiedTradeProduct>\n";
                // + " <GlobalID schemeID=\"0160\">4012345001235</GlobalID>\n"
                if (currentItem.getProduct().getSellerAssignedID() != null)
                {
                    xml = xml + "				<ram:SellerAssignedID>" + XMLTools.encodeXML(currentItem.getProduct().getSellerAssignedID()) + "</ram:SellerAssignedID>\n";
                }
                if (currentItem.getProduct().getBuyerAssignedID() != null)
                {
                    xml = xml + "				<ram:BuyerAssignedID>" + XMLTools.encodeXML(currentItem.getProduct().getBuyerAssignedID()) + "</ram:BuyerAssignedID>\n";
                }
                string allowanceChargeStr = "";
                

                xml = xml + "					<ram:Name>" + XMLTools.encodeXML(currentItem.getProduct().getName()) + "</ram:Name>\n" + "				<ram:Description>" + XMLTools.encodeXML(currentItem.getProduct().getDescription()) + "</ram:Description>\n" + "			</ram:SpecifiedTradeProduct>\n" + "			<ram:SpecifiedLineTradeAgreement>\n" + "				<ram:GrossPriceProductTradePrice>\n" + "					<ram:ChargeAmount>" + priceFormat(lc.getPriceGross()) + "</ram:ChargeAmount>\n" + "<ram:BasisQuantity unitCode=\"" + XMLTools.encodeXML(currentItem.getProduct().getUnit()) + "\">" + quantityFormat(currentItem.getBasisQuantity()) + "</ram:BasisQuantity>\n" + allowanceChargeStr + "				</ram:GrossPriceProductTradePrice>\n" + "				<ram:NetPriceProductTradePrice>\n" + "					<ram:ChargeAmount>" + priceFormat(lc.getPrice()) + "</ram:ChargeAmount>\n" + "					<ram:BasisQuantity unitCode=\"" + XMLTools.encodeXML(currentItem.getProduct().getUnit()) + "\">" + quantityFormat(currentItem.getBasisQuantity()) + "</ram:BasisQuantity>\n" + "				</ram:NetPriceProductTradePrice>\n" + "			</ram:SpecifiedLineTradeAgreement>\n" + "			<ram:SpecifiedLineTradeDelivery>\n" + "				<ram:BilledQuantity unitCode=\"" + XMLTools.encodeXML(currentItem.getProduct().getUnit()) + "\">" + quantityFormat(currentItem.getQuantity()) + "</ram:BilledQuantity>\n" + "			</ram:SpecifiedLineTradeDelivery>\n" + "			<ram:SpecifiedLineTradeSettlement>\n" + "				<ram:ApplicableTradeTax>\n" + "					<ram:TypeCode>VAT</ram:TypeCode>\n" + exemptionReason + "					<ram:CategoryCode>" + currentItem.getProduct().getTaxCategoryCode() + "</ram:CategoryCode>\n" + "					<ram:RateApplicablePercent>" + vatFormat(currentItem.getProduct().getVATPercent()) + "</ram:RateApplicablePercent>\n" + "				</ram:ApplicableTradeTax>\n";
                if ((currentItem.getDetailedDeliveryPeriodFrom() != null) || (currentItem.getDetailedDeliveryPeriodTo() != null))
                {
                    xml = xml + "<ram:BillingSpecifiedPeriod>";
                    if (currentItem.getDetailedDeliveryPeriodFrom() != null)
                    {
                        xml = xml + "<ram:StartDateTime><udt:DateTimeString format='102'>" + ((DateTime)currentItem.getDetailedDeliveryPeriodFrom()).ToString(zugferdDateFormatting) + "</udt:DateTimeString></ram:StartDateTime>";
                    }
                    if (currentItem.getDetailedDeliveryPeriodTo() != null)
                    {
                        xml = xml + "<ram:EndDateTime><udt:DateTimeString format='102'>" + ((DateTime)currentItem.getDetailedDeliveryPeriodTo()).ToString(zugferdDateFormatting) + "</udt:DateTimeString></ram:EndDateTime>";
                    }
                    xml = xml + "</ram:BillingSpecifiedPeriod>";

                }

                xml = xml + "				<ram:SpecifiedTradeSettlementLineMonetarySummation>\n" + "					<ram:LineTotalAmount>" + currencyFormat(lc.getItemTotalNetAmount()) + "</ram:LineTotalAmount>\n" + "				</ram:SpecifiedTradeSettlementLineMonetarySummation>\n";
                if (currentItem.getAdditionalReferencedDocumentID() != null)
                {
                    xml = xml + "			<ram:AdditionalReferencedDocument><ram:IssuerAssignedID>" + currentItem.getAdditionalReferencedDocumentID() + "</ram:IssuerAssignedID><ram:TypeCode>130</ram:TypeCode></ram:AdditionalReferencedDocument>\n";

                }
                xml = xml + "			</ram:SpecifiedLineTradeSettlement>\n" + "		</ram:IncludedSupplyChainTradeLineItem>\n";

            }

            xml = xml + "		<ram:ApplicableHeaderTradeAgreement>\n";
            if (trans.getReferenceNumber() != null)
            {
                xml = xml + "			<ram:BuyerReference>" + XMLTools.encodeXML(trans.getReferenceNumber()) + "</ram:BuyerReference>\n";

            }
            xml = xml + "			<ram:SellerTradeParty>\n" + getTradePartyAsXML(trans.getSender(), true, false) + "			</ram:SellerTradeParty>\n" + "			<ram:BuyerTradeParty>\n";
            // + " <ID>GE2020211</ID>\n"
            // + " <GlobalID schemeID=\"0088\">4000001987658</GlobalID>\n"

            xml += getTradePartyAsXML(trans.getRecipient(), false, false);
            xml += "			</ram:BuyerTradeParty>\n";

            if (trans.getBuyerOrderReferencedDocumentID() != null)
            {
                xml = xml + "   <ram:BuyerOrderReferencedDocument>\n" + "       <ram:IssuerAssignedID>" + XMLTools.encodeXML(trans.getBuyerOrderReferencedDocumentID()) + "</ram:IssuerAssignedID>\n" + "   </ram:BuyerOrderReferencedDocument>\n";
            }
            if (trans.getContractReferencedDocument() != null)
            {
                xml = xml + "   <ram:ContractReferencedDocument>\n" + "       <ram:IssuerAssignedID>" + XMLTools.encodeXML(trans.getContractReferencedDocument()) + "</ram:IssuerAssignedID>\n" + "    </ram:ContractReferencedDocument>\n";
            }

            // Additional Documents of XRechnung (Rechnungsbegruendende Unterlagen - BG-24 XRechnung)
            /*		if (trans.getAdditionalReferencedDocuments() != null)
                    {
                        foreach (FileAttachment f in trans.getAdditionalReferencedDocuments())
                        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final String documentContent = new String(Base64.getEncoder().encodeToString(f.getData()));
                            string documentContent = new string(Convert.ToBase64String(f.getData()));
                            xml = xml + "  <ram:AdditionalReferencedDocument>\n" + "    <ram:IssuerAssignedID>" + f.getFilename() + "</ram:IssuerAssignedID>\n" + "    <ram:TypeCode>916</ram:TypeCode>\n" + "    <ram:Name>" + f.getDescription() + "</ram:Name>\n" + "    <ram:AttachmentBinaryObject mimeCode=\"" + f.getMimetype() + "\"\n" + "      filename=\"" + f.getFilename() + "\">" + documentContent + "</ram:AttachmentBinaryObject>\n" + "  </ram:AdditionalReferencedDocument>\n";
                        }
                    }
            */
            xml = xml + "		</ram:ApplicableHeaderTradeAgreement>\n" + "		<ram:ApplicableHeaderTradeDelivery>\n";
            if (this.trans.getDeliveryAddress() != null)
            {
                xml += "<ram:ShipToTradeParty>" + getTradePartyAsXML(this.trans.getDeliveryAddress(), false, true) + "</ram:ShipToTradeParty>";
            }

            xml += "			<ram:ActualDeliverySupplyChainEvent>\n" + "				<ram:OccurrenceDateTime>";

            if (trans.getDeliveryDate() != null)
            {
                xml += "<udt:DateTimeString format=\"102\">" + ((DateTime)trans.getDeliveryDate()).ToString(zugferdDateFormatting) + "</udt:DateTimeString>";
            }
            else
            {
                throw new System.InvalidOperationException("No delivery date provided");
            }
            xml += "</ram:OccurrenceDateTime>\n";
            xml += "			</ram:ActualDeliverySupplyChainEvent>\n" + "		</ram:ApplicableHeaderTradeDelivery>\n" + "		<ram:ApplicableHeaderTradeSettlement>\n" + "			<ram:PaymentReference>" + XMLTools.encodeXML(trans.getNumber()) + "</ram:PaymentReference>\n" + "			<ram:InvoiceCurrencyCode>" + trans.getCurrency() + "</ram:InvoiceCurrencyCode>\n";

            if (trans.getTradeSettlementPayment() != null)
            {
                foreach (IZUGFeRDTradeSettlementPayment payment in trans.getTradeSettlementPayment())
                {
                    if (payment != null)
                    {
                        hasDueDate = true;
                        xml += payment.getSettlementXML();
                    }
                }
            }
            if (trans.getTradeSettlement() != null)
            {
                foreach (IZUGFeRDTradeSettlement payment in trans.getTradeSettlement())
                {
                    if (payment != null)
                    {
                        if (payment is IZUGFeRDTradeSettlementPayment)
                        {
                            hasDueDate = true;
                        }
                        xml += payment.getSettlementXML();
                    }
                }
            }
            /*		if (trans.getDocumentCode() == DocumentCodeTypeConstants.CORRECTEDINVOICE)
                    {
                        hasDueDate = false;
                    }
            */
            Dictionary<decimal, VATAmount> VATPercentAmountMap = calc.getVATPercentAmountMap();
            foreach (decimal currentTaxPercent in VATPercentAmountMap.Keys)
            {
                VATAmount amount = VATPercentAmountMap[currentTaxPercent];
                if (amount != null)
                {
                    xml += "			<ram:ApplicableTradeTax>\n" + "				<ram:CalculatedAmount>" + currencyFormat(amount.getCalculated()) + "</ram:CalculatedAmount>\n" + "				<ram:TypeCode>VAT</ram:TypeCode>\n" + exemptionReason + "				<ram:BasisAmount>" + currencyFormat(amount.getBasis()) + "</ram:BasisAmount>\n" + "				<ram:CategoryCode>" + amount.getCategoryCode() + "</ram:CategoryCode>\n" + "				<ram:RateApplicablePercent>" + vatFormat(currentTaxPercent) + "</ram:RateApplicablePercent>\n" + "			</ram:ApplicableTradeTax>\n"; //$NON-NLS-2$

                }
            }
            if ((trans.getDetailedDeliveryPeriodFrom() != null) || (trans.getDetailedDeliveryPeriodTo() != null))
            {
                xml = xml + "<ram:BillingSpecifiedPeriod>";
                if (trans.getDetailedDeliveryPeriodFrom() != null)
                {
                    xml = xml + "<ram:StartDateTime><udt:DateTimeString format='102'>" + ((DateTime)trans.getDetailedDeliveryPeriodFrom()).ToString(zugferdDateFormatting) + "</udt:DateTimeString></ram:StartDateTime>";
                }
                if (trans.getDetailedDeliveryPeriodTo() != null)
                {
                    xml = xml + "<ram:EndDateTime><udt:DateTimeString format='102'>" + ((DateTime)trans.getDetailedDeliveryPeriodTo()).ToString(zugferdDateFormatting) + "</udt:DateTimeString></ram:EndDateTime>";
                }
                xml = xml + "</ram:BillingSpecifiedPeriod>";


            }
            /*
                    if ((trans.getZFCharges() != null) && (trans.getZFCharges().length > 0))
                    {

                        foreach (decimal currentTaxPercent in VATPercentAmountMap.Keys)
                        {
                            if (calc.getChargesForPercent(currentTaxPercent).compareTo(decimal.ZERO) != 0)
                            {


                                xml = xml + "	 <ram:SpecifiedTradeAllowanceCharge>\n" + "        <ram:ChargeIndicator>\n" + "          <udt:Indicator>true</udt:Indicator>\n" + "        </ram:ChargeIndicator>\n" + "        <ram:ActualAmount>" + currencyFormat(calc.getChargesForPercent(currentTaxPercent)) + "</ram:ActualAmount>\n" + "        <ram:Reason>" + XMLTools.encodeXML(calc.getChargeReasonForPercent(currentTaxPercent)) + "</ram:Reason>\n" + "        <ram:CategoryTradeTax>\n" + "          <ram:TypeCode>VAT</ram:TypeCode>\n" + "          <ram:CategoryCode>" + VATPercentAmountMap[currentTaxPercent].getCategoryCode() + "</ram:CategoryCode>\n" + "          <ram:RateApplicablePercent>" + vatFormat(currentTaxPercent) + "</ram:RateApplicablePercent>\n" + "        </ram:CategoryTradeTax>\n" + "      </ram:SpecifiedTradeAllowanceCharge>	\n";

                            }
                        }

                    }

                    if ((trans.getZFAllowances() != null) && (trans.getZFAllowances().length > 0))
                    {
                        foreach (decimal currentTaxPercent in VATPercentAmountMap.Keys)
                        {
                            if (calc.getAllowancesForPercent(currentTaxPercent).compareTo(decimal.ZERO) != 0)
                            {
                                xml = xml + "	 <ram:SpecifiedTradeAllowanceCharge>\n" + "        <ram:ChargeIndicator>\n" + "          <udt:Indicator>false</udt:Indicator>\n" + "        </ram:ChargeIndicator>\n" + "        <ram:ActualAmount>" + currencyFormat(calc.getAllowancesForPercent(currentTaxPercent)) + "</ram:ActualAmount>\n" + "        <ram:Reason>" + XMLTools.encodeXML(calc.getAllowanceReasonForPercent(currentTaxPercent)) + "</ram:Reason>\n" + "        <ram:CategoryTradeTax>\n" + "          <ram:TypeCode>VAT</ram:TypeCode>\n" + "          <ram:CategoryCode>" + VATPercentAmountMap[currentTaxPercent].getCategoryCode() + "</ram:CategoryCode>\n" + "          <ram:RateApplicablePercent>" + vatFormat(currentTaxPercent) + "</ram:RateApplicablePercent>\n" + "        </ram:CategoryTradeTax>\n" + "      </ram:SpecifiedTradeAllowanceCharge>	\n";
                            }
                        }
                    }
            */

            if (trans.getPaymentTerms() == null)
            {
                xml = xml + "			<ram:SpecifiedTradePaymentTerms>\n" + "				<ram:Description>" + paymentTermsDescription + "</ram:Description>\n";

                /*			if (trans.getTradeSettlement() != null)
                            {
                                foreach (IZUGFeRDTradeSettlement payment in trans.getTradeSettlement())
                                {
                                    if ((payment != null) && (payment is IZUGFeRDTradeSettlementDebit))
                                    {
                                        xml += payment.getPaymentXML();
                                    }
                                }
                            }
                */
                if (hasDueDate && (trans.getDueDate() != null))
                {
                    xml = xml + "				<ram:DueDateDateTime><udt:DateTimeString format=\"102\">" + ((DateTime)trans.getDueDate()).ToString(zugferdDateFormatting) + "</udt:DateTimeString></ram:DueDateDateTime>\n"; // 20130704

                }
                xml = xml + "			</ram:SpecifiedTradePaymentTerms>\n";
            }
            else
            {
                xml = xml + buildPaymentTermsXml();
            }


            //string allowanceTotalLine = "<ram:AllowanceTotalAmount>" + currencyFormat(calc.getAllowancesForPercent(null)) + "</ram:AllowanceTotalAmount>";

            //string chargesTotalLine = "<ram:ChargeTotalAmount>" + currencyFormat(calc.getChargesForPercent(null)) + "</ram:ChargeTotalAmount>";
            string chargesTotalLine = "";
            string allowanceTotalLine = "";
            xml = xml + "<ram:SpecifiedTradeSettlementHeaderMonetarySummation>\n" + "<ram:LineTotalAmount>" + currencyFormat(calc.getTotal()) + "</ram:LineTotalAmount>\n" + chargesTotalLine + allowanceTotalLine + "<ram:TaxBasisTotalAmount>" + currencyFormat(calc.getTaxBasis()) + "</ram:TaxBasisTotalAmount>\n" + "				<ram:TaxTotalAmount currencyID=\"" + trans.getCurrency() + "\">" + currencyFormat(calc.getGrandTotal() - calc.getTaxBasis()) + "</ram:TaxTotalAmount>\n" + "				<ram:GrandTotalAmount>" + currencyFormat(calc.getGrandTotal()) + "</ram:GrandTotalAmount>\n" + "             <ram:TotalPrepaidAmount>" + currencyFormat(calc.getTotalPrepaid()) + "</ram:TotalPrepaidAmount>\n" + "				<ram:DuePayableAmount>" + currencyFormat(calc.getGrandTotal() - calc.getTotalPrepaid()) + "</ram:DuePayableAmount>\n" + "			</ram:SpecifiedTradeSettlementHeaderMonetarySummation>\n" + "		</ram:ApplicableHeaderTradeSettlement>\n";
            // + " <IncludedSupplyChainTradeLineItem>\n"
            // + " <AssociatedDocumentLineDocument>\n"
            // + " <IncludedNote>\n"
            // + " <Content>Wir erlauben uns Ihnen folgende Positionen aus der Lieferung Nr.
            // 2013-51112 in Rechnung zu stellen:</Content>\n"
            // + " </IncludedNote>\n"
            // + " </AssociatedDocumentLineDocument>\n"
            // + " </IncludedSupplyChainTradeLineItem>\n";

            xml = xml + "	</rsm:SupplyChainTradeTransaction>\n" + "</rsm:CrossIndustryInvoice>";

            System.Text.UTF8Encoding encoding = new
                     System.Text.UTF8Encoding();

            byte[] zugferdRaw;
            zugferdRaw = encoding.GetBytes(xml); ;

            zugferdData = XMLTools.removeBOM(zugferdRaw);
        }

        public void setProfile(Profile p)
        {
            profile = p;
        }
        private string buildPaymentTermsXml()
        {
            string paymentTermsXml = "<ram:SpecifiedTradePaymentTerms>";

            IZUGFeRDPaymentTerms paymentTerms = trans.getPaymentTerms();
            //IZUGFeRDPaymentDiscountTerms discountTerms = paymentTerms.getDiscountTerms();
            DateTime dueDate = paymentTerms.getDueDate();
            if (dueDate != null /*&& discountTerms != null && discountTerms.getBaseDate() != null*/)
            {
                throw new System.InvalidOperationException("if paymentTerms.dueDate is specified, paymentTerms.discountTerms.baseDate has not to be specified");
            }
            paymentTermsXml += "<ram:Description>" + paymentTerms.getDescription() + "</ram:Description>";
            if (dueDate != null)
            {
                paymentTermsXml += "<ram:DueDateDateTime>";
                paymentTermsXml += "<udt:DateTimeString format=\"102\">" + dueDate.ToString(zugferdDateFormatting) + "</udt:DateTimeString>";
                paymentTermsXml += "</ram:DueDateDateTime>";
            }
            /*
                    if (discountTerms != null)
                    {
                        paymentTermsXml += "<ram:ApplicableTradePaymentDiscountTerms>";
                        string currency = trans.getCurrency();
                        string basisAmount = currencyFormat(calc.getGrandTotal());
                        paymentTermsXml += "<ram:BasisAmount currencyID=\"" + currency + "\">" + basisAmount + "</ram:BasisAmount>";
                        paymentTermsXml += "<ram:CalculationPercent>" + discountTerms.getCalculationPercentage().ToString() + "</ram:CalculationPercent>";

                        if (discountTerms.getBaseDate() != null)
                        {
                            DateTime baseDate = discountTerms.getBaseDate();
                            paymentTermsXml += "<ram:BasisDateTime>";
                            paymentTermsXml += "<udt:DateTimeString format=\"102\">" + zugferdDateFormat.format(baseDate) + "</udt:DateTimeString>";
                            paymentTermsXml += "</ram:BasisDateTime>";

                            paymentTermsXml += "<ram:BasisPeriodMeasure unitCode=\"" + discountTerms.getBasePeriodUnitCode() + "\">" + discountTerms.getBasePeriodMeasure() + "</ram:BasisPeriodMeasure>";
                        }

                        paymentTermsXml += "</ram:ApplicableTradePaymentDiscountTerms>";
                    }
            */
            paymentTermsXml += "</ram:SpecifiedTradePaymentTerms>";
            return paymentTermsXml;
        }


    }
}