













using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TMI.Web.Models
{
    [MetadataType(typeof(MailReceiverMetadata))]
    public partial class MailReceiver
    {
    }

    public partial class MailReceiverMetadata
    {

        [Required(ErrorMessage = "Please enter : ID")]

        [Display(Name = "ID")]

        public int ID { get; set; }


        [Display(Name = "ErrType")]

        public string ErrType { get; set; }


        [Display(Name = "ErrMethod")]

        public string ErrMethod { get; set; }


        [Display(Name = "RecMailAddress")]

        public string RecMailAddress { get; set; }


        [Display(Name = "CCMailAddress")]

        public string CCMailAddress { get; set; }


        [Display(Name = "ADDID")]

        public string ADDID { get; set; }


        [Display(Name = "ADDWHO")]

        public string ADDWHO { get; set; }


        [Display(Name = "ADDTS")]

        public DateTime ADDTS { get; set; }


        [Display(Name = "EDITWHO")]

        public string EDITWHO { get; set; }


        [Display(Name = "EDITTS")]

        public DateTime EDITTS { get; set; }


        [Display(Name = "EDITID")]

        public string EDITID { get; set; }


    }




	public class MailReceiverChangeViewModel
    {
        public IEnumerable<MailReceiver> inserted { get; set; }
        public IEnumerable<MailReceiver> deleted { get; set; }
        public IEnumerable<MailReceiver> updated { get; set; }
    }

}
