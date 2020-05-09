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
    public class OPS_H_OrdersController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<OPS_H_Order>, Repository<OPS_H_Order>>();
        //container.RegisterType<IOPS_H_OrderService, OPS_H_OrderService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IOPS_H_OrderService  _oPS_H_OrderService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/OPS_H_Orders";

        public OPS_H_OrdersController (IOPS_H_OrderService  oPS_H_OrderService, IUnitOfWorkAsync unitOfWork)
        {
            _oPS_H_OrderService  = oPS_H_OrderService;
            _unitOfWork = unitOfWork;
        }

        // GET: OPS_H_Orders/Index
        public ActionResult Index()
        {
            //var ops_h_order  = _oPS_H_OrderService.Queryable().AsQueryable();
            //return View(ops_h_order  );
			return View();
        }

        // Get :OPS_H_Orders/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
			var ops_h_order = _oPS_H_OrderService.Query(new OPS_H_OrderQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            var datarows = ops_h_order.Select( n => new {  
                                                Id = n.Id, 
                                                Shipper_H = n.Shipper_H, 
                                                Consignee_H = n.Consignee_H, 
                                                Notify_Part_H = n.Notify_Part_H, 
                                                Currency_H = n.Currency_H, 
                                                Bragainon_Article_H = n.Bragainon_Article_H, 
                                                Pay_Mode_H = n.Pay_Mode_H, 
                                                Carriage_H = n.Carriage_H, 
                                                Incidental_Expenses_H = n.Incidental_Expenses_H, 
                                                Declare_Value_Trans_H = n.Declare_Value_Trans_H, 
                                                Declare_Value_Ciq_H = n.Declare_Value_Ciq_H, 
                                                Risk_H = n.Risk_H, 
                                                Marks_H = n.Marks_H, 
                                                EN_Name_H = n.EN_Name_H, 
                                                Pieces_H = n.Pieces_H, 
                                                Weight_H = n.Weight_H, 
                                                Volume_H = n.Volume_H, 
                                                Charge_Weight_H = n.Charge_Weight_H, 
                                                HBL = n.HBL, 
                                                Operation_Id = n.Operation_Id, 
                                                Is_Self = n.Is_Self, 
                                                Ty_Type = n.Ty_Type, 
                                                Lot_No = n.Lot_No, 
                                                Hbl_Feight = n.Hbl_Feight, 
                                                Is_XC = n.Is_XC, 
                                                Is_BAS = n.Is_BAS, 
                                                Is_DCZ = n.Is_DCZ, 
                                                Is_ZB = n.Is_ZB, 
                                                ADDPoint = n.ADDPoint, 
                                                EDITPoint = n.EDITPoint, 
                                                Status = n.Status,
                                                Batch_Num = n.Batch_Num,
                                                OperatingPoint = n.OperatingPoint}).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(OPS_H_OrderChangeViewModel ops_h_order)
        {
            if (ops_h_order.updated != null)
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

                foreach (var updated in ops_h_order.updated)
                {
                    _oPS_H_OrderService.Update(updated);
                }
            }
            if (ops_h_order.deleted != null)
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

                foreach (var deleted in ops_h_order.deleted)
                {
                    _oPS_H_OrderService.Delete(deleted);
                }
            }
            if (ops_h_order.inserted != null)
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

                foreach (var inserted in ops_h_order.inserted)
                {
                    _oPS_H_OrderService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((ops_h_order.updated != null && ops_h_order.updated.Any()) || 
				(ops_h_order.deleted != null && ops_h_order.deleted.Any()) || 
				(ops_h_order.inserted != null && ops_h_order.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(ops_h_order);
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
		       
        // GET: OPS_H_Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPS_H_Order oPS_H_Order = _oPS_H_OrderService.Find(id);
            if (oPS_H_Order == null)
            {
                return HttpNotFound();
            }
            return View(oPS_H_Order);
        }

        // GET: OPS_H_Orders/Create
        public ActionResult Create()
        {
            OPS_H_Order oPS_H_Order = new OPS_H_Order();
            //set default value
            return View(oPS_H_Order);
        }

        // POST: OPS_H_Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Shipper_H,Consignee_H,Notify_Part_H,Currency_H,Bragainon_Article_H,Pay_Mode_H,Carriage_H,Incidental_Expenses_H,Declare_Value_Trans_H,Declare_Value_Ciq_H,Risk_H,Marks_H,EN_Name_H,Pieces_H,Weight_H,Volume_H,Charge_Weight_H,HBL,Operation_Id,Is_Self,Ty_Type,Lot_No,Hbl_Feight,Is_XC,Is_BAS,Is_DCZ,Is_ZB,ADDPoint,EDITPoint,Status,Batch_Num,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] OPS_H_Order oPS_H_Order)
        {
            if (ModelState.IsValid)
            {
				_oPS_H_OrderService.Insert(oPS_H_Order);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a OPS_H_Order record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(oPS_H_Order);
        }

        // GET: OPS_H_Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPS_H_Order oPS_H_Order = _oPS_H_OrderService.Find(id);
            if (oPS_H_Order == null)
            {
                return HttpNotFound();
            }
            return View(oPS_H_Order);
        }

        // POST: OPS_H_Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Shipper_H,Consignee_H,Notify_Part_H,Currency_H,Bragainon_Article_H,Pay_Mode_H,Carriage_H,Incidental_Expenses_H,Declare_Value_Trans_H,Declare_Value_Ciq_H,Risk_H,Marks_H,EN_Name_H,Pieces_H,Weight_H,Volume_H,Charge_Weight_H,HBL,Operation_Id,Is_Self,Ty_Type,Lot_No,Hbl_Feight,Is_XC,Is_BAS,Is_DCZ,Is_ZB,ADDPoint,EDITPoint,Status,Batch_Num,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] OPS_H_Order oPS_H_Order)
        {
            if (ModelState.IsValid)
            {
				oPS_H_Order.ObjectState = ObjectState.Modified;
				_oPS_H_OrderService.Update(oPS_H_Order);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a OPS_H_Order record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(oPS_H_Order);
        }

        // GET: OPS_H_Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPS_H_Order oPS_H_Order = _oPS_H_OrderService.Find(id);
            if (oPS_H_Order == null)
            {
                return HttpNotFound();
            }
            return View(oPS_H_Order);
        }

        // POST: OPS_H_Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OPS_H_Order oPS_H_Order = _oPS_H_OrderService.Find(id);
            _oPS_H_OrderService.Delete(oPS_H_Order);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a OPS_H_Order record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "ops_h_order_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _oPS_H_OrderService.ExportExcel(filterRules,sort, order );
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
