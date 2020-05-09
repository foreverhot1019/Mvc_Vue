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
    public class FeeUnitsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<FeeUnit>, Repository<FeeUnit>>();
        //container.RegisterType<IFeeUnitService, FeeUnitService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IFeeUnitService  _feeUnitService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/FeeUnits";

        public FeeUnitsController (IFeeUnitService  feeUnitService, IUnitOfWorkAsync unitOfWork)
            :base(false,true)
        {
            _feeUnitService  = feeUnitService;
            _unitOfWork = unitOfWork;
        }

        // GET: FeeUnits/Index
        public ActionResult Index()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            //var feeunit  = _feeUnitService.Queryable().AsQueryable();
            //return View(feeunit  );
			return View();
        }

        // Get :FeeUnits/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
			var feeunit = _feeUnitService.Query(new FeeUnitQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            var datarows = feeunit.Select( n => new {  
                           Id = n.Id, 
                           FeeUnitName = n.FeeUnitName, 
                           Remark = n.Remark, 
                           Description = n.Description, 
                           Status = n.Status, 
                           OperatingPoint = n.OperatingPoint, 
                           ADDWHO = n.ADDWHO, 
                           ADDTS = n.ADDTS, 
                           EDITWHO = n.EDITWHO, 
                           EDITTS = n.EDITTS}).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(FeeUnitChangeViewModel feeunit)
        {
            if (feeunit.updated != null)
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

                foreach (var updated in feeunit.updated)
                {
                    _feeUnitService.Update(updated);
                }
            }
            if (feeunit.deleted != null)
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

                foreach (var deleted in feeunit.deleted)
                {
                    _feeUnitService.Delete(deleted);
                }
            }
            if (feeunit.inserted != null)
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

                foreach (var inserted in feeunit.inserted)
                {
                    _feeUnitService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((feeunit.updated != null && feeunit.updated.Any()) || 
				(feeunit.deleted != null && feeunit.deleted.Any()) || 
				(feeunit.inserted != null && feeunit.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(feeunit);
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
		       
        // GET: FeeUnits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeeUnit feeUnit = _feeUnitService.Find(id);
            if (feeUnit == null)
            {
                return HttpNotFound();
            }
            return View(feeUnit);
        }

        // GET: FeeUnits/Create
        public ActionResult Create()
        {
            FeeUnit feeUnit = new FeeUnit();
            //set default value
            return View(feeUnit);
        }

        // POST: FeeUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FeeUnitName,Remark,Description,Status,OperatingPoint,ADDWHO,ADDTS,EDITWHO,EDITTS")] FeeUnit feeUnit)
        {
            if (ModelState.IsValid)
            {
				_feeUnitService.Insert(feeUnit);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a FeeUnit record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(feeUnit);
        }

        // GET: FeeUnits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeeUnit feeUnit = _feeUnitService.Find(id);
            if (feeUnit == null)
            {
                return HttpNotFound();
            }
            return View(feeUnit);
        }

        // POST: FeeUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FeeUnitName,Remark,Description,Status,OperatingPoint,ADDWHO,ADDTS,EDITWHO,EDITTS")] FeeUnit feeUnit)
        {
            if (ModelState.IsValid)
            {
				feeUnit.ObjectState = ObjectState.Modified;
				_feeUnitService.Update(feeUnit);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a FeeUnit record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(feeUnit);
        }

        // GET: FeeUnits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeeUnit feeUnit = _feeUnitService.Find(id);
            if (feeUnit == null)
            {
                return HttpNotFound();
            }
            return View(feeUnit);
        }

        // POST: FeeUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FeeUnit feeUnit = _feeUnitService.Find(id);
            _feeUnitService.Delete(feeUnit);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a FeeUnit record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "feeunit_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _feeUnitService.ExportExcel(filterRules,sort, order );
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取币制信息代码和名称
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerFeeUnits(int page = 1, int rows = 10, string q = "")
        {
            var feeUnitRep = _unitOfWork.RepositoryAsync<FeeUnit>();
            var data = feeUnitRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.FeeUnitName.Contains(q) || n.Id.ToString().Contains(q));
            }
            var list = data.Select(n => new { ID = n.Id, TEXT = n.FeeUnitName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);

        }
        
        /// <summary>
        /// 获取币制信息代码和名称
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerFeeUnits_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var FeeUnitRep = (IEnumerable<FeeUnit>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeUnit);
            var QFeeUnit = FeeUnitRep.Where(x => x.FeeUnitName == null);
            foreach (var item in QFeeUnit)
            {
                item.FeeUnitName = item.FeeUnitName ?? "";
            }
            var data = FeeUnitRep;
            if (q != "")
            {
                data = data.Where(n => n.FeeUnitName.Contains(q) || n.Id.ToString().Contains(q));
            }
            var list = data.Select(n => new { ID = n.Id, TEXT = n.FeeUnitName });
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
