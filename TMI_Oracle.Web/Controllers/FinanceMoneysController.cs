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
    public class FinanceMoneysController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<FinanceMoney>, Repository<FinanceMoney>>();
        //container.RegisterType<IFinanceMoneyService, FinanceMoneyService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IFinanceMoneyService _financeMoneyService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/FinanceMoneys";

        public FinanceMoneysController(IFinanceMoneyService financeMoneyService, IUnitOfWorkAsync unitOfWork)
        {
            _financeMoneyService = financeMoneyService;
            _unitOfWork = unitOfWork;
        }

        // GET: FinanceMoneys/Index
        public ActionResult Index()
        {
            //var financemoney  = _financeMoneyService.Queryable().Include(f => f.OOrder).AsQueryable();
            //return View(financemoney);
            return View();
        }

        // Get :FinanceMoneys/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var financemoney = _financeMoneyService.Query(new FinanceMoneyQuery().Withfilter(filters)).Include(f => f.OOrder).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = financemoney.Select(n => new
            {
                //OOrderOrderNo = (n.OOrder == null ? "" : n.OOrder.OrderNo),
                Id = n.Id,
                OrderId = n.OrderId,
                CostMoneyId = n.CostMoneyId,
                SupplierName = n.SupplierName,
                ServiceProject = n.ServiceProject,
                Price = n.Price,
                Num = n.Num,
                TotalAmount = n.TotalAmount,
                RequestAmount = n.RequestAmount,
                ExcessAmount = n.ExcessAmount,
                Remark = n.Remark,
                AuditStatus = n.AuditStatus,
                OperatingPoint = n.OperatingPoint
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(FinanceMoneyChangeViewModel financemoney)
        {
            if (financemoney.updated != null)
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

                foreach (var updated in financemoney.updated)
                {
                    _financeMoneyService.Update(updated);
                }
            }
            if (financemoney.deleted != null)
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

                foreach (var deleted in financemoney.deleted)
                {
                    _financeMoneyService.Delete(deleted);
                }
            }
            if (financemoney.inserted != null)
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

                foreach (var inserted in financemoney.inserted)
                {
                    _financeMoneyService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((financemoney.updated != null && financemoney.updated.Any()) ||
                (financemoney.deleted != null && financemoney.deleted.Any()) ||
                (financemoney.inserted != null && financemoney.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(financemoney);
                    return Json(new { Success = true, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作", ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, TMI.Web.Models.EnumType.NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOrders()
        {
            var orderRepository = _unitOfWork.Repository<Order>();
            var data = orderRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, OrderNo = n.OrderNo });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        // GET: FinanceMoneys/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinanceMoney financeMoney = _financeMoneyService.Find(id);
            if (financeMoney == null)
            {
                return HttpNotFound();
            }
            return View(financeMoney);
        }

        // GET: FinanceMoneys/Create
        public ActionResult Create()
        {
            FinanceMoney financeMoney = new FinanceMoney();
            //set default value
            var orderRepository = _unitOfWork.Repository<Order>();
            ViewBag.OrderId = new SelectList(orderRepository.Queryable(), "Id", "OrderNo");
            return View(financeMoney);
        }

        // POST: FinanceMoneys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OOrder,Id,OrderId,SupplierName,ServiceProject,Price,Num,TotalAmount,RequestAmount,ExcessAmount,Remark,AuditStatus,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] FinanceMoney financeMoney)
        {
            if (ModelState.IsValid)
            {
                _financeMoneyService.Insert(financeMoney);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a FinanceMoney record");
                return RedirectToAction("Index");
            }

            var orderRepository = _unitOfWork.Repository<Order>();
            ViewBag.OrderId = new SelectList(orderRepository.Queryable(), "Id", "OrderNo", financeMoney.OrderId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(financeMoney);
        }

        // GET: FinanceMoneys/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinanceMoney financeMoney = _financeMoneyService.Find(id);
            if (financeMoney == null)
            {
                return HttpNotFound();
            }
            var orderRepository = _unitOfWork.Repository<Order>();
            ViewBag.OrderId = new SelectList(orderRepository.Queryable(), "Id", "OrderNo", financeMoney.OrderId);
            return View(financeMoney);
        }

        // POST: FinanceMoneys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OOrder,Id,OrderId,SupplierName,ServiceProject,Price,Num,TotalAmount,RequestAmount,ExcessAmount,Remark,AuditStatus,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] FinanceMoney financeMoney)
        {
            if (ModelState.IsValid)
            {
                financeMoney.ObjectState = ObjectState.Modified;
                _financeMoneyService.Update(financeMoney);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a FinanceMoney record");
                return RedirectToAction("Index");
            }
            var orderRepository = _unitOfWork.Repository<Order>();
            ViewBag.OrderId = new SelectList(orderRepository.Queryable(), "Id", "OrderNo", financeMoney.OrderId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(financeMoney);
        }

        // GET: FinanceMoneys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinanceMoney financeMoney = _financeMoneyService.Find(id);
            if (financeMoney == null)
            {
                return HttpNotFound();
            }
            return View(financeMoney);
        }

        // POST: FinanceMoneys/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FinanceMoney financeMoney = _financeMoneyService.Find(id);
            _financeMoneyService.Delete(financeMoney);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a FinanceMoney record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "financemoney_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _financeMoneyService.ExportExcel(filterRules, sort, order);
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
