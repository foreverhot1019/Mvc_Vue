using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //主分单关联表
    public partial class OPS_M_H_Relation : Entity
    {
        public OPS_M_H_Relation()
        {

        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_M_H_Point", IsUnique = false, Order = 3)]
        [Display(Name = "总单ID", Description = "总单ID(用于关联分单号)")]
        public int MBL_Id { get; set; }

        [Index("IX_M_H_Point", IsUnique = false, Order = 4)]
        [Display(Name = "分单ID", Description = "分单ID(用于关联总单号)")]
        public int HBL_Id { get; set; }


        #region 基本字段

        [Index("IX_M_H_Point", IsUnique = false, Order = 2)]
        [Display(Name = "使用状态", Description = "状态(0:否,1:是)")]
        public AirOutEnumType.UseStatusIsOrNoEnum Status { get; set; }

        [Index("IX_M_H_Point", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }

        //[Display(Name = "备注", Description = "备注")]
        //[MaxLength(500)]
        //public string Remark { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }

        #endregion


    }
}