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
    public class SallersController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Saller>, Repository<Saller>>();
        //container.RegisterType<ISallerService, SallerService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ISallerService _sallerService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/Sallers";

        public SallersController(ISallerService sallerService, IUnitOfWorkAsync unitOfWork)
            : base(false, true)
        {
            _sallerService = sallerService;
            _unitOfWork = unitOfWork;
        }

        // GET: Sallers/Index
        public ActionResult Index()
        {
            //var sallers  = _sallerService.Queryable().AsQueryable();
            //return View(sallers  );
            return View();
        }

        // Get :Sallers/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var sallers = _sallerService.Query(new SallerQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = sallers.Select(n => new
            {
                Id = n.Id,
                Name = n.Name,
                Code = n.Code,
                PhoneNumber = n.PhoneNumber,
                Company = n.Company,
                Address = n.Address,
                Description = n.Description,
                Status = n.Status,
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
        public ActionResult SaveData(SallerChangeViewModel sallers)
        {
            List<string> ArrUpSQL = new List<string>();//更新使用销售的表
            var TmpSQL = "update {0} t set t.SallerName ='{1}' where t.SallerId={2}";
            if (sallers.updated != null)
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

                foreach (var updated in sallers.updated)
                {
                    _sallerService.Update(updated);
                    ArrUpSQL.Add(string.Format(TmpSQL, "CusBusInfos", updated.Name, updated.Id));
                    ArrUpSQL.Add(string.Format(TmpSQL, "OPS_EntrustmentInfors", updated.Name, updated.Id));
                }
            }
            if (sallers.deleted != null)
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

                foreach (var deleted in sallers.deleted)
                {
                    _sallerService.Delete(deleted);
                }
            }
            if (sallers.inserted != null)
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

                foreach (var inserted in sallers.inserted)
                {
                    _sallerService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((sallers.updated != null && sallers.updated.Any()) ||
                (sallers.deleted != null && sallers.deleted.Any()) ||
                (sallers.inserted != null && sallers.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(sallers);

                    #region 更新其他引用销售的名称

                    if (sallers.updated != null && sallers.updated.Any())
                    {
                        try
                        {
                            int ret = SQLDALHelper.OracleHelper.ExecuteSqlTran(ArrUpSQL);
                        }
                        catch (Exception e)
                        {
                            Common.AddError(e, NotificationTag.Sys, "系统异常");
                            string ErrMSg = Common.GetExceptionMsg(e);
                            return Json(new { Success = false, ErrMsg = "保存成功，但更新销售名称失败：" + ErrMSg, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    #endregion

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

        /// <summary>
        /// 测试 数据唯一验证
        /// </summary>
        /// <param name="Id">主键</param>
        /// <param name="Name">姓名</param>
        /// <returns></returns>
        public ActionResult GetIsUse(int Id, string Name)
        {
            var IsUse = _sallerService.Queryable().Any(x => x.Name == Name && x.Id != Id);
            return Json(!IsUse);
        }

        // GET: Sallers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Saller saller = _sallerService.Find(id);
            if (saller == null)
            {
                return HttpNotFound();
            }
            return View(saller);
        }

        // GET: Sallers/Create
        public ActionResult Create()
        {
            Saller saller = new Saller();
            //set default value
            return View(saller);
        }

        // POST: Sallers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArrCusBusInfo,Id,Name,PhoneNumber,Company,Address,Description,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Saller saller)
        {
            if (ModelState.IsValid)
            {
                _sallerService.Insert(saller);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Saller record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(saller);
        }

        // GET: Sallers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Saller saller = _sallerService.Find(id);
            if (saller == null)
            {
                return HttpNotFound();
            }
            return View(saller);
        }

        // POST: Sallers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArrCusBusInfo,Id,Name,PhoneNumber,Company,Address,Description,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Saller saller)
        {
            if (ModelState.IsValid)
            {
                saller.ObjectState = ObjectState.Modified;
                _sallerService.Update(saller);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Saller record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(saller);
        }

        // GET: Sallers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Saller saller = _sallerService.Find(id);
            if (saller == null)
            {
                return HttpNotFound();
            }
            return View(saller);
        }

        // POST: Sallers/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Saller saller = _sallerService.Find(id);
            _sallerService.Delete(saller);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Saller record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "sallers_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _sallerService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取销售信息
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerSallers(int page = 1, int rows = 10, string q = "")
        {
            var SallerRep = _unitOfWork.RepositoryAsync<Saller>();
            var data = SallerRep.Queryable().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable);
            if (q != "")
            {
                int ID = 0;
                int.TryParse(q, out ID);
                if (ID > 0)
                    data = data.Where(n => n.Id == ID || n.Name.StartsWith(q) || n.Code.StartsWith(q));
                else
                    data = data.Where(n => n.Name.StartsWith(q) || n.Code.StartsWith(q));
            }
            var list = data.Select(n => new { ID = n.Id, TEXT = n.Name, Code = n.Code, PhoneNumber = n.PhoneNumber, Company = n.Company, IDTEXT = n.Id + "|" + n.Name, Address = n.Address });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取销售信息
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerSallers_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var SallerRep = (IEnumerable<Saller>)CacheHelper.Get_SetCache(Common.CacheNameS.Saller);
            SallerRep = SallerRep.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable);
            var QSaller = SallerRep.Where(x => x.Company == null || x.Address == null);
            foreach (var item in QSaller)
            {
                item.Company = item.Company ?? "";
                item.Address = item.Address ?? "";
            }
            if (q != "")
            {
                int ID = 0;
                int.TryParse(q, out ID);
                if (ID > 0)
                    SallerRep = SallerRep.Where(n => n.Id == ID || n.Name.StartsWith(q) || n.Code.Contains(q));
                else
                    SallerRep = SallerRep.Where(n => n.Name.StartsWith(q) || n.Code.Contains(q));
            }
            var list = SallerRep.Select(n => new { ID = n.Id, TEXT = n.Name, Code = n.Code, PhoneNumber = n.PhoneNumber, Company = n.Company, IDTEXT = n.Id + "|" + n.Name, Address = n.Address });
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
