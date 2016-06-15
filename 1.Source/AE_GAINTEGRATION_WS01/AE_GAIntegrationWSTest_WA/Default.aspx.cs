using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using AE_GAIntegrationWSTest_WA.InventoryGoods;

namespace AE_GAIntegrationWSTest_WA
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
              //  InventoryGoodsSavingClient InGoodsSave = new InventoryGoodsSavingClient();     

                double dblQuan = Convert.ToDouble("0.0");

                if (!IsPostBack)
                {
                //    string FileName = @"C:\Users\Guna\Documents\INVENTORY_GOODS_ISSUE _Multiple.txt";
                //    var stream = File.OpenText(FileName);
                //    string jsonString = stream.ReadToEnd();

                //    string strResultJson = InGoodsSave.SaveTheInventoryGoodIssue(jsonString);


                //  DataTable dt = JsonStringToDataTable(jsonString);

                    var dt1I = creatingTempTableWithDatas();

                    string strval = GetJson(dt1I);

                    var dt1R = creatingTempTableWithDatasReceipt();

                    string strvalRec = GetJson(dt1R);    
                    
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString().Replace("\n","<br/>"));
            }
        }


        public DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] stringSeparators = new string[] { "\"INSTRUCTIONS\":" };

            string[] jsonStringArray1 = jsonString.Split(stringSeparators, StringSplitOptions.None);
            string[] jsonStringArray = Regex.Split(jsonStringArray1[1].Replace("[", "").Replace("]", "").Replace("\r\n","") , "},{");            
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                //string[] jsonsplit = 
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", "").Replace("\r\n", "").Replace("\t", "").Replace(" ",""), ",");

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
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", "").Replace("\r\n", "").Replace("\t", "").Replace(" ", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }

        public string GetJson(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new

            System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows =
              new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        DataTable creatingTempTableWithDatas()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemCode");
            dt.Columns.Add("Whscode");
            dt.Columns.Add("BatchCode");
            dt.Columns.Add("ReasonCode");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("PostingDate");
            dt.Columns.Add("DocumentDate");
            dt.Columns.Add("GLAccount");
            dt.Columns.Add("BusinessUnit");
            dt.Columns.Add("Remarks");
            dt.Columns.Add("JournalRemarks");
            dt.Columns.Add("Reference");

            dt.Rows.Add("ITEM0000000527","BLK_01","13/0433","001","2","6/12/2016","6/12/2016","510200100","DSG","","","REF123");
            dt.Rows.Add("ITEM0000000004","BLK_08","","002","3","6/11/2016","6/11/2016","510200100","QC","","","");
					



            //dt.Rows.Add("123fasda34", "123dadq", "xs21daf43sd", "001", "10", "06/06/2016 00:00:00", "6/6/2016 00:00:00", "jlk432jhy", "23", "dsfdsfdsfs", "dfsdfsfsdf","ggdf");
            //dt.Rows.Add("32534dfg", "9f3rf4t", "gcjsh", "003", "45", "06/09/2016 00:00:00", "6/09/2016 00:00:00", "34zxcvdzxv", "32", "dsfdsfdsfs", "dfsdfsfsdf", "ggdf");


            return dt;

        }


        DataTable creatingTempTableWithDatasReceipt()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemCode");
            dt.Columns.Add("Whscode");
            dt.Columns.Add("BatchCode");
            dt.Columns.Add("ReasonCode");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Price");            
            dt.Columns.Add("PostingDate");
            dt.Columns.Add("DocumentDate");
            dt.Columns.Add("GLAccount");
            dt.Columns.Add("BusinessUnit");
            dt.Columns.Add("Remarks");
            dt.Columns.Add("JournalRemarks");
            dt.Columns.Add("Reference");

            dt.Rows.Add("ITEM0000000527", "BLK_01", "LOT1", "001", "10","13", "6/12/2016", "6/12/2016", "510200100", "DSG", "", "", "REF123");
            dt.Rows.Add("ITEM0000000004", "BLK_08", "", "002", "20","12", "6/11/2016", "6/11/2016", "510200100", "QC", "", "", "");
            			
            		


            return dt;

        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {                       

                try
                {

                    InventoryClient InGoodsSave = new InventoryClient();

                    if (FileUploadControl.HasFile)
                    {

                        //string filename = Path.GetFileName(FileUploadControl.FileName);
                        //string str = Path.GetFullPath(FileUploadControl.FileName);
                        //string str1 = Path.GetFullPath(Convert.ToString(FileUploadControl.PostedFile.FileName));
                        //FileUploadControl.SaveAs(Server.MapPath("~/") + filename);


                        string jsonString = string.Empty; string strResultJson = string.Empty;
                        using (StreamReader inputStreamReader = new StreamReader(FileUploadControl.PostedFile.InputStream))
                        {
                            jsonString = inputStreamReader.ReadToEnd();
                        }


                       // var tableResult = JsonStringToDataTable(jsonString);

                        //string FileName = @"C:\Users\Guna\Documents\INVENTORY_GOODS_ISSUE _Multiple.txt";
                        //var stream = File.OpenText(FileName);
                        //string jsonString = stream.ReadToEnd();

                       if (jsonString.Contains("INVENTORY_GOODS_ISSUE")){

                        strResultJson = InGoodsSave.SaveInventoryGoodsIssue(jsonString);
                       }
                       else
                       {
                           strResultJson = InGoodsSave.SaveInventoryGoodsRecipet(jsonString);
                       }


                      //  DataTable dt = JsonStringToDataTable(jsonString);

                        //  var dt11 = creatingTempTableWithDatas();

                       // string strval = GetJson(dt);

                       txtResult.Text = strResultJson.ToString();
                       StatusLabel.ForeColor = System.Drawing.Color.Green;
                       StatusLabel.Text = "SUCCESS";
                    }

                }
                catch (Exception ex)
                {
                    StatusLabel.ForeColor = System.Drawing.Color.Red;
                    StatusLabel.Text = "Upload status: The file could not be uploaded. The following error occured: " + ex.Message;
                    Response.Write(ex.Message.ToString().Replace("\n", "<br/>"));
                    
                }



            


           





        }










    }
}
