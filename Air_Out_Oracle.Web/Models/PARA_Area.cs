using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //���ز�����ѯ--���ڵ���
    public partial class PARA_Area : Entity
    {
        public PARA_Area()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "���Ҵ���", Description = "���Ҵ���")]
        [Required, Index("IX_PARA_Area", IsUnique = true, Order = 1)]
        [StringLength(10)]
        public string Country_CO { get; set; }

        [Display(Name = "��������", Description = "��������")]
        [Required, Index("IX_PARA_Area", IsUnique = true, Order = 2)]
        [StringLength(10)]
        public string AreaCode { get; set; }

        [Display(Name = "��������", Description = "��������")]
        [StringLength(100)]
        public string AreaName { get; set; }

        [Display(Name = "ʹ��״̬")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

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
