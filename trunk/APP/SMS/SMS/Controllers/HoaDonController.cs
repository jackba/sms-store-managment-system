using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.App_Start;

namespace SMS.Controllers
{
    [CustomActionFilter]
    public class HoaDonController : Controller
    {
        //
        // GET: /HoaDon/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowDetail(int id)
        {
            return View();
        }

    }
}
