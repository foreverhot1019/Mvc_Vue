using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //外部用户 进仓编号 关联
    public partial class PortalEntryIDLink : Entity
    {
        //public PortalEntryIDLink()
        //{
        //    UserId = "-";
        //    DepartMent = "-";
        //    EntryID = "-";
        //}

        [Display(Name = "主键")]
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "用户Id")]
        [Required,MaxLength(50)]
        [Index("IX_PEIDLink",IsUnique=true,Order=1)]
        public string UserId { get; set; }

        [Display(Name = "部门")]
        [Required, MaxLength(50)]
        public string DepartMent { get; set; }

        [Display(Name = "进仓编号")]
        [Required, MaxLength(50)]
        [Index("IX_PEIDLink", IsUnique = true, Order = 2)]
        public string EntryID { get; set; }
    }
}