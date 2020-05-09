using AirOut.Web.Extensions;
using AirOut.Web.Models;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AirOut.Web.Controllers
{
    //[Authorize]
    [CustomerExceptionHandleFilter]
    public class HomeController : Controller
    {
        private readonly IUnitOfWorkAsync _unitOfWork;

        public HomeController(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            bool IsAdminAuth = false;
            if (Utility.CurrentAppUser == null)
                IsAdminAuth = false;
            else
            {
                if (Utility.CurrentAppUser.UserName.ToLower() == "admin")
                    IsAdminAuth = true;
            }
            if (!IsAdminAuth)
            {
                IsAdminAuth = Utility.CurrentUserRoles == null ? false : Utility.CurrentUserRoles.Any(x => x.Name.Contains("超级管理员"));
            }

            #region 测试

            Test();

            #endregion

            //var TbAp = SQLDALHelper.OracleHelper.GetDataSet("select * from BMS_BILL_APS ");

            return View();
        }
        
        public ActionResult TS_Index()
        {
            return View();
        }

        /// <summary>
        /// 测试
        /// </summary>
        public void Test()
        {
            #region LuneceNet

            //var QMessage = _unitOfWork.Repository<AirOut.Web.Models.Message>().Queryable().Where(x => x.NewDate.Month == DateTime.Now.Month).ToList();
            //var _Type = typeof(AirOut.Web.Models.Message);
            //var _entityPropertys = _Type.GetProperties();
            //var _TypeStr = _Type.ToString();

            //foreach (var item in QMessage)
            //{
            //    //if (!LuneceManager.IndexManager.OIndexManager.SearchFromIndexData("", 10, 0, _TypeStr, item.Id.ToString()).Any())
            //        LuneceManager.IndexManager.OIndexManager.LuneceInsert(item, _entityPropertys);
            //}
            //var LunceneReasult = LuneceManager.IndexManager.OIndexManager.SearchFromIndexData("00000022000000047305", 10, 1, _TypeStr);

            #endregion

            #region 测试输出模板

            //var OEMS_HEAD = _unitOfWork.Repository<EMS_HEAD>().Queryable().Where(x=>x.EMS_EXGS.Any()).Include(x=>x.EMS_EXGS).Include(x=>x.EMS_IMGS).OrderByDescending(x => x.ID).FirstOrDefault();
            //var retMsg = WordHelper.SetWordModel_BookMarkByT<EMS_HEAD>(OEMS_HEAD);

            #endregion

            #region 测试条形码

            //string BarCodeStr = "GJA3223640020170160004120";
            //string imgName = DateTime.Now.ToString("yyyymmddHHmmss");
            //BarCode.DrawingBarCode dbd = new BarCode.DrawingBarCode();
            //System.Drawing.Image img1 = dbd.DrawingBarCodeImage(BarCodeStr, 50);
            //img1.Save(Server.MapPath("/DownLoad/" + imgName + "_1.jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            //System.Drawing.Image img2 = dbd.GetCodeImage(BarCodeStr);
            //img2.Save(Server.MapPath("/DownLoad/" + imgName + "_2.jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            //dbd.Height = 60;
            //dbd.ViewFont = new System.Drawing.Font("宋体", 10);
            //System.Drawing.Image img3 = dbd.GetCodeImage(BarCodeStr, BarCode.DrawingBarCode.Code39Model.Code39Normal, true);
            //img3.Save(Server.MapPath("/DownLoad/" + imgName + "_3.gif"), System.Drawing.Imaging.ImageFormat.Gif);

            #endregion

            #region 测试OneNote图片识别中文

            //string StrMsg = "";
            //bool ret = OCRReadText.OCRReadTextByOneNote(Server.MapPath("/FileModel/PrintImage/12.jpg"), 1500, out StrMsg);
            //string StrMsg1 = "";
            //bool ret1 = OCRReadText.OCRReadTextByOneNote(Server.MapPath("/FileModel/PrintImage/13.jpg"), 1500, out StrMsg);

            #endregion

            #region 动态调用关务云WebService

            //WebServiceHelper.CustomCloudServive.RetValidData mmevent1 = new WebServiceHelper.CustomCloudServive.RetValidData(ValidErrorEvent);
            //WebServiceHelper.CustomCloudServive ss1 = new WebServiceHelper.CustomCloudServive(mmevent1);
            //ss1.SendCustomData(81);

            #endregion

            #region 调用Infor ASN

            //string ReceiptKey = "1";//Infor ASN单号
            //string StoreKey = "2";//Infor 货主
            //string SUsr9 = "3";//Infor ASN作业单号
            //string OrderKey = "4";//Infor 出货单号
            //string SUsr2 = "5";//Infor SO作业单号
            //string InforWhcode = "FEILI_wmwhse1";//Infor 仓库指定
            //string RetXMLString = "";//返回XML 数据
            ////ASN
            //if (string.IsNullOrEmpty(ReceiptKey) || string.IsNullOrEmpty(StoreKey) || string.IsNullOrEmpty(SUsr9) || string.IsNullOrEmpty(InforWhcode))
            //{
            //    RetXMLString = "ASN单号，货主，ASN报关号，仓库 都不允许 为空";
            //}
            //else
            //{
            //    RetXMLString = RFDeviceAPP.Common.ASN.AdvancedShipNotice.UpdateASNByECDS(ReceiptKey, StoreKey, SUsr9, InforWhcode);
            //    //System.Threading.Thread.Sleep(10);循环的话 加个Sleep比较好
            //    if (!string.IsNullOrEmpty(RetXMLString))
            //    {
            //        //
            //    }
            //}
            ////SO
            //if (string.IsNullOrEmpty(OrderKey) || string.IsNullOrEmpty(StoreKey) || string.IsNullOrEmpty(SUsr2) || string.IsNullOrEmpty(InforWhcode))
            //{
            //    RetXMLString = "出货单号，货主，SO报关号，仓库 都不允许 为空";
            //}
            //else
            //{
            //    RetXMLString = RFDeviceAPP.Common.Orders.ShipmentOrder.UpdateSOByECDS(OrderKey, StoreKey, SUsr2, InforWhcode);
            //    //System.Threading.Thread.Sleep(10);循环的话 加个Sleep比较好
            //    if (!string.IsNullOrEmpty(RetXMLString))
            //    {
            //        //
            //    }
            //}

            #endregion

            #region  发送邮件

            //var ArrSendToMailErr = new List<string> { "Server Error", "操作超时" };
            //var MethodObj = new List<object> { new { ss = "123", tt = "345" } };
            //string MethodretobjStr = "Server Error";
            //string methodname = "SI_ECDS_ECDS1003";
            //var WhereArrSendToMailErr = ArrSendToMailErr.Where(x => MethodretobjStr.Contains(x));
            //if (WhereArrSendToMailErr.Any())
            //{
            //    new System.Threading.Thread(o =>
            //    {
            //        try
            //        {
            //            AirOut.Web.Models.WebdbContext NewAppContxt = new AirOut.Web.Models.WebdbContext();
            //            List<string> ArrRecMail = new List<string>();
            //            List<string> ArrCCMail = new List<string>();
            //            var ArrMailRecver = NewAppContxt.MailReceiver.Where(x => WhereArrSendToMailErr.Contains(x.ErrType) && x.ErrMethod.ToUpper() == methodname.ToUpper()).ToList();
            //            if (ArrMailRecver.Any())
            //            {
            //                ArrRecMail = ArrMailRecver.Select(x => x.RecMailAddress).ToList();
            //                string CCAddressStr = string.Join(",", ArrMailRecver.Select(x => x.CCMailAddress));
            //                if (string.IsNullOrWhiteSpace(CCAddressStr))
            //                {
            //                    ArrCCMail = CCAddressStr.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            //                }
            //                string Subject = "TM-Server Error";
            //                string Body = Subject + " 发送内容：" + Newtonsoft.Json.JsonConvert.SerializeObject(MethodObj);
            //                MailSendHelper.MailSend OMailSend = new MailSendHelper.MailSend("Michael1019_wang@feiliks.com", "Michael1019", ArrRecMail, Subject, Body, false, ArrCCMail);
            //                OMailSend.OSMTPMailSetting = new MailSendHelper.SMTPMailSetting();
            //                string ErrMsg = OMailSend.SendMail();
            //            }

            //        }
            //        catch (Exception ex)
            //        {
            //            string ErrMsgStr = AirOut.Web.Extensions.Common.GetExceptionMsg(ex);
            //            SQLDALHelper.WriteLogHelper.WriteLog("发送邮件错误：" + ErrMsgStr + Newtonsoft.Json.JsonConvert.SerializeObject(MethodObj), "Mail", true);
            //        }
            //    }) { IsBackground = true }.Start();
            //}

            #endregion

            #region Newtonsoft.Json 去除 序列化Json无限循环回路

            //Newtonsoft.Json.JsonSerializerSettings OJsonSrlizerSettingsNoLoop = new Newtonsoft.Json.JsonSerializerSettings();
            ////循环引用时 忽略
            //OJsonSrlizerSettingsNoLoop.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //////定义序列化Json的行为，使其忽略引用对象（导航属性）
            ////OJsonSrlizerSettingsNoLoop.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;

            //var OEMSHEAD = _unitOfWork.Repository<EMS_HEAD>().Queryable().FirstOrDefault();
            //OEMSHEAD.EMS_EXGS = _unitOfWork.Repository<EMS_EXG>().Queryable().Where(x => x.EMS_HEADId == OEMSHEAD.ID).Take(10).ToList();

            //var JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(OEMSHEAD, OJsonSrlizerSettingsNoLoop);

            #endregion
        }

        /// <summary>
        /// 控制拍照
        /// </summary>
        /// <param name="AccessTocken">应用权限Token</param>
        /// <param name="deviceSerial">设备序列号</param>
        /// <param name="channelNo">设备通道</param>
        /// <returns></returns>
        public ActionResult TakePhotoByEZ7(string _AccessTocken, string _deviceSerial, int? _channelNo = 1)
        {
            try
            {
                string DeviceSerial = _deviceSerial ?? "738231995";//设备序列号
                //萤石帐户：foreverhot1019，密码：ww19871019
                //string Password = "MGMNRY";//设备验证码
                //string AppKey = "093d2928e9944746b650a38a6f52c69e";//应用Key
                //string Secret ="844847e73685bd6f8eabae054da65313";//应用密钥
                string AccessToken = _AccessTocken ?? "at.5izo8eif3zsie5zl7nnmuubd9w260chy-8oh9rvci1t-01olw6z-n9oltbldj";//应用权限Token
                int? ChannelNo = _channelNo ?? 1;
                string Host = "https://open.ys7.com";
                string PostPath = "/api/lapp/device/capture";
                var Encode = System.Text.Encoding.UTF8;
                var BodyStr = "accessToken=" + AccessToken + "&deviceSerial=" + DeviceSerial + "&channelNo=" + ChannelNo;
                //var ReqContentType = "application/x-www-form-urlencoded";//请求类型

                #region 发送需要间隔不然会被锁定进入黑名单

                RcdSendSession ORcdSendSession = new RcdSendSession(1,true);
                string SessionType = "EZ7TakePhoto-";
                string KeyId = DeviceSerial;
                string ErrMsg = ORcdSendSession.ValidSendSession(SessionType, KeyId);
                if (!string.IsNullOrWhiteSpace(ErrMsg))
                {
                    return Json(new { Success = false, ErrMsg = "获取照片必须间隔一定的时间-" + ErrMsg }, JsonRequestBehavior.AllowGet);
                }

                #endregion

                System.Net.HttpWebRequest httpWebReq = Common.CreateWebRequest(Host + PostPath, Encode, BodyStr, 10000, "POST");
                var HttpWebRes = httpWebReq.GetResponse();
                var ResContextType = HttpWebRes.ContentType;//返回类型
                var streamReader = HttpWebRes.GetResponseStream();

                #region 将基础流写入内存流

                var memoryStream = new MemoryStream();
                const int bufferLength = 1024;
                byte[] buffer = new byte[bufferLength];
                int actual = 0;
                while ((actual = streamReader.Read(buffer, 0, bufferLength)) > 0)
                {
                    memoryStream.Write(buffer, 0, actual);
                }
                memoryStream.Position = 0;
                streamReader.Close();

                #endregion

                string responseContent = Encode.GetString(memoryStream.GetBuffer());
                //关闭流和链接
                HttpWebRes.Close();
                httpWebReq.Abort();
                if (!string.IsNullOrEmpty(responseContent))
                {
                    dynamic resData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
                    if (resData != null)
                    {
                        var _Code = resData.code ?? "";
                        var msg = resData.msg ?? "";
                        string newMsg = msg.ToString();
                        string _CodeS = _Code.ToString();
                        int Code = 0;
                        if (!int.TryParse(_CodeS, out Code))
                        {
                            return null;
                        }
                        //返回状态
                        EZ_TakePicStatus OEZ_TakePicStatus = (EZ_TakePicStatus)Code;
                        //var type = msg.GetType();
                        //if(type != typeof(string)){
                        //    msg = msg.Value.ToString();
                        //}
                        if (Code == 200)
                        {
                            if (resData.data != null)
                            {
                                var _picUrl = resData.data.picUrl ?? "";
                                var picUrl = _picUrl.ToString();
                                if (!string.IsNullOrEmpty(picUrl))
                                {
                                    System.Net.HttpWebRequest PichttpWebReq = Common.CreateWebRequest(picUrl, System.Text.Encoding.UTF8);
                                    var PicResponse = PichttpWebReq.GetResponse();
                                    //获取返回的数据流
                                    System.IO.Stream Stream_Reader = PicResponse.GetResponseStream();

                                    #region 读取流-必须先读取到内存中

                                    MemoryStream ms = new MemoryStream();
                                    //读取缓冲
                                    byte[] Newbuffer = new byte[bufferLength];
                                    actual = 0;
                                    while ((actual = Stream_Reader.Read(buffer, 0, bufferLength)) > 0)
                                    {
                                        ms.Write(buffer, 0, actual);
                                    }
                                    ms.Position = 0;
                                    Stream_Reader.Close();

                                    #endregion

                                    //返回的数据格式
                                    var PicResContentType = PicResponse.ContentType;

                                    string FileFolder = Server.MapPath("/EZ_Pic/" + DateTime.Now.ToString("yyyy-MM") + "/" + DateTime.Now.Day);
                                    if (!System.IO.Directory.Exists(FileFolder))
                                        System.IO.Directory.CreateDirectory(FileFolder);
                                    string FullFileName = FileFolder + "/" + Path.GetFileName(picUrl);
                                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(ms);
                                    bmp.Save(FullFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    return File(ms.ToArray(), PicResContentType);

                                    //var ArrEnumContentType = Common.GetEnumToDic("ContentType", "AirOut.Web.Controllers",".");
                                    //var WhereArrEnumContentType = ArrEnumContentType.Where(x => x.DisplayName == PicResContentType.ToLower());
                                    //if (WhereArrEnumContentType.Any())
                                    //{
                                    //    var OEnumContentType = WhereArrEnumContentType.FirstOrDefault();
                                    //    if (OEnumContentType.Key == "jpg")
                                    //    {
                                    //        string FileFolder = Server.MapPath("/EZ_Pic/" + DateTime.Now.ToString("yyyy-MM") + "/" + DateTime.Now.Day);
                                    //        if (!System.IO.Directory.Exists(FileFolder))
                                    //            System.IO.Directory.CreateDirectory(FileFolder);
                                    //        string FullFileName = FileFolder + "/" + Path.GetFileName(picUrl);
                                    //        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(ms);
                                    //        bmp.Save(FullFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    //        return File(ms.ToArray(), PicResContentType);
                                    //    }
                                    //    else
                                    //        return null;
                                    //}
                                    //else
                                    //    return null;
                                }
                                else
                                    return null;
                            }
                            else
                                return null;
                        }
                        else
                            return Json(new { Success = false, ErrMsg = OEZ_TakePicStatus }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Common.WriteLogByLog4Net(ex, Common.Log4NetMsgType.Error);
                return null;
            }
        }

        [AllowAnonymous]
        public ActionResult NoQXErr(string returnUrl = "", string ViewMsg = "")
        {
            ViewBag.Message = ViewMsg;
            return View();
        }

        [AllowAnonymous]
        public ActionResult PageNotFound(string returnUrl = "", string ViewMsg = "")
        {
            ViewBag.Message = ViewMsg;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ServerError(string returnUrl = "", string ViewMsg = "")
        {
            ViewBag.Message = ViewMsg;
            return View();
        }

        public ActionResult PdfTest()
        {
            TestObj OTestObj = new TestObj();
            OTestObj.CusBusName = "测试";
            OTestObj.CusBusCode = "Test";
            var ret = WordHelper.SetWordModel_BookMarkByT<TestObj>(OTestObj, Server.MapPath("/FileModel/pdf.docx"), Aspose.Words.SaveFormat.Pdf);
            return File(ret.MsgStr, "application/pdf");//,"123.pdf"
        }

        /// <summary>
        /// 测试数据仓库
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult DataBaseTest()
        {
            DataBase_FL ODataBase_FL = new DataBase_FL();
            ODataBase_FL.PostOPS_EttInforToDataWareHouse();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

    }

    public class TestObj
    {
        public string CusBusName { get; set; }
        public string CusBusCode { get; set; }
    }

    /// <summary>
    /// 常见的Content-Type类型
    /// </summary>
    enum ContentType
    {
        //常见的媒体格式类型如下：
        [Display(Description = "html格式", Name = "text/html")]
        html,
        [Display(Description = "纯文本格式", Name = "text/plain")]
        text,
        [Display(Description = "xml格式", Name = "text/xml")]
        xml,
        [Display(Description = "gif图片格式", Name = "image/gif")]
        gif,
        [Display(Description = "jpg图片格式", Name = "image/jpeg")]
        jpg,
        [Display(Description = "png图片格式", Name = "image/png")]
        png,
        [Display(Description = "ico图片格式", Name = "image/x-icon")]
        ico,
        [Display(Description = "javascript格式", Name = "text/javascript")]
        js,
        [Display(Description = "mp3格式", Name = "audio/mp3")]
        mp3,
        [Display(Description = "mp4格式", Name = "video/mpeg4")]
        mp4,
        //以application开头的媒体格式类型：
        [Display(Description = "XHTML格式", Name = "application/xhtml+xml")]
        xhtml,
        [Display(Description = " XML数据格式", Name = "application/xml")]
        appxnl,
        [Display(Description = "Atom XML聚合格式", Name = "application/atom+xml")]
        atmxml,
        [Display(Description = " JSON数据格式", Name = "application/json")]
        json,
        [Display(Description = "pdf格式", Name = "application/pdf")]
        pdf,
        [Display(Description = " Word文档格式", Name = "application/msword")]
        msword,
        [Display(Description = " 二进制流数据（如常见的文件下载）", Name = "application/octet-stream")]
        stream,
        [Display(Description = " <form encType=””>中默认的encType，form表单数据被编码为key/value格式发送到服务器（表单默认的提交数据的格式）   另外一种常见的媒体格式是上传文件之时使用的", Name = "application/x-www-form-urlencoded")]
        form,
        [Display(Description = " 需要在表单中进行文件上传时，就需要使用该格式", Name = "multipart/form-data")]
        formstream
    }

    /// <summary>
    /// 萤石云拍照 返回状态
    /// </summary>
    enum EZ_TakePicStatus
    {
        [Display(Name = "未知错误", Description = "")]
        未知错误 = 0,
        [Display(Name = "请求成功", Description = "")]
        操作成功 = 200,
        [Display(Name = "参数为空或格式不正确", Description = "")]
        参数错误 = 10001,
        [Display(Name = "重新获取accessToken", Description = "")]
        accessToken异常或过期 = 10002,
        [Display(Name = "appKey被冻结", Description = "")]
        appKey异常 = 10005,
        [Display(Name = "设备不属于当前用户或者未分享给当前用户", Description = "")]
        无权限进行抓图 = 10051,
        [Display(Name = "设备不存在", Description = "")]
        设备不存在 = 20002,
        [Display(Name = "检查设备网络状况，稍后再试", Description = "")]
        网络异常 = 20006,
        [Display(Name = "检查设备是否在线", Description = "")]
        设备不在线 = 20007,
        [Display(Name = "操作过于频繁或者设备不支持萤石协议抓拍", Description = "")]
        设备响应超时 = 20008,
        [Display(Name = "设备序列号不合法", Description = "")]
        deviceSerial不合法 = 20014,
        [Display(Name = "检查设备是否包含该通道", Description = "")]
        该用户下该通道不存在 = 20032,
        [Display(Name = "接口调用异常", Description = "")]
        数据异常 = 49999,
        [Display(Name = "设备返回失败", Description = "")]
        设备抓图失败 = 60017,
        [Display(Name = "确认设备是否支持抓图", Description = "")]
        不支持该命令 = 60020
    }
}