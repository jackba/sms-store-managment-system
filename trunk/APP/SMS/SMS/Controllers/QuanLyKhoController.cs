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

        public ActionResult ImportRepoter(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page)
        {
            return View();
        }

        public ActionResult ImportRepoterDetail(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName,DateTime? fromDate, DateTime?toDate ,int? page)
        {
            var ctx = new SmsContext();
            if (kind == null){
                kind = -1;
            }
            ViewBag.InputKind = kind;
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductId = 0;
            }

            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }else
            {
                ViewBag.FromDate = DateTime.Parse(fromDate.ToString()).ToString("dd/MM/yyyy");
            }

            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(toDate.ToString()).ToString("dd/MM/yyyy");
            }
            var tonkho = ctx.Database.SqlQuery<SP_IMPORT_REPORTER_DETAIL_Result>("exec SP_IMPORT_REPORTER_DETAIL @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<SP_IMPORT_REPORTER_DETAIL_Result>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<SP_IMPORT_REPORTER_DETAIL_Result> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ImportReportDetail model = new ImportReportDetail();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_IMPORT_REPORTER_SUM @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<InventoryTotal>().First();
            model.VALUE = (double)total.VALUE;
            return View(model);
        }
        
        [HttpPost]
        public ActionResult ImportRepoterDetail(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page, bool? flag)
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

            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromDate.ToString()).ToString("dd/MM/yyyy");
            }

            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(toDate.ToString()).ToString("dd/MM/yyyy");
            }
            var tonkho = ctx.Database.SqlQuery<SP_IMPORT_REPORTER_DETAIL_Result>("exec SP_IMPORT_REPORTER_DETAIL @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<SP_IMPORT_REPORTER_DETAIL_Result>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<SP_IMPORT_REPORTER_DETAIL_Result> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ImportReportDetail model = new ImportReportDetail();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_IMPORT_REPORTER_SUM @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<InventoryTotal>().First();
            model.VALUE = (double)total.VALUE;
            return View(model);
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
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<Inventory>().Take(SystemConstant.MAX_ROWS); 
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
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<Inventory>().Take(SystemConstant.MAX_ROWS);
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
