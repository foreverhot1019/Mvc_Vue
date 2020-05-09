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
using System.Threading.Tasks;

namespace AirOut.Web.Controllers
{
    public class Bms_Bill_ArsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Bms_Bill_Ar>, Repository<Bms_Bill_Ar>>();
        //container.RegisterType<IBms_Bill_ArService, Bms_Bill_ArService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IBms_Bill_ArService _bms_Bill_ArService;
        private readonly IBms_Bill_ApService _bms_Bill_ApService;
        private readonly IFinanceService _financeService;
        private readonly ICustomerQuotedPriceService _customerQuotedPriceService;
        private readonly IChangeOrderHistoryService _changeOrderHistoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/Bms_Bill_Ars";
        //
        private RedisHelp.RedisHelper ORedisHelper { get; set; }

        public Bms_Bill_ArsController(IBms_Bill_ArService bms_Bill_ArService, IChangeOrderHistoryService changeOrderHistoryService, IUnitOfWorkAsync unitOfWork)
        {
            _bms_Bill_ArService = bms_Bill_ArService;
            _changeOrderHistoryService = changeOrderHistoryService;
            _unitOfWork = unitOfWork;
            _bms_Bill_ApService = (IBms_Bill_ApService)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(Bms_Bill_ApService), "Bms_Bill_ApService");
            _financeService = (IFinanceService)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(FinanceService), "FinanceService");
            _customerQuotedPriceService = (ICustomerQuotedPriceService)AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(CustomerQuotedPriceService), "CustomerQuotedPriceService");
            ORedisHelper = new RedisHelp.RedisHelper();
        }

        // GET: Bms_Bill_Ars/Index
        public ActionResult Index()
        {
            //var bms_bill_ar  = _bms_Bill_ArService.Queryable().AsQueryable();
            //return View(bms_bill_ar  );
            return View();
        }

        // Get :Bms_Bill_Ars/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var bms_bill_ar = _bms_Bill_ArService.Query(new Bms_Bill_ArQuery().Withfilter(filters)).Select().AsQueryable();
            var datarows = bms_bill_ar.Select(n => new
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
                n.Invoice_No,
                n.Invoice_Status,
                n.Invoice_Desc,
                n.Invoice_Remark,
                n.Invoice_Name,
                n.Invoice_Date,
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
                n.BillEDITID,
                n.BillEDITWHO,
                n.BillEDITTS
            });
            totalCount = datarows.Count();
            var QResult = datarows.OrderBy(sort, order).Skip((page - 1) * rows).Take(rows).ToList();

            #region 获取 缓存 数据

            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();//帐户
            var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);//枚举
            //var ArrPARA_CURR = (IEnumerable<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);//币种
            var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);//客商
            var QArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "PayWay");//支付方式
            var Q_Result = from n in QResult
                           join a in ArrAppUser on n.EDITWHO equals a.UserName into a_tmp
                           from atmp in a_tmp.DefaultIfEmpty()
                           join f in ArrAppUser on n.ADDWHO equals f.UserName into f_tmp
                           from ftmp in f_tmp.DefaultIfEmpty()
                           join c in ArrAppUser on n.BillEDITWHO equals c.UserName into c_tmp
                           from ctmp in f_tmp.DefaultIfEmpty()
                           join b in QArrBD_DEFDOC_LIST on n.Payway equals b.LISTNAME into b_tmp
                           from btmp in b_tmp.DefaultIfEmpty()
                           //join d in ArrPARA_CURR on n.Money_Code equals d.CURR_CODE into d_tmp
                           //from dtmp in d_tmp.DefaultIfEmpty()
                           join e in ArrCusBusInfo on n.Bill_Object_Id equals e.EnterpriseId into e_tmp
                           from etmp in e_tmp.DefaultIfEmpty()
                           select new
                           {
                               n.Id,
                               n.Dzbh,
                               n.Ops_M_OrdId,
                               n.Ops_H_OrdId,
                               n.Line_No,
                               n.Bill_Type,
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
                               n.Bill_Date,
                               n.AuditNo,
                               n.AuditId,
                               n.AuditName,
                               n.AuditDate,
                               n.AuditStatus,
                               n.Sumbmit_Status,
                               n.Sumbmit_No,
                               n.Sumbmit_Name,
                               n.Sumbmit_Date,
                               n.Invoice_No,
                               n.Invoice_Status,
                               n.Invoice_Desc,
                               n.Invoice_Remark,
                               n.Invoice_Name,
                               n.Invoice_Date,
                               n.SellAccount_Status,
                               n.SellAccount_Name,
                               n.SellAccount_Date,
                               n.Cancel_Id,
                               n.Cancel_Name,
                               n.Cancel_Date,
                               n.Create_Status,
                               n.Cancel_Status,
                               n.Status,
                               n.OperatingPoint,
                               n.ADDID,
                               n.ADDTS,
                               n.ADDWHO,
                               ADDWHONAME = ftmp == null ? "" : ftmp.UserNameDesc,
                               n.EDITID,
                               n.EDITTS,
                               n.EDITWHO,
                               EDITWHONAME = atmp == null ? "" : atmp.UserNameDesc,
                               n.BillEDITID,
                               n.BillEDITTS,
                               n.BillEDITWHO,
                               BillEDITWHONAME = ctmp == null ? "" : ctmp.UserNameDesc
                           };

            #endregion

            var pagelist = new { total = totalCount, rows = Q_Result };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(Bms_Bill_ArChangeViewModel bms_bill_ar)
        {
            if (bms_bill_ar.updated != null)
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

                foreach (var updated in bms_bill_ar.updated)
                {
                    _bms_Bill_ArService.Update(updated);
                }
            }
            if (bms_bill_ar.deleted != null)
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

                var ArrDeltId = bms_bill_ar.deleted.Select(x => x.Id);
                var ArrDelt = _bms_Bill_ArService.Queryable().Where(x => ArrDeltId.Contains(x.Id)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                if (ArrDelt.Any(x => x.AuditStatus < 0 || x.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess))
                {
                    //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
                    string ActionGuidName1 = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
                    string ActionGuid1 = ViewData[ActionGuidName1] == null ? "" : ViewData[ActionGuidName1].ToString();
                    return Json(new { Success = false, ErrMsg = "应收账单，部分数据已经审核，无法删除！", ActionGuidName = ActionGuidName1, ActionGuid = ActionGuid1 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Bms_Bill_Ar_DtlRep = _unitOfWork.Repository<Bms_Bill_Ar_Dtl>();
                    foreach (var deleted in ArrDelt)
                    {
                        if (deleted.ArrBms_Bill_Ar_Dtl != null && deleted.ArrBms_Bill_Ar_Dtl.Any())
                        {
                            var ArrdeltDtlId = deleted.ArrBms_Bill_Ar_Dtl.Select(x => x.Id).ToList();
                            foreach (var deltDtlId in ArrdeltDtlId)
                                Bms_Bill_Ar_DtlRep.Delete(deltDtlId);
                        }
                        _bms_Bill_ArService.Delete(deleted.Id);
                    }
                }
            }
            if (bms_bill_ar.inserted != null)
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

                foreach (var inserted in bms_bill_ar.inserted)
                {
                    _bms_Bill_ArService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((bms_bill_ar.updated != null && bms_bill_ar.updated.Any()) ||
                (bms_bill_ar.deleted != null && bms_bill_ar.deleted.Any()) ||
                (bms_bill_ar.inserted != null && bms_bill_ar.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(bms_bill_ar);
                    if (bms_bill_ar.deleted != null && bms_bill_ar.deleted.Any())
                    {
                        foreach(var item in bms_bill_ar.deleted){
                            ORedisHelper.ListRightPush(Common.RedisKeyDelArAp_Dtl + ":" + DateTime.Now.ToString("yyyyMMdd"), "Bms_Bill_ArAp@|@" + item.Id.ToString() + "@|@1@|@" + DateTime.Now.ToString("yyyyMMdd HHmmss"));
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

        // GET: Bms_Bill_Ars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bms_Bill_Ar bms_Bill_Ar = _bms_Bill_ArService.Find(id);
            if (bms_Bill_Ar == null)
            {
                return HttpNotFound();
            }
            return View(bms_Bill_Ar);
        }

        // GET: Bms_Bill_Ars/Create
        public ActionResult Create()
        {
            Bms_Bill_Ar bms_Bill_Ar = SetDefaultBms_Bill_Ar();
            string NullLayOutStr = Request["NullLayOut"] ?? "";
            ViewBag.NullLayOut = string.IsNullOrEmpty(NullLayOutStr) ? false : true;
            //set default value
            return View(bms_Bill_Ar);
        }

        // POST: Bms_Bill_Ars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[Bind(Include = "Id,Dzbh,Line_No,Bill_Account,Bill_Account2,Money_Code,Bill_Object,Payway,Bill_Object_Id,Bill_Type,Bill_Date,Summary,Remark,Create_Status,Bill_Object_Name,Bill_Id,Dk_Operation_Id,Bgj_No,Org_Money_Code,Org_Bill_Account2,Ex_Rate,Hk_Line_No,Jk_Status,Jk_No,Nowent_Acc,Ishk_Status,Sbsx_Status,Dsdf_Status,AuditId,AuditName,AuditDate,AuditStatus,Cancel_Id,Cancel_Name,Cancel_Date,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")]
        public ActionResult Create(Bms_Bill_Ar bms_Bill_Ar)
        {
            if (ModelState.IsValid)
            {
                _bms_Bill_ArService.Insert(bms_Bill_Ar);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Bms_Bill_Ar record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bms_Bill_Ar);
        }

        // GET: Bms_Bill_Ars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bms_Bill_Ar bms_Bill_Ar = _bms_Bill_ArService.Find(id);
            string NullLayOutStr = Request["NullLayOut"] ?? "";
            ViewBag.NullLayOut = string.IsNullOrEmpty(NullLayOutStr) ? false : true;
            if (bms_Bill_Ar == null)
            {
                return HttpNotFound();
            }
            return View(bms_Bill_Ar);
        }

        // POST: Bms_Bill_Ars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Bms_Bill_Ar bms_Bill_Ar)
        {
            if (ModelState.IsValid)
            {
                bms_Bill_Ar.ObjectState = ObjectState.Modified;
                _bms_Bill_ArService.Update(bms_Bill_Ar);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Bms_Bill_Ar record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bms_Bill_Ar);
        }

        // GET: Bms_Bill_Ars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bms_Bill_Ar bms_Bill_Ar = _bms_Bill_ArService.Find(id);
            if (bms_Bill_Ar == null)
            {
                return HttpNotFound();
            }
            return View(bms_Bill_Ar);
        }

        // POST: Bms_Bill_Ars/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bms_Bill_Ar bms_Bill_Ar = _bms_Bill_ArService.Find(id);
            _bms_Bill_ArService.Delete(bms_Bill_Ar);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Bms_Bill_Ar record");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 应收账单 查看
        /// </summary>
        /// <param name="OPS_M_OrdId">总单ID</param>
        /// <param name="OPS_H_OrdId">分单ID</param>
        /// <returns></returns>
        public ActionResult BmsBillArView(int OPS_M_OrdId = 0, int OPS_H_OrdId = 0)
        {
            var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var OBmsBillAp_ArView = new BmsBillAp_ArView();
            OBmsBillAp_ArView.IsBmsBillAr = true;//应收账单
            //委托
            var OPS_EntrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>();
            var OPS_M_OrderRep = _unitOfWork.Repository<OPS_M_Order>();//主单
            if (OPS_M_OrdId > 0 || OPS_H_OrdId > 0)
            {
                if (OPS_M_OrdId > 0 && OPS_H_OrdId > 0)
                    ModelState.AddModelError("", "总单/分单 主键只能有一个");
                else
                {
                    if (OPS_M_OrdId > 0)
                    {
                        var OPS_M_Order = OPS_M_OrderRep.Queryable().Where(x => x.Id == OPS_M_OrdId && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                        if (OPS_M_Order == null || OPS_M_Order.Id <= 0)
                        {
                            ModelState.AddModelError("", "总单数据不存在或已删除");
                        }
                        else if (OPS_M_Order.OPS_BMS_Status)
                        {
                            ModelState.AddModelError("", "分摊数据，无法做应收的任何操作");
                        }
                        else
                        {
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
                                OBmsBillAp_ArView.Num = OOPS_EntrustmentInfor.Pieces_SK;//件数
                                OBmsBillAp_ArView.Settle_Weight = OOPS_EntrustmentInfor.Account_Weight_SK;//分单 -结算重量
                                OBmsBillAp_ArView.Volume = OOPS_EntrustmentInfor.Volume_SK;//体积
                                OBmsBillAp_ArView.Weight = OOPS_EntrustmentInfor.Account_Weight_DC;//总单 -结算重量

                                #region 应收数据 合计&毛利
                                //该票业务下 所有发票金额+D/N 金额，-委托C/N金额- 分摊金额

                                var ArrAr = _bms_Bill_ArService.Queryable().Where(x => x.Ops_M_OrdId == OPS_M_OrdId).ToList().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && !x.Cancel_Status);
                                //应付 过滤 总单应付数据 !x.IsMBLJS && 
                                var ArrAp = _bms_Bill_ApService.Queryable().Where(x => x.Ops_M_OrdId == OPS_M_OrdId).ToList().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && !x.Cancel_Status);

                                //应收/应付 费用
                                var ArrArApFinance = ArrAr.Select(x => new Finance
                                {
                                    IsAr = true,
                                    Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want,
                                    Money_Code = x.Money_Code,
                                    Bill_Account2 = x.Bill_Account2,
                                    Id = x.Id
                                }).Concat(ArrAp.Select(x => new Finance
                                {
                                    IsAr = false,
                                    Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want,
                                    Money_Code = x.Money_Code,
                                    Bill_Account2 = x.Bill_Account2,
                                    Id = x.Id
                                })).ToList();
                                if (ArrArApFinance != null && ArrArApFinance.Any())
                                {
                                    var ArrArBill_Account2 = _financeService.GetArApBillAccountByFlight_Date(ArrArApFinance);
                                    var ArTotalCNY = ArrArBill_Account2.Where(x => x.IsAr).Sum(x => x.NewBill_Account2Total);//应收总计费用
                                    var ApTotalCNY = ArrArBill_Account2.Where(x => !x.IsAr).Sum(x => x.NewBill_Account2Total);//应收总计费用
                                    var TotalStr = string.Join(" ", ArrArBill_Account2.Where(x => x.IsAr).Select(x => x.Money_Code + "：" + x.Bill_Account2Total + (x.Money_Code.ToUpper() != "CNY" ? (" 折合CNY：" + x.NewBill_Account2Total) : "")));
                                    TotalStr += " 合计CNY：" + ArTotalCNY;
                                    TotalStr += " 毛利：" + Math.Round(ArTotalCNY - ApTotalCNY, 2);

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
                                OBmsBillAp_ArView.MBL = OOPS_EntrustmentInfor.MBL; //总单号
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
                                if (!string.IsNullOrEmpty(OPS_H_Order.Shipper_H))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OPS_H_Order.Shipper_H);
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
                                OBmsBillAp_ArView.Num = OOPS_EntrustmentInfor.Pieces_SK;//件数
                                OBmsBillAp_ArView.Settle_Weight = OOPS_EntrustmentInfor.Account_Weight_SK;//分单 -结算重量
                                OBmsBillAp_ArView.Volume = OOPS_EntrustmentInfor.Volume_SK;//体积
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
        /// 应收账单 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ListView()
        {
            return PartialView();
        }

        /// <summary>
        /// 应收账单 和明细 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Dtl_ListView()
        {
            return PartialView();
        }

        /// <summary>
        /// 应收审核
        /// </summary>
        /// <param name="AuditId">应收账单Id</param>
        /// <param name="AuditState">审核状态（默认通过）</param>
        /// <returns></returns>
        public ActionResult Audit(List<int> ArrAuditId, int AuditState = (int)AirOutEnumType.AuditStatusEnum.AuditSuccess)
        {
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                ORetAudit = _bms_Bill_ArService.Audit(ArrAuditId, AuditState);
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
                var Oreq = Request;
                var Bms_Bill_ArRep = _unitOfWork.Repository<Bms_Bill_Ar>();
                if (Ops_M_Id <= 0)
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "应收账单-Id 必须大于0！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                else
                {
                    var ArrBms_Bill_Ar = Bms_Bill_ArRep.Queryable().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Ops_M_OrdId == Ops_M_Id).Include(x=>x.ArrBms_Bill_Ar_Dtl).ToList();
                    if (ArrBms_Bill_Ar.Any())
                    {
                        if (ArrBms_Bill_Ar.Any(x =>( x.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet && x.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess )|| 
                            x.ArrBms_Bill_Ar_Dtl.Any(n => n.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess && n.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet && n.Status == AirOutEnumType.UseStatusEnum.Enable  )))
                        {
                            ORetAudit.Success = false;
                            ORetAudit.ErrMsg = "应收账单-" + ArrBms_Bill_Ar.FirstOrDefault().Dzbh + "，结算失败：该单已被审核！";
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                        }
                    }
                    
                    #region 结算

                    var OOPS_M_Order = _unitOfWork.Repository<OPS_M_Order>().Queryable().Where(x => x.Id == Ops_M_Id).Include(x => x.OPS_EntrustmentInfors).ToList();
                    var RetMsg = _customerQuotedPriceService.AutoAddFee(OOPS_M_Order.FirstOrDefault());
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
        /// 获取应收/应付-添加/编辑 视图
        /// </summary>
        /// <param name="Id">应收/应付-ID，编辑表头时，使用</param>
        /// <param name="DtlId">应收/应付 明细-ID，编辑明细时，使用</param>
        /// <param name="DtlHeadId">应收/应付 明细表头-ID，新增表体时使用</param>
        /// <param name="IsBmsBillAr">应收/应付</param>
        /// <returns></returns>
        public ActionResult AddEdit_PopupWin(int Id = 0, int DtlId = 0, int DtlHeadId = 0, bool? IsBmsBillAr = null)
        {
            var Settle_Code = Request["Settle_Code"] ?? "";//结算方 代码
            var Settle_CodeName = "";//结算方 名称
            var DzbhStr = Request["Dzbh"] ?? "";
            var Ops_M_OrdIdStr = Request["Ops_M_OrdId"] ?? "";
            int Ops_M_OrdId = 0;//主单Id
            int.TryParse(Ops_M_OrdIdStr, out Ops_M_OrdId);
            var NumStr = Request["Num"] ?? "0";
            int Num = 1;
            int.TryParse(NumStr, out Num);
            ViewBag.Num = Num;
            BmsBillAp_ArPopupView OBmsBillAp_ArPopupView = new BmsBillAp_ArPopupView();
            OBmsBillAp_ArPopupView.OPS_M_OrdId = Ops_M_OrdId;
            OBmsBillAp_ArPopupView.Dzbh = DzbhStr;

            if (IsBmsBillAr != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settle_Code))
                    {
                        var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                        var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == Settle_Code).FirstOrDefault();
                        if (QArrCusBusInfo != null && QArrCusBusInfo.Id > 0)
                            Settle_CodeName = QArrCusBusInfo.CHNName;
                    }
                    var OPS_M_OrderRep = _unitOfWork.Repository<OPS_M_Order>();
                    var OOPS_M_Order = OPS_M_OrderRep.Queryable().Where(x => x.Id == Ops_M_OrdId).FirstOrDefault();
                    if (OOPS_M_Order == null || OOPS_M_Order.Id <= 0)
                    {
                        ModelState.AddModelError("", "添加/编辑-表头/表体时，总单数据不存在！");
                        return PartialView("AddEdit_PopupWin", null);
                    }
                    else
                    {
                        OBmsBillAp_ArPopupView.MBL = OOPS_M_Order.MBL;
                        OBmsBillAp_ArPopupView.IsMBLJS = OOPS_M_Order.OPS_BMS_Status;
                    }
                }
                catch
                {

                }
                if (Id > 0 || DtlId > 0)
                {
                    if (Id > 0 && DtlId > 0)
                    {
                        ModelState.AddModelError("", "编辑表头/表体时，每次只能编辑一个！");
                        return PartialView("AddEdit_PopupWin", null);
                    }
                    if (DtlHeadId > 0)
                    {
                        ModelState.AddModelError("", "新增表体时，表头和表体主键，不能不为0！");
                        return PartialView("AddEdit_PopupWin", null);
                    }
                }
                if (DtlHeadId > 0 && (Id > 0 && DtlId > 0))
                {
                    ModelState.AddModelError("", "新增表体时，表头和表体主键，不能不为0！");
                    return PartialView("AddEdit_PopupWin", null);
                }

                if ((bool)IsBmsBillAr)
                {
                    if (Id > 0)
                    {
                        #region 编辑应收-表头数据

                        var OBms_Bill_ArDtl = new Bms_Bill_Ar_Dtl();
                        var QBmsBillAr = _bms_Bill_ArService.Queryable().Where(x => x.Id == Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                        if (QBmsBillAr == null || QBmsBillAr.Id <= 0)
                        {
                            ModelState.AddModelError("", "编辑应收-表头时，表头数据已被删除/作废！");
                            return PartialView("AddEdit_PopupWin", null);
                        }
                        else
                        {
                            if (QBmsBillAr.AuditStatus < 0 || QBmsBillAr.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess)
                            {
                                ModelState.AddModelError("", "编辑应收-表头时，表头数据已被审核，无法编辑！");
                                return PartialView("AddEdit_PopupWin", null);
                            }

                            var OAr_ApHead = new Ar_ApHead();
                            OAr_ApHead.Id = Id;
                            OAr_ApHead.Status = QBmsBillAr.Status;
                            OAr_ApHead.AuditStatus = QBmsBillAr.AuditStatus;
                            OAr_ApHead.Create_Status = (int)QBmsBillAr.Create_Status;
                            OAr_ApHead.Bill_Type = QBmsBillAr.Bill_Type;
                            OAr_ApHead.Money_Code = QBmsBillAr.Money_Code;
                            OAr_ApHead.Org_Money_Code = QBmsBillAr.Org_Money_Code;
                            OAr_ApHead.Bill_Object_Id = QBmsBillAr.Bill_Object_Id;
                            OAr_ApHead.Bill_Object_Name = QBmsBillAr.Bill_Object_Name;
                            OAr_ApHead.Payway = QBmsBillAr.Payway;
                            OAr_ApHead.Remark = QBmsBillAr.Remark;
                            OAr_ApHead.Bill_TaxRateType = QBmsBillAr.Bill_TaxRateType;
                            OAr_ApHead.Bill_TaxRate = QBmsBillAr.Bill_TaxRate;
                            OAr_ApHead.Bill_HasTax = QBmsBillAr.Bill_HasTax;
                            OBmsBillAp_ArPopupView.Ar_ApHead = OAr_ApHead;
                            var ODynamic = _bms_Bill_ArService.GetFromNAME(QBmsBillAr);
                            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
                            ViewBag.Bms_Bill_Ar = OBmsBillAp_ArPopupView;
                        }

                        #endregion
                    }
                    else
                    {
                        var OAr_ApHead = new Ar_ApHead();
                        OAr_ApHead.Create_Status = (int)AirOutEnumType.Bms_BillCreate_Status.HandSet;
                        OAr_ApHead.Status = AirOutEnumType.UseStatusEnum.Enable;
                        OAr_ApHead.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                        OAr_ApHead.Bill_Type = "FP";
                        OAr_ApHead.Money_Code = "CNY";
                        OAr_ApHead.Org_Money_Code = "CNY";
                        OAr_ApHead.Payway = "YJZZ";
                        OAr_ApHead.Bill_Object_Id = Settle_Code;// "ZWYBJ01";
                        OAr_ApHead.Bill_Object_Name = Settle_CodeName; //"阪急阪神国际货运(上海)有限公司 ";
                        OAr_ApHead.Bill_TaxRateType = "X0";
                        dynamic ODynamic = new System.Dynamic.ExpandoObject();
                        ODynamic.Bill_TaxRateTypeNAME = "0% 销项税－无税率";
                        ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
                        OBmsBillAp_ArPopupView.Ar_ApHead = OAr_ApHead;
                        ViewBag.Bms_Bill_Ar = OBmsBillAp_ArPopupView;
                    }

                    if (DtlId <= 0)
                    {
                        #region 新增表体

                        var OAr_ApDetl = new Ar_ApDetl();
                        //明细默认值
                        //OAr_ApDetl.Qty = 1;
                        OAr_ApDetl.DtlHeadId = DtlHeadId;
                        OBmsBillAp_ArPopupView.Ar_ApDetl = OAr_ApDetl;

                        ViewBag.Bms_Bill_ArDtl = OBmsBillAp_ArPopupView;

                        #endregion
                    }
                    else
                    {
                        #region 编辑应收-表体

                        var BmsBillArDtlRep = _unitOfWork.Repository<Bms_Bill_Ar_Dtl>();
                        var QBmsBillArDtl = BmsBillArDtlRep.Queryable().Where(x => x.Id == DtlId && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                        if (QBmsBillArDtl == null || QBmsBillArDtl.Id <= 0)
                        {
                            ModelState.AddModelError("", "编辑应收-表体时，表体数据已被删除/作废！");
                            return PartialView("AddEdit_PopupWin", null);
                        }
                        else
                        {
                            var QBmsBillAr = _bms_Bill_ArService.Queryable().Where(x => x.Id == QBmsBillArDtl.Bms_Bill_Ar_Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                            if (QBmsBillAr == null || QBmsBillAr.Id <= 0)
                            {
                                ModelState.AddModelError("", "编辑应收-表体时，表头数据已被删除/作废！");
                                return PartialView("AddEdit_PopupWin", null);
                            }
                            else
                            {
                                if (QBmsBillAr.AuditStatus < 0 || QBmsBillAr.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess)
                                {
                                    ModelState.AddModelError("", "编辑应收-表体时，表头数据已被审核，表体无法编辑！");
                                    return PartialView("AddEdit_PopupWin", null);
                                }
                            }
                            var OAr_ApDetl = new Ar_ApDetl();
                            OAr_ApDetl.DtlId = DtlId;
                            OAr_ApDetl.Charge_Code = QBmsBillArDtl.Charge_Code;
                            OAr_ApDetl.Charge_Desc = QBmsBillArDtl.Charge_Desc;
                            OAr_ApDetl.Unitprice2 = QBmsBillArDtl.Unitprice2;
                            OAr_ApDetl.Account2 = QBmsBillArDtl.Account2;
                            OAr_ApDetl.Summary = QBmsBillArDtl.Summary;
                            OAr_ApDetl.Qty = QBmsBillArDtl.Qty;
                            OBmsBillAp_ArPopupView.Ar_ApDetl = OAr_ApDetl;

                            ViewBag.Bms_Bill_ArDtl = OBmsBillAp_ArPopupView;
                        }

                        #endregion
                    }
                }
                else
                {
                    if (Id > 0)
                    {
                        #region 编辑应付-表头

                        var BmsBillApRep = _unitOfWork.RepositoryAsync<Bms_Bill_Ap>();
                        var QBmsBillAp = BmsBillApRep.Queryable().Where(x => x.Id == Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                        if (QBmsBillAp == null || QBmsBillAp.Id <= 0)
                        {
                            ModelState.AddModelError("", "编辑应付-表头时，表头数据已被删除/作废！");
                            return PartialView("AddEdit_PopupWin", null);
                        }
                        else
                        {
                            if (!QBmsBillAp.IsMBLJS && (QBmsBillAp.AuditStatus < 0 || QBmsBillAp.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess))
                            {
                                ModelState.AddModelError("", "编辑应付-表体时，表头数据已被审核，无法编辑！");
                                return PartialView("AddEdit_PopupWin", null);
                            }
                            ViewData["FTParentId"] = QBmsBillAp.FTParentId;//指示 分摊数据

                            var OAr_ApHead = new Ar_ApHead();
                            OAr_ApHead.Id = Id;
                            OAr_ApHead.Status = QBmsBillAp.Status;
                            OAr_ApHead.AuditStatus = QBmsBillAp.AuditStatus;
                            OAr_ApHead.Create_Status = (int)QBmsBillAp.Create_Status;
                            OAr_ApHead.Bill_Type = QBmsBillAp.Bill_Type;
                            OAr_ApHead.Money_Code = QBmsBillAp.Money_Code;
                            OAr_ApHead.Org_Money_Code = QBmsBillAp.Org_Money_Code;
                            OAr_ApHead.Bill_Object_Id = QBmsBillAp.Bill_Object_Id;
                            OAr_ApHead.Bill_Object_Name = QBmsBillAp.Bill_Object_Name;
                            OAr_ApHead.Payway = QBmsBillAp.Payway;
                            OAr_ApHead.Remark = QBmsBillAp.Remark;
                            OAr_ApHead.Bill_TaxRateType = QBmsBillAp.Bill_TaxRateType;
                            OAr_ApHead.Bill_TaxRate = QBmsBillAp.Bill_TaxRate;
                            OAr_ApHead.Bill_HasTax = QBmsBillAp.Bill_HasTax;
                            OBmsBillAp_ArPopupView.Ar_ApHead = OAr_ApHead;
                            //DataTableImportMappingService ODataImpMapping = new DataTableImportMappingService(_unitOfWork.RepositoryAsync<DataTableImportMapping>());
                            Bms_Bill_ApService OBmsBillArServ = new Bms_Bill_ApService(BmsBillApRep, null);
                            var ODynamic = OBmsBillArServ.GetFromNAME(QBmsBillAp);
                            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);

                            ViewBag.Bms_Bill_Ap = OBmsBillAp_ArPopupView;
                        }

                        #endregion
                    }
                    else
                    {
                        var OAr_ApHead = new Ar_ApHead();
                        OAr_ApHead.Create_Status = (int)AirOutEnumType.Bms_BillCreate_Status.HandSet;
                        OAr_ApHead.Status = AirOutEnumType.UseStatusEnum.Enable;
                        OAr_ApHead.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                        OAr_ApHead.Bill_Type = "C/N";
                        OAr_ApHead.Money_Code = "CNY";
                        OAr_ApHead.Payway = "YJZZ";
                        OAr_ApHead.Org_Money_Code = "CNY";
                        //OAr_ApHead.Bill_Object_Id = Settle_Code;// "ZWYBJ01";
                        //OAr_ApHead.Bill_Object_Name = Settle_CodeName;// "阪急阪神国际货运(上海)有限公司 ";
                        OAr_ApHead.Bill_TaxRateType = "J0";
                        dynamic ODynamic = new System.Dynamic.ExpandoObject();
                        ODynamic.Bill_TaxRateTypeNAME = "0% 进项税－无税率";
                        ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
                        OBmsBillAp_ArPopupView.Ar_ApHead = OAr_ApHead;

                        ViewBag.Bms_Bill_Ap = OBmsBillAp_ArPopupView;
                    }

                    if (DtlId <= 0)
                    {
                        #region 新增表体

                        var OAr_ApDetl = new Ar_ApDetl();
                        //明细默认值
                        //OAr_ApDetl.Qty = 1;
                        OAr_ApDetl.DtlHeadId = DtlHeadId;
                        OBmsBillAp_ArPopupView.Ar_ApDetl = OAr_ApDetl;

                        ViewBag.Bms_Bill_ApDtl = OBmsBillAp_ArPopupView;

                        #endregion
                    }
                    else
                    {
                        #region 编辑应付-表体

                        var BmsBillApDtlRep = _unitOfWork.Repository<Bms_Bill_Ap_Dtl>();
                        var QBmsBillApDtl = BmsBillApDtlRep.Queryable().Where(x => x.Id == DtlId && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                        if (QBmsBillApDtl == null || QBmsBillApDtl.Id <= 0)
                        {
                            ModelState.AddModelError("", "编辑应付-表体时，表体数据已被删除/作废！");
                            return PartialView("AddEdit_PopupWin", null);
                        }
                        else
                        {
                            var BmsBillApRep = _unitOfWork.Repository<Bms_Bill_Ap>();
                            var QBmsBillAp = BmsBillApRep.Queryable().Where(x => x.Id == QBmsBillApDtl.Bms_Bill_Ap_Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                            if (QBmsBillAp == null || QBmsBillAp.Id <= 0)
                            {
                                ModelState.AddModelError("", "编辑应付-表体时，表头数据已被删除/作废！");
                                return PartialView("AddEdit_PopupWin", null);
                            }
                            else
                            {
                                if (QBmsBillAp.AuditStatus < 0 || QBmsBillAp.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess)
                                {
                                    ModelState.AddModelError("", "编辑应付-表体时，表头数据已被审核，表体无法编辑！");
                                    return PartialView("AddEdit_PopupWin", null);
                                }
                            }
                            ViewData["FTParentId"] = QBmsBillAp.FTParentId;//指示 分摊数据

                            var OAr_ApDetl = new Ar_ApDetl();
                            OAr_ApDetl.DtlId = DtlId;
                            OAr_ApDetl.Charge_Code = QBmsBillApDtl.Charge_Code;
                            OAr_ApDetl.Charge_Desc = QBmsBillApDtl.Charge_Desc;
                            OAr_ApDetl.Unitprice2 = QBmsBillApDtl.Unitprice2;
                            OAr_ApDetl.Account2 = QBmsBillApDtl.Account2;
                            OAr_ApDetl.Summary = QBmsBillApDtl.Summary;
                            OAr_ApDetl.Qty = QBmsBillApDtl.Qty;
                            OBmsBillAp_ArPopupView.Ar_ApDetl = OAr_ApDetl;

                            ViewBag.Bms_Bill_ApDtl = OBmsBillAp_ArPopupView;
                        }

                        #endregion
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "数据格式错误！");
            }
            return PartialView("AddEdit_PopupWin", IsBmsBillAr);
        }

        /// <summary>
        /// 设置默认值
        /// </summary>
        /// <returns></returns>
        public Bms_Bill_Ar SetDefaultBms_Bill_Ar()
        {
            Bms_Bill_Ar bms_Bill_Ar = new Bms_Bill_Ar();
            bms_Bill_Ar.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
            bms_Bill_Ar.Status = AirOutEnumType.UseStatusEnum.Enable;
            bms_Bill_Ar.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
            bms_Bill_Ar.Bill_Type = "FP";
            bms_Bill_Ar.Money_Code = "CNY";
            bms_Bill_Ar.Org_Money_Code = "CNY";
            bms_Bill_Ar.Bill_Object_Id = "ZWYBJ01";
            bms_Bill_Ar.Bill_Object_Name = "阪急阪神国际货运(上海)有限公司 ";
            return bms_Bill_Ar;
        }

        /// <summary>
        /// 保存应收/应付 弹出视图
        /// 表头新增的话，可以一起新增明细，编辑时 不能 同时编辑表头 表体
        /// </summary>
        /// <param name="OBmsBillAp_ArPopupView">应收/应付-新增/编辑 视图</param>
        /// <returns></returns>
        public ActionResult SaveAddEdit_PopupWin(BmsBillAp_ArPopupView OBmsBillAp_ArPopupView)
        {
            if (!Request.IsAjaxRequest())
                return Content("请求错误，只允许Ajax请求！");
            var ErrMsg = "";
            if (OBmsBillAp_ArPopupView != null)
            {
                if (OBmsBillAp_ArPopupView.IsBms_Bill_Ar == null)
                    ErrMsg = "未指明应收/付，数据！";
                else if (OBmsBillAp_ArPopupView.OPS_M_OrdId <= 0 || string.IsNullOrWhiteSpace(OBmsBillAp_ArPopupView.Dzbh))
                {
                    ErrMsg = "委托数据，不明确或业务编号为空！";
                }
                else
                {
                    var OPS_M_OrderRep = _unitOfWork.Repository<OPS_M_Order>();
                    var OOPS_M_Order = OPS_M_OrderRep.Queryable().Where(x => x.Id == OBmsBillAp_ArPopupView.OPS_M_OrdId && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                    if (OOPS_M_Order == null || OOPS_M_Order.Id <= 0)
                    {
                        ErrMsg = "委托数据，已被删除/作废！";
                    }
                    else
                    {
                        WebdbContext WebdbContxt = _unitOfWork.getDbContext() as WebdbContext;// AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                        if ((bool)OBmsBillAp_ArPopupView.IsBms_Bill_Ar)
                        {
                            #region 应收 表头新增的话，可以一起新增明细，编辑时 不能 同时编辑表头 表体

                            Bms_Bill_Ar OBms_Bill_Ar = new Bms_Bill_Ar();
                            if (OBmsBillAp_ArPopupView.Ar_ApHead != null && OBmsBillAp_ArPopupView.Ar_ApHead.Id > 0)
                            {
                                OBms_Bill_Ar = _bms_Bill_ArService.Queryable().Where(x => x.Id == OBmsBillAp_ArPopupView.Ar_ApHead.Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                                if (OBms_Bill_Ar == null || OBms_Bill_Ar.Id <= 0)
                                    ErrMsg = "编辑 应收数据，已被删除/作废！";
                                else
                                {
                                    #region 验证表头数据

                                    if (OBms_Bill_Ar.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                                    {
                                        ErrMsg = "编辑 应收数据，已审核，无法操作！";
                                        return Json(new { Success = false, ErrMsg = ErrMsg });
                                    }

                                    #endregion

                                    #region 编辑表头

                                    System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> entry = WebdbContxt.Entry<Bms_Bill_Ar>(OBms_Bill_Ar);
                                    //OBms_Bill_Ar.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                    entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                    bool IsModify = false;
                                    if (OBms_Bill_Ar.Bill_Type != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Type)
                                    {
                                        OBms_Bill_Ar.Bill_Type = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Type;
                                        entry.Property(t => t.Bill_Type).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                    }
                                    if (OBms_Bill_Ar.Money_Code != OBmsBillAp_ArPopupView.Ar_ApHead.Money_Code)
                                    {
                                        OBms_Bill_Ar.Money_Code = OBmsBillAp_ArPopupView.Ar_ApHead.Money_Code;
                                        entry.Property(t => t.Money_Code).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                    }
                                    if (OBms_Bill_Ar.Bill_Object_Id != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Id)
                                    {
                                        OBms_Bill_Ar.Bill_Object_Id = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Id;
                                        entry.Property(t => t.Bill_Object_Id).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                    }
                                    if (OBms_Bill_Ar.Bill_Object_Name != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Name)
                                    {
                                        OBms_Bill_Ar.Bill_Object_Name = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Name;
                                        entry.Property(t => t.Bill_Object_Name).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                    }
                                    if (OBms_Bill_Ar.Payway != OBmsBillAp_ArPopupView.Ar_ApHead.Payway)
                                    {
                                        OBms_Bill_Ar.Payway = OBmsBillAp_ArPopupView.Ar_ApHead.Payway;
                                        entry.Property(t => t.Payway).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                    }
                                    if (OBms_Bill_Ar.Remark != OBmsBillAp_ArPopupView.Ar_ApHead.Remark)
                                    {
                                        OBms_Bill_Ar.Remark = OBmsBillAp_ArPopupView.Ar_ApHead.Remark;
                                        entry.Property(t => t.Remark).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                    }
                                    if (OBms_Bill_Ar.Bill_TaxRateType != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRateType)
                                    {
                                        OBms_Bill_Ar.Bill_TaxRateType = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRateType;//税率类型
                                        entry.Property(t => t.Bill_TaxRateType).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                        OBms_Bill_Ar.BillEDITID = CurrentAppUser.Id;
                                        entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                        OBms_Bill_Ar.BillEDITWHO = CurrentAppUser.UserName;
                                        entry.Property(t => t.BillEDITWHO).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                        OBms_Bill_Ar.BillEDITTS = DateTime.Now;
                                        entry.Property(t => t.BillEDITTS).IsModified = true; //设置要更新的属性
                                        IsModify = true;

                                    }
                                    if (OBms_Bill_Ar.Bill_TaxRate != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRate)
                                    {
                                        OBms_Bill_Ar.Bill_TaxRate = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRate;//税率
                                        entry.Property(t => t.Bill_TaxRate).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                    }
                                    if (OBms_Bill_Ar.Bill_HasTax != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_HasTax)
                                    {
                                        OBms_Bill_Ar.Bill_HasTax = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_HasTax;//含税/不含税
                                        entry.Property(t => t.Bill_HasTax).IsModified = true; //设置要更新的属性
                                        IsModify = true;
                                    }
                                    if (entry.Property(t => t.Bill_HasTax).IsModified || entry.Property(t => t.Bill_TaxRate).IsModified)
                                    {
                                        dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ar.Bill_HasTax, OBms_Bill_Ar.Bill_TaxRate, OBms_Bill_Ar.Bill_Account2);
                                        if (OCalcTaxRate.Success)
                                        {
                                            IsModify = true;
                                            //价（实际金额）
                                            OBms_Bill_Ar.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                            entry.Property(t => t.Bill_Amount).IsModified = true; //设置要更新的属性
                                            //税金 （实际金额 * 税率）
                                            OBms_Bill_Ar.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                            entry.Property(t => t.Bill_TaxAmount).IsModified = true; //设置要更新的属性
                                            //价税合计 (价+税金)
                                            OBms_Bill_Ar.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                            entry.Property(t => t.Bill_AmountTaxTotal).IsModified = true; //设置要更新的属性
                                        }
                                        else
                                        {
                                            ErrMsg = "编辑 应收数据-错误：更新-计算价/税/价税合计出错！";
                                            return Json(new { Success = false, ErrMsg = ErrMsg });
                                        }
                                    }
                                    if (IsModify)
                                        OBms_Bill_Ar.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                    else
                                    {
                                        ErrMsg = "编辑 应收数据-错误：没有需要更新的数据！";
                                        return Json(new { Success = false, ErrMsg = ErrMsg });
                                    }

                                    #endregion
                                }
                            }
                            else
                            {
                                var Bms_Bill_Ar_DtlRep = _unitOfWork.Repository<Bms_Bill_Ar_Dtl>();
                                if (OBmsBillAp_ArPopupView.Ar_ApDetl != null && OBmsBillAp_ArPopupView.Ar_ApDetl.DtlId > 0)
                                {
                                    #region 编辑明细

                                    var OBms_Bill_Ar_Dtl = Bms_Bill_Ar_DtlRep.Queryable().Where(x => x.Id == OBmsBillAp_ArPopupView.Ar_ApDetl.DtlId && x.Status == AirOutEnumType.UseStatusEnum.Enable).Include(x => x.OBms_Bill_Ar).FirstOrDefault();
                                    if (OBms_Bill_Ar_Dtl == null || OBms_Bill_Ar_Dtl.Id <= 0)
                                        ErrMsg = "编辑 应收明细，数据已被删除/作废！";
                                    else
                                    {
                                        #region 验证数据

                                        if (OBms_Bill_Ar_Dtl.OBms_Bill_Ar.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                                        {
                                            ErrMsg = "编辑 应收明细，已审核，无法操作！";
                                            return Json(new { Success = false, ErrMsg = ErrMsg });
                                        }

                                        #endregion

                                        #region 修改主单 实际金额

                                        OBms_Bill_Ar = OBms_Bill_Ar_Dtl.OBms_Bill_Ar;
                                        if (OBms_Bill_Ar != null && OBms_Bill_Ar.Id > 0)
                                        {
                                            System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> entry1 = WebdbContxt.Entry<Bms_Bill_Ar>(OBms_Bill_Ar);
                                            OBms_Bill_Ar.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                            entry1.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）

                                            OBms_Bill_Ar.Bill_Account2 += OBmsBillAp_ArPopupView.Ar_ApDetl.Account2 - OBms_Bill_Ar_Dtl.Account2;
                                            //Common.SetEntryPartialModify(entry1);//效率差
                                            entry1.Property(t => t.Bill_Account2).IsModified = true; //设置要更新的属性
                                            dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ar.Bill_HasTax, OBms_Bill_Ar.Bill_TaxRate, OBms_Bill_Ar.Bill_Account2);
                                            if (OCalcTaxRate.Success)
                                            {
                                                if (OBms_Bill_Ar.Bill_Amount != OCalcTaxRate.Bill_Amount || entry1.Property(t => t.Bill_Account2).IsModified)
                                                {
                                                    OBms_Bill_Ar.BillEDITID = CurrentAppUser.Id;
                                                    entry1.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                    OBms_Bill_Ar.BillEDITWHO = CurrentAppUser.UserName;
                                                    entry1.Property(t => t.BillEDITWHO).IsModified = true; //设置要更新的属性
                                                    OBms_Bill_Ar.BillEDITTS = DateTime.Now;
                                                    entry1.Property(t => t.BillEDITTS).IsModified = true; //设置要更新的属性
                                                }
                                                //价（实际金额）
                                                OBms_Bill_Ar.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                                entry1.Property(t => t.Bill_Amount).IsModified = true; //设置要更新的属性
                                                //税金 （实际金额 * 税率）
                                                OBms_Bill_Ar.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                                entry1.Property(t => t.Bill_TaxAmount).IsModified = true; //设置要更新的属性
                                                //价税合计 (价+税金)
                                                OBms_Bill_Ar.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                                entry1.Property(t => t.Bill_AmountTaxTotal).IsModified = true; //设置要更新的属性
                                            }
                                            else
                                            {
                                                ErrMsg = "编辑 应收明细数据-错误：更新主数据-计算 价/税/价税合计出错！";
                                                return Json(new { Success = false, ErrMsg = ErrMsg });
                                            }
                                        }
                                        else
                                        {
                                            ErrMsg = "编辑 应收明细，主数据已被删除/作废，无法编辑！";
                                        }

                                        #endregion

                                        System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar_Dtl> entry = WebdbContxt.Entry<Bms_Bill_Ar_Dtl>(OBms_Bill_Ar_Dtl);
                                        //OBms_Bill_Ar_Dtl.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                        //System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> Arentry = WebdbContxt.Entry<Bms_Bill_Ar>(OBms_Bill_Ar_Dtl.OBms_Bill_Ar);
                                        //OBms_Bill_Ar_Dtl.OBms_Bill_Ar.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                        //Arentry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                        bool IsModify = false;
                                        if (OBms_Bill_Ar_Dtl.Charge_Code != OBmsBillAp_ArPopupView.Ar_ApDetl.Charge_Code)
                                        {
                                            OBms_Bill_Ar_Dtl.Charge_Code = OBmsBillAp_ArPopupView.Ar_ApDetl.Charge_Code;
                                            entry.Property(t => t.Charge_Code).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ar_Dtl.Charge_Desc != OBmsBillAp_ArPopupView.Ar_ApDetl.Charge_Desc)
                                        {
                                            OBms_Bill_Ar_Dtl.Charge_Desc = OBmsBillAp_ArPopupView.Ar_ApDetl.Charge_Desc;
                                            entry.Property(t => t.Charge_Desc).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ar_Dtl.Unitprice2 != OBmsBillAp_ArPopupView.Ar_ApDetl.Unitprice2)
                                        {
                                            OBms_Bill_Ar_Dtl.Unitprice2 = OBmsBillAp_ArPopupView.Ar_ApDetl.Unitprice2;
                                            entry.Property(t => t.Unitprice2).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ar_Dtl.Qty != OBmsBillAp_ArPopupView.Ar_ApDetl.Qty)
                                        {
                                            OBms_Bill_Ar_Dtl.Qty = OBmsBillAp_ArPopupView.Ar_ApDetl.Qty;
                                            entry.Property(t => t.Qty).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ar_Dtl.Account2 != OBmsBillAp_ArPopupView.Ar_ApDetl.Account2)
                                        {
                                            OBms_Bill_Ar_Dtl.Account2 = OBmsBillAp_ArPopupView.Ar_ApDetl.Account2;
                                            entry.Property(t => t.Account2).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                            OBms_Bill_Ar_Dtl.BillEDITID = CurrentAppUser.Id;
                                            entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                            OBms_Bill_Ar_Dtl.BillEDITWHO = CurrentAppUser.UserName;
                                            entry.Property(t => t.BillEDITWHO).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                            OBms_Bill_Ar_Dtl.BillEDITTS = DateTime.Now;
                                            entry.Property(t => t.BillEDITTS).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                            //OBms_Bill_Ar_Dtl.OBms_Bill_Ar.BillEDITID = CurrentAppUser.Id;
                                            //Arentry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                            //IsModify = true;
                                            //OBms_Bill_Ar_Dtl.OBms_Bill_Ar.BillEDITWHO = CurrentAppUser.UserName;
                                            //Arentry.Property(t => t.BillEDITWHO).IsModified = true; //设置要更新的属性
                                            //IsModify = true;
                                            //OBms_Bill_Ar_Dtl.OBms_Bill_Ar.BillEDITTS = DateTime.Now;
                                            //Arentry.Property(t => t.BillEDITTS).IsModified = true; //设置要更新的属性
                                            //IsModify = true;
                                        }
                                        if (OBms_Bill_Ar_Dtl.Summary != OBmsBillAp_ArPopupView.Ar_ApDetl.Summary)
                                        {
                                            OBms_Bill_Ar_Dtl.Summary = OBmsBillAp_ArPopupView.Ar_ApDetl.Summary;
                                            entry.Property(t => t.Summary).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (IsModify)
                                            OBms_Bill_Ar_Dtl.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                        else
                                        {
                                            ErrMsg = "编辑 应收明细数据-错误：没有需要更新的数据！";
                                            return Json(new { Success = false, ErrMsg = ErrMsg });
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    if (OBmsBillAp_ArPopupView.Ar_ApDetl != null && OBmsBillAp_ArPopupView.Ar_ApDetl.DtlHeadId <= 0)
                                    {
                                        #region 新增表头

                                        OBms_Bill_Ar.Bill_Date = DateTime.Now;
                                        OBms_Bill_Ar.Status = OBmsBillAp_ArPopupView.Ar_ApHead.Status;
                                        OBms_Bill_Ar.AuditStatus = OBmsBillAp_ArPopupView.Ar_ApHead.AuditStatus;
                                        OBms_Bill_Ar.Create_Status = (AirOutEnumType.Bms_BillCreate_Status)Enum.Parse(typeof(AirOutEnumType.Bms_BillCreate_Status), OBmsBillAp_ArPopupView.Ar_ApHead.Create_Status.ToString());
                                        OBms_Bill_Ar.Bill_Type = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Type;
                                        OBms_Bill_Ar.Money_Code = OBmsBillAp_ArPopupView.Ar_ApHead.Money_Code;
                                        OBms_Bill_Ar.Org_Money_Code = OBmsBillAp_ArPopupView.Ar_ApHead.Org_Money_Code;
                                        OBms_Bill_Ar.Bill_Object_Id = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Id;
                                        OBms_Bill_Ar.Bill_Object_Name = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Name;
                                        OBms_Bill_Ar.Ops_M_OrdId = OBmsBillAp_ArPopupView.OPS_M_OrdId;
                                        OBms_Bill_Ar.Dzbh = OBmsBillAp_ArPopupView.Dzbh;
                                        OBms_Bill_Ar.Payway = OBmsBillAp_ArPopupView.Ar_ApHead.Payway;
                                        OBms_Bill_Ar.Remark = OBmsBillAp_ArPopupView.Ar_ApHead.Remark;
                                        OBms_Bill_Ar.Bill_TaxRateType = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRateType;
                                        OBms_Bill_Ar.Bill_TaxRate = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRate;
                                        OBms_Bill_Ar.Bill_HasTax = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_HasTax;
                                        if (!string.IsNullOrWhiteSpace(OBmsBillAp_ArPopupView.MBL))
                                            OBms_Bill_Ar.MBL = OBmsBillAp_ArPopupView.MBL;
                                        var Line_No = SequenceBuilder.NextBmsBillArLineNo(true, OBmsBillAp_ArPopupView.Dzbh);
                                        OBms_Bill_Ar.Line_No = Convert.ToInt32(Line_No);

                                        #endregion

                                        #region 新增明细

                                        List<Ar_ApDetl> ArrAr_ApDetl = new List<Ar_ApDetl>();
                                        if (OBmsBillAp_ArPopupView.ArrAr_ApDtl == null || OBmsBillAp_ArPopupView.ArrAr_ApDtl.Count() <= 1)
                                        {
                                            ArrAr_ApDetl.Add(OBmsBillAp_ArPopupView.Ar_ApDetl);
                                        }
                                        else
                                        {
                                            ArrAr_ApDetl = ArrAr_ApDetl.Concat(OBmsBillAp_ArPopupView.ArrAr_ApDtl).ToList();
                                        }
                                        if (ArrAr_ApDetl.Any())
                                        {
                                            if (ArrAr_ApDetl.Count > 1)
                                                OBms_Bill_Ar.Bill_Account2 = ArrAr_ApDetl.Sum(x => x.Account2);
                                            else
                                                OBms_Bill_Ar.Bill_Account2 = ArrAr_ApDetl[0].Account2;
                                            dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ar.Bill_HasTax, OBms_Bill_Ar.Bill_TaxRate, OBms_Bill_Ar.Bill_Account2);
                                            if (OCalcTaxRate.Success)
                                            {
                                                //价（实际金额）
                                                OBms_Bill_Ar.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                                //税金 （实际金额 * 税率）
                                                OBms_Bill_Ar.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                                //价税合计 (价+税金)
                                                OBms_Bill_Ar.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                            }
                                            else
                                            {
                                                ErrMsg = "新增 应收数据-错误：计算价/税/价税合计出错！";
                                                return Json(new { Success = false, ErrMsg = ErrMsg });
                                            }
                                        }
                                        _bms_Bill_ArService.Insert(OBms_Bill_Ar);

                                        foreach (var item in ArrAr_ApDetl)
                                        {
                                            if (!string.IsNullOrWhiteSpace(OBmsBillAp_ArPopupView.Dzbh) &&
                                                OBmsBillAp_ArPopupView.OPS_M_OrdId > 0 &&
                                               !string.IsNullOrWhiteSpace(item.Charge_Code) &&
                                               !string.IsNullOrWhiteSpace(item.Charge_Desc))
                                            {
                                                Bms_Bill_Ar_Dtl OBms_Bill_Ar_Dtl = new Bms_Bill_Ar_Dtl();
                                                OBms_Bill_Ar_Dtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                                                OBms_Bill_Ar_Dtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                                                OBms_Bill_Ar_Dtl.Dzbh = OBmsBillAp_ArPopupView.Dzbh;
                                                OBms_Bill_Ar_Dtl.Ops_M_OrdId = OBmsBillAp_ArPopupView.OPS_M_OrdId;
                                                OBms_Bill_Ar_Dtl.Charge_Code = item.Charge_Code;
                                                OBms_Bill_Ar_Dtl.Charge_Desc = item.Charge_Desc;
                                                OBms_Bill_Ar_Dtl.Unitprice2 = item.Unitprice2;
                                                OBms_Bill_Ar_Dtl.Qty = item.Qty;
                                                OBms_Bill_Ar_Dtl.Account2 = item.Account2;
                                                OBms_Bill_Ar_Dtl.Summary = item.Summary;
                                                OBms_Bill_Ar_Dtl.Bms_Bill_Ar_Id = OBms_Bill_Ar.Id;
                                                OBms_Bill_Ar_Dtl.Line_No = OBms_Bill_Ar.Line_No;
                                                var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, OBmsBillAp_ArPopupView.Dzbh + "_" + OBms_Bill_Ar.Line_No, true);
                                                OBms_Bill_Ar_Dtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                                //OBms_Bill_Ar_Dtl.OBms_Bill_Ar.BillEDITID = CurrentAppUser.Id;
                                                //OBms_Bill_Ar_Dtl.OBms_Bill_Ar.BillEDITWHO = CurrentAppUser.UserName;
                                                //OBms_Bill_Ar_Dtl.OBms_Bill_Ar.BillEDITTS = DateTime.Now;
                                                Bms_Bill_Ar_DtlRep.Insert(OBms_Bill_Ar_Dtl);
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region 新增明细

                                        OBms_Bill_Ar = _bms_Bill_ArService.Queryable().Where(x => x.Id == OBmsBillAp_ArPopupView.Ar_ApDetl.DtlHeadId && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                                        if (OBms_Bill_Ar == null || OBms_Bill_Ar.Id <= 0)
                                            ErrMsg = "编辑 应收数据，已被删除/作废！";
                                        else
                                        {
                                            #region 验证数据

                                            if (OBms_Bill_Ar.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                                            {
                                                ErrMsg = "编辑 应收数据，已审核，无法操作！";
                                                return Json(new { Success = false, ErrMsg = ErrMsg });
                                            }

                                            #endregion

                                            List<Ar_ApDetl> ArrAr_ApDetl = new List<Ar_ApDetl>();
                                            if (OBmsBillAp_ArPopupView.ArrAr_ApDtl == null || OBmsBillAp_ArPopupView.ArrAr_ApDtl.Count() <= 1)
                                            {
                                                ArrAr_ApDetl.Add(OBmsBillAp_ArPopupView.Ar_ApDetl);
                                            }
                                            else
                                            {
                                                ArrAr_ApDetl = ArrAr_ApDetl.Concat(OBmsBillAp_ArPopupView.ArrAr_ApDtl).ToList();
                                            }
                                            if (ArrAr_ApDetl.Any())
                                            {
                                                #region 修改主单 实际金额

                                                System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> entry = WebdbContxt.Entry<Bms_Bill_Ar>(OBms_Bill_Ar);
                                                OBms_Bill_Ar.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                                entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                                decimal T_Bill_Account2 = 0;
                                                if (ArrAr_ApDetl.Count > 1)
                                                    T_Bill_Account2 = ArrAr_ApDetl.Sum(x => x.Account2);
                                                else
                                                    T_Bill_Account2 = ArrAr_ApDetl[0].Account2;

                                                OBms_Bill_Ar.Bill_Account2 += T_Bill_Account2;
                                                entry.Property(t => t.Bill_Account2).IsModified = true; //设置要更新的属性

                                                dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ar.Bill_HasTax, OBms_Bill_Ar.Bill_TaxRate, OBms_Bill_Ar.Bill_Account2);
                                                if (OCalcTaxRate.Success)
                                                {
                                                    //价（实际金额）
                                                    OBms_Bill_Ar.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                                    entry.Property(t => t.Bill_Amount).IsModified = true; //设置要更新的属性
                                                    //税金 （实际金额 * 税率）
                                                    OBms_Bill_Ar.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                                    entry.Property(t => t.Bill_TaxAmount).IsModified = true; //设置要更新的属性
                                                    //价税合计 (价+税金)
                                                    OBms_Bill_Ar.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                                    entry.Property(t => t.Bill_AmountTaxTotal).IsModified = true; //设置要更新的属性

                                                    OBms_Bill_Ar.BillEDITID = CurrentAppUser.Id;
                                                    entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                    OBms_Bill_Ar.BillEDITWHO = CurrentAppUser.UserName;
                                                    entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                    OBms_Bill_Ar.BillEDITTS = DateTime.Now;
                                                    entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                }
                                                else
                                                {
                                                    ErrMsg = "修改 应收数据-错误：计算价/税/价税合计出错！";
                                                    return Json(new { Success = false, ErrMsg = ErrMsg });
                                                }

                                                #endregion
                                            }

                                            foreach (var item in ArrAr_ApDetl)
                                            {
                                                if (!string.IsNullOrWhiteSpace(OBmsBillAp_ArPopupView.Dzbh) &&
                                                    OBmsBillAp_ArPopupView.OPS_M_OrdId > 0 &&
                                                   !string.IsNullOrWhiteSpace(item.Charge_Code) &&
                                                   !string.IsNullOrWhiteSpace(item.Charge_Desc))
                                                {
                                                    Bms_Bill_Ar_Dtl OBms_Bill_Ar_Dtl = new Bms_Bill_Ar_Dtl();
                                                    OBms_Bill_Ar_Dtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                                                    OBms_Bill_Ar_Dtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                                                    OBms_Bill_Ar_Dtl.Dzbh = OBmsBillAp_ArPopupView.Dzbh;
                                                    OBms_Bill_Ar_Dtl.Ops_M_OrdId = OBmsBillAp_ArPopupView.OPS_M_OrdId;
                                                    OBms_Bill_Ar_Dtl.Charge_Code = item.Charge_Code;
                                                    OBms_Bill_Ar_Dtl.Charge_Desc = item.Charge_Desc;
                                                    OBms_Bill_Ar_Dtl.Unitprice2 = item.Unitprice2;
                                                    OBms_Bill_Ar_Dtl.Qty = item.Qty;
                                                    OBms_Bill_Ar_Dtl.Account2 = item.Account2;
                                                    OBms_Bill_Ar_Dtl.Summary = item.Summary;
                                                    OBms_Bill_Ar_Dtl.Bms_Bill_Ar_Id = OBms_Bill_Ar.Id;
                                                    OBms_Bill_Ar_Dtl.Line_No = OBms_Bill_Ar.Line_No;
                                                    var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, OBmsBillAp_ArPopupView.Dzbh + "_" + OBms_Bill_Ar.Line_No, true);
                                                    OBms_Bill_Ar_Dtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                                    Bms_Bill_Ar_DtlRep.Insert(OBms_Bill_Ar_Dtl);
                                                }
                                                else
                                                {
                                                    ErrMsg = "新增 应收数据，业务编号/主键，或费用名称/说明 为空！";
                                                    break;
                                                }
                                            }
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region 应付 表头新增的话，可以一起新增明细，编辑时 不能 同时编辑表头 表体

                            var Bms_Bill_ApRep = _unitOfWork.Repository<Bms_Bill_Ap>();
                            Bms_Bill_Ap OBms_Bill_Ap = new Bms_Bill_Ap();
                            if (OBmsBillAp_ArPopupView.Ar_ApHead != null && OBmsBillAp_ArPopupView.Ar_ApHead.Id > 0)
                            {
                                OBms_Bill_Ap = Bms_Bill_ApRep.Queryable().Where(x => x.Id == OBmsBillAp_ArPopupView.Ar_ApHead.Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                                if (OBms_Bill_Ap == null || OBms_Bill_Ap.Id <= 0)
                                    ErrMsg = "编辑 应付数据，已被删除/作废！";
                                else
                                {
                                    #region 验证数据

                                    if (OBms_Bill_Ap.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                                    {
                                        ErrMsg = "编辑 应付数据，已审核，无法操作！";
                                        return Json(new { Success = false, ErrMsg = ErrMsg });
                                    }

                                    #endregion

                                    if (OBms_Bill_Ap.FTParentId > 0)
                                    {
                                        ErrMsg = "编辑 应付数据，分摊数据无法编辑！";
                                    }
                                    else
                                    {
                                        #region 编辑表头

                                        System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap> entry = WebdbContxt.Entry<Bms_Bill_Ap>(OBms_Bill_Ap);
                                        //OBms_Bill_Ap.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）

                                        bool IsModify = false;
                                        if (OBms_Bill_Ap.Bill_Type != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Type)
                                        {
                                            OBms_Bill_Ap.Bill_Type = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Type;
                                            entry.Property(t => t.Bill_Type).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ap.Money_Code != OBmsBillAp_ArPopupView.Ar_ApHead.Money_Code)
                                        {
                                            OBms_Bill_Ap.Money_Code = OBmsBillAp_ArPopupView.Ar_ApHead.Money_Code;
                                            entry.Property(t => t.Money_Code).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ap.Bill_Object_Id != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Id)
                                        {
                                            OBms_Bill_Ap.Bill_Object_Id = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Id;
                                            entry.Property(t => t.Bill_Object_Id).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ap.Bill_Object_Name != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Name)
                                        {
                                            OBms_Bill_Ap.Bill_Object_Name = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Name;
                                            entry.Property(t => t.Bill_Object_Name).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ap.Payway != OBmsBillAp_ArPopupView.Ar_ApHead.Payway)
                                        {
                                            OBms_Bill_Ap.Payway = OBmsBillAp_ArPopupView.Ar_ApHead.Payway;
                                            entry.Property(t => t.Payway).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ap.Remark != OBmsBillAp_ArPopupView.Ar_ApHead.Remark)
                                        {
                                            OBms_Bill_Ap.Remark = OBmsBillAp_ArPopupView.Ar_ApHead.Remark;
                                            entry.Property(t => t.Remark).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ap.Bill_TaxRateType != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRateType)
                                        {
                                            OBms_Bill_Ap.Bill_TaxRateType = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRateType;//税率类型
                                            entry.Property(t => t.Bill_TaxRateType).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                            OBms_Bill_Ap.BillEDITID = CurrentAppUser.Id;
                                            entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                            OBms_Bill_Ap.BillEDITWHO = CurrentAppUser.UserName;
                                            entry.Property(t => t.BillEDITWHO).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                            OBms_Bill_Ap.BillEDITTS = DateTime.Now;
                                            entry.Property(t => t.BillEDITTS).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ap.Bill_TaxRate != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRate)
                                        {
                                            OBms_Bill_Ap.Bill_TaxRate = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRate;//税率
                                            entry.Property(t => t.Bill_TaxRate).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (OBms_Bill_Ap.Bill_HasTax != OBmsBillAp_ArPopupView.Ar_ApHead.Bill_HasTax)
                                        {
                                            OBms_Bill_Ap.Bill_HasTax = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_HasTax;//含税/不含税
                                            entry.Property(t => t.Bill_HasTax).IsModified = true; //设置要更新的属性
                                            IsModify = true;
                                        }
                                        if (entry.Property(t => t.Bill_HasTax).IsModified || entry.Property(t => t.Bill_TaxRate).IsModified)
                                        {
                                            dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ap.Bill_HasTax, OBms_Bill_Ap.Bill_TaxRate, OBms_Bill_Ap.Bill_Account2);
                                            if (OCalcTaxRate.Success)
                                            {
                                                //价（实际金额）
                                                OBms_Bill_Ap.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                                entry.Property(t => t.Bill_Amount).IsModified = true; //设置要更新的属性
                                                //税金 （实际金额 * 税率）
                                                OBms_Bill_Ap.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                                entry.Property(t => t.Bill_TaxAmount).IsModified = true; //设置要更新的属性
                                                //价税合计 (价+税金)
                                                OBms_Bill_Ap.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                                entry.Property(t => t.Bill_AmountTaxTotal).IsModified = true; //设置要更新的属性
                                            }
                                            else
                                            {
                                                ErrMsg = "编辑 应付数据-错误：计算价/税/价税合计出错！";
                                                return Json(new { Success = false, ErrMsg = ErrMsg });
                                            }
                                        }
                                        if (IsModify)
                                            OBms_Bill_Ap.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                        else
                                        {
                                            ErrMsg = "编辑 应付数据-错误：没有需要更新的数据！";
                                            return Json(new { Success = false, ErrMsg = ErrMsg });
                                        }

                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                var Bms_Bill_Ap_DtlRep = _unitOfWork.Repository<Bms_Bill_Ap_Dtl>();
                                if (OBmsBillAp_ArPopupView.Ar_ApDetl != null && OBmsBillAp_ArPopupView.Ar_ApDetl.DtlId > 0)
                                {
                                    #region 编辑明细

                                    var OBms_Bill_Ap_Dtl = Bms_Bill_Ap_DtlRep.Queryable().Where(x => x.Id == OBmsBillAp_ArPopupView.Ar_ApDetl.DtlId && x.Status == AirOutEnumType.UseStatusEnum.Enable).Include(x => x.OBms_Bill_Ap).FirstOrDefault();
                                    if (OBms_Bill_Ap_Dtl == null || OBms_Bill_Ap_Dtl.Id <= 0)
                                        ErrMsg = "编辑 应付明细，数据已被删除/作废！";
                                    else
                                    {
                                        #region 验证数据

                                        if (OBms_Bill_Ap_Dtl.OBms_Bill_Ap.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                                        {
                                            ErrMsg = "编辑 应付明细，已审核，无法操作！";
                                            return Json(new { Success = false, ErrMsg = ErrMsg });
                                        }

                                        #endregion
                                        OBms_Bill_Ap = OBms_Bill_Ap_Dtl.OBms_Bill_Ap;
                                        if (OBms_Bill_Ap == null || OBms_Bill_Ap.Id <= 0)
                                        {
                                            ErrMsg = "编辑 应付明细，主数据已被删除/作废，无法编辑！";
                                        }
                                        else if (OBms_Bill_Ap.FTParentId > 0)
                                        {
                                            ErrMsg = "编辑 应付明细，分摊数据无法编辑！";
                                        }
                                        else
                                        {
                                            #region 修改主单 实际金额

                                            System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap> entry1 = WebdbContxt.Entry<Bms_Bill_Ap>(OBms_Bill_Ap);
                                            OBms_Bill_Ap.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                            entry1.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）

                                            OBms_Bill_Ap.Bill_Account2 += OBmsBillAp_ArPopupView.Ar_ApDetl.Account2 - OBms_Bill_Ap_Dtl.Account2;
                                            entry1.Property(t => t.Bill_Account2).IsModified = true; //设置要更新的属性

                                            dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ap.Bill_HasTax, OBms_Bill_Ap.Bill_TaxRate, OBms_Bill_Ap.Bill_Account2);
                                            if (OCalcTaxRate.Success)
                                            {
                                                if (OBms_Bill_Ap.Bill_Amount != OCalcTaxRate.Bill_Amount || entry1.Property(t => t.Bill_Account2).IsModified) 
                                                {
                                                    OBms_Bill_Ap.BillEDITID = CurrentAppUser.Id;
                                                    entry1.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                    OBms_Bill_Ap.BillEDITWHO = CurrentAppUser.UserName;
                                                    entry1.Property(t => t.BillEDITWHO).IsModified = true; //设置要更新的属性
                                                    OBms_Bill_Ap.BillEDITTS = DateTime.Now;
                                                    entry1.Property(t => t.BillEDITTS).IsModified = true; //设置要更新的属性
                                                }
                                                //价（实际金额）
                                                OBms_Bill_Ap.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                                entry1.Property(t => t.Bill_Amount).IsModified = true; //设置要更新的属性
                                                //税金 （实际金额 * 税率）
                                                OBms_Bill_Ap.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                                entry1.Property(t => t.Bill_TaxAmount).IsModified = true; //设置要更新的属性
                                                //价税合计 (价+税金)
                                                OBms_Bill_Ap.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                                entry1.Property(t => t.Bill_AmountTaxTotal).IsModified = true; //设置要更新的属性                                        
                                            }
                                            else
                                            {
                                                ErrMsg = "修改 应付数据-错误：计算价/税/价税合计出错！";
                                                return Json(new { Success = false, ErrMsg = ErrMsg });
                                            }

                                            #endregion

                                            System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap_Dtl> entry = WebdbContxt.Entry<Bms_Bill_Ap_Dtl>(OBms_Bill_Ap_Dtl);                                          
                                            OBms_Bill_Ap_Dtl.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                            entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                            //System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap> Apentry = WebdbContxt.Entry<Bms_Bill_Ap>(OBms_Bill_Ap_Dtl.OBms_Bill_Ap);
                                            //OBms_Bill_Ap_Dtl.OBms_Bill_Ap.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                            //Apentry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                            bool IsModify = false;
                                            bool IsBillAccountChg = false;
                                            if (OBms_Bill_Ap_Dtl.Account2 != OBmsBillAp_ArPopupView.Ar_ApDetl.Account2) 
                                            {
                                  
                                            }
                                            if (OBms_Bill_Ap_Dtl.Charge_Code != OBmsBillAp_ArPopupView.Ar_ApDetl.Charge_Code)
                                            {
                                                OBms_Bill_Ap_Dtl.Charge_Code = OBmsBillAp_ArPopupView.Ar_ApDetl.Charge_Code;
                                                entry.Property(t => t.Charge_Code).IsModified = true; //设置要更新的属性
                                                IsModify = true;
                                            }
                                            if (OBms_Bill_Ap_Dtl.Charge_Desc != OBmsBillAp_ArPopupView.Ar_ApDetl.Charge_Desc)
                                            {
                                                OBms_Bill_Ap_Dtl.Charge_Desc = OBmsBillAp_ArPopupView.Ar_ApDetl.Charge_Desc;
                                                entry.Property(t => t.Charge_Desc).IsModified = true; //设置要更新的属性
                                                IsModify = true;
                                            }
                                            if (OBms_Bill_Ap_Dtl.Unitprice2 != OBmsBillAp_ArPopupView.Ar_ApDetl.Unitprice2)
                                            {
                                                OBms_Bill_Ap_Dtl.Unitprice2 = OBmsBillAp_ArPopupView.Ar_ApDetl.Unitprice2;
                                                entry.Property(t => t.Unitprice2).IsModified = true; //设置要更新的属性
                                                IsModify = true;
                                            }
                                            if (OBms_Bill_Ap_Dtl.Qty != OBmsBillAp_ArPopupView.Ar_ApDetl.Qty)
                                            {
                                                OBms_Bill_Ap_Dtl.Qty = OBmsBillAp_ArPopupView.Ar_ApDetl.Qty;
                                                entry.Property(t => t.Qty).IsModified = true; //设置要更新的属性
                                                IsModify = true;
                                            }
                                            if (OBms_Bill_Ap_Dtl.Account2 != OBmsBillAp_ArPopupView.Ar_ApDetl.Account2)
                                            {
                                                OBms_Bill_Ap_Dtl.Account2 = OBmsBillAp_ArPopupView.Ar_ApDetl.Account2;
                                                entry.Property(t => t.Account2).IsModified = true; //设置要更新的属性
                                                IsModify = true;
                                                OBms_Bill_Ap_Dtl.BillEDITID = CurrentAppUser.Id;
                                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                IsModify = true;
                                                OBms_Bill_Ap_Dtl.BillEDITWHO = CurrentAppUser.UserName;
                                                entry.Property(t => t.BillEDITWHO).IsModified = true; //设置要更新的属性
                                                IsModify = true;
                                                OBms_Bill_Ap_Dtl.BillEDITTS = DateTime.Now;
                                                entry.Property(t => t.BillEDITTS).IsModified = true; //设置要更新的属性
                                                IsModify = true;
                                                //OBms_Bill_Ap_Dtl.OBms_Bill_Ap.BillEDITID = CurrentAppUser.Id;
                                                //Apentry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                //IsModify = true;
                                                //OBms_Bill_Ap_Dtl.OBms_Bill_Ap.BillEDITWHO = CurrentAppUser.UserName;
                                                //Apentry.Property(t => t.BillEDITWHO).IsModified = true; //设置要更新的属性
                                                //IsModify = true;
                                                //OBms_Bill_Ap_Dtl.OBms_Bill_Ap.BillEDITTS = DateTime.Now;
                                                //Apentry.Property(t => t.BillEDITTS).IsModified = true; //设置要更新的属性
                                                //IsModify = true;
                                            }
                                            if (OBms_Bill_Ap_Dtl.Summary != OBmsBillAp_ArPopupView.Ar_ApDetl.Summary)
                                            {
                                                OBms_Bill_Ap_Dtl.Summary = OBmsBillAp_ArPopupView.Ar_ApDetl.Summary;
                                                entry.Property(t => t.Summary).IsModified = true; //设置要更新的属性
                                                IsModify = true;
                                            }
                                            if (IsModify)
                                                OBms_Bill_Ap_Dtl.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                            else
                                            {
                                                ErrMsg = "修改 应付明细数据-错误：没有需要更新的数据！";
                                                return Json(new { Success = false, ErrMsg = ErrMsg });
                                            }
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    if (OBmsBillAp_ArPopupView.Ar_ApDetl != null && OBmsBillAp_ArPopupView.Ar_ApDetl.DtlHeadId <= 0)
                                    {
                                        #region 新增表头

                                        OBms_Bill_Ap.Bill_Date = DateTime.Now;
                                        OBms_Bill_Ap.Status = OBmsBillAp_ArPopupView.Ar_ApHead.Status;
                                        OBms_Bill_Ap.AuditStatus = OBmsBillAp_ArPopupView.Ar_ApHead.AuditStatus;
                                        OBms_Bill_Ap.Create_Status = (AirOutEnumType.Bms_BillCreate_Status)Enum.Parse(typeof(AirOutEnumType.Bms_BillCreate_Status), OBmsBillAp_ArPopupView.Ar_ApHead.Create_Status.ToString());
                                        OBms_Bill_Ap.Bill_Type = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Type;
                                        OBms_Bill_Ap.Money_Code = OBmsBillAp_ArPopupView.Ar_ApHead.Money_Code;
                                        OBms_Bill_Ap.Org_Money_Code = OBmsBillAp_ArPopupView.Ar_ApHead.Org_Money_Code;
                                        OBms_Bill_Ap.Bill_Object_Id = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Id;
                                        OBms_Bill_Ap.Bill_Object_Name = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_Object_Name;
                                        OBms_Bill_Ap.Ops_M_OrdId = OBmsBillAp_ArPopupView.OPS_M_OrdId;
                                        OBms_Bill_Ap.Dzbh = OBmsBillAp_ArPopupView.Dzbh;
                                        OBms_Bill_Ap.Payway = OBmsBillAp_ArPopupView.Ar_ApHead.Payway;
                                        OBms_Bill_Ap.Remark = OBmsBillAp_ArPopupView.Ar_ApHead.Remark;
                                        OBms_Bill_Ap.Bill_TaxRateType = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRateType;
                                        OBms_Bill_Ap.Bill_TaxRate = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_TaxRate;
                                        OBms_Bill_Ap.Bill_HasTax = OBmsBillAp_ArPopupView.Ar_ApHead.Bill_HasTax;
                                        if (!string.IsNullOrWhiteSpace(OBmsBillAp_ArPopupView.MBL))
                                            OBms_Bill_Ap.MBL = OBmsBillAp_ArPopupView.MBL;
                                        //直接读取 主单信息上的 总单应付 标志
                                        OBms_Bill_Ap.IsMBLJS = OOPS_M_Order.OPS_BMS_Status;//OBmsBillAp_ArPopupView.IsMBLJS;
                                        var Line_No = SequenceBuilder.NextBmsBillArLineNo(false, OBmsBillAp_ArPopupView.Dzbh);
                                        OBms_Bill_Ap.Line_No = Convert.ToInt32(Line_No);

                                        #endregion

                                        #region 新增明细

                                        List<Ar_ApDetl> ArrAr_ApDetl = new List<Ar_ApDetl>();
                                        if (OBmsBillAp_ArPopupView.ArrAr_ApDtl == null || OBmsBillAp_ArPopupView.ArrAr_ApDtl.Count() <= 1)
                                        {
                                            ArrAr_ApDetl.Add(OBmsBillAp_ArPopupView.Ar_ApDetl);
                                        }
                                        else
                                        {
                                            ArrAr_ApDetl = ArrAr_ApDetl.Concat(OBmsBillAp_ArPopupView.ArrAr_ApDtl).ToList();
                                        }
                                        if (ArrAr_ApDetl.Any())
                                        {
                                            if (ArrAr_ApDetl.Count > 1)
                                                OBms_Bill_Ap.Bill_Account2 = ArrAr_ApDetl.Sum(x => x.Account2);
                                            else
                                                OBms_Bill_Ap.Bill_Account2 = ArrAr_ApDetl[0].Account2;
                                            dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ap.Bill_HasTax, OBms_Bill_Ap.Bill_TaxRate, OBms_Bill_Ap.Bill_Account2);
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
                                                ErrMsg = "新增 应付数据-错误：计算价/税/价税合计出错！";
                                                return Json(new { Success = false, ErrMsg = ErrMsg });
                                            }
                                        }
                                        Bms_Bill_ApRep.Insert(OBms_Bill_Ap);
                                        foreach (var item in ArrAr_ApDetl)
                                        {
                                            if (!string.IsNullOrWhiteSpace(OBmsBillAp_ArPopupView.Dzbh) &&
                                                OBmsBillAp_ArPopupView.OPS_M_OrdId > 0 &&
                                               !string.IsNullOrWhiteSpace(item.Charge_Code) &&
                                               !string.IsNullOrWhiteSpace(item.Charge_Desc))
                                            {
                                                Bms_Bill_Ap_Dtl OBms_Bill_Ap_Dtl = new Bms_Bill_Ap_Dtl();
                                                OBms_Bill_Ap_Dtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                                                OBms_Bill_Ap_Dtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                                                OBms_Bill_Ap_Dtl.Dzbh = OBmsBillAp_ArPopupView.Dzbh;
                                                OBms_Bill_Ap_Dtl.Ops_M_OrdId = OBmsBillAp_ArPopupView.OPS_M_OrdId;
                                                OBms_Bill_Ap_Dtl.Charge_Code = item.Charge_Code;
                                                OBms_Bill_Ap_Dtl.Charge_Desc = item.Charge_Desc;
                                                OBms_Bill_Ap_Dtl.Unitprice2 = item.Unitprice2;
                                                OBms_Bill_Ap_Dtl.Qty = item.Qty;
                                                OBms_Bill_Ap_Dtl.Account2 = item.Account2;
                                                OBms_Bill_Ap_Dtl.Summary = item.Summary;
                                                OBms_Bill_Ap_Dtl.Bms_Bill_Ap_Id = OBms_Bill_Ap.Id;
                                                OBms_Bill_Ap_Dtl.Line_No = OBms_Bill_Ap.Line_No;
                                                var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, OBmsBillAp_ArPopupView.Dzbh + "_" + OBms_Bill_Ap.Line_No, true);
                                                OBms_Bill_Ap_Dtl.Line_Id = Convert.ToInt32(DtlLine_No);                                               
                                                Bms_Bill_Ap_DtlRep.Insert(OBms_Bill_Ap_Dtl);
                                           }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region 新增明细

                                        OBms_Bill_Ap = Bms_Bill_ApRep.Queryable().Where(x => x.Id == OBmsBillAp_ArPopupView.Ar_ApDetl.DtlHeadId && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                                        if (OBms_Bill_Ap == null || OBms_Bill_Ap.Id <= 0)
                                            ErrMsg = "编辑 应付数据，已被删除/作废！";
                                        else
                                        {
                                            #region 验证数据

                                            if (OBms_Bill_Ap.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                                            {
                                                ErrMsg = "编辑 应付数据，已审核，无法操作！";
                                                return Json(new { Success = false, ErrMsg = ErrMsg });
                                            }

                                            #endregion
                                            List<Ar_ApDetl> ArrAr_ApDetl = new List<Ar_ApDetl>();
                                            if (OBmsBillAp_ArPopupView.ArrAr_ApDtl == null || OBmsBillAp_ArPopupView.ArrAr_ApDtl.Count() <= 1)
                                            {
                                                ArrAr_ApDetl.Add(OBmsBillAp_ArPopupView.Ar_ApDetl);
                                            }
                                            else
                                            {
                                                ArrAr_ApDetl = ArrAr_ApDetl.Concat(OBmsBillAp_ArPopupView.ArrAr_ApDtl).ToList();
                                            }
                                            if (ArrAr_ApDetl.Any())
                                            {
                                                #region 修改主单 实际金额

                                                System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap> entry = WebdbContxt.Entry<Bms_Bill_Ap>(OBms_Bill_Ap);
                                                OBms_Bill_Ap.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                                entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                                decimal T_Bill_Account2 = 0;
                                                if (ArrAr_ApDetl.Count > 1)
                                                    T_Bill_Account2 = ArrAr_ApDetl.Sum(x => x.Account2);
                                                else
                                                    T_Bill_Account2 = ArrAr_ApDetl[0].Account2;

                                                OBms_Bill_Ap.Bill_Account2 += T_Bill_Account2;
                                                entry.Property(t => t.Bill_Account2).IsModified = true; //设置要更新的属性

                                                dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ap.Bill_HasTax, OBms_Bill_Ap.Bill_TaxRate, OBms_Bill_Ap.Bill_Account2);
                                                if (OCalcTaxRate.Success)
                                                {
                                                    //价（实际金额）
                                                    OBms_Bill_Ap.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                                    entry.Property(t => t.Bill_Amount).IsModified = true; //设置要更新的属性
                                                    //税金 （实际金额 * 税率）
                                                    OBms_Bill_Ap.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                                    entry.Property(t => t.Bill_TaxAmount).IsModified = true; //设置要更新的属性
                                                    //价税合计 (价+税金)
                                                    OBms_Bill_Ap.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                                    entry.Property(t => t.Bill_AmountTaxTotal).IsModified = true; //设置要更新的属性

                                                    OBms_Bill_Ap.BillEDITID = CurrentAppUser.Id;
                                                    entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                    OBms_Bill_Ap.BillEDITWHO = CurrentAppUser.UserName;
                                                    entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                    OBms_Bill_Ap.BillEDITTS = DateTime.Now;
                                                    entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                                }
                                                else
                                                {
                                                    ErrMsg = "修改 应付数据-错误：计算价/税/价税合计出错！";
                                                    return Json(new { Success = false, ErrMsg = ErrMsg });
                                                }

                                                #endregion
                                            }

                                            foreach (var item in ArrAr_ApDetl)
                                            {
                                                if (!string.IsNullOrWhiteSpace(OBmsBillAp_ArPopupView.Dzbh) &&
                                                    OBmsBillAp_ArPopupView.OPS_M_OrdId > 0 &&
                                                   !string.IsNullOrWhiteSpace(item.Charge_Code) &&
                                                   !string.IsNullOrWhiteSpace(item.Charge_Desc))
                                                {
                                                    Bms_Bill_Ap_Dtl OBms_Bill_Ap_Dtl = new Bms_Bill_Ap_Dtl();
                                                    OBms_Bill_Ap_Dtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                                                    OBms_Bill_Ap_Dtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                                                    OBms_Bill_Ap_Dtl.Dzbh = OBmsBillAp_ArPopupView.Dzbh;
                                                    OBms_Bill_Ap_Dtl.Ops_M_OrdId = OBmsBillAp_ArPopupView.OPS_M_OrdId;
                                                    OBms_Bill_Ap_Dtl.Charge_Code = item.Charge_Code;
                                                    OBms_Bill_Ap_Dtl.Charge_Desc = item.Charge_Desc;
                                                    OBms_Bill_Ap_Dtl.Unitprice2 = item.Unitprice2;
                                                    OBms_Bill_Ap_Dtl.Qty = item.Qty;
                                                    OBms_Bill_Ap_Dtl.Account2 = item.Account2;
                                                    OBms_Bill_Ap_Dtl.Summary = item.Summary;
                                                    OBms_Bill_Ap_Dtl.Bms_Bill_Ap_Id = OBms_Bill_Ap.Id;
                                                    OBms_Bill_Ap_Dtl.Line_No = OBms_Bill_Ap.Line_No;
                                                    var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, OBmsBillAp_ArPopupView.Dzbh + "_" + OBms_Bill_Ap.Line_No, true);
                                                    OBms_Bill_Ap_Dtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                                    Bms_Bill_Ap_DtlRep.Insert(OBms_Bill_Ap_Dtl);
                                                }
                                                else
                                                {
                                                    ErrMsg = "新增 应付数据，业务编号/主键，或费用名称/说明 为空！";
                                                    break;
                                                }
                                            }
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }
            }
            else
                ErrMsg = "数据错误！";
            if (string.IsNullOrEmpty(ErrMsg))
            {
                try
                {
                    var ret = _unitOfWork.SaveChanges();
                    #region 操作日志
                    
                    #endregion
                    return Json(new { Success = true });
                }
                catch (Exception ex)
                {
                    ErrMsg = Common.GetExceptionMsg(ex);
                    return Json(new { Success = false, ErrMsg = ErrMsg });
                }
            }
            else
                return Json(new { Success = false, ErrMsg = ErrMsg });
        }

        /// <summary>
        /// 获取 应收/应付-编辑税金 视图
        /// </summary>
        /// <param name="Id">应收/应付-ID</param>
        /// <param name="IsBmsBillAr">应收/应付</param>
        /// <returns></returns>
        public ActionResult TaxRateEdit_PopupWin(int Id = 0, bool? IsBmsBillAr = null)
        {
            Ar_ApTaxRateEditView OAr_ApTaxRateEditView = new Ar_ApTaxRateEditView();
            OAr_ApTaxRateEditView.IsBms_Bill_Ar = IsBmsBillAr;
            if (IsBmsBillAr == null)
            {
                ModelState.AddModelError("", "数据格式错误，应收/付 不明确！");
                OAr_ApTaxRateEditView.IsBms_Bill_Ar = null;
            }
            else
            {
                if ((bool)IsBmsBillAr)
                {
                    #region 应收数据

                    var OBmsBillAr = _bms_Bill_ArService.Queryable().Where(x => x.Id == Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                    if (OBmsBillAr == null || OBmsBillAr.Id <= 0)
                    {
                        ModelState.AddModelError("", "应收数据，已被删除/作废！");
                        OAr_ApTaxRateEditView.IsBms_Bill_Ar = null;
                    }
                    else
                    {
                        if (!OBmsBillAr.Sumbmit_Status)
                        {
                            ModelState.AddModelError("", "应收数据，没有提交，无法修改！");
                            OAr_ApTaxRateEditView.IsBms_Bill_Ar = null;
                        }
                        else
                        {
                            OAr_ApTaxRateEditView.Bms_Bill_ArAp_Id = OBmsBillAr.Id;
                            OAr_ApTaxRateEditView.Bill_Account2 = OBmsBillAr.Bill_Account2;
                            OAr_ApTaxRateEditView.Bill_Amount = OBmsBillAr.Bill_Amount;
                            OAr_ApTaxRateEditView.Bill_TaxAmount = OBmsBillAr.Bill_TaxAmount;
                            var NewBill_TaxAmount = Math.Round(OBmsBillAr.Bill_TaxAmount, 2).ToString();
                            var idx = NewBill_TaxAmount.IndexOf(".");
                            if (idx > 0)
                            {
                                int Bill_TaxAmount_Precision = 0;
                                int Bill_TaxAmount_Profix = 0;
                                var Profix = NewBill_TaxAmount.Substring(0, idx);
                                var Precision = NewBill_TaxAmount.Substring(idx + 1);
                                if (int.TryParse(Profix, out Bill_TaxAmount_Profix))
                                {
                                    OAr_ApTaxRateEditView.Bill_TaxAmount_Profix = Bill_TaxAmount_Profix;
                                }
                                else
                                {
                                    ModelState.AddModelError("", "应收数据，税金截取小数出错！");
                                    OAr_ApTaxRateEditView.IsBms_Bill_Ar = null;
                                }
                                if (int.TryParse(Precision, out Bill_TaxAmount_Precision))
                                {
                                    OAr_ApTaxRateEditView.Bill_TaxAmount_Precision = Bill_TaxAmount_Precision;
                                }
                                else
                                {
                                    ModelState.AddModelError("", "应收数据，税金截取小数出错！");
                                    OAr_ApTaxRateEditView.IsBms_Bill_Ar = null;
                                }
                            }
                            else
                            {
                                OAr_ApTaxRateEditView.Bill_TaxAmount_Profix = Convert.ToInt32(NewBill_TaxAmount);
                                OAr_ApTaxRateEditView.Bill_TaxAmount_Precision = 0;
                            }
                            OAr_ApTaxRateEditView.Bill_AmountTaxTotal = OBmsBillAr.Bill_AmountTaxTotal;
                        }
                    }

                    #endregion
                }
                else
                {
                    #region 应付数据

                    var Bms_Bill_ApRep = _unitOfWork.Repository<Bms_Bill_Ap>();
                    var OBmsBillAp = Bms_Bill_ApRep.Queryable().Where(x => x.Id == Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                    if (OBmsBillAp == null || OBmsBillAp.Id <= 0)
                    {
                        ModelState.AddModelError("", "应付数据，已被删除/作废！");
                        OAr_ApTaxRateEditView.IsBms_Bill_Ar = null;
                    }
                    else
                    {
                        if (!OBmsBillAp.Sumbmit_Status)
                        {
                            ModelState.AddModelError("", "应付数据，没有提交，无法修改！");
                            OAr_ApTaxRateEditView.IsBms_Bill_Ar = null;
                        }
                        else
                        {
                            OAr_ApTaxRateEditView.Bms_Bill_ArAp_Id = OBmsBillAp.Id;
                            OAr_ApTaxRateEditView.Bill_Account2 = OBmsBillAp.Bill_Account2;
                            OAr_ApTaxRateEditView.Bill_Amount = OBmsBillAp.Bill_Amount;
                            OAr_ApTaxRateEditView.Bill_TaxAmount = OBmsBillAp.Bill_TaxAmount;
                            var NewBill_TaxAmount = Math.Round(OBmsBillAp.Bill_TaxAmount, 2).ToString();
                            var idx = NewBill_TaxAmount.IndexOf(".");
                            if (idx > 0)
                            {
                                int Bill_TaxAmount_Precision = 0;
                                int Bill_TaxAmount_Profix = 0;
                                var Profix = NewBill_TaxAmount.Substring(0, idx);
                                var Precision = NewBill_TaxAmount.Substring(idx + 1);
                                if (int.TryParse(Profix, out Bill_TaxAmount_Profix))
                                {
                                    OAr_ApTaxRateEditView.Bill_TaxAmount_Profix = Bill_TaxAmount_Profix;
                                }
                                else
                                {
                                    ModelState.AddModelError("", "应付数据，税金截取小数出错！");
                                    OAr_ApTaxRateEditView.IsBms_Bill_Ar = null;
                                }
                                if (int.TryParse(Precision, out Bill_TaxAmount_Precision))
                                {
                                    OAr_ApTaxRateEditView.Bill_TaxAmount_Precision = Bill_TaxAmount_Precision;
                                }
                                else
                                {
                                    ModelState.AddModelError("", "应付数据，税金截取小数出错！");
                                    OAr_ApTaxRateEditView.IsBms_Bill_Ar = null;
                                }
                            }
                            else
                            {
                                OAr_ApTaxRateEditView.Bill_TaxAmount_Profix = Convert.ToInt32(NewBill_TaxAmount);
                                OAr_ApTaxRateEditView.Bill_TaxAmount_Precision = 0;
                            }
                            OAr_ApTaxRateEditView.Bill_AmountTaxTotal = OBmsBillAp.Bill_AmountTaxTotal;
                        }
                    }

                    #endregion
                }
            }

            return PartialView("TaxRateEdit_PopupWin", OAr_ApTaxRateEditView);
        }

        /// <summary>
        /// 保存应收/应付-编辑税金
        /// </summary>
        /// <param name="OAr_ApTaxRateEditView">应收/应付-编辑税金 视图</param>
        /// <returns></returns>
        public ActionResult SaveTaxRateEditPopupWin(Ar_ApTaxRateEditView OAr_ApTaxRateEditView)
        {
            if (!Request.IsAjaxRequest())
                return Content("请求错误，只允许Ajax请求！");
            var ErrMsg = "";//错误信息
            if (OAr_ApTaxRateEditView.IsBms_Bill_Ar == null)
            {
                ErrMsg = "数据格式错误，应收/付 不明确！";
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
            else
            {
                if ((bool)OAr_ApTaxRateEditView.IsBms_Bill_Ar)
                {
                    #region 应收数据

                    var OBmsBillAr = _bms_Bill_ArService.Queryable().Where(x => x.Id == OAr_ApTaxRateEditView.Bms_Bill_ArAp_Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                    if (OBmsBillAr == null || OBmsBillAr.Id <= 0)
                    {
                        ErrMsg = "应收数据，已被删除/作废！";
                    }
                    else
                    {
                        if (!OBmsBillAr.Sumbmit_Status)
                        {
                            ErrMsg = "应收数据，没有提交，无法修改！";
                        }
                        else
                        {
                            if (!ModelState.IsValid)
                            {
                                var modelStateErrors = String.Join("", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
                                ErrMsg = "应收数据-验证错误：" + modelStateErrors + "！";
                            }

                            decimal Bill_TaxAmount = OBmsBillAr.Bill_TaxAmount;
                            var Bill_TaxAmountStr = Math.Round(Bill_TaxAmount, 2).ToString();
                            var idx = Bill_TaxAmountStr.IndexOf(".");
                            string NewBill_TaxAmountStr = "";
                            if (idx > 0)
                            {
                                string Bill_TaxAmountProfix = Bill_TaxAmountStr.Substring(0, idx);
                                NewBill_TaxAmountStr = Bill_TaxAmountProfix + "." + OAr_ApTaxRateEditView.Bill_TaxAmount_Precision;
                            }
                            else
                            {
                                NewBill_TaxAmountStr = Bill_TaxAmountStr + "." + OAr_ApTaxRateEditView.Bill_TaxAmount_Precision;
                            }
                            if (decimal.TryParse(NewBill_TaxAmountStr, out Bill_TaxAmount))
                            {
                                OBmsBillAr.Bill_TaxAmount = Bill_TaxAmount;

                                #region 保存数据

                                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                                System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> entry = WebdbContxt.Entry<Bms_Bill_Ar>(OBmsBillAr);
                                OBmsBillAr.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                entry.Property(x => x.Bill_TaxAmount).IsModified = true;

                                OBmsBillAr.BillEDITID = CurrentAppUser.Id;
                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                OBmsBillAr.BillEDITWHO = CurrentAppUser.UserName;
                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                OBmsBillAr.BillEDITTS = DateTime.Now;
                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性

                                #endregion
                            }
                            else
                            {
                                ErrMsg = "应收数据，税金转换出错！";
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region 应付数据

                    var Bms_Bill_ApRep = _unitOfWork.Repository<Bms_Bill_Ap>();
                    var OBmsBillAp = Bms_Bill_ApRep.Queryable().Where(x => x.Id == OAr_ApTaxRateEditView.Bms_Bill_ArAp_Id && x.Status == AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                    if (OBmsBillAp == null || OBmsBillAp.Id <= 0)
                    {
                        ErrMsg = "应付数据，已被删除/作废！";
                    }
                    else
                    {
                        if (!OBmsBillAp.Sumbmit_Status)
                        {
                            ErrMsg = "应付数据，没有提交，无法修改！";
                        }
                        else
                        {
                            if (!ModelState.IsValid)
                            {
                                var modelStateErrors = String.Join("", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
                                ErrMsg = "应收数据-验证错误：" + modelStateErrors + "！";
                            }

                            decimal Bill_TaxAmount = OBmsBillAp.Bill_TaxAmount;
                            var Bill_TaxAmountStr = Math.Round(Bill_TaxAmount, 2).ToString();
                            var idx = Bill_TaxAmountStr.IndexOf(".");
                            string NewBill_TaxAmountStr = "";
                            if (idx > 0)
                            {
                                string Bill_TaxAmountProfix = Bill_TaxAmountStr.Substring(0, idx);
                                NewBill_TaxAmountStr = Bill_TaxAmountProfix + "." + OAr_ApTaxRateEditView.Bill_TaxAmount_Precision;
                            }
                            else
                            {
                                NewBill_TaxAmountStr = Bill_TaxAmountStr + "." + OAr_ApTaxRateEditView.Bill_TaxAmount_Precision;
                            }
                            if (decimal.TryParse(NewBill_TaxAmountStr, out Bill_TaxAmount))
                            {
                                OBmsBillAp.Bill_TaxAmount = Bill_TaxAmount;

                                #region 保存数据

                                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                                System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap> entry = WebdbContxt.Entry<Bms_Bill_Ap>(OBmsBillAp);
                                OBmsBillAp.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                entry.Property(x => x.Bill_TaxAmount).IsModified = true;

                                OBmsBillAp.BillEDITID = CurrentAppUser.Id;
                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                OBmsBillAp.BillEDITWHO = CurrentAppUser.UserName;
                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                OBmsBillAp.BillEDITTS = DateTime.Now;
                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性

                                #endregion
                            }
                            else
                            {
                                ErrMsg = "应收数据，税金转换出错！";
                            }
                        }
                    }

                    #endregion
                }
                if (string.IsNullOrEmpty(ErrMsg))
                {
                    try
                    {
                        var ret = _unitOfWork.SaveChanges();
                        return Json(new { Success = true });
                    }
                    catch (Exception ex)
                    {
                        ErrMsg = Common.GetExceptionMsg(ex);
                        return Json(new { Success = false, ErrMsg = ErrMsg });
                    }
                }
                else
                    return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 应收/应付 费用单列账单
        /// 将费用明细，重新创建一个 应收/应付 表头 表体
        /// </summary>
        /// <param name="ArrArApFeeIds">应收/应付 费用明细-Id</param>
        /// <param name="IsBmsBillAr">应收/应付</param>
        /// <returns></returns>
        public ActionResult Fee2OneColumn(List<int> ArrArApFeeIds, bool? IsBmsBillAr = null)
        {
            string ErrMsg = "";
            string ContentFee2OneColumn = "";
            int MBLID = 0;
            if (IsBmsBillAr == null)
            {
                ErrMsg = "数据格式错误，应收/付 不明确！";
            }
            else
            {
                if (ArrArApFeeIds == null || !ArrArApFeeIds.Any())
                {
                    ErrMsg = "费用明细，主键Id，不能为空";
                }
                else
                {
                    if (ArrArApFeeIds.Any(x => x <= 0))
                        ErrMsg = "费用明细，主键Id必须都大于0";
                    else
                    {
                        WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                        if ((bool)IsBmsBillAr)
                        {
                            #region 应收

                            var BmsBillArDtlRep = _unitOfWork.Repository<Bms_Bill_Ar_Dtl>();
                            var ArrBmsBillArDtl = BmsBillArDtlRep.Queryable().Where(x => ArrArApFeeIds.Contains(x.Id) && x.Status == AirOutEnumType.UseStatusEnum.Enable).Include(x => x.OBms_Bill_Ar).ToList();
                            if (ArrBmsBillArDtl != null && ArrBmsBillArDtl.Any() && ArrBmsBillArDtl.Count != ArrArApFeeIds.Count())
                            {
                                ErrMsg = "应收费用明细，所选数据，有数据已被删除/作废";
                            }
                            else
                            {
                                if (ArrBmsBillArDtl.Any(x => x.AuditStatus < 0 || x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess))
                                {
                                    ErrMsg = "应收费用明细，所选数据，有数据已被审核，无法操作";
                                }
                                else
                                {
                                    var OBms_Bill_Ar = ArrBmsBillArDtl.FirstOrDefault().OBms_Bill_Ar;
                                    MBLID = OBms_Bill_Ar.Ops_M_OrdId;
                                    if (ArrBmsBillArDtl.Any(x => x.Bms_Bill_Ar_Id != OBms_Bill_Ar.Id))
                                    {
                                        ErrMsg = "应收费用明细，所选数据，不在同一费用里";
                                    }
                                    else
                                    {
                                        #region 新增应收表头

                                        var NewBms_Bill_Ar = new Bms_Bill_Ar();
                                        NewBms_Bill_Ar.Bill_Date = DateTime.Now;
                                        NewBms_Bill_Ar.Status = AirOutEnumType.UseStatusEnum.Enable;
                                        NewBms_Bill_Ar.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                                        NewBms_Bill_Ar.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                                        NewBms_Bill_Ar.Bill_Type = OBms_Bill_Ar.Bill_Type;
                                        NewBms_Bill_Ar.Money_Code = OBms_Bill_Ar.Money_Code;
                                        NewBms_Bill_Ar.Org_Money_Code = OBms_Bill_Ar.Org_Money_Code;
                                        NewBms_Bill_Ar.Bill_Object_Id = OBms_Bill_Ar.Bill_Object_Id;
                                        NewBms_Bill_Ar.Bill_Object_Name = OBms_Bill_Ar.Bill_Object_Name;
                                        NewBms_Bill_Ar.Ops_M_OrdId = OBms_Bill_Ar.Ops_M_OrdId;
                                        NewBms_Bill_Ar.Dzbh = OBms_Bill_Ar.Dzbh;
                                        NewBms_Bill_Ar.MBL = OBms_Bill_Ar.MBL;
                                        NewBms_Bill_Ar.Payway = OBms_Bill_Ar.Payway;
                                        NewBms_Bill_Ar.Remark = OBms_Bill_Ar.Remark;
                                        NewBms_Bill_Ar.Bill_TaxRateType = OBms_Bill_Ar.Bill_TaxRateType;
                                        NewBms_Bill_Ar.Bill_TaxRate = OBms_Bill_Ar.Bill_TaxRate;
                                        NewBms_Bill_Ar.Bill_HasTax = OBms_Bill_Ar.Bill_HasTax;
                                        NewBms_Bill_Ar.Bill_Account2 = ArrBmsBillArDtl.Sum(x => x.Account2);
                                        var Line_No = SequenceBuilder.NextBmsBillArLineNo(true, OBms_Bill_Ar.Dzbh);
                                        NewBms_Bill_Ar.Line_No = Convert.ToInt32(Line_No);

                                        #region 计算 价 税 价税合计

                                        dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ar.Bill_HasTax, OBms_Bill_Ar.Bill_TaxRate, NewBms_Bill_Ar.Bill_Account2);
                                        if (OCalcTaxRate.Success)
                                        {
                                            //价（实际金额）
                                            NewBms_Bill_Ar.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                            //税金 （实际金额 * 税率）
                                            NewBms_Bill_Ar.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                            //价税合计 (价+税金)
                                            NewBms_Bill_Ar.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                        }
                                        else
                                        {
                                            ErrMsg = "新增 应收数据-错误：计算价/税/价税合计出错！";
                                            return Json(new { Success = false, ErrMsg = ErrMsg });
                                        }

                                        #endregion

                                        _bms_Bill_ArService.Insert(NewBms_Bill_Ar);

                                        #endregion

                                        #region 编辑 费用明细

                                        foreach (var item in ArrBmsBillArDtl)
                                        {
                                            System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar_Dtl> entry = WebdbContxt.Entry<Bms_Bill_Ar_Dtl>(item);
                                            item.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                            entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）

                                            item.Bms_Bill_Ar_Id = NewBms_Bill_Ar.Id;
                                            item.Line_No = NewBms_Bill_Ar.Line_No;
                                            var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh + "_" + item.Line_No, true);
                                            item.Line_Id = Convert.ToInt32(DtlLine_No);

                                            entry.Property(x => x.Bms_Bill_Ar_Id).IsModified = true;
                                            entry.Property(x => x.Line_No).IsModified = true;
                                            entry.Property(x => x.Line_Id).IsModified = true;
                                        }

                                        #endregion

                                        #region 编辑 应收账单 实际金额 价 税 价税合计

                                        System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> entry1 = WebdbContxt.Entry<Bms_Bill_Ar>(OBms_Bill_Ar);
                                        OBms_Bill_Ar.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                        entry1.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）

                                        OBms_Bill_Ar.Bill_Account2 += -NewBms_Bill_Ar.Bill_Account2;

                                        #region 计算 价 税 价税合计

                                        dynamic O_CalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ar.Bill_HasTax, OBms_Bill_Ar.Bill_TaxRate, OBms_Bill_Ar.Bill_Account2);
                                        if (O_CalcTaxRate.Success)
                                        {
                                            //价（实际金额）
                                            OBms_Bill_Ar.Bill_Amount = O_CalcTaxRate.Bill_Amount;
                                            //税金 （实际金额 * 税率）
                                            OBms_Bill_Ar.Bill_TaxAmount = O_CalcTaxRate.Bill_TaxAmount;
                                            //价税合计 (价+税金)
                                            OBms_Bill_Ar.Bill_AmountTaxTotal = O_CalcTaxRate.Bill_AmountTaxTotal;
                                        }
                                        else
                                        {
                                            ErrMsg = "编辑 应收账单-错误：计算价/税/价税合计出错！";
                                            return Json(new { Success = false, ErrMsg = ErrMsg });
                                        }

                                        #endregion

                                        entry1.Property(x => x.Bill_Account2).IsModified = true;
                                        entry1.Property(x => x.Bill_Amount).IsModified = true;
                                        entry1.Property(x => x.Bill_TaxAmount).IsModified = true;
                                        entry1.Property(x => x.Bill_AmountTaxTotal).IsModified = true;

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region 应付

                            var BmsBillApDtlRep = _unitOfWork.Repository<Bms_Bill_Ap_Dtl>();
                            var ArrBmsBillApDtl = BmsBillApDtlRep.Queryable().Where(x => ArrArApFeeIds.Contains(x.Id) && x.Status == AirOutEnumType.UseStatusEnum.Enable).Include(x => x.OBms_Bill_Ap).ToList();
                            if (ArrBmsBillApDtl != null && ArrBmsBillApDtl.Any() && ArrBmsBillApDtl.Count != ArrArApFeeIds.Count())
                            {
                                ErrMsg = "应付费用明细，所选数据，有数据已被删除/作废";
                            }
                            else
                            {
                                if (ArrBmsBillApDtl.Any(x => x.AuditStatus < 0 || x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess))
                                {
                                    ErrMsg = "应付费用明细，所选数据，有数据已被审核，无法操作";
                                }
                                else
                                {
                                    var OBmsBillAp = ArrBmsBillApDtl.FirstOrDefault().OBms_Bill_Ap;
                                    if (ArrBmsBillApDtl.Any(x => x.Bms_Bill_Ap_Id != OBmsBillAp.Id))
                                    {
                                        ErrMsg = "应付费用明细，所选数据，不在同一费用里";
                                    }
                                    else
                                    {
                                        var BmsBillApRep = _unitOfWork.Repository<Bms_Bill_Ap>();
                                        MBLID = OBmsBillAp.Ops_M_OrdId;

                                        #region 新增应付表头

                                        var NewBms_Bill_Ap = new Bms_Bill_Ap();
                                        NewBms_Bill_Ap.Bill_Date = DateTime.Now;
                                        NewBms_Bill_Ap.Status = AirOutEnumType.UseStatusEnum.Enable;
                                        NewBms_Bill_Ap.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                                        NewBms_Bill_Ap.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                                        NewBms_Bill_Ap.Bill_Type = OBmsBillAp.Bill_Type;
                                        NewBms_Bill_Ap.Money_Code = OBmsBillAp.Money_Code;
                                        NewBms_Bill_Ap.Org_Money_Code = OBmsBillAp.Org_Money_Code;
                                        NewBms_Bill_Ap.Bill_Object_Id = OBmsBillAp.Bill_Object_Id;
                                        NewBms_Bill_Ap.Bill_Object_Name = OBmsBillAp.Bill_Object_Name;
                                        NewBms_Bill_Ap.Ops_M_OrdId = OBmsBillAp.Ops_M_OrdId;
                                        NewBms_Bill_Ap.Dzbh = OBmsBillAp.Dzbh;
                                        NewBms_Bill_Ap.MBL = OBmsBillAp.MBL;
                                        NewBms_Bill_Ap.Payway = OBmsBillAp.Payway;
                                        NewBms_Bill_Ap.Remark = OBmsBillAp.Remark;
                                        NewBms_Bill_Ap.Bill_TaxRateType = OBmsBillAp.Bill_TaxRateType;
                                        NewBms_Bill_Ap.Bill_TaxRate = OBmsBillAp.Bill_TaxRate;
                                        NewBms_Bill_Ap.Bill_HasTax = OBmsBillAp.Bill_HasTax;
                                        var Line_No = SequenceBuilder.NextBmsBillArLineNo(false, OBmsBillAp.Dzbh);
                                        NewBms_Bill_Ap.Line_No = Convert.ToInt32(Line_No);
                                        NewBms_Bill_Ap.Bill_Account2 = ArrBmsBillApDtl.Sum(x => x.Account2);

                                        #region 计算 价 税 价税合计

                                        dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBmsBillAp.Bill_HasTax, OBmsBillAp.Bill_TaxRate, NewBms_Bill_Ap.Bill_Account2);
                                        if (OCalcTaxRate.Success)
                                        {
                                            //价（实际金额）
                                            NewBms_Bill_Ap.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                            //税金 （实际金额 * 税率）
                                            NewBms_Bill_Ap.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                            //价税合计 (价+税金)
                                            NewBms_Bill_Ap.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                        }
                                        else
                                        {
                                            ErrMsg = "新增 应付数据-错误：计算价/税/价税合计出错！";
                                            return Json(new { Success = false, ErrMsg = ErrMsg });
                                        }

                                        #endregion

                                        BmsBillApRep.Insert(NewBms_Bill_Ap);

                                        #endregion

                                        #region 编辑 费用明细

                                        foreach (var item in ArrBmsBillApDtl)
                                        {
                                            System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap_Dtl> entry = WebdbContxt.Entry<Bms_Bill_Ap_Dtl>(item);
                                            item.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                            entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）

                                            item.Bms_Bill_Ap_Id = NewBms_Bill_Ap.Id;
                                            item.Line_No = NewBms_Bill_Ap.Line_No;
                                            var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, item.Dzbh + "_" + item.Line_No, true);
                                            item.Line_Id = Convert.ToInt32(DtlLine_No);

                                            entry.Property(x => x.Bms_Bill_Ap_Id).IsModified = true;
                                            entry.Property(x => x.Line_No).IsModified = true;
                                            entry.Property(x => x.Line_Id).IsModified = true;
                                        }

                                        #endregion

                                        #region 编辑 应付账单 实际金额 价 税 价税合计

                                        System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap> entry1 = WebdbContxt.Entry<Bms_Bill_Ap>(OBmsBillAp);
                                        OBmsBillAp.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                                        entry1.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）

                                        OBmsBillAp.Bill_Account2 += -NewBms_Bill_Ap.Bill_Account2;

                                        #region 计算 价 税 价税合计

                                        dynamic O_CalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBmsBillAp.Bill_HasTax, OBmsBillAp.Bill_TaxRate, OBmsBillAp.Bill_Account2);
                                        if (O_CalcTaxRate.Success)
                                        {
                                            //价（实际金额）
                                            OBmsBillAp.Bill_Amount = O_CalcTaxRate.Bill_Amount;
                                            //税金 （实际金额 * 税率）
                                            OBmsBillAp.Bill_TaxAmount = O_CalcTaxRate.Bill_TaxAmount;
                                            //价税合计 (价+税金)
                                            OBmsBillAp.Bill_AmountTaxTotal = O_CalcTaxRate.Bill_AmountTaxTotal;
                                        }
                                        else
                                        {
                                            ErrMsg = "编辑 应付账单-错误：计算价/税/价税合计出错！";
                                            return Json(new { Success = false, ErrMsg = ErrMsg });
                                        }

                                        #endregion

                                        entry1.Property(x => x.Bill_Account2).IsModified = true;
                                        entry1.Property(x => x.Bill_Amount).IsModified = true;
                                        entry1.Property(x => x.Bill_TaxAmount).IsModified = true;
                                        entry1.Property(x => x.Bill_AmountTaxTotal).IsModified = true;

                                        #endregion

                                        ContentFee2OneColumn = "费用单列 是否应收:" + IsBmsBillAr + " DZBH:" + OBmsBillAp.Dzbh + " NewLine_No:" + Convert.ToInt32(Line_No) + " FromLine_No:" + OBmsBillAp.Line_No;
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(ErrMsg))
            {
                try
                {
                    _changeOrderHistoryService.InsertChangeOrdHistory(MBLID, "Bms_Bill_Ar", ChangeOrderHistory.EnumChangeType.Insert, ContentFee2OneColumn, 0, 0, 1);
                    var ret = _unitOfWork.SaveChanges();
                    return Json(new { Success = true });
                }
                catch (Exception ex)
                {
                    ErrMsg = Common.GetExceptionMsg(ex);
                    return Json(new { Success = false, ErrMsg = ErrMsg });
                }
            }
            else
                return Json(new { Success = false, ErrMsg = ErrMsg });
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "bms_bill_ar_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _bms_Bill_ArService.ExportExcel(filterRules, sort, order);
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