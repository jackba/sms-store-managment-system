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
    public class NhaCungCapController : Controller
    {
        /**
         * Tìm kiếm nhà cung cấp 
         **/
        [HttpPost]
        public JsonResult FindSuggest(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.NHA_CUNG_CAP
                                    where (x.TEN_NHA_CUNG_CAP.ToLower().Contains(prefixText.ToLower()) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_NHA_CUNG_CAP,
                                        value = x.TEN_NHA_CUNG_CAP
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }

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
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;
            IPagedList<NhaCungCapModel> khuVucs = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
            switch (sortOrder)
            {
                case "id":
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.NhaCungCap.MA_NHA_CUNG_CAP).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    khuVucs = theListContext.OrderByDescending(DonVi => DonVi.NhaCungCap.MA_NHA_CUNG_CAP).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.NhaCungCap.TEN_NHA_CUNG_CAP).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    khuVucs = theListContext.OrderByDescending(DonVi => DonVi.NhaCungCap.TEN_NHA_CUNG_CAP).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.NhaCungCap.MA_NHA_CUNG_CAP).ToPagedList(pageIndex, pageSize);
                    break;
            }
            return View(khuVucs);
        }

       
        [CustomActionFilter]
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

        [CustomActionFilter]
        public ActionResult AddNew()
        {
            return View();
        }

        [CustomActionFilter]
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

        [CustomActionFilter]
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

        [CustomActionFilter]
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

        [CustomActionFilter]
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
