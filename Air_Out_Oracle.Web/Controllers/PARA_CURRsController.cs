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
    public class PARA_CURRsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<PARA_CURR>, Repository<PARA_CURR>>();
        //container.RegisterType<IPARA_CURRService, PARA_CURRService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPARA_CURRService _pARA_CURRService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/PARA_CURRs";

        public PARA_CURRsController(IPARA_CURRService pARA_CURRService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _pARA_CURRService = pARA_CURRService;
            _unitOfWork = unitOfWork;
            IsAutoResetCache = true;
        }

        // GET: PARA_CURRs/Index
        public ActionResult Index()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            //var para_curr  = _pARA_CURRService.Queryable().AsQueryable();
            //return View(para_curr  );
            return View();
        }

        // Get :PARA_CURRs/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var para_curr = _pARA_CURRService.Query(new PARA_CURRQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = para_curr.Select(n => new
            {
                ID = n.ID,
                CURR_CODE = n.CURR_CODE,
                CURR_Name = n.CURR_Name,
                CURR_NameEng = n.CURR_NameEng,
                Money_CODE = n.Money_CODE,
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
        public ActionResult SaveData(PARA_CURRChangeViewModel para_curr)
        {
            if (para_curr.updated != null)
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

                foreach (var updated in para_curr.updated)
                {
                    _pARA_CURRService.Update(updated);
                }
            }
            if (para_curr.deleted != null)
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

                foreach (var deleted in para_curr.deleted)
                {
                    _pARA_CURRService.Delete(deleted);
                }
            }
            if (para_curr.inserted != null)
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

                foreach (var inserted in para_curr.inserted)
                {
                    _pARA_CURRService.Insert(inserted);
                }
            }
            ////如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            //string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            //string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((para_curr.updated != null && para_curr.updated.Any()) ||
                (para_curr.deleted != null && para_curr.deleted.Any()) ||
                (para_curr.inserted != null && para_curr.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(para_curr);
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

        // GET: PARA_CURRs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_CURR pARA_CURR = _pARA_CURRService.Find(id);
            if (pARA_CURR == null)
            {
                return HttpNotFound();
            }
            return View(pARA_CURR);
        }

        // GET: PARA_CURRs/Create
        public ActionResult Create()
        {
            PARA_CURR pARA_CURR = new PARA_CURR();
            //set default value
            return View(pARA_CURR);
        }

        // POST: PARA_CURRs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CURR_CODE,CURR_Name,CURR_NameEng,Money_CODE,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_CURR pARA_CURR)
        {
            if (ModelState.IsValid)
            {
                _pARA_CURRService.Insert(pARA_CURR);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a PARA_CURR record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_CURR);
        }

        // GET: PARA_CURRs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_CURR pARA_CURR = _pARA_CURRService.Find(id);
            if (pARA_CURR == null)
            {
                return HttpNotFound();
            }
            return View(pARA_CURR);
        }

        // POST: PARA_CURRs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CURR_CODE,CURR_Name,CURR_NameEng,Money_CODE,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_CURR pARA_CURR)
        {
            if (ModelState.IsValid)
            {
                pARA_CURR.ObjectState = ObjectState.Modified;
                _pARA_CURRService.Update(pARA_CURR);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a PARA_CURR record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_CURR);
        }

        // GET: PARA_CURRs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_CURR pARA_CURR = _pARA_CURRService.Find(id);
            if (pARA_CURR == null)
            {
                return HttpNotFound();
            }
            return View(pARA_CURR);
        }

        // POST: PARA_CURRs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PARA_CURR pARA_CURR = _pARA_CURRService.Find(id);
            _pARA_CURRService.Delete(pARA_CURR);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a PARA_CURR record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "para_curr_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _pARA_CURRService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取币制信息
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_CURR(int page = 1, int rows = 10, string q = "")
        {
            var para_CURRRep = _unitOfWork.RepositoryAsync<PARA_CURR>().Queryable();
            if (q != "")
            {
                para_CURRRep = para_CURRRep.Where(n => n.CURR_CODE.Contains(q) || n.CURR_Name.Contains(q));
            }
            var list = para_CURRRep.Select(n => new { ID = n.CURR_CODE, TEXT = n.CURR_Name });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取币制信息
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_CURR_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var para_CURRRep = (IEnumerable<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            var Qpara_CURR = para_CURRRep.Where(x => x.CURR_CODE == null || x.CURR_Name == null);
            if (Qpara_CURR.Any())
            {
                foreach (var item in Qpara_CURR)
                {
                    item.CURR_CODE = item.CURR_CODE ?? "";
                    item.CURR_Name = item.CURR_Name ?? "";
                }
            }
            
            if (q != "")
            {
                para_CURRRep = para_CURRRep.Where(n => n.CURR_CODE.Contains(q) || n.CURR_Name.Contains(q));
            }
            var list = para_CURRRep.Select(n => new { ID = n.CURR_CODE, TEXT = n.CURR_Name });
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
