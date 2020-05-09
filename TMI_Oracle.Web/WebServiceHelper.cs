using TMI.Web.Models;
using Microsoft.CSharp;
using Oracle.ManagedDataAccess.Client;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TMI.Web.WebServiceHelper
{
    /// <summary>
    /// WebService加锁字段
    /// </summary>
    public static class WebServiceLockObj
    {
        public static object lockAssemblyWebServiceFileCopy = new object();
        public static object lockWebServiceHelperWriteLog = new object();

        public static ReaderWriterLock ORWLogLocker = TMI.Web.Extensions.Common.ORWLogLocker; //new ReaderWriterLock();
    }

    /// <summary>
    /// WebService 触发
    /// </summary>
    public class WebServiceHandle
    {
        //返回值 事件
        public delegate void RetValidData(Tuple<bool, object> retTuple);
        public event RetValidData EnventRetValid = null;

        /// <summary>
        /// SQL语句参数
        /// </summary>
        public OracleParameter[] Param_s { get; set; }

        /// <summary>
        /// 接口用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 接口密码
        /// </summary>
        public string PasswordStr { get; set; }

        /// <summary>
        /// 方法参数值
        /// 当有值时，不再处理配置的XML
        /// </summary>
        public object[] MethodArgs { get; set; }

        /// <summary>
        /// WSDL服务地址
        /// </summary>
        public string WebServiceUrl { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        public string WebServiceMethodName { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string WebServiceClassName { get; set; }

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        /// 错误的string形式 模板
        /// </summary>
        public string returnErrTypeStr { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_WebServiceUrl">WSDL服务地址</param>
        /// <param name="_WebServiceMethodName">方法名</param>
        /// <param name="_ModelPath">模板文件位置</param>
        /// <param name="_args">要发送的 参数</param>
        /// <param name="_event">方法</param>
        /// <param name="_WebServiceClassName">类名（可为空，默认第一个）</param>
        public WebServiceHandle(string _WebServiceUrl, string _WebServiceMethodName, string _ModelPath = "", object[] MethodArgs = null, OracleParameter[] _Param_s = null, RetValidData _event = null, string _WebServiceClassName = "", string _returnErrTypeStr = "")
        {
            WebServiceUrl = _WebServiceUrl;
            WebServiceMethodName = _WebServiceMethodName;
            WebServiceClassName = _WebServiceClassName;
            ModelPath = _ModelPath;
            Param_s = _Param_s;
            EnventRetValid = _event;
            returnErrTypeStr = _returnErrTypeStr;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="ModelPath"></param>
        /// <returns>success,failure+失败原因</returns>
        public Tuple<bool, object> GetData(ref object[] retObj, XmlDocument doc = null)
        {
            retObj = null;
            //方法 参数 值
            List<object> ArrParamMeters = new List<object>();
            //if (EnventRetValid == null)
            //    throw new Exception("错误触发事件，不能为空");
            if (Param_s == null)
            {
                retObj = null;
                Tuple<bool, object> retTuple = new Tuple<bool, object>(false, (object)"SQL语句参数，不能为空");
                //EnventRetValid(retTuple);
                return retTuple;
            }

            if (!File.Exists(ModelPath))
            {
                retObj = null;
                Tuple<bool, object> retTuple = new Tuple<bool, object>(false, (object)"模板文件不存在");
                //EnventRetValid(retTuple);
                return retTuple;
            }
            if (doc == null)
            {
                doc = new XmlDocument();
                string XMLPath = System.Configuration.ConfigurationManager.AppSettings["WebServiceSettingModelPath"] ?? "\\WebServiceWSDL\\";
                int index = ModelPath.IndexOf(XMLPath);
                if (index != 0)
                {
                    ModelPath = ModelPath.Substring(index);
                }
                string LoadPath = "";
                if (HttpContext.Current == null || HttpContext.Current.Server == null)
                    LoadPath = TMI.Web.Extensions.Common.GetMapPath(ModelPath);
                else
                    LoadPath = HttpContext.Current.Server.MapPath(ModelPath);

                doc.Load(LoadPath);
            }

            XmlNode xmlMethodNode = doc.SelectSingleNode("WebServiceMethods/Method[@name='" + WebServiceMethodName + "']");
            if (xmlMethodNode != null)
            {
                #region  处理方法的参数

                XmlNodeList ArrMethodParams = xmlMethodNode.SelectNodes("ParamMeters/Param");
                foreach (XmlNode ParamNode in ArrMethodParams)
                {
                    //参数 数据集存储
                    DataSet ds = new DataSet();

                    #region  获取数据集

                    XmlNodeList ArrSQLNode = ParamNode.SelectNodes("descendant::SQL");//"//SQL");

                    foreach (XmlNode item in ArrSQLNode)
                    {
                        DataTable tab = new DataTable(item.ParentNode.Attributes["name"].Value);
                        try
                        {
                            if (!string.IsNullOrEmpty(item.InnerText))
                            {
                                List<OracleParameter> _Param_s = new List<OracleParameter>();
                                foreach (OracleParameter OrPitem in Param_s)
                                {
                                    string ParamName = OrPitem.ParameterName;
                                    ParamName = ParamName.IndexOf(":") >= 0 ? ParamName : (":" + ParamName);
                                    if (item.InnerText.Contains(ParamName))
                                    {
                                        OracleParameter OrParm = OrPitem.Clone() as OracleParameter;
                                        _Param_s.Add(OrParm);
                                    }
                                }

                                DataSet dsitem = SQLDALHelper.OracleHelper.GetDataSet(CommandType.Text, item.InnerText, _Param_s.Any() ? _Param_s.ToArray() : null);
                                if (dsitem != null)
                                {
                                    if (dsitem.Tables.Count > 0)
                                    {
                                        foreach (DataTable tab1 in dsitem.Tables)
                                        {
                                            tab = tab1.Copy();
                                            tab.TableName = item.ParentNode.Attributes["name"].Value;
                                            ds.Tables.Add(tab);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ds.Tables.Add(tab);
                            }
                        }
                        catch (Exception ex)
                        {
                            Tuple<bool, object> retTuple = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，获取数据失败(+" + TMI.Web.Extensions.Common.GetExceptionMsg(ex) + "+)");
                            //EnventRetValid(retTuple);
                            return retTuple;
                        }
                    }
                    if (ds.Tables.Count <= 0)
                    {
                        Tuple<bool, object> retTuple = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，获取数据失败");
                        //EnventRetValid(retTuple);
                        return retTuple;
                    }

                    #endregion

                    XmlNode TableNodel = ParamNode.ChildNodes[0];
                    if (TableNodel == null)
                    {
                        retObj = null;
                        Tuple<bool, object> retTuple = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，参数（Param/table）未配置");
                        //EnventRetValid(retTuple);
                        return retTuple;
                    }
                    if (TableNodel.SelectNodes("column").Count <= 0)
                    {
                        retObj = null;
                        Tuple<bool, object> retTuple = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，参数（Param/table/column）未配置");
                        //EnventRetValid(retTuple);
                        return retTuple;
                    }
                    //数据集
                    DataTable dt = ds.Tables[TableNodel.Attributes["name"].Value];

                    #region 验证 数据

                    Tuple<bool, object> _retTuple = ValidDataIsOK(ds, dt == null ? new DataRow[] { } : dt.Select(), TableNodel);
                    if (!_retTuple.Item1)
                    {
                        retObj = null;
                        //EnventRetValid(_retTuple);
                        return _retTuple;
                    }

                    #endregion

                    #region 获取数据

                    object ParamDataObj = null;
                    Tuple<bool, object> retTuple_ = GetParamData(out ParamDataObj, ds, dt == null ? new DataRow[] { } : dt.Select(), TableNodel);
                    if (!retTuple_.Item1)
                    {
                        retObj = null;
                        return retTuple_;
                    }
                    //string OutPutStr = ParamNode.Attributes["OutPut"] == null ? "" : ParamNode.Attributes["OutPut"].Value;
                    //if (OutPutStr == "XML")
                    //{
                    //    XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                    //    xmlnsEmpty.Add("", "");

                    //    System.Xml.Serialization.XmlSerializer xmlser = new System.Xml.Serialization.XmlSerializer(ParamDataObj.GetType());
                    //    StringBuilder sb = new StringBuilder();
                    //    using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true }))
                    //    {
                    //        xmlser.Serialize(writer, ParamDataObj, xmlnsEmpty);
                    //    }
                    //    ArrParamMeters.Add(sb.ToString());
                    //}
                    //else
                    ArrParamMeters.Add(ParamDataObj);

                    #endregion
                }

                #endregion
            }
            else
            {
                retObj = null;
                Tuple<bool, object> retTuple = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，未配置");
                //EnventRetValid(retobj);
                return retTuple;
            }

            retObj = ArrParamMeters.ToArray();
            Tuple<bool, object> ret_Tuple = new Tuple<bool, object>(true, "");
            //EnventRetValid(retTuple_);
            return ret_Tuple;
        }

        /// <summary>
        /// 验证 必填数据
        /// </summary>
        /// <param name="ds">所有的数据的ds</param>
        /// <param name="dt">当前要验证的 dt</param>
        /// <param name="TableNodel">当前要验证的 Table节点</param>
        /// <returns></returns>
        private Tuple<bool, object> ValidDataIsOK(DataSet ds, DataRow[] rows, XmlNode TableNodel)
        {
            try
            {
                #region 验证 数据

                //XmlNode ParentTableNode = null;
                //if (TableNodel != null)
                //    if (TableNodel.ParentNode != null)
                //        if (TableNodel.ParentNode.ParentNode != null)
                //            ParentTableNode = TableNodel.ParentNode.ParentNode;
                //DataTable Parentdt = null;
                //if (ParentTableNode != null)
                //{
                //    if (ParentTableNode.Name == "table")
                //    {
                //        string tabName = ParentTableNode.Attributes["name"] == null ? "" : ParentTableNode.Attributes["name"].Value;
                //        if (!string.IsNullOrEmpty(tabName))
                //            Parentdt = ds.Tables[tabName];
                //    }
                //}

                //转换为 动态类
                List<dynamic> Arrdynamic = TMI.Web.Extensions.Common.TableToDynamic(rows);
                XmlNodeList ArrRequiredColumnNodes = TableNodel.SelectNodes("column[@Required='true']");
                foreach (XmlNode RequiredColumnNode in ArrRequiredColumnNodes)
                {
                    string field = RequiredColumnNode.Attributes["localname"] == null ? "" : RequiredColumnNode.Attributes["localname"].Value;
                    string fielddesc = RequiredColumnNode.Attributes["localdesc"] == null ? "" : RequiredColumnNode.Attributes["localdesc"].Value;
                    string fieldErrMsgStr = (string.IsNullOrEmpty(fielddesc) ? "" : (fielddesc + "-")) + field;
                    string fieldType = RequiredColumnNode.Attributes["dataType"] == null ? "" : RequiredColumnNode.Attributes["dataType"].Value;

                    if (fieldType == "[]" || fieldType == "{}")
                    {
                        var ChildTabNode = RequiredColumnNode.SelectSingleNode("table");
                        if (ChildTabNode != null)
                        {
                            string fieldTabName = ChildTabNode.Attributes["name"] == null ? "" : ChildTabNode.Attributes["name"].Value;
                            string fieldTabNameDesc = ChildTabNode.Attributes["desc"] == null ? "" : ChildTabNode.Attributes["desc"].Value;
                            string fieldTabNameErrMsgStr = (string.IsNullOrEmpty(fieldTabNameDesc) ? "" : (fieldTabNameDesc + "-")) + fieldTabName;
                            if (!string.IsNullOrEmpty(fieldTabName))
                            {
                                DataTable childdt = ds.Tables[fieldTabName];
                                if (childdt != null)
                                {
                                    if (childdt.Rows.Count > 0)
                                    {
                                        #region 主表字表 关联关系语句 并验证数据

                                        string parentrefrenceKey = ChildTabNode.Attributes["parentrefrenceKey"] == null ? "" : ChildTabNode.Attributes["parentrefrenceKey"].Value;
                                        string refrenceKey = ChildTabNode.Attributes["refrenceKey"] == null ? "" : ChildTabNode.Attributes["refrenceKey"].Value;

                                        if (!string.IsNullOrEmpty(parentrefrenceKey) && !string.IsNullOrEmpty(refrenceKey))
                                        {
                                            string[] ArrPfreKeys = parentrefrenceKey.Split(',');
                                            string[] ArrfreKeys = refrenceKey.Split(',');
                                            #region 数据与上级有关联性 则关联后再验证
                                            if (rows.Any())
                                            {
                                                foreach (DataRow dr in rows)
                                                {
                                                    string SeltSqlStr = "";
                                                    for (int i = 0; i < ArrPfreKeys.Length; i++)
                                                    {
                                                        string str = ArrPfreKeys[i];
                                                        if (SeltSqlStr == "")
                                                        {
                                                            if (ArrfreKeys.Length > i && dr[str] != null)
                                                                SeltSqlStr = ArrfreKeys[i] + " = '" + dr[str] + "'";
                                                        }
                                                        else
                                                        {
                                                            if (ArrfreKeys.Length > i && dr[str] != null)
                                                                SeltSqlStr += " and " + ArrfreKeys[i] + " = '" + dr[str] + "'";
                                                        }
                                                    }

                                                    //获取 关联的数据
                                                    DataRow[] ChilddataRows = null;
                                                    ChilddataRows = childdt.Select(SeltSqlStr);
                                                    if (ChilddataRows != null)
                                                    {
                                                        if (ChilddataRows.Count() <= 0)
                                                        {
                                                            Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabNameErrMsgStr + "）是必填字段，不能为空");
                                                            return retobj;
                                                        }
                                                        else
                                                        {
                                                            Tuple<bool, object> retobj = ValidDataIsOK(ds, ChilddataRows, ChildTabNode);
                                                            if (!retobj.Item1)
                                                                return retobj;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabNameErrMsgStr + "）是必填字段，不能为空");
                                                        return retobj;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabNameErrMsgStr + "）是必填字段，且与上级数据有关联，但上级数据为空，这是不允许的");
                                                return retobj;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            #region 数据与上级mei有关联性 则验证本身

                                            //获取 关联的数据
                                            DataRow[] ChilddataRows = null;
                                            ChilddataRows = childdt.Select();
                                            if (ChilddataRows != null)
                                            {
                                                if (ChilddataRows.Count() <= 0)
                                                {
                                                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabNameErrMsgStr + "）是必填字段，不能为空");
                                                    return retobj;
                                                }
                                                else
                                                {
                                                    Tuple<bool, object> retobj = ValidDataIsOK(ds, ChilddataRows, ChildTabNode);
                                                    if (!retobj.Item1)
                                                        return retobj;
                                                }
                                            }
                                            else
                                            {
                                                Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabNameErrMsgStr + "）是必填字段，不能为空");
                                                return retobj;
                                            }

                                            #endregion
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabNameErrMsgStr + "）是必填字段，不能为空");
                                        return retobj;
                                    }
                                }
                                else
                                {
                                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabNameErrMsgStr + "）是必填字段，不能为空");
                                    return retobj;
                                }
                            }
                            else
                            {
                                Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，找不到必填字段字段（" + fieldTabNameErrMsgStr + "）");
                                return retobj;
                            }
                        }
                        else
                        {
                            Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，找不到必填字段字段（" + fieldErrMsgStr + "）");
                            return retobj;
                        }
                    }
                    else
                    {
                        if (rows.Any())
                        {
                            var WhereArrdynamic = Arrdynamic.Where(x => ((IDictionary<string, object>)x).Any(n => n.Key.ToLower() == field.ToLower()));//&& ((string)n.Value == null || (string)n.Value == ""))
                            if (WhereArrdynamic.Any())
                            {
                                if (WhereArrdynamic.Any(x => ((IDictionary<string, object>)x).Any(n => n.Key.ToLower() == field.ToLower() && string.IsNullOrEmpty((string)Convert.ChangeType(n.Value, typeof(string))))))
                                {
                                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldErrMsgStr + "）是必填字段，不能为空");
                                    return retobj;
                                }
                            }
                            else
                            {
                                Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，找不到必填字段字段（" + fieldErrMsgStr + "）");
                                return retobj;
                            }
                        }
                        else
                        {
                            Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldErrMsgStr + "）是必填字段，不能为空");
                            return retobj;
                        }
                    }
                }

                #endregion

                Tuple<bool, object> _retobj = new Tuple<bool, object>(true, (object)"");
                return _retobj;
            }
            catch (Exception ex)
            {
                Tuple<bool, object> exretTuple = new Tuple<bool, object>(false, TMI.Web.Extensions.Common.GetExceptionMsg(ex));
                return exretTuple;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="ParamObjData">存放参数节点数据</param>
        /// <param name="ds">当前参数节点的所有数据集合</param>
        /// <param name="rows">当前table节点数据</param>
        /// <param name="TableNode">当前table节点</param>
        /// <returns></returns>
        private Tuple<bool, object> GetParamData(out object ParamObjData, DataSet ds, DataRow[] rows, XmlNode TableNode)
        {
            try
            {
                ParamObjData = null;
                if (TableNode == null)
                {
                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，参数 table节点不存在");
                    return retobj;
                }
                string tabName = TableNode.Attributes["name"] == null ? "" : TableNode.Attributes["name"].Value;
                if (TableNode.ParentNode == null)
                {
                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，参数 " + tabName + "上级节点不存在");
                    return retobj;
                }
                string dataType = TableNode.ParentNode.Attributes["dataType"] == null ? "" : TableNode.ParentNode.Attributes["dataType"].Value;
                //转换成string
                List<string> ArrStrColumnValue = new List<string>() { "decimal", "double", "float" };

                if (dataType == "[]")
                {
                    #region 数组

                    //记录数据
                    List<object> ArrParamObj = new List<object>();

                    //string SeltSqlStr = "";
                    //string parentrefrenceKey = TableNode.Attributes["parentrefrenceKey"] == null ? "" : TableNode.Attributes["parentrefrenceKey"].Value;
                    //string refrenceKey = TableNode.Attributes["refrenceKey"] == null ? "" : TableNode.Attributes["refrenceKey"].Value;

                    //if (!string.IsNullOrEmpty(parentrefrenceKey) && !string.IsNullOrEmpty(refrenceKey))
                    //{
                    //    string[] ArrPfreKeys = parentrefrenceKey.Split(',');
                    //    string[] ArrfreKeys = refrenceKey.Split(',');
                    //    #region 数据与上级有关联性 则关联后再验证
                    //    if (rows.Any())
                    //    {
                    //        foreach (DataRow dr in rows)
                    //        {
                    //            for (int i = 0; i < ArrPfreKeys.Length; i++)
                    //            {
                    //                string str = ArrPfreKeys[i];
                    //                if (SeltSqlStr == "")
                    //                {
                    //                    if (ArrfreKeys.Length > i && dr[str] != null)
                    //                        SeltSqlStr = ArrfreKeys[i] + " = " + dr[str];
                    //                }
                    //                else
                    //                {
                    //                    if (ArrfreKeys.Length > i && dr[str] != null)
                    //                        SeltSqlStr += " and" + ArrfreKeys[i] + " = " + dr[str];
                    //                }
                    //            }

                    //            //获取 关联的数据
                    //            DataRow[] ChilddataRows = null;
                    //            ChilddataRows = childdt.Select(SeltSqlStr);
                    //            if (ChilddataRows != null)
                    //            {
                    //                if (ChilddataRows.Count() <= 0)
                    //                {
                    //                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabName + "）是必填字段，不能为空");
                    //                    return retobj;
                    //                }
                    //                else
                    //                {
                    //                    Tuple<bool, object> retobj = ValidDataIsOK(ds, ChilddataRows, ChildTabNode);
                    //                    if (!retobj.Item1)
                    //                        return retobj;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabName + "）是必填字段，不能为空");
                    //                return retobj;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，字段（" + fieldTabName + "）是必填字段，且与上级数据有关联，但上级数据为空，这是不允许的");
                    //        return retobj;
                    //    }
                    //    #endregion
                    //}
                    //else
                    //{
                    //}

                    #region 数据与上级没有关联性 则验证本身

                    if (rows.Any())
                    {
                        foreach (DataRow dr in rows)
                        {
                            dynamic dynamicObj = new System.Dynamic.ExpandoObject();
                            var dymic = (IDictionary<string, object>)dynamicObj;
                            XmlNodeList ArrColumnNodes = TableNode.SelectNodes("column");
                            foreach (XmlNode ColumnNode in ArrColumnNodes)
                            {
                                string ColumnNodename = ColumnNode.Attributes["name"] == null ? null : ColumnNode.Attributes["name"].Value;
                                string ColumnNodelocalname = ColumnNode.Attributes["localname"] == null ? null : ColumnNode.Attributes["localname"].Value;
                                string ColumnNodedataType = ColumnNode.Attributes["dataType"] == null ? null : ColumnNode.Attributes["dataType"].Value;

                                #region  设置动态数据

                                if (ColumnNodedataType != null)
                                {
                                    if (ColumnNodedataType.ToLower() == "[]" || ColumnNodedataType.ToLower() == "{}")
                                    {
                                        dymic[ColumnNodename] = null;

                                        #region 对象或 数组

                                        var ChildTableNode = ColumnNode.SelectSingleNode("table");
                                        if (ChildTableNode != null)
                                        {
                                            string ChildTableNodename = ChildTableNode.Attributes["name"] == null ? null : ChildTableNode.Attributes["name"].Value;
                                            //子的table
                                            DataTable dtChildTable = ds.Tables[ChildTableNodename];
                                            if (dtChildTable != null)
                                            {
                                                string ChildTabSeltSqlStr = "";
                                                string ChildTabparentrefrenceKey = ChildTableNode.Attributes["parentrefrenceKey"] == null ? "" : ChildTableNode.Attributes["parentrefrenceKey"].Value;
                                                string ChildTabrefrenceKey = ChildTableNode.Attributes["refrenceKey"] == null ? "" : ChildTableNode.Attributes["refrenceKey"].Value;

                                                #region 设置关联关系

                                                if (!string.IsNullOrEmpty(ChildTabparentrefrenceKey) && !string.IsNullOrEmpty(ChildTabrefrenceKey))
                                                {
                                                    string[] ArrChildTabPfreKeys = ChildTabparentrefrenceKey.Split(',');
                                                    string[] ArrChildTabfreKeys = ChildTabrefrenceKey.Split(',');

                                                    for (int i = 0; i < ArrChildTabPfreKeys.Length; i++)
                                                    {
                                                        string str = ArrChildTabPfreKeys[i];
                                                        if (ChildTabSeltSqlStr == "")
                                                        {
                                                            if (ArrChildTabfreKeys.Length > i && dr[str] != null)
                                                                ChildTabSeltSqlStr = ArrChildTabfreKeys[i] + " = '" + dr[str] + "'";
                                                        }
                                                        else
                                                        {
                                                            if (ArrChildTabfreKeys.Length > i && dr[str] != null)
                                                                ChildTabSeltSqlStr += " and" + ArrChildTabfreKeys[i] + " = '" + dr[str] + "'";
                                                        }
                                                    }
                                                }

                                                #endregion

                                                DataRow[] ChildTableRows = dtChildTable.Select(ChildTabSeltSqlStr);
                                                object ColumnObj = null;
                                                Tuple<bool, object> retTuplt = GetParamData(out ColumnObj, ds, ChildTableRows, ChildTableNode);
                                                if (!retTuplt.Item1)
                                                {
                                                    ParamObjData = null;
                                                    return retTuplt;
                                                }
                                                dymic[ColumnNodename] = ColumnObj;
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        if (ArrStrColumnValue.Any(x => x == ColumnNodedataType.ToLower()))
                                        {
                                            decimal decimalVal = 0;
                                            string StrValue = dr.Table.Columns.Contains(ColumnNodelocalname) ? dr[ColumnNodelocalname].ToString() : "";
                                            if (decimal.TryParse(StrValue, out decimalVal))
                                                dymic[ColumnNodename] = decimalVal.ToString("#0.#########");
                                            else
                                                dymic[ColumnNodename] = StrValue;
                                        }
                                        else
                                        {
                                            dymic[ColumnNodename] = dr.Table.Columns.Contains(ColumnNodelocalname) ? dr[ColumnNodelocalname] : "";
                                        }
                                    }
                                }
                                else
                                {
                                    if (ArrStrColumnValue.Any(x => x == ColumnNodedataType.ToLower()))
                                    {
                                        dymic[ColumnNodename] = dr.Table.Columns.Contains(ColumnNodelocalname) ? dr[ColumnNodelocalname].ToString() : "";
                                    }
                                    else
                                    {
                                        dymic[ColumnNodename] = dr.Table.Columns.Contains(ColumnNodelocalname) ? dr[ColumnNodelocalname] : "";
                                    }
                                }

                                #endregion
                            }
                            ArrParamObj.Add(dynamicObj);
                        }
                    }
                    else
                    {
                        dynamic dynamicObj = new System.Dynamic.ExpandoObject();
                        var dymic = (IDictionary<string, object>)dynamicObj;
                        XmlNodeList ArrColumnNodes = TableNode.SelectNodes("column");
                        foreach (XmlNode ColumnNode in ArrColumnNodes)
                        {
                            string ColumnNodename = ColumnNode.Attributes["name"] == null ? null : ColumnNode.Attributes["name"].Value;
                            string ColumnNodelocalname = ColumnNode.Attributes["localname"] == null ? null : ColumnNode.Attributes["localname"].Value;
                            string ColumnNodedataType = ColumnNode.Attributes["dataType"] == null ? null : ColumnNode.Attributes["dataType"].Value;

                            #region  设置动态数据

                            if (ColumnNodedataType != null)
                            {
                                if (ColumnNodedataType.ToLower() == "[]" || ColumnNodedataType.ToLower() == "{}")
                                {
                                    dymic[ColumnNodename] = null;

                                    #region 对象或 数组

                                    var ChildTableNode = ColumnNode.SelectSingleNode("table");
                                    if (ChildTableNode != null)
                                    {
                                        string ChildTableNodename = ChildTableNode.Attributes["name"] == null ? null : ChildTableNode.Attributes["name"].Value;
                                        //子的table
                                        DataTable dtChildTable = ds.Tables[ChildTableNodename];
                                        if (dtChildTable != null)
                                        {
                                            string ChildTabSeltSqlStr = "";
                                            string ChildTabparentrefrenceKey = ChildTableNode.Attributes["parentrefrenceKey"] == null ? "" : ChildTableNode.Attributes["parentrefrenceKey"].Value;
                                            string ChildTabrefrenceKey = ChildTableNode.Attributes["refrenceKey"] == null ? "" : ChildTableNode.Attributes["refrenceKey"].Value;
                                            if (!string.IsNullOrEmpty(ChildTabparentrefrenceKey) || !string.IsNullOrEmpty(ChildTabrefrenceKey))
                                            {
                                                ParamObjData = null;
                                                Tuple<bool, object> retTuple = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，参数的子表与主表有关联，但上级数据为空，这是不允许的");
                                                //EnventRetValid(retTuple);
                                                return retTuple;
                                            }
                                            DataRow[] ChildTableRows = dtChildTable.Select(ChildTabSeltSqlStr);
                                            object ColumnObj = null;
                                            Tuple<bool, object> retTuplt = GetParamData(out ColumnObj, ds, ChildTableRows, ChildTableNode);
                                            if (!retTuplt.Item1)
                                            {
                                                ParamObjData = null;
                                                return retTuplt;
                                            }
                                            dymic[ColumnNodename] = ColumnObj;
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    dymic[ColumnNodename] = "";
                                }
                            }
                            else
                            {
                                dymic[ColumnNodename] = "";
                            }

                            #endregion
                        }
                        ArrParamObj.Add(dynamicObj);
                    }

                    #endregion

                    ParamObjData = ArrParamObj;

                    #endregion
                }
                else if (dataType == "{}")
                {
                    #region 对象

                    dynamic dynamicObj = new System.Dynamic.ExpandoObject();
                    var dymic = (IDictionary<string, object>)dynamicObj;

                    if (rows.Any())
                    {
                        DataRow dr = rows[0];
                        XmlNodeList ArrColumnNodes = TableNode.SelectNodes("column");
                        foreach (XmlNode ColumnNode in ArrColumnNodes)
                        {
                            string ColumnNodename = ColumnNode.Attributes["name"] == null ? null : ColumnNode.Attributes["name"].Value;
                            string ColumnNodelocalname = ColumnNode.Attributes["localname"] == null ? null : ColumnNode.Attributes["localname"].Value;
                            string ColumnNodedataType = ColumnNode.Attributes["dataType"] == null ? null : ColumnNode.Attributes["dataType"].Value;

                            #region  设置动态数据

                            if (ColumnNodedataType != null)
                            {
                                if (ColumnNodedataType.ToLower() == "[]" || ColumnNodedataType.ToLower() == "{}")
                                {
                                    dymic[ColumnNodename] = null;

                                    #region 对象或 数组

                                    var ChildTableNode = ColumnNode.SelectSingleNode("table");
                                    if (ChildTableNode != null)
                                    {
                                        string ChildTableNodename = ChildTableNode.Attributes["name"] == null ? null : ChildTableNode.Attributes["name"].Value;
                                        //子的table
                                        DataTable dtChildTable = ds.Tables[ChildTableNodename];
                                        if (dtChildTable != null)
                                        {
                                            string ChildTabSeltSqlStr = "";
                                            string ChildTabparentrefrenceKey = ChildTableNode.Attributes["parentrefrenceKey"] == null ? "" : ChildTableNode.Attributes["parentrefrenceKey"].Value;
                                            string ChildTabrefrenceKey = ChildTableNode.Attributes["refrenceKey"] == null ? "" : ChildTableNode.Attributes["refrenceKey"].Value;

                                            #region 设置关联关系

                                            if (!string.IsNullOrEmpty(ChildTabparentrefrenceKey) && !string.IsNullOrEmpty(ChildTabrefrenceKey))
                                            {
                                                string[] ArrChildTabPfreKeys = ChildTabparentrefrenceKey.Split(',');
                                                string[] ArrChildTabfreKeys = ChildTabrefrenceKey.Split(',');

                                                for (int i = 0; i < ArrChildTabPfreKeys.Length; i++)
                                                {
                                                    string str = ArrChildTabPfreKeys[i];
                                                    if (ChildTabSeltSqlStr == "")
                                                    {
                                                        if (ArrChildTabfreKeys.Length > i && dr[str] != null)
                                                            ChildTabSeltSqlStr = ArrChildTabfreKeys[i] + " = '" + dr[str] + "'";
                                                    }
                                                    else
                                                    {
                                                        if (ArrChildTabfreKeys.Length > i && dr[str] != null)
                                                            ChildTabSeltSqlStr += " and" + ArrChildTabfreKeys[i] + " = '" + dr[str] + "'";
                                                    }
                                                }
                                            }

                                            #endregion

                                            DataRow[] ChildTableRows = dtChildTable.Select(ChildTabSeltSqlStr);
                                            object ColumnObj = null;
                                            Tuple<bool, object> retTuplt = GetParamData(out ColumnObj, ds, ChildTableRows, ChildTableNode);
                                            if (!retTuplt.Item1)
                                            {
                                                ParamObjData = null;
                                                return retTuplt;
                                            }
                                            dymic[ColumnNodename] = ColumnObj;
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    if (ArrStrColumnValue.Any(x => x == ColumnNodedataType.ToLower()))
                                    {
                                        decimal decimalVal = 0;
                                        string StrValue = dr.Table.Columns.Contains(ColumnNodelocalname) ? dr[ColumnNodelocalname].ToString() : "";
                                        if (decimal.TryParse(StrValue, out decimalVal))
                                            dymic[ColumnNodename] = decimalVal.ToString("#0.#########");
                                        else
                                            dymic[ColumnNodename] = StrValue;
                                    }
                                    else
                                    {
                                        dymic[ColumnNodename] = dr.Table.Columns.Contains(ColumnNodelocalname) ? dr[ColumnNodelocalname] : "";
                                    }
                                }
                            }
                            else
                            {
                                if (ArrStrColumnValue.Any(x => x == ColumnNodedataType.ToLower()))
                                {
                                    dymic[ColumnNodename] = dr.Table.Columns.Contains(ColumnNodelocalname) ? dr[ColumnNodelocalname].ToString() : "";
                                }
                                else
                                {
                                    dymic[ColumnNodename] = dr.Table.Columns.Contains(ColumnNodelocalname) ? dr[ColumnNodelocalname] : "";
                                }
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        XmlNodeList ArrColumnNodes = TableNode.SelectNodes("column");
                        foreach (XmlNode ColumnNode in ArrColumnNodes)
                        {
                            string ColumnNodename = ColumnNode.Attributes["name"] == null ? null : ColumnNode.Attributes["name"].Value;
                            string ColumnNodelocalname = ColumnNode.Attributes["localname"] == null ? null : ColumnNode.Attributes["localname"].Value;
                            string ColumnNodedataType = ColumnNode.Attributes["dataType"] == null ? null : ColumnNode.Attributes["dataType"].Value;

                            #region  设置动态数据

                            if (ColumnNodedataType != null)
                            {
                                if (ColumnNodedataType.ToLower() == "[]" || ColumnNodedataType.ToLower() == "{}")
                                {
                                    dymic[ColumnNodename] = null;

                                    #region 对象或 数组

                                    var ChildTableNode = ColumnNode.SelectSingleNode("table");
                                    if (ChildTableNode != null)
                                    {
                                        string ChildTableNodename = ChildTableNode.Attributes["name"] == null ? null : ChildTableNode.Attributes["name"].Value;
                                        //子的table
                                        DataTable dtChildTable = ds.Tables[ChildTableNodename];
                                        if (dtChildTable != null)
                                        {
                                            string ChildTabSeltSqlStr = "";
                                            string ChildTabparentrefrenceKey = ChildTableNode.Attributes["parentrefrenceKey"] == null ? "" : ChildTableNode.Attributes["parentrefrenceKey"].Value;
                                            string ChildTabrefrenceKey = ChildTableNode.Attributes["refrenceKey"] == null ? "" : ChildTableNode.Attributes["refrenceKey"].Value;
                                            if (!string.IsNullOrEmpty(ChildTabparentrefrenceKey) || !string.IsNullOrEmpty(ChildTabrefrenceKey))
                                            {
                                                ParamObjData = null;
                                                Tuple<bool, object> retTuple = new Tuple<bool, object>(false, (object)"WebService模板" + WebServiceMethodName + "方法，参数的子表与主表有关联，但上级数据为空，这是不允许的");
                                                //EnventRetValid(retTuple);
                                                return retTuple;
                                            }
                                            DataRow[] ChildTableRows = dtChildTable.Select(ChildTabSeltSqlStr);
                                            object ColumnObj = null;
                                            Tuple<bool, object> retTuplt = GetParamData(out ColumnObj, ds, ChildTableRows, ChildTableNode);
                                            if (!retTuplt.Item1)
                                            {
                                                ParamObjData = null;
                                                return retTuplt;
                                            }
                                            dymic[ColumnNodename] = ColumnObj;
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    dymic[ColumnNodename] = "";
                                }
                            }
                            else
                            {
                                dymic[ColumnNodename] = "";
                            }

                            #endregion
                        }
                    }

                    ParamObjData = dynamicObj;

                    #endregion
                }
                else
                {
                    #region 字段

                    object obj = null;
                    if (rows != null)
                    {
                        if (rows.Any())
                        {
                            DataRow dr = rows[0];
                            XmlNode ColumnNode = TableNode.SelectSingleNode("column");
                            if (ColumnNode != null)
                            {
                                string columnlocalname = ColumnNode.Attributes["localname"] == null ? "" : ColumnNode.Attributes["localname"].Value;
                                if (!string.IsNullOrEmpty(columnlocalname))
                                    obj = dr[columnlocalname];
                            }
                        }
                    }
                    ParamObjData = obj;

                    #endregion
                }

                Tuple<bool, object> _retobj = new Tuple<bool, object>(true, (object)"");
                return _retobj;
            }
            catch (Exception ex)
            {
                ParamObjData = null;
                Tuple<bool, object> exretTuple = new Tuple<bool, object>(false, TMI.Web.Extensions.Common.GetExceptionMsg(ex));
                return exretTuple;
            }
        }

        /// <summary>
        /// 执行WebService方法
        /// 错误信息和成功信息，都在委托方法中执行
        /// </summary>
        public void StartWebService(XmlDocument doc = null)
        {
            try
            {
                object[] args = null;
                if (MethodArgs != null)
                    args = MethodArgs;
                else
                {
                    Tuple<bool, object> _retTupleobj = GetData(ref args, doc);
                    if (!_retTupleobj.Item1)
                    {
                        if (EnventRetValid != null)
                            EnventRetValid(_retTupleobj);
                        return;
                    }
                }

                #region 一个方法 多种数据配置时

                string WebServiceMethodNameStr = WebServiceMethodName;
                XmlNode xmlMethodNode = doc.SelectSingleNode("WebServiceMethods/Method[@name='" + WebServiceMethodName + "']");
                if (xmlMethodNode != null)
                {
                    //一个方法 多种数据配置时
                    if (xmlMethodNode.Attributes["MethodName"] != null)
                    {
                        WebServiceMethodNameStr = xmlMethodNode.Attributes["MethodName"].Value;
                    }
                }

                #endregion

                //Tuple<bool, object> retTupleobj = WebServiceHelper.InvokeWebServiceInMemeory(WebServiceUrl, WebServiceMethodName, args, UserName, PasswordStr, "", returnErrTypeStr);
                Tuple<bool, object> retTupleobj = WebServiceHelper.InvokeWebServiceOutPutDLL(WebServiceUrl, WebServiceMethodNameStr, args, ModelPath, Param_s, UserName, PasswordStr, "", returnErrTypeStr);
                if (!string.IsNullOrEmpty(returnErrTypeStr))
                {
                    string retStr = retTupleobj.Item2.ToString();
                    if (retStr.Replace(" ", "").IndexOf(returnErrTypeStr.Replace("'", "\"")) >= 0 || retStr.Replace(" ", "").IndexOf(returnErrTypeStr.Replace("\"", "'")) >= 0)
                    {
                        retTupleobj = new Tuple<bool, object>(false, retStr);
                        try
                        {
                            TMI.Web.Extensions.Common.WriteLog_LocalByRWLogLocker(retStr, "ServiceLog\\ErrorReturn", true, false);
                            //WebServiceLockObj.ORWLogLocker.AcquireWriterLock(500);
                            //SQLDALHelper.WriteLogHelper.WriteLog(retStr, "ServiceLog\\ErrorReturn", true, false);
                            //WebServiceLockObj.ORWLogLocker.ReleaseWriterLock();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                if (EnventRetValid != null)
                {
                    if (!retTupleobj.Item1)
                    {
                        string retStr = retTupleobj.Item2.ToString();
                        retStr += ((args == null || !args.Any()) ? "" : (Newtonsoft.Json.JsonConvert.SerializeObject(args) + "")) + ((Param_s == null || !Param_s.Any()) ? "" : Newtonsoft.Json.JsonConvert.SerializeObject(Param_s));
                        retTupleobj = new Tuple<bool, object>(false, retStr);
                        EnventRetValid(retTupleobj);
                    }
                    else
                    {
                        retTupleobj = new Tuple<bool, object>(true, "");
                        EnventRetValid(retTupleobj);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = TMI.Web.Extensions.Common.GetExceptionMsg(ex);
                Tuple<bool, object> _retTupleobj = new Tuple<bool, object>(false, ErrMsg);
                if (EnventRetValid != null)
                    EnventRetValid(_retTupleobj);
                return;
            }
        }
    }

    /// <summary>
    /// 动态生成 WebService dll
    /// </summary>
    public static class WebServiceHelper
    {
        //EntityFrameWork
        public static WebdbContext appContext = new WebdbContext();
        public static Repository.Pattern.Ef6.UnitOfWork unitOfWork_ = new Repository.Pattern.Ef6.UnitOfWork(appContext);

        //需要重发的方法
        private static List<string> ArrTMMethod = new List<string>();
        private static string TMMethodJson = System.Configuration.ConfigurationManager.AppSettings["TMMethod"] ?? "";
        //需要重发的错误返回键值
        private static List<string> ArrTMErrKey = new List<string>();
        private static string TMErrKeyJson = System.Configuration.ConfigurationManager.AppSettings["TMErrKey"] ?? "";
        //需要发送邮件的错误返回键值
        private static List<string> ArrSendToMailErr = new List<string>();
        private static string SendToMailErrJson = System.Configuration.ConfigurationManager.AppSettings["SendToMailErr"] ?? "";

        //是否 保留发送报文
        private static string StrWriteServiceLog = System.Configuration.ConfigurationManager.AppSettings["IsWriteServiceLog"] ?? "1";
        //保留发送报文目录
        private static string ServiceLogAddress = System.Configuration.ConfigurationManager.AppSettings["ServiceLogAddress"] ?? "ServiceLog";

        ///// <summary>
        ///// 动态WebService的类型
        ///// </summary>
        //private static Type WebServiceType { get; set; }

        ///// <summary>
        ///// 动态WebService的类名
        ///// </summary>
        //private static string ServiceClassName { get; set; }

        /// < summary>          
        /// 动态调用web服务 
        /// < /summary>          
        /// < param name="url">WSDL服务地址< /param>
        /// < param name="classname">类名< /param>  
        /// < param name="methodname">方法名< /param>  
        /// < param name="args">参数< /param> 
        /// < returns>< /returns>
        private static object InvokeWebService(string url, string classname, string methodname, object[] args, OracleParameter[] Param_s, string _ModelPath, string UserName = "", string PasswordStr = "")
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = GetWsClassName(url);
            }
            try
            {
                string XMLPath = System.Configuration.ConfigurationManager.AppSettings["WebServiceSettingModelPath"] ?? "\\WebServiceWSDL\\";
                if (_ModelPath.IndexOf(".") > 0)
                    _ModelPath = _ModelPath.Substring(0, _ModelPath.LastIndexOf("\\"));
                if (_ModelPath.IndexOf(":") <= 0)
                {
                    if (_ModelPath.IndexOf(XMLPath) < 0)
                        XMLPath = XMLPath + _ModelPath;
                    if (HttpContext.Current == null || HttpContext.Current.Server == null)
                        _ModelPath = TMI.Web.Extensions.Common.GetMapPath(XMLPath);
                    else
                        _ModelPath = HttpContext.Current.Server.MapPath(XMLPath);
                }

                //获取WSDL   
                WebClient wc = new WebClient();
                if (!string.IsNullOrEmpty(UserName))
                    wc.Credentials = new NetworkCredential(UserName, PasswordStr);
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);//创建和格式化 WSDL 文档文件。
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();//创建客户端代理类。
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码          
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider icc = new CSharpCodeProvider();//用于创建和检索代码生成器和代码编译器的实例
                //设定编译参数                 
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //编译代理类                 
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                //生成代理实例，并调用方法   
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type WebServiceType = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(WebServiceType);//创建实例
                MethodInfo[] ServiceMethods = WebServiceType.GetMethods();
                System.Reflection.MethodInfo mi = ServiceMethods.Where(x => x.Name.ToLower() == methodname.ToLower()).FirstOrDefault();
                if (mi != null)
                {
                    ParameterInfo[] Parameters = mi.GetParameters();
                    return mi.Invoke(obj, args);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }

        /// <summary>
        /// 获取WebService 类名
        /// </summary>
        /// <param name="wsUrl"></param>
        /// <returns></returns>
        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }

        /// </summary>  
        /// 根据指定的信息，调用远程WebService方法  
        /// </summary>  
        /// <param name="url">WebService的http形式的地址</param>  
        /// <param name="nameSpace">欲调用的WebService的命名空间</param>  
        /// <param name="classname">欲调用的WebService的类名（不包括命名空间前缀）</param>  
        /// <param name="methodname">欲调用的WebService的方法名</param>  
        /// <param name="args">参数列表</param>  
        /// <returns>WebService的执行结果</returns>  
        private static Tuple<bool, object> InvokeWebServiceInMemeory(string url, string methodname, object[] args, OracleParameter[] Param_s, string _ModelPath, string UserName = "", string PasswordStr = "", string classname = "", string nameSpace = "WECDSWebService.DynamicWebCalling", string retErrTypeStr = "")
        {
            try
            {
                string XMLPath = System.Configuration.ConfigurationManager.AppSettings["WebServiceSettingModelPath"] ?? "\\WebServiceWSDL\\";
                if (_ModelPath.IndexOf(".") > 0)
                    _ModelPath = _ModelPath.Substring(0, _ModelPath.LastIndexOf("\\"));
                if (_ModelPath.IndexOf(":") <= 0)
                {
                    if (_ModelPath.IndexOf(XMLPath) < 0)
                        XMLPath = XMLPath + _ModelPath;
                    if (HttpContext.Current == null || HttpContext.Current.Server == null)
                        _ModelPath = TMI.Web.Extensions.Common.GetMapPath(XMLPath);
                    else
                        _ModelPath = HttpContext.Current.Server.MapPath(XMLPath);
                }

                //1.使用WebClient 下载WSDL信息
                WebClient wc = new WebClient();
                if (!string.IsNullOrEmpty(UserName))
                    wc.Credentials = new NetworkCredential(UserName, PasswordStr);
                Stream stream = wc.OpenRead(url);
                //2.创建和格式化WSDL文档
                ServiceDescription srvDesc = ServiceDescription.Read(stream);
                List<System.Web.Services.Description.Service> ArrServices = new List<System.Web.Services.Description.Service>();
                foreach (System.Web.Services.Description.Service item in srvDesc.Services)
                {
                    ArrServices.Add(item);
                }
                if (ArrServices.Any())
                {
                    var WhereArrServices = ArrServices.Where(x => x.Name.ToLower() == classname);
                    if (!WhereArrServices.Any())
                        classname = ArrServices.FirstOrDefault().Name;
                }
                else
                {
                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"服务不存在");
                    return retobj;
                }
                //3. 创建客户端代理代理类
                ServiceDescriptionImporter srvDescInporter = new ServiceDescriptionImporter();
                srvDescInporter.ProtocolName = "Soap";//指定访问协议
                srvDescInporter.Style = ServiceDescriptionImportStyle.Client;//生成客户端代理，默认。
                srvDescInporter.AddServiceDescription(srvDesc, "", ""); //添加WSDL文档。
                //4 .使用 CodeDom 编译客户端代理类。
                CodeNamespace codeNamespce = new CodeNamespace(nameSpace);
                CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
                codeCompileUnit.Namespaces.Add(codeNamespce);
                srvDescInporter.Import(codeNamespce, codeCompileUnit);
                //代码生成器
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                //表示用于调用编译器的参数。
                System.CodeDom.Compiler.CompilerParameters parameter = new System.CodeDom.Compiler.CompilerParameters();
                parameter.GenerateExecutable = false;   //设置是否生成可执行文件。
                parameter.GenerateInMemory = true;      //设置是否在内存中生成输出。
                //parameter.OutputAssembly = HttpContext.Current.Server.MapPath("/" + classname) + "/WebServiceDynamic.dll"; // 可以指定你所需的任何文件名。  
                parameter.ReferencedAssemblies.Add("System.dll");   //ReferencedAssemblies  获取当前项目所引用的程序集。
                parameter.ReferencedAssemblies.Add("System.XML.dll");
                parameter.ReferencedAssemblies.Add("System.Web.Services.dll");
                parameter.ReferencedAssemblies.Add("System.Data.dll");

                //获取从编译器返回的编译结果。
                System.CodeDom.Compiler.CompilerResults cr = provider.CompileAssemblyFromDom(parameter, codeCompileUnit);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    //throw new Exception(sb.ToString());
                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)sb.ToString());
                    return retobj;
                }
                //已程编译序集的零时路径
                string pathAssenbly = cr.PathToAssembly;
                //获取已编译的程序集，然后通过反射进行动态调用。
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type WebServiceType = assembly.GetType(nameSpace + "." + classname, true, true); // 如果在前面为代理类添加了命名空间，此处需要将命名空间添加到类型前面。
                if (WebServiceType != null)
                {
                    object obj = Activator.CreateInstance(WebServiceType);//创建实例
                    var WebServiceProtitys = obj.GetType().GetProperties();
                    var WhereCredentials = WebServiceProtitys.Where(x => x.Name.IndexOf("Credentials") >= 0);
                    if (WhereCredentials.Any())
                    {
                        if (!string.IsNullOrEmpty(UserName))
                        {
                            WhereCredentials.FirstOrDefault().SetValue(obj, new NetworkCredential(UserName, PasswordStr));
                        }
                    }
                    MethodInfo[] ServiceMethods = WebServiceType.GetMethods();//获取所有方法
                    System.Reflection.MethodInfo mi = ServiceMethods.Where(x => x.Name.ToLower() == methodname.ToLower()).FirstOrDefault(); //获取指定方法
                    if (mi != null)
                    {
                        List<object> MethodObj = new List<object>();
                        if (args != null)
                        {
                            ParameterInfo[] Parameters = mi.GetParameters();//获取所有参数
                            for (int i = 0; i < Parameters.Length; i++)
                            {
                                if (args.Length > i)
                                {
                                    ParameterInfo itemMethodParm = Parameters[i];
                                    Type MethodParmType = itemMethodParm.ParameterType;
                                    if (MethodParmType.GetInterface("IEnumerable", false) != null &&
                                        (MethodParmType.Name.ToLower().IndexOf("string") < 0 ||
                                        (MethodParmType.Name.ToLower().IndexOf("string") >= 0 &&
                                        (MethodParmType.Name.ToLower().IndexOf("[]") > 0 || MethodParmType.Name.ToLower().IndexOf("<") > 0))))
                                    {
                                        //是List数组还是Array数组
                                        bool IsList = true;

                                        #region  创建List<T> 实例 并赋值 不管是否是 Array数组 都先实例化 List数组

                                        Type ListTType = null;//泛型类
                                        var IEnumerableTypes = MethodParmType.GetGenericArguments();
                                        if (IEnumerableTypes.Any())
                                        {
                                            //List<> 数组
                                            ListTType = IEnumerableTypes[0];
                                        }
                                        else
                                        {
                                            //数组
                                            ListTType = null;//数组类型
                                            ListTType = assembly.GetType(MethodParmType.FullName.Replace("[]", ""));
                                            IsList = false;
                                        }
                                        if (ListTType != null)
                                        {
                                            Type ListType = typeof(List<>);
                                            ListType = ListType.MakeGenericType(ListTType);
                                            //创建List<T> 数组 实例
                                            var ObjListT = Activator.CreateInstance(ListType);
                                            //List数组的Add方法
                                            MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                            if (AddMethodInfo != null)
                                            {
                                                Type argsType = args[i].GetType();
                                                if (argsType.GetInterface("IEnumerable", false) != null &&
                                                (argsType.Name.ToLower().IndexOf("string") < 0 ||
                                                (argsType.Name.ToLower().IndexOf("string") >= 0 &&
                                                (argsType.Name.ToLower().IndexOf("[]") > 0 || argsType.Name.ToLower().IndexOf("<") > 0))))
                                                {
                                                    var ArrIEnumerable = args[i] as System.Collections.IEnumerable;
                                                    foreach (var itemIE in ArrIEnumerable)
                                                    {
                                                        //转换类型 并相同字段赋值
                                                        var ListitemObj = SetSamaProtity(ListTType, itemIE, assembly, true);
                                                        //执行 List数组的Add方法
                                                        AddMethodInfo.Invoke(ObjListT, new object[] { ListitemObj });
                                                    }
                                                    if (IsList)
                                                    {
                                                        MethodObj.Add(ObjListT);
                                                    }
                                                    else
                                                    {
                                                        MethodInfo ToArrayMethodInfo = ListType.GetMethod("ToArray");
                                                        if (ToArrayMethodInfo != null)
                                                        {
                                                            var ArrObj = ToArrayMethodInfo.Invoke(ObjListT, null);
                                                            MethodObj.Add(ArrObj);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"参数类型不匹配");
                                                    return retobj;
                                                }
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        MethodObj.Add(SetSamaProtity(MethodParmType, args[i], assembly, true));
                                    }
                                }
                                else
                                    MethodObj.Add(null);
                            }
                        }
                        else
                        {
                            MethodObj = new List<object>();
                        }

                        bool IsWriteServiceLog = false;//是否保留报文
                        if (StrWriteServiceLog == "1")
                        {
                            IsWriteServiceLog = true;
                        }
                        else if (StrWriteServiceLog == "是")
                        {
                            IsWriteServiceLog = true;
                        }
                        else if (StrWriteServiceLog.ToLower() == "true")
                        {
                            IsWriteServiceLog = true;
                        }

                        string LabelStr = methodname + "(" + url + ")";
                        //报文
                        string MethodParamJsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(MethodObj);
                        if (IsWriteServiceLog)
                        {
                            TMI.Web.Extensions.Common.WriteLog_LocalByRWLogLocker(MethodParamJsonObj, ServiceLogAddress, true, false, LabelStr, null, WebServiceLockObj.ORWLogLocker);
                            //WebServiceLockObj.ORWLogLocker.AcquireWriterLock(500);
                            //SQLDALHelper.WriteLogHelper.WriteLog(MethodParamJsonObj, ServiceLogAddress, true, false, LabelStr);
                            //WebServiceLockObj.ORWLogLocker.ReleaseWriterLock();
                        }

                        var _Methodretobj = mi.Invoke(obj, MethodObj.ToArray());
                        string MethodretobjStr = "";
                        if (_Methodretobj != null)
                        {
                            if (!_Methodretobj.GetType().Equals(typeof(System.String)))
                                MethodretobjStr = Newtonsoft.Json.JsonConvert.SerializeObject(_Methodretobj);
                            else
                                MethodretobjStr = _Methodretobj.ToString();
                        }

                        //if (MethodretobjStr.Contains("锁定"))
                        //{
                        //    WebdbContext AppContxt = new WebdbContext();
                        //    TMLockJson OTMLockJson = new TMLockJson();
                        //    OTMLockJson.IsUnLock = false;
                        //    OTMLockJson.JsonStr = MethodParamJsonObj;
                        //    OTMLockJson.LockKey = "锁定";
                        //    OTMLockJson.Url = url;
                        //    OTMLockJson.MethodName = methodname;
                        //    OTMLockJson.UserName = UserName;
                        //    OTMLockJson.PasswordStr = PasswordStr;
                        //    OTMLockJson.retErrTypeStr = retErrTypeStr;
                        //    var dbSet = AppContxt.TMLockJson.Attach(OTMLockJson);
                        //    dbSet.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                        //    AppContxt.Entry(OTMLockJson).State = System.Data.Entity.EntityState.Added;
                        //    AppContxt.SaveChanges();
                        //}

                        var Methodretobj = mi.Invoke(obj, MethodObj.ToArray());
                        Tuple<bool, object> _retobj = new Tuple<bool, object>(true, Methodretobj);
                        return _retobj;
                    }
                    else
                    {
                        Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"要执行的方法不存在");
                        return retobj;
                    }
                }
                else
                {
                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"找不到可执行的WebService服务类Type");
                    return retobj;
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
                Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)TMI.Web.Extensions.Common.GetExceptionMsg(ex));
                return retobj;
            }
        }

        /// <summary>
        /// 设置相同属性值
        /// </summary>
        /// <typeparam name="SetType">要设置的类型 Type</typeparam>
        /// <typeparam name="GetobjModel">获取相同值的 数据</typeparam>
        /// <param name="IngoreFieldCase">是否区分大小写</param>
        /// <returns></returns>
        public static object SetSamaProtity(Type SetType, Object GetobjModel, System.Reflection.Assembly assembly, bool IngoreFieldCase = false)
        {
            #region 赋值相同项

            object SetobjModel = null;
            if (SetType != null)
            {
                SetobjModel = Activator.CreateInstance(SetType);
            }
            else
                return null;

            System.Reflection.PropertyInfo[] Set_PropertyInfos = SetobjModel == null ? new System.Reflection.PropertyInfo[] { } : SetobjModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            System.Reflection.PropertyInfo[] Get_PropertyInfos = new System.Reflection.PropertyInfo[] { };
            Type Get_Type = GetobjModel.GetType();
            bool IsDynamic = false;
            if (Get_Type.FullName.IndexOf("Dynamic") > 0)
            {
                IsDynamic = true;
            }
            else
            {
                if (GetobjModel != null)
                {
                    Get_PropertyInfos = GetobjModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                }
                else
                {
                    return null;
                }
            }

            if (!Set_PropertyInfos.Any())
            {
                System.Reflection.FieldInfo[] Set_FieldInfos = SetobjModel == null ? new System.Reflection.FieldInfo[] { } : SetobjModel.GetType().GetFields().Where(x => x.MemberType == MemberTypes.Field).ToArray();
                //遍历该model实体的所有字段
                foreach (System.Reflection.FieldInfo fi in Set_FieldInfos)
                {
                    //设置值
                    var SetObjVal = fi.GetValue(SetobjModel);
                    string DataType = fi.FieldType.Name;
                    //获取值
                    object GetObjVal = null;
                    if (IsDynamic)
                    {
                        var WhereGetfi_s = ((IDictionary<string, object>)GetobjModel).Where(x => IngoreFieldCase ? (x.Key.ToUpper() == fi.Name.ToUpper()) : x.Key == fi.Name);
                        if (WhereGetfi_s.Any())
                        {
                            var Getfi = WhereGetfi_s.First();
                            GetObjVal = Getfi.Value;
                        }
                    }
                    else
                    {
                        var WhereGetfi_s = Get_PropertyInfos.Where(x => IngoreFieldCase ? (x.Name.ToUpper() == fi.Name.ToUpper()) : x.Name == fi.Name);
                        if (WhereGetfi_s.Any())
                        {
                            var Getfi = WhereGetfi_s.First();
                            GetObjVal = Getfi.GetValue(GetobjModel);
                        }
                    }
                    if (GetObjVal == null)
                        continue;
                    //泛型
                    if (fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //泛型 类型
                        var Arguments = fi.FieldType.GetGenericArguments();
                        Type ChildType = Arguments[0].GetType();
                    }
                    //判断是否派生自IEnumerable(string 是特殊的数组)
                    else //if (fi.FieldType.GetInterface("IEnumerable", false) != null && DataType.ToLower().IndexOf("string") < 0)
                        if (fi.FieldType.GetInterface("IEnumerable", false) != null &&
                        (fi.FieldType.Name.ToLower().IndexOf("string") < 0 ||
                        (fi.FieldType.Name.ToLower().IndexOf("string") >= 0 &&
                        (fi.FieldType.Name.ToLower().IndexOf("[]") > 0 || fi.FieldType.Name.ToLower().IndexOf("<") > 0))))
                        {
                            var Arrobjval = GetObjVal as System.Collections.IEnumerable;

                            //是List数组还是Array数组
                            bool IsList = true;

                            #region  创建List<T> 实例 并赋值

                            Type ListTType = null;//泛型类
                            var IEnumerableTypes = fi.FieldType.GetGenericArguments();
                            if (IEnumerableTypes.Any())
                            {
                                //List<> 数组
                                ListTType = IEnumerableTypes[0];
                            }
                            else
                            {
                                //数组
                                ListTType = null;//数组类型
                                ListTType = assembly.GetType(fi.FieldType.FullName.Replace("[]", ""));
                                IsList = false;
                            }

                            Type ListType = typeof(List<>);
                            ListType = ListType.MakeGenericType(ListTType);
                            //创建List数组实例
                            var ObjListT = Activator.CreateInstance(ListType);
                            Type argsType = GetObjVal.GetType();
                            //if (argsType.GetInterface("IEnumerable", false) != null && (argsType.Name.ToLower().IndexOf("string") < 0 && argsType.Name.ToLower().IndexOf("[]") < 0 && argsType.Name.ToLower().IndexOf("<") < 0))
                            if (argsType.GetInterface("IEnumerable", false) != null &&
                            (argsType.Name.ToLower().IndexOf("string") < 0 ||
                            (argsType.Name.ToLower().IndexOf("string") >= 0 &&
                            (argsType.Name.ToLower().IndexOf("[]") > 0 || argsType.Name.ToLower().IndexOf("<") > 0))))
                            {
                                MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                if (AddMethodInfo != null)
                                {
                                    foreach (var item in Arrobjval)
                                    {
                                        var obj = SetSamaProtity(ListTType, item, assembly, IngoreFieldCase);
                                        AddMethodInfo.Invoke(ObjListT, new object[] { obj });
                                    }
                                }

                                if (IsList)
                                {
                                    TMI.Web.Extensions.Common.setProtityValue(SetobjModel, fi, ObjListT);
                                }
                                else
                                {
                                    MethodInfo ToArrayMethodInfo = ListType.GetMethod("ToArray");
                                    if (ToArrayMethodInfo != null)
                                    {
                                        var ArrObj = ToArrayMethodInfo.Invoke(ObjListT, null);
                                        TMI.Web.Extensions.Common.setProtityValue(SetobjModel, fi, ArrObj);
                                    }
                                }
                            }

                            #endregion
                        }
                        //判断是否是 基元类型 string struct datetime decimal 为特殊的 基元类型
                        //基元类型：sbyte / byte / short / ushort /int / uint / long / ulong / char / float / double / bool
                        else if (fi.FieldType.IsClass && !fi.FieldType.IsPrimitive && fi.FieldType.Name.ToLower().IndexOf("string") < 0)
                        {
                            var obj = SetSamaProtity(fi.FieldType, GetObjVal, assembly, IngoreFieldCase);
                            TMI.Web.Extensions.Common.setProtityValue(SetobjModel, fi, obj);
                        }
                        else
                        {
                            TMI.Web.Extensions.Common.setProtityValue(SetobjModel, fi, GetObjVal);
                        }
                }
            }
            else
            {
                //遍历该model实体的所有字段
                foreach (System.Reflection.PropertyInfo fi in Set_PropertyInfos)
                {
                    //设置值
                    var SetObjVal = fi.GetValue(SetobjModel);
                    string DataType = fi.PropertyType.Name;
                    //获取值
                    object GetObjVal = null;
                    if (IsDynamic)
                    {
                        var WhereGetfi_s = ((IDictionary<string, object>)GetobjModel).Where(x => IngoreFieldCase ? (x.Key.ToUpper() == fi.Name.ToUpper()) : x.Key == fi.Name);
                        if (WhereGetfi_s.Any())
                        {
                            var Getfi = WhereGetfi_s.First();
                            GetObjVal = Getfi.Value;
                        }
                    }
                    else
                    {
                        var WhereGetfi_s = Get_PropertyInfos.Where(x => IngoreFieldCase ? (x.Name.ToUpper() == fi.Name.ToUpper()) : x.Name == fi.Name);
                        if (WhereGetfi_s.Any())
                        {
                            var Getfi = WhereGetfi_s.First();
                            GetObjVal = Getfi.GetValue(GetobjModel);
                        }
                    }


                    if (GetObjVal == null)
                        continue;
                    //泛型 decimal? int? 等
                    if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //泛型 类型
                        var Arguments = fi.PropertyType.GetGenericArguments();
                        //Type ChildType = null;
                        //判断是否是 基元类型 string struct 为特殊的 基元类型
                        //基元类型：sbyte / byte / short / ushort /int / uint / long / ulong / char / float / double / bool
                        //if (Arguments[0].IsPrimitive || Arguments[0].Name.ToLower().IndexOf("string") >= 0)
                        //{
                        //}
                        TMI.Web.Extensions.Common.setProtityValue(SetobjModel, fi, GetObjVal);
                    }
                    //判断是否派生自IEnumerable(string 是特殊的数组)
                    else //if (fi.PropertyType.GetInterface("IEnumerable", false) != null && DataType.ToLower().IndexOf("string") < 0)
                        if (fi.PropertyType.GetInterface("IEnumerable", false) != null &&
                        (fi.PropertyType.Name.ToLower().IndexOf("string") < 0 ||
                        (fi.PropertyType.Name.ToLower().IndexOf("string") >= 0 &&
                        (fi.PropertyType.Name.ToLower().IndexOf("[]") > 0 || fi.PropertyType.Name.ToLower().IndexOf("<") > 0))))
                        {
                            var Arrobjval = GetObjVal as System.Collections.IEnumerable;

                            //是List数组还是Array数组
                            bool IsList = true;

                            #region  创建List<T> 实例 并赋值

                            Type ListTType = null;//泛型类
                            var IEnumerableTypes = fi.PropertyType.GetGenericArguments();
                            if (IEnumerableTypes.Any())
                            {
                                //List<> 数组
                                ListTType = IEnumerableTypes[0];
                            }
                            else
                            {
                                //数组
                                ListTType = null;//数组类型
                                ListTType = assembly.GetType(fi.PropertyType.FullName.Replace("[]", ""));
                                IsList = false;
                            }

                            Type ListType = typeof(List<>);
                            ListType = ListType.MakeGenericType(ListTType);
                            //创建List数组实例
                            var ObjListT = Activator.CreateInstance(ListType);
                            Type argsType = GetObjVal.GetType();
                            //if (argsType.GetInterface("IEnumerable", false) != null && (argsType.Name.ToLower().IndexOf("string") < 0 && argsType.Name.ToLower().IndexOf("[]") < 0 && argsType.Name.ToLower().IndexOf("<") < 0))
                            if (argsType.GetInterface("IEnumerable", false) != null &&
                            (argsType.Name.ToLower().IndexOf("string") < 0 ||
                            (argsType.Name.ToLower().IndexOf("string") >= 0 &&
                            (argsType.Name.ToLower().IndexOf("[]") > 0 || argsType.Name.ToLower().IndexOf("<") > 0))))
                            {
                                MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                if (AddMethodInfo != null)
                                {
                                    foreach (var item in Arrobjval)
                                    {
                                        var obj = SetSamaProtity(ListTType, item, assembly, IngoreFieldCase);
                                        AddMethodInfo.Invoke(ObjListT, new object[] { obj });
                                    }
                                }
                                if (IsList)
                                {
                                    TMI.Web.Extensions.Common.setProtityValue(SetobjModel, fi, ObjListT);
                                }
                                else
                                {
                                    MethodInfo ToArrayMethodInfo = ListType.GetMethod("ToArray");
                                    if (ToArrayMethodInfo != null)
                                    {
                                        var ArrObj = ToArrayMethodInfo.Invoke(ObjListT, null);
                                        TMI.Web.Extensions.Common.setProtityValue(SetobjModel, fi, ArrObj);
                                    }
                                }
                            }

                            #endregion
                        }
                        //判断是否是 基元类型 string struct datetime decimal 为特殊的 基元类型
                        //基元类型：sbyte / byte / short / ushort /int / uint / long / ulong / char / float / double / bool
                        else if (fi.PropertyType.IsClass && !fi.PropertyType.IsPrimitive && fi.PropertyType.Name.ToLower().IndexOf("string") < 0)
                        {
                            var obj = SetSamaProtity(fi.PropertyType, GetObjVal, assembly, IngoreFieldCase);
                            TMI.Web.Extensions.Common.setProtityValue(SetobjModel, fi, obj);
                        }
                        else
                        {
                            TMI.Web.Extensions.Common.setProtityValue(SetobjModel, fi, GetObjVal);
                        }
                }
            }

            #endregion

            return SetobjModel;
        }

        /// </summary>  
        /// 根据指定的信息，调用远程WebService方法  
        /// </summary>  
        /// <param name="url">WebService的http形式的地址</param>  
        /// <param name="methodname">欲调用的WebService的方法名</param>  
        /// <param name="args">参数列表</param>  
        /// <param name="classname">欲调用的WebService的类名（不包括命名空间前缀）</param>  
        /// <returns>WebService的执行结果</returns>  
        public static Tuple<bool, object> InvokeWebServiceOutPutDLL(string url, string methodname, object[] args, string _ModelPath, OracleParameter[] Param_s, string UserName = "", string PasswordStr = "", string classname = "", string retErrTypeStr = "")
        {
            string ErrLogSubject = "调用" + methodname + "接口错误";
            try
            {
                #region 读取 重复发送TM 配置项

                if (!string.IsNullOrWhiteSpace(TMMethodJson))
                {
                    try
                    {
                        ArrTMMethod = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(TMMethodJson);
                    }
                    catch (Exception)
                    {

                    }
                }
                if (!string.IsNullOrWhiteSpace(TMErrKeyJson))
                {
                    try
                    {
                        ArrTMErrKey = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(TMErrKeyJson);
                    }
                    catch (Exception)
                    {

                    }
                }
                if (!string.IsNullOrWhiteSpace(SendToMailErrJson))
                {
                    try
                    {
                        ArrSendToMailErr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(SendToMailErrJson);
                    }
                    catch (Exception)
                    {

                    }
                }

                #endregion

                string nameSpace = "WebServiceDynamic";
                string XMLPath = System.Configuration.ConfigurationManager.AppSettings["WebServiceSettingModelPath"] ?? "\\WebServiceWSDL\\";
                if (_ModelPath.IndexOf(".") > 0)
                    _ModelPath = _ModelPath.Substring(0, _ModelPath.LastIndexOf("\\"));
                if (_ModelPath.IndexOf(":") <= 0)
                {
                    if (_ModelPath.IndexOf(XMLPath) < 0)
                        _ModelPath = XMLPath + _ModelPath;
                    if (HttpContext.Current == null || HttpContext.Current.Server == null)
                        _ModelPath = TMI.Web.Extensions.Common.GetMapPath(_ModelPath);
                    else
                        _ModelPath = HttpContext.Current.Server.MapPath(_ModelPath);
                }

                if (!Directory.Exists(_ModelPath))
                {
                    Directory.CreateDirectory(_ModelPath);
                }

                //动态WebService Type
                Type WebServiceType = null;
                System.Reflection.Assembly assembly = null;
                //WebService名称
                string DLLName = getMd5Str(url);
                string DLLFilePath = _ModelPath + "\\" + DLLName + ".dll";
                if (checkIsInCache(DLLFilePath))
                {
                    assembly = Assembly.LoadFrom(DLLFilePath);
                    Type[] types = assembly.GetTypes();
                    string objTypeName = "";
                    foreach (Type t in types)
                    {
                        if (t.BaseType == typeof(System.Web.Services.Protocols.SoapHttpClientProtocol))
                        {
                            objTypeName = t.Name;
                            WebServiceType = t;
                            break;
                        }
                    }
                    //WebServiceLockObj.ORWLogLocker.AcquireWriterLock(500);
                    //WriteLogHelper.WriteLog("objTypeName-" + objTypeName + "-" + DLLFilePath);
                    //WebServiceLockObj.ORWLogLocker.ReleaseWriterLock();
                }
                if (WebServiceType == null)
                {
                    #region 动态创建 WebService类

                    //WebServiceLockObj.ORWLogLocker.AcquireWriterLock(500);
                    //WriteLogHelper.WriteLog("动态创建 WebService类-" + url + "-" + DLLFilePath);
                    //WebServiceLockObj.ORWLogLocker.ReleaseWriterLock();

                    //1.使用WebClient 下载WSDL信息
                    WebClient wc = new WebClient();
                    if (!string.IsNullOrEmpty(UserName))
                        wc.Credentials = new NetworkCredential(UserName, PasswordStr);
                    Stream stream = wc.OpenRead(url);
                    //2.创建和格式化WSDL文档
                    ServiceDescription srvDesc = ServiceDescription.Read(stream);
                    List<System.Web.Services.Description.Service> ArrServices = new List<System.Web.Services.Description.Service>();
                    foreach (System.Web.Services.Description.Service item in srvDesc.Services)
                    {
                        ArrServices.Add(item);
                    }
                    if (ArrServices.Any())
                    {
                        var WhereArrServices = ArrServices.Where(x => x.Name.ToLower() == classname);
                        if (!WhereArrServices.Any())
                            classname = ArrServices.FirstOrDefault().Name;
                    }
                    else
                    {
                        WirteLog(ErrLogSubject, "调用" + methodname + "错误", url, "服务不存在", methodname, Param_s);
                        Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"服务不存在");
                        return retobj;
                    }

                    //加锁
                    lock (WebServiceLockObj.lockAssemblyWebServiceFileCopy)
                    {
                        //3. 创建客户端代理代理类
                        ServiceDescriptionImporter srvDescInporter = new ServiceDescriptionImporter();
                        srvDescInporter.ProtocolName = "Soap";//指定访问协议
                        srvDescInporter.Style = ServiceDescriptionImportStyle.Client;//生成客户端代理，默认。
                        srvDescInporter.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateNewAsync;
                        srvDescInporter.AddServiceDescription(srvDesc, "", ""); //添加WSDL文档。
                        //4 .使用 CodeDom 编译客户端代理类。
                        CodeNamespace codeNamespce = new CodeNamespace(nameSpace);// 为代理类添加命名空间，缺省为全局空间。  
                        CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
                        codeCompileUnit.Namespaces.Add(codeNamespce);
                        srvDescInporter.Import(codeNamespce, codeCompileUnit);
                        //代码生成器
                        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                        //表示用于调用编译器的参数。
                        System.CodeDom.Compiler.CompilerParameters parameter = new System.CodeDom.Compiler.CompilerParameters();
                        parameter.GenerateExecutable = false;   //设置是否生成可执行文件。
                        parameter.GenerateInMemory = false;//是否在内存中输出
                        parameter.IncludeDebugInformation = false;//是否在已编译的可执行文件中包含调试信息  
                        //parameter.OutputAssembly = DLLName + "_" + (new Random().Next(1, 999).ToString("000")) + ".dll";// "WebServiceDynamic.dll"; // 可以指定你所需的任何文件名。 
                        parameter.ReferencedAssemblies.Add("System.dll");   //ReferencedAssemblies  获取当前项目所引用的程序集。
                        parameter.ReferencedAssemblies.Add("System.XML.dll");
                        parameter.ReferencedAssemblies.Add("System.Web.Services.dll");
                        parameter.ReferencedAssemblies.Add("System.Data.dll");

                        //获取从编译器返回的编译结果。
                        System.CodeDom.Compiler.CompilerResults cr = provider.CompileAssemblyFromDom(parameter, codeCompileUnit);
                        provider.Dispose();
                        if (true == cr.Errors.HasErrors)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                            {
                                sb.Append(ce.ToString());
                                sb.Append(System.Environment.NewLine);
                            }
                            //throw new Exception(sb.ToString());
                            WirteLog(ErrLogSubject, "调用" + methodname + "错误", url, "创建DLL错误，" + sb.ToString(), methodname, Param_s);
                            Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)sb.ToString());
                            return retobj;
                        }
                        File.Copy(cr.PathToAssembly, DLLFilePath);

                        //获取已编译的程序集，然后通过反射进行动态调用。
                        assembly = Assembly.LoadFrom(DLLFilePath);
                        //WriteLogHelper.WriteLog("assemblyDLL-" + nameSpace + "." + classname);
                        WebServiceType = assembly.GetType(nameSpace + "." + classname); // 如果在前面为代理类添加了命名空间，此处需要将命名空间添加到类型前面。
                    }

                    #endregion
                }
                if (WebServiceType != null)
                {
                    object obj = Activator.CreateInstance(WebServiceType);//创建实例
                    //WriteLogHelper.WriteLog("CreateInstance-" + WebServiceType.Name);
                    var WebServiceProtitys = obj.GetType().GetProperties();
                    var WhereCredentials = WebServiceProtitys.Where(x => x.Name.IndexOf("Credentials") >= 0);
                    if (WhereCredentials.Any())
                    {
                        if (!string.IsNullOrEmpty(UserName))
                        {
                            WhereCredentials.FirstOrDefault().SetValue(obj, new NetworkCredential(UserName, PasswordStr));
                        }
                    }

                    MethodInfo[] ServiceMethods = WebServiceType.GetMethods();//获取所有方法
                    System.Reflection.MethodInfo mi = ServiceMethods.Where(x => x.Name.ToLower() == methodname.ToLower()).FirstOrDefault(); //获取指定方法
                    if (mi != null)
                    {
                        #region 服务方法的参数类型-转换

                        List<object> MethodObj = new List<object>();
                        if (args != null)
                        {
                            ParameterInfo[] Parameters = mi.GetParameters();//获取所有参数
                            for (int i = 0; i < Parameters.Length; i++)
                            {
                                if (args.Length > i)
                                {
                                    ParameterInfo itemMethodParm = Parameters[i];
                                    Type MethodParmType = itemMethodParm.ParameterType;
                                    if (MethodParmType.GetInterface("IEnumerable", false) != null &&
                                        (MethodParmType.Name.ToLower().IndexOf("string") < 0 ||
                                        (MethodParmType.Name.ToLower().IndexOf("string") >= 0 &&
                                        (MethodParmType.Name.ToLower().IndexOf("[]") > 0 || MethodParmType.Name.ToLower().IndexOf("<") > 0))))
                                    {
                                        //是List数组还是Array数组
                                        bool IsList = true;

                                        #region  创建List<T> 实例 并赋值 不管是否是 Array数组 都先实例化 List数组

                                        Type ListTType = null;//泛型类
                                        var IEnumerableTypes = MethodParmType.GetGenericArguments();
                                        if (IEnumerableTypes.Any())
                                        {
                                            //List<> 数组
                                            ListTType = IEnumerableTypes[0];
                                        }
                                        else
                                        {
                                            //数组
                                            ListTType = null;//数组类型
                                            string TypeName = MethodParmType.FullName.Replace("[]", "");
                                            ListTType = MethodParmType.Assembly.GetType(TypeName);
                                            IsList = false;
                                        }
                                        if (ListTType != null)
                                        {
                                            Type ListType = typeof(List<>);
                                            ListType = ListType.MakeGenericType(ListTType);
                                            //创建List<T> 数组 实例
                                            var ObjListT = Activator.CreateInstance(ListType);
                                            //List数组的Add方法
                                            MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                            if (AddMethodInfo != null)
                                            {
                                                Type argsType = args[i].GetType();
                                                if (argsType.GetInterface("IEnumerable", false) != null &&
                                                (argsType.Name.ToLower().IndexOf("string") < 0 ||
                                                (argsType.Name.ToLower().IndexOf("string") >= 0 &&
                                                (argsType.Name.ToLower().IndexOf("[]") > 0 || argsType.Name.ToLower().IndexOf("<") > 0))))
                                                {
                                                    var ArrIEnumerable = args[i] as System.Collections.IEnumerable;
                                                    Type itemIEType = null;
                                                    foreach (var itemIE in ArrIEnumerable)
                                                    {
                                                        if (itemIEType == null)
                                                            itemIEType = itemIE.GetType();
                                                        //判断是否是 基元类型 string struct datetime decimal 为特殊的 基元类型
                                                        //基元类型：sbyte / byte / short / ushort /int / uint / long / ulong / char / float / double / bool
                                                        if ((itemIEType.IsPrimitive || itemIEType.IsValueType || itemIEType == typeof(string) || itemIEType == typeof(decimal) || itemIEType == typeof(DateTime)) && itemIEType.Name.ToLower().IndexOf("struct") < 0)//判断是否是 基元类型 string struct 为特殊的 基元类型
                                                        {
                                                            if (itemIEType == ListTType)
                                                            {
                                                                AddMethodInfo.Invoke(ObjListT, new object[] { itemIE });
                                                            }
                                                            else
                                                            {
                                                                var itemobjval = Convert.ChangeType(itemIE, ListTType);
                                                                AddMethodInfo.Invoke(ObjListT, new object[] { itemobjval });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //转换类型 并相同字段赋值
                                                            var ListitemObj = SetSamaProtity(ListTType, itemIE, assembly, true);
                                                            //执行 List数组的Add方法
                                                            AddMethodInfo.Invoke(ObjListT, new object[] { ListitemObj });
                                                        }
                                                    }
                                                    if (IsList)
                                                    {
                                                        MethodObj.Add(ObjListT);
                                                    }
                                                    else
                                                    {
                                                        MethodInfo ToArrayMethodInfo = ListType.GetMethod("ToArray");
                                                        if (ToArrayMethodInfo != null)
                                                        {
                                                            var ArrObj = ToArrayMethodInfo.Invoke(ObjListT, null);
                                                            MethodObj.Add(ArrObj);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    WirteLog(ErrLogSubject, "调用" + methodname + "错误", url, "参数类型不匹配", methodname, Param_s);
                                                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"参数类型不匹配");
                                                    return retobj;
                                                }
                                            }
                                        }

                                        #endregion
                                    }
                                    else if (MethodParmType.IsGenericType && MethodParmType.GetGenericTypeDefinition() == typeof(Nullable<>))//泛型 decimal? int? 等
                                    {
                                        //泛型类型
                                        var Arguments = MethodParmType.GetGenericArguments();
                                        //判断是否是 基元类型 string struct datetime decimal 为特殊的 基元类型
                                        //基元类型：sbyte / byte / short / ushort /int / uint / long / ulong / char / float / double / bool
                                        if ((Arguments[0].IsPrimitive || Arguments[0].IsValueType || Arguments[0] == typeof(string) || Arguments[0] == typeof(decimal) || Arguments[0] == typeof(DateTime)) && Arguments[0].Name.ToLower().IndexOf("struct") < 0)//判断是否是 基元类型 string struct 为特殊的 基元类型
                                        {
                                            if (args[i] != null)
                                            {
                                                //判断是否是 基元类型 string struct 为特殊的 基元类型
                                                var valType = args[i].GetType();
                                                if (valType == MethodParmType)
                                                {
                                                    MethodObj.Add(args[i]);
                                                }
                                                else
                                                {
                                                    var objVal = Activator.CreateInstance(Arguments[0]);
                                                    objVal = Convert.ChangeType(args[i], Arguments[0]);
                                                    MethodObj.Add(objVal);
                                                    //MethodObj.Add(Convert.ChangeType(args[i], MethodParmType));
                                                    //MethodObj.Add(args[i].ChangeType(MethodParmType));
                                                }
                                            }
                                            else
                                                MethodObj.Add(null);
                                        }
                                        else
                                        {
                                            MethodObj.Add(SetSamaProtity(MethodParmType, args[i], assembly, true));
                                        }
                                    }
                                    //判断是否是 基元类型 string struct datetime decimal 为特殊的 基元类型
                                    //基元类型：sbyte / byte / short / ushort /int / uint / long / ulong / char / float / double / bool
                                    else if ((MethodParmType.IsPrimitive || MethodParmType.IsValueType || MethodParmType == typeof(string) || MethodParmType == typeof(decimal) || MethodParmType == typeof(DateTime)) && MethodParmType.Name.ToLower().IndexOf("struct") < 0)
                                    {
                                        //判断是否是 基元类型 string struct 为特殊的 基元类型
                                        var valType = args[i].GetType();
                                        if (valType == MethodParmType)
                                        {
                                            MethodObj.Add(args[i]);
                                        }
                                        else
                                        {
                                            MethodObj.Add(Convert.ChangeType(args[i], valType));
                                            //MethodObj.Add(args[i].ChangeType(MethodParmType));
                                        }
                                    }
                                    else
                                    {
                                        MethodObj.Add(SetSamaProtity(MethodParmType, args[i], assembly, true));
                                    }
                                }
                                else
                                    MethodObj.Add(null);
                            }
                        }
                        else
                        {
                            MethodObj = new List<object>();
                        }

                        #endregion

                        #region 是否保留服务日志

                        bool IsWriteServiceLog = false;//是否保留报文
                        if (StrWriteServiceLog == "1")
                        {
                            IsWriteServiceLog = true;
                        }
                        else if (StrWriteServiceLog == "是")
                        {
                            IsWriteServiceLog = true;
                        }
                        else if (StrWriteServiceLog.ToLower() == "true")
                        {
                            IsWriteServiceLog = true;
                        }

                        string LabelStr = methodname + "(" + url + ")";
                        //报文
                        string MethodParamJsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(MethodObj);
                        if (IsWriteServiceLog)
                        {
                            ////加锁
                            //lock (WebServiceLockObj.lockWebServiceHelperWriteLog)
                            //{
                            //    SQLDALHelper.WriteLogHelper.WriteLog(MethodParamJsonObj, ServiceLogAddress, true, false, LabelStr);
                            //}
                            try
                            {
                                TMI.Web.Extensions.Common.WriteLog_LocalByRWLogLocker(MethodParamJsonObj, ServiceLogAddress, true, false, LabelStr, null, WebServiceLockObj.ORWLogLocker);
                                //WebServiceLockObj.ORWLogLocker.AcquireWriterLock(500);
                                //SQLDALHelper.WriteLogHelper.WriteLog(MethodParamJsonObj, ServiceLogAddress, true, false, LabelStr);
                                //WebServiceLockObj.ORWLogLocker.ReleaseWriterLock();
                            }
                            catch (Exception)
                            {
                                //TMI.Web.Extensions.Common.WriteLog_Local
                            }
                        }

                        #endregion

                        //Invoke返回结果
                        object _Methodretobj = null;
                        //调用WebService 报错
                        bool IsInvokeErr = false;
                        try
                        {
                            _Methodretobj = mi.Invoke(obj, MethodObj.ToArray());
                        }
                        catch (Exception ex)
                        {
                            _Methodretobj = (object)TMI.Web.Extensions.Common.GetExceptionMsg(ex);
                            IsInvokeErr = true;
                        }
                        //Invoke返回结果转换为Json
                        string MethodretobjStr = "";
                        if (_Methodretobj != null)
                        {
                            if (!_Methodretobj.GetType().Equals(typeof(System.String)))
                                MethodretobjStr = Newtonsoft.Json.JsonConvert.SerializeObject(_Methodretobj);
                            else
                                MethodretobjStr = _Methodretobj.ToString();
                        }

                        #region 插入需要重复发送TM的数据 锁单 Server Error 等

                        //if (ArrTMMethod.Any(x => x == methodname.ToUpper()))
                        //{
                        //    var WhereArrTMErrKey = ArrTMErrKey.Where(x => MethodretobjStr.Contains(x));
                        //    if (WhereArrTMErrKey.Any())
                        //    {
                        //        WebdbContext AppContxt = new WebdbContext();
                        //        TMLockJson OTMLockJson = new TMLockJson();
                        //        OTMLockJson.IsUnLock = false;
                        //        OTMLockJson.JsonStr = MethodParamJsonObj;
                        //        OTMLockJson.LockKey = WhereArrTMErrKey.FirstOrDefault();
                        //        OTMLockJson.Url = url;
                        //        OTMLockJson.MethodName = methodname;
                        //        OTMLockJson.UserName = UserName;
                        //        OTMLockJson.PasswordStr = PasswordStr;
                        //        OTMLockJson.retErrTypeStr = retErrTypeStr;
                        //        var dbSet = AppContxt.TMLockJson.Attach(OTMLockJson);
                        //        dbSet.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                        //        AppContxt.Entry(OTMLockJson).State = System.Data.Entity.EntityState.Added;
                        //        AppContxt.SaveChanges();

                        //        #region 异步发送邮件 不会阻塞 主线程

                        //        var WhereArrSendToMailErr = ArrSendToMailErr.Where(x => MethodretobjStr.Contains(x));
                        //        if (WhereArrSendToMailErr.Any())
                        //        {
                        //            new System.Threading.Thread(o =>
                        //            {
                        //                try
                        //                {
                        //                    WebdbContext NewAppContxt = new WebdbContext();
                        //                    List<string> ArrRecMail = new List<string>();
                        //                    List<string> ArrCCMail = new List<string>();
                        //                    var ArrMailRecver = NewAppContxt.MailReceiver.Where(x => WhereArrSendToMailErr.Contains(x.ErrType) && x.ErrMethod.ToUpper() == methodname.ToUpper()).ToList();
                        //                    if (ArrMailRecver.Any())
                        //                    {
                        //                        ArrRecMail = ArrMailRecver.Select(x => x.RecMailAddress).ToList();
                        //                        //第一次 获取所有CCMailAddress 是多个的（以,分割）
                        //                        ArrCCMail = ArrMailRecver.Select(x => x.CCMailAddress).ToList();
                        //                        string CCAddressStr = string.Join(",", ArrCCMail);
                        //                        if (string.IsNullOrWhiteSpace(CCAddressStr))
                        //                        {
                        //                            ArrCCMail = new List<string>();
                        //                            ArrCCMail = CCAddressStr.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        //                        }
                        //                        string Subject = methodname + "-Server Error";
                        //                        string Body = Subject + " 发送内容：" + Newtonsoft.Json.JsonConvert.SerializeObject(MethodObj);
                        //                        MailSendHelper.MailSend OMailSend = new MailSendHelper.MailSend(Subject, Body, ArrRecMail, ArrCCMail);
                        //                        OMailSend.SendMailDisplay = "Michael1019";
                        //                        OMailSend.OSMTPMailSetting = new MailSendHelper.SMTPMailSetting();
                        //                        string ErrMsg = OMailSend.SendMail();
                        //                    }

                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    string ErrMsgStr = TMI.Web.Extensions.Common.GetExceptionMsg(ex);

                        //                    TMI.Web.Extensions.Common.WriteLog_LocalByRWLogLocker("发送邮件错误：" + ErrMsgStr + Newtonsoft.Json.JsonConvert.SerializeObject(MethodObj), "Mail//Error", true);
                        //                    //WebServiceLockObj.ORWLogLocker.AcquireWriterLock(500);
                        //                    //SQLDALHelper.WriteLogHelper.WriteLog("发送邮件错误：" + ErrMsgStr + Newtonsoft.Json.JsonConvert.SerializeObject(MethodObj), "Mail", true);
                        //                    //WebServiceLockObj.ORWLogLocker.ReleaseWriterLock();
                        //                }
                        //            }) { IsBackground = true }.Start();
                        //        }

                        //        #endregion
                        //    }
                        //}

                        #endregion

                        //调用WebService 报错
                        if (IsInvokeErr)
                        {
                            WirteLog(ErrLogSubject, "mi.Invoke", url, MethodretobjStr, methodname, Param_s);
                            Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)MethodretobjStr);
                            return retobj;
                        }
                        else
                        {
                            Tuple<bool, object> _retobj = new Tuple<bool, object>(true, MethodretobjStr);
                            return _retobj;
                        }
                    }
                    else
                    {
                        WirteLog(ErrLogSubject, "调用" + methodname + "错误", url, "要执行的方法不存在", methodname, Param_s);
                        Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"要执行的方法不存在");
                        return retobj;
                    }
                }
                else
                {
                    WirteLog(ErrLogSubject, "调用" + methodname + "错误", url, "找不到可执行的WebService服务类Type", methodname, Param_s);
                    Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)"找不到可执行的WebService服务类Type");
                    return retobj;
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
                string ErrMsg = TMI.Web.Extensions.Common.GetExceptionMsg(ex);
                WirteLog(ErrLogSubject, "catch", url, ErrMsg, methodname, Param_s);
                Tuple<bool, object> retobj = new Tuple<bool, object>(false, (object)ErrMsg);
                return retobj;
            }
        }

        /// <summary>
        /// 获取MD5字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string getMd5Str(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();

            //Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
            //byte[] unicodeText = new byte[str.Length * 2];
            //enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);
            //MD5 md5 = new MD5CryptoServiceProvider();
            //byte[] result = md5.ComputeHash(unicodeText);
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < result.Length; i++)
            //{
            //    sb.Append(result[i].ToString("X2"));
            //}
            //return sb.ToString();
        }

        /// <summary>                           
        /// 是否已经存在该程序集                                
        /// </summary>                                  
        /// <returns>false:不存在该程序集,true:已经存在该程序集</returns>                                
        private static bool checkIsInCache(string dllFilePath)
        {
            if (File.Exists(dllFilePath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加服务错误日志
        /// </summary>
        /// <param name="notificationTag"></param>
        /// <param name="subject"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="content"></param>
        /// <param name="messageType"></param>
        public static void WirteLog(string subject, string key1, string key2, string content, string methodname, OracleParameter[] Param_s, TMI.Web.Models.EnumType.MessageType MsgType = TMI.Web.Models.EnumType.MessageType.Error)
        {
            try
            {
                string Key1_Str = "";
                foreach (var item in Param_s)
                {
                    if (string.IsNullOrEmpty(Key1_Str))
                        Key1_Str += item.ParameterName + ":" + item.Value.ToString();
                    else
                        Key1_Str += "," + item.ParameterName + ":" + item.Value.ToString();
                }
                //清除其他实体的操作
                appContext = new WebdbContext();
                unitOfWork_ = new Repository.Pattern.Ef6.UnitOfWork(appContext);
                foreach (var item in appContext.ChangeTracker.Entries())
                {
                    item.State = System.Data.Entity.EntityState.Unchanged;
                }

                var NotificationRep = unitOfWork_.Repository<Notification>();
                var MessageRep = unitOfWork_.Repository<TMI.Web.Models.Message>();
                string name = TMI.Web.Models.EnumType.NotificationTag.Servive.ToString();
                var notification = NotificationRep.Queryable().Where(x => x.Name == name).FirstOrDefault();
                if (notification != null)
                {
                    TMI.Web.Models.Message message = new TMI.Web.Models.Message();
                    message.Content = content;
                    message.Key1 = Key1_Str;
                    message.Key2 = key2;
                    message.CreatedDate = DateTime.Now;
                    message.NewDate = DateTime.Now;
                    message.NotificationId = notification.Id;
                    message.Subject = subject;
                    message.Type = MsgType.ToString();
                    message.CreatedBy = methodname;
                    message.CreatedDate = DateTime.Now;
                    MessageRep.Insert(message);
                }
                unitOfWork_.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

    }

}