using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMI.Web.Models
{
    //乘机人信息
    public partial class PlanePerson:Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "机票订单")]
        public int? AirTicketOrderId { get; set; }

        [Display(Name = "机票订单")]
        [ForeignKey("AirTicketOrderId")]
        public virtual AirTicketOrder OAirTicket { get; set; }

        [Display(Name = "中文名")]
        [MaxLength(50)]
        public string NameChs { get; set; }

        [Display(Name = "英文姓")]
        [MaxLength(100)]
        public string LastNameEng { get; set; }

        [Display(Name = "英文名")]
        [MaxLength(100)]
        public string FirstNameEng { get; set; }

        #region ScaffoldColumn

        [Index("IX_PlanePerson_OP", IsUnique = false, Order = 1)]
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
}