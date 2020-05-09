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
    public class PARA_AirPortsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<PARA_AirPort>, Repository<PARA_AirPort>>();
        //container.RegisterType<IPARA_AirPortService, PARA_AirPortService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPARA_AirPortService _pARA_AirPortService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/PARA_AirPorts";

        public PARA_AirPortsController(IPARA_AirPortService pARA_AirPortService, IUnitOfWorkAsync unitOfWork)
        {
            _pARA_AirPortService = pARA_AirPortService;
            _unitOfWork = unitOfWork;
            IsAutoResetCache = true;
        }

        // GET: PARA_AirPorts/Index
        public ActionResult Index()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            //var para_airport  = _pARA_AirPortService.Queryable().AsQueryable();
            //return View(para_airport  );
            return View();
        }

        // Get :PARA_AirPorts/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var para_airport = _pARA_AirPortService.Query(new PARA_AirPortQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = para_airport.Select(n => new
            {
                Id = n.Id,
                PortCode = n.PortCode,
                PortName = n.PortName,
                PortNameEng = n.PortNameEng,
                PortType = n.PortType,
                Description = n.Description,
                CountryCode = n.CountryCode,
                Status = n.Status,
                PeacePortName = n.PeacePortName,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(PARA_AirPortChangeViewModel para_airport)
        {
            if (para_airport.updated != null)
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

                foreach (var updated in para_airport.updated)
                {
                    _pARA_AirPortService.Update(updated);
                }
            }
            if (para_airport.deleted != null)
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

                foreach (var deleted in para_airport.deleted)
                {
                    _pARA_AirPortService.Delete(deleted);
                }
            }
            if (para_airport.inserted != null)
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

                foreach (var inserted in para_airport.inserted)
                {
                    _pARA_AirPortService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((para_airport.updated != null && para_airport.updated.Any()) ||
                (para_airport.deleted != null && para_airport.deleted.Any()) ||
                (para_airport.inserted != null && para_airport.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(para_airport);
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

        // GET: PARA_AirPorts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_AirPort pARA_AirPort = _pARA_AirPortService.Find(id);
            if (pARA_AirPort == null)
            {
                return HttpNotFound();
            }
            return View(pARA_AirPort);
        }

        // GET: PARA_AirPorts/Create
        public ActionResult Create()
        {
            PARA_AirPort pARA_AirPort = new PARA_AirPort();
            //set default value
            return View(pARA_AirPort);
        }

        // POST: PARA_AirPorts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PortCode,PortName,PortNameEng,PortType,Description,CountryCode,Status,PeacePortName,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_AirPort pARA_AirPort)
        {
            if (ModelState.IsValid)
            {
                _pARA_AirPortService.Insert(pARA_AirPort);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a PARA_AirPort record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_AirPort);
        }

        // GET: PARA_AirPorts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_AirPort pARA_AirPort = _pARA_AirPortService.Find(id);
            if (pARA_AirPort == null)
            {
                return HttpNotFound();
            }
            return View(pARA_AirPort);
        }

        // POST: PARA_AirPorts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PortCode,PortName,PortNameEng,PortType,Description,CountryCode,Status,PeacePortName,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_AirPort pARA_AirPort)
        {
            if (ModelState.IsValid)
            {
                pARA_AirPort.ObjectState = ObjectState.Modified;
                _pARA_AirPortService.Update(pARA_AirPort);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a PARA_AirPort record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_AirPort);
        }

        // GET: PARA_AirPorts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_AirPort pARA_AirPort = _pARA_AirPortService.Find(id);
            if (pARA_AirPort == null)
            {
                return HttpNotFound();
            }
            return View(pARA_AirPort);
        }

        // POST: PARA_AirPorts/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PARA_AirPort pARA_AirPort = _pARA_AirPortService.Find(id);
            _pARA_AirPortService.Delete(pARA_AirPort);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a PARA_AirPort record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "para_airport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _pARA_AirPortService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取港口
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">每页行</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPARA_AirPorts(int page = 1, int rows = 10, string q = "")
        {
            var para_AirPort = _unitOfWork.RepositoryAsync<PARA_AirPort>();
            var post = para_AirPort.Queryable();
            if (q != "")
            {
                post = post.Where(n => n.PortCode.Contains(q) || n.PortName.Contains(q));
            }
            var listpost = post.Select(n => new { ID = n.PortCode, TEXT = n.PortName, IDTEXT = n.PortCode + "|" + n.PortNameEng });

            int totalCount = 0;
            totalCount = listpost.Count();
            return Json(new { total = totalCount, rows = listpost.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取港口
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">每页行</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPARA_AirPorts_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var para_AirPort = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var Qpara_AirPort = para_AirPort.Where(x => x.PortCode == null || x.PortName == null);
            if (Qpara_AirPort.Any())
            {
                foreach (var item in Qpara_AirPort)
                {
                    item.PortCode = item.PortCode ?? "";
                    item.PortName = item.PortName ?? "";
                }
            }
            if (q != "")
            {
                para_AirPort = para_AirPort.Where(n => n.PortCode.Contains(q) || n.PortName.Contains(q));
            }
            var listpost = para_AirPort.Select(n => new { ID = n.PortCode, TEXT = n.PortName, IDTEXT = n.PortCode + "|" + n.PortNameEng });

            int totalCount = 0;
            totalCount = listpost.Count();
            return Json(new { total = totalCount, rows = listpost.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取港口
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">每页行</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_AirPorts(int page = 1, int rows = 10, string q = "")
        {
            var para_AirPort = _unitOfWork.RepositoryAsync<PARA_AirPort>();
            var post = para_AirPort.Queryable();
            if (q != "")
            {
                post = post.Where(n => n.PortCode.Contains(q) || n.PortName.Contains(q));
            }
            var listpost = post.Select(n => new { ID = n.PortCode, TEXT = n.PortName, IDTEXT = n.PortCode + "|" + n.PortNameEng });

            int totalCount = 0;
            totalCount = listpost.Count();
            return Json(new { total = totalCount, rows = listpost.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取港口
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">每页行</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerPARA_AirPorts_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var para_AirPort = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var Qpara_AirPort = para_AirPort.Where(x => x.PortCode == null || x.PortName == null);
            if (Qpara_AirPort.Any())
            {
                foreach (var item in Qpara_AirPort)
                {
                    item.PortCode = item.PortCode ?? "";
                    item.PortName = item.PortName ?? "";
                }
            }
            if (q != "")
            {
                para_AirPort = para_AirPort.Where(n => n.PortCode.Contains(q) || n.PortName.Contains(q));
            }
            var listpost = para_AirPort.Select(n => new { ID = n.PortCode, TEXT = n.PortName, IDTEXT = n.PortCode + "|" + n.PortNameEng });

            int totalCount = 0;
            totalCount = listpost.Count();
            return Json(new { total = totalCount, rows = listpost.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
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