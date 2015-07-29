using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Esquel.Library.Infrastructure.Configuration;
using Esquel.Library.Infrastructure.Logging;

namespace Esquel.GEK.QI.DataAccess
{
    /// <summary>
    /// SQL直接帮助类（直接操作数据库）
    /// </summary>
    public static class SqlHelper
    {
        static string _connectionString = "";

        static SqlHelper()
        {
            ApplicationSettingsFactory.InitializeApplicationSettingsFactory(new WebConfigApplicationSettings());

            _connectionString = ApplicationSettingsFactory.GetApplicationSettings().Database;
        }

        /// <summary>
        /// Return current date time from database
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentTime()
        {
            return DateTime.Parse(FetchSingleScalar("SELECT GETDATE()").ToString());
        }

        public static SqlTransaction BeginTransation()
        {
            SqlConnection conn =
            GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlTransaction trans =
            conn.BeginTransaction();

            return trans;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static string GetConnectionString()
        {
            return _connectionString;
        }

        public static object ExecStroedProcedureWithTrans(SqlConnection connection, SqlTransaction tran, string spName, IDataParameter[] paramers)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }


            SqlCommand cmd = new SqlCommand(spName, connection, tran);
            cmd.CommandTimeout = 300;
            cmd.CommandType = CommandType.StoredProcedure;
            if (null != paramers)
            {
                foreach (SqlParameter param in paramers)
                {
                    cmd.Parameters.Add(param);
                }
            }

            //Add parameter to receive return value of stored procedure
            SqlParameter pr = new SqlParameter("ReturnValue", SqlDbType.Int);
            pr.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(pr);

            cmd.ExecuteNonQuery();

            return pr.Value;
        }

        public static DataTable ExcuteDataTableByTextWithTrans(SqlConnection connection, SqlTransaction tran, string sqlText, IDataParameter[] paramers)
        {
            DataTable dt = new DataTable("");
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = new SqlCommand(sqlText, connection, tran);
            cmd.CommandTimeout = 300;
            cmd.CommandType = CommandType.Text;
            if (null != paramers)
            {
                foreach (SqlParameter param in paramers)
                {
                    cmd.Parameters.Add(param);
                }
            }

            ////Add parameter to receive return value of stored procedure
            //SqlParameter pr = new SqlParameter("ReturnValue", SqlDbType.Int);
            //pr.Direction = ParameterDirection.ReturnValue;
            //cmd.Parameters.Add(pr);

            SqlDataAdapter ada = new SqlDataAdapter(cmd);
            ada.Fill(dt);

            return dt;
            //return pr.Value;
        }

        /// <summary>
        /// Execute sql query text and return record set as DataTable
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public static DataTable RunQuery(string sqlText)
        {
            DataTable dt = new DataTable("");
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sqlText, con);
                    SqlDataAdapter ada = new SqlDataAdapter(cmd);
                    ada.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().Error(ex);
            }
            return dt;
        }

        /// <summary>
        /// Execute sql query text and return record set as DataTable
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public static DataTable RunQuery(string sqlText, params IDataParameter[] paramers)
        {
            DataTable dt = new DataTable("");
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand sqlComm = new SqlCommand(sqlText, con);
                    foreach (SqlParameter sqlpara in paramers)
                    {
                        sqlComm.Parameters.Add(sqlpara);
                    }
                    SqlDataAdapter ada = new SqlDataAdapter(sqlComm);
                    ada.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().Error(ex);
            }
            return dt;
        }

        /// <summary>
        /// Execute sql query text and return record set as DataTable
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public static DataTable RunQuery(string sqlText, IDataParameter[] paramers, string connectionString)
        {
            DataTable dt = new DataTable("");
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand sqlComm = new SqlCommand(sqlText, con);
                    foreach (SqlParameter sqlpara in paramers)
                    {
                        sqlComm.Parameters.Add(sqlpara);
                    }
                    SqlDataAdapter ada = new SqlDataAdapter(sqlComm);
                    ada.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().Error(ex);
            }
            return dt;
        }

        /// <summary>
        /// Execute sql command text and return result as int
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns>Return effect record count</returns>
        public static int RunCommand(string sqlText, params System.Data.IDataParameter[] parmers)
        {
            int result = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sqlText, con);
                    if (null != parmers)
                    {
                        foreach (SqlParameter param in parmers)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    result = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().Error(ex);
            }
            return result;
        }


        /// <summary>
        /// Execute sql command text and return result as int
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTableByQuery(string sqlText, params System.Data.IDataParameter[] parmers)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sqlText, conn);
                cmd.CommandTimeout = 600;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                using (SqlDataAdapter sqlDataAdapert = new SqlDataAdapter(cmd))
                {
                    sqlDataAdapert.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// Execute sql command text and return result as int
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns>Return effect record count</returns>
        public static int RunCommand(string sqlText)
        {
            int result = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sqlText, con);
                    result = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().Error(ex);
            }
            return result;
        }

        /// <summary>
        /// execute stored procedure and no return.
        /// </summary>
        /// <param name="spName"> store procedure name</param>
        /// <param name="parmers"> paramers </param>       
        public static object ExecStroedProcedure(string spName, params System.Data.IDataParameter[] parmers)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandTimeout = 300;
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                //Add parameter to receive return value of stored procedure
                SqlParameter pr = new SqlParameter("ReturnValue", SqlDbType.Int);
                pr.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(pr);

                cmd.ExecuteNonQuery();
                return pr.Value;
                //Process return value of execution
                //if (pr.Value != null && int.Parse(pr.Value.ToString()) > 0)
                //{
                //    LogWriter.Write(LogLevel.Warning, "SGS.OTS.DataAccess.SqlHelper.ExecStroedProcedure", "Stored procedure return special value. There maybe are unexpected exceptions.", cmd.CommandText);
                //}
            }
        }


        /// <summary>
        /// execute stored procedure and no return.
        /// </summary>
        /// <param name="spName"> store procedure name</param>
        /// <param name="parmers"> paramers </param>       
        public static void ExecStroedProcedure(string spName, int timeout, params System.Data.IDataParameter[] parmers)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandTimeout = timeout;
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                //Add parameter to receive return value of stored procedure
                SqlParameter pr = new SqlParameter("ReturnValue", SqlDbType.Int);
                pr.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(pr);

                cmd.ExecuteNonQuery();

                //Process return value of execution
                //if (pr.Value != null && int.Parse(pr.Value.ToString()) > 0)
                //{
                //    LogWriter.Write(LogLevel.Warning, "SGS.OTS.DataAccess.SqlHelper.ExecStroedProcedure", "Stored procedure return special value. There maybe are unexpected exceptions.", cmd.CommandText);
                //}
            }
        }

        /// <summary>
        /// execute stored procedure and return effect recorded count.
        /// </summary>
        /// <param name="spName"> store procedure name</param>
        /// <param name="parmers"> paramers </param> 
        /// <returns>effect record count</returns>
        public static int ExecNonQueryByStroedProcedure(string spName, params System.Data.IDataParameter[] parmers)
        {
            int result = -1;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandTimeout = 60 * 3;
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                //Add parameter to receive return value of stored procedure
                //SqlParameter pr = new SqlParameter("ReturnValue", SqlDbType.Int);
                //pr.Direction = ParameterDirection.ReturnValue;
                //cmd.Parameters.Add(pr);

                result = cmd.ExecuteNonQuery();

                //Process return value of execution
                //if (pr.Value != null && int.Parse(pr.Value.ToString()) > 0)
                //{
                //    LogWriter.Write(LogLevel.Warning, "SGS.OTS.DataAccess.SqlHelper.ExecNonQueryByStroedProcedure", "Stored procedure return special value. There maybe are unexpected exceptions.", cmd.CommandText);
                //}
            }

            return result;
        }

        /// <summary>
        /// execute stored procedure and return value of stored procedure.
        /// </summary>
        /// <param name="spName">stored procedure name</param>
        /// <param name="useReturnValue">specify if use return value in stored procedure, if false, return effected record count.</param>
        /// <param name="parmers"></param>
        /// <returns>if useReturnValue='true', return returned value from stored procedure; if useReturnValue='false', return effected record count.</returns>
        public static int ExecNonQueryByStroedProcedure(string spName, bool useReturnValue, params System.Data.IDataParameter[] parmers)
        {
            int result = -1;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                //Add parameter to receive return value of stored procedure
                SqlParameter pr = new SqlParameter("ReturnValue", SqlDbType.Int);
                pr.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(pr);

                result = cmd.ExecuteNonQuery();

                //Process return value of execution
                if (useReturnValue && pr.Value != null)
                {
                    result = Convert.ToInt32(pr.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// execute stored procedure and return value of stored procedure.
        /// </summary>
        /// <param name="spName">stored procedure name</param>
        /// <param name="useReturnValue">specify if use return value in stored procedure, if false, return effected record count.</param>
        /// <param name="parmers"></param>
        /// <returns>if useReturnValue='true', return returned value from stored procedure; if useReturnValue='false', return effected record count.</returns>
        public static int ExecNonQueryByStroedProcedure(string spName, bool useReturnValue, int timeOut, params System.Data.IDataParameter[] parmers)
        {
            int result = -1;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = timeOut;

                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                //Add parameter to receive return value of stored procedure
                SqlParameter pr = new SqlParameter("ReturnValue", SqlDbType.Int);
                pr.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(pr);

                result = cmd.ExecuteNonQuery();

                //Process return value of execution
                if (useReturnValue && pr.Value != null)
                {
                    result = Convert.ToInt32(pr.Value);
                }
            }

            return result;
        }


        /// <summary>
        /// execute stored procedure and return DataTable.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parmers"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTableByStoredProcedure(string spName, IDataParameter[] parmers)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandTimeout = 600;
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                using (SqlDataAdapter sqlDataAdapert = new SqlDataAdapter(cmd))
                {
                    sqlDataAdapert.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// execute stored procedure and return DataTable.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parmers"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTableByStoredProcedure(string spName, int timeout, IDataParameter[] parmers)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandTimeout = timeout;
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                using (SqlDataAdapter sqlDataAdapert = new SqlDataAdapter(cmd))
                {
                    sqlDataAdapert.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// execute stored procedure and return DataTable.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parmers"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTableByStoredProcedure(string spName, IDataParameter[] parmers, string connectionString)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandTimeout = 1000;
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                using (SqlDataAdapter sqlDataAdapert = new SqlDataAdapter(cmd))
                {
                    sqlDataAdapert.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// execute stored procedure and return DataTable.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parmers"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTableByStoredProcedureTimeout(string spName, int timeout, IDataParameter[] parmers)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandTimeout = timeout;
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                using (SqlDataAdapter sqlDataAdapert = new SqlDataAdapter(cmd))
                {
                    sqlDataAdapert.Fill(dt);
                }
            }

            return dt;
        }


        /// <summary>
        ///  execute stored procedure and return datareader
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parmers"></param>
        /// <returns></returns>
        public static IDataReader ExecuteDataReaderByStoredProcedure(string spName, IDataParameter[] parmers)
        {
            IDataReader reader = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                reader = cmd.ExecuteReader();
            }

            return reader;

        }



        /// <summary>
        /// execute stored procedure and return DataSet
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parmers"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSetByStoredProcedure(string spName, IDataParameter[] parmers)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                using (SqlDataAdapter sqlDataAdapert = new SqlDataAdapter(cmd))
                {
                    sqlDataAdapert.Fill(ds);
                }
            }

            return ds;
        }




        /// <summary>
        /// execute stored procedure and return object
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parmers"></param>
        /// <returns></returns>
        public static object ExecuteScalarByStoredProcedure(string spName, IDataParameter[] parmers)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (null != parmers)
                {
                    foreach (SqlParameter param in parmers)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                object o = cmd.ExecuteScalar();

                for(int i=parmers.Length-1; i>=0; i--)
                {
                    if (parmers[i].Direction == ParameterDirection.ReturnValue)
                    {
                        return cmd.Parameters[parmers[i].ParameterName].Value;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// create input parameters sql or store proedure
        /// </summary>
        /// <param name="ParamName">parameter name</param>
        /// <param name="DbType">sqldbtype</param>
        /// <param name="Size">parameter length</param>
        /// <param name="Value">parameter value</param>
        /// <returns>result</returns>
        public static IDataParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object value)
        {
            return MakeParam(ParamName, DbType, Size, System.Data.ParameterDirection.Input, value);
        }

        /// <summary>
        /// Create output parameters sql or store proedure
        /// </summary>
        /// <param name="ParamName">parameter name </param>
        /// <param name="DbType">sqldbtype</param>
        /// <param name="Size">parameter length</param>
        /// <returns>result </returns>
        public static IDataParameter MakeOutParam(string ParamName, System.Data.SqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, System.Data.ParameterDirection.Output, null);
        }

        /// <summary>
        /// make parameter (input & output)
        /// </summary>
        /// <typeparam name="TValue">parameter type</typeparam>
        /// <param name="ParamName">parameter value</param>
        /// <param name="DbType">sqldbtype</param>
        /// <param name="Size">parameter length</param>
        /// <param name="Direction">direction :input/output</param>
        /// <param name="Value">parameter value</param>
        /// <returns>result</returns>
        private static IDataParameter MakeParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object value)
        {
            System.Data.IDataParameter param;

            if (Size > 0)
                param = new SqlParameter(ParamName, DbType, Size);
            else
                param = new SqlParameter(ParamName, DbType);

            param.Direction = Direction;
            if (!(Direction == System.Data.ParameterDirection.Output && value == null))
                param.Value = value;
            return param;
        }

        /// <summary>
        /// Execute sql query text and return single scalar value
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        internal static object FetchSingleScalar(string sqlText)
        {
            object result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sqlText, con);
                    result = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().Error(ex);
            }
            return result;
        }
    }
}
