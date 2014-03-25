using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "SMS - Store Managemanet System";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Quản lý kho";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult BanHang()
        {
            ViewBag.Message = "Bán Hàng";

            return View();
        }

        public ActionResult QuanLyKho()
        {
            ViewBag.Message = "Quan Ly Kho";

            return View();
        }
        public ActionResult DanhMuc()
        {
            ViewBag.Message = "Các hạng mục danh mục";

            return View();
        }

        public ActionResult QuanTri()
        {
            ViewBag.Message = "Các hạng mục dành cho nhà quản trị.";

            return View();
        }

    }
}
