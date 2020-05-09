using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDALHelper
{
    /// <summary>
    /// Oracle数据库访问层
    /// </summary>
    public class OracleHelper
    {
        static OracleHelper()
        {
            string DbSchema = System.Configuration.ConfigurationManager.AppSettings["DbSchema"] ?? "";
            string SQLWriteToLog = System.Configuration.ConfigurationManager.AppSettings["SQLWriteToLog"] ?? "";
            DBSCHEMA = DbSchema;
            WriteToLog = SQLWriteToLog;
        }
        //<add key="ConnStringMain" value="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.20.60.35)(PORT=1521))(CONNECT_DATA=(SID=untestdb)));User Id=cusdoc;Password=cusdoc;Pooling=true;Min Pool Size=100;Max Pool Size=1000;"/>
        /// <summary>
        /// database connection string
        /// </summary>
        //public static readonly string CONN_STRING_NON_DTC = System.Configuration.ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();
        public static string CONN_STRING_NON_DTC
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["OracleConnection"] == null ? "" : System.Configuration.ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();//"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.20.60.35)(PORT=1521))(CONNECT_DATA=(SID=untestdb)));User Id=wecds;Password=wecds20160908;Pooling=true;Min Pool Size=100;Max Pool Size=1000;";
            }
        }

        /// <summary>
        /// database schema string
        /// </summary>
        public static readonly string DBSCHEMA;// = (System.Configuration.ConfigurationManager.AppSettings["DbSchema"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["DbSchema"].ToString());

        /// <summary>
        /// 是否写日志 "1"写日志
        /// </summary>
        public static readonly string WriteToLog;// = (System.Configuration.ConfigurationManager.AppSettings["SQLWriteToLog"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["SQLWriteToLog"].ToString());

        /// <summary>
        /// Hashtable to store cached parameters
        /// </summary>
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParms">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params OracleParameter[] cmdParms)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandTimeout = 300;//5分钟

            using (OracleConnection conn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                if (WriteToLog == "1")
                {
                    if (cmdParms != null)
                    {
                        WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                    }
                    else
                    {
                        WriteLogHelper.WriteLog(cmdText);
                    }
                }

                return val;
            }
        }

        /// <summary>
        /// Execute a OracleCommand (that returns no resultset) against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="conn">an existing database connection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParms">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleConnection conn, CommandType cmdType, string cmdText, params OracleParameter[] cmdParms)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandTimeout = 300;//5分钟

            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (WriteToLog == "1")
            {
                if (cmdParms != null)
                {
                    WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                }
                else
                {
                    WriteLogHelper.WriteLog(cmdText);
                }
            }

            return val;
        }

        /// <summary>
        /// return object
        /// </summary>
        /// <param name="FieldsName">Field Name</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParms">an array of SqlParamters used to execute the command</param>
        /// <returns>return ohiect </returns>
        public static object ReturnObjectParamExecuteNonQuery(string FieldsName, CommandType cmdType, string cmdText, params OracleParameter[] cmdParms)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandTimeout = 300;//5分钟
            using (OracleConnection conn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                cmd.ExecuteNonQuery();
                object paramval = cmd.Parameters[FieldsName].Value;
                cmd.Parameters.Clear();

                if (conn.State == ConnectionState.Open)
                    conn.Close();
                if (WriteToLog == "1")
                {
                    if (cmdParms != null)
                    {
                        WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                    }
                    else
                    {
                        WriteLogHelper.WriteLog(cmdText);
                    }
                }

                return paramval;
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) using an existing SQL Transaction 
        /// using the provided parameters.
        /// </summary>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParams">an array of SqlParamters used to execute the command</param>
        /// <returns>return a string</returns>
        public static string ReturnStringFieldsByExecute(CommandType cmdType, string cmdText, params OracleParameter[] cmdParms)
        {
            OracleCommand Oraclecmd = new OracleCommand();
            Oraclecmd.CommandTimeout = 300;//5分钟
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                PrepareCommand(Oraclecmd, OracleConn, null, cmdType, cmdText, cmdParms);
                string strReturn = Oraclecmd.ExecuteScalar().ToString();

                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();
                if (WriteToLog == "1")
                {
                    if (cmdParms != null)
                    {
                        WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                    }
                    else
                    {
                        WriteLogHelper.WriteLog(cmdText);
                    }
                }

                return (strReturn);
            }
        }

        /// <summary>
        /// Execute a OracleCommand (that returns no resultset) using an existing Oracle Transaction 
        /// using the provided parameters.
        /// </summary>
        /// <param name="trans">an existing Oracle transaction</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParams">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] cmdParms)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandTimeout = 300;//5分钟

            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            if (WriteToLog == "1")
            {
                if (cmdParms != null)
                {
                    WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                }
                else
                {
                    WriteLogHelper.WriteLog(cmdText);
                }
            }

            return val;
        }

        /// <summary>
        /// Execute a OracleCommand (that returns no resultset) using an existing Oracle Transaction 
        /// using the provided parameters.
        /// </summary>
        /// <param name="trans">an existing Oracle transaction</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParams">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string cmdText)
        {
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandTimeout = 300;//5分钟

                PrepareCommand(cmd, OracleConn, null, CommandType.Text, cmdText, null);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();
                if (WriteToLog == "1")
                {
                    WriteLogHelper.WriteLog(cmdText);
                }

                return val;
            }
        }

        /// <summary>
        /// Execute a OracleCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParams">an array of SqlParamters used to execute the command</param>
        /// <returns>A OracleDataReader containing the results</returns>
        public static OracleDataReader ExecuteReader(CommandType cmdType, string cmdText, params OracleParameter[] cmdParms)
        {
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandTimeout = 300;//5分钟

                // we use a try/catch here because if the method throws an exception we want to 
                // close the connection throw code, because no datareader will exist, hence the 
                // commandBehaviour.CloseConnection will not work

                PrepareCommand(cmd, OracleConn, null, cmdType, cmdText, cmdParms);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();

                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();
                if (WriteToLog == "1")
                {
                    if (cmdParms != null)
                    {
                        WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                    }
                    else
                    {
                        WriteLogHelper.WriteLog(cmdText);
                    }
                }

                return rdr;
            }
        }

        /// <summary>
        /// Execute a OracleCommand that returns a IDataAdapter against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParams">an array of SqlParamters used to execute the command</param>
        /// <returns>return a IDataAdapter</returns>
        public static IDataAdapter ReturnDataAdapterByExecute(CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                OracleCommand OracleCmd = new OracleCommand();
                OracleCmd.CommandTimeout = 300;//5分钟

                PrepareCommand(OracleCmd, OracleConn, null, cmdType, cmdText, cmdParams);
                OracleDataAdapter cmdDa = new OracleDataAdapter();
                cmdDa.SelectCommand = OracleCmd;
                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();

                if (WriteToLog == "1")
                {
                    if (cmdParams != null)
                    {
                        WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParams.Select(x => x.ParameterName)));
                    }
                    else
                    {
                        WriteLogHelper.WriteLog(cmdText);
                    }
                }

                return cmdDa;
            }
        }

        /// <summary>
        /// Execute a OracleCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParams">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText, params OracleParameter[] cmdParms)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandTimeout = 300;//5分钟
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                PrepareCommand(cmd, OracleConn, null, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();

                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();

                if (WriteToLog == "1")
                {
                    if (cmdParms != null)
                    {
                        WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                    }
                    else
                    {
                        WriteLogHelper.WriteLog(cmdText);
                    }
                }

                return val;
            }
        }

        /// <summary>
        /// Execute a OracleCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="conn">an existing database connection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParams">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(OracleConnection OracleConn, CommandType cmdType, string cmdText, params OracleParameter[] cmdParms)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandTimeout = 300;//5分钟

            PrepareCommand(cmd, OracleConn, null, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();

            if (OracleConn.State == ConnectionState.Open)
                OracleConn.Close();

            if (WriteToLog == "1")
            {
                if (cmdParms != null)
                {
                    WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                }
                else
                {
                    WriteLogHelper.WriteLog(cmdText);
                }
            }

            return val;
        }

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of OracleParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params OracleParameter[] cmdParms)
        {
            parmCache[cacheKey] = cmdParms;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached OracleParamters array</returns>
        public static OracleParameter[] GetCachedParameters(string cacheKey)
        {
            OracleParameter[] cachedParms = (OracleParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            OracleParameter[] clonedParms = new OracleParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (OracleParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">OracleCommand object</param>
        /// <param name="conn">OracleConnection object</param>
        /// <param name="trans">OracleTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">OracleParameters to use in the command</param>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            //默认情况下ODP.Net 绑定变量时，sql语句中的变量顺序必须和变量绑定顺序一致，否则Fill查不到数据，cmd.ExecuteNonQuery()返回0无法执行， 将BindByName 设为true后，sql变量顺序和绑定顺序即可不一致.
            cmd.BindByName = true;
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandTimeout = 300;//5分钟

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }

            if (WriteToLog == "1")
            {
                if (cmdParms != null)
                {
                    WriteLogHelper.WriteLog(cmdText + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                }
                else
                {
                    WriteLogHelper.WriteLog(cmdText);
                }
            }
        }

        /// <summary>
        /// Execute a OracleCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="strSql">Command text</param>
        /// <returns>return DataSet</returns>
        public static DataSet GetDataSet(String strOracle)
        {
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                if (WriteToLog == "1")
                {
                    WriteLogHelper.WriteLog(strOracle);
                }

                OracleDataAdapter p_Dr = new OracleDataAdapter(strOracle, OracleConn);
                DataSet p_Ds = new DataSet();
                p_Dr.Fill(p_Ds);
                p_Dr.Dispose();
                OracleConn.Dispose();
                return p_Ds;
            }
        }

        /// <summary>
        ///  Execute a OracleCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="cmdType">Command type</param>
        /// <param name="sql">Command text</param>
        /// <param name="cmdParams">SqlParameters to use in the command</param>
        /// <returns>return DataSet</returns>
        public static DataSet GetDataSet(CommandType cmdType, string sql, params OracleParameter[] cmdParams)
        {
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandTimeout = 300;//3分钟

                if (WriteToLog == "1")
                {
                    if (cmdParams != null)
                    {
                        WriteLogHelper.WriteLog(sql + " - " + string.Join(",", cmdParams.Select(x => x.ParameterName)));
                    }
                    else
                    {
                        WriteLogHelper.WriteLog(sql);
                    }
                }

                DataSet ds = new DataSet();

                PrepareCommand(cmd, OracleConn, null, cmdType, sql, cmdParams);
                OracleDataAdapter cmdDa = new OracleDataAdapter(cmd);
                cmdDa.Fill(ds);
                cmdDa.Dispose();
                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();

                return ds;
            }
        }

        /// <summary>
        /// Execute a OracleCommand that returns OracleDataReader against an existing database connection 
        /// using the provided parameters. 
        /// </summary>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="intRecordCount">return record count</param>
        /// <param name="strField">output SqlParameters name</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        /// <returns>return OracleDataReader</returns>
        public static OracleDataReader ReturnDataReaderAndRecordCountByExecute(CommandType cmdType, string cmdText, out int intRecordCount, string strField, params OracleParameter[] cmdParms)
        {
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                OracleCommand Oraclecmd = new OracleCommand();

                Oraclecmd.CommandTimeout = 300;//5分钟

                PrepareCommand(Oraclecmd, OracleConn, null, cmdType, cmdText, cmdParms);
                Oraclecmd.ExecuteNonQuery();
                intRecordCount = (int)(Oraclecmd.Parameters[strField].Value);
                OracleDataReader Oraclereader = Oraclecmd.ExecuteReader(CommandBehavior.CloseConnection);
                Oraclecmd.Parameters.Clear();
                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();
                return (Oraclereader);
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static int ExecuteSqlTran(System.Collections.Generic.List<String> SQLStringList, params OracleParameter[] cmdParms)
        {
            int ret = 0;
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                OracleConn.Open();
                OracleCommand Oraclecmd = new OracleCommand();
                Oraclecmd.Connection = OracleConn;
                Oraclecmd.CommandTimeout = 300;//5分钟
                OracleTransaction tx = OracleConn.BeginTransaction();
                Oraclecmd.Transaction = tx;
                //默认情况下ODP.Net 绑定变量时，sql语句中的变量顺序必须和变量绑定顺序一致，否则Fill查不到数据，cmd.ExecuteNonQuery()返回0无法执行， 将BindByName 设为true后，sql变量顺序和绑定顺序即可不一致.
                Oraclecmd.BindByName = true;
                try
                {
                    if (cmdParms != null)
                    {
                        foreach (OracleParameter parm in cmdParms)
                            Oraclecmd.Parameters.Add(parm);
                    }

                    foreach (string sql in SQLStringList)
                    {
                        if (!String.IsNullOrEmpty(sql))
                        {
                            if (WriteToLog == "1")
                            {
                                if (cmdParms != null)
                                {
                                    WriteLogHelper.WriteLog(sql + " - " + string.Join(",", cmdParms.Select(x => x.ParameterName)));
                                }
                                else
                                {
                                    WriteLogHelper.WriteLog(sql);
                                }
                            }
                            Oraclecmd.CommandText = sql;
                            ret += Oraclecmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    tx.Rollback();
                    //throw new Exception(E.Message);
                    ret = 0;
                }
                finally
                {
                    if (OracleConn.State != ConnectionState.Closed)
                    {
                        OracleConn.Close();
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static int ExecuteSqlTransaction(ArrayList SQLStringList)
        {
            int val = 0;
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                OracleConn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = OracleConn;
                cmd.CommandTimeout = 300;//5分钟
                OracleTransaction tx = OracleConn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            if (WriteToLog == "1")
                            {
                                WriteLogHelper.WriteLog(strsql);
                            }
                            cmd.CommandText = strsql;
                            val = val + cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (System.Data.OracleClient.OracleException E)
                {
                    val = 0;
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
                finally
                {
                    if (OracleConn.State != ConnectionState.Closed)
                    {
                        OracleConn.Close();
                    }
                }
            }
            return val;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static int ExecuteSqlTran(System.Collections.Generic.List<String> SQLStringList)
        {
            int ret = 0;
            using (OracleConnection OracleConn = new OracleConnection(CONN_STRING_NON_DTC))
            {
                OracleConn.Open();
                OracleCommand Oraclecmd = new OracleCommand();
                Oraclecmd.Connection = OracleConn;
                Oraclecmd.CommandTimeout = 300;//5分钟
                OracleTransaction tx = OracleConn.BeginTransaction();
                Oraclecmd.Transaction = tx;
                try
                {
                    foreach (string sql in SQLStringList)
                    {
                        if (!String.IsNullOrEmpty(sql))
                        {
                            if (WriteToLog == "1")
                            {
                                WriteLogHelper.WriteLog(sql);
                            }
                            Oraclecmd.CommandText = sql;
                            ret += Oraclecmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    tx.Rollback();
                    //throw new Exception(E.Message);
                    ret = 0;
                }
                finally
                {
                    if (OracleConn.State != ConnectionState.Closed)
                    {
                        OracleConn.Close();
                    }
                }
                return ret;
            }
        }

    }
}