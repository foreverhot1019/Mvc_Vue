using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace TMI.Web.Models
{
    //���Ҵ���
    public partial class PARA_Country : Entity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Index("IX_P_CountryNOKey", IsUnique = true, Order = 1)]
        [StringLength(3), Display(Name = "���ִ���", Description = "���𣨵���������")]
        public string COUNTRY_NO { get; set; }

        [Index("IX_P_CountryCOKey", IsUnique = true, Order = 1)]
        [StringLength(3), Display(Name = "����", Description = "���𣨵���������")]
        public string COUNTRY_CO { get; set; }

        [StringLength(50,MinimumLength=0), Display(Name = "����Ӣ�ļ��", Description = "����Ӣ�ļ��")]
        public string COUNTRY_EN { get; set; }

        [StringLength(50), Display(Name = "���𣨵���������", Description = "���𣨵���������")]
        public string COUNTRY_NA { get; set; }

        [StringLength(1)]
        public string EXAM_MARK { get; set; }

        [StringLength(1)]
        public string HIGH_LOW { get; set; }

        [Display(Name = "ʹ��״̬")]
        public bool Status { get; set; }

        [Display(Name = "������", Description = "������")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string ADDWHO { get; set; }

        [Display(Name = "����ʱ��", Description = "����ʱ��")]
        [ScaffoldColumn(false)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "�޸���", Description = "�޸���")]
        [StringLength(20)]
        [ScaffoldColumn(false)]
        public string EDITWHO { get; set; }

        [Display(Name = "�޸�ʱ��", Description = "�޸�ʱ��")]
        [ScaffoldColumn(false)]
        public DateTime? EDITTS { get; set; }
    }
}
