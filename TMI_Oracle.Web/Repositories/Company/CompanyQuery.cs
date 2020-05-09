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
    public class CompanyQuery : QueryObject<Company>
    {
        public CompanyQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.CIQID.Contains(search) ||
                x.Name.Contains(search) ||
                x.SimpleName.Contains(search) ||
                x.Eng_Name.Contains(search) ||
                x.Address.Contains(search) ||
                x.City.Contains(search) ||
                x.Province.Contains(search) ||
                x.RegisterDate.ToString().Contains(search) ||
                x.Logo.Contains(search) ||
                x.ContractStart.ToString().Contains(search) ||
                x.ContractEnd.ToString().Contains(search) ||
                x.AuthorizeAmount.ToString().Contains(search) ||
                x.Currency.Contains(search) ||
                x.CheckBillDate.Contains(search) ||
                x.PayPalDate.Contains(search) ||
                x.InvoiceType.Contains(search) ||
                x.Remark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public CompanyQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.CIQID.Contains(search) ||
                x.Name.Contains(search) ||
                x.SimpleName.Contains(search) ||
                x.Eng_Name.Contains(search) ||
                x.Address.Contains(search) ||
                x.City.Contains(search) ||
                x.Province.Contains(search) ||
                x.RegisterDate.ToString().Contains(search) ||
                x.Logo.Contains(search) ||
                x.ContractStart.ToString().Contains(search) ||
                x.ContractEnd.ToString().Contains(search) ||
                x.AuthorizeAmount.ToString().Contains(search) ||
                x.Currency.Contains(search) ||
                x.CheckBillDate.Contains(search) ||
                x.PayPalDate.Contains(search) ||
                x.InvoiceType.Contains(search) ||
                x.Remark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public CompanyQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    if (rule.field == "q" && !string.IsNullOrEmpty(rule.value))
                    {
                        var val = rule.value;
                        int i_val = 0;
                        int.TryParse(val, out i_val);
                        if (i_val > 0)
                            And(x => x.Id == i_val || x.CIQID.StartsWith(val) || x.Name.StartsWith(val) || x.Eng_Name.StartsWith(val));
                        else
                            And(x => x.CIQID.StartsWith(val) || x.Name.StartsWith(val) || x.Eng_Name.StartsWith(val));
                    }
                    if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Id == val);
                    }
                    if (rule.field == "CIQID" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CIQID.StartsWith(rule.value));
                    }
                    if (rule.field == "Name" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Name.StartsWith(rule.value));
                    }
                    if (rule.field == "SimpleName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.SimpleName.StartsWith(rule.value));
                    }
                    if (rule.field == "Eng_Name" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Eng_Name.StartsWith(rule.value));
                    }
                    if (rule.field == "Address" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Address.StartsWith(rule.value));
                    }
                    if (rule.field == "City" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.City.StartsWith(rule.value));
                    }
                    if (rule.field == "Province" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Province.StartsWith(rule.value));
                    }
                    if (rule.field == "RegisterDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.RegisterDate == date);
                    }
                    if (rule.field == "_RegisterDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.RegisterDate >= date);
                    }
                    if (rule.field == "RegisterDate_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.RegisterDate <= date);
                    }
                    if (rule.field == "Logo" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Logo.StartsWith(rule.value));
                    }
                    if (rule.field == "ContractStart" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ContractStart == date);
                    }
                    if (rule.field == "_ContractStart" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ContractStart >= date);
                    }
                    if (rule.field == "ContractStart_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ContractStart <= date);
                    }
                    if (rule.field == "ContractEnd" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ContractEnd == date);
                    }
                    if (rule.field == "_ContractEnd" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ContractEnd >= date);
                    }
                    if (rule.field == "ContractEnd_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.ContractEnd <= date);
                    }
                    if (rule.field == "AuthorizeAmount" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDecimal())
                    {
                        var val = Convert.ToDecimal(rule.value);
                        And(x => x.AuthorizeAmount == val);
                    }
                    if (rule.field == "Currency" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Currency.StartsWith(rule.value));
                    }
                    if (rule.field == "CheckBillDate" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CheckBillDate.StartsWith(rule.value));
                    }
                    if (rule.field == "PayPalDate" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.PayPalDate.StartsWith(rule.value));
                    }
                    if (rule.field == "InvoiceType" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.InvoiceType.StartsWith(rule.value));
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