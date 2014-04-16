using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using SMS.Models;
using PagedList;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    public class DonViController : Controller
    {
        //
        // GET: /DonVi/
        [HttpGet]
        public ActionResult Index(string searchString, string sortOrder, string currentFilter, int? page)
        {
            var ctx = new SmsContext();
            if (!String.IsNullOrEmpty(searchString) &&( page == null || page == 0))
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
            var theListContext = (from s in ctx.DON_VI_TINH
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  where (s.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) || s.TEN_DON_VI.ToUpper().Contains(searchString.ToUpper()) || s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())))
                                  select new DonViTinh
                                  {
                                      DonVi = s,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;
           
            IPagedList<DonViTinh> donViTinhs = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null? 1: (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            switch (sortOrder)
            {
                case "id":
                    donViTinhs = theListContext.OrderBy(DonVi => DonVi.DonVi.MA_DON_VI).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    donViTinhs = theListContext.OrderByDescending(DonVi => DonVi.DonVi.MA_DON_VI).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    donViTinhs = theListContext.OrderBy(DonVi => DonVi.DonVi.TEN_DON_VI).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    donViTinhs = theListContext.OrderByDescending(DonVi => DonVi.DonVi.TEN_DON_VI).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    donViTinhs = theListContext.OrderBy(DonVi => DonVi.DonVi.MA_DON_VI).ToPagedList(pageIndex, pageSize);
                    break;
            }
            return View(donViTinhs);
        }

        [HttpGet]
        public ActionResult AddNew()
        {
            
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy đơn vị tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            DON_VI_TINH donvi = ctx.DON_VI_TINH.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {
                ViewBag.donVi = donvi;
                return View(donvi);
            }else
            {
                ViewBag.Message = "Không tìm thấy đơn vị tương ứng.";
                return View("../Home/Error"); ;
            }
           
        }

       

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy đơn vị tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.DON_VI_TINH.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {
                donvi.ACTIVE = "I";
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Không tìm thấy đơn vị tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [HttpPost]
        public ActionResult Edit(SMS.Models.DON_VI_TINH donVi)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var donVitinh = db.DON_VI_TINH.Find((int)donVi.MA_DON_VI);
                donVitinh.TEN_DON_VI = donVi.TEN_DON_VI;
                donVitinh.GHI_CHU = donVi.GHI_CHU;
                donVitinh.ACTIVE = "A";
                donVitinh.UPDATE_AT = DateTime.Now;
                donVitinh.UPDATE_BY = (int)Session["UserId"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddNew(SMS.Models.DON_VI_TINH donVi)
        {
                if (ModelState.IsValid)
                {
                    var db = new SmsContext();
                    var donVitinh = db.DON_VI_TINH.Create();
                    donVitinh.TEN_DON_VI = donVi.TEN_DON_VI;
                    donVitinh.GHI_CHU = donVi.GHI_CHU;
                    donVitinh.ACTIVE = "A";
                    donVitinh.UPDATE_AT = DateTime.Now;
                    donVitinh.CREATE_AT = DateTime.Now;
                    donVitinh.UPDATE_BY = (int)Session["UserId"];
                    donVitinh.CREATE_BY = (int)Session["UserId"];
                    db.DON_VI_TINH.Add(donVitinh);
                    db.SaveChanges();
                    return Redirect("Index");
                }
                return View();
        }
    }
}
