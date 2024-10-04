
namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{

	public class LineCalculator
	{
		private decimal price;
		private decimal priceGross;
		private decimal itemTotalNetAmount;
		private decimal itemTotalVATAmount;
		private decimal allowance = decimal.Zero;
		private decimal charge = decimal.Zero;

		//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
		//ORIGINAL LINE: public LineCalculator(IZUGFeRDExportableItem currentItem)
		public LineCalculator(IZUGFeRDExportableItem currentItem)
		{
			/*
					if (currentItem.getItemAllowances() != null && currentItem.getItemAllowances().length > 0)
					{
			//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			//ORIGINAL LINE: foreach(IZUGFeRDAllowanceCharge allowance @in currentItem.getItemAllowances())

						{
							addAllowance(allowance.getTotalAmount(currentItem));
						}
					}
					if (currentItem.getItemCharges() != null && currentItem.getItemCharges().length > 0)
					{
			//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			//ORIGINAL LINE: foreach(IZUGFeRDAllowanceCharge charge @in currentItem.getItemCharges())

						{
							addCharge(charge.getTotalAmount(currentItem));
						}
					}
			//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			//ORIGINAL LINE: @decimal multiplicator = currentItem.getProduct().getVATPercent().divide(100);
			*/
			decimal multiplicator = currentItem.getProduct().getVATPercent() / 100m;
			priceGross = currentItem.getPrice(); // see https://github.com/ZUGFeRD/mustangproject/issues/159
			price = priceGross - allowance + charge;
			itemTotalNetAmount = Math.Round(price * currentItem.getQuantity() / currentItem.getBasisQuantity(), 2, MidpointRounding.AwayFromZero);
			itemTotalVATAmount = itemTotalNetAmount * multiplicator;

		}


		public virtual decimal getPrice()
		{

			return price;

		}

		public virtual decimal getItemTotalNetAmount()
		{

			return itemTotalNetAmount;

		}

		public virtual decimal getItemTotalVATAmount()
		{
			return itemTotalVATAmount;
		}

		public virtual decimal getItemTotalGrossAmount()
		{
			return itemTotalNetAmount;
		}

		public virtual decimal getPriceGross()
		{
			return priceGross;
		}

		public virtual void addAllowance(decimal b)
		{
			allowance += b;
		}

		public virtual void addCharge(decimal b)
		{
			charge += b;
		}


	}
}