using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TMI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TMI.Web.Controllers
{
    public class ManagementController : Controller
    {
        private ApplicationUserManager userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }

        private ApplicationRoleManager roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                roleManager = value;
            }
        }

        public ManagementController() { 
        }

        public ManagementController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing && userManager != null)
            //{
            //    userManager.Dispose();
            //    userManager = null;
            //}
            //if (disposing && roleManager != null)
            //{
            //    roleManager.Dispose();
            //    roleManager = null;
            //}
            base.Dispose(disposing);
        }

        private ActionResult _Index()
        {
            ViewBag.RoleSelectList = RoleManager.Roles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToArray();
            try
            {
                ViewBag.MaxRolesCount = UserManager.Users.Max(u => u.Roles.Count);
            }
            catch (InvalidOperationException)
            {
                ViewBag.MaxRolesCount = 0;
            }
            return View("Index", new ManagementViewModel
            {
                Users = UserManager.Users.ToList(),
                Roles = RoleManager.Roles.ToList()
            });
        }

        public ActionResult Index()
        {
            if (RoleManager.Roles.Count() == 0)
            {
                RoleManager.Create(new ApplicationRole() { Name = "admin" });

            }
            return _Index();
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("management/roles")]
        public async Task<ActionResult> AddRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new ApplicationRole { Name = model.Name });
                if (result.Succeeded)
                {
                    //RedirectToAction("Index");
                    return _Index();
                }
                else
                {
                    AddErrors(result);
                }
            }
            return _Index();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("management/roles/{id}")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                await RoleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 添加账户角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Button("Attach")]
        [Route("management/account/roles")]
        public async Task<ActionResult> AttachRole(AttachRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await UserManager.AddToRoleAsync(model.UserId, model.RoleName);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(result);
                }
            }
            return _Index();
        }

        /// <summary>
        /// 删除账户角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Button("Purge")]
        [Route("management/account/roles")]
        public async Task<ActionResult> PurgeRole(AttachRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await UserManager.RemoveFromRoleAsync(model.UserId, model.RoleName);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(result);
                }
            }
            return _Index();
        }

        /// <summary>
        /// 添加账户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("management/account")]
        public async Task<ActionResult> AddAccount(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(result);
                }
            }
            return _Index();
        }

        /// <summary>
        /// 删除账户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("management/account/{id}")]
        public async Task<ActionResult> DeleteAccount(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                await UserManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}