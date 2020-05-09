using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(ChangeOrderHistoryMetadata))]
    public partial class ChangeOrderHistory
    {
    }

    public partial class ChangeOrderHistoryMetadata
    {
        public int Id { get; set; }

        [Display(Name = "操作主键", Description = "操作主键")]
        public int Key_Id { get; set; }

        [Display(Name = "操作主表", Description = "操作主表")]
        [StringLength(50)]
        public string TableName { get; set; }

        [Display(Name = "操作类型", Description = "操作类型")]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ChangeOrderHistory.EnumChangeType ChangeType
        {
            get;
            set;
            //get
            //{
            //    ChangeOrderHistory.EnumChangeType OEnumChangeType;
            //    Enum.TryParse<ChangeOrderHistory.EnumChangeType>(Int_ChangeType.ToString(), out OEnumChangeType);
            //    return OEnumChangeType;
            //}
            //set {
            //    this.Int_ChangeType = (int)value;
            //}
        }

        //[Display(Name = "操作类型", Description = "操作类型")]
        //public int Int_ChangeType { get; set; }

        [Display(Name = "添加数量", Description = "添加数量")]
        public int InsertNum { get; set; }

        [Display(Name = "修改数量", Description = "修改数量")]
        public int UpdateNum { get; set; }

        [Display(Name = "删除数量", Description = "删除数量")]
        public int DeleteNum { get; set; }

        [Display(Name = "内容", Description = "内容")]
        [StringLength(200)]
        public string Content { get; set; }

        [Display(Name = "创建人ID", Description = "创建人ID")]
        [StringLength(50)]
        public string ADDID { get; set; }

        [Display(Name = "创建人", Description = "创建人")]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "创建日期", Description = "创建日期")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改日期", Description = "修改日期")]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人ID", Description = "修改人ID")]
        [StringLength(50)]
        public string EDITID { get; set; }

        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }


    }




	public class ChangeOrderHistoryChangeViewModel
    {
        public IEnumerable<ChangeOrderHistory> inserted { get; set; }
        public IEnumerable<ChangeOrderHistory> deleted { get; set; }
        public IEnumerable<ChangeOrderHistory> updated { get; set; }
    }

}
