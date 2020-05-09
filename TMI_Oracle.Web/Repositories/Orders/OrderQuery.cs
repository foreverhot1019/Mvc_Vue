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
    public class OrderQuery : QueryObject<Order>
    {
        public OrderQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.OrderNo.Contains(search) ||
                x.AdvisoryOrderId.ToString().Contains(search) ||
                x.AdvsyOrdNo.Contains(search) ||
                x.STravleDate.ToString().Contains(search) ||
                x.ETravleDate.ToString().Contains(search) ||
                x.RouteNo.Contains(search) ||
                x.RouteName.Contains(search) ||
                x.Saller.Contains(search) ||
                x.OP.Contains(search) ||
                x.ForeCastPayDate.ToString().Contains(search) ||
                x.SupplyierNo.Contains(search) ||
                x.SupplierName.Contains(search) ||
                x.Contact.Contains(search) ||
                x.RoutePhoto.Contains(search) ||
                x.Remark.Contains(search) ||
                x.AdultNum.ToString().Contains(search) ||
                x.AdultActualNum.ToString().Contains(search) ||
                x.AdultPrice.ToString().Contains(search) ||
                x.BoyNum.ToString().Contains(search) ||
                x.BoyActualNum.ToString().Contains(search) ||
                x.BoyPrice.ToString().Contains(search) ||
                x.ChildNum.ToString().Contains(search) ||
                x.ChildActualNum.ToString().Contains(search) ||
                x.ChildPrice.ToString().Contains(search) ||
                x.PriceRepair.ToString().Contains(search) ||
                x.PriceRepairRemark.Contains(search) ||
                x.TotalRemark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public OrderQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.OrderNo.Contains(search) ||
                x.AdvisoryOrderId.ToString().Contains(search) ||
                x.AdvsyOrdNo.Contains(search) ||
                x.STravleDate.ToString().Contains(search) ||
                x.ETravleDate.ToString().Contains(search) ||
                x.RouteNo.Contains(search) ||
                x.RouteName.Contains(search) ||
                x.Saller.Contains(search) ||
                x.OP.Contains(search) ||
                x.ForeCastPayDate.ToString().Contains(search) ||
                x.SupplyierNo.Contains(search) ||
                x.SupplierName.Contains(search) ||
                x.Contact.Contains(search) ||
                x.RoutePhoto.Contains(search) ||
                x.Remark.Contains(search) ||
                x.AdultNum.ToString().Contains(search) ||
                x.AdultActualNum.ToString().Contains(search) ||
                x.AdultPrice.ToString().Contains(search) ||
                x.BoyNum.ToString().Contains(search) ||
                x.BoyActualNum.ToString().Contains(search) ||
                x.BoyPrice.ToString().Contains(search) ||
                x.ChildNum.ToString().Contains(search) ||
                x.ChildActualNum.ToString().Contains(search) ||
                x.ChildPrice.ToString().Contains(search) ||
                x.PriceRepair.ToString().Contains(search) ||
                x.PriceRepairRemark.Contains(search) ||
                x.TotalRemark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public OrderQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    //if (rule.field == "ArrOrderCustomer" && !string.IsNullOrEmpty(rule.value))
                    //{	
                    //    And(x => x.ArrOrderCustomer == rule.value);
                    //}
                    //if (rule.field == "OAdvisoryOrder" && !string.IsNullOrEmpty(rule.value))
                    //{	
                    //    And(x => x.OAdvisoryOrder == rule.value);
                    //}
                    if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Id == val);
                    }
                    if (rule.field == "OrderNo" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.OrderNo.StartsWith(rule.value));
                    }
                    if (rule.field == "AdvisoryOrderId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.AdvisoryOrderId == val);
                    }
                    if (rule.field == "AdvsyOrdNo" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.AdvsyOrdNo.StartsWith(rule.value));
                    }
                    if (rule.field == "STravleDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.STravleDate == date);
                    }
                    if (rule.field == "_STravleDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.STravleDate >= date);
                    }
                    if (rule.field == "STravleDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.STravleDate <= date);
                    }
                    if (rule.field == "ETravleDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ETravleDate == date);
                    }
                    if (rule.field == "_ETravleDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ETravleDate >= date);
                    }
                    if (rule.field == "ETravleDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ETravleDate <= date);
                    }
                    if (rule.field == "RouteNo" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.RouteNo.StartsWith(rule.value));
                    }
                    if (rule.field == "RouteName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.RouteName.StartsWith(rule.value));
                    }
                    if (rule.field == "Saller" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Saller.StartsWith(rule.value));
                    }
                    if (rule.field == "OP" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.OP.StartsWith(rule.value));
                    }
                    if (rule.field == "ForeCastPayDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ForeCastPayDate == date);
                    }
                    if (rule.field == "_ForeCastPayDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ForeCastPayDate >= date);
                    }
                    if (rule.field == "ForeCastPayDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ForeCastPayDate <= date);
                    }
                    if (rule.field == "SupplyierNo" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.SupplyierNo.StartsWith(rule.value));
                    }
                    if (rule.field == "SupplierName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.SupplierName.StartsWith(rule.value));
                    }
                    if (rule.field == "Contact" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Contact.StartsWith(rule.value));
                    }
                    if (rule.field == "RoutePhoto" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.RoutePhoto.StartsWith(rule.value));
                    }
                    if (rule.field == "Remark" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Remark.StartsWith(rule.value));
                    }
                    if (rule.field == "AdultNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.AdultNum == val);
                    }
                    if (rule.field == "AdultActualNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.AdultActualNum == val);
                    }
                    if (rule.field == "AdultPrice" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.AdultPrice == val);
                    }
                    if (rule.field == "BoyNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.BoyNum == val);
                    }
                    if (rule.field == "BoyActualNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.BoyActualNum == val);
                    }
                    if (rule.field == "BoyPrice" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.BoyPrice == val);
                    }
                    if (rule.field == "ChildNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.ChildNum == val);
                    }
                    if (rule.field == "ChildActualNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.ChildActualNum == val);
                    }
                    if (rule.field == "ChildPrice" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.ChildPrice == val);
                    }
                    if (rule.field == "PriceRepair" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.PriceRepair == val);
                    }
                    if (rule.field == "PriceRepairRemark" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.PriceRepairRemark.StartsWith(rule.value));
                    }
                    if (rule.field == "TotalRemark" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TotalRemark.StartsWith(rule.value));
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