﻿



//using System;
//using System.ComponentModel.DataAnnotations;
//using System.Collections.Generic;

//namespace TMI.Web.Models
//{
//    [MetadataType(typeof(BaseCodeMetadata))]
//    public partial class BaseCode
//    {
//    }

//    public partial class BaseCodeMetadata
//    {
//        [Required(ErrorMessage = "Please enter : Id")]
//        [Display(Name = "Id")]
//        public int Id { get; set; }

//        [Required(ErrorMessage = "Please enter : 代码类型")]
//        [Display(Name = "代码类型")]
//        [MaxLength(20)]
//        public string CodeType { get; set; }

//        [Required(ErrorMessage = "Please enter : 描述")]
//        [Display(Name = "描述")]
//        [MaxLength(50)]
//        public string Description { get; set; }

//    }
//    public class BaseCodeChangeViewModel
//    {
//        public IEnumerable<BaseCode> inserted { get; set; }
//        public IEnumerable<BaseCode> deleted { get; set; }
//        public IEnumerable<BaseCode> updated { get; set; }
//    }
//}
