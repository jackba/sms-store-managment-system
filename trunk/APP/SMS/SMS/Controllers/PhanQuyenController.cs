﻿using PagedList;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class PhanQuyenController : Controller
    {
        //
        // GET: /PhanQuyen/

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
            var theListContext = (from u in ctx.PHAN_QUYEN
                                  join u1 in ctx.NGUOI_DUNG on u.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  join u2 in ctx.NGUOI_DUNG on u.UPDATE_BY equals u2.MA_NGUOI_DUNG
                                  where (u.ACTIVE == "A" && (String.IsNullOrEmpty(searchString)))
                                  select new PhanQuyenObj
                                  {
                                      PhanQuyen = u,
                                      NguoiTao = u1,
                                      NguoiCapNhat = u2
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;

            IPagedList<PhanQuyenObj> phanQuyens = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
            switch (sortOrder)
            {
                case "id":
                    phanQuyens = theListContext.OrderBy(pq => pq.PhanQuyen.MA_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    phanQuyens = theListContext.OrderByDescending(pq => pq.PhanQuyen.MA_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    phanQuyens = theListContext.OrderBy(pq => pq.PhanQuyen.MA_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
            }
            return View(phanQuyens);
        }

        [HttpGet]
        public ActionResult AddNew()
        {
            //Nguoi Dung
            BindNguoiDung();

            //Ma Nhom
            BindNhomNguoiDung();

            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy phân quyền tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            PHAN_QUYEN phanquyen = ctx.PHAN_QUYEN.Find(id);
            if (phanquyen.ACTIVE.Equals("A"))
            {
                //Nguoi Dung
                BindNguoiDung();

                //Ma Nhom
                BindNhomNguoiDung();

                ViewBag.phanQuyen = phanquyen;
                return View(phanquyen);
            }
            else
            {
                ViewBag.Message = "Không tìm thấy phân quyền tương ứng.";
                return View("../Home/Error"); ;
            }

        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy phân quyền tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var phanquyen = ctx.PHAN_QUYEN.Find(id);
            if (phanquyen.ACTIVE.Equals("A"))
            {
                phanquyen.ACTIVE = "I";
                phanquyen.UPDATE_AT = DateTime.Now;
                phanquyen.CREATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Không tìm thấy phân quyền tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [HttpPost]
        public ActionResult Edit(PHAN_QUYEN phanQuyen)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var phanquyen = db.PHAN_QUYEN.Find((int)phanQuyen.ID);

                phanquyen.QUYEN_ADMIN = phanQuyen.QUYEN_ADMIN;
                phanquyen.QUYEN_DANH_MUC_SAN_PHAM = phanQuyen.QUYEN_DANH_MUC_SAN_PHAM;
                phanquyen.QUYEN_BAN_HANG = phanQuyen.QUYEN_BAN_HANG;
                phanquyen.QUYEN_THAU_NGAN = phanQuyen.QUYEN_THAU_NGAN;
                phanquyen.QUYEN_QUAN_LY_KHO = phanQuyen.QUYEN_QUAN_LY_KHO;
                phanquyen.MA_NHOM_NGUOI_DUNG = phanQuyen.MA_NHOM_NGUOI_DUNG;

                phanquyen.ACTIVE = "A";
                phanquyen.UPDATE_AT = DateTime.Now;
                phanquyen.UPDATE_BY = (int)Session["UserId"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddNew(PHAN_QUYEN phanQuyen)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var phanquyen = db.PHAN_QUYEN.Create();

                phanquyen.MA_NGUOI_DUNG = phanQuyen.MA_NGUOI_DUNG;
                phanquyen.QUYEN_ADMIN = phanQuyen.QUYEN_ADMIN;
                phanquyen.QUYEN_DANH_MUC_SAN_PHAM = phanQuyen.QUYEN_DANH_MUC_SAN_PHAM;
                phanquyen.QUYEN_BAN_HANG = phanQuyen.QUYEN_BAN_HANG;
                phanquyen.QUYEN_THAU_NGAN = phanQuyen.QUYEN_THAU_NGAN;
                phanquyen.QUYEN_QUAN_LY_KHO = phanQuyen.QUYEN_QUAN_LY_KHO;
                phanquyen.MA_NHOM_NGUOI_DUNG = phanQuyen.MA_NHOM_NGUOI_DUNG;

                phanquyen.ACTIVE = "A";
                phanquyen.UPDATE_AT = DateTime.Now;
                phanquyen.CREATE_AT = DateTime.Now;
                phanquyen.UPDATE_BY = (int)Session["UserId"];
                phanquyen.CREATE_BY = (int)Session["UserId"];
                db.PHAN_QUYEN.Add(phanquyen);
                db.SaveChanges();
                return Redirect("Index");
            }
            return View();
        }

        private void BindNhomNguoiDung()
        {
            using (var ctx = new SmsContext())
            {
                List<NHOM_NGUOI_DUNG> nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(m => m.ACTIVE.Equals("A")).ToList();
                ViewBag.NhomNguoiDung = nhomNguoiDung;
            }
        }

        private void BindNguoiDung()
        {
            using (var ctx = new SmsContext())
            {
                List<NGUOI_DUNG> listNguoiDung = ctx.NGUOI_DUNG.Where(m => m.ACTIVE.Equals("A")).ToList();
                ViewBag.ListNguoiDung = listNguoiDung;
            }
        }
    }
}
