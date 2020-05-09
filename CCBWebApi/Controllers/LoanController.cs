using CCBWebApi.Extensions;
using CCBWebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCBWebApi.Controllers
{
    public class LoansController : Controller
    {
        /// <summary>
        /// dbContext
        /// </summary>
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        /// <summary>
        /// 异步写日志
        /// </summary>
        private bool AsyncWriteLog = false;

        public LoansController()
        {

        }

        // GET: Loan
        public ActionResult Index()
        {
            return View();
        }

        // Get :Companies/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var Query = dbContext.ResLoan.AsQueryable();
            if(filters!=null && filters.Any()){
                foreach(var item in filters){
                    if (item.field == "Trans_Id")
                    {
                        Query = Query.Where(x => x.Trans_Id.StartsWith(item.value));
                    }
                    if (item.field == "Trans_Code")
                    {
                        Query = Query.Where(x => x.Trans_Code.StartsWith(item.value));
                    }
                    if (item.field == "Unn_Soc_Cr_Cd")
                    {
                        Query = Query.Where(x => x.Unn_Soc_Cr_Cd.StartsWith(item.value));
                    }
                    if (item.field == "Rfnd_AccNo")
                    {
                        Query = Query.Where(x => x.Rfnd_AccNo.StartsWith(item.value));
                    }
                    if (item.field == "Sgn_Cst_Nm")
                    {
                        Query = Query.Where(x => x.Sgn_Cst_Nm.StartsWith(item.value));
                    }
                    if (item.field == "Sign_Dt")
                    {
                        var date = Common.ParseStrToDateTime(item.value);
                        if (date.HasValue)
                            Query = Query.Where(x => x.Sign_Dt == date.Value);
                    }
                    if (item.field == "_Sign_Dt")
                    {
                        var date = Common.ParseStrToDateTime(item.value);
                        if (date.HasValue)
                            Query = Query.Where(x => x.Sign_Dt >= date.Value);
                    }
                    if (item.field == "Sign_Dt_")
                    {
                        var date = Common.ParseStrToDateTime(item.value);
                        if (date.HasValue)
                            Query = Query.Where(x => x.Sign_Dt <= date.Value);
                    }
                    if (item.field == "Lmt_ExDat")
                    {
                        var date = Common.ParseStrToDateTime(item.value);
                        if (date.HasValue)
                            Query = Query.Where(x => x.Lmt_ExDat == date.Value);
                    }
                    if (item.field == "_Lmt_ExDat")
                    {
                        var date = Common.ParseStrToDateTime(item.value);
                        if (date.HasValue)
                            Query = Query.Where(x => x.Lmt_ExDat >= date.Value);
                    }
                    if (item.field == "Lmt_ExDat_")
                    {
                        var date = Common.ParseStrToDateTime(item.value);
                        if (date.HasValue)
                            Query = Query.Where(x => x.Lmt_ExDat <= date.Value);
                    }
                }
            }
            totalCount = Query.Count();
            int skip = page-1;

            Query = Query.OrderBy(sort, order).Skip(rows * skip).Take(rows);
            var ccc = Query.ToList();
            var datarows = Query.Select(n => new
            {
                n.Id,
                n.Trans_Id,
                n.Trans_Code,
                n.Unn_Soc_Cr_Cd,
                n.Sgn_Cst_Nm,
                n.CoPlf_ID,
                n.Sign_Dt,
                n.AR_Lmt,
                n.Lmt_ExDat,
                n.Rfnd_AccNo,
                n.Remark,
                n.Status,
                n.AuditStatus,
                n.AddUser,
                n.AddDate,
                n.LastEditUser,
                n.LastEditDate,
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(ResLoanViewModel resloan)
        {
            var _companyService = dbContext.ResLoan;
            if (resloan.updated != null)
            {
                foreach (var updated in resloan.updated)
                {
                    if (updated.Id > 0)
                    {
                        var OEntry = dbContext.Entry(updated);
                        OEntry.State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }
            if (resloan.deleted != null)
            {
                foreach (var deleted in resloan.deleted)
                {
                    if (deleted.Id > 0)
                    {
                        var OEntry = dbContext.Entry(deleted);
                        OEntry.State = System.Data.Entity.EntityState.Deleted;
                    }
                }
            }
            if (resloan.inserted != null)
            {
                foreach (var inserted in resloan.inserted)
                {
                    if (inserted.Id <= 0)
                    {
                        var OEntry = dbContext.Entry(inserted);
                        OEntry.State = System.Data.Entity.EntityState.Added;
                    }
                }
            }
            try
            {
                if ((resloan.updated != null && resloan.updated.Any()) ||
                (resloan.deleted != null && resloan.deleted.Any()) ||
                (resloan.inserted != null && resloan.inserted.Any()))
                {
                    dbContext.SaveChanges();
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}