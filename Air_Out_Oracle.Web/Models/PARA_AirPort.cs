using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //���ز�����ѯ--�ۿڣ�Port��
    public partial class PARA_AirPort : Entity
    {
        public PARA_AirPort()
        {
            Status = AirOutEnumType.UseStatusEnum.Enable;
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_PARA_AirPort", IsUnique = true, Order = 0)]
        [Required, StringLength(20)]
        [Display(Name = "�ڰ�����", Description = "�ڰ�����")]
        public string PortCode { get; set; }

        [StringLength(20)]
        [Display(Name = "��������", Description = "��������")]
        public string PortName { get; set; }

        [StringLength(50)]
        [Display(Name = "Ӣ������", Description = "Ӣ������")]
        public string PortNameEng { get; set; }

        [StringLength(50)]
        [Display(Name = "�ڰ�����", Description = "�ڰ�����")]
        public string PortType { get; set; }

        [StringLength(20)]
        [Display(Name = "���Ҵ���", Description = "���Ҵ���")]
        public string CountryCode { get; set; }

        [StringLength(50)]
        [Display(Name = "̫ƽ�ۿ�����", Description = "̫ƽ�ۿ�����")]
        public string PeacePortName { get; set; }

        [Display(Name = "����")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "ʹ��״̬")]
        public AirOutEnumType.UseStatusEnum Status { get; set; }

        [Display(Name = "������", Description = "������")]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "����ʱ��", Description = "����ʱ��")]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "�޸���", Description = "�޸���")]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "�޸�ʱ��", Description = "�޸�ʱ��")]
        public DateTime? EDITTS { get; set; }
    }
}
