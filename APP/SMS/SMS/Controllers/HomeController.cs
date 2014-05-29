using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data;
using System.Data.SqlClient;
using PagedList;
using System.Diagnostics;
using SMS.App_Start;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    [CustomActionFilter]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Import()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Export()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Return()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult SmsMessage()
        {
            var ctx = new SmsContext();
            SMS_MESSAGES model = new SMS_MESSAGES();
            int groupUserId = (int)Session["GroupUserId"];
            model = ctx.SMS_MESSAGES.Include("NGUOI_DUNG1").OrderByDescending(uh => uh.ID)
                .FirstOrDefault(uh => uh.ACTIVE == "A" && (uh.ID_NHOM_NGUOI_NHAN == groupUserId || uh.ID_NHOM_NGUOI_NHAN == null));
            return PartialView("SmsMessage", model);
        }
        [HttpPost]
        public PartialViewResult PagingContent(string SearchString, int? currentPageIndex)
        {

            IPagedList<SP_GET_TON_KHO_ALERT> tk = null;

            ViewBag.SearchString = SearchString;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            // var watch = Stopwatch.StartNew();
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            string s = ctx.Database.Connection.ToString();
            var tonkho = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", new SqlParameter("NAME", string.IsNullOrEmpty(SearchString) ? "" : SearchString.Trim())).ToList<SP_GET_TON_KHO_ALERT>().Take(SystemConstant.MAX_ROWS); ;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = tonkho.Count();
            tk = tonkho.ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.KhoList = ListKho;
            ViewBag.tonKho = tk;
            GetTonKhoAlertModel model = new GetTonKhoAlertModel();
            model.WarningList = tk;
            return PartialView("IndexPartialView", model);
        }

        [HttpPost]
        public ActionResult Index(string SearchString, int? page)
        {
            IPagedList<SP_GET_TON_KHO_ALERT> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            string s = ctx.Database.Connection.ToString();
            var tonkho = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", new SqlParameter("NAME", string.IsNullOrEmpty(SearchString) ? "" : SearchString.Trim())).ToList<SP_GET_TON_KHO_ALERT>().Take(SystemConstant.MAX_ROWS); ;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = tonkho.Count();
            tk = tonkho.ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.KhoList = ListKho;
            ViewBag.SearchString = SearchString;
            ViewBag.tonKho = tk;
            GetTonKhoAlertModel model = new GetTonKhoAlertModel();
            model.WarningList = tk;
            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult BanHang()
        {
            return View();
        }

        public ActionResult QuanLyKho()
        {
            return View();
        }

        public ActionResult DanhMuc()
        {
            return View();
        }

        public ActionResult QuanTri()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult NguoiDung()
        {
            return View();
        }
    }
}
