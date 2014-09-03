using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.App_Start;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    public class SaleReportController : Controller
    {
        //
        // GET: /SaleReport/

        public ActionResult Index()
        {
            return View();
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult SaleReport(DateTime? fromDate, DateTime? toDate, int? saleId, int? recieptID)
        {
            var ctx = new SmsContext();
            fromDate = fromDate == null ? SystemConstant.MIN_DATE : fromDate;
            toDate = toDate == null ? SystemConstant.MAX_DATE : toDate;
            var resultList = ctx.SP_GET_HOA_DON(fromDate, toDate, saleId, recieptID).ToList<SP_GET_HOA_DON_Result>();
            ViewBag.Count = resultList.Count();
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult SaleReport(DateTime? fromDate, DateTime? toDate, int? saleId, int? recieptID, bool? flg)
        {
            return View();
        }


    }
}
