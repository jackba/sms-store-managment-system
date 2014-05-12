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

namespace SMS.Controllers
{

    [Authorize]
    [HandleError]
    public class BanHangController : Controller
    {
        [HttpGet]
        public ActionResult LapHoaDon()
        {
            var ctx = new SmsContext();
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            ViewBag.KhoList = ListKho;
            return View();
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

        [HttpPost]
        public JsonResult FindDonViTinhByMaSP(string maSP)
        {
            var ctx = new SmsContext();
            ObjectResult<SP_GET_DON_VI_TINH_Result> resultDonViTinh = ctx.SP_GET_DON_VI_TINH(int.Parse(maSP));
            var result = Json(resultDonViTinh.Take(5).ToList());
            return result;
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
        public ActionResult ExportExcel(FormCollection collection)
        {
            string fileName = "BaoGiaKhachHang_";
            fileName += DateTime.Now.ToString("dd-MM-yyyy-HHmmss");
            fileName += ".xls";
            string listProductID = "";
            string[] arrProductID= new string[]{};

            if (collection.AllKeys.Contains("ListProductID"))
            {
                listProductID = collection.Get("ListProductID");
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

            return RedirectToAction("ListPriceProducts");
        }
    }

}