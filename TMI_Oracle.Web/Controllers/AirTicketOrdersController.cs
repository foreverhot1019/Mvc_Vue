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
using System.Text.RegularExpressions;
using System.Globalization;

namespace TMI.Web.Controllers
{
    public class AirTicketOrdersController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<AirTicketOrder>, Repository<AirTicketOrder>>();
        //container.RegisterType<IAirTicketOrderService, AirTicketOrderService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IAirTicketOrderService  _airTicketOrderService;
        private readonly IPlanePersonService _planePersonService;
        private readonly IAirLineService _airLineService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        //验证权限的名称
        private string ControllerQXName = "/AirTicketOrders";

        public AirTicketOrdersController(IAirTicketOrderService airTicketOrderService, IPlanePersonService planePersonService, IAirLineService airLineService,IUnitOfWorkAsync unitOfWork)
        {
            _airTicketOrderService  = airTicketOrderService;
            _planePersonService = planePersonService;
            _airLineService = airLineService;
            _unitOfWork = unitOfWork;
        }

        // GET: AirTicketOrders/Index
        public ActionResult Index()
        {
            //var airticketorder  = _airTicketOrderService.Queryable().AsQueryable();
            //return View(airticketorder  );
			return View();
        }

        // Get :AirTicketOrders/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            var AirLineRep = _airLineService.Queryable();
            //var airticketorder = _airTicketOrderService.Query(new AirTicketOrderQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount); 
            var airticketorder = _airTicketOrderService.Query(new AirTicketOrderQuery().Withfilter(filters));
            //int pagenum = offset / limit +1;        
            List<string> ArrPPALfilterRules = new List<string>(){
                 "AirCompany",
                 "TicketNum",
                 "Flight_No"
            };
            var ArrId = airticketorder.Select(x => x.Id);
            List<int> newArrId = new List<int>();
            bool Ishasvalue = false;
            var QAirLine = AirLineRep.Where(x => ArrId.Contains((int)x.AirTicketOrderId));
            var QArrPPALfilterRules = filters.Where(x => ArrPPALfilterRules.Contains(x.field));
            if (QArrPPALfilterRules.Any())
            {
                Ishasvalue = true;
                QArrPPALfilterRules = filters.Where(x => x.field == "AirCompany");
                if (QArrPPALfilterRules.Any())
                {
                    var val = QArrPPALfilterRules.FirstOrDefault().value;
                    QAirLine = QAirLine.Where(x => x.AirCompany == val);
                }
                QArrPPALfilterRules = filters.Where(x => x.field == "TicketNum");
                if (QArrPPALfilterRules.Any())
                {
                    var val = QArrPPALfilterRules.FirstOrDefault().value;
                    QAirLine = QAirLine.Where(x => x.TicketNum == val);
                }
                QArrPPALfilterRules = filters.Where(x => x.field == "Flight_No");
                if (QArrPPALfilterRules.Any())
                {
                    var val = QArrPPALfilterRules.FirstOrDefault().value;
                    QAirLine = QAirLine.Where(x => x.Flight_No == val);
                }

                if (QAirLine.Any())
                {
                    newArrId = QAirLine.Select(x => (int)x.AirTicketOrderId).ToList();
                }

            }
            var datarows = airticketorder.Select( n => new {  
                    Id = n.Id, 
                    AirTicketNo = n.AirTicketNo, 
                    CompanyCIQID = n.CompanyCIQID, 
                    CompanyName = n.CompanyName, 
                    Saller = n.Saller, 
                    AirTicketOrdType = n.AirTicketOrdType, 
                    TicketType = n.TicketType, 
                    PlanePerson = n.PlanePerson, 
                    PNR = n.PNR, 
                    TravlePersonNum = n.TravlePersonNum, 
                    ExpectPaymentDate = n.ExpectPaymentDate, 
                    SupplierName = n.SupplierName, 
                    AirTicketNo_Org = n.AirTicketNo_Org, 
                    Status = n.Status, 
                    AuditStatus = n.AuditStatus, 
                    OperatingPoint = n.OperatingPoint,
                    ADDTS = n.ADDTS,
                    ADDID = n.ADDID,
                    ADDWHO = n.ADDWHO,
                    EDITID = n.EDITID,
                    EDITTS = n.EDITTS,
                    EDITWHO = n.EDITWHO }).ToList();

            if(Ishasvalue)
                datarows = datarows.Where(x=>newArrId.Contains(x.Id)).ToList();
            var QResult = datarows.AsQueryable().OrderBy(sort, order).Skip((page - 1) * rows).Take(rows).ToList();
            var pagelist = new { total = totalCount, rows = QResult };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
        public ActionResult SaveData(AirTicketOrderChangeViewModel airticketorder)
        {
            var AirTicketOrderRep = _unitOfWork.Repository<AirTicketOrder>();
            var PlanePersonRep = _unitOfWork.Repository<PlanePerson>();
            var AirLineRep = _unitOfWork.Repository<AirLine>();
            int ATOrdNum = 0, PlaPerNum = 0, AirLineNum = 0;
            var DbCTxt = _unitOfWork.getDbContext();
            if (airticketorder.ATPAdeltRows != null)
            {
                var ATPAdeltRows = airticketorder.ATPAdeltRows;
                if (ATPAdeltRows.DelPlanePerson != null && ATPAdeltRows.DelPlanePerson.Any()) 
                {
                    var data = ATPAdeltRows.DelPlanePerson;
                    data = data.Where(x => x > 0);
                    if (data.Any()) 
                    {
                        data.ToList().ForEach((i) =>
                        {
                            PlanePersonRep.Delete(i);
                        });
                    }
                }
                if (ATPAdeltRows.DelAirLine != null && ATPAdeltRows.DelAirLine.Any())
                {
                    var data = ATPAdeltRows.DelAirLine;
                    data = data.Where(x => x > 0);
                    if (data.Any())
                    {
                        data.ToList().ForEach((i) =>
                        {
                            AirLineRep.Delete(i);
                        });
                    }
                }
            }

            if (airticketorder.updated != null)
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

                var ArrAirTicketOrderId = airticketorder.updated.Select(x => (int?)x.AirTicketOrder.Id);

                foreach (var updated in airticketorder.updated)
                {
                    _airTicketOrderService.Update(updated.AirTicketOrder);
                    if (updated.ArrAirLine != null && updated.ArrAirLine.Any()) 
                    { 
                        foreach(var item in updated.ArrAirLine){
                            if (item.Id > 0)
                                AirLineRep.Update(item);
                            else 
                            {
                                if (item.AirTicketOrderId == null)
                                    item.AirTicketOrderId = updated.AirTicketOrder.Id;
                                AirLineRep.Insert(item);
                            }
                        }
                    }
                    if (updated.ArrPlanePerson != null && updated.ArrPlanePerson.Any())
                    {
                        foreach (var item in updated.ArrPlanePerson)
                        {
                            if (item.Id > 0)
                                PlanePersonRep.Update(item);
                            else
                            {
                                if (item.AirTicketOrderId == null)
                                    item.AirTicketOrderId = updated.AirTicketOrder.Id;
                                PlanePersonRep.Insert(item);
                            }
                        }
                    }
                }
            }
            if (airticketorder.deleted != null)
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

                foreach (var deleted in airticketorder.deleted)
                {
                    var QPlanePerson = PlanePersonRep.Queryable().Where(x => x.AirTicketOrderId == deleted.AirTicketOrder.Id);
                    var QAirLine = AirLineRep.Queryable().Where(x => x.AirTicketOrderId == deleted.AirTicketOrder.Id);
                    if (QPlanePerson.Any())
                    {
                        foreach (var item in QPlanePerson) 
                        {
                            PlanePersonRep.Delete(item);
                        }  
                    }
                    if (QAirLine.Any())
                    {
                        foreach (var item in QAirLine)
                        {
                            AirLineRep.Delete(item);
                        }
                    }
                    _airTicketOrderService.Delete(deleted.AirTicketOrder);
                }
            }
            if (airticketorder.inserted != null)
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

                foreach (var inserted in airticketorder.inserted)
                {
                    var OAirticketorder = inserted.AirTicketOrder;
                    OAirticketorder.Id = --ATOrdNum;
                    OAirticketorder.ObjectState = ObjectState.Added;
                    DbCTxt.Entry(OAirticketorder).State = EntityState.Added;
                    //获取机票订单流水号
                    OAirticketorder.AirTicketNo = SequenceBuilder.NextJPOrderNo();
                    if (inserted.ArrPlanePerson != null && inserted.ArrPlanePerson.Any()) 
                    {
                        foreach (var item in inserted.ArrPlanePerson) 
                        {
                            if (item.Id <= 0) 
                            {
                                item.Id = --PlaPerNum;
                                item.AirTicketOrderId = OAirticketorder.Id;
                                PlanePersonRep.Insert(item);
                            }
                        }
                    }
                    if (inserted.ArrAirLine !=null && inserted.ArrAirLine.Any())
                    {
                        foreach (var item in inserted.ArrAirLine)
                        {
                            if (item.Id <= 0)
                            {
                                item.Id = --AirLineNum;
                                item.AirTicketOrderId = OAirticketorder.Id;
                                AirLineRep.Insert(item);
                            }
                        }
                    }
                    _airTicketOrderService.Insert(OAirticketorder);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((airticketorder.updated != null && airticketorder.updated.Any()) || 
				(airticketorder.deleted != null && airticketorder.deleted.Any()) || 
				(airticketorder.inserted != null && airticketorder.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(airticketorder);
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

        /// <summary>
        /// 获取乘机人信息
        /// </summary>
        /// <param name="OrderId">机票订单Id</param>
        /// <returns></returns>
        public ActionResult GetPlanePersonId(int AirTicketOrderId)
        {
            var PlanePersonRep = _unitOfWork.Repository<PlanePerson>();
            var data = PlanePersonRep.Queryable().Where(x => x.AirTicketOrderId == AirTicketOrderId).ToList();
            var rows = data;
            return Json(new { Success = true, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取航班信息
        /// </summary>
        /// <param name="OrderId">机票订单Id</param>
        /// <returns></returns>
        public ActionResult GetAirLineId(int AirTicketOrderId)
        {
            var AirLineRep = _unitOfWork.Repository<AirLine>();
            var data = AirLineRep.Queryable().Where(x => x.AirTicketOrderId == AirTicketOrderId).ToList();
            var rows = data;
            return Json(new { Success = true, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 乘机人信息导入
        /// </summary>
        /// <param name="EXPinfo">导入数据</param>
        /// <returns></returns>
        public ActionResult ExportPPInfo(string EXPinfo) 
        {
            if (EXPinfo == null || EXPinfo == "") {
                return Json(new { Success = false, ErrMsg = "导入信息为空！" });
            }
            var rgx = new Regex("(\\d[.][\u4e00-\u9fa5]{1,4}[a-zA-z]+[/]?[a-zA-z]+)|(\\d[.][a-zA-z]+[/]?[a-zA-z]+)");
            var Chnrgx = new Regex("[\u4e00-\u9fa5]{1,4}");
            var LEngrgx = new Regex("[a-zA-z]+[/]");
            var FEngrgx = new Regex("[/][a-zA-z]+");
            EXPinfo = EXPinfo.Replace("\n", "");
            var ArrMatch = rgx.Matches(EXPinfo);
            List<string> ArrMsgInfo = new List<string>();
            List<PlanePerson> ArrPlanePerson = new List<PlanePerson>();

            var EXPinfoLENGTH = EXPinfo.Length;
            string PNR;
            if (EXPinfoLENGTH > 6)
                PNR = EXPinfo.Substring(EXPinfoLENGTH - 6);
            else
                PNR = null;
            foreach (System.Text.RegularExpressions.Match match in ArrMatch)
            {
                var ArrMatchGp = match.Groups;
                if (ArrMatchGp.Count > 0)
                {
                    var gpVal = match.Groups[0].Value;
                    ArrMsgInfo.Add(gpVal);
                }
            }
            if (ArrMsgInfo.Any()) {
                foreach (var item in ArrMsgInfo)
                {
                    PlanePerson PP = new PlanePerson();
                    string Feng = null;
                    string Leng = null;
                    var ChnMarch = Chnrgx.Matches(item);
                    foreach (System.Text.RegularExpressions.Match match in ChnMarch) 
                    {
                        if (match.Groups.Count > 0)
                        {
                            PP.NameChs = match.Groups[0].Value;
                        }
                    }
                    var LEngMarch = LEngrgx.Matches(item);
                    foreach (System.Text.RegularExpressions.Match match in LEngMarch)
                    {
                        if (match.Groups.Count > 0)
                        {
                            var gpVal = match.Groups[0].Value;
                            var LEngName = gpVal.Replace("/", "");
                            PP.LastNameEng = LEngName;
                        }
                    }
                    var FEngMarch = FEngrgx.Matches(item);
                    foreach (System.Text.RegularExpressions.Match match in FEngMarch)
                    {
                        if (match.Groups.Count > 0)
                        {
                            var gpVal = match.Groups[0].Value;
                            var FEngName = gpVal.Replace("/", "");
                            PP.FirstNameEng = FEngName;
                        }
                    }
                    ArrPlanePerson.Add(PP);
                }
            }


            return Json(new { Success = true, rows = ArrPlanePerson, PNR = PNR}, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 航班信息导入
        /// </summary>
        /// <param name="EXPinfo">导入数据</param>
        /// <returns></returns>
        public ActionResult ExportALInfo(string EXPinfo)
        {
            if (EXPinfo == null || EXPinfo == "")
            {
                return Json(new { Success = false, ErrMsg = "导入信息为空！" });
            }
            var rgx = new Regex("\\d[.][a-zA-Z0-9]+[\\s][a-zA-z]{1}[\\s][a-zA-Z]{2}[0-9]{2}[a-zA-Z]{3}[\\s][a-zA-Z]{6}[\\s][a-zA-Z0-9]{3}[\\s][0-9]{4}[\\s][0-9]{4}");
            var FLNOrgx = new Regex("\\d[.][a-zA-Z0-9]+");
            var CErgx = new Regex("[\\s][a-zA-z]{1}[\\s]");
            var XRYrgx = new Regex("[a-zA-Z]{2}[0-9]{2}[a-zA-Z]{3}");
            var CityandNumrgx = new Regex("[a-zA-Z]{6}[\\s][a-zA-Z0-9]{3}");
            var Taketimergx = new Regex("[0-9]{4}[\\s][0-9]{4}");
            EXPinfo = EXPinfo.Replace("\n", "");
            var ArrMatch = rgx.Matches(EXPinfo);
            List<string> ArrMsgInfo = new List<string>();
            List<AirLine> ArrAirLine = new List<AirLine>();

            foreach (System.Text.RegularExpressions.Match match in ArrMatch)
            {
                var ArrMatchGp = match.Groups;
                if (ArrMatchGp.Count > 0)
                {
                    var gpVal = match.Groups[0].Value;
                    //最后一个匹配，最后一行 没有 换行符
                    ArrMsgInfo.Add(gpVal);
                }
            }
            if (ArrMsgInfo.Any())
            {
                foreach (var item in ArrMsgInfo)
                {
                    AirLine AL = new AirLine();
                    string Feng = null;
                    string Leng = null;
                    var Flight_No = FLNOrgx.Matches(item);
                    foreach (System.Text.RegularExpressions.Match match in Flight_No)
                    {
                        if (match.Groups.Count > 0)
                        {
                            AL.Flight_No = match.Groups[0].Value.Substring(2);
                        }
                    }
                    var CW = CErgx.Matches(item);
                    foreach (System.Text.RegularExpressions.Match match in CW)
                    {
                        if (match.Groups.Count > 0)
                        {
                            var gpVal = match.Groups[0].Value.Trim();
                            AL.Position = gpVal;
                        }
                    }
                    var FEngMarch = XRYrgx.Matches(item);
                    foreach (System.Text.RegularExpressions.Match match in FEngMarch)
                    {
                        if (match.Groups.Count > 0)
                        {
                            var gpVal = match.Groups[0].Value;
                        }
                    }
                    var CityandNum = CityandNumrgx.Matches(item);
                    foreach (System.Text.RegularExpressions.Match match in CityandNum)
                    {
                        if (match.Groups.Count > 0)
                        {
                            var gpVal = match.Groups[0].Value;
                            AL.City = gpVal.Substring(0, 6);
                        }
                    }
                    var Taketime = Taketimergx.Matches(item);
                    foreach (System.Text.RegularExpressions.Match match in Taketime)
                    {
                        if (match.Groups.Count > 0)
                        {
                            var gpVal = match.Groups[0].Value;
                        }
                    }
                    AL.TakeOffTime = DateTime.Now;
                    AL.ArrivalTime = DateTime.Now;
                    AL.Flight_Date_Want = DateTime.Now;
                    ArrAirLine.Add(AL);
                }
            }


            return Json(new { Success = true, rows = ArrAirLine }, JsonRequestBehavior.AllowGet);
        }
		       
        // GET: AirTicketOrders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirTicketOrder airTicketOrder = _airTicketOrderService.Find(id);
            if (airTicketOrder == null)
            {
                return HttpNotFound();
            }
            return View(airTicketOrder);
        }

        // GET: AirTicketOrders/Create
        public ActionResult Create()
        {
            AirTicketOrder airTicketOrder = new AirTicketOrder();
            //set default value
            return View(airTicketOrder);
        }

        // POST: AirTicketOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArrAirLine,ArrPlanePerson,Id,AirTicketNo,CompanyCIQID,CompanyName,Saller,AirTicketOrdType,TicketType,PlanePerson,PNR,TravlePersonNum,ExpectPaymentDate,SupplierName,AirTicketNo_Org,Status,AuditStatus,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] AirTicketOrder airTicketOrder)
        {
            if (ModelState.IsValid)
            {
				_airTicketOrderService.Insert(airTicketOrder);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a AirTicketOrder record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(airTicketOrder);
        }

        // GET: AirTicketOrders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirTicketOrder airTicketOrder = _airTicketOrderService.Find(id);
            if (airTicketOrder == null)
            {
                return HttpNotFound();
            }
            return View(airTicketOrder);
        }

        // POST: AirTicketOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArrAirLine,ArrPlanePerson,Id,AirTicketNo,CompanyCIQID,CompanyName,Saller,AirTicketOrdType,TicketType,PlanePerson,PNR,TravlePersonNum,ExpectPaymentDate,SupplierName,AirTicketNo_Org,Status,AuditStatus,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] AirTicketOrder airTicketOrder)
        {
            if (ModelState.IsValid)
            {
				airTicketOrder.ObjectState = ObjectState.Modified;
				_airTicketOrderService.Update(airTicketOrder);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a AirTicketOrder record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(airTicketOrder);
        }

        // GET: AirTicketOrders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirTicketOrder airTicketOrder = _airTicketOrderService.Find(id);
            if (airTicketOrder == null)
            {
                return HttpNotFound();
            }
            return View(airTicketOrder);
        }

        // POST: AirTicketOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AirTicketOrder airTicketOrder = _airTicketOrderService.Find(id);
            _airTicketOrderService.Delete(airTicketOrder);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a AirTicketOrder record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "airticketorder_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _airTicketOrderService.ExportExcel(filterRules,sort, order );
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
