using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;
using System.ComponentModel;

namespace TMI.Web.Models
{
    public class FileATTACH : Entity
    {
        public FileATTACH()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Index("IX_FileATTACH", IsUnique = false, Order = 1)]
        [Display(Name = "文件标识", Description = "相同标识 关联多个")]
        [Required, MaxLength(100)]
        public string FileGuid { get; set; }

        [Display(Name = "原文件名", Description = "原文件名")]
        [Required, MaxLength(100)]
        public string OriginFileName { get; set; }

        [DefaultValue("-")]
        [Display(Name = "文件扩展名", Description = "文件扩展名")]
        [Required,MaxLength(20)]
        public string FileExtension { get; set; }

        [Display(Name = "文件大小", Description = "文件大小")]
        public long FileSize { get; set; }

        [Display(Name = "文件名", Description = "文件名")]
        [Required, MaxLength(100)]
        public string FileName { get; set; }

        [Display(Name = "文件路径", Description = "文件路径")]
        [Required, MaxLength(200)]
        public string FilePath { get; set; }

        [Display(Name = "文件大小", Description = "文件大小")]
        [Required]
        public decimal FileLength { get; set; }

        [Display(Name = "文件流", Description = "文件流")]
        [Required]
        public byte[] FileData { get; set; }

        [Display(Name = "文件流", Description = "如果是图片，加图片时 添加 缩略图")]
        public byte[] PicMinData { get; set; }

        [Display(Name = "添加时间", Description = "添加时间")]
        public DateTime ADDTS { get; set; }

        [Display(Name = "添加人", Description = "添加人")]
        [Required,MaxLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "修改人", Description = "修改人")]
        [MaxLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改时间", Description = "修改时间")]
        public DateTime? EDITTS { get; set; }
    }
}