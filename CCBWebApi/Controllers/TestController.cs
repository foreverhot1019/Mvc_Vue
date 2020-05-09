using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using CCBWebApi.Models;
using CCBWebApi.Extensions;
using System.Xml.Linq;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace CCBWebApi.Controllers
{
    public class TestController : Controller
    {
        /// <summary>
        /// dbContext
        /// </summary>
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        /// <summary>
        /// 异步写日志
        /// </summary>
        private bool AsyncWriteLog = false;

        public TestController()
        {
            bool? _AsyncWriteLog = CacheHelper.Get_SetBoolConfAppSettings("/LoanController", "AsyncWriteLog");
            if (_AsyncWriteLog.HasValue && _AsyncWriteLog.Value)
                AsyncWriteLog = true;
        }

        /// <summary>
        /// 获取-获取应收账款信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("getinfoT", Name = "getinfoT")]
        public async Task<string> Postgetinfo(string ENTITY_XML)
        {
            FLD1001 OEntity = new FLD1001();
            string ErrMsg = "";
            try
            {
                OEntity = ENTITY_XML.toTypeObj<FLD1001>();
                if (OEntity != null)
                {
                    #region 插入数据库日志

                    Message OMsg = new Message();
                    OMsg.AddDate = DateTime.Now;
                    OMsg.AddUser = "getinfo";
                    OMsg.IsRequest = true;
                    OMsg.Trans_Id = OEntity.OEntityHead.Trans_Id;
                    OMsg.Trans_Code = OEntity.OEntityHead.Trans_Code;
                    OMsg.Content = ENTITY_XML;
                    if (AsyncWriteLog)
                        WriteLogHelper.AddMessageToRedis(OMsg, EnumType.RedisLogMsgType.SQLMessage, WriteLogHelper.RedisKeyMessageLog);
                    else
                        WriteLogHelper.WriteLog_Local(ENTITY_XML, "/LoanController/getinfo");

                    #endregion

                    var tfValid = ValidateTrans_Id(OEntity.OEntityHead.Trans_Id, OEntity.OEntityHead.Trans_Code);
                    if (!tfValid)
                    {
                        OEntity.OEntityHead.return_code = "000001";//000000：成功，其他错误
                        OEntity.OEntityHead.return_msg = "流水号格式错误";
                    }
                    else
                    {
                        var OLoanCompany = await GetLoanCompany(OEntity);
                        if (OLoanCompany == null || !OLoanCompany.Any())
                        {
                            OEntity.OEntityHead.return_code = "000001";//000000：成功，其他错误
                        }
                        else
                        {
                            OEntity.OEntityHead.return_code = "000000";//000000：成功，其他错误
                            OEntity.OEntityHead.return_msg = "";
                            var FLD1001Data = OEntity.OEntityData;
                            List<FLD1001ResData> ArrFLS1001Data = new List<FLD1001ResData>();
                            foreach (var item in OLoanCompany)
                            {
                                ArrFLS1001Data.Add(new FLD1001ResData
                                {
                                    Splr_Nm = item.Splr_Nm,
                                    Unn_Soc_Cr_Cd = item.Unn_Soc_Cr_Cd,
                                    Pyr_Nm = item.Pyr_Nm,
                                    TLmt_Val = item.TLmt_Val.ToString("f2"),
                                    LoanApl_Amt = item.LoanApl_Amt,
                                    Txn_ExDat = item.Txn_ExDat.ToString("f2"),
                                    Rfnd_AccNo = item.Rfnd_AccNo,
                                });
                            }
                            OEntity.ArrFLD1001ResData = ArrFLS1001Data.ToArray();
                            OEntity.OEntityData = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.StackTrace;
            }
            finally
            {
                if (!string.IsNullOrEmpty(ErrMsg))
                {
                    OEntity.OEntityHead.return_code = "000001";//000000：成功，其他错误
                    OEntity.OEntityHead.return_msg = ErrMsg;
                    OEntity.OEntityData = null;
                    OEntity.ArrFLD1001ResData = null;
                }
            }
            return OEntity.toXMLStr<FLD1001>();
        }

        /// <summary>
        /// 获取贷款客户数据
        /// 每次只抓一个
        /// </summary>
        /// <param name="OEntity"></param>
        /// <returns></returns>
        private async Task<List<LoanCompany>> GetLoanCompany(FLD1001 OEntity)
        {
            List<LoanCompany> OLoanCompany = null;
            string LogPath = "/LoanController/GetLoanCompany";
            string ErrMsg = "";
            try
            {
                if (OEntity != null && OEntity.OEntityHead != null && OEntity.OEntityData != null)
                {
                    var OEntityData = OEntity.OEntityData;
                    if (!(string.IsNullOrEmpty(OEntityData.Unn_Soc_Cr_Cd) || string.IsNullOrEmpty(OEntityData.KeyCode)))
                    {
                        OLoanCompany = await dbContext.LoanCompany.Where(x => x.Unn_Soc_Cr_Cd == OEntityData.Unn_Soc_Cr_Cd && x.KeyCode == OEntityData.KeyCode).ToListAsync();
                        if (OLoanCompany.Any(x => x.Id != default(Guid)))
                            OLoanCompany = null;
                        else
                            ErrMsg = "获取贷款客户数据错误：" + OEntityData.Unn_Soc_Cr_Cd + "/" + OEntityData.KeyCode + " 找不到数据";
                    }
                    else
                        ErrMsg = "获取贷款客户数据错误：统一社会信用号/唯一码 不存在";
                }
                else
                    ErrMsg = "获取贷款客户数据错误：Head/Data不存在";
            }
            catch (Exception ex)
            {
                ErrMsg = Common.GetExceptionMsg(ex) + "-" + ex.StackTrace;
            }
            finally
            {
                if (!string.IsNullOrEmpty(ErrMsg))
                {
                    OEntity.OEntityHead.return_msg = ErrMsg;
                    ErrMsg += Newtonsoft.Json.JsonConvert.SerializeObject(OEntity);
                    WriteLogHelper.WriteLog_Local(ErrMsg, LogPath, true, AsyncWriteLog);
                }
            }
            return OLoanCompany;
        }

        /// <summary>
        /// 验证流水号
        /// </summary>
        /// <param name="Trans_Id"></param>
        /// <returns></returns>
        private bool ValidateTrans_Id(string Trans_Id, string Trans_Code)
        {
            bool ret = false;
            var Regx = new System.Text.RegularExpressions.Regex("^\\d{8}" + Trans_Code + "\\d{6}$");
            var rgMatch = Regx.Match(Trans_Id);
            if (rgMatch.Success)
                ret = true;
            return ret;
        }

        /// <summary>
        /// 测试新增数据
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("insertLoanCo", Name = "insertLoanCo")]
        public bool insertLoanCompany()
        {
            var OLoanCompany = new LoanCompany();
            //var OEntry = dbContext.Entry(OLoanCompany);
            //OEntry.State = EntityState.Added;
            OLoanCompany.Id = Guid.NewGuid();
            OLoanCompany.KeyCode = "KeyCode20190416001";
            OLoanCompany.CoPlf_ID = "CoPlf_ID123";
            OLoanCompany.LoanApl_Amt = 3000;
            OLoanCompany.Pyr_Nm = "Pyr_Nm";
            OLoanCompany.Rfnd_AccNo = "Rfnd_AccNo";
            OLoanCompany.Splr_Nm = "Splr_Nm";
            OLoanCompany.Status = (EnumType.StatusEnum)1;
            OLoanCompany.AuditStatus = (EnumType.AuditStatusEnum)0;
            OLoanCompany.TLmt_Val = 1000;
            OLoanCompany.Trans_Code = string.Empty;
            OLoanCompany.Trans_Id = string.Empty;
            OLoanCompany.Txn_ExDat = DateTime.Now;
            OLoanCompany.Unn_Soc_Cr_Cd = "Unn_Soc_Cr_Cd123";
            OLoanCompany.AddDate = DateTime.Now;
            OLoanCompany.AddUser = "admin";
            //OLoanCompany.LastEditDate = DateTime.Now;
            //OLoanCompany.LastEditUser = string.Empty;
            dbContext.LoanCompany.Add(OLoanCompany);
            var ret = dbContext.SaveChanges();
            return ret > 0;
        }

        /// <summary>
        /// 测试新增数据
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetLoanCo", Name = "GetLoanCo")]
        public IEnumerable<LoanCompany> GetLoanCompany()
        {
            var ArrLoanCompany = dbContext.LoanCompany.ToList();
            var Num = 0;
            foreach (var item in ArrLoanCompany)
            {
                var OEntry = dbContext.Entry(item);
                OEntry.State = EntityState.Unchanged;
                item.LoanApl_Amt += ++Num;
            }
            dbContext.SaveChanges();
            return ArrLoanCompany;
        }

        //[AllowAnonymous]
        //public string SignIn(string Name)
        //{
        //}

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            if (User!=null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var OOwinContext = Request.GetOwinContext();
                OOwinContext.Authentication.SignOut(User.Identity.AuthenticationType);
            }
            return Redirect("/Test/Login");
        }

        [AllowAnonymous]
        [Route("GetLogin", Name = "GetLogin")]
        public ActionResult Login()
        {
            return View("Login");
        }

        [AllowAnonymous]
        [Route("AppLogin", Name = "AppLogin")]
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            string accessToken = "";
            if (!ModelState.IsValid)
                return View();
            if (!User.Identity.IsAuthenticated)
            {
                var contxt = HttpContext.GetOwinContext();
                var Authentication = contxt.Authentication;
                var OUser = dbContext.Users.Where(x => x.UserName == model.UserName).FirstOrDefault();
                if(string.IsNullOrEmpty(OUser.Id))
                {
                    ModelState.AddModelError("", "帐户不存在/密码不正确");
                    return View();
                }
                else
                {
                    string PasswordHash = OUser.PasswordHash;
                    PasswordHasher OPasswordHasher = new PasswordHasher();
                    if (OPasswordHasher.VerifyHashedPassword(PasswordHash, model.Password) == PasswordVerificationResult.Failed)
                    {
                        ModelState.AddModelError("", "帐户不存在/密码不正确");
                        return View();
                    }
                }
                var props = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(1),
                };
                var ArrClaimsIdentity = new List<ClaimsIdentity>();
                var ArrAuthenticationTypes = new string[] { DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie, Microsoft.Owin.Security.OAuth.OAuthDefaults.AuthenticationType };// Authentication.GetAuthenticationTypes();
                var UserName = HttpContext.Request["UserName"] ?? "Michael";
                foreach (var AuthenticationType in ArrAuthenticationTypes)
                {
                    var identity = new ClaimsIdentity(AuthenticationType);
                    identity.AddClaim(new Claim(ClaimTypes.Name, UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserName));
                    if (Authentication.User == null || string.IsNullOrEmpty(User.Identity.Name))
                    {
                        if (AuthenticationType.IndexOf("Cookie") > 0)
                        {
                            ClaimsPrincipal OClaimsPrincipal = new ClaimsPrincipal(identity);
                            Authentication.User = OClaimsPrincipal;
                        }
                    }
                    ArrClaimsIdentity.Add(identity);
                }
                Authentication.SignIn(props, ArrClaimsIdentity.ToArray());

                //ClaimsIdentity oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
                //ClaimsIdentity cookieIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
                //ClaimsIdentity BearerIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.ExternalBearer);

                ////AuthenticationProperties properties = new AuthenticationProperties(new Dictionary<string, string> { { "userName", HttpContext.Current.Request["UserName"] } });
                //AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(HttpContext.Current.Request["UserName"]);

                //var newTicket = new AuthenticationTicket(BearerIdentity, properties);
                //var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(newTicket);

                //Authentication.SignIn(properties, BearerIdentity, cookieIdentity, oAuthIdentity);
                ////HttpContext.Current.Response.Cookies.Add(new HttpCookie("BearerIdentity", accessToken));
            }
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "/Loans/Index";
            return Redirect(returnUrl);
            //return Content("<script>window.location.href='" + returnUrl + "'</script>", "text/html");
        }

    }
}
