namespace Ghostscript.NET.FacturX.ZUGFeRD
{

	public interface IZUGFeRDExportableContact
	{

		/// <summary>
		/// customer identification assigned by the seller
		/// </summary>
		/// <returns> customer identification </returns>
		string getID()
		{
			return null;
		}


		/// <summary>
		/// First and last name of the recipient
		/// </summary>
		/// <returns> First and last name of the recipient </returns>
		string? getName()
		{
			return null;
		}

		string getPhone()
		{
			return null;
		}

		string getEMail()
		{
			return null;
		}

		string getFax()
		{
			return null;
		}


		/// <summary>
		/// Postal code of the recipient
		/// </summary>
		/// <returns> Postal code of the recipient </returns>
		string getZIP()
		{
			return null;
		}


		/// <summary>
		/// VAT ID (Umsatzsteueridentifikationsnummer) of the contact
		/// </summary>
		/// <returns> VAT ID (Umsatzsteueridentifikationsnummer) of the contact </returns>
		string getVATID()
		{
			return null;
		}


		/// <summary>
		/// two-letter country code of the contact
		/// </summary>
		/// <returns> two-letter iso country code of the contact </returns>
		string getCountry()
		{
			return null;
		}


		/// <summary>
		/// Returns the city of the contact
		/// </summary>
		/// <returns> Returns the city of the recipient </returns>
		string getLocation()
		{
			return null;
		}


		/// <summary>
		/// Returns the street address (street+number) of the contact
		/// </summary>
		/// <returns> street address (street+number) of the contact </returns>
		string getStreet()
		{
			return null;
		}

		/// <summary>
		/// returns additional address information which is display in xml tag "LineTwo"
		/// </summary>
		/// <returns> additional address information </returns>
		string getAdditionalAddress()
		{
			return null;
		}

	}
}