using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TMI.Web.Extensions
{
    public class HTML_SQLStrHelper
    {
        /// <summary>
        /// 为HTML输出替换特殊字符
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public string ReplaceForHtmlOutput(string inputString, int maxLength)
        {
            StringBuilder retVal = new StringBuilder();
            if ((inputString != null) && (inputString != String.Empty))
            {
                inputString = inputString.Trim();
                if (maxLength > 0 && inputString.Length > maxLength)
                {
                    inputString = inputString.Substring(0, maxLength);
                }
                for (int i = 0; i < inputString.Length; i++)
                {
                    switch (inputString[i])
                    {
                        case '"':
                            retVal.Append("&quot;");
                            break;
                        case '<':
                            retVal.Append("&lt;");
                            break;
                        case '>':
                            retVal.Append("&gt;");
                            break;
                        case '\'':
                            retVal.Append("''");
                            break;
                        default:
                            retVal.Append(inputString[i]);
                            break;
                    }
                }
                //retVal.Replace("'", "''");
            }
            return retVal.ToString();
        }

        /// <summary>
        /// 为数据库的输出替换特殊字符
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public string ReplaceForDatabaseInput(string inputString, int maxLength)
        {
            StringBuilder retVal = new StringBuilder();
            if ((inputString != null) && (inputString != String.Empty))
            {
                inputString = inputString.Trim();
                if (maxLength > 0 && inputString.Length > maxLength)
                {
                    inputString = inputString.Substring(0, maxLength);
                }
                for (int i = 0; i < inputString.Length; i++)
                {
                    switch (inputString[i])
                    {
                        //case '"':
                        //    retVal.Append("&quot;");
                        //    break;
                        //case '<':
                        //    retVal.Append("&lt;");
                        //    break;
                        //case '>':
                        //    retVal.Append("&gt;");
                        //    break;
                        case '\'':
                            retVal.Append("''");
                            break;
                        default:
                            retVal.Append(inputString[i]);
                            break;
                    }
                }
                //retVal.Replace("'", "''");
            }
            return retVal.ToString();
        }

        /// <summary>
        /// 字符在字符串中的出现次数
        /// </summary>
        /// <param name="subChar"></param>
        /// <param name="mainString"></param>
        /// <returns></returns>
        public int Times(char subChar, string mainString)
        {
            int count = 0;
            int start = 0;
            while ((start = mainString.IndexOf(subChar, start)) >= 0)
            {
                start += 1;
                count++;
            }
            return count;
        }

        /// <summary>
        /// 字符串在字符串中的出现次数
        /// </summary>
        /// <param name="subString"></param>
        /// <param name="mainString"></param>
        /// <returns></returns>
        public int Times(string subString, string mainString)
        {
            int count = 0;
            int start = 0;
            while ((start = mainString.IndexOf(subString, start)) >= 0)
            {
                start += subString.Length;
                count++;
            }
            return count;
        }

        /// <summary>
        /// 变更XML字符串时间格式
        /// </summary>
        /// <param name="XMLStr">XML字符串数据</param>
        /// <param name="repStr">开始替换字符串标记</param>
        /// <param name="EndrepStr">结束替换字符串标记</param>
        /// <param name="DateRepFormatter">日期格式</param>
        /// <returns></returns>
        public string UpdateDateTime(string XMLStr, string repStr, string EndrepStr, string DateRepFormatter = "yyyy-MM-ddTHH:mm:ss")
        {
            try
            {
                int repStrFirstIndex = XMLStr.IndexOf(repStr);
                if (repStrFirstIndex >= 0)
                {
                    string StartStr = XMLStr.Substring(0, repStrFirstIndex);
                    string EndStr = XMLStr.Substring(repStrFirstIndex);
                    int EndrepStrFirstIndex = EndStr.IndexOf(EndrepStr);
                    if (EndrepStrFirstIndex > 0)
                    {
                        string dtStr = EndStr.Substring(repStr.Length, EndrepStrFirstIndex - repStr.Length);
                        string End_Str = EndStr.Substring(EndrepStrFirstIndex + EndrepStr.Length);
                        DateTime dt = new DateTime();
                        if (!DateTime.TryParse(dtStr, out dt))
                        {
                            dtStr = DateTime.Now.ToString(DateRepFormatter);
                        }
                        else
                            dtStr = dt.ToString(DateRepFormatter);
                        while (End_Str.IndexOf(repStr) > 0)
                        {
                            End_Str = UpdateDateTime(End_Str, repStr, EndrepStr, DateRepFormatter);
                        }
                        EndStr = repStr + dtStr + EndrepStr + End_Str;
                    }
                    XMLStr = StartStr + EndStr;
                }
                return XMLStr;
            }
            catch
            {
                return XMLStr;
            }
        }

        /// <summary>
        /// 替换XML节点内容
        /// </summary>
        /// <param name="XMLStr">XML字符串数据</param>
        /// <param name="repStr">开始替换字符串标记<APP_PERSON></param>
        /// <param name="EndrepStr">结束替换字符串标记</APP_PERSON></param>
        /// <param name="ReplaceValue">新内容</param>
        /// <returns></returns>
        public string ReplaceXMLValue(string XMLStr, string repStr, string EndrepStr, string ReplaceValue)
        {
            try
            {
                int repStrFirstIndex = XMLStr.IndexOf(repStr);
                if (repStrFirstIndex >= 0)
                {
                    string StartStr = XMLStr.Substring(0, repStrFirstIndex);
                    string EndStr = XMLStr.Substring(repStrFirstIndex);
                    int EndrepStrFirstIndex = EndStr.IndexOf(EndrepStr);
                    if (EndrepStrFirstIndex > 0)
                    {
                        string dtStr = EndStr.Substring(repStr.Length, EndrepStrFirstIndex - repStr.Length);
                        string End_Str = EndStr.Substring(EndrepStrFirstIndex + EndrepStr.Length);

                        if (!string.IsNullOrEmpty(ReplaceValue))
                        {
                            dtStr = ReplaceValue;
                        }

                        while (End_Str.IndexOf(repStr) > 0)
                        {
                            End_Str = UpdateDateTime(End_Str, repStr, EndrepStr, ReplaceValue);
                        }
                        EndStr = repStr + dtStr + EndrepStr + End_Str;
                    }
                    XMLStr = StartStr + EndStr;
                }
                return XMLStr;
            }
            catch
            {
                return XMLStr;
            }
        }

        /// <summary>
        /// 加密解密
        /// </summary>
        public class Encrypt
        {
            /// <summary>
            /// 签名加密
            /// </summary>
            /// <param name="contents">要加密的数据</param>
            /// <param name="base64LocalKey">本地密钥</param>
            /// <param name="base64RemoteKey">远程密钥</param>
            /// <returns>加密后的数据</returns>
            public static string SignaureEncrypt(string contents, string base64LocalKey, string base64RemoteKey)
            {
                //
                RSACryptoServiceProvider rsaService = new RSACryptoServiceProvider();
                RSACryptoServiceProvider rsaClient = new RSACryptoServiceProvider();
                rsaService.ImportCspBlob(Convert.FromBase64String(base64RemoteKey));
                rsaClient.ImportCspBlob(Convert.FromBase64String(base64LocalKey));
                SymmetricAlgorithm sa = Rijndael.Create();
                sa.GenerateIV();

                string key = Convert.ToBase64String(sa.Key);
                key = rsaEncrypt(key, rsaService);
                string data = SaEncrypt(contents, sa);
                string signature = SignData(data, rsaClient);
                //string head = resEncrypt(key + "," + signature, rsaService);
                //string result = head + "," + data;
                string result = key + "," + signature + "," + data;
                return result;
            }

            /// <summary>
            /// 签名解密
            /// </summary>
            /// <param name="contents">要解密的数据</param>
            /// <param name="base64LocalKey">本地密钥</param>
            /// <param name="base64RemoteKey">远程密钥</param>
            /// <returns>解密后的数据</returns>
            public static string SignatureDecrypt(string contents, string base64LocalKey, string base64RemoteKey)
            {
                RSACryptoServiceProvider rsaClient = new RSACryptoServiceProvider();
                rsaClient.ImportCspBlob(Convert.FromBase64String(base64LocalKey));
                string[] commandSplit = contents.Split(',');
                //string head = rsaDecrypt(commandSplit[0], rsaClient);
                //string[] heads = head.Split(',');
                //string key = heads[0];
                //string signature = heads[1];
                string key = rsaDecrypt(commandSplit[0], rsaClient);
                string signature = commandSplit[1];
                SymmetricAlgorithm sa = Rijndael.Create();
                sa.Key = Convert.FromBase64String(key);
                string data = SaDecrypt(commandSplit[2], sa);

                //验证签名
                RSACryptoServiceProvider rsaService = new RSACryptoServiceProvider();
                rsaService.ImportCspBlob(Convert.FromBase64String(base64RemoteKey));
                if (!VerifyData(commandSplit[2], signature, rsaService))
                {
                    throw new Exception("签名验证失败！");
                }

                string result = data;
                return result;
            }

            /// <summary>
            /// MD5加密并返回16进制编码的字符串
            /// </summary>
            /// <param name="contents">要加密的数据</param>
            /// <returns>加密后的数据</returns>
            public static string Md5ForBit(string contents)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] inputByte = Encoding.Default.GetBytes(contents);
                return BitConverter.ToString(md5.ComputeHash(inputByte)).Replace("-", "").ToLower();
            }

            /// <summary>
            /// MD5加密返回base64编码的字符串
            /// </summary>
            /// <param name="contents">要加密的数据</param>
            /// <returns>加密后的数据</returns>
            public static string Md5ForBase64(string contents)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] inputByte = Encoding.Default.GetBytes(contents);
                return Convert.ToBase64String(md5.ComputeHash(inputByte)).Replace("-", "").ToLower();
            }

            /// <summary>
            /// Sha1加密返回16进制的字符串
            /// </summary>
            /// <param name="contents">要加密的数据</param>
            /// <returns>加密后的数据</returns>
            public static string Sha1ForBit(string contents)
            {
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                byte[] inputByte = Encoding.Default.GetBytes(contents);
                return BitConverter.ToString(sha1.ComputeHash(inputByte)).Replace("-", "").ToLower();
            }

            /// <summary>
            /// Sha1加密返回base64编码的字符串
            /// </summary>
            /// <param name="contents">要加密的数据</param>
            /// <returns>加密后的数据</returns>
            public static string Sha1ForBase64(string contents)
            {
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                byte[] inputByte = Encoding.Default.GetBytes(contents);
                return Convert.ToBase64String(sha1.ComputeHash(inputByte)).Replace("-", "").ToLower();
            }

            /// <summary>
            /// 非对称加密
            /// </summary>
            /// <param name="contents">要加密的数据</param>
            /// <param name="base64Key">密钥</param>
            /// <returns>加密后的数据</returns>
            public static string RsaEncrypt(string contents, string base64Key)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportCspBlob(Convert.FromBase64String(base64Key));
                string output = rsaEncrypt(contents, rsa);
                return output;
            }

            /// <summary>
            /// 非对称解密
            /// </summary>
            /// <param name="contents">要解密的数据</param>
            /// <param name="base64Key">密钥</param>
            /// <returns>解密后的数据</returns>
            public static string RsaDecrypt(string contents, string base64Key)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportCspBlob(Convert.FromBase64String(base64Key));
                string output = rsaDecrypt(contents, rsa);
                return output;
            }

            /// <summary>
            /// 签名
            /// </summary>
            /// <param name="contents">要签名的数据</param>
            /// <param name="base64Key">密钥（本地密钥）</param>
            /// <returns>得到的签名结果</returns>
            public static string Signature(string contents, string base64Key)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportCspBlob(Convert.FromBase64String(base64Key));
                string output = SignData(contents, rsa);
                return output;
            }

            /// <summary>
            /// 签名验证
            /// </summary>
            /// <param name="contents">要验证的数据</param>
            /// <param name="signature">数据相应的签名</param>
            /// <param name="base64Key">密钥（远程密钥）</param>
            /// <returns>是否成功</returns>
            public static bool Varify(string contents, string signature, string base64Key)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportCspBlob(Convert.FromBase64String(base64Key));
                bool output = VerifyData(contents, signature, rsa);
                return output;
            }

            /// <summary>
            /// 对称加密
            /// </summary>
            /// <param name="contents">要加密的数据</param>
            /// <param name="base64Key">密钥</param>
            /// <returns>加密后的数据</returns>
            public static string SaEncrypt(string contents, string base64Key)
            {
                SymmetricAlgorithm sa = Rijndael.Create();
                sa.Key = Convert.FromBase64String(base64Key);
                string output = SaEncrypt(contents, sa);
                return output;
            }

            /// <summary>
            /// 对称解密
            /// </summary>
            /// <param name="contents">要解密的数据</param>
            /// <param name="base64Key">密钥</param>
            /// <returns>解密后的数据</returns>
            public static string SaDecrypt(string contents, string base64Key)
            {
                SymmetricAlgorithm sa = Rijndael.Create();
                sa.Key = Convert.FromBase64String(base64Key);
                string output = SaDecrypt(contents, sa);
                return output;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="includePrivateParameters"></param>
            /// <returns></returns>
            public static string BuildRsaKey(bool includePrivateParameters)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                return Convert.ToBase64String(rsa.ExportCspBlob(includePrivateParameters));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static string BuildSaKey()
            {
                SymmetricAlgorithm sa = Rijndael.Create();
                sa.GenerateKey();
                return Convert.ToBase64String(sa.Key);
            }

            /// <summary>
            /// (使用客户端私钥)解密
            /// </summary>
            /// <param name="input"></param>
            /// <param name="provider"></param>
            /// <returns></returns>
            static string rsaDecrypt(string input, RSACryptoServiceProvider provider)
            {
                byte[] bytes = provider.Decrypt(Convert.FromBase64String(input), false);
                string result = Encoding.UTF8.GetString(bytes);
                return result;
            }

            /// <summary>
            /// (使用服务器公钥)加密
            /// </summary>
            /// <param name="input"></param>
            /// <param name="provider"></param>
            /// <returns></returns>
            static string rsaEncrypt(string input, RSACryptoServiceProvider provider)
            {
                byte[] bytes = provider.Encrypt(Encoding.UTF8.GetBytes(input), false);
                string result = Convert.ToBase64String(bytes);
                return result;
            }

            /// <summary>
            /// (使用客户端私钥)对数据签名
            /// </summary>
            /// <param name="input"></param>
            /// <param name="provider"></param>
            /// <returns></returns>
            static string SignData(string input, RSACryptoServiceProvider provider)
            {
                byte[] bytes = provider.SignData(Encoding.UTF8.GetBytes(input), new SHA1CryptoServiceProvider());
                //byte[] bytes2 = new byte[20];
                //for (int i = 0; i < bytes2.Length; i++)
                //{
                //    bytes2[i] = bytes[i];
                //}
                //string result = Convert.ToBase64String(bytes2);
                string result = Convert.ToBase64String(bytes);
                return result;
            }

            /// <summary>
            /// (使用服务器公钥)验证数据签名
            /// </summary>
            /// <param name="data"></param>
            /// <param name="signature"></param>
            /// <param name="provider"></param>
            /// <returns></returns>
            static bool VerifyData(string data, string signature, RSACryptoServiceProvider provider)
            {
                return provider.VerifyData(Encoding.UTF8.GetBytes(data), new SHA1CryptoServiceProvider(), Convert.FromBase64String(signature));

                //byte[] bytes = provider.SignData(Encoding.UTF8.GetBytes(data), new SHA1CryptoServiceProvider());
                //byte[] bytes2 = Convert.FromBase64String(signature);
                //for (int i = 0; i < bytes2.Length; i++)
                //{
                //    if (bytes2[i] != bytes[i])
                //    {
                //        return false;
                //    }
                //}
                //return true;
            }

            /// <summary>
            /// (对等)加密
            /// </summary>
            /// <param name="input"></param>
            /// <param name="provider">包含密钥的provider</param>
            /// <returns></returns>
            static string SaEncrypt(string input, SymmetricAlgorithm provider)
            {
                provider.Mode = CipherMode.ECB;  //块处理模式
                provider.Padding = PaddingMode.Zeros; //末尾数据块的填充模式

                //establish crypto stream
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write);

                byte[] cipherbytes; //加密后的数据

                byte[] plainbytes = Encoding.UTF8.GetBytes(input);

                //加密过程
                cs.Write(plainbytes, 0, plainbytes.Length);
                cs.Close();
                cipherbytes = ms.ToArray();
                ms.Close();
                string result = Convert.ToBase64String(cipherbytes);
                return result;
            }

            /// <summary>
            /// (对称)解密
            /// </summary>
            /// <param name="input"></param>
            /// <param name="provider">包含密钥的provider</param>
            /// <returns></returns>
            static string SaDecrypt(string input, SymmetricAlgorithm provider)
            {
                provider.Mode = CipherMode.ECB;  //块处理模式
                provider.Padding = PaddingMode.Zeros; //末尾数据块的填充模式

                //establish crypto stream
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(input));
                CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Read);

                byte[] finalbytes;  //解密后的数据
                StringBuilder sb = new StringBuilder();
                for (; ; )
                {
                    finalbytes = new byte[1024];
                    int length = cs.Read(finalbytes, 0, 1024);
                    if (length <= 0)
                    {
                        break;
                    }
                    sb.Append(Encoding.UTF8.GetString(finalbytes, 0, length));
                }
                string result = sb.ToString();
                return result;
            }
        }
    }
}