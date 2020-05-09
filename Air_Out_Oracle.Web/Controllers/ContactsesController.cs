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
    public class ContactsesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Contacts>, Repository<Contacts>>();
        //container.RegisterType<IContactsService, ContactsService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IContactsService  _contactsService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/OPS_M_Orders";

        public ContactsesController (IContactsService  contactsService, IUnitOfWorkAsync unitOfWork)
        {
            _contactsService  = contactsService;
            _unitOfWork = unitOfWork;
            
        }

        // GET: Contactses/Index
        public ActionResult Index()
        {
            //var contacts  = _contactsService.Queryable().AsQueryable();
            //return View(contacts  );
            
            //string input = "1\r\n2\r\n3\r\n4";
            //System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("(.*)\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //var ArrMatch = rgx.Matches(input);
            //foreach(System.Text.RegularExpressions.Match imatch in ArrMatch){
            //    //imatch.Groups[1] 就是 Regex里的值（）
            //    foreach (System.Text.RegularExpressions.Group gp in imatch.Groups)
            //    {
            //        var ss = gp.Value;
            //    }
            //}

			return View();
        }

        // Get :Contactses/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
			var contacts = _contactsService.Query(new ContactsQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            var datarows = contacts.Select( n => new {  
                Id = n.Id, 
                CusBusInfoId = n.CusBusInfoId, 
                CompanyName = n.CompanyName, 
                CompanyCode = n.CompanyCode, 
                CoAddress = n.CoAddress, 
                CoArea = n.CoArea, 
                CoCountry = n.CoCountry, 
                Contact = n.Contact, 
                ContactInfo = n.ContactInfo, 
                Status = n.Status, 
                Remark = n.Remark, 
                OperatingPoint = n.OperatingPoint}).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(ContactsChangeViewModel contacts)
        {
            if (contacts.updated != null)
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

                foreach (var updated in contacts.updated)
                {
                    _contactsService.Update(updated);
                }
            }
            if (contacts.deleted != null)
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

                foreach (var deleted in contacts.deleted)
                {
                    _contactsService.Delete(deleted);
                }
            }
            if (contacts.inserted != null)
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

                foreach (var inserted in contacts.inserted)
                {
                    _contactsService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((contacts.updated != null && contacts.updated.Any()) || 
				(contacts.deleted != null && contacts.deleted.Any()) || 
				(contacts.inserted != null && contacts.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(contacts);
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
		       
        // GET: Contactses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contacts contacts = _contactsService.Find(id);
            if (contacts == null)
            {
                return HttpNotFound();
            }
            return View(contacts);
        }

        // GET: Contactses/Create
        public ActionResult Create()
        {
            Contacts contacts = new Contacts();
            //set default value
            return View(contacts);
        }

        // POST: Contactses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CusBusInfoId,CompanyName,CompanyCode,CoAddress,CoArea,CoCountry,Contact,ContactInfo,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Contacts contacts)
        {
            if (ModelState.IsValid)
            {
				_contactsService.Insert(contacts);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Contacts record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(contacts);
        }

        // GET: Contactses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contacts contacts = _contactsService.Find(id);
            if (contacts == null)
            {
                return HttpNotFound();
            }
            return View(contacts);
        }

        // POST: Contactses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CusBusInfoId,CompanyName,CompanyCode,CoAddress,CoArea,CoCountry,Contact,ContactInfo,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Contacts contacts)
        {
            if (ModelState.IsValid)
            {
				contacts.ObjectState = ObjectState.Modified;
				_contactsService.Update(contacts);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Contacts record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(contacts);
        }

        // GET: Contactses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contacts contacts = _contactsService.Find(id);
            if (contacts == null)
            {
                return HttpNotFound();
            }
            return View(contacts);
        }

        // POST: Contactses/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contacts contacts = _contactsService.Find(id);
            _contactsService.Delete(contacts);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Contacts record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "contacts_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _contactsService.ExportExcel(filterRules,sort, order );
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
