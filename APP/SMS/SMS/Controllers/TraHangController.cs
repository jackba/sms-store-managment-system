using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class TraHangController : Controller
    {
        //
        // GET: /TraHang/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult IndexPartialView()
        {
            return PartialView();
        }
    }
}
