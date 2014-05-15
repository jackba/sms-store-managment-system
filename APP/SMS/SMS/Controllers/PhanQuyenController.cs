﻿using PagedList;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.App_Start;

namespace SMS.Controllers
{
    [CustomActionFilter]
    public class PhanQuyenController : Controller
    {
        //
        // GET: /PhanQuyen/

        [HttpGet]
        public ActionResult Index()
        {
            var ctx = new SmsContext();
            var list = ctx.SP_GET_ALL_ROLE(0).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_ALL_ROLE_Result>();
            RoleModel model = new RoleModel();       
            int pageSize = SystemConstant.ROWS;
            int pageIndex = 1;
            model.RoleList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            return View(model);
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
                
                var oldPermission = db.PHAN_QUYEN.FirstOrDefault(u => u.MA_NGUOI_DUNG == phanQuyen.MA_NGUOI_DUNG 
                    && u.ACTIVE == "A");
                if (oldPermission != null)
                {
                    oldPermission.QUYEN_ADMIN = phanQuyen.QUYEN_ADMIN;
                    oldPermission.QUYEN_DANH_MUC_SAN_PHAM = phanQuyen.QUYEN_DANH_MUC_SAN_PHAM;
                    oldPermission.QUYEN_BAN_HANG = phanQuyen.QUYEN_BAN_HANG;
                    oldPermission.QUYEN_THAU_NGAN = phanQuyen.QUYEN_THAU_NGAN;
                    oldPermission.QUYEN_QUAN_LY_KHO = phanQuyen.QUYEN_QUAN_LY_KHO;
                    oldPermission.MA_NHOM_NGUOI_DUNG = phanQuyen.MA_NHOM_NGUOI_DUNG;
                    oldPermission.ACTIVE = "A";
                    oldPermission.UPDATE_AT = DateTime.Now;
                    oldPermission.CREATE_AT = DateTime.Now;
                    oldPermission.UPDATE_BY = (int)Session["UserId"];
                    oldPermission.CREATE_BY = (int)Session["UserId"];
                    db.SaveChanges();
                    return Redirect("Index");
                }else
                {
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