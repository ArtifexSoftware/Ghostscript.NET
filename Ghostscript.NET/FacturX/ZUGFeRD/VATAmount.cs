using System;

using java.math;
namespace Ghostscript.NET.FacturX.ZUGFeRD
{

	public class VATAmount
	{

		public VATAmount(BigDecimal basis, BigDecimal calculated, string categoryCode) : base()
		{
			this.basis = basis;
			this.calculated = calculated;
			this.categoryCode = categoryCode;
		}

		internal BigDecimal basis, calculated, applicablePercent;

		internal string categoryCode;

		public virtual BigDecimal getApplicablePercent()
		{

			return applicablePercent;
		}
		public virtual VATAmount setApplicablePercent(BigDecimal value)
		{
			this.applicablePercent = value;
			return this;
		}


		public virtual BigDecimal getBasis()
		{

			return basis;
		}
		public virtual VATAmount setBasis(BigDecimal value)
		{
			this.basis = value.setScale(2, RoundingMode.HALF_UP);
			return this;

		}





		public virtual BigDecimal getCalculated() {
			return calculated;
		}


		public virtual VATAmount setCalculated(BigDecimal value) {

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
			return new VATAmount(basis.add(v.getBasis()), calculated.add(v.getCalculated()), this.categoryCode);
		}

		public virtual VATAmount subtract(VATAmount v)
		{
			return new VATAmount(basis.subtract(v.getBasis()), calculated.subtract(v.getCalculated()), this.categoryCode);
		}

	}
}