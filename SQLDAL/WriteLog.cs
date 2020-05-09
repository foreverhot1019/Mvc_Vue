using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.RegularExpressions;

namespace SQLDALHelper
{
    public class WriteLogHelper
    {
        //public static readonly string WriteToLogPath = System.Configuration.ConfigurationManager.AppSettings["WriteToLog"];

        #region Write Log information

        /// <summary>
        /// WriteLog
        /// </summary>
        /// <param name="nmm_strMessage">内容</param>
        /// <param name="LogFilePathName">日志目录名</param>
        /// <param name="DayFilePathName">是否生成 日目录</param>
        /// <param name="HourFileName">是否生成 小时文件名</param>
        /// <param name="FileName">文件名称（HourFileName为true时，设置无效）</param>
        public static void WriteLog(string nmm_strMessage, string LogFilePathName = "SQLDALHelper", bool DayFilePathName = false, bool HourFileName = false,string FileName="")
        {
            string retErrMsg = "";
            StreamWriter nmm_StreamWriter = null; 
            Stream nmm_Stream = null;
            try
            {
                string LogDirPath = "";
                //if (HttpContext.Current == null)
                //{
                if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows应用程序则相等   
                {
                    LogDirPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + (LogFilePathName == "" ? "SQLDALHelper" : LogFilePathName) + "\\";
                }
                else
                {
                    LogDirPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + (LogFilePathName == "" ? "SQLDALHelper" : LogFilePathName) + "\\";
                }
                //}
                //else
                //    LogDirPath = HttpContext.Current.Server.MapPath("/Log");
                string reg = @"\:" + @"|\;" + @"|\/" + @"|\\" + @"|\|" + @"|\," + @"|\*" + @"|\?" + @"|\""" + @"|\<" + @"|\>";//特殊字符
                Regex r = new Regex(reg);
                string txtName = string.IsNullOrEmpty(FileName) ? DateTime.Now.ToString("yyyy-MM-dd") : FileName.Replace("\\", "-");
                txtName = r.Replace(txtName, "_");//将特殊字符替换为""
                LogDirPath += DateTime.Now.ToString("yyyy-MM");
                if (DayFilePathName)
                {
                    LogDirPath += "\\" + DateTime.Now.ToString("dd") + "号";
                }
                if (HourFileName)
                {
                    LogDirPath += "\\" + DateTime.Now.ToString("HH") + "点";
                    txtName = DateTime.Now.ToString("yyyy-MM-dd HH");
                }

                if (!Directory.Exists(LogDirPath))
                    Directory.CreateDirectory(LogDirPath);
                string nmm_strFileName = LogDirPath + "\\" + txtName + ".txt";
                //file not exists                        
                if (File.Exists(nmm_strFileName))
                    nmm_Stream = new FileStream(nmm_strFileName, System.IO.FileMode.Append);
                //file not exists  
                else
                    nmm_Stream = new FileStream(nmm_strFileName, System.IO.FileMode.Create);
                nmm_StreamWriter = new StreamWriter(nmm_Stream, Encoding.Unicode);
                nmm_strMessage = DateTime.Now.ToString() + ":  " + nmm_strMessage;
                //Write log
                nmm_StreamWriter.WriteLine(nmm_strMessage);
                nmm_StreamWriter.Flush();
            }
            catch(Exception ex)
            {
                retErrMsg = GetExceptionMsg(ex);
            }
            finally
            {
                if (nmm_StreamWriter != null)
                {
                    nmm_StreamWriter.Close();
                }
                if (nmm_Stream != null)
                    nmm_Stream.Close();
            }
            //return retErrMsg;
        }

        /// <summary>
        /// WriteLog
        /// </summary>
        /// <param name="nmm_strMessage">内容</param>
        /// <param name="LogFilePathName">日志目录名</param>
        /// <param name="NowTime">时间</param>
        /// <param name="DayFilePathName">是否生成 日目录</param>
        /// <param name="HourFileName">是否生成 小时文件名</param>
        /// <param name="FileName">文件名称（HourFileName为true时，设置无效）</param>
        public static void NewWriteLog(string nmm_strMessage, string LogFilePathName = "SQLDALHelper",DateTime? _NowTime = null, bool DayFilePathName = false, bool HourFileName = false, string FileName = "")
        {
            string retErrMsg = "";
            try
            {
                DateTime NowTime = _NowTime == null ? DateTime.Now : (Convert.ToDateTime(_NowTime));

                string LogDirPath = "";
                //if (HttpContext.Current == null)
                //{
                if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows应用程序则相等   
                {
                    LogDirPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + (LogFilePathName == "" ? "SQLDALHelper" : LogFilePathName) + "\\";
                }
                else
                {
                    LogDirPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + (LogFilePathName == "" ? "SQLDALHelper" : LogFilePathName) + "\\";
                }
                //}
                //else
                //    LogDirPath = HttpContext.Current.Server.MapPath("/Log");
                string reg = @"\:" + @"|\;" + @"|\/" + @"|\\" + @"|\|" + @"|\," + @"|\*" + @"|\?" + @"|\""" + @"|\<" + @"|\>";//特殊字符
                Regex r = new Regex(reg);
                string txtName = string.IsNullOrEmpty(FileName) ? DateTime.Now.ToString("yyyy-MM-dd") : FileName.Replace("\\", "-");
                txtName = r.Replace(txtName, "_");//将特殊字符替换为""
                LogDirPath += NowTime.ToString("yyyy-MM");
                if (DayFilePathName)
                {
                    LogDirPath += "\\" + NowTime.ToString("dd") + "号";
                }
                if (HourFileName)
                {
                    LogDirPath += "\\" + NowTime.ToString("HH") + "点";
                    txtName = NowTime.ToString("yyyy-MM-dd HH");
                }

                if (!Directory.Exists(LogDirPath))
                    Directory.CreateDirectory(LogDirPath);
                string nmm_strFileName = LogDirPath + "\\" + txtName + ".txt";
                Stream nmm_Stream;
                //file not exists                        
                if (File.Exists(nmm_strFileName))
                    nmm_Stream = new FileStream(nmm_strFileName, System.IO.FileMode.Append);
                //file not exists  
                else
                    nmm_Stream = new FileStream(nmm_strFileName, System.IO.FileMode.Create);
                StreamWriter nmm_StreamWriter = new StreamWriter(nmm_Stream, Encoding.Unicode);
                nmm_strMessage = NowTime.ToString() + ":  " + nmm_strMessage;
                //Write log
                nmm_StreamWriter.WriteLine(nmm_strMessage);
                nmm_StreamWriter.Flush();
                nmm_Stream.Close();
            }
            catch (Exception ex)
            {
                retErrMsg = GetExceptionMsg(ex);
            }
            //return retErrMsg;
        }

        /// <summary>
        /// 获取 错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetExceptionMsg(Exception ex)
        {
            if (ex is System.Data.SqlClient.SqlException)
            {
                var e = ex as System.Data.SqlClient.SqlException;
                return ex.InnerException == null ? ex.Message : (ex.InnerException.InnerException == null ? ex.Message : ex.InnerException.InnerException.Message);
            }
            else if (ex is Oracle.ManagedDataAccess.Client.OracleException)
            {
                var e = ex as Oracle.ManagedDataAccess.Client.OracleException;
                return ex.InnerException == null ? ex.Message : (ex.InnerException.InnerException == null ? ex.Message : ex.InnerException.InnerException.Message);
            }
            else
            {
                string ErrMsg = ex.Message;
                Exception e_x = ex.InnerException;
                while (e_x != null)
                {
                    if (!string.IsNullOrEmpty(e_x.Message))
                    {
                        ErrMsg = e_x.Message;
                    }
                    e_x = e_x.InnerException;
                }
                return ErrMsg;
                //return ex.InnerException == null ? ex.Message : (ex.InnerException.InnerException == null ? ex.InnerException.Message : ex.InnerException.InnerException.Message);
            }
        }

        /// <summary>
        /// 添加服务错误日志
        /// </summary>
        /// <param name="subject">主题</param>
        /// <param name="key1">键1(Param_s不为空时，取 Param_s里的数据)</param>
        /// <param name="key2">键2</param>
        /// <param name="content">内容</param>
        /// <param name="Param_s">参数</param>
        /// <param name="messageType">Information,Message,Error,Alert,Warning</param>
        /// <param name="NotificationName">提醒类型</param>
        public static void WirteLog(string subject, string key1, string key2, string content, string methodname, OracleParameter[] Param_s, string messageType = "Error", string NotificationName = "TMService")
        {
            try
            {
                string InsertSQLStr = "";

                string Key1_Str = "";
                if (Param_s != null)
                {
                    if (Param_s.Any())
                    {
                        foreach (var item in Param_s)
                        {
                            if (string.IsNullOrEmpty(Key1_Str))
                                Key1_Str += item.ParameterName + ":" + item.Value.ToString();
                            else
                                Key1_Str += "," + item.ParameterName + ":" + item.Value.ToString();
                        }
                    }
                    else
                        Key1_Str = key1;
                }
                else
                    Key1_Str = key1;

                OracleParameter[] OrclParams = new OracleParameter[]{
                    new OracleParameter(":V_ID",OracleDbType.Int16),
                    new OracleParameter(":V_SUBJECT",OracleDbType.NVarchar2,100),
                    new OracleParameter(":V_KEY1",OracleDbType.NVarchar2,100),
                    new OracleParameter(":V_KEY2",OracleDbType.NVarchar2,100),
                    new OracleParameter(":V_CONTENT",OracleDbType.NVarchar2),
                    new OracleParameter(":V_TYPE",OracleDbType.NVarchar2,20),
                    new OracleParameter(":V_NEWDATE",OracleDbType.Date),
                    new OracleParameter(":V_ISSENDED",OracleDbType.Int16),
                    new OracleParameter(":V_SENDDATE",OracleDbType.Date),
                    new OracleParameter(":V_NOTIFICATIONName",OracleDbType.NVarchar2),
                    new OracleParameter(":V_CREATEDDATE",OracleDbType.Date),
                    new OracleParameter(":V_MODIFIEDDATE",OracleDbType.Date),
                    new OracleParameter(":V_CREATEDBY",OracleDbType.NVarchar2,20),
                    new OracleParameter(":V_MODIFIEDBY",OracleDbType.NVarchar2,20),
                    new OracleParameter(":V_OutID", OracleDbType.Int16, ParameterDirection.Output)
                };
                OrclParams.Where(x => x.ParameterName == ":V_ID").FirstOrDefault().Value = 0;
                OrclParams.Where(x => x.ParameterName == ":V_SUBJECT").FirstOrDefault().Value = subject;
                OrclParams.Where(x => x.ParameterName == ":V_KEY1").FirstOrDefault().Value = Key1_Str;
                OrclParams.Where(x => x.ParameterName == ":V_KEY2").FirstOrDefault().Value = key2;
                OrclParams.Where(x => x.ParameterName == ":V_CONTENT").FirstOrDefault().Value = content;
                OrclParams.Where(x => x.ParameterName == ":V_TYPE").FirstOrDefault().Value = messageType;
                OrclParams.Where(x => x.ParameterName == ":V_NEWDATE").FirstOrDefault().Value = DateTime.Now;
                OrclParams.Where(x => x.ParameterName == ":V_ISSENDED").FirstOrDefault().Value = 0;
                OrclParams.Where(x => x.ParameterName == ":V_SENDDATE").FirstOrDefault().Value = DateTime.Now;
                OrclParams.Where(x => x.ParameterName == ":V_NOTIFICATIONName").FirstOrDefault().Value = NotificationName;
                OrclParams.Where(x => x.ParameterName == ":V_CREATEDDATE").FirstOrDefault().Value = DateTime.Now;
                OrclParams.Where(x => x.ParameterName == ":V_MODIFIEDDATE").FirstOrDefault().Value = DBNull.Value;
                OrclParams.Where(x => x.ParameterName == ":V_CREATEDBY").FirstOrDefault().Value = methodname;
                OrclParams.Where(x => x.ParameterName == ":V_MODIFIEDBY").FirstOrDefault().Value = DBNull.Value;
                InsertSQLStr = @"begin INSERTMESSAGE(
                V_ID               => :V_ID                ,
                V_SUBJECT          => :V_SUBJECT           ,
                V_KEY1             => :V_KEY1              ,
                V_KEY2             => :V_KEY2              ,
                V_CONTENT          => :V_CONTENT           ,
                V_TYPE             => :V_TYPE              ,
                V_NEWDATE          => :V_NEWDATE           ,
                V_ISSENDED         => :V_ISSENDED          ,
                V_SENDDATE         => :V_SENDDATE          ,
                V_NOTIFICATIONName => :V_NOTIFICATIONName  ,
                V_CREATEDDATE      => :V_CREATEDDATE       ,
                V_MODIFIEDDATE     => :V_MODIFIEDDATE      ,
                V_CREATEDBY        => :V_CREATEDBY         ,
                V_MODIFIEDBY       => :V_MODIFIEDBY        ,
                V_OutID            => :V_OutID); 
                end;";

                SQLDALHelper.OracleHelper.ExecuteNonQuery(CommandType.Text, InsertSQLStr, OrclParams);

                object retObj = OrclParams.Where(x => x.ParameterName == ":V_OutID").FirstOrDefault().Value.ToString();
                int retNumber = 0;
                if (retObj != null)
                    retNumber = Convert.ToInt16(retObj);

                //string name = NotificationName;
                //if (notification != null)
                //{
                //    KSWECDS.Web.Models.Message message = new KSWECDS.Web.Models.Message();
                //    message.Content = content;
                //    message.Key1 = Key1_Str;
                //    message.Key2 = key2;
                //    message.CreatedDate = DateTime.Now;
                //    message.NewDate = DateTime.Now;
                //    message.NotificationId = notification.Id;
                //    message.Subject = subject;
                //    message.Type = MsgType.ToString();
                //    message.CreatedBy = methodname;
                //    message.CreatedDate = DateTime.Now;
                //    MessageRep.Insert(message);
                //}
            }
            catch (Exception)
            {

            }
        }

        #endregion
    }
}