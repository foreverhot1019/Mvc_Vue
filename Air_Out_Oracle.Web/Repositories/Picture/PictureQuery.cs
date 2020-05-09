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
   public class PictureQuery:QueryObject<Picture>
    {
        public PictureQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.Code.Contains(search) || 
x.Address.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) || 
x.Remark.Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PictureQuery WithPopupSearch(string search,string para="")
        {
            if (!string.IsNullOrEmpty(search))
                And( x =>  
x.Id.ToString().Contains(search) || 
x.Code.Contains(search) || 
x.Address.Contains(search) || 
x.OperatingPoint.ToString().Contains(search) || 
x.Remark.Contains(search) || 
x.ADDWHO.Contains(search) || 
x.ADDTS.ToString().Contains(search) || 
x.EDITWHO.Contains(search) || 
x.EDITTS.ToString().Contains(search) );
            return this;
        }

		public PictureQuery Withfilter(IEnumerable<filterRule> filters)
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
					if (rule.field == "Code" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Code.StartsWith(rule.value));
					}
					if (rule.field == "Adress" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Address.StartsWith(rule.value));
					}
					if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
					{
						 int val = Convert.ToInt32(rule.value);
						 And(x => x.OperatingPoint == val);
                    }
                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.UseStatusIsOrNoEnum>(rule.value);
                        And(x => x.Status == EnumVal);
                    }
                    if (rule.field == "Type" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.PictureTypeEnum>(rule.value);
                        And(x => x.Type == EnumVal);
                    }
					if (rule.field == "Remark" && !string.IsNullOrEmpty(rule.value))
					{
						 And(x => x.Remark.StartsWith(rule.value));
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