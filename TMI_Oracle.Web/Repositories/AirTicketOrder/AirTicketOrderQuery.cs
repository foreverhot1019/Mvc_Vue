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
   public class AirTicketOrderQuery:QueryObject<AirTicketOrder>
    {
        public AirTicketOrderQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                x.Id.ToString().Contains(search) || 
                x.AirTicketNo.Contains(search) || 
                x.CompanyCIQID.Contains(search) || 
                x.CompanyName.Contains(search) || 
                x.Saller.Contains(search) || 
                x.PlanePerson.Contains(search) || 
                x.PNR.Contains(search) || 
                x.TravlePersonNum.ToString().Contains(search) || 
                x.ExpectPaymentDate.ToString().Contains(search) || 
                x.SupplierName.Contains(search) || 
                x.AirTicketNo_Org.Contains(search) || 
                x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public AirTicketOrderQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                     x.Id.ToString().Contains(search) || 
                     x.AirTicketNo.Contains(search) || 
                     x.CompanyCIQID.Contains(search) || 
                     x.CompanyName.Contains(search) || 
                     x.Saller.Contains(search) || 
                     x.PlanePerson.Contains(search) || 
                     x.PNR.Contains(search) || 
                     x.TravlePersonNum.ToString().Contains(search) || 
                     x.ExpectPaymentDate.ToString().Contains(search) || 
                     x.SupplierName.Contains(search) || 
                     x.AirTicketNo_Org.Contains(search) || 
                     x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public AirTicketOrderQuery Withfilter(IEnumerable<filterRule> filters)
        {
           if (filters != null)
           {
				foreach (var rule in filters)
				{
					//if (rule.field == "ArrAirLine" && !string.IsNullOrEmpty(rule.value))
					//{	
					//    And(x => x.ArrAirLine == rule.value);
					//}
					//if (rule.field == "ArrPlanePerson" && !string.IsNullOrEmpty(rule.value))
					//{	
					//    And(x => x.ArrPlanePerson == rule.value);
					//}
					if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Id == val);
					}
					if (rule.field == "AirTicketNo" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.AirTicketNo.StartsWith(rule.value));
					}
					if (rule.field == "CompanyCIQID" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CompanyCIQID.StartsWith(rule.value));
					}
					if (rule.field == "CompanyName" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CompanyName.StartsWith(rule.value));
					}
					if (rule.field == "Saller" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Saller.StartsWith(rule.value));
					}
					if (rule.field == "PlanePerson" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.PlanePerson.StartsWith(rule.value));
					}
					if (rule.field == "PNR" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.PNR.StartsWith(rule.value));
					}
					if (rule.field == "TravlePersonNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.TravlePersonNum == val);
					}
					if (rule.field == "ExpectPaymentDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ExpectPaymentDate == date);
					}
					if (rule.field == "_ExpectPaymentDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ExpectPaymentDate >= date);
					}
					if (rule.field == "ExpectPaymentDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ExpectPaymentDate <= date);
					}
					if (rule.field == "SupplierName" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.SupplierName.StartsWith(rule.value));
					}
					if (rule.field == "AirTicketNo_Org" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.AirTicketNo_Org.StartsWith(rule.value));
					}
					if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.OperatingPoint == val);
					}
                    if (rule.field == "AirTicketOrdType")
                    {
                        EnumType.AirTicketOrdTypeEnum AirTicketOrdType;
                        if (Enum.TryParse<EnumType.AirTicketOrdTypeEnum>(rule.value, out AirTicketOrdType))
                        {
                            And(x => x.AirTicketOrdType == AirTicketOrdType);
                        }
                    }
                    if (rule.field == "TicketType")
                    {
                        EnumType.TicketTypeEnum TicketType;
                        if (Enum.TryParse<EnumType.TicketTypeEnum>(rule.value, out TicketType))
                        {
                            And(x => x.TicketType == TicketType);
                        }
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