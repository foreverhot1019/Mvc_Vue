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
    public class BD_DEFDOC_LISTsController : BaseController
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<BD_DEFDOC_LIST>, Repository<BD_DEFDOC_LIST>>();
        //container.RegisterType<IBD_DEFDOC_LISTService, BD_DEFDOC_LISTService>();

        //private WebdbContext db = new WebdbContext();
        private readonly IBD_DEFDOC_LISTService _bD_DEFDOC_LISTService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/BD_DEFDOCs";

        public BD_DEFDOC_LISTsController(IBD_DEFDOC_LISTService bD_DEFDOC_LISTService, IUnitOfWorkAsync unitOfWork)
        {
            _bD_DEFDOC_LISTService = bD_DEFDOC_LISTService;
            _unitOfWork = unitOfWork;
        }

        // GET: BD_DEFDOC_LISTs/Index
        public ActionResult Index()
        {
            //return View(bd_defdoc_list);
            return View();
        }

        // Get :BD_DEFDOC_LISTs/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;

            var bd_defdoc_list = _bD_DEFDOC_LISTService.Query(new BD_DEFDOC_LISTQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);

            var datarows = bd_defdoc_list.Select(n => new
            {
                ID = n.ID,
                n.DOCCODE,
                DOCID = n.DOCID,
                LISTCODE = n.LISTCODE,
                LISTNAME = n.LISTNAME,
                ListFullName = n.ListFullName,
                R_CODE = n.R_CODE,
                ENAME = n.ENAME,
                n.SortNum,
                REMARK = n.REMARK,
                STATUS = n.STATUS,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS,
                OperatingPoint = n.OperatingPoint
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(BD_DEFDOC_LISTChangeViewModel bd_defdoc_list)
        {
            if (bd_defdoc_list.updated != null)
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

                foreach (var updated in bd_defdoc_list.updated)
                {
                    var data = _unitOfWork.Repository<BD_DEFDOC>().Query(x => x.ID.Equals(updated.DOCID)).Select(c => new { DOCCODE = c.DOCCODE });
                    updated.DOCCODE = data.ToList().First().DOCCODE;

                    _bD_DEFDOC_LISTService.Update(updated);
                }
            }
            if (bd_defdoc_list.deleted != null)
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

                foreach (var deleted in bd_defdoc_list.deleted)
                {
                    _bD_DEFDOC_LISTService.Delete(deleted);
                }
            }
            if (bd_defdoc_list.inserted != null)
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

                foreach (var inserted in bd_defdoc_list.inserted)
                {
                    var data = _unitOfWork.Repository<BD_DEFDOC>().Query(x => x.ID.Equals(inserted.DOCID)).Select(c => new { DOCCODE = c.DOCCODE });
                    inserted.DOCCODE = data.ToList().First().DOCCODE;
                    _bD_DEFDOC_LISTService.Insert(inserted);
                }
            }

            try
            {
                if ((bd_defdoc_list.updated != null && bd_defdoc_list.updated.Any()) ||
                    (bd_defdoc_list.deleted != null && bd_defdoc_list.deleted.Any()) ||
                    (bd_defdoc_list.inserted != null && bd_defdoc_list.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    AutoResetCache(bd_defdoc_list);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 自动更新 缓存
        /// </summary>
        /// <param name="bd_cusservices"></param>
        public void AutoResetCache(BD_DEFDOC_LISTChangeViewModel ChangView)
        {
            Common.CacheNameS OCacheName = Common.CacheNameS.BD_DEFDOC_LIST;
            try
            {
                if ((ChangView.updated != null && ChangView.updated.Any()) || (ChangView.deleted != null && ChangView.deleted.Any()) || (ChangView.inserted != null && ChangView.inserted.Any()))
                {
                    CacheHelper.AutoResetCache(ChangView);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = OCacheName + "-更新缓存错误:" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Cache/BD_DEFDOC_LIST", true);
            }
        }

        public ActionResult GetBD_DEFDOC()
        {
            var bd_defdocRepository = _unitOfWork.Repository<BD_DEFDOC>();
            var data = bd_defdocRepository.Queryable().ToList();
            var rows = data.Select(n => new
            {
                ID = n.ID,
                DOCCODE = n.DOCCODE
            });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_OWNER_CODENAME(string OWNER_CODE = "", string SHIPPERCODE = "")
        {
            string result = "";
            if (!string.IsNullOrEmpty(OWNER_CODE) && !string.IsNullOrEmpty(SHIPPERCODE))
            {
                var BD_DEFDOCQuery = _unitOfWork.Repository<BD_DEFDOC_LIST>().Queryable().Where(x => x.STATUS == "1");
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.DOCCODE == "OWNER_CODE" && x.LISTCODE == OWNER_CODE && x.ENAME == SHIPPERCODE).OrderBy(x => x.DOCCODE);
                if (BD_DEFDOCQuery.Count() > 0)
                {
                    result = BD_DEFDOCQuery.FirstOrDefault().LISTNAME;
                }

            }
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        // GET: BD_DEFDOC_LISTs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_DEFDOC_LIST bD_DEFDOC_LIST = _bD_DEFDOC_LISTService.Find(id);
            if (bD_DEFDOC_LIST == null)
            {
                return HttpNotFound();
            }
            return View(bD_DEFDOC_LIST);
        }

        // GET: BD_DEFDOC_LISTs/Create
        public ActionResult Create()
        {
            BD_DEFDOC_LIST bD_DEFDOC_LIST = new BD_DEFDOC_LIST();
            var bd_defdocRepository = _unitOfWork.Repository<BD_DEFDOC>();
            ViewBag.DOCID = new SelectList(bd_defdocRepository.Queryable(), "ID", "DOCCODE");
            return View(bD_DEFDOC_LIST);
        }

        // POST: BD_DEFDOC_LISTs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BD_DEFDOC,ID,DOCID,LISTCODE,LISTNAME,ListFullName,REMARK,STATUS,ADDWHO,ADDTS,EDITWHO,EDITTS,R_CODE,ENAME,OperatingPoint")] BD_DEFDOC_LIST bD_DEFDOC_LIST)
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
                _bD_DEFDOC_LISTService.Insert(bD_DEFDOC_LIST);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a BD_DEFDOC_LIST record");
                return RedirectToAction("Index");
            }

            var bd_defdocRepository = _unitOfWork.Repository<BD_DEFDOC>();
            ViewBag.DOCID = new SelectList(bd_defdocRepository.Queryable(), "ID", "DOCCODE", bD_DEFDOC_LIST.DOCID);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bD_DEFDOC_LIST);
        }

        // GET: BD_DEFDOC_LISTs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_DEFDOC_LIST bD_DEFDOC_LIST = _bD_DEFDOC_LISTService.Find(id);
            if (bD_DEFDOC_LIST == null)
            {
                return HttpNotFound();
            }
            var bd_defdocRepository = _unitOfWork.Repository<BD_DEFDOC>();
            ViewBag.DOCID = new SelectList(bd_defdocRepository.Queryable(), "ID", "DOCCODE", bD_DEFDOC_LIST.DOCID);
            return View(bD_DEFDOC_LIST);
        }

        // POST: BD_DEFDOC_LISTs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BD_DEFDOC,ID,DOCID,LISTCODE,LISTNAME,ListFullName,REMARK,STATUS,ADDWHO,ADDTS,EDITWHO,EDITTS,R_CODE,ENAME,OperatingPoint")] BD_DEFDOC_LIST bD_DEFDOC_LIST)
        {
            #region

            var ControllActinMsg = "编辑";
            bool IsHaveQx = Common.ModelIsHaveQX("/BD_AGENCIESs", "Edit", ControllActinMsg);
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
                bD_DEFDOC_LIST.ObjectState = ObjectState.Modified;
                _bD_DEFDOC_LISTService.Update(bD_DEFDOC_LIST);

                //_unitOfWork.SaveChanges();
                //if (Request.IsAjaxRequest())
                //{
                //    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                //}
                try
                {
                    _unitOfWork.SaveChanges();
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }

                DisplaySuccessMessage("Has update a BD_DEFDOC_LIST record");
                return RedirectToAction("Index");
            }
            var bd_defdocRepository = _unitOfWork.Repository<BD_DEFDOC>();
            ViewBag.DOCID = new SelectList(bd_defdocRepository.Queryable(), "ID", "DOCCODE", bD_DEFDOC_LIST.DOCID);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(bD_DEFDOC_LIST);
        }

        // GET: BD_DEFDOC_LISTs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_DEFDOC_LIST bD_DEFDOC_LIST = _bD_DEFDOC_LISTService.Find(id);
            if (bD_DEFDOC_LIST == null)
            {
                return HttpNotFound();
            }
            return View(bD_DEFDOC_LIST);
        }

        // POST: BD_DEFDOC_LISTs/Delete/5
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

            BD_DEFDOC_LIST bD_DEFDOC_LIST = _bD_DEFDOC_LISTService.Find(id);
            _bD_DEFDOC_LISTService.Delete(bD_DEFDOC_LIST);

            //_unitOfWork.SaveChanges();
            //if (Request.IsAjaxRequest())
            //{
            //    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            //}
            try
            {
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            DisplaySuccessMessage("Has delete a BD_DEFDOC_LIST record");
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

            var fileName = "bd_defdoc_list_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _bD_DEFDOC_LISTService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 枚举数据字典
        /// </summary>
        /// <param name="DOCCODE"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult Get_DEFDOC_DICT(string DOCCODE = "", string q = "", string REMARK = "")
        {
            string toUpper = Request["toUpper"] ?? "";
            if (!string.IsNullOrEmpty(toUpper))
                q = q.ToUpper();
            if (DOCCODE.ToLower() == "null")
                DOCCODE = "";
            var BD_DEFDOCQuery = _unitOfWork.Repository<BD_DEFDOC_LIST>().Queryable();
            if (!string.IsNullOrEmpty(DOCCODE))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.DOCCODE == DOCCODE).OrderBy(x => x.ID);
            }
            if (!string.IsNullOrEmpty(q))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.LISTNAME.Contains(q) || x.LISTCODE.Contains(q)).OrderBy(x => x.ID);
            }
            if (!string.IsNullOrEmpty(REMARK))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.REMARK.Contains(REMARK)).OrderBy(x => x.ID);
            }
            var ArrBD_DEFDOC = BD_DEFDOCQuery.Select(x => new
            {
                Id = x.ID,
                ID = x.LISTCODE,
                TEXT = x.LISTNAME,
                x.ENAME,
                x.ListFullName,
                REMARK = x.REMARK
            }).ToList().OrderBy(x => x.Id).ThenBy(x => x.ID); ;
            return Json(ArrBD_DEFDOC, JsonRequestBehavior.AllowGet);
        }
        
        //从缓存 获取枚举数据字典
        public ActionResult Get_DEFDOC_DICT_FromCache(string DOCCODE = "", string q = "", string REMARK = "")
        {
            string toUpper = Request["toUpper"] ?? "";
            if (!string.IsNullOrEmpty(toUpper))
                q = q.ToUpper();
            if (DOCCODE.ToLower() == "null")
                DOCCODE = "";
            var BD_DEFDOCQuery = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST); //_unitOfWork.Repository<BD_DEFDOC_LIST>().Queryable();
            var QArrBD_DEFDOC = BD_DEFDOCQuery.Where(x => x.DOCCODE == null || x.LISTNAME == null || x.LISTCODE == null);
            if(QArrBD_DEFDOC!=null && QArrBD_DEFDOC.Any()){
                foreach (var item in QArrBD_DEFDOC)
                {
                    item.DOCCODE = item.DOCCODE ?? "";
                    item.LISTNAME = item.LISTNAME ?? "";
                    item.LISTCODE = item.LISTCODE ?? "";
                }
            }
            if (!string.IsNullOrEmpty(DOCCODE))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.DOCCODE == DOCCODE);
            }
            if (!string.IsNullOrEmpty(q))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.LISTNAME.Contains(q) || x.LISTCODE.Contains(q));
            }
            if (!string.IsNullOrEmpty(REMARK))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.REMARK.Contains(REMARK));
            }
            var ArrBD_DEFDOC = BD_DEFDOCQuery.Select(x => new
            {
                Id =x.ID,
                ID = x.LISTCODE,
                TEXT = x.LISTNAME,
                x.ENAME,
                x.ListFullName,
                REMARK = x.REMARK
            }).ToList().OrderBy(x => x.Id).ThenBy(x=>x.ID);
            return Json(ArrBD_DEFDOC, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPager_DEFDOC_DICT(int page = 1, int rows = 10, string DOCCODE = "", string q = "")
        {
            string toUpper = Request["toUpper"] ?? "";
            if (!string.IsNullOrEmpty(toUpper))
                q = q.ToUpper();
            if (DOCCODE.ToLower() == "null")
                DOCCODE = "";
            var BD_DEFDOCQuery = _unitOfWork.Repository<BD_DEFDOC_LIST>().Queryable().Where(x => x.STATUS == "1");
            if (!string.IsNullOrEmpty(DOCCODE))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.DOCCODE == DOCCODE).OrderBy(x => x.DOCCODE);
            }
            if (!string.IsNullOrEmpty(q))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.LISTNAME.Contains(q) || x.LISTCODE.Contains(q));
            }
            var ArrBD_DEFDOC = BD_DEFDOCQuery.Select(x => new
            {
                Id = x.ID,
                ID = x.LISTCODE,
                TEXT = x.LISTNAME,
                x.ENAME,
                x.ListFullName,
                REMARK = x.REMARK
            }).OrderBy(x => x.Id).ThenBy(x => x.ID);

            int totalCount = 0;
            totalCount = ArrBD_DEFDOC.Count();
            return Json(new { total = totalCount, rows = ArrBD_DEFDOC.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).OrderBy(x => x.ID).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //从缓存 获取枚举数据字典 带分页
        public ActionResult GetPager_DEFDOC_DICT_FromCache(int page = 1, int rows = 10, string DOCCODE = "", string q = "")
        {
            string toUpper = Request["toUpper"] ?? "";
            if (!string.IsNullOrEmpty(toUpper))
                q = q.ToUpper();
            if (DOCCODE.ToLower() == "null")
                DOCCODE = "";
            var BD_DEFDOCQuery = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            var QArrBD_DEFDOC = BD_DEFDOCQuery.Where(x => x.DOCCODE == null || x.LISTNAME == null || x.LISTCODE == null );
            if (QArrBD_DEFDOC != null && QArrBD_DEFDOC.Any())
            {
                foreach (var item in QArrBD_DEFDOC)
                {
                    item.DOCCODE = item.DOCCODE ?? "";
                    item.LISTNAME = item.LISTNAME ?? "";
                    item.LISTCODE = item.LISTCODE ?? "";
                }
            }
            if (!string.IsNullOrEmpty(DOCCODE))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.DOCCODE == DOCCODE);
            }
            if (!string.IsNullOrEmpty(q))
            {
                BD_DEFDOCQuery = BD_DEFDOCQuery.Where(x => x.LISTNAME.Contains(q) || x.LISTCODE.Contains(q));
            }
            var ArrBD_DEFDOC = BD_DEFDOCQuery.Where(x => x.STATUS == "1").Select(x => new
            {
                Id = x.ID,
                ID = x.LISTCODE,
                TEXT = x.LISTNAME,
                x.ENAME,
                x.ListFullName,
                REMARK = x.REMARK
            }).OrderBy(x=>x.Id).ThenBy(x => x.ID);

            int totalCount = 0;
            totalCount = ArrBD_DEFDOC.Count();
            return Json(new { total = totalCount, rows = ArrBD_DEFDOC.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).OrderBy(x => x.ID).ToList() }, JsonRequestBehavior.AllowGet);
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
