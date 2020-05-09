using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBWebApi
{
    public class GuidConvertHelper
    {
        /// <summary>
        /// .Net-Guid转换为 Oracle-Guid
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DotNetToOracle(string text)
        {
            Guid guid = new Guid(text);
            return BitConverter.ToString(guid.ToByteArray()).Replace("-", "");
        }

        /// <summary>
        /// Oracle-Guid转换为 .Net-Guid
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string OracleToDotNet(string text)
        {
            byte[] bytes = ParseHex(text);
            Guid guid = new Guid(bytes);
            return guid.ToString().ToUpperInvariant();
        }

        /// <summary>
        /// Guid转Hex
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] ParseHex(string text)
        {
            byte[] ret = new byte[text.Length / 2];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
            }
            return ret;
        }

    }
}