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
        public ActionResult Index(string SearchString, int? page, bool? flag)
        {
            IPagedList<SP_GET_TON_KHO_ALERT> tk = null;
            
            ViewBag.CurrentFilter = SearchString;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            
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
            //watch.Stop();
            //var elapsedMs = watch.ElapsedMilliseconds;
           // ViewBag.Count = elapsedMs;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string SearchString, int? page)
        {
            IPagedList<SP_GET_TON_KHO_ALERT> tk = null;
            ViewBag.CurrentFilter = SearchString;
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
