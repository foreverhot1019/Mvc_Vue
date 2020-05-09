using RedisHelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMI.Web
{
    /// <summary>
    /// 带检测的控制器
    /// </summary>
    public class ControllerChkRptByRedis : Controller
    {
        /// <summary>
        /// Redis 辅助类
        /// </summary>
        public RedisHelper ORedisHelper = new RedisHelper();

        /// <summary>
        /// Redis键过期时间10分钟
        /// </summary>
        public TimeSpan? ExprissTime = TimeSpan.FromMinutes(10);

        public ControllerChkRptByRedis()
        {

        }

        /// <summary>
        /// 插入Redis String类型键值
        /// </summary>
        /// <param name="Key">Redis 键</param>
        /// <param name="ObjVal">Redis 值</param>
        /// <param name="ExprissTime">过期时间间隔（分钟）</param>
        /// <returns></returns>
        private string AddRedisStringKey<T>(string Key, T ObjVal, int ExprissMinutes = 0) where T : class
        {
            string ErrMsg = "";
            try
            {
                string StrVal = "";
                var ValType = ObjVal.GetType();

                if (ValType == typeof(string) || ValType.IsValueType || !ValType.IsClass)
                {
                    StrVal = ObjVal.ToString();
                }
                else
                {
                    StrVal = Newtonsoft.Json.JsonConvert.SerializeObject(ObjVal);
                }
                if (ExprissMinutes > 0)
                    ExprissTime = TimeSpan.FromMinutes(ExprissMinutes);

                bool TF = ORedisHelper.StringSet(Key, StrVal, ExprissTime);
                if (!TF)
                    ErrMsg = "设置Redis键值，错误";
            }
            catch (Exception ex)
            {
                ErrMsg = TMI.Web.Extensions.Common.GetExceptionMsg(ex);
            }

            return ErrMsg;
        }

        /// <summary>
        /// 获取Redis String类型Key值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="Key">Redis String类型Key值</param>
        /// <returns></returns>
        private T GetRedisStringKey<T>(string Key) where T : class, new()
        {
            try
            {
                T NewObj = new T();

                var ValType = NewObj.GetType();

                string RedisKeyStr = ORedisHelper.StringGet(Key); //null;// 
                if (string.IsNullOrEmpty(RedisKeyStr))
                {
                    NewObj = null;
                }
                else
                {
                    if (ValType == typeof(string) || ValType.IsValueType || !ValType.IsClass)
                    {
                        NewObj = (T)Convert.ChangeType(RedisKeyStr,ValType);
                    }
                    else
                        NewObj = (T)Newtonsoft.Json.JsonConvert.DeserializeObject(RedisKeyStr, ValType);
                }

                return NewObj;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 检测是否重复操作
        /// </summary>
        /// <param name="Key">Redis String类型Key值</param>
        /// <param name="ExprissMinutes">过期时间间隔（分钟）</param>
        /// <returns>空：成功</returns>
        public string CheckRepeatTiQu(string Key, int ExprissMinutes = 0)
        {
            string ErrMsg = "";
            try
            {
                //if(Key.IndexOf('{')>=0)
                //    Key =Key.Replace('{','[').Replace('}',']');
                //Key = "{" + Key + "}";

                //利用自增数据检测，>1的都是 重复提交
                double OutPutNum = ORedisHelper.StringIncrement(Key);
                if (OutPutNum != 1)
                    ErrMsg = "错误：存在多次提交";
                var rt = ORedisHelper.KeyExpire(Key, TimeSpan.FromMinutes(5));
                if (!rt)
                {
                    TMI.Web.Extensions.Common.WriteLog_Local("设置" + Key + "过期时间错误！", "CheckRpt_Redis", true);
                }

                //CkRpt_Guid_Date Tp_GuidNow = new CkRpt_Guid_Date(Guid.NewGuid().ToString(), DateTime.Now);
                //ErrMsg = AddRedisStringKey(Key, Tp_GuidNow, ExprissMinutes);
                //if (string.IsNullOrEmpty(ErrMsg))
                //{
                //    CkRpt_Guid_Date retObj = GetRedisStringKey<CkRpt_Guid_Date>(Key);
                //    if (Tp_GuidNow.GuidStr != retObj.GuidStr)
                //    {
                //        ErrMsg = "错误：存在多次提交";
                //    }
                //    //else
                //    //{
                //    //    ORedisHelper.KeyDelete(Key);
                //    //}
                //}
            }
            catch (Exception ex)
            {
                ErrMsg = TMI.Web.Extensions.Common.GetExceptionMsg(ex);
            }
            return ErrMsg;
        }

        /// <summary>
        /// 虚方法 可以被重写
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult TestVirtual()
        {
            var aatype = this.GetType();
            return Json(new { Success = true });
        }

        ///// <summary>
        ///// 抽象方法 只能存在与 抽象类中 被标记的方法不能实现改方法 
        ///// 抽象类 不能 实例化
        ///// </summary>
        ///// <returns></returns>
        //public abstract ActionResult TestAbstract();

        /// <summary>
        /// 验证类
        /// </summary>
        public class CkRpt_Guid_Date
        {
            public CkRpt_Guid_Date()
            {

            }

            public CkRpt_Guid_Date(string _GuidStr, DateTime _Now)
            {
                GuidStr = _GuidStr;
                Now = _Now;
            }

            public string GuidStr { get; set; }

            public DateTime Now { get; set; }
        }
    }
}