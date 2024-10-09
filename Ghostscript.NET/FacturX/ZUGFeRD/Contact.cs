namespace Ghostscript.NET.FacturX.ZUGFeRD
{

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	///*
	/// a named contact person in an organisation
	/// for the organisation/company itsel please </summary>
	/// <seealso cref="TradeParty"/>
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @JsonIgnoreProperties(ignoreUnknown = true) public class Contact implements org.mustangproject.ZUGFeRD.IZUGFeRDExportableContact
	public class Contact : IZUGFeRDExportableContact
	{

		protected internal string name, phone, email, zip, street, location, country;
		protected internal string fax = null;

		/// <summary>
		///*
		/// default constructor.
		/// Name, phone and email of sender contact person are e.g. required by XRechnung </summary>
		/// <param name="name"> full name of the contact </param>
		/// <param name="phone"> full phone number </param>
		/// <param name="email"> email address of the contact </param>
		public Contact(string name, string phone, string email)
		{
			this.name = name;
			this.phone = phone;
			this.email = email;
		}

		/// <summary>
		///*
		/// empty constructor.
		/// as always, not recommended, for jackson...
		/// </summary>
		public Contact()
		{
		}

		/// <summary>
		///*
		/// complete specification of a named contact with a different address </summary>
		/// <param name="name"> full name </param>
		/// <param name="phone"> full phone number </param>
		/// <param name="email"> full email </param>
		/// <param name="street"> street+number </param>
		/// <param name="zip"> postcode </param>
		/// <param name="location"> city </param>
		/// <param name="country"> two-letter iso code </param>
		public Contact(string name, string phone, string email, string street, string zip, string location, string country)
		{
			this.name = name;
			this.phone = phone;
			this.email = email;
			this.street = street;
			this.zip = zip;
			this.location = location;
			this.country = country;

		}

		/// <summary>
		///*
		/// XML parsing constructor </summary>
		/// <param name="nodes"> the nodelist returned e.g. from xpath </param>
		public Contact(NodeList nodes)
		{
			if (nodes.getLength() > 0)
			{

				/*
							   will parse sth like
								<ram:DefinedTradeContact>
									<ram:PersonName>Name</ram:PersonName>
									<ram:TelephoneUniversalCommunication>
										<ram:CompleteNumber>069 100-0</ram:CompleteNumber>
									</ram:TelephoneUniversalCommunication>
									<ram:EmailURIUniversalCommunication>
										<ram:URIID>test@example.com</ram:URIID>
									</ram:EmailURIUniversalCommunication>
								</ram:DefinedTradeContact>
				 */
				for (int nodeIndex = 0; nodeIndex < nodes.getLength(); nodeIndex++)
				{
					//nodes.item(i).getTextContent())) {
					Node currentItemNode = nodes.item(nodeIndex);
					if (currentItemNode.getLocalName() != null)
					{

						if (currentItemNode.getLocalName().Equals("PersonName"))
						{
							setName(currentItemNode.getFirstChild().getNodeValue());
						}
						if (currentItemNode.getLocalName().Equals("TelephoneUniversalCommunication"))
						{
							NodeList tel = currentItemNode.getChildNodes();
							for (int telChildIndex = 0; telChildIndex < tel.getLength(); telChildIndex++)
							{
								if (tel.item(telChildIndex).getLocalName() != null)
								{
									if (tel.item(telChildIndex).getLocalName().Equals("CompleteNumber"))
									{
										setPhone(tel.item(telChildIndex).getTextContent());
									}
								}
							}
						}
						if (currentItemNode.getLocalName().Equals("EmailURIUniversalCommunication"))
						{
							NodeList email = currentItemNode.getChildNodes();
							for (int emailChildIndex = 0; emailChildIndex < email.getLength(); emailChildIndex++)
							{
								if (email.item(emailChildIndex).getLocalName() != null)
								{
									if (email.item(emailChildIndex).getLocalName().Equals("URIID"))
									{
										setEMail(email.item(emailChildIndex).getTextContent());
									}
								}
							}
						}
					}
				}
			}
		}


		public string? getName()
		{
			return name;
		}

		/// <summary>
		/// the first and last name of the contact
		/// </summary>
		/// <param name="name"> first and last name </param>
		/// <returns> fluent setter </returns>
		public virtual Contact setName(string name)
		{
			this.name = name;
			return this;
		}

		public string getPhone()
		{
			return phone;
		}

		/// <summary>
		///*
		/// complete phone number of the contact </summary>
		/// <param name="phone"> the complete phone number </param>
		/// <returns> fluent setter </returns>
		public virtual Contact setPhone(string phone)
		{
			this.phone = phone;
			return this;
		}

		public string getFax()
		{
			return fax;
		}

		/// <summary>
		///*
		/// (optional) complete fax number </summary>
		/// <param name="fax"> complete fax number of the contact </param>
		/// <returns> fluent setter </returns>
		public virtual Contact setFax(string fax)
		{
			this.fax = fax;
			return this;
		}

		public virtual string getEMail()
		{
			return email;
		}

		/// <summary>
		///*
		/// personal email address of the contact person </summary>
		/// <param name="email"> the email address of the contact </param>
		/// <returns> fluent setter </returns>
		public virtual Contact setEMail(string email)
		{
			this.email = email;
			return this;
		}

		public virtual string getZIP()
		{
			return zip;
		}

		/// <summary>
		///*
		/// the postcode, if the address is different to the organisation </summary>
		/// <param name="zip"> the postcode of the contact </param>
		/// <returns> fluent setter </returns>
		public virtual Contact setZIP(string zip)
		{
			this.zip = zip;
			return this;
		}

		public string getStreet()
		{
			return street;
		}

		/// <summary>
		/// street and number, if the address is different to the organisation
		/// </summary>
		/// <param name="street"> street and number of the contact </param>
		/// <returns> fluent setter </returns>
		public virtual Contact setStreet(string street)
		{
			this.street = street;
			return this;
		}
		public string getLocation()
		{
			return location;
		}

		/// <summary>
		///*
		/// city of the contact person, if different from organisation </summary>
		/// <param name="location"> city </param>
		/// <returns> fluent setter </returns>
		public virtual Contact setLocation(string location)
		{
			this.location = location;
			return this;
		}

		public string getCountry()
		{
			return country;
		}

		/// <summary>
		///*
		/// two-letter ISO country code of the contact, if different from organisation </summary>
		/// <param name="country"> two-letter iso code </param>
		/// <returns> fluent setter </returns>
		public virtual Contact setCountry(string country)
		{
			this.country = country;
			return this;
		}

	}
}