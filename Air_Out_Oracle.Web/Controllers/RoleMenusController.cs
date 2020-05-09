using System;
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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace AirOut.Web.Controllers
{
    [Authorize]
    public class RoleMenusController : Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<RoleMenu>, Repository<RoleMenu>>();
        //container.RegisterType<IRoleMenuService, RoleMenuService>();

        //private TmsAppContext db = new TmsAppContext();

        private ApplicationUserManager userManager;
        /// <summary>
        /// 用户管理
        /// </summary>
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

        private ApplicationRoleManager _roleManager;
        /// <summary>
        /// 角色管理
        /// </summary>
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<RoleMenu>, Repository<RoleMenu>>();
        //container.RegisterType<IRoleMenuService, RoleMenuService>();

        private readonly IRoleMenuService _roleMenuService;
        private readonly IMenuItemService _menuItemService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public RoleMenusController(IRoleMenuService roleMenuService, IUnitOfWorkAsync unitOfWork, IMenuItemService menuItemService, ApplicationRoleManager roleManager)
        {
            _roleMenuService = roleMenuService;
            _menuItemService = menuItemService;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        // GET: RoleMenus/Index
        public ActionResult Index()
        {
            var rolemenus = _roleMenuService.Queryable().Include(r => r.MenuItem).AsQueryable();
            var menus = _menuItemService.Queryable().Include(x => x.SubMenus).Where(x => x.IsEnabled && x.Parent == null);
            var roles = _roleManager.Roles;
            var roleview = new List<RoleView>();

            var All_MyMenus = from p in _unitOfWork.Repository<RoleMenuAction>().Queryable()
                              group p by new { p.RoleId, p.MenuId } into g
                              select new
                              {
                                  g.Key.RoleId,
                                  g.Key.MenuId,
                                  Num = g.Count()
                              };
            var AllMyMenus = All_MyMenus.ToList();

            foreach (var role in roles)
            {
                //var mymenus = _roleMenuService.GetByRoleName(role.Name);
                var mymenus = AllMyMenus.Where(x => x.RoleId == role.Id);

                RoleView r = new RoleView();
                r.RoleName = role.Name;
                r.RoleId = role.Id;
                r.Count = mymenus.Sum(x => x.Num);
                roleview.Add(r);
            }

            ViewBag.Menus = menus;
            ViewBag.Roles = roleview;

            List<MenuAction> MenuActions = new List<MenuAction>();
            if (HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] != null)
            {
                MenuActions = HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] as List<MenuAction>;
            }
            ViewBag.MenuActions = MenuActions;

            return View(rolemenus);
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult RenderMenus()
        {
            var roles = UserManager.GetRoles(this.User.Identity.GetUserId()).ToArray();
            var menus = _roleMenuService.RenderMenus(roles);
            return PartialView("_navMenuBar", menus);
        }

        public ActionResult GetMenuList()
        {
            var MenuActionRep = _unitOfWork.Repository<MenuAction>();
            var menus = _menuItemService.Queryable().Include(x => x.SubMenus);//.Where(x => x.IsEnabled);
            var totalCount = menus.Count();
            var datarows = menus.OrderBy(x => x.Code).Select(x => new
            {
                Id = x.Id,
                Title = x.Title,
                Code = x.Code,
                _parentId = x.ParentId,
                Url = x.Url
            });

            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取权限菜单
        /// 获取权限对应可操作的页面的动作
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public ActionResult GetMenus(string roleName)
        {
            var rolemenus = _roleMenuService.GetByRoleName(roleName);
            //var all = _roleMenuService.RenderMenus(roleName);

            List<MenuAction> MenuActions = new List<MenuAction>();
            MenuActions = (List<MenuAction>)CacheHelper.Get_SetCache(Common.CacheNameS.MenuAction);
            //if (HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] != null)
            //{
            //    MenuActions = HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] as List<MenuAction>;
            //}

            //--select RoleId,MenuId,MAX([Create]),MAX([Edit]),MAX([Delete]),MAX([Index]) from (
            //--select a.Id,a.MenuId,a.RoleId,
            //--(case When a.ActionId=1 then 1 else 0 end) as [Create],
            //--(case When a.ActionId=2 then 1 else 0 end) as [Edit],
            //--(case When a.ActionId=3 then 1 else 0 end)as [Delete],
            //--(case When a.ActionId=4 then 1 else 0 end)as [Index] from RoleMenuActions a
            //--) b
            //--group by RoleId,MenuId

            StringBuilder SQLStrBMax = new StringBuilder();
            StringBuilder SQLStrBCas = new StringBuilder();
            StringBuilder SQLStrBC = new StringBuilder();
            List<string> fieldList = new List<string>() { "Id", "MenuId", "RoleId", "RoleName", "IsEnable" }; //From config or db
            if (MenuActions.Any())
            {
                foreach (var item in MenuActions)
                {
                    fieldList.Add(item.Code);
                    SQLStrBMax.Append(",MAX(\"" + item.Code + "\") as \"" + item.Code + "\" ");
                    SQLStrBCas.Append(",(case When a.ActionId=" + item.Id + " then 1 else 0 end)as \"" + item.Code + "\"");
                    SQLStrBC.Append(",c.\"" + item.Code + "\"");
                }
            }

            List<object> ArrObj = new List<object>();

            string SQLStr = "select m.Id,m.RoleId,m.MenuId,m.RoleName " + SQLStrBC.ToString() + " from ( select RoleId,MenuId" + SQLStrBMax.ToString() + " from ( select a.Id,a.MenuId,a.RoleId" + SQLStrBCas.ToString() + " from RoleMenuActions a ) b group by RoleId,MenuId ) c left join RoleMenus m on m.RoleId = c.RoleId and m.MenuId = c.MenuId where m.RoleName = :V_ROLENAME";
            OracleParameter[] param = new OracleParameter[]{
                 new OracleParameter ("V_ROLENAME", roleName )
            };
            DataSet ds = SQLDALHelper.OracleHelper.GetDataSet(CommandType.Text, SQLStr.ToUpper(), param);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        dynamic dobj = new System.Dynamic.ExpandoObject();
                        var dic = (IDictionary<string, object>)dobj;

                        foreach (var fieldItem in fieldList)
                        {
                            dic[fieldItem] = "";
                            if (ds.Tables[0].Columns.Contains(fieldItem.ToUpper()))
                            {
                                if (dr[fieldItem] != null)
                                    dic[fieldItem] = dr[fieldItem].ToString();
                            }
                        }
                        ArrObj.Add(dic);
                    }
                }
            }

            return Json(ArrObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提交权限菜单
        /// </summary>
        /// <param name="JsonStrSeltMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Submit(string JsonStrSeltMenu)
        {
            List<MenuAction> MenuActions = new List<MenuAction>();
            if (HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] != null)
            {
                MenuActions = HttpContext.Cache[Common.GeCacheEnumByName("MenuAction").ToString()] as List<MenuAction>;
            }

            List<RoleMenuAction> ArrRleMenAct = new List<RoleMenuAction>();

            #region 数据转换

            //Json数据格式 转换成 匿名类
            dynamic dyobj = Newtonsoft.Json.JsonConvert.DeserializeObject(JsonStrSeltMenu);
            foreach (var item in dyobj)
            {
                //获取 dynamic 数据集 
                Dictionary<string, object> dicDynamics = Common.GetDictKeyValueForDynamic(item);
                foreach (var itemMenuAction in MenuActions)
                {
                    RoleMenuAction ORoleMenuAction = new RoleMenuAction();
                    ORoleMenuAction.ActionId = itemMenuAction.Id;
                    var RoleMenuActType = ORoleMenuAction.GetType();
                    //获取菜单代码
                    string MenuActionCode = itemMenuAction.Code;

                    System.Reflection.PropertyInfo[] PropertyInfos = RoleMenuActType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    //遍历该model实体的所有字段
                    foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                    {
                        //获取字段名，用于查找该字段对应的display数据，来源List<ColumValue>
                        string _FiledName = fi.Name;
                        object val = null;

                        if (dicDynamics.Any())
                        {
                            var wheredicDynamic = dicDynamics.Where(x => x.Key == _FiledName);
                            if (wheredicDynamic.Any())
                            {
                                val = wheredicDynamic.FirstOrDefault().Value;
                            }
                        }

                        //object s = fi.GetValue(ORoleMenuAction, null);
                        if (val != null)
                        {
                            int newObj = 0;
                            if (int.TryParse(val.ToString(), out newObj))
                            {
                                newObj = Convert.ToInt32(val);
                                fi.SetValue(ORoleMenuAction, newObj, null);
                            }
                            else
                                fi.SetValue(ORoleMenuAction, val, null);
                        }
                    }
                    if (dicDynamics.Any(x => x.Key == MenuActionCode && Convert.ToInt32(x.Value) > 0))
                        ArrRleMenAct.Add(ORoleMenuAction);
                }
            }

            #endregion

            #region  添加 RoleMenuAction

            if (ArrRleMenAct.Any())
            {
                var RoleMenuActionRep = _unitOfWork.Repository<RoleMenuAction>();
                var RoleMenuRep = _unitOfWork.Repository<RoleMenu>();
                var Roles = RoleManager.Roles.ToList();

                var RoleIds = ArrRleMenAct.GroupBy(x => x.RoleId).Select(x => x.Key);
                var Role_MenuActions = RoleMenuActionRep.Queryable().AsNoTracking().Where(x => RoleIds.Contains(x.RoleId)).ToList();

                var Role_Menus = RoleMenuRep.Query(x => RoleIds.Contains(x.RoleId) && x.IsEnabled == true).Select().ToList();

                var GroupRoleMenuByArrRleMenAct = ArrRleMenAct.GroupBy(x => new { x.RoleId, x.MenuId }).Select(x => new RoleMenu
                {
                    RoleName = Roles.Where(n => n.Id == x.Key.RoleId).FirstOrDefault().Name,
                    IsEnabled = true,
                    RoleId = x.Key.RoleId,
                    MenuId = x.Key.MenuId == null ? 0 : Convert.ToInt32(x.Key.MenuId)
                });

                IEnumerable<RoleMenuAction> WhereSeltRoleMenuAction = null;
                IEnumerable<RoleMenu> WhereSeltRoleMenu = null;

                #region 菜单动作

                //零时变量 自动更新缓存 使用
                RoleMenuChangeViewModel OViewRoleMenu = new RoleMenuChangeViewModel();
                List<RoleMenu> ArrdelRoleMenu = new List<RoleMenu>();
                List<RoleMenu> ArraddRoleMenu = new List<RoleMenu>();
                //清除多余的菜单动作
                foreach (var item in Role_Menus)
                {
                    WhereSeltRoleMenu = GroupRoleMenuByArrRleMenAct.Where(x => x.RoleId == item.RoleId && x.MenuId == item.MenuId);
                    if (!WhereSeltRoleMenu.Any())
                    {
                        RoleMenuRep.Delete(item.Id);
                        ArrdelRoleMenu.Add(item);
                    }
                }
                var IdNum = 0;
                //添加没有包含的菜单动作
                foreach (var item in GroupRoleMenuByArrRleMenAct)
                {
                    WhereSeltRoleMenu = Role_Menus.Where(x => x.RoleId == item.RoleId && x.MenuId == item.MenuId);
                    if (!WhereSeltRoleMenu.Any())
                    {
                        item.Id = IdNum++;
                        RoleMenuRep.Insert(item);
                        ArrdelRoleMenu.Add(item);
                    }
                }
                OViewRoleMenu.deleted = ArrdelRoleMenu;
                OViewRoleMenu.inserted = ArraddRoleMenu;

                #endregion

                #region 角色菜单动作

                //零时变量 自动更新缓存 使用
                RoleMenuActionChangeViewModel OViewRoleMenuAction = new RoleMenuActionChangeViewModel();
                List<RoleMenuAction> ArrdelRoleMenuAction = new List<RoleMenuAction>();
                List<RoleMenuAction> ArraddRoleMenuAction = new List<RoleMenuAction>();
                //清除多余的角色菜单动作
                foreach (var item in Role_MenuActions)
                {
                    WhereSeltRoleMenuAction = ArrRleMenAct.Where(x => x.RoleId == item.RoleId && x.MenuId == item.MenuId && x.ActionId == item.ActionId);
                    if (!WhereSeltRoleMenuAction.Any())
                    {
                        RoleMenuActionRep.Delete(item.Id);
                        ArrdelRoleMenuAction.Add(item);
                    }
                }
                IdNum = 0;
                //添加没有包含的角色菜单动作
                foreach (var item in ArrRleMenAct)
                {
                    WhereSeltRoleMenuAction = Role_MenuActions.Where(x => x.RoleId == item.RoleId && x.MenuId == item.MenuId && x.ActionId == item.ActionId);
                    if (!WhereSeltRoleMenuAction.Any())
                    {
                        item.Id = IdNum++;
                        RoleMenuActionRep.Insert(item);
                        ArraddRoleMenuAction.Add(item);
                    }
                }
                OViewRoleMenuAction.deleted = ArrdelRoleMenuAction;
                OViewRoleMenuAction.inserted = ArraddRoleMenuAction;

                #endregion

                try
                {
                    _unitOfWork.SaveChanges();
                    //自动设置缓存
                    CacheHelper.AutoResetCache(OViewRoleMenu);
                    CacheHelper.AutoResetCache(OViewRoleMenuAction);
                    if (Request.IsAjaxRequest())
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                    var List = e.EntityValidationErrors.Select(x => new
                    {
                        ErrMsg = x.Entry.Entity.GetType().ToString() + "-" + string.Join(";", x.ValidationErrors)
                    });
                    if (List.Any())
                    {
                        if (Request.IsAjaxRequest())
                            return Json(new { Success = false, ErrMsg = string.Join("；", List) }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    if (Request.IsAjaxRequest())
                        return Json(new { Success = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                if (Request.IsAjaxRequest())
                    return Json(new { Success = false, ErrMsg = "没有 需要更新 数据库的操作" }, JsonRequestBehavior.AllowGet);

            #endregion

            return View();
        }

        // Get :RoleMenus/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult PageList(int offset = 0, int limit = 10, string search = "", string sort = "", string order = "")
        {
            int totalCount = 0;
            int pagenum = offset / limit + 1;
            var rolemenus = _roleMenuService.Query(new RoleMenuQuery().WithAnySearch(search)).Include(r => r.MenuItem).OrderBy(n => n.OrderBy(sort, order)).SelectPage(pagenum, limit, out totalCount);

            var rows = rolemenus.Select(n => new
            {
                MenuItemTitle = (n.MenuItem == null ? "" : n.MenuItem.Title),
                Id = n.Id,
                RoleName = n.RoleName,
                MenuId = n.MenuId,
                IsEnabled = n.IsEnabled
            }).ToList();
            var pagelist = new { total = totalCount, rows = rows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        // GET: RoleMenus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMenu roleMenu = _roleMenuService.Find(id);
            if (roleMenu == null)
            {
                return HttpNotFound();
            }
            return View(roleMenu);
        }

        // GET: RoleMenus/Create
        public ActionResult Create()
        {
            RoleMenu roleMenu = new RoleMenu();
            //set default value
            var menuitemRepository = _unitOfWork.Repository<MenuItem>();
            ViewBag.MenuId = new SelectList(menuitemRepository.Queryable(), "Id", "Title");
            return View(roleMenu);
        }

        // POST: RoleMenus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MenuItem,Id,RoleName,MenuId,IsEnabled")] RoleMenu roleMenu)
        {
            Common.ModelIsHaveQX();
            if (ModelState.IsValid)
            {
                _roleMenuService.Insert(roleMenu);
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a RoleMenu record");
                return RedirectToAction("Index");
            }

            var menuitemRepository = _unitOfWork.Repository<MenuItem>();
            ViewBag.MenuId = new SelectList(menuitemRepository.Queryable(), "Id", "Title", roleMenu.MenuId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(roleMenu);
        }

        // GET: RoleMenus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMenu roleMenu = _roleMenuService.Find(id);
            if (roleMenu == null)
            {
                return HttpNotFound();
            }
            var menuitemRepository = _unitOfWork.Repository<MenuItem>();
            ViewBag.MenuId = new SelectList(menuitemRepository.Queryable(), "Id", "Title", roleMenu.MenuId);
            return View(roleMenu);
        }

        // POST: RoleMenus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MenuItem,Id,RoleName,MenuId,IsEnabled")] RoleMenu roleMenu)
        {
            if (ModelState.IsValid)
            {
                roleMenu.ObjectState = ObjectState.Modified;
                _roleMenuService.Update(roleMenu);

                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a RoleMenu record");
                return RedirectToAction("Index");
            }
            var menuitemRepository = _unitOfWork.Repository<MenuItem>();
            ViewBag.MenuId = new SelectList(menuitemRepository.Queryable(), "Id", "Title", roleMenu.MenuId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(roleMenu);
        }

        // GET: RoleMenus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMenu roleMenu = _roleMenuService.Find(id);
            if (roleMenu == null)
            {
                return HttpNotFound();
            }
            return View(roleMenu);
        }

        // POST: RoleMenus/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoleMenu roleMenu = _roleMenuService.Find(id);
            _roleMenuService.Delete(roleMenu);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a RoleMenu record");
            return RedirectToAction("Index");
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
