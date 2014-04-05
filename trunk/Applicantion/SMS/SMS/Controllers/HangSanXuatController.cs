using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;

namespace SMS.Controllers
{
    public class HangSanXuatController : Controller
    {
        //
        // GET: /HangSanXuat/

        public ActionResult Index()
        {
            var ctx = new SmsContext();
            var theListContext = (from s in ctx.NHA_SAN_XUAT
                                  where (s.ACTIVE == "A")
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  select new HangSanXuatModel
                                  {
                                      HangSanXuat = s,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  });
            ViewBag.theList = theListContext.ToList<HangSanXuatModel>();
            return View();
        }

        [HttpPost]
        public ActionResult Index(string searchString)
        {
            var ctx = new SmsContext();
            var theListContext = (from s in ctx.NHA_SAN_XUAT
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  where (s.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) || s.TEN_NHA_SAN_XUAT.ToUpper().Contains(searchString.ToUpper()) /*|| s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())*/))
                                  select new HangSanXuatModel
                                  {
                                      HangSanXuat = s,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  });
            ViewBag.CurrentFilter = searchString;
            ViewBag.theList = theListContext;
            return View();
        }

        public ActionResult AddNew()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNew(SMS.Models.NHA_SAN_XUAT khuVuc)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var khuVucNew = db.NHA_SAN_XUAT.Create();
                khuVucNew.TEN_NHA_SAN_XUAT = khuVuc.TEN_NHA_SAN_XUAT;
                //khuVucNew.GHI_CHU = khuVuc.GHI_CHU;
                khuVucNew.ACTIVE = "A";
                khuVucNew.UPDATE_AT = DateTime.Now;
                khuVucNew.CREATE_AT = DateTime.Now;
                khuVucNew.UPDATE_BY = (int)Session["UserId"];
                khuVucNew.CREATE_BY = (int)Session["UserId"];
                db.NHA_SAN_XUAT.Add(khuVucNew);
                db.SaveChanges();
                return Redirect("Index");
            }
            return View();
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy hãng sản xuất tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            NHA_SAN_XUAT khuVuc = ctx.NHA_SAN_XUAT.Find(id);
            if (khuVuc.ACTIVE.Equals("A"))
            {
                return View(khuVuc);
            }
            else
            {
                ViewBag.Message = "Không tìm thấy hãng sản xuất tương ứng.";
                return View("../Home/Error"); ;
            }

        }
        [HttpPost]
        public ActionResult Edit(SMS.Models.NHA_SAN_XUAT khuVuc)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var khuvuc = db.NHA_SAN_XUAT.Find((int)khuVuc.MA_NHA_SAN_XUAT);
                khuvuc.TEN_NHA_SAN_XUAT = khuVuc.TEN_NHA_SAN_XUAT;
                //khuvuc.GHI_CHU = khuVuc.GHI_CHU;
                khuvuc.ACTIVE = "A";
                khuvuc.UPDATE_AT = DateTime.Now;
                khuvuc.UPDATE_BY = (int)Session["UserId"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy hãng sản xuất tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.NHA_SAN_XUAT.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {
                donvi.ACTIVE = "I";
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Không tìm thấy hãng sản xuất tương ứng";
                return View("../Home/Error"); ;
            }
        }
    }
}
