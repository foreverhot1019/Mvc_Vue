













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(TestMVC_CRUDMetadata))]
    public partial class TestMVC_CRUD
    {
    }

    public partial class TestMVC_CRUDMetadata
    {

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="业务编号",Description="业务编号")]

        [MaxLength(50)]

        public string Dzbh{ get; set; }

        [Display(Name="测试1",Description="测试1")]

        [MaxLength(50)]

        public string TestColumn1{ get; set; }

        [Display(Name="测试2",Description="测试2")]

        [MaxLength(50)]

        public string TestColumn2{ get; set; }

        [Display(Name="测试3",Description="测试3")]

        [MaxLength(50)]

        public string TestColumn3{ get; set; }

        [Display(Name="测试4",Description="测试4")]

        [MaxLength(50)]

        public string TestColumn4{ get; set; }

        [Display(Name="测试5",Description="测试5")]

        [MaxLength(50)]

        public string TestColumn5{ get; set; }

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

	public class TestMVC_CRUDChangeViewModel
    {
        public IEnumerable<TestMVC_CRUD> inserted { get; set; }

        public IEnumerable<TestMVC_CRUD> deleted { get; set; }

        public IEnumerable<TestMVC_CRUD> updated { get; set; }
    }
}
