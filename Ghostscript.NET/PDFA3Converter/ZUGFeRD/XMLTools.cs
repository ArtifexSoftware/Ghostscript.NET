// Copyright (C) 2024 Artifex Software, Inc.
//
// This file is part of Ghostscript.NET.
//
// Ghostscript.NET is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// Ghostscript.NET is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
// details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Ghostscript.NET. If not, see 
// <https://www.gnu.org/licenses/agpl-3.0.en.html>
//
// Alternative licensing terms are available from the licensor.
// For commercial licensing, see <https://www.artifex.com/> or contact
// Artifex Software, Inc., 39 Mesa Street, Suite 108A, San Francisco,
// CA 94129, USA, for further information.
using System;
using System.Text;
using System.Security;
using System.Globalization;

namespace Ghostscript.NET.PDFA3Converter.ZUGFeRD
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


		public static string nDigitFormat(decimal value, int scale)
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
			return Math.Round(value, 2, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);

		}


		public static string encodeXML(string s)
		{
			 return SecurityElement.Escape(s);
		}


        /*/// <summary>
        /// Returns the Byte Order Mark size and thus allows to skips over a BOM
        /// at the beginning of the given ByteArrayInputStream, if one exists.
        /// </summary>
		
        /// <param name="is"> the ByteArrayInputStream used </param>
        /// <exception cref="IOException"> if can not be read from is </exception>
        /// <see href="https://www.w3.org/TR/xml/#sec-guessing">Autodetection of Character Encodings</see>
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
		*/

        /// <summary>
        ///*
        /// removes utf8 byte order marks from byte arrays, in case one is there </summary>
        /// <param name="zugferdRaw"> the CII XML </param>
        /// <returns> the byte array without bom </returns>
        public static byte[] removeBOM(byte[] zugferdRaw)
		{
			byte[] zugferdData;
			if (zugferdRaw[0] == unchecked( 0xEF) && zugferdRaw[1] == unchecked( 0xBB) && zugferdRaw[2] == unchecked( 0xBF))
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
