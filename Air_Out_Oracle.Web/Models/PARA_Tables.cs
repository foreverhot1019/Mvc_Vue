
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //海关参数查询
    public partial class PARA_Tables : Entity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(50), Display(Name = "海关参数表名称", Description = "海关参数表名称")]
        public string PARA_tbName { get; set; }

        [StringLength(50), Display(Name = "海关参数Code", Description = "海关参数Code")]
        public string PARA_Code { get; set; }

        [StringLength(50), Display(Name = "海关参数名称", Description = "海关参数名称")]
        public string PARA_Name { get; set; }

        [StringLength(200), Display(Name = "海关参数Code列名", Description = "海关参数Code列名")]
        public string PARA_CodeColumn { get; set; }

        [StringLength(200), Display(Name = "海关参数名称列名", Description = "海关参数名称列名")]
        public string PARA_NameColumn { get; set; }        
       
    }
}
