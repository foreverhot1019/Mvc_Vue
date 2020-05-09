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
    public class OperatePointListQuery : QueryObject<OperatePointList>
    {
        public OperatePointListQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.ID.ToString().Contains(search) || 
                    x.OperatePointID.ToString().Contains(search) || 
                    x.OperatePointCode.ToString().Contains(search) ||
                    x.CompanyCode.Contains(search) ||
                    x.CompanyName.Contains(search) || 
                    x.ADDTS.ToString().Contains(search) || 
                    x.ADDID.Contains(search) || 
                    x.ADDWHO.Contains(search) || 
                    x.EDITWHO.Contains(search) || 
                    x.EDITID.Contains(search) || 
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public OperatePointListQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.ID.ToString().Contains(search) ||
                    x.OperatePointID.ToString().Contains(search) ||
                    x.OperatePointCode.ToString().Contains(search) ||
                    x.CompanyCode.Contains(search) ||
                    x.CompanyName.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.ADDID.Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITID.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public OperatePointListQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    if (rule.field == "ID" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.ID == val);
                    }

                    if (rule.field == "OperatePointID" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.OperatePointID == val);
                    }

                    if (rule.field == "OperatePointCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.OperatePointCode.Contains(rule.value));
                    }

                    if (rule.field == "CompanyCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CompanyCode.Contains(rule.value));
                    }

                    if (rule.field == "CompanyName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CompanyName.Contains(rule.value));
                    }

                    if (rule.field == "IsEnabled" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.IsEnabled == boolval);
                    }

                    if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ADDTS > date);
                    }

                    if (rule.field == "ADDID" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ADDID.Contains(rule.value));
                    }

                    if (rule.field == "ADDWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ADDWHO.Contains(rule.value));
                    }

                    if (rule.field == "EDITWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EDITWHO.Contains(rule.value));
                    }

                    if (rule.field == "EDITID" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EDITID.Contains(rule.value));
                    }

                    if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.EDITTS > date);
                    }
                }
            }
            return this;
        }
    }
}



