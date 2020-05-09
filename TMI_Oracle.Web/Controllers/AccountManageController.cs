using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TMI.Web.Models;
using TMI.Web.Extensions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TMI.Web.Controllers
{
    [Authorize]
    public class AccountManageController : BaseController//Controller
    {
        /// <summary>
        /// Redis助手
        /// </summary>
        private RedisHelp.RedisHelper ORedisHelp = new RedisHelp.RedisHelper();
        private string RedisHashUserKey = "UserS";

        private ApplicationUserManager _userManager;

        public AccountManageController()
        {
        }

        public AccountManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) :
            base(false, true)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {

            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //验证权限的名称
        private string ControllerQXName = "/AccountManage";

        public ActionResult Index()
        {
            return View();
        }

        // Get :AccountManager/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult PageList(int offset = 0, int limit = 10, string search = "", string sort = "UserName", string order = "asc")
        {
            int totalCount = 0;
            int pagenum = offset / limit + 1;

            var users = _userManager.Users.Where(n => n.UserName.Contains(search) || n.Email.Contains(search) || n.PhoneNumber.Contains(search)).OrderByName(sort, order);
            totalCount = users.Count();
            var datalist = users.Skip(offset).Take(limit);
            var rows = datalist.Select(n => new
            {
                Id = n.Id,
                UserName = n.UserName,
                Email = n.Email,
                PhoneNumber = n.PhoneNumber,
                AccessFailedCount = n.AccessFailedCount,
                LockoutEnabled = n.LockoutEnabled,
                LockoutEndDateUtc = n.LockoutEndDateUtc,
                n.UserOperatPoint,
                n.DepartMent
            }).ToList();
            var pagelist = new { total = totalCount, rows = rows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;

            ApplicationDbContext AppDbContext = new ApplicationDbContext();

            var users = AppDbContext.Users.OrderByName(sort, order);
            //users = users.Where(x => x.DepartMent == null);

            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    if (filter.field == "UserNameDesc")
                    {
                        if (!string.IsNullOrWhiteSpace(filter.value))
                            users = users.Where(x => x.UserNameDesc.Contains(filter.value));
                    }
                    if (filter.field == "UserName")
                    {
                        if (!string.IsNullOrWhiteSpace(filter.value))
                            users = users.Where(x => x.UserName.Contains(filter.value));
                    }
                    if (filter.field == "Email")
                    {
                        if (!string.IsNullOrWhiteSpace(filter.value))
                            users = users.Where(x => x.Email.Contains(filter.value));
                    }
                    if (filter.field == "PhoneNumber")
                    {
                        if (!string.IsNullOrWhiteSpace(filter.value))
                            users = users.Where(x => x.PhoneNumber.Contains(filter.value));
                    }
                    if (filter.field == "_LockoutEndDateUtc")
                    {
                        if (!string.IsNullOrWhiteSpace(filter.value))
                        {
                            var _LockoutEndDateUtc = Common.ParseStrToDateTime(filter.value);
                            if (_LockoutEndDateUtc.HasValue)
                                users = users.Where(x => x.LockoutEndDateUtc >= _LockoutEndDateUtc);
                        }
                    }
                    if (filter.field == "LockoutEndDateUtc_")
                    {
                        if (!string.IsNullOrWhiteSpace(filter.value))
                        {
                            var LockoutEndDateUtc_ = Common.ParseStrToDateTime(filter.value);
                            if (LockoutEndDateUtc_.HasValue)
                                users = users.Where(x => x.LockoutEndDateUtc <= LockoutEndDateUtc_);
                        }
                    }
                    if (filter.field == "DepartMent")
                    {
                        if (!string.IsNullOrWhiteSpace(filter.value))
                            users = users.Where(x => x.DepartMent.Contains(filter.value));
                    }
                }
            }
            totalCount = users.Count();
            var datalist = users.Skip(rows * (page - 1)).Take(rows).ToList();
            var datarows = datalist.Select(n => new
            {
                n.Id,
                n.UserName,
                n.UserNameDesc,
                n.Email,
                Password = "",
                n.PhoneNumber,
                n.AccessFailedCount,
                n.LockoutEnabled,
                n.LockoutEndDateUtc,
                n.UserOperatPoint,
                n.DepartMent
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_addwho(string q = "")
        {
            ApplicationDbContext AppDbContext = new ApplicationDbContext();

            var users = AppDbContext.Users.AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                users = users.Where(x => x.UserName.Contains(q) || x.UserNameDesc.Contains(q)).OrderBy(x => x.Id);
            }

            var Arr_addwho = users.Select(x => new
            {
                ID = x.UserName,
                TEXT = x.UserNameDesc
            }).ToList();
            return Json(Arr_addwho, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveData(UserChangeViewModel users)
        {
            ApplicationUserViewModel OApplicationUserViewModel = new ApplicationUserViewModel();
            ApplicationDbContext AppDbContext = new ApplicationDbContext();
            IEnumerable<string> ArrIds = new List<string>(); //= users.updated.Select(n => n.UserName);
            if (users.updated != null)
            {
                ArrIds = users.updated.Select(n => n.Id);
            }
            if (users.deleted != null)
            {
                ArrIds = ArrIds.Union(users.deleted.Select(n => n.Id));
            }
            //if (users.inserted != null)
            //{
            //    UserNames = UserNames.Union(users.inserted.Select(n => n.UserName));
            //}
            var Users = AppDbContext.Users.Where(x => ArrIds.Contains(x.Id)).ToList();
            if (users.updated != null)
            {
                List<ApplicationUser> ArrUser = new List<ApplicationUser>();
                foreach (var updated in users.updated)
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

                    var AllUsers = AppDbContext.Users.Where(x => x.Id != updated.Id);
                    var Cuser = AllUsers.Where(x => x.UserName == updated.UserName).ToList();
                    if (Cuser.Any())
                    {
                        return Json(new { Success = false, ErrMsg = "用户名已经存在！" }, JsonRequestBehavior.AllowGet);
                    }
                    var WhereUser = Users.Where(x => x.Id == updated.Id);
                    if (WhereUser.Any())
                    {
                        var user = WhereUser.FirstOrDefault();
                        user.UserName = updated.UserName;
                        user.Email = updated.Email;
                        user.PhoneNumber = updated.PhoneNumber;
                        if (user.LockoutEnabled != updated.LockoutEnabled)
                        {
                            if (!updated.LockoutEnabled)
                            {
                                user.LockoutEndDateUtc = DateTime.Now;
                            }
                            else
                                user.LockoutEndDateUtc = updated.LockoutEndDateUtc;
                        }
                        else
                            user.LockoutEndDateUtc = updated.LockoutEndDateUtc;
                        user.LockoutEnabled = updated.LockoutEnabled;
                        user.UserNameDesc = updated.UserNameDesc;
                        user.DepartMent = updated.DepartMent;
                        if (!string.IsNullOrEmpty(updated.Password))
                        {
                            user.PasswordHash = SignInManager.UserManager.PasswordHasher.HashPassword(updated.Password);
                        }
                        AppDbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        ArrUser.Add(user);
                        //更新安全票据
                        UserManager.UpdateSecurityStampAsync(user.Id);
                        //var user = new ApplicationUser { UserName = updated.UserName, Email = updated.Email, LockoutEnabled = updated.LockoutEnabled, UserNameDesc = updated.UserNameDesc };
                        //string PasswordHasher = "";
                        //if (!string.IsNullOrEmpty(updated.Password))
                        //{
                        //    PasswordHasher = SignInManager.UserManager.PasswordHasher.HashPassword(updated.Password);
                        //    user.PasswordHash = PasswordHasher;
                        //}

                        //var result = UserManager.Update(user);
                    }
                }
                try
                {
                    AppDbContext.SaveChanges();
                    foreach (var user in ArrUser)
                    {
                        ORedisHelp.HashSet<ApplicationUser>(RedisHashUserKey, user.Id, user);
                    }
                    OApplicationUserViewModel.updated = ArrUser;
                }
                catch (Exception ex)
                {
                    var ErrMsg = Common.GetExceptionMsg(ex);
                    return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
                }
            }
            if (users.deleted != null)
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

                foreach (var deleted in users.deleted)
                {
                    var WhereUser = Users.Where(x => x.Id == deleted.Id);
                    if (WhereUser.Any())
                    {
                        var user = WhereUser.FirstOrDefault();
                        AppDbContext.Entry(user).State = System.Data.Entity.EntityState.Deleted;
                    }
                    //var user = new ApplicationUser
                    //    {
                    //        UserName = deleted.UserName,
                    //        Email = deleted.Email
                    //    };
                    //var result = UserManager.Delete(user);
                }
                try
                {
                    AppDbContext.SaveChanges();
                    foreach (var deleted in users.deleted)
                    {
                        ORedisHelp.HashDelete(RedisHashUserKey, deleted.Id);
                    }
                    OApplicationUserViewModel.deleted = users.deleted.Select(x => new ApplicationUser()
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        UserNameDesc = x.UserNameDesc,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        PasswordHash = x.Password,
                        LockoutEnabled = x.LockoutEnabled,
                        DepartMent = x.DepartMent
                    });
                }
                catch (Exception ex)
                {
                    var ErrMsg = Common.GetExceptionMsg(ex);
                    return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
                }

            }
            if (users.inserted != null)
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
                List<ApplicationUser> ArrUser = new List<ApplicationUser>();
                foreach (var inserted in users.inserted)
                {
                    var QQUSER = AppDbContext.Users.Where(x => x.UserName == inserted.UserName).ToList();
                    if (QQUSER.Any())
                    {
                        return Json(new { Success = false, ErrMsg = "用户名已经存在！" }, JsonRequestBehavior.AllowGet);
                    }

                    var user = new ApplicationUser
                    {
                        UserName = inserted.UserName,
                        Email = inserted.Email,
                        LockoutEnabled = inserted.LockoutEnabled,
                        UserNameDesc = inserted.UserNameDesc,
                        PhoneNumber = inserted.PhoneNumber,
                        DepartMent = inserted.DepartMent,
                        SecurityStamp = Guid.NewGuid().ToString().ToLower(),
                        PasswordHash = SignInManager.UserManager.PasswordHasher.HashPassword(string.IsNullOrEmpty(inserted.Password) ? "000000" : inserted.Password),
                        LockoutEndDateUtc = inserted.LockoutEndDateUtc,
                    };
                    AppDbContext.Entry(user).State = System.Data.Entity.EntityState.Added;
                    ArrUser.Add(user);
                    //var result = UserManager.Create(user, string.IsNullOrEmpty(inserted.Password) ? "000000" : inserted.Password);
                }
                try
                {
                    AppDbContext.SaveChanges();
                    foreach (var instUser in ArrUser)
                    {
                        ORedisHelp.HashSet<ApplicationUser>(RedisHashUserKey, instUser.Id, instUser);
                    }
                    OApplicationUserViewModel.inserted = ArrUser;
                }
                catch (Exception ex)
                {
                    var ErrMsg = Common.GetExceptionMsg(ex);
                    return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
                }
            }
            if ((OApplicationUserViewModel.updated != null && OApplicationUserViewModel.updated.Any()) ||
                (OApplicationUserViewModel.deleted != null && OApplicationUserViewModel.deleted.Any()) ||
                (OApplicationUserViewModel.inserted != null && OApplicationUserViewModel.inserted.Any()))
            {
                try
                {
                    //自动更新 缓存
                    if (IsAutoResetCache)
                        AutoResetCache(OApplicationUserViewModel);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    var ErrMsg = Common.GetExceptionMsg(ex);
                    return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { Success = false, ErrMsg = "没有任何需要更新的数据" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult Get_PageUserList(int page = 1, int rows = 10, string q = "")
        {
            int totalCount = 0;

            var users = _userManager.Users;
            //去除接单客服
            string IsJDServ = Request["IsJDServ"] ?? "";
            if (!string.IsNullOrEmpty(q))
            {
                users = users.Where(x => x.UserName.Contains(q) || x.UserNameDesc.Contains(q));
            }
            var rowsdata = users.Select(n => new
            {
                Id = n.Id,
                UserName = n.UserName,
                UserNameDesc = n.UserNameDesc,
                Email = n.Email,
                PhoneNumber = n.PhoneNumber,
                AccessFailedCount = n.AccessFailedCount,
                LockoutEnabled = n.LockoutEnabled,
                LockoutEndDateUtc = n.LockoutEndDateUtc,
                n.UserOperatPoint,
                n.DepartMent
            }).Distinct();
            totalCount = rowsdata.Count();
            var pagelist = new { total = totalCount, rows = rowsdata.OrderBy(x => x.Id).Skip(rows * (page - 1)).Take(rows).ToList() };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult Get_PageUserList_FromCache(int page = 1, int rows = 10, string q = "")
        {
            int totalCount = 0;

            var ArrUsers = (IEnumerable<ApplicationUser>)CacheHelper.Get_SetCache(Common.CacheNameS.ApplicationUser);
            var QArrUsers = ArrUsers.Where(x => x.UserName == null || x.UserNameDesc == null);
            if (QArrUsers != null && QArrUsers.Any())
            {
                foreach (var item in QArrUsers)
                {
                    item.UserName = item.UserName ?? "";
                    item.UserNameDesc = item.UserNameDesc ?? "";
                }
            }
            //去除接单客服
            string IsJDServ = Request["IsJDServ"] ?? "";
            if (!string.IsNullOrEmpty(q))
            {
                ArrUsers = ArrUsers.Where(x => x.UserName.Contains(q) || x.UserNameDesc.Contains(q));
            }
            var rowsdata = ArrUsers.Select(n => new
            {
                Id = n.Id,
                UserName = n.UserName,
                UserNameDesc = n.UserNameDesc,
                Email = n.Email,
                PhoneNumber = n.PhoneNumber,
                AccessFailedCount = n.AccessFailedCount,
                LockoutEnabled = n.LockoutEnabled,
                LockoutEndDateUtc = n.LockoutEndDateUtc,
                n.UserOperatPoint,
                ID = n.UserName,
                TEXT = n.UserNameDesc,
                n.DepartMent
            }).Distinct();
            totalCount = rowsdata.Count();
            var pagelist = new { total = totalCount, rows = rowsdata.OrderBy(x => x.Id).Skip(rows * (page - 1)).Take(rows).ToList() };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
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

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            #region
            ApplicationDbContext AppDbContext = new ApplicationDbContext();
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

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserNameDesc = model.UserNameDesc,
                    UserName = model.UserName,
                    Email = model.Email,
                    DepartMent = model.Department,
                    PhoneNumber = model.PhoneNumber,
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (IsAutoResetCache)
                        AutoResetCache(model);
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    //var Users = AppDbContext.Users.Where(x => x.UserName == model.UserName).FirstOrDefault();
                    //IdentityResult resultRole = UserManager.AddToRoles(Users.Id, "TEST");
                    //if (resultRole.Succeeded) 
                    //{
                    return RedirectToAction("Index", "AccountManage");
                    //}
                    //else
                    //{
                    //    AddErrors(result);
                    //}

                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("Error");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ApplicationUser user)
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

            if (ModelState.IsValid)
            {
                var item = await UserManager.FindByIdAsync(user.Id);
                item.UserName = user.UserName;
                item.PhoneNumber = user.PhoneNumber;
                item.Email = user.Email;
                var result = await UserManager.UpdateAsync(item);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "AccountManager");
                }
                AddErrors(result);
            }
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
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

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(id);
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    return RedirectToAction("Index", "AccountManager");
                }
                AddErrors(result);
            }
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}