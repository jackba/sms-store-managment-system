using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;

namespace SMS.Controllers
{
    public class SanPhamController : Controller
    {

     
        [HttpGet]
        public ActionResult Index(string sortOrder, string CurrentFilter, int? currentPageIndex)
        {
           
            //if (!String.IsNullOrEmpty(CurrentFilter))
            //{
            //    currentPageIndex = 1;
            //}
            //else
            //{
            //    ViewBag.CurrentFilter = CurrentFilter;
            //}

            IPagedList<SanPhamDisplay> listResult = LayDanhSachSanPham(sortOrder, CurrentFilter, currentPageIndex);

            return View(listResult);
        }


        [HttpGet]
        public ActionResult AddNew()
        {
            var ctx = new SmsContext();
            BindListDV(ctx);
            BindListNSX(ctx);
            SetModeTitle(false);
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy sản phẩm tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            SAN_PHAM sp = ctx.SAN_PHAM.Find(id);
            if (sp.ACTIVE.Equals("A"))
            {
                SetModeTitle(true);
                BindListDV(ctx);
                BindListNSX(ctx);

                SetHiddenFields(sp);

                return View("../SanPham/AddNew", sp);

            }
            else
            {
                ViewBag.Message = "Không tìm thấy sản phẩm tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [HttpPost]
        public ActionResult Edit(SAN_PHAM product)
        {
            var db = new SmsContext();
            if (ModelState.IsValid)
            {
                var sp = db.SAN_PHAM.Find((int)product.MA_SAN_PHAM);

                sp.TEN_SAN_PHAM = product.TEN_SAN_PHAM;
                sp.KICH_THUOC = product.KICH_THUOC;
                sp.CAN_NANG = product.CAN_NANG;
                if (-1 == product.MA_DON_VI)
                {
                    product.MA_DON_VI = null;
                }
                if (-1 == product.MA_NHA_SAN_XUAT)
                {
                    product.MA_NHA_SAN_XUAT = null;
                }
                sp.MA_DON_VI = product.MA_DON_VI;
                sp.MA_NHA_SAN_XUAT = product.MA_NHA_SAN_XUAT;
                sp.DAC_TA = product.DAC_TA;
                sp.GIA_BAN_1 = product.GIA_BAN_1;
                sp.GIA_BAN_2 = product.GIA_BAN_2;
                sp.GIA_BAN_3 = product.GIA_BAN_3;
                sp.CHIEC_KHAU_1 = product.CHIEC_KHAU_1;
                sp.CHIEC_KHAU_2 = product.CHIEC_KHAU_2;
                sp.CHIEC_KHAU_3 = product.CHIEC_KHAU_3;
                sp.CO_SO_TOI_THIEU = product.CO_SO_TOI_THIEU;
                sp.CO_SO_TOI_DA = product.CO_SO_TOI_DA;
                //common fields
                sp.ACTIVE = "A";
                sp.UPDATE_AT = DateTime.Now;
                sp.CREATE_AT = DateTime.Now;
                sp.UPDATE_BY = (int)Session["UserId"];
                sp.CREATE_BY = (int)Session["UserId"];

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            BindListDV(db);
            BindListNSX(db);
            SetModeTitle(true);
            SetHiddenFields(product);
            return View();
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
            var sp = ctx.SAN_PHAM.Find(id);
            if (sp.ACTIVE.Equals("A"))
            {
                sp.ACTIVE = "I";
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
        public ActionResult AddNew(SAN_PHAM product)
        {
            var db = new SmsContext();
            if (ModelState.IsValid)
            {
                var sp = db.SAN_PHAM.Create();
                // input fields
                sp.TEN_SAN_PHAM = product.TEN_SAN_PHAM;
                sp.KICH_THUOC = product.KICH_THUOC;
                sp.CAN_NANG = product.CAN_NANG;
                if (-1 == product.MA_DON_VI)
                {
                    product.MA_DON_VI = null;
                }
                if (-1 == product.MA_NHA_SAN_XUAT)
                {
                    product.MA_NHA_SAN_XUAT = null;
                }
                sp.MA_DON_VI = product.MA_DON_VI;
                sp.MA_NHA_SAN_XUAT = product.MA_NHA_SAN_XUAT;
                sp.DAC_TA = product.DAC_TA;
                sp.GIA_BAN_1 = product.GIA_BAN_1;
                sp.GIA_BAN_2 = product.GIA_BAN_2;
                sp.GIA_BAN_3 = product.GIA_BAN_3;
                sp.CHIEC_KHAU_1 = product.CHIEC_KHAU_1;
                sp.CHIEC_KHAU_2 = product.CHIEC_KHAU_2;
                sp.CHIEC_KHAU_3 = product.CHIEC_KHAU_3;
                sp.CO_SO_TOI_THIEU = product.CO_SO_TOI_THIEU;
                sp.CO_SO_TOI_DA = product.CO_SO_TOI_DA;
                //common fields
                sp.ACTIVE = "A";
                sp.UPDATE_AT = DateTime.Now;
                sp.CREATE_AT = DateTime.Now;
                sp.UPDATE_BY = (int)Session["UserId"];
                sp.CREATE_BY = (int)Session["UserId"];

                db.SAN_PHAM.Add(sp);
                db.SaveChanges();
                return Redirect("Index");
            }

            BindListDV(db);
            BindListNSX(db);
            SetModeTitle(false);
            SetHiddenFields(product);
            return View();
        }

        #region Common function
      

        private void BindListDV(SmsContext ctx)
        {
            var listDV = new List<DON_VI_TINH>();
            listDV.Add(new DON_VI_TINH { MA_DON_VI = -1, TEN_DON_VI = "Chọn đơn vị tính" });
            var dsDonVi = (from s in ctx.DON_VI_TINH select s).ToList<DON_VI_TINH>();
            if (null != dsDonVi)
            {
                listDV.AddRange(dsDonVi);
            }
            ViewBag.dsDonVi = listDV;

        }

        private void BindListNSX(SmsContext ctx)
        {
            var listNSX = new List<NHA_SAN_XUAT>();
            listNSX.Add(new NHA_SAN_XUAT { MA_NHA_SAN_XUAT = -1, TEN_NHA_SAN_XUAT = "Chọn nhà sản xuất" });
            var dsNSX = from s in ctx.NHA_SAN_XUAT select s;
            if (dsNSX != null && dsNSX.Count() > 0)
            {
                listNSX.AddRange(dsNSX);
            }
            ViewBag.dsNSX = listNSX;

        }
        private void SetHiddenFields(SAN_PHAM sp)
        {
            if (sp != null)
            {
                ViewBag.CanNang = sp.CAN_NANG;
                ViewBag.DVSelected = sp.MA_DON_VI;
                ViewBag.NSXSelected = sp.MA_NHA_SAN_XUAT;
                ViewBag.GiaBan1 = sp.GIA_BAN_1;
                ViewBag.GiaBan2 = sp.GIA_BAN_2;
                ViewBag.GiaBan3 = sp.GIA_BAN_3;
                ViewBag.ChietKhau1 = sp.CHIEC_KHAU_1;
                ViewBag.ChietKhau2 = sp.CHIEC_KHAU_2;
                ViewBag.ChietKhau3 = sp.CHIEC_KHAU_3;
                ViewBag.CoSoMin = sp.CO_SO_TOI_THIEU;
                ViewBag.CoSoMax = sp.CO_SO_TOI_DA;
            }

        }
        private void SetModeTitle(bool isModeUpdate)
        {
            if (isModeUpdate)
            {
                ViewBag.Title = "Cập nhật sản phẩm";
                ViewBag.Mode = "UPDATE";
            }
            else
            {
                ViewBag.Title = "Thêm mới sản phẩm";
                ViewBag.Mode = "CREATE";
            }
        }

        private IPagedList<SanPhamDisplay> LayDanhSachSanPham(string sortOrder, string CurrentFilter, int? currentPageIndex)
        {
            int pageSize = SystemConstant.ROWS;

            ViewBag.CurrentFilter = CurrentFilter;
            ViewBag.CurrentSort = sortOrder;

            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";

            var ctx = new SmsContext();
            var contentLst = (from s in ctx.SAN_PHAM
                                where (s.ACTIVE == "A"
                                && (String.IsNullOrEmpty(CurrentFilter)
                                || s.TEN_SAN_PHAM.ToUpper().Contains(CurrentFilter.ToUpper())
                                || s.DAC_TA.ToUpper().Contains(CurrentFilter.ToUpper())
                                || s.KICH_THUOC.ToUpper().Contains(CurrentFilter.ToUpper())))
                                join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                                join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                                join dv in ctx.DON_VI_TINH on s.MA_DON_VI equals dv.MA_DON_VI into dv_join
                                from dv in dv_join.DefaultIfEmpty()
                                join dv in ctx.NHA_SAN_XUAT on s.MA_NHA_SAN_XUAT equals dv.MA_NHA_SAN_XUAT into nsx_join
                                from nsx in nsx_join.DefaultIfEmpty()
                                select new SanPhamDisplay
                                {
                                    SanPham = s,
                                    NguoiTao = u,
                                    NguoiCapNhat = u1,
                                    DonVi = dv,
                                    NhaSanXuat = nsx
                                }).Take(SystemConstant.MAX_ROWS);



            IPagedList<SanPhamDisplay> DisplayContentLst = null;

            int pageIndex = (currentPageIndex ?? 1);

            switch (sortOrder)
            {
                case "id_desc":
                    DisplayContentLst = contentLst.OrderByDescending(u => u.SanPham.MA_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    DisplayContentLst = contentLst.OrderBy(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    DisplayContentLst = contentLst.OrderByDescending(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    DisplayContentLst = contentLst.OrderBy(u => u.SanPham.MA_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
            }

            ViewBag.CurrentFilter = CurrentFilter;
            ViewBag.DisplayContentLst = DisplayContentLst;
            return DisplayContentLst;
        }

        private object removeCommaInput(object value)
        {
            if (value != null && value.ToString() != null && value.ToString().Contains(",") == true)
            {
                value = value.ToString().Replace(",", "").Trim();
                return value;
            }
            return null;
        }

        #endregion
    }

}
