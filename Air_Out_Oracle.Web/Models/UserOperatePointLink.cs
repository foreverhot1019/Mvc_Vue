using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //用户 操作点关联
    public partial class UserOperatePointLink : Entity
    {
        [Display(Name = "主键")]
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "用户Id")]
        [StringLength(50), Index("IX_UserOperatePointLink",IsUnique=true,Order=1)]
        public string UserId { get; set; }

        [Display(Name = "操作点Id")]
        [Index("IX_UserOperatePointLink",IsUnique=true,Order=2)]
        public int OperateOpintId { get; set; }



        //[Display(Name = "是否启用")]
        //public bool IsEnabled { get; set; }

        //[Display(Name = "添加时间", Description = "添加时间")]
        //public DateTime? ADDTS { get; set; }

        //[Display(Name = "添加人", Description = "添加人")]
        //[StringLength(20)]
        //public string ADDID { get; set; }

        //[Display(Name = "添加人", Description = "添加人")]
        //[StringLength(20)]
        //public string ADDWHO { get; set; }

        //[Display(Name = "修改人", Description = "修改人")]
        //[StringLength(20)]
        //public string EDITWHO { get; set; }

        //[Display(Name = "修改人", Description = "修改人")]
        //[StringLength(20)]
        //public string EDITID { get; set; }

        //[Display(Name = "修改时间", Description = "修改时间")]
        //public DateTime? EDITTS { get; set; }
    }
}