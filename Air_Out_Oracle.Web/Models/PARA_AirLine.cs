using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //航班信息表
    public partial class PARA_AirLine : Entity
    {
        public PARA_AirLine()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "航班代码")]
        [Required,StringLength(50)]
        [Index("IX_P_AirLine_Key", IsUnique = true, Order = 1)]
        public string AirCode { get; set; }

        [Display(Name = "航线")]
        [StringLength(50)]
        public string AirLine { get; set; }

        [Display(Name = "航空公司")]
        [StringLength(50)]
        [Index("IX_P_AirLine_Key", IsUnique = true, Order = 2)]
        public string AirCompany { get; set; }

        [Display(Name = "起始地")]
        [StringLength(50)]
        [Index("IX_P_AirLine_Key", IsUnique = true, Order = 3)]
        public string StarStation { get; set; }

        [Display(Name = "中转地")]
        [StringLength(50)]
        public string TransferStation { get; set; }

        [Display(Name = "目的地")]
        [StringLength(50)]
        public string EndStation { get; set; }

        [Display(Name = "航期")]
        public DateTime? AirDate { get; set; }

        [Display(Name = "航班时间")]
        [StringLength(10)]
        public string AirTime { get; set; }

        [Display(Name = "描述")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "使用状态")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

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

    }
}