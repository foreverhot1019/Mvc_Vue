using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TMI.Web.Models
{
    /// <summary>
    /// 数据变更日志
    /// </summary>
    public partial class ChangeOrderHistory : Entity
    {
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum EnumChangeType:int
        {
            UnChange = 0,
            Insert = 1,
            Modify = 2,
            Delete = 3
        }

        public ChangeOrderHistory()
        {
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_Point_Key_Id", IsUnique = false, Order = 2)]
        [Display(Name = "操作主键", Description = "操作主键")]
        public int Key_Id { get; set; }

        [Index("IX_Point_Key_Id", IsUnique = false, Order = 3)]
        [Display(Name = "操作主表", Description = "操作主表")]
        [StringLength(50)]
        public string TableName { get; set; }

        [Display(Name = "操作类型", Description = "操作类型")]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public EnumChangeType ChangeType { get; set; }

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

        [Index("IX_Point", IsUnique = false, Order = 1)]
        [Index("IX_Point_Key_Id", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }

    }
}