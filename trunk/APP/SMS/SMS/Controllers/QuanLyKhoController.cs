using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data.SqlClient;
using PagedList;
using System.Data;


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
        public ActionResult FifoReport(int? StoreId, int? ProductId, int? page, string StoreName, string ProductName )
        {
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductId = 0;
            }
            var idStoreParam = new SqlParameter
            {
                ParameterName = "MA_KHO",
                Value = Convert.ToInt32(StoreId)
            };
            var idProductParam = new SqlParameter
            {
                ParameterName = "MA_SAN_PHAM",
                Value = Convert.ToInt32(ProductId)
            };

            var totalExport = new SqlParameter
            {
                ParameterName = "GIA_VON_HANG_BAN_TOTAL",
                Value = Convert.ToDouble(0),
                Direction = ParameterDirection.Output
            };
            var totalLeft = new SqlParameter
            {
                ParameterName = "GIA_TRI_HANG_TON_TOTAL",
                Value = Convert.ToDouble(0),
                Direction = ParameterDirection.Output
            };
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var tonkho = ctx.Database.SqlQuery<Fifo>("exec STMA_GET_GIA_TRI_HANG_BAN_TON @MA_KHO, @MA_SAN_PHAM, @GIA_VON_HANG_BAN_TOTAL OUT, @GIA_TRI_HANG_TON_TOTAL OUT",
                idStoreParam,
                idProductParam,
                totalExport,
                totalLeft
                ).ToList<Fifo>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<Fifo> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            FifoModel model = new FifoModel();
            model.ResultList = tk;
            var exportValue = (double)totalExport.Value;
            var leftValue = (double)totalLeft.Value;
            model.GIA_VON_HANG_BAN_TOTAL = exportValue;
            model.GIA_TRI_HANG_TON_TOTAL = leftValue;
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            return View(model);
        }

        [HttpPost]
        public ActionResult FifoReport(int? StoreId, int? ProductId, int? page, bool? flag, string StoreName, string ProductName)
        {
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductId = 0;
            }

            var idStoreParam = new SqlParameter
            {
                ParameterName = "MA_KHO",
                Value = Convert.ToInt32(StoreId)
            };
            var idProductParam = new SqlParameter
            {
                ParameterName = "MA_SAN_PHAM",
                Value = Convert.ToInt32(ProductId)
            };

            var totalExport = new SqlParameter
            {
                ParameterName = "GIA_VON_HANG_BAN_TOTAL",
                Value = Convert.ToDouble(0),
                Direction = ParameterDirection.Output
            };
            var totalLeft = new SqlParameter
            {
                ParameterName = "GIA_TRI_HANG_TON_TOTAL",
                Value = Convert.ToDouble(0),
                Direction = ParameterDirection.Output
            };
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var tonkho = ctx.Database.SqlQuery<Fifo>("exec STMA_GET_GIA_TRI_HANG_BAN_TON @MA_KHO, @MA_SAN_PHAM, @GIA_VON_HANG_BAN_TOTAL OUT, @GIA_TRI_HANG_TON_TOTAL OUT",
                idStoreParam,
                idProductParam,
                totalExport,
                totalLeft
                ).ToList<Fifo>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<Fifo> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            FifoModel model = new FifoModel();
            model.ResultList = tk;
            var exportValue = (double)totalExport.Value;
            var leftValue = (double)totalLeft.Value;
            model.GIA_VON_HANG_BAN_TOTAL = exportValue;
            model.GIA_TRI_HANG_TON_TOTAL = leftValue;
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            return View(model);
        }

        [HttpGet]
        public ActionResult ImExDetail(int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page)
        {
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
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
            var tonkho = ctx.Database.SqlQuery<ImEx>("exec SP_GET_NHAP_XUAT @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<ImEx>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<ImEx> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ImExModel model = new ImExModel();
            model.ResultList = tk;
            return View(model);
        }


        [HttpPost]
        public ActionResult ImExDetail(int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page, bool? flag)
        {
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
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
            var tonkho = ctx.Database.SqlQuery<ImEx>("exec SP_GET_NHAP_XUAT @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<ImEx>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<ImEx> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ImExModel model = new ImExModel();
            model.ResultList = tk;
            return View(model);
        }

        [HttpGet]
        public ActionResult ExportReportDetail(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page)
        {
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            if (kind == null)
            {
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
            var tonkho = ctx.Database.SqlQuery<ExportReportDetail>("exec SP_EXPORT_REPORT_DETAIL @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<ExportReportDetail>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<ExportReportDetail> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ExportReportDetailModel model = new ExportReportDetailModel();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_EXPORT_REPORT_SUM @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
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
        public ActionResult ExportReportDetail(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page, bool?flag)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
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
            var tonkho = ctx.Database.SqlQuery<ExportReportDetail>("exec SP_EXPORT_REPORT_DETAIL @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<ExportReportDetail>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<ExportReportDetail> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ExportReportDetailModel model = new ExportReportDetailModel();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_EXPORT_REPORT_SUM @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
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

        public ActionResult ExportReport(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
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
            var tonkho = ctx.Database.SqlQuery<ExportRepot>("exec SP_EXPORT_REPORT @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<ExportRepot>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<ExportRepot> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ExportRepotModel model = new ExportRepotModel();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_EXPORT_REPORT_SUM @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
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
        public ActionResult ExportReport(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page, bool? flag)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
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
            var tonkho = ctx.Database.SqlQuery<ExportRepot>("exec SP_EXPORT_REPORT @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<ExportRepot>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<ExportRepot> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ExportRepotModel model = new ExportRepotModel();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_EXPORT_REPORT_SUM @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
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
        public ActionResult ImportRepoter(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
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
            var tonkho = ctx.Database.SqlQuery<SpImportRepoter>("exec SP_IMPORT_REPORTER @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<SpImportRepoter>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<SpImportRepoter> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            SpImportRepoterModel model = new SpImportRepoterModel();
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
        public ActionResult ImportRepoter(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page, bool? flag)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
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
            var tonkho = ctx.Database.SqlQuery<SpImportRepoter>("exec SP_IMPORT_REPORTER @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<SpImportRepoter>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<SpImportRepoter> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            SpImportRepoterModel model = new SpImportRepoterModel();
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
