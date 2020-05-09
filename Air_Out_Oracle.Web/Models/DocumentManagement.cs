using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace AirOut.Web.Models
{
    public partial class DocumentManagement : Entity
    {
        public DocumentManagement()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "业务编号", Description = "业务编号")]
        [MaxLength(50)]
        public string Operation_ID { get; set; }

        [Display(Name = "单证类型", Description = "单证类型")]
        [MaxLength(50)]
        public string DZ_Type { get; set; }

        [Display(Name = "海关编号", Description = "海关编号")]
        [MaxLength(50)]
        public string Doc_NO { get; set; }

        [Display(Name = "贸易方式", Description = "贸易方式")]
        [MaxLength(50)]
        public string Trade_Mode { get; set; }

        [Display(Name = "退单打印份数", Description = "退单打印份数")]
        [MaxLength(100)]
        public string Return_Print { get; set; }

        [Display(Name = "联单数", Description = "联单数")]
        public decimal? QTY { get; set; }

        [Display(Name = "报关抬头", Description = "报关抬头")]
        [MaxLength(50)]
        public string BG_TT { get; set; }

        [Display(Name = "拼号", Description = "拼号")]
        [MaxLength(50)]
        public string Ping_Name { get; set; }

        [Display(Name = "退单日期", Description = "退单日期")]
        public DateTime? Return_Date { get; set; }

        [Display(Name = "打印日期", Description = "打印日期")]
        public DateTime? Print_Date { get; set; }

        [Display(Name = "退客户日期", Description = "退客户日期")]
        public DateTime? Return_Customer_Date { get; set; }

        #region 新增字段

        [Display(Name = "总单号", Description = "总单号")]
        [MaxLength(20)]
        public string MBL { get; set; }

        [Display(Name = "委托方", Description = "委托方")]
        [MaxLength(100)]
        public string Entrustment_Name { get; set; }

        [Display(Name = "委托方编号", Description = "委托方编号")]
        [MaxLength(100)]
        public string Entrustment_Code { get; set; }

        [Display(Name = "航班日期", Description = "航班日期")]
        public DateTime? Flight_Date_Want { get; set; }

        [Display(Name = "是否退单", Description = "是否退单(0:否,1:是)")]
        public bool? Is_Return { get; set; }

        [Display(Name = "退单人", Description = "退单人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ReturnID { get; set; }

        [Display(Name = "退单人", Description = "退单人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string ReturnWHO { get; set; }

        [Display(Name = "是否退客户", Description = "是否退客户(0:否,1:是)")]
        public bool? Is_Return_Customer { get; set; }

        [Display(Name = "退客户人", Description = "退客户人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string Return_CustomerID { get; set; }

        [Display(Name = "退客户人", Description = "退客户人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string Return_CustomerWHO { get; set; }

        [Display(Name = "是否打印", Description = "是否打印(0:否,1:是)")]
        public bool? Is_Print { get; set; }

        [Display(Name = "签收单编号", Description = "签收单编号")]
        [MaxLength(20)]
        public string SignReceipt_Code { get; set; }

        [Display(Name = "报关方式", Description = "报关方式")]
        [MaxLength(50)]
        public string Customs_Declaration { get; set; }


        #endregion

        [Display(Name = "备注", Description = "备注")]
        [MaxLength(100)]
        public string Remark { get; set; }

        [Index("IX_DocMant_Point", IsUnique = false, Order = 2)]
        [Display(Name = "使用状态", Description = "状态-0:草稿,1:启用,-1:停用")]
        [Required]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Index("IX_DocMant_Point", IsUnique = false, Order = 1)]
        [Display(Name = "操作点", Description = "操作点")]
        public int OperatingPoint { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string ADDID { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string ADDWHO { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(50)]
        [ScaffoldColumn(false)]
        public string EDITID { get; set; }
    }
}