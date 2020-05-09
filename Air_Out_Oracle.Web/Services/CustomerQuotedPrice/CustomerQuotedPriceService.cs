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
    public class CustomerQuotedPriceService : Service<CustomerQuotedPrice>, ICustomerQuotedPriceService
    {
        private readonly IRepositoryAsync<CustomerQuotedPrice> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        private readonly IBms_Bill_ArService _bms_Bill_ArService;

        //
        private RedisHelp.RedisHelper ORedisHelper { get; set; }

        public CustomerQuotedPriceService(IRepositoryAsync<CustomerQuotedPrice> repository, IDataTableImportMappingService mappingservice,
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
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "CustomerQuotedPrice").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                CustomerQuotedPrice item = new CustomerQuotedPrice();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type customerquotedpricetype = item.GetType();
                        PropertyInfo propertyInfo = customerquotedpricetype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type customerquotedpricetype = item.GetType();
                        PropertyInfo propertyInfo = customerquotedpricetype.GetProperty(field.FieldName);
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
            var customerquotedprice = this.Query(new CustomerQuotedPriceQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = customerquotedprice.Select(n => new
            {
                Id = n.Id,
                SerialNo = n.SerialNo,
                BusinessType = n.BusinessType,
                CustomerCode = n.CustomerCode,
                LocalWHMark = n.LocalWHMark,
                StartPlace = n.StartPlace,
                TransitPlace = n.TransitPlace,
                EndPlace = n.EndPlace,
                ProxyOperator = n.ProxyOperator,
                CusDefinition = n.CusDefinition,
                Receiver = n.Receiver,
                Shipper = n.Shipper,
                Contact = n.Contact,
                QuotedPricePolicy = n.QuotedPricePolicy,
                Seller = n.Seller,
                StartDate = n.StartDate,
                EndDate = n.EndDate,
                Description = n.Description,
                AuditStatus = n.AuditStatus,
                Status = n.Status,
                OperatingPoint = n.OperatingPoint,
                ADDWHO = n.ADDWHO,
                ADDTS = n.ADDTS,
                EDITWHO = n.EDITWHO,
                EDITTS = n.EDITTS
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(CustomerQuotedPrice), datarows);
        }

        /// <summary>
        /// 应付主单结算
        /// 主单 结算：应付 按订舱方找报价
        /// </summary>
        /// <param name="Ops_M_Id">主单Id</param>
        /// <param name="Pay"></param>
        /// <returns></returns>
        public string SettleMOrderPay(int Ops_M_Id)
        {
            string ErrMsg = "";
            var ArrOPS_M_Order = new List<OPS_M_Order>();
            var OPS_M_OrderRep = _repository.GetRepository<OPS_M_Order>();//主单
            //OPS_EntrustmentInfor OOPS_EntrustmentInfor = null;
            //var OPS_EntrustmentInforRep = _repository.GetRepository<OPS_EntrustmentInfor>();//委托
            //Warehouse_receipt OWH_Receipt = null;
            //var Warehouse_receiptRep = _repository.GetRepository<Warehouse_receipt>();//仓库

            #region 获取主单信息

            if (Ops_M_Id <= 0)
            {
                ErrMsg = "主单不存在";
            }
            else
            {
                var OOPS_M_Order = OPS_M_OrderRep.Queryable().Where(x => x.Id == Ops_M_Id && x.Status == AirOut.Web.Models.AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                if (OOPS_M_Order == null || OOPS_M_Order.Id <= 0)
                    ErrMsg = "找不到主单数据";
                else if (string.IsNullOrEmpty(OOPS_M_Order.MBL))
                {
                    ErrMsg = "必须有总单号，才可以结算";
                }
                else
                {
                    //OOPS_EntrustmentInfor = OPS_EntrustmentInforRep.Queryable().Where(x => x.MBLId == Ops_M_Id && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                    //if (OOPS_EntrustmentInfor == null || OOPS_EntrustmentInfor.Id <= 0)
                    //    ErrMsg = "数据不完整，找不到委托数据";
                    //OWH_Receipt = Warehouse_receiptRep.Queryable().Where(x => x.MBL == OOPS_M_Order.MBL && x.Status == AirOut.Web.Models.AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                    //if (OWH_Receipt == null || OWH_Receipt.Id <= 0)
                    //    ErrMsg = "数据不完整，找不到仓库接单数据";

                    //获取所有总单号相同的 主单
                    ArrOPS_M_Order = OPS_M_OrderRep.Queryable().Where(x => x.MBL == OOPS_M_Order.MBL && x.Status == AirOut.Web.Models.AirOutEnumType.UseStatusIsOrNoEnum.Enable).ToList();
                }
            }

            #endregion

            //验证数据格式
            if (string.IsNullOrEmpty(ErrMsg))
            {
                //客户报价
                List<CustomerQuotedPrice> ArrCusQuotedPrice = (List<CustomerQuotedPrice>)CacheHelper.Get_SetCache(Common.CacheNameS.CustomerQuotedPrice);
                #region 寻找报价

                var QArrCusQuotedPrice = ArrCusQuotedPrice.Where(x =>
                    x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess &&
                    x.Status == AirOutEnumType.UseStatusEnum.Enable);
                if (ArrOPS_M_Order.Any())
                {
                    var ArrAirways_Code = ArrOPS_M_Order.Select(x => x.Airways_Code);//订舱方
                    //找出 所有应付 客户代码为订舱方的报价
                    QArrCusQuotedPrice = QArrCusQuotedPrice.Where(x => ArrAirways_Code.Contains(x.CustomerCode));
                    foreach (var OOPS_M_Order in ArrOPS_M_Order)
                    {
                        //应付客户（航空公司）
                        var ArrPayCustomer = new Dictionary<string, string>(){
                            {"空运费",OOPS_M_Order.Airways_Code },//计费重量（KG）
                            {"制单费",OOPS_M_Order.Airways_Code },//票
                            {"仓储费","仓库接单地面代理" }//计费重量（KG）
                        };
                    }
                }
                else
                    ErrMsg = "找不到主单号信息";

                #endregion
            }

            return ErrMsg;
        }

        /// <summary>
        /// 应收分单结算
        /// </summary>
        /// <param name="Ops_H_Id"></param>
        /// <returns></returns>
        public string SettleHOrderRec(int Ops_H_Id)
        {
            string ErrMsg = "";
            var OPS_H_OrderRep = _repository.GetRepository<OPS_H_Order>();//分单
            var OPS_M_OrderRep = _repository.GetRepository<OPS_M_Order>();//主单
            OPS_H_Order OOPS_H_Order = null;
            OPS_M_Order OOPS_M_Order = null;

            #region 获取分单信息

            if (Ops_H_Id <= 0)
            {
                ErrMsg = "分单不存在";
            }
            else
            {
                OOPS_H_Order = OPS_H_OrderRep.Queryable().Where(x => x.Id == Ops_H_Id && x.Status == AirOut.Web.Models.AirOutEnumType.UseStatusEnum.Enable).FirstOrDefault();
                if (OOPS_H_Order == null || OOPS_H_Order.Id <= 0)
                    ErrMsg = "找不到分单数据";
                else
                {
                    OOPS_M_Order = OPS_M_OrderRep.Queryable().Where(x => x.Id == OOPS_H_Order.MBLId && x.Status == AirOut.Web.Models.AirOutEnumType.UseStatusIsOrNoEnum.Enable).FirstOrDefault();
                    if (OOPS_M_Order == null || OOPS_M_Order.Id <= 0)
                        ErrMsg = "找不到主单数据";
                }
            }

            #endregion

            //验证数据格式
            if (string.IsNullOrEmpty(ErrMsg))
            {
                //客户报价
                List<CustomerQuotedPrice> ArrCusQuotedPrice = (List<CustomerQuotedPrice>)CacheHelper.Get_SetCache(Common.CacheNameS.CustomerQuotedPrice);
                #region 寻找报价

                var QArrCusQuotedPrice = ArrCusQuotedPrice.Where(x =>
                    x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess &&
                    x.Status == AirOutEnumType.UseStatusEnum.Enable);
                //应付客户（航空公司）
                var ArrPayCustomer = new Dictionary<string, string>(){ 
                    {"报关费", OOPS_M_Order.Airways_Code },//票
                    {"商检费", OOPS_M_Order.Airways_Code },//票，商检标志
                };

                #endregion
            }

            return ErrMsg;
        }

        /// <summary>
        /// 应收自动结算
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
                    "KYF","RYF","ZZF","KCF","ZDF","FDF","YSF","ZF"//订舱主单-结算重量
                };
                if (ArrFeeType == null || !ArrFeeType.Any())
                {
                    ArrFeeType = new List<string> { 
                        "XXF","CCF","BGF","YDF","LDF"
                    };
                    ArrFeeType = ArrJSZL.Concat(ArrFeeType);
                }
                //反向从依赖注入中取上下文dbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;

                var ArrArDtl_Add = new List<Bms_Bill_Ar_Dtl>();//记录新增的明细

                #region 验证应付账单表头，是否已存在

                var BmsArRep = _repository.GetRepository<Bms_Bill_Ar>();
                var BmsArDtlRep = _repository.GetRepository<Bms_Bill_Ar_Dtl>();
                Bms_Bill_Ar OBms_Bill_Ar;
                var QBms_Bill_Ar = BmsArRep.Queryable().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Ops_M_OrdId == OOPS_M_Order.Id).ToList().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && !x.Cancel_Status).OrderByDescending(x => x.Id).ToList();
                var QBms_Bill_ArDtl = BmsArDtlRep.Queryable().Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Ops_M_OrdId == OOPS_M_Order.Id).ToList();
                if (QBms_Bill_Ar.Any(x=>x.EDITTS.HasValue) || QBms_Bill_ArDtl.Any(x =>  x.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet && x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess))
                {
                    retMsg = "应收账单含有已审核/修改的自动账单";//错误信息
                    return retMsg;
                }
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
                var Dzbh = OOPS_EntrustmentInfor.Operation_Id;//应收-作业单号
                var ArrLineNo = new List<Tuple<string, int>>();//应收-序号 刷新表头 合计时使用
                //--------------------------------- 结算用到值 ---------------------------------------
                var Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want;//航班日期
                var Entrustment_Code = OOPS_EntrustmentInfor.Entrustment_Name;//委托方
                var Carriage_Account_Code = OOPS_EntrustmentInfor.Carriage_Account_Code;//结算方
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

                if (!QBms_Bill_Ar.Any())
                {
                    #region 创建应付表头数据

                    //OBms_Bill_Ar = new Bms_Bill_Ar();

                    //var entry = WebdbContxt.Entry(OBms_Bill_Ar);
                    //OBms_Bill_Ar.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                    //entry.State = EntityState.Added;

                    //OBms_Bill_Ar.Ops_M_OrdId = OOPS_M_Order.Id;
                    //OBms_Bill_Ar.Dzbh = OOPS_EntrustmentInfor.Operation_Id;
                    //OBms_Bill_Ar.MBL = OOPS_EntrustmentInfor.MBL;
                    //OBms_Bill_Ar.Bill_Date = DateTime.Now;
                    //OBms_Bill_Ar.Status = AirOutEnumType.UseStatusEnum.Enable;
                    //OBms_Bill_Ar.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                    //OBms_Bill_Ar.Create_Status = AirOutEnumType.Bms_BillCreate_Status.AutoSet;
                    //OBms_Bill_Ar.Payway = "YJZZ";
                    //OBms_Bill_Ar.Money_Code = "CNY";
                    //OBms_Bill_Ar.Org_Money_Code = "CNY";
                    //OBms_Bill_Ar.Bill_Type = "FP";
                    //OBms_Bill_Ar.Bill_TaxRateType = "X0";
                    //OBms_Bill_Ar.Bill_TaxRate = 0;
                    //OBms_Bill_Ar.Bill_HasTax = false;
                    //OBms_Bill_Ar.Bill_Object_Id = Entrustment_Code;
                    //if (!string.IsNullOrEmpty(OBms_Bill_Ar.Bill_Object_Id))
                    //{
                    //    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OBms_Bill_Ar.Bill_Object_Id);
                    //    if (QArrCusBusInfo.Any())
                    //    {
                    //        OBms_Bill_Ar.Bill_Object_Name = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//结算方 = 主单收货人
                    //    }
                    //}

                    #endregion
                }
                else
                {
                    var QAutoAr = QBms_Bill_Ar.Where(x => x.ArrBms_Bill_Ar_Dtl.Any(n => n.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet));
                    if (QAutoAr.Any())
                    {
                        OBms_Bill_Ar = QAutoAr.FirstOrDefault();
                    }
                    else
                        OBms_Bill_Ar = QBms_Bill_Ar.FirstOrDefault();
                }

                #endregion

                #region 找报价（不匹配区间条件）

                //取应付报价-缓存
                var ArrCusQuotedPrice = (IEnumerable<CustomerQuotedPrice>)CacheHelper.Get_SetCache(Common.CacheNameS.CustomerQuotedPrice);
                ArrCusQuotedPrice = ArrCusQuotedPrice.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.AuditStatus == AirOutEnumType.AuditStatusEnum.AuditSuccess).ToList();
                var QArrCusQuotedPrice = ArrCusQuotedPrice.Where(x => x.StartDate <= Flight_Date_Want && x.EndDate > Flight_Date_Want);
                if (!QArrCusQuotedPrice.Any())//航班日期
                {
                    retMsg = "找不到应付报价-航班日期";//错误信息
                    return retMsg;
                }
                QArrCusQuotedPrice = QArrCusQuotedPrice.Where(x => x.CustomerCode == Entrustment_Code);
                if (!QArrCusQuotedPrice.Any())//目的地 委托方
                {
                    retMsg = "找不到应收报价-委托方";//错误信息
                    return retMsg;
                }
                if (QArrCusQuotedPrice.Any(x => x.EndPlace == EndPlace))//目的地 委托方
                    QArrCusQuotedPrice = QArrCusQuotedPrice.Where(x => x.EndPlace == EndPlace);
                else
                    QArrCusQuotedPrice = QArrCusQuotedPrice.Where(x => string.IsNullOrWhiteSpace(x.EndPlace));

                #endregion

                if (!QArrCusQuotedPrice.Any())
                {
                    retMsg = "找不到应收报价";//错误信息
                    return retMsg;
                }
                else
                {
                    var IdNum = 0;//应收 账单Id
                    var CusQuoPriceDtlRep = _repository.GetRepository<CusQuotedPriceDtl>();
                    var CusQPId = QArrCusQuotedPrice.OrderByDescending(x => x.Id).FirstOrDefault().Id;
                    var ArrCusQuotedPriceDtl = CusQuoPriceDtlRep.Queryable().Where(x => x.CusQPId == CusQPId).ToList();
                    foreach (var item in ArrFeeType)
                    {
                        CusQuotedPriceDtl OCusQPDtl = null;

                        #region 删除自动结算明细

                        //获取 费用明细
                        IEnumerable<Bms_Bill_Ar_Dtl> QBms_Bill_Ar_Dtl = QBms_Bill_ArDtl.Where(x => x.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet && x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess && x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Charge_Code == item).ToList();

                        if (QBms_Bill_Ar_Dtl.Any())
                        {
                            var DelDate = DateTime.Now.ToString("yyyyMMdd");
                            var DelDateTime = DateTime.Now.ToString("yyyyMMdd HHmmss");
                            foreach (var itemDtl in QBms_Bill_Ar_Dtl)
                            {
                                if (!ArrLineNo.Any(x => x.Item1 == itemDtl.Dzbh && x.Item2 == itemDtl.Line_No))
                                    ArrLineNo.Add(new Tuple<string, int>(itemDtl.Dzbh, itemDtl.Line_No));//记录 应收-序号，重新合计费用时 使用
                                //设置删除状态
                                var i_entryDtl = WebdbContxt.Entry(itemDtl);
                                itemDtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Deleted;
                                i_entryDtl.State = EntityState.Deleted;
                                ORedisHelper.ListRightPush(Common.RedisKeyDelArAp_Dtl + ":" + DelDate, "Bms_Bill_ArApDTL@|@" + itemDtl.Id.ToString() + "@|@1@|@" + DelDateTime);
                            }
                        }

                        #endregion

                        #region 获取报价区间匹配 判断报价

                        var QArrCusQPDtl = ArrCusQuotedPriceDtl.Where(x => x.FeeCode == item);
                        if (!QArrCusQPDtl.Any())
                            continue;
                        switch (item)
                        {
                            case "XXF"://【AMS份数】有值
                                if (AMS <= 0)
                                    continue;
                                break;
                            case "CCF"://勾选KSF  且  交货点有值（匹配对应）
                                //if (KSF == true && !string.IsNullOrWhiteSpace(Delivery_Point))
                                //{
                                //    QArrCusQPDtl = QArrCusQPDtl.Where(x => x.Is_Book_Flat != null && x.Is_Book_Flat == true && x.DeliveryPoint == Delivery_Point);
                                //    if (!QArrCusQPDtl.Any())
                                //    continue;
                                //}
                                //else
                                //    continue;
                                break;
                            case "LDF"://联单费//【单证管理-联单数】有值
                                //单证管理-联单数合计变更保存
                                if (SumQTY == null || SumQTY <= 0)
                                    continue;
                                break;
                            case "YDF"://运抵费//【报关方式】有值
                            case "BGF"://报关费//【报关方式】有值
                                //一票KSF业务下：会有一个报关费账单，含多条费用明细
                                if (!ArrCusInspection.Any())
                                    continue;
                                break;
                        }
                        var NewQCusQPDtl = QArrCusQPDtl.Where(x => !string.IsNullOrEmpty(x.FeeCondition) && x.FeeCondition != "-");
                        if (NewQCusQPDtl.Any())
                        {
                            NewQCusQPDtl = NewQCusQPDtl.Where(x => !string.IsNullOrEmpty(x.CalcSign1) || !string.IsNullOrEmpty(x.CalcSign2));
                            var NewArrCusQPDtl = new List<CusQuotedPriceDtl>();//零时记录报价

                            #region 满足 区间 报价

                            foreach (var CMitem in NewQCusQPDtl)
                            {
                                decimal CalcVal = 0;

                                #region 获取 计算单位 值

                                //计费单位
                                string BillingUnitName = "";
                                var QArrFeeUnit = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "FeeConditionAr" && x.LISTCODE == CMitem.FeeCondition);
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
                                                NewArrCusQPDtl.Add(CMitem);
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
                                                NewArrCusQPDtl.Add(CMitem);
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
                                                NewArrCusQPDtl.Add(CMitem);
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
                                                NewArrCusQPDtl.Add(CMitem);
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
                                                NewArrCusQPDtl.Add(CMitem);
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
                                                NewArrCusQPDtl.Add(CMitem);
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
                                            NewArrCusQPDtl.Add(CMitem);
                                            continue;
                                        }
                                        break;
                                    case ">=":
                                        if (CMitem.FeeConditionVal2 <= CalcVal)
                                        {
                                            NewArrCusQPDtl.Add(CMitem);
                                            continue;
                                        }
                                        break;
                                    case "<":
                                        if (CMitem.FeeConditionVal2 > CalcVal)
                                        {
                                            NewArrCusQPDtl.Add(CMitem);
                                            continue;
                                        }
                                        break;
                                    case "<=":
                                        if (CMitem.FeeConditionVal2 >= CalcVal)
                                        {
                                            NewArrCusQPDtl.Add(CMitem);
                                            continue;
                                        }
                                        break;
                                    case "==":
                                        if (CMitem.FeeConditionVal2 == CalcVal)
                                        {
                                            NewArrCusQPDtl.Add(CMitem);
                                            continue;
                                        }
                                        break;
                                    case "!=":
                                        if (CMitem.FeeConditionVal2 != CalcVal)
                                        {
                                            NewArrCusQPDtl.Add(CMitem);
                                            continue;
                                        }
                                        break;
                                }

                                #endregion
                            }

                            #endregion

                            if (NewArrCusQPDtl.Any())
                            {
                                OCusQPDtl = NewArrCusQPDtl.OrderByDescending(x => x.Id).FirstOrDefault();
                            }
                            else
                            {
                                var WQArrQPDtl = QArrCusQPDtl.Where(x => string.IsNullOrEmpty(x.FeeCondition) || x.FeeCondition == "-");
                                if (WQArrQPDtl.Any())
                                    OCusQPDtl = WQArrQPDtl.FirstOrDefault();
                                else
                                    OCusQPDtl = new CusQuotedPriceDtl();//不匹配任何报价
                            }
                        }

                        #endregion

                        if (OCusQPDtl == null)
                            OCusQPDtl = QArrCusQPDtl.OrderByDescending(x => x.Id).FirstOrDefault();

                        if (OCusQPDtl != null && OCusQPDtl.Id > 0)
                        {
                            #region 计费单位

                            string BillingUnitName = "";
                            int BillingUnit = 0;
                            int.TryParse(OCusQPDtl.BillingUnit, out BillingUnit);
                            var QArrFeeUnit = ArrFeeUnit.Where(x => x.Id == BillingUnit);
                            if (QArrFeeUnit.Any())
                            {
                                BillingUnitName = QArrFeeUnit.FirstOrDefault().FeeUnitName;
                            }
                            bool ManyBGF = false;//BGF 按多个生产
                            decimal BillingUnitVal = 0;//计费单位-值
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

                            switch (OCusQPDtl.CalcFormula)
                            {
                                case "A"://单价*计费单位
                                    break;
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

                            var QQBms_Bill_Ar = QBms_Bill_Ar.Where(x => x.Bill_Object_Id == Entrustment_Code && x.Money_Code == OCusQPDtl.CurrencyCode);
                            if (QQBms_Bill_Ar.Any())
                            {
                                var WQQBms_Bill_Ar = QQBms_Bill_Ar.Where(x => x.Create_Status == AirOutEnumType.Bms_BillCreate_Status.AutoSet);
                                if (WQQBms_Bill_Ar.Any())
                                    OBms_Bill_Ar = WQQBms_Bill_Ar.FirstOrDefault();
                                else
                                    OBms_Bill_Ar = QQBms_Bill_Ar.FirstOrDefault();
                            }
                            else
                            {
                                #region 创建应收表头数据

                                OBms_Bill_Ar = new Bms_Bill_Ar();

                                var entry = WebdbContxt.Entry(OBms_Bill_Ar);
                                OBms_Bill_Ar.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                entry.State = EntityState.Added;

                                OBms_Bill_Ar.Id = --IdNum;
                                OBms_Bill_Ar.Ops_M_OrdId = OOPS_M_Order.Id;
                                OBms_Bill_Ar.Dzbh = OOPS_EntrustmentInfor.Operation_Id;
                                OBms_Bill_Ar.MBL = string.IsNullOrWhiteSpace(OOPS_EntrustmentInfor.MBL) ? "-" : OOPS_EntrustmentInfor.MBL;
                                OBms_Bill_Ar.Bill_Date = DateTime.Now;
                                OBms_Bill_Ar.Status = AirOutEnumType.UseStatusEnum.Enable;
                                OBms_Bill_Ar.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                                OBms_Bill_Ar.Create_Status = AirOutEnumType.Bms_BillCreate_Status.AutoSet;
                                OBms_Bill_Ar.Payway = "YJZZ";
                                OBms_Bill_Ar.Money_Code = OCusQPDtl.CurrencyCode;
                                OBms_Bill_Ar.Org_Money_Code = OCusQPDtl.CurrencyCode;
                                OBms_Bill_Ar.Bill_Type = "FP";
                                OBms_Bill_Ar.Bill_TaxRateType = "X0";
                                OBms_Bill_Ar.Bill_TaxRate = 0;
                                OBms_Bill_Ar.Bill_HasTax = false;
                                OBms_Bill_Ar.Bill_Object_Id = Carriage_Account_Code;//20190203-由委托方改成结算方
                                if (!string.IsNullOrEmpty(OBms_Bill_Ar.Bill_Object_Id))
                                {
                                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OBms_Bill_Ar.Bill_Object_Id);
                                    if (QArrCusBusInfo.Any())
                                    {
                                        OBms_Bill_Ar.Bill_Object_Name = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;//结算方 = 主单收货人
                                    }
                                }

                                #endregion

                                QBms_Bill_Ar.Add(OBms_Bill_Ar);
                            }

                            //if (OBms_Bill_Ar.Id <= 0 && OBms_Bill_Ar.Line_No <= 0)
                            //{
                            //    var Line_No = SequenceBuilder.NextBmsBillArLineNo(true, OBms_Bill_Ar.Dzbh);
                            //    OBms_Bill_Ar.Line_No = Convert.ToInt32(Line_No);
                            //}
                            //if (!ArrLineNo.Any(x => x == OBms_Bill_Ar.Line_No))
                            //    ArrLineNo.Add(OBms_Bill_Ar.Line_No);//记录 应收-序号，重新合计费用时 使用

                            #region 新增明细费用

                            Bms_Bill_Ar_Dtl OBms_Bill_Ar_Dtl = new Bms_Bill_Ar_Dtl();

                            OBms_Bill_Ar_Dtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.AutoSet;
                            OBms_Bill_Ar_Dtl.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                            OBms_Bill_Ar_Dtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                            OBms_Bill_Ar_Dtl.Dzbh = OBms_Bill_Ar.Dzbh;
                            OBms_Bill_Ar_Dtl.Ops_M_OrdId = OBms_Bill_Ar.Ops_M_OrdId;
                            OBms_Bill_Ar_Dtl.Charge_Code = item;
                            OBms_Bill_Ar_Dtl.Unitprice2 = OCusQPDtl.Price;
                            OBms_Bill_Ar_Dtl.Summary = "";
                            OBms_Bill_Ar_Dtl.Bms_Bill_Ar_Id = OBms_Bill_Ar.Id;
                            OBms_Bill_Ar_Dtl.Line_No = OBms_Bill_Ar.Line_No;

                            #region 读取费用名称

                            var QArrFeeTypes = ArrFeeTypes.Where(x => x.FeeCode == item);
                            if (QArrFeeTypes.Any())
                            {
                                OBms_Bill_Ar_Dtl.Charge_Desc = QArrFeeTypes.FirstOrDefault().FeeName;
                            }
                            if (string.IsNullOrWhiteSpace(OBms_Bill_Ar_Dtl.Charge_Desc))
                            {
                                return "费用名称无法读取";
                            }

                            #endregion

                            #region 计费单位

                            if (!ManyBGF)
                                OBms_Bill_Ar_Dtl.Qty = BillingUnitVal;
                            //string BillingUnitName = "";
                            //int BillingUnit = 0;
                            //int.TryParse(OCusQPDtl.BillingUnit, out BillingUnit);
                            //var QArrFeeUnit = ArrFeeUnit.Where(x => x.Id == BillingUnit);
                            //if (QArrFeeUnit.Any())
                            //{
                            //    BillingUnitName = QArrFeeUnit.FirstOrDefault().FeeUnitName;
                            //}
                            //bool ManyBGF = false;//BGF 按多个生产
                            //switch (BillingUnitName)
                            //{
                            //    case "总单结算重量":
                            //        OBms_Bill_Ar_Dtl.Qty = Account_Weight_DC;
                            //        break;
                            //    case "总单毛重":
                            //        OBms_Bill_Ar_Dtl.Qty = Weight_DC;
                            //        break;
                            //    case "分单结算重量":
                            //        OBms_Bill_Ar_Dtl.Qty = Account_Weight_SK;
                            //        break;
                            //    case "AMS份数":
                            //        OBms_Bill_Ar_Dtl.Qty = AMS;
                            //        break;
                            //    case "报关票数":
                            //        if (item == "BGF" || item == "YDF")
                            //            ManyBGF = true;
                            //        break;
                            //    case "报关票数合计":
                            //        OBms_Bill_Ar_Dtl.Qty = SumNum_PG ?? 0;
                            //        break;
                            //    case "联单数合计":
                            //        OBms_Bill_Ar_Dtl.Qty = SumQTY ?? 0;
                            //        break;
                            //}

                            #endregion

                            #region 计算公式

                            switch (OCusQPDtl.CalcFormula)
                            {
                                case "A"://单价*计费单位
                                    OBms_Bill_Ar_Dtl.Account2 = OCusQPDtl.Price * OBms_Bill_Ar_Dtl.Qty;
                                    break;
                                case "B"://单价*1
                                    OBms_Bill_Ar_Dtl.Qty = 1;
                                    OBms_Bill_Ar_Dtl.Account2 = OCusQPDtl.Price * 1;
                                    break;
                                case "C"://单价*(计费单位-1)
                                    OBms_Bill_Ar_Dtl.Qty = OBms_Bill_Ar_Dtl.Qty - 1;
                                    OBms_Bill_Ar_Dtl.Account2 = OCusQPDtl.Price * OBms_Bill_Ar_Dtl.Qty;
                                    break;
                            }

                            #endregion

                            if (OCusQPDtl.FeeMin != null && OBms_Bill_Ar_Dtl.Account2 < OCusQPDtl.FeeMin)
                                OBms_Bill_Ar_Dtl.Account2 = OCusQPDtl.FeeMin.Value;
                            if (OCusQPDtl.FeeMax != null && OBms_Bill_Ar_Dtl.Account2 > OCusQPDtl.FeeMax)
                                OBms_Bill_Ar_Dtl.Account2 = OCusQPDtl.FeeMax.Value;

                            #region 计算 价 税 价税合计

                            OBms_Bill_Ar_Dtl.Bill_HasTax = OBms_Bill_Ar.Bill_HasTax;
                            OBms_Bill_Ar_Dtl.Bill_TaxRate = OBms_Bill_Ar.Bill_TaxRate;

                            var O_CalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ar.Bill_HasTax, OBms_Bill_Ar.Bill_TaxRate, OBms_Bill_Ar_Dtl.Account2);
                            if (O_CalcTaxRate.Success)
                            {
                                //价（实际金额）
                                OBms_Bill_Ar_Dtl.Bill_Amount = O_CalcTaxRate.Bill_Amount;
                                //税金 （实际金额 * 税率）
                                OBms_Bill_Ar_Dtl.Bill_TaxAmount = O_CalcTaxRate.Bill_TaxAmount;
                                //价税合计 (价+税金)
                                OBms_Bill_Ar_Dtl.Bill_AmountTaxTotal = O_CalcTaxRate.Bill_AmountTaxTotal;
                            }
                            else
                            {
                                Common.WriteLog_Local("AutoAddFee-计算价税合计出错", "CusQPService", true, true);
                            }

                            #endregion

                            if (ManyBGF)
                            {
                                #region 运抵费/报关费 生成 多条明细数据

                                foreach (var InDtlitem in ArrCusInspection)
                                {
                                    var OOCusQPDtl = QArrCusQPDtl.Where(x => x.CustomsType == InDtlitem.Customs_Declaration).FirstOrDefault();
                                    if (OOCusQPDtl != null && OOCusQPDtl.Id > 0)
                                    {
                                        var OOBms_Bill_Ar_Dtl = new Bms_Bill_Ar_Dtl();

                                        OOBms_Bill_Ar_Dtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.AutoSet;
                                        OOBms_Bill_Ar_Dtl.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                                        OOBms_Bill_Ar_Dtl.Status = AirOutEnumType.UseStatusEnum.Enable;
                                        OOBms_Bill_Ar_Dtl.Dzbh = OBms_Bill_Ar.Dzbh;
                                        OOBms_Bill_Ar_Dtl.Ops_M_OrdId = OBms_Bill_Ar.Ops_M_OrdId;
                                        OOBms_Bill_Ar_Dtl.Charge_Code = item;
                                        if (item == "YDF")
                                            OOBms_Bill_Ar_Dtl.Charge_Desc = "运抵费";
                                        else
                                            OOBms_Bill_Ar_Dtl.Charge_Desc = "报关费";
                                        OOBms_Bill_Ar_Dtl.Unitprice2 = OOCusQPDtl.Price;
                                        OOBms_Bill_Ar_Dtl.Summary = "";
                                        OOBms_Bill_Ar_Dtl.Bms_Bill_Ar_Id = OBms_Bill_Ar.Id;
                                        OOBms_Bill_Ar_Dtl.Line_No = OBms_Bill_Ar.Line_No;
                                        //var InDtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, OBms_Bill_Ar.Dzbh + "_" + OBms_Bill_Ar.Line_No, true);
                                        //OOBms_Bill_Ar_Dtl.Line_Id = Convert.ToInt32(InDtlLine_No);

                                        OOBms_Bill_Ar_Dtl.Qty = InDtlitem.Num_BG ?? 0;

                                        #region 计算公式

                                        switch (OOCusQPDtl.CalcFormula)
                                        {
                                            case "A"://单价*计费单位
                                                OOBms_Bill_Ar_Dtl.Account2 = OOCusQPDtl.Price * OOBms_Bill_Ar_Dtl.Qty;
                                                break;
                                            case "B"://单价*1
                                                OOBms_Bill_Ar_Dtl.Qty = 1;
                                                OOBms_Bill_Ar_Dtl.Account2 = OOCusQPDtl.Price * 1;
                                                break;
                                            case "C"://单价*(计费单位-1)
                                                OOBms_Bill_Ar_Dtl.Qty = OOBms_Bill_Ar_Dtl.Qty - 1;
                                                OOBms_Bill_Ar_Dtl.Account2 = OOCusQPDtl.Price * OOBms_Bill_Ar_Dtl.Qty;
                                                break;
                                        }

                                        #endregion

                                        if (OOCusQPDtl.FeeMin != null && OOBms_Bill_Ar_Dtl.Account2 < OOCusQPDtl.FeeMin)
                                            OOBms_Bill_Ar_Dtl.Account2 = OOCusQPDtl.FeeMin.Value;
                                        if (OOCusQPDtl.FeeMin != null && OOBms_Bill_Ar_Dtl.Account2 > OOCusQPDtl.FeeMax)
                                            OOBms_Bill_Ar_Dtl.Account2 = OOCusQPDtl.FeeMax.Value;

                                        OOBms_Bill_Ar_Dtl.Bill_HasTax = OBms_Bill_Ar.Bill_HasTax;
                                        OOBms_Bill_Ar_Dtl.Bill_TaxRate = OBms_Bill_Ar.Bill_TaxRate;

                                        #region 价 税 价税合计

                                        dynamic OCalcTaxRate = _bms_Bill_ArService.CalcTaxRate(OBms_Bill_Ar.Bill_HasTax, OBms_Bill_Ar.Bill_TaxRate, OOBms_Bill_Ar_Dtl.Account2);
                                        if (OCalcTaxRate.Success)
                                        {
                                            //价（实际金额）
                                            OOBms_Bill_Ar_Dtl.Bill_Amount = OCalcTaxRate.Bill_Amount;
                                            //税金 （实际金额 * 税率）
                                            OOBms_Bill_Ar_Dtl.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                                            //价税合计 (价+税金)
                                            OOBms_Bill_Ar_Dtl.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                                        }
                                        else
                                        {
                                            Common.WriteLog_Local("AutoAddFee-计算价税合计出错", "CusQPService", true, true);
                                        }

                                        #endregion

                                        ////设置新增状态
                                        //var InentryDtl = WebdbContxt.Entry(OOBms_Bill_Ar_Dtl);
                                        //OOBms_Bill_Ar_Dtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                        //InentryDtl.State = EntityState.Added;
                                        ArrArDtl_Add.Add(OOBms_Bill_Ar_Dtl);//记录新增的明细数据
                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                //var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, OBms_Bill_Ar.Dzbh + "_" + OBms_Bill_Ar.Line_No, true);
                                //OBms_Bill_Ar_Dtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                ////设置新增状态
                                //var entryDtl = WebdbContxt.Entry(OBms_Bill_Ar_Dtl);
                                //OBms_Bill_Ar_Dtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                //entryDtl.State = EntityState.Added;
                                ArrArDtl_Add.Add(OBms_Bill_Ar_Dtl);//记录新增的明细数据
                            }

                            #endregion
                        }
                    }
                }
                #region 计算是否需要新增和序号

                foreach (var item in QBms_Bill_Ar)
                {
                    var DtlNum = 0;//明细数据零时自增Id
                    var ArrBms_Bill_Ar_Dtl = ArrArDtl_Add.Where(x => x.Bms_Bill_Ar_Id == item.Id);
                    if (item.ArrBms_Bill_Ar_Dtl != null && item.ArrBms_Bill_Ar_Dtl.Any())
                    {
                        ArrBms_Bill_Ar_Dtl = ArrBms_Bill_Ar_Dtl.Union(item.ArrBms_Bill_Ar_Dtl);
                    }
                    if (item.Id <= 0)
                    {
                        var Unchange = false;
                        if (!ArrBms_Bill_Ar_Dtl.Any())
                        {
                            item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Unchanged;
                            Unchange = true;
                        }
                        if (ArrBms_Bill_Ar_Dtl.Any())
                        {
                            if (!Unchange)
                            {
                                var Line_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh);
                                item.Line_No = Convert.ToInt32(Line_No);
                            }
                            foreach (var itemDtl in ArrBms_Bill_Ar_Dtl)
                            {
                                if (Unchange)
                                {
                                    //itemDtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Unchanged;
                                }
                                else
                                {
                                    if (itemDtl.Id == 0)
                                    {
                                        itemDtl.Id = --DtlNum;
                                        itemDtl.Line_No = item.Line_No;
                                        var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh + "_" + item.Line_No, true);
                                        itemDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                        //设置新增状态
                                        var entryDtl = WebdbContxt.Entry(itemDtl);
                                        itemDtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                        entryDtl.State = EntityState.Added;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ArrBms_Bill_Ar_Dtl != null && ArrBms_Bill_Ar_Dtl.Any())
                        {
                            foreach (var itemDtl in ArrBms_Bill_Ar_Dtl)
                            {
                                if (itemDtl.Id == 0)
                                {
                                    itemDtl.Id = --DtlNum;
                                    itemDtl.Line_No = item.Line_No;
                                    var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh + "_" + item.Line_No, true);
                                    itemDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                    //设置新增状态
                                    var entryDtl = WebdbContxt.Entry(itemDtl);
                                    itemDtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                    entryDtl.State = EntityState.Added;
                                }
                            }
                        }
                    }
                    if (!ArrLineNo.Any(x => x.Item1 == item.Dzbh && x.Item2 == item.Line_No))
                        ArrLineNo.Add(new Tuple<string, int>(item.Dzbh, item.Line_No));//记录 应收-序号，重新合计费用时 使用
                }

                #endregion

                WebdbContxt.SaveChanges();

                #region 设置总价和价税合计

                int IsAr = 1;
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
                        var ErrMsg = "应收自动结算，刷新合计错误：" + Common.GetExceptionMsg(e);
                        Common.WriteLog_Local(ErrMsg, "ApAuto", true, true);
                    }
                }

                #endregion

                return retMsg;
            }
            catch (Exception ex)
            {
                var ErrMsg = "应收自动结算错误：" + Common.GetExceptionMsg(ex);
                Common.WriteLog_Local(ErrMsg, "ArAuto", true, true);
                return ErrMsg;
            }
        }
    }
}