using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace AirOut.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    [MetadataType(typeof(ApplicationUserMetadata))]
    public class ApplicationUser : IdentityUser
    {
        [Display(Name="部门")]
        [StringLength(50)]
        public string DepartMent { get; set; }

        [Display(Name = "用户名")]
        [StringLength(50)]
        public string UserNameDesc { get; set; }

        [Display(Name = "用户操作点")]
        [StringLength(50)]
        public string UserOperatPoint { get; set; }

        //public bool IsEnabled { get; set; }

        [Display(Name = "接单人")]
        public bool IsJDServ { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //设置为false 不然 AutoResetCache时，ApplicationUser类型转换失败
            //已在 AutoResetCache时处理，转换为真实 类2018-04-19
            //this.Configuration.ProxyCreationEnabled = false;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Oracle 表所有者，（SQL 改成 dbo(默认)，也可删除此设置）
            modelBuilder.HasDefaultSchema(DbSchema);

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    #region ApplicationUser MetadataType字段说明

    public class ApplicationUserMetadata
    {
        [Display(Name = "主键")]
        public string Id { get; set; }

        [Display(Name = "登录名"), Required(ErrorMessage = "登录名不能为空")]
        public string UserName { get; set; }

        [Display(Name = "姓名"), StringLength(50)]
        public string UserNameDesc { get; set; }

        [Display(Name = "密码",Description="Hash值")]
        public string PasswordHash { get; set; }

        [Display(Name = "邮件")]
        public string Email { get; set; }

        [Display(Name = "联系电话")]
        [Phone(ErrorMessage="不是合法的电话号码")]
        public string PhoneNumber { get; set; }

        //[Display(Name = "是否启用")]
        //public bool IsEnabled { get; set; }

        [Display(Name = "邮件确认")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "安全码")]
        public string SecurityStamp { get; set; }

        [Display(Name = "联系电话确认")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "二次因子启用")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "结束锁定时间")]
        public System.DateTime? LockoutEndDateUtc { get; set; }

        [Display(Name = "锁定启用")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "访问失败次数")]
        public int AccessFailedCount { get; set; }

        [Display(Name = "操作点")]
        public string UserOperatPoint { get; set; }
    }

    #endregion
}