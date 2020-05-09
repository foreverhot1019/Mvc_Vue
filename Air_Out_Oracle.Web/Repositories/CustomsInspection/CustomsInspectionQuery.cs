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
   public class CustomsInspectionQuery:QueryObject<CustomsInspection>
    {
        public CustomsInspectionQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                        x.Id.ToString().Contains(search) || 
                        x.Operation_ID.Contains(search) || 
                        x.Flight_NO.Contains(search) || 
                        x.Flight_Date_Want.ToString().Contains(search) || 
                        x.MBL.Contains(search) || 
                        x.Consign_Code_CK.Contains(search) || 
                        x.Book_Flat_Code.Contains(search) || 
                        x.Customs_Declaration.Contains(search) || 
                        x.Num_BG.ToString().Contains(search) || 
                        x.Remarks_BG.Contains(search) || 
                        x.Customs_Broker_BG.Contains(search) || 
                        x.Customs_Date_BG.ToString().Contains(search) || 
                        x.Pieces_TS.ToString().Contains(search) || 
                        x.Weight_TS.ToString().Contains(search) || 
                        x.Volume_TS.ToString().Contains(search) || 
                        x.Pieces_Fact.ToString().Contains(search) || 
                        x.Weight_Fact.ToString().Contains(search) || 
                        x.Volume_Fact.ToString().Contains(search) || 
                        x.Pieces_BG.ToString().Contains(search) || 
                        x.Weight_BG.ToString().Contains(search) || 
                        x.Volume_BG.ToString().Contains(search) || 
                        x.Check_QTY.ToString().Contains(search) || 
                        x.Check_Date.ToString().Contains(search) || 
                        x.Fileupload.Contains(search) || 
                        x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public CustomsInspectionQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
                        x.Id.ToString().Contains(search) || 
                        x.Operation_ID.Contains(search) || 
                        x.Flight_NO.Contains(search) || 
                        x.Flight_Date_Want.ToString().Contains(search) || 
                        x.MBL.Contains(search) || 
                        x.Consign_Code_CK.Contains(search) || 
                        x.Book_Flat_Code.Contains(search) || 
                        x.Customs_Declaration.Contains(search) || 
                        x.Num_BG.ToString().Contains(search) || 
                        x.Remarks_BG.Contains(search) || 
                        x.Customs_Broker_BG.Contains(search) || 
                        x.Customs_Date_BG.ToString().Contains(search) || 
                        x.Pieces_TS.ToString().Contains(search) || 
                        x.Weight_TS.ToString().Contains(search) || 
                        x.Volume_TS.ToString().Contains(search) || 
                        x.Pieces_Fact.ToString().Contains(search) || 
                        x.Weight_Fact.ToString().Contains(search) || 
                        x.Volume_Fact.ToString().Contains(search) || 
                        x.Pieces_BG.ToString().Contains(search) || 
                        x.Weight_BG.ToString().Contains(search) || 
                        x.Volume_BG.ToString().Contains(search) || 
                        x.Check_QTY.ToString().Contains(search) || 
                        x.Check_Date.ToString().Contains(search) || 
                        x.Fileupload.Contains(search) || 
                        x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public CustomsInspectionQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "Operation_ID" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Operation_ID.StartsWith(rule.value));
					}
					if (rule.field == "Flight_NO" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Flight_NO.StartsWith(rule.value));
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
					if (rule.field == "MBL" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.MBL.StartsWith(rule.value));
					}
					if (rule.field == "Consign_Code_CK" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Consign_Code_CK.StartsWith(rule.value));
					}
					if (rule.field == "Book_Flat_Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Book_Flat_Code.StartsWith(rule.value));
					}
					if (rule.field == "Customs_Declaration" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Customs_Declaration.StartsWith(rule.value));
					}
					if (rule.field == "Num_BG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Num_BG == val);
					}
					if (rule.field == "Remarks_BG" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Remarks_BG.StartsWith(rule.value));
					}
					if (rule.field == "Customs_Broker_BG" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Customs_Broker_BG.StartsWith(rule.value));
					}
					if (rule.field == "Customs_Date_BG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Customs_Date_BG == date);
					}
					if (rule.field == "_Customs_Date_BG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Customs_Date_BG >= date);
					}
					if (rule.field == "Customs_Date_BG_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Customs_Date_BG <= date);
					}
					if (rule.field == "Pieces_TS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_TS == val);
					}
					if (rule.field == "Weight_TS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_TS == val);
					}
					if (rule.field == "Volume_TS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_TS == val);
					}
					if (rule.field == "Pieces_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_Fact == val);
					}
					if (rule.field == "Weight_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_Fact == val);
					}
					if (rule.field == "Volume_Fact" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_Fact == val);
					}
					if (rule.field == "Pieces_BG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Pieces_BG == val);
					}
					if (rule.field == "Weight_BG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Weight_BG == val);
					}
					if (rule.field == "Volume_BG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Volume_BG == val);
					}
					if (rule.field == "IS_Checked_BG" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.IS_Checked_BG == boolval);
					}
					if (rule.field == "Check_QTY" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Check_QTY == date);
					}
					if (rule.field == "_Check_QTY" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Check_QTY >= date);
					}
					if (rule.field == "Check_QTY_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.Check_QTY <= date);
					}
					if (rule.field == "Check_Date" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Check_Date == val);
					}
					if (rule.field == "Fileupload" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Fileupload.StartsWith(rule.value));
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