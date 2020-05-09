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
    public class PARA_CountryQuery : QueryObject<PARA_Country>
    {
        public PARA_CountryQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.COUNTRY_CO.Contains(search) ||
                x.COUNTRY_EN.Contains(search) ||
                x.COUNTRY_NA.Contains(search) ||
                x.EXAM_MARK.Contains(search) ||
                x.HIGH_LOW.Contains(search));
            return this;
        }

        public PARA_CountryQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.COUNTRY_CO.Contains(search) ||
                x.COUNTRY_EN.Contains(search) ||
                x.COUNTRY_NA.Contains(search) ||
                x.EXAM_MARK.Contains(search) ||
                x.HIGH_LOW.Contains(search));
            return this;
        }

        public PARA_CountryQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    if (rule.field == "COUNTRY_CO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.COUNTRY_CO.StartsWith(rule.value));
                    }
                    if (rule.field == "COUNTRY_EN" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.COUNTRY_EN.StartsWith(rule.value));
                    }
                    if (rule.field == "COUNTRY_NA" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.COUNTRY_NA.StartsWith(rule.value));
                    }
                    if (rule.field == "EXAM_MARK" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EXAM_MARK.StartsWith(rule.value));
                    }
                    if (rule.field == "HIGH_LOW" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.HIGH_LOW.StartsWith(rule.value));
                    }
                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Status == boolval);
                    }
                    //if (rule.field == "ADDWHO" && !string.IsNullOrEmpty(rule.value))
                    //{
                    //	 And(x => x.ADDWHO.StartsWith(rule.value));
                    //}
                    //if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{	
                    //	 var date = Convert.ToDateTime(rule.value);
                    //	 And(x => x.ADDTS == date);
                    //}
                    //if (rule.field == "_ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{	
                    //	 var date = Convert.ToDateTime(rule.value);
                    //	 And(x => x.ADDTS >= date);
                    //}
                    //if (rule.field == "ADDTS_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{	
                    //	 var date = Convert.ToDateTime(rule.value);
                    //	 And(x => x.ADDTS <= date);
                    //}
                    //if (rule.field == "EDITWHO" && !string.IsNullOrEmpty(rule.value))
                    //{
                    //	 And(x => x.EDITWHO.StartsWith(rule.value));
                    //}
                    //if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{	
                    //	 var date = Convert.ToDateTime(rule.value);
                    //	 And(x => x.EDITTS == date);
                    //}
                    //if (rule.field == "_EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{	
                    //	 var date = Convert.ToDateTime(rule.value);
                    //	 And(x => x.EDITTS >= date);
                    //}
                    //if (rule.field == "EDITTS_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{	
                    //	 var date = Convert.ToDateTime(rule.value);
                    //	 And(x => x.EDITTS <= date);
                    //}
                }
            }
            return this;
        }
    }
}