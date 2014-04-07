using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;

namespace SMS.Controllers
{
    public class KhachHangController : Controller
    {
        //
        // GET: /KhachHang/

        public ActionResult Index()
        {
           
            return View();
        }

        [HttpGet]
        public ActionResult AddNew()
        {
            var ctx = new SmsContext();
            var khuVucList = (from s in ctx.KHU_VUC
                              where s.ACTIVE == "A"
                              select s).ToList<KHU_VUC>();
            ViewBag.khuVucList = khuVucList;
            return View();
        }

    }
}
