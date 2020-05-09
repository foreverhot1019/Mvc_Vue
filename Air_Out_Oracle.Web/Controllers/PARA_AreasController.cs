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
    public class PARA_AreasController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<PARA_Area>, Repository<PARA_Area>>();
        //container.RegisterType<IPARA_AreaService, PARA_AreaService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPARA_AreaService _pARA_AreaService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/PARA_Areas";

        public PARA_AreasController(IPARA_AreaService pARA_AreaService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _pARA_AreaService = pARA_AreaService;
            _unitOfWork = unitOfWork;
        }

        // GET: PARA_Areas/Index
        public ActionResult Index()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            //var para_area  = _pARA_AreaService.Queryable().AsQueryable();
            //return View(para_area  );
            return View();
        }

        // Get :PARA_Areas/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var para_area = _pARA_AreaService.Query(new PARA_AreaQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = para_area.Select(n => new
            {
                ID = n.ID,
                Country_CO = n.Country_CO,
                AreaCode = n.AreaCode,
                AreaName = n.AreaName,
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
        public ActionResult SaveData(PARA_AreaChangeViewModel para_area)
        {
            if (para_area.updated != null)
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

                foreach (var updated in para_area.updated)
                {
                    _pARA_AreaService.Update(updated);
                }
            }
            if (para_area.deleted != null)
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

                foreach (var deleted in para_area.deleted)
                {
                    _pARA_AreaService.Delete(deleted);
                }
            }
            if (para_area.inserted != null)
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

                foreach (var inserted in para_area.inserted)
                {
                    _pARA_AreaService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((para_area.updated != null && para_area.updated.Any()) ||
                (para_area.deleted != null && para_area.deleted.Any()) ||
                (para_area.inserted != null && para_area.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(para_area);
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

        // GET: PARA_Areas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Area pARA_Area = _pARA_AreaService.Find(id);
            if (pARA_Area == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Area);
        }

        // GET: PARA_Areas/Create
        public ActionResult Create()
        {
            PARA_Area pARA_Area = new PARA_Area();
            //set default value
            return View(pARA_Area);
        }

        // POST: PARA_Areas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(PARA_Area pARA_Area)
        {
            if (ModelState.IsValid)
            {
                _pARA_AreaService.Insert(pARA_Area);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a PARA_Area record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_Area);
        }

        // GET: PARA_Areas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Area pARA_Area = _pARA_AreaService.Find(id);
            if (pARA_Area == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Area);
        }

        // POST: PARA_Areas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(PARA_Area pARA_Area)
        {
            if (ModelState.IsValid)
            {
                pARA_Area.ObjectState = ObjectState.Modified;
                _pARA_AreaService.Update(pARA_Area);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a PARA_Area record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_Area);
        }

        // GET: PARA_Areas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_Area pARA_Area = _pARA_AreaService.Find(id);
            if (pARA_Area == null)
            {
                return HttpNotFound();
            }
            return View(pARA_Area);
        }

        // POST: PARA_Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PARA_Area pARA_Area = _pARA_AreaService.Find(id);
            _pARA_AreaService.Delete(pARA_Area);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a PARA_Area record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "para_area_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _pARA_AreaService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取区域
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_Area(int page = 1, int rows = 10, string q = "")
        {
            var para_areaRep = _unitOfWork.RepositoryAsync<PARA_Area>();
            var data = para_areaRep.Queryable();
            if (q != "")
            {
                int i_q = 0;
                if (int.TryParse(q, out i_q))
                {
                    data = data.Where(n => n.ID == i_q || n.AreaCode.Contains(q) || n.AreaName.Contains(q));
                }
                else
                {
                    data = data.Where(n => n.AreaCode.Contains(q) || n.AreaName.Contains(q));
                }
            }
            var list = data.Select(n => new { ID = n.ID, AreaCode = n.AreaCode, TEXT = n.AreaName, Name = n.AreaName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取区域
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_Area_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var para_areaRep = (IEnumerable<PARA_Area>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_Area);
            var Qpara_area = para_areaRep.Where(x => x.AreaCode == null || x.AreaName == null);
            if (Qpara_area.Any())
            {
                foreach (var item in Qpara_area)
                {
                    item.AreaCode = item.AreaCode ?? "";
                    item.AreaName = item.AreaName ?? "";
                }
            }
            if (q != "")
            {
                int i_q = 0;
                if (int.TryParse(q, out i_q))
                {
                    para_areaRep = para_areaRep.Where(n => n.ID == i_q || n.AreaCode.Contains(q) || n.AreaName.Contains(q));
                }
                else
                {
                    para_areaRep = para_areaRep.Where(n => n.AreaCode.Contains(q) || n.AreaName.Contains(q));
                }
            }
            var list = para_areaRep.Select(n => new { ID = n.ID, AreaCode = n.AreaCode, TEXT = n.AreaName, Name = n.AreaName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取区域+口岸（起始的/目的地使用）
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerAddressPlace(int page = 1, int rows = 10, string q = "")
        {
            var pARA_AreaRep = _unitOfWork.RepositoryAsync<PARA_Area>();
            var data = pARA_AreaRep.Queryable();
            var para_AirPort = _unitOfWork.RepositoryAsync<PARA_AirPort>();
            var post = para_AirPort.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.AreaCode.Contains(q) || n.AreaName.Contains(q));
                post = post.Where(n => n.PortCode.Contains(q) || n.PortName.Contains(q));
            }
            var listarea = data.Select(n => new { ID = n.ID.ToString(), TEXT = n.AreaName });
            var listpost = post.Select(n => new { ID = n.PortCode, TEXT = n.PortName });
            var list = listarea.Concat(listpost);

            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取区域+口岸（起始的/目的地使用）
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerAddressPlace_FromCache(int page = 1, int rows = 10, string q = "")
        {
            IEnumerable<ComboDridListModel> QData = _pARA_AreaService.GetComboDridListModel_FromCache();
            //搜索
            if (q != "")
            {
                QData = QData.Where(n => n.ID.Contains(q) || n.TEXT.Contains(q));
            }
            int totalCount = 0;
            totalCount = QData.Count();
            return Json(new { total = totalCount, rows = QData.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
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
