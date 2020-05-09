using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.SqlServer;
using Repository.Pattern.Repositories;
using Repository.Pattern.Ef6;
using System.Web.WebPages;
using AirOut.Web.Models;
using AirOut.Web.Extensions;

namespace AirOut.Web.Repositories
{
    public class PARA_AirLineQuery : QueryObject<PARA_AirLine>
    {
        public PARA_AirLineQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.AirCode.Contains(search) ||
                x.AirLine.Contains(search) ||
                x.AirCompany.Contains(search) ||
                x.StarStation.Contains(search) ||
                x.TransferStation.Contains(search) ||
                x.EndStation.Contains(search) ||
                x.AirDate.ToString().Contains(search) ||
                x.Description.Contains(search) ||
                x.ADDWHO.Contains(search) ||
                x.ADDTS.ToString().Contains(search) ||
                x.EDITWHO.Contains(search) ||
                x.EDITTS.ToString().Contains(search));
            return this;
        }

        public PARA_AirLineQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.AirCode.Contains(search) ||
                x.AirLine.Contains(search) ||
                x.AirCompany.Contains(search) ||
                x.StarStation.Contains(search) ||
                x.TransferStation.Contains(search) ||
                x.EndStation.Contains(search) ||
                x.AirDate.ToString().Contains(search) ||
                x.Description.Contains(search) ||
                x.ADDWHO.Contains(search) ||
                x.ADDTS.ToString().Contains(search) ||
                x.EDITWHO.Contains(search) ||
                x.EDITTS.ToString().Contains(search));
            return this;
        }

        public PARA_AirLineQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Id == val);
                    }
                    if (rule.field == "AirCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.AirCode.Contains(rule.value));
                    }
                    if (rule.field == "AirLine" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.AirLine.Contains(rule.value));
                    }
                    if (rule.field == "AirCompany" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.AirCompany.Contains(rule.value));
                    }
                    if (rule.field == "StarStation" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.StarStation.Contains(rule.value));
                    }
                    if (rule.field == "TransferStation" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TransferStation.Contains(rule.value));
                    }
                    if (rule.field == "EndStation" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EndStation.Contains(rule.value));
                    }
                    if (rule.field == "AirDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => SqlFunctions.DateDiff("d", date, x.AirDate) >= 0);
                    }
                    if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Description.Contains(rule.value));
                    }
                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.UseStatusEnum>(rule.value);
                        //int boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Status == EnumVal);
                    }
                    if (rule.field == "ADDWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ADDWHO.Contains(rule.value));
                    }
                    if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => SqlFunctions.DateDiff("d", date, x.ADDTS) >= 0);
                    }
                    if (rule.field == "EDITWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EDITWHO.Contains(rule.value));
                    }
                    if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => SqlFunctions.DateDiff("d", date, x.EDITTS) >= 0);
                    }
                }
            }
            return this;
        }
    }
}