using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    /// <summary>
    /// 记录发送时间 并验证是否在规定时间内可再次发送
    /// 如：发送华东接口 间隔时间
    /// </summary>
    public class RcdSendSession
    {
        //是否使用Cache
        private bool UseCache = false;
        private int HDMQSleepTime = 5;
        private string HDMQSleepTimeStr { get; set; }

        public RcdSendSession()
        {
            HDMQSleepTimeStr = System.Configuration.ConfigurationManager.AppSettings["HDMQSleepTime"] ?? "";
            if (!int.TryParse(HDMQSleepTimeStr, out HDMQSleepTime))
                HDMQSleepTime = 5;
        }

        public RcdSendSession(int _HDMQSleepTime, bool _UseCache = false)
        {
            HDMQSleepTime = _HDMQSleepTime;
            UseCache = _UseCache;
        }

        /// <summary>
        /// 是否可发送华东
        /// 默认5分钟之内不允许再次发送
        /// </summary>
        /// <param name="SessionType">键类型</param>
        /// <param name="KeyId">主键值</param>
        /// <returns></returns>
        public virtual string ValidSendSession(string SessionType, string KeyId)
        {
            string ErrMsg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(SessionType) || string.IsNullOrWhiteSpace(KeyId))
                {
                    ErrMsg = "键类型和主键值，不能为空！";
                }
                DateTime? LastOneByOneDateTime;
                if (!UseCache)
                {
                    //最后一次 发送时间
                    LastOneByOneDateTime = HttpContext.Current.Session[SessionType + KeyId] == null ? null : (DateTime?)HttpContext.Current.Session[SessionType + KeyId];
                }
                else
                    LastOneByOneDateTime = System.Web.HttpRuntime.Cache[SessionType + KeyId] == null ? null : (DateTime?)System.Web.HttpRuntime.Cache[SessionType + KeyId];

                if (LastOneByOneDateTime != null)
                {
                    if (DateTime.Compare(((DateTime)LastOneByOneDateTime).AddMinutes(HDMQSleepTime), DateTime.Now) > 0)
                    {
                        ErrMsg = HDMQSleepTime + "分钟之内，不能再次发送！";
                    }
                }

                if (string.IsNullOrWhiteSpace(ErrMsg))
                {
                    if (!UseCache)
                        HttpContext.Current.Session[SessionType + KeyId] = DateTime.Now;
                    else
                        System.Web.HttpRuntime.Cache[SessionType + KeyId] = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                AirOut.Web.Extensions.Common.WriteLogByLog4Net(ex, Extensions.Common.Log4NetMsgType.Error);
                ErrMsg = "出现错误，暂时不能发送,请复制下面文字给管理员：" + ex.StackTrace;
            }
            return ErrMsg;
        }
    }
}