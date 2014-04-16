using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;

namespace SMS.Controllers
{

    public class KhachHangController : Controller
    {
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
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.AdminSortParm = sortOrder == "admin_name" ? "admin_name_desc" : "admin_name";
            var theListContext = (from KhachHang in ctx.KHACH_HANG
                                  join KhuVuc in ctx.KHU_VUC 
                                  on KhachHang.MA_KHU_VUC equals KhuVuc.MA_KHU_VUC into kv
                                  from kVuc in kv.DefaultIfEmpty()
                                  join u in ctx.NGUOI_DUNG on KhachHang.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on KhachHang.UPDATE_BY equals u1.MA_NGUOI_DUNG
                                  where
                                  (KhachHang.ACTIVE == "A"
                                  && (String.IsNullOrEmpty(searchString)
                                  || KhachHang.TEN_KHACH_HANG.ToUpper().Contains(searchString.ToUpper())
                                  || KhachHang.MA_THE_KHACH_HANG.ToUpper().Contains(searchString.ToUpper())
                                  || KhachHang.SO_DIEN_THOAI.ToUpper().Contains(searchString.ToUpper())
                                  || kVuc.TEN_KHU_VUC.ToUpper().Contains(searchString.ToUpper()))
                                  )
                                  select new KhachHangModel
                                  {
                                      KhachHang = KhachHang,
                                      KhuVuc = kVuc, 
                                      NguoiCapNhat = u1, 
                                      NguoiTao = u
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;
            IPagedList<KhachHangModel> khachHangs = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            switch (sortOrder)
            {
                case "id":
                    khachHangs = theListContext.OrderBy(KhachHang => KhachHang.KhachHang.MA_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    khachHangs = theListContext.OrderByDescending(KhachHang => KhachHang.KhachHang.MA_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    khachHangs = theListContext.OrderBy(KhachHang => KhachHang.KhachHang.TEN_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    khachHangs = theListContext.OrderByDescending(KhachHang => KhachHang.KhachHang.TEN_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "admin_name":
                    khachHangs = theListContext.OrderBy(KhachHang => KhachHang.KhuVuc.TEN_KHU_VUC).ToPagedList(pageIndex, pageSize);
                    break;
                case "admin_name_desc":
                    khachHangs = theListContext.OrderByDescending(KhachHang => KhachHang.KhuVuc.TEN_KHU_VUC).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khachHangs = theListContext.OrderBy(KhachHang => KhachHang.KhachHang.MA_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
            }
            return View(khachHangs);
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
        public ActionResult UpdateDebit()
        {
            return View();
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
