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
    public class ContactsQuery : QueryObject<Contacts>
    {
        public ContactsQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.CusBusInfoId.Contains(search) ||
                x.CompanyName.Contains(search) ||
                x.CompanyCode.Contains(search) ||
                x.CoAddress.Contains(search) ||
                x.CoArea.Contains(search) ||
                x.CoCountry.Contains(search) ||
                x.Contact.Contains(search) ||
                x.ContactInfo.Contains(search) ||
                x.Remark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public ContactsQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.CusBusInfoId.Contains(search) ||
                x.CompanyName.Contains(search) ||
                x.CompanyCode.Contains(search) ||
                x.CoAddress.Contains(search) ||
                x.CoArea.Contains(search) ||
                x.CoCountry.Contains(search) ||
                x.Contact.Contains(search) ||
                x.ContactInfo.Contains(search) ||
                x.Remark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public ContactsQuery Withfilter(IEnumerable<filterRule> filters)
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
                    if (rule.field == "CusBusInfoId" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CusBusInfoId.StartsWith(rule.value));
                    }
                    if (rule.field == "ContactType" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ContactType ==rule.value);
                    }
                    if (rule.field == "CompanyName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CompanyName.StartsWith(rule.value));
                    }
                    if (rule.field == "CompanyCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CompanyCode.StartsWith(rule.value));
                    }
                    if (rule.field == "CoAddress" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CoAddress.StartsWith(rule.value));
                    }
                    if (rule.field == "CoArea" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CoArea.StartsWith(rule.value));
                    }
                    if (rule.field == "CoCountry" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CoCountry.StartsWith(rule.value));
                    }
                    if (rule.field == "Contact" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Contact.StartsWith(rule.value));
                    }
                    if (rule.field == "ContactInfo" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ContactInfo.Contains(rule.value));
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