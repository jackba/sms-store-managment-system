using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.App_Start;
using SMS.Models;
using PagedList;

namespace SMS.Controllers
{
    [CustomActionFilter]
    public class HoaDonController : Controller
    {
        //
        // GET: /HoaDon/


        public ActionResult Index(DateTime? fromdate, DateTime? todate, 
            int? customerId, string customerName, int? salerId, string salerName,
            int? accountantId, string accountantName, int? status, int? page)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(salerName))
            {
                salerId = 0;
            }
            if (string.IsNullOrEmpty(accountantName))
            {
                accountantId = 0;
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
                salerId = Convert.ToInt32(Session["UserId"]);
            }
            if (!(bool)Session["IsAdmin"] && (bool)Session["IsAccounting"])
            {
                accountantId = Convert.ToInt32(Session["UserId"]);
            }
            var ctx = new SmsContext();
            var list = ctx.SP_GET_HOA_DON_BH(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_BH_Result>();
            HoaDonBHModel model = new HoaDonBHModel();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            model.HoaDonList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.InputKind = Convert.ToInt32(status);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? salerId, string salerName,
            int? accountantId, string accountantName, int? status, int? page, bool? flg)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(salerName))
            {
                salerId = 0;
            }
            if (string.IsNullOrEmpty(accountantName))
            {
                accountantId = 0;
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
                salerId = Convert.ToInt32(Session["UserId"]);
            }
            if (!(bool)Session["IsAdmin"] && (bool)Session["IsAccounting"])
            {
                accountantId = Convert.ToInt32(Session["UserId"]);
            }
            var ctx = new SmsContext();
            var list = ctx.SP_GET_HOA_DON_BH(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_BH_Result>();
            HoaDonBHModel model = new HoaDonBHModel();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            model.HoaDonList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.InputKind = Convert.ToInt32(status);
            return View(model);
        }


        public ActionResult ShowDetail(int id)
        {
            return View();
        }

    }
}
