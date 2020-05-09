using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TMI.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        //[Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "User Name")]

        public string UserName { get; set; }

        [Required]
        [Display(Name = "操作点")]
        public string OP { get; set; }

        [Required,MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        //[EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "UserNameDesc")]
        public string UserNameDesc { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]+$")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Display(Name = "IsJDServ")]
        public bool IsJDServ { get; set; }
        ////[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        ////[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [EmailAddress]
        [Display(Name = "邮件")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 长度最少 {2} 位.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 长度最少 {2} 位.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("NewPassword", ErrorMessage = "两次密码不一致")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class UserViewModel
    {
        [Key]
        public string Id { get; set; }
        [Required,StringLength(20,MinimumLength=5),Display(Name="登录名")]
        public string UserName { get; set; }
        [Required,MaxLength(10), Display(Name = "用户名")]
        public string UserNameDesc { get; set; }
        [Required, Display(Name = "邮箱")]
        public string Email { get; set; }
        [Display(Name = "电话")]
        public string PhoneNumber { get; set; }
        [Display(Name = "密码")]
        public string Password { get; set; }
        [Display(Name = "部门")]
        public string DepartMent { get; set; }
        //[Display(Name = "部门")]
        //public string EntryID { get; set; }
        [Display(Name = "锁定")]
        public bool LockoutEnabled { get; set; }
        [Display(Name = "登录失败")]
        public int AccessFailedCount { get; set; }
        [Display(Name = "锁定结束")]
        public System.DateTime? LockoutEndDateUtc { get; set; }
        
    }

    public class UserChangeViewModel
    {
        public IEnumerable<UserViewModel> inserted { get; set; }
        public IEnumerable<UserViewModel> deleted { get; set; }
        public IEnumerable<UserViewModel> updated { get; set; }
    }

    public class ApplicationUserViewModel
    {
        public IEnumerable<ApplicationUser> inserted { get; set; }
        public IEnumerable<ApplicationUser> deleted { get; set; }
        public IEnumerable<ApplicationUser> updated { get; set; }
    }
}
