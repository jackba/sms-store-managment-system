using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data;
using PagedList;
using System.Data.SqlClient;
using System.Transactions;


namespace SMS.Controllers
{
    public class ExportController : Controller
    {
        //
        // GET: /Export/

        public ActionResult EditCancelTicket(int id)
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            var infor = ctx.XUAT_KHO.Where(u => u.ACTIVE == "A" && u.LY_DO_XUAT == 1 && u.MA_XUAT_KHO == id).FirstOrDefault();
            if (infor == null)
            {
                return RedirectToAction("ExportCancelList", new { @message = "Không tìm thấy phiếu xuất hủy này, vui lòng kiểm tra lại" });
            }
            EditCancelTicketModel model = new EditCancelTicketModel();
            if (!(bool)Session["IsAdmin"])
            {
                model.Infor.MA_KHO_XUAT = Convert.ToInt32(Session["MyStore"]);
            }
            model.Stores = stores;
            model.Units = units;
            model.Infor = infor;
            var detail = ctx.SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN(Convert.ToInt32(id)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN_Result>();
            model.Detail = detail;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditCancelTicket(EditCancelTicketModel model)
        {
            var ctx = new SmsContext();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var infor = ctx.XUAT_KHO.Create();
                    infor.MA_KHO_XUAT = model.Infor.MA_KHO_XUAT;
                    //infor.MA_KHO = model.Infor.MA_KHO;
                    //infor.MA_NHA_CUNG_CAP = model.Infor.MA_NHA_CUNG_CAP;
                    //infor.NGAY_NHAP = model.Infor.NGAY_NHAP;
                    infor.NGAY_XUAT = model.Infor.NGAY_XUAT;
                    infor.MA_NHAN_VIEN_XUAT = Convert.ToInt32(Session["UserId"]);
                    //infor.SO_HOA_DON = model.Infor.SO_HOA_DON;
                    infor.CREATE_AT = DateTime.Now;
                    infor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.ACTIVE = "A";
                    infor.GHI_CHU = model.Infor.GHI_CHU;
                    infor.LY_DO_XUAT = 1; // nhập mua hàng
                    ctx.XUAT_KHO.Add(infor);
                    ctx.SaveChanges();

                    ctx.CHI_TIET_XUAT_KHO.RemoveRange(ctx.CHI_TIET_XUAT_KHO.Where(u => u.MA_XUAT_KHO == model.Infor.MA_XUAT_KHO));
                    CHI_TIET_XUAT_KHO exportDetail;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            exportDetail = ctx.CHI_TIET_XUAT_KHO.Create();
                            exportDetail.ACTIVE = "A";
                            exportDetail.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            exportDetail.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            exportDetail.HE_SO = detail.HE_SO;
                            exportDetail.SO_LUONG = detail.SO_LUONG_TEMP * detail.HE_SO;
                            exportDetail.MA_DON_VI = detail.MA_DON_VI;
                            exportDetail.MA_XUAT_KHO = infor.MA_XUAT_KHO;
                            exportDetail.CREATE_AT = DateTime.Now;
                            exportDetail.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            exportDetail.UPDATE_AT = DateTime.Now;
                            exportDetail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            //exportDetail.GIA_VON = detail.GIA_VON / detail.HE_SO;
                            ctx.CHI_TIET_XUAT_KHO.Add(exportDetail);
                            ctx.SaveChanges();
                        }
                    }
                    transaction.Complete();
                    return RedirectToAction("ExportCancelList", new { @inforMessage = "Xuất hủy thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("ExportCancelList", new { @message = "Xuất hủy thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        public ActionResult ExportCancelList(string message, string inforMessage)
        {
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            return View();
        }

        [HttpPost]
        public PartialViewResult ExportCancelListPartialView(int? storeId, string storeName, int? exporterId,
            string exporterName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(exporterName))
            {
                exporterName = string.Empty;
                exporterId = 0;
            }
            if (string.IsNullOrEmpty(storeName))
            {
                storeName = string.Empty;
                storeId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
                exporterId = Convert.ToInt32(Session["UserId"]);
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var ctx = new SmsContext();
            var theList = ctx.SP_GET_EXPORT_4_CANCEL(storeId, storeName, exporterId, exporterName, fromDate, toDate).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_EXPORT_4_CANCEL_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            CancelExportModel model = new CancelExportModel();
            model.TheList = theList.ToPagedList(pageIndex, pageSize);
            model.Count = theList.Count;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.ExporterId = exporterId;
            ViewBag.ExporterName = exporterName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return PartialView("ExportCancelListPartialView", model);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy phiếu xuất kho tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var phieuXuat = ctx.XUAT_KHO.Find(id);
            if (phieuXuat.ACTIVE.Equals("A"))
            {
                var IdParam = new SqlParameter
                {
                    ParameterName = "MA_XUAT_KHO",
                    Value = Convert.ToInt32(id)
                };

                var UserIdParam = new SqlParameter
                {
                    ParameterName = "MA_NHAN_VIEN_TH",
                    Value = Convert.ToInt32(Session["UserId"])
                };

                var returnValue = new SqlParameter
                {
                    ParameterName = "RETURN_VALUE",
                    Value = Convert.ToInt32(0),
                    Direction = ParameterDirection.Output
                };
                var result = ctx.Database.ExecuteSqlCommand("exec SP_DELETE_EXPORT_4_SALE @MA_XUAT_KHO, @MA_NHAN_VIEN_TH, @RETURN_VALUE OUT",
                    IdParam,
                    UserIdParam,
                    returnValue
                    );
                int returnVal = Convert.ToInt32(returnValue.Value);
                if (returnVal == 1)
                {
                    return RedirectToAction("SaleExportList", new { @inforMessage = "Hủy thành công!" });
                }else
                {
                    return RedirectToAction("SaleExportList", new { @message = "Không thể hủy phiếu xuất kho này! Vui lòng thực hiện lại lần nữa." });
                }
            }
            else
            {
                ViewBag.Message = "Không tìm thấy phiếu xuất kho tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        public ActionResult SaleExportList(string message, string inforMessage)
        {
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            return View();
        }
        [HttpPost]
        public PartialViewResult SaleExportListPagingContent(int? storeId, string storeName, int? exporterId, string exporterName, int? customerId,
            string customerName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            

            if (string.IsNullOrEmpty(storeName))
            {
                storeName = string.Empty;
                storeId = 0;
            }
            if (string.IsNullOrEmpty(exporterName))
            {
                exporterName = string.Empty;
                exporterId = 0;
            }
            if (string.IsNullOrEmpty(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            

            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }

            if (!(bool)Session["IsAdmin"])
            {
                exporterId = Convert.ToInt32(Session["UserId"]);
            }

            var storeIdParam = new SqlParameter
            {
                ParameterName = "MA_KHO",
                Value = Convert.ToInt32(storeId)
            };

            var exporterIdParam = new SqlParameter
            {
                ParameterName = "MA_NHAN_VIEN_XUAT",
                Value = Convert.ToInt32(exporterId)
            };

            var exporterNameParam = new SqlParameter
            {
                ParameterName = "TEN_NHAN_VIEN_XUAT",
                Value = exporterName
            };

            var customerIdParam = new SqlParameter
            {
                ParameterName = "MA_KHACH_HANG",
                Value = Convert.ToInt32(customerId)
            };

            var customerNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHACH_HANG",
                Value = customerName
            };

            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }

            var fromDateParam = new SqlParameter
            {
                ParameterName = "FROM_DATE",
                Value = Convert.ToDateTime(fromDate)
            };

            var toDateParam = new SqlParameter
            {
                ParameterName = "TO_DATE",
                Value = Convert.ToDateTime(toDate)
            };

            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var exportList = ctx.Database.SqlQuery<SaleExportListModel>("Exec SP_GET_PHIEU_XUAT_KHO_BAN_LE @MA_KHO, @MA_NHAN_VIEN_XUAT, @TEN_NHAN_VIEN_XUAT, @MA_KHACH_HANG, @TEN_KHACH_HANG, @FROM_DATE , @TO_DATE",
               storeIdParam,
               exporterIdParam,
               exporterNameParam,
               customerIdParam,
               customerNameParam,
               fromDateParam,
               toDateParam
            ).ToList<SaleExportListModel>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ExportModel model = new ExportModel();
            model.SaleExportList = exportList.ToPagedList(pageIndex, pageSize);
            model.PageCount = exportList.Count;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = customerName;
            ViewBag.ExporterId = exporterId;
            ViewBag.ExporterName = exporterName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return PartialView("SaleExportListPartialView", model);
        }

        [HttpPost]
        public PartialViewResult SaleExportListPartialView(int? storeId, string storeName, int? exporterId, string exporterName, int? customerId,
            string customerName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(storeName))
            {
                storeName = string.Empty;
                storeId = 0;
            }
            if (string.IsNullOrEmpty(exporterName))
            {
                exporterName = string.Empty;
                exporterId = 0;
            }
            if (string.IsNullOrEmpty(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            var ctx = new SmsContext();

            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }

            if (!(bool)Session["IsAdmin"])
            {
                exporterId = Convert.ToInt32(Session["UserId"]);
            }

            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }

            var storeIdParam = new SqlParameter
            {
                ParameterName = "MA_KHO",
                Value = Convert.ToInt32(storeId)
            };

            var exporterIdParam = new SqlParameter
            {
                ParameterName = "MA_NHAN_VIEN_XUAT",
                Value = Convert.ToInt32(exporterId)
            };

            var exporterNameParam = new SqlParameter
            {
                ParameterName = "TEN_NHAN_VIEN_XUAT",
                Value = exporterName
            };

            var customerIdParam = new SqlParameter
            {
                ParameterName = "MA_KHACH_HANG",
                Value = Convert.ToInt32(customerId)
            };

            var customerNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHACH_HANG",
                Value = customerName
            };

            
            var fromDateParam = new SqlParameter
            {
                ParameterName = "FROM_DATE",
                Value = Convert.ToDateTime(fromDate)
            };

            var toDateParam = new SqlParameter
            {
                ParameterName = "TO_DATE",
                Value = Convert.ToDateTime(toDate)
            };

            ctx.Database.CommandTimeout = 300;
            var exportList = ctx.Database.SqlQuery<SaleExportListModel>("exec SP_GET_PHIEU_XUAT_KHO_BAN_LE @MA_KHO, @MA_NHAN_VIEN_XUAT, @TEN_NHAN_VIEN_XUAT, @MA_KHACH_HANG, @TEN_KHACH_HANG, @FROM_DATE , @TO_DATE",
                storeIdParam,
                exporterIdParam,
                exporterNameParam,
                customerIdParam,
                customerNameParam,
                fromDateParam, 
                toDateParam
             ).ToList<SaleExportListModel>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ExportModel model = new ExportModel();
            model.SaleExportList = exportList.ToPagedList(pageIndex, pageSize);
            model.PageCount = exportList.Count;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = customerName;
            ViewBag.ExporterId = exporterId;
            ViewBag.ExporterName = exporterName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return PartialView("SaleExportListPartialView", model);
        }

        public ActionResult ExportList()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult ExportListPartialView()
        {
            return PartialView("ExportListPartialView");
        }

        [HttpPost]
        public ActionResult Export(ExportModel model)
        {
            var ctx = new SmsContext();

            var storeId = new SqlParameter
            {
                ParameterName = "MA_KHO",
                Value = Convert.ToInt32(model.storeId)
            };

            var InvoiceId = new SqlParameter
            {
                ParameterName = "MA_HOA_DON",
                Value = Convert.ToInt32(model.Infor.MA_HOA_DON)
            };
            var UserId = new SqlParameter
            {
                ParameterName = "MA_NHAN_VIEN_THUC_HIEN",
                Value = Convert.ToInt32(Session["UserId"])
            };
            var customerName = new SqlParameter
            {
                ParameterName = "TEN_KHACH_HANG",
                Value = model.Infor.TEN_KHACH_HANG
            };
            var returnValue = new SqlParameter
            {
                ParameterName = "RETURN_VALUE",
                Value = Convert.ToInt32(0),
                Direction = ParameterDirection.Output
            };

            ctx.Database.CommandTimeout = 300;
            var export = ctx.Database.ExecuteSqlCommand("exec SP_SALE_EXPORT @MA_KHO, @MA_HOA_DON, @MA_NHAN_VIEN_THUC_HIEN, @TEN_KHACH_HANG , @RETURN_VALUE OUT",
                storeId,
                InvoiceId,
                UserId,
                customerName,
                returnValue
             );
            
            int returnVal = Convert.ToInt32(returnValue.Value);

            if(returnVal == -1)
            {
                return RedirectToAction("Index", new { @message = "Không thể xuất kho hóa đơn này. Lý do: có thể hóa đơn đã được xuất kho, hay đã bị hủy." });
            }
            else if (returnVal == 0)
            {
                ViewBag.Message = "Không đủ số lượng để xuất kho";
            }else
            {
                return RedirectToAction("Index", new { @messageInfor = "Xuất kho thành công" });
            }
            var infor = ctx.SP_GET_HOA_DON_INFO(model.storeId).FirstOrDefault();
            var detailList = ctx.SP_GET_HD_DETAIL_FOR_EXPORT(Convert.ToInt32(model.storeId), Convert.ToInt32(model.Infor.MA_HOA_DON)).ToList<SP_GET_HD_DETAIL_FOR_EXPORT_Result>();
            model.DetailList = detailList;
            model.Infor = infor;
            return View(model);
        }

        public ActionResult Export(int id, int? makho)
        {
            var ctx = new SmsContext();
            var infor =  ctx.SP_GET_HOA_DON_INFO(id).FirstOrDefault();
            if (makho == null)
            {
                makho = 0;
            }
            var detailList = ctx.SP_GET_HD_DETAIL_FOR_EXPORT(Convert.ToInt32(makho), Convert.ToInt32(id)).ToList<SP_GET_HD_DETAIL_FOR_EXPORT_Result>();
            ExportModel model = new ExportModel();
            model.DetailList = detailList;
            model.Infor = infor;
            model.storeId = Convert.ToInt32(makho);
            return View(model);
        }


        public ActionResult Index(string message, string messageInfor)
        {
            ViewBag.Message = message;
            ViewBag.MessageInfor = messageInfor;
            return View();
        }

        [HttpPost]
        public PartialViewResult PagingContent(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? storeId, string storeName, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            if (string.IsNullOrEmpty(storeName))
            {
                storeName = string.Empty;
                storeId = 0;
            }
            if (fromdate == null)
            {
                fromdate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
            }

            if (todate == null)
            {
                todate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(todate.ToString()).ToString("dd/MM/yyyy");
            }
            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var list = ctx.SP_GET_HOA_DON_CAN_XUAT_KHO(Convert.ToInt32(customerId), customerName, Convert.ToInt32(storeId), fromdate, todate).OrderByDescending(uh => uh.NGAY_BAN)
                .Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result>();
            ExportModel model = new ExportModel();
            model.WaitingList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.storeId = storeId;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = customerName;
            ViewBag.Todate = todate;
            ViewBag.Fromdate = fromdate;
            return PartialView("IndexPartialView", model);
        }

        [HttpPost]
        public PartialViewResult IndexPartialView(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? storeId, string storeName, int? currentPageIndex)
        {

            if (string.IsNullOrEmpty(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            if (string.IsNullOrEmpty(storeName))
            {
                storeName = string.Empty;
                storeId = 0;
            }
            if (fromdate == null)
            {
                fromdate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
            }

            if (todate == null)
            {
                todate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(todate.ToString()).ToString("dd/MM/yyyy");
            }
            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }

            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var list = ctx.SP_GET_HOA_DON_CAN_XUAT_KHO(Convert.ToInt32(customerId), customerName, Convert.ToInt32(storeId), fromdate, todate).OrderByDescending(uh => uh.NGAY_BAN)
                .Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result>();
            ExportModel model = new ExportModel();
            model.WaitingList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = customerName;
            ViewBag.Todate = todate;
            ViewBag.Fromdate = fromdate;
            return PartialView("IndexPartialView", model);
        }

        [HttpPost]
        public ActionResult XuatHuy(ExportModelXuatHuy model)
        {
            var ctx = new SmsContext();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var infor = ctx.XUAT_KHO.Create();
                    infor.MA_KHO_XUAT = model.Infor.MA_KHO_XUAT;
                    //infor.MA_KHO = model.Infor.MA_KHO;
                    //infor.MA_NHA_CUNG_CAP = model.Infor.MA_NHA_CUNG_CAP;
                    //infor.NGAY_NHAP = model.Infor.NGAY_NHAP;
                    infor.NGAY_XUAT = model.Infor.NGAY_XUAT;
                    infor.MA_NHAN_VIEN_XUAT = Convert.ToInt32(Session["UserId"]);
                    //infor.SO_HOA_DON = model.Infor.SO_HOA_DON;
                    infor.CREATE_AT = DateTime.Now;
                    infor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.ACTIVE = "A";
                    infor.GHI_CHU = model.Infor.GHI_CHU;
                    infor.LY_DO_XUAT = 1; // nhập mua hàng
                    ctx.XUAT_KHO.Add(infor);
                    ctx.SaveChanges();

                    CHI_TIET_XUAT_KHO exportDetail;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            exportDetail = ctx.CHI_TIET_XUAT_KHO.Create();
                            exportDetail.ACTIVE = "A";
                            exportDetail.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            exportDetail.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            exportDetail.HE_SO = detail.HE_SO;
                            exportDetail.SO_LUONG = detail.SO_LUONG_TEMP * detail.HE_SO;
                            exportDetail.MA_DON_VI = detail.MA_DON_VI;
                            exportDetail.MA_XUAT_KHO = infor.MA_XUAT_KHO;
                            exportDetail.CREATE_AT = DateTime.Now;
                            exportDetail.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            exportDetail.UPDATE_AT = DateTime.Now;
                            exportDetail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            exportDetail.GIA_VON = detail.GIA_VON / detail.HE_SO;
                            ctx.CHI_TIET_XUAT_KHO.Add(exportDetail);
                            ctx.SaveChanges();
                        }
                    }
                    transaction.Complete();
                    return RedirectToAction("ExportCancelList", new { @inforMessage = "Xuất hủy thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("ExportCancelList", new { @message = "Xuất hủy thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        public ActionResult XuatHuy()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            ViewBag.Stores = stores;
            ExportModelXuatHuy model = new ExportModelXuatHuy();
            if (!(bool)Session["IsAdmin"])
            {
                model.Infor.MA_KHO_XUAT = Convert.ToInt32(Session["MyStore"]);
            }
            model.Stores = stores;
            model.Units = units;
            ViewBag.InputKind = -1;
            return View(model);
        }
    }
}
