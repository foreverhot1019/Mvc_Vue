using CCBWebApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;

namespace CCBWebApi.Extensions
{
    public class Common
    {
        #region 加锁

        //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
        public static object lockCacheHelper = new object();

        //本地日志 线程获取信息锁
        public static object TimerLocalLogLocker = new object();
        //数据库日志 线程获取信息锁
        public static object TimerMessageLgLocker = new object();

        #endregion

        /// <summary>
        /// 获取当前命名空间
        /// </summary>
        private static string GetCurrentNamespace()
        {
            string NameSpaceStr = "";
            //取得当前方法命名空间
            NameSpaceStr = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            return NameSpaceStr;
        }

        /// <summary>
        /// 获取当前类命名空间
        /// </summary>
        private static string GetCurrentNamespace_ClassName()
        {
            string NameSpace_ClassName = "";
            //取得当前方法类全名
            NameSpace_ClassName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            return NameSpace_ClassName;
        }

        //当前程序集
        public static System.Reflection.Assembly Assembly
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly();
            }
        }

        /// <summary>
        /// 存储Cache所有名称
        /// </summary>
        public enum CacheNameS
        {
            //Start 特殊缓存---------------------------
            [Display(Name = "dbContextMember", Description = "dbContext类的所有表")]
            dbContextMember,
            [Display(Name = "AllEntityAssembly", Description = "网站所有反射类")]
            AllEntityAssembly,
            [Display(Name = "SetDefaults", Description = "所有设置的默认值")]
            SetDefaults,
            [Display(Name = "ApplicationRole", Description = "所有角色")]
            ApplicationRole,
            [Display(Name = "ApplicationUser", Description = "所有账户")]
            ApplicationUser,
            [Display(Name = "LinqEnumerableMethods", Description = "Linq-Enumerable类反射的所有方法")]
            LinqEnumerableMethods,
            [Display(Name = "IListMethods", Description = "IList类反射的所有方法")]
            IListMethods,
            [Display(Name = "ExpressionMethods", Description = "Expression表达树的所有方法")]
            ExpressionMethods,
            [Display(Name = "AsyncWriteLog", Description = "异步写日志")]
            AsyncWriteLog,
            //------------------------------end 特殊缓存

        }

        #region 枚举操作

        /// <summary>
        /// 获取Session枚举值
        /// </summary>
        /// <param name="EnumValName">枚举键</param>
        /// <param name="enumName">枚举名称</param>
        /// <returns></returns>
        public static object GeSessionEnumByName(string FieldStr)
        {
            object EnumVal = null;
            EnumVal = GetEnumByName(FieldStr, "SessionNameS");
            return EnumVal;
        }

        /// <summary>
        /// 获取缓存枚举值
        /// </summary>
        /// <param name="EnumValName">枚举键</param>
        /// <param name="enumName">枚举名称</param>
        /// <returns></returns>
        public static object GeCacheEnumByName(string FieldStr)
        {
            object EnumVal = null;
            EnumVal = GetEnumByName(FieldStr, "CacheNameS");
            return EnumVal;
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="EnumValName">枚举键</param>
        /// <param name="enumName">枚举名称</param>
        /// <returns></returns>
        public static object GetEnumByName(string FieldStr, string enumName)
        {
            object EnumVal = null;
            try
            {
                Assembly assem = Assembly.GetExecutingAssembly();
                Type type = assem.GetType(GetCurrentNamespace_ClassName() + "+" + enumName);

                try
                {
                    var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                    foreach (var fi in fields)
                    {
                        if (fi.Name == FieldStr)
                        {
                            DisplayAttribute attr;
                            attr = (DisplayAttribute)fi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

                            if (attr == null)
                            {
                                EnumVal = (object)FieldStr;
                            }
                            else
                                EnumVal = (object)((attr != null) ? attr.GetName() : String.Empty);
                            break;
                        }
                    }
                }
                catch
                {
                    EnumVal = null;
                }
            }
            catch
            {
                EnumVal = null;
            }
            return EnumVal;
        }

        /// <summary>
        /// Enum 转换成 字典类型
        /// </summary>
        /// <param name="enumName">枚举名称</param>
        /// <returns></returns>
        public static List<EnumModelType> GetEnumToDic(string enumName, string namespaseStr = "", string JoinCalc = "+")
        {
            List<EnumModelType> ArrEnumMember = new List<EnumModelType>();
            try
            {
                Assembly assem = Assembly.GetExecutingAssembly();
                namespaseStr = string.IsNullOrEmpty(namespaseStr) ? GetCurrentNamespace_ClassName() : namespaseStr;
                string CacheEnumName = namespaseStr + JoinCalc + enumName;
                var Cache_Obj = CacheHelper.GetCache(CacheEnumName);
                if (Cache_Obj != null)
                {
                    return (List<EnumModelType>)Cache_Obj;
                }
                Type type = assem.GetType(CacheEnumName);
                foreach (string FieldStr in Enum.GetNames(type))
                {
                    EnumModelType o = new EnumModelType();
                    string NameStr = "";
                    string DescptStr = "";
                    var field = type.GetField(FieldStr);
                    if (field != null)
                    {
                        if (field.Name == FieldStr)
                        {
                            DisplayAttribute attr;
                            attr = (DisplayAttribute)field.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

                            if (attr != null)
                            {
                                NameStr = (attr != null) ? attr.GetName() : String.Empty;
                                DescptStr = (attr != null) ? attr.GetDescription() : String.Empty;
                            }
                        }
                    }
                    o.Key = FieldStr;
                    o.Value = (int)Enum.Parse(type, FieldStr);
                    o.DisplayName = NameStr;
                    o.DisplayDescription = DescptStr;

                    if (!string.IsNullOrEmpty(FieldStr))
                    {
                        ArrEnumMember.Add(o);
                    }
                }
                //设置缓存
                CacheHelper.SetCache(CacheEnumName, ArrEnumMember);
            }
            catch
            {
                ArrEnumMember = new List<EnumModelType>();
            }
            return ArrEnumMember;
        }

        /// <summary>
        /// 获取枚举
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="enumVal">枚举名称/值</param>
        /// <returns></returns>
        public static T GetEnumVal<T>(string enumVal) where T : struct, IConvertible
        {
            int enumIntVal;
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            if (int.TryParse(enumVal, out enumIntVal))
            {
                T foo = (T)Enum.ToObject(typeof(T), enumIntVal);
                return foo;
            }
            else
            {
                T foo = (T)Enum.Parse(typeof(T), enumVal);
                // the foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
                if (!Enum.IsDefined(typeof(T), foo) && !foo.ToString().Contains(","))
                    throw new InvalidOperationException(enumVal + " is not an underlying value of the YourEnum enumeration.");
                return foo;
            }
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="EnumObj"></param>
        /// <returns></returns>
        public static string GetEnumDisplay(object EnumObj)
        {
            var type = EnumObj.GetType();
            if (type.IsEnum)
            {
                var EName = Enum.GetName(type, EnumObj);
                foreach (var FieldStr in Enum.GetNames(type))
                {
                    if (FieldStr == EName)
                    {
                        string NameStr = "";
                        string DescptStr = "";
                        var field = type.GetField(FieldStr);
                        if (field != null)
                        {
                            if (field.Name == FieldStr)
                            {
                                DisplayAttribute attr;
                                attr = (DisplayAttribute)field.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

                                if (attr != null)
                                {
                                    NameStr = (attr != null) ? attr.GetName() : String.Empty;
                                    DescptStr = (attr != null) ? attr.GetDescription() : String.Empty;
                                }
                            }
                        }
                        return DescptStr;
                    }
                }
                return "";
            }
            else
                return "";
        }

        #endregion

        /// <summary>
        /// 获取 错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetExceptionMsg(Exception ex)
        {
            if (ex is System.Data.SqlClient.SqlException)
            {
                var e = ex as System.Data.SqlClient.SqlException;
                return ex.InnerException == null ? ex.Message : (ex.InnerException.InnerException == null ? ex.Message : ex.InnerException.InnerException.Message);
            }
            else if (ex is Oracle.ManagedDataAccess.Client.OracleException)
            {
                var e = ex as Oracle.ManagedDataAccess.Client.OracleException;
                return ex.InnerException == null ? ex.Message : (ex.InnerException.InnerException == null ? ex.Message : ex.InnerException.InnerException.Message);
            }
            else if (ex is System.Data.Entity.Infrastructure.DbUpdateException)
            {
                var e = ex as System.Data.Entity.Infrastructure.DbUpdateException;
                return ex.InnerException == null ? ex.Message : (ex.InnerException.InnerException == null ? ex.Message : ex.InnerException.InnerException.Message);
            }
            else if (ex is System.Data.Entity.Validation.DbEntityValidationException)
            {
                var e = ex as System.Data.Entity.Validation.DbEntityValidationException;
                var ValidaErr = e.EntityValidationErrors;
                if (ValidaErr.Any())
                {
                    List<string> ArrEntityErr = new List<string>();
                    foreach (var item in ValidaErr)
                    {
                        if (item.ValidationErrors.Any())
                        {
                            foreach (var itemErrMsg in item.ValidationErrors)
                            {
                                string EntityFullName = item.Entry.Entity.GetType().ToString();
                                string EntityName = EntityFullName.Substring(EntityFullName.LastIndexOf('.') <= 0 ? 0 : EntityFullName.LastIndexOf('.') + 1);
                                ArrEntityErr.Add(EntityName + ":" + itemErrMsg.ErrorMessage);
                            }
                        }
                    }
                    return string.Join("<br/>", ArrEntityErr.Take(10)) + (ArrEntityErr.Count() > 10 ? "....." : "");
                }
                else
                    return ex.Message;
            }
            else
            {
                string ErrMsg = ex.Message;
                Exception e_x = ex.InnerException;
                while (e_x != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(e_x.Message))
                        {
                            ErrMsg = e_x.Message;
                        }
                        e_x = e_x.InnerException;
                    }
                    catch (Exception e)
                    {
                        if (string.IsNullOrEmpty(ErrMsg))
                            ErrMsg = string.IsNullOrEmpty(e.Message) ? "未知错误" : e.Message;
                        break;
                    }
                }
                return ErrMsg;
            }
        }

        /// <summary>
        /// String转换成Bool
        /// 如果是Int >0 为True
        /// 如果是String =true 为True
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static bool ChangStrToBool(string Str)
        {
            bool IsTrue = false;
            int Num = 0;
            if (int.TryParse(Str, out Num))
            {
                if (Num >= 1)
                    IsTrue = true;
            }
            else
            {
                if (Str.ToLower() == "true")
                    IsTrue = true;
            }
            return IsTrue;
        }

        #region 自动设置 两个类的 字段相同的数据

        /// <summary>
        /// 自动设置属性相同的值
        /// </summary>
        /// <typeparam name="TClass">要获取值的类</typeparam>
        /// <typeparam name="T">要添加的类</typeparam>
        /// <param name="objModel">要获取值的类数据</param>
        /// <param name="OldObjModel">要获取值的类未修改时的数据</param>
        /// <param name="_unitOfWork"></param>
        /// <param name="AutoInsert">是否自动添加</param>
        /// <returns></returns>
        public static T AutosetSameProtity<TClass, T>(object objModel, object OldObjModel, bool AutoInsert, bool IngoreFieldCase = false) where T : class, new()
        {
            //实例化 需要新增的类
            T TobjModel = new T();
            //try
            //{
            if (objModel != null)
            {
                #region 赋值相同项

                System.Reflection.PropertyInfo[] TClass_PropertyInfos = objModel == null ? new System.Reflection.PropertyInfo[] { } : objModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                System.Reflection.PropertyInfo[] OldTClass_PropertyInfos = OldObjModel == null ? new System.Reflection.PropertyInfo[] { } : OldObjModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                System.Reflection.PropertyInfo[] Change_PropertyInfos = TobjModel == null ? new System.Reflection.PropertyInfo[] { } : TobjModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                var _ChangeProtitys = Change_PropertyInfos.Where(x => x.Name.EndsWith("_Change"));

                //遍历该model实体的所有字段
                foreach (System.Reflection.PropertyInfo fi in TClass_PropertyInfos)
                {
                    string DataType = fi.PropertyType.Name;

                    //泛型
                    if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var Arguments = fi.PropertyType.GetGenericArguments();
                        if (Arguments.Count() == 1)
                        {
                            Type ChildType = Arguments[0];
                            DataType = Arguments[0].Name;
                            if (ChildType != null)
                            {
                                if (ChildType == typeof(DateTime) || ChildType == typeof(int) || ChildType == typeof(decimal) ||
                                    ChildType == typeof(double) || ChildType == typeof(float) || ChildType == typeof(bool))
                                {
                                    var Changefi_s = Change_PropertyInfos.Where(x => IngoreFieldCase ? (x.Name.ToUpper() == fi.Name.ToUpper()) : x.Name == fi.Name);// && x.PropertyType == fi.PropertyType
                                    if (Changefi_s.Any())
                                    {
                                        var Changefi = Changefi_s.First();
                                        object objval = fi.GetValue(objModel);
                                        //Changefi.SetValue(TobjModel, objval);
                                        setProtityValue(TobjModel, Changefi, objval);
                                    }
                                }
                            }
                        }
                    }
                    ////判断是否派生自IEnumerable
                    //else if (fi.PropertyType.GetInterface("IEnumerable", false) != null && DataType.ToLower().IndexOf("string") < 0)
                    //{
                    //}
                    else
                    {
                        var Changefi_s = Change_PropertyInfos.Where(x => IngoreFieldCase ? (x.Name.ToUpper() == fi.Name.ToUpper()) : x.Name == fi.Name);// && x.PropertyType == fi.PropertyType
                        if (Changefi_s.Any())
                        {
                            //带_Change的为变更后数据
                            var Where_ChangeProtitys = _ChangeProtitys.Where(x => IngoreFieldCase ? (x.Name.ToUpper() == (fi.Name + "_Change").ToUpper()) : x.Name == (fi.Name + "_Change"));
                            if (Where_ChangeProtitys.Any())
                            {
                                var Changefi = Where_ChangeProtitys.First();
                                Changefi.SetValue(TobjModel, fi.GetValue(objModel));

                                var WhereOldTClass_PropertyInfos = OldTClass_PropertyInfos.Where(x => x.Name == fi.Name);
                                if (WhereOldTClass_PropertyInfos.Any())
                                {
                                    Changefi = Changefi_s.First();
                                    object objval = WhereOldTClass_PropertyInfos.FirstOrDefault().GetValue(OldObjModel);
                                    //Changefi.SetValue(TobjModel, objval);
                                    setProtityValue(TobjModel, Changefi, objval);
                                }
                            }
                            else
                            {
                                var Changefi = Changefi_s.First();
                                object objval = fi.GetValue(objModel);
                                //Changefi.SetValue(TobjModel, objval);
                                setProtityValue(TobjModel, Changefi, objval);
                            }
                        }
                    }
                }

                #endregion

                if (AutoInsert)
                {
                    #region 自动新增

                    ////类的Type
                    //Type type = typeof(T);
                    ////UnitOfWork类的Type
                    //Type UnitOfWorkType = _unitOfWork.GetType();
                    ////获取UnitOfWork类的Repository泛型方法
                    //MethodInfo Method = UnitOfWorkType.GetMethod("Repository");
                    ////为Repository泛型方法 添加泛型类
                    //Method = Method.MakeGenericMethod(type);
                    ////类的IRepository对象
                    //var Rep = Method.Invoke(_unitOfWork, null);
                    //Type RepType = Rep.GetType();
                    ////类的IRepository对象的所有方法
                    //MethodInfo[] Methods = RepType.GetMethods();
                    //MethodInfo InsertMethod = Methods.Where(x => x.Name == "Insert").FirstOrDefault();
                    //if (InsertMethod != null)
                    //{
                    //    #region 判断必填项 是否已填写

                    //    foreach (System.Reflection.PropertyInfo Changefi in Change_PropertyInfos)
                    //    {
                    //        //属性数据类型
                    //        string DataType = Changefi.PropertyType.Name;
                    //        if (GetAttributeRequired(Changefi) || GetMetaRequired(Changefi))
                    //        {
                    //            //属性值
                    //            var valobj = Changefi.GetValue(TobjModel);
                    //            if (valobj == null)
                    //            {
                    //                if (Changefi.Name == "ID")
                    //                {
                    //                    Changefi.SetValue(TobjModel, 0);
                    //                }
                    //                if (Changefi.Name == "EMS_ORG_Type")
                    //                {
                    //                    var EMS_ORG_Type = typeof(TClass).ToString();
                    //                    var index = 0;
                    //                    index = EMS_ORG_Type.LastIndexOf('.');
                    //                    Changefi.SetValue(TobjModel, index > 0 ? EMS_ORG_Type.Substring(index + 1) : EMS_ORG_Type);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                if (valobj.ToString() == "")
                    //                {
                    //                    if (Changefi.Name == "ID")
                    //                    {
                    //                        Changefi.SetValue(TobjModel, 0);
                    //                    }
                    //                    if (Changefi.Name == "EMS_ORG_Type")
                    //                    {
                    //                        var EMS_ORG_Type = typeof(TClass).ToString();
                    //                        var index = 0;
                    //                        index = EMS_ORG_Type.LastIndexOf('.');
                    //                        Changefi.SetValue(TobjModel, index > 0 ? EMS_ORG_Type.Substring(index + 1) : EMS_ORG_Type);
                    //                    }
                    //                }
                    //            }
                    //        }

                    //        //去除编辑状态字段值
                    //        if (SetEditPropertyNames.Contains(Changefi.Name))
                    //        {
                    //            if (DataType.IndexOf("String") >= 0)
                    //                Changefi.SetValue(TobjModel, "");
                    //            if (DataType.IndexOf("DateTime") >= 0)
                    //                Changefi.SetValue(TobjModel, null);
                    //        }
                    //    }

                    //    #endregion

                    //    List<object> ArrParam = new List<object>() { TobjModel };
                    //    InsertMethod.Invoke(Rep, ArrParam.ToArray());
                    //}

                    #endregion
                }
            }
            //}
            //catch(Exception ex)
            //{

            //}
            return TobjModel;
        }

        /// <summary>
        /// 自动设置属性相同的值
        /// </summary>
        /// <typeparam name="TSet">设置的Model类型</typeparam>
        /// <typeparam name="TGet">读取的Model类型</typeparam>
        /// <param name="SetobjModel">设置的Model</param>
        /// <param name="GetobjModel">读取的Model</param>
        /// <param name="IngoreFieldCase">不区字段分大小写</param>
        /// <param name="Set_PropertyInfos">设置的Model字段属性</param>
        /// <param name="Get_PropertyInfos">读取的Model字段属性</param>
        /// <param name="IngorNullGetVal">忽略空值</param>
        /// <returns></returns>
        public static TSet SetSamaProtity<TSet, TGet>(TSet SetobjModel, TGet GetobjModel, bool IngoreFieldCase = false,
            System.Reflection.PropertyInfo[] Set_PropertyInfos = null, System.Reflection.PropertyInfo[] Get_PropertyInfos = null,
            bool IngorNullGetVal = false)
            where TSet : class, new()
            where TGet : class, new()
        {
            #region 赋值相同项


            if (SetobjModel == null)
                SetobjModel = new TSet();

            if (Set_PropertyInfos == null)
                Set_PropertyInfos = SetobjModel == null ? new System.Reflection.PropertyInfo[] { } : SetobjModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

            if (Get_PropertyInfos == null)
                Get_PropertyInfos = GetobjModel == null ? new System.Reflection.PropertyInfo[] { } : GetobjModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

            //遍历该model实体的所有字段
            foreach (System.Reflection.PropertyInfo fi in Set_PropertyInfos)
            {
                string DataType = fi.PropertyType.Name;
                //泛型
                if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    //实体泛型的类型
                    var Arguments = fi.PropertyType.GetGenericArguments();
                    if (Arguments.Count() == 1)
                    {
                        Type ChildType = Arguments[0];
                        DataType = Arguments[0].Name;
                        if (ChildType != null)
                        {
                            if (ChildType == typeof(DateTime) || ChildType == typeof(int) || ChildType == typeof(decimal) ||
                                ChildType == typeof(double) || ChildType == typeof(float) || ChildType == typeof(bool))
                            {
                                var WhereGetfi_s = Get_PropertyInfos.Where(x => IngoreFieldCase ? (x.Name.ToUpper() == fi.Name.ToUpper()) : x.Name == fi.Name);
                                if (WhereGetfi_s.Any())
                                {
                                    var Getfi = WhereGetfi_s.First();
                                    if (Getfi.CanRead && fi.CanWrite)
                                    {
                                        var GetVal = Getfi.GetValue(GetobjModel);
                                        if (IngorNullGetVal)
                                        {
                                            if (!string.IsNullOrWhiteSpace(GetVal.ToString()))
                                                setProtityValue(SetobjModel, fi, GetVal);
                                        }
                                        else
                                            setProtityValue(SetobjModel, fi, GetVal);
                                    }
                                }
                            }
                        }
                    }
                }
                ////判断是否派生自IEnumerable
                //else if (fi.PropertyType.GetInterface("IEnumerable", false) != null && DataType.ToLower().IndexOf("string") < 0)
                //{
                //}
                else
                {
                    var WhereGetfi_s = Get_PropertyInfos.Where(x => IngoreFieldCase ? (x.Name.ToUpper() == fi.Name.ToUpper()) : x.Name == fi.Name);
                    if (WhereGetfi_s.Any())
                    {
                        var Getfi = WhereGetfi_s.First();
                        if (Getfi.CanRead && fi.CanWrite)
                        {
                            var GetVal = Getfi.GetValue(GetobjModel);
                            if (IngorNullGetVal)
                            {
                                if (!string.IsNullOrWhiteSpace(GetVal.ToString()))
                                    setProtityValue(SetobjModel, fi, GetVal);
                            }
                            else
                                setProtityValue(SetobjModel, fi, GetVal);
                        }
                    }
                }
            }

            #endregion

            return SetobjModel;
        }

        /// <summary>
        /// 设置相同属性值
        /// </summary>
        /// <typeparam name="SetType">要设置的类型 Type</typeparam>
        /// <typeparam name="GetobjModel">获取相同值的 数据</typeparam>
        /// <param name="IngoreFieldCase">是否区分大小写</param>
        /// <returns></returns>
        public static object SetSamaProtity(Type SetType, Object GetobjModel, System.Reflection.Assembly assembly = null, bool IngoreFieldCase = false, System.Reflection.PropertyInfo[] Set_PropertyInfos = null, System.Reflection.PropertyInfo[] Get_PropertyInfos = null)
        {
            if (assembly == null)
                assembly = Assembly;

            #region 赋值相同项

            object SetobjModel = null;
            if (SetType != null)
            {
                SetobjModel = Activator.CreateInstance(SetType);
            }
            else
                return null;

            if (Set_PropertyInfos == null)
                Set_PropertyInfos = SetobjModel == null ? new System.Reflection.PropertyInfo[] { } : SetobjModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (Get_PropertyInfos == null)
                Get_PropertyInfos = new System.Reflection.PropertyInfo[] { };

            Type Get_Type = GetobjModel.GetType();
            bool IsDynamic = false;

            if (Get_Type.FullName.IndexOf("Dynamic") > 0)
            {
                IsDynamic = true;
            }
            else
            {
                if (GetobjModel != null)
                {
                    Get_PropertyInfos = GetobjModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                }
                else
                {
                    return null;
                }
            }

            if (!Set_PropertyInfos.Any())
            {
                System.Reflection.FieldInfo[] Set_FieldInfos = SetobjModel == null ? new System.Reflection.FieldInfo[] { } : SetobjModel.GetType().GetFields().Where(x => x.MemberType == MemberTypes.Field).ToArray();
                //遍历该model实体的所有字段
                foreach (System.Reflection.FieldInfo fi in Set_FieldInfos)
                {
                    //设置值
                    var SetObjVal = fi.GetValue(SetobjModel);
                    string DataType = fi.FieldType.Name;
                    //获取值
                    object GetObjVal = null;
                    if (IsDynamic)
                    {
                        var WhereGetfi_s = ((IDictionary<string, object>)GetobjModel).Where(x => IngoreFieldCase ? (x.Key.ToUpper() == fi.Name.ToUpper()) : x.Key == fi.Name);
                        if (WhereGetfi_s.Any())
                        {
                            var Getfi = WhereGetfi_s.First();
                            GetObjVal = Getfi.Value;
                        }
                    }
                    else
                    {
                        var WhereGetfi_s = Get_PropertyInfos.Where(x => IngoreFieldCase ? (x.Name.ToUpper() == fi.Name.ToUpper()) : x.Name == fi.Name);
                        if (WhereGetfi_s.Any())
                        {
                            var Getfi = WhereGetfi_s.First();
                            GetObjVal = Getfi.GetValue(GetobjModel);
                        }
                    }
                    if (GetObjVal == null)
                        continue;
                    //泛型
                    if (fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //泛型 类型
                        var Arguments = fi.FieldType.GetGenericArguments();
                        if (Arguments.Count() == 1)
                        {
                            Type ChildType = Arguments[0];
                            DataType = Arguments[0].Name;
                            if (ChildType != null)
                            {
                                if (ChildType == typeof(DateTime) || ChildType == typeof(int) || ChildType == typeof(decimal) ||
                                    ChildType == typeof(double) || ChildType == typeof(float) || ChildType == typeof(bool))
                                {
                                    Common.setProtityValue(SetobjModel, fi, GetObjVal);
                                }
                            }
                        }
                    }
                    //判断是否派生自IEnumerable(string 是特殊的数组)
                    else //if (fi.FieldType.GetInterface("IEnumerable", false) != null && DataType.ToLower().IndexOf("string") < 0)
                        if (fi.FieldType.GetInterface("IEnumerable", false) != null &&
                        (fi.FieldType.Name.ToLower().IndexOf("string") < 0 ||
                        (fi.FieldType.Name.ToLower().IndexOf("string") >= 0 &&
                        (fi.FieldType.Name.ToLower().IndexOf("[]") > 0 || fi.FieldType.Name.ToLower().IndexOf("<") > 0))))
                        {
                            var Arrobjval = GetObjVal as System.Collections.IEnumerable;

                            //是List数组还是Array数组
                            bool IsList = true;

                            #region  创建List<T> 实例 并赋值

                            Type ListTType = null;//泛型类
                            var IEnumerableTypes = fi.FieldType.GetGenericArguments();
                            if (IEnumerableTypes.Any())
                            {
                                //List<> 数组
                                ListTType = IEnumerableTypes[0];
                            }
                            else
                            {
                                //数组
                                ListTType = null;//数组类型
                                ListTType = assembly.GetType(fi.FieldType.FullName.Replace("[]", ""));
                                IsList = false;
                            }

                            Type ListType = typeof(List<>);
                            ListType = ListType.MakeGenericType(ListTType);
                            //创建List数组实例
                            var ObjListT = Activator.CreateInstance(ListType);
                            Type argsType = GetObjVal.GetType();
                            //if (argsType.GetInterface("IEnumerable", false) != null && (argsType.Name.ToLower().IndexOf("string") < 0 && argsType.Name.ToLower().IndexOf("[]") < 0 && argsType.Name.ToLower().IndexOf("<") < 0))
                            if (argsType.GetInterface("IEnumerable", false) != null &&
                            (argsType.Name.ToLower().IndexOf("string") < 0 ||
                            (argsType.Name.ToLower().IndexOf("string") >= 0 &&
                            (argsType.Name.ToLower().IndexOf("[]") > 0 || argsType.Name.ToLower().IndexOf("<") > 0))))
                            {
                                MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                if (AddMethodInfo != null)
                                {
                                    foreach (var item in Arrobjval)
                                    {
                                        var obj = SetSamaProtity(ListTType, item, assembly, IngoreFieldCase);
                                        AddMethodInfo.Invoke(ObjListT, new object[] { obj });
                                    }
                                }

                                if (IsList)
                                {
                                    Common.setProtityValue(SetobjModel, fi, ObjListT);
                                }
                                else
                                {
                                    MethodInfo ToArrayMethodInfo = ListType.GetMethod("ToArray");
                                    if (ToArrayMethodInfo != null)
                                    {
                                        var ArrObj = ToArrayMethodInfo.Invoke(ObjListT, null);
                                        Common.setProtityValue(SetobjModel, fi, ArrObj);
                                    }
                                }
                            }

                            #endregion
                        }
                        //判断是否是 基元类型 string struct datetime decimal 为特殊的 基元类型
                        //基元类型：sbyte / byte / short / ushort /int / uint / long / ulong / char / float / double / bool
                        //if ((fi.FieldType.IsPrimitive || fi.FieldType.IsValueType || fi.FieldType == typeof(string) || fi.FieldType == typeof(decimal) || fi.FieldType == typeof(DateTime)) && fi.FieldType.Name.ToLower().IndexOf("struct") < 0)
                        else if (fi.FieldType.IsClass && !fi.FieldType.IsPrimitive && fi.FieldType.Name.ToLower().IndexOf("string") < 0)
                        {
                            var obj = SetSamaProtity(fi.FieldType, GetObjVal, assembly, IngoreFieldCase);
                            Common.setProtityValue(SetobjModel, fi, obj);
                        }
                        else
                        {
                            Common.setProtityValue(SetobjModel, fi, GetObjVal);
                        }
                }
            }
            else
            {
                //遍历该model实体的所有字段
                foreach (System.Reflection.PropertyInfo fi in Set_PropertyInfos)
                {
                    //设置值
                    var SetObjVal = fi.GetValue(SetobjModel);
                    string DataType = fi.PropertyType.Name;
                    //获取值
                    object GetObjVal = null;
                    if (IsDynamic)
                    {
                        var WhereGetfi_s = ((IDictionary<string, object>)GetobjModel).Where(x => IngoreFieldCase ? (x.Key.ToUpper() == fi.Name.ToUpper()) : x.Key == fi.Name);
                        if (WhereGetfi_s.Any())
                        {
                            var Getfi = WhereGetfi_s.First();
                            GetObjVal = Getfi.Value;
                        }
                    }
                    else
                    {
                        var WhereGetfi_s = Get_PropertyInfos.Where(x => IngoreFieldCase ? (x.Name.ToUpper() == fi.Name.ToUpper()) : x.Name == fi.Name);
                        if (WhereGetfi_s.Any())
                        {
                            var Getfi = WhereGetfi_s.First();
                            GetObjVal = Getfi.GetValue(GetobjModel);
                        }
                    }


                    if (GetObjVal == null)
                        continue;
                    //泛型
                    if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //泛型 类型
                        var Arguments = fi.PropertyType.GetGenericArguments();
                        if (Arguments.Count() == 1)
                        {
                            Type ChildType = Arguments[0];
                            DataType = Arguments[0].Name;
                            if (ChildType != null)
                            {
                                if (ChildType == typeof(DateTime) || ChildType == typeof(int) || ChildType == typeof(decimal) ||
                                    ChildType == typeof(double) || ChildType == typeof(float) || ChildType == typeof(bool))
                                {
                                    Common.setProtityValue(SetobjModel, fi, GetObjVal);
                                }
                            }
                        }
                    }
                    //判断是否派生自IEnumerable(string 是特殊的数组)
                    else //if (fi.PropertyType.GetInterface("IEnumerable", false) != null && DataType.ToLower().IndexOf("string") < 0)
                        if (fi.PropertyType.GetInterface("IEnumerable", false) != null &&
                        (fi.PropertyType.Name.ToLower().IndexOf("string") < 0 ||
                        (fi.PropertyType.Name.ToLower().IndexOf("string") >= 0 &&
                        (fi.PropertyType.Name.ToLower().IndexOf("[]") > 0 || fi.PropertyType.Name.ToLower().IndexOf("<") > 0))))
                        {
                            var Arrobjval = GetObjVal as System.Collections.IEnumerable;

                            //是List数组还是Array数组
                            bool IsList = true;

                            #region  创建List<T> 实例 并赋值

                            Type ListTType = null;//泛型类
                            var IEnumerableTypes = fi.PropertyType.GetGenericArguments();
                            if (IEnumerableTypes.Any())
                            {
                                //List<> 数组
                                ListTType = IEnumerableTypes[0];
                            }
                            else
                            {
                                //数组
                                ListTType = null;//数组类型
                                ListTType = assembly.GetType(fi.PropertyType.FullName.Replace("[]", ""));
                                IsList = false;
                            }

                            Type ListType = typeof(List<>);
                            ListType = ListType.MakeGenericType(ListTType);
                            //创建List数组实例
                            var ObjListT = Activator.CreateInstance(ListType);
                            Type argsType = GetObjVal.GetType();
                            //if (argsType.GetInterface("IEnumerable", false) != null && (argsType.Name.ToLower().IndexOf("string") < 0 && argsType.Name.ToLower().IndexOf("[]") < 0 && argsType.Name.ToLower().IndexOf("<") < 0))
                            if (argsType.GetInterface("IEnumerable", false) != null &&
                            (argsType.Name.ToLower().IndexOf("string") < 0 ||
                            (argsType.Name.ToLower().IndexOf("string") >= 0 &&
                            (argsType.Name.ToLower().IndexOf("[]") > 0 || argsType.Name.ToLower().IndexOf("<") > 0))))
                            {
                                MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                if (AddMethodInfo != null)
                                {
                                    foreach (var item in Arrobjval)
                                    {
                                        var obj = SetSamaProtity(ListTType, item, assembly, IngoreFieldCase);
                                        AddMethodInfo.Invoke(ObjListT, new object[] { obj });
                                    }
                                }
                                if (IsList)
                                {
                                    Common.setProtityValue(SetobjModel, fi, ObjListT);
                                }
                                else
                                {
                                    MethodInfo ToArrayMethodInfo = ListType.GetMethod("ToArray");
                                    if (ToArrayMethodInfo != null)
                                    {
                                        var ArrObj = ToArrayMethodInfo.Invoke(ObjListT, null);
                                        Common.setProtityValue(SetobjModel, fi, ArrObj);
                                    }
                                }
                            }

                            #endregion
                        }
                        //判断是否是 基元类型 string struct datetime decimal 为特殊的 基元类型
                        //基元类型：sbyte / byte / short / ushort /int / uint / long / ulong / char / float / double / bool
                        else if (fi.PropertyType.IsClass && !fi.PropertyType.IsPrimitive && fi.PropertyType.Name.ToLower().IndexOf("string") < 0)
                        {
                            var obj = SetSamaProtity(fi.PropertyType, GetObjVal, assembly, IngoreFieldCase);
                            Common.setProtityValue(SetobjModel, fi, obj);
                        }
                        else
                        {
                            Common.setProtityValue(SetobjModel, fi, GetObjVal);
                        }
                }
            }

            #endregion

            return SetobjModel;
        }

        /// <summary>
        /// 两个结构相同的类进行赋值
        /// </summary>
        /// <typeparam name="T">赋值类</typeparam>
        /// <typeparam name="L">被赋值类</typeparam>
        /// <param name="t">赋值类的数据</param>
        /// <param name="Exclude">排除的字段</param>
        /// <returns></returns>
        public static L SetProperties<T, L>(T t, string Exclude) where L : new()
        {
            if (t == null)
            {
                return default(L);
            }
            if (string.IsNullOrEmpty(Exclude))
            {
                return default(L);
            }
            var _entity = Exclude.Split(',');

            System.Reflection.PropertyInfo[] propertiesT = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            System.Reflection.PropertyInfo[] propertiesL = typeof(L).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (propertiesT.Length != propertiesL.Length || propertiesL.Length == 0)
            {
                return default(L);
            }
            L setT = new L();
            foreach (System.Reflection.PropertyInfo itemL in propertiesL)
            {
                if (_entity.Contains(itemL.Name))
                {
                    continue;
                }
                foreach (System.Reflection.PropertyInfo itemT in propertiesT)
                {
                    if (itemL.Name == itemT.Name)
                    {
                        object value = itemT.GetValue(t, null);
                        itemL.SetValue(setT, value, null);
                    }
                }
            }
            return setT;
        }

        #endregion

        /// <summary>
        /// 根据 类 和 字段 设置值
        /// </summary>
        /// <param name="TableClass"></param>
        /// <param name="FiledName"></param>
        /// <param name="DefaultValue"></param>
        public static void setProtityValue(Object TableClass = null, string FiledName = "", object DefaultValue = null)
        {
            try
            {
                if (TableClass == null || string.IsNullOrEmpty(FiledName))
                    return;

                System.Reflection.PropertyInfo[] PropertyInfos = TableClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                //遍历该model实体的所有字段
                foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                {
                    //获取字段名，用于查找该字段对应的display数据，来源List<ColumValue>
                    String _FiledName = fi.Name;
                    object s = fi.GetValue(TableClass, null);
                    string DataType = "";
                    if (fi.Name.ToLower() == FiledName.ToLower())
                    {
                        DataType = fi.PropertyType.Name;
                        //如果是 泛型 decimal?、List<T> 等等
                        if (fi.PropertyType.IsGenericType)
                        {
                            //如果是decimal?等泛型
                            if (fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                var Arguments = fi.PropertyType.GetGenericArguments();
                                if (Arguments.Any())
                                {
                                    if (Arguments.Count() == 1)
                                    {
                                        DataType = Arguments[0].Name;
                                    }
                                }
                            }
                        }
                        switch (DataType.ToLower())
                        {
                            case "int":
                                int Dftint = 0;
                                if (int.TryParse(DefaultValue.ToString(), out Dftint))
                                {
                                    fi.SetValue(TableClass, Dftint, null);
                                }
                                break;
                            case "int32":
                                int Dftint32 = 0;
                                if (int.TryParse(DefaultValue.ToString(), out Dftint32))
                                {
                                    fi.SetValue(TableClass, Dftint32, null);
                                }
                                break;
                            case "int64":
                                Int64 Dftint64 = 0;
                                if (Int64.TryParse(DefaultValue.ToString(), out Dftint64))
                                {
                                    fi.SetValue(TableClass, Dftint64, null);
                                }
                                break;
                            case "decimal":
                                decimal Dftdecimal = 0;
                                if (decimal.TryParse(DefaultValue.ToString(), out Dftdecimal))
                                {
                                    fi.SetValue(TableClass, Dftdecimal, null);
                                }
                                break;
                            case "double":
                                double Dftdouble = 0;
                                if (double.TryParse(DefaultValue.ToString(), out Dftdouble))
                                {
                                    fi.SetValue(TableClass, Dftdouble, null);
                                }
                                break;
                            case "float":
                                float Dftfloat = 0;
                                if (float.TryParse(DefaultValue.ToString(), out Dftfloat))
                                {
                                    fi.SetValue(TableClass, Dftfloat, null);
                                }
                                break;
                            case "string":
                                fi.SetValue(TableClass, DefaultValue.ToString(), null);
                                break;
                            case "datetime":
                                int TDatetime = 0;
                                if (int.TryParse(DefaultValue.ToString(), out TDatetime))
                                {
                                    fi.SetValue(TableClass, DateTime.Now.AddDays(TDatetime), null);
                                }
                                else
                                {
                                    DateTime DftDateTime = new DateTime();
                                    if (DateTime.TryParse(DefaultValue.ToString(), out DftDateTime))
                                    {
                                        fi.SetValue(TableClass, DftDateTime, null);
                                    }
                                }
                                break;
                            case "bool":
                                bool Dftbool = false;
                                if (bool.TryParse(DefaultValue.ToString(), out Dftbool))
                                {
                                    fi.SetValue(TableClass, Dftbool, null);
                                }
                                break;
                            default:
                                fi.SetValue(TableClass, DefaultValue, null);
                                break;
                        }
                        break;
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 根据 类 和 字段 设置值
        /// </summary>
        /// <param name="TableClass"></param>
        /// <param name="fi"></param>
        /// <param name="DefaultValue"></param>
        public static void setProtityValue(Object TableClass = null, PropertyInfo fi = null, object DefaultValue = null)
        {
            try
            {
                if (fi != null)
                {
                    //获取字段名，用于查找该字段对应的display数据，来源List<ColumValue>
                    String _FiledName = fi.Name;
                    object s = fi.GetValue(TableClass, null);
                    string DataType = "";
                    DataType = fi.PropertyType.Name;
                    //如果是 泛型 decimal?、List<T> 等等
                    if (fi.PropertyType.IsGenericType)
                    {
                        //如果是decimal?等泛型
                        if (fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var Arguments = fi.PropertyType.GetGenericArguments();
                            if (Arguments.Any())
                            {
                                if (Arguments.Count() == 1)
                                {
                                    DataType = Arguments[0].Name;
                                }
                            }
                        }
                    }
                    var StrVal = DefaultValue == null ? "" : DefaultValue.ToString();
                    switch (DataType.ToLower())
                    {
                        case "int":
                        case "int32":
                            int Dftint = 0;
                            if (int.TryParse(StrVal, out Dftint))
                            {
                                fi.SetValue(TableClass, Dftint, null);
                            }
                            break;
                        case "long":
                        case "int64":
                            Int64 Dftint64 = 0;
                            if (Int64.TryParse(StrVal, out Dftint64))
                            {
                                fi.SetValue(TableClass, Dftint64, null);
                            }
                            break;
                        case "decimal":
                            decimal Dftdecimal = 0;
                            if (decimal.TryParse(StrVal, out Dftdecimal))
                            {
                                fi.SetValue(TableClass, Dftdecimal, null);
                            }
                            break;
                        case "double":
                            double Dftdouble = 0;
                            if (double.TryParse(StrVal, out Dftdouble))
                            {
                                fi.SetValue(TableClass, Dftdouble, null);
                            }
                            break;
                        case "float":
                            float Dftfloat = 0;
                            if (float.TryParse(StrVal, out Dftfloat))
                            {
                                fi.SetValue(TableClass, Dftfloat, null);
                            }
                            break;
                        case "string":
                            fi.SetValue(TableClass, StrVal, null);
                            break;
                        case "datetime":
                            int TDatetime = 0;
                            if (int.TryParse(StrVal, out TDatetime))
                            {
                                fi.SetValue(TableClass, DateTime.Now.AddDays(TDatetime), null);
                            }
                            else
                            {
                                DateTime DftDateTime = new DateTime();
                                if (DateTime.TryParse(StrVal, out DftDateTime))
                                {
                                    fi.SetValue(TableClass, DftDateTime, null);
                                }
                            }
                            break;
                        case "bool":
                            bool Dftbool = false;
                            if (bool.TryParse(StrVal, out Dftbool))
                            {
                                fi.SetValue(TableClass, Dftbool, null);
                            }
                            break;
                        default:
                            fi.SetValue(TableClass, DefaultValue, null);
                            break;
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 根据 类 和 字段 设置值
        /// </summary>
        /// <param name="TableClass"></param>
        /// <param name="fi"></param>
        /// <param name="DefaultValue"></param>
        public static void setProtityValue(Object TableClass = null, FieldInfo fi = null, object DefaultValue = null)
        {
            try
            {
                if (fi != null)
                {
                    //获取字段名，用于查找该字段对应的display数据，来源List<ColumValue>
                    String _FiledName = fi.Name;
                    object s = fi.GetValue(TableClass);
                    string DataType = "";
                    DataType = fi.FieldType.Name;
                    //如果是 泛型 decimal?、List<T> 等等
                    if (fi.FieldType.IsGenericType)
                    {
                        //如果是decimal?等泛型
                        if (fi.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var Arguments = fi.FieldType.GetGenericArguments();
                            if (Arguments.Any())
                            {
                                if (Arguments.Count() == 1)
                                {
                                    DataType = Arguments[0].Name;
                                }
                            }
                        }
                    }
                    var StrVal = DefaultValue == null ? "" : DefaultValue.ToString();
                    switch (DataType.ToLower())
                    {
                        case "int":
                        case "int32":
                            int Dftint = 0;
                            if (int.TryParse(StrVal, out Dftint))
                            {
                                fi.SetValue(TableClass, Dftint);
                            }
                            break;
                        case "int64":
                        case "long":
                            Int64 Dftint64 = 0;
                            if (Int64.TryParse(StrVal, out Dftint64))
                            {
                                fi.SetValue(TableClass, Dftint64);
                            }
                            break;
                        case "decimal":
                            decimal Dftdecimal = 0;
                            if (decimal.TryParse(StrVal, out Dftdecimal))
                            {
                                fi.SetValue(TableClass, Dftdecimal);
                            }
                            break;
                        case "double":
                            double Dftdouble = 0;
                            if (double.TryParse(StrVal, out Dftdouble))
                            {
                                fi.SetValue(TableClass, Dftdouble);
                            }
                            break;
                        case "float":
                            float Dftfloat = 0;
                            if (float.TryParse(StrVal, out Dftfloat))
                            {
                                fi.SetValue(TableClass, Dftfloat);
                            }
                            break;
                        case "string":
                            fi.SetValue(TableClass, StrVal);
                            break;
                        case "datetime":
                            int TDatetime = 0;
                            if (int.TryParse(StrVal, out TDatetime))
                            {
                                fi.SetValue(TableClass, DateTime.Now.AddDays(TDatetime));
                            }
                            else
                            {
                                DateTime DftDateTime = new DateTime();
                                if (DateTime.TryParse(StrVal, out DftDateTime))
                                {
                                    fi.SetValue(TableClass, DftDateTime);
                                }
                            }
                            break;
                        case "bool":
                            bool Dftbool = false;
                            if (bool.TryParse(StrVal, out Dftbool))
                            {
                                fi.SetValue(TableClass, Dftbool);
                            }
                            break;
                        default:
                            fi.SetValue(TableClass, DefaultValue);
                            break;
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 根据 类 和 字段 获取值
        /// </summary>
        /// <param name="TableClass"></param>
        /// <param name="FiledName"></param>
        /// <param name="IngoreFieldCase">不区分 字段名 大小写</param>
        /// <returns></returns>
        public static object GetProtityValue(Object TableClass = null, string FiledName = "", bool IngoreFieldCase = true)
        {
            try
            {
                object retValue = "";
                Dictionary<string, object> dict = new Dictionary<string, object>();
                if (TableClass == null || string.IsNullOrEmpty(FiledName))
                    return null;

                System.Reflection.PropertyInfo[] PropertyInfos = TableClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                //遍历该model实体的所有字段
                foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                {
                    //获取字段名，用于查找该字段对应的display数据，来源List<ColumValue>
                    String _FiledName = fi.Name;
                    object fival = fi.GetValue(TableClass, null);
                    string DataType = "";
                    if (IngoreFieldCase ? fi.Name.ToLower() == FiledName.ToLower() : fi.Name == FiledName)
                    {
                        var retVal = fi.GetValue(TableClass);
                        DataType = fi.PropertyType.Name;
                        //判断是否是泛型
                        if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var Arguments = fi.PropertyType.GetGenericArguments();
                            if (Arguments.Any())
                            {
                                if (Arguments.Count() == 1)
                                {
                                    DataType = Arguments[0].Name;
                                }
                            }
                        }
                        var StrVal = retVal == null ? "" : retVal.ToString();
                        switch (DataType.ToLower())
                        {
                            case "int":
                            case "int32":
                                int Dftint = 0;
                                if (int.TryParse(StrVal, out Dftint))
                                {
                                    retVal = (object)Dftint;
                                    retValue = retVal;
                                }
                                break;
                            case "int64":
                            case "long":
                                Int64 Dftint64 = 0;
                                if (Int64.TryParse(StrVal, out Dftint64))
                                {
                                    retVal = (object)Dftint64;
                                    retValue = retVal;
                                }
                                break;
                            case "string":
                                retValue = retVal;
                                break;
                            case "datetime":
                                int TDatetime = 0;
                                if (int.TryParse(StrVal, out TDatetime))
                                {
                                    retVal = (object)TDatetime;
                                    retValue = retVal;
                                }
                                else
                                {
                                    DateTime DftDateTime = new DateTime();
                                    if (DateTime.TryParse(StrVal, out DftDateTime))
                                    {
                                        retVal = (object)DftDateTime;
                                        retValue = retVal;
                                    }
                                }
                                break;
                            case "bool":
                                bool Dftbool = false;
                                if (bool.TryParse(StrVal, out Dftbool))
                                {
                                    retVal = (object)Dftbool;
                                    retValue = retVal;
                                }
                                break;
                            case "decimal":
                                decimal Dftdecimal = 0;
                                if (decimal.TryParse(StrVal, out Dftdecimal))
                                {
                                    if (FiledName == "Volume_CK" && TableClass.ToString() == "AirOut.Web.Controllers.Warehouse_receiptsController+pdfBF")
                                    {
                                        string str = Dftdecimal.ToString("f3");
                                        retVal = (object)str;
                                        retValue = retVal;
                                    }
                                    else
                                    {
                                        string str = Dftdecimal.ToString("f2");
                                        retVal = (object)str;
                                        retValue = retVal;
                                    }
                                }
                                break;
                            case "double":
                                double Dftdouble = 0;
                                if (double.TryParse(StrVal, out Dftdouble))
                                {
                                    string str = Dftdouble.ToString("f2");
                                    retVal = (object)str;
                                    retValue = retVal;
                                }
                                break;
                            case "float":
                                float Dftfloat = 0;
                                if (float.TryParse(StrVal, out Dftfloat))
                                {
                                    string str = Dftfloat.ToString("f2");
                                    retVal = (object)str;
                                    retValue = retVal;
                                }
                                break;
                            default:
                                retValue = retVal;
                                break;
                        }
                        dict.Add(DataType, fival);
                        break;
                    }
                }
                return retValue;// dict;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据 类 的 字段的 ProtityInfo
        /// </summary>
        /// <param name="TableClass"></param>
        /// <param name="FiledName"></param>
        /// <param name="IngoreFieldCase">不区分 字段名 大小写</param>
        /// <returns></returns>
        public static PropertyInfo GetProtityInfoByFieldName(Object TableClass = null, string FiledName = "", bool IngoreFieldCase = true)
        {
            try
            {
                PropertyInfo retPropertyInfo = null;
                Dictionary<string, object> dict = new Dictionary<string, object>();

                if (TableClass == null || string.IsNullOrEmpty(FiledName))
                    return null;

                retPropertyInfo = TableClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).
                    Where(x => IngoreFieldCase ? x.Name.ToLower() == FiledName.ToLower() : x.Name == FiledName).FirstOrDefault();

                return retPropertyInfo;// dict;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取属性是否时必须的
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool GetAttributeRequired(PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), true);
            if (atts.Length == 0)
                return false;
            return true;
        }

        /// <summary>
        /// 获取属性的MetaType是否是必须的
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool GetMetaRequired(PropertyInfo property)
        {
            var atts = property.DeclaringType.GetCustomAttributes(typeof(MetadataTypeAttribute), true);
            if (atts.Length == 0)
                return false;

            var metaAttr = atts[0] as MetadataTypeAttribute;
            var metaProperty = metaAttr.MetadataClassType.GetProperty(property.Name);
            if (metaProperty == null)
                return false;
            return GetAttributeRequired(metaProperty);
        }

        /// <summary>
        /// 反射获取 主键值
        /// </summary>
        /// <returns></returns>
        public static string GetKeyValue(object entity, PropertyInfo[] _entityProptys)
        {
            string KeyVal = "";
            foreach (var propinfo in _entityProptys)
            {
                var ArrKeyAttr = propinfo.GetCustomAttributes(typeof(KeyAttribute), false);
                if (ArrKeyAttr.Any())
                {
                    KeyVal = propinfo.GetValue(entity).ToString();
                    break;
                }
            }
            if (string.IsNullOrEmpty(KeyVal))
            {
                var KeyProptyS = _entityProptys.Where(x => x.Name.ToLower() == "ID");
                if (KeyProptyS.Any())
                {
                    KeyVal = KeyProptyS.FirstOrDefault().GetValue(entity).ToString();
                }
            }
            return KeyVal;
        }

        #region 获取/设置 类的默认值，存储在XML文件中

        //private static DateTime? LastEditTime = null;//最后修改时间
        /// <summary>
        /// 获取费用-计费条件XML
        /// </summary>
        /// <returns></returns>
        public static List<dynamic> GetBillFormulaXML()
        {
            string XMLPath = System.Configuration.ConfigurationManager.AppSettings["BillFormulaXml"] ?? "/App_Data/BillFormula.xml";
            List<dynamic> ArrBillFormula = new List<dynamic>();
            try
            {
                FileInfo FInfo = new FileInfo(HttpContext.Current.Server.MapPath(XMLPath));
                if (FInfo.Exists)
                {
                    //已在CacheHelper中设置文件依赖（文件修改 清空缓存）
                    //if (LastEditTime == null)
                    //    LastEditTime = FInfo.LastWriteTime;
                    //else
                    //{
                    //    var ObjBillFormulaXML = CacheHelper.Get_SetCache(CacheNameS.BillFormulaXML);
                    //    if (ObjBillFormulaXML != null)
                    //    {
                    //        if (LastEditTime >= FInfo.LastWriteTime)
                    //            return (List<dynamic>)ObjBillFormulaXML;
                    //    }
                    //}

                    XmlDocument doc = new XmlDocument();
                    doc.Load(FInfo.FullName);
                    XmlNode xmlTableNode = doc.SelectSingleNode("BillFormula");
                    if (xmlTableNode != null)
                    {
                        if (xmlTableNode.HasChildNodes)
                        {
                            XmlNodeList xmltableNodes = xmlTableNode.SelectNodes("item");
                            if (xmltableNodes != null)
                            {
                                foreach (XmlNode xmlAutoSetNode in xmltableNodes)
                                {
                                    dynamic dyObj = new System.Dynamic.ExpandoObject();
                                    dyObj.ID = dyObj.Name = xmlAutoSetNode.Attributes["Name"].Value;
                                    dyObj.TEXT = dyObj.Desc = xmlAutoSetNode.Attributes["desc"].Value;
                                    dyObj.Display = xmlAutoSetNode.InnerText;
                                    ArrBillFormula.Add(dyObj);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                ArrBillFormula = new List<dynamic>();
            }
            return ArrBillFormula;
        }

        /// <summary>
        /// 获取所有设置默认值的Table名称
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllDefaultTabName()
        {
            string XMLPath = System.Configuration.ConfigurationManager.AppSettings["SetDefaultsXml"] ?? "/App_Data/SetDefaults.xml";
            List<string> ArrSetDefaultTabName = new List<string>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath(XMLPath));
                XmlNode xmlTableNode = doc.SelectSingleNode("WebSettings/DefaultTables");
                if (xmlTableNode != null)
                {
                    if (xmlTableNode.HasChildNodes)
                    {
                        XmlNodeList xmltableNodes = xmlTableNode.SelectNodes("table");
                        if (xmltableNodes != null)
                        {
                            foreach (XmlNode xmlAutoSetNode in xmltableNodes)
                            {
                                var TabName = xmlAutoSetNode.Attributes["name"].Value;
                                var TabNameChs = xmlAutoSetNode.Attributes["value"].Value;
                                ArrSetDefaultTabName.Add(TabName);
                            }
                        }
                    }
                }
            }
            catch
            {
                ArrSetDefaultTabName = new List<string>();
            }
            return ArrSetDefaultTabName;
        }

        /// <summary>
        /// 根据表名 获取表的所有默认值 并返回为 List<SetDefaults>
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static List<SetDefaults> getAllSetDefaultsByTable(string TableName = "")
        {
            string XMLPath = System.Configuration.ConfigurationManager.AppSettings["SetDefaultsXml"] ?? "/App_Data/SetDefaults.xml";
            List<SetDefaults> ArrSetDefaults = new List<SetDefaults>();
            SetDefaults OSetDefaults = new SetDefaults();
            if (TableName == "")
            {
                ArrSetDefaults = new List<SetDefaults>();
            }
            else
            {
                try
                {
                    OSetDefaults = null;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(HttpContext.Current.Server.MapPath(XMLPath));
                    XmlNode xmlTableNode = doc.SelectSingleNode("WebSettings/table[@name='" + TableName + "']");
                    if (xmlTableNode != null)
                    {
                        if (xmlTableNode.HasChildNodes)
                        {
                            XmlNodeList xmlAutoSetNodes = xmlTableNode.SelectNodes("AutoSet");
                            if (xmlAutoSetNodes != null)
                            {
                                foreach (XmlNode xmlAutoSetNode in xmlAutoSetNodes)
                                {
                                    OSetDefaults = new SetDefaults();
                                    OSetDefaults.TableName = xmlAutoSetNode.ParentNode == null ? "" : xmlAutoSetNode.ParentNode.Attributes["name"].Value;
                                    OSetDefaults.TableNameChs = xmlAutoSetNode.ParentNode == null ? "" : xmlAutoSetNode.ParentNode.Attributes["value"].Value;
                                    OSetDefaults.ColumnName = xmlAutoSetNode.Attributes["colum"].Value;
                                    OSetDefaults.ColumnNameChs = xmlAutoSetNode.Attributes["name"].Value;
                                    OSetDefaults.DataType = xmlAutoSetNode.Attributes["type"].Value;
                                    OSetDefaults.DefaultValue = xmlAutoSetNode.Attributes["value"].Value;
                                    ArrSetDefaults.Add(OSetDefaults);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    ArrSetDefaults = new List<SetDefaults>();
                }
            }
            return ArrSetDefaults;
        }

        /// <summary>
        /// 为类设置 默认值
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="TableClass"></param>
        public static void SetDefaultValueToModel(string TableName = "", Object TableClass = null)
        {
            if (TableName == "")
                return;
            if (TableClass == null)
                return;
            List<SetDefaults> list = getAllSetDefaultsByTable(TableName);
            if (!list.Any())
                return;
            System.Reflection.PropertyInfo[] PropertyInfos = TableClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            //遍历该model实体的所有字段
            foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
            {
                //获取字段名，用于查找该字段对应的display数据，来源List<ColumValue>
                String FiledName = fi.Name;
                object s = fi.GetValue(TableClass, null);
                //获取display属性操作对象
                DisplayAttribute disAttr = (DisplayAttribute)fi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

                foreach (var item in list)
                {
                    if (item.ColumnName == FiledName)
                    {
                        switch (item.DataType.ToLower())
                        {
                            case "int":
                                int Dftint = 0;
                                if (int.TryParse(item.DefaultValue, out Dftint))
                                {
                                    fi.SetValue(TableClass, Dftint, null);
                                }
                                break;
                            case "int32":
                                int Dftint32 = 0;
                                if (int.TryParse(item.DefaultValue, out Dftint32))
                                {
                                    fi.SetValue(TableClass, Dftint32, null);
                                }
                                break;
                            case "int64":
                                Int64 Dftint64 = 0;
                                if (Int64.TryParse(item.DefaultValue, out Dftint64))
                                {
                                    fi.SetValue(TableClass, Dftint64, null);
                                }
                                break;
                            case "decimal":
                                decimal Dftdecimal = 0;
                                if (decimal.TryParse(item.DefaultValue, out Dftdecimal))
                                {
                                    fi.SetValue(TableClass, Dftdecimal, null);
                                }
                                break;
                            case "double":
                                double Dftdouble = 0;
                                if (double.TryParse(item.DefaultValue.ToString(), out Dftdouble))
                                {
                                    fi.SetValue(TableClass, Dftdouble, null);
                                }
                                break;
                            case "float":
                                float Dftfloat = 0;
                                if (float.TryParse(item.DefaultValue.ToString(), out Dftfloat))
                                {
                                    fi.SetValue(TableClass, Dftfloat, null);
                                }
                                break;
                            case "string":
                                fi.SetValue(TableClass, item.DefaultValue, null);
                                break;
                            case "datetime":
                                int TDatetime = 0;
                                if (int.TryParse(item.DefaultValue, out TDatetime))
                                {
                                    fi.SetValue(TableClass, DateTime.Now.AddDays(TDatetime), null);
                                }
                                else
                                {
                                    DateTime DftDateTime = new DateTime();
                                    if (DateTime.TryParse(item.DefaultValue, out DftDateTime))
                                    {
                                        fi.SetValue(TableClass, DftDateTime, null);
                                    }
                                }
                                break;
                            case "boolean":
                                bool Dftboolean = false;
                                if (bool.TryParse(item.DefaultValue, out Dftboolean))
                                {
                                    fi.SetValue(TableClass, Dftboolean, null);
                                }
                                break;
                            case "bool":
                                bool Dftbool = false;
                                if (bool.TryParse(item.DefaultValue, out Dftbool))
                                {
                                    fi.SetValue(TableClass, Dftbool, null);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }


            }

        }

        #endregion

        /// <summary>
        /// 将字符串时间 转换成 时间格式
        /// </summary>
        /// <param name="dateStr">字符串时间</param>
        /// <param name="ArrFormat">时间格式（如：yyyy-MM-dd HH:mm:ss）</param>
        /// <returns></returns>
        public static DateTime? ParseStrToDateTime(string dateStr, params string[] ArrFormat)
        {
            if (ArrFormat == null || !ArrFormat.Any())
            {
                ArrFormat = new string[]{
                   "yyyy/M/d", 
                   "yyyy/M/d h:mm",
                   "yyyy/M/d hh:mm",
                   "yyyy/M/d HH:mm",
                   "yyyy/M/d h:mm:ss", 
                   "yyyy/M/d hh:mm:ss", 
                   "yyyy/M/d HH:mm:ss", 
                   "yyyy/MM/dd", 
                   "yyyy/MM/dd h:mm",
                   "yyyy/MM/dd hh:mm",
                   "yyyy/MM/dd HH:mm",
                   "yyyy/MM/dd h:mm:ss", 
                   "yyyy/MM/dd hh:mm:ss", 
                   "yyyy/MM/dd HH:mm:ss"
                };
            }

            try
            {
                if (dateStr.IndexOf("-") > 0)
                {
                    //dateStr = Regex.Replace(dateStr, "/-/g", "/", RegexOptions.None);
                    dateStr = dateStr.Replace("-", "/");
                }
                DateTime dateVal;
                if (DateTime.TryParseExact(dateStr, ArrFormat, new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.None, out dateVal))
                //if (DateTime.TryParseExact(dateStr, ArrFormat, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dateVal))
                {
                    return dateVal;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Unable to convert '{0}' to a date.", dateStr);
                WriteLogHelper.WriteLogByLog4Net(ex, EnumType.Log4NetMsgType.Error);
                return null;
            }
        }

        /// <summary>
        /// 获取类的中文名
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetDisplayName(Type dataType, string fieldName)
        {
            // First look into attributes on a type and it's parents
            DisplayAttribute attr;
            attr = (DisplayAttribute)dataType.GetProperty(fieldName).GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

            if (attr == null)
            {
                return String.Empty;
            }
            else
                return (attr != null) ? attr.GetName() : String.Empty;
        }

        /// <summary>
        /// 获取类的Metadata中设置的Display中文名
        /// 先取 类中的 Display然后去 Metadata类中的Display
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="fieldName"></param>
        /// <returns>有MetadataType取Metadata,没有取类中的</returns>
        public static string GetMetaDataDisplayName(Type dataType, string fieldName)
        {
            // First look into attributes on a type and it's parents
            DisplayAttribute attr = null;
            attr = (DisplayAttribute)dataType.GetProperty(fieldName).GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

            MetadataTypeAttribute metadataType = (MetadataTypeAttribute)dataType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
            if (metadataType != null)
            {
                var property = metadataType.MetadataClassType.GetProperty(fieldName);
                if (property != null)
                {
                    attr = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                }
            }

            return (attr != null) ? attr.Name : String.Empty;
        }

        /// <summary>
        /// 获取类的Metadata中设置的Display中文名
        /// 先取 类中的 Display然后去 Metadata类中的Display
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="fieldName"></param>
        /// <returns>有MetadataType取Metadata,没有取类中的</returns>
        public static string GetDataDisplayName(PropertyInfo pi)
        {
            // First look into attributes on a type and it's parents
            DisplayAttribute attr = null;
            attr = (DisplayAttribute)pi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
            if (attr != null)
                return attr.Name;
            else
            {
                MetadataTypeAttribute metadataType = (MetadataTypeAttribute)pi.ReflectedType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
                if (metadataType != null)
                {
                    var property = metadataType.MetadataClassType.GetProperty(pi.Name);
                    if (property != null)
                    {
                        attr = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                        if (attr != null)
                            return attr.Name;
                    }
                }

                return String.Empty;

            }
        }

        /// <summary>
        /// 先取 类中的 Display属性，没有再去然后去 Metadata类中的Display属性
        /// 获取类的Metadata中设置的Display中文名
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="fieldName"></param>
        /// <returns>有MetadataType取Metadata,没有取类中的</returns>
        public static DisplayAttribute GetDataDisplayAttr(PropertyInfo pi)
        {
            // First look into attributes on a type and it's parents
            DisplayAttribute attr = null;
            attr = (DisplayAttribute)pi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
            if (attr == null)
            {
                MetadataTypeAttribute metadataType = (MetadataTypeAttribute)pi.ReflectedType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
                if (metadataType != null)
                {
                    var property = metadataType.MetadataClassType.GetProperty(pi.Name);
                    if (property != null)
                    {
                        attr = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                    }
                }
            }
            return attr;
        }

        /// <summary>
        /// 获取类的Metadata中设置的Display中文名
        /// 先取 类中的 Display然后去 Metadata类中的Display
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="fieldName"></param>
        /// <returns>有MetadataType取Metadata,没有取类中的</returns>
        public static string GetOnlyMetaDataDisplayName(Type dataType, string fieldName)
        {
            // First look into attributes on a type and it's parents
            DisplayAttribute attr = null;
            // Look for [MetadataType] attribute in type hierarchy
            // http://stackoverflow.com/questions/1910532/attribute-isdefined-doesnt-see-attributes-applied-with-metadatatype-class

            MetadataTypeAttribute metadataType = (MetadataTypeAttribute)dataType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
            if (metadataType != null)
            {
                var property = metadataType.MetadataClassType.GetProperty(fieldName);
                if (property != null)
                {
                    var ss = property.GetCustomAttributes();
                    attr = (DisplayAttribute)property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), true).SingleOrDefault();
                }
            }

            return (attr != null) ? attr.Name : String.Empty;
        }
    }
}