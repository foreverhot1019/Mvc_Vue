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
    public class DocumentManagementsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<DocumentManagement>, Repository<DocumentManagement>>();
        //container.RegisterType<IDocumentManagementService, DocumentManagementService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IDocumentManagementService  _documentManagementService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ICustomsInspectionService _customsInspectionService;
        private readonly ICostMoneyService _costMoneyService;
        private readonly ICustomerQuotedPriceService _customerQuotedPriceService;
        private readonly IOPS_EntrustmentInforService _oPS_EntrustmentInforService;

        //验证权限的名称
        private string ControllerQXName = "/OPS_M_Orders";

        public DocumentManagementsController(IDocumentManagementService documentManagementService, IOPS_EntrustmentInforService oPS_EntrustmentInforService, IUnitOfWorkAsync unitOfWork, ICustomsInspectionService customsInspectionService)
        {
            _documentManagementService  = documentManagementService;
            _unitOfWork = unitOfWork;
            _customsInspectionService = customsInspectionService;
            _costMoneyService = (ICostMoneyService)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(CostMoneyService), "CostMoneyService");
            _customerQuotedPriceService = (ICustomerQuotedPriceService)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(CustomerQuotedPriceService), "CustomerQuotedPriceService");
            _oPS_EntrustmentInforService = oPS_EntrustmentInforService;

        }

        // GET: DocumentManagements/Index
        public ActionResult Index(string id = "0", string Operation_ID = "")
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
              
                int head_Id = 0;
                if (!int.TryParse(id, out head_Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (head_Id > 0) { 
                    var CusInsData = _unitOfWork.Repository<CustomsInspection>().Query(x => x.Id == head_Id).Select().FirstOrDefault();
                    if (CusInsData != null)
                    {
                        var CusInsoperation_id = CusInsData.Operation_ID;
                        var ops_entrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>().Query(x => x.Operation_Id == CusInsoperation_id).Select().FirstOrDefault();
                        var OPS_ENid = ops_entrustmentInforRep.Id;
                        ViewBag.CusInsData = CusInsData;
                        ViewBag.OPS_ENid = OPS_ENid;
                        ViewBag.Entrustment_Code = ops_entrustmentInforRep.Entrustment_Code;
                        ViewBag.OPS_Mid = ops_entrustmentInforRep.MBLId;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Operation_ID))
                        {
                            var ops_entrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>().Query(x => x.Operation_Id == Operation_ID).Select().FirstOrDefault();
                            if (ops_entrustmentInforRep != null)
                            {
                                ViewBag.CusInsData = new CustomsInspection();
                                ViewBag.OPS_ENid = ops_entrustmentInforRep.Id;
                                ViewBag.Entrustment_Code = ops_entrustmentInforRep.Entrustment_Code;
                                ViewBag.OPS_Mid = ops_entrustmentInforRep.MBLId;
                                return HttpNotFound();
                            }
                        }
                    }
                }
                else
                {
                    if (Operation_ID == null || Operation_ID == "")
                    {
                        ViewBag.CusInsData = new CustomsInspection();
                        ViewBag.OPS_ENid = -1;
                        ViewBag.Entrustment_Code = "";
                        ViewBag.OPS_Mid = -1;
                        return HttpNotFound();
                    }
                    var CusInsData = _unitOfWork.Repository<CustomsInspection>().Query(x => x.Operation_ID == Operation_ID).Select().FirstOrDefault();
                    if (CusInsData != null)
                    {
                        var CusInsoperation_id = CusInsData.Operation_ID;
                        var ops_entrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>().Query(x => x.Operation_Id == CusInsoperation_id).Select().FirstOrDefault();
                        var OPS_ENid = ops_entrustmentInforRep.Id;
                        ViewBag.CusInsData = CusInsData;
                        ViewBag.OPS_ENid = OPS_ENid;
                        ViewBag.Entrustment_Code = ops_entrustmentInforRep.Entrustment_Code;
                        ViewBag.OPS_Mid = ops_entrustmentInforRep.MBLId;
                    }
                    else
                    {
                        var customsinspection = new CustomsInspection();
                        var ops_entrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>().Query(x => x.Operation_Id == Operation_ID).Select().FirstOrDefault();
                        if (ops_entrustmentInforRep != null)
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
                            customsinspection.Customs_Broker_BG = null;
                            customsinspection.Customs_Date_BG = null;
                            customsinspection.Pieces_BG = null;
                            customsinspection.Weight_BG = null;
                            customsinspection.Volume_BG = null;
                            customsinspection.IS_Checked_BG = false;
                            customsinspection.Check_QTY = null;
                            customsinspection.Check_Date = null;
                            customsinspection.Fileupload = null;

                            ViewBag.OPS_ENid = ops_entrustmentInforRep.Id;
                            ViewBag.Entrustment_Code = ops_entrustmentInforRep.Entrustment_Code;
                            ViewBag.OPS_Mid = ops_entrustmentInforRep.MBLId;
                        }
                        else
                        {
                            ViewBag.OPS_ENid = -1;
                            ViewBag.Entrustment_Code = "";
                            ViewBag.OPS_Mid = -1;
                        }
                        ViewBag.CusInsData = customsinspection;
                    }
                }
			return View();
        }


        /// <summary>
        /// 退单登记
        /// </summary>
        /// <param name="id">总单ID</param>
        /// <returns></returns>
        public ActionResult Edit_Refund_Register(int id = 0)
        {
            ViewBag.MBLId = id;
            return View();
        }


        /// <summary>
        /// 退客户登记
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit_Return_Customer()
        {
            return View();
        }

        /// <summary>
        /// 签收单打印
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit_PrintSignReceipt()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveAppend(DocumentManagement documentManagement) 
        {
            try
            {
                if (documentManagement != null)
                {
                    if (documentManagement.Id == 0)
                    {
                        _documentManagementService.Insert(documentManagement);
                    }
                    else
                    {
                        var item = _documentManagementService.Find(documentManagement.Id);
                        item.Operation_ID = documentManagement.Operation_ID;
                        item.DZ_Type = documentManagement.DZ_Type;
                        item.Doc_NO = documentManagement.Doc_NO;
                        item.Trade_Mode = documentManagement.Trade_Mode;
                        item.Return_Print = documentManagement.Return_Print;
                        item.QTY = documentManagement.QTY;
                        item.Ping_Name = documentManagement.Ping_Name;
                        item.BG_TT = documentManagement.BG_TT;
                        item.Print_Date = documentManagement.Print_Date;
                        item.Flight_Date_Want = documentManagement.Flight_Date_Want;
                        item.Entrustment_Name = documentManagement.Entrustment_Name;
                        item.Entrustment_Code = documentManagement.Entrustment_Code;
                        item.MBL = documentManagement.MBL;
                        item.ObjectState = ObjectState.Modified;
                        _documentManagementService.Update(item);
                    }
                }
                _unitOfWork.SaveChanges();

                #region 结算

                dynamic ORetAudit = new System.Dynamic.ExpandoObject();
                var QOPS_EttInfor = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable().Where(x =>!x.Is_TG && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable && x.Operation_Id == documentManagement.Operation_ID).ToList();
                if (QOPS_EttInfor.Any())
                {
                    var OPS_M_Id = QOPS_EttInfor.FirstOrDefault().MBLId;
                    var OOPS_M_Order = _unitOfWork.Repository<OPS_M_Order>().Queryable().Where(x => x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable &&  x.Id == OPS_M_Id).FirstOrDefault();
                    if (OOPS_M_Order != null && OOPS_M_Order.Id > 0)
                    {
                        OOPS_M_Order.OPS_EntrustmentInfors = QOPS_EttInfor;
                        var ApRetMsg = _costMoneyService.AutoAddFee(OOPS_M_Order, new List<string>() { "LDF"});
                        var ArRetMsg = _customerQuotedPriceService.AutoAddFee(OOPS_M_Order, new List<string>() { "LDF" });
                        if (!string.IsNullOrWhiteSpace(ApRetMsg) || !string.IsNullOrWhiteSpace(ArRetMsg))
                        {
                            string ErrMsg = "";
                            if (!string.IsNullOrWhiteSpace(ApRetMsg))
                                ErrMsg += " 应付错误：" + ApRetMsg;
                            if (!string.IsNullOrWhiteSpace(ArRetMsg))
                                ErrMsg += " 应收错误：" + ArRetMsg;
                            ORetAudit.Success = false;
                            ORetAudit.ErrMsg = ErrMsg;
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                        }
                    }
                    else
                    {
                        ORetAudit.Success = false;
                        ORetAudit.ErrMsg = "总单数据，已被删除或作废";
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }
                }
                else
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "委托数据，已被删除，作废或退关";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }

                #endregion

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, er = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Get :DocumentManagements/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(string operation_id,int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules) ?? new List<filterRule>();
            var ArrFilter = new List<filterRule>();
            if (!filters.Any(x => x.field == "Operation_ID") && operation_id != "r")
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
			var documentmanagement = _documentManagementService.Query(new DocumentManagementQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            var datarows = documentmanagement.Select( n => new {  
                              Id = n.Id, 
                              Operation_ID = n.Operation_ID, 
                              DZ_Type = n.DZ_Type, 
                              Doc_NO = n.Doc_NO, 
                              Trade_Mode = n.Trade_Mode, 
                              Return_Print = n.Return_Print, 
                              QTY = n.QTY, 
                              BG_TT = n.BG_TT, 
                              Ping_Name = n.Ping_Name, 
                              Return_Date = n.Return_Date, 
                              Print_Date = n.Print_Date,
                              Print_Type = (n.Print_Date == null ? "未打印":"已打印"),
                              Return_Customer_Date = n.Return_Customer_Date,
                              MBL = n.MBL,
                              Entrustment_Code = n.Entrustment_Code,
                              Entrustment_Name = n.Entrustment_Name,
                              Flight_Date_Want = n.Flight_Date_Want,
                              Is_Return = n.Is_Return,
                              ReturnID = n.ReturnID,
                              ReturnWHO = n.ReturnWHO,
                              Is_Return_Customer = n.Is_Return_Customer,
                              Return_CustomerID = n.Return_CustomerID,
                              Return_CustomerWHO = n.Return_CustomerWHO,
                              Is_Print = n.Is_Print,
                              SignReceipt_Code = n.SignReceipt_Code,
                              Remark = n.Remark, 
                              Status = n.Status, 
                              OperatingPoint = n.OperatingPoint, 
                              ADDWHO = n.ADDWHO, 
                              ADDTS = n.ADDTS, 
                              EDITWHO = n.EDITWHO, 
                              EDITTS = n.EDITTS}).ToList();
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var allrows = from n in datarows
                       join x in ArrCusBusInfo on n.Entrustment_Name equals x.EnterpriseId into x_tep
                       from xtmp in x_tep.DefaultIfEmpty()
                       select new
                       {
                           Id = n.Id,
                           Operation_ID = n.Operation_ID,
                           DZ_Type = n.DZ_Type,
                           Doc_NO = n.Doc_NO,
                           Trade_Mode = n.Trade_Mode,
                           Return_Print = n.Return_Print,
                           QTY = n.QTY,
                           BG_TT = n.BG_TT,
                           Ping_Name = n.Ping_Name,
                           Return_Date = n.Return_Date,
                           Print_Date = n.Print_Date,
                           Print_Type = (n.Print_Date == null ? "未打印" : "已打印"),
                           Return_Customer_Date = n.Return_Customer_Date,
                           MBL = n.MBL,
                           Entrustment_Code = n.Entrustment_Code,
                           Entrustment_Name = n.Entrustment_Name,
                           Entrustment_NameName = xtmp == null ? "" : xtmp.EnterpriseShortName,
                           Flight_Date_Want = n.Flight_Date_Want,
                           Is_Return = n.Is_Return,
                           ReturnID = n.ReturnID,
                           ReturnWHO = n.ReturnWHO,
                           Is_Return_Customer = n.Is_Return_Customer,
                           Return_CustomerID = n.Return_CustomerID,
                           Return_CustomerWHO = n.Return_CustomerWHO,
                           Is_Print = n.Is_Print,
                           SignReceipt_Code = n.SignReceipt_Code,
                           Remark = n.Remark,
                           Status = n.Status,
                           OperatingPoint = n.OperatingPoint,
                           ADDWHO = n.ADDWHO,
                           ADDTS = n.ADDTS,
                           EDITWHO = n.EDITWHO,
                           EDITTS = n.EDITTS
                       };
            var pagelist = new { total = totalCount, rows = allrows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(DocumentManagementChangeViewModel documentmanagement)
        {
            if (documentmanagement.updated != null)
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

                foreach (var updated in documentmanagement.updated)
                {
                    _documentManagementService.Update(updated);
                }
            }
            if (documentmanagement.deleted != null)
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

                foreach (var deleted in documentmanagement.deleted)
                {
                    _documentManagementService.Delete(deleted);
                }
            }
            if (documentmanagement.inserted != null)
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

                foreach (var inserted in documentmanagement.inserted)
                {
                    _documentManagementService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((documentmanagement.updated != null && documentmanagement.updated.Any()) || 
				(documentmanagement.deleted != null && documentmanagement.deleted.Any()) || 
				(documentmanagement.inserted != null && documentmanagement.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(documentmanagement);
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
		       
        // GET: DocumentManagements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentManagement documentManagement = _documentManagementService.Find(id);
            if (documentManagement == null)
            {
                return HttpNotFound();
            }
            return View(documentManagement);
        }

        // GET: DocumentManagements/Create
        public ActionResult Create()
        {
            DocumentManagement documentManagement = new DocumentManagement();
            //set default value
            return View(documentManagement);
        }

        // POST: DocumentManagements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Operation_ID,DZ_Type,Doc_NO,Trade_Mode,Return_Print,QTY,BG_TT,Ping_Name,Return_Date,Print_Date,Return_Customer_Date,Remark,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] DocumentManagement documentManagement)
        {
            if (ModelState.IsValid)
            {
				_documentManagementService.Insert(documentManagement);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a DocumentManagement record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(documentManagement);
        }

        // GET: DocumentManagements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentManagement documentManagement = _documentManagementService.Find(id);
            if (documentManagement == null)
            {
                return HttpNotFound();
            }
            return View(documentManagement);
        }

        // POST: DocumentManagements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Operation_ID,DZ_Type,Doc_NO,Trade_Mode,Return_Print,QTY,BG_TT,Ping_Name,Return_Date,Print_Date,Return_Customer_Date,Remark,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] DocumentManagement documentManagement)
        {
            if (ModelState.IsValid)
            {
				documentManagement.ObjectState = ObjectState.Modified;
				_documentManagementService.Update(documentManagement);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a DocumentManagement record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(documentManagement);
        }

        // GET: DocumentManagements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentManagement documentManagement = _documentManagementService.Find(id);
            if (documentManagement == null)
            {
                return HttpNotFound();
            }
            return View(documentManagement);
        }

        // POST: DocumentManagements/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocumentManagement documentManagement = _documentManagementService.Find(id);
            _documentManagementService.Delete(documentManagement);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a DocumentManagement record");
            return RedirectToAction("Index");
        }


		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _documentManagementService.ExportExcel(filterRules,sort, order );
            return File(stream, "application/vnd.ms-excel", fileName);
        }


        //导出Excel
        [HttpPost]
        public ActionResult ExportSelectedExcel(Array ArrId = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var strids = "";
            //foreach (var item in filterRules)
            //{
            //    strids = item.ToString();
            //}
            var idarr = strids.Split(",");
            var data = _documentManagementService.Queryable().Where(x => idarr.Contains(x.Doc_NO)).ToList();

            var datarows = data.Select((n, idx) => new
            {
                Id = idx+1,
                Doc_NO = n.Doc_NO,
                MBL = n.MBL,
                Ping_Name = n.Ping_Name,
                BG_TT = n.BG_TT,
                Flight_Date_Want = n.Flight_Date_Want,
                Entrustment_Name = n.Entrustment_Name,
                Return_Date = n.Return_Date,
                Return_Customer_Date = n.Return_Customer_Date,
                Operation_ID = n.Operation_ID,
                SignReceipt_Code = n.SignReceipt_Code,
                Trade_Mode = n.Trade_Mode,
                ReturnWHO = n.ReturnWHO,
                Entrustment_Code = n.Entrustment_Code
            }).ToList();

            var stream = ExcelHelper.ExportExcel(typeof(DocumentManagement), datarows, "ExportDocumentManagementList"); ;
            return File(stream, "application/vnd.ms-excel", fileName);
        }


        public ActionResult ExportReceiptNotes(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "" ,string UserCode = "")
        {
            try
            {
                List<int> ArrId = new List<int>();
                var strids = "";
                if (typeName == "签收单打印") {
                    foreach (var item in filterRules)
                    {
                        ArrId.AddRange(Convert.ToInt32(item));
                    }
                }              
                else{
                    foreach (var item in filterRules)
                    {
                        strids = item.ToString();
                    }
                    var idarr = strids.Split(",");
                    foreach (var item in idarr)
                    {
                        ArrId.AddRange(Convert.ToInt32(item));
                    }

                }
                var data = _documentManagementService.Queryable().Where(x => ArrId.Contains(x.Id)).ToList();

                if (data == null || data.Count == 0)
                {
                    return null;
                }
                var Qcus = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();

                var rowsdata = data.Select(x => new
                {
                    Dzbh = x.Operation_ID,
                    Doc_No = x.Doc_NO,
                }).ToList();

                List<ExceptNotes> ExceptNotesList = new List<ExceptNotes>();
                foreach (var item in rowsdata)
                {
                    ExceptNotes t = new ExceptNotes();
                    t.Dzbh = item.Dzbh;
                    t.Doc_No = item.Doc_No;
                    var Qop_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).ToList();
                    if (Qop_en.Count() == 0)
                    {
                        return Json(new { Success = false, ErrMsg = "选中的信息中包含业务编号不存在的信息" }, JsonRequestBehavior.AllowGet);
                    }
                    if (Qop_en.Any())
                    {
                        t.Entrustment_Code = Qop_en.FirstOrDefault().Entrustment_Code;
                        t.Flight_Date_Want = Convert.ToDateTime(Qop_en.FirstOrDefault().Flight_Date_Want).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        t.Entrustment_Code = null;
                        t.Flight_Date_Want = null;
                    }
                    var cusIn = _customsInspectionService.Queryable().Where(x => x.Operation_ID == item.Dzbh).ToList();
                    if (cusIn.Any())
                    {
                        t.Remark = cusIn.FirstOrDefault().Remarks_BG;
                    }
                    ExceptNotesList.AddRange(t);
                }

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                dynamic ORetAudit = new System.Dynamic.ExpandoObject();

                var SignReceipt_Code_No = SequenceBuilder.NextSignReceipt_Code_No();
                if (data.Any())
                {
                    foreach (var item in data)
                    {
                        System.Data.Entity.Infrastructure.DbEntityEntry<DocumentManagement> entry = WebdbContxt.Entry<DocumentManagement>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        //var IsModified = entry.Property(t => t.SignReceipt_Code).IsModified;
                        item.SignReceipt_Code = SignReceipt_Code_No;
                        entry.Property(t => t.SignReceipt_Code).IsModified = true; //设置要更新的属性  
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                    }
                    WebdbContxt.SaveChanges();
                }

                string outfile = "";
                var outbytes = ExcelHelper.OutPutExcelByArr(ExceptNotesList.ToList(), @"\FileModel\签收单.xlsx", 3, 0, out outfile, "签收单");
                var ms = new System.IO.MemoryStream(outbytes);
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
                Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                sheet.Cells[2, 3].Value = "编号：" + SignReceipt_Code_No;
                var ArrDzbh = ExceptNotesList.Select(x => x.Dzbh).ToList();
                var TOEntrustmentShortName = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDzbh.Contains(x.Operation_Id)).ToList();
                if (TOEntrustmentShortName.Count() == 0)
                {
                    return Json(new { Success = false, ErrMsg = "选中的信息中包含业务编号不存在的信息" }, JsonRequestBehavior.AllowGet);
                }
                var QEn_Name = Qcus.Where(x => x.EnterpriseId == TOEntrustmentShortName.FirstOrDefault().Entrustment_Name).ToList();
                if (QEn_Name.Any())
                {
                    sheet.Cells[2, 0].Value = "TO：" + QEn_Name.FirstOrDefault().EnterpriseShortName;
                }
                else
                    sheet.Cells[2, 0].Value = "TO：" + TOEntrustmentShortName.FirstOrDefault().Entrustment_Name;
                var Counts = ExceptNotesList.Count();
                sheet.Cells[Counts + 4, 1].Value = "客户签名：";
                var QUserName = ArrAppUser.Where(x => x.UserName == UserCode).ToList();
                if (QUserName.Any())
                {
                    sheet.Cells[Counts + 4, 3].Value = "打印人：" + QUserName.FirstOrDefault().UserNameDesc;
                }
                else
                    sheet.Cells[Counts + 4, 3].Value = "打印人：";
                sheet.Cells[Counts + 5, 3].Value = "打印日期：" + DateTime.Now.ToString("yyyy-MM-dd");
                sheet.Cells.Merge(Counts + 6, 1, 1, 4);
                Aspose.Cells.Style newstyle = workbook.Styles[workbook.Styles.Add()];//新增样式
                newstyle.Font.Size = 18;
                sheet.Cells[Counts + 6, 1].Value = "收到后请务必签名回传 Fax：021-63306913/15";
                sheet.Cells[Counts + 6, 1].SetStyle(newstyle);
                for (var i = 0; i < Counts + 4; i++)
                {
                    sheet.Cells.Rows[i + 2].Height = 16;
                }
                sheet.AutoFitRow(Counts + 6);
                if (typeName == "签收单打印")
                {
                    sheet.Cells.Columns[0].Width = 16;
                    sheet.Cells.Columns[1].Width = 15;
                    sheet.Cells.Columns[2].Width = 18;
                    sheet.Cells.Columns[3].Width = 12;
                    sheet.Cells.Columns[4].Width = 14;
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    string FName = Server.MapPath(dPath + "/pdfDD/" + fileName);
                    //var newms = Server.MapPath(@"\DownLoad\ReceiptForm.xls");
                    workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
                    //ORetAudit.Success = true;
                    //ORetAudit.fileName = fileName;
                    //ORetAudit.SignReceipt_Code_No = SignReceipt_Code_No;
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                    //return File(newms, "application/vnd.ms-excel", fileName);
                }
                else {
                    sheet.Cells.Columns[0].Width = 20;
                    sheet.Cells.Columns[1].Width = 14;
                    sheet.Cells.Columns[2].Width = 20;
                    sheet.Cells.Columns[3].Width = 14;
                    sheet.Cells.Columns[4].Width = 14;
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    //string FName = Server.MapPath(dPath + "/pdfDD/" + fileName);
                    var newms = Server.MapPath(@"\DownLoad\ReceiptForm.xls");
                    workbook.Save(newms);
                    //ORetAudit.Success = true;
                    //ORetAudit.fileName = fileName;
                    //ORetAudit.SignReceipt_Code_No = SignReceipt_Code_No;
                    //return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                    return File(newms, "application/vnd.ms-excel", fileName);
                }
               
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
            
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

        public class ExceptNotes
        {           
            public string Doc_No { get; set; }
            public string Dzbh { get; set; }
            public string Entrustment_Code { get; set; }
            public string Flight_Date_Want { get; set; }
            public string Remark { get; set; }
        }
    }
}
