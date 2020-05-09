using StackExchange.Redis;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;

namespace RedisHelp
{
    /// <summary>
    /// ConnectionMultiplexer对象管理帮助类
    /// </summary>
    public static class RedisConnectionHelp
    {
        //系统自定义Key前缀
        public static readonly string SysCustomKey = ConfigurationManager.AppSettings["redisKey"] ?? "";

        private static readonly string RedisConnectionString = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"] ?? "";
        //"127.0.0.1:6379,allowadmin=true
        //private static readonly string RedisConnectionString = ConfigurationManager.ConnectionStrings["RedisExchangeHosts"] == null ? "" : ConfigurationManager.ConnectionStrings["RedisExchangeHosts"].ConnectionString;

        private static readonly object Locker = new object();
        private static ConnectionMultiplexer _instance;
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        /// <summary>
        /// 单例获取
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            GetManager();
                        }
                    }
                } 
                
                return _instance;
            }
        }

        /// <summary>
        /// 缓存获取
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnectionMultiplexer(string connectionString)
        {
            if (!ConnectionCache.ContainsKey(connectionString))
            {
                ConnectionCache[connectionString] = GetManager(connectionString);
            }
            return ConnectionCache[connectionString];
        }

        public static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            ConnectionMultiplexer connect;

            //connectionString = connectionString ?? RedisConnectionString;
            //connect = ConnectionMultiplexer.Connect(connectionString);

            string RedisConnectionJson = System.Configuration.ConfigurationManager.AppSettings["RedisConnectionJson"] ?? "";
            List<RedisConnection> ArrRedisConn = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RedisConnection>>(RedisConnectionJson);
            //"172.20.60.186:6379,172.20.60.186:6381,172.20.70.196:6379,172.20.70.207:6379,allowAdmin=true"
            string RedisConnectionStr = string.IsNullOrEmpty(connectionString) ? RedisConnectionString : "";

            #region 连接普通的redis服务器 

            //ConnectionMultiplexer conn = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            ConfigurationOptions option = ConfigurationOptions.Parse(RedisConnectionStr);
            //ConfigurationOptions option = new ConfigurationOptions();
            //if (ArrRedisConn != null || ArrRedisConn.Any())
            //{
            //    foreach (var item in ArrRedisConn)
            //    {
            //        option.EndPoints.Add(item.IP, item.Port);
            //    }
            //}
            //else
            //{
            //    option.EndPoints.Add("172.20.60.186", 6379);
            //    option.EndPoints.Add("172.20.60.186", 6381);
            //    option.EndPoints.Add("172.20.70.196", 6379);
            //    option.EndPoints.Add("172.20.70.207", 6379);
            //    //子表
            //    option.EndPoints.Add("172.20.60.186", 6380);
            //    option.EndPoints.Add("172.20.70.196", 6380);
            //    option.EndPoints.Add("172.20.70.207", 6380);
            //}
            ////option.AllowAdmin = true;
            ////option.CommandMap = CommandMap.Create(new HashSet<string>() { "INFO", "CONFIG", "CLUSTER","PING", "ECHO", "CLIENT" });
            connect = ConnectionMultiplexer.Connect(option);

            #endregion

            #region 连接TW代理服务器

            //ConfigurationOptions twOption = new ConfigurationOptions();
            //if (ArrRedisConn != null || ArrRedisConn.Any())
            //{
            //    foreach (var item in ArrRedisConn)
            //    {
            //        twOption.EndPoints.Add(item.IP, item.Port);
            //    }
            //}
            //twOption.Proxy = Proxy.Twemproxy;//代理的类型
            //connect = ConnectionMultiplexer.Connect(twOption);

            #endregion

            #region 连接Sentinal仲裁哨兵服务器

            //ConfigurationOptions sentinelConfig = new ConfigurationOptions();
            //sentinelConfig.ServiceName = "master1";
            //sentinelConfig.EndPoints.Add("192.168.2.3", 26379);
            //sentinelConfig.EndPoints.Add("192.168.2.3", 26380);
            //sentinelConfig.TieBreaker = "";//这行在sentinel模式必须加上sentinelConfig.CommandMap = CommandMap.Sentinel
            //sentinelConfig.CommandMap = CommandMap.Sentinel;// Need Version 3.0 for the INFO command?sentinelConfig.DefaultVersion = new Version(3, 0);
            //sentinelConfig.DefaultVersion = new Version(3, 0);
            //connect = ConnectionMultiplexer.Connect(sentinelConfig);

            #endregion

            //注册如下事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;

            if (!string.IsNullOrWhiteSpace(RedisConnectionStr))
            {
                ConnectionCache[RedisConnectionStr] = connect;
                _instance = connect;
            }

            return connect;
        }

        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.WriteLine("Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine("ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine("InternalError:Message" + e.Exception.Message);
        }

        #endregion 事件
    }

    public class RedisConnection
    {
        public string IP { get; set; }

        public int Port { get; set; }
    }

    /// <summary>
    /// 根据RedisKey获取Redis-EndPoint,并返回Redis链接
    /// </summary>
    public sealed class RedisNewConnection
    {
        //加锁防止线程不安全
        private static object lockGetConnMutiplxer = new object();
        //Lazy 默认线程安全
        private static readonly Lazy<RedisNewConnection> ORedisNewConnection = new Lazy<RedisNewConnection>(() => new RedisNewConnection(), true);
        //暂存所有链接，一个IP只创建一个链接
        private static Lazy<Dictionary<string, ConnectionMultiplexer>> ODictConnMutiplxer = new Lazy<Dictionary<string, ConnectionMultiplexer>>(() =>
        {
            Dictionary<string, ConnectionMultiplexer> dictRedisConnMutipler = new Dictionary<string, ConnectionMultiplexer>();
            try
            {
                string RedisConnectionString = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"] ?? "";
                if (string.IsNullOrWhiteSpace(RedisConnectionString))
                {
                    var ArrRedisConnStr = RedisConnectionString.Split(',');
                    foreach (var item in ArrRedisConnStr)
                    {
                        if (item.IndexOf(":") > 0)
                        {
                            var ArrStrEndPoint = item.Split(':');
                            System.Net.IPAddress IPadr;
                            if (System.Net.IPAddress.TryParse(ArrStrEndPoint[0], out IPadr))//先把string类型转换成IPAddress类型
                            {
                                int port;
                                if (int.TryParse(ArrStrEndPoint[1], out port))
                                {
                                    var RedisEndPoint = new System.Net.IPEndPoint(IPadr, port);
                                    ConfigurationOptions option = new ConfigurationOptions();
                                    option.EndPoints.Add(RedisEndPoint);
                                    var OConnectionMultiplexer = ConnectionMultiplexer.Connect(option);
                                    dictRedisConnMutipler.Add(RedisEndPoint.ToString(), OConnectionMultiplexer);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return dictRedisConnMutipler;
        }, true);

        public RedisNewConnection()
        {
        }

        /// <summary>
        /// 单例模式
        /// </summary>
        public RedisNewConnection Instance
        {
            get
            {
                return ORedisNewConnection.Value;
            }
        }

        /// <summary>
        /// 获取目标EndPoint的，Redis链接
        /// </summary>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        public ConnectionMultiplexer GetEndPointConnMutipler(string EndPoint)
        {
            if (!string.IsNullOrWhiteSpace(EndPoint))
            {
                var WhereConn = ODictConnMutiplxer.Value.Where(x => x.Key == EndPoint);
                if (WhereConn.Any())
                {
                    return WhereConn.FirstOrDefault().Value;
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// 获取Redis链接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public ConnectionMultiplexer GetConnMutiplxer(RedisKey Key, ConnectionMultiplexer OConnMutiplxer, int dbNum = 0)
        {
            ConnectionMultiplexer OConnectionMultiplexer = null;
            if (OConnMutiplxer == null)
            {
                OConnMutiplxer = RedisConnectionHelp.Instance;
            }
            var RedisEndPoint = OConnMutiplxer.GetDatabase(dbNum).IdentifyEndpoint(Key);
            lock (lockGetConnMutiplxer)
            {
                try
                {
                    var WhereODictConnMutiplxer = ODictConnMutiplxer.Value.Where(x => x.Key == RedisEndPoint.ToString());
                    if (WhereODictConnMutiplxer.Any())
                    {
                        OConnectionMultiplexer = WhereODictConnMutiplxer.FirstOrDefault().Value;
                    }
                    else
                    {
                        ConfigurationOptions option = new ConfigurationOptions();
                        option.EndPoints.Add(RedisEndPoint);
                        OConnectionMultiplexer = ConnectionMultiplexer.Connect(option);
                        ODictConnMutiplxer.Value.Add(RedisEndPoint.ToString(), OConnectionMultiplexer);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return OConnectionMultiplexer;
        }
    }

}