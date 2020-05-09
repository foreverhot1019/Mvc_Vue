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
   public class DailyRateQuery:QueryObject<DailyRate>
    {
        public DailyRateQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.LocalCurrency.Contains(search) || 
x.LocalCurrCode.Contains(search) || 
x.ForeignCurrency.Contains(search) || 
x.ForeignCurrCode.Contains(search) || 
x.PriceType.Contains(search) || 
x.BankName.Contains(search) || 
x.Price.ToString().Contains(search) || 
x.ScrapyDate.ToString().Contains(search) || 
x.Description.Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public DailyRateQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.LocalCurrency.Contains(search) || 
x.LocalCurrCode.Contains(search) || 
x.ForeignCurrency.Contains(search) || 
x.ForeignCurrCode.Contains(search) || 
x.PriceType.Contains(search) || 
x.BankName.Contains(search) || 
x.Price.ToString().Contains(search) || 
x.ScrapyDate.ToString().Contains(search) || 
x.Description.Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public DailyRateQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "LocalCurrency" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.LocalCurrency.StartsWith(rule.value));
					}
					if (rule.field == "LocalCurrCode" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.LocalCurrCode.StartsWith(rule.value));
					}
					if (rule.field == "ForeignCurrency" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.ForeignCurrency.StartsWith(rule.value));
					}
					if (rule.field == "ForeignCurrCode" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.ForeignCurrCode.StartsWith(rule.value));
					}
					if (rule.field == "PriceType" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.PriceType.StartsWith(rule.value));
					}
					if (rule.field == "BankName" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.BankName.StartsWith(rule.value));
					}
					if (rule.field == "Price" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
					{
						 var val = Convert.ToDecimal(rule.value);
						 And(x => x.Price == val);
					}
					if (rule.field == "ScrapyDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ScrapyDate == date);
					}
					if (rule.field == "_ScrapyDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ScrapyDate >= date);
					}
					if (rule.field == "ScrapyDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
					{	
						 var date = Convert.ToDateTime(rule.value);
						 And(x => x.ScrapyDate <= date);
					}
					if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Description.StartsWith(rule.value));
					}
					if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
					{	
						 var boolval=Convert.ToBoolean(rule.value);
						 And(x => x.Status == boolval);
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