using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Collections.Generic;

namespace SQLDALHelper
{
    public class SQLHelper
    {
        public static string CONN_STRING_NON_DTC
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"] == null ? "" : ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            }
        }

        /// <summary>
        /// database schema string
        /// </summary>
        public static readonly string DBSCHEMA = ConfigurationManager.AppSettings["DbSchema"] == null ? "" : ConfigurationManager.AppSettings["DbSchema"].ToString();

        // Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        public static SqlConnection OpenDatabase()
        {
            SqlConnection nmm_con = new SqlConnection(CONN_STRING_NON_DTC);

            return nmm_con;
        }

        public static SqlConnection OpenDatabase2(string nmm_sqlcon)
        {
            SqlConnection nmm_con = new SqlConnection(ConfigurationManager.AppSettings[nmm_sqlcon]);

            return nmm_con;
        }

        public static DataSet GetDataSet2(String strSql, string nmm_sqlcon)
        {
            SqlConnection p_Conn2 = new SqlConnection(ConfigurationManager.AppSettings[nmm_sqlcon]);

            SqlDataAdapter p_Dr2 = new SqlDataAdapter(strSql, p_Conn2);
            DataSet p_Ds2 = new DataSet();
            p_Dr2.Fill(p_Ds2);
            p_Dr2.Dispose();
            p_Conn2.Dispose();
            return p_Ds2;
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(CONN_STRING_NON_DTC))
            {
                cmd.CommandTimeout = 300;
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 300;
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 鏉╂柨娲栨稉鈧稉鐚砨ject缁鐎烽惃鍕棘閺?
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="paramName"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ReturnObjectParamExecuteNonQuery(string FieldsName, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(CONN_STRING_NON_DTC))
            {
                cmd.CommandTimeout = 300;
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                cmd.ExecuteNonQuery();
                object paramval = cmd.Parameters[FieldsName].Value;
                cmd.Parameters.Clear();
                return paramval;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static string ReturnStringFieldsByExecute(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlConnection sqlConn = new SqlConnection(CONN_STRING_NON_DTC);
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandTimeout = 300;
            try
            {
                PrepareCommand(sqlcmd, sqlConn, null, cmdType, cmdText, cmdParms);
                string strReturn = sqlcmd.ExecuteScalar().ToString();
                return (strReturn);
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlConn.Dispose();
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) using an existing SQL Transaction 
        /// using the provided parameters.
        /// </summary>
        /// <param name="trans">an existing sql transaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 300;
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a SqlCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the results</returns>
        public static SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(CONN_STRING_NON_DTC);

            cmd.CommandTimeout = 300;
            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        public static IDataAdapter ReturnDataAdapterByExecute(CommandType cmdType, string cmdText, params SqlParameter[] cmdParams)
        {
            SqlConnection sqlConn = new SqlConnection(CONN_STRING_NON_DTC);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandTimeout = 300;
            try
            {
                PrepareCommand(sqlCmd, sqlConn, null, cmdType, cmdText, cmdParams);
                SqlDataAdapter cmdDa = new SqlDataAdapter();
                //SqlCommandBuilder ocb = new SqlCommandBuilder(cmdDa);
                cmdDa.SelectCommand = sqlCmd;

                return cmdDa;
            }
            catch
            {
                sqlConn.Dispose();
                throw;
            }
        }

        public static DataSet ReturnDataSetByExecute(CommandType cmdType, string cmdText, params SqlParameter[] cmdParams)
        {
            SqlConnection sqlConn = new SqlConnection(CONN_STRING_NON_DTC);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandTimeout = 300;
            try
            {
                PrepareCommand(sqlCmd, sqlConn, null, cmdType, cmdText, cmdParams);
                SqlDataAdapter cmdDa = new SqlDataAdapter();
                //SqlCommandBuilder ocb = new SqlCommandBuilder(cmdDa);
                cmdDa.SelectCommand = sqlCmd;
                DataSet ds = new DataSet();
                cmdDa.Fill(ds);

                return ds;
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlConn.Dispose();
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(CONN_STRING_NON_DTC))
            {
                cmd.CommandTimeout = 300;
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 300;
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] cmdParms)
        {
            parmCache[cacheKey] = cmdParms;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        public static DataSet GetDataSet(String strSql)
        {
            SqlConnection p_Conn = new SqlConnection(CONN_STRING_NON_DTC);

            SqlDataAdapter p_Dr = new SqlDataAdapter(strSql, p_Conn);
            DataSet p_Ds = new DataSet();
            p_Dr.Fill(p_Ds);
            p_Dr.Dispose();
            p_Conn.Dispose();
            return p_Ds;
        }

        /// <summary>
        /// By Mikey
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="cmdType"></param>
        /// <param name="sql"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string connString, string sql)
        {
            SqlConnection cn = new SqlConnection(connString);
            SqlDataAdapter dr = new SqlDataAdapter(sql, cn);
            DataSet ds = new DataSet();
            dr.Fill(ds);
            dr.Dispose();
            cn.Dispose();
            return ds;
        }

        public static DataSet GetDataSet(string connString, CommandType cmdType, string sql, params SqlParameter[] cmdParams)
        {
            SqlConnection sqlConn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 300;
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, sqlConn, null, cmdType, sql, cmdParams);
                SqlDataAdapter cmdDa = new SqlDataAdapter(cmd);
                cmdDa.Fill(ds);
                cmdDa.Dispose();
                return ds;
            }
            catch
            {

                throw;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static DataSet GetDataSet(SqlConnection conn, CommandType cmdType, string sql, SqlParameter[] cmdParams)
        {
            SqlConnection sqlConn = conn;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 300;
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, sqlConn, null, cmdType, sql, cmdParams);
                SqlDataAdapter cmdDa = new SqlDataAdapter(cmd);
                cmdDa.Fill(ds);
                cmdDa.Dispose();
                return ds;
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static DataSet GetDataSet(SqlTransaction trans, CommandType cmdType, string sql, params SqlParameter[] cmdParams)
        {
            SqlConnection sqlConn = trans.Connection;
            SqlCommand cmd = new SqlCommand();

            cmd.CommandTimeout = 300;

            cmd.Transaction = trans;

            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, sqlConn, null, cmdType, sql, cmdParams);
                SqlDataAdapter cmdDa = new SqlDataAdapter(cmd);
                cmdDa.Fill(ds);
                cmdDa.Dispose();
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetDataSet(CommandType cmdType, string sql, params SqlParameter[] cmdParams)
        {
            SqlConnection sqlConn = new SqlConnection(CONN_STRING_NON_DTC);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandTimeout = 300;
            DataSet ds = new DataSet();
            try
            {
                PrepareCommand(cmd, sqlConn, null, cmdType, sql, cmdParams);
                SqlDataAdapter cmdDa = new SqlDataAdapter(cmd);
                cmdDa.Fill(ds);
                cmdDa.Dispose();
                return ds;
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static SqlDataReader ReturnDataReaderAndRecordCountByExecute(CommandType cmdType, string cmdText, out int intRecordCount, string strField, params SqlParameter[] cmdParms)
        {
            SqlConnection sqlConn = new SqlConnection(CONN_STRING_NON_DTC);
            SqlCommand sqlcmd = new SqlCommand();

            sqlcmd.CommandTimeout = 300;
            try
            {
                PrepareCommand(sqlcmd, sqlConn, null, cmdType, cmdText, cmdParms);
                sqlcmd.ExecuteNonQuery();
                intRecordCount = (int)(sqlcmd.Parameters[strField].Value);
                SqlDataReader sqlreader = sqlcmd.ExecuteReader(CommandBehavior.CloseConnection);
                sqlcmd.Parameters.Clear();
                return (sqlreader);
            }
            catch
            {
                sqlConn.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static int ExecuteSqlTransaction(ArrayList SQLStringList)
        {
            int val = 0;
            using (SqlConnection p_Conn = new SqlConnection(CONN_STRING_NON_DTC))
            {
                p_Conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = p_Conn;
                SqlTransaction tx = p_Conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            val = val + cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (Exception E)
                {
                    val = 0;
                    tx.Rollback();
                    throw new Exception(E.Message);
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
            using (SqlConnection p_Conn = new SqlConnection(CONN_STRING_NON_DTC))
            {
                p_Conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = p_Conn;
                SqlTransaction tx = p_Conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    foreach (string sql in SQLStringList)
                    {
                        if (!String.IsNullOrEmpty(sql))
                        {
                            cmd.CommandText = sql;
                            ret += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (Exception)
                {
                    tx.Rollback();
                    //throw new Exception(E.Message);
                    ret = 0;
                }
                finally
                {
                    if (p_Conn.State != ConnectionState.Closed)
                    {
                        p_Conn.Close();
                    }
                }
                return ret;
            }
        }
    }
}