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
    public class PortalEntryIDLinksController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<PortalEntryIDLink>, Repository<PortalEntryIDLink>>();
        //container.RegisterType<IPortalEntryIDLinkService, PortalEntryIDLinkService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPortalEntryIDLinkService  _portalEntryIDLinkService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/PortalEntryIDLinksController";

        public PortalEntryIDLinksController (IPortalEntryIDLinkService  portalEntryIDLinkService, IUnitOfWorkAsync unitOfWork)
        {
            _portalEntryIDLinkService  = portalEntryIDLinkService;
            _unitOfWork = unitOfWork;
        }

        // GET: PortalEntryIDLinks/Index
        public ActionResult Index()
        {
            //var portalentryidlink  = _portalEntryIDLinkService.Queryable().AsQueryable();
            //return View(portalentryidlink  );
			return View();
        }

        // Get :PortalEntryIDLinks/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
			var portalentryidlink = _portalEntryIDLinkService.Query(new PortalEntryIDLinkQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            var datarows = portalentryidlink.Select( n => new {  
ID = n.ID, 
UserId = n.UserId, 
DepartMent = n.DepartMent, 
EntryID = n.EntryID}).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(PortalEntryIDLinkChangeViewModel portalentryidlink)
        {
            if (portalentryidlink.updated != null)
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

                foreach (var updated in portalentryidlink.updated)
                {
                    _portalEntryIDLinkService.Update(updated);
                }
            }
            if (portalentryidlink.deleted != null)
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

                foreach (var deleted in portalentryidlink.deleted)
                {
                    _portalEntryIDLinkService.Delete(deleted);
                }
            }
            if (portalentryidlink.inserted != null)
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

                foreach (var inserted in portalentryidlink.inserted)
                {
                    _portalEntryIDLinkService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((portalentryidlink.updated != null && portalentryidlink.updated.Any()) || 
				(portalentryidlink.deleted != null && portalentryidlink.deleted.Any()) || 
				(portalentryidlink.inserted != null && portalentryidlink.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(portalentryidlink);
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
		       
        // GET: PortalEntryIDLinks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PortalEntryIDLink portalEntryIDLink = _portalEntryIDLinkService.Find(id);
            if (portalEntryIDLink == null)
            {
                return HttpNotFound();
            }
            return View(portalEntryIDLink);
        }

        // GET: PortalEntryIDLinks/Create
        public ActionResult Create()
        {
            PortalEntryIDLink portalEntryIDLink = new PortalEntryIDLink();
            //set default value
            return View(portalEntryIDLink);
        }

        // POST: PortalEntryIDLinks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserId,DepartMent,EntryID")] PortalEntryIDLink portalEntryIDLink)
        {
            if (ModelState.IsValid)
            {
				_portalEntryIDLinkService.Insert(portalEntryIDLink);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a PortalEntryIDLink record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(portalEntryIDLink);
        }

        // GET: PortalEntryIDLinks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PortalEntryIDLink portalEntryIDLink = _portalEntryIDLinkService.Find(id);
            if (portalEntryIDLink == null)
            {
                return HttpNotFound();
            }
            return View(portalEntryIDLink);
        }

        // POST: PortalEntryIDLinks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserId,DepartMent,EntryID")] PortalEntryIDLink portalEntryIDLink)
        {
            if (ModelState.IsValid)
            {
				portalEntryIDLink.ObjectState = ObjectState.Modified;
				_portalEntryIDLinkService.Update(portalEntryIDLink);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a PortalEntryIDLink record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(portalEntryIDLink);
        }

        // GET: PortalEntryIDLinks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PortalEntryIDLink portalEntryIDLink = _portalEntryIDLinkService.Find(id);
            if (portalEntryIDLink == null)
            {
                return HttpNotFound();
            }
            return View(portalEntryIDLink);
        }

        // POST: PortalEntryIDLinks/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PortalEntryIDLink portalEntryIDLink = _portalEntryIDLinkService.Find(id);
            _portalEntryIDLinkService.Delete(portalEntryIDLink);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a PortalEntryIDLink record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "portalentryidlink_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _portalEntryIDLinkService.ExportExcel(filterRules,sort, order );
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
