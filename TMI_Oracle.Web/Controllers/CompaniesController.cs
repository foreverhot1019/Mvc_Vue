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
    public class CompaniesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Company>, Repository<Company>>();
        //container.RegisterType<ICompanyService, CompanyService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ICompanyService _companyService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/Companies";

        public CompaniesController(ICompanyService companyService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _companyService = companyService;
            _unitOfWork = unitOfWork;
        }

        // GET: Companies/Index
        public ActionResult Index()
        {
            //var company  = _companyService.Queryable().AsQueryable();
            //return View(company  );
            return View();
        }

        // Get :Companies/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var company = _companyService.Query(new CompanyQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = company.Select(n => new
            {
                Id = n.Id,
                CIQID = n.CIQID,
                Name = n.Name,
                SimpleName = n.SimpleName,
                Eng_Name = n.Eng_Name,
                Address = n.Address,
                City = n.City,
                Province = n.Province,
                RegisterDate = n.RegisterDate,
                Logo = n.Logo,
                ContractStart = n.ContractStart,
                ContractEnd = n.ContractEnd,
                AuthorizeAmount = n.AuthorizeAmount,
                Currency = n.Currency,
                CheckBillDate = n.CheckBillDate,
                PayPalDate = n.PayPalDate,
                InvoiceType = n.InvoiceType,
                Remark = n.Remark,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                n.ADDID,
                n.ADDTS,
                n.ADDWHO,
                n.EDITID,
                n.EDITTS,
                n.EDITWHO,
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(CompanyChangeViewModel company)
        {
            if (company.updated != null)
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

                foreach (var updated in company.updated)
                {
                    _companyService.Update(updated);
                }
            }
            if (company.deleted != null)
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

                foreach (var deleted in company.deleted)
                {
                    _companyService.Delete(deleted);
                }
            }
            if (company.inserted != null)
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

                foreach (var inserted in company.inserted)
                {
                    _companyService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((company.updated != null && company.updated.Any()) ||
                (company.deleted != null && company.deleted.Any()) ||
                (company.inserted != null && company.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(company);
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

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _companyService.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            Company company = new Company();
            //set default value
            return View(company);
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CIQID,Name,SimpleName,Eng_Name,Address,City,Province,RegisterDate,Logo,ContractStart,ContractEnd,AuthorizeAmount,Currency,CheckBillDate,PayPalDate,InvoiceType,Remark,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Company company)
        {
            if (ModelState.IsValid)
            {
                _companyService.Insert(company);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Company record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _companyService.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CIQID,Name,SimpleName,Eng_Name,Address,City,Province,RegisterDate,Logo,ContractStart,ContractEnd,AuthorizeAmount,Currency,CheckBillDate,PayPalDate,InvoiceType,Remark,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Company company)
        {
            if (ModelState.IsValid)
            {
                company.ObjectState = ObjectState.Modified;
                _companyService.Update(company);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Company record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _companyService.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = _companyService.Find(id);
            _companyService.Delete(company);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Company record");
            return RedirectToAction("Index");
        }

        public ActionResult GetPagerCompany_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var CompanyRep = (IEnumerable<Company>)CacheHelper.Get_SetCache(Common.CacheNameS.Company);
            var QCompany = CompanyRep.Where(x => x.CIQID == null || x.Name == null);
            if (QCompany.Any())
            {
                foreach (var item in QCompany)
                {
                    item.CIQID = item.CIQID ?? "";
                    item.Name = item.Name ?? "";
                }
            }

            if (q != "")
            {
                CompanyRep = CompanyRep.Where(n => n.CIQID.Contains(q) || n.Name.Contains(q));
            }
            var list = CompanyRep.Select(n => new { ID = n.CIQID, TEXT = n.Name });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "company_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _companyService.ExportExcel(filterRules, sort, order);
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
