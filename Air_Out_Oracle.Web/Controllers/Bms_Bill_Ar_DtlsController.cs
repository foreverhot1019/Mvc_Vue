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
    public class Bms_Bill_Ar_DtlsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Bms_Bill_Ar_Dtl>, Repository<Bms_Bill_Ar_Dtl>>();
        //container.RegisterType<IBms_Bill_Ar_DtlService, Bms_Bill_Ar_DtlService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IBms_Bill_ArService _bms_Bill_ArService;
        private readonly IBms_Bill_Ar_DtlService _bms_Bill_Ar_DtlService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        //验证权限的名称
        private string ControllerQXName = "/Bms_Bill_Ars";
        //
        private RedisHelp.RedisHelper ORedisHelper { get; set; }

        public Bms_Bill_Ar_DtlsController(IBms_Bill_Ar_DtlService bms_Bill_Ar_DtlService, IBms_Bill_ArService bms_Bill_ArService, IUnitOfWorkAsync unitOfWork)
        {
            _bms_Bill_ArService = bms_Bill_ArService;
            _bms_Bill_Ar_DtlService = bms_Bill_Ar_DtlService;
            _unitOfWork = unitOfWork;
            ORedisHelper = new RedisHelp.RedisHelper();
        }

        // GET: Bms_Bill_Ar_Dtls/Index
        public ActionResult Index()
        {
            //var bms_bill_ar_dtl  = _bms_Bill_Ar_DtlService.Queryable().Include(b => b.OBms_Bill_Ar).AsQueryable();
            //return View(bms_bill_ar_dtl);
            return View();
        }

        // Get :Bms_Bill_Ar_Dtls/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var bms_bill_ar_dtl = _bms_Bill_Ar_DtlService.Query(new Bms_Bill_Ar_DtlQuery().Withfilter(filters)).Include(b => b.OBms_Bill_Ar).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var data_rows = bms_bill_ar_dtl.Select(n => new
            {
                n.Id,
                n.Bms_Bill_Ar_Id,
                n.Dzbh,
                n.Line_No,
                n.Line_Id,
                n.Charge_Code,
                n.Charge_Desc,
                n.Unitprice,
                n.Unitprice2,
                n.Qty,
                n.Account,
                n.Account2,
                n.Money_Code,
                n.Summary,
                n.Collate_Id,
                n.Collate_Name,
                n.Collate_Date,
                n.Collate_Status,
                n.Collate_No,
                n.User_Id,
                n.User_Name,
                n.Status,
                n.Create_Status,
                n.OperatingPoint,
                n.ADDID,
                n.ADDTS,
                n.ADDWHO,
                n.EDITID,
                n.EDITTS,
                n.EDITWHO,
                n.BillEDITID,
                n.BillEDITWHO,
                n.BillEDITTS
            }).ToList();
            var ArrAppUser = (List<ApplicationUser>)CacheHelper.Get_SetApplicationUser();//帐户
            var datarows = from n in data_rows
                           join a in ArrAppUser on n.ADDID equals a.Id into a_tmp
                           from atmp in a_tmp.DefaultIfEmpty()
                           join b in ArrAppUser on n.ADDID equals b.Id into b_tmp
                           from btmp in b_tmp.DefaultIfEmpty()
                           join m in ArrAppUser on n.BillEDITID equals m.Id into m_tmp
                           from mtmp in m_tmp.DefaultIfEmpty()
                           select new
                           {
                               n.Id,
                               n.Bms_Bill_Ar_Id,
                               n.Dzbh,
                               n.Line_No,
                               n.Line_Id,
                               n.Charge_Code,
                               n.Charge_Desc,
                               n.Unitprice,
                               Unitprice2 = n.Unitprice2 == 0 ? "0" : Math.Round((decimal)n.Unitprice2, 2).ToString("#0.00"),
                               n.Qty,
                               Account = n.Account == 0 ? "0" : Math.Round((decimal)n.Account, 2).ToString("#0.00"),
                               Account2 = n.Account2 == 0 ? "0" : Math.Round((decimal)n.Account2, 2).ToString("#0.00"),
                               n.Money_Code,
                               n.Summary,
                               n.Collate_Id,
                               n.Collate_Name,
                               n.Collate_Date,
                               n.Collate_Status,
                               n.Collate_No,
                               n.User_Id,
                               n.User_Name,
                               n.Status,
                               n.Create_Status,
                               n.OperatingPoint,
                               n.ADDID,
                               n.ADDTS,
                               n.ADDWHO,
                               ADDWHONAME = atmp == null ? "" : atmp.UserNameDesc,
                               n.EDITID,
                               n.EDITTS,
                               n.EDITWHO,
                               EDITWHONAME = btmp == null ? "" : btmp.UserNameDesc,
                               n.BillEDITID,
                               n.BillEDITTS,
                               n.BillEDITWHO,
                               BillEDITWHONAME = mtmp == null ? "" : mtmp.UserNameDesc
                           };

            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(Bms_Bill_Ar_DtlChangeViewModel bms_bill_ar_dtl)
        {
            WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
            var ArrAr_Id = new List<int?>();//记录修改和新增的应收头Id
            if (bms_bill_ar_dtl.updated != null)
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

                foreach (var updated in bms_bill_ar_dtl.updated)
                {
                    _bms_Bill_Ar_DtlService.Update(updated);
                    //记录修改和新增的应付头Id
                    ArrAr_Id.Add(updated.Bms_Bill_Ar_Id);
                }
            }
            if (bms_bill_ar_dtl.deleted != null)
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

                var ArrDeltId = bms_bill_ar_dtl.deleted.Select(x => x.Id);
                var ArrDelt = _bms_Bill_Ar_DtlService.Queryable().Where(x => ArrDeltId.Contains(x.Id)).Include(x => x.OBms_Bill_Ar).ToList();
                if (ArrDelt.Any(x => x.OBms_Bill_Ar.AuditStatus < 0 || x.OBms_Bill_Ar.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess))
                {
                    //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
                    string ActionGuidName1 = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
                    string ActionGuid1 = ViewData[ActionGuidName1] == null ? "" : ViewData[ActionGuidName1].ToString();
                    return Json(new { Success = false, ErrMsg = "应收账单，部分数据已经审核，无法删除！", ActionGuidName = ActionGuidName1, ActionGuid = ActionGuid1 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var ArrArId = ArrDelt.GroupBy(x => x.Bms_Bill_Ar_Id).Select(x => x.Key);
                    foreach (var Id in ArrArId)
                    {
                        var QArrDelt = ArrDelt.Where(x => x.Bms_Bill_Ar_Id == Id);
                        if (QArrDelt.Any())
                        {
                            var DeltAccount2 = QArrDelt.Sum(x => x.Account2);
                            var OAr = QArrDelt.FirstOrDefault().OBms_Bill_Ar;

                            var entry = WebdbContxt.Entry<Bms_Bill_Ar>(OAr);
                            entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                            OAr.Bill_Account2 += -DeltAccount2;
                            entry.Property(x => x.Bill_Account2).IsModified = true;
                            if (OAr.Bill_Account2 > 0)
                            {
                                dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OAr.Bill_HasTax, OAr.Bill_TaxRate, OAr.Bill_Account2);
                                if (OCalcTaxRate.Success)
                                {
                                    //价（实际金额）
                                    OAr.Bill_Amount = OCalcTaxRate.Bill_Amount;//净金额-价
                                    entry.Property(x => x.Bill_Amount).IsModified = true;
                                    //税金 （实际金额 * 税率）
                                    OAr.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                    entry.Property(x => x.Bill_TaxAmount).IsModified = true;
                                    //价税合计 (价+税金)
                                    OAr.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                    entry.Property(x => x.Bill_AmountTaxTotal).IsModified = true;

                                    OAr.BillEDITID = CurrentAppUser.Id;
                                    entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                    OAr.BillEDITWHO = CurrentAppUser.UserName;
                                    entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                    OAr.BillEDITTS = DateTime.Now;
                                    entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                }
                                else
                                    return Json(new { Success = false, ErrMsg = "重新计算价，税，价税合计" });
                            }
                            else
                            {
                                //价（实际金额）
                                OAr.Bill_Amount = 0;//净金额-价
                                entry.Property(x => x.Bill_Amount).IsModified = true;
                                //税金 （实际金额 * 税率）
                                OAr.Bill_TaxAmount = 0;
                                entry.Property(x => x.Bill_TaxAmount).IsModified = true;
                                //价税合计 (价+税金)
                                OAr.Bill_AmountTaxTotal = 0;
                                entry.Property(x => x.Bill_AmountTaxTotal).IsModified = true;

                                OAr.BillEDITID = CurrentAppUser.Id;
                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                OAr.BillEDITWHO = CurrentAppUser.UserName;
                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                                OAr.BillEDITTS = DateTime.Now;
                                entry.Property(t => t.BillEDITID).IsModified = true; //设置要更新的属性
                            }
                            OAr.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                        }
                    }
                    foreach (var deleted in ArrDeltId)
                    {
                        _bms_Bill_Ar_DtlService.Delete(deleted);
                    }
                }
            }
            if (bms_bill_ar_dtl.inserted != null)
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

                foreach (var inserted in bms_bill_ar_dtl.inserted)
                {
                    _bms_Bill_Ar_DtlService.Insert(inserted);
                    //记录修改和新增的应付头Id
                    ArrAr_Id.Add(inserted.Bms_Bill_Ar_Id);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((bms_bill_ar_dtl.updated != null && bms_bill_ar_dtl.updated.Any()) ||
                (bms_bill_ar_dtl.deleted != null && bms_bill_ar_dtl.deleted.Any()) ||
                (bms_bill_ar_dtl.inserted != null && bms_bill_ar_dtl.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(bms_bill_ar_dtl);
                    if (bms_bill_ar_dtl.deleted != null && bms_bill_ar_dtl.deleted.Any())
                    {
                        foreach (var item in bms_bill_ar_dtl.deleted)
                        {
                            ORedisHelper.ListRightPush(Common.RedisKeyDelArAp_Dtl + ":" + DateTime.Now.ToString("yyyyMMdd"), "Bms_Bill_ArApDTL@|@" + item.Id.ToString() + "@|@1@|@" + DateTime.Now.ToString("yyyyMMdd HHmmss"));
                        }
                    }
                    #region 应收头数据-记录修改人

                    ArrAr_Id = ArrAr_Id.Where(x => x != null && x > 0).Distinct().ToList();
                    var CurrentAppUser = Utility.CurrentAppUser;
                    if (ArrAr_Id.Any() && CurrentAppUser != null && !string.IsNullOrWhiteSpace(CurrentAppUser.Id))
                    {
                        var UpdateSQL = "update bms_bill_ars t set t.EDITWHO='" + CurrentAppUser.UserName + "',t.EDITTS=sysdate,t.EDITID='" + CurrentAppUser.Id + "' where t.id in ('" + string.Join("','", ArrAr_Id) + "')";
                    }

                    #endregion
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
        /// 获取应收头数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBms_Bill_Ar()
        {
            var bms_bill_arRepository = _unitOfWork.Repository<Bms_Bill_Ar>();
            var data = bms_bill_arRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, Dzbh = n.Dzbh });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        // GET: Bms_Bill_Ar_Dtls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bms_Bill_Ar_Dtl bms_Bill_Ar_Dtl = _bms_Bill_Ar_DtlService.Find(id);
            if (bms_Bill_Ar_Dtl == null)
            {
                return HttpNotFound();
            }
            return View(bms_Bill_Ar_Dtl);
        }

        // GET: Bms_Bill_Ar_Dtls/Create
        public ActionResult Create()
        {
            Bms_Bill_Ar_Dtl bms_Bill_Ar_Dtl = new Bms_Bill_Ar_Dtl();
            //set default value
            var bms_bill_arRepository = _unitOfWork.Repository<Bms_Bill_Ar>();
            ViewBag.Bms_Bill_Ar_Id = new SelectList(bms_bill_arRepository.Queryable(), "Id", "Dzbh");
            return View(bms_Bill_Ar_Dtl);
        }

        // POST: Bms_Bill_Ar_Dtls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OBms_Bill_Ar,Id,Bms_Bill_Ar_Id,Dzbh,Line_No,Line_Id,Charge_Code,Charge_Desc,Unitprice,Unitprice2,Qty,Account,Account2,Money_Code,Summary,Ap_Check_No,Invoice_No,Invoice_Id,Invoice_Name,Invoice_Date,Invoice_Status,Chalk_No,Chalk_Id,Chalk_Name,Chalk_Date,Chalk_Status,Collate_Id,Collate_Name,Collate_Date,Collate_Status,Refer_No,Refer_Status,Blank_Status,Create_Status,Acc_Charge_Desc,Inv_Notes,Blank_Id,Blank_Name,Blank_Date,Refer_No_Out,Collate_No,User_Id,User_Name,Special_Status,Org_Account2,Box_Type,Nowent_Acc_Dtl,Accsubjid,Invos_Status,Cancel_Id,Cancel_Name,Cancel_Date,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Bms_Bill_Ar_Dtl bms_Bill_Ar_Dtl)
        {
            if (ModelState.IsValid)
            {
                _bms_Bill_Ar_DtlService.Insert(bms_Bill_Ar_Dtl);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Bms_Bill_Ar_Dtl record");
                return RedirectToAction("Index");
            }

            var bms_bill_arRepository = _unitOfWork.Repository<Bms_Bill_Ar>();
            ViewBag.Bms_Bill_Ar_Id = new SelectList(bms_bill_arRepository.Queryable(), "Id", "Dzbh", bms_Bill_Ar_Dtl.Bms_Bill_Ar_Id);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bms_Bill_Ar_Dtl);
        }

        // GET: Bms_Bill_Ar_Dtls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bms_Bill_Ar_Dtl bms_Bill_Ar_Dtl = _bms_Bill_Ar_DtlService.Find(id);
            if (bms_Bill_Ar_Dtl == null)
            {
                return HttpNotFound();
            }
            var bms_bill_arRepository = _unitOfWork.Repository<Bms_Bill_Ar>();
            ViewBag.Bms_Bill_Ar_Id = new SelectList(bms_bill_arRepository.Queryable(), "Id", "Dzbh", bms_Bill_Ar_Dtl.Bms_Bill_Ar_Id);
            return View(bms_Bill_Ar_Dtl);
        }

        // POST: Bms_Bill_Ar_Dtls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OBms_Bill_Ar,Id,Bms_Bill_Ar_Id,Dzbh,Line_No,Line_Id,Charge_Code,Charge_Desc,Unitprice,Unitprice2,Qty,Account,Account2,Money_Code,Summary,Ap_Check_No,Invoice_No,Invoice_Id,Invoice_Name,Invoice_Date,Invoice_Status,Chalk_No,Chalk_Id,Chalk_Name,Chalk_Date,Chalk_Status,Collate_Id,Collate_Name,Collate_Date,Collate_Status,Refer_No,Refer_Status,Blank_Status,Create_Status,Acc_Charge_Desc,Inv_Notes,Blank_Id,Blank_Name,Blank_Date,Refer_No_Out,Collate_No,User_Id,User_Name,Special_Status,Org_Account2,Box_Type,Nowent_Acc_Dtl,Accsubjid,Invos_Status,Cancel_Id,Cancel_Name,Cancel_Date,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Bms_Bill_Ar_Dtl bms_Bill_Ar_Dtl)
        {
            if (ModelState.IsValid)
            {
                bms_Bill_Ar_Dtl.ObjectState = ObjectState.Modified;
                _bms_Bill_Ar_DtlService.Update(bms_Bill_Ar_Dtl);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Bms_Bill_Ar_Dtl record");
                return RedirectToAction("Index");
            }
            var bms_bill_arRepository = _unitOfWork.Repository<Bms_Bill_Ar>();
            ViewBag.Bms_Bill_Ar_Id = new SelectList(bms_bill_arRepository.Queryable(), "Id", "Dzbh", bms_Bill_Ar_Dtl.Bms_Bill_Ar_Id);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bms_Bill_Ar_Dtl);
        }

        // GET: Bms_Bill_Ar_Dtls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bms_Bill_Ar_Dtl bms_Bill_Ar_Dtl = _bms_Bill_Ar_DtlService.Find(id);
            if (bms_Bill_Ar_Dtl == null)
            {
                return HttpNotFound();
            }
            return View(bms_Bill_Ar_Dtl);
        }

        // POST: Bms_Bill_Ar_Dtls/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bms_Bill_Ar_Dtl bms_Bill_Ar_Dtl = _bms_Bill_Ar_DtlService.Find(id);
            _bms_Bill_Ar_DtlService.Delete(bms_Bill_Ar_Dtl);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Bms_Bill_Ar_Dtl record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "bms_bill_ar_dtl_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _bms_Bill_Ar_DtlService.ExportExcel(filterRules, sort, order);
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
