using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.App_Start;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    public class ChuyenDoiDonViController : Controller
    {
        //
        // GET: /ChuyenDoiDonVi/
        [CustomActionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [CustomActionFilter]
        public ActionResult AddNew()
        {
            return View();
        }
    }
}
