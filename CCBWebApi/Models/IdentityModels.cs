using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CCBWebApi.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加个人资料数据，若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            //: base("OracleDbContext", throwIfV1Schema: false)
            : base("Name=DefaultConnection")
        {
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

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<LoanCompany> LoanCompany { get; set; }

        public DbSet<ResLoan> ResLoan { get; set; }

        public DbSet<Message> Message { get; set; }

        public DbSet<Loanapplbook> Loanapplbook { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoanCompany>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);//Guid类型主键自增长

            //Oracle 表所有者，（SQL 改成 dbo(默认)，也可删除此设置）
            modelBuilder.HasDefaultSchema(DbSchema);
            ////将数据列转换成大写
            //modelBuilder.Properties().Configure(x => x.HasColumnName(GetColumnName(x.ClrPropertyInfo)));
            ////将TableName转大写,TableName 指定的除外
            //modelBuilder.Types().Configure(c => c.ToTable(GetTableName(c.ClrType)));

            //EF 生成的SQL 语句 日志
            Database.Log = query =>
            {
                //Console.Write(query);
                System.Diagnostics.Debug.Print(query);
                //System.Diagnostics.Debug.Write(query);
            };

            //从不创建数据库
            Database.SetInitializer<ApplicationDbContext>(null);

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

        public override int SaveChanges()
        {
            var ArrEntry = this.ChangeTracker.Entries();
            return base.SaveChanges();
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
}