using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //公司
    public partial class Company : Entity
    {
        public Company()
        {
            
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "企业名称")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "企业简称")]
        [MaxLength(30)]
        public string SimpleName { get; set; }

        [Display(Name = "企业英文名称")]
        [MaxLength(100)]
        public string Eng_Name { get; set; }

        [Display(Name = "地址")]
        [MaxLength(100)]
        public string Address { get; set; }

        [Display(Name = "市")]
        [MaxLength(20)]
        public string City { get; set; }

        [Display(Name = "省份")]
        [MaxLength(20)]
        public string Province { get; set; }

        [Display(Name = "注册日期")]
        public DateTime? RegisterDate { get; set; }

        [Display(Name = "Logo")]
        [MaxLength(500)]
        public string Logo { get; set; }
         
    }
}