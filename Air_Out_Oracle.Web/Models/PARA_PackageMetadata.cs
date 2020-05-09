













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(PARA_PackageMetadata))]
    public partial class PARA_Package
    {
    }

    public partial class PARA_PackageMetadata
    {

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="代码",Description="代码")]

        [MaxLength(50)]

        public string PackageCode{ get; set; }

        [Display(Name="名称",Description="名称")]

        [MaxLength(50)]

        public string PackageName{ get; set; }

        [Display(Name="是否木质",Description="是否木质")]

        public bool IsWood{ get; set; }

        [Display(Name="描述",Description="描述")]

        [MaxLength(500)]

        public string Description{ get; set; }

        [Display(Name="是否使用",Description="是否使用")]

        public bool Status{ get; set; }

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

	public class PARA_PackageChangeViewModel
    {
        public IEnumerable<PARA_Package> inserted { get; set; }

        public IEnumerable<PARA_Package> deleted { get; set; }

        public IEnumerable<PARA_Package> updated { get; set; }
    }
}
