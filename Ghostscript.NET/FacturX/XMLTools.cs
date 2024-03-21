using System;
using System.Text;
using java.math;
using System.Security;

namespace Ghostscript.NET.FacturX
{



    public class XMLTools
	{
		public virtual string escapeAttributeEntities(string s)
		{
//			return base.escapeAttributeEntities(s);
			return s;
		}

		public virtual string escapeElementEntities(string s)
		{
//			return base.escapeElementEntities(s);
			return s;

		}


		public static string nDigitFormat(BigDecimal value, int scale)
		{
			/*
			 * I needed 123,45, locale independent.I tried
			 * NumberFormat.getCurrencyInstance().format( 12345.6789 ); but that is locale
			 * specific.I also tried DecimalFormat df = new DecimalFormat( "0,00" );
			 * df.setDecimalSeparatorAlwaysShown(true); df.setGroupingUsed(false);
			 * DecimalFormatSymbols symbols = new DecimalFormatSymbols();
			 * symbols.setDecimalSeparator(','); symbols.setGroupingSeparator(' ');
			 * df.setDecimalFormatSymbols(symbols);
			 *
			 * but that would not switch off grouping. Although I liked very much the
			 * (incomplete) "BNF diagram" in
			 * http://docs.oracle.com/javase/tutorial/i18n/format/decimalFormat.html in the
			 * end I decided to calculate myself and take eur+sparator+cents
			 *
			 */
			return value.setScale(scale, RoundingMode.HALF_UP).toPlainString();

		}


		public static string encodeXML(String s)
		{
			 return SecurityElement.Escape(s);
		}


		/// <summary>
		/// Returns the Byte Order Mark size and thus allows to skips over a BOM
		/// at the beginning of the given ByteArrayInputStream, if one exists.
		/// </summary>
		/// <param name="is"> the ByteArrayInputStream used </param>
		/// <exception cref="IOException"> if can not be read from is </exception>
		/// <seealso cref="<a href="https://www.w3.org/TR/xml/.sec-guessing">Autodetection of Character Encodings</a>"
		/// 
		/// public static int guessBOMSize(ByteArrayInputStream is) throws IOException {
		/// byte[] pad = new byte[4];
		/// is.read(pad);
		/// is.reset();
		/// int test2 = ((pad[0] & 0xFF) << 8) | (pad[1] & 0xFF);
		/// int test3 = ((test2 & 0xFFFF) << 8) | (pad[2] & 0xFF);
		/// int test4 = ((test3 & 0xFFFFFF) << 8) | (pad[3] & 0xFF);
		/// //
		/// if (test4 == 0x0000FEFF || test4 == 0xFFFE0000 || test4 == 0x0000FFFE || test4 == 0xFEFF0000) {
		///		// UCS-4: BOM takes 4 bytes
		///		return 4;
		/// } else if (test3 == 0xEFBBFF) {
		///		// UTF-8: BOM takes 3 bytes
		///		return 3;
		/// } else if (test2 == 0xFEFF || test2 == 0xFFFE) {
		///		// UTF-16: BOM takes 2 bytes
		///		return 2;
		/// }
		/// return 0;
		/// }/>

		/// <summary>
		///*
		/// removes utf8 byte order marks from byte arrays, in case one is there </summary>
		/// <param name="zugferdRaw"> the CII XML </param>
		/// <returns> the byte array without bom </returns>
		public static byte[] removeBOM(byte[] zugferdRaw)
		{
			byte[] zugferdData;
			if ((zugferdRaw[0] == unchecked((byte) 0xEF)) && (zugferdRaw[1] == unchecked((byte) 0xBB)) && (zugferdRaw[2] == unchecked((byte) 0xBF)))
			{
				// I don't like BOMs, lets remove it
				zugferdData = new byte[zugferdRaw.Length - 3];
				Array.Copy(zugferdRaw, 3, zugferdData, 0, zugferdRaw.Length - 3);
			}
			else
			{
				zugferdData = zugferdRaw;
			}
			return zugferdData;
		}


	}
}
