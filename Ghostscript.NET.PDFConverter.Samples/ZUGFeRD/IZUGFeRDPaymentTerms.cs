using System;

namespace Ghostscript.NET.PDFConverter.Samples.ZUGFeRD
{


    public interface IZUGFeRDPaymentTerms
    {

        string getDescription();

        DateTime getDueDate();

        //		IZUGFeRDPaymentDiscountTerms getDiscountTerms();
    }
}