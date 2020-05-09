using TMI.Web.Extensions;
using TMI.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;
using System.Web.Caching;

namespace TMI.Web
{
    public class CacheHelper : GetKeyPerpoty
    {
        private static string LogFolder = "CacheHelper";
        //特殊的缓存键值
        private static List<string> ArrSpecialCacheKey = new List<string>();

        public CacheHelper()
        {
            GetSpecialCacheKey();
        }

        /// <summary>
        /// 获取特殊缓存键值
        /// </summary>
        public static void GetSpecialCacheKey()
        {
            try
            {
                List<string> _ArrSpecialCacheKey = new List<string>();
                if (ArrSpecialCacheKey == null || !ArrSpecialCacheKey.Any())
                {
                    var SpecialCacheKeyStr = ConfigurationManager.AppSettings["SpecialCacheKeyStr"] ?? "";
                    if (!string.IsNullOrEmpty(SpecialCacheKeyStr))
                        _ArrSpecialCacheKey = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(SpecialCacheKeyStr);
                    if (!(_ArrSpecialCacheKey == null || !_ArrSpecialCacheKey.Any()))
                    {
                        ArrSpecialCacheKey = _ArrSpecialCacheKey;
                    }
                }
            }
            catch (Exception ex)
            {
                ArrSpecialCacheKey = new List<string>();
                var ErrMsg = Common.GetExceptionMsg(ex);
                Common.WriteLog_Local(ErrMsg, LogFolder, true);
            }
        }

        /// <summary>  
        /// 获取数据缓存  
        /// </summary>  
        /// <param name="cacheKey">键</param>  
        public static object GetCache(string cacheKey)
        {
            var objCache = HttpRuntime.Cache.Get(cacheKey);
            return objCache;
        }

        /// <summary>  
        /// 设置数据缓存  
        /// </summary>  
        public static void SetCacheFile(string cacheKey, object objObject, CacheDependency dependencies = null)
        {
            var objCache = HttpRuntime.Cache;
            //lock (Common.lockCacheHelper)
            //{
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                RemoveCache(cacheKey);
            }
            if (dependencies != null)
                objCache.Insert(cacheKey, objObject, dependencies);
            else
                objCache.Insert(cacheKey, objObject);
            //}
        }

        /// <summary>  
        /// 设置数据缓存  
        /// </summary>  
        public static void SetCache(string cacheKey, object objObject)
        {
            var objCache = HttpRuntime.Cache;
            //lock (Common.lockCacheHelper)
            //{
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                RemoveCache(cacheKey);
            }
            objCache.Insert(cacheKey, objObject);
            //}
        }

        /// <summary>  
        /// 设置数据缓存  
        /// </summary>  
        public static void SetCache(string cacheKey, object objObject, int timeout = 7200)
        {
            try
            {
                if (objObject == null) return;
                var objCache = HttpRuntime.Cache;

                //lock (Common.lockCacheHelper)
                //{
                if (HttpRuntime.Cache[cacheKey] != null)
                {
                    RemoveCache(cacheKey);
                }
                if (timeout > 0)
                {
                    //相对过期  
                    //objCache.Insert(cacheKey, objObject, null, DateTime.MaxValue, timeout, CacheItemPriority.NotRemovable, null);  
                    //绝对过期时间  
                    objCache.Insert(cacheKey, objObject, null, DateTime.UtcNow.AddSeconds(timeout), TimeSpan.Zero, CacheItemPriority.High, null);
                }
                else
                {
                    //绝对过期时间  
                    objCache.Insert(cacheKey, objObject);
                }
                //}
            }
            catch (Exception)
            {
                //throw;  
            }
        }

        /// <summary>  
        /// 移除指定数据缓存  
        /// </summary>  
        public static void RemoveCache(string cacheKey)
        {
            var cache = HttpRuntime.Cache;
            cache.Remove(cacheKey);
        }

        /// <summary>  
        /// 移除全部缓存  
        /// </summary>  
        public static void RemoveAllCache()
        {
            var cache = HttpRuntime.Cache;
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cache.Remove(cacheEnum.Key.ToString());
            }
        }

        /// <summary>
        /// 获取 缓存
        /// </summary>
        /// <param name="cacheKey">缓存枚举</param>
        /// <returns></returns>
        public static object Get_SetCache(TMI.Web.Extensions.Common.CacheNameS OCacheKey)
        {
            if (ArrSpecialCacheKey == null || !ArrSpecialCacheKey.Any())
                GetSpecialCacheKey();
            //特殊缓存键值，手动实现
            if (ArrSpecialCacheKey != null)
            {
                if (ArrSpecialCacheKey.Any(x => x == OCacheKey.ToString()))
                {
                    return Get_SetSpecialCache(OCacheKey.ToString());
                }
            }

            string cacheKey = OCacheKey.ToString();
            WebdbContext dbContext = new WebdbContext();
            string CacheName = "dbContextMember";
            MemberInfo[] ArrMember = new MemberInfo[] { };
            try
            {
                var Cacheobj = GetCache(CacheName);
                if (Cacheobj == null)
                {
                    ArrMember = dbContext.GetType().GetMembers();
                    lock (Common.lockCacheHelper)
                    {
                        SetCache(CacheName, ArrMember);
                    }
                }
                else
                    ArrMember = (MemberInfo[])Cacheobj;
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, LogFolder);
            }
            if (ArrMember.Any())
            {
                try
                {
                    ////加锁
                    //lock (Common.lockCacheHelper)
                    //{
                    var Cacheobj = GetCache(cacheKey);
                    if (Cacheobj == null)
                    {
                        var WhereArrMember = ArrMember.Where(x => x.Name == cacheKey);
                        if (WhereArrMember.Any())
                        {
                            Type GenericType = null;
                            PropertyInfo pi = (PropertyInfo)WhereArrMember.Where(x => x.MemberType == MemberTypes.Property).FirstOrDefault();
                            if (pi != null)
                            {
                                if (pi.PropertyType.IsGenericType)
                                {
                                    Type[] ArrGenericType = pi.PropertyType.GetGenericArguments();
                                    GenericType = ArrGenericType[0];
                                }
                                else
                                    GenericType = pi.PropertyType;
                            }

                            var WhereArrMethod = ArrMember.Where(x => x.MemberType == MemberTypes.Method && x.Name == "Set").Select(x => (MethodInfo)x);
                            MethodInfo SetMethod = WhereArrMethod.Where(x => !x.IsGenericMethod).FirstOrDefault();
                            var obj = SetMethod.Invoke(dbContext, new object[] { GenericType });

                            MethodInfo ToListMethod = typeof(System.Linq.Enumerable).GetMethod("ToList");
                            if (ToListMethod != null)
                            {
                                var NewToListMethod = ToListMethod.MakeGenericMethod(GenericType);
                                var Newobj = NewToListMethod.Invoke(null, new object[] { obj });
                                //将数据 新增到
                                SetCache(cacheKey, Newobj);
                                return Newobj;
                            }
                            else
                                return null;
                        }
                        else
                        {
                            string ErrMsg = cacheKey + ":无法找到请求的数据类";
                            SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, LogFolder);
                            return null;
                        }
                    }
                    else
                        return Cacheobj;
                    //}
                }
                catch (Exception ex)
                {
                    string ErrMsg = cacheKey + ":" + Common.GetExceptionMsg(ex);
                    SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, LogFolder);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #region 特殊缓存键值，手动实现

        /// <summary>
        /// 特殊的缓存键值
        /// </summary>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        public static object Get_SetSpecialCache(string CacheName)
        {
            object retObj = null;
            switch (CacheName)
            {
                case "AllEntityAssembly":
                    retObj = Get_SetAllEntityAssembly();
                    break;
                case "dbContextMember":
                    retObj = Get_SetdbContextMember();
                    break;
                case "NoQxValid":
                    retObj = Get_SetNoQxValid();
                    break;
                case "dictOnetoManyControllerName":
                    retObj = Get_SetOnetoManyCntrlerName();
                    break;
                case "ApplicationRole":
                    retObj = Get_SetApplicationRole();
                    break;
                case "ApplicationUser":
                    retObj = Get_SetApplicationUser();
                    break;
                case "SetDefaults":
                    retObj = Get_SetSetDefaults();
                    break;
                case "BillFormulaXML":
                    retObj = Get_SetBillFormulaXML();
                    break;
                case "LinqEnumerableMethods":
                    retObj = Get_SetLinqEnumerableMethods();
                    break;
                case "IListMethods":
                    retObj = Get_SetIListMethods();
                    break;
                case "AsyncWriteLog":
                    retObj = Get_SetBoolConfAppSettings("", CacheName);
                    break;
                case "IsWriteDataToRedis":
                    retObj = Get_SetBoolConfAppSettings("", CacheName);
                    break;
                case "IsWriteDataToLunece":
                    retObj = Get_SetBoolConfAppSettings("", CacheName);
                    break;
                case "ScrapyPath":
                    retObj = Get_SetStringConfAppSettings("", CacheName);
                    break;
                default:
                    break;
            }
            return retObj;
        }

        /// <summary>
        /// 获取WebConfig-Bool类型设置
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        public static object Get_SetBoolConfAppSettings(string logPath, string CacheName)
        {
            object retobj = null;
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;
                if (HttpRuntime.Cache[CacheName] != null)
                    retobj = HttpRuntime.Cache[CacheName];
                if (retobj == null)
                {
                    string ConfAppSettingStr = System.Configuration.ConfigurationManager.AppSettings[CacheName] ?? "";
                    bool IsWriteDataToLunece = Common.ChangStrToBool(ConfAppSettingStr);
                    retobj = IsWriteDataToLunece;
                    CacheHelper.SetCache(CacheName, IsWriteDataToLunece);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                retobj = null;
            }
            return retobj;
        }

        /// <summary>
        /// 获取WebConfig-string类型设置
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        public static object Get_SetStringConfAppSettings(string logPath, string CacheName)
        {
            string retobj = null;
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;
                if (HttpRuntime.Cache[CacheName] != null)
                    retobj = HttpRuntime.Cache[CacheName].ToString();
                if (retobj == null || retobj == "")
                {
                    string ConfAppSettingStr = System.Configuration.ConfigurationManager.AppSettings[CacheName] ?? "";
                    retobj = ConfAppSettingStr;
                    CacheHelper.SetCache(CacheName, ConfAppSettingStr);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                retobj = null;
            }
            return retobj;
        }

        /// <summary>
        /// 获取 dbContext 所有Member
        /// </summary>
        /// <param name="AppdbContext"></param>
        /// <param name="logPath"></param>
        /// <returns></returns>
        public static object Get_SetdbContextMember(WebdbContext AppdbContext = null, string logPath = "", string CacheName = "dbContextMember")
        {
            MemberInfo[] ArrMember = new MemberInfo[] { };
            try
            {
                if (AppdbContext == null)
                    AppdbContext = new WebdbContext();
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;

                if (HttpRuntime.Cache[CacheName] != null)
                    ArrMember = HttpRuntime.Cache[CacheName] as MemberInfo[];
                if (ArrMember == null || !ArrMember.Any())
                {
                    ArrMember = AppdbContext.GetType().GetMembers();
                    CacheHelper.SetCache(CacheName, ArrMember);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                ArrMember = null;
            }
            return ArrMember;
        }

        /// <summary>
        /// 获取网站所有反射类
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        public static Object Get_SetAllEntityAssembly(string logPath = "", string CacheName = "AllEntityAssembly")
        {
            List<Type> ArrPropertyInfo = new List<Type>();

            try
            {
                var MyAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;

                if (HttpRuntime.Cache[CacheName] != null)
                    ArrPropertyInfo = HttpRuntime.Cache[CacheName] as List<Type>;
                if (ArrPropertyInfo == null || !ArrPropertyInfo.Any())
                {
                    ArrPropertyInfo = MyAssembly.GetTypes().ToList();
                    CacheHelper.SetCache(CacheName, ArrPropertyInfo);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                ArrPropertyInfo = null;
            }
            return ArrPropertyInfo;
        }

        /// <summary>
        /// 无需验证权限的页面配置
        /// </summary>
        /// <returns></returns>
        public static object Get_SetNoQxValid(string logPath = "", string CacheName = "NoQxValid")
        {
            Dictionary<string, string> NoQxValid = new Dictionary<string, string>();
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;
                //lock (Common.lockCacheNoQxValid)
                //{
                if (HttpRuntime.Cache[CacheName] != null)
                    NoQxValid = HttpRuntime.Cache[CacheName] as Dictionary<string, string>;
                if (NoQxValid == null || !NoQxValid.Any())
                {
                    NoQxValid = new Dictionary<string, string>();
                    NoQxValid.Add("logoff", "account");//退出
                    NoQxValid.Add("index", "home");//主页
                    NoQxValid.Add("noqxerr", "home");//没有权限 页面
                    NoQxValid.Add("pagenotfound", "home");//找不到文件
                    NoQxValid.Add("servererror", "home");//错误页面
                    NoQxValid.Add("savelayoutoperatepoint", "operatepoints");//登陆后选择操作点 ajax
                    NoQxValid.Add("upload", "fileupload");//上传文件界面

                    SetCache(CacheName, NoQxValid);
                }
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                NoQxValid = new Dictionary<string, string>();
            }
            return NoQxValid;
        }

        /// <summary>
        /// 一个 controll对应 多个controll的权限
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        public static object Get_SetOnetoManyCntrlerName(string logPath = "", string CacheName = "dictOnetoManyControllerName")
        {
            Dictionary<string, List<string>> dictOnetoManyControllerName = new Dictionary<string, List<string>>();
            try
            {
                //lock (Common.lockCachedictOnetoManyControllerName)
                //{
                if (HttpRuntime.Cache[CacheName] != null)
                    dictOnetoManyControllerName = HttpRuntime.Cache[CacheName] as Dictionary<string, List<string>>;
                if (dictOnetoManyControllerName == null || !dictOnetoManyControllerName.Any())
                {
                    dictOnetoManyControllerName = new Dictionary<string, List<string>>();
                    dictOnetoManyControllerName.Add("OPS_M_Orders", new List<string>(){
                        "CustomsInspections",
                        "DocumentManagements"
                    });
                    dictOnetoManyControllerName.Add("Bms_Bill_Ars", new List<string>(){
                        "Bms_Bill_Ar_Dtls"
                    });
                    dictOnetoManyControllerName.Add("Bms_Bill_Aps", new List<string>(){
                        "Bms_Bill_Ap_Dtls"
                    });
                    SetCache(CacheName, dictOnetoManyControllerName);
                }
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                dictOnetoManyControllerName = new Dictionary<string, List<string>>();
            }
            return dictOnetoManyControllerName;
        }

        /// <summary>
        /// 获取 角色
        /// </summary>
        /// <param name="AppdbContext"></param>
        /// <param name="logPath"></param>
        /// <returns></returns>
        public static object Get_SetApplicationRole(ApplicationDbContext AppdbContext = null, string logPath = "", string CacheName = "ApplicationRole")
        {
            //缓存菜单
            List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> ArrApplicationRole = new List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>();
            try
            {
                if (AppdbContext == null)
                    AppdbContext = new ApplicationDbContext();
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;
                ////加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                //lock (Common.lockCacheUser)
                //{
                if (HttpRuntime.Cache[CacheName] != null)
                    ArrApplicationRole = HttpRuntime.Cache[CacheName] as List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>;
                if (ArrApplicationRole == null || !ArrApplicationRole.Any())
                {
                    ArrApplicationRole = AppdbContext.Roles.ToList();
                    CacheHelper.SetCache(CacheName, ArrApplicationRole);
                }
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                ArrApplicationRole = null;
            }
            return ArrApplicationRole;
        }

        /// <summary>
        /// 获取 用户
        /// </summary>
        /// <param name="AppdbContext"></param>
        /// <param name="logPath"></param>
        /// <returns></returns>
        public static object Get_SetApplicationUser(ApplicationDbContext AppdbContext = null, string logPath = "", string CacheName = "ApplicationUser")
        {
            //缓存菜单
            List<ApplicationUser> ArrApplicationUser = new List<ApplicationUser>();
            try
            {
                if (AppdbContext == null)
                    AppdbContext = new ApplicationDbContext();
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;
                ////加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
                //lock (Common.lockCacheUser)
                //{
                if (HttpRuntime.Cache[CacheName] != null)
                    ArrApplicationUser = HttpRuntime.Cache[CacheName] as List<ApplicationUser>;
                if (ArrApplicationUser == null || !ArrApplicationUser.Any())
                {
                    ArrApplicationUser = AppdbContext.Users.ToList();
                    CacheHelper.SetCache(CacheName, ArrApplicationUser);
                }
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                ArrApplicationUser = null;
            }
            return ArrApplicationUser;
        }

        /// <summary>
        /// 设置获取 缓存默认值
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        public static object Get_SetSetDefaults(string FilePath = "/App_Data/SetDefaults.xml", string logPath = "", string CacheName = "SetDefaults")
        {
            IEnumerable<SetDefaults> ArrSetDefaults = new List<SetDefaults>();
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;

                if (HttpRuntime.Cache[CacheName] != null)
                    ArrSetDefaults = HttpRuntime.Cache[CacheName] as IEnumerable<SetDefaults>;

                if (ArrSetDefaults == null || !ArrSetDefaults.Any())
                {
                    List<string> ArrTabName = Common.GetAllDefaultTabName();
                    foreach (var TabName in ArrTabName)
                    {
                        if (!string.IsNullOrEmpty(TabName))
                        {
                            IEnumerable<SetDefaults> Arr_SetDefaults = Common.getAllSetDefaultsByTable(TabName);
                            if (!(Arr_SetDefaults == null || !Arr_SetDefaults.Any()))
                            {
                                ArrSetDefaults = ArrSetDefaults.Concat(Arr_SetDefaults);
                            }
                        }
                    } 
                    if (HttpContext.Current != null && HttpContext.Current.Server != null)
                        FilePath = HttpContext.Current.Server.MapPath(FilePath);
                    if (System.IO.File.Exists(FilePath))
                    {
                        System.Web.Caching.CacheDependency dep = new System.Web.Caching.CacheDependency(FilePath);
                        SetCacheFile(CacheName, ArrSetDefaults, dep);
                    }
                    else
                    {
                        SetCache(CacheName, ArrSetDefaults);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                ArrSetDefaults = null;
            }
            return ArrSetDefaults;
        }

        /// <summary>
        /// 获取费用-计费条件XML
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        public static object Get_SetBillFormulaXML(string FilePath = "/App_Data/BillFormula.xml", string logPath = "", string CacheName = "BillFormulaXML")
        {
            List<dynamic> ArrSetDefaults = new List<dynamic>();
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;

                if (HttpRuntime.Cache[CacheName] != null)
                    ArrSetDefaults = HttpRuntime.Cache[CacheName] as List<dynamic>;

                if (ArrSetDefaults == null || !ArrSetDefaults.Any())
                {
                    ArrSetDefaults = Common.GetBillFormulaXML();
                    if (HttpContext.Current != null && HttpContext.Current.Server != null)
                        FilePath = HttpContext.Current.Server.MapPath(FilePath);
                    if (System.IO.File.Exists(FilePath))
                    {
                        System.Web.Caching.CacheDependency dep = new System.Web.Caching.CacheDependency(FilePath);
                        SetCacheFile(CacheName, ArrSetDefaults, dep);
                    }
                    else
                    {
                        SetCache(CacheName, ArrSetDefaults);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                ArrSetDefaults = null;
            }
            return ArrSetDefaults;
        }

        /// <summary>
        /// 获取 Linq-Enumerable 所有方法
        /// </summary>
        /// <param name="AppdbContext"></param>
        /// <param name="logPath"></param>
        /// <returns></returns>
        public static object Get_SetLinqEnumerableMethods(string logPath = "", string CacheName = "LinqEnumerableMethods")
        {
            MemberInfo[] ArrMember = new MemberInfo[] { };
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;

                if (HttpRuntime.Cache[CacheName] != null)
                    ArrMember = HttpRuntime.Cache[CacheName] as MemberInfo[];
                if (ArrMember == null || !ArrMember.Any())
                {
                    ArrMember = typeof(System.Linq.Enumerable).GetMethods();
                    CacheHelper.SetCache(CacheName, ArrMember);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                ArrMember = null;
            }
            return ArrMember;
        }

        /// <summary>
        /// 获取 IList 所有方法
        /// </summary>
        /// <param name="AppdbContext"></param>
        /// <param name="logPath"></param>
        /// <returns></returns>
        public static object Get_SetIListMethods(string logPath = "", string CacheName = "IListMethods")
        {
            MemberInfo[] ArrMember = new MemberInfo[] { };
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;

                if (HttpRuntime.Cache[CacheName] != null)
                    ArrMember = HttpRuntime.Cache[CacheName] as MemberInfo[];
                if (ArrMember == null || !ArrMember.Any())
                {
                    ArrMember = typeof(System.Collections.IList).GetMethods();
                    CacheHelper.SetCache(CacheName, ArrMember);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                ArrMember = null;
            }
            return ArrMember;
        }

        /// <summary>
        /// 获取 Expression表达树 所有方法
        /// </summary>
        /// <param name="AppdbContext"></param>
        /// <param name="logPath"></param>
        /// <returns></returns>
        public static object Get_SetExpressionMethods(string logPath = "", string CacheName = "ExpressionMethods")
        {
            MemberInfo[] ArrMember = new MemberInfo[] { };
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    logPath = LogFolder;

                if (HttpRuntime.Cache[CacheName] != null)
                    ArrMember = HttpRuntime.Cache[CacheName] as MemberInfo[];
                if (ArrMember == null || !ArrMember.Any())
                {
                    ArrMember = typeof(Expression).GetMethods();
                    CacheHelper.SetCache(CacheName, ArrMember);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = CacheName + ":" + Common.GetExceptionMsg(ex);
                //SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Global");
                Common.WriteLog_Local(ErrMsg, logPath, true);
                ArrMember = null;
            }
            return ArrMember;
        }

        #endregion

        /// <summary>
        /// 自动更新 缓存
        /// </summary>
        /// <param name="ObjChangViewModel"></param>
        /// <param name="cacheKey"></param>
        public static void AutoResetCache(object ObjChangViewModel)
        {
            try
            {
                Common.CacheNameS OCacheName;
                //Linq-Enumerable的所有方法
                MethodInfo[] ArrLinqMethod = Get_SetLinqEnumerableMethods() as MethodInfo[];//typeof(System.Linq.Enumerable).GetMethods();
                MethodInfo LinqToListMethod = ArrLinqMethod.Where(x => x.Name == "ToList").FirstOrDefault();

                MethodInfo[] ArrIListMethod = Get_SetIListMethods() as MethodInfo[]; //typeof(System.Collections.IList).GetMethods();
                MethodInfo IListAddMethod = ArrIListMethod.Where(x => x.Name == "Add").FirstOrDefault();
                MethodInfo IListRemoveMethod = ArrIListMethod.Where(x => x.Name == "Remove").FirstOrDefault();
                var ArrExpressionMethod = Get_SetExpressionMethods() as MethodInfo[]; //typeof(Expression).GetMethods();

                //缓存数据
                Object CacheObj = null;
                var ChangViewModelType = ObjChangViewModel.GetType();
                PropertyInfo[] ArrProtyInfo = ChangViewModelType.GetProperties();
                var ChangViewModelPi = ArrProtyInfo.Where(x => x.Name.ToLower() == "inserted" || x.Name.ToLower() == "updated" || x.Name.ToLower() == "deleted");
                if (!ChangViewModelPi.Any())
                {
                    return;
                }
                else
                {
                    var ChildType = ArrProtyInfo.First().PropertyType;
                    if (ChildType.IsGenericType)
                    {
                        var ArrGericType = ChildType.GetGenericArguments();
                        var GericType = ArrGericType.FirstOrDefault();
                        CacheObj = Enum.Parse(typeof(Common.CacheNameS), GericType.Name);
                    }
                }
                if (CacheObj != null)
                    OCacheName = (Common.CacheNameS)CacheObj;
                else
                    return;

                var ArrCache = (System.Collections.IEnumerable)CacheHelper.Get_SetCache(OCacheName);
                if (ArrCache == null)
                    return;

                ////缓存数据类型
                //Type CacheType = null;
                //var ArrCacheType = ArrCache.GetType();
                //if (ArrCacheType.IsGenericType)
                //{
                //    var Arguments = ArrCacheType.GetGenericArguments();
                //    //如果是decimal?等泛型
                //    //if (ArrCacheType.GetGenericTypeDefinition() == typeof(Nullable<>))
                //    if (Arguments.Any())
                //    {
                //        if (Arguments.Count() == 1)
                //        {
                //            CacheType = Arguments[0];
                //        }
                //    }
                //}

                foreach (PropertyInfo pi in ChangViewModelPi)
                {
                    var obj = pi.GetValue(ObjChangViewModel);
                    if (obj != null)
                    {
                        //判断是否派生自IEnumerable(string 是特殊的数组)
                        if (pi.PropertyType.GetInterface("IEnumerable", false) != null && (pi.PropertyType.Name.ToLower().IndexOf("string") < 0 ||
                        (pi.PropertyType.Name.ToLower().IndexOf("string") >= 0 && (pi.PropertyType.Name.ToLower().IndexOf("[]") > 0 || pi.PropertyType.Name.ToLower().IndexOf("<") > 0))))
                        {
                            var Arrobj = (System.Collections.IList)obj;
                            string KeyName = "ID";
                            if (Arrobj.Count > 0)
                            {
                                Type itemType = null;
                                PropertyInfo[] ArrPi = new PropertyInfo[] { };
                                PropertyInfo KeyPi = null;
                                foreach (var item in Arrobj)
                                {
                                    if (itemType == null)
                                    {
                                        itemType = item.GetType();
                                        //虚拟类转换为真实类
                                        if (itemType.FullName.IndexOf("System.Data.Entity.DynamicProxies") >= 0)
                                            itemType = System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(itemType); 
                                        if (!ArrPi.Any())
                                            ArrPi = itemType.GetProperties();
                                        KeyPi = GetKeyProperty(ArrPi);
                                        if (KeyPi != null)
                                            KeyName = KeyPi.Name;
                                        else
                                            KeyPi = ArrPi.Where(x => x.Name.ToUpper() == KeyName).FirstOrDefault();
                                    }


                                    if (KeyPi != null)
                                    {
                                        var objKeyValue = KeyPi.GetValue(item);
                                        if (objKeyValue == null)
                                            continue;
                                        if (pi.Name.ToLower() == "inserted" || pi.Name.ToLower() == "updated" || pi.Name.ToLower() == "deleted")
                                        {
                                            #region 反射创建Expression<Func<Lambda>>条件表达式

                                            var parameter = Expression.Parameter(itemType, "p");

                                            var _fucType = typeof(Func<,>);
                                            var fucType = _fucType.MakeGenericType(new Type[] { itemType, typeof(bool) });
                                            MethodInfo _LambdaMethod = null;
                                            var Arr_LambdaMethod = ArrExpressionMethod.Where(x => x.Name == "Lambda" && x.IsGenericMethod && x.GetParameters().Length == 2);
                                            var TypeParamExprs = typeof(IEnumerable<ParameterExpression>);

                                            foreach (var Methoditem in Arr_LambdaMethod)
                                            {
                                                var ArrParamInfo = Methoditem.GetParameters();
                                                if (ArrParamInfo.Any(x => x.ParameterType == TypeParamExprs))
                                                {
                                                    _LambdaMethod = Methoditem;
                                                    break;
                                                }
                                            }
                                            //Lambda 方法
                                            var LambdaMethod = _LambdaMethod.MakeGenericMethod(new Type[] { fucType });
                                            //Expression 条件
                                            Expression Condition = Expression.Equal(Expression.PropertyOrField(parameter, KeyName), Expression.Constant(objKeyValue));
                                            //返回 Expression
                                            var predicate = LambdaMethod.Invoke(null, new object[]{
                                                Condition,
                                                new List<ParameterExpression> { parameter }
                                            });

                                            #endregion

                                            #region 反射转换Expression<Func<Lambda>>条件表达式 为 Func<Lambda>委托

                                            //创建 Expression 委托
                                            var ArrExpressionTMethod = predicate.GetType().GetMethods();
                                            var ExpressionTMethod = ArrExpressionTMethod.Where(x => x.Name == "Compile" && x.GetParameters().Length == 0).FirstOrDefault();
                                            var lambda = ExpressionTMethod.Invoke(predicate, new object[] { });

                                            #endregion

                                            #region 调用Where 方法 传递条件Func<>委托

                                            var _LinqWhereMethod = ArrLinqMethod.Where(x => x.Name == "Where" && x.IsGenericMethod);
                                            var LinqWhereMethod = _LinqWhereMethod.FirstOrDefault().MakeGenericMethod(new Type[] { itemType });

                                            #region 直接利用Expression.Call调用方法

                                            ////1.静态方法 instance直接赋值null
                                            //Expression callExpr = Expression.Call(null, LinqWhereMethod, new Expression[] { Expression.Constant(ArrCache), Expression.Constant(lambda) });
                                            ////2.非静态方法 instance可直接赋值
                                            //Expression callExpr = Expression.Call(Expression.Constant("sample string"), typeof(String).GetMethod("ToUpper", new Type[] { }));
                                            //// 1.Print out the expression.
                                            //Console.WriteLine(callExpr.ToString());
                                            //// The following statement first creates an expression tree,
                                            //// 2.then compiles it, and then executes it.  
                                            //Console.WriteLine(Expression.Lambda<Func<String>>(callExpr).Compile()());

                                            #endregion

                                            #region 调用 Linq.Enumerable.Where和Linq.Enumerable.ToList

                                            //调用Linq.Enumerable.Where
                                            var param = LinqWhereMethod.GetParameters();
                                            var _WhereArrCache = LinqWhereMethod.Invoke(null, new object[] { ArrCache, lambda });
                                            //调用Linq.Enumerable.ToList
                                            var NewLinqToListMethod = LinqToListMethod.MakeGenericMethod(itemType);
                                            var WhereArrCache = NewLinqToListMethod.Invoke(null, new object[] { _WhereArrCache });

                                            #endregion

                                            #endregion

                                            var IListWhereArrCache = (System.Collections.IList)WhereArrCache;
                                            if (IListWhereArrCache.Count > 0)
                                            {
                                                if (pi.Name.ToLower() == "inserted")
                                                    continue;

                                                if (pi.Name.ToLower() == "updated")
                                                {
                                                    foreach (var itemIList in IListWhereArrCache)
                                                    {
                                                        lock (Common.lockCacheHelper)
                                                            Common.SetSamaProtity(itemIList, item);
                                                    }
                                                    continue;
                                                }
                                                if (pi.Name.ToLower() == "deleted")
                                                {
                                                    lock (Common.lockCacheHelper)
                                                    {
                                                        #region 转换为List方式 删除

                                                        //MethodInfo New_LinqToListMethod = null;
                                                        //if (!ArrListTMethod.Any())
                                                        //{
                                                        //    Type GericListType = ListType.MakeGenericType(itemType);
                                                        //    ArrListTMethod = GericListType.GetMethods();
                                                        //    ListTRemoveMethod = ArrListTMethod.Where(x => x.Name == "Remove" && x.GetParameters().Count() == 1).FirstOrDefault();
                                                        //    New_LinqToListMethod = LinqToListMethod.MakeGenericMethod(itemType);
                                                        //}
                                                        //var ListData = New_LinqToListMethod.Invoke(null, new object[] { ArrCache });
                                                        //ListTRemoveMethod.Invoke(ListData, new object[] { IListWhereArrCache[0] });

                                                        #endregion
                                                        //注意必须用 Linq 取出来的数据 否则无法删除
                                                        IListRemoveMethod.Invoke(ArrCache, new object[] { IListWhereArrCache[0] });
                                                    }
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                if (pi.Name.ToLower() == "inserted" || pi.Name.ToLower() == "updated")
                                                {
                                                    lock (Common.lockCacheHelper)
                                                        IListAddMethod.Invoke(ArrCache, new object[] { item });
                                                    continue;
                                                }
                                                if (pi.Name.ToLower() == "deleted")
                                                    continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ObjChangViewModel.ToString() + "-更新缓存错误:" + Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "Cache", true);
            }
        }

        //得到动态方法调用对应的委托，参数1：要调用的方法信息，参数2：可变参数
        public static Action<object, object[]> GetMethodDelegate(MethodInfo mi, object[] args)
        {
            var param_obj = Expression.Parameter(typeof(object), "obj");
            var param_args = Expression.Parameter(typeof(object[]), "args");
            //对可变参数进行转义
            var body_args = new Expression[args == null ? 0 : args.Length];
            Expression body = null;
            var i = 0;
            foreach (var bodyarg in body_args)
            {
                //从param_args这个object[]中取值
                var index = Expression.Constant(i, typeof(int));
                var param_arg = Expression.ArrayIndex(param_args, index);
                //类型转换
                body_args[i] = Expression.Convert(param_arg, args[i].GetType());
                i++;
            }
            //转换成相应的对象实例类型
            var body_obj = Expression.Convert(param_obj, mi.DeclaringType);
            //方法调用
            body = Expression.Call(body_obj, mi, body_args);
            return Expression.Lambda<Action<object, object[]>>(body, param_obj, param_args).Compile();
        }

        public static object TestLinqExpression()
        {
            // Creating a parameter expression.  
            ParameterExpression value = Expression.Parameter(typeof(int), "value");

            // Creating an expression to hold a local variable.   
            ParameterExpression result = Expression.Parameter(typeof(int), "result");

            // Creating a label to jump to from a loop.  
            LabelTarget label = Expression.Label(typeof(int));

            // Creating a method body.  
            BlockExpression block = Expression.Block(
                // Adding a local variable.  
                new[] { result },
                // Assigning a constant to a local variable: result = 1  
                Expression.Assign(result, Expression.Constant(1)),
                // Adding a loop.  
                    Expression.Loop(
                // Adding a conditional block into the loop.  
                       Expression.IfThenElse(
                // Condition: value > 1  
                           Expression.GreaterThan(value, Expression.Constant(1)),
                // If true: result *= value --  
                //Expression.MultiplyAssign(result, Expression.PostDecrementAssign(value)),//乘法
                           Expression.AddAssign(result, Expression.PostDecrementAssign(value)),//加法

                // If false, exit the loop and go to the label.  
                           Expression.Break(label, result)
                       ),
                // Label to jump to.  
                   label
                )
            );

            // Compile and execute an expression tree.  
            int factorial = Expression.Lambda<Func<int, int>>(block, value).Compile()(5);
            return factorial;
        }

        #region 一个方法3种实现形式

        /// <summary>
        /// C#代码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static int CSharpFact(int value)
        {
            int result = 1;
            while (value >= 1)
            {
                result *= value--;
            }
            return result;
        }

        /// <summary>
        /// C# Expression表达树 
        /// </summary>
        /// <returns></returns>
        static Func<int, int> ETFact()
        {
            // Creating a parameter expression.
            ParameterExpression value = Expression.Parameter(typeof(int), "value");

            // Creating an expression to hold a local variable.
            ParameterExpression result = Expression.Parameter(typeof(int), "result");

            // Creating a label to jump to from a loop.
            LabelTarget label = Expression.Label(typeof(int));

            // Creating a method body.
            BlockExpression block = Expression.Block(
                // Adding a local variable.
                new[] { result },
                // Assigning a constant to a local variable: result = 1
                Expression.Assign(result, Expression.Constant(1)),
                // Adding a loop.
                Expression.Loop(
                // Adding a conditional block into the loop.
                    Expression.IfThenElse(
                // Condition: value >= 1
                    Expression.GreaterThanOrEqual(value, Expression.Constant(1)),
                // If true: result *= value —
                    Expression.MultiplyAssign(result, Expression.PostDecrementAssign(value)),
                // If false, exit from loop and go to a label.
                    Expression.Break(label, result)
                    ),
                // Label to jump to.
                    label
                )
            );
            // Compile an expression tree and return a delegate.
            return Expression.Lambda<Func<int, int>>(block, value).Compile();
        }

        /// <summary>
        /// C# 中间语言
        /// </summary>
        /// <returns></returns>
        static Func<int, int> ILFact()
        {
            var method = new DynamicMethod(
            "factorial", typeof(int),
            new[] { typeof(int) }
            );
            var il = method.GetILGenerator();
            var result = il.DeclareLocal(typeof(int));
            var startWhile = il.DefineLabel();
            var returnResult = il.DefineLabel();

            // result = 1
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Stloc, result);

            // if (value <= 1) branch end
            il.MarkLabel(startWhile);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Ble_S, returnResult);

            // result *= (value–)
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Sub);
            il.Emit(OpCodes.Starg_S, 0);
            il.Emit(OpCodes.Mul);
            il.Emit(OpCodes.Stloc, result);

            // end while
            il.Emit(OpCodes.Br_S, startWhile);

            // return result
            il.MarkLabel(returnResult);
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);

            return (Func<int, int>)method.CreateDelegate(typeof(Func<int, int>));
        }

        #endregion

        public override string Test(string Str)
        {
            Str += "override-";
            return base.Test(Str);
        }
    }

    /// <summary>
    /// 获取 类的主键
    /// </summary>
    public class GetKeyPerpoty
    {
        public GetKeyPerpoty()
        {

        }

        public virtual string Test(string Str)
        {
            return Str + "_T";
        }

        /// <summary>
        /// 获取类的 主键字段名
        /// </summary>
        /// <param name="itemType"></param>
        public static PropertyInfo GetKeyProperty(Type itemType)
        {
            if (itemType == null)
            {
                return null;
            }
            else
            {
                var ArrProperty = itemType.GetProperties().Where(x => x.CustomAttributes.Any(n => n.GetType() == typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));
                if (ArrProperty.Any())
                {
                    return ArrProperty.FirstOrDefault();
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 获取类的 主键字段名
        /// </summary>
        /// <param name="itemType"></param>
        public static PropertyInfo GetKeyProperty(PropertyInfo[] ArrPropertyInfo = null)
        {
            if (ArrPropertyInfo == null)
            {
                return null;
            }
            else
            {
                var ArrProperty = ArrPropertyInfo.Where(x => x.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true) != null);
                if (ArrProperty.Any())
                {
                    return ArrProperty.FirstOrDefault();
                }
                else
                    return null;
            }
        }
    }
}