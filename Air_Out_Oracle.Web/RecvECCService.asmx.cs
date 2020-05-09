using AirOut.Web.Extensions;
using AirOut.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;

namespace AirOut.Web
{
    /// <summary>
    /// RecvECCService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class RecvECCService : System.Web.Services.WebService
    {
         private string WebServiceAddUser = "RecvECCService";
        //private string WebServiceAddUserID = "RecvECC-WebService";
        private string WebServiceName = "RecvECC-WebService";

        private WebdbContext appContext = new WebdbContext();
        //Redis服务
        RedisHelp.RedisHelper ORedisHelp = null;
        //异步记录日志
        private bool AsyncWebServiceLog = false;
        //缓存提醒类型数据
        IEnumerable<Notification> QNotification;
        //缓存操作点数据
        IEnumerable<OperatePoint> QOperatePoint;
        // 接收ECC 提交/签收号 正则验证
        string ECCRecvRegx = "";

        /// <summary>
        /// 本地IP
        /// </summary>
        string HostAddress = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        public RecvECCService()
        {
            //获取Notification-Message类型
            GetQNotification();
            //获取操作点
            GetQOperatePoint();

            try
            {
                ORedisHelp = new RedisHelp.RedisHelper();
            }
            catch (Exception)
            {
                ORedisHelp = null;
                Common.WriteLog_Local("创建Redis失败，链接无效或出错", WebServiceName + "\\Redis", true, AsyncWebServiceLog, false, "", DateTime.Now);
            }

            #region 异步记录日志

            try
            {
                string ConfigName = "AsyncWebServiceLog";
                string AsyncWebServiceLogStr = System.Configuration.ConfigurationManager.AppSettings[ConfigName] ?? "";
                AsyncWebServiceLog = Common.ChangStrToBool(AsyncWebServiceLogStr);
            }
            catch (Exception)
            {
                Common.WriteLog_Local("读取AsyncWebServiceLog失败", WebServiceName + "\\Config", true, AsyncWebServiceLog, false, "", DateTime.Now);
            }

            #endregion

            try
            {
                ECCRecvRegx = (string)CacheHelper.Get_SetStringConfAppSettings(WebServiceName, "ECCRecvRegx");
                string hostname = System.Net.Dns.GetHostName();//得到本机名

                System.Net.IPHostEntry localhost = System.Net.Dns.GetHostEntry(hostname);//得到IPv4和IPv6的地址
                System.Net.IPAddress localaddr = localhost.AddressList.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).OrderBy(x => x).FirstOrDefault();
                HostAddress = localaddr == null ? "" : localaddr.ToString();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 获取Notification-Message类型
        /// </summary>
        private IEnumerable<Notification> GetQNotification()
        {
            IEnumerable<Notification> _QNotification = null;
            try
            {
                if (QNotification == null || !QNotification.Any())
                {
                    _QNotification = (IEnumerable<Notification>)CacheHelper.Get_SetCache(Common.CacheNameS.Notification);
                    if (_QNotification == null || !_QNotification.Any())
                    {
                        _QNotification = appContext.Notification.ToList();
                    }
                    QNotification = _QNotification;
                }
            }
            catch
            {
                _QNotification = appContext.Notification.ToList();
            }
            return _QNotification;
        }

        /// <summary>
        /// 获取Notification-Message类型
        /// </summary>
        private IEnumerable<OperatePoint> GetQOperatePoint()
        {
            IEnumerable<OperatePoint> _QOperatePoint = null;
            try
            {
                if (QOperatePoint == null || !QOperatePoint.Any())
                {
                    _QOperatePoint = (IEnumerable<OperatePoint>)CacheHelper.Get_SetCache(Common.CacheNameS.OperatePoint);
                    if (_QOperatePoint == null || !_QOperatePoint.Any())
                    {
                        _QOperatePoint = appContext.OperatePoint.ToList();
                    }
                    QOperatePoint = _QOperatePoint;
                }
            }
            catch
            {
                _QOperatePoint = appContext.OperatePoint.ToList();
            }
            return _QOperatePoint;
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// 接收ECC-发票号
        /// </summary>
        /// <param name="InvoiceNo">发票号</param>
        /// <param name="AirOutNo">空出提交/签收号</param>
        /// <param name="CancelStatus">是否取消</param>
        /// <param name="InvoiceDate">开票日期</param>
        /// <returns></returns>
        [WebMethod(Description="接收发票号")]
        public ReturnMsg RecInvoiceNo(string InvoiceNo, string AirOutNo, bool CancelStatus = false,DateTime? InvoiceDate=null)
        {
            var OReturnMsg = new ReturnMsg();
            string Key1 = "";
            string Key2 = "";
            try
            {
                if (!InvoiceDate.HasValue)
                    InvoiceDate = DateTime.Now;
                Common.WriteLog_Local(InvoiceNo + "-" + AirOutNo + "-" + CancelStatus.ToString() + "-" + InvoiceDate.Value.ToString("yyyy-MM-dd hh:mm:ss"), "RecvECCSrv", true, true);
                if (!string.IsNullOrWhiteSpace(AirOutNo))
                {
                    var MRegx = new Regex(ECCRecvRegx);
                    var match = MRegx.Match(AirOutNo);
                    if (!match.Success)
                    {
                        OReturnMsg.RetKey = "ALL";
                        OReturnMsg.Success = false;
                        OReturnMsg.ErrMsg = "空出提交/签收号，规则不正确(T181213001,Q181213001)";
                        return OReturnMsg;
                    }
                }

                var Tag = string.IsNullOrWhiteSpace(AirOutNo) ? "" : AirOutNo.Substring(0, 1);
                if (CancelStatus)
                {
                    #region 取消发票

                    if (string.IsNullOrWhiteSpace(AirOutNo))
                    {
                        OReturnMsg.RetKey = "ALL";
                        OReturnMsg.Success = false;
                        OReturnMsg.ErrMsg = "空出提交/签收号，不能为空";
                        return OReturnMsg;
                    }
                    else
                    {
                        if (Tag == "T")
                        {
                            var NewAirOutNo = AirOutNo.Replace("T", "TJAE");
                            var ArrAr = appContext.Bms_Bill_Ar.AsQueryable().Where(x => x.Sumbmit_No == NewAirOutNo).ToList();
                            if (!ArrAr.Any())
                            {
                                OReturnMsg.RetKey = "ALL";
                                OReturnMsg.Success = false;
                                OReturnMsg.ErrMsg = "提交号(" + AirOutNo + ")下，没有数据";
                                return OReturnMsg;
                            }
                            foreach(var item in ArrAr){
                                var entry = appContext.Entry(item);
                                item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                                entry.State = System.Data.Entity.EntityState.Unchanged;
                                item.Sumbmit_ECCNo = "";
                                entry.Property(x => x.Sumbmit_ECCNo).IsModified = true;
                            }
                        }
                        else if (Tag == "Q")
                        {
                            var NewAirOutNo = AirOutNo.Replace("Q", "QSAE");
                            var ArrAp = appContext.Bms_Bill_Ap.AsQueryable().Where(x => x.SignIn_No == NewAirOutNo).ToList();
                            if (!ArrAp.Any())
                            {
                                OReturnMsg.RetKey = "ALL";
                                OReturnMsg.Success = false;
                                OReturnMsg.ErrMsg = "签收号(" + AirOutNo + ")下，没有数据";
                                return OReturnMsg;
                            }
                            foreach (var item in ArrAp)
                            {
                                var entry = appContext.Entry(item);
                                item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                                entry.State = System.Data.Entity.EntityState.Unchanged;
                                item.SignIn_ECCNo = "";
                                entry.Property(x => x.SignIn_ECCNo).IsModified = true;
                            }
                        }
                        appContext.SaveChanges();

                        OReturnMsg.RetKey = "ALL";
                        OReturnMsg.Success = true;
                        OReturnMsg.ErrMsg = "";
                        return OReturnMsg;
                    }

                    #endregion
                }
                else
                {
                    #region 获取发票号

                    if (string.IsNullOrWhiteSpace("InvoiceNo") || string.IsNullOrWhiteSpace("AirOutNo"))
                    {
                        OReturnMsg.RetKey = "ALL";
                        OReturnMsg.Success = false;
                        OReturnMsg.ErrMsg = "发票号和空出提交/签收号，不能为空";
                        return OReturnMsg;
                    }
                    else
                    {
                        if (Tag == "T")
                        {
                            var NewAirOutNo = AirOutNo.Replace("T", "TJAE");
                            var ArrAr = appContext.Bms_Bill_Ar.AsQueryable().Where(x => x.Sumbmit_No == NewAirOutNo).ToList();
                            if (!ArrAr.Any())
                            {
                                OReturnMsg.RetKey = "ALL";
                                OReturnMsg.Success = false;
                                OReturnMsg.ErrMsg = "提交号(" + AirOutNo + ")下，没有数据，无法回填发票号";
                                return OReturnMsg;
                            }
                            foreach (var item in ArrAr)
                            {
                                var entry = appContext.Entry(item);
                                item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                                entry.State = System.Data.Entity.EntityState.Unchanged;

                                item.Invoice_Status = true;
                                entry.Property(x => x.Invoice_Status).IsModified = true;
                                item.Invoice_No = InvoiceNo;
                                entry.Property(x => x.Invoice_No).IsModified = true;
                                item.Invoice_Date = InvoiceDate;
                                entry.Property(x => x.Invoice_Date).IsModified = true;
                                item.Invoice_Name = WebServiceAddUser;
                                entry.Property(x => x.Invoice_Name).IsModified = true;
                                item.Invoice_Id = WebServiceAddUser;
                                entry.Property(x => x.Invoice_Id).IsModified = true;
                                //item.Invoice_Desc = "";
                                //entry.Property(x => x.Invoice_Desc).IsModified = true;
                            }
                        }
                        else if (Tag == "Q")
                        {
                            var NewAirOutNo = AirOutNo.Replace("Q", "QSAE");
                            var ArrAp = appContext.Bms_Bill_Ap.AsQueryable().Where(x => x.SignIn_No == NewAirOutNo).ToList();
                            if (!ArrAp.Any())
                            {
                                OReturnMsg.RetKey = "ALL";
                                OReturnMsg.Success = false;
                                OReturnMsg.ErrMsg = "签收号(" + AirOutNo + ")下，没有数据，无法回填发票号";
                                return OReturnMsg;
                            }
                            foreach (var item in ArrAp)
                            {
                                var entry = appContext.Entry(item);
                                item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                                entry.State = System.Data.Entity.EntityState.Unchanged;

                                item.Invoice_Status = true;
                                entry.Property(x => x.Invoice_Status).IsModified = true;
                                item.Invoice_No = InvoiceNo;
                                entry.Property(x => x.Invoice_No).IsModified = true;
                                item.Invoice_Date = InvoiceDate;
                                entry.Property(x => x.Invoice_Date).IsModified = true;
                                item.Invoice_Name = WebServiceAddUser;
                                entry.Property(x => x.Invoice_Name).IsModified = true;
                                item.Invoice_Id = WebServiceAddUser;
                                entry.Property(x => x.Invoice_Id).IsModified = true;
                                //item.Invoice_Desc = "";
                                //entry.Property(x => x.Invoice_Desc).IsModified = true;
                            }
                        }
                        appContext.SaveChanges();

                        OReturnMsg.RetKey = "ALL";
                        OReturnMsg.Success = true;
                        OReturnMsg.ErrMsg = "";
                        return OReturnMsg;
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = "数据处理错误：" + Common.GetExceptionMsg(ex);
                WirteLog("数据处理错误：", Key1, Key2, WebServiceName + "-" + ErrMsg);
                OReturnMsg.RetKey = "ALL";
                OReturnMsg.Success = false;
                OReturnMsg.ErrMsg = ErrMsg;
            }
            return OReturnMsg;
        }

        /// <summary>
        /// 返回信息
        /// </summary>
        public class ReturnMsg
        {
            public ReturnMsg()
            {
                Success = false;
                RetKey = "ALL";
            }

            public ReturnMsg(string _ErrMsg, bool _Success = false, string _RetKey = "ALL")
            {
                ErrMsg = _ErrMsg;
                Success = _Success;
                RetKey = _RetKey;
            }

            /// <summary>
            /// 返回对应主键
            /// </summary>
            public string RetKey { get; set; }

            /// <summary>
            /// 是否成功
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// 错误信息
            /// </summary>
            public string ErrMsg { get; set; }
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
        private void WirteLog(string subject, string key1, string key2, string content, MessageType MsgType = MessageType.Error)
        {
            Message message = new Message();
            try
            {
                //清除其他实体的操作
                WebdbContext appContext = new WebdbContext();
                Repository.Pattern.Ef6.UnitOfWork unitOfWork_ = new Repository.Pattern.Ef6.UnitOfWork(appContext);

                if (!string.IsNullOrEmpty(key1))
                {
                    if (key1.Length > 100)
                    {
                        key1 = key1.Substring(0, 100);
                        int KeyNum = key1.LastIndexOf(',');
                        if (KeyNum < key1.Length)
                        {
                            key1 = key1.Substring(0, KeyNum);
                        }
                        else if (KeyNum == key1.Length)
                        {
                            key1 = key1.Substring(0, key1.Length - 1);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(key2))
                {
                    if (key2.Length > 100)
                    {
                        key2 = key2.Substring(0, 100);
                        int KeyNum = key2.LastIndexOf(',');
                        if (KeyNum < key2.Length)
                        {
                            key2 = key2.Substring(0, KeyNum);
                        }
                        else if (KeyNum == key2.Length)
                        {
                            key2 = key2.Substring(0, key2.Length - 1);
                        }
                    }
                }

                message.Content = content;
                message.Key1 = key1;
                message.Key2 = key2;
                message.CreatedDate = DateTime.Now;
                message.NewDate = DateTime.Now;
                message.NotificationId = 0;
                message.Subject = subject + "-" + HostAddress;
                message.Type = MsgType.ToString();
                message.CreatedBy = WebServiceAddUser;
                message.CreatedDate = DateTime.Now;

                string name = NotificationTag.RecvECCService.ToString();
                var notification = QNotification.Where(x => x.Name == name).FirstOrDefault();
                if (notification == null || notification.Id <= 0)
                {
                    var NotificationRep = unitOfWork_.Repository<Notification>();
                    notification = NotificationRep.Queryable().Where(x => x.Name == name).FirstOrDefault();
                }

                if (notification != null)
                {
                    message.NotificationId = notification.Id;
                    if (!AsyncWebServiceLog)
                    {
                        var MessageRep = unitOfWork_.Repository<Message>();
                        MessageRep.Insert(message);
                        unitOfWork_.SaveChanges();
                    }
                    else
                    {
                        Common.AddMessageToRedis(message, Common.RedisLogMsgType.SQLMessage, Common.RedisKeyMessageLog);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = Common.GetExceptionMsg(ex) + " - " + Newtonsoft.Json.JsonConvert.SerializeObject(message);
                Common.WriteLog_Local(ErrMsg, WebServiceName, true, AsyncWebServiceLog, false, "", DateTime.Now);
            }
        }

    }
}