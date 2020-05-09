













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(SupplierMetadata))]
    public partial class Supplier
    {
    }

    public partial class SupplierMetadata
    {

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="供应商代码",Description="供应商代码")]

        [MaxLength(50)]

        public string Code{ get; set; }

        [Display(Name="供应商名称",Description="供应商名称")]

        [MaxLength(50)]

        public string Name{ get; set; }

        [Display(Name="备注",Description="备注")]

        [MaxLength(500)]

        public string Remark{ get; set; }

        [Display(Name="操作点",Description="操作点")]

        public int OperatingPoint{ get; set; }

        [Display(Name="新增人",Description="新增人")]

        [MaxLength(50)]

        public string ADDID{ get; set; }

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

        [Display(Name="修改人",Description="修改人")]

        [MaxLength(50)]

        public string EDITID{ get; set; }

	}

	public class SupplierChangeViewModel
    {
        public IEnumerable<Supplier> inserted { get; set; }

        public IEnumerable<Supplier> deleted { get; set; }

        public IEnumerable<Supplier> updated { get; set; }
    }
}
