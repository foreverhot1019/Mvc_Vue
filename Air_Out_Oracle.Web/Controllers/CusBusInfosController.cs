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
    public class CusBusInfosController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<CusBusInfo>, Repository<CusBusInfo>>();
        //container.RegisterType<ICusBusInfoService, CusBusInfoService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ICusBusInfoService _cusBusInfoService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/CusBusInfos";

        public CusBusInfosController(ICusBusInfoService cusBusInfoService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            IsAutoResetCache = true;
            _cusBusInfoService = cusBusInfoService;
            _unitOfWork = unitOfWork;
        }

        // GET: CusBusInfos/Index
        public ActionResult Index()
        {
            //var cusbusinfo  = _cusBusInfoService.Queryable().AsQueryable();
            //return View(cusbusinfo  );
            return View();
        }

        /// <summary>
        /// 获取列表数据
        /// Get :CusBusInfos/PageList
        /// For Index View Boostrap-Table load  data 
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="sort">排序字段</param>
        /// <param name="order">排序顺序</param>
        /// <param name="filterRules">搜索条件Json数组</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var cusbusinfo = _cusBusInfoService.Query(new CusBusInfoQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = cusbusinfo.Select(n => new
            {
                Id = n.Id,
                EnterpriseId = n.EnterpriseId,
                EnterpriseShortName = n.EnterpriseShortName,
                EnterpriseName = n.EnterpriseName,
                EnterpriseGroupCode = n.EnterpriseGroupCode,
                TopEnterpriseCode = n.TopEnterpriseCode,
                CIQID = n.CIQID,
                CHECKID = n.CHECKID,
                //CustomsId = n.CustomsId,
                CustomsCode = n.CustomsCode,
                CHNName = n.CHNName,
                EngName = n.EngName,
                AddressCHN = n.AddressCHN,
                AddressEng = n.AddressEng,
                WebSite = n.WebSite,
                //TradeTypeId = n.TradeTypeId,
                TradeTypeCode = n.TradeTypeCode,
                //AreaId = n.AreaId,
                AreaCode = n.AreaCode,
                //CountryId = n.CountryId,
                CountryCode = n.CountryCode,
                //CopoKindId = n.CopoKindId,
                CopoKindCode = n.CopoKindCode,
                CorpartiPerson = n.CorpartiPerson,
                ResteredCapital = n.ResteredCapital,
                IsInternalCompany = n.IsInternalCompany,
                Description = n.Description,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                n.ADDID,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS,
                n.EDITID,
                Currency = n.Currency,
                EnterpriseGroup = n.EnterpriseGroup,
                n.SallerId,
                n.SallerName,
            }).ToList();

            #region 取缓存数据

            var ArrCopoKindCode = (List<CoPoKind>)CacheHelper.Get_SetCache(Common.CacheNameS.CoPoKind);
            var ArrTradeType = (List<TradeType>)CacheHelper.Get_SetCache(Common.CacheNameS.TradeType);
            var ArrPARA_Country = (List<PARA_Country>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Country);
            var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            var ArrPARA_Customs = (List<PARA_Customs>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Customs);
            var ArrPARA_Area = (List<PARA_Area>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Area);

            #endregion

            var RetData = from n in datarows
                          join a in ArrCopoKindCode on n.CopoKindCode equals a.Code into a_tmp
                          from atmp in a_tmp.DefaultIfEmpty()
                          join b in ArrTradeType on n.TradeTypeCode equals b.Code into b_tmp
                          from btmp in b_tmp.DefaultIfEmpty()
                          join c in ArrPARA_Country on n.CountryCode equals c.COUNTRY_CO into c_tmp
                          from ctmp in c_tmp.DefaultIfEmpty()
                          join d in ArrPARA_CURR on n.Currency equals d.CURR_CODE into d_tmp
                          from dtmp in d_tmp.DefaultIfEmpty()
                          join e in ArrPARA_Customs on n.CustomsCode equals e.Customs_Code into e_tmp
                          from etmp in e_tmp.DefaultIfEmpty()
                          join f in ArrPARA_Area on new { CountryCode = n.CountryCode, AreaCode = n.AreaCode } equals new { CountryCode = f.Country_CO, AreaCode = f.AreaCode } into f_tmp
                          from ftmp in f_tmp.DefaultIfEmpty()
                          select new
            {
                n.Id,
                n.EnterpriseId,
                n.EnterpriseShortName,
                n.EnterpriseName,
                n.EnterpriseGroupCode,
                n.TopEnterpriseCode,
                n.EnterpriseGroup,
                n.CIQID,
                n.CHECKID,
                n.CHNName,
                n.EngName,
                n.AddressCHN,
                n.AddressEng,
                n.WebSite,
                n.CustomsCode,
                CustomsCodeNAME = etmp == null ? "" : etmp.Customs_Name,
                n.TradeTypeCode,
                TradeTypeCodeNAME = btmp == null ? "" : btmp.Name,
                n.AreaCode,
                AreaCodeNAME = ftmp == null ? "" : ftmp.AreaName,
                Currency = n.Currency,
                CurrencyNAME = dtmp == null ? "" : dtmp.CURR_Name,
                n.CountryCode,
                CountryCodeNAME = ctmp == null ? "" : ctmp.COUNTRY_NA,
                n.CopoKindCode,
                CopoKindCodeNAME = atmp == null ? "" : atmp.Name,
                n.CorpartiPerson,
                n.ResteredCapital,
                n.IsInternalCompany,
                n.Description,
                n.Status,
                n.OperatingPoint,
                n.ADDWHO,
                n.ADDTS,
                n.ADDID,
                n.EDITWHO,
                n.EDITTS,
                n.EDITID,
                n.SallerId,
                n.SallerName,
            };

            var pagelist = new { total = totalCount, rows = RetData };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 列表保存
        /// </summary>
        /// <param name="cusbusinfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData(CusBusInfoChangeViewModel cusbusinfo)
        {
            if (cusbusinfo.updated != null)
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

                foreach (var updated in cusbusinfo.updated)
                {
                    _cusBusInfoService.Update(updated);
                }
            }
            if (cusbusinfo.deleted != null)
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

                foreach (var deleted in cusbusinfo.deleted)
                {
                    _cusBusInfoService.Delete(deleted);
                }
            }
            if (cusbusinfo.inserted != null)
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

                foreach (var inserted in cusbusinfo.inserted)
                {
                    //inserted.Status = "1";
                    _cusBusInfoService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((cusbusinfo.updated != null && cusbusinfo.updated.Any()) ||
                (cusbusinfo.deleted != null && cusbusinfo.deleted.Any()) ||
                (cusbusinfo.inserted != null && cusbusinfo.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(cusbusinfo);
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
        /// 测试 数据唯一验证
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="EnterpriseId"></param>
        /// <returns></returns>
        public ActionResult GetIsUse(int Id, string EnterpriseId)
        {
            var IsUse = _cusBusInfoService.Queryable().Any(x => x.EnterpriseId == EnterpriseId && x.Id != Id);
            return Json(!IsUse);
        }

        /// <summary>
        /// GET: CusBusInfos/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CusBusInfo cusBusInfo = _cusBusInfoService.Find(id);
            if (cusBusInfo == null)
            {
                return HttpNotFound();
            }
            return View(cusBusInfo);
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(CusBusInfo OCusBusInfo)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();
            if (OCusBusInfo.SallerId != null && OCusBusInfo.SallerId>0)
            {
                var ArrSaller = (List<Saller>)CacheHelper.Get_SetCache(Common.CacheNameS.Saller);
                var OSaller = ArrSaller.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Id == OCusBusInfo.SallerId).FirstOrDefault();
                ODynamic.SallerIdNAME = OSaller == null ? "" : OSaller.Name;
            }
            if (!string.IsNullOrEmpty(OCusBusInfo.CopoKindCode))
            {
                var ArrCopoKindCode = (List<CoPoKind>)CacheHelper.Get_SetCache(Common.CacheNameS.CoPoKind);
                var OCopoKindCode = ArrCopoKindCode.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Code == OCusBusInfo.CopoKindCode).FirstOrDefault();
                ODynamic.CopoKindCodeNAME = OCopoKindCode == null ? "" : OCopoKindCode.Name;
            }
            if (!string.IsNullOrEmpty(OCusBusInfo.TradeTypeCode))
            {
                var ArrTradeType = (List<TradeType>)CacheHelper.Get_SetCache(Common.CacheNameS.TradeType);
                var OTradeType = ArrTradeType.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Code == OCusBusInfo.TradeTypeCode).FirstOrDefault();
                ODynamic.TradeTypeCodeNAME = OTradeType == null ? "" : OTradeType.Name;
            }
            if (!string.IsNullOrEmpty(OCusBusInfo.CountryCode) || !string.IsNullOrEmpty(OCusBusInfo.InvoiceCountryCode))
            {
                var ArrPARA_Country = (List<PARA_Country>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Country);
                if (!string.IsNullOrEmpty(OCusBusInfo.CountryCode))
                {
                    var OPARA_Country = ArrPARA_Country.Where(x => x.COUNTRY_CO == OCusBusInfo.CountryCode).FirstOrDefault();
                    ODynamic.CountryCodeNAME = OPARA_Country == null ? "" : OPARA_Country.COUNTRY_NA;
                }
                if (!string.IsNullOrEmpty(OCusBusInfo.InvoiceCountryCode))
                {
                    var OPARA_Country = ArrPARA_Country.Where(x => x.COUNTRY_CO == OCusBusInfo.InvoiceCountryCode).FirstOrDefault();
                    ODynamic.InvoiceCountryCodeNAME = OPARA_Country == null ? "" : OPARA_Country.COUNTRY_NA;
                }
            }
            if (!string.IsNullOrEmpty(OCusBusInfo.Currency))
            {
                var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
                var OPARA_CURR = ArrPARA_CURR.Where(x => x.CURR_CODE == OCusBusInfo.Currency).FirstOrDefault();
                ODynamic.CurrencyNAME = OPARA_CURR == null ? "" : OPARA_CURR.CURR_Name;
            }
            if (!string.IsNullOrEmpty(OCusBusInfo.CustomsCode))
            {
                var ArrPARA_Customs = (List<PARA_Customs>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Customs);
                var OPARA_Customs = ArrPARA_Customs.Where(x => x.Customs_Code == OCusBusInfo.CustomsCode).FirstOrDefault();
                ODynamic.CustomsCodeNAME = OPARA_Customs == null ? "" : OPARA_Customs.Customs_Name;
            }
            if (!string.IsNullOrEmpty(OCusBusInfo.CustomsCode) && (!string.IsNullOrEmpty(OCusBusInfo.AreaCode) || !string.IsNullOrEmpty(OCusBusInfo.ECCAreaCode)))
            {
                var ArrPARA_Area = (List<PARA_Area>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Area);
                if (!string.IsNullOrEmpty(OCusBusInfo.AreaCode))
                {
                    var OPARA_Area = ArrPARA_Area.Where(x => x.Country_CO == OCusBusInfo.CountryCode && x.ID.ToString() == OCusBusInfo.AreaCode).FirstOrDefault();
                    ODynamic.AreaCodeNAME = OPARA_Area == null ? "" : OPARA_Area.AreaName;
                }
                if (!string.IsNullOrEmpty(OCusBusInfo.ECCAreaCode))
                {
                    var OPARA_Area = ArrPARA_Area.Where(x => x.Country_CO == OCusBusInfo.CountryCode && x.ID.ToString() == OCusBusInfo.ECCAreaCode).FirstOrDefault();
                    ODynamic.ECCAreaCodeNAME = OPARA_Area == null ? "" : OPARA_Area.AreaName;
                }
            }
            if (!string.IsNullOrEmpty(OCusBusInfo.TaxPayerType) || !string.IsNullOrEmpty(OCusBusInfo.EnterpriseType))
            {
                var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
                if (!string.IsNullOrEmpty(OCusBusInfo.TaxPayerType))
                {
                    var OBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "TaxPayerType" && x.LISTCODE == OCusBusInfo.TaxPayerType).FirstOrDefault();
                    ODynamic.TaxPayerTypeNAME = OBD_DEFDOC_LIST == null ? "" : OBD_DEFDOC_LIST.LISTNAME;
                }
                if (!string.IsNullOrEmpty(OCusBusInfo.TaxPayerType))
                {
                    var OBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "EnterpriseType" && x.LISTCODE == OCusBusInfo.TaxPayerType).FirstOrDefault();
                    ODynamic.EnterpriseTypeNAME = OBD_DEFDOC_LIST == null ? "" : OBD_DEFDOC_LIST.LISTNAME;
                }
            }

            if (!string.IsNullOrEmpty(OCusBusInfo.Delivery_Point))
            {
                var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
                var OBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "Delivery_Point" && x.LISTCODE == OCusBusInfo.Delivery_Point).FirstOrDefault();
                ODynamic.Delivery_PointNAME = OBD_DEFDOC_LIST == null ? "" : OBD_DEFDOC_LIST.LISTNAME;
            }
            return ODynamic;
        }

        // GET: CusBusInfos/Create
        public ActionResult Create()
        {
            CusBusInfo cusBusInfo = new CusBusInfo();
            //set default value  
            return View(cusBusInfo);
        }

        // POST: CusBusInfos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(CusBusInfo cusBusInfo)
        {
            if (ModelState.IsValid)
            {
                //防止前台 数据未设置上
                //if (cusBusInfo.SallerId.HasValue && cusBusInfo.SallerId > 0)
                //{
                //    var ArrSaller = (List<Saller>)CacheHelper.Get_SetCache(Common.CacheNameS.Saller);
                //    var OSaller = ArrSaller.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Id == cusBusInfo.SallerId).FirstOrDefault();
                //    cusBusInfo.SallerName = OSaller == null ? "" : OSaller.Name;
                //}
                _cusBusInfoService.Insert(cusBusInfo);
                _unitOfWork.SaveChanges();

                //自动更新 缓存
                if (IsAutoResetCache)
                {
                    CusBusInfoChangeViewModel cusbusinfo = new CusBusInfoChangeViewModel();
                    cusbusinfo.inserted = new List<CusBusInfo>() { cusBusInfo };
                    AutoResetCache(cusbusinfo);
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a CusBusInfo record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var ODynamic = GetFromNAME(cusBusInfo);
                ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            }

            DisplayErrorMessage();
            return View(cusBusInfo);
        }

        // GET: CusBusInfos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CusBusInfo cusBusInfo = _cusBusInfoService.Find(id);
            if (cusBusInfo == null)
            {
                return HttpNotFound();
            }
            else
            {
                var ODynamic = GetFromNAME(cusBusInfo);
                ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            }
            return View(cusBusInfo);
        }

        // POST: CusBusInfos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(CusBusInfo cusBusInfo)
        {
            if (ModelState.IsValid)
            {
                //防止前台 数据未设置上
                //if (cusBusInfo.SallerId.HasValue && cusBusInfo.SallerId > 0)
                //{
                //    var ArrSaller = (List<Saller>)CacheHelper.Get_SetCache(Common.CacheNameS.Saller);
                //    var OSaller = ArrSaller.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Id == cusBusInfo.SallerId).FirstOrDefault();
                //    cusBusInfo.SallerName = OSaller == null ? "" : OSaller.Name;
                //}
                cusBusInfo.ObjectState = ObjectState.Modified;
                _cusBusInfoService.Update(cusBusInfo);
                _unitOfWork.SaveChanges();
                //自动更新 缓存
                if (IsAutoResetCache)
                {
                    CusBusInfoChangeViewModel cusbusinfo = new CusBusInfoChangeViewModel();
                    cusbusinfo.updated = new List<CusBusInfo>() { cusBusInfo };
                    AutoResetCache(cusbusinfo);
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a CusBusInfo record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var ODynamic = GetFromNAME(cusBusInfo);
                ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            }
            DisplayErrorMessage();
            return View(cusBusInfo);
        }

        // GET: CusBusInfos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CusBusInfo cusBusInfo = _cusBusInfoService.Find(id);
            if (cusBusInfo == null)
            {
                return HttpNotFound();
            }
            return View(cusBusInfo);
        }

        // POST: CusBusInfos/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CusBusInfo cusBusInfo = _cusBusInfoService.Find(id);
            _cusBusInfoService.Delete(cusBusInfo);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a CusBusInfo record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "cusbusinfo_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _cusBusInfoService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取客商信息
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerCusBusInfos(int page = 1, int rows = 10, string q = "")
        {
            var cusBusInfoRep = _unitOfWork.RepositoryAsync<CusBusInfo>();
            var data = cusBusInfoRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.EnterpriseId.Contains(q) || n.EnterpriseShortName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.EnterpriseId, TEXT = n.EnterpriseShortName, EnterpriseName = n.EnterpriseName, CHNName = n.CHNName, IDTEXT = n.EnterpriseId + "|" + n.EnterpriseShortName, AddressEng = n.AddressEng, SallerId = n.SallerId, SallerName = n.SallerName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取客商信息
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerCusBusInfos_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var cusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var BD_DEFDOC_LISTRep = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            var QArrBD_DEFDOC_LIST = BD_DEFDOC_LISTRep.Where(x => x.DOCCODE == "Delivery_Point");//交货地点
            var QCusBusInfo = cusBusInfoRep.Where(x => x.EnterpriseId == null || x.EnterpriseShortName == null || x.CHNName == null);
            foreach (var item in QCusBusInfo)
            {
                item.EnterpriseId = item.EnterpriseId ?? "";
                //item.EnterpriseName = item.EnterpriseName ?? "";
                item.CHNName = item.CHNName ?? "";
                item.EnterpriseShortName = item.EnterpriseShortName ?? "";
            }
            if (q != "")
            {
                cusBusInfoRep = cusBusInfoRep.Where(n => n.EnterpriseId.StartsWith(q) || n.EnterpriseShortName.StartsWith(q));
            }

            var list = (from n in cusBusInfoRep
                        join m in QArrBD_DEFDOC_LIST on n.Delivery_Point equals m.LISTCODE into m_tmp
                        from mtmp in m_tmp.DefaultIfEmpty()
                        select new
                        {
                            ID = n.EnterpriseId,
                            TEXT = n.EnterpriseShortName,
                            EnterpriseName = n.EnterpriseName,
                            CHNName = n.CHNName,
                            IDTEXT = n.EnterpriseId + "|" + n.EnterpriseShortName,
                            AddressEng = n.AddressEng,
                            SallerId = n.SallerId,
                            SallerName = n.SallerName,
                            Delivery_PointCode = n.Delivery_Point,
                            Delivery_PointName = mtmp == null ? "" : mtmp.LISTNAME
                        }).ToList();
            //var list = cusBusInfoRep.Select(n => new 
            //{ 
            //    ID = n.EnterpriseId, 
            //    TEXT = n.EnterpriseShortName, 
            //    EnterpriseName = n.EnterpriseName, 
            //    CHNName = n.CHNName, 
            //    IDTEXT = n.EnterpriseId + "|" + n.EnterpriseShortName, 
            //    AddressEng = n.AddressEng, 
            //    SallerId = n.SallerId, 
            //    SallerName = n.SallerName 
            //});

            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取客商联系人
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerCusBusInfo_Linker(int page = 1, int rows = 10, string q = "")
        {
            var cusBusInfoRep = _unitOfWork.RepositoryAsync<CusBusInfo>();
            var data = cusBusInfoRep.Query(n => !string.IsNullOrEmpty(n.LinkerMan)).Select().AsQueryable();
            if (q != "")
            {
                data = data.Where(n => n.LinkerMan.Contains(q) || n.EnterpriseId.Contains(q) || n.EnterpriseShortName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.EnterpriseId, TEXT = n.LinkerMan, TelePhone = n.Telephone, Fax = n.Fax });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取客商联系人
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerCusBusInfo_Linker_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var cusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var QCusBusInfo = cusBusInfoRep.Where(x => x.EnterpriseId == null || x.EnterpriseShortName == null || x.CHNName == null);
            foreach (var item in QCusBusInfo)
            {
                item.EnterpriseId = item.EnterpriseId ?? "";
                //item.EnterpriseName = item.EnterpriseName ?? "";
                item.CHNName = item.CHNName ?? "";
                item.EnterpriseShortName = item.EnterpriseShortName ?? "";
            }
            var QcusBusInfoRep = cusBusInfoRep.Where(x => !string.IsNullOrEmpty(x.LinkerMan));
            if (q != "")
            {
                QcusBusInfoRep = QcusBusInfoRep.Where(n => n.LinkerMan.Contains(q) || n.EnterpriseId.StartsWith(q) || n.EnterpriseShortName.StartsWith(q));
            }
            var list = QcusBusInfoRep.Select(n => new { ID = n.EnterpriseId, TEXT = n.LinkerMan, TelePhone = n.Telephone, Fax = n.Fax });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //获取客商状态枚举
        public ActionResult GetStatus()
        {
            var ArrEnum = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            var list = ArrEnum.Select(n => new { Value = n.Value, Text = n.DisplayName, Key = n.Key });

            return Json(list, JsonRequestBehavior.AllowGet);
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
