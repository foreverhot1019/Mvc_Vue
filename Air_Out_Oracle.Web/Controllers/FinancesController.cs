using AirOut.Web.Extensions;
using AirOut.Web.Models;
using AirOut.Web.Repositories;
using AirOut.Web.Services;
using Newtonsoft.Json;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AirOut.Web.Controllers
{
    public class FinancesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Bms_Bill_Ap>, Repository<Bms_Bill_Ap>>();
        //container.RegisterType<IBms_Bill_ApService, Bms_Bill_ApService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IBms_Bill_ApService bms_Bill_ApService;
        private readonly IBms_Bill_ArService bms_Bill_ArService;
        private readonly IFinanceService FinanceService;
        private readonly IUnitOfWorkAsync unitOfWork;
        private readonly IOPS_EntrustmentInforService _oPS_EntrustmentInforService;
        //验证权限的名称
        private string ControllerQXName = "/Finances";

        public FinancesController(IBms_Bill_ApService _bms_Bill_ApService,
            IBms_Bill_ArService _bms_Bill_ArService,
            IFinanceService _financeservice,
            IUnitOfWorkAsync _unitOfWork,
            IOPS_EntrustmentInforService oPS_EntrustmentInforService)
        {
            bms_Bill_ApService = _bms_Bill_ApService;
            bms_Bill_ArService = _bms_Bill_ArService;
            FinanceService = _financeservice;
            unitOfWork = _unitOfWork;
            _oPS_EntrustmentInforService = oPS_EntrustmentInforService;
        }

        /// <summary>
        /// Get :Finances/PageList
        /// For Index View EasyUI-datagrid load  data 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="filterRules"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            string AddfilterRules = Request["AddfilterRules"] ?? "";

            List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
            if (!string.IsNullOrWhiteSpace(filterRules))
                Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
            var NewAddfilters = new List<IEnumerable<filterRule>>();
            if (Addfilters != null && Addfilters.Any())
            {
                foreach (var item in Addfilters)
                {
                    NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                }
            }

            int totalCount = 0;
            //int pagenum = offset / limit +1;
            //获取数据
            IQueryable<Finance> Qquery;
            Qquery = FinanceService.GetData(filterRules, null);
            if (NewAddfilters != null && NewAddfilters.Any())
            {
                foreach (var item in NewAddfilters)
                {
                    if (item != null && item.Any())
                    {
                        var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                        Qquery = Qquery.Union(Qquery1);
                    }
                }
            }
            var QResult = Qquery.ToList();
            #region 排除-移除的项

            string RemoveIdStr = Request["RemoveId"] ?? "";
            var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
            if (ArrRemoveId != null && ArrRemoveId.Any())
            {
                var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                QResult = (from p in QResult
                           where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                           select p).ToList();
                
                //在移除的账单中若是有符合增加查询的数据仍带出
                foreach (var item in NewAddfilters)
                {
                    if (item != null && item.Any())
                    {
                        var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null).ToList();
                        Qquery1 = (from p in Qquery1
                                   where ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                                   select p).ToList();
                        QResult = QResult.Union(Qquery1).ToList();
                    }


                }

            }

            #endregion

            totalCount = QResult.Count();
            var QQResult = QResult.AsQueryable().OrderBy(sort == "Bill_Object_IdNAME" ? "Bill_Object_Id" : sort, order).Skip((page - 1) * rows).Take(rows).ToList();

            #region 获取 缓存 数据

            var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);//客商
            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();//帐户
            var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);//枚举
            var ArrPARA_AirPort = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);//港口
            var ArrPARA_CURR = (IEnumerable<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);//币种

            var QArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "PayWay");
            var Q_Result = from n in QQResult
                           join a in ArrAppUser on n.EDITWHO equals a.UserName into a_tmp
                           from atmp in a_tmp.DefaultIfEmpty()
                           join g in ArrAppUser on n.BillEDITWHO equals g.UserName into g_tmp
                           from gtmp in g_tmp.DefaultIfEmpty()
                           join b in QArrBD_DEFDOC_LIST on n.Payway equals b.LISTNAME into b_tmp
                           from btmp in b_tmp.DefaultIfEmpty()
                           join c in ArrPARA_AirPort on n.End_Port equals c.PortCode into c_tmp
                           from ctmp in c_tmp.DefaultIfEmpty()
                           join d in ArrPARA_CURR on n.Money_Code equals d.CURR_CODE into d_tmp
                           from dtmp in d_tmp.DefaultIfEmpty()
                           join e in ArrCusBusInfo on n.Carriage_Account_Code equals e.EnterpriseId into e_tmp
                           from etmp in e_tmp.DefaultIfEmpty()
                           join f in ArrCusBusInfo on n.Bill_Object_Id equals f.EnterpriseId into f_tmp
                           from ftmp in f_tmp.DefaultIfEmpty()
                           select new
                           {
                               Id = n.Id,
                               n.MBL,
                               n.IsAr,
                               Dzbh = n.Dzbh,
                               n.Ops_M_OrdId,
                               n.IsMBLJS,//总单结算-分摊数据时产生
                               n.FTParentId,//分摊主数据-分摊数据时，记录有哪个数据 分摊下来的
                               //n.Ops_H_OrdId,
                               Line_No = n.Line_No,
                               Bill_Type = n.Bill_Type,
                               Bill_Account = n.Bill_Account,
                               //Bill_Account2 = n.Bill_Account2,
                               Bill_Account2 = n.Bill_Account2 == 0 ? "0" : Math.Round((decimal)n.Bill_Account2, 2).ToString("#0.00"),
                               n.Bill_TaxRate,
                               n.Bill_TaxRateType,
                               n.Bill_HasTax,
                               Bill_Amount = n.Bill_Amount == 0 ? "0" : Math.Round((decimal)n.Bill_Amount, 2).ToString("#0.00"),
                               Bill_TaxAmount = n.Bill_TaxAmount == 0 ? "0" : Math.Round((decimal)n.Bill_TaxAmount, 2).ToString("#0.00"),
                               Bill_AmountTaxTotal = n.Bill_AmountTaxTotal == 0 ? "0" : Math.Round((decimal)n.Bill_AmountTaxTotal, 2).ToString("#0.00"),
                               Money_Code = n.Money_Code,
                               Money_CodeNAME = dtmp == null ? string.Empty : dtmp.CURR_Name,
                               n.Carriage_Account_Code,
                               Carriage_Account_CodeNAME = etmp == null ? string.Empty : etmp.EnterpriseName,
                               Bill_Object_Id = n.Bill_Object_Id,
                               Bill_Object_IdNAME = ftmp == null ? string.Empty : ftmp.EnterpriseName,
                               Bill_Object_Name = n.Bill_Object_Name,
                               Payway = n.Payway,
                               PaywayNAME = btmp == null ? string.Empty : btmp.LISTNAME,
                               Remark = n.Remark,
                               Bill_Date = n.Bill_Date,
                               AuditStatus = n.AuditStatus,
                               n.Sumbmit_Status,//提交标志
                               n.Sumbmit_No,//提交号
                               n.Sumbmit_Name,    //提交人
                               n.Sumbmit_Date,    //提交时间
                               SignIn_Status = n.SignIn_Status,//签收标志
                               SignIn_No = n.SignIn_No,       //签收号
                               n.Invoice_Status,//开票标志
                               n.Invoice_No,      //开票号码
                               n.Invoice_MoneyCode,//开票币种
                               n.Invoice_FeeType,//开票费目
                               n.Invoice_Id,//开票人Id
                               n.Invoice_Name,//开票人
                               n.Invoice_Date,    //开票日期
                               n.Invoice_Remark,//开票要求
                               n.SellAccount_Status,//销账标志
                               n.SellAccount_Name,//销账人
                               n.SellAccount_Date,//销账日期
                               n.Create_Status,
                               n.Cancel_Status,
                               n.Status,
                               Sumbmit_ECCNo = n.Sumbmit_ECCNo,//ECC发票号 
                               SignIn_ECCNo = n.SignIn_ECCNo,//采购订单号 
                               ECC_Status = n.ECC_Status,//ECC状态 
                               ECC_StatusMsg = n.ECC_StatusMsg,//ECC状态信息 
                               ECC_InvoiceSendDate = n.ECC_InvoiceSendDate,//ECC发票发送时间
                               ECC_InvoiceRecvDate = n.ECC_InvoiceRecvDate,//ECC发票接收时间
                               n.OperatingPoint,
                               n.ADDID,
                               n.ADDTS,
                               n.ADDWHO,
                               EDITID = n.EDITID,
                               EDITTS = n.EDITTS,
                               EDITWHO = n.EDITWHO,
                               EDITWHONAME = atmp == null ? string.Empty : atmp.UserNameDesc,
                               BillEDITID = n.BillEDITID,
                               BillEDITTS = n.BillEDITTS,
                               BillEDITWHO = n.BillEDITWHO,
                               BillEDITWHONAME = gtmp == null ? string.Empty : gtmp.UserNameDesc,
                               n.End_Port,
                               //End_PortNAME = ctmp == null ? string.Empty : ctmp.PortName,
                               n.Flight_Date_Want
                           };

            #endregion

            var pagelist = new { total = totalCount, rows = Q_Result };
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult
            {
                Content = serializer.Serialize(pagelist),      //data为要序列化的LINQ对象
                ContentType = "application/json"
            };
            return result;
            //return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get :Finances/GetDtlData
        /// For Index View EasyUI-datagrid load  data 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="filterRules"></param>
        /// <returns></returns>
        public ActionResult GetDtlData(List<Finance> ArrFinance, int page = 1, int rows = 10, string sort = "ID", string order = "asc", string filterRules = "")
        {
            int totalCount = 0;
            if (ArrFinance == null || !ArrFinance.Any())
                return Json(new { total = totalCount });
            //获取数据
            IQueryable<FinanceDtl> Qquery;
            Qquery = FinanceService.GetDtlData(ArrFinance, filterRules);

            totalCount = Qquery.Count();
            var QResult = Qquery.OrderBy(sort, order).Skip((page - 1) * rows).Take(rows).ToList();

            #region 获取 缓存 数据

            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();//帐户
            var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);//枚举
            var ArrPARA_CURR = (IEnumerable<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);//币种

            var QArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "PayWay");
            var Q_Result = from n in QResult
                           join a in ArrAppUser on n.EDITWHO equals a.UserName into a_tmp
                           from atmp in a_tmp.DefaultIfEmpty()
                           join g in ArrAppUser on n.BillEDITWHO equals g.UserName into g_tmp
                           from gtmp in g_tmp.DefaultIfEmpty()
                           join b in QArrBD_DEFDOC_LIST on n.Payway equals b.LISTNAME into b_tmp
                           from btmp in b_tmp.DefaultIfEmpty()
                           join c in ArrPARA_CURR on n.Money_Code equals c.CURR_CODE into c_tmp
                           from ctmp in c_tmp.DefaultIfEmpty()
                           join d in ArrAppUser on n.ADDWHO equals d.UserName into d_tmp
                           from dtmp in d_tmp.DefaultIfEmpty()
                           select new
                           {
                               Id = n.Id,
                               n.MBL,
                               Dzbh = n.Dzbh,
                               n.Ops_M_OrdId,
                               //n.Ops_H_OrdId,
                               Line_No = n.Line_No,
                               Line_Id = n.Line_Id,//费用序号
                               Bill_Type = n.Bill_Type,
                               n.IsAr,
                               Bill_Amount = n.Bill_Amount == 0 ? "0" : Math.Round((decimal)n.Bill_Amount, 2).ToString("#0.00"),
                               n.Bill_TaxRate,
                               n.Bill_HasTax,
                               Bill_AmountTaxTotal = n.Bill_AmountTaxTotal == 0 ? "0" : Math.Round((decimal)n.Bill_Amount, 2).ToString("#0.00"),
                               Money_Code = n.Money_Code,
                               Money_CodeNAME = ctmp == null ? string.Empty : ctmp.CURR_Name,
                               Charge_Code = n.Charge_Code,// 费用代码 
                               Charge_Desc = n.Charge_Desc,// 费用名称 
                               Unitprice2 = n.Unitprice2 == 0 ? "0" : Math.Round((decimal)n.Unitprice2, 2).ToString("#0.00"),// 实际单价 
                               Qty = n.Qty,// 数量 
                               Account2 = n.Account2 == 0 ? "0" : Math.Round((decimal)n.Account2, 2).ToString("#0.00"),// 实际金额
                               Bill_Object_Id = n.Bill_Object_Id,
                               Bill_Object_Name = n.Bill_Object_Name,
                               Payway = n.Payway,
                               PaywayNAME = btmp == null ? string.Empty : btmp.LISTNAME,
                               Remark = n.Remark,
                               Bill_Date = n.Bill_Date,
                               AuditStatus = n.AuditStatus,
                               n.Sumbmit_Status,//提交标志
                               n.Sumbmit_No,//提交号
                               n.Sumbmit_Name,    //提交人
                               n.Sumbmit_Date,    //提交时间
                               SignIn_Status = n.SignIn_Status,//签收标志
                               SignIn_No = n.SignIn_No,       //签收号
                               n.Invoice_Status,//开票标志
                               n.Invoice_No,      //开票号码
                               n.Invoice_MoneyCode,//开票币种
                               n.Invoice_FeeType,//开票费目
                               n.Invoice_Id,//开票人Id
                               n.Invoice_Name,//开票人
                               n.Invoice_Date,    //开票日期
                               n.SellAccount_Status,//销账标志
                               n.SellAccount_Name,//销账人
                               n.SellAccount_Date,//销账日期
                               n.Create_Status,
                               n.Cancel_Status,
                               n.Status,
                               n.OperatingPoint,
                               n.ADDID,
                               n.ADDTS,
                               n.ADDWHO,
                               ADDWHONAME = dtmp == null ? string.Empty : dtmp.UserNameDesc,
                               EDITID = n.EDITID,
                               EDITTS = n.EDITTS,
                               EDITWHO = n.EDITWHO,
                               EDITWHONAME = atmp == null ? string.Empty : atmp.UserNameDesc,
                               BillEDITID = n.BillEDITID,
                               BillEDITTS = n.BillEDITTS,
                               BillEDITWHO = n.BillEDITWHO,
                               BillEDITWHONAME = gtmp == null ? string.Empty : gtmp.UserNameDesc,
                           };

            #endregion

            var pagelist = new { total = totalCount, rows = Q_Result };

            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="Finance"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData(FinanceChangeViewModel Finance)
        {
            if (Finance.updated != null)
            {
                #region

                var ControllActinMsg = "编辑";
                bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Edit", ControllActinMsg);
                if (!IsHaveQx)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = "您没有权限" + ControllActinMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                        return null;
                    }
                }

                #endregion

                //foreach (var updated in Finance.updated)
                //{
                //    _bms_Bill_ApService.Update(updated);
                //}
            }
            if (Finance.deleted != null)
            {
                #region

                var ControllActinMsg = "删除";
                bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Delete", ControllActinMsg);
                if (!IsHaveQx)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = "您没有权限" + ControllActinMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                        return null;
                    }
                }

                #endregion

                var ArrDeltId = Finance.deleted.Select(x => x.Id);
                //var ArrDelt = _bms_Bill_ApService.Queryable().Where(x => ArrDeltId.Contains(x.Id)).ToList();
                //if (ArrDelt.Any(x => x.AuditStatus < 0 || x.AuditStatus >= AirOutEnumType.AuditStatusEnum.AuditSuccess))
                //{
                //    //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
                //    string ActionGuidName1 = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
                //    string ActionGuid1 = ViewData[ActionGuidName1] == null ? "" : ViewData[ActionGuidName1].ToString();
                //    return Json(new { Success = false, ErrMsg = "应付账单，部分数据已经审核，无法删除！", ActionGuidName = ActionGuidName1, ActionGuid = ActionGuid1 }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    foreach (var deleted in Finance.deleted)
                //    {
                //        _bms_Bill_ApService.Delete(deleted.Id);
                //    }
                //}
            }
            if (Finance.inserted != null)
            {
                #region

                var ControllActinMsg = "创建";
                bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Create", ControllActinMsg);
                if (!IsHaveQx)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = "您没有权限" + ControllActinMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                        return null;
                    }
                }

                #endregion

                //foreach (var inserted in Finance.inserted)
                //{
                //    _bms_Bill_ApService.Insert(inserted);
                //}
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((Finance.updated != null && Finance.updated.Any()) ||
                (Finance.deleted != null && Finance.deleted.Any()) ||
                (Finance.inserted != null && Finance.inserted.Any()))
                {
                    unitOfWork.SaveChanges();
                    return Json(new { Success = true, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作", ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GET: Finance
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 金额汇总
        /// 应收/应付+币别 分开合计
        /// </summary>
        /// <param name="filterRules">搜索条件</param>
        /// <returns></returns>
        public ActionResult AmountToatal(string filterRules = "")
        {
            try
            {
                //汇率
                var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);//unitOfWork.Repository<Rate>().Queryable();
                //获取数据
                IQueryable<Finance> Qquery = FinanceService.GetData(filterRules);

                #region 获取增加查询的数据

                string AddfilterRules = Request["AddfilterRules"] ?? "";

                List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
                if (!string.IsNullOrWhiteSpace(filterRules))
                    Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
                var NewAddfilters = new List<IEnumerable<filterRule>>();
                if (Addfilters != null && Addfilters.Any())
                {
                    foreach (var item in Addfilters)
                    {
                        NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                    }
                }
                if (NewAddfilters != null && NewAddfilters.Any())
                {
                    foreach (var item in NewAddfilters)
                    {
                        if (item != null && item.Any())
                        {
                            var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                            Qquery = Qquery.Union(Qquery1);
                        }
                    }
                }

                #endregion

                var QResult = Qquery.ToList();

                #region 排除-移除的项

                string RemoveIdStr = Request["RemoveId"] ?? "";
                var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
                if (ArrRemoveId != null && ArrRemoveId.Any())
                {
                    var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                    QResult = (from p in QResult
                               where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                               select p).ToList();
                }

                #endregion

                if (QResult.Any(x => x.Flight_Date_Want == null))
                {
                    return Json(new { Success = false, ErrMsg = "无法汇总金额,所选数据中 含有航班日期为空的数据！" });
                }

                var ArApTotal = FinanceService.GetArApBillAccountByFlight_Date(QResult);

                string ArTotal = "应收：" + string.Join(" , ", ArApTotal.Where(x => x.IsAr).Select(x => x.Money_Code + ":" + x.Bill_Account2Total.ToString("0.00")));
                string ApTotal = "应付：" + string.Join(" , ", ArApTotal.Where(x => !x.IsAr).Select(x => x.Money_Code + ":" + x.Bill_Account2Total.ToString("0.00")));
                decimal Ar = ArApTotal.Where(x => x.IsAr).Sum(x => x.NewBill_Account2Total);
                decimal Ap = ArApTotal.Where(x => !x.IsAr).Sum(x => x.NewBill_Account2Total);

                return Json(new { Success = true, ErrMsg = ArTotal + " " + ApTotal + "<br/>毛利：折合CNY：" + (Ar - Ap).ToString("0.00") });
            }
            catch (Exception ex)
            {
                Common.WriteLogByLog4Net(ex, Common.Log4NetMsgType.Error);
                return Json(new { Success = false, ErrMsg = Common.GetExceptionMsg(ex) });
            }
        }

        /// <summary>
        /// 统计选中金额汇总
        /// 应收/应付+币别 分开合计
        /// </summary>
        /// <param name="ArrFinance">需要统计的应收/付 数据集</param>
        /// <returns></returns>
        public ActionResult AmountToatalSelted(List<Finance> ArrFinance)
        {
            try
            {
                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return Json(new { Success = false, ErrMsg = "统计选中金额汇总，没有任何数据需要合计！" });
                }

                var ArApTotal = FinanceService.GetArApBillAccountByFlight_Date(ArrFinance);

                //string ArTotal = "应收：" + string.Join(" , ", ArApTotal.Where(x => x.IsAr).Select(x => x.Money_Code + ":" + x.Bill_Account2Total.ToString("0.00")));
                //string ApTotal = "应付：" + string.Join(" , ", ArApTotal.Where(x => !x.IsAr).Select(x => x.Money_Code + ":" + x.Bill_Account2Total.ToString("0.00")));
                //return Json(new { Success = true, ErrMsg = ArTotal + " " + ApTotal });

                string ArTotal = "应收：" + string.Join(" , ", ArApTotal.Where(x => x.IsAr).Select(x => x.Money_Code + ":" + x.Bill_Account2Total.ToString("0.00")));
                string ApTotal = "应付：" + string.Join(" , ", ArApTotal.Where(x => !x.IsAr).Select(x => x.Money_Code + ":" + x.Bill_Account2Total.ToString("0.00")));
                decimal Ar = ArApTotal.Where(x => x.IsAr).Sum(x => x.NewBill_Account2Total);
                decimal Ap = ArApTotal.Where(x => !x.IsAr).Sum(x => x.NewBill_Account2Total);

                return Json(new { Success = true, ErrMsg = ArTotal + " " + ApTotal + "<br/>毛利：折合CNY：" + (Ar - Ap).ToString("0.00") });
            }
            catch (Exception ex)
            {
                Common.WriteLogByLog4Net(ex, Common.Log4NetMsgType.Error);
                return Json(new { Success = false, ErrMsg = Common.GetExceptionMsg(ex) });
            }
        }

        /// <summary>
        /// 应收/付 审核
        /// </summary>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <param name="AuditState">审核状态（默认通过）</param>
        /// <returns></returns>
        public ActionResult Audit(List<Finance> ArrFinance, int AuditState = (int)AirOutEnumType.AuditStatusEnum.AuditSuccess)
        {
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                if (ArrFinance == null || !ArrFinance.Any())
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "审核出错：<br/>操作数据不能为空！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "审核出错：<br/>操作数据不能为Null！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                List<int> ArrArAuditId = ArrFinance.Where(x => x.IsAr).Select(x => x.Id).ToList();
                List<int> ArrApAuditId = ArrFinance.Where(x => !x.IsAr).Select(x => x.Id).ToList();
                if (ArrArAuditId.Any() || ArrApAuditId.Any())
                {
                    if (ArrArAuditId.Any() && ArrApAuditId.Any())
                    {
                        #region 应收 和 应付 数据审核

                        ORetAudit = bms_Bill_ArService.Audit(ArrArAuditId, AuditState);
                        if (ORetAudit.Success)
                        {
                            //审核 不修改 修改人 修改时间 数据
                            SQLDALHelper.OracleHelper.ExecuteNonQuery("update bms_Bill_Ars t set t.EDITID=null,t.EDITWHO=null,t.EDITTS =null where t.id in ('" + string.Join("','", ArrArAuditId) + "')");
                            ORetAudit = bms_Bill_ApService.Audit(ArrApAuditId, AuditState);
                            if (!ORetAudit.Success)
                            {
                                ORetAudit.ErrMsg = "应收审核成功，但 " + ORetAudit.ErrMsg;
                                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                            }
                            else
                            {
                                //审核 不修改 修改人 修改时间 数据
                                SQLDALHelper.OracleHelper.ExecuteNonQuery("update bms_Bill_Aps t set t.EDITID=null,t.EDITWHO=null,t.EDITTS =null where t.id in ('" + string.Join("','", ArrApAuditId) + "')");
                                ORetAudit.Success = true;
                                ORetAudit.ErrMsg = "审核成功";
                                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                            }
                        }
                        else
                        {
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                        }

                        #endregion
                    }
                    else
                    {
                        #region 应收 或 应付 数据审核

                        if (ArrArAuditId.Any())
                        {
                            ORetAudit = bms_Bill_ArService.Audit(ArrArAuditId, AuditState);
                            //审核 不修改 修改人 修改时间 数据
                            SQLDALHelper.OracleHelper.ExecuteNonQuery("update bms_Bill_Ars t set t.EDITID=null,t.EDITWHO=null,t.EDITTS =null where t.id in ('" + string.Join("','", ArrArAuditId) + "')");
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                        }
                        if (ArrApAuditId.Any())
                        {
                            ORetAudit = bms_Bill_ApService.Audit(ArrApAuditId, AuditState);
                            //审核 不修改 修改人 修改时间 数据
                            SQLDALHelper.OracleHelper.ExecuteNonQuery("update bms_Bill_Aps t set t.EDITID=null,t.EDITWHO=null,t.EDITTS =null where t.id in ('" + string.Join("','", ArrApAuditId) + "')");
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                        }
                        ORetAudit.Success = true;
                        ORetAudit.ErrMsg = "审核成功";
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");

                        #endregion
                    }
                }
                else
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "审核出错：<br/>没有需要审核的数据！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
            }
            catch (Exception ex)
            {
                var errMsg = Common.GetExceptionMsg(ex);
                ORetAudit.Success = false;
                ORetAudit.ErrMsg = "审核出错：<br/>" + errMsg;
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
        }

        /// <summary>
        /// 编辑账单类型和结算方
        /// </summary>
        /// <returns></returns>
        public ActionResult ArApEdit_PopupWin()
        {
            return PartialView("ArApEdit_PopupWin");
        }

        /// <summary>
        /// 编辑账单类型和结算方
        /// </summary>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <param name="Bill_Type">账单类型</param>
        /// <param name="SettleAccout">结算方</param>
        /// <param name="SettleAccout">结算方名称</param>
        /// <returns></returns>
        public ActionResult SaveArApEdit_PopupWin(List<Finance> ArrFinance, string Bill_Type = "", string SettleAccout = "", string SettleAccoutName = "")
        {
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                if (string.IsNullOrWhiteSpace(Bill_Type) && string.IsNullOrWhiteSpace(SettleAccout))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "编辑账单出错：<br/>编辑内容不能为空！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                if (ArrFinance == null || !ArrFinance.Any())
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "编辑账单出错：<br/>操作数据不能为空！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "编辑账单出错：<br/>操作数据不能为Null！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;

                List<int> ArrArId = ArrFinance.Where(x => x.IsAr).Select(x => x.Id).ToList();
                List<int> ArrApId = ArrFinance.Where(x => !x.IsAr).Select(x => x.Id).ToList();
                if (ArrArId.Any() || ArrApId.Any())
                {
                    var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                    if (!string.IsNullOrWhiteSpace(SettleAccout) && string.IsNullOrWhiteSpace(SettleAccoutName))
                    {
                        var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == SettleAccout);
                        if (QArrCusBusInfo.Any())
                        {
                            SettleAccoutName = QArrCusBusInfo.FirstOrDefault().CHNName;
                        }
                        else
                        {
                            ORetAudit.Success = false;
                            ORetAudit.ErrMsg = "编辑账单出错：<br/>结算方数据无法找到！";
                            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                        }
                    }
                    var ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArId.Contains(x.Id)).ToList();
                    var ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrApId.Contains(x.Id)).ToList();

                    #region 应收 或 应付 数据编辑

                    if (ArrAr.Any())
                    {
                        foreach (var item in ArrAr)
                        {
                            System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                            entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                            var isModify = false;
                            if (!string.IsNullOrWhiteSpace(SettleAccout))
                            {
                                item.Bill_Object_Id = SettleAccout;
                                entry.Property(t => t.Bill_Object_Id).IsModified = true; //设置要更新的属性
                                item.Bill_Object_Name = SettleAccoutName;
                                entry.Property(t => t.Bill_Object_Name).IsModified = true; //设置要更新的属性
                                isModify = true;
                            }
                            if (!string.IsNullOrWhiteSpace(Bill_Type))
                            {
                                item.Bill_Type = Bill_Type;
                                entry.Property(t => t.Bill_Type).IsModified = true; //设置要更新的属性
                                isModify = true;
                            }
                            if (isModify)
                                item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        }
                    }
                    if (ArrAp.Any())
                    {
                        foreach (var item in ArrAp)
                        {
                            System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap> entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                            entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                            var isModify = false;
                            if (!string.IsNullOrWhiteSpace(SettleAccout))
                            {
                                var IsModified = entry.Property(t => t.Bill_Type).IsModified;
                                item.Bill_Object_Id = SettleAccout;
                                entry.Property(t => t.Bill_Object_Id).IsModified = true; //设置要更新的属性
                                item.Bill_Object_Name = SettleAccoutName;
                                entry.Property(t => t.Bill_Object_Name).IsModified = true; //设置要更新的属性
                                isModify = true;
                            }
                            if (!string.IsNullOrWhiteSpace(Bill_Type))
                            {
                                item.Bill_Type = Bill_Type;
                                entry.Property(t => t.Bill_Type).IsModified = true; //设置要更新的属性
                                isModify = true;
                            }
                            if (isModify)
                                item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        }
                    }
                    WebdbContxt.SaveChanges();

                    ORetAudit.Success = true;
                    ORetAudit.ErrMsg = "编辑账单成功";
                    dynamic data = new System.Dynamic.ExpandoObject();
                    if (!string.IsNullOrWhiteSpace(SettleAccout))
                    {
                        data.Bill_Object_Id = SettleAccout;
                        data.Bill_Object_Name = SettleAccoutName;
                    }
                    if (!string.IsNullOrWhiteSpace(Bill_Type))
                    {
                        data.Bill_Type = Bill_Type;
                    }
                    ORetAudit.data = data;
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");

                    #endregion
                }
                else
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "编辑账单出错：<br/>没有需要审核的数据！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
            }
            catch (Exception ex)
            {
                var errMsg = Common.GetExceptionMsg(ex);
                ORetAudit.Success = false;
                ORetAudit.ErrMsg = "编辑账单出错：<br/>" + errMsg;
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
        }

        /// <summary>
        /// 开具D/N发票
        /// </summary>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <returns></returns>
        public ActionResult AddDNFP(List<Finance> ArrFinance)
        {
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "开具D/N发票出错：<br/>操作数据不能为空！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "开具D/N发票出错：<br/>操作数据不能为Null！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                if (ArrFinance.Any(x => x.Bill_Type != "D/N"))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "开具D/N发票出错：<br/>操作数据，必须都D/N账单！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                //提交号
                var ArrSubmitNo = ArrFinance.Select(x => x.Sumbmit_No);
                var ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrSubmitNo.Contains(x.Sumbmit_No)).ToList();
                if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || !x.Sumbmit_Status || x.Status != AirOutEnumType.UseStatusEnum.Enable))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "开具D/N发票出错：<br/>部分数据，没有审核通过；或没有提交；或已经作废/删除！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }

                #endregion

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                var Invoice_No = SequenceBuilder.NextDNFPNo();//获取DN 发票流水号
                foreach (var item in ArrAr)
                {
                    System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                    entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                    item.Invoice_No = Invoice_No;
                    item.Invoice_Status = true;
                    item.Invoice_Date = DateTime.Now;
                    item.Invoice_Id = Utility.CurrentAppUser.UserName;
                    item.Invoice_Name = Utility.CurrentAppUser.UserNameDesc;
                    entry.Property(t => t.Invoice_No).IsModified = true; //设置要更新的属性
                    entry.Property(t => t.Invoice_Status).IsModified = true; //设置要更新的属性
                    entry.Property(t => t.Invoice_Date).IsModified = true; //设置要更新的属性
                    entry.Property(t => t.Invoice_Id).IsModified = true; //设置要更新的属性
                    entry.Property(t => t.Invoice_Name).IsModified = true; //设置要更新的属性
                    item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                }
                WebdbContxt.SaveChanges();
                ORetAudit.Success = true;
                ORetAudit.ErrMsg = "";
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
            catch (Exception ex)
            {
                var errMsg = Common.GetExceptionMsg(ex);
                ORetAudit.Success = false;
                ORetAudit.ErrMsg = "开具D/N发票出错：<br/>" + errMsg;
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
        }

        /// <summary>
        /// 应收应付 账单作废
        /// </summary>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <returns></returns>
        public ActionResult ArApCancel(List<Finance> ArrFinance)
        {
            dynamic ORetAudit = new System.Dynamic.ExpandoObject();
            try
            {
                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "账单作废出错：<br/>操作数据不能为空！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    ORetAudit.Success = false;
                    ORetAudit.ErrMsg = "账单作废出错：<br/>操作数据不能为Null！";
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
                }

                #endregion

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                var ArrArId = ArrFinance.Where(x => x.IsAr).Select(x => x.Id);
                if (ArrArId.Any())
                {
                    var ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArId.Contains(x.Id)).ToList();
                    foreach (var item in ArrAr)
                    {
                        System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ar> entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.Status = AirOutEnumType.UseStatusEnum.Disable;
                        item.Cancel_Status = true;
                        item.Cancel_Date = DateTime.Now;
                        item.Cancel_Id = Utility.CurrentAppUser.UserName;
                        item.Cancel_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(t => t.Status).IsModified = true; //设置要更新的属性
                        entry.Property(t => t.Cancel_Status).IsModified = true; //设置要更新的属性
                        entry.Property(t => t.Cancel_Date).IsModified = true; //设置要更新的属性
                        entry.Property(t => t.Cancel_Id).IsModified = true; //设置要更新的属性
                        entry.Property(t => t.Cancel_Name).IsModified = true; //设置要更新的属性
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                    }
                }
                var ArrApId = ArrFinance.Where(x => !x.IsAr).Select(x => x.Id);
                if (ArrApId.Any())
                {
                    var ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrApId.Contains(x.Id)).ToList();
                    foreach (var item in ArrAp)
                    {
                        System.Data.Entity.Infrastructure.DbEntityEntry<Bms_Bill_Ap> entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.Status = AirOutEnumType.UseStatusEnum.Disable;
                        item.Cancel_Status = true;
                        item.Cancel_Date = DateTime.Now;
                        item.Cancel_Id = Utility.CurrentAppUser.UserName;
                        item.Cancel_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(t => t.Status).IsModified = true; //设置要更新的属性
                        entry.Property(t => t.Cancel_Status).IsModified = true; //设置要更新的属性
                        entry.Property(t => t.Cancel_Date).IsModified = true; //设置要更新的属性
                        entry.Property(t => t.Cancel_Id).IsModified = true; //设置要更新的属性
                        entry.Property(t => t.Cancel_Name).IsModified = true; //设置要更新的属性
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                    }
                }
                WebdbContxt.SaveChanges();
                ORetAudit.Success = true;
                ORetAudit.ErrMsg = "";
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
            catch (Exception ex)
            {
                var errMsg = Common.GetExceptionMsg(ex);
                ORetAudit.Success = false;
                ORetAudit.ErrMsg = "账单作废出错：<br/>" + errMsg;
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(ORetAudit), "application/json");
            }
        }

        /// <summary>
        /// 增加查询
        /// 应收/应付+币别 分开合计
        /// </summary>
        /// <param name="filterRules">搜索条件</param>
        /// <returns></returns>
        public ActionResult AddSearch(string filterRules = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filterRules))
                    return Json(new { Success = false, ErrMsg = "搜索条件不能为空！" });
                //获取数据
                var Qquery = FinanceService.GetData(filterRules);
                var QResult = Qquery.ToList();
                if (QResult == null || !QResult.Any())
                {
                    return Json(new { Success = false, ErrMsg = "没有查询到任何数据！" });
                }
                else
                {
                    return Json(new { Success = true, Rows = QResult });
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogByLog4Net(ex, Common.Log4NetMsgType.Error);
                return Json(new { Success = false, ErrMsg = Common.GetExceptionMsg(ex) });
            }
        }

        /// <summary>
        /// 应收/应付 提交 界面
        /// </summary>
        /// <param name="ArrFinance">应收/付 账单</param>
        /// <returns></returns>
        public ActionResult ArApSubmit_PopupWin(List<Finance> ArrFinance)
        {
            IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
            IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
            OPS_EntrustmentInfor OOpsEttInfor = new OPS_EntrustmentInfor();
            bool IsAr = false;

            #region 验证状态

            if (ArrFinance == null || !ArrFinance.Any())
            {
                ModelState.AddModelError("", "错误：操作数据不能为空！");
                return PartialView("ArApSubmit_PopupWin");
            }
            if (ArrFinance.Any(x => x == null || x.Id <= 0))
            {
                ModelState.AddModelError("", "错误：操作数据不能为Null！");
                return PartialView("ArApSubmit_PopupWin");
            }
            var Arr = from p in ArrFinance
                      group p by p.IsAr into g
                      select new
                      {
                          IsAr = g.Key,
                          Num = g.Count()
                      };
            if (Arr.Count() > 1)
            {
                ModelState.AddModelError("", "错误：不能同时操作 应收/应付数据！");
                return PartialView("ArApSubmit_PopupWin");
            }
            else
            {
                IsAr = Arr.FirstOrDefault().IsAr;
                ViewData["IsAr"] = IsAr;
                var ArrArApId = ArrFinance.Select(x => x.Id);
                if (IsAr)
                {
                    if (ArrFinance.Any(x => !x.IsAr))
                    {
                        ModelState.AddModelError("", "错误：应收提交,不能含有应付数据！");
                        return PartialView("ArApSubmit_PopupWin");
                    }

                    ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                    if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Sumbmit_Status || x.Cancel_Status))
                    {
                        ModelState.AddModelError("", "错误：应收 部分数据,未审核，已提交或已作废！");
                        return PartialView("ArApSubmit_PopupWin");
                    }
                    var GroupArrAr = from p in ArrAr
                                     group p by new { p.Bill_Object_Id, p.Money_Code, p.Bill_Type } into g
                                     select new
                                     {
                                         Money_Code = g.Key.Money_Code,
                                         Bill_Type = g.Key.Bill_Type,
                                         Bill_Object_Id = g.Key,
                                         Num = g.Count()
                                     };
                    if (GroupArrAr.Count() > 1)
                    {
                        ModelState.AddModelError("", "错误：应收 数据,结算方，币别，账单类型 不一致！");
                        return PartialView("ArApSubmit_PopupWin");
                    }

                    var OPS_EntrustmentInforRep = unitOfWork.Repository<OPS_EntrustmentInfor>();
                    var Ops_M_OrdId = ArrAr.FirstOrDefault().Ops_M_OrdId;
                    OOpsEttInfor = OPS_EntrustmentInforRep.Queryable().Where(x => !x.Is_TG && x.MBLId == Ops_M_OrdId).FirstOrDefault();
                    if (OOpsEttInfor == null || OOpsEttInfor.Id <= 0)
                    {
                        ModelState.AddModelError("", "错误：应收 数据,委托 数据不存在或已退关！");
                        return PartialView("ArApSubmit_PopupWin");
                    }
                }
                else
                {
                    if (ArrFinance.Any(x => x.IsAr))
                    {
                        ModelState.AddModelError("", "错误：应付提交,不能含有应收数据！");
                        return PartialView("ArApSubmit_PopupWin");
                    }
                    ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                    if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Sumbmit_Status || x.Cancel_Status))
                    {
                        ModelState.AddModelError("", "错误：应付 部分数据,未审核，已提交或已作废！");
                        return PartialView("ArApSubmit_PopupWin");
                    }
                    var GroupArrAp = from p in ArrAp
                                     group p by p.Bill_Object_Id into g
                                     select new
                                     {
                                         Bill_Object_Id = g.Key,
                                         Num = g.Count()
                                     };
                    if (GroupArrAp.Count() > 1)
                    {
                        ModelState.AddModelError("", "错误：应付 数据,结算方不一致！");
                        return PartialView("ArApSubmit_PopupWin");
                    }

                    var OPS_EntrustmentInforRep = unitOfWork.Repository<OPS_EntrustmentInfor>();
                    var Ops_M_OrdId = ArrAp.FirstOrDefault().Ops_M_OrdId;
                    OOpsEttInfor = OPS_EntrustmentInforRep.Queryable().Where(x => !x.Is_TG && x.MBLId == Ops_M_OrdId).FirstOrDefault();
                    if (OOpsEttInfor == null || OOpsEttInfor.Id <= 0)
                    {
                        ModelState.AddModelError("", "错误：应付 数据,委托 数据不存在或已退关！");
                        return PartialView("ArApSubmit_PopupWin");
                    }
                }
            }

            #endregion

            Finance OFinance = new Finance();
            OFinance.IsAr = IsAr;
            OFinance.Invoice_MoneyCode = "CNY";
            OFinance.Invoice_FeeType = "";
            OFinance.Sumbmit_No = SequenceBuilder.NextSubmit_No(IsAr);//提交号
            if (IsAr)
            {
                OFinance.Invoice_TaxRateType = "X0";
                var OAr = ArrAr.FirstOrDefault();
                OFinance.Bill_Object_Id = OAr.Bill_Object_Id;
                OFinance.Bill_Object_Name = OAr.Bill_Object_Name;
                OFinance.Money_Code = OAr.Money_Code;
                OFinance.Bill_HasTax = OAr.Bill_HasTax;
                OFinance.Bill_TaxAmount = OAr.Bill_TaxAmount;
                OFinance.Bill_TaxRate = OAr.Bill_TaxRate;
                OFinance.Bill_TaxRateType = OAr.Bill_TaxRateType;
                OFinance.Bill_AmountTaxTotal = OAr.Bill_AmountTaxTotal;
                OFinance.Bill_Account2 = ArrAr.Sum(x => x.Bill_Account2);
                OFinance.Bill_Type = OAr.Bill_Type;
                OFinance.Dzbh = OAr.Dzbh;
                OFinance.Line_No = OAr.Line_No;
                OFinance.MBL = OAr.MBL;
                OFinance.Invoice_Remark = "空运出口/" + OAr.MBL + "/" + OOpsEttInfor.HBL + "/" + OOpsEttInfor.Operation_Id + "/" + (OOpsEttInfor.Flight_Date_Want == null ? "" : ((DateTime)OOpsEttInfor.Flight_Date_Want).ToString("yyyy-MM-dd")) + "/" + OOpsEttInfor.End_Port;//开票要求
                if (!string.IsNullOrEmpty(OAr.Invoice_MoneyCode))
                    OFinance.Invoice_MoneyCode = OAr.Invoice_MoneyCode;
                if (!string.IsNullOrEmpty(OAr.Invoice_FeeType))
                    OFinance.Invoice_FeeType = OAr.Invoice_FeeType;
                if (!string.IsNullOrEmpty(OAr.Invoice_TaxRateType))
                    OFinance.Invoice_TaxRateType = OAr.Invoice_TaxRateType;
                OFinance.Invoice_HasTax = OAr.Invoice_HasTax;
            }
            else
            {
                OFinance.Invoice_TaxRateType = "J0";
                var OAp = ArrAp.FirstOrDefault();
                OFinance.Bill_Object_Id = OAp.Bill_Object_Id;
                OFinance.Bill_Object_Name = OAp.Bill_Object_Name;
                OFinance.Money_Code = OAp.Money_Code;
                OFinance.Bill_HasTax = OAp.Bill_HasTax;
                OFinance.Bill_TaxAmount = OAp.Bill_TaxAmount;
                OFinance.Bill_TaxRate = OAp.Bill_TaxRate;
                OFinance.Bill_TaxRateType = OAp.Bill_TaxRateType;
                OFinance.Bill_AmountTaxTotal = OAp.Bill_AmountTaxTotal;
                OFinance.Bill_Account2 = ArrAp.Sum(x => x.Bill_Account2);
                OFinance.Bill_Type = OAp.Bill_Type;
                OFinance.Dzbh = OAp.Dzbh;
                OFinance.Line_No = OAp.Line_No;
                OFinance.MBL = OAp.MBL;
                OFinance.Invoice_Remark = "空运出口/" + OAp.MBL + "/" + OOpsEttInfor.HBL + "/" + OOpsEttInfor.Operation_Id + "/" + (OOpsEttInfor.Flight_Date_Want == null ? "" : ((DateTime)OOpsEttInfor.Flight_Date_Want).ToString("yyyy-MM-dd")) + "/" + OOpsEttInfor.End_Port;//开票要求
                if (!string.IsNullOrEmpty(OAp.Invoice_MoneyCode))
                    OFinance.Invoice_MoneyCode = OAp.Invoice_MoneyCode;
                if (!string.IsNullOrEmpty(OAp.Invoice_FeeType))
                    OFinance.Invoice_FeeType = OAp.Invoice_FeeType;
                if (!string.IsNullOrEmpty(OAp.Invoice_TaxRateType))
                    OFinance.Invoice_TaxRateType = OAp.Invoice_TaxRateType;
                OFinance.Invoice_HasTax = OAp.Invoice_HasTax;
            }

            return PartialView("ArApSubmit_PopupWin", OFinance);
        }

        /// <summary>
        /// 应收/应付 提交 保存
        /// </summary>
        /// <param name="OFinance">提交的数据</param>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <returns></returns>
        public ActionResult SaveArApSubmit_PopupWin(Finance OFinance, List<Finance> ArrFinance, bool SendECC = false)
        {
            try
            {
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
                bool IsAr = false;
                var SendECCStr = Request["SendECC"] ?? "";
                if (!string.IsNullOrEmpty(SendECCStr))
                    SendECC = Common.ChangStrToBool(SendECCStr);

                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为空！" });
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为Null！" });
                }
                if (string.IsNullOrEmpty(OFinance.Invoice_FeeType) || string.IsNullOrEmpty(OFinance.Invoice_TaxRateType) || string.IsNullOrEmpty(OFinance.Invoice_MoneyCode))
                {
                    return Json(new { Success = false, ErrMsg = "错误：开票费用，开票税率，开票币别，不能为空！" });
                }
                var Arr = from p in ArrFinance
                          group p by p.IsAr into g
                          select new
                          {
                              IsAr = g.Key,
                              Num = g.Count()
                          };
                if (Arr.Count() > 1)
                {
                    return Json(new { Success = false, ErrMsg = "错误：不能同时操作 应收/应付数据！" });
                }
                else
                {
                    IsAr = Arr.FirstOrDefault().IsAr;
                    ViewData["IsAr"] = IsAr;
                    var ArrArApId = ArrFinance.Select(x => x.Id);
                    if (IsAr)
                    {
                        ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Sumbmit_Status || x.Cancel_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据,未审核，已提交或已作废！" });
                        }
                        var GroupArrAr = from p in ArrAr
                                         group p by new { p.Bill_Object_Id, p.Money_Code, p.Bill_Type } into g
                                         select new
                                         {
                                             Money_Code = g.Key.Money_Code,
                                             Bill_Type = g.Key.Bill_Type,
                                             Bill_Object_Id = g.Key,
                                             Num = g.Count()
                                         };
                        if (GroupArrAr.Count() > 1)
                        {
                            return Json(new { Success = true, ErrMsg = "错误：应收 数据,结算方不一致！" });
                        }
                    }
                    else
                    {
                        ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Sumbmit_Status || x.Cancel_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据,未审核，已提交或已作废！" });
                        }
                        var GroupArrAp = from p in ArrAp
                                         group p by p.Bill_Object_Id into g
                                         select new
                                         {
                                             Bill_Object_Id = g.Key,
                                             Num = g.Count()
                                         };
                        if (GroupArrAp.Count() > 1)
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 数据,结算方，币别，账单类型不一致！" });
                        }
                        if (SendECC)
                            return Json(new { Success = false, ErrMsg = "错误：应付 数据,不能提交发送，只能签收再发送！" });
                    }
                }

                if (OFinance == null)
                    OFinance = new Finance();
                if (string.IsNullOrWhiteSpace(OFinance.Sumbmit_No))
                    return Json(new { Success = false, ErrMsg = "错误：提交号，不能为空！" });
                if (SendECC)
                {
                    if (string.IsNullOrWhiteSpace(OFinance.Invoice_FeeType) || string.IsNullOrWhiteSpace(OFinance.Invoice_MoneyCode))
                        return Json(new { Success = false, ErrMsg = "错误：保存并发送时，费目类型或开票币别，不能为空！" });
                }

                #endregion

                #region 数据处理

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                if (IsAr)
                {
                    #region 应收 提交

                    foreach (var item in ArrAr)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.Sumbmit_No = OFinance.Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No).IsModified = true;
                        item.Sumbmit_Status = true;
                        entry.Property(x => x.Sumbmit_Status).IsModified = true;
                        item.Sumbmit_No_Org = OFinance.Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No_Org).IsModified = true;
                        item.Sumbmit_Date = DateTime.Now;
                        entry.Property(x => x.Sumbmit_Date).IsModified = true;
                        item.Sumbmit_Id = Utility.CurrentAppUser.Id;
                        entry.Property(x => x.Sumbmit_Id).IsModified = true;
                        item.Sumbmit_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(x => x.Sumbmit_Name).IsModified = true;

                        item.Invoice_MoneyCode = OFinance.Invoice_MoneyCode;
                        entry.Property(x => x.Invoice_MoneyCode).IsModified = true;
                        item.Invoice_FeeType = OFinance.Invoice_FeeType;
                        entry.Property(x => x.Invoice_FeeType).IsModified = true;
                        item.Invoice_HasTax = OFinance.Invoice_HasTax;
                        entry.Property(x => x.Invoice_HasTax).IsModified = true;
                        item.Invoice_TaxRateType = OFinance.Invoice_TaxRateType;
                        entry.Property(x => x.Invoice_TaxRateType).IsModified = true;
                        item.Invoice_Remark = OFinance.Invoice_Remark;//开票要求
                        entry.Property(x => x.Invoice_Remark).IsModified = true;
                        item.ECCInvoice_Rate = OFinance.ECCInvoice_Rate;//ECC开票汇率
                        entry.Property(x => x.ECCInvoice_Rate).IsModified = true;

                        //if (!string.IsNullOrWhiteSpace(OFinance.Bill_TaxRateType))
                        //{
                        //    if (item.Bill_TaxRateType != OFinance.Bill_TaxRateType || item.Bill_HasTax != OFinance.Bill_HasTax)
                        //    {
                        //        if (item.Bill_TaxRateType != OFinance.Bill_TaxRateType)
                        //        {
                        //            item.Bill_TaxRateType = OFinance.Bill_TaxRateType;
                        //            entry.Property(x => x.Bill_TaxRateType).IsModified = true;
                        //        }
                        //        if (item.Bill_HasTax != OFinance.Bill_HasTax)
                        //        {
                        //            item.Bill_HasTax = OFinance.Bill_HasTax;
                        //            entry.Property(x => x.Bill_HasTax).IsModified = true;
                        //        }
                        //        #region 重新计算 价 税 价税合计

                        //        dynamic OCalcTaxRate = bms_Bill_ArService.CalcTaxRate(item.Bill_HasTax, item.Bill_TaxRate, item.Bill_Account2);
                        //        if (OCalcTaxRate.Success)
                        //        {
                        //            //价（实际金额）
                        //            item.Bill_Amount = OCalcTaxRate.Bill_Amount;
                        //            entry.Property(t => t.Bill_Amount).IsModified = true; //设置要更新的属性
                        //            //税金 （实际金额 * 税率）
                        //            item.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                        //            entry.Property(t => t.Bill_TaxAmount).IsModified = true; //设置要更新的属性
                        //            //价税合计 (价+税金)
                        //            item.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                        //            entry.Property(t => t.Bill_AmountTaxTotal).IsModified = true; //设置要更新的属性
                        //        }
                        //        else
                        //        {
                        //            return Json(new { Success = false, ErrMsg = "应收数据-错误：更新-计算价/税/价税合计出错！" });
                        //        }

                        //        #endregion
                        //    }
                        //}
                    }

                    #endregion
                }
                else
                {
                    #region 应付 提交

                    foreach (var item in ArrAp)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.Sumbmit_No = OFinance.Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No).IsModified = true;
                        item.Sumbmit_Status = true;
                        entry.Property(x => x.Sumbmit_Status).IsModified = true;
                        item.Sumbmit_No_Org = OFinance.Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No_Org).IsModified = true;
                        item.Sumbmit_Date = DateTime.Now;
                        entry.Property(x => x.Sumbmit_Date).IsModified = true;
                        item.Sumbmit_Id = Utility.CurrentAppUser.Id;
                        entry.Property(x => x.Sumbmit_Id).IsModified = true;
                        item.Sumbmit_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(x => x.Sumbmit_Name).IsModified = true;

                        item.Invoice_MoneyCode = OFinance.Invoice_MoneyCode;
                        entry.Property(x => x.Invoice_MoneyCode).IsModified = true;
                        item.Invoice_FeeType = OFinance.Invoice_FeeType;
                        entry.Property(x => x.Invoice_FeeType).IsModified = true;
                        item.Invoice_HasTax = OFinance.Invoice_HasTax;
                        entry.Property(x => x.Invoice_HasTax).IsModified = true;
                        item.Invoice_TaxRateType = OFinance.Invoice_TaxRateType;
                        entry.Property(x => x.Invoice_TaxRateType).IsModified = true;
                        item.Invoice_Remark = OFinance.Invoice_Remark;//开票要求
                        entry.Property(x => x.Invoice_Remark).IsModified = true;
                        item.ECCInvoice_Rate = OFinance.ECCInvoice_Rate;//ECC开票汇率
                        entry.Property(x => x.ECCInvoice_Rate).IsModified = true;

                        //if (!string.IsNullOrWhiteSpace(OFinance.Bill_TaxRateType))
                        //{
                        //    if (item.Bill_TaxRateType != OFinance.Bill_TaxRateType || item.Bill_HasTax != OFinance.Bill_HasTax)
                        //    {
                        //        if (item.Bill_TaxRateType != OFinance.Bill_TaxRateType)
                        //        {
                        //            item.Bill_TaxRateType = OFinance.Bill_TaxRateType;
                        //            entry.Property(x => x.Bill_TaxRateType).IsModified = true;
                        //        }
                        //        if (item.Bill_HasTax != OFinance.Bill_HasTax)
                        //        {
                        //            item.Bill_HasTax = OFinance.Bill_HasTax;
                        //            entry.Property(x => x.Bill_HasTax).IsModified = true;
                        //        }
                        //        #region 重新计算 价 税 价税合计

                        //        dynamic OCalcTaxRate = bms_Bill_ArService.CalcTaxRate(item.Bill_HasTax, item.Bill_TaxRate, item.Bill_Account2);
                        //        if (OCalcTaxRate.Success)
                        //        {
                        //            //价（实际金额）
                        //            item.Bill_Amount = OCalcTaxRate.Bill_Amount;
                        //            entry.Property(t => t.Bill_Amount).IsModified = true; //设置要更新的属性
                        //            //税金 （实际金额 * 税率）
                        //            item.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                        //            entry.Property(t => t.Bill_TaxAmount).IsModified = true; //设置要更新的属性
                        //            //价税合计 (价+税金)
                        //            item.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                        //            entry.Property(t => t.Bill_AmountTaxTotal).IsModified = true; //设置要更新的属性
                        //        }
                        //        else
                        //        {
                        //            return Json(new { Success = false, ErrMsg = "应付数据-错误：更新-计算价/税/价税合计出错！" });
                        //        }

                        //        #endregion
                        //    }
                        //}
                    }

                    #endregion
                }

                #region 异步 发送ECC 数据存储在Redis

                var SendECCErrMsg = "";
                if (SendECC)
                {
                    OFinance.Sumbmit_Date = DateTime.Now;
                    OFinance.Sumbmit_Name = Utility.CurrentAppUser.UserNameDesc;
                    OFinance.Sumbmit_Status = true;
                    var ORet = Send_ECC(OFinance);
                    if (!ORet.Success)
                        SendECCErrMsg = ORet.RetMsg;// "发送ECC错误：未知";
                    else
                    {
                        if (IsAr)
                        {
                            var ArrEditAr = WebdbContxt.ChangeTracker.Entries<Bms_Bill_Ar>().Where(_e => _e.State == EntityState.Modified);
                            foreach (var itemAr in ArrEditAr)
                            {
                                itemAr.Entity.ECC_InvoiceSendDate = DateTime.Now;
                                itemAr.Property(x => x.ECC_InvoiceSendDate).IsModified = true;
                            }
                        }
                        else
                        {
                            var ArrEditAp = WebdbContxt.ChangeTracker.Entries<Bms_Bill_Ap>().Where(_e => _e.State == EntityState.Modified);
                            foreach (var itemAp in ArrEditAp)
                            {
                                itemAp.Entity.ECC_InvoiceSendDate = DateTime.Now;
                                itemAp.Property(x => x.ECC_InvoiceSendDate).IsModified = true;
                            }
                        }
                    }
                }

                #endregion

                unitOfWork.SaveChanges();

                #endregion

                dynamic OResult = new System.Dynamic.ExpandoObject();
                OResult.Success = true;
                OResult.ErrMsg = (string.IsNullOrWhiteSpace(SendECCErrMsg) ? "" : ("保存成功，但 " + SendECCErrMsg));

                dynamic data = new System.Dynamic.ExpandoObject();
                data.Sumbmit_No = OFinance.Sumbmit_No;
                data.Sumbmit_Status = true;
                data.Sumbmit_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                data.Sumbmit_Id = Utility.CurrentAppUser.Id;
                data.Sumbmit_Name = Utility.CurrentAppUser.UserNameDesc;
                data.Invoice_MoneyCode = OFinance.Invoice_MoneyCode;
                data.Invoice_FeeType = OFinance.Invoice_FeeType;
                data.Invoice_HasTax = OFinance.Invoice_HasTax;
                data.Invoice_TaxRateType = OFinance.Invoice_TaxRateType;
                data.Invoice_Remark = OFinance.Invoice_Remark;

                OResult.data = data;

                //return Json(OResult);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(OResult), "application/json");

            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 添加/修改 提交号
        /// </summary>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <param name="Sumbmit_No">提交号</param>
        /// <param name="IsEdit">是否 修改提交号</param>
        /// <returns></returns>
        public ActionResult ArApAddSubmit(List<Finance> ArrFinance, string Sumbmit_No, bool IsEdit)
        {
            try
            {
                IEnumerable<Bms_Bill_Ap> ArrSumbmit_Ap = new List<Bms_Bill_Ap>();//提交号数据
                IEnumerable<Bms_Bill_Ar> ArrSumbmit_Ar = new List<Bms_Bill_Ar>();//提交号数据
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
                bool IsAr = false;

                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为空！" });
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为Null！" });
                }
                var Arr = from p in ArrFinance
                          group p by p.IsAr into g
                          select new
                          {
                              IsAr = g.Key,
                              Num = g.Count()
                          };
                if (Arr.Count() > 1)
                {
                    return Json(new { Success = false, ErrMsg = "错误：不能同时操作 应收/应付数据！" });
                }
                else
                {
                    IsAr = Arr.FirstOrDefault().IsAr;
                    ViewData["IsAr"] = IsAr;
                    var ArrArApId = ArrFinance.Select(x => x.Id);
                    if (IsAr)
                    {
                        ArrSumbmit_Ar = bms_Bill_ArService.Queryable().AsNoTracking().Where(x => x.Sumbmit_No == Sumbmit_No).ToList();
                        if (!ArrSumbmit_Ar.Any() || ArrSumbmit_Ar.Any(x => string.IsNullOrEmpty(x.Invoice_FeeType) || string.IsNullOrEmpty(x.Invoice_MoneyCode) || string.IsNullOrEmpty(x.Invoice_TaxRateType)))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 找不到提交号为：（" + Sumbmit_No + "）数据,或开票费目，开票税率，开票币别 为空！" });
                        }
                        ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据,未审核或已作废！" });
                        }
                        if (ArrAr.Any(x => x.ECC_InvoiceSendDate != null))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据,已经发送ECC无法继续追加在 改提交号下！" });
                        }
                        if (!IsEdit)
                        {
                            if (ArrAr.Any(x => x.Sumbmit_Status))
                            {
                                return Json(new { Success = false, ErrMsg = "错误：应收 部分数据 已提交！" });
                            }
                        }
                        var GroupArrAr = from p in ArrAr
                                         group p by p.Bill_Object_Id into g
                                         select new
                                         {
                                             Bill_Object_Id = g.Key,
                                             Num = g.Count()
                                         };
                        if (GroupArrAr.Count() > 1)
                        {
                            return Json(new { Success = true, ErrMsg = "错误：应收 数据,结算方不一致！" });
                        }
                    }
                    else
                    {
                        ArrSumbmit_Ap = bms_Bill_ApService.Queryable().AsNoTracking().Where(x => x.Sumbmit_No == Sumbmit_No).ToList();
                        if (!ArrSumbmit_Ap.Any() || ArrSumbmit_Ap.Any(x => string.IsNullOrEmpty(x.Invoice_FeeType) || string.IsNullOrEmpty(x.Invoice_MoneyCode) || string.IsNullOrEmpty(x.Invoice_TaxRateType)))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 找不到提交号为：（" + Sumbmit_No + "）数据,或开票费目，开票税率，开票币别 为空！" });
                        }
                        ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据,未审核或已作废！" });
                        }
                        if (!IsEdit)
                        {
                            if (ArrAr.Any(x => x.Sumbmit_Status))
                            {
                                return Json(new { Success = false, ErrMsg = "错误：应付 部分数据 已提交！" });
                            }
                        }
                        var GroupArrAp = from p in ArrAp
                                         group p by p.Bill_Object_Id into g
                                         select new
                                         {
                                             Bill_Object_Id = g.Key,
                                             Num = g.Count()
                                         };
                        if (GroupArrAp.Count() > 1)
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 数据,结算方不一致！" });
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(Sumbmit_No))
                    return Json(new { Success = false, ErrMsg = "错误：提交号，不能为空！" });

                #endregion

                #region 数据处理

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                if (IsAr)
                {
                    #region 应收 添加提交号

                    var OAr = ArrSumbmit_Ar.FirstOrDefault();
                    foreach (var item in ArrAr)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        if (IsEdit)
                            item.Sumbmit_No_Org = item.Sumbmit_No;
                        else
                            item.Sumbmit_No_Org = Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No_Org).IsModified = true;
                        item.Sumbmit_No = Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No).IsModified = true;
                        item.Sumbmit_Status = true;
                        entry.Property(x => x.Sumbmit_Status).IsModified = true;
                        item.Sumbmit_Date = DateTime.Now;
                        entry.Property(x => x.Sumbmit_Date).IsModified = true;
                        item.Sumbmit_Id = Utility.CurrentAppUser.Id;
                        entry.Property(x => x.Sumbmit_Id).IsModified = true;
                        item.Sumbmit_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(x => x.Sumbmit_Name).IsModified = true;

                        item.Invoice_MoneyCode = OAr.Invoice_MoneyCode;
                        entry.Property(x => x.Invoice_MoneyCode).IsModified = true;
                        item.Invoice_FeeType = OAr.Invoice_FeeType;
                        entry.Property(x => x.Invoice_FeeType).IsModified = true;
                        item.Invoice_HasTax = OAr.Invoice_HasTax;
                        entry.Property(x => x.Invoice_HasTax).IsModified = true;
                        item.Invoice_TaxRateType = OAr.Invoice_TaxRateType;
                        entry.Property(x => x.Invoice_TaxRateType).IsModified = true;
                    }

                    #endregion
                }
                else
                {
                    #region 应付 添加提交号

                    var OAp = ArrSumbmit_Ap.FirstOrDefault();
                    foreach (var item in ArrAp)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        if (IsEdit)
                            item.Sumbmit_No_Org = item.Sumbmit_No;
                        else
                            item.Sumbmit_No_Org = Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No_Org).IsModified = true;
                        item.Sumbmit_No = Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No).IsModified = true;
                        item.Sumbmit_Status = true;
                        entry.Property(x => x.Sumbmit_Status).IsModified = true;
                        item.Sumbmit_Date = DateTime.Now;
                        entry.Property(x => x.Sumbmit_Date).IsModified = true;
                        item.Sumbmit_Id = Utility.CurrentAppUser.Id;
                        entry.Property(x => x.Sumbmit_Id).IsModified = true;
                        item.Sumbmit_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(x => x.Sumbmit_Name).IsModified = true;

                        item.Invoice_MoneyCode = OAp.Invoice_MoneyCode;
                        entry.Property(x => x.Invoice_MoneyCode).IsModified = true;
                        item.Invoice_FeeType = OAp.Invoice_FeeType;
                        entry.Property(x => x.Invoice_FeeType).IsModified = true;
                        item.Invoice_HasTax = OAp.Invoice_HasTax;
                        entry.Property(x => x.Invoice_HasTax).IsModified = true;
                        item.Invoice_TaxRateType = OAp.Invoice_TaxRateType;
                        entry.Property(x => x.Invoice_TaxRateType).IsModified = true;
                    }

                    #endregion
                }

                #endregion

                unitOfWork.SaveChanges();

                dynamic OResult = new System.Dynamic.ExpandoObject();
                OResult.Success = true;
                OResult.ErrMsg = "";
                OResult.data = new
                {
                    Sumbmit_No = Sumbmit_No,
                    Sumbmit_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Sumbmit_Id = Utility.CurrentAppUser.Id,
                    Sumbmit_Name = Utility.CurrentAppUser.UserNameDesc
                };
                //return Json(OResult);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(OResult), "application/json");
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 取消提交
        /// </summary>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <param name="SendECC">发送取消ECC</param>
        /// <returns></returns>
        public ActionResult ArApCancelSubmit(List<Finance> ArrFinance)
        {
            try
            {
                bool SendECC = false;
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
                Finance OFinance = new Finance();
                bool IsAr = false;

                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为空！" });
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为Null！" });
                }
                var Arr = from p in ArrFinance
                          group p by p.IsAr into g
                          select new
                          {
                              IsAr = g.Key,
                              Num = g.Count()
                          };
                if (Arr.Count() > 1)
                {
                    return Json(new { Success = false, ErrMsg = "错误：不能同时操作 应收/应付数据！" });
                }
                else
                {
                    IsAr = Arr.FirstOrDefault().IsAr;
                    ViewData["IsAr"] = IsAr;
                    var ArrArApId = ArrFinance.Select(x => x.Id);
                    if (IsAr)
                    {
                        SendECC = true;
                        var Sumbmit_No = "";
                        var ArrAr_Sumbmit_No = bms_Bill_ArService.Queryable().AsNoTracking().Where(x => ArrArApId.Contains(x.Id)).Select(x => x.Sumbmit_No).ToList();
                        //var Sumbmit_NoNum = ArrAr_Sumbmit_No.Distinct().Count();
                        //if (Sumbmit_NoNum > 1)
                        //{
                        //    return Json(new { Success = false, ErrMsg = "错误：应收数据,含有不同的提交号！" });
                        //}
                        //else
                        //    Sumbmit_No = ArrAr_Sumbmit_No.FirstOrDefault();
                        if (!ArrAr_Sumbmit_No.Any())
                            return Json(new { Success = false, ErrMsg = "错误：应收数据,提交号不能为空！" });
                        //已提交 发送ECC的数据，取消全部
                        ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrAr_Sumbmit_No.Contains(x.Sumbmit_No)).ToList();
                        //已提交 未发送ECC的数据，取消选择的数据
                        if (!ArrAr.Any(x => x.ECC_InvoiceSendDate.HasValue))
                        {
                            ArrAr = ArrAr.Where(x => ArrArApId.Contains(x.Id)).ToList();
                        }

                        if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据,未审核或已作废！" });
                        }
                        if (ArrAr.Any(x => !x.Sumbmit_Status || x.Invoice_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据, 未提交或已开票！" });
                        }
                        if (ArrAr.Any(x => x.ECC_InvoiceSendDate != null && x.ECC_Status == null && string.IsNullOrWhiteSpace(x.Sumbmit_ECCNo)))
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据, 已发送ECC，但还未收到回执或ECC发票号为空，不能取消提交！" });
                    }
                    else
                    {
                        var Sumbmit_No = "";
                        var ArrAp_Sumbmit_No = bms_Bill_ApService.Queryable().AsNoTracking().Where(x => ArrArApId.Contains(x.Id)).Select(x => x.Sumbmit_No).ToList();
                        //var Sumbmit_NoNum = ArrAp_Sumbmit_No.Distinct().Count();
                        //if (Sumbmit_NoNum > 1)
                        //{
                        //    return Json(new { Success = false, ErrMsg = "错误：应付数据,含有不同的提交号！" });
                        //}
                        //else
                        //    Sumbmit_No = ArrAp_Sumbmit_No.FirstOrDefault();
                        if (!ArrAp_Sumbmit_No.Any())
                            return Json(new { Success = false, ErrMsg = "错误：应付数据,提交号不能为空！" });
                        //已提交 发送ECC的数据，取消全部
                        ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrAp_Sumbmit_No.Contains(x.Sumbmit_No)).ToList();
                        //已提交 未发送ECC的数据，取消选择的数据
                        if (!ArrAp.Any(x => x.ECC_InvoiceSendDate.HasValue))
                        {
                            ArrAp = ArrAp.Where(x => ArrArApId.Contains(x.Id)).ToList();
                        }

                        if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据,未审核或已作废 ！" });
                        }
                        if (ArrAp.Any(x => !x.Sumbmit_Status || x.Invoice_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据, 未提交或已开票！" });
                        }
                    }
                }

                #endregion

                #region 数据处理

                //默认 为取消发送ECC 报文
                OFinance.IsAr = IsAr;
                OFinance.Cancel_Date = DateTime.Now;
                OFinance.Cancel_Status = true;//Cancel_Status 和 Status 都为取消时  会发送 ECC取消报文
                OFinance.Status = AirOutEnumType.UseStatusEnum.Disable;

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                if (IsAr)
                {
                    #region 取消应收 提交

                    //设置数据
                    var OAr = ArrAr.FirstOrDefault();
                    OFinance.Sumbmit_No = OAr.Sumbmit_No;
                    OFinance.Sumbmit_ECCNo = OAr.Sumbmit_ECCNo;
                    OFinance.Sumbmit_Date = OAr.Sumbmit_Date;
                    OFinance.Sumbmit_Name = OAr.Sumbmit_Name;
                    OFinance.Sumbmit_Status = OAr.Sumbmit_Status;
                    OFinance.ECC_Status = OAr.ECC_Status;
                    OFinance.ECC_StatusMsg = OAr.ECC_StatusMsg;

                    foreach (var item in ArrAr)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.Sumbmit_No_Org = item.Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No_Org).IsModified = true;
                        item.Sumbmit_No = "";
                        entry.Property(x => x.Sumbmit_No).IsModified = true;
                        item.Sumbmit_Status = false;
                        entry.Property(x => x.Sumbmit_Status).IsModified = true;
                        item.Sumbmit_Date = null;
                        entry.Property(x => x.Sumbmit_Date).IsModified = true;
                        item.Sumbmit_Id = "";
                        entry.Property(x => x.Sumbmit_Id).IsModified = true;
                        item.Sumbmit_Name = "";
                        entry.Property(x => x.Sumbmit_Name).IsModified = true;

                        item.Invoice_MoneyCode = "";
                        entry.Property(x => x.Invoice_MoneyCode).IsModified = true;
                        item.Invoice_FeeType = "";
                        entry.Property(x => x.Invoice_FeeType).IsModified = true;
                        item.Invoice_HasTax = false;
                        entry.Property(x => x.Invoice_HasTax).IsModified = true;
                        item.Invoice_TaxRateType = "";
                        entry.Property(x => x.Invoice_TaxRateType).IsModified = true;
                    }

                    #endregion
                }
                else
                {
                    #region 取消应付 提交

                    ////设置数据
                    //var OAp = ArrAp.FirstOrDefault();
                    //OFinance.SignIn_No = OAp.SignIn_No;
                    //OFinance.SignIn_ECCNo = OAp.SignIn_ECCNo;
                    //OFinance.SignIn_Date = OAp.SignIn_Date;
                    //OFinance.SignIn_Name = OAp.SignIn_Name;
                    //OFinance.SignIn_Status = OAp.SignIn_Status;
                    //OFinance.ECC_Status = OAp.ECC_Status;
                    //OFinance.ECC_StatusMsg = OAp.ECC_StatusMsg;

                    foreach (var item in ArrAp)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.Sumbmit_No_Org = item.Sumbmit_No;
                        entry.Property(x => x.Sumbmit_No_Org).IsModified = true;
                        item.Sumbmit_No = "";
                        entry.Property(x => x.Sumbmit_No).IsModified = true;
                        item.Sumbmit_Status = false;
                        entry.Property(x => x.Sumbmit_Status).IsModified = true;
                        item.Sumbmit_Date = null;
                        entry.Property(x => x.Sumbmit_Date).IsModified = true;
                        item.Sumbmit_Id = "";
                        entry.Property(x => x.Sumbmit_Id).IsModified = true;
                        item.Sumbmit_Name = "";
                        entry.Property(x => x.Sumbmit_Name).IsModified = true;

                        item.Invoice_MoneyCode = "";
                        entry.Property(x => x.Invoice_MoneyCode).IsModified = true;
                        item.Invoice_FeeType = "";
                        entry.Property(x => x.Invoice_FeeType).IsModified = true;
                        item.Invoice_HasTax = false;
                        entry.Property(x => x.Invoice_HasTax).IsModified = true;
                        item.Invoice_TaxRateType = "";
                        entry.Property(x => x.Invoice_TaxRateType).IsModified = true;
                    }

                    #endregion
                }

                #region 异步 发送ECC 数据存储在Redis

                var SendECCErrMsg = "";
                if (SendECC)
                {
                    var ORet = Send_ECC(OFinance);
                    if (!ORet.Success)
                        SendECCErrMsg = ORet.RetMsg;// "发送ECC错误：未知";
                    else
                    {
                        if (IsAr)
                        {
                            var ArrEditAr = WebdbContxt.ChangeTracker.Entries<Bms_Bill_Ar>().Where(_e => _e.State == EntityState.Modified);
                            foreach (var itemAr in ArrEditAr)
                            {
                                itemAr.Entity.ECC_InvoiceSendDate = DateTime.Now;
                                itemAr.Property(x => x.ECC_InvoiceSendDate).IsModified = true;
                            }
                        }
                        else
                        {
                            var ArrEditAp = WebdbContxt.ChangeTracker.Entries<Bms_Bill_Ap>().Where(_e => _e.State == EntityState.Modified);
                            foreach (var itemAp in ArrEditAp)
                            {
                                itemAp.Entity.ECC_InvoiceSendDate = DateTime.Now;
                                itemAp.Property(x => x.ECC_InvoiceSendDate).IsModified = true;
                            }
                        }
                    }
                }

                #endregion

                #endregion

                unitOfWork.SaveChanges();

                if (!string.IsNullOrWhiteSpace(SendECCErrMsg))
                    return Json(new { Success = true, ErrMsg = "保存成功，但 " + SendECCErrMsg });
                else
                    return Json(new { Success = true, ErrMsg = "" });
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 应收/应付 开票 界面
        /// </summary>
        /// <param name="ArrFinance">应收/付 账单</param>
        /// <returns></returns>
        public ActionResult ArApAddInvoice_PopupWin(List<Finance> ArrFinance)
        {
            IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
            IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
            bool IsAr = false;

            #region 验证状态

            if (ArrFinance == null || !ArrFinance.Any())
            {
                ModelState.AddModelError("", "错误：操作数据不能为空！");
                return PartialView("ArApAddInvoice_PopupWin");
            }
            if (ArrFinance.Any(x => x == null || x.Id <= 0))
            {
                ModelState.AddModelError("", "错误：操作数据不能为Null！");
                return PartialView("ArApAddInvoice_PopupWin");
            }
            var Arr = from p in ArrFinance
                      group p by p.IsAr into g
                      select new
                      {
                          IsAr = g.Key,
                          Num = g.Count()
                      };
            if (Arr.Count() > 1)
            {
                ModelState.AddModelError("", "错误：不能同时操作 应收/应付数据！");
                return PartialView("ArApAddInvoice_PopupWin");
            }
            else
            {
                IsAr = Arr.FirstOrDefault().IsAr;
                ViewData["IsAr"] = IsAr;
                var ArrArApId = ArrFinance.Select(x => x.Id);
                if (IsAr)
                {
                    ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                    if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || x.Invoice_Status))
                    {
                        ModelState.AddModelError("", "错误：应收 部分数据，未审核，未提交，已作废或已开票！");
                        return PartialView("ArApSubmit_PopupWin");
                    }
                    var GroupArrAr = from p in ArrAr
                                     group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                     select new
                                     {
                                         Money_Code = g.Key.Money_Code,
                                         Bill_Object_Id = g.Key.Bill_Object_Id,
                                         Num = g.Count()
                                     };
                    if (GroupArrAr.Count() > 1)
                    {
                        ModelState.AddModelError("", "错误：应收 数据,结算方/币别 不一致！");
                        return PartialView("ArApAddInvoice_PopupWin");
                    }
                }
                else
                {
                    ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                    if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || x.Invoice_Status))
                    {
                        ModelState.AddModelError("", "错误：应付 部分数据，未审核，未提交，已作废或已开票！");
                        return PartialView("ArApAddInvoice_PopupWin");
                    }
                    var GroupArrAp = from p in ArrAp
                                     group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                     select new
                                     {
                                         Money_Code = g.Key.Money_Code,
                                         Bill_Object_Id = g.Key.Bill_Object_Id,
                                         Num = g.Count()
                                     };
                    if (GroupArrAp.Count() > 1)
                    {
                        ModelState.AddModelError("", "错误：应付 数据,结算方/币别 不一致！");
                        return PartialView("ArApAddInvoice_PopupWin");
                    }
                }
            }

            #endregion

            Finance OFinance = new Finance();
            OFinance.IsAr = IsAr;
            OFinance.Invoice_Date = DateTime.Today;
            if (IsAr)
            {
                var OAr = ArrAr.FirstOrDefault();
                OFinance.Bill_Type = OAr.Bill_Type;
                OFinance.Bill_Object_Id = OAr.Bill_Object_Id;
                OFinance.Money_Code = OAr.Money_Code;
                OFinance.Invoice_MoneyCode = OAr.Invoice_MoneyCode;
                OFinance.Invoice_FeeType = OAr.Invoice_FeeType;
                OFinance.Bill_Account2 = ArrAr.Sum(x => x.Bill_Account2);// OAr.Bill_Account2;
            }
            else
            {
                var OAp = ArrAp.FirstOrDefault();
                OFinance.Bill_Type = OAp.Bill_Type;
                OFinance.Bill_Object_Id = OAp.Bill_Object_Id;
                OFinance.Money_Code = OAp.Money_Code;
                OFinance.Invoice_MoneyCode = OAp.Invoice_MoneyCode;
                OFinance.Invoice_FeeType = OAp.Invoice_FeeType;
                OFinance.Bill_Account2 = ArrAp.Sum(x => x.Bill_Account2); //OAp.Bill_Account2;
            }

            return PartialView("ArApAddInvoice_PopupWin", OFinance);
        }

        /// <summary>
        /// 应收/应付 开票 保存
        /// </summary>
        /// <param name="OFinance">提交的数据</param>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <returns></returns>
        public ActionResult SaveArApAddInvoice_PopupWin(Finance OFinance, List<Finance> ArrFinance)
        {
            try
            {
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
                bool IsAr = false;

                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为空！" });
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为Null！" });
                }
                var Arr = from p in ArrFinance
                          group p by p.IsAr into g
                          select new
                          {
                              IsAr = g.Key,
                              Num = g.Count()
                          };
                if (Arr.Count() > 1)
                {
                    return Json(new { Success = false, ErrMsg = "错误：不能同时操作 应收/应付数据！" });
                }
                else
                {
                    IsAr = Arr.FirstOrDefault().IsAr;
                    ViewData["IsAr"] = IsAr;
                    var ArrArApId = ArrFinance.Select(x => x.Id);
                    if (IsAr)
                    {
                        ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || x.Invoice_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据，未审核，未提交，已作废或已开票！" });
                        }
                        var GroupArrAr = from p in ArrAr
                                         group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                         select new
                                         {
                                             Money_Code = g.Key.Money_Code,
                                             Bill_Object_Id = g.Key.Bill_Object_Id,
                                             Num = g.Count()
                                         };
                        if (GroupArrAr.Count() > 1)
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 数据,结算方/币别 不一致！" });
                        }
                    }
                    else
                    {
                        ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || x.Invoice_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据，未审核，未提交，已作废或已开票！" });
                        }
                        var GroupArrAp = from p in ArrAp
                                         group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                         select new
                                         {
                                             Money_Code = g.Key.Money_Code,
                                             Bill_Object_Id = g.Key.Bill_Object_Id,
                                             Num = g.Count()
                                         };
                        if (GroupArrAp.Count() > 1)
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 数据,结算方/币别 不一致！" });
                        }
                    }
                }

                if (OFinance == null)
                    OFinance = new Finance();
                if (string.IsNullOrWhiteSpace(OFinance.Invoice_No) || OFinance.Invoice_Date == null)
                {
                    return Json(new { Success = false, ErrMsg = "发票号 或 开票时间 是必填项！" });
                }

                #endregion

                #region 保存数据

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;

                if (IsAr)
                {
                    #region 应收 提交

                    foreach (var item in ArrAr)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.Invoice_No = OFinance.Invoice_No;
                        entry.Property(x => x.Invoice_No).IsModified = true;
                        item.Invoice_Status = true;
                        entry.Property(x => x.Invoice_Status).IsModified = true;
                        item.Invoice_Date = OFinance.Invoice_Date;
                        entry.Property(x => x.Invoice_Date).IsModified = true;
                        item.Invoice_Id = Utility.CurrentAppUser.Id;
                        entry.Property(x => x.Invoice_Id).IsModified = true;
                        item.Invoice_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(x => x.Invoice_Name).IsModified = true;
                    }

                    #endregion
                }
                else
                {
                    #region 应收 提交

                    foreach (var item in ArrAp)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.Invoice_No = OFinance.Invoice_No;
                        entry.Property(x => x.Invoice_No).IsModified = true;
                        item.Invoice_Status = true;
                        entry.Property(x => x.Invoice_Status).IsModified = true;
                        item.Invoice_Date = OFinance.Invoice_Date;
                        entry.Property(x => x.Invoice_Date).IsModified = true;
                        item.Invoice_Id = Utility.CurrentAppUser.Id;
                        entry.Property(x => x.Invoice_Id).IsModified = true;
                        item.Invoice_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(x => x.Invoice_Name).IsModified = true;
                    }

                    #endregion
                }

                unitOfWork.SaveChanges();

                #endregion

                dynamic OResult = new System.Dynamic.ExpandoObject(); ;
                OResult.Success = true;
                OResult.ErrMsg = "";
                OResult.data = new
                {
                    Invoice_No = OFinance.Invoice_No,
                    Invoice_Status = true,
                    Invoice_Date = OFinance.Invoice_Date,
                    Invoice_Id = Utility.CurrentAppUser.Id,
                    Invoice_Name = Utility.CurrentAppUser.UserNameDesc
                };

                //return Json(OResult);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(OResult), "application/json");
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 取消发票
        /// </summary>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <returns></returns>
        public ActionResult CancelInvoice(List<Finance> ArrFinance)
        {
            try
            {
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
                bool IsAr = false;

                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为空！" });
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为Null！" });
                }
                var Arr = from p in ArrFinance
                          group p by p.IsAr into g
                          select new
                          {
                              IsAr = g.Key,
                              Num = g.Count()
                          };
                if (Arr.Count() > 1)
                {
                    return Json(new { Success = false, ErrMsg = "错误：不能同时操作 应收/应付数据！" });
                }
                else
                {
                    IsAr = Arr.FirstOrDefault().IsAr;
                    ViewData["IsAr"] = IsAr;
                    var ArrArApId = ArrFinance.Select(x => x.Id);
                    if (IsAr)
                    {
                        ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据,未审核或已作废！" });
                        }
                        if (ArrAr.Any(x => !x.Sumbmit_Status || !x.Invoice_Status || x.SellAccount_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据, 未提交,未开票或 已销账！" });
                        }
                    }
                    else
                    {
                        ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据,未审核或已作废 ！" });
                        }
                        if (ArrAp.Any(x => !x.Sumbmit_Status || !x.Invoice_Status || x.SellAccount_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据, 未提交,未开票或 已销账！" });
                        }
                    }
                }

                #endregion

                #region 数据处理

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                if (IsAr)
                {
                    #region 取消应收 提交

                    foreach (var item in ArrAr)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.Invoice_No = "";
                        entry.Property(x => x.Invoice_No).IsModified = true;
                        item.Invoice_Status = false;
                        entry.Property(x => x.Invoice_Status).IsModified = true;
                        item.Invoice_Date = null;
                        entry.Property(x => x.Invoice_Date).IsModified = true;
                        item.Invoice_Id = "";
                        entry.Property(x => x.Invoice_Id).IsModified = true;
                        item.Invoice_Name = "";
                        entry.Property(x => x.Invoice_Name).IsModified = true;
                    }

                    #endregion
                }
                else
                {
                    #region 取消应付 提交

                    foreach (var item in ArrAp)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.Invoice_No = "";
                        entry.Property(x => x.Invoice_No).IsModified = true;
                        item.Invoice_Status = false;
                        entry.Property(x => x.Invoice_Status).IsModified = true;
                        item.Invoice_Date = null;
                        entry.Property(x => x.Invoice_Date).IsModified = true;
                        item.Invoice_Id = "";
                        entry.Property(x => x.Invoice_Id).IsModified = true;
                        item.Invoice_Name = "";
                        entry.Property(x => x.Invoice_Name).IsModified = true;
                    }

                    #endregion
                }

                #endregion

                unitOfWork.SaveChanges();

                dynamic OResult = new System.Dynamic.ExpandoObject(); ;
                OResult.Success = true;
                OResult.ErrMsg = "";
                OResult.data = new
                {
                    Invoice_No = "",
                    Invoice_Status = false,
                    Invoice_Date = "",
                    Invoice_Id = "",
                    Invoice_Name = ""
                };
                //return Json(OResult);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(OResult), "application/json");
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 发票作废
        /// </summary>
        /// <param name="Invoice_No">发票号</param>
        /// <returns></returns>
        public ActionResult InvoiceCancel(string Invoice_No)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Invoice_No))
                    return Json(new { Success = false, ErrMsg = "错误：发票号，不能为空！" });
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                List<Bms_Bill_Ap_Dtl> ArrApDtl = new List<Bms_Bill_Ap_Dtl>();
                IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
                List<Bms_Bill_Ar_Dtl> ArrArDtl = new List<Bms_Bill_Ar_Dtl>();
                ArrAr = bms_Bill_ArService.Queryable().Where(x => x.Invoice_No == Invoice_No).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList().Where(x => x.Line_No > 0);
                ArrAp = bms_Bill_ApService.Queryable().Where(x => x.Invoice_No == Invoice_No).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList().Where(x => x.Line_No > 0);
                if (ArrAr.Any() && ArrAp.Any())
                {
                    return Json(new { Success = false, ErrMsg = "错误：不能同时 作废 应收和应付发票！" });
                }
                else if (!ArrAr.Any() && !ArrAp.Any())
                    return Json(new { Success = false, ErrMsg = "错误：无法找到需要作废 应收或应付发票！" });

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                if (ArrAr.Any())
                {
                    if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status))
                        return Json(new { Success = false, ErrMsg = "错误：应收 部分数据,未审核或已作废！！" });
                    if (ArrAr.Any(x => !x.Sumbmit_Status || !x.Invoice_Status || x.SellAccount_Status))
                    {
                        return Json(new { Success = false, ErrMsg = "错误：应收 部分数据, 未提交,未开票或 已销账！" });
                    }

                    //要发送 取消报文的提交号+发票号
                    var ArrFinance = (from p in ArrAr
                                      //where !string.IsNullOrEmpty(p.Sumbmit_No) && !string.IsNullOrEmpty(p.Sumbmit_ECCNo)
                                      group p by new { p.Sumbmit_No, p.Sumbmit_ECCNo } into g
                                      select new { Id = g.Max(n => n.Id), g.Key.Sumbmit_No, g.Key.Sumbmit_ECCNo })
                                      .Distinct()
                                      .Select(x => new Finance
                                      {
                                          Id = x.Id,
                                          IsAr = true,
                                          Sumbmit_No = x.Sumbmit_No,
                                          Sumbmit_ECCNo = x.Sumbmit_ECCNo,
                                          Cancel_Status = true,//Cancel_Status 和 Status 都为取消时  会发送 ECC取消报文
                                          Status = AirOutEnumType.UseStatusEnum.Disable
                                      }).ToList();

                    var Bms_Bill_Ar_DtlRep = unitOfWork.Repository<Bms_Bill_Ar_Dtl>();
                    //ArrArDtl = Bms_Bill_Ar_DtlRep.Queryable().Where(x => x.OBms_Bill_Ar.Invoice_No == Invoice_No).ToList();
                    var Num = -ArrAr.Count() * 2 - 10;
                    var DtlNum = -ArrAr.Sum(x => x.ArrBms_Bill_Ar_Dtl.Count()) * 2 - 10;
                    var ArrProperty = typeof(Bms_Bill_Ar).GetProperties(System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    var ArrDtlProperty = typeof(Bms_Bill_Ar_Dtl).GetProperties(System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    foreach (var item in ArrAr)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.ECC_InvoiceSendDate = DateTime.Now;
                        entry.Property(x => x.ECC_InvoiceSendDate).IsModified = true;

                        #region 将账单序号变为负数

                        item.Line_No = -item.Line_No;
                        entry.Property(x => x.Line_No).IsModified = true;
                        item.Cancel_Status = true;
                        entry.Property(x => x.Cancel_Status).IsModified = true;
                        item.Status = AirOutEnumType.UseStatusEnum.Disable;
                        entry.Property(x => x.Status).IsModified = true;

                        #region 复制 2个(账单及其明细) 1个将 金额变为负数 对冲，一个去除任何状态的新数据

                        var ArCopy1 = new Bms_Bill_Ar();//对冲账单
                        var ArCopy2 = new Bms_Bill_Ar();//无状态新账单
                        Common.SetSamaProtity(ArCopy1, item, false, ArrProperty, ArrProperty);
                        Common.SetSamaProtity(ArCopy2, item, false, ArrProperty, ArrProperty);

                        #region 对冲账单 金额变为负数

                        ArCopy1.Id = Num++;
                        var Line_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh);
                        ArCopy1.Line_No = -Convert.ToInt32(Line_No);
                        ArCopy1.Cancel_Status = true;
                        ArCopy1.Status = AirOutEnumType.UseStatusEnum.Disable;
                        ArCopy1.Bill_Date = DateTime.Now;
                        ArCopy1.Bill_Amount = -ArCopy1.Bill_Amount;//价
                        ArCopy1.Bill_TaxAmount = -ArCopy1.Bill_TaxAmount;//税金
                        ArCopy1.Bill_AmountTaxTotal = -ArCopy1.Bill_AmountTaxTotal;//价税合计
                        ArCopy1.Bill_Account2 = -ArCopy1.Bill_Account2;//实际金额
                        ArCopy1.ADDID = "";
                        ArCopy1.ADDTS = null;
                        ArCopy1.ADDWHO = "";
                        ArCopy1.EDITID = "";
                        ArCopy1.EDITTS = null;
                        ArCopy1.EDITWHO = "";
                        if (item.ArrBms_Bill_Ar_Dtl != null && item.ArrBms_Bill_Ar_Dtl.Any())
                        {
                            ArrArDtl = new List<Bms_Bill_Ar_Dtl>();
                            foreach (var OBms_Bill_Ar_Dtl in item.ArrBms_Bill_Ar_Dtl)
                            {
                                var entryDtl = WebdbContxt.Entry<Bms_Bill_Ar_Dtl>(OBms_Bill_Ar_Dtl);
                                entryDtl.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                OBms_Bill_Ar_Dtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                                OBms_Bill_Ar_Dtl.Cancel_Status = true;
                                entryDtl.Property(x => x.Cancel_Status).IsModified = true;
                                OBms_Bill_Ar_Dtl.Status = AirOutEnumType.UseStatusEnum.Disable;
                                entryDtl.Property(x => x.Status).IsModified = true;

                                var ArDtl = new Bms_Bill_Ar_Dtl();
                                ArrArDtl.Add(ArDtl);
                                //反射设置 两个类 相同字段名数据 （效率差-没有直接设置快）
                                Common.SetSamaProtity(ArDtl, OBms_Bill_Ar_Dtl, false, ArrDtlProperty, ArrDtlProperty);
                                ArDtl.Bms_Bill_Ar_Id = ArCopy1.Id;
                                ArDtl.Id = DtlNum++;
                                ArDtl.Line_No = ArCopy1.Line_No;
                                var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh + "_" + -ArCopy1.Line_No, true);
                                ArDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                ArDtl.Cancel_Status = true;
                                ArDtl.Status = AirOutEnumType.UseStatusEnum.Disable;
                                ArDtl.Unitprice2 = -ArDtl.Unitprice2;//单价
                                ArDtl.Account2 = -ArDtl.Account2;//实际金额
                                ArDtl.Bill_Amount = -ArDtl.Bill_Amount;//价
                                ArDtl.Bill_TaxAmount = -ArDtl.Bill_TaxAmount;//税金
                                ArDtl.Bill_AmountTaxTotal = -ArDtl.Bill_AmountTaxTotal;//价税合计
                                ArDtl.ADDID = "";
                                ArDtl.ADDTS = null;
                                ArDtl.ADDWHO = "";
                                ArDtl.EDITID = "";
                                ArDtl.EDITTS = null;
                                ArDtl.EDITWHO = "";
                                ArDtl.OBms_Bill_Ar = null;
                                Bms_Bill_Ar_DtlRep.Insert(ArDtl);
                            }
                            ArCopy1.ArrBms_Bill_Ar_Dtl = ArrArDtl;
                        }

                        #endregion

                        #region 无状态账单

                        ArCopy2.Id = Num++;
                        Line_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh);
                        ArCopy2.Line_No = Convert.ToInt32(Line_No);
                        ArCopy2.Bill_Date = DateTime.Now;
                        ArCopy2.Status = AirOutEnumType.UseStatusEnum.Enable;
                        ArCopy2.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                        ArCopy2.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;

                        ArCopy2.Sumbmit_ECCNo = null;//ECC发票号
                        ArCopy2.Invoice_MoneyCode = null;//开票币种
                        ArCopy2.Invoice_FeeType = null;//开票费目
                        ArCopy2.Invoice_TaxRateType = null;//开票税率
                        ArCopy2.Invoice_HasTax = false;//含税/不含税
                        ArCopy2.ECC_Status = null;//ECC状态
                        ArCopy2.ECC_StatusMsg = null;//ECC状态信息
                        ArCopy2.ECC_InvoiceSendDate = null;//ECC发票发送时间
                        ArCopy2.ECC_InvoiceRecvDate = null;//ECC发票接收时间        
                        ArCopy2.AuditNo = null;//审核号
                        ArCopy2.AuditId = null;//审核人Id
                        ArCopy2.AuditName = null;//审核人
                        ArCopy2.AuditDate = null;//审核日期
                        ArCopy2.Cancel_Status = false;//作废标志
                        ArCopy2.Cancel_Id = null;//作废人Id
                        ArCopy2.Cancel_Name = null;//作废人
                        ArCopy2.Cancel_Date = null;//作废日期
                        ArCopy2.Sumbmit_Status = false;//提交标志
                        ArCopy2.Sumbmit_No = null;//发票号
                        ArCopy2.Sumbmit_No_Org = null;//原始提交号
                        ArCopy2.Sumbmit_Id = null;//提交人Id
                        ArCopy2.Sumbmit_Name = null;//提交人
                        ArCopy2.Sumbmit_Date = null;//提交日期
                        ArCopy2.Invoice_Status = false;//开票标志
                        ArCopy2.Invoice_No = null;//开票号码
                        ArCopy2.Invoice_Desc = null;//开票名称
                        ArCopy2.Invoice_Id = null;//开票人Id
                        ArCopy2.Invoice_Name = null;//开票人
                        ArCopy2.Invoice_Date = null;//开票日期
                        ArCopy2.Invoice_Remark = null;//开票要求
                        ArCopy2.SellAccount_Status = false;//销账标志
                        ArCopy2.SellAccount_Id = null;//销账人Id
                        ArCopy2.SellAccount_Name = null;//销账人
                        ArCopy2.SellAccount_Date = null;//销账日期
                        ArCopy2.ADDID = "";
                        ArCopy2.ADDTS = null;
                        ArCopy2.ADDWHO = "";
                        ArCopy2.EDITID = "";
                        ArCopy2.EDITTS = null;
                        ArCopy2.EDITWHO = "";
                        if (item.ArrBms_Bill_Ar_Dtl != null && item.ArrBms_Bill_Ar_Dtl.Any())
                        {
                            ArrArDtl = new List<Bms_Bill_Ar_Dtl>();
                            foreach (var OBms_Bill_Ar_Dtl in item.ArrBms_Bill_Ar_Dtl)
                            {
                                var ArDtl = new Bms_Bill_Ar_Dtl();
                                ArrArDtl.Add(ArDtl);
                                //反射设置 两个类 相同字段名数据 （效率差-没有直接设置快）
                                Common.SetSamaProtity(ArDtl, OBms_Bill_Ar_Dtl, false, ArrDtlProperty, ArrDtlProperty);
                                ArDtl.Bms_Bill_Ar_Id = ArCopy2.Id;
                                ArDtl.Id = DtlNum++;
                                ArDtl.Line_No = ArCopy2.Line_No;
                                var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh + "_" + ArCopy2.Line_No, true);
                                ArDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                ArDtl.Collate_Id = null;//对帐人Id
                                ArDtl.Collate_Name = null;//对帐人
                                ArDtl.Collate_Date = null;//对帐日期
                                ArDtl.Collate_Status = 0;//对帐标志
                                ArDtl.Collate_No = null;//对账号码
                                ArDtl.User_Id = null;//操作人员
                                ArDtl.User_Name = null;//操作人名
                                ArDtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                                ArDtl.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;//审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                                ArDtl.AuditNo = null;//审核号
                                ArDtl.Invoice_Status = false;//开票标志
                                ArDtl.Invoice_No = null;//开票号码
                                ArDtl.Sumbmit_Status = false;//提交标志
                                ArDtl.Sumbmit_No = null;//提交号
                                ArDtl.SellAccount_Status = false;//销账标志
                                ArDtl.Cancel_Status = false;//作废标志
                                ArDtl.OBms_Bill_Ar = null;
                                ArDtl.ADDID = "";
                                ArDtl.ADDTS = null;
                                ArDtl.ADDWHO = "";
                                ArDtl.EDITID = "";
                                ArDtl.EDITTS = null;
                                ArDtl.EDITWHO = "";
                                Bms_Bill_Ar_DtlRep.Insert(ArDtl);
                            }
                            ArCopy2.ArrBms_Bill_Ar_Dtl = ArrArDtl;
                        }

                        #endregion

                        bms_Bill_ArService.Insert(ArCopy1);
                        bms_Bill_ArService.Insert(ArCopy2);

                        #endregion

                        #endregion
                    }
                    #region 异步 发送ECC 数据存储在Redis

                    var SendECC = true;//发送ECC
                    var SendECCErrMsg = "";
                    if (SendECC)
                    {
                        foreach (var OFinance in ArrFinance)
                        {
                            var ORet = Send_ECC(OFinance);
                            if (!ORet.Success)
                            {
                                SendECCErrMsg = ORet.RetMsg;// "发送ECC错误：未知";
                                Common.WriteLog_Local("发票作废错误：(" + OFinance.Id + "-" + OFinance.Sumbmit_No + "-" + OFinance.Sumbmit_ECCNo + ")" + SendECCErrMsg, "FinanceController\\InvoiceCancel", true, true);
                            }
                        }
                    }

                    #endregion
                }

                if (ArrAp.Any())
                {
                    if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status))
                        return Json(new { Success = false, ErrMsg = "错误：应收 部分数据,未审核或已作废！！" });
                    if (ArrAp.Any(x => !x.Sumbmit_Status || !x.Invoice_Status || x.SellAccount_Status))
                    {
                        return Json(new { Success = false, ErrMsg = "错误：应收 部分数据, 未提交,未开票或 已销账！" });
                    }
                    var Bms_Bill_Ap_DtlRep = unitOfWork.Repository<Bms_Bill_Ap_Dtl>();
                    //ArrApDtl = Bms_Bill_Ap_DtlRep.Queryable().Where(x => x.OBms_Bill_Ap.Invoice_No == Invoice_No).ToList();
                    var Num = -ArrAp.Count() * 2 - 10;
                    var DtlNum = -ArrAp.Sum(x => x.ArrBms_Bill_Ap_Dtl.Count()) * 2 - 10;
                    var ArrProperty = typeof(Bms_Bill_Ap).GetProperties(System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    var ArrDtlProperty = typeof(Bms_Bill_Ap_Dtl).GetProperties(System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    foreach (var item in ArrAp)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        #region 将账单序号变为负数

                        item.Line_No = -item.Line_No;
                        entry.Property(x => x.Line_No).IsModified = true;
                        item.Cancel_Status = true;
                        entry.Property(x => x.Cancel_Status).IsModified = true;
                        item.Status = AirOutEnumType.UseStatusEnum.Disable;
                        entry.Property(x => x.Status).IsModified = true;

                        #region 复制 2个(账单及其明细) 1个将 金额变为负数 对冲，一个去除任何状态的新数据

                        var ApCopy1 = new Bms_Bill_Ap();//对冲账单
                        var ApCopy2 = new Bms_Bill_Ap();//无状态新账单
                        Common.SetSamaProtity(ApCopy1, item, false, ArrProperty, ArrProperty);
                        Common.SetSamaProtity(ApCopy2, item, false, ArrProperty, ArrProperty);

                        #region 对冲账单 金额变为负数

                        ApCopy1.Id = Num++;
                        var Line_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh);
                        ApCopy1.Line_No = -Convert.ToInt32(Line_No);
                        ApCopy1.Cancel_Status = true;
                        ApCopy1.Status = AirOutEnumType.UseStatusEnum.Disable;
                        ApCopy1.Bill_Date = DateTime.Now;
                        ApCopy1.Bill_Amount = -ApCopy1.Bill_Amount;//价
                        ApCopy1.Bill_TaxAmount = -ApCopy1.Bill_TaxAmount;//税金
                        ApCopy1.Bill_AmountTaxTotal = -ApCopy1.Bill_AmountTaxTotal;//价税合计
                        ApCopy1.Bill_Account2 = -ApCopy1.Bill_Account2;//实际金额
                        ApCopy1.ADDID = "";
                        ApCopy1.ADDTS = null;
                        ApCopy1.ADDWHO = "";
                        ApCopy1.EDITID = "";
                        ApCopy1.EDITTS = null;
                        ApCopy1.EDITWHO = "";
                        if (item.ArrBms_Bill_Ap_Dtl != null && item.ArrBms_Bill_Ap_Dtl.Any())
                        {
                            ArrApDtl = new List<Bms_Bill_Ap_Dtl>();
                            foreach (var OBms_Bill_Ap_Dtl in item.ArrBms_Bill_Ap_Dtl)
                            {
                                var entryDtl = WebdbContxt.Entry<Bms_Bill_Ap_Dtl>(OBms_Bill_Ap_Dtl);
                                entryDtl.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                                OBms_Bill_Ap_Dtl.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                                OBms_Bill_Ap_Dtl.Cancel_Status = true;
                                entryDtl.Property(x => x.Cancel_Status).IsModified = true;
                                OBms_Bill_Ap_Dtl.Status = AirOutEnumType.UseStatusEnum.Disable;
                                entryDtl.Property(x => x.Status).IsModified = true;

                                var ApDtl = new Bms_Bill_Ap_Dtl();
                                ArrApDtl.Add(ApDtl);
                                //反射设置 两个类 相同字段名数据 （效率差-没有直接设置快）
                                Common.SetSamaProtity(ApDtl, OBms_Bill_Ap_Dtl, false, ArrDtlProperty, ArrDtlProperty);
                                ApDtl.Bms_Bill_Ap_Id = ApCopy1.Id;
                                ApDtl.Id = DtlNum++;
                                ApDtl.Line_No = ApCopy1.Line_No;
                                var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh + "_" + -ApCopy1.Line_No, true);
                                ApDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                ApDtl.Cancel_Status = true;
                                ApDtl.Status = AirOutEnumType.UseStatusEnum.Disable;
                                ApDtl.Unitprice2 = -ApDtl.Unitprice2;//单价
                                ApDtl.Account2 = -ApDtl.Account2;//实际金额
                                ApDtl.Bill_Amount = -ApDtl.Bill_Amount;//价
                                ApDtl.Bill_TaxAmount = -ApDtl.Bill_TaxAmount;//税金
                                ApDtl.Bill_AmountTaxTotal = -ApDtl.Bill_AmountTaxTotal;//价税合计
                                ApDtl.ADDID = "";
                                ApDtl.ADDTS = null;
                                ApDtl.ADDWHO = "";
                                ApDtl.EDITID = "";
                                ApDtl.EDITTS = null;
                                ApDtl.EDITWHO = "";
                                ApDtl.OBms_Bill_Ap = null;
                                Bms_Bill_Ap_DtlRep.Insert(ApDtl);
                            }
                            ApCopy1.ArrBms_Bill_Ap_Dtl = ArrApDtl;
                        }

                        #endregion

                        #region 无状态账单

                        ApCopy2.Id = Num++;
                        Line_No = SequenceBuilder.NextBmsBillArLineNo(true, item.Dzbh);
                        ApCopy2.Line_No = Convert.ToInt32(Line_No);
                        ApCopy2.Bill_Date = DateTime.Now;
                        ApCopy2.Status = AirOutEnumType.UseStatusEnum.Enable;
                        ApCopy2.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;
                        ApCopy2.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;

                        ApCopy2.SignIn_ECCNo = null;//ECC发票号
                        ApCopy2.Invoice_MoneyCode = null;//开票币种
                        ApCopy2.Invoice_FeeType = null;//开票费目
                        ApCopy2.Invoice_TaxRateType = null;//开票税率
                        ApCopy2.Invoice_HasTax = false;//含税/不含税
                        ApCopy2.ECC_Status = null;//ECC状态
                        ApCopy2.ECC_StatusMsg = null;//ECC状态信息
                        ApCopy2.ECC_InvoiceSendDate = null;//ECC发票发送时间
                        ApCopy2.ECC_InvoiceRecvDate = null;//ECC发票接收时间        
                        ApCopy2.AuditNo = null;//审核号
                        ApCopy2.AuditId = null;//审核人Id
                        ApCopy2.AuditName = null;//审核人
                        ApCopy2.AuditDate = null;//审核日期
                        ApCopy2.Cancel_Status = false;//作废标志
                        ApCopy2.Cancel_Id = null;//作废人Id
                        ApCopy2.Cancel_Name = null;//作废人
                        ApCopy2.Cancel_Date = null;//作废日期
                        ApCopy2.Sumbmit_Status = false;//提交标志
                        ApCopy2.Sumbmit_No = null;//发票号
                        ApCopy2.Sumbmit_No_Org = null;//原始提交号
                        ApCopy2.Sumbmit_Id = null;//提交人Id
                        ApCopy2.Sumbmit_Name = null;//提交人
                        ApCopy2.Sumbmit_Date = null;//提交日期
                        ApCopy2.SignIn_Status = false;//签收标志
                        ApCopy2.SignIn_No = null;//签收号
                        ApCopy2.SignIn_Id = null;//签收人Id
                        ApCopy2.SignIn_Name = null;//签收人
                        ApCopy2.SignIn_Date = null;//签收日期
                        ApCopy2.Invoice_Status = false;//开票标志
                        ApCopy2.Invoice_No = null;//开票号码
                        ApCopy2.Invoice_Desc = null;//开票名称
                        ApCopy2.Invoice_Id = null;//开票人Id
                        ApCopy2.Invoice_Name = null;//开票人
                        ApCopy2.Invoice_Date = null;//开票日期
                        ApCopy2.Invoice_Remark = null;//开票要求
                        ApCopy2.SellAccount_Status = false;//销账标志
                        ApCopy2.SellAccount_Id = null;//销账人Id
                        ApCopy2.SellAccount_Name = null;//销账人
                        ApCopy2.SellAccount_Date = null;//销账日期

                        ApCopy2.ADDID = "";
                        ApCopy2.ADDTS = null;
                        ApCopy2.ADDWHO = "";
                        ApCopy2.EDITID = "";
                        ApCopy2.EDITTS = null;
                        ApCopy2.EDITWHO = "";
                        if (item.ArrBms_Bill_Ap_Dtl != null && item.ArrBms_Bill_Ap_Dtl.Any())
                        {
                            ArrApDtl = new List<Bms_Bill_Ap_Dtl>();
                            foreach (var OBms_Bill_Ap_Dtl in item.ArrBms_Bill_Ap_Dtl)
                            {
                                var ApDtl = new Bms_Bill_Ap_Dtl();
                                ArrApDtl.Add(ApDtl);
                                //反射设置 两个类 相同字段名数据 （效率差-没有直接设置快）
                                Common.SetSamaProtity(ApDtl, OBms_Bill_Ap_Dtl, false, ArrDtlProperty, ArrDtlProperty);
                                ApDtl.Bms_Bill_Ap_Id = ApCopy2.Id;
                                ApDtl.Id = DtlNum++;
                                ApDtl.Line_No = ApCopy2.Line_No;
                                var DtlLine_No = SequenceBuilder.NextBmsBillArLineNo(true, ApCopy2.Dzbh + "_" + ApCopy2.Line_No, true);
                                ApDtl.Line_Id = Convert.ToInt32(DtlLine_No);
                                ApDtl.Collate_Id = null;//对帐人Id
                                ApDtl.Collate_Name = null;//对帐人
                                ApDtl.Collate_Date = null;//对帐日期
                                ApDtl.Collate_Status = 0;//对帐标志
                                ApDtl.Collate_No = null;//对账号码
                                ApDtl.User_Id = null;//操作人员
                                ApDtl.User_Name = null;//操作人名
                                ApDtl.Create_Status = AirOutEnumType.Bms_BillCreate_Status.HandSet;
                                ApDtl.AuditStatus = AirOutEnumType.AuditStatusEnum.draft;//审批状态", Description = "0:草稿,1:审批中,2:审批通过,-1:审批拒绝
                                ApDtl.AuditNo = null;//审核号
                                ApDtl.Invoice_Status = false;//开票标志
                                ApDtl.Invoice_No = null;//开票号码
                                ApDtl.Sumbmit_Status = false;//提交标志
                                ApDtl.Sumbmit_No = null;//提交号
                                ApDtl.SellAccount_Status = false;//销账标志
                                ApDtl.Cancel_Status = false;//作废标志
                                ApDtl.OBms_Bill_Ap = null;
                                Bms_Bill_Ap_DtlRep.Insert(ApDtl);
                            }
                            ApCopy2.ArrBms_Bill_Ap_Dtl = ArrApDtl;
                        }

                        #endregion

                        bms_Bill_ApService.Insert(ApCopy1);
                        bms_Bill_ApService.Insert(ApCopy2);

                        #endregion

                        #endregion
                    }
                }
                unitOfWork.SaveChanges();

                dynamic OResult = new System.Dynamic.ExpandoObject(); ;
                OResult.Success = true;
                OResult.ErrMsg = "";
                //return Json(OResult);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(OResult), "application/json");
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 取消签收
        /// </summary>
        /// <param name="ArrFinance"></param>
        /// <returns></returns>
        public ActionResult SignInCancel(List<Finance> ArrFinance)
        {
            try
            {
                bool SendECC = true;
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
                Finance OFinance = new Finance();
                bool IsAr = false;

                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为空！" });
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为Null！" });
                }
                var Arr = from p in ArrFinance
                          group p by p.IsAr into g
                          select new
                          {
                              IsAr = g.Key,
                              Num = g.Count()
                          };
                if (Arr.Count() > 1)
                {
                    return Json(new { Success = false, ErrMsg = "错误：不能同时操作 应收/应付数据！" });
                }
                else
                {
                    IsAr = Arr.FirstOrDefault().IsAr;
                    ViewData["IsAr"] = IsAr;
                    var ArrArApId = ArrFinance.Select(x => x.Id);
                    if (IsAr)
                    {
                        return Json(new { Success = false, ErrMsg = "错误：应收 数据,不能签收！" });
                    }
                    else
                    {
                        var SignIn_No = "";
                        var ArrAp_SignIn_No = bms_Bill_ApService.Queryable().AsNoTracking().Where(x => ArrArApId.Contains(x.Id)).Select(x => x.SignIn_No).ToList();
                        var SignIn_NoNum = ArrAp_SignIn_No.Distinct().Count();
                        if (SignIn_NoNum > 1)
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付数据,含有不同的签收号！" });
                        }
                        else
                            SignIn_No = ArrAp_SignIn_No.FirstOrDefault();

                        ArrAp = bms_Bill_ApService.Queryable().Where(x => x.SignIn_No == SignIn_No).ToList();
                        if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据,未审核或已作废 ！" });
                        }
                        if (ArrAp.Any(x => !x.Sumbmit_Status || !x.Invoice_Status || !x.SignIn_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据, 未提交,未开票或 未签收！" });
                        }
                        if (ArrAp.Any(x => x.ECC_InvoiceSendDate != null && x.ECC_Status == null && string.IsNullOrWhiteSpace(x.SignIn_ECCNo)))
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据, 已发送，但还未收到回执或ECC采购订单号为空，不能取消签收！" });
                    }
                }

                #endregion

                #region 数据处理

                //默认 为取消发送ECC 报文
                OFinance.IsAr = IsAr;
                OFinance.Cancel_Date = DateTime.Now;
                OFinance.Cancel_Status = true;//Cancel_Status 和 Status 都为取消时  会发送 ECC取消报文
                OFinance.Status = AirOutEnumType.UseStatusEnum.Disable;
                //设置数据
                var OAp = ArrAp.FirstOrDefault();
                OFinance.SignIn_No = OAp.SignIn_No;
                OFinance.SignIn_ECCNo = OAp.SignIn_ECCNo;
                OFinance.SignIn_Date = OAp.SignIn_Date;
                OFinance.SignIn_Name = OAp.SignIn_Name;
                OFinance.SignIn_Status = OAp.SignIn_Status;
                OFinance.ECC_Status = OAp.ECC_Status;
                OFinance.ECC_StatusMsg = OAp.ECC_StatusMsg;

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;

                foreach (var item in ArrAp)
                {
                    var entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                    entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                    item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                    item.SignIn_No = "";
                    entry.Property(x => x.SignIn_No).IsModified = true;
                    item.SignIn_Status = false;
                    entry.Property(x => x.SignIn_Status).IsModified = true;
                    item.SignIn_Date = null;
                    entry.Property(x => x.SignIn_Date).IsModified = true;
                    item.SignIn_Id = "";
                    entry.Property(x => x.SignIn_Id).IsModified = true;
                    item.SignIn_Name = "";
                    entry.Property(x => x.SignIn_Name).IsModified = true;
                }

                #endregion

                #region 异步 发送ECC 数据存储在Redis

                var SendECCErrMsg = "";
                if (SendECC)
                {
                    var ORet = Send_ECC(OFinance);
                    if (!ORet.Success)
                        SendECCErrMsg = ORet.RetMsg;// "发送ECC错误：未知";
                    else
                    {
                        if (IsAr)
                        {
                            var ArrEditAr = WebdbContxt.ChangeTracker.Entries<Bms_Bill_Ar>().Where(_e => _e.State == EntityState.Modified);
                            foreach (var itemAr in ArrEditAr)
                            {
                                itemAr.Entity.ECC_InvoiceSendDate = DateTime.Now;
                                itemAr.Property(x => x.ECC_InvoiceSendDate).IsModified = true;
                            }
                        }
                        else
                        {
                            var ArrEditAp = WebdbContxt.ChangeTracker.Entries<Bms_Bill_Ap>().Where(_e => _e.State == EntityState.Modified);
                            foreach (var itemAp in ArrEditAp)
                            {
                                itemAp.Entity.ECC_InvoiceSendDate = DateTime.Now;
                                itemAp.Property(x => x.ECC_InvoiceSendDate).IsModified = true;
                            }
                        }
                    }
                }

                #endregion

                unitOfWork.SaveChanges();

                dynamic OResult = new System.Dynamic.ExpandoObject(); ;
                OResult.Success = true;
                OResult.ErrMsg = (string.IsNullOrWhiteSpace(SendECCErrMsg) ? "" : ("保存成功，但 " + SendECCErrMsg));
                OResult.data = new
                {
                    SignIn_No = "",
                    SignIn_Status = false,
                    SignIn_Date = "",
                    SignIn_Id = "",
                    SignIn_Name = ""
                };
                //return Json(OResult);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(OResult), "application/json");
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 应付 签收界面
        /// </summary>
        /// <param name="ArrFinance">应付 账单</param>
        /// <returns></returns>
        public ActionResult ArApSignIn_PopupWin(List<Finance> ArrFinance)
        {
            IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
            IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
            bool IsAr = false;

            #region 验证状态

            if (ArrFinance == null || !ArrFinance.Any())
            {
                ModelState.AddModelError("", "错误：操作数据不能为空！");
                return PartialView("ArApSignIn_PopupWin");
            }
            if (ArrFinance.Any(x => x == null || x.Id <= 0))
            {
                ModelState.AddModelError("", "错误：操作数据不能为Null！");
                return PartialView("ArApSignIn_PopupWin");
            }
            var Arr = from p in ArrFinance
                      group p by p.IsAr into g
                      select new
                      {
                          IsAr = g.Key,
                          Num = g.Count()
                      };
            if (Arr.Count() > 1)
            {
                ModelState.AddModelError("", "错误：不能同时操作 应收/应付数据！");
                return PartialView("ArApSignIn_PopupWin");
            }
            else
            {
                IsAr = Arr.FirstOrDefault().IsAr;
                ViewData["IsAr"] = IsAr;
                var ArrArApId = ArrFinance.Select(x => x.Id);
                if (IsAr)
                {
                    ModelState.AddModelError("", "错误：应收 数据，没有签收功能！");
                    return PartialView("ArApSignIn_PopupWin");
                }
                else
                {
                    ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                    if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || !x.Invoice_Status || x.SignIn_Status))
                    {
                        ModelState.AddModelError("", "错误：应付 部分数据，未审核，已作废，未提交,未开票或已签收！");
                        return PartialView("ArApSignIn_PopupWin");
                    }
                    var GroupArrAp = from p in ArrAp
                                     group p by new { p.Bill_Object_Id, p.Money_Code, p.Invoice_FeeType, p.Invoice_TaxRateType, p.Invoice_MoneyCode } into g
                                     select new
                                     {
                                         Bill_Object_Id = g.Key.Bill_Object_Id,
                                         Money_Code = g.Key.Money_Code,
                                         Invoice_FeeType = g.Key.Invoice_FeeType,
                                         Invoice_TaxRateType = g.Key.Invoice_TaxRateType,
                                         Invoice_MoneyCode = g.Key.Invoice_MoneyCode,
                                         Num = g.Count()
                                     };
                    if (GroupArrAp.Count() > 1)
                    {
                        return Json(new { Success = false, ErrMsg = "错误：应付 数据,结算方,币别，开票费目，开票税率，开票币别不一致！" });
                    }
                }
            }

            #endregion

            Finance OFinance = new Finance();
            OFinance.IsAr = IsAr;
            if (!IsAr)
            {
                //var ArrFeeType = (IEnumerable<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
                var OAp = ArrAp.FirstOrDefault();

                OFinance.Bill_Type = OAp.Bill_Type;
                OFinance.Sumbmit_No = OAp.Sumbmit_No;
                OFinance.Bill_Object_Id = OAp.Bill_Object_Id;
                OFinance.Bill_Object_Name = OAp.Bill_Object_Name;
                OFinance.Money_Code = OAp.Money_Code;
                OFinance.Invoice_MoneyCode = OAp.Invoice_MoneyCode;
                OFinance.Invoice_FeeType = OAp.Invoice_FeeType;
                //var QArrFeeType = ArrFeeType.Where(x => x.FeeCode == OFinance.Invoice_FeeType);
                //if (QArrFeeType.Any())
                //{
                //    var OFeeType = QArrFeeType.FirstOrDefault();
                //    OFinance.Invoice_FeeTypeNAME = OFeeType.FeeName;
                //}
                OFinance.Invoice_TaxRateType = OAp.Invoice_TaxRateType;
                OFinance.Invoice_HasTax = OAp.Invoice_HasTax;
                OFinance.Bill_Account2 = ArrAp.Sum(x => x.Bill_Account2);
                OFinance.SignIn_No = SequenceBuilder.NextSignIn_No();
                OFinance.Invoice_Date = OAp.Invoice_Date;
                OFinance.Invoice_No = OAp.Invoice_No;
            }

            return PartialView("ArApSignIn_PopupWin", OFinance);
        }

        /// <summary>
        /// 应付 签收 保存
        /// </summary>
        /// <param name="OFinance">提交的数据</param>
        /// <param name="ArrFinance">应付 数据集</param>
        /// <returns></returns>
        public ActionResult SaveArApSignIn_PopupWin(Finance OFinance, List<Finance> ArrFinance)
        {
            try
            {
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
                bool IsAr = false;

                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为空！" });
                    //ModelState.AddModelError("", "错误：操作数据不能为空！");
                    //return PartialView("ArApSignIn_PopupWin");
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为Null！" });
                    //ModelState.AddModelError("", "错误：操作数据不能为Null！");
                    //return PartialView("ArApSignIn_PopupWin");
                }
                var Arr = from p in ArrFinance
                          group p by p.IsAr into g
                          select new
                          {
                              IsAr = g.Key,
                              Num = g.Count()
                          };
                if (Arr.Count() > 1)
                {
                    return Json(new { Success = false, ErrMsg = "错误：不能同时操作 应收/应付数据！" });
                    //ModelState.AddModelError("", "错误：不能同时操作 应收/应付数据！");
                    //return PartialView("ArApSignIn_PopupWin");
                }
                else
                {
                    IsAr = Arr.FirstOrDefault().IsAr;
                    ViewData["IsAr"] = IsAr;
                    var ArrArApId = ArrFinance.Select(x => x.Id);
                    if (IsAr)
                    {
                        return Json(new { Success = false, ErrMsg = "错误：应收 数据，没有签收功能！" });
                        //ModelState.AddModelError("", "错误：应收 数据，没有签收功能！");
                        //return PartialView("ArApSignIn_PopupWin");
                    }
                    else
                    {
                        ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || !x.Invoice_Status || x.SignIn_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据，未审核，已作废，未提交,未开票或已签收！" });
                            //ModelState.AddModelError("", "错误：应付 部分数据，未审核，已作废，未提交,未开票或已签收！");
                            //return PartialView("ArApSignIn_PopupWin");
                        }
                        var GroupArrAp = from p in ArrAp
                                         group p by new { p.Bill_Object_Id, p.Money_Code, p.Invoice_FeeType, p.Invoice_TaxRateType, p.Invoice_MoneyCode } into g
                                         select new
                                         {
                                             Bill_Object_Id = g.Key.Bill_Object_Id,
                                             Money_Code = g.Key.Money_Code,
                                             Invoice_FeeType = g.Key.Invoice_FeeType,
                                             Invoice_TaxRateType = g.Key.Invoice_TaxRateType,
                                             Invoice_MoneyCode = g.Key.Invoice_MoneyCode,
                                             Num = g.Count()
                                         };
                        if (GroupArrAp.Count() > 1)
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 数据,结算方,币别，开票费目，开票税率，开票币别不一致！" });
                            //ModelState.AddModelError("", "错误：应付 数据,结算方,币别，开票费目，开票税率，开票币别不一致！");
                            //return PartialView("ArApSignIn_PopupWin");
                        }
                    }
                }

                #endregion

                #region 保存数据

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;

                if (!IsAr)
                {
                    foreach (var item in ArrAp)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.SignIn_No = OFinance.SignIn_No;
                        entry.Property(x => x.SignIn_No).IsModified = true;
                        item.SignIn_Status = true;
                        entry.Property(x => x.SignIn_Status).IsModified = true;
                        item.SignIn_Date = DateTime.Now;
                        entry.Property(x => x.SignIn_Date).IsModified = true;
                        item.SignIn_Id = Utility.CurrentAppUser.Id;
                        entry.Property(x => x.SignIn_Id).IsModified = true;
                        item.SignIn_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(x => x.SignIn_Name).IsModified = true;
                    }
                }

                #endregion

                #region 异步 发送ECC 数据存储在Redis

                var SendECCErrMsg = "";

                OFinance.SignIn_Date = DateTime.Now;
                OFinance.SignIn_Name = Utility.CurrentAppUser.UserNameDesc;
                OFinance.SignIn_Status = true;
                var ORet = Send_ECC(OFinance);
                if (!ORet.Success)
                    SendECCErrMsg = ORet.RetMsg;// "发送ECC错误：未知";
                else
                {
                    if (!IsAr)
                    {
                        var ArrEditAp = WebdbContxt.ChangeTracker.Entries<Bms_Bill_Ap>().Where(_e => _e.State == EntityState.Modified);
                        foreach (var itemAp in ArrEditAp)
                        {
                            itemAp.Entity.ECC_InvoiceSendDate = DateTime.Now;
                            itemAp.Property(x => x.ECC_InvoiceSendDate).IsModified = true;
                        }
                    }
                }

                #endregion

                unitOfWork.SaveChanges();
                if (!string.IsNullOrWhiteSpace(SendECCErrMsg))
                    return Json(new { Success = false, ErrMsg = "保存成功，但 " + SendECCErrMsg });
                else
                {
                    dynamic OResult = new System.Dynamic.ExpandoObject(); ;
                    OResult.Success = true;
                    OResult.ErrMsg = "";
                    OResult.data = new
                    {
                        SignIn_No = OFinance.SignIn_No,
                        SignIn_Status = true,
                        SignIn_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        SignIn_Id = Utility.CurrentAppUser.Id,
                        SignIn_Name = Utility.CurrentAppUser.UserNameDesc
                    };

                    //return Json(OResult);
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(OResult), "application/json");
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 应收/付 销账
        /// </summary>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <returns></returns>
        public ActionResult ArApSellAccount_PopupWin(List<Finance> ArrFinance)
        {
            IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
            IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
            bool IsAr = false;

            #region 验证状态

            if (ArrFinance == null || !ArrFinance.Any())
            {
                ModelState.AddModelError("", "错误：操作数据不能为空！");
                return PartialView("ArApSellAccount_PopupWin");
            }
            if (ArrFinance.Any(x => x == null || x.Id <= 0))
            {
                ModelState.AddModelError("", "错误：操作数据不能为Null！");
                return PartialView("ArApSellAccount_PopupWin");
            }
            var Arr = from p in ArrFinance
                      group p by p.IsAr into g
                      select new
                      {
                          IsAr = g.Key,
                          Num = g.Count()
                      };
            if (Arr.Count() > 1)
            {
                ModelState.AddModelError("", "错误：不能同时操作 应收/应付数据！");
                return PartialView("ArApSellAccount_PopupWin");
            }
            else
            {
                IsAr = Arr.FirstOrDefault().IsAr;
                ViewData["IsAr"] = IsAr;
                var ArrArApId = ArrFinance.Select(x => x.Id);
                if (IsAr)
                {
                    ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                    if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || !x.Invoice_Status))
                    {
                        ModelState.AddModelError("", "错误：应收 部分数据，未审核，已作废，未提交或未开票！");
                        return PartialView("ArApSellAccount_PopupWin");
                    }
                    var GroupArrAr = from p in ArrAr
                                     group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                     select new
                                     {
                                         Money_Code = g.Key.Money_Code,
                                         Bill_Object_Id = g.Key.Bill_Object_Id,
                                         Num = g.Count()
                                     };
                    if (GroupArrAr.Count() > 1)
                    {
                        ModelState.AddModelError("", "错误：应收 数据,结算方/币别 不一致！");
                        return PartialView("ArApSellAccount_PopupWin");
                    }
                }
                else
                {
                    ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                    if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || !x.Invoice_Status))
                    {
                        ModelState.AddModelError("", "错误：应付 部分数据，未审核，已作废，未提交或未开票！");
                        return PartialView("ArApSellAccount_PopupWin");
                    }
                    var GroupArrAp = from p in ArrAp
                                     group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                     select new
                                     {
                                         Money_Code = g.Key.Money_Code,
                                         Bill_Object_Id = g.Key.Bill_Object_Id,
                                         Num = g.Count()
                                     };
                    if (GroupArrAp.Count() > 1)
                    {
                        ModelState.AddModelError("", "错误：应付 数据,结算方/币别 不一致！");
                        return PartialView("ArApSellAccount_PopupWin");
                    }
                }
            }

            #endregion

            Finance OFinance = new Finance();
            OFinance.IsAr = IsAr;
            if (IsAr)
            {
                var TotalBill_Account2 = ArrAr.Sum(x => x.Bill_Account2);
                OFinance.Bill_Account2 = TotalBill_Account2;
                OFinance.Money_Code = ArrAr.FirstOrDefault().Money_Code;
            }
            else
            {
                var TotalBill_Account2 = ArrAp.Sum(x => x.Bill_Account2);
                OFinance.Bill_Account2 = TotalBill_Account2;
                OFinance.Money_Code = ArrAp.FirstOrDefault().Money_Code;
            }

            return PartialView("ArApSellAccount_PopupWin", OFinance);
        }

        /// <summary>
        /// 应收/付 销账
        /// </summary>
        /// <param name="OFinance">提交的数据</param>
        /// <param name="ArrFinance">应收/付 数据集</param>
        /// <returns></returns>
        public ActionResult SaveArApSellAccount_PopupWin(Finance OFinance, List<Finance> ArrFinance)
        {
            try
            {
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                IEnumerable<Bms_Bill_Ar> ArrAr = new List<Bms_Bill_Ar>();
                bool IsAr = false;

                #region 验证状态

                if (ArrFinance == null || !ArrFinance.Any())
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为空！" });
                }
                if (ArrFinance.Any(x => x == null || x.Id <= 0))
                {
                    return Json(new { Success = false, ErrMsg = "错误：操作数据不能为Null！" });
                }
                var Arr = from p in ArrFinance
                          group p by p.IsAr into g
                          select new
                          {
                              IsAr = g.Key,
                              Num = g.Count()
                          };
                if (Arr.Count() > 1)
                {
                    return Json(new { Success = false, ErrMsg = "错误：不能同时操作 应收/应付数据！" });
                }
                else
                {
                    IsAr = Arr.FirstOrDefault().IsAr;
                    ViewData["IsAr"] = IsAr;
                    var ArrArApId = ArrFinance.Select(x => x.Id);
                    if (IsAr)
                    {
                        ArrAr = bms_Bill_ArService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAr.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || !x.Invoice_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 部分数据，未审核，已作废，未提交或未开票！" });
                        }
                        var GroupArrAr = from p in ArrAr
                                         group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                         select new
                                         {
                                             Money_Code = g.Key.Money_Code,
                                             Bill_Object_Id = g.Key.Bill_Object_Id,
                                             Num = g.Count()
                                         };
                        if (GroupArrAr.Count() > 1)
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应收 数据,结算方/币别 不一致！" });
                        }
                    }
                    else
                    {
                        ArrAp = bms_Bill_ApService.Queryable().Where(x => ArrArApId.Contains(x.Id)).ToList();
                        if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status || !x.Invoice_Status))
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 部分数据，未审核，已作废，未提交或未开票！" });
                        }
                        var GroupArrAp = from p in ArrAp
                                         group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                         select new
                                         {
                                             Money_Code = g.Key.Money_Code,
                                             Bill_Object_Id = g.Key.Bill_Object_Id,
                                             Num = g.Count()
                                         };
                        if (GroupArrAp.Count() > 1)
                        {
                            return Json(new { Success = false, ErrMsg = "错误：应付 数据,结算方/币别 不一致！" });
                        }
                    }
                }

                #endregion

                #region 保存数据

                //反向从依赖注入中 取 DbContext
                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;

                if (IsAr)
                {
                    foreach (var item in ArrAr)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ar>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.SellAccount_Status = true;
                        entry.Property(x => x.SellAccount_Status).IsModified = true;
                        item.SellAccount_Date = DateTime.Now;
                        entry.Property(x => x.SellAccount_Date).IsModified = true;
                        item.SellAccount_Id = Utility.CurrentAppUser.Id;
                        entry.Property(x => x.SellAccount_Id).IsModified = true;
                        item.SellAccount_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(x => x.SellAccount_Name).IsModified = true;
                    }
                }
                else
                {
                    foreach (var item in ArrAp)
                    {
                        var entry = WebdbContxt.Entry<Bms_Bill_Ap>(item);
                        entry.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        item.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                        item.SellAccount_Status = true;
                        entry.Property(x => x.SellAccount_Status).IsModified = true;
                        item.SellAccount_Date = DateTime.Now;
                        entry.Property(x => x.SellAccount_Date).IsModified = true;
                        item.SellAccount_Id = Utility.CurrentAppUser.Id;
                        entry.Property(x => x.SellAccount_Id).IsModified = true;
                        item.SellAccount_Name = Utility.CurrentAppUser.UserNameDesc;
                        entry.Property(x => x.SellAccount_Name).IsModified = true;
                    }
                }

                unitOfWork.SaveChanges();

                #endregion

                dynamic OResult = new System.Dynamic.ExpandoObject(); ;
                OResult.Success = true;
                OResult.ErrMsg = "";
                OResult.data = new
                {
                    SellAccount_Status = true,
                    SellAccount_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    SellAccount_Id = Utility.CurrentAppUser.Id,
                    SellAccount_Name = Utility.CurrentAppUser.UserNameDesc
                };

                //return Json(OResult);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(OResult), "application/json");
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 应付 航天增值税
        /// </summary>
        /// <param name="ArrFinance">应付 数据集</param>
        /// <returns></returns>
        public ActionResult ApValAddedTax_PopupWin(Finance OFinance)
        {
            IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
            OPS_EntrustmentInfor OOpsEttInfor = new OPS_EntrustmentInfor();
            CusBusInfo OCusBusInfo = new CusBusInfo();

            #region 验证状态

            if (OFinance == null || OFinance.Id <= 0 || OFinance.Ops_M_OrdId <= 0 || OFinance.IsAr)
            {
                ModelState.AddModelError("", "错误：操作数据不能为空 或只有应付数据，才有此功能！");
                return PartialView("ApValAddedTax_PopupWin");
            }
            if (string.IsNullOrWhiteSpace(OFinance.Sumbmit_No))
            {
                ModelState.AddModelError("", "错误：操作数据 提交号 不能为空！");
                return PartialView("ApValAddedTax_PopupWin");
            }

            ArrAp = bms_Bill_ApService.Queryable().Where(x => x.Sumbmit_No == OFinance.Sumbmit_No).ToList();
            if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status))
            {
                ModelState.AddModelError("", "错误：应付 部分数据，未审核，已作废或未提交！");
                return PartialView("ApValAddedTax_PopupWin");
            }
            var GroupArrAp = from p in ArrAp
                             group p by new { p.Bill_Object_Id, p.Money_Code } into g
                             select new
                             {
                                 Money_Code = g.Key.Money_Code,
                                 Bill_Object_Id = g.Key.Bill_Object_Id,
                                 Num = g.Count()
                             };
            if (GroupArrAp.Count() > 1)
            {
                ModelState.AddModelError("", "错误：应付 数据,结算方/币别 不一致！");
                return PartialView("ApValAddedTax_PopupWin");
            }
            var OPS_EntrustmentInforRep = unitOfWork.Repository<OPS_EntrustmentInfor>();
            OOpsEttInfor = OPS_EntrustmentInforRep.Queryable().Where(x => !x.Is_TG && x.MBLId == OFinance.Ops_M_OrdId).FirstOrDefault();
            if (OOpsEttInfor == null || OOpsEttInfor.Id <= 0)
            {
                ModelState.AddModelError("", "错误：应付 数据,委托 数据不存在或已退关！");
                return PartialView("ApValAddedTax_PopupWin");
            }
            var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OFinance.Bill_Object_Id).FirstOrDefault();
            if (OCusBusInfo == null || OCusBusInfo.Id <= 0)
            {
                ModelState.AddModelError("", "错误：应付 数据,客商（" + OFinance.Bill_Object_Id + "） 数据不存在！");
                return PartialView("ApValAddedTax_PopupWin");
            }

            #endregion

            #region 获取数据

            ApValAddedTax OApValAddedTax = new ApValAddedTax();
            var OAp = ArrAp.FirstOrDefault();
            OApValAddedTax.Ops_M_OrdId = OAp.Ops_M_OrdId;
            OApValAddedTax.MBL = OAp.MBL;
            OApValAddedTax.Bill_Account2 = ArrAp.Sum(x => x.Bill_Account2);
            OApValAddedTax.Money_Code = OAp.Money_Code;
            OApValAddedTax.Sumbmit_No = OAp.Sumbmit_No;
            OApValAddedTax.Bill_Object_Id = OAp.Bill_Object_Id;
            OApValAddedTax.Bill_Object_Name = OAp.Bill_Object_Name;
            OApValAddedTax.Bill_Amount = ArrAp.Sum(x => x.Bill_Amount);
            OApValAddedTax.Bill_TaxAmount = ArrAp.Sum(x => x.Bill_TaxAmount);
            OApValAddedTax.Bill_AmountTaxTotal = ArrAp.Sum(x => x.Bill_AmountTaxTotal);
            OApValAddedTax.Remark = "空运出口/" + OAp.MBL + "/" + OOpsEttInfor.HBL + "/" + (OOpsEttInfor.Flight_Date_Want == null ? "" : ((DateTime)OOpsEttInfor.Flight_Date_Want).ToString("yyyy-MM-dd")) + "/" + OOpsEttInfor.End_Port;
            //银行数据
            OApValAddedTax.UnifiedSocialCreditCode = OCusBusInfo.UnifiedSocialCreditCode;
            OApValAddedTax.InvoiceCountryCode = OCusBusInfo.InvoiceCountryCode;
            OApValAddedTax.InvoiceAddress = OCusBusInfo.InvoiceAddress;
            OApValAddedTax.BankName = OCusBusInfo.BankName;
            OApValAddedTax.BankAccount = OCusBusInfo.BankAccount;

            #endregion

            return PartialView("ApValAddedTax_PopupWin", OApValAddedTax);
        }

        /// <summary>
        /// 应付 航天增值税
        /// </summary>
        /// <param name="ArrFinance">应付 数据集</param>
        /// <returns></returns>
        public ActionResult SaveApValAddedTax_PopupWin(ApValAddedTax OApValAddedTax)
        {
            try
            {
                IEnumerable<Bms_Bill_Ap> ArrAp = new List<Bms_Bill_Ap>();
                OPS_EntrustmentInfor OOpsEttInfor = new OPS_EntrustmentInfor();
                CusBusInfo OCusBusInfo = new CusBusInfo();

                #region 验证状态

                if (OApValAddedTax == null || string.IsNullOrWhiteSpace(OApValAddedTax.Bill_Object_Id) || string.IsNullOrWhiteSpace(OApValAddedTax.Money_Code))
                {
                    ModelState.AddModelError("", "错误：操作数据，结算方，币制 不能为空！");
                    return PartialView("ApValAddedTax_PopupWin");
                }
                if (string.IsNullOrWhiteSpace(OApValAddedTax.Sumbmit_No) || OApValAddedTax.Ops_M_OrdId <= 0)
                {
                    ModelState.AddModelError("", "错误：操作数据 提交号或总单主键 不能为空！");
                    return PartialView("ApValAddedTax_PopupWin");
                }
                else
                {
                    ArrAp = bms_Bill_ApService.Queryable().Where(x => x.Sumbmit_No == OApValAddedTax.Sumbmit_No).ToList();

                    if (ArrAp.Any(x => x.AuditStatus != AirOutEnumType.AuditStatusEnum.AuditSuccess || x.Cancel_Status || !x.Sumbmit_Status))
                    {
                        ModelState.AddModelError("", "错误：应付 部分数据，未审核，已作废或未提交！");
                        return PartialView("ApValAddedTax_PopupWin");
                    }
                    var GroupArrAp = from p in ArrAp
                                     group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                     select new
                                     {
                                         Money_Code = g.Key.Money_Code,
                                         Bill_Object_Id = g.Key.Bill_Object_Id,
                                         Num = g.Count()
                                     };
                    if (GroupArrAp.Count() > 1)
                    {
                        ModelState.AddModelError("", "错误：应付 数据,结算方/币别 不一致！");
                        return PartialView("ArApSellAccount_PopupWin");
                    }
                }
                var OPS_EntrustmentInforRep = unitOfWork.Repository<OPS_EntrustmentInfor>();
                OOpsEttInfor = OPS_EntrustmentInforRep.Queryable().Where(x => !x.Is_TG && x.MBLId == OApValAddedTax.Ops_M_OrdId).FirstOrDefault();
                if (OOpsEttInfor == null || OOpsEttInfor.Id <= 0)
                {
                    ModelState.AddModelError("", "错误：应付 数据,委托 数据不存在或已退关！");
                    return PartialView("ArApSellAccount_PopupWin");
                }
                var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == OApValAddedTax.Bill_Object_Id).FirstOrDefault();
                if (OCusBusInfo == null || OCusBusInfo.Id <= 0)
                {
                    ModelState.AddModelError("", "错误：应付 数据,客商（" + OApValAddedTax.Bill_Object_Id + "） 数据不存在！");
                    return PartialView("ArApSellAccount_PopupWin");
                }

                #endregion

                #region 保存数据

                return Json(new { Success = true, ErrMsg = "" });

                #endregion
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 发送 ECC
        /// </summary>
        /// <param name="Submit_No"></param>
        /// <param name="SignIn_No"></param>
        /// <returns></returns>
        public RetResult Send_ECC(Finance OFinance)
        {
            var ORetResult = new RetResult(true, "");
            try
            {
                if (OFinance.Cancel_Status && OFinance.Status == AirOutEnumType.UseStatusEnum.Disable)
                {
                    if (OFinance.IsAr)
                    {
                        if ((string.IsNullOrWhiteSpace(OFinance.Sumbmit_No) || string.IsNullOrWhiteSpace(OFinance.Sumbmit_ECCNo)))
                        {
                            ORetResult.Success = false;
                            ORetResult.RetMsg = "发送ECC错误：提交号或ECC发票号 为空";
                            return ORetResult;
                        }
                    }
                    else
                    {
                        if ((string.IsNullOrWhiteSpace(OFinance.SignIn_No) || string.IsNullOrWhiteSpace(OFinance.SignIn_ECCNo)))
                        {
                            ORetResult.Success = false;
                            ORetResult.RetMsg = "发送ECC错误：签收号或采购订单号 为空";
                            return ORetResult;
                        }
                    }
                }
                else
                {
                    var ArrBD_DEFDOC_LIST = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
                    var QArrBD_DEFDOC_LIST = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "Bill_TaxRateType" && x.LISTCODE == OFinance.Invoice_TaxRateType);
                    decimal Bill_TaxRate = 0;
                    if (QArrBD_DEFDOC_LIST.Any())
                    {
                        var Bill_TaxRateStr = QArrBD_DEFDOC_LIST.FirstOrDefault().ENAME;
                        decimal.TryParse(Bill_TaxRateStr, out Bill_TaxRate);
                    }
                    else
                    {
                        ORetResult.Success = false;
                        ORetResult.RetMsg = "发送ECC错误：税率无法找到";
                        return ORetResult;
                    }
                    dynamic OCalcTaxRate = bms_Bill_ArService.CalcTaxRate(OFinance.Invoice_HasTax, Bill_TaxRate, OFinance.Bill_Account2);
                    if (OCalcTaxRate.Success)
                    {
                        //价（实际金额）
                        OFinance.Bill_Amount = OCalcTaxRate.Bill_Amount;//净金额-价
                        //税金 （实际金额 * 税率）
                        OFinance.Bill_TaxAmount = OCalcTaxRate.Bill_TaxAmount;
                        //价税合计 (价+税金)
                        OFinance.Bill_AmountTaxTotal = OCalcTaxRate.Bill_AmountTaxTotal;
                    }
                    else
                    {
                        ORetResult.Success = false;
                        ORetResult.RetMsg = "发送ECC错误：重新计算，价，税，价税合计出错";
                        return ORetResult;
                    }
                    var ArrFeeType = (IEnumerable<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);
                    var QArrFeeType = ArrFeeType.Where(x => x.FeeCode == OFinance.Invoice_FeeType);
                    if (QArrFeeType.Any())
                    {
                        var OFeeType = QArrFeeType.FirstOrDefault();
                        OFinance.Invoice_FeeTypeNAME = OFeeType.FeeName;//开票费目名称
                        OFinance.ECC_Code = OFeeType.ECC_Code;//开票费目-开票费用代码-ECC代码
                    }
                    else
                    {
                        ORetResult.Success = false;
                        ORetResult.RetMsg = "发送ECC错误：开票费目名称无法找到";
                        return ORetResult;
                    }
                }
                RedisHelp.RedisHelper ORedisHelper = new RedisHelp.RedisHelper();
                ORedisHelper.ListRightPush<Finance>(Common.RedisKeySendECC, OFinance);
            }
            catch (Exception ex)
            {
                ORetResult.Success = false;
                ORetResult.RetMsg = Common.GetExceptionMsg(ex);
            }

            return ORetResult;
        }

        /// <summary>
        /// 应付对账单(不含税)
        /// </summary>
        /// <param name="ArrFinance">选中的数据</param>
        /// <param name="filterRules">查询条件</param>
        /// <param name="typeName">报表名称</param>
        /// <param name="CheckALLstatus">是否全选</param>
        /// <returns></returns>
        public ActionResult ExportBms_Ap_account(string ArrFinance = "", string filterRules = "", string typeName = "", bool CheckALLstatus = false)
        {
            try
            {
                var Qquery = FinanceService.GetData(filterRules);
                string AddfilterRules = Request["AddfilterRules"] ?? "";

                List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
                if (!string.IsNullOrWhiteSpace(filterRules))
                    Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
                var NewAddfilters = new List<IEnumerable<filterRule>>();
                if (Addfilters != null && Addfilters.Any())
                {
                    foreach (var item in Addfilters)
                    {
                        NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                    }
                }
                if (NewAddfilters != null && NewAddfilters.Any())
                {
                    foreach (var item in NewAddfilters)
                    {
                        if (item != null && item.Any())
                        {
                            var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                            Qquery = Qquery.Union(Qquery1);
                        }
                    }
                }

                var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
                var QResult = Qquery.ToList();

                #region 排除-移除的项

                string RemoveIdStr = Request["RemoveId"] ?? "";
                var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
                if (ArrRemoveId != null && ArrRemoveId.Any())
                {
                    var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                    QResult = (from p in QResult
                               where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                               select p).ToList();
                }

                #endregion

                var OP_ID = QResult.Select(x => x.Dzbh).Distinct();
                var ops_data = _oPS_EntrustmentInforService.Queryable().Where(x => OP_ID.Contains(x.Operation_Id) && x.Is_TG == false).Select(x => x.Operation_Id).ToList();
                QResult = QResult.Where(x => ops_data.Contains(x.Dzbh)).ToList();
                var ArrBMSID = QResult.Where(x => x.IsAr == false).Select(x => x.Id);
                //var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                var strids = "";
                var idarr = strids.Split(",");
                var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();
                var ArrOps_M_ID = data.Select(x => x.MBLId);
                var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
                if (data == null || data.Count == 0)
                {
                    return null;
                }
                var BmsBillApRep = unitOfWork.Repository<Bms_Bill_Ap>().Queryable();
                var BmsBillApDtlRep = unitOfWork.Repository<Bms_Bill_Ap_Dtl>().Queryable();
                var QBmsBillAp = BmsBillApRep.Where(x => idarr.Contains(x.Dzbh)).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();
                var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);

                var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(ArrFinance);
                var ArrRowsBmsid = ArrRows.Select(x => x.Id);
                var Consign_Code = data.Select(x => x.Consign_Code);
                var CCops_en = _oPS_EntrustmentInforService.Queryable().Where(x => Consign_Code.Contains(x.Consign_Code)).Select(x => x.Operation_Id).ToList();
                var NweQBmsBillAp = BmsBillApRep.Where(x => ArrBMSID.Contains(x.Id) && x.Cancel_Status == false).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();
                if (CheckALLstatus == true)
                {
                    NweQBmsBillAp = NweQBmsBillAp.Where(x => ArrBMSID.Contains(x.Id)).ToList();
                }
                else
                    NweQBmsBillAp = NweQBmsBillAp.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();

                //var QBmsBillAr = BmsBillArRep.Where(x => ArrOps_M_ID.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();

                List<Bms_ap_account> TestbmsList = new List<Bms_ap_account>();
                List<Bms_ap_account_HasTax> TestbmsList1 = new List<Bms_ap_account_HasTax>();
                if (!NweQBmsBillAp.Any())
                {
                    return Json(new { Success = false, ErrMsg = "所选账务信息中没有应付信息！" });
                }
                //应付对账单（不含税）
                foreach (var item in NweQBmsBillAp)
                {
                    Bms_ap_account t = new Bms_ap_account();
                    var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                    t.Operation_Id = item.IsMBLJS == true ? "" : item.Dzbh;
                    t.Flight_Date_Want = Qops_en.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_en.Flight_Date_Want).ToString("yyyy-MM-dd");
                    t.MBL = item.MBL == null ? "" : (item.MBL == "-" ? "-" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7));
                    t.Money_Code = item.Money_Code;
                    t.Bill_Account2 = item.Bill_Account2;
                    var Qcus = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                    if (Qcus.Any())
                    {
                        t.Settle_Code = Qcus.FirstOrDefault().EnterpriseName;
                    }
                    else
                        t.Settle_Code = item.Bill_Object_Name;
                    t.Invoice_No = item.Invoice_No;
                    t.Sumbmit_Status = item.Sumbmit_Status;
                    //t.Sumbmit_No_Org = item.Sumbmit_No_Org;
                    var QBmsBillApdtl = item.ArrBms_Bill_Ap_Dtl;
                    t.Bms_ar_dtl = string.Join("\r\n", QBmsBillApdtl.OrderBy(x=>x.Line_Id).ThenBy(x=>x.Line_No).Select(x => x.Charge_Desc + ":" + x.Account2));
                    t.Remark = item.Remark;
                    t.End_Port = Qops_en.End_Port;

                    TestbmsList.AddRange(t);
                }

                string Consign_Name;
                var QCusBus = CusBusInfoRep.Where(x => x.EnterpriseId == NweQBmsBillAp.FirstOrDefault().Bill_Object_Id).ToList();
                if (QCusBus.Any())
                {
                    Consign_Name = QCusBus.FirstOrDefault().EnterpriseName;
                }
                else
                    Consign_Name = NweQBmsBillAp.FirstOrDefault().Bill_Object_Id;
                string outfile = "";
                var outbytes = ExcelHelper.OutPutExcelByArr(TestbmsList.ToList().OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.MBL).ThenBy(x => x.Operation_Id), @"\FileModel\Bms_Ap_account.xlsx", 4, 0, out outfile, "应付对账单");
                var ms = new System.IO.MemoryStream(outbytes);
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
                Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];

                var aaa = "";
                var bbb = "";
                foreach (var item in filters)
                {

                    if (item.field == "_Flight_Date_Want")
                    {
                        aaa = item.value;
                    }
                    if (item.field == "Flight_Date_Want_")
                    {
                        bbb = item.value;
                    }
                }
                sheet.Cells[0, 0].Value = Consign_Name + "空运应付对账单";
                sheet.Cells[1, 0].Value = (aaa == "" ? "" : Convert.ToDateTime(aaa).ToString("yyyyMMdd")) + ((aaa == "" & bbb == "") ? "" : "-") + (bbb == "" ? "" : Convert.ToDateTime(bbb).ToString("yyyyMMdd"));
                //sheet.Cells[1, 0].Value = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("yyyyMMdd") + "-" + DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1).ToString("yyyyMMdd");
                sheet.Cells[3, 8].Value = "打印时间：" + DateTime.Now.ToString("yyyy-MM-dd");
                List<AccountTotal> AccountTotallist = new List<AccountTotal>();
                List<AccountTotal> AccountTotallist1 = new List<AccountTotal>();
                foreach (var item in TestbmsList)
                {
                    AccountTotal t = new AccountTotal();
                    var Qrate = RateRep.Where(x => x.LocalCurrCode == item.Money_Code && x.ForeignCurrCode == "CNY" && x.Year == (item.Flight_Date_Want == null ? DateTime.Now.Year : (Convert.ToDateTime(item.Flight_Date_Want).Year)) && x.Month == (item.Flight_Date_Want == null ? DateTime.Now.Month : (Convert.ToDateTime(item.Flight_Date_Want).Month))).FirstOrDefault();
                    t.Flight_Date_Want = item.Flight_Date_Want;
                    t.Money_Code = item.Money_Code;
                    t.Rate = item.Money_Code == "CNY" ? 1 : (Qrate == null ? 1 : Qrate.RecRate);
                    t.Bill_Accounts = item.Bill_Account2;
                    t.Bill_TaxAmount = 0;
                    t.Bill_AmountTaxTotal = 0;
                    AccountTotallist.AddRange(t);
                }
                var Sumtotal = AccountTotallist.Select(x => x.Rate * x.Bill_Accounts).Sum();
                var CCC = (from p in AccountTotallist
                           group p by p.Money_Code into g
                           select new
                           {
                               Money_Code = g.Key,
                               TOTALBill = g.Sum(x => x.Bill_Accounts),
                           }).ToList();


                var TestbmsListcount = TestbmsList.Count();
                sheet.Cells[TestbmsListcount + 5, 2].Value = "合计";
                //sheet.Cells[TestbmsListcount + 5, 3].Value = "CNY";
                //sheet.Cells[TestbmsListcount + 5, 4].Value = Sumtotal;
                for (var i = 0; i < CCC.Count(); i++)
                {
                    //sheet.Cells.Merge(TestbmsListcount + 5 + i, 3, 1, 2);
                    sheet.Cells[TestbmsListcount + 5 + i, 3].Value = CCC[i].Money_Code;
                    sheet.Cells[TestbmsListcount + 5 + i, 4].Value = Math.Round((decimal)CCC[i].TOTALBill, 2).ToString("#0.00");
                    Aspose.Cells.Style styletotal = workbook.Styles[workbook.Styles.Add()];//新增样式
                    styletotal.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
                    styletotal.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字 靠左   
                    styletotal.Font.Size = 10;
                    styletotal.Font.IsBold = true;
                    styletotal.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    for (var m = 0; m < 11; m++)
                    {
                        sheet.Cells[TestbmsListcount + 5 + i, m].SetStyle(styletotal);
                    }
                }
                sheet.Cells.Columns[0].Width = 9;
                sheet.Cells.Columns[1].Width = 9;
                sheet.Cells.Columns[2].Width = 12;
                sheet.Cells.Columns[3].Width = 3;
                sheet.Cells.Columns[4].Width = 11;
                sheet.Cells.Columns[5].Width = 31;
                sheet.Cells.Columns[6].Width = 10;
                sheet.Cells.Columns[7].Width = 3;
                sheet.Cells.Columns[8].Width = 18;//费用明细
                sheet.Cells.Columns[9].Width = 8;
                sheet.Cells.Columns[10].Width = 6;
                Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];//新增样式
                style.ShrinkToFit = true;
                style.Font.Size = 10;
                style.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字 靠左                             
                style.Font.Color = System.Drawing.Color.Black;
                style.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;

                for (var i = 5; i < TestbmsListcount + 5; i++)
                {
                    sheet.Cells[i, 5].SetStyle(style);
                }

                sheet.AutoFitRows();
                for (var i = 0; i < TestbmsListcount + 5; i++)
                {
                    var test = sheet.Cells.GetRowHeight(i + 4);
                    sheet.Cells.Rows[i + 4].Height = test + 5;
                }

                //for (var i = 4; i < TestbmsListcount + CCC.Count + 5; i++) 
                //{
                //    Aspose.Cells.Style styleBB1 = workbook.Styles[workbook.Styles.Add()];//新增样式
                //    styleBB1.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thick;
                //    styleBB1.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                //    styleBB1.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                //    styleBB1.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                //    Aspose.Cells.Style styleBB2 = workbook.Styles[workbook.Styles.Add()];//新增样式
                //    styleBB1.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                //    styleBB2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thick;
                //    styleBB2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                //    styleBB2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                //    sheet.Cells[i, 0].SetStyle(styleBB1);
                //    sheet.Cells[i, 10].SetStyle(styleBB2);                  
                //}
              
                    if (typeName == "打印应付对账单")
                    {
                        var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                        var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                        string FName = Server.MapPath(dPath + "/pdfDD/" + fileName);
                        workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                    }
                    else
                    {
                        var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                        var newms = Server.MapPath(@"\DownLoad\Bms_Ap_account.xls");
                        workbook.Save(newms);
                        //workbook.Save(newms, Aspose.Cells.SaveFormat.Pdf);
                        return File(newms, "application/vnd.ms-excel", fileName);
                    }

            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
            //return File(FName, "application/octet-stream", fileName);
        }

        /// <summary>
        /// 应付对账单(含税)
        /// </summary>
        /// <param name="ArrFinance">选中的数据</param>
        /// <param name="filterRules">查询条件</param>
        /// <param name="typeName">报表名称</param>
        /// <param name="CheckALLstatus">是否全选</param>
        /// <returns></returns>
        public ActionResult ExportBms_Ap_account_HasTax(string ArrFinance = "", string filterRules = "", string typeName = "", bool CheckALLstatus = false)
        {
            try
            {
                var Qquery = FinanceService.GetData(filterRules);
                string AddfilterRules = Request["AddfilterRules"] ?? "";

                List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
                if (!string.IsNullOrWhiteSpace(filterRules))
                    Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
                var NewAddfilters = new List<IEnumerable<filterRule>>();
                if (Addfilters != null && Addfilters.Any())
                {
                    foreach (var item in Addfilters)
                    {
                        NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                    }
                }
                if (NewAddfilters != null && NewAddfilters.Any())
                {
                    foreach (var item in NewAddfilters)
                    {
                        if (item != null && item.Any())
                        {
                            var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                            Qquery = Qquery.Union(Qquery1);
                        }
                    }
                }
                var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
                var QResult = Qquery.ToList();

                #region 排除-移除的项

                string RemoveIdStr = Request["RemoveId"] ?? "";
                var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
                if (ArrRemoveId != null && ArrRemoveId.Any())
                {
                    var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                    QResult = (from p in QResult
                               where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                               select p).ToList();
                }

                #endregion

                var OP_ID = QResult.Select(x => x.Dzbh).Distinct();
                var ops_data = _oPS_EntrustmentInforService.Queryable().Where(x => OP_ID.Contains(x.Operation_Id) && x.Is_TG == false).Select(x => x.Operation_Id).ToList();
                QResult = QResult.Where(x => ops_data.Contains(x.Dzbh)).ToList();
                var ArrBMSID = QResult.Where(x => x.IsAr == false).Select(x => x.Id);
                //var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                var strids = "";
                var idarr = strids.Split(",");
                var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();
                var ArrOps_M_ID = data.Select(x => x.MBLId);
                var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
                if (data == null || data.Count == 0)
                {
                    return null;
                }
                var BmsBillApRep = unitOfWork.Repository<Bms_Bill_Ap>().Queryable();
                var BmsBillApDtlRep = unitOfWork.Repository<Bms_Bill_Ap_Dtl>().Queryable();
                var QBmsBillAp = BmsBillApRep.Where(x => idarr.Contains(x.Dzbh)).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();
                var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);

                var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(ArrFinance);
                var ArrRowsBmsid = ArrRows.Select(x => x.Id);
                var Consign_Code = data.Select(x => x.Consign_Code);
                var CCops_en = _oPS_EntrustmentInforService.Queryable().Where(x => Consign_Code.Contains(x.Consign_Code)).Select(x => x.Operation_Id).ToList();
                var NweQBmsBillAp = BmsBillApRep.Where(x => ArrBMSID.Contains(x.Id) && x.Cancel_Status == false).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();
                if (CheckALLstatus == true)
                {
                    NweQBmsBillAp = NweQBmsBillAp.Where(x => ArrBMSID.Contains(x.Id)).ToList();
                }
                else
                    NweQBmsBillAp = NweQBmsBillAp.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();
                //var QBmsBillAr = BmsBillArRep.Where(x => ArrOps_M_ID.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();

                List<Bms_ap_account> TestbmsList = new List<Bms_ap_account>();
                List<Bms_ap_account_HasTax> TestbmsList1 = new List<Bms_ap_account_HasTax>();
                if (!NweQBmsBillAp.Any())
                {
                    return Json(new { Success = false, ErrMsg = "所选账务信息中没有应付信息！" });
                }

                //应付对账单（含税）
                foreach (var item in NweQBmsBillAp)
                {
                    Bms_ap_account_HasTax t = new Bms_ap_account_HasTax();
                    var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                    t.Operation_Id = item.IsMBLJS == true ? "" : item.Dzbh;
                    t.Flight_Date_Want = Qops_en.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_en.Flight_Date_Want).ToString("yyyy-MM-dd");
                    t.MBL = item.MBL == null ? "" : (item.MBL == "-" ? "-" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7));
                    t.Money_Code = item.Money_Code;
                    t.Bill_Account2 = item.Bill_Account2;
                    t.Bill_TaxAmount = item.Bill_TaxAmount;
                    t.Bill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                    var Qcus = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                    if (Qcus.Any())
                    {
                        t.Settle_Code = Qcus.FirstOrDefault().EnterpriseName;
                    }
                    else
                        t.Settle_Code = item.Bill_Object_Name;
                    t.Invoice_No = item.Invoice_No;
                    t.Sumbmit_Status = item.Sumbmit_Status;
                    //t.Sumbmit_No_Org = item.Sumbmit_No_Org;
                    var QBmsBillApdtl = item.ArrBms_Bill_Ap_Dtl;
                    t.Bms_ar_dtl = string.Join("\r\n", QBmsBillApdtl.OrderBy(x => x.Line_Id).ThenBy(x => x.Line_No).Select(x => x.Charge_Desc + ":" + x.Account2));
                    t.Remark = item.Remark;
                    t.End_Port = Qops_en.End_Port;
                    TestbmsList1.AddRange(t);
                }

                //var QCusBus = CusBusInfoRep.Where(x => x.EnterpriseId == NweQBmsBillAp.FirstOrDefault().Bill_Object_Id).ToList();
                var QCusBus = CusBusInfoRep.Where(x => x.EnterpriseId == NweQBmsBillAp.FirstOrDefault().Bill_Object_Id).ToList();
                string Consign_Name;
                if (QCusBus.Any())
                {
                    Consign_Name = QCusBus.FirstOrDefault().EnterpriseName;
                }
                else
                    Consign_Name = NweQBmsBillAp.FirstOrDefault().Bill_Object_Name;
                string outfile = "";
                var outbytes = ExcelHelper.OutPutExcelByArr(TestbmsList1.ToList().OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.MBL).ThenBy(x => x.Operation_Id), @"\FileModel\Bms_Ap_account_HasTax.xlsx", 4, 0, out outfile, "应付对账单(含税)");
                var ms = new System.IO.MemoryStream(outbytes);
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
                //Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];

                var aaa = "";
                var bbb = "";
                foreach (var item in filters)
                {

                    if (item.field == "_Flight_Date_Want")
                    {
                        aaa = item.value;
                    }
                    if (item.field == "Flight_Date_Want_")
                    {
                        bbb = item.value;
                    }
                }
                //sheet.Cells[0, 0].Value = Consign_Name + "空运应付对账单";
                //sheet.Cells[1, 0].Value = (aaa == "" ? "" : Convert.ToDateTime(aaa).ToString("yyyyMMdd")) + ((aaa == "" & bbb == "") ? "" : "-") + (bbb == "" ? "" : Convert.ToDateTime(bbb).ToString("yyyyMMdd"));
                ////sheet.Cells[1, 0].Value = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("yyyyMMdd") + "-" + DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1).ToString("yyyyMMdd");
                //sheet.Cells[3, 9].Value = "打印时间：" + DateTime.Now.ToString("yyyy-MM-dd");
                //List<AccountTotal> AccountTotallist = new List<AccountTotal>();
                List<AccountTotal> AccountTotallist1 = new List<AccountTotal>();

                Aspose.Cells.Worksheet sheet1 = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                sheet1.Cells[0, 0].Value = Consign_Name + "空运应付对账单";
                sheet1.Cells[1, 0].Value = (aaa == "" ? "" : Convert.ToDateTime(aaa).ToString("yyyyMMdd")) + ((aaa == "" & bbb == "") ? "" : "-") + (bbb == "" ? "" : Convert.ToDateTime(bbb).ToString("yyyyMMdd"));
                //sheet1.Cells[1, 0].Value = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("yyyyMMdd") + "-" + DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1).ToString("yyyyMMdd");
                sheet1.Cells[3, 9].Value = "打印时间：" + DateTime.Now.ToString("yyyy-MM-dd");
                foreach (var item in TestbmsList1)
                {
                    AccountTotal t = new AccountTotal();
                    var Qrate = RateRep.Where(x => x.LocalCurrCode == item.Money_Code && x.ForeignCurrCode == "CNY" && x.Year == (item.Flight_Date_Want == null ? DateTime.Now.Year : (Convert.ToDateTime(item.Flight_Date_Want).Year)) && x.Month == (item.Flight_Date_Want == null ? DateTime.Now.Month : (Convert.ToDateTime(item.Flight_Date_Want).Month))).FirstOrDefault();
                    t.Flight_Date_Want = item.Flight_Date_Want;
                    t.Money_Code = item.Money_Code;
                    t.Rate = item.Money_Code == "CNY" ? 1 : (Qrate == null ? 1 : Qrate.RecRate);
                    t.Bill_Accounts = item.Bill_Account2;
                    t.Bill_TaxAmount = item.Bill_TaxAmount;
                    t.Bill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                    AccountTotallist1.AddRange(t);
                }
                var Sumtotal1 = AccountTotallist1.Select(x => x.Rate * x.Bill_Accounts).Sum();
                var SumtotalTax = AccountTotallist1.Select(x => x.Rate * x.Bill_TaxAmount).Sum();
                var SumAmountTaxTotal = AccountTotallist1.Select(x => x.Rate * x.Bill_AmountTaxTotal).Sum();
                var CCC = (from p in AccountTotallist1
                           group p by p.Money_Code into g
                           select new
                           {
                               Money_Code = g.Key,
                               TOTALBill_Accounts = g.Sum(x => x.Bill_Accounts),
                               TOTALBill_TaxAmount = g.Sum(x => x.Bill_TaxAmount),
                               TOTALBill_AmountTaxTotal = g.Sum(x => x.Bill_AmountTaxTotal),
                           }).ToList();

                var TestbmsListcount1 = TestbmsList1.Count();
                sheet1.Cells[TestbmsListcount1 + 5, 2].Value = "合计";
                //sheet1.Cells[TestbmsListcount1 + 5, 3].Value = "CNY";
                //sheet1.Cells[TestbmsListcount1 + 5, 4].Value = Sumtotal1;
                //sheet1.Cells[TestbmsListcount1 + 5, 5].Value = SumtotalTax;
                //sheet1.Cells[TestbmsListcount1 + 5, 6].Value = SumAmountTaxTotal;
                for (var i = 0; i < CCC.Count(); i++)
                {
                    //sheet1.Cells.Merge(TestbmsListcount1 + 5 + i, 3, 1, 5);
                    sheet1.Cells[TestbmsListcount1 + 5 + i, 3].Value = CCC[i].Money_Code;
                    sheet1.Cells[TestbmsListcount1 + 5 + i, 4].Value = Math.Round((decimal)CCC[i].TOTALBill_Accounts, 2).ToString("#0.00");
                    sheet1.Cells[TestbmsListcount1 + 5 + i, 5].Value = Math.Round((decimal)CCC[i].TOTALBill_TaxAmount, 2).ToString("#0.00");
                    sheet1.Cells[TestbmsListcount1 + 5 + i, 6].Value = Math.Round((decimal)CCC[i].TOTALBill_AmountTaxTotal, 2).ToString("#0.00");
                    Aspose.Cells.Style styletotal = workbook.Styles[workbook.Styles.Add()];//新增样式
                    styletotal.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
                    styletotal.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字 靠左   
                    styletotal.Font.Size = 10;
                    styletotal.Font.IsBold = true;
                    styletotal.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    for (var m = 0; m < 13; m++)
                    {
                        sheet1.Cells[TestbmsListcount1 + 5 + i, m].SetStyle(styletotal);
                    }
                }
                //for (var i = 0; i < CCC.Count(); i++)
                //{
                //    sheet1.Cells.Merge(TestbmsListcount1 + 5 + i, 3, 1, 5);
                //    //sheet1.Cells[TestbmsListcount1 + 6 + i, 4].Value = CCC[i].Money_Code + ":" + CCC[i].TOTALBill_Accounts;Math.Round((decimal)CCC[i].TOTALBill, 2).ToString("#0.00")
                //    //sheet1.Cells[TestbmsListcount1 + 6 + i, 5].Value = CCC[i].Money_Code + ":" + CCC[i].TOTALBill_TaxAmount;
                //    //sheet1.Cells[TestbmsListcount1 + 6 + i, 6].Value = CCC[i].Money_Code + ":" + CCC[i].TOTALBill_AmountTaxTotal;
                //    sheet1.Cells[TestbmsListcount1 + 5 + i, 3].Value = CCC[i].Money_Code + ":" + Math.Round((decimal)CCC[i].TOTALBill_Accounts,2).ToString("#0.00") + "   " + CCC[i].Money_Code + ":" + Math.Round((decimal)CCC[i].TOTALBill_TaxAmount, 2).ToString("#0.00") + "   " + CCC[i].Money_Code + ":" + Math.Round((decimal)CCC[i].TOTALBill_AmountTaxTotal,2).ToString("#0.00");

                //}

                sheet1.Cells.Columns[0].Width = 9;
                sheet1.Cells.Columns[1].Width = 9;
                sheet1.Cells.Columns[2].Width = 12;
                sheet1.Cells.Columns[3].Width = 3;
                sheet1.Cells.Columns[4].Width = 10;
                sheet1.Cells.Columns[5].Width = 7;
                sheet1.Cells.Columns[6].Width = 10;
                sheet1.Cells.Columns[7].Width = 20;
                sheet1.Cells.Columns[8].Width = 10;
                sheet1.Cells.Columns[9].Width = 3;
                sheet1.Cells.Columns[10].Width = 16;//费用明细
                sheet1.Cells.Columns[11].Width = 7;
                sheet1.Cells.Columns[12].Width = 4;
                sheet1.AutoFitRows();
                Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];//新增样式
                style.ShrinkToFit = true;
                style.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字 靠左                             
                style.Font.Size = 10;
                style.Font.Color = System.Drawing.Color.Black;
                style.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                for (var i = 5; i < TestbmsListcount1 + 5; i++)
                {
                    sheet1.Cells[i, 7].SetStyle(style);
                }
                for (var i = 0; i < TestbmsListcount1 + 5; i++)
                {
                    var test = sheet1.Cells.GetRowHeight(i + 4);
                    sheet1.Cells.Rows[i + 4].Height = test + 5;
                }
                //for (var i = 0; i < TestbmsListcount1 + 5; i++)
                //{
                //    sheet1.AutoFitRow(i);
                //}

                //var newms = Server.MapPath(@"\DownLoad\111.xls");
                if (typeName == "打印应付对账单(含税)")
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    string FName = Server.MapPath(dPath + "/pdfDD/" + fileName);
                    workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                }
                else
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    var newms = Server.MapPath(@"\DownLoad\Bms_Ap_account_HasTax.xls");
                    workbook.Save(newms);
                    //workbook.Save(newms, Aspose.Cells.SaveFormat.Pdf);
                    return File(newms, "application/vnd.ms-excel", fileName);
                }

            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
            //return File(FName, "application/octet-stream", fileName);
        }

        /// <summary>
        /// 应收对账单(不含税)
        /// </summary>
        /// <param name="ArrFinance">选中的数据</param>
        /// <param name="filterRules">查询条件</param>
        /// <param name="typeName">报表名称</param>
        /// <param name="CheckALLstatus">是否全选</param>
        /// <returns></returns>
        public ActionResult ExportBms_Ar_account(string ArrFinance = "", string filterRules = "", string typeName = "", bool CheckALLstatus = false)
        {
            try
            {
                if (filterRules == "undefined" || filterRules == null)
                {
                    return Json(new { Success = false, ErrMsg = "搜索条件为空！" });
                }
                var Qquery = FinanceService.GetData(filterRules);
                string AddfilterRules = Request["AddfilterRules"] ?? "";

                List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
                if (!string.IsNullOrWhiteSpace(filterRules))
                    Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
                var NewAddfilters = new List<IEnumerable<filterRule>>();
                if (Addfilters != null && Addfilters.Any())
                {
                    foreach (var item in Addfilters)
                    {
                        NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                    }
                }
                if (NewAddfilters != null && NewAddfilters.Any())
                {
                    foreach (var item in NewAddfilters)
                    {
                        if (item != null && item.Any())
                        {
                            var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                            Qquery = Qquery.Union(Qquery1);
                        }
                    }
                }
                var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
                var QResult = Qquery.ToList();

                #region 排除-移除的项

                string RemoveIdStr = Request["RemoveId"] ?? "";
                var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
                if (ArrRemoveId != null && ArrRemoveId.Any())
                {
                    var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                    QResult = (from p in QResult
                               where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                               select p).ToList();
                }

                #endregion

                var OP_ID = QResult.Select(x => x.Dzbh).Distinct();
                var ops_data = _oPS_EntrustmentInforService.Queryable().Where(x => OP_ID.Contains(x.Operation_Id) && x.Is_TG == false).Select(x => x.Operation_Id).ToList();
                QResult = QResult.Where(x => ops_data.Contains(x.Dzbh)).ToList();
                //if (QResult.Count == 0) 
                //{
                //    return Json(new { Success = false, ErrMsg = "无查询数据！" });
                //}
                var ArrBMSID = QResult.Where(x => x.IsAr == true).Select(x => x.Id);
                //var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                var strids = "";
                foreach (var item in filterRules)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");
                var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();
                var ArrOps_M_ID = data.Select(x => x.MBLId);
                var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
                var BmsBillArRep = unitOfWork.Repository<Bms_Bill_Ar>().Queryable();
                var BmsBillArDtlRep = unitOfWork.Repository<Bms_Bill_Ar_Dtl>().Queryable();
                var QBmsBillAr = BmsBillArRep.Where(x => idarr.Contains(x.Dzbh)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);

                var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(ArrFinance);
                var ArrRowsBmsid = ArrRows.Select(x => x.Id);
                var OPS_EnName = data.Select(x => x.Entrustment_Name);
                var OPS_DZBH = _oPS_EntrustmentInforService.Queryable().Where(x => OPS_EnName.Contains(x.Entrustment_Name)).Select(x => x.Operation_Id).ToList();
                var NweQBmsBillAr = BmsBillArRep.Where(x => ArrBMSID.Contains(x.Id) && x.Cancel_Status == false).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                if (CheckALLstatus == true)
                {
                    NweQBmsBillAr = NweQBmsBillAr.Where(x => ArrBMSID.Contains(x.Id)).ToList();
                }
                else
                    NweQBmsBillAr = NweQBmsBillAr.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();


                //var QBmsBillAr = BmsBillArRep.Where(x => ArrOps_M_ID.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                if (!NweQBmsBillAr.Any())
                {
                    return Json(new { Success = false, ErrMsg = "所选账务信息中没有应收信息！" });
                }
                List<Bms_ar_account> TestbmsList = new List<Bms_ar_account>();
                List<Bms_ar_account_HasTax> TestbmsList1 = new List<Bms_ar_account_HasTax>();

                //应收对账单（不含税）
                foreach (var item in NweQBmsBillAr)
                {
                    Bms_ar_account t = new Bms_ar_account();
                    var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                    t.Operation_Id = item.Dzbh;
                    t.Flight_Date_Want = Qops_en.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_en.Flight_Date_Want).ToString("yyyy-MM-dd");
                    t.MBL = item.MBL == null ? "" : (item.MBL == "-" ? "-" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7));
                    t.Money_Code = item.Money_Code;
                    t.Bill_Account2 = item.Bill_Account2;
                    var Qcus = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                    if (Qcus.Any())
                    {
                        t.Settle_Code = Qcus.FirstOrDefault().EnterpriseName;
                    }
                    else
                        t.Settle_Code = item.Bill_Object_Name;
                    t.Invoice_No = item.Invoice_No;
                    t.Sumbmit_Status = item.SellAccount_Status;
                    t.Sumbmit_No_Org = item.SellAccount_Name;
                    var QBmsBillArdtl = item.ArrBms_Bill_Ar_Dtl;
                    t.Bms_ar_dtl = string.Join("\r\n", QBmsBillArdtl.OrderBy(x => x.Line_Id).ThenBy(x => x.Line_No).Select(x => x.Charge_Desc + ":" + x.Account2));
                    t.Remark = item.Remark;
                    t.End_Port = Qops_en.End_Port;

                    TestbmsList.AddRange(t);
                }

                //var QCusBus = CusBusInfoRep.Where(x => x.EnterpriseId == NweQBmsBillAr.FirstOrDefault().Bill_Object_Id).ToList();
                string EnterpriseName;
                var QCusBus = CusBusInfoRep.Where(x => x.EnterpriseId == NweQBmsBillAr.FirstOrDefault().Bill_Object_Id).ToList();
                if (QCusBus.Any())
                {
                    EnterpriseName = QCusBus.FirstOrDefault().EnterpriseName;
                }
                else
                    EnterpriseName = NweQBmsBillAr.FirstOrDefault().Bill_Object_Name;
                string outfile = "";
                var outbytes = ExcelHelper.OutPutExcelByArr(TestbmsList.ToList().OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.MBL).ThenBy(x => x.Operation_Id), @"\FileModel\Bms_Ar_account.xlsx", 4, 0, out outfile, "应付对账单");
                var ms = new System.IO.MemoryStream(outbytes);
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);

                //不含税
                Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                //if (filterRules.Contains("Entrustment_Name"))
                //{
                //    sheet.Cells[0, 0].Value = EnterpriseName + "空运应付对账单"; ;
                //}
                var aaa = "";
                var bbb = "";
                foreach (var item in filters)
                {

                    if (item.field == "_Flight_Date_Want")
                    {
                        aaa = item.value;
                    }
                    if (item.field == "Flight_Date_Want_")
                    {
                        bbb = item.value;
                    }
                }
                sheet.Cells[0, 0].Value = EnterpriseName + "空运应收对账单"; ;
                sheet.Cells[1, 0].Value = (aaa == "" ? "" : Convert.ToDateTime(aaa).ToString("yyyyMMdd")) + ((aaa == "" & bbb == "") ? "" : "-") + (bbb == "" ? "" : Convert.ToDateTime(bbb).ToString("yyyyMMdd"));
                //sheet.Cells[1, 0].Value = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("yyyyMMdd") + "-" + DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1).ToString("yyyyMMdd");
                sheet.Cells[3, 9].Value = "打印时间：" + DateTime.Now.ToString("yyyy-MM-dd");
                //var aabb = RateRep.Where(x => x.LocalCurrCode == itemm.Money_Code && x.ForeignCurrCode == "CNY" && x.Year == (item.Flight_Date_Want == null ? DateTime.Now.Year : ((DateTime)item.Flight_Date_Want).Year) && x.Month == (item.Flight_Date_Want == null ? DateTime.Now.Month : ((DateTime)item.Flight_Date_Want).Month)).FirstOrDefault();
                List<AccountTotal> AccountTotallist = new List<AccountTotal>();
                List<AccountTotal> AccountTotallist1 = new List<AccountTotal>();
                foreach (var item in TestbmsList)
                {
                    AccountTotal t = new AccountTotal();
                    var Qrate = RateRep.Where(x => x.LocalCurrCode == item.Money_Code && x.ForeignCurrCode == "CNY" && x.Year == ((item.Flight_Date_Want == null || item.Flight_Date_Want == "") ? DateTime.Now.Year : (Convert.ToDateTime(item.Flight_Date_Want).Year)) && x.Month == ((item.Flight_Date_Want == null || item.Flight_Date_Want == "") ? DateTime.Now.Month : (Convert.ToDateTime(item.Flight_Date_Want).Month))).FirstOrDefault();
                    t.Flight_Date_Want = item.Flight_Date_Want;
                    t.Money_Code = item.Money_Code;
                    t.Rate = item.Money_Code == "CNY" ? 1 : (Qrate == null ? 1 : Qrate.RecRate);
                    t.Bill_Accounts = item.Bill_Account2;
                    t.Bill_TaxAmount = 0;
                    t.Bill_AmountTaxTotal = 0;
                    AccountTotallist.AddRange(t);
                }
                var Sumtotal = AccountTotallist.Select(x => x.Rate * x.Bill_Accounts).Sum();
                //Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];//新增样式
                //style.Font.Color = System.Drawing.Color.Blue;
                //style.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;//文字居中
                var TestbmsListcount = TestbmsList.Count();
                var CCC = (from p in AccountTotallist
                           group p by p.Money_Code into g
                           select new
                           {
                               Money_Code = g.Key,
                               TOTALBill = g.Sum(x => x.Bill_Accounts),
                           }).ToList();

                sheet.Cells[TestbmsListcount + 5, 2].Value = "合计";
                //sheet.Cells[TestbmsListcount + 5, 3].Value = "CNY";
                //sheet.Cells.Merge(TestbmsListcount + 5, 4, 1, 2);
                //sheet.Cells[TestbmsListcount + 5, 4].Value = Sumtotal;
                //sheet.Cells[7, 7].SetStyle(style);

                for (var i = 0; i < CCC.Count(); i++)
                {
                    //sheet.Cells.Merge(TestbmsListcount + 5 + i, 3, 1, 2);
                    sheet.Cells[TestbmsListcount + 5 + i, 3].Value = CCC[i].Money_Code;
                    sheet.Cells[TestbmsListcount + 5 + i, 4].Value = Math.Round((decimal)CCC[i].TOTALBill, 2).ToString("#0.00");
                    Aspose.Cells.Style styletotal = workbook.Styles[workbook.Styles.Add()];//新增样式
                    styletotal.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
                    styletotal.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字 靠左   
                    styletotal.Font.Size = 10;
                    styletotal.Font.IsBold = true;
                    styletotal.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    for (var m = 0; m < 12; m++)
                    {
                        sheet.Cells[TestbmsListcount + 5 + i, m].SetStyle(styletotal);
                    }
                }

                sheet.Cells.Columns[0].Width = 9;
                sheet.Cells.Columns[1].Width = 9;
                sheet.Cells.Columns[2].Width = 12;
                sheet.Cells.Columns[3].Width = 3;
                sheet.Cells.Columns[4].Width = 10;
                sheet.Cells.Columns[5].Width = 25;
                sheet.Cells.Columns[6].Width = 12;
                sheet.Cells.Columns[7].Width = 3;
                sheet.Cells.Columns[8].Width = 8;
                sheet.Cells.Columns[9].Width = 16;//费用明细
                sheet.Cells.Columns[10].Width = 8;
                sheet.Cells.Columns[11].Width = 5;
                sheet.AutoFitRows();
                Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];//新增样式
                style.ShrinkToFit = true;
                style.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字 靠左                             
                style.Font.Size = 10;
                style.Font.Color = System.Drawing.Color.Black;
                style.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                for (var i = 5; i < TestbmsListcount + 5; i++)
                {
                    sheet.Cells[i, 5].SetStyle(style);
                }
                for (var i = 0; i < TestbmsListcount + 5; i++)
                {
                    var test = sheet.Cells.GetRowHeight(i + 4);
                    sheet.Cells.Rows[i + 4].Height = test + 5;
                }

                if (typeName == "打印应收对账单")
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    string FName = Server.MapPath(dPath + "/pdfDD/" + fileName);
                    workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                }
                else
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    var newms = Server.MapPath(@"\DownLoad\Bms_Ar_account.xls");
                    workbook.Save(newms);
                    //workbook.Save(newms, Aspose.Cells.SaveFormat.Pdf);
                    return File(newms, "application/vnd.ms-excel", fileName);
                }

            }

            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 应付对账单(含税)
        /// </summary>
        /// <param name="ArrFinance">选中的数据</param>
        /// <param name="filterRules">查询条件</param>
        /// <param name="typeName">报表名称</param>
        /// <param name="CheckALLstatus">是否全选</param>
        /// <returns></returns>
        public ActionResult ExportBms_Ar_account_HasTax(string ArrFinance = "", string filterRules = "", string typeName = "", bool CheckALLstatus = false)
        {
            try
            {
                if (filterRules == "undefined" || filterRules == null)
                {
                    return Json(new { Success = false, ErrMsg = "搜索条件为空！" });
                }
                var Qquery = FinanceService.GetData(filterRules);
                string AddfilterRules = Request["AddfilterRules"] ?? "";

                List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
                if (!string.IsNullOrWhiteSpace(filterRules))
                    Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
                var NewAddfilters = new List<IEnumerable<filterRule>>();
                if (Addfilters != null && Addfilters.Any())
                {
                    foreach (var item in Addfilters)
                    {
                        NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                    }
                }
                if (NewAddfilters != null && NewAddfilters.Any())
                {
                    foreach (var item in NewAddfilters)
                    {
                        if (item != null && item.Any())
                        {
                            var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                            Qquery = Qquery.Union(Qquery1);
                        }
                    }
                }
                var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
                var QResult = Qquery.ToList();

                #region 排除-移除的项

                string RemoveIdStr = Request["RemoveId"] ?? "";
                var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
                if (ArrRemoveId != null && ArrRemoveId.Any())
                {
                    var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                    QResult = (from p in QResult
                               where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                               select p).ToList();
                }

                #endregion

                var OP_ID = QResult.Select(x => x.Dzbh).Distinct();
                var ops_data = _oPS_EntrustmentInforService.Queryable().Where(x => OP_ID.Contains(x.Operation_Id) && x.Is_TG == false).Select(x => x.Operation_Id).ToList();
                QResult = QResult.Where(x => ops_data.Contains(x.Dzbh)).ToList();
                var ArrBMSID = QResult.Where(x => x.IsAr == true).Select(x => x.Id);
                var strids = "";
                foreach (var item in filterRules)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");
                var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();
                var ArrOps_M_ID = data.Select(x => x.MBLId);
                var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
                var BmsBillArRep = unitOfWork.Repository<Bms_Bill_Ar>().Queryable();
                var BmsBillArDtlRep = unitOfWork.Repository<Bms_Bill_Ar_Dtl>().Queryable();
                var QBmsBillAr = BmsBillArRep.Where(x => idarr.Contains(x.Dzbh)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);

                var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(ArrFinance);
                var ArrRowsBmsid = ArrRows.Select(x => x.Id);
                var OPS_EnName = data.Select(x => x.Entrustment_Name);
                var OPS_DZBH = _oPS_EntrustmentInforService.Queryable().Where(x => OPS_EnName.Contains(x.Entrustment_Name)).Select(x => x.Operation_Id).ToList();
                var NweQBmsBillAr = BmsBillArRep.Where(x => ArrBMSID.Contains(x.Id) && x.Cancel_Status == false).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                if (CheckALLstatus == true)
                {
                    NweQBmsBillAr = NweQBmsBillAr.Where(x => ArrBMSID.Contains(x.Id)).ToList();
                }
                else
                    NweQBmsBillAr = NweQBmsBillAr.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();

                //var QBmsBillAr = BmsBillArRep.Where(x => ArrOps_M_ID.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                if (!NweQBmsBillAr.Any())
                {
                    return Json(new { Success = false, ErrMsg = "所选账务信息中没有应收信息！" });
                }
                List<Bms_ar_account> TestbmsList = new List<Bms_ar_account>();
                List<Bms_ar_account_HasTax> TestbmsList1 = new List<Bms_ar_account_HasTax>();

                //应收对账单（含税）
                foreach (var item in NweQBmsBillAr)
                {
                    Bms_ar_account_HasTax t = new Bms_ar_account_HasTax();
                    var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                    t.Operation_Id = item.Dzbh;
                    t.Flight_Date_Want = Qops_en.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_en.Flight_Date_Want).ToString("yyyy-MM-dd");
                    t.MBL = item.MBL == null ? "" : (item.MBL == "-" ? "-" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7));
                    t.Money_Code = item.Money_Code;
                    t.Bill_Account2 = item.Bill_Account2;
                    t.Bill_TaxAmount = item.Bill_TaxAmount;
                    t.Bill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                    var Qcus = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                    if (Qcus.Any())
                    {
                        t.Settle_Code = Qcus.FirstOrDefault().EnterpriseName;
                    }
                    else
                        t.Settle_Code = item.Bill_Object_Name;
                    t.Invoice_No = item.Invoice_No;
                    t.Sumbmit_Status = item.SellAccount_Status;
                    t.Sumbmit_No_Org = item.SellAccount_Name;
                    var QBmsBillArdtl = item.ArrBms_Bill_Ar_Dtl;
                    t.Bms_ar_dtl = string.Join("\r\n", QBmsBillArdtl.OrderBy(x => x.Line_Id).ThenBy(x => x.Line_No).Select(x => x.Charge_Desc + ":" + x.Account2));
                    t.Remark = item.Remark;
                    t.End_Port = Qops_en.End_Port;
                    TestbmsList1.AddRange(t);
                }

                //var QCusBus = CusBusInfoRep.Where(x => x.EnterpriseId == NweQBmsBillAr.FirstOrDefault().Bill_Object_Id).ToList();
                string EnterpriseName;
                var QCusBus = CusBusInfoRep.Where(x => x.EnterpriseId == NweQBmsBillAr.FirstOrDefault().Bill_Object_Id).ToList();
                if (QCusBus.Any())
                {
                    EnterpriseName = QCusBus.FirstOrDefault().EnterpriseName;
                }
                else
                    EnterpriseName = NweQBmsBillAr.FirstOrDefault().Bill_Object_Name;
                string outfile = "";
                var outbytes = ExcelHelper.OutPutExcelByArr(TestbmsList1.ToList().OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.MBL).ThenBy(x => x.Operation_Id), @"\FileModel\Bms_Ar_account_HasTax.xlsx", 4, 0, out outfile, "应收对账单(含税)");
                var ms = new System.IO.MemoryStream(outbytes);
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);

                //不含税
                //Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                //if (filterRules.Contains("Entrustment_Name"))
                //{
                //    sheet.Cells[0, 0].Value = EnterpriseName + "空运应付对账单"; ;
                //}
                var aaa = "";
                var bbb = "";
                foreach (var item in filters)
                {

                    if (item.field == "_Flight_Date_Want")
                    {
                        aaa = item.value;
                    }
                    if (item.field == "Flight_Date_Want_")
                    {
                        bbb = item.value;
                    }
                }

                List<AccountTotal> AccountTotallist1 = new List<AccountTotal>();             
                //含税
                Aspose.Cells.Worksheet sheet1 = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                sheet1.Cells[0, 0].Value = EnterpriseName + "空运应收对账单";
                sheet1.Cells[1, 0].Value = (aaa == "" ? "" : Convert.ToDateTime(aaa).ToString("yyyyMMdd")) + ((aaa == "" & bbb == "") ? "" : "-") + (bbb == "" ? "" : Convert.ToDateTime(bbb).ToString("yyyyMMdd"));
                //sheet1.Cells[1, 0].Value = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("yyyyMMdd") + "-" + DateTime.Now.AddDays(1 - DateTime.Now.Day).AddDays(-1).ToString("yyyyMMdd");
                sheet1.Cells[3, 9].Value = "打印时间：" + DateTime.Now.ToString("yyyy-MM-dd");
                foreach (var item in TestbmsList1)
                {
                    AccountTotal t = new AccountTotal();
                    var Qrate = RateRep.Where(x => x.LocalCurrCode == item.Money_Code && x.ForeignCurrCode == "CNY" && x.Year == ((item.Flight_Date_Want == null || item.Flight_Date_Want == "") ? DateTime.Now.Year : (Convert.ToDateTime(item.Flight_Date_Want).Year)) && x.Month == ((item.Flight_Date_Want == null || item.Flight_Date_Want == "") ? DateTime.Now.Month : (Convert.ToDateTime(item.Flight_Date_Want).Month))).FirstOrDefault();
                    t.Flight_Date_Want = item.Flight_Date_Want;
                    t.Money_Code = item.Money_Code;
                    t.Rate = item.Money_Code == "CNY" ? 1 : (Qrate == null ? 1 : Qrate.RecRate);
                    t.Bill_Accounts = item.Bill_Account2;
                    t.Bill_TaxAmount = item.Bill_TaxAmount;
                    t.Bill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                    AccountTotallist1.AddRange(t);
                }
                var Sumtotal1 = AccountTotallist1.Select(x => x.Rate * x.Bill_Accounts).Sum();
                var SumtotalTax = AccountTotallist1.Select(x => x.Rate * x.Bill_TaxAmount).Sum();
                var SumAmountTaxTotal = AccountTotallist1.Select(x => x.Rate * x.Bill_AmountTaxTotal).Sum();

                var CCC = (from p in AccountTotallist1
                           group p by p.Money_Code into g
                           select new
                           {
                               Money_Code = g.Key,
                               TOTALBill_Accounts = g.Sum(x => x.Bill_Accounts),
                               TOTALBill_TaxAmount = g.Sum(x => x.Bill_TaxAmount),
                               TOTALBill_AmountTaxTotal = g.Sum(x => x.Bill_AmountTaxTotal),
                           }).ToList();


                var TestbmsListcount1 = TestbmsList1.Count();
                sheet1.Cells[TestbmsListcount1 + 5, 2].Value = "合计";
                sheet1.Cells[TestbmsListcount1 + 5, 3].Value = "CNY";
                sheet1.Cells[TestbmsListcount1 + 5, 4].Value = Sumtotal1;
                sheet1.Cells[TestbmsListcount1 + 5, 5].Value = SumtotalTax;
                sheet1.Cells[TestbmsListcount1 + 5, 6].Value = SumAmountTaxTotal;

                for (var i = 0; i < CCC.Count(); i++)
                {
                    //sheet1.Cells.Merge(TestbmsListcount1 + 5 + i, 3, 1, 5);
                    sheet1.Cells[TestbmsListcount1 + 5 + i, 3].Value = CCC[i].Money_Code;
                    sheet1.Cells[TestbmsListcount1 + 5 + i, 4].Value = Math.Round((decimal)CCC[i].TOTALBill_Accounts, 2).ToString("#0.00");
                    sheet1.Cells[TestbmsListcount1 + 5 + i, 5].Value = Math.Round((decimal)CCC[i].TOTALBill_TaxAmount, 2).ToString("#0.00");
                    sheet1.Cells[TestbmsListcount1 + 5 + i, 6].Value = Math.Round((decimal)CCC[i].TOTALBill_AmountTaxTotal, 2).ToString("#0.00");
                    Aspose.Cells.Style styletotal = workbook.Styles[workbook.Styles.Add()];//新增样式
                    styletotal.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
                    styletotal.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字 靠左   
                    styletotal.Font.Size = 10;
                    styletotal.Font.IsBold = true;
                    styletotal.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    styletotal.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    for (var m = 0; m < 14; m++)
                    {
                        sheet1.Cells[TestbmsListcount1 + 5 + i, m].SetStyle(styletotal);
                    }
                }
                //for (var i = 0; i < CCC.Count(); i++)
                //{
                //    sheet1.Cells.Merge(TestbmsListcount1 + 5 + i, 3, 1, 5);
                //    //sheet1.Cells[TestbmsListcount1 + 6 + i, 4].Value = CCC[i].Money_Code + ":" + CCC[i].TOTALBill_Accounts;
                //    //sheet1.Cells[TestbmsListcount1 + 6 + i, 5].Value = CCC[i].Money_Code + ":" + CCC[i].TOTALBill_TaxAmount;
                //    //sheet1.Cells[TestbmsListcount1 + 6 + i, 6].Value = CCC[i].Money_Code + ":" + CCC[i].TOTALBill_AmountTaxTotal;
                //    sheet1.Cells[TestbmsListcount1 + 5 + i, 3].Value = CCC[i].Money_Code + ":" + Math.Round((decimal)CCC[i].TOTALBill_Accounts,2).ToString("#0.00") + "   " + CCC[i].Money_Code + ":" + Math.Round((decimal)CCC[i].TOTALBill_TaxAmount,2).ToString("#0.00") + "   " + CCC[i].Money_Code + ":" + Math.Round((decimal)CCC[i].TOTALBill_AmountTaxTotal,2).ToString("#0.00");

                //}

                sheet1.Cells.Columns[0].Width = 9;
                sheet1.Cells.Columns[1].Width = 9;
                sheet1.Cells.Columns[2].Width = 12;
                sheet1.Cells.Columns[3].Width = 3;
                sheet1.Cells.Columns[4].Width = 10;
                sheet1.Cells.Columns[5].Width = 7;
                sheet1.Cells.Columns[6].Width = 10;
                sheet1.Cells.Columns[7].Width = 18;
                sheet1.Cells.Columns[8].Width = 9;
                sheet1.Cells.Columns[9].Width = 3;
                sheet1.Cells.Columns[10].Width = 5;
                sheet1.Cells.Columns[11].Width = 15;//费用明细
                sheet1.Cells.Columns[12].Width = 5;
                sheet1.Cells.Columns[13].Width = 4;
                sheet1.AutoFitRows();
                Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];//新增样式
                style.ShrinkToFit = true;
                style.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字 靠左                             
                style.Font.Size = 10;
                style.Font.Color = System.Drawing.Color.Black;
                style.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                for (var i = 5; i < TestbmsListcount1 + 5; i++)
                {
                    sheet1.Cells[i, 7].SetStyle(style);
                }
                for (var i = 0; i < TestbmsListcount1 + 5; i++)
                {
                    var test = sheet1.Cells.GetRowHeight(i + 4);
                    sheet1.Cells.Rows[i + 4].Height = test + 5;
                }
                //for (var i = 0; i < TestbmsListcount1 + 5; i++)
                //{
                //    sheet1.AutoFitRow(i);
                //}

                //var newms = Server.MapPath(@"\DownLoad\111.xls");
                if (typeName == "打印应收对账单(含税)")
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    string FName = Server.MapPath(dPath + "/pdfDD/" + fileName);
                    workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                }
                else
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    var newms = Server.MapPath(@"\DownLoad\Bms_Ar_account_HasTax.xls");
                    workbook.Save(newms);
                    //workbook.Save(newms, Aspose.Cells.SaveFormat.Pdf);
                    return File(newms, "application/vnd.ms-excel", fileName);
                }

            }

            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        /// <summary>
        /// 应付明细报表
        /// </summary>
        /// <param name="ArrFinance">选中的数据</param>
        /// <param name="filterRules">查询条件</param>
        /// <param name="typeName">报表名称</param>
        /// <param name="CheckALLstatus">是否全选</param>
        /// <returns></returns>
        public ActionResult ExportBms_Ap_Fee(string ArrFinance = "", string filterRules = "", string typeName = "", bool CheckALLstatus = false)
        {
            if (filterRules == "undefined" || filterRules == null)
            {
                return Json(new { Success = false, ErrMsg = "搜索条件为空！" });
            }
            var Qquery = FinanceService.GetData(filterRules);
            string AddfilterRules = Request["AddfilterRules"] ?? "";

            List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
            if (!string.IsNullOrWhiteSpace(filterRules))
                Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
            var NewAddfilters = new List<IEnumerable<filterRule>>();
            if (Addfilters != null && Addfilters.Any())
            {
                foreach (var item in Addfilters)
                {
                    NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                }
            }
            if (NewAddfilters != null && NewAddfilters.Any())
            {
                foreach (var item in NewAddfilters)
                {
                    if (item != null && item.Any())
                    {
                        var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                        Qquery = Qquery.Union(Qquery1);
                    }
                }
            }
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var QResult = Qquery.ToList();

            #region 排除-移除的项

            string RemoveIdStr = Request["RemoveId"] ?? "";
            var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
            if (ArrRemoveId != null && ArrRemoveId.Any())
            {
                var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                QResult = (from p in QResult
                           where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                           select p).ToList();
            }

            #endregion

            var OP_ID = QResult.Select(x => x.Dzbh).Distinct();
            var ops_data = _oPS_EntrustmentInforService.Queryable().Where(x => OP_ID.Contains(x.Operation_Id) && x.Is_TG == false).Select(x => x.Operation_Id).ToList();
            QResult = QResult.Where(x => ops_data.Contains(x.Dzbh)).ToList();
            var ArrBMSID = QResult.Where(x => x.IsAr == false).Select(x => x.Id);
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();
            //var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(ArrFinance);
            var ArrRowsBmsid = ArrRows.Select(x => x.Id);
            var ArrOps_M_ID = data.Select(x => x.MBLId);
            var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
            if (data == null || data.Count == 0)
            {
                return null;
            }
            var BmsBillApRep = unitOfWork.Repository<Bms_Bill_Ap>().Queryable();
            var BmsBillApDtlRep = unitOfWork.Repository<Bms_Bill_Ap_Dtl>().Queryable();
            var QBmsBillAp = BmsBillApRep.Where(x => ArrBMSID.Contains(x.Id) && x.Cancel_Status == false).ToList();
            //QBmsBillAp = QBmsBillAp.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();
            //QBmsBillAp = QBmsBillAp.Where(x => idarr.Contains(x.Id)).ToList();
            var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);

            //var OPS_EnName = data.Select(x => x.Entrustment_Name);
            //var OPS_DZBH = _oPS_EntrustmentInforService.Queryable().Where(x => OPS_EnName.Contains(x.Entrustment_Name)).Select(x => x.Operation_Id).ToList();
            //var NweQBmsBillAr = BmsBillArRep.Where(x => OPS_DZBH.Contains(x.Dzbh)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
            //var QBmsBillAr = BmsBillArRep.Where(x => ArrOps_M_ID.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
            if (CheckALLstatus == true)
            {
                QBmsBillAp = QBmsBillAp.Where(x => ArrBMSID.Contains(x.Id)).ToList();
            }
            else
                QBmsBillAp = QBmsBillAp.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();

            List<BmsFee> BmsFeeList = new List<BmsFee>();
            if (!QBmsBillAp.Any())
            {
                return Json(new { Success = false, ErrMsg = "所选账务信息中没有应付信息！" });
            }
            foreach (var item in QBmsBillAp)
            {
                BmsFee t = new BmsFee();
                var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                t.Operation_Id = item.IsMBLJS == true ? "" : item.Dzbh;
                t.Flight_Date_Want = Qops_en.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_en.Flight_Date_Want).ToString("yyyy-MM-dd");
                t.MBL = item.MBL == null ? "" : (item.MBL == "-" ? "-" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7));
                var Qcus = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                if (Qcus.Any())
                {
                    t.Settle_Code = Qcus.FirstOrDefault().EnterpriseName;
                }
                else
                    t.Settle_Code = item.Bill_Object_Name;
                t.Money_Code = item.Money_Code;
                t.End_Port = Qops_en.End_Port;
                t.Account2 = item.Bill_Account2;
                BmsFeeList.AddRange(t);
            }

            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(BmsFeeList.ToList().OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.MBL).ThenBy(x => x.Operation_Id), @"\FileModel\BmsArFee.xlsx", 0, 0, out outfile, "");
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            var newms = Server.MapPath(@"\DownLoad\Bms_Ap_Fee.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 应收明细报表
        /// </summary>
        /// <param name="ArrFinance">选中的数据</param>
        /// <param name="filterRules">查询条件</param>
        /// <param name="typeName">报表名称</param>
        /// <param name="CheckALLstatus">是否全选</param>
        /// <returns></returns>
        public ActionResult ExportBms_Ar_Fee(string ArrFinance = "", string filterRules = "", string typeName = "", bool CheckALLstatus = false)
        {
            if (filterRules == "undefined" || filterRules == null)
            {
                return Json(new { Success = false, ErrMsg = "搜索条件为空！" });
            }
            var Qquery = FinanceService.GetData(filterRules);
            string AddfilterRules = Request["AddfilterRules"] ?? "";

            List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
            if (!string.IsNullOrWhiteSpace(filterRules))
                Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
            var NewAddfilters = new List<IEnumerable<filterRule>>();
            if (Addfilters != null && Addfilters.Any())
            {
                foreach (var item in Addfilters)
                {
                    NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                }
            }
            if (NewAddfilters != null && NewAddfilters.Any())
            {
                foreach (var item in NewAddfilters)
                {
                    if (item != null && item.Any())
                    {
                        var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                        Qquery = Qquery.Union(Qquery1);
                    }
                }
            }
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var QResult = Qquery.ToList();

            #region 排除-移除的项

            string RemoveIdStr = Request["RemoveId"] ?? "";
            var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
            if (ArrRemoveId != null && ArrRemoveId.Any())
            {
                var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                QResult = (from p in QResult
                           where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                           select p).ToList();
            }

            #endregion

            var OP_ID = QResult.Select(x => x.Dzbh).Distinct();
            var ops_data = _oPS_EntrustmentInforService.Queryable().Where(x => OP_ID.Contains(x.Operation_Id) && x.Is_TG == false).Select(x => x.Operation_Id).ToList();
            QResult = QResult.Where(x => ops_data.Contains(x.Dzbh)).ToList();
            var ArrBMSID = QResult.Where(x => x.IsAr == true).Select(x => x.Id);
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();
            //var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            var ArrOps_M_ID = data.Select(x => x.MBLId);
            var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(ArrFinance);
            var ArrRowsBmsid = ArrRows.Select(x => x.Id);
            var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
            if (data == null || data.Count == 0)
            {
                return null;
            }
            var BmsBillArRep = unitOfWork.Repository<Bms_Bill_Ar>().Queryable();
            var BmsBillArDtlRep = unitOfWork.Repository<Bms_Bill_Ar_Dtl>().Queryable();
            var QBmsBillAr = BmsBillArRep.Where(x => ArrBMSID.Contains(x.Id) && x.Cancel_Status == false).ToList();
            //QBmsBillAr = QBmsBillAr.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();
            var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);

            //var OPS_EnName = data.Select(x => x.Entrustment_Name);
            //var OPS_DZBH = _oPS_EntrustmentInforService.Queryable().Where(x => OPS_EnName.Contains(x.Entrustment_Name)).Select(x => x.Operation_Id).ToList();
            //var NweQBmsBillAr = BmsBillArRep.Where(x => OPS_DZBH.Contains(x.Dzbh)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
            //var QBmsBillAr = BmsBillArRep.Where(x => ArrOps_M_ID.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();

            if (CheckALLstatus == true)
            {
                QBmsBillAr = QBmsBillAr.Where(x => ArrBMSID.Contains(x.Id)).ToList();
            }
            else
                QBmsBillAr = QBmsBillAr.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();
            List<BmsFee> BmsFeeList = new List<BmsFee>();
            if (!QBmsBillAr.Any())
            {
                return Json(new { Success = false, ErrMsg = "所选账务信息中没有应收信息！" });
            }
            foreach (var item in QBmsBillAr)
            {
                BmsFee t = new BmsFee();
                var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                t.Operation_Id = item.Dzbh;
                t.Flight_Date_Want = Qops_en.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_en.Flight_Date_Want).ToString("yyyy-MM-dd");
                t.MBL = item.MBL == null ? "" : (item.MBL == "-" ? "-" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7));
                var Qcus = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                if (Qcus.Any())
                {
                    t.Settle_Code = Qcus.FirstOrDefault().EnterpriseName;
                }
                else
                    t.Settle_Code = item.Bill_Object_Name;
                t.Money_Code = item.Money_Code;
                t.End_Port = Qops_en.End_Port;
                t.Account2 = item.Bill_Account2;
                BmsFeeList.AddRange(t);
            }

            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(BmsFeeList.ToList().OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.MBL).ThenBy(x => x.Operation_Id), @"\FileModel\BmsArFee.xlsx", 0, 0, out outfile, "");
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            var newms = Server.MapPath(@"\DownLoad\Bms_Ar_Fee.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 应付费用明细报表
        /// </summary>
        /// <param name="ArrFinance">选中的数据</param>
        /// <param name="filterRules">查询条件</param>
        /// <param name="typeName">报表名称</param>
        /// <param name="CheckALLstatus">是否全选</param>
        /// <returns></returns>
        public ActionResult ExportBms_Ap_Fee_Dtl(string ArrFinance = "", string filterRules = "", string typeName = "", bool CheckALLstatus = false)
        {
            if (filterRules == "undefined" || filterRules == null)
            {
                return Json(new { Success = false, ErrMsg = "搜索条件为空！" });
            }
            var Qquery = FinanceService.GetData(filterRules);
            string AddfilterRules = Request["AddfilterRules"] ?? "";

            List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
            if (!string.IsNullOrWhiteSpace(filterRules))
                Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
            var NewAddfilters = new List<IEnumerable<filterRule>>();
            if (Addfilters != null && Addfilters.Any())
            {
                foreach (var item in Addfilters)
                {
                    NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                }
            }
            if (NewAddfilters != null && NewAddfilters.Any())
            {
                foreach (var item in NewAddfilters)
                {
                    if (item != null && item.Any())
                    {
                        var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                        Qquery = Qquery.Union(Qquery1);
                    }
                }
            }
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var QResult = Qquery.ToList();

            #region 排除-移除的项

            string RemoveIdStr = Request["RemoveId"] ?? "";
            var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
            if (ArrRemoveId != null && ArrRemoveId.Any())
            {
                var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                QResult = (from p in QResult
                           where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                           select p).ToList();
            }

            #endregion

            var OP_ID = QResult.Select(x => x.Dzbh).Distinct();
            var ops_data = _oPS_EntrustmentInforService.Queryable().Where(x => OP_ID.Contains(x.Operation_Id) && x.Is_TG == false).Select(x => x.Operation_Id).ToList();
            QResult = QResult.Where(x => ops_data.Contains(x.Dzbh)).ToList();
            var ArrBMSID = QResult.Where(x => x.IsAr == false).Select(x => x.Id);
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();
            //var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            var ArrOps_M_ID = data.Select(x => x.MBLId);
            var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
            var BmsBillApRep = unitOfWork.Repository<Bms_Bill_Ap>().Queryable();
            var BmsBillApDtlRep = unitOfWork.Repository<Bms_Bill_Ap_Dtl>().Queryable();
            //var QBmsBillAr = BmsBillArRep.Where(x => idarr.Contains(x.Dzbh)).ToList();
            var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);

            //var OPS_EnName = data.Select(x => x.Entrustment_Name);
            //var OPS_DZBH = _oPS_EntrustmentInforService.Queryable().Where(x => OPS_EnName.Contains(x.Entrustment_Name)).Select(x => x.Operation_Id).ToList();
            //var NweQBmsBillAr = BmsBillArRep.Where(x => OPS_DZBH.Contains(x.Dzbh)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
            var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(ArrFinance);
            var ArrRowsBmsid = ArrRows.Select(x => x.Id);
            var QBmsBillAp = BmsBillApRep.Where(x => ArrBMSID.Contains(x.Id) && x.Cancel_Status == false).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();
            //QBmsBillAp = QBmsBillAp.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();

            if (CheckALLstatus == true)
            {
                QBmsBillAp = QBmsBillAp.Where(x => ArrBMSID.Contains(x.Id)).ToList();
            }
            else
                QBmsBillAp = QBmsBillAp.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();
            List<BmsFeedtl> BmsFeedtlList = new List<BmsFeedtl>();
            if (!QBmsBillAp.Any())
            {
                return Json(new { Success = false, ErrMsg = "所选账务信息中没有应付信息！" });
            }
            foreach (var item in QBmsBillAp)
            {
                BmsFeedtl t = new BmsFeedtl();
                var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                t.Operation_Id = item.IsMBLJS == true ? "" : item.Dzbh;
                t.Flight_Date_Want = Qops_en.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_en.Flight_Date_Want).ToString("yyyy-MM-dd");
                t.MBL = item.MBL == null ? "" : (item.MBL == "-" ? "-" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7));
                var Qcus = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                if (Qcus.Any())
                {
                    t.Settle_Code = Qcus.FirstOrDefault().EnterpriseName;
                }
                else
                    t.Settle_Code = item.Bill_Object_Name;
                t.Money_Code = item.Money_Code;
                t.End_Port = Qops_en.End_Port;
                var QBmsBillArdtl = item.ArrBms_Bill_Ap_Dtl;
                t.Charge_Desc = string.Join("\r\n", QBmsBillArdtl.Select(x => x.Charge_Desc));
                t.Account2 = item.Bill_Account2;
                BmsFeedtlList.AddRange(t);
            }

            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(BmsFeedtlList.ToList().OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.MBL).ThenBy(x => x.Operation_Id), @"\FileModel\BmsArFeedtl.xlsx", 0, 0, out outfile, "应收费用明细");
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            for (var i = 0; i < BmsFeedtlList.Count + 1; i++)
            {
                sheet.AutoFitRow(i);
            }
            var newms = Server.MapPath(@"\DownLoad\Bms_Ap_Fee_Dtl.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 应收费用明细报表
        /// </summary>
        /// <param name="ArrFinance">选中的数据</param>
        /// <param name="filterRules">查询条件</param>
        /// <param name="typeName">报表名称</param>
        /// <param name="CheckALLstatus">是否全选</param>
        public ActionResult ExportBms_Ar_Fee_Dtl(string ArrFinance = "", string filterRules = "", string typeName = "", bool CheckALLstatus = false)
        {
            if (filterRules == "undefined" || filterRules == null)
            {
                return Json(new { Success = false, ErrMsg = "搜索条件为空！" });
            }
            var Qquery = FinanceService.GetData(filterRules);
            string AddfilterRules = Request["AddfilterRules"] ?? "";

            List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
            if (!string.IsNullOrWhiteSpace(filterRules))
                Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
            var NewAddfilters = new List<IEnumerable<filterRule>>();
            if (Addfilters != null && Addfilters.Any())
            {
                foreach (var item in Addfilters)
                {
                    NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                }
            }
            if (NewAddfilters != null && NewAddfilters.Any())
            {
                foreach (var item in NewAddfilters)
                {
                    if (item != null && item.Any())
                    {
                        var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                        Qquery = Qquery.Union(Qquery1);
                    }
                }
            }
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var QResult = Qquery.ToList();

            #region 排除-移除的项

            string RemoveIdStr = Request["RemoveId"] ?? "";
            var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
            if (ArrRemoveId != null && ArrRemoveId.Any())
            {
                var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                QResult = (from p in QResult
                           where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                           select p).ToList();
            }

            #endregion

            var OP_ID = QResult.Select(x => x.Dzbh).Distinct();
            var ops_data = _oPS_EntrustmentInforService.Queryable().Where(x => OP_ID.Contains(x.Operation_Id) && x.Is_TG == false).Select(x => x.Operation_Id).ToList();
            QResult = QResult.Where(x => ops_data.Contains(x.Dzbh)).ToList();
            var ArrBMSID = QResult.Where(x => x.IsAr == true).Select(x => x.Id);
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();
            //var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            var ArrOps_M_ID = data.Select(x => x.MBLId);
            var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
            var BmsBillArRep = unitOfWork.Repository<Bms_Bill_Ar>().Queryable();
            var BmsBillArDtlRep = unitOfWork.Repository<Bms_Bill_Ar_Dtl>().Queryable();
            //var QBmsBillAr = BmsBillArRep.Where(x => idarr.Contains(x.Dzbh)).ToList();
            var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);

            //var OPS_EnName = data.Select(x => x.Entrustment_Name);
            //var OPS_DZBH = _oPS_EntrustmentInforService.Queryable().Where(x => OPS_EnName.Contains(x.Entrustment_Name)).Select(x => x.Operation_Id).ToList();
            //var NweQBmsBillAr = BmsBillArRep.Where(x => OPS_DZBH.Contains(x.Dzbh)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
            var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(ArrFinance);
            var ArrRowsBmsid = ArrRows.Select(x => x.Id);
            var QBmsBillAr = BmsBillArRep.Where(x => ArrBMSID.Contains(x.Id) && x.Cancel_Status == false).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
            //QBmsBillAr = QBmsBillAr.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();
            if (CheckALLstatus == true)
            {
                QBmsBillAr = QBmsBillAr.Where(x => ArrBMSID.Contains(x.Id)).ToList();
            }
            else
                QBmsBillAr = QBmsBillAr.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();
            List<BmsFeedtl> BmsFeedtlList = new List<BmsFeedtl>();
            if (!QBmsBillAr.Any())
            {
                return Json(new { Success = false, ErrMsg = "所选账务信息中没有应收信息！" });
            }
            foreach (var item in QBmsBillAr)
            {
                BmsFeedtl t = new BmsFeedtl();
                var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                t.Operation_Id = item.Dzbh;
                t.Flight_Date_Want = Qops_en.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_en.Flight_Date_Want).ToString("yyyy-MM-dd");
                t.MBL = item.MBL == null ? "" : (item.MBL == "-" ? "-" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7));
                var Qcus = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                if (Qcus.Any())
                {
                    t.Settle_Code = Qcus.FirstOrDefault().EnterpriseName;
                }
                else
                    t.Settle_Code = item.Bill_Object_Name;
                t.Money_Code = item.Money_Code;
                t.End_Port = Qops_en.End_Port;
                var QBmsBillArdtl = item.ArrBms_Bill_Ar_Dtl;
                t.Charge_Desc = string.Join("\r\n", QBmsBillArdtl.Select(x => x.Charge_Desc));
                t.Account2 = item.Bill_Account2;
                BmsFeedtlList.AddRange(t);
            }

            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(BmsFeedtlList.ToList().OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.MBL).ThenBy(x => x.Operation_Id), @"\FileModel\BmsArFeedtl.xlsx", 0, 0, out outfile, "应收费用明细");
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            for (var i = 0; i < BmsFeedtlList.Count + 1; i++)
            {
                sheet.AutoFitRow(i);
            }
            var newms = Server.MapPath(@"\DownLoad\Bms_Ar_Fee_Dtl.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 打印/导出结算方应付/应收明细报表
        /// </summary>
        /// <param name="filterRules">查询条件</param>
        /// <param name="typeName">报表名称</param>
        /// <param name="filterArr">选中数据</param>
        /// <param name="IsAr">应收true/应付false</param>
        /// <param name="CheckALLstatus">是否全选</param>
        /// <returns></returns>
        public ActionResult ExportAR_For_Settle(string filterRules = "", string typeName = "", string filterArr = "", bool IsAr = false, bool CheckALLstatus = false)
        {
            try
            {
                if (filterRules == "undefined" || filterRules == null)
                {
                    return Json(new { Success = false, ErrMsg = "搜索条件为空！" });
                }

                var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(filterArr);
                var ArrRowsBmsid = ArrRows.Select(x => x.Id).ToList();
                var Qquery = FinanceService.GetData(filterRules);
                string AddfilterRules = Request["AddfilterRules"] ?? "";

                List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
                if (!string.IsNullOrWhiteSpace(filterRules))
                    Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
                var NewAddfilters = new List<IEnumerable<filterRule>>();
                if (Addfilters != null && Addfilters.Any())
                {
                    foreach (var item in Addfilters)
                    {
                        NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                    }
                }
                if (NewAddfilters != null && NewAddfilters.Any())
                {
                    foreach (var item in NewAddfilters)
                    {
                        if (item != null && item.Any())
                        {
                            var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                            Qquery = Qquery.Union(Qquery1);
                        }
                    }
                }
                var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
                if (!CheckALLstatus)
                {
                    Qquery = Qquery.Where(x => ArrRowsBmsid.Contains(x.Id));
                }
                var QResult = Qquery.ToList();

                #region 排除-移除的项

                string RemoveIdStr = Request["RemoveId"] ?? "";
                var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
                if (ArrRemoveId != null && ArrRemoveId.Any())
                {
                    var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                    QResult = (from p in QResult
                               where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                               select p).ToList();
                }

                #endregion

                var ArrBMSID = QResult.Where(x => x.IsAr == IsAr).Select(x => x.Id);

                var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();

                var ArrOps_M_ID = data.Select(x => x.MBLId);
                var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);

                var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);
                var PARA_CURRRep = (IEnumerable<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
                string fileaddress = "";
                List<ARAP_For_Settle> ARAPForSettleList = new List<ARAP_For_Settle>();
                if (IsAr)
                {
                    var BmsBillArRep = unitOfWork.Repository<Bms_Bill_Ar>().Queryable();
                    var BmsBillArDtlRep = unitOfWork.Repository<Bms_Bill_Ar_Dtl>().Queryable();
                    var QBmsBillAr = BmsBillArRep.Where(x => ArrBMSID.Contains(x.Id)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                    var TotalQBmsBillAr = (from p in QBmsBillAr
                                           group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                           select new
                                           {
                                               Bill_Object_Id = g.Max(x => x.Bill_Object_Id),
                                               Bill_Object_Name = g.Max(x => x.Bill_Object_Name),
                                               Money_Code = g.Max(x => x.Money_Code),
                                               Bill_Account2 = g.Sum(x => x.Bill_Account2),
                                               Bill_TaxAmount = g.Sum(x => x.Bill_TaxAmount),
                                               Bill_AmountTaxTotal = g.Sum(x => x.Bill_AmountTaxTotal),
                                           }).ToList();

                    foreach (var item in TotalQBmsBillAr)
                    {
                        ARAP_For_Settle t = new ARAP_For_Settle();
                        //var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                        t.Settle_Code = item.Bill_Object_Id;
                        var QCusBusInfo = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                        if (QCusBusInfo.Any())
                        {
                            t.Settle_Name = QCusBusInfo.FirstOrDefault().EnterpriseName;
                        }
                        else
                            t.Settle_Name = item.Bill_Object_Name;
                        if (item.Money_Code == "CNY")
                        {
                            var curr = PARA_CURRRep.Where(x => x.CURR_CODE == item.Money_Code).FirstOrDefault();
                            if (curr != null)
                            {
                                t.Money_Code = curr.CURR_Name;
                            }
                            else
                            {
                                t.Money_Code = item.Money_Code;
                            }
                            t.Bill_Account2 = item.Bill_Account2;
                            t.Bill_TaxAmount = item.Bill_TaxAmount;
                            t.Bill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                            t.FMoney_Code = "";
                            t.FBill_Account2 = null;
                            t.FBill_TaxAmount = null;
                            t.FBill_AmountTaxTotal = null;

                        }
                        else
                        {
                            t.Money_Code = "";
                            t.Bill_Account2 = null;
                            t.Bill_TaxAmount = null;
                            t.Bill_AmountTaxTotal = null;
                            var Qcurr = PARA_CURRRep.Where(x => x.CURR_CODE == item.Money_Code).FirstOrDefault();
                            if (Qcurr != null)
                            {
                                t.FMoney_Code = Qcurr.CURR_Name;
                            }
                            else
                            {
                                t.FMoney_Code = item.Money_Code;
                            }
                            t.FBill_Account2 = item.Bill_Account2;
                            t.FBill_TaxAmount = item.Bill_TaxAmount;
                            t.FBill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                        }

                        //var QBmsBillArdtl = item.ArrBms_Bill_Ar_Dtl;
                        ARAPForSettleList.AddRange(t);
                    }
                    if (typeName.Contains("打印"))
                    {
                        fileaddress = @"\FileModel\BMS_AR_FOR_Settle_Pdf.xlsx";
                    }
                    else
                    {
                        fileaddress = @"\FileModel\BMS_AR_FOR_Settle.xlsx";
                    }
                }
                else
                {
                    var BmsBillAPRep = unitOfWork.Repository<Bms_Bill_Ap>().Queryable();
                    var BmsBillAPDtlRep = unitOfWork.Repository<Bms_Bill_Ap_Dtl>().Queryable();
                    var QBmsBillAP = BmsBillAPRep.Where(x => ArrBMSID.Contains(x.Id)).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();
                    var TotalQBmsBillAp = (from p in QBmsBillAP
                                           group p by new { p.Bill_Object_Id, p.Money_Code } into g
                                           select new
                                           {
                                               Bill_Object_Id = g.Max(x => x.Bill_Object_Id),
                                               Bill_Object_Name = g.Max(x => x.Bill_Object_Name),
                                               Money_Code = g.Max(x => x.Money_Code),
                                               Bill_Account2 = g.Sum(x => x.Bill_Account2),
                                               Bill_TaxAmount = g.Sum(x => x.Bill_TaxAmount),
                                               Bill_AmountTaxTotal = g.Sum(x => x.Bill_AmountTaxTotal),
                                           }).ToList();

                    foreach (var item in TotalQBmsBillAp)
                    {
                        ARAP_For_Settle t = new ARAP_For_Settle();
                        //var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Dzbh).FirstOrDefault();
                        t.Settle_Code = item.Bill_Object_Id;
                        var QCusBusInfo = CusBusInfoRep.Where(x => x.EnterpriseId == item.Bill_Object_Id).ToList();
                        if (QCusBusInfo.Any())
                        {
                            t.Settle_Name = QCusBusInfo.FirstOrDefault().EnterpriseName;
                        }
                        else
                            t.Settle_Name = item.Bill_Object_Name;
                        if (item.Money_Code == "CNY")
                        {
                            var curr = PARA_CURRRep.Where(x => x.CURR_CODE == item.Money_Code).FirstOrDefault();
                            if (curr != null)
                            {
                                t.Money_Code = curr.CURR_Name;
                            }
                            else
                            {
                                t.Money_Code = item.Money_Code;
                            }
                            t.Bill_Account2 = item.Bill_Account2;
                            t.Bill_TaxAmount = item.Bill_TaxAmount;
                            t.Bill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                            t.FMoney_Code = "";
                            t.FBill_Account2 = null;
                            t.FBill_TaxAmount = null;
                            t.FBill_AmountTaxTotal = null;
                            //var QBmsBillArdtl = item.ArrBms_Bill_Ap_Dtl;
                        }
                        else
                        {
                            t.Money_Code = "";
                            t.Bill_Account2 = null;
                            t.Bill_TaxAmount = null;
                            t.Bill_AmountTaxTotal = null;
                            var Qcurr = PARA_CURRRep.Where(x => x.CURR_CODE == item.Money_Code).FirstOrDefault();
                            if (Qcurr != null)
                            {
                                t.FMoney_Code = Qcurr.CURR_Name;
                            }
                            else
                            {
                                t.FMoney_Code = item.Money_Code;
                            }
                            t.FBill_Account2 = item.Bill_Account2;
                            t.FBill_TaxAmount = item.Bill_TaxAmount;
                            t.FBill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                        }
                        ARAPForSettleList.AddRange(t);
                    }
                    if (typeName.Contains("打印"))
                    {
                        fileaddress = @"\FileModel\BMS_AP_FOR_Settle_Pdf.xlsx";
                    }
                    else
                    {
                        fileaddress = @"\FileModel\BMS_AP_FOR_Settle.xlsx";
                    }
                }
                string outfile = "";
                int HeadLineRowIndex = 0;
                if (typeName.Contains("打印"))
                {
                    HeadLineRowIndex = 1;
                }
                var outbytes = ExcelHelper.OutPutExcelByArr(ARAPForSettleList.OrderBy(x=>x.Settle_Code).ToList(), fileaddress, HeadLineRowIndex, 0, out outfile, typeName);
                var ms = new System.IO.MemoryStream(outbytes);
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
                Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];

                if (typeName.Contains("打印"))
                {

                    List<ARAP_For_Settle> AccountTotallist1 = new List<ARAP_For_Settle>();
                    Aspose.Cells.Worksheet sheet1 = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                    var TestbmsListcount1 = ARAPForSettleList.Count();
                    sheet1.Cells[0, 9].Value = DateTime.Now.ToString("yyyy-MM-dd");

                    sheet1.Cells.Columns[0].Width = 10;
                    sheet1.Cells.Columns[1].Width = 31;
                    sheet1.Cells.Columns[2].Width = 8;
                    sheet1.Cells.Columns[3].Width = 9;
                    sheet1.Cells.Columns[4].Width = 8;
                    sheet1.Cells.Columns[5].Width = 12;
                    sheet1.Cells.Columns[6].Width = 8;
                    sheet1.Cells.Columns[7].Width = 9;
                    sheet1.Cells.Columns[8].Width = 8;
                    sheet1.Cells.Columns[9].Width = 13;
                    Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];//新增样式
                    style.ShrinkToFit = true;
                    style.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
                    style.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;//文字 靠左                             
                    style.Font.Color = System.Drawing.Color.Black;
                    style.Font.Size = 10;
                    style.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    style.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    style.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    style.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
                    for (var i = 1; i < TestbmsListcount1 + 2; i++)
                    {
                        sheet1.Cells[i, 1].SetStyle(style);
                    }
                    for (var i = 0; i < TestbmsListcount1 + 2; i++)
                    {
                        sheet1.AutoFitRow(i);
                    }
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    string FName = Server.MapPath(dPath + "/pdfDD/" + fileName);
                    workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);

                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                }
                else
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    var newms = Server.MapPath(@"\DownLoad\222.xls");
                    workbook.Save(newms);
                    return File(newms, "application/vnd.ms-excel", fileName);

                }

            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        public ActionResult ExportOperationFee(string ArrFinance = "", string filterRules = "", string typeName = "", bool CheckALLstatus = false)
        {
            try
            {
                if (filterRules == "undefined" || filterRules == null)
                {
                    return Json(new { Success = false, ErrMsg = "搜索条件为空！" });
                }
                var Qquery = FinanceService.GetData(filterRules);
                string AddfilterRules = Request["AddfilterRules"] ?? "";

                List<IEnumerable<filterRule>> Addfilters = new List<IEnumerable<filterRule>>();
                if (!string.IsNullOrWhiteSpace(filterRules))
                    Addfilters = JsonConvert.DeserializeObject<List<IEnumerable<filterRule>>>(AddfilterRules);
                var NewAddfilters = new List<IEnumerable<filterRule>>();
                if (Addfilters != null && Addfilters.Any())
                {
                    foreach (var item in Addfilters)
                    {
                        NewAddfilters.Add(item.Where(x => !string.IsNullOrWhiteSpace(x.value)).ToList());//去除空的 数据
                    }
                }
                if (NewAddfilters != null && NewAddfilters.Any())
                {
                    foreach (var item in NewAddfilters)
                    {
                        if (item != null && item.Any())
                        {
                            var Qquery1 = FinanceService.GetData(Newtonsoft.Json.JsonConvert.SerializeObject(item), null);
                            Qquery = Qquery.Union(Qquery1);
                        }
                    }
                }
                var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
                var QResult = Qquery.ToList();

                #region 排除-移除的项

                string RemoveIdStr = Request["RemoveId"] ?? "";
                var ArrRemoveId = JsonConvert.DeserializeObject<List<Finance>>(RemoveIdStr);
                if (ArrRemoveId != null && ArrRemoveId.Any())
                {
                    var ArrIsArId = ArrRemoveId.Select(x => new { IsAr = x.IsAr, Id = x.Id });
                    QResult = (from p in QResult
                               where !ArrIsArId.Contains(new { IsAr = p.IsAr, Id = p.Id })
                               select p).ToList();
                }

                #endregion

                var OP_ID = QResult.Select(x => x.Dzbh).Distinct();
                var ops_data = _oPS_EntrustmentInforService.Queryable().Where(x => OP_ID.Contains(x.Operation_Id) && x.Is_TG == false).Select(x => x.Operation_Id).ToList();
                QResult = QResult.Where(x => ops_data.Contains(x.Dzbh) && x.IsAr == false).ToList();
                //var ArrCarriage_Account_Code = QResult.Select(x => x.Carriage_Account_Code).Distinct();
                //if (ArrCarriage_Account_Code.Count() > 1)
                //{
                //    return Json(new { Success = false, ErrMsg = "选中的账务信息中含有多个结算方！" });
                //}
                var ArrBMSID = QResult.Where(x => x.IsAr == false).Select(x => x.Id);
                var ArrDZBH = QResult.Select(x => x.Dzbh).Distinct();
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => ArrDZBH.Contains(x.Operation_Id)).ToList();
                //var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
                var ArrRows = JsonConvert.DeserializeObject<IEnumerable<Finance>>(ArrFinance);
                var ArrRowsBmsid = ArrRows.Select(x => x.Id);
                var ArrOps_M_ID = data.Select(x => x.MBLId);
                var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
                if (data == null || data.Count == 0)
                {
                    return null;
                }
                var BmsBillApRep = unitOfWork.Repository<Bms_Bill_Ap>().Queryable();
                var BmsBillApDtlRep = unitOfWork.Repository<Bms_Bill_Ap_Dtl>().Queryable();
                var QBmsBillAp = BmsBillApRep.Where(x => ArrBMSID.Contains(x.Id) && x.Cancel_Status == false).ToList();
                //QBmsBillAp = QBmsBillAp.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();
                //QBmsBillAp = QBmsBillAp.Where(x => idarr.Contains(x.Id)).ToList();
                var CusBusInfoRep = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                var RateRep = (IEnumerable<Rate>)CacheHelper.Get_SetCache(Common.CacheNameS.Rate);

                //var OPS_EnName = data.Select(x => x.Entrustment_Name);
                //var OPS_DZBH = _oPS_EntrustmentInforService.Queryable().Where(x => OPS_EnName.Contains(x.Entrustment_Name)).Select(x => x.Operation_Id).ToList();
                //var NweQBmsBillAr = BmsBillArRep.Where(x => OPS_DZBH.Contains(x.Dzbh)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                //var QBmsBillAr = BmsBillArRep.Where(x => ArrOps_M_ID.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
                //if (CheckALLstatus == true)
                //{
                //    QBmsBillAp = QBmsBillAp.Where(x => ArrBMSID.Contains(x.Id)).ToList();
                //}
                //else
                //    QBmsBillAp = QBmsBillAp.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();

                if (CheckALLstatus == true)
                {
                    QResult = QResult.Where(x => ArrBMSID.Contains(x.Id)).ToList();
                }
                else
                    QResult = QResult.Where(x => ArrRowsBmsid.Contains(x.Id)).ToList();
                var ArrBill_Object_Id = QResult.Select(x => x.Bill_Object_Id).Distinct();
                if (ArrBill_Object_Id.Count() > 1)
                {
                    return Json(new { Success = false, ErrMsg = "选中的账务信息中含有多个结算方！" });
                }
                var QBill_Type = QResult.Where(x => x.Bill_Type == "D/N" || x.Bill_Type == "FP");
                if (QBill_Type.Any()) 
                {
                    return Json(new { Success = false, ErrMsg = "选中的账务信息中含有应收信息！" });
                }
                if (QResult.Any(x => x.SignIn_ECCNo == null)) 
                {
                    return Json(new { Success = false, ErrMsg = "选中的账务信息中缺少采购订单号！" });
                }
                var MoneyCode = QResult.Select(x=>x.Money_Code).ToList().Distinct();
                if (MoneyCode.Count() > 1) 
                {
                    return Json(new { Success = false, ErrMsg = "选中的账务信息中有不同币种，无法打印！" });
                }
                var QResultARR = QResult.Where(x => ArrDZBH.Contains(x.Dzbh));
                List<OperationFee> OperationFeeList = new List<OperationFee>();

                OperationFee t = new OperationFee();
                List<AccountTotal> AccountTotallist = new List<AccountTotal>();

                var Qcus = CusBusInfoRep.Where(x => x.EnterpriseId == QResult.FirstOrDefault().Bill_Object_Id).ToList();
                if (Qcus.Any())
                {
                    t.Settle_Name = Qcus.FirstOrDefault().EnterpriseName;
                }
                else
                    t.Settle_Name = QResult.FirstOrDefault().Bill_Object_Name;
                var aaa = "";
                var bbb = "";
                foreach (var itemm in filters)
                {

                    if (itemm.field == "_Flight_Date_Want")
                    {
                        aaa = itemm.value;
                    }
                    if (itemm.field == "Flight_Date_Want_")
                    {
                        bbb = itemm.value;
                    }
                }
                t.Flight_Date_Want = (aaa == "" ? "" : Convert.ToDateTime(aaa).ToString("yyyyMMdd")) + ((aaa == "" & bbb == "") ? "" : "-") + (bbb == "" ? "" : Convert.ToDateTime(bbb).ToString("yyyyMMdd"));
                t.SignIn_ECCNo = string.Join(",", QResult.Where(x => !string.IsNullOrWhiteSpace(x.SignIn_ECCNo)).Select(x => x.SignIn_ECCNo).Distinct());
                t.Remark = QResult.Where(x => !string.IsNullOrWhiteSpace(x.Invoice_No)).Select(x => x.Invoice_No).Distinct().Count().ToString();
                foreach (var item in QResultARR)
                {
                    AccountTotal B = new AccountTotal();
                    var Qrate = RateRep.Where(x => x.LocalCurrCode == item.Money_Code && x.ForeignCurrCode == "CNY" && x.Year == (item.Flight_Date_Want == null ? DateTime.Now.Year : (Convert.ToDateTime(item.Flight_Date_Want).Year)) && x.Month == (item.Flight_Date_Want == null ? DateTime.Now.Month : (Convert.ToDateTime(item.Flight_Date_Want).Month))).FirstOrDefault();
                    B.Flight_Date_Want = item.Flight_Date_Want == null ? "" : Convert.ToDateTime(item.Flight_Date_Want).ToString("yyyy-MM-dd");
                    B.Money_Code = item.Money_Code;
                    B.Rate = item.Money_Code == "CNY" ? 1 : (Qrate == null ? 1 : Qrate.RecRate);
                    B.Bill_Accounts = item.Bill_Account2;
                    B.Bill_TaxAmount = item.Bill_TaxAmount;
                    B.Bill_AmountTaxTotal = item.Bill_AmountTaxTotal;
                    AccountTotallist.AddRange(B);

                }
                t.Bill_Account2 = AccountTotallist.Select(x => x.Bill_Accounts).Sum();
                t.UBill_Account2 = AccountTotallist.FirstOrDefault().Money_Code +" "+ Math.Round((decimal)t.Bill_Account2, 2).ToString("#0.00"); ;
                var BD_DEFDOC_LISTRep = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
                var paywayName = BD_DEFDOC_LISTRep.Where(x => x.LISTCODE == QResult.FirstOrDefault().Payway);
                if (paywayName.Any())
                {
                    t.Payway = paywayName.FirstOrDefault().LISTNAME;
                }
                else t.Payway = QResult.FirstOrDefault().Payway;
                
                t.SignIn_Name = QResult.FirstOrDefault().SignIn_Name;
                if (t.Bill_Account2 != null && t.Bill_Account2 != 0) 
                {
                    
                    var UpperBill_Account2 = MoneyToUpper(t.Bill_Account2.ToString());
                    for (int i = 1; i <= UpperBill_Account2.Length; i++)
                    {
                        UpperBill_Account2 = UpperBill_Account2.Insert(i, " ");
                        i++;
                    }
                    t.UpperBill_Account2 = "报销金额(大写)    " + UpperBill_Account2;
                }
                var FFlight_Date_Want = QResult.OrderBy(x => x.Flight_Date_Want).FirstOrDefault().Flight_Date_Want;
                var LFlight_Date_Want = QResult.OrderBy(x => x.Flight_Date_Want).LastOrDefault().Flight_Date_Want;
              

                OperationFeeList.AddRange(t);

                Dictionary<string, Tuple<int, int>> dict = new Dictionary<string, Tuple<int, int>>() { 
             {"Settle_Name",new Tuple<int,int>(4,1)},
             {"Bill_Account2",new Tuple<int,int>(4,9)},
             {"Flight_Date_Want",new Tuple<int,int>(5,1)},
             {"SignIn_ECCNo",new Tuple<int,int>(6,1)},
             {"Remark",new Tuple<int,int>(4,10)}, 
             {"UpperBill_Account2",new Tuple<int,int>(7,0)}, 
             {"UBill_Account2",new Tuple<int,int>(7,9)},
             {"PrintTime",new Tuple<int,int>(2,9)},
             {"Payway",new Tuple<int,int>(2,2)},
             {"SignIn_Name",new Tuple<int,int>(9,10)},
           
            };

                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(Server.MapPath("/FileModel/OperationFee.xlsx"));
                for (int i = 0; i < OperationFeeList.Count(); i++)
                {
                    if (i > 0)
                    {
                        workbook.Worksheets.AddCopy(0);
                    }
                    var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[workbook.Worksheets.Count - 1];
                    sheet.Name = "业务费用报销单" + (i + 1);
                    var objValue = OperationFeeList[i];

                    #region 插入数据
                    var Settle_Name = dict.Where(x => x.Key == "Settle_Name");
                    if (Settle_Name.Any())
                    {
                        var Tobj = Settle_Name.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Settle_Name;
                    }
                    var Bill_Account2 = dict.Where(x => x.Key == "Bill_Account2");
                    if (Bill_Account2.Any())
                    {
                        var Tobj = Bill_Account2.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = Math.Round((decimal)objValue.Bill_Account2, 2).ToString("#0.00");
                    }
                    var Flight_Date_Want = dict.Where(x => x.Key == "Flight_Date_Want");
                    if (Flight_Date_Want.Any())
                    {
                        var Tobj = Flight_Date_Want.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_Date_Want;
                    }
                    var SignIn_ECCNo = dict.Where(x => x.Key == "SignIn_ECCNo");
                    if (SignIn_ECCNo.Any())
                    {
                        var Tobj = SignIn_ECCNo.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.SignIn_ECCNo;
                    }
                    var Remark = dict.Where(x => x.Key == "Remark");
                    if (Remark.Any())
                    {
                        var Tobj = Remark.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Remark;
                    }
                    var UpperBill_Account2 = dict.Where(x => x.Key == "UpperBill_Account2");
                    if (UpperBill_Account2.Any())
                    {
                        var Tobj = UpperBill_Account2.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.UpperBill_Account2;
                    }
                    var UBill_Account2 = dict.Where(x => x.Key == "UBill_Account2");
                    if (UBill_Account2.Any())
                    {
                        var Tobj = UBill_Account2.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.UBill_Account2;
                    }
                    var PrintTime = dict.Where(x => x.Key == "PrintTime");
                    if (PrintTime.Any())
                    {
                        var Tobj = PrintTime.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = DateTime.Now.Year + " 年 " + DateTime.Now.Month + " 月 " +DateTime.Now.Day + " 日 ";
                    }
                    var Payway = dict.Where(x => x.Key == "Payway");
                    if (Payway.Any())
                    {
                        var Tobj = Payway.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Payway;
                    }
                    var SignIn_Name = dict.Where(x => x.Key == "SignIn_Name");
                    if (SignIn_Name.Any())
                    {
                        var Tobj = SignIn_Name.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.SignIn_Name;
                    }
                    #endregion
                }
                var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                string FName = Server.MapPath(dPath + "/pdfDD/" + fileName);
                //workbook.Save(FName);
                workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
                //System.IO.FileInfo oo = new System.IO.FileInfo(FName);
                //application/pdf;application/octet-stream; 
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                //return File(FName, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }
        }

        #region 结算报表临时类
        /// <summary>
        /// 金额转换成中文大写金额
        /// </summary>
        /// <param name="LowerMoney">eg:10.74</param>
        /// <returns></returns>
        public static string MoneyToUpper(string LowerMoney)
        {
            string functionReturnValue = null;
            bool IsNegative = false; // 是否是负数
            if (LowerMoney.Trim().Substring(0, 1) == "-")
            {
                // 是负数则先转为正数
                LowerMoney = LowerMoney.Trim().Remove(0, 1);
                IsNegative = true;
            }
            string strLower = null;
            string strUpart = null;
            string strUpper = null;
            int iTemp = 0;
            // 保留两位小数 123.489→123.49　　123.4→123.4
            LowerMoney = Math.Round(double.Parse(LowerMoney), 2).ToString();
            if (LowerMoney.IndexOf(".") > 0)
            {
                if (LowerMoney.IndexOf(".") == LowerMoney.Length - 2)
                {
                    LowerMoney = LowerMoney + "0";
                }
            }
            else
            {
                LowerMoney = LowerMoney + ".00";
            }
            strLower = LowerMoney;
            iTemp = 1;
            strUpper = "";
            while (iTemp <= strLower.Length)
            {
                switch (strLower.Substring(strLower.Length - iTemp, 1))
                {
                    case ".":
                        strUpart = "元";
                        break;
                    case "0":
                        strUpart = "零";
                        break;
                    case "1":
                        strUpart = "壹";
                        break;
                    case "2":
                        strUpart = "贰";
                        break;
                    case "3":
                        strUpart = "叁";
                        break;
                    case "4":
                        strUpart = "肆";
                        break;
                    case "5":
                        strUpart = "伍";
                        break;
                    case "6":
                        strUpart = "陆";
                        break;
                    case "7":
                        strUpart = "柒";
                        break;
                    case "8":
                        strUpart = "捌";
                        break;
                    case "9":
                        strUpart = "玖";
                        break;
                }

                switch (iTemp)
                {
                    case 1:
                        strUpart = strUpart + "分";
                        break;
                    case 2:
                        strUpart = strUpart + "角";
                        break;
                    case 3:
                        strUpart = strUpart + "";
                        break;
                    case 4:
                        strUpart = strUpart + "";
                        break;
                    case 5:
                        strUpart = strUpart + "拾";
                        break;
                    case 6:
                        strUpart = strUpart + "佰";
                        break;
                    case 7:
                        strUpart = strUpart + "仟";
                        break;
                    case 8:
                        strUpart = strUpart + "万";
                        break;
                    case 9:
                        strUpart = strUpart + "拾";
                        break;
                    case 10:
                        strUpart = strUpart + "佰";
                        break;
                    case 11:
                        strUpart = strUpart + "仟";
                        break;
                    case 12:
                        strUpart = strUpart + "亿";
                        break;
                    case 13:
                        strUpart = strUpart + "拾";
                        break;
                    case 14:
                        strUpart = strUpart + "佰";
                        break;
                    case 15:
                        strUpart = strUpart + "仟";
                        break;
                    case 16:
                        strUpart = strUpart + "万";
                        break;
                    default:
                        strUpart = strUpart + "";
                        break;
                }

                strUpper = strUpart + strUpper;
                iTemp = iTemp + 1;
            }

            strUpper = strUpper.Replace("零拾", "零");
            strUpper = strUpper.Replace("零佰", "零");
            strUpper = strUpper.Replace("零仟", "零");
            strUpper = strUpper.Replace("零零零", "零");
            strUpper = strUpper.Replace("零零", "零");
            strUpper = strUpper.Replace("零角零分", "整");
            strUpper = strUpper.Replace("零分", "整");
            strUpper = strUpper.Replace("零角", "零");
            strUpper = strUpper.Replace("零亿零万零元", "亿元");
            strUpper = strUpper.Replace("亿零万零元", "亿元");
            strUpper = strUpper.Replace("零亿零万", "亿");
            strUpper = strUpper.Replace("零万零元", "万元");
            strUpper = strUpper.Replace("零亿", "亿");
            strUpper = strUpper.Replace("零万", "万");
            strUpper = strUpper.Replace("零元", "元");
            strUpper = strUpper.Replace("零零", "零");

            // 对壹元以下的金额的处理
            if (strUpper.Substring(0, 1) == "元")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "零")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "角")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "分")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "整")
            {
                strUpper = "零元整";
            }
            functionReturnValue = strUpper;

            if (IsNegative == true)
            {
                return "负" + functionReturnValue;
            }
            else
            {
                return functionReturnValue;
            }
        }

        public class Bms_ap_account
        {
            public string Operation_Id { get; set; }
            public string Flight_Date_Want { get; set; }
            public string MBL { get; set; }
            public string Money_Code { get; set; }
            public decimal? Bill_Account2 { get; set; }
            public string Settle_Code { get; set; }//应收-客户名称   应付-供方名称
            public string Invoice_No { get; set; }
            public bool Sumbmit_Status { get; set; }//应收-销账标志   应付-提交标志
            //public string Sumbmit_No_Org { get; set; }//应收-销账人   应付-原始提交号
            public string Bms_ar_dtl { get; set; }
            public string Remark { get; set; }
            public string End_Port { get; set; }
        }
        public class Bms_ap_account_HasTax
        {
            public string Operation_Id { get; set; }
            public string Flight_Date_Want { get; set; }
            public string MBL { get; set; }
            public string Money_Code { get; set; }
            public decimal? Bill_Account2 { get; set; }
            public decimal Bill_TaxAmount { get; set; }
            public decimal Bill_AmountTaxTotal { get; set; }
            public string Settle_Code { get; set; }//应收-客户名称   应付-供方名称
            public string Invoice_No { get; set; }
            public bool Sumbmit_Status { get; set; }//应收-销账标志   应付-提交标志
            //public string Sumbmit_No_Org { get; set; }//应收-销账人   应付-原始提交号
            public string Bms_ar_dtl { get; set; }
            public string Remark { get; set; }
            public string End_Port { get; set; }
        }
        public class Bms_ar_account
        {
            public string Operation_Id { get; set; }
            public string Flight_Date_Want { get; set; }
            public string MBL { get; set; }
            public string Money_Code { get; set; }
            public decimal? Bill_Account2 { get; set; }
            public string Settle_Code { get; set; }//应收-客户名称   应付-供方名称
            public string Invoice_No { get; set; }
            public bool Sumbmit_Status { get; set; }//应收-销账标志   应付-提交标志
            public string Sumbmit_No_Org { get; set; }//应收-销账人   应付-原始提交号
            public string Bms_ar_dtl { get; set; }
            public string Remark { get; set; }
            public string End_Port { get; set; }
        }
        public class Bms_ar_account_HasTax
        {
            public string Operation_Id { get; set; }
            public string Flight_Date_Want { get; set; }
            public string MBL { get; set; }
            public string Money_Code { get; set; }
            public decimal? Bill_Account2 { get; set; }
            public decimal Bill_TaxAmount { get; set; }
            public decimal Bill_AmountTaxTotal { get; set; }
            public string Settle_Code { get; set; }//应收-客户名称   应付-供方名称
            public string Invoice_No { get; set; }
            public bool Sumbmit_Status { get; set; }//应收-销账标志   应付-提交标志
            public string Sumbmit_No_Org { get; set; }//应收-销账人   应付-原始提交号
            public string Bms_ar_dtl { get; set; }
            public string Remark { get; set; }
            public string End_Port { get; set; }
        }
        public class AccountTotal
        {
            public string Flight_Date_Want { get; set; }//开航日期
            public string Money_Code { get; set; }//币种
            public decimal Rate { get; set; }//汇率
            public decimal? Bill_Accounts { get; set; }//金额
            public decimal Bill_TaxAmount { get; set; }//税金
            public decimal Bill_AmountTaxTotal { get; set; }//价税合计

        }
        public class BmsFee
        {
            public string Operation_Id { get; set; }
            public string Flight_Date_Want { get; set; }
            public string MBL { get; set; }
            public string Settle_Code { get; set; }
            public string End_Port { get; set; }
            public string Money_Code { get; set; }
            public decimal Account2 { get; set; }
        }
        public class BmsFeedtl
        {
            public string Operation_Id { get; set; }
            public string Flight_Date_Want { get; set; }
            public string MBL { get; set; }
            public string Settle_Code { get; set; }
            public string End_Port { get; set; }
            public string Money_Code { get; set; }
            public string Charge_Desc { get; set; }
            public decimal Account2 { get; set; }
        }
        public class ARAP_For_Settle
        {
            public string Settle_Code { get; set; }
            public string Settle_Name { get; set; }
            public string Money_Code { get; set; }
            public decimal? Bill_Account2 { get; set; }
            public decimal? Bill_TaxAmount { get; set; }
            public decimal? Bill_AmountTaxTotal { get; set; }
            public string FMoney_Code { get; set; }
            public decimal? FBill_Account2 { get; set; }
            public decimal? FBill_TaxAmount { get; set; }
            public decimal? FBill_AmountTaxTotal { get; set; }

        }
        public class OperationFee
        {
            public string Settle_Name { get; set; }
            public decimal? Bill_Account2 { get; set; }
            public string UBill_Account2 { get; set; }
            public string Flight_Date_Want { get; set; }
            public string SignIn_ECCNo { get; set; }
            public string Remark { get; set; }
            public string UpperBill_Account2 { get; set; }
            public string SignIn_Name { get; set; }
            public string Payway { get; set; }
        }

        #endregion
    }
}