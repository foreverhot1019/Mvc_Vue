using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.SqlServer;
using Repository.Pattern.Repositories;
using Repository.Pattern.Ef6;
using System.Web.WebPages;
using TMI.Web.Models;
using TMI.Web.Extensions;

namespace TMI.Web.Repositories
{
    public class RateQuery : QueryObject<Rate>
    {
        public RateQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                    x.Id.ToString().Contains(search) ||
                    x.LocalCurrency.Contains(search) ||
                    x.LocalCurrCode.Contains(search) ||
                    x.ForeignCurrency.Contains(search) ||
                    x.ForeignCurrCode.Contains(search) ||
                    x.Year.ToString().Contains(search) ||
                    x.Month.ToString().Contains(search) ||
                    x.RecRate.ToString().Contains(search) ||
                    x.PayRate.ToString().Contains(search) ||
                    x.Status.ToString().Contains(search) ||
                    x.Description.Contains(search) ||
                    x.OperatingPoint.ToString().Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public RateQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                    x.Id.ToString().Contains(search) ||
                    x.LocalCurrency.Contains(search) ||
                    x.LocalCurrCode.Contains(search) ||
                    x.ForeignCurrency.Contains(search) ||
                    x.ForeignCurrCode.Contains(search) ||
                    x.Year.ToString().Contains(search) ||
                    x.Month.ToString().Contains(search) ||
                    x.RecRate.ToString().Contains(search) ||
                    x.PayRate.ToString().Contains(search) ||
                    x.Status.ToString().Contains(search) ||
                    x.Description.Contains(search) ||
                    x.OperatingPoint.ToString().Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public RateQuery Withfilter(IEnumerable<filterRule> filters)
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
                    if (rule.field == "LocalCurrency" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.LocalCurrency.StartsWith(rule.value));
                    }
                    if (rule.field == "LocalCurrCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.LocalCurrCode.StartsWith(rule.value));
                    }
                    if (rule.field == "ForeignCurrency" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ForeignCurrency.StartsWith(rule.value));
                    }
                    if (rule.field == "ForeignCurrCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ForeignCurrCode.StartsWith(rule.value));
                    }
                    if (rule.field == "Year" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Year == val);
                    }
                    if (rule.field == "Month" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Month == val);
                    }
                    if (rule.field == "RecRate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.RecRate == val);
                    }
                    if (rule.field == "PayRate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.PayRate == val);
                    }
                    if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Description.StartsWith(rule.value));
                    }
                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        var EnumVal = Common.GetEnumVal<EnumType.UseStatusEnum>(rule.value);
                        //var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Status == EnumVal);
                    }
                    if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.OperatingPoint == val);
                    }
                    if (rule.field == "ADDWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ADDWHO.StartsWith(rule.value));
                    }
                    if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ADDTS == date);
                    }
                    if (rule.field == "_ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ADDTS >= date);
                    }
                    if (rule.field == "ADDTS_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ADDTS <= date);
                    }
                    if (rule.field == "EDITWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EDITWHO.StartsWith(rule.value));
                    }
                    if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.EDITTS == date);
                    }
                    if (rule.field == "_EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.EDITTS >= date);
                    }
                    if (rule.field == "EDITTS_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.EDITTS <= date);
                    }
                }
            }
            return this;
        }
    }
}