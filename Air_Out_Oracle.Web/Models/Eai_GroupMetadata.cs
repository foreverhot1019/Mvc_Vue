using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(Eai_GroupMetadata))]
    public partial class Eai_Group
    {
    }

    public partial class Eai_GroupMetadata
    {
        [Required(ErrorMessage = "Please enter:Id")]
        [Display(Name = "Id", Description = "Id")]
        public int Id { get; set; }

        [Display(Name = "集团代码", Description = "集团代码")]
        [MaxLength(50)]
        public string Code { get; set; }

        [Display(Name = "集团名称", Description = "集团名称")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

        [Display(Name = "使用状态", Description = "使用状态")]
        public AirOut.Web.Models.AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        public DateTime ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        public string EDITID { get; set; }

    }

    public class Eai_GroupChangeViewModel
    {
        public IEnumerable<Eai_Group> inserted { get; set; }

        public IEnumerable<Eai_Group> deleted { get; set; }

        public IEnumerable<Eai_Group> updated { get; set; }
    }
}
