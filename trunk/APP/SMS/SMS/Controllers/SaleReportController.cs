using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;

namespace SMS.Controllers
{
    public class SaleReportController : Controller
    {
        //
        // GET: /SaleReport/

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult SaleReport(DateTime? fromDate, DateTime? toDate, int? saleId, int? recieptID)
        {
            var ctx = new SmsContext();

            return View();
        }

        [HttpPost]
        public ActionResult SaleReport(DateTime? fromDate, DateTime? toDate, int? saleId, int? recieptID, bool? flg)
        {
            return View();
        }


    }
}
