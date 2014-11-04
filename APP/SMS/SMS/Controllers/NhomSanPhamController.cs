using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using SMS.Models;
using PagedList;
using SMS.App_Start;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]   
    public class NhomSanPhamController : Controller
    {
        //
        // GET: /NhomSanPham/
        [HttpPost]
        public JsonResult FindSuggest(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.NHOM_SAN_PHAM
                                    where (x.TEN_NHOM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_NHOM,
                                        value = x.TEN_NHOM
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            ctx.Dispose();
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestConvertNotRootUnit(string prefixText, int rootUnitId)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.NHOM_SAN_PHAM
                                    where (x.TEN_NHOM.StartsWith(prefixText) && x.ACTIVE.Equals("A") && x.MA_NHOM  != rootUnitId)
                                    select new
                                    {
                                        id = x.MA_NHOM,
                                        value = x.TEN_NHOM
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            ctx.Dispose();
            return result;
        }

       

        [CustomActionFilter]
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
            var theListContext = (from s in ctx.NHOM_SAN_PHAM
                                  join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  where (s.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) || s.TEN_NHOM.ToUpper().Contains(searchString.ToUpper()) || s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())))
                                  select new NhomSanPhamClass
                                  {
                                      NhomSanPham = s,
                                      NguoiTao = u,
                                      NguoiCapNhat = u1
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;
           
            IPagedList<NhomSanPhamClass> nhomSPList = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null? 1: (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
            switch (sortOrder)
            {
                case "id":
                    nhomSPList = theListContext.OrderBy(Nhom => Nhom.NhomSanPham.MA_NHOM).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    nhomSPList = theListContext.OrderByDescending(Nhom => Nhom.NhomSanPham.MA_NHOM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    nhomSPList = theListContext.OrderBy(Nhom => Nhom.NhomSanPham.TEN_NHOM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    nhomSPList = theListContext.OrderByDescending(Nhom => Nhom.NhomSanPham.TEN_NHOM).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    nhomSPList = theListContext.OrderBy(Nhom => Nhom.NhomSanPham.MA_NHOM).ToPagedList(pageIndex, pageSize);
                    break;
            }
            ctx.Dispose();
            return View(nhomSPList);
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult AddNew()
        {
            
            return View();
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy nhóm sản phẩm tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            NHOM_SAN_PHAM nhomSP = ctx.NHOM_SAN_PHAM.Find(id);
            if (nhomSP.ACTIVE.Equals("A"))
            {
                ViewBag.donVi = nhomSP;
                ctx.Dispose();
                return View(nhomSP);
            }else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy nhóm sản phẩm tương ứng.";
                return View("../Home/Error"); ;
            }
           
        }

       
        [CustomActionFilter]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy nhóm sản phẩm tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var nhomSP = ctx.NHOM_SAN_PHAM.Find(id);
            if (nhomSP.ACTIVE.Equals("A"))
            {
                nhomSP.ACTIVE = "I";
                nhomSP.UPDATE_AT = DateTime.Now;
                nhomSP.CREATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("Index");
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy nhóm sản phẩm tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult Edit(SMS.Models.NHOM_SAN_PHAM pNhomSP)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var nhomSP = db.NHOM_SAN_PHAM.Find((int)pNhomSP.MA_NHOM);
                nhomSP.TEN_NHOM = pNhomSP.TEN_NHOM;
                nhomSP.GHI_CHU = pNhomSP.GHI_CHU;
                nhomSP.ACTIVE = "A";
                nhomSP.UPDATE_AT = DateTime.Now;
                nhomSP.UPDATE_BY = (int)Session["UserId"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult AddNew(SMS.Models.NHOM_SAN_PHAM pNhomSP)
        {
                if (ModelState.IsValid)
                {
                    var db = new SmsContext();
                    var nhomSP = db.NHOM_SAN_PHAM.Create();
                    nhomSP.TEN_NHOM = pNhomSP.TEN_NHOM;
                    nhomSP.GHI_CHU = pNhomSP.GHI_CHU;
                    nhomSP.ACTIVE = "A";
                    nhomSP.UPDATE_AT = DateTime.Now;
                    nhomSP.CREATE_AT = DateTime.Now;
                    nhomSP.UPDATE_BY = (int)Session["UserId"];
                    nhomSP.CREATE_BY = (int)Session["UserId"];
                    db.NHOM_SAN_PHAM.Add(nhomSP);
                    db.SaveChanges();
                    return Redirect("Index");
                }
                return View();
        }
    }
}
