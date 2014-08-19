using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Report
    {
        public double SO_TIEN_KHACH_TRA { get; set; }
        public double SO_TIEN_NO_GOI_DAU { get; set; }
        public double BUGET_TOTAL { get; set; }
        public double RETURN_TOTAL { get; set; }
        public double TOTAL { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
    }

    public class ReportWeek
    {
        public double SO_TIEN_KHACH_TRA { get; set; }
        public double SO_TIEN_NO_GOI_DAU { get; set; }
        public double BUGET_TOTAL { get; set; }
        public double RETURN_TOTAL { get; set; }
        public double TOTAL { get; set; }
        public int WEEK { get; set; }
        public int YEAR { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
    }



    public class ReportWeekModel
    {
        public List<ReportWeek> TheList { get; set; }
        public Report Total { get; set; }
    }

    public class ReportModel
    {
        public List<Report> TheList { get; set; }
        public Report Total { get; set; }
    }
}