using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirOut.Web.Models
{
    public partial class SetDefaults
    {
    }

    public partial class SetDefaultsdata
    {

        [Required]
        [Display(Name = "TableName", Order = 0)]
        public string TableName { get; set; }

        [Required]
        [Display(Name = "TableNameChs", Order = 1)]
        public string TableNameChs { get; set; }

        [Required]
        [Display(Name = "ColumnName", Order = 2)]
        public string ColumnName { get; set; }

        [Required]
        [Display(Name = "ColumnNameChs", Order = 3)]
        public string ColumnNameChs { get; set; }

        [Required]
        [Display(Name = "DefaultValue", Order = 4)]
        public string DefaultValue { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "DataType", Order = 5)]
        public string DataType { get; set; }
    }


    public class SetDefaultsChangeViewModel
    {
        public IEnumerable<SetDefaults> inserted { get; set; }

        public IEnumerable<SetDefaults> deleted { get; set; }

        public IEnumerable<SetDefaults> updated { get; set; }
    }
}