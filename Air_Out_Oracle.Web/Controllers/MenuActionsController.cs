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
    public class MenuActionsController : Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<MenuAction>, Repository<MenuAction>>();
        //container.RegisterType<IMenuActionService, MenuActionService>();

        private WebdbContext db = new WebdbContext();
        private readonly IMenuActionService _menuActionService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public MenuActionsController(IMenuActionService menuActionService, IUnitOfWorkAsync unitOfWork)
        {
            _menuActionService = menuActionService;
            _unitOfWork = unitOfWork;
        }

        // GET: MenuActions/Index
        public ActionResult Index()
        {
            #region

            var ControllActinMsg = "查看";
            bool IsHaveQx = Common.ModelIsHaveQX("/MenuActions", "View", ControllActinMsg);
            if (!IsHaveQx)
            {
                Common.ExitToNoQx("您没有权限" + ControllActinMsg);
            }

            #endregion
            //var menuactions  = _menuActionService.Queryable().AsQueryable();
            //return View(menuactions  );
            return View();
        }

        // Get :MenuActions/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var menuactions = _menuActionService.Query(new MenuActionQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = menuactions.Select(n => new
            {
                Id = n.Id,
                Name = n.Name,
                Code = n.Code,
                Sort = n.Sort,
                IsEnabled = n.IsEnabled,
                Description = n.Description
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(MenuActionChangeViewModel menuactions)
        {
            if (menuactions.updated != null)
            {
                #region

                var ControllActinMsg = "编辑";
                bool IsHaveQx = Common.ModelIsHaveQX("/MenuActions", "Edit", ControllActinMsg);
                if (!IsHaveQx)
                {
                    Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                }

                #endregion

                foreach (var updated in menuactions.updated)
                {
                    updated.ObjectState = ObjectState.Modified;
                    //db.MenuActions.Attach(updated);
                    //db.SaveChanges();
                    _menuActionService.Update(updated);
                }
            }

            if (menuactions.deleted != null)
            {
                #region

                var ControllActinMsg = "删除";
                bool IsHaveQx = Common.ModelIsHaveQX("/MenuActions", "Delete", ControllActinMsg);
                if (!IsHaveQx)
                {
                    Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                }

                #endregion

                foreach (var deleted in menuactions.deleted)
                {
                    _menuActionService.Delete(deleted);
                }
            }

            if (menuactions.inserted != null)
            {
                #region

                var ControllActinMsg = "创建";
                bool IsHaveQx = Common.ModelIsHaveQX("/MenuActions", "Create", ControllActinMsg);
                if (!IsHaveQx)
                {
                    Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                }

                #endregion

                foreach (var inserted in menuactions.inserted)
                {
                    _menuActionService.Insert(inserted);
                }
            }
            try
            {
                _unitOfWork.SaveChanges();

                if (menuactions.updated != null || menuactions.deleted != null || menuactions.inserted != null)
                {
                    if (menuactions.updated.Any() || menuactions.deleted.Any() || menuactions.inserted.Any())
                    {
                        #region 更新菜单动作缓存

                        //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                        lock (Common.lockCacheMenuAction)
                        {
                            List<MenuAction> ArrMenuAction = _unitOfWork.Repository<MenuAction>().Query().Select().OrderBy(x => x.Code).ToList();
                            if (HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] != null)
                                HttpContext.Cache.Remove(Common.GeCacheEnumByName("MenuAction").ToString());
                            HttpContext.Cache.Insert(Common.GeCacheEnumByName("MenuAction").ToString(), ArrMenuAction, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                        }

                        #endregion
                    }
                }
                if (Request.IsAjaxRequest())
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Common.AddError(ex, NotificationTag.Sys, "系统异常");
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        // GET: MenuActions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuAction menuAction = _menuActionService.Find(id);
            if (menuAction == null)
            {
                return HttpNotFound();
            }
            return View(menuAction);
        }

        // GET: MenuActions/Create
        public ActionResult Create()
        {
            #region

            var ControllActinMsg = "创建";
            bool IsHaveQx = Common.ModelIsHaveQX("/MenuActions", "Create", ControllActinMsg);
            if (!IsHaveQx)
            {
                Common.ExitToNoQx("您没有权限" + ControllActinMsg);
            }

            #endregion

            MenuAction menuAction = new MenuAction();
            //set default value
            return View(menuAction);
        }

        // POST: MenuActions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Code,Sort,IsEnabled,Description,CreatedUserId,CreatedDateTime,LastEditUserId,LastEditDateTime")] MenuAction menuAction)
        {
            #region

            var ControllActinMsg = "创建";
            bool IsHaveQx = Common.ModelIsHaveQX("/MenuActions", "Create", ControllActinMsg);
            if (!IsHaveQx)
            {
                Common.ExitToNoQx("您没有权限" + ControllActinMsg);
            }

            #endregion

            if (ModelState.IsValid)
            {
                _menuActionService.Insert(menuAction);
                try
                {
                    _unitOfWork.SaveChanges();

                    #region 更新菜单动作缓存

                    //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                    lock (Common.lockCacheMenuAction)
                    {
                        List<MenuAction> ArrMenuAction = _unitOfWork.Repository<MenuAction>().Query().Select().OrderBy(x => x.Code).ToList();
                        if (HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] != null)
                            HttpContext.Cache.Remove(Common.GeCacheEnumByName("MenuAction").ToString());
                        HttpContext.Cache.Insert(Common.GeCacheEnumByName("MenuAction").ToString(), ArrMenuAction, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                    }

                    #endregion

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    //Common.AddError(ex, NotificationTag.Sys, "系统异常");
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }

                DisplaySuccessMessage("Has append a MenuAction record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(menuAction);
        }

        // GET: MenuActions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuAction menuAction = _menuActionService.Find(id);
            if (menuAction == null)
            {
                return HttpNotFound();
            }
            return View(menuAction);
        }

        // POST: MenuActions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Code,Sort,IsEnabled,Description,CreatedUserId,CreatedDateTime,LastEditUserId,LastEditDateTime")] MenuAction menuAction)
        {
            #region

            var ControllActinMsg = "编辑";
            bool IsHaveQx = Common.ModelIsHaveQX("/MenuActions", "Edit", ControllActinMsg);
            if (!IsHaveQx)
            {
                Common.ExitToNoQx("您没有权限" + ControllActinMsg);
            }

            #endregion

            if (ModelState.IsValid)
            {
                menuAction.ObjectState = ObjectState.Modified;
                _menuActionService.Update(menuAction);
                try
                {
                    _unitOfWork.SaveChanges();

                    #region 更新菜单动作缓存

                    //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                    lock (Common.lockCacheMenuAction)
                    {
                        List<MenuAction> ArrMenuAction = _unitOfWork.Repository<MenuAction>().Query().Select().OrderBy(x => x.Code).ToList();
                        if (HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] != null)
                            HttpContext.Cache.Remove(Common.GeCacheEnumByName("MenuAction").ToString());
                        HttpContext.Cache.Insert(Common.GeCacheEnumByName("MenuAction").ToString(), ArrMenuAction, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                    }

                    #endregion

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    //Common.AddError(ex, NotificationTag.Sys, "系统异常");
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }

                DisplaySuccessMessage("Has update a MenuAction record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(menuAction);
        }

        // GET: MenuActions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuAction menuAction = _menuActionService.Find(id);
            if (menuAction == null)
            {
                return HttpNotFound();
            }
            return View(menuAction);
        }

        // POST: MenuActions/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            #region

            var ControllActinMsg = "删除";
            bool IsHaveQx = Common.ModelIsHaveQX("/MenuActions", "Delete", ControllActinMsg);
            if (!IsHaveQx)
            {
                Common.ExitToNoQx("您没有权限" + ControllActinMsg);
            }

            #endregion

            MenuAction menuAction = _menuActionService.Find(id);
            _menuActionService.Delete(menuAction);
            try
            {
                _unitOfWork.SaveChanges();

                #region 更新菜单动作缓存

                //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                lock (Common.lockCacheMenuAction)
                {
                    List<MenuAction> ArrMenuAction = _unitOfWork.Repository<MenuAction>().Query().Select().OrderBy(x => x.Code).ToList();
                    if (HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] != null)
                        HttpContext.Cache.Remove(Common.GeCacheEnumByName("MenuAction").ToString());
                    HttpContext.Cache.Insert(Common.GeCacheEnumByName("MenuAction").ToString(), ArrMenuAction, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                #endregion

                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //Common.AddError(ex, NotificationTag.Sys, "系统异常");
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            DisplaySuccessMessage("Has delete a MenuAction record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            #region

            var ControllActinMsg = "导出";
            bool IsHaveQx = Common.ModelIsHaveQX("/MenuActions", "ExPort", ControllActinMsg);
            if (!IsHaveQx)
            {
                Common.ExitToNoQx("您没有权限" + ControllActinMsg);
            }

            #endregion

            var fileName = "menuactions_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _menuActionService.ExportExcel(filterRules, sort, order);
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
