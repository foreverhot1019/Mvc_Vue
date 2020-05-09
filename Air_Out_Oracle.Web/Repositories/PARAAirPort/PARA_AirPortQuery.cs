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
    public class PARA_AirPortQuery : QueryObject<PARA_AirPort>
    {
        public PARA_AirPortQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                    x.Id.ToString().Contains(search) ||
                    x.PortCode.Contains(search) ||
                    x.PortName.Contains(search) ||
                    x.PortNameEng.Contains(search) ||
                    x.PortType.Contains(search) ||
                    x.CountryCode.Contains(search) ||
                    x.Description.Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public PARA_AirPortQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                    x.Id.ToString().Contains(search) ||
                    x.PortCode.Contains(search) ||
                    x.PortName.Contains(search) ||
                    x.PortNameEng.Contains(search) ||
                    x.PortType.Contains(search) ||
                    x.CountryCode.Contains(search) ||
                    x.Description.Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public PARA_AirPortQuery Withfilter(IEnumerable<filterRule> filters)
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
                    if (rule.field == "PortCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.PortCode.StartsWith(rule.value));
                    }
                    if (rule.field == "PortName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.PortName.StartsWith(rule.value));
                    }
                    if (rule.field == "PortNameEng" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.PortNameEng.StartsWith(rule.value));
                    }
                    if (rule.field == "PortType" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.PortType.StartsWith(rule.value));
                    }
                    if (rule.field == "CountryCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CountryCode.StartsWith(rule.value));
                    }
                    if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Description.StartsWith(rule.value));
                    }
                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.UseStatusEnum>(rule.value);
                        //var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Status == EnumVal);
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