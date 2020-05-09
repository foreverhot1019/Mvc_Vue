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
    public class CustomerQuery : QueryObject<Customer>
    {
        public CustomerQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.NameChs.Contains(search) ||
                x.NamePinYin.Contains(search) ||
                x.NameEng.Contains(search) ||
                x.Birthday.ToString().Contains(search) ||
                x.AirLineMember.Contains(search) ||
                x.Contact.Contains(search) ||
                x.Saller.Contains(search) ||
                x.OP.Contains(search) ||
                x.ComponyName.Contains(search) ||
                x.ComponyId.ToString().Contains(search) ||
                x.IdCard.Contains(search) ||
                x.IdCardLimit_S.ToString().Contains(search) ||
                x.IdCardLimit_E.ToString().Contains(search) ||
                x.IdCardPhoto_A.Contains(search) ||
                x.IdCardPhoto_B.Contains(search) ||
                x.Passpord.Contains(search) ||
                x.PasspordLimit_S.ToString().Contains(search) ||
                x.PasspordLimit_E.ToString().Contains(search) ||
                x.PasspordPhoto_A.Contains(search) ||
                x.PasspordPhoto_B.Contains(search) ||
                x.HK_MacauPass.Contains(search) ||
                x.HK_MacauPassLimit_S.ToString().Contains(search) ||
                x.HK_MacauPassLimit_E.ToString().Contains(search) ||
                x.HK_MacauPassPhoto_A.Contains(search) ||
                x.HK_MacauPassPhoto_B.Contains(search) ||
                x.TaiwanPass.Contains(search) ||
                x.TaiwanPassLimit_S.ToString().Contains(search) ||
                x.TaiwanPassLimit_E.ToString().Contains(search) ||
                x.TaiwanPassPhoto_A.Contains(search) ||
                x.TaiwanPassPhoto_B.Contains(search) ||
                x.TWIdCard.Contains(search) ||
                x.TWIdCardLimit_S.ToString().Contains(search) ||
                x.TWIdCardLimit_E.ToString().Contains(search) ||
                x.TWIdCardPhoto_A.Contains(search) ||
                x.TWIdCardPhoto_B.Contains(search) ||
                x.HometownPass.Contains(search) ||
                x.HometownPassLimit_S.ToString().Contains(search) ||
                x.HometownPassLimit_E.ToString().Contains(search) ||
                x.HometownPassPhoto_A.Contains(search) ||
                x.HometownPassPhoto_B.Contains(search) ||
                x.Remark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public CustomerQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x =>
                x.Id.ToString().Contains(search) ||
                x.NameChs.Contains(search) ||
                x.NamePinYin.Contains(search) ||
                x.NameEng.Contains(search) ||
                x.Birthday.ToString().Contains(search) ||
                x.AirLineMember.Contains(search) ||
                x.Contact.Contains(search) ||
                x.Saller.Contains(search) ||
                x.OP.Contains(search) ||
                x.ComponyName.Contains(search) ||
                x.ComponyId.ToString().Contains(search) ||
                x.IdCard.Contains(search) ||
                x.IdCardLimit_S.ToString().Contains(search) ||
                x.IdCardLimit_E.ToString().Contains(search) ||
                x.IdCardPhoto_A.Contains(search) ||
                x.IdCardPhoto_B.Contains(search) ||
                x.Passpord.Contains(search) ||
                x.PasspordLimit_S.ToString().Contains(search) ||
                x.PasspordLimit_E.ToString().Contains(search) ||
                x.PasspordPhoto_A.Contains(search) ||
                x.PasspordPhoto_B.Contains(search) ||
                x.HK_MacauPass.Contains(search) ||
                x.HK_MacauPassLimit_S.ToString().Contains(search) ||
                x.HK_MacauPassLimit_E.ToString().Contains(search) ||
                x.HK_MacauPassPhoto_A.Contains(search) ||
                x.HK_MacauPassPhoto_B.Contains(search) ||
                x.TaiwanPass.Contains(search) ||
                x.TaiwanPassLimit_S.ToString().Contains(search) ||
                x.TaiwanPassLimit_E.ToString().Contains(search) ||
                x.TaiwanPassPhoto_A.Contains(search) ||
                x.TaiwanPassPhoto_B.Contains(search) ||
                x.TWIdCard.Contains(search) ||
                x.TWIdCardLimit_S.ToString().Contains(search) ||
                x.TWIdCardLimit_E.ToString().Contains(search) ||
                x.TWIdCardPhoto_A.Contains(search) ||
                x.TWIdCardPhoto_B.Contains(search) ||
                x.HometownPass.Contains(search) ||
                x.HometownPassLimit_S.ToString().Contains(search) ||
                x.HometownPassLimit_E.ToString().Contains(search) ||
                x.HometownPassPhoto_A.Contains(search) ||
                x.HometownPassPhoto_B.Contains(search) ||
                x.Remark.Contains(search) ||
                x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public CustomerQuery Withfilter(IEnumerable<filterRule> filters)
        {
            if (filters != null)
            {
                foreach (var rule in filters)
                {
                    //if (rule.field == "OCompany" && !string.IsNullOrEmpty(rule.value))
                    //{	
                    //    And(x => x.OCompany == rule.value);
                    //}
                    if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Id == val);
                    }
                    if (rule.field == "NameChs" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.NameChs.StartsWith(rule.value));
                    }
                    if (rule.field == "NamePinYin" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.NamePinYin.StartsWith(rule.value));
                    }
                    if (rule.field == "NameEng" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.NameEng.StartsWith(rule.value));
                    }
                    if (rule.field == "Sex" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.Sex == boolval);
                    }
                    if (rule.field == "Birthday" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.Birthday == date);
                    }
                    if (rule.field == "_Birthday" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.Birthday >= date);
                    }
                    if (rule.field == "Birthday_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.Birthday <= date);
                    }
                    if (rule.field == "AirLineMember" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.AirLineMember.StartsWith(rule.value));
                    }
                    if (rule.field == "Contact" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Contact.StartsWith(rule.value));
                    }
                    if (rule.field == "Saller" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Saller.StartsWith(rule.value));
                    }
                    if (rule.field == "OP" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.OP.StartsWith(rule.value));
                    }
                    if (rule.field == "ComponyName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ComponyName.StartsWith(rule.value));
                    }
                    if (rule.field == "ComponyId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.ComponyId == val);
                    }
                    if (rule.field == "IdCard" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.IdCard.StartsWith(rule.value));
                    }
                    if (rule.field == "IdCardLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.IdCardLimit_S == date);
                    }
                    if (rule.field == "_IdCardLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.IdCardLimit_S >= date);
                    }
                    if (rule.field == "IdCardLimit_S_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.IdCardLimit_S <= date);
                    }
                    if (rule.field == "IdCardLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.IdCardLimit_E == date);
                    }
                    if (rule.field == "_IdCardLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.IdCardLimit_E >= date);
                    }
                    if (rule.field == "IdCardLimit_E_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.IdCardLimit_E <= date);
                    }
                    if (rule.field == "IdCardPhoto_A" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.IdCardPhoto_A.StartsWith(rule.value));
                    }
                    if (rule.field == "IdCardPhoto_B" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.IdCardPhoto_B.StartsWith(rule.value));
                    }
                    if (rule.field == "Passpord" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Passpord.StartsWith(rule.value));
                    }
                    if (rule.field == "PasspordLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.PasspordLimit_S == date);
                    }
                    if (rule.field == "_PasspordLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.PasspordLimit_S >= date);
                    }
                    if (rule.field == "PasspordLimit_S_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.PasspordLimit_S <= date);
                    }
                    if (rule.field == "PasspordLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.PasspordLimit_E == date);
                    }
                    if (rule.field == "_PasspordLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.PasspordLimit_E >= date);
                    }
                    if (rule.field == "PasspordLimit_E_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.PasspordLimit_E <= date);
                    }
                    if (rule.field == "PasspordPhoto_A" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.PasspordPhoto_A.StartsWith(rule.value));
                    }
                    if (rule.field == "PasspordPhoto_B" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.PasspordPhoto_B.StartsWith(rule.value));
                    }
                    if (rule.field == "HK_MacauPass" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.HK_MacauPass.StartsWith(rule.value));
                    }
                    if (rule.field == "HK_MacauPassLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HK_MacauPassLimit_S == date);
                    }
                    if (rule.field == "_HK_MacauPassLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HK_MacauPassLimit_S >= date);
                    }
                    if (rule.field == "HK_MacauPassLimit_S_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HK_MacauPassLimit_S <= date);
                    }
                    if (rule.field == "HK_MacauPassLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HK_MacauPassLimit_E == date);
                    }
                    if (rule.field == "_HK_MacauPassLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HK_MacauPassLimit_E >= date);
                    }
                    if (rule.field == "HK_MacauPassLimit_E_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HK_MacauPassLimit_E <= date);
                    }
                    if (rule.field == "HK_MacauPassPhoto_A" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.HK_MacauPassPhoto_A.StartsWith(rule.value));
                    }
                    if (rule.field == "HK_MacauPassPhoto_B" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.HK_MacauPassPhoto_B.StartsWith(rule.value));
                    }
                    if (rule.field == "TaiwanPass" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TaiwanPass.StartsWith(rule.value));
                    }
                    if (rule.field == "TaiwanPassLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TaiwanPassLimit_S == date);
                    }
                    if (rule.field == "_TaiwanPassLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TaiwanPassLimit_S >= date);
                    }
                    if (rule.field == "TaiwanPassLimit_S_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TaiwanPassLimit_S <= date);
                    }
                    if (rule.field == "TaiwanPassLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TaiwanPassLimit_E == date);
                    }
                    if (rule.field == "_TaiwanPassLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TaiwanPassLimit_E >= date);
                    }
                    if (rule.field == "TaiwanPassLimit_E_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TaiwanPassLimit_E <= date);
                    }
                    if (rule.field == "TaiwanPassPhoto_A" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TaiwanPassPhoto_A.StartsWith(rule.value));
                    }
                    if (rule.field == "TaiwanPassPhoto_B" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TaiwanPassPhoto_B.StartsWith(rule.value));
                    }
                    if (rule.field == "TWIdCard" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TWIdCard.StartsWith(rule.value));
                    }
                    if (rule.field == "TWIdCardLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TWIdCardLimit_S == date);
                    }
                    if (rule.field == "_TWIdCardLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TWIdCardLimit_S >= date);
                    }
                    if (rule.field == "TWIdCardLimit_S_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TWIdCardLimit_S <= date);
                    }
                    if (rule.field == "TWIdCardLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TWIdCardLimit_E == date);
                    }
                    if (rule.field == "_TWIdCardLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TWIdCardLimit_E >= date);
                    }
                    if (rule.field == "TWIdCardLimit_E_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.TWIdCardLimit_E <= date);
                    }
                    if (rule.field == "TWIdCardPhoto_A" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TWIdCardPhoto_A.StartsWith(rule.value));
                    }
                    if (rule.field == "TWIdCardPhoto_B" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TWIdCardPhoto_B.StartsWith(rule.value));
                    }
                    if (rule.field == "HometownPass" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.HometownPass.StartsWith(rule.value));
                    }
                    if (rule.field == "HometownPassLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HometownPassLimit_S == date);
                    }
                    if (rule.field == "_HometownPassLimit_S" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HometownPassLimit_S >= date);
                    }
                    if (rule.field == "HometownPassLimit_S_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HometownPassLimit_S <= date);
                    }
                    if (rule.field == "HometownPassLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HometownPassLimit_E == date);
                    }
                    if (rule.field == "_HometownPassLimit_E" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HometownPassLimit_E >= date);
                    }
                    if (rule.field == "HometownPassLimit_E_" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => x.HometownPassLimit_E <= date);
                    }
                    if (rule.field == "HometownPassPhoto_A" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.HometownPassPhoto_A.StartsWith(rule.value));
                    }
                    if (rule.field == "HometownPassPhoto_B" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.HometownPassPhoto_B.StartsWith(rule.value));
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