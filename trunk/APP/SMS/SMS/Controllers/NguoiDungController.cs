﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using SMS.Models;
using PagedList;
using System.IO;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    public class NguoiDungController : Controller
    {
        //
        // GET: /DonVi/
        [HttpGet]
        public ActionResult Index(string searchString, string sortOrder, string currentFilter, int? page)
        {
            var ctx = new SmsContext();
            if (!String.IsNullOrEmpty(searchString) && (page == null || page == 0))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = sortOrder == "id_desc" ? "id" : "id_desc";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            var theListContext = (from u in ctx.NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on u.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  join u2 in ctx.NGUOI_DUNG on u.UPDATE_BY equals u2.MA_NGUOI_DUNG
                                  where (u.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) || u.TEN_NGUOI_DUNG.ToUpper().Contains(searchString.ToUpper()) || u.USER_NAME.ToUpper().Contains(searchString.ToUpper())))
                                  select new NguoiDungObj
                                  {
                                      NguoiDung = u,
                                      NguoiTao = u1,
                                      NguoiCapNhat = u2
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;

            IPagedList<NguoiDungObj> nguoiDungs = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
            switch (sortOrder)
            {
                case "id":
                    nguoiDungs = theListContext.OrderBy(nd => nd.NguoiDung.MA_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    nguoiDungs = theListContext.OrderByDescending(nd => nd.NguoiDung.MA_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    nguoiDungs = theListContext.OrderBy(nd => nd.NguoiDung.TEN_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    nguoiDungs = theListContext.OrderByDescending(nd => nd.NguoiDung.TEN_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    nguoiDungs = theListContext.OrderBy(nd => nd.NguoiDung.MA_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
            }
            return View(nguoiDungs);
        }

        [HttpGet]
        public ActionResult AddNew()
        {
            //Ma Kho
            BindKho();

            //Ma Nhom
            BindNhomNguoiDung();

            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            NGUOI_DUNG nguoidung = ctx.NGUOI_DUNG.Find(id);
            if (nguoidung.ACTIVE.Equals("A"))
            {
                //Ma Kho
                BindKho();

                //Ma Nhom
                BindNhomNguoiDung();

                ViewBag.nguoiDung = nguoidung;
                return View(nguoidung);
            }
            else
            {
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                return View("../Home/Error"); ;
            }

        }
        
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var nguoidung = ctx.NGUOI_DUNG.Find(id);
            if (nguoidung.ACTIVE.Equals("A"))
            {
                nguoidung.ACTIVE = "I";
                nguoidung.UPDATE_AT = DateTime.Now;
                nguoidung.CREATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [HttpPost]
        public ActionResult Edit(SMS.Models.NGUOI_DUNG nguoiDung)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var nguoidung = db.NGUOI_DUNG.Find((int)nguoiDung.MA_NGUOI_DUNG);
                nguoidung.TEN_NGUOI_DUNG = nguoiDung.TEN_NGUOI_DUNG;
                nguoidung.NGAY_SINH = nguoiDung.NGAY_SINH;
                nguoidung.SO_CHUNG_MINH = nguoiDung.SO_CHUNG_MINH;
                nguoidung.DIA_CHI = nguoiDung.DIA_CHI;
                nguoidung.SO_DIEN_THOAI = nguoiDung.SO_DIEN_THOAI;
                nguoidung.MA_KHO = nguoiDung.MA_KHO;
                nguoidung.USER_NAME = nguoiDung.USER_NAME;
                nguoidung.MAT_KHAU = nguoiDung.MAT_KHAU;
                nguoidung.NGAY_VAO_LAM = nguoiDung.NGAY_VAO_LAM;

                if (Request.Files[0].InputStream.Length != 0)
                {
                    Stream fileStream = Request.Files[0].InputStream;
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                    nguoidung.HINH_ANH = bytes;
                }
                nguoidung.MA_NHOM_NGUOI_DUNG = nguoiDung.MA_NHOM_NGUOI_DUNG;
                nguoidung.GHI_CHU = nguoiDung.GHI_CHU;
                nguoidung.ACTIVE = "A";
                nguoidung.UPDATE_AT = DateTime.Now;
                nguoidung.UPDATE_BY = (int)Session["UserId"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddNew(SMS.Models.NGUOI_DUNG nguoiDung)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var nguoidung = db.NGUOI_DUNG.Create();
                nguoidung.TEN_NGUOI_DUNG = nguoiDung.TEN_NGUOI_DUNG;
                nguoidung.NGAY_SINH = nguoiDung.NGAY_SINH;
                nguoidung.SO_CHUNG_MINH = nguoiDung.SO_CHUNG_MINH;
                nguoidung.DIA_CHI = nguoiDung.DIA_CHI;
                nguoidung.SO_DIEN_THOAI = nguoiDung.SO_DIEN_THOAI;
                nguoidung.MA_KHO = nguoiDung.MA_KHO;
                nguoidung.USER_NAME = nguoiDung.USER_NAME;
                nguoidung.MAT_KHAU = nguoiDung.MAT_KHAU;
                nguoidung.NGAY_VAO_LAM = nguoiDung.NGAY_VAO_LAM;

                if (Request.Files[0].InputStream.Length != 0)
                {
                    Stream fileStream = Request.Files[0].InputStream;
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                    nguoidung.HINH_ANH = bytes;
                }
                nguoidung.MA_NHOM_NGUOI_DUNG = nguoiDung.MA_NHOM_NGUOI_DUNG;
                nguoidung.GHI_CHU = nguoiDung.GHI_CHU;
                nguoidung.ACTIVE = "A";
                nguoidung.UPDATE_AT = DateTime.Now;
                nguoidung.CREATE_AT = DateTime.Now;
                nguoidung.UPDATE_BY = (int)Session["UserId"];
                nguoidung.CREATE_BY = (int)Session["UserId"];
                db.NGUOI_DUNG.Add(nguoidung);
                db.SaveChanges();
                return Redirect("Index");
            }
            return View();
        }

        [HttpGet]
        public FileContentResult GetImage(int id)
        {
            using (var ctx = new SmsContext())
            {
                var nd = ctx.NGUOI_DUNG.FirstOrDefault(p => p.MA_NGUOI_DUNG == id);

                if (nd != null && nd.HINH_ANH != null)
                {
                    return File(nd.HINH_ANH, "image/png");
                }
            }
            return null;
        }

        private void BindKho()
        {
            using (var ctx = new SmsContext())
            {
                List<KHO> kho = ctx.KHOes.Where(m => m.ACTIVE.Equals("A")).ToList();
                ViewBag.Kho = kho;
            }
        }

        private void BindNhomNguoiDung()
        {
            using (var ctx = new SmsContext())
            {
                List<NHOM_NGUOI_DUNG> nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(m => m.ACTIVE.Equals("A")).ToList();
                ViewBag.NhomNguoiDung = nhomNguoiDung;
            }
        }
    }
}
