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
using TMI.Web.Models;
using TMI.Web.Services;
using TMI.Web.Repositories;
using TMI.Web.Extensions;

namespace TMI.Web.Controllers
{
    public class PARA_CountriesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<PARA_Country>, Repository<PARA_Country>>();
        //container.RegisterType<IPARA_CountryService, PARA_CountryService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPARA_CountryService _pARA_CountryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/PARA_Countries";

        public PARA_CountriesController(IPARA_CountryService pARA_CountryService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _pARA_CountryService = pARA_CountryService;
            _unitOfWork = unitOfWork;
        }

        // GET: PARA_Countries/Index
        public ActionResult Index()
        {
            //var para_country  = _pARA_CountryService.Queryable().AsQueryable();
            //return View(para_country  );
            return View();
        }

        // Get :PARA_Countries/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "COUNTRY_CO", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var para_country = _pARA_CountryService.Query(new PARA_CountryQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = para_country.Select(n => new
            {
                COUNTRY_CO = n.COUNTRY_CO,
                COUNTRY_NO = n.COUNTRY_NO,
                COUNTRY_EN = n.COUNTRY_EN,
                COUNTRY_NA = n.COUNTRY_NA,
                EXAM_MARK = n.EXAM_MARK,
                HIGH_LOW = n.HIGH_LOW,
                Status = n.Status
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(PARA_CountryChangeViewModel para_country)
        {
            if (para_country.updated != null)
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

                foreach (var updated in para_country.updated)
                {
                    _pARA_CountryService.Update(updated);
                }
            }
            if (para_country.deleted != null)
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

                foreach (var deleted in para_country.deleted)
                {
                    _pARA_CountryService.Delete(deleted);
                }
            }
            if (para_country.inserted != null)
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

                foreach (var inserted in para_country.inserted)
                {
                    _pARA_CountryService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((para_country.updated != null && para_country.updated.Any()) ||
                (para_country.deleted != null && para_country.deleted.Any()) ||
                (para_country.inserted != null && para_country.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(para_country);
                    return Json(new { Success = true, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作", ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: PARA_Countries/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Country pARA_Country = _pARA_CountryService.Find(id);
            if (pARA_Country == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Country);
        }

        // GET: PARA_Countries/Create
        public ActionResult Create()
        {
            PARA_Country pARA_Country = new PARA_Country();
            //set default value
            return View(pARA_Country);
        }

        // POST: PARA_Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "COUNTRY_CO,COUNTRY_EN,COUNTRY_NA,EXAM_MARK,HIGH_LOW,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_Country pARA_Country)
        {
            if (ModelState.IsValid)
            {
                _pARA_CountryService.Insert(pARA_Country);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a PARA_Country record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_Country);
        }

        // GET: PARA_Countries/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Country pARA_Country = _pARA_CountryService.Find(id);
            if (pARA_Country == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Country);
        }

        // POST: PARA_Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "COUNTRY_CO,COUNTRY_EN,COUNTRY_NA,EXAM_MARK,HIGH_LOW,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_Country pARA_Country)
        {
            if (ModelState.IsValid)
            {
                pARA_Country.ObjectState = ObjectState.Modified;
                _pARA_CountryService.Update(pARA_Country);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a PARA_Country record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_Country);
        }

        // GET: PARA_Countries/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Country pARA_Country = _pARA_CountryService.Find(id);
            if (pARA_Country == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Country);
        }

        // POST: PARA_Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PARA_Country pARA_Country = _pARA_CountryService.Find(id);
            _pARA_CountryService.Delete(pARA_Country);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a PARA_Country record");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 获取国家代码
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_Country(int page = 1, int rows = 10, string q = "")
        {
            var para_CountryRep = _unitOfWork.RepositoryAsync<PARA_Country>();
            var data = para_CountryRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.COUNTRY_NA.Contains(q) || n.COUNTRY_CO.Contains(q) || n.COUNTRY_EN.Contains(q));
            }
            var list = data.Select(n => new { ID = n.COUNTRY_CO, TEXT = n.COUNTRY_NA, Name = n.COUNTRY_NA,ENName = n.COUNTRY_EN });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取国家代码
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_Country_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var para_CountryRep = (IEnumerable<PARA_Country>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Country);
            var Qpara_Country = para_CountryRep.Where(x => x.COUNTRY_NA == null || x.COUNTRY_EN == null || x.COUNTRY_CO == null);
            if (Qpara_Country.Any())
            {
                foreach (var item in Qpara_Country)
                {
                    item.COUNTRY_CO = item.COUNTRY_CO ?? "";
                    item.COUNTRY_NA = item.COUNTRY_NA ?? "";
                    item.COUNTRY_EN = item.COUNTRY_EN ?? "";
                }
            }
            if (q != "")
            {
                para_CountryRep = para_CountryRep.Where(n => n.COUNTRY_NA.Contains(q) || n.COUNTRY_CO.Contains(q) || n.COUNTRY_EN.Contains(q));
            }
            var list = para_CountryRep.Select(n => new { ID = n.COUNTRY_CO, TEXT = n.COUNTRY_NA, Name = n.COUNTRY_NA, ENName = n.COUNTRY_EN });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "para_country_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _pARA_CountryService.ExportExcel(filterRules, sort, order);
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
