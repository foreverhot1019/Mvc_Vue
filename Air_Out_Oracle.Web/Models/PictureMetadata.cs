













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(PictureMetadata))]
    public partial class Picture
    {
    }

    public partial class PictureMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

		[Display(Name="Id")]

		public int Id{ get; set; }

        [Display(Name = "关联编号")]

		public string Code{ get; set; }

        [Display(Name = "上传状态")]

        public AirOutEnumType.UseStatusIsOrNoEnum Status { get; set; }

        [Display(Name = "类型")]

        public AirOutEnumType.PictureTypeEnum Type { get; set; }

        [Display(Name = "地址")]

		public string Address{ get; set; }

        [Display(Name = "操作点")]

		public int OperatingPoint{ get; set; }

        [Display(Name = "备注")]

		public string Remark{ get; set; }

        [Display(Name = "新增人")]

		public string ADDWHO{ get; set; }

        [Display(Name = "新增时间")]

		public DateTime ADDTS{ get; set; }

        [Display(Name = "修改人")]

		public string EDITWHO{ get; set; }

        [Display(Name = "修改时间")]

		public DateTime EDITTS{ get; set; }

	}

	public class PictureChangeViewModel
    {
        public IEnumerable<Picture> inserted { get; set; }

        public IEnumerable<Picture> deleted { get; set; }

        public IEnumerable<Picture> updated { get; set; }
    }
}
