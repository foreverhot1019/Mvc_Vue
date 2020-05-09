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
    public class SallerQuery : QueryObject<Saller>
    {
        /// <summary>
        /// 任何搜索
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public SallerQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.Name.Contains(search) ||
                x.PhoneNumber.Contains(search) ||
                x.Company.Contains(search) ||
                x.Address.Contains(search) ||
                x.Description.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search) ||
                x.ADDWHO.Contains(search) ||
                x.ADDTS.ToString().Contains(search) ||
                x.EDITWHO.Contains(search) ||
                x.EDITTS.ToString().Contains(search));
            return this;
        }

        /// <summary>
        /// 弹出框搜索
        /// </summary>
        /// <param name="search"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public SallerQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.Name.Contains(search) ||
                x.PhoneNumber.Contains(search) ||
                x.Company.Contains(search) ||
                x.Address.Contains(search) ||
                x.Description.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search) ||
                x.ADDWHO.Contains(search) ||
                x.ADDTS.ToString().Contains(search) ||
                x.EDITWHO.Contains(search) ||
                x.EDITTS.ToString().Contains(search));
            return this;
        }

        /// <summary>
        /// 搜索拼接
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public SallerQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    //if (rule.field == "ArrCusBusInfo" && !string.IsNullOrEmpty(rule.value))
                    //{	
                    //    And(x => x.ArrCusBusInfo == rule.value);
                    //}
                    if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Id == val);
                    }
                    if (rule.field == "Code" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Code.StartsWith(rule.value));
                    }
                    if (rule.field == "Name" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Name.StartsWith(rule.value));
                    }
                    if (rule.field == "PhoneNumber" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.PhoneNumber.StartsWith(rule.value));
                    }
                    if (rule.field == "Company" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Company.StartsWith(rule.value));
                    }
                    if (rule.field == "Address" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Address.StartsWith(rule.value));
                    }
                    if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Description.StartsWith(rule.value));
                    }
                    if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.OperatingPoint == val);
                    }
                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.UseStatusEnum>(rule.value);
                        //var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Status == EnumVal);
                    }
                    //if (rule.field == "ADDID" && !string.IsNullOrEmpty(rule.value))
                    //{
                    //	 And(x => x.ADDID.StartsWith(rule.value));
                    //}
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