namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{

	public interface IZUGFeRDExportableTradeParty
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
		/// customer global identification assigned by the seller
		/// </summary>
		/// <returns> customer identification </returns>
		string getGlobalID()
		{
			return null;
		}

		/// <summary>
		///*
		/// gets the official representation </summary>
		/// <returns> the interface with the attributes of the legal organisation </returns>
		/*	IZUGFeRDLegalOrganisation getLegalOrganisation()
			{
				return null;
			}
			*/
		/// <summary>
		/// customer global identification scheme
		/// </summary>
		/// <returns> customer identification </returns>
		string getGlobalIDScheme()
		{
			return null;
		}


		IZUGFeRDExportableContact getContact()
		{
			return null;
		}

		/// <summary>
		/// First and last name of the recipient
		/// </summary>
		/// <returns> First and last name of the recipient </returns>
		string getName();
		/// <summary>
		/// Postal code of the recipient
		/// </summary>
		/// <returns> Postal code of the recipient </returns>
		string getZIP();
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
		string getCountry();
		/// <summary>
		/// Returns the city of the contact
		/// </summary>
		/// <returns> Returns the city of the recipient </returns>
		string getLocation();
		/// <summary>
		/// Returns the street address (street+number) of the contact
		/// </summary>
		/// <returns> street address (street+number) of the contact </returns>
		string getStreet();
		/// <summary>
		/// returns additional address information which is display in xml tag "LineTwo"
		/// </summary>
		/// <returns> additional address information </returns>
		string getAdditionalAddress()
		{
			return null;
		}
		/// <summary>
		///*
		/// obligatory for sender but not for recipient </summary>
		/// <returns> the tax id as string </returns>
		string getTaxID()
		{
			return null;
		}
	}
}