﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;

namespace SMS.Controllers
{
    public class NhaCungCapController : Controller
    {
        //
        // GET: /NhaCungCap/

        public ActionResult Index()
        {
            var ctx = new SmsContext();
            var theListContext = (from s in ctx.NHA_CUNG_CAP
                                  where (s.ACTIVE == "A")
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  select new NhaCungCapModel
                                  {
                                      NhaCungCap = s,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  });
            ViewBag.theList = theListContext.ToList<NhaCungCapModel>();
            return View();
        }

        [HttpPost]
        public ActionResult Index(string searchString)
        {
            var ctx = new SmsContext();
            var theListContext = (from s in ctx.NHA_CUNG_CAP
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  where (s.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) ||
                                  s.SO_DIEN_THOAI.ToUpper().Contains(searchString.ToUpper()) ||
                                  s.TEN_NGUOI_LIEN_HE.ToUpper().Contains(searchString.ToUpper()) || 
                                  s.TEN_NHA_CUNG_CAP.ToUpper().Contains(searchString.ToUpper()) || 
                                  s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())))
                                  select new NhaCungCapModel
                                  {
                                      NhaCungCap = s,
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
        public ActionResult AddNew(SMS.Models.NHA_CUNG_CAP khuVuc)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var khuVucNew = db.NHA_CUNG_CAP.Create();
                khuVucNew.TEN_NHA_CUNG_CAP = khuVuc.TEN_NHA_CUNG_CAP;
                khuVucNew.SO_DIEN_THOAI = khuVuc.SO_DIEN_THOAI;
                khuVucNew.DIA_CHI = khuVuc.DIA_CHI;
                khuVucNew.TEN_NGUOI_LIEN_HE = khuVuc.TEN_NGUOI_LIEN_HE;
                khuVucNew.EMAIL = khuVuc.EMAIL;
                khuVucNew.GHI_CHU = khuVuc.GHI_CHU;
                khuVucNew.ACTIVE = "A";
                khuVucNew.UPDATE_AT = DateTime.Now;
                khuVucNew.CREATE_AT = DateTime.Now;
                khuVucNew.UPDATE_BY = (int)Session["UserId"];
                khuVucNew.CREATE_BY = (int)Session["UserId"];
                db.NHA_CUNG_CAP.Add(khuVucNew);
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
                ViewBag.Message = "Không tìm thấy nhà cung cấp.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            NHA_CUNG_CAP khuVuc = ctx.NHA_CUNG_CAP.Find(id);
            if (khuVuc.ACTIVE.Equals("A"))
            {
                return View(khuVuc);
            }
            else
            {
                ViewBag.Message = "Không tìm thấy nhà cung cấp.";
                return View("../Home/Error"); ;
            }

        }

        [HttpPost]
        public ActionResult Edit(SMS.Models.NHA_CUNG_CAP khuVuc)
        {
            if (ModelState.IsValid)
            {
                var ctx = new SmsContext();
                var khuVucNew = ctx.NHA_CUNG_CAP.Find(khuVuc.MA_NHA_CUNG_CAP);
                khuVucNew.TEN_NHA_CUNG_CAP = khuVuc.TEN_NHA_CUNG_CAP;
                khuVucNew.SO_DIEN_THOAI = khuVuc.SO_DIEN_THOAI;
                khuVucNew.DIA_CHI = khuVuc.DIA_CHI;
                khuVucNew.TEN_NGUOI_LIEN_HE = khuVuc.TEN_NGUOI_LIEN_HE;
                khuVucNew.EMAIL = khuVuc.EMAIL;
                khuVucNew.GHI_CHU = khuVuc.GHI_CHU;
                khuVucNew.ACTIVE = "A";
                khuVucNew.UPDATE_AT = DateTime.Now;
                khuVucNew.UPDATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy nhà cung cấp tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.NHA_CUNG_CAP.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {
                donvi.ACTIVE = "I";
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Không tìm thấy nhà cung cấp tương ứng.";
                return View("../Home/Error"); ;
            }
        }
    } 
}