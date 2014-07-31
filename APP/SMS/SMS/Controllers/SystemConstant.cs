using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMS.Models;
namespace SMS.Controllers
{
    public class SystemConstant
    {
        public const int ROWS = 5;
        public const int MAX_ROWS = 999;
        private static System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("vi-VN");
        public static DateTime MIN_DATE = getMinDate();
        public static DateTime MAX_DATE = DateTime.ParseExact("01/01/9999", "dd/MM/yyyy", cultureinfo);
        public static string SALT = "2014";
        public static DateTime getMinDate()
        {            
            return DateTime.ParseExact("01/01/2014", "dd/MM/yyyy", cultureinfo);
        }
    }
    
}