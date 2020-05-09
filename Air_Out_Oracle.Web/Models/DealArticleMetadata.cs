using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(DealArticleMetadata))]
    public partial class DealArticle
    {
    }

    public partial class DealArticleMetadata
    {

		[Required(ErrorMessage = "Please enter:Id")]

		[Display(Name="Id")]

		public int Id{ get; set; }

        [Display(Name = "成交条款代码")]

		public string DealArticleCode{ get; set; }

        [Display(Name = "成交条款名称")]

		public string DealArticleName{ get; set; }

        [Display(Name = "运输条款")]

		public string TransArticle{ get; set; }

        [Display(Name = "成交条款描述")]

		public string Description{ get; set; }

        #region 新增字段 20181121

        [Display(Name = "付款方式", Description = "付款方式")]
        [MaxLength(50)]
        public string Pay_ModeCode { get; set; }

        [Display(Name = "运费P/C", Description = "运费P/C")]
        [MaxLength(1)]
        public string Carriage { get; set; }

        [Display(Name = "运费英文", Description = "运费英文")]
        [MaxLength(50)]
        public string CarriageEns { get; set; }

        [Display(Name = "杂费P/C", Description = "杂费P/C")]
        [MaxLength(1)]
        public string Incidental_Expenses { get; set; }

        [Display(Name = "杂费英文", Description = "杂费英文")]
        [MaxLength(50)]
        public string Incidental_ExpensesEns { get; set; }

        #endregion 

        [Display(Name = "使用状态")]

        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "操作点")]

		public int OperatingPoint{ get; set; }

        [Display(Name = "新增人")]

		public string ADDWHO{ get; set; }

        [Display(Name = "新增时间")]

		public DateTime ADDTS{ get; set; }

        [Display(Name = "修改人")]

		public string EDITWHO{ get; set; }

        [Display(Name = "修改时间")]

		public DateTime EDITTS{ get; set; }

	}

	public class DealArticleChangeViewModel
    {
        public IEnumerable<DealArticle> inserted { get; set; }

        public IEnumerable<DealArticle> deleted { get; set; }

        public IEnumerable<DealArticle> updated { get; set; }
    }
}
