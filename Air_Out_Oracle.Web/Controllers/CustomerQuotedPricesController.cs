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

namespace AirOut.Web.Controllers
{
    public class CustomerQuotedPricesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<CustomerQuotedPrice>, Repository<CustomerQuotedPrice>>();
        //container.RegisterType<ICustomerQuotedPriceService, CustomerQuotedPriceService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPARA_AreaService _pARA_AreaService;
        private readonly ICustomerQuotedPriceService _customerQuotedPriceService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/CustomerQuotedPrices";

        public CustomerQuotedPricesController(
            ICustomerQuotedPriceService customerQuotedPriceService,
            IPARA_AreaService pARA_AreaService,
            IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _customerQuotedPriceService = customerQuotedPriceService;
            _pARA_AreaService = pARA_AreaService;
            _unitOfWork = unitOfWork;
            ControllerQXName = "/" + this.GetType().Name.Replace("Controller", "");
        }

        // GET: CustomerQuotedPrices/Index
        public ActionResult Index()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            var ArrAuditStatus = Common.GetEnumToDic("AuditStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrAuditStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrAuditStatus);
            //var customerquotedprice  = _customerQuotedPriceService.Queryable().AsQueryable();
            //return View(customerquotedprice  );
            return View();
        }

        /// <summary>
        /// 审批
        /// </summary>
        /// <returns></returns>
        public ActionResult Audit()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            var ArrAuditStatus = Common.GetEnumToDic("AuditStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrAuditStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrAuditStatus);
            //var customerquotedprice  = _customerQuotedPriceService.Queryable().AsQueryable();
            //return View(customerquotedprice  );
            return View();
        }

        // Get :CustomerQuotedPrices/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            var QBD_DEFDOC_LIST = CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST) as List<BD_DEFDOC_LIST>;
            var QCusBusInfo = CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo) as List<CusBusInfo>;
            var QPARA_Area = CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Area) as List<PARA_Area>;
            //int pagenum = offset / limit +1;
            var customerquotedprice = _customerQuotedPriceService.Query(new CustomerQuotedPriceQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = customerquotedprice.Select(n => new
            {
                Id = n.Id,
                SerialNo = n.SerialNo,
                BusinessType = n.BusinessType,
                CustomerCode = n.CustomerCode,
                LocalWHMark = n.LocalWHMark,
                StartPlace = n.StartPlace,
                TransitPlace = n.TransitPlace,
                EndPlace = n.EndPlace,
                ProxyOperator = n.ProxyOperator,
                CusDefinition = n.CusDefinition,
                Receiver = n.Receiver,
                Shipper = n.Shipper,
                Contact = n.Contact,
                QuotedPricePolicy = n.QuotedPricePolicy,
                Seller = n.Seller,
                StartDate = n.StartDate,
                EndDate = n.EndDate,
                Description = n.Description,
                AuditStatus = n.AuditStatus,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();
            var ArrCusBusInfo = CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo) as List<CusBusInfo>;
            var ARRArea_AirPort = CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort) as List<PARA_AirPort>;
            var QBusinessType = QBD_DEFDOC_LIST.Where(x => x.DOCCODE == "BusinessTy").ToList();
            var ArrApplicationUser = CacheHelper.Get_SetCache(Common.CacheNameS.ApplicationUser) as List<ApplicationUser>;

            var resultData = (from x in datarows
                              //join t in QBusinessType on x.BusinessType equals t.LISTCODE into tmp1
                              //from ttmp1 in tmp1.DefaultIfEmpty()
                              join t2 in ArrCusBusInfo on x.CustomerCode equals t2.EnterpriseId into tmp2
                              from ttmp2 in tmp2.DefaultIfEmpty()
                              //join t3 in ARRArea_AirPort on x.StartPlace equals t3.PortCode into tmp3
                              //from ttmp3 in tmp3.DefaultIfEmpty()
                              //join t4 in ARRArea_AirPort on x.EndPlace equals t4.PortCode into tmp4
                              //from ttmp4 in tmp4.DefaultIfEmpty()
                              //join t5 in ARRArea_AirPort on x.TransitPlace equals t5.PortCode into tmp5
                              //from ttmp5 in tmp5.DefaultIfEmpty()
                              join t6 in ArrApplicationUser on x.ADDWHO equals t6.UserName into tmp6
                              from ttmp6 in tmp6.DefaultIfEmpty()
                              join t7 in ArrApplicationUser on x.EDITWHO equals t7.UserName into tmp7
                              from ttmp7 in tmp7.DefaultIfEmpty()
                              select new
                              {
                                  Id = x.Id,
                                  SerialNo = x.SerialNo,
                                  //BusinessType = x.BusinessType,
                                  //BusinessTypeNAME = ttmp1 == null ? string.Empty : ttmp1.LISTNAME,
                                  CustomerCode = x.CustomerCode,
                                  CustomerCodeNAME = ttmp2 == null ? string.Empty : ttmp2.EnterpriseShortName,
                                  LocalWHMark = x.LocalWHMark,
                                  StartPlace = x.StartPlace,
                                  //StartPlaceNAME = ttmp3 == null ? string.Empty : ttmp3.PlaceName,
                                  EndPlace = x.EndPlace,
                                  //EndPlaceNAME = ttmp4 == null ? string.Empty : ttmp4.PortName,
                                  TransitPlace = x.TransitPlace,
                                  //TransitPlaceNAME = ttmp5 == null ? string.Empty : ttmp5.PortName,
                                  ProxyOperator = x.ProxyOperator,
                                  CusDefinition = x.CusDefinition,
                                  Receiver = x.Receiver,
                                  Shipper = x.Shipper,
                                  Contact = x.Contact,
                                  QuotedPricePolicy = x.QuotedPricePolicy,
                                  Seller = x.Seller,
                                  StartDate = x.StartDate,
                                  EndDate = x.EndDate,
                                  Description = x.Description,
                                  AuditStatus = x.AuditStatus,
                                  Status = x.Status,
                                  OperatingPoint = x.OperatingPoint,
                                  ADDWHO = x.ADDWHO,
                                  ADDWHONAME = ttmp6 == null ? string.Empty : ttmp6.UserNameDesc,
                                  ADDTS = x.ADDTS,
                                  EDITWHO = x.EDITWHO,
                                  EDITWHONAME = ttmp7 == null ? string.Empty : ttmp7.UserNameDesc,
                                  EDITTS = x.EDITTS
                              }).ToList();
            var pagelist = new { total = totalCount, rows = resultData };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="customerquotedprice"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData(CustomerQuotedPriceChangeViewModel customerquotedprice, int? id)
        {
            string serialNo = string.Empty;
            if (customerquotedprice.updated != null)
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

                foreach (var updated in customerquotedprice.updated)
                {
                    _customerQuotedPriceService.Update(updated);
                }
            }
            if (customerquotedprice.deleted != null)
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

                foreach (var deleted in customerquotedprice.deleted)
                {
                    _customerQuotedPriceService.Delete(deleted);
                }
            }
            if (customerquotedprice.inserted != null)
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

                foreach (var inserted in customerquotedprice.inserted)
                {
                    _customerQuotedPriceService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((customerquotedprice.updated != null && customerquotedprice.updated.Any()) ||
                (customerquotedprice.deleted != null && customerquotedprice.deleted.Any()) ||
                (customerquotedprice.inserted != null && customerquotedprice.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(customerquotedprice);
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

        /// <summary>
        /// 批量送审
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveBatchAuditStatus(Array ids)
        {
            var strids = "";
            foreach (var item in ids)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var ArrUpte = new List<CustomerQuotedPrice>();
            foreach (var id in idarr)
            {
                CustomerQuotedPrice customerQuotedPrice = _customerQuotedPriceService.Find(Int32.Parse(id));
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.Auditing)
                {
                    //状态为审批中，跳过
                    continue;
                }
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                {
                    //状态为审批通过，跳过
                    continue;
                }
                customerQuotedPrice.AuditStatus = AirOutEnumType.AuditStatusEnum.Auditing;
                _customerQuotedPriceService.Update(customerQuotedPrice);
                ArrUpte.Add(customerQuotedPrice);
            }
            try
            {
                if (ArrUpte.Any())
                {
                    _unitOfWork.SaveChanges();
                    var ChangeViewModel = new CustomerQuotedPriceChangeViewModel();
                    ChangeViewModel.updated = ArrUpte;
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(ChangeViewModel);
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult SaveAuditFail(Array ids)
        {
            var Strids = "";
            foreach (var item in ids)
            {
                Strids = item.ToString();
            }
            var idArr = Strids.Split(",");
            var ArrUpte = new List<CustomerQuotedPrice>();
            foreach (var id in idArr)
            {
                CustomerQuotedPrice customerQuotedPrice = _customerQuotedPriceService.Find(Int32.Parse(id));
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.draft)
                {
                    //状态为草稿，跳过 
                    continue;
                }
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditFail)
                {
                    //状态为拒绝，跳过 
                    continue;
                }
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                {
                    //状态为通过，跳过 
                    continue;
                }
                customerQuotedPrice.AuditStatus = AirOutEnumType.AuditStatusEnum.AuditFail;
                _customerQuotedPriceService.Update(customerQuotedPrice);
                ArrUpte.Add(customerQuotedPrice);
            }
            try
            {
                if (ArrUpte.Any())
                {
                    _unitOfWork.SaveChanges();
                    var ChangeViewModel = new CustomerQuotedPriceChangeViewModel();
                    ChangeViewModel.updated = ArrUpte;
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(ChangeViewModel);
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Succee = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAuditsuccee(Array ids)
        {
            var Strids = "";
            foreach (var item in ids)
            {
                Strids = item.ToString();
            }
            var idArr = Strids.Split(",");
            var ArrUpte = new List<CustomerQuotedPrice>();
            foreach (var id in idArr)
            {
                CustomerQuotedPrice customerQuotedPrice = _customerQuotedPriceService.Find(Int32.Parse(id));
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.draft)
                {
                    //状态为草稿，跳过 
                    continue;
                }
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditFail)
                {
                    //状态为拒绝，跳过 
                    continue;
                }
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                {
                    //状态为通过，跳过 
                    continue;
                }
                customerQuotedPrice.AuditStatus = AirOutEnumType.AuditStatusEnum.AuditSuccess;
                _customerQuotedPriceService.Update(customerQuotedPrice);
                ArrUpte.Add(customerQuotedPrice);
            }
            try
            {
                if (ArrUpte.Any())
                {
                    _unitOfWork.SaveChanges();
                    var ChangeViewModel = new CustomerQuotedPriceChangeViewModel();
                    ChangeViewModel.updated = ArrUpte;
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(ChangeViewModel);
                    return Json(new { Success = true, ErrMsg = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Succee = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveDeleteMore(Array ids)
        {
            var strids = "";
            foreach (var item in ids)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var ArrDelt = new List<CustomerQuotedPrice>();
            foreach (var id in idarr)
            {
                CustomerQuotedPrice customerQuotedPrice = _customerQuotedPriceService.Find(Int32.Parse(id));
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.Auditing)
                {
                    //状态为审批中，跳过
                    continue;
                }
                if (customerQuotedPrice.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                {
                    //状态为审批通过，跳过
                    continue;
                }
                _customerQuotedPriceService.Delete(customerQuotedPrice);
                ArrDelt.Add(customerQuotedPrice);
            }
            try
            {
                if (ArrDelt.Any())
                {
                    _unitOfWork.SaveChanges();
                    var ChangeViewModel = new CustomerQuotedPriceChangeViewModel();
                    ChangeViewModel.deleted = ArrDelt;
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(ChangeViewModel);
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "没有任何需要删除数据库的操作" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        // GET: CustomerQuotedPrices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerQuotedPrice customerQuotedPrice = _customerQuotedPriceService.Find(id);
            if (customerQuotedPrice == null)
            {
                return HttpNotFound();
            }
            return View(customerQuotedPrice);
        }

        // GET: CustomerQuotedPrices/Create
        public ActionResult Create(int? id = 0)
        {
            CustomerQuotedPrice customerQuotedPrice = new CustomerQuotedPrice();
            //set default value
            if (id == 0)
            {
                customerQuotedPrice.StartDate = DateTime.Parse(DateTime.Now.ToString("d"));
                customerQuotedPrice.EndDate = DateTime.Parse(DateTime.Now.ToString("d")).AddYears(2);
                customerQuotedPrice.Status = AirOutEnumType.UseStatusEnum.Enable;
                customerQuotedPrice.AuditStatus = 0;
                ViewData["ADDWHONAME"] = Utility.CurrentAppUser.UserNameDesc;
            }
            else
            {
                CustomerQuotedPrice customerQuotedPriceEdit = _customerQuotedPriceService.Find(id);
                if (customerQuotedPriceEdit == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    customerQuotedPrice = customerQuotedPriceEdit;
                }
                ViewData["ADDWHONAME"] = Utility.CurrentAppUser.UserNameDesc;
            }

            return View(customerQuotedPrice);
        }

        // POST: CustomerQuotedPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(CustomerQuotedPrice customerQuotedPrice)
        {
            ModelState.Remove("SerialNo");//去除验证
            if (ModelState.IsValid)
            {
                customerQuotedPrice.SerialNo = SequenceBuilder.NextCustomerQuotedSerial_No();
                _customerQuotedPriceService.Insert(customerQuotedPrice);
                _unitOfWork.SaveChanges();
                var ChangeViewModel = new CustomerQuotedPriceChangeViewModel();
                ChangeViewModel.inserted = new List<CustomerQuotedPrice> { customerQuotedPrice };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(ChangeViewModel);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a CustomerQuotedPrice record");
                return RedirectToAction("Index");
            }

            var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            var ODynamic = GetFromNAME(customerQuotedPrice);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return View(customerQuotedPrice);
        }

        // GET: CustomerQuotedPrices/Edit/5
        public ActionResult Edit(int? id)
        {
            //计费公式 缓存
            ViewData["BillFormulaXML"] = AirOut.Web.CacheHelper.Get_SetCache(AirOut.Web.Extensions.Common.CacheNameS.BillFormulaXML);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerQuotedPrice customerQuotedPrice = _customerQuotedPriceService.Find(id);
            //if (customerQuotedPrice.ADDTS == null)
            //    customerQuotedPrice.ADDTS.Value.ToString()== "";
            if (customerQuotedPrice == null)
            {
                return HttpNotFound();
            }
            else
            {
                var ArrApplicationUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetCache(Common.CacheNameS.ApplicationUser);
                var OAppUser = ArrApplicationUser.Where(x => x.UserName == customerQuotedPrice.ADDWHO).FirstOrDefault();
                ViewData["ADDWHONAME"] = OAppUser == null ? "" : OAppUser.UserNameDesc;
                ViewData["EDITWHONAME"] = Utility.CurrentAppUser.UserNameDesc;
                var ODynamic = GetFromNAME(customerQuotedPrice);
                ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            }
            return View(customerQuotedPrice);
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(CustomerQuotedPrice OCustomerQuotedPrice)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();

            //if (!string.IsNullOrEmpty(OCustomerQuotedPrice.BusinessType))
            //{
            //    var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            //    var OBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "BusinessTy" && x.LISTCODE == OCustomerQuotedPrice.BusinessType).FirstOrDefault();
            //    ODynamic.BusinessTypeNAME = OBD_DEFDOC_LIST == null ? "" : OBD_DEFDOC_LIST.LISTNAME;
            //}

            if (!string.IsNullOrEmpty(OCustomerQuotedPrice.CustomerCode))
            {
                var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OCustomerQuotedPrice.CustomerCode).FirstOrDefault();
                ODynamic.CustomerCodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseName;
            }

            if (!string.IsNullOrEmpty(OCustomerQuotedPrice.EndPlace))
            {
                var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
                var OPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OCustomerQuotedPrice.EndPlace).FirstOrDefault();
                if (OPARA_AirPort != null && !string.IsNullOrEmpty(OPARA_AirPort.PortName))
                {
                    ODynamic.EndPlaceNAME = OPARA_AirPort == null ? "" : OPARA_AirPort.PortName;
                }
            }

            //IEnumerable<ComboDridListModel> QAddressPlace = _pARA_AreaService.GetComboDridListModel_FromCache();
            //if (!string.IsNullOrEmpty(OCustomerQuotedPrice.StartPlace))
            //{
            //    var OPARA_Area = QAddressPlace.Where(x => x.ID == OCustomerQuotedPrice.StartPlace).FirstOrDefault();
            //    if (OPARA_Area != null && !string.IsNullOrEmpty(OPARA_Area.ID))
            //    {
            //        ODynamic.StartPlaceNAME = OPARA_Area == null ? "" : OPARA_Area.TEXT;
            //    }
            //}
            //if (!string.IsNullOrEmpty(OCustomerQuotedPrice.TransitPlace))
            //{
            //    var OPARA_Area = QAddressPlace.Where(x => x.ID == OCustomerQuotedPrice.TransitPlace).FirstOrDefault();
            //    if (OPARA_Area != null && !string.IsNullOrEmpty(OPARA_Area.ID))
            //    {
            //        ODynamic.TransitPlaceNAME = OPARA_Area == null ? "" : OPARA_Area.TEXT;
            //    }
            //}
            //if (!string.IsNullOrEmpty(OCustomerQuotedPrice.EndPlace))
            //{
            //    var OPARA_Area = QAddressPlace.Where(x => x.ID == OCustomerQuotedPrice.EndPlace).FirstOrDefault();
            //    if (OPARA_Area != null && !string.IsNullOrEmpty(OPARA_Area.ID))
            //    {
            //        ODynamic.EndPlaceNAME = OPARA_Area == null ? "" : OPARA_Area.TEXT;
            //    }
            //}

            return ODynamic;
        }

        // POST: CustomerQuotedPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerQuotedPrice customerQuotedPrice)
        {
            ModelState.Remove("SerialNo,ADDTS,EDITTS");
            if (ModelState.IsValid)
            {
                customerQuotedPrice.ObjectState = ObjectState.Modified;
                _customerQuotedPriceService.Update(customerQuotedPrice);
                _unitOfWork.SaveChanges();
                var ChangeViewModel = new CustomerQuotedPriceChangeViewModel();
                ChangeViewModel.updated = new List<CustomerQuotedPrice> { customerQuotedPrice };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(ChangeViewModel);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a CustomerQuotedPrice record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(customerQuotedPrice);
        }

        /// <summary>
        /// 审批编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AuditEdit(int? id)
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            var ArrAuditStatus = Common.GetEnumToDic("AuditStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrAuditStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrAuditStatus);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerQuotedPrice customerQuotedPrice = _customerQuotedPriceService.Find(id);
            //if (customerQuotedPrice.ADDTS == null)
            //    customerQuotedPrice.ADDTS.Value.ToString()== "";
            if (customerQuotedPrice == null)
            {
                return HttpNotFound();
            }
            return View(customerQuotedPrice);
        }

        // POST: CustomerQuotedPrices/AuditEdit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AuditEdit(CustomerQuotedPrice customerQuotedPrice)
        {
            ModelState.Remove("SerialNo,ADDTS,EDITTS");
            if (ModelState.IsValid)
            {
                customerQuotedPrice.ObjectState = ObjectState.Modified;
                _customerQuotedPriceService.Update(customerQuotedPrice);
                _unitOfWork.SaveChanges();
                var ChangeViewModel = new CustomerQuotedPriceChangeViewModel();
                ChangeViewModel.updated = new List<CustomerQuotedPrice> { customerQuotedPrice };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(ChangeViewModel);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a CustomerQuotedPrice record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(customerQuotedPrice);
        }

        // GET: CustomerQuotedPrices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerQuotedPrice customerQuotedPrice = _customerQuotedPriceService.Find(id);
            if (customerQuotedPrice == null)
            {
                return HttpNotFound();
            }
            return View(customerQuotedPrice);
        }

        // POST: CustomerQuotedPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomerQuotedPrice customerQuotedPrice = _customerQuotedPriceService.Find(id);
            _customerQuotedPriceService.Delete(customerQuotedPrice);
            _unitOfWork.SaveChanges();
            var ChangeViewModel = new CustomerQuotedPriceChangeViewModel();
            ChangeViewModel.deleted = new List<CustomerQuotedPrice> { customerQuotedPrice };
            //自动更新 缓存
            if (IsAutoResetCache)
                AutoResetCache(ChangeViewModel);
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a CustomerQuotedPrice record");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 复制新增
        /// </summary>
        /// <param name="ArrId">报价Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchCopyAdd(List<int> ArrId)
        {
            try
            {
                #region 验证

                if (ArrId == null || !ArrId.Any())
                {
                    return Json(new { Success = false, ErrMsg = "请至少选择一条数据" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ArrId = ArrId.Where(x => x > 0).ToList();
                }

                #endregion

                var ArrCusQuotedPrice = _customerQuotedPriceService.Queryable().Where(x => ArrId.Contains(x.Id)).ToList();
                ArrCusQuotedPrice = ArrCusQuotedPrice.Where(x => x.Status ==  AirOutEnumType.UseStatusEnum.Enable).ToList();
                if (ArrCusQuotedPrice.Count < ArrId.Count)
                    return Json(new { Success = false, ErrMsg = "所选数据中，部分数据 已被作废或删除" }, JsonRequestBehavior.AllowGet);

                var CusQuotedPriceDtlRep = _unitOfWork.Repository<CusQuotedPriceDtl>();
                var ArrCusQuotedPriceDtl = CusQuotedPriceDtlRep.Queryable().Where(x => ArrId.Contains(x.CusQPId)).ToList();

                var ChangViewModel = new CustomerQuotedPriceChangeViewModel();
                var AddChang = new List<CustomerQuotedPrice>();
                var AddDtlChang = new List<CusQuotedPriceDtl>();
                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                var IdNum = 0;//新增Id
                foreach (var item in ArrCusQuotedPrice)
                {
                    var Newitem = new CustomerQuotedPrice();
                    Newitem = Common.SetSamaProtity(Newitem, item);
                    Newitem.Id = IdNum--;
                    Newitem.SerialNo = SequenceBuilder.NextCustomerQuotedSerial_No();
                    Newitem.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                    Newitem.Status = AirOutEnumType.UseStatusEnum.Enable;
                    Newitem.ADDTS = null;
                    Newitem.ADDWHO = null;
                    Newitem.ADDID = null;
                    Newitem.EDITTS = null;
                    Newitem.EDITWHO = null;
                    var entry = WebdbContxt.Entry(Newitem);
                    entry.State = EntityState.Added;//必须设置为 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                    Newitem.ObjectState = ObjectState.Added;
                    AddChang.Add(Newitem);
                    var ret = 0;
                    try
                    {
                        ret = WebdbContxt.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        var ErrMsg = Common.GetExceptionMsg(ex);
                        Common.WriteLog_Local("BatchCopyAdd保存数据错误：" + item.Id + "-" + ErrMsg, "CusQP", true, true);
                    }
                    if (Newitem.Id <= 0)
                        continue;

                    #region 复制明细

                    var IdDtlNum = 0;//新增Id
                    var ArrCusQPDtl = ArrCusQuotedPriceDtl.Where(x => x.CusQPId == item.Id);
                    if (ArrCusQPDtl.Any())
                    {
                        foreach (var itemDtl in ArrCusQPDtl)
                        {
                            var NewitemDtl = new CusQuotedPriceDtl();
                            NewitemDtl = Common.SetSamaProtity(NewitemDtl, itemDtl);
                            NewitemDtl.Id = IdDtlNum--;
                            NewitemDtl.CusQPId = Newitem.Id;
                            NewitemDtl.CusQPSerialNo = Newitem.SerialNo;
                            NewitemDtl.ADDTS = null;
                            NewitemDtl.ADDWHO = null;
                            NewitemDtl.ADDID = null;
                            NewitemDtl.EDITTS = null;
                            NewitemDtl.EDITWHO = null;
                            var Dtlentry = WebdbContxt.Entry(NewitemDtl);
                            Dtlentry.State = EntityState.Added;//必须设置为 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                            NewitemDtl.ObjectState = ObjectState.Added;
                            AddDtlChang.Add(NewitemDtl);
                        }
                        try
                        {
                            ret = WebdbContxt.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            var ErrMsg = Common.GetExceptionMsg(e);
                            Common.WriteLog_Local("BatchCopyAdd保存明细数据错误：" + item.Id + "-" + ErrMsg, "CusQP", true, true);
                        }
                    }

                    #endregion
                }

                if (AddChang.Any())
                {
                    ChangViewModel.inserted = AddChang;
                    if (IsAutoResetCache)
                        AutoResetCache(ChangViewModel);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新的数据" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "customerquotedprice_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _customerQuotedPriceService.ExportExcel(filterRules, sort, order);
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
