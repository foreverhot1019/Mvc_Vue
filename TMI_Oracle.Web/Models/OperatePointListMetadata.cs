using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(OperatePointListMetadata))]
    public partial class OperatePointList
    {
    }

    public partial class OperatePointListMetadata
    {
        [Display(Name = "主键")]
        public int ID { get; set; }

        [Display(Name = "操作点ID")]
        public int? OperatePointID { get; set; }

        [Display(Name = "操作点代码")]
        [StringLength(50)]
        public string OperatePointCode { get; set; }

        [Display(Name = "操作点")]
        public OperatePoint OperatePoint { get; set; }

        [Display(Name = "仓库代码")]
        [StringLength(50)]
        public string CompanyCode { get; set; }

        [Display(Name = "仓库名称")]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [Display(Name = "是否启用")]
        public bool IsEnabled { get; set; }

        [Display(Name = "添加时间", Description = "添加时间")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "添加人ID", Description = "添加人ID")]
        [StringLength(50)]
        public string ADDID { get; set; }

        [Display(Name = "添加人", Description = "添加人")]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改人ID", Description = "修改人ID")]
        [StringLength(50)]
        public string EDITID { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime? EDITTS { get; set; }

    }

    public class OperatePointListChangeViewModel
    {
        public IEnumerable<OperatePointList> inserted { get; set; }
        public IEnumerable<OperatePointList> deleted { get; set; }
        public IEnumerable<OperatePointList> updated { get; set; }
    }

}
