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
    public class NguoiDungController : Controller
    {
        SmsContext ctx = new SmsContext();
        //
        // GET: /NguoiDung/
        public ActionResult Index()
        {
            List<NGUOI_DUNG> lsNguoiDung = ctx.NGUOI_DUNG.ToList();

            return View(lsNguoiDung);
        }

        private void BindKho()
        {
            //List<KHO> city = ctx.KHOes.Where(a => a.ACTIVE == "A").ToList();
            List<KHO> kho = ctx.KHOes.ToList();
            ViewBag.Kho = kho;
        }

        private void BindNhomNguoiDung()
        {
            //List<NHOM_NGUOI_DUNG> nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(a => a.ACTIVE == "A").ToList();
            List<NHOM_NGUOI_DUNG> nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.ToList();
            ViewBag.NhomNguoiDung = nhomNguoiDung;
        }

        [HttpGet]
        public ActionResult AddNew()
        {
            //Ma Kho
            BindKho();

            //Ma Nhom
            BindNhomNguoiDung();

            //var objNguoiDung = new NGUOI_DUNG();
            //return View(objNguoiDung);
            return View();
        }

        [HttpPost]
        public ActionResult AddNew(NGUOI_DUNG nguoidung)
        {
            if (ModelState.IsValid)
            {
                //Created By
                nguoidung.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                nguoidung.CREATE_AT = DateTime.Now;

                ctx.NGUOI_DUNG.Add(nguoidung);
                ctx.SaveChanges();

                return Redirect("Index");
            }
            else
            {
                ViewBag.SelPass = nguoidung.MAT_KHAU;
                BindKho();
                BindNhomNguoiDung();
                return View();
            }

        }
    }
}
