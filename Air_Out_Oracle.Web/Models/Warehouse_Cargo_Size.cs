using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    /// <summary>
    /// 仓库接单子表
    /// </summary>
    public partial class Warehouse_Cargo_Size : Entity
    {
        public Warehouse_Cargo_Size(){

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "仓库接单Id", Description = "仓库接单Id")]
        [Required]
        public int? Warehouse_Receipt_Id { get; set; }

        [ForeignKey("Warehouse_Receipt_Id")]
        public virtual Warehouse_receipt OWarehouse_receipt { get; set; }

        [Display(Name = "进仓编号", Description = "进仓编号")]
        [Required]
        [MaxLength(20)]
        public string Entry_Id { get; set; }

        [Display(Name = "长", Description = "长")]
        public decimal? CM_Length { get; set; }

        [Display(Name = "宽", Description = "宽")]
        public decimal? CM_Width { get; set; }

        [Display(Name = "高", Description = "高")]
        public decimal? CM_Height { get; set; }

        [Display(Name = "件", Description = "件")]
        public decimal? CM_Piece { get; set; }

        #region 基本字段

        [Index("IX_WH_CgS_Status", IsUnique = false, Order = 1)]
        [Display(Name = "使用状态", Description = "状态(0:否,1:是)")]
        public AirOutEnumType.UseStatusIsOrNoEnum Status { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

        [Index("IX_WH_CgS_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion
    }
}