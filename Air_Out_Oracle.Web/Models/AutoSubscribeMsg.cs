using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirOut.Web.Models
{
    /// <summary>
    /// 发送/推送 消息 格式
    /// </summary>
    public class AutoSubscribeMsg
    {
        public string MSG { get; set; }

        public string reciver { get; set; }

        public string sender { get; set; }

        /// <summary>
        /// 延迟推送
        /// </summary>
        public int delay { get; set; }

        /// <summary>
        /// 推送类型
        /// 为all时，推送所有
        /// </summary>
        public string type { get; set; }
    }
}