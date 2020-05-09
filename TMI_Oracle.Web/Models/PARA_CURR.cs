namespace TMI.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Repository.Pattern.Ef6;

    //���ز�����ѯ--����
    public partial class PARA_CURR : Entity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required, Index("IX_PARA_CURR", IsUnique = true, Order = 0)]
        [StringLength(3)]
        [Display(Name = "���ƴ���", Description = "���ƴ���")]
        public string CURR_CODE { get; set; }

        [StringLength(10)]
        [Display(Name = "��������", Description = "��������")]
        public string CURR_Name { get; set; }

        [StringLength(20)]
        [Display(Name = "����Ӣ�ļ��", Description = "����Ӣ�ļ��")]
        public string CURR_NameEng { get; set; }

        [StringLength(10)]
        [Display(Name = "���Ҵ���", Description = "���Ҵ���")]
        public string Money_CODE { get; set; }

        [Display(Name = "����")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "ʹ��״̬")]
        public EnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "������", Description = "������")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string ADDWHO { get; set; }

        [Display(Name = "����ʱ��", Description = "����ʱ��")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "�޸���", Description = "�޸���")]
        [StringLength(20)]
        [ScaffoldColumn(true)]
        public string EDITWHO { get; set; }

        [Display(Name = "�޸�ʱ��", Description = "�޸�ʱ��")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }
    }
}
