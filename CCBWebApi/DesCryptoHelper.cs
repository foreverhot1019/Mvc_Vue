using CCBWebApi.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace CCBWebApi
{
    /// <summary>
    /// Des对称加密算法
    /// </summary>
    public class DesCryptoHelper
    {
        //必须是8位64个字节
        private string strKey_Default = "fldkdjhs";//默认 对称算法密钥
        /// <summary>
        /// 对称算法密钥
        /// </summary>
        public string strKey { get; set; }

        private string strIV_Default = "00000000";//默认 对称算法初始化向量
        /// <summary>
        /// 对称算法初始化向量
        /// </summary>
        public string strIV { get; set; }

        /// <summary>
        /// 默认编码格式
        /// </summary>
        private Encoding _OEncoding = Encoding.UTF8;//默认编码格式

        /// <summary>
        /// 构造函数
        /// </summary>
        public DesCryptoHelper()
        {
            strKey = strKey_Default;
            strIV = strIV_Default;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_strKey">对称算法密钥</param>
        /// <param name="_strIV">对称算法初始化向量</param>
        public DesCryptoHelper(string _strKey, string _strIV)
        {
            if (string.IsNullOrEmpty(_strKey))
                strKey = strKey_Default;
            else
                strKey = _strKey;
            if (string.IsNullOrEmpty(_strIV))
                strIV = strIV_Default;
            else
                strIV = _strIV;
        }

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="_strQ"></param>
        /// <returns></returns>
        public string Encrypt(string _strQ, Encoding OEncoding = null)
        {
            try
            {
                if (OEncoding == null)
                    OEncoding = _OEncoding;//默认编码格式
                if (string.IsNullOrEmpty(_strQ))
                    return null;
                else
                {
                    byte[] buffer = OEncoding.GetBytes(_strQ);
                    MemoryStream ms = new MemoryStream();
                    DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
                    CryptoStream encStream = new CryptoStream(ms, tdes.CreateEncryptor(OEncoding.GetBytes(strKey), OEncoding.GetBytes(strIV)), CryptoStreamMode.Write);
                    encStream.Write(buffer, 0, buffer.Length);
                    encStream.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());//.Replace("+", "%");
                }
            }
            catch (Exception ex)
            {
                WriteLogHelper.WriteLogByLog4Net(ex, Models.EnumType.Log4NetMsgType.Error);
                return null;
            }
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="_strQ"></param>
        /// <returns></returns>
        public string Decrypt(string _strQ, Encoding OEncoding = null)
        {
            try
            {
                if (OEncoding == null)
                    OEncoding = _OEncoding;//默认编码格式
                if (string.IsNullOrEmpty(_strQ))
                    return null;
                else
                {
                    //_strQ = _strQ.Replace("%2B", "+");
                    byte[] buffer = Convert.FromBase64String(_strQ);
                    MemoryStream ms = new MemoryStream();
                    DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
                    CryptoStream encStream = new CryptoStream(ms, tdes.CreateDecryptor(OEncoding.GetBytes(strKey), OEncoding.GetBytes(strIV)), CryptoStreamMode.Write);
                    encStream.Write(buffer, 0, buffer.Length);
                    encStream.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                WriteLogHelper.WriteLogByLog4Net(ex, Models.EnumType.Log4NetMsgType.Error);
                return null;
            }
        }

        /// <summary>
        /// 解密加载
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="fileName"></param>
        public void XmlLoadDecrypt(XmlDocument xmlDoc, string fileName, Encoding OEncoding = null)
        {
            try
            {
                if (OEncoding == null)
                    OEncoding = _OEncoding;//默认编码格式
                FileStream fileStream = new FileStream(fileName, FileMode.Open);
                byte[] bsXml = new byte[fileStream.Length];
                fileStream.Read(bsXml, 0, bsXml.Length);
                fileStream.Close();

                MemoryStream ms = new MemoryStream();
                DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
                CryptoStream encStream = new CryptoStream(ms, tdes.CreateDecryptor(OEncoding.GetBytes(strKey), OEncoding.GetBytes(strIV)), CryptoStreamMode.Write);
                encStream.Write(bsXml, 0, bsXml.Length);
                encStream.FlushFinalBlock();

                xmlDoc.Load(new MemoryStream(ms.ToArray()));
            }
            catch (Exception ex)
            {
                WriteLogHelper.WriteLogByLog4Net(ex, Models.EnumType.Log4NetMsgType.Error);
            }
        }

        /// <summary>
        /// 加密存储
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="fileName"></param>
        public void XmlSaveEncrypt(XmlDocument xmlDoc, string fileName, Encoding OEncoding = null)
        {
            try
            {
                if (OEncoding == null)
                    OEncoding = _OEncoding;//默认编码格式
                if (!File.Exists(fileName))
                    File.Create(fileName).Close();

                FileStream fileStream = new FileStream(fileName, FileMode.Truncate);
                MemoryStream msXml = new MemoryStream();
                xmlDoc.Save(msXml);

                DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
                CryptoStream cs = new CryptoStream(fileStream, tdes.CreateEncryptor(OEncoding.GetBytes(strKey), OEncoding.GetBytes(strIV)), CryptoStreamMode.Write);
                cs.Write(msXml.ToArray(), 0, msXml.ToArray().Length);
                cs.FlushFinalBlock();

                msXml.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                WriteLogHelper.WriteLogByLog4Net(ex, Models.EnumType.Log4NetMsgType.Error);
            }
        }

        /// <summary>
        /// 测试加密解密
        /// </summary>
        public void TestDesCrypto()
        {
            // 测试代码
            string str1 = "abcdefghijklmnopqrstuvwxyz";
            Encrypt(str1);//Encrypt
            Decrypt(Encrypt(str1));//Decrypt

            //Load Decrypt Xml"))
            var XmlDoc = new XmlDocument();
            XmlLoadDecrypt(XmlDoc, "Assets/123.xml");
            var XmlRoot = XmlDoc.DocumentElement;

            //Save Encrypt Xml"))
            var XmlDoc1 = new XmlDocument();
            var XmlRoot1 = XmlDoc1.CreateElement("RegisterRecords");
            XmlDoc1.AppendChild(XmlRoot1);
            XmlRoot1.SetAttribute("Test", "something");
            XmlSaveEncrypt(XmlDoc1, "Assets/123.xml");
        }
    }
}