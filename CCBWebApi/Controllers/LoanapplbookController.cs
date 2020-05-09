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
    public class LoanapplbookController : Controller
    {
        /// <summary>
        /// dbContext
        /// </summary>
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        /// <summary>
        /// 异步写日志
        /// </summary>
        private bool AsyncWriteLog = false;

        public LoanapplbookController()
        {

        }
        // GET: Loanapplbook
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var Query = dbContext.Loanapplbook.AsQueryable();
            if (filters != null && filters.Any())
            {
                foreach (var item in filters)
                {
                    if (item.field == "Unn_Soc_Cr_Cd")
                    {
                        Query = Query.Where(x => x.Unn_Soc_Cr_Cd.StartsWith(item.value));
                    }
                    if (item.field == "AcctBeginDate")
                    {
                        var date = Common.ParseStrToDateTime(item.value);
                        if (date.HasValue)
                            Query = Query.Where(x => x.AcctBeginDate == date.Value);
                    }
                    if (item.field == "_AcctBeginDate")
                    {
                        var date = Common.ParseStrToDateTime(item.value);
                        if (date.HasValue)
                            Query = Query.Where(x => x.AcctBeginDate >= date.Value);
                    }
                    if (item.field == "AcctBeginDate_")
                    {
                        var date = Common.ParseStrToDateTime(item.value);
                        if (date.HasValue)
                            Query = Query.Where(x => x.AcctBeginDate <= date.Value);
                    }
                    if (item.field == "CustName")
                    {
                        Query = Query.Where(x => x.CustName.StartsWith(item.value));
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
                    if (item.field == "Rfnd_AccNo")
                    {
                        Query = Query.Where(x => x.Rfnd_AccNo.StartsWith(item.value));
                    }
                   
                }
            }
            totalCount = Query.Count();
            int skip = page - 1;

            Query = Query.OrderBy(sort, order).Skip(rows * skip).Take(rows);
            var datarows = Query.Select(n => new
            {
                n.Id,
                n.Unn_Soc_Cr_Cd,
                n.AcctBeginDate,
                n.CustName,
                n.AR_Lmt,
                n.Lmt_ExDat,
                n.LoanBal,
                n.Rfnd_AccNo,
                n.UploadDate,
                n.AddUser,
                n.AddDate,
                n.LastEditUser,
                n.LastEditDate,
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }
    }
}