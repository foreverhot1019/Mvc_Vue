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
   public class CusQuotedPriceDtlQuery:QueryObject<CusQuotedPriceDtl>
    {
        public CusQuotedPriceDtlQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.CusQPSerialNo.Contains(search) || 
x.CusQPId.ToString().Contains(search) || 
x.QPSerialNo.Contains(search) || 
x.QPId.ToString().Contains(search) || 
x.FeeCode.Contains(search) || 
x.FeeName.Contains(search) || 
x.StartPlace.Contains(search) || 
x.TransitPlace.Contains(search) || 
x.EndPlace.Contains(search) || 
x.AirLineCo.Contains(search) || 
x.AirLineNo.Contains(search) || 
x.WHBuyer.Contains(search) || 
x.DealWithArticle.Contains(search) || 
x.CustomsType.Contains(search) || 
x.MoorLevel.Contains(search) || 
x.BillingUnit.Contains(search) || 
x.Price.ToString().Contains(search) || 
x.CurrencyCode.Contains(search) || 
x.FeeConditionVal1.ToString().Contains(search) || 
x.CalcSign1.Contains(search) || 
x.FeeCondition.Contains(search) || 
x.CalcSign2.Contains(search) || 
x.FeeConditionVal2.ToString().Contains(search) || 
x.CalcFormula.Contains(search) || 
x.FeeMin.ToString().Contains(search) || 
x.FeeMax.ToString().Contains(search) || 
x.StartDate.ToString().Contains(search) || 
x.EndDate.ToString().Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public CusQuotedPriceDtlQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.CusQPSerialNo.Contains(search) || 
x.CusQPId.ToString().Contains(search) || 
x.QPSerialNo.Contains(search) || 
x.QPId.ToString().Contains(search) || 
x.FeeCode.Contains(search) || 
x.FeeName.Contains(search) || 
x.StartPlace.Contains(search) || 
x.TransitPlace.Contains(search) || 
x.EndPlace.Contains(search) || 
x.AirLineCo.Contains(search) || 
x.AirLineNo.Contains(search) || 
x.WHBuyer.Contains(search) || 
x.DealWithArticle.Contains(search) || 
x.CustomsType.Contains(search) || 
x.MoorLevel.Contains(search) || 
x.BillingUnit.Contains(search) || 
x.Price.ToString().Contains(search) || 
x.CurrencyCode.Contains(search) || 
x.FeeConditionVal1.ToString().Contains(search) || 
x.CalcSign1.Contains(search) || 
x.FeeCondition.Contains(search) || 
x.CalcSign2.Contains(search) || 
x.FeeConditionVal2.ToString().Contains(search) || 
x.CalcFormula.Contains(search) || 
x.FeeMin.ToString().Contains(search) || 
x.FeeMax.ToString().Contains(search) || 
x.StartDate.ToString().Contains(search) || 
x.EndDate.ToString().Contains(search) || 
x.OperatingPoint.ToString().Contains(search) );
            return this;
        }

		public CusQuotedPriceDtlQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "CusQPSerialNo" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CusQPSerialNo.StartsWith(rule.value));
					}
					if (rule.field == "CusQPId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.CusQPId == val);
					}
					if (rule.field == "QPSerialNo" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.QPSerialNo.StartsWith(rule.value));
					}
					if (rule.field == "QPId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.QPId == val);
					}
					if (rule.field == "FeeCode" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.FeeCode.StartsWith(rule.value));
					}
					if (rule.field == "FeeName" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.FeeName.StartsWith(rule.value));
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
					if (rule.field == "AirLineCo" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.AirLineCo.StartsWith(rule.value));
					}
					if (rule.field == "AirLineNo" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.AirLineNo.StartsWith(rule.value));
					}
					if (rule.field == "WHBuyer" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.WHBuyer.StartsWith(rule.value));
					}
					if (rule.field == "ProxyOperator" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.ProxyOperator == boolval);
					}
					if (rule.field == "DealWithArticle" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.DealWithArticle.StartsWith(rule.value));
					}
					if (rule.field == "BSA" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.BSA == boolval);
					}
					if (rule.field == "CustomsType" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CustomsType.StartsWith(rule.value));
					}
					if (rule.field == "InspectMark" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.InspectMark == boolval);
					}
					if (rule.field == "GetOrdMark" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.GetOrdMark == boolval);
					}
					if (rule.field == "MoorLevel" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.MoorLevel.StartsWith(rule.value));
					}
					if (rule.field == "BillingUnit" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.BillingUnit.StartsWith(rule.value));
					}
					if (rule.field == "Price" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Price == val);
					}
					if (rule.field == "CurrencyCode" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CurrencyCode.StartsWith(rule.value));
					}
					if (rule.field == "FeeConditionVal1" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.FeeConditionVal1 == val);
					}
					if (rule.field == "CalcSign1" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CalcSign1.StartsWith(rule.value));
					}
					if (rule.field == "FeeCondition" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.FeeCondition.StartsWith(rule.value));
					}
					if (rule.field == "CalcSign2" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CalcSign2.StartsWith(rule.value));
					}
					if (rule.field == "FeeConditionVal2" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.FeeConditionVal2 == val);
					}
					if (rule.field == "CalcFormula" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.CalcFormula.StartsWith(rule.value));
					}
					if (rule.field == "FeeMin" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.FeeMin == val);
					}
					if (rule.field == "FeeMax" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.FeeMax == val);
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