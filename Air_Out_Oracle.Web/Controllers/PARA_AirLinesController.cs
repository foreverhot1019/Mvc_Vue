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
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AirOut.Web.Controllers
{
    public class PARA_AirLinesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<PARA_AirLine>, Repository<PARA_AirLine>>();
        //container.RegisterType<IPARA_AirLineService, PARA_AirLineService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPARA_AirLineService _pARA_AirLineService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/PARA_AirLines";

        public PARA_AirLinesController(IPARA_AirLineService pARA_AirLineService, IUnitOfWorkAsync unitOfWork)
        {
            _pARA_AirLineService = pARA_AirLineService;
            _unitOfWork = unitOfWork;
            IsAutoResetCache = true;
        }

        // GET: PARA_AirLines/Index
        public ActionResult Index()
        {
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "AirOut.Web.Models.AirOutEnumType");
            ViewData["ArrStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(ArrStatus);
            return View();
        }

        // Get :PARA_AirLines/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var para_airline = _pARA_AirLineService.Query(new PARA_AirLineQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);

            var datarows = para_airline.Select(n => new
            {
                Id = n.Id,
                AirCode = n.AirCode,
                AirLine = n.AirLine,
                AirCompany = n.AirCompany,
                StarStation = n.StarStation,
                TransferStation = n.TransferStation,
                EndStation = n.EndStation,
                AirDate = n.AirDate,
                AirTime = n.AirTime,
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
        public ActionResult SaveData(PARA_AirLineChangeViewModel para_airline)
        {
            if (para_airline.updated != null)
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

                foreach (var updated in para_airline.updated)
                {
                    _pARA_AirLineService.Update(updated);
                }
            }
            if (para_airline.deleted != null)
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

                foreach (var deleted in para_airline.deleted)
                {
                    _pARA_AirLineService.Delete(deleted);
                }
            }
            if (para_airline.inserted != null)
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

                foreach (var inserted in para_airline.inserted)
                {
                    _pARA_AirLineService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((para_airline.updated != null && para_airline.updated.Any()) ||
                (para_airline.deleted != null && para_airline.deleted.Any()) ||
                (para_airline.inserted != null && para_airline.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(para_airline);
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

        // GET: PARA_AirLines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_AirLine pARA_AirLine = _pARA_AirLineService.Find(id);
            if (pARA_AirLine == null)
            {
                return HttpNotFound();
            }
            return View(pARA_AirLine);
        }

        // GET: PARA_AirLines/Create
        public ActionResult Create()
        {
            PARA_AirLine pARA_AirLine = new PARA_AirLine();
            //set default value
            return View(pARA_AirLine);
        }

        // POST: PARA_AirLines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AirCode,AirLine,AirCompany,StarStation,TransferStation,EndStation,AirDate,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_AirLine pARA_AirLine)
        {
            if (ModelState.IsValid)
            {
                _pARA_AirLineService.Insert(pARA_AirLine);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a PARA_AirLine record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_AirLine);
        }

        // GET: PARA_AirLines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_AirLine pARA_AirLine = _pARA_AirLineService.Find(id);
            if (pARA_AirLine == null)
            {
                return HttpNotFound();
            }
            return View(pARA_AirLine);
        }

        // POST: PARA_AirLines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AirCode,AirLine,AirCompany,StarStation,TransferStation,EndStation,AirDate,Description,Status,ADDWHO,ADDTS,EDITWHO,EDITTS")] PARA_AirLine pARA_AirLine)
        {
            if (ModelState.IsValid)
            {
                pARA_AirLine.ObjectState = ObjectState.Modified;
                _pARA_AirLineService.Update(pARA_AirLine);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a PARA_AirLine record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(pARA_AirLine);
        }

        // GET: PARA_AirLines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PARA_AirLine pARA_AirLine = _pARA_AirLineService.Find(id);
            if (pARA_AirLine == null)
            {
                return HttpNotFound();
            }
            return View(pARA_AirLine);
        }

        // POST: PARA_AirLines/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PARA_AirLine pARA_AirLine = _pARA_AirLineService.Find(id);
            _pARA_AirLineService.Delete(pARA_AirLine);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a PARA_AirLine record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "para_airline_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _pARA_AirLineService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 取航班信息
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerAirLine(int page = 1, int rows = 10, string q = "")
        {
            var para_AirLineRep = _unitOfWork.RepositoryAsync<PARA_AirLine>();
            var data = para_AirLineRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.AirCode.Contains(q) || n.AirLine.Contains(q));
            }
            var list = data.Select(n => new { ID = n.AirCode, TEXT = n.AirLine });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取航班信息
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="rows">显示条数</param>
        /// <param name="q">搜索条件</param>
        /// <returns></returns>
        public ActionResult GetPagerAirLine_FromCache(int page = 1, int rows = 10, string q = "")
        {
            var para_AirLine = (IEnumerable<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);
            var Qpara_AirLine = para_AirLine.Where(x => x.AirCode == null || x.AirLine == null);
            foreach (var item in Qpara_AirLine)
            {
                item.AirCode = item.AirCode ?? "";
                item.AirLine = item.AirLine ?? "";
            }
            if (q != "")
            {
                para_AirLine = para_AirLine.Where(n => n.AirCode.Contains(q) || n.AirLine.Contains(q));
            }
            var list = para_AirLine.Select(n => new { ID = n.AirCode, TEXT = n.AirLine, Time = n.AirTime  });
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

    public class MyClass
    {
        List<string> ArrRetMsg { get; set; }
        public MyClass(List<string> _ArrRetMsg)
        {
            ArrRetMsg = _ArrRetMsg;
            DisplayValue(); //这里不会阻塞  
            //System.Diagnostics.Debug.WriteLine();
            ArrRetMsg.Add("MyClass() End.");
        }

        public System.Threading.Tasks.Task<double> GetValueAsync(double num1, double num2)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    num1 = num1 / num2;
                }
                return num1;
            });
        }

        public async void DisplayValue()
        {
            double result = await GetValueAsync(1234.5, 1.01);//此处会开新线程处理GetValueAsync任务，然后方法马上返回  
            //这之后的所有代码都会被封装成委托，在GetValueAsync任务完成时调用  
            //System.Diagnostics.Debug.WriteLine("Value is : " + result);
            ArrRetMsg.Add("Value is : " + result);
        }
    }

    public class MyClass1
    {
        List<string> ArrRetMsg { get; set; }
        public MyClass1(List<string> _ArrRetMsg)
        {
            ArrRetMsg = _ArrRetMsg;
            DisplayValue(); //这里不会阻塞  
            //System.Diagnostics.Debug.WriteLine();
            ArrRetMsg.Add("MyClass1() End.");
        }

        public System.Threading.Tasks.Task<double> GetValueAsync(double num1, double num2)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    num1 = num1 / num2;
                }
                return num1;
            });
        }

        public async void DisplayValue()
        {
            double result = await GetValueAsync(1234.5, 1.01);//此处会开新线程处理GetValueAsync任务，然后方法马上返回  
            //这之后的所有代码都会被封装成委托，在GetValueAsync任务完成时调用  
            //System.Diagnostics.Debug.WriteLine("Value is : " + result);
            ArrRetMsg.Add("Value1 is : " + result);
        }
    }
}
