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
    public class PlanePersonsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<PlanePerson>, Repository<PlanePerson>>();
        //container.RegisterType<IPlanePersonService, PlanePersonService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPlanePersonService  _planePersonService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/PlanePersons";

        public PlanePersonsController (IPlanePersonService  planePersonService, IUnitOfWorkAsync unitOfWork)
        {
            _planePersonService  = planePersonService;
            _unitOfWork = unitOfWork;
        }

        // GET: PlanePersons/Index
        public ActionResult Index()
        {
            //var planeperson  = _planePersonService.Queryable().Include(p => p.OAirTicket).AsQueryable();
             //return View(planeperson);
			 return View();
        }

        // Get :PlanePersons/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
			var planeperson = _planePersonService.Query(new PlanePersonQuery().Withfilter(filters)).Include(p => p.OAirTicket).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            var datarows = planeperson.Select( n => new {  
OAirTicketAirTicketNo = (n.OAirTicket==null?"": n.OAirTicket.AirTicketNo), 
Id = n.Id, 
AirTicketOrderId = n.AirTicketOrderId, 
NameChs = n.NameChs, 
LastNameEng = n.LastNameEng, 
FirstNameEng = n.FirstNameEng, 
OperatingPoint = n.OperatingPoint}).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(PlanePersonChangeViewModel planeperson)
        {
            if (planeperson.updated != null)
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

                foreach (var updated in planeperson.updated)
                {
                    _planePersonService.Update(updated);
                }
            }
            if (planeperson.deleted != null)
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

                foreach (var deleted in planeperson.deleted)
                {
                    _planePersonService.Delete(deleted);
                }
            }
            if (planeperson.inserted != null)
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

                foreach (var inserted in planeperson.inserted)
                {
                    _planePersonService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((planeperson.updated != null && planeperson.updated.Any()) || 
				(planeperson.deleted != null && planeperson.deleted.Any()) || 
				(planeperson.inserted != null && planeperson.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(planeperson);
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
			
		public ActionResult GetAirTicketOrder()
        {
            var airticketorderRepository = _unitOfWork.Repository<AirTicketOrder>();
            var data = airticketorderRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, AirTicketNo = n.AirTicketNo });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
		       
        // GET: PlanePersons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanePerson planePerson = _planePersonService.Find(id);
            if (planePerson == null)
            {
                return HttpNotFound();
            }
            return View(planePerson);
        }

        // GET: PlanePersons/Create
        public ActionResult Create()
        {
            PlanePerson planePerson = new PlanePerson();
            //set default value
            var airticketorderRepository = _unitOfWork.Repository<AirTicketOrder>();
            ViewBag.AirTicketOrderId = new SelectList(airticketorderRepository.Queryable(), "Id", "AirTicketNo");
            return View(planePerson);
        }

        // POST: PlanePersons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OAirTicket,Id,AirTicketOrderId,NameChs,LastNameEng,FirstNameEng,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] PlanePerson planePerson)
        {
            if (ModelState.IsValid)
            {
				_planePersonService.Insert(planePerson);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a PlanePerson record");
                return RedirectToAction("Index");
            }

            var airticketorderRepository = _unitOfWork.Repository<AirTicketOrder>();
            ViewBag.AirTicketOrderId = new SelectList(airticketorderRepository.Queryable(), "Id", "AirTicketNo", planePerson.AirTicketOrderId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(planePerson);
        }

        // GET: PlanePersons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanePerson planePerson = _planePersonService.Find(id);
            if (planePerson == null)
            {
                return HttpNotFound();
            }
            var airticketorderRepository = _unitOfWork.Repository<AirTicketOrder>();
            ViewBag.AirTicketOrderId = new SelectList(airticketorderRepository.Queryable(), "Id", "AirTicketNo", planePerson.AirTicketOrderId);
            return View(planePerson);
        }

        // POST: PlanePersons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OAirTicket,Id,AirTicketOrderId,NameChs,LastNameEng,FirstNameEng,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] PlanePerson planePerson)
        {
            if (ModelState.IsValid)
            {
				planePerson.ObjectState = ObjectState.Modified;
				_planePersonService.Update(planePerson);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a PlanePerson record");
                return RedirectToAction("Index");
            }
            var airticketorderRepository = _unitOfWork.Repository<AirTicketOrder>();
            ViewBag.AirTicketOrderId = new SelectList(airticketorderRepository.Queryable(), "Id", "AirTicketNo", planePerson.AirTicketOrderId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(planePerson);
        }

        // GET: PlanePersons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanePerson planePerson = _planePersonService.Find(id);
            if (planePerson == null)
            {
                return HttpNotFound();
            }
            return View(planePerson);
        }

        // POST: PlanePersons/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlanePerson planePerson = _planePersonService.Find(id);
            _planePersonService.Delete(planePerson);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a PlanePerson record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "planeperson_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _planePersonService.ExportExcel(filterRules,sort, order );
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
