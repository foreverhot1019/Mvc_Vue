using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using AirOut.Web.Models;
using AirOut.Web.Services;
using AirOut.Web.Repositories;
using AirOut.Web.Extensions;
using System.Data.Entity.Validation;
using Repository.Pattern.Ef6;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AirOut.Web.Controllers
{
    public class OPS_M_OrdersController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<OPS_M_Order>, Repository<OPS_M_Order>>();
        //container.RegisterType<IOPS_M_OrderService, OPS_M_OrderService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IOPS_M_OrderService _oPS_M_OrderService;
        private readonly IOPS_EntrustmentInforService _oPS_EntrustmentInforService;
        private readonly IOPS_H_OrderService _oPS_H_OrderService;
        private readonly IWarehouse_receiptService _warehouse_receiptService;
        private readonly IWarehouse_Cargo_SizeService _warehouse_Cargo_SizeService;
        private readonly ICustomsInspectionService _customsInspectionService;
        private readonly IContactsService _contactsService;
        private readonly IPictureService _pictureService;
        private readonly IBms_Bill_ApService _bms_Bill_ApService;
        private readonly ICostMoneyService _costMoneyService;
        private readonly ICustomerQuotedPriceService _customerQuotedPriceService;
        private readonly IChangeOrderHistoryService _changeOrderHistoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/OPS_M_Orders";

        public OPS_M_OrdersController(
            IOPS_M_OrderService oPS_M_OrderService, IOPS_EntrustmentInforService oPS_EntrustmentInforService,
            IOPS_H_OrderService oPS_H_OrderService, IWarehouse_receiptService warehouse_receiptService,
            IWarehouse_Cargo_SizeService warehouse_Cargo_SizeService, ICustomsInspectionService customsInspectionService,
            IContactsService contactsService, IPictureService pictureService,
            IBms_Bill_ApService bms_Bill_ApService, ICostMoneyService costMoneyService,
            ICustomerQuotedPriceService customerQuotedPriceService, IChangeOrderHistoryService changeOrderHistoryService, IUnitOfWorkAsync unitOfWork)
        {
            _oPS_M_OrderService = oPS_M_OrderService;
            _oPS_EntrustmentInforService = oPS_EntrustmentInforService;
            _oPS_H_OrderService = oPS_H_OrderService;
            _warehouse_receiptService = warehouse_receiptService;
            _warehouse_Cargo_SizeService = warehouse_Cargo_SizeService;
            _customsInspectionService = customsInspectionService;
            _contactsService = contactsService;
            _pictureService = pictureService;
            _bms_Bill_ApService = bms_Bill_ApService;
            _costMoneyService = costMoneyService;
            _customerQuotedPriceService = customerQuotedPriceService;
            _changeOrderHistoryService = changeOrderHistoryService;
            _unitOfWork = unitOfWork;
        }

        // GET: OPS_M_Orders/Index
        public ActionResult Index()
        {
            //var ops_m_order  = _oPS_M_OrderService.Queryable().AsQueryable();
            //return View(ops_m_order  );
            return View();
        }

        public ActionResult WarehouseSee(string MBL = "", int MBLId = 0, string HBL = "")
        {
            ViewBag.MBL = MBL;
            ViewBag.HBL = HBL;
            ViewBag.MBLId = MBLId;
            return View();
        }

        // Get :OPS_M_Orders/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;

            #region 查询，主单、委托、分单

            var QM = _unitOfWork.Repository<OPS_M_Order>().Queryable();
            var QE = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable();
            var ops = (from n in QM
                       join x in QE on n.Id equals x.MBLId
                       select new
                       {
                           Id = n.Id,
                           WTId = x.Id,
                           MBL = n.MBL,
                           HBL = x.HBL,
                           Airways_Code = x.Airways_Code,
                           FWD_Code = n.FWD_Code,
                           Shipper_M = n.Shipper_M,
                           Consignee_M = n.Consignee_M,
                           Notify_Part_M = n.Notify_Part_M,
                           Depart_Port = n.Depart_Port,
                           End_Port = n.End_Port,
                           Flight_No = n.Flight_No,
                           Flight_Date_Want = n.Flight_Date_Want,
                           Currency_M = n.Currency_M,
                           Bragainon_Article_M = n.Bragainon_Article_M,
                           Pay_Mode_M = n.Pay_Mode_M,
                           Carriage_M = n.Carriage_M,
                           Incidental_Expenses_M = n.Incidental_Expenses_M,
                           Declare_Value_Trans_M = n.Declare_Value_Trans_M,
                           Declare_Value_Ciq_M = n.Declare_Value_Ciq_M,
                           Risk_M = n.Risk_M,
                           Marks_M = n.Marks_M,
                           EN_Name_M = n.EN_Name_M,
                           Hand_Info_M = n.Hand_Info_M,
                           Signature_Agent_M = n.Signature_Agent_M,
                           Rate_Class_M = n.Rate_Class_M,
                           Air_Frae = n.Air_Frae,
                           AWC = n.AWC,
                           Pieces_M = n.Pieces_M,
                           Weight_M = n.Weight_M,
                           Volume_M = n.Volume_M,
                           Charge_Weight_M = n.Charge_Weight_M,
                           Price_Article = n.Price_Article,
                           CCC = n.CCC,
                           File_M = n.File_M,
                           Status = n.Status,
                           OperatingPoint = n.OperatingPoint,
                           Operation_Id = x.Operation_Id,
                           Entrustment_Code = x.Entrustment_Code,
                           Entrustment_Name = x.Entrustment_Name,
                           Is_Book_Flat = x.Is_Book_Flat,
                           Book_Flat_Code = x.Book_Flat_Code,
                           Batch_Num = x.Batch_Num,
                           ADDID = n.ADDID,
                           ADDWHO = n.ADDWHO,
                           Is_BAS = x.Is_BAS,
                           Is_DCZ = x.Is_DCZ,
                           Is_ZB = x.Is_ZB,
                           Is_TG = x.Is_TG,
                           Is_HDQ = x.Is_HDQ,
                           Is_BG = x.Is_BG,
                           Is_BQ = x.Is_BQ,
                           Is_OutGoing = x.Is_OutGoing,
                           SendOut_ZD = n.SendOut_ZD,
                           Pieces_Fact = x.Pieces_Fact,
                           Weight_Fact = x.Weight_Fact,
                           Volume_Fact = x.Volume_Fact,
                           Pieces_SK = x.Pieces_SK,
                           Weight_SK = x.Weight_SK,
                           Volume_SK = x.Volume_SK,
                           Is_Self = x.Is_Self,
                           Remark = x.Remark,
                           OPS_BMS_Status = n.OPS_BMS_Status,
                           x.SallerId,
                           x.SallerName,
                       }).AsQueryable();
            if (filters != null)
            {
                #region 加载查询条件

                foreach (var item in filters)
                {
                    if (item.field == "Only_Self" && !string.IsNullOrEmpty(item.value))
                    {
                        var name = CurrentAppUser.Id;
                        ops = ops.Where(x => (x.ADDID == name));
                    }
                    if (item.field == "Operation_Id" && !string.IsNullOrEmpty(item.value))
                    {
                        ops = ops.Where(x => x.Operation_Id.Contains(item.value));
                    }
                    if (item.field == "MBL" && !string.IsNullOrEmpty(item.value))
                    {
                        string value = Common.RemoveNotNumber(item.value);
                        ops = ops.Where(x => x.MBL.Contains(value));
                    }
                    if (item.field == "Entrustment_Code" && !string.IsNullOrEmpty(item.value))
                    {
                        ops = ops.Where(x => x.Entrustment_Code.Contains(item.value));
                    }
                    if (item.field == "Entrustment_Name" && !string.IsNullOrEmpty(item.value))
                    {
                        ops = ops.Where(x => x.Entrustment_Name == item.value);
                    }
                    if (item.field == "Book_Flat_Code" && !string.IsNullOrEmpty(item.value))
                    {
                        ops = ops.Where(x => x.Book_Flat_Code == item.value);
                    }
                    if (item.field == "Batch_Num" && !string.IsNullOrEmpty(item.value))
                    {
                        ops = ops.Where(x => x.Batch_Num == item.value);
                    }
                    if (item.field == "Depart_Port" && !string.IsNullOrEmpty(item.value))
                    {
                        ops = ops.Where(x => x.Depart_Port == item.value);
                    }
                    if (item.field == "_Flight_Date_Want" && !string.IsNullOrEmpty(item.value))
                    {
                        var date = Convert.ToDateTime(item.value);
                        ops = ops.Where(x => x.Flight_Date_Want >= date);
                    }
                    if (item.field == "Flight_Date_Want_" && !string.IsNullOrEmpty(item.value))
                    {
                        var date = Convert.ToDateTime(item.value);
                        ops = ops.Where(x => x.Flight_Date_Want <= date);
                    }
                    if (item.field == "End_Port" && !string.IsNullOrEmpty(item.value))
                    {
                        ops = ops.Where(x => x.End_Port == item.value);
                    }
                    if (item.field == "ADDID" && !string.IsNullOrEmpty(item.value))
                    {
                        var valarr = item.value.Split(",");
                        ops = ops.Where(x => (valarr.Contains(x.ADDID) || valarr.Contains(x.ADDWHO)));
                    }
                    if (item.field == "Flight_No" && !string.IsNullOrEmpty(item.value))
                    {
                        ops = ops.Where(x => x.Flight_No == item.value);
                    }
                    if (item.field == "Is_BAS" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        if (boolval == true)
                            ops = ops.Where(x => x.Is_BAS == boolval);
                        else
                            ops = ops.Where(x => (x.Is_BAS == boolval || x.Is_BAS == null));
                    }
                    if (item.field == "Is_DCZ" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        if (boolval == true)
                            ops = ops.Where(x => x.Is_DCZ == boolval);
                        else
                            ops = ops.Where(x => (x.Is_DCZ == boolval || x.Is_DCZ == null));
                    }
                    if (item.field == "Is_ZB" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        if (boolval == true)
                            ops = ops.Where(x => x.Is_ZB == boolval);
                        else
                            ops = ops.Where(x => (x.Is_ZB == boolval || x.Is_ZB == null));
                    }
                    if (item.field == "Is_TG" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        if (boolval == true)
                            ops = ops.Where(x => x.Is_TG == boolval);
                        else
                            ops = ops.Where(x => (x.Is_TG == boolval || x.Is_TG == null));
                    }
                    if (item.field == "Is_HDQ" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        if (boolval == true)
                            ops = ops.Where(x => x.Is_HDQ == boolval);
                        else
                            ops = ops.Where(x => (x.Is_HDQ == boolval || x.Is_HDQ == null));
                    }
                    if (item.field == "Is_BG" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        if (boolval == true)
                            ops = ops.Where(x => x.Is_BG == boolval);
                        else
                            ops = ops.Where(x => (x.Is_BG == boolval || x.Is_BG == null));
                    }
                    if (item.field == "Is_BQ" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        if (boolval == true)
                            ops = ops.Where(x => x.Is_BQ == boolval);
                        else
                            ops = ops.Where(x => (x.Is_BQ == boolval || x.Is_BQ == null));
                    }
                    if (item.field == "Is_OutGoing" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        if (boolval == true)
                            ops = ops.Where(x => x.Is_OutGoing == boolval);
                        else
                            ops = ops.Where(x => (x.Is_OutGoing == boolval || x.Is_OutGoing == null));
                    }
                    if (item.field == "SendOut_ZD" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        if (boolval == true)
                            ops = ops.Where(x => x.SendOut_ZD == boolval);
                        else
                            ops = ops.Where(x => (x.SendOut_ZD == boolval || x.SendOut_ZD == null));
                    }
                    if (item.field == "SendOut_FD" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);
                        var QH = _unitOfWork.Repository<OPS_H_Order>().Queryable();
                        if (boolval == true)
                        {
                            var ops_h = (from h in QH
                                         where h.SendOut_FD == boolval
                                         select new
                                         {
                                             MBLId = h.MBLId
                                         }).AsQueryable().Distinct();
                            ops = (from n in ops
                                   join h in ops_h on n.Id equals h.MBLId
                                   select n).AsQueryable();
                        }
                        else
                        {
                            var ops_h = (from h in QH
                                         where h.SendOut_FD == boolval || h.SendOut_FD == null
                                         select new
                                         {
                                             MBLId = h.MBLId
                                         }).AsQueryable().Distinct();
                            ops = (from n in ops
                                   join h in ops_h on n.Id equals h.MBLId
                                   select n).AsQueryable();
                        }
                    }
                    if (item.field == "OPS_BMS_Status" && !string.IsNullOrEmpty(item.value))
                    {
                        var boolval = Convert.ToBoolean(item.value);

                        if (boolval == true)
                        {
                            ops = ops.Where(x => x.OPS_BMS_Status == boolval);
                        }
                        else
                        {
                            ops = ops.Where(x => x.OPS_BMS_Status == boolval || x.OPS_BMS_Status == null);
                        }
                    }
                    if (item.field == "SallerId")
                    {
                        int SallerId = 0;
                        int.TryParse(item.value, out SallerId);
                        if (SallerId > 0)
                            ops = ops.Where(x => x.SallerId == SallerId);
                        else
                            ops = ops.Where(x => x.SallerName.StartsWith(item.value));
                    }
                }
                #endregion
            }

            #endregion

            totalCount = ops.Count();

            decimal? totalPieces_Fact = 0;
            decimal? totalWeight_Fact = 0;
            decimal? totalVolume_Fact = 0;
            if (totalCount > 0)
            {
                //var ss = ops.Sum(x => x.Pieces_Fact);
                var Qsum = ops.GroupBy(r => 0).Select(g => new
                {
                    Pieces_Fact = g.Sum(r => r.Pieces_Fact),
                    Weight_Fact = g.Sum(r => r.Weight_Fact),
                    Volume_Fact = g.Sum(r => r.Volume_Fact)
                });
                var sum = Qsum.ToList();
                foreach (var item in sum)
                {
                    if (item.Pieces_Fact != null)
                    {
                        totalPieces_Fact = item.Pieces_Fact;
                    }
                    if (item.Weight_Fact != null)
                    {
                        totalWeight_Fact = item.Weight_Fact;
                    }
                    if (item.Volume_Fact != null)
                    {
                        totalVolume_Fact = item.Volume_Fact;
                    }
                }
            }

            var datarows = ops.OrderBy(sort, order).Skip((page - 1) * rows).Take(rows).ToList();

            #region 取缓存数据

            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            var ArrDealArticle = (List<DealArticle>)CacheHelper.Get_SetCache(Common.CacheNameS.DealArticle);
            var ArrCodeItem = (List<CodeItem>)CacheHelper.Get_SetCache(Common.CacheNameS.CodeItem);
            var ArrUsers = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetCache(Common.CacheNameS.ApplicationUser);

            #endregion

            #region 获取基础数据的名称

            var opsdata = from n in datarows
                          join a in ArrCusBusInfo on n.Airways_Code equals a.EnterpriseId into a_tmp
                          from atmp in a_tmp.DefaultIfEmpty()
                          join b in ArrCusBusInfo on n.FWD_Code equals b.EnterpriseId into b_tmp
                          from btmp in b_tmp.DefaultIfEmpty()
                          join c in ArrPARA_AirPort on n.Depart_Port equals c.PortCode into c_tmp
                          from ctmp in c_tmp.DefaultIfEmpty()
                          join d in ArrPARA_AirPort on n.End_Port equals d.PortCode into d_tmp
                          from dtmp in d_tmp.DefaultIfEmpty()
                          join e in ArrPARA_CURR on n.Currency_M equals e.CURR_CODE into e_tmp
                          from etmp in e_tmp.DefaultIfEmpty()
                          join f in ArrDealArticle on n.Bragainon_Article_M equals f.DealArticleCode into f_tmp
                          from ftmp in f_tmp.DefaultIfEmpty()
                          join g in ArrCodeItem on n.Pay_Mode_M equals g.Code into g_tmp
                          from gtmp in g_tmp.DefaultIfEmpty()
                          join h in ArrCusBusInfo on n.Entrustment_Name equals h.EnterpriseId into h_tmp
                          from htmp in h_tmp.DefaultIfEmpty()
                          join i in ArrCusBusInfo on n.Book_Flat_Code equals i.EnterpriseId into i_tmp
                          from itmp in i_tmp.DefaultIfEmpty()
                          join j in ArrUsers on n.ADDWHO equals j.UserName into j_tmp
                          from jtmp in j_tmp.DefaultIfEmpty()
                          select new
                          {
                              Id = n.Id,
                              WTId = n.WTId,
                              MBL = n.MBL,
                              HBL = n.HBL,
                              //Airways_Code = n.Airways_Code,
                              Airways_Code = (atmp == null ? "" : atmp.EnterpriseShortName),
                              FWD_Code = n.FWD_Code,
                              FWD_CodeNAME = (btmp == null ? "" : btmp.EnterpriseShortName),
                              Shipper_M = n.Shipper_M,
                              Consignee_M = n.Consignee_M,
                              Notify_Part_M = n.Notify_Part_M,
                              Depart_Port = n.Depart_Port,
                              Depart_PortNAME = (ctmp == null ? "" : n.Depart_Port + "|" + ctmp.PortNameEng),
                              End_Port = n.End_Port,
                              End_PortNAME = (dtmp == null ? "" : n.End_Port + "|" + dtmp.PortNameEng),
                              Flight_No = n.Flight_No,
                              Flight_Date_Want = n.Flight_Date_Want,
                              Currency_M = n.Currency_M,
                              Currency_MNAME = (etmp == null ? "" : etmp.CURR_Name),
                              Bragainon_Article_M = n.Bragainon_Article_M,
                              Bragainon_Article_MNAME = (ftmp == null ? "" : ftmp.DealArticleName),
                              Pay_Mode_M = n.Pay_Mode_M,
                              Pay_Mode_MNAME = (gtmp == null ? "" : gtmp.Text),
                              Carriage_M = n.Carriage_M,
                              Incidental_Expenses_M = n.Incidental_Expenses_M,
                              Declare_Value_Trans_M = n.Declare_Value_Trans_M,
                              Declare_Value_Ciq_M = n.Declare_Value_Ciq_M,
                              Risk_M = n.Risk_M,
                              Marks_M = n.Marks_M,
                              EN_Name_M = n.EN_Name_M,
                              Hand_Info_M = n.Hand_Info_M,
                              Signature_Agent_M = n.Signature_Agent_M,
                              Rate_Class_M = n.Rate_Class_M,
                              Air_Frae = n.Air_Frae,
                              AWC = n.AWC,
                              Pieces_M = n.Pieces_M,
                              Weight_M = n.Weight_M,
                              Volume_M = n.Volume_M,
                              Charge_Weight_M = n.Charge_Weight_M,
                              Price_Article = n.Price_Article,
                              CCC = n.CCC,
                              File_M = n.File_M,
                              Status = n.Status,
                              OperatingPoint = n.OperatingPoint,
                              Operation_Id = n.Operation_Id,
                              Entrustment_Code = n.Entrustment_Code,
                              //Entrustment_Name = n.Entrustment_Name,
                              Entrustment_Name = (htmp == null ? "" : htmp.EnterpriseShortName),
                              Is_Book_Flat = n.Is_Book_Flat,
                              //Book_Flat_Code = n.Book_Flat_Code,
                              Book_Flat_Code = (itmp == null ? "" : itmp.EnterpriseShortName),
                              Batch_Num = n.Batch_Num,
                              ADDID = n.ADDID,
                              //ADDWHO = n.ADDWHO,
                              ADDWHO = (jtmp == null ? "" : jtmp.UserNameDesc),
                              Is_BAS = n.Is_BAS,
                              Is_DCZ = n.Is_DCZ,
                              Is_ZB = n.Is_ZB,
                              Is_TG = n.Is_TG,
                              Is_HDQ = n.Is_HDQ,
                              Is_BG = n.Is_BG,
                              Is_BQ = n.Is_BQ,
                              Is_OutGoing = n.Is_OutGoing,
                              SendOut_ZD = n.SendOut_ZD,
                              Pieces_Fact = n.Pieces_Fact,
                              Weight_Fact = n.Weight_Fact,
                              Volume_Fact = n.Volume_Fact,
                              Pieces_SK = n.Pieces_SK,
                              Weight_SK = n.Weight_SK,
                              Volume_SK = n.Volume_SK,
                              Is_Self = n.Is_Self,
                              Remark = n.Remark,
                              n.SallerId,
                              n.SallerName,
                          };

            #endregion

            List<dynamic> footerlist = new List<dynamic> {    //footer要为json数组，这里用list
                new {totalCount= totalCount,totalPieces_Fact=totalPieces_Fact,totalWeight_Fact=totalWeight_Fact,totalVolume_Fact=totalVolume_Fact},
             };
            var pagelist = new { total = totalCount, rows = opsdata, footer = footerlist };
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

        [HttpPost]
        public ActionResult SaveData(OPS_M_OrderChangeViewModel ops_m_order)
        {
            if (ops_m_order.updated != null)
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

                foreach (var updated in ops_m_order.updated)
                {
                    _oPS_M_OrderService.Update(updated);
                }
            }
            if (ops_m_order.deleted != null)
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

                foreach (var deleted in ops_m_order.deleted)
                {
                    _oPS_M_OrderService.Delete(deleted);
                }
            }
            if (ops_m_order.inserted != null)
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

                foreach (var inserted in ops_m_order.inserted)
                {
                    _oPS_M_OrderService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((ops_m_order.updated != null && ops_m_order.updated.Any()) ||
                (ops_m_order.deleted != null && ops_m_order.deleted.Any()) ||
                (ops_m_order.inserted != null && ops_m_order.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(ops_m_order);
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

        // GET: OPS_M_Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPS_M_Order oPS_M_Order = _oPS_M_OrderService.Find(id);
            if (oPS_M_Order == null)
            {
                return HttpNotFound();
            }
            return View(oPS_M_Order);
        }

        //新增和复制新增
        // GET: OPS_M_Orders/Create
        public ActionResult Create(int? Oid = 0)
        {
            OPS_M_Order oPS_M_Order = new OPS_M_Order();
            OPS_EntrustmentInfor oPS_EntrustmentInfor = new OPS_EntrustmentInfor();
            OPS_H_Order oPS_H_Order = new OPS_H_Order();
            var AirTime = "";//航班时间
            if (Oid > 0)
            {
                var oPS_M_OrderEdit = _oPS_M_OrderService.Queryable().Where(x => x.Id == Oid)
                .Include(x => x.OPS_EntrustmentInfors)
                .Include(x => x.OPS_H_Orders)
                .FirstOrDefault();

                if (oPS_M_OrderEdit == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    //复制主单信息
                    oPS_M_Order.Consignee_M = oPS_M_OrderEdit.Consignee_M;
                    oPS_M_Order.Airways_Code = oPS_M_OrderEdit.Airways_Code;
                    oPS_M_Order.Flight_No = oPS_M_OrderEdit.Flight_No;
                    oPS_M_Order.FWD_Code = oPS_M_OrderEdit.FWD_Code;
                    oPS_M_Order.Depart_Port = oPS_M_OrderEdit.Depart_Port;
                    oPS_M_Order.End_Port = oPS_M_OrderEdit.End_Port;
                    oPS_M_Order.Flight_Date_Want = oPS_M_OrderEdit.Flight_Date_Want;

                    //复制委托信息
                    foreach (var item in oPS_M_OrderEdit.OPS_EntrustmentInfors)
                    {
                        oPS_EntrustmentInfor = new OPS_EntrustmentInfor();
                        oPS_EntrustmentInfor.Is_Self = item.Is_Self;
                        oPS_EntrustmentInfor.Is_DCZ = item.Is_DCZ;
                        oPS_EntrustmentInfor.Consign_Code = item.Consign_Code;
                        oPS_EntrustmentInfor.Entrustment_Name = item.Entrustment_Name;
                        oPS_EntrustmentInfor.Entrustment_Code = item.Entrustment_Code;
                        oPS_EntrustmentInfor.FWD_Code = item.FWD_Code;
                        oPS_EntrustmentInfor.Consignee_Code = item.Consignee_Code;
                        oPS_EntrustmentInfor.Carriage_Account_Code = item.Carriage_Account_Code;
                        oPS_EntrustmentInfor.Depart_Port = item.Depart_Port;
                        oPS_EntrustmentInfor.Transfer_Port = item.Transfer_Port;
                        oPS_EntrustmentInfor.End_Port = item.End_Port;
                        oPS_EntrustmentInfor.Remark = item.Remark;
                        oPS_EntrustmentInfor.Is_Book_Flat = item.Is_Book_Flat;
                        oPS_EntrustmentInfor.Bulk_Percent_DC = item.Bulk_Percent_DC;//收款分单 分泡比
                        oPS_EntrustmentInfor.Book_Flat_Code = item.Book_Flat_Code;//订舱主单 分泡比
                        oPS_EntrustmentInfor.Bulk_Percent_SK = item.Bulk_Percent_SK;
                        oPS_EntrustmentInfor.Airways_Code = item.Airways_Code;
                        oPS_EntrustmentInfor.Flight_No = item.Flight_No;
                        oPS_EntrustmentInfor.Flight_Date_Want = item.Flight_Date_Want;
                        oPS_EntrustmentInfor.Delivery_Point = item.Delivery_Point;
                        oPS_EntrustmentInfor.Shipper_H = item.Shipper_H;
                        oPS_EntrustmentInfor.Consignee_H = item.Consignee_H;
                        oPS_EntrustmentInfor.Notify_Part_H = item.Notify_Part_H;

                        oPS_M_Order.OPS_EntrustmentInfors.Add(oPS_EntrustmentInfor);
                    }
                    //复制分单信息
                    foreach (var item in oPS_M_OrderEdit.OPS_H_Orders)
                    {
                        oPS_H_Order = new OPS_H_Order();
                        oPS_H_Order.Shipper_H = item.Shipper_H;
                        oPS_H_Order.Consignee_H = item.Consignee_H;
                        oPS_H_Order.Notify_Part_H = item.Notify_Part_H;
                        oPS_H_Order.Currency_H = item.Currency_H;
                        oPS_H_Order.Bragainon_Article_H = item.Bragainon_Article_H;
                        oPS_H_Order.Pay_Mode_H = item.Pay_Mode_H;
                        oPS_H_Order.Carriage_H = item.Carriage_H;
                        oPS_H_Order.Incidental_Expenses_H = item.Incidental_Expenses_H;
                        oPS_H_Order.Declare_Value_Trans_H = item.Declare_Value_Trans_H;
                        oPS_H_Order.Declare_Value_Ciq_H = item.Declare_Value_Ciq_H;
                        oPS_H_Order.Risk_H = item.Risk_H;
                        oPS_H_Order.Marks_H = item.Marks_H;
                        oPS_H_Order.EN_Name_H = item.EN_Name_H;

                        oPS_M_Order.OPS_H_Orders.Add(oPS_H_Order);
                    }
                }
            }
            else
            {
                //oPS_EntrustmentInfor.Flight_Date_Want = DateTime.Now;//取消航班日期默认值，当前时间
                oPS_EntrustmentInfor.Depart_Port = "PVG";
                //新增
                oPS_M_Order.Depart_Port = "PVG";
                oPS_M_Order.OPS_EntrustmentInfors.Add(oPS_EntrustmentInfor);
                //oPS_M_Order.Flight_Date_Want = DateTime.Now;
                oPS_M_Order.Pay_Mode_M = "PP";
                oPS_H_Order.Declare_Value_Trans_H = "NVD";
                oPS_H_Order.Declare_Value_Ciq_H = "NCV";
                oPS_H_Order.Risk_H = "XXX";
                oPS_M_Order.OPS_H_Orders.Add(oPS_H_Order);
                ViewBag.h_order = new OPS_H_Order();
            }
            var ODynamic = GetFromNAME(oPS_M_Order, 0);
            if (Oid <= 0)
            {
                ODynamic.Depart_PortNAME = "PVG|PUDONG";
            }
            ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);

            if (!string.IsNullOrEmpty(oPS_EntrustmentInfor.Flight_No))
            {
                var AirLine = _unitOfWork.RepositoryAsync<PARA_AirLine>().Queryable().Where(x => x.AirCode == oPS_EntrustmentInfor.Flight_No).FirstOrDefault();
                if (AirLine != null)
                {
                    AirTime = AirLine.AirTime;
                }
            }
            ViewBag.AirTime = AirTime;
            //set default value
            return View(oPS_M_Order);
        }

        // POST: OPS_M_Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(OPS_M_Order oPS_M_Order)
        {
            if (ModelState.IsValid)
            {
                _oPS_M_OrderService.Insert(oPS_M_Order);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a OPS_M_Order record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(oPS_M_Order);
        }

        // GET: OPS_EntrustmentInfors/Create
        public ActionResult AddEntrustmentInforEditFore(int? id = 0, int i = 0, int tabindex = 0, int mblid = 0)
        {
            OPS_EntrustmentInfor oPS_EntrustmentInfor = new OPS_EntrustmentInfor();
            ViewBag.AirTime = "";//航班时间
            if (id == 0)
            {//新增
                oPS_EntrustmentInfor.Id = -i;
                //oPS_EntrustmentInfor.Operation_Id = SequenceBuilder.NextEntrustmentInforOperation_IdSerial_No();
                if (mblid > 0)
                {
                    var oPS_M_Order = _oPS_M_OrderService.Find(mblid);
                    if (oPS_M_Order != null)
                    {
                        oPS_EntrustmentInfor.MBLId = oPS_M_Order.Id;
                        oPS_EntrustmentInfor.MBL = oPS_M_Order.MBL;
                        oPS_EntrustmentInfor.Airways_Code = oPS_M_Order.Airways_Code;
                        oPS_EntrustmentInfor.FWD_Code = oPS_M_Order.FWD_Code;
                        oPS_EntrustmentInfor.Depart_Port = oPS_M_Order.Depart_Port;
                        oPS_EntrustmentInfor.End_Port = oPS_M_Order.End_Port;
                        oPS_EntrustmentInfor.Flight_No = oPS_M_Order.Flight_No;
                        oPS_EntrustmentInfor.Flight_Date_Want = oPS_M_Order.Flight_Date_Want;
                        oPS_EntrustmentInfor.Notify_Part_M = oPS_M_Order.Notify_Part_M;
                        oPS_EntrustmentInfor.Shipper_M = oPS_M_Order.Shipper_M;
                        oPS_EntrustmentInfor.Pieces_DC = oPS_M_Order.Pieces_M;
                        oPS_EntrustmentInfor.Weight_DC = oPS_M_Order.Weight_M;
                        oPS_EntrustmentInfor.Volume_DC = oPS_M_Order.Volume_M;
                        oPS_EntrustmentInfor.Consignee_M = oPS_M_Order.Consignee_M;
                    }
                    oPS_M_Order.OPS_EntrustmentInfors.Add(oPS_EntrustmentInfor);
                    var ODynamic = GetFromNAME(oPS_M_Order, i - 1);
                    ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
                }
            }
            else
            {
                oPS_EntrustmentInfor = _oPS_EntrustmentInforService.Find(id);
                var oPS_M_Order = _oPS_M_OrderService.Find(oPS_EntrustmentInfor.MBLId);
                if (oPS_M_Order != null)
                {
                    var ODynamic = GetFromNAME(oPS_M_Order, i - 1);
                    ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
                }
            }
            ViewBag.i = i;
            ViewBag.tabindex = tabindex;
            //set default value
            return View(oPS_EntrustmentInfor);
        }

        // GET: OPS_EntrustmentInfors/Create
        public ActionResult AddHOrderEditFore(int? id = 0, int i = 0, int tabindex = 0, int mblid = 0)
        {
            OPS_M_Order oPS_M_Order = new OPS_M_Order();
            OPS_H_Order oPS_H_Order = new OPS_H_Order();
            ViewBag.AirTime = "";//航班时间

            if (id == 0)
            {//新增
                if (mblid > 0)
                {
                    oPS_M_Order = _oPS_M_OrderService.Find(mblid);
                    if (oPS_M_Order != null)
                    {
                        oPS_H_Order.MBLId = oPS_M_Order.Id;
                        oPS_H_Order.MBL = oPS_M_Order.MBL;
                    }
                }
                for (int k = 0; k < i - 1; k++)
                {
                    OPS_H_Order item = new OPS_H_Order();
                    item.Id = -1 - k;
                    oPS_M_Order.OPS_H_Orders.Add(item);
                }
                //oPS_H_Order.Operation_Id = SequenceBuilder.NextEntrustmentInforOperation_IdSerial_No();
                oPS_H_Order.Pay_Mode_H = "PP";
                oPS_H_Order.Carriage_H = "P";
                oPS_H_Order.Incidental_Expenses_H = "P";
                oPS_H_Order.Declare_Value_Trans_H = "NVD";
                oPS_H_Order.Declare_Value_Ciq_H = "NCV";
                oPS_H_Order.Risk_H = "XXX";
                oPS_M_Order.OPS_H_Orders.Add(oPS_H_Order);
                var ODynamic = GetFromNAME(oPS_M_Order, i - 1);
                ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            }
            else
            {
                oPS_H_Order = _oPS_H_OrderService.Find(id);
                if (oPS_H_Order != null)
                {
                    if (!string.IsNullOrEmpty(oPS_H_Order.Shipper_H))
                    {
                        oPS_H_Order.Shipper_H = Common.GetContactsStringView(oPS_H_Order.Shipper_H);
                    }
                    if (!string.IsNullOrEmpty(oPS_H_Order.Consignee_H))
                    {
                        oPS_H_Order.Consignee_H = Common.GetContactsStringView(oPS_H_Order.Consignee_H);
                    }
                    if (!string.IsNullOrEmpty(oPS_H_Order.Notify_Part_H))
                    {
                        oPS_H_Order.Notify_Part_H = Common.GetContactsStringView(oPS_H_Order.Notify_Part_H);
                    }

                    if (mblid > 0)
                    {
                        oPS_M_Order = _oPS_M_OrderService.Queryable().Where(x => x.Id == mblid).FirstOrDefault();
                    }
                    else
                    {
                        oPS_M_Order = _oPS_M_OrderService.Queryable().Where(x => x.Id == oPS_H_Order.MBLId).FirstOrDefault();
                    }
                    if (oPS_M_Order != null)
                    {
                        var ops_h_orderlist = _oPS_H_OrderService.Queryable().Where(x => x.MBLId == oPS_M_Order.Id).OrderBy(x => x.Id).ToList();
                        var intk = i - ops_h_orderlist.Count;
                        for (var k = 0; k < intk; k++)
                        {
                            if (mblid > 0)
                            {
                                ops_h_orderlist.Add(oPS_H_Order);
                            }
                        }
                        oPS_M_Order.OPS_H_Orders = ops_h_orderlist;
                        var ODynamic = GetFromNAME(oPS_M_Order, i - 1);
                        ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
                        //oPS_M_Order.OPS_H_Orders.Add(oPS_H_Order);
                    }
                }
            }
            ViewBag.oPS_H_Order = oPS_H_Order;
            ViewBag.i = i;
            ViewBag.tabindex = tabindex;
            var hbl_picture = new List<Picture>();
            if (oPS_M_Order != null)
            {
                if (oPS_M_Order != null & oPS_M_Order.OPS_H_Orders.Count > 0)
                {
                    var k = 0;
                    foreach (var item in oPS_M_Order.OPS_H_Orders)
                    {
                        if (k != i - 1)
                        {
                            k++;
                            continue;
                        }
                        var h_order = item;
                        if (!string.IsNullOrEmpty(h_order.HBL))
                        {
                            var code = h_order.HBL + h_order.Id.ToString();
                            var picture = _pictureService.Queryable().Where(x => x.Code.Equals(code) && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable && x.Type == AirOutEnumType.PictureTypeEnum.Fileupload_HBL).ToList();
                            if (picture != null)
                            {
                                hbl_picture = picture;
                            }
                        }
                    }
                }
            }
            ViewBag.hbl_picture = hbl_picture;
            //set default value
            return View(oPS_M_Order);
        }

        /// <summary>
        /// 保存接单数据（委托-主单-分单 信息）
        /// </summary>
        /// <param name="OPS_M_Order"></param>
        /// <param name="DeleteWTXX"></param>
        /// <param name="DeleteFDXX"></param>
        /// <param name="Is_BindingWarehouse"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SaveEMH(OPS_M_Order OPS_M_Order, string DeleteWTXX = "", string DeleteFDXX = "", string Is_BindingWarehouse = "")//OPS_EntrustmentInfor EntrustmentInfor, OPS_M_Order M_Order, OPS_H_Order H_Order
        {
            try
            {
                //验证重复提交
                var ret = RepeatSubmit_Post();
                if (!ret.Success)
                {
                    return Json(new { success = false, err = ret.RetMsg, ActionGuid = ret.ActionGuid, ActionGuidName = ret.ActionGuidName }, JsonRequestBehavior.AllowGet);
                }


                string ErrStr = "";
                var Operation_Id = "";//委托编号
                bool is_self = false;
                var m_operation = ChangeOrderHistory.EnumChangeType.UnChange;//记录主单是什么操作
                var e_operation = ChangeOrderHistory.EnumChangeType.UnChange;//记录委托是什么操作
                //var h_operation_add = ChangeOrderHistory.EnumChangeType.UnChange;//记录分单插入操作
                //var h_operation_add_num = 0;//记录分单插入条数
                //var h_operation_edit = ChangeOrderHistory.EnumChangeType.UnChange;//记录分单修改操作
                //var h_operation_edit_num = 0;//记录分单修改条数

                List<string> excludeM = new List<string>() { "Id", "OPS_H_Orders", "OPS_EntrustmentInfors", "OperatingPoint", "ADDID", "ADDWHO", "ADDTS", "EDITWHO", "EDITTS", "EDITID" };
                List<string> excludeE = new List<string>() { "Id", "OPS_M_Order", "MBLId", "Custom_Code", "Area_Code", "Shipper_H", "Consignee_H", "Notify_Part_H", "Shipper_M", "Consignee_M", "Notify_Part_M", "OperatingPoint", "ADDID", "ADDWHO", "ADDTS", "EDITWHO", "EDITTS", "EDITID" };

                var ContentM = ""; //记录总单修改的数据
                var ContentE = ""; //记录委托单修改的数据
                var ContentH_add = ""; //记录分单新增的数据
                var ContentH_edit = ""; //记录分单修改的数据

                if (OPS_M_Order == null)
                {
                    return Json(new { success = false, err = "主单数据为空" }, JsonRequestBehavior.AllowGet);
                }
                if (OPS_M_Order.Flight_Date_Want == null || OPS_M_Order.OPS_EntrustmentInfors.Any(x => !x.Flight_Date_Want.HasValue))
                {
                    return Json(new { success = false, err = "航班日期不能为空！" }, JsonRequestBehavior.AllowGet);
                }
                OPS_M_Order.MBL = Common.RemoveNotNumber(OPS_M_Order.MBL);

                #region 总单 发货人 收货人 通知人 换行符转换

                //if (!string.IsNullOrEmpty(OPS_M_Order.Shipper_M))
                //{
                //    OPS_M_Order.Shipper_M = Common.GetContactsString(OPS_M_Order.Shipper_M);
                //}
                //if (!string.IsNullOrEmpty(OPS_M_Order.Consignee_M))
                //{
                //    OPS_M_Order.Consignee_M = Common.GetContactsString(OPS_M_Order.Consignee_M);
                //}
                //if (!string.IsNullOrEmpty(OPS_M_Order.Notify_Part_M))
                //{
                //    OPS_M_Order.Notify_Part_M = Common.GetContactsString(OPS_M_Order.Notify_Part_M); ;
                //}

                #endregion

                OPS_EntrustmentInfor OOPS_EntrustmentInfor = null;//创建
                if (OPS_M_Order.Id == 0)
                {
                    m_operation = ChangeOrderHistory.EnumChangeType.Insert;
                    ContentM = OPS_M_Order.MBL;

                    var Flight_Date_Want = OPS_M_Order.OPS_EntrustmentInfors.FirstOrDefault().Flight_Date_Want;
                    Operation_Id = SequenceBuilder.NextEntrustmentInforOperation_IdSerial_No(Flight_Date_Want);

                    #region 接单新增

                    foreach (var EntrustmentInfor in OPS_M_Order.OPS_EntrustmentInfors)
                    {
                        OOPS_EntrustmentInfor = EntrustmentInfor;
                        EntrustmentInfor.MBL = Common.RemoveNotNumber(EntrustmentInfor.MBL);
                        if (EntrustmentInfor.Id <= 0)
                        {
                            e_operation = ChangeOrderHistory.EnumChangeType.Insert;
                            ContentE = EntrustmentInfor.Operation_Id;
                            EntrustmentInfor.MBLId = OPS_M_Order.Id;
                            EntrustmentInfor.Operation_Id = Operation_Id;
                            is_self = EntrustmentInfor.Is_Self;
                            if (EntrustmentInfor.Is_Self)
                            {
                                if (string.IsNullOrEmpty(EntrustmentInfor.HBL))
                                    EntrustmentInfor.HBL = Operation_Id;
                            }
                            _oPS_EntrustmentInforService.Insert(EntrustmentInfor);
                        }
                        else
                        {
                            #region 新增主单时，修改保存手动绑定的委托信息

                            e_operation = ChangeOrderHistory.EnumChangeType.Modify;
                            ContentE = EntrustmentInfor.Operation_Id;

                            var item = _oPS_EntrustmentInforService.Find(EntrustmentInfor.Id);
                            //ContentE = Common.DifferenceComparison<OPS_EntrustmentInfor>(item, EntrustmentInfor, excludeE);
                            ContentE = _oPS_EntrustmentInforService.DifferenceComparisonE(item, EntrustmentInfor);

                            item.MBLId = OPS_M_Order.Id;
                            item.Consign_Code = EntrustmentInfor.Consign_Code;
                            item.Consign_Code = EntrustmentInfor.Consign_Code;
                            item.Custom_Code = EntrustmentInfor.Custom_Code;
                            item.Operation_Id = EntrustmentInfor.Operation_Id;
                            item.Entrustment_Name = EntrustmentInfor.Entrustment_Name;
                            item.Entrustment_Code = EntrustmentInfor.Entrustment_Code;
                            item.FWD_Code = EntrustmentInfor.FWD_Code;
                            item.Consignee_Code = EntrustmentInfor.Consignee_Code;
                            item.Carriage_Account_Code = EntrustmentInfor.Carriage_Account_Code;
                            item.Incidental_Account_Code = EntrustmentInfor.Incidental_Account_Code;
                            item.Depart_Port = EntrustmentInfor.Depart_Port;
                            item.Transfer_Port = EntrustmentInfor.Transfer_Port;
                            item.End_Port = EntrustmentInfor.End_Port;

                            #region 换行符转换

                            //if (EntrustmentInfor.Shipper_H != null)
                            //{
                            //    EntrustmentInfor.Shipper_H = Common.GetContactsString(EntrustmentInfor.Shipper_H);
                            //}
                            //if (EntrustmentInfor.Consignee_H != null)
                            //{
                            //    EntrustmentInfor.Consignee_H = Common.GetContactsString(EntrustmentInfor.Consignee_H);
                            //}
                            //if (EntrustmentInfor.Notify_Part_H != null)
                            //{
                            //    EntrustmentInfor.Notify_Part_H = Common.GetContactsString(EntrustmentInfor.Notify_Part_H);
                            //}

                            //if (EntrustmentInfor.Shipper_M != null)
                            //{
                            //    EntrustmentInfor.Shipper_M = Common.GetContactsString(EntrustmentInfor.Shipper_M);
                            //}
                            //if (EntrustmentInfor.Consignee_M != null)
                            //{
                            //    EntrustmentInfor.Consignee_M = Common.GetContactsString(EntrustmentInfor.Consignee_M);
                            //}
                            //if (EntrustmentInfor.Notify_Part_M != null)
                            //{
                            //    EntrustmentInfor.Notify_Part_M = Common.GetContactsString(EntrustmentInfor.Notify_Part_M);
                            //}

                            #endregion

                            //主/分单 发货人 收货人 通知人
                            item.Shipper_H = EntrustmentInfor.Shipper_H;
                            item.Consignee_H = EntrustmentInfor.Consignee_H;
                            item.Notify_Part_H = EntrustmentInfor.Notify_Part_H;
                            item.Shipper_M = EntrustmentInfor.Shipper_M;
                            item.Consignee_M = EntrustmentInfor.Consignee_M;
                            item.Notify_Part_M = EntrustmentInfor.Notify_Part_M;

                            item.Pieces_TS = EntrustmentInfor.Pieces_TS;
                            item.Weight_TS = EntrustmentInfor.Weight_TS;
                            item.Pieces_SK = EntrustmentInfor.Pieces_SK;
                            item.Slac_SK = EntrustmentInfor.Slac_SK;
                            item.Weight_SK = EntrustmentInfor.Weight_SK;
                            item.Pieces_DC = EntrustmentInfor.Pieces_DC;
                            item.Slac_DC = EntrustmentInfor.Slac_DC;
                            item.Weight_DC = EntrustmentInfor.Weight_DC;
                            item.IS_MoorLevel = EntrustmentInfor.IS_MoorLevel;
                            item.MoorLevel = EntrustmentInfor.MoorLevel;
                            item.Volume_TS = EntrustmentInfor.Volume_TS;
                            item.Charge_Weight_TS = EntrustmentInfor.Charge_Weight_TS;
                            item.Bulk_Weight_TS = EntrustmentInfor.Bulk_Weight_TS;
                            item.Volume_SK = EntrustmentInfor.Volume_SK;
                            item.Charge_Weight_SK = EntrustmentInfor.Charge_Weight_SK;
                            item.Bulk_Weight_SK = EntrustmentInfor.Bulk_Weight_SK;
                            item.Bulk_Percent_SK = EntrustmentInfor.Bulk_Percent_SK;
                            item.Account_Weight_SK = EntrustmentInfor.Account_Weight_SK;
                            item.Volume_DC = EntrustmentInfor.Volume_DC;
                            item.Charge_Weight_DC = EntrustmentInfor.Charge_Weight_DC;
                            item.Bulk_Weight_DC = EntrustmentInfor.Bulk_Weight_DC;
                            item.Bulk_Percent_DC = EntrustmentInfor.Bulk_Percent_DC;
                            item.Account_Weight_DC = EntrustmentInfor.Account_Weight_DC;
                            //item.Pieces_Fact = EntrustmentInfor.Pieces_Fact;
                            //item.Weight_Fact = EntrustmentInfor.Weight_Fact;
                            //item.Volume_Fact = EntrustmentInfor.Volume_Fact;
                            //item.Charge_Weight_Fact = EntrustmentInfor.Charge_Weight_Fact;
                            //item.Bulk_Weight_Fact = EntrustmentInfor.Bulk_Weight_Fact;
                            //item.Bulk_Percent_Fact = EntrustmentInfor.Bulk_Percent_Fact;
                            //item.Account_Weight_Fact = EntrustmentInfor.Account_Weight_Fact;
                            item.Marks_H = EntrustmentInfor.Marks_H;
                            item.EN_Name_H = EntrustmentInfor.EN_Name_H;
                            item.Is_Book_Flat = EntrustmentInfor.Is_Book_Flat;
                            item.Book_Flat_Code = EntrustmentInfor.Book_Flat_Code;
                            item.Airways_Code = EntrustmentInfor.Airways_Code;
                            item.Flight_No = EntrustmentInfor.Flight_No;
                            item.MBL = Common.RemoveNotNumber(EntrustmentInfor.MBL);
                            item.HBL = EntrustmentInfor.HBL;
                            item.Flight_Date_Want = EntrustmentInfor.Flight_Date_Want;
                            item.Book_Remark = EntrustmentInfor.Book_Remark;
                            item.Delivery_Point = EntrustmentInfor.Delivery_Point;
                            item.Warehouse_Code = EntrustmentInfor.Warehouse_Code;
                            item.RK_Date = EntrustmentInfor.RK_Date;
                            item.CK_Date = EntrustmentInfor.CK_Date;
                            item.CH_Name = EntrustmentInfor.CH_Name;
                            item.AMS = EntrustmentInfor.AMS;
                            item.Lot_No = EntrustmentInfor.Lot_No;
                            item.Is_Self = EntrustmentInfor.Is_Self;
                            item.Ty_Type = EntrustmentInfor.Ty_Type;
                            item.Lot_No = EntrustmentInfor.Lot_No;
                            item.Hbl_Feight = EntrustmentInfor.Hbl_Feight;
                            item.Is_XC = EntrustmentInfor.Is_XC;
                            item.Is_BAS = EntrustmentInfor.Is_BAS;
                            item.Is_DCZ = EntrustmentInfor.Is_DCZ;
                            item.Is_ZB = EntrustmentInfor.Is_ZB;
                            item.Is_TG = EntrustmentInfor.Is_TG;
                            item.ADDPoint = EntrustmentInfor.ADDPoint;
                            item.EDITPoint = EntrustmentInfor.EDITPoint;
                            item.Batch_Num = EntrustmentInfor.Batch_Num;
                            item.Status = EntrustmentInfor.Status;
                            item.Remark = EntrustmentInfor.Remark;
                            item.OperatingPoint = EntrustmentInfor.OperatingPoint;
                            item.Is_HDQ = EntrustmentInfor.Is_HDQ;
                            item.ObjectState = ObjectState.Modified;

                            ErrStr += saveWarehousReceipts(item, EntrustmentInfor, OPS_M_Order.Id, Is_BindingWarehouse);
                            item.Marks_H = "";

                            _oPS_EntrustmentInforService.Update(item);

                            #endregion
                        }

                        #region 保存主单/分单 发货人，收货人和通知人 委托方Entrustment_Name

                        //主单发货人
                        _contactsService.InsertContact(EntrustmentInfor.Shipper_M, "shipper_m", EntrustmentInfor.FWD_Code);
                        ////主单收货人
                        //_contactsService.InsertContact(EntrustmentInfor.Consignee_M, "consignee_m", EntrustmentInfor.FWD_Code);
                        //主单通知人
                        _contactsService.InsertContact(EntrustmentInfor.Notify_Part_M, "notify_part_m", EntrustmentInfor.FWD_Code);

                        //分单发货人
                        _contactsService.InsertContact(EntrustmentInfor.Shipper_H, "shipper_h", EntrustmentInfor.Entrustment_Name);
                        //分单收货人
                        _contactsService.InsertContact(EntrustmentInfor.Consignee_H, "consignee_h", EntrustmentInfor.Entrustment_Name);
                        //分单通知人
                        _contactsService.InsertContact(EntrustmentInfor.Notify_Part_H, "notify_part_h", EntrustmentInfor.Entrustment_Name);

                        #endregion
                    }
                    foreach (var H_Order in OPS_M_Order.OPS_H_Orders)
                    {
                        H_Order.MBL = Common.RemoveNotNumber(H_Order.MBL);
                        if (H_Order.Id <= 0)
                        {
                            H_Order.ObjectState = ObjectState.Added;
                            //h_operation_add_num = h_operation_add_num + 1;
                            //h_operation_add = ChangeOrderHistory.EnumChangeType.Insert;
                            ContentH_add = H_Order.HBL;

                            H_Order.MBLId = OPS_M_Order.Id;
                            H_Order.Operation_Id = Operation_Id;
                            if (is_self)
                            {
                                if (string.IsNullOrEmpty(H_Order.HBL))
                                    H_Order.HBL = Operation_Id;
                            }
                            _oPS_H_OrderService.Insert(H_Order);
                        }
                        else
                        {
                            #region 新增主单时，修改保存手动绑定的分单信息

                            H_Order.ObjectState = ObjectState.Modified;
                            //h_operation_edit_num = h_operation_edit_num + 1;
                            //h_operation_edit = ChangeOrderHistory.EnumChangeType.Modify;
                            ContentH_edit = H_Order.HBL;

                            var item = _oPS_H_OrderService.Find(H_Order.Id);
                            item.MBLId = OPS_M_Order.Id;
                            item.MBL = Common.RemoveNotNumber(H_Order.MBL);

                            #region 保存分单 发货人 收货人 通知人 委托方Entrustment_Code

                            ////分单发货人
                            //_contactsService.InsertContact(H_Order.Shipper_H, "shipper_h", OPS_M_Order.FWD_Code);
                            ////分单收货人
                            //_contactsService.InsertContact(H_Order.Consignee_H, "consignee_h", OPS_M_Order.FWD_Code);
                            ////分单通知人
                            //_contactsService.InsertContact(H_Order.Notify_Part_H, "notify_part_h", OPS_M_Order.FWD_Code);

                            #endregion

                            #region 换行符转换

                            //if (H_Order.Shipper_H != null)
                            //{
                            //    H_Order.Shipper_H = Common.GetContactsString(H_Order.Shipper_H);
                            //}
                            //if (H_Order.Consignee_H != null)
                            //{
                            //    H_Order.Consignee_H = Common.GetContactsString(H_Order.Consignee_H);
                            //}
                            //if (H_Order.Notify_Part_H != null)
                            //{
                            //    H_Order.Notify_Part_H = Common.GetContactsString(H_Order.Notify_Part_H);
                            //}

                            #endregion

                            //分单 发货人 收货人 通知人
                            item.Shipper_H = H_Order.Shipper_H;
                            item.Consignee_H = H_Order.Consignee_H;
                            item.Notify_Part_H = H_Order.Notify_Part_H;

                            item.Currency_H = H_Order.Currency_H;
                            item.Bragainon_Article_H = H_Order.Bragainon_Article_H;
                            item.Pay_Mode_H = H_Order.Pay_Mode_H;
                            item.Carriage_H = H_Order.Carriage_H;
                            item.Incidental_Expenses_H = H_Order.Incidental_Expenses_H;
                            item.Declare_Value_Trans_H = H_Order.Declare_Value_Trans_H;
                            item.Declare_Value_Ciq_H = H_Order.Declare_Value_Ciq_H;
                            item.Risk_H = H_Order.Risk_H;
                            item.Marks_H = H_Order.Marks_H;
                            item.EN_Name_H = H_Order.EN_Name_H;
                            item.Pieces_H = H_Order.Pieces_H;
                            item.Weight_H = H_Order.Weight_H;
                            item.Volume_H = H_Order.Volume_H;
                            item.Charge_Weight_H = H_Order.Charge_Weight_H;
                            if (is_self)
                            {
                                H_Order.HBL = Operation_Id;
                            }
                            else
                            {
                                item.HBL = H_Order.HBL;
                            }
                            item.Operation_Id = Operation_Id;
                            item.Is_Self = H_Order.Is_Self;
                            item.Ty_Type = H_Order.Ty_Type;
                            item.Lot_No = H_Order.Lot_No;
                            item.Hbl_Feight = H_Order.Hbl_Feight;
                            item.Is_XC = H_Order.Is_XC;
                            item.Is_BAS = H_Order.Is_BAS;
                            item.Is_DCZ = H_Order.Is_DCZ;
                            item.Is_ZB = H_Order.Is_ZB;
                            item.ADDPoint = H_Order.ADDPoint;
                            item.EDITPoint = H_Order.EDITPoint;
                            item.Status = H_Order.Status;
                            item.Batch_Num = H_Order.Batch_Num;
                            item.OperatingPoint = H_Order.OperatingPoint;
                            item.ObjectState = ObjectState.Modified;
                            _oPS_H_OrderService.Update(item);

                            #endregion
                        }
                    }
                    _oPS_M_OrderService.Insert(OPS_M_Order);

                    #endregion
                }
                else
                {
                    #region 接单修改

                    var morder = _oPS_M_OrderService.Find(OPS_M_Order.Id);
                    if (morder != null)
                    {
                        #region 主单信息保存

                        m_operation = ChangeOrderHistory.EnumChangeType.Modify;
                        //ContentM = Common.DifferenceComparison<OPS_M_Order>(morder, OPS_M_Order, excludeM);
                        ContentM = _oPS_M_OrderService.DifferenceComparisonM(morder, OPS_M_Order);

                        morder.MBL = Common.RemoveNotNumber(OPS_M_Order.MBL);
                        morder.Airways_Code = OPS_M_Order.Airways_Code;
                        morder.FWD_Code = OPS_M_Order.FWD_Code;
                        morder.Shipper_M = OPS_M_Order.Shipper_M;
                        morder.Consignee_M = OPS_M_Order.Consignee_M;
                        morder.Notify_Part_M = OPS_M_Order.Notify_Part_M;
                        morder.Depart_Port = OPS_M_Order.Depart_Port;
                        morder.End_Port = OPS_M_Order.End_Port;
                        morder.Flight_No = OPS_M_Order.Flight_No;
                        morder.Flight_Date_Want = OPS_M_Order.Flight_Date_Want;
                        morder.Currency_M = OPS_M_Order.Currency_M;
                        morder.Bragainon_Article_M = OPS_M_Order.Bragainon_Article_M;
                        morder.Pay_Mode_M = OPS_M_Order.Pay_Mode_M;
                        morder.Carriage_M = OPS_M_Order.Carriage_M;
                        morder.Incidental_Expenses_M = OPS_M_Order.Incidental_Expenses_M;
                        morder.Declare_Value_Trans_M = OPS_M_Order.Declare_Value_Trans_M;
                        morder.Declare_Value_Ciq_M = OPS_M_Order.Declare_Value_Ciq_M;
                        morder.Risk_M = OPS_M_Order.Risk_M;
                        morder.Marks_M = OPS_M_Order.Marks_M;
                        morder.EN_Name_M = OPS_M_Order.EN_Name_M;
                        morder.Hand_Info_M = OPS_M_Order.Hand_Info_M;
                        morder.Signature_Agent_M = OPS_M_Order.Signature_Agent_M;
                        morder.Rate_Class_M = OPS_M_Order.Rate_Class_M;
                        morder.Air_Frae = OPS_M_Order.Air_Frae;
                        morder.AWC = OPS_M_Order.AWC;
                        morder.Pieces_M = OPS_M_Order.Pieces_M;
                        morder.Weight_M = OPS_M_Order.Weight_M;
                        morder.Volume_M = OPS_M_Order.Volume_M;
                        morder.Charge_Weight_M = OPS_M_Order.Charge_Weight_M;
                        morder.Price_Article = OPS_M_Order.Price_Article;
                        morder.CCC = OPS_M_Order.CCC;
                        morder.File_M = OPS_M_Order.File_M;
                        morder.Status = OPS_M_Order.Status;
                        morder.OperatingPoint = OPS_M_Order.OperatingPoint;
                        morder.ObjectState = ObjectState.Modified;
                        _oPS_M_OrderService.Update(morder);

                        #endregion

                        #region 委托信息保存

                        foreach (var EntrustmentInfor in OPS_M_Order.OPS_EntrustmentInfors)
                        {
                            e_operation = ChangeOrderHistory.EnumChangeType.Modify;
                            ContentE = EntrustmentInfor.Operation_Id;

                            EntrustmentInfor.MBL = Common.RemoveNotNumber(EntrustmentInfor.MBL);
                            if (EntrustmentInfor.Id <= 0)
                            {
                                //修改主单时  新增一条新的分单信息
                                //EntrustmentInfor.MBLId = OPS_M_Order.Id;
                                //_oPS_EntrustmentInforService.Insert(EntrustmentInfor);
                                continue;
                            }
                            var item = _oPS_EntrustmentInforService.Find(EntrustmentInfor.Id);
                            //ContentE = Common.DifferenceComparison<OPS_EntrustmentInfor>(item, EntrustmentInfor, excludeE);
                            ContentE = _oPS_EntrustmentInforService.DifferenceComparisonE(item, EntrustmentInfor);

                            OOPS_EntrustmentInfor = item;
                            item.MBLId = OPS_M_Order.Id;
                            item.Consign_Code = EntrustmentInfor.Consign_Code;
                            item.Custom_Code = EntrustmentInfor.Custom_Code;
                            item.Area_Code = EntrustmentInfor.Area_Code;
                            Operation_Id = item.Operation_Id = EntrustmentInfor.Operation_Id;
                            item.Entrustment_Name = EntrustmentInfor.Entrustment_Name;
                            item.Entrustment_Code = EntrustmentInfor.Entrustment_Code;
                            item.FWD_Code = EntrustmentInfor.FWD_Code;
                            item.Consignee_Code = EntrustmentInfor.Consignee_Code;
                            item.Carriage_Account_Code = EntrustmentInfor.Carriage_Account_Code;
                            item.Incidental_Account_Code = EntrustmentInfor.Incidental_Account_Code;
                            item.Depart_Port = EntrustmentInfor.Depart_Port;
                            item.Transfer_Port = EntrustmentInfor.Transfer_Port;
                            item.End_Port = EntrustmentInfor.End_Port;

                            #region 保存主单(国外代理)/分单(委托人) 发货人，收货人和通知人 委托人：Entrustment_Name-国外代理：FWD_Code

                            //主单发货人
                            _contactsService.InsertContact(EntrustmentInfor.Shipper_M, "shipper_m", EntrustmentInfor.FWD_Code);
                            //主单收货人
                            _contactsService.InsertContact(EntrustmentInfor.Consignee_M, "consignee_m", EntrustmentInfor.FWD_Code);
                            //主单通知人
                            _contactsService.InsertContact(EntrustmentInfor.Notify_Part_M, "notify_part_m", EntrustmentInfor.FWD_Code);

                            //分单发货人
                            _contactsService.InsertContact(EntrustmentInfor.Shipper_H, "shipper_h", EntrustmentInfor.Entrustment_Name);
                            //分单收货人
                            _contactsService.InsertContact(EntrustmentInfor.Consignee_H, "consignee_h", EntrustmentInfor.Entrustment_Name);
                            //分单通知人
                            _contactsService.InsertContact(EntrustmentInfor.Notify_Part_H, "notify_part_h", EntrustmentInfor.Entrustment_Name);

                            #endregion

                            #region 换行符转换

                            //if (EntrustmentInfor.Shipper_H != null)
                            //{
                            //    EntrustmentInfor.Shipper_H = Common.GetContactsString(EntrustmentInfor.Shipper_H);
                            //}
                            //if (EntrustmentInfor.Consignee_H != null)
                            //{
                            //    EntrustmentInfor.Consignee_H = Common.GetContactsString(EntrustmentInfor.Consignee_H);
                            //}
                            //if (EntrustmentInfor.Notify_Part_H != null)
                            //{
                            //    EntrustmentInfor.Notify_Part_H = Common.GetContactsString(EntrustmentInfor.Notify_Part_H);
                            //}
                            //if (EntrustmentInfor.Shipper_M != null)
                            //{
                            //    EntrustmentInfor.Shipper_M = Common.GetContactsString(EntrustmentInfor.Shipper_M);
                            //}
                            //if (EntrustmentInfor.Consignee_M != null)
                            //{
                            //    EntrustmentInfor.Consignee_M = Common.GetContactsString(EntrustmentInfor.Consignee_M);
                            //}
                            //if (EntrustmentInfor.Notify_Part_M != null)
                            //{
                            //    EntrustmentInfor.Notify_Part_M = Common.GetContactsString(EntrustmentInfor.Notify_Part_M);
                            //}

                            #endregion

                            //主/分单 发货人 收货人 通知人
                            item.Shipper_H = EntrustmentInfor.Shipper_H;
                            item.Consignee_H = EntrustmentInfor.Consignee_H;
                            item.Notify_Part_H = EntrustmentInfor.Notify_Part_H;
                            item.Shipper_M = EntrustmentInfor.Shipper_M;
                            item.Consignee_M = EntrustmentInfor.Consignee_M;
                            item.Notify_Part_M = EntrustmentInfor.Notify_Part_M;

                            item.Pieces_TS = EntrustmentInfor.Pieces_TS;
                            item.Weight_TS = EntrustmentInfor.Weight_TS;
                            item.Pieces_SK = EntrustmentInfor.Pieces_SK;
                            item.Slac_SK = EntrustmentInfor.Slac_SK;
                            item.Weight_SK = EntrustmentInfor.Weight_SK;
                            item.Pieces_DC = EntrustmentInfor.Pieces_DC;
                            item.Slac_DC = EntrustmentInfor.Slac_DC;
                            item.Weight_DC = EntrustmentInfor.Weight_DC;
                            item.IS_MoorLevel = EntrustmentInfor.IS_MoorLevel;
                            item.MoorLevel = EntrustmentInfor.MoorLevel;
                            item.Volume_TS = EntrustmentInfor.Volume_TS;
                            item.Charge_Weight_TS = EntrustmentInfor.Charge_Weight_TS;
                            item.Bulk_Weight_TS = EntrustmentInfor.Bulk_Weight_TS;
                            item.Volume_SK = EntrustmentInfor.Volume_SK;
                            item.Charge_Weight_SK = EntrustmentInfor.Charge_Weight_SK;
                            item.Bulk_Weight_SK = EntrustmentInfor.Bulk_Weight_SK;
                            item.Bulk_Percent_SK = EntrustmentInfor.Bulk_Percent_SK;
                            item.Account_Weight_SK = EntrustmentInfor.Account_Weight_SK;
                            item.Volume_DC = EntrustmentInfor.Volume_DC;
                            item.Charge_Weight_DC = EntrustmentInfor.Charge_Weight_DC;
                            item.Bulk_Weight_DC = EntrustmentInfor.Bulk_Weight_DC;
                            item.Bulk_Percent_DC = EntrustmentInfor.Bulk_Percent_DC;
                            item.Account_Weight_DC = EntrustmentInfor.Account_Weight_DC;
                            //item.Pieces_Fact = EntrustmentInfor.Pieces_Fact;
                            //item.Weight_Fact = EntrustmentInfor.Weight_Fact;
                            //item.Volume_Fact = EntrustmentInfor.Volume_Fact;
                            //item.Charge_Weight_Fact = EntrustmentInfor.Charge_Weight_Fact;
                            //item.Bulk_Weight_Fact = EntrustmentInfor.Bulk_Weight_Fact;
                            //item.Bulk_Percent_Fact = EntrustmentInfor.Bulk_Percent_Fact;
                            //item.Account_Weight_Fact = EntrustmentInfor.Account_Weight_Fact;
                            item.Marks_H = EntrustmentInfor.Marks_H;
                            item.EN_Name_H = EntrustmentInfor.EN_Name_H;
                            item.Is_Book_Flat = EntrustmentInfor.Is_Book_Flat;
                            item.Book_Flat_Code = EntrustmentInfor.Book_Flat_Code;
                            item.Airways_Code = EntrustmentInfor.Airways_Code;
                            item.Flight_No = EntrustmentInfor.Flight_No;
                            item.MBL = Common.RemoveNotNumber(EntrustmentInfor.MBL);

                            is_self = EntrustmentInfor.Is_Self;
                            if (EntrustmentInfor.Is_Self && string.IsNullOrEmpty(EntrustmentInfor.HBL))
                            {
                                item.HBL = Operation_Id;
                            }
                            else
                            {
                                item.HBL = EntrustmentInfor.HBL;
                            }
                            item.Flight_Date_Want = EntrustmentInfor.Flight_Date_Want;
                            item.Book_Remark = EntrustmentInfor.Book_Remark;
                            item.Delivery_Point = EntrustmentInfor.Delivery_Point;
                            item.Warehouse_Code = EntrustmentInfor.Warehouse_Code;
                            item.RK_Date = EntrustmentInfor.RK_Date;
                            item.CK_Date = EntrustmentInfor.CK_Date;
                            item.CH_Name = EntrustmentInfor.CH_Name;
                            item.AMS = EntrustmentInfor.AMS;
                            //item.Lot_No = EntrustmentInfor.Lot_No;
                            item.Is_Self = EntrustmentInfor.Is_Self;
                            item.Ty_Type = EntrustmentInfor.Ty_Type;
                            item.Hbl_Feight = EntrustmentInfor.Hbl_Feight;
                            item.Is_XC = EntrustmentInfor.Is_XC;
                            item.Is_BAS = EntrustmentInfor.Is_BAS;
                            item.Is_DCZ = EntrustmentInfor.Is_DCZ;
                            item.Is_ZB = EntrustmentInfor.Is_ZB;
                            item.Is_TG = EntrustmentInfor.Is_TG;
                            item.ADDPoint = EntrustmentInfor.ADDPoint;
                            item.EDITPoint = EntrustmentInfor.EDITPoint;
                            item.Batch_Num = EntrustmentInfor.Batch_Num;
                            item.Status = EntrustmentInfor.Status;
                            item.Remark = EntrustmentInfor.Remark;
                            item.OperatingPoint = EntrustmentInfor.OperatingPoint;
                            item.Is_HDQ = EntrustmentInfor.Is_HDQ;

                            //2019-03-15 新增
                            item.SallerId = EntrustmentInfor.SallerId;
                            item.SallerName = EntrustmentInfor.SallerName;

                            item.ObjectState = ObjectState.Modified;

                            ErrStr += saveWarehousReceipts(item, EntrustmentInfor, OPS_M_Order.Id, Is_BindingWarehouse);
                            item.Marks_H = "";

                            _oPS_EntrustmentInforService.Update(item);

                        }
                        #endregion

                        #region 分单信息保存

                        foreach (var H_Order in OPS_M_Order.OPS_H_Orders)
                        {
                            if (H_Order.Id <= 0)
                            {
                                H_Order.ObjectState = ObjectState.Added;
                                //h_operation_add_num = h_operation_add_num + 1;
                                //h_operation_add = ChangeOrderHistory.EnumChangeType.Insert;
                                ContentH_add = H_Order.HBL;

                                H_Order.MBL = Common.RemoveNotNumber(H_Order.MBL);
                                //修改主单时，新增 一条分单信息
                                H_Order.MBLId = OPS_M_Order.Id;
                                _oPS_H_OrderService.Insert(H_Order);
                                continue;
                            }
                            H_Order.ObjectState = ObjectState.Modified;
                            //h_operation_edit_num = h_operation_edit_num + 1;
                            //h_operation_edit = ChangeOrderHistory.EnumChangeType.Modify;
                            ContentH_edit = H_Order.HBL;

                            var item = _oPS_H_OrderService.Find(H_Order.Id);
                            item.MBLId = OPS_M_Order.Id;
                            item.MBL = Common.RemoveNotNumber(H_Order.MBL);

                            #region 保存分单 发货人 收货人 通知人

                            ////分单发货人
                            //_contactsService.InsertContact(H_Order.Shipper_H, "shipper_h", OPS_M_Order.FWD_Code);
                            ////分单收货人
                            //_contactsService.InsertContact(H_Order.Consignee_H, "consignee_h", OPS_M_Order.FWD_Code);
                            ////分单通知人
                            //_contactsService.InsertContact(H_Order.Notify_Part_H, "notify_part_h", OPS_M_Order.FWD_Code);

                            #endregion

                            #region 换行符转换

                            //if (H_Order.Shipper_H != null)
                            //{
                            //    H_Order.Shipper_H = Common.GetContactsString(H_Order.Shipper_H);
                            //}
                            //if (H_Order.Consignee_H != null)
                            //{
                            //    H_Order.Consignee_H = Common.GetContactsString(H_Order.Consignee_H);
                            //}
                            //if (H_Order.Notify_Part_H != null)
                            //{
                            //    H_Order.Notify_Part_H = Common.GetContactsString(H_Order.Notify_Part_H);
                            //}

                            #endregion

                            item.Consignee_H = H_Order.Consignee_H;
                            item.Shipper_H = H_Order.Shipper_H;
                            item.Notify_Part_H = H_Order.Notify_Part_H;

                            item.Currency_H = H_Order.Currency_H;
                            item.Bragainon_Article_H = H_Order.Bragainon_Article_H;
                            item.Pay_Mode_H = H_Order.Pay_Mode_H;
                            item.Carriage_H = H_Order.Carriage_H;
                            item.Incidental_Expenses_H = H_Order.Incidental_Expenses_H;
                            item.Declare_Value_Trans_H = H_Order.Declare_Value_Trans_H;
                            item.Declare_Value_Ciq_H = H_Order.Declare_Value_Ciq_H;
                            item.Risk_H = H_Order.Risk_H;
                            item.Marks_H = H_Order.Marks_H;
                            item.EN_Name_H = H_Order.EN_Name_H;
                            item.Pieces_H = H_Order.Pieces_H;
                            item.Weight_H = H_Order.Weight_H;
                            item.Volume_H = H_Order.Volume_H;
                            item.Charge_Weight_H = H_Order.Charge_Weight_H;
                            if (is_self && string.IsNullOrEmpty(H_Order.HBL))
                            {
                                item.HBL = H_Order.Operation_Id;
                            }
                            else
                            {
                                item.HBL = H_Order.HBL;
                            }
                            item.Operation_Id = H_Order.Operation_Id;
                            item.Is_Self = H_Order.Is_Self;
                            item.Ty_Type = H_Order.Ty_Type;
                            item.Lot_No = H_Order.Lot_No;
                            item.Hbl_Feight = H_Order.Hbl_Feight;
                            item.Is_XC = H_Order.Is_XC;
                            item.Is_BAS = H_Order.Is_BAS;
                            item.Is_DCZ = H_Order.Is_DCZ;
                            item.Is_ZB = H_Order.Is_ZB;
                            item.ADDPoint = H_Order.ADDPoint;
                            item.EDITPoint = H_Order.EDITPoint;
                            item.Status = H_Order.Status;
                            item.Batch_Num = H_Order.Batch_Num;
                            item.OperatingPoint = H_Order.OperatingPoint;
                            item.ObjectState = ObjectState.Modified;
                            _oPS_H_OrderService.Update(item);
                        }

                        #endregion
                    }

                    #endregion
                }

                #region 解除委托信息和总单的绑定

                if (DeleteWTXX != "")
                {
                    var _wtID = DeleteWTXX.Split(",");
                    foreach (var id in _wtID)
                    {
                        if (id != null && id != "")
                        {
                            var item = _oPS_EntrustmentInforService.Find(Int32.Parse(id));
                            if (item != null)
                            {
                                item.MBLId = null;
                                item.MBL = "";
                                item.ObjectState = ObjectState.Modified;
                                _oPS_EntrustmentInforService.Update(item);
                            }
                        }
                    }
                }

                #endregion

                #region 解除分单信息和总单的绑定

                if (DeleteFDXX != "")
                {
                    var _fdID = DeleteFDXX.Split(",");
                    foreach (var id in _fdID)
                    {
                        if (id != null && id != "")
                        {
                            var item = _oPS_H_OrderService.Find(Int32.Parse(id));
                            if (item != null)
                            {
                                item.MBL = "";
                                item.MBLId = null;
                                item.ObjectState = ObjectState.Modified;
                                _oPS_H_OrderService.Update(item);
                            }
                        }
                    }
                }

                #endregion

                var ErrMsg = UpdateEttInfoByMBL(ref OOPS_EntrustmentInfor);

                OPS_M_Order.End_Port = OOPS_EntrustmentInfor.End_Port;//目的港
                OPS_M_Order.Flight_No = OOPS_EntrustmentInfor.Flight_No;//航班号
                OPS_M_Order.FWD_Code = OOPS_EntrustmentInfor.FWD_Code;//国外代理
                OPS_M_Order.Airways_Code = OOPS_EntrustmentInfor.Airways_Code;//航空公司
                OPS_M_Order.Flight_Date_Want = OOPS_EntrustmentInfor.Flight_Date_Want;//航班日期

                //foreach (var Hitem in OPS_M_Order.OPS_H_Orders)
                //{
                //    Hitem.End_Port = OOPS_EntrustmentInfor.End_Port;//目的港
                //    Hitem.Flight_No = OOPS_EntrustmentInfor.Flight_No;//航班号
                //    Hitem.FWD_Code = OOPS_EntrustmentInfor.FWD_Code;//国外代理
                //    Hitem.Airways_Code = OOPS_EntrustmentInfor.Airways_Code;//航空公司
                //}

                if (!string.IsNullOrEmpty(ErrMsg))
                    return Json(new { success = false, err = "错误：" + ErrMsg, Operation_Id = Operation_Id }, JsonRequestBehavior.AllowGet);

                _unitOfWork.SaveChanges();

                if (!string.IsNullOrWhiteSpace(OPS_M_Order.MBL))
                {
                    string errbms = _bms_Bill_ApService.InsertFT_EntrustmentInfor(OPS_M_Order.MBL);
                    if (!string.IsNullOrEmpty(errbms))
                    {
                        ErrStr = errbms + "\r\n";
                        return Json(new { success = true, err = ErrStr, Operation_Id = Operation_Id, Id = OPS_M_Order.Id }, JsonRequestBehavior.AllowGet);
                    }
                }

                #region 添加日志信息

                if (m_operation == ChangeOrderHistory.EnumChangeType.Insert)
                {
                    _changeOrderHistoryService.InsertChangeOrdHistory(OPS_M_Order.Id, "OPS_M_Order", m_operation, "承揽操作 " + ContentM, 0, 0, 1);
                }
                else if (m_operation == ChangeOrderHistory.EnumChangeType.Modify)
                {
                    _changeOrderHistoryService.InsertChangeOrdHistory(OPS_M_Order.Id, "OPS_M_Order", m_operation, "承揽操作 " + ContentM, 0, 1, 0);
                }
                if (e_operation == ChangeOrderHistory.EnumChangeType.Insert)
                {
                    foreach (var item in OPS_M_Order.OPS_EntrustmentInfors)
                    {
                        _changeOrderHistoryService.InsertChangeOrdHistory(item.Id, "OPS_EntrustmentInfor", e_operation, "承揽操作 " + ContentE, 0, 0, 1);
                    }
                }
                else if (e_operation == ChangeOrderHistory.EnumChangeType.Modify)
                {
                    foreach (var item in OPS_M_Order.OPS_EntrustmentInfors)
                    {
                        _changeOrderHistoryService.InsertChangeOrdHistory(item.Id, "OPS_EntrustmentInfor", e_operation, "承揽操作 " + ContentE, 0, 1, 0);
                    }
                }
                if (OPS_M_Order != null || OPS_M_Order.OPS_H_Orders != null)
                {
                    //foreach (var item in OPS_M_Order.OPS_H_Orders)
                    //{
                    //    if (item.ObjectState == ObjectState.Added)
                    //    {
                    //        _changeOrderHistoryService.InsertChangeOrdHistory(item.Id, "OPS_H_Order", h_operation_add, "承揽操作 " + ContentH_add, 0, 0, 1);
                    //    }
                    //    else if (item.ObjectState == ObjectState.Modified)
                    //    {
                    //        _changeOrderHistoryService.InsertChangeOrdHistory(OPS_M_Order.Id, "OPS_H_Order", h_operation_edit, "承揽操作 " + ContentH_edit, 0, 1, 0);
                    //    }
                    //}
                }
                //if (h_operation_add == ChangeOrderHistory.EnumChangeType.Insert)
                //{
                //    _changeOrderHistoryService.InsertChangeOrdHistory(OPS_M_Order.Id, "OPS_H_Order", h_operation_add, ContentH_add, 0, 0, h_operation_add_num);
                //}
                //if (h_operation_edit == ChangeOrderHistory.EnumChangeType.Modify)
                //{
                //    _changeOrderHistoryService.InsertChangeOrdHistory(OPS_M_Order.Id, "OPS_H_Order", h_operation_edit, ContentH_edit, 0, h_operation_edit_num, 0);
                //}

                #endregion

                #region 自动结算-有总单号时才产生费用

                if (!string.IsNullOrWhiteSpace(OPS_M_Order.MBL))
                {
                    var ApRetMsg = _costMoneyService.AutoAddFee(OPS_M_Order);
                    var ArRetMsg = _customerQuotedPriceService.AutoAddFee(OPS_M_Order);
                    if (!string.IsNullOrWhiteSpace(ApRetMsg) || !string.IsNullOrWhiteSpace(ArRetMsg))
                    {
                        string Err_Msg = "";
                        if (!string.IsNullOrWhiteSpace(ApRetMsg))
                            Err_Msg += " 应付错误：" + ApRetMsg;
                        if (!string.IsNullOrWhiteSpace(ArRetMsg))
                            Err_Msg += " 应收错误：" + ArRetMsg;
                    }

                    #region 更新 应收/应付 账单总单号

                    List<string> ArrUpdateMblSQLStr = new List<string>(){
                        "update Bms_Bill_Ars t set t.mbl=:V_MBL where t.ops_m_ordid in (select o.MBLId from ops_entrustmentinfors o where o.MBL=:V_MBL)",
                        "update Bms_Bill_Aps t set t.mbl=:V_MBL where t.ops_m_ordid in (select o.MBLId from ops_entrustmentinfors o where o.MBL=:V_MBL)"
                    };
                    SQLDALHelper.OracleHelper.ExecuteSqlTran(ArrUpdateMblSQLStr, new Oracle.ManagedDataAccess.Client.OracleParameter(":V_MBL", OPS_M_Order.MBL));

                    #endregion
                }

                #endregion

                return Json(new { success = true, err = ErrStr, Operation_Id = Operation_Id, Id = OPS_M_Order.Id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { success = false, err = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
        }

        //通过 进仓实际件数、进仓实际毛重、进仓实际体积，判断绑定或是解绑  仓库接单
        private string saveWarehousReceipts(OPS_EntrustmentInfor EntrustmentInfor, OPS_EntrustmentInfor newEntrustmentInfor, int mblid, string Is_BindingWarehouse)
        {
            #region 通过 进仓实际件数、进仓实际毛重、进仓实际体积，判断绑定或是解绑  仓库接单
            var operation = "";
            if (EntrustmentInfor != null)
            {
                operation = EntrustmentInfor.Operation_Id;
            }
            string ErrStr = "";
            var warehouse_receipt = _warehouse_receiptService.Queryable().Where(x => x.MBLId == mblid).ToList();
            if (Is_BindingWarehouse == "3")
            {
                //取消绑定进仓信息
                //var warehouse_receipt = _warehouse_receiptService.Queryable().Where(x => x.MBLId == mblid).ToList();
                if (warehouse_receipt != null && warehouse_receipt.Count() > 0)
                {
                    foreach (var warehouse in warehouse_receipt)
                    {
                        //var warehouse = warehouse_receipt.FirstOrDefault();
                        warehouse.MBL = "";
                        warehouse.HBL = "";
                        warehouse.MBLId = null;
                        warehouse.Is_Binding = false;
                        warehouse.Consign_Code_CK = "";
                        warehouse.End_Port = "";
                        warehouse.Flight_Date_Want = null;
                        warehouse.FLIGHT_No = "";
                        _warehouse_receiptService.Update(warehouse);
                        _changeOrderHistoryService.InsertChangeOrdHistory(warehouse.Id, "Warehouse_receipt", ChangeOrderHistory.EnumChangeType.Modify, "承揽操作 " + "取消绑定 " + operation, 0, 1, 0);

                        EntrustmentInfor.Pieces_Fact = 0;
                        EntrustmentInfor.Weight_Fact = 0;
                        EntrustmentInfor.Volume_Fact = 0;
                        EntrustmentInfor.Charge_Weight_Fact = 0;
                        EntrustmentInfor.Bulk_Weight_Fact = 0;
                        //EntrustmentInfor.Bulk_Percent_Fact = 0;
                        EntrustmentInfor.Account_Weight_Fact = 0;
                    }
                }
            }
            else
            {
                #region 绑定进仓信息

                if (EntrustmentInfor.Marks_H.IsNotNullOrEmpty())
                {
                    var is_binding = false;
                    //借用拼箱码，暂时存储仓库接单的id号
                    var arrid = EntrustmentInfor.Marks_H.Split(",");//绑定按钮事件
                    foreach (var id in arrid)
                    {
                        int wid = id.ToInt32();
                        var list = _warehouse_receiptService.Queryable().Where(x => x.Id == wid).ToList();
                        if (list != null && list.Count > 0)
                        {
                            var warehouse = list.FirstOrDefault();
                            if (warehouse.Is_Binding == true)
                            {
                                ErrStr = "仓库接单" + warehouse.Warehouse_Id + "已经被绑定  \r\n";
                            }
                            else
                            {
                                is_binding = true;
                                warehouse.MBLId = mblid;
                                warehouse.MBL = Common.RemoveNotNumber(EntrustmentInfor.MBL);
                                warehouse.HBL = EntrustmentInfor.Operation_Id;//仓库接单因需求修改，由分单号修改为业务编号绑定
                                warehouse.Is_Binding = true;
                                warehouse.Consign_Code_CK = EntrustmentInfor.Entrustment_Name;
                                warehouse.End_Port = EntrustmentInfor.End_Port;
                                warehouse.Flight_Date_Want = EntrustmentInfor.Flight_Date_Want;
                                warehouse.FLIGHT_No = EntrustmentInfor.Flight_No;
                                _warehouse_receiptService.Update(warehouse);

                                _changeOrderHistoryService.InsertChangeOrdHistory(warehouse.Id, "Warehouse_receipt", ChangeOrderHistory.EnumChangeType.Modify, "承揽操作 " + "绑定 " + operation, 0, 1, 0);

                            }
                        }
                    }
                    if (Is_BindingWarehouse == "2")
                    {
                        //取消绑定按钮事件
                        //var warehouse_receipt = _warehouse_receiptService.Queryable().Where(x => x.MBLId == mblid).ToList();
                        if (warehouse_receipt != null && warehouse_receipt.Count() > 0)
                        {
                            var cencelbindinglist = warehouse_receipt.Where(x => !(arrid.Contains(x.Id.ToString()))).ToList();
                            if (cencelbindinglist != null)
                            {
                                foreach (var warehouse in cencelbindinglist)
                                {
                                    warehouse.MBL = "";
                                    warehouse.HBL = "";
                                    warehouse.MBLId = null;
                                    warehouse.Is_Binding = false;
                                    warehouse.Consign_Code_CK = "";
                                    warehouse.End_Port = "";
                                    warehouse.Flight_Date_Want = null;
                                    warehouse.FLIGHT_No = "";
                                    _warehouse_receiptService.Update(warehouse);
                                    _changeOrderHistoryService.InsertChangeOrdHistory(warehouse.Id, "Warehouse_receipt", ChangeOrderHistory.EnumChangeType.Modify, "承揽操作 " + "取消绑定 " + operation, 0, 1, 0);

                                    EntrustmentInfor.Pieces_Fact = newEntrustmentInfor.Pieces_Fact;
                                    EntrustmentInfor.Weight_Fact = newEntrustmentInfor.Weight_Fact;
                                    EntrustmentInfor.Volume_Fact = newEntrustmentInfor.Volume_Fact;
                                    EntrustmentInfor.Charge_Weight_Fact = newEntrustmentInfor.Charge_Weight_Fact;
                                    EntrustmentInfor.Bulk_Weight_Fact = newEntrustmentInfor.Bulk_Weight_Fact;
                                    EntrustmentInfor.Bulk_Percent_Fact = newEntrustmentInfor.Bulk_Percent_Fact;
                                    EntrustmentInfor.Account_Weight_Fact = newEntrustmentInfor.Account_Weight_Fact;
                                }
                            }
                        }
                    }
                    else
                    {

                        //当前页面，如果对绑定没有任何操作，在保存承揽接单时，
                        //根据仓库接单中的是否绑定栏位和承揽接单的实际件数、实际毛重、实际体积共同判断，是否清空承揽接单中的实际数据
                        if ((warehouse_receipt == null || !warehouse_receipt.Any()) && is_binding == false)
                        {
                            if (EntrustmentInfor.Pieces_Fact > 0 || EntrustmentInfor.Weight_Fact > 0 || EntrustmentInfor.Volume_Fact > 0)
                            {
                                EntrustmentInfor.Pieces_Fact = 0;
                                EntrustmentInfor.Weight_Fact = 0;
                                EntrustmentInfor.Volume_Fact = 0;
                                EntrustmentInfor.Charge_Weight_Fact = 0;
                                EntrustmentInfor.Bulk_Weight_Fact = 0;
                                //EntrustmentInfor.Bulk_Percent_Fact = 0;
                                EntrustmentInfor.Account_Weight_Fact = 0;
                            }
                        }
                        else
                        {
                            if (newEntrustmentInfor.Pieces_Fact > 0 || newEntrustmentInfor.Weight_Fact > 0 || newEntrustmentInfor.Volume_Fact > 0)
                            {
                                EntrustmentInfor.Pieces_Fact = newEntrustmentInfor.Pieces_Fact;
                                EntrustmentInfor.Weight_Fact = newEntrustmentInfor.Weight_Fact;
                                EntrustmentInfor.Volume_Fact = newEntrustmentInfor.Volume_Fact;
                                EntrustmentInfor.Charge_Weight_Fact = newEntrustmentInfor.Charge_Weight_Fact;
                                EntrustmentInfor.Bulk_Weight_Fact = newEntrustmentInfor.Bulk_Weight_Fact;
                                EntrustmentInfor.Bulk_Percent_Fact = newEntrustmentInfor.Bulk_Percent_Fact;
                                EntrustmentInfor.Account_Weight_Fact = newEntrustmentInfor.Account_Weight_Fact;
                            }
                        }
                    }
                }

                #endregion
            }

            return ErrStr;

            #endregion
        }

        /// <summary>
        /// 航班日期前后15天内且总单号相同的业务
        /// 委托信息中的：订舱主单的件、毛、体行信息同步（含靠级标志等）
        /// </summary>
        /// <param name="EntrustmentInfor">当前编辑的委托</param>
        /// <param name="DateNum">前后天数（默认15天）</param>
        private string UpdateEttInfoByMBL(ref OPS_EntrustmentInfor OEntrustmentInfor, int DateNum = 15)
        {
            try
            {
                if (OEntrustmentInfor == null || string.IsNullOrEmpty(OEntrustmentInfor.MBL) || OEntrustmentInfor.Flight_Date_Want == null)
                    return "";
                if (DateNum <= 0)
                    DateNum = 15;
                var formatStr = "yyyy/MM/dd";
                var NowDate = OEntrustmentInfor.Flight_Date_Want;// Common.ParseStrToDateTime(DateTime.Now.ToString(formatStr), formatStr);
                if (NowDate == null)
                    NowDate = Common.ParseStrToDateTime(DateTime.Now.ToString(formatStr), formatStr);
                else
                    NowDate = Common.ParseStrToDateTime(((DateTime)OEntrustmentInfor.Flight_Date_Want).ToString(formatStr), formatStr);
                var MinDate = ((DateTime)NowDate).AddDays(-DateNum);
                var MaxDate = ((DateTime)NowDate).AddDays(DateNum + 1);
                var MBL = OEntrustmentInfor.MBL;
                var Id = OEntrustmentInfor.Id;
                var ArrOPS_EttInfor = _oPS_EntrustmentInforService.Queryable().Where(x => x.Flight_Date_Want > MinDate && x.Flight_Date_Want <= MaxDate && x.MBL == MBL).Include(x => x.OPS_M_Order).ToList();
                if (ArrOPS_EttInfor.Any() && Id > 0)
                    ArrOPS_EttInfor = ArrOPS_EttInfor.Where(x => x.Id != Id).ToList();
                OPS_EntrustmentInfor MaxEttInfor = null;
                if (ArrOPS_EttInfor.Any())
                {
                    MaxEttInfor = ArrOPS_EttInfor.Where(x => x.Pieces_DC != null || x.Weight_DC != null || x.Volume_DC != null).OrderByDescending(x => x.Id).FirstOrDefault();
                }
                var QEnd_Port = ArrOPS_EttInfor.Where(x => !string.IsNullOrEmpty(x.End_Port)).FirstOrDefault();
                string End_Port = QEnd_Port == null ? "" : QEnd_Port.End_Port;//目的港
                var QBook_Flat_Code = ArrOPS_EttInfor.Where(x => !string.IsNullOrEmpty(x.Book_Flat_Code)).FirstOrDefault();
                string Book_Flat_Code = QBook_Flat_Code == null ? "" : QBook_Flat_Code.Book_Flat_Code;//订舱方
                var QAirways_Code = ArrOPS_EttInfor.Where(x => !string.IsNullOrEmpty(x.Airways_Code)).FirstOrDefault();
                string Airways_Code = QAirways_Code == null ? "" : QAirways_Code.Airways_Code;//航空公司
                var QFlight_Date_Want = ArrOPS_EttInfor.Where(x => x.Flight_Date_Want != null).FirstOrDefault();
                DateTime? Flight_Date_Want = QFlight_Date_Want == null ? null : QFlight_Date_Want.Flight_Date_Want;//航班日期
                var QFlight_No = ArrOPS_EttInfor.Where(x => !string.IsNullOrEmpty(x.Flight_No)).FirstOrDefault();
                string Flight_No = QFlight_No == null ? "" : QFlight_No.Flight_No;//航班号
                var QFWD_Code = ArrOPS_EttInfor.Where(x => !string.IsNullOrEmpty(x.FWD_Code)).FirstOrDefault();
                string FWD_Code = QFWD_Code == null ? "" : QFWD_Code.FWD_Code;//国外代理
                var QDelivery_Point = ArrOPS_EttInfor.Where(x => !string.IsNullOrEmpty(x.Delivery_Point)).FirstOrDefault();
                string Delivery_Point = QDelivery_Point == null ? "" : QDelivery_Point.Delivery_Point;//交货地点

                WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;

                if ((OEntrustmentInfor.Pieces_DC != null ||
                    OEntrustmentInfor.Weight_DC != null ||
                    OEntrustmentInfor.Volume_DC != null) ||
                    !string.IsNullOrEmpty(OEntrustmentInfor.End_Port) ||
                    !string.IsNullOrEmpty(OEntrustmentInfor.Book_Flat_Code) ||
                    !string.IsNullOrEmpty(OEntrustmentInfor.Airways_Code) ||
                    OEntrustmentInfor.Flight_Date_Want != null ||
                    !string.IsNullOrEmpty(OEntrustmentInfor.Flight_No) ||
                    !string.IsNullOrEmpty(OEntrustmentInfor.FWD_Code) ||
                    !string.IsNullOrEmpty(OEntrustmentInfor.Delivery_Point))
                {
                    var SetSelfPWVCBA = false;
                    foreach (var item in ArrOPS_EttInfor)
                    {
                        var entity = WebdbContxt.Entry<OPS_EntrustmentInfor>(item);
                        item.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                        entity.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        if (OEntrustmentInfor.Pieces_DC != null || OEntrustmentInfor.Weight_DC != null || OEntrustmentInfor.Volume_DC != null)
                        {
                            item.Pieces_DC = OEntrustmentInfor.Pieces_DC;//件数
                            entity.Property(x => x.Pieces_DC).IsModified = true;

                            item.Weight_DC = OEntrustmentInfor.Weight_DC;//毛重
                            entity.Property(x => x.Weight_DC).IsModified = true;

                            item.Volume_DC = OEntrustmentInfor.Volume_DC;//体积
                            entity.Property(x => x.Volume_DC).IsModified = true;

                            item.Slac_DC = OEntrustmentInfor.Slac_DC;//SLAC
                            entity.Property(x => x.Slac_DC).IsModified = true;

                            item.IS_MoorLevel = OEntrustmentInfor.IS_MoorLevel;//是否靠级
                            entity.Property(x => x.IS_MoorLevel).IsModified = true;

                            item.MoorLevel = OEntrustmentInfor.MoorLevel;//靠级
                            entity.Property(x => x.MoorLevel).IsModified = true;

                            item.Charge_Weight_DC = OEntrustmentInfor.Charge_Weight_DC;//计费重量
                            entity.Property(x => x.Charge_Weight_DC).IsModified = true;

                            item.Bulk_Weight_DC = OEntrustmentInfor.Bulk_Weight_DC;//泡重KG
                            entity.Property(x => x.Bulk_Weight_DC).IsModified = true;

                            item.Bulk_Percent_DC = OEntrustmentInfor.Bulk_Percent_DC;//分泡比%
                            entity.Property(x => x.Bulk_Percent_DC).IsModified = true;

                            item.Account_Weight_DC = OEntrustmentInfor.Account_Weight_DC;//结算重量
                            entity.Property(x => x.Account_Weight_DC).IsModified = true;
                        }
                        else
                        {
                            if (!SetSelfPWVCBA)
                            {
                                if (MaxEttInfor != null)
                                {
                                    OEntrustmentInfor.Pieces_DC = MaxEttInfor.Pieces_DC;//件数
                                    OEntrustmentInfor.Weight_DC = MaxEttInfor.Weight_DC;//毛重
                                    OEntrustmentInfor.Volume_DC = MaxEttInfor.Volume_DC;//体积
                                    OEntrustmentInfor.Slac_DC = MaxEttInfor.Slac_DC;//SLAC
                                    OEntrustmentInfor.IS_MoorLevel = MaxEttInfor.IS_MoorLevel;//是否靠级
                                    OEntrustmentInfor.MoorLevel = MaxEttInfor.MoorLevel;//靠级
                                    OEntrustmentInfor.Charge_Weight_DC = MaxEttInfor.Charge_Weight_DC;//计费重量
                                    OEntrustmentInfor.Bulk_Weight_DC = MaxEttInfor.Bulk_Weight_DC;//泡重KG
                                    OEntrustmentInfor.Bulk_Percent_DC = MaxEttInfor.Bulk_Percent_DC;//分泡比%
                                    OEntrustmentInfor.Account_Weight_DC = MaxEttInfor.Account_Weight_DC;//结算重量
                                }
                                SetSelfPWVCBA = true;
                            }
                        }
                        var OOPS_M_Order = item.OPS_M_Order;
                        System.Data.Entity.Infrastructure.DbEntityEntry<OPS_M_Order> entity_M = null;
                        if (OOPS_M_Order != null && OOPS_M_Order.Id > 0)
                        {
                            entity_M = WebdbContxt.Entry(OOPS_M_Order);
                            OOPS_M_Order.ObjectState = ObjectState.Modified;//必须设置位 Modified 与 entry.State 比对一致将不作修改（直接 修改entry.State 为Modified，将更新所有字段）
                            entity_M.State = EntityState.Unchanged;//必须设置位 Unchanged ，后面 设置entry.Property-IsModified 才会有效，entry.State 自动会变为Modified（只是部分）
                        }
                        if (!string.IsNullOrEmpty(OEntrustmentInfor.End_Port))
                        {
                            item.End_Port = OEntrustmentInfor.End_Port;//目的港
                            entity.Property(x => x.End_Port).IsModified = true;
                            if (OOPS_M_Order != null)
                            {
                                OOPS_M_Order.End_Port = OEntrustmentInfor.End_Port;//目的港
                                entity_M.Property(x => x.End_Port).IsModified = true;
                            }
                        }
                        else
                            OEntrustmentInfor.End_Port = End_Port;
                        if (!string.IsNullOrEmpty(OEntrustmentInfor.Book_Flat_Code))
                        {
                            item.Book_Flat_Code = OEntrustmentInfor.Book_Flat_Code;//订舱方
                            entity.Property(x => x.Book_Flat_Code).IsModified = true;
                        }
                        else
                            OEntrustmentInfor.Book_Flat_Code = Book_Flat_Code;
                        if (!string.IsNullOrEmpty(OEntrustmentInfor.Airways_Code))
                        {
                            item.Airways_Code = OEntrustmentInfor.Airways_Code;//航空公司
                            entity.Property(x => x.Airways_Code).IsModified = true;
                            if (OOPS_M_Order != null)
                            {
                                OOPS_M_Order.Airways_Code = OEntrustmentInfor.Airways_Code;//航空公司
                                entity_M.Property(x => x.Airways_Code).IsModified = true;
                            }
                        }
                        else
                            OEntrustmentInfor.Airways_Code = Airways_Code;
                        if (OEntrustmentInfor.Flight_Date_Want != null)
                        {
                            item.Flight_Date_Want = OEntrustmentInfor.Flight_Date_Want;//航班日期
                            entity.Property(x => x.Flight_Date_Want).IsModified = true;
                            if (OOPS_M_Order != null)
                            {
                                OOPS_M_Order.Flight_Date_Want = OEntrustmentInfor.Flight_Date_Want;//航班日期
                                entity_M.Property(x => x.Flight_Date_Want).IsModified = true;
                            }
                        }
                        else
                            OEntrustmentInfor.Flight_Date_Want = Flight_Date_Want;
                        if (!string.IsNullOrEmpty(OEntrustmentInfor.Flight_No))
                        {
                            item.Flight_No = OEntrustmentInfor.Flight_No;//航班号
                            entity.Property(x => x.Flight_No).IsModified = true;
                            if (OOPS_M_Order != null)
                            {
                                OOPS_M_Order.Flight_No = OEntrustmentInfor.Flight_No;//航班号
                                entity_M.Property(x => x.Flight_No).IsModified = true;
                            }
                        }
                        else
                            OEntrustmentInfor.Flight_No = Flight_No;
                        if (!string.IsNullOrEmpty(OEntrustmentInfor.FWD_Code))
                        {
                            item.FWD_Code = OEntrustmentInfor.FWD_Code;//国外代理
                            entity.Property(x => x.FWD_Code).IsModified = true;
                            if (OOPS_M_Order != null)
                            {
                                OOPS_M_Order.FWD_Code = OEntrustmentInfor.FWD_Code;//国外代理
                                entity_M.Property(x => x.FWD_Code).IsModified = true;
                            }
                        }
                        else
                            OEntrustmentInfor.FWD_Code = FWD_Code;

                        if (!string.IsNullOrEmpty(OEntrustmentInfor.Delivery_Point))
                        {
                            item.Delivery_Point = OEntrustmentInfor.Delivery_Point;//交货地点
                            entity.Property(x => x.Delivery_Point).IsModified = true;
                        }
                        else
                            OEntrustmentInfor.Delivery_Point = Delivery_Point;
                    }
                }
                else
                {
                    if (MaxEttInfor != null)
                    {
                        OEntrustmentInfor.Pieces_DC = MaxEttInfor.Pieces_DC;//件数
                        OEntrustmentInfor.Weight_DC = MaxEttInfor.Weight_DC;//毛重
                        OEntrustmentInfor.Volume_DC = MaxEttInfor.Volume_DC;//体积
                        OEntrustmentInfor.Slac_DC = MaxEttInfor.Slac_DC;//SLAC
                        OEntrustmentInfor.IS_MoorLevel = MaxEttInfor.IS_MoorLevel;//是否靠级
                        OEntrustmentInfor.MoorLevel = MaxEttInfor.MoorLevel;//靠级
                        OEntrustmentInfor.Charge_Weight_DC = MaxEttInfor.Charge_Weight_DC;//计费重量
                        OEntrustmentInfor.Bulk_Weight_DC = MaxEttInfor.Bulk_Weight_DC;//泡重KG
                        OEntrustmentInfor.Bulk_Percent_DC = MaxEttInfor.Bulk_Percent_DC;//分泡比%
                        OEntrustmentInfor.Account_Weight_DC = MaxEttInfor.Account_Weight_DC;//结算重量
                    }

                    if (string.IsNullOrEmpty(OEntrustmentInfor.FWD_Code))
                        OEntrustmentInfor.End_Port = End_Port;
                    if (string.IsNullOrEmpty(OEntrustmentInfor.Book_Flat_Code))
                        OEntrustmentInfor.Book_Flat_Code = Book_Flat_Code;
                    if (string.IsNullOrEmpty(OEntrustmentInfor.Airways_Code))
                        OEntrustmentInfor.Airways_Code = Airways_Code;
                    if (OEntrustmentInfor.Flight_Date_Want == null)
                        OEntrustmentInfor.Flight_Date_Want = Flight_Date_Want;
                    if (string.IsNullOrEmpty(OEntrustmentInfor.Flight_No))
                        OEntrustmentInfor.Flight_No = Flight_No;
                    if (string.IsNullOrEmpty(OEntrustmentInfor.FWD_Code))
                        OEntrustmentInfor.FWD_Code = FWD_Code;
                    if (string.IsNullOrEmpty(OEntrustmentInfor.Delivery_Point))
                        OEntrustmentInfor.Delivery_Point = Delivery_Point;
                }

                return "";
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return ErrMsg;
            }
        }

        //加载委托信息页签
        public ActionResult GetOPS_H_Order()
        {
            return PartialView("AddEntrustmentInforEditFore");
        }

        //加载分单信息页签
        public ActionResult GetOPS_EntrustmentInfor()
        {
            return PartialView("AddHOrderEditFore");
        }

        //批量修改保存
        [HttpPost]
        public ActionResult SaveBatchUpdate(Array ids, string filterRules)
        {
            try
            {
                var strids = "";
                foreach (var item in ids)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");

                var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
                foreach (var id in idarr)
                {
                    var ops_e = _oPS_EntrustmentInforService.Find(Int32.Parse(id));
                    if (ops_e == null)
                    {
                        return Json(new { success = false, err = "没有委托信息" }, JsonRequestBehavior.AllowGet);
                    }
                    var ops_m = _oPS_M_OrderService.Find(ops_e.MBLId);
                    if (ops_m == null)
                    {
                        return Json(new { success = false, err = "没有总单信息" }, JsonRequestBehavior.AllowGet);
                    }
                    var ops_h = _oPS_H_OrderService.Queryable().Where(x => x.MBLId == ops_e.MBLId);
                    if (ops_h == null)
                    {
                        return Json(new { success = false, err = "没有分单信息" }, JsonRequestBehavior.AllowGet);
                    }

                    CustomsInspection newcusinsp = new CustomsInspection();
                    if (filters != null)
                    {
                        var is_ops_m = false;//总单是否更新
                        var is_ops_e = false;//委托是否更新
                        var is_ops_h = false;//分单是否更新
                        var is_cusinsp = false; //报关信息
                        #region 更新总单、委托、分单的部分字段
                        foreach (var rule in filters)
                        {
                            if (rule.field == "Flight_No" && !string.IsNullOrEmpty(rule.value))
                            {
                                ops_e.Flight_No = rule.value;
                                ops_m.Flight_No = rule.value;
                                is_ops_m = true; is_ops_e = true;
                            }
                            if (rule.field == "Flight_Date_Want" && !string.IsNullOrEmpty(rule.value))
                            {
                                var date = Convert.ToDateTime(rule.value);
                                ops_e.Flight_Date_Want = date;
                                ops_m.Flight_Date_Want = date;
                                is_ops_m = true; is_ops_e = true;
                            }
                            if (rule.field == "MBL" && !string.IsNullOrEmpty(rule.value))
                            {
                                foreach (var item in ops_h)
                                {
                                    item.MBL = rule.value;
                                }
                                ops_e.MBL = rule.value;
                                ops_m.MBL = rule.value;
                                is_ops_m = true; is_ops_e = true; is_ops_h = true;
                            }
                            if (rule.field == "HBL" && !string.IsNullOrEmpty(rule.value))
                            {
                                if (ops_e.Is_Self)
                                {
                                    foreach (var item in ops_h)
                                    {
                                        item.HBL = rule.value;
                                        is_ops_h = true;
                                    }
                                    ops_e.HBL = rule.value;
                                    is_ops_e = true;
                                }
                            }
                            if (rule.field == "Depart_Port" && !string.IsNullOrEmpty(rule.value))
                            {
                                ops_m.Depart_Port = rule.value;
                                ops_e.Depart_Port = rule.value;
                                is_ops_m = true; is_ops_e = true;
                            }
                            if (rule.field == "End_Port" && !string.IsNullOrEmpty(rule.value))
                            {
                                ops_m.End_Port = rule.value;
                                ops_e.End_Port = rule.value;
                                is_ops_m = true; is_ops_e = true;
                            }
                            if (rule.field == "Book_Flat_Code" && !string.IsNullOrEmpty(rule.value))
                            {
                                ops_e.Book_Flat_Code = rule.value;
                                is_ops_e = true;
                            }
                            if (rule.field == "Airways_Code" && !string.IsNullOrEmpty(rule.value))
                            {
                                ops_e.Airways_Code = rule.value;
                                ops_m.Airways_Code = rule.value;
                                is_ops_m = true; is_ops_e = true;
                            }
                            if (rule.field == "Entrustment_Name" && !string.IsNullOrEmpty(rule.value))
                            {
                                ops_e.Entrustment_Name = rule.value;
                                ops_e.Consign_Code = rule.value;
                                ops_e.Carriage_Account_Code = rule.value;
                                is_ops_e = true;
                            }
                            if (rule.field == "Delivery_Point" && !string.IsNullOrEmpty(rule.value))
                            {
                                ops_e.Delivery_Point = rule.value;
                                is_ops_e = true;
                            }

                            if (rule.field == "Customs_Declaration" && !string.IsNullOrEmpty(rule.value))
                            {//报关方式
                                var cusInspDataRep = _unitOfWork.RepositoryAsync<CustomsInspection>().Queryable().Where(x => x.Operation_ID == ops_e.Operation_Id && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.draft).ToList();
                                if (cusInspDataRep != null && cusInspDataRep.Count > 0)
                                {
                                    var cusinspfirst = cusInspDataRep.FirstOrDefault();
                                    newcusinsp.Operation_ID = cusinspfirst.Operation_ID;
                                    newcusinsp.Flight_NO = cusinspfirst.Flight_NO;
                                    newcusinsp.MBL = cusinspfirst.MBL;
                                    newcusinsp.Consign_Code_CK = cusinspfirst.Consign_Code_CK;
                                    newcusinsp.Book_Flat_Code = cusinspfirst.Book_Flat_Code;
                                    newcusinsp.Pieces_TS = cusinspfirst.Pieces_TS;
                                    newcusinsp.Weight_TS = cusinspfirst.Weight_TS;
                                    newcusinsp.Volume_TS = cusinspfirst.Volume_TS;
                                    newcusinsp.Pieces_Fact = cusinspfirst.Pieces_Fact;
                                    newcusinsp.Weight_Fact = cusinspfirst.Weight_Fact;
                                    newcusinsp.Volume_Fact = cusinspfirst.Volume_Fact;
                                    newcusinsp.Pieces_BG = cusinspfirst.Pieces_BG;
                                    newcusinsp.Weight_BG = cusinspfirst.Weight_BG;
                                    newcusinsp.Volume_BG = cusinspfirst.Volume_BG;
                                    newcusinsp.IS_Checked_BG = cusinspfirst.IS_Checked_BG;
                                    newcusinsp.Check_QTY = cusinspfirst.Check_QTY;
                                    newcusinsp.Check_Date = cusinspfirst.Check_Date;
                                    newcusinsp.Fileupload = cusinspfirst.Fileupload;
                                    newcusinsp.Customs_Declaration = rule.value;
                                    is_cusinsp = true;
                                    foreach (var cusinsp in cusInspDataRep)
                                    {
                                        _customsInspectionService.Delete(cusinsp);
                                    }
                                }
                            }
                            if (rule.field == "Num_BG" && !string.IsNullOrEmpty(rule.value))
                            {//更新报关报检中的  报关票数
                                newcusinsp.Num_BG = Int32.Parse(rule.value);
                                is_cusinsp = true;
                            }
                            if (rule.field == "Customs_Broker_BG" && !string.IsNullOrEmpty(rule.value))
                            {//报关方
                                newcusinsp.Customs_Broker_BG = rule.value;
                                is_cusinsp = true;
                            }
                            if (rule.field == "Customs_Date_BG" && !string.IsNullOrEmpty(rule.value))
                            {//报关日期
                                var date = Convert.ToDateTime(rule.value);
                                newcusinsp.Customs_Date_BG = date;
                                is_cusinsp = true;
                            }
                        }
                        #endregion
                        if (is_ops_m)
                        {//更新总单
                            _oPS_M_OrderService.Update(ops_m);
                        }
                        if (is_ops_e)
                        {//更新委托
                            _oPS_EntrustmentInforService.Update(ops_e);
                        }
                        if (is_ops_h)
                        {//更新分单
                            foreach (var item in ops_h)
                            {
                                _oPS_H_OrderService.Update(item);
                            }
                        }
                        if (is_cusinsp)
                        {
                            _customsInspectionService.Insert(newcusinsp);
                        }

                    }
                }
                _unitOfWork.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, err = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(OPS_M_Order _oPS_M_Order, int index = 0)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();

            //客商信息
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            if (!string.IsNullOrEmpty(_oPS_M_Order.Airways_Code))
            {//航空公司
                var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == _oPS_M_Order.Airways_Code).FirstOrDefault();
                ODynamic.Airways_CodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
            }
            var OPS_EntrustmentInfor = _oPS_M_Order.OPS_EntrustmentInfors.FirstOrDefault();
            OPS_EntrustmentInfor = OPS_EntrustmentInfor ?? new OPS_EntrustmentInfor();
            if (!string.IsNullOrEmpty(_oPS_M_Order.FWD_Code) || !string.IsNullOrEmpty(OPS_EntrustmentInfor.FWD_Code))
            {//国外代理
                var FWD_Code = string.IsNullOrEmpty(OPS_EntrustmentInfor.FWD_Code) ? _oPS_M_Order.FWD_Code : OPS_EntrustmentInfor.FWD_Code;
                var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == FWD_Code).FirstOrDefault();
                ODynamic.FWD_CodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
            }

            //港口
            var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            if (!string.IsNullOrEmpty(_oPS_M_Order.Depart_Port))
            {//启运港
                var OPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == _oPS_M_Order.Depart_Port).FirstOrDefault();
                ODynamic.Depart_PortNAME = OPARA_AirPort == null ? "" : _oPS_M_Order.Depart_Port + "|" + OPARA_AirPort.PortNameEng;
            }
            if (!string.IsNullOrEmpty(_oPS_M_Order.End_Port) || !string.IsNullOrEmpty(OPS_EntrustmentInfor.End_Port))
            {//目的港
                var End_Port = string.IsNullOrEmpty(OPS_EntrustmentInfor.End_Port) ? _oPS_M_Order.End_Port : OPS_EntrustmentInfor.End_Port;
                var OPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == End_Port).FirstOrDefault();
                ODynamic.End_PortNAME = OPARA_AirPort == null ? "" : End_Port + "|" + OPARA_AirPort.PortNameEng;
            }

            //币种
            var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);
            if (!string.IsNullOrEmpty(_oPS_M_Order.Currency_M))
            {//币种
                var OPARA_CURR = ArrPARA_CURR.Where(x => x.CURR_CODE == _oPS_M_Order.Currency_M).FirstOrDefault();
                ODynamic.Currency_MNAME = OPARA_CURR == null ? "" : OPARA_CURR.CURR_Name;
            }

            //航线
            var ArrPARA_AirLine = (List<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);
            if (!string.IsNullOrEmpty(_oPS_M_Order.Flight_No))
            {//航班信息
                var OARA_AirLine = ArrPARA_AirLine.Where(x => x.AirCode == _oPS_M_Order.Flight_No).FirstOrDefault();
                ODynamic.Flight_NoNAME = OARA_AirLine == null ? "" : OARA_AirLine.AirCompany;
            }

            //成交条款
            //var ArrDealArticle = (List<DealArticle>)CacheHelper.Get_SetCache(Common.CacheNameS.DealArticle);
            //if (!string.IsNullOrEmpty(_oPS_M_Order.Bragainon_Article_M))
            //{//成交条款
            //    var ODealArticle = ArrDealArticle.Where(x => x.DealArticleCode.Contains(_oPS_M_Order.Bragainon_Article_M)).FirstOrDefault();
            //    ODynamic.Bragainon_Article_MNAME = ODealArticle == null ? "" : ODealArticle.DealArticleName;
            //}

            //基础代码
            var ArrCodeItem = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST); //(List<CodeItem>)CacheHelper.Get_SetCache(Common.CacheNameS.CodeItem);

            if (!string.IsNullOrEmpty(_oPS_M_Order.Pay_Mode_M))
            {//付款方式
                var ODealArticle = ArrCodeItem.Where(x => x.DOCCODE == "Pay_Mode" && x.LISTCODE == _oPS_M_Order.Pay_Mode_M).FirstOrDefault();
                ODynamic.Pay_Mode_MNAME = ODealArticle == null ? "" : _oPS_M_Order.Pay_Mode_M + "|" + ODealArticle.LISTNAME;
            }
            else
            {
                var ODealArticle = ArrCodeItem.Where(x => x.DOCCODE == "Pay_Mode" && x.LISTCODE == "PP").FirstOrDefault();
                ODynamic.Pay_Mode_MNAME = ODealArticle == null ? "" : ODealArticle.LISTNAME;
            }
            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();
            //添加人
            if (!string.IsNullOrEmpty(_oPS_M_Order.ADDWHO))
            {
                var OAppUser = ArrAppUser.Where(x => x.UserName == _oPS_M_Order.ADDWHO).FirstOrDefault();
                ODynamic.ADDWHONAME = OAppUser == null ? "" : OAppUser.UserNameDesc;
            }
            //修改人
            if (!string.IsNullOrEmpty(_oPS_M_Order.EDITWHO))
            {
                var OAppUser = ArrAppUser.Where(x => x.UserName == _oPS_M_Order.EDITWHO).FirstOrDefault();
                ODynamic.EDITWHONAME = OAppUser == null ? "" : OAppUser.UserNameDesc;
            }

            int i = 0;
            foreach (var item in _oPS_M_Order.OPS_EntrustmentInfors)
            {
                if (i == index)
                {
                    //if (!string.IsNullOrEmpty(item.Ty_Type))
                    //{//接单类型
                    //    var OCodeItem = ArrCodeItem.Where(x => x.Code.Contains(item.Ty_Type) && x.Id == 41).FirstOrDefault();
                    //    ODynamic.Ty_TypeNAME = OCodeItem == null ? "" : OCodeItem.Text;
                    //}
                    if (!string.IsNullOrEmpty(item.Consign_Code))
                    {//发货方
                        var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Consign_Code).FirstOrDefault();
                        ODynamic.Consign_CodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
                    }
                    if (!string.IsNullOrEmpty(item.Entrustment_Name))
                    {//委托方
                        var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Entrustment_Name).FirstOrDefault();
                        ODynamic.Entrustment_NameNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
                    }
                    if (!string.IsNullOrEmpty(item.Consignee_Code))
                    {//收货方
                        var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Consignee_Code).FirstOrDefault();
                        ODynamic.Consignee_CodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
                    }
                    if (!string.IsNullOrEmpty(item.Carriage_Account_Code))
                    {//运费结算方
                        var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Carriage_Account_Code).FirstOrDefault();
                        ODynamic.Carriage_Account_CodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
                    }
                    if (!string.IsNullOrEmpty(item.Incidental_Account_Code))
                    {//杂费结算方
                        var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Incidental_Account_Code).FirstOrDefault();
                        ODynamic.Incidental_Account_CodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
                    }
                    if (!string.IsNullOrEmpty(item.MoorLevel))
                    {//靠级
                        var OCodeItem = ArrCodeItem.Where(x => x.DOCCODE == "MoorLevel" && x.LISTCODE == item.MoorLevel).FirstOrDefault();
                        ODynamic.MoorLevelNAME = OCodeItem == null ? "" : OCodeItem.LISTNAME;
                    }
                    if (!string.IsNullOrEmpty(item.Book_Flat_Code))
                    {//订舱方
                        var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Book_Flat_Code).FirstOrDefault();
                        ODynamic.Book_Flat_CodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
                    }
                    if (!string.IsNullOrEmpty(item.Transfer_Port))
                    {//中转港
                        var OPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == item.Transfer_Port).FirstOrDefault();
                        ODynamic.Transfer_PortNAME = OPARA_AirPort == null ? "" : item.Transfer_Port + "|" + OPARA_AirPort.PortNameEng;
                    }
                    if (!string.IsNullOrEmpty(item.Delivery_Point))
                    {//交货地点
                        var ODealArticle = ArrCodeItem.Where(x => x.DOCCODE == "Delivery_Point" && x.LISTCODE == item.Delivery_Point).FirstOrDefault();
                        ODynamic.Delivery_PointNAME = ODealArticle == null ? "" : ODealArticle.LISTNAME;
                    }
                    if (item.SallerId.HasValue && item.SallerId > 0)//销售
                    {
                        var ArrSaller = (List<Saller>)CacheHelper.Get_SetCache(Common.CacheNameS.Saller);
                        var OSaller = ArrSaller.Where(x => x.Status == AirOutEnumType.UseStatusEnum.Enable && x.Id == item.SallerId).FirstOrDefault();
                        ODynamic.SallerIdNAME = OSaller == null ? "" : OSaller.Name;
                    }
                    i = 0;
                    break;
                }
                else
                {
                    break;
                }
                i++;
            }
            foreach (var item in _oPS_M_Order.OPS_H_Orders)
            {
                if (i == index)
                {
                    if (!string.IsNullOrEmpty(item.Currency_H))
                    {//分单币种
                        var OPARA_CURR = ArrPARA_CURR.Where(x => x.CURR_CODE.Contains(item.Currency_H)).FirstOrDefault();
                        ODynamic.Currency_HNAME = OPARA_CURR == null ? "" : OPARA_CURR.CURR_Name;
                    }
                    //if (!string.IsNullOrEmpty(item.Bragainon_Article_H))
                    //{//分单成交条款 
                    //    var ODealArticle = ArrDealArticle.Where(x => x.DealArticleCode.Contains(item.Bragainon_Article_H)).FirstOrDefault();
                    //    ODynamic.Bragainon_Article_HNAME = ODealArticle == null ? "" : ODealArticle.DealArticleName;
                    //}
                    //if (!string.IsNullOrEmpty(item.Pay_Mode_H))
                    //{//分单付款方式
                    //    var ODealArticle = ArrCodeItem.Where(x => x.DOCCODE == "Pay_Mode" && x.LISTCODE == item.Pay_Mode_H).FirstOrDefault();
                    //    ODynamic.Pay_Mode_HNAME = ODealArticle == null ? "" : _oPS_M_Order.Pay_Mode_M + "|" + ODealArticle.LISTNAME;
                    //}
                    //else
                    //{
                    //    var ODealArticle = ArrCodeItem.Where(x => x.Code == "PP").FirstOrDefault();
                    //    ODynamic.Pay_Mode_HNAME = ODealArticle == null ? "" : ODealArticle.Text;
                    //}
                    i = 0;
                    break;
                }
                i++;
            }

            return ODynamic;
        }

        // GET: OPS_M_Orders/Edit/5
        public ActionResult Edit(int id, string Operation_Id = "")
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var oPS_M_Order = new OPS_M_Order();
            var oPS_EntrustmentInfor = new OPS_EntrustmentInfor();
            if (id >= 0)
            {
                oPS_M_Order = _oPS_M_OrderService.Queryable().Where(x => x.Id == id)
                    .Include(x => x.OPS_EntrustmentInfors)
                    .FirstOrDefault();
                if (oPS_M_Order == null)
                {
                    return HttpNotFound();
                }
                foreach (var item in oPS_M_Order.OPS_EntrustmentInfors)
                {
                    oPS_EntrustmentInfor = item;
                }
                var ops_h_orderlist = _oPS_H_OrderService.Queryable().Where(x => x.MBLId == oPS_M_Order.Id).OrderBy(x => x.Id).ToList();

                oPS_M_Order.OPS_H_Orders = ops_h_orderlist;
                var ODynamic = GetFromNAME(oPS_M_Order, 0);
                ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            }
            else
            {
                var ops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == Operation_Id).FirstOrDefault();
                if (ops_en == null)
                {
                    return HttpNotFound();
                }
                oPS_M_Order = _oPS_M_OrderService.Queryable().Where(x => x.Id == ops_en.MBLId)
                    .Include(x => x.OPS_EntrustmentInfors)
                    .FirstOrDefault();
                if (oPS_M_Order == null)
                {
                    return HttpNotFound();
                }
                foreach (var item in oPS_M_Order.OPS_EntrustmentInfors)
                {
                    oPS_EntrustmentInfor = item;
                }
                var ops_h_orderlist = _oPS_H_OrderService.Queryable().Where(x => x.MBLId == oPS_M_Order.Id).OrderBy(x => x.Id).ToList();
                oPS_M_Order.OPS_H_Orders = ops_h_orderlist;
                var ODynamic = GetFromNAME(oPS_M_Order, 0);
                ViewData["FormNAME"] = Newtonsoft.Json.JsonConvert.SerializeObject(ODynamic);
            }
            var AirTime = "";
            if (!string.IsNullOrEmpty(oPS_EntrustmentInfor.Flight_No))
            {
                var AirLine = _unitOfWork.RepositoryAsync<PARA_AirLine>().Queryable().Where(x => x.AirCode == oPS_EntrustmentInfor.Flight_No).FirstOrDefault();
                if (AirLine != null)
                {
                    AirTime = AirLine.AirTime;
                }
            }
            ViewBag.AirTime = AirTime;

            var mbl_picture = new List<Picture>();
            var hbl_picture = new List<Picture>();
            if (oPS_M_Order != null)
            {
                #region 取总单的附档导入文件
                if (!string.IsNullOrEmpty(oPS_M_Order.MBL))
                {
                    var code = oPS_M_Order.MBL + oPS_M_Order.Id.ToString();
                    var picture = _pictureService.Queryable().Where(x => x.Code.Equals(code) && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable && x.Type == AirOutEnumType.PictureTypeEnum.Fileupload_MBL).ToList();
                    if (picture != null)
                    {
                        mbl_picture = picture;
                    }
                }
                #endregion

                #region 取分单的附档导入文件
                if (oPS_M_Order.OPS_H_Orders != null && oPS_M_Order.OPS_H_Orders.Count > 0)
                {
                    var h_order = oPS_M_Order.OPS_H_Orders.FirstOrDefault();
                    if (!string.IsNullOrEmpty(h_order.HBL))
                    {
                        var code = h_order.HBL + h_order.Id.ToString();
                        var picture = _pictureService.Queryable().Where(x => x.Code.Equals(code) && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable && x.Type == AirOutEnumType.PictureTypeEnum.Fileupload_HBL).ToList();
                        if (picture != null)
                        {
                            hbl_picture = picture;
                        }
                    }
                }

                #endregion
            }
            ViewBag.mbl_picture = mbl_picture;
            ViewBag.hbl_picture = hbl_picture;

            return View(oPS_M_Order);
        }

        // POST: OPS_M_Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(OPS_M_Order oPS_M_Order)
        {
            if (ModelState.IsValid)
            {
                oPS_M_Order.ObjectState = ObjectState.Modified;
                _oPS_M_OrderService.Update(oPS_M_Order);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a OPS_M_Order record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(oPS_M_Order);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="id">总单ID</param>
        /// <returns></returns>
        public ActionResult Edit_BatchUpdate(int id = 0)
        {
            ViewBag.MBLId = id;
            return View();
        }

        /// <summary>
        /// 退单登记
        /// </summary>
        /// <param name="id">总单ID</param>
        /// <returns></returns>
        public ActionResult Edit_Refund_Register(int id = 0)
        {
            ViewBag.MBLId = id;
            return View();
        }

        // GET: OPS_M_Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPS_M_Order oPS_M_Order = _oPS_M_OrderService.Find(id);
            if (oPS_M_Order == null)
            {
                return HttpNotFound();
            }
            return View(oPS_M_Order);
        }

        // POST: OPS_M_Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OPS_M_Order oPS_M_Order = _oPS_M_OrderService.Find(id);
            _oPS_M_OrderService.Delete(oPS_M_Order);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a OPS_M_Order record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _oPS_M_OrderService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        //导出操作清单
        [HttpPost]
        public ActionResult ExportOperationList(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            if (data == null || data.Count == 0)
            {
                return null;
            }
            var Qcus = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var QPARA_AirLineRep = (IEnumerable<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);


            //var rows = (from a in data
            //            join c in Qcus on a.Entrustment_Name equals c.EnterpriseId into c_tmp
            //            from ctmp in c_tmp.DefaultIfEmpty()

            //            select new
            //             {
            //                 Operation_Id = a.Operation_Id,
            //                 Consign_Code = ctmp == null ? a.Entrustment_Name : ctmp.EnterpriseShortName,//委托方
            //                 MBL = a.MBL == null ? "" : a.MBL.Substring(0, 3) + "-" + a.MBL.Substring(3, 4) + " " + a.MBL.Substring(7),
            //                 Flight_Date_Want = a.Flight_No + "/",
            //                 End_Port = a.End_Port,
            //                 Pieces_Fact = a.Pieces_Fact,
            //                 Weight_Fact = a.Weight_Fact,
            //                 Volume_Fact = a.Volume_Fact,
            //                 Pieces_SK = a.Pieces_SK,
            //                 Weight_SK = a.Weight_SK,
            //                 Volume_SK = a.Volume_SK,
            //                 Remark = a.Remark,
            //                 Flight_Date_Wantdate = a.Flight_Date_Want,
            //                 Flight_No = a.Flight_No,
            //             }).OrderBy(x => x.Flight_Date_Wantdate).ThenBy(x => x.Flight_No).ThenBy(x => x.End_Port).ThenBy(x => x.MBL).ToList();

            var rows = (from a in data
                        join c in Qcus on a.Entrustment_Name equals c.EnterpriseId into c_tmp
                        from ctmp in c_tmp.DefaultIfEmpty()
                        join p in QPARA_AirLineRep on a.Flight_No equals p.AirCode into p_tmp
                        from ptmp in p_tmp.DefaultIfEmpty()

                        select new
                        {
                            Operation_Id = a.Operation_Id,
                            Consign_Code = ctmp == null ? a.Entrustment_Name : ctmp.EnterpriseShortName,//委托方
                            MBL = a.MBL == null ? "" : a.MBL.Substring(0, 3) + "-" + a.MBL.Substring(3, 4) + " " + a.MBL.Substring(7),
                            Flight_Date_Want = a.Flight_No,
                            FlightDateWant = ptmp == null ? "" : ptmp.AirTime,
                            Flight_Date_Want_Time1 = a.Flight_Date_Want == null ? "" : Convert.ToDateTime(a.Flight_Date_Want).ToString("yyyy-MM-dd"),
                            Flight_Date_Want_Time = a.Flight_Date_Want == null ? "" : Convert.ToDateTime(a.Flight_Date_Want).ToString("MM-dd"),
                            End_Port = a.End_Port,
                            Pieces_Fact = a.Pieces_Fact,
                            Weight_Fact = a.Weight_Fact,
                            Volume_Fact = a.Volume_Fact,
                            Pieces_SK = a.Pieces_SK,
                            Weight_SK = a.Weight_SK,
                            Volume_SK = a.Volume_SK,
                            Remark = a.Remark,
                        }).OrderBy(x => x.Flight_Date_Want_Time1).ThenBy(x => x.FlightDateWant).ThenBy(x => x.End_Port).ThenBy(x => x.MBL).ToList();

            var rowsresult = rows.Select(n => new
            {
                n.Operation_Id,
                n.Consign_Code,
                n.MBL,
                Flight_Date_Want = n.Flight_Date_Want + "/" + n.Flight_Date_Want_Time + "/",
                n.End_Port,
                n.Pieces_Fact,
                n.Weight_Fact,
                n.Volume_Fact,
                n.Pieces_SK,
                n.Weight_SK,
                n.Volume_SK,
                n.Remark,
            }).ToList();
            //var rowsresult = rows.Select(n => new
            //{
            //    n.Operation_Id,
            //    n.Consign_Code,
            //    n.MBL,
            //    n.Flight_Date_Want,
            //    n.End_Port,
            //    n.Pieces_Fact,
            //    n.Weight_Fact,
            //    n.Volume_Fact,
            //    n.Pieces_SK,
            //    n.Weight_SK,
            //    n.Volume_SK,
            //    n.Remark,
            //}).ToList();

            List<OperationList> OperationList = new List<OperationList>();
            foreach (var item in rowsresult)
            {
                OperationList O = new OperationList();
                O.Operation_Id = item.Operation_Id;
                O.Consign_Code = item.Consign_Code;
                O.MBL = item.MBL;
                O.Flight_Date_Want = item.Flight_Date_Want;
                O.End_Port = item.End_Port;
                O.Pieces_Fact = item.Pieces_Fact;
                O.Weight_Fact = item.Weight_Fact;
                O.Volume_Fact = item.Volume_Fact;
                O.Pieces_SK = item.Pieces_SK;
                O.Weight_SK = item.Weight_SK;
                O.Volume_SK = item.Volume_SK;
                O.Remark = item.Remark;
                OperationList.AddRange(O);

            }
            OperationList o = new OperationList();
            o.Operation_Id = "";
            o.Consign_Code = "";
            o.MBL = "";
            o.Flight_Date_Want = "";
            o.End_Port = "";
            o.Pieces_Fact = null;
            o.Weight_Fact = null;
            o.Volume_Fact = null;
            o.Pieces_SK = null;
            o.Weight_SK = null;
            o.Volume_SK = null;
            o.Remark = "";

            //按照排序后，判断前后目的港不同并插入空行
            var m = 0;
            var countOperationList = OperationList.Count();
            for (var i = 0; i < countOperationList + m; i++)
            {
                var item = OperationList[i];
                if (i + 1 == countOperationList + m)
                {
                    break;
                }
                var next = OperationList[i + 1];
                if (item.End_Port != next.End_Port)
                {
                    OperationList.Insert(i + 1, o);
                    m++;
                    i++;
                }
                else
                {
                    continue;
                }
            }

            ////获取 空白行 插入位置
            //Dictionary<string, int> dict = new Dictionary<string, int>();
            //var cout = OperationList.Count();
            //for (var i = 0; i < cout; i++)
            //{
            //    var item = OperationList[i];
            //    if (i + 1 == cout)
            //    {
            //        break;
            //    }
            //    var next = OperationList[i + 1];
            //    if ((item.End_Port != next.End_Port) && item.End_Port != null)
            //    {
            //        dict.Add(item.End_Port, dict.Count() + i + 1);
            //    }
            //}
            //foreach (var ss in dict)
            //{
            //    OperationList OO = new OperationList();
            //    OO.Operation_Id = "";
            //    OO.Consign_Code = "";
            //    OO.MBL = "";
            //    OO.Flight_Date_Want = "";
            //    OO.End_Port = "";
            //    OO.Pieces_Fact = null;
            //    OO.Weight_Fact = null;
            //    OO.Volume_Fact = null;
            //    OO.Pieces_SK = null;
            //    OO.Weight_SK = null;
            //    OO.Volume_SK = null;
            //    OO.Remark = "";
            //    OperationList.Insert(ss.Value, OO);
            //}

            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(OperationList.ToList(), @"\FileModel\CZQDmodel.xlsx", 0, 0, out outfile, "操作清单");
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            sheet.Cells.Columns[0].Width = 9;
            sheet.Cells.Columns[1].Width = 16;
            sheet.Cells.Columns[2].Width = 12;
            sheet.Cells.Columns[3].Width = 16;
            sheet.Cells.Columns[4].Width = 6;
            sheet.Cells.Columns[5].Width = 8;
            sheet.Cells.Columns[6].Width = 8;
            sheet.Cells.Columns[7].Width = 8;
            sheet.Cells.Columns[8].Width = 8;
            sheet.Cells.Columns[9].Width = 8;
            sheet.Cells.Columns[10].Width = 8;
            sheet.Cells.Columns[11].Width = 25;
            var newms = Server.MapPath(@"\DownLoad\456.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        //导出业务接单信息
        [HttpPost]
        public ActionResult ExportExcelOrder(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            if (data == null || data.Count == 0)
            {
                return null;
            }
            var Qcus = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();
            var PARA_AirLineRep = (IEnumerable<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);

            var rowsdate = (from a in data
                            join c in Qcus on a.Entrustment_Name equals c.EnterpriseId into c_tmp
                            from ctmp in c_tmp.DefaultIfEmpty()
                            join d in Qcus on a.Book_Flat_Code equals d.EnterpriseId into d_tmp
                            from dtmp in d_tmp.DefaultIfEmpty()
                            join e in Qcus on a.Airways_Code equals e.EnterpriseId into e_tmp
                            from etmp in e_tmp.DefaultIfEmpty()
                            join b in ArrAppUser on a.ADDWHO equals b.UserName into b_tmp
                            from btmp in b_tmp.DefaultIfEmpty()
                            join p in PARA_AirLineRep on a.Flight_No equals p.AirCode into p_tmp
                            from ptmp in p_tmp.DefaultIfEmpty()
                            select new
                            {
                                Batch_Num = a.Batch_Num,
                                Operation_Id = a.Operation_Id,
                                Entrustment_Name = ctmp == null ? a.Entrustment_Name : ctmp.EnterpriseShortName,
                                MBL = a.MBL == null ? "" : a.MBL.Substring(0, 3) + "-" + a.MBL.Substring(3, 4) + " " + a.MBL.Substring(7),
                                HBL = a.HBL,
                                Flight_No = a.Flight_No,
                                FlightDateWant = ptmp == null ? "" : ptmp.AirTime,
                                Flight_Date_Want = a.Flight_Date_Want == null ? "" : Convert.ToDateTime(a.Flight_Date_Want).ToString("yyyy-MM-dd"),
                                Book_Flat_Code = dtmp == null ? a.Book_Flat_Code : dtmp.EnterpriseShortName,
                                Airways_Code = etmp == null ? a.Airways_Code : etmp.EnterpriseShortName,
                                Depart_Port = a.Depart_Port,
                                End_Port = a.End_Port,
                                Pieces_Fact = a.Pieces_Fact,
                                Weight_Fact = a.Weight_Fact,
                                Volume_Fact = a.Volume_Fact,
                                Pieces_SK = a.Pieces_SK,
                                Weight_SK = a.Weight_SK,
                                Volume_SK = a.Volume_SK,
                                Is_Self = a.Is_Self == true ? "是" : "否",
                                Entrustment_Code = a.Entrustment_Code,
                                Remark = a.Remark,
                                ADDWHO = btmp == null ? a.ADDWHO : btmp.UserNameDesc,

                            }).OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.FlightDateWant).ThenBy(x => x.End_Port).ThenBy(x => x.MBL).AsQueryable().ToList();
            var dataresult = rowsdate.Select(n => new
            {
                n.Batch_Num,
                n.Operation_Id,
                n.Entrustment_Name,
                n.MBL,
                n.HBL,
                n.Flight_No,
                n.Flight_Date_Want,
                n.Book_Flat_Code,
                n.Airways_Code,
                n.Depart_Port,
                n.End_Port,
                n.Pieces_Fact,
                n.Weight_Fact,
                n.Volume_Fact,
                n.Pieces_SK,
                n.Weight_SK,
                n.Volume_SK,
                n.Is_Self,
                n.Entrustment_Code,
                n.Remark,
                n.ADDWHO,
            });
            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(dataresult.ToList(), @"\FileModel\YWJD.xlsx", 0, 0, out outfile, "业务接单信息");
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            sheet.Cells.Columns[0].Width = 10;//批次号
            sheet.Cells.Columns[1].Width = 10;//业务编号
            sheet.Cells.Columns[2].Width = 15;//委托方
            sheet.Cells.Columns[3].Width = 12;//总单号
            sheet.Cells.Columns[4].Width = 10;//分单号
            sheet.Cells.Columns[5].Width = 6;//航班号
            sheet.Cells.Columns[6].Width = 10;//航班时间
            sheet.Cells.Columns[7].Width = 15;//订舱方
            sheet.Cells.Columns[8].Width = 15;//航空公司
            sheet.Cells.Columns[9].Width = 8;//起运港
            sheet.Cells.Columns[10].Width = 8;//目的港
            sheet.Cells.Columns[11].Width = 8;//实际件数
            sheet.Cells.Columns[12].Width = 8;//实际重量
            sheet.Cells.Columns[13].Width = 8;//实际体积
            sheet.Cells.Columns[14].Width = 8;//分单件数
            sheet.Cells.Columns[15].Width = 8;//分单毛重
            sheet.Cells.Columns[16].Width = 8;//分单体积
            sheet.Cells.Columns[17].Width = 6;//自营
            sheet.Cells.Columns[18].Width = 12;// 委托方编号
            sheet.Cells.Columns[19].Width = 25;//备注
            sheet.Cells.Columns[20].Width = 10;//接单人员
            var newms = Server.MapPath(@"\DownLoad\789.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        //导出拼货表
        [HttpPost]
        public ActionResult ExportSpellingList(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            //var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            if (data == null || data.Count == 0)
            {
                return null;
            }

            #region 查询需要导出的数据

            //对选中的行进行字段取值处理
            var rows = data.Select(n => new
            {
                Operation_Id = n.Operation_Id,
                Consign_Code = n.Consign_Code,//发货方
                RK = "",//拼号
                MBL = n.MBL,
                HBL = n.HBL,
                MBLId = n.MBLId,
                End_Port = n.End_Port,
                Fncidental_Expenses_M = "",//主单—杂费P/C
                FPieces = n.Pieces_Fact,
                FWeight = n.Weight_Fact,
                FVolume = n.Volume_Fact,
                TPieces = n.Pieces_TS,
                TWeight = n.Weight_TS,
                TVolume = n.Volume_TS,
                Flight_Date_Want = n.Flight_Date_Want == null ? "" : Convert.ToDateTime(n.Flight_Date_Want).ToString("yyyy-MM-dd"),//航班号/日期
                Flight_Date_Want_MD = n.Flight_Date_Want == null ? "" : Convert.ToDateTime(n.Flight_Date_Want).ToString("MM-dd"),
                Flight_No = n.Flight_No,
                Bragainon_Article_H = "",//分单 - 成交条款
                Weight_BG = 0, //报关报检 - 报关重量
                Remark = n.Remark + "  " + (n.MoorLevel == null ? "" : "靠级" + n.MoorLevel),
                MoorLevel = n.MoorLevel,
                Book_Flat_Code = n.Book_Flat_Code,
                Delivery_Point = n.Delivery_Point,
            }).ToList();
            #endregion

            var QM = _unitOfWork.Repository<OPS_M_Order>().Queryable().ToList();
            var QH = _unitOfWork.Repository<OPS_H_Order>().Queryable().ToList();
            var insp = _unitOfWork.Repository<CustomsInspection>().Queryable().ToList();
            var Qcus = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var QBD_DEFDOC_LIST = ((IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST)).Where(x => x.DOCCODE == "Delivery_Point");
            var PARA_AirLineRep = (IEnumerable<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);
            //对于一个业务编号下的报关重量合计
            var QQcus = (from n in insp
                         group n by n.Operation_ID into g
                         select new
                         {
                             Operation_ID = g.Key,
                             Weight_BG = g.Sum(x => x.Weight_BG),
                         }).ToList();

            //var QE = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable();

            //关联分单，报关信息，枚举，客商，航班信息，进行数据整合按照航班日期-航班号日期排序
            var rowsdata = (from n in rows
                            //join m in QM on n.MBLId equals m.Id
                            join h in QH on n.Operation_Id equals h.Operation_Id into h_tmp
                            from htmp in h_tmp.DefaultIfEmpty()
                            join c in Qcus on n.Consign_Code equals c.EnterpriseId into c_tmp
                            from ctmp in c_tmp.DefaultIfEmpty()
                            join cc in Qcus on n.Book_Flat_Code equals cc.EnterpriseId into cc_tmp
                            from cctmp in cc_tmp.DefaultIfEmpty()
                            join b in QBD_DEFDOC_LIST on n.Delivery_Point equals b.LISTCODE into b_tmp
                            from bbtmp in b_tmp.DefaultIfEmpty()
                            join p in QQcus on n.Operation_Id equals p.Operation_ID into i_tmp
                            from ptmp in i_tmp.DefaultIfEmpty()
                            join a in PARA_AirLineRep on n.Flight_No equals a.AirCode into a_tmp
                            from atmp in a_tmp.DefaultIfEmpty()

                            select new
                        {
                            Operation_Id = n.Operation_Id,
                            Consign_Code = ctmp == null ? n.Consign_Code : ctmp.EnterpriseShortName,//发货方
                            RK = n.RK,
                            MBL = n.MBL == null ? "" : n.MBL.Substring(0, 3) + "-" + n.MBL.Substring(3, 4) + " " + n.MBL.Substring(7),
                            HBL = n.HBL,
                            End_Port = n.End_Port,
                            Incidental_Expenses_M = (htmp == null ? "" : (htmp.Incidental_Expenses_H == "C" ? "C" : "P")),//主单—杂费P/C
                            FPieces = n.FPieces,//进仓实际
                            FWeight = n.FWeight,
                            FVolume = n.FVolume,
                            TPieces = n.TPieces,//托书
                            TWeight = n.TWeight,
                            TVolume = n.TVolume,
                            Flight_Date_Want = n.Flight_Date_Want,//航班号/日期
                            Flight_Date_Want_MD = n.Flight_Date_Want_MD,
                            Flight_No = n.Flight_No,
                            Flight_NoDATE = atmp == null ? "" : atmp.AirTime,
                            Bragainon_Article_H = (htmp == null ? "" : htmp.Bragainon_Article_H),//分单 - 成交条款
                            Weight_BG = (ptmp == null ? null : ptmp.Weight_BG), //报关报检 - 报关重量
                            Remark = n.Remark,
                            Delivery_Point = bbtmp == null ? n.Delivery_Point : bbtmp.ListFullName,
                            Book_Flat_Code = cctmp == null ? n.Book_Flat_Code : cctmp.EnterpriseShortName,//(n.Flight_No + "/" + n.Delivery_Point) == "/" ? "" : (n.Flight_No + "/" + n.Delivery_Point),
                        }).OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.Flight_NoDATE);
            //对rowsdata进行字段整合，并取出总单号不为空的数据进行航班日期-航班号日期-目的港-总单号-业务编号排序
            var rowsresult1 = rowsdata.Select(n => new
            {
                n.Operation_Id,
                n.Consign_Code,
                RK = n.RK,
                n.MBL,
                n.HBL,
                n.End_Port,
                n.Incidental_Expenses_M,
                FPieces = n.FPieces == null ? 0 : Math.Round((decimal)n.FPieces, 3),
                FWeight = n.FWeight == null ? 0 : Math.Round((decimal)n.FWeight, 3),
                FVolume = n.FVolume == null ? 0 : Math.Round((decimal)n.FVolume, 3),
                TPieces = n.TPieces == null ? 0 : Math.Round((decimal)n.TPieces, 3),
                TWeight = n.TWeight == null ? 0 : Math.Round((decimal)n.TWeight, 3),
                TVolume = n.TVolume == null ? 0 : Math.Round((decimal)n.TVolume, 3),
                Flight_Date_Want = n.Flight_No + "/" + (n.Flight_Date_Want_MD == null ? "" : n.Flight_Date_Want_MD),
                FlightDateWant = n.Flight_Date_Want,
                Flight_NoDATE = n.Flight_NoDATE,
                n.Bragainon_Article_H,
                n.Weight_BG,
                n.Remark,
                Delivery_Point_Book_Flat_Code = n.Book_Flat_Code + "/" + n.Delivery_Point,
            }).OrderBy(x => x.FlightDateWant).ThenBy(x => x.Flight_NoDATE).ThenBy(x => x.End_Port).ThenBy(x => x.MBL).ThenBy(x => x.Operation_Id).ToList();

            //对rowsdata进行字段整合，取出总单号为空的数据
            var rowsresult2 = rowsdata.Select(n => new
            {
                n.Operation_Id,
                n.Consign_Code,
                RK = n.RK,
                n.MBL,
                n.HBL,
                n.End_Port,
                n.Incidental_Expenses_M,
                FPieces = n.FPieces == null ? 0 : Math.Round((decimal)n.FPieces, 3),
                FWeight = n.FWeight == null ? 0 : Math.Round((decimal)n.FWeight, 3),
                FVolume = n.FVolume == null ? 0 : Math.Round((decimal)n.FVolume, 3),
                TPieces = n.TPieces == null ? 0 : Math.Round((decimal)n.TPieces, 3),
                TWeight = n.TWeight == null ? 0 : Math.Round((decimal)n.TWeight, 3),
                TVolume = n.TVolume == null ? 0 : Math.Round((decimal)n.TVolume, 3),
                Flight_Date_Want = n.Flight_No + "/" + n.Flight_Date_Want,
                FlightDateWant = n.Flight_Date_Want,
                Flight_NoDATE = n.Flight_NoDATE,
                n.Bragainon_Article_H,
                n.Weight_BG,
                n.Remark,
                Delivery_Point_Book_Flat_Code = n.Book_Flat_Code + "/" + n.Delivery_Point,
            }).Where(x => x.MBL == null || x.MBL == "").ToList();


            //.ThenBy(x => x.Flight_NoDATE).ThenBy(x => x.End_Port).ThenByDescending(x => x.MBL);
            //将总单号为空的放在最前面，不为空的放后面
            //var rowsresult = rowsresult2.Concat(rowsresult1);
            //获取 总单号 排序
            int count = 0;
            var grop = from p in rowsresult1
                       group p by p.MBL into g
                       select new
                       {
                           Operation_Id = "",
                           Consign_Code = "",
                           RK = count++,
                           MBL = g.Key,
                           HBL = "",
                           End_Port = "",
                           Incidental_Expenses_M = "",
                           HPieces = g.Sum(x => x.FPieces),
                           HVolume = g.Sum(x => x.FVolume),
                           HWeight = g.Sum(x => x.FWeight),
                           Flight_Date_Want = "",
                           Bragainon_Article_H = "",
                           Weight_BG = 0,
                           Remark = "",
                           Delivery_Point_Book_Flat_Code = "",
                       };

            //var fuc = new Func<string, int>((x) => { return grop.Where(n => n.MBL == x).FirstOrDefault().RK; });//匿名方法 = 委托


            var rowsresults = rowsresult1.Select(n => new
            {
                Operation_Id = n.Operation_Id,
                Consign_Code = n.Consign_Code,
                RK = 1,
                MBL = n.MBL,
                n.HBL,
                n.End_Port,
                n.Incidental_Expenses_M,
                HPieces = (n.FPieces == 0 ? "" : n.FPieces.ToString()) + "|" + (n.TPieces == 0 ? "" : n.TPieces.ToString()),
                HWeight = (n.FWeight == 0 ? "" : n.FWeight.ToString()) + "|" + (n.TWeight == 0 ? "" : n.TWeight.ToString()),
                HVolume = (n.FVolume == 0 ? "" : n.FVolume.ToString()) + "|" + (n.TVolume == 0 ? "" : n.TVolume.ToString()),
                n.Flight_Date_Want,
                n.FlightDateWant,
                n.Bragainon_Article_H,
                Weight_BG = n.Weight_BG,
                n.Remark,
                Delivery_Point_Book_Flat_Code = n.Delivery_Point_Book_Flat_Code,
            }).ToList();

            var grop1 = from p in rowsresults
                        group p by p.FlightDateWant into g
                        select g.OrderByDescending(x => x.MBL == "");

            List<SpellingList> ListSpellingList = new List<SpellingList>();

            foreach (var itemm in grop1)
            {
                foreach (var item in itemm)
                {
                    SpellingList s = new SpellingList();
                    s.Operation_Id = item.Operation_Id;
                    s.Consign_Code = item.Consign_Code;
                    s.RK = item.RK;
                    s.MBL = item.MBL;
                    s.HBL = item.HBL;
                    s.End_Port = item.End_Port;
                    s.Incidental_Expenses_M = item.Incidental_Expenses_M;
                    s.HPieces = item.HPieces;
                    s.HWeight = item.HWeight;
                    s.HVolume = item.HVolume;
                    s.Flight_Date_Want = item.Flight_Date_Want;
                    s.Bragainon_Article_H = item.Bragainon_Article_H;
                    s.Weight_BG = item.Weight_BG == 0 ? (int?)null : item.Weight_BG;
                    s.Remark = item.Remark;
                    s.Delivery_Point_Book_Flat_Code = item.Delivery_Point_Book_Flat_Code;
                    ListSpellingList.AddRange(s);
                }
            }

            //foreach (var item in rowsresults)
            //{
            //    SpellingList s = new SpellingList();
            //    s.Operation_Id = item.Operation_Id;
            //    s.Consign_Code = item.Consign_Code;
            //    s.RK = item.RK;
            //    s.MBL = item.MBL;
            //    s.HBL = item.HBL;
            //    s.End_Port = item.End_Port;
            //    s.Incidental_Expenses_M = item.Incidental_Expenses_M;
            //    s.HPieces = item.HPieces;
            //    s.HWeight = item.HWeight;
            //    s.HVolume = item.HVolume;
            //    s.Flight_Date_Want = item.Flight_Date_Want;
            //    s.Bragainon_Article_H = item.Bragainon_Article_H;
            //    s.Weight_BG = item.Weight_BG.ToString();
            //    s.Remark = item.Remark;
            //    s.Delivery_Point_Book_Flat_Code = item.Delivery_Point_Book_Flat_Code;
            //    ListSpellingList.AddRange(s);
            //}




            //Dictionary<string, int> dicttest = new Dictionary<string, int>();
            //Dictionary<string, int> dictmbl = new Dictionary<string, int>();
            //var countmbl = ListSpellingList.Count();
            //for (var i = 1; i < countmbl + 1; i++)
            //{
            //    var item = ListSpellingList[i - 1];
            //    if (i == countmbl)
            //    {
            //        dictmbl.Add(item.MBL, dictmbl.Count() + i);
            //        break;
            //    }
            //    var next = ListSpellingList[i];
            //    if (item.MBL != "" && item.MBL != null) 
            //    {
            //        if (item.MBL != next.MBL)
            //        {
            //            var aa = dictmbl.Where(x => x.Key == item.MBL && (item.MBL != null || item.MBL != "")).FirstOrDefault();
            //            if (aa.Key == item.MBL)
            //            {
            //                ListSpellingList.Remove(item);
            //                ListSpellingList.Insert(aa.Value - 1, item);


            //            }
            //            else
            //            {
            //                dictmbl.Add(item.MBL, i);
            //            }
            //        }               
            //    }
            //}

            ////获取 合计行 插入位置
            //Dictionary<string, int> dict = new Dictionary<string, int>();
            //var cout = ListSpellingList.Count();
            //for (var i = 1; i < cout + 1; i++)
            //{
            //    var item = ListSpellingList[i - 1];
            //    if (i == cout)
            //    {
            //        dict.Add(item.MBL, dict.Count() + i);
            //        break;
            //    }
            //    var next = ListSpellingList[i];
            //    if (item.MBL != next.MBL)
            //    {
            //        dict.Add(item.MBL, dict.Count() + i);
            //    }
            //}

            var m = 0;
            var countListSpellingList = ListSpellingList.Count();
            for (var i = 0; i < countListSpellingList + m; i++)
            {
                var item = ListSpellingList[i];
                var aaa = grop.Where(x => x.MBL == item.MBL).FirstOrDefault();
                var MBLCOUNT = rowsresults.Where(x => x.MBL == item.MBL).Count();
                SpellingList sss = new SpellingList();
                sss.Operation_Id = aaa.Operation_Id;
                sss.Consign_Code = aaa.Consign_Code;
                sss.RK = null;
                sss.MBL = "";
                sss.HBL = aaa.HBL;
                sss.End_Port = aaa.End_Port;
                sss.Incidental_Expenses_M = aaa.Incidental_Expenses_M;
                sss.HPieces = (aaa.HPieces == 0 || MBLCOUNT < 2 || aaa.MBL == null || aaa.MBL == "") ? "" : aaa.HPieces.ToString();
                sss.HWeight = (aaa.HWeight == 0 || MBLCOUNT < 2 || aaa.MBL == null || aaa.MBL == "") ? "" : aaa.HWeight.ToString();
                sss.HVolume = (aaa.HVolume == 0 || MBLCOUNT < 2 || aaa.MBL == null || aaa.MBL == "") ? "" : aaa.HVolume.ToString();
                sss.Flight_Date_Want = aaa.Flight_Date_Want;
                sss.Bragainon_Article_H = aaa.Bragainon_Article_H;
                sss.Weight_BG = aaa.Weight_BG == 0 ? (int?)null : aaa.Weight_BG;
                sss.Remark = aaa.Remark;
                sss.Delivery_Point_Book_Flat_Code = aaa.Delivery_Point_Book_Flat_Code;

                if (i + 1 == countListSpellingList + m)
                {
                    ListSpellingList.Insert(i + 1, sss);
                    break;
                }
                var next = ListSpellingList[i + 1];
                if (item.MBL != next.MBL || (item.MBL == "" || item.MBL == null))
                {

                    ListSpellingList.Insert(i + 1, sss);
                    m++;
                    i++;
                }
                else
                {
                    continue;
                }

            }

            ////插入 合计列
            //foreach (var ss in dict)
            //{
            //    var aaa = grop.Where(x => x.MBL == ss.Key).FirstOrDefault();
            //    var MBLCOUNT = rowsresults.Where(x => x.MBL == ss.Key).Count();
            //    SpellingList sss = new SpellingList();
            //    sss.Operation_Id = aaa.Operation_Id;
            //    sss.Consign_Code = aaa.Consign_Code;
            //    sss.RK = null;
            //    sss.MBL = "";
            //    sss.HBL = aaa.HBL;
            //    sss.End_Port = aaa.End_Port;
            //    sss.Incidental_Expenses_M = aaa.Incidental_Expenses_M;
            //    sss.HPieces = (aaa.HPieces == 0 || MBLCOUNT < 2 || aaa.MBL == null || aaa.MBL == "") ? "" : aaa.HPieces.ToString();
            //    sss.HWeight = (aaa.HWeight == 0 || MBLCOUNT < 2 || aaa.MBL == null || aaa.MBL == "") ? "" : aaa.HWeight.ToString();
            //    sss.HVolume = (aaa.HVolume == 0 || MBLCOUNT < 2 || aaa.MBL == null || aaa.MBL == "") ? "" : aaa.HVolume.ToString();
            //    sss.Flight_Date_Want = aaa.Flight_Date_Want;
            //    sss.Bragainon_Article_H = aaa.Bragainon_Article_H;
            //    sss.Weight_BG = aaa.Weight_BG.ToString();
            //    sss.Remark = aaa.Remark;
            //    sss.Delivery_Point_Book_Flat_Code = aaa.Delivery_Point_Book_Flat_Code;
            //    ListSpellingList.Insert(ss.Value, sss);
            //}
            var RKnum = 1;
            for (var i = 1; i < ListSpellingList.Count(); i++)
            {

                var item = ListSpellingList[i - 1];
                var nextitem = ListSpellingList[i];
                if (item.Operation_Id == null || item.Operation_Id == "")
                {
                    item.RK = null;
                }
                else if (item.MBL == null || item.MBL == "")
                {
                    item.RK = 1;
                }
                else if (item.MBL == nextitem.MBL)
                {

                    item.RK = RKnum;
                    RKnum++;
                }
                else
                {
                    item.RK = RKnum;
                    RKnum = 1;
                }
            }

            #region 去除 autoFilter

            //int sheetRow = 3;
            //for (int outer = 0; outer < outerSourceTable.Rows.Count; outer++)
            //{
            //    var outerThingId = Convert.ToInt32(outerSourceTable.Rows[outer]["OuterThingId"]);
            //    var outerThingName = Convert.ToString(outerSourceTable.Rows[outer]["OuterThing"]);
            //    var innerThingsTable = _repository.GetInnerThings(outerThingId);
            //    if (innerThingsTable.Rows.Count > 0)
            //    {
            //        myWorksheet.Cells[sheetRow, 1].Value = outerThingName;

            //        // Load the data into the worksheet. We need to load a row at a time
            //        // to avoid the auto-filter bug
            //        for (int inner = 0; inner < innerThingsTable.Rows.Count; inner++)
            //        {
            //            var innerName = Convert.ToString(innerThingsTable.Rows[inner]["Name"]);
            //            var innerDescr = Convert.ToString(innerThingsTable.Rows[inner]["Description"]);
            //            myWorksheet.Cells[sheetRow, 2].Value = innerName;
            //            myWorksheet.Cells[sheetRow, 3].Value = innerDescr;
            //            sheetRow++;
            //        }
            //        sheetRow++;
            //    }
            //}

            #endregion

            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(ListSpellingList.ToList(), @"\FileModel\PHBUPDATE.xlsx", 4, 0, out outfile, "拼货表");
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            sheet.Cells[3, 0].Value = DateTime.Now.ToString("HH:mm");
            sheet.Cells.Columns[0].Width = 9;
            sheet.Cells.Columns[1].Width = 16;
            sheet.Cells.Columns[2].Width = 3;
            sheet.Cells.Columns[3].Width = 11;
            sheet.Cells.Columns[4].Width = 10;
            sheet.Cells.Columns[5].Width = 4;
            sheet.Cells.Columns[6].Width = 2;
            sheet.Cells.Columns[7].Width = 8;
            sheet.Cells.Columns[8].Width = 8;
            sheet.Cells.Columns[9].Width = 8;
            sheet.Cells.Columns[10].Width = 10;
            sheet.Cells.Columns[11].Width = 4;
            sheet.Cells.Columns[12].Width = 6;
            sheet.Cells.Columns[13].Width = 19;
            sheet.Cells.Columns[14].Width = 16;
            Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];//新增样式
            style.ShrinkToFit = true;
            style.VerticalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字 靠左                             
            style.Font.Color = System.Drawing.Color.Black;
            style.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
            style.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
            style.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
            style.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
            for (var i = 5; i < ListSpellingList.Count() + 5; i++)
            {
                sheet.Cells[i, 1].SetStyle(style);
            }
            var newms = Server.MapPath(@"\DownLoad\123.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        //导出仓单
        [HttpPost]
        public ActionResult ExportWarehouseReceipt(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var idarraa = idarr.Count();
            if (idarraa > 1)
            {
                return Json(new { Success = false, ErrMsg = "提示：请选择一条接单信息！" });
            }
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id) && !x.Is_TG).ToList();
            if (!data.Any()) 
            {
                return Json(new { Success = false, ErrMsg = "接单信息已删除或已退关！" });
            }
            var OP_MBL = data.Select(x => x.MBL).ToList();
            var QH = _unitOfWork.Repository<OPS_H_Order>().Queryable().ToList();
            var QM = _unitOfWork.Repository<OPS_M_Order>().Queryable().ToList();
            var QOPS_En = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable().ToList();
            var QPARA_AirPort = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var QCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var QQH = QH.Where(x => OP_MBL.Contains(x.MBL)).ToList();
            var QQM = QM.Where(x => OP_MBL.Contains(x.MBL)).ToList();
            List<pdfCDEx> ListpdfCDEx = new List<pdfCDEx>();


            //var headrows = data.Select(n => new
            //{
            //    MBL = n.MBL,
            //    Depart_Port = n.Depart_Port,
            //    End_Port = n.End_Port,
            //    Flight_No = n.Flight_No + "/" + n.Flight_Date_Want,
            //    Shipper_M = n.Shipper_M,
            //}).ToList();            
            var headrows = (from a in data
                            join b in QPARA_AirPort on a.End_Port equals b.PortCode into b_tmp
                            from btmp in b_tmp.DefaultIfEmpty()
                            join c in QPARA_AirPort on a.Depart_Port equals c.PortCode into c_tmp
                            from ctmp in c_tmp.DefaultIfEmpty()
                            join d in QCusBusInfo on a.FWD_Code equals d.EnterpriseId into d_tmp
                            from dtmp in d_tmp.DefaultIfEmpty()
                            select new
                            {
                                MBL = a.MBL == null ? "" : a.MBL.Substring(0, 3) + "-" + a.MBL.Substring(3, 4) + " " + a.MBL.Substring(7),
                                Depart_Port = ctmp == null ? a.Depart_Port : ctmp.PortNameEng,
                                End_Port = btmp == null ? a.End_Port : btmp.PortNameEng,
                                Flight_No = a.Flight_No + "/" + (a.Flight_Date_Want == null ? "" : (Convert.ToDateTime(a.Flight_Date_Want).ToString("MMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 3) + " " + Convert.ToDateTime(a.Flight_Date_Want).ToString("dd,yyyy"))),
                                FWD_Code = dtmp == null ? "" : dtmp.EngName
                            }).ToList();
            var rowsdata = (from n in data
                            join c in QH.Where(x => x.Is_TG == false) on n.MBL equals c.MBL into c_tmp
                            from ctmp in c_tmp.DefaultIfEmpty()
                            join b in QPARA_AirPort on n.End_Port equals b.PortCode into b_tmp
                            from btmp in b_tmp.DefaultIfEmpty()
                            select new
                            {
                                HBL = ctmp == null ? null : ctmp.HBL,
                                SLAC_SK = n.Slac_SK,
                                Operation_Id = ctmp == null ? null : ctmp.Operation_Id,
                                Pieces_H = ctmp == null ? null : ctmp.Pieces_H,
                                Weight_H = ctmp == null ? null : ctmp.Weight_H,
                                EN_Name_H = ctmp == null ? null : ctmp.EN_Name_H == null ? "" : ctmp.EN_Name_H.Replace("\\r\\n", "\n"),
                                End_Port = btmp == null ? n.End_Port : btmp.PortNameEng,
                                Shipper_H = ctmp == null ? null : ctmp.Shipper_H == null ? "" : ctmp.Shipper_H.Replace("\\r\\n", "\n"),
                                Consignee_H = ctmp == null ? null : ctmp.Consignee_H == null ? "" : ctmp.Consignee_H.Replace("\\r\\n", "\n"),
                                Pay_Mode_H = ctmp == null ? null : ctmp.Pay_Mode_H,
                                MBLId = ctmp == null ? null:ctmp.MBLId,
                            }).ToList();

            var rowsdata_result = (from n in rowsdata
                                   join c in QOPS_En on n.MBLId equals c.MBLId into c_tmp
                                   from ctmp in c_tmp.DefaultIfEmpty()
                                   select new
                                   {
                                       HBL = n.HBL,
                                       SLAC_SK = n.SLAC_SK,
                                       Operation_Id = n.Operation_Id,
                                       Pieces_H = n.Pieces_H,
                                       Weight_H = n.Weight_H,
                                       EN_Name_H = n.EN_Name_H,
                                       End_Port = n.End_Port,
                                       Shipper_H = n.Shipper_H,
                                       Consignee_H = n.Consignee_H,
                                       Pay_Mode_H = n.Pay_Mode_H,
                                       Is_TG = ctmp == null ? true :ctmp.Is_TG,
                                   }).ToList();

            foreach (var item in rowsdata_result)
            {
                pdfCDEx t = new pdfCDEx();
                var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Operation_Id && !x.Is_TG).FirstOrDefault();
                if (Qops_en == null)
                    continue;
                t.HAWB = item.HBL;             
                t.Pieces = Qops_en.Slac_SK == null ? item.Pieces_H.ToString() : (item.Pieces_H + "\n" + "SLAC:" + Qops_en.Slac_SK + "CTNS");
                t.Weight = item.Weight_H;
                t.EN_Name_H = item.EN_Name_H;
                t.End_Port = item.End_Port;
                t.Shipper_H = item.Shipper_H;
                t.Consignee_H = item.Consignee_H;
                t.Pay_Mode_H = item.Pay_Mode_H;
                ListpdfCDEx.AddRange(t);
            }
            //var rowresult = (from n in rowsdata
            //                 join c in Qops_en on n.Operation_Id equals c.Operation_Id into c_tmp
            //                 from ctmp in c_tmp.DefaultIfEmpty()
            //                 select new
            //                 {
            //                     HBL = n.HBL,
            //                     Pieces_H = ctmp.Slac_SK == null ? n.Pieces_H.ToString() : (n.Pieces_H + "\n" + "SLAC:" + ctmp.Slac_SK + "CTNS"),
            //                     n.Weight_H,
            //                     n.EN_Name_H,
            //                     n.End_Port,
            //                     n.Shipper_H,
            //                     n.Consignee_H,
            //                     n.Pay_Mode_H,
            //                 }).ToList();
            //var rowresult = rowsdata.Select(n => new
            //{
            //    n.HBL,
            //    Pieces_H = n.SLAC_SK == null ? n.Pieces_H.ToString() : (n.Pieces_H + "\n" + "SLAC:"+ n.SLAC_SK + "CTNS"),
            //    n.Weight_H,
            //    n.EN_Name_H,
            //    n.End_Port,
            //    n.Shipper_H,
            //    n.Consignee_H,
            //    n.Pay_Mode_H,
            //}).ToList();


            //var rowsdata = QQH.Select(n => new
            //{
            //    HBL = n.HBL,
            //    Pieces_H = n.Pieces_H,
            //    Weight_H = n.Weight_H,
            //    EN_Name_H = n.EN_Name_H,
            //    Shipper_H = n.Shipper_H,
            //    Consignee_H = n.Consignee_H,
            //    Pay_Mode_H = n.Pay_Mode_H,

            //}).ToList();


            var rows = (from n in data
                        join h in QH on n.Operation_Id equals h.Operation_Id
                        into h_tmp
                        from htmp in h_tmp.DefaultIfEmpty()
                        select new
                        {
                            Operation_Id = n.Operation_Id,
                            Pieces_SK = n.Pieces_SK,
                            Weight_SK = n.Weight_SK,
                            EN_Name_H = (htmp == null ? "" : htmp.EN_Name_H == null ? "" : htmp.EN_Name_H.Replace("\\r\\n", "\n")),
                            End_Port = n.End_Port,
                            Shipper_H = n.Shipper_H == null ? "" : n.Shipper_H.Replace("\\r\\n", "\n"),
                            Consignee_H = n.Consignee_H == null ? "" : n.Consignee_H.Replace("\\r\\n", "\n"),
                            Pay_Mode_H = (htmp == null ? "" : htmp.Pay_Mode_H),
                        }).ToList();

            //var rows = data.Select(n => new
            //{
            //    Operation_Id = n.Operation_Id,
            //    Consign_Code = n.Consign_Code,//发货方
            //    MBL = n.MBL,
            //    Flight_Date_Want = n.Flight_Date_Want,
            //    Position = "",
            //    End_Port = n.End_Port,
            //    Pieces_Fact = n.Pieces_Fact,
            //    Weight_Fact = n.Weight_Fact,
            //    Volume_Fact = n.Volume_Fact,
            //    Pieces_SK = n.Pieces_SK,
            //    Weight_SK = n.Weight_SK,
            //    Volume_SK = n.Volume_SK,
            //    Remark = n.Remark,
            //}).ToList();
            //var stream = ExcelHelper.ExportWarehouseReceiptExcel(typeof(OPS_EntrustmentInfor), rowsdata, headrows, "ExportWarehouseReceipt");
            //return File(stream, "application/vnd.ms-excel", fileName);

            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(ListpdfCDEx.ToList(), @"\FileModel\aaa.xlsx", 6, 0, out outfile, "仓单");

            #region//单元格样式 排版
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            sheet.Cells.Columns[0].Width = 19;
            sheet.Cells.Columns[1].Width = 9;
            sheet.Cells.Columns[2].Width = 9;
            sheet.Cells.Columns[3].Width = 20;
            sheet.Cells.Columns[4].Width = 9;
            sheet.Cells.Columns[5].Width = 25;
            sheet.Cells.Columns[6].Width = 24;
            sheet.Cells.Columns[7].Width = 10;

            var count = rowsdata_result.Where(x => !x.Is_TG).Count();
            for (var i = 7; i < 7 + count; i++)
            {
                sheet.Cells.Rows[i].Height = 77;
            }
            var TOTALPACKAGS = rowsdata_result.Where(x=>!x.Is_TG).Select(x => x.Pieces_H).Sum();
            var TOTALGrossWeight = rowsdata_result.Where(x => !x.Is_TG).Select(x => x.Weight_H).Sum();
            sheet.Cells[count + 8, 0].Value = "TOTAL NO. OF HAWB：";
            sheet.Cells[count + 8, 3].Value = " TOTAL PACKAGS：";
            sheet.Cells[count + 8, 5].Value = "TOTAL GrossWeight：";
            sheet.Cells[count + 8, 1].Value = count;
            sheet.Cells[count + 8, 4].Value = TOTALPACKAGS;
            sheet.Cells[count + 8, 6].Value = TOTALGrossWeight;
            sheet.Cells[count + 8, 7].Value = "KG";
            Aspose.Cells.Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式
            Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式
            Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式
            style1.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字居中
            style1.Font.Name = "宋体";//文字字体
            style1.Font.Size = 12;//文字大小
            style1.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = Aspose.Cells.CellBorderType.Thin;
            style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;//文字居中
            style2.Font.Name = "宋体";//文字字体
            style2.Font.Size = 12;//文字大小
            style3.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//文字居中
            style3.Font.Name = "宋体";//文字字体
            style3.Font.Size = 12;//文字大小
            sheet.Cells[count + 8, 0].SetStyle(style3);
            sheet.Cells[count + 8, 3].SetStyle(style3);
            sheet.Cells[count + 8, 5].SetStyle(style3);
            sheet.Cells[count + 8, 1].SetStyle(style1);
            sheet.Cells[count + 8, 4].SetStyle(style1);
            sheet.Cells[count + 8, 6].SetStyle(style1);
            sheet.Cells[count + 8, 7].SetStyle(style2);
            sheet.Cells[2, 1].Value = headrows.FirstOrDefault().MBL;
            sheet.Cells[2, 4].Value = headrows.FirstOrDefault().Depart_Port;
            sheet.Cells[2, 6].Value = headrows.FirstOrDefault().End_Port;
            sheet.Cells[4, 1].Value = headrows.FirstOrDefault().Flight_No;
            sheet.Cells[4, 4].Value = headrows.FirstOrDefault().FWD_Code;

            #endregion

            var newms = Server.MapPath(@"\DownLoad\222.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
            //var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            //var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
            //string FName = Server.MapPath(dPath + "/ExlDD/" + fileName);
            //workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
            ////return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
            //return File(FName, "application/pdf", fileName);
        }

        //导出报关明细
        public ActionResult ExportCustomDtl(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            var cus = _unitOfWork.Repository<CustomsInspection>().Queryable();
            if (data == null || data.Count == 0)
            {
                return null;
            }
            var rows = data.Select(n => new
            {
                Operation_Id = n.Operation_Id,//业务编号
                MBLId = n.MBLId,//总单ID
                Entrustment_Code = n.Entrustment_Code,//委托方编号
                MBL = n.MBL, //总单号
                HBL = n.HBL,//分单号
            }).ToList();
            var dataresult = (from a in data
                              join b in cus on a.Operation_Id equals b.Operation_ID into b_tmp
                              from btmp in b_tmp.DefaultIfEmpty()
                              select new
                              {
                                  MBL = a.MBL,//总单号
                                  HBL = a.HBL,//分单号
                                  Entrustment_Code = a.Entrustment_Code,//委托方编号
                                  Customs_Date_BG = btmp == null ? "" : Convert.ToDateTime(btmp.Customs_Date_BG).ToString("yyyy-MM-dd"), //报关日期
                                  Doc_No = "",//报关单号
                                  Pieces_BG = btmp.Pieces_BG,//报关件数
                                  Weight_BG = btmp.Weight_BG,//报关重量
                                  Book_Flat_Code = a.Book_Flat_Code,//订舱方
                                  Remark = btmp.Remarks_BG //备注
                              }).ToList();
            var stream = ExcelHelper.ExportExcel(typeof(OPS_EntrustmentInfor), dataresult, "ExportOperationList");
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        //导出订舱单
        public ActionResult ExportBook_Flat(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            var FltNo = data.Where(x => !string.IsNullOrWhiteSpace(x.Flight_No)).Select(x => x.Flight_No).ToList();
            var FltNodata = _oPS_EntrustmentInforService.Queryable().Where(x => FltNo.Contains(x.Flight_No)).ToList();
            if (data == null || data.Count == 0)
            {
                return null;
            }
            var QH = _unitOfWork.Repository<OPS_H_Order>().Queryable().ToList();
            var PARA_AirLineRep = (IEnumerable<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);
            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();

            var rows = (from n in data
                        join c in PARA_AirLineRep on n.Flight_No equals c.AirCode into c_tmp
                        from ctmp in c_tmp.DefaultIfEmpty()
                        join a in ArrAppUser on n.ADDWHO equals a.UserName into a_tmp
                        from atmp in a_tmp.DefaultIfEmpty()
                        select new
                       {
                           Flight_No = n.Flight_No, //航班号
                           Flight_Date_Want = n.Flight_Date_Want, //航班日期
                           FlightNoDate = ctmp == null ? " " : ctmp.AirTime,
                           //FltNoDate = n.Flight_No + "/" + Convert.ToDateTime(n.Flight_Date_Want).ToString("ddMMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 5),//航班号/航班日期-日月英文                
                           Dest = n.End_Port,//目的港          
                           //AWBNO = n.MBL == null ? "" : n.MBL.Substring(0, 3) + "-" + n.MBL.Substring(3, 4) + " " + n.MBL.Substring(7),//总单号
                           AWBNO = n.MBL,
                           //Flight_No = n.Flight_No,
                           //Flight_Date_Want = Convert.ToDateTime(n.Flight_Date_Want).ToString("ddMMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 5),//航班日期
                           GWT = n.Weight_TS,//托书毛重
                           VOL = n.Volume_TS,//托书体积
                           Q7 = "",
                           Q6 = "",
                           X8 = "",
                           A2 = "",
                           AKE = "",
                           PEB = "",
                           BULK = "",
                           BUremark = "",
                           ConxFlts = "",
                           PCS = n.Pieces_TS,//托书件数
                           Dimension = n.Remark,//备注
                           ADHOC = atmp == null ? atmp.UserName : atmp.UserNameDesc,
                       }).OrderBy(x => x.Flight_Date_Want).ThenBy(x => x.FlightNoDate).ThenBy(x => x.Dest).ThenBy(x => x.AWBNO);

            var rowsresult = rows.Select(n => new
            {
                FltNoDate = n.Flight_No + "/" + (n.Flight_Date_Want == null ? "" : Convert.ToDateTime(n.Flight_Date_Want).ToString("ddMMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 5).ToUpper()),//航班号/航班日期-日月英文 
                n.Dest,
                Prefix = n.AWBNO == null ? "" : n.AWBNO.Substring(0, 3),
                AWBNO = n.AWBNO == null ? "" : n.AWBNO.Substring(3),//总单号
                n.GWT,
                n.VOL,
                n.Q7,
                n.Q6,
                n.X8,
                n.A2,
                n.AKE,
                n.PEB,
                n.BULK,
                n.BUremark,
                n.ConxFlts,
                n.PCS,
                n.Dimension,
                n.ADHOC
            }).ToList();

            List<Book_Fligt> Book_FligtList = new List<Book_Fligt>();
            foreach (var item in rowsresult)
            {
                Book_Fligt t = new Book_Fligt();
                t.FltNoDate = item.FltNoDate;
                t.Dest = item.Dest;
                t.Prefix = item.Prefix;
                t.AWBNO = item.AWBNO;
                t.GWT = item.GWT;
                t.VOL = item.VOL;
                t.Q7 = item.Q7;
                t.Q6 = item.Q6;
                t.X8 = item.X8;
                t.A2 = item.A2;
                t.AKE = item.AKE;
                t.PEB = item.PEB;
                t.BULK = item.BULK;
                t.BUremark = item.BUremark;
                t.ConxFlts = item.ConxFlts;
                t.PCS = item.PCS;
                t.Dimension = item.Dimension;
                t.ADHOC = item.ADHOC;
                Book_FligtList.AddRange(t);
            }
            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(Book_FligtList.ToList(), @"\FileModel\Book_Fligt.xlsx", 1, 0, out outfile, "订舱单");
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            sheet.Cells.Columns[0].Width = 11;
            sheet.Cells.Columns[1].Width = 6;
            sheet.Cells.Columns[2].Width = 6;
            sheet.Cells.Columns[3].Width = 11;
            sheet.Cells.Columns[4].Width = 8;
            sheet.Cells.Columns[5].Width = 8;
            sheet.Cells.Columns[6].Width = 6;
            sheet.Cells.Columns[7].Width = 6;
            sheet.Cells.Columns[8].Width = 6;
            sheet.Cells.Columns[9].Width = 6;
            sheet.Cells.Columns[10].Width = 6;
            sheet.Cells.Columns[11].Width = 6;
            sheet.Cells.Columns[12].Width = 6;
            sheet.Cells.Columns[13].Width = 6;
            sheet.Cells.Columns[14].Width = 6;
            sheet.Cells.Columns[15].Width = 6;
            sheet.Cells.Columns[16].Width = 12;
            sheet.Cells.Columns[17].Width = 7;
            Aspose.Cells.Style CellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
            CellStyle.Font.Size = 10;
            sheet.Cells[Book_FligtList.Count + 2, 3].Value = "TOTAL";
            sheet.Cells[Book_FligtList.Count + 2, 3].SetStyle(CellStyle);
            var SumGWT = Book_FligtList.Select(x => x.GWT).Sum();
            var SumVOL = Book_FligtList.Select(x => x.VOL).Sum();
            sheet.Cells[Book_FligtList.Count + 2, 4].Value = SumGWT;
            sheet.Cells[Book_FligtList.Count + 2, 5].Value = SumVOL;
            sheet.Cells[Book_FligtList.Count + 2, 15].Value = "TEL：";
            sheet.Cells[Book_FligtList.Count + 2, 4].SetStyle(CellStyle);
            sheet.Cells[Book_FligtList.Count + 2, 5].SetStyle(CellStyle);
            var newms = Server.MapPath(@"\DownLoad\789.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        //导出ADHOC
        public ActionResult ExportADHOC(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "", string UserCode = "")
        {
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
            if (data == null || data.Count == 0)
            {
                return null;
            }

            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();
            var rowdata = data.Select(n => new
            {
                Now_Date = DateTime.Now.ToString("yyyy-MM-dd"),
                MBL = n.MBL == null ? "" : n.MBL.Substring(3),
                Agent_Name = "KSF",
                End_Port = n.End_Port,
                Flight_Date_Want = n.Flight_Date_Want,
                Weight_DC = n.Weight_DC,
                Volume_DC = n.Volume_DC,
            }).ToList();
            List<ADHOC> ADHOCList = new List<ADHOC>();
            foreach (var item in rowdata)
            {
                ADHOC t = new ADHOC();
                t.Now_Date = item.Now_Date;
                t.MBL = item.MBL;
                t.Agent_Name = item.Agent_Name;
                t.End_Port = item.End_Port;
                t.Flight_Date_Want = Convert.ToDateTime(item.Flight_Date_Want).ToString("yyyy-MM-dd");
                t.Weight_DC = item.Weight_DC;
                t.Volume_DC = item.Volume_DC;
                t.Rate1 = "";
                t.AdhocRate = "";
                ADHOCList.AddRange(t);
            }

            string outfile = "";
            var outbytes = ExcelHelper.OutPutExcelByArr(ADHOCList.ToList(), @"\FileModel\ADHOC.xlsx", 4, 0, out outfile, "ADHOC");
            var ms = new System.IO.MemoryStream(outbytes);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ms);
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            sheet.Cells[1, 1].Value = DateTime.Now.ToString("yyyy-MM-dd");
            var QUserName = ArrAppUser.Where(x => x.UserName == UserCode).ToList();
            if (QUserName.Any())
            {
                sheet.Cells[2, 1].Value = QUserName.FirstOrDefault().UserNameDesc;
            }
            else
            {
                sheet.Cells[2, 1].Value = UserCode;
            }
            sheet.Cells.Columns[0].Width = 13;
            sheet.Cells.Columns[1].Width = 10;
            sheet.Cells.Columns[2].Width = 11;
            sheet.Cells.Columns[3].Width = 8;
            sheet.Cells.Columns[4].Width = 12;
            sheet.Cells.Columns[5].Width = 9;
            sheet.Cells.Columns[6].Width = 9;
            sheet.Cells.Columns[7].Width = 9;
            sheet.Cells.Columns[8].Width = 10;
            var newms = Server.MapPath(@"\DownLoad\789.xls");
            workbook.Save(newms);
            return File(newms, "application/vnd.ms-excel", fileName);
        }

        //导出仓单PDF
        public ActionResult ExportWarehouseReceiptPdf(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {

            try {
                var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                var strids = "";
                foreach (var item in filterRules)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id) && !x.Is_TG).ToList();
                if (!data.Any())
                {
                    return Json(new { Success = false, ErrMsg = "接单信息已删除或已退关！" });
                }
                var OP_MBL = data.Select(x => x.MBL).ToList();
                var QH = _unitOfWork.Repository<OPS_H_Order>().Queryable().ToList();
                var QM = _unitOfWork.Repository<OPS_M_Order>().Queryable().ToList();
                var QPARA_AirPort = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
                var QCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
                var QQH = QH.Where(x => OP_MBL.Contains(x.MBL)).ToList();
                var QQM = QM.Where(x => OP_MBL.Contains(x.MBL)).ToList();
                var headrows = (from a in data
                                join b in QPARA_AirPort on a.End_Port equals b.PortCode into b_tmp
                                from btmp in b_tmp.DefaultIfEmpty()
                                join c in QPARA_AirPort on a.Depart_Port equals c.PortCode into c_tmp
                                from ctmp in c_tmp.DefaultIfEmpty()
                                join d in QCusBusInfo on a.FWD_Code equals d.EnterpriseId into d_tmp
                                from dtmp in d_tmp.DefaultIfEmpty()
                                select new
                                {
                                    MBL = a.MBL == null ? "" : a.MBL.Substring(0, 3) + "-" + a.MBL.Substring(3, 4) + " " + a.MBL.Substring(7),
                                    Depart_Port = ctmp == null ? a.Depart_Port : ctmp.PortNameEng,
                                    End_Port = btmp == null ? a.End_Port : btmp.PortNameEng,
                                    Flight_No = a.Flight_No + "/" + (a.Flight_Date_Want == null ? "" : (Convert.ToDateTime(a.Flight_Date_Want).ToString("MMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 3) + " " + Convert.ToDateTime(a.Flight_Date_Want).ToString("dd,yyyy"))),
                                    FWD_Code = dtmp == null ? "" : dtmp.EngName
                                }).ToList();



                List<string> ArrPdfFile = new List<string>();
                List<string> ArrErrMsg = new List<string>();
                List<pdfCDTEST> ArrExcelFile = new List<pdfCDTEST>();

                //foreach (var row in data)
                //{
                //    var h_orderlist = QH.Where(x => x.MBLId == row.MBLId)
                //        .Select(n => new {
                //            HBL = n.HBL,
                //            Pieces_H = n.Pieces_H,
                //            Weight_H = n.Weight_H,
                //            EN_Name_H = n.EN_Name_H,
                //            End_Port = row.End_Port,
                //            Consignee_H = n.Consignee_H,
                //            Shipper_H = n.Shipper_H,
                //            Pay_Mode_H = n.Pay_Mode_H
                //        });

                //PDF固定台头
                foreach (var item in headrows)
                {
                    pdfCDTEST p = new pdfCDTEST();
                    p.MBL = item.MBL;
                    p.End_PortEng = item.End_Port;
                    p.Depart_PortEng = item.Depart_Port;
                    p.Flight_No = item.Flight_No;
                    p.FWD_Code = item.FWD_Code;

                    var rowsdata = (from n in data
                                    join c in QH.Where(x => x.Is_TG == false) on n.MBL equals c.MBL into c_tmp
                                    from ctmp in c_tmp.DefaultIfEmpty()
                                    join b in QPARA_AirPort on n.End_Port equals b.PortCode into b_tmp
                                    from btmp in b_tmp.DefaultIfEmpty()
                                    select new
                                    {
                                        HBL = ctmp == null ? null : ctmp.HBL,
                                        SLAC_SK = n.Slac_SK,
                                        Operation_Id = ctmp == null ? null : ctmp.Operation_Id,
                                        Pieces_H = ctmp == null ? null : ctmp.Pieces_H,
                                        Weight_H = ctmp == null ? null : ctmp.Weight_H,
                                        EN_Name_H = ctmp == null ? null : ctmp.EN_Name_H,
                                        End_Port = btmp == null ? n.End_Port : btmp.PortNameEng,
                                        Shipper_H = ctmp == null ? null : ctmp.Shipper_H,
                                        Consignee_H = ctmp == null ? null : ctmp.Consignee_H,
                                        Pay_Mode_H = ctmp == null ? null : ctmp.Pay_Mode_H,
                                    });

                    foreach (var a in rowsdata)
                    {
                        pdfCDTEST pp = new pdfCDTEST();
                        pp.HBL = a.HBL;
                        pp.Pieces = a.Pieces_H;
                        var Qops_en = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == a.Operation_Id && !x.Is_TG).ToList();
                        if (Qops_en.Any())
                        {
                            pp.Piecestest = Qops_en.FirstOrDefault().Slac_SK == null ? a.Pieces_H.ToString() : (a.Pieces_H + "\n" + "SLAC:" + Qops_en.FirstOrDefault().Slac_SK + "CTNS");
                        }
                        else
                            continue;
                        pp.Weight = a.Weight_H;
                        pp.EN_Name_H = a.EN_Name_H;
                        pp.End_Port = a.End_Port;
                        pp.Shipper_H = a.Shipper_H;
                        pp.Consignee_H = a.Consignee_H;
                        pp.Pay_Mode_H = a.Pay_Mode_H;

                        ArrExcelFile.AddRange(pp);
                    }



                    //var Ret = WordHelper.SetWordModel_BookMarkByT<pdfCD>(p, Server.MapPath("/FileModel/PDFWarehouseList.docx"), Aspose.Words.SaveFormat.Doc);
                    var Ret = WordHelper.SetWordModel_BookMarkByT<pdfCDTEST>(p, Server.MapPath("/FileModel/PDFWarehouseList.docx"), ArrExcelFile, 0, "", Aspose.Words.SaveFormat.Doc);
                    if (Ret.retResult)
                    {
                        ArrPdfFile.Add(Ret.MsgStr);

                    }
                    else
                    {
                        ArrErrMsg.Add(Ret.MsgStr);
                    }

                }

                //PDF列表部分
                //var rowsdata = (from n in data
                //                join c in QH.Where(x => x.Is_TG == false) on n.MBL equals c.MBL into c_tmp
                //                from ctmp in c_tmp.DefaultIfEmpty()
                //                join b in QPARA_AirPort on n.End_Port equals b.PortCode into b_tmp
                //                from btmp in b_tmp.DefaultIfEmpty()
                //                select new
                //                {
                //                    HBL = ctmp == null ? null : ctmp.HBL,
                //                    Pieces_H = ctmp == null ? null : ctmp.Pieces_H,
                //                    Weight_H = ctmp == null ? null : ctmp.Weight_H,
                //                    EN_Name_H = ctmp == null ? null : ctmp.EN_Name_H,
                //                    End_Port = btmp == null ? n.End_Port : btmp.PortNameEng,
                //                    Shipper_H = ctmp == null ? null : ctmp.Shipper_H,
                //                    Consignee_H = ctmp == null ? null : ctmp.Consignee_H,
                //                    Pay_Mode_H = ctmp == null ? null : ctmp.Pay_Mode_H,
                //                }).ToList();
                //}



                Aspose.Words.Document newDoc = null;
                for (var i = 0; i < ArrPdfFile.Count; i++)
                {
                    var item = ArrPdfFile[i];
                    if (i == 0)
                    {
                        newDoc = new Aspose.Words.Document(item);
                        RemovePageBreaks(newDoc);
                    }
                    else
                    {
                        var doc = new Aspose.Words.Document(item);
                        newDoc.AppendDocument(doc, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
                        RemovePageBreaks(newDoc);
                    }
                }

                var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                string FName = Server.MapPath(dPath + "/pdfCD/" + fileName);
                newDoc.Save(FName, Aspose.Words.SaveFormat.Pdf);

                System.IO.FileInfo oo = new System.IO.FileInfo(FName);
                if (typeName == "打印仓单")
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                }
                else
                {
                    return File(FName, "application/octet-stream;", oo.Name);
                }
                //return null;
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }        
        }

        //导出进仓通知书
        public ActionResult ExportEnterWHInform(Array filterRules = null, string typeName = "")
        {
            try
            {
                var strids = "";
                foreach (var item in filterRules)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
                var ArrOps_M_ID = data.Select(x => x.MBLId);
                var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
                if (data == null || data.Count == 0)
                {
                    return null;
                }

                //缓存数据
                var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);

                List<ExcelTS> ArrExcelFile = new List<ExcelTS>();

                foreach (var item in data)
                {
                    ExcelTS T = new ExcelTS();
                    var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Entrustment_Name);
                    if (QArrCusBusInfo.Any())
                        T.Entrustment_Name = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;
                    else
                        T.Entrustment_Name = item.Entrustment_Name;
                    T.DZbh = item.Operation_Id;
                    T.End_PortEn = item.End_Port;
                    T.Pieces_TS = item.Pieces_TS.ToString();
                    T.Flight_Date_Want = Convert.ToDateTime(item.Flight_Date_Want).ToString("yyyy-MM-dd");
                    ArrExcelFile.AddRange(T);

                }
                #region 获取Excel中字段位置

                Dictionary<string, Tuple<int, int>> dict = new Dictionary<string, Tuple<int, int>>() { 
             {"Entrustment_Name",new Tuple<int,int>(7,2)},
             {"DZbh",new Tuple<int,int>(8,2)},
             {"End_PortEn",new Tuple<int,int>(8,7)},
             {"Pieces_TS",new Tuple<int,int>(8,10)},
             {"Flight_Date_Want",new Tuple<int,int>(7,7)},          
            };

                #endregion


                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(Server.MapPath("/FileModel/普货地图.xls"));
                for (int i = 0; i < ArrExcelFile.Count(); i++)
                {
                    if (i > 0)
                    {
                        //var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                        //Aspose.Cells.Worksheet Owsheet = new Aspose.Cells.Worksheet();
                        //Owsheet.Copy(sheet);
                        workbook.Worksheets.AddCopy(0);

                    }
                    var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[workbook.Worksheets.Count - 1];
                    sheet.Name = "进仓通知书" + (i + 1);
                    var objValue = ArrExcelFile[i];

                    #region 插入数据
                    var DZbh = dict.Where(x => x.Key == "DZbh");
                    if (DZbh.Any())
                    {
                        var Tobj = DZbh.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.DZbh;
                    }
                    var Entrustment_Name = dict.Where(x => x.Key == "Entrustment_Name");
                    if (Entrustment_Name.Any())
                    {
                        var Tobj = Entrustment_Name.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Entrustment_Name;
                    }
                    var End_PortEn = dict.Where(x => x.Key == "End_PortEn");
                    if (End_PortEn.Any())
                    {
                        var Tobj = End_PortEn.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_PortEn;
                    }
                    var Pieces_TS = dict.Where(x => x.Key == "Pieces_TS");
                    if (Pieces_TS.Any())
                    {
                        var Tobj = Pieces_TS.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Pieces_TS;
                    }
                    var Flight_Date_Want = dict.Where(x => x.Key == "Flight_Date_Want");
                    if (Flight_Date_Want.Any())
                    {
                        var Tobj = Flight_Date_Want.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_Date_Want;
                    }

                    #endregion
                }

                if (typeName == "进仓通知书")
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    string FName = Server.MapPath(dPath + "/ExlDD/" + fileName);
                    workbook.Save(FName);

                    //System.IO.FileInfo oo = new System.IO.FileInfo(FName);
                    //application/pdf;application/octet-stream; 
                    return File(FName, "application/octet-stream", fileName);
                }
                else
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    string FName = Server.MapPath(dPath + "/ExlDD/" + fileName);
                    workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
                    //return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                    return File(FName, "application/pdf", fileName);
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }

        }

        //导出托书
        public ActionResult ExportTS(Array filterRules = null, string typeName = "")
        {
            try
            {
                var strids = "";
                foreach (var item in filterRules)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).ToList();
                var ArrOps_M_ID = data.Select(x => x.MBLId);
                var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
                if (data == null || data.Count == 0)
                {
                    return null;
                }

                //缓存数据
                var ArrPARA_AirPort = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
                var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();
                List<ExcelTS> ArrExcelFile = new List<ExcelTS>();

                foreach (var item in data)
                {
                    ExcelTS T = new ExcelTS();
                    T.DZbh = item.Operation_Id;
                    T.End_PortEn = item.End_Port;
                    T.Depart_PortEn = item.Depart_Port;
                    var QArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == item.End_Port);
                    if (QArrPARA_AirPort.Any())
                    {
                        var QPARA_AirPort = QArrPARA_AirPort.FirstOrDefault();
                        T.End_Port = QPARA_AirPort.PortName == null ? QPARA_AirPort.PortNameEng : QPARA_AirPort.PortName;
                    }
                    else
                        T.End_Port = item.End_Port;//目的港
                    var QQArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == item.Depart_Port);
                    if (QQArrPARA_AirPort.Any())
                    {
                        var QPARA_AirPort = QQArrPARA_AirPort.FirstOrDefault();
                        T.Depart_Port = QPARA_AirPort.PortName == null ? QPARA_AirPort.PortNameEng : QPARA_AirPort.PortName;
                    }
                    else
                        T.Depart_Port = item.Depart_Port;//起运港
                    T.Pieces_TS = item.Pieces_TS.ToString();
                    T.Weight_TS = item.Weight_TS.ToString();
                    T.Volume_TS = item.Volume_TS.ToString();
                    T.Flight_Date_Want = item.Flight_Date_Want == null ? "" : Convert.ToDateTime(item.Flight_Date_Want).ToString("yyyy-MM-dd");
                    T.Flight_No = item.Flight_No;
                    var QArrAppUser = ArrAppUser.Where(x => x.UserName == item.ADDWHO).FirstOrDefault();
                    if (QArrAppUser != null && !string.IsNullOrWhiteSpace(QArrAppUser.Id))
                    {
                        T.ADDWHO = QArrAppUser.UserNameDesc;
                    }
                    else
                        T.ADDWHO = item.ADDWHO;
                    T.PrintDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                    T.Consignee_M = item.Consignee_M == null ? "" : item.Consignee_M.ToString().Replace("\\r\\n", "\n");
                    T.Shipper_M = item.Shipper_M == null ? "" : item.Shipper_M.ToString().Replace("\\r\\n", "\n");
                    T.Notify_Part_M = item.Notify_Part_M == null ? "" : item.Notify_Part_M.ToString().Replace("\\r\\n", "\n");
                    var QOPS_H_Order = _oPS_H_OrderService.Queryable().Where(x => x.Operation_Id == item.Operation_Id);
                    var QQOPS_H_Order = QOPS_H_Order.Where(x => x.Id == QOPS_H_Order.Min(y => y.Id)).FirstOrDefault();
                    if (QQOPS_H_Order != null)
                    {
                        if (!string.IsNullOrWhiteSpace(QQOPS_H_Order.Marks_H) && QQOPS_H_Order.Marks_H.Contains("\n"))
                        {
                            T.Marks_H = QQOPS_H_Order.Marks_H.Split("\n")[0];
                        }
                        else
                        {
                            T.Marks_H = QQOPS_H_Order.Marks_H;
                        }
                        if (!string.IsNullOrWhiteSpace(QQOPS_H_Order.EN_Name_H) && QQOPS_H_Order.EN_Name_H.Contains("\n"))
                        {
                            T.EN_Name_H = QQOPS_H_Order.EN_Name_H.Split("\n")[0];
                        }
                        else
                        {
                            T.EN_Name_H = QQOPS_H_Order.EN_Name_H;
                        }
                    }
                    else
                    {
                        T.Marks_H = "";
                        T.EN_Name_H = "";
                    }




                    ArrExcelFile.AddRange(T);

                }
                #region 获取Excel中字段位置

                Dictionary<string, Tuple<int, int>> dict = new Dictionary<string, Tuple<int, int>>() { 
             {"DZbh",new Tuple<int,int>(5,8)},
             {"End_PortEn",new Tuple<int,int>(8,8)},
             {"Depart_PortEn",new Tuple<int,int>(8,3)},
             {"End_Port",new Tuple<int,int>(7,6)},
             {"Depart_Port",new Tuple<int,int>(7,2)},
             {"Pieces_TS",new Tuple<int,int>(12,3)},
             {"Weight_TS",new Tuple<int,int>(12,8)},
             {"Volume_TS",new Tuple<int,int>(12,9)},
             {"ADDWHO",new Tuple<int,int>(41,6)},
             {"PrintDate",new Tuple<int,int>(44,9)},
             {"Flight_Date_Want",new Tuple<int,int>(23,7)},
             {"Flight_No",new Tuple<int,int>(24,7)},
             {"Shipper_M",new Tuple<int,int>(24,0)},
             {"Consignee_M",new Tuple<int,int>(31,0)},
             {"Notify_Part_M",new Tuple<int,int>(38,0)},
             {"Marks_H",new Tuple<int,int>(12,0)},
             {"EN_Name_H",new Tuple<int,int>(12,4)},
            
            };

                #endregion


                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(Server.MapPath("/FileModel/托书.xls"));
                for (int i = 0; i < ArrExcelFile.Count(); i++)
                {
                    if (i > 0)
                    {
                        //var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                        //Aspose.Cells.Worksheet Owsheet = new Aspose.Cells.Worksheet();
                        //Owsheet.Copy(sheet);
                        workbook.Worksheets.AddCopy(0);

                    }
                    var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[workbook.Worksheets.Count - 1];
                    sheet.Name = "托书" + (i + 1);
                    var objValue = ArrExcelFile[i];

                    #region 插入数据
                    var DZbh = dict.Where(x => x.Key == "DZbh");
                    if (DZbh.Any())
                    {
                        var Tobj = DZbh.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.DZbh;
                    }
                    var End_PortEn = dict.Where(x => x.Key == "End_PortEn");
                    if (End_PortEn.Any())
                    {
                        var Tobj = End_PortEn.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_PortEn;
                    }
                    var Depart_PortEn = dict.Where(x => x.Key == "Depart_PortEn");
                    if (Depart_PortEn.Any())
                    {
                        var Tobj = Depart_PortEn.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Depart_PortEn;
                    }
                    var End_Port = dict.Where(x => x.Key == "End_Port");
                    if (End_Port.Any())
                    {
                        var Tobj = End_Port.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_Port;
                    }
                    var Depart_Port = dict.Where(x => x.Key == "Depart_Port");
                    if (Depart_Port.Any())
                    {
                        var Tobj = Depart_Port.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Depart_Port;
                    }
                    var Pieces_TS = dict.Where(x => x.Key == "Pieces_TS");
                    if (Pieces_TS.Any())
                    {
                        var Tobj = Pieces_TS.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Pieces_TS;
                    }
                    var Weight_TS = dict.Where(x => x.Key == "Weight_TS");
                    if (Weight_TS.Any())
                    {
                        var Tobj = Weight_TS.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Weight_TS;
                    }
                    var Volume_TS = dict.Where(x => x.Key == "Volume_TS");
                    if (Volume_TS.Any())
                    {
                        var Tobj = Volume_TS.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Volume_TS;
                    }
                    var ADDWHO = dict.Where(x => x.Key == "ADDWHO");
                    if (ADDWHO.Any())
                    {
                        var Tobj = ADDWHO.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.ADDWHO;
                    }
                    var PrintDate = dict.Where(x => x.Key == "PrintDate");
                    if (End_Port.Any())
                    {
                        var Tobj = PrintDate.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.PrintDate;
                    }
                    var Flight_Date_Want = dict.Where(x => x.Key == "Flight_Date_Want");
                    if (Flight_Date_Want.Any())
                    {
                        var Tobj = Flight_Date_Want.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_Date_Want;
                    }
                    var Flight_No = dict.Where(x => x.Key == "Flight_No");
                    if (Flight_No.Any())
                    {
                        var Tobj = Flight_No.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No;
                    }
                    var Consignee_M = dict.Where(x => x.Key == "Consignee_M");
                    if (Consignee_M.Any())
                    {
                        var Tobj = Consignee_M.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Consignee_M;
                    }
                    var Shipper_M = dict.Where(x => x.Key == "Shipper_M");
                    if (Shipper_M.Any())
                    {
                        var Tobj = Shipper_M.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Shipper_M;
                    }
                    var Notify_Part_M = dict.Where(x => x.Key == "Notify_Part_M");
                    if (Notify_Part_M.Any())
                    {
                        var Tobj = Notify_Part_M.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Notify_Part_M;
                    }
                    var Marks_H = dict.Where(x => x.Key == "Marks_H");
                    if (Marks_H.Any())
                    {
                        var Tobj = Marks_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Marks_H;
                    }
                    var EN_Name_H = dict.Where(x => x.Key == "EN_Name_H");
                    if (EN_Name_H.Any())
                    {
                        var Tobj = EN_Name_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.EN_Name_H;
                    }
                    #endregion
                }
                if (typeName == "托书")
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    string FName = Server.MapPath(dPath + "/ExlDD/" + fileName);
                    workbook.Save(FName);
                    //System.IO.FileInfo oo = new System.IO.FileInfo(FName);
                    //application/pdf;application/octet-stream; 
                    return File(FName, "application/octet-stream", fileName);
                }
                else
                {
                    var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                    string FName = Server.MapPath(dPath + "/ExlDD/" + fileName);
                    workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
                    //return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                    return File(FName, "application/pdf", fileName);
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }

        }

        //导出分单Excel
        public ActionResult ExportOP_H(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).OrderBy(sort, order).ToList();
            var idArrUpdate = data.Select(x => x.Operation_Id).ToList();
            var QOPS_H_OrderRep = _unitOfWork.Repository<OPS_H_Order>().Queryable();
            var ArrOps_M_ID = data.Select(x => x.MBLId);
            var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
            if (data == null || data.Count == 0)
            {
                return null;
            }

            var QPARA_AirPortRep = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            //var ArrOPS_H_Order = QOPS_H_OrderRep.Where(x => idArrUpdate.Contains(x.Operation_Id)).OrderBy(sort, order).ToList();
            var ops_En = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable();
            List<ExcelOP_H> ArrPdfFile = new List<ExcelOP_H>();
            var ArrOPS_H_Order = (from n in data
                                  join h in QOPS_H_OrderRep on n.Operation_Id equals h.Operation_Id into h_tmp
                                  from htmp in h_tmp.DefaultIfEmpty()
                                  select new
                                  {
                                      htmp.Operation_Id,
                                      htmp.MBL,
                                      htmp.HBL,
                                      htmp.Shipper_H,
                                      htmp.Notify_Part_H,
                                      htmp.Consignee_H,
                                      htmp.Carriage_H,
                                      htmp.Bragainon_Article_H,
                                      htmp.Is_Self,
                                      htmp.Declare_Value_Ciq_H,
                                      htmp.Declare_Value_Trans_H,
                                      htmp.Pieces_H,
                                      htmp.Weight_H,
                                      htmp.Volume_H,
                                      htmp.Charge_Weight_H,
                                      htmp.Marks_H,
                                      htmp.EN_Name_H,
                                  }).ToList();
            foreach (var item in ArrOPS_H_Order)
            {
                ExcelOP_H o = new ExcelOP_H();
                o.MBL = item.MBL == null ? "" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7);   //总单号
                o.HBL = item.HBL;//分单号
                o.Shipper_H = item.Shipper_H == null ? item.Shipper_H : item.Shipper_H.ToString().Replace("\\r\\n", "\n");//分单发货人
                o.Consignee_H = item.Consignee_H == null ? item.Consignee_H : item.Consignee_H.ToString().Replace("\\r\\n", "\n");//分单收货人
                o.Notify_Part_H = item.Notify_Part_H == null ? item.Notify_Part_H : item.Notify_Part_H.ToString().Replace("\\r\\n", "\n");//分单通知人
                var Qops_En = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Operation_Id).FirstOrDefault();
                var QDepart_Port = QPARA_AirPortRep.Where(x => x.PortCode == Qops_En.Depart_Port).FirstOrDefault();
                if (QDepart_Port != null)
                {
                    o.Depart_PortEng = QDepart_Port.PortNameEng; //起运港英文全称
                }
                else
                    o.Depart_PortEng = Qops_En.Depart_Port;
                var QEnd_Port = QPARA_AirPortRep.Where(x => x.PortCode == Qops_En.End_Port).FirstOrDefault();
                if (QEnd_Port != null)
                {
                    o.End_PortEng = QEnd_Port.PortNameEng;   //目的港全称
                }
                else
                    o.End_PortEng = Qops_En.End_Port;
                o.End_Port = Qops_En.End_Port;      //目的港三字代码
                o.Flight_No = Qops_En.Flight_No;//航班号
                o.Flight_No1 = Qops_En.Flight_No == null ? "" : Qops_En.Flight_No.Substring(0, 2); //航班号前两位
                o.Flight_Date_Want = Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("yyyy-MM-dd"); //航班日期
                o.Flight_No_Flight_Date_Want = Qops_En.Flight_No + " " + ((item.Is_Self == false ? (Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("dd,yyyy")) : (Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("MMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 3)).ToUpper() + " " + Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("dd,yyyy")));
                o.Carriage_H_Bragainon_Article_H = (item.Carriage_H == null ? "" : (item.Carriage_H == "C" ? "FREIGHT COLLECT" : "FREIGHT PREPAID")) + " " + item.Bragainon_Article_H; //运费p/c + 成交条款
                o.Carriage_H = item.Carriage_H;
                o.Flight_No_Flight_Depart_PortEng = (Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("MMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 3).ToUpper() + " " + Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("dd,yyyy")) + " " + o.Depart_PortEng + " FEILI";//航班日期 + 起运港英文全称
                o.Shipper_M = Qops_En.Consignee_M == null ? Qops_En.Consignee_M : Qops_En.Consignee_M.ToString().Replace("\\r\\n", "\n"); //总单收货人
                o.Declare_Value_Ciq_H = item.Declare_Value_Ciq_H;   //申明价值海关
                o.Declare_Value_Trans_H = item.Declare_Value_Trans_H;  //申明价值运输
                o.Pieces_H = item.Pieces_H.ToString() + "\n\n" + (Qops_En.Slac_SK == null ? "" : "SLAC:" + Qops_En.Slac_SK);//分单件数
                o.Weight_H = item.Weight_H.ToString();//分单毛重
                o.Volume_H = item.Volume_H == null ? "" : "VOL:" + item.Volume_H.ToString() + " CBM";//分单体积
                o.Charge_Weight_H = item.Charge_Weight_H.ToString(); //分单计费重量
                o.Marks_H = item.Marks_H == null ? item.Marks_H : item.Marks_H.ToString().Replace("\\r\\n", "\n"); //唛头
                o.EN_Name_H = item.EN_Name_H == null ? item.EN_Name_H : item.EN_Name_H.ToString().Replace("\\r\\n", "\n"); //英文货名


                ArrPdfFile.Add(o);
            }

            #region 获取Excel中字段位置

            Dictionary<string, Tuple<int, int>> dict = new Dictionary<string, Tuple<int, int>>() { 
             {"MBL",new Tuple<int,int>(1,18)},
             {"HBL",new Tuple<int,int>(1,28)}, 
             {"Shipper_H",new Tuple<int,int>(2,1)},
             {"Consignee_H",new Tuple<int,int>(8,1)},
             {"Notify_Part_H",new Tuple<int,int>(15,1)},
             {"Depart_PortEng",new Tuple<int,int>(20,1)},
             {"End_Port",new Tuple<int,int>(22,1)},
             {"End_PortEng",new Tuple<int,int>(24,1)},
             {"Flight_No1",new Tuple<int,int>(22,4)},
             {"Flight_No_Flight_Date_Want",new Tuple<int,int>(24,9)},
             {"Carriage_H_Bragainon_Article_H",new Tuple<int,int>(15,15)},
             {"Declare_Value_Ciq_H",new Tuple<int,int>(22,28)},
             {"Declare_Value_Trans_H",new Tuple<int,int>(22,26)},
             {"Pieces_H",new Tuple<int,int>(30,1)},
             {"Weight_H",new Tuple<int,int>(30,3)},
             {"Charge_Weight_H",new Tuple<int,int>(30,10)},
             {"Marks_H",new Tuple<int,int>(33,8)},
             {"EN_Name_H",new Tuple<int,int>(30,17)},
             {"Volume_H",new Tuple<int,int>(44,12)},
             {"Flight_No_Flight_Depart_PortEng",new Tuple<int,int>(51,12)},
             {"Shipper_M",new Tuple<int,int>(26,1)},
             {"PP1",new Tuple<int,int>(22,20)},
             {"PP2",new Tuple<int,int>(22,22)},
             {"CC1",new Tuple<int,int>(22,21)},
             {"CC2",new Tuple<int,int>(22,23)},
             {"PPASAR1",new Tuple<int,int>(42,1)},
             {"PPASAR2",new Tuple<int,int>(50,1)},
             {"PPASAR3",new Tuple<int,int>(52,1)},
             {"CCASAR1",new Tuple<int,int>(42,7)},
             {"CCASAR2",new Tuple<int,int>(50,7)},
             {"CCASAR3",new Tuple<int,int>(52,7)},

             
             


            };

            #endregion
            //Aspose.Cells.Workbook workbook1 = new Aspose.Cells.Workbook();
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(Server.MapPath("/FileModel/分单Excel.xls"));
            //var sheet1 = workbook.Worksheets.Add("123");
            //sheet1 = workbook1.Worksheets[0];
            List<object> Arr = new List<object>();
            Arr.AddRange();
            for (int i = 0; i < ArrPdfFile.Count(); i++)
            {
                if (i > 0)
                {
                    //var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                    //Aspose.Cells.Worksheet Owsheet = new Aspose.Cells.Worksheet();
                    //Owsheet.Copy(sheet);
                    workbook.Worksheets.AddCopy(0);

                }
                var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[workbook.Worksheets.Count - 1];
                sheet.Name = "分单" + (i + 1);
                var objValue = ArrPdfFile[i];

                #region 插入数据

                var MBL = dict.Where(x => x.Key == "MBL");
                if (MBL.Any())
                {
                    var Tobj = MBL.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.MBL;
                }
                var HBL = dict.Where(x => x.Key == "HBL");
                if (HBL.Any())
                {
                    var Tobj = HBL.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.HBL;
                }
                var Shipper_H = dict.Where(x => x.Key == "Shipper_H");
                if (Shipper_H.Any())
                {
                    var Tobj = Shipper_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Shipper_H;
                }
                var Consignee_H = dict.Where(x => x.Key == "Consignee_H");
                if (Consignee_H.Any())
                {
                    var Tobj = Consignee_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Consignee_H;
                }
                var Notify_Part_H = dict.Where(x => x.Key == "Notify_Part_H");
                if (Notify_Part_H.Any())
                {
                    var Tobj = Notify_Part_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Notify_Part_H;
                }
                var Depart_PortEng = dict.Where(x => x.Key == "Depart_PortEng");
                if (Depart_PortEng.Any())
                {
                    var Tobj = Depart_PortEng.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Depart_PortEng;
                }
                var End_Port = dict.Where(x => x.Key == "End_Port");
                if (End_Port.Any())
                {
                    var Tobj = End_Port.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_Port;
                }
                var End_PortEng = dict.Where(x => x.Key == "End_PortEng");
                if (End_PortEng.Any())
                {
                    var Tobj = End_PortEng.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_PortEng;
                }
                var Flight_No1 = dict.Where(x => x.Key == "Flight_No1");
                if (Flight_No1.Any())
                {
                    var Tobj = Flight_No1.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No1;
                }
                var Flight_No_Flight_Date_Want = dict.Where(x => x.Key == "Flight_No_Flight_Date_Want");
                if (Flight_No_Flight_Date_Want.Any())
                {
                    var Tobj = Flight_No_Flight_Date_Want.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No_Flight_Date_Want;
                }
                var Carriage_H_Bragainon_Article_H = dict.Where(x => x.Key == "Carriage_H_Bragainon_Article_H");
                if (Carriage_H_Bragainon_Article_H.Any())
                {
                    var Tobj = Carriage_H_Bragainon_Article_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H_Bragainon_Article_H;
                }
                var Declare_Value_Ciq_H = dict.Where(x => x.Key == "Declare_Value_Ciq_H");
                if (Declare_Value_Ciq_H.Any())
                {
                    var Tobj = Declare_Value_Ciq_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Declare_Value_Ciq_H;
                }
                var Declare_Value_Trans_H = dict.Where(x => x.Key == "Declare_Value_Trans_H");
                if (Declare_Value_Trans_H.Any())
                {
                    var Tobj = Declare_Value_Trans_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Declare_Value_Trans_H;
                }
                var Pieces_H = dict.Where(x => x.Key == "Pieces_H");
                if (Pieces_H.Any())
                {
                    var Tobj = Pieces_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Pieces_H;
                }
                var Weight_H = dict.Where(x => x.Key == "Weight_H");
                if (Weight_H.Any())
                {
                    var Tobj = Weight_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Weight_H;
                }
                var Charge_Weight_H = dict.Where(x => x.Key == "Charge_Weight_H");
                if (Charge_Weight_H.Any())
                {
                    var Tobj = Charge_Weight_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Charge_Weight_H;
                }
                var Marks_H = dict.Where(x => x.Key == "Marks_H");
                if (Marks_H.Any())
                {
                    var Tobj = Marks_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Marks_H;
                }
                var EN_Name_H = dict.Where(x => x.Key == "EN_Name_H");
                if (EN_Name_H.Any())
                {
                    var Tobj = EN_Name_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.EN_Name_H;
                }
                var Flight_No_Flight_Depart_PortEng = dict.Where(x => x.Key == "Flight_No_Flight_Depart_PortEng");
                if (Flight_No_Flight_Depart_PortEng.Any())
                {
                    var Tobj = Flight_No_Flight_Depart_PortEng.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No_Flight_Depart_PortEng;
                }
                var Volume_H = dict.Where(x => x.Key == "Volume_H");
                if (Volume_H.Any())
                {
                    var Tobj = Volume_H.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Volume_H;
                }
                var Shipper_M = dict.Where(x => x.Key == "Shipper_M");
                if (Shipper_M.Any())
                {
                    var Tobj = Shipper_M.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Shipper_M;
                }
                var PP1 = dict.Where(x => x.Key == "PP1");
                if (PP1.Any())
                {
                    var Tobj = PP1.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "X" : "";
                }
                var PP2 = dict.Where(x => x.Key == "PP2");
                if (PP1.Any())
                {
                    var Tobj = PP2.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "X" : "";
                }
                var CC1 = dict.Where(x => x.Key == "CC1");
                if (PP1.Any())
                {
                    var Tobj = CC1.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "X" : "";
                }
                var CC2 = dict.Where(x => x.Key == "CC2");
                if (PP1.Any())
                {
                    var Tobj = CC2.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "X" : "";
                }
                var PPASAR1 = dict.Where(x => x.Key == "PPASAR1");
                if (PPASAR1.Any())
                {
                    var Tobj = PPASAR1.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "AS ARRANGED" : "";
                }
                var PPASAR2 = dict.Where(x => x.Key == "PPASAR2");
                if (PPASAR2.Any())
                {
                    var Tobj = PPASAR2.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "AS ARRANGED" : "";
                }
                var PPASAR3 = dict.Where(x => x.Key == "PPASAR3");
                if (PPASAR3.Any())
                {
                    var Tobj = PPASAR3.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "AS ARRANGED" : "";
                }
                var CCASAR1 = dict.Where(x => x.Key == "CCASAR1");
                if (CCASAR1.Any())
                {
                    var Tobj = CCASAR1.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "AS ARRANGED" : "";
                }
                var CCASAR2 = dict.Where(x => x.Key == "CCASAR2");
                if (PPASAR2.Any())
                {
                    var Tobj = CCASAR2.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "AS ARRANGED" : "";
                }
                var CCASAR3 = dict.Where(x => x.Key == "CCASAR3");
                if (CCASAR3.Any())
                {
                    var Tobj = CCASAR3.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "AS ARRANGED" : "";
                }

                #endregion
            }
            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
            string FName = Server.MapPath(dPath + "/ExlDD/" + fileName);
            workbook.Save(FName);
            //System.IO.FileInfo oo = new System.IO.FileInfo(FName);
            //application/pdf;application/octet-stream; 
            return File(FName, "application/octet-stream", fileName);
        }

        //导出分单PDF
        //public ActionResult ExportOP_HPDF(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        //{
        //    try
        //    {
        //        var strids = "";
        //        foreach (var item in filterRules)
        //        {
        //            strids = item.ToString();
        //        }
        //        var idarr = strids.Split(",");
        //        var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).OrderBy(sort, order).ToList();
        //        var QOPS_H_OrderRep = _unitOfWork.Repository<OPS_H_Order>().Queryable();
        //        var ArrOps_M_ID = data.Select(x => x.MBLId);
        //        var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
        //        if (data == null || data.Count == 0)
        //        {
        //            return null;
        //        }

        //        var QPARA_AirPortRep = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
        //        //var ArrOPS_H_Order = QOPS_H_OrderRep.Where(x => idarr.Contains(x.Operation_Id)).ToList().ToList();
        //        var ArrOPS_H_Order = (from n in data
        //                              join h in QOPS_H_OrderRep on n.Operation_Id equals h.Operation_Id into h_tmp
        //                              from htmp in h_tmp.DefaultIfEmpty()
        //                              select new
        //                              {
        //                                  htmp.Operation_Id,
        //                                  htmp.MBL,
        //                                  htmp.HBL,
        //                                  htmp.Shipper_H,
        //                                  htmp.Notify_Part_H,
        //                                  htmp.Consignee_H,
        //                                  htmp.Carriage_H,
        //                                  htmp.Bragainon_Article_H,
        //                                  htmp.Is_Self,
        //                                  htmp.Declare_Value_Ciq_H,
        //                                  htmp.Declare_Value_Trans_H,
        //                                  htmp.Pieces_H,
        //                                  htmp.Weight_H,
        //                                  htmp.Volume_H,
        //                                  htmp.Charge_Weight_H,
        //                                  htmp.Marks_H,
        //                                  htmp.EN_Name_H,
        //                              }).ToList();
        //        var ops_En = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable();
        //        List<ExcelOP_H> ArrPdfFile = new List<ExcelOP_H>();

        //        foreach (var item in ArrOPS_H_Order)
        //        {
        //            ExcelOP_H o = new ExcelOP_H();
        //            o.MBL = item.MBL == null ? "" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7);   //总单号
        //            o.HBL = item.HBL;//分单号
        //            o.Shipper_H = item.Shipper_H == null ? item.Shipper_H : (item.Shipper_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", "");//分单发货人
        //            o.Consignee_H = item.Consignee_H == null ? item.Consignee_H : (item.Consignee_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", "");//分单收货人
        //            o.Notify_Part_H = item.Notify_Part_H == null ? item.Notify_Part_H : (item.Notify_Part_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", "");//分单通知人
        //            var Qops_En = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Operation_Id).FirstOrDefault();
        //            var QDepart_Port = QPARA_AirPortRep.Where(x => x.PortCode == Qops_En.Depart_Port).FirstOrDefault();
        //            if (QDepart_Port != null)
        //            {
        //                o.Depart_PortEng = QDepart_Port.PortNameEng; //起运港英文全称
        //            }
        //            else
        //                o.Depart_PortEng = Qops_En.Depart_Port;
        //            var QEnd_Port = QPARA_AirPortRep.Where(x => x.PortCode == Qops_En.End_Port).FirstOrDefault();
        //            if (QEnd_Port != null)
        //            {
        //                o.End_PortEng = QEnd_Port.PortNameEng;   //目的港全称
        //            }
        //            else
        //                o.End_PortEng = Qops_En.End_Port;
        //            o.End_Port = Qops_En.End_Port;      //目的港三字代码
        //            o.Flight_No = Qops_En.Flight_No;//航班号
        //            o.Flight_No1 = Qops_En.Flight_No == null ? "" : Qops_En.Flight_No.Substring(0, 2); //航班号前两位
        //            o.Flight_Date_Want = Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("yyyy-MM-dd"); //航班日期
        //            o.Flight_No_Flight_Date_Want = Qops_En.Flight_No + " " + ((item.Is_Self == false ? (Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("dd,yyyy")) : (Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("MMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 3)).ToUpper() + " " + Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("dd,yyyy")));
        //            o.Carriage_H_Bragainon_Article_H = (item.Carriage_H == null ? "" : (item.Carriage_H == "C" ? "FREIGHT COLLECT" : "FREIGHT PREPAID")) + " " + item.Bragainon_Article_H; //运费p/c + 成交条款
        //            o.Carriage_H = item.Carriage_H;
        //            o.Flight_No_Flight_Depart_PortEng = (Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("MMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 3).ToUpper() + " " + Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("dd,yyyy")) + " " + o.Depart_PortEng + " FEILI";//航班日期 + 起运港英文全称
        //            o.Shipper_M = Qops_En.Consignee_M == null ? Qops_En.Consignee_M : (Qops_En.Consignee_M.ToString().Replace("\\r\\n", "\n")).Replace("\t", ""); //总单收货人
        //            o.Declare_Value_Ciq_H = item.Declare_Value_Ciq_H;   //申明价值海关
        //            o.Declare_Value_Trans_H = item.Declare_Value_Trans_H;  //申明价值运输
        //            o.Pieces_H = item.Pieces_H.ToString() + "\n\n" + (Qops_En.Slac_SK == null ? "" : "SLAC:" + Qops_En.Slac_SK); ;//分单件数
        //            o.Weight_H = item.Weight_H.ToString();//分单毛重
        //            o.Volume_H = item.Volume_H == null ? "" : "VOL:" + item.Volume_H.ToString() + " CBM";//分单体积
        //            o.Charge_Weight_H = item.Charge_Weight_H.ToString(); //分单计费重量
        //            o.Marks_H = item.Marks_H == null ? item.Marks_H : (item.Marks_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", ""); //唛头
        //            o.EN_Name_H = item.EN_Name_H == null ? item.EN_Name_H : (item.EN_Name_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", ""); //英文货名


        //            ArrPdfFile.Add(o);
        //        }

        //        #region 获取Excel中字段位置

        //        Dictionary<string, Tuple<int, int>> dict = new Dictionary<string, Tuple<int, int>>() { 
        //     {"MBL",new Tuple<int,int>(1,22)},
        //     {"HBL",new Tuple<int,int>(1,28)}, 
        //     {"Shipper_H",new Tuple<int,int>(2,1)},
        //     {"Consignee_H",new Tuple<int,int>(8,1)},
        //     {"Notify_Part_H",new Tuple<int,int>(17,1)},
        //     {"Depart_PortEng",new Tuple<int,int>(23,1)},
        //     {"End_Port",new Tuple<int,int>(25,4)},
        //     {"End_PortEng",new Tuple<int,int>(28,1)},
        //     {"Flight_No1",new Tuple<int,int>(25,10)},
        //     {"Flight_No_Flight_Date_Want",new Tuple<int,int>(28,10)},
        //     {"Carriage_H_Bragainon_Article_H",new Tuple<int,int>(17,20)},
        //     {"Declare_Value_Ciq_H",new Tuple<int,int>(25,29)},
        //     {"Declare_Value_Trans_H",new Tuple<int,int>(25,28)},
        //     {"Pieces_H",new Tuple<int,int>(37,1)},
        //     {"Weight_H",new Tuple<int,int>(37,5)},
        //     {"Charge_Weight_H",new Tuple<int,int>(37,15)},
        //     {"Marks_H",new Tuple<int,int>(40,11)},
        //     {"EN_Name_H",new Tuple<int,int>(37,22)},
        //     {"Volume_H",new Tuple<int,int>(53,19)},
        //     {"Flight_No_Flight_Depart_PortEng",new Tuple<int,int>(62,19)},
        //     {"Shipper_M",new Tuple<int,int>(31,1)},
        //     {"PP1",new Tuple<int,int>(26,24)},
        //     {"PP2",new Tuple<int,int>(26,26)},
        //     {"CC1",new Tuple<int,int>(26,25)},
        //     {"CC2",new Tuple<int,int>(26,27)},
        //     {"PPASAR1",new Tuple<int,int>(50,1)},
        //     {"PPASAR2",new Tuple<int,int>(59,1)},
        //     {"PPASAR3",new Tuple<int,int>(62,1)},
        //     {"CCASAR1",new Tuple<int,int>(50,10)},
        //     {"CCASAR2",new Tuple<int,int>(59,10)},
        //     {"CCASAR3",new Tuple<int,int>(62,10)},





        //    };

        //        #endregion
        //        //Aspose.Cells.Workbook workbook1 = new Aspose.Cells.Workbook();
        //        Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(Server.MapPath("/FileModel/分单PDF.xls"));

        //        //var sheet1 = workbook.Worksheets.Add("123");
        //        //sheet1 = workbook1.Worksheets[0];

        //        List<object> Arr = new List<object>();
        //        Arr.AddRange();
        //        for (int i = 0; i < ArrPdfFile.Count(); i++)
        //        {
        //            if (i > 0)
        //            {
        //                //var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
        //                //Aspose.Cells.Worksheet Owsheet = new Aspose.Cells.Worksheet();
        //                //Owsheet.Copy(sheet);
        //                workbook.Worksheets.AddCopy(0);

        //            }
        //            var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[workbook.Worksheets.Count - 1];
        //            sheet.Name = "分单" + (i + 1);
        //            var objValue = ArrPdfFile[i];

        //            #region 插入数据

        //            var MBL = dict.Where(x => x.Key == "MBL");
        //            if (MBL.Any())
        //            {
        //                var Tobj = MBL.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.MBL;
        //            }
        //            var HBL = dict.Where(x => x.Key == "HBL");
        //            if (HBL.Any())
        //            {
        //                var Tobj = HBL.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.HBL;
        //            }
        //            var Shipper_H = dict.Where(x => x.Key == "Shipper_H");
        //            if (Shipper_H.Any())
        //            {
        //                var Tobj = Shipper_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Shipper_H;
        //            }
        //            var Consignee_H = dict.Where(x => x.Key == "Consignee_H");
        //            if (Consignee_H.Any())
        //            {
        //                var Tobj = Consignee_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Consignee_H;
        //            }
        //            var Notify_Part_H = dict.Where(x => x.Key == "Notify_Part_H");
        //            if (Notify_Part_H.Any())
        //            {
        //                var Tobj = Notify_Part_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Notify_Part_H;
        //            }
        //            var Depart_PortEng = dict.Where(x => x.Key == "Depart_PortEng");
        //            if (Depart_PortEng.Any())
        //            {
        //                var Tobj = Depart_PortEng.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Depart_PortEng;
        //            }
        //            var End_Port = dict.Where(x => x.Key == "End_Port");
        //            if (End_Port.Any())
        //            {
        //                var Tobj = End_Port.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_Port;
        //            }
        //            var End_PortEng = dict.Where(x => x.Key == "End_PortEng");
        //            if (End_PortEng.Any())
        //            {
        //                var Tobj = End_PortEng.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_PortEng;
        //            }
        //            var Flight_No1 = dict.Where(x => x.Key == "Flight_No1");
        //            if (Flight_No1.Any())
        //            {
        //                var Tobj = Flight_No1.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No1;
        //            }
        //            var Flight_No_Flight_Date_Want = dict.Where(x => x.Key == "Flight_No_Flight_Date_Want");
        //            if (Flight_No_Flight_Date_Want.Any())
        //            {
        //                var Tobj = Flight_No_Flight_Date_Want.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No_Flight_Date_Want;
        //            }
        //            var Carriage_H_Bragainon_Article_H = dict.Where(x => x.Key == "Carriage_H_Bragainon_Article_H");
        //            if (Carriage_H_Bragainon_Article_H.Any())
        //            {
        //                var Tobj = Carriage_H_Bragainon_Article_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H_Bragainon_Article_H;
        //            }
        //            var Declare_Value_Ciq_H = dict.Where(x => x.Key == "Declare_Value_Ciq_H");
        //            if (Declare_Value_Ciq_H.Any())
        //            {
        //                var Tobj = Declare_Value_Ciq_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Declare_Value_Ciq_H;
        //            }
        //            var Declare_Value_Trans_H = dict.Where(x => x.Key == "Declare_Value_Trans_H");
        //            if (Declare_Value_Trans_H.Any())
        //            {
        //                var Tobj = Declare_Value_Trans_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Declare_Value_Trans_H;
        //            }
        //            var Pieces_H = dict.Where(x => x.Key == "Pieces_H");
        //            if (Pieces_H.Any())
        //            {
        //                var Tobj = Pieces_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Pieces_H;
        //            }
        //            var Weight_H = dict.Where(x => x.Key == "Weight_H");
        //            if (Weight_H.Any())
        //            {
        //                var Tobj = Weight_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Weight_H;
        //            }
        //            var Charge_Weight_H = dict.Where(x => x.Key == "Charge_Weight_H");
        //            if (Charge_Weight_H.Any())
        //            {
        //                var Tobj = Charge_Weight_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Charge_Weight_H;
        //            }
        //            var Marks_H = dict.Where(x => x.Key == "Marks_H");
        //            if (Marks_H.Any())
        //            {
        //                var Tobj = Marks_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Marks_H;
        //            }
        //            var EN_Name_H = dict.Where(x => x.Key == "EN_Name_H");
        //            if (EN_Name_H.Any())
        //            {
        //                var Tobj = EN_Name_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.EN_Name_H;
        //            }
        //            var Flight_No_Flight_Depart_PortEng = dict.Where(x => x.Key == "Flight_No_Flight_Depart_PortEng");
        //            if (Flight_No_Flight_Depart_PortEng.Any())
        //            {
        //                var Tobj = Flight_No_Flight_Depart_PortEng.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No_Flight_Depart_PortEng;
        //            }
        //            var Volume_H = dict.Where(x => x.Key == "Volume_H");
        //            if (Volume_H.Any())
        //            {
        //                var Tobj = Volume_H.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Volume_H;
        //            }
        //            var Shipper_M = dict.Where(x => x.Key == "Shipper_M");
        //            if (Shipper_M.Any())
        //            {
        //                var Tobj = Shipper_M.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Shipper_M;
        //            }
        //            var PP1 = dict.Where(x => x.Key == "PP1");
        //            if (PP1.Any())
        //            {
        //                var Tobj = PP1.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "X" : "";
        //            }
        //            var PP2 = dict.Where(x => x.Key == "PP2");
        //            if (PP1.Any())
        //            {
        //                var Tobj = PP2.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "X" : "";
        //            }
        //            var CC1 = dict.Where(x => x.Key == "CC1");
        //            if (PP1.Any())
        //            {
        //                var Tobj = CC1.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "X" : "";
        //            }
        //            var CC2 = dict.Where(x => x.Key == "CC2");
        //            if (PP1.Any())
        //            {
        //                var Tobj = CC2.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "X" : "";
        //            }
        //            var PPASAR1 = dict.Where(x => x.Key == "PPASAR1");
        //            if (PPASAR1.Any())
        //            {
        //                var Tobj = PPASAR1.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "AS ARRANGED" : "";
        //            }
        //            var PPASAR2 = dict.Where(x => x.Key == "PPASAR2");
        //            if (PPASAR2.Any())
        //            {
        //                var Tobj = PPASAR2.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "AS ARRANGED" : "";
        //            }
        //            var PPASAR3 = dict.Where(x => x.Key == "PPASAR3");
        //            if (PPASAR3.Any())
        //            {
        //                var Tobj = PPASAR3.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "AS ARRANGED" : "";
        //            }
        //            var CCASAR1 = dict.Where(x => x.Key == "CCASAR1");
        //            if (CCASAR1.Any())
        //            {
        //                var Tobj = CCASAR1.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "AS ARRANGED" : "";
        //            }
        //            var CCASAR2 = dict.Where(x => x.Key == "CCASAR2");
        //            if (PPASAR2.Any())
        //            {
        //                var Tobj = CCASAR2.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "AS ARRANGED" : "";
        //            }
        //            var CCASAR3 = dict.Where(x => x.Key == "CCASAR3");
        //            if (CCASAR3.Any())
        //            {
        //                var Tobj = CCASAR3.First().Value;
        //                sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "AS ARRANGED" : "";
        //            }

        //            #endregion
        //        }
        //        var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
        //        var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
        //        string FName = Server.MapPath(dPath + "/ExlDD/" + fileName);
        //        workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
        //        //System.IO.FileInfo oo = new System.IO.FileInfo(FName);
        //        //application/pdf;application/octet-stream; 
        //        System.IO.FileInfo oo = new System.IO.FileInfo(FName);
        //        if (typeName == "打印分单")
        //        {
        //            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
        //        }
        //        else
        //        {
        //            return File(FName, "application/pdf", oo.Name);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var ErrMsg = Common.GetExceptionMsg(ex);
        //        return Json(new { Success = false, ErrMsg = ErrMsg });
        //    }

        //}

        //导出封面excel 

        //导出分单PDF
        public ActionResult ExportOP_HPDF(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            try
            {
                var strids = "";
                foreach (var item in filterRules)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");
                var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).OrderBy(sort, order).ToList();
                var QOPS_H_OrderRep = _unitOfWork.Repository<OPS_H_Order>().Queryable();
                var ArrOps_M_ID = data.Select(x => x.MBLId);
                var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
                if (data == null || data.Count == 0)
                {
                    return null;
                }

                var QPARA_AirPortRep = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
                //var ArrOPS_H_Order = QOPS_H_OrderRep.Where(x => idarr.Contains(x.Operation_Id)).ToList().ToList();
                var ArrOPS_H_Order = (from n in data
                                      join h in QOPS_H_OrderRep on n.Operation_Id equals h.Operation_Id into h_tmp
                                      from htmp in h_tmp.DefaultIfEmpty()
                                      select new
                                      {
                                          htmp.Operation_Id,
                                          htmp.MBL,
                                          htmp.HBL,
                                          htmp.Shipper_H,
                                          htmp.Notify_Part_H,
                                          htmp.Consignee_H,
                                          htmp.Carriage_H,
                                          htmp.Bragainon_Article_H,
                                          htmp.Is_Self,
                                          htmp.Declare_Value_Ciq_H,
                                          htmp.Declare_Value_Trans_H,
                                          htmp.Pieces_H,
                                          htmp.Weight_H,
                                          htmp.Volume_H,
                                          htmp.Charge_Weight_H,
                                          htmp.Marks_H,
                                          htmp.EN_Name_H,
                                      }).ToList();
                var ops_En = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable();
                List<ExcelOP_H> ArrPdfFile = new List<ExcelOP_H>();

                foreach (var item in ArrOPS_H_Order)
                {
                    ExcelOP_H o = new ExcelOP_H();
                    o.MBL = item.MBL == null ? "" : item.MBL.Substring(0, 3) + "-" + item.MBL.Substring(3, 4) + " " + item.MBL.Substring(7);   //总单号
                    o.HBL = item.HBL;//分单号
                    o.Shipper_H = item.Shipper_H == null ? item.Shipper_H : (item.Shipper_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", "");//分单发货人
                    o.Consignee_H = item.Consignee_H == null ? item.Consignee_H : (item.Consignee_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", "");//分单收货人
                    o.Notify_Part_H = item.Notify_Part_H == null ? item.Notify_Part_H : (item.Notify_Part_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", "");//分单通知人
                    var Qops_En = _oPS_EntrustmentInforService.Queryable().Where(x => x.Operation_Id == item.Operation_Id).FirstOrDefault();
                    var QDepart_Port = QPARA_AirPortRep.Where(x => x.PortCode == Qops_En.Depart_Port).FirstOrDefault();
                    if (QDepart_Port != null)
                    {
                        o.Depart_PortEng = QDepart_Port.PortNameEng; //起运港英文全称
                    }
                    else
                        o.Depart_PortEng = Qops_En.Depart_Port;
                    var QEnd_Port = QPARA_AirPortRep.Where(x => x.PortCode == Qops_En.End_Port).FirstOrDefault();
                    if (QEnd_Port != null)
                    {
                        o.End_PortEng = QEnd_Port.PortNameEng;   //目的港全称
                    }
                    else
                        o.End_PortEng = Qops_En.End_Port;
                    o.End_Port = Qops_En.End_Port;      //目的港三字代码
                    o.Flight_No = Qops_En.Flight_No;//航班号
                    o.Flight_No1 = Qops_En.Flight_No == null ? "" : Qops_En.Flight_No.Substring(0, 2); //航班号前两位
                    o.Flight_Date_Want = Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("yyyy-MM-dd"); //航班日期
                    o.Flight_No_Flight_Date_Want = Qops_En.Flight_No + " " + ((item.Is_Self == false ? (Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("dd,yyyy")) : (Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("MMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 3)).ToUpper() + " " + Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("dd,yyyy")));
                    o.Carriage_H_Bragainon_Article_H = (item.Carriage_H == null ? "" : (item.Carriage_H == "C" ? "FREIGHT COLLECT" : "FREIGHT PREPAID")) + " " + item.Bragainon_Article_H; //运费p/c + 成交条款
                    o.Carriage_H = item.Carriage_H;
                    o.Flight_No_Flight_Depart_PortEng = (Qops_En.Flight_Date_Want == null ? "" : Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("MMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 3).ToUpper() + " " + Convert.ToDateTime(Qops_En.Flight_Date_Want).ToString("dd,yyyy")) + " " + o.Depart_PortEng + " FEILI";//航班日期 + 起运港英文全称
                    o.Shipper_M = Qops_En.Consignee_M == null ? Qops_En.Consignee_M : (Qops_En.Consignee_M.ToString().Replace("\\r\\n", "\n")).Replace("\t", ""); //总单收货人
                    o.Declare_Value_Ciq_H = item.Declare_Value_Ciq_H;   //申明价值海关
                    o.Declare_Value_Trans_H = item.Declare_Value_Trans_H;  //申明价值运输
                    o.Pieces_H = item.Pieces_H.ToString() + "\n\n" + (Qops_En.Slac_SK == null ? "" : "SLAC:" + Qops_En.Slac_SK); ;//分单件数
                    o.Weight_H = item.Weight_H.ToString();//分单毛重
                    o.Volume_H = item.Volume_H == null ? "" : "VOL:" + item.Volume_H.ToString() + " CBM";//分单体积
                    o.Charge_Weight_H = item.Charge_Weight_H.ToString(); //分单计费重量
                    o.Marks_H = item.Marks_H == null ? item.Marks_H : (item.Marks_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", ""); //唛头
                    o.EN_Name_H = item.EN_Name_H == null ? item.EN_Name_H : (item.EN_Name_H.ToString().Replace("\\r\\n", "\n")).Replace("\t", ""); //英文货名


                    ArrPdfFile.Add(o);
                }

                #region 获取Excel中字段位置

                Dictionary<string, Tuple<int, int>> dict = new Dictionary<string, Tuple<int, int>>() { 
                    {"MBL",new Tuple<int,int>(0,21)},
                    {"HBL",new Tuple<int,int>(0,27)}, 
                    {"Shipper_H",new Tuple<int,int>(1,0)},
                    {"Consignee_H",new Tuple<int,int>(7,0)},
                    {"Notify_Part_H",new Tuple<int,int>(16,0)},
                    {"Depart_PortEng",new Tuple<int,int>(22,0)},
                    {"End_Port",new Tuple<int,int>(24,1)},
                    {"End_PortEng",new Tuple<int,int>(27,0)},
                    {"Flight_No1",new Tuple<int,int>(24,9)},
                    {"Flight_No_Flight_Date_Want",new Tuple<int,int>(27,9)},
                    {"Carriage_H_Bragainon_Article_H",new Tuple<int,int>(16,19)},
                    {"Declare_Value_Ciq_H",new Tuple<int,int>(24,27)},
                    {"Declare_Value_Trans_H",new Tuple<int,int>(24,28)},
                    {"Pieces_H",new Tuple<int,int>(36,0)},
                    {"Weight_H",new Tuple<int,int>(36,4)},
                    {"Charge_Weight_H",new Tuple<int,int>(36,14)},
                    {"Marks_H",new Tuple<int,int>(39,10)},
                    {"EN_Name_H",new Tuple<int,int>(36,21)},
                    {"Volume_H",new Tuple<int,int>(52,18)},
                    {"Flight_No_Flight_Depart_PortEng",new Tuple<int,int>(61,18)},
                    {"Shipper_M",new Tuple<int,int>(30,0)},
                    {"PP1",new Tuple<int,int>(25,23)},
                    {"PP2",new Tuple<int,int>(25,25)},
                    {"CC1",new Tuple<int,int>(25,24)},
                    {"CC2",new Tuple<int,int>(25,26)},
                    {"PPASAR1",new Tuple<int,int>(49,0)},
                    {"PPASAR2",new Tuple<int,int>(58,0)},
                    {"PPASAR3",new Tuple<int,int>(61,0)},
                    {"CCASAR1",new Tuple<int,int>(49,9)},
                    {"CCASAR2",new Tuple<int,int>(58,9)},
                    {"CCASAR3",new Tuple<int,int>(61,9)},
                };

                #endregion
                //Aspose.Cells.Workbook workbook1 = new Aspose.Cells.Workbook();
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(Server.MapPath("/FileModel/分单PDFupdate.xlsx"));

                //var sheet1 = workbook.Worksheets.Add("123");
                //sheet1 = workbook1.Worksheets[0];

                List<object> Arr = new List<object>();
                Arr.AddRange();
                for (int i = 0; i < ArrPdfFile.Count(); i++)
                {
                    if (i > 0)
                    {
                        //var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                        //Aspose.Cells.Worksheet Owsheet = new Aspose.Cells.Worksheet();
                        //Owsheet.Copy(sheet);
                        workbook.Worksheets.AddCopy(0);

                    }
                    var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[workbook.Worksheets.Count - 1];
                    sheet.Name = "分单" + (i + 1);
                    var objValue = ArrPdfFile[i];

                    #region 插入数据

                    var MBL = dict.Where(x => x.Key == "MBL");
                    if (MBL.Any())
                    {
                        var Tobj = MBL.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.MBL;
                    }
                    var HBL = dict.Where(x => x.Key == "HBL");
                    if (HBL.Any())
                    {
                        var Tobj = HBL.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.HBL;
                    }
                    var Shipper_H = dict.Where(x => x.Key == "Shipper_H");
                    if (Shipper_H.Any())
                    {
                        var Tobj = Shipper_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Shipper_H;
                    }
                    var Consignee_H = dict.Where(x => x.Key == "Consignee_H");
                    if (Consignee_H.Any())
                    {
                        var Tobj = Consignee_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Consignee_H;
                    }
                    var Notify_Part_H = dict.Where(x => x.Key == "Notify_Part_H");
                    if (Notify_Part_H.Any())
                    {
                        var Tobj = Notify_Part_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Notify_Part_H;
                    }
                    var Depart_PortEng = dict.Where(x => x.Key == "Depart_PortEng");
                    if (Depart_PortEng.Any())
                    {
                        var Tobj = Depart_PortEng.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Depart_PortEng;
                    }
                    var End_Port = dict.Where(x => x.Key == "End_Port");
                    if (End_Port.Any())
                    {
                        var Tobj = End_Port.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_Port;
                    }
                    var End_PortEng = dict.Where(x => x.Key == "End_PortEng");
                    if (End_PortEng.Any())
                    {
                        var Tobj = End_PortEng.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_PortEng;
                    }
                    var Flight_No1 = dict.Where(x => x.Key == "Flight_No1");
                    if (Flight_No1.Any())
                    {
                        var Tobj = Flight_No1.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No1;
                    }
                    var Flight_No_Flight_Date_Want = dict.Where(x => x.Key == "Flight_No_Flight_Date_Want");
                    if (Flight_No_Flight_Date_Want.Any())
                    {
                        var Tobj = Flight_No_Flight_Date_Want.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No_Flight_Date_Want;
                    }
                    var Carriage_H_Bragainon_Article_H = dict.Where(x => x.Key == "Carriage_H_Bragainon_Article_H");
                    if (Carriage_H_Bragainon_Article_H.Any())
                    {
                        var Tobj = Carriage_H_Bragainon_Article_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H_Bragainon_Article_H;
                    }
                    var Declare_Value_Ciq_H = dict.Where(x => x.Key == "Declare_Value_Ciq_H");
                    if (Declare_Value_Ciq_H.Any())
                    {
                        var Tobj = Declare_Value_Ciq_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Declare_Value_Ciq_H;
                    }
                    var Declare_Value_Trans_H = dict.Where(x => x.Key == "Declare_Value_Trans_H");
                    if (Declare_Value_Trans_H.Any())
                    {
                        var Tobj = Declare_Value_Trans_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Declare_Value_Trans_H;
                    }
                    var Pieces_H = dict.Where(x => x.Key == "Pieces_H");
                    if (Pieces_H.Any())
                    {
                        var Tobj = Pieces_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Pieces_H;
                    }
                    var Weight_H = dict.Where(x => x.Key == "Weight_H");
                    if (Weight_H.Any())
                    {
                        var Tobj = Weight_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Weight_H;
                    }
                    var Charge_Weight_H = dict.Where(x => x.Key == "Charge_Weight_H");
                    if (Charge_Weight_H.Any())
                    {
                        var Tobj = Charge_Weight_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Charge_Weight_H;
                    }
                    var Marks_H = dict.Where(x => x.Key == "Marks_H");
                    if (Marks_H.Any())
                    {
                        var Tobj = Marks_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Marks_H;
                    }
                    var EN_Name_H = dict.Where(x => x.Key == "EN_Name_H");
                    if (EN_Name_H.Any())
                    {
                        var Tobj = EN_Name_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.EN_Name_H;
                    }
                    var Flight_No_Flight_Depart_PortEng = dict.Where(x => x.Key == "Flight_No_Flight_Depart_PortEng");
                    if (Flight_No_Flight_Depart_PortEng.Any())
                    {
                        var Tobj = Flight_No_Flight_Depart_PortEng.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Flight_No_Flight_Depart_PortEng;
                    }
                    var Volume_H = dict.Where(x => x.Key == "Volume_H");
                    if (Volume_H.Any())
                    {
                        var Tobj = Volume_H.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Volume_H;
                    }
                    var Shipper_M = dict.Where(x => x.Key == "Shipper_M");
                    if (Shipper_M.Any())
                    {
                        var Tobj = Shipper_M.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Shipper_M;
                    }
                    var PP1 = dict.Where(x => x.Key == "PP1");
                    if (PP1.Any())
                    {
                        var Tobj = PP1.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "X" : "";
                    }
                    var PP2 = dict.Where(x => x.Key == "PP2");
                    if (PP1.Any())
                    {
                        var Tobj = PP2.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "X" : "";
                    }
                    var CC1 = dict.Where(x => x.Key == "CC1");
                    if (PP1.Any())
                    {
                        var Tobj = CC1.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "X" : "";
                    }
                    var CC2 = dict.Where(x => x.Key == "CC2");
                    if (PP1.Any())
                    {
                        var Tobj = CC2.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "X" : "";
                    }
                    var PPASAR1 = dict.Where(x => x.Key == "PPASAR1");
                    if (PPASAR1.Any())
                    {
                        var Tobj = PPASAR1.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "AS ARRANGED" : "";
                    }
                    var PPASAR2 = dict.Where(x => x.Key == "PPASAR2");
                    if (PPASAR2.Any())
                    {
                        var Tobj = PPASAR2.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "AS ARRANGED" : "";
                    }
                    var PPASAR3 = dict.Where(x => x.Key == "PPASAR3");
                    if (PPASAR3.Any())
                    {
                        var Tobj = PPASAR3.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "P" ? "AS ARRANGED" : "";
                    }
                    var CCASAR1 = dict.Where(x => x.Key == "CCASAR1");
                    if (CCASAR1.Any())
                    {
                        var Tobj = CCASAR1.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "AS ARRANGED" : "";
                    }
                    var CCASAR2 = dict.Where(x => x.Key == "CCASAR2");
                    if (PPASAR2.Any())
                    {
                        var Tobj = CCASAR2.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "AS ARRANGED" : "";
                    }
                    var CCASAR3 = dict.Where(x => x.Key == "CCASAR3");
                    if (CCASAR3.Any())
                    {
                        var Tobj = CCASAR3.First().Value;
                        sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Carriage_H == "C" ? "AS ARRANGED" : "";
                    }

                    #endregion
                }
                var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
                string FName = Server.MapPath(dPath + "/ExlDD/" + fileName);
                //var newms = Server.MapPath(@"\DownLoad\分单PDFupdate.xlsx");
                //workbook.Save(newms);
                //return File(newms, "application/vnd.ms-excel", fileName);
                workbook.Save(FName, Aspose.Cells.SaveFormat.Pdf);
                //System.IO.FileInfo oo = new System.IO.FileInfo(FName);
                //application/pdf;application/octet-stream; 
                System.IO.FileInfo oo = new System.IO.FileInfo(FName);
                if (typeName == "打印分单")
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(fileName), "application/json");
                }
                else
                {
                    return File(FName, "application/pdf", oo.Name);
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg });
            }

        }

        public ActionResult ExportWHReceiptExcel(Array filterRules = null, string sort = "Id", string order = "asc", string typeName = "")
        {
            var strids = "";
            foreach (var item in filterRules)
            {
                strids = item.ToString();
            }
            var idarr = strids.Split(",");
            var data = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Operation_Id)).OrderBy(sort, order).ToList();
            var ArrMBL = data.Where(x => !string.IsNullOrWhiteSpace(x.MBL)).Select(x => x.MBL);
            var Qarrmnl = _oPS_EntrustmentInforService.Queryable().Where(x => ArrMBL.Contains(x.MBL) && x.OPS_M_Order.OPS_BMS_Status == true).OrderBy(sort, order).Select(x => x.MBLId);
            var ArrOps_M_ID = data.Select(x => x.MBLId);
            Qarrmnl = Qarrmnl.Union(ArrOps_M_ID);

            if (data == null || data.Count == 0)
            {
                return null;
            }
            #region  数据获取

            List<OPS_M_Order> ArrOPS_M_Order = new List<OPS_M_Order>();
            var OPS_M_OrderRep = _unitOfWork.Repository<OPS_M_Order>().Queryable();

            List<OPS_H_Order> ArrOPS_H_Order = new List<OPS_H_Order>();
            var OPS_H_OrderRep = _unitOfWork.Repository<OPS_H_Order>().Queryable();

            List<CustomsInspection> ArrCustomsInspection = new List<CustomsInspection>();
            var CustomsInspectionRep = _unitOfWork.Repository<CustomsInspection>().Queryable();

            List<DocumentManagement> ArrDocumentManagement = new List<DocumentManagement>();
            var DocumentManagementRep = _unitOfWork.Repository<DocumentManagement>().Queryable();

            List<Bms_Bill_Ap> ArrBmsBillAp = new List<Bms_Bill_Ap>();
            var BmsBillApRep = _unitOfWork.Repository<Bms_Bill_Ap>().Queryable();
            List<Bms_Bill_Ap_Dtl> ArrBmsBillApDtl = new List<Bms_Bill_Ap_Dtl>();
            var BmsBillApDtlRep = _unitOfWork.Repository<Bms_Bill_Ap_Dtl>().Queryable();

            List<Bms_Bill_Ar> ArrBmsBillAr = new List<Bms_Bill_Ar>();
            var BmsBillArRep = _unitOfWork.Repository<Bms_Bill_Ar>().Queryable();
            List<Bms_Bill_Ar_Dtl> ArrBmsBillArDtl = new List<Bms_Bill_Ar_Dtl>();
            var BmsBillArDtlRep = _unitOfWork.Repository<Bms_Bill_Ar_Dtl>().Queryable();

            List<Rate> ArrRate = new List<Rate>();
            var RateRep = _unitOfWork.Repository<Rate>().Queryable();

            List<DailyRate> ArrDailyRate = new List<DailyRate>();
            var DailyRateRep = _unitOfWork.Repository<DailyRate>().Queryable();

            //缓存数据
            var ArrPARA_AirPort = (IEnumerable<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var ArrCusBusInfo = (IEnumerable<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();
            var QFeeType = (IEnumerable<FeeType>)CacheHelper.Get_SetCache(Common.CacheNameS.FeeType);


            List<TObjTest> ArrPdfFile = new List<TObjTest>();
            List<string> ArrErrMsg = new List<string>();
            List<Task> ArrTask = new List<Task>();
            //主单
            //ArrTask.Add(Task.Run(() =>
            //{
            var QOPS_M_Order = OPS_M_OrderRep.Where(x => ArrOps_M_ID.Contains(x.Id)).Include(x => x.OPS_H_Orders).ToList();
            ArrOPS_M_Order.AddRange(QOPS_M_Order);
            //}));
            ////分单
            //ArrTask.Add(Task.Run(() =>
            //{
            //    var QOPS_H_Order = OPS_H_OrderRep.Where(x => ArrOps_M_ID.Contains(x.MBLId)).ToList();
            //    ArrOPS_H_Order.AddRange(QOPS_H_Order);
            //}));
            //报关报检
            //ArrTask.Add(Task.Run(() =>
            //{
            var QCustomsInspection = CustomsInspectionRep.Where(x => idarr.Contains(x.Operation_ID)).ToList();
            ArrCustomsInspection.AddRange(QCustomsInspection);
            //}));
            //单证管理
            //ArrTask.Add(Task.Run(() =>
            //{
            var QDocumentManagement = DocumentManagementRep.Where(x => idarr.Contains(x.Operation_ID)).ToList();
            ArrDocumentManagement.AddRange(QDocumentManagement);
            //}));
            //应收账单
            //ArrTask.Add(Task.Run(() =>
            //{
            var QBmsBillAr = BmsBillArRep.Where(x => Qarrmnl.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ar_Dtl).ToList();
            ArrBmsBillAr.AddRange(QBmsBillAr);
            //}));
            //ArrTask.Add(Task.Run(() =>
            //{
            //    var QBmsBillArDtl = BmsBillArDtlRep.Where(x => ArrOps_M_ID.Contains(x.Ops_M_OrdId)).ToList();
            //    ArrBmsBillArDtl.AddRange(QBmsBillArDtl);
            //}));
            //应付账单
            //ArrTask.Add(Task.Run(() =>
            //{
            var QBmsBillAp = BmsBillApRep.Where(x => Qarrmnl.Contains(x.Ops_M_OrdId)).Include(x => x.ArrBms_Bill_Ap_Dtl).ToList();
            ArrBmsBillAp.AddRange(QBmsBillAp);
            //}));
            //ArrTask.Add(Task.Run(() =>
            //{
            //    var qq = BmsBillApDtlRep.Where(x => ArrMBL.Contains(x.OBms_Bill_Ap.MBL) || ArrOps_M_ID.Contains(x.Ops_M_OrdId)).AsQueryable();
            //    var QBmsBillApDtl = qq.ToList();
            //    ArrBmsBillApDtl.AddRange(QBmsBillApDtl);
            //}));
            //Task.WaitAll(ArrTask.ToArray());
            //List<Bms_Bill_Ap_Dtl> ArrApDtl = new List<Bms_Bill_Ap_Dtl>();
            //ArrBmsBillAp.ForEach((x) =>
            //{
            //    if (x.ArrBms_Bill_Ap_Dtl != null)
            //    {
            //        ArrApDtl.AddRange(x.ArrBms_Bill_Ap_Dtl);
            //    }
            //});
            //List<Bms_Bill_Ar_Dtl> ArrArDtl = new List<Bms_Bill_Ar_Dtl>();
            //ArrBmsBillAr.ForEach((x) =>
            //{
            //    if (x.ArrBms_Bill_Ar_Dtl != null)
            //    {
            //        ArrArDtl.AddRange(x.ArrBms_Bill_Ar_Dtl);
            //    }
            //});

            #endregion
            //var data1 = data.OrderBy(sort, order).ToList();
            foreach (var item in data)
            {
                TObjTest c = new TObjTest();
                var QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Entrustment_Name);
                if (QArrCusBusInfo.Any())
                    c.CusBusName = QArrCusBusInfo.FirstOrDefault().EnterpriseShortName;
                else
                    c.CusBusName = item.Entrustment_Name;
                c.Dzbh = item.Operation_Id;
                var QArrAppUser = ArrAppUser.Where(x => x.UserName == item.ADDWHO).FirstOrDefault();
                if (QArrAppUser != null && !string.IsNullOrWhiteSpace(QArrAppUser.Id))
                {
                    c.ADDWHO = QArrAppUser.UserNameDesc;
                }
                else
                    c.ADDWHO = item.ADDWHO;

                string Book_Flat_Name = "";
                QArrCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Book_Flat_Code);
                if (QArrCusBusInfo.Any())
                {
                    var OCusBusInfo = QArrCusBusInfo.FirstOrDefault();
                    c.Book_Flat_Code = OCusBusInfo.EnterpriseShortName;
                }
                else
                    c.Book_Flat_Code = item.Book_Flat_Code;
                var mbllist = OPS_M_OrderRep.Where(x => x.MBL == item.MBL && x.OPS_BMS_Status == true).ToList().Count();
                //var MBLcount = mbllist.Select(x => x.Dzbh).Distinct().Count();
                var DZbhcount = _oPS_EntrustmentInforService.Queryable().Where(x => x.MBL == item.MBL && x.Is_TG == false).Count();
                var DZbhcount2 = DZbhcount - mbllist;
                c.Num_NoTG = DZbhcount2.ToString();
                //string End_PortName = "";
                //var QArrPARA_AirPort = ArrPARA_AirPort.Where(x => x.PortCode == item.End_Port);
                //if (QArrPARA_AirPort.Any())
                //{
                //    var OPARA_AirPort = QArrPARA_AirPort.FirstOrDefault();
                //    End_PortName = OPARA_AirPort.PortName;
                //}
                c.End_Port = item.End_Port;
                //if (item.MBL == null || item.MBL == "") 
                //{
                //    c.MBL = item.MBL;
                //}
                //else
                //{
                //    var MblArr = item.MBL.ToCharArray().ToList();
                //    MblArr.Insert(7, ' ');
                //    MblArr.Insert(3, '-');
                //    var NewMBL = MblArr.ToString();
                //    c.MBL = NewMBL;
                //}
                //var MblArr = item.MBL.ToCharArray().ToList();
                //MblArr.Insert(7, ' ');
                //MblArr.Insert(3, '-');
                //var NewMBL = MblArr.ToString();

                var MBL1 = item.MBL == null ? "" : item.MBL.Substring(0, 3);
                var MBL2 = item.MBL == null ? "" : item.MBL.Substring(3, 4);
                var MBL3 = item.MBL == null ? "" : item.MBL.Substring(7);
                c.MBL = MBL1 + "-" + MBL2 + " " + MBL3;

                c.Starttime = Convert.ToDateTime(item.Flight_Date_Want).ToString("yyyy-MM-dd") + "/" + item.Flight_No;
                string YDpay = "";
                string HBL = "";
                var QArrOPS_M_Order = ArrOPS_M_Order.Where(x => x.Id == item.MBLId);
                if (QArrOPS_M_Order.Any())
                {
                    var OOPS_M_Order = QArrOPS_M_Order.FirstOrDefault();
                    if (OOPS_M_Order.OPS_H_Orders != null && OOPS_M_Order.OPS_H_Orders.Any())
                    {
                        var OOPS_H_Order = OOPS_M_Order.OPS_H_Orders.FirstOrDefault();
                        YDpay = OOPS_H_Order.Pay_Mode_H + "/" + OOPS_H_Order.Bragainon_Article_H;
                        HBL = OOPS_H_Order.HBL;
                    }
                }
                c.HBL = HBL;
                c.YDpay = YDpay;
                c.Is_XC = item.Is_XC ? "√" : "";
                var OCustomsInspectionYTH = (from p in ArrCustomsInspection
                                             where (p.Customs_Declaration == "1" || p.Customs_Declaration == "2" || p.Customs_Declaration == "3") && p.Operation_ID == item.Operation_Id
                                             group p by p.Customs_Declaration into g
                                             select new
                                             {
                                                 Customs_Declaration = g.Key,
                                                 Num_BG = g.Sum(x => x.Num_BG),
                                                 IS_Checked_BG = g.Any(x => x.IS_Checked_BG)
                                             }).ToList();
                c.IS_Checked_BG = ArrCustomsInspection == null ? "" : (CustomsInspectionRep.Where(p => p.Operation_ID == item.Operation_Id).Any(x => x.IS_Checked_BG) ? "√" : "");
                c.Num_BGPH = OCustomsInspectionYTH == null ? "" : (OCustomsInspectionYTH.Where(x => x.Customs_Declaration == "1").Sum(x => x.Num_BG) == 0 ? "" : OCustomsInspectionYTH.Where(x => x.Customs_Declaration == "1").Sum(x => x.Num_BG).ToString());
                c.Num_BGYTH = OCustomsInspectionYTH == null ? "" : (OCustomsInspectionYTH.Where(x => x.Customs_Declaration == "2").Sum(x => x.Num_BG) == 0 ? "" : OCustomsInspectionYTH.Where(x => x.Customs_Declaration == "2").Sum(x => x.Num_BG).ToString());
                c.Num_BGZBG = OCustomsInspectionYTH == null ? "" : (OCustomsInspectionYTH.Where(x => x.Customs_Declaration == "3").Sum(x => x.Num_BG) == 0 ? "" : OCustomsInspectionYTH.Where(x => x.Customs_Declaration == "3").Sum(x => x.Num_BG).ToString());


                var ODocumentManagement = (from p in ArrDocumentManagement
                                           where p.Operation_ID == item.Operation_Id
                                           group p by p.Operation_ID into g
                                           select new
                                           {
                                               LDQTY = g.Sum(x => x.QTY),
                                           }).FirstOrDefault();
                c.LDQTY = ODocumentManagement == null ? "" : ODocumentManagement.LDQTY.ToString();
                c.MPieces_DC = item.Pieces_DC.ToString() + " PKG";
                if (item.IS_MoorLevel == true)
                {
                    c.KG_KG = "/靠级" + item.MoorLevel + " KG";
                }
                else c.KG_KG = "";
                c.MWeight_DC = item.IS_MoorLevel == true ? item.Weight_DC.ToString() + " KG" + "/" + item.MoorLevel + "KG" : item.Weight_DC.ToString() + " KG";
                c.MVolume_DC = item.Volume_DC == null ? " CBM" : Math.Round((decimal)item.Volume_DC, 3).ToString("#0.000") + " CBM";
                var MIsP_Z = item.Charge_Weight_DC - item.Weight_DC;
                c.MCharge_Weight_DC = MIsP_Z > 0 ? item.Charge_Weight_DC.ToString() + " KG" : "";
                c.SPieces_SK = item.Pieces_SK.ToString() + " PKG";
                c.SWeight_SK = item.Weight_SK.ToString() + " KG";
                c.SVolume_SK = item.Volume_SK == null ? " CBM" : Math.Round((decimal)item.Volume_SK, 3).ToString("#0.000") + " CBM";
                var SIsP_Z = item.Charge_Weight_SK - item.Weight_SK;
                c.SCharge_Weight_SK = SIsP_Z > 0 ? item.Charge_Weight_SK.ToString() + " KG" : "";
                c.FPieces_Fact = item.Pieces_Fact.ToString() + " PKG";
                c.FWeight_Fact = item.Weight_Fact.ToString() + " KG";
                c.FVolume_Fact = item.Volume_Fact == null ? " CBM" : Math.Round((decimal)item.Volume_Fact, 3).ToString("#0.000") + " CBM";
                var FIsP_Z = item.Charge_Weight_Fact - item.Weight_Fact;
                c.FCharge_Weight_Fact = FIsP_Z > 0 ? item.Charge_Weight_Fact.ToString() + " KG" : "";
                c.HPieces_SK = item.Pieces_SK.ToString() + " PKG";
                c.HWeight_DC = item.Account_Weight_SK.ToString() + " KG";
                c.HVolume_SK = item.Volume_SK == null ? "CBM" : Math.Round((decimal)item.Volume_SK, 3).ToString("#0.000") + " CBM";
                var HIsP_Z = item.Charge_Weight_SK - item.Weight_SK;
                c.HCharge_Weight_SK = HIsP_Z > 0 ? item.Charge_Weight_SK.ToString() + " KG" : "";
                c.Is_Self1 = item.Is_Self ? "" : "√";
                c.Is_Self2 = item.Is_Self ? "√" : "";

                var ArrFeeType = QFeeType.Select(x => x.FeeName).ToList();
                var ArrFeeType1 = QFeeType.Select(x => x.FeeName).ToList();
                var ArrFeeType2 = QFeeType.Select(x => x.FeeName).ToList();
                var ArrFeeType3 = QFeeType.Select(x => x.FeeName).ToList();
                List<string> ArrFee = new List<string> { "空运费", "战争附加险", "仓储费", "报关费", "燃油附加费", "卡车费" };
                for (var i = 0; i < ArrFee.Count; i++)
                {
                    ArrFeeType.Remove(ArrFee[i]);
                }
                var ArrFeeSum = ArrFeeType;
                //List<string> ArrFeeSum = new List<string> { "垫板费", "刻章费", "托盘费", "燃料费", "燃油费", "分单费", "关封费", "杂费", "制单费", "信息费", "运抵费", "运输费", "代理费", "其他费用", "过路费", "CHC费用", "报关联单费", "查验费", "操作费", "附加费", "CCA费用", "单证费", "消磁服务费", "鉴定费", "出口操作费", "港务费", "搞定费", "快递费", "翻板费" };
                List<string> ArrFeeAR = new List<string> { "空运费", "战争附加险", "报关费", "燃油附加费", "燃油费" };
                //List<string> ArrFeeSumAR = new List<string> { "垫板费", "刻章费", "托盘费", "卡车费", "仓储费", "燃料费", "分单费", "关封费", "杂费", "制单费", "信息费", "运抵费", "运输费", "代理费", "其他费用", "过路费", "CHC费用", "报关联单费", "查验费", "操作费", "附加费", "CCA费用", "单证费", "消磁服务费", "鉴定费", "出口操作费", "港务费", "搞定费", "快递费", "翻板费" };
                for (var i = 0; i < ArrFeeAR.Count; i++)
                {
                    ArrFeeType1.Remove(ArrFeeAR[i]);
                }
                var ArrFeeSumAR = ArrFeeType1;
                List<Fee> ArrMBLFee = new List<Fee>();
                List<Fee> ArrMBLFeeYFHZ = new List<Fee>();
                List<Fee> ArrMBLFeeNew = new List<Fee>();
                List<Fee> ArrMBLFeeNewYFHZ = new List<Fee>();

                #region 应付

                //分摊+委托数据
                //var Sum = ArrBmsBillAp.Where(p => p.Bill_Type == "C/N" && p.FTParentId > 0 && p.Ops_M_OrdId == item.MBLId).Sum(x => x.Bill_Account2);
                var Sum = ArrBmsBillAp.Where(p => (p.Bill_Type == "C/N" || p.Bill_Type == "FTYF") && p.Ops_M_OrdId == item.MBLId).Sum(x => x.Bill_Account2);
                var SumAr = ArrBmsBillAr.Where(p => (p.Bill_Type == "D/N" || p.Bill_Type == "FP") && p.Ops_M_OrdId == item.MBLId).Sum(x => x.Bill_Account2);
                //委托+总单号
                var QZDYF = _oPS_EntrustmentInforService.Queryable().Where(x => x.MBL == item.MBL && x.OPS_M_Order.OPS_BMS_Status == true).OrderBy(sort, order).Select(x => x.MBLId);
                var aa = ArrBmsBillAp.Where(p => p.Bill_Type == "C/N" && p.FTParentId == 0 && (p.Ops_M_OrdId == item.MBLId || p.MBL == item.MBL));
                ArrBmsBillAp.Where(p => p.Bill_Type == "C/N" && p.FTParentId == 0 && (p.Ops_M_OrdId == item.MBLId || QZDYF.Contains(p.Ops_M_OrdId))).ForEach((p) =>
                {
                    //if (p.Ops_M_OrdId != item.MBLId && !p.IsMBLJS)
                    //{
                    //}
                    //else
                    //{
                    //    if (p.Money_Code != "CNY")
                    //    {
                    //        #region 计算外汇

                    //        #endregion
                    //    }
                    //p.ArrBms_Bill_Ap_Dtl = ArrBmsBillApDtl.Where(x => x.Bms_Bill_Ap_Id == p.Id);
                    if (p.ArrBms_Bill_Ap_Dtl != null && p.ArrBms_Bill_Ap_Dtl.Any())
                    {
                        p.ArrBms_Bill_Ap_Dtl.Where(x => ArrFee.Contains(x.Charge_Desc)).ForEach((n) =>
                        {
                            var QArrMBLFee = ArrMBLFee.Where(x => (x.Key == n.Charge_Desc && x.Unitprice2 == n.Unitprice2 && x.Qty == n.Qty) && !x.IsAr);
                            if (QArrMBLFee.Any())
                            {
                                var OArrMBLFee = QArrMBLFee.FirstOrDefault();
                                OArrMBLFee.Qty += n.Qty;
                                OArrMBLFee.Unitprice2 = n.Unitprice2;
                                OArrMBLFee.Account2 += n.Account2;
                                OArrMBLFee.Money_Code = p.Money_Code == "CNY" ? "" : p.Money_Code;
                                OArrMBLFee.RankingNo = n.Charge_Code == "KYF" ? 1 : 2;
                            }
                            else
                            {
                                ArrMBLFee.Add(new Fee(n.Charge_Desc, true, n.Qty, n.Unitprice2, n.Account2, p.Money_Code == "CNY" ? "" : p.Money_Code, 1, n.Charge_Code == "KYF" ? 1 : 2));
                            }
                        });
                        p.ArrBms_Bill_Ap_Dtl.Where(x => ArrFeeSum.Contains(x.Charge_Desc)).ForEach((n) =>
                        {
                            var QArrMBLFee = ArrMBLFee.Where(x => x.Key == n.Charge_Desc && !x.IsAr && x.Unitprice2 == n.Unitprice2);
                            if (QArrMBLFee.Any())
                            {
                                var OArrMBLFee = QArrMBLFee.FirstOrDefault();
                                OArrMBLFee.Account2 += n.Account2;
                                OArrMBLFee.Money_Code = p.Money_Code == "CNY" ? "" : p.Money_Code;
                                OArrMBLFee.RankingNo = n.Charge_Code == "KYF" ? 1 : 2;
                            }
                            else
                            {
                                ArrMBLFee.Add(new Fee(n.Charge_Desc, false, n.Qty, n.Unitprice2, n.Account2, p.Money_Code == "CNY" ? "" : p.Money_Code, 1, n.Charge_Code == "KYF" ? 1 : 2));
                            }
                        });
                    }
                    //}
                });

                #endregion

                #region 应付汇总
                ArrBmsBillAp.Where(p => (p.Bill_Type == "C/N" || p.Bill_Type == "FTYF") && p.Ops_M_OrdId == item.MBLId).ForEach((p) =>
                {
                    if (p.Money_Code != "CNY")
                    {
                        #region 计算外汇

                        #endregion
                    }
                    //p.ArrBms_Bill_Ap_Dtl = ArrBmsBillApDtl.Where(x => x.Bms_Bill_Ap_Id == p.Id);
                    if (p.ArrBms_Bill_Ap_Dtl != null && p.ArrBms_Bill_Ap_Dtl.Any())
                    {
                        p.ArrBms_Bill_Ap_Dtl.Where(x => ArrFee.Contains(x.Charge_Desc) || ArrFeeSum.Contains(x.Charge_Desc)).ForEach((n) =>
                        {

                            var QArrMBLFeeYFHZ = ArrMBLFeeYFHZ.Where(x => x.Key == n.Charge_Desc && x.Unitprice2 == n.Unitprice2);


                            if (QArrMBLFeeYFHZ.Any())
                            {
                                var OArrMBLFeeYFHZ = QArrMBLFeeYFHZ.FirstOrDefault();
                                OArrMBLFeeYFHZ.Qty += n.Qty;
                                OArrMBLFeeYFHZ.Unitprice2 = n.Unitprice2;
                                OArrMBLFeeYFHZ.Account2 += n.Account2;
                                OArrMBLFeeYFHZ.Money_Code = p.Money_Code == "CNY" ? "" : p.Money_Code;
                                OArrMBLFeeYFHZ.RankingNo = n.Charge_Code == "KYF" ? 1 : 2;
                            }
                            else
                            {
                                ArrMBLFeeYFHZ.Add(new Fee(n.Charge_Desc, true, n.Qty, n.Unitprice2, n.Account2, p.Money_Code == "CNY" ? "" : p.Money_Code, 0, n.Charge_Code == "KYF" ? 1 : 2, true));
                            }
                        });
                        //p.ArrBms_Bill_Ap_Dtl.Where(x => ArrFeeSum.Contains(x.Charge_Desc)).ForEach((n) =>
                        //{
                        //    var QArrMBLFeeYFHZ = ArrMBLFeeYFHZ.Where(x => x.Key == n.Charge_Desc);
                        //    if (QArrMBLFeeYFHZ.Any())
                        //    {
                        //        var OArrMBLFeeYFHZ = QArrMBLFeeYFHZ.FirstOrDefault();
                        //        OArrMBLFeeYFHZ.Account2 += n.Account2;
                        //        OArrMBLFeeYFHZ.Money_Code = p.Money_Code; OArrMBLFeeYFHZ.Money_Code = p.Money_Code;
                        //    }
                        //    else
                        //    {
                        //        ArrMBLFeeYFHZ.Add(new Fee(n.Charge_Desc, false, n.Qty, n.Unitprice2, n.Account2, p.Money_Code, 0, true));
                        //    }
                        //});
                    }
                });
                #endregion

                #region 应收
                //委托
                ArrBmsBillAr.Where(p => (p.Bill_Type == "FP" || p.Bill_Type == "D/N") && (p.Ops_M_OrdId == item.MBLId)).ForEach((p) =>
                {
                    if (p.Money_Code != "CNY")
                    {
                        #region 计算外汇

                        #endregion
                    }
                    //p.ArrBms_Bill_Ar_Dtl = ArrBmsBillArDtl.Where(x => x.Bms_Bill_Ar_Id == p.Id);
                    if (p.ArrBms_Bill_Ar_Dtl != null && p.ArrBms_Bill_Ar_Dtl.Any())
                    {
                        p.ArrBms_Bill_Ar_Dtl.Where(x => ArrFeeAR.Contains(x.Charge_Desc)).ForEach((n) =>
                        {

                            var QArrMBLFee = ArrMBLFee.Where(x => x.Key == n.Charge_Desc && x.Unitprice2 == n.Unitprice2 && x.IsAr);


                            if (QArrMBLFee.Any())
                            {
                                var OArrMBLFee = QArrMBLFee.FirstOrDefault();
                                OArrMBLFee.Qty += n.Qty;
                                OArrMBLFee.Unitprice2 = n.Unitprice2;
                                OArrMBLFee.Account2 += n.Account2;
                                OArrMBLFee.Money_Code = p.Money_Code == "CNY" ? "" : p.Money_Code;
                                OArrMBLFee.RankingNo = n.Charge_Code == "KYF" ? 1 : 2;
                            }
                            else
                            {
                                ArrMBLFee.Add(new Fee(n.Charge_Desc, true, n.Qty, n.Unitprice2, n.Account2, p.Money_Code == "CNY" ? "" : p.Money_Code, 0, n.Charge_Code == "KYF" ? 1 : 2, true));
                            }
                        });
                        p.ArrBms_Bill_Ar_Dtl.Where(x => ArrFeeSumAR.Contains(x.Charge_Desc)).ForEach((n) =>
                        {
                            var QArrMBLFee = ArrMBLFee.Where(x => x.Key == n.Charge_Desc && x.IsAr && x.Unitprice2 == n.Unitprice2);
                            if (QArrMBLFee.Any())
                            {
                                var OArrMBLFee = QArrMBLFee.FirstOrDefault();
                                OArrMBLFee.Account2 += n.Account2;
                                OArrMBLFee.Money_Code = p.Money_Code; OArrMBLFee.Money_Code = p.Money_Code;
                                OArrMBLFee.RankingNo = n.Charge_Code == "KYF" ? 1 : 2;
                            }
                            else
                            {
                                ArrMBLFee.Add(new Fee(n.Charge_Desc, false, n.Qty, n.Unitprice2, n.Account2, p.Money_Code == "CNY" ? "" : p.Money_Code, 0, n.Charge_Code == "KYF" ? 1 : 2, true));
                            }
                        });
                    }
                });

                #endregion

                foreach (var itemm in ArrMBLFee)
                {
                    var aabb = RateRep.Where(x => x.LocalCurrCode == itemm.Money_Code && x.ForeignCurrCode == "CNY" && x.Year == (item.Flight_Date_Want == null ? DateTime.Now.Year : ((DateTime)item.Flight_Date_Want).Year) && x.Month == (item.Flight_Date_Want == null ? DateTime.Now.Month : ((DateTime)item.Flight_Date_Want).Month)).FirstOrDefault();

                    ArrMBLFeeNew.Add(new Fee(itemm.Key, itemm.type, itemm.Qty, itemm.Unitprice2, itemm.Account2, itemm.Money_Code == "CNY" ? "" : itemm.Money_Code, aabb == null ? 1 : aabb.RecRate, itemm.RankingNo, itemm.IsAr));

                    //var aabbb = aabb.ToList();
                }
                //foreach (var itemm in ArrMBLFee)
                //{
                //    var aabb = RateRep.Where(x => x.ForeignCurrCode == itemm.Money_Code && x.Year == DateTime.Now.Year && x.Month == DateTime.Now.Month).FirstOrDefault();

                //    ArrMBLFeeNew.Add(new Fee(itemm.Key, itemm.type, itemm.Qty, itemm.Unitprice2, itemm.Account2, itemm.Money_Code == "CNY" ? "" : itemm.Money_Code, aabb == null ? 1 : aabb.RecRate, itemm.IsAr));

                //    //var aabbb = aabb.ToList();
                //}
                foreach (var itemmm in ArrMBLFeeYFHZ)
                {
                    var aabb = RateRep.Where(x => x.LocalCurrCode == itemmm.Money_Code && x.ForeignCurrCode == "CNY" && x.Year == (item.Flight_Date_Want == null ? DateTime.Now.Year : ((DateTime)item.Flight_Date_Want).Year) && x.Month == (item.Flight_Date_Want == null ? DateTime.Now.Month : ((DateTime)item.Flight_Date_Want).Month)).FirstOrDefault();

                    ArrMBLFeeNewYFHZ.Add(new Fee(itemmm.Key, false, itemmm.Qty, itemmm.Unitprice2, itemmm.Account2, itemmm.Money_Code == "CNY" ? "" : itemmm.Money_Code, aabb == null ? 1 : aabb.PayRate, itemmm.RankingNo, false));

                    //var aabbb = aabb.ToList();
                }
                dynamic dyObj = new System.Dynamic.ExpandoObject();
                //应付空运费
                string YFKYF = "";
                string YFKYF1 = "";
                string YFKYF2 = "";
                decimal SumYFKYF = 0;
                decimal SumYFKYF1 = 0;
                decimal SumYFKYF2 = 0;
                var Where_ArrMBLFee = ArrMBLFeeNew.Where(x => !x.IsAr);
                List<string> ArrFee1 = new List<string> { "空运费", "战争附加险", "燃油附加费", "卡车费" };
                List<string> ArrFee2 = new List<string> { "杂费", "制单费", "信息费", "CCA费用", "运抵费" ,"CHC费用"};
                var Q_ArrMBLFee = Where_ArrMBLFee.Where(x => ArrFee1.Contains(x.Key) && x.type == true);
                var Q_ArrMBLFee3 = Where_ArrMBLFee.Where(x => ArrFee1.Contains(x.Key) && x.type.ToString() == "True");
                var Q_ArrMBLFee1 = Where_ArrMBLFee.Where(x => ArrFee1.Contains(x.Key) || ArrFee2.Contains(x.Key));
                var SumFee = Q_ArrMBLFee.Sum(x => x.Account2);
                var Sumfee = Q_ArrMBLFee.Where(x => x.Money_Code == "").Sum(x => x.Account2);
                var GroupMoneycode1 = from p in Q_ArrMBLFee1
                                      group p by p.Money_Code into g
                                      select new
                                      {
                                          Money_Code = g.Max(x => x.Money_Code),
                                          Rate = g.Max(x => x.Rate),
                                          sumAccount2 = g.Sum(x => x.Account2),

                                      };
                var mmmm = GroupMoneycode1.ToList();
                var summm = string.Join("+", mmmm.Select(x => x.Money_Code + x.sumAccount2));
                c.YFKYF = string.Join("+", Q_ArrMBLFee3.OrderBy(x => x.RankingNo).Select(x => x.Money_Code + ((x.Qty * x.Unitprice2 == x.Account2) ? (x.Unitprice2 + "*" + x.Qty) : (x.Account2).ToString())));
                Q_ArrMBLFee = ArrMBLFeeNew.Where(x => ArrFee2.Contains(x.Key) && !x.IsAr && !x.type);
                var GroupMoneycode2 = from p in Q_ArrMBLFee
                                      group p by p.Money_Code into g
                                      select new
                                      {
                                          Money_Code = g.Max(x => x.Money_Code),
                                          Rate = g.Max(x => x.Rate),
                                          sumAccount2 = g.Sum(x => x.Account2),

                                      };
                var mmmmm = GroupMoneycode2.ToList();
                if (Q_ArrMBLFee.Any() && Q_ArrMBLFee3.Any())
                {
                    c.YFKYF += "+" + string.Join("+", Q_ArrMBLFee.Select(x => x.Money_Code + x.Account2));
                }
                else
                {
                    c.YFKYF += string.Join("+", Q_ArrMBLFee.Select(x => x.Money_Code + x.Account2));
                }
                c.YFKYF += SumFee == 0 ? "" : (Q_ArrMBLFee3.Count() == 1 && Q_ArrMBLFee3.Any(x => x.Qty * x.Unitprice2 != x.Account2)) ? "" : ("=" + summm);

                //应付仓储费
                string YFCCF = "";
                string SumYFCCF = "";
                List<string> ArrYFCCF = new List<string> { "仓储费" };
                Q_ArrMBLFee = Where_ArrMBLFee.Where(x => ArrYFCCF.Contains(x.Key) && x.type);
                GroupMoneycode1 = (from p in Q_ArrMBLFee
                                   group p by p.Money_Code into g
                                   select new
                                   {
                                       Money_Code = g.Max(x => x.Money_Code),
                                       Rate = g.Max(x => x.Rate),
                                       sumAccount2 = g.Sum(x => x.Account2),

                                   }).ToList();
                if (Q_ArrMBLFee.Any())
                {
                    SumYFCCF = string.Join("+", GroupMoneycode1.Select(x => x.Money_Code + x.sumAccount2));
                    if (Q_ArrMBLFee.Count() == 1 && Q_ArrMBLFee.Any(x => x.Unitprice2 * x.Qty != x.Account2))
                    {
                        YFCCF = SumYFCCF;
                    }
                    else
                        YFCCF = string.Join("+", Q_ArrMBLFee.Select(x => x.Money_Code + ((x.Qty * x.Unitprice2 == x.Account2) ? (x.Unitprice2 + "*" + x.Qty) : (x.Account2).ToString()))) + "=" + SumYFCCF;
                }
                c.YFCCF = YFCCF;

                //应付报关费
                string YFBBF = "";
                string SumYFBBF = "";
                List<string> ArrYFBBF = new List<string> { "报关费" };
                List<string> ArrYFBBF1 = new List<string> { "关封费" };
                Q_ArrMBLFee = Where_ArrMBLFee.Where(x => ArrYFBBF.Contains(x.Key) && x.type == true);
                Q_ArrMBLFee3 = Where_ArrMBLFee.Where(x => ArrYFBBF.Contains(x.Key) && x.type.ToString() == "True");
                Q_ArrMBLFee1 = Where_ArrMBLFee.Where(x => ArrYFBBF1.Contains(x.Key) || ArrYFBBF.Contains(x.Key));
                SumFee = Q_ArrMBLFee.Sum(x => x.Account2);
                Sumfee = Q_ArrMBLFee.Where(x => x.Money_Code == "").Sum(x => x.Account2);
                GroupMoneycode1 = from p in Q_ArrMBLFee1
                                  group p by p.Money_Code into g
                                  select new
                                  {
                                      Money_Code = g.Max(x => x.Money_Code),
                                      Rate = g.Max(x => x.Rate),
                                      sumAccount2 = g.Sum(x => x.Account2),

                                  };
                mmmm = GroupMoneycode1.ToList();
                summm = string.Join("+", mmmm.Select(x => x.Money_Code + x.sumAccount2));
                if (Q_ArrMBLFee3.Any(x => x.Unitprice2 * x.Qty != x.Account2))
                {

                }
                c.YFBBF = string.Join("+", Q_ArrMBLFee3.Select(x => x.Money_Code + ((x.Qty * x.Unitprice2 == x.Account2) ? (x.Unitprice2 + "*" + x.Qty) : (x.Account2).ToString())));
                Q_ArrMBLFee = ArrMBLFeeNew.Where(x => ArrYFBBF1.Contains(x.Key) && !x.IsAr && !x.type);
                GroupMoneycode2 = from p in Q_ArrMBLFee
                                  group p by p.Money_Code into g
                                  select new
                                  {
                                      Money_Code = g.Max(x => x.Money_Code),
                                      Rate = g.Max(x => x.Rate),
                                      sumAccount2 = g.Sum(x => x.Account2),

                                  };
                mmmmm = GroupMoneycode2.ToList();
                if (Q_ArrMBLFee.Any() && Q_ArrMBLFee3.Any())
                {
                    c.YFBBF += "+" + string.Join("+", Q_ArrMBLFee.Select(x => x.Money_Code + x.Account2));
                }
                else
                {
                    c.YFBBF += string.Join("+", Q_ArrMBLFee.Select(x => x.Money_Code + x.Account2));
                }
                c.YFBBF += SumFee == 0 ? "" : (Q_ArrMBLFee3.Count() == 1 && Q_ArrMBLFee3.Any(x => x.Qty * x.Unitprice2 != x.Account2)) ? "" : ("=" + summm);

                //应付车队费
                string YFCDF = "";
                string SumYFCDF = "";
                List<string> ArrYFCDF = new List<string> { "运输费" };
                Q_ArrMBLFee = Where_ArrMBLFee.Where(x => ArrYFCDF.Contains(x.Key) && !x.type);
                GroupMoneycode1 = (from p in Q_ArrMBLFee
                                   group p by p.Money_Code into g
                                   select new
                                   {
                                       Money_Code = g.Max(x => x.Money_Code),
                                       Rate = g.Max(x => x.Rate),
                                       sumAccount2 = g.Sum(x => x.Account2),

                                   }).ToList();
                SumYFCDF = string.Join("+", GroupMoneycode1.Select(x => x.Money_Code + x.sumAccount2));
                c.YFCDF = SumYFCDF.ToString();

                //应付代理费
                string YFDLF = "";
                string SumYFDLF = "";
                List<string> ArrYFDLF = new List<string> { "代理费" };
                Q_ArrMBLFee = Where_ArrMBLFee.Where(x => ArrYFDLF.Contains(x.Key) && !x.type);
                GroupMoneycode1 = (from p in Q_ArrMBLFee
                                   group p by p.Money_Code into g
                                   select new
                                   {
                                       Money_Code = g.Max(x => x.Money_Code),
                                       Rate = g.Max(x => x.Rate),
                                       sumAccount2 = g.Sum(x => x.Account2),

                                   }).ToList();
                SumYFDLF = string.Join("+", GroupMoneycode1.Select(x => x.Money_Code + x.sumAccount2));
                c.YFDLF = SumYFDLF.ToString();


                //应付其他费
                string YFQTFY2 = "";
                string YFQTFY1 = "";
                string YFQTFY = "";
                decimal SumYFQTFY1 = 0;
                decimal SumYFQTFY2 = 0;
                //List<string> ArrYFQTFY = new List<string> { };
                //var Q_ArrMBLFeeYF = Where_ArrMBLFee.Where(x => ArrYFQTFY.Contains(x.Key) && !x.IsAr && x.type);

                //if (Q_ArrMBLFeeYF.Any())
                //{
                //    YFQTFY2 = string.Join(" ", Q_ArrMBLFeeYF.Select(x => x.Key + ":" + x.Money_Code + x.Unitprice2.ToString() + "*" + x.Qty.ToString() + "=" + x.Money_Code + x.Account2));
                //    SumYFQTFY2 = Q_ArrMBLFeeYF.Sum(x => x.Account2);
                //}

                List<string> NoArrYFQTFY1 = new List<string> { "空运费", "战争附加险", "燃油附加费", "卡车费", "杂费", "制单费", "信息费", "CCA费用", "仓储费", "报关费", "关封费", "运输费", "代理费", "运抵费", "CHC费用" };
                for (var i = 0; i < NoArrYFQTFY1.Count; i++)
                {
                    ArrFeeType2.Remove(NoArrYFQTFY1[i]);
                }
                var ArrYFQTFY1 = ArrFeeType2;
                //List<string> ArrYFQTFY1 = new List<string> { "垫板费", "刻章费", "托盘费", "分单费", "运抵费", "燃油费", "其他费用", "燃料费", "过路费", "CHC费用", "报关联单费", "查验费", "操作费", "附加费", "单证费", "消磁服务费", "鉴定费", "出口操作费", "港务费", "搞定费", "快递费", "翻板费" };

                Q_ArrMBLFee = Where_ArrMBLFee.Where(x => ArrYFQTFY1.Contains(x.Key) && !x.type);

                Q_ArrMBLFee.Select(x => x.Key + ":" + x.Money_Code + x.Account2 + " ").ToList().ForEach((x, y) =>
                {
                    c.YFQTFY += x;
                    if ((y + 1) % 2 == 0)
                        c.YFQTFY += "\r\n";
                });
                //c.YFQTFY = string.Join("", NewQ_ArrMBLFee);

                //应付汇总
                //decimal YFAccountSum = Sum;
                var Where_ArrMBLFeeYFHZ = ArrMBLFeeNewYFHZ.Where(x => !x.IsAr);
                Q_ArrMBLFee = Where_ArrMBLFeeYFHZ.Where(x => (ArrYFQTFY1.Contains(x.Key) || ArrYFDLF.Contains(x.Key) || ArrYFCDF.Contains(x.Key) || ArrYFBBF.Contains(x.Key) || ArrYFBBF1.Contains(x.Key) || ArrYFCCF.Contains(x.Key) || ArrFee1.Contains(x.Key) || ArrFee2.Contains(x.Key)));
                var GroupMoneycodeArYFHZ = (from p in Where_ArrMBLFeeYFHZ
                                            group p by p.Money_Code into g
                                            select new
                                            {
                                                Money_Code = g.Max(x => x.Money_Code) == "" ? "CNY" : g.Max(x => x.Money_Code),
                                                Rate = g.Max(x => x.Rate),
                                                sumAccount2 = g.Sum(x => x.Account2),

                                            }).ToList();
                var FLHZ = string.Join(" ", GroupMoneycodeArYFHZ.Select(x => x.Money_Code + ":" + x.sumAccount2.ToString()));
                var Qsum = GroupMoneycodeArYFHZ.Select(x => x.sumAccount2 * x.Rate).Sum();
                c.YFAccountSum = FLHZ == "" ? "" : FLHZ;

                //应收空运费
                decimal SumYSKYF = 0;
                var Where_ArrMBLFeeAr = ArrMBLFeeNew.Where(x => x.IsAr);
                List<string> ArrFeeAr1 = new List<string> { "空运费", "战争附加险", "燃油附加费" };
                List<string> ArrFeeAr2 = new List<string> { "制单费", "信息费", "杂费" };
                var Q_ArrMBLFeeAr = Where_ArrMBLFeeAr.Where(x => ArrFeeAr1.Contains(x.Key) && x.type);
                var Q_ArrMBLFee2 = Where_ArrMBLFeeAr.Where(x => ArrFeeAr1.Contains(x.Key) || ArrFeeAr2.Contains(x.Key));
                var Q_ArrMBLFeeAr3 = Where_ArrMBLFeeAr.Where(x => ArrFeeAr1.Contains(x.Key) && x.type);
                var SumfeeAr = Q_ArrMBLFee.Where(x => x.Money_Code == "").Sum(x => x.Account2);
                var GroupMoneycodeAr1 = (from p in Q_ArrMBLFee2
                                         group p by p.Money_Code into g
                                         select new
                                         {
                                             Money_Code = g.Max(x => x.Money_Code),
                                             Rate = g.Max(x => x.Rate),
                                             sumAccount2 = g.Sum(x => x.Account2),

                                         }).ToList();
                decimal SumFeeAr = Q_ArrMBLFeeAr.Sum(x => x.Account2);
                var sumAr = string.Join("+", GroupMoneycodeAr1.Select(x => x.Money_Code + x.sumAccount2));
                c.YSKYF = string.Join("+", Q_ArrMBLFeeAr.OrderBy(x => x.RankingNo).Select(x => x.Money_Code + ((x.Qty * x.Unitprice2 == x.Account2) ? (x.Unitprice2 + "*" + x.Qty) : (x.Account2).ToString())));
                Q_ArrMBLFeeAr = ArrMBLFeeNew.Where(x => ArrFeeAr2.Contains(x.Key) && x.IsAr && !x.type);
                var GroupMoneycodeAr2 = from p in Q_ArrMBLFeeAr
                                        group p by p.Money_Code into g
                                        select new
                                        {
                                            Money_Code = g.Max(x => x.Money_Code),
                                            Rate = g.Max(x => x.Rate),
                                            sumAccount2 = g.Sum(x => x.Account2),

                                        };
                if (Q_ArrMBLFeeAr.Any() && Q_ArrMBLFeeAr3.Any())
                {
                    SumYSKYF += Q_ArrMBLFeeAr.Sum(x => x.Account2);
                    c.YSKYF += "+" + string.Join("+", Q_ArrMBLFeeAr.Select(x => x.Money_Code + x.Account2));
                }
                else
                {
                    c.YSKYF += string.Join("+", Q_ArrMBLFeeAr.Select(x => x.Money_Code + x.Account2));
                }
                c.YSKYF += SumFeeAr == 0 ? "" : (Q_ArrMBLFeeAr3.Count() == 1 && Q_ArrMBLFeeAr3.Any(x => x.Qty * x.Unitprice2 != x.Account2)) ? "" : ("=" + sumAr);


                //应收报关费
                string YFBBFAR = "";
                string SumYFBBFAR = "";
                List<string> ArrYSBBF = new List<string> { "报关费" };
                List<string> ArrYSBBF1 = new List<string> { "报关联单费" };
                Q_ArrMBLFeeAr = Where_ArrMBLFeeAr.Where(x => ArrYSBBF.Contains(x.Key) && x.type);
                Q_ArrMBLFee2 = Where_ArrMBLFeeAr.Where(x => ArrYSBBF.Contains(x.Key) || ArrYSBBF1.Contains(x.Key));
                Q_ArrMBLFeeAr3 = Where_ArrMBLFeeAr.Where(x => ArrYSBBF.Contains(x.Key) && x.type);
                SumfeeAr = Q_ArrMBLFee.Where(x => x.Money_Code == "").Sum(x => x.Account2);
                GroupMoneycodeAr1 = (from p in Q_ArrMBLFee2
                                     group p by p.Money_Code into g
                                     select new
                                     {
                                         Money_Code = g.Max(x => x.Money_Code),
                                         Rate = g.Max(x => x.Rate),
                                         sumAccount2 = g.Sum(x => x.Account2),

                                     }).ToList();
                SumFeeAr = Q_ArrMBLFeeAr.Sum(x => x.Account2);
                sumAr = string.Join("+", GroupMoneycodeAr1.Select(x => x.Money_Code + x.sumAccount2));
                c.YSBBF = string.Join("+", Q_ArrMBLFeeAr.Select(x => x.Money_Code + ((x.Qty * x.Unitprice2 == x.Account2) ? (x.Unitprice2 + "*" + x.Qty) : (x.Account2).ToString())));
                Q_ArrMBLFeeAr = ArrMBLFeeNew.Where(x => ArrYSBBF1.Contains(x.Key) && x.IsAr && !x.type);
                GroupMoneycodeAr2 = from p in Q_ArrMBLFeeAr
                                    group p by p.Money_Code into g
                                    select new
                                    {
                                        Money_Code = g.Max(x => x.Money_Code),
                                        Rate = g.Max(x => x.Rate),
                                        sumAccount2 = g.Sum(x => x.Account2),

                                    };
                if (Q_ArrMBLFeeAr.Any() && Q_ArrMBLFeeAr3.Any())
                {
                    SumYSKYF += Q_ArrMBLFeeAr.Sum(x => x.Account2);
                    c.YSBBF += "+" + string.Join("+", Q_ArrMBLFeeAr.Select(x => x.Money_Code + x.Account2));
                }
                else
                {
                    c.YSBBF += string.Join("+", Q_ArrMBLFeeAr.Select(x => x.Money_Code + x.Account2));
                }
                c.YSBBF += SumFeeAr == 0 ? "" : (Q_ArrMBLFeeAr3.Count() == 1 && Q_ArrMBLFeeAr3.Any(x => x.Qty * x.Unitprice2 != x.Account2)) ? "" : ("=" + sumAr);
                //Q_ArrMBLFeeAr = Where_ArrMBLFeeAr.Where(x => ArrYSBBF.Contains(x.Key) && x.type);
                ////var count3 = Q_ArrMBLFee.Count();
                //GroupMoneycodeAr1 = (from p in Q_ArrMBLFeeAr
                //                     group p by p.Money_Code into g
                //                     select new
                //                     {
                //                         Money_Code = g.Max(x => x.Money_Code),
                //                         Rate = g.Max(x => x.Rate),
                //                         sumAccount2 = g.Sum(x => x.Account2),

                //                     }).ToList();
                //sumAr = string.Join("+", GroupMoneycodeAr1.Select(x => x.Money_Code + x.sumAccount2));
                //c.YFBBF = string.Join("+", Q_ArrMBLFeeAr.Select(x => x.Money_Code + x.Unitprice2 + "*" + x.Qty));
                //if (Q_ArrMBLFeeAr.Any())
                //{
                //    SumYFBBFAR = string.Join("+", GroupMoneycodeAr1.Select(x => x.Money_Code + x.sumAccount2));
                //    YFBBFAR = string.Join("+", Q_ArrMBLFeeAr.Select(x => x.Money_Code + x.Unitprice2.ToString() + "*" + x.Qty.ToString())) + "=" + SumYFBBFAR;
                //}
                //Q_ArrMBLFeeAr = ArrMBLFeeNew.Where(x => ArrYSBBF1.Contains(x.Key) && x.IsAr && !x.type);
                //GroupMoneycodeAr2 = (from p in Q_ArrMBLFeeAr
                //                     group p by p.Money_Code into g
                //                     select new
                //                     {
                //                         Money_Code = g.Max(x => x.Money_Code),
                //                         Rate = g.Max(x => x.Rate),
                //                         sumAccount2 = g.Sum(x => x.Account2),

                //                     }).ToList();
                //if (Q_ArrMBLFeeAr.Any())
                //{
                //    SumYFBBFAR = string.Join("+", GroupMoneycodeAr2.Select(x => x.Money_Code + x.sumAccount2));
                //    //YFBBFAR = string.Join("+", Q_ArrMBLFeeAr.Select(x => x.Money_Code + x.Unitprice2.ToString() + "*" + x.Qty.ToString())) + "=" + SumYFBBFAR;
                //}
                //c.YSBBF = YFBBFAR;

                //应收车队费
                string YSCDF = "";
                string SumYSCDF = "";
                List<string> ArrYSCDF = new List<string> { "运输费" };
                Q_ArrMBLFeeAr = Where_ArrMBLFeeAr.Where(x => ArrYSCDF.Contains(x.Key) && !x.type);
                GroupMoneycodeAr1 = (from p in Q_ArrMBLFeeAr
                                     group p by p.Money_Code into g
                                     select new
                                     {
                                         Money_Code = g.Max(x => x.Money_Code),
                                         Rate = g.Max(x => x.Rate),
                                         sumAccount2 = g.Sum(x => x.Account2),

                                     }).ToList();
                SumYSCDF = string.Join("+", GroupMoneycodeAr1.Select(x => x.Money_Code + x.sumAccount2));
                c.YSCDF = SumYSCDF.ToString();

                //应收代理费
                string YSDLF = "";
                string SumYSDLF = "";
                List<string> ArrYSDLF = new List<string> { "代理费" };
                Q_ArrMBLFeeAr = Where_ArrMBLFeeAr.Where(x => ArrYSDLF.Contains(x.Key) && !x.type);
                GroupMoneycodeAr1 = (from p in Q_ArrMBLFeeAr
                                     group p by p.Money_Code into g
                                     select new
                                     {
                                         Money_Code = g.Max(x => x.Money_Code),
                                         Rate = g.Max(x => x.Rate),
                                         sumAccount2 = g.Sum(x => x.Account2),

                                     }).ToList();
                SumYSDLF = string.Join("+", GroupMoneycodeAr1.Select(x => x.Money_Code + x.sumAccount2));
                c.YSDLF = SumYSDLF.ToString();

                //应付其他费
                string YSQTFY2 = "";
                string YSQTFY1 = "";
                string YSQTFY = "";
                decimal SumYSQTFY1 = 0;
                decimal SumYSQTFY2 = 0;
                List<string> ArrYSQTFY = new List<string> { "燃油费" };
                Q_ArrMBLFeeAr = Where_ArrMBLFeeAr.Where(x => ArrYSQTFY.Contains(x.Key) && x.IsAr && x.type);
                if (Q_ArrMBLFeeAr.Any())
                {
                    if (Q_ArrMBLFeeAr.Count() == 1 && Q_ArrMBLFeeAr.Any(x => x.Qty * x.Unitprice2 != x.Account2))
                    {
                        YSQTFY2 = string.Join(" ", Q_ArrMBLFeeAr.Select(x => x.Key + ":" + x.Money_Code + ((x.Qty * x.Unitprice2 == x.Account2) ? (x.Unitprice2 + "*" + x.Qty) : (x.Account2).ToString())));
                    }
                    else
                        YSQTFY2 = string.Join(" ", Q_ArrMBLFeeAr.Select(x => x.Key + ":" + x.Money_Code + ((x.Qty * x.Unitprice2 == x.Account2) ? (x.Unitprice2 + "*" + x.Qty) : (x.Account2).ToString()) + "=" + x.Account2));
                    SumYSQTFY2 = Q_ArrMBLFeeAr.Sum(x => x.Account2);
                }

                List<string> NoArrYSQTFY1 = new List<string> { "空运费", "战争附加险", "燃油附加费", "制单费", "信息费", "杂费", "报关费", "报关联单费", "运输费", "代理费", "燃油费" };
                for (var i = 0; i < NoArrYSQTFY1.Count; i++)
                {
                    ArrFeeType3.Remove(NoArrYSQTFY1[i]);
                }
                var ArrYSQTFY1 = ArrFeeType3;
                //List<string> ArrYSQTFY1 = new List<string> { "垫板费", "刻章费", "托盘费", "关封费", "仓储费", "卡车费", "CCA费用", "运抵费", "燃料费", "其他费用", "过路费", "CHC费用", "查验费", "操作费", "附加费", "单证费", "消磁服务费", "鉴定费", "出口操作费", "港务费", "搞定费", "快递费", "翻板费", "分单费" };
                Q_ArrMBLFeeAr = Where_ArrMBLFeeAr.Where(x => ArrYSQTFY1.Contains(x.Key) && !x.type);

                //c.YSQTFY = string.Join("\r\n", Q_ArrMBLFeeAr.Select(x => x.Key + ":" + x.Money_Code + x.Account2)) + " " + YSQTFY2;

                Q_ArrMBLFeeAr.Select(x => x.Key + ":" + x.Money_Code + x.Account2 + " ").ToList().ForEach((x, y) =>
                {
                    c.YSQTFY += x;
                    if ((y + 1) % 2 == 0)
                        c.YSQTFY += "\r\n";
                });
                c.YSQTFY = c.YSQTFY + YSQTFY2;
                //应收汇总
                //decimal YSAccountSum = SumAr;
                Q_ArrMBLFeeAr = Where_ArrMBLFeeAr.Where(x => (ArrYSBBF1.Contains(x.Key) || ArrYSCDF.Contains(x.Key) || ArrYSBBF.Contains(x.Key) || ArrYSQTFY1.Contains(x.Key) || ArrYSQTFY.Contains(x.Key) || ArrYSDLF.Contains(x.Key) || ArrFeeAr1.Contains(x.Key) || ArrFeeAr2.Contains(x.Key)) && x.IsAr);
                var GroupMoneycodeArYSHZ = (from p in Q_ArrMBLFeeAr
                                            group p by p.Money_Code into g
                                            select new
                                            {
                                                Money_Code = g.Max(x => x.Money_Code) == "" ? "CNY" : g.Max(x => x.Money_Code),
                                                Rate = g.Max(x => x.Rate),
                                                sumAccount2 = g.Sum(x => x.Account2),

                                            }).ToList();
                var FLHZAR = string.Join(" ", GroupMoneycodeArYSHZ.Select(x => x.Money_Code + ":" + x.sumAccount2.ToString()));
                var QQSUM = GroupMoneycodeArYSHZ.Select(x => x.sumAccount2 * x.Rate).Sum();
                c.YSAccountSum = FLHZAR == "" ? "" : FLHZAR;

                //毛利
                string YFYSprofit = "";
                c.YFYSprofit = (QQSUM - Qsum) == 0 ? "" : Convert.ToDecimal((QQSUM - Qsum)).ToString("#0.00");
                c.YFTOTAL = Qsum == 0 ? "总计：" : "总计：" + Convert.ToDecimal(Qsum).ToString("#0.00");
                c.YSTOTAL = QQSUM == 0 ? "总计：" : "总计：" + Convert.ToDecimal(QQSUM).ToString("#0.00");
                ArrPdfFile.Add(c);
            }

            #region 获取Excel中字段位置

            Dictionary<string, Tuple<int, int>> dict = new Dictionary<string, Tuple<int, int>>() { 
             {"CustomName",new Tuple<int,int>(1,2)},
             {"ADDWHO",new Tuple<int,int>(1,6)},
             {"Dhbz",new Tuple<int,int>(1,10)},
             {"Is_Self1",new Tuple<int,int>(5,3)},
             {"Is_Self2",new Tuple<int,int>(3,3)},
             {"Book_Flat_Code",new Tuple<int,int>(8,2)},
             {"End_Port",new Tuple<int,int>(8,9)},
             {"MBL",new Tuple<int,int>(9,2)},
             {"HBL",new Tuple<int,int>(9,9)},
             {"Starttime",new Tuple<int,int>(10,2)},
             {"YDpay",new Tuple<int,int>(10,9)},
             {"MPieces_DC",new Tuple<int,int>(11,3)},
             {"MWeight_DC",new Tuple<int,int>(11,6)},
             {"MVolume_DC",new Tuple<int,int>(11,9)},
             {"MCharge_Weight_DC",new Tuple<int,int>(11,11)},
             {"SPieces_SK",new Tuple<int,int>(12,3)},
             {"SWeight_SK",new Tuple<int,int>(12,6)},
             {"SVolume_SK",new Tuple<int,int>(12,9)},
             {"SCharge_Weight_SK",new Tuple<int,int>(12,11)},
             {"FPieces_Fact",new Tuple<int,int>(13,3)},
             {"FWeight_Fact",new Tuple<int,int>(13,6)},
             {"FVolume_Fact",new Tuple<int,int>(13,9)},
             {"FCharge_Weight_Fact",new Tuple<int,int>(13,11)},
             {"HPieces_SK",new Tuple<int,int>(14,3)},
             {"HWeight_DC",new Tuple<int,int>(14,6)},
             {"HVolume_SK",new Tuple<int,int>(14,9)},
             {"HCharge_Weight_SK",new Tuple<int,int>(14,11)},
             {"Is_XC",new Tuple<int,int>(17,3)},
             {"IS_Checked_BG",new Tuple<int,int>(18,3)},
             {"Num_BGPH",new Tuple<int,int>(18,9)},
             {"Num_BGYTH",new Tuple<int,int>(19,9)},
             {"LDQTY",new Tuple<int,int>(19,3)},
             {"YFKYF",new Tuple<int,int>(26,2)},
             {"YFCCF",new Tuple<int,int>(28,2)},
             {"YFBGF",new Tuple<int,int>(30,2)},
             {"YFCDF",new Tuple<int,int>(32,2)},
             {"YFDLF",new Tuple<int,int>(34,2)},
             {"YFQTFY",new Tuple<int,int>(36,2)},
             {"YFAccountSum",new Tuple<int,int>(38,2)},
             {"YSKYF",new Tuple<int,int>(28,9)},
             {"YSBGF",new Tuple<int,int>(30,9)},
             {"YSCDF",new Tuple<int,int>(32,9)},
             {"YSDLF",new Tuple<int,int>(34,9)},
             {"YSQTFY",new Tuple<int,int>(36,9)},
             {"YSAccountSum",new Tuple<int,int>(38,9)},
             {"YFYSprofit",new Tuple<int,int>(40,9)},
             {"Num_BGZBG",new Tuple<int,int>(17,9)},
             {"Num_NoTG",new Tuple<int,int>(3,9)},
             {"YFTOTAL",new Tuple<int,int>(39,2)},
             {"YSTOTAL",new Tuple<int,int>(39,9)}

            };

            #endregion

            //Aspose.Cells.Workbook workbook1 = new Aspose.Cells.Workbook();
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(Server.MapPath("/FileModel/封面Excel.xls"));
            //var sheet1 = workbook.Worksheets.Add("123");
            //sheet1 = workbook1.Worksheets[0];

            List<object> Arr = new List<object>();
            Arr.AddRange();
            for (int i = 0; i < ArrPdfFile.Count(); i++)
            {
                if (i > 0)
                {
                    //var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                    //Aspose.Cells.Worksheet Owsheet = new Aspose.Cells.Worksheet();
                    //Owsheet.Copy(sheet);
                    workbook.Worksheets.AddCopy(0);

                }
                var sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[workbook.Worksheets.Count - 1];
                sheet.Name = "封面" + (i + 1);
                var objValue = ArrPdfFile[i];

                #region 插入数据
                var CUS = dict.Where(x => x.Key == "CustomName");
                if (CUS.Any())
                {
                    var Tobj = CUS.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.CusBusName;
                }
                var ADD = dict.Where(x => x.Key == "ADDWHO");
                if (ADD.Any())
                {
                    var Tobj = ADD.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.ADDWHO;
                }
                var Dhbz = dict.Where(x => x.Key == "Dhbz");
                if (Dhbz.Any())
                {
                    var Tobj = Dhbz.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Dzbh;
                }
                var Is_Self1 = dict.Where(x => x.Key == "Is_Self1");
                if (Is_Self1.Any())
                {
                    var Tobj = Is_Self1.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Is_Self1;
                }
                var Is_Self2 = dict.Where(x => x.Key == "Is_Self2");
                if (Is_Self2.Any())
                {
                    var Tobj = Is_Self2.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Is_Self2;
                }
                var Book_Flat_Code = dict.Where(x => x.Key == "Book_Flat_Code");
                if (Book_Flat_Code.Any())
                {
                    var Tobj = Book_Flat_Code.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Book_Flat_Code;
                }
                var End_Port = dict.Where(x => x.Key == "End_Port");
                if (End_Port.Any())
                {
                    var Tobj = End_Port.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.End_Port;
                }
                var MBL = dict.Where(x => x.Key == "MBL");
                if (MBL.Any())
                {
                    var Tobj = MBL.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.MBL;
                }
                var HBLD = dict.Where(x => x.Key == "HBL");
                if (HBLD.Any())
                {
                    var Tobj = HBLD.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.HBL;
                }
                var Starttime = dict.Where(x => x.Key == "Starttime");
                if (End_Port.Any())
                {
                    var Tobj = Starttime.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Starttime;
                }
                var YDpayE = dict.Where(x => x.Key == "YDpay");
                if (End_Port.Any())
                {
                    var Tobj = YDpayE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YDpay;
                }
                var MPieces_DC = dict.Where(x => x.Key == "MPieces_DC");
                if (MPieces_DC.Any())
                {
                    var Tobj = MPieces_DC.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.MPieces_DC;
                }
                var MWeight_DC = dict.Where(x => x.Key == "MWeight_DC");
                if (MWeight_DC.Any())
                {
                    var Tobj = MWeight_DC.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.MWeight_DC;
                }
                var MVolume_DC = dict.Where(x => x.Key == "MVolume_DC");
                if (MVolume_DC.Any())
                {
                    var Tobj = MVolume_DC.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.MVolume_DC;
                }
                var MCharge_Weight_DC = dict.Where(x => x.Key == "MCharge_Weight_DC");
                if (MCharge_Weight_DC.Any())
                {
                    var Tobj = MCharge_Weight_DC.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.MCharge_Weight_DC;
                }
                var SPieces_SK = dict.Where(x => x.Key == "SPieces_SK");
                if (SPieces_SK.Any())
                {
                    var Tobj = SPieces_SK.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.SPieces_SK;
                }
                var SWeight_SK = dict.Where(x => x.Key == "SWeight_SK");
                if (SWeight_SK.Any())
                {
                    var Tobj = SWeight_SK.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.SWeight_SK;
                }
                var SVolume_SK = dict.Where(x => x.Key == "SVolume_SK");
                if (SVolume_SK.Any())
                {
                    var Tobj = SVolume_SK.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.SVolume_SK;
                }
                var SCharge_Weight_SK = dict.Where(x => x.Key == "SCharge_Weight_SK");
                if (SCharge_Weight_SK.Any())
                {
                    var Tobj = SCharge_Weight_SK.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.SCharge_Weight_SK;
                }
                var FPieces_Fact = dict.Where(x => x.Key == "FPieces_Fact");
                if (FPieces_Fact.Any())
                {
                    var Tobj = FPieces_Fact.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.FPieces_Fact;
                }
                var FWeight_Fact = dict.Where(x => x.Key == "FWeight_Fact");
                if (FWeight_Fact.Any())
                {
                    var Tobj = FWeight_Fact.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.FWeight_Fact;
                }
                var FVolume_Fact = dict.Where(x => x.Key == "FVolume_Fact");
                if (FVolume_Fact.Any())
                {
                    var Tobj = FVolume_Fact.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.FVolume_Fact;
                }
                var FCharge_Weight_Fact = dict.Where(x => x.Key == "FCharge_Weight_Fact");
                if (FCharge_Weight_Fact.Any())
                {
                    var Tobj = FCharge_Weight_Fact.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.FCharge_Weight_Fact;
                }
                var HPieces_SK = dict.Where(x => x.Key == "HPieces_SK");
                if (HPieces_SK.Any())
                {
                    var Tobj = HPieces_SK.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.HPieces_SK;
                }
                var HWeight_DC = dict.Where(x => x.Key == "HWeight_DC");
                if (HWeight_DC.Any())
                {
                    var Tobj = HWeight_DC.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.HWeight_DC;
                }
                var HVolume_SK = dict.Where(x => x.Key == "HVolume_SK");
                if (HVolume_SK.Any())
                {
                    var Tobj = HVolume_SK.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.HVolume_SK;
                }
                var HCharge_Weight_SK = dict.Where(x => x.Key == "HCharge_Weight_SK");
                if (HCharge_Weight_SK.Any())
                {
                    var Tobj = HCharge_Weight_SK.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.HCharge_Weight_SK;
                }
                var Is_XC = dict.Where(x => x.Key == "Is_XC");
                if (Is_XC.Any())
                {
                    var Tobj = Is_XC.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Is_XC;
                }
                var IS_Checked_BG = dict.Where(x => x.Key == "IS_Checked_BG");
                if (IS_Checked_BG.Any())
                {
                    var Tobj = IS_Checked_BG.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.IS_Checked_BG;
                }
                var Num_NoTG = dict.Where(x => x.Key == "Num_NoTG");
                if (Num_NoTG.Any())
                {
                    var Tobj = Num_NoTG.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Num_NoTG;
                }
                var Num_BGZBG = dict.Where(x => x.Key == "Num_BGZBG");
                if (Num_BGZBG.Any())
                {
                    var Tobj = Num_BGZBG.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Num_BGZBG;
                }
                var Num_BGPH = dict.Where(x => x.Key == "Num_BGPH");
                if (Num_BGPH.Any())
                {
                    var Tobj = Num_BGPH.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Num_BGPH;
                }
                var Num_BGYTH = dict.Where(x => x.Key == "Num_BGYTH");
                if (Num_BGYTH.Any())
                {
                    var Tobj = Num_BGYTH.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.Num_BGYTH;
                }
                var LDQTY = dict.Where(x => x.Key == "LDQTY");
                if (LDQTY.Any())
                {
                    var Tobj = LDQTY.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.LDQTY;
                }
                var YFKYFE = dict.Where(x => x.Key == "YFKYF");
                if (YFKYFE.Any())
                {
                    var Tobj = YFKYFE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YFKYF;
                }
                var YFCCFE = dict.Where(x => x.Key == "YFCCF");
                if (YFCCFE.Any())
                {
                    var Tobj = YFCCFE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YFCCF;
                }
                var YFBGFE = dict.Where(x => x.Key == "YFBGF");
                if (YFBGFE.Any())
                {
                    var Tobj = YFBGFE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YFBBF;
                }
                var YFCDFE = dict.Where(x => x.Key == "YFCDF");
                if (YFCDFE.Any())
                {
                    var Tobj = YFCDFE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YFCDF;
                }
                var YFDLFE = dict.Where(x => x.Key == "YFDLF");
                if (YFDLFE.Any())
                {
                    var Tobj = YFDLFE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YFDLF;
                }
                var YFQTFYE = dict.Where(x => x.Key == "YFQTFY");
                if (YFQTFYE.Any())
                {
                    var Tobj = YFQTFYE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YFQTFY;
                }
                var YFAccountSumE = dict.Where(x => x.Key == "YFAccountSum");
                if (YFAccountSumE.Any())
                {
                    var Tobj = YFAccountSumE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YFAccountSum;
                }
                var YSKYFE = dict.Where(x => x.Key == "YSKYF");
                if (YSKYFE.Any())
                {
                    var Tobj = YSKYFE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YSKYF;
                }
                var YSBGFE = dict.Where(x => x.Key == "YSBGF");
                if (YSBGFE.Any())
                {
                    var Tobj = YSBGFE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YSBBF;
                }
                var YSCDFE = dict.Where(x => x.Key == "YSCDF");
                if (YSCDFE.Any())
                {
                    var Tobj = YSCDFE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YSCDF;
                }
                var YSDLFE = dict.Where(x => x.Key == "YSDLF");
                if (YSDLFE.Any())
                {
                    var Tobj = YSDLFE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YSDLF;
                }
                var YSQTFYE = dict.Where(x => x.Key == "YSQTFY");
                if (YSQTFYE.Any())
                {
                    var Tobj = YSQTFYE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YSQTFY;
                }
                var YSAccountSumE = dict.Where(x => x.Key == "YSAccountSum");
                if (YSAccountSumE.Any())
                {
                    var Tobj = YSAccountSumE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YSAccountSum;
                }
                var YFYSprofitE = dict.Where(x => x.Key == "YFYSprofit");
                if (YFYSprofitE.Any())
                {
                    var Tobj = YFYSprofitE.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YFYSprofit;
                }
                var YSTOTAL = dict.Where(x => x.Key == "YSTOTAL");
                if (YSTOTAL.Any())
                {
                    var Tobj = YSTOTAL.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YSTOTAL;
                }
                var YFTOTAL = dict.Where(x => x.Key == "YFTOTAL");
                if (YFTOTAL.Any())
                {
                    var Tobj = YFTOTAL.First().Value;
                    sheet.Cells[Tobj.Item1, Tobj.Item2].Value = objValue.YFTOTAL;
                }
                #endregion
            }

            var fileName = typeName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var dPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "/FileDownLoad/";
            string FName = Server.MapPath(dPath + "/ExlDD/" + fileName);
            workbook.Save(FName);

            //System.IO.FileInfo oo = new System.IO.FileInfo(FName);
            //application/pdf;application/octet-stream; 
            return File(FName, "application/octet-stream", fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static void RemovePageBreaks(Aspose.Words.Document doc)
        {
            // Retrieve all paragraphs in the document.
            Aspose.Words.NodeCollection paragraphs = doc.GetChildNodes(Aspose.Words.NodeType.Paragraph, true);

            // Iterate through all paragraphs
            foreach (Aspose.Words.Paragraph para in paragraphs)
            {
                // If the paragraph has a page break before set then clear it.
                if (para.ParagraphFormat.PageBreakBefore)
                    para.ParagraphFormat.PageBreakBefore = false;

                // Check all runs in the paragraph for page breaks and remove them.
                foreach (Aspose.Words.Run run in para.Runs)
                {
                    if (run.Text.Contains(Aspose.Words.ControlChar.PageBreak))
                        run.Text = run.Text.Replace(Aspose.Words.ControlChar.PageBreak, string.Empty);
                }

            }

        }

        //PDF,EXCEL报表导出临时类
        #region  PDF,EXCEL报表导出临时类

        /// <summary>
        /// 费用计算 临时类
        /// </summary>
        public class Fee
        {
            public Fee(string _Key, bool _type, decimal _Qty, decimal _Unitprice2, decimal _Account2, string _Money_Code, decimal _Rate, int _RankingNo, bool _IsAr = false)
            {
                Key = _Key;
                type = _type;
                Qty = _Qty;
                Unitprice2 = _Unitprice2;
                Account2 = _Account2;
                Money_Code = _Money_Code;
                Rate = _Rate;
                RankingNo = _RankingNo;
                //Flight_Date_Want = _Flight_Date_Want;
                IsAr = _IsAr;
            }
            public string Key { get; set; }//费用类型
            public bool IsAr { get; set; }//应收数据
            public bool type { get; set; }//乘法数据
            public decimal Qty { get; set; }
            public decimal Unitprice2 { get; set; }
            public decimal Account2 { get; set; }
            public string Money_Code { get; set; }
            public decimal Rate { get; set; }
            public int RankingNo { get; set; }
            //public DateTime? Flight_Date_Want { get; set; }

        }

        public class ADHOC
        {
            public string Now_Date { get; set; }
            public string MBL { get; set; }
            public string Agent_Name { get; set; }
            public string End_Port { get; set; }
            public string Flight_Date_Want { get; set; }
            public decimal? Weight_DC { get; set; }
            public decimal? Volume_DC { get; set; }
            public string Rate1 { get; set; }
            public string AdhocRate { get; set; }

        }

        public class Book_Fligt
        {
            public string FltNoDate { get; set; }
            public string Dest { get; set; }
            public string Prefix { get; set; }
            public string AWBNO { get; set; }
            public decimal? GWT { get; set; }
            public decimal? VOL { get; set; }
            public string Q7 { get; set; }
            public string Q6 { get; set; }
            public string X8 { get; set; }
            public string A2 { get; set; }
            public string AKE { get; set; }
            public string PEB { get; set; }
            public string BULK { get; set; }
            public string BUremark { get; set; }
            public string ConxFlts { get; set; }
            public decimal? PCS { get; set; }
            public string Dimension { get; set; }
            public string ADHOC { get; set; }

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

        public class AccountTotal
        {
            public string Flight_Date_Want { get; set; }//开航日期
            public string Money_Code { get; set; }//币种
            public decimal Rate { get; set; }//汇率
            public decimal? Bill_Accounts { get; set; }//金额
            public decimal Bill_TaxAmount { get; set; }//税金
            public decimal Bill_AmountTaxTotal { get; set; }//价税合计

        }

        //费用明细报表
        public class BMSARDTL
        {
            public string Operation_Id { get; set; }
            public string MBL { get; set; }
            public string Flight_Date_Want { get; set; }
            public string Settle_Code { get; set; }
            public string Charge_Desc { get; set; }
            public string Money_Code { get; set; }
            public decimal? Bill_Account2 { get; set; }
            public string Sumbmit_No { get; set; }
            public string Sumbmit_No_Org { get; set; }
            public string End_Port { get; set; }


        }

        public class Bms_ar_ap_account
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

        public class Bms_ar_ap_account_HasTax
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

        public class OperationList
        {
            public string Operation_Id { get; set; }
            public string Consign_Code { get; set; }
            public string MBL { get; set; }
            public string Flight_Date_Want { get; set; }
            public string End_Port { get; set; }
            public decimal? Pieces_Fact { get; set; }
            public decimal? Weight_Fact { get; set; }
            public decimal? Volume_Fact { get; set; }
            public decimal? Pieces_SK { get; set; }
            public decimal? Weight_SK { get; set; }
            public decimal? Volume_SK { get; set; }
            public string Remark { get; set; }
        }

        public class SpellingList
        {
            public string Operation_Id { get; set; }
            public string Consign_Code { get; set; }
            public int? RK { get; set; }
            public string MBL { get; set; }
            public string HBL { get; set; }
            public string End_Port { get; set; }
            public string Incidental_Expenses_M { get; set; }
            public string HPieces { get; set; }
            public string HWeight { get; set; }
            public string HVolume { get; set; }
            public string Flight_Date_Want { get; set; }
            public string Bragainon_Article_H { get; set; }
            public decimal? Weight_BG { get; set; }
            public string Remark { get; set; }
            public string Delivery_Point_Book_Flat_Code { get; set; }


        }

        public class TObjTest
        {
            public string CusBusName { get; set; }
            public string Dzbh { get; set; }
            public string ADDWHO { get; set; }
            public string Book_Flat_Code { get; set; }
            public string End_Port { get; set; }
            public string MBL { get; set; }
            public string HBL { get; set; }
            public string Starttime { get; set; }
            public string YDpay { get; set; }
            public string Is_Self1 { get; set; }
            public string Is_Self2 { get; set; }
            public string MPieces_DC { get; set; }
            public string MWeight_DC { get; set; }
            public string MVolume_DC { get; set; }
            public string MCharge_Weight_DC { get; set; }
            public string SPieces_SK { get; set; }
            public string SWeight_SK { get; set; }
            public string SVolume_SK { get; set; }
            public string SCharge_Weight_SK { get; set; }
            public string FPieces_Fact { get; set; }
            public string FWeight_Fact { get; set; }
            public string FVolume_Fact { get; set; }
            public string FCharge_Weight_Fact { get; set; }
            public string HPieces_SK { get; set; }
            public string HWeight_DC { get; set; }
            public string HVolume_SK { get; set; }
            public string HCharge_Weight_SK { get; set; }
            public string KG_KG { get; set; }
            public string Is_XC { get; set; }
            public string Num_BGPH { get; set; }
            public string Num_BGYTH { get; set; }
            public string Num_BGZBG { get; set; }
            public string Num_NoTG { get; set; }
            public string IS_Checked_BG { get; set; }
            public string LDQTY { get; set; }
            public string YFKYF { get; set; }
            public string YSKYF { get; set; }
            public string YFCCF { get; set; }
            public string YFBBF { get; set; }
            public string YFSJF { get; set; }
            public string YFCDF { get; set; }
            public string YFDLF { get; set; }
            public string YFQTFY { get; set; }
            public string YSBBF { get; set; }
            public string YSSJF { get; set; }
            public string YSCDF { get; set; }
            public string YSDLF { get; set; }
            public string YSQTFY { get; set; }
            public string YFAccountSum { get; set; }
            public string YSAccountSum { get; set; }
            public string YFYSprofit { get; set; }
            public string YSTOTAL { get; set; }
            public string YFTOTAL { get; set; }


        }

        public class ExcelTS
        {
            public string Entrustment_Name { get; set; }
            public string Depart_PortEn { get; set; }
            public string Depart_Port { get; set; }
            public string End_PortEn { get; set; }
            public string End_Port { get; set; }
            public string DZbh { get; set; }
            public string Pieces_TS { get; set; }
            public string Weight_TS { get; set; }
            public string Volume_TS { get; set; }
            public string Flight_Date_Want { get; set; }
            public string Flight_No { get; set; }
            public string ADDWHO { get; set; }
            public string PrintDate { get; set; }
            public string Shipper_M { get; set; }
            public string Consignee_M { get; set; }
            public string Notify_Part_M { get; set; }
            public string Marks_H { get; set; }
            public string EN_Name_H { get; set; }

        }

        public class ExcelOP_H
        {
            public string MBL { get; set; }
            public string HBL { get; set; }
            public string Shipper_H { get; set; }
            public string Consignee_H { get; set; }
            public string Notify_Part_H { get; set; }
            public string End_Port { get; set; }
            public string End_PortEng { get; set; }
            public string Depart_Port { get; set; }
            public string Depart_PortEng { get; set; }
            public string Flight_No { get; set; }
            public string Flight_No1 { get; set; }
            public string Flight_Date_Want { get; set; }
            public string Flight_No_Flight_Date_Want { get; set; }
            public string Incidental_Expenses_H { get; set; }
            public string Declare_Value_Trans_H { get; set; }
            public string Declare_Value_Ciq_H { get; set; }
            public string Bragainon_Article_H { get; set; }
            public string Shipper_M { get; set; }
            public string Pieces_H { get; set; }
            public string Weight_H { get; set; }
            public string Volume_H { get; set; }
            public string Charge_Weight_H { get; set; }
            public string Marks_H { get; set; }
            public string Carriage_H_Bragainon_Article_H { get; set; }
            public string Carriage_H { get; set; }
            public string EN_Name_H { get; set; }
            public string Flight_No_Flight_Depart_PortEng { get; set; }
        }

        public class pdfCD
        {
            public string MBL { get; set; }
            public string HBL { get; set; }
            public string End_PortEng { get; set; }
            public string Depart_PortEng { get; set; }
            public string Flight_No { get; set; }
            public string FWD_Code { get; set; }
            public decimal? Pieces { get; set; }
            public decimal? Weight { get; set; }
            public string EN_Name_H { get; set; }
            public string End_Port { get; set; }
            public string Shipper_H { get; set; }
            public string Consignee_H { get; set; }
            public string Pay_Mode_H { get; set; }
        }

        public class pdfCDEx
        {
            public string HAWB { get; set; }
            public string Pieces { get; set; }
            public decimal? Weight { get; set; }
            public string EN_Name_H { get; set; }
            public string End_Port { get; set; }
            public string Shipper_H { get; set; }
            public string Consignee_H { get; set; }
            public string Pay_Mode_H { get; set; }
        }

        public class pdfCDTEST
        {
            public string MBL { get; set; }
            public string HBL { get; set; }
            public string End_PortEng { get; set; }
            public string Depart_PortEng { get; set; }
            public string Flight_No { get; set; }
            public string FWD_Code { get; set; }
            public decimal? Pieces { get; set; }
            public string Piecestest { get; set; }
            public decimal? Weight { get; set; }
            public string EN_Name_H { get; set; }
            public string End_Port { get; set; }
            public string Shipper_H { get; set; }
            public string Consignee_H { get; set; }
            public string Pay_Mode_H { get; set; }
        }

        #endregion

        /// <summary>
        /// 获取主单信息选择框
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSeltOPS_M_Ord()
        {
            ViewBag.Num = Convert.ToInt32(Request["Num"] ?? "1");
            var IsBmsBillArStr = (Request["IsBmsBillAr"] ?? "");
            bool? IsBmsBillAr = null;
            if (!string.IsNullOrEmpty(IsBmsBillArStr))
                IsBmsBillAr = Common.ChangStrToBool(IsBmsBillArStr);
            else
                ModelState.AddModelError("", "错误，未指定应收/应付 账单！");
            return PartialView("SeltOPS_M_Ord", IsBmsBillAr);
        }

        /// <summary>
        /// 应收/应付 获取数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBmsData(int page = 1, int rows = 10, string sort = "MBL", string order = "desc", string filterRules = "")
        {
            var OPS_BMS_StatusStr = Request["OPS_BMS_Status"] ?? "";
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);

            int totalCount = 0;
            var QOPS_M_Order = _unitOfWork.Repository<OPS_M_Order>().Queryable();
            var QOPS_EttInfor = _unitOfWork.Repository<OPS_EntrustmentInfor>().Queryable();
            var QResult = (from m in QOPS_M_Order
                           join p in QOPS_EttInfor on m.Id equals p.MBLId
                           select new
                           {
                               Ops_M_OrdId = m.Id,
                               p.Operation_Id,
                               p.Flight_Date_Want,
                               p.MBL,
                               p.Entrustment_Code,//委托方编号
                               p.HBL,
                               p.Entrustment_Name,
                               p.Carriage_Account_Code,
                               p.FWD_Code,//国外代理
                               p.Book_Flat_Code,
                               m.Pay_Mode_M,
                               p.Depart_Port,
                               p.End_Port,
                               p.Is_TG,
                               p.ADDTS,
                               p.ADDWHO,
                               m.OPS_BMS_Status
                           }).AsQueryable();

            if (!string.IsNullOrWhiteSpace(OPS_BMS_StatusStr))
            {
                QResult = QResult.Where(x => !x.OPS_BMS_Status);
            }

            if (filters != null)
            {
                #region 加载查询条件

                foreach (var item in filters)
                {
                    if (item.field == "Operation_Id" && !string.IsNullOrEmpty(item.value))
                    {
                        QResult = QResult.Where(x => x.Operation_Id.Contains(item.value));
                    }
                    if (item.field == "MBL" && !string.IsNullOrEmpty(item.value))
                    {
                        item.value = Common.RemoveNotNumber(item.value);
                        QResult = QResult.Where(x => x.MBL.Contains(item.value));
                    }
                    if (item.field == "_Flight_Date_Want" && !string.IsNullOrEmpty(item.value))
                    {
                        var date = Convert.ToDateTime(item.value);
                        QResult = QResult.Where(x => x.Flight_Date_Want >= date);
                    }
                    if (item.field == "Flight_Date_Want_" && !string.IsNullOrEmpty(item.value))
                    {
                        var date = Convert.ToDateTime(item.value);
                        QResult = QResult.Where(x => x.Flight_Date_Want <= date);
                    }
                    if (item.field == "Entrustment_Code" && !string.IsNullOrEmpty(item.value))
                    {
                        QResult = QResult.Where(x => x.Entrustment_Code.Contains(item.value));
                    }
                    if (item.field == "ADDWHO" && !string.IsNullOrEmpty(item.value))
                    {
                        QResult = QResult.Where(x => x.ADDWHO.Contains(item.value));
                    }
                    if (item.field == "End_Port" && !string.IsNullOrEmpty(item.value))
                    {
                        QResult = QResult.Where(x => x.End_Port == item.value);
                    }
                    if (item.field == "Entrustment_Name" && !string.IsNullOrEmpty(item.value))
                    {
                        QResult = QResult.Where(x => x.Entrustment_Name == item.value);
                    }
                    if (item.field == "Book_Flat_Code" && !string.IsNullOrEmpty(item.value))
                    {
                        QResult = QResult.Where(x => x.Book_Flat_Code == item.value);
                    }
                }

                #endregion
            }
            totalCount = QResult.Count();
            var datarows = QResult.OrderBy(sort, order).ThenByDescending(x => x.OPS_BMS_Status).Skip((page - 1) * rows).Take(rows).ToList();

            #region 取缓存数据

            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);
            var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);
            var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);

            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();

            #endregion

            var RetData = from n in datarows
                          join b in ArrCusBusInfo on n.FWD_Code equals b.EnterpriseId into b_tmp
                          from btmp in b_tmp.DefaultIfEmpty()
                          join c in ArrPARA_AirPort on n.Depart_Port equals c.PortCode into c_tmp
                          from ctmp in c_tmp.DefaultIfEmpty()
                          join d in ArrPARA_AirPort on n.End_Port equals d.PortCode into d_tmp
                          from dtmp in d_tmp.DefaultIfEmpty()
                          join e in ArrAppUser on n.ADDWHO equals e.UserName into e_tmp
                          from etmp in e_tmp.DefaultIfEmpty()
                          join g in ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "Pay_Mode") on n.Pay_Mode_M equals g.LISTCODE into g_tmp
                          from gtmp in g_tmp.DefaultIfEmpty()
                          join h in ArrCusBusInfo on n.Entrustment_Name equals h.EnterpriseId into h_tmp
                          from htmp in h_tmp.DefaultIfEmpty()
                          join i in ArrCusBusInfo on n.Book_Flat_Code equals i.EnterpriseId into i_tmp
                          from itmp in i_tmp.DefaultIfEmpty()
                          join j in ArrCusBusInfo on n.Carriage_Account_Code equals j.EnterpriseId into j_tmp
                          from jtmp in j_tmp.DefaultIfEmpty()
                          select new
                          {
                              n.Ops_M_OrdId,
                              n.Operation_Id,
                              n.Flight_Date_Want,
                              n.MBL,
                              n.Entrustment_Code,
                              n.HBL,
                              n.Entrustment_Name,
                              Entrustment_NameNAME = htmp == null ? "" : htmp.EnterpriseShortName,
                              n.Carriage_Account_Code,
                              Carriage_Account_CodeNAME = jtmp == null ? "" : jtmp.EnterpriseShortName,
                              n.FWD_Code,
                              FWD_CodeNAME = btmp == null ? "" : btmp.EnterpriseShortName,
                              n.Book_Flat_Code,
                              Book_Flat_CodeNAME = itmp == null ? "" : itmp.EnterpriseShortName,
                              n.Pay_Mode_M,
                              Pay_Mode_MNAME = gtmp == null ? "" : gtmp.LISTNAME,
                              n.Depart_Port,
                              Depart_PortNAME = ctmp == null ? "" : ctmp.PortName,
                              n.End_Port,
                              End_PortNAME = dtmp == null ? "" : dtmp.PortName,
                              n.Is_TG,
                              n.ADDTS,
                              n.ADDWHO,
                              ADDWHONAME = etmp == null ? "" : etmp.UserNameDesc,
                              n.OPS_BMS_Status
                          };

            var pagelist = new { total = totalCount, rows = RetData };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改总单号
        /// </summary>
        /// <param name="ArrOps_M_Ids"></param>
        /// <param name="NewMBL"></param>
        public ActionResult EditMBL(IEnumerable<int> ArrOps_M_Ids, string NewMBL)
        {
            var a = Request["ArrOps_M_Ids"] ?? "";
            var ErrMsg = _oPS_M_OrderService.EditMBL(ArrOps_M_Ids, NewMBL);
            if (!string.IsNullOrEmpty(ErrMsg))
            {
                ErrMsg = "修改总单号失败：" + ErrMsg;
            }
            else
            {
                var ApErrMsg = _bms_Bill_ApService.InsertFT_EntrustmentInfor(NewMBL);
                if (!string.IsNullOrEmpty(ApErrMsg))
                {
                    ErrMsg = "修改总单号成功，但插入总单应付行失败" + ApErrMsg;
                }
            }
            #region 更新 应收/应付 账单总单号

            if (!string.IsNullOrWhiteSpace(NewMBL))
            {
                List<string> ArrUpdateMblSQLStr = new List<string>(){
                    "update Bms_Bill_Ars t set t.mbl=:V_MBL where t.ops_m_ordid in (select o.MBLId from ops_entrustmentinfors o where o.MBL=:V_MBL)",
                    "update Bms_Bill_Aps t set t.mbl=:V_MBL where t.ops_m_ordid in (select o.MBLId from ops_entrustmentinfors o where o.MBL=:V_MBL)"
                };
                SQLDALHelper.OracleHelper.ExecuteSqlTran(ArrUpdateMblSQLStr, new Oracle.ManagedDataAccess.Client.OracleParameter(":V_MBL", NewMBL));
            }

            #endregion
            if (string.IsNullOrEmpty(ErrMsg))
                return Json(new { Success = true, ErrMsg = "" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
        }

        private void DisplaySuccessMessage(string msgText)
        {
            TempData["SuccessMessage"] = msgText;
        }

        private void DisplayErrorMessage()
        {
            TempData["ErrorMessage"] = "Save changes was unsuccessful.";
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _unitOfWork.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}