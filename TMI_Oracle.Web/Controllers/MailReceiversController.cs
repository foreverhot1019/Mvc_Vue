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
    public class MailReceiversController : Controller
    {
        private readonly IMailReceiverService _mailReceiverService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/MailReceivers";

        public MailReceiversController(IMailReceiverService mailReceiverService, IUnitOfWorkAsync unitOfWork)
        {
            _mailReceiverService = mailReceiverService;
            _unitOfWork = unitOfWork;
        }

        // GET: MailReceivers/Index
        public ActionResult Index()
        {
            //需要重发的方法
            List<string> ArrTMMethod = new List<string>();
            string TMMethodJson = System.Configuration.ConfigurationManager.AppSettings["TMMethod"] ?? "";
            //需要重发的错误返回键值
            List<string> ArrTMErrKey = new List<string>();
            string TMErrKeyJson = System.Configuration.ConfigurationManager.AppSettings["TMErrKey"] ?? "";
            if (!string.IsNullOrWhiteSpace(TMMethodJson))
            {
                try
                {
                    ArrTMMethod = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(TMMethodJson);
                }
                catch (Exception)
                {

                }
            }
            if (!string.IsNullOrWhiteSpace(TMErrKeyJson))
            {
                try
                {
                    ArrTMErrKey = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(TMErrKeyJson);
                }
                catch (Exception)
                {

                }
            }
            ViewBag.ArrTMMethod = Newtonsoft.Json.JsonConvert.SerializeObject(ArrTMMethod.Select(x => new { id = x, text = x })).Replace("\"","'");
            ViewBag.ArrTMErrKey = Newtonsoft.Json.JsonConvert.SerializeObject(ArrTMErrKey.Select(x => new { id = x, text = x })).Replace("\"", "'");

            //var mailreceiver  = _mailReceiverService.Queryable().AsQueryable();
            //return View(mailreceiver  );
            return View();
        }

        // Get :MailReceivers/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var mailreceiver = _mailReceiverService.Query(new MailReceiverQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = mailreceiver.Select(n => new
            {
                ID = n.ID,
                ErrType = n.ErrType,
                ErrMethod = n.ErrMethod,
                RecMailAddress = n.RecMailAddress,
                CCMailAddress = n.CCMailAddress,
                ADDID = n.ADDID,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS,
                EDITID = n.EDITID,
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(MailReceiverChangeViewModel mailreceiver)
        {
            if (mailreceiver.updated != null)
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

                foreach (var updated in mailreceiver.updated)
                {
                    _mailReceiverService.Update(updated);
                }
            }
            if (mailreceiver.deleted != null)
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

                foreach (var deleted in mailreceiver.deleted)
                {
                    _mailReceiverService.Delete(deleted);
                }
            }
            if (mailreceiver.inserted != null)
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

                foreach (var inserted in mailreceiver.inserted)
                {
                    _mailReceiverService.Insert(inserted);
                }
            }
            try
            {
                if ((mailreceiver.updated != null && mailreceiver.updated.Any()) ||
                    (mailreceiver.deleted != null && mailreceiver.deleted.Any()) ||
                    (mailreceiver.inserted != null && mailreceiver.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: MailReceivers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailReceiver mailReceiver = _mailReceiverService.Find(id);
            if (mailReceiver == null)
            {
                return HttpNotFound();
            }
            return View(mailReceiver);
        }

        // GET: MailReceivers/Create
        public ActionResult Create()
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

            MailReceiver mailReceiver = new MailReceiver();
            //set default value
            return View(mailReceiver);
        }

        // POST: MailReceivers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ErrType,ErrMethod,RecMailAddress,CCMailAddress,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] MailReceiver mailReceiver)
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

            if (ModelState.IsValid)
            {
                _mailReceiverService.Insert(mailReceiver);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a MailReceiver record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(mailReceiver);
        }

        // GET: MailReceivers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailReceiver mailReceiver = _mailReceiverService.Find(id);
            if (mailReceiver == null)
            {
                return HttpNotFound();
            }
            return View(mailReceiver);
        }

        // POST: MailReceivers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ErrType,ErrMethod,RecMailAddress,CCMailAddress,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] MailReceiver mailReceiver)
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

            if (ModelState.IsValid)
            {
                mailReceiver.ObjectState = ObjectState.Modified;
                _mailReceiverService.Update(mailReceiver);

                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a MailReceiver record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(mailReceiver);
        }

        // GET: MailReceivers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailReceiver mailReceiver = _mailReceiverService.Find(id);
            if (mailReceiver == null)
            {
                return HttpNotFound();
            }
            return View(mailReceiver);
        }

        // POST: MailReceivers/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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

            MailReceiver mailReceiver = _mailReceiverService.Find(id);
            _mailReceiverService.Delete(mailReceiver);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a MailReceiver record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            #region

            var ControllActinMsg = "导出";
            bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Export", ControllActinMsg);
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

            var fileName = "MailReceiver_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _mailReceiverService.ExportExcel(filterRules, sort, order);
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
