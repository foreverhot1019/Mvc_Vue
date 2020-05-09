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
    public class PARA_PackagesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<PARA_Package>, Repository<PARA_Package>>();
        //container.RegisterType<IPARA_PackageService, PARA_PackageService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPARA_PackageService _pARA_PackageService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/PARA_Packages";

        public PARA_PackagesController(IPARA_PackageService pARA_PackageService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _pARA_PackageService = pARA_PackageService;
            _unitOfWork = unitOfWork;
        }

        // GET: PARA_Packages/Index
        public ActionResult Index()
        {
            //var para_package  = _pARA_PackageService.Queryable().AsQueryable();
            //return View(para_package  );
            return View();
        }

        // Get :PARA_Packages/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var para_package = _pARA_PackageService.Query(new PARA_PackageQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = para_package.Select(n => new
            {
                Id = n.Id,
                PackageCode = n.PackageCode,
                PackageName = n.PackageName,
                IsWood = n.IsWood,
                Description = n.Description,
                Status = n.Status,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(PARA_PackageChangeViewModel para_package)
        {
            if (para_package.updated != null)
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

                foreach (var updated in para_package.updated)
                {
                    _pARA_PackageService.Update(updated);
                }
            }
            if (para_package.deleted != null)
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

                foreach (var deleted in para_package.deleted)
                {
                    _pARA_PackageService.Delete(deleted);
                }
            }
            if (para_package.inserted != null)
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

                foreach (var inserted in para_package.inserted)
                {
                    _pARA_PackageService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((para_package.updated != null && para_package.updated.Any()) ||
                (para_package.deleted != null && para_package.deleted.Any()) ||
                (para_package.inserted != null && para_package.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(para_package);
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

        // GET: PARA_Packages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Package pARA_Package = _pARA_PackageService.Find(id);
            if (pARA_Package == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Package);
        }

        // GET: PARA_Packages/Create
        public ActionResult Create()
        {
            PARA_Package pARA_Package = new PARA_Package();
            //set default value
            return View(pARA_Package);
        }

        // POST: PARA_Packages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PackageCode,PackageName,IsWood,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_Package pARA_Package)
        {
            if (ModelState.IsValid)
            {
                _pARA_PackageService.Insert(pARA_Package);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a PARA_Package record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_Package);
        }

        // GET: PARA_Packages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Package pARA_Package = _pARA_PackageService.Find(id);
            if (pARA_Package == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Package);
        }

        // POST: PARA_Packages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PackageCode,PackageName,IsWood,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_Package pARA_Package)
        {
            if (ModelState.IsValid)
            {
                pARA_Package.ObjectState = ObjectState.Modified;
                _pARA_PackageService.Update(pARA_Package);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a PARA_Package record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_Package);
        }

        // GET: PARA_Packages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Package pARA_Package = _pARA_PackageService.Find(id);
            if (pARA_Package == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Package);
        }

        // POST: PARA_Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PARA_Package pARA_Package = _pARA_PackageService.Find(id);
            _pARA_PackageService.Delete(pARA_Package);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a PARA_Package record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "para_package_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _pARA_PackageService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取包装方式
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">每页行</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_Packages(int page = 1, int rows = 10, string q = "")
        {
            var PARA_PackageRep = _unitOfWork.RepositoryAsync<PARA_Package>();
            var post = PARA_PackageRep.Queryable();
            if (q != "")
            {
                post = post.Where(n => n.PackageCode.Contains(q) || n.PackageName.Contains(q));
            }
            var list = post.Select(n => new { ID = n.PackageCode, TEXT = n.PackageName });

            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取包装方式
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">每页行</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_AirPorts_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var PARA_Package = (IEnumerable<PARA_Package>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Package);
            var QPARA_Package = PARA_Package.Where(x => x.PackageCode == null || x.PackageName == null);
            if (QPARA_Package.Any())
            {
                foreach (var item in QPARA_Package)
                {
                    item.PackageCode = item.PackageCode ?? "";
                    item.PackageName = item.PackageName ?? "";
                }
            }
            if (q != "")
            {
                PARA_Package = PARA_Package.Where(n => n.PackageCode.Contains(q) || n.PackageName.Contains(q));
            }
            var listpost = PARA_Package.Select(n => new { ID = n.PackageCode, TEXT = n.PackageName });

            int totalCount = 0;
            totalCount = listpost.Count();
            return Json(new { total = totalCount, rows = listpost.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
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
