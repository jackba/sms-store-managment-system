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
using System.Data.Entity.Core.Objects;
using SMS.App_Start;
using System.Globalization;
using System.Transactions;

namespace SMS.Controllers
{

    [Authorize]
    [HandleError]
    [CustomActionFilter]
    public class BanHangController : Controller
    {
        [HttpGet]

        public ActionResult AddNew()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList(); ;
            HoaDonBanHang hoanDon = new HoaDonBanHang();
            hoanDon.Store = stores;
            hoanDon.Units = units;
            return View(hoanDon);
        }
        [HttpGet]
        public ActionResult HoaDonBanHang(int MaHoaDon)
        {
            var ctx = new SmsContext();
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            ViewBag.KhoList = ListKho;
            ViewBag.UserId = Session["UserId"];
            ViewBag.MyStore = Session["MyStore"];
            ViewBag.MaHoaDon = MaHoaDon;

            if ( MaHoaDon > 0)
            {
                HoaDonBanHang hdbh = new HoaDonBanHang();

                hdbh.KH_Info = getCustomerByBillNo(MaHoaDon);
                hdbh.lstChiTietHoaDon = getDetailsByBillNo(MaHoaDon);
                return View(hdbh);
            }
            return View();
        }
        // Chinh sua hoa don - START
        private KhachHangInfo getCustomerByBillNo(int MaHoaDon)
        {
            var ctx = new SmsContext();
            var hd = (from x in ctx.HOA_DON
                      join u in ctx.KHACH_HANG on x.MA_KHACH_HANG equals u.MA_KHACH_HANG into kh
                      from r in kh.DefaultIfEmpty()
                      where (x.MA_HOA_DON == MaHoaDon  && x.ACTIVE=="A")
                      select new KhachHangInfo
                      {
                          Ma_HD = x.MA_HOA_DON,
                          Ma_KH = (r == null ? -1 : r.MA_KHACH_HANG),
                          Ten_KH = (r == null ? x.TEN_KHACH_HANG : r.TEN_KHACH_HANG),
                          Ngay_Ban = x.NGAY_BAN,
                          Ngay_Giao = x.NGAY_GIAO,
                          Dia_Chi = x.DIA_CHI_GIAO_HANG
                      }).FirstOrDefault();
            return hd;
        }
        private List<ChiTiet_HoaDon> getDetailsByBillNo(int MaHoaDon)
        {
            var ctx = new SmsContext();
            //ctx.Configuration.LazyLoadingEnabled = true;
            var lstChiTietHD = (from x in ctx.HOA_DON
                      join u in ctx.CHI_TIET_HOA_DON.Include("KHO") on x.MA_HOA_DON equals u.MA_HOA_DON
                      join sp in ctx.SAN_PHAM.Include("DON_VI_TINH") on u.MA_SAN_PHAM equals sp.MA_SAN_PHAM
                      where (x.MA_HOA_DON == MaHoaDon && u.ACTIVE == "A" )
                      select new ChiTiet_HoaDon
                      {
                          Code_SP = sp.CODE,
                          Ma_SP = u.MA_SAN_PHAM,
                          Ten_SP = sp.TEN_SAN_PHAM,
                          Gia_Ban = u.GIA_BAN_TRUOC_CK,
                          Phan_Tram_CK  = u.PHAN_TRAM_CHIEC_KHAU,
                          Gia_Thuc = u.DON_GIA,
                          So_Luong = u.SO_LUONG,
                          Ma_Kho_Xuat = u.MA_KHO_XUAT,
                          Ten_Kho_Xuat = u.KHO.TEN_KHO,
                          Ma_Don_Vi = sp.DON_VI_TINH.MA_DON_VI,
                          Ten_Don_Vi = sp.DON_VI_TINH.TEN_DON_VI
                      }).ToList();
            return lstChiTietHD;
        }
        // Chinh sua hoa don - END 
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
            
            var result = Json(suggestedProducts.Take(10).ToList());
            return result;
        }
        [HttpPost]

        public JsonResult FindStore()
        {
            var ctx = new SmsContext();
            
            var store = from x in ctx.KHOes
                                    where (x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        value = x.MA_KHO,
                                        name = x.TEN_KHO
                                    };
            var result = Json(store.ToList());
            return result;
        }


        //[HttpPost]
        //public JsonResult FindSuggestByCode(string prefixText, string typeCustomer)
        //{
        //    var ctx = new SmsContext();
        //    var suggestedProducts = from x in ctx.SAN_PHAM
        //                            where (x.CODE.Contains(prefixText) && x.ACTIVE.Equals("A"))
        //                            select new
        //                            {
        //                                id = x.MA_SAN_PHAM,
        //                                name = x.TEN_SAN_PHAM,
        //                                price = typeCustomer.Equals("1") ? x.GIA_BAN_1 ?? 0 :
        //                                            (typeCustomer.Equals("2") ? x.GIA_BAN_2 ?? 0 : x.GIA_BAN_3 ?? 0),
        //                                discount = typeCustomer.Equals("1") ? x.CHIEC_KHAU_1 ?? 0 :
        //                                            (typeCustomer.Equals("2") ? x.CHIEC_KHAU_2 ?? 0 : x.CHIEC_KHAU_3 ?? 0)
        //                            };
        //    var result = Json(suggestedProducts.Take(10).ToList());
        //    return result;
        //}

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
            var result = Json(suggestedProducts.Take(10).ToList());
            
            return result;
        }

        [HttpPost]
        public JsonResult FindDonViTinhByMaSP(string maSP)
        {
            var ctx = new SmsContext();
            ObjectResult<SP_GET_DON_VI_TINH_Result> resultDonViTinh = ctx.SP_GET_DON_VI_TINH(int.Parse(maSP));
            var result = Json(resultDonViTinh.Take(100).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindFactorOfProduct(string maSP, string unitNo)
        {
            var ctx = new SmsContext();
            ObjectResult<SP_GET_DON_VI_TINH_Result> resultDonViTinh = ctx.SP_GET_DON_VI_TINH(int.Parse(maSP));
            foreach (SP_GET_DON_VI_TINH_Result dvt in resultDonViTinh)
            {
                if (int.Parse(unitNo) == dvt.MA_DON_VI)
                {
                   var result = Json(new { heso =  dvt.HE_SO });
                   return result;
                }
            }
            return null;
        }

        [HttpPost]
        public JsonResult FindFactorOfProduct1(int maSP, int unitNo)
        {
            var ctx = new SmsContext();
            var result = ctx.SP_GET_PRICE_BY_UNIT(maSP, unitNo).FirstOrDefault();
            if (result == null)
            {
                return null;
            }
            var jresult = Json(new { heso = result.HE_SO, 
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
        public JsonResult CheckingProductInAllStore(string maSP)
        {
            var ctx = new SmsContext();

            int productNo = Convert.ToInt32(maSP);
            var tonkho = ctx.Database.SqlQuery<CheckingStoreModel>("exec SP_GET_TON_KHO_BY_ID @INPUT_ID ", 
                new SqlParameter("INPUT_ID", productNo)).ToList<CheckingStoreModel>().Take(SystemConstant.MAX_ROWS);
            
            var result = Json(tonkho.ToList());
            return result;
        }

        [HttpPost]
        public ActionResult SaveBill(int MaHoaDon, FormCollection collection)
        {

            if (0 < MaHoaDon)
            {
                return updateBill(MaHoaDon, collection);
            }
            else
            {
                return insertBill(collection);
            }
        }

        private ActionResult insertBill(FormCollection collection)
        {
            int MaHD = -1;

            string SoHD = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();

            string listRowNum = "";
            string[] arrRowNum = new string[] { };

            if (collection.AllKeys.Contains("ListRowNum"))
            {
                listRowNum = collection.Get("ListRowNum");
                arrRowNum = listRowNum.Split(new char[] { ',' });
            }

            System.Text.StringBuilder msgStringBuilder = new System.Text.StringBuilder();
            var db = new SmsContext();

            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    //save HOA_DON
                    var hd = db.HOA_DON.Create();
                    hd.SO_HOA_DON = SoHD;
                    string maKH = collection.Get("MaKhachHang");
                    if (!String.IsNullOrEmpty(maKH))
                    {
                        hd.MA_KHACH_HANG = int.Parse(maKH);
                    }
                    else
                    {
                        hd.TEN_KHACH_HANG = collection.Get("TenKhachHang");
                    }
                    hd.MA_NHAN_VIEN_BAN = (int)Session["UserId"];

                    string dateSell = collection.Get("NgayBan");
                    string dateDelivery = collection.Get("NgayGiao");
                    if (!String.IsNullOrEmpty(dateSell))
                    {
                        hd.NGAY_BAN = DateTime.ParseExact(dateSell, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(dateDelivery))
                    {
                        hd.NGAY_GIAO = DateTime.ParseExact(dateDelivery, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    hd.DIA_CHI_GIAO_HANG = collection.Get("DiaChi");
                    hd.STATUS = 1;

                    //common fields
                    hd.ACTIVE = "A";
                    hd.UPDATE_AT = DateTime.Now;
                    hd.CREATE_AT = DateTime.Now;
                    hd.UPDATE_BY = (int)Session["UserId"];
                    hd.CREATE_BY = (int)Session["UserId"];

                    db.HOA_DON.Add(hd);
                    db.SaveChanges();

                    MaHD = hd.MA_HOA_DON;
                    //save CHI_TIET_HOA_DON
                    double SL = 0;
                    double GB = 0;
                    double GT = 0;
                    double CK = 0;

                    foreach (string rowNum in arrRowNum)
                    {
                        if (!String.IsNullOrEmpty(rowNum))
                        {
                            var cthd = db.CHI_TIET_HOA_DON.Create();

                            cthd.MA_HOA_DON = MaHD;

                            cthd.MA_SAN_PHAM = int.Parse(collection.Get("MaSanPham_" + rowNum));


                            double.TryParse(string.IsNullOrEmpty(collection.Get("SoLuong_" + rowNum)) ? "0" : collection.Get("SoLuong_" + rowNum).Replace(",", ""), out SL);
                            cthd.SO_LUONG = SL;

                            double.TryParse(string.IsNullOrEmpty(collection.Get("GiaBan_" + rowNum)) ? "0" : collection.Get("GiaBan_" + rowNum).Replace(",", ""), out GB);
                            cthd.GIA_BAN_TRUOC_CK = GB;

                            double.TryParse(string.IsNullOrEmpty(collection.Get("GiaThuc_" + rowNum)) ? "0" : collection.Get("GiaThuc_" + rowNum).Replace(",", ""), out GT);
                            cthd.DON_GIA = GT;

                            double.TryParse(string.IsNullOrEmpty(collection.Get("ChietKhau_" + rowNum)) ? "0" : collection.Get("ChietKhau_" + rowNum).Replace(",", ""), out CK);
                            cthd.PHAN_TRAM_CHIEC_KHAU = CK;

                            cthd.MA_KHO_XUAT = int.Parse(collection.Get("MaKho_" + rowNum));

                            //common fields
                            cthd.ACTIVE = "A";
                            cthd.UPDATE_AT = DateTime.Now;
                            cthd.CREATE_AT = DateTime.Now;
                            cthd.UPDATE_BY = (int)Session["UserId"];
                            cthd.CREATE_BY = (int)Session["UserId"];

                            db.CHI_TIET_HOA_DON.Add(cthd);
                            db.SaveChanges();
                        }
                    }

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    msgStringBuilder.Append(ex.Message);
                    return Content(msgStringBuilder.ToString());
                }
            }

            msgStringBuilder.Append("<div>Hóa đơn đã được lưu thành công.</div>");
            msgStringBuilder.Append("<div>Thông tin hóa đơn</div>");
            msgStringBuilder.Append("<div style=\"margin-left: 15px;\"> + Mã hóa đơn : " + MaHD + "</div>");
            msgStringBuilder.Append("<div style=\"margin-left: 15px;\"> + Số hóa đơn : " + SoHD + "</div>");
            return Content(msgStringBuilder.ToString());
        }

        private ActionResult updateBill(int MaHoaDon, FormCollection collection)
        {
            string SoHD = "" ;

            
            string listRowNum = "";
            string[] arrRowNum = new string[] { };

            if (collection.AllKeys.Contains("ListRowNum"))
            {
                listRowNum = collection.Get("ListRowNum");
                arrRowNum = listRowNum.Split(new char[] { ',' });
            }

            System.Text.StringBuilder msgStringBuilder = new System.Text.StringBuilder();
            var db = new SmsContext();
            var hd = db.HOA_DON.Find(MaHoaDon);
            SoHD = hd.SO_HOA_DON; 

            using (var transaction = new System.Transactions.TransactionScope())
            {
               
                try
                {
                    //save HOA_DON
                    //var hd = db.HOA_DON.Create();
                    //hd.SO_HOA_DON = SoHD;
                    string maKH = collection.Get("MaKhachHang");
                    if (!String.IsNullOrEmpty(maKH))
                    {
                        hd.MA_KHACH_HANG = int.Parse(maKH);
                    }
                    else
                    {
                        hd.TEN_KHACH_HANG = collection.Get("TenKhachHang");
                    }
                    hd.MA_NHAN_VIEN_BAN = (int)Session["UserId"];

                    string dateSell = collection.Get("NgayBan");
                    string dateDelivery = collection.Get("NgayGiao");
                    if (!String.IsNullOrEmpty(dateSell))
                    {
                        hd.NGAY_BAN = DateTime.ParseExact(dateSell, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(dateDelivery))
                    {
                        hd.NGAY_GIAO = DateTime.ParseExact(dateDelivery, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    hd.DIA_CHI_GIAO_HANG = collection.Get("DiaChi");
                    hd.STATUS = 1;

                    //common fields
                    hd.ACTIVE = "A";
                    hd.UPDATE_AT = DateTime.Now;
                    hd.UPDATE_BY = (int)Session["UserId"];

                    //db.HOA_DON.Add(hd);
                    db.SaveChanges();

                    //MaHD = hd.MA_HOA_DON;
                    //save CHI_TIET_HOA_DON
                    double SL = 0;
                    double GB = 0;
                    double GT = 0;
                    double CK = 0;

                    // update all data with MaHoaDon
                    //db.CHI_TIET_HOA_DON.Where(x => x.MA_HOA_DON == MaHoaDon)
                    //    .ToList().ForEach(a => a.ACTIVE = "I");

                    //remove all data with MaHoaDon
                    db.CHI_TIET_HOA_DON.RemoveRange(db.CHI_TIET_HOA_DON.Where(x => x.MA_HOA_DON == MaHoaDon));
                        
                    foreach (string rowNum in arrRowNum)
                    {
                        if (!String.IsNullOrEmpty(rowNum))
                        {
                            var cthd = db.CHI_TIET_HOA_DON.Create();

                            cthd.MA_HOA_DON = MaHoaDon;

                            cthd.MA_SAN_PHAM = int.Parse(collection.Get("MaSanPham_" + rowNum));


                            double.TryParse(string.IsNullOrEmpty(collection.Get("SoLuong_" + rowNum)) ? "0" : collection.Get("SoLuong_" + rowNum).Replace(",", ""), out SL);
                            cthd.SO_LUONG = SL;

                            double.TryParse(string.IsNullOrEmpty(collection.Get("GiaBan_" + rowNum)) ? "0" : collection.Get("GiaBan_" + rowNum).Replace(",", ""), out GB);
                            cthd.GIA_BAN_TRUOC_CK = GB;

                            double.TryParse(string.IsNullOrEmpty(collection.Get("GiaThuc_" + rowNum)) ? "0" : collection.Get("GiaThuc_" + rowNum).Replace(",", ""), out GT);
                            cthd.DON_GIA = GT;

                            double.TryParse(string.IsNullOrEmpty(collection.Get("ChietKhau_" + rowNum)) ? "0" : collection.Get("ChietKhau_" + rowNum).Replace(",", ""), out CK);
                            cthd.PHAN_TRAM_CHIEC_KHAU = CK;

                            cthd.MA_KHO_XUAT = int.Parse(collection.Get("MaKho_" + rowNum));

                            //common fields
                            cthd.ACTIVE = "A";
                            cthd.UPDATE_AT = DateTime.Now;
                            cthd.UPDATE_BY = (int)Session["UserId"];
                            cthd.CREATE_AT = DateTime.Now;
                            cthd.CREATE_BY = (int)Session["UserId"];

                            db.CHI_TIET_HOA_DON.Add(cthd);
                            db.SaveChanges();
                        }
                    }

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    msgStringBuilder.Append(ex.Message);
                    return Content(msgStringBuilder.ToString());
                }
            }

            msgStringBuilder.Append("<div>Hóa đơn đã được lưu thành công.</div>");
            msgStringBuilder.Append("<div>Thông tin hóa đơn</div>");
            msgStringBuilder.Append("<div style=\"margin-left: 15px;\"> + Mã hóa đơn : " + MaHoaDon + "</div>");
            msgStringBuilder.Append("<div style=\"margin-left: 15px;\"> + Số hóa đơn : " + SoHD + "</div>");
            return Content(msgStringBuilder.ToString());
        }

        [HttpGet]
        public ActionResult EditBill()
        {
            var ctx = new SmsContext();
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            ViewBag.KhoList = ListKho;
            ViewBag.UserId = Session["UserId"];
            ViewBag.MyStore = Session["MyStore"];
            return View();
        }
        

        [HttpPost]
        public ActionResult ExportExcel(FormCollection collection)
        {
            string fileName = "BaoGiaKhachHang_";
            fileName += DateTime.Now.ToString("dd-MM-yyyy-HHmmss");
            fileName += ".xls";
            string listProductID = "";
            string[] arrProductID= new string[]{};

            if (collection.AllKeys.Contains("ListRowNum"))
            {
                listProductID = collection.Get("ListRowNum");
                arrProductID = listProductID.Split(new char[] { ',' });
            }

            var products = new System.Data.DataTable("Products");
            products.Columns.Add("Mã Sản Phẩm", typeof(string));
            products.Columns.Add("Tên Sản Phẩm", typeof(string));
            products.Columns.Add("Giá bán", typeof(string));
            products.Columns.Add("Chiết khấu", typeof(string));
            products.Columns.Add("Giá thực", typeof(string));
            foreach (string id in arrProductID)
            {
                products.Rows.Add(id, collection.Get("TenSanPham_"  + id) , collection.Get("GiaBan_" + id) , 
                                       collection.Get("ChietKhau_"  + id) , collection.Get("GiaThuc_" + id));
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

            return Content("ListPriceProducts");
        }
    }

}