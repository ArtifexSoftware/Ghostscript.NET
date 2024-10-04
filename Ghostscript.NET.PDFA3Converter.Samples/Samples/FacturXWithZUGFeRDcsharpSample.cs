//
// AddWatermarkSample.cs
// This file is part of Ghostscript.NET.Samples project
//
// Author: Josip Habjan (habjan@gmail.com, http://www.linkedin.com/in/habjan) 
// Copyright (c) 2013-2016 by Josip Habjan. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
