using System;
using System.Text;

namespace Ghostscript.NET
{
    public class StringHelper
    {
        #region ToUtf8String

        public static string ToUtf8String(string value)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(value);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        #endregion
    }
}
