using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class ChuyenDoiDonViController : Controller
    {
        //
        // GET: /ChuyenDoiDonVi/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddNew()
        {
            return View();
        }
    }
}
