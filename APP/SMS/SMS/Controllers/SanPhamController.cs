using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SMS.Controllers
{

    [Authorize]
    [HandleError]
    public class SanPhamController : Controller
    {
        public const string SEARCH_ADVANCE = "SearchAdvanceCondition";
        IPagedList<SanPhamDisplay> listResult = null;

        [HttpGet]
        public ActionResult Index(string sortOrder, string CurrentFilter, int? currentPageIndex)
        {
            Session[SEARCH_ADVANCE] = null;
            listResult = LayDanhSachSanPham(sortOrder, CurrentFilter, currentPageIndex);
            return View(listResult);
        }

        [HttpGet]
        public ActionResult ListPriceProducts()
        {          
            return View();
        }

        [HttpPost]
        public PartialViewResult PagingContent(string sortOrder, string CurrentFilter, int? currentPageIndex)
        {
            if (Session[SEARCH_ADVANCE] == null)
            {
                listResult = LayDanhSachSanPham(sortOrder, CurrentFilter, currentPageIndex);
            }
            else
            {
                ProductSA psa = (ProductSA)Session[SEARCH_ADVANCE];
                listResult = LayDanhSachSanPham(sortOrder, psa, currentPageIndex);
            }

            return PartialView("SanPhamPV", listResult);
        }

        [HttpPost]
        public JsonResult FindSuggest(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                 where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                 select new
                                 {
                                     id = x.MA_SAN_PHAM,
                                     value = x.TEN_SAN_PHAM
                                 };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestByTypeCustomer(string prefixText, string typeCustomer)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        name = x.TEN_SAN_PHAM,
                                        price = typeCustomer.Equals("1") ? x.GIA_BAN_1 ?? 0: 
                                                    (typeCustomer.Equals("2") ? x.GIA_BAN_2 ?? 0 : x.GIA_BAN_3 ?? 0),
                                        discount = typeCustomer.Equals("1") ? x.CHIEC_KHAU_1 ?? 0 : 
                                                    (typeCustomer.Equals("2") ? x.CHIEC_KHAU_2 ?? 0 : x.CHIEC_KHAU_3 ?? 0)
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
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
        public PartialViewResult SearchAdvance(FormCollection collection)
        {
            ProductSA psa = new ProductSA();
            if (collection.AllKeys.Contains("TenSanPham"))
            {
                psa.TenSanPham = collection.Get("TenSanPham");
            }
            if (collection.AllKeys.Contains("KichThuoc"))
            {
                psa.KichThuoc = collection.Get("KichThuoc");
            }
            if (collection.AllKeys.Contains("TrongLuongFrom"))
            {
                psa.TrongLuongFrom = collection.Get("TrongLuongFrom");
            }
            if (collection.AllKeys.Contains("TrongLuongTo"))
            {
                psa.TrongLuongTo = collection.Get("TrongLuongTo");
            }
            if (collection.AllKeys.Contains("DonViTinh"))
            {
                psa.DonViTinh = collection.Get("DonViTinh");
            }
            if (collection.AllKeys.Contains("NhaSanXuat"))
            {
                psa.NhaSanXuat = collection.Get("NhaSanXuat");
            }
            if (collection.AllKeys.Contains("DacTa"))
            {
                psa.DacTa = collection.Get("DacTa");
            }
            if (collection.AllKeys.Contains("GiaBanFrom"))
            {
                psa.GiaBanFrom = collection.Get("GiaBanFrom");
            }
            if (collection.AllKeys.Contains("GiaBanTo"))
            {
                psa.GiaBanTo = collection.Get("GiaBanTo");
            }

            if (collection.AllKeys.Contains("ChietKhauFrom"))
            {
                psa.ChietKhauFrom = collection.Get("ChietKhauFrom");
            }
            if (collection.AllKeys.Contains("ChietKhauTo"))
            {
                psa.ChietKhauTo = collection.Get("ChietKhauTo");
            }

            if (collection.AllKeys.Contains("CoSoFrom"))
            {
                psa.CoSoFrom = collection.Get("CoSoFrom");
            }
            if (collection.AllKeys.Contains("CoSoTo"))
            {
                psa.CoSoTo = collection.Get("CoSoTo");
            }


            Session[SEARCH_ADVANCE] = psa;

            IPagedList<SanPhamDisplay> listResult = LayDanhSachSanPham(null, psa, null);
            return PartialView("SanPhamPV", listResult);
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
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.CurrentFilter = CurrentFilter;
            ViewBag.DisplayContentLst = DisplayContentLst;
            return DisplayContentLst;
        }

        private IPagedList<SanPhamDisplay> LayDanhSachSanPham(string sortOrder, ProductSA psa, int? currentPageIndex)
        {
            int pageSize = SystemConstant.ROWS;

            ViewBag.CurrentSort = sortOrder;

            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";

            double weightFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.TrongLuongFrom) ? "0" : psa.TrongLuongFrom.Replace("'", ""), out weightFrom);
            double weightTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.TrongLuongTo) ? "0" : psa.TrongLuongTo.Replace("'", ""), out weightTo);
            double priceFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.GiaBanFrom) ? "0" : psa.GiaBanFrom.Replace("'", ""), out priceFrom);
            double priceTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.GiaBanTo) ? "0" : psa.GiaBanTo.Replace("'", ""), out priceTo);
            double discountFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.ChietKhauFrom) ? "0" : psa.ChietKhauFrom.Replace("'", ""), out discountFrom);
            double discountTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.ChietKhauTo) ? "0" : psa.ChietKhauTo.Replace("'", ""), out discountTo);
            double amoutFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.CoSoFrom) ? "0" : psa.CoSoFrom.Replace("'", ""), out amoutFrom);
            double amountTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.CoSoTo) ? "0" : psa.CoSoTo.Replace("'", ""), out amountTo);

            var ctx = new SmsContext();
            var contentLst = (from s in ctx.SAN_PHAM
                              where (s.ACTIVE == "A"
                                && (String.IsNullOrEmpty(psa.TenSanPham)
                                || s.TEN_SAN_PHAM.ToUpper().Contains(psa.TenSanPham.ToUpper()))
                                && (String.IsNullOrEmpty(psa.KichThuoc)
                                || s.KICH_THUOC.ToUpper().Contains(psa.KichThuoc.ToUpper()))
                               
                                && (String.IsNullOrEmpty(psa.TrongLuongFrom)
                                || s.CAN_NANG >= weightFrom)
                                && (String.IsNullOrEmpty(psa.TrongLuongTo)
                                || s.CAN_NANG <= weightTo)
                               
                                && (String.IsNullOrEmpty(psa.DonViTinh)
                                || s.DON_VI_TINH.TEN_DON_VI.ToUpper().Contains(psa.DonViTinh.ToUpper()))
                               
                                && (String.IsNullOrEmpty(psa.NhaSanXuat)
                                || s.NHA_SAN_XUAT.TEN_NHA_SAN_XUAT.ToUpper().Contains(psa.NhaSanXuat.ToUpper()))
                               
                                && (String.IsNullOrEmpty(psa.DacTa)
                                || s.DAC_TA.ToUpper().Contains(psa.DacTa.ToUpper()))

                                && (String.IsNullOrEmpty(psa.GiaBanFrom)
                                || s.GIA_BAN_1 >= priceFrom || s.GIA_BAN_2 >= priceFrom || s.GIA_BAN_3 >= priceFrom)
                                && (String.IsNullOrEmpty(psa.GiaBanTo)
                                || s.GIA_BAN_1 <= priceTo || s.GIA_BAN_2 <= priceTo || s.GIA_BAN_3 <= priceTo)

                                && (String.IsNullOrEmpty(psa.ChietKhauFrom)
                                || s.CHIEC_KHAU_1 >= discountFrom || s.CHIEC_KHAU_2 >= discountFrom || s.CHIEC_KHAU_3 >= discountFrom)
                                && (String.IsNullOrEmpty(psa.ChietKhauTo)
                                || s.CHIEC_KHAU_1 <= discountTo || s.CHIEC_KHAU_2 <= discountTo || s.CHIEC_KHAU_3 <= discountTo)

                                && (String.IsNullOrEmpty(psa.CoSoFrom)
                                || s.CO_SO_TOI_THIEU >= amoutFrom)
                                && (String.IsNullOrEmpty(psa.CoSoTo)
                                || s.CO_SO_TOI_DA <= amountTo)
                              )
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

            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.CurrentFilterAdvance = psa;
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

        [HttpGet]
        public ActionResult Warning(string SearchString, int? page, bool? flag)
        {
            IPagedList<SP_GET_TON_KHO_ALERT> tk = null;
            ViewBag.CurrentFilter = SearchString;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;

            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            var tonkho = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", new SqlParameter("NAME", string.IsNullOrEmpty(SearchString) ? "" : SearchString.Trim())).ToList<SP_GET_TON_KHO_ALERT>().Take(SystemConstant.MAX_ROWS); ;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = tonkho.Count();
            tk = tonkho.ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.KhoList = ListKho;
            ViewBag.tonKho = tk;
            GetTonKhoAlertModel model = new GetTonKhoAlertModel();
            model.WarningList = tk;
            return View(model);
        }

        [HttpPost]
        public ActionResult Warning(string SearchString, int? page)
        {
            IPagedList<SP_GET_TON_KHO_ALERT> tk = null;
            ViewBag.CurrentFilter = SearchString;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;

            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            var tonkho = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", new SqlParameter("NAME", string.IsNullOrEmpty(SearchString) ? "" : SearchString.Trim())).ToList<SP_GET_TON_KHO_ALERT>().Take(SystemConstant.MAX_ROWS); ;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = tonkho.Count();
            tk = tonkho.ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.KhoList = ListKho;
            ViewBag.tonKho = tk;
            GetTonKhoAlertModel model = new GetTonKhoAlertModel();
            model.WarningList = tk;
            return View(model);
        }
    }

}