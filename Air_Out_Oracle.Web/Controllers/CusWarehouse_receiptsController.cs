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
    //[Authorize(Roles = "外部客户管理,外客户,管理员")]
    public class CusWarehouse_receiptsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Warehouse_receipt>, Repository<Warehouse_receipt>>();
        //container.RegisterType<IWarehouse_receiptService, Warehouse_receiptService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IWarehouse_receiptService _warehouse_receiptService;
        private readonly IWarehouse_Cargo_SizeService _warehouse_Cargo_SizeService;
        private readonly IPictureService _pictureService;
        private readonly IPARA_PackageService _para_PackageService;
        private readonly IOPS_EntrustmentInforService _oPS_EntrustmentInforService;
        private readonly IChangeOrderHistoryService _changeOrderHistoryService;
        private readonly IPortalEntryIDLinkService _portalEntryIDLinkService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/Warehouse_receipts";

        public CusWarehouse_receiptsController(IPortalEntryIDLinkService portalEntryIDLinkService,IWarehouse_receiptService warehouse_receiptService, IWarehouse_Cargo_SizeService warehouse_Cargo_SizeService, IPictureService pictureService, IPARA_PackageService para_PackageService, IOPS_EntrustmentInforService oPS_EntrustmentInforService, IChangeOrderHistoryService changeOrderHistoryService, IUnitOfWorkAsync unitOfWork)
        {
            _warehouse_receiptService = warehouse_receiptService;
            _warehouse_Cargo_SizeService = warehouse_Cargo_SizeService;
            _pictureService = pictureService;
            _para_PackageService = para_PackageService;
            _oPS_EntrustmentInforService = oPS_EntrustmentInforService;
            _changeOrderHistoryService = changeOrderHistoryService;
            _portalEntryIDLinkService = portalEntryIDLinkService;
            _unitOfWork = unitOfWork;
        }

        // GET: Warehouse_receipts/Index
        public ActionResult Index()
        {
            //var warehouse_receipt  = _warehouse_receiptService.Queryable().AsQueryable();
            //return View(warehouse_receipt  );
            return View();
        }

        // Get :Warehouse_receipts/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            WebdbContext OWebdbCtxt = new WebdbContext();
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var QPortalEntryIDLinkRep = _portalEntryIDLinkService.Queryable().ToList();
            var warehouse_receipt = _warehouse_receiptService.Query(new Warehouse_receiptQuery().Withfilter(filters)).Select().AsQueryable();   
      
           //.OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);

            //var data = warehouse_receipt.Select(n => new
            //{
            //    Id = n.Id,
            //    Warehouse_Id = n.Warehouse_Id,
            //    Entry_Id = n.Entry_Id,
            //    Warehouse_Code = n.Warehouse_Code,
            //    Packing = n.Packing,
            //    Pieces_CK = n.Pieces_CK,
            //    Weight_CK = n.Weight_CK,
            //    Volume_CK = n.Volume_CK,
            //    CHARGE_WEIGHT_CK = n.CHARGE_WEIGHT_CK,
            //    Bulk_Weight_CK = n.Bulk_Weight_CK,
            //    Damaged_CK = n.Damaged_CK,
            //    Damaged_Num = n.Damaged_Num,
            //    Dampness_CK = n.Dampness_CK,
            //    Dampness_Num = n.Dampness_Num,
            //    Deformation = n.Deformation,
            //    Deformation_Num = n.Deformation_Num,
            //    Is_GF = n.Is_GF,
            //    Closure_Remark = n.Closure_Remark,
            //    Is_QG = n.Is_QG,
            //    Warehouse_Remark = n.Warehouse_Remark,
            //    Consign_Code_CK = n.Consign_Code_CK,
            //    MBL = n.MBL,
            //    HBL = n.HBL,
            //    Flight_Date_Want = n.Flight_Date_Want,
            //    FLIGHT_No = n.FLIGHT_No,
            //    End_Port = n.End_Port,
            //    In_Date = n.In_Date,
            //    In_Time = n.In_Time,
            //    Out_Date = n.Out_Date,
            //    Out_Time = n.Out_Time,
            //    CH_Name_CK = n.CH_Name_CK,
            //    Is_CustomerReturn = n.Is_CustomerReturn,
            //    Is_MyReturn = n.Is_MyReturn,
            //    Truck_Id = n.Truck_Id,
            //    Driver = n.Driver,
            //    Is_DamageUpload = n.Is_DamageUpload,
            //    Is_DeliveryUpload = n.Is_DeliveryUpload,
            //    Is_EntryUpload = n.Is_EntryUpload,
            //    Status = n.Status,
            //    Is_Binding = n.Is_Binding,
            //    Remark = n.Remark,
            //    OperatingPoint = n.OperatingPoint
            //}).ToList();

            var pARA_Package = _unitOfWork.RepositoryAsync<PARA_Package>().Queryable();
            var bD_DEFDOC_LIST = _unitOfWork.RepositoryAsync<BD_DEFDOC_LIST>().Queryable().Where(x => x.DOCCODE == "WAREHOUSE");
            List<Warehouse_receipt> NEWLIST = new List<Warehouse_receipt>();
            List<Warehouse_receipt> FINALLIST = new List<Warehouse_receipt>();
            

            var datarows = from n in warehouse_receipt
                           join p in pARA_Package on n.Packing equals p.PackageCode into p_tmp
                           from ptmp in p_tmp.DefaultIfEmpty()
                           join b in bD_DEFDOC_LIST on n.Warehouse_Code equals b.LISTCODE into b_tmp
                           from btmp in b_tmp.DefaultIfEmpty()
                           select new
                           {
                               Id = n.Id,
                               Warehouse_Id = n.Warehouse_Id,
                               Entry_Id = n.Entry_Id,
                               Warehouse_Code = n.Warehouse_Code,
                               Warehouse_CodeName = btmp == null ? null : btmp.LISTNAME,
                               Pieces_CK = n.Pieces_CK,
                               Weight_CK = n.Weight_CK,
                               Volume_CK = n.Volume_CK,
                               Packing = n.Packing,
                               PackingName = ptmp == null ? null : ptmp.PackageName,
                               CHARGE_WEIGHT_CK = n.CHARGE_WEIGHT_CK,
                               Bulk_Weight_CK = n.Bulk_Weight_CK,
                               Damaged_CK = n.Damaged_CK,
                               Damaged_Num = n.Damaged_Num,
                               Dampness_CK = n.Dampness_CK,
                               Dampness_Num = n.Dampness_Num,
                               Deformation = n.Deformation,
                               Deformation_Num = n.Deformation_Num,
                               Is_GF = n.Is_GF,
                               Closure_Remark = n.Closure_Remark,
                               Is_QG = n.Is_QG,
                               Warehouse_Remark = n.Warehouse_Remark,
                               Consign_Code_CK = n.Consign_Code_CK,
                               MBL = n.MBL,
                               HBL = n.HBL,
                               Flight_Date_Want = n.Flight_Date_Want,
                               FLIGHT_No = n.FLIGHT_No,
                               End_Port = n.End_Port,
                               In_Date = n.In_Date,
                               In_Time = n.In_Time,
                               Out_Date = n.Out_Date,
                               Out_Time = n.Out_Time,
                               CH_Name_CK = n.CH_Name_CK,
                               Is_CustomerReturn = n.Is_CustomerReturn,
                               Is_MyReturn = n.Is_MyReturn,
                               Truck_Id = n.Truck_Id,
                               Driver = n.Driver,
                               Is_DamageUpload = n.Is_DamageUpload,
                               Is_DeliveryUpload = n.Is_DeliveryUpload,
                               Is_EntryUpload = n.Is_EntryUpload,
                               Status = n.Status,
                               Is_Binding = n.Is_Binding,
                               Remark = n.Remark,
                               OperatingPoint = n.OperatingPoint
                           };
            foreach (var item in datarows) 
            {
                Warehouse_receipt t = new Warehouse_receipt();
                               t.Id = item.Id;
                               t.Warehouse_Id = item.Warehouse_Id;
                               t.Entry_Id = item.Entry_Id;
                               t.Warehouse_Code = item.Warehouse_CodeName;
                               t.Pieces_CK = item.Pieces_CK;
                               t.Weight_CK = item.Weight_CK;
                               t.Volume_CK = item.Volume_CK;
                               t.Packing = item.PackingName;
                               t.CHARGE_WEIGHT_CK = item.CHARGE_WEIGHT_CK;
                               t.Bulk_Weight_CK = item.Bulk_Weight_CK;
                               t.Damaged_CK = item.Damaged_CK;
                               t.Damaged_Num = item.Damaged_Num;
                               t.Dampness_CK = item.Dampness_CK;
                               t.Dampness_Num = item.Dampness_Num;
                               t.Deformation = item.Deformation;
                               t.Deformation_Num = item.Deformation_Num;
                               t.Is_GF = item.Is_GF;
                               t.Closure_Remark = item.Closure_Remark;
                               t.Is_QG = item.Is_QG;
                               t.Warehouse_Remark = item.Warehouse_Remark;
                               t.Consign_Code_CK = item.Consign_Code_CK;
                               t.MBL = item.MBL;
                               t.HBL = item.HBL;
                               t.Flight_Date_Want = item.Flight_Date_Want;
                               t.FLIGHT_No = item.FLIGHT_No;
                               t.End_Port = item.End_Port;
                               t.In_Date = item.In_Date;
                               t.In_Time = item.In_Time;
                               t.Out_Date = item.Out_Date;
                               t.Out_Time = item.Out_Time;
                               t.CH_Name_CK = item.CH_Name_CK;
                               t.Is_CustomerReturn = item.Is_CustomerReturn;
                               t.Is_MyReturn = item.Is_MyReturn;
                               t.Truck_Id = item.Truck_Id;
                               t.Driver = item.Driver;
                               t.Is_DamageUpload = item.Is_DamageUpload;
                               t.Is_DeliveryUpload = item.Is_DeliveryUpload;
                               t.Is_EntryUpload = item.Is_EntryUpload;
                               t.Status = item.Status;
                               t.Is_Binding = item.Is_Binding;
                               t.Remark = item.Remark;
                               t.OperatingPoint = item.OperatingPoint;
                               NEWLIST.AddRange(t);
            }

            //var q_dataresult = datarows.OrderBy(sort, order).Skip((page - 1) * rows).Take(rows).ToList();
            if (CurrentAppUser.DepartMent == null)
            {
                totalCount = datarows.Count();
                //var data_Qresult = datarows.AsQueryable();
                var Q_Result = datarows.OrderBy(sort,order).Skip((page - 1) * rows).Take(rows).ToList();
                var pagelist = new { total = totalCount, rows = Q_Result };
                return Json(pagelist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var CurrentUserId = CurrentAppUser.Id;
                var ArrEntry = QPortalEntryIDLinkRep.Where(x => x.UserId == CurrentUserId).Select(x => x.EntryID);

                var dataresult = NEWLIST.Where(x => x.Status == AirOutEnumType.UseStatusIsOrNoEnum.draft);
                foreach (var item in ArrEntry)
                {
                    var Entrydata = dataresult.Where(x => x.Entry_Id.StartsWith(item));
                    FINALLIST.AddRange(Entrydata);
                }
                totalCount = FINALLIST.Count();
                var data_Qresult = FINALLIST.AsQueryable();
                var Q_Result = data_Qresult.OrderBy(sort, order).Skip((page - 1) * rows).Take(rows).ToList();
                var pagelist = new { total = totalCount, rows = Q_Result };
                return Json(pagelist, JsonRequestBehavior.AllowGet);               
            }
           
        }

        [HttpPost]
        public ActionResult SaveData(Warehouse_receiptChangeViewModel warehouse_receipt)
        {
            if (warehouse_receipt.updated != null)
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

                foreach (var updated in warehouse_receipt.updated)
                {
                    _warehouse_receiptService.Update(updated);
                }
            }
            if (warehouse_receipt.deleted != null)
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

                foreach (var deleted in warehouse_receipt.deleted)
                {
                    _warehouse_receiptService.Delete(deleted);
                }
            }
            if (warehouse_receipt.inserted != null)
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

                foreach (var inserted in warehouse_receipt.inserted)
                {
                    _warehouse_receiptService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((warehouse_receipt.updated != null && warehouse_receipt.updated.Any()) ||
                (warehouse_receipt.deleted != null && warehouse_receipt.deleted.Any()) ||
                (warehouse_receipt.inserted != null && warehouse_receipt.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(warehouse_receipt);
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

        // GET: Warehouse_receipts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_receipt warehouse_receipt = _warehouse_receiptService.Find(id);
            if (warehouse_receipt == null)
            {
                return HttpNotFound();
            }
            return View(warehouse_receipt);
        }

        // GET: Warehouse_receipts/Create
        public ActionResult Create()
        {
            Warehouse_receipt warehouse_receipt = new Warehouse_receipt();
            warehouse_receipt.Warehouse_Id = SequenceBuilder.Nextwarehouse_receipt_Warehouse_CodeSerial_No();
            //set default value
            return View(warehouse_receipt);
        }

        // GET: Warehouse_receipts/Create
        public ActionResult CreateWindow()
        {
            Warehouse_receipt warehouse_receipt = new Warehouse_receipt();
            //warehouse_receipt.Warehouse_Id = SequenceBuilder.Nextwarehouse_receipt_Warehouse_CodeSerial_No();
            //if (warehouse_receipt.Flight_Date_Want == null)
            //{
            //    //warehouse_receipt.Flight_Date_Want = Convert.ToDateTime("1900-01-01");
            //}
            DateTime dt = DateTime.Now;
            if (warehouse_receipt.In_Date == null)
            {
                warehouse_receipt.In_Date = dt;
            }
            if (warehouse_receipt.In_Time == null)
            {
                warehouse_receipt.In_Time = dt.ToShortTimeString().ToString();
            }
            //增加默认新增人和新增时间 2018-12-6 dz
            warehouse_receipt.ADDID = CurrentAppUser.Id;
            warehouse_receipt.ADDWHO = CurrentAppUser.UserNameDesc;
            warehouse_receipt.ADDTS = dt;

            //if (warehouse_receipt.Out_Date == null)
            //{
            //    //warehouse_receipt.Out_Date = Convert.ToDateTime("1900-01-01");
            //}
            ViewBag.picture = new List<Picture>();
            var ActionGuid = Guid.NewGuid().ToString();
            var ActionGuidName = "ActionGuid_" + Guid.NewGuid().ToString();
            TempData[ActionGuidName] = ActionGuid;
            ViewData["ActionGuidName"] = ActionGuidName;
            ViewData[ActionGuidName] = ActionGuid;
            //set default value
            return PartialView(warehouse_receipt);
        }

        // POST: Warehouse_receipts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Warehouse_Id,Entry_Id,Warehouse_Code,Pieces_CK,Weight_CK,Volume_CK,Packing,Bulk_Weight_CK,Damaged_CK,Dampness_CK,Deformation,Is_GF,Closure_Remark,Is_QG,Warehouse_Remark,Consign_Code_CK,MBL,HBL,Flight_Date_Want,FLIGHT_No,End_Port,In_Date,Out_Date,CH_Name_CK,Is_CustomerReturn,Is_MyReturn,Truck_Id,Driver,Is_DamageUpload,Is_DeliveryUpload,Is_EntryUpload,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Warehouse_receipt warehouse_receipt)
        {
            if (ModelState.IsValid)
            {
                _warehouse_receiptService.Insert(warehouse_receipt);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Warehouse_receipt record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(warehouse_receipt);
        }

        /// <summary>
        /// 保存仓库接单信息
        /// </summary>
        /// <param name="WarehouseReceipt"></param>
        /// <param name="warehouse_cargo_size"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveWarehouse(Warehouse_receipt WarehouseReceipt = null, Warehouse_Cargo_SizeChangeViewModel warehouse_cargo_size = null)//OPS_EntrustmentInfor EntrustmentInfor, OPS_M_Order M_Order, OPS_H_Order H_Order
        {
            try
            {
                #region 表单唯一值,防止 重复提交

                string ActionGuidName = Request["ActionGuidName"] ?? "";
                string ActionGuid = Request["ActionGuid"] ?? "";
                var IsPostRepeat = false;
                var NewActionGuid = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(ActionGuidName) || string.IsNullOrEmpty(ActionGuid) || !TempData.ContainsKey(ActionGuidName))
                {
                    IsPostRepeat = true;
                }
                else if (TempData[ActionGuidName].ToString() != ActionGuid)
                {
                    IsPostRepeat = true;
                }
                else
                {
                    TempData[ActionGuidName] = NewActionGuid;
                }
                if (IsPostRepeat)
                {
                    TempData[ActionGuidName] = NewActionGuid;
                    ViewData["ActionGuidName"] = ActionGuidName;
                    ViewData[ActionGuidName] = NewActionGuid;
                    return Json(new { success = false, err = "重复提交，或表单数据已更新，请刷新后再保存！", ActionGuid = NewActionGuid, ActionGuidName = ActionGuidName }, JsonRequestBehavior.AllowGet);
                }

                #endregion

                var Warehouse_Id = "";
                List<string> excludeW = new List<string>() { "Id", "OperatingPoint", "ADDID", "ADDWHO", "ADDTS", "EDITWHO", "EDITTS", "EDITID" };
                var ContentW = "";

                if (WarehouseReceipt != null)
                {
                    if (WarehouseReceipt.Id == 0)
                    {//新增仓库接单主表
                        Warehouse_Id = WarehouseReceipt.Warehouse_Id = SequenceBuilder.Nextwarehouse_receipt_Warehouse_CodeSerial_No();
                        WarehouseReceipt.Id = -1;
                        WarehouseReceipt.ObjectState = ObjectState.Added;
                        _warehouse_receiptService.Insert(WarehouseReceipt);
                    }
                    else
                    {
                        #region 仓库接单主表修改保存
                        var warehouse_receipt = _warehouse_receiptService.Find(WarehouseReceipt.Id);

                        ContentW = Common.DifferenceComparison<Warehouse_receipt>(warehouse_receipt, WarehouseReceipt, excludeW);

                        if (warehouse_receipt != null)
                        {
                            if (warehouse_receipt.Entry_Id != WarehouseReceipt.Entry_Id)
                            {
                                var details = _warehouse_Cargo_SizeService.Queryable().Where(x => x.Warehouse_Receipt_Id == WarehouseReceipt.Id).ToList();
                                if (details != null)
                                {
                                    foreach (var item in details)
                                    {
                                        item.Entry_Id = WarehouseReceipt.Entry_Id;
                                        item.ObjectState = ObjectState.Modified;
                                        _warehouse_Cargo_SizeService.Update(item);

                                    }
                                }
                            }
                            Warehouse_Id = WarehouseReceipt.Warehouse_Id;
                            warehouse_receipt.Warehouse_Code = WarehouseReceipt.Warehouse_Code;
                            warehouse_receipt.Entry_Id = WarehouseReceipt.Entry_Id;
                            warehouse_receipt.Pieces_CK = WarehouseReceipt.Pieces_CK;
                            warehouse_receipt.Weight_CK = WarehouseReceipt.Weight_CK;
                            warehouse_receipt.Volume_CK = WarehouseReceipt.Volume_CK;
                            warehouse_receipt.Packing = WarehouseReceipt.Packing;
                            warehouse_receipt.CHARGE_WEIGHT_CK = WarehouseReceipt.CHARGE_WEIGHT_CK;
                            warehouse_receipt.Bulk_Weight_CK = WarehouseReceipt.Bulk_Weight_CK;
                            warehouse_receipt.Damaged_CK = WarehouseReceipt.Damaged_CK;
                            warehouse_receipt.Damaged_Num = WarehouseReceipt.Damaged_Num;
                            warehouse_receipt.Dampness_CK = WarehouseReceipt.Dampness_CK;
                            warehouse_receipt.Dampness_Num = WarehouseReceipt.Dampness_Num;
                            warehouse_receipt.Deformation = WarehouseReceipt.Deformation;
                            warehouse_receipt.Deformation_Num = WarehouseReceipt.Deformation_Num;
                            warehouse_receipt.Is_GF = WarehouseReceipt.Is_GF;
                            warehouse_receipt.Closure_Remark = WarehouseReceipt.Closure_Remark;
                            warehouse_receipt.Is_QG = WarehouseReceipt.Is_QG;
                            warehouse_receipt.Warehouse_Remark = WarehouseReceipt.Warehouse_Remark;
                            warehouse_receipt.Consign_Code_CK = WarehouseReceipt.Consign_Code_CK;
                            warehouse_receipt.MBL = WarehouseReceipt.MBL;
                            warehouse_receipt.MBLId = WarehouseReceipt.MBLId;
                            warehouse_receipt.HBL = WarehouseReceipt.HBL;
                            warehouse_receipt.Flight_Date_Want = WarehouseReceipt.Flight_Date_Want;
                            warehouse_receipt.FLIGHT_No = WarehouseReceipt.FLIGHT_No;
                            warehouse_receipt.End_Port = WarehouseReceipt.End_Port;
                            warehouse_receipt.In_Date = WarehouseReceipt.In_Date;
                            warehouse_receipt.In_Time = WarehouseReceipt.In_Time;
                            warehouse_receipt.Out_Date = WarehouseReceipt.Out_Date;
                            warehouse_receipt.Out_Time = WarehouseReceipt.Out_Time;
                            warehouse_receipt.CH_Name_CK = WarehouseReceipt.CH_Name_CK;
                            warehouse_receipt.Is_CustomerReturn = WarehouseReceipt.Is_CustomerReturn;
                            warehouse_receipt.Is_MyReturn = WarehouseReceipt.Is_MyReturn;
                            warehouse_receipt.Truck_Id = WarehouseReceipt.Truck_Id;
                            warehouse_receipt.Driver = WarehouseReceipt.Driver;
                            warehouse_receipt.Is_DamageUpload = WarehouseReceipt.Is_DamageUpload;
                            warehouse_receipt.Is_DeliveryUpload = WarehouseReceipt.Is_DeliveryUpload;
                            warehouse_receipt.Is_EntryUpload = WarehouseReceipt.Is_EntryUpload;
                            warehouse_receipt.Status = WarehouseReceipt.Status;
                            warehouse_receipt.Remark = WarehouseReceipt.Remark;
                            warehouse_receipt.OperatingPoint = WarehouseReceipt.OperatingPoint;
                            warehouse_receipt.Is_Binding = WarehouseReceipt.Is_Binding;
                            WarehouseReceipt.ObjectState = ObjectState.Modified;
                            _warehouse_receiptService.Update(warehouse_receipt);

                        }
                        #endregion
                    }

                    #region 添加修改删除仓库明细

                    if (warehouse_cargo_size != null)
                    {
                        if (warehouse_cargo_size.updated != null)
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

                            foreach (var updated in warehouse_cargo_size.updated)
                            {
                                var detail = _warehouse_Cargo_SizeService.Find(updated.Id);
                                if (detail != null)
                                {
                                    detail.CM_Length = updated.CM_Length;
                                    detail.CM_Width = updated.CM_Width;
                                    detail.CM_Height = updated.CM_Height;
                                    detail.CM_Piece = updated.CM_Piece;
                                    updated.Warehouse_Receipt_Id = WarehouseReceipt.Id;
                                    updated.Entry_Id = WarehouseReceipt.Entry_Id;
                                    updated.ObjectState = ObjectState.Modified;
                                    _warehouse_Cargo_SizeService.Update(detail);
                                }
                                else
                                {
                                    if (updated.CM_Length != null || updated.CM_Height != null || updated.CM_Piece != null || updated.CM_Width != null)
                                    {
                                        updated.ObjectState = ObjectState.Added;
                                        updated.Warehouse_Receipt_Id = WarehouseReceipt.Id;
                                        updated.Entry_Id = WarehouseReceipt.Entry_Id;
                                        _warehouse_Cargo_SizeService.Insert(updated);
                                    }
                                }
                            }
                        }
                        if (warehouse_cargo_size.deleted != null)
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

                            foreach (var deleted in warehouse_cargo_size.deleted)
                            {
                                var detaildeleted = _warehouse_Cargo_SizeService.Find(deleted.Id);
                                if (detaildeleted != null)
                                {
                                    _warehouse_Cargo_SizeService.Delete(detaildeleted);
                                }
                            }
                        }
                        if (warehouse_cargo_size.inserted != null)
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

                            foreach (var inserted in warehouse_cargo_size.inserted)
                            {
                                if (inserted.CM_Length != null || inserted.CM_Height != null || inserted.CM_Piece != null || inserted.CM_Width != null)
                                {
                                    inserted.Warehouse_Receipt_Id = WarehouseReceipt.Id;
                                    inserted.Entry_Id = WarehouseReceipt.Entry_Id;
                                    _warehouse_Cargo_SizeService.Insert(inserted);
                                }
                            }
                        }
                    }
                    #endregion
                    //if (WarehouseSize != null && WarehouseSize.Count > 0)
                    //{
                    //    foreach(var item in WarehouseSize){
                    //        if (item.Id == 0)
                    //        {//仓库订单明细新增
                    //            item.Warehouse_Receipt_Id = WarehouseReceipt.Id;
                    //            item.Entry_Id = WarehouseReceipt.Entry_Id;
                    //            _warehouse_Cargo_SizeService.Insert(item);
                    //        }
                    //        else
                    //        {
                    //            var detail = _warehouse_Cargo_SizeService.Find(item.Id);
                    //            detail.CM_Length = item.CM_Length;
                    //            detail.CM_Width = item.CM_Width;
                    //            detail.CM_Height = item.CM_Height;
                    //            detail.CM_Piece = item.CM_Piece;
                    //            detail.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                    //            _warehouse_Cargo_SizeService.Update(detail);
                    //        }
                    //    }
                    //}
                    _unitOfWork.SaveChanges();

                    #region 绑定数据库
                    if (WarehouseReceipt.Is_Binding)
                    {
                        //如果已经绑定，则更新承揽接单中的实际数量
                        var opslist = _oPS_EntrustmentInforService.Queryable().Where(x => (x.MBLId == WarehouseReceipt.MBLId) && x.Is_TG == false).ToList();
                        if (opslist != null && opslist.Count == 1)
                        {
                            decimal? Volume_CK = 0;
                            decimal? Weight_CK = 0;
                            decimal? Pieces_CK = 0;
                            decimal? Charge_Weight_CK = 0;
                            var warlist = _warehouse_receiptService.Queryable().Where(x => x.MBLId == WarehouseReceipt.MBLId && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.draft).ToList();
                            if (warlist != null)
                            {
                                foreach (var war in warlist)
                                {
                                    Volume_CK += war.Volume_CK;
                                    Weight_CK += war.Weight_CK;
                                    Pieces_CK += war.Pieces_CK;
                                    Charge_Weight_CK += war.CHARGE_WEIGHT_CK;
                                }
                            }
                            if (WarehouseReceipt.Id <= 0)
                            {
                                Volume_CK += WarehouseReceipt.Volume_CK;
                                Weight_CK += WarehouseReceipt.Weight_CK;
                                Pieces_CK += WarehouseReceipt.Pieces_CK;
                                Charge_Weight_CK += WarehouseReceipt.CHARGE_WEIGHT_CK;
                            }
                            var item = opslist.FirstOrDefault();
                            item.Volume_Fact = Volume_CK;
                            item.Weight_Fact = Weight_CK;
                            item.Pieces_Fact = Pieces_CK;
                            item.Charge_Weight_Fact = Charge_Weight_CK;
                            _oPS_EntrustmentInforService.Update(item);
                            _unitOfWork.SaveChanges();

                            var ContentE = "仓库操作 " + item.Operation_Id + "," + Pieces_CK.ToString() + "," + Weight_CK.ToString() + "," + Volume_CK.ToString();
                            _changeOrderHistoryService.InsertChangeOrdHistory(item.Id, "OPS_EntrustmentInfor", ChangeOrderHistory.EnumChangeType.Modify, ContentE, 0, 1, 0);
                        }
                    }
                    #endregion

                    #region 仓库接单日志
                    if (WarehouseReceipt.ObjectState == ObjectState.Modified)
                    {
                        _changeOrderHistoryService.InsertChangeOrdHistory(WarehouseReceipt.Id, "Warehouse_receipt", ChangeOrderHistory.EnumChangeType.Modify, "仓库操作 " + ContentW, 0, 1, 0);
                    }
                    else
                    {
                        _changeOrderHistoryService.InsertChangeOrdHistory(WarehouseReceipt.Id, "Warehouse_receipt", ChangeOrderHistory.EnumChangeType.Insert, "仓库操作 " + ContentW, 0, 0, 1);
                    }

                    if (warehouse_cargo_size != null)
                    {

                        if (warehouse_cargo_size.updated != null)
                        {
                            foreach (var updated in warehouse_cargo_size.updated)
                            {
                                if (updated.ObjectState == ObjectState.Modified)
                                {
                                    ContentW = "仓库操作 " + updated.Entry_Id + "," + updated.CM_Length + "*" + updated.CM_Width + "*" + updated.CM_Height + "*" + updated.CM_Piece;
                                    _changeOrderHistoryService.InsertChangeOrdHistory(updated.Id, "Warehouse_Cargo_Size", ChangeOrderHistory.EnumChangeType.Modify, ContentW, 0, 1, 0);
                                }
                                //else
                                //{
                                //    Content = updated.Entry_Id + "," + updated.CM_Length + "*" + updated.CM_Width + "*" + updated.CM_Height + "*" + updated.CM_Piece;
                                //    _changeOrderHistoryService.InsertChangeOrdHistory(updated.Id, "Warehouse_Cargo_Size", ChangeOrderHistory.EnumChangeType.Insert, Content, 0, 0, 1);
                                //}
                            }
                        }
                    }

                    #endregion
                    return Json(new { success = true, Id = WarehouseReceipt.Id, Warehouse_Id = Warehouse_Id, ActionGuid = NewActionGuid, ActionGuidName = ActionGuidName }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, err = "仓库接单信息为空", ActionGuid = NewActionGuid, ActionGuidName = ActionGuidName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ActionGuidName = Guid.NewGuid().ToString();
                var NewActionGuid = Guid.NewGuid().ToString();
                TempData[ActionGuidName] = NewActionGuid;
                ViewData["ActionGuidName"] = ActionGuidName;
                ViewData[ActionGuidName] = NewActionGuid;
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { success = false, err = ErrMsg, ActionGuid = NewActionGuid, ActionGuidName = ActionGuidName }, JsonRequestBehavior.AllowGet); ;
            }
        }

        /// <summary>
        /// 进仓编号绑定承揽接单
        /// </summary>
        /// <param name="Entry_Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BindingOPS(Warehouse_receipt warehousereceipt)
        {
            var receipt = _warehouse_receiptService.Queryable().Where(x => (x.Entry_Id.Equals(warehousereceipt.Entry_Id) || x.MBL.Equals(warehousereceipt.Entry_Id) || x.HBL.Equals(warehousereceipt.Entry_Id)) && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.draft).ToList();
            //if (receipt.Count > 0)
            //{//如果该进仓编号在数据表中已经存在，则绑定失败，
            //    var recitem = receipt.FirstOrDefault();
            //    if (recitem.Is_Binding)
            //    {
            //        return Json(new { success = false, isempty = false, ErrMsg = "该进仓编号已经存在或已经绑定了委托！" }, JsonRequestBehavior.AllowGet);
            //    }
            //}
            var entru = _oPS_EntrustmentInforService.Queryable().Where(x => (x.MBL.Equals(warehousereceipt.Entry_Id) || x.Operation_Id.Equals(warehousereceipt.Entry_Id)) && x.Is_TG == false).ToList();
            if (entru.Count == 0)
            {//如果该进仓编号在委托表中没有对应的总单号或业务编号，则绑定失败
                return Json(new { success = false, isempty = false, ErrMsg = "该进仓编号对应的总单号和业务编号不存在！" }, JsonRequestBehavior.AllowGet);
            }
            if (entru.Count > 1)
            {//如果该进仓编号在委托表中没有对应的总单号或委托编号超过1条，则绑定失败
                var m_order = _unitOfWork.RepositoryAsync<OPS_M_Order>().Queryable().Where(x => x.MBL == warehousereceipt.Entry_Id && x.OPS_BMS_Status == false).ToList();
                if (m_order == null)
                {
                    return Json(new { success = false, isempty = false, ErrMsg = "该进仓编号对应的总单号没有找到，不能绑定！" }, JsonRequestBehavior.AllowGet);
                }
                if (m_order.Count > 1)
                {
                    return Json(new { success = false, isempty = false, ErrMsg = "该进仓编号对应的总单号和业务编号超过1条，不能绑定！" }, JsonRequestBehavior.AllowGet);
                }
                else if (m_order.Count == 1)
                {
                    var _m_order = m_order.FirstOrDefault();
                    entru = entru.Where(x => x.MBLId == _m_order.Id).ToList();
                    if (entru.Count == 0)
                    {//如果该进仓编号在委托表中没有对应的总单号或业务编号，则绑定失败
                        return Json(new { success = false, isempty = false, ErrMsg = "该进仓编号对应的总单号和业务编号不存在！" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var item = entru.FirstOrDefault();
            if (item.Flight_Date_Want != null)
            {
                System.TimeSpan ts = DateTime.Now - (DateTime)item.Flight_Date_Want;
                int days = ts.Days;
                if (days > 15)
                {
                    return Json(new { success = false, isempty = true, ErrMsg = "该进仓编号对应的总单号或业务编号已经超过15天，请到承揽接单手工绑定！" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    warehousereceipt.Is_Binding = true;
                    warehousereceipt.MBL = item.MBL;
                    warehousereceipt.HBL = item.Operation_Id;
                    warehousereceipt.MBLId = item.MBLId;
                    warehousereceipt.Consign_Code_CK = item.Entrustment_Name;
                    warehousereceipt.End_Port = item.End_Port;
                    warehousereceipt.Flight_Date_Want = item.Flight_Date_Want;
                    warehousereceipt.FLIGHT_No = item.Flight_No;
                }
            }
            else
            {
                warehousereceipt.Is_Binding = true;
                warehousereceipt.MBL = item.MBL;
                warehousereceipt.HBL = item.Operation_Id;
                warehousereceipt.MBLId = item.MBLId;
                warehousereceipt.Consign_Code_CK = item.Entrustment_Name;
                warehousereceipt.End_Port = item.End_Port;
                warehousereceipt.Flight_Date_Want = item.Flight_Date_Want;
                warehousereceipt.FLIGHT_No = item.Flight_No;
            }
            return Json(new { success = true, data = warehousereceipt }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 作废仓库接单信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult SaveDeclareInvalid(List<int> ids, string type = "")
        {
            try
            {
                string ErrMsg = "";
                var strids = "";
                var update = "0";
                var idarr = ids;
                var Arr = _warehouse_receiptService.Queryable().Where(x => idarr.Contains(x.Id)).Include(x => x.ArrWarehouse_Cargo_Size).ToList();
                foreach (var id in idarr)
                {
                    Warehouse_receipt item = Arr.Where(x => x.Id == id).FirstOrDefault();
                    if (item != null && item.Id > 0)
                    {
                        if (!item.Is_Binding)
                        {
                            if (type == "invalid")
                            {
                                item.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                                _warehouse_receiptService.Update(item);
                                var wardetails = _warehouse_Cargo_SizeService.Queryable().Where(x => x.Entry_Id == item.Entry_Id).ToList();
                                if (wardetails != null)
                                {
                                    foreach (var detail in wardetails)
                                    {
                                        detail.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                                        _warehouse_Cargo_SizeService.Update(detail);
                                    }
                                }
                                update = "1";
                                ErrMsg = "作废完成！";
                            }
                            else if (type == "delete")
                            {
                                update = "1";
                                if (item.ArrWarehouse_Cargo_Size != null && item.ArrWarehouse_Cargo_Size.Any())
                                {
                                    var ArrDtlId = item.ArrWarehouse_Cargo_Size.Select(x => x.Id).ToList();
                                    foreach (var dtlId in ArrDtlId)
                                    {
                                        _warehouse_Cargo_SizeService.Delete(dtlId);
                                    }
                                }
                                _warehouse_receiptService.Delete(item.Id);
                                ErrMsg = "删除完成！";
                            }
                        }
                        else
                        {
                            ErrMsg = "进仓编号：" + item.Entry_Id + "已经绑定承揽接单！";
                        }
                    }
                }
                _unitOfWork.SaveChanges();
                if (update == "1")
                {
                    foreach (var id in idarr)
                    {
                        Warehouse_receipt EntrustmentInfor = _warehouse_receiptService.Find(id);
                        //自动更新 缓存
                        if (IsAutoResetCache)
                            AutoResetCache(EntrustmentInfor);
                        return Json(new { Success = true, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);

                    }
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

        // GET: Warehouse_receipts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_receipt warehouse_receipt = _warehouse_receiptService.Find(id);
            if (warehouse_receipt == null)
            {
                return HttpNotFound();
            }
            var picture = _pictureService.Queryable().Where(x => x.Code.Equals(warehouse_receipt.Entry_Id) && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).ToList();
            if (picture.Count == 0)
            {
                ViewBag.picture = new Picture();
            }
            else
            {
                foreach (var item in picture)
                {
                    item.OperatingPoint = (int)item.Type;
                }
                ViewBag.picture = picture;
            }
            return View(warehouse_receipt);
        }

        // GET: Warehouse_receipts/Edit/5
        public ActionResult EditWindow(int? id, string Warehouse_Id = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_receipt warehouse_receipt = null;
            if (id != 0)
            {
                warehouse_receipt = _warehouse_receiptService.Find(id);
            }
            else
            {
                var war = _warehouse_receiptService.Queryable().Where(x => x.Warehouse_Id == Warehouse_Id).ToList();
                if (war != null && war.Count() > 0)
                {
                    warehouse_receipt = war.FirstOrDefault();
                }
            }
            if (warehouse_receipt == null)
            {
                return HttpNotFound();
            }
            var picture = _pictureService.Queryable().Where(x => x.Code.Equals(warehouse_receipt.Warehouse_Id) && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).ToList();
            if (picture == null)
            {
                ViewBag.picture = new List<Picture>();
            }
            else
            {
                foreach (var item in picture)
                {
                    item.OperatingPoint = (int)item.Type;
                }
                ViewBag.picture = picture;
            }
            var ODynamic = GetFromNAME(warehouse_receipt);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);

            return PartialView("EditWindow", warehouse_receipt);
            //return View(warehouse_receipt);
        }



        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(Warehouse_receipt _Warehouse_receipt, int index = 0)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();

            //客商信息
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);

            if (!string.IsNullOrEmpty(_Warehouse_receipt.Consign_Code_CK))
            {//委托方
                var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == _Warehouse_receipt.Consign_Code_CK).FirstOrDefault();
                ODynamic.Consign_Code_CKNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
            }

            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();
            //添加人
            if (!string.IsNullOrEmpty(_Warehouse_receipt.ADDWHO))
            {
                var OAppUser = ArrAppUser.Where(x => x.UserName == _Warehouse_receipt.ADDWHO).FirstOrDefault();
                ODynamic.ADDWHONAME = OAppUser == null ? "" : OAppUser.UserNameDesc;
            }
            //修改人
            if (!string.IsNullOrEmpty(_Warehouse_receipt.EDITWHO))
            {
                var OAppUser = ArrAppUser.Where(x => x.UserName == _Warehouse_receipt.EDITWHO).FirstOrDefault();
                ODynamic.EDITWHONAME = OAppUser == null ? "" : OAppUser.UserNameDesc;
            }


            ////港口
            //var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            //if (!string.IsNullOrEmpty(_Warehouse_receipt.End_Port))
            //{//目的港
            //    var OPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == _Warehouse_receipt.End_Port).FirstOrDefault();
            //    ODynamic.End_PortNAME = OPARA_AirPort == null ? "" : _Warehouse_receipt.End_Port;
            //}

            return ODynamic;
        }

        // POST: Warehouse_receipts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Warehouse_Id,Entry_Id,Warehouse_Code,Pieces_CK,Weight_CK,Volume_CK,Packing,Bulk_Weight_CK,Damaged_CK,Dampness_CK,Deformation,Is_GF,Closure_Remark,Is_QG,Warehouse_Remark,Consign_Code_CK,MBL,HBL,Flight_Date_Want,FLIGHT_No,End_Port,In_Date,Out_Date,CH_Name_CK,Is_CustomerReturn,Is_MyReturn,Truck_Id,Driver,Is_DamageUpload,Is_DeliveryUpload,Is_EntryUpload,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Warehouse_receipt warehouse_receipt)
        {
            if (ModelState.IsValid)
            {
                warehouse_receipt.ObjectState = ObjectState.Modified;
                _warehouse_receiptService.Update(warehouse_receipt);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Warehouse_receipt record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(warehouse_receipt);
        }

        // GET: Warehouse_receipts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_receipt warehouse_receipt = _warehouse_receiptService.Find(id);
            if (warehouse_receipt == null)
            {
                return HttpNotFound();
            }
            return View(warehouse_receipt);
        }

        // POST: Warehouse_receipts/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Warehouse_receipt warehouse_receipt = _warehouse_receiptService.Find(id);
            _warehouse_receiptService.Delete(warehouse_receipt);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Warehouse_receipt record");
            return RedirectToAction("Index");
        }

        //获取仓库信息代码和名称
        public ActionResult GetCodeItem(int page = 1, int rows = 10, string q = "", string CodeType = "")
        {
            var baseCodeRep = _unitOfWork.RepositoryAsync<BaseCode>();
            var codeItemRep = _unitOfWork.RepositoryAsync<CodeItem>();

            var data = from n in codeItemRep.Queryable()
                       join b in baseCodeRep.Queryable() on n.BaseCodeId equals b.Id into nb
                       from bc in nb.DefaultIfEmpty()
                       where bc.CodeType == CodeType//new CodeType.Contains(bc.CodeType)
                       select new
                       {
                           n.Code,
                           n.Text,
                           bc.CodeType,
                       };
            if (q != "")
            {
                data = data.Where(n => n.Code.Contains(q) || n.Code.Contains(q));
            }
            var list = data.Select(n => new { ID = n.Code, TEXT = n.Text, IDTEXT = n.Code + "|" + n.Text });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //获取币制信息代码和名称
        public ActionResult GetPARA_Package(int page = 1, int rows = 10, string q = "")
        {
            var para_PackageRep = _unitOfWork.RepositoryAsync<PARA_Package>();
            var data = para_PackageRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.PackageCode.Contains(q) || n.PackageName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.PackageCode, TEXT = n.PackageName, IDTEXT = n.PackageCode + "|" + n.PackageName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }
        //导出批量进仓单
        [HttpPost]
        public ActionResult ExportBatchExcel(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _warehouse_receiptService.Queryable().Where(x => idarr.Contains(x.Warehouse_Id)).ToList();
            //if (data == null || data.Count == 0)
            //{
            //    return null;
            //}
            var details = _warehouse_Cargo_SizeService.Queryable().ToList();
            var packinglist = _unitOfWork.RepositoryAsync<PARA_Package>().Queryable().ToList();

            var datarows = (from n in data
                            join x in details on n.Id equals x.Warehouse_Receipt_Id into x_tmp
                            from xtmp in x_tmp.DefaultIfEmpty()
                            join p in packinglist on n.Packing equals p.PackageCode into p_tmp
                            from ptmp in p_tmp.DefaultIfEmpty()
                            orderby n.Entry_Id

                            select new
                            {
                                Entry_Id = n.Entry_Id,
                                Pieces_CK = n.Pieces_CK,
                                Weight_CK = n.Weight_CK,
                                Volume_CK = n.Volume_CK,
                                Size = xtmp == null ? "" : "【" + (xtmp.CM_Length == null ? "" : xtmp.CM_Length.ToString()) + "*" + (xtmp.CM_Width == null ? "" : xtmp.CM_Width.ToString()) + "*" + (xtmp.CM_Height == null ? "" : xtmp.CM_Height.ToString()) + "*" + (xtmp.CM_Piece == null ? "" : xtmp.CM_Piece.ToString()) + "】",
                                Packing = (n.Pieces_CK == null ? "" : n.Pieces_CK.ToString()) + (ptmp == null ? "" : ptmp.PackageName),//包装情况
                                Remark = n.Remark,
                                //Document = "" //随机单证
                            }).ToList();
            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(datarows.ToList(), @"\FileModel\ExportBatchExcel.xls", 0, 0, out outfile, "进仓单");
            var ms = new System.IO.MemoryStream(outbytes);
            var newms = Server.MapPath(@"\DownLoad\aaa.xls");
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 货物入库单(导出进仓单PDF)
        /// </summary>
        /// <param name="filterRules"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public ActionResult exportPDFWarehouseReceiptlistPDF(Array filterRules = null, string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            var strids = "";
            string loginname = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _warehouse_receiptService.Queryable().Where(x => idarr.Contains(x.Warehouse_Id)).Include(x => x.ArrWarehouse_Cargo_Size).ToList();

            var ArrPARA_Package = (List<PARA_Package>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Package);
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var ArrUsers = (List<ApplicationUser>)CacheHelper.Get_SetCache(Common.CacheNameS.ApplicationUser);

            List<pdfJCD> ArrExcelFile = new List<pdfJCD>();
            List<string> ArrPdfFile = new List<string>();
            List<string> ArrErrMsg = new List<string>();
            var rows = from n in data
                       join c in ArrCusBusInfo on n.Consign_Code_CK equals c.EnterpriseId into c_tmp
                       from ctmp in c_tmp.DefaultIfEmpty()
                       join p in ArrPARA_Package on n.Packing equals p.PackageCode into p_tmp
                       from ptmp in p_tmp.DefaultIfEmpty()
                       join u in ArrUsers on n.ADDWHO equals u.UserName into u_tmp
                       from utmp in u_tmp.DefaultIfEmpty()
                       select new
                       {
                           Id = n.Id,
                           Entry_Id = n.Entry_Id,
                           Warehouse_Id = n.Warehouse_Id,
                           Out_Date = n.In_Date,
                           Out_Time = n.In_Time,
                           Consign_Code_CK_Name = ctmp == null ? "" : ctmp.EnterpriseShortName,
                           End_Port = n.End_Port,
                           ArrWarehouse_Cargo_Size = n.ArrWarehouse_Cargo_Size == null ? null : n.ArrWarehouse_Cargo_Size.Where(x => x.Status == AirOutEnumType.UseStatusIsOrNoEnum.draft),
                           Mask = "",
                           Packing_Name = ptmp == null ? "" : n.Pieces_CK + " " + ptmp.PackageName,
                           Remark = n.Remark,
                           Pieces_CK = n.Pieces_CK,
                           Weight_CK = n.Weight_CK,
                           Volume_CK = n.Volume_CK,
                           username = (utmp == null ? "" : utmp.UserNameDesc),
                       };

            if (rows != null)
            {
                foreach (var row in rows)
                {
                    pdfJCD jcd = new pdfJCD();
                    jcd.Entry_Id = row.Entry_Id;
                    jcd.Warehouse_Id = row.Warehouse_Id;
                    if (row.Out_Date != null)
                    {
                        DateTime dt = (DateTime)row.Out_Date;
                        jcd.Year = dt.Year.ToString().Substring(2, 2);
                        jcd.Month = dt.Month.ToString();
                        jcd.Day = dt.Day.ToString();
                    }
                    jcd.Out_Time = row.Out_Time;
                    jcd.Consign_Code_CK_Name = row.Consign_Code_CK_Name;
                    jcd.End_Port = row.End_Port;
                    jcd.Packing_Name = row.Packing_Name;
                    jcd.Remark = row.Remark;
                    if (row.Pieces_CK != null)
                        jcd.Pieces_CK = row.Pieces_CK.ToString();//ConvertToChineseNumber(row.Pieces_CK.ToString());
                    if (row.Weight_CK != null)
                        jcd.Weight_CK = row.Weight_CK.ToString();
                    if (row.Volume_CK != null)
                        jcd.Volume_CK = row.Volume_CK.ToString();
                    string mask = "";
                    if (row.ArrWarehouse_Cargo_Size != null)
                    {
                        int dnum = 0;
                        foreach (var item in row.ArrWarehouse_Cargo_Size)
                        {
                            pdfJCDList detail = new pdfJCDList();
                            if (item.CM_Length != null)
                            {
                                detail.CM_Length = item.CM_Length.ToString();
                                mask = getMask(mask, dnum, detail.CM_Length, "CM_Length");
                            }
                            if (item.CM_Width != null)
                            {
                                detail.CM_Width = item.CM_Width.ToString();
                                mask = getMask(mask, dnum, detail.CM_Width, "CM_Width");
                            }
                            if (item.CM_Height != null)
                            {
                                detail.CM_Height = item.CM_Height.ToString();
                                mask = getMask(mask, dnum, detail.CM_Height, "CM_Height");
                            }
                            if (item.CM_Piece != null)
                            {
                                detail.CM_Piece = item.CM_Piece.ToString();
                                mask = getMask(mask, dnum, detail.CM_Piece, "CM_Piece");
                            }
                            jcd.ArrPdfJCDList.Add(detail);
                            dnum = dnum + 1;
                        }
                    }

                    jcd.Mask = mask;
                    loginname = row.username;
                    ArrExcelFile.Add(jcd);
                    int listnum = 0;
                    if (jcd.ArrPdfJCDList != null)
                    {
                        listnum = jcd.ArrPdfJCDList.Count();
                    }

                    List<pdfJCD> ExcelFileList = new List<pdfJCD>();
                    ExcelFileList.Add(jcd);
                    var Ret = WordHelper.SetWordModel_BookMarkByT<pdfJCD>(jcd, Server.MapPath("/FileModel/PDFWarehouseReceipt.docx"), ExcelFileList, listnum, loginname, Aspose.Words.SaveFormat.Doc);
                    if (Ret.retResult)
                    {
                        ArrPdfFile.Add(Ret.MsgStr);
                    }
                    else
                    {
                        ArrErrMsg.Add(Ret.MsgStr);
                    }
                }

            }
            Aspose.Words.Document newDoc = null;
            for (var i = 0; i < ArrPdfFile.Count; i++)
            {
                var item = ArrPdfFile[i];
                if (i == 0)
                {
                    newDoc = new Aspose.Words.Document(item);
                    RemovePageBreaks(newDoc);
                }
                else
                {
                    var doc = new Aspose.Words.Document(item);
                    newDoc.AppendDocument(doc, Aspose.Words.ImportFormatMode.UseDestinationStyles);
                    RemovePageBreaks(newDoc);
                }
            }

            var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
            string FName = Server.MapPath(dPath + "/pdfDD/" + fileName);
            newDoc.Save(FName, Aspose.Words.SaveFormat.Pdf);

            System.IO.FileInfo oo = new System.IO.FileInfo(FName);
            return File(FName, "application/octet-stream;", oo.Name);
        }

        /// <summary>
        /// 货物入库单导出PDF时，唛头
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="dnum"></param>
        /// <param name="detail"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string getMask(string mask, int dnum, string detail, string type)
        {
            if (dnum > 7)
            {
                if (mask != "" && type == "CM_Length")
                {
                    mask = mask + ", ";
                }
                if (type == "CM_Length")
                    mask = mask + detail;
                else
                    mask = mask + "*" + detail;
            }
            return mask;
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
        /// <returns></returns>
        private static void RemovePageBreaks(Aspose.Words.Document doc)
        {
            // Retrieve all paragraphs in the document.
            Aspose.Words.NodeCollection paragraphs = doc.GetChildNodes(Aspose.Words.NodeType.Paragraph, true);

            // Iterate through all paragraphs
            foreach (Aspose.Words.Paragraph para in paragraphs)
            {
                // If the paragraph has a page break before set then clear it.
                if (para.ParagraphFormat.PageBreakBefore)
                    para.ParagraphFormat.PageBreakBefore = false;

                // Check all runs in the paragraph for page breaks and remove them.
                foreach (Aspose.Words.Run run in para.Runs)
                {
                    if (run.Text.Contains(Aspose.Words.ControlChar.PageBreak))
                        run.Text = run.Text.Replace(Aspose.Words.ControlChar.PageBreak, string.Empty);
                }

            }

        }


        public string ConvertToChineseNumber(string old)
        {
            Chinese ch = new Chinese();
            long num = Convert.ToInt64(old);
            string re = ch.returnResult(num);
            if (re.StartsWith("壹拾"))
            {
                re = re.Substring(1, re.Length - 1);
            }

            return (re);
        }

        public class Chinese
        {
            public string returnResult(long num)
            {
                string numStr = num.ToString();
                if (numStr.Length > 8 & numStr.Length < 16)
                {
                    string[] firstSplit = new string[2];
                    firstSplit[0] = numStr.Substring(0, numStr.Length - 8);
                    firstSplit[1] = numStr.Substring(numStr.Length - 8, 8);
                    string result1 = getString(firstSplit[0]) + "億";
                    string result2 = getString(firstSplit[1]);

                    return result1 + result2;
                }
                else
                {
                    return getString(numStr);
                }
            }

            public string getString(string str)
            {
                if (str.Length > 4)
                {
                    string[] secondSplit = new string[2];
                    secondSplit[0] = str.Substring(0, str.Length - 4);
                    secondSplit[1] = str.Substring(str.Length - 4, 4);
                    string result1 = getRe(secondSplit[0]);
                    string result2 = getRe(secondSplit[1]);
                    if (!secondSplit[0].Equals("0000"))
                    {
                        result1 += "萬";
                    }

                    return result1 + result2;
                }
                else
                {
                    return getRe(str);

                }
            }

            int[] value = { 1000, 100, 10 };

            public string getRe(string doWith)
            {
                char[] number = doWith.ToCharArray();
                int length = number.Length;
                string re = "";

                for (int i = 0; i < length; i++)
                {
                    switch (number[i])
                    {
                        case '0':

                            if (re.EndsWith("零"))
                            {
                                re += "";
                            }
                            else
                            {
                                re += "零";
                            }

                            break;
                        case '1':
                            re += "壹";
                            break;
                        case '2':
                            re += "贰";
                            break;
                        case '3':
                            re += "叁";
                            break;
                        case '4':
                            re += "肆";
                            break;
                        case '5':
                            re += "伍";
                            break;
                        case '6':
                            re += "陆";
                            break;
                        case '7':
                            re += "柒";
                            break;
                        case '8':
                            re += "捌";
                            break;
                        case '9':
                            re += "玖";
                            break;
                    }

                    int index = (int)Math.Pow(10, length - i - 1);
                    if (number[i].ToString() == "0")
                    {
                        index = -1;
                    }
                    switch (index)
                    {
                        case 1000:
                            re += "仟";
                            break;
                        case 100:
                            re += "佰";
                            break;
                        case 10:
                            re += "拾";
                            break;
                    }
                }

                if (re.EndsWith("零"))
                {
                    re = re.Substring(0, re.Length - 1);
                }
                return re;

            }
        }

        public class pdfJCD
        {
            public pdfJCD()
            {
                ArrPdfJCDList = new List<pdfJCDList>();
            }
            public string Entry_Id { get; set; }
            public string Warehouse_Id { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Day { get; set; }
            public string Out_Time { get; set; }
            public string Consign_Code_CK_Name { get; set; }
            public string Packing_Name { get; set; }
            public string End_Port { get; set; }
            public string Mask { get; set; }
            public string Remark { get; set; }
            public string Pieces_CK { get; set; }
            public string Weight_CK { get; set; }
            public string Volume_CK { get; set; }
            public virtual List<pdfJCDList> ArrPdfJCDList { get; set; }
        }

        public class pdfJCDList
        {
            public string CM_Length { get; set; }
            public string CM_Width { get; set; }
            public string CM_Height { get; set; }
            public string CM_Piece { get; set; }
        }

    }
}