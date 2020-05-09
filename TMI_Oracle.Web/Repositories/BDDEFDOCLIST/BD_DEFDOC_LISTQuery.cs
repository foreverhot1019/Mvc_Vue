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
    public class BD_DEFDOC_LISTQuery : QueryObject<BD_DEFDOC_LIST>
    {
        public BD_DEFDOC_LISTQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.ID.ToString().Contains(search) ||
                    x.DOCID.ToString().Contains(search) ||
                    x.LISTCODE.Contains(search) ||
                    x.LISTNAME.Contains(search) ||
                    x.ListFullName.Contains(search)||
                    x.REMARK.Contains(search) ||
                    x.STATUS.Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search) ||
                    x.R_CODE.Contains(search) ||
                    x.ENAME.Contains(search));
            return this;
        }

        public BD_DEFDOC_LISTQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.ID.ToString().Contains(search) ||
                    x.DOCID.ToString().Contains(search) ||
                    x.LISTCODE.Contains(search) ||
                    x.LISTNAME.Contains(search) ||
                    x.ListFullName.Contains(search) ||
                    x.REMARK.Contains(search) ||
                    x.STATUS.Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search) ||
                    x.R_CODE.Contains(search) ||
                    x.ENAME.Contains(search));
            return this;
        }

        public BD_DEFDOC_LISTQuery Withfilter(IEnumerable<filterRule> filters)
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

                    if (rule.field == "DOCID" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.DOCID == val);
                    }

                    if (rule.field == "LISTCODE" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.LISTCODE.Contains(rule.value));
                    }

                    if (rule.field == "LISTNAME" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.LISTNAME.Contains(rule.value));
                    }
                    if (rule.field == "ListFullName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.LISTNAME.Contains(rule.value));
                    }
                    if (rule.field == "REMARK" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.REMARK.Contains(rule.value));
                    }

                    if (rule.field == "STATUS" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.STATUS.Contains(rule.value));
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

                    if (rule.field == "R_CODE" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.R_CODE.Contains(rule.value));
                    }

                    if (rule.field == "ENAME" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ENAME.Contains(rule.value));
                    }

                }
            }
            return this;
        }
    }
}



