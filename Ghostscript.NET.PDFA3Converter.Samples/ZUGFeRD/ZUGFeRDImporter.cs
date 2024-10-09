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
using System.IO;
using System.Xml;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Logging;

/// <summary>
/// ********************************************************************** Copyright 2018 Jochen Staerk Use is subject to license terms. Licensed under the
/// Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
/// http://www.apache.org/licenses/LICENSE-2.0. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed
/// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions
/// and limitations under the License.
/// </summary>
namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{

    /// <summary>
    /// Mustangproject's ZUGFeRD implementation ZUGFeRD importer Licensed under the APLv2
    /// 
    /// @date 2014-07-07
    /// @version 1.1.0
    /// @author jstaerk
    /// </summary>
    public class ZUGFeRDImporter
    {

        /// <summary>
        /// if metadata has been found
        /// </summary>
        //JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods of the current type:
        protected internal bool containsMeta_Conflict = false;
        protected System.Xml.XmlDocument xmlDoc;
        /// <summary>
        /// map filenames of additional XML files to their contents
        /// </summary>
        private Dictionary<string, sbyte[]> additionalXMLs = new Dictionary<string, sbyte[]>();
        /// <summary>
        /// Raw XML form of the extracted data - may be directly obtained.
        /// </summary>
        private byte[] rawXML = null;
        /// <summary>
        /// XMP metadata
        /// </summary>
        private string xmpString = null; // XMP metadata
        /// <summary>
        /// parsed Document
        /// </summary>
        private XmlDocument document;

        private readonly ILogger<ZUGFeRDImporter> _logger;


        protected internal ZUGFeRDImporter()
        {
            //constructor for extending classes
        }

        public ZUGFeRDImporter(string pdfFilename)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug);
        });

            if (!File.Exists(pdfFilename)) {
                throw new Exception("File not found");
            }

            _logger = loggerFactory.CreateLogger<ZUGFeRDImporter>();
            _logger.LogInformation("Example log message");
            FromPDF(pdfFilename);
            //FileStream fs = File.OpenRead(pdfFilename);
            //		extractLowLevel(fs);
        }


        public ZUGFeRDImporter(Stream pdfStream)
        {
            try
            {
                extractLowLevel(pdfStream);
            }
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final java.io.IOException e)
            catch (IOException e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                //		_logger.LogError(e.Message);
                //		throw new ZUGFeRDExportException(e);
            }
        }


        /// <summary>
        /// Extracts a ZUGFeRD invoice from a PDF document represented by an input stream. Errors are reported via exception handling.
        /// </summary>
        /// <param name="pdfStream"> a inputstream of a pdf file </param>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void extractLowLevel(java.io.InputStream pdfStream) throws java.io.IOException
        private void extractLowLevel(Stream pdfStream)
        {

        }

        /// <summary>
        ///     Liest eine PDF-Datei, füllt mPDFInfo und liefert im Erfolgsfall (es ist eine ZF-Datei) den Stream zurück um die XML-Daten auszulesen.
        ///     Normal und vor dem schreiben kann die Check-Methode aufgerufen werden, importCheck ist eine autarke Methode die zurückliefert ob ein Leseversuch
        ///     (in dem Fall einer ZUGFeRD-Datei) überhaupt Aussicht auf Erfolg brächte 
        ///     </summary>
        ///     <param name="pPDFFile"></param>
        ///     <returns></returns>
        protected PdfStream GetStreamFromPDF(string pPDFFile)
        {
            PdfReader reader = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                reader = new PdfReader(pPDFFile);
            }
            catch (Exception ex)
            {
                return default;
            }// No valid PDF file 
             

            PdfDocument doc = new PdfDocument(reader);
            PdfAConformanceLevel PDFConfLevel = doc.GetReader().GetPdfAConformanceLevel();
            if (PDFConfLevel == null)
            {
                return default;
            }

            //mPDFFileInfo.levelConf = PDFConfLevel.GetConformance();
            //mPDFFileInfo.levelPart = PDFConfLevel.GetPart();
            //mPDFFileInfo.isPDFA3 = System.Convert.ToBoolean(mPDFFileInfo.levelPart == "3"); // True

            if ((PDFConfLevel.GetPart() != "3") && (PDFConfLevel.GetPart() != "4"))
            {
                return default;
            }                

            PdfDictionary root = doc.GetCatalog().GetPdfObject();
            PdfDictionary names = root.GetAsDictionary(PdfName.Names);
            PdfDictionary embeddedFiles; // = names.GetAsDictionary(PdfName.EmbeddedFiles)
            if (names == null)
            {
                return default;
            }                
            embeddedFiles = names.GetAsDictionary(PdfName.EmbeddedFiles);
            PdfArray namesArray = embeddedFiles.GetAsArray(PdfName.Names);
            int currentNameIndex = 0;
            PdfDictionary filespec;
            PdfStream stream = null/* TODO Change to default(_) if this is not a reference type */;

            if (namesArray == null)
            {
                // Es gibt in PDF/A-3 zwei Möglichkeiten Dateien einzubetten: Flat arrays und "normal"
                // hier Flat Arrays
                PdfArray kidsArray = (PdfArray)embeddedFiles.Get(PdfName.Kids);
                filespec = kidsArray.GetAsDictionary(0);
                PdfObject pdfNamesArrayObject = filespec.Get(PdfName.Names);
                if (pdfNamesArrayObject.IsArray())
                {
                    PdfArray pdfNamesArray = (PdfArray)pdfNamesArrayObject;

                    //mPDFFileInfo.ctrAttachments = pdfNamesArray.Size;
                    for (int j = 0; j <= pdfNamesArray.Size() - 1; j++)
                    {
                        PdfObject pdfNamesArrayItemObject = pdfNamesArray.Get(j);
                        if (pdfNamesArrayItemObject.IsString())
                        {
                            PdfString filename = (PdfString)pdfNamesArrayItemObject;

                        }
                        else if (pdfNamesArrayItemObject.IsDictionary())
                        {
                            PdfDictionary pdfNamesArrayDictionary = (PdfDictionary)pdfNamesArrayItemObject;
                            PdfDictionary pdfEFdict = (PdfDictionary)pdfNamesArrayDictionary.Get(PdfName.EF);
                            stream = (PdfStream)pdfEFdict.Get(PdfName.F);
                        }
                    }
                }
            }
            else
            {
                //mPDFFileInfo.ctrAttachments = namesArray.Size;
                // Hier werden normal eingebettete Dateien ausgelesen
                while (currentNameIndex < namesArray.Size())
                {
                    namesArray.GetAsName(currentNameIndex);
                    currentNameIndex += 1;
                    filespec = namesArray.GetAsDictionary(currentNameIndex);
                    currentNameIndex += 1;
                    PdfDictionary refs = filespec.GetAsDictionary(PdfName.EF);
                    foreach (PdfName key in refs.KeySet())
                    {
                        string filename = filespec.GetAsString(key).ToString();

                        if (filename.ToLower() == "factur-x.xml" | filename.ToLower() == "zugferd-invoice.xml" | filename.ToLower() == "xrechnung.xml" | filename.ToLower() == "order-x.xml" | filename.ToLower() == "deliver-x.xml")
                            stream = refs.GetAsStream(key);
                    }
                }
            }

            return stream;
        }


        /// <summary>
        /// Lesen der ZUGFeRD-XML-Struktur aus einer PDF-Datei in eine Zieldatei
        /// </summary>
        /// <param name="pPDFFile"></param>
        /// <returns>false, falls nicht gelesen werden kann. Probleme können in dem Fall mPDFInfo entnommen werden, dort muss alles auf True bzw. 1 stehen</returns>
        public bool FromPDF(string pPDFFile)
        {
            PdfStream stream = GetStreamFromPDF(pPDFFile);
            if (stream == null)
                return false; // details in mPDFInfo
            else
            {
                xmlDoc = new System.Xml.XmlDocument();                
                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding(false);                
                byte[] xmlBytes = stream.GetBytes();
                string xml = enc.GetString(xmlBytes);

                if (xmlBytes.Length > 3 & xmlBytes[0] == 239 & xmlBytes[1] == 187 & xmlBytes[2] == 191)
                    // has a UTF8 BOM of three bytes, which is converted to ONE single unicode character in the String
                    xml = xml.Substring(1);


                xmlDoc.LoadXml(xml);

                rawXML = xmlBytes;
                //            FromXML(xmlDoc);

                return true;
            }
        }



        /// <summary>
        /// Returns the raw XML data as extracted from the ZUGFeRD PDF file.
        /// </summary>
        /// <returns> the raw ZUGFeRD XML data </returns>
        public virtual byte[] getRawXML()
        {
            return rawXML;
        }


        protected internal virtual string extractString(string xpathStr)
        {
            XmlNode resultNode = xmlDoc.SelectSingleNode(xpathStr);

            if (resultNode?.InnerText == null) 
            { 
                return ""; 
            }
            return resultNode.InnerText;
        }


        /// <returns> the reference (purpose) the sender specified for this invoice </returns>
        public virtual string getForeignReference()
        {
            string result = extractString("//*[local-name() = 'ApplicableHeaderTradeSettlement']/*[local-name() = 'PaymentReference']");
            if (string.ReferenceEquals(result, null) || result.Length == 0)
            {
                result = extractString("//*[local-name() = 'ApplicableSupplyChainTradeSettlement']/*[local-name() = 'PaymentReference']");
            }
            return result;
        }

        /// <returns> the ZUGFeRD Profile </returns>
        public virtual string getZUGFeRDProfil()
        {
            switch (extractString("//*[local-name() = 'GuidelineSpecifiedDocumentContextParameter']//*[local-name() = 'ID']"))
            {
                case "urn:cen.eu:en16931:2017":
                case "urn:ferd:CrossIndustryDocument:invoice:1p0:comfort":
                    return "COMFORT";
                case "urn:cen.eu:en16931:2017#compliant#urn:factur-x.eu:1p0:basic":
                case "urn:ferd:CrossIndustryDocument:invoice:1p0:basic":
                    return "BASIC";
                case "urn:factur-x.eu:1p0:basicwl":
                    return "BASIC WL";
                case "urn:factur-x.eu:1p0:minimum":
                    return "MINIMUM";
                case "urn:ferd:CrossIndustryDocument:invoice:1p0:extended":
                case "urn:cen.eu:en16931:2017#conformant#urn:factur-x.eu:1p0:extended":
                    return "EXTENDED";
            }
            return "";
        }

        /// <returns> the Invoice Currency Code </returns>
        public virtual string getInvoiceCurrencyCode()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'ApplicableSupplyChainTradeSettlement']//*[local-name() = 'InvoiceCurrencyCode']");
                }
                else
                {
                    return extractString("//*[local-name() = 'ApplicableHeaderTradeSettlement']//*[local-name() = 'InvoiceCurrencyCode']");
                }
            }            
            catch (Exception e)
            {             
                _logger.LogError(e.Message);

                return "";
            }
        }

        /// <returns> the IssuerAssigned ID </returns>
        public virtual string getIssuerAssignedID()
        {
            return extractIssuerAssignedID("BuyerOrderReferencedDocument");
        }

        /// <returns> the SellerOrderReferencedDocument IssuerAssigned ID </returns>
        public virtual string getSellerOrderReferencedDocumentIssuerAssignedID()
        {
            return extractIssuerAssignedID("SellerOrderReferencedDocument");
        }

        /// <returns> the IssuerAssigned ID </returns>
        public virtual string getContractOrderReferencedDocumentIssuerAssignedID()
        {
            return extractIssuerAssignedID("ContractReferencedDocument");
        }

        private string extractIssuerAssignedID(string propertyName)
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'BuyerOrderReferencedDocument']//*[local-name() = 'ID']");
                }
                else
                {
                    return extractString("//*[local-name() = 'BuyerOrderReferencedDocument']//*[local-name() = 'IssuerAssignedID']");
                }
            }            
            catch (Exception e)
            {                
                _logger.LogError(e.Message);
                return "";
            }
        }

        /// <returns> the BuyerTradeParty ID </returns>
        public virtual string getBuyerTradePartyID()
        {
            return extractString("//*[local-name() = 'BuyerTradeParty']//*[local-name() = 'ID']");
        }

        /// <returns> the Issue Date() </returns>
        public virtual string getIssueDate()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'HeaderExchangedDocument']//*[local-name() = 'IssueDateTime']//*[local-name() = 'DateTimeString']");
                }
                else
                {
                    return extractString("//*[local-name() = 'ExchangedDocument']//*[local-name() = 'IssueDateTime']//*[local-name() = 'DateTimeString']");
                }
            }            
            catch (Exception e)
            {             
                _logger.LogError(e.Message);
                return "";
            }
        }

        /// <returns> the TaxBasisTotalAmount </returns>
        public virtual string getTaxBasisTotalAmount()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementMonetarySummation']//*[local-name() = 'TaxBasisTotalAmount']");
                }
                else
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementHeaderMonetarySummation']//*[local-name() = 'TaxBasisTotalAmount']");
                }
            }            
            catch (Exception e)
            {             
                _logger.LogError(e.Message);
                return "";
            }
        }

        /// <returns> the TaxTotalAmount </returns>
        public virtual string getTaxTotalAmount()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementMonetarySummation']//*[local-name() = 'TaxTotalAmount']");
                }
                else
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementHeaderMonetarySummation']//*[local-name() = 'TaxTotalAmount']");
                }
            }
            catch (Exception e)
            {             
                _logger.LogError(e.Message);
                return "";
            }
        }

        /// <returns> the RoundingAmount </returns>
        public virtual string getRoundingAmount()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementMonetarySummation']//*[local-name() = 'RoundingAmount']");
                }
                else
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementHeaderMonetarySummation']//*[local-name() = 'RoundingAmount']");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return "";
            }
        }

        /// <returns> the TotalPrepaidAmount </returns>
        public virtual string getPaidAmount()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementMonetarySummation']//*[local-name() = 'TotalPrepaidAmount']");
                }
                else
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementHeaderMonetarySummation']//*[local-name() = 'TotalPrepaidAmount']");
                }
            }
            catch (Exception e)
            {             
                _logger.LogError(e.Message);
                return "";
            }
        }

        /// <returns> SellerTradeParty GlobalID </returns>
        public virtual string getSellerTradePartyGlobalID()
        {
            return extractString("//*[local-name() = 'SellerTradeParty']//*[local-name() = 'GlobalID']");
        }

        /// <returns> the BuyerTradeParty GlobalID </returns>
        public virtual string getBuyerTradePartyGlobalID()
        {
            return extractString("//*[local-name() = 'BuyerTradeParty']//*[local-name() = 'GlobalID']");
        }

        /// <returns> the BuyerTradeParty SpecifiedTaxRegistration ID </returns>
        public virtual string getBuyertradePartySpecifiedTaxRegistrationID()
        {
            return extractString("//*[local-name() = 'BuyerTradeParty']//*[local-name() = 'SpecifiedTaxRegistration']//*[local-name() = 'ID']");
        }


        /// <returns> the IncludedNote </returns>
        public virtual string getIncludedNote()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'HeaderExchangedDocument']//*[local-name() = 'IncludedNote']");
                }
                else
                {
                    return extractString("//*[local-name() = 'ExchangedDocument']//*[local-name() = 'IncludedNote']");
                }
            }
            catch (Exception e)
            {             
                _logger.LogError(e.Message);
                return "";
            }
        }


        /// <returns> the BuyerTradeParty Name </returns>
        public virtual string getBuyerTradePartyName()
        {
            return extractString("//*[local-name() = 'BuyerTradeParty']//*[local-name() = 'Name']");
        }


        /// <returns> the line Total Amount </returns>
        public virtual string getLineTotalAmount()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementMonetarySummation']//*[local-name() = 'LineTotalAmount']");
                }
                else
                {
                    return extractString("//*[local-name() = 'SpecifiedTradeSettlementHeaderMonetarySummation']//*[local-name() = 'LineTotalAmount']");
                }
            }
            catch (Exception e)
            {             
                _logger.LogError(e.Message);
                return "";
            }
        }

        /// <returns> the Payment Terms </returns>
        public virtual string getPaymentTerms()
        {
            return extractString("//*[local-name() = 'SpecifiedTradePaymentTerms']//*[local-name() = 'Description']");
        }

        /// <returns> the Taxpoint Date </returns>
        public virtual string getTaxPointDate()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'ActualDeliverySupplyChainEvent']//*[local-name() = 'OccurrenceDateTime']//*[local-name() = 'DateTimeString']");
                }
                else
                {
                    return extractString("//*[local-name() = 'ActualDeliverySupplyChainEvent']//*[local-name() = 'OccurrenceDateTime']//*[local-name() = 'DateTimeString']");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return "";
            }
        }

        /// <returns> the Invoice ID </returns>
        public virtual string getInvoiceID()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'HeaderExchangedDocument']//*[local-name() = 'ID']");
                }
                else
                {
                    return extractString("//*[local-name() = 'ExchangedDocument']//*[local-name() = 'ID']");
                }
            }

            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return "";
            }
        }



        /// <returns> the document code </returns>
        public virtual string getDocumentCode()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'HeaderExchangedDocument']/*[local-name() = 'TypeCode']");
                }
                else
                {
                    return extractString("//*[local-name() = 'ExchangedDocument']/*[local-name() = 'TypeCode']");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return "";
            }
        }


        /// <returns> the referred document </returns>
        public virtual string getReference()
        {
            try
            {
                if (getVersion() == 1)
                {
                    return extractString("//*[local-name() = 'ApplicableSupplyChainTradeAgreement']/*[local-name() = 'BuyerReference']");
                }
                else
                {
                    return extractString("//*[local-name() = 'ApplicableHeaderTradeAgreement']/*[local-name() = 'BuyerReference']");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return "";
            }
        }


        /// <returns> the sender's bank's BIC code </returns>
        public virtual string getBIC()
        {
            return extractString("//*[local-name() = 'PayeeSpecifiedCreditorFinancialInstitution']/*[local-name() = 'BICID']");
        }


        /// <returns> the sender's bank name </returns>
        public virtual string getBankName()
        {
            return extractString("//*[local-name() = 'PayeeSpecifiedCreditorFinancialInstitution']/*[local-name() = 'Name']");
        }


        /// <returns> the sender's account IBAN code </returns>
        public virtual string getIBAN()
        {
            return extractString("//*[local-name() = 'PayeePartyCreditorFinancialAccount']/*[local-name() = 'IBANID']");
        }


        public virtual string getHolder()
        {
            return extractString("//*[local-name() = 'SellerTradeParty']/*[local-name() = 'Name']");
        }


        /// <returns> the total payable amount </returns>
        public virtual string getAmount()
        {
            // xmlDoc property is a  XmlDocument
            // 
            XmlNode amountNode = xmlDoc.SelectSingleNode("//*[local-name() = 'SpecifiedTradeSettlementHeaderMonetarySummation']/*[local-name() = 'DuePayableAmount']");
            if (amountNode.InnerText == null) { return ""; }
            return amountNode.InnerText;
        }


        /// <returns> when the payment is due </returns>
        public virtual string getDueDate()
        {
            return extractString("//*[local-name() = 'SpecifiedTradePaymentTerms']/*[local-name() = 'DueDateDateTime']/*[local-name() = 'DateTimeString']");
        }


        public virtual int getVersion()
        {
            if (!containsMeta_Conflict)
            {
                throw new Exception("Not yet parsed");
            }
            if (getUTF8().Contains("<rsm:CrossIndustryDocument"))
            {
                return 1;
            }
            else if (getUTF8().Contains("<rsm:CrossIndustryInvoice"))
            {
                return 2;
            }
            throw new Exception("ZUGFeRD version could not be determined");
        }

        public virtual string getUTF8()
        {
            if (rawXML == null)
            {
                return null;
            }
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            return encoding.GetString(rawXML);

        }
    }
}
