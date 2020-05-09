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
    public class QuotedPricesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<QuotedPrice>, Repository<QuotedPrice>>();
        //container.RegisterType<IQuotedPriceService, QuotedPriceService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IQuotedPriceService _quotedPriceService;
        private readonly ICusQuotedPriceDtlService _cusQuotedPriceDtlService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/QuotedPrices";

        public QuotedPricesController(IQuotedPriceService quotedPriceService, 
            ICusQuotedPriceDtlService cusQuotedPriceDtlService,
            IUnitOfWorkAsync unitOfWork) :
            base(false, true)
        {
            _cusQuotedPriceDtlService = cusQuotedPriceDtlService;
            _quotedPriceService = quotedPriceService;
            _unitOfWork = unitOfWork;
            ControllerQXName = "/" + this.GetType().Name.Replace("Controller", "");
        }

        // GET: QuotedPrices/Index
        public ActionResult Index()
        {
            //计费公式 缓存
            ViewData["BillFormulaXML"] = AirOut.Web.CacheHelper.Get_SetCache(AirOut.Web.Extensions.Common.CacheNameS.BillFormulaXML);
            return View();
        }

        // Get :QuotedPrices/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var quotedprice = _quotedPriceService.Query(new QuotedPriceQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = quotedprice.Select(n => new
            {
                Id = n.Id,
                SerialNo = n.SerialNo,
                SettleAccount = n.SettleAccount,
                FeeCode = n.FeeCode,
                FeeName = n.FeeName,
                StartPlace = n.StartPlace,
                TransitPlace = n.TransitPlace,
                EndPlace = n.EndPlace,
                AirLineCo = n.AirLineCo,
                AirLineNo = n.AirLineNo,
                WHBuyer = n.WHBuyer,
                ProxyOperator = n.ProxyOperator,
                DealWithArticle = n.DealWithArticle,
                BSA = n.BSA,
                CustomsType = n.CustomsType,
                InspectMark = n.InspectMark,
                GetOrdMark = n.GetOrdMark,
                MoorLevel = n.MoorLevel,
                BillingUnit = n.BillingUnit,
                Price = n.Price,
                CurrencyCode = n.CurrencyCode,
                FeeConditionVal1 = n.FeeConditionVal1,
                CalcSign1 = n.CalcSign1,
                FeeCondition = n.FeeCondition,
                CalcSign2 = n.CalcSign2,
                FeeConditionVal2 = n.FeeConditionVal2,
                CalcFormula = n.CalcFormula,
                FeeMin = n.FeeMin,
                FeeMax = n.FeeMax,
                AuditStatus = n.AuditStatus,
                StartDate = n.StartDate,
                EndDate = n.EndDate,
                Description = n.Description,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();

            #region 取缓存

            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();//帐户
            var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            //var ArrFeeType = (List<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
            //var ArrDealArticle = (List<DealArticle>)CacheHelper.Get_SetCache(Common.CacheNameS.DealArticle);
            var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var ArrPARA_AirLine = (List<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);
            var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            var ArrFeeUnit = (List<FeeUnit>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeUnit);
            var ArrBillFormula = (IEnumerable<dynamic>)AirOut.Web.CacheHelper.Get_SetCache(AirOut.Web.Extensions.Common.CacheNameS.BillFormulaXML);

            //var ArrMoorLevel = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "MoorLevel");
            //var ArrCustomsType = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "CusType");
            var ArrFeeCondition = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "FeeConditionAr");
            //var ArrDeliveryPoint = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "DeliveryPoint");
            var Retdata = from p in datarows
                          join a in ArrBillFormula on p.CalcFormula equals a.ID into a_tmp
                          from atmp in a_tmp.DefaultIfEmpty()
                          //join b in ArrCusBusInfo on p.SettleAccount equals b.EnterpriseId into b_tmp
                          //from btmp in b_tmp.DefaultIfEmpty()
                          join c in ArrCusBusInfo on p.WHBuyer equals c.EnterpriseId into c_tmp
                          from ctmp in c_tmp.DefaultIfEmpty()
                          //join d in ArrDealArticle on p.DealWithArticle equals d.DealArticleCode into d_tmp
                          //from dtmp in d_tmp.DefaultIfEmpty()
                          //join e in ArrCusBusInfo on p.StartPlace equals e.EnterpriseId into e_tmp
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
                          join m in ArrAppUser on p.ADDWHO equals m.UserName into m_tmp
                          from mtmp in m_tmp.DefaultIfEmpty()
                          join n in ArrAppUser on p.EDITWHO equals n.UserName into n_tmp
                          from ntmp in n_tmp.DefaultIfEmpty()
                          select new
                          {
                              p.Id,
                              p.SerialNo,
                              p.SettleAccount,
                              p.FeeCode,
                              p.FeeName,
                              p.StartPlace,
                              //StartPlaceNAME = etmp == null ? string.Empty : etmp.EnterpriseShortName,
                              p.TransitPlace,
                              p.EndPlace,
                              //EndPlaceNAME = ftmp == null ? string.Empty : ftmp.PortName,
                              p.AirLineCo,
                              AirLineCoNAME = gtmp == null ? string.Empty : gtmp.EnterpriseShortName,
                              p.AirLineNo,
                              p.WHBuyer,
                              WHBuyerNAME = ctmp == null ? string.Empty : ctmp.EnterpriseShortName,
                              //p.ProxyOperator,
                              //p.DealWithArticle,
                              //p.BSA,
                              p.CustomsType,
                              //p.InspectMark,
                              //p.GetOrdMark,
                              //p.MoorLevel,
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
                              CalcFormulaNAME = atmp == null ? string.Empty : atmp.TEXT,
                              p.FeeMin,
                              p.FeeMax,
                              p.AuditStatus,
                              p.StartDate,
                              p.EndDate,
                              p.Description,
                              p.Status,
                              p.OperatingPoint,
                              p.ADDWHO,
                              ADDWHONAME = mtmp == null ? string.Empty : mtmp.UserNameDesc,
                              p.ADDTS,
                              p.EDITWHO,
                              EDITWHONAME = ntmp == null ? string.Empty : ntmp.UserNameDesc,
                              p.EDITTS
                          };

            #endregion

            var pagelist = new { total = totalCount, rows = Retdata };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveBatchDelete(Array ids)
        {
            var strids = "";
            var delete = "0";
            foreach (var item in ids)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            foreach (var id in idarr)
            {
                QuotedPrice quotedPrice = _quotedPriceService.Find(Int32.Parse(id));
                if (quotedPrice.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable)
                {//状态为是，则跳过
                    continue;
                }
                delete = "1";
                _quotedPriceService.Delete(quotedPrice);
            }
            try
            {
                _unitOfWork.SaveChanges();
                if (delete == "1")
                {
                    foreach (var id in idarr)
                    {
                        QuotedPrice quotedPrice = _quotedPriceService.Find(Int32.Parse(id));
                        //自动更新 缓存
                        if (IsAutoResetCache)
                            AutoResetCache(quotedPrice);
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="quotedprice"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData(QuotedPriceChangeViewModel quotedprice)
        {
            if (quotedprice.updated != null)
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

                foreach (var updated in quotedprice.updated)
                {
                    _quotedPriceService.Update(updated);
                }
            }
            if (quotedprice.deleted != null)
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

                foreach (var deleted in quotedprice.deleted)
                {
                    _quotedPriceService.Delete(deleted);
                }
            }
            if (quotedprice.inserted != null)
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

                foreach (var inserted in quotedprice.inserted)
                {
                    _quotedPriceService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((quotedprice.updated != null && quotedprice.updated.Any()) ||
                (quotedprice.deleted != null && quotedprice.deleted.Any()) ||
                (quotedprice.inserted != null && quotedprice.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(quotedprice);
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

        // GET: QuotedPrices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuotedPrice quotedPrice = _quotedPriceService.Find(id);
            if (quotedPrice == null)
            {
                return HttpNotFound();
            }
            return View(quotedPrice);
        }

        // GET: QuotedPrices/Create
        public ActionResult Create(int? id = 0)
        {
            QuotedPrice quotedPrice = new QuotedPrice();
            quotedPrice.ADDTS = DateTime.Now;
            if (id == 0)
            {
                quotedPrice.SerialNo = SequenceBuilder.NextQuotedPriceSerial_No();
                //quotedPrice.StartDate = DateTime.Parse(DateTime.Now.ToString("d"));
                //quotedPrice.EndDate = DateTime.Parse(DateTime.Now.ToString("d")).AddYears(2);
                quotedPrice.CurrencyCode = "CNY";
                quotedPrice.AuditStatus = AirOutEnumType.AuditStatusEnum.AuditSuccess;
                quotedPrice.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
            }
            else
            {
                QuotedPrice quotedPriceEdit = _quotedPriceService.Find(id);
                if (quotedPriceEdit == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    quotedPrice = Common.SetProperties<QuotedPrice, QuotedPrice>(quotedPriceEdit, "Id,SerialNo,ADDWHO,ADDTS,EDITWHO,EDITTS");
                    //quotedPrice.SerialNo = SequenceBuilder.NextQuotedPriceSerial_No();
                    quotedPrice.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                    quotedPrice.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                    if (quotedPriceEdit.ADDTS != null)
                        quotedPrice.ADDTS = quotedPriceEdit.ADDTS;
                }
            }
            //计费公式 缓存
            ViewData["BillFormulaXML"] = AirOut.Web.CacheHelper.Get_SetCache(AirOut.Web.Extensions.Common.CacheNameS.BillFormulaXML);
            var ODynamic = GetFromNAME(quotedPrice);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            //set default value
            return View(quotedPrice);
        }

        // POST: QuotedPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(QuotedPrice quotedPrice)
        {
            quotedPrice.BSA = Common.ChangStrToBool(Request["BSA"] ?? "");
            quotedPrice.ProxyOperator = Common.ChangStrToBool(Request["ProxyOperator"] ?? "");
            quotedPrice.InspectMark = Common.ChangStrToBool(Request["InspectMark"] ?? "");
            quotedPrice.GetOrdMark = Common.ChangStrToBool(Request["GetOrdMark"] ?? "");
            quotedPrice.SettleAccount = "-";
            ModelState.Remove("BSA");
            ModelState.Remove("ProxyOperator");
            ModelState.Remove("InspectMark");
            ModelState.Remove("GetOrdMark");
            ModelState.Remove("SettleAccount");
            var modelStateErrors = "";
            if (ModelState.IsValid)
            {
                _quotedPriceService.Insert(quotedPrice);
                _unitOfWork.SaveChanges();
                var quotedpriceVM = new QuotedPriceChangeViewModel();
                quotedpriceVM.inserted = new List<QuotedPrice> { quotedPrice };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(quotedpriceVM);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a QuotedPrice record");
                return RedirectToAction("Index");
            }
            else
                modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));

            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            var ODynamic = GetFromNAME(quotedPrice);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return View(quotedPrice);
        }

        // GET: QuotedPrices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //计费公式 缓存
            ViewData["BillFormulaXML"] = AirOut.Web.CacheHelper.Get_SetCache(AirOut.Web.Extensions.Common.CacheNameS.BillFormulaXML);
            QuotedPrice quotedPrice = _quotedPriceService.Find(id);
            if (quotedPrice.ADDTS == null)
                quotedPrice.ADDTS = DateTime.Now;
            if (quotedPrice == null)
            {
                return HttpNotFound();
            }
            var ODynamic = GetFromNAME(quotedPrice);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return View(quotedPrice);
        }

        // POST: QuotedPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(QuotedPrice quotedPrice)
        {
            quotedPrice.BSA = Common.ChangStrToBool(Request["BSA"] ?? "");
            quotedPrice.ProxyOperator = Common.ChangStrToBool(Request["ProxyOperator"] ?? "");
            quotedPrice.InspectMark = Common.ChangStrToBool(Request["InspectMark"] ?? "");
            quotedPrice.GetOrdMark = Common.ChangStrToBool(Request["GetOrdMark"] ?? "");
            quotedPrice.SettleAccount = "-";
            ModelState.Remove("BSA");
            ModelState.Remove("ProxyOperator");
            ModelState.Remove("InspectMark");
            ModelState.Remove("GetOrdMark");
            ModelState.Remove("SettleAccount");
            if (ModelState.IsValid)
            {
                quotedPrice.ObjectState = ObjectState.Modified;
                _quotedPriceService.Update(quotedPrice);
                _unitOfWork.SaveChanges();
                var quotedpriceVM = new QuotedPriceChangeViewModel();
                quotedpriceVM.updated = new List<QuotedPrice> { quotedPrice };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(quotedpriceVM);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a QuotedPrice record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            var ODynamic = GetFromNAME(quotedPrice);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return View(quotedPrice);
        }

        // GET: QuotedPrices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuotedPrice quotedPrice = _quotedPriceService.Find(id);
            if (quotedPrice == null)
            {
                return HttpNotFound();
            }
            return View(quotedPrice);
        }

        // POST: QuotedPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuotedPrice quotedPrice = _quotedPriceService.Find(id);
            _quotedPriceService.Delete(quotedPrice);
            _unitOfWork.SaveChanges();
            var quotedpriceVM = new QuotedPriceChangeViewModel();
            quotedpriceVM.deleted = new List<QuotedPrice> { quotedPrice };
            //自动更新 缓存
            if (IsAutoResetCache)
                AutoResetCache(quotedpriceVM);
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a QuotedPrice record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "报价信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _quotedPriceService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(QuotedPrice OQuotedPrice)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();

            #region 取缓存

            var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var ArrFeeType = (List<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
            var ArrDealArticle = (List<DealArticle>)CacheHelper.Get_SetCache(Common.CacheNameS.DealArticle);
            var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var ArrPARA_AirLine = (List<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);
            var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            var ArrFeeUnit = (List<FeeUnit>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeUnit);
            
            #endregion

            //费用代码
            if (!string.IsNullOrEmpty(OQuotedPrice.FeeCode))
            {
                var OArrFeeType = ArrFeeType.Where(x => x.FeeCode == OQuotedPrice.FeeCode).FirstOrDefault();
                ODynamic.FeeTypeNAME = OArrFeeType == null ? "" : OArrFeeType.FeeName;
            }
            //结算方，获取的客商信息
            if (!string.IsNullOrEmpty(OQuotedPrice.SettleAccount) && OQuotedPrice.SettleAccount != "-")
            {
                var OArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OQuotedPrice.SettleAccount).FirstOrDefault();
                ODynamic.SettleAccountNAME = OArrCusBusInfo == null ? "" : OArrCusBusInfo.EnterpriseShortName;
            }
            //订舱方
            if (!string.IsNullOrEmpty(OQuotedPrice.WHBuyer) && OQuotedPrice.WHBuyer != "-")
            {
                var OArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OQuotedPrice.WHBuyer).FirstOrDefault();
                ODynamic.WHBuyerNAME = OArrCusBusInfo == null ? "" : OArrCusBusInfo.EnterpriseShortName;
            }
            //成交条款
            if (!string.IsNullOrEmpty(OQuotedPrice.DealWithArticle) && OQuotedPrice.DealWithArticle != "-")
            {
                var OArrDealArticle = ArrDealArticle.Where(x => x.DealArticleCode == OQuotedPrice.DealWithArticle).FirstOrDefault();
                ODynamic.DealWithArticleNAME = OArrDealArticle == null ? "" : OArrDealArticle.DealArticleName;
            }
            //起始地
            if (!string.IsNullOrEmpty(OQuotedPrice.StartPlace) && OQuotedPrice.StartPlace != "-")
            {
                var OArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OQuotedPrice.StartPlace).FirstOrDefault();
                ODynamic.StartPlaceNAME = OArrPARA_AirPort == null ? "" : OArrPARA_AirPort.PortName;
            }
            ////中转地
            //if (!string.IsNullOrEmpty(OQuotedPrice.TransitPlace) && OQuotedPrice.TransitPlace != "-")
            //{
            //    var OArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OQuotedPrice.TransitPlace).FirstOrDefault();
            //    ODynamic.TransitPlaceNAME = OArrPARA_AirPort == null ? "" : OArrPARA_AirPort.PortName;
            //}
            //目的地
            if (!string.IsNullOrEmpty(OQuotedPrice.EndPlace) && OQuotedPrice.EndPlace != "-")
            {
                var OArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OQuotedPrice.EndPlace).FirstOrDefault();
                ODynamic.EndPlaceNAME = OArrPARA_AirPort == null ? "" : OArrPARA_AirPort.PortName;
            }
            //航空公司
            if (!string.IsNullOrEmpty(OQuotedPrice.AirLineCo) && OQuotedPrice.AirLineCo != "-")
            {
                var OArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OQuotedPrice.AirLineCo).FirstOrDefault();
                ODynamic.AirLineCoNAME = OArrCusBusInfo == null ? "" : OArrCusBusInfo.EnterpriseShortName;
            }
            //航班号
            if (!string.IsNullOrEmpty(OQuotedPrice.AirLineNo) && OQuotedPrice.AirLineNo != "-")
            {
                var OArrPARA_AirLine = ArrPARA_AirLine.Where(x => x.AirCode == OQuotedPrice.AirLineNo).FirstOrDefault();
                ODynamic.AirLineNoNAME = OArrPARA_AirLine == null ? "" : OArrPARA_AirLine.AirLine;
            }
            //靠级
            if (!string.IsNullOrEmpty(OQuotedPrice.MoorLevel) && OQuotedPrice.MoorLevel != "-")
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "MoorLevel" && x.LISTCODE == OQuotedPrice.MoorLevel).FirstOrDefault();
                ODynamic.MoorLevelNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //代操作ProxyOperator
            //BSA
            //审批状态AuditStatus
            //使用状态Status
            //报关方式
            if (!string.IsNullOrEmpty(OQuotedPrice.CustomsType) && OQuotedPrice.CustomsType != "-")
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "CusType" && x.LISTCODE == OQuotedPrice.CustomsType).FirstOrDefault();
                ODynamic.CustomsTypeNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //币制
            if (!string.IsNullOrEmpty(OQuotedPrice.CurrencyCode) && OQuotedPrice.CurrencyCode != "-")
            {
                var OArrPARA_CURR = ArrPARA_CURR.Where(x => x.CURR_CODE == OQuotedPrice.CurrencyCode).FirstOrDefault();
                ODynamic.CurrencyCodeNAME = OArrPARA_CURR == null ? "" : OArrPARA_CURR.CURR_Name;
            }
            //计费单位
            if (!string.IsNullOrEmpty(OQuotedPrice.BillingUnit) && OQuotedPrice.BillingUnit != "-")
            {
                var BillingUnit = 0;
                if (int.TryParse(OQuotedPrice.BillingUnit, out BillingUnit))
                {
                    var OArrFeeUnit = ArrFeeUnit.Where(x => x.Id == BillingUnit).FirstOrDefault();
                    ODynamic.BillingUnitNAME = OArrFeeUnit == null ? "" : OArrFeeUnit.FeeUnitName;
                }
            }
            //计算单位
            if (!string.IsNullOrEmpty(OQuotedPrice.FeeCondition) && OQuotedPrice.FeeCondition != "-")
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "FeeConditionAr" && x.LISTCODE == OQuotedPrice.FeeCondition).FirstOrDefault();
                ODynamic.FeeConditionNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //商检标志
            //取单标志
            //计费运算符1
            //计费运算符2
            //计费公式
            return ODynamic;
        }

        /// <summary>
        /// 获取弹出选择框
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQPSeltdView()
        {
            string NumStr = Request["Num"] ?? "";
            int Num = 1;
            int.TryParse(NumStr, out Num);
            ViewData["Num"] = Num;
            return PartialView("QPSeltdView");
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="QPId"></param>
        /// <returns></returns>
        public ActionResult GetQPEditFormView(int QPId)
        {
            var QuotedPrice = _quotedPriceService.Queryable().Where(x => x.Id == QPId).FirstOrDefault();
            var ODynamic = GetFromNAME(QuotedPrice);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return PartialView("EditFormView", QuotedPrice);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="CusQPDtlId"></param>
        /// <returns></returns>
        public ActionResult GetCusQPEditFormView(int CusQPDtlId)
        {
            var OQuotedPrice = new QuotedPrice();
            var OCusQPDtl = _cusQuotedPriceDtlService.Queryable().AsNoTracking().Where(x => x.Id == CusQPDtlId).FirstOrDefault();
            if (OCusQPDtl == null || OCusQPDtl.Id <= 0)
                OQuotedPrice = null;
            else
            {
                //var OCusQP = _customerQuotedPriceService.Queryable().AsNoTracking().Where(x => x.Id == OCusQPDtl.CusQPId).FirstOrDefault();
                //OQuotedPrice.SettleAccount = OCusQP.CustomerCode;
                //OQuotedPrice.AuditStatus = OCusQP.AuditStatus;
                //OQuotedPrice.Description = OCusQP.Description;
                ////OQuotedPrice.Status = OCusQP.Status;

                //OQuotedPrice.Id = OCusQPDtl.QPId;
                OQuotedPrice.SerialNo = OCusQPDtl.QPSerialNo;
                OQuotedPrice.FeeCode = OCusQPDtl.FeeCode;
                OQuotedPrice.FeeName = OCusQPDtl.FeeName;
                OQuotedPrice.StartPlace = OCusQPDtl.StartPlace;
                OQuotedPrice.TransitPlace = OCusQPDtl.TransitPlace;
                OQuotedPrice.EndPlace = OCusQPDtl.EndPlace;
                OQuotedPrice.AirLineCo = OCusQPDtl.AirLineCo;
                OQuotedPrice.AirLineNo = OCusQPDtl.AirLineNo;
                OQuotedPrice.WHBuyer = OCusQPDtl.WHBuyer;
                OQuotedPrice.ProxyOperator = OCusQPDtl.ProxyOperator;
                OQuotedPrice.DealWithArticle = OCusQPDtl.DealWithArticle;
                OQuotedPrice.BSA = OCusQPDtl.BSA;
                OQuotedPrice.CustomsType = OCusQPDtl.CustomsType;
                OQuotedPrice.InspectMark = OCusQPDtl.InspectMark;
                OQuotedPrice.GetOrdMark = OCusQPDtl.GetOrdMark;
                OQuotedPrice.MoorLevel = OCusQPDtl.MoorLevel;
                OQuotedPrice.BillingUnit = OCusQPDtl.BillingUnit;
                OQuotedPrice.Price = OCusQPDtl.Price;
                OQuotedPrice.CurrencyCode = OCusQPDtl.CurrencyCode;
                OQuotedPrice.FeeConditionVal1 = OCusQPDtl.FeeConditionVal1;
                OQuotedPrice.CalcSign1 = OCusQPDtl.CalcSign1;
                OQuotedPrice.FeeCondition = OCusQPDtl.FeeCondition;
                OQuotedPrice.CalcSign2 = OCusQPDtl.CalcSign2;
                OQuotedPrice.FeeConditionVal2 = OCusQPDtl.FeeConditionVal2;
                OQuotedPrice.CalcFormula = OCusQPDtl.CalcFormula;
                OQuotedPrice.FeeMin = OCusQPDtl.FeeMin;
                OQuotedPrice.FeeMax = OCusQPDtl.FeeMax;
                OQuotedPrice.StartDate = OCusQPDtl.StartDate;
                OQuotedPrice.EndDate = OCusQPDtl.EndDate;
                OQuotedPrice.OperatingPoint = OCusQPDtl.OperatingPoint;
                OQuotedPrice.ADDWHO = OCusQPDtl.ADDWHO;
                OQuotedPrice.ADDTS = OCusQPDtl.ADDTS;
                OQuotedPrice.EDITWHO = OCusQPDtl.EDITWHO;
                OQuotedPrice.EDITTS = OCusQPDtl.EDITTS;
            }
            var ODynamic = GetFromNAME(OQuotedPrice);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return PartialView("EditFormView", OQuotedPrice);
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

                var ArrQuotedPrice = _quotedPriceService.Queryable().Where(x => ArrId.Contains(x.Id)).ToList();
                ArrQuotedPrice = ArrQuotedPrice.Where(x => x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).ToList();
                if (ArrQuotedPrice.Count < ArrId.Count)
                    return Json(new { Success = false, ErrMsg = "所选数据中，部分数据 已被作废或删除" }, JsonRequestBehavior.AllowGet);

                var IdNum = 0;
                var ChangViewModel = new QuotedPriceChangeViewModel();
                var AddChang = new List<QuotedPrice>();
                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                foreach (var item in ArrQuotedPrice)
                {
                    var Newitem = new QuotedPrice();
                    Newitem = Common.SetSamaProtity(Newitem, item);
                    Newitem.Id = IdNum--;
                    Newitem.SerialNo = SequenceBuilder.NextQuotedPriceSerial_No();
                    Newitem.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                    Newitem.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                    Newitem.ADDTS = null;
                    Newitem.ADDWHO = null;
                    Newitem.ADDID = null;
                    Newitem.EDITTS = null;
                    Newitem.EDITWHO = null;
                    var entry = WebdbContxt.Entry(Newitem);
                    entry.State = EntityState.Added;//必须设置为 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                    Newitem.ObjectState = ObjectState.Added;
                    AddChang.Add(Newitem);
                }

                if (AddChang.Any())
                {
                    WebdbContxt.SaveChanges();
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