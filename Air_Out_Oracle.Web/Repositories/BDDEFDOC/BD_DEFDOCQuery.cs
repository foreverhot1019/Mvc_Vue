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
    public class BD_DEFDOCQuery : QueryObject<BD_DEFDOC>
    {
        public BD_DEFDOCQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.ID.ToString().Contains(search) || 
                    x.DOCCODE.Contains(search) || 
                    x.DOCNAME.Contains(search) || 
                    x.REMARK.Contains(search) || 
                    x.STATUS.Contains(search)||
                    x.ADDWHO.Contains(search) || 
                    x.ADDTS.ToString().Contains(search) || 
                    x.EDITWHO.Contains(search) || 
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public BD_DEFDOCQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.ID.ToString().Contains(search) ||
                    x.DOCCODE.Contains(search) ||
                    x.DOCNAME.Contains(search) ||
                    x.REMARK.Contains(search) ||
                    x.STATUS.Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public BD_DEFDOCQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    if (rule.field == "ROWID" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.ID == val);
                    }

                    if (rule.field == "DOCCODE" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.DOCCODE.Contains(rule.value));
                    }

                    if (rule.field == "DOCNAME" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.DOCNAME.Contains(rule.value));
                    }

                    if (rule.field == "REMARK" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.REMARK.Contains(rule.value));
                    }
                    if (rule.field == "STATUS" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.REMARK.Contains(rule.value));
                    }

                    if (rule.field == "ADDWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ADDWHO.Contains(rule.value));
                    }

                    if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ADDTS >= date);
                    }

                    if (rule.field == "EDITWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EDITWHO.Contains(rule.value));
                    }

                    if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
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



