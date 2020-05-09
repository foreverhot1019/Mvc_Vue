
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    public class FileATTACH : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Index("IX_FileATTACH", IsUnique = false, Order = 1)]
        [Display(Name = "文件标识", Description = "相同标识 关联多个")]
        [StringLength(100)]
        public string FileGuid { get; set; }

        [Display(Name = "文件名称", Description = "文件名称")]
        [StringLength(100)]
        public string FileName { get; set; }

        [Display(Name = "文件路径", Description = "文件路径")]
        [StringLength(200)]
        public string FilePath { get; set; }

        [Display(Name = "文件大小", Description = "文件大小")]
        public decimal FileLength { get; set; }

        [Display(Name = "文件流", Description = "文件流")]
        public byte[] FileData { get; set; }

        [Display(Name = "文件流", Description = "如果是图片，加图片时 添加 缩略图")]
        public byte[] PicMinData { get; set; }

        [Display(Name = "添加时间", Description = "添加时间")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "添加人", Description = "添加人")]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime? EDITTS { get; set; }
    }
}