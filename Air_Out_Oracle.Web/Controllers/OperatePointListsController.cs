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
    public class OperatePointListsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<OperatePointList>, Repository<OperatePointList>>();
        //container.RegisterType<IOperatePointListService, OperatePointListService>();

        //private WebdbContext db = new WebdbContext();
        private readonly IOperatePointListService _operatePointListService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        //验证权限的名称
        private string ControllerQXName = "/OperatePointLists";//"/EMS_ORG_BOMs"

        public OperatePointListsController(IOperatePointListService operatePointListService, IUnitOfWorkAsync unitOfWork)
            : base(false, false)
        {
            _operatePointListService = operatePointListService;
            _unitOfWork = unitOfWork;
        }

        // GET: OperatePointLists/Index
        public ActionResult Index(int OperaPotId = 0, string OperaPotCode = "")
        {
            ViewBag.OperaPotId = OperaPotId;
            ViewBag.OperaPotCode = OperaPotCode;
            //var operatepointlist  = _operatePointListService.Queryable().Include(o => o.OperatePoint).AsQueryable();

            //return View(operatepointlist);
            return View();
        }

        // Get :OperatePointLists/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            int OperaPotId = ViewBag.OperaPotId == null ? 0 : Convert.ToInt32(ViewBag.OperaPotId);
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            if (filters != null)
            {
                var Wherefilter = filters.Where(x => x.field == "OperatePointID");
                if (Wherefilter.Any())
                {
                    if (string.IsNullOrEmpty(Wherefilter.FirstOrDefault().value))
                        Wherefilter.FirstOrDefault().value = OperaPotId.ToString();
                }
                else
                {
                    filterRule OfilterRile = new filterRule();
                    OfilterRile.field = "OperatePointID";
                    OfilterRile.op = "equals";
                    OfilterRile.value = OperaPotId.ToString();
                    (filters as List<filterRule>).Add(OfilterRile);
                }
            }
            else
            {
                List<filterRule> filter_s = new List<filterRule>();
                filterRule OfilterRile = new filterRule();
                OfilterRile.field = "OperatePointID";
                OfilterRile.op = "equals";
                OfilterRile.value = OperaPotId.ToString();
                filter_s.Add(OfilterRile);
                filters = filter_s;
            }
            int totalCount = 0;
            //int pagenum = offset / limit +1;

            var operatepointlist = _operatePointListService.Query(new OperatePointListQuery().Withfilter(filters)).
                Include(o => o.OperatePoint).
                OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);

            var datarows = operatepointlist.Select(n => new
            {
                OperatePointOperatePointCode = (n.OperatePoint == null ? "" : n.OperatePoint.OperatePointCode),
                ID = n.ID,
                OperatePointID = n.OperatePointID,
                OperatePointCode = n.OperatePointCode,
                CompanyCode = n.CompanyCode,
                CompanyName = n.CompanyName,
                IsEnabled = n.IsEnabled,
                ADDTS = n.ADDTS,
                ADDID = n.ADDID,
                ADDWHO = n.ADDWHO,
                EDITWHO = n.EDITWHO,
                EDITID = n.EDITID,
                EDITTS = n.EDITTS
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(OperatePointListChangeViewModel operatepointlist)
        {
            if (operatepointlist.updated != null)
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

                foreach (var updated in operatepointlist.updated)
                {
                    _operatePointListService.Update(updated);
                }
            }
            if (operatepointlist.deleted != null)
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

                foreach (var deleted in operatepointlist.deleted)
                {
                    _operatePointListService.Delete(deleted);
                }
            }
            if (operatepointlist.inserted != null)
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

                foreach (var inserted in operatepointlist.inserted)
                {
                    _operatePointListService.Insert(inserted);
                }
            }
            try
            {
                if ((operatepointlist.updated != null && operatepointlist.updated.Any()) ||
                (operatepointlist.deleted != null && operatepointlist.deleted.Any()) ||
                (operatepointlist.inserted != null && operatepointlist.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(operatepointlist);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOperatePoint()
        {
            var operatepointRepository = _unitOfWork.Repository<OperatePoint>();
            var data = operatepointRepository.Queryable().ToList();
            var rows = data.Select(n => new { ID = n.ID, OperatePointCode = n.OperatePointCode });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        // GET: OperatePointLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperatePointList operatePointList = _operatePointListService.Find(id);
            if (operatePointList == null)
            {
                return HttpNotFound();
            }
            return View(operatePointList);
        }

        // GET: OperatePointLists/Create
        public ActionResult Create()
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

            OperatePointList operatePointList = new OperatePointList();
            //set default value
            var operatepointRepository = _unitOfWork.Repository<OperatePoint>();
            ViewBag.OperatePointID = new SelectList(operatepointRepository.Queryable(), "ID", "OperatePointCode");
            return View(operatePointList);
        }

        // POST: OperatePointLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OperatePoint,ID,OperatePointID,OperatePointCode,WareHouseCode,WareHouseName,IsEnabled,ADDTS,ADDID,ADDWHO,EDITWHO,EDITID,EDITTS")] OperatePointList operatePointList)
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

            if (ModelState.IsValid)
            {
                _operatePointListService.Insert(operatePointList);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a OperatePointList record");
                return RedirectToAction("Index");
            }

            var operatepointRepository = _unitOfWork.Repository<OperatePoint>();
            ViewBag.OperatePointID = new SelectList(operatepointRepository.Queryable(), "ID", "OperatePointCode", operatePointList.OperatePointID);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(operatePointList);
        }

        // GET: OperatePointLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperatePointList operatePointList = _operatePointListService.Find(id);
            if (operatePointList == null)
            {
                return HttpNotFound();
            }
            var operatepointRepository = _unitOfWork.Repository<OperatePoint>();
            ViewBag.OperatePointID = new SelectList(operatepointRepository.Queryable(), "ID", "OperatePointCode", operatePointList.OperatePointID);
            return View(operatePointList);
        }

        // POST: OperatePointLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OperatePoint,ID,OperatePointID,OperatePointCode,WareHouseCode,WareHouseName,IsEnabled,ADDTS,ADDID,ADDWHO,EDITWHO,EDITID,EDITTS")] OperatePointList operatePointList)
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

            if (ModelState.IsValid)
            {
                operatePointList.ObjectState = ObjectState.Modified;
                _operatePointListService.Update(operatePointList);

                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a OperatePointList record");
                return RedirectToAction("Index");
            }
            var operatepointRepository = _unitOfWork.Repository<OperatePoint>();
            ViewBag.OperatePointID = new SelectList(operatepointRepository.Queryable(), "ID", "OperatePointCode", operatePointList.OperatePointID);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(operatePointList);
        }

        // GET: OperatePointLists/Delete/5
        public ActionResult Delete(int? id)
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperatePointList operatePointList = _operatePointListService.Find(id);
            if (operatePointList == null)
            {
                return HttpNotFound();
            }
            return View(operatePointList);
        }

        // POST: OperatePointLists/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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

            OperatePointList operatePointList = _operatePointListService.Find(id);
            _operatePointListService.Delete(operatePointList);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a OperatePointList record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "operatepointlist_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _operatePointListService.ExportExcel(filterRules, sort, order);
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

        /// <summary>
        /// 枚举数据字典
        /// </summary>
        /// <param name="DOCCODE"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetSYS_WAREHOUSE_CONFIG(string q = "")
        {
            int OperatingPointID = GetPID();
            q = q.ToUpper();

            var BD_DEFDOCQuery = _unitOfWork.Repository<OperatePointList>().Queryable().Where(p => p.OperatePointID == OperatingPointID);

            if (!string.IsNullOrEmpty(q))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.CompanyCode.Contains(q)).OrderBy(x => x.ID);
            }

            var ArrBD_DEFDOC = BD_DEFDOCQuery.Select(x => new
            {
                ID = x.CompanyCode,
                TEXT = x.CompanyCode,
            }).ToList();
            return Json(ArrBD_DEFDOC, JsonRequestBehavior.AllowGet);
        }

        private static int GetPID()
        {
            #region 操作点筛选 角色是超级管理员或用户名是admin 操作点是多个 不用筛选

            int OperatingPointID = 0;
            var CurrentUserOperatePointS = AirOut.Web.Controllers.Utility.CurrentUserOperatePoint;
            if (CurrentUserOperatePointS != null)
            {
                if (CurrentUserOperatePointS.Count == 1)
                {
                    // PointId = CurrentUserOperatePointS.FirstOrDefault().OperatePointLists.Where(x => x.IsEnabled == true).OrderBy(x => x.ID).FirstOrDefault().ID;
                    var PointId = CurrentUserOperatePointS.FirstOrDefault().ID;

                    if (PointId > 0)
                    {
                        OperatingPointID = PointId;
                    }
                }
            }

            #endregion

            return OperatingPointID;
        }
    }
}
