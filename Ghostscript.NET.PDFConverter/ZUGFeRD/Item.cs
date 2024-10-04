using System;
using System.Collections.Generic;

namespace Ghostscript.NET.FacturX.ZUGFeRD
{


    /// <summary>
    ///*
    /// describes any invoice line
    /// </summary>

    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @JsonIgnoreProperties(ignoreUnknown = true) public class Item implements org.mustangproject.ZUGFeRD.IZUGFeRDExportableItem
    public class Item : IZUGFeRDExportableItem
	{
		protected internal decimal price, quantity, tax, grossPrice, lineTotalAmount;
		protected internal DateTime? detailedDeliveryPeriodFrom = null, detailedDeliveryPeriodTo = null;
		protected internal string id;
		protected internal string referencedLineID = null;
		protected internal Product product;
		protected internal List<string> notes = null;
		//protected internal List<ReferencedDocument> referencedDocuments = null;
//		protected internal List<IZUGFeRDAllowanceCharge> Allowances = new List<IZUGFeRDAllowanceCharge>(), Charges = new List<IZUGFeRDAllowanceCharge>();

		/// <summary>
		///*
		/// default constructor </summary>
		/// <param name="product"> contains the products name, tax rate, and unit </param>
		/// <param name="price"> the base price of one item the product </param>
		/// <param name="quantity"> the number, dimensions or the weight of the delivered product or good in this context </param>
		public Item(Product product, decimal price, decimal quantity)
		{
			this.price = price;
			this.quantity = quantity;
			this.product = product;
		}


		/// <summary>
		///*
		/// empty constructor
		/// do not use, but might be used e.g. by jackson
		/// 
		/// </summary>
		public Item()
		{
		}

		public virtual Item addReferencedLineID(string s)
		{
			referencedLineID = s;
			return this;
		}

		/// <summary>
		///*
		/// BT 132 (issue https://github.com/ZUGFeRD/mustangproject/issues/247)
		/// @return
		/// </summary>
		public string getBuyerOrderReferencedDocumentLineID()
		{
			return referencedLineID;
		}

		public virtual decimal getLineTotalAmount()
		{
			return lineTotalAmount;
		}

		/// <summary>
		/// should only be set by calculator classes or maybe when reading from XML </summary>
		/// <param name="lineTotalAmount"> price*quantity of this line </param>
		/// <returns> fluent setter </returns>
		public virtual Item setLineTotalAmount(decimal lineTotalAmount)
		{
			this.lineTotalAmount = lineTotalAmount;
			return this;
		}

		public virtual decimal getGrossPrice()
		{
			return grossPrice;
		}


		/// <summary>
		///*
		/// the list price without VAT (sic!), refer to EN16931-1 for definition </summary>
		/// <param name="grossPrice"> the list price without VAT </param>
		/// <returns> fluent setter </returns>
		public virtual Item setGrossPrice(decimal grossPrice)
		{
			this.grossPrice = grossPrice;
			return this;
		}


		public virtual decimal getTax()
		{
			return tax;
		}

		public virtual Item setTax(decimal tax)
		{
			this.tax = tax;
			return this;
		}

		public virtual Item setId(string id)
		{
			this.id = id;
			return this;
		}

		public virtual string getId()
		{
			return id;
		}

		public decimal getPrice()
		{
			return price;
		}

		public virtual Item setPrice(decimal price)
		{
			this.price = price;
			return this;
		}



		public decimal getQuantity()
		{
			return quantity;
		}

		public virtual Item setQuantity(decimal quantity)
		{
			this.quantity = quantity;
			return this;
		}

		public IZUGFeRDExportableProduct getProduct()
		{
			return product;
		}

/*		public override IZUGFeRDAllowanceCharge[] getItemAllowances()
		{
			if (Allowances.Count == 0)
			{
				return null;
			}
			else
			{
				return Allowances.ToArray();
			}
		}

		public override IZUGFeRDAllowanceCharge[] getItemCharges()
		{
			if (Charges.Count == 0)
			{
				return null;
			}
			else
			{
				return Charges.ToArray();
			}
		}

*/

		public string[] getNotes()
		{
			if (notes == null)
			{
				return null;
			}
			return notes.ToArray();
		}

		public virtual Item setProduct(Product product)
		{
			this.product = product;
			return this;
		}


		/// <summary>
		///*
		/// Adds a item level addition to the price (will be multiplied by quantity) </summary>
		/// <seealso cref="org.mustangproject.Charge"/>
		/// <param name="izac"> a relative or absolute charge </param>
		/// <returns> fluent setter </returns>
/*		public virtual Item addCharge(IZUGFeRDAllowanceCharge izac)
		{
			Charges.Add(izac);
			return this;
		}

		/// <summary>
		///*
		/// Adds a item level reduction the price (will be multiplied by quantity) </summary>
		/// <seealso cref="org.mustangproject.Allowance"/>
		/// <param name="izac"> a relative or absolute allowance </param>
		/// <returns> fluent setter </returns>
		public virtual Item addAllowance(IZUGFeRDAllowanceCharge izac)
		{
			Allowances.Add(izac);
			return this;
		}
*/
		/// <summary>
		///*
		/// adds item level freetext fields (includednote) </summary>
		/// <param name="text"> UTF8 plain text </param>
		/// <returns> fluent setter </returns>
		public virtual Item addNote(string text)
		{
			if (notes == null)
			{
				notes = new List<string>();
			}
			notes.Add(text);
			return this;
		}

		/// <summary>
		///*
		/// adds item level Referenced documents along with their typecodes and issuerassignedIDs </summary>
		/// <param name="doc"> the ReferencedDocument to add </param>
		/// <returns> fluent setter </returns>
        /*
		public virtual Item addReferencedDocument(ReferencedDocument doc)
		{
			if (referencedDocuments == null)
			{
				referencedDocuments = new List<ReferencedDocument>();
			}
			referencedDocuments.Add(doc);
			return this;
		}

		public override IReferencedDocument[] getReferencedDocuments()
		{
			if (referencedDocuments == null)
			{
				return null;
			}
			return referencedDocuments.ToArray();
		}*/
		/// <summary>
		///*
		/// specify a item level delivery period
		/// (apart from the document level delivery period, and the document level
		/// delivery day, which is probably anyway required)
		/// </summary>
		/// <param name="from"> start date </param>
		/// <param name="to"> end date </param>
		/// <returns> fluent setter </returns>
		public virtual Item setDetailedDeliveryPeriod(DateTime from, DateTime to)
		{
			detailedDeliveryPeriodFrom = from;
			detailedDeliveryPeriodTo = to;
			return this;
		}

		/// <summary>
		///*
		/// specifies the item level delivery period (there is also one on document level),
		/// this will be included in a BillingSpecifiedPeriod element </summary>
		/// <returns> the beginning of the delivery period </returns>
		public virtual DateTime? getDetailedDeliveryPeriodFrom()
		{
			return detailedDeliveryPeriodFrom;
		}

		/// <summary>
		///*
		/// specifies the item level delivery period (there is also one on document level),
		/// this will be included in a BillingSpecifiedPeriod element </summary>
		/// <returns> the end of the delivery period </returns>
		public virtual DateTime? getDetailedDeliveryPeriodTo()
		{
			return detailedDeliveryPeriodTo;
		}

	}
}
