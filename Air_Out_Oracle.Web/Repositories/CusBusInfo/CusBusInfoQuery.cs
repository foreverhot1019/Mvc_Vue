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
    public class CusBusInfoQuery : QueryObject<CusBusInfo>
    {
        public CusBusInfoQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                    x.Id.ToString().Contains(search) ||
                    x.EnterpriseId.Contains(search) ||
                    x.EnterpriseShortName.Contains(search) ||
                    x.EnterpriseGroupCode.Contains(search) ||
                    x.TopEnterpriseCode.Contains(search) ||
                    x.CIQID.Contains(search) ||
                    x.CHECKID.Contains(search) ||
                    x.CustomsCode.Contains(search) ||
                    x.CHNName.Contains(search) ||
                    x.EngName.Contains(search) ||
                    x.AddressCHN.Contains(search) ||
                    x.AddressEng.Contains(search) ||
                    x.WebSite.Contains(search) ||
                    x.TradeTypeCode.Contains(search) ||
                    x.AreaCode.Contains(search) ||
                    x.CountryCode.Contains(search) ||
                    x.CopoKindCode.Contains(search) ||
                    x.CorpartiPerson.Contains(search) ||
                    x.ResteredCapital.Contains(search) ||
                    x.Description.Contains(search) ||
                    x.OperatingPoint.ToString().Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public CusBusInfoQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                    x.Id.ToString().Contains(search) ||
                    x.EnterpriseId.Contains(search) ||
                    x.EnterpriseShortName.Contains(search) ||
                    x.EnterpriseGroupCode.Contains(search) ||
                    x.TopEnterpriseCode.Contains(search) ||
                    x.CIQID.Contains(search) ||
                    x.CHECKID.Contains(search) ||
                    x.CustomsCode.Contains(search) ||
                    x.CHNName.Contains(search) ||
                    x.EngName.Contains(search) ||
                    x.AddressCHN.Contains(search) ||
                    x.AddressEng.Contains(search) ||
                    x.WebSite.Contains(search) ||
                    x.TradeTypeCode.Contains(search) ||
                    x.AreaCode.Contains(search) ||
                    x.CountryCode.Contains(search) ||
                    x.CopoKindCode.Contains(search) ||
                    x.CorpartiPerson.Contains(search) ||
                    x.ResteredCapital.Contains(search) ||
                    x.Description.Contains(search) ||
                    x.OperatingPoint.ToString().Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public CusBusInfoQuery Withfilter(IEnumerable<filterRule> filters)
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
                    if (rule.field == "EnterpriseId" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EnterpriseId.StartsWith(rule.value));
                    }
                    if (rule.field == "EnterpriseShortName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EnterpriseShortName.StartsWith(rule.value));
                    }
                    if (rule.field == "EnterpriseGroupCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EnterpriseGroupCode.StartsWith(rule.value));
                    }
                    if (rule.field == "TopEnterpriseCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TopEnterpriseCode.StartsWith(rule.value));
                    }
                    if (rule.field == "CIQID" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CIQID.StartsWith(rule.value));
                    }
                    if (rule.field == "CHECKID" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CHECKID.StartsWith(rule.value));
                    }
                    if (rule.field == "CustomsCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CustomsCode.StartsWith(rule.value));
                    }
                    if (rule.field == "CHNName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CHNName.StartsWith(rule.value));
                    }
                    if (rule.field == "EngName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EngName.StartsWith(rule.value));
                    }
                    if (rule.field == "AddressCHN" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.AddressCHN.StartsWith(rule.value));
                    }
                    if (rule.field == "AddressEng" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.AddressEng.StartsWith(rule.value));
                    }
                    if (rule.field == "WebSite" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.WebSite.StartsWith(rule.value));
                    }
                    if (rule.field == "TradeTypeCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TradeTypeCode.StartsWith(rule.value));
                    }
                    if (rule.field == "AreaCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.AreaCode.StartsWith(rule.value));
                    }
                    if (rule.field == "CountryCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CountryCode.StartsWith(rule.value));
                    }
                    if (rule.field == "CopoKindCode" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CopoKindCode.StartsWith(rule.value));
                    }
                    if (rule.field == "CorpartiPerson" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CorpartiPerson.StartsWith(rule.value));
                    }
                    if (rule.field == "ResteredCapital" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ResteredCapital.StartsWith(rule.value));
                    }
                    if (rule.field == "IsInternalCompany" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.IsInternalCompany == boolval);
                    }
                    if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Description.StartsWith(rule.value));
                    }
                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        //int enumVal = int.Parse(rule.value);
                        AirOutEnumType.UseStatusEnum enumType;// = (AirOutEnumType.UseStatusEnum)enumVal;
                        if (Enum.TryParse<AirOutEnumType.UseStatusEnum>(rule.value, out enumType))
                        {
                            And(x => x.Status == enumType);
                        }
                        //else
                        //{
                        //    And(x => x.Status == enumVal);
                        //}
                    }
                    if (rule.field == "_Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        //int enumVal = int.Parse(rule.value);
                        AirOutEnumType.UseStatusEnum enumType;// = (AirOutEnumType.UseStatusEnum)enumVal;
                        if (Enum.TryParse<AirOutEnumType.UseStatusEnum>(rule.value, out enumType))
                        {
                            And(x => x.Status >= enumType);
                        }
                        //else
                        //{
                        //    And(x => x.Status == enumVal);
                        //}
                    }
                    if (rule.field == "Status_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        //int enumVal = int.Parse(rule.value);
                        AirOutEnumType.UseStatusEnum enumType;// = (AirOutEnumType.UseStatusEnum)enumVal;
                        if (Enum.TryParse<AirOutEnumType.UseStatusEnum>(rule.value, out enumType))
                        {
                            And(x => x.Status <= enumType);
                        }
                        //else
                        //{
                        //    And(x => x.Status == enumVal);
                        //}
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