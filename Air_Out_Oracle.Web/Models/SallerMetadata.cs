using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(SallerMetadata))]
    public partial class Saller
    {
    }

    public partial class SallerMetadata
    {
        [Display(Name="客商",Description="客商")]
        public CusBusInfo ArrCusBusInfo{ get; set; }

        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name="Id",Description="Id")]
        public int Id{ get; set; }

        [Display(Name="姓名",Description="姓名")]
        [MaxLength(50)]
        public string Name{ get; set; }

        [Display(Name="电话",Description="电话")]
        [MaxLength(200)]
        public string PhoneNumber{ get; set; }

        [Display(Name="公司",Description="公司")]
        [MaxLength(200)]
        public string Company{ get; set; }

        [Display(Name="地址",Description="地址")]
        [MaxLength(500)]
        public string Address{ get; set; }

        [Display(Name="描述",Description="描述")]
        [MaxLength(500)]
        public string Description{ get; set; }

        [Display(Name="使用状态",Description="使用状态")]
        public AirOut.Web.Models.AirOutEnumType.UseStatusEnum Status{ get; set; }

        [Display(Name="操作点",Description="操作点")]
        public int OperatingPoint{ get; set; }

        [Display(Name="新增人",Description="新增人")]
        [MaxLength(50)]
        public string ADDID{ get; set; }

        [Display(Name="新增人",Description="新增人")]
        [MaxLength(20)]
        public string ADDWHO{ get; set; }

        [Display(Name="新增时间",Description="新增时间")]
        public DateTime? ADDTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]
        [MaxLength(20)]
        public string EDITWHO{ get; set; }

        [Display(Name="修改时间",Description="修改时间")]
        public DateTime? EDITTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]
        [MaxLength(50)]
        public string EDITID{ get; set; }

	}

	public class SallerChangeViewModel
    {
        public IEnumerable<Saller> inserted { get; set; }

        public IEnumerable<Saller> deleted { get; set; }

        public IEnumerable<Saller> updated { get; set; }
    }
}
