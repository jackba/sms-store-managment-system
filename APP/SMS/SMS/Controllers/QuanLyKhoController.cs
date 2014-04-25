using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data.SqlClient;
using PagedList;


namespace SMS.Controllers
{
    public class QuanLyKhoController : Controller
    {
        //
        // GET: /QuanLyKho/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Inventory(int? StoreId, int? ProductId, string StoreName, string ProductName, int? page, bool? flag)
        {
            var ctx = new SmsContext();
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductId = 0;
            }
            var tonkho = ctx.Database.SqlQuery<Inventory>("exec SP_GET_INVENTORY @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<Inventory>();
            ViewBag.Count = tonkho.Count();
            IPagedList<Inventory> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            GetInventoryModel model = new GetInventoryModel();
            model.InventoryList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_GET_VALUE_OF_INVENTORY @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<InventoryTotal>().First() ;
            model.VALUE = total.VALUE;
            return View(model);
        }

        [HttpPost]
        public ActionResult Inventory(int? StoreId, int? ProductId, string StoreName, string ProductName, int? page)
        {
            var ctx = new SmsContext();
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductId = 0;
            }

            var tonkho = ctx.Database.SqlQuery<Inventory>("exec SP_GET_INVENTORY @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<Inventory>();
            ViewBag.Count = tonkho.Count();
            IPagedList<Inventory> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            GetInventoryModel model = new GetInventoryModel();
            model.InventoryList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_GET_VALUE_OF_INVENTORY @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<InventoryTotal>().First();
            model.VALUE = total.VALUE;
            return View(model);
        }



    }
}
