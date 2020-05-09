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
    /// 销售人
    /// </summary>
    public partial class Saller : Entity
    {
        public Saller()
        {
            ArrCusBusInfo = new HashSet<CusBusInfo>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "姓名", Description = "")]
        [Required]
        [MaxLength(50)]
        [Index("IX_Saller_Key", IsUnique = true, Order = 1)]
        [System.Web.Mvc.Remote("GetIsUse", "Sallers", AdditionalFields = "Id", HttpMethod = "post", ErrorMessage = "姓名已存在")]
        public string Name { get; set; }

        [Display(Name = "代码", Description = "")]
        [Required]
        [MaxLength(50)]
        [Index("IX_Saller_Code", IsUnique = true, Order = 1)]
        public string Code { get; set; }

        [Display(Name = "电话", Description = "")]
        [Required]
        [MaxLength(200)]
        public string PhoneNumber { get; set; }

        [Display(Name = "公司", Description = "")]
        [MaxLength(200)]
        public string Company { get; set; }

        [Display(Name = "地址", Description = "")]
        [MaxLength(500)]
        public string Address { get; set; }

        [Display(Name = "描述")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "使用状态", Description = "状态( 0:草稿,1:启用,-1:停用)")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "客商")]
        public ICollection<CusBusInfo> ArrCusBusInfo { get; set; }

        #region 不使用 基架 生成字段

        [Index("IX_Point", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(true)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        [ScaffoldColumn(true)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion
    }
}