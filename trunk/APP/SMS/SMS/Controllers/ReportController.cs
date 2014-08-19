using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data.SqlClient;

namespace SMS.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MonthlyReport()
        {
            var ctx = new SmsContext();
            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_GET_STATISTICS_BY_MONTH").Take(SystemConstant.MAX_ROWS).ToList<Report>();
            var total = ctx.Database.SqlQuery<Report>("exec SP_GET_STATISTICS_OF_YEAR").Take(SystemConstant.MAX_ROWS).FirstOrDefault();
            ReportModel model = new ReportModel();
            model.TheList = tonkho;
            model.Total = total;
            return View(model);
        }

        public ActionResult WeeklyReport()
        {
            var ctx = new SmsContext();
            var tonkho = ctx.Database.SqlQuery<ReportWeek>("exec SP_GET_STATISTICS_BY_WEEK_OF_YEAR").Take(SystemConstant.MAX_ROWS).ToList<ReportWeek>();
            var total = ctx.Database.SqlQuery<Report>("exec SP_GET_STATISTICS_OF_YEAR").Take(SystemConstant.MAX_ROWS).FirstOrDefault();
            ReportWeekModel model = new ReportWeekModel();
            model.TheList = tonkho;
            model.Total = total;
            return View(model);
        }

        public ActionResult WeReport()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult WeReportPartialView(DateTime? fromDate, DateTime? toDate)
        {
            var ctx = new SmsContext();
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var FromDateParam = new SqlParameter
            {
                ParameterName = "START_TIME",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "END_TIME",
                Value = Convert.ToDateTime(toDate)
            };
            var tonkho = ctx.Database.SqlQuery<ReportWeek>("exec SP_REPORT_BY_WEEK @START_TIME, @END_TIME",
               FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<ReportWeek>();
            ReportWeekModel model = new ReportWeekModel();
            model.TheList = tonkho;
            return PartialView("WeReportPartialView", model);
        }
        public ActionResult MthReport()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult MthReportPartialView(DateTime? fromDate, DateTime? toDate)
        {
            var ctx = new SmsContext();
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var FromDateParam = new SqlParameter
            {
                ParameterName = "START_TIME",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "END_TIME",
                Value = Convert.ToDateTime(toDate)
            };
            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_REPORT_BY_MONTH @START_TIME, @END_TIME",
               FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<Report>();
            ReportModel model = new ReportModel();
            model.TheList = tonkho;
            return PartialView("MthReportPartialView", model);
        }
    }
}
