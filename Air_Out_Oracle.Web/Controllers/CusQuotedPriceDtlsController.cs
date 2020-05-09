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
    public class CusQuotedPriceDtlsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<CusQuotedPriceDtl>, Repository<CusQuotedPriceDtl>>();
        //container.RegisterType<ICusQuotedPriceDtlService, CusQuotedPriceDtlService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ICustomerQuotedPriceService _customerQuotedPriceService;
        private readonly ICusQuotedPriceDtlService _cusQuotedPriceDtlService;
        private readonly IQuotedPriceService _QuotedPriceService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/CustomerQuotedPrices";

        public CusQuotedPriceDtlsController(ICustomerQuotedPriceService customerQuotedPriceService,
            ICusQuotedPriceDtlService cusQuotedPriceDtlService,
            IQuotedPriceService quotedPriceService,
            IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _customerQuotedPriceService = customerQuotedPriceService;
            _cusQuotedPriceDtlService = cusQuotedPriceDtlService;
            _QuotedPriceService = quotedPriceService;
            _unitOfWork = unitOfWork;
            //ControllerQXName = "/" + this.GetType().Name.Replace("Controller", "");
        }

        // GET: CusQuotedPriceDtls/Index
        public ActionResult Index()
        {
            //var cusquotedpricedtl  = _cusQuotedPriceDtlService.Queryable().AsQueryable();
            //return View(cusquotedpricedtl  );
            return View();
        }

        // Get :CusQuotedPriceDtls/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var cusquotedpricedtl = _cusQuotedPriceDtlService.Query(new CusQuotedPriceDtlQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = cusquotedpricedtl.Select(n => new
            {
                Id = n.Id,
                n.CusQPSerialNo,
                n.CusQPId,
                n.QPSerialNo,
                n.QPId,
                n.FeeCode,
                n.FeeName,
                n.StartPlace,
                n.TransitPlace,
                n.EndPlace,
                n.AirLineCo,
                n.AirLineNo,
                n.WHBuyer,
                n.ProxyOperator,
                n.DealWithArticle,
                n.BSA,
                n.CustomsType,
                n.InspectMark,
                n.GetOrdMark,
                n.MoorLevel,
                n.BillingUnit,
                n.Price,
                n.CurrencyCode,
                n.FeeConditionVal1,
                n.CalcSign1,
                n.FeeCondition,
                n.CalcSign2,
                n.FeeConditionVal2,
                n.CalcFormula,
                n.FeeMin,
                n.FeeMax,
                n.StartDate,
                n.EndDate,
                n.ADDID,
                n.ADDTS,
                n.ADDWHO,
                n.EDITID,
                n.EDITTS,
                n.EDITWHO,
                n.OperatingPoint
            }).ToList();

            #region 取缓存

            var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            //var ArrFeeType = (List<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
            //var ArrDealArticle = (List<DealArticle>)CacheHelper.Get_SetCache(Common.CacheNameS.DealArticle);
            //var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            //var ArrPARA_AirLine = (List<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);
            var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            var ArrFeeUnit = (List<FeeUnit>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeUnit);
            var ArrBillFormula = (IEnumerable<dynamic>)AirOut.Web.CacheHelper.Get_SetCache(AirOut.Web.Extensions.Common.CacheNameS.BillFormulaXML);

            //var ArrMoorLevel = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "MoorLevel");
            //var ArrCustomsType = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "DeclCustom");
            var ArrFeeCondition = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "FeeConditionAr");
            //FeeCode SettleAccount WHBuyer DealWithArticle StartPlace EndPlace AirLineCo MoorLevel  CustomsType CurrencyCode BillingUnit FeeCondition
            var Retdata = from p in datarows
                          //join a in ArrDeliveryPoint on p.DeliveryPoint equals a.LISTCODE into a_tmp
                          //from atmp in a_tmp.DefaultIfEmpty()
                          //join b in ArrCusBusInfo on p.SettleAccount equals b.EnterpriseId into b_tmp
                          //from btmp in b_tmp.DefaultIfEmpty()
                          join c in ArrCusBusInfo on p.WHBuyer equals c.EnterpriseId into c_tmp
                          from ctmp in c_tmp.DefaultIfEmpty()
                          //join d in ArrDealArticle on p.DealWithArticle equals d.DealArticleCode into d_tmp
                          //from dtmp in d_tmp.DefaultIfEmpty()
                          //join e in ArrPARA_AirPort on p.StartPlace equals e.PortCode into e_tmp
                          //from etmp in e_tmp.DefaultIfEmpty()
                          //join f in ArrPARA_AirPort on p.EndPlace equals f.PortCode into f_tmp
                          //from ftmp in f_tmp.DefaultIfEmpty()
                          join g in ArrCusBusInfo on p.AirLineCo equals g.EnterpriseId into g_tmp
                          from gtmp in g_tmp.DefaultIfEmpty()
                          //join h in ArrCustomsType on p.CustomsType equals h.LISTCODE into h_tmp
                          //from htmp in h_tmp.DefaultIfEmpty()
                          join i in ArrPARA_CURR on p.CurrencyCode equals i.CURR_CODE into i_tmp
                          from itmp in i_tmp.DefaultIfEmpty()
                          join j in ArrFeeUnit on p.BillingUnit equals j.Id.ToString() into j_tmp
                          from jtmp in j_tmp.DefaultIfEmpty()
                          join k in ArrFeeCondition on p.FeeCondition equals k.LISTCODE into k_tmp
                          from ktmp in k_tmp.DefaultIfEmpty()
                          join l in ArrBillFormula on p.CalcFormula equals l.ID into l_tmp
                          from ltmp in l_tmp.DefaultIfEmpty()
                          select new
                          {
                              p.Id,
                              p.CusQPSerialNo,
                              p.CusQPId,
                              p.QPSerialNo,
                              p.QPId,
                              p.FeeCode,
                              p.FeeName,
                              p.StartPlace,
                              p.TransitPlace,
                              p.EndPlace,
                              //EndPlaceNAME = ftmp == null ? string.Empty : ftmp.PortName,
                              p.AirLineCo,
                              AirLineCoNAME = gtmp == null ? string.Empty : gtmp.EnterpriseShortName,
                              p.AirLineNo,
                              p.WHBuyer,
                              WHBuyerNAME = ctmp == null ? string.Empty : ctmp.EnterpriseShortName,
                              p.ProxyOperator,
                              p.DealWithArticle,
                              p.BSA,
                              p.CustomsType,
                              p.InspectMark,
                              p.GetOrdMark,
                              p.MoorLevel,
                              p.BillingUnit,
                              BillingUnitNAME = jtmp == null ? string.Empty : jtmp.FeeUnitName,
                              p.Price,
                              p.CurrencyCode,
                              CurrencyCodeNAME = itmp == null ? string.Empty : itmp.CURR_Name,
                              p.FeeConditionVal1,
                              p.CalcSign1,
                              p.FeeCondition,
                              FeeConditionNAME = ktmp == null ? string.Empty : ktmp.LISTNAME,
                              p.CalcSign2,
                              p.FeeConditionVal2,
                              CalcFuc = string.IsNullOrEmpty(p.FeeCondition) ? "" : (string.IsNullOrEmpty(p.CalcSign1) ? "" : (p.FeeConditionVal1.ToString() + p.CalcSign1)) + (ktmp == null ? p.FeeCondition : ktmp.LISTNAME) + (string.IsNullOrEmpty(p.CalcSign2) ? "" : (p.CalcSign2 + p.FeeConditionVal2.ToString())),
                              p.CalcFormula,
                              CalcFormulaNAME = ltmp == null ? string.Empty : ltmp.TEXT,
                              p.FeeMin,
                              p.FeeMax,
                              p.StartDate,
                              p.EndDate,
                              p.ADDID,
                              p.ADDWHO,
                              p.ADDTS,
                              p.EDITID,
                              p.EDITWHO,
                              p.EDITTS,
                              p.OperatingPoint,
                          };

            #endregion

            var pagelist = new { total = totalCount, rows = Retdata };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="cusquotedpricedtl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData(CusQuotedPriceDtlChangeViewModel cusquotedpricedtl)
        {
            if (cusquotedpricedtl.updated != null)
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

                foreach (var updated in cusquotedpricedtl.updated)
                {
                    _cusQuotedPriceDtlService.Update(updated);
                }
            }
            if (cusquotedpricedtl.deleted != null)
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

                foreach (var deleted in cusquotedpricedtl.deleted)
                {
                    _cusQuotedPriceDtlService.Delete(deleted);
                }
            }
            if (cusquotedpricedtl.inserted != null)
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

                foreach (var inserted in cusquotedpricedtl.inserted)
                {
                    _cusQuotedPriceDtlService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((cusquotedpricedtl.updated != null && cusquotedpricedtl.updated.Any()) ||
                (cusquotedpricedtl.deleted != null && cusquotedpricedtl.deleted.Any()) ||
                (cusquotedpricedtl.inserted != null && cusquotedpricedtl.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(cusquotedpricedtl);
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

        // GET: CusQuotedPriceDtls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CusQuotedPriceDtl cusQuotedPriceDtl = _cusQuotedPriceDtlService.Find(id);
            if (cusQuotedPriceDtl == null)
            {
                return HttpNotFound();
            }
            return View(cusQuotedPriceDtl);
        }

        // GET: CusQuotedPriceDtls/Create
        public ActionResult Create()
        {
            CusQuotedPriceDtl cusQuotedPriceDtl = new CusQuotedPriceDtl();
            //set default value
            return View(cusQuotedPriceDtl);
        }

        // POST: CusQuotedPriceDtls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(CusQuotedPriceDtl cusQuotedPriceDtl)
        {
            if (ModelState.IsValid)
            {
                _cusQuotedPriceDtlService.Insert(cusQuotedPriceDtl);
                _unitOfWork.SaveChanges();
                var ChangeViewModel = new CusQuotedPriceDtlChangeViewModel();
                ChangeViewModel.inserted = new List<CusQuotedPriceDtl> { cusQuotedPriceDtl };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(ChangeViewModel);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a CusQuotedPriceDtl record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(cusQuotedPriceDtl);
        }

        // GET: CusQuotedPriceDtls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CusQuotedPriceDtl cusQuotedPriceDtl = _cusQuotedPriceDtlService.Find(id);
            if (cusQuotedPriceDtl == null)
            {
                return HttpNotFound();
            }
            return View(cusQuotedPriceDtl);
        }

        // POST: CusQuotedPriceDtls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(CusQuotedPriceDtl cusQuotedPriceDtl)
        {
            if (ModelState.IsValid)
            {
                cusQuotedPriceDtl.ObjectState = ObjectState.Modified;
                _cusQuotedPriceDtlService.Update(cusQuotedPriceDtl);
                _unitOfWork.SaveChanges();
                var ChangeViewModel = new CusQuotedPriceDtlChangeViewModel();
                ChangeViewModel.updated = new List<CusQuotedPriceDtl> { cusQuotedPriceDtl };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(ChangeViewModel);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a CusQuotedPriceDtl record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(cusQuotedPriceDtl);
        }

        // GET: CusQuotedPriceDtls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CusQuotedPriceDtl cusQuotedPriceDtl = _cusQuotedPriceDtlService.Find(id);
            if (cusQuotedPriceDtl == null)
            {
                return HttpNotFound();
            }
            return View(cusQuotedPriceDtl);
        }

        // POST: CusQuotedPriceDtls/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CusQuotedPriceDtl cusQuotedPriceDtl = _cusQuotedPriceDtlService.Find(id);
            _cusQuotedPriceDtlService.Delete(cusQuotedPriceDtl);
            _unitOfWork.SaveChanges();
            var ChangeViewModel = new CusQuotedPriceDtlChangeViewModel();
            ChangeViewModel.deleted = new List<CusQuotedPriceDtl> { cusQuotedPriceDtl };
            //自动更新 缓存
            if (IsAutoResetCache)
                AutoResetCache(ChangeViewModel);
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a CusQuotedPriceDtl record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "cusquotedpricedtl_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _cusQuotedPriceDtlService.ExportExcel(filterRules, sort, order);
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
