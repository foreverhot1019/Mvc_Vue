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
    public class AirLinesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<AirLine>, Repository<AirLine>>();
        //container.RegisterType<IAirLineService, AirLineService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IAirLineService  _airLineService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/AirLines";

        public AirLinesController (IAirLineService  airLineService, IUnitOfWorkAsync unitOfWork)
        {
            _airLineService  = airLineService;
            _unitOfWork = unitOfWork;
        }

        // GET: AirLines/Index
        public ActionResult Index()
        {
            //var airline  = _airLineService.Queryable().Include(a => a.OAirTicket).AsQueryable();
             //return View(airline);
			 return View();
        }

        // Get :AirLines/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
			var airline = _airLineService.Query(new AirLineQuery().Withfilter(filters)).Include(a => a.OAirTicket).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            var datarows = airline.Select( n => new {  
OAirTicketAirTicketNo = (n.OAirTicket==null?"": n.OAirTicket.AirTicketNo), 
Id = n.Id, 
AirTicketOrderId = n.AirTicketOrderId, 
AirLineType = n.AirLineType, 
AirCompany = n.AirCompany, 
Flight_No = n.Flight_No, 
City = n.City, 
Position = n.Position, 
Flight_Date_Want = n.Flight_Date_Want, 
TakeOffTime = n.TakeOffTime, 
ArrivalTime = n.ArrivalTime, 
TicketPrice = n.TicketPrice, 
BillTaxAmount = n.BillTaxAmount, 
CostMoney = n.CostMoney, 
SellPrice = n.SellPrice, 
TicketNum = n.TicketNum, 
Profit = n.Profit, 
Insurance = n.Insurance, 
Is_Endorse = n.Is_Endorse, 
EndorseDate = n.EndorseDate, 
EndorseWho = n.EndorseWho, 
Is_ReturnTicket = n.Is_ReturnTicket, 
ReturnTicketDate = n.ReturnTicketDate, 
ReturnTicketWho = n.ReturnTicketWho, 
Is_Cancel = n.Is_Cancel, 
CancelDate = n.CancelDate, 
CancelWho = n.CancelWho, 
Remark = n.Remark, 
OperatingPoint = n.OperatingPoint}).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(AirLineChangeViewModel airline)
        {
            if (airline.updated != null)
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

                foreach (var updated in airline.updated)
                {
                    _airLineService.Update(updated);
                }
            }
            if (airline.deleted != null)
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

                foreach (var deleted in airline.deleted)
                {
                    _airLineService.Delete(deleted);
                }
            }
            if (airline.inserted != null)
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

                foreach (var inserted in airline.inserted)
                {
                    _airLineService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((airline.updated != null && airline.updated.Any()) || 
				(airline.deleted != null && airline.deleted.Any()) || 
				(airline.inserted != null && airline.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(airline);
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
		       
        // GET: AirLines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirLine airLine = _airLineService.Find(id);
            if (airLine == null)
            {
                return HttpNotFound();
            }
            return View(airLine);
        }

        // GET: AirLines/Create
        public ActionResult Create()
        {
            AirLine airLine = new AirLine();
            //set default value
            var airticketorderRepository = _unitOfWork.Repository<AirTicketOrder>();
            ViewBag.AirTicketOrderId = new SelectList(airticketorderRepository.Queryable(), "Id", "AirTicketNo");
            return View(airLine);
        }

        // POST: AirLines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OAirTicket,Id,AirTicketOrderId,AirLineType,AirCompany,Flight_No,City,Position,Flight_Date_Want,TakeOffTime,ArrivalTime,TicketPrice,BillTaxAmount,CostMoney,SellPrice,TicketNum,Profit,Insurance,Is_Endorse,EndorseDate,EndorseWho,Is_ReturnTicket,ReturnTicketDate,ReturnTicketWho,Is_Cancel,CancelDate,CancelWho,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] AirLine airLine)
        {
            if (ModelState.IsValid)
            {
				_airLineService.Insert(airLine);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a AirLine record");
                return RedirectToAction("Index");
            }

            var airticketorderRepository = _unitOfWork.Repository<AirTicketOrder>();
            ViewBag.AirTicketOrderId = new SelectList(airticketorderRepository.Queryable(), "Id", "AirTicketNo", airLine.AirTicketOrderId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(airLine);
        }

        // GET: AirLines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirLine airLine = _airLineService.Find(id);
            if (airLine == null)
            {
                return HttpNotFound();
            }
            var airticketorderRepository = _unitOfWork.Repository<AirTicketOrder>();
            ViewBag.AirTicketOrderId = new SelectList(airticketorderRepository.Queryable(), "Id", "AirTicketNo", airLine.AirTicketOrderId);
            return View(airLine);
        }

        // POST: AirLines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OAirTicket,Id,AirTicketOrderId,AirLineType,AirCompany,Flight_No,City,Position,Flight_Date_Want,TakeOffTime,ArrivalTime,TicketPrice,BillTaxAmount,CostMoney,SellPrice,TicketNum,Profit,Insurance,Is_Endorse,EndorseDate,EndorseWho,Is_ReturnTicket,ReturnTicketDate,ReturnTicketWho,Is_Cancel,CancelDate,CancelWho,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] AirLine airLine)
        {
            if (ModelState.IsValid)
            {
				airLine.ObjectState = ObjectState.Modified;
				_airLineService.Update(airLine);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a AirLine record");
                return RedirectToAction("Index");
            }
            var airticketorderRepository = _unitOfWork.Repository<AirTicketOrder>();
            ViewBag.AirTicketOrderId = new SelectList(airticketorderRepository.Queryable(), "Id", "AirTicketNo", airLine.AirTicketOrderId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(airLine);
        }

        // GET: AirLines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirLine airLine = _airLineService.Find(id);
            if (airLine == null)
            {
                return HttpNotFound();
            }
            return View(airLine);
        }

        // POST: AirLines/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AirLine airLine = _airLineService.Find(id);
            _airLineService.Delete(airLine);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a AirLine record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "airline_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _airLineService.ExportExcel(filterRules,sort, order );
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
