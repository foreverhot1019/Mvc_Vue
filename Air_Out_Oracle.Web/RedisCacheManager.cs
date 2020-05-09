using AirOut.Web.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AirOut.Web
{
    public class RedisCacheManager
    {
        public class RedisManager
        {
            public static readonly RedisManager ORedisManager = new RedisManager();
            //去除Json无限循环回路
            Newtonsoft.Json.JsonSerializerSettings OJsonSrlizerSettingsNoLoop = new Newtonsoft.Json.JsonSerializerSettings();

            #region Redis 缓存

            /// <summary>
            /// 锁
            /// </summary>
            public static ReaderWriterLock ORWLocker = new ReaderWriterLock();

            //插入Redis缓存
            private static string RedisManagerWCFUrl { get; set; }
            //IRedisManagerWCF ProxyRedis { get; set; }

            /// <summary>
            /// 缓存到Redis的表的队列
            /// </summary>
            private static Queue<RedisHashQueue> RedisQueue = new Queue<RedisHashQueue>();

            /// <summary>
            /// 需要缓存到Redis的表
            /// </summary>
            private static List<string> RedisCacheClass = new List<string> { 
                "ASN_ORDER_DETAIL",
                "ASN_ORDER_HEAD",
                "FOO_FWOREFRENCE",
                "FOO_ORDERS",
                "FWO_ORDERS",
                "SO_ORDER_DETAIL_PICKING",
                "SO_ORDER_DETAIL_SHIPMENT",
                "SO_ORDER_HEAD"
            };

            #endregion

            /// <summary>
            /// 错误日志 文件夹名称
            /// </summary>
            private string RedisErrorDir
            {
                get
                {
                    var LEDir = System.Configuration.ConfigurationManager.AppSettings["RedisErrorDir"] ?? "Redis";
                    return LEDir;
                }
            }

            private RedisManager()
            {
                RedisManagerWCFUrl = System.Configuration.ConfigurationManager.AppSettings["RedisManagerWCF"] ?? "";
                //ProxyRedis = WCFHelper.WCFInvokeFactory.CreateServiceByUrl<IRedisManagerWCF>(RedisManagerWCFUrl);

                #region 去除 序列化Json无限循环回路

                //循环引用时 忽略
                OJsonSrlizerSettingsNoLoop.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                ////定义序列化Json的行为，使其忽略引用对象（导航属性,主键 会被转义-$主键）
                //OJsonSrlizerSettingsNoLoop.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;

                #endregion
            }

            /// <summary>
            /// 分析Entity_s
            /// </summary>
            /// <param name="ArrEntity"></param>
            /// <param name="ORedisType"></param>
            public void AnalysisEntity(List<object> ArrEntity, RedisType ORedisType)
            {
                List<Tuple<string, object>> ArrTuple = ArrEntity.Select(x => new Tuple<string, object>(x.GetType().Name.ToUpper(), x)).ToList();
                var DistinctTableName = ArrTuple.Select(x => x.Item1).Distinct();
                foreach (var TableName in DistinctTableName)
                {
                    if (!RedisCacheClass.Any(x => x == TableName))
                        continue;
                    RedisHashQueue ORedisHashQueue = new RedisHashQueue();
                    try
                    {
                        ORedisHashQueue.HashKey = TableName;
                        var QArrTuple = ArrTuple.Where(x => x.Item1 == TableName);
                        if (QArrTuple.Any())
                        {
                            List<RedisHashKeyValue> ArrRedisHashKeyValue = new List<RedisHashKeyValue>();
                            PropertyInfo[] _entityProptys = QArrTuple.FirstOrDefault().Item2.GetType().GetProperties();
                            ReaderWriterLock ORW_Locker = new ReaderWriterLock();
                            int MaxTask = 3;
                            int MaxTake = 1000;
                            List<Task> ArrTask = new List<Task>();
                            int ArrNum = QArrTuple.Count();
                            MaxTake = ArrNum % MaxTask > 0 ? (ArrNum / MaxTask + 1) : MaxTask;
                            int Skip = 0;
                            while (Skip < ArrNum)
                            {
                                var NewQArrTuple = QArrTuple.Skip(Skip).Take(MaxTake);
                                Skip += NewQArrTuple.Count();
                                ArrTask.Add(Task.Run(() =>
                                {
                                    foreach (var OTuple in NewQArrTuple)
                                    {
                                        if (OTuple.Item2 != null)
                                        {
                                            RedisHashKeyValue ORedisHashKeyValue = new RedisHashKeyValue();
                                            ORedisHashKeyValue.HashKeyName = Common.GetKeyValue(OTuple.Item2, _entityProptys);
                                            ORedisHashKeyValue.HashKeyValue = Newtonsoft.Json.JsonConvert.SerializeObject(OTuple.Item2, OJsonSrlizerSettingsNoLoop);
                                            if (ORedisType == RedisType.Delete)
                                                ORedisHashKeyValue.IsDelt = true;
                                            if (!string.IsNullOrEmpty(ORedisHashKeyValue.HashKeyName))
                                            {
                                                ORW_Locker.AcquireWriterLock(100);
                                                ArrRedisHashKeyValue.Add(ORedisHashKeyValue);
                                                ORW_Locker.ReleaseWriterLock();
                                            }
                                        }
                                    }
                                    //try
                                    //{
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    //
                                    //}
                                }));
                            }
                            Task.WaitAll(ArrTask.ToArray());
                            ORedisHashQueue.ArrRedisHashKeyValue = ArrRedisHashKeyValue;
                        }
                        if (!string.IsNullOrEmpty(ORedisHashQueue.HashKey) && !(ORedisHashQueue.ArrRedisHashKeyValue == null || ORedisHashQueue.ArrRedisHashKeyValue.Any()))
                        {
                            EnqueueRedis(ORedisHashQueue);
                        }
                    }
                    catch (Exception ex)
                    {
                        string ErrMsg = Common.GetExceptionMsg(ex);
                        if (!(string.IsNullOrEmpty(ORedisHashQueue.HashKey) || ORedisHashQueue.ArrRedisHashKeyValue == null || ORedisHashQueue.ArrRedisHashKeyValue.Any()))
                            SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg + "-" + Newtonsoft.Json.JsonConvert.SerializeObject(ORedisHashQueue, OJsonSrlizerSettingsNoLoop), RedisErrorDir, true);
                        return;
                    }
                }
            }

            /// <summary>
            /// 入队列
            /// </summary>
            /// <param name="ORedisHashQueue"></param>
            private void EnqueueRedis(RedisHashQueue ORedisHashQueue)
            {
                if (ORedisHashQueue != null)
                {
                    ORWLocker.AcquireWriterLock(100);
                    RedisQueue.Enqueue(ORedisHashQueue);
                    ORWLocker.ReleaseWriterLock();
                }
            }

            public void StartNewThread()
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DeQueueToRedis));
            }

            //定义一个线程 将队列中的数据取出来 插入索引库中
            private void DeQueueToRedis(object para)
            {
                while (true)
                {
                    if (RedisQueue.Count > 0)
                    {
                        try
                        {
                            AnalysisQueue();
                        }
                        catch (Exception ex)
                        {
                            SQLDALHelper.WriteLogHelper.WriteLog("缓存Redis错误(线程)：" + AirOut.Web.Extensions.Common.GetExceptionMsg(ex), RedisErrorDir, true);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }

            /// <summary>
            /// 分析队列
            /// </summary>
            public void AnalysisQueue()
            {
                RedisHashQueue ORedisHashQueue = null;
                ORWLocker.AcquireWriterLock(100);
                ORedisHashQueue = RedisQueue.Dequeue();
                ORWLocker.ReleaseWriterLock();
                if (ORedisHashQueue != null)
                {
                    var ArrDeltRedis = ORedisHashQueue.ArrRedisHashKeyValue.Where(x => x.IsDelt);
                    if (ArrDeltRedis.Any())
                    {
                        RedisDelt(ORedisHashQueue.HashKey, ArrDeltRedis);
                    }
                    var ArrAdd_UpdRedis = ORedisHashQueue.ArrRedisHashKeyValue.Where(x => !x.IsDelt);
                    if (ArrDeltRedis.Any())
                    {
                        RedisAdd_Update(ORedisHashQueue.HashKey, ArrAdd_UpdRedis);
                    }
                }
            }

            /// <summary>
            /// Redis删除
            /// </summary>
            /// <param name="TabName"></param>
            /// <param name="ArrRedisHashKeyValue"></param>
            public void RedisDelt(string TabName, IEnumerable<RedisHashKeyValue> ArrRedisHashKeyValue)
            {
                List<int> ArrIntHashKeyName = new List<int>();
                var ArrHashKeyName = ArrRedisHashKeyValue.Select(x => x.HashKeyName);
                foreach (var item in ArrHashKeyName)
                {
                    int outHashKeyName = 0;
                    if (int.TryParse(item, out outHashKeyName))
                        ArrIntHashKeyName.Add(outHashKeyName);
                }
                //string retErrMsg = ProxyRedis.DeltHashRedisByTabName_Key(TabName, ArrIntHashKeyName.ToArray());
            }

            /// <summary>
            /// Redis 新增或更新
            /// </summary>
            /// <param name="TabName"></param>
            /// <param name="ArrRedisHashKeyValue"></param>
            public void RedisAdd_Update(string TabName, IEnumerable<RedisHashKeyValue> ArrRedisHashKeyValue)
            {
                Dictionary<int, string> ArrIntHashKeyName = new Dictionary<int, string>();
                foreach (var item in ArrRedisHashKeyValue)
                {
                    int outHashKeyName = 0;
                    if (int.TryParse(item.HashKeyName, out outHashKeyName))
                    {
                        if (!ArrIntHashKeyName.Any(x => x.Key == outHashKeyName))
                            ArrIntHashKeyName.Add(outHashKeyName, item.HashKeyValue);
                    }
                }

                //string retErrMsg = ProxyRedis.AddHashRedisValByTabName(TabName, ArrIntHashKeyName);
            }
        }
    }

    //操作类型枚举
    public enum RedisType
    {
        Insert_Update = 0,
        Delete = 1
    }

    /// <summary>
    /// Redis-Hash队列
    /// </summary>
    public class RedisHashQueue
    {
        [Display(Name = "RedisHash", Description = "要添加到Redis-Hash的表")]
        public string HashKey { get; set; }

        [Display(Name = "RedisHashKeyValue", Description = "要添加到Redis-Hash的表的列主键和值")]
        public IEnumerable<RedisHashKeyValue> ArrRedisHashKeyValue { get; set; }
    }

    public class RedisHashKeyValue
    {
        [Display(Name = "RedisHashKeyName", Description = "要添加到Redis-Hash的表的列主键")]
        public string HashKeyName { get; set; }

        [Display(Name = "RedisHashKeyValue", Description = "要添加到Redis-Hash的表的列")]
        public string HashKeyValue { get; set; }

        [Display(Name = "删除标志", Description = "标记是否是删除Redis-Hash")]
        public bool IsDelt { get; set; }
    }

}