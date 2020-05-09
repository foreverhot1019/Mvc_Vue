using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CCBWebApi.Models;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text;
using CCBWebApi.Extensions;

namespace CCBWebApi.Controllers
{
    [RoutePrefix("Loan")]
    public class apiLoanController : ApiController
    {
        /// <summary>
        /// dbContext
        /// </summary>
        private ApplicationDbContext dbContext;

        /// <summary>
        /// 异步写日志
        /// </summary>
        private bool AsyncWriteLog = false;

        /// <summary>
        /// Des对称加密算法
        /// </summary>
        private DesCryptoHelper ODesCryptoHelper { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public apiLoanController()
        {
            bool? _AsyncWriteLog = CacheHelper.Get_SetBoolConfAppSettings("/LoanController", "AsyncWriteLog");
            if (_AsyncWriteLog.HasValue && _AsyncWriteLog.Value)
                AsyncWriteLog = true;
            ODesCryptoHelper = new DesCryptoHelper();
            dbContext = new ApplicationDbContext();
        }

        /// <summary>
        /// 获取-获取应收账款信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getinfo")]
        public async Task<string> PostGetInfo([FromBody]string ENTITY_XML)
        {
            FLD1001 OEntity = new FLD1001();
            string ErrMsg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(ENTITY_XML))
                    ErrMsg = "数据不能为空";
                else
                {
                    bool isBase64 = ENTITY_XML.Substring(0, 1) != "<";//Base64Helper.IsBase64(ENTITY_XML);
                    if (isBase64)//base64解密
                        ENTITY_XML = ODesCryptoHelper.Decrypt(ENTITY_XML);
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
                    else
                        ErrMsg = "XML数据,无法转换/转换失败";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = Common.GetExceptionMsg(ex) + ":" + ex.StackTrace;
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
            string XMLStr = OEntity.toXMLStr<FLD1001>();
            string retDesXMLStr = ODesCryptoHelper.Encrypt(XMLStr);

            if (AsyncWriteLog)
                WriteLogHelper.AddMessageToRedis(XMLStr + "---DesCrypto---" + retDesXMLStr, EnumType.RedisLogMsgType.LocalLog, WriteLogHelper.RedisKeyLocalLog);
            else
                WriteLogHelper.WriteLog_Local(XMLStr + "---DesCrypto---" + retDesXMLStr, "/LoanController/getinfo");

            return retDesXMLStr;
        }

        /// <summary>
        /// 获取-获取应收账款信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getinfoD")]
        public async Task<string> PostGetInfo([FromBody]dynamic ObjDyc)
        {
            string ENTITY_XML = ObjDyc == null ? "" : ObjDyc.ENTITY_XML.ToString();
            FLD1001 OEntity = new FLD1001();
            List<LoanCompany> OLoanCompany = null;
            string ErrMsg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(ENTITY_XML))
                    ErrMsg = "数据不能为空";
                else
                {
                    var db_Context = new ApplicationDbContext();
                    bool isBase64 = ENTITY_XML.Substring(0, 1) != "<";//Base64Helper.IsBase64(ENTITY_XML);
                    if (isBase64)//base64解密
                        ENTITY_XML = ODesCryptoHelper.Decrypt(ENTITY_XML);

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
                            OLoanCompany = await GetLoanCompany(OEntity);
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
                                        Txn_ExDat = item.Txn_ExDat.ToString("yyyyMMdd"),
                                        Rfnd_AccNo = item.Rfnd_AccNo,
                                    });

                                    //#region 更新 LoanCompany为作废

                                    ////dbContext.Set<LoanCompany>().Attach(item);
                                    //var OEntry = dbContext.Entry(item);
                                    //item.Status = EnumType.StatusEnum.Disable;
                                    //OEntry.Property(x => x.Status).IsModified = true;
                                    //item.LastEditUser = User == null ? "admin" : User.Identity == null ? "admin" : User.Identity.Name;
                                    //OEntry.Property(x => x.LastEditUser).IsModified = true;
                                    //item.LastEditDate = DateTime.Now;
                                    //OEntry.Property(x => x.LastEditDate).IsModified = true;

                                    //#endregion
                                }
                                OEntity.ArrFLD1001ResData = ArrFLS1001Data.ToArray();
                                OEntity.OEntityData = null;
                            }
                            //保存数据
                            var ret = await dbContext.SaveChangesAsync();
                        }
                    }
                    else
                        ErrMsg = "XML数据,无法转换/转换失败";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = Common.GetExceptionMsg(ex) + ":" + ex.StackTrace;
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
            string XMLStr = OEntity.toXMLStr<FLD1001>();
            string retDesXMLStr = ODesCryptoHelper.Encrypt(XMLStr);

            if (AsyncWriteLog)
                WriteLogHelper.AddMessageToRedis(XMLStr + "---DesCrypto---" + retDesXMLStr, EnumType.RedisLogMsgType.LocalLog, WriteLogHelper.RedisKeyLocalLog);
            else
                WriteLogHelper.WriteLog_Local(XMLStr + "---DesCrypto---" + retDesXMLStr, "/LoanController/getinfo");

            return retDesXMLStr;
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
                    //if (!(string.IsNullOrEmpty(OEntityData.Unn_Soc_Cr_Cd) || string.IsNullOrEmpty(OEntityData.KeyCode)))
                    if (!string.IsNullOrEmpty(OEntityData.Unn_Soc_Cr_Cd))
                    {
                        //OLoanCompany = await dbContext.LoanCompany.Where(x => x.Unn_Soc_Cr_Cd == OEntityData.Unn_Soc_Cr_Cd && x.KeyCode == OEntityData.KeyCode).ToListAsync();
                        OLoanCompany = await dbContext.LoanCompany.Where(x => x.Unn_Soc_Cr_Cd == OEntityData.Unn_Soc_Cr_Cd && x.Status >= EnumType.StatusEnum.Draft).ToListAsync();
                        if (!OLoanCompany.Any() || OLoanCompany.Any(x => x.Id == default(Guid)))
                        {
                            ErrMsg = "获取贷款客户数据错误：" + OEntityData.Unn_Soc_Cr_Cd + " 找不到数据";
                            //ErrMsg = "获取贷款客户数据错误：" + OEntityData.Unn_Soc_Cr_Cd + "/" + OEntityData.KeyCode + " 找不到数据";
                        }
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
        /// 获取通知
        /// </summary>
        /// <param name="ENTITY_XML"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resultnotice")]
        public async Task<string> PostResultNotice([FromBody]string ENTITY_XML)
        {
            FLD1002 OEntity = new FLD1002();
            string ErrMsg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(ENTITY_XML))
                    ErrMsg = "数据不能为空";
                else
                {
                    bool isBase64 = ENTITY_XML.Substring(0, 1) != "<";//Base64Helper.IsBase64(ENTITY_XML);
                    if (isBase64)//base64解密
                        ENTITY_XML = ODesCryptoHelper.Decrypt(ENTITY_XML);
                    OEntity = ENTITY_XML.toTypeObj<FLD1002>();
                    if (OEntity != null)
                    {
                        #region 插入数据库日志

                        Message OMsg = new Message();
                        OMsg.AddDate = DateTime.Now;
                        OMsg.AddUser = "resultnotice";
                        OMsg.IsRequest = true;
                        OMsg.Trans_Id = OEntity.OEntityHead.Trans_Id;
                        OMsg.Trans_Code = OEntity.OEntityHead.Trans_Code;
                        OMsg.Content = ENTITY_XML;
                        if (AsyncWriteLog)
                            WriteLogHelper.AddMessageToRedis(OMsg, EnumType.RedisLogMsgType.SQLMessage, WriteLogHelper.RedisKeyMessageLog);
                        else
                            WriteLogHelper.WriteLog_Local(ENTITY_XML, "/LoanController/resultnotice");

                        #endregion

                        var tfValid = ValidateTrans_Id(OEntity.OEntityHead.Trans_Id, OEntity.OEntityHead.Trans_Code);
                        if (!tfValid)
                        {
                            OEntity.OEntityHead.return_code = "000001";//000000：成功，其他错误
                            OEntity.OEntityHead.return_msg = "流水号格式错误";
                        }
                        else
                        {
                            var OEntityData = OEntity == null ? null : OEntity.OEntityData;
                            if (OEntityData == null)
                            {
                                OEntity.OEntityHead.return_code = "000001";//000000：成功，其他错误
                            }
                            else
                            {
                                OEntity.OEntityHead.return_code = "000000";//000000：成功，其他错误
                                OEntity.OEntityHead.return_msg = "成功";

                                #region 回执信息 插入数据库

                                var OResLoan = new ResLoan();
                                //OResLoan.Id = 0;
                                OResLoan.AddDate = DateTime.Now;
                                OResLoan.AddUser = "resultnotice";
                                decimal AR_Lmt = 0;
                                decimal.TryParse(OEntityData.AR_Lmt, out AR_Lmt);
                                OResLoan.AR_Lmt = AR_Lmt;
                                OResLoan.AuditStatus = EnumType.AuditStatusEnum.draft;
                                OResLoan.CoPlf_ID = OEntityData.CoPlf_ID;
                                OResLoan.Lmt_ExDat = Common.ParseStrToDateTime(OEntityData.Lmt_ExDat);
                                OResLoan.Remark = OEntityData.remark1;
                                OResLoan.Rfnd_AccNo = OEntityData.Rfnd_AccNo;
                                OResLoan.Sgn_Cst_Nm = OEntityData.Sgn_Cst_Nm;
                                DateTime? Sign_Dt = Common.ParseStrToDateTime(OEntityData.Sign_Dt);
                                if (Sign_Dt.HasValue)
                                    OResLoan.Sign_Dt = Sign_Dt.Value;
                                else
                                    OResLoan.Sign_Dt = DateTime.MinValue;
                                OResLoan.Status = EnumType.StatusEnum.Enable;
                                OResLoan.Trans_Code = OEntity.OEntityHead.Trans_Code;
                                OResLoan.Trans_Id = OEntity.OEntityHead.Trans_Id;
                                OResLoan.Unn_Soc_Cr_Cd = OEntityData.Unn_Soc_Cr_Cd;
                                var OEntry = dbContext.Entry(OResLoan);
                                OEntry.State = EntityState.Added;

                                #endregion

                                #region 更新 LoanCompany

                                await UpdateLoanCompany(OResLoan.Rfnd_AccNo);

                                #endregion

                                var ret = await dbContext.SaveChangesAsync();
                            }
                        }
                    }
                    else
                        ErrMsg = "XML数据,无法转换/转换失败";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = Common.GetExceptionMsg(ex) + ":" + ex.StackTrace;
            }
            finally
            {
                if (!string.IsNullOrEmpty(ErrMsg))
                {
                    OEntity.OEntityHead.return_code = "000001";//000000：成功，其他错误
                    OEntity.OEntityHead.return_msg = ErrMsg;
                    OEntity.OEntityData = null;
                }
            }
            string XMLStr = OEntity.toXMLStr<FLD1002>();
            string retDesXMLStr = ODesCryptoHelper.Encrypt(XMLStr);

            if (AsyncWriteLog)
                WriteLogHelper.AddMessageToRedis(XMLStr + "---DesCrypto---" + retDesXMLStr, EnumType.RedisLogMsgType.LocalLog, WriteLogHelper.RedisKeyLocalLog);
            else
                WriteLogHelper.WriteLog_Local(XMLStr + "---DesCrypto---" + retDesXMLStr, "/LoanController/resultnotice");

            return retDesXMLStr;
        }

        /// <summary>
        /// 获取通知
        /// </summary>
        /// <param name="ENTITY_XML"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resultnoticeD")]
        public async Task<string> PostResultNotice([FromBody]dynamic ObjDyc)
        {
            string ENTITY_XML = ObjDyc == null ? "" : ObjDyc.ENTITY_XML.ToString();
            FLD1002 OEntity = new FLD1002();
            string ErrMsg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(ENTITY_XML))
                    ErrMsg = "数据不能为空";
                else
                {
                    bool isBase64 = ENTITY_XML.Substring(0, 1) != "<";//Base64Helper.IsBase64(ENTITY_XML);
                    if (isBase64)//base64解密
                        ENTITY_XML = ODesCryptoHelper.Decrypt(ENTITY_XML);
                    OEntity = ENTITY_XML.toTypeObj<FLD1002>();
                    if (OEntity != null)
                    {
                        #region 插入数据库日志

                        Message OMsg = new Message();
                        OMsg.AddDate = DateTime.Now;
                        OMsg.AddUser = "resultnotice";
                        OMsg.IsRequest = true;
                        OMsg.Trans_Id = OEntity.OEntityHead.Trans_Id;
                        OMsg.Trans_Code = OEntity.OEntityHead.Trans_Code;
                        OMsg.Content = ENTITY_XML;
                        if (AsyncWriteLog)
                            WriteLogHelper.AddMessageToRedis(OMsg, EnumType.RedisLogMsgType.SQLMessage, WriteLogHelper.RedisKeyMessageLog);
                        else
                            WriteLogHelper.WriteLog_Local(ENTITY_XML, "/LoanController/resultnotice");

                        #endregion

                        var tfValid = ValidateTrans_Id(OEntity.OEntityHead.Trans_Id, OEntity.OEntityHead.Trans_Code);
                        if (!tfValid)
                        {
                            OEntity.OEntityHead.return_code = "000001";//000000：成功，其他错误
                            OEntity.OEntityHead.return_msg = "流水号格式错误";
                        }
                        else
                        {
                            var OEntityData = OEntity == null ? null : OEntity.OEntityData;
                            if (OEntityData == null)
                            {
                                OEntity.OEntityHead.return_code = "000001";//000000：成功，其他错误
                            }
                            else
                            {
                                OEntity.OEntityHead.return_code = "000000";//000000：成功，其他错误
                                OEntity.OEntityHead.return_msg = "成功";

                                #region 回执信息 插入数据库

                                var OResLoan = new ResLoan();
                                //OResLoan.Id = 0;
                                OResLoan.AddDate = DateTime.Now;
                                OResLoan.AddUser = "resultnotice";
                                decimal AR_Lmt = 0;
                                decimal.TryParse(OEntityData.AR_Lmt, out AR_Lmt);
                                OResLoan.AR_Lmt = AR_Lmt;
                                OResLoan.AuditStatus = EnumType.AuditStatusEnum.draft;
                                OResLoan.CoPlf_ID = OEntityData.CoPlf_ID;
                                OResLoan.Lmt_ExDat = Common.ParseStrToDateTime(OEntityData.Lmt_ExDat);
                                OResLoan.Remark = OEntityData.remark1;
                                OResLoan.Rfnd_AccNo = OEntityData.Rfnd_AccNo;
                                OResLoan.Sgn_Cst_Nm = OEntityData.Sgn_Cst_Nm;
                                DateTime? Sign_Dt = Common.ParseStrToDateTime(OEntityData.Sign_Dt);
                                if (Sign_Dt.HasValue)
                                    OResLoan.Sign_Dt = Sign_Dt.Value;
                                else
                                    OResLoan.Sign_Dt = DateTime.MinValue;
                                OResLoan.Status = EnumType.StatusEnum.Enable;
                                OResLoan.Trans_Code = OEntity.OEntityHead.Trans_Code;
                                OResLoan.Trans_Id = OEntity.OEntityHead.Trans_Id;
                                OResLoan.Unn_Soc_Cr_Cd = OEntityData.Unn_Soc_Cr_Cd;
                                var OEntry = dbContext.Entry(OResLoan);
                                OEntry.State = EntityState.Added;

                                #endregion

                                #region 更新 LoanCompany

                                await UpdateLoanCompany(OResLoan.Rfnd_AccNo);

                                #endregion

                                var ret = await dbContext.SaveChangesAsync();
                            }
                        }
                    }
                    else
                        ErrMsg = "XML数据,无法转换/转换失败";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = Common.GetExceptionMsg(ex) + ":" + ex.StackTrace;
            }
            finally
            {
                if (!string.IsNullOrEmpty(ErrMsg))
                {
                    OEntity.OEntityHead.return_code = "000001";//000000：成功，其他错误
                    OEntity.OEntityHead.return_msg = ErrMsg;
                    OEntity.OEntityData = null;
                }
            }
            string XMLStr = OEntity.toXMLStr<FLD1002>();
            string retDesXMLStr = ODesCryptoHelper.Encrypt(XMLStr);

            if (AsyncWriteLog)
                WriteLogHelper.AddMessageToRedis(XMLStr + "---DesCrypto---" + retDesXMLStr, EnumType.RedisLogMsgType.LocalLog, WriteLogHelper.RedisKeyLocalLog);
            else
                WriteLogHelper.WriteLog_Local(XMLStr + "---DesCrypto---" + retDesXMLStr, "/LoanController/resultnotice");

            return retDesXMLStr;
        }

        /// <summary>
        /// 更新 LoanCompany 为作废
        /// </summary>
        /// <param name="Rfnd_AccNo">货款账号</param>
        /// <returns></returns>
        private async Task UpdateLoanCompany(string Rfnd_AccNo)
        {
            var ArrLoanCompany = await dbContext.LoanCompany.Where(x => x.Rfnd_AccNo == Rfnd_AccNo && x.Status >= EnumType.StatusEnum.Draft).ToListAsync();
            if (ArrLoanCompany == null || ArrLoanCompany.Any(x => x.Id == default(Guid)))
            {
                var Err_Msg = "找不到Rfnd_AccNo:" + Rfnd_AccNo + "的LoanCompany数据集";
                if (AsyncWriteLog)
                    WriteLogHelper.AddMessageToRedis(Err_Msg, EnumType.RedisLogMsgType.LocalLog, WriteLogHelper.RedisKeyLocalLog);
                else
                    WriteLogHelper.WriteLog_Local(Err_Msg, "/LoanController/UpdateLoanCompany");
            }
            else
            {
                foreach (var item in ArrLoanCompany) 
                {
                    var O_Entry = dbContext.Entry(item);
                    item.Status = EnumType.StatusEnum.Disable;
                    O_Entry.Property(x => x.Status).IsModified = true;
                    item.LastEditUser = User == null ? "admin" : User.Identity == null ? "admin" : User.Identity.Name;
                    O_Entry.Property(x => x.LastEditUser).IsModified = true;
                    item.LastEditDate = DateTime.Now;
                    O_Entry.Property(x => x.LastEditDate).IsModified = true;
                    //dbContext.Set<LoanCompany>().Attach(OLoanCompany);
                }                                
            }
        }

        // GET: api/Loan
        public IQueryable<LoanCompany> GetLoanCompany()
        {
            return dbContext.LoanCompany;
        }

        // GET: api/Loan/5
        [ResponseType(typeof(LoanCompany))]
        public async Task<IHttpActionResult> GetLoanCompany(Guid id)
        {
            LoanCompany loanCompany = await dbContext.LoanCompany.FindAsync(id);
            if (loanCompany == null)
            {
                return NotFound();
            }

            return Ok(loanCompany);
        }

        // PUT: api/Loan/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLoanCompany(Guid id, LoanCompany loanCompany)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != loanCompany.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(loanCompany).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanCompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Loan
        [ResponseType(typeof(LoanCompany))]
        public async Task<IHttpActionResult> PostLoanCompany(LoanCompany loanCompany)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                dbContext.LoanCompany.Add(loanCompany);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                return Ok(new { Success = false, ErrMsg = ErrMsg });
            }

            return CreatedAtRoute("DefaultApi", new { id = loanCompany.Id }, loanCompany);
        }

        // DELETE: api/Loan/5
        [ResponseType(typeof(LoanCompany))]
        public async Task<IHttpActionResult> DeleteLoanCompany(Guid id)
        {
            LoanCompany loanCompany = await dbContext.LoanCompany.FindAsync(id);
            if (loanCompany == null)
            {
                return NotFound();
            }

            dbContext.LoanCompany.Remove(loanCompany);
            await dbContext.SaveChangesAsync();

            return Ok(loanCompany);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LoanCompanyExists(Guid id)
        {
            return dbContext.LoanCompany.Count(e => e.Id == id) > 0;
        }

        /// <summary>
        /// 验证流水号
        /// </summary>
        /// <param name="Trans_Id"></param>
        /// <returns></returns>
        private bool ValidateTrans_Id(string Trans_Id, string Trans_Code)
        {
            bool ret = false;
            string Trans_Id_Regx = CacheHelper.Get_SetStringConfAppSettings("/apiLoanController/ValidateTrans_Id", "Trans_Id_Regx").Replace("Trans_Code", Trans_Code);
            var Regx = new System.Text.RegularExpressions.Regex(Trans_Id_Regx);
            var rgMatch = Regx.Match(Trans_Id);
            if (rgMatch.Success)
                ret = true;
            return ret;
        }

        /// <summary>
        /// Des解密
        /// </summary>
        /// <param name="DesSettings"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DesDecrypt")]
        public string DesDecrypt(dynamic DesSettings)
        {
            string PswStr = DesSettings.Password;
            string IVStr = DesSettings.IV;
            string EnCrypStr = DesSettings.EnCrypStr;
            DesCryptoHelper ODes = new DesCryptoHelper(PswStr, IVStr);
            return ODes.Decrypt(EnCrypStr);
        }

    }

    [RoutePrefix("Auth_Code")]
    public class CodesController : ApiController
    {
        [HttpGet]
        [Route("Get_Code", Name = "Get_Code")]
        public async Task<System.Net.Http.HttpResponseMessage> Get(string code)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:62194/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "/Token", new
                {
                    client_id = "fldkdjhs",//必须是某个字段
                    client_secret = "fld_47HDu8s",//必须是某个字段
                    Code = code,//可自定义
                    grant_type = "authorization_code",//必须是authorization_code
                    redirect_uri = "http://localhost:62194/Auth_Code/Get_Code?Code=" + code,
                });
            response.EnsureSuccessStatusCode();

            return new System.Net.Http.HttpResponseMessage()
            {
                Content = new System.Net.Http.StringContent(code, Encoding.UTF8, "text/plain")
            };
        }
    }
}