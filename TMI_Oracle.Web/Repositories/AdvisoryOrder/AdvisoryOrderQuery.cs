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
    public class AdvisoryOrderQuery : QueryObject<AdvisoryOrder>
    {
        public AdvisoryOrderQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.OrderNo.Contains(search) ||
                x.RouteNo.Contains(search) ||
                //x.STravleDate.ToString().Contains(search) ||
                //x.ETravleDate.ToString().Contains(search) ||
                x.TravlePersonNum.ToString().Contains(search) ||
                x.UnitPrice.ToString().Contains(search) ||
                x.TotalPrice.ToString().Contains(search) ||
                x.RoutePhoto.Contains(search) ||
                x.Remark.Contains(search) ||
                x.CustomerId.ToString().Contains(search) ||
                x.CustomerName.Contains(search) ||
                x.Contact.Contains(search) ||
                x.Saller.Contains(search) ||
                x.OP.Contains(search) ||
                x.ComponyName.Contains(search) ||
                x.ComponyId.ToString().Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public AdvisoryOrderQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.OrderNo.Contains(search) ||
                x.RouteNo.Contains(search) ||
                //x.STravleDate.ToString().Contains(search) ||
                //x.ETravleDate.ToString().Contains(search) ||
                x.TravlePersonNum.ToString().Contains(search) ||
                x.UnitPrice.ToString().Contains(search) ||
                x.TotalPrice.ToString().Contains(search) ||
                x.RoutePhoto.Contains(search) ||
                x.Remark.Contains(search) ||
                x.CustomerId.ToString().Contains(search) ||
                x.CustomerName.Contains(search) ||
                x.Contact.Contains(search) ||
                x.Saller.Contains(search) ||
                x.OP.Contains(search) ||
                x.ComponyName.Contains(search) ||
                x.ComponyId.ToString().Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public AdvisoryOrderQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    //if (rule.field == "OCompany" && !string.IsNullOrEmpty(rule.value))
                    //{	
                    //    And(x => x.OCompany == rule.value);
                    //}
                    //if (rule.field == "OCustomer" && !string.IsNullOrEmpty(rule.value))
                    //{	
                    //    And(x => x.OCustomer == rule.value);
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
                    if (rule.field == "RouteNo" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.RouteNo.StartsWith(rule.value));
                    }
                    //if (rule.field == "STravleDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{
                    //    var date = Convert.ToDateTime(rule.value);
                    //    And(x => x.STravleDate == date);
                    //}
                    //if (rule.field == "_STravleDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{
                    //    var date = Convert.ToDateTime(rule.value);
                    //    And(x => x.STravleDate >= date);
                    //}
                    //if (rule.field == "STravleDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{
                    //    var date = Convert.ToDateTime(rule.value);
                    //    And(x => x.STravleDate <= date);
                    //}
                    //if (rule.field == "ETravleDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{
                    //    var date = Convert.ToDateTime(rule.value);
                    //    And(x => x.ETravleDate == date);
                    //}
                    //if (rule.field == "_ETravleDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{
                    //    var date = Convert.ToDateTime(rule.value);
                    //    And(x => x.ETravleDate >= date);
                    //}
                    //if (rule.field == "ETravleDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    //{
                    //    var date = Convert.ToDateTime(rule.value);
                    //    And(x => x.ETravleDate <= date);
                    //}
                    if (rule.field == "TravleOrdType")
                    {
                        EnumType.TravleOrdTypeEnum TravleOrdType;
                        if(Enum.TryParse<EnumType.TravleOrdTypeEnum>(rule.value,out TravleOrdType)){
                            And(x => x.TravleOrdType == TravleOrdType);
                        }
                    }
                    if (rule.field == "TravleType")
                    {
                        EnumType.TravleTypeEnum TravleType;
                        if (Enum.TryParse<EnumType.TravleTypeEnum>(rule.value, out TravleType))
                        {
                            And(x => x.TravleType == TravleType);
                        }
                    }
                    if (rule.field == "Status")
                    {
                        EnumType.OrderStatusEnum Status;
                        if (Enum.TryParse<EnumType.OrderStatusEnum>(rule.value, out Status))
                        {
                            And(x => x.Status == Status);
                        }
                    }
                    if (rule.field == "ContactType")
                    {
                        EnumType.ContactType ContactType;
                        if (Enum.TryParse<EnumType.ContactType>(rule.value, out ContactType))
                        {
                            And(x => x.ContactType == ContactType);
                        }
                    }
                    if (rule.field == "RouteName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.RouteName.StartsWith(rule.value));
                    }
                    if (rule.field == "CustomerRequire" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CustomerRequire.Contains(rule.value));
                    }
                    if (rule.field == "TravlePersonNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.TravlePersonNum == val);
                    }
                    if (rule.field == "UnitPrice" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.UnitPrice == val);
                    }
                    if (rule.field == "TotalPrice" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.TotalPrice == val);
                    }
                    if (rule.field == "RoutePhoto" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.RoutePhoto.StartsWith(rule.value));
                    }
                    if (rule.field == "Remark" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Remark.StartsWith(rule.value));
                    }
                    if (rule.field == "CustomerId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.CustomerId == val);
                    }
                    if (rule.field == "CustomerName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CustomerName.StartsWith(rule.value));
                    }
                    if (rule.field == "Contact" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Contact.StartsWith(rule.value));
                    }
                    if (rule.field == "Saller" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Saller.StartsWith(rule.value));
                    }
                    if (rule.field == "OP" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.OP.StartsWith(rule.value));
                    }
                    if (rule.field == "ComponyName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ComponyName.StartsWith(rule.value));
                    }
                    if (rule.field == "ComponyId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.ComponyId == val);
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