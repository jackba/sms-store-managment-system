using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class KhuVucController : Controller
    {


        [HttpPost]
        public JsonResult Find(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedUsers = from x in ctx.KHU_VUC
                                 where (x.TEN_KHU_VUC.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                 select new
                                 {
                                     id = x.MA_KHU_VUC,
                                     value = x.TEN_KHU_VUC
                                 };
            var result = Json(suggestedUsers.Take(10).ToList());
            ctx.Dispose();
            return result;
        }

        //
        // GET: /KhuVuc/
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
            var theListContext = (from s in ctx.KHU_VUC
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  where (s.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) || s.TEN_KHU_VUC.ToUpper().Contains(searchString.ToUpper()) || s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())))
                                  select new KhuVucModel
                                  {
                                      KhuVuc = s,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;
            IPagedList<KhuVucModel> khuVucs = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
            switch (sortOrder)
            {
                case "id":
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.KhuVuc.MA_KHU_VUC).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    khuVucs = theListContext.OrderByDescending(DonVi => DonVi.KhuVuc.MA_KHU_VUC).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.KhuVuc.TEN_KHU_VUC).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    khuVucs = theListContext.OrderByDescending(DonVi => DonVi.KhuVuc.TEN_KHU_VUC).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khuVucs = theListContext.OrderBy(DonVi => DonVi.KhuVuc.MA_KHU_VUC).ToPagedList(pageIndex, pageSize);
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
            var theListContext = (from s in ctx.KHU_VUC
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  where (s.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) || s.TEN_KHU_VUC.ToUpper().Contains(searchString.ToUpper()) || s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())))
                                  select new KhuVucModel
                                  {
                                      KhuVuc = s,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  });
            ViewBag.CurrentFilter = searchString;
            ViewBag.theList = theListContext;
            ctx.Dispose();
            return View();
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult AddNew()
        {
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult AddNew(SMS.Models.KHU_VUC khuVuc)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var khuVucNew = db.KHU_VUC.Create();
                khuVucNew.TEN_KHU_VUC = khuVuc.TEN_KHU_VUC;
                khuVucNew.GHI_CHU = khuVuc.GHI_CHU;
                khuVucNew.ACTIVE = "A";
                khuVucNew.UPDATE_AT = DateTime.Now;
                khuVucNew.CREATE_AT = DateTime.Now;
                khuVucNew.UPDATE_BY = (int)Session["UserId"];
                khuVucNew.CREATE_BY = (int)Session["UserId"];
                db.KHU_VUC.Add(khuVucNew);
                db.SaveChanges();
                return RedirectToAction("Index").Success("Lưu thành công");
            }
            return View();
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            KHU_VUC khuVuc = ctx.KHU_VUC.Find(id);
            if (khuVuc.ACTIVE.Equals("A"))
            {
                ctx.Dispose();
                return View(khuVuc);
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }

        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult Edit(SMS.Models.KHU_VUC khuVuc)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var khuvuc = db.KHU_VUC.Find((int)khuVuc.MA_KHU_VUC);
                khuvuc.TEN_KHU_VUC = khuVuc.TEN_KHU_VUC;
                khuvuc.GHI_CHU = khuVuc.GHI_CHU;
                khuvuc.ACTIVE = "A";
                khuvuc.UPDATE_AT = DateTime.Now;
                khuvuc.UPDATE_BY = (int)Session["UserId"];
                db.SaveChanges();
                return RedirectToAction("Index").Success("Lưu thành công.");
            }
            return View();
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.KHU_VUC.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {
                donvi.ACTIVE = "I";
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("Index").Success("Xóa thành công.");
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }
        }

    }
}
