using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using TMI.Web.Models;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TMI.Web.Models
{
    public class WebdbContext : DataContext
    {
        public WebdbContext()
            //: base("Name=OracleConnection")
            : base("Name=DefaultConnection")
        {
            string ConfigName = "IsWriteDataToLunece";
            string IsWriteDataToLuneceStr = System.Configuration.ConfigurationManager.AppSettings[ConfigName] ?? "";
            IsWriteDataToLunece = TMI.Web.Extensions.Common.ChangStrToBool(IsWriteDataToLuneceStr);

            ConfigName = "IsWriteDataToRedis";
            string IsWriteDataToRedisStr = System.Configuration.ConfigurationManager.AppSettings["IsWriteDataToRedis"] ?? "";
            bool IsWriteDataToRedis = TMI.Web.Extensions.Common.ChangStrToBool(IsWriteDataToRedisStr);
            //this.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.ValidateOnSaveEnabled = false;
        }

        /// <summary>
        /// Oracle 表所有者，（SQL 改成 dbo(默认)，也可删除此设置）
        /// </summary>
        public string DbSchema
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DbSchema"] == null)
                    return "dbo";
                else
                    return System.Configuration.ConfigurationManager.AppSettings["DbSchema"].ToString();
            }
        }

        //添加时需要设定的字段名
        public static List<string> SetAddPropertyNames = new List<string>() { 
            "CreatedUserId",
            "CreatedUserName",
            "CreatedDateTime",
            "ADDID",
            "ADDWHO",
            "ADDTS",
            "OperatingPoint"
        };
        //编辑时需要设定的字段名
        public static List<string> SetEditPropertyNames = new List<string>() { 
            "LastEditUserId",
            "LastEditUserName",
            "LastEditDateTime",
            "EDITID",
            "EDITWHO",
            "EDITTS",
            "OperatingPoint"
        };
        //无需设定默认值的类
        public static List<string> NoAutoSetClassNames = new List<string>()
        {
        };
        //无需默认设置编辑人的名称 
        public static List<string> NoAutoSetUserName = new List<string> { 
            "CRMService"
        };

        #region  数据表

        /// <summary>
        /// 枚举表
        /// </summary>
        public DbSet<BD_DEFDOC> BD_DEFDOC { get; set; }

        /// <summary>
        /// 枚举表明细
        /// </summary>
        public DbSet<BD_DEFDOC_LIST> BD_DEFDOC_LIST { get; set; }

        public DbSet<MenuItem> MenuItem { get; set; }

        public DbSet<RoleMenu> RoleMenu { get; set; }

        public DbSet<DataTableImportMapping> DataTableImportMapping { get; set; }

        public DbSet<Company> Company { get; set; }

        public DbSet<Message> Message { get; set; }

        public DbSet<Notification> Notification { get; set; }

        public DbSet<Sequencer> Sequencer { get; set; }

        public DbSet<MenuAction> MenuAction { get; set; }

        public DbSet<RoleMenuAction> RoleMenuAction { get; set; }

        public DbSet<FileATTACH> FileATTACH { get; set; }

        public virtual DbSet<ChangeOrderHistory> ChangeOrderHistory { get; set; }

        public virtual DbSet<OperatePoint> OperatePoint { get; set; }

        public virtual DbSet<OperatePointList> OperatePointList { get; set; }

        public virtual DbSet<UserOperatePointLink> UserOperatePointLink { get; set; }

        public virtual DbSet<MailReceiver> MailReceiver { get; set; }

        public virtual DbSet<PARA_CURR> PARA_CURR { get; set; }

        public virtual DbSet<PARA_Country> PARA_Country { get; set; }

        public virtual DbSet<DailyRate> DailyRate { get; set; }

        public virtual DbSet<Rate> Rate { get; set; }

        #endregion

        #region TMI


        //--------------------------------------   基础资料    ----------------------------------------

        public DbSet<Customer> Customer { get; set; }

        public DbSet<AdvisoryOrder> AdvisoryOrder { get; set; }

        public DbSet<SupplyierAskPrice> SupplyierAskPrice { get; set; }

        public DbSet<OrderCustomer> OrderCustomer { get; set; }

        public DbSet<CostMoney> CostMoney { get; set; }

        public DbSet<ActualMoney> ActualMoney { get; set; }

        public DbSet<FinanceMoney> FinanceMoney { get; set; }

        public DbSet<Supplier> Supplier { get; set; }

        public DbSet<AirTicketOrder> AirTicketOrder { get; set; }

        public DbSet<AirLine> AirLine { get; set; }

        public DbSet<PlanePerson> PlanePerson { get; set; }

        public DbSet<TestMVC_CRUD> TestMVC_CRUD { get; set; }

        #endregion

        /// <summary>
        /// 开启全文检索功能
        /// </summary>
        private bool IsWriteDataToLunece { get; set; }

        /// <summary>
        /// 开启Redis缓存
        /// </summary>
        private bool IsWriteDataToRedis { get; set; }

        public override int SaveChanges()
        {
            int ret = 0;
            try
            {
                //WebdbContext db = new WebdbContext();
                //db.Configuration.AutoDetectChangesEnabled = false;

                var Entitys = ChangeTracker.Entries();
                List<Object> ArrRedisInsertUpdateObj = new List<object>();

                //没有变动跳过
                if (!Entitys.Any(_e => _e.State != EntityState.Unchanged))
                    return 0;

                string CurrentUserId = TMI.Web.Controllers.Utility.CurrentAppUser == null ? "" : TMI.Web.Controllers.Utility.CurrentAppUser.Id;
                string CurrentUserName = TMI.Web.Controllers.Utility.CurrentAppUser == null ? "" : TMI.Web.Controllers.Utility.CurrentAppUser.UserName;
                var CurrentUserOperatePoint = TMI.Web.Controllers.Utility.CurrentUserOperatePoint ?? new List<OperatePoint>();

                #region 获取所有新增的数据

                var _entitiesAdded = Entitys.Where(_e => _e.State == EntityState.Added).ToList();

                foreach (var entityitem in _entitiesAdded)
                {
                    var _entity = entityitem.Entity;
                    var _entityProptys = _entity.GetType().GetProperties();
                    if (IsWriteDataToRedis)
                        ArrRedisInsertUpdateObj.Add(_entity);

                    //自动设置属性
                    var AutoSetProtityS = _entityProptys.Where(x => SetAddPropertyNames.Contains(x.Name));

                    foreach (var propinfo in AutoSetProtityS)
                    {
                        var propertityVal = propinfo.GetValue(_entity);
                        #region 自动设置

                        if (propinfo.Name == "CreatedUserId")
                        {
                            if (propertityVal != null)
                            {
                                if (string.IsNullOrEmpty(propertityVal.ToString()))
                                {
                                    propinfo.SetValue(_entity, CurrentUserId);
                                }
                            }
                            else
                                propinfo.SetValue(_entity, CurrentUserId);
                        }
                        if (propinfo.Name == "CreatedUserName")
                        {
                            if (propertityVal != null)
                            {
                                if (string.IsNullOrEmpty(propertityVal.ToString()))
                                {
                                    propinfo.SetValue(_entity, CurrentUserName);
                                }
                            }
                            else
                                propinfo.SetValue(_entity, CurrentUserName);
                        }
                        if (propinfo.Name == "CreatedDateTime")
                        {
                            if (propertityVal != null)
                            {
                                if (string.IsNullOrEmpty(propertityVal.ToString()))
                                {
                                    propinfo.SetValue(_entity, DateTime.Now);
                                }
                            }
                            else
                                propinfo.SetValue(_entity, DateTime.Now);
                        }
                        //if (propinfo.Name == "LastEditUserId")
                        //{
                        //    propinfo.SetValue(_entity, CurrentUserId);
                        //}
                        //if (propinfo.Name == "LastEditUserName")
                        //{
                        //    propinfo.SetValue(_entity, CurrentUserName);
                        //}
                        //if (propinfo.Name == "LastEditDateTime")
                        //{
                        //    propinfo.SetValue(_entity, DateTime.Now);
                        //}

                        if (propinfo.Name == "ADDID")
                        {
                            if (propertityVal != null)
                            {
                                if (string.IsNullOrEmpty(propertityVal.ToString()))
                                {
                                    propinfo.SetValue(_entity, CurrentUserId);
                                }
                            }
                            else
                                propinfo.SetValue(_entity, CurrentUserId);
                        }
                        if (propinfo.Name == "ADDWHO")
                        {
                            if (propertityVal != null)
                            {
                                if (string.IsNullOrEmpty(propertityVal.ToString()))
                                {
                                    propinfo.SetValue(_entity, CurrentUserName);
                                }
                            }
                            else
                                propinfo.SetValue(_entity, CurrentUserName);
                        }
                        if (propinfo.Name == "ADDTS")
                        {
                            if (propertityVal != null)
                            {
                                if (string.IsNullOrEmpty(propertityVal.ToString()))
                                {
                                    propinfo.SetValue(_entity, DateTime.Now);
                                }
                            }
                            else
                                propinfo.SetValue(_entity, DateTime.Now);
                        }

                        if (propinfo.Name == "OperatingPoint")
                        {
                            var ObjOperatingPoint = propertityVal;
                            int OperatePointListID = 0;
                            if (ObjOperatingPoint != null)
                            {
                                if (int.TryParse(ObjOperatingPoint.ToString(), out OperatePointListID))
                                {
                                    if (OperatePointListID == 0)
                                    {
                                        if (CurrentUserOperatePoint != null)
                                        {
                                            if (CurrentUserOperatePoint.Any())
                                            {
                                                if (CurrentUserOperatePoint.Count == 1)
                                                {
                                                    OperatePointListID = CurrentUserOperatePoint.FirstOrDefault().ID;
                                                    propinfo.SetValue(_entity, OperatePointListID);
                                                }
                                                else
                                                    propinfo.SetValue(_entity, ObjOperatingPoint);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (CurrentUserOperatePoint != null)
                                    {
                                        if (CurrentUserOperatePoint.Any())
                                        {
                                            if (CurrentUserOperatePoint.Count == 1)
                                                OperatePointListID = CurrentUserOperatePoint.FirstOrDefault().ID;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (CurrentUserOperatePoint != null)
                                {
                                    if (CurrentUserOperatePoint.Any())
                                    {
                                        if (CurrentUserOperatePoint.Count == 1)
                                            OperatePointListID = CurrentUserOperatePoint.FirstOrDefault().ID;
                                    }
                                }
                            }
                            propinfo.SetValue(_entity, OperatePointListID);
                        }

                        //if (propinfo.Name == "EDITID")
                        //{
                        //    propinfo.SetValue(_entity, CurrentUserId);
                        //}
                        //if (propinfo.Name == "EDITWHO")
                        //{
                        //    propinfo.SetValue(_entity, CurrentUserName);
                        //}
                        //if (propinfo.Name == "EDITTS")
                        //{
                        //    propinfo.SetValue(_entity, DateTime.Now);
                        //}

                        #endregion
                    }

                    #region 设置是否自动更新

                    //var AutoUpdateProtys = _entityProptys.Where(x => x.GetCustomAttributes(typeof(CustomerUpdateAttr)).Any());
                    //if (AutoUpdateProtys.Any())
                    //{
                    //    foreach (var propInfo in AutoUpdateProtys)
                    //    {
                    //        object[] objAttrs = propInfo.GetCustomAttributes(typeof(CustomerUpdateAttr), true);
                    //        if (objAttrs.Length > 0)
                    //        {
                    //            CustomerUpdateAttr attr = objAttrs[0] as CustomerUpdateAttr;
                    //            if (attr != null)
                    //            {
                    //                if (!attr.AutoUpdate) //是否要自动更新
                    //                {
                    //                    base.Entry(entityitem).Property(propInfo.Name).IsModified = false;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    #endregion

                    #region 全文检索

                    try
                    {
                        if (IsWriteDataToLunece)
                            LuneceManager.IndexManager.OIndexManager.LuneceInsert(_entity, _entityProptys);
                    }
                    catch (Exception ex)
                    {
                        SQLDALHelper.WriteLogHelper.WriteLog("全文检索错误（LuneceInsert）：" + TMI.Web.Extensions.Common.GetExceptionMsg(ex), "LuneceError", true);
                    }

                    #endregion
                }

                #endregion

                #region 获取所有更新的数据

                var _entitiesChanged = ChangeTracker.Entries().Where(_e => _e.State == EntityState.Modified).ToList();
                foreach (var entityitem in _entitiesChanged)
                {
                    var _entity = entityitem.Entity;
                    var _entityProptys = _entity.GetType().GetProperties();
                    if (IsWriteDataToRedis)
                        ArrRedisInsertUpdateObj.Add(_entity);

                    //var stateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(_entity);
                    //entityitem.State = EntityState.Unchanged;
                    //stateEntry.SetModified();

                    //自动设置属性
                    var AutoSetProtityS = _entityProptys.Where(x => SetEditPropertyNames.Contains(x.Name));

                    #region 排除自动更新属性

                    ////排除自动更新属性
                    //Dictionary<string, bool> dictAutoUpdateProtys = new Dictionary<string, bool>();

                    var AutoUpdateProtys = _entityProptys.Where(x => x.GetCustomAttributes(typeof(CustomerUpdateAttr)).Any());
                    if (AutoUpdateProtys.Any())
                    {
                        foreach (var propInfo in AutoUpdateProtys)
                        {
                            object[] objAttrs = propInfo.GetCustomAttributes(typeof(CustomerUpdateAttr), true);
                            if (objAttrs.Length > 0)
                            {
                                CustomerUpdateAttr attr = objAttrs[0] as CustomerUpdateAttr;
                                if (attr != null)
                                {
                                    //dictAutoUpdateProtys.Add(propInfo.Name,attr.AutoUpdate);

                                    if (!attr.AutoUpdate) //是否要自动更新
                                    {
                                        this.Entry(_entity).Property(propInfo.Name).IsModified = false;
                                        //var Oldval = entityitem.OriginalValues[propInfo.Name];
                                        //propInfo.SetValue(entityitem.Entity, Oldval);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    foreach (var propinfo in AutoSetProtityS)
                    {
                        var propertityVal = propinfo.GetValue(_entity);
                        //var WheredictAutoUpdateProtys = dictAutoUpdateProtys.Where(x=>x.Key == propinfo.Name);
                        //if (WheredictAutoUpdateProtys.Any())
                        //{
                        //    if (WheredictAutoUpdateProtys.FirstOrDefault().Value)
                        //    {
                        //        stateEntry.SetModifiedProperty(propinfo.Name);
                        //    }
                        //}
                        //else
                        //    stateEntry.SetModifiedProperty(propinfo.Name);

                        //if (AutoSetProtityS.Any(x => x.Name == propinfo.Name))
                        //{
                        #region 自动设置

                        if (propinfo.Name == "LastEditUserId")
                        {
                            propinfo.SetValue(_entity, CurrentUserId);
                        }
                        if (propinfo.Name == "LastEditUserName")
                        {
                            propinfo.SetValue(_entity, CurrentUserName);
                        }
                        if (propinfo.Name == "LastEditDateTime")
                        {
                            propinfo.SetValue(_entity, DateTime.Now);
                        }

                        if (propinfo.Name == "EDITID")
                        {
                            string EDITID = "";
                            var objEDITID = propertityVal;
                            if (objEDITID != null)
                            {
                                EDITID = objEDITID.ToString();
                            }
                            if (string.IsNullOrEmpty(EDITID))
                                propinfo.SetValue(_entity, CurrentUserId);
                            else
                            {
                                if (!NoAutoSetUserName.Any(x => x.ToLower() == EDITID.ToLower()))
                                {
                                    if (!string.IsNullOrEmpty(CurrentUserId))
                                        propinfo.SetValue(_entity, CurrentUserId);
                                }
                            }
                        }
                        if (propinfo.Name == "EDITWHO")
                        {
                            string EDITWHO = "";
                            var objEDITWHO = propertityVal;
                            if (objEDITWHO != null)
                            {
                                EDITWHO = objEDITWHO.ToString();
                            }
                            if (string.IsNullOrEmpty(EDITWHO))
                            {
                                propinfo.SetValue(_entity, CurrentUserName);
                            }
                            else
                            {
                                if (!NoAutoSetUserName.Any(x => x.ToLower() == EDITWHO.ToLower()))
                                {
                                    if (!string.IsNullOrEmpty(CurrentUserName))
                                        propinfo.SetValue(_entity, CurrentUserName);
                                }
                            }
                        }
                        if (propinfo.Name == "EDITTS")
                        {
                            propinfo.SetValue(_entity, DateTime.Now);
                        }

                        #endregion
                        //}
                    }

                    #region 全文检索&Redis缓存

                    try
                    {
                        LuneceManager.IndexManager.OIndexManager.LuneceModify(_entity, _entityProptys);
                    }
                    catch (Exception ex)
                    {
                        SQLDALHelper.WriteLogHelper.WriteLog("全文检索错误（LuneceModify）：" + TMI.Web.Extensions.Common.GetExceptionMsg(ex), "LuneceError", true);
                    }

                    #endregion
                }

                #endregion

                #region 获取所有删除的数据

                var _entitiesDeleted = Entitys.Where(_e => _e.State == EntityState.Deleted).ToList();
                foreach (var entityitem in _entitiesChanged)
                {
                    var _entity = entityitem.Entity;
                    var _entityProptys = _entity.GetType().GetProperties();
                    try
                    {
                        if (IsWriteDataToLunece)
                            LuneceManager.IndexManager.OIndexManager.LuneceDelete(_entity, _entityProptys);
                    }
                    catch (Exception ex)
                    {
                        SQLDALHelper.WriteLogHelper.WriteLog("全文检索错误（LuneceDelete）：" + TMI.Web.Extensions.Common.GetExceptionMsg(ex), "LuneceError", true);
                    }
                }
                try
                {
                    RedisCacheManager.RedisManager.ORedisManager.AnalysisEntity(_entitiesDeleted.Select(x => x.Entity).ToList(), RedisType.Delete);
                }
                catch
                {

                }

                #endregion

                ret = base.SaveChanges();

                #region  Redis缓存

                try
                {
                    if (IsWriteDataToRedis)
                        RedisCacheManager.RedisManager.ORedisManager.AnalysisEntity(ArrRedisInsertUpdateObj, RedisType.Insert_Update);
                }
                catch
                {

                }

                #endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = TMI.Web.Extensions.Common.GetExceptionMsg(ex);
                SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg, "SaveChanges", true);
                throw ex;
            }
            return ret;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Oracle 表所有者，（SQL 改成 dbo(默认)，也可删除此设置）
            modelBuilder.HasDefaultSchema(DbSchema);
            //将数据列转换成大写
            modelBuilder.Properties().Configure(x => x.HasColumnName(GetColumnName(x.ClrPropertyInfo)));
            //将TableName转大写,TableName 指定的除外
            modelBuilder.Types().Configure(c => c.ToTable(GetTableName(c.ClrType)));
            //设置联合主键
            //modelBuilder.Entity<ModelName>().HasKey(t => new { t.VERDORCODE, t.MFLAG });

            #region AirOut 特殊数据格式设置

            #region 设置删除数据关系

            //modelBuilder.Entity<EMS_HEAD>()
            //    .HasMany(e => e.EMS_EXGS)
            //    .WithRequired(e => e.EMS_HEAD)
            //    .WillCascadeOnDelete(false);

            #endregion

            #region 设置 string 是否可为空 也可以在 Models里设置[Required]

            //modelBuilder.Entity<VIP_CDAReturnXML>().Property(e => e.MQLable).IsRequired();

            #endregion

            #region 已经 手动设置过 数据位数

            //统一设置Decimal 长度（数据库实际位数可以缩短）
            modelBuilder.Properties<decimal>().Configure(c => c.HasPrecision(28, 9));
            //手动特殊话设置
            //modelBuilder.Entity<EMS_EXG>()//类
            //    .Property(e => e.FACTOR_1)//字段
            //    .HasPrecision(18, 9);//长度

            #endregion

            #endregion

            //关闭一对多的级联删除。
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //关闭多对多的级联删除。
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //移除EF的表名公约  
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //移除对MetaData表的查询验证，要不然每次都要访问Metadata这个表
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();

            ////数据库不存在时重新创建数据库
            //Database.SetInitializer<WebdbContext>(new CreateDatabaseIfNotExists<WebdbContext>());
            ////每次启动应用程序时创建数据库
            //Database.SetInitializer<WebdbContext>(new DropCreateDatabaseAlways<WebdbContext>());
            ////模型更改时重新创建数据库
            //Database.SetInitializer<WebdbContext>(new DropCreateDatabaseIfModelChanges<WebdbContext>());
            //从不创建数据库
            Database.SetInitializer<WebdbContext>(null);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetTableName(Type type)
        {
            try
            {
                TableAttribute[] tableAttributes = (TableAttribute[])type.GetCustomAttributes(typeof(TableAttribute), false);

                if (!tableAttributes.Any())
                {
                    //var pluralizationService = DbConfiguration.DependencyResolver.GetService<IPluralizationService>();
                    //var result = pluralizationService.Pluralize(type.Name);
                    //var result = Regex.Replace(type.Name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);
                    var result = StringUtil.ToPlural(type.Name);

                    return result.ToUpper();
                }
                else
                {
                    var tableattr = tableAttributes.FirstOrDefault();
                    if (tableattr != null)
                    {
                        if (!string.IsNullOrEmpty(tableattr.Name))
                            return tableattr.Name;
                        else
                            return StringUtil.ToPlural(type.Name).ToUpper();
                    }
                    else
                        return StringUtil.ToPlural(type.Name).ToUpper();
                }
            }
            catch (Exception)
            {
                return StringUtil.ToPlural(type.Name).ToUpper();
            }
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetColumnName(PropertyInfo pri)
        {
            try
            {
                ColumnAttribute[] columnAttribute = (ColumnAttribute[])pri.GetCustomAttributes(typeof(ColumnAttribute), false);

                if (columnAttribute != null)
                {
                    if (!columnAttribute.Any())
                    {
                        //var pluralizationService = DbConfiguration.DependencyResolver.GetService<IPluralizationService>();
                        //var result = pluralizationService.Pluralize(type.Name);
                        //var result = Regex.Replace(type.Name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);
                        var result = pri.Name;

                        return result.ToUpper();
                    }
                    else
                    {
                        var columnattr = columnAttribute.FirstOrDefault();
                        if (columnattr != null)
                        {
                            if (!string.IsNullOrEmpty(columnattr.Name))
                                return columnattr.Name;
                            else
                                return pri.Name.ToUpper();
                        }
                        else
                            return pri.Name.ToUpper();
                    }
                }
                else
                    return pri.Name.ToUpper();
            }
            catch (Exception)
            {
                return StringUtil.ToPlural(pri.Name).ToUpper();
            }
        }

        /// <summary>
        /// 添加时需要设定的字段名
        /// 英文单词 单复数转换
        /// </summary>
        public static class StringUtil
        {
            /// <summary>
            /// 单词变成单数形式
            /// </summary>
            /// <param name="word"></param>
            /// <returns></returns>
            public static string ToSingular(string word)
            {
                Regex plural1 = new Regex("(?<keep>[^aeiou])ies$");
                Regex plural2 = new Regex("(?<keep>[aeiou]y)s$");
                Regex plural3 = new Regex("(?<keep>[sxzh])es$");
                Regex plural4 = new Regex("(?<keep>[^sxzhyu])s$");

                if (plural1.IsMatch(word))
                    return plural1.Replace(word, "${keep}y");
                else if (plural2.IsMatch(word))
                    return plural2.Replace(word, "${keep}");
                else if (plural3.IsMatch(word))
                    return plural3.Replace(word, "${keep}");
                else if (plural4.IsMatch(word))
                    return plural4.Replace(word, "${keep}");

                return word;
            }

            /// <summary>
            /// 单词变成复数形式
            /// </summary>
            /// <param name="word"></param>
            /// <returns></returns>
            public static string ToPlural(string word)
            {
                Regex plural1 = new Regex("(?<keep>[^aeiou])y$");
                Regex plural2 = new Regex("(?<keep>[aeiou]y)$");
                Regex plural3 = new Regex("(?<keep>[sxzh])$");
                Regex plural4 = new Regex("(?<keep>[^sxzhy])$");

                if (plural1.IsMatch(word))
                    return plural1.Replace(word, "${keep}ies");
                else if (plural2.IsMatch(word))
                    return plural2.Replace(word, "${keep}s");
                else if (plural3.IsMatch(word))
                    return plural3.Replace(word, "${keep}es");
                else if (plural4.IsMatch(word))
                    return plural4.Replace(word, "${keep}s");

                return word;
            }
        }

        public System.Data.Entity.DbSet<TMI.Web.Models.Order> Orders { get; set; }

    }

    //配置公约
    public class EFConfiguration : DbConfiguration
    {
        public EFConfiguration()
        {
            //AddInterceptor(new StringTrimmerInterceptor());
            ////或者注册在Global.asax中的Application_Start 
            //DbInterception.Add(new EFIntercepterLogging());
        }
    }

    #region 使 EntityFrameWork 自动 去除字符串 末尾 空格

    /// <summary>
    /// 设置 SQL 条件 "123 "与"123"比对的问题
    /// 使 EntityFrameWork 自动 去除字符串 末尾 空格
    /// SQL 默认去除 最末尾的空格 "123 " 等价于 "123"
    /// </summary>
    public class StringTrimmerInterceptor : IDbCommandTreeInterceptor
    {
        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace == DataSpace.SSpace)
            {
                var queryCommand = interceptionContext.Result as DbQueryCommandTree;
                if (queryCommand != null)
                {
                    var newQuery = queryCommand.Query.Accept(new StringTrimmerQueryVisitor());
                    interceptionContext.Result = new DbQueryCommandTree(
                        queryCommand.MetadataWorkspace,
                        queryCommand.DataSpace,
                        newQuery);
                }
            }
        }

        private class StringTrimmerQueryVisitor : DefaultExpressionVisitor
        {
            private static readonly string[] _typesToTrim = { "nvarchar", "varchar", "char", "nchar" };

            public override DbExpression Visit(DbNewInstanceExpression expression)
            {
                var arguments = expression.Arguments.Select(a =>
                {
                    var propertyArg = a as DbPropertyExpression;
                    if (propertyArg != null && _typesToTrim.Contains(propertyArg.Property.TypeUsage.EdmType.Name))
                    {
                        return EdmFunctions.Trim(a);
                    }

                    return a;
                });

                return DbExpressionBuilder.New(expression.ResultType, arguments);
            }
        }
    }

    #endregion

    #region 记录 EntityFrameWork 生成的SQL 以及 性能

    /// <summary>
    /// 记录 EntityFrameWork 生成的SQL 以及 性能
    /// 使用EntityFramework6.1的DbCommandInterceptor拦截生成的SQL语句
    /// </summary>
    public class EFIntercepterLogging : DbCommandInterceptor
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public override void ScalarExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            base.ScalarExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void ScalarExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            _stopwatch.Stop();
            if (interceptionContext.Exception != null)
            {
                Trace.TraceError("Exception:{1} \r\n --> Error executing command: {0}", command.CommandText, interceptionContext.Exception.ToString());
            }
            else
            {
                Trace.TraceInformation("\r\n执行时间:{0} 毫秒\r\n-->ScalarExecuted.Command:{1}\r\n", _stopwatch.ElapsedMilliseconds, command.CommandText);
            }
            base.ScalarExecuted(command, interceptionContext);
        }

        public override void NonQueryExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            base.NonQueryExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void NonQueryExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            _stopwatch.Stop();
            if (interceptionContext.Exception != null)
            {
                Trace.TraceError("Exception:{1} \r\n --> Error executing command:\r\n {0}", command.CommandText, interceptionContext.Exception.ToString());
            }
            else
            {
                Trace.TraceInformation("\r\n执行时间:{0} 毫秒\r\n-->NonQueryExecuted.Command:\r\n{1}", _stopwatch.ElapsedMilliseconds, command.CommandText);
            }
            base.NonQueryExecuted(command, interceptionContext);
        }

        public override void ReaderExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            base.ReaderExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void ReaderExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            _stopwatch.Stop();
            if (interceptionContext.Exception != null)
            {
                Trace.TraceError("Exception:{1} \r\n --> Error executing command:\r\n {0}", command.CommandText, interceptionContext.Exception.ToString());
            }
            else
            {
                Trace.TraceInformation("\r\n执行时间:{0} 毫秒 \r\n -->ReaderExecuted.Command:\r\n{1}", _stopwatch.ElapsedMilliseconds, command.CommandText);
            }
            base.ReaderExecuted(command, interceptionContext);
        }
    }

    #endregion
}