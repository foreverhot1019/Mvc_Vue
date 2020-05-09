using AirOut.Web.Extensions;
using AirOut.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
//using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AirOut.Web
{
    /// <summary>
    /// CRMService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class CRMService : System.Web.Services.WebService
    {
        private string WebServiceAddUser = "CRMService";
        //private string WebServiceAddUserID = "CRM-WebService";
        private string WebServiceName = "CRM-WebService";

        private WebdbContext appContext = new WebdbContext();
        //Redis服务
        RedisHelp.RedisHelper ORedisHelp = null;
        //异步记录日志
        private bool AsyncWebServiceLog = false;
        //缓存提醒类型数据
        IEnumerable<Notification> QNotification;
        //缓存操作点数据
        IEnumerable<OperatePoint> QOperatePoint;

        /// <summary>
        /// 本地IP
        /// </summary>
        string HostAddress = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        public CRMService()
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
                string hostname = System.Net.Dns.GetHostName();//得到本机名
                //System.Net.IPHostEntry OIPHost = System.Net.Dns.GetHostByName(hostname);//方法已过期，只得到IPv4的地址
                //HostAddress = OIPHost == null ? "" : (OIPHost.AddressList.Any() ? OIPHost.AddressList.Select(x => x.ToString()).OrderBy(x => x).FirstOrDefault() : "");

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
                        //WebdbContext appContext = new WebdbContext();
                        _QNotification = appContext.Notification.ToList();
                    }
                    QNotification = _QNotification;
                }
            }
            catch
            {
                //WebdbContext appContext = new WebdbContext();
                _QNotification = appContext.Notification.ToList();
                //appContext.Dispose();

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
                        //WebdbContext appContext = new WebdbContext();
                        _QOperatePoint = appContext.OperatePoint.ToList();
                    }
                    QOperatePoint = _QOperatePoint;
                }
            }
            catch
            {
                //WebdbContext appContext = new WebdbContext();
                _QOperatePoint = appContext.OperatePoint.ToList();
                //appContext.Dispose();
            }
            return _QOperatePoint;
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "接收CRM-客户数据", MessageName = "ReceiveCusBusInfo")]
        public ReturnMsg ReceiveCusBusInfo(string ArrCusBusInfo_Json)
        {
            var OReturnMsg = new ReturnMsg();
            string Key1 = "";
            string Key2 = "";
            try
            {
                //从缓存去数据
                var ArrPARA_Customs = (List<PARA_Customs>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Customs);//关区代码
                var ArrPARA_Area = (List<PARA_Area>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Area);//区域编码
                var ArrTradeType = (List<TradeType>)CacheHelper.Get_SetCache(Common.CacheNameS.TradeType);//行业类型
                var ArrCoPoKind = (List<CoPoKind>)CacheHelper.Get_SetCache(Common.CacheNameS.CoPoKind);//企业性质
                var ArrPARA_Country = (List<PARA_Country>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Country);//国家
                var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);//枚举TaxPayerType
                var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);//币制

                List<NewCusBusInfo> ArrCusBusInfo = new List<NewCusBusInfo>();
                if (string.IsNullOrWhiteSpace(ArrCusBusInfo_Json))
                {
                    OReturnMsg.RetKey = "ALL";
                    OReturnMsg.Success = false;
                    OReturnMsg.ErrMsg = "数据不能为空";
                    return OReturnMsg;
                }
                else
                {
                    ArrCusBusInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NewCusBusInfo>>(ArrCusBusInfo_Json);
                    if (ArrCusBusInfo.Any())
                    {
                        var Q_CIQID = ArrCusBusInfo.Select(x => x.CIQID);
                        var Q_EnterpriseId = ArrCusBusInfo.Select(x => x.EnterpriseId);
                        Key1 = string.Join(",", Q_EnterpriseId);
                        if (Q_EnterpriseId.Any(x => string.IsNullOrWhiteSpace(x)))
                        {
                            OReturnMsg.RetKey = "ALL";
                            OReturnMsg.Success = false;
                            OReturnMsg.ErrMsg = "业务伙伴代码，不能为空";
                            return OReturnMsg;
                        }
                        if (Q_CIQID.Any(x => string.IsNullOrWhiteSpace(x)))
                        {
                            OReturnMsg.RetKey = "ALL";
                            OReturnMsg.Success = false;
                            OReturnMsg.ErrMsg = "海关编码，不能为空";
                            return OReturnMsg;
                        }
                        //记录验证错误
                        List<Tuple<int, CusBusInfo, List<ValidationResult>>> ArrNewCusBusInfoValid = new List<Tuple<int, CusBusInfo, List<ValidationResult>>>();
                        ValidationContext ValidContxt = new ValidationContext(ArrCusBusInfo);

                        Q_EnterpriseId = Q_EnterpriseId.Where(x => !string.IsNullOrWhiteSpace(x));
                        //获取缓存客商数据
                        var QCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                        var Q_EnterpriseId_Data = QCusBusInfo.Where(x => Q_EnterpriseId.Contains(x.EnterpriseId)).ToList();

                        var SetPropertitys = typeof(CusBusInfo).GetProperties();
                        var GetPropertitys = typeof(NewCusBusInfo).GetProperties();
                        var AddID = -ArrCusBusInfo.Count - 100;
                        var Num = 0;
                        var ArrSaveChanges = new CusBusInfoChangeViewModel();
                        var inserted = new List<CusBusInfo>();
                        var updated = new List<CusBusInfo>();
                        foreach (var item in ArrCusBusInfo)
                        {
                            Num++;
                            var OCusBusInfo = Q_EnterpriseId_Data.Where(x => x.EnterpriseId == item.EnterpriseId).FirstOrDefault();
                            OCusBusInfo = OCusBusInfo == null ? new CusBusInfo() : OCusBusInfo;

                            #region 转换代码

                            //关区
                            if (!string.IsNullOrEmpty(item.CustomsCode) && item.CustomsCode != "-")
                            {
                                var QPARA_Customs = ArrPARA_Customs.Where(x => x.Customs_Code == item.CustomsCode || x.Customs_Name == item.CustomsCode);
                                if (QPARA_Customs.Any())
                                {
                                    item.CustomsCode = QPARA_Customs.FirstOrDefault().Customs_Code;
                                }
                                else
                                {
                                    var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, new List<ValidationResult> { 
                                        new ValidationResult("关区不存在", new string[] { "CustomsCode" }) 
                                    });
                                    ArrNewCusBusInfoValid.Add(OTuple);
                                    continue;
                                }
                            }
                            //行业类型代码
                            if (!string.IsNullOrEmpty(item.TradeTypeCode) && item.TradeTypeCode != "-")
                            {
                                var QTradeType = ArrTradeType.Where(x => x.Code == item.TradeTypeCode || x.Name == item.TradeTypeCode);
                                if (QTradeType.Any())
                                {
                                    item.TradeTypeCode = QTradeType.FirstOrDefault().Code;
                                }
                                else
                                {
                                    var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, new List<ValidationResult> { 
                                        new ValidationResult("行业类型不存在", new string[] { "TradeTypeCode" }) 
                                    });
                                    ArrNewCusBusInfoValid.Add(OTuple);
                                    continue;
                                }
                            }
                            //国家代码
                            if (!string.IsNullOrEmpty(item.CountryCode) && item.CountryCode != "-")
                            {
                                var QArrPARA_Country = ArrPARA_Country.Where(x => x.COUNTRY_CO == item.CountryCode || x.COUNTRY_NA == item.CountryCode);
                                if (QArrPARA_Country.Any())
                                {
                                    item.CountryCode = QArrPARA_Country.FirstOrDefault().COUNTRY_CO;
                                }
                                else
                                {
                                    var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, new List<ValidationResult> { 
                                        new ValidationResult("国家代码不存在", new string[] { "CountryCode" }) 
                                    });
                                    ArrNewCusBusInfoValid.Add(OTuple);
                                    continue;
                                }
                                //区域代码
                                if (!string.IsNullOrEmpty(item.AreaCode) && item.AreaCode != "-")
                                {
                                    var QArrPARA_Area = ArrPARA_Area.Where(x => x.Country_CO == item.CountryCode && (x.AreaCode == item.AreaCode || x.AreaName == item.AreaCode));
                                    if (QArrPARA_Area.Any())
                                    {
                                        item.AreaCode = QArrPARA_Area.FirstOrDefault().AreaCode;
                                    }
                                    else
                                    {
                                        var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, new List<ValidationResult> { 
                                            new ValidationResult("区域代码不存在", new string[] { "AreaCode" }) 
                                        });
                                        ArrNewCusBusInfoValid.Add(OTuple);
                                        continue;
                                    }
                                }
                                //ECC区域代码
                                if (!string.IsNullOrEmpty(item.ECCAreaCode) && item.ECCAreaCode != "-")
                                {
                                    var QArrPARA_Area = ArrPARA_Area.Where(x => x.Country_CO == item.CountryCode && x.AreaCode == item.ECCAreaCode || x.AreaName == item.ECCAreaCode);
                                    if (QArrPARA_Area.Any())
                                    {
                                        item.ECCAreaCode = QArrPARA_Area.FirstOrDefault().AreaCode;
                                    }
                                    else
                                    {
                                        var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, new List<ValidationResult> { 
                                            new ValidationResult("ECC区域代码不存在", new string[] { "ECCAreaCode" }) 
                                        });
                                        ArrNewCusBusInfoValid.Add(OTuple);
                                        continue;
                                    }
                                }
                            }
                            //企业性质代码
                            if (!string.IsNullOrEmpty(item.CopoKindCode) && item.CopoKindCode != "-")
                            {
                                var QArrCoPoKind = ArrCoPoKind.Where(x => x.Code == item.CopoKindCode || x.Name == item.CopoKindCode);
                                if (QArrCoPoKind.Any())
                                {
                                    item.CopoKindCode = QArrCoPoKind.FirstOrDefault().Code;
                                }
                                else
                                {
                                    var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, new List<ValidationResult> { 
                                        new ValidationResult("企业性质不存在", new string[] { "CopoKindCode" }) 
                                    });
                                    ArrNewCusBusInfoValid.Add(OTuple);
                                    continue;
                                }
                            }
                            //纳税人性质
                            if (!string.IsNullOrEmpty(item.TaxPayerType) && item.TaxPayerType != "-")
                            {
                                var QArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "TaxPayerType" && (x.LISTCODE == item.TaxPayerType || x.LISTNAME == item.TaxPayerType));
                                if (QArrBD_DEFDOC_LIST.Any())
                                {
                                    item.TaxPayerType = QArrBD_DEFDOC_LIST.FirstOrDefault().LISTCODE;
                                }
                                else
                                {
                                    var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, new List<ValidationResult> { 
                                        new ValidationResult("纳税人性质不存在", new string[] { "TaxPayerType" }) 
                                    });
                                    ArrNewCusBusInfoValid.Add(OTuple);
                                    continue;
                                }
                            }
                            //开票国家代码
                            if (!string.IsNullOrEmpty(item.InvoiceCountryCode) && item.InvoiceCountryCode != "-")
                            {
                                var QArrPARA_Country = ArrPARA_Country.Where(x => x.COUNTRY_CO == item.InvoiceCountryCode || x.COUNTRY_NA == item.InvoiceCountryCode);
                                if (QArrPARA_Country.Any())
                                {
                                    item.InvoiceCountryCode = QArrPARA_Country.FirstOrDefault().COUNTRY_CO;
                                }
                                else
                                {
                                    var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, new List<ValidationResult> { 
                                        new ValidationResult("开票国家代码不存在", new string[] { "InvoiceCountryCode" }) 
                                    });
                                    ArrNewCusBusInfoValid.Add(OTuple);
                                    continue;
                                }
                            }
                            //开户币种
                            if (!string.IsNullOrEmpty(item.Currency) && item.Currency != "-")
                            {
                                var QArrPARA_CURR = ArrPARA_CURR.Where(x => x.CURR_CODE == item.Currency || x.CURR_Name == item.Currency);
                                if (QArrPARA_CURR.Any())
                                {
                                    item.Currency = QArrPARA_CURR.FirstOrDefault().CURR_CODE;
                                }
                                else
                                {
                                    var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, new List<ValidationResult> { 
                                        new ValidationResult("开户币种不存在", new string[] { "Currency" }) 
                                    });
                                    ArrNewCusBusInfoValid.Add(OTuple);
                                    continue;
                                }
                            }

                            #endregion

                            //反射 设置效率差
                            //Common.SetSamaProtity(OCusBusInfo, item, false, SetPropertitys, GetPropertitys, true);
                            SetCusBusInfo(OCusBusInfo, item);
                            var OEntry = appContext.Entry(OCusBusInfo);
                            if (OCusBusInfo == null || OCusBusInfo.Id <= 0)
                            {
                                OCusBusInfo.Id = AddID++;

                                OEntry.State = System.Data.Entity.EntityState.Added;
                                OEntry.Entity.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                OCusBusInfo.ADDID = WebServiceAddUser;
                                OCusBusInfo.ADDWHO = WebServiceAddUser;
                                OCusBusInfo.OperatingPoint = 1;
                                inserted.Add(OCusBusInfo); 
                            }
                            else
                            {
                                OEntry.State = System.Data.Entity.EntityState.Modified;
                                OEntry.Entity.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                                OCusBusInfo.EDITID = WebServiceAddUser;
                                OCusBusInfo.EDITWHO = WebServiceAddUser;
                                updated.Add(OCusBusInfo);
                            }

                            var ArrValidation = new List<ValidationResult>();
                            var isValid = Validator.TryValidateObject(ArrCusBusInfo[0], new ValidationContext(ArrCusBusInfo[0]), ArrValidation, true);
                            if (!isValid)
                            {
                                var OTuple = new Tuple<int, CusBusInfo, List<ValidationResult>>(Num, OCusBusInfo, ArrValidation);
                                ArrNewCusBusInfoValid.Add(OTuple);
                            }
                        }
                        if (!ArrNewCusBusInfoValid.Any())
                        {
                            appContext.SaveChanges();
                            try
                            {
                                ArrSaveChanges.inserted = inserted;
                                ArrSaveChanges.updated = updated;
                                CacheHelper.AutoResetCache(ArrSaveChanges);
                            }
                            catch (Exception e)
                            {
                                var ErrMsg = "更新缓存错误：" + Common.GetExceptionMsg(e);
                                WirteLog("更新缓存错误：", Key1, Key2, WebServiceName + "-" + ErrMsg);
                            }
                            OReturnMsg.RetKey = "ALL";
                            OReturnMsg.Success = true;
                            OReturnMsg.ErrMsg = "";
                        }
                        else
                        {
                            OReturnMsg.RetKey = "ALL";
                            OReturnMsg.Success = false;
                            OReturnMsg.ErrMsg = string.Join("<br />", ArrNewCusBusInfoValid.Select(x => "第" + x.Item1 + "条数据验证错误:" + x.Item2.EnterpriseId + "-" + x.Item2.CIQID + ":" + string.Join(",", x.Item3.Select(n => n.ErrorMessage))));
                        }
                    }
                    else
                    {
                        OReturnMsg.RetKey = "ALL";
                        OReturnMsg.Success = false;
                        OReturnMsg.ErrMsg = "数据不能为空";
                    }
                }
                if (!OReturnMsg.Success)
                    WirteLog("数据处理错误：", Key1, Key2, WebServiceName + "-" + OReturnMsg.ErrMsg);
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
        /// 设置 字段对应
        /// </summary>
        /// <param name="OCusBusInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private void SetCusBusInfo(CusBusInfo OCusBusInfo, NewCusBusInfo item)
        {
            //业务伙伴代码
            OCusBusInfo.EnterpriseId = item.EnterpriseId;
            //业务伙伴全称
            OCusBusInfo.EnterpriseName = item.EnterpriseName;
            //业务伙伴简称
            OCusBusInfo.EnterpriseShortName = item.EnterpriseShortName;
            //业务伙伴中文名称
            OCusBusInfo.CHNName = item.CHNName;
            //业务伙伴英文名称
            OCusBusInfo.EngName = item.EngName;
            //中文地址
            OCusBusInfo.AddressCHN = item.AddressCHN;
            //英文地址
            OCusBusInfo.AddressEng = item.AddressEng;
            //集团代码
            OCusBusInfo.EnterpriseGroupCode = item.EnterpriseGroupCode;
            //总公司编码
            OCusBusInfo.TopEnterpriseCode = item.TopEnterpriseCode;
            //海关编码
            OCusBusInfo.CIQID = item.CIQID;
            //关区代码
            OCusBusInfo.CustomsCode = item.CustomsCode;
            //网址
            OCusBusInfo.WebSite = item.WebSite;
            //行业类型代码
            //OCusBusInfo.TeadeTypeCode = item.TeadeTypeCode;
            //区域代码
            OCusBusInfo.AreaCode = item.AreaCode;
            //国家代码
            OCusBusInfo.CountryCode = item.CountryCode;
            //企业性质代码
            OCusBusInfo.CopoKindCode = item.CopoKindCode;
            //企业法人
            OCusBusInfo.CorpartiPerson = item.CorpartiPerson;
            //注册资金
            OCusBusInfo.ResteredCapital = item.ResteredCapital;
            //是否内部公司(1-是,0-否)
            OCusBusInfo.IsInternalCompany = item.IsInternalCompany;
            //集团
            OCusBusInfo.EnterpriseGroup = item.EnterpriseGroup;
            //客户分类
            OCusBusInfo.EnterpriseType = item.EnterpriseType;
            //ECC区域代码
            OCusBusInfo.ECCAreaCode = item.ECCAreaCode;
            //联系人
            OCusBusInfo.LinkerMan = item.LinkerMan;
            //联系电话", Description = "总机
            OCusBusInfo.Telephone = item.Telephone;
            //传真
            OCusBusInfo.Fax = item.Fax;
            //纳税人性质
            OCusBusInfo.TaxPayerType = item.TaxPayerType;
            //统一社会信用代码
            OCusBusInfo.UnifiedSocialCreditCode = item.UnifiedSocialCreditCode;
            //国家代码"
            OCusBusInfo.InvoiceCountryCode = item.InvoiceCountryCode;
            //开票地址"
            OCusBusInfo.InvoiceAddress = item.InvoiceAddress;
            //银行名称
            OCusBusInfo.BankName = item.BankName;
            //银行账号
            OCusBusInfo.BankAccount = item.BankAccount;
            //开户币种
            OCusBusInfo.Currency = item.Currency;
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
        /// CRM回传字段
        /// </summary>
        public class NewCusBusInfo
        {
            public NewCusBusInfo()
            {
            }

            [Display(Name = "业务伙伴代码")]
            [Required(ErrorMessage = "业务伙伴代码不能为空！")]
            [MaxLength(20)]
            public string EnterpriseId { get; set; }

            [Display(Name = "业务伙伴全称", Description = "中国企业和中文名称一致，外国企业 和 英文名称一致")]
            [Required, MaxLength(500)]
            public string EnterpriseName { get; set; }

            [Display(Name = "业务伙伴简称")]
            [Required, MaxLength(50)]
            public string EnterpriseShortName { get; set; }

            [Display(Name = "业务伙伴中文名称")]
            [MaxLength(100)]
            public string CHNName { get; set; }

            [Display(Name = "业务伙伴英文名称")]
            [MaxLength(100)]
            public string EngName { get; set; }

            [Display(Name = "中文地址")]
            [MaxLength(200)]
            public string AddressCHN { get; set; }

            [Display(Name = "英文地址")]
            [MaxLength(200)]
            public string AddressEng { get; set; }

            [Display(Name = "集团代码")]
            [MaxLength(50)]
            public string EnterpriseGroupCode { get; set; }

            [Display(Name = "总公司编码")]
            [MaxLength(50)]
            public string TopEnterpriseCode { get; set; }

            [Display(Name = "海关编码")]
            [Required]
            [MaxLength(50)]
            public string CIQID { get; set; }

            [Display(Name = "关区代码")]
            [MaxLength(50)]
            public string CustomsCode { get; set; }

            [Display(Name = "网址")]
            [MaxLength(200)]
            public string WebSite { get; set; }

            [Display(Name = "行业类型代码")]
            [MaxLength(50)]
            public string TradeTypeCode { get; set; }

            [Display(Name = "区域代码")]
            [Required, MaxLength(50)]
            public string AreaCode { get; set; }

            [Display(Name = "国家代码")]
            [Required, MaxLength(50)]
            public string CountryCode { get; set; }

            [Display(Name = "企业性质代码")]
            [MaxLength(50)]
            public string CopoKindCode { get; set; }

            [Display(Name = "企业法人")]
            [MaxLength(50)]
            public string CorpartiPerson { get; set; }

            [Display(Name = "注册资金")]
            [Required]
            [MaxLength(50)]
            public string ResteredCapital { get; set; }

            [Display(Name = "是否内部公司(1-是,0-否)")]
            public bool IsInternalCompany { get; set; }

            [Display(Name = "集团")]
            [MaxLength(50)]
            public string EnterpriseGroup { get; set; }

            [Display(Name = "客户分类")]
            [Required]
            [MaxLength(50)]
            public string EnterpriseType { get; set; }

            [Display(Name = "ECC区域代码")]
            [Required]
            [MaxLength(50)]
            public string ECCAreaCode { get; set; }

            [Display(Name = "联系人")]
            [MaxLength(50)]
            public string LinkerMan { get; set; }

            [Display(Name = "联系电话", Description = "总机")]
            [MaxLength(100)]
            public string Telephone { get; set; }

            [Display(Name = "传真")]
            [MaxLength(100)]
            public string Fax { get; set; }

            [Display(Name = "纳税人性质")]
            [Required, MaxLength(50)]
            public string TaxPayerType { get; set; }

            [Display(Name = "统一社会信用代码")]
            [Required, MaxLength(50)]
            public string UnifiedSocialCreditCode { get; set; }

            [Display(Name = "国家代码", Description = "开票国家代码")]
            [Required]
            [MaxLength(50)]
            public string InvoiceCountryCode { get; set; }

            [Display(Name = "开票地址", Description = "开票地址")]
            [Required]
            [MaxLength(100)]
            public string InvoiceAddress { get; set; }

            [Display(Name = "银行名称", Description = "")]
            [Required]
            [MaxLength(100)]
            public string BankName { get; set; }

            [Display(Name = "银行账号", Description = "")]
            [Required]
            [MaxLength(100)]
            public string BankAccount { get; set; }

            [Display(Name = "开户币种", Description = "")]
            [Required]
            [MaxLength(50)]
            public string Currency { get; set; }

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
                message.Subject = WebServiceName + "-" + subject + "-" + HostAddress;
                message.Type = MsgType.ToString();
                message.CreatedBy = WebServiceAddUser;
                message.CreatedDate = DateTime.Now;

                string name = NotificationTag.CRMService.ToString();
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