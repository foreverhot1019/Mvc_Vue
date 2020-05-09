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
   public class AirLineQuery:QueryObject<AirLine>
    {
        public AirLineQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.AirTicketOrderId.ToString().Contains(search) || 
x.AirCompany.Contains(search) || 
x.Flight_No.Contains(search) || 
x.City.Contains(search) || 
x.Position.Contains(search) || 
x.Flight_Date_Want.ToString().Contains(search) || 
x.TakeOffTime.ToString().Contains(search) || 
x.ArrivalTime.ToString().Contains(search) || 
x.TicketPrice.ToString().Contains(search) || 
x.BillTaxAmount.ToString().Contains(search) || 
x.CostMoney.ToString().Contains(search) || 
x.SellPrice.ToString().Contains(search) || 
x.TicketNum.Contains(search) || 
x.Profit.ToString().Contains(search) || 
x.Insurance.ToString().Contains(search) || 
x.EndorseDate.ToString().Contains(search) || 
x.EndorseWho.Contains(search) || 
x.ReturnTicketDate.ToString().Contains(search) || 
x.ReturnTicketWho.Contains(search) || 
x.CancelDate.ToString().Contains(search) || 
x.CancelWho.Contains(search) || 
x.Remark.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public AirLineQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.AirTicketOrderId.ToString().Contains(search) || 
x.AirCompany.Contains(search) || 
x.Flight_No.Contains(search) || 
x.City.Contains(search) || 
x.Position.Contains(search) || 
x.Flight_Date_Want.ToString().Contains(search) || 
x.TakeOffTime.ToString().Contains(search) || 
x.ArrivalTime.ToString().Contains(search) || 
x.TicketPrice.ToString().Contains(search) || 
x.BillTaxAmount.ToString().Contains(search) || 
x.CostMoney.ToString().Contains(search) || 
x.SellPrice.ToString().Contains(search) || 
x.TicketNum.Contains(search) || 
x.Profit.ToString().Contains(search) || 
x.Insurance.ToString().Contains(search) || 
x.EndorseDate.ToString().Contains(search) || 
x.EndorseWho.Contains(search) || 
x.ReturnTicketDate.ToString().Contains(search) || 
x.ReturnTicketWho.Contains(search) || 
x.CancelDate.ToString().Contains(search) || 
x.CancelWho.Contains(search) || 
x.Remark.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public AirLineQuery Withfilter(IEnumerable<filterRule> filters)
        {
           if (filters != null)
           {
				foreach (var rule in filters)
				{
					//if (rule.field == "OAirTicket" && !string.IsNullOrEmpty(rule.value))
					//{	
					//    And(x => x.OAirTicket == rule.value);
					//}
					if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.Id == val);
					}
					if (rule.field == "AirTicketOrderId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.AirTicketOrderId == val);
					}
					if (rule.field == "AirCompany" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.AirCompany.StartsWith(rule.value));
					}
					if (rule.field == "Flight_No" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Flight_No.StartsWith(rule.value));
					}
					if (rule.field == "City" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.City.StartsWith(rule.value));
					}
					if (rule.field == "Position" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Position.StartsWith(rule.value));
					}
					if (rule.field == "Flight_Date_Want" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Flight_Date_Want == date);
					}
					if (rule.field == "_Flight_Date_Want" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Flight_Date_Want >= date);
					}
					if (rule.field == "Flight_Date_Want_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Flight_Date_Want <= date);
					}
					if (rule.field == "TakeOffTime" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.TakeOffTime == date);
					}
					if (rule.field == "_TakeOffTime" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.TakeOffTime >= date);
					}
					if (rule.field == "TakeOffTime_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.TakeOffTime <= date);
					}
					if (rule.field == "ArrivalTime" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ArrivalTime == date);
					}
					if (rule.field == "_ArrivalTime" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ArrivalTime >= date);
					}
					if (rule.field == "ArrivalTime_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ArrivalTime <= date);
					}
					if (rule.field == "TicketPrice" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.TicketPrice == val);
					}
					if (rule.field == "BillTaxAmount" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.BillTaxAmount == val);
					}
					if (rule.field == "CostMoney" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.CostMoney == val);
					}
					if (rule.field == "SellPrice" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.SellPrice == val);
					}
					if (rule.field == "TicketNum" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.TicketNum.StartsWith(rule.value));
					}
					if (rule.field == "Profit" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Profit == val);
					}
					if (rule.field == "Insurance" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Insurance == val);
					}
					if (rule.field == "Is_Endorse" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_Endorse == boolval);
					}
					if (rule.field == "EndorseDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.EndorseDate == date);
					}
					if (rule.field == "_EndorseDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.EndorseDate >= date);
					}
					if (rule.field == "EndorseDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.EndorseDate <= date);
					}
					if (rule.field == "EndorseWho" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.EndorseWho.StartsWith(rule.value));
					}
					if (rule.field == "Is_ReturnTicket" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_ReturnTicket == boolval);
					}
					if (rule.field == "ReturnTicketDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ReturnTicketDate == date);
					}
					if (rule.field == "_ReturnTicketDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ReturnTicketDate >= date);
					}
					if (rule.field == "ReturnTicketDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ReturnTicketDate <= date);
					}
					if (rule.field == "ReturnTicketWho" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.ReturnTicketWho.StartsWith(rule.value));
					}
					if (rule.field == "Is_Cancel" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Is_Cancel == boolval);
					}
					if (rule.field == "CancelDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.CancelDate == date);
					}
					if (rule.field == "_CancelDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.CancelDate >= date);
					}
					if (rule.field == "CancelDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.CancelDate <= date);
					}
					if (rule.field == "CancelWho" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CancelWho.StartsWith(rule.value));
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