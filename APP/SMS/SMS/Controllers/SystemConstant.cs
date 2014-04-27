using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Controllers
{
    public class SystemConstant
    {
        public static int ROWS = 10;
        public static int MAX_ROWS = 999;
        private static System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("vi-VN");
        public static DateTime MIN_DATE = DateTime.ParseExact("01/01/2014", "dd/MM/yyyy", cultureinfo);
        public static DateTime MAX_DATE = DateTime.ParseExact("01/01/9999", "dd/MM/yyyy", cultureinfo);
    }
}