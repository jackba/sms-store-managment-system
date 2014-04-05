using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;

namespace SMS.Controllers
{
    public class NguoiDungController : Controller
    {
        //
        // GET: /NguoiDung/
        public ActionResult Index()
        {
            SmsContext ctx = new SmsContext();
            List<NGUOI_DUNG> lsNguoiDung = ctx.NGUOI_DUNG.ToList();

            return View(lsNguoiDung);
        }

    }
}
