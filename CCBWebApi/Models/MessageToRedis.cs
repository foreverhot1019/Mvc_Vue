using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBWebApi.Models
{
    /// <summary>
    /// 信息存储到Redis
    /// </summary>
    public class MessageToRedis
    {
        public MessageToRedis()
        {

        }

        public MessageToRedis(EnumType.RedisLogMsgType _ORedisLogMsgType = EnumType.RedisLogMsgType.LocalLog)
        {
            ORedisLogMsgType = _ORedisLogMsgType;
            NowDate = DateTime.Now;
        }

        public MessageToRedis(object _OMsg, DateTime _NowDate, EnumType.RedisLogMsgType _ORedisLogMsgType)
        {
            OMsg = _OMsg;
            NowDate = _NowDate;
            ORedisLogMsgType = _ORedisLogMsgType;
        }

        /// <summary>
        /// 日志文件目录
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public object OMsg { get; set; }

        /// <summary>
        /// 日志类型
        /// SQLMessage 存到数据库
        /// LocalLog 存到本地
        /// </summary>
        public EnumType.RedisLogMsgType ORedisLogMsgType { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime NowDate { get; set; }
    }
}