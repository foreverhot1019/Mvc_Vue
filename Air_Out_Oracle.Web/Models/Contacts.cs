using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    public partial class Contacts : Entity
    {
        public Contacts()
        {
            Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
            ContactType = "-";
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_Contacts_Type", IsUnique = false, Order = 1)]
        [Display(Name = "类别", Description = "")]
        [Required,MaxLength(20)]
        public string ContactType { get; set; }

        [Index("IX_Contacts_CId", IsUnique = false, Order = 1)]
        [Display(Name = "客商代码（业务伙伴代码）", Description = "客商代码")]
        [StringLength(50)]
        public string CusBusInfoId { get; set; }

        [Display(Name = "名称", Description = "")]
        [MaxLength(200)]
        public string CompanyName { get; set; }

        [Display(Name = "代码", Description = "代码")]
        [MaxLength(50)]
        public string CompanyCode { get; set; }

        [Display(Name = "地址", Description = "")]
        [MaxLength(500)]
        public string CoAddress { get; set; }

        [Display(Name = "区域", Description = "县/市")]
        [MaxLength(50)]
        public string CoArea { get; set; }

        [Display(Name = "国家", Description = "")]
        [MaxLength(50)]
        public string CoCountry { get; set; }

        [Display(Name = "联系电话", Description = "")]
        [MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "联系人", Description = "")]
        [MaxLength(50)]
        public string ContactWHO { get; set; }

        [Display(Name = "联系地址信息", Description = "")]
        [MaxLength(2000)]
        public string ContactInfo { get; set; }

        #region 基本字段

        [Display(Name = "使用状态", Description = "状态(0:否,1:是)")]
        public AirOutEnumType.UseStatusIsOrNoEnum Status { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(100)]
        public string Remark { get; set; }

        [Index("IX_Contacts_OP", IsUnique = false, Order = 1)]
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