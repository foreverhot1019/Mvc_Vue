using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(UserOperatePointLinkMetadata))]
    public partial class UserOperatePointLink
    {
    }

    public partial class UserOperatePointLinkMetadata
    {
        [Required(ErrorMessage = "Please enter : ID")]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [Display(Name = "OperateOpintId")]
        public int OperateOpintId { get; set; }

    }

    public class UserOperatePointLinkChangeViewModel
    {
        public IEnumerable<UserOperatePointLink> inserted { get; set; }
        public IEnumerable<UserOperatePointLink> deleted { get; set; }
        public IEnumerable<UserOperatePointLink> updated { get; set; }
    }

}
