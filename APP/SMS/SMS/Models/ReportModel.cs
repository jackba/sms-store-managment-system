using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

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
        public DateTime DAY { get; set; }
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

    public class ReportReturn2Provider
    {
        public int ID { get; set; }
        public int MA_NHA_CUNG_CAP { get; set; }
        public string TEN_NHA_CUNG_CAP { get; set; }
        public DateTime NGAY_LAP_PHIEU { get; set; }
        public string TEN_NGUOI_DUNG { get; set; }
        public double TOTAL { get; set; }
    }

    public class ReportReturn2ProviderModel
    {
        public int Count { get; set; }
        public IPagedList<ReportReturn2Provider> Details { get; set; }
    }

    public class ReportDebitColection
    {
        public int ID { get; set; }
        public int MA_KHACH_HANG { get; set; }
        public DateTime NGAY_PHAT_SINH { get; set; }
        public double PHAT_SINH { get; set; }
        public string TEN_KHACH_HANG { get; set; }
    }
    public class ReportDebitColectionModel
    {
        public List<ReportDebitColection> Details { get; set; }
    }
}