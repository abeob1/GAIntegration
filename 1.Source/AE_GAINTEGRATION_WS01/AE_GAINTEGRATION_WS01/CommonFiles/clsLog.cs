using System;
using System.IO;
using System.Reflection;

namespace AE_GAINTEGRATION_WS01
{
    public class clsLog
    {
        #region Objects
        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public Int16 p_iErrDispMethod;
        public Int16 p_iDeleteDebugLog = 0;
        public string p_sLogDir;
        public string path;
        private const Int16 MAXFILESIZE_IN_MB = 5;
        private const string LOG_FILE_ERROR = "ErrorLog";
        private const string LOG_FILE_ERROR_ARCH = "ErrorLog_";
        private const string LOG_FILE_DEBUG = "DebugLog";
        private const string LOG_FILE_DEBUG_ARCH = "DebugLog_";
        private const Int16 FILE_SIZE_CHECK_ENABLE = 1;
        private const Int16 FILE_SIZE_CHECK_DISABLE = 0;
        #endregion

        #region Methods
        public long WriteToLogFile_Debug(string strErrText, string strSourceName, Int16 intCheckFileForDelete = 1)
        {
            long functionReturnValue = 0;

            StreamWriter oStreamWriter = null;
            string strFileName = string.Empty;
            string strArchFileName = string.Empty;
            string strTempString = string.Empty;

            double lngFileSizeInMB = 0;

            try
            {

                if (strSourceName.Length > 30)
                    strTempString = strTempString.PadLeft(0);
                else
                    strTempString = strTempString.PadLeft(30 - strSourceName.Length);

                strSourceName = strTempString.ToString().Trim() + strSourceName.Trim();

                strErrText = "[" + string.Format(DateTime.Now.ToString(), "MM/dd/yyyy HH:mm:ss") + "]" + "[" + strSourceName + "] " + strErrText;

                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string Datapath = Uri.UnescapeDataString(uri.Path);
                path = Path.GetDirectoryName(Datapath);

                strFileName = path + "\\" + LOG_FILE_DEBUG + ".log";
                strArchFileName = path + "\\" + LOG_FILE_DEBUG_ARCH + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";

                if (intCheckFileForDelete == FILE_SIZE_CHECK_ENABLE)
                {
                    if (File.Exists(strFileName))
                    {
                        FileInfo fi = new FileInfo(strFileName);

                        lngFileSizeInMB = (fi.Length / 1024) / 1024;

                        if (lngFileSizeInMB >= MAXFILESIZE_IN_MB)
                        {
                            if (p_iDeleteDebugLog == 1)
                            {
                                foreach (string sFileName in Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), LOG_FILE_DEBUG_ARCH + "*"))
                                {
                                    File.Delete(sFileName);
                                }
                            }
                            File.Move(strFileName, strArchFileName);
                        }
                    }
                }
                oStreamWriter = File.AppendText(strFileName);
                oStreamWriter.WriteLine(strErrText);
                functionReturnValue = RTN_SUCCESS;
            }
            catch (Exception)
            {
                functionReturnValue = RTN_ERROR;
            }
            finally
            {
                if ((oStreamWriter != null))
                {
                    oStreamWriter.Flush();
                    oStreamWriter.Close();
                    oStreamWriter = null;
                }
            }
            return functionReturnValue;

        }

        public long WriteToLogFile(string strErrText, string strSourceName, Int16 intCheckFileForDelete = 1)
        {
            long functionReturnValue = 0;

            StreamWriter oStreamWriter = null;
            string strFileName = string.Empty;
            string strArchFileName = string.Empty;
            double lngFileSizeInMB = 0;
            string strTempString = string.Empty;

            try
            {
                if (strSourceName.Length > 30)
                    strTempString = strTempString.PadLeft(0);
                else
                    strTempString = strTempString.PadLeft(30 - strSourceName.Length);

                strSourceName = strTempString.ToString() + strSourceName;

                strErrText = "[" + string.Format(DateTime.Now.ToString(), "MM/dd/yyyy HH:mm:ss") + "]" + "[" + strSourceName + "] " + strErrText;

                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string Datapath = Uri.UnescapeDataString(uri.Path);
                path = Path.GetDirectoryName(Datapath);

                strFileName = path + "\\" + LOG_FILE_ERROR + ".log";
                strArchFileName = path + "\\" + LOG_FILE_ERROR_ARCH + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";

                if (intCheckFileForDelete == FILE_SIZE_CHECK_ENABLE)
                {
                    if (File.Exists(strFileName))
                    {
                        FileInfo fi = new FileInfo(strFileName);

                        lngFileSizeInMB = (fi.Length / 1024) / 1024;

                        if (lngFileSizeInMB >= MAXFILESIZE_IN_MB)
                        {
                            if (p_iDeleteDebugLog == 1)
                            {
                                foreach (string sFileName in Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), LOG_FILE_ERROR_ARCH + "*"))
                                {
                                    File.Delete(sFileName);
                                }
                            }
                            File.Move(strFileName, strArchFileName);
                        }
                    }
                }
                oStreamWriter = File.AppendText(strFileName);
                oStreamWriter.WriteLine(strErrText);
                functionReturnValue = RTN_SUCCESS;
            }
            catch (Exception)
            {
                functionReturnValue = RTN_ERROR;
            }
            finally
            {
                if ((oStreamWriter != null))
                {
                    oStreamWriter.Flush();
                    oStreamWriter.Close();
                    oStreamWriter = null;
                }
            }
            return functionReturnValue;

        }
        #endregion
    }
}
