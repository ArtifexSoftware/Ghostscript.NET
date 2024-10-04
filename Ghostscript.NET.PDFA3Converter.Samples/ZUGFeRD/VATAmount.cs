using System;

namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{

	public class VATAmount
	{

		public VATAmount(decimal basis, decimal calculated, string categoryCode) : base()
		{
			this.basis = basis;
			this.calculated = calculated;
			this.categoryCode = categoryCode;
		}

		internal decimal basis, calculated, applicablePercent;

		internal string categoryCode;

		public virtual decimal getApplicablePercent()
		{

			return applicablePercent;
		}
		public virtual VATAmount setApplicablePercent(decimal value)
		{
			this.applicablePercent = value;
			return this;
		}


		public virtual decimal getBasis()
		{

			return basis;
		}
		public virtual VATAmount setBasis(decimal value)
		{
			this.basis = Math.Round(value, 2, MidpointRounding.AwayFromZero);
			return this;

		}





		public virtual decimal getCalculated() {
			return calculated;
		}


		public virtual VATAmount setCalculated(decimal value) {

			this.calculated = value;
			return this;
		}


		/// 
		/// @deprecated Use <seealso cref="getCategoryCode() instead"/> 
		/// <returns> String with category code </returns>
		[Obsolete("Use <seealso cref=\"getCategoryCode() instead\"/>")]
		public virtual string getDocumentCode()
		{

			return categoryCode;
		}
		public virtual VATAmount setDocumentCode(string value)

		{
			this.categoryCode = value;
			return this;
		}



		public virtual string getCategoryCode()
		{

			return categoryCode;
		}
		public virtual VATAmount setCategoryCode(String value)
		{
			this.categoryCode = value;
			return this;
		}



		public virtual VATAmount add(VATAmount v)
		{
			return new VATAmount(basis + v.getBasis(), calculated + v.getCalculated(), this.categoryCode);
		}

		public virtual VATAmount subtract(VATAmount v)
		{
			return new VATAmount(basis - v.getBasis(), calculated - v.getCalculated(), this.categoryCode);
		}

	}
}