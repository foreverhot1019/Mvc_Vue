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
    public class Warehouse_Cargo_SizesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Warehouse_Cargo_Size>, Repository<Warehouse_Cargo_Size>>();
        //container.RegisterType<IWarehouse_Cargo_SizeService, Warehouse_Cargo_SizeService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IWarehouse_Cargo_SizeService  _warehouse_Cargo_SizeService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/Warehouse_Cargo_Sizes";

        public Warehouse_Cargo_SizesController (IWarehouse_Cargo_SizeService  warehouse_Cargo_SizeService, IUnitOfWorkAsync unitOfWork)
        {
            _warehouse_Cargo_SizeService  = warehouse_Cargo_SizeService;
            _unitOfWork = unitOfWork;
        }

        // GET: Warehouse_Cargo_Sizes/Index
        public ActionResult Index()
        {
            //var warehouse_cargo_size  = _warehouse_Cargo_SizeService.Queryable().AsQueryable();
            //return View(warehouse_cargo_size  );
			return View();
        }

        // Get :Warehouse_Cargo_Sizes/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);

            int totalCount = 0;
            if (filters.Count() != 0)
            {
                //int pagenum = offset / limit +1;
                var warehouse_cargo_size = _warehouse_Cargo_SizeService.Query(new Warehouse_Cargo_SizeQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
                var datarows = warehouse_cargo_size.AsQueryable().ToList();
                //.Select( n => new {  
                //                Id = n.Id, 
                //                Warehouse_Receipt_Id = n.Warehouse_Receipt_Id, 
                //                Entry_Id = n.Entry_Id, 
                //                CM_Length = n.CM_Length, 
                //                CM_Width = n.CM_Width, 
                //                CM_Height = n.CM_Height, 
                //                CM_Piece = n.CM_Piece, 
                //                Status = n.Status, 
                //                Remark = n.Remark, 
                //                OperatingPoint = n.OperatingPoint}).ToList();
                if (totalCount < 10)
                {
                    for (var insert = totalCount; insert < 10; insert++)
                    {
                        var cargo = new Warehouse_Cargo_Size();
                        cargo.Id = -insert;
                        datarows.Add(cargo);
                    }
                }
                var pagelist = new { total = totalCount, rows = datarows }; 
                return Json(pagelist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var datarows = new List<Warehouse_Cargo_Size>();
                for (var insert = totalCount; insert < 10; insert++)
                {
                    var cargo = new Warehouse_Cargo_Size();
                    cargo.Id = -insert;
                    datarows.Add(cargo);
                }
                var pagelist = new { total = totalCount, rows = datarows };
                return Json(pagelist, JsonRequestBehavior.AllowGet);
            }
        }

		[HttpPost]
		public ActionResult SaveData(Warehouse_Cargo_SizeChangeViewModel warehouse_cargo_size)
        {
            if (warehouse_cargo_size.updated != null)
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

                foreach (var updated in warehouse_cargo_size.updated)
                {
                    _warehouse_Cargo_SizeService.Update(updated);
                }
            }
            if (warehouse_cargo_size.deleted != null)
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

                foreach (var deleted in warehouse_cargo_size.deleted)
                {
                    _warehouse_Cargo_SizeService.Delete(deleted);
                }
            }
            if (warehouse_cargo_size.inserted != null)
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

                foreach (var inserted in warehouse_cargo_size.inserted)
                {
                    _warehouse_Cargo_SizeService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((warehouse_cargo_size.updated != null && warehouse_cargo_size.updated.Any()) || 
				(warehouse_cargo_size.deleted != null && warehouse_cargo_size.deleted.Any()) || 
				(warehouse_cargo_size.inserted != null && warehouse_cargo_size.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(warehouse_cargo_size);
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
		       
        // GET: Warehouse_Cargo_Sizes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_Cargo_Size warehouse_Cargo_Size = _warehouse_Cargo_SizeService.Find(id);
            if (warehouse_Cargo_Size == null)
            {
                return HttpNotFound();
            }
            return View(warehouse_Cargo_Size);
        }

        // GET: Warehouse_Cargo_Sizes/Create
        public ActionResult Create()
        {
            Warehouse_Cargo_Size warehouse_Cargo_Size = new Warehouse_Cargo_Size();
            //set default value
            return View(warehouse_Cargo_Size);
        }

        // POST: Warehouse_Cargo_Sizes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Warehouse_Receipt_Id,Entry_Id,CM_Length,CM_Width,CM_Height,CM_Piece,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Warehouse_Cargo_Size warehouse_Cargo_Size)
        {
            if (ModelState.IsValid)
            {
				_warehouse_Cargo_SizeService.Insert(warehouse_Cargo_Size);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Warehouse_Cargo_Size record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(warehouse_Cargo_Size);
        }

        // GET: Warehouse_Cargo_Sizes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_Cargo_Size warehouse_Cargo_Size = _warehouse_Cargo_SizeService.Find(id);
            if (warehouse_Cargo_Size == null)
            {
                return HttpNotFound();
            }
            return View(warehouse_Cargo_Size);
        }

        // POST: Warehouse_Cargo_Sizes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Warehouse_Receipt_Id,Entry_Id,CM_Length,CM_Width,CM_Height,CM_Piece,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Warehouse_Cargo_Size warehouse_Cargo_Size)
        {
            if (ModelState.IsValid)
            {
				warehouse_Cargo_Size.ObjectState = ObjectState.Modified;
				_warehouse_Cargo_SizeService.Update(warehouse_Cargo_Size);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Warehouse_Cargo_Size record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(warehouse_Cargo_Size);
        }

        // GET: Warehouse_Cargo_Sizes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_Cargo_Size warehouse_Cargo_Size = _warehouse_Cargo_SizeService.Find(id);
            if (warehouse_Cargo_Size == null)
            {
                return HttpNotFound();
            }
            return View(warehouse_Cargo_Size);
        }

        // POST: Warehouse_Cargo_Sizes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Warehouse_Cargo_Size warehouse_Cargo_Size = _warehouse_Cargo_SizeService.Find(id);
            _warehouse_Cargo_SizeService.Delete(warehouse_Cargo_Size);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Warehouse_Cargo_Size record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "warehouse_cargo_size_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _warehouse_Cargo_SizeService.ExportExcel(filterRules,sort, order );
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
