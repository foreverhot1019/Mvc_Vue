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
    public class CostMoneyService : Service<CostMoney>, ICostMoneyService
    {
        private readonly IRepositoryAsync<CostMoney> _repository;
        private readonly IBms_Bill_ArService _bms_Bill_ArService;
        private readonly IDataTableImportMappingService _mappingservice;
        //
        private RedisHelp.RedisHelper ORedisHelper { get; set; }

        public CostMoneyService(IRepositoryAsync<CostMoney> repository,
            IDataTableImportMappingService mappingservice,
            IBms_Bill_ArService bms_Bill_ArService)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
            _bms_Bill_ArService = bms_Bill_ArService;
            ORedisHelper = new RedisHelp.RedisHelper();
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "CostMoney").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                CostMoney item = new CostMoney();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type costmoneytype = item.GetType();
                        PropertyInfo propertyInfo = costmoneytype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type costmoneytype = item.GetType();
                        PropertyInfo propertyInfo = costmoneytype.GetProperty(field.FieldName);
                        if (defval.ToLower() == "now" && propertyInfo.PropertyType == typeof(DateTime))
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

        public Stream ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var costmoney = this.Query(new CostMoneyQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = costmoney.Select(n => new
            {
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
                EDITTS = n.EDITTS
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(CostMoney), datarows);
        }

        /// <summary>
        /// 应付自动结算
        /// </summary>
        /// <param name="OOPS_M_Order">接单数据</param>
        /// <param name="ArrFeeType">费用</param>
        /// <returns></returns>
        public string AutoAddFee(OPS_M_Order OOPS_M_Order, IEnumerable<String> ArrFeeType = null)
        {
            try
            {
                var retMsg = "";//错误信息
                if (OOPS_M_Order == null || OOPS_M_Order.Id <= 0 || OOPS_M_Order.OPS_EntrustmentInfors == null || OOPS_M_Order.OPS_EntrustmentInfors.Any(x => string.IsNullOrEmpty(x.Operation_Id)))
                {
                    retMsg = "应付自动结算，接单主单为空或接单委托为空";//错误信息
                    return retMsg;
                }
                var ArrJSZL = new List<string> { 
                    "KYF","RYF","ZZF","KCF","ZDF",//订舱主单-结算重量
                };
                if (ArrFeeType == null || !ArrFeeType.Any())
                {
                    ArrFeeType = new List<string> {
                        "XXF","CCF","BGF","YDF","LDF"
                    };
                }
                ArrFeeType = ArrJSZL.Concat(ArrFeeType);
                //反向从依赖注入中取上下文dbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;

                var ArrApDtl_Add = new List<Bms_Bill_Ap_Dtl>();//记录新增的明细
                #region 验证应付账单表头，是否已存在

                var OPS_M_OrdRep = _repository.GetRepository<OPS_M_Order>();
                var BmsApRep = _repository.GetRepository<Bms_Bill_Ap>();
                var BmsApDtlRep = _repository.GetRepository<Bms_Bill_Ap_Dtl>();
                //枚举
                var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
                //客商
                var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                //计费单位
                var ArrFeeUnit = (IEnumerable<FeeUnit>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeUnit);
                //费用代码
                var ArrFeeTypes = (IEnumerable<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
                //委托
                var OOPS_EntrustmentInfor = OOPS_M_Order.OPS_EntrustmentInfors == null ? null : OOPS_M_Order.OPS_EntrustmentInfors.FirstOrDefault();
                var Dzbh = OOPS_EntrustmentInfor.Operation_Id;//应付-作业单号
                var ArrLineNo = new List<Tuple<string, int>>();//应收-序号 刷新表头 合计时使用
                var MBL = OOPS_EntrustmentInfor.MBL;//总单号
                int OPS_M_OrdId_ZD = 0;//总单应付空订单 ID
                var Dzbh_ZD = "";//总单应付-作业单号
                var ArrOPS_M_OrdId = new List<int>() { OOPS_M_Order.Id };//该总单号下,所有应付行账单（包括总单应付行账单）
                if (!string.IsNullOrWhiteSpace(MBL))
                {
                    var ArrOPS_M_Ord = OPS_M_OrdRep.Queryable().Where(x => !x.OPS_EntrustmentInfors.Any(n => n.Is_TG) && x.MBL == MBL).Include(x => x.OPS_EntrustmentInfors).ToList();
                    if (ArrOPS_M_Ord != null && ArrOPS_M_Ord.Any())
                    {
                        var OPS_M_Ord_ZD = ArrOPS_M_Ord.Where(x => x.OPS_BMS_Status).FirstOrDefault();
                        if (OPS_M_Ord_ZD != null && OPS_M_Ord_ZD.Id > 0)
                        {
                            OPS_M_OrdId_ZD = OPS_M_Ord_ZD.Id;
                            if (OPS_M_Ord_ZD.OPS_EntrustmentInfors.Any())
                                Dzbh_ZD = OPS_M_Ord_ZD.OPS_EntrustmentInfors.FirstOrDefault().Operation_Id;
                        }
                        var QOPS_M_OrdId = ArrOPS_M_Ord.Where(x => x.Id != OOPS_M_Order.Id).Select(x => x.Id).ToList();
                        ArrOPS_M_OrdId.AddRange(QOPS_M_OrdId);//该总单号下 所有的应付数据
                    }
                }
                //应付账单
                Bms_Bill_Ap OBms_Bill_Ap;
                var QBms_Bill_Ap = BmsApRep.Queryable().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && ArrOPS_M_OrdId.Contains(x.Ops_M_OrdId)).ToList().Where(x => !x.Cancel_Status).OrderByDescending(x => x.Id).ToList();
                var QBms_Bill_ApDtl = BmsApDtlRep.Queryable().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && ArrOPS_M_OrdId.Contains(x.Ops_M_OrdId)).ToList();
                if (QBms_Bill_Ap.Any(x => x.EDITTS.HasValue && x.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet) || QBms_Bill_ApDtl.Any(x => x.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet && x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess))
                {
                    retMsg = "应付账单含有已审核/修改的自动账单(包含总单应付行关联的账单)";//错误信息
                    return retMsg;
                }
                //--------------------------------- 结算用到值 ---------------------------------------
                var Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want;//航班日期
                var Book_Flat_Code = OOPS_EntrustmentInfor.Book_Flat_Code;//订航方
                var EndPlace = OOPS_EntrustmentInfor.End_Port;//目的地
                var Airways_Code = OOPS_EntrustmentInfor.Airways_Code;//航空公司
                var StartPlace = OOPS_EntrustmentInfor.Depart_Port;//起始地
                var KSF = OOPS_EntrustmentInfor.Is_Book_Flat;//KSF
                var IS_MoorLevel = OOPS_EntrustmentInfor.IS_MoorLevel;//是否靠级
                var Is_BAS = OOPS_EntrustmentInfor.Is_BAS;//BSA
                var Delivery_Point = OOPS_EntrustmentInfor.Delivery_Point;//交货点
                decimal Account_Weight_DC = OOPS_EntrustmentInfor.Account_Weight_DC == null ? 0 : OOPS_EntrustmentInfor.Account_Weight_DC.Value;//订仓主单-结算重量
                decimal Account_Weight_SK = OOPS_EntrustmentInfor.Account_Weight_SK == null ? 0 : OOPS_EntrustmentInfor.Account_Weight_SK.Value;//订仓分单-结算重量
                decimal AMS = OOPS_EntrustmentInfor.AMS == null ? 0 : OOPS_EntrustmentInfor.AMS.Value;//AMS份数
                decimal Weight_DC = OOPS_EntrustmentInfor.Weight_DC == null ? 0 : OOPS_EntrustmentInfor.Weight_DC.Value;//订仓主单-毛重
                //单证信息
                var ArrDocManagement = _repository.GetRepository<DocumentManagement>().Queryable().Where(x => x.Operation_ID == OOPS_EntrustmentInfor.Operation_Id).ToList();
                var SumQTY = ArrDocManagement.Sum(x => x.QTY);//联单数合计
                //报关信息
                var ArrCusInspection = _repository.GetRepository<CustomsInspection>().Queryable().Where(x => x.Operation_ID == OOPS_EntrustmentInfor.Operation_Id).ToList().
                    Where(x => !string.IsNullOrWhiteSpace(x.Customs_Declaration));
                var SumNum_PG = ArrCusInspection.Sum(x => x.Num_BG);//报关票数合计

                if (QBms_Bill_Ap.Any())
                {
                    var QAutoAp = QBms_Bill_Ap.Where(x => x.ArrBms_Bill_Ap_Dtl.Any(n => n.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet));
                    if (QAutoAp.Any())
                    {
                        OBms_Bill_Ap = QAutoAp.FirstOrDefault();
                    }
                    else
                        OBms_Bill_Ap = QBms_Bill_Ap.FirstOrDefault();
                }

                #endregion

                #region 找报价（不匹配区间条件）

                //取应付报价-缓存
                var ArrCostMoney = (IEnumerable<CostMoney>)CacheHelper.Get_SetCache(Common.CacheNameS.CostMoney);
                ArrCostMoney = ArrCostMoney.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess).ToList();
                var QArrCostMoney = ArrCostMoney.Where(x => x.StartDate <= Flight_Date_Want && x.EndDate > Flight_Date_Want);
                if (!QArrCostMoney.Any())//航班日期
                {
                    retMsg = "找不到应付报价-航班日期";//错误信息
                    return retMsg;
                }
                //QArrCostMoney = QArrCostMoney.Where(x => x.EndPlace == EndPlace && x.WHBuyer == Book_Flat_Code);
                //if (!QArrCostMoney.Any())//订航方
                //{
                //    retMsg = "找不到应付报价-订航方";//错误信息
                //    return retMsg;
                //}

                #endregion

                if (!QArrCostMoney.Any())
                {
                    retMsg = "找不到应付报价";//错误信息
                    return retMsg;
                }
                else
                {
                    var IdNum = 0;//应付 账单Id
                    //var setMO = false;//设置币制和结算方
                    foreach (var item in ArrFeeType)
                    {
                        #region 删除自动结算明细

                        //获取 费用明细
                        IEnumerable<Bms_Bill_Ap_Dtl> QBms_Bill_Ap_Dtl = QBms_Bill_ApDtl.Where(x => x.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet && x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess && x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Charge_Code == item).ToList();

                        if (QBms_Bill_Ap_Dtl.Any())
                        {
                            if (item == "BGF")//只删除 本单委托和总单应付行账单的 BGF
                                QBms_Bill_Ap_Dtl = QBms_Bill_Ap_Dtl.Where(x => x.Ops_M_OrdId == OOPS_M_Order.Id || x.Ops_M_OrdId == OPS_M_OrdId_ZD);
                            var DelDate = DateTime.Now.ToString("yyyyMMdd");
                            var DelDateTime = DateTime.Now.ToString("yyyyMMdd HHmmss");
                            foreach (var itemDtl in QBms_Bill_Ap_Dtl)
                            {
                                if (!ArrLineNo.Any(x => x.Item1 == itemDtl.Dzbh && x.Item2 == itemDtl.Line_No))
                                    ArrLineNo.Add(new Tuple<string, int>(itemDtl.Dzbh, itemDtl.Line_No));//记录 应付-序号，重新合计费用时 使用

                                //设置删除状态
                                var i_entryDtl = WebdbContxt.Entry(itemDtl);
                                itemDtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Deleted;
                                i_entryDtl.State = EntityState.Deleted;
                                ORedisHelper.ListRightPush(Common.RedisKeyDelArAp_Dtl + ":" + DelDate, "Bms_Bill_ArApDTL@|@" + itemDtl.Id.ToString() + "@|@0@|@" + DelDateTime);
                            }
                        }

                        #endregion

                        var QQArrCostMoney = QArrCostMoney.Where(x => x.FeeCode == item);
                        if (!QQArrCostMoney.Any())
                            continue;

                        #region 取报价 不找区间

                        if (QQArrCostMoney.Any(x => x.EndPlace == EndPlace))//目的地
                            QQArrCostMoney = QQArrCostMoney.Where(x => x.EndPlace == EndPlace || string.IsNullOrWhiteSpace(x.EndPlace));
                        else
                            QQArrCostMoney = QQArrCostMoney.Where(x => string.IsNullOrWhiteSpace(x.EndPlace));

                        if (QQArrCostMoney.Any(x => x.WHBuyer == Book_Flat_Code))//订航方
                            QQArrCostMoney = QQArrCostMoney.Where(x => x.WHBuyer == Book_Flat_Code);
                        else
                            QQArrCostMoney = QQArrCostMoney.Where(x => string.IsNullOrWhiteSpace(x.WHBuyer));

                        if (QQArrCostMoney.Any(x => x.AirLineCo == Airways_Code))//航空公司
                            QQArrCostMoney = QQArrCostMoney.Where(x => x.AirLineCo == Airways_Code);
                        else
                            QQArrCostMoney = QQArrCostMoney.Where(x => string.IsNullOrWhiteSpace(x.AirLineCo));

                        if (QQArrCostMoney.Any(x => x.StartPlace == StartPlace))//起始地
                            QQArrCostMoney = QQArrCostMoney.Where(x => x.StartPlace == StartPlace);
                        else
                            QQArrCostMoney = QQArrCostMoney.Where(x => string.IsNullOrWhiteSpace(x.StartPlace));

                        if (KSF == null || KSF == false)//KSF
                        {
                            if (QQArrCostMoney.Any(x => x.Is_Book_Flat != null && x.Is_Book_Flat == false))
                                QQArrCostMoney = QQArrCostMoney.Where(x => x.Is_Book_Flat != null && x.Is_Book_Flat == false);
                            else
                                QQArrCostMoney = QQArrCostMoney.Where(x => x.Is_Book_Flat == null);
                        }
                        else
                        {
                            if (QQArrCostMoney.Any(x => x.Is_Book_Flat != null && x.Is_Book_Flat == true))
                                QQArrCostMoney = QQArrCostMoney.Where(x => x.Is_Book_Flat != null && x.Is_Book_Flat == true);
                            else
                                QQArrCostMoney = QQArrCostMoney.Where(x => x.Is_Book_Flat == null);
                        }

                        if (!Is_BAS)//BSA
                        {
                            if (QQArrCostMoney.Any(x => !string.IsNullOrWhiteSpace(x.BSA) && x.BSA == "0"))
                                QQArrCostMoney = QQArrCostMoney.Where(x => !string.IsNullOrWhiteSpace(x.BSA) && x.BSA == "0");
                            else
                                QQArrCostMoney = QQArrCostMoney.Where(x => string.IsNullOrWhiteSpace(x.BSA));
                        }
                        else
                        {
                            if (QQArrCostMoney.Any(x => !string.IsNullOrWhiteSpace(x.BSA) && x.BSA == "1"))
                                QQArrCostMoney = QQArrCostMoney.Where(x => !string.IsNullOrWhiteSpace(x.BSA) && x.BSA == "1");
                            else
                                QQArrCostMoney = QQArrCostMoney.Where(x => string.IsNullOrWhiteSpace(x.BSA));
                        }

                        if (IS_MoorLevel)//是否靠级
                        {
                            var MoorLevel = OOPS_EntrustmentInfor.MoorLevel;//靠级单位
                            if (QQArrCostMoney.Any(x => x.MoorLevel == MoorLevel))
                                QQArrCostMoney = QQArrCostMoney.Where(x => x.MoorLevel == MoorLevel);
                            else
                                QQArrCostMoney = QQArrCostMoney.Where(x => string.IsNullOrWhiteSpace(x.MoorLevel));
                        }
                        else
                            QQArrCostMoney = QQArrCostMoney.Where(x => string.IsNullOrWhiteSpace(x.MoorLevel));

                        #endregion

                        CostMoney OCostMoney = null;
                        OBms_Bill_Ap = null;

                        #region 获取报价区间匹配 判断报价

                        var QCostMoney = QQArrCostMoney;
                        if (!QCostMoney.Any())
                            continue;
                        switch (item)
                        {
                            case "XXF"://【AMS份数】有值
                                if (AMS <= 0)
                                    continue;
                                break;
                            case "CCF"://勾选KSF  且  交货点有值（匹配对应）
                                if (KSF == true && !string.IsNullOrWhiteSpace(Delivery_Point))
                                {
                                    QCostMoney = QCostMoney.Where(x => x.Is_Book_Flat != null && x.Is_Book_Flat == true && x.DeliveryPoint == Delivery_Point);
                                    if (!QCostMoney.Any())
                                        continue;
                                }
                                else
                                    continue;
                                break;
                            case "YDF"://运抵费//【报关方式】有值
                            case "BGF"://报关费//【报关方式】有值
                                //一票KSF业务下：会有一个报关费账单，含多条费用明细
                                if (!ArrCusInspection.Any())
                                    continue;
                                break;
                        }

                        var NewQCostMoney = QCostMoney.Where(x => !string.IsNullOrEmpty(x.FeeCondition) && x.FeeCondition != "-");
                        if (NewQCostMoney.Any())
                        {
                            NewQCostMoney = NewQCostMoney.Where(x => !string.IsNullOrEmpty(x.CalcSign1) || !string.IsNullOrEmpty(x.CalcSign2));
                            if (NewQCostMoney.Any())
                            {
                                var NewArrCostMoney = new List<CostMoney>();//零时记录报价

                                #region 满足 区间 报价

                                foreach (var CMitem in NewQCostMoney)
                                {
                                    decimal CalcVal = 0;

                                    #region 获取 计算单位 值

                                    //计费单位
                                    string BillingUnitName = "";
                                    var QArrFeeUnit = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "FeeConditionAp" && x.LISTCODE == CMitem.FeeCondition);
                                    if (QArrFeeUnit.Any())
                                    {
                                        BillingUnitName = QArrFeeUnit.FirstOrDefault().LISTNAME;
                                        switch (BillingUnitName)
                                        {
                                            case "总单结算重量":
                                                CalcVal = Account_Weight_DC;
                                                break;
                                            case "总单毛重":
                                                CalcVal = Weight_DC;
                                                break;
                                            case "分单结算重量":
                                                CalcVal = Account_Weight_SK;
                                                break;
                                            case "AMS份数":
                                                CalcVal = AMS;
                                                break;
                                            case "报关票数":
                                                break;
                                            case "报关票数合计":
                                                CalcVal = SumNum_PG ?? 0;
                                                break;
                                            case "联单数合计":
                                                CalcVal = SumQTY ?? 0;
                                                break;
                                        }
                                    }

                                    #endregion

                                    if (CalcVal <= 0)//跳过没有 计算值的费用（报关费 联单费）
                                        continue;

                                    #region 区间计算

                                    switch (CMitem.CalcSign1)
                                    {
                                        case ">":
                                            if (CMitem.FeeConditionVal1 > CalcVal)
                                            {
                                                if (CMitem.FeeConditionVal2 <= 0)
                                                {
                                                    NewArrCostMoney.Add(CMitem);
                                                    continue;
                                                }
                                            }
                                            else
                                                continue;
                                            break;
                                        case ">=":
                                            if (CMitem.FeeConditionVal1 >= CalcVal)
                                            {
                                                if (CMitem.FeeConditionVal2 <= 0)
                                                {
                                                    NewArrCostMoney.Add(CMitem);
                                                    continue;
                                                }
                                            }
                                            else
                                                continue;
                                            break;
                                        case "<":
                                            if (CMitem.FeeConditionVal1 < CalcVal)
                                            {
                                                if (CMitem.FeeConditionVal2 <= 0)
                                                {
                                                    NewArrCostMoney.Add(CMitem);
                                                    continue;
                                                }
                                            }
                                            else
                                                continue;
                                            break;
                                        case "<=":
                                            if (CMitem.FeeConditionVal1 <= CalcVal)
                                            {
                                                if (CMitem.FeeConditionVal2 <= 0)
                                                {
                                                    NewArrCostMoney.Add(CMitem);
                                                    continue;
                                                }
                                            }
                                            else
                                                continue;
                                            break;
                                        case "==":
                                            if (CMitem.FeeConditionVal1 == CalcVal)
                                            {
                                                if (CMitem.FeeConditionVal2 <= 0)
                                                {
                                                    NewArrCostMoney.Add(CMitem);
                                                    continue;
                                                }
                                            }
                                            else
                                                continue;
                                            break;
                                        case "!=":
                                            if (CMitem.FeeConditionVal1 != CalcVal)
                                            {
                                                if (CMitem.FeeConditionVal2 <= 0)
                                                {
                                                    NewArrCostMoney.Add(CMitem);
                                                    continue;
                                                }
                                            }
                                            else
                                                continue;
                                            break;
                                        default:
                                            continue;
                                            break;
                                    }
                                    switch (CMitem.CalcSign2)
                                    {
                                        case ">":
                                            if (CMitem.FeeConditionVal2 < CalcVal)
                                            {
                                                NewArrCostMoney.Add(CMitem);
                                                continue;
                                            }
                                            break;
                                        case ">=":
                                            if (CMitem.FeeConditionVal2 <= CalcVal)
                                            {
                                                NewArrCostMoney.Add(CMitem);
                                                continue;
                                            }
                                            break;
                                        case "<":
                                            if (CMitem.FeeConditionVal2 > CalcVal)
                                            {
                                                NewArrCostMoney.Add(CMitem);
                                                continue;
                                            }
                                            break;
                                        case "<=":
                                            if (CMitem.FeeConditionVal2 >= CalcVal)
                                            {
                                                NewArrCostMoney.Add(CMitem);
                                                continue;
                                            }
                                            break;
                                        case "==":
                                            if (CMitem.FeeConditionVal2 == CalcVal)
                                            {
                                                NewArrCostMoney.Add(CMitem);
                                                continue;
                                            }
                                            break;
                                        case "!=":
                                            if (CMitem.FeeConditionVal2 != CalcVal)
                                            {
                                                NewArrCostMoney.Add(CMitem);
                                                continue;
                                            }
                                            break;
                                    }

                                    #endregion
                                }

                                #endregion

                                if (NewArrCostMoney.Any())
                                {
                                    OCostMoney = NewArrCostMoney.OrderByDescending(x => x.Id).FirstOrDefault();
                                }
                                else
                                {
                                    var WQCostMoney = QCostMoney.Where(x => string.IsNullOrEmpty(x.FeeCondition) || x.FeeCondition == "-");
                                    if (WQCostMoney.Any())
                                        OCostMoney = WQCostMoney.FirstOrDefault();
                                    else
                                        OCostMoney = new CostMoney();//不匹配任何报价
                                }
                            }
                        }

                        #endregion

                        if (OCostMoney == null)
                            OCostMoney = QCostMoney.OrderByDescending(x => x.Id).FirstOrDefault();

                        if (OCostMoney != null && OCostMoney.Id > 0)
                        {
                            #region 计费单位 

                            string BillingUnitName = "";
                            int BillingUnit = 0;
                            int.TryParse(OCostMoney.BillingUnit, out BillingUnit);
                            var QArrFeeUnit = ArrFeeUnit.Where(x => x.Id == BillingUnit);
                            if (QArrFeeUnit.Any())
                            {
                                BillingUnitName = QArrFeeUnit.FirstOrDefault().FeeUnitName;
                            }
                            bool ManyBGF = false;//BGF 按多个生产
                            decimal BillingUnitVal = 0;
                            switch (BillingUnitName)
                            {
                                case "总单结算重量":
                                    BillingUnitVal = Account_Weight_DC;
                                    break;
                                case "总单毛重":
                                    BillingUnitVal = Weight_DC;
                                    break;
                                case "分单结算重量":
                                    BillingUnitVal = Account_Weight_SK;
                                    break;
                                case "AMS份数":
                                    BillingUnitVal = AMS;
                                    break;
                                case "报关票数":
                                    if (item == "BGF" || item == "YDF")
                                        ManyBGF = true;
                                    break;
                                case "报关票数合计":
                                    BillingUnitVal = SumNum_PG ?? 0;
                                    break;
                                case "联单数合计":
                                    BillingUnitVal = SumQTY ?? 0;
                                    break;
                            }

                            #endregion

                            #region 计算公式

                            switch (OCostMoney.CalcFormula)
                            {
                                //case "A"://单价*计费单位
                                //    OBms_Bill_Ap_Dtl.Account2 = OCostMoney.Price * OBms_Bill_Ap_Dtl.Qty;
                                //    break;
                                case "B"://单价*1
                                    BillingUnitVal = 1;
                                    break;
                                case "C"://单价*(计费单位-1)
                                    BillingUnitVal = BillingUnitVal - 1;
                                    break;
                            }

                            #endregion

                            //数量小于0的，跳过
                            if (!ManyBGF && BillingUnitVal <= 0)
                                continue;

                            var QQBms_Bill_Ap = QBms_Bill_Ap.Where(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess && x.Bill_Object_Id == OCostMoney.SettleAccount && x.Money_Code == OCostMoney.CurrencyCode);
                            //是否含有总单应付行 过滤 其他委托数据
                            //BGF不挂在总单应付行中，其他数据都挂在 总单应付行
                            if (item == "BGF" || OPS_M_OrdId_ZD <= 0)
                            {
                                //过滤 其他委托数据
                                QQBms_Bill_Ap = QQBms_Bill_Ap.Where(x => x.Ops_M_OrdId == OOPS_M_Order.Id && !x.IsMBLJS);
                            }
                            else
                            {
                                QQBms_Bill_Ap = QQBms_Bill_Ap.Where(x => x.IsMBLJS);
                            }

                            if (QQBms_Bill_Ap.Any())
                            {
                                OBms_Bill_Ap = QQBms_Bill_Ap.FirstOrDefault();
                            }
                            if (OBms_Bill_Ap == null)
                            {
                                #region 创建应付表头数据

                                OBms_Bill_Ap = new Bms_Bill_Ap();

                                var entry = WebdbContxt.Entry(OBms_Bill_Ap);
                                OBms_Bill_Ap.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                entry.State = EntityState.Added;

                                OBms_Bill_Ap.Id = --IdNum;
                                OBms_Bill_Ap.Ops_M_OrdId = OOPS_M_Order.Id;
                                OBms_Bill_Ap.Dzbh = OOPS_EntrustmentInfor.Operation_Id;
                                OBms_Bill_Ap.MBL = string.IsNullOrWhiteSpace(OOPS_EntrustmentInfor.MBL) ? "-" : OOPS_EntrustmentInfor.MBL;
                                OBms_Bill_Ap.IsMBLJS = false;//指示- 是/否 总分应付数据
                                if (OPS_M_OrdId_ZD > 0 && item != "BGF")//是否含有总单应付行 BGF挂在 本订单下，其他费用 都挂在 总单应付行订单下
                                {
                                    OBms_Bill_Ap.Ops_M_OrdId = OPS_M_OrdId_ZD;
                                    OBms_Bill_Ap.Dzbh = Dzbh_ZD;
                                    OBms_Bill_Ap.IsMBLJS = true;
                                }
                                //var Line_No = SequenceBuilder.NextBmsBillArLineNo(false, OBms_Bill_Ap.Dzbh);
                                //OBms_Bill_Ap.Line_No = Convert.ToInt32(Line_No);
                                OBms_Bill_Ap.Bill_Date = DateTime.Now;
                                OBms_Bill_Ap.Status = AirOutEnumType.UseStatusEnum.Enable;
                                OBms_Bill_Ap.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                                OBms_Bill_Ap.Create_Status = AirOutEnumType.Bms_BillCreate_Status.AutoSet;
                                OBms_Bill_Ap.Payway = "YJZZ";// "PJYF";
                                OBms_Bill_Ap.Money_Code = "CNY";
                                OBms_Bill_Ap.Org_Money_Code = "CNY";
                                OBms_Bill_Ap.Bill_Type = "C/N";
                                OBms_Bill_Ap.Bill_TaxRateType = "J0";
                                OBms_Bill_Ap.Bill_TaxRate = 0;
                                OBms_Bill_Ap.Bill_HasTax = false;

                                #endregion

                                QBms_Bill_Ap.Add(OBms_Bill_Ap);
                            }

                            if (OBms_Bill_Ap.Id <= 0 && string.IsNullOrWhiteSpace(OBms_Bill_Ap.Bill_Object_Id))
                            {
                                #region 设置 序号 币制和结算方

                                //var Line_No = SequenceBuilder.NextBmsBillArLineNo(false, OBms_Bill_Ap.Dzbh);
                                //OBms_Bill_Ap.Line_No = Convert.ToInt32(Line_No);

                                OBms_Bill_Ap.Money_Code = OCostMoney.CurrencyCode;
                                OBms_Bill_Ap.Org_Money_Code = OCostMoney.CurrencyCode;

                                OBms_Bill_Ap.Bill_Object_Id = OCostMoney.SettleAccount;
                                OBms_Bill_Ap.Bill_Object_Name = "";
                                if (!string.IsNullOrEmpty(OCostMoney.SettleAccount))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OCostMoney.SettleAccount);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBms_Bill_Ap.Bill_Object_Name = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//结算方 = 主单收货人
                                    }
                                }

                                #endregion
                            }
                            //if (!ArrLineNo.Any(x => x == OBms_Bill_Ap.Line_No))
                            //    ArrLineNo.Add(OBms_Bill_Ap.Line_No);//记录 应付-序号，重新合计费用时 使用

                            #region 新增明细费用

                            var DelNum = 0;
                            Bms_Bill_Ap_Dtl OBms_Bill_Ap_Dtl = new Bms_Bill_Ap_Dtl();

                            OBms_Bill_Ap_Dtl.Id = --DelNum;
                            OBms_Bill_Ap_Dtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.AutoSet;
                            OBms_Bill_Ap_Dtl.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                            OBms_Bill_Ap_Dtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                            OBms_Bill_Ap_Dtl.Dzbh = OBms_Bill_Ap.Dzbh;
                            OBms_Bill_Ap_Dtl.Ops_M_OrdId = OBms_Bill_Ap.Ops_M_OrdId;
                            OBms_Bill_Ap_Dtl.Charge_Code = item;
                            OBms_Bill_Ap_Dtl.Unitprice2 = OCostMoney.Price;
                            OBms_Bill_Ap_Dtl.Summary = "";
                            OBms_Bill_Ap_Dtl.Bms_Bill_Ap_Id = OBms_Bill_Ap.Id;
                            OBms_Bill_Ap_Dtl.Line_No = OBms_Bill_Ap.Line_No;
                            //var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, OBms_Bill_Ap.Dzbh + "_" + OBms_Bill_Ap.Line_No, true);
                            //OBms_Bill_Ap_Dtl.Line_Id = Convert.ToInt32(DtlLine_No);

                            #region 读取费用名称

                            var QArrFeeTypes = ArrFeeTypes.Where(x => x.FeeCode == item);
                            if (QArrFeeTypes.Any())
                            {
                                OBms_Bill_Ap_Dtl.Charge_Desc = QArrFeeTypes.FirstOrDefault().FeeName;
                            }
                            if (string.IsNullOrWhiteSpace(OBms_Bill_Ap_Dtl.Charge_Desc))
                            {
                                return "费用名称无法读取";
                            }

                            #endregion

                            #region 计费单位

                            if (!ManyBGF)
                                OBms_Bill_Ap_Dtl.Qty = BillingUnitVal;
                            //string BillingUnitName = "";
                            //int BillingUnit = 0;
                            //int.TryParse(OCostMoney.BillingUnit, out BillingUnit);
                            //var QArrFeeUnit = ArrFeeUnit.Where(x => x.Id == BillingUnit);
                            //if (QArrFeeUnit.Any())
                            //{
                            //    BillingUnitName = QArrFeeUnit.FirstOrDefault().FeeUnitName;
                            //}
                            //bool ManyBGF = false;//BGF 按多个生产
                            //switch (BillingUnitName)
                            //{
                            //    case "总单结算重量":
                            //        OBms_Bill_Ap_Dtl.Qty = Account_Weight_DC;
                            //        break;
                            //    case "总单毛重":
                            //        OBms_Bill_Ap_Dtl.Qty = Weight_DC;
                            //        break;
                            //    case "分单结算重量":
                            //        OBms_Bill_Ap_Dtl.Qty = Account_Weight_SK;
                            //        break;
                            //    case "AMS份数":
                            //        OBms_Bill_Ap_Dtl.Qty = AMS;
                            //        break;
                            //    case "报关票数":
                            //        if (item == "BGF" || item == "YDF")
                            //            ManyBGF = true;
                            //        break;
                            //    case "报关票数合计":
                            //        OBms_Bill_Ap_Dtl.Qty = SumNum_PG ?? 0;
                            //        break;
                            //    case "联单数合计":
                            //        OBms_Bill_Ap_Dtl.Qty = SumQTY ?? 0;
                            //        break;
                            //}

                            #endregion

                            #region 计算公式

                            switch (OCostMoney.CalcFormula)
                            {
                                case "A"://单价*计费单位
                                    OBms_Bill_Ap_Dtl.Account2 = OCostMoney.Price * OBms_Bill_Ap_Dtl.Qty;
                                    break;
                                case "B"://单价*1
                                    OBms_Bill_Ap_Dtl.Qty = 1;
                                    OBms_Bill_Ap_Dtl.Account2 = OCostMoney.Price * 1;
                                    break;
                                case "C"://单价*(计费单位-1)
                                    OBms_Bill_Ap_Dtl.Qty = OBms_Bill_Ap_Dtl.Qty - 1;
                                    OBms_Bill_Ap_Dtl.Account2 = OCostMoney.Price * OBms_Bill_Ap_Dtl.Qty;
                                    break;
                            }

                            #endregion

                            if (OCostMoney.FeeMin != null && OBms_Bill_Ap_Dtl.Account2 < OCostMoney.FeeMin)
                                OBms_Bill_Ap_Dtl.Account2 = OCostMoney.FeeMin.Value;
                            if (OCostMoney.FeeMax != null && OBms_Bill_Ap_Dtl.Account2 > OCostMoney.FeeMax)
                                OBms_Bill_Ap_Dtl.Account2 = OCostMoney.FeeMax.Value;

                            #region 价 税 价税合计

                            OBms_Bill_Ap_Dtl.Bill_HasTax = OBms_Bill_Ap.Bill_HasTax;
                            OBms_Bill_Ap_Dtl.Bill_TaxRate = OBms_Bill_Ap.Bill_TaxRate;

                            dynamic O_CalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ap.Bill_HasTax, OBms_Bill_Ap.Bill_TaxRate, OBms_Bill_Ap_Dtl.Account2);
                            if (O_CalcTaxRate.Success)
                            {
                                //价（实际金额）
                                OBms_Bill_Ap_Dtl.Bill_Amount = O_CalcTaxRate.Bill_Amount;
                                //税金 （实际金额 * 税率）
                                OBms_Bill_Ap_Dtl.Bill_TaxAmount = O_CalcTaxRate.Bill_TaxAmount;
                                //价税合计 (价+税金)
                                OBms_Bill_Ap_Dtl.Bill_AmountTaxTotal = O_CalcTaxRate.Bill_AmountTaxTotal;
                            }
                            else
                            {
                                Common.WriteLog_Local("AutoAddFee-计算价税合计出错", "CostMoneyService", true, true);
                            }

                            #endregion

                            if (ManyBGF)
                            {
                                #region 运抵费/报关费 生成 多条明细数据

                                foreach (var InDtlitem in ArrCusInspection)
                                {
                                    var OOCostMoney = QCostMoney.Where(x => x.CustomsType == InDtlitem.Customs_Declaration).FirstOrDefault();
                                    if (OOCostMoney != null && OOCostMoney.Id > 0)
                                    {
                                        var OOBms_Bill_Ap_Dtl = new Bms_Bill_Ap_Dtl();

                                        OOBms_Bill_Ap_Dtl.Id = --DelNum;
                                        OOBms_Bill_Ap_Dtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.AutoSet;
                                        OOBms_Bill_Ap_Dtl.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                                        OOBms_Bill_Ap_Dtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                                        OOBms_Bill_Ap_Dtl.Dzbh = OBms_Bill_Ap.Dzbh;
                                        OOBms_Bill_Ap_Dtl.Ops_M_OrdId = OBms_Bill_Ap.Ops_M_OrdId;
                                        OOBms_Bill_Ap_Dtl.Charge_Code = item;
                                        if (item == "YDF")
                                            OOBms_Bill_Ap_Dtl.Charge_Desc = "运抵费";
                                        else
                                            OOBms_Bill_Ap_Dtl.Charge_Desc = "报关费";
                                        OOBms_Bill_Ap_Dtl.Unitprice2 = OOCostMoney.Price;
                                        OOBms_Bill_Ap_Dtl.Summary = "";
                                        OOBms_Bill_Ap_Dtl.Bms_Bill_Ap_Id = OBms_Bill_Ap.Id;
                                        OOBms_Bill_Ap_Dtl.Line_No = OBms_Bill_Ap.Line_No;
                                        //var InDtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, OBms_Bill_Ap.Dzbh + "_" + OBms_Bill_Ap.Line_No, true);
                                        //OOBms_Bill_Ap_Dtl.Line_Id = Convert.ToInt32(InDtlLine_No);

                                        OOBms_Bill_Ap_Dtl.Qty = InDtlitem.Num_BG ?? 0;

                                        #region 计算公式

                                        switch (OOCostMoney.CalcFormula)
                                        {
                                            case "A"://单价*计费单位
                                                OOBms_Bill_Ap_Dtl.Account2 = OOCostMoney.Price * OOBms_Bill_Ap_Dtl.Qty;
                                                break;
                                            case "B"://单价*1
                                                OOBms_Bill_Ap_Dtl.Qty = 1;
                                                OOBms_Bill_Ap_Dtl.Account2 = OOCostMoney.Price * 1;
                                                break;
                                            case "C"://单价*(计费单位-1)
                                                OOBms_Bill_Ap_Dtl.Qty = OOBms_Bill_Ap_Dtl.Qty - 1;
                                                OOBms_Bill_Ap_Dtl.Account2 = OOCostMoney.Price * OOBms_Bill_Ap_Dtl.Qty;
                                                break;
                                        }

                                        #endregion

                                        if (OOCostMoney.FeeMin != null && OOBms_Bill_Ap_Dtl.Account2 < OOCostMoney.FeeMin)
                                            OOBms_Bill_Ap_Dtl.Account2 = OOCostMoney.FeeMin.Value;
                                        if (OOCostMoney.FeeMin != null && OOBms_Bill_Ap_Dtl.Account2 > OOCostMoney.FeeMax)
                                            OOBms_Bill_Ap_Dtl.Account2 = OOCostMoney.FeeMax.Value;

                                        #region 价 税 价税合计

                                        OOBms_Bill_Ap_Dtl.Bill_HasTax = OBms_Bill_Ap.Bill_HasTax;
                                        OOBms_Bill_Ap_Dtl.Bill_TaxRate = OBms_Bill_Ap.Bill_TaxRate;

                                        dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ap.Bill_HasTax, OBms_Bill_Ap.Bill_TaxRate, OOBms_Bill_Ap_Dtl.Account2);
                                        if (OCalcTaxRate.Success)
                                        {
                                            //价（实际金额）
                                            OOBms_Bill_Ap_Dtl.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                            //税金 （实际金额 * 税率）
                                            OOBms_Bill_Ap_Dtl.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                            //价税合计 (价+税金)
                                            OOBms_Bill_Ap_Dtl.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                        }
                                        else
                                        {
                                            Common.WriteLog_Local("AutoAddFee-计算价税合计出错", "CostMoneyService", true, true);
                                        }

                                        #endregion

                                        //设置新增状态
                                        var InentryDtl = WebdbContxt.Entry(OOBms_Bill_Ap_Dtl);
                                        OOBms_Bill_Ap_Dtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                        InentryDtl.State = EntityState.Added;
                                        ArrApDtl_Add.Add(OOBms_Bill_Ap_Dtl);//记录新增的明细数据
                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                //var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, OBms_Bill_Ap.Dzbh + "_" + OBms_Bill_Ap.Line_No, true);
                                //OBms_Bill_Ap_Dtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                //设置新增状态
                                var entryDtl = WebdbContxt.Entry(OBms_Bill_Ap_Dtl);
                                OBms_Bill_Ap_Dtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                entryDtl.State = EntityState.Added;
                                ArrApDtl_Add.Add(OBms_Bill_Ap_Dtl);//记录新增的明细数据
                            }

                            #endregion
                        }
                    }
                }
                #region 计算是否需要新增和序号

                foreach (var item in QBms_Bill_Ap)
                {
                    var ArrBms_Bill_Ap_Dtl = ArrApDtl_Add.Where(x => x.Bms_Bill_Ap_Id == item.Id);
                    if (item.ArrBms_Bill_Ap_Dtl != null && item.ArrBms_Bill_Ap_Dtl.Any())
                    {
                        ArrBms_Bill_Ap_Dtl = ArrBms_Bill_Ap_Dtl.Union(item.ArrBms_Bill_Ap_Dtl.Where(x => x.Id <= 0));
                    }
                    if (item.Id <= 0)
                    {
                        var Unchange = false;
                        if (!ArrBms_Bill_Ap_Dtl.Any())
                        {
                            item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Unchanged;
                            Unchange = true;
                        }
                        if (ArrBms_Bill_Ap_Dtl.Any())
                        {
                            if (!Unchange)
                            {
                                var Line_No = SequenceBuilder.NextBmsBillArLineNo(false, item.Dzbh);
                                item.Line_No = Convert.ToInt32(Line_No);
                            }
                            foreach (var itemDtl in ArrBms_Bill_Ap_Dtl)
                            {
                                if (Unchange)
                                {
                                    itemDtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Unchanged;
                                }
                                else
                                {
                                    itemDtl.Line_No = item.Line_No;
                                    var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, item.Dzbh + "_" + item.Line_No, true);
                                    itemDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ArrBms_Bill_Ap_Dtl != null && ArrBms_Bill_Ap_Dtl.Any())
                        {
                            foreach (var itemDtl in ArrBms_Bill_Ap_Dtl)
                            {
                                itemDtl.Line_No = item.Line_No;
                                var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(false, item.Dzbh + "_" + item.Line_No, true);
                                itemDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                            }
                        }
                    }
                    if (!ArrLineNo.Any(x => x.Item1 == item.Dzbh && x.Item2 == item.Line_No))
                        ArrLineNo.Add(new Tuple<string, int>(item.Dzbh, item.Line_No));//记录 应付-序号，重新合计费用时 使用
                }

                #endregion

                WebdbContxt.SaveChanges();

                #region 设置总价和价税合计

                int IsAr = 0;
                var DistArrLineNo = ArrLineNo.Distinct();
                foreach (var LineNo in DistArrLineNo)
                {
                    try
                    {
                        Oracle.ManagedDataAccess.Client.OracleParameter[] ArrParam = new Oracle.ManagedDataAccess.Client.OracleParameter[] { 
                            new  Oracle.ManagedDataAccess.Client.OracleParameter(":V_IsAr",IsAr),
                            new  Oracle.ManagedDataAccess.Client.OracleParameter(":V_LineNo",LineNo.Item2),
                            new  Oracle.ManagedDataAccess.Client.OracleParameter(":V_Dzbh",LineNo.Item1),
                        };
                        string SQLStr = @"begin
                        SetBmsArApATATT(V_IsAr => :V_IsAr,
                          V_Dzbh => :V_Dzbh,
                          V_LineNo => :V_LineNo);
                        end;";
                        //更新 价 税 价税合计
                        var ret = SQLDALHelper.OracleHelper.ExecuteNonQuery(CommandType.Text, SQLStr, ArrParam);
                    }
                    catch (Exception e)
                    {
                        var ErrMsg = "应付自动结算，刷新合计错误：" + Common.GetExceptionMsg(e);
                        Common.WriteLog_Local(ErrMsg, "ApAuto", true, true);
                    }
                }

                #endregion

                return retMsg;
            }
            catch (Exception ex)
            {
                var ErrMsg = "应付自动结算错误：" + Common.GetExceptionMsg(ex);
                Common.WriteLog_Local(ErrMsg, "ApAuto", true, true);
                return ErrMsg;
            }
        }
    }
}