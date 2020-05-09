using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace AirOut.Web
{
    public class WriteLogHelper
    {
        //public static readonly string WriteToLogPath = System.Configuration.ConfigurationManager.AppSettings["WriteToLog"];

        #region Write Log information

        /// <summary>
        /// write log 
        /// </summary>
        /// <param name="nmm_strMessage">log message</param>
        public static void WriteLog(string nmm_strMessage)
        {
            try
            {
                string LogDirPath = "";
                if (HttpContext.Current == null)
                {
                    if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows应用程序则相等   
                    {
                        LogDirPath = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    else
                    {
                        LogDirPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\";
                    }
                }
                else
                    LogDirPath = HttpContext.Current.Server.MapPath("/Log");
                LogDirPath += "\\"+DateTime.Now.ToString("yyyy-MM");

                if (!Directory.Exists(LogDirPath))
                    Directory.CreateDirectory(LogDirPath);
                string nmm_strFileName = LogDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                Stream nmm_Stream;
                //file not exists                        
                if (File.Exists(nmm_strFileName))
                    nmm_Stream = new FileStream(nmm_strFileName, System.IO.FileMode.Append);
                //file not exists  
                else
                    nmm_Stream = new FileStream(nmm_strFileName, System.IO.FileMode.Create);
                StreamWriter nmm_StreamWriter = new StreamWriter(nmm_Stream, Encoding.Unicode);
                nmm_strMessage = DateTime.Now.ToString() + ":  " + nmm_strMessage;
                //Write log

                nmm_StreamWriter.WriteLine(nmm_strMessage);
                nmm_StreamWriter.Flush();
                nmm_Stream.Close();
            }
            catch
            {
                ;
            }
        }

        #endregion
    }
}