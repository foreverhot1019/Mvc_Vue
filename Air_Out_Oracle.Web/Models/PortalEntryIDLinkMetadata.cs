using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(PortalEntryIDLinkMetadata))]
    public partial class PortalEntryIDLink
    {
    }

    public partial class PortalEntryIDLinkMetadata
    {

        [Required(ErrorMessage = "Please enter:主键")]

        [Display(Name="主键",Description="主键")]

        public int ID{ get; set; }

        [Required(ErrorMessage = "Please enter:用户Id")]

        [Display(Name="用户Id",Description="用户Id")]

        [MaxLength(50)]

        public string UserId{ get; set; }

        [Required(ErrorMessage = "Please enter:部门")]

        [Display(Name="部门",Description="部门")]

        [MaxLength(50)]

        public string DepartMent{ get; set; }

        [Required(ErrorMessage = "Please enter:进仓编号")]

        [Display(Name="进仓编号",Description="进仓编号")]

        [MaxLength(50)]

        public string EntryID{ get; set; }

	}

	public class PortalEntryIDLinkChangeViewModel
    {
        public IEnumerable<PortalEntryIDLink> inserted { get; set; }

        public IEnumerable<PortalEntryIDLink> deleted { get; set; }

        public IEnumerable<PortalEntryIDLink> updated { get; set; }
    }
}
