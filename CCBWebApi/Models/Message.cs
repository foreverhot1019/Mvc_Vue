using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCBWebApi.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Request请求")]
        public bool IsRequest { get; set; }	

        [Display(Name="交易流水号")]
        [MaxLength(50)]
        public string Trans_Id{ get; set; }	

        [Display(Name="交易码")]
        [MaxLength(50)]
        public string Trans_Code{ get; set; }

        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "新增人", Description = "新增人")]
        [MaxLength(20)]
        [ScaffoldColumn(false)]
        public string AddUser { get; set; }

        [Display(Name = "新增时间", Description = "新增时间")]
        [ScaffoldColumn(false)]
        public DateTime AddDate { get; set; }

    }
}