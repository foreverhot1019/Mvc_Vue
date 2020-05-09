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
    public class TradeTypesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<TradeType>, Repository<TradeType>>();
        //container.RegisterType<ITradeTypeService, TradeTypeService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ITradeTypeService _tradeTypeService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/TradeTypes";

        public TradeTypesController(ITradeTypeService tradeTypeService, IUnitOfWorkAsync unitOfWork)
        {
            IsAutoResetCache = true;
            _tradeTypeService = tradeTypeService;
            _unitOfWork = unitOfWork;
        }

        // GET: TradeTypes/Index
        public ActionResult Index()
        {
            //var tradetype  = _tradeTypeService.Queryable().AsQueryable();
            //return View(tradetype  );
            return View();
        }

        // Get :TradeTypes/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var tradetype = _tradeTypeService.Query(new TradeTypeQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = tradetype.Select(n => new
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
        public ActionResult SaveData(TradeTypeChangeViewModel tradetype)
        {
            if (tradetype.updated != null)
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

                foreach (var updated in tradetype.updated)
                {
                    _tradeTypeService.Update(updated);
                }
            }
            if (tradetype.deleted != null)
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

                foreach (var deleted in tradetype.deleted)
                {
                    _tradeTypeService.Delete(deleted);
                }
            }
            if (tradetype.inserted != null)
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

                foreach (var inserted in tradetype.inserted)
                {
                    _tradeTypeService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((tradetype.updated != null && tradetype.updated.Any()) ||
                (tradetype.deleted != null && tradetype.deleted.Any()) ||
                (tradetype.inserted != null && tradetype.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(tradetype);
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

        // GET: TradeTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TradeType tradeType = _tradeTypeService.Find(id);
            if (tradeType == null)
            {
                return HttpNotFound();
            }
            return View(tradeType);
        }

        // GET: TradeTypes/Create
        public ActionResult Create()
        {
            TradeType tradeType = new TradeType();
            //set default value
            return View(tradeType);
        }

        // POST: TradeTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] TradeType tradeType)
        {
            if (ModelState.IsValid)
            {
                _tradeTypeService.Insert(tradeType);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a TradeType record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(tradeType);
        }

        // GET: TradeTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TradeType tradeType = _tradeTypeService.Find(id);
            if (tradeType == null)
            {
                return HttpNotFound();
            }
            return View(tradeType);
        }

        // POST: TradeTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] TradeType tradeType)
        {
            if (ModelState.IsValid)
            {
                tradeType.ObjectState = ObjectState.Modified;
                _tradeTypeService.Update(tradeType);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a TradeType record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(tradeType);
        }

        // GET: TradeTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TradeType tradeType = _tradeTypeService.Find(id);
            if (tradeType == null)
            {
                return HttpNotFound();
            }
            return View(tradeType);
        }

        // POST: TradeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TradeType tradeType = _tradeTypeService.Find(id);
            _tradeTypeService.Delete(tradeType);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a TradeType record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "tradetype_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _tradeTypeService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取 行业类型
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerTradeTypes(int page = 1, int rows = 10, string q = "")
        {
            var TradeTypeRep = _unitOfWork.RepositoryAsync<TradeType>();
            var data = TradeTypeRep.Queryable();
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
        /// 获取 行业类型(缓存)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerTradeTypes_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var TradeTypeRep = (IEnumerable<TradeType>)CacheHelper.Get_SetCache(Common.CacheNameS.TradeType);
            var QTradeTypeRep = TradeTypeRep.Where(x => x.Code == null || x.Name == null);
            if (QTradeTypeRep.Any())
            {
                foreach (var item in QTradeTypeRep)
                {
                    item.Code = item.Code ?? "";
                    item.Name = item.Name ?? "";
                }
            }
            if (q != "")
            {
                TradeTypeRep = TradeTypeRep.Where(n => n.Code.Contains(q) || n.Name.Contains(q));
            }
            var list = TradeTypeRep.Select(n => new { ID = n.Code, TEXT = n.Name });
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
