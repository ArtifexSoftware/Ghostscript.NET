
using java.math;
namespace Ghostscript.NET.FacturX.ZUGFeRD
{

	public class LineCalculator
	{
		private BigDecimal price;
		private BigDecimal priceGross;
		private BigDecimal itemTotalNetAmount;
		private BigDecimal itemTotalVATAmount;
		private BigDecimal allowance = BigDecimal.ZERO;
		private BigDecimal charge = BigDecimal.ZERO;

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
			BigDecimal multiplicator = currentItem.getProduct().getVATPercent().divide(new BigDecimal(100));
			priceGross = currentItem.getPrice(); // see https://github.com/ZUGFeRD/mustangproject/issues/159
			price = priceGross.subtract(allowance).add(charge);
			itemTotalNetAmount = currentItem.getQuantity().multiply(price).divide(currentItem.getBasisQuantity()).setScale(2, BigDecimal.ROUND_HALF_UP);
			itemTotalVATAmount = itemTotalNetAmount.multiply(multiplicator);

		}


		public virtual BigDecimal getPrice()
		{

			return price;

		}

		public virtual BigDecimal getItemTotalNetAmount()
		{

			return itemTotalNetAmount;

		}

		public virtual BigDecimal getItemTotalVATAmount()
		{
			return itemTotalVATAmount;
		}

		public virtual BigDecimal getItemTotalGrossAmount()
		{
			return itemTotalNetAmount;
		}

		public virtual BigDecimal getPriceGross()
		{
			return priceGross;
		}

		public virtual void addAllowance(BigDecimal b)
		{
			allowance = allowance.add(b);
		}

		public virtual void addCharge(BigDecimal b)
		{
			charge = charge.add(b);
		}


	}
}