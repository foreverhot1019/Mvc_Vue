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
    public class CostMoneysController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<CostMoney>, Repository<CostMoney>>();
        //container.RegisterType<ICostMoneyService, CostMoneyService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ICostMoneyService _costMoneyService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/CostMoneys";

        public CostMoneysController(ICostMoneyService costMoneyService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _costMoneyService = costMoneyService;
            _unitOfWork = unitOfWork;
        }

        // GET: CostMoneys/Index
        public ActionResult Index()
        {
            //var costmoney  = _costMoneyService.Queryable().AsQueryable();
            //return View(costmoney  );
            return View();
        }

        // GET: CostMoneys/Index
        public ActionResult Audit()
        {
            //var costmoney  = _costMoneyService.Queryable().AsQueryable();
            //return View(costmoney  );
            return View();
        }

        // Get :CostMoneys/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            IEnumerable<CostMoney> costmoney;
            //int pagenum = offset / limit +1;
            if (sort == "CalcFuc" && order == "asc")
            {
                costmoney = _costMoneyService.Query(new CostMoneyQuery().Withfilter(filters)).OrderBy(n => n.OrderBy("FeeConditionVal1", order).ThenBy(x => x.FeeConditionVal2)).SelectPage(page, rows, out totalCount);
            }
            else if (sort == "CalcFuc" && order == "desc")
            {
                costmoney = _costMoneyService.Query(new CostMoneyQuery().Withfilter(filters)).OrderBy(n => n.OrderBy("FeeConditionVal1", order).ThenByDescending(x => x.FeeConditionVal2)).SelectPage(page, rows, out totalCount);
            }
            else {
                costmoney = _costMoneyService.Query(new CostMoneyQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            };
            var datarows = costmoney.Select(n => new
            {
                Id = n.Id,
                SerialNo = n.SerialNo,
                n.Is_Book_Flat,//KSF
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
                n.DeliveryPoint,
                Description = n.Description,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();

            #region 取缓存

            var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            //var ArrFeeType = (List<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
            var ArrDealArticle = (List<DealArticle>)CacheHelper.Get_SetCache(Common.CacheNameS.DealArticle);
            var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var ArrPARA_AirLine = (List<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);
            var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            var ArrFeeUnit = (List<FeeUnit>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeUnit);
            var ArrBillFormula = (IEnumerable<dynamic>)AirOut.Web.CacheHelper.Get_SetCache(AirOut.Web.Extensions.Common.CacheNameS.BillFormulaXML);

            var ArrMoorLevel = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "MoorLevel");
            var ArrCustomsType = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "CusType");
            var ArrFeeCondition = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "FeeConditionAp");
            var ArrDeliveryPoint = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "Delivery_Point");
            //FeeCode SettleAccount WHBuyer DealWithArticle StartPlace EndPlace AirLineCo MoorLevel  CustomsType CurrencyCode BillingUnit FeeCondition
            var Retdata = from p in datarows
                          join a in ArrDeliveryPoint on p.DeliveryPoint equals a.LISTCODE into a_tmp
                          from atmp in a_tmp.DefaultIfEmpty()
                          join b in ArrCusBusInfo on p.SettleAccount equals b.EnterpriseId into b_tmp
                          from btmp in b_tmp.DefaultIfEmpty()
                          join c in ArrCusBusInfo on p.WHBuyer equals c.EnterpriseId into c_tmp
                          from ctmp in c_tmp.DefaultIfEmpty()
                          join d in ArrDealArticle on p.DealWithArticle equals d.DealArticleCode into d_tmp
                          from dtmp in d_tmp.DefaultIfEmpty()
                          //join e in ArrPARA_AirPort on p.StartPlace equals e.PortCode into e_tmp
                          //from etmp in e_tmp.DefaultIfEmpty()
                          //join f in ArrPARA_AirPort on p.EndPlace equals f.PortCode into f_tmp
                          //from ftmp in f_tmp.DefaultIfEmpty()
                          join g in ArrCusBusInfo on p.AirLineCo equals g.EnterpriseId into g_tmp
                          from gtmp in g_tmp.DefaultIfEmpty()
                          join h in ArrCustomsType on p.CustomsType equals h.LISTCODE into h_tmp
                          from htmp in h_tmp.DefaultIfEmpty()
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
                              p.SerialNo,
                              p.Is_Book_Flat,//KSF
                              p.SettleAccount,
                              SettleAccountNAME = btmp == null ? string.Empty : btmp.EnterpriseShortName,
                              p.FeeCode,
                              p.FeeName,
                              p.StartPlace,
                              //StartPlaceNAME = etmp == null ? string.Empty : etmp.PortName,
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
                              DealWithArticleNAME = dtmp == null ? string.Empty : dtmp.DealArticleName,
                              p.BSA,
                              p.CustomsType,
                              CustomsTypeNAME = htmp == null ? string.Empty : htmp.LISTNAME,
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
                              p.AuditStatus,
                              p.StartDate,
                              p.EndDate,
                              p.DeliveryPoint,
                              DeliveryPointNAME = atmp == null ? string.Empty : atmp.LISTNAME,
                              p.Description,
                              p.Status,
                              p.OperatingPoint,
                              p.ADDWHO,
                              p.ADDTS,
                              p.EDITWHO,
                              p.EDITTS
                          };

            #endregion

            var pagelist = new { total = totalCount, rows = Retdata };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="ArrId">报价Id</param>
        /// <param name="EndDate">报价结束日期</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchEdit(List<int> ArrId, DateTime? EndDate)
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
                if (EndDate == null)
                    return Json(new { Success = false, ErrMsg = "报价结束日期,不能为空" }, JsonRequestBehavior.AllowGet);
                //else if (EndDate.Value < DateTime.Today.AddDays(-1))
                //{
                //    return Json(new { Success = false, ErrMsg = "报价结束日期,不能<昨天" }, JsonRequestBehavior.AllowGet);
                //}

                #endregion

                var ArrCostMoney = _costMoneyService.Queryable().Where(x => ArrId.Contains(x.Id)).ToList();
                ArrCostMoney = ArrCostMoney.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable).ToList();
                if (ArrCostMoney.Count < ArrId.Count)
                    return Json(new { Success = false, ErrMsg = "所选数据中，部分数据 已被作废或删除" }, JsonRequestBehavior.AllowGet);

                var ChangViewModel = new CostMoneyChangeViewModel();
                var UpChang = new List<CostMoney>();
                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                foreach (var item in ArrCostMoney)
                {
                    var entry = WebdbContxt.Entry(item);
                    entry.State = EntityState.Unchanged;//必须设置为 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                    if (item.EndDate != EndDate)
                    {
                        item.EndDate = EndDate;
                        entry.Property(x => x.EndDate).IsModified = true;
                        item.ObjectState = ObjectState.Modified;//有变更 必须是Modified，才会触发，不然 UnitOfWork-ef扩展 会将State 重置为ObjectState相同的值
                        UpChang.Add(item);
                    }
                }

                if (UpChang.Any())
                {
                    _unitOfWork.SaveChanges();
                    ChangViewModel.updated = UpChang;
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

                var ArrCostMoney = _costMoneyService.Queryable().Where(x => ArrId.Contains(x.Id)).ToList();
                ArrCostMoney = ArrCostMoney.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable).ToList();
                if (ArrCostMoney.Count < ArrId.Count)
                    return Json(new { Success = false, ErrMsg = "所选数据中，部分数据 已被作废或删除" }, JsonRequestBehavior.AllowGet);

                var ChangViewModel = new CostMoneyChangeViewModel();
                var AddChang = new List<CostMoney>();
                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                var IdNum = 0;
                foreach (var item in ArrCostMoney)
                {
                    var Newitem = new CostMoney();
                    Newitem = Common.SetSamaProtity(Newitem, item);
                    Newitem.Id = --IdNum;
                    Newitem.SerialNo = SequenceBuilder.NextCostMoney_No();
                    Newitem.Status = AirOutEnumType.UseStatusEnum.Enable;
                    Newitem.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
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

        /// <summary>
        /// 批量送审
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="auditstatus"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveBatchAuditStatus(Array ids, string auditstatus = "Auditing")
        {
            var strids = "";
            foreach (var item in ids)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",").Select(x => Int32.Parse(x)).Where(x => x > 0);
            var ArrCostMoney = _costMoneyService.Queryable().Where(x => idarr.Contains(x.Id));

            var ChangViewModel = new CostMoneyChangeViewModel();
            var UpChang = new List<CostMoney>();
            //反向从依赖注入中 取 DbContext
            WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
            foreach (var costMoney in ArrCostMoney)
            {
                if (costMoney == null || costMoney.Id <= 0)
                    continue;
                switch (auditstatus)
                {
                    case "Auditing":
                        if (costMoney.AuditStatus == AirOutEnumType.AuditStatusEnum.draft)
                        {
                            var entry = WebdbContxt.Entry(costMoney);
                            entry.State = EntityState.Unchanged;//必须设置为 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                            costMoney.AuditStatus = AirOutEnumType.AuditStatusEnum.Auditing;
                            entry.Property(x => x.AuditStatus).IsModified = true;
                            costMoney.ObjectState = ObjectState.Modified;//有变更 必须是Modified，才会触发，不然 UnitOfWork-ef扩展 会将State 重置为ObjectState相同的值
                            UpChang.Add(costMoney);
                        }
                        else
                            continue;
                        break;
                    case "AuditSuccess":
                        if (costMoney.AuditStatus == AirOutEnumType.AuditStatusEnum.Auditing)
                        {
                            var entry = WebdbContxt.Entry(costMoney);
                            entry.State = EntityState.Unchanged;//必须设置为 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                            costMoney.AuditStatus = AirOutEnumType.AuditStatusEnum.AuditSuccess;
                            entry.Property(x => x.AuditStatus).IsModified = true;
                            costMoney.ObjectState = ObjectState.Modified;//有变更 必须是Modified，才会触发，不然 UnitOfWork-ef扩展 会将State 重置为ObjectState相同的值
                            UpChang.Add(costMoney);
                        }
                        else
                            continue;
                        break;
                    case "AuditFail":
                        if (costMoney.AuditStatus == AirOutEnumType.AuditStatusEnum.Auditing)
                        {
                            var entry = WebdbContxt.Entry(costMoney);
                            entry.State = EntityState.Unchanged;//必须设置为 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                            costMoney.AuditStatus = AirOutEnumType.AuditStatusEnum.AuditFail;
                            entry.Property(x => x.AuditStatus).IsModified = true;
                            costMoney.ObjectState = ObjectState.Modified;//有变更 必须是Modified，才会触发，不然 UnitOfWork-ef扩展 会将State 重置为ObjectState相同的值
                            UpChang.Add(costMoney);
                        }
                        else
                            continue;
                        break;
                }
                //_costMoneyService.Update(costMoney);
            }
            try
            {
                _unitOfWork.SaveChanges();
                if (UpChang.Any())
                {
                    ChangViewModel.updated = UpChang;
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(ChangViewModel);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
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
            foreach (var item in ids)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",").Select(x => Int32.Parse(x)).Where(x => x > 0);
            var ArrCostMoney = _costMoneyService.Queryable().Where(x => idarr.Contains(x.Id));

            var ChangViewModel = new CostMoneyChangeViewModel();
            var DelChang = new List<CostMoney>();
            //反向从依赖注入中 取 DbContext
            WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
            foreach (var costMoney in ArrCostMoney)
            {
                if (costMoney.AuditStatus == AirOutEnumType.AuditStatusEnum.Auditing)
                {
                    //状态为审批中，跳过
                    continue;
                }
                if (costMoney.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess)
                {
                    //状态为审批通过，跳过
                    continue;
                }
                DelChang.Add(costMoney);
                _costMoneyService.Delete(costMoney);
            }
            try
            {
                _unitOfWork.SaveChanges();
                if (DelChang.Any())
                {
                    ChangViewModel.deleted = DelChang;
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(ChangViewModel);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
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
        }

        /// <summary>
        /// 列表保存数据变成
        /// </summary>
        /// <param name="costmoney"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData(CostMoneyChangeViewModel costmoney)
        {
            if (costmoney.updated != null)
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

                foreach (var updated in costmoney.updated)
                {
                    _costMoneyService.Update(updated);
                }
            }
            if (costmoney.deleted != null)
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

                foreach (var deleted in costmoney.deleted)
                {
                    _costMoneyService.Delete(deleted);
                }
            }
            if (costmoney.inserted != null)
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

                foreach (var inserted in costmoney.inserted)
                {
                    _costMoneyService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((costmoney.updated != null && costmoney.updated.Any()) ||
                (costmoney.deleted != null && costmoney.deleted.Any()) ||
                (costmoney.inserted != null && costmoney.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(costmoney);
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

        // GET: CostMoneys/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CostMoney costMoney = _costMoneyService.Find(id);
            if (costMoney == null)
            {
                return HttpNotFound();
            }
            return View(costMoney);
        }

        /// <summary>
        /// 创建/复制新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Create(int? id = 0)
        {
            CostMoney costMoney = new CostMoney();
            costMoney.ADDTS = DateTime.Now;
            if (id == 0)
            {
                //costMoney.StartDate = DateTime.Parse(DateTime.Now.ToString("d"));
                //costMoney.EndDate = DateTime.Parse(DateTime.Now.ToString("d")).AddYears(2);
                costMoney.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                costMoney.Status = AirOutEnumType.UseStatusEnum.Enable;
                costMoney.InspectMark = "1";
                costMoney.GetOrdMark = "1";
                costMoney.CurrencyCode = "CNY";
                costMoney.StartPlace = "PVG";
                //costMoney.BSA = "1";
                //costMoney.ProxyOperator = "1";
            }
            else
            {
                CostMoney costMoneyEdit = _costMoneyService.Find(id);
                if (costMoneyEdit == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    costMoney = Common.SetProperties<CostMoney, CostMoney>(costMoneyEdit, "Id,SerialNo,ADDWHO,ADDTS,EDITWHO,EDITTS");
                    //costMoney.SerialNo = SequenceBuilder.NextSerial_No();
                    costMoney.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                    if (costMoneyEdit.ADDTS != null)
                        costMoney.ADDTS = costMoneyEdit.ADDTS;
                }
            }
            //计费公式 缓存
            ViewData["BillFormulaXML"] = AirOut.Web.CacheHelper.Get_SetCache(AirOut.Web.Extensions.Common.CacheNameS.BillFormulaXML);
            var ODynamic = GetFromNAME(costMoney);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return View(costMoney);
        }

        // POST: CostMoneys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(CostMoney costMoney)
        {
            ModelState.Remove("SerialNo");
            if (ModelState.IsValid)
            {
                costMoney.SerialNo = SequenceBuilder.NextCostMoney_No();
                _costMoneyService.Insert(costMoney);
                _unitOfWork.SaveChanges();

                var ChangViewModel = new CostMoneyChangeViewModel();
                ChangViewModel.inserted = new List<CostMoney>() { costMoney };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(ChangViewModel);

                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true, SerialNo = costMoney.SerialNo }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a CostMoney record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            var ODynamic = GetFromNAME(costMoney);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return View(costMoney);
        }

        // GET: CostMoneys/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //计费公式 缓存
            ViewData["BillFormulaXML"] = AirOut.Web.CacheHelper.Get_SetCache(AirOut.Web.Extensions.Common.CacheNameS.BillFormulaXML);
            CostMoney costMoney = _costMoneyService.Find(id);
            if (costMoney.ADDTS == null)
                costMoney.ADDTS = DateTime.Now;
            if (costMoney == null)
            {
                return HttpNotFound();
            }
            var ODynamic = GetFromNAME(costMoney);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return View(costMoney);
        }

        // POST: CostMoneys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(CostMoney costMoney)
        {
            if (ModelState.IsValid)
            {
                costMoney.ObjectState = ObjectState.Modified;
                _costMoneyService.Update(costMoney);
                _unitOfWork.SaveChanges();
                var ChangViewModel = new CostMoneyChangeViewModel();
                ChangViewModel.updated = new List<CostMoney>() { costMoney };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(ChangViewModel);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a CostMoney record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            var ODynamic = GetFromNAME(costMoney);
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            return View(costMoney);
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCostMoney"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(CostMoney OCostMoney)
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
            if (!string.IsNullOrEmpty(OCostMoney.FeeCode))
            {
                var OArrFeeType = ArrFeeType.Where(x => x.FeeCode == OCostMoney.FeeCode).FirstOrDefault();
                ODynamic.FeeTypeNAME = OArrFeeType == null ? "" : OArrFeeType.FeeName;
            }
            //结算方，获取的客商信息
            if (!string.IsNullOrEmpty(OCostMoney.SettleAccount))
            {
                var OArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OCostMoney.SettleAccount).FirstOrDefault();
                ODynamic.SettleAccountNAME = OArrCusBusInfo == null ? "" : OArrCusBusInfo.EnterpriseShortName;
            }
            //订舱方
            if (!string.IsNullOrEmpty(OCostMoney.WHBuyer))
            {
                var OArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OCostMoney.WHBuyer).FirstOrDefault();
                ODynamic.WHBuyerNAME = OArrCusBusInfo == null ? "" : OArrCusBusInfo.EnterpriseShortName;
            }
            //成交条款
            if (!string.IsNullOrEmpty(OCostMoney.DealWithArticle))
            {
                var OArrDealArticle = ArrDealArticle.Where(x => x.DealArticleCode == OCostMoney.DealWithArticle).FirstOrDefault();
                ODynamic.DealWithArticleNAME = OArrDealArticle == null ? "" : OArrDealArticle.DealArticleName;
            }
            //起始地
            if (!string.IsNullOrEmpty(OCostMoney.StartPlace))
            {
                var OArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OCostMoney.StartPlace).FirstOrDefault();
                ODynamic.StartPlaceNAME = OArrPARA_AirPort == null ? "" : OArrPARA_AirPort.PortName;
            }
            ////中转地
            //if (!string.IsNullOrEmpty(OCostMoney.TransitPlace))
            //{
            //    var OArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OCostMoney.TransitPlace).FirstOrDefault();
            //    ODynamic.TransitPlaceNAME = OArrPARA_AirPort == null ? "" : OArrPARA_AirPort.PortName;
            //}
            //目的地
            if (!string.IsNullOrEmpty(OCostMoney.EndPlace))
            {
                var OArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OCostMoney.EndPlace).FirstOrDefault();
                ODynamic.EndPlaceNAME = OArrPARA_AirPort == null ? "" : OArrPARA_AirPort.PortCode;
            }
            //航空公司
            if (!string.IsNullOrEmpty(OCostMoney.AirLineCo))
            {
                var OArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OCostMoney.AirLineCo).FirstOrDefault();
                ODynamic.AirLineCoNAME = OArrCusBusInfo == null ? "" : OArrCusBusInfo.EnterpriseShortName;
            }
            //航班号
            if (!string.IsNullOrEmpty(OCostMoney.AirLineNo))
            {
                var OArrPARA_AirLine = ArrPARA_AirLine.Where(x => x.AirCode == OCostMoney.AirLineNo).FirstOrDefault();
                ODynamic.AirLineNoNAME = OArrPARA_AirLine == null ? "" : OArrPARA_AirLine.AirLine;
            }
            //靠级
            if (!string.IsNullOrEmpty(OCostMoney.MoorLevel))
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "MoorLevel" && x.LISTCODE == OCostMoney.MoorLevel).FirstOrDefault();
                ODynamic.MoorLevelNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //代操作ProxyOperator
            //BSA
            //审批状态AuditStatus
            //使用状态Status
            //报关方式
            if (!string.IsNullOrEmpty(OCostMoney.CustomsType))
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "CusType" && x.LISTCODE == OCostMoney.CustomsType).FirstOrDefault();
                ODynamic.CustomsTypeNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //币制
            if (!string.IsNullOrEmpty(OCostMoney.CurrencyCode))
            {
                var OArrPARA_CURR = ArrPARA_CURR.Where(x => x.CURR_CODE == OCostMoney.CurrencyCode).FirstOrDefault();
                ODynamic.CurrencyCodeNAME = OArrPARA_CURR == null ? "" : OArrPARA_CURR.CURR_Name;
            }
            //计费单位
            if (!string.IsNullOrEmpty(OCostMoney.BillingUnit))
            {
                var BillingUnit = 0;
                if (int.TryParse(OCostMoney.BillingUnit, out BillingUnit))
                {
                    var OArrFeeUnit = ArrFeeUnit.Where(x => x.Id == BillingUnit).FirstOrDefault();
                    ODynamic.BillingUnitNAME = OArrFeeUnit == null ? "" : OArrFeeUnit.FeeUnitName;
                }
            }
            //计算单位
            if (!string.IsNullOrEmpty(OCostMoney.FeeCondition))
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "FeeConditionAp" && x.LISTCODE == OCostMoney.FeeCondition).FirstOrDefault();
                ODynamic.FeeConditionNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //交货点
            if (!string.IsNullOrEmpty(OCostMoney.DeliveryPoint))
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "Delivery_Point" && x.LISTCODE == OCostMoney.DeliveryPoint).FirstOrDefault();
                ODynamic.DeliveryPointNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //商检标志
            //取单标志
            //计费运算符1
            //计费运算符2
            //计费公式
            return ODynamic;
        }

        // GET: CostMoneys/Edit/5
        public ActionResult EditAudit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CostMoney costMoney = _costMoneyService.Find(id);
            if (costMoney.ADDTS == null)
                costMoney.ADDTS = DateTime.Now;
            if (costMoney == null)
            {
                return HttpNotFound();
            }
            return View(costMoney);
        }

        // POST: CostMoneys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditAudit(CostMoney costMoney)
        {
            if (ModelState.IsValid)
            {
                costMoney.ObjectState = ObjectState.Modified;
                _costMoneyService.Update(costMoney);
                _unitOfWork.SaveChanges();
                var ChangViewModel = new CostMoneyChangeViewModel();
                ChangViewModel.updated = new List<CostMoney>() { costMoney };
                //自动更新 缓存
                if (IsAutoResetCache)
                    AutoResetCache(ChangViewModel);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a CostMoney record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(costMoney);
        }

        /// <summary>
        /// 将一个实体类复制到另一个实体类
        /// </summary>
        /// <param name="objectsrc">源实体类</param>
        /// <param name="objectdest">复制到的实体类</param>
        /// <param name="excudeFields">不复制的属性</param>
        public void EntityToEntity(object objectsrc, object objectdest, params string[] excudeFields)
        {
            var sourceType = objectsrc.GetType();
            var destType = objectdest.GetType();
            foreach (var item in destType.GetProperties())
            {
                if (excudeFields.Any(x => x.ToUpper() == item.Name))
                    continue;
                item.SetValue(objectdest, sourceType.GetProperty(item.ToString().ToLower()).GetValue(objectsrc, null), null);
            }
        }

        // GET: CostMoneys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CostMoney costMoney = _costMoneyService.Find(id);
            if (costMoney == null)
            {
                return HttpNotFound();
            }
            return View(costMoney);
        }

        // POST: CostMoneys/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CostMoney costMoney = _costMoneyService.Find(id);
            _costMoneyService.Delete(costMoney);
            _unitOfWork.SaveChanges();
            var ChangViewModel = new CostMoneyChangeViewModel();
            ChangViewModel.deleted = new List<CostMoney>() { costMoney };
            //自动更新 缓存
            if (IsAutoResetCache)
                AutoResetCache(ChangViewModel);
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a CostMoney record");
            return RedirectToAction("Index");
        }

        //获取客商状态枚举
        public ActionResult GetAuditStatus()
        {
            var ArrEnum = Common.GetEnumToDic("AuditStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            var list = ArrEnum.Select(n => new { ID = n.Value.ToString(), TEXT = n.DisplayName, Key = n.Key });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //获取客商状态枚举
        public ActionResult UseStatusIsOrNoEnum()
        {
            var ArrEnum = Common.GetEnumToDic("UseStatusIsOrNoEnum", "AirOut.Web.Models.AirOutEnumType");
            var list = ArrEnum.Select(n => new { ID = n.Value, TEXT = n.DisplayName, Key = n.Key });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //获取运算符
        public ActionResult UseCalcSignEnum()
        {
            var ArrEnum = Common.GetEnumToDic("UseCalcSignEnum", "AirOut.Web.Models.AirOutEnumType");
            var list = ArrEnum.Select(n => new { ID = n.Value, TEXT = n.DisplayName, Key = n.Key });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "成本信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _costMoneyService.ExportExcel(filterRules, sort, order);
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
