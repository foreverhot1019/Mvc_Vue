using System;
using System.Collections.Generic;
//using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace AirOut.Web
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
            var SQLStr = " update sequencers t SET t.seed=" + seed + " WHERE  prefix='" + v_prefix + "'";
            int ret = SQLDALHelper.OracleHelper.ExecuteNonQuery(SQLStr);
            return ret;
        });

        /// <summary>
        /// 清除 Redis 自增Key
        /// 
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
                        AirOut.Web.Extensions.Common.WriteLog_Local("删除-" + item.Key + "错误" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:dd"), "SequenceBuilder");
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
            catch(Exception ex){
                Console.Write(ex);
            }
        }

        /// <summary>
        /// 获取流水号
        /// </summary>
        /// <param name="ptag"></param>
        /// <param name="delSequenceKey">要删除的键</param>
        /// <returns></returns>
        public static int GETSEQNO(OracleParameter[] ptag,string delSequenceKey="")
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
                        
                        int newval =0;
                        if (DictDeltRedisIncrKey.TryRemove(delSequenceKey, out newval))
                        {
                            DictDeltRedisIncrKey.TryAdd(delSequenceKey, newval++);
                        }
                    }else{
                        DictDeltRedisIncrKey.GetOrAdd(delSequenceKey,1);
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

        //获取成本管理中的记录号的流水号
        public static string NextCostMoney_No()
        {
            string DelSeqName = "YF" + DateTime.Now.AddMonths(-3).ToString("yyMM");//删除3月前的键
            string SeqName = "YF" + DateTime.Now.ToString("yyMM");
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
                return SeqName + OStrVal;
            }
            else
                return SeqName + Seed.ToString("00000");
        }

        //获取报价信息中的记录号的流水号
        public static string NextQuotedPriceSerial_No()
        {
            var date = DateTime.Now;
            string SeqName = "Q" + date.ToString("yyyyMMdd");
            string DelSeqName = "Q" + date.AddDays(-3).ToString("yyyyMMdd");//删除3天前的键
            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] { 
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag,DelSeqName); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            int Seed = 0;
            if (!int.TryParse(OStrVal, out Seed))
            {
                return SeqName + OStrVal;
            }
            else
                return SeqName + Seed.ToString("0000");
        }

        //获取客户报价中的记录号的流水号uoted
        public static string NextCustomerQuotedSerial_No()
        {
            string DelSeqName = "BJ" + DateTime.Now.AddMonths(-3).ToString("yyMM");//删除3月前的键
            string SeqName = "BJ" + DateTime.Now.ToString("yyMM");
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
                return SeqName + OStrVal;
            }
            else
                return SeqName + Seed.ToString("00000");
        }

        //获取签收单报表的签收单编号的流水号
        public static string NextSignReceipt_Code_No()
        {
            string SeqName = "QSD" + DateTime.Now.ToString("yyMM");
            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] { 
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            int Seed = 0;
            if (!int.TryParse(OStrVal, out Seed))
            {
                return SeqName + OStrVal;
            }
            else
                return SeqName + Seed.ToString("0000");
        }

        //获取承揽接单业务编号的流水号
        public static string NextEntrustmentInforOperation_IdSerial_No(DateTime? Flight_Date_Want)
        {
            if (Flight_Date_Want == null)
                Flight_Date_Want = DateTime.Now;

            string SeqName = "KSF" + Flight_Date_Want.Value.ToString("yy");
            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] { 
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            int Seed = 0;
            if (!int.TryParse(OStrVal, out Seed))
            {
                return SeqName + OStrVal;
            }
            else
                return SeqName + Seed.ToString("00000");
        }

        //获取仓库接单仓库编号的流水号
        public static string Nextwarehouse_receipt_Warehouse_CodeSerial_No()
        {
            string SeqName = "WH" + DateTime.Now.ToString("yyMM");
            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] { 
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            int Seed = 0;
            if (!int.TryParse(OStrVal, out Seed))
            {
                return SeqName + OStrVal;
            }
            else
                return SeqName + Seed.ToString("00000");
        }

        //获取承揽接单业务编号的流水号 KBA011 KBA
        public static string NextLot_NoSerial_No(string maxLot_No)
        {
            var en = "";
            var nextupen = "";
            //KBA9998 - KBA9999
            if (maxLot_No != null && maxLot_No.Length > 3)
            {
                nextupen = maxLot_No.Substring(2, 1);
                if (maxLot_No.Substring(3) == "9999")
                {
                    en = maxLot_No.Substring(0, 2);
                    nextupen = getNextUpEn(en);
                }
            }
            else
            {
                nextupen = getNextUpEn(en);
            }
            string SeqName = "KB" + nextupen;
            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] { 
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            int Seed = 0;
            if (!int.TryParse(OStrVal, out Seed))
            {
                return SeqName + OStrVal;
            }
            else
                return SeqName + Seed.ToString("0000");
        }

        /// <summary>
        /// 获取 A-Z 递增
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        private static string getNextUpEn(string en)
        {
            if (en == null || en.Equals(""))
                return "A";
            char lastE = 'Z';
            int lastEnglish = (int)lastE;
            char[] c = en.ToCharArray();
            if (c.Length > 1)
            {
                return null;
            }
            else
            {
                int now = (int)c[0];
                if (now >= lastEnglish)
                    return null;
                char uppercase = (char)(now + 1);
                return uppercase + "";
            }
        }

        /// <summary>
        /// 获取应收/付 序号 
        /// </summary>
        /// <param name="IsAr">是否应收</param>
        /// <param name="Dzbh">作业编号</param>
        /// <param name="IsDtl">是否明细</param>
        /// <returns></returns>
        public static string NextBmsBillArLineNo(bool IsAr, string Dzbh, bool IsDtl = false)
        {
            string SeqName = "BmsBill:" + (IsAr ? "Ar" : "Ap") + (IsDtl ? "Dtl" : "") + Dzbh;
            OracleParameter OutPut = new OracleParameter(":v_OutPutNum", OracleDbType.Int32);
            OutPut.Direction = System.Data.ParameterDirection.Output;
            OracleParameter[] ptag = new OracleParameter[] { 
                new OracleParameter(":v_Prefix", SeqName),
                OutPut
            };
            var result = GETSEQNO(ptag); //db.Database.SqlQuery<int>("begin GETSEQNO(v_prefix => :v_prefix,v_OutPutNum => :v_OutPutNum); end;", ptag).First();
            string OStrVal = OutPut.Value.ToString();
            return OStrVal;
        }

        /// <summary>
        /// 获取审核 流水号
        /// </summary>
        /// <param name="IsAr">是否应收</param>
        /// <returns>SH+yyyyMMdd+序号（四位不足补0）</returns>
        public static string NextAuditNo(bool IsAr)
        {
            var deldate = DateTime.Now.AddDays(-2).ToString("yyyyMMdd");
            var date = DateTime.Now.ToString("yyyyMMdd");
            string SeqName = "AuditNo_" + (IsAr ? "Ar" : "Ap") + ":" + date;
            string DelSeqName = "AuditNo_" + (IsAr ? "Ar" : "Ap") + ":" + deldate;//删除2天前的 Sequence 键

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
                return "SH" + date + OStrVal.PadLeft(4, '0');
            }
            else
                return "SH" + date + Seed.ToString("0000");
        }

        /// <summary>
        /// 获取DN发票 流水号
        /// </summary>
        /// <returns>DNAE+yyMMdd+序号（三位不足补0）</returns>
        public static string NextDNFPNo()
        {
            var deldate = DateTime.Now.AddDays(-2).ToString("yyMMdd");
            var date = DateTime.Now.ToString("yyMMdd");
            string SeqName = "DNFPNo_:" + date;
            string DelSeqName = "DNFPNo_:" + deldate;//删除2天前的 Sequence 键

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
                return "DNAE" + date + OStrVal.PadLeft(3, '0');
            }
            else
                return "DNAE" + date + Seed.ToString("000");
        }

        /// <summary>
        /// 获取应收/付 提交 流水号
        /// </summary>
        /// <returns>TJAE+yyMMdd+序号（三位不足补0）</returns>
        public static string NextSubmit_No(bool IsAr)
        {
            var deldate = DateTime.Now.AddDays(-2).ToString("yyMMdd");
            var date = DateTime.Now.ToString("yyMMdd");
            var profix = "Submit_No_:";// +"Submit_No_" + (IsAr ? "Ar" : "Ap") + ":";
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
                return "TJAE" + date + OStrVal.PadLeft(3, '0');
            }
            else
                return "TJAE" + date + Seed.ToString("000");
        }

        /// <summary>
        /// 获取应付 签收 流水号
        /// </summary>
        /// <returns>QSAE+yyMMdd+序号（三位不足补0）</returns>
        public static string NextSignIn_No()
        {
            var deldate = DateTime.Now.AddDays(-2).ToString("yyMMdd");
            var date = DateTime.Now.ToString("yyMMdd");
            var profix = "SignIn_No_:";
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
                return "QSAE" + date + OStrVal.PadLeft(3, '0');
            }
            else
                return "QSAE" + date + Seed.ToString("000");
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