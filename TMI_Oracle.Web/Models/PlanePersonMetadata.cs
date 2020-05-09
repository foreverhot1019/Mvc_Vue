













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(PlanePersonMetadata))]
    public partial class PlanePerson
    {
    }

    public partial class PlanePersonMetadata
    {

        [Display(Name="机票订单",Description="机票订单")]

        public AirTicketOrder OAirTicket{ get; set; }

        [Required(ErrorMessage = "Please enter:Id")]

        [Display(Name="Id",Description="Id")]

        public int Id{ get; set; }

        [Display(Name="机票订单",Description="机票订单")]

        public int AirTicketOrderId{ get; set; }

        [Display(Name="中文名",Description="中文名")]

        [MaxLength(50)]

        public string NameChs{ get; set; }

        [Display(Name="英文姓",Description="英文姓")]

        [MaxLength(100)]

        public string LastNameEng{ get; set; }

        [Display(Name="英文名",Description="英文名")]

        [MaxLength(100)]

        public string FirstNameEng{ get; set; }

        [Display(Name="操作点",Description="操作点")]

        public int OperatingPoint{ get; set; }

        [Display(Name="新增人",Description="新增人")]

        [MaxLength(50)]

        public string ADDID{ get; set; }

        [Display(Name="新增人",Description="新增人")]

        [MaxLength(20)]

        public string ADDWHO{ get; set; }

        [Display(Name="新增时间",Description="新增时间")]

        public DateTime ADDTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]

        [MaxLength(20)]

        public string EDITWHO{ get; set; }

        [Display(Name="修改时间",Description="修改时间")]

        public DateTime EDITTS{ get; set; }

        [Display(Name="修改人",Description="修改人")]

        [MaxLength(50)]

        public string EDITID{ get; set; }

	}

	public class PlanePersonChangeViewModel
    {
        public IEnumerable<PlanePerson> inserted { get; set; }

        public IEnumerable<PlanePerson> deleted { get; set; }

        public IEnumerable<PlanePerson> updated { get; set; }
    }
}
