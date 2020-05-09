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
    public class CustomsInspectionsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<CustomsInspection>, Repository<CustomsInspection>>();
        //container.RegisterType<ICustomsInspectionService, CustomsInspectionService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ICustomsInspectionService _customsInspectionService;
        private readonly IOPS_EntrustmentInforService _ops_EntrustmentInforService;
        private readonly IPictureService _pictureService;
        private readonly ICostMoneyService _costMoneyService;
        private readonly ICustomerQuotedPriceService _customerQuotedPriceService;
        private readonly IChangeOrderHistoryService _changeOrderHistoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/OPS_M_Orders";

        public CustomsInspectionsController(ICustomsInspectionService customsInspectionService, IPictureService pictureService, IOPS_EntrustmentInforService ops_EntrustmentInforService, IChangeOrderHistoryService changeOrderHistoryService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _customsInspectionService = customsInspectionService;
            _pictureService = pictureService;
            _ops_EntrustmentInforService = ops_EntrustmentInforService;
            _costMoneyService = (ICostMoneyService)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(CostMoneyService), "CostMoneyService");
            _customerQuotedPriceService = (ICustomerQuotedPriceService)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(CustomerQuotedPriceService), "CustomerQuotedPriceService");
            _changeOrderHistoryService = changeOrderHistoryService;
            _unitOfWork = unitOfWork;
        }

        // GET: CustomsInspections/Index
        //public ActionResult Index()
        //{
        //    var customsinspection = _customsInspectionService.Queryable().AsQueryable();
        //    return View(customsinspection);
        //    return View();
        //}

        // Get :CustomsInspections/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(string operation_id, int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules) ?? new List<filterRule>();
            var ArrFilter = new List<filterRule>();
            if (!filters.Any(x => x.field == "Operation_ID"))
            {
                var OPERATIONIDfilterRule = new filterRule();
                OPERATIONIDfilterRule.field = "Operation_ID";
                OPERATIONIDfilterRule.op = "equals";
                OPERATIONIDfilterRule.value = operation_id.ToString();
                ArrFilter.Add(OPERATIONIDfilterRule);
            }
            if (ArrFilter.Any())
                filters = filters.Concat(ArrFilter);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var customsinspection = _customsInspectionService.Query(new CustomsInspectionQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = customsinspection.Select(n => new
            {
                Id = n.Id,
                Operation_ID = n.Operation_ID,
                Flight_NO = n.Flight_NO,
                Flight_Date_Want = n.Flight_Date_Want,
                MBL = n.MBL,
                Consign_Code_CK = n.Consign_Code_CK,
                Book_Flat_Code = n.Book_Flat_Code,
                Customs_Declaration = n.Customs_Declaration,
                Num_BG = n.Num_BG,
                Remarks_BG = n.Remarks_BG,
                Customs_Broker_BG = n.Customs_Broker_BG,
                Customs_Date_BG = n.Customs_Date_BG,
                Pieces_TS = n.Pieces_TS,
                Weight_TS = n.Weight_TS,
                Volume_TS = n.Volume_TS,
                Pieces_Fact = n.Pieces_Fact,
                Weight_Fact = n.Weight_Fact,
                Volume_Fact = n.Volume_Fact,
                Pieces_BG = n.Pieces_BG,
                Weight_BG = n.Weight_BG,
                Volume_BG = n.Volume_BG,
                IS_Checked_BG = n.IS_Checked_BG,
                Check_QTY = n.Check_QTY,
                Check_Date = n.Check_Date,
                Fileupload = n.Fileupload,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetOps_entruin(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        //{
        //    var customsinspection = _unitOfWork.Repository<CustomsInspection>();
        //    var ops_entrustmentInfor = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable();
        //    var result = from n in customsinspection.Queryable()
        //                 join m in ops_entrustmentInfor on n.Operation_ID equals m.Operation_Id into hed
        //                 from ntmp in hed.DefaultIfEmpty()
        //                 select new
        //                 {
        //                     Customs_Declaration = n.Customs_Declaration,
        //                     Num_BG = n.Num_BG,
        //                     Remarks_BG = n.Remarks_BG,
        //                     Customs_Date_BG = n.Customs_Date_BG,
        //                 };
        //    var results = result.ToList();
        //    return Json(results, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult SaveData(CustomsInspectionChangeViewModel customsinspection)
        {
            if (customsinspection.updated != null)
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

                foreach (var updated in customsinspection.updated)
                {
                    _customsInspectionService.Update(updated);
                }
            }
            if (customsinspection.deleted != null)
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

                foreach (var deleted in customsinspection.deleted)
                {
                    _customsInspectionService.Delete(deleted);
                }
            }
            if (customsinspection.inserted != null)
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

                foreach (var inserted in customsinspection.inserted)
                {
                    _customsInspectionService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((customsinspection.updated != null && customsinspection.updated.Any()) ||
                (customsinspection.deleted != null && customsinspection.deleted.Any()) ||
                (customsinspection.inserted != null && customsinspection.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(customsinspection);
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

        //保存新增
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SaveAppend(CustomsInspection customsInspection)
        {
            try
            {
                var ContentInsp = "";
                List<string> excludeInsp = new List<string>() { "Id", "OperatingPoint", "ADDID", "ADDWHO", "ADDTS", "EDITWHO", "EDITTS", "EDITID" };
              
                if (customsInspection != null)
                {
                    if (customsInspection.Id == 0)
                    {
                        customsInspection.ObjectState = ObjectState.Added;
                        _customsInspectionService.Insert(customsInspection);
                    }
                    else
                    {
                        var item = _customsInspectionService.Find(customsInspection.Id);
                        ContentInsp = Common.DifferenceComparison<CustomsInspection>(item, customsInspection, excludeInsp);
                        
                        item.Operation_ID = customsInspection.Operation_ID;
                        item.Flight_NO = customsInspection.Flight_NO;
                        item.Flight_Date_Want = customsInspection.Flight_Date_Want;
                        item.MBL = customsInspection.MBL;
                        item.Consign_Code_CK = customsInspection.Consign_Code_CK;
                        item.Book_Flat_Code = customsInspection.Book_Flat_Code;
                        item.Customs_Declaration = customsInspection.Customs_Declaration;
                        item.Num_BG = customsInspection.Num_BG;
                        item.Remarks_BG = customsInspection.Remarks_BG;
                        item.Customs_Broker_BG = customsInspection.Customs_Broker_BG;
                        item.Customs_Date_BG = customsInspection.Customs_Date_BG;
                        item.Pieces_TS = customsInspection.Pieces_TS;
                        item.Weight_TS = customsInspection.Weight_TS;
                        item.Volume_TS = customsInspection.Volume_TS;
                        item.Pieces_Fact = customsInspection.Pieces_Fact;
                        item.Weight_Fact = customsInspection.Weight_Fact;
                        item.Volume_Fact = customsInspection.Volume_Fact;
                        item.Pieces_BG = customsInspection.Pieces_BG;
                        item.Weight_BG = customsInspection.Weight_BG;
                        item.Volume_BG = customsInspection.Volume_BG;
                        item.IS_Checked_BG = customsInspection.IS_Checked_BG;
                        item.Check_QTY = customsInspection.Check_QTY;
                        item.Check_Date = customsInspection.Check_Date;
                        item.Fileupload = customsInspection.Fileupload;
                        item.Status = customsInspection.Status;
                        item.OperatingPoint = customsInspection.OperatingPoint;
                        item.ObjectState = ObjectState.Modified;
                        customsInspection.ObjectState = ObjectState.Modified;
                        _customsInspectionService.Update(item);
                    }
                    var CI = _customsInspectionService.Queryable().Where(x => x.Operation_ID == customsInspection.Operation_ID).ToList();
                    if (CI != null)
                    {
                        foreach (var item in CI)
                        {
                            item.Pieces_BG = customsInspection.Pieces_BG;
                            item.Weight_BG = customsInspection.Weight_BG;
                            item.Volume_BG = customsInspection.Volume_BG;
                            item.IS_Checked_BG = customsInspection.IS_Checked_BG;
                            item.Check_QTY = customsInspection.Check_QTY;
                            item.Check_Date = customsInspection.Check_Date;
                            _customsInspectionService.Update(item);
                        }
                    }
                    var QE = _ops_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == customsInspection.Operation_ID).FirstOrDefault();
                    if (QE != null)
                    {
                        QE.Is_BG = true;
                        _ops_EntrustmentInforService.Update(QE);
                        var ContentE = "报关操作 Operation_Id:" + QE.Operation_Id + ",Is_BG:" + QE.Is_BG;
                        _changeOrderHistoryService.InsertChangeOrdHistory(QE.Id, "OPS_EntrustmentInfor", ChangeOrderHistory.EnumChangeType.Modify, ContentE, 0, 1, 0);
                    }
                }
                _unitOfWork.SaveChanges();

                #region 报关日志

                ContentInsp = "报关操作 " + ContentInsp;
                if (customsInspection.ObjectState == ObjectState.Modified)
                {
                    _changeOrderHistoryService.InsertChangeOrdHistory(customsInspection.Id, "CustomsInspection", ChangeOrderHistory.EnumChangeType.Modify, ContentInsp, 0, 1, 0);
                }
                else 
                {
                    _changeOrderHistoryService.InsertChangeOrdHistory(customsInspection.Id, "CustomsInspection", ChangeOrderHistory.EnumChangeType.Insert, ContentInsp, 0, 0, 1);
                }
                

                #endregion

                #region 结算

                dynamic ORetAudit = new System.Dynamic.ExpandoObject();
                var QOPS_EttInfor = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable().Where(x => !x.Is_TG && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable && x.Operation_Id == customsInspection.Operation_ID).ToList();
                if (QOPS_EttInfor.Any())
                {
                    var OPS_M_Id = QOPS_EttInfor.FirstOrDefault().MBLId;
                    var OOPS_M_Order = _unitOfWork.Repository<OPS_M_Order>().Queryable().Where(x => x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable && x.Id == OPS_M_Id).FirstOrDefault();
                    if (OOPS_M_Order != null && OOPS_M_Order.Id > 0)
                    {
                        OOPS_M_Order.OPS_EntrustmentInfors = QOPS_EttInfor;
                        var ApRetMsg = _costMoneyService.AutoAddFee(OOPS_M_Order, new List<string>() { "BGF", "YDF" });
                        var ArRetMsg = _customerQuotedPriceService.AutoAddFee(OOPS_M_Order, new List<string>() { "BGF", "YDF" });
                        if (!string.IsNullOrWhiteSpace(ApRetMsg) || !string.IsNullOrWhiteSpace(ArRetMsg))
                        {
                            string ErrMsg = "";
                            if (!string.IsNullOrWhiteSpace(ApRetMsg))
                                ErrMsg += " 应付错误：" + ApRetMsg;
                            if (!string.IsNullOrWhiteSpace(ArRetMsg))
                                ErrMsg += " 应收错误：" + ArRetMsg;
                            ORetAudit.success = false;
                            ORetAudit.err = ErrMsg;
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                        }
                    }
                    else
                    {
                        ORetAudit.success = false;
                        ORetAudit.err = "总单数据，已被删除或作废";
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }
                }
                else
                {
                    ORetAudit.success = false;
                    ORetAudit.err = "委托数据，已被删除，作废或退关";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }

                #endregion


                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customsInspection"></param>
        /// <returns></returns>
        public ActionResult SaveCusIndex(CustomsInspection customsInspection)
        {
            try
            {
                if (customsInspection != null)
                {
                    if (customsInspection.Id == 0)
                    {
                        _customsInspectionService.Insert(customsInspection);
                    }
                    else
                    {
                        var item = _customsInspectionService.Find(customsInspection.Id);
                        item.Operation_ID = customsInspection.Operation_ID;
                        item.Flight_NO = customsInspection.Flight_NO;
                        item.Flight_Date_Want = customsInspection.Flight_Date_Want;
                        item.MBL = customsInspection.MBL;
                        item.Consign_Code_CK = customsInspection.Consign_Code_CK;
                        item.Book_Flat_Code = customsInspection.Book_Flat_Code;
                        item.Customs_Declaration = customsInspection.Customs_Declaration;
                        item.Num_BG = customsInspection.Num_BG;
                        item.Remarks_BG = customsInspection.Remarks_BG;
                        item.Customs_Broker_BG = customsInspection.Customs_Broker_BG;
                        item.Customs_Date_BG = customsInspection.Customs_Date_BG;
                        item.Pieces_TS = customsInspection.Pieces_TS;
                        item.Weight_TS = customsInspection.Weight_TS;
                        item.Volume_TS = customsInspection.Volume_TS;
                        item.Pieces_Fact = customsInspection.Pieces_Fact;
                        item.Weight_Fact = customsInspection.Weight_Fact;
                        item.Volume_Fact = customsInspection.Volume_Fact;
                        item.Pieces_BG = customsInspection.Pieces_BG;
                        item.Weight_BG = customsInspection.Weight_BG;
                        item.Volume_BG = customsInspection.Volume_BG;
                        item.IS_Checked_BG = customsInspection.IS_Checked_BG;
                        item.Check_QTY = customsInspection.Check_QTY;
                        item.Check_Date = customsInspection.Check_Date;
                        item.Fileupload = customsInspection.Fileupload;
                        item.Status = customsInspection.Status;
                        item.OperatingPoint = customsInspection.OperatingPoint;
                        item.ObjectState = ObjectState.Modified;
                        _customsInspectionService.Update(item);
                    }
                }
                _unitOfWork.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, err = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: CustomsInspections/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomsInspection customsInspection = _customsInspectionService.Find(id);
            if (customsInspection == null)
            {
                return HttpNotFound();
            }
            return View(customsInspection);
        }

        // GET: CustomsInspections/Create
        public ActionResult Create()
        {
            CustomsInspection customsInspection = new CustomsInspection();
            //set default value
            return View(customsInspection);
        }

        // POST: CustomsInspections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Operation_ID,Flight_NO,Flight_Date_Want,MBL,Consign_Code_CK,Book_Flat_Code,Customs_Declaration,Num_BG,Remarks_BG,Customs_Broker_BG,Customs_Date_BG,Pieces_TS,Weight_TS,Volume_TS,Pieces_Fact,Weight_Fact,Volume_Fact,Pieces_BG,Weight_BG,Volume_BG,IS_Checked_BG,Check_QTY,Check_Date,Fileupload,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] CustomsInspection customsInspection)
        {
            if (ModelState.IsValid)
            {
                _customsInspectionService.Insert(customsInspection);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a CustomsInspection record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(customsInspection);
        }
        
        // GET: CustomsInspections/CusInsIndex
        //public ActionResult CusInsIndex(int? id)
        //{
        //    var customsinspection = _unitOfWork.Repository<CustomsInspection>().Queryable();
        //    var ops_entrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>();
        //    CustomsInspection customsInspection = new CustomsInspection();
        //    var result = from n in ops_entrustmentInforRep.Queryable()
        //                 select new
        //                 {
        //                     n.Id,
        //                     Operation_ID = n.Operation_Id,
        //                     Flight_NO = n.FLIGHT_No,
        //                     Flight_Date_Want = n.Flight_Date_Want,
        //                     MBL = n.MBL,
        //                     Consign_Code_CK = n.Consign_Code,
        //                     Book_Flat_Code = n.Book_Flat_Code,
        //                     Pieces_TS = n.Pieces_TS,
        //                     Weight_TS = n.Weight_TS,
        //                     Volume_TS = n.Volume_TS,
        //                     Pieces_Fact = n.Pieces_Fact,
        //                     Weight_Fact = n.Weight_Fact,
        //                     Volume_Fact = n.Volume_Fact,
        //                 };
        //    var result_data = result.ToList();
        //    return Json(result_data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult getOidDta(string oId) {
        //    int head_Id = 0;
        //    if (!int.TryParse(oId, out head_Id)) {
        //        return Json(new{Success=false,ErrMsg = "assdasdsasd" }, JsonRequestBehavior.AllowGet);
        //    }
        //    var customsinspection = _unitOfWork.Repository<CustomsInspection>().Queryable().FirstOrDefault() ;
        //    var ops_entrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>().Query(x => x.Id == head_Id).Select().FirstOrDefault();
        //    CustomsInspection customsInspection = new CustomsInspection();
        //    //var result = from n in ops_entrustmentInforRep
        //    //             select new
        //    //             {
        //    //                 n.Id,
        //    //                 Operation_ID = n.Operation_Id,
        //    //                 Flight_NO = n.FLIGHT_No,
        //    //                 Flight_Date_Want = n.Flight_Date_Want,
        //    //                 MBL = n.MBL,
        //    //                 Consign_Code_CK = n.Consign_Code,
        //    //                 Book_Flat_Code = n.Book_Flat_Code,
        //    //                 Pieces_TS = n.Pieces_TS,
        //    //                 Weight_TS = n.Weight_TS,
        //    //                 Volume_TS = n.Volume_TS,
        //    //                 Pieces_Fact = n.Pieces_Fact,
        //    //                 Weight_Fact = n.Weight_Fact,
        //    //                 Volume_Fact = n.Volume_Fact,
        //    //             };
        //    if (customsinspection != null) {
        //        if (ops_entrustmentInforRep != null) {
        //            customsinspection.Operation_ID = ops_entrustmentInforRep.Hbl_Feight;
        //        }
        //    }

        //    var result_data = result.ToList();
        //    return Json(result_data, JsonRequestBehavior.AllowGet);
        //}


        // POST: CustomsInspections/CusInsIndex
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CusInsIndex([Bind(Include = "Id,Operation_ID,Flight_NO,Flight_Date_Want,MBL,Consign_Code_CK,Book_Flat_Code,Customs_Declaration,Num_BG,Remarks_BG,Customs_Broker_BG,Customs_Date_BG,Pieces_TS,Weight_TS,Volume_TS,Pieces_Fact,Weight_Fact,Volume_Fact,Pieces_BG,Weight_BG,Volume_BG,IS_Checked_BG,Check_QTY,Check_Date,Fileupload,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] CustomsInspection customsInspection)
        {
            if (ModelState.IsValid)
            {
                _customsInspectionService.Insert(customsInspection);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a CustomsInspection record");
                return RedirectToAction("CusInsIndex");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(customsInspection);
        }

        // GET: CustomsInspections/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomsInspection customsInspection = _customsInspectionService.Find(id);
            if (customsInspection == null)
            {
                return HttpNotFound();
            }
            return View(customsInspection);
        }

        // POST: CustomsInspections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Operation_ID,Flight_NO,Flight_Date_Want,MBL,Consign_Code_CK,Book_Flat_Code,Customs_Declaration,Num_BG,Remarks_BG,Customs_Broker_BG,Customs_Date_BG,Pieces_TS,Weight_TS,Volume_TS,Pieces_Fact,Weight_Fact,Volume_Fact,Pieces_BG,Weight_BG,Volume_BG,IS_Checked_BG,Check_QTY,Check_Date,Fileupload,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] CustomsInspection customsInspection)
        {
            if (ModelState.IsValid)
            {
                customsInspection.ObjectState = ObjectState.Modified;
                _customsInspectionService.Update(customsInspection);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a CustomsInspection record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(customsInspection);
        }

        // GET: CustomsInspections/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomsInspection customsInspection = _customsInspectionService.Find(id);
            if (customsInspection == null)
            {
                return HttpNotFound();
            }
            return View(customsInspection);
        }

        // POST: CustomsInspections/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomsInspection customsInspection = _customsInspectionService.Find(id);
            _customsInspectionService.Delete(customsInspection);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a CustomsInspection record");
            return RedirectToAction("Index");
        }

        //删除保存
        [HttpPost]
        public ActionResult SaveDelete(Array ids)
        {
            var strids = "";
            var delete = "0";
            foreach (var item in ids)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var ArrDelt = new List<CustomsInspection>();
            foreach (var id in idarr)
            {
                CustomsInspection customsInspection = _customsInspectionService.Find(Int32.Parse(id));
                delete = "1";
                customsInspection.ObjectState = ObjectState.Deleted;
                _customsInspectionService.Delete(customsInspection);

                ArrDelt.Add(customsInspection);

                var QE = _ops_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == customsInspection.Operation_ID);
                var Qcus = _customsInspectionService.Queryable().Where(x => x.Operation_ID == customsInspection.Operation_ID);

                if (QE.Count()>0 && Qcus.Count() == 1)
                {
                    var QQE = QE.FirstOrDefault();
                    QQE.Is_BG = false;
                    _ops_EntrustmentInforService.Update(QQE);
                }

                var ContentInsp = "报关操作 Operation_Id:" + customsInspection.Operation_ID + ",Customs_Declaration:" + customsInspection.Customs_Declaration;
                if (customsInspection.ObjectState == ObjectState.Deleted)
                {
                    _changeOrderHistoryService.InsertChangeOrdHistory(customsInspection.Id, "CustomsInspection", ChangeOrderHistory.EnumChangeType.Delete, ContentInsp, 1, 0, 0);
                }
            }
            try
            {
                if (delete == "1")
                {
                    foreach (var id in idarr)
                    {
                        _unitOfWork.SaveChanges();
                        var ChangeViewModel = new CustomsInspectionChangeViewModel();
                        ChangeViewModel.deleted = ArrDelt;

                        //自动更新 缓存
                        //if (IsAutoResetCache)
                        //    AutoResetCache(ChangeViewModel);
                        //return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }
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

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "customsinspection_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _customsInspectionService.ExportExcel(filterRules, sort, order);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OPS_ENid"></param>
        /// <returns></returns>
        public ActionResult CusIndex(string OPS_ENid)
        {
            int head_Id = 0;
            if (!int.TryParse(OPS_ENid, out head_Id))
            {
                return View(new CustomsInspection());
            }
            if (head_Id <= 0)
            {
                return View(new CustomsInspection());
            }
            var ops_entrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>().Query(x => x.Id == head_Id).Select().FirstOrDefault();
            if (ops_entrustmentInforRep == null)
            {
                return View(new CustomsInspection());
            }
            ViewBag.OPS_ENid = head_Id;
            ViewBag.MBLId = ops_entrustmentInforRep.MBLId;
            var customsinspection = new CustomsInspection();

            var customsinspections = _customsInspectionService.Queryable().Where(x => x.Operation_ID == ops_entrustmentInforRep.Operation_Id).OrderByDescending(x => x.Id).FirstOrDefault();
            if (customsinspections != null)
            {
                customsinspection = customsinspections;
                customsinspection.Operation_ID = ops_entrustmentInforRep.Operation_Id;
                customsinspection.Flight_NO = ops_entrustmentInforRep.Flight_No;
                customsinspection.Flight_Date_Want = ops_entrustmentInforRep.Flight_Date_Want;
                customsinspection.MBL = ops_entrustmentInforRep.MBL;
                customsinspection.Consign_Code_CK = ops_entrustmentInforRep.Entrustment_Name;
                customsinspection.Book_Flat_Code = ops_entrustmentInforRep.Book_Flat_Code;
                customsinspection.Pieces_TS = ops_entrustmentInforRep.Pieces_TS;
                customsinspection.Weight_TS = ops_entrustmentInforRep.Weight_TS;
                customsinspection.Volume_TS = ops_entrustmentInforRep.Volume_TS;
                customsinspection.Pieces_Fact = ops_entrustmentInforRep.Pieces_Fact;
                customsinspection.Weight_Fact = ops_entrustmentInforRep.Weight_Fact;
                customsinspection.Volume_Fact = ops_entrustmentInforRep.Volume_Fact;
                //customsinspection.Customs_Declaration = null;
                //customsinspection.Num_BG = null;
                //customsinspection.Remarks_BG = null;
                //customsinspection.Customs_Broker_BG = null;
                //customsinspection.Customs_Date_BG = null;
                //customsinspection.Pieces_BG = null;
                //customsinspection.Weight_BG = null;
                //customsinspection.Volume_BG = null;
                //customsinspection.IS_Checked_BG = false;
                //customsinspection.Check_QTY = null;
                //customsinspection.Check_Date = null;
                //customsinspection.Fileupload = null;
                //customsinspection.Customs_Date_BG = DateTime.Now;
                //if (customsinspection.Customs_Date_BG == null)
                //{
                //    customsinspection.Customs_Date_BG = Convert.ToDateTime("1900-01-01"); 
                //}

                //if (string.IsNullOrEmpty(customsinspection.Customs_Broker_BG))
                //{
                //    customsinspection.Customs_Broker_BG = "QPBGH01";
                //}
                var picture = _pictureService.Queryable().Where(x => x.Code.Equals(customsinspection.Operation_ID) && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).ToList();
                if (picture == null)
                {
                    ViewBag.picture = null;
                }
                else
                {
                    foreach (var item in picture)
                    {
                        item.OperatingPoint = (int)item.Type;
                    }
                    ViewBag.picture = picture;
                }
            }
            else
            {
                customsinspection.Operation_ID = ops_entrustmentInforRep.Operation_Id;
                customsinspection.Flight_NO = ops_entrustmentInforRep.Flight_No;
                customsinspection.Flight_Date_Want = ops_entrustmentInforRep.Flight_Date_Want;
                customsinspection.MBL = ops_entrustmentInforRep.MBL;
                customsinspection.Consign_Code_CK = ops_entrustmentInforRep.Entrustment_Name;
                customsinspection.Book_Flat_Code = ops_entrustmentInforRep.Book_Flat_Code;
                customsinspection.Pieces_TS = ops_entrustmentInforRep.Pieces_TS;
                customsinspection.Weight_TS = ops_entrustmentInforRep.Weight_TS;
                customsinspection.Volume_TS = ops_entrustmentInforRep.Volume_TS;
                customsinspection.Pieces_Fact = ops_entrustmentInforRep.Pieces_Fact;
                customsinspection.Weight_Fact = ops_entrustmentInforRep.Weight_Fact;
                customsinspection.Volume_Fact = ops_entrustmentInforRep.Volume_Fact;
                customsinspection.Customs_Declaration = null;
                customsinspection.Num_BG = null;
                customsinspection.Remarks_BG = null;
                customsinspection.Customs_Broker_BG = "";// "QPBGH01";
                customsinspection.Customs_Date_BG = DateTime.Now;
                customsinspection.Pieces_BG = null;
                customsinspection.Weight_BG = null;
                customsinspection.Volume_BG = null;
                customsinspection.IS_Checked_BG = false;
                customsinspection.Check_QTY = null;
                customsinspection.Check_Date = null;
                customsinspection.Fileupload = null;
                ViewBag.picture = null;
            }

            var ODynamic = GetFromNAME(customsinspection, 0);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);

            return View(customsinspection);
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(CustomsInspection customsinspection, int index = 0)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();

            //客商信息
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            if (!string.IsNullOrEmpty(customsinspection.Customs_Broker_BG))
            {//航空公司
                var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == customsinspection.Customs_Broker_BG).FirstOrDefault();
                ODynamic.Customs_Broker_BGNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
            }
            if (!string.IsNullOrEmpty(customsinspection.Consign_Code_CK))
            {//订舱方
                var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == customsinspection.Consign_Code_CK).FirstOrDefault();
                ODynamic.Consign_Code_CKNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
            }
            if (!string.IsNullOrEmpty(customsinspection.Book_Flat_Code))
            {//委托方
                var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == customsinspection.Book_Flat_Code).FirstOrDefault();
                ODynamic.Book_Flat_CodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
            }

            return ODynamic;
        }
    }
}
