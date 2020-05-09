using TMI.Web.Extensions;
using TMI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace TMI.Web
{
    public class AsyncWriteLogByRedis
    {
        public class AsyncWriteLog
        {
            RedisHelp.RedisHelper ORedisHelp = null;
            private readonly string RedisKeyLocalLog = Common.RedisKeyLocalLog;
            private readonly string RedisKeyMessageLog = Common.RedisKeyMessageLog;

            string LogPath = "AsyncWriteLogByRedis\\";

            public static readonly AsyncWriteLog OAsyncWriteLog = new AsyncWriteLog();

            /// <summary>
            /// 线程休息时间 毫秒
            /// </summary>
            private int ThreadSleepTime = 1000;

            /// <summary>
            /// 记录线程
            /// </summary>
            public List<ThreadObj> ArrTimer = new List<ThreadObj>();

            //启用异步写日志（数据暂存在Redis）
            public bool AsyncWebServiceLog = false;

            //最大线程数
            public int MaxThread { get; set; }
            //每个线程 最大处理数据数
            public int MaxTake { get; set; }
            //去除Json无限循环回路
            Newtonsoft.Json.JsonSerializerSettings OJsonSrlizerSettingsNoLoop = new Newtonsoft.Json.JsonSerializerSettings();

            public AsyncWriteLog()
            {
                //获取Config配置信息
                ReLoadConfigSettings();

                #region 去除 序列化Json无限循环回路

                //循环引用时 忽略
                OJsonSrlizerSettingsNoLoop.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                ////定义序列化Json的行为，使其忽略引用对象（导航属性,主键 会被转义-$主键）
                //OJsonSrlizerSettingsNoLoop.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;

                #endregion

                try
                {
                    ORedisHelp = new RedisHelp.RedisHelper();
                }
                catch (Exception)
                {
                    ORedisHelp = null;
                    WriteLogByRWLocker("创建Redis失败，链接无效或出错", LogPath + "Redis");
                }
            }

            /// <summary>
            /// 启动线程
            /// </summary>
            public void StartNewThread()
            {
                if (AsyncWebServiceLog)
                {
                    #region 本地日志 线程

                    System.Threading.Thread LocalLogThread = new Thread(timerCallLocalLog);
                    LocalLogThread.Start();
                    ArrTimer.Add(new ThreadObj("LocalLog", LocalLogThread));

                    #endregion

                    #region 数据库Message 线程

                    System.Threading.Thread MessageThread = new Thread(timerCallMessage);
                    MessageThread.Start();
                    ArrTimer.Add(new ThreadObj("Message", MessageThread));

                    #endregion
                }
            }

            #region 本地日志

            /// <summary>
            /// 循环线程 获取 要写入本地的日志数据
            /// </summary>
            public void timerCallLocalLog()
            {
                while (true)
                {
                    SQLDALHelper.WriteLogHelper.WriteLog("测试是否死掉", LogPath + "Timer\\LocalLog", true, true);
                    lock (Common.TimerLocalLogLocker)
                    {
                        SQLDALHelper.WriteLogHelper.WriteLog("测试Lock是否死掉", LogPath + "Timer\\LocalLog", true, true);
                        try
                        {
                            if (ORedisHelp != null)
                            {
                                #region 注释

                                int i = 0;
                                while (i < MaxThread)
                                {
                                    i++;
                                    MessageToRedis OMessageToRedis = ORedisHelp.ListLeftPop<MessageToRedis>(RedisKeyLocalLog);
                                    if (OMessageToRedis != null)
                                    {
                                        Thread LocalLogThread = new Thread(doLocalLog);
                                        LocalLogThread.Start(OMessageToRedis);
                                    }
                                    else
                                        break;
                                }

                                #endregion

                                #region 获取Redis-MaxThread条数

                                //List<MessageToRedis> ArrMessageToRedis = ORedisHelp.ListRange<MessageToRedis>(RedisKeyLocalLog, MaxThread);
                                //foreach (var OMessageToRedis in ArrMessageToRedis)
                                //{
                                //    if (OMessageToRedis != null)
                                //    {
                                //        Thread LocalLogThread = new Thread(doLocalLog);
                                //        //LocalLogThread.IsBackground = true;
                                //        LocalLogThread.Start(OMessageToRedis);
                                //    }
                                //    else
                                //        continue;
                                //}
                                //ORedisHelp.ListTrim(RedisKeyLocalLog, MaxThread);

                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                string ErrMsgStr = Common.GetExceptionMsg(ex);
                                WriteLogByRWLocker(ErrMsgStr, LogPath + "LocalLog");
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    Thread.Sleep(ThreadSleepTime);
                }
            }

            /// <summary>
            /// 写入本地日志数据
            /// </summary>
            /// <param name="ObjArg"></param>
            public void doLocalLog(object ObjArg)
            {
                try
                {
                    if (ObjArg != null)
                    {
                        MessageToRedis OMessageToRedis = (MessageToRedis)ObjArg;
                        if (OMessageToRedis != null)
                        {
                            if (OMessageToRedis.OMsg != null)
                            {
                                var MsgType = OMessageToRedis.OMsg.GetType();
                                if (MsgType == typeof(String))
                                    Common.WriteLog_Local(OMessageToRedis.OMsg.ToString(), OMessageToRedis.FolderPath, true, false, false, "", OMessageToRedis.NowDate);
                                else
                                    WriteLogByRWLocker("数据转换失败", LogPath + "LocalLog");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        var ErrMsg = Common.GetExceptionMsg(ex);
                        string ErrMsgStr = ErrMsg + (ObjArg == null ? "" : ("-" + Newtonsoft.Json.JsonConvert.SerializeObject(ObjArg)));
                        WriteLogByRWLocker(ErrMsgStr, LogPath + "LocalLog");
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            #endregion

            #region SQL日志

            /// <summary>
            /// 循环线程 获取 要写入数据库Message的数据
            /// </summary>
            public void timerCallMessage()
            {
                while (true)
                {
                    SQLDALHelper.WriteLogHelper.WriteLog("测试是否死掉", LogPath + "Timer\\MessageLog", true, true);
                    lock (Common.TimerMessageLgLocker)
                    {
                        SQLDALHelper.WriteLogHelper.WriteLog("测试Lock是否死掉", LogPath + "Timer\\MessageLog", true, true);
                        try
                        {
                            if (ORedisHelp != null)
                            {
                                int i = 0;
                                while (i < MaxThread)
                                {
                                    i++;
                                    MessageToRedis OMessageToRedis = ORedisHelp.ListLeftPop<MessageToRedis>(RedisKeyMessageLog);
                                    if (OMessageToRedis != null)
                                    {
                                        Thread MessageLogThread = new Thread(doMessageLog);
                                        MessageLogThread.Start(OMessageToRedis);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            WriteLogByRWLocker("数据转换失败", LogPath + "MessageLog");
                                        }
                                        catch (Exception)
                                        {

                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                WriteLogByRWLocker("数据转换失败", LogPath + "Redis");
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                string ErrMsgStr = Common.GetExceptionMsg(ex);
                                WriteLogByRWLocker(ErrMsgStr, LogPath + "MessageLog");
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    Thread.Sleep(ThreadSleepTime);
                }
            }

            /// <summary>
            /// 写入数据库Message的数据
            /// </summary>
            /// <param name="ObjArg"></param>
            public void doMessageLog(object ObjArg)
            {
                try
                {
                    if (ObjArg != null)
                    {
                        MessageToRedis OMessageToRedis = (MessageToRedis)ObjArg;
                        if (OMessageToRedis != null)
                        {
                            if (OMessageToRedis.OMsg != null)
                            {
                                WebdbContext dbContxt = new WebdbContext();

                                Message OMessage = null;
                                var MsgType = OMessageToRedis.OMsg.GetType();
                                try
                                {
                                    OMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(OMessageToRedis.OMsg.ToString());
                                }
                                catch (Exception)
                                {
                                    if (MsgType == typeof(Message))
                                        OMessage = (Message)OMessageToRedis.OMsg;
                                    else
                                        OMessage = null;
                                }
                                if (OMessage != null)
                                {
                                    OMessage.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                    //dbContxt.Entry<Message>(OMessage);
                                    dbContxt.Set<Message>().Add(OMessage);
                                }
                                else
                                    WriteLogByRWLocker("数据转换失败", LogPath + "MessageLog");
                                dbContxt.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        var ErrMsg = Common.GetExceptionMsg(ex);
                        string ErrMsgStr = ErrMsg + (ObjArg == null ? "" : ("-" + Newtonsoft.Json.JsonConvert.SerializeObject(ObjArg)));
                        WriteLogByRWLocker(ErrMsgStr, LogPath + "MessageLog");
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            #endregion

            /// <summary>
            /// 获取Config配置信息
            /// </summary>
            public void ReLoadConfigSettings()
            {
                #region Timer线程 执行间隔时间

                string RedisTM_InforSleepStr = System.Configuration.ConfigurationManager.AppSettings["GetRedisMsgLogSleep"] ?? "3000";
                if (!int.TryParse(RedisTM_InforSleepStr, out ThreadSleepTime))
                    ThreadSleepTime = 1000;

                #endregion

                #region 多线程 最大线程数和每个线程最大处理数

                //最大线程数
                int Max_Thread = 0;
                string _MaxThread = System.Configuration.ConfigurationManager.AppSettings["AsyncMsgLogMaxThread"] ?? "10";
                if (int.TryParse(_MaxThread, out Max_Thread))
                    MaxThread = Max_Thread;
                else
                    MaxThread = 10;
                //每个线程 最大处理数据数
                int Max_Take = 0;
                string _MaxTake = System.Configuration.ConfigurationManager.AppSettings["AsyncMsgLogMaxTake"] ?? "10";
                if (int.TryParse(_MaxTake, out Max_Take))
                    MaxTake = Max_Take;
                else
                    MaxTake = 10;

                #endregion

                #region 异步写日志

                string AsyncWebServiceLogStr = System.Configuration.ConfigurationManager.AppSettings["AsyncWebServiceLog"] ?? "";
                AsyncWebServiceLog = Common.ChangStrToBool(AsyncWebServiceLogStr);

                #endregion
            }

            /// <summary>
            /// 插入日志
            /// </summary>
            /// <param name="ErrMsgStr"></param>
            /// <param name="FolderPath"></param>
            public void WriteLogByRWLocker(string ErrMsgStr, string FolderPath, bool DayFilePathName = true, bool HourFileName = false, string FileName = "")
            {
                Common.WriteLog_Local(ErrMsgStr, FolderPath, DayFilePathName, false, HourFileName, FileName);
            }
        }
    }
}