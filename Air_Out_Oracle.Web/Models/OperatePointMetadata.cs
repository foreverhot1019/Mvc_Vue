



using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(OperatePointMetadata))]
    public partial class OperatePoint
    {
    }

    public partial class OperatePointMetadata
    {
        [Display(Name = "主键")]
        public int ID { get; set; }

        [Display(Name = "代码")]
        [StringLength(50)]
        public string OperatePointCode { get; set; }

        [Display(Name = "名称")]
        [StringLength(100)]
        public string OperatePointName { get; set; }

        [Display(Name = "描述")]
        [StringLength(100)]
        public string Description { get; set; }

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

        [Display(Name = "操作仓库", Description = "操作仓库")]
        public ICollection<OperatePointList> OperatePointLists { get; set; }

    }

    public class OperatePointChangeViewModel
    {
        public IEnumerable<OperatePoint> inserted { get; set; }
        public IEnumerable<OperatePoint> deleted { get; set; }
        public IEnumerable<OperatePoint> updated { get; set; }
    }

}
