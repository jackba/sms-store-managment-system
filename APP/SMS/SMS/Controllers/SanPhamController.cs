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
using System.IO;
using System.Web.UI;
using System.Text;
using SMS.App_Start;

namespace SMS.Controllers
{

    [Authorize]
    [HandleError]
    [CustomActionFilter]
    public class SanPhamController : Controller
    {
        public const string SEARCH_ADVANCE = "SearchAdvanceCondition";
        IPagedList<SanPhamDisplay> listResult = null;

        [HttpGet]
        public ActionResult Index()
        {
            Session[SEARCH_ADVANCE] = null;
            return View();
        }

        [HttpGet]
        public ActionResult ListPriceProducts()
        {          
            return View();
        }


        [HttpPost]
        public JsonResult FindProductByCode(string code)
        {
            var ctx = new SmsContext();
            var result = ctx.SAN_PHAM.Where(u => u.ACTIVE == "A" && u.CODE.ToLower() == code.ToLower()).FirstOrDefault();
            if (result == null)
            {
                return null;
            }
            var jresult = Json(new
            {
                ma_san_pham = result.MA_SAN_PHAM,
                ten_san_pham = result.TEN_SAN_PHAM,
                code = result.CODE,
                gia_ban_1 = result.GIA_BAN_1,
                gia_ban_2 = result.GIA_BAN_2,
                gia_ban_3 = result.GIA_BAN_3,
                chiec_khau_1 = result.CHIEC_KHAU_1,
                chiec_khau_2 = result.CHIEC_KHAU_2,
                chiec_khau_3 = result.CHIEC_KHAU_3,
            });
            return jresult;
        }

        [HttpPost]
        public PartialViewResult PagingContent(string sortOrder, string CurrentFilter, int? currentPageIndex)
        {
            if (Session[SEARCH_ADVANCE] == null)
            {
                listResult = GetListProductNotSearchAdvance(sortOrder, CurrentFilter, currentPageIndex);
            }
            else
            {
                ProductSA psa = (ProductSA)Session[SEARCH_ADVANCE];
                listResult = GetListProductSearchAdvance(sortOrder, psa, currentPageIndex);
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
        public JsonResult FindSuggestName(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        value = x.TEN_SAN_PHAM, 
                                        code = x.CODE
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestByCd(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    where (x.CODE.ToLower().StartsWith(prefixText.ToLower()) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        value = x.CODE,
                                        name = x.TEN_SAN_PHAM
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestForReturn(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    where ((x.CODE.ToLower().StartsWith(prefixText.ToLower())
                                    || x.TEN_SAN_PHAM.ToLower().StartsWith(prefixText.ToLower())) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        code = x.CODE,
                                        name = x.TEN_SAN_PHAM,
                                        price = (x.GIA_BAN_3 == null ? 0 : x.GIA_BAN_3*90/100)
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }


        [HttpPost]
        public JsonResult FindSuggestByCode(string prefixText, string typeCustomer)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    join u in ctx.DON_VI_TINH on x.MA_DON_VI equals u.MA_DON_VI
                                    where (x.CODE.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        code = x.CODE,
                                        label = x.CODE,
                                        name = x.TEN_SAN_PHAM,
                                        unit = x.MA_DON_VI,
                                        unitNm = u.TEN_DON_VI,
                                        price = typeCustomer.Equals("1") ? x.GIA_BAN_1 ?? 0 :
                                                    (typeCustomer.Equals("2") ? x.GIA_BAN_2 ?? 0 : x.GIA_BAN_3 ?? 0),
                                        discount = typeCustomer.Equals("1") ? x.CHIEC_KHAU_1 ?? 0 :
                                                    (typeCustomer.Equals("2") ? x.CHIEC_KHAU_2 ?? 0 : x.CHIEC_KHAU_3 ?? 0)
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestByTypeCustomer(string prefixText, string typeCustomer)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    join u in ctx.DON_VI_TINH on x.MA_DON_VI equals u.MA_DON_VI
                                    where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        code = x.CODE,
                                        name = x.TEN_SAN_PHAM,
                                        label = x.TEN_SAN_PHAM,    
                                        unit = x.MA_DON_VI,
                                        unitNm = u.TEN_DON_VI,
                                        price = typeCustomer.Equals("1") ? x.GIA_BAN_1 ?? 0: 
                                                    (typeCustomer.Equals("2") ? x.GIA_BAN_2 ?? 0 : x.GIA_BAN_3 ?? 0),
                                        discount = typeCustomer.Equals("1") ? x.CHIEC_KHAU_1 ?? 0 : 
                                                    (typeCustomer.Equals("2") ? x.CHIEC_KHAU_2 ?? 0 : x.CHIEC_KHAU_3 ?? 0)
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestConvert(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    join u in ctx.DON_VI_TINH on x.MA_DON_VI equals u.MA_DON_VI
                                    where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        value = x.TEN_SAN_PHAM,
                                        unitRoot = x.MA_DON_VI,
                                        unitName = u.TEN_DON_VI
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }

        [HttpPost]
        public ActionResult ExportExcel(FormCollection collection)
        {
            string fileName = "BaoGiaKhachHang_";
            fileName += DateTime.Now.ToString("dd-MM-yyyy-HHmmss");
            fileName += ".xls";
            string listProductID = "";
            string[] arrRowNum= new string[]{};

            if (collection.AllKeys.Contains("ListProductID"))
            {
                listProductID = collection.Get("ListProductID");
                arrRowNum = listProductID.Split(new char[] { ',' });
            }

            var products = new System.Data.DataTable("Products");
            products.Columns.Add("Mã Sản Phẩm", typeof(string));
            products.Columns.Add("Tên Sản Phẩm", typeof(string));
            products.Columns.Add("Giá bán", typeof(string));
            products.Columns.Add("Chiết khấu", typeof(string));
            products.Columns.Add("Giá thực", typeof(string));
            foreach (string RowNum in arrRowNum)
            {

                products.Rows.Add(RowNum.Split(new char[]{'_'})[0], 
                    collection.Get("TenSanPham_"  + RowNum) , collection.Get("GiaBan_" + RowNum) , 
                                       collection.Get("ChietKhau_"  + RowNum) , collection.Get("GiaThuc_" + RowNum));
            }

            var grid = new GridView();

            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.ms-excel";
            //Response.Charset = "utf-8";
            //Response.HeaderEncoding = Encoding.UTF8;
            //Response.ContentEncoding = Encoding.UTF8;
            Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"/>");

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("ListPriceProducts");
        }

        [HttpGet]
        public ActionResult AddNew()
        {
            var ctx = new SmsContext();
            BindListDV(ctx);
            BindListNSX(ctx);
            SetModeTitle(false);
            SetDefaultValue();
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            string msg =  "Không tìm thấy sản phẩm tương ứng.";
            if (id <= 0)
            {
                ViewBag.Message = msg; 
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
                ViewBag.Message = msg; 
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

            IPagedList<SanPhamDisplay> listResult = GetListProductSearchAdvance(null, psa, null);
            return PartialView("SanPhamPV", listResult);
        }
        [HttpPost]
        public ActionResult Edit(SAN_PHAM productUpdated)
        {
            var db = new SmsContext();
            var sp = db.SAN_PHAM.Find((int)productUpdated.MA_SAN_PHAM);

            sp.TEN_SAN_PHAM = productUpdated.TEN_SAN_PHAM;
            sp.KICH_THUOC = productUpdated.KICH_THUOC;
            sp.CAN_NANG = productUpdated.CAN_NANG;
            sp.MA_DON_VI = productUpdated.MA_DON_VI;
            
            if (-1 == productUpdated.MA_NHA_SAN_XUAT)
            {
                productUpdated.MA_NHA_SAN_XUAT = null;
            }
            sp.MA_DON_VI = productUpdated.MA_DON_VI;
            sp.MA_NHA_SAN_XUAT = productUpdated.MA_NHA_SAN_XUAT;
            sp.DAC_TA = productUpdated.DAC_TA;
            sp.GIA_BAN_1 = productUpdated.GIA_BAN_1;
            sp.GIA_BAN_2 = productUpdated.GIA_BAN_2;
            sp.GIA_BAN_3 = productUpdated.GIA_BAN_3;
            sp.CHIEC_KHAU_1 = productUpdated.CHIEC_KHAU_1;
            sp.CHIEC_KHAU_2 = productUpdated.CHIEC_KHAU_2;
            sp.CHIEC_KHAU_3 = productUpdated.CHIEC_KHAU_3;
            sp.CO_SO_TOI_THIEU = productUpdated.CO_SO_TOI_THIEU;
            sp.CO_SO_TOI_DA = productUpdated.CO_SO_TOI_DA;
            //common fields
            sp.ACTIVE = "A";
            sp.UPDATE_AT = DateTime.Now;
            sp.UPDATE_BY = (int)Session["UserId"];

            db.SaveChanges();
            return RedirectToAction("Index");
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
        public ActionResult AddNew(SAN_PHAM productInsert)
        {
            var db = new SmsContext();
                var sp = db.SAN_PHAM.Create();
                // input fields
                sp.TEN_SAN_PHAM = productInsert.TEN_SAN_PHAM;
                sp.KICH_THUOC = productInsert.KICH_THUOC;
                sp.CAN_NANG = productInsert.CAN_NANG;
                sp.MA_DON_VI = productInsert.MA_DON_VI;

                if (-1 == productInsert.MA_NHA_SAN_XUAT)
                {
                    productInsert.MA_NHA_SAN_XUAT = null;
                }
                sp.MA_DON_VI = productInsert.MA_DON_VI;
                sp.MA_NHA_SAN_XUAT = productInsert.MA_NHA_SAN_XUAT;
                sp.DAC_TA = productInsert.DAC_TA;
                sp.GIA_BAN_1 = productInsert.GIA_BAN_1;
                sp.GIA_BAN_2 = productInsert.GIA_BAN_2;
                sp.GIA_BAN_3 = productInsert.GIA_BAN_3;
                sp.CHIEC_KHAU_1 = productInsert.CHIEC_KHAU_1;
                sp.CHIEC_KHAU_2 = productInsert.CHIEC_KHAU_2;
                sp.CHIEC_KHAU_3 = productInsert.CHIEC_KHAU_3;
                sp.CO_SO_TOI_THIEU = productInsert.CO_SO_TOI_THIEU;
                sp.CO_SO_TOI_DA = productInsert.CO_SO_TOI_DA;
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

        private void SetDefaultValue()
        {
            ViewBag.CanNang = "0";
            ViewBag.GiaBan1 = "0";
            ViewBag.GiaBan2 = "0";
            ViewBag.GiaBan3 = "0";
            ViewBag.ChietKhau1 = "0";
            ViewBag.ChietKhau2 = "0";
            ViewBag.ChietKhau3 = "0";
            ViewBag.CoSoMin = "0";
            ViewBag.CoSoMax = "0";
        }
        private IPagedList<SanPhamDisplay> GetListProductNotSearchAdvance(string sortOrder, string CurrentFilter, int? currentPageIndex)
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
            ViewBag.Count = contentLst.Count();
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

        private IPagedList<SanPhamDisplay> GetListProductSearchAdvance(string sortOrder, ProductSA psa, int? currentPageIndex)
        {
            int pageSize = SystemConstant.ROWS;

            ViewBag.CurrentSort = sortOrder;

            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";

            double weightFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.TrongLuongFrom) ? "0" : psa.TrongLuongFrom.Replace(",", ""), out weightFrom);
            double weightTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.TrongLuongTo) ? "0" : psa.TrongLuongTo.Replace(",", ""), out weightTo);
            double priceFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.GiaBanFrom) ? "0" : psa.GiaBanFrom.Replace(",", ""), out priceFrom);
            double priceTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.GiaBanTo) ? "0" : psa.GiaBanTo.Replace(",", ""), out priceTo);
            double discountFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.ChietKhauFrom) ? "0" : psa.ChietKhauFrom.Replace(",", ""), out discountFrom);
            double discountTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.ChietKhauTo) ? "0" : psa.ChietKhauTo.Replace(",", ""), out discountTo);
            double amoutFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.CoSoFrom) ? "0" : psa.CoSoFrom.Replace(",", ""), out amoutFrom);
            double amountTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.CoSoTo) ? "0" : psa.CoSoTo.Replace(",", ""), out amountTo);

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
            ViewBag.Count = contentLst.Count();
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
           
            /*IPagedList<SP_GET_TON_KHO_ALERT> tk = null;
            ViewBag.CurrentFilter = SearchString;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;

            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            var tonkho = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", 
                new SqlParameter("NAME", string.IsNullOrEmpty(SearchString)
                    ? "" : SearchString.Trim())).ToList<SP_GET_TON_KHO_ALERT>().Take(SystemConstant.MAX_ROWS); ;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = tonkho.Count();
            tk = tonkho.ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.KhoList = ListKho;
            ViewBag.tonKho = tk;
            GetTonKhoAlertModel model = new GetTonKhoAlertModel();
            model.WarningList = tk;*/
            return View();
        }

        [HttpPost]
        public PartialViewResult PagingContentWarning(string SearchString, int? currentPageIndex)
        {

            IPagedList<SP_GET_TON_KHO_ALERT> tk = null;

            ViewBag.SearchString = SearchString;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            // var watch = Stopwatch.StartNew();
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            string s = ctx.Database.Connection.ToString();
            var tonkho = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", new SqlParameter("NAME", string.IsNullOrEmpty(SearchString) ? "" : SearchString.Trim())).ToList<SP_GET_TON_KHO_ALERT>().Take(SystemConstant.MAX_ROWS); ;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = tonkho.Count();
            tk = tonkho.ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.KhoList = ListKho;
            ViewBag.tonKho = tk;
            GetTonKhoAlertModel model = new GetTonKhoAlertModel();
            model.WarningList = tk;
            return PartialView("WarningPartialView", model);
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


        /*** CONVERT UNIT START **/
        [HttpGet]
        public ActionResult ConvertUnitOfProducts()
        {            
            return View();
        }

        [HttpPost]
        public PartialViewResult ConvertUnitOfProducts(string sortOrder, string CurrentFilter, int? currentPageIndex)
        {
             int pageSize = SystemConstant.ROWS;

            ViewBag.CurrentSort = sortOrder;

            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";

            var ctx = new SmsContext();
            var theListContext = (from cd in ctx.CHUYEN_DOI_DON_VI_TINH
                                  join sp in ctx.SAN_PHAM on cd.MA_SAN_PHAN equals sp.MA_SAN_PHAM
                                  join dv1 in ctx.DON_VI_TINH on sp.MA_DON_VI equals dv1.MA_DON_VI
                                  join dv2 in ctx.DON_VI_TINH on cd.MA_DON_VI_VAO equals dv2.MA_DON_VI
                                  join u in ctx.NGUOI_DUNG on cd.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on cd.UPDATE_BY equals u1.MA_NGUOI_DUNG
                                  where
                                  (cd.ACTIVE == "A"
                                  && (string.IsNullOrEmpty(CurrentFilter) 
                                    || sp.TEN_SAN_PHAM.ToLower().Contains(CurrentFilter.Trim().ToLower())
                                    || dv1.TEN_DON_VI.ToLower().Contains(CurrentFilter.Trim().ToLower())
                                    || dv2.TEN_DON_VI.ToLower().Contains(CurrentFilter.Trim().ToLower())))
                                  select new ChuyenDoiDonViTinhModel
                                  {
                                      ChuyenDoiDonVi = cd,
                                      SanPham = sp,
                                      DonViCuoi = dv1,
                                      DonViVao = dv2,
                                      NguoiCapNhat = u1,
                                      NguoiTao = u
                                  }).ToList<ChuyenDoiDonViTinhModel>();
            IPagedList<ChuyenDoiDonViTinhModel> khachHangs = null;
           
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
       
            switch (sortOrder)
            {
                case "id_desc":
                    khachHangs = theListContext.OrderByDescending(u => u.ChuyenDoiDonVi.MA_CHUYEN_DOI).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    khachHangs = theListContext.OrderBy(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    khachHangs = theListContext.OrderByDescending(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khachHangs = theListContext.OrderBy(u => u.ChuyenDoiDonVi.MA_CHUYEN_DOI).ToPagedList(pageIndex, pageSize);
                    break;
            }

            ViewBag.CurrentFilter = CurrentFilter;


            return PartialView("ListConvertUnitPartialView", khachHangs);
        }

        [HttpGet]
        public ActionResult AddNewConvertUnitOfProducts()
        {
            SetModeUnitTitle(false);
            ViewBag.HeSo = "2";
            return View();
        }

        [HttpPost]
        public ActionResult AddNewConvertUnitOfProducts(SMS.Models.CHUYEN_DOI_DON_VI_TINH convertUnitInsert)
        {
            var ctx = new SmsContext();
            var conUnit = ctx.CHUYEN_DOI_DON_VI_TINH.Create();
            conUnit.MA_SAN_PHAN = convertUnitInsert.MA_SAN_PHAN;
            conUnit.MA_DON_VI_VAO = convertUnitInsert.MA_DON_VI_VAO;
            conUnit.HE_SO = convertUnitInsert.HE_SO;

            conUnit.ACTIVE = "A";
            conUnit.UPDATE_AT = DateTime.Now;
            conUnit.CREATE_AT = DateTime.Now;
            conUnit.UPDATE_BY = (int)Session["UserId"];
            conUnit.CREATE_BY = (int)Session["UserId"];

            ctx.CHUYEN_DOI_DON_VI_TINH.Add(conUnit);
            ctx.SaveChanges();
            return Redirect("ConvertUnitOfProducts");
        }

        [HttpGet]
        public ActionResult EditConvertUnitOfProducts(int id)
        {
            string msg = "Không tìm thấy chuyển đổi đơn vị tương ứng.";
            if (id <= 0)
            {
                ViewBag.Message = msg;
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            CHUYEN_DOI_DON_VI_TINH cddv = ctx.CHUYEN_DOI_DON_VI_TINH.Find(id);
            if (cddv.ACTIVE.Equals("A"))
            {
                SetModeUnitTitle(true);

                SetHiddenConvertUnitFields(cddv);

                return View("../SanPham/AddNewConvertUnitOfProducts", cddv);

            }
            else
            {
                ViewBag.Message = msg;
                return View("../Home/Error"); ;
            }

        }

        [HttpPost]
        public ActionResult EditConvertUnitOfProducts(SMS.Models.CHUYEN_DOI_DON_VI_TINH convertUnitUpdated)
        {
            var db = new SmsContext();
            if (ModelState.IsValid)
            {
                var cddv = db.CHUYEN_DOI_DON_VI_TINH.Find((int)convertUnitUpdated.MA_CHUYEN_DOI);
                cddv.MA_SAN_PHAN = convertUnitUpdated.MA_SAN_PHAN;
                cddv.MA_DON_VI_VAO = convertUnitUpdated.MA_DON_VI_VAO;
                cddv.HE_SO = convertUnitUpdated.HE_SO;
                //common fields
                cddv.ACTIVE = "A";
                cddv.UPDATE_AT = DateTime.Now;
                cddv.UPDATE_BY = (int)Session["UserId"];

                db.SaveChanges();
                return RedirectToAction("ConvertUnitOfProducts");
            }

            return View();
        }

        private void SetModeUnitTitle(bool isModeUpdate)
        {
            if (isModeUpdate)
            {
                ViewBag.Title = "Cập nhật chuyển đổi đơn vị";
                ViewBag.ModeUnit = "UPDATE";
            }
            else
            {
                ViewBag.Title = "Thêm mới chuyển đổi đơn vị";
                ViewBag.ModeUnit = "CREATE";
            }
        }
        private void SetHiddenConvertUnitFields(CHUYEN_DOI_DON_VI_TINH cddv)
        {
            if (cddv != null)
            {
                var ctx = new SmsContext();
                SAN_PHAM sp = ctx.SAN_PHAM.Find(cddv.MA_SAN_PHAN);
                ViewBag.ProductName = sp.TEN_SAN_PHAM ;
                DON_VI_TINH dv = ctx.DON_VI_TINH.Find(cddv.MA_DON_VI_VAO);
                ViewBag.InputUnitName = dv.TEN_DON_VI ;
                ViewBag.HeSo = cddv.HE_SO;
                DON_VI_TINH dvroot = ctx.DON_VI_TINH.Find(sp.MA_DON_VI);
                ViewBag.UnitName = dvroot.TEN_DON_VI;
            }

        }
        [HttpGet]
        public ActionResult DeleteConvertUnit(int id)
        {
            string msg = "Không tìm thấy chuyển đổi đơn vị tương ứng.";
            if (id <= 0)
            {
                ViewBag.Message = msg;
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            CHUYEN_DOI_DON_VI_TINH cddv = ctx.CHUYEN_DOI_DON_VI_TINH.Find(id);
            if (cddv.ACTIVE.Equals("A"))
            {
                cddv.ACTIVE = "I";
                ctx.SaveChanges();
                return RedirectToAction("ConvertUnitOfProducts");
            }
            else
            {
                ViewBag.Message = msg;
                return View("../Home/Error"); ;
            }
        }
        /*** CONVERT UNIT END **/
    }

}