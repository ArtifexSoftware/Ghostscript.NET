using System;

namespace Ghostscript.NET.FacturX.ZUGFeRD
{


    public interface IZUGFeRDPaymentTerms
    {

        string getDescription();

        DateTime getDueDate();

        //		IZUGFeRDPaymentDiscountTerms getDiscountTerms();
    }
}