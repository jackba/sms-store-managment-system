using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;

namespace SMS.Controllers
{
    public class TraHangController : Controller
    {
        //
        // GET: /TraHang/

        public ActionResult Show(int id, string message, string messageInfor)
        {
            var ctx = new SmsContext();
            var invoiceInfor = ctx.SP_GET_HOA_DON_INFO(id).FirstOrDefault();
            List<V_HOA_DON> detailList = ctx.V_HOA_DON.Where(dh => dh.MA_HOA_DON == id).ToList();
            InvoicesModel model = new InvoicesModel();
            model.Infor = invoiceInfor;
            model.detailList = detailList;
            ViewBag.Message = message;
            ViewBag.MessageInfor = messageInfor;
            return View(model);
        }

        public ActionResult Index(string message, string inforMessage)
        {
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            return View();
        }

        [HttpPost]
        public PartialViewResult IndexPartialView(int? billId, string billCode,
            int? customerId, string customerName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            if (string.IsNullOrEmpty(billCode))
            {
                billCode = string.Empty;
                billId = 0;
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
            var exportedList = ctx.SP_GET_HOA_DON_EXPORTED(Convert.ToInt32(billId), billCode,
                Convert.ToInt32(customerId), customerName, Convert.ToDateTime(fromDate),
                Convert.ToDateTime(toDate)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_EXPORTED_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ExportModel model = new ExportModel();
            model.ExportedList = exportedList.ToPagedList(pageIndex, pageSize);
            model.PageCount = exportedList.Count;
            ViewBag.BillId = billId;
            ViewBag.BillCode = billCode;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = customerName;
            return PartialView("IndexPartialView", model);
        }
    }
}
