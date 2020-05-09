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
    public class OperatePointsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<OperatePoint>, Repository<OperatePoint>>();
        //container.RegisterType<IOperatePointService, OperatePointService>();

        //private WebdbContext db = new WebdbContext();
        private readonly IOperatePointService _operatePointService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        //验证权限的名称
        private string ControllerQXName = "/OperatePoints";//"/EMS_ORG_BOMs"

        public OperatePointsController(IOperatePointService operatePointService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _operatePointService = operatePointService;
            _unitOfWork = unitOfWork;
        }

        // GET: OperatePoints/Index
        public ActionResult Index()
        {
            //var operatepoint  = _operatePointService.Queryable().AsQueryable();
            //return View(operatepoint  );
            return View();
        }

        // Get :OperatePoints/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var operatepoint = _operatePointService.Query(new OperatePointQuery().Withfilter(filters)).Include(x => x.OperatePointLists).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = operatepoint.Select(n => new
            {
                ID = n.ID,
                OperatePointCode = n.OperatePointCode,
                OperatePointName = n.OperatePointName,
                Description = n.Description,
                IsEnabled = n.IsEnabled,
                ADDTS = n.ADDTS,
                ADDID = n.ADDID,
                ADDWHO = n.ADDWHO,
                EDITWHO = n.EDITWHO,
                EDITID = n.EDITID,
                EDITTS = n.EDITTS,
                OperatePointListNum = n.OperatePointLists.Count()
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(OperatePointChangeViewModel operatepoint)
        {
            if (operatepoint.updated != null)
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

                foreach (var updated in operatepoint.updated)
                {
                    _operatePointService.Update(updated);
                }
            }
            if (operatepoint.deleted != null)
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

                foreach (var deleted in operatepoint.deleted)
                {
                    _operatePointService.Delete(deleted);
                }
            }
            if (operatepoint.inserted != null)
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

                foreach (var inserted in operatepoint.inserted)
                {
                    _operatePointService.Insert(inserted);
                }
            }

            try
            {
                if ((operatepoint.updated != null && operatepoint.updated.Any()) ||
                    (operatepoint.deleted != null && operatepoint.deleted.Any()) ||
                    (operatepoint.inserted != null && operatepoint.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();

                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(operatepoint);

                    #region 更新菜单缓存

                    ////加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                    //lock (Common.lockCacheOperatePoints)
                    //{
                    //    List<OperatePoint> ArrOperatePoint = _unitOfWork.Repository<OperatePoint>().Queryable().AsNoTracking().OrderBy(x => x.OperatePointCode).ToList();
                    //    if (HttpContext.Cache["OperatePoint"] != null)
                    //        HttpContext.Cache.Remove("OperatePoint");
                    //    HttpContext.Cache.Insert("OperatePoint", ArrOperatePoint, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                    //}

                    #endregion

                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: OperatePoints/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperatePoint operatePoint = _operatePointService.Find(id);
            if (operatePoint == null)
            {
                return HttpNotFound();
            }
            return View(operatePoint);
        }

        // GET: OperatePoints/Create
        public ActionResult Create()
        {
            OperatePoint operatePoint = new OperatePoint();
            //set default value
            return View(operatePoint);
        }

        // POST: OperatePoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OperatePointLists,ID,OperatePointCode,OperatePointName,Description,IsEnabled,ADDTS,ADDID,ADDWHO,EDITWHO,EDITID,EDITTS")] OperatePoint operatePoint)
        {
            if (ModelState.IsValid)
            {
                _operatePointService.Insert(operatePoint);
                _unitOfWork.SaveChanges();

                #region 更新菜单缓存

                //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                lock (Common.lockCacheOperatePoints)
                {
                    List<OperatePoint> ArrOperatePoint = _unitOfWork.Repository<OperatePoint>().Queryable().AsNoTracking().OrderBy(x => x.OperatePointCode).ToList();
                    if (HttpContext.Cache[Common.GeCacheEnumByName("OperatePoints").ToString()] != null)
                        HttpContext.Cache.Remove(Common.GeCacheEnumByName("OperatePoints").ToString());
                    HttpContext.Cache.Insert(Common.GeCacheEnumByName("OperatePoints").ToString(), ArrOperatePoint, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                #endregion

                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a OperatePoint record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(operatePoint);
        }

        // GET: OperatePoints/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperatePoint operatePoint = _operatePointService.Find(id);
            if (operatePoint == null)
            {
                return HttpNotFound();
            }
            return View(operatePoint);
        }

        // POST: OperatePoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OperatePointLists,ID,OperatePointCode,OperatePointName,Description,IsEnabled,ADDTS,ADDID,ADDWHO,EDITWHO,EDITID,EDITTS")] OperatePoint operatePoint)
        {
            if (ModelState.IsValid)
            {
                operatePoint.ObjectState = ObjectState.Modified;
                _operatePointService.Update(operatePoint);

                _unitOfWork.SaveChanges();

                #region 更新菜单缓存

                //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                lock (Common.lockCacheOperatePoints)
                {
                    List<OperatePoint> ArrOperatePoint = _unitOfWork.Repository<OperatePoint>().Queryable().AsNoTracking().OrderBy(x => x.OperatePointCode).ToList();
                    if (HttpContext.Cache[Common.GeCacheEnumByName("OperatePoints").ToString()] != null)
                        HttpContext.Cache.Remove(Common.GeCacheEnumByName("OperatePoints").ToString());
                    HttpContext.Cache.Insert(Common.GeCacheEnumByName("OperatePoints").ToString(), ArrOperatePoint, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                #endregion

                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a OperatePoint record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(operatePoint);
        }

        // GET: OperatePoints/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperatePoint operatePoint = _operatePointService.Find(id);
            if (operatePoint == null)
            {
                return HttpNotFound();
            }
            return View(operatePoint);
        }

        // POST: OperatePoints/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OperatePoint operatePoint = _operatePointService.Find(id);
            _operatePointService.Delete(operatePoint);
            _unitOfWork.SaveChanges();

            #region 更新菜单缓存

            //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
            lock (Common.lockCacheOperatePoints)
            {
                List<OperatePoint> ArrOperatePoint = _unitOfWork.Repository<OperatePoint>().Queryable().AsNoTracking().OrderBy(x => x.OperatePointCode).ToList();
                if (HttpContext.Cache[Common.GeCacheEnumByName("OperatePoints").ToString()] != null)
                    HttpContext.Cache.Remove(Common.GeCacheEnumByName("OperatePoints").ToString());
                HttpContext.Cache.Insert(Common.GeCacheEnumByName("OperatePoints").ToString(), ArrOperatePoint, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            #endregion

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a OperatePoint record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "operatepoint_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _operatePointService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取操作点
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOperatePoints(string UserId)
        {
            try
            {
                var UserOperatePointLinkRep = _unitOfWork.Repository<UserOperatePointLink>();
                var UserOptLink = UserOperatePointLinkRep.Queryable().Where(x => x.UserId == UserId).Select(x => x.OperateOpintId);
                var UserOperatePoints = from n in _unitOfWork.Repository<OperatePoint>().Queryable()
                                        where n.IsEnabled == true && UserOptLink.Contains(n.ID)
                                        select new
                                        {
                                            ID = n.ID,
                                            OperatePointCode = n.OperatePointCode,
                                            OperatePointName = n.OperatePointName
                                        };
                var ArrUserOperatePoints = UserOperatePoints.ToList();

                var OperatePoints = _operatePointService.Query(x => x.IsEnabled == true).Select(x => new
                {
                    x.ID,
                    x.OperatePointCode,
                    x.OperatePointName
                }).ToList();
                var result = OperatePoints.Select(x => new
                {
                    x.ID,
                    x.OperatePointCode,
                    x.OperatePointName,
                    UserChecked = ArrUserOperatePoints.Any(n => n.ID == x.ID)
                });

                return Json(new { Success = true, UserOperatePoints = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 保存用户选择的操作点
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OperatePointIDs"></param>
        /// <returns></returns>
        public ActionResult SaveLayOutOperatePoint(string UserId = "", int OperatePointID = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(UserId) || OperatePointID <= 0)
                {
                    return Json(new { Success = false, ErrMsg = "数据格式不正确" }, JsonRequestBehavior.AllowGet);
                }
                var OperatePoint = _operatePointService.Query(x => x.ID == OperatePointID).Select().ToList();
                if (OperatePoint.Any())
                {
                    HttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint").ToString()] = OperatePoint;

                    if (HttpContext.Request.Cookies[UserId] == null)
                    {
                        System.Web.HttpCookie newcookie = new HttpCookie(UserId);
                        newcookie.Value = Newtonsoft.Json.JsonConvert.SerializeObject(OperatePoint);
                        newcookie.Expires = DateTime.Now.AddDays(1);
                        HttpContext.Response.Cookies.Add(newcookie);
                    }
                    else
                    {
                        string OperatePointStr = Newtonsoft.Json.JsonConvert.SerializeObject(OperatePoint);
                        //HttpContext.Request.Cookies[UserId].Value = OperatePointStr;
                        System.Web.HttpCookie newcookie = new HttpCookie(UserId);
                        newcookie.Value = OperatePointStr;
                        newcookie.Expires = DateTime.Now.AddDays(1);
                        HttpContext.Response.Cookies.Add(newcookie);

                        ApplicationDbContext AppDbContext = new ApplicationDbContext();
                        var QUsers = AppDbContext.Users.AsQueryable().Where(x => x.Id == UserId).ToList();
                        if (QUsers.Any())
                        {
                            var OUsers = QUsers.FirstOrDefault();
                            OUsers.UserOperatPoint = OperatePointID.ToString();
                            AppDbContext.Entry(OUsers).State = System.Data.Entity.EntityState.Modified;
                            AppDbContext.SaveChanges();
                        }
                    }

                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "操作点不存在" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        //获取所有的操作点
        public ActionResult GetOperatePoint()
        {
            var operatepointRep = _unitOfWork.RepositoryAsync<OperatePoint>();
            var data = operatepointRep.Queryable();
            var list = data.Select(n => new { Value = n.ID, Text = n.OperatePointName });
            return Json(list, JsonRequestBehavior.AllowGet);
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
