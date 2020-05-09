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
    public class ChangeOrderHistoryQuery : QueryObject<ChangeOrderHistory>
    {
        public ChangeOrderHistoryQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.Id.ToString().Contains(search) ||
                    x.Key_Id.ToString().Contains(search) ||
                    x.TableName.Contains(search) || 
                    x.InsertNum.ToString().Contains(search) || 
                    x.UpdateNum.ToString().Contains(search) || 
                    x.DeleteNum.ToString().Contains(search) || 
                    x.Content.Contains(search) || 
                    x.ADDID.Contains(search) || 
                    x.ADDWHO.Contains(search) || 
                    x.ADDTS.ToString().Contains(search) || 
                    x.EDITWHO.Contains(search) || 
                    x.EDITTS.ToString().Contains(search) || 
                    x.EDITID.Contains(search) || 
                    x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public ChangeOrderHistoryQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.Id.ToString().Contains(search) ||
                    x.Key_Id.ToString().Contains(search) ||
                    x.TableName.Contains(search) ||
                    x.InsertNum.ToString().Contains(search) ||
                    x.UpdateNum.ToString().Contains(search) ||
                    x.DeleteNum.ToString().Contains(search) ||
                    x.Content.Contains(search) ||
                    x.ADDID.Contains(search) ||
                    x.ADDWHO.Contains(search) ||
                    x.ADDTS.ToString().Contains(search) ||
                    x.EDITWHO.Contains(search) ||
                    x.EDITTS.ToString().Contains(search) ||
                    x.EDITID.Contains(search) ||
                    x.OperatingPoint.ToString().Contains(search));
            return this;
        }

        public ChangeOrderHistoryQuery Withfilter(IEnumerable<filterRule> filters)
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

                    if (rule.field == "Key_Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.Key_Id == val);
                    }

                    if (rule.field == "TableName" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.TableName.StartsWith(rule.value));
                    }

                    if (rule.field == "InsertNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.InsertNum == val);
                    }

                    if (rule.field == "UpdateNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.UpdateNum == val);
                    }

                    if (rule.field == "DeleteNum" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.DeleteNum == val);
                    }

                    if (rule.field == "Content" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Content.Contains(rule.value));
                    }

                    if (rule.field == "ADDID" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ADDID == rule.value);
                    }

                    if (rule.field == "ADDWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.ADDWHO.Contains(rule.value));
                    }

                    if (rule.field == "ADDTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => date <= x.ADDTS);
                    }

                    if (rule.field == "EDITWHO" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EDITWHO.Contains(rule.value));
                    }

                    if (rule.field == "EDITTS" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => date <= x.EDITTS);
                    }

                    if (rule.field == "EDITID" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.EDITID == rule.value);
                    }

                    if (rule.field == "OperatingPoint" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.OperatingPoint == val);
                    }

                }
            }
            return this;
        }
    }
}



