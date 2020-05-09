using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Models
{
    //���ز�����ѯ--���� ��������
    public partial class PARA_Customs : Entity
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "�����ڰ�����", Description = "�����ڰ�����")]
        public string Customs_Code { get; set; }

        [StringLength(20)]
        [Display(Name = "�����ڰ�����", Description = "�����ڰ�����")]
        public string Customs_Name { get; set; }

        [StringLength(100)]
        [Display(Name = "ƴ�����", Description = "ƴ�����")]
        public string PinYinSimpleName { get; set; }

        [Display(Name = "����")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "ʹ��״̬")]
        public bool Status { get; set; }

        [Display(Name = "������", Description = "������")]
        [ScaffoldColumn(true)]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "����ʱ��", Description = "����ʱ��")]
        [ScaffoldColumn(true)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "�޸���", Description = "�޸���")]
        [ScaffoldColumn(true)]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "�޸�ʱ��", Description = "�޸�ʱ��")]
        [ScaffoldColumn(true)]
        public DateTime? EDITTS { get; set; }
    }
}
