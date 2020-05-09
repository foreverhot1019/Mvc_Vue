using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace CCBWebApi.Extensions
{
    public static class XMLHelper
    {
        /// <summary>
        /// 异步写日志
        /// </summary>
        private static bool AsyncWriteLog
        {
            get
            {
                bool? _AsyncWriteLog = CacheHelper.Get_SetBoolConfAppSettings("/LoanController", "AsyncWriteLog");
                if (_AsyncWriteLog.HasValue && _AsyncWriteLog.Value)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// XML字符串转类值
        /// </summary>
        /// <param name="XMLStr"></param>
        /// <param name="_type"></param>
        /// <param name="Namespaces"></param>
        /// <returns></returns>
        public static T toTypeObj<T>(this string XMLStr, Encoding OEncoding = null, string Namespaces = "") where T : class, new()
        {
            var ObjVal = new T();
            try
            {
                if (OEncoding == null)
                    OEncoding = System.Text.Encoding.UTF8;
                var _type = typeof(T);
                XMLStr = System.Text.RegularExpressions.Regex.Replace(XMLStr, "^[^<]", "").Replace("\r", "").Replace("\n", "").Replace("\r\n", "").Replace("\n\n", "");
                XMLStr = XMLStr.Replace("xmlns:s0", "xmlns");
                using (StringReader sr = new StringReader(XMLStr))
                {
                    //xmlns=\"HTTP://WWW.ECIDH.COM/WISTRON_KS/B2BRETURN\"
                    System.Xml.Serialization.XmlSerializer xmlser = new XmlSerializer(_type, Namespaces);
                    ObjVal = (T)xmlser.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = Common.GetExceptionMsg(ex) + "-" + ex.StackTrace;
                WriteLogHelper.WriteLog_Local(ErrMsg, "/XMLHelper/toTypeObj", true, AsyncWriteLog);
                ObjVal = null;
            }
            return ObjVal;
        }

        /// <summary>
        /// 类值转换字符串
        /// </summary>
        /// <param name="ObjVal"></param>
        /// <param name="Namespaces"></param>
        /// <returns></returns>
        public static string toXMLStr<T>(this T ObjVal, Encoding OEncoding = null, string Namespaces = "") where T : class, new()
        {
            string XMLStr = "";
            try
            {
                if (OEncoding == null)
                    OEncoding = System.Text.Encoding.UTF8;
                var _type = ObjVal.GetType();
                XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", Namespaces);
                System.Xml.Serialization.XmlSerializer xmlser = new System.Xml.Serialization.XmlSerializer(_type);

                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "    ";
                settings.NewLineChars = "\r\n";
                settings.Encoding = OEncoding;
                //settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
                settings.OmitXmlDeclaration = false;  // 不生成声明头
                using (MemoryStream ms = new MemoryStream())
                using (XmlWriter writer = XmlWriter.Create(ms, settings))
                {
                    xmlser.Serialize(writer, ObjVal, xmlnsEmpty);
                    XMLStr = OEncoding.GetString(ms.ToArray());
                }

                //string AddStr = " xmlns=\"HTTP://WWW.ECIDH.COM/WISTRON_KS/B2BLMM\"";
                //string repStr = "<B2BLMM>";
                //int repStrFirstIndex = XMLStr.IndexOf(repStr);
                //if (repStrFirstIndex >= 0)
                //{
                //    XMLStr = XMLStr.Insert(repStrFirstIndex + repStr.Length - 1, AddStr);
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = Common.GetExceptionMsg(ex) + "-" + ex.StackTrace;
                WriteLogHelper.WriteLog_Local(ErrMsg, "/XMLHelper/toXMLStr", true, AsyncWriteLog);
                XMLStr = "";
            }
            return XMLStr;
        }

        /// <summary>
        /// 变更XML字符串时间格式
        /// </summary>
        /// <param name="XMLStr">XML字符串数据</param>
        /// <param name="repStr">XML节点 开始字符串</param>
        /// <param name="EndrepStr">XML节点 结束字符串</param>
        /// <returns></returns>
        public static string UpdateDateTime(string XMLStr, string repStr, string EndrepStr, string DateRepFormatter = "yyyy-MM-ddTHH:mm:ss")
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
                    //repStrFirstIndex = XMLStr.IndexOf(repStr);
                    //if (repStrFirstIndex > 0)
                    //{
                    //    XMLStr = UpdateDateTime(XMLStr, repStr, EndrepStr, DateRepFormatter);
                    //}
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
        /// <param name="repStr">>XML节点 开始字符串</param>
        /// <param name="EndrepStr">XML节点 结束字符串</param>
        /// <param name="ReplaceValue">替换值</param>
        /// <returns></returns>
        public static string ReplaceXMLValue(string XMLStr, string repStr, string EndrepStr, string ReplaceValue)
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
                            End_Str = ReplaceXMLValue(End_Str, repStr, EndrepStr, ReplaceValue);
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
    }
}