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
    public class FeeTypesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<FeeType>, Repository<FeeType>>();
        //container.RegisterType<IFeeTypeService, FeeTypeService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IFeeTypeService _feeTypeService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/FeeTypes";

        public FeeTypesController(IFeeTypeService feeTypeService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _feeTypeService = feeTypeService;
            _unitOfWork = unitOfWork;
        }

        // GET: FeeTypes/Index
        public ActionResult Index()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            //var feetype  = _feeTypeService.Queryable().AsQueryable();
            //return View(feetype  );
            return View();
        }

        // Get :FeeTypes/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var feetype = _feeTypeService.Query(new FeeTypeQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = feetype.Select(n => new
            {
                Id = n.Id,
                FeeCode = n.FeeCode,
                FeeName = n.FeeName,
                FeeEName = n.FeeEName,
                InspectionFee = n.InspectionFee,
                CustomsFee = n.CustomsFee,
                MathDate = n.MathDate,
                Description = n.Description,
                Status = n.Status,
                n.ECC_Code,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(FeeTypeChangeViewModel feetype)
        {
            if (feetype.updated != null)
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

                foreach (var updated in feetype.updated)
                {
                    _feeTypeService.Update(updated);
                }
            }
            if (feetype.deleted != null)
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

                foreach (var deleted in feetype.deleted)
                {
                    _feeTypeService.Delete(deleted);
                }
            }
            if (feetype.inserted != null)
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

                foreach (var inserted in feetype.inserted)
                {
                    _feeTypeService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((feetype.updated != null && feetype.updated.Any()) ||
                (feetype.deleted != null && feetype.deleted.Any()) ||
                (feetype.inserted != null && feetype.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(feetype);
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

        // GET: FeeTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeeType feeType = _feeTypeService.Find(id);
            if (feeType == null)
            {
                return HttpNotFound();
            }
            return View(feeType);
        }

        // GET: FeeTypes/Create
        public ActionResult Create()
        {
            FeeType feeType = new FeeType();
            //set default value
            return View(feeType);
        }

        // POST: FeeTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FeeCode,FeeName,FeeEName,InspectionFee,CustomsFee,MathDate,Description,Status,OperatingPoint,ADDWHO,ADDTS,EDITWHO,EDITTS")] FeeType feeType)
        {
            if (ModelState.IsValid)
            {
                _feeTypeService.Insert(feeType);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a FeeType record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(feeType);
        }

        // GET: FeeTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeeType feeType = _feeTypeService.Find(id);
            if (feeType == null)
            {
                return HttpNotFound();
            }
            return View(feeType);
        }

        // POST: FeeTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FeeCode,FeeName,FeeEName,InspectionFee,CustomsFee,MathDate,Description,Status,OperatingPoint,ADDWHO,ADDTS,EDITWHO,EDITTS")] FeeType feeType)
        {
            if (ModelState.IsValid)
            {
                feeType.ObjectState = ObjectState.Modified;
                _feeTypeService.Update(feeType);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a FeeType record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(feeType);
        }

        // GET: FeeTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeeType feeType = _feeTypeService.Find(id);
            if (feeType == null)
            {
                return HttpNotFound();
            }
            return View(feeType);
        }

        // POST: FeeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FeeType feeType = _feeTypeService.Find(id);
            _feeTypeService.Delete(feeType);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a FeeType record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "feetype_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _feeTypeService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取费用信息代码和名称
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerFeeTypes(int page = 1, int rows = 10, string q = "")
        {
            var feeTypeRep = _unitOfWork.RepositoryAsync<FeeType>();
            var data = feeTypeRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.FeeCode.Contains(q) || n.FeeName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.FeeCode, TEXT = n.FeeName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取费用信息代码和名称
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetPagerFeeTypes_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var FeeTypeRep = (IEnumerable<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
            var QFfeeType = FeeTypeRep.Where(x => x.FeeCode == null || x.FeeName == null);
            foreach (var item in QFfeeType)
            {
                item.FeeCode = item.FeeCode ?? "";
                item.FeeName = item.FeeName ?? "";
            }
            var data = FeeTypeRep;
            if (q != "")
            {
                data = data.Where(n => n.FeeCode.Contains(q) || n.FeeName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.FeeCode, TEXT = n.FeeName });
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
