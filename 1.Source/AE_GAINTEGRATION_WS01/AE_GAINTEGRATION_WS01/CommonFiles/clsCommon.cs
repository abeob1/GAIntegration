using System;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace AE_GAINTEGRATION_WS01
{
    public class clsCommon
    {
        #region Objects
        clsLog oLog = new clsLog();
        public Int16 p_iDebugMode = DEBUG_ON;
        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public string sErrDesc = string.Empty;

        #endregion

        #region Methods

        public DataTable ExecuteSQLQuery(string sQuery, string sCompanyCode)
        {
            string sFuncName = "ExecuteSQLQuery()";
            string sConstr = ConfigurationManager.ConnectionStrings["DBSSG"].ToString();

            string[] sArray = sConstr.Split(';');
            string sSplitCompany = sConstr.Split(';').Last();
            string sSplit1 = sSplitCompany.Split('=').First();
            string sCompanyGenerate = sSplit1 + "=" + sCompanyCode;

            sConstr = sArray[0] + ";" + sArray[1] + ";" + sArray[2] + ";" + sArray[3] + ";" + sCompanyGenerate;
            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Connection String : " + sConstr, sFuncName);

            System.Data.Odbc.OdbcConnection oCon = new System.Data.Odbc.OdbcConnection(sConstr);
            System.Data.Odbc.OdbcCommand oCmd = new System.Data.Odbc.OdbcCommand();
            DataSet oDs = new DataSet();

            try
            {

                oCon.Open();
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandText = sQuery;
                oCmd.Connection = oCon;
                oCmd.CommandTimeout = 120;
                System.Data.Odbc.OdbcDataAdapter da = new System.Data.Odbc.OdbcDataAdapter(oCmd);
                da.Fill(oDs);
            }
            catch (Exception ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With Error", sFuncName);
                oCon.Dispose();
                throw new Exception(ex.Message);
            }
            return oDs.Tables[0];
        }

        public string ExecuteNonQuery(string sQuery)
        {
            string sFuncName = "ExecuteNonQuery()";

            string sConstr = ConfigurationManager.ConnectionStrings["adapterConnection"].ToString();
            System.Data.Odbc.OdbcConnection oCon = new System.Data.Odbc.OdbcConnection(sConstr);
            System.Data.Odbc.OdbcCommand oCmd = new System.Data.Odbc.OdbcCommand();

            try
            {
                oCon.Open();
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandText = sQuery;
                oCmd.Connection = oCon;
                oCmd.CommandTimeout = 120;
                oCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                oCon.Dispose();
                throw new Exception(ex.Message);
            }
            return "SUCCESS";
        }

        public string ExecuteQuery(string sQuery, string sCompanyCode)
        {
            string sFuncName = "ExecuteQuery()";

            string sConstr = ConfigurationManager.ConnectionStrings["DBSSG"].ToString();

            string[] sArray = sConstr.Split(';');
            string sSplitCompany = sConstr.Split(';').Last();
            string sSplit1 = sSplitCompany.Split('=').First();
            string sCompanyGenerate = sSplit1 + "=" + sCompanyCode;

            sConstr = sArray[0] + ";" + sArray[1] + ";" + sArray[2] + ";" + sArray[3] + ";" + sCompanyGenerate;
            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Connection String : " + sConstr, sFuncName);

            System.Data.Odbc.OdbcConnection oCon = new System.Data.Odbc.OdbcConnection(sConstr);
            System.Data.Odbc.OdbcCommand oCmd = new System.Data.Odbc.OdbcCommand();

            try
            {
                oCon.Open();
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandText = sQuery;
                oCmd.Connection = oCon;
                oCmd.CommandTimeout = 120;
                oCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                oCon.Dispose();
                throw new Exception(ex.Message);
            }
            return "SUCCESS";
        }

        public DataTable ExecuteSelectQuery(string sQuery)
        {
            string sFuncName = "ExecuteSelectQuery()";

            string sConstr = ConfigurationManager.ConnectionStrings["adapterConnection"].ToString();
            System.Data.Odbc.OdbcConnection oCon = new System.Data.Odbc.OdbcConnection(sConstr);
            System.Data.Odbc.OdbcCommand oCmd = new System.Data.Odbc.OdbcCommand();
            DataSet oDs = new DataSet();

            try
            {
                oCon.Open();
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandText = sQuery;
                oCmd.Connection = oCon;
                oCmd.CommandTimeout = 120;
                System.Data.Odbc.OdbcDataAdapter da = new System.Data.Odbc.OdbcDataAdapter(oCmd);
                da.Fill(oDs);
            }
            catch (Exception ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with Error", sFuncName);
                oCon.Dispose();
                throw new Exception(ex.Message);
            }
            return oDs.Tables[0];
        }

        public DataTable ExecuteSelectQueryInSQLServer(string sQuery)
        {
            string sFuncName = "ExecuteSelectQuery()";

            string sConstr = ConfigurationManager.ConnectionStrings["sqlConnection"].ToString();
            SqlConnection oCon = new SqlConnection(sConstr);
            SqlCommand oCmd = new SqlCommand();
            DataSet oDs = new DataSet();

            try
            {
                oCon.Open();
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandText = sQuery;
                oCmd.Connection = oCon;
                oCmd.CommandTimeout = 120;
                SqlDataAdapter da = new SqlDataAdapter(oCmd);
                da.Fill(oDs);
            }
            catch (Exception ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with Error", sFuncName);
                oCon.Dispose();
                throw new Exception(ex.Message);
            }
            return oDs.Tables[0];
        }

        // Code changes done here with Param
        public DataTable ExecuteSelectQuery(string sQuery, System.Data.Odbc.OdbcParameter[] param)
        {
            string sFuncName = "ExecuteSelectQuery()";

            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);
            string sConstr = ConfigurationManager.ConnectionStrings["adapterConnection"].ToString();
            System.Data.Odbc.OdbcConnection oCon = new System.Data.Odbc.OdbcConnection(sConstr);
            System.Data.Odbc.OdbcCommand oCmd = new System.Data.Odbc.OdbcCommand();
            DataSet oDs = new DataSet();

            try
            {
                oCon.Open();
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandText = sQuery;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SQL Query : " + sQuery, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before adding Parameters", sFuncName);
                foreach (var item in param)
                {
                    oCmd.Parameters.Add(item);
                }
                //oCmd.Parameters.Add((param);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After adding parameters", sFuncName);
                oCmd.Connection = oCon;
                oCmd.CommandTimeout = 120;
                System.Data.Odbc.OdbcDataAdapter da = new System.Data.Odbc.OdbcDataAdapter(oCmd);
                da.Fill(oDs);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with Success", sFuncName);
            }
            catch (Exception ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with Error", sFuncName);
                oCon.Dispose();
                throw new Exception(ex.Message);
            }
            return oDs.Tables[0];
        }

        public string ExecuteNonQuery(string sQuery, System.Data.Odbc.OdbcParameter[] param)
        {
            string sFuncName = "ExecuteNonQuery()";

            string sConstr = ConfigurationManager.ConnectionStrings["adapterConnection"].ToString();
            System.Data.Odbc.OdbcConnection oCon = new System.Data.Odbc.OdbcConnection(sConstr);
            System.Data.Odbc.OdbcCommand oCmd = new System.Data.Odbc.OdbcCommand();

            try
            {
                oCon.Open();
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandText = sQuery;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SQL Query : " + sQuery, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before adding Parameters", sFuncName);
                foreach (var item in param)
                {
                    oCmd.Parameters.Add(item);
                }
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After adding parameters", sFuncName);

                oCmd.Connection = oCon;
                oCmd.CommandTimeout = 120;
                oCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                oCon.Dispose();
                throw new Exception(ex.Message);
            }
            return "SUCCESS";
        }

        public string ExecuteQuery(string sQuery, string sCompanyCode, System.Data.Odbc.OdbcParameter[] param)
        {
            string sFuncName = "ExecuteQuery()";

            string sConstr = ConfigurationManager.ConnectionStrings["DBSSG"].ToString();

            string[] sArray = sConstr.Split(';');
            string sSplitCompany = sConstr.Split(';').Last();
            string sSplit1 = sSplitCompany.Split('=').First();
            string sCompanyGenerate = sSplit1 + "=" + sCompanyCode;

            sConstr = sArray[0] + ";" + sArray[1] + ";" + sArray[2] + ";" + sArray[3] + ";" + sCompanyGenerate;
            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Connection String : " + sConstr, sFuncName);

            System.Data.Odbc.OdbcConnection oCon = new System.Data.Odbc.OdbcConnection(sConstr);
            System.Data.Odbc.OdbcCommand oCmd = new System.Data.Odbc.OdbcCommand();

            try
            {
                oCon.Open();
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandText = sQuery;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SQL Query : " + sQuery, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before adding Parameters", sFuncName);
                foreach (var item in param)
                {
                    oCmd.Parameters.Add(item);
                }
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After adding parameters", sFuncName);
                oCmd.Connection = oCon;
                oCmd.CommandTimeout = 120;
                oCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                oCon.Dispose();
                throw new Exception(ex.Message);
            }
            return "SUCCESS";
        }

        public DataTable ExecuteSQLQuery(string sQuery, string sCompanyCode, System.Data.Odbc.OdbcParameter[] param)
        {
            string sFuncName = "ExecuteSQLQuery()";
            string sConstr = ConfigurationManager.ConnectionStrings["DBSSG"].ToString();

            string[] sArray = sConstr.Split(';');
            string sSplitCompany = sConstr.Split(';').Last();
            string sSplit1 = sSplitCompany.Split('=').First();
            string sCompanyGenerate = sSplit1 + "=" + sCompanyCode;

            sConstr = sArray[0] + ";" + sArray[1] + ";" + sArray[2] + ";" + sArray[3] + ";" + sCompanyGenerate;
            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Connection String : " + sConstr, sFuncName);

            System.Data.Odbc.OdbcConnection oCon = new System.Data.Odbc.OdbcConnection(sConstr);
            System.Data.Odbc.OdbcCommand oCmd = new System.Data.Odbc.OdbcCommand();
            DataSet oDs = new DataSet();
            try
            {

                oCon.Open();
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandText = sQuery;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SQL Query : " + sQuery, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before adding Parameters", sFuncName);
                foreach (var item in param)
                {
                    oCmd.Parameters.Add(item);
                }
                //oCmd.Parameters.Add((param);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After adding parameters", sFuncName);

                oCmd.Connection = oCon;
                oCmd.CommandTimeout = 120;
                System.Data.Odbc.OdbcDataAdapter da = new System.Data.Odbc.OdbcDataAdapter(oCmd);
                da.Fill(oDs);
            }
            catch (Exception ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With Error", sFuncName);
                oCon.Dispose();
                throw new Exception(ex.Message);
            }
            return oDs.Tables[0];
        }


        #endregion
    }
}
