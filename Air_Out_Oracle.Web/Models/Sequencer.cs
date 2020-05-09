using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    //Please RegisterDbSet in DbContext.cs
    //public DbSet<BaseCode> BaseCodes { get; set; }
    //public Entity.DbSet<CodeItem> CodeItems { get; set; }
    public partial class Sequencer:Entity
    {
        public Sequencer()
        {
        }

        [Key]
        public int Id { get; set; }

        [Display( Name="序号",Description="Sequencer 序列"),Required(ErrorMessage="必填")]
        //[Index("IX_SEQUENCER", IsUnique = true, Order = 1)]
        public int Seed { get; set; }

        [MaxLength(20), Display(Name = "键值", Description = "Sequencer 序列 键值"), Required(ErrorMessage = "必填")]
        [Index("IX_SEQUENCER", IsUnique = true, Order = 2)]
        public string Prefix { get; set; }

    }
}