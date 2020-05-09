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
    public class CustomersController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Customer>, Repository<Customer>>();
        //container.RegisterType<ICustomerService, CustomerService>();
        //private WebdbContext db = new WebdbContext();

        private readonly ICustomerService _customerService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        //验证权限的名称
        private string ControllerQXName = "/Customers";

        public CustomersController(ICustomerService customerService, IUnitOfWorkAsync unitOfWork)
        {
            _customerService = customerService;
            _unitOfWork = unitOfWork;
        }

        // GET: Customers/Index
        public ActionResult Index()
        {
            //var customer  = _customerService.Queryable().Include(c => c.OCompany).AsQueryable();
            //return View(customer);
            return View();
        }

        // Get :Customers/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;

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
            });// 客户等级
            var ArrCustomerSource = Common.GetEnumToDic("CustomerSource", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });// 客户来源
            var ArrCustomerType = Common.GetEnumToDic("CustomerType", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });// 客户类型
            var ArrActiveStatus = Common.GetEnumToDic("ActiveStatus", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });// 活跃状态
            var ArrStatus = Common.GetEnumToDic("UseStatusEnum", "TMI.Web.Models.EnumType").Select(n => new
            {
                ID = n.Value.ToString(),
                TEXT = n.DisplayName,
                Value = (int)n.Value,
                DisplayName = n.DisplayName
            });//使用状态
            var ArrCompany = (List<Company>)CacheHelper.Get_SetCache(Common.CacheNameS.Company);// 公司

            var CustomerRep = _unitOfWork.RepositoryAsync<Customer>();
            var customer = _customerService.Query(new CustomerQuery().Withfilter(filters)).
                OrderBy(n => n.OrderBy(sort, order)).
                SelectPage(page, rows, out totalCount);

            var datarows = (from n in customer
                            join a_tmp in ArrContactType on (int)n.ContactType equals a_tmp.Value into atmp
                            from a in atmp.DefaultIfEmpty()
                            join b_tmp in ArrCustomerLevel on (int)n.CustomerLevel equals b_tmp.Value into btmp
                            from b in btmp.DefaultIfEmpty()
                            join c_tmp in ArrCustomerSource on (int)n.CustomerSource equals c_tmp.Value into ctmp
                            from c in ctmp.DefaultIfEmpty()
                            join d_tmp in ArrCustomerType on (int)n.CustomerType equals d_tmp.Value into dtmp
                            from d in dtmp.DefaultIfEmpty()
                            join e_tmp in ArrActiveStatus on (int)n.ActiveStatus equals e_tmp.Value into etmp
                            from e in dtmp.DefaultIfEmpty()
                            join f_tmp in ArrStatus on (int)n.Status equals f_tmp.Value into ftmp
                            from f in ftmp.DefaultIfEmpty()
                            join g_tmp in ArrCompany on n.ComponyId equals g_tmp.Id into gtmp
                            from g in gtmp.DefaultIfEmpty()
                            select new
                            {
                                Id = n.Id,
                                n.CustomerNo,
                                NameChs = n.NameChs,
                                NamePinYin = n.NamePinYin,
                                NameEng = n.NameEng,
                                Sex = n.Sex,
                                Birthday = n.Birthday,
                                AirLineMember = n.AirLineMember,
                                Saller = n.Saller,
                                OP = n.OP,
                                Contact = n.Contact,
                                ContactType = n.ContactType,
                                ContactTypeNAME = a == null ? string.Empty : a.DisplayName,
                                CustomerLevel = n.CustomerLevel,
                                CustomerLevelNAME = b == null ? string.Empty : b.DisplayName,
                                CustomerSource = n.CustomerSource,
                                CustomerSourceNAME = c == null ? string.Empty : c.DisplayName,
                                CustomerType = n.CustomerType,
                                CustomerTypeNAME = d == null ? string.Empty : d.DisplayName,
                                ActiveStatus = n.ActiveStatus,
                                ActiveStatusNAME = e == null ? string.Empty : e.DisplayName,
                                ComponyName = n.ComponyName,
                                ComponyId = n.ComponyId,
                                ComponyIdNAME = g == null ? string.Empty : g.Name,
                                IdCard = n.IdCard,
                                IdCardLimit_S = n.IdCardLimit_S,
                                IdCardLimit_E = n.IdCardLimit_E,
                                IdCardPhoto_A = n.IdCardPhoto_A,
                                IdCardPhoto_B = n.IdCardPhoto_B,
                                Passpord = n.Passpord,
                                PasspordLimit_S = n.PasspordLimit_S,
                                PasspordLimit_E = n.PasspordLimit_E,
                                PasspordPhoto_A = n.PasspordPhoto_A,
                                PasspordPhoto_B = n.PasspordPhoto_B,
                                HK_MacauPass = n.HK_MacauPass,
                                HK_MacauPassLimit_S = n.HK_MacauPassLimit_S,
                                HK_MacauPassLimit_E = n.HK_MacauPassLimit_E,
                                HK_MacauPassPhoto_A = n.HK_MacauPassPhoto_A,
                                HK_MacauPassPhoto_B = n.HK_MacauPassPhoto_B,
                                TaiwanPass = n.TaiwanPass,
                                TaiwanPassLimit_S = n.TaiwanPassLimit_S,
                                TaiwanPassLimit_E = n.TaiwanPassLimit_E,
                                TaiwanPassPhoto_A = n.TaiwanPassPhoto_A,
                                TaiwanPassPhoto_B = n.TaiwanPassPhoto_B,
                                TWIdCard = n.TWIdCard,
                                TWIdCardLimit_S = n.TWIdCardLimit_S,
                                TWIdCardLimit_E = n.TWIdCardLimit_E,
                                TWIdCardPhoto_A = n.TWIdCardPhoto_A,
                                TWIdCardPhoto_B = n.TWIdCardPhoto_B,
                                HometownPass = n.HometownPass,
                                HometownPassLimit_S = n.HometownPassLimit_S,
                                HometownPassLimit_E = n.HometownPassLimit_E,
                                HometownPassPhoto_A = n.HometownPassPhoto_A,
                                HometownPassPhoto_B = n.HometownPassPhoto_B,
                                Remark = n.Remark,
                                Status = n.Status,
                                StatusNAME = f == null ? string.Empty : f.DisplayName,
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

        [HttpPost]
        public ActionResult SaveData(CustomerChangeViewModel customer)
        {
            if (customer.updated != null)
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

                foreach (var updated in customer.updated)
                {
                    _customerService.Update(updated);
                }
            }
            if (customer.deleted != null)
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

                foreach (var deleted in customer.deleted)
                {
                    _customerService.Delete(deleted);
                }
            }
            if (customer.inserted != null)
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

                foreach (var inserted in customer.inserted)
                {
                    inserted.CustomerNo = SequenceBuilder.NextCustomerNo();
                    _customerService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((customer.updated != null && customer.updated.Any()) ||
                (customer.deleted != null && customer.deleted.Any()) ||
                (customer.inserted != null && customer.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(customer);
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

        public ActionResult GetCompany()
        {
            var companyRepository = _unitOfWork.Repository<Company>();
            var data = companyRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, Name = n.Name });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = _customerService.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            Customer customer = new Customer();
            //set default value
            var companyRepository = _unitOfWork.Repository<Company>();
            ViewBag.ComponyId = new SelectList(companyRepository.Queryable(), "Id", "Name");
            return View(customer);
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OCompany,Id,NameChs,NamePinYin,NameEng,Sex,Birthday,AirLineMember,ContactType,Contact,ActiveStatus,Saller,OP,CustomerLevel,CustomerSource,CustomerType,ComponyName,ComponyId,IdCard,IdCardLimit_S,IdCardLimit_E,IdCardPhoto_A,IdCardPhoto_B,Passpord,PasspordLimit_S,PasspordLimit_E,PasspordPhoto_A,PasspordPhoto_B,HK_MacauPass,HK_MacauPassLimit_S,HK_MacauPassLimit_E,HK_MacauPassPhoto_A,HK_MacauPassPhoto_B,TaiwanPass,TaiwanPassLimit_S,TaiwanPassLimit_E,TaiwanPassPhoto_A,TaiwanPassPhoto_B,TWIdCard,TWIdCardLimit_S,TWIdCardLimit_E,TWIdCardPhoto_A,TWIdCardPhoto_B,HometownPass,HometownPassLimit_S,HometownPassLimit_E,HometownPassPhoto_A,HometownPassPhoto_B,Remark,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _customerService.Insert(customer);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Customer record");
                return RedirectToAction("Index");
            }

            var companyRepository = _unitOfWork.Repository<Company>();
            ViewBag.ComponyId = new SelectList(companyRepository.Queryable(), "Id", "Name", customer.ComponyId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = _customerService.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            var companyRepository = _unitOfWork.Repository<Company>();
            ViewBag.ComponyId = new SelectList(companyRepository.Queryable(), "Id", "Name", customer.ComponyId);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OCompany,Id,NameChs,NamePinYin,NameEng,Sex,Birthday,AirLineMember,ContactType,Contact,ActiveStatus,Saller,OP,CustomerLevel,CustomerSource,CustomerType,ComponyName,ComponyId,IdCard,IdCardLimit_S,IdCardLimit_E,IdCardPhoto_A,IdCardPhoto_B,Passpord,PasspordLimit_S,PasspordLimit_E,PasspordPhoto_A,PasspordPhoto_B,HK_MacauPass,HK_MacauPassLimit_S,HK_MacauPassLimit_E,HK_MacauPassPhoto_A,HK_MacauPassPhoto_B,TaiwanPass,TaiwanPassLimit_S,TaiwanPassLimit_E,TaiwanPassPhoto_A,TaiwanPassPhoto_B,TWIdCard,TWIdCardLimit_S,TWIdCardLimit_E,TWIdCardPhoto_A,TWIdCardPhoto_B,HometownPass,HometownPassLimit_S,HometownPassLimit_E,HometownPassPhoto_A,HometownPassPhoto_B,Remark,Status,OperatingPoint,ADDID,ADDWHO,ADDTS,EDITWHO,EDITTS,EDITID")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.ObjectState = ObjectState.Modified;
                _customerService.Update(customer);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Customer record");
                return RedirectToAction("Index");
            }
            var companyRepository = _unitOfWork.Repository<Company>();
            ViewBag.ComponyId = new SelectList(companyRepository.Queryable(), "Id", "Name", customer.ComponyId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = _customerService.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = _customerService.Find(id);
            _customerService.Delete(customer);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Customer record");
            return RedirectToAction("Index");
        }

        //导出Excel
        [HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "customer_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _customerService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 获取客户主键
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="BirthDay"></param>
        /// <returns></returns>
        public ActionResult GetCustomerIdByName(OrderCustomerView OrderCuntomer)
        {
            int retKey = 0;
            var CustomerRep = _unitOfWork.Repository<Customer>();
            try
            {
                var NameChs = OrderCuntomer.NameChs;
                var Birthday = OrderCuntomer.Birthday;
                var QCustomer = _customerService.Queryable().Where(x => x.NameChs == NameChs);
                if (Birthday != null)
                {
                    QCustomer = QCustomer.Where(x => x.Birthday == Birthday);
                }
                if (QCustomer.Any())
                {
                    retKey = QCustomer.FirstOrDefault().Id;
                }
                else
                {
                    var OCustomer = new Customer();
                    OCustomer.CustomerNo = SequenceBuilder.NextCustomerNo();
                    OCustomer.NameChs = OrderCuntomer.NameChs;
                    OCustomer.NamePinYin = OrderCuntomer.NameEng;
                    OCustomer.NameEng = OrderCuntomer.NameEng;
                    OCustomer.Birthday = OrderCuntomer.Birthday;
                    OCustomer.ContactType = EnumType.ContactType.微信;
                    OCustomer.ActiveStatus = EnumType.ActiveStatus.正常;
                    OCustomer.CustomerLevel = EnumType.CustomerLevel.一级;
                    OCustomer.CustomerSource = EnumType.CustomerSource.开发;
                    OCustomer.CustomerType = EnumType.CustomerType.团客;
                    OCustomer.Sex = OrderCuntomer.Sex;
                    OCustomer.BornCity = OrderCuntomer.BornCity;
                    OCustomer.CheckCity = OrderCuntomer.CheckCity;

                    #region
                    if (!string.IsNullOrEmpty(OrderCuntomer.IdCardType)) 
                    {
                        OCustomer.IdCardType = OrderCuntomer.IdCardType;

                        //IdCardType:身份证S = 1,护照H = 2,港澳通行证G = 3,台湾通行证T = 4,台胞证B = 5,回乡证X =6
                        var ArrIdCardType = OrderCuntomer.IdCardType.Trim().Split('/');
                        var ArrIdCardNo = OrderCuntomer.IdCardNo.Trim().Split('/');
                        var IdCardTypeLen = ArrIdCardType.Length;

                        if (ArrIdCardType.Any() && ArrIdCardNo.Length == IdCardTypeLen)
                        {
                            var func = new Action<string, string, DateTime?, DateTime?>((Id_CardType, Id_CardNo, Limit_S, Limit_E) =>
                            {
                                switch (Id_CardType)
                                {
                                    case "1"://身份证
                                        OCustomer.IdCard = Id_CardNo; 
                                        if (Limit_S != null)
                                        {
                                            OCustomer.IdCardLimit_S = Limit_S;
                                        }
                                        if (Limit_E != null)
                                        {
                                            OCustomer.IdCardLimit_E = Limit_E;
                                        }
                                        break;
                                    case "2"://护照
                                        OCustomer.Passpord = Id_CardNo;
                                        if (Limit_S != null)
                                        {
                                            OCustomer.PasspordLimit_S = Limit_S;
                                        }
                                        if (Limit_E != null)
                                        {
                                            OCustomer.PasspordLimit_E = Limit_E;
                                        }
                                        break;
                                    case "3"://港澳通行证
                                        OCustomer.HK_MacauPass = Id_CardNo;
                                        if (Limit_S != null)
                                        {
                                            OCustomer.HK_MacauPassLimit_S = Limit_S;
                                        }
                                        if (Limit_E != null)
                                        {
                                            OCustomer.HK_MacauPassLimit_E = Limit_E;
                                        }
                                        break;
                                    case "4"://台湾通行证
                                        OCustomer.TaiwanPass = Id_CardNo;
                                        if (Limit_S != null)
                                        {
                                            OCustomer.TaiwanPassLimit_S = Limit_S;
                                        }
                                        if (Limit_E != null)
                                        {
                                            OCustomer.TaiwanPassLimit_E = Limit_E;
                                        }
                                        break;
                                    case "5"://台胞证
                                        OCustomer.TWIdCard = Id_CardNo;
                                        if (Limit_S != null)
                                        {
                                            OCustomer.TWIdCardLimit_S = Limit_S;
                                        }
                                        if (Limit_E != null)
                                        {
                                            OCustomer.TWIdCardLimit_E = Limit_E;
                                        }
                                        break;
                                    case "6"://回乡证
                                        OCustomer.HometownPass = Id_CardNo;
                                        if (Limit_S != null)
                                        {
                                            OCustomer.HometownPassLimit_S = Limit_S;
                                        }
                                        if (Limit_E != null)
                                        {
                                            OCustomer.HometownPassLimit_E = Limit_E;
                                        }
                                        break;
                                }
                            });//设置证件号

                            int i = 0;
                            while (i < IdCardTypeLen)
                            {
                                var Id_CardType = ArrIdCardType[i];
                                var Id_CardNo = ArrIdCardNo[i];
                                var Limit_S = i == 0 ? OrderCuntomer.Limit_S : null;//签发日期
                                var Limit_E = i == 0 ? OrderCuntomer.Limit_E : null;//有效期
                                func(Id_CardType, Id_CardNo, Limit_S, Limit_E);
                                ++i;
                            }
                        }
                    }

                   #endregion

                    OCustomer.Remark = OrderCuntomer.Remark;
                    CustomerRep.Insert(OCustomer);

                    _unitOfWork.SaveChanges();

                     var QQCustomer = _customerService.Queryable().Where(x => x.NameChs == NameChs);
                     if (Birthday != null)
                     {
                        QQCustomer = QQCustomer.Where(x => x.Birthday == Birthday);
                     }
                     retKey = QQCustomer.FirstOrDefault().Id;
                  }
               
                return Json(new { Success = retKey > 0, CustomerId = retKey }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新客户数据
        /// </summary>
        /// <param name="OCustomer">客户信息</param>
        /// <returns></returns>
        public ActionResult UpdateCustomer(IEnumerable<OrderCustomerView> ArrOrdCustomerView)
        {
            try
            {
                if (ArrOrdCustomerView != null && ArrOrdCustomerView.Any())
                {
                    var ArrCustomerId = ArrOrdCustomerView.Select(x => x.CustomerId).Where(x => x != null && x > 0).ToList();
                    var ArrCustomer = _customerService.Queryable().Where(x => ArrCustomerId.Contains(x.Id)).ToList();
                    if (ArrCustomer.Any())
                    {
                        var DbContxt = _unitOfWork.getDbContext();
                        foreach (var item in ArrOrdCustomerView)
                        {
                            var OCustomer = ArrCustomer.Where(x => x.Id == item.CustomerId).FirstOrDefault();
                            if (OCustomer != null && OCustomer.Id > 0)
                            {
                                OCustomer.ObjectState = ObjectState.Modified;
                                var OEntry = DbContxt.Entry(OCustomer);
                                #region 英文名
                                if (!string.IsNullOrEmpty(item.NameEng))
                                {
                                    if (item.NameEng != OCustomer.NameEng)
                                    {
                                        OCustomer.NameEng = item.NameEng;
                                        OEntry.Property(x => x.NameEng).IsModified = true;
                                    }
                                }
                                #endregion
                                #region 性别
                                if (item.Sex != null)
                                {
                                    if (item.Sex != OCustomer.Sex)
                                    {
                                        OCustomer.Sex = item.Sex;
                                        OEntry.Property(x => x.Sex).IsModified = true;
                                    }
                                }
                                #endregion
                                #region 出生年月
                                if (item.Birthday > DateTime.MinValue)
                                {
                                    if (item.Birthday != OCustomer.Birthday)
                                    {
                                        OCustomer.Birthday = item.Birthday;
                                        OEntry.Property(x => x.Birthday).IsModified = true;
                                    }
                                }
                                #endregion
                                #region 出生地
                                if (!string.IsNullOrEmpty(item.BornCity))
                                {
                                    if (item.BornCity != OCustomer.BornCity)
                                    {
                                        OCustomer.BornCity = item.BornCity;
                                        OEntry.Property(x => x.BornCity).IsModified = true;
                                    }
                                }
                                #endregion
                                #region 签发地
                                if (!string.IsNullOrEmpty(item.CheckCity))
                                {
                                    if (item.CheckCity != OCustomer.CheckCity)
                                    {
                                        OCustomer.CheckCity = item.CheckCity;
                                        OEntry.Property(x => x.CheckCity).IsModified = true;
                                    }
                                }
                                #endregion
                                #region 证件类型 证件号 
		
                                //签发日期 有效期 只会更新第一个证件
                                if (!string.IsNullOrEmpty(item.IdCardType))
                                {
                                    if (item.IdCardType != OCustomer.IdCardType)
                                    {
                                        OCustomer.IdCardType = item.IdCardType;
                                        OEntry.Property(x => x.IdCardType).IsModified = true;

                                        #region 按IdCardType顺序 更新证件号

                                        //IdCardType:身份证S = 1,护照H = 2,港澳通行证G = 3,台湾通行证T = 4,台胞证B = 5,回乡证X =6
                                        var ArrIdCardType = item.IdCardType.Trim().Split('/');
                                        var ArrIdCardNo = item.IdCardNo.Trim().Split('/');
                                        var IdCardTypeLen = ArrIdCardType.Length;

                                        if (ArrIdCardType.Any() && ArrIdCardNo.Length == IdCardTypeLen)
                                        {
                                            #region 清空证件号

                                            //OCustomer.IdCard = "";
                                            //OEntry.Property(x => x.IdCard).IsModified = true;

                                            //OCustomer.Passpord = "";
                                            //OEntry.Property(x => x.Passpord).IsModified = true;

                                            //OCustomer.HK_MacauPass = "";
                                            //OEntry.Property(x => x.HK_MacauPass).IsModified = true;

                                            //OCustomer.TaiwanPass = "";
                                            //OEntry.Property(x => x.TaiwanPass).IsModified = true;

                                            //OCustomer.TWIdCard = "";
                                            //OEntry.Property(x => x.TWIdCard).IsModified = true;

                                            //OCustomer.HometownPass = "";
                                            //OEntry.Property(x => x.HometownPass).IsModified = true;

                                            #endregion

                                            var func = new Action<string, string, DateTime?, DateTime?>((Id_CardType, Id_CardNo, Limit_S, Limit_E) =>
                                            {
                                                switch (Id_CardType)
                                                {
                                                    case "S"://身份证
                                                        OCustomer.IdCard = Id_CardNo;
                                                        OEntry.Property(x => x.IdCard).IsModified = true;
                                                        if (Limit_S != null)
                                                        {
                                                            OCustomer.IdCardLimit_S = Limit_S;
                                                            OEntry.Property(x => x.IdCardLimit_S).IsModified = true;
                                                        }
                                                        if (Limit_E != null)
                                                        {
                                                            OCustomer.IdCardLimit_E = Limit_E;
                                                            OEntry.Property(x => x.IdCardLimit_E).IsModified = true;
                                                        }
                                                        break;
                                                    case "H"://护照
                                                        OCustomer.Passpord = Id_CardNo;
                                                        OEntry.Property(x => x.Passpord).IsModified = true;
                                                        if (Limit_S != null)
                                                        {
                                                            OCustomer.PasspordLimit_S = Limit_S;
                                                            OEntry.Property(x => x.PasspordLimit_S).IsModified = true;
                                                        }
                                                        if (Limit_E != null)
                                                        {
                                                            OCustomer.PasspordLimit_E = Limit_E;
                                                            OEntry.Property(x => x.PasspordLimit_E).IsModified = true;
                                                        }
                                                        break;
                                                    case "G"://港澳通行证
                                                        OCustomer.HK_MacauPass = Id_CardNo;
                                                        OEntry.Property(x => x.HK_MacauPass).IsModified = true;
                                                        if (Limit_S != null)
                                                        {
                                                            OCustomer.HK_MacauPassLimit_S = Limit_S;
                                                            OEntry.Property(x => x.HK_MacauPassLimit_S).IsModified = true;
                                                        }
                                                        if (Limit_E != null)
                                                        {
                                                            OCustomer.HK_MacauPassLimit_E = Limit_E;
                                                            OEntry.Property(x => x.HK_MacauPassLimit_E).IsModified = true;
                                                        }
                                                        break;
                                                    case "T"://台湾通行证
                                                        OCustomer.TaiwanPass = Id_CardNo;
                                                        OEntry.Property(x => x.TaiwanPass).IsModified = true;
                                                        if (Limit_S != null)
                                                        {
                                                            OCustomer.TaiwanPassLimit_S = Limit_S;
                                                            OEntry.Property(x => x.TaiwanPassLimit_S).IsModified = true;
                                                        }
                                                        if (Limit_E != null)
                                                        {
                                                            OCustomer.TaiwanPassLimit_E = Limit_E;
                                                            OEntry.Property(x => x.TaiwanPassLimit_E).IsModified = true;
                                                        }
                                                        break;
                                                    case "B"://台胞证
                                                        OCustomer.TWIdCard = Id_CardNo;
                                                        OEntry.Property(x => x.TWIdCard).IsModified = true;
                                                        if (Limit_S != null)
                                                        {
                                                            OCustomer.TWIdCardLimit_S = Limit_S;
                                                            OEntry.Property(x => x.TWIdCardLimit_S).IsModified = true;
                                                        }
                                                        if (Limit_E != null)
                                                        {
                                                            OCustomer.TWIdCardLimit_E = Limit_E;
                                                            OEntry.Property(x => x.TWIdCardLimit_E).IsModified = true;
                                                        }
                                                        break;
                                                    case "X"://回乡证
                                                        OCustomer.HometownPass = Id_CardNo;
                                                        OEntry.Property(x => x.HometownPass).IsModified = true;
                                                        if (Limit_S != null)
                                                        {
                                                            OCustomer.HometownPassLimit_S = Limit_S;
                                                            OEntry.Property(x => x.HometownPassLimit_S).IsModified = true;
                                                        }
                                                        if (Limit_E != null)
                                                        {
                                                            OCustomer.HometownPassLimit_E = Limit_E;
                                                            OEntry.Property(x => x.HometownPassLimit_E).IsModified = true;
                                                        }
                                                        break;
                                                }
                                            });//设置证件号

                                            int i = 0;
                                            while (i < IdCardTypeLen)
                                            {
                                                var Id_CardType = ArrIdCardType[i];
                                                var Id_CardNo = ArrIdCardNo[i];
                                                var Limit_S = i == 0 ? item.Limit_S : null;//签发日期
                                                var Limit_E = i == 0 ? item.Limit_E : null;//有效期
                                                func(Id_CardType, Id_CardNo, Limit_S, Limit_E);
                                                ++i;
                                            }
                                        }

                                        #endregion
                                    }
                                }

                                #endregion
                                #region 备注
                                if (!string.IsNullOrEmpty(item.Remark))
                                {
                                    if (item.Remark != OCustomer.Remark)
                                    {
                                        OCustomer.Remark = item.Remark;
                                        OEntry.Property(x => x.Remark).IsModified = true;
                                    }
                                }
                                #endregion
                            }
                        }
                        _unitOfWork.SaveChanges();
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { Success = false, ErrMsg = "找不到，客户数据" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有要更新的客户数据" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
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

        /// <summary>
        /// Cpu资源监控
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCpuMonitor()
        {
            System.Diagnostics.Stopwatch OStopwatch = new System.Diagnostics.Stopwatch();
            OStopwatch.Start();
            //var CpuCounter = new System.Diagnostics.PerformanceCounter();
            //CpuCounter.CategoryName = "Processor";
            //CpuCounter.CounterName = "% Processor Time";
            //CpuCounter.InstanceName = "_Total";
            //CpuCounter.MachineName = ".";
            //var CpuCounter = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total") { MachineName = "." };
            //var CpuVal = CpuCounter.NextValue();
            //CpuVal = CpuCounter.NextValue();

            //List<float> ArrCpuNum = new List<float>();
            //while (ArrCpuNum.Count < 10)
            //{
            //    ArrCpuNum.Add(CPUMonitor.getValue());
            //    System.Threading.Thread.Sleep(10);
            //}
            var OCpuMonitor = CPUMonitor.getMonitor();
            var CpuCounter = OCpuMonitor.getValue();
            OStopwatch.Stop();

            return Json(new { CpuCounter = CpuCounter, Stopwaych = OStopwatch.ElapsedMilliseconds }, JsonRequestBehavior.AllowGet);
        }
    }
}