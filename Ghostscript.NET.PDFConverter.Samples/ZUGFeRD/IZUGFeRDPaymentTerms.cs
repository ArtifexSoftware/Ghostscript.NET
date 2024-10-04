using System;

namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{


    public interface IZUGFeRDPaymentTerms
    {

        string getDescription();

        DateTime getDueDate();

        //		IZUGFeRDPaymentDiscountTerms getDiscountTerms();
    }
}