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
    public class ChangeOrderHistoriesController : BaseController// Controller
    {
        //private WebdbContext db = new WebdbContext();
        private readonly IChangeOrderHistoryService _changeOrderHistoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //只需要在对象成员前面加上[Dependency]，
        //就是把构造函数去掉，成员对象上面加[Dependency]注入
        [Microsoft.Practices.Unity.Dependency]
        public IChangeOrderHistoryService OIChangeOrderHistoryService { get; set; }

        public ChangeOrderHistoriesController(IChangeOrderHistoryService changeOrderHistoryService, IUnitOfWorkAsync unitOfWork)
        {
            _changeOrderHistoryService = changeOrderHistoryService;
            _unitOfWork = unitOfWork;
        }

        // GET: ChangeOrderHistories/Index
        public ActionResult Index()
        {
            var ArrEnumChangeType = Common.GetEnumToDic("EnumChangeType", "TMI.Web.Models.ChangeOrderHistory");
            ViewData["EnumChangeType"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrEnumChangeType.Select(x => new { text = x.Key, value = x.Value, x.DisplayName, x.DisplayDescription }));
            //var changeorderhistory  = _changeOrderHistoryService.Queryable().AsQueryable();
            //return View(changeorderhistory  );
            return View();
        }

        // Get :ChangeOrderHistories/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var changeorderhistory = _changeOrderHistoryService.Query(new ChangeOrderHistoryQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = changeorderhistory.Select(n => new
            {
                Id = n.Id,
                Key_Id = n.Key_Id,
                TableName = n.TableName,
                ChangeType = n.ChangeType,
                InsertNum = n.InsertNum,
                UpdateNum = n.UpdateNum,
                DeleteNum = n.DeleteNum,
                Content = n.Content,
                ADDID = n.ADDID,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS,
                EDITID = n.EDITID,
                OperatingPoint = n.OperatingPoint
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(ChangeOrderHistoryChangeViewModel changeorderhistory)
        {
            if (changeorderhistory.updated != null)
            {
                foreach (var updated in changeorderhistory.updated)
                {
                    _changeOrderHistoryService.Update(updated);
                }
            }
            if (changeorderhistory.deleted != null)
            {
                foreach (var deleted in changeorderhistory.deleted)
                {
                    _changeOrderHistoryService.Delete(deleted);
                }
            }
            if (changeorderhistory.inserted != null)
            {
                foreach (var inserted in changeorderhistory.inserted)
                {
                    _changeOrderHistoryService.Insert(inserted);
                }
            }
            _unitOfWork.SaveChanges();

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        // GET: ChangeOrderHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChangeOrderHistory changeOrderHistory = _changeOrderHistoryService.Find(id);
            if (changeOrderHistory == null)
            {
                return HttpNotFound();
            }
            return View(changeOrderHistory);
        }

        // GET: ChangeOrderHistories/Create
        public ActionResult Create()
        {
            ChangeOrderHistory changeOrderHistory = new ChangeOrderHistory();
            //set default value
            return View(changeOrderHistory);
        }

        // POST: ChangeOrderHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Key_Id,TableName,ChangeType,InsertNum,UpdateNum,DeleteNum,Content,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID,OperatingPoint")] ChangeOrderHistory changeOrderHistory)
        {
            if (ModelState.IsValid)
            {
                _changeOrderHistoryService.Insert(changeOrderHistory);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a ChangeOrderHistory record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(changeOrderHistory);
        }

        // GET: ChangeOrderHistories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChangeOrderHistory changeOrderHistory = _changeOrderHistoryService.Find(id);
            if (changeOrderHistory == null)
            {
                return HttpNotFound();
            }
            return View(changeOrderHistory);
        }

        // POST: ChangeOrderHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Key_Id,TableName,ChangeType,InsertNum,UpdateNum,DeleteNum,Content,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID,OperatingPoint")] ChangeOrderHistory changeOrderHistory)
        {
            if (ModelState.IsValid)
            {
                changeOrderHistory.ObjectState = ObjectState.Modified;
                _changeOrderHistoryService.Update(changeOrderHistory);

                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a ChangeOrderHistory record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(changeOrderHistory);
        }

        // GET: ChangeOrderHistories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChangeOrderHistory changeOrderHistory = _changeOrderHistoryService.Find(id);
            if (changeOrderHistory == null)
            {
                return HttpNotFound();
            }
            return View(changeOrderHistory);
        }

        // POST: ChangeOrderHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChangeOrderHistory changeOrderHistory = _changeOrderHistoryService.Find(id);
            _changeOrderHistoryService.Delete(changeOrderHistory);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a ChangeOrderHistory record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "changeorderhistory_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _changeOrderHistoryService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);

        }

        /// <summary>
        /// 获取弹出框操作日志界面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPopupOrdChangHis()
        {
            return PartialView("_GetPopupOrdChangHis");
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
