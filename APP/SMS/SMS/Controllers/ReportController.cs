using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data.SqlClient;
using PagedList;
using System.Web.Helpers;

namespace SMS.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult ReportByArea()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult ReportByAreaPartialView(int areaId, string areaName, DateTime? fromDate, DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(areaName))
            {
                areaName = string.Empty;
                areaId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var ctx = new SmsContext();
            var customerIdParam = new SqlParameter
            {
                ParameterName = "MA_KHU_VUC",
                Value = Convert.ToInt32(areaId)
            };
            var customerNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHU_VUC",
                Value = areaName
            };
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

            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_REPORT_BY_DAY_AND_AREA  @START_TIME, @END_TIME, @MA_KHU_VUC, @TEN_KHU_VUC ",
                FromDateParam, ToDateParam, customerIdParam,customerNameParam ).Take(SystemConstant.MAX_ROWS).ToList<Report>();
            ReportModel model = new ReportModel();
            model.TheList = tonkho;

            return PartialView("ReportByAreaPartialView", model);
        }


        public ActionResult ReportDebitColecction()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult ReportDebitColecctionPartialView(int? customerId, string customerName, DateTime? fromDate, DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var ctx = new SmsContext();
            var customerIdParam = new SqlParameter
            {
                ParameterName = "CUSTOMER_ID",
                Value = Convert.ToInt32(customerId)
            };
            var customerNameParam = new SqlParameter
            {
                ParameterName = "CUSTOMER_NAME",
                Value = customerName
            };
            var FromDateParam = new SqlParameter
            {
                ParameterName = "FROM_DATE",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "TO_DATE",
                Value = Convert.ToDateTime(toDate)
            };
            var tonkho = ctx.Database.SqlQuery<ReportDebitColection>("exec SP_GET_REPORT_DEBT_COLLECTION @CUSTOMER_ID, @CUSTOMER_NAME, @FROM_DATE, @TO_DATE", customerIdParam, customerNameParam,
                FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<ReportDebitColection>();
            ReportDebitColectionModel model = new ReportDebitColectionModel();
            model.Details = tonkho;
            return PartialView("ReportDebitColecctionPartialView", model);
        }
        public ActionResult Return2ProviderReport()
        {
            return View();
        }

        public ActionResult GetRainfallChart()
        {
            var ctx = new SmsContext();
            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_GET_STATISTICS_BY_MONTH").Take(SystemConstant.MAX_ROWS).ToList<Report>();
            var chart = new Chart(width:800, height: 400)
                .AddSeries(
                        xValue: tonkho.Select(x => x.MONTH).ToArray(),
                        yValues: tonkho.Select(x => x.TOTAL).ToArray()
                      ).AddTitle("Biểu đồ doanh thu sau khi trừ trả hàng").Write();
            return null;
        }

        [HttpPost]
        public PartialViewResult Return2ProviderReportPartialView(int? providerId, string providerName,
            DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            if (string.IsNullOrWhiteSpace(providerName))
            {
                providerName = string.Empty;
                providerId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }

            var providerIdParam = new SqlParameter
            {
                ParameterName = "PROVIDER_ID",
                Value = Convert.ToInt32(providerId)
            };
            var providerNameParam = new SqlParameter
            {
                ParameterName = "PROVIDER_NAME",
                Value = providerName
            };
            var FromDateParam = new SqlParameter
            {
                ParameterName = "FROM_DATE",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "TO_DATE",
                Value = Convert.ToDateTime(toDate)
            };
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            var details = ctx.Database.SqlQuery<ReportReturn2Provider>("exec SP_GET_REPORT_RETURN_2_PROVIDER @PROVIDER_ID, @PROVIDER_NAME, @FROM_DATE, @TO_DATE ", 
                providerIdParam, providerNameParam, FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<ReportReturn2Provider>();
            ReportReturn2ProviderModel model = new ReportReturn2ProviderModel();
            model.Count = details.Count;
            model.Details = details.ToPagedList(pageIndex, pageSize);

            ViewBag.ProviderId = providerId;
            ViewBag.ProviderName = providerName;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.Todate = ((DateTime)toDate).ToString("dd/MM/yyyy");

            return PartialView("Return2ProviderReportPartialView", model);
        }
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

        public ActionResult GetRainfallChartWeek()
        {
            var ctx = new SmsContext();
            var tonkho = ctx.Database.SqlQuery<ReportWeek>("exec SP_GET_STATISTICS_BY_WEEK_OF_YEAR").Take(SystemConstant.MAX_ROWS).ToList<ReportWeek>();
            var chart = new Chart(width: 800, height: 400)
                .AddSeries(
                        xValue: tonkho.Select(x => x.WEEK).ToArray(),
                        yValues: tonkho.Select(x => x.TOTAL).ToArray()
                      ).AddTitle("Biểu đồ doanh thu sau khi trừ trả hàng").Write();
            return null;
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
        
        public ActionResult DayReport()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult DayReportPartialView(DateTime? fromDate, DateTime? toDate)
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
            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_REPORT_BY_DAY @START_TIME, @END_TIME",
               FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<Report>();
            ReportModel model = new ReportModel();
            model.TheList = tonkho;
            return PartialView("DayReportPartialView", model);
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
