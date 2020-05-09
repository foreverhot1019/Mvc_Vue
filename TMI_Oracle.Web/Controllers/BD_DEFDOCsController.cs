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
    public class BD_DEFDOCsController : BaseController
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<BD_DEFDOC>, Repository<BD_DEFDOC>>();
        //container.RegisterType<IBD_DEFDOCService, BD_DEFDOCService>();

        //private WebdbContext db = new WebdbContext();

        private readonly IBD_DEFDOCService _bD_DEFDOCService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public BD_DEFDOCsController(IBD_DEFDOCService bD_DEFDOCService, IUnitOfWorkAsync unitOfWork)
        {
            _bD_DEFDOCService = bD_DEFDOCService;
            _unitOfWork = unitOfWork;
        }

        // GET: BD_DEFDOCs/Index
        public ActionResult Index()
        {
            //var bd_defdoc  = _bD_DEFDOCService.Queryable().AsQueryable();
            //return View(bd_defdoc  );
            return View();
        }

        // Get :BD_DEFDOCs/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var bd_defdoc = _bD_DEFDOCService.Query(new BD_DEFDOCQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = bd_defdoc.Select(n => new
            {
                ID = n.ID,
                TEXT=n.DOCNAME,
                DOCCODE = n.DOCCODE,
                DOCNAME = n.DOCNAME,
                REMARK = n.REMARK,
                STATUS = n.STATUS,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS,
                OperatingPoint = n.OperatingPoint
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(BD_DEFDOCChangeViewModel bd_defdoc)
        {
            if (bd_defdoc.updated != null)
            {
                #region

                var ControllActinMsg = "编辑";
                bool IsHaveQx = Common.ModelIsHaveQX("/BD_DEFDOCs", "Edit", ControllActinMsg);
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

                foreach (var updated in bd_defdoc.updated)
                {
                    _bD_DEFDOCService.Update(updated);
                }
            }
            if (bd_defdoc.deleted != null)
            {
                #region

                var ControllActinMsg = "删除";
                bool IsHaveQx = Common.ModelIsHaveQX("/BD_DEFDOCs", "Delete", ControllActinMsg);
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

                foreach (var deleted in bd_defdoc.deleted)
                {
                    _bD_DEFDOCService.Delete(deleted);
                }
            }
            if (bd_defdoc.inserted != null)
            {
                #region

                var ControllActinMsg = "创建";
                bool IsHaveQx = Common.ModelIsHaveQX("/BD_DEFDOCs", "Create", ControllActinMsg);
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

                foreach (var inserted in bd_defdoc.inserted)
                {
                    _bD_DEFDOCService.Insert(inserted);
                }
            }
            try
            {
                if ((bd_defdoc.updated != null && bd_defdoc.updated.Any()) ||
                    (bd_defdoc.deleted != null && bd_defdoc.deleted.Any()) ||
                    (bd_defdoc.inserted != null && bd_defdoc.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBD_DEFDOC()
        {
            var bd_defdocRepository = _unitOfWork.Repository<BD_DEFDOC>();
            var data = bd_defdocRepository.Queryable().ToList();
            var rows = data.Select(n => new
            {
                ID = n.ID,
                DOCCODE = n.DOCCODE
            });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        // GET: BD_DEFDOCs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_DEFDOC bD_DEFDOC = _bD_DEFDOCService.Find(id);
            if (bD_DEFDOC == null)
            {
                return HttpNotFound();
            }
            return View(bD_DEFDOC);
        }


        // GET: BD_DEFDOCs/Create
        public ActionResult Create()
        {
            BD_DEFDOC bD_DEFDOC = new BD_DEFDOC();

            #region

            var ControllActinMsg = "创建";
            bool IsHaveQx = Common.ModelIsHaveQX("/BD_DEFDOCs", "Create", ControllActinMsg);
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

            //set default value
            return View(bD_DEFDOC);
        }

        // POST: BD_DEFDOCs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BD_DEFDOC_LIST,ID,DOCCODE,DOCNAME,REMARK,STATUS,ADDWHO,ADDTS,EDITWHO,EDITTS")] BD_DEFDOC bD_DEFDOC)
        {
            #region

            var ControllActinMsg = "创建";
            bool IsHaveQx = Common.ModelIsHaveQX("/BD_DEFDOCs", "Create", ControllActinMsg);
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
                bD_DEFDOC.ObjectState = ObjectState.Added;
                foreach (var item in bD_DEFDOC.BD_DEFDOC_LIST)
                {
                    item.DOCID = bD_DEFDOC.ID;
                    item.ObjectState = ObjectState.Added;
                }
                _bD_DEFDOCService.InsertOrUpdateGraph(bD_DEFDOC);

                try
                {
                    _unitOfWork.SaveChanges();
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }

                DisplaySuccessMessage("Has append a BD_DEFDOC record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bD_DEFDOC);
        }

        // GET: BD_DEFDOCs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_DEFDOC bD_DEFDOC = _bD_DEFDOCService.Find(id);
            if (bD_DEFDOC == null)
            {
                return HttpNotFound();
            }
            return View(bD_DEFDOC);
        }

        // POST: BD_DEFDOCs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BD_DEFDOC_LIST,ID,DOCCODE,DOCNAME,REMARK,ADDWHO,STATUS,ADDTS,EDITWHO,EDITTS")] BD_DEFDOC bD_DEFDOC)
        {
            #region

            var ControllActinMsg = "编辑";
            bool IsHaveQx = Common.ModelIsHaveQX("/BD_AGENCIESs", "Edit", ControllActinMsg);
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
                bD_DEFDOC.ObjectState = ObjectState.Modified;
                foreach (var item in bD_DEFDOC.BD_DEFDOC_LIST)
                {
                    item.DOCID = bD_DEFDOC.ID;
                    //set ObjectState with conditions
                    if (item.ID <= 0)
                        item.ObjectState = ObjectState.Added;
                    else
                        item.ObjectState = ObjectState.Modified;
                }

                _bD_DEFDOCService.InsertOrUpdateGraph(bD_DEFDOC);

                try
                {
                    _unitOfWork.SaveChanges();
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }

                DisplaySuccessMessage("Has update a BD_DEFDOC record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bD_DEFDOC);
        }

        // GET: BD_DEFDOCs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_DEFDOC bD_DEFDOC = _bD_DEFDOCService.Find(id);
            if (bD_DEFDOC == null)
            {
                return HttpNotFound();
            }
            return View(bD_DEFDOC);
        }

        // POST: BD_DEFDOCs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            #region

            var ControllActinMsg = "删除";
            bool IsHaveQx = Common.ModelIsHaveQX("/BD_DEFDOCs", "Delete", ControllActinMsg);
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

            BD_DEFDOC bD_DEFDOC = _bD_DEFDOCService.Find(id);
            _bD_DEFDOCService.Delete(bD_DEFDOC);

            try
            {
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            DisplaySuccessMessage("Has delete a BD_DEFDOC record");
            return RedirectToAction("Index");
        }

        // Get Detail Row By Id For Edit
        // Get : BD_DEFDOCs/EditBD_DEFDOC_LIST/:id
        [HttpGet]
        public ActionResult EditBD_DEFDOC_LIST(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bd_defdoc_listRepository = _unitOfWork.Repository<BD_DEFDOC_LIST>();
            var bd_defdoc_list = bd_defdoc_listRepository.Find(id);

            var bd_defdocRepository = _unitOfWork.Repository<BD_DEFDOC>();

            if (bd_defdoc_list == null)
            {
                ViewBag.DOCID = new SelectList(bd_defdocRepository.Queryable(), "ID", "DOCCODE");

                //return HttpNotFound();
                return PartialView("_BD_DEFDOC_LISTEditForm", new BD_DEFDOC_LIST());
            }
            else
            {
                ViewBag.DOCID = new SelectList(bd_defdocRepository.Queryable(), "ID", "DOCCODE", bd_defdoc_list.DOCID);

            }
            return PartialView("_BD_DEFDOC_LISTEditForm", bd_defdoc_list);
        }

        // Get Create Row By Id For Edit
        // Get : BD_DEFDOCs/CreateBD_DEFDOC_LIST
        [HttpGet]
        public ActionResult CreateBD_DEFDOC_LIST()
        {
            var bd_defdocRepository = _unitOfWork.Repository<BD_DEFDOC>();
            ViewBag.DOCID = new SelectList(bd_defdocRepository.Queryable(), "ID", "DOCCODE");
            return PartialView("_BD_DEFDOC_LISTEditForm");
        }

        // Post Delete Detail Row By Id
        // Get : BD_DEFDOCs/DeleteBD_DEFDOC_LIST/:id
        [HttpPost, ActionName("DeleteBD_DEFDOC_LIST")]
        public ActionResult DeleteBD_DEFDOC_LISTConfirmed(int id)
        {
            #region

            var ControllActinMsg = "删除";
            bool IsHaveQx = Common.ModelIsHaveQX("/BD_AGENCIESs", "Delete", ControllActinMsg);
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

            var bd_defdoc_listRepository = _unitOfWork.Repository<BD_DEFDOC_LIST>();
            bd_defdoc_listRepository.Delete(id);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Order record");
            return RedirectToAction("Index");
        }

        // Get : BD_DEFDOCs/GetBD_DEFDOC_LISTByDOCID/:id
        [HttpGet]
        public ActionResult GetBD_DEFDOC_LISTByDOCID(int id)
        {
            var bd_defdoc_list = _bD_DEFDOCService.GetBD_DEFDOC_LISTByDOCID(id);
            if (Request.IsAjaxRequest())
            {
                return Json(bd_defdoc_list.Select(n => new
                {
                    BD_DEFDOCDOCCODE = (n.BD_DEFDOC == null ? "" : n.BD_DEFDOC.DOCCODE),
                    ID = n.ID,
                    DOCID = n.DOCID,
                    LISTCODE = n.LISTCODE,
                    LISTNAME = n.LISTNAME,
                    ListFullName = n.ListFullName,
                    REMARK = n.REMARK,
                    STATUS = n.STATUS,
                    ADDWHO = n.ADDWHO,
                    ADDTS = n.ADDTS,
                    EDITWHO = n.EDITWHO,
                    EDITTS = n.EDITTS,
                    R_CODE = n.R_CODE,
                    ENAME = n.ENAME,
                    OperatingPoint = n.OperatingPoint
                }), JsonRequestBehavior.AllowGet);
            }
            return View(bd_defdoc_list);
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            #region

            var ControllActinMsg = "导出";
            bool IsHaveQx = Common.ModelIsHaveQX("/BD_DEFDOCs", "Export", ControllActinMsg);
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

            var fileName = "bd_defdoc_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _bD_DEFDOCService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取账册项号
        /// </summary>
        /// <param name="EMS_NO"></param>
        /// <returns></returns>
        public ActionResult GetBD_DEFDOCListSByDOCCODE(string DOCCODE = "", string q = "")
        {
            if (DOCCODE.ToLower() == "null")
                DOCCODE = "";
            var BD_DEFDOC_LISTQuery = _unitOfWork.Repository<BD_DEFDOC_LIST>().Queryable();
            if (!string.IsNullOrEmpty(DOCCODE))
            {
                BD_DEFDOC_LISTQuery = BD_DEFDOC_LISTQuery.Where(x => x.BD_DEFDOC.DOCCODE == DOCCODE);
            }
            if (!string.IsNullOrEmpty(q))
            {
                decimal dcml = 0;
                if (decimal.TryParse(q, out dcml))
                {
                    BD_DEFDOC_LISTQuery = BD_DEFDOC_LISTQuery.Where(x => x.LISTCODE == DOCCODE || x.LISTNAME.Contains(q));
                }
                else
                {
                    BD_DEFDOC_LISTQuery = BD_DEFDOC_LISTQuery.Where(x => x.LISTCODE.Contains(q) || x.LISTNAME.Contains(q));
                }
            }
            var ArrBD_DEFDOCList = BD_DEFDOC_LISTQuery.Select(x => new
            {
                x.ID,
                x.LISTCODE,
                x.LISTNAME,
                x.DOCID
            }).OrderBy(x => x.ID).ThenBy(x => x.LISTCODE).ToList();
            return Json(ArrBD_DEFDOCList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取账册项号
        /// </summary>
        /// <param name="EMS_NO"></param>
        /// <returns></returns>
        public ActionResult GetPagerBD_DEFDOCListSByDOCCODE(int page = 1, int rows = 10, string DOCCODE = "", string q = "")
        {
            if (DOCCODE.ToLower() == "null")
                DOCCODE = "";
            var BD_DEFDOC_LISTQuery = _unitOfWork.Repository<BD_DEFDOC_LIST>().Queryable();
            if (!string.IsNullOrEmpty(DOCCODE))
            {
                BD_DEFDOC_LISTQuery = BD_DEFDOC_LISTQuery.Where(x => x.BD_DEFDOC.DOCCODE == DOCCODE);
            }
            int totalCount = 0;
            if (!string.IsNullOrEmpty(q))
            {
                decimal dcml = 0;
                if (decimal.TryParse(q, out dcml))
                {
                    BD_DEFDOC_LISTQuery = BD_DEFDOC_LISTQuery.Where(x => x.LISTCODE == DOCCODE || x.LISTNAME.Contains(q));
                }
                else
                {
                    BD_DEFDOC_LISTQuery = BD_DEFDOC_LISTQuery.Where(x => x.LISTCODE.Contains(q) || x.LISTNAME.Contains(q));
                }
            }
            totalCount = BD_DEFDOC_LISTQuery.Count();
            var ArrBD_DEFDOCList = BD_DEFDOC_LISTQuery.Select(x => new
            {
                x.ID,
                x.LISTCODE,
                x.LISTNAME,
                x.DOCID
            }).OrderBy(x => x.ID).ThenBy(x => x.LISTCODE).Skip(rows * (page - 1)).OrderBy(x => x.ID).Take(rows).ToList();

            return Json(new { total = totalCount, rows = ArrBD_DEFDOCList }, JsonRequestBehavior.AllowGet);
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
