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
using System.Diagnostics;
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using System.IO;
using Ghostscript.NET.PDFA3Converter;
using s2industries.ZUGFeRD;


namespace Ghostscript.NET.PDFA3Converter.Samples
{
    public class FacturXWithZUGFeRDcsharpSample : ISample
    {
        public void Start()
        {
            string outFilename = "factur-x.xml";
            InvoiceDescriptor invoice = CreateInvoice();
            invoice.Save(outFilename, ZUGFeRDVersion.Version22, s2industries.ZUGFeRD.Profile.Comfort);

            PDFA3Converter converter = new PDFA3Converter(@"d:\gs\gs9.56.1\bin\gsdll64.dll");
            converter.SetZUGFeRDProfile("EN 16931");
            converter.SetZUGFeRDVersion("2.3");
            converter.SetEmbeddedXMLFile(outFilename);
            converter.ConvertToPDFA3(@"e:\in.pdf", @"e:\out-zugferdcsharp.pdf");
        }


        private InvoiceDescriptor CreateInvoice()
        {
            InvoiceDescriptor desc = InvoiceDescriptor.CreateInvoice("471102", new DateTime(2018, 03, 05), CurrencyCodes.EUR);
            desc.Name = "WARENRECHNUNG";
            desc.AddNote("Rechnung gemäß Bestellung vom 01.03.2018.");
            desc.AddNote(note: "Lieferant GmbH\r\nLieferantenstraße 20\r\n80333 München\r\nDeutschland\r\nGeschäftsführer: Hans Muster\r\nHandelsregisternummer: H A 123\r\n",
                         subjectCode: SubjectCodes.REG
                        );

            desc.AddTradeLineItem(name: "Trennblätter A4",
                                  unitCode: QuantityCodes.H87,
                                  sellerAssignedID: "TB100A4",
                                  id: new GlobalID(GlobalIDSchemeIdentifiers.EAN, "4012345001235"),
                                  grossUnitPrice: 9.9m,
                                  netUnitPrice: 9.9m,
                                  billedQuantity: 20m,
                                  taxType: TaxTypes.VAT,
                                  categoryCode: TaxCategoryCodes.S,
                                  taxPercent: 19m
                                 );

            desc.AddTradeLineItem(name: "Joghurt Banane",
                unitCode: QuantityCodes.H87,
                sellerAssignedID: "ARNR2",
                id: new GlobalID(GlobalIDSchemeIdentifiers.EAN, "4000050986428"),
                grossUnitPrice: 5.5m,
                netUnitPrice: 5.5m,
                billedQuantity: 50,
                taxType: TaxTypes.VAT,
                categoryCode: TaxCategoryCodes.S,
                taxPercent: 7
                );

            desc.ReferenceOrderNo = "04011000-12345-34";
            desc.SetSeller(name: "Lieferant GmbH",
                           postcode: "80333",
                           city: "München",
                           street: "Lieferantenstraße 20",
                           country: CountryCodes.DE,
                           id: "",
                           globalID: new GlobalID(GlobalIDSchemeIdentifiers.GLN, "4000001123452"),
                           legalOrganization: new LegalOrganization(GlobalIDSchemeIdentifiers.GLN, "4000001123452", "Lieferant GmbH")
                           );
            desc.SetSellerContact(name: "Max Mustermann",
                                  orgunit: "Muster-Einkauf",
                                  emailAddress: "Max@Mustermann.de",
                                  phoneno: "+49891234567"
                                 );
            desc.AddSellerTaxRegistration("201/113/40209", TaxRegistrationSchemeID.FC);
            desc.AddSellerTaxRegistration("DE123456789", TaxRegistrationSchemeID.VA);

            desc.SetBuyer(name: "Kunden AG Mitte",
                          postcode: "69876",
                          city: "Frankfurt",
                          street: "Kundenstraße 15",
                          country: CountryCodes.DE,
                          id: "GE2020211"
                          );

            desc.ActualDeliveryDate = new DateTime(2018, 03, 05);
            desc.SetPaymentMeans(PaymentMeansTypeCodes.SEPACreditTransfer, "Zahlung per SEPA Überweisung.");
            desc.AddCreditorFinancialAccount(iban: "DE02120300000000202051", bic: "BYLADEM1001", name: "Kunden AG");
            //desc.AddDebitorFinancialAccount(iban: "DB02120300000000202051", bic: "DBBYLADEM1001", bankName: "KundenDB AG");
            desc.AddApplicableTradeTax(basisAmount: 275.0m,
                                 percent: 7m,
                                 typeCode: TaxTypes.VAT,
                                 categoryCode: TaxCategoryCodes.S
                                 );

            desc.AddApplicableTradeTax(basisAmount: 198.0m,
                                       percent: 19m,
                                       typeCode: TaxTypes.VAT,
                                       categoryCode: TaxCategoryCodes.S
                                       );

            desc.SetTradePaymentTerms("Zahlbar innerhalb 30 Tagen netto bis 04.04.2018, 3% Skonto innerhalb 10 Tagen bis 15.03.2018");
            desc.SetTotals(lineTotalAmount: 473.0m,
                           taxBasisAmount: 473.0m,
                           taxTotalAmount: 56.87m,
                           grandTotalAmount: 529.87m,
                           duePayableAmount: 529.87m
                          );

            return desc;
        } // !CreateInvoice()
    }
}
