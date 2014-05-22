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
    public class KhoController : Controller
    {
        //
        // GET: /Kho/

        [HttpGet]
        [CustomActionFilter]
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
            var theListContext = (from s in ctx.KHOes
                                  join u2 in ctx.NGUOI_DUNG on s.MA_NGUOI_DUNG_DAU equals u2.MA_NGUOI_DUNG
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.UPDATE_BY equals u1.MA_NGUOI_DUNG
                                  where (s.ACTIVE == "A" && (String.IsNullOrEmpty(searchString)
                                  || s.TEN_KHO.ToUpper().Contains(searchString.ToUpper())
                                  || s.SO_DIEN_THOAI.ToUpper().Contains(searchString.ToUpper())
                                  || u2.TEN_NGUOI_DUNG.ToUpper().Contains(searchString.ToUpper())
                                  || s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())))
                                  select new KhoModel
                                  {
                                      Kho = s,
                                      NguoiTao = u,
                                      NguoiDungDau = u2,
                                      NguoiCapNhat = u1
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;
            IPagedList<KhoModel> khuVucs = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
            switch (sortOrder)
            {
                case "id":
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.Kho.MA_KHO).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    khuVucs = theListContext.OrderByDescending(DonVi => DonVi.Kho.MA_KHO).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.Kho.TEN_KHO).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    khuVucs = theListContext.OrderByDescending(DonVi => DonVi.Kho.TEN_KHO).ToPagedList(pageIndex, pageSize);
                    break;
                case "admin_name":
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.NguoiDungDau.TEN_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                case "admin_name_desc":
                    khuVucs = theListContext.OrderByDescending(DonVi => DonVi.NguoiDungDau.TEN_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.Kho.MA_KHO).ToPagedList(pageIndex, pageSize);
                    break;
            }
            return View(khuVucs);
        }

        [HttpPost]
        [CustomActionFilter]
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

        [CustomActionFilter]
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
        [CustomActionFilter]
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
        [CustomActionFilter]
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
        [CustomActionFilter]
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
                khuVucNew.UPDATE_BY = (int)Session["UserId"];
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
        [CustomActionFilter]
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
                khuVuc.UPDATE_AT = DateTime.Now;
                khuVuc.UPDATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }

        }

        [HttpPost]
        public JsonResult FindSuggest(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.KHOes
                                    where (x.TEN_KHO.Contains(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_KHO,
                                        value = x.TEN_KHO
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }

    }
}
