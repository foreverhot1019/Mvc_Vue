using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;

using AirOut.Web.Models;
using AirOut.Web.Repositories;

using System.Data;
using System.Reflection;

using Newtonsoft.Json;
using AirOut.Web.Extensions;
using System.IO;

namespace AirOut.Web.Services
{
    public class QuotedPriceService : Service< QuotedPrice >, IQuotedPriceService
    {
        private readonly IRepositoryAsync<QuotedPrice> _repository;
		private readonly IDataTableImportMappingService _mappingservice;
        public  QuotedPriceService(IRepositoryAsync< QuotedPrice> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
		public void ImportDataTable(System.Data.DataTable datatable)
        {
			var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "QuotedPrice").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                QuotedPrice item = new QuotedPrice();
                foreach (var field in mapping)
                {
					var defval = field.DefaultValue;
					var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
					if (contation && row[field.SourceFieldName] != DBNull.Value)
					{
						Type quotedpricetype = item.GetType();
						PropertyInfo propertyInfo = quotedpricetype.GetProperty(field.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
					}
					else if (!string.IsNullOrEmpty(defval))
					{
						Type quotedpricetype = item.GetType();
						PropertyInfo propertyInfo = quotedpricetype.GetProperty(field.FieldName);
						if (defval.ToLower() == "now" && propertyInfo.PropertyType ==typeof(DateTime))
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(DateTime.Now, propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(defval, propertyInfo.PropertyType), null);
                        }
					}
                }
                
                this.Insert(item);
            }
        }

		public Stream ExportExcel(string filterRules = "",string sort = "Id", string order = "asc")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			var quotedprice = this.Query(new QuotedPriceQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).Select().ToList();
			var datarows = quotedprice.Select( n => new {  
Id = n.Id, 
SerialNo = n.SerialNo, 
SettleAccount = n.SettleAccount, 
FeeCode = n.FeeCode, 
FeeName = n.FeeName, 
StartPlace = n.StartPlace, 
TransitPlace = n.TransitPlace, 
EndPlace = n.EndPlace, 
AirLineCo = n.AirLineCo, 
AirLineNo = n.AirLineNo, 
WHBuyer = n.WHBuyer, 
ProxyOperator = n.ProxyOperator, 
DealWithArticle = n.DealWithArticle, 
BSA = n.BSA, 
CustomsType = n.CustomsType, 
InspectMark = n.InspectMark, 
GetOrdMark = n.GetOrdMark, 
MoorLevel = n.MoorLevel, 
BillingUnit = n.BillingUnit, 
Price = n.Price, 
CurrencyCode = n.CurrencyCode, 
FeeConditionVal1 = n.FeeConditionVal1, 
CalcSign1 = n.CalcSign1, 
FeeCondition = n.FeeCondition, 
CalcSign2 = n.CalcSign2, 
FeeConditionVal2 = n.FeeConditionVal2, 
CalcFormula = n.CalcFormula, 
FeeMin = n.FeeMin, 
FeeMax = n.FeeMax, 
AuditStatus = n.AuditStatus, 
StartDate = n.StartDate, 
EndDate = n.EndDate, 
Description = n.Description, 
Status = n.Status, 
OperatingPoint = n.OperatingPoint, 
ADDWHO = n.ADDWHO, 
ADDTS = n.ADDTS, 
EDITWHO = n.EDITWHO, 
EDITTS = n.EDITTS}).ToList();
           
            return ExcelHelper.ExportExcel(typeof(QuotedPrice), datarows);
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(QuotedPrice OQuotedPrice)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();

            #region 取缓存

            var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var ArrFeeType = (List<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
            var ArrDealArticle = (List<DealArticle>)CacheHelper.Get_SetCache(Common.CacheNameS.DealArticle);
            var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var ArrPARA_AirLine = (List<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);
            var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            var ArrFeeUnit = (List<FeeUnit>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeUnit);

            #endregion

            //费用代码
            if (!string.IsNullOrEmpty(OQuotedPrice.FeeCode))
            {
                var OArrFeeType = ArrFeeType.Where(x => x.FeeCode == OQuotedPrice.FeeCode).FirstOrDefault();
                ODynamic.FeeTypeNAME = OArrFeeType == null ? "" : OArrFeeType.FeeName;
            }
            //结算方，获取的客商信息
            if (!string.IsNullOrEmpty(OQuotedPrice.SettleAccount) && OQuotedPrice.SettleAccount != "-")
            {
                var OArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OQuotedPrice.SettleAccount).FirstOrDefault();
                ODynamic.SettleAccountNAME = OArrCusBusInfo == null ? "" : OArrCusBusInfo.EnterpriseShortName;
            }
            //订舱方
            if (!string.IsNullOrEmpty(OQuotedPrice.WHBuyer) && OQuotedPrice.WHBuyer != "-")
            {
                var OArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OQuotedPrice.WHBuyer).FirstOrDefault();
                ODynamic.WHBuyerNAME = OArrCusBusInfo == null ? "" : OArrCusBusInfo.EnterpriseShortName;
            }
            //成交条款
            if (!string.IsNullOrEmpty(OQuotedPrice.DealWithArticle) && OQuotedPrice.DealWithArticle != "-")
            {
                var OArrDealArticle = ArrDealArticle.Where(x => x.DealArticleCode == OQuotedPrice.DealWithArticle).FirstOrDefault();
                ODynamic.DealWithArticleNAME = OArrDealArticle == null ? "" : OArrDealArticle.DealArticleName;
            }
            //起始地
            if (!string.IsNullOrEmpty(OQuotedPrice.StartPlace) && OQuotedPrice.StartPlace != "-")
            {
                var OArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OQuotedPrice.StartPlace).FirstOrDefault();
                ODynamic.StartPlaceNAME = OArrPARA_AirPort == null ? "" : OArrPARA_AirPort.PortName;
            }
            ////中转地
            //if (!string.IsNullOrEmpty(OQuotedPrice.TransitPlace) && OQuotedPrice.TransitPlace != "-")
            //{
            //    var OArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OQuotedPrice.TransitPlace).FirstOrDefault();
            //    ODynamic.TransitPlaceNAME = OArrPARA_AirPort == null ? "" : OArrPARA_AirPort.PortName;
            //}
            //目的地
            if (!string.IsNullOrEmpty(OQuotedPrice.EndPlace) && OQuotedPrice.EndPlace != "-")
            {
                var OArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == OQuotedPrice.EndPlace).FirstOrDefault();
                ODynamic.EndPlaceNAME = OArrPARA_AirPort == null ? "" : OArrPARA_AirPort.PortName;
            }
            //航空公司
            if (!string.IsNullOrEmpty(OQuotedPrice.AirLineCo) && OQuotedPrice.AirLineCo != "-")
            {
                var OArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OQuotedPrice.AirLineCo).FirstOrDefault();
                ODynamic.AirLineCoNAME = OArrCusBusInfo == null ? "" : OArrCusBusInfo.EnterpriseShortName;
            }
            //航班号
            if (!string.IsNullOrEmpty(OQuotedPrice.AirLineNo) && OQuotedPrice.AirLineNo != "-")
            {
                var OArrPARA_AirLine = ArrPARA_AirLine.Where(x => x.AirCode == OQuotedPrice.AirLineNo).FirstOrDefault();
                ODynamic.AirLineNoNAME = OArrPARA_AirLine == null ? "" : OArrPARA_AirLine.AirLine;
            }
            //靠级
            if (!string.IsNullOrEmpty(OQuotedPrice.MoorLevel) && OQuotedPrice.MoorLevel != "-")
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "MoorLevel" && x.LISTCODE == OQuotedPrice.MoorLevel).FirstOrDefault();
                ODynamic.MoorLevelNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //代操作ProxyOperator
            //BSA
            //审批状态AuditStatus
            //使用状态Status
            //报关方式
            if (!string.IsNullOrEmpty(OQuotedPrice.CustomsType) && OQuotedPrice.CustomsType != "-")
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "CusType" && x.LISTCODE == OQuotedPrice.CustomsType).FirstOrDefault();
                ODynamic.CustomsTypeNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //币制
            if (!string.IsNullOrEmpty(OQuotedPrice.CurrencyCode) && OQuotedPrice.CurrencyCode != "-")
            {
                var OArrPARA_CURR = ArrPARA_CURR.Where(x => x.CURR_CODE == OQuotedPrice.CurrencyCode).FirstOrDefault();
                ODynamic.CurrencyCodeNAME = OArrPARA_CURR == null ? "" : OArrPARA_CURR.CURR_Name;
            }
            //计费单位
            if (!string.IsNullOrEmpty(OQuotedPrice.BillingUnit) && OQuotedPrice.BillingUnit != "-")
            {
                var BillingUnit = 0;
                if (int.TryParse(OQuotedPrice.BillingUnit, out BillingUnit))
                {
                    var OArrFeeUnit = ArrFeeUnit.Where(x => x.Id == BillingUnit).FirstOrDefault();
                    ODynamic.BillingUnitNAME = OArrFeeUnit == null ? "" : OArrFeeUnit.FeeUnitName;
                }
            }
            //计算单位
            if (!string.IsNullOrEmpty(OQuotedPrice.FeeCondition) && OQuotedPrice.FeeCondition != "-")
            {
                var OArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "FeeConditionAr" && x.LISTCODE == OQuotedPrice.FeeCondition).FirstOrDefault();
                ODynamic.FeeConditionNAME = OArrBD_DEFDOC_LIST == null ? "" : OArrBD_DEFDOC_LIST.LISTNAME;
            }
            //商检标志
            //取单标志
            //计费运算符1
            //计费运算符2
            //计费公式
            return ODynamic;
        }
    }
}