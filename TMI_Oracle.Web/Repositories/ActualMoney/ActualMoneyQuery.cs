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
    public class ActualMoneyQuery : QueryObject<ActualMoney>
    {
        public ActualMoneyQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.OrderId.ToString().Contains(search) ||
                x.SupplierName.Contains(search) ||
                x.ServiceProject.Contains(search) ||
                x.Price.ToString().Contains(search) ||
                x.Num.ToString().Contains(search) ||
                x.TotalAmount.ToString().Contains(search) ||
                x.RequestAmount.ToString().Contains(search) ||
                x.ExcessAmount.ToString().Contains(search) ||
                x.Remark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public ActualMoneyQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.OrderId.ToString().Contains(search) ||
                x.SupplierName.Contains(search) ||
                x.ServiceProject.Contains(search) ||
                x.Price.ToString().Contains(search) ||
                x.Num.ToString().Contains(search) ||
                x.TotalAmount.ToString().Contains(search) ||
                x.RequestAmount.ToString().Contains(search) ||
                x.ExcessAmount.ToString().Contains(search) ||
                x.Remark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public ActualMoneyQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    //if (rule.field == "OOrder" && !string.IsNullOrEmpty(rule.value))
                    //{	
                    //    And(x => x.OOrder == rule.value);
                    //}
                    if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Id == val);
                    }
                    if (rule.field == "OrderId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.OrderId == val);
                    }
                    if (rule.field == "SupplierName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.SupplierName.StartsWith(rule.value));
                    }
                    if (rule.field == "ServiceProject" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ServiceProject.StartsWith(rule.value));
                    }
                    if (rule.field == "Price" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.Price == val);
                    }
                    if (rule.field == "Num" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.Num == val);
                    }
                    if (rule.field == "TotalAmount" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.TotalAmount == val);
                    }
                    if (rule.field == "RequestAmount" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.RequestAmount == val);
                    }
                    if (rule.field == "ExcessAmount" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.ExcessAmount == val);
                    }
                    if (rule.field == "Remark" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Remark.StartsWith(rule.value));
                    }
                    if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.OperatingPoint == val);
                    }
                    //if (rule.field == "ADDID" && !string.IsNullOrEmpty(rule.value))
                    //{
                    //	 And(x => x.ADDID.StartsWith(rule.value));
                    //}
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
                    //if (rule.field == "EDITID" && !string.IsNullOrEmpty(rule.value))
                    //{
                    //	 And(x => x.EDITID.StartsWith(rule.value));
                    //}
                }
            }
            return this;
        }
    }
}