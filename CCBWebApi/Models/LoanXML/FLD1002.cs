using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CCBWebApi.Models
{
    [Serializable]
    [XmlRoot("ENTITY")]
    public class FLD1002
    {
        [XmlElement("Head")]
        public EntityHead OEntityHead { get; set; }
        [XmlElement("Data")]
        public FLD1002Data OEntityData { get; set; }
    }

    [Serializable]
    public class FLD1002Data
    {
        public string Unn_Soc_Cr_Cd { get; set; }	//统一社会信用代码
        public string KeyCode { get; set; }	//唯一码，区分客户贷款
        public string CoPlf_ID { get; set; }	//合作平台编号
        public string Sign_Dt { get; set; }	//签约日期
        public string Sgn_Cst_Nm { get; set; }	//签约客户名称
        public string AR_Lmt { get; set; }	//合约额度
        public string Lmt_ExDat { get; set; }	//额度到期日期
        public string Rfnd_AccNo { get; set; }	//回款账号
        public string remark1 { get; set; }	//保留字段1
        public string remark2 { get; set; }	//保留字段2
    }
}