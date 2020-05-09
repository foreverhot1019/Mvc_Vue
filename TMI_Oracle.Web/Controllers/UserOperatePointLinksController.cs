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
    public class UserOperatePointLinksController : Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<UserOperatePointLink>, Repository<UserOperatePointLink>>();
        //container.RegisterType<IUserOperatePointLinkService, UserOperatePointLinkService>();

        //private WebdbContext db = new WebdbContext();
        private readonly IUserOperatePointLinkService _userOperatePointLinkService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public UserOperatePointLinksController(IUserOperatePointLinkService userOperatePointLinkService, IUnitOfWorkAsync unitOfWork)
        {
            _userOperatePointLinkService = userOperatePointLinkService;
            _unitOfWork = unitOfWork;
        }

        // GET: UserOperatePointLinks/Index
        public ActionResult Index()
        {
            //var useroperatepointlink  = _userOperatePointLinkService.Queryable().AsQueryable();
            //return View(useroperatepointlink  );
            return View();
        }

        // Get :UserOperatePointLinks/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var useroperatepointlink = _userOperatePointLinkService.Query(new UserOperatePointLinkQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = useroperatepointlink.Select(n => new
            {
                ID = n.ID,
                UserId = n.UserId,
                OperateOpintId = n.OperateOpintId
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(UserOperatePointLinkChangeViewModel useroperatepointlink)
        {
            if (useroperatepointlink.updated != null)
            {
                foreach (var updated in useroperatepointlink.updated)
                {
                    _userOperatePointLinkService.Update(updated);
                }
            }
            if (useroperatepointlink.deleted != null)
            {
                foreach (var deleted in useroperatepointlink.deleted)
                {
                    _userOperatePointLinkService.Delete(deleted);
                }
            }
            if (useroperatepointlink.inserted != null)
            {
                foreach (var inserted in useroperatepointlink.inserted)
                {
                    _userOperatePointLinkService.Insert(inserted);
                }
            }
            _unitOfWork.SaveChanges();

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        // GET: UserOperatePointLinks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserOperatePointLink userOperatePointLink = _userOperatePointLinkService.Find(id);
            if (userOperatePointLink == null)
            {
                return HttpNotFound();
            }
            return View(userOperatePointLink);
        }

        // GET: UserOperatePointLinks/Create
        public ActionResult Create()
        {
            UserOperatePointLink userOperatePointLink = new UserOperatePointLink();
            //set default value
            return View(userOperatePointLink);
        }

        // POST: UserOperatePointLinks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserId,OperateOpintId")] UserOperatePointLink userOperatePointLink)
        {
            if (ModelState.IsValid)
            {
                _userOperatePointLinkService.Insert(userOperatePointLink);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a UserOperatePointLink record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(userOperatePointLink);
        }

        // GET: UserOperatePointLinks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserOperatePointLink userOperatePointLink = _userOperatePointLinkService.Find(id);
            if (userOperatePointLink == null)
            {
                return HttpNotFound();
            }
            return View(userOperatePointLink);
        }

        // POST: UserOperatePointLinks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserId,OperateOpintId")] UserOperatePointLink userOperatePointLink)
        {
            if (ModelState.IsValid)
            {
                userOperatePointLink.ObjectState = ObjectState.Modified;
                _userOperatePointLinkService.Update(userOperatePointLink);

                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a UserOperatePointLink record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(userOperatePointLink);
        }

        // GET: UserOperatePointLinks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserOperatePointLink userOperatePointLink = _userOperatePointLinkService.Find(id);
            if (userOperatePointLink == null)
            {
                return HttpNotFound();
            }
            return View(userOperatePointLink);
        }

        // POST: UserOperatePointLinks/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserOperatePointLink userOperatePointLink = _userOperatePointLinkService.Find(id);
            _userOperatePointLinkService.Delete(userOperatePointLink);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a UserOperatePointLink record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "useroperatepointlink_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _userOperatePointLinkService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 保存用户选择的操作点
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OperatePointIDs"></param>
        /// <returns></returns>
        public ActionResult SaveUserOperatePoints(string UserId = "", int[] OperatePointIDs = null)
        {
            try
            {
                if (string.IsNullOrEmpty(UserId) || OperatePointIDs == null || !OperatePointIDs.Any())
                {
                    return Json(new { Success = false, ErrMsg = "数据格式不正确" }, JsonRequestBehavior.AllowGet);
                }
                var ArrUserOpLinkCache = (List<UserOperatePointLink>)CacheHelper.Get_SetCache(Common.CacheNameS.UserOperatePointLink);
                var UserOptLnks = _userOperatePointLinkService.Query(x => x.UserId == UserId).Select().ToList();
                if (UserOptLnks.Any())
                {
                    var U_OperatePointIDs = UserOptLnks.Select(x=>x.OperateOpintId);
                    var Adds = OperatePointIDs.Where(x => !U_OperatePointIDs.Contains(x));
                    var UserOptLnks_Add = Adds.Select(x => new UserOperatePointLink
                    {
                        UserId = UserId,
                        OperateOpintId = x,
                        ObjectState = ObjectState.Added
                    });
                    _userOperatePointLinkService.InsertGraphRange(UserOptLnks_Add);
                    var Delts = UserOptLnks.Where(x => !OperatePointIDs.Contains(x.OperateOpintId));
                    foreach (var item in Delts)
                    {
                        _userOperatePointLinkService.Delete(item.ID);
                        //删除缓存内容
                        ArrUserOpLinkCache.Remove(item);
                    }
                    _unitOfWork.SaveChanges();
                    //添加缓存内容
                    ArrUserOpLinkCache.AddRange(UserOptLnks_Add);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var UserOptLnks_Add = OperatePointIDs.Select(x=>new UserOperatePointLink { 
                        UserId = UserId,
                        OperateOpintId = x,
                        ObjectState = ObjectState.Added
                    });
                    _userOperatePointLinkService.InsertGraphRange(UserOptLnks_Add);
                    _unitOfWork.SaveChanges();
                    //添加缓存内容
                    ArrUserOpLinkCache.AddRange(UserOptLnks_Add);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
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
