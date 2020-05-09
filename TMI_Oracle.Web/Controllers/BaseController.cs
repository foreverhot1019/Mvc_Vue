using TMI.Web.Extensions;
using TMI.Web.Models;
using TMI.Web.Services;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace TMI.Web.Controllers
{
    public class BaseController : Controller
    {
        //private readonly Microsoft.Practices.Unity.IUnityContainer OIUnityContainer = TMI.Web.App_Start.UnityConfig.Instance.Value;
        //private readonly IChangeOrderHistoryService _changeOrderHistoryService;
        //构造函数
        public BaseController()
        {
            ////返回调用者
            //IUserService IUser = OIUnityContainer.Resolve(<IChangeOrderHistoryService>();
        }

        //构造函数
        public BaseController(bool _AutoCheckRepeatSumbit, bool _IsAutoResetCache =false)
        {
            AutoCheckRepeatSumbit = _AutoCheckRepeatSumbit;
            IsAutoResetCache = _IsAutoResetCache;
        }

        /// <summary>
        /// 启用自动验证 重复提交
        /// 启用后，如果是Ajax请求 会自动赋值 新的表单唯一值（写在 LayOut-ajaxSetUp(datafilter)里）
        /// （注意一个多个post 回发，会引发重复提交错误，建议使用 手动验证重复提交）
        /// </summary>
        public bool AutoCheckRepeatSumbit { get; set; }

        /// <summary>
        /// 启用自动更新缓存（代码生成用）
        /// 实际方法请在合适的地方，自行调用（一般在写再SaveData里）
        /// </summary>
        public bool IsAutoResetCache { get; set; }

        /// <summary>
        /// 在调用操作方法前调用。
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            OActionContext = filterContext;
            OAuthorizationContext = new AuthorizationContext(filterContext.Controller.ControllerContext, filterContext.ActionDescriptor);
            OControllerContext = filterContext.Controller.ControllerContext;
            OHttpContext = filterContext.HttpContext;

            //获取当前 请求的 Controller名称
            controllerName = OAuthorizationContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            //获取当前 请求的 Controller中的Action名称
            actionName = OAuthorizationContext.ActionDescriptor.ActionName;
            //获取当前 请求的 Action类型 是 Get 还是 POST
            RequestType = OHttpContext.Request.RequestType;
            //判断请求是否是Ajax请求
            IsAjaxRequest = OHttpContext.Request.IsAjaxRequest();

            InitAction();

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 在调用操作方法后调用。
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (AutoCheckRepeatSumbit && IsAjaxRequest && RequestType.ToLower() == "post" )
            {
                var BaseController = filterContext.Controller;
                if (filterContext.Result is JsonResult)
                {
                    JsonResult retObj = (JsonResult)filterContext.Result;
                    try
                    {
                        var TypeJRet = retObj.Data.GetType();
                        if (TypeJRet.Name.StartsWith("<>f__AnonymousType") || TypeJRet.Name.StartsWith("dynamic") || TypeJRet.Name.StartsWith("ExpandoObject"))
                        {
                            //必须置空，以防止返回匿名类（匿名类不可修改值）
                            filterContext.Result = new EmptyResult();
                            dynamic NewretObj = new ExpandoObject();
                            var JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(retObj.Data);
                            NewretObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(JsonStr);
                            string ActionGuidName = BaseController.ViewData["ActionGuidName"] == null ? "" : BaseController.ViewData["ActionGuidName"].ToString();
                            string ActionGuid = BaseController.ViewData[ActionGuidName] == null ? "" : BaseController.ViewData[ActionGuidName].ToString();
                            NewretObj.ActionGuidName = ActionGuidName;
                            NewretObj.ActionGuid = ActionGuid;
                            var NewretJsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(NewretObj);
                            //filterContext.Result 置空后 可写入Json字符串返回
                            filterContext.HttpContext.Response.Write(NewretJsonStr);
                        }
                    }
                    catch (Exception ex)
                    {
                        filterContext.Result = retObj;
                    }
                }
            }
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// 自定义异常筛选器
        /// 在 App_Start/FilterConfig.cs中配置HandleErrorAttribute
        /// 也可以在Web.Config system.web 下设置（可以分配处理错误 控制器）
        ///<customErrors mode="On" defaultRedirect="~/Home/ServerError">
        ///  <error statusCode="404" redirect="~/Home/PageNotFound" />
        ///</customErrors>
        /// 先走 App_Start/FilterConfig.cs中配置的 ExceptionHandle-OnException=>BaseController-OnException=>Global-Application_Error
        /// View 访问的是 出错控制器下的 ViewName（默认Error），没有会去找 Shared 目录下的ViewName
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        /// <summary>
        /// 处理Action权限和重复提交
        /// </summary>
        private void InitAction()
        {
            switch (RequestType.ToLower())
            {
                case "get":
                    RequestTypeName = "访问";
                    //不是 Ajax请求无需 回发 表单唯一值
                    if (!IsAjaxRequest)
                    {
                        RepeatSubmit_Get();
                    }
                    break;
                case "post":
                    RequestTypeName = "提交";
                    //启用自动验证重复提交
                    if (AutoCheckRepeatSumbit)
                    {
                        var CheckRepeatStr = Request["CheckRepeat"] ?? "";
                        if (!string.IsNullOrWhiteSpace(CheckRepeatStr))
                            break;
                        RetResult ORetResult = RepeatSubmit_Post();
                        if (!ORetResult.Success)
                        {
                            if (!IsAjaxRequest)
                            {
                                if (OHttpContext.Request.UrlReferrer != null)
                                    OActionContext.Result = new RedirectResult(base.Request.UrlReferrer.ToString());
                                else
                                    OActionContext.Result = new RedirectResult("/Home/Index");
                            }
                            else
                            {
                                //返回值
                                var retObj = new { Success = ORetResult.Success, ErrMsg = ORetResult.RetMsg, ActionGuidName = ORetResult.ActionGuidName, ActionGuid = ORetResult.ActionGuid };
                                var OJsonResult = new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                    MaxJsonLength = Int32.MaxValue,
                                    ContentType = "application/json",
                                    Data = retObj
                                };
                                OActionContext.Result = OJsonResult;
                            }
                        }
                    }
                    break;
                default:
                    RequestTypeName = RequestType;
                    break;
            }
        }

        /// <summary>
        /// 控制器 Action 容器
        /// </summary>
        protected ActionExecutingContext OActionContext;

        /// <summary>
        /// 控制器容器
        /// </summary>
        protected ControllerContext OControllerContext;

        /// <summary>
        /// Http请求容器
        /// </summary>
        protected HttpContextBase OHttpContext;

        /// <summary>
        /// 当前控制器的 权限内容容器
        /// </summary>
        protected AuthorizationContext OAuthorizationContext;

        /// <summary>
        /// 当前 请求的 Controller名称
        /// </summary>
        protected string controllerName { get; set; }

        /// <summary>
        /// 当前 请求的 Action名称
        /// </summary>
        protected string actionName { get; set; }

        /// <summary>
        /// 当前 请求的 Action类型 是 Get 还是 POST
        /// </summary>
        protected string RequestType { get; set; }

        /// <summary>
        /// 当前 请求的 Action类型 是 Get 还是 POST
        /// </summary>
        protected string RequestTypeName { get; set; }

        /// <summary>
        /// 当前 请求是否是Ajax请求
        /// </summary>
        protected bool IsAjaxRequest { get; set; }

        #region 验证 重复提交

        /// <summary>
        /// Get验证页面重复提交
        /// Get 时 赋值 TempData 并回发到前台，Post时 使用
        /// </summary>
        /// <returns></returns>
        protected virtual RetResult RepeatSubmit_Get()
        {
            RetResult tfRet = new RetResult(true, "");

            try
            {
                #region 利用TempData生命周期特性 防止二次提交

                string ActionGuidName = "ActionGuid_" + Guid.NewGuid().ToString();
                string ActionGuid = TempData.ContainsKey(ActionGuidName) ? TempData[ActionGuidName].ToString() : "";
                if (string.IsNullOrEmpty(ActionGuid))
                    ActionGuid = Guid.NewGuid().ToString();
                TempData[ActionGuidName] = ActionGuid;
                ViewData["ActionGuidName"] = ActionGuidName;
                ViewData[ActionGuidName] = ActionGuid;

                #endregion
            }
            catch (Exception ex)
            {
                tfRet.Success = false;
                tfRet.RetMsg = Common.GetExceptionMsg(ex);
            }
            finally
            {

            }

            return tfRet;
        }

        /// <summary>
        /// Post验证页面重复提交
        /// </summary>
        /// <returns></returns>
        protected virtual RetResult RepeatSubmit_Post()
        {
            RetResult tfRet = new RetResult(true, "");
            try
            {
                #region 表单唯一值,防止 重复提交

                string ActionGuidName = Request["ActionGuidName"] ?? "";
                string ActionGuid = Request["ActionGuid"] ?? "";
                if (string.IsNullOrEmpty(ActionGuidName) || string.IsNullOrEmpty(ActionGuid) || !TempData.ContainsKey(ActionGuidName))
                {
                    tfRet.Success = false;
                    tfRet.RetMsg = "重复提交，或表单数据已更新，请刷新后再保存！";
                }
                else if (TempData[ActionGuidName].ToString() != ActionGuid)
                {
                    tfRet.Success = false;
                    tfRet.RetMsg = "重复提交，或表单数据已更新，请刷新后再保存！";
                }
                else
                {
                    TempData[ActionGuidName] = Guid.NewGuid().ToString();
                }
                UpdateRepeatSumit(tfRet);

                #endregion
            }
            catch (Exception ex)
            {
                tfRet.Success = false;
                tfRet.RetMsg = Common.GetExceptionMsg(ex);
            }
            finally
            {

            }

            return tfRet;
        }

        /// <summary>
        /// 更新 重复提交唯一值
        /// </summary>
        protected virtual void UpdateRepeatSumit(RetResult tfRet = null)
        {
            var ActionGuid = Guid.NewGuid().ToString();
            var ActionGuidName = "ActionGuid_" + Guid.NewGuid().ToString();
            TempData[ActionGuidName] = ActionGuid;
            ViewData["ActionGuidName"] = ActionGuidName;
            ViewData[ActionGuidName] = ActionGuid;
            if (tfRet != null)
            {
                tfRet.ActionGuidName = ActionGuidName;
                tfRet.ActionGuid = ActionGuid;
            }
        }

        #endregion

        /// <summary>
        /// 验证Ajax权限(Action权限验证，已经在 Models/UserAuthAttribute.cs中统一验证)
        /// </summary>
        /// <param name="ActionStr"></param>
        /// <param name="ActionType"></param>
        /// <returns></returns>
        protected virtual RetResult ValidAjaxQX(string ActionStr, string ActionType)
        {
            RetResult ORetResult = new RetResult(true, "");
            if (string.IsNullOrEmpty(ActionStr))
                ActionStr = actionName;
            var ControllActinMsg = "编辑";
            bool IsHaveQx = Common.ModelIsHaveQX("/" + controllerName, "Edit", ControllActinMsg);
            if (!IsHaveQx)
            {
                if (Request.IsAjaxRequest())
                {
                    ORetResult.Success = false;
                    ORetResult.RetMsg = "您没有权限" + RequestType + "！";
                }
                else
                {
                    ORetResult.Success = false;
                    ORetResult.RetMsg = "您没有权限" + RequestType + "！";
                }
            }
            return ORetResult;
        }

        #region 登录账户和权限

        /// <summary>
        /// 当前用户
        /// </summary>
        protected ApplicationUser CurrentAppUser
        {
            get
            {
                if (base.HttpContext != null)
                {
                    if (base.HttpContext.User != null)
                    {
                        if (base.HttpContext.User.Identity.IsAuthenticated)
                        {
                            var SessionLoginUser = base.HttpContext.Session[Common.GeSessionEnumByName("LoginUser").ToString()];
                            if (SessionLoginUser != null)
                                return SessionLoginUser as ApplicationUser;//HttpContext.Current.Session[Common.GeSessionEnumByName("LoginUser").ToString()] as ApplicationUser;
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            //set
            //{
            //    base.HttpContext.Session[Common.GeSessionEnumByName("LoginUser").ToString()] = value;
            //}
        }

        /// <summary>
        /// 当前用户权限
        /// </summary>
        protected List<ApplicationRole> CurrentUserRoles
        {
            get
            {
                if (base.HttpContext != null)
                {
                    if (base.HttpContext.User != null)
                    {
                        if (base.HttpContext.User.Identity.IsAuthenticated)
                        {
                            var SessionLoginUserRoles = base.HttpContext.Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()];
                            if (SessionLoginUserRoles != null)
                                return SessionLoginUserRoles as List<ApplicationRole>;
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            //set
            //{
            //    base.HttpContext.Session[Common.GeSessionEnumByName("LoginUserRoles").ToString()] = value;
            //}
        }

        /// <summary>
        /// 当前用户登录的操作点，只有 admin 或 权限 含有 超级管理员时 会又多个操作点
        /// </summary>
        //private List<OperatePoint> CurrentUserOperatePoint
        protected OperatePoint CurrentUserOperatePoint
        {
            get
            {
                if (base.HttpContext == null || base.HttpContext.User == null || base.HttpContext.User.Identity == null)
                {
                    return null;
                }
                if (base.HttpContext.User.Identity.IsAuthenticated)
                {
                    using (WebdbContext appContext = new WebdbContext())
                    {
                        Repository.Pattern.Ef6.UnitOfWork unitOfWork_ = new Repository.Pattern.Ef6.UnitOfWork(appContext);
                        string name = "CURRENT-OPSITE-" + base.HttpContext.User.Identity.Name;
                        HttpCookie cookie = base.HttpContext.Request.Cookies.Get(name);
                        if (cookie == null)
                        {
                            //HttpContext.Current.Response.Redirect("/Login");
                            return null;
                        }
                        else
                        {
                            int id = Convert.ToInt32(cookie.Value);
                            var query = unitOfWork_.Repository<OperatePoint>().Queryable().Where(x => x.ID == id).ToList();
                            if (query.Any())
                                return query.FirstOrDefault();
                            else
                                return null;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 当前用户设置的操作点，只有 admin 或 权限 含有 超级管理员时 所有操作点
        /// </summary>
        protected List<OperatePoint> CurrentUserOperatePoint_s
        {
            get
            {
                var ArrUserOperatePoints = new List<OperatePoint>();
                if (base.HttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint_s").ToString()] == null)
                {
                    if (CurrentAppUser != null)
                    {
                        WebdbContext dbContext = new WebdbContext();
                        var IQUserOptPont = from p in dbContext.OperatePoint
                                            //.Include("OperatePoint.OperatePointList")
                                            //where dbContext.UserOperatePointLink.Where(x => x.UserId == CurrentAppUser.Id).Select(x => x.OperateOpintId).Contains(p.ID)
                                            select p;
                        if (CurrentAppUser.UserName.ToLower() != "admin" && !CurrentUserRoles.Any(x => x.Name.Contains("超级管理员")))
                        {
                            IQUserOptPont = IQUserOptPont.Where(p => dbContext.UserOperatePointLink.Where(x => x.UserId == CurrentAppUser.Id).Select(x => x.OperateOpintId).Contains(p.ID));
                        }
                        ArrUserOperatePoints = IQUserOptPont.ToList();
                        if (ArrUserOperatePoints.Any())
                            base.HttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint_s").ToString()] = ArrUserOperatePoints;
                        else
                            ArrUserOperatePoints = null;
                    }
                    else
                        ArrUserOperatePoints = null;
                    return ArrUserOperatePoints;
                }
                else
                {
                    ArrUserOperatePoints = base.HttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint_s").ToString()] as List<OperatePoint>;
                    return ArrUserOperatePoints;
                }
            }
            //set
            //{
            //    base.HttpContext.Session[Common.GeSessionEnumByName("LoginUserOperatePoint_s").ToString()] = value;
            //}
        }

        #endregion

        #region 自动更新缓存

        /// <summary>
        /// 自动更新 缓存
        /// 注意：只可以自动更新 数据表缓存，其他缓存-请重写实现
        /// </summary>
        /// <param name="bd_cusservices"></param>
        protected virtual void AutoResetCache(Object ObjChangView)
        {
            try
            {
                //Object CacheObj = null;
                //var ChangViewModelType = ObjChangView.GetType(); 
                //PropertyInfo[] ArrProtyInfo = ChangViewModelType.GetProperties();
                //var ChangViewModelPi = ArrProtyInfo.Where(x => x.Name.ToLower() == "inserted" || x.Name.ToLower() == "updated" || x.Name.ToLower() == "deleted");
                //if (!ChangViewModelPi.Any())
                //{
                //    return;
                //}
                //else
                //{
                //    var ChildType = ArrProtyInfo.First().GetType();
                //    if (ChildType.IsGenericType)
                //    {
                //        var ArrGericType = ChildType.GetGenericArguments();
                //        var GericType = ArrGericType.FirstOrDefault();
                //        CacheObj = Enum.Parse(typeof(Common.CacheNameS), GericType.Name);
                //    }
                //}
                //if (CacheObj != null)
                //{
                //    Common.CacheNameS OCacheName = (Common.CacheNameS)CacheObj;
                //}
                CacheHelper.AutoResetCache(ObjChangView);
            }
            catch (Exception ex)
            {
                string ErrMsg = controllerName + "-更新缓存错误:" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Cache/" + controllerName, true);
            }
        }

        #endregion

    }

    /// <summary>
    /// 返回值
    /// </summary>
    public class RetResult
    {
        public RetResult(bool _Success, string _RetMsg)
        {
            Success = _Success;
            RetMsg = _RetMsg;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string RetMsg { get; set; }

        /// <summary>
        /// 表单唯一值
        /// </summary>
        public string ActionGuidName { get; set; }

        /// <summary>
        /// 表单唯一值
        /// </summary>
        public string ActionGuid { get; set; }

    }

}