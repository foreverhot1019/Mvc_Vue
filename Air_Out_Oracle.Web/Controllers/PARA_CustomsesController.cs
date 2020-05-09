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
    public class PARA_CustomsesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<PARA_Customs>, Repository<PARA_Customs>>();
        //container.RegisterType<IPARA_CustomsService, PARA_CustomsService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPARA_CustomsService _pARA_CustomsService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/PARA_Customses";

        public PARA_CustomsesController(IPARA_CustomsService pARA_CustomsService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _pARA_CustomsService = pARA_CustomsService;
            _unitOfWork = unitOfWork;
        }

        // GET: PARA_Customses/Index
        public ActionResult Index()
        {
            //var para_customs  = _pARA_CustomsService.Queryable().AsQueryable();
            //return View(para_customs  );
            return View();
        }

        // Get :PARA_Customses/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Customs_Code", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var para_customs = _pARA_CustomsService.Query(new PARA_CustomsQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = para_customs.Select(n => new
            {
                Customs_Code = n.Customs_Code,
                Customs_Name = n.Customs_Name,
                PinYinSimpleName = n.PinYinSimpleName,
                Description = n.Description,
                Status = n.Status,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(PARA_CustomsChangeViewModel para_customs)
        {
            if (para_customs.updated != null)
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

                foreach (var updated in para_customs.updated)
                {
                    _pARA_CustomsService.Update(updated);
                }
            }
            if (para_customs.deleted != null)
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

                foreach (var deleted in para_customs.deleted)
                {
                    _pARA_CustomsService.Delete(deleted);
                }
            }
            if (para_customs.inserted != null)
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

                foreach (var inserted in para_customs.inserted)
                {
                    _pARA_CustomsService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((para_customs.updated != null && para_customs.updated.Any()) ||
                (para_customs.deleted != null && para_customs.deleted.Any()) ||
                (para_customs.inserted != null && para_customs.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(para_customs);
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

        // GET: PARA_Customses/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Customs pARA_Customs = _pARA_CustomsService.Find(id);
            if (pARA_Customs == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Customs);
        }

        // GET: PARA_Customses/Create
        public ActionResult Create()
        {
            PARA_Customs pARA_Customs = new PARA_Customs();
            //set default value
            return View(pARA_Customs);
        }

        // POST: PARA_Customses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Customs_Code,Customs_Name,PinYinSimpleName,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_Customs pARA_Customs)
        {
            if (ModelState.IsValid)
            {
                _pARA_CustomsService.Insert(pARA_Customs);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a PARA_Customs record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_Customs);
        }

        // GET: PARA_Customses/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Customs pARA_Customs = _pARA_CustomsService.Find(id);
            if (pARA_Customs == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Customs);
        }

        // POST: PARA_Customses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Customs_Code,Customs_Name,PinYinSimpleName,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_Customs pARA_Customs)
        {
            if (ModelState.IsValid)
            {
                pARA_Customs.ObjectState = ObjectState.Modified;
                _pARA_CustomsService.Update(pARA_Customs);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a PARA_Customs record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_Customs);
        }

        // GET: PARA_Customses/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Customs pARA_Customs = _pARA_CustomsService.Find(id);
            if (pARA_Customs == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Customs);
        }

        // POST: PARA_Customses/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PARA_Customs pARA_Customs = _pARA_CustomsService.Find(id);
            _pARA_CustomsService.Delete(pARA_Customs);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a PARA_Customs record");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 获取关区
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPARA_Customs(int page = 1, int rows = 10, string q = "")
        {
            var para_CustomsRep = _unitOfWork.RepositoryAsync<PARA_Customs>();
            var data = para_CustomsRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.Customs_Code.Contains(q) || n.Customs_Name.Contains(q) || n.PinYinSimpleName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.Customs_Code, TEXT = n.Customs_Name, Name = n.Customs_Name, PinYinSName = n.PinYinSimpleName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取关区
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPARA_Customs_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var para_CustomsRep = (IEnumerable<PARA_Customs>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Customs);
            var Qpara_CustomsRep = para_CustomsRep.Where(x => x.Customs_Code == null || x.Customs_Name == null || x.PinYinSimpleName == null);
            if (Qpara_CustomsRep.Any())
            {
                foreach (var item in Qpara_CustomsRep)
                {
                    item.Customs_Code = item.Customs_Code ?? "";
                    item.Customs_Name = item.Customs_Name ?? "";
                    item.PinYinSimpleName = item.PinYinSimpleName ?? "";
                }
            }
            if (q != "")
            {
                para_CustomsRep = para_CustomsRep.Where(n => n.Customs_Code.Contains(q) || n.Customs_Name.Contains(q) || n.PinYinSimpleName.Contains(q));
            }
            var list = para_CustomsRep.Select(n => new { ID = n.Customs_Code, TEXT = n.Customs_Name, Name = n.Customs_Name, PinYinSName = n.PinYinSimpleName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "para_customs_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _pARA_CustomsService.ExportExcel(filterRules, sort, order);
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
