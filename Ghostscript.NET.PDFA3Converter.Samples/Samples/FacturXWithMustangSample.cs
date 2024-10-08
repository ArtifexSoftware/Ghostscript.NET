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
using Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD;


namespace Ghostscript.NET.PDFA3Converter.Samples
{ 
    public class FacturXWithMustangSample : ISample
    {
        public void Start()
        {
            
            Invoice i = (new Invoice()).setDueDate(DateTime.Now).setIssueDate(DateTime.Now).setDeliveryDate(DateTime.Now).setSender((new TradeParty("Test company", "Test Street 1", "55232", "Test City", "DE")).addTaxID("DE4711").addVATID("DE0815").setContact(new Contact("Hans Test", "+49123456789", "test@example.org")).addBankDetails(new BankDetails("DE12500105170648489890", "COBADEFXXX"))).setRecipient(new TradeParty("Franz Müller", "Test Steet 12", "55232", "Entenhausen", "DE")).setReferenceNumber("991-01484-64").setNumber("123").
                    addItem(new Item(new Product("Test product", "", "C62", 19m), 1.0m, 1.0m));

            ZUGFeRD2PullProvider zf2p = new ZUGFeRD2PullProvider();
            zf2p.setProfile(Profiles.getByName("XRechnung"));
            zf2p.generateXML(i);
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            string outfilename = "factur-x.xml";
            File.WriteAllBytes(outfilename, zf2p.getXML());

            PDFA3Converter converter = new PDFA3Converter(@"d:\gs\gs9.56.1\bin\gsdll64.dll");
            converter.SetZUGFeRDProfile(Profiles.getByName("EN16931").getXMPName());
            converter.SetZUGFeRDVersion("2.1");
            converter.SetEmbeddedXMLFile(outfilename);
            converter.ConvertToPDFA3(@"sample-invoice.pdf", @"sample-invoice-mustang.pdf");
        }
    }
}
