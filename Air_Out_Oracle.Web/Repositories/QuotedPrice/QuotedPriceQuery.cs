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
    public class QuotedPriceQuery : QueryObject<QuotedPrice>
    {
        public QuotedPriceQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                    x.SerialNo.Contains(search) ||
                    x.SettleAccount.Contains(search) ||
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
                    x.CalcSign1.Contains(search) ||
                    x.FeeCondition.Contains(search) ||
                    x.CalcSign2.Contains(search) ||
                    x.CalcFormula.Contains(search) ||
                    x.StartDate.ToString().Contains(search) ||
                    x.EndDate.ToString().Contains(search) ||
                    x.Description.Contains(search) ||
                    x.OperatingPoint.ToString().Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public QuotedPriceQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                    x.SerialNo.Contains(search) ||
                    x.SettleAccount.Contains(search) ||
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
                    x.CalcSign1.Contains(search) ||
                    x.FeeCondition.Contains(search) ||
                    x.CalcSign2.Contains(search) ||
                    x.CalcFormula.Contains(search) ||
                    x.StartDate.ToString().Contains(search) ||
                    x.EndDate.ToString().Contains(search) ||
                    x.Description.Contains(search) ||
                    x.OperatingPoint.ToString().Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search));
            return this;
        }

        public QuotedPriceQuery Withfilter(IEnumerable<filterRule> filters)
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
                    if (rule.field == "SerialNo" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.SerialNo.StartsWith(rule.value));
                    }
                    if (rule.field == "SettleAccount" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.SettleAccount.StartsWith(rule.value));
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
                    if (rule.field == "ProxyOperator" && !string.IsNullOrEmpty(rule.value))
                    {
                        bool TF = Common.ChangStrToBool(rule.value);
                        And(x => x.ProxyOperator == TF);
                    }
                    if (rule.field == "DealWithArticle" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.DealWithArticle.StartsWith(rule.value));
                    }
                    if (rule.field == "BSA" && !string.IsNullOrEmpty(rule.value))
                    {
                        bool TF = Common.ChangStrToBool(rule.value);
                        And(x => x.BSA == TF);
                    }
                    if (rule.field == "CustomsType" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CustomsType.StartsWith(rule.value));
                    }
                    if (rule.field == "InspectMark" && !string.IsNullOrEmpty(rule.value))
                    {
                        bool TF = Common.ChangStrToBool(rule.value);
                        And(x => x.InspectMark == TF);
                    }
                    if (rule.field == "GetOrdMark" && !string.IsNullOrEmpty(rule.value))
                    {
                        bool TF = Common.ChangStrToBool(rule.value);
                        And(x => x.GetOrdMark == TF);
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
                    if (rule.field == "FeeConditionVal1" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                            And(x => x.FeeConditionVal1 == decml);
                    }
                    if (rule.field == "_FeeConditionVal1" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                            And(x => x.FeeConditionVal1 >= decml);
                    }
                    if (rule.field == "FeeConditionVal1_" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                            And(x => x.FeeConditionVal1 <= decml);
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
                    if (rule.field == "FeeConditionVal2" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                            And(x => x.FeeConditionVal2 == decml);
                    }
                    if (rule.field == "_FeeConditionVal2" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                            And(x => x.FeeConditionVal2 >= decml);
                    }
                    if (rule.field == "FeeConditionVal2_" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                            And(x => x.FeeConditionVal2 <= decml);
                    }
                    if (rule.field == "CalcFormula" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.CalcFormula.StartsWith(rule.value));
                    }
                    if (rule.field == "FeeMin" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                            And(x => x.FeeMin == decml);
                    }
                    if (rule.field == "_FeeMin" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                        {
                            And(x => x.FeeMin >= decml);
                        }
                    }
                    if (rule.field == "FeeMin_" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                        {
                            And(x => x.FeeMin <= decml);
                        }
                    }
                    if (rule.field == "FeeMax" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                        {
                            And(x => x.FeeMax == decml);
                        }
                    }
                    if (rule.field == "_FeeMax" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                        {
                            And(x => x.FeeMax >= decml);
                        }
                    }
                    if (rule.field == "FeeMax_" && !string.IsNullOrEmpty(rule.value))
                    {
                        decimal decml = 0;
                        if (decimal.TryParse(rule.value, out decml))
                        {
                            And(x => x.FeeMax <= decml);
                        }
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
                    if (rule.field == "Description" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Description.StartsWith(rule.value));
                    }
                    if (rule.field == "AuditStatus" && !string.IsNullOrEmpty(rule.value))
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.AuditStatusEnum>(rule.value);
                        And(x => x.AuditStatus == EnumVal);
                    }
                    if (rule.field == "Status" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        var EnumVal = Common.GetEnumVal<AirOutEnumType.UseStatusIsOrNoEnum>(rule.value);
                        And(x => x.Status == EnumVal);
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
                        var date = Convert.ToDateTime(rule.value).AddDays(1);
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