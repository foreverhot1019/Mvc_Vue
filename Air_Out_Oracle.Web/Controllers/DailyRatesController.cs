using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using AirOut.Web.Models;
using AirOut.Web.Services;
using AirOut.Web.Repositories;
using AirOut.Web.Extensions;
using WIA;

namespace AirOut.Web.Controllers
{
    [AllowAnonymous]
    public class DailyRatesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<DailyRate>, Repository<DailyRate>>();
        //container.RegisterType<IDailyRateService, DailyRateService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IDailyRateService _dailyRateService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/DailyRates";

        //Redis服务
        private RedisHelp.RedisHelper ORedisHelp = null;
        //异步记录日志
        private bool AsyncWriteLog = false;
        //币种-代码缓存
        private List<CurrencyCode> ArrCurrencyCode = new List<CurrencyCode>();

        public DailyRatesController(IDailyRateService dailyRateService, IUnitOfWorkAsync unitOfWork)
            : base(true)
        {
            var WebServiceName = this.GetType().Name;

            #region Redis初始化链接

            try
            {
                ORedisHelp = new RedisHelp.RedisHelper();
            }
            catch (Exception)
            {
                ORedisHelp = null;
                Common.WriteLog_Local("创建Redis失败，链接无效或出错", WebServiceName + "\\Redis", true, AsyncWriteLog, false, "", DateTime.Now);
            }

            #endregion

            #region 异步记录日志

            try
            {
                AsyncWriteLog = (bool)CacheHelper.Get_SetBoolConfAppSettings(WebServiceName + "\\Config", Common.CacheNameS.AsyncWriteLog.ToString());
            }
            catch (Exception)
            {
                Common.WriteLog_Local("读取AsyncWebServiceLog失败", WebServiceName + "\\Config", true, AsyncWriteLog, false, "", DateTime.Now);
            }

            #endregion

            _dailyRateService = dailyRateService;
            _unitOfWork = unitOfWork;

            getArrCurrencyCode();
        }

        /// <summary>
        /// 获取币种代码
        /// </summary>
        /// <returns></returns>
        public void getArrCurrencyCode()
        {
            if (ArrCurrencyCode == null || !ArrCurrencyCode.Any())
            {
                var CurrencyCodeJsonStr = ORedisHelp.StringGet("CurrencyCode");
                if (!string.IsNullOrEmpty(CurrencyCodeJsonStr))
                    ArrCurrencyCode = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CurrencyCode>>(CurrencyCodeJsonStr);
            }
        }

        // GET: DailyRates/Index
        public ActionResult Index()
        {
            //var dailyrate  = _dailyRateService.Queryable().AsQueryable();
            //return View(dailyrate  );
            //List<DeviceInfo> ArrDeviceInfos = DeviceManager();
            //Save(ArrDeviceInfos[0]);
            return View();
        }

        #region WIA扫描仪

        public List<DeviceInfo> DeviceManager()
        {
            List<dynamic> ArrDeviceProty = new List<dynamic>();
            List<DeviceInfo> ArrDeviceInfos = new List<DeviceInfo>();
            // Create a DeviceManager instance
            var deviceManager = new DeviceManager();

            // Loop through the list of devices
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                var ScanDevice = deviceManager.DeviceInfos[i];
                // Skip the device if it's not a scanner
                if (ScanDevice.Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }
                ArrDeviceInfos.Add(ScanDevice);
                var Num = ScanDevice.Properties.Count;
                for (var x = 1; x <= Num; x++)
                {
                    var ItemProtyName = ScanDevice.Properties[x].Name;
                    var ItemProtyValue = ScanDevice.Properties[x].get_Value();
                    dynamic DeviceProty = new System.Dynamic.ExpandoObject();
                    DeviceProty.Name = ItemProtyName;
                    DeviceProty.Value = ItemProtyValue;
                    ArrDeviceProty.Add(DeviceProty);
                }

                // Print something like e.g "WIA Canoscan 4400F"
                //Console.WriteLine(deviceManager.DeviceInfos[i].Properties["Name"].get_Value());
                // e.g Canoscan 4400F
                //Console.WriteLine(deviceManager.DeviceInfos[i].Properties["Description"].get_Value());
                // e.g \\.\Usbscan0
                //Console.WriteLine(deviceManager.DeviceInfos[i].Properties["Port"].get_Value());
            }
            ViewBag.ArrDeviceProty = ArrDeviceProty;
            return ArrDeviceInfos;
        }

        public void Save(DeviceInfo ObjIDeviceInfo)
        {
            try
            {
                // Connect to the first available scanner
                var device = ObjIDeviceInfo.Connect();

                List<dynamic> ArrDeviceItemProty = new List<dynamic>();
                // Select the scanner
                var scannerItem = device.Items[1];
                var Num = device.Items.Count;
                for (var i = 1; i <= Num; i++)
                {
                    var Item = device.Items[i];
                    for (var x = 1; x <= Item.Properties.Count; x++)
                    {
                        var ItemProtyName = device.Items[i].Properties[x].Name;
                        var ItemProtyValue = device.Items[i].Properties[x].get_Value();
                        dynamic DeviceProty = new System.Dynamic.ExpandoObject();
                        DeviceProty.Name = ItemProtyName;
                        DeviceProty.Value = ItemProtyValue;
                        ArrDeviceItemProty.Add(DeviceProty);
                    }
                }
                ViewBag.ArrDeviceItemProty = ArrDeviceItemProty;

                // Set the scanner settings
                //int resolution = 150;
                //int width_pixel = 1250;
                //int height_pixel = 1700;
                //int color_mode = 1;
                //AdjustScannerSettings(scannerItem, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                // Retrieve a image in JPEG format and store it into a variable
                var imageFile = (ImageFile)scannerItem.Transfer(FormatID.wiaFormatJPEG);

                // Save the image in some path with filename
                var path = @"D:\Scanner\Image\";
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                var ScannerFile = path + (new Random()).Next(1, 100).ToString("00") + "scan.jpg";
                if (System.IO.File.Exists(ScannerFile))
                {
                    System.IO.File.Delete(ScannerFile);
                }

                // Save image !
                imageFile.SaveFile(ScannerFile);
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                // Convert the error code to UINT
                uint errorCode = (uint)e.ErrorCode;

                // See the error codes
                if (errorCode == 0x80210006)
                {
                    Console.WriteLine("The scanner is busy or isn't ready");
                }
                else if (errorCode == 0x80210064)
                {
                    Console.WriteLine("The scanning process has been cancelled.");
                }
                else if (errorCode == 0x8021000C)
                {
                    Console.WriteLine("There is an incorrect setting on the WIA device.");
                }
                else if (errorCode == 0x80210005)
                {
                    Console.WriteLine("The device is offline. Make sure the device is powered on and connected to the PC.");
                }
                else if (errorCode == 0x80210001)
                {
                    Console.WriteLine("An unknown error has occurred with the WIA device.");
                }
                else
                {
                    Console.WriteLine(errorCode + "An unknown error has occurred with the WIA device.");
                }
            }
        }

        public void ScannerDialog()
        {
            // Create a DeviceManager instance
            var deviceManager = new DeviceManager();

            // Create an empty variable to store the scanner instance
            DeviceInfo firstScannerAvailable = null;

            // Loop through the list of devices to choose the first available
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                // Skip the device if it's not a scanner
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }
                firstScannerAvailable = deviceManager.DeviceInfos[i];
                break;
            }
            // Connect to the first available scanner
            var device = firstScannerAvailable.Connect();
            // Select the scanner
            var scannerItem = device.Items[1];

            CommonDialogClass dlg = new CommonDialogClass();
            try
            {
                object scanResult = dlg.ShowTransfer(scannerItem, WIA.FormatID.wiaFormatPNG, true);
                if (scanResult != null)
                {
                    ImageFile image = (ImageFile)scanResult;
                    // Do the rest of things as save the image 
                }
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                // Display the exception in the console.
                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                // Catch 2 of the most common exceptions
                if (errorCode == 0x80210006)
                {
                    Console.WriteLine("The scanner is busy or isn't ready");
                }
                else if (errorCode == 0x80210064)
                {
                    Console.WriteLine("The scanning process has been cancelled.");
                }
            }
        }

        /// <summary>
        /// Adjusts the settings of the scanner with the providen parameters.
        /// </summary>
        /// <param name="scannnerItem">Scanner Item</param>
        /// <param name="scanResolutionDPI">Provide the DPI resolution that should be used e.g 150</param>
        /// <param name="scanStartLeftPixel"></param>
        /// <param name="scanStartTopPixel"></param>
        /// <param name="scanWidthPixels"></param>
        /// <param name="scanHeightPixels"></param>
        /// <param name="brightnessPercents"></param>
        /// <param name="contrastPercents">Modify the contrast percent</param>
        /// <param name="colorMode">Set the color mode</param>
        private static void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, int colorMode)
        {
            const string WIA_SCAN_COLOR_MODE = "6146";
            const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
            const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
            const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
            const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
            const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
            const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
            const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
            const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
        }

        /// <summary>
        /// Modify a WIA property
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        private static void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            Property prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }

        #endregion

        // Get :DailyRates/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var dailyrate = _dailyRateService.Query(new DailyRateQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = dailyrate.Select(n => new
            {
                Id = n.Id,
                LocalCurrency = n.LocalCurrency,
                LocalCurrCode = n.LocalCurrCode,
                ForeignCurrency = n.ForeignCurrency,
                ForeignCurrCode = n.ForeignCurrCode,
                PriceType = n.PriceType,
                BankName = n.BankName,
                Price = n.Price,
                ScrapyDate = n.ScrapyDate,
                Description = n.Description,
                Status = n.Status,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(DailyRateChangeViewModel dailyrate)
        {
            if (dailyrate.updated != null)
            {
                #region

                var ControllActinMsg = "编辑";
                bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Edit", ControllActinMsg);
                if (!IsHaveQx)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = "您没有权限" + ControllActinMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                        return null;
                    }
                }

                #endregion

                foreach (var updated in dailyrate.updated)
                {
                    _dailyRateService.Update(updated);
                }
            }
            if (dailyrate.deleted != null)
            {
                #region

                var ControllActinMsg = "删除";
                bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Delete", ControllActinMsg);
                if (!IsHaveQx)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = "您没有权限" + ControllActinMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                        return null;
                    }
                }

                #endregion

                foreach (var deleted in dailyrate.deleted)
                {
                    _dailyRateService.Delete(deleted);
                }
            }
            if (dailyrate.inserted != null)
            {
                #region

                var ControllActinMsg = "创建";
                bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Create", ControllActinMsg);
                if (!IsHaveQx)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = "您没有权限" + ControllActinMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                        return null;
                    }
                }

                #endregion

                foreach (var inserted in dailyrate.inserted)
                {
                    _dailyRateService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((dailyrate.updated != null && dailyrate.updated.Any()) ||
                (dailyrate.deleted != null && dailyrate.deleted.Any()) ||
                (dailyrate.inserted != null && dailyrate.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(dailyrate);
                    return Json(new { Success = true, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作", ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: DailyRates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyRate dailyRate = _dailyRateService.Find(id);
            if (dailyRate == null)
            {
                return HttpNotFound();
            }
            return View(dailyRate);
        }

        /// <summary>
        /// 获取实时汇率
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDailyRateByRedis()
        {
            //时间转换格式
            var StrDateTimeFormat = new string[] {
                                "yyyyMMdd", 
                                "yyyy-MM-dd", 
                                "yyyy/MM/dd", 
                                "yyyyMMddHHmm", "yyyyMMddHHmmss", 
                                "yyyy-MM-dd HH:mm:ss", 
                                "yyyy/MM/dd HH:mm:ss" 
                            };
            List<string> ArrErrMsg = new List<string>();
            Tuple<bool, string> RetRunCmd;
            //最后一次爬取时间
            DateTime ScrapyLastDate;
            var ScrapyLastDateStr = Session["ScrapyLastDate"] == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : Session["ScrapyLastDate"].ToString();
            if (!DateTime.TryParseExact(ScrapyLastDateStr, StrDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ScrapyLastDate))
            {
                RetRunCmd = new Tuple<bool, string>(false, "爬虫爬取时间至少需要间隔1小时");
                ScrapyLastDate = DateTime.Now;
            }
            else
            {
                //时间间隔
                TimeSpan Time_Span = DateTime.Now - ScrapyLastDate;
                if (Time_Span.Hours > 1)
                {
                    string ScrapyPath = (string)CacheHelper.Get_SetCache(Common.CacheNameS.ScrapyPath);
                    if (string.IsNullOrWhiteSpace(ScrapyPath))
                    {
                        RetRunCmd = new Tuple<bool, string>(false, "ScrapyPath爬虫路径未明确");
                    }
                    else
                    {
                        //CMD 执行爬虫程序
                        RetRunCmd = Common.RunCmd("cmd.exe", "scrapy crawl RMBHL --nolog", ScrapyPath);
                    }
                }
                else
                    RetRunCmd = new Tuple<bool, string>(false, "爬虫爬取时间至少需要间隔1小时");
            }
            if (RetRunCmd.Item1)
            {
                #region 获取爬虫 数据（Redis）

                if (ArrCurrencyCode == null || !ArrCurrencyCode.Any())
                {
                    getArrCurrencyCode();
                }
                Dictionary<string, IEnumerable<DailyRateJson>> DictArrDailyRate = new Dictionary<string, IEnumerable<DailyRateJson>>();
                var Set_RMBHL_Key = "{RMBHL}:";//redis 集群时 {}会强制存放于某一个hashslot上,否则会报错
                var SetLen = ORedisHelp.SetLength(Set_RMBHL_Key);//Redis-Set 集合长度
                var ODailyRateKey = "";//记录Redis-Set—Key
                int Num = 0;//记录次数，重复出错次数2倍于Redis-Set 集合长度
                int MaxErrNum = 10;//允许最大出错次数
                int ErrNum = 0;//记录出错次数

                #region 从Redis获取并暂存到 字典数据集 DictArrDailyRate

                while ((!string.IsNullOrEmpty(ODailyRateKey) && ErrNum < MaxErrNum) || Num == 0)
                {
                    if (Num == 0)
                    {
                        ODailyRateKey = ORedisHelp.SetPop<string>(Set_RMBHL_Key);
                        if (string.IsNullOrEmpty(ODailyRateKey))
                            break;
                    }
                    Num++;
                    var NewArrDailyRate = ORedisHelp.SetMembers<string>(ODailyRateKey);
                    if (NewArrDailyRate == null || !NewArrDailyRate.Any())
                    {
                        ODailyRateKey = ORedisHelp.SetPop<string>(Set_RMBHL_Key);
                        continue;
                    }
                    else
                    {
                        long ret = ORedisHelp.SetRemove<string>(ODailyRateKey, NewArrDailyRate);
                        if (ret > 0)
                        {
                            var ArrDailyRateJson = NewArrDailyRate.Select(x => Newtonsoft.Json.JsonConvert.DeserializeObject<DailyRateJson>(x));
                            var WhereDictArrDailyRate = DictArrDailyRate.Where(x => x.Key == ODailyRateKey);
                            if (WhereDictArrDailyRate.Any())
                            {
                                WhereDictArrDailyRate.FirstOrDefault().Value.ToList().AddRange(ArrDailyRateJson);
                            }
                            else
                            {
                                DictArrDailyRate.Add(ODailyRateKey, ArrDailyRateJson);
                            }
                        }
                        else
                        {
                            ErrNum++;
                            ArrErrMsg.Add("Set集合(" + ODailyRateKey + ")删除元素错误");
                            var setAddTF = ORedisHelp.SetAdd<string>("{RMBHL}:", ODailyRateKey);
                            if (!setAddTF)
                            {
                                ErrNum++;
                                ArrErrMsg.Add(ODailyRateKey + "_" + "重新插入(" + Set_RMBHL_Key + ")集合错误");
                            }
                            ODailyRateKey = ORedisHelp.SetPop<string>(Set_RMBHL_Key);
                            continue;
                        }
                    }
                    ODailyRateKey = ORedisHelp.SetPop<string>(Set_RMBHL_Key);
                }

                #endregion

                if (ArrErrMsg.Any())
                {
                    if (Request.IsAjaxRequest())
                    {
                        var ErrMsgStr = string.Join(",", ArrErrMsg);
                        return Json(new { Success = false, ErrMsg = ErrMsgStr }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return HttpNotFound();
                }
                else
                {
                    #region  整理数据，并插入数据库

                    foreach (var item in DictArrDailyRate)
                    {
                        DateTime ScrapyDate;
                        var ScrapyDateStr = item.Key;
                        var len = Set_RMBHL_Key.Length;
                        var i = ScrapyDateStr.IndexOf(Set_RMBHL_Key) + len;
                        var e = ScrapyDateStr.IndexOf('_');
                        if (i >= len && (i + Set_RMBHL_Key.Length) < e)
                        {
                            ScrapyDateStr = ScrapyDateStr.Substring(i, (e - i));

                            if (!DateTime.TryParseExact(ScrapyDateStr, StrDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ScrapyDate))
                            {
                                ArrErrMsg.Add(item.Key + ":计算时间错误");
                                break;
                            }
                            else
                            {
                                IEnumerable<DailyRate> ArrDailyRate = item.Value.Select((x, num) => new DailyRate
                                {
                                    Id = num,
                                    LocalCurrency = x.LocalCurrency,
                                    LocalCurrCode = "CNY",
                                    ForeignCurrency = x.ForeignCurrency,
                                    ForeignCurrCode = ArrCurrencyCode.Where(n => n.currency == x.ForeignCurrency).FirstOrDefault().code,
                                    PriceType = x.PriceType,
                                    BankName = x.BankName,
                                    Price = x.Price1 == null ? 0 : (decimal)x.Price1,
                                    ScrapyDate = ScrapyDate,
                                    Status = true
                                });
                                _dailyRateService.InsertRange(ArrDailyRate);
                            }
                        }
                    }
                    try
                    {
                        _unitOfWork.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        var ErrMsg = Common.GetExceptionMsg(ex);
                        ArrErrMsg.Add("插入实时汇率错误：" + ErrMsg);
                    }

                    #endregion
                }
                if (ArrErrMsg.Any())
                {
                    if (Request.IsAjaxRequest())
                    {
                        var ErrMsgStr = string.Join(",", ArrErrMsg);
                        return Json(new { Success = false, ErrMsg = ErrMsgStr }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return HttpNotFound();
                }
                else
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = true, ErrMsg = RetRunCmd.Item2 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return HttpNotFound();
                }

                #endregion
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = false, ErrMsg = RetRunCmd.Item2 }, JsonRequestBehavior.AllowGet);
                }
                else
                    return HttpNotFound();
            }
        }

        // GET: DailyRates/Create
        public ActionResult Create()
        {
            DailyRate dailyRate = new DailyRate();
            //set default value
            return View(dailyRate);
        }

        // POST: DailyRates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LocalCurrency,LocalCurrCode,ForeignCurrency,ForeignCurrCode,PriceType,BankName,Price,ScrapyDate,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] DailyRate dailyRate)
        {
            if (ModelState.IsValid)
            {
                _dailyRateService.Insert(dailyRate);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a DailyRate record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(dailyRate);
        }

        // GET: DailyRates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyRate dailyRate = _dailyRateService.Find(id);
            if (dailyRate == null)
            {
                return HttpNotFound();
            }
            return View(dailyRate);
        }

        // POST: DailyRates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LocalCurrency,LocalCurrCode,ForeignCurrency,ForeignCurrCode,PriceType,BankName,Price,ScrapyDate,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] DailyRate dailyRate)
        {
            if (ModelState.IsValid)
            {
                dailyRate.ObjectState = ObjectState.Modified;
                _dailyRateService.Update(dailyRate);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a DailyRate record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(dailyRate);
        }

        // GET: DailyRates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyRate dailyRate = _dailyRateService.Find(id);
            if (dailyRate == null)
            {
                return HttpNotFound();
            }
            return View(dailyRate);
        }

        // POST: DailyRates/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DailyRate dailyRate = _dailyRateService.Find(id);
            _dailyRateService.Delete(dailyRate);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a DailyRate record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "dailyrate_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _dailyRateService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        private void DisplaySuccessMessage(string msgText)
        {
            TempData["SuccessMessage"] = msgText;
        }

        private void DisplayErrorMessage()
        {
            TempData["ErrorMessage"] = "Save changes was unsuccessful.";
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _unitOfWork.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
