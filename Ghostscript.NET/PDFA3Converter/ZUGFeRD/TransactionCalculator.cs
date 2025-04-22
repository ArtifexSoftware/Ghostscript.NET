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

namespace Ghostscript.NET.PDFA3Converter.ZUGFeRD
{

    public class TransactionCalculator
    {
        protected internal IExportableTransaction trans;

        /// <summary>
        ///*
        /// </summary>
        /// <param name="trans"> the invoice (or IExportableTransaction) to be calculated </param>
        public TransactionCalculator(IExportableTransaction trans)
        {
            this.trans = trans;
        }

        /// <summary>
        ///*
        /// if something had already been paid in advance, this will get it from the transaction </summary>
        /// <returns> prepaid amount </returns>
        protected internal virtual decimal getTotalPrepaid()
        {
            if (trans.getTotalPrepaidAmount() == decimal.MinValue)
            {
                return decimal.Zero;
            }
            else
            {
                return Math.Round(trans.getTotalPrepaidAmount(), 2, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        ///*
        /// the invoice total with VAT, corrected by prepaid amount, allowances and charges </summary>
        /// <returns> the invoice total including taxes </returns>
        public virtual decimal getGrandTotal()
        {

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            decimal res = getTaxBasis();
            Dictionary<decimal, VATAmount> VATPercentAmountMap = getVATPercentAmountMap();
            foreach (decimal currentTaxPercent in VATPercentAmountMap.Keys)
            {
                VATAmount amount = VATPercentAmountMap[currentTaxPercent];
                res = res += amount.getCalculated();
            }
            return Math.Round(res, 2, MidpointRounding.AwayFromZero);

        }

        /*/// <summary>
        ///*
        /// returns total of charges for this tax rate </summary>
        /// <param name="percent"> a specific rate, or null for any rate </param>
        /// <returns> the total amount </returns>

        
        protected internal virtual decimal getChargesForPercent(decimal percent)
        {
            IZUGFeRDAllowanceCharge[] charges = trans.getZFCharges();
            return sumAllowanceCharge(percent, charges);
        }

        private decimal sumAllowanceCharge(decimal percent, IZUGFeRDAllowanceCharge[] charges)
        {
            decimal res = decimal.ZERO;
            if ((charges != null) && (charges.Length > 0))
            {
                foreach (IZUGFeRDAllowanceCharge currentCharge in charges)
                {
                    if ((percent == null) || (currentCharge.getTaxPercent().compareTo(percent) == 0))
                    {
                        res = res + currentCharge.getTotalAmount(this);
                    }
                }
            }
            return res;
        }

        /// <summary>
        ///*
        /// returns a (potentially concatenated) string of charge reasons, or "Charges" if none are defined </summary>
        /// <param name="percent"> a specific rate, or null for any rate </param>
        /// <returns> the space separated String </returns>
        protected internal virtual string getChargeReasonForPercent(decimal percent)
        {
            IZUGFeRDAllowanceCharge[] charges = trans.getZFCharges();
            string res = getAllowanceChargeReasonForPercent(percent, charges);
            if ("".Equals(res))
            {
                res = "Charges";
            }
            return res;
        }

        private string getAllowanceChargeReasonForPercent(decimal percent, IZUGFeRDAllowanceCharge[] charges)
        {
            string res = " ";
            if ((charges != null) && (charges.Length > 0))
            {
                foreach (IZUGFeRDAllowanceCharge currentCharge in charges)
                {
                    if ((percent == null) || (currentCharge.getTaxPercent().compareTo(percent) == 0) && currentCharge.getReason() != null)
                    {
                        res += currentCharge.getReason() + " ";
                    }
                }
            }
            res = res.Substring(0, res.Length - 1);
            return res;
        }

        /// <summary>
        ///*
        /// returns a (potentially concatenated) string of allowance reasons, or "Allowances", if none are defined </summary>
        /// <param name="percent"> a specific rate, or null for any rate </param>
        /// <returns> the space separated String </returns>
        protected internal virtual string getAllowanceReasonForPercent(decimal percent)
        {
            IZUGFeRDAllowanceCharge[] allowances = trans.getZFAllowances();
            string res = getAllowanceChargeReasonForPercent(percent, allowances);
            if ("".Equals(res))
            {
                res = "Allowances";
            }
            return res;
        }


        /// <summary>
        ///*
        /// returns total of allowances for this tax rate </summary>
        /// <param name="percent"> a specific rate, or null for any rate </param>
        /// <returns> the total amount </returns>
        protected internal virtual decimal getAllowancesForPercent(decimal percent)
        {
            IZUGFeRDAllowanceCharge[] allowances = trans.getZFAllowances();
            return sumAllowanceCharge(percent, allowances);
        }
        */

        /// <summary>
        ///*
        /// returns the total net value of all items, without document level charges/allowances </summary>
        /// <returns> item sum </returns>
        protected internal virtual decimal getTotal()
        {
            //JAVA TO C# CONVERTER TODO TASK: Method reference constructor syntax is not converted by Java to C# Converter:

            decimal res = decimal.Zero;
            foreach (IZUGFeRDExportableItem currentItem in trans.getZFItems())
            {
                LineCalculator lc = new LineCalculator(currentItem);
                res += lc.getItemTotalGrossAmount();
            }
            return res;



        }

        /// <summary>
        ///*
        /// returns the total net value of the invoice, including charges/allowances on document
        /// level </summary>
        /// <returns> item sum +- charges/allowances </returns>
        protected internal virtual decimal getTaxBasis()
        {
            return getTotal() /*+ (getChargesForPercent(null).setScale(2, RoundingMode.HALF_UP)).subtract(getAllowancesForPercent(null).setScale(2, RoundingMode.HALF_UP)).setScale(2, RoundingMode.HALF_UP)*/;
        }

        /// <summary>
        /// which taxes have been used with which amounts in this transaction, empty for
        /// no taxes, or e.g. 19:190 and 7:14 if 1000 Eur were applicable to 19% VAT
        /// (=190 EUR VAT) and 200 EUR were applicable to 7% (=14 EUR VAT) 190 Eur
        /// </summary>
        /// <returns> which taxes have been used with which amounts in this invoice </returns>
        protected internal virtual Dictionary<decimal, VATAmount> getVATPercentAmountMap()
        {
            Dictionary<decimal, VATAmount> hm = new Dictionary<decimal, VATAmount>();

            foreach (IZUGFeRDExportableItem currentItem in trans.getZFItems())
            {
                decimal percent = currentItem.getProduct().getVATPercent();

                LineCalculator lc = new LineCalculator(currentItem);
                VATAmount itemVATAmount = new VATAmount(lc.getItemTotalNetAmount(), lc.getItemTotalVATAmount(), currentItem.getProduct().getTaxCategoryCode());


                if (!hm.ContainsKey(percent))
                {
                    hm[percent] = itemVATAmount;
                }
                else
                {
                    VATAmount current = hm[percent];
                    hm[percent] = current.add(itemVATAmount);
                }
            }

            /*
                    IZUGFeRDAllowanceCharge[] charges = trans.getZFCharges();
                    if ((charges != null) && (charges.Length > 0))
                    {
                        foreach (IZUGFeRDAllowanceCharge currentCharge in charges)
                        {
                            VATAmount theAmount = hm[currentCharge.getTaxPercent().stripTrailingZeros()];
                            if (theAmount == null)
                            {
                                theAmount = new VATAmount(decimal.Zero, decimal.Zero, currentCharge.getCategoryCode() != null ? currentCharge.getCategoryCode() : "S");
                            }
                            theAmount.setBasis(theAmount.getBasis().add(currentCharge.getTotalAmount(this)));
                            decimal factor = currentCharge.getTaxPercent().divide(new decimal(100));
                            theAmount.setCalculated(theAmount.getBasis().multiply(factor));
                            hm[currentCharge.getTaxPercent().stripTrailingZeros()] = theAmount;
                        }
                    }
                    IZUGFeRDAllowanceCharge[] allowances = trans.getZFAllowances();
                    if ((allowances != null) && (allowances.Length > 0))
                    {
                        foreach (IZUGFeRDAllowanceCharge currentAllowance in allowances)
                        {
                            VATAmount theAmount = hm[currentAllowance.getTaxPercent().stripTrailingZeros()];
                            if (theAmount == null)
                            {
                                theAmount = new VATAmount(decimal.ZERO, decimal.ZERO, currentAllowance.getCategoryCode() != null ? currentAllowance.getCategoryCode() : "S");
                            }
                            theAmount.setBasis(theAmount.getBasis().subtract(currentAllowance.getTotalAmount(this)));
                            decimal factor = currentAllowance.getTaxPercent().divide(new decimal(100));
                            theAmount.setCalculated(theAmount.getBasis().multiply(factor));

                            hm[currentAllowance.getTaxPercent().stripTrailingZeros()] = theAmount;
                        }
                    }*/

            return hm;

        }


        public decimal getValue()
        {
            return getTotal();
        }

    }
}