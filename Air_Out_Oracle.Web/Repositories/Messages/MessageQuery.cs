



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
    public class MessageQuery : QueryObject<Message>
    {
        public MessageQuery WithAnySearch(string search)
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.Id.ToString().Contains(search) || x.Subject.Contains(search) || x.Content.Contains(search) || x.Type.Contains(search) || x.NewDate.ToString().Contains(search) || x.SendDate.ToString().Contains(search) || x.NotificationId.ToString().Contains(search));
            return this;
        }

        public MessageQuery WithPopupSearch(string search, string para = "")
        {
            if (!string.IsNullOrEmpty(search))
                And(x => x.Id.ToString().Contains(search) || x.Subject.Contains(search) || x.Content.Contains(search) || x.Type.Contains(search) || x.NewDate.ToString().Contains(search) || x.SendDate.ToString().Contains(search) || x.NotificationId.ToString().Contains(search));
            return this;
        }

        public MessageQuery Withfilter(IEnumerable<filterRule> filters)
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




                    if (rule.field == "Subject" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Subject.Contains(rule.value));
                    }
                    if (rule.field == "Key1" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Key1.Contains(rule.value));
                    }
                    if (rule.field == "Key2" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Key2.Contains(rule.value));
                    }



                    if (rule.field == "Content" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Content.Contains(rule.value));
                    }





                    if (rule.field == "Type" && !string.IsNullOrEmpty(rule.value))
                    {
                        And(x => x.Type.Contains(rule.value));
                    }








                    if (rule.field == "NewDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => SqlFunctions.DateDiff("d", date, x.NewDate) >= 0);
                    }






                    if (rule.field == "IsSended" && !string.IsNullOrEmpty(rule.value) && rule.value.IsBool())
                    {
                        var boolval = Convert.ToBoolean(rule.value);
                        And(x => x.IsSended == boolval);
                    }




                    if (rule.field == "SendDate" && !string.IsNullOrEmpty(rule.value) && rule.value.IsDateTime())
                    {
                        var date = Convert.ToDateTime(rule.value);
                        And(x => SqlFunctions.DateDiff("d", date, x.SendDate) >= 0);
                    }



                    if (rule.field == "NotificationId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
                    {
                        int val = Convert.ToInt32(rule.value);
                        And(x => x.NotificationId == val);
                    }





                }
            }
            return this;
        }
    }
}



