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
using TMI.Web.Models;
using TMI.Web.Services;
using TMI.Web.Repositories;
using TMI.Web.Extensions;
using System.Threading.Tasks;

namespace TMI.Web.Controllers
{
    public class OrdersController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Order>, Repository<Order>>();
        //container.RegisterType<IOrderService, OrderService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IOrderService _orderService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/OrdersController";

        public OrdersController(IOrderService orderService, IUnitOfWorkAsync unitOfWork)
        {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
        }

        // GET: Orders/Index
        public ActionResult Index()
        {
            //var orders  = _orderService.Queryable().Include(o => o.OAdvisoryOrder).AsQueryable();
            //return View(orders);
            return View();
        }

        // Get :Orders/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var orders = _orderService.Query(new OrderQuery().Withfilter(filters)).Include(o => o.OAdvisoryOrder).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = orders.Select(n => new
            {
                OAdvisoryOrderOrderNo = (n.OAdvisoryOrder == null ? "" : n.OAdvisoryOrder.OrderNo),
                Id = n.Id,
                OrderNo = n.OrderNo,
                AdvisoryOrderId = n.AdvisoryOrderId,
                AdvsyOrdNo = n.AdvsyOrdNo,
                TravleOrdType = n.TravleOrdType,
                TravleType = n.TravleType,
                STravleDate = n.STravleDate,
                ETravleDate = n.ETravleDate,
                RouteNo = n.RouteNo,
                RouteName = n.RouteName,
                TravlePersonNum = n.TravlePersonNum,
                UnitPrice = n.UnitPrice,
                TotalPrice = n.TotalPrice,
                Saller = n.Saller,
                OP = n.OP,
                ForeCastPayDate = n.ForeCastPayDate,
                SupplyierNo = n.SupplyierNo,
                SupplierName = n.SupplierName,
                Contact = n.Contact,
                PayType = n.PayType,
                RoutePhoto = n.RoutePhoto,
                CustomerRequire = n.CustomerRequire,
                Remark = n.Remark,
                Status = n.Status,
                AuditStatus = n.AuditStatus,
                AdultNum = n.AdultNum,
                AdultActualNum = n.AdultActualNum,
                AdultPrice = n.AdultPrice,
                BoyNum = n.BoyNum,
                BoyActualNum = n.BoyActualNum,
                BoyPrice = n.BoyPrice,
                ChildNum = n.ChildNum,
                ChildActualNum = n.ChildActualNum,
                ChildPrice = n.ChildPrice,
                PriceRepair = n.PriceRepair,
                PriceRepairRemark = n.PriceRepairRemark,
                TotalRemark = n.TotalRemark,
                OperatingPoint = n.OperatingPoint,
                ADDTS = n.ADDTS,
                ADDID = n.ADDID,
                ADDWHO = n.ADDWHO,
                EDITID = n.EDITID,
                EDITTS = n.EDITTS,
                EDITWHO = n.EDITWHO,
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData(OrderChangeViewModel orders)
        {
            var OrderCustomerRep = _unitOfWork.Repository<OrderCustomer>();
            var CostMoneyRep = _unitOfWork.Repository<CostMoney>();
            var ActualMoneyRep = _unitOfWork.Repository<ActualMoney>();
            var FinanceMoneyRep = _unitOfWork.Repository<FinanceMoney>();
            if (orders.OOCAFdeltRows != null)
            {
                var OOCAFdeltRows = orders.OOCAFdeltRows;
                if (OOCAFdeltRows.DelOrdCustomer != null && OOCAFdeltRows.DelOrdCustomer.Any())
                {
                    var data = OOCAFdeltRows.DelOrdCustomer;
                    data = data.Where(x => x > 0);
                    if (data.Any())
                    {
                        data.ToList().ForEach((i) => {
                            OrderCustomerRep.Delete(i);
                        });
                    }
                }
                if (OOCAFdeltRows.DelCostMoney != null && OOCAFdeltRows.DelCostMoney.Any())
                {
                    var data = OOCAFdeltRows.DelCostMoney;
                    data = data.Where(x => x > 0);
                    if (data.Any())
                    {
                        data.ToList().ForEach((i) =>
                        {
                            CostMoneyRep.Delete(i);
                        });
                    }
                }
                if (OOCAFdeltRows.DelActualMoney != null && OOCAFdeltRows.DelActualMoney.Any())
                {
                    var data = OOCAFdeltRows.DelActualMoney;
                    data = data.Where(x => x > 0);
                    if (data.Any())
                    {
                        data.ToList().ForEach((i) =>
                        {
                            ActualMoneyRep.Delete(i);
                        });
                    }
                }
                if (OOCAFdeltRows.DelFinanceMoney != null && OOCAFdeltRows.DelFinanceMoney.Any())
                {
                    var data = OOCAFdeltRows.DelFinanceMoney;
                    data = data.Where(x => x > 0);
                    if (data.Any())
                    {
                        data.ToList().ForEach((i) =>
                        {
                            FinanceMoneyRep.Delete(i);
                        });
                    }
                }
            }

            if (orders.updated != null)
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

                var ArrOrderId = orders.updated.Select(x => (int?)x.Order.Id);//订单主键

                #region 线程池 获取数据

                //IEnumerable<OrderCustomer> ArrOrderCustomer = null;//团客信息
                //IEnumerable<CostMoney> ArrCostMoney = null;//预算成本
                //IEnumerable<ActualMoney> ArrActualMoney = null;//实际成本
                //IEnumerable<FinanceMoney> ArrFinanceMoney = null;//财务请款

                //List<Task> ArrTask = new List<Task>();
                //ArrTask.Add(Task.Run(() =>
                //{
                //    var OrderCustomer_Rep = _unitOfWork.Repository<OrderCustomer>();
                //    ArrOrderCustomer = OrderCustomer_Rep.Queryable().AsNoTracking().Where(x => ArrOrderId.Contains(x.OrderId)).Include(x => x.OCustomer).ToList();
                //}));//订单客户
                //ArrTask.Add(Task.Run(() =>
                //{
                //    var CostMoney_Rep = _unitOfWork.Repository<CostMoney>();
                //    ArrCostMoney = CostMoney_Rep.Queryable().AsNoTracking().Where(x => ArrOrderId.Contains(x.OrderId)).ToList();
                //}));//预算成本
                //ArrTask.Add(Task.Run(() =>
                //{
                //    var ActualMoney_Rep = _unitOfWork.Repository<ActualMoney>();
                //    ArrActualMoney = ActualMoney_Rep.Queryable().AsNoTracking().Where(x => ArrOrderId.Contains(x.OrderId)).ToList();
                //}));//实际成本
                //ArrTask.Add(Task.Run(() =>
                //{
                //    var FinanceMoney_Rep = _unitOfWork.Repository<FinanceMoney>();
                //    ArrFinanceMoney = FinanceMoney_Rep.Queryable().AsNoTracking().Where(x => ArrOrderId.Contains(x.OrderId)).ToList();
                //}));//请款
                //Task.WaitAll(ArrTask.ToArray());

                #endregion

                foreach (var updated in orders.updated)
                {
                    _orderService.Update(updated.Order);
                    if (updated.ArrOrderCustomer != null && updated.ArrOrderCustomer.Any())
                    {
                        foreach (var item in updated.ArrOrderCustomer)
                        {
                            if (item.Id <= 0)
                            {
                                var OOrderCustomer = new OrderCustomer();
                                OOrderCustomer.OrderId = updated.Order.Id;
                                if (item.CustomerId == null || item.CustomerId <= 0)
                                {

                                }
                                OOrderCustomer.CustomerId = item.CustomerId;
                                OrderCustomerRep.Insert(OOrderCustomer);
                            }
                        }
                    }
                    if (updated.ArrCostMoney != null && updated.ArrCostMoney.Any())
                    {
                        foreach (var item in updated.ArrCostMoney)
                        {
                            if(item.Id>0)
                                CostMoneyRep.Update(item);
                            else
                            {
                                CostMoneyRep.Insert(item);
                            }
                            //含有 预算成本时 才会有财务情况
                            if (updated.ArrFinanceMoney != null && updated.ArrFinanceMoney.Any())
                            {
                                var QArrFinanceMoney = updated.ArrFinanceMoney.Where(x => x.CostMoneyId == item.Id);
                                foreach (var itemFM in QArrFinanceMoney)
                                {
                                    if(itemFM.Id<=0)
                                        FinanceMoneyRep.Insert(itemFM);
                                    else
                                        FinanceMoneyRep.Update(itemFM);
                                }
                            }
                        }
                    }
                    //if (updated.ArrFinanceMoney != null && updated.ArrFinanceMoney.Any())
                    //{
                    //    var QArrFinanceMoney = updated.ArrFinanceMoney.Where(x => x.Id > 0);
                    //    foreach (var item in QArrFinanceMoney)
                    //    {
                    //        FinanceMoneyRep.Update(item);
                    //    }
                    //}
                    if (updated.ArrActualMoney != null && updated.ArrActualMoney.Any())
                    {
                        foreach (var item in updated.ArrActualMoney)
                        {
                            if (item.Id > 0)
                                ActualMoneyRep.Update(item);
                            else
                                ActualMoneyRep.Insert(item);
                        }
                    }
                }
            }
            if (orders.deleted != null)
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

                foreach (var deleted in orders.deleted)
                {
                    var QOrderCustomer = OrderCustomerRep.Queryable().Where(x => x.OrderId == deleted.Order.Id);
                    var QCostMoney = CostMoneyRep.Queryable().Where(x => x.OrderId == deleted.Order.Id);
                    var QActualMoney = ActualMoneyRep.Queryable().Where(x => x.OrderId == deleted.Order.Id);
                    var QFinanceMoney = FinanceMoneyRep.Queryable().Where(x => x.OrderId == deleted.Order.Id);
                    if (QOrderCustomer.Any())
                    {
                        foreach (var item in QOrderCustomer)
                        {
                            OrderCustomerRep.Delete(item.Id);
                        }
                    }
                    if (QCostMoney.Any())
                    {
                        //含有 预算成本时 才会有财务情况
                        if (QFinanceMoney.Any())
                        {
                            foreach (var item in QFinanceMoney)
                            {
                                FinanceMoneyRep.Delete(item.Id);
                            }
                        }
                        foreach (var item in QCostMoney)
                        {
                            CostMoneyRep.Delete(item.Id);
                        }
                    }
                    if (QActualMoney.Any())
                    {
                        foreach (var item in QActualMoney)
                        {
                            ActualMoneyRep.Delete(item.Id);
                        }
                    }
                    //先删子集
                    _orderService.Delete(deleted.Order);
                }
            }
            if (orders.inserted != null)
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

                foreach (var inserted in orders.inserted)
                {
                    //团客订单编号
                    inserted.Order.OrderNo = SequenceBuilder.NextTKOrderNo();
                    inserted.Order.ForeCastPayDate = DateTime.Parse("1753-01-01 00:00:00");
                    _orderService.Insert(inserted.Order);
                    if (inserted.ArrOrderCustomer != null && inserted.ArrOrderCustomer.Any())
                    {
                        foreach (var item in inserted.ArrOrderCustomer)
                        {
                            var OOrderCustomer = new OrderCustomer();
                            OOrderCustomer.OrderId = inserted.Order.Id;
                            OOrderCustomer.CustomerId = item.CustomerId;
                            OrderCustomerRep.Insert(OOrderCustomer);
                        }
                    }
                    if (inserted.ArrCostMoney != null && inserted.ArrCostMoney.Any())
                    {
                        foreach (var item in inserted.ArrCostMoney)
                        {
                            item.OrderId = inserted.Order.Id;
                            CostMoneyRep.Insert(item);
                        }
                        //含有 预算成本时 才会有财务情况
                        if (inserted.ArrFinanceMoney != null && inserted.ArrFinanceMoney.Any())
                        {
                            foreach (var item in inserted.ArrFinanceMoney)
                            {
                                item.OrderId = inserted.Order.Id;
                                item.CostMoneyId = inserted.Order.Id;
                                FinanceMoneyRep.Insert(item);
                            }
                        }
                    }
                    if (inserted.ArrActualMoney != null && inserted.ArrActualMoney.Any())
                    {
                        foreach (var item in inserted.ArrActualMoney)
                        {
                            item.OrderId = inserted.Order.Id;
                            ActualMoneyRep.Insert(item);
                        }
                    }
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((orders.updated != null && orders.updated.Any()) ||
                (orders.deleted != null && orders.deleted.Any()) ||
                (orders.inserted != null && orders.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(orders);
                    return Json(new { Success = true, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作", ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, TMI.Web.Models.EnumType.NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取咨询单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAdvisoryOrder()
        {
            var advisoryorderRepository = _unitOfWork.Repository<AdvisoryOrder>();
            var data = advisoryorderRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, OrderNo = n.OrderNo });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Customer 转换为 OrderCustomerView
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerable<OrderCustomerView> ConvertOrderCustomerView(IEnumerable<Customer> data)
        {
          var IdCardNoFunc = new Func<Customer, string>((OCustomer) =>
            {
                #region 证件号拼接

                var IdCardNo = "";
                if (OCustomer == null || OCustomer.Id <= 0)
                    return IdCardNo;

                var IdCardType = OCustomer.IdCardType;
                if (string.IsNullOrEmpty(IdCardType))
                {
                    #region 补齐IdCardType

                    if (!string.IsNullOrWhiteSpace(OCustomer.IdCard))//身份证
                    {
                        IdCardNo += OCustomer.IdCard + "/";
                        IdCardType += "S/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.Passpord))//护照
                    {
                        IdCardNo += OCustomer.Passpord + "/";
                        IdCardType += "H/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.HK_MacauPass))//港澳通行证 
                    {
                        IdCardNo += OCustomer.HK_MacauPass + "/";
                        IdCardType += "G/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.TaiwanPass))//台湾通行证
                    {
                        IdCardNo += OCustomer.TaiwanPass + "/";
                        IdCardType += "T/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.TWIdCard))//台胞证
                    {
                        IdCardNo += OCustomer.TWIdCard + "/";
                        IdCardType += "B/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.HometownPass))//回乡证
                    {
                        IdCardNo += OCustomer.HometownPass + "/";
                        IdCardType += "X/";
                    }
                    if (!string.IsNullOrEmpty(IdCardNo))
                    {
                        OCustomer.IdCardType = IdCardType.Substring(0, IdCardType.Length - 2);
                    }

                    #endregion
                }
                else
                {
                    #region 按IdCardType顺序 拼接IdCardNo

                    //IdCardType:身份证S = 1,护照H = 2,港澳通行证G = 3,台湾通行证T = 4,台胞证B = 5,回乡证X =6
                    var ArrIdCardType = IdCardType.Split('/');
                    if (ArrIdCardType.Any())
                    {
                        var func = new Action<string>((Id_CardType) =>
                        {
                            switch (Id_CardType)
                            {
                                case "S"://身份证
                                    IdCardNo += OCustomer.IdCard + "/";
                                    break;
                                case "H"://护照
                                    IdCardNo += OCustomer.Passpord + "/";
                                    break;
                                case "G"://港澳通行证
                                    IdCardNo += OCustomer.HK_MacauPass + "/";
                                    break;
                                case "T"://台湾通行证
                                    IdCardNo += OCustomer.TaiwanPass + "/";
                                    break;
                                case "B"://台胞证
                                    IdCardNo += OCustomer.TWIdCard + "/";
                                    break;
                                case "X"://回乡证
                                    IdCardNo += OCustomer.HometownPass + "/";
                                    break;
                            }
                        });
                        foreach (var Id_CardType in ArrIdCardType)
                        {
                            func(Id_CardType);
                        }
                    }

                    #endregion
                }
                if (!string.IsNullOrEmpty(IdCardNo))
                {
                    IdCardNo = IdCardNo.Substring(0, IdCardNo.Length - 2);
                }
                return IdCardNo;

                #endregion
            });
            var Limit_SFunc = new Func<Customer, DateTime?>((OCustomer) =>
            {
                #region 有效期开始

                DateTime? Limit_S = null;
                if (OCustomer == null || OCustomer.Id <= 0)
                    return Limit_S;

                if (OCustomer.IdCardLimit_S.HasValue)//身份证
                {
                    Limit_S = OCustomer.IdCardLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.PasspordLimit_S.HasValue)//护照
                {
                    Limit_S = OCustomer.PasspordLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.HK_MacauPassLimit_S.HasValue)//港澳通行证 
                {
                    Limit_S = OCustomer.HK_MacauPassLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.TaiwanPassLimit_S.HasValue)//台湾通行证
                {
                    Limit_S = OCustomer.TaiwanPassLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.TWIdCardLimit_S.HasValue)//台胞证
                {
                    Limit_S = OCustomer.TWIdCardLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.HometownPassLimit_S.HasValue)//回乡证
                {
                    Limit_S = OCustomer.HometownPassLimit_S.Value;
                    return Limit_S;
                }
                return Limit_S;

                #endregion
            });
            var Limit_EFunc = new Func<Customer, DateTime?>((OCustomer) =>
            {
                #region 有效期截至

                DateTime? Limit_E = null;
                if (OCustomer == null || OCustomer.Id <= 0)
                    return Limit_E;

                if (OCustomer.IdCardLimit_E.HasValue)//身份证
                {
                    Limit_E = OCustomer.IdCardLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.PasspordLimit_E.HasValue)//护照
                {
                    Limit_E = OCustomer.PasspordLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.HK_MacauPassLimit_E.HasValue)//港澳通行证 
                {
                    Limit_E = OCustomer.HK_MacauPassLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.TaiwanPassLimit_E.HasValue)//台湾通行证
                {
                    Limit_E = OCustomer.TaiwanPassLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.TWIdCardLimit_E.HasValue)//台胞证
                {
                    Limit_E = OCustomer.TWIdCardLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.HometownPassLimit_E.HasValue)//回乡证
                {
                    Limit_E = OCustomer.HometownPassLimit_E.Value;
                    return Limit_E;
                }
                return Limit_E;

                #endregion
            });
            var rows = data.Select(x => new OrderCustomerView
            {
                Id = 0,
                OrderId = 0,
                CustomerId = x.Id,
                NameChs = x.NameChs,//中文名
                NameEng = x.NameEng,//英文名
                Sex = x.Sex,//性别
                Birthday = x.Birthday,//出生年月
                BornCity = x.BornCity,//出生地
                IdCardNo = IdCardNoFunc(x),//证件号
                IdCardType = x.IdCardType,//证件类型
                CheckCity = x.CheckCity,//签发地
                Limit_S = Limit_SFunc(x),//签发日期
                Limit_E = Limit_EFunc(x),//有效期
                Remark = x.Remark,//备注
            });
            return rows;
        }

        /// <summary>
        /// 获取订单客户
        /// </summary>
        /// <param name="NameChs">客户中文名</param>
        /// <returns></returns>
        public ActionResult GetOrderCustomerByNameChs(string NameChs)
        {
            var CustomerRep = _unitOfWork.Repository<Customer>();
            var data = CustomerRep.Queryable().Where(x => x.NameChs.StartsWith(NameChs)).ToList();
            var rows = ConvertOrderCustomerView(data);
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// OrderCustomer 转换为 OrderCustomerView
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerable<OrderCustomerView> ConvertOrderCustomerView(IEnumerable<OrderCustomer> data)
        {
            var IdCardNoFunc = new Func<Customer, string>((OCustomer) =>
            {
                #region 证件号拼接

                var IdCardNo = "";
                if (OCustomer == null || OCustomer.Id <= 0)
                    return IdCardNo;

                var IdCardType = OCustomer.IdCardType;
                if (string.IsNullOrEmpty(IdCardType))
                {
                    #region 补齐IdCardType

                    if (!string.IsNullOrWhiteSpace(OCustomer.IdCard))//身份证
                    {
                        IdCardNo += OCustomer.IdCard + "/";
                        IdCardType += "S/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.Passpord))//护照
                    {
                        IdCardNo += OCustomer.Passpord + "/";
                        IdCardType += "H/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.HK_MacauPass))//港澳通行证 
                    {
                        IdCardNo += OCustomer.HK_MacauPass + "/";
                        IdCardType += "G/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.TaiwanPass))//台湾通行证
                    {
                        IdCardNo += OCustomer.TaiwanPass + "/";
                        IdCardType += "T/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.TWIdCard))//台胞证
                    {
                        IdCardNo += OCustomer.TWIdCard + "/";
                        IdCardType += "B/";
                    }
                    if (!string.IsNullOrWhiteSpace(OCustomer.HometownPass))//回乡证
                    {
                        IdCardNo += OCustomer.HometownPass + "/";
                        IdCardType += "X/";
                    }
                    if (!string.IsNullOrEmpty(IdCardNo))
                    {
                        OCustomer.IdCardType = IdCardType.Substring(0, IdCardType.Length - 2);
                    }

                    #endregion
                }
                else
                {
                    #region 按IdCardType顺序 拼接IdCardNo

                    //IdCardType:身份证S = 1,护照H = 2,港澳通行证G = 3,台湾通行证T = 4,台胞证B = 5,回乡证X =6
                    var ArrIdCardType = IdCardType.Split('/');
                    if (ArrIdCardType.Any())
                    {
                        var func = new Action<string>((Id_CardType) =>
                        {
                            switch (Id_CardType)
                            {
                                case "S"://身份证
                                    IdCardNo += OCustomer.IdCard + "/";
                                    break;
                                case "H"://护照
                                    IdCardNo += OCustomer.Passpord + "/";
                                    break;
                                case "G"://港澳通行证
                                    IdCardNo += OCustomer.HK_MacauPass + "/";
                                    break;
                                case "T"://台湾通行证
                                    IdCardNo += OCustomer.TaiwanPass + "/";
                                    break;
                                case "B"://台胞证
                                    IdCardNo += OCustomer.TWIdCard + "/";
                                    break;
                                case "X"://回乡证
                                    IdCardNo += OCustomer.HometownPass + "/";
                                    break;
                            }
                        });
                        foreach (var Id_CardType in ArrIdCardType)
                        {
                            func(Id_CardType);
                        }
                    }

                    #endregion
                }
                if (!string.IsNullOrEmpty(IdCardNo))
                {
                    IdCardNo = IdCardNo.Substring(0, IdCardNo.Length - 2);
                }
                return IdCardNo;

                #endregion
            });
            var Limit_SFunc = new Func<Customer, DateTime?>((OCustomer) =>
            {
                #region 有效期开始

                DateTime? Limit_S = null;
                if (OCustomer == null || OCustomer.Id <= 0)
                    return Limit_S;

                if (OCustomer.IdCardLimit_S.HasValue)//身份证
                {
                    Limit_S = OCustomer.IdCardLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.PasspordLimit_S.HasValue)//护照
                {
                    Limit_S = OCustomer.PasspordLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.HK_MacauPassLimit_S.HasValue)//港澳通行证 
                {
                    Limit_S = OCustomer.HK_MacauPassLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.TaiwanPassLimit_S.HasValue)//台湾通行证
                {
                    Limit_S = OCustomer.TaiwanPassLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.TWIdCardLimit_S.HasValue)//台胞证
                {
                    Limit_S = OCustomer.TWIdCardLimit_S.Value;
                    return Limit_S;
                }
                if (OCustomer.HometownPassLimit_S.HasValue)//回乡证
                {
                    Limit_S = OCustomer.HometownPassLimit_S.Value;
                    return Limit_S;
                }
                return Limit_S;

                #endregion
            });
            var Limit_EFunc = new Func<Customer, DateTime?>((OCustomer) =>
            {
                #region 有效期截至

                DateTime? Limit_E = null;
                if (OCustomer == null || OCustomer.Id <= 0)
                    return Limit_E;

                if (OCustomer.IdCardLimit_E.HasValue)//身份证
                {
                    Limit_E = OCustomer.IdCardLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.PasspordLimit_E.HasValue)//护照
                {
                    Limit_E = OCustomer.PasspordLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.HK_MacauPassLimit_E.HasValue)//港澳通行证 
                {
                    Limit_E = OCustomer.HK_MacauPassLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.TaiwanPassLimit_E.HasValue)//台湾通行证
                {
                    Limit_E = OCustomer.TaiwanPassLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.TWIdCardLimit_E.HasValue)//台胞证
                {
                    Limit_E = OCustomer.TWIdCardLimit_E.Value;
                    return Limit_E;
                }
                if (OCustomer.HometownPassLimit_E.HasValue)//回乡证
                {
                    Limit_E = OCustomer.HometownPassLimit_E.Value;
                    return Limit_E;
                }
                return Limit_E;

                #endregion
            });
            var rows = data.Select(x => new OrderCustomerView
            {
                Id = x.Id,
                OrderId = x.OrderId,
                CustomerId = x.CustomerId,
                NameChs = x.OCustomer.NameChs,//中文名
                NameEng = x.OCustomer.NameEng,//英文名
                Sex = x.OCustomer.Sex,//性别
                Birthday = x.OCustomer.Birthday,//出生年月
                BornCity = x.OCustomer.BornCity,//出生地
                IdCardNo = IdCardNoFunc(x.OCustomer),//证件号
                IdCardType = x.OCustomer.IdCardType,//证件类型
                CheckCity = x.OCustomer.CheckCity,//签发地
                Limit_S = Limit_SFunc(x.OCustomer),//签发日期
                Limit_E = Limit_EFunc(x.OCustomer),//有效期
                Remark = x.OCustomer.Remark,//备注
            });
            return rows;
        }

        /// <summary>
        /// 获取订单客户
        /// </summary>
        /// <param name="OrderId">订单Id</param>
        /// <returns></returns>
        public ActionResult GetOrderCustomerById(int OrderId)
        {
            var OrderCustomerRep = _unitOfWork.Repository<OrderCustomer>();
            var data = OrderCustomerRep.Queryable().Where(x => x.OrderId == OrderId).Include(x => x.OCustomer).ToList();
            var rows = ConvertOrderCustomerView(data);
            return Json(new { Success = true, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取预算成本
        /// </summary>
        /// <param name="OrderId">订单Id</param>
        /// <returns></returns>
        public ActionResult GetCostMoneyById(int OrderId)
        {
            var CostMoneyRep = _unitOfWork.Repository<CostMoney>();
            var data = CostMoneyRep.Queryable().Where(x => x.OrderId == OrderId).ToList();
            var rows = data;
            return Json(new { Success = true, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取实际成本
        /// </summary>
        /// <param name="OrderId">订单Id</param>
        /// <returns></returns>
        public ActionResult GetActualMoneyById(int OrderId)
        {
            var ActualMoneyRep = _unitOfWork.Repository<ActualMoney>();
            var data = ActualMoneyRep.Queryable().Where(x => x.OrderId == OrderId).ToList();
            var rows = data;
            return Json(new { Success = true, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取财务请款
        /// </summary>
        /// <param name="OrderId">订单Id</param>
        /// <returns></returns>
        public ActionResult GetFinanceMoneyById(int OrderId)
        {
            var FinanceMoneyRep = _unitOfWork.Repository<FinanceMoney>();
            var data = FinanceMoneyRep.Queryable().Where(x => x.OrderId == OrderId).ToList();
            var rows = data;
            return Json(new { Success = true, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = _orderService.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            Order order = new Order();
            //set default value
            var advisoryorderRepository = _unitOfWork.Repository<AdvisoryOrder>();
            ViewBag.AdvisoryOrderId = new SelectList(advisoryorderRepository.Queryable(), "Id", "OrderNo");
            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArrOrderCustomer,OAdvisoryOrder,Id,OrderNo,AdvisoryOrderId,AdvsyOrdNo,TravleOrdType,TravleType,STravleDate,ETravleDate,RouteNo,RouteName,Saller,OP,ForeCastPayDate,SupplyierNo,SupplierName,Contact,PayType,RoutePhoto,Remark,Status,AuditStatus,AdultNum,AdultActualNum,AdultPrice,BoyNum,BoyActualNum,BoyPrice,ChildNum,ChildActualNum,ChildPrice,PriceRepair,PriceRepairRemark,TotalRemark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Order order)
        {
            if (ModelState.IsValid)
            {
                _orderService.Insert(order);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Order record");
                return RedirectToAction("Index");
            }

            var advisoryorderRepository = _unitOfWork.Repository<AdvisoryOrder>();
            ViewBag.AdvisoryOrderId = new SelectList(advisoryorderRepository.Queryable(), "Id", "OrderNo", order.AdvisoryOrderId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = _orderService.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            var advisoryorderRepository = _unitOfWork.Repository<AdvisoryOrder>();
            ViewBag.AdvisoryOrderId = new SelectList(advisoryorderRepository.Queryable(), "Id", "OrderNo", order.AdvisoryOrderId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArrOrderCustomer,OAdvisoryOrder,Id,OrderNo,AdvisoryOrderId,AdvsyOrdNo,TravleOrdType,TravleType,STravleDate,ETravleDate,RouteNo,RouteName,Saller,OP,ForeCastPayDate,SupplyierNo,SupplierName,Contact,PayType,RoutePhoto,Remark,Status,AuditStatus,AdultNum,AdultActualNum,AdultPrice,BoyNum,BoyActualNum,BoyPrice,ChildNum,ChildActualNum,ChildPrice,PriceRepair,PriceRepairRemark,TotalRemark,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.ObjectState = ObjectState.Modified;
                _orderService.Update(order);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Order record");
                return RedirectToAction("Index");
            }
            var advisoryorderRepository = _unitOfWork.Repository<AdvisoryOrder>();
            ViewBag.AdvisoryOrderId = new SelectList(advisoryorderRepository.Queryable(), "Id", "OrderNo", order.AdvisoryOrderId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = _orderService.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = _orderService.Find(id);
            _orderService.Delete(order);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Order record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "orders_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _orderService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
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
