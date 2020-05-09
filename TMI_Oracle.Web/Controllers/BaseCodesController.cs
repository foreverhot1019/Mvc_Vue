﻿




//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Repository.Pattern.UnitOfWork;
//using Repository.Pattern.Infrastructure;
//using Newtonsoft.Json;
//using TMI.Web.Models;
//using TMI.Web.Services;
//using TMI.Web.Repositories;
//using TMI.Web.Extensions;


//namespace TMI.Web.Controllers
//{
//    public class BaseCodesController : Controller
//    {

//        //Please RegisterType UnityConfig.cs
//        //container.RegisterType<IRepositoryAsync<BaseCode>, Repository<BaseCode>>();
//        //container.RegisterType<IBaseCodeService, BaseCodeService>();

//        //private StoreContext db = new StoreContext();
//        private readonly IBaseCodeService _baseCodeService;
//        private readonly IUnitOfWorkAsync _unitOfWork;

//        public BaseCodesController(IBaseCodeService baseCodeService, IUnitOfWorkAsync unitOfWork)
//        {
//            _baseCodeService = baseCodeService;
//            _unitOfWork = unitOfWork;
//        }

//        // GET: BaseCodes/Index
//        public ActionResult Index()
//        {

//            //var basecodes  = _baseCodeService.Queryable().AsQueryable();
//            //return View(basecodes  );
//            return View();
//        }

//        // Get :BaseCodes/PageList
//        // For Index View Boostrap-Table load  data 
//        [HttpGet]
//        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
//        {
//            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
//            int totalCount = 0;
//            //int pagenum = offset / limit +1;
//            var basecodes = _baseCodeService.Query(new BaseCodeQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
//            var datarows = basecodes.Select(n => new { Id = n.Id, CodeType = n.CodeType, Description = n.Description }).ToList();
//            var pagelist = new { total = totalCount, rows = datarows };
//            return Json(pagelist, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public ActionResult SaveData(BaseCodeChangeViewModel basecodes)
//        {
//            if (basecodes.updated != null)
//            {
//                foreach (var updated in basecodes.updated)
//                {
//                    _baseCodeService.Update(updated);
//                }
//            }
//            if (basecodes.deleted != null)
//            {
//                foreach (var deleted in basecodes.deleted)
//                {
//                    _baseCodeService.Delete(deleted);
//                }
//            }
//            if (basecodes.inserted != null)
//            {
//                foreach (var inserted in basecodes.inserted)
//                {
//                    _baseCodeService.Insert(inserted);
//                }
//            }
//            _unitOfWork.SaveChanges();

//            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
//        }


//        public ActionResult GetBaseCodes()
//        {
//            var basecodeRepository = _unitOfWork.Repository<BaseCode>();
//            var data = basecodeRepository.Queryable().ToList();
//            var rows = data.Select(n => new { Id = n.Id, CodeType = n.CodeType });
//            return Json(rows, JsonRequestBehavior.AllowGet);
//        }


//        // GET: BaseCodes/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            BaseCode baseCode = _baseCodeService.Find(id);
//            if (baseCode == null)
//            {
//                return HttpNotFound();
//            }
//            return View(baseCode);
//        }


//        // GET: BaseCodes/Create
//        public ActionResult Create()
//        {
//            BaseCode baseCode = new BaseCode();
//            //set default value
//            return View(baseCode);
//        }

//        // POST: BaseCodes/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "CodeItems,Id,CodeType,Description")] BaseCode baseCode)
//        {
//            if (ModelState.IsValid)
//            {
//                baseCode.ObjectState = ObjectState.Added;
//                foreach (var item in baseCode.CodeItems)
//                {
//                    item.BaseCodeId = baseCode.Id;
//                    item.ObjectState = ObjectState.Added;
//                }
//                _baseCodeService.InsertOrUpdateGraph(baseCode);
//                _unitOfWork.SaveChanges();
//                if (Request.IsAjaxRequest())
//                {
//                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
//                }
//                DisplaySuccessMessage("Has append a BaseCode record");
//                return RedirectToAction("Index");
//            }

//            if (Request.IsAjaxRequest())
//            {
//                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
//                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
//            }
//            DisplayErrorMessage();
//            return View(baseCode);
//        }

//        // GET: BaseCodes/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            BaseCode baseCode = _baseCodeService.Find(id);
//            if (baseCode == null)
//            {
//                return HttpNotFound();
//            }
//            return View(baseCode);
//        }

//        // POST: BaseCodes/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "CodeItems,Id,CodeType,Description")] BaseCode baseCode)
//        {
//            if (ModelState.IsValid)
//            {
//                baseCode.ObjectState = ObjectState.Modified;
//                foreach (var item in baseCode.CodeItems)
//                {
//                    item.BaseCodeId = baseCode.Id;
//                    //set ObjectState with conditions
//                    if (item.Id <= 0)
//                        item.ObjectState = ObjectState.Added;
//                    else
//                        item.ObjectState = ObjectState.Modified;
//                }

//                _baseCodeService.InsertOrUpdateGraph(baseCode);

//                _unitOfWork.SaveChanges();
//                if (Request.IsAjaxRequest())
//                {
//                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
//                }
//                DisplaySuccessMessage("Has update a BaseCode record");
//                return RedirectToAction("Index");
//            }
//            if (Request.IsAjaxRequest())
//            {
//                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
//                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
//            }
//            DisplayErrorMessage();
//            return View(baseCode);
//        }

//        // GET: BaseCodes/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            BaseCode baseCode = _baseCodeService.Find(id);
//            if (baseCode == null)
//            {
//                return HttpNotFound();
//            }
//            return View(baseCode);
//        }

//        // POST: BaseCodes/Delete/5
//        [HttpPost, ActionName("Delete")]
//        //[ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            BaseCode baseCode = _baseCodeService.Find(id);
//            _baseCodeService.Delete(baseCode);
//            _unitOfWork.SaveChanges();
//            if (Request.IsAjaxRequest())
//            {
//                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
//            }
//            DisplaySuccessMessage("Has delete a BaseCode record");
//            return RedirectToAction("Index");
//        }


//        // Get Detail Row By Id For Edit
//        // Get : BaseCodes/EditCodeItem/:id
//        [HttpGet]
//        public ActionResult EditCodeItem(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            var codeitemRepository = _unitOfWork.Repository<CodeItem>();
//            var codeitem = codeitemRepository.Find(id);

//            var basecodeRepository = _unitOfWork.Repository<BaseCode>();

//            if (codeitem == null)
//            {
//                ViewBag.BaseCodeId = new SelectList(basecodeRepository.Queryable(), "Id", "CodeType");

//                //return HttpNotFound();
//                return PartialView("_CodeItemEditForm", new CodeItem());
//            }
//            else
//            {
//                ViewBag.BaseCodeId = new SelectList(basecodeRepository.Queryable(), "Id", "CodeType", codeitem.BaseCodeId);

//            }
//            return PartialView("_CodeItemEditForm", codeitem);

//        }

//        // Get Create Row By Id For Edit
//        // Get : BaseCodes/CreateCodeItem
//        [HttpGet]
//        public ActionResult CreateCodeItem()
//        {
//            var basecodeRepository = _unitOfWork.Repository<BaseCode>();
//            ViewBag.BaseCodeId = new SelectList(basecodeRepository.Queryable(), "Id", "CodeType");
//            return PartialView("_CodeItemEditForm");

//        }

//        // Post Delete Detail Row By Id
//        // Get : BaseCodes/DeleteCodeItem/:id
//        [HttpPost, ActionName("DeleteCodeItem")]
//        public ActionResult DeleteCodeItemConfirmed(int id)
//        {
//            var codeitemRepository = _unitOfWork.Repository<CodeItem>();
//            codeitemRepository.Delete(id);
//            _unitOfWork.SaveChanges();
//            if (Request.IsAjaxRequest())
//            {
//                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
//            }
//            DisplaySuccessMessage("Has delete a Order record");
//            return RedirectToAction("Index");
//        }



//        // Get : BaseCodes/GetCodeItemsByBaseCodeId/:id
//        [HttpGet]
//        public ActionResult GetCodeItemsByBaseCodeId(int id)
//        {
//            var codeitems = _baseCodeService.GetCodeItemsByBaseCodeId(id);
//            if (Request.IsAjaxRequest())
//            {
//                return Json((codeitems==null?null:codeitems.Select(n => new { BaseCodeCodeType = (n.BaseCode == null ? "" : n.BaseCode.CodeType), Id = n.Id, Code = n.Code, Text = n.Text, BaseCodeId = n.BaseCodeId })), JsonRequestBehavior.AllowGet);
//            }
//            return View(codeitems);

//        }


//        private void DisplaySuccessMessage(string msgText)
//        {
//            TempData["SuccessMessage"] = msgText;
//        }

//        private void DisplayErrorMessage()
//        {
//            TempData["ErrorMessage"] = "Save changes was unsuccessful.";
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                _unitOfWork.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
