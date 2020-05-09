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

namespace TMI.Web.Controllers
{
    public class AdvisoryOrdersController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<AdvisoryOrder>, Repository<AdvisoryOrder>>();
        //container.RegisterType<IAdvisoryOrderService, AdvisoryOrderService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IAdvisoryOrderService _advisoryOrderService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/AdvisoryOrders";

        public AdvisoryOrdersController(IAdvisoryOrderService advisoryOrderService, IUnitOfWorkAsync unitOfWork)
        {
            _advisoryOrderService = advisoryOrderService;
            _unitOfWork = unitOfWork;
        }

        // GET: AdvisoryOrders/Index
        public ActionResult Index()
        {
            //var advisoryorder = _advisoryOrderService.Queryable().Include(a => a.OCompany).Include(a => a.OCustomer).AsQueryable();
            //return View(advisoryorder);
            return View();
        }

        // Get :AdvisoryOrders/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var advisoryorder = _advisoryOrderService.Query(new AdvisoryOrderQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            #region 取缓存数据

            var ArrTravleTypeEnum = Common.GetEnumToDic("TravleTypeEnum", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });//旅游类型
            var ArrTravleOrdTypeEnum = Common.GetEnumToDic("TravleOrdTypeEnum", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });//旅游订单类型
            var ArrOrderStatusEnum = Common.GetEnumToDic("OrderStatusEnum", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });//订单状态类型

            var ArrContactType = Common.GetEnumToDic("ContactType", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });//联系方式
            var ArrCustomerLevel = Common.GetEnumToDic("CustomerLevel", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });//客户等级
            var ArrCustomerSource = Common.GetEnumToDic("CustomerSource", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });//客户来源
            var ArrCustomerType = Common.GetEnumToDic("CustomerType", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });//客户类型
            var ArrActiveStatus = Common.GetEnumToDic("ActiveStatus", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });//活跃状态
            //var ArrCompany = (List<Company>)CacheHelper.Get_SetCache(Common.CacheNameS.Company);// 公司

            #endregion

            var datarows = (from n in advisoryorder
                            join atmp in ArrTravleTypeEnum on (int)n.TravleType equals atmp.Value into a_tmp
                            from a in a_tmp.DefaultIfEmpty()
                            join btmp in ArrTravleOrdTypeEnum on (int)n.TravleOrdType equals btmp.Value into b_tmp
                            from b in b_tmp.DefaultIfEmpty()
                            join ctmp in ArrContactType on (int)n.ContactType equals ctmp.Value into c_tmp
                            from c in c_tmp.DefaultIfEmpty()
                            join dtmp in ArrCustomerLevel on (int)n.CustomerLevel equals dtmp.Value into d_tmp
                            from d in d_tmp.DefaultIfEmpty()
                            join etmp in ArrCustomerSource on (int)n.CustomerSource equals etmp.Value into e_tmp
                            from e in e_tmp.DefaultIfEmpty()
                            join ftmp in ArrCustomerType on (int)n.CustomerType equals ftmp.Value into f_tmp
                            from f in f_tmp.DefaultIfEmpty()
                            join gtmp in ArrActiveStatus on (int)n.ActiveStatus equals gtmp.Value into g_tmp
                            from g in g_tmp.DefaultIfEmpty()
                            join htmp in ArrOrderStatusEnum on (int)n.Status equals htmp.Value into h_tmp
                            from h in h_tmp.DefaultIfEmpty()
                            select new
                            {
                                Id = n.Id,
                                OrderNo = n.OrderNo,
                                TravleType = n.TravleType,
                                TravleTypeNAME = a == null ? string.Empty : a.DisplayName,
                                n.TravleOrdType,
                                TravleOrdTypeNAME = b == null ? string.Empty : b.DisplayName,
                                RouteNo = n.RouteNo,
                                //STravleDate = n.STravleDate,
                                //ETravleDate = n.ETravleDate,
                                TravlePersonNum = n.TravlePersonNum,
                                UnitPrice = n.UnitPrice,
                                TotalPrice = n.TotalPrice,
                                RoutePhoto = n.RoutePhoto,
                                Remark = n.Remark,
                                Status = n.Status,
                                StatusNAME = h == null ? string.Empty : h.DisplayName,
                                CustomerId = n.CustomerId,
                                CustomerName = n.CustomerName,
                                ContactType = n.ContactType,
                                ContactTypeNAME = c == null ? string.Empty : c.DisplayName,
                                Contact = n.Contact,
                                Saller = n.Saller,
                                OP = n.OP,
                                CustomerType = n.CustomerType,
                                CustomerTypeNAME = f == null ? string.Empty : f.DisplayName,
                                CustomerSource = n.CustomerSource,
                                CustomerSourceNAME = e == null ? string.Empty : e.DisplayName,
                                ActiveStatus = n.ActiveStatus,
                                ActiveStatusNAME = g == null ? string.Empty : g.DisplayName,
                                CustomerLevel = n.CustomerLevel,
                                CustomerLevelNAME = d == null ? string.Empty : d.DisplayName,
                                ComponyName = n.ComponyName,
                                ComponyId = n.ComponyId,
                                n.RouteName,
                                n.CustomerRequire,
                                OperatingPoint = n.OperatingPoint,
                                n.ADDID,
                                n.ADDTS,
                                n.ADDWHO,
                                n.EDITID,
                                n.EDITTS,
                                n.EDITWHO,
                            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ajax 保存数据
        /// </summary>
        /// <param name="advisoryorder"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData(AdvisoryOrderChangeViewModel advisoryorder)
        {
            var DbCTxt = _unitOfWork.getDbContext();
            var SupplyierAskPriceRep = _unitOfWork.Repository<SupplyierAskPrice>();
            int AsOrdNum = 0, SpyAskPrcNum = 0;
            if (advisoryorder.updated != null)
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

                var ArrAdvisoryOrderId = advisoryorder.updated.Select(n => (int?)n.Id);
                var QSupplyierAskPrice = SupplyierAskPriceRep.Queryable().AsNoTracking().Where(x => ArrAdvisoryOrderId.Contains(x.AdvisoryOrderId)).ToList();

                foreach (var updated in advisoryorder.updated)
                {
                    var ArrSpyAskPrcId = new List<int>();
                    //_advisoryOrderService.Update(updated);
                    if (updated.ArrSupplyierAskPrice.Any())
                    {
                        foreach (var item in updated.ArrSupplyierAskPrice)
                        {
                            if (item.Id <= 0)
                            {
                                item.Id = --SpyAskPrcNum;
                                item.AdvisoryOrderId = updated.Id;
                                item.OAdvisoryOrder = null;
                                item.ObjectState = ObjectState.Added;
                                DbCTxt.Entry(item).State = EntityState.Added;
                            }
                            else
                            {
                                ArrSpyAskPrcId.Add(item.Id);
                                item.ObjectState = ObjectState.Modified;
                                DbCTxt.Entry(item).State = EntityState.Modified;
                            }
                        }
                    }
                    updated.ObjectState = ObjectState.Modified;
                    DbCTxt.Entry(updated).State = EntityState.Modified;

                    #region 删除

                    if (ArrSpyAskPrcId.Any())
                    {
                        var ArrSupplyierAskPrice = QSupplyierAskPrice.Where(x => x.AdvisoryOrderId == updated.Id && !ArrSpyAskPrcId.Contains(x.Id)).ToList();
                        foreach (var item in ArrSupplyierAskPrice)
                        {
                            //var OSet = DbCTxt.Set<SupplyierAskPrice>().Attach(item);
                            item.OAdvisoryOrder = null;
                            item.ObjectState = ObjectState.Deleted;
                            var OEntry = DbCTxt.Entry(item);
                            OEntry.State = EntityState.Deleted;
                        }
                    }

                    #endregion

                    // 新增/更新 潜在客户
                    Add_UpdateCustomer(updated);
                }
            }
            if (advisoryorder.deleted != null)
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

                var ArrSpyAskPrcId = advisoryorder.deleted.Select(n => (int?)n.Id);
                var QSupplyierAskPrice = SupplyierAskPriceRep.Queryable().Where(x => ArrSpyAskPrcId.Contains(x.AdvisoryOrderId)).ToList();

                foreach (var deleted in advisoryorder.deleted)
                {
                    var ArrSupplyierAskPrice = QSupplyierAskPrice.Where(x => x.AdvisoryOrderId == deleted.Id).ToList();
                    if (ArrSupplyierAskPrice.Any())
                    {
                        var len = ArrSupplyierAskPrice.Count - 1;
                        for (var i = len; i >= 0; i--)
                        {
                            var item = ArrSupplyierAskPrice[i];
                            if (item.Id > 0)
                            {
                                item.OAdvisoryOrder = null;
                                item.ObjectState = ObjectState.Deleted;
                                DbCTxt.Entry(item).State = EntityState.Deleted;
                            }
                        }
                    }
                    deleted.ObjectState = ObjectState.Deleted;
                    DbCTxt.Entry(deleted).State = EntityState.Deleted;
                }
            }
            if (advisoryorder.inserted != null)
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

                foreach (var inserted in advisoryorder.inserted)
                {
                    inserted.Id = --AsOrdNum;
                    inserted.ObjectState = ObjectState.Added;
                    DbCTxt.Entry(inserted).State = EntityState.Added;
                    // 获取咨询单流水号
                    inserted.OrderNo = SequenceBuilder.NextAdvisoryOrdNo();
                    if (inserted.ArrSupplyierAskPrice.Any())
                    {
                        foreach (var item in inserted.ArrSupplyierAskPrice)
                        {
                            if (item.Id <= 0)
                            {
                                item.Id = --SpyAskPrcNum;
                                item.AdvisoryOrderId = inserted.Id;
                                //item.OAdvisoryOrder = null;
                                item.ObjectState = ObjectState.Added;
                                DbCTxt.Entry(item).State = EntityState.Added;
                            }
                            else
                            {
                                item.ObjectState = ObjectState.Modified;
                                DbCTxt.Entry(item).State = EntityState.Modified;
                            }
                        }
                    }
                    // 新增/更新 潜在客户
                    Add_UpdateCustomer(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((advisoryorder.updated != null && advisoryorder.updated.Any()) ||
                (advisoryorder.deleted != null && advisoryorder.deleted.Any()) ||
                (advisoryorder.inserted != null && advisoryorder.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(advisoryorder);
                    return Json(new { Success = true, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作", ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, EnumType.NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 新增/更新 潜在客户
        /// 客户姓名和联系方式
        /// </summary>
        public void Add_UpdateCustomer(AdvisoryOrder OAdvisoryOrder)
        {
            if (OAdvisoryOrder != null)
            {
                var dbContext = _unitOfWork.getDbContext();
                var CustomerRep = _unitOfWork.Repository<Customer>();
                var Name = OAdvisoryOrder.CustomerName;//客户姓名
                var ContactType = OAdvisoryOrder.ContactType;//联系方式
                var Contact = OAdvisoryOrder.Contact;//联系方式
                var QCustomer = CustomerRep.Queryable().Where(x => x.NameChs == Name && x.ContactType == ContactType && x.Contact == Contact).ToList();
                Customer OCustomer = new Customer();
                if (QCustomer.Any())
                {
                    OCustomer = QCustomer.FirstOrDefault();
                    OAdvisoryOrder.CustomerId = OCustomer.Id;//客户
                }
                OCustomer.NameChs = Name;//客户姓名
                OCustomer.NamePinYin = PingYinHelper.ConvertToAllSpell(Name);//客户姓名 拼英
                if (string.IsNullOrEmpty(OCustomer.NameEng))
                    OCustomer.NameEng = OCustomer.NamePinYin;//客户姓名 拼英
                OCustomer.ContactType = OAdvisoryOrder.ContactType;//联系方式
                OCustomer.Contact = OAdvisoryOrder.Contact;//联系方式
                OCustomer.Saller = OAdvisoryOrder.Saller;//销售
                OCustomer.OP = OAdvisoryOrder.OP;//客服
                OCustomer.CustomerType = OAdvisoryOrder.CustomerType;//客户类型EnumType.CustomerType
                OCustomer.CustomerSource = OAdvisoryOrder.CustomerSource;//客户来源EnumType.CustomerSource
                OCustomer.ActiveStatus = OAdvisoryOrder.ActiveStatus;//活跃状态EnumType.ActiveStatus 
                OCustomer.CustomerLevel = OAdvisoryOrder.CustomerLevel;//商机等级EnumType.CustomerLevel
                OCustomer.ComponyName = OAdvisoryOrder.ComponyName;//公司名称
                OCustomer.ComponyId = OAdvisoryOrder.ComponyId;//公司
                OCustomer.Birthday = DateTime.Parse("1753-01-01 00:00:00");
                var OEntry = dbContext.Entry(OCustomer);
                if (OCustomer.Id <= 0)
                {
                    OCustomer.CustomerNo = SequenceBuilder.NextCustomerNo();
                    OCustomer.ObjectState = ObjectState.Added;
                    OEntry.State = EntityState.Added;
                }
                else
                {
                    OCustomer.ObjectState = ObjectState.Modified;
                    OEntry.Property(x => x.Contact).IsModified = true;
                    OEntry.Property(x => x.Saller).IsModified = true;
                    OEntry.Property(x => x.OP).IsModified = true;
                    OEntry.Property(x => x.CustomerType).IsModified = true;
                    OEntry.Property(x => x.CustomerSource).IsModified = true;
                    OEntry.Property(x => x.ActiveStatus).IsModified = true;
                    OEntry.Property(x => x.CustomerLevel).IsModified = true;
                    OEntry.Property(x => x.ComponyName).IsModified = true;
                    OEntry.Property(x => x.ComponyId).IsModified = true;
                }
            }
        }

        /// <summary>
        /// 获取公司
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCompany()
        {
            var companyRepository = _unitOfWork.Repository<Company>();
            var data = companyRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, CIQID = n.CIQID });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取客户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCustomer()
        {
            var customerRepository = _unitOfWork.Repository<Customer>();
            var data = customerRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, NameChs = n.NameChs });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        // GET: AdvisoryOrders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdvisoryOrder advisoryOrder = _advisoryOrderService.Find(id);
            if (advisoryOrder == null)
            {
                return HttpNotFound();
            }
            return View(advisoryOrder);
        }

        // GET: AdvisoryOrders/Create
        public ActionResult Create()
        {
            AdvisoryOrder advisoryOrder = new AdvisoryOrder();
            //set default value
            var companyRepository = _unitOfWork.Repository<Company>();
            ViewBag.ComponyId = new SelectList(companyRepository.Queryable(), "Id", "CIQID");
            var customerRepository = _unitOfWork.Repository<Customer>();
            ViewBag.CustomerId = new SelectList(customerRepository.Queryable(), "Id", "NameChs");
            return View(advisoryOrder);
        }

        // POST: AdvisoryOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OCompany,OCustomer,Id,OrderNo,TravleType,RouteNo,STravleDate,ETravleDate,TravlePersonNum,UnitPrice,TotalPrice,RoutePhoto,Remark,Status,CustomerId,CustomerName,ContactType,Contact,Saller,OP,CustomerType,CustomerSource,ActiveStatus,CustomerLevel,ComponyName,ComponyId,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] AdvisoryOrder advisoryOrder)
        {
            if (ModelState.IsValid)
            {
                _advisoryOrderService.Insert(advisoryOrder);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a AdvisoryOrder record");
                return RedirectToAction("Index");
            }

            var companyRepository = _unitOfWork.Repository<Company>();
            ViewBag.ComponyId = new SelectList(companyRepository.Queryable(), "Id", "CIQID", advisoryOrder.ComponyId);
            var customerRepository = _unitOfWork.Repository<Customer>();
            ViewBag.CustomerId = new SelectList(customerRepository.Queryable(), "Id", "NameChs", advisoryOrder.CustomerId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(advisoryOrder);
        }

        // GET: AdvisoryOrders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdvisoryOrder advisoryOrder = _advisoryOrderService.Find(id);
            if (advisoryOrder == null)
            {
                return HttpNotFound();
            }
            var companyRepository = _unitOfWork.Repository<Company>();
            ViewBag.ComponyId = new SelectList(companyRepository.Queryable(), "Id", "CIQID", advisoryOrder.ComponyId);
            var customerRepository = _unitOfWork.Repository<Customer>();
            ViewBag.CustomerId = new SelectList(customerRepository.Queryable(), "Id", "NameChs", advisoryOrder.CustomerId);
            return View(advisoryOrder);
        }

        // POST: AdvisoryOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OCompany,OCustomer,Id,OrderNo,TravleType,RouteNo,STravleDate,ETravleDate,TravlePersonNum,UnitPrice,TotalPrice,RoutePhoto,Remark,Status,CustomerId,CustomerName,ContactType,Contact,Saller,OP,CustomerType,CustomerSource,ActiveStatus,CustomerLevel,ComponyName,ComponyId,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] AdvisoryOrder advisoryOrder)
        {
            if (ModelState.IsValid)
            {
                advisoryOrder.ObjectState = ObjectState.Modified;
                _advisoryOrderService.Update(advisoryOrder);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a AdvisoryOrder record");
                return RedirectToAction("Index");
            }
            var companyRepository = _unitOfWork.Repository<Company>();
            ViewBag.ComponyId = new SelectList(companyRepository.Queryable(), "Id", "CIQID", advisoryOrder.ComponyId);
            var customerRepository = _unitOfWork.Repository<Customer>();
            ViewBag.CustomerId = new SelectList(customerRepository.Queryable(), "Id", "NameChs", advisoryOrder.CustomerId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(advisoryOrder);
        }

        // GET: AdvisoryOrders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdvisoryOrder advisoryOrder = _advisoryOrderService.Find(id);
            if (advisoryOrder == null)
            {
                return HttpNotFound();
            }
            return View(advisoryOrder);
        }

        // POST: AdvisoryOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdvisoryOrder advisoryOrder = _advisoryOrderService.Find(id);
            _advisoryOrderService.Delete(advisoryOrder);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a AdvisoryOrder record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "advisoryorder_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _advisoryOrderService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        #region 供应商报价

        /// <summary>
        /// 获取供应商报价
        /// </summary>
        /// <param name="Id">咨询单Id</param>
        /// <returns></returns>
        public ActionResult GetSplyAskPrcById(int Id, bool? Target = null)
        {
            var SupplyierAskPriceRep = _unitOfWork.Repository<SupplyierAskPrice>();
            if (Id > 0)
            {
                var Arr = SupplyierAskPriceRep.Queryable().Where(x => x.AdvisoryOrderId == Id);
                if (Target.HasValue)
                    Arr = Arr.Where(x => x.Target == Target.Value);

                #region 取缓存数据

                var ArrBD_DEFDOC_LIST = (List<BD_DEFDOC_LIST>)CacheHelper.Get_SetCache(Common.CacheNameS.BD_DEFDOC_LIST);
                var ArrServiceProject = ArrBD_DEFDOC_LIST.Where(x => x.DOCCODE == "ServiceProject").ToList();//服务类型

                #endregion

                var data = Arr.ToList();
                var rows = from x in data
                           join a_tmp in ArrServiceProject on x.ServiceProject equals a_tmp.LISTCODE into atmp
                           from a in atmp.DefaultIfEmpty()
                           select new
                           {
                               x.Id,
                               x.AdvisoryOrderId,
                               x.SupplierName,
                               x.ServiceProject,
                               ServiceProjectNAME = a == null ? string.Empty : a.LISTNAME,
                               x.Price,
                               x.RouteName,
                               x.Target,
                               x.Remark,
                               x.OperatingPoint,
                               x.ADDID,
                               x.ADDTS,
                               x.ADDWHO,
                               x.EDITID,
                               x.EDITTS,
                               x.EDITWHO
                           };

                return Json(new { Success = true, rows = rows }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Success = false, ErrMsg = "咨询单Id，未提供" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

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