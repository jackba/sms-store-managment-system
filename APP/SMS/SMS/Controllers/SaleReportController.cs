using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.App_Start;

namespace SMS.Controllers
{
    [CustomActionFilter]
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
            var resultList = ctx.SP_GET_HOA_DON(fromDate, toDate, saleId, recieptID).ToList<SP_GET_HOA_DON_Result>();
            ViewBag.Count = resultList.Count();
            return View();
        }

        [HttpPost]
        public ActionResult SaleReport(DateTime? fromDate, DateTime? toDate, int? saleId, int? recieptID, bool? flg)
        {
            return View();
        }


    }
}
