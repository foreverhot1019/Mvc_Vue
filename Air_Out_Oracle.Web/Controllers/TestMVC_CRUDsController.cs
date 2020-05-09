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
    public class TestMVC_CRUDsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<TestMVC_CRUD>, Repository<TestMVC_CRUD>>();
        //container.RegisterType<ITestMVC_CRUDService, TestMVC_CRUDService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ITestMVC_CRUDService  _testMVC_CRUDService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/TestMVC_CRUDsController";

        public TestMVC_CRUDsController (ITestMVC_CRUDService  testMVC_CRUDService, IUnitOfWorkAsync unitOfWork)
        {
            _testMVC_CRUDService  = testMVC_CRUDService;
            _unitOfWork = unitOfWork;
        }

        // GET: TestMVC_CRUDs/Index
        public ActionResult Index()
        {
            //var testmvc_crud  = _testMVC_CRUDService.Queryable().AsQueryable();
            //return View(testmvc_crud  );
			return View();
        }

        // Get :TestMVC_CRUDs/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
			var testmvc_crud = _testMVC_CRUDService.Query(new TestMVC_CRUDQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            var datarows = testmvc_crud.Select( n => new {  
Id = n.Id, 
Dzbh = n.Dzbh, 
TestColumn1 = n.TestColumn1, 
TestColumn2 = n.TestColumn2, 
TestColumn3 = n.TestColumn3, 
TestColumn4 = n.TestColumn4, 
TestColumn5 = n.TestColumn5}).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(TestMVC_CRUDChangeViewModel testmvc_crud)
        {
            if (testmvc_crud.updated != null)
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

                foreach (var updated in testmvc_crud.updated)
                {
                    _testMVC_CRUDService.Update(updated);
                }
            }
            if (testmvc_crud.deleted != null)
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

                foreach (var deleted in testmvc_crud.deleted)
                {
                    _testMVC_CRUDService.Delete(deleted);
                }
            }
            if (testmvc_crud.inserted != null)
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

                foreach (var inserted in testmvc_crud.inserted)
                {
                    _testMVC_CRUDService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((testmvc_crud.updated != null && testmvc_crud.updated.Any()) || 
				(testmvc_crud.deleted != null && testmvc_crud.deleted.Any()) || 
				(testmvc_crud.inserted != null && testmvc_crud.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
					if(IsAutoResetCache)
						AutoResetCache(testmvc_crud);
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
		       
        // GET: TestMVC_CRUDs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestMVC_CRUD testMVC_CRUD = _testMVC_CRUDService.Find(id);
            if (testMVC_CRUD == null)
            {
                return HttpNotFound();
            }
            return View(testMVC_CRUD);
        }

        // GET: TestMVC_CRUDs/Create
        public ActionResult Create()
        {
            TestMVC_CRUD testMVC_CRUD = new TestMVC_CRUD();
            //set default value
            return View(testMVC_CRUD);
        }

        // POST: TestMVC_CRUDs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Dzbh,TestColumn1,TestColumn2,TestColumn3,TestColumn4,TestColumn5,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] TestMVC_CRUD testMVC_CRUD)
        {
            if (ModelState.IsValid)
            {
				_testMVC_CRUDService.Insert(testMVC_CRUD);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a TestMVC_CRUD record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(testMVC_CRUD);
        }

        // GET: TestMVC_CRUDs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestMVC_CRUD testMVC_CRUD = _testMVC_CRUDService.Find(id);
            if (testMVC_CRUD == null)
            {
                return HttpNotFound();
            }
            return View(testMVC_CRUD);
        }

        // POST: TestMVC_CRUDs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Dzbh,TestColumn1,TestColumn2,TestColumn3,TestColumn4,TestColumn5,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] TestMVC_CRUD testMVC_CRUD)
        {
            if (ModelState.IsValid)
            {
				testMVC_CRUD.ObjectState = ObjectState.Modified;
				_testMVC_CRUDService.Update(testMVC_CRUD);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a TestMVC_CRUD record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(testMVC_CRUD);
        }

        // GET: TestMVC_CRUDs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestMVC_CRUD testMVC_CRUD = _testMVC_CRUDService.Find(id);
            if (testMVC_CRUD == null)
            {
                return HttpNotFound();
            }
            return View(testMVC_CRUD);
        }

        // POST: TestMVC_CRUDs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestMVC_CRUD testMVC_CRUD = _testMVC_CRUDService.Find(id);
            _testMVC_CRUDService.Delete(testMVC_CRUD);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a TestMVC_CRUD record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "testmvc_crud_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _testMVC_CRUDService.ExportExcel(filterRules,sort, order );
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
