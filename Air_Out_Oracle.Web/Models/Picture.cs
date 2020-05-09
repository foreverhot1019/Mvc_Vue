using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //图片表
    public partial class Picture : Entity
    {
        public Picture()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "关联编号", Description = "关联编号")]
        [MaxLength(50)]
        public string Code { get; set; }

        [Index("IX_Pic_Status", IsUnique = false, Order = 1)]
        [Display(Name = "上传状态", Description = "上传状态")]
        public AirOutEnumType.UseStatusIsOrNoEnum Status { get; set; }

        [Index("IX_Pic_Status", IsUnique = false, Order = 2)]
        [Display(Name = "类型", Description = "类型(变形,受潮,破损,POD)")]
        public AirOutEnumType.PictureTypeEnum Type { get; set; }

        [Display(Name = "地址", Description = "图片存储地址")]
        [MaxLength(500)]
        public string Address { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [StringLength(100)]
        public string Remark { get; set; }

        #region 基本字段

        [Index("IX_Pic_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
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