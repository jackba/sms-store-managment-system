using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
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

        [HttpPost]
        public ActionResult AddNew(Models.KHACH_HANG khachHang)
        {
            var ctx = new SmsContext();
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var khuVucNew = db.KHACH_HANG.Create();
                khuVucNew.TEN_KHACH_HANG = khachHang.TEN_KHACH_HANG;
                khuVucNew.MA_THE_KHACH_HANG = khachHang.MA_THE_KHACH_HANG;
                khuVucNew.DIA_CHI = khachHang.DIA_CHI;
                khuVucNew.SO_DIEN_THOAI = khachHang.SO_DIEN_THOAI;
                khuVucNew.EMAIL = khachHang.EMAIL;
                khuVucNew.MA_KHU_VUC = khachHang.MA_KHU_VUC;
                khuVucNew.DOANH_SO = khachHang.DOANH_SO;
                khuVucNew.NO_GOI_DAU = khachHang.NO_GOI_DAU;
                khuVucNew.NGAY_PHAT_SINH_NO = khachHang.NGAY_PHAT_SINH_NO;
                khuVucNew.ACTIVE = "A";
                khuVucNew.UPDATE_AT = DateTime.Now;
                khuVucNew.CREATE_AT = DateTime.Now;
                khuVucNew.UPDATE_BY = (int)Session["UserId"];
                khuVucNew.CREATE_BY = (int)Session["UserId"];
                db.KHACH_HANG.Add(khuVucNew);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                var khuVucList = (from s in ctx.KHU_VUC
                                  where s.ACTIVE == "A"
                                  select s).ToList<KHU_VUC>();
                ViewBag.khuVucList = khuVucList;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Edit()
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
