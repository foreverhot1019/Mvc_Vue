using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    public class ComboDridListModel
    {
        [Display(Name="代码",Description="唯一键")]
        public string ID { get; set; }
        [Display(Name = "名称", Description = "名称")]
        public string TEXT { get; set; }
        [Display(Name = "备注", Description = "备注")]
        public string Remark { get; set; }
    }
}