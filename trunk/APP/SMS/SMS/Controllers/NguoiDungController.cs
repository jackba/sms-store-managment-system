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
            List<NGUOI_DUNG> lsNguoiDung = ctx.NGUOI_DUNG.Where(m => m.ACTIVE.Equals("A")).ToList();

            return View(lsNguoiDung);
        }

        private void BindKho()
        {
            //List<KHO> city = ctx.KHOes.Where(a => a.ACTIVE == "A").ToList();
            List<KHO> kho = ctx.KHOes.Where(m => m.ACTIVE.Equals("A")).ToList();
            ViewBag.Kho = kho;
        }

        private void BindNhomNguoiDung()
        {
            //List<NHOM_NGUOI_DUNG> nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(a => a.ACTIVE == "A").ToList();
            List<NHOM_NGUOI_DUNG> nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(m => m.ACTIVE.Equals("A")).ToList();
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
                //Active
                nguoidung.ACTIVE = "A";

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

        [HttpGet]
        public ActionResult Edit(int id = -1)
        {
            var nguoidung = ctx.NGUOI_DUNG.SingleOrDefault(n => n.MA_NGUOI_DUNG == id);

            if (nguoidung == null)
            {
                //TODO: return error message
                return View("Index");
            }
            else
            {
                //Ma Kho
                BindKho();

                //Ma Nhom
                BindNhomNguoiDung();

                return View(nguoidung);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, NGUOI_DUNG nguoidung)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                var dbNguoiDung = ctx.NGUOI_DUNG.SingleOrDefault(n => n.MA_NGUOI_DUNG == id);

                dbNguoiDung.TEN_NGUOI_DUNG = nguoidung.TEN_NGUOI_DUNG;
                dbNguoiDung.NGAY_SINH = nguoidung.NGAY_SINH;
                dbNguoiDung.SO_CHUNG_MINH = nguoidung.SO_CHUNG_MINH;
                dbNguoiDung.DIA_CHI = nguoidung.DIA_CHI;
                dbNguoiDung.SO_DIEN_THOAI = nguoidung.SO_DIEN_THOAI;
                dbNguoiDung.MA_KHO = nguoidung.MA_KHO;
                dbNguoiDung.USER_NAME = nguoidung.USER_NAME;
                dbNguoiDung.MAT_KHAU = nguoidung.MAT_KHAU;
                dbNguoiDung.NGAY_VAO_LAM = nguoidung.NGAY_VAO_LAM;
                dbNguoiDung.GHI_CHU = nguoidung.GHI_CHU;
                dbNguoiDung.MA_NHOM_NGUOI_DUNG = nguoidung.MA_NHOM_NGUOI_DUNG;

                //Updated By
                nguoidung.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                nguoidung.UPDATE_AT = DateTime.Now;

                ctx.SaveChanges();

                //return Redirect("Index");
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.SelPass = nguoidung.MAT_KHAU;
                BindKho();
                BindNhomNguoiDung();
                return View();
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var dbNguoiDung = ctx.NGUOI_DUNG.Find(id);

            if (dbNguoiDung == null)
            {
                ViewBag.Message = "Không thể xóa người dùng này";
                return View("Error");
            }
            else
            {
                dbNguoiDung.ACTIVE = "I";
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
