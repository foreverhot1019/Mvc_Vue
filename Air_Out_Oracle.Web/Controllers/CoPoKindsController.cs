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
    public class CoPoKindsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<CoPoKind>, Repository<CoPoKind>>();
        //container.RegisterType<ICoPoKindService, CoPoKindService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ICoPoKindService _coPoKindService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/CoPoKinds";

        public CoPoKindsController(ICoPoKindService coPoKindService, IUnitOfWorkAsync unitOfWork)
        {
            IsAutoResetCache = true;
            _coPoKindService = coPoKindService;
            _unitOfWork = unitOfWork;
        }

        // GET: CoPoKinds/Index
        public ActionResult Index()
        {
            //var copokind  = _coPoKindService.Queryable().AsQueryable();
            //return View(copokind  );
            return View();
        }

        // Get :CoPoKinds/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var copokind = _coPoKindService.Query(new CoPoKindQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = copokind.Select(n => new
            {
                ID = n.ID,
                Code = n.Code,
                Name = n.Name,
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
        public ActionResult SaveData(CoPoKindChangeViewModel copokind)
        {
            if (copokind.updated != null)
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

                foreach (var updated in copokind.updated)
                {
                    _coPoKindService.Update(updated);
                }
            }
            if (copokind.deleted != null)
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

                foreach (var deleted in copokind.deleted)
                {
                    _coPoKindService.Delete(deleted);
                }
            }
            if (copokind.inserted != null)
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

                foreach (var inserted in copokind.inserted)
                {
                    _coPoKindService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((copokind.updated != null && copokind.updated.Any()) ||
                (copokind.deleted != null && copokind.deleted.Any()) ||
                (copokind.inserted != null && copokind.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(copokind);
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

        // GET: CoPoKinds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoPoKind coPoKind = _coPoKindService.Find(id);
            if (coPoKind == null)
            {
                return HttpNotFound();
            }
            return View(coPoKind);
        }

        // GET: CoPoKinds/Create
        public ActionResult Create()
        {
            CoPoKind coPoKind = new CoPoKind();
            //set default value
            return View(coPoKind);
        }

        // POST: CoPoKinds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] CoPoKind coPoKind)
        {
            if (ModelState.IsValid)
            {
                _coPoKindService.Insert(coPoKind);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a CoPoKind record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(coPoKind);
        }

        // GET: CoPoKinds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoPoKind coPoKind = _coPoKindService.Find(id);
            if (coPoKind == null)
            {
                return HttpNotFound();
            }
            return View(coPoKind);
        }

        // POST: CoPoKinds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] CoPoKind coPoKind)
        {
            if (ModelState.IsValid)
            {
                coPoKind.ObjectState = ObjectState.Modified;
                _coPoKindService.Update(coPoKind);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a CoPoKind record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(coPoKind);
        }

        // GET: CoPoKinds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoPoKind coPoKind = _coPoKindService.Find(id);
            if (coPoKind == null)
            {
                return HttpNotFound();
            }
            return View(coPoKind);
        }

        // POST: CoPoKinds/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CoPoKind coPoKind = _coPoKindService.Find(id);
            _coPoKindService.Delete(coPoKind);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a CoPoKind record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "copokind_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _coPoKindService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取企业性质
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerCoPoKinds(int page = 1, int rows = 10, string q = "")
        {
            var cusBusInfoRep = _unitOfWork.RepositoryAsync<CoPoKind>();
            var data = cusBusInfoRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.Code.Contains(q) || n.Name.Contains(q));
            }
            var list = data.Select(n => new { ID = n.Code, TEXT = n.Name });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取企业性质(缓存)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerCoPoKinds_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var CoPoKindRep = (IEnumerable<CoPoKind>)CacheHelper.Get_SetCache(Common.CacheNameS.CoPoKind);
            var QCoPoKind = CoPoKindRep.Where(x => x.Code == null || x.Name == null);
            foreach (var item in QCoPoKind)
            {
                item.Code = item.Code ?? "";
                item.Name = item.Name ?? "";
            }
            if (q != "")
            {
                CoPoKindRep = CoPoKindRep.Where(n => n.Code.Contains(q) || n.Name.Contains(q));
            }
            var list = CoPoKindRep.Select(n => new { ID = n.Code, TEXT = n.Name });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
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
