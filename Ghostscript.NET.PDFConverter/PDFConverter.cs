using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.FileProviders;

using Ghostscript.NET.FacturX.ZUGFeRD;


namespace Ghostscript.NET.PDFConverter
{
    /// <summary>
    /// This class allows to convert an existing PDF file into PDF A/3 with ZUGFeRD/ XRechnung attached to it.
    /// </summary>
    public class PDFConverter
    {
        private readonly string? AdobeICCFilePath = null;
        private readonly string? PostScriptBigScriptPath = null;

        private readonly string TemporaryDirectory = Path.GetTempPath();        

        protected string? GSDLLPath = null;
        protected string? xmlInvoicePath = null;
        protected string? ZUGFeRDVersion = null;
        protected string? ZUGFeRDProfile = null;


        /// <summary>
        /// The constructor of the class accepts both input and output path for PDF conversion.
        /// </summary>
        /// <param name="sourcePDFPath">PDF input path </param>
        /// <param name="targetPDFPath">PDF-A/3 output path</param>        
        public PDFConverter(String gsdll)
        {
            GSDLLPath = gsdll;
            AdobeICCFilePath = this.TemporaryDirectory + "\\AdobeRGB1998.icc";
            PostScriptBigScriptPath = this.TemporaryDirectory + "\\pdfconvert.ps";
        }


        public PDFConverter SetZUGFeRDVersion(string zugferdVersion) 
        {
            ZUGFeRDVersion = zugferdVersion;
            return this;        
        }


        public PDFConverter SetZUGFeRDProfile(string profile) 
        {
            ZUGFeRDProfile = profile;
            return this;
        }


        /// <summary>
        /// Reads the xml file from the given path in order to embed it into the PDF A/3 file.
        /// </summary>
        ///     ''' <param name="xmlFilePath"></param>       
        public void SetEmbeddedXMLFile(string xmlFilePath)
        {
            xmlInvoicePath = xmlFilePath;
        } // !SetEmbeddedXMLFile()



        public void PrepareICC()
        {
            string tempfilename = Path.GetTempPath() + "AdobeRGB1998.icc";
            StoreEmbeddedResourceLocally("assets\\AdobeCompat-v2.icc", tempfilename);
        } // !PrepareICC()


        private byte[] LoadEmbeddedResource(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using (Stream stream = assembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    return null;
                }                    

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        } // !LoadEmbeddedResource()


        private bool StoreEmbeddedResourceLocally(string resourcePath, string localPath)
        {
            byte[] data = LoadEmbeddedResource(resourcePath);
            if (data == null)
            {
                return false;
            }

            System.IO.File.WriteAllBytes(localPath, data);
            return true;
        } // !StoreEmbeddedResourceLocally()


        /// <summary>
        /// Creates a Postscript file or - more exact: PDFMark file - which is interpreted by Ghostscript and converts file into PDF-A/3
        /// </summary>        
        private void WritePDFMark()
        {                       
            if (String.IsNullOrWhiteSpace(xmlInvoicePath))
            {
                throw new Exception("No valid xml invoice path. Aborting.");
            }

            if (!File.Exists(xmlInvoicePath))
            {
                throw new FileNotFoundException(xmlInvoicePath);
            }
            
            if (!File.Exists(AdobeICCFilePath))
            {
                throw new FileNotFoundException(AdobeICCFilePath);
            }

            string PDFmark = System.Text.Encoding.Default.GetString(LoadEmbeddedResource("Ghostscript.NET.PDFConverter.assets.pdfMarkA3.template"));
            PDFmark = PDFmark.Replace("{{EscapedEmbeddedICCFile}}", AdobeICCFilePath.Replace(@"\", @"\\")); // properly escape path for pdfmark

            if (xmlInvoicePath != null)
            {
                FileInfo fi = new FileInfo(xmlInvoicePath);
                string PDFmarkZUGFeRD = System.Text.Encoding.Default.GetString(LoadEmbeddedResource("Ghostscript.NET.PDFConverter.assets.pdfMarkZUGFeRD.template"));
                PDFmarkZUGFeRD = PDFmarkZUGFeRD.Replace("{{Date}}", DateTime.Now.ToString("yyyyMMddHHmmssK").Replace(":", "'"));
                PDFmarkZUGFeRD = PDFmarkZUGFeRD.Replace("{{EscapedEmbeddedXMLFile}}", xmlInvoicePath.Replace(@"\", @"\\")); // properly escape path for pdfpark
                PDFmarkZUGFeRD = PDFmarkZUGFeRD.Replace("{{SizeInBytes}}", fi.Length.ToString());
                PDFmarkZUGFeRD = PDFmarkZUGFeRD.Replace("{{FXVersion}}", ZUGFeRDVersion);
                PDFmarkZUGFeRD = PDFmarkZUGFeRD.Replace("{{FXComformanceLevel}}", ZUGFeRDProfile);
                PDFmark += PDFmarkZUGFeRD;
            }

            UTF8Encoding utf8 = new UTF8Encoding(false); // do not use BOM s. https://docs.microsoft.com/de-de/dotnet/api/system.text.utf8encoding?view=netcore-3.1
            File.WriteAllBytes(PostScriptBigScriptPath, utf8.GetBytes(PDFmark));
        } // !WritePDFMark()


        // <summary>
        // Checks if the output file can be written at all
        // </summary>
        public bool IsFileLocked(FileInfo file)
        {
            FileStream stream = (FileStream)null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException generatedExceptionName)
            {
                // handle the exception your way
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return false;
        } // !IsFileLocked()


        /// <summary>
        /// Converts a PDF file into PDF A/3 and attaches the given xml invoice
        /// 
        /// <param name="sourcePDFPath">PDF input path </param>
        /// <param name="targetPDFPath">PDF-A/3 output path</param>  
        /// <returns>True if successful, false otherwise</returns>
        public bool ConvertToPDFA3(string sourcePDFPath, string targetPDFPath)
        {
            // based on https://github.com/jhabjan/Ghostscript.NET/blob/master/Ghostscript.NET.Samples/Samples/ProcessorSample1.cs
            if (!File.Exists(GSDLLPath))
            {
                throw new FileNotFoundException(GSDLLPath);
            }                     

            if (!File.Exists(sourcePDFPath))
            {
                throw new FileNotFoundException(sourcePDFPath);
            }

            if (sourcePDFPath.Equals(targetPDFPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Source file and target file cannot be the same");
            }

            if (File.Exists(targetPDFPath))
            {
                if (FileSystem.GetAttr(targetPDFPath) == Constants.vbReadOnly)
                {
                    throw new UnauthorizedAccessException(targetPDFPath);
                }

                FileInfo fi = new FileInfo(targetPDFPath);
                if (IsFileLocked(fi))
                {
                    throw new Exception($"File {targetPDFPath} cannot be written. Might be opened/ locked.");
                }
            }

            PrepareICC();
            WritePDFMark();

            GhostscriptVersionInfo? gsVersion = new GhostscriptVersionInfo(GSDLLPath);
            GhostscriptLibrary ghostscriptLibrary = new GhostscriptLibrary(gsVersion);
            GhostscriptPipedOutput gsPipedOutput = new GhostscriptPipedOutput();

            List<string> switches = new List<string>();            
            switches.Add(""); // first parameter might be ignored 
            switches.Add("-P"); // allow access to resources within the current directory
            switches.Add("-dPDFA=3"); // convert to A/3 pat 1/3
                                      // switches.Add("-dCompressStreams=false") hat problems as apparently XMP Metadata was compressed by ZUGFeRD. Obsolete in the meantime
            switches.Add("-sColorConversionStrategy=RGB"); // necessary for PDF/A conversion
            switches.Add("-sDEVICE=pdfwrite"); // Device for rasterization. Mandatory
            switches.Add($"-o{targetPDFPath}"); // Output path
            switches.Add("-dPDFACompatibilityPolicy=1"); // convert to A/3 part 2/3
            switches.Add("-dRenderIntent=3"); // convert to A/3 part 3/3            
            switches.Add(PostScriptBigScriptPath); // PDFMark program file that shall be interpreted.
                                             // see https://www.adobe.com/content/dam/acom/en/devnet/acrobat/pdfs/pdfmark_reference.pdf
                                             // and https://gitlab.com/crossref/pdfmark
            switches.Add(sourcePDFPath); // PDF input file

            bool success = false;
            using (GhostscriptProcessor processor = new GhostscriptProcessor(ghostscriptLibrary))
            {
                VerboseMsgBoxOutput stdio = new VerboseMsgBoxOutput();
                processor.StartProcessing(switches.ToArray(), stdio);

                // (erfolglose) Versuche, das Hngen zu vermeiden...
                processor.Dispose();
            }
            success = true;
            return success;
        } // !ConvertToPDFA3()


        // <summary>
        // In order to debug Ghostscript it is helpful to direct msgbox output
        // Known error codes: -100: file not found or canot be written
        // </summary>
        public class VerboseMsgBoxOutput : GhostscriptStdIO
        {
            public VerboseMsgBoxOutput() : base(true, true, true)
            {
            }

            public override void StdOut(string output)
            {
                Console.Write("Out:" + output);
            }

            public override void StdError(string error)
            {
                Console.Write("Error:" + error);
            }

            public override void StdIn(out string input, int count)
            {
                input = "debug input";
            }
        }
    } }
