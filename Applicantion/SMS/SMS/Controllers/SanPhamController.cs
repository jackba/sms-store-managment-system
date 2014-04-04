using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class SanPhamController : Controller
    {
        //
        // GET: /SanPham/

        public ActionResult Index()
        {
            return View();
        }

         [HttpGet]
        public ActionResult AddNew()
        {
            return View();
        }

    }
}
