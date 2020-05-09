﻿using System;
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
    public class MenuItemsController : Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<MenuItem>, Repository<MenuItem>>();
        //container.RegisterType<IMenuItemService, MenuItemService>();

        //private StoreContext db = new StoreContext();
        private readonly IMenuItemService _menuItemService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public MenuItemsController(IMenuItemService menuItemService, IUnitOfWorkAsync unitOfWork)
        {
            _menuItemService = menuItemService;
            _unitOfWork = unitOfWork;
        }

        // GET: MenuItems/Index
        public ActionResult Index()
        {
            //var menuitems  = _menuItemService.Queryable().Include(m => m.Parent).AsQueryable();
            //return View(menuitems);
            return View();
        }

        // Get :MenuItems/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;

            var menuitems = _menuItemService.Query(new MenuItemQuery().Withfilter(filters)).Include(m => m.Parent).
                OrderBy(n => n.OrderBy(sort, order)).
                SelectPage(page, rows, out totalCount);

            var datarows = menuitems.Select(n => new
            {
                ParentTitle = (n.Parent == null ? "" : n.Parent.Title),
                Id = n.Id,
                Title = n.Title,
                Action = n.Action,
                Controller = n.Controller,
                Description = n.Description,
                Code = n.Code,
                Url = n.Url,
                IconCls = n.IconCls,
                IsEnabled = n.IsEnabled,
                ParentId = n.ParentId
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(MenuItemChangeViewModel menuitems)
        {
            if (menuitems.updated != null)
            {
                foreach (var updated in menuitems.updated)
                {
                    _menuItemService.Update(updated);
                }
            }
            if (menuitems.deleted != null)
            {
                foreach (var deleted in menuitems.deleted)
                {
                    _menuItemService.Delete(deleted);
                }
            }
            if (menuitems.inserted != null)
            {
                foreach (var inserted in menuitems.inserted)
                {
                    _menuItemService.Insert(inserted);
                }
            }
            _unitOfWork.SaveChanges();

            if ((menuitems.updated != null && menuitems.updated.Any()) || 
                (menuitems.deleted != null && menuitems.deleted.Any()) || 
                (menuitems.inserted != null && menuitems.inserted.Any()))
            {
                #region 更新菜单缓存

                //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                lock (Common.lockCacheMenuItem)
                {
                    List<MenuItem> ArrMenuItem = _unitOfWork.Repository<MenuItem>().Query().Select().OrderBy(x => x.Code).ToList();
                    if (HttpContext.Cache[Common.GeCacheEnumByName("MenuItem").ToString()] != null)
                        HttpContext.Cache.Remove(Common.GeCacheEnumByName("MenuItem").ToString());
                    HttpContext.Cache.Insert(Common.GeCacheEnumByName("MenuItem").ToString(), ArrMenuItem, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                #endregion
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMenuItems()
        {
            var menuitemRepository = _unitOfWork.Repository<MenuItem>();
            var data = menuitemRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, Title = n.Title });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateWithController()
        {
            _menuItemService.CreateWithController();
            _unitOfWork.SaveChanges();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReBuildMenus()
        {
            _menuItemService.CreateWithController();
            _unitOfWork.SaveChanges();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        // GET: MenuItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItem menuItem = _menuItemService.Find(id);
            if (menuItem == null)
            {
                return HttpNotFound();
            }
            return View(menuItem);
        }

        // GET: MenuItems/Create
        public ActionResult Create()
        {
            MenuItem menuItem = new MenuItem();
            //set default value
            var menuitemRepository = _unitOfWork.Repository<MenuItem>();
            ViewBag.ParentId = new SelectList(menuitemRepository.Queryable(), "Id", "Title");
            return View(menuItem);
        }

        // POST: MenuItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Parent,SubMenus,Id,Title,Description,Code,Url,IconCls,IsEnabled,ParentId,Action,Controller")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                _menuItemService.Insert(menuItem);
                _unitOfWork.SaveChanges();

                #region 更新菜单缓存

                //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                lock (Common.lockCacheMenuItem)
                {
                    List<MenuItem> ArrMenuItem = _unitOfWork.Repository<MenuItem>().Query().Select().OrderBy(x => x.Code).ToList();
                    if (HttpContext.Cache[Common.GeCacheEnumByName("MenuItem").ToString()] != null)
                        HttpContext.Cache.Remove(Common.GeCacheEnumByName("MenuItem").ToString());
                    HttpContext.Cache.Insert(Common.GeCacheEnumByName("MenuItem").ToString(), ArrMenuItem, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                #endregion

                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a MenuItem record");
                return RedirectToAction("Index");
            }

            var menuitemRepository = _unitOfWork.Repository<MenuItem>();
            ViewBag.ParentId = new SelectList(menuitemRepository.Queryable(), "Id", "Title", menuItem.ParentId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(menuItem);
        }

        // GET: MenuItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItem menuItem = _menuItemService.Find(id);
            if (menuItem == null)
            {
                return HttpNotFound();
            }
            var menuitemRepository = _unitOfWork.Repository<MenuItem>();
            ViewBag.ParentId = new SelectList(menuitemRepository.Queryable(), "Id", "Title", menuItem.ParentId);
            return View(menuItem);
        }

        // POST: MenuItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Parent,SubMenus,Id,Title,Description,Code,Url,IconCls,IsEnabled,ParentId,Action,Controller")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                menuItem.ObjectState = ObjectState.Modified;
                _menuItemService.Update(menuItem);

                _unitOfWork.SaveChanges();

                #region 更新菜单缓存

                //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                lock (Common.lockCacheMenuItem)
                {
                    List<MenuItem> ArrMenuItem = _unitOfWork.Repository<MenuItem>().Query().Select().OrderBy(x => x.Code).ToList();
                    if (HttpContext.Cache[Common.GeCacheEnumByName("MenuItem").ToString()] != null)
                        HttpContext.Cache.Remove(Common.GeCacheEnumByName("MenuItem").ToString());
                    HttpContext.Cache.Insert(Common.GeCacheEnumByName("MenuItem").ToString(), ArrMenuItem, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                #endregion

                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a MenuItem record");
                return RedirectToAction("Index");
            }
            var menuitemRepository = _unitOfWork.Repository<MenuItem>();
            ViewBag.ParentId = new SelectList(menuitemRepository.Queryable(), "Id", "Title", menuItem.ParentId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(menuItem);
        }

        // GET: MenuItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItem menuItem = _menuItemService.Find(id);
            if (menuItem == null)
            {
                return HttpNotFound();
            }
            return View(menuItem);
        }

        // POST: MenuItems/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuItem menuItem = _menuItemService.Find(id);
            _menuItemService.Delete(menuItem);
            _unitOfWork.SaveChanges();

            #region 更新菜单缓存

            //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
            lock (Common.lockCacheMenuItem)
            {
                List<MenuItem> ArrMenuItem = _unitOfWork.Repository<MenuItem>().Query().Select().OrderBy(x => x.Code).ToList();
                if (HttpContext.Cache[Common.GeCacheEnumByName("MenuItem").ToString()] != null)
                    HttpContext.Cache.Remove(Common.GeCacheEnumByName("MenuItem").ToString());
                HttpContext.Cache.Insert(Common.GeCacheEnumByName("MenuItem").ToString(), ArrMenuItem, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            #endregion

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a MenuItem record");
            return RedirectToAction("Index");
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
