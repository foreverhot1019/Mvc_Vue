using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCBWebApi.Models;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace CCBWebApi.Extensions
{
    public static class WriteLogHelper
    {
        /// <summary>
        /// 本地日志锁
        /// </summary>
        public static ReaderWriterLock ORWLogLocker = new ReaderWriterLock();

        #region Redis键值

        #region Redis队列键值

        //日志队列
        public static readonly string RedisKeyMessageLog = "WebSrvc_MessageLog";
        public static readonly string RedisKeyLocalLog = "WebSrvc_LocalLog";

        #endregion

        #endregion

        #region 日志

        #region 插入Redis日志

        /// <summary>
        /// 将日志数据写入Redis
        /// </summary>
        /// <param name="OMsg">错误内容</param>
        /// <param name="ORedisMsgType">错误类型（本地，Message类）</param>
        /// <param name="RedisKeyName">Redis得String类型Key值</param>
        /// <param name="FolderPath">本地日志文件地址</param>
        /// <param name="ORedisHelp">Redis帮助类</param>
        /// <returns></returns>
        public static bool AddMessageToRedis(object OMsg, EnumType.RedisLogMsgType ORedisLogMsgType, String RedisKeyName, string FolderPath = "Log", RedisHelp.RedisHelper ORedisHelp = null)
        {
            bool IsOK = false;

            try
            {
                MessageToRedis OMessageToRedis = new MessageToRedis(ORedisLogMsgType);
                OMessageToRedis.OMsg = OMsg;
                OMessageToRedis.FolderPath = FolderPath;
                if (ORedisHelp == null)
                    ORedisHelp = new RedisHelp.RedisHelper();
                //插入Redis队列
                ORedisHelp.ListRightPush<MessageToRedis>(RedisKeyName, OMessageToRedis);
                IsOK = true;
            }
            catch (Exception ex)
            {
                string ErrMsgStr = Common.GetExceptionMsg(ex);
                WriteLog_Local(ErrMsgStr, "Log\\AddMessageToRedis");
            }

            return IsOK;
        }

        #endregion

        #region 写入本地日志

        /// <summary>
        /// 写入本地日志（加锁）
        /// </summary>
        /// <param name="ErrMsgStr">错误信息</param>
        /// <param name="FolderPath">日志文件地址</param>
        /// <param name="DayFilePathName">日志文件地址按天文件夹记录</param>
        /// <param name="AddToRedis">日志存储到Redis</param>
        /// <param name="HourFileName">日志文件地址按小时文件夹记录</param>
        /// <param name="FileName">日志文件名称</param>
        /// <param name="AddDate">写入日志时间Redis</param>
        /// <returns></returns>
        public static bool WriteLog_Local(string ErrMsgStr, string FolderPath = "Log", bool DayFilePathName = true, bool AddToRedis = false, bool HourFileName = false, string FileName = "", DateTime? AddDate = null)
        {
            bool retTF = true;
            if (AddToRedis)
            {
                try
                {
                    retTF = AddMessageToRedis(ErrMsgStr, EnumType.RedisLogMsgType.LocalLog, RedisKeyLocalLog, FolderPath);
                }
                catch (Exception)
                {
                    retTF = false;
                }
            }
            else
            {
                try
                {
                    ORWLogLocker.AcquireWriterLock(1000);
                    try
                    {
                        if (AddDate == null)
                            SQLDALHelper.WriteLogHelper.WriteLog(ErrMsgStr, FolderPath, DayFilePathName, HourFileName, FileName);
                        else
                            SQLDALHelper.WriteLogHelper.NewWriteLog(ErrMsgStr, FolderPath, AddDate, DayFilePathName, HourFileName, FileName);
                    }
                    catch (Exception ex)
                    {
                        if (string.IsNullOrWhiteSpace(FileName))
                            FileName = Guid.NewGuid().ToString();
                        else
                            FileName = Guid.NewGuid().ToString() + FileName;

                        string ErrMsg = Common.GetExceptionMsg(ex);
                        if (AddDate == null)
                            SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg + "-" + ErrMsgStr, FolderPath, DayFilePathName, HourFileName, FileName);
                        else
                            SQLDALHelper.WriteLogHelper.NewWriteLog(ErrMsg + "-" + ErrMsgStr, FolderPath, AddDate, DayFilePathName, HourFileName, FileName);
                    }
                    finally
                    {
                        if (ORWLogLocker.IsWriterLockHeld)
                            ORWLogLocker.ReleaseWriterLock();
                    }
                }
                catch (Exception ex)
                {
                    if (ORWLogLocker.IsWriterLockHeld)
                        ORWLogLocker.ReleaseWriterLock();
                    retTF = false;
                    ErrMsgStr += Common.GetExceptionMsg(ex);

                    if (string.IsNullOrWhiteSpace(FileName))
                        FileName = Guid.NewGuid().ToString();
                    else
                        FileName = Guid.NewGuid().ToString() + FileName;
                    if (AddDate == null)
                        SQLDALHelper.WriteLogHelper.WriteLog(ErrMsgStr, FolderPath, DayFilePathName, HourFileName, FileName);
                    else
                        SQLDALHelper.WriteLogHelper.NewWriteLog(ErrMsgStr, FolderPath, AddDate, DayFilePathName, HourFileName, FileName);
                }
            }
            return retTF;
        }

        /// <summary>
        /// 写入本地日志（加锁）
        /// </summary>
        /// <param name="ErrMsgStr">错误信息</param>
        /// <param name="FolderPath">日志文件地址</param>
        /// <param name="DayFilePathName">日志文件地址按天文件夹记录</param>
        /// <param name="HourFileName">日志文件地址按小时文件夹记录</param>
        /// <param name="FileName">日志文件名称</param>
        /// <param name="AddDate">写入日志时间</param>
        /// <param name="_ORWLogLocker">读写锁</param>
        /// <returns></returns>
        public static bool WriteLog_LocalByRWLogLocker(string ErrMsgStr, string FolderPath = "Log", bool DayFilePathName = true,
            bool HourFileName = false, string FileName = "", DateTime? AddDate = null, ReaderWriterLock _ORWLogLocker = null)
        {
            bool retTF = true;
            ReaderWriterLock New_ORWLogLocker = ORWLogLocker;

            try
            {
                if (_ORWLogLocker != null)
                    New_ORWLogLocker = _ORWLogLocker;
                //开启写入锁
                New_ORWLogLocker.AcquireWriterLock(1000);
                try
                {
                    if (AddDate == null)
                        SQLDALHelper.WriteLogHelper.WriteLog(ErrMsgStr, FolderPath, DayFilePathName, HourFileName, FileName);
                    else
                        SQLDALHelper.WriteLogHelper.NewWriteLog(ErrMsgStr, FolderPath, AddDate, DayFilePathName, HourFileName, FileName);
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrWhiteSpace(FileName))
                        FileName = Guid.NewGuid().ToString();
                    else
                        FileName = Guid.NewGuid().ToString() + FileName;

                    string ErrMsg = Common.GetExceptionMsg(ex);
                    if (AddDate == null)
                        SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg + "-" + ErrMsgStr, FolderPath, DayFilePathName, HourFileName, FileName);
                    else
                        SQLDALHelper.WriteLogHelper.NewWriteLog(ErrMsg + "-" + ErrMsgStr, FolderPath, AddDate, DayFilePathName, HourFileName, FileName);
                }
                finally
                {
                    if (New_ORWLogLocker.IsWriterLockHeld)
                        New_ORWLogLocker.ReleaseWriterLock();
                }
            }
            catch (Exception ex)
            {
                if (New_ORWLogLocker.IsWriterLockHeld)
                    New_ORWLogLocker.ReleaseWriterLock();
                retTF = false;
                ErrMsgStr += Common.GetExceptionMsg(ex);

                if (string.IsNullOrWhiteSpace(FileName))
                    FileName = Guid.NewGuid().ToString();
                else
                    FileName = Guid.NewGuid().ToString() + FileName;
                if (AddDate == null)
                    SQLDALHelper.WriteLogHelper.WriteLog(ErrMsgStr, FolderPath, DayFilePathName, HourFileName, FileName);
                else
                    SQLDALHelper.WriteLogHelper.NewWriteLog(ErrMsgStr, FolderPath, AddDate, DayFilePathName, HourFileName, FileName);
            }

            return retTF;
        }

        #endregion

        #region LoganNet

        /// <summary>
        /// Log4Net写日志
        /// level（级别）：标识这条日志信息的重要级别None>Fatal>ERROR>WARN>DEBUG>INFO>ALL，设定一个
        /// </summary>
        /// <param name="ErrMSg">错误信息</param>
        /// <param name="OLog4NetMsgType">错误类型</param>
        /// <param name="ex">错误堆栈</param>
        public static void WriteLogByLog4Net(string ErrMSg, EnumType.Log4NetMsgType OLog4NetMsgType = EnumType.Log4NetMsgType.Info, Exception ex = null)
        {
            ILog log = log4net.LogManager.GetLogger("WebApp");
            switch (OLog4NetMsgType)
            {
                case EnumType.Log4NetMsgType.Fatal:
                    if (ex != null)
                        log.Fatal(ErrMSg, ex);//严重错误
                    else
                        log.Fatal(ErrMSg);//严重错误
                    break;
                case EnumType.Log4NetMsgType.Error:
                    if (ex != null)
                        log.Error(ErrMSg, ex);//错误
                    else
                        log.Error(ErrMSg);//错误
                    break;
                case EnumType.Log4NetMsgType.Warn:
                    if (ex != null)
                        log.Warn(ErrMSg, ex);//记录警告信息
                    else
                        log.Warn(ErrMSg);//记录警告信息
                    break;
                case EnumType.Log4NetMsgType.Debug:
                    if (ex != null)
                        log.Debug(ErrMSg, ex);//记录调试信息
                    else
                        log.Debug(ErrMSg);//记录调试信息
                    break;
                case EnumType.Log4NetMsgType.Info:
                    if (ex != null)
                        log.Info(ErrMSg, ex); //记录一般信息
                    else
                        log.Info(ErrMSg); //记录一般信息
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="ex">错误堆栈</param>
        /// <param name="OLog4NetMsgType">错误类型</param>
        public static void WriteLogByLog4Net(Exception ex, EnumType.Log4NetMsgType OLog4NetMsgType = EnumType.Log4NetMsgType.Info)
        {
            ILog log = log4net.LogManager.GetLogger("WebApp");
            string ErrMSg = "";
            if (ex == null)
            {
                ErrMSg = Common.GetExceptionMsg(ex);
            }
            switch (OLog4NetMsgType)
            {
                case EnumType.Log4NetMsgType.Fatal:
                    if (ex != null)
                        log.Fatal(ErrMSg, ex);//严重错误
                    else
                        log.Fatal(ErrMSg);//严重错误
                    break;
                case EnumType.Log4NetMsgType.Error:
                    if (ex != null)
                        log.Error(ErrMSg, ex);//错误
                    else
                        log.Error(ErrMSg);//错误
                    break;
                case EnumType.Log4NetMsgType.Warn:
                    if (ex != null)
                        log.Warn(ErrMSg, ex);//记录警告信息
                    else
                        log.Warn(ErrMSg);//记录警告信息
                    break;
                case EnumType.Log4NetMsgType.Debug:
                    if (ex != null)
                        log.Debug(ErrMSg, ex);//记录调试信息
                    else
                        log.Debug(ErrMSg);//记录调试信息
                    break;
                case EnumType.Log4NetMsgType.Info:
                    if (ex != null)
                        log.Info(ErrMSg, ex); //记录一般信息
                    else
                        log.Info(ErrMSg); //记录一般信息
                    break;
                default:
                    break;
            }
        }

        #endregion

        #endregion

        #region Redis异步推送

        /// <summary>
        /// 添加 错误信息然后推送
        /// </summary>
        /// <param name="ErrMsg">错误信息</param>
        /// <param name="Receiver">错误信息</param>
        /// <param name="SunChannel">推送频道</param>
        /// <param name="ORedisHelp">redis 链接</param>
        /// <param name="delay">延迟推送时间（毫秒，默认 0：不延迟）</param>
        public static void AddRedisToSubscribe(string ErrMsg, string Receiver, string SunChannel, RedisHelp.RedisHelper ORedisHelp = null, string Sender = "TM_Auto", int delay = 0)
        {
            if (!string.IsNullOrWhiteSpace(ErrMsg))
            {
                if (ORedisHelp == null)
                    ORedisHelp = new RedisHelp.RedisHelper();
                AutoSubscribeMsg OMsg = new AutoSubscribeMsg();
                OMsg.MSG = ErrMsg;
                OMsg.reciver = Receiver;
                OMsg.sender = Sender;
                OMsg.delay = delay;
                OMsg.type = "";
                ORedisHelp.Publish(SunChannel, Newtonsoft.Json.JsonConvert.SerializeObject(OMsg));
            }
        }

        #endregion

        #region 执行windows程序

        /// <summary>
        /// 运行新的进程（CMD，WinRar命令行等）
        /// 不显示命令窗口
        /// </summary>
        /// <param name ="processName">进程名称(完整路径，环境变量里 有配置时，不要完整路径)</param>
        /// <param name="cmdPath">命令行跳转路径(cmd.exe时，可能需要)</param>
        /// <param name="cmdStr">执行命令行参数</param>
        /// <param name="WaitForExit">执行命令行超时时间（毫秒）</param>
        public static Tuple<bool, string> RunCmd(string processName, string cmdStr, string cmdPath = "", int WaitForExit = 300000)
        {
            bool result = false;
            try
            {
                //没有命令 不执行
                if (string.IsNullOrEmpty(processName) || string.IsNullOrEmpty(cmdStr))
                    return new Tuple<bool, string>(result, "没有进程名称和要执行的命令");
                //执行路径 不存在 不执行
                if (!string.IsNullOrEmpty(cmdPath))
                    if (!Directory.Exists(cmdPath))
                        return new Tuple<bool, string>(result, "命令行跳转路径不存在");
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = processName;       //执行进程名称
                    myPro.StartInfo.UseShellExecute = false;      //是否使用操作系统shell启动
                    myPro.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
                    myPro.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                    myPro.StartInfo.RedirectStandardError = true; //重定向标准错误输出
                    myPro.StartInfo.CreateNoWindow = true;        //不显示程序窗口

                    myPro.Start();

                    //不执行路径 跳转命令
                    if (!string.IsNullOrEmpty(cmdPath))
                    {
                        //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 
                        var CMDPath = "cd /d \"" + cmdPath + "\" ";
                        myPro.StandardInput.WriteLine(CMDPath);
                    }

                    //在前面的命令执行完成后，要加exit命令，否则后面调用ReadtoEnd()命令会假死。
                    var CMDStr = string.Format(@"{0} {1} ", cmdStr, "&exit");
                    myPro.StandardInput.WriteLine(CMDStr);

                    //p.StandardInput.WriteLine("exit");
                    //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
                    //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

                    myPro.StandardInput.AutoFlush = true;
                    var bo = myPro.WaitForExit(WaitForExit);//设定5分钟
                    if (!bo)
                    {
                        myPro.Kill();
                        return new Tuple<bool, string>(false, "执行命令 超时");
                    }
                    else
                    {
                        //获取cmd窗口的输出信息
                        string output = myPro.StandardOutput.ReadToEnd();
                        return new Tuple<bool, string>(true, output);
                    }
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return new Tuple<bool, string>(false, ErrMsg);
            }
        }

        /// <summary>
        /// 运行新的进程
        /// </summary>
        /// <param name="cmdPath">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        /// <param name="WaitForExit">执行命令行超时时间（毫秒）</param>
        public static bool RunCmd_(string cmdExePath, string cmdStr, int WaitForExit = 300000)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExePath, cmdStr);
                    //psi.FileName = processName;
                    psi.UseShellExecute = false;
                    psi.RedirectStandardInput = true;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;
                    psi.CreateNoWindow = true;

                    myPro.StartInfo = psi;
                    myPro.Start();
                    var po = myPro.WaitForExit(WaitForExit);
                    if (!po)
                        myPro.Kill();
                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }

        #endregion
    }
}