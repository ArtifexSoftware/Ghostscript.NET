namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{

	public class Profile
	{
		protected internal string name, id;

		/// <summary>
		///*
		/// Contruct </summary>
		/// <param name="name"> human readable name of the profile, also used as basis to detemine the XMP Name </param>
		/// <param name="ID"> XML Guideline ID </param>
		public Profile(string name, string ID)
		{
			this.name = name;
			this.id = ID;
		}

		/// <summary>
		///*
		/// gets the name </summary>
		/// <returns> the name of the profile </returns>
		public virtual string getName()
		{
			return name;
		}

		/// <summary>
		///*
		/// get guideline id </summary>
		/// <returns> the XML Guideline ID of the profile </returns>
		public virtual string getID()
		{
			return id;
		}

		/// <summary>
		///*
		/// if the profile is embedded in PDF we need RDF metadata </summary>
		/// <returns> the XMP name string of the profile </returns>
		public virtual string getXMPName()
		{
			if (name.Equals("BASICWL"))
			{
				return "BASIC WL";
			}
			else if (name.Equals("EN16931"))
			{
				return "EN 16931";
			}
			else
			{
				return name;
			}
		}
	}
}