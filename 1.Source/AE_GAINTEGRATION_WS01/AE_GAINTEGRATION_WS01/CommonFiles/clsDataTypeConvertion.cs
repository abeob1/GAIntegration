using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;

namespace AE_GAINTEGRATION_WS01
{
    public class clsDataTypeConvertion
    {


        public static string ConvertToString(object inputValue, [Optional] string strFormatValue)
        {
            try
            {               
                string strValue = Convert.ToString( (inputValue != null) ?  inputValue : string.Empty);
                return strValue;
            }
            catch (Exception)
            {                
                throw;
            }
        }


        public static int ConvertToInteger(object inputValue, string strFormatValue)
        {
            try
            {
                int strValue = Convert.ToInt32((inputValue != null) ? inputValue : 0);
                return strValue;

            }
            catch (Exception)
            {
                throw;
            }
        }



        public static decimal ConvertToDecimal(object inputValue, string strFormatValue)
        {
            try
            {
                int strValue = Convert.ToInt32((inputValue != null) ? inputValue : 0);
                return strValue;

            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}