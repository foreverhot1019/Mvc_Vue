using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace RedisHelp
{
    /// <summary>
    /// Redis操作
    /// </summary>
    public class RedisHelper
    {
        private int DbNum { get; set; }//取哪个redisDB，集群的时候 只有一个
        private ConnectionMultiplexer _conn;
        public string CustomKey;
        public IDatabase database;

        #region 构造函数

        public RedisHelper(int dbNum = 0)
            : this(dbNum, null)
        {
        }

        public RedisHelper(int dbNum, string readWriteHosts)
        {
            DbNum = dbNum;
            if (string.IsNullOrWhiteSpace(readWriteHosts))
                _conn = RedisConnectionHelp.Instance;
            else
                _conn = RedisConnectionHelp.GetConnectionMultiplexer(readWriteHosts);
            if (_conn != null)
                database = _conn.GetDatabase(DbNum);
        }

        #endregion 构造函数

        #region String

        #region 同步方法

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return Do(db => db.StringSet(key, value, expiry));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public bool StringSet(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddSysCustomKey(p.Key), p.Value)).ToList();
            return Do(db => db.StringSet(newkeyValues.ToArray()));
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            string json = ConvertJson(obj);
            return Do(db => db.StringSet(key, json, expiry));
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            key = AddSysCustomKey(key);
            RedisValue ORVal = Do(db => db.StringGet(key));

            return ORVal.ToString();
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public RedisValue[] StringGet(List<string> listKey)
        {
            List<string> newKeys = listKey.Select(x=>AddSysCustomKey(x)).ToList();
            return Do(db => db.StringGet(ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => ConvertObj<T>(db.StringGet(key)));
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.StringIncrement(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double StringDecrement(string key, double val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.StringDecrement(key, val));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringSetAsync(key, value, expiry));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddSysCustomKey(p.Key), p.Value)).ToList();
            return await Do(db => db.StringSetAsync(newkeyValues.ToArray()));
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            string json = ConvertJson(obj);
            return await Do(db => db.StringSetAsync(key, json, expiry));
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public async Task<string> StringGetAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringGetAsync(key));
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public async Task<RedisValue[]> StringGetAsync(List<string> listKey)
        {
            List<string> newKeys = listKey.Select(AddSysCustomKey).ToList();
            return await Do(db => db.StringGetAsync(ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> StringGetAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            string result = await Do(db => db.StringGetAsync(key));
            return ConvertObj<T>(result);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> StringIncrementAsync(string key, double val = 1)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringIncrementAsync(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> StringDecrementAsync(string key, double val = 1)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringDecrementAsync(key, val));
        }

        #endregion 异步方法

        #endregion String

        #region Hash

        #region 同步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.HashExists(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                string json = ConvertJson(t);
                return db.HashSet(key, dataKey, json);
            });
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <param name="key">Hash 键</param>
        /// <param name="dataKey">Hash 键中的唯一Key值</param>
        /// <param name="JsonStr">Hash 键中的Value值</param>
        /// <param name="_When"></param>
        /// <param name="Flags"></param>
        /// <returns></returns>
        public bool HashSet(string key, string dataKey, string JsonStr, When _When = When.Always, CommandFlags Flags = CommandFlags.None)
        {
            //When.Always(默认) 无论是否存在现有值，都应该进行操作
            //When.Exists 只有当有现有值时，操作才会发生
            //When.NotExists 只有当没有现有值时才会发生该操作
            //CommandFlags.None(默认-PreferMaster)  如果可用，则应在主机上执行此操作，但是如果没有主机可用，则可以在从机上执行读操作。 这个是默认选项。
            //CommandFlags.HighPriority 此命令可能会跳转尚未写入redis流的常规优先级命令。
            //CommandFlags.FireAndForget 立即收到预期返回类型的默认值（该值不表示服务器上的任何值）。
            //CommandFlags.DemandMaster 该操作只能在主机上执行
            //CommandFlags.PreferSlave 如果可用，则应在从站执行此操作，但如果没有从站可用，则将在主站上执行此操作。 仅适用于阅读操作。
            //CommandFlags.DemandSlave 此操作只能在从站上执行。 仅适用于阅读操作。
            //CommandFlags.NoRedirect 表示由于ASK或MOVED响应，此操作不应转发到其他服务器

            key = AddSysCustomKey(key);
            return Do(db =>
            {
                return db.HashSet(key, dataKey, JsonStr, When.Always, CommandFlags.DemandMaster);
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.HashDelete(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public long HashDelete(string key, List<RedisValue> dataKeys)
        {
            key = AddSysCustomKey(key);
            //List<RedisValue> dataKeys1 = new List<RedisValue>() {"1","2"};
            return Do(db => db.HashDelete(key, dataKeys.ToArray()));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                string value = db.HashGet(key, dataKey);
                return ConvertObj<T>(value);
            });
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public string HashGet(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                string value = db.HashGet(key, dataKey);
                return value;
            });
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string HashGetAll(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                HashEntry[] ArrHash = db.HashGetAll(key);
                return "[" + string.Join(",", ArrHash.Select(x => x.Value.ToString())) + "]";
            });
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.HashIncrement(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.HashDecrement(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                RedisValue[] values = db.HashKeys(key);
                return ConvetList<T>(values);
            });
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.HashExistsAsync(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T t)
        {
            key = AddSysCustomKey(key);
            return await Do(db =>
            {
                string json = ConvertJson(t);
                return db.HashSetAsync(key, dataKey, json);
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.HashDeleteAsync(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public async Task<long> HashDeleteAsync(string key, List<RedisValue> dataKeys)
        {
            key = AddSysCustomKey(key);
            //List<RedisValue> dataKeys1 = new List<RedisValue>() {"1","2"};
            return await Do(db => db.HashDeleteAsync(key, dataKeys.ToArray()));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<T> HashGetAsync<T>(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            string value = await Do(db => db.HashGetAsync(key, dataKey));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<string> HashGetAsync(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            string value = await Do(db => db.HashGetAsync(key, dataKey));
            return value;
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> HashGetAllAsync(string key)
        {
            key = AddSysCustomKey(key);
            HashEntry[] ArrHash = await Do(db => db.HashGetAllAsync(key));
            string value = "[" + string.Join(",", ArrHash.Select(x => x.Value.ToString())) + "]";
            return value;
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> HashIncrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.HashIncrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> HashDecrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.HashDecrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> HashKeysAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            RedisValue[] values = await Do(db => db.HashKeysAsync(key));
            return ConvetList<T>(values);
        }

        #endregion 异步方法

        #endregion Hash

        #region List

        #region 同步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRemove<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            Do(db => db.ListRemove(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis =>
            {
                var values = redis.ListRange(key);
                return ConvetList<T>(values);
            });
        }

        /// <summary>
        /// 入队
        /// 加在尾部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRightPush<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            Do(db => db.ListRightPush(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出队
        /// 取出最后一个数据，并删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                var value = db.ListRightPop(key);
                return ConvertObj<T>(value);
            });
        }

        /// <summary>
        /// 入栈
        /// 加在最前面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListLeftPush<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            Do(db => db.ListLeftPush(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出栈
        /// 取出最前面的数据，并删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                var value = db.ListLeftPop(key);
                return ConvertObj<T>(value);
            });
        }

        /// <summary>
        /// 获取数据
        /// 根据数据排序的位置
        /// </summary>
        /// <param name="key">List键</param>
        /// <param name="start">开始位置0，负的为 从最往前</param>
        /// <param name="stop">结束位置 -1为最后的位置</param>
        public List<T> ListRange<T>(string key, int start = 0, int stop = -1)
        {
            key = AddSysCustomKey(key);
            var ArrRedisValue = Do(db => db.ListRange(key, start, stop));
            return ConvetList<T>(ArrRedisValue);
        }

        /// <summary>
        /// 取出数据
        /// 根据数据排序的位置
        /// </summary>
        /// <param name="key">List键</param>
        /// <param name="start">开始位置0</param>
        /// <param name="stop">结束位置 -1为最后的位置</param>
        public bool ListTrim(string key, int start = 0, int stop = -1)
        {
            try
            {
                key = AddSysCustomKey(key);
                var db = _conn.GetDatabase(DbNum);
                db.ListTrim(key, start, stop);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取前几条数据
        /// </summary>
        /// <param name="key">List键</param>
        /// <param name="Num">前Num条数据</param>
        public List<T> ListRange<T>(string key, int Num = 10)
        {
            int start = 0;
            return ListRange<T>(key, start, Num);
        }

        /// <summary>
        /// 出队前Num条数据
        /// </summary>
        /// <param name="key">List键</param>
        /// <param name="Num">开始位置0，且必须为正数</param>
        public bool ListTrim(string key, int Num = 1)
        {
            try
            {
                if (Num > 0)
                    return ListTrim(key, Num, -1);
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.ListLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRemoveAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.ListRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var values = await Do(redis => redis.ListRangeAsync(key));
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRightPushAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.ListRightPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var value = await Do(db => db.ListRightPopAsync(key));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListLeftPushAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.ListLeftPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var value = await Do(db => db.ListLeftPopAsync(key));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 获取数据
        /// 根据数据排序的位置
        /// </summary>
        /// <param name="key">List键</param>
        /// <param name="start">开始位置0，负的为 从最往前</param>
        /// <param name="stop">结束位置 -1为最后的位置</param>
        public async Task<List<T>> ListRangeAsync<T>(string key, int start = 0, int stop = -1)
        {
            key = AddSysCustomKey(key);
            var ArrRedisValue = await Do(db => db.ListRangeAsync(key, start, stop));
            return ConvetList<T>(ArrRedisValue);
        }

        /// <summary>
        /// 取出数据
        /// 根据数据排序的位置
        /// </summary>
        /// <param name="key">List键</param>
        /// <param name="start">开始位置0</param>
        /// <param name="stop">结束位置 -1为最后的位置</param>
        public async Task<bool> ListTrimAsync(string key, int start = 0, int stop = -1)
        {
            try
            {
                key = AddSysCustomKey(key);
                var db = _conn.GetDatabase(DbNum);
                await db.ListTrimAsync(key, start, stop);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取前几条数据
        /// </summary>
        /// <param name="key">List键</param>
        /// <param name="Num">前Num条数据</param>
        public List<T> ListRangeAsync<T>(string key, int Num = 10)
        {
            int start = 0;
            return ListRangeAsync<T>(key, start, Num).Result;
        }

        /// <summary>
        /// 出队前Num条数据
        /// </summary>
        /// <param name="key">List键</param>
        /// <param name="Num">开始位置0，且必须为正数</param>
        public bool ListTrimAsync(string key, int Num = 1)
        {
            try
            {
                if (Num > 0)
                    return ListTrimAsync(key, Num, -1).Result;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.ListLengthAsync(key));
        }

        #endregion 异步方法

        #endregion List

        #region Set 集合

        #region 同步方法

        /// <summary>
        /// 添加 Set集合元素
        /// </summary>
        /// <typeparam name="T">类型（转换成 Json）</typeparam>
        /// <param name="key">Set集合键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SetAdd<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SetAdd(key, ConvertJson<T>(value)));
        }

        /// <summary>
        /// 获取Set集合，元素数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SetLength(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SetLength(key));
        }

        /// <summary>
        /// 判断Set集合 是否存在元素T。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetContains<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SetContains(key, ConvertJson<T>(value)));
        }

        /// <summary>
        /// 返回Set集合中的所有成员。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SetMembers<T>(string key)
        {
            key = AddSysCustomKey(key);
            var values = Do(redis => redis.SetMembers(key));
            if (values == null || !values.Any())
                return null;
            else
                return ConvetList<T>(values);
        }

        /// <summary>
        /// 移除Set集合中的一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="ArrValues"></param>
        /// <returns></returns>
        public bool SetRemove<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SetRemove(key, ConvertJson<T>(value)));
        }

        /// <summary>
        /// 移除Set集合中的一个或多个元素，不存在的 member 元素会被忽略。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="ArrValues"></param>
        /// <returns></returns>
        public long SetRemove<T>(string key, List<T> ArrValues)
        {
            key = AddSysCustomKey(key);
            var ArrRedisValue = ConvertRedisValues(ArrValues);
            return Do(redis => redis.SetRemove(key, ArrRedisValue));
        }

        /// <summary>
        /// 移除并返回集合中的一个随机元素。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T SetPop<T>(string key)
        {
            key = AddSysCustomKey(key);
            var redisval = Do(redis => redis.SetPop(key));
            if (redisval.IsNullOrEmpty)
                return default(T);
            else
                return ConvertObj<T>(redisval);
        }

        #endregion 同步方法

        #region 异步方法

        public async Task<bool> SetAddAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SetAddAsync(key, ConvertJson<T>(value)));
        }

        /// <summary>
        /// 获取Set集合，元素数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SetLengthAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SetLengthAsync(key));
        }

        /// <summary>
        /// 判断Set集合 是否存在元素T。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SetContainsAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SetContainsAsync(key, ConvertJson<T>(value)));
        }

        /// <summary>
        /// 返回Set集合中的所有成员。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> SetMembersAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var values = await Do(redis => redis.SetMembersAsync(key));
            if (values == null || !values.Any())
                return null;
            else
                return ConvetList<T>(values);
        }

        /// <summary>
        /// 移除Set集合中的一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="ArrValues"></param>
        /// <returns></returns>
        public async Task<bool> SetRemoveAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SetRemoveAsync(key, ConvertJson<T>(value)));
        }

        /// <summary>
        /// 移除Set集合中的一个或多个元素，不存在的 member 元素会被忽略。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="ArrValues"></param>
        /// <returns></returns>
        public async Task<long> SetRemoveAsync<T>(string key, List<T> ArrValues)
        {
            key = AddSysCustomKey(key);
            var ArrRedisValue = ConvertRedisValues(ArrValues);
            return await Do(redis => redis.SetRemoveAsync(key, ArrRedisValue));
        }

        /// <summary>
        /// 移除并返回集合中的一个随机元素。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> SetPopAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var redisval = await Do(redis => redis.SetPopAsync(key));
            if (redisval.IsNullOrEmpty)
                return default(T);
            else
                return ConvertObj<T>(redisval);
        }

        #endregion 异步方法

        #endregion Set 集合

        #region SortedSet 有序集合

        #region 同步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SortedSetAdd(key, ConvertJson<T>(value), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SortedSetRemove<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SortedSetRemove(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis =>
            {
                var values = redis.SortedSetRangeByRank(key);
                return ConvetList<T>(values);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SortedSetLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SortedSetAddAsync(key, ConvertJson<T>(value), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SortedSetRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var values = await Do(redis => redis.SortedSetRangeByRankAsync(key));
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SortedSetLengthAsync(key));
        }

        #endregion 异步方法

        #endregion SortedSet 有序集合

        #region key

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.KeyDelete(key));
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public long KeyDelete(List<string> keys)
        {
            List<string> newKeys = keys.Select(AddSysCustomKey).ToList();
            return Do(db => db.KeyDelete(ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.KeyExists(key));
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.KeyRename(key, newKey));
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return Do(db => db.KeyExpire(key, expiry));
        }

        /// <summary>
        /// 获取键应该被放在那台服务器上（集群模式时使用）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public System.Net.EndPoint GetRedisEndPoint(string key, bool IsAddSysCustomKey = true)
        {
            key = IsAddSysCustomKey ? AddSysCustomKey(key) : key;
            try
            {
                var newDb = _conn.GetDatabase(DbNum);
                return newDb.IdentifyEndpoint(key);
                //return Do(db => db.IdentifyEndpoint(key));
            }
            catch
            {
                return null;
            }
        }

        #endregion key

        #region 发布订阅

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber sub = _conn.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long Publish<T>(string channel, T msg)
        {
            ISubscriber sub = _conn.GetSubscriber();
            return sub.Publish(channel, ConvertJson(msg));
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = _conn.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            ISubscriber sub = _conn.GetSubscriber();
            sub.UnsubscribeAll();
        }

        #endregion 发布订阅

        #region 其他

        public ITransaction CreateTransaction()
        {
            return GetDatabase().CreateTransaction();
        }

        public IDatabase GetDatabase()
        {
            return _conn.GetDatabase(DbNum);
        }

        public IServer GetServer(string hostAndPort)
        {
            return _conn.GetServer(hostAndPort);
        }

        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        public void SetSysCustomKey(string customKey)
        {
            CustomKey = customKey;
        }

        #endregion 其他

        #region 辅助方法

        private string AddSysCustomKey(string oldKey)
        {
            var prefixKey = CustomKey ?? RedisConnectionHelp.SysCustomKey;
            var NewKey = prefixKey + oldKey;

            ////更新服务目标
            //updateDatabase(NewKey);

            return NewKey;
        }

        private T Do<T>(Func<IDatabase, T> func)
        {
            ////Random ran = new Random();
            ////int connNum = Convert.ToInt32(_conn.OperationCount);
            ////int _DbNum = ran.Next(1, connNum);
            //var database = _conn.GetDatabase(DbNum);
            //return func(database);

            try
            {
                if (database == null || !_conn.IsConnected)
                {
                    if (!_conn.IsConnected)
                    {
                        var new_conn = RedisConnectionHelp.Instance;
                        database = new_conn.GetDatabase(DbNum);
                    }
                    else if (database == null)
                        database = _conn.GetDatabase(DbNum);
                }
                var retVal = func(database);
                return retVal;
            }
            catch (Exception ex)
            {
                WriteLogByLog4Net("Redis命令" + (func.Method == null ? "" : func.Method.Name) + "失败：" + ex.Message, Log4NetMsgType.Warn); 
                try
                {
                    if (ex.Message.ToLower().IndexOf("no connection is available") >= 0)
                    {
                        _conn = RedisConnectionHelp.GetManager();
                    }
                }
                catch (Exception e)
                {
                    WriteLogByLog4Net("Redis链接-重建失败：" + (func.Method == null ? "" : func.Method.Name) + ex.Message, Log4NetMsgType.Fatal);
                }
                return default(T);
            }
        }

        private string ConvertJson<T>(T value)
        {
            if (value == null)
                return null;
            string result = value is string ? value.ToString() : Newtonsoft.Json.JsonConvert.SerializeObject(value);//JsonConvert.SerializeObject(value);
            return result;
        }

        private T ConvertObj<T>(RedisValue value)
        {
            var Ttype = typeof(T);
            if (value == RedisValue.Null)
                return default(T);
            else if (Ttype == typeof(string))
                return (T)Convert.ChangeType(value, Ttype);
            else
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);//JsonConvert.DeserializeObject<T>(value);
        }

        private List<T> ConvetList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        private RedisKey[] ConvertRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }

        private RedisValue[] ConvertRedisValues<T>(List<T> redisValues)
        {
            return redisValues.Select(redisValue => (RedisValue)ConvertJson<T>(redisValue)).ToArray();
        }

        /// <summary>
        /// 更新服务目标
        /// </summary>
        /// <param name="key"></param>
        private void updateDatabase(string key)
        {
            if (_conn.GetEndPoints().Count() > 1)
            {
                var Arr = _conn.GetEndPoints();
                RedisNewConnection ORedisNewConnection = new RedisNewConnection();
                var OConnMutpler = ORedisNewConnection.Instance.GetConnMutiplxer(key, _conn, DbNum);
                database = OConnMutpler.GetDatabase(DbNum);
            }
        }

        /// <summary>
        /// 获取Redis键所在服务端EndPoint
        /// </summary>
        /// <param name="listKey"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetEndPointByKeys(List<string> listKey, bool IsAddSysCustomKey = true)
        {
            Dictionary<string, List<string>> dictEndPointKeys = new Dictionary<string, List<string>>();
            var ArrEndPoints = _conn.GetEndPoints();
            if (ArrEndPoints.Count() > 1)
            {
                foreach (var itemKey in listKey)
                {
                    try
                    {
                        var NewitemKey = IsAddSysCustomKey ? AddSysCustomKey(itemKey) : itemKey;
                        var OEndPoint = GetRedisEndPoint(NewitemKey, IsAddSysCustomKey);
                        var WheredictEndPointKeys = dictEndPointKeys.Where(x => x.Key == OEndPoint.ToString());
                        if (!WheredictEndPointKeys.Any())
                        {
                            dictEndPointKeys.Add(OEndPoint.ToString(), new List<string> { NewitemKey });
                        }
                        else
                        {
                            WheredictEndPointKeys.FirstOrDefault().Value.Add(NewitemKey);
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
            else
            {
                dictEndPointKeys.Add(ArrEndPoints.FirstOrDefault().ToString(), listKey);
            }
            return dictEndPointKeys;
        }

        /// <summary>
        /// Log4Net 信息 类型
        /// level（级别）：标识这条日志信息的重要级别None>Fatal>ERROR>WARN>DEBUG>INFO>ALL，设定一个
        /// </summary>
        public enum Log4NetMsgType
        {
            Fatal = 1,
            Error = 2,
            Warn = 3,
            Debug = 4,
            Info = 5
        };

        /// <summary>
        /// 写日志
        /// level（级别）：标识这条日志信息的重要级别None>Fatal>ERROR>WARN>DEBUG>INFO>ALL，设定一个
        /// </summary>
        public void WriteLogByLog4Net(string ErrMSg, Log4NetMsgType OLog4NetMsgType = Log4NetMsgType.Info)
        {
            ILog log = log4net.LogManager.GetLogger("RedisHelper");
            switch (OLog4NetMsgType)
            {
                case Log4NetMsgType.Fatal:
                    log.Fatal(ErrMSg, new Exception("RedisHelper,发生了一个致命错误"));//严重错误
                    break;
                case Log4NetMsgType.Error:
                    log.Error(ErrMSg, new Exception("RedisHelper,发生了一个异常"));//错误
                    break;
                case Log4NetMsgType.Warn:
                    log.Warn(ErrMSg);//记录警告信息
                    break;
                case Log4NetMsgType.Debug:
                    log.Debug(ErrMSg);//记录调试信息
                    break;
                case Log4NetMsgType.Info:
                    log.Info(ErrMSg); //记录一般信息
                    break;
                default:
                    break;
            }
        }

        #endregion 辅助方法
    }
}