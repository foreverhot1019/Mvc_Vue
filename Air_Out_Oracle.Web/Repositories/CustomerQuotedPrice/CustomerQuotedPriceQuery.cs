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
   public class CustomerQuotedPriceQuery:QueryObject<CustomerQuotedPrice>
    {
        public CustomerQuotedPriceQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                     x.Id.ToString().Contains(search) || 
                     x.SerialNo.Contains(search) || 
                     x.BusinessType.Contains(search) || 
                     x.CustomerCode.Contains(search) || 
                     x.LocalWHMark.Contains(search) || 
                     x.StartPlace.Contains(search) || 
                     x.TransitPlace.Contains(search) || 
                     x.EndPlace.Contains(search) || 
                     x.ProxyOperator.Contains(search) || 
                     x.CusDefinition.Contains(search) || 
                     x.Receiver.Contains(search) || 
                     x.Shipper.Contains(search) || 
                     x.Contact.Contains(search) || 
                     x.QuotedPricePolicy.Contains(search) || 
                     x.Seller.Contains(search) || 
                     x.StartDate.ToString().Contains(search) || 
                     x.EndDate.ToString().Contains(search) || 
                     x.Description.Contains(search) || 
                     x.OperatingPoint.ToString().Contains(search) || 
                     x.ADDWHO.Contains(search) || 
                     x.ADDTS.ToString().Contains(search) || 
                     x.EDITWHO.Contains(search) || 
                     x.EDITTS.ToString().Contains(search) );
                     
            return this;
        }

		public CustomerQuotedPriceQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                     x.Id.ToString().Contains(search) || 
                     x.SerialNo.Contains(search) || 
                     x.BusinessType.Contains(search) || 
                     x.CustomerCode.Contains(search) || 
                     x.LocalWHMark.Contains(search) || 
                     x.StartPlace.Contains(search) || 
                     x.TransitPlace.Contains(search) || 
                     x.EndPlace.Contains(search) || 
                     x.ProxyOperator.Contains(search) || 
                     x.CusDefinition.Contains(search) || 
                     x.Receiver.Contains(search) || 
                     x.Shipper.Contains(search) || 
                     x.Contact.Contains(search) || 
                     x.QuotedPricePolicy.Contains(search) || 
                     x.Seller.Contains(search) || 
                     x.StartDate.ToString().Contains(search) || 
                     x.EndDate.ToString().Contains(search) || 
                     x.Description.Contains(search) || 
                     x.OperatingPoint.ToString().Contains(search) || 
                     x.ADDWHO.Contains(search) || 
                     x.ADDTS.ToString().Contains(search) || 
                     x.EDITWHO.Contains(search) || 
                     x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public CustomerQuotedPriceQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "SerialNo" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.SerialNo.StartsWith(rule.value));
					}
					if (rule.field == "BusinessType" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.BusinessType.StartsWith(rule.value));
					}
					if (rule.field == "CustomerCode" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CustomerCode.StartsWith(rule.value));
					}
					if (rule.field == "LocalWHMark" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.LocalWHMark.StartsWith(rule.value));
					}
					if (rule.field == "StartPlace" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.StartPlace.StartsWith(rule.value));
					}
					if (rule.field == "TransitPlace" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.TransitPlace.StartsWith(rule.value));
					}
					if (rule.field == "EndPlace" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.EndPlace.StartsWith(rule.value));
					}
					if (rule.field == "ProxyOperator" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.ProxyOperator.StartsWith(rule.value));
					}
					if (rule.field == "CusDefinition" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CusDefinition.StartsWith(rule.value));
					}
					if (rule.field == "Receiver" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Receiver.StartsWith(rule.value));
					}
					if (rule.field == "Shipper" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Shipper.StartsWith(rule.value));
					}
					if (rule.field == "Contact" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Contact.StartsWith(rule.value));
					}
					if (rule.field == "QuotedPricePolicy" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.QuotedPricePolicy.StartsWith(rule.value));
					}
					if (rule.field == "Seller" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Seller.StartsWith(rule.value));
					}
					if (rule.field == "StartDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.StartDate == date);
					}
					if (rule.field == "_StartDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.StartDate >= date);
					}
					if (rule.field == "StartDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.StartDate <= date);
					}
					if (rule.field == "EndDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.EndDate == date);
					}
					if (rule.field == "_EndDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.EndDate >= date);
					}
					if (rule.field == "EndDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.EndDate <= date);
					}
					if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Description.StartsWith(rule.value));
					}
                    if (rule.field == "AuditStatus" && !string.IsNullOrEmpty(rule.value))
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.AuditStatusEnum>(rule.value);
                        And(x => x.AuditStatus == EnumVal);
                    }
                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value))
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.UseStatusEnum>(rule.value);
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