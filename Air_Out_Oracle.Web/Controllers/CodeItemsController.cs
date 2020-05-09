


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
    public class CodeItemsController : Controller
    {
        
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<CodeItem>, Repository<CodeItem>>();
        //container.RegisterType<ICodeItemService, CodeItemService>();
        
        //private WebdbContext db = new WebdbContext();
        private readonly ICodeItemService  _codeItemService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public CodeItemsController (ICodeItemService  codeItemService, IUnitOfWorkAsync unitOfWork)
        {
            _codeItemService  = codeItemService;
            _unitOfWork = unitOfWork;
        }

        // GET: CodeItems/Index
        public ActionResult Index()
        {
            
            //var codeitems  = _codeItemService.Queryable().Include(c => c.BaseCode).AsQueryable();
            
             //return View(codeitems);
			 return View();
        }

        // Get :CodeItems/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            			 
            var codeitems  = _codeItemService.Query(new CodeItemQuery().Withfilter(filters)).Include(c => c.BaseCode).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            
                        var datarows = codeitems .Select(  n => new { BaseCodeCodeType = (n.BaseCode==null?"": n.BaseCode.Description) , Id = n.Id , Code = n.Code , Text = n.Text , Description = n.Description , BaseCodeId = n.BaseCodeId }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }


        //获取基础代码数据项
        public ActionResult GetItems(string id) {
            var items =   _codeItemService.GetByCodeType(id);
            var data = items.Select(x => new { Code = x.Code, Text = x.Text }).ToList();
            data.Insert(0, new { Code = "", Text = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(CodeItemChangeViewModel codeitems)
        {
            if (codeitems.updated != null)
            {
                foreach (var updated in codeitems.updated)
                {
                    _codeItemService.Update(updated);
                }
            }
            if (codeitems.deleted != null)
            {
                foreach (var deleted in codeitems.deleted)
                {
                    _codeItemService.Delete(deleted);
                }
            }
            if (codeitems.inserted != null)
            {
                foreach (var inserted in codeitems.inserted)
                {
                    _codeItemService.Insert(inserted);
                }
            }
            _unitOfWork.SaveChanges();

            return Json(new {Success=true}, JsonRequestBehavior.AllowGet);
        }

				public ActionResult GetBaseCodes()
        {
            var basecodeRepository = _unitOfWork.Repository<BaseCode>();
            var data = basecodeRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, CodeType = n.CodeType, Name = n.Description });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
		
		
       
        // GET: CodeItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodeItem codeItem = _codeItemService.Find(id);
            if (codeItem == null)
            {
                return HttpNotFound();
            }
            return View(codeItem);
        }
        

        // GET: CodeItems/Create
        public ActionResult Create()
        {
            CodeItem codeItem = new CodeItem();
            //set default value
            var basecodeRepository = _unitOfWork.Repository<BaseCode>();
            ViewBag.BaseCodeId = new SelectList(basecodeRepository.Queryable(), "Id", "CodeType");
            return View(codeItem);
        }

        // POST: CodeItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BaseCode,Id,Code,Text,Description,BaseCodeId")] CodeItem codeItem)
        {
            if (ModelState.IsValid)
            {
             				_codeItemService.Insert(codeItem);
                           _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a CodeItem record");
                return RedirectToAction("Index");
            }

            var basecodeRepository = _unitOfWork.Repository<BaseCode>();
            ViewBag.BaseCodeId = new SelectList(basecodeRepository.Queryable(), "Id", "CodeType", codeItem.BaseCodeId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(codeItem);
        }

        // GET: CodeItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodeItem codeItem = _codeItemService.Find(id);
            if (codeItem == null)
            {
                return HttpNotFound();
            }
            var basecodeRepository = _unitOfWork.Repository<BaseCode>();
            ViewBag.BaseCodeId = new SelectList(basecodeRepository.Queryable(), "Id", "CodeType", codeItem.BaseCodeId);
            return View(codeItem);
        }

        // POST: CodeItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BaseCode,Id,Code,Text,Description,BaseCodeId")] CodeItem codeItem)
        {
            if (ModelState.IsValid)
            {
                codeItem.ObjectState = ObjectState.Modified;
                				_codeItemService.Update(codeItem);
                                
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a CodeItem record");
                return RedirectToAction("Index");
            }
            var basecodeRepository = _unitOfWork.Repository<BaseCode>();
            ViewBag.BaseCodeId = new SelectList(basecodeRepository.Queryable(), "Id", "CodeType", codeItem.BaseCodeId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(codeItem);
        }

        // GET: CodeItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodeItem codeItem = _codeItemService.Find(id);
            if (codeItem == null)
            {
                return HttpNotFound();
            }
            return View(codeItem);
        }

        // POST: CodeItems/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CodeItem codeItem =  _codeItemService.Find(id);
             _codeItemService.Delete(codeItem);
            _unitOfWork.SaveChanges();
           if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            DisplaySuccessMessage("Has delete a CodeItem record");
            return RedirectToAction("Index");
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
        {
            var fileName = "codeitems_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream=  _codeItemService.ExportExcel(filterRules,sort, order );
            return File(stream, "application/vnd.ms-excel", fileName);
      
        }

        //获取基础信息代码和名称
        public ActionResult GetCodeItem(int page = 1, int rows = 10, string q = "", int id = 0)
        {
            var codeItemRep = _unitOfWork.RepositoryAsync<CodeItem>();
            var data = codeItemRep.Queryable().Where(x => x.BaseCodeId == id);

            if (q != "")
            {
                data = data.Where(n => n.Code.Contains(q) || n.Code.Contains(q));
            }
            var list = data.Select(n => new { ID = n.Code, TEXT = n.Text });
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
