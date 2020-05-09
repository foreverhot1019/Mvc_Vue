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
    public class Eai_GroupsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Eai_Group>, Repository<Eai_Group>>();
        //container.RegisterType<IEai_GroupService, Eai_GroupService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IEai_GroupService _eai_GroupService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/Eai_Groups";

        public Eai_GroupsController(IEai_GroupService eai_GroupService, IUnitOfWorkAsync unitOfWork)
        {
            _eai_GroupService = eai_GroupService;
            _unitOfWork = unitOfWork;
        }

        // GET: Eai_Groups/Index
        public ActionResult Index()
        {
            //var eai_group  = _eai_GroupService.Queryable().AsQueryable();
            //return View(eai_group  );
            return View();
        }

        // Get :Eai_Groups/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var eai_group = _eai_GroupService.Query(new Eai_GroupQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = eai_group.Select(n => new
            {
                Id = n.Id,
                Code = n.Code,
                Name = n.Name,
                Remark = n.Remark,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(Eai_GroupChangeViewModel eai_group)
        {
            if (eai_group.updated != null)
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

                foreach (var updated in eai_group.updated)
                {
                    _eai_GroupService.Update(updated);
                }
            }
            if (eai_group.deleted != null)
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

                foreach (var deleted in eai_group.deleted)
                {
                    _eai_GroupService.Delete(deleted);
                }
            }
            if (eai_group.inserted != null)
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

                foreach (var inserted in eai_group.inserted)
                {
                    _eai_GroupService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((eai_group.updated != null && eai_group.updated.Any()) ||
                (eai_group.deleted != null && eai_group.deleted.Any()) ||
                (eai_group.inserted != null && eai_group.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(eai_group);
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

        // GET: Eai_Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eai_Group eai_Group = _eai_GroupService.Find(id);
            if (eai_Group == null)
            {
                return HttpNotFound();
            }
            return View(eai_Group);
        }

        // GET: Eai_Groups/Create
        public ActionResult Create()
        {
            Eai_Group eai_Group = new Eai_Group();
            //set default value
            return View(eai_Group);
        }

        // POST: Eai_Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Remark,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Eai_Group eai_Group)
        {
            if (ModelState.IsValid)
            {
                _eai_GroupService.Insert(eai_Group);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Eai_Group record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(eai_Group);
        }

        // GET: Eai_Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eai_Group eai_Group = _eai_GroupService.Find(id);
            if (eai_Group == null)
            {
                return HttpNotFound();
            }
            return View(eai_Group);
        }

        // POST: Eai_Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Remark,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Eai_Group eai_Group)
        {
            if (ModelState.IsValid)
            {
                eai_Group.ObjectState = ObjectState.Modified;
                _eai_GroupService.Update(eai_Group);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Eai_Group record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(eai_Group);
        }

        // GET: Eai_Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eai_Group eai_Group = _eai_GroupService.Find(id);
            if (eai_Group == null)
            {
                return HttpNotFound();
            }
            return View(eai_Group);
        }

        // POST: Eai_Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Eai_Group eai_Group = _eai_GroupService.Find(id);
            _eai_GroupService.Delete(eai_Group);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Eai_Group record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "eai_group_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _eai_GroupService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取Eai_Group Json 数据集
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerEai_Group(int page = 1, int rows = 10, string q = "")
        {
            var Eai_GroupRep = _unitOfWork.RepositoryAsync<Eai_Group>();
            var data = Eai_GroupRep.Queryable();
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
        /// 缓存获取Eai_Group Json 数据集
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerEai_Group_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var Eai_GroupRep = (IEnumerable<Eai_Group>)CacheHelper.Get_SetCache(Common.CacheNameS.Eai_Group);
            var QEai_Group = Eai_GroupRep.Where(x => x.Code == null || x.Name == null);
            if (QEai_Group.Any())
            {
                foreach (var item in QEai_Group)
                {
                    item.Code = item.Code ?? "";
                    item.Name = item.Name ?? "";
                }
            }
            if (q != "")
            {
                Eai_GroupRep = Eai_GroupRep.Where(n => n.Code.Contains(q) || n.Name.Contains(q));
            }
            var list = Eai_GroupRep.Select(n => new { ID = n.Code, TEXT = n.Name });
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
