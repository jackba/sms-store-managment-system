using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data;
using PagedList;
using System.Data.SqlClient;

namespace SMS.Controllers
{
    public class ExportController : Controller
    {
        //
        // GET: /Export/


        

        public ActionResult Export(int id, int? makho)
        {
            var ctx = new SmsContext();
            var infor =  ctx.SP_GET_HOA_DON_INFO(id).FirstOrDefault();
            if (makho == null)
            {
                makho = 0;
            }
            var detailList = ctx.SP_GET_HD_DETAIL_FOR_EXPORT(Convert.ToInt32(id), Convert.ToInt32(makho)).ToList<SP_GET_HD_DETAIL_FOR_EXPORT_Result>();
            ExportModel model = new ExportModel();
            model.DetailList = detailList;
            model.Infor = infor;
            return View(model);
        }


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult PagingContent(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? storeId, string storeName, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(storeName))
            {
                storeId = 0;
            }
            if (fromdate == null)
            {
                fromdate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
            }

            if (todate == null)
            {
                todate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(todate.ToString()).ToString("dd/MM/yyyy");
            }
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var list = ctx.SP_GET_HOA_DON_CAN_XUAT_KHO(Convert.ToInt32(customerId), customerName, Convert.ToInt32(storeId), fromdate, todate).OrderByDescending(uh => uh.NGAY_BAN)
                .Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result>();
            ExportModel model = new ExportModel();
            model.WaitingList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.storeId = storeId;
            return PartialView("IndexPartialView", model);
        }

        [HttpPost]
        public PartialViewResult IndexPartialView(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? storeId, string storeName, int? currentPageIndex)
        {

            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(storeName))
            {
                storeId = 0;
            }
            if (fromdate == null)
            {
                fromdate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
            }

            if (todate == null)
            {
                todate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(todate.ToString()).ToString("dd/MM/yyyy");
            }
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var list = ctx.SP_GET_HOA_DON_CAN_XUAT_KHO(Convert.ToInt32(customerId), customerName, Convert.ToInt32(storeId), fromdate, todate).OrderByDescending(uh => uh.NGAY_BAN)
                .Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result>();
            ExportModel model = new ExportModel();
            model.WaitingList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.storeId = storeId;
            return PartialView("IndexPartialView", model);
        }
    }
}
