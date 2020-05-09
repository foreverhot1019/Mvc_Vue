using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(TradeTypeMetadata))]
    public partial class TradeType
    {
    }

    public partial class TradeTypeMetadata
    {
        [Required(ErrorMessage = "Please enter:ID")]
        [Display(Name="ID",Description="ID")]
        public int ID{ get; set; }

        [Display(Name="代码",Description="代码")]
        [MaxLength(50)]
        public string Code{ get; set; }

        [Display(Name="名称",Description="名称")]
        [MaxLength(100)]
        public string Name{ get; set; }

        [Display(Name="描述",Description="描述")]
        [MaxLength(500)]
        public string Description{ get; set; }

        [Display(Name="使用状态",Description="使用状态")]
        public AirOut.Web.Models.AirOutEnumType.UseStatusEnum Status{ get; set; }

        [Display(Name="新增人",Description="新增人")]
        [MaxLength(20)]
        public string ADDWHO{ get; set; }

        [Display(Name="新增时间",Description="新增时间")]
        public DateTime ADDTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]
        [MaxLength(20)]
        public string EDITWHO{ get; set; }

        [Display(Name="修改时间",Description="修改时间")]
        public DateTime EDITTS{ get; set; }

	}

	public class TradeTypeChangeViewModel
    {
        public IEnumerable<TradeType> inserted { get; set; }

        public IEnumerable<TradeType> deleted { get; set; }

        public IEnumerable<TradeType> updated { get; set; }
    }
}
