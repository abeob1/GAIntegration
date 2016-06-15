using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Data;
using System.Configuration;

namespace AE_GAINTEGRATION_WS01
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Inventory" in code, svc and config file together.
    public class Inventory : IInventory
    {
        #region Variable declarations
        SAPbobsCOM.Company oDICompany = null;
        clsLog oLog = new clsLog();
        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public string sErrDesc = string.Empty;
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["sqlConnection"].ConnectionString;
        public static string ConnString = ConfigurationManager.ConnectionStrings["sapConnection"].ConnectionString;
        #endregion

        #region Public Methods

        /// <summary>
        /// This function is using to save Inventory Goods Issue in Database
        /// </summary>
        /// <param name="jsonString">Input is JSon string </param>
        /// <returns>return Output is string type(Json string) </returns>
        public string SaveInventoryGoodsIssue(string jsonString)
        {
            string sFuncName = "SaveTheInventoryGoodIssue";
            //jsonString == "{"ARC_ORDER": {"ORDER": "INVENTORY_GOODS_ISSUE_DEBIT","INSTRUCTIONS": {"ItemCode": "123fasda34mnd65s","Whscode": "123dadq","BatchCode": "xs21daf43sd","ReasonCode": 001,"Quantity": 10,"PostingDate": "06/06/2016 00:00:00","DocumentDate": "6/6/2016 00:00:00","GLAccount": "jlk432jhy","BusinessUnit": "23","Remarks": "dsfdsfdsfs","JournalRemarks": "dfsdfsfsdf","Reference": "ggdf"}}}";

            //jsonString == "{"ARC_ORDER": {"ORDER": "INVENTORY_GOODS_ISSUE_DEBIT","INSTRUCTIONS": [{"ItemCode":"123fasda34","Whscode":"123dadq","BatchCode":"xs21daf43sd","ReasonCode":"001","Quantity":"10","PostingDate":"06/06/2016 00:00:00","DocumentDate":"6/6/2016 00:00:00","GLAccount":"jlk432jhy","BusinessUnit":"23","Remarks":"dsfdsfdsfs","JournalRemarks":"dfsdfsfsdf","Reference":"ggdf"},{"ItemCode":"32534dfg","Whscode":"9f3rf4t","BatchCode":"gcjsh","ReasonCode":"003","Quantity":"45","PostingDate":"06/09/2016 00:00:00","DocumentDate":"6/09/2016 00:00:00","GLAccount":"34zxcvdzxv","BusinessUnit":"32","Remarks":"dsfdsfdsfs","JournalRemarks":"dfsdfsfsdf","Reference":"ggdf"}]}};

            // orginal data string   ITEM0000000527
            //jsonString = "{"ARC_ORDER": {"ORDER": "INVENTORY_GOODS_ISSUE_DEBIT","INSTRUCTIONS": [{"ItemCode":"ITEM0000000527","Whscode":"BLK_01","BatchCode":"13/0433","ReasonCode":"001","Quantity":"2","PostingDate":"6/12/2016","DocumentDate":"6/12/2016","GLAccount":"510200100","BusinessUnit":"DSG","Remarks":"","JournalRemarks":"","Reference":"REF123"},{"ItemCode":"ITEM0000000004","Whscode":"BLK_08","BatchCode":"","ReasonCode":"002","Quantity":"3","PostingDate":"6/11/2016","DocumentDate":"6/11/2016","GLAccount":"510200100","BusinessUnit":"QC","Remarks":"","JournalRemarks":"","Reference":""}]}}";

            oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);
            string strJson = string.Empty, strValidationMsg = string.Empty, strDBValidationMsg = string.Empty;
            try
            {
                var dtLocal = JsonStringToDataTable(jsonString);

                strValidationMsg = MandatoryMaxLengthValidation(dtLocal);
                if (strValidationMsg.Contains("SUCCESS"))
                {
                    strDBValidationMsg = FieldsCheckingInDatabase_GoodsIssue(dtLocal);
                }

                if (strValidationMsg.Contains("SUCCESS") && strDBValidationMsg.Contains("SUCCESS"))
                {
                    oLog.WriteToLogFile_Debug("Before converting datatable ", sFuncName);
                    // JEEVA code here
                    var dtResult = InvGoodsIssue(dtLocal);
                    strJson = GetJsonFromDataTable(dtResult);

                    oLog.WriteToLogFile_Debug("After converting datatable ", sFuncName);
                }
                else
                {
                    throw new FaultException(string.Format("Inventory Goods Issue insertion failed. \n {0} \n\n {1} ",
                        (strValidationMsg.Contains("SUCCESS") ? "" : strValidationMsg.ToString()),
                        (strDBValidationMsg.Contains("SUCCESS") ? "" : strDBValidationMsg.ToString())));
                }


                return strJson;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message.ToString());
            }
        }

        /// <summary>
        /// This function is using to save Inventory Goods Receipt in Database
        /// </summary>
        /// <param name="jsonString">Input is JSon string </param>
        /// <returns>return Output is string type(Json string) </returns>
        public string SaveInventoryGoodsRecipet(string jsonString)
        {
            //jsonString == "{"ARC_ORDER": {"ORDER": "INVENTORY_GOODS_ISSUE_RECEIPT","INSTRUCTIONS": {"ItemCode": "123","Whscode": "123","BatchCode": 123,"ReasonCode": 002,"Quantity": 10,"Price": 1542.00,"PostingDate": "06/06/2016 00:00:00","DocumentDate": "6/6/2016 00:00:00","GLAccount": "jlk432jhy","BusinessUnit": "23","Remarks": "dsfdsfdsfs","JournalRemarks": "dfsdfsfsdf","Reference": "ggdf"}}}";

            // orginal data string
            //jsonString = "{"ARC_ORDER": {"ORDER": "INVENTORY_GOODS_RECEIPT","INSTRUCTIONS": [{"ItemCode":"ITEM0000000527","Whscode":"BLK_01","BatchCode":"LOT1","ReasonCode":"001","Quantity":"10","Price":"13","PostingDate":"6/12/2016","DocumentDate":"6/12/2016","GLAccount":"510200100","BusinessUnit":"DSG","Remarks":"","JournalRemarks":"","Reference":"REF123"},{"ItemCode":"ITEM0000000004","Whscode":"BLK_08","BatchCode":"","ReasonCode":"002","Quantity":"20","Price":"12","PostingDate":"6/11/2016","DocumentDate":"6/11/2016","GLAccount":"510200100","BusinessUnit":"QC","Remarks":"","JournalRemarks":"","Reference":""}]}}";


            //{"ARC_ORDER": {"ORDER": "INVENTORY_GOODS_RECEIPT","INSTRUCTIONS": [{"ItemCode":"ITEM0000000527","Whscode":"BLK_01","BatchCode":"LOT1","ReasonCode":"001","Quantity":"10","Price":"13","PostingDate":"6/12/2016","DocumentDate":"6/12/2016","GLAccount":"510200100","BusinessUnit":"DSG","Remarks":"","JournalRemarks":"","Reference":"REF123"},{"ItemCode":"ITEM0000000004","Whscode":"BLK_08","BatchCode":"","ReasonCode":"002","Quantity":"20","Price":"12","PostingDate":"6/11/2016","DocumentDate":"6/11/2016","GLAccount":"510200100","BusinessUnit":"QC","Remarks":"","JournalRemarks":"","Reference":""},{"ItemCode":"ITEM0000009414","Whscode":"BLK_08","BatchCode":"ABC123","ReasonCode":"002","Quantity":"20","Price":"12","PostingDate":"6/11/2016","DocumentDate":"6/11/2016","GLAccount":"510600600","BusinessUnit":"QC","Remarks":"Remarks","JournalRemarks":"Journal remarks","Reference":"ref"}]}}


            string strJson = string.Empty, strValidationMsg = string.Empty, strDBValidationMsg = string.Empty;
            try
            {
                var dtLocal = JsonStringToDataTable(jsonString);

                oLog.WriteToLogFile("before MandatoryMaxLengthValidation :" + dtLocal.Rows.Count.ToString(), "SaveTheInventoryGoodRecipet()");

                strValidationMsg = MandatoryMaxLengthValidation(dtLocal);
                oLog.WriteToLogFile("After MandatoryMaxLengthValidation :" + dtLocal.Rows.Count.ToString(), "SaveTheInventoryGoodRecipet()");
                if (strValidationMsg.Contains("SUCCESS"))
                {
                    strDBValidationMsg = FieldsCheckingInDatabase_GoodsReceipt(dtLocal);
                }

                if (strValidationMsg.Contains("SUCCESS") && strDBValidationMsg.Contains("SUCCESS"))
                {
                    // JEEVA code here   
                    oLog.WriteToLogFile_Debug("Datatable rows count: " + dtLocal.Rows.Count.ToString(), "SaveTheInventoryGoodRecipet()");
                    strJson = InvGoodsReceipt(dtLocal) == "SUCCESS" ? "0" : strJson;


                }
                else
                {
                    throw new FaultException(string.Format("Inventory Goods Receipt insertion failed. \n {0} \n\n {1} ",
                        (strValidationMsg.Contains("SUCCESS") ? "" : strValidationMsg.ToString()),
                        (strDBValidationMsg.Contains("SUCCESS") ? "" : strDBValidationMsg.ToString())));
                }


                return strJson;
            }
            catch (Exception ex)
            {
                oLog.WriteToLogFile("Error :" + ex.StackTrace.ToString(), "SaveTheInventoryGoodRecipet()");
                throw new FaultException(ex.Message.ToString());
            }
        }

        #endregion

        #region Private Methods
        DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] stringSeparators = new string[] { "\"INSTRUCTIONS\":" };


            string[] jsonStringArray1 = jsonString.Split(stringSeparators, StringSplitOptions.None);
            //jsonStringArray1[0].Split(':')[2].Replace("\"","").Replace(",","")

            if (jsonStringArray1.Length > 0)
            {
                oLog.WriteToLogFile("jsonString :" + jsonStringArray1[0].ToString() + "\n\n", "JsonStringToDataTable()");
                oLog.WriteToLogFile("jsonString :" + jsonStringArray1[1].ToString() + "\n\n", "JsonStringToDataTable()");
            }

            string[] jsonStringArray = Regex.Split(jsonStringArray1[1].Replace("[", "").Replace("]", "").Replace("\r\n", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", "").Replace("\r\n", "").Replace("\t", "").Replace(" ", ""), ",");

                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", "").Replace("\r\n", "").Replace("\t", ""), ",");
                DataRow nr = dt.NewRow();

                oLog.WriteToLogFile("Row data  :" + string.Join(",", RowData).ToString(), "JsonStringToDataTable()");
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "").Replace(" ", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        oLog.WriteToLogFile("Row adding to table :" + ex.StackTrace.ToString(), "JsonStringToDataTable()");
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }

            oLog.WriteToLogFile("Table rows count :" + dt.Rows.Count.ToString(), "JsonStringToDataTable()");
            return dt;
        }

        string GetJsonFromDataTable(DataTable dtLocal)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new

            System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows =
              new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            foreach (DataRow dr in dtLocal.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dtLocal.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        string MandatoryMaxLengthValidation(DataTable dtLocal)
        {
            string strInfoMsg = string.Empty;
            InventoryGoodsIssue objIGI = new InventoryGoodsIssue();
            InventoryGoodsReceipt objIGR = new InventoryGoodsReceipt();
            int rIntex = 0;
            if (dtLocal != null && dtLocal.Rows.Count > 0)
            {
                foreach (DataRow drow in dtLocal.Rows)
                {
                    oLog.WriteToLogFile("rows no :" + rIntex.ToString(), "MandatoryMaxLengthValidation()");
                    foreach (DataColumn dc in drow.Table.Columns)
                    {
                        oLog.WriteToLogFile("ColumnsName :" + dc.ColumnName.Trim().ToUpper(), "MandatoryMaxLengthValidation()");
                        try
                        {
                            switch (dc.ColumnName.Trim().ToUpper())
                            {
                                case "ITEMCODE": objIGI.ItemCode = dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(); break;
                                case "WHSCODE": objIGI.Whscode = dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(); break;
                                case "BATCHCODE":
                                    oLog.WriteToLogFile("BATCHCODE :" + dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(), "MandatoryMaxLengthValidation()");
                                    if (dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim() != string.Empty)
                                    {
                                        oLog.WriteToLogFile("BATCHCODE con.. true :" + dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(), "MandatoryMaxLengthValidation()");
                                        objIGI.BatchCode = dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim();
                                    }
                                    break;
                                case "REASONCODE":
                                    if (dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim() != string.Empty)
                                    { objIGI.ReasonCode = dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(); } break;
                                case "QUANTITY":
                                    double dblQuan = Convert.ToDouble(dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString()); objIGI.Quantity = dblQuan;
                                    if (dblQuan < 1)
                                    {
                                        throw new FaultException("Quantity should be greater than 0");
                                    }
                                    break;
                                case "PRICE":
                                    if (dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim() != string.Empty)
                                    {
                                        double dblPrice = Convert.ToDouble(dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString()); objIGR.Price = dblPrice;
                                    }
                                    break;
                                case "POSTINGDATE":
                                    if (dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim() == string.Empty)
                                    {
                                        throw new FaultException("Posting date should not empty");
                                    }
                                    else
                                    {
                                        objIGI.PostingDate = Convert.ToDateTime(dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim());
                                    }
                                    break;
                                case "DOCUMENTDATE":
                                    if (dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim() == string.Empty)
                                    {
                                        throw new FaultException("Document date should not empty");
                                    }
                                    else
                                    {
                                        objIGI.DocumentDate = Convert.ToDateTime(dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim());
                                    }
                                    break;
                                case "GLACCOUNT": objIGI.GLAccount = dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(); break;
                                case "BUSINESSUNIT": objIGI.BusinessUnit = dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(); break;
                                case "REMARKS": objIGI.Remarks = dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(); break;
                                case "JOURNALREMARKS": objIGI.JournalRemarks = dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(); break;
                                case "REFERENCE": objIGI.Reference = dtLocal.Rows[rIntex][dc.ColumnName.Trim()].ToString().Trim(); break;

                            }
                        }
                        catch (Exception ex)
                        {
                            oLog.WriteToLogFile("Error msg :" + ex.Message.ToString(), "MandatoryMaxLengthValidation()");
                            strInfoMsg += string.Format("{0} \n", ex.Message.ToString());
                            continue;
                        }
                    }
                    rIntex++;
                }
            }
            else
            {
                strInfoMsg = "No records found in Datatable..!";
            }

            strInfoMsg = strInfoMsg.Trim() == string.Empty ? "SUCCESS" : strInfoMsg;
            return strInfoMsg;

        }

        string FieldsCheckingInDatabase_GoodsIssue(DataTable dtLocal)
        {
            try
            {
                string strInfoMsg = string.Empty;
                clsCommon objDataProvider = new clsCommon();
                DataTable dtTemp = new DataTable(); string strQry = string.Empty;

                foreach (DataRow drow in dtLocal.Rows)
                {

                    foreach (DataColumn dc in drow.Table.Columns)
                    {
                        try
                        {
                            switch (dc.ColumnName.Trim().ToUpper())
                            {
                                case "ITEMCODE":
                                    strQry = string.Format("select * from OITM where ItemCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("Item Code not available in Database"); }
                                    break;
                                case "WHSCODE":
                                    strQry = string.Format("select * from OWHS where WhsCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("WhsCode not available in Database"); }
                                    break;
                                case "BATCHCODE":
                                    strQry = string.Format("SELECT ManBtchNum FROM OITM WHERE ItemCode =  '{0}'", dtLocal.Rows[0]["ITEMCODE"].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);

                                    if (dtTemp == null || dtTemp.Rows.Count == 0)
                                    {
                                        if (dtLocal.Rows[0]["ManBtchNum"].ToString().ToUpper().Trim().Equals("Y"))
                                        {
                                            strQry = string.Format("select BatchNum,ItemCode from OIBT where ItemCode =  '{0}' and BatchNum ='{1}'",
                                                dtLocal.Rows[0]["ITEMCODE"].ToString().Trim(), dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                            dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                            if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("Batch Code not available in Database"); }
                                        }
                                    }
                                    break;
                                //case "REASONCODE":
                                //    strQry = string.Format("select * from OPRC where PrcCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                //    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                //    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("Reason Code not available in Database"); }
                                //    break;
                                //case "QUANTITY":
                                //    strQry = string.Format("select ItemCode,whscode,OnHand from OITW where ItemCode =  '{0}' and whscode ='{1}'",
                                //        dtLocal.Rows[0]["ITEMCODE"].ToString().Trim(), dtLocal.Rows[0]["WHSCODE"].ToString().Trim());
                                //    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                //    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("Quantity not available in Database"); }
                                //    break;

                                //case "POSTINGDATE": IGI.PostingDate = Convert.ToDateTime(dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim()); break;
                                //case "DOCUMENTDATE": IGI.DocumentDate = Convert.ToDateTime(dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim()); break;

                                case "GLACCOUNT":
                                    strQry = string.Format("select * from OACT where AcctCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("GL ACCOUNT not available in Database"); }
                                    break;
                                case "BUSINESSUNIT":
                                    strQry = string.Format("select OcrCode from OOCR where OcrCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("BUSINESS UNIT not available in Database"); }
                                    break;



                            }
                        }
                        catch (Exception ex)
                        {

                            strInfoMsg += string.Format("{0} \n", ex.Message.ToString());
                            continue;
                        }
                    }

                }

                strInfoMsg = strInfoMsg.Trim() == string.Empty ? "SUCCESS" : strInfoMsg;


                dtTemp.Dispose();
                return strInfoMsg;

            }
            catch (Exception ex)
            {
                throw new FaultException(string.Format("Getting error in FieldsCheckingInDatabase_GoodsIssue function: \n {0}", ex.Message.ToString()));
            }
        }

        string FieldsCheckingInDatabase_GoodsReceipt(DataTable dtLocal)
        {
            try
            {
                string strInfoMsg = string.Empty;
                clsCommon objDataProvider = new clsCommon();
                DataTable dtTemp = new DataTable(); string strQry = string.Empty;

                foreach (DataRow drow in dtLocal.Rows)
                {

                    foreach (DataColumn dc in drow.Table.Columns)
                    {
                        try
                        {
                            switch (dc.ColumnName.Trim().ToUpper())
                            {
                                case "ITEMCODE":
                                    strQry = string.Format("select * from OITM where ItemCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("Item Code not available in Database"); }
                                    break;
                                case "WHSCODE":
                                    strQry = string.Format("select * from OWHS where WhsCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("WhsCode not available in Database"); }
                                    break;
                                case "BATCHCODE":
                                    strQry = string.Format("SELECT ManBtchNum FROM OITM WHERE ItemCode =  '{0}' ", dtLocal.Rows[0]["ITEMCODE"].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);

                                    if (dtTemp == null || dtTemp.Rows.Count == 0)
                                    {
                                        if (dtLocal.Rows[0]["ManBtchNum"].ToString().ToUpper().Trim().Equals("Y"))
                                        {
                                            strQry = string.Format("select BatchNum,ItemCode from OIBT where ItemCode =  '{0}' and BatchNum ='{1}'", dtLocal.Rows[0]["ITEMCODE"].ToString().Trim(), dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                            dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                            if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("Batch Code not available in Database"); }
                                        }
                                    }
                                    break;
                                //case "REASONCODE":
                                //    strQry = string.Format("select * from OPRC where PrcCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                //    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                //    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("Reason Code not available in Database"); }
                                //    break;
                                //case "QUANTITY":
                                //    break;
                                //case "POSTINGDATE":  break;
                                //case "DOCUMENTDATE": break;

                                case "GLACCOUNT":
                                    strQry = string.Format("select * from OACT where AcctCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("GLACCOUNT not available in Database"); }
                                    break;
                                case "BUSINESSUNIT":
                                    strQry = string.Format("select OcrCode from OOCR where OcrCode =  '{0}'", dtLocal.Rows[0][dc.ColumnName.Trim()].ToString().Trim());
                                    dtTemp = objDataProvider.ExecuteSelectQueryInSQLServer(strQry);
                                    if (dtTemp == null || dtTemp.Rows.Count == 0) { throw new FaultException("BUSINESS UNIT not available in Database"); }
                                    break;



                            }
                        }
                        catch (Exception ex)
                        {

                            strInfoMsg += string.Format("{0} \n", ex.Message.ToString());
                            continue;
                        }
                    }

                }

                strInfoMsg = strInfoMsg.Trim() == string.Empty ? "SUCCESS" : strInfoMsg;


                dtTemp.Dispose();
                return strInfoMsg;

            }
            catch (Exception ex)
            {
                throw new FaultException(string.Format("Getting error in FieldsCheckingInDatabase_GoodsReceipt function: \n {0}", ex.Message.ToString()));
            }
        }

        public SAPbobsCOM.Company ConnectToTargetCompany(string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            //SAPbobsCOM.Company oDICompany = new SAPbobsCOM.Company();
            string sConnString = string.Empty;
            DataView oDTView = new DataView();
            string[] MyArr = new string[7];


            try
            {
                sFuncName = "ConnectToTargetCompany()";

                //oSessionCompany = oSessionCompany +  sSessionUserName;
                // SAPbobsCOM.Company Convert.ToString(Session["sLoginUserName"]);
                //SAPbobsCOM.Company = sSessionUserName + oSessionCompany;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                // oDICompany = (SAPbobsCOM.Company)Session["SAPCompany"];

                MyArr = ConnString.Split(';');
                string DatabaseName, SQLUser, SQLPwd, SQLServer, LicenseServer, DbUserName, DbPassword = string.Empty;
                SQLServer = MyArr[0].ToString();
                DatabaseName = MyArr[1].ToString();
                SQLUser = MyArr[2].ToString();
                SQLPwd = MyArr[3].ToString();
                LicenseServer = MyArr[4].ToString();
                DbUserName = MyArr[5].ToString();
                DbPassword = MyArr[6].ToString();

                if (oDICompany != null)
                {
                    if (oDICompany.CompanyDB == sCompanyDB)
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("ODICompany Name " + oDICompany.CompanyDB, sFuncName);
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SCompanyDB " + sCompanyDB, sFuncName);
                        return oDICompany;
                    }

                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);

                sConnString = ConnectionString;

                oDICompany = ConnectToTargetCompany(oDICompany, DbUserName, DbPassword
                                   , DatabaseName, SQLServer, LicenseServer
                                   , SQLUser, SQLPwd, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                //SAPCompany(oDIComapny, sConnString);

                return oDICompany;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        public SAPbobsCOM.Company ConnectToTargetCompany(SAPbobsCOM.Company oCompany, string sUserName, string sPassword, string sDBName,
                                                        string sServer, string sLicServerName, string sDBUserName
                                                       , string sDBPassword, string sErrDesc)
        {
            string sFuncName = string.Empty;
            //SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
            long lRetCode;

            try
            {
                sFuncName = "ConnectToTargetCompany()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (oCompany != null)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Disconnecting the Company object - Company Name " + oCompany.CompanyName, sFuncName);
                    oCompany.Disconnect();
                }
                oCompany = new SAPbobsCOM.Company();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After Initializing Company Connection ", sFuncName);
                oCompany.Server = sServer;
                oCompany.LicenseServer = sLicServerName;
                oCompany.DbUserName = sDBUserName;
                oCompany.DbPassword = sDBPassword;
                oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English;
                oCompany.UseTrusted = false;
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;


                oCompany.CompanyDB = sDBName;// sDataBaseName;
                oCompany.UserName = sUserName;
                oCompany.Password = sPassword;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Connecting the Database...", sFuncName);

                lRetCode = oCompany.Connect();

                if (lRetCode != 0)
                {

                    throw new ArgumentException(oCompany.GetLastErrorDescription());
                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Company Connection Established", sFuncName);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                    return oCompany;
                }

            }
            catch (Exception Ex)
            {

                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        //SAPbobsCOM.Company ConnectToTargetCompany()
        //{
        //    string sFuncName = string.Empty;
        //    string sReturnValue = string.Empty;
        //    DataSet oDTCompanyList = new DataSet();
        //    DataSet oDSResult = new DataSet();
        //    //SAPbobsCOM.Company oDICompany = new SAPbobsCOM.Company();
        //    string sConnString = string.Empty;
        //    DataView oDTView = new DataView();
        //    string[] MyArr = new string[7];
        //    long lRetCode;

        //    try
        //    {
        //        sFuncName = "ConnectToTargetCompany()";
        //        oLog.WriteToLogFile_Debug("begin : " + connectionString, sFuncName);

        //        MyArr = connectionString.Split(';');
        //        string DatabaseName, SQLUser, SQLPwd, SQLServer, LicenseServer, DbUserName, DbPassword = string.Empty;
        //        SQLServer = MyArr[0].ToString();
        //        DatabaseName = MyArr[1].ToString();
        //        SQLUser = MyArr[2].ToString();
        //        SQLPwd = MyArr[3].ToString();
        //        LicenseServer = MyArr[4].ToString();
        //        DbUserName = MyArr[5].ToString();
        //        DbPassword = MyArr[6].ToString();

        //        oLog.WriteToLogFile_Debug("before oDICompany connecting" , sFuncName);

        //        oDICompany = new SAPbobsCOM.Company();
        //        oDICompany.Server = SQLServer;
        //        oDICompany.LicenseServer = LicenseServer;
        //        oDICompany.DbUserName = SQLUser;
        //        oDICompany.DbPassword = SQLPwd;
        //        oDICompany.language = SAPbobsCOM.BoSuppLangs.ln_English;
        //        oDICompany.UseTrusted = false;
        //        oDICompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;

        //        oLog.WriteToLogFile_Debug("after oDICompany connecting", sFuncName);

        //        oDICompany.CompanyDB = DatabaseName;// sDataBaseName;
        //        oDICompany.UserName = DbUserName;
        //        oDICompany.Password = DbPassword;

        //        lRetCode = oDICompany.Connect();
        //        if (lRetCode != 0)
        //        {

        //            throw new ArgumentException(oDICompany.GetLastErrorDescription());
        //        }
        //        else
        //        {
        //            return oDICompany;
        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }

        //}

        DataTable InvGoodsIssue(DataTable oDatatable)
        {
            string strMsg = string.Empty;
            string sFuncName = string.Empty;
            int iCount;
            string sSQL = string.Empty;
            SAPbobsCOM.Recordset oRecordSet;
            string sBatch = string.Empty;
            DataTable oOutDatatable = new DataTable();

            try
            {
                sFuncName = "InvGoodsIssue";

                if (oDatatable != null && oDatatable.Rows.Count > 0)
                {
                    oLog.WriteToLogFile_Debug("before connecting ConnectToTargetCompany ", sFuncName);
                    oDICompany = ConnectToTargetCompany(ConnectionString);
                    oLog.WriteToLogFile_Debug("After connecting ConnectToTargetCompany ", sFuncName);
                    if (oDICompany.Connected)
                    {
                        String sSAPDBName = oDICompany.CompanyDB;

                        oLog.WriteToLogFile_Debug("Company DB Name: " + sSAPDBName.ToString(), sFuncName);
                        SAPbobsCOM.Documents oGoodsIssue = null;
                        oGoodsIssue = (SAPbobsCOM.Documents)oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);
                        //  oGoodsIssue = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit));

                        iCount = 1;
                        foreach (DataRow odr in oDatatable.Rows)
                        {
                            oGoodsIssue.DocDate = Convert.ToDateTime(odr["PostingDate"].ToString());
                            oGoodsIssue.TaxDate = Convert.ToDateTime(odr["DocumentDate"].ToString());
                            oGoodsIssue.Reference1 = odr["Reference"].ToString();
                            oGoodsIssue.Comments = odr["Remarks"].ToString();
                            oGoodsIssue.JournalMemo = odr["JournalRemarks"].ToString();
                            oGoodsIssue.UserFields.Fields.Item("U_REASONCODE").Value = odr["ReasonCode"].ToString();

                            if (iCount > 1)
                            {
                                oGoodsIssue.Lines.Add();
                            }
                            oGoodsIssue.Lines.ItemCode = odr["ItemCode"].ToString();
                            oGoodsIssue.Lines.WarehouseCode = odr["Whscode"].ToString();
                            oGoodsIssue.Lines.Quantity = Convert.ToDouble(odr["Quantity"].ToString());
                            oGoodsIssue.Lines.AccountCode = odr["GLAccount"].ToString();
                            oGoodsIssue.Lines.CostingCode = odr["BusinessUnit"].ToString();

                            sSQL = "SELECT ManBtchNum FROM OITM WHERE ItemCode = '" + odr["ItemCode"].ToString() + "' ";
                            oRecordSet = (SAPbobsCOM.Recordset)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset));
                            oRecordSet.DoQuery(sSQL);
                            if (oRecordSet.RecordCount > 0)
                            {
                                sBatch = oRecordSet.Fields.Item("ManBtchNum").Value;
                            }
                            else
                            {
                                sBatch = "N";
                            }

                            if (sBatch == "Y")
                            {
                                if (odr["BatchCode"].ToString() != string.Empty)
                                {
                                    sSQL = "SELECT T0.ItemCode, T0.DistNumber,SUM(T1.Quantity) [Quantity] FROM OBTN T0 ";
                                    sSQL += " INNER JOIN OBTQ T1 ON T0.AbsEntry = T1.MdAbsEntry AND T0.SysNumber = T1.SysNumber ";
                                    sSQL += " WHERE T0.ItemCode = '" + odr["ItemCode"].ToString() + "' AND T1.WhsCode = '" + odr["Whscode"].ToString() + "' ";
                                    sSQL += " AND T0.DistNumber = '" + odr["BatchCode"].ToString() + "' ";
                                    sSQL += " GROUP BY T0.DistNumber,T0.ItemCode HAVING SUM(T1.Quantity) > 0 ";
                                    oRecordSet.DoQuery(sSQL);
                                    if (oRecordSet.RecordCount > 0)
                                    {
                                        oGoodsIssue.Lines.BatchNumbers.BatchNumber = odr["BatchCode"].ToString();
                                        oGoodsIssue.Lines.BatchNumbers.Quantity = Convert.ToDouble(odr["Quantity"].ToString());
                                    }
                                }
                            }
                            iCount = iCount + 1;
                        }
                        oLog.WriteToLogFile_Debug("before oGoodsIssue.Add()", sFuncName);

                        if (oGoodsIssue.Add() == 0)
                        {
                            string sDocEntry = string.Empty;
                            sDocEntry = oDICompany.GetNewObjectKey();
                            oLog.WriteToLogFile_Debug("New Docentry " + sDocEntry, sFuncName);
                            oOutDatatable = oDatatable.Copy();
                            oOutDatatable.Columns.Remove("ReasonCode");
                            oOutDatatable.Columns.Remove("Remarks");
                            oOutDatatable.Columns.Remove("JournalRemarks");
                            oOutDatatable.Columns.Remove("Reference");
                            oOutDatatable.Columns.Add("ItemCost", typeof(System.Double));

                            foreach (DataRow dRow in oOutDatatable.Rows)
                            {
                                string sItemCode = dRow["ItemCode"].ToString();

                                sSQL = "SELECT Price FROM IGE1 WHERE ItemCode = '" + sItemCode + "'";
                                oRecordSet = (SAPbobsCOM.Recordset)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset));
                                oRecordSet.DoQuery(sSQL);
                                if (oRecordSet.RecordCount > 0)
                                {
                                    dRow["ItemCost"] = oRecordSet.Fields.Item("Price").Value;
                                }
                                else
                                {
                                    dRow["ItemCost"] = 0;
                                }
                            }

                            strMsg = "SUCCESS";
                        }
                        else
                        {
                            if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            strMsg = oDICompany.GetLastErrorDescription();
                            oLog.WriteToLogFile_Debug("" + strMsg, sFuncName);
                            throw new ArgumentException(strMsg);
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                strMsg = Ex.Message.ToString();
                throw Ex;
            }
            return oOutDatatable;
        }

        string InvGoodsReceipt(DataTable oDatatable)
        {
            string strMsg = string.Empty;
            string sFuncName = string.Empty;
            int iCount;
            string sSQL = string.Empty;
            SAPbobsCOM.Recordset oRecordSet;
            string sBatch = string.Empty;

            try
            {
                sFuncName = "InvGoodsReceipt";

                if (oDatatable != null && oDatatable.Rows.Count > 0)
                {
                    oDICompany = ConnectToTargetCompany(ConnectionString);
                    if (oDICompany.Connected)
                    {
                        SAPbobsCOM.Documents oGoodsReceipt = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry));

                        iCount = 1;
                        foreach (DataRow odr in oDatatable.Rows)
                        {
                            oGoodsReceipt.DocDate = Convert.ToDateTime(odr["PostingDate"].ToString());
                            oGoodsReceipt.TaxDate = Convert.ToDateTime(odr["DocumentDate"].ToString());
                            oGoodsReceipt.Reference1 = odr["Reference"].ToString();
                            oGoodsReceipt.Comments = odr["Remarks"].ToString();
                            oGoodsReceipt.JournalMemo = odr["JournalRemarks"].ToString();
                            oGoodsReceipt.UserFields.Fields.Item("U_REASONCODE").Value = odr["ReasonCode"].ToString();

                            if (iCount > 1)
                            {
                                oGoodsReceipt.Lines.Add();
                            }
                            oLog.WriteToLogFile_Debug("ItemCode Receipt : " + odr["ItemCode"].ToString(), sFuncName);
                            oGoodsReceipt.Lines.ItemCode = odr["ItemCode"].ToString();
                            oGoodsReceipt.Lines.WarehouseCode = odr["Whscode"].ToString();
                            oGoodsReceipt.Lines.Quantity = Convert.ToDouble(odr["Quantity"].ToString());
                            oGoodsReceipt.Lines.Price = Convert.ToDouble(odr["Price"].ToString());
                            oGoodsReceipt.Lines.AccountCode = odr["GLAccount"].ToString();
                            oGoodsReceipt.Lines.CostingCode = odr["BusinessUnit"].ToString();

                            sSQL = "SELECT ManBtchNum FROM OITM WHERE ItemCode = '" + odr["ItemCode"].ToString() + "' ";
                            oRecordSet = (SAPbobsCOM.Recordset)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset));
                            oRecordSet.DoQuery(sSQL);

                            sBatch = (oRecordSet.RecordCount > 0) ? oRecordSet.Fields.Item("ManBtchNum").Value : "N";

                            oLog.WriteToLogFile_Debug("BatchCode : " + sBatch, sFuncName);

                            if (sBatch.ToUpper() == "Y")
                            {
                                if (odr["BatchCode"].ToString() != string.Empty)
                                {
                                    oGoodsReceipt.Lines.BatchNumbers.BatchNumber = odr["BatchCode"].ToString();
                                    oGoodsReceipt.Lines.BatchNumbers.Quantity = Convert.ToDouble(odr["Quantity"].ToString());
                                    oGoodsReceipt.Lines.BatchNumbers.AddmisionDate = Convert.ToDateTime(odr["PostingDate"].ToString());
                                }
                            }
                            iCount = iCount + 1;
                        }

                        oLog.WriteToLogFile_Debug("before oGoodsReceipt.Add() : " + sBatch, sFuncName);
                        if (oGoodsReceipt.Add() == 0)
                        {
                            string sDocEntry = string.Empty;
                            sDocEntry = oDICompany.GetNewObjectKey();
                            strMsg = "SUCCESS";
                            oLog.WriteToLogFile_Debug("Success oGoodsReceipt.Add() : " + sBatch, sFuncName);
                        }
                        else
                        {
                            oLog.WriteToLogFile_Debug("Else oGoodsReceipt.Add() : " + sBatch, sFuncName);
                            if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            strMsg = oDICompany.GetLastErrorDescription(); throw new ArgumentException(strMsg);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                strMsg = Ex.Message.ToString();
                throw Ex;
            }
            return strMsg;
        }






        #endregion
    }
}
