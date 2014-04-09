using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;

namespace SMS.Controllers
{
    public class KhoController : Controller
    {
        //
        // GET: /Kho/

        public ActionResult Index()
        {

            var ctx = new SmsContext();
            var theListContext = (from s in ctx.KHOes
                                  where (s.ACTIVE == "A")
                                  join u2 in ctx.NGUOI_DUNG on s.MA_NGUOI_DUNG_DAU equals u2.MA_NGUOI_DUNG
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  select new KhoModel
                                  {
                                      Kho = s,
                                      NguoiDungDau = u2,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  });
            ViewBag.theList = theListContext.ToList<KhoModel>();
            return View();
        }
        [HttpPost]
        public ActionResult Index(string searchString)
        {

            var ctx = new SmsContext();
            var theListContext = (from s in ctx.KHOes
                                  join u2 in ctx.NGUOI_DUNG on s.MA_NGUOI_DUNG_DAU equals u2.MA_NGUOI_DUNG
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  where (s.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) 
                                  || s.TEN_KHO.ToUpper().Contains(searchString.ToUpper())
                                  || s.SO_DIEN_THOAI.ToUpper().Contains(searchString.ToUpper())
                                  || u2.TEN_NGUOI_DUNG.ToUpper().Contains(searchString.ToUpper())
                                  || s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())))
                                  select new KhoModel
                                  {
                                      Kho = s,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  });
            ViewBag.theList = theListContext.ToList<KhoModel>();
            return View();
        }

        public ActionResult AddNew()
        {
            KHO model = new KHO();
            var ctx = new SmsContext();
            var nguoiDungList = (from s in ctx.NGUOI_DUNG
                           where s.ACTIVE == "A"
                                 select s).ToList<NGUOI_DUNG>();
            ViewBag.nguoiDungList = nguoiDungList;
            return View();
        }

        [HttpPost]
        public ActionResult AddNew(SMS.Models.KHO kho)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var khuVucNew = db.KHOes.Create();
                khuVucNew.TEN_KHO = kho.TEN_KHO;
                khuVucNew.GHI_CHU = kho.GHI_CHU;
                khuVucNew.DIA_CHI = kho.DIA_CHI;
                khuVucNew.SO_DIEN_THOAI = kho.SO_DIEN_THOAI;
                khuVucNew.MA_NGUOI_DUNG_DAU = kho.MA_NGUOI_DUNG_DAU;
                khuVucNew.ACTIVE = "A";
                khuVucNew.UPDATE_AT = DateTime.Now;
                khuVucNew.CREATE_AT = DateTime.Now;
                khuVucNew.UPDATE_BY = (int)Session["UserId"];
                khuVucNew.CREATE_BY = (int)Session["UserId"];
                db.KHOes.Add(khuVucNew);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var ctx = new SmsContext();
            var nguoiDungList = (from s in ctx.NGUOI_DUNG
                                 where s.ACTIVE == "A"
                                 select s).ToList<NGUOI_DUNG>();
            ViewBag.nguoiDungList = nguoiDungList;
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            KHO khuVuc = ctx.KHOes.Find(id);
            var nguoiDungList = (from s in ctx.NGUOI_DUNG
                                 where s.ACTIVE == "A"
                                 select s).ToList<NGUOI_DUNG>();
            ViewBag.nguoiDungList = nguoiDungList;
            if (khuVuc.ACTIVE.Equals("A"))
            {
                return View(khuVuc);
            }
            else
            {
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }

        }


        [HttpPost]
        public ActionResult Edit(SMS.Models.KHO kho)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var khuVucNew = db.KHOes.Find(kho.MA_KHO);
                khuVucNew.TEN_KHO = kho.TEN_KHO;
                khuVucNew.GHI_CHU = kho.GHI_CHU;
                khuVucNew.DIA_CHI = kho.DIA_CHI;
                khuVucNew.SO_DIEN_THOAI = kho.SO_DIEN_THOAI;
                khuVucNew.MA_NGUOI_DUNG_DAU = kho.MA_NGUOI_DUNG_DAU;
                khuVucNew.ACTIVE = "A";
                khuVucNew.UPDATE_AT = DateTime.Now;
                khuVucNew.CREATE_AT = DateTime.Now;
                khuVucNew.UPDATE_BY = (int)Session["UserId"];
                khuVucNew.CREATE_BY = (int)Session["UserId"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var ctx = new SmsContext();
            var nguoiDungList = (from s in ctx.NGUOI_DUNG
                                 where s.ACTIVE == "A"
                                 select s).ToList<NGUOI_DUNG>();
            ViewBag.nguoiDungList = nguoiDungList;
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            KHO khuVuc = ctx.KHOes.Find(id);
            if (khuVuc.ACTIVE.Equals("A"))
            {
                khuVuc.ACTIVE = "I";
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }

        }

    }
}
