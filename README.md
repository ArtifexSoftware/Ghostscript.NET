**Ghostscript.NET** - (written in C#) is the most completed managed wrapper library around the Ghostscript library (32-bit & 64-bit), an interpreter for the PostScript language, PDF, related software and documentation.

**Note**: This project currently has only been tested with versions of Ghostscript < 10, see [release versions](https://github.com/ArtifexSoftware/ghostpdl-downloads/releases).

[**NuGet: PM> Install-Package Ghostscript.NET**](http://nuget.org/packages/Ghostscript.NET/)

**Contains**
 * GhostscriptViewer - View PDF, EPS or multi-page PostScript files on the screen
 * GhostscriptRasterizer - Rasterize PDF, EPS or multi-page PostScript files to any common image format.
 * GhostscriptProcessor - An easy way to call a Ghostscript library with a custom arguments / switches.
 * GhostscriptInterpreter - The PostScript interpreter.

**Other features**
 * allows you to rasterize files in memory without storing the output to disk.
 * supports zoom-in and zoom-out.
 * supports progressive update.
 * allows you to run multiple Ghostscript instances simultaneously within a single process.
 * compatible with 32-bit and 64-bit Ghostscript native library.

**Latest changes - 2021-03-09 - v.1.2.3.**
* fixed GhostscriptRasterizer/GhostscriptViewer and Ghostscript v.9.50+ compatibility issues.

**Latest changes - 2021-02-04 - v.1.2.2.**
 * fixed Ghostscript v.9.26 + (all later versions) compatibility.
 * fixed problem when opening path/file that contains non ASCII characters.
 * fixed "Arithmetic operation resulted in an overflow" when using multithread instance.
 * changed Y and Y DPI settings to match GhostscriptViewer.
 * fixed CurrentPage -> TotalPages logging.
 * fixed watermark transparency bug for PDF.
 
**Samples built on the top of the Ghostscript.NET library**

Direct postscript interpretation via Ghostscript.NET:

![Ghostscript.NET.Display](https://i.ibb.co/Fnk8rFP/ss-jj-1899.png)

Ghostscript.NET.Viewer (supports viewing of the PDF, EPS and multi-page PS files):

![Ghostscript.NET.Viewer](http://a.fsdn.com/con/app/proj/ghostscriptnet/screenshots/gs-net-render.png)

# PDF A/3 Conversion Using Ghostscript.NET.PDFA3Converter

The `Ghostscript.NET.PDFA3Converter` extension of the Ghostscript.NET library simplifies converting existing PDF files into PDF/A-3 format. This format is particularly useful for embedding XML-based representations of invoices, such as those used in **XRechnung** and **Factur-X** standards.

**_NOTE:_** Please ensure to tag [stephanstapel](https://github.com/stephanstapel) with any [issues](https://github.com/ArtifexSoftware/Ghostscript.NET/issues) regarding the PDF A/3 converter.

## Key Features
- Supports the embedding of XML-based invoices into PDF/A-3 format.
- Includes necessary ICC profiles and pdfMark templates for compliance with PDF/A-3 standards.
- Designed to support electronic invoicing initiatives such as **XRechnung** and **Factur-X**.

## Basic PDF A/3 Conversion

To convert an existing PDF file to PDF/A-3, the following code snippet demonstrates the process:

```csharp
PDFA3Converter converter = new PDFA3Converter(@"%PROGRAM FILES%\gs\gs9.56.1\bin\gsdll64.dll"); // Specify the Ghostscript DLL path
converter.ConvertToPDFA3(@"sample-invoice.pdf", @"sample-invoice-pdfa3.pdf"); // Convert input PDF to PDF/A-3
```

This method will generate a plain PDF/A-3 file without any embedded XML invoice.

## Embedding XML Invoices (ZUGFeRD / Factur-X)
The primary use case for PDF/A-3 in Europe involves embedding XML invoices within a PDF document, providing both machine-readable (XML) and human-readable (PDF) representations. This includes initiatives such as [XRechnung](https://de.wikipedia.org/wiki/XRechnung) and [Factur-X](http://fnfe-mpe.org/factur-x/factur-x_en/).

To understand how the Ghostscript.NET.PDFA3Converter extension works, the Ghostscript.NET.PDFA3Converter.Samples project includes a couple of samples along with a sample implementation of a ZUGFeRD generator, generated out of the [Mustang project](https://www.mustangproject.org). More ZUGFeRD/ XRechnung generators are available, for example [ZUGFeRD-csharp](https://github.com/stephanstapel/ZUGFeRD-csharp).

### Step 1: Generate the XML Invoice
You can generate an XML invoice using either the Mustang sample implementation or the ZUGFeRD-csharp library.

In order to generate the xml invoice, you can use either the Mustang sample implementation like this:

```csharp
Invoice i = (new Invoice()).setDueDate(DateTime.Now).setIssueDate(DateTime.Now).setDeliveryDate(DateTime.Now).setSender((new TradeParty("Test company", "Test Street 1", "55232", "Test City", "DE")).addTaxID("DE4711").addVATID("DE0815").setContact(new Contact("Hans Test", "+49123456789", "test@example.org")).addBankDetails(new BankDetails("DE12500105170648489890", "COBADEFXXX"))).setRecipient(new TradeParty("Franz Müller", "Test Steet 12", "55232", "Entenhausen", "DE")).setReferenceNumber("991-01484-64").setNumber("123").
        addItem(new Item(new Product("Test product", "", "C62", 19m), 1.0m, 1.0m));

ZUGFeRD2PullProvider zf2p = new ZUGFeRD2PullProvider();
zf2p.setProfile(Profiles.getByName("XRechnung"));
zf2p.generateXML(i);
System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

string outfilename = "factur-x.xml";
File.WriteAllBytes(outfilename, zf2p.getXML());
```

Using ZUGFeRD-csharp:

```csharp
string outFilename = "factur-x.xml";
InvoiceDescriptor invoice = CreateInvoice(); // please see the sample project for details on how the invoice structure is created
invoice.Save(outFilename, ZUGFeRDVersion.Version23, s2industries.ZUGFeRD.Profile.Comfort);
```

Both examples output the XML invoice as ```factur-x.xml```.

### Step 2: Embed the XML Invoice into the PDF/A-3 File

Once the XML invoice is generated, it can be embedded into a PDF/A-3 file using the ```PDFA3Converter``` class. The converter also supports tagging the file with the correct ZUGFeRD profile and version:

```csharp
PDFA3Converter converter = new PDFA3Converter(@"%PROGRAM FILES%\gs9.56.1\bin\gsdll64.dll");
converter.SetZUGFeRDProfile(...);
converter.SetZUGFeRDVersion("2.3");
converter.SetEmbeddedXMLFile(outfilename); // the xml invoice file that was just generated
converter.ConvertToPDFA3(@"sample-invoice.pdf", @"sample-invoice-pdfa3.pdf");
```


# License and Copyright

Available under both, open-source AGPL and commercial license agreements.

Please read the full text of the [AGPL license agreement](https://www.gnu.org/licenses/agpl-3.0.html) (which is also included here in file COPYING) to ensure that your use case complies with the guidelines of this license. If you determine you cannot meet the requirements of the AGPL, please contact [Artifex](https://artifex.com/contact/ghostscript-inquiry.php) for more information regarding a commercial license.

Artifex is the exclusive commercial licensing agent for Ghostscript.


