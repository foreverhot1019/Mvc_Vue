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
    public class DealArticlesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<DealArticle>, Repository<DealArticle>>();
        //container.RegisterType<IDealArticleService, DealArticleService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IDealArticleService _dealArticleService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/DealArticles";

        public DealArticlesController(IDealArticleService dealArticleService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _dealArticleService = dealArticleService;
            _unitOfWork = unitOfWork;
        }

        // GET: DealArticles/Index
        public ActionResult Index()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            //var dealarticle  = _dealArticleService.Queryable().AsQueryable();
            //return View(dealarticle  );
            return View();
        }

        // Get :DealArticles/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var dealarticle = _dealArticleService.Query(new DealArticleQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = dealarticle.Select(n => new
            {
                Id = n.Id,
                DealArticleCode = n.DealArticleCode,
                DealArticleName = n.DealArticleName,
                TransArticle = n.TransArticle,
                Description = n.Description,
                Pay_ModeCode = n.Pay_ModeCode,
                Carriage = n.Carriage,
                CarriageEns = n.CarriageEns,
                Incidental_Expenses = n.Incidental_Expenses,
                Incidental_ExpensesEns = n.Incidental_ExpensesEns,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(DealArticleChangeViewModel dealarticle)
        {
            if (dealarticle.updated != null)
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

                foreach (var updated in dealarticle.updated)
                {
                    _dealArticleService.Update(updated);
                }
            }
            if (dealarticle.deleted != null)
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

                foreach (var deleted in dealarticle.deleted)
                {
                    _dealArticleService.Delete(deleted);
                }
            }
            if (dealarticle.inserted != null)
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

                foreach (var inserted in dealarticle.inserted)
                {
                    _dealArticleService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((dealarticle.updated != null && dealarticle.updated.Any()) ||
                (dealarticle.deleted != null && dealarticle.deleted.Any()) ||
                (dealarticle.inserted != null && dealarticle.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(dealarticle);
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

        // GET: DealArticles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DealArticle dealArticle = _dealArticleService.Find(id);
            if (dealArticle == null)
            {
                return HttpNotFound();
            }
            return View(dealArticle);
        }

        // GET: DealArticles/Create
        public ActionResult Create()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            DealArticle dealArticle = new DealArticle();
            //set default value
            return View(dealArticle);
        }

        // POST: DealArticles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DealArticleCode,DealArticleName,TransArticle,Description,Status,OperatingPoint,ADDWHO,ADDTS,EDITWHO,EDITTS")] DealArticle dealArticle)
        {
            if (ModelState.IsValid)
            {
                _dealArticleService.Insert(dealArticle);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a DealArticle record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(dealArticle);
        }

        // GET: DealArticles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DealArticle dealArticle = _dealArticleService.Find(id);
            if (dealArticle == null)
            {
                return HttpNotFound();
            }
            return View(dealArticle);
        }

        // POST: DealArticles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DealArticleCode,DealArticleName,TransArticle,Description,Status,OperatingPoint,ADDWHO,ADDTS,EDITWHO,EDITTS")] DealArticle dealArticle)
        {
            if (ModelState.IsValid)
            {
                dealArticle.ObjectState = ObjectState.Modified;
                _dealArticleService.Update(dealArticle);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a DealArticle record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(dealArticle);
        }

        // GET: DealArticles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DealArticle dealArticle = _dealArticleService.Find(id);
            if (dealArticle == null)
            {
                return HttpNotFound();
            }
            return View(dealArticle);
        }

        // POST: DealArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DealArticle dealArticle = _dealArticleService.Find(id);
            _dealArticleService.Delete(dealArticle);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a DealArticle record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "dealarticle_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _dealArticleService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取成交条款代码和名称
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerDealArticle(int page = 1, int rows = 10, string q = "")
        {
            var dealArticleRep = _unitOfWork.RepositoryAsync<DealArticle>();
            var data = dealArticleRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.DealArticleCode.Contains(q) || n.DealArticleName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.DealArticleCode, TEXT = n.DealArticleName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取成交条款代码和名称
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerDealArticle_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var DealArticleRep = (IEnumerable<DealArticle>)CacheHelper.Get_SetCache(Common.CacheNameS.DealArticle);
            var QDealArticle = DealArticleRep.Where(x => x.DealArticleCode == null || x.DealArticleName == null);
            if (QDealArticle.Any())
            {
                foreach (var item in QDealArticle)
                {
                    item.DealArticleName = item.DealArticleName ?? "";
                    item.DealArticleCode = item.DealArticleCode ?? "";
                }
            }
            var data = DealArticleRep;
            if (q != "")
            {
                data = data.Where(n => n.DealArticleCode.Contains(q) || n.DealArticleName.Contains(q));
            }
            var list = data.Select(n => new { 
                ID = n.DealArticleCode, 
                TEXT = n.DealArticleName, 
                Pay_ModeCode = n.Pay_ModeCode, 
                Carriage = n.Carriage, 
                CarriageEns = n.CarriageEns, 
                Incidental_Expenses = n.Incidental_Expenses,
                Incidental_ExpensesEns = n.Incidental_ExpensesEns
            });
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
