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
    public class Bms_Bill_ApsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Bms_Bill_Ap>, Repository<Bms_Bill_Ap>>();
        //container.RegisterType<IBms_Bill_ApService, Bms_Bill_ApService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IBms_Bill_ApService _bms_Bill_ApService;
        private readonly IBms_Bill_ArService _bms_Bill_ArService;
        private readonly IFinanceService _financeService;
        private readonly ICostMoneyService _costMoneyService;
        private readonly IChangeOrderHistoryService _changeOrderHistoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/Bms_Bill_Aps";
        //
        private RedisHelp.RedisHelper ORedisHelper { get; set; }

        public Bms_Bill_ApsController(IBms_Bill_ApService bms_Bill_ApService, IBms_Bill_ArService bms_Bill_ArService, IChangeOrderHistoryService changeOrderHistoryService, IUnitOfWorkAsync unitOfWork)
        {
            _bms_Bill_ApService = bms_Bill_ApService;
            _bms_Bill_ArService = bms_Bill_ArService;
            _changeOrderHistoryService = changeOrderHistoryService;
            _financeService = (IFinanceService)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(FinanceService), "FinanceService");
            _costMoneyService = (ICostMoneyService)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(CostMoneyService), "CostMoneyService");
            _unitOfWork = unitOfWork;
            ORedisHelper = new RedisHelp.RedisHelper();
        }

        // GET: Bms_Bill_Aps/Index
        public ActionResult Index()
        {
            //var bms_bill_ap  = _bms_Bill_ApService.Queryable().AsQueryable();
            //return View(bms_bill_ap  );
            return View();
        }

        // Get :Bms_Bill_Aps/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var OPS_EntrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable();
            var Qbms_bill_ap = _bms_Bill_ApService.Query(new Bms_Bill_ApQuery().Withfilter(filters)).Select().AsQueryable();
            var datarows = from n in Qbms_bill_ap
                           join p in OPS_EntrustmentInforRep on n.Dzbh equals p.Operation_Id into p_tmp
                           from ptmp in p_tmp.DefaultIfEmpty()
                           select new
            {
                Id = n.Id,
                Dzbh = n.Dzbh,
                n.Ops_M_OrdId,
                n.Ops_H_OrdId,
                Line_No = n.Line_No,
                Bill_Type = n.Bill_Type,
                Bill_Account = n.Bill_Account,
                Bill_Account2 = n.Bill_Account2,
                Org_Bill_Account2 = n.Org_Bill_Account2,
                n.Bill_Amount,
                n.Bill_TaxRate,
                n.Bill_TaxRateType,
                n.Bill_HasTax,
                n.Bill_TaxAmount,
                n.Bill_AmountTaxTotal,
                Money_Code = n.Money_Code,
                Org_Money_Code = n.Org_Money_Code,
                Bill_Object_Id = n.Bill_Object_Id,
                Bill_Object_Name = n.Bill_Object_Name,
                Payway = n.Payway,
                Remark = n.Remark,
                Summary = n.Summary,
                Bill_Date = n.Bill_Date,
                AuditNo = n.AuditNo,
                AuditId = n.AuditId,
                AuditName = n.AuditName,
                AuditDate = n.AuditDate,
                AuditStatus = n.AuditStatus,
                n.Sumbmit_Status,
                n.Sumbmit_No,
                n.Sumbmit_Name,
                n.Sumbmit_Date,
                n.SignIn_Status,
                n.SignIn_No,
                n.SignIn_Name,
                n.SignIn_Date,
                n.Invoice_No,
                n.SellAccount_Status,
                n.SellAccount_Name,
                n.SellAccount_Date,
                Cancel_Id = n.Cancel_Id,
                Cancel_Name = n.Cancel_Name,
                Cancel_Date = n.Cancel_Date,
                n.Create_Status,
                n.Cancel_Status,
                n.Status,
                OperatingPoint = n.OperatingPoint,
                n.ADDID,
                n.ADDTS,
                n.ADDWHO,
                n.EDITID,
                n.EDITTS,
                n.EDITWHO,
                End_Port = ptmp.End_Port,
                Flight_Date_Want = ptmp.Flight_Date_Want,
                n.BillEDITID,
                n.BillEDITWHO,
                n.BillEDITTS
            };
            totalCount = datarows.Count();
            var QResult = datarows.OrderBy(sort, order).Skip((page - 1) * rows).Take(rows).ToList();

            #region 获取 缓存 数据

            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();//帐户
            var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);//枚举
            var ArrPARA_AirPort = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);//港口
            //var ArrPARA_CURR = (IEnumerable<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);//币种
            var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);//客商

            var QArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "PayWay");//支付方式

            var Q_Result = from n in QResult
                           join a in ArrAppUser on n.EDITWHO equals a.UserName into a_tmp
                           from atmp in a_tmp.DefaultIfEmpty()
                           join f in ArrAppUser on n.ADDWHO equals f.UserName into f_tmp
                           from ftmp in f_tmp.DefaultIfEmpty()
                           join m in ArrAppUser on n.BillEDITWHO equals m.UserName into m_tmp
                           from mtmp in m_tmp.DefaultIfEmpty()
                           join b in QArrBD_DEFDOC_LIST on n.Payway equals b.LISTNAME into b_tmp
                           from btmp in b_tmp.DefaultIfEmpty()
                           join c in ArrPARA_AirPort on n.End_Port equals c.PortCode into c_tmp
                           from ctmp in c_tmp.DefaultIfEmpty()
                           //join d in ArrPARA_CURR on n.Money_Code equals d.CURR_CODE into d_tmp
                           //from dtmp in d_tmp.DefaultIfEmpty()
                           join e in ArrCusBusInfo on n.Bill_Object_Id equals e.EnterpriseId into e_tmp
                           from etmp in e_tmp.DefaultIfEmpty()
                           select new
                           {
                               Id = n.Id,
                               Dzbh = n.Dzbh,
                               n.Ops_M_OrdId,
                               n.Ops_H_OrdId,
                               Line_No = n.Line_No,
                               Bill_Type = n.Bill_Type,
                               Bill_Account = n.Bill_Account == 0 ? "0" : Math.Round((decimal)n.Bill_Account, 2).ToString("#0.00"),
                               Bill_Account2 = n.Bill_Account2 == 0 ? "0" : Math.Round((decimal)n.Bill_Account2, 2).ToString("#0.00"),
                               Org_Bill_Account2 = n.Org_Bill_Account2 == 0 ? "0" : Math.Round((decimal)n.Org_Bill_Account2, 2).ToString("#0.00"),
                               Bill_Amount = n.Bill_Amount == 0 ? "0" : Math.Round((decimal)n.Bill_Amount, 2).ToString("#0.00"),
                               Bill_TaxAmount = n.Bill_TaxAmount == 0 ? "0" : Math.Round((decimal)n.Bill_TaxAmount, 2).ToString("#0.00"),
                               n.Bill_TaxRate,
                               n.Bill_TaxRateType,
                               n.Bill_HasTax,
                               Bill_AmountTaxTotal = n.Bill_AmountTaxTotal == 0 ? "0" : Math.Round((decimal)n.Bill_AmountTaxTotal, 2).ToString("#0.00"),
                               Money_Code = n.Money_Code,
                               //Money_CodeNAME = dtmp == null ? "" : dtmp.CURR_Name,
                               Org_Money_Code = n.Org_Money_Code,
                               Bill_Object_Id = n.Bill_Object_Id,
                               Bill_Object_IdNAME = etmp == null ? "" : etmp.EnterpriseName,
                               Bill_Object_Name = n.Bill_Object_Name,
                               Payway = n.Payway,
                               PaywayNAME = btmp == null ? "" : btmp.LISTNAME,
                               Remark = n.Remark,
                               Summary = n.Summary,
                               Bill_Date = n.Bill_Date,
                               AuditNo = n.AuditNo,
                               AuditId = n.AuditId,
                               AuditName = n.AuditName,
                               AuditDate = n.AuditDate,
                               AuditStatus = n.AuditStatus,
                               n.Sumbmit_Status,
                               n.Sumbmit_No,
                               n.Sumbmit_Name,
                               n.Sumbmit_Date,
                               n.SignIn_Status,
                               n.SignIn_No,
                               n.SignIn_Name,
                               n.SignIn_Date,
                               n.Invoice_No,
                               n.SellAccount_Status,
                               n.SellAccount_Name,
                               n.SellAccount_Date,
                               Cancel_Id = n.Cancel_Id,
                               Cancel_Name = n.Cancel_Name,
                               Cancel_Date = n.Cancel_Date,
                               n.Create_Status,
                               n.Cancel_Status,
                               n.Status,
                               OperatingPoint = n.OperatingPoint,
                               n.ADDID,
                               n.ADDTS,
                               n.ADDWHO,
                               ADDWHONAME = ftmp == null ? "" : ftmp.UserNameDesc,
                               n.EDITID,
                               n.EDITTS,
                               n.EDITWHO,
                               EDITWHONAME = atmp == null ? "" : atmp.UserNameDesc,
                               n.End_Port,
                               End_PortNAME = ctmp == null ? "" : ctmp.PortName,
                               n.Flight_Date_Want,
                               n.BillEDITID,
                               n.BillEDITTS,
                               n.BillEDITWHO,
                               BillEDITWHONAME = mtmp == null ? "" : mtmp.UserNameDesc
                           };

            #endregion

            var pagelist = new { total = totalCount, rows = Q_Result };

            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(Bms_Bill_ApChangeViewModel bms_bill_ap)
        {
            if (bms_bill_ap.updated != null)
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

                foreach (var updated in bms_bill_ap.updated)
                {
                    _bms_Bill_ApService.Update(updated);
                }
            }
            if (bms_bill_ap.deleted != null)
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

                var ArrDeltId = bms_bill_ap.deleted.Select(x => x.Id);
                var ArrDelt = _bms_Bill_ApService.Queryable().Where(x => ArrDeltId.Contains(x.Id)).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();
                if (ArrDelt.Any(x => x.AuditStatus < 0 || x.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess))
                {
                    //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
                    string ActionGuidName1 = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
                    string ActionGuid1 = ViewData[ActionGuidName1] == null ? "" : ViewData[ActionGuidName1].ToString();
                    return Json(new { Success = false, ErrMsg = "应付账单，部分数据已经审核，无法删除！", ActionGuidName = ActionGuidName1, ActionGuid = ActionGuid1 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Bms_Bill_Ap_DtlRep = _unitOfWork.Repository<Bms_Bill_Ap_Dtl>();
                    foreach (var deleted in ArrDelt)
                    {
                        if (deleted.ArrBms_Bill_Ap_Dtl != null && deleted.ArrBms_Bill_Ap_Dtl.Any())
                        {
                            var ArrdeltDtlId = deleted.ArrBms_Bill_Ap_Dtl.Select(x => x.Id).ToList();
                            foreach (var deltDtlId in ArrdeltDtlId)
                                Bms_Bill_Ap_DtlRep.Delete(deltDtlId);                        
                        }

                        #region 日志记录

                        var ContentApdelete = "应付账单 删除" + " Dzbh:" + deleted.Dzbh + " Account2:" + deleted.Bill_Account2 + " DelWho:" + Utility.CurrentAppUser.UserNameDesc;
                        _changeOrderHistoryService.InsertChangeOrdHistory(deleted.Ops_M_OrdId, "Bms_Bill_Ap", ChangeOrderHistory.EnumChangeType.Insert, ContentApdelete, 1, 0, 0);

                        #endregion

                        _bms_Bill_ApService.Delete(deleted.Id);
                    }
                }
            }
            if (bms_bill_ap.inserted != null)
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

                foreach (var inserted in bms_bill_ap.inserted)
                {
                    _bms_Bill_ApService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((bms_bill_ap.updated != null && bms_bill_ap.updated.Any()) ||
                (bms_bill_ap.deleted != null && bms_bill_ap.deleted.Any()) ||
                (bms_bill_ap.inserted != null && bms_bill_ap.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(bms_bill_ap);
                    if (bms_bill_ap.deleted != null && bms_bill_ap.deleted.Any())
                    {
                        foreach (var item in bms_bill_ap.deleted)
                        {
                            ORedisHelper.ListRightPush(Common.RedisKeyDelArAp_Dtl + ":" + DateTime.Now.ToString("yyyyMMdd"), "Bms_Bill_ArAp@|@" + item.Id.ToString() + "@|@0@|@" + DateTime.Now.ToString("yyyyMMdd HHmmss"));
                        }
                    }
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

        // GET: Bms_Bill_Aps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bms_Bill_Ap bms_Bill_Ap = _bms_Bill_ApService.Find(id);
            if (bms_Bill_Ap == null)
            {
                return HttpNotFound();
            }
            return View(bms_Bill_Ap);
        }

        // GET: Bms_Bill_Aps/Create
        public ActionResult Create()
        {
            Bms_Bill_Ap bms_Bill_Ap = new Bms_Bill_Ap();
            bms_Bill_Ap.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
            bms_Bill_Ap.Status = AirOutEnumType.UseStatusEnum.Enable;
            bms_Bill_Ap.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
            bms_Bill_Ap.Money_Code = "CNY";
            string NullLayOutStr = Request["NullLayOut"] ?? "";
            ViewBag.NullLayOut = string.IsNullOrEmpty(NullLayOutStr) ? false : true;
            //set default value
            return View(bms_Bill_Ap);
        }

        // POST: Bms_Bill_Aps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Dzbh,Line_No,Bill_Account,Bill_Account2,Money_Code,Bill_Object,Payway,Bill_Object_Id,Bill_Type,Bill_Date,Summary,Remark,Create_Status,Bill_Object_Name,Bgj_No,Org_Money_Code,Org_Bill_Account2,Ex_Rate,Hk_Line_No,Jk_Status,Jk_No,Nowent_Acc,Ishk_Status,Sbsx_Status,Dsdf_Status,AuditId,AuditName,AuditDate,AuditStatus,Cancel_Id,Cancel_Name,Cancel_Date,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Bms_Bill_Ap bms_Bill_Ap)
        {
            if (ModelState.IsValid)
            {
                _bms_Bill_ApService.Insert(bms_Bill_Ap);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Bms_Bill_Ap record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bms_Bill_Ap);
        }

        // GET: Bms_Bill_Aps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bms_Bill_Ap bms_Bill_Ap = _bms_Bill_ApService.Find(id);
            string NullLayOutStr = Request["NullLayOut"] ?? "";
            ViewBag.NullLayOut = string.IsNullOrEmpty(NullLayOutStr) ? false : true;
            if (bms_Bill_Ap == null)
            {
                return HttpNotFound();
            }
            return View(bms_Bill_Ap);
        }

        // POST: Bms_Bill_Aps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Dzbh,Line_No,Bill_Account,Bill_Account2,Money_Code,Bill_Object,Payway,Bill_Object_Id,Bill_Type,Bill_Date,Summary,Remark,Create_Status,Bill_Object_Name,Bgj_No,Org_Money_Code,Org_Bill_Account2,Ex_Rate,Hk_Line_No,Jk_Status,Jk_No,Nowent_Acc,Ishk_Status,Sbsx_Status,Dsdf_Status,AuditId,AuditName,AuditDate,AuditStatus,Cancel_Id,Cancel_Name,Cancel_Date,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Bms_Bill_Ap bms_Bill_Ap)
        {
            if (ModelState.IsValid)
            {
                bms_Bill_Ap.ObjectState = ObjectState.Modified;
                _bms_Bill_ApService.Update(bms_Bill_Ap);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Bms_Bill_Ap record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bms_Bill_Ap);
        }

        // GET: Bms_Bill_Aps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bms_Bill_Ap bms_Bill_Ap = _bms_Bill_ApService.Find(id);
            if (bms_Bill_Ap == null)
            {
                return HttpNotFound();
            }
            return View(bms_Bill_Ap);
        }

        // POST: Bms_Bill_Aps/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bms_Bill_Ap bms_Bill_Ap = _bms_Bill_ApService.Find(id);
            _bms_Bill_ApService.Delete(bms_Bill_Ap);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Bms_Bill_Ap record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "bms_bill_ap_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _bms_Bill_ApService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 应付账单 查看
        /// </summary>
        /// <param name="OPS_M_OrdId">总单ID</param>
        /// <param name="OPS_H_OrdId">分单ID</param>
        /// <returns></returns>
        public ActionResult BmsBillApView(int OPS_M_OrdId = 0, int OPS_H_OrdId = 0)
        {
            var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var OBmsBillAp_ArView = new BmsBillAp_ArView();
            OBmsBillAp_ArView.IsBmsBillAr = false;//应收账单
            //委托
            var OPS_EntrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>();
            if (OPS_M_OrdId > 0 || OPS_H_OrdId > 0)
            {
                if (OPS_M_OrdId > 0 && OPS_H_OrdId > 0)
                    ModelState.AddModelError("", "总单/分单 主键只能有一个");
                else
                {
                    var OPS_M_OrderRep = _unitOfWork.Repository<OPS_M_Order>();//主单
                    if (OPS_M_OrdId > 0)
                    {
                        var OPS_M_Order = OPS_M_OrderRep.Queryable().Where(x => x.Id == OPS_M_OrdId && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                        if (OPS_M_Order == null || OPS_M_Order.Id <= 0)
                        {
                            ModelState.AddModelError("", "总单数据不存在或已删除");
                        }
                        else
                        {
                            ViewData["OPS_BMS_Status"] = OPS_M_Order.OPS_BMS_Status;//是否 总单 分摊数据
                            //委托
                            var OOPS_EntrustmentInfor = OPS_EntrustmentInforRep.Queryable().Where(x => x.MBLId == OPS_M_Order.Id && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                            if (OOPS_EntrustmentInfor == null || OOPS_EntrustmentInfor.Id <= 0)
                            {
                                ModelState.AddModelError("", "委托数据不存在或已删除");
                            }
                            else
                            {
                                OBmsBillAp_ArView.Operation_Id = OOPS_EntrustmentInfor.Operation_Id;//业务编号 = 总单号
                                OBmsBillAp_ArView.OPS_M_Id = OPS_M_Order.Id;//总单Id
                                //OBmsBillAp_ArView.OPS_H_Id = OPS_M_Order.Id;//分单Id
                                OBmsBillAp_ArView.MBL = OOPS_EntrustmentInfor.MBL;//总单号
                                OBmsBillAp_ArView.HBL = OOPS_EntrustmentInfor.HBL;//分单号
                                OBmsBillAp_ArView.Airways_Code = OOPS_EntrustmentInfor.Airways_Code;//航空公司
                                if (!string.IsNullOrEmpty(OOPS_EntrustmentInfor.Airways_Code))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OOPS_EntrustmentInfor.Airways_Code);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBmsBillAp_ArView.Airways_CodeNAME = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//客户 = 主单发货人
                                    }
                                }
                                //OBmsBillAp_ArView.AuditDate = null;//审核时间
                                //OBmsBillAp_ArView.BG_Num;//报关单套数
                                //OBmsBillAp_ArView.BG_PageNum;//报关单张数
                                OBmsBillAp_ArView.Cancellation = (int)OOPS_EntrustmentInfor.Status > 0 ? false : true;//作废
                                OBmsBillAp_ArView.DestinationPort = OOPS_EntrustmentInfor.End_Port;//目的港
                                OBmsBillAp_ArView.Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want;//航班日期
                                OBmsBillAp_ArView.Flight_No = OOPS_EntrustmentInfor.Flight_No;//航班号
                                OBmsBillAp_ArView.Foreign_Proxy = OOPS_EntrustmentInfor.FWD_Code; //国外代理
                                if (!string.IsNullOrEmpty(OOPS_EntrustmentInfor.FWD_Code))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OOPS_EntrustmentInfor.FWD_Code);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBmsBillAp_ArView.Foreign_ProxyNAME = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//客户 = 主单发货人
                                    }
                                }
                                OBmsBillAp_ArView.Pay_Mode = OPS_M_Order.Pay_Mode_M;//付款方式
                                OBmsBillAp_ArView.Remark = OOPS_EntrustmentInfor.Remark;//结算备注
                                OBmsBillAp_ArView.Customer_Code = OPS_M_Order.Shipper_M;//客户 = 主单发货人
                                if (!string.IsNullOrEmpty(OPS_M_Order.Shipper_M))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OPS_M_Order.Shipper_M);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBmsBillAp_ArView.Customer_CodeNAME = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//客户 = 主单发货人
                                    }
                                }
                                OBmsBillAp_ArView.Settle_Code = OOPS_EntrustmentInfor.Carriage_Account_Code;//结算方 = 主单收货人
                                if (!string.IsNullOrEmpty(OOPS_EntrustmentInfor.Carriage_Account_Code))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OOPS_EntrustmentInfor.Carriage_Account_Code);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBmsBillAp_ArView.Settle_CodeNAME = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//结算方 = 主单收货人
                                    }
                                }
                                OBmsBillAp_ArView.Num = OOPS_EntrustmentInfor.Pieces_SK;//分单 -件数
                                OBmsBillAp_ArView.Settle_Weight = OOPS_EntrustmentInfor.Account_Weight_SK;//分单 -结算重量
                                OBmsBillAp_ArView.Volume = OOPS_EntrustmentInfor.Volume_SK;//分单 -体积
                                OBmsBillAp_ArView.Weight = OOPS_EntrustmentInfor.Account_Weight_DC;//总单 -结算重量

                                #region 应付数据 合计&毛利
                                //该票业务下 所有发票金额+D/N 金额，-委托C/N金额- 分摊金额

                                //应付 过滤 总单应付数据 !x.IsMBLJS && 
                                var ArrAp = _bms_Bill_ApService.Queryable().Where(x => x.Ops_M_OrdId == OPS_M_OrdId).ToList().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && !x.Cancel_Status);

                                //应收/应付 费用
                                var ArrArApFinance = ArrAp.Select(x => new Finance
                                {
                                    IsAr = false,
                                    Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want,
                                    Money_Code = x.Money_Code,
                                    Bill_Account2 = x.Bill_Account2,
                                    Id = x.Id
                                }).ToList();
                                if (ArrArApFinance != null && ArrArApFinance.Any())
                                {
                                    var ArrArBill_Account2 = _financeService.GetArApBillAccountByFlight_Date(ArrArApFinance);
                                    var ApTotalCNY = ArrArBill_Account2.Where(x => !x.IsAr).Sum(x => x.NewBill_Account2Total);//应收总计费用
                                    var TotalStr = string.Join(" ", ArrArBill_Account2.Where(x => !x.IsAr).Select(x => x.Money_Code + "：" + x.Bill_Account2Total + (x.Money_Code.ToUpper() != "CNY" ? (" 折合CNY：" + x.NewBill_Account2Total) : "")));
                                    TotalStr += " 合计CNY：" + ApTotalCNY;
                                    ViewData["TotalStr"] = TotalStr;
                                }

                                #endregion
                            }
                        }
                    }
                    if (OPS_H_OrdId > 0)
                    {
                        var OPS_H_OrderRep = _unitOfWork.Repository<OPS_H_Order>();
                        var OPS_H_Order = OPS_H_OrderRep.Queryable().Where(x => x.Id == OPS_H_OrdId && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                        if (OPS_H_Order == null || OPS_H_Order.Id <= 0)
                        {
                            ModelState.AddModelError("", "分单数据不存在或已删除");
                        }
                        else
                        {
                            //委托
                            var OOPS_EntrustmentInfor = OPS_EntrustmentInforRep.Queryable().Where(x => x.Operation_Id == OPS_H_Order.Operation_Id && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                            if (OOPS_EntrustmentInfor == null || OOPS_EntrustmentInfor.Id <= 0)
                            {
                                ModelState.AddModelError("", "委托数据不存在或已删除");
                            }
                            else
                            {
                                var OPS_M_Order = OPS_M_OrderRep.Queryable().Where(x => x.Id == OOPS_EntrustmentInfor.MBLId && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                                if (OPS_M_Order == null || OPS_M_Order.Id <= 0)
                                {
                                    OPS_M_Order = null;
                                }
                                OBmsBillAp_ArView.Operation_Id = OOPS_EntrustmentInfor.Operation_Id;//业务编号
                                OBmsBillAp_ArView.OPS_M_Id = OOPS_EntrustmentInfor.MBLId ?? 0;//总单Id
                                //OBmsBillAp_ArView.OPS_H_Id = OPS_H_Order.Id;//分单Id
                                OBmsBillAp_ArView.MBL = OOPS_EntrustmentInfor.MBL;//总单号
                                OBmsBillAp_ArView.HBL = OOPS_EntrustmentInfor.HBL;//分单号
                                OBmsBillAp_ArView.Airways_Code = OOPS_EntrustmentInfor.Airways_Code;//航空公司
                                if (!string.IsNullOrEmpty(OOPS_EntrustmentInfor.Airways_Code))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OOPS_EntrustmentInfor.Airways_Code);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBmsBillAp_ArView.Airways_CodeNAME = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//客户 = 主单发货人
                                    }
                                }
                                //OBmsBillAp_ArView.AuditDate = null;//审核时间
                                //OBmsBillAp_ArView.BG_Num;//报关单套数
                                //OBmsBillAp_ArView.BG_PageNum;//报关单张数
                                OBmsBillAp_ArView.Cancellation = (int)OOPS_EntrustmentInfor.Status > 0 ? false : true;//作废
                                OBmsBillAp_ArView.DestinationPort = OOPS_EntrustmentInfor.End_Port;//目的港
                                OBmsBillAp_ArView.Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want;//航班日期
                                OBmsBillAp_ArView.Flight_No = OOPS_EntrustmentInfor.Flight_No;//航班号
                                OBmsBillAp_ArView.Foreign_Proxy = OOPS_EntrustmentInfor.FWD_Code; //国外代理
                                if (!string.IsNullOrEmpty(OOPS_EntrustmentInfor.FWD_Code))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OOPS_EntrustmentInfor.FWD_Code);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBmsBillAp_ArView.Foreign_ProxyNAME = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//客户 = 主单发货人
                                    }
                                }
                                OBmsBillAp_ArView.Pay_Mode = OPS_M_Order == null ? OPS_H_Order.Pay_Mode_H : OPS_M_Order.Pay_Mode_M;//付款方式
                                OBmsBillAp_ArView.Remark = OOPS_EntrustmentInfor.Remark;//结算备注
                                OBmsBillAp_ArView.Customer_Code = OPS_H_Order.Shipper_H;//客户 = 分单发货人
                                if (!string.IsNullOrEmpty(OPS_M_Order.Shipper_M))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OPS_M_Order.Shipper_M);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBmsBillAp_ArView.Customer_CodeNAME = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//客户 = 主单发货人
                                    }
                                }
                                OBmsBillAp_ArView.Settle_Code = OOPS_EntrustmentInfor.Carriage_Account_Code;//结算方 = 分单收货人
                                if (!string.IsNullOrEmpty(OOPS_EntrustmentInfor.Carriage_Account_Code))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OOPS_EntrustmentInfor.Carriage_Account_Code);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBmsBillAp_ArView.Settle_CodeNAME = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//结算方 = 主单收货人
                                    }
                                }
                                OBmsBillAp_ArView.Num = OOPS_EntrustmentInfor.Pieces_SK;//分单 -件数
                                OBmsBillAp_ArView.Settle_Weight = OOPS_EntrustmentInfor.Account_Weight_SK;//分单 -结算重量
                                OBmsBillAp_ArView.Volume = OOPS_EntrustmentInfor.Volume_SK;//分单 -体积
                                OBmsBillAp_ArView.Weight = OOPS_EntrustmentInfor.Account_Weight_DC;//总单 -结算重量
                            }
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "总单/分单 主键是必需的");
            }
            var MasterView = View(OBmsBillAp_ArView);
            return MasterView;
        }

        /// <summary>
        /// 应付账单 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ListView()
        {
            return PartialView();
        }

        /// <summary>
        /// 应付账单和明细 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Dtl_ListView()
        {
            return PartialView();
        }

        /// <summary>
        /// 应付审核
        /// </summary>
        /// <param name="AuditId">应付账单Id</param>
        /// <param name="AuditState">审核状态（默认通过）</param>
        /// <returns></returns>
        public ActionResult Audit(List<int> ArrAuditId, int AuditState = (int)AirOutEnumType.AuditStatusEnum.AuditSuccess)
        {
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                ORetAudit = _bms_Bill_ApService.Audit(ArrAuditId, AuditState);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
            catch (Exception ex)
            {
                var errMsg = Common.GetExceptionMsg(ex);
                ORetAudit.Success = false;
                ORetAudit.ErrMsg = "审核出错：<br/>" + errMsg;
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
        }

        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="Ops_M_Id">主单Id</param>
        /// <returns></returns>
        public ActionResult SettleAccount(int Ops_M_Id = 0)
        {
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                var Bms_Bill_ApRep = _unitOfWork.Repository<Bms_Bill_Ap>();
                if (Ops_M_Id <= 0)
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "应付账单-Id 必须大于0！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                else
                {
                    var ArrBms_Bill_Ap = Bms_Bill_ApRep.Queryable().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Ops_M_OrdId == Ops_M_Id).Include(x=>x.ArrBms_Bill_Ap_Dtl).ToList();
                    if (ArrBms_Bill_Ap.Any())
                    {
                        if (ArrBms_Bill_Ap.Any(x =>( x.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet && x.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess )|| 
                            x.ArrBms_Bill_Ap_Dtl.Any(n => n.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess && n.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet && n.Status == AirOutEnumType.UseStatusEnum.Enable  )))
                        {
                            ORetAudit.Success = false;
                            ORetAudit.ErrMsg = "应付账单-" + ArrBms_Bill_Ap.FirstOrDefault().Dzbh + "，结算失败：该单已被审核！";
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                        }
                    }

                    #region 结算

                    var OOPS_M_Order = _unitOfWork.Repository<OPS_M_Order>().Queryable().Where(x => x.Id == Ops_M_Id).Include(x => x.OPS_EntrustmentInfors).ToList();
                    var RetMsg = _costMoneyService.AutoAddFee(OOPS_M_Order.FirstOrDefault());
                    if (!string.IsNullOrWhiteSpace(RetMsg))
                    {
                        ORetAudit.Success = false;
                        ORetAudit.ErrMsg = RetMsg;
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }
                    else
                    {
                        ORetAudit.Success = true;
                        ORetAudit.ErrMsg = "";
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                var errMsg = Common.GetExceptionMsg(ex);
                ORetAudit.Success = false;
                ORetAudit.ErrMsg = "结算出错：<br/>" + errMsg;
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="CopyId">应付账单Id</param>
        /// <returns></returns>
        public ActionResult Copy(int CopyId = 0)
        {
            int BmsBillApId = CopyId;
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                var Bms_Bill_ApRep = _unitOfWork.Repository<Bms_Bill_Ap>();
                if (BmsBillApId <= 0)
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "应付账单-Id 必须大于0！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                else
                {
                    var OBms_Bill_Ap = Bms_Bill_ApRep.Queryable().Where(x => x.Id == BmsBillApId && x.Status == AirOutEnumType.UseStatusEnum.Enable).Include(x => x.ArrBms_Bill_Ap_Dtl).FirstOrDefault();
                    if (OBms_Bill_Ap == null || OBms_Bill_Ap.Id <= 0)
                    {
                        ORetAudit.Success = false;
                        ORetAudit.ErrMsg = "找不到应付账单，请确定数据是否被其他人删除或作废！";
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }
                    else
                    {
                        var OBms_Bill_ApCopy = new Bms_Bill_Ap();
                        OBms_Bill_ApCopy.Dzbh = OBms_Bill_Ap.Dzbh;
                        OBms_Bill_ApCopy.MBL = OBms_Bill_Ap.MBL;
                        OBms_Bill_ApCopy.IsMBLJS = OBms_Bill_Ap.IsMBLJS;
                        OBms_Bill_ApCopy.Ops_M_OrdId = OBms_Bill_Ap.Ops_M_OrdId;
                        OBms_Bill_ApCopy.Status = AirOutEnumType.UseStatusEnum.Enable;
                        OBms_Bill_ApCopy.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                        OBms_Bill_ApCopy.Bill_HasTax = OBms_Bill_Ap.Bill_HasTax;
                        OBms_Bill_ApCopy.Bill_TaxRate = OBms_Bill_Ap.Bill_TaxRate;
                        OBms_Bill_ApCopy.Bill_TaxRateType = OBms_Bill_Ap.Bill_TaxRateType;
                        OBms_Bill_ApCopy.Bill_TaxAmount = 0;// OBms_Bill_Ap.Bill_TaxAmount;
                        OBms_Bill_ApCopy.Bill_AmountTaxTotal = 0;// OBms_Bill_Ap.Bill_AmountTaxTotal;
                        OBms_Bill_ApCopy.Bill_Type = OBms_Bill_Ap.Bill_Type;
                        OBms_Bill_ApCopy.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                        OBms_Bill_ApCopy.Bill_Date = DateTime.Now;
                        OBms_Bill_ApCopy.Bill_Account2 = 0;
                        OBms_Bill_ApCopy.Bill_Object_Id = OBms_Bill_Ap.Bill_Object_Id;
                        OBms_Bill_ApCopy.Bill_Object_Name = OBms_Bill_Ap.Bill_Object_Name;
                        OBms_Bill_ApCopy.Money_Code = OBms_Bill_Ap.Money_Code;
                        OBms_Bill_ApCopy.Org_Money_Code = OBms_Bill_Ap.Org_Money_Code;
                        OBms_Bill_ApCopy.Payway = OBms_Bill_Ap.Payway;
                        var Line_No = SequenceBuilder.NextBmsBillArLineNo(false, OBms_Bill_Ap.Dzbh);
                        OBms_Bill_ApCopy.Line_No = Convert.ToInt32(Line_No);
                        _bms_Bill_ApService.Insert(OBms_Bill_ApCopy);
                        //var ArrBms_Bill_Ap_Dtl = OBms_Bill_Ap.ArrBms_Bill_Ap_Dtl;
                        //if (ArrBms_Bill_Ap_Dtl != null && ArrBms_Bill_Ap_Dtl.Any())
                        //{
                        //    var Bms_Bill_Ap_DtlRep = _unitOfWork.Repository<Bms_Bill_Ap_Dtl>();
                        //    #region 明细数据

                        //    foreach (var itemDtl in ArrBms_Bill_Ap_Dtl)
                        //    {
                        //        var OBms_Bill_Ap_Dtl = new Bms_Bill_Ap_Dtl();
                        //        OBms_Bill_Ap_Dtl.Bms_Bill_Ap_Id = OBms_Bill_ApCopy.Id;
                        //        OBms_Bill_Ap_Dtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                        //        OBms_Bill_Ap_Dtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                        //        OBms_Bill_Ap_Dtl.Ops_M_OrdId = itemDtl.Ops_M_OrdId;
                        //        OBms_Bill_Ap_Dtl.Dzbh = itemDtl.Dzbh;
                        //        OBms_Bill_Ap_Dtl.Charge_Code = itemDtl.Charge_Code;
                        //        OBms_Bill_Ap_Dtl.Charge_Desc = itemDtl.Charge_Desc;
                        //        OBms_Bill_Ap_Dtl.Unitprice2 = itemDtl.Unitprice2;
                        //        OBms_Bill_Ap_Dtl.Qty = itemDtl.Qty;
                        //        OBms_Bill_Ap_Dtl.Account = itemDtl.Account;
                        //        OBms_Bill_Ap_Dtl.Account2 = itemDtl.Account2;
                        //        OBms_Bill_Ap_Dtl.Summary = itemDtl.Summary;
                        //        OBms_Bill_Ap_Dtl.Money_Code = itemDtl.Money_Code;
                        //        OBms_Bill_Ap_Dtl.Bill_Amount = itemDtl.Bill_Amount;
                        //        OBms_Bill_Ap_Dtl.Bill_AmountTaxTotal = itemDtl.Bill_AmountTaxTotal;
                        //        OBms_Bill_Ap_Dtl.Bill_HasTax = itemDtl.Bill_HasTax;
                        //        OBms_Bill_Ap_Dtl.Bill_TaxAmount = itemDtl.Bill_TaxAmount;
                        //        OBms_Bill_Ap_Dtl.Bill_TaxRate = itemDtl.Bill_TaxRate;
                        //        OBms_Bill_Ap_Dtl.Line_No = OBms_Bill_ApCopy.Line_No;
                        //        var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, OBms_Bill_Ap_Dtl.Dzbh + "_" + OBms_Bill_Ap_Dtl.Line_No, true);
                        //        OBms_Bill_Ap_Dtl.Line_Id = Convert.ToInt32(DtlLine_No);

                        //        Bms_Bill_Ap_DtlRep.Insert(OBms_Bill_Ap_Dtl);
                        //    }

                        //    #endregion
                        //}
                        #region 日志记录

                        var ContentCopy = "应付账单 复制" + " DZBH:" + OBms_Bill_Ap.Dzbh + " NewLine_No:" + Convert.ToInt32(Line_No) + " FromLine_No:" + OBms_Bill_Ap.Line_No;
                        _changeOrderHistoryService.InsertChangeOrdHistory(OBms_Bill_Ap.Ops_M_OrdId, "Bms_Bill_Ar", ChangeOrderHistory.EnumChangeType.Insert, ContentCopy, 0, 0, 1);

                        #endregion

                       
                        _unitOfWork.SaveChanges();
                        ORetAudit.Success = true;
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                var errMsg = Common.GetExceptionMsg(ex);
                ORetAudit.Success = false;
                ORetAudit.ErrMsg = "复制出错：<br/>" + errMsg;
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
        }

        /// <summary>
        /// 应付分摊
        /// </summary>
        /// <param name="MBL">总单号</param>
        /// <param name="YFFTId">应付分摊Id</param>
        /// <returns></returns>
        public ActionResult YFFT(string MBL, List<int> ArrYFFTId)
        {
            IEnumerable<int?> ArrBmsBillApId = ArrYFFTId.Select(x => (int?)x);
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                var OPS_EntrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>();
                var Bms_Bill_ApRep = _unitOfWork.Repository<Bms_Bill_Ap>();
                var Bms_Bill_Ap_DtlRep = _unitOfWork.Repository<Bms_Bill_Ap_Dtl>();
                var OPS_M_OrderRep = _unitOfWork.Repository<OPS_M_Order>();
                if (ArrBmsBillApId == null || ArrBmsBillApId.Any(x => x <= 0))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "应付账单-Id 必须大于0！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                else
                {
                    //删除已分摊的数据
                    var ArrDeltSQL = new List<string>() { 
                        "delete from Bms_Bill_Ap_Dtls t where exists(select 1 from Bms_Bill_Aps b where t.BMS_BILL_AP_ID = b.id and b.FTPARENTID in (" + string.Join(",", ArrBmsBillApId) + "))",
                        "delete from Bms_Bill_Aps t where t.FTPARENTID in (" + string.Join(",", ArrBmsBillApId) + ")"
                    };
                    SQLDALHelper.OracleHelper.ExecuteSqlTran(ArrDeltSQL, null);
                    //所有要分摊的数据
                    var ArrBmsBillAp = Bms_Bill_ApRep.Queryable().Where(x => ArrBmsBillApId.Contains(x.Id)).ToList();
                    var ArrBmsBillApDtl = Bms_Bill_Ap_DtlRep.Queryable().Where(x => ArrBmsBillApId.Contains(x.Bms_Bill_Ap_Id)).ToList();
                    var TotalBill_Account2 = ArrBmsBillApDtl.Sum(x => x.Account2);//合计所有数据总计
                    //找出所有 需要插入分摊的数据
                    var ArrOPS_EntrustmentInfor = OPS_EntrustmentInforRep.Queryable().Where(x => x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable && x.MBL == MBL).ToList();
                    var ArrOPS_M_Order = OPS_M_OrderRep.Queryable().Where(x => x.MBL == MBL).ToList();
                    var ArrOpsMId = ArrOPS_M_Order.Where(x => x.OPS_BMS_Status).Select(x => (int?)x.Id);//去除主单应付分摊 委托
                    ArrOPS_EntrustmentInfor = ArrOPS_EntrustmentInfor.Where(x => !ArrOpsMId.Contains(x.MBLId) && !x.Is_TG).ToList();
                    var Weight_SKTotal = ArrOPS_EntrustmentInfor.Sum(x => x.Weight_SK);//收款分单的毛重合计

                    if (ArrOPS_EntrustmentInfor.Any())
                    {
                        if (ArrOPS_EntrustmentInfor.Any(x => x.Weight_SK == null || x.Weight_SK <= 0))
                        {
                            ORetAudit.Success = false;
                            ORetAudit.ErrMsg = "委托数据，数据中 含有 收款分单-毛重 为 0的数据，无法分摊！";
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                        }
                    }
                    else
                    {
                        ORetAudit.Success = false;
                        ORetAudit.ErrMsg = "委托数据，被其他人删除或作废！";
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }

                    if (ArrBmsBillAp == null || !ArrBmsBillAp.Any() || ArrBmsBillAp.Count != ArrBmsBillApId.Count())
                    {
                        ORetAudit.Success = false;
                        ORetAudit.ErrMsg = "应付账单-有数据 被其他人删除或作废！";
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }

                    List<string> ArrErrMsg = new List<string>();//错误数据
                    //记录 应付数据
                    List<Bms_Bill_Ap> ArrBms_Bill_Ap = new List<Bms_Bill_Ap>();
                    var Num = 0;//主单ID，累加
                    var NumDtl = 0;//明细ID，累加
                    foreach (var OBms_Bill_Ap in ArrBmsBillAp)
                    {
                        #region 验证数据

                        if (OBms_Bill_Ap == null || OBms_Bill_Ap.Id <= 0)
                        {
                            ArrErrMsg.Add("找不到应付账单，请确定数据是否被其他人删除或作废！");
                            break;
                        }
                        else if (!OBms_Bill_Ap.IsMBLJS)
                        {
                            ArrErrMsg.Add("应付账单(" + OBms_Bill_Ap.MBL + "/" + OBms_Bill_Ap.Line_No + ")，不是应付分摊数据！");
                            break;
                        }
                        else if (OBms_Bill_Ap.Status != AirOutEnumType.UseStatusEnum.Enable)
                        {
                            ArrErrMsg.Add("应付账单(" + OBms_Bill_Ap.MBL + "/" + OBms_Bill_Ap.Line_No + ")，已被其他人删除或作废！");
                            break;
                        }
                        //else if (OBms_Bill_Ap.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess)
                        //{
                        //    ArrErrMsg.Add("应付账单(" + OBms_Bill_Ap.MBL + "/" + OBms_Bill_Ap.Line_No + ")，未审核！");
                        //    break;
                        //}

                        #endregion

                        #region 逻辑处理

                        //筛选对应的明细数据
                        var QArrBmsBillApDtl = ArrBmsBillApDtl.Where(x => x.Bms_Bill_Ap_Id == OBms_Bill_Ap.Id).ToList();
                        if (QArrBmsBillApDtl == null || !QArrBmsBillApDtl.Any())
                        {
                            ArrErrMsg.Add("应付账单(" + OBms_Bill_Ap.MBL + "/" + OBms_Bill_Ap.Line_No + ")明细信息，不存在！");
                            break;
                        }
                        var DtlNum = QArrBmsBillApDtl.Count;//明细总数
                        var Bill_Account2 = QArrBmsBillApDtl.Sum(x => x.Account2);//计算下合计，防止主单上的数据不准
                        //记录总的委托数
                        var EttInforNum = ArrOPS_EntrustmentInfor.Count;
                        for (int i = 0; i < EttInforNum; i++)
                        {
                            Num++;//主单ID，累加
                            var item = ArrOPS_EntrustmentInfor[i];
                            //if (!ArrOPS_M_Order.Any(x => x.OPS_BMS_Status && x.Id == item.MBLId))
                            //    continue;//跳过主单应付分摊 委托

                            #region 分摊账单

                            var OBms_Bill_ApCopy = new Bms_Bill_Ap();
                            var ArrBms_Bill_Ap_Dtl = new List<Bms_Bill_Ap_Dtl>();
                            var BL = (decimal)item.Weight_SK / (decimal)Weight_SKTotal;//分摊 百分比
                            decimal FTTotal = 0;//分摊价
                            //最后一个 = 总价 - 已分摊的价
                            if ((i + 1) == EttInforNum)
                                FTTotal = Bill_Account2 - ArrBms_Bill_Ap.Where(x => x.FTParentId == OBms_Bill_Ap.Id).Sum(x => x.Bill_Account2);
                            else
                                FTTotal = Math.Round(BL * Bill_Account2, 2);

                            OBms_Bill_ApCopy.IsMBLJS = false;//是否总单结算 分摊
                            OBms_Bill_ApCopy.FTParentId = OBms_Bill_Ap.Id;//记录 哪个 账单分摊下来的
                            OBms_Bill_ApCopy.Dzbh = item.Operation_Id;
                            OBms_Bill_ApCopy.Ops_M_OrdId = (int)item.MBLId;
                            OBms_Bill_ApCopy.Status = AirOutEnumType.UseStatusEnum.Enable;
                            OBms_Bill_ApCopy.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                            OBms_Bill_ApCopy.Bill_Type = "FTYF";
                            OBms_Bill_ApCopy.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                            OBms_Bill_ApCopy.Bill_Date = DateTime.Now;
                            OBms_Bill_ApCopy.Bill_Account2 = FTTotal;
                            OBms_Bill_ApCopy.Bill_Object_Id = OBms_Bill_Ap.Bill_Object_Id;
                            OBms_Bill_ApCopy.Bill_Object_Name = OBms_Bill_Ap.Bill_Object_Name;
                            OBms_Bill_ApCopy.Money_Code = OBms_Bill_Ap.Money_Code;
                            OBms_Bill_ApCopy.Org_Money_Code = OBms_Bill_Ap.Org_Money_Code;
                            OBms_Bill_ApCopy.Payway = OBms_Bill_Ap.Payway;
                            OBms_Bill_ApCopy.Bill_HasTax = OBms_Bill_Ap.Bill_HasTax;
                            OBms_Bill_ApCopy.Bill_TaxRate = OBms_Bill_Ap.Bill_TaxRate;
                            OBms_Bill_ApCopy.Bill_TaxRateType = OBms_Bill_Ap.Bill_TaxRateType;
                            #region 计算 价 税 价税合计

                            var OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_ApCopy.Bill_HasTax, OBms_Bill_ApCopy.Bill_TaxRate, FTTotal);
                            if (OCalcTaxRate.Success)
                            {
                                //价（实际金额）
                                OBms_Bill_Ap.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                //税金 （实际金额 * 税率）
                                OBms_Bill_Ap.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                //价税合计 (价+税金)
                                OBms_Bill_Ap.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                            }
                            else
                            {
                                ArrErrMsg.Add("应付账单(" + OBms_Bill_Ap.MBL + "/" + OBms_Bill_Ap.Line_No + ")信息，计算价/税/价税合计出错！！");
                                break;
                            }

                            #endregion
                            var Line_No = SequenceBuilder.NextBmsBillArLineNo(false, item.Operation_Id);
                            OBms_Bill_ApCopy.Line_No = Convert.ToInt32(Line_No);
                            OBms_Bill_ApCopy.Id = Num;

                            OBms_Bill_ApCopy.ObjectState = ObjectState.Added;
                            _bms_Bill_ApService.Insert(OBms_Bill_ApCopy);

                            for (int n = 0; n < DtlNum; n++)
                            {
                                NumDtl++;
                                #region 费用明细

                                var itemDtl = QArrBmsBillApDtl[n];
                                var OApDtl = new Bms_Bill_Ap_Dtl();
                                OApDtl.Status = AirOutEnumType.UseStatusEnum.Enable;

                                OApDtl.Unitprice2 = 0;
                                OApDtl.Qty = 0;
                                if ((n + 1) == DtlNum)
                                    OApDtl.Account2 = FTTotal - ArrBms_Bill_Ap_Dtl.Sum(x => x.Account2);
                                else
                                    OApDtl.Account2 = Math.Round(BL * itemDtl.Account2, 2);

                                OApDtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                                OApDtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                                OApDtl.Dzbh = item.Operation_Id;
                                OApDtl.Ops_M_OrdId = (int)item.MBLId;
                                OApDtl.Charge_Code = itemDtl.Charge_Code;
                                OApDtl.Charge_Desc = itemDtl.Charge_Desc;
                                OApDtl.Summary = itemDtl.Summary;
                                OApDtl.Id = NumDtl;
                                OApDtl.Bms_Bill_Ap_Id = OBms_Bill_ApCopy.Id;
                                OApDtl.Line_No = OBms_Bill_ApCopy.Line_No;
                                var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, OBms_Bill_ApCopy.Dzbh + "_" + OBms_Bill_ApCopy.Line_No, true);
                                OApDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                OApDtl.Bill_HasTax = OBms_Bill_ApCopy.Bill_HasTax;
                                OApDtl.Bill_TaxRate = OBms_Bill_ApCopy.Bill_TaxRate;

                                #region 计算 价 税 价税合计

                                var O_CalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_ApCopy.Bill_HasTax, OBms_Bill_ApCopy.Bill_TaxRate, OApDtl.Account2);
                                if (O_CalcTaxRate.Success)
                                {
                                    //价（实际金额）
                                    OApDtl.Bill_Amount = O_CalcTaxRate.Bill_Amount;
                                    //税金 （实际金额 * 税率）
                                    OApDtl.Bill_TaxAmount = O_CalcTaxRate.Bill_TaxAmount;
                                    //价税合计 (价+税金)
                                    OApDtl.Bill_AmountTaxTotal = O_CalcTaxRate.Bill_AmountTaxTotal;
                                }
                                else
                                {
                                    ArrErrMsg.Add("应付账单明细(" + OBms_Bill_Ap.MBL + "/" + OBms_Bill_Ap.Line_No + ")明细信息，计算价/税/价税合计出错！！");
                                    break;
                                }

                                #endregion


                                OApDtl.ObjectState = ObjectState.Added;
                                Bms_Bill_Ap_DtlRep.Insert(OApDtl);

                                ArrBms_Bill_Ap_Dtl.Add(OApDtl);

                                #region

                                var ContentFTDEL = "应付分摊 明细" + " DZBH:" + OApDtl.Dzbh + " Line_No:" + OBms_Bill_ApCopy.Line_No + " Line_Id:" + OApDtl.Id + " Account2:" + OApDtl.Account2 + " FTParentId:" + OBms_Bill_ApCopy.FTParentId;
                                _changeOrderHistoryService.InsertChangeOrdHistory(OBms_Bill_ApCopy.Ops_M_OrdId, "Bms_Bill_Ap", ChangeOrderHistory.EnumChangeType.Insert, ContentFTDEL, 0, 0, 1);

                                #endregion

                                #endregion
                            }
                            OBms_Bill_ApCopy.ArrBms_Bill_Ap_Dtl = ArrBms_Bill_Ap_Dtl;
                            ArrBms_Bill_Ap.Add(OBms_Bill_ApCopy);

                            #region

                            var ContentFTBT = "应付分摊 表头" + " DZBH:" + OBms_Bill_ApCopy.Dzbh + " Line_No:" + OBms_Bill_ApCopy.Line_No + " Account2:" + OBms_Bill_ApCopy.Bill_Account2 + " FTParentId:" + OBms_Bill_ApCopy.FTParentId;
                            _changeOrderHistoryService.InsertChangeOrdHistory(OBms_Bill_ApCopy.Ops_M_OrdId, "Bms_Bill_Ap", ChangeOrderHistory.EnumChangeType.Insert, ContentFTBT, 0, 0, 1);

                            #endregion

                            #endregion
                        }

                        #endregion
                    }
                    if (ArrErrMsg.Any())
                    {
                        var errMsg = string.Join(",", ArrErrMsg);
                        ORetAudit.Success = false;
                        ORetAudit.ErrMsg = "应付分摊出错：<br/>" + errMsg;
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }
                    else
                    {
                        #region 插入数据

                        //var Num = 0;
                        //foreach (var item in ArrBms_Bill_Ap)
                        //{
                        //    if (item.ArrBms_Bill_Ap_Dtl != null && item.ArrBms_Bill_Ap_Dtl.Any())
                        //    {
                        //        Num++;
                        //        item.Id = Num;
                        //        _bms_Bill_ApService.Insert(item);
                        //        var NumDtl = 0;
                        //        foreach (var itemDtl in item.ArrBms_Bill_Ap_Dtl)
                        //        {
                        //            NumDtl++;
                        //            itemDtl.Id = NumDtl;
                        //            itemDtl.Bms_Bill_Ap_Id = item.Id;
                        //            itemDtl.Line_No = item.Line_No;
                        //            var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, item.Dzbh + "_" + item.Line_No, true);
                        //            itemDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                        //            itemDtl.Bill_HasTax = item.Bill_HasTax;
                        //            itemDtl.Bill_TaxRate = item.Bill_TaxRate;
                        //            itemDtl.Bill_Amount = item.Bill_TaxRate;
                        //            itemDtl.Bill_TaxAmount = item.Bill_TaxRate;
                        //            itemDtl.Bill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                        //            Bms_Bill_Ap_DtlRep.Insert(itemDtl);
                        //        }
                        //    }
                        //}

                        #endregion
                        _unitOfWork.SaveChanges();
                        ORetAudit.Success = true;
                        ORetAudit.ErrMsg = "";
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                var errMsg = Common.GetExceptionMsg(ex);
                ORetAudit.Success = false;
                ORetAudit.ErrMsg = "应付分摊出错：<br/>" + errMsg;
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
        }

        /// <summary>
        /// 账务操作
        /// </summary>
        /// <returns></returns>
        public ActionResult Finance()
        {
            return View();
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
