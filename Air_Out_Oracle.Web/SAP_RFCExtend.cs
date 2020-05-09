//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using SAP.Middleware.Connector;
//using System.Collections;

//namespace AirOut.Web
//{
//    public class SAP_RFCExtend
//    {
//        private static string RfcParamJsonStr = "";
//        private static RfcConfigParameters ORfcCParam = null;
//        private static RfcDestination dest = null;
//        static SAP_RFCExtend()
//        {
//            RfcParamJsonStr = System.Configuration.ConfigurationManager.AppSettings["ECCRfcApi"] ?? "";
//            if (string.IsNullOrEmpty(RfcParamJsonStr))
//            {
//                var dictParam = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(RfcParamJsonStr);
//                ORfcCParam = new RfcConfigParameters();
//                ORfcCParam.AddRange(dictParam.ToArray());
//            }
//            dest = RfcDestinationManager.GetDestination(ORfcCParam);
//        }

//        /// <summary>
//        /// 执行 SAP-RFC 方法
//        /// </summary>
//        /// <param name="RFCname">方法名</param>
//        /// <param name="argStr">参数</param>
//        /// <param name="IT_Param">表</param>
//        /// <param name="IS_Param">结构</param>
//        /// <returns></returns>
//        public static IRfcFunction RFCfun(string RFCname, Hashtable argStr, Hashtable IT_Param = null, Hashtable IS_Param = null)
//        {
//            if (string.IsNullOrEmpty(RFCname)) return null;
//            //RFC调用函数
//            RfcRepository rfcrep = dest.Repository;
//            IRfcFunction myfun = null;
//            myfun = rfcrep.CreateFunction(RFCname);//SAP里面的函数名称/RFCname Z1I_TM_TM064

//            #region 表

//            IRfcTable it_ano = null;
//            if (IT_Param != null)
//            {
//                foreach (string key in IT_Param.Keys)
//                {
//                    it_ano = myfun.GetTable(key);
//                    Hashtable[] _list = (Hashtable[])IT_Param[key];
//                    for (int idx = 0; idx < _list.Length; idx++)
//                    {
//                        it_ano.Append();
//                        foreach (string pkey in _list[idx].Keys)
//                        {
//                            it_ano.SetValue(pkey, _list[idx][pkey]);
//                        }
//                    }
//                }
//            }

//            #endregion

//            #region 结构

//            IRfcStructure i_structure = null;
//            if (IS_Param != null)
//            {
//                foreach (string key in IS_Param.Keys)
//                {
//                    i_structure = myfun.GetStructure(key);
//                    Hashtable d = (Hashtable)IS_Param[key];
//                    foreach (string d_key in d.Keys)
//                    {
//                        i_structure.SetValue(d_key, d[d_key]);
//                    }
//                }
//            }

//            #endregion

//            #region 直接参数赋值

//            if (argStr != null)
//            {
//                foreach (DictionaryEntry de in argStr)
//                {
//                    myfun.SetValue(de.Key.ToString(), de.Value);
//                }
//            }

//            #endregion

//            //执行
//            myfun.Invoke(dest);

//            return myfun;
//        }
//    }
//}