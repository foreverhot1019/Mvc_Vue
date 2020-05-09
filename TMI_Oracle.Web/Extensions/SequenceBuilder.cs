using System;
using System.Collections.Generic;
//using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace TMI.Web
{
    public static class SequenceBuilder
    {
        //获取流水号 加锁
        static object LockGETSEQNO = new object();
        /// <summary>
        /// Redis帮助
        /// </summary>
        static RedisHelp.RedisHelper ORedisHelp = new RedisHelp.RedisHelper();
        //记录删除Key
        private static System.Collections.Concurrent.ConcurrentDictionary<string, int> DictDeltRedisIncrKey = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();

        static Models.WebdbContext db = new Models.WebdbContext();

        /// <summary>
        /// 更新数据库 sequencers表，委托
        /// </summary>
        private static Func<int, string, int> updateSQLSequenceFuc = new Func<int, string, int>((seed, v_prefix) =>
        {
            var SQLStr = $"update sequencers SET seed={seed} WHERE  prefix='{v_prefix}'";
            int ret = SQLDALHelper.SQLHelper.ExecuteNonQuery(System.Data.CommandType.Text, SQLStr, null);
            return ret;
        });

        /// <summary>
        /// 清除 Redis 自增Key
        /// </summary>
        public static void DeltCurrDictRedisIncrKey()
        {
            try
            {
                var QCurrDict = DictDeltRedisIncrKey;//.Where(n => n.Value >= 1);
                foreach (var item in QCurrDict)
                {
                    int val = 0;
                    if (!DictDeltRedisIncrKey.TryRemove(item.Key, out val))
                    {
                        TMI.Web.Extensions.Common.WriteLog_Local("删除-" + item.Key + "错误" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:dd"), "SequenceBuilder");
                    }
                    else
                    {
                        var DelKey = Task.Run(() =>
                        {
                            ORedisHelp.KeyDelete("Seqcer_" + item.Key);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }

        /// <summary>
        /// 获取流水号
        /// </summary>
        /// <param name="ptag"></param>
        /// <param name="delSequenceKey">要删除的键</param>
        /// <returns></returns>
        public static int GETSEQNO(OracleParameter[] ptag, string delSequenceKey = "")
        {
            int result = -1;

            //lock (LockGETSEQNO)
            //{
            //result = db.Database.ExecuteSqlCommand("begin GETSEQNO(v_prefix => :v_Prefix,v_OutPutNum => :v_OutPutNum); end;", ptag);
            try
            {
                var v_Prefix = ptag.Where(x => x.ParameterName.ToLower() == ":v_prefix").FirstOrDefault().Value.ToString();
                var v_OutPutNum = ptag.Where(x => x.ParameterName.ToLower() == ":v_outputnum").FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(delSequenceKey))
                {
                    var QDictDeltRedisIncrKey = DictDeltRedisIncrKey.Where(x => x.Key == delSequenceKey);
                    if (QDictDeltRedisIncrKey.Any())
                    {
                        //调整成异步删除 每天 23-3 点之间执行
                        //var DelKey = Task.Run(() =>
                        //{
                        //    ORedisHelp.KeyDelete("Seqcer_" + delSequenceKey);
                        //});
                        var ODictDeltRedisIncrKey = QDictDeltRedisIncrKey.FirstOrDefault();
                        var val = ODictDeltRedisIncrKey.Value;

                        int newval = 0;
                        if (DictDeltRedisIncrKey.TryRemove(delSequenceKey, out newval))
                        {
                            DictDeltRedisIncrKey.TryAdd(delSequenceKey, newval++);
                        }
                    }
                    else
                    {
                        DictDeltRedisIncrKey.GetOrAdd(delSequenceKey, 1);
                    }
                }
                double OutPutNum = 0;//流水号
                var GetNum = Task.Run(() =>
                {
                    OutPutNum = ORedisHelp.StringIncrement("Seqcer_" + v_Prefix);
                });
                GetNum.Wait();//等待 获取流水执行完毕
                if (OutPutNum > result)
                {
                    result = Convert.ToInt32(OutPutNum);
                    v_OutPutNum.Value = result;
                    Task.Run(new Func<int>(() =>
                    {
                        return updateSQLSequenceFuc(result, v_Prefix);
                    }));
                    //SQLDALHelper.OracleHelper.ExecuteNonQuery(" update sequencers t SET t.seed=" + OutPutNum + " WHERE  prefix='" + v_Prefix + "'");
                }
            }
            catch (Exception)
            {
                result = -1;
            }
            //}

            return result;
        }

        /// <summary>
        /// 获取客户流水号
        /// </summary>
        /// <returns>CM+yyMMdd+序号（三位不足补0）</returns>
        public static string NextCustomerNo()
        {
            var deldate = DateTime.Now.AddDays(-2).ToString("yyMMdd");
            var date = DateTime.Now.ToString("yyMMdd");
            var profix = "CustomerNo_:";
            string SeqName = profix + date;
            string DelSeqName = profix + deldate;//删除2天前的 Sequence 键

            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] {
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag, DelSeqName); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            int Seed = 0;
            if (!int.TryParse(OStrVal, out Seed))
            {
                return "CM" + date + OStrVal.PadLeft(3, '0');
            }
            else
                return "CM" + date + Seed.ToString("000");
        }

        /// <summary>
        /// 获取咨询单流水号
        /// </summary>
        /// <returns>ZX+yyMMdd+序号（三位不足补0）</returns>
        public static string NextAdvisoryOrdNo()
        {
            var deldate = DateTime.Now.AddDays(-2).ToString("yyMMdd");
            var date = DateTime.Now.ToString("yyMMdd");
            var profix = "AdvisoryOrdNo_:";
            string SeqName = profix + date;
            string DelSeqName = profix + deldate;//删除2天前的 Sequence 键

            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] {
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag, DelSeqName); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            int Seed = 0;
            if (!int.TryParse(OStrVal, out Seed))
            {
                return "ZX" + date + OStrVal.PadLeft(3, '0');
            }
            else
                return "ZX" + date + Seed.ToString("000");
        }

        /// <summary>
        /// 获取团客顶单流水号
        /// </summary>
        /// <returns>TK+yyMMdd+序号（三位不足补0）</returns>
        public static string NextJPOrderNo()
        {
            var deldate = DateTime.Now.AddDays(-2).ToString("yyMMdd");
            var date = DateTime.Now.ToString("yyMMdd");
            var profix = "JPOrderNo_:";
            string SeqName = profix + date;
            string DelSeqName = profix + deldate;//删除2天前的 Sequence 键

            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] {
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag, DelSeqName); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            int Seed = 0;
            if (!int.TryParse(OStrVal, out Seed))
            {
                return "J" + date + OStrVal.PadLeft(3, '0');
            }
            else
                return "J" + date + Seed.ToString("000");
        }

        /// <summary>
        /// 获取团客顶单流水号
        /// </summary>
        /// <returns>TK+yyMMdd+序号（三位不足补0）</returns>
        public static string NextTKOrderNo()
        {
            var deldate = DateTime.Now.AddDays(-2).ToString("yyMMdd");
            var date = DateTime.Now.ToString("yyMMdd");
            var profix = "TKOrderNo_:";
            string SeqName = profix + date;
            string DelSeqName = profix + deldate;//删除2天前的 Sequence 键

            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] {
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag, DelSeqName); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            int Seed = 0;
            if (!int.TryParse(OStrVal, out Seed))
            {
                return "TK" + date + OStrVal.PadLeft(3, '0');
            }
            else
                return "TK" + date + Seed.ToString("000");
        }
    }

    /// <summary>
    /// 清除 Redis序列键
    /// </summary>
    public class TimerDeltSequenceRedisKey : System.Web.Hosting.IRegisteredObject
    {
        System.Threading.Timer OTime;
        private int period = 1000 * 60 * 60;//间隔事件 毫秒

        public TimerDeltSequenceRedisKey()
        {
            OTime = new System.Threading.Timer(new System.Threading.TimerCallback((x) =>
            {
                var day = DateTime.Now.ToString("yyyy/MM/dd");
                var date = DateTime.Now;
                var MinDate = DateTime.MinValue;
                var MaxDate = DateTime.MinValue;
                //string Msg = "OTime:" + date.ToString("yyyy-MM-dd HH:mm:ss");
                //Console.WriteLine("----------------"+Msg);
                if (!DateTime.TryParse(day + " 23:00:00", out MinDate)) MinDate = DateTime.MinValue;
                if (!DateTime.TryParse(day + " 03:00:00", out MaxDate)) MaxDate = DateTime.MinValue;
                var a = date < MaxDate;
                var b = date > MinDate;
                if (MinDate != MaxDate && (a || b))
                {
                    SequenceBuilder.DeltCurrDictRedisIncrKey();
                }
            }), null, 0, period);
        }

        public void Stop(bool immediate)
        {
            OTime.Dispose();
        }
    }
}