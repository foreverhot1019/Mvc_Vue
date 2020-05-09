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

namespace AirOut.Web.Controllers
{
    public class OPS_EntrustmentInforsController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<OPS_EntrustmentInfor>, Repository<OPS_EntrustmentInfor>>();
        //container.RegisterType<IOPS_EntrustmentInforService, OPS_EntrustmentInforService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IOPS_EntrustmentInforService _oPS_EntrustmentInforService;
        private readonly IOPS_M_OrderService _oPS_M_OrderService;
        private readonly IOPS_H_OrderService _oPS_H_OrderService;
        private readonly IWarehouse_receiptService _warehouse_receiptService;
        private readonly IWarehouse_Cargo_SizeService _warehouse_Cargo_SizeService;
        private readonly ICustomsInspectionService _customsInspectionService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/OPS_EntrustmentInfors";

        public OPS_EntrustmentInforsController(IOPS_EntrustmentInforService oPS_EntrustmentInforService, IOPS_M_OrderService oPS_M_OrderService,
            IOPS_H_OrderService oPS_H_OrderService, IWarehouse_receiptService warehouse_receiptService,
            IWarehouse_Cargo_SizeService warehouse_Cargo_SizeService, ICustomsInspectionService customsInspectionService, IUnitOfWorkAsync unitOfWork)
        {
            _oPS_EntrustmentInforService = oPS_EntrustmentInforService;
            _oPS_M_OrderService = oPS_M_OrderService;
            _oPS_H_OrderService = oPS_H_OrderService;
            _warehouse_receiptService = warehouse_receiptService;
            _warehouse_Cargo_SizeService = warehouse_Cargo_SizeService;
            _customsInspectionService = customsInspectionService;
            _unitOfWork = unitOfWork;
        }

        // GET: OPS_EntrustmentInfors/Index
        public ActionResult Index()
        {
            //var ops_entrustmentinfor  = _oPS_EntrustmentInforService.Queryable().AsQueryable();           
            //return View(ops_entrustmentinfor  );
            return View();
        }

        public ActionResult ContentResultDemo(int? id)
        {
            var ops_entrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>().Query(x => x.Id == id).Select().FirstOrDefault();
            var ops_entrustmentInfor_OP_ID = ops_entrustmentInforRep.Operation_Id;
            var CusInsData = _unitOfWork.Repository<CustomsInspection>().Query(x => x.Operation_ID == ops_entrustmentInfor_OP_ID).Select().LastOrDefault();
            if (CusInsData == null)
            {
                return Json(new { Success = false, ErrMsg = "该订单未完成报关" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = true, Id = CusInsData.Id }, JsonRequestBehavior.AllowGet);
        }

        // Get :OPS_EntrustmentInfors/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var ops_entrustmentinfor = _oPS_EntrustmentInforService.Query(new OPS_EntrustmentInforQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);

            var datarows = ops_entrustmentinfor.Select(n => new
            {
                Id = n.Id,
                MBLId = n.MBLId,
                Operation_Id = n.Operation_Id,
                Consign_Code = n.Consign_Code,
                Custom_Code = n.Custom_Code,
                Area_Code = n.Area_Code,
                Entrustment_Name = n.Entrustment_Name,
                Entrustment_Code = n.Entrustment_Code,
                FWD_Code = n.FWD_Code,
                Consignee_Code = n.Consignee_Code,
                Carriage_Account_Code = n.Carriage_Account_Code,
                Incidental_Account_Code = n.Incidental_Account_Code,
                Depart_Port = n.Depart_Port,
                Transfer_Port = n.Transfer_Port,
                End_Port = n.End_Port,
                Shipper_H = n.Shipper_H,
                Consignee_H = n.Consignee_H,
                Notify_Part_H = n.Notify_Part_H,
                Shipper_M = n.Shipper_M,
                Consignee_M = n.Consignee_M,
                Notify_Part_M = n.Notify_Part_M,
                Pieces_TS = n.Pieces_TS,
                Weight_TS = n.Weight_TS,
                Pieces_SK = n.Pieces_SK,
                Slac_SK = n.Slac_SK,
                Weight_SK = n.Weight_SK,
                Pieces_DC = n.Pieces_DC,
                Slac_DC = n.Slac_DC,
                Weight_DC = n.Weight_DC,
                Pieces_Fact = n.Pieces_Fact,
                Weight_Fact = n.Weight_Fact,
                IS_MoorLevel = n.IS_MoorLevel,
                MoorLevel = n.MoorLevel,
                Volume_TS = n.Volume_TS,
                Charge_Weight_TS = n.Charge_Weight_TS,
                Bulk_Weight_TS = n.Bulk_Weight_TS,
                Volume_SK = n.Volume_SK,
                Charge_Weight_SK = n.Charge_Weight_SK,
                Bulk_Weight_SK = n.Bulk_Weight_SK,
                Bulk_Percent_SK = n.Bulk_Percent_SK,
                Account_Weight_SK = n.Account_Weight_SK,
                Volume_DC = n.Volume_DC,
                Charge_Weight_DC = n.Charge_Weight_DC,
                Bulk_Weight_DC = n.Bulk_Weight_DC,
                Bulk_Percent_DC = n.Bulk_Percent_DC,
                Account_Weight_DC = n.Account_Weight_DC,
                Volume_Fact = n.Volume_Fact,
                Charge_Weight_Fact = n.Charge_Weight_Fact,
                Bulk_Weight_Fact = n.Bulk_Weight_Fact,
                Bulk_Percent_Fact = n.Bulk_Percent_Fact,
                Account_Weight_Fact = n.Account_Weight_Fact,
                Marks_H = n.Marks_H,
                EN_Name_H = n.EN_Name_H,
                Book_Flat_Code = n.Book_Flat_Code,
                Airways_Code = n.Airways_Code,
                FLIGHT_No = n.Flight_No,
                MBL = n.MBL,
                HBL = n.HBL,
                Flight_Date_Want = n.Flight_Date_Want,
                Book_Remark = n.Book_Remark,
                Delivery_Point = n.Delivery_Point,
                Warehouse_Code = n.Warehouse_Code,
                RK_Date = n.RK_Date,
                CK_Date = n.CK_Date,
                CH_Name = n.CH_Name,
                AMS = n.AMS,
                Is_Self = n.Is_Self,
                Ty_Type = n.Ty_Type,
                Lot_No = n.Lot_No,
                Hbl_Feight = n.Hbl_Feight,
                Is_XC = n.Is_XC,
                Is_BAS = n.Is_BAS,
                Is_DCZ = n.Is_DCZ,
                Is_ZB = n.Is_ZB,
                ADDPoint = n.ADDPoint,
                EDITPoint = n.EDITPoint,
                Batch_Num = n.Batch_Num,
                Status = n.Status,
                Remark = n.Remark,
                OperatingPoint = n.OperatingPoint,
                CusInspInfor = ""
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };

            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取批量维护数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="filterRules"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDataRows(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var ops_entrustmentinfor = _oPS_EntrustmentInforService.Query(new OPS_EntrustmentInforQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().AsQueryable();//.SelectPage(page, rows, out totalCount).AsQueryable();


            var ops_m_order = _unitOfWork.RepositoryAsync<OPS_M_Order>().Queryable().Where(x => x.OPS_BMS_Status == false).AsQueryable();//总单信息

            var datas = (from o in ops_m_order
                         join n in ops_entrustmentinfor on o.Id equals n.MBLId
                         select new
                         {
                             Id = n.Id,
                             MBLId = n.MBLId,
                             Operation_Id = n.Operation_Id,
                             Consign_Code = n.Consign_Code,
                             Custom_Code = n.Custom_Code,
                             Area_Code = n.Area_Code,
                             Entrustment_Name = n.Entrustment_Name,
                             Entrustment_Code = n.Entrustment_Code,
                             FWD_Code = n.FWD_Code,
                             Consignee_Code = n.Consignee_Code,
                             Carriage_Account_Code = n.Carriage_Account_Code,
                             Incidental_Account_Code = n.Incidental_Account_Code,
                             Depart_Port = n.Depart_Port,
                             Transfer_Port = n.Transfer_Port,
                             End_Port = n.End_Port,
                             Shipper_H = n.Shipper_H,
                             Consignee_H = n.Consignee_H,
                             Notify_Part_H = n.Notify_Part_H,
                             Shipper_M = n.Shipper_M,
                             Consignee_M = n.Consignee_M,
                             Notify_Part_M = n.Notify_Part_M,
                             Pieces_TS = n.Pieces_TS,
                             Weight_TS = n.Weight_TS,
                             Pieces_SK = n.Pieces_SK,
                             Slac_SK = n.Slac_SK,
                             Weight_SK = n.Weight_SK,
                             Pieces_DC = n.Pieces_DC,
                             Slac_DC = n.Slac_DC,
                             Weight_DC = n.Weight_DC,
                             Pieces_Fact = n.Pieces_Fact,
                             Weight_Fact = n.Weight_Fact,
                             IS_MoorLevel = n.IS_MoorLevel,
                             MoorLevel = n.MoorLevel,
                             Volume_TS = n.Volume_TS,
                             Charge_Weight_TS = n.Charge_Weight_TS,
                             Bulk_Weight_TS = n.Bulk_Weight_TS,
                             Volume_SK = n.Volume_SK,
                             Charge_Weight_SK = n.Charge_Weight_SK,
                             Bulk_Weight_SK = n.Bulk_Weight_SK,
                             Bulk_Percent_SK = n.Bulk_Percent_SK,
                             Account_Weight_SK = n.Account_Weight_SK,
                             Volume_DC = n.Volume_DC,
                             Charge_Weight_DC = n.Charge_Weight_DC,
                             Bulk_Weight_DC = n.Bulk_Weight_DC,
                             Bulk_Percent_DC = n.Bulk_Percent_DC,
                             Account_Weight_DC = n.Account_Weight_DC,
                             Volume_Fact = n.Volume_Fact,
                             Charge_Weight_Fact = n.Charge_Weight_Fact,
                             Bulk_Weight_Fact = n.Bulk_Weight_Fact,
                             Bulk_Percent_Fact = n.Bulk_Percent_Fact,
                             Account_Weight_Fact = n.Account_Weight_Fact,
                             Marks_H = n.Marks_H,
                             EN_Name_H = n.EN_Name_H,
                             Book_Flat_Code = n.Book_Flat_Code,
                             Airways_Code = n.Airways_Code,
                             Flight_No = n.Flight_No,
                             MBL = n.MBL,
                             HBL = n.HBL,
                             Flight_Date_Want = n.Flight_Date_Want,
                             Book_Remark = n.Book_Remark,
                             Delivery_Point = n.Delivery_Point,
                             Warehouse_Code = n.Warehouse_Code,
                             RK_Date = n.RK_Date,
                             CK_Date = n.CK_Date,
                             CH_Name = n.CH_Name,
                             AMS = n.AMS,
                             Is_Self = n.Is_Self,
                             Ty_Type = n.Ty_Type,
                             Lot_No = n.Lot_No,
                             Hbl_Feight = n.Hbl_Feight,
                             Is_XC = n.Is_XC,
                             Is_BAS = n.Is_BAS,
                             Is_DCZ = n.Is_DCZ,
                             Is_ZB = n.Is_ZB,
                             ADDPoint = n.ADDPoint,
                             EDITPoint = n.EDITPoint,
                             Batch_Num = n.Batch_Num,
                             Status = n.Status,
                             Remark = n.Remark,
                             OperatingPoint = n.OperatingPoint,
                         }).AsQueryable();

            totalCount = datas.Count();
            var data = datas.OrderBy(sort, order).Skip((page - 1) * rows).Take(rows).ToList();

            var ListCusInsp = _unitOfWork.RepositoryAsync<CustomsInspection>().Queryable().ToList();//报关信息
            var ListCusBusInfo = _unitOfWork.RepositoryAsync<CusBusInfo>().Queryable().ToList();//客商信息
            var ListCodeItem_BG = _unitOfWork.RepositoryAsync<BD_DEFDOC_LIST>().Queryable().Where(x => x.DOCCODE == "CusType").ToList(); //基础代码 报关方式
            var ListCodeItem = _unitOfWork.RepositoryAsync<BD_DEFDOC_LIST>().Queryable().Where(x => x.DOCCODE == "Delivery_Point").ToList(); //基础代码 交货点


            //ViewData["ListCusInsp"] = ListCusInsp;
            var empIds = "";
            ListCusInsp.ForEach(a => empIds += a.Id + ",");

            var asQQcus = (from n in ListCusInsp
                           join c in ListCodeItem_BG on n.Customs_Declaration equals c.LISTCODE
                           select new
                           {
                               Operation_ID = n.Operation_ID,
                               Customs_Declaration = c == null ? "" : c.LISTNAME,
                               Num_BG = n.Num_BG
                           }).AsQueryable();

            var QQcus = (from n in asQQcus
                         group n by n.Operation_ID into g
                         select new
                         {
                             Operation_ID = g.Key,
                             Customs_Broker_BG = g.Select(x => x.Customs_Declaration + ":" + x.Num_BG.ToString()),
                             Customs_Broker_BG_Name = ""
                         }).ToList();
            List<Cus_bg> bglist = new List<Cus_bg>();
            foreach (var item in QQcus)
            {
                Cus_bg bg = new Cus_bg();
                bg.Operation_ID = item.Operation_ID;
                string ss = "";
                foreach (var tt in item.Customs_Broker_BG)
                {
                    if (ss == "")
                    {
                        ss = tt.ToString();
                        continue;
                    }
                    ss = ss + "," + tt.ToString();
                }
                bg.Customs_Broker_BG_Name = ss;
                bglist.Add(bg);
            }

            var datarows = (from n in data
                            join a in ListCusBusInfo on n.Entrustment_Name equals a.EnterpriseId into a_tmp
                            from atmp in a_tmp.DefaultIfEmpty()
                            join b in ListCusBusInfo on n.Book_Flat_Code equals b.EnterpriseId into b_tmp
                            from btmp in b_tmp.DefaultIfEmpty()
                            join c in ListCusBusInfo on n.Airways_Code equals c.EnterpriseId into c_tmp
                            from ctmp in c_tmp.DefaultIfEmpty()
                            join d in ListCodeItem on n.Delivery_Point equals d.LISTCODE into d_tmp
                            from dtmp in d_tmp.DefaultIfEmpty()
                            join e in bglist on n.Operation_Id equals e.Operation_ID into e_tmp
                            from etmp in e_tmp.DefaultIfEmpty()
                            select new
                            {
                                Id = n.Id,
                                MBLId = n.MBLId,
                                Operation_Id = n.Operation_Id,
                                Consign_Code = n.Consign_Code,
                                Custom_Code = n.Custom_Code,
                                Area_Code = n.Area_Code,
                                //Entrustment_Name = n.Entrustment_Name,
                                Entrustment_Name = atmp == null ? "" : atmp.EnterpriseShortName,
                                Entrustment_Code = n.Entrustment_Code,
                                FWD_Code = n.FWD_Code,
                                Consignee_Code = n.Consignee_Code,
                                Carriage_Account_Code = n.Carriage_Account_Code,
                                Incidental_Account_Code = n.Incidental_Account_Code,
                                Depart_Port = n.Depart_Port,
                                Transfer_Port = n.Transfer_Port,
                                End_Port = n.End_Port,
                                Shipper_H = n.Shipper_H,
                                Consignee_H = n.Consignee_H,
                                Notify_Part_H = n.Notify_Part_H,
                                Shipper_M = n.Shipper_M,
                                Consignee_M = n.Consignee_M,
                                Notify_Part_M = n.Notify_Part_M,
                                Pieces_TS = n.Pieces_TS,
                                Weight_TS = n.Weight_TS,
                                Pieces_SK = n.Pieces_SK,
                                Slac_SK = n.Slac_SK,
                                Weight_SK = n.Weight_SK,
                                Pieces_DC = n.Pieces_DC,
                                Slac_DC = n.Slac_DC,
                                Weight_DC = n.Weight_DC,
                                Pieces_Fact = n.Pieces_Fact,
                                Weight_Fact = n.Weight_Fact,
                                IS_MoorLevel = n.IS_MoorLevel,
                                MoorLevel = n.MoorLevel,
                                Volume_TS = n.Volume_TS,
                                Charge_Weight_TS = n.Charge_Weight_TS,
                                Bulk_Weight_TS = n.Bulk_Weight_TS,
                                Volume_SK = n.Volume_SK,
                                Charge_Weight_SK = n.Charge_Weight_SK,
                                Bulk_Weight_SK = n.Bulk_Weight_SK,
                                Bulk_Percent_SK = n.Bulk_Percent_SK,
                                Account_Weight_SK = n.Account_Weight_SK,
                                Volume_DC = n.Volume_DC,
                                Charge_Weight_DC = n.Charge_Weight_DC,
                                Bulk_Weight_DC = n.Bulk_Weight_DC,
                                Bulk_Percent_DC = n.Bulk_Percent_DC,
                                Account_Weight_DC = n.Account_Weight_DC,
                                Volume_Fact = n.Volume_Fact,
                                Charge_Weight_Fact = n.Charge_Weight_Fact,
                                Bulk_Weight_Fact = n.Bulk_Weight_Fact,
                                Bulk_Percent_Fact = n.Bulk_Percent_Fact,
                                Account_Weight_Fact = n.Account_Weight_Fact,
                                Marks_H = n.Marks_H,
                                EN_Name_H = n.EN_Name_H,
                                //Book_Flat_Code = n.Book_Flat_Code,
                                Book_Flat_Code = btmp == null ? "" : btmp.EnterpriseShortName,
                                //Airways_Code = n.Airways_Code,
                                Airways_Code = ctmp == null ? "" : ctmp.EnterpriseShortName,
                                Flight_No = n.Flight_No,
                                MBL = n.MBL,
                                HBL = n.HBL,
                                Flight_Date_Want = n.Flight_Date_Want,
                                Book_Remark = n.Book_Remark,
                                Delivery_Point = dtmp == null ? "" : dtmp.LISTNAME,
                                Warehouse_Code = n.Warehouse_Code,
                                RK_Date = n.RK_Date,
                                CK_Date = n.CK_Date,
                                CH_Name = n.CH_Name,
                                AMS = n.AMS,
                                Is_Self = n.Is_Self,
                                Ty_Type = n.Ty_Type,
                                Lot_No = n.Lot_No,
                                Hbl_Feight = n.Hbl_Feight,
                                Is_XC = n.Is_XC,
                                Is_BAS = n.Is_BAS,
                                Is_DCZ = n.Is_DCZ,
                                Is_ZB = n.Is_ZB,
                                ADDPoint = n.ADDPoint,
                                EDITPoint = n.EDITPoint,
                                Batch_Num = n.Batch_Num,
                                Status = n.Status,
                                Remark = n.Remark,
                                OperatingPoint = n.OperatingPoint,
                                CusInspInfor = etmp == null ? "" : etmp.Customs_Broker_BG_Name,
                            }).ToList();

            var pagelist = new { total = totalCount, rows = datarows };

            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        public class Cus_bg
        {
            public string Operation_ID { get; set; }
            public string Customs_Broker_BG_Name { get; set; }
        }

        /// <summary>
        /// 获取Combogrid-Text显示名
        /// </summary>
        /// <param name="OCustomerQuotedPrice"></param>
        /// <returns></returns>
        public dynamic GetFromNAME(OPS_EntrustmentInfor _OPS_EntrustmentInfors)
        {
            dynamic ODynamic = new System.Dynamic.ExpandoObject();

            //客商信息
            var ArrCusBusInfo = (List<CusBusInfo>)CacheHelper.Get_SetCache(Common.CacheNameS.CusBusInfo);


            //港口
            var ArrPARA_AirPort = (List<PARA_AirPort>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirPort);


            //币种
            var ArrPARA_CURR = (List<PARA_CURR>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_CURR);

            //航线
            var ArrPARA_AirLine = (List<PARA_AirLine>)CacheHelper.Get_SetCache(Common.CacheNameS.PARA_AirLine);


            //基础代码
            var ArrCodeItem = (IEnumerable<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST); //(List<CodeItem>)CacheHelper.Get_SetCache(Common.CacheNameS.CodeItem);

            var ArrAppUser = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetApplicationUser();

            var item = _OPS_EntrustmentInfors;

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
            if (!string.IsNullOrEmpty(item.Airways_Code))
            {//航空公司
                var OCusBusInfo = ArrCusBusInfo.Where(x => x.EnterpriseId == item.Airways_Code).FirstOrDefault();
                ODynamic.Airways_CodeNAME = OCusBusInfo == null ? "" : OCusBusInfo.EnterpriseShortName;
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

            return ODynamic;
        }

        [HttpPost]
        public ActionResult SaveData(OPS_EntrustmentInforChangeViewModel ops_entrustmentinfor)
        {
            if (ops_entrustmentinfor.updated != null)
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

                foreach (var updated in ops_entrustmentinfor.updated)
                {
                    _oPS_EntrustmentInforService.Update(updated);
                }
            }
            if (ops_entrustmentinfor.deleted != null)
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

                foreach (var deleted in ops_entrustmentinfor.deleted)
                {
                    _oPS_EntrustmentInforService.Delete(deleted);
                }
            }
            if (ops_entrustmentinfor.inserted != null)
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

                foreach (var inserted in ops_entrustmentinfor.inserted)
                {
                    _oPS_EntrustmentInforService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((ops_entrustmentinfor.updated != null && ops_entrustmentinfor.updated.Any()) ||
                (ops_entrustmentinfor.deleted != null && ops_entrustmentinfor.deleted.Any()) ||
                (ops_entrustmentinfor.inserted != null && ops_entrustmentinfor.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(ops_entrustmentinfor);
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

        // GET: OPS_EntrustmentInfors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPS_EntrustmentInfor oPS_EntrustmentInfor = _oPS_EntrustmentInforService.Find(id);
            if (oPS_EntrustmentInfor == null)
            {
                return HttpNotFound();
            }
            return View(oPS_EntrustmentInfor);
        }

        // GET: OPS_EntrustmentInfors/Create
        public ActionResult Create()
        {
            OPS_EntrustmentInfor oPS_EntrustmentInfor = new OPS_EntrustmentInfor();
            //set default value
            return View(oPS_EntrustmentInfor);
        }

        // POST: OPS_EntrustmentInfors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Operation_Id,Consign_Code,Custom_Code,Area_Code,Entrustment_Name,Entrustment_Code,FWD_Code,Consignee_Code,Carriage_Account_Code,Incidental_Account_Code,Depart_Port,Transfer_Port,End_Port,Shipper_H,Consignee_H,Notify_Part_H,Shipper_M,Consignee_M,Notify_Part_M,Pieces_TS,Weight_TS,Pieces_SK,Slac_SK,Weight_SK,Pieces_DC,Slac_DC,Weight_DC,Pieces_Fact,Weight_Fact,IS_MoorLevel,MoorLevel,Volume_TS,Charge_Weight_TS,Bulk_Weight_TS,Volume_SK,Charge_Weight_SK,Bulk_Weight_SK,Bulk_Percent_SK,Account_Weight_SK,Volume_DC,Charge_Weight_DC,Bulk_Weight_DC,Bulk_Percent_DC,Account_Weight_DC,Volume_Fact,Charge_Weight_Fact,Bulk_Weight_Fact,Bulk_Percent_Fact,Account_Weight_Fact,Marks_H,EN_Name_H,Book_Flat_Code,Airways_Code,FLIGHT_No,MBL,HBL,Flight_Date_Want,Book_Remark,Delivery_Point,Warehouse_Code,RK_Date,CK_Date,CH_Name,AMS,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] OPS_EntrustmentInfor oPS_EntrustmentInfor)
        {
            if (ModelState.IsValid)
            {
                _oPS_EntrustmentInforService.Insert(oPS_EntrustmentInfor);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a OPS_EntrustmentInfor record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(oPS_EntrustmentInfor);
        }

        // GET: OPS_EntrustmentInfors/Create
        public ActionResult OPS_Create_M()
        {
            OPS_EntrustmentInfor oPS_EntrustmentInfor = new OPS_EntrustmentInfor();
            //oPS_EntrustmentInfor.Operation_Id = SequenceBuilder.NextEntrustmentInforOperation_IdSerial_No();//2018-12-20

            ViewBag.m_order = new OPS_M_Order();
            ViewBag.h_order = new OPS_H_Order();

            //set default value
            return View(oPS_EntrustmentInfor);
        }

        // GET: OPS_EntrustmentInfors/Create
        public ActionResult OPS_Create(int? id = 0)
        {
            OPS_EntrustmentInfor oPS_EntrustmentInfor = new OPS_EntrustmentInfor();
            OPS_M_Order m_order = new OPS_M_Order();
            OPS_H_Order h_order = new OPS_H_Order();
            ViewBag.m_order = new OPS_M_Order();
            ViewBag.h_order = new OPS_H_Order();
            ViewBag.AirTime = "";//航班时间
            if (id == 0)
            {
                //新增
                //oPS_EntrustmentInfor.Operation_Id = SequenceBuilder.NextEntrustmentInforOperation_IdSerial_No();//2018-12-20
            }
            else
            {//复制新增
                var oPS_EntrustmentInforEdit = _oPS_EntrustmentInforService.Find(id);
                if (oPS_EntrustmentInforEdit == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    oPS_EntrustmentInfor = Common.SetProperties<OPS_EntrustmentInfor, OPS_EntrustmentInfor>(oPS_EntrustmentInforEdit, "Id,Operation_Id,ADDWHO,ADDTS,EDITWHO,EDITTS");
                    //oPS_EntrustmentInfor.Operation_Id = SequenceBuilder.NextEntrustmentInforOperation_IdSerial_No();//2018-12-20
                    if (!string.IsNullOrEmpty(oPS_EntrustmentInforEdit.MBL))
                    {
                        var m_orderEdit = _oPS_M_OrderService.Queryable().Where(x => x.MBL.Equals(oPS_EntrustmentInforEdit.MBL)).ToList();
                        if (m_order != null)
                        {
                            m_order = Common.SetProperties<OPS_M_Order, OPS_M_Order>(m_orderEdit.FirstOrDefault(), "Id,Operation_Id,ADDWHO,ADDTS,EDITWHO,EDITTS");

                            ViewBag.m_order = m_order;
                        }
                        else
                        {
                            ViewBag.m_order = new OPS_M_Order();
                        }
                    }
                    if (!string.IsNullOrEmpty(oPS_EntrustmentInforEdit.HBL))
                    {
                        var h_orderEdit = _oPS_H_OrderService.Queryable().Where(x => x.HBL.Equals(oPS_EntrustmentInforEdit.HBL) && x.Operation_Id.Equals(oPS_EntrustmentInforEdit.Operation_Id)).ToList();
                        if (h_order != null)
                        {
                            h_order = Common.SetProperties<OPS_H_Order, OPS_H_Order>(h_orderEdit.FirstOrDefault(), "Id,Operation_Id,ADDWHO,ADDTS,EDITWHO,EDITTS");

                            ViewBag.h_order = h_order;
                        }
                        else
                        {
                            ViewBag.h_order = new OPS_H_Order();
                        }
                    }
                    if (!string.IsNullOrEmpty(oPS_EntrustmentInfor.Flight_No))
                    {
                        var para_AirLineRep = _unitOfWork.RepositoryAsync<PARA_AirLine>();
                        var data = para_AirLineRep.Queryable().Where(x => x.AirCode.Equals(oPS_EntrustmentInfor.Flight_No)).FirstOrDefault();
                        if (data != null)
                        {
                            if (data.AirTime != null)
                            {
                                ViewBag.AirTime = data.AirTime;
                            }
                            else
                            {
                                ViewBag.AirTime = "";
                            }
                        }
                    }
                }
            }
            //set default value
            return View(oPS_EntrustmentInfor);
        }

        // GET: OPS_EntrustmentInfors/Create
        public ActionResult AddEditFore(int? id = 0)
        {
            OPS_EntrustmentInfor oPS_EntrustmentInfor = new OPS_EntrustmentInfor();
            ViewBag.AirTime = "";//航班时间
            if (id == 0)
            {//新增
                //oPS_EntrustmentInfor.Operation_Id = SequenceBuilder.NextEntrustmentInforOperation_IdSerial_No();//2018-12-20
            }
            else
            {
                oPS_EntrustmentInfor = _oPS_EntrustmentInforService.Find(id);
            }
            //set default value
            return View(oPS_EntrustmentInfor);
        }

        // POST: OPS_EntrustmentInfors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult OPS_Create([Bind(Include = "Id,Operation_Id,Consign_Code,Custom_Code,Area_Code,Entrustment_Name,Entrustment_Code,FWD_Code,Consignee_Code,Carriage_Account_Code,Incidental_Account_Code,Depart_Port,Transfer_Port,End_Port,Shipper_H,Consignee_H,Notify_Part_H,Shipper_M,Consignee_M,Notify_Part_M,Pieces_TS,Weight_TS,Pieces_SK,Slac_SK,Weight_SK,Pieces_DC,Slac_DC,Weight_DC,Pieces_Fact,Weight_Fact,IS_MoorLevel,MoorLevel,Volume_TS,Charge_Weight_TS,Bulk_Weight_TS,Volume_SK,Charge_Weight_SK,Bulk_Weight_SK,Bulk_Percent_SK,Account_Weight_SK,Volume_DC,Charge_Weight_DC,Bulk_Weight_DC,Bulk_Percent_DC,Account_Weight_DC,Volume_Fact,Charge_Weight_Fact,Bulk_Weight_Fact,Bulk_Percent_Fact,Account_Weight_Fact,Marks_H,EN_Name_H,Book_Flat_Code,Airways_Code,FLIGHT_No,MBL,HBL,Flight_Date_Want,Book_Remark,Delivery_Point,Warehouse_Code,RK_Date,CK_Date,CH_Name,AMS,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] OPS_EntrustmentInfor oPS_EntrustmentInfor)
        {
            if (ModelState.IsValid)
            {
                _oPS_EntrustmentInforService.Insert(oPS_EntrustmentInfor);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a OPS_EntrustmentInfor record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(oPS_EntrustmentInfor);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SaveEMH(OPS_EntrustmentInfor EntrustmentInfor = null, OPS_M_Order M_Order = null, OPS_H_Order H_Order = null, string Is_Binding = "0")//OPS_EntrustmentInfor EntrustmentInfor, OPS_M_Order M_Order, OPS_H_Order H_Order
        {
            try
            {
                #region 委托信息保存
                if (EntrustmentInfor != null)
                {
                    if (EntrustmentInfor.Id == 0)
                    {
                        _oPS_EntrustmentInforService.Insert(EntrustmentInfor);
                    }
                    else
                    {
                        var item = _oPS_EntrustmentInforService.Find(EntrustmentInfor.Id);
                        item.Consign_Code = EntrustmentInfor.Consign_Code;
                        item.Entrustment_Name = EntrustmentInfor.Entrustment_Name;
                        item.Entrustment_Code = EntrustmentInfor.Entrustment_Code;
                        item.FWD_Code = EntrustmentInfor.FWD_Code;
                        item.Consignee_Code = EntrustmentInfor.Consignee_Code;
                        item.Carriage_Account_Code = EntrustmentInfor.Carriage_Account_Code;
                        item.Incidental_Account_Code = EntrustmentInfor.Incidental_Account_Code;
                        item.Depart_Port = EntrustmentInfor.Depart_Port;
                        item.Transfer_Port = EntrustmentInfor.Transfer_Port;
                        item.End_Port = EntrustmentInfor.End_Port;
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
                        item.Pieces_Fact = EntrustmentInfor.Pieces_Fact;
                        item.Weight_Fact = EntrustmentInfor.Weight_Fact;
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
                        item.Volume_Fact = EntrustmentInfor.Volume_Fact;
                        item.Charge_Weight_Fact = EntrustmentInfor.Charge_Weight_Fact;
                        item.Bulk_Weight_Fact = EntrustmentInfor.Bulk_Weight_Fact;
                        item.Bulk_Percent_Fact = EntrustmentInfor.Bulk_Percent_Fact;
                        item.Account_Weight_Fact = EntrustmentInfor.Account_Weight_Fact;
                        item.Marks_H = EntrustmentInfor.Marks_H;
                        item.EN_Name_H = EntrustmentInfor.EN_Name_H;
                        item.Book_Flat_Code = EntrustmentInfor.Book_Flat_Code;
                        item.Airways_Code = EntrustmentInfor.Airways_Code;
                        item.Flight_No = EntrustmentInfor.Flight_No;
                        item.MBL = EntrustmentInfor.MBL;
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
                        item.ADDPoint = EntrustmentInfor.ADDPoint;
                        item.EDITPoint = EntrustmentInfor.EDITPoint;
                        //item.Batch_Num = EntrustmentInfor.Batch_Num;
                        item.Status = EntrustmentInfor.Status;
                        item.Remark = EntrustmentInfor.Remark;
                        item.OperatingPoint = EntrustmentInfor.OperatingPoint;
                        item.ObjectState = ObjectState.Modified;
                        _oPS_EntrustmentInforService.Update(item);
                    }
                }
                #endregion

                #region 主单信息保存
                if (M_Order != null)
                {
                    if (M_Order.Id != 0)
                    {
                        var item = _oPS_M_OrderService.Find(M_Order.Id);
                        item.MBL = M_Order.MBL;
                        item.Airways_Code = M_Order.Airways_Code;
                        item.FWD_Code = M_Order.FWD_Code;
                        item.Shipper_M = M_Order.Shipper_M;
                        item.Consignee_M = M_Order.Consignee_M;
                        item.Notify_Part_M = M_Order.Notify_Part_M;
                        item.Depart_Port = M_Order.Depart_Port;
                        item.End_Port = M_Order.End_Port;
                        item.Flight_No = M_Order.Flight_No;
                        item.Flight_Date_Want = M_Order.Flight_Date_Want;
                        item.Currency_M = M_Order.Currency_M;
                        item.Bragainon_Article_M = M_Order.Bragainon_Article_M;
                        item.Pay_Mode_M = M_Order.Pay_Mode_M;
                        item.Carriage_M = M_Order.Carriage_M;
                        item.Incidental_Expenses_M = M_Order.Incidental_Expenses_M;
                        item.Declare_Value_Trans_M = M_Order.Declare_Value_Trans_M;
                        item.Declare_Value_Ciq_M = M_Order.Declare_Value_Ciq_M;
                        item.Risk_M = M_Order.Risk_M;
                        item.Marks_M = M_Order.Marks_M;
                        item.EN_Name_M = M_Order.EN_Name_M;
                        item.Hand_Info_M = M_Order.Hand_Info_M;
                        item.Signature_Agent_M = M_Order.Signature_Agent_M;
                        item.Rate_Class_M = M_Order.Rate_Class_M;
                        item.Air_Frae = M_Order.Air_Frae;
                        item.AWC = M_Order.AWC;
                        item.Pieces_M = M_Order.Pieces_M;
                        item.Weight_M = M_Order.Weight_M;
                        item.Volume_M = M_Order.Volume_M;
                        item.Charge_Weight_M = M_Order.Charge_Weight_M;
                        item.Price_Article = M_Order.Price_Article;
                        item.CCC = M_Order.CCC;
                        item.File_M = M_Order.File_M;
                        item.Status = M_Order.Status;
                        item.OperatingPoint = M_Order.OperatingPoint;
                        item.ObjectState = ObjectState.Modified;
                        _oPS_M_OrderService.Update(item);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(M_Order.MBL))
                        {
                            var m_order = _oPS_M_OrderService.Queryable().Where(x => x.MBL.Equals(M_Order.MBL)).ToList();
                            if (m_order.Count == 0)
                            {//主单为空时
                                _oPS_M_OrderService.Insert(M_Order);
                            }
                            else
                            {
                                var item = m_order.FirstOrDefault();
                                item.MBL = M_Order.MBL;
                                item.Airways_Code = M_Order.Airways_Code;
                                item.FWD_Code = M_Order.FWD_Code;
                                item.Shipper_M = M_Order.Shipper_M;
                                item.Consignee_M = M_Order.Consignee_M;
                                item.Notify_Part_M = M_Order.Notify_Part_M;
                                item.Depart_Port = M_Order.Depart_Port;
                                item.End_Port = M_Order.End_Port;
                                item.Flight_No = M_Order.Flight_No;
                                item.Flight_Date_Want = M_Order.Flight_Date_Want;
                                item.Currency_M = M_Order.Currency_M;
                                item.Bragainon_Article_M = M_Order.Bragainon_Article_M;
                                item.Pay_Mode_M = M_Order.Pay_Mode_M;
                                item.Carriage_M = M_Order.Carriage_M;
                                item.Incidental_Expenses_M = M_Order.Incidental_Expenses_M;
                                item.Declare_Value_Trans_M = M_Order.Declare_Value_Trans_M;
                                item.Declare_Value_Ciq_M = M_Order.Declare_Value_Ciq_M;
                                item.Risk_M = M_Order.Risk_M;
                                item.Marks_M = M_Order.Marks_M;
                                item.EN_Name_M = M_Order.EN_Name_M;
                                item.Hand_Info_M = M_Order.Hand_Info_M;
                                item.Signature_Agent_M = M_Order.Signature_Agent_M;
                                item.Rate_Class_M = M_Order.Rate_Class_M;
                                item.Air_Frae = M_Order.Air_Frae;
                                item.AWC = M_Order.AWC;
                                item.Pieces_M = M_Order.Pieces_M;
                                item.Weight_M = M_Order.Weight_M;
                                item.Volume_M = M_Order.Volume_M;
                                item.Charge_Weight_M = M_Order.Charge_Weight_M;
                                item.Price_Article = M_Order.Price_Article;
                                item.CCC = M_Order.CCC;
                                item.File_M = M_Order.File_M;
                                item.Status = M_Order.Status;
                                item.OperatingPoint = M_Order.OperatingPoint;
                                item.ObjectState = ObjectState.Modified;
                                _oPS_M_OrderService.Update(item);
                            }
                        }
                    }
                }
                #endregion

                #region 分单信息保存
                if (H_Order != null)
                {
                    if (H_Order.Id == 0)
                    {
                        _oPS_H_OrderService.Insert(H_Order);
                    }
                    else
                    {
                        var item = _oPS_H_OrderService.Find(H_Order.Id);
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
                        item.HBL = H_Order.HBL;
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
                }
                #endregion

                #region 保存仓库接单关联信息
                if (Is_Binding == "-1")
                {//取消绑定进仓信息
                    var warehouse_receipt = _warehouse_receiptService.Queryable().Where(x => x.MBL.Equals(EntrustmentInfor.MBL) && x.HBL.Equals(EntrustmentInfor.HBL)).ToList();
                    if (warehouse_receipt != null && warehouse_receipt.Count() > 0)
                    {
                        var item = warehouse_receipt.FirstOrDefault();
                        item.MBL = "";
                        item.HBL = "";
                        item.Is_Binding = false;
                        _warehouse_receiptService.Update(item);
                    }
                }
                else if (Is_Binding != "0")
                {//绑定进仓信息
                    var item = _warehouse_receiptService.Find(Int32.Parse(Is_Binding));
                    if (item != null)
                    {
                        item.MBL = EntrustmentInfor.MBL;
                        item.HBL = EntrustmentInfor.HBL;
                        item.Is_Binding = true;
                        _warehouse_receiptService.Update(item);
                    }
                }
                #endregion

                _unitOfWork.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, err = ex.Message }, JsonRequestBehavior.AllowGet); ;
            }
        }

        // GET: OPS_EntrustmentInfors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPS_EntrustmentInfor oPS_EntrustmentInfor = _oPS_EntrustmentInforService.Find(id);
            if (oPS_EntrustmentInfor == null)
            {
                return HttpNotFound();
            }
            return View(oPS_EntrustmentInfor);
        }

        // POST: OPS_EntrustmentInfors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Operation_Id,Consign_Code,Custom_Code,Area_Code,Entrustment_Name,Entrustment_Code,FWD_Code,Consignee_Code,Carriage_Account_Code,Incidental_Account_Code,Depart_Port,Transfer_Port,End_Port,Shipper_H,Consignee_H,Notify_Part_H,Shipper_M,Consignee_M,Notify_Part_M,Pieces_TS,Weight_TS,Pieces_SK,Slac_SK,Weight_SK,Pieces_DC,Slac_DC,Weight_DC,Pieces_Fact,Weight_Fact,IS_MoorLevel,MoorLevel,Volume_TS,Charge_Weight_TS,Bulk_Weight_TS,Volume_SK,Charge_Weight_SK,Bulk_Weight_SK,Bulk_Percent_SK,Account_Weight_SK,Volume_DC,Charge_Weight_DC,Bulk_Weight_DC,Bulk_Percent_DC,Account_Weight_DC,Volume_Fact,Charge_Weight_Fact,Bulk_Weight_Fact,Bulk_Percent_Fact,Account_Weight_Fact,Marks_H,EN_Name_H,Book_Flat_Code,Airways_Code,FLIGHT_No,MBL,HBL,Flight_Date_Want,Book_Remark,Delivery_Point,Warehouse_Code,RK_Date,CK_Date,CH_Name,AMS,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] OPS_EntrustmentInfor oPS_EntrustmentInfor)
        {
            if (ModelState.IsValid)
            {
                oPS_EntrustmentInfor.ObjectState = ObjectState.Modified;
                _oPS_EntrustmentInforService.Update(oPS_EntrustmentInfor);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a OPS_EntrustmentInfor record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(oPS_EntrustmentInfor);
        }

        // GET: OPS_EntrustmentInfors/OPS_Edit/5
        public ActionResult OPS_Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPS_EntrustmentInfor oPS_EntrustmentInfor = _oPS_EntrustmentInforService.Find(id);
            if (oPS_EntrustmentInfor == null)
            {
                return HttpNotFound();
            }
            ViewBag.m_order = new OPS_M_Order();
            if (!string.IsNullOrEmpty(oPS_EntrustmentInfor.MBL))
            {
                var m_order = _oPS_M_OrderService.Queryable().Where(x => x.MBL.Equals(oPS_EntrustmentInfor.MBL)).ToList();
                if (m_order.Count != 0)
                {
                    ViewBag.m_order = m_order.FirstOrDefault();
                }
                else
                {
                    ViewBag.m_order = new OPS_M_Order();
                }
            }
            ViewBag.h_order = new OPS_H_Order();
            if (!string.IsNullOrEmpty(oPS_EntrustmentInfor.HBL))
            {
                var h_order = _oPS_H_OrderService.Queryable().Where(x => x.HBL.Equals(oPS_EntrustmentInfor.HBL) && x.Operation_Id.Equals(oPS_EntrustmentInfor.Operation_Id)).ToList();
                if (h_order.Count != 0)
                {
                    ViewBag.h_order = h_order.FirstOrDefault();
                }
                else
                {
                    ViewBag.h_order = new OPS_H_Order();
                }
            }
            ViewBag.AirTime = "";//航班时间
            if (!string.IsNullOrEmpty(oPS_EntrustmentInfor.Flight_No))
            {
                var para_AirLineRep = _unitOfWork.RepositoryAsync<PARA_AirLine>();
                var data = para_AirLineRep.Queryable().Where(x => x.AirCode.Equals(oPS_EntrustmentInfor.Flight_No)).FirstOrDefault();
                if (data != null)
                {
                    if (data.AirTime != null)
                    {
                        ViewBag.AirTime = data.AirTime;
                    }
                    else
                    {
                        ViewBag.AirTime = "";
                    }
                }
            }

            var ops_entrustmentInforRep = _unitOfWork.Repository<OPS_EntrustmentInfor>().Query(x => x.Id == id).Select().FirstOrDefault();
            var ops_entrustmentInfor_OP_ID = ops_entrustmentInforRep.Operation_Id;
            var CusInsData = _unitOfWork.Repository<CustomsInspection>().Query(x => x.Operation_ID == ops_entrustmentInfor_OP_ID).Select().LastOrDefault();
            ViewBag.CusInsData = CusInsData;
            if (CusInsData != null)
            {
                ViewBag.CusInsDataID = CusInsData.Id;
            }
            else
            {
                ViewBag.CusInsDataID = "000";
            }
            return View(oPS_EntrustmentInfor);
        }

        // POST: OPS_EntrustmentInfors/OPS_Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult OPS_Edit([Bind(Include = "Id,Operation_Id,Consign_Code,Custom_Code,Area_Code,Entrustment_Name,Entrustment_Code,FWD_Code,Consignee_Code,Carriage_Account_Code,Incidental_Account_Code,Depart_Port,Transfer_Port,End_Port,Shipper_H,Consignee_H,Notify_Part_H,Shipper_M,Consignee_M,Notify_Part_M,Pieces_TS,Weight_TS,Pieces_SK,Slac_SK,Weight_SK,Pieces_DC,Slac_DC,Weight_DC,Pieces_Fact,Weight_Fact,IS_MoorLevel,MoorLevel,Volume_TS,Charge_Weight_TS,Bulk_Weight_TS,Volume_SK,Charge_Weight_SK,Bulk_Weight_SK,Bulk_Percent_SK,Account_Weight_SK,Volume_DC,Charge_Weight_DC,Bulk_Weight_DC,Bulk_Percent_DC,Account_Weight_DC,Volume_Fact,Charge_Weight_Fact,Bulk_Weight_Fact,Bulk_Percent_Fact,Account_Weight_Fact,Marks_H,EN_Name_H,Book_Flat_Code,Airways_Code,FLIGHT_No,MBL,HBL,Flight_Date_Want,Book_Remark,Delivery_Point,Warehouse_Code,RK_Date,CK_Date,CH_Name,AMS,Status,Remark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] OPS_EntrustmentInfor oPS_EntrustmentInfor)
        {
            if (ModelState.IsValid)
            {
                oPS_EntrustmentInfor.ObjectState = ObjectState.Modified;
                _oPS_EntrustmentInforService.Update(oPS_EntrustmentInfor);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a OPS_EntrustmentInfor record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(oPS_EntrustmentInfor);
        }

        /// <summary>
        /// 保存生成拼箱码
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult SaveLot_No(Array ids)
        {
            try
            {
                var strids = "";
                var update = "0";
                var newLot_No = "";
                foreach (var item in ids)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");
                foreach (var id in idarr)
                {
                    OPS_EntrustmentInfor item = _oPS_EntrustmentInforService.Find(Int32.Parse(id));
                    var maxLot_No = _oPS_EntrustmentInforService.Queryable().OrderByDescending(x => x.Lot_No).FirstOrDefault();
                    if (item != null)
                    {
                        if (update == "0")
                        {
                            item.Lot_No = SequenceBuilder.NextLot_NoSerial_No(maxLot_No.Lot_No);
                            newLot_No = item.Lot_No;
                        }
                        else
                        {
                            item.Lot_No = newLot_No;
                        }
                        _oPS_EntrustmentInforService.Update(item);
                        update = "1";

                    }
                }
                _unitOfWork.SaveChanges();
                if (update == "1")
                {
                    foreach (var id in idarr)
                    {
                        OPS_EntrustmentInfor EntrustmentInfor = _oPS_EntrustmentInforService.Find(Int32.Parse(id));
                        //自动更新 缓存
                        if (IsAutoResetCache)
                            AutoResetCache(EntrustmentInfor);
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        /// <summary>
        /// 保存生成批次号
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="Batch_Num">批次号</param>
        /// <returns></returns>
        public ActionResult SaveBatch_Num(Array ids, string Batch_Num)
        {
            try
            {
                var strids = "";
                var update = "0";
                foreach (var item in ids)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");
                foreach (var id in idarr)
                {
                    OPS_EntrustmentInfor item = _oPS_EntrustmentInforService.Find(Int32.Parse(id));
                    if (item != null)
                    {
                        item.Batch_Num = Batch_Num;
                        _oPS_EntrustmentInforService.Update(item);
                        update = "1";
                    }
                }
                _unitOfWork.SaveChanges();
                if (update == "1")
                {
                    foreach (var id in idarr)
                    {
                        OPS_EntrustmentInfor EntrustmentInfor = _oPS_EntrustmentInforService.Find(Int32.Parse(id));
                        //自动更新 缓存
                        if (IsAutoResetCache)
                            AutoResetCache(EntrustmentInfor);
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        /// <summary>
        /// 保存出库和未出库
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult SaveBatch_OG(Array ids)
        {
            try
            {
                var strids = "";
                var update = "0";
                foreach (var item in ids)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",");
                foreach (var id in idarr)
                {
                    OPS_EntrustmentInfor item = _oPS_EntrustmentInforService.Find(Int32.Parse(id));
                    if (item != null)
                    {
                        if (item.Is_OutGoing == true)
                        {
                            item.Is_OutGoing = false;
                        }
                        else
                        {
                            item.Is_OutGoing = true;
                        }
                        _oPS_EntrustmentInforService.Update(item);
                        update = "1";
                    }
                }
                _unitOfWork.SaveChanges();
                if (update == "1")
                {
                    foreach (var id in idarr)
                    {
                        OPS_EntrustmentInfor EntrustmentInfor = _oPS_EntrustmentInforService.Find(Int32.Parse(id));
                        //自动更新 缓存
                        if (IsAutoResetCache)
                            AutoResetCache(EntrustmentInfor);
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        /// <summary>
        /// 保存退关或取消退关
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="is_tg">是否退关</param>
        /// <returns></returns>
        public ActionResult SaveBatch_TG(Array ids, bool is_tg =true)
        {
            try
            {
                bool NeedConfirm = false;//强制删除
                var NeedConfirmStr = Request["NeedConfirm"] ?? "";
                var m_IdsStr = Request["m_Ids"] ?? "";
                if (!string.IsNullOrWhiteSpace(NeedConfirmStr))
                {
                    NeedConfirm = Common.ChangStrToBool(NeedConfirmStr);
                }
                var strids = "";
                var ErrMsg = "";
                foreach (var item in ids)
                {
                    strids = item.ToString();
                }
                var idarr = strids.Split(",").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Int32.Parse(x)).Distinct();
                if (!is_tg)
                {
                    #region 取消退关
                    
                    //反向从依赖注入中取上下文dbContext
                    WebdbContext WebdbContxt = AirOut.Web.App_Start.UnityConfig.GetConfiguredContainer().Resolve(typeof(WebdbContext), "WebdbContext") as WebdbContext;
                    var ArrOPS_EttInfor = _oPS_EntrustmentInforService.Queryable().Where(x => idarr.Contains(x.Id)).Include(x => x.OPS_M_Order).ToList().Where(x => x.Is_TG);
                    foreach (var item in ArrOPS_EttInfor)
                    {
                        if (item.OPS_M_Order != null && item.OPS_M_Order.Id > 0 && !item.OPS_M_Order.OPS_BMS_Status)
                        {
                            var entry = WebdbContxt.Entry(item);
                            entry.State = EntityState.Modified;
                            item.ObjectState = ObjectState.Modified;
                            item.Is_TG = false;
                        }
                    }
                    WebdbContxt.SaveChanges();
                    return Json(new { Success = true, ErrMsg = "" }, JsonRequestBehavior.AllowGet);

                    #endregion
                }
                IEnumerable<int> Ops_M_Ids = new List<int>();
                //Ops_M_Ids = Ops_M_Ids.Concat(idarr);
                if (!string.IsNullOrEmpty(m_IdsStr))
                {
                    Ops_M_Ids = m_IdsStr.Split(",").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Int32.Parse(x)).Distinct();
                }
                ErrMsg = _oPS_EntrustmentInforService.Batch_TG(Ops_M_Ids, NeedConfirm);
                if (!string.IsNullOrWhiteSpace(ErrMsg))
                {
                    return Json(new { Success = false, ErrMsg = ErrMsg, NeedConfirm= 1 }, JsonRequestBehavior.AllowGet);
                }
                _unitOfWork.SaveChanges();
                return Json(new { Success = true, ErrMsg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string ErrMSg = Common.GetExceptionMsg(ex);
                Common.WriteLog_Local("退关失败SaveBatch_TG：" + ErrMSg, "OPS_EttInfor", true, true);
                return Json(new { Success = false, ErrMsg = ErrMSg }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: OPS_EntrustmentInfors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPS_EntrustmentInfor oPS_EntrustmentInfor = _oPS_EntrustmentInforService.Find(id);
            if (oPS_EntrustmentInfor == null)
            {
                return HttpNotFound();
            }
            return View(oPS_EntrustmentInfor);
        }

        // POST: OPS_EntrustmentInfors/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OPS_EntrustmentInfor oPS_EntrustmentInfor = _oPS_EntrustmentInforService.Find(id);
            _oPS_EntrustmentInforService.Delete(oPS_EntrustmentInfor);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a OPS_EntrustmentInfor record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "ops_entrustmentinfor_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _oPS_EntrustmentInforService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        //获取仓库接单明细表中，长宽高件数
        public ActionResult GetLWGP(int ID = 0, string MBL = "", string HBL = "")
        {
            var warehouse_receipt = _warehouse_receiptService.Queryable().Where(x => x.MBLId == ID).Include(x => x.ArrWarehouse_Cargo_Size).ToList();
            var ArrRet = new List<string>();
            if (warehouse_receipt != null && warehouse_receipt.Count() > 0)
            {
                foreach (var rec in warehouse_receipt)
                {
                    var details = rec.ArrWarehouse_Cargo_Size;
                    if (details != null || details.Any())
                    {
                        ArrRet.AddRange(details.Select(item => item.CM_Length + "*" + item.CM_Width + "*" + item.CM_Height + "*" + item.CM_Piece));
                    }
                }
            }
            return Json(new { Success = true, ReturnRemark = string.Join("/", ArrRet) }, JsonRequestBehavior.AllowGet);
        }

        //获取基础信息代码和名称
        public ActionResult GetCodeItem(int page = 1, int rows = 10, string q = "", int id = 0)
        {
            var codeItemRep = _unitOfWork.RepositoryAsync<CodeItem>();
            var data = codeItemRep.Queryable().Where(x => x.BaseCodeId == id);

            if (q != "")
            {
                data = data.Where(n => n.Code.Contains(q) || n.Code.Contains(q));
            }
            var list = data.Select(n => new { ID = n.Code, TEXT = n.Text });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //获取客商信息代码和名称
        public ActionResult GetCusBusInf(int page = 1, int rows = 10, string q = "")
        {
            var cusBusInfoRep = _unitOfWork.RepositoryAsync<CusBusInfo>();
            var data = cusBusInfoRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.EnterpriseId.Contains(q) || n.EnterpriseShortName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.EnterpriseId, TEXT = n.EnterpriseShortName, IDTEXT = n.EnterpriseId + "|" + n.EnterpriseShortName, AddressEng = n.AddressEng });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //获取客商信息代码、名称、关区、区域
        public ActionResult GetCusBusInfCode(int page = 1, int rows = 10, string q = "")
        {
            var cusBusInfoRep = _unitOfWork.RepositoryAsync<CusBusInfo>();
            var data = cusBusInfoRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.EnterpriseId.Contains(q) || n.EnterpriseShortName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.EnterpriseId, TEXT = n.EnterpriseShortName, IDTEXT = n.EnterpriseId + "|" + n.EnterpriseShortName, CustomsCode = n.CustomsCode, AreaCode = n.AreaCode, SallerId = n.SallerId, SallerName = n.SallerName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //获取币制信息代码和名称
        public ActionResult GetPARA_CURR(int page = 1, int rows = 10, string q = "")
        {
            var para_CURRRep = _unitOfWork.RepositoryAsync<PARA_CURR>();
            var data = para_CURRRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.CURR_CODE.Contains(q) || n.CURR_Name.Contains(q));
            }
            var list = data.Select(n => new { ID = n.CURR_CODE, TEXT = n.CURR_Name, IDTEXT = n.CURR_CODE + "|" + n.CURR_Name });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //获取港口信息代码和名称
        public ActionResult GetPARA_AIRPorts(int page = 1, int rows = 10, string q = "")
        {
            var para_AirPortRep = _unitOfWork.RepositoryAsync<PARA_AirPort>();
            var data = para_AirPortRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.PortCode.Contains(q) || n.PortName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.PortCode, TEXT = n.PortName, IDTEXT = n.PortCode + "|" + n.PortName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //获取航班信息代码和名称
        public ActionResult GetPARA_AirLine(int page = 1, int rows = 10, string q = "")
        {
            var para_AirLineRep = _unitOfWork.RepositoryAsync<PARA_AirLine>();
            var data = para_AirLineRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.AirCode.Contains(q) || n.AirCompany.Contains(q));
            }
            var list = data.Select(n => new { ID = n.AirCode, TEXT = n.AirCompany, Time = n.AirTime });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
        }

        //获取成交条款信息代码和名称
        public ActionResult GetDealArticle(int page = 1, int rows = 10, string q = "")
        {
            var dealArticleRep = _unitOfWork.RepositoryAsync<DealArticle>();
            var data = dealArticleRep.Queryable();
            if (q != "")
            {
                data = data.Where(n => n.DealArticleCode.Contains(q) || n.DealArticleName.Contains(q));
            }
            var list = data.Select(n => new { ID = n.DealArticleCode, TEXT = n.DealArticleName, IDTEXT = n.DealArticleName + "|" + n.DealArticleName });
            int totalCount = 0;
            totalCount = list.Count();
            return Json(new { total = totalCount, rows = list.OrderBy(x => x.ID).Skip(rows * (page - 1)).Take(rows).ToList() }, JsonRequestBehavior.AllowGet);
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
