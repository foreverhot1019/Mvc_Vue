using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CCBWebApi.Models
{
    [Serializable]
    [XmlRoot("ENTITY")]
    public class FLD1001
    {
        [XmlElement("Head")]
        public EntityHead OEntityHead { get; set; }

        [XmlElement("Data")]
        public FLD1001Data OEntityData { get; set; }
        /// <summary>
        /// 为Null值不渲染节点
        /// 开头必须为ShouldSerialize
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeOEntityData()
        {
            return OEntityData == null || string.IsNullOrEmpty(OEntityData.Unn_Soc_Cr_Cd);
        }

        //[XmlArray("Receivable_Accout_List")]
        //[XmlArrayItem("unit")]
        [XmlElement("Receivable_Accout_List")]
        public FLD1001ResData[] ArrFLD1001ResData { get; set; }
    }

    [Serializable]
    public class EntityHead
    {
        //[XmlElement(IsNullable = true)]//强制生成节点
        public string Trans_Id { get; set; }	//交易流水号
        public string Trans_Code { get; set; }	//交易码
        public string return_code { get; set; }	//返回码
        public string return_msg { get; set; }	//返回描述

        //public int? BATCH_QTY { get; set; }
        ///// <summary>
        ///// 为Null值不渲染节点
        ///// 开头必须为ShouldSerialize
        ///// </summary>
        ///// <returns></returns>
        //public bool ShouldSerializeBATCH_QTY()
        //{
        //    return BATCH_QTY.HasValue;
        //}
    }

    [Serializable]
    public class FLD1001Data
    {
        //[XmlAttribute()]//节点渲染为属性
        public string Unn_Soc_Cr_Cd { get; set; }	//统一社会信用代码	String
        public string KeyCode { get; set; }	//唯一码，区分客户贷款
        public string CoPlf_ID { get; set; }	//合作平台编号	String
        public string remark1 { get; set; }	//保留字段1	String
        public string remark2 { get; set; }	//保留字段2	String
        public string Splr_Nm { get; set; }	//供应商名称
        public string Pyr_Nm { get; set; }	//付款方名称
        public string TLmt_Val { get; set; }	//总额度值
        public decimal LoanApl_Amt { get; set; }	//申请贷款金额
        public string Txn_ExDat { get; set; }	//交易到期日期
        public string Rfnd_AccNo { get; set; }	//回款账号
    }

    [Serializable]
    public class FLD1001ResData
    {
        public string Splr_Nm { get; set; }	//供应商名称
        public string Unn_Soc_Cr_Cd { get; set; }	//统一社会信用代码	String
        public string Pyr_Nm { get; set; }	//付款方名称
        public string TLmt_Val { get; set; }	//总额度值
        public decimal LoanApl_Amt { get; set; }	//申请贷款金额
        public string Txn_ExDat { get; set; }	//交易到期日期
        public string Rfnd_AccNo { get; set; }	//回款账号
    }

}