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
using Ghostscript.NET.PDFConverter;
using Ghostscript.NET.FacturX.ZUGFeRD;


namespace Ghostscript.NET.PDFConverter.Samples
{
    public class FacturXWriteSample : ISample
    {


        public void Start()
        {
            
            Invoice i = (new Invoice()).setDueDate(DateTime.Now).setIssueDate(DateTime.Now).setDeliveryDate(DateTime.Now).setSender((new TradeParty("Test company", "Test Street 1", "55232", "Test City", "DE")).addTaxID("DE4711").addVATID("DE0815").setContact(new Contact("Hans Test", "+49123456789", "test@example.org")).addBankDetails(new BankDetails("DE12500105170648489890", "COBADEFXXX"))).setRecipient(new TradeParty("Franz Müller", "Test Steet 12", "55232", "Entenhausen", "DE")).setReferenceNumber("991-01484-64").setNumber("123").
                    addItem(new Item(new Product("Test product", "", "C62", 19m), 1.0m, 1.0m));

            ZUGFeRD2PullProvider zf2p = new ZUGFeRD2PullProvider();
            zf2p.setProfile(Profiles.getByName("XRechnung"));
            zf2p.generateXML(i);
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            string outfilename = "xrechnung.xml";
            File.WriteAllBytes(outfilename, zf2p.getXML());

            PDFConverter converter = new PDFConverter(@"e:\in.pdf", @"e:\out.pdf");
            converter.SetZUGFeRDProfile(Profiles.getByName("EN16931").getXMPName());
            converter.SetZUGFeRDVersion("2.1");
            converter.SetEmbeddedXMLFile(outfilename);
            converter.ConvertToPDFA3(@"d:\gs\gs9.56.1\bin\gsdll64.dll");
        }

    }
}
