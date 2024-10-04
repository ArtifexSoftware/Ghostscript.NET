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
namespace Ghostscript.NET.PDFConverter.Samples.ZUGFeRD
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

        ///     ''' Liest eine PDF-Datei, füllt mPDFInfo und liefert im Erfolgsfall (es ist eine ZF-Datei) den Stream zurück um die XML-Daten auszulesen.

        ///     ''' Normal und vor dem schreiben kann die Check-Methode aufgerufen werden, importCheck ist eine autarke Methode die zurückliefert ob ein Leseversuch

        ///     ''' (in dem Fall einer ZUGFeRD-Datei) überhaupt Aussicht auf Erfolg brächte 

        ///     ''' </summary>

        ///     ''' <param name="pPDFFile"></param>

        ///     ''' <returns></returns>
        protected PdfStream GetStreamFromPDF(string pPDFFile)
        {
            PdfReader reader = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                reader = new PdfReader(pPDFFile);
            }
            catch (Exception ex)
            {
                return null/* TODO Change to default(_) if this is not a reference type */;
            }// No valid PDF file 
             //mPDFFileInfo.isPDF = true;

            PdfDocument doc = new PdfDocument(reader);

            PdfAConformanceLevel PDFConfLevel = doc.GetReader().GetPdfAConformanceLevel();
            if (PDFConfLevel == null)
                return null/* TODO Change to default(_) if this is not a reference type */;// kein PDF/A
                                                                                           //mPDFFileInfo.isPDFA = true;

            //mPDFFileInfo.levelConf = PDFConfLevel.GetConformance();
            //mPDFFileInfo.levelPart = PDFConfLevel.GetPart();
            //mPDFFileInfo.isPDFA3 = System.Convert.ToBoolean(mPDFFileInfo.levelPart == "3"); // True

            if ((PDFConfLevel.GetPart() != "3") && (PDFConfLevel.GetPart() != "4"))
                return null/* TODO Change to default(_) if this is not a reference type */;// kein PDF/A-3


            PdfDictionary root = doc.GetCatalog().GetPdfObject();
            PdfDictionary names = root.GetAsDictionary(PdfName.Names);
            PdfDictionary embeddedFiles; // = names.GetAsDictionary(PdfName.EmbeddedFiles)
            if (names == null)
                return null/* TODO Change to default(_) if this is not a reference type */;// keine eingebetteten dateien
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
            //if (stream != null)
            //  mPDFFileInfo.hasZFAttachment = true;


            return stream;
        }

        /// <summary>
        ///     ''' Lesen der ZUGFeRD-XML-Struktur aus einer PDF-Datei in eine Zieldatei
        ///     ''' </summary>
        ///     ''' <param name="pPDFFile"></param>
        ///     ''' <returns>false, falls nicht gelesen werden kann. Probleme können in dem Fall mPDFInfo entnommen werden, dort muss alles auf True bzw. 1 stehen</returns>
        public bool FromPDF(string pPDFFile)
        {
            PdfStream stream = GetStreamFromPDF(pPDFFile);
            if (stream == null)
                return false; // details in mPDFInfo
            else
            {
                xmlDoc = new System.Xml.XmlDocument();
                // Dim encoding As New System.Text.UTF8Encoding(True) ' The boolean parameter controls BOM
                // Dim reader As New System.IO.StreamReader("your file path", encoding)


                // Dim xm As XmlReader = XmlReader.Create(New StringReader(Text.Encoding.UTF8.GetString(stream.GetBytes)))
                // 
                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding(false);
                // Dim xm As XmlReader = XmlReader.Create(enc.GetString(stream.GetBytes))
                // Dim encoding As New System.Text.UTF8Encoding(False) ' The boolean parameter controls BOM

                // Dim readerx As New System.IO.StreamReader("C:\Users\jstaerk\test.xml", encoding)
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
        /*
                protected internal virtual Document getDocument()
                {
                    return document;
                }


                private void setDocument()
                {
                    DocumentBuilderFactory xmlFact = DocumentBuilderFactory.newInstance();
                    xmlFact.setNamespaceAware(true);
                    DocumentBuilder builder = xmlFact.newDocumentBuilder();
                    MemoryStream @is = new MemoryStream(rawXML);
                ///	is.skip(guessBOMSize(is));
                    document = builder.parse(@is);
                }


                public virtual void setRawXML(sbyte[] rawXML)
                {
                    this.rawXML = rawXML;
                    try
                    {
                        setDocument();
                    }
                    catch (Exception e) when (e is ParserConfigurationException || e is SAXException)
                    {
        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                        _logger.LogError(e.Message);
                        throw new ZUGFeRDExportException(e);
                    }
                }



        */
        protected internal virtual string extractString(string xpathStr)
        {
            XmlNode resultNode = xmlDoc.SelectSingleNode(xpathStr);




            /*			string result = extractString("//*[local-name() = 'SpecifiedTradeSettlementHeaderMonetarySummation']/*[local-name() = 'DuePayableAmount']");
                        if (string.ReferenceEquals(result, null) || result.Length == 0)
                        {

                            // fx/zf would be SpecifiedTradeSettlementMonetarySummation
                            // 				but ox is SpecifiedTradeSettlementHeaderMonetarySummation...
                            result = extractString("//*[local-name() = 'GrandTotalAmount']");


                        }
                        */
            if (resultNode.InnerText == null) { return ""; }
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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
            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
            //ORIGINAL LINE: catch (final Exception e)
            catch (Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
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



            /*			string result = extractString("//*[local-name() = 'SpecifiedTradeSettlementHeaderMonetarySummation']/*[local-name() = 'DuePayableAmount']");
                        if (string.ReferenceEquals(result, null) || result.Length == 0)
                        {

                            // fx/zf would be SpecifiedTradeSettlementMonetarySummation
                            // 				but ox is SpecifiedTradeSettlementHeaderMonetarySummation...
                            result = extractString("//*[local-name() = 'GrandTotalAmount']");


                        }
                        */
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

        /*
                public virtual Dictionary<string, sbyte[]> getAdditionalData()
                {
                    return additionalXMLs;
                }


                /// <summary>
                /// get xmp metadata of the PDF, null if not available
                /// </summary>
                /// <returns> string </returns>
                public virtual string getXMP()
                {
                    return xmpString;
                }


                /// <returns> if export found parseable ZUGFeRD data </returns>
                public virtual bool containsMeta()
                {
                    return containsMeta_Conflict;
                }


                /// <param name="meta"> raw XML to be set </param>
                /// <exception cref="IOException"> if raw can not be set </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public void setMeta(String meta) throws java.io.IOException
                public virtual void setMeta(string meta)
                {
                    setRawXML(meta.GetBytes());
                }


                /// <returns> raw XML of the invoice </returns>
                public virtual string getMeta()
                {
                    if (rawXML == null)
                    {
                        return null;
                    }

                    return StringHelper.NewString(rawXML);
                }


    

                /// <returns> return UTF8 XML (without BOM) of the invoice </returns>
               




                /// <summary>
                /// will return true if the metadata (just extract-ed or set with setMeta) contains ZUGFeRD XML
                /// </summary>
                /// <returns> true if the invoice contains ZUGFeRD XML </returns>
                public virtual bool canParse()
                {

                    // SpecifiedExchangedDocumentContext is in the schema, so a relatively good
                    // indication if zugferd is present - better than just invoice
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final String meta = getMeta();
                    string meta = getMeta();
                    return (!string.ReferenceEquals(meta, null)) && (meta.Length > 0) && ((meta.Contains("SpecifiedExchangedDocumentContext") || meta.Contains("ExchangedDocumentContext")));
                }


                internal static string convertStreamToString(Stream @is)
                {
                    // source https://stackoverflow.com/questions/309424/how-do-i-read-convert-an-inputstream-into-a-string-in-java referring to
                    // https://community.oracle.com/blogs/pat/2004/10/23/stupid-scanner-tricks
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final java.util.Scanner s = new java.util.Scanner(is, "UTF-8").useDelimiter("\\A");
                    Scanner s = (new Scanner(@is, "UTF-8")).useDelimiter("\\A");
                    return s.hasNext() ? s.next() : "";
                }

                /// <summary>
                /// returns an instance of PostalTradeAddress for SellerTradeParty section </summary>
                /// <returns> an instance of PostalTradeAddress </returns>
                public virtual PostalTradeAddress getBuyerTradePartyAddress()
                {

                    NodeList nl = null;

                    try
                    {
                        if (getVersion() == 1)
                        {
                            nl = getNodeListByPath("//*[local-name() = 'CrossIndustryDocument']//*[local-name() = 'SpecifiedSupplyChainTradeTransaction']/*[local-name() = 'ApplicableSupplyChainTradeAgreement']//*[local-name() = 'BuyerTradeParty']//*[local-name() = 'PostalTradeAddress']");
                        }
                        else
                        {
                            nl = getNodeListByPath("//*[local-name() = 'CrossIndustryInvoice']//*[local-name() = 'SupplyChainTradeTransaction']//*[local-name() = 'ApplicableHeaderTradeAgreement']//*[local-name() = 'BuyerTradeParty']//*[local-name() = 'PostalTradeAddress']");
                        }
                    }
        //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
        //ORIGINAL LINE: catch (final Exception e)
                    catch (Exception e)
                    {
        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                        _logger.LogError(e.Message);
                        return null;
                    }

                    return getAddressFromNodeList(nl);
                }

                /// <summary>
                /// returns an instance of PostalTradeAddress for SellerTradeParty section </summary>
                /// <returns> an instance of PostalTradeAddress </returns>
                public virtual PostalTradeAddress getSellerTradePartyAddress()
                {

                    NodeList nl = null;
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final PostalTradeAddress address = new PostalTradeAddress();
                    PostalTradeAddress address = new PostalTradeAddress();

                    try
                    {
                        if (getVersion() == 1)
                        {
                            nl = getNodeListByPath("//*[local-name() = 'CrossIndustryDocument']//*[local-name() = 'SpecifiedSupplyChainTradeTransaction']//*[local-name() = 'ApplicableSupplyChainTradeAgreement']//*[local-name() = 'SellerTradeParty']//*[local-name() = 'PostalTradeAddress']");
                        }
                        else
                        {
                            nl = getNodeListByPath("//*[local-name() = 'CrossIndustryInvoice']//*[local-name() = 'SupplyChainTradeTransaction']//*[local-name() = 'ApplicableHeaderTradeAgreement']//*[local-name() = 'SellerTradeParty']//*[local-name() = 'PostalTradeAddress']");
                        }
                    }
        //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
        //ORIGINAL LINE: catch (final Exception e)
                    catch (Exception e)
                    {
        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                        _logger.LogError(e.Message);
                        return null;
                    }

                    return getAddressFromNodeList(nl);
                }

                private PostalTradeAddress getAddressFromNodeList(NodeList nl)
                {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final PostalTradeAddress address = new PostalTradeAddress();
                    PostalTradeAddress address = new PostalTradeAddress();

                    if (nl != null)
                    {
                        for (int i = 0; i < nl.getLength(); i++)
                        {
                            Node n = nl.item(i);
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.w3c.dom.NodeList nodes = n.getChildNodes();
                            NodeList nodes = n.getChildNodes();
                            for (int j = 0; j < nodes.getLength(); j++)
                            {
                                n = nodes.item(j);
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final short nodeType = n.getNodeType();
                                short nodeType = n.getNodeType();
                                if ((nodeType == Node.ELEMENT_NODE) && (n.getLocalName() != null))
                                {
                                    switch (n.getLocalName())
                                    {
                                        case "PostcodeCode":
                                            address.setPostCodeCode("");
                                            if (n.getFirstChild() != null)
                                            {
                                                address.setPostCodeCode(n.getFirstChild().getNodeValue());
                                            }
                                            break;
                                        case "LineOne":
                                            address.setLineOne("");
                                            if (n.getFirstChild() != null)
                                            {
                                                address.setLineOne(n.getFirstChild().getNodeValue());
                                            }
                                            break;
                                        case "LineTwo":
                                            address.setLineTwo("");
                                            if (n.getFirstChild() != null)
                                            {
                                                address.setLineTwo(n.getFirstChild().getNodeValue());
                                            }
                                            break;
                                        case "LineThree":
                                            address.setLineThree("");
                                            if (n.getFirstChild() != null)
                                            {
                                                address.setLineThree(n.getFirstChild().getNodeValue());
                                            }
                                            break;
                                        case "CityName":
                                            address.setCityName("");
                                            if (n.getFirstChild() != null)
                                            {
                                                address.setCityName(n.getFirstChild().getNodeValue());
                                            }
                                            break;
                                        case "CountryID":
                                            address.setCountryID("");
                                            if (n.getFirstChild() != null)
                                            {
                                                address.setCountryID(n.getFirstChild().getNodeValue());
                                            }
                                            break;
                                        case "CountrySubDivisionName":
                                            address.setCountrySubDivisionName("");
                                            if (n.getFirstChild() != null)
                                            {
                                                address.setCountrySubDivisionName(n.getFirstChild().getNodeValue());
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    return address;
                }

                /// <summary>
                /// returns a list of LineItems </summary>
                /// <returns> a List of LineItem instances </returns>
                public virtual IList<Item> getLineItemList()
                {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final java.util.List<org.w3c.dom.Node> nodeList = getLineItemNodes();
                    IList<Node> nodeList = getLineItemNodes();
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final java.util.List<org.mustangproject.Item> lineItemList = new java.util.ArrayList<>();
                    IList<Item> lineItemList = new List<Item>();

                    foreach (Node n in nodeList)
                    {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.mustangproject.Item lineItem = new org.mustangproject.Item(null, null, null);
                        Item lineItem = new Item(null, null, null);
                        lineItem.setProduct(new Product(null,null,null,null));

        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.w3c.dom.NodeList nl = n.getChildNodes();
                        NodeList nl = n.getChildNodes();
                        for (int i = 0; i < nl.getLength(); i++)
                        {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.w3c.dom.Node nn = nl.item(i);
                            Node nn = nl.item(i);
                            Node node = null;
                            if (nn.getLocalName() != null)
                            {
                                switch (nn.getLocalName())
                                {
                                    case "SpecifiedLineTradeAgreement":
                                    case "SpecifiedSupplyChainTradeAgreement":

                                        node = getNodeByName(nn.getChildNodes(), "NetPriceProductTradePrice");
                                        if (node != null)
                                        {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.w3c.dom.NodeList tradeAgreementChildren = node.getChildNodes();
                                            NodeList tradeAgreementChildren = node.getChildNodes();
                                            node = getNodeByName(tradeAgreementChildren, "ChargeAmount");
                                            lineItem.setPrice(trydecimal(getNodeValue(node)));
                                            node = getNodeByName(tradeAgreementChildren, "BasisQuantity");
                                            if (node != null && node.getAttributes() != null)
                                            {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.w3c.dom.Node unitCodeAttribute = node.getAttributes().getNamedItem("unitCode");
                                                Node unitCodeAttribute = node.getAttributes().getNamedItem("unitCode");
                                                if (unitCodeAttribute != null)
                                                {
                                                    lineItem.getProduct().setUnit(unitCodeAttribute.getNodeValue());
                                                }
                                            }
                                        }

                                        node = getNodeByName(nn.getChildNodes(), "GrossPriceProductTradePrice");
                                        if (node != null)
                                        {
                                            node = getNodeByName(node.getChildNodes(), "ChargeAmount");
                                            lineItem.setGrossPrice(trydecimal(getNodeValue(node)));
                                        }
                                        break;

                                    case "AssociatedDocumentLineDocument":

                                        node = getNodeByName(nn.getChildNodes(), "LineID");
                                        lineItem.setId(getNodeValue(node));
                                        break;

                                    case "SpecifiedTradeProduct":

                                        node = getNodeByName(nn.getChildNodes(), "SellerAssignedID");
                                        lineItem.getProduct().setSellerAssignedID(getNodeValue(node));

                                        node = getNodeByName(nn.getChildNodes(), "BuyerAssignedID");
                                        lineItem.getProduct().setBuyerAssignedID(getNodeValue(node));

                                        node = getNodeByName(nn.getChildNodes(), "Name");
                                        lineItem.getProduct().setName(getNodeValue(node));

                                        node = getNodeByName(nn.getChildNodes(), "Description");
                                        lineItem.getProduct().setDescription(getNodeValue(node));
                                        break;

                                    case "SpecifiedLineTradeDelivery":
                                    case "SpecifiedSupplyChainTradeDelivery":
                                        node = getNodeByName(nn.getChildNodes(), "BilledQuantity");
                                        lineItem.setQuantity(trydecimal(getNodeValue(node)));
                                        break;

                                    case "SpecifiedLineTradeSettlement":
                                        node = getNodeByName(nn.getChildNodes(), "ApplicableTradeTax");
                                        if (node != null)
                                        {
                                            node = getNodeByName(node.getChildNodes(), "RateApplicablePercent");
                                            lineItem.getProduct().setVATPercent(trydecimal(getNodeValue(node)));
                                        }

                                        node = getNodeByName(nn.getChildNodes(), "ApplicableTradeTax");
                                        if (node != null)
                                        {
                                            node = getNodeByName(node.getChildNodes(), "CalculatedAmount");
                                            lineItem.setTax(trydecimal(getNodeValue(node)));
                                        }

                                        node = getNodeByName(nn.getChildNodes(), "SpecifiedTradeSettlementLineMonetarySummation");
                                        if (node != null)
                                        {
                                            node = getNodeByName(node.getChildNodes(), "LineTotalAmount");
                                            lineItem.setLineTotalAmount(trydecimal(getNodeValue(node)));
                                        }
                                        break;
                                    case "SpecifiedSupplyChainTradeSettlement":
                                        //ZF 1!

                                        node = getNodeByName(nn.getChildNodes(), "ApplicableTradeTax");
                                        if (node != null)
                                        {
                                            node = getNodeByName(node.getChildNodes(), "ApplicablePercent");
                                            lineItem.getProduct().setVATPercent(trydecimal(getNodeValue(node)));
                                        }

                                        node = getNodeByName(nn.getChildNodes(), "ApplicableTradeTax");
                                        if (node != null)
                                        {
                                            node = getNodeByName(node.getChildNodes(), "CalculatedAmount");
                                            lineItem.setTax(trydecimal(getNodeValue(node)));
                                        }

                                        node = getNodeByName(nn.getChildNodes(), "SpecifiedTradeSettlementMonetarySummation");
                                        if (node != null)
                                        {
                                            node = getNodeByName(node.getChildNodes(), "LineTotalAmount");
                                            lineItem.setLineTotalAmount(trydecimal(getNodeValue(node)));
                                        }
                                        break;
                                }
                            }
                        }
                        lineItemList.Add(lineItem);
                    }
                    return lineItemList;
                }

                /// <summary>
                /// returns a List of LineItem Nodes from ZUGFeRD XML </summary>
                /// <returns> a List of Node instances </returns>
                public virtual IList<Node> getLineItemNodes()
                {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final java.util.List<org.w3c.dom.Node> lineItemNodes = new java.util.ArrayList<>();
                    IList<Node> lineItemNodes = new List<Node>();
                    NodeList nl = null;
                    try
                    {
                        if (getVersion() == 1)
                        {
                            nl = getNodeListByPath("//*[local-name() = 'CrossIndustryDocument']//*[local-name() = 'SpecifiedSupplyChainTradeTransaction']//*[local-name() = 'IncludedSupplyChainTradeLineItem']");
                        }
                        else
                        {
                            nl = getNodeListByPath("//*[local-name() = 'CrossIndustryInvoice']//*[local-name() = 'SupplyChainTradeTransaction']//*[local-name() = 'IncludedSupplyChainTradeLineItem']");
                        }
                    }
        //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
        //ORIGINAL LINE: catch (final Exception e)
                    catch (Exception e)
                    {
        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                        _logger.LogError(e.Message);
                    }

                    for (int i = 0; i < nl.getLength(); i++)
                    {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.w3c.dom.Node n = nl.item(i);
                        Node n = nl.item(i);
                        lineItemNodes.Add(n);
                    }
                    return lineItemNodes;
                }

                /// <summary>
                /// Returns a node, found by name. If more nodes with the same name are present, the first occurence will be returned </summary>
                /// <param name="nl"> - A NodeList which may contains the searched node </param>
                /// <param name="name"> The nodes name </param>
                /// <returns> a Node or null, if nothing is found </returns>
                private Node getNodeByName(NodeList nl, string name)
                {
                    for (int i = 0; i < nl.getLength(); i++)
                    {
                        if ((nl.item(i).getLocalName() != null) && (nl.item(i).getLocalName().Equals(name)))
                        {
                            return nl.item(i);
                        }
                        else if (nl.item(i).getChildNodes().getLength() > 0)
                        {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.w3c.dom.Node node = getNodeByName(nl.item(i).getChildNodes(), name);
                            Node node = getNodeByName(nl.item(i).getChildNodes(), name);
                            if (node != null)
                            {
                                return node;
                            }
                        }
                    }
                    return null;
                }

                /// <summary>
                /// Get a NodeList by providing an path </summary>
                /// <param name="path"> a compliable Path </param>
                /// <returns> a Nodelist or null, if an error occurs </returns>
                public virtual NodeList getNodeListByPath(string path)
                {

        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final javax.xml.xpath.XPathFactory xpathFact = javax.xml.xpath.XPathFactory.newInstance();
                    XPathFactory xpathFact = XPathFactory.newInstance();
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final javax.xml.xpath.XPath xPath = xpathFact.newXPath();
                    XPath xPath = xpathFact.newXPath();
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final String s = path;
                    string s = path;

                    try
                    {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final javax.xml.xpath.XPathExpression xpr = xPath.compile(s);
                        XPathExpression xpr = xPath.compile(s);
                        return (NodeList) xpr.evaluate(getDocument(), XPathConstants.NODESET);
                    }
        //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
        //ORIGINAL LINE: catch (final Exception e)
                    catch (Exception e)
                    {
        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                        _logger.LogError(e.Message);
                        return null;
                    }
                }

                /// <summary>
                /// returns the value of an node </summary>
                /// <param name="node"> the Node to get the value from </param>
                /// <returns> A String or empty String, if no value was found </returns>
                private string getNodeValue(Node node)
                {
                    if (node != null)
                    {
                        if (node.getFirstChild() != null)
                        {
                            return node.getFirstChild().getNodeValue();
                        }
                    }
                    return "";
                }

                /// <summary>
                /// tries to convert an String to decimal. </summary>
                /// <param name="nodeValue"> The value as String </param>
                /// <returns> a decimal with the value provides as String or a decimal with value 0.00 if an error occurs </returns>
                private decimal trydecimal(string nodeValue)
                {
                    try
                    {
                        return new decimal(nodeValue);
                    }
        //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
        //ORIGINAL LINE: catch (final Exception e)
                    catch (ception)
                    {
                        try
                        {
                            return new decimal(Convert.ToSingle(nodeValue));
                        }
        //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
        //ORIGINAL LINE: catch (final Exception ex)
                        catch (ception)
                        {
                            return new decimal("0.00");
                        }
                    }
                }*/
    }
}
