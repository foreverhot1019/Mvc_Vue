using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AirOut.Web.Models
{
    [MetadataType(typeof(CompanyMetadata))]
    public partial class Company
    {
    }

    public partial class CompanyMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter : 企业名称")]
        [Display(Name = "企业名称")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Display(Name = "企业简称")]
        [MaxLength(30)]
        public string SimpleName { get; set; }

        [Display(Name = "企业英文名称")]
        [MaxLength(100)]
        public string Eng_Name { get; set; }

        [Required(ErrorMessage = "Please enter : 地址")]
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

	public class CompanyChangeViewModel
    {
        public IEnumerable<Company> inserted { get; set; }
        public IEnumerable<Company> deleted { get; set; }
        public IEnumerable<Company> updated { get; set; }
    }

}
