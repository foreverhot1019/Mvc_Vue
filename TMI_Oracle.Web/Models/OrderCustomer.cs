using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    //订单客户
    public partial class OrderCustomer : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "订单")]
        public int? OrderId { get; set; }

        [Display(Name = "订单")]
        [ForeignKey("OrderId")]
        public virtual Order OOrder { get; set; }

        #region 客户

        [Display(Name = "客户", Description = "")]
        public int? CustomerId { get; set; }

        [Display(Name = "客户", Description = "")]
        [ForeignKey("CustomerId")]
        public virtual Customer OCustomer { get; set; }

        #endregion

        #region ScaffoldColumn

        [Index("IX_OrdCustomer_OP", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        [ScaffoldColumn(false)]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }

        #endregion
    }

    /// <summary>
    /// 订单客户 显示
    /// </summary>
    public class OrderCustomerView
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? CustomerId { get; set; }
        public string NameChs { get; set; }//中文名
        public string NameEng { get; set; }//英文名
        public bool? Sex { get; set; }//性别
        public DateTime Birthday { get; set; }//出生年月
        public string BornCity { get; set; }//出生地
        public string IdCardType { get; set; }//证件类型
        public string IdCardNo { get; set; }//证件号
        public string CheckCity { get; set; }//签发地
        public DateTime? Limit_S { get; set; }//签发日期
        public DateTime? Limit_E { get; set; }//有效期
        public string Remark { get; set; }//备注
    }
}