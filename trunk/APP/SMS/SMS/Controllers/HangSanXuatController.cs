using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;
using SMS.App_Start;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    public class HangSanXuatController : Controller
    {
        //
        // GET: /HangSanXuat/
        [CustomActionFilter]
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
            var theListContext = (from s in ctx.NHA_SAN_XUAT
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  where (s.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) || s.TEN_NHA_SAN_XUAT.ToUpper().Contains(searchString.ToUpper()) /*|| s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())*/))
                                  select new HangSanXuatModel
                                  {
                                      HangSanXuat = s,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;
            IPagedList<HangSanXuatModel> khuVucs = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
            switch (sortOrder)
            {
                case "id":
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.HangSanXuat.MA_NHA_SAN_XUAT).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    khuVucs = theListContext.OrderByDescending(DonVi => DonVi.HangSanXuat.MA_NHA_SAN_XUAT).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.HangSanXuat.TEN_NHA_SAN_XUAT).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    khuVucs = theListContext.OrderByDescending(DonVi => DonVi.HangSanXuat.TEN_NHA_SAN_XUAT).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.HangSanXuat.MA_NHA_SAN_XUAT).ToPagedList(pageIndex, pageSize);
                    break;
            }
            ctx.Dispose();
            return View(khuVucs);
        }

       
        [CustomActionFilter]
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
            ctx.Dispose();
            return View();
        }

        [CustomActionFilter]
        public ActionResult AddNew()
        {
            return View();
        }

        [CustomActionFilter]
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

        [CustomActionFilter]
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
                ctx.Dispose();
                return View(khuVuc);
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy hãng sản xuất tương ứng.";
                return View("../Home/Error"); ;
            }

        }

        [CustomActionFilter]
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

        [CustomActionFilter]
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
                ctx.Dispose();
                return RedirectToAction("Index");
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy hãng sản xuất tương ứng";
                return View("../Home/Error"); ;
            }
        }
    }
}
