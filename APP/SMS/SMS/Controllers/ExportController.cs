﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data;
using PagedList;
using System.Data.SqlClient;
using System.Transactions;
using SMS.App_Start;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]   
    public class ExportController : Controller
    {

        public ActionResult ShowDetail(int id, int? flg)
        {
            ExportedDetailModel model = new ExportedDetailModel();
            model.flg = (int)flg;
            var ctx = new SmsContext();
            var theList = ctx.SP_GET_EXPORT_DETAIL_BY_ID(id).ToList<SP_GET_EXPORT_DETAIL_BY_ID_Result>();
            model.thelist = theList;
            return View(model);
        }

        public PartialViewResult ShowDetailPtv(int id)
        {
            var ctx = new SmsContext();
            var theList = ctx.SP_GET_EXPORT_DETAIL_BY_ID(id).ToList < SP_GET_EXPORT_DETAIL_BY_ID_Result>();
            ExportedDetailModel model = new ExportedDetailModel();
            model.thelist = theList;
            return PartialView("ShowDetailPtv", model);
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult Export2Provider(Export2ProviderModel model)
        {
            var ctx = new SmsContext();

            var storeId = new SqlParameter
            {
                ParameterName = "STORE_ID",
                Value = Convert.ToInt32(model.StoreId)
            };

            var InvoiceId = new SqlParameter
            {
                ParameterName = "ID",
                Value = Convert.ToInt32(model.Infor.ID)
            };
            var UserId = new SqlParameter
            {
                ParameterName = "USER_ID",
                Value = Convert.ToInt32(Session["UserId"])
            };
            var customerName = new SqlParameter
            {
                ParameterName = "EXPORT_DATE",
                Value = Convert.ToDateTime(model.exportDate)
            };
            var returnValue = new SqlParameter
            {
                ParameterName = "RETURN_VALUE",
                Value = Convert.ToInt32(0),
                Direction = ParameterDirection.Output
            };

            ctx.Database.CommandTimeout = 300;
            var export = ctx.Database.ExecuteSqlCommand("exec SP_EXPORT_4_RETURN_2_PROVIDER @ID, @STORE_ID, @USER_ID, @EXPORT_DATE , @RETURN_VALUE OUT",
                storeId,
                InvoiceId,
                UserId,
                customerName,
                returnValue
             );

            int returnVal = Convert.ToInt32(returnValue.Value);

            if (returnVal == -1)
            {
                ctx.Dispose();
                return RedirectToAction("WaitingExport2Provider", new { @message = "Không thể xuất kho hóa đơn này. Lý do: có thể hóa đơn đã được xuất kho, hay đã bị hủy." });
            }
            else if (returnVal == 0)
            {
                ViewBag.Message = "Không đủ số lượng để xuất kho";
                ctx.Dispose();
                //return RedirectToAction("WaitingExport2Provider", new { @message = "Không đủ số lượng để xuất kho." });
            }
            else
            {
                ctx.Dispose();
                return RedirectToAction("WaitingExport2Provider", new { @messageInfor = "Xuất kho thành công" });
            }
            return View(model);
        }



        [CustomActionFilter]
        public ActionResult Export2Provider(int id, int? storeId)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy phiếu xuất kho tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var Infor = ctx.TRA_HANG_NCC.Include("NHA_CUNG_CAP").Where(u => u.ACTIVE == "A" && u.ID == id).FirstOrDefault();
            if (Infor == null)
            {
                ViewBag.Message = "Không tìm thấy phiếu xuất kho tương ứng.";
                ctx.Dispose();
                return View("../Home/Error");
            }
            var details = ctx.SP_GET_DE_OF_RE_2_PR_BY_ST_AND_INV_ID(Convert.ToInt32(storeId), id).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_DE_OF_RE_2_PR_BY_ST_AND_INV_ID_Result>();
            Export2ProviderModel model = new Export2ProviderModel();
            model.TheList = details;
            model.Infor = Infor;
            model.StoreId = Convert.ToInt32(storeId);
            ctx.Dispose();
            return View(model);
        }

        [CustomActionFilter]
        public ActionResult DeleteExport2Provider(int id)
        {
            if (id < 0)
            {
                return RedirectToAction("WaitingExport2Provider", new { @message = "Không tồn tại phiếu xuất kho này. Vui lòng kiểm tra lại" });
            }
            var ctx = new SmsContext();
            var infor = ctx.XUAT_KHO.Find(id);
            if (infor == null || infor.ACTIVE != "A")
            {
                ctx.Dispose();
                return RedirectToAction("WaitingExport2Provider", new { @message = "Không tồn tại phiếu xuất kho này. Vui lòng kiểm tra lại" });
            }
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    infor.ACTIVE = "I";
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();
                    var details = ctx.CHI_TIET_XUAT_KHO.Where(u => u.ACTIVE == "A" && u.MA_XUAT_KHO == id).ToList<CHI_TIET_XUAT_KHO>();
                    foreach (var detail in details)
                    {
                        detail.ACTIVE = "I";
                        detail.UPDATE_AT = DateTime.Now;
                        detail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                        ctx.SaveChanges();
                    }
                    transaction.Complete();
                    ctx.Dispose();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    Transaction.Current.Rollback();
                    ctx.Dispose();
                    return RedirectToAction("WaitingExport2Provider", new { @message = "Hủy phiếu trả thất bại, vui lòng liên hệ admin." });
                }
            }
            ctx.Dispose();
            return RedirectToAction("WaitingExport2Provider", new { @messageInfor = "Xóa phiếu xuất kho thành công." });
        }

        [CustomActionFilter]
        public ActionResult DeleteExport(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy phiếu xuất kho tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.XUAT_KHO.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {

                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        donvi.ACTIVE = "I";
                        donvi.UPDATE_AT = DateTime.Now;
                        donvi.CREATE_BY = (int)Session["UserId"];
                        ctx.SaveChanges();

                        var details = ctx.CHI_TIET_XUAT_KHO.Where(u => u.ACTIVE == "A" && u.MA_XUAT_KHO == id).ToList<CHI_TIET_XUAT_KHO>();
                        foreach (var detail in details)
                        {
                            detail.ACTIVE = "I";
                            detail.UPDATE_AT = DateTime.Now;
                            detail.CREATE_BY = (int)Session["UserId"];
                            ctx.SaveChanges();
                        }
                        transaction.Complete();
                        ctx.Dispose();
                        return RedirectToAction("ExportCancelList", new { @messageInfor = "Xóa phiếu xuất kho thành công." });
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                        Transaction.Current.Rollback();
                        ctx.Dispose();
                        return RedirectToAction("ExportCancelList", new { @message = "Xóa phiếu xuất kho thất bại." });
                    }
                }

            }
            else
            {
                ViewBag.Message = "Không tìm thấy xuất kho tương ứng.";
                ctx.Dispose();
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        public ActionResult WaitingExport2Provider()
        {
            ViewBag.InputKind = 0;
            return View();
        }

        [HttpPost]
        public PartialViewResult WaitingExport2ProviderPartialView(int? status, int? storeId,
            string storeName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            if (string.IsNullOrWhiteSpace(storeName))
            {
                storeName = string.Empty;
                storeId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var thelist = ctx.SP_GET_WAITING_EX_2_PROVIDER(storeId, storeName, fromDate, toDate, status).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_WAITING_EX_2_PROVIDER_Result>();
            ViewBag.InputKind = status;
            ViewBag.StoreName = storeName;
            ViewBag.StoreId = storeId;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            WaitingExport2ProviderListModel model = new WaitingExport2ProviderListModel();
            model.TheList = thelist.ToPagedList(pageIndex, pageSize);
            model.Count = thelist.Count;
            ctx.Dispose();
            return PartialView("WaitingExport2ProviderPartialView", model);
        }

        [CustomActionFilter]
        public ActionResult EditCancelTicket(int id)
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            var infor = ctx.XUAT_KHO.Where(u => u.ACTIVE == "A" && u.LY_DO_XUAT == 1 && u.MA_XUAT_KHO == id).FirstOrDefault();
            if (infor == null)
            {
                return RedirectToAction("ExportCancelList", new { @message = "Không tìm thấy phiếu xuất hủy này, vui lòng kiểm tra lại." }).Error("Không tìm thấy phiếu xuất hủy này, vui lòng kiểm tra lại.");
            }
            var storeList = ctx.SP_GET_STORES_BY_USR_ID(Convert.ToInt32(Session["UserId"])).ToList<SP_GET_STORES_BY_USR_ID_Result>();
            EditCancelTicketModel model = new EditCancelTicketModel();
            model.Stores = stores;
            model.StoreList = storeList;
            model.Units = units;
            model.Infor = infor;
            var detail = ctx.SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN(Convert.ToInt32(id)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN_Result>();
            model.Detail = detail;
            ctx.Dispose();
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
                    var infor = ctx.XUAT_KHO.Find(model.Infor.MA_XUAT_KHO);
                    if (infor == null || infor.ACTIVE != "A")
                    {
                        return RedirectToAction("ExportCancelList", new { @message = "Phiếu xuất hủy này không tồn tại, vui lòng liên hệ admin." });
                    }
                    infor.MA_KHO_XUAT = model.Infor.MA_KHO_XUAT;                                    
                    infor.NGAY_XUAT = model.Infor.NGAY_XUAT;
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.GHI_CHU = model.Infor.GHI_CHU;
                    infor.LY_DO_XUAT = 1; 
                    ctx.SaveChanges();

                    ctx.CHI_TIET_XUAT_KHO.RemoveRange(ctx.CHI_TIET_XUAT_KHO.Where(u => u.MA_XUAT_KHO == model.Infor.MA_XUAT_KHO));
                    CHI_TIET_XUAT_KHO exportDetail;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG != 1 && detail.MA_SAN_PHAM != null && !string.IsNullOrWhiteSpace(detail.MA_SAN_PHAM.ToString()))
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
                            ctx.CHI_TIET_XUAT_KHO.Add(exportDetail);
                            ctx.SaveChanges();
                        }
                    }
                    transaction.Complete();
                    ctx.Dispose();
                    return RedirectToAction("ExportCancelList", new { @inforMessage = "Sửa phiếu xuất hủy thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    ctx.Dispose();
                    return RedirectToAction("ExportCancelList", new { @message = "Sửa phiếu xuất hủy thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        [CustomActionFilter]
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
            storeId = string.IsNullOrEmpty(storeName) || storeId == null ? 0 : storeId;
            exporterId = string.IsNullOrEmpty(exporterName) || exporterId == null ? 0 : exporterId;
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
            ctx.Dispose();
            return PartialView("ExportCancelListPartialView", model);
        }

        [CustomActionFilter]
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
                    ctx.Dispose();
                    return RedirectToAction("SaleExportList", new { @inforMessage = "Hủy thành công!" });
                }else
                {
                    ctx.Dispose();
                    return RedirectToAction("SaleExportList", new { @message = "Không thể hủy phiếu xuất kho này! Vui lòng thực hiện lại lần nữa." });
                }
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy phiếu xuất kho tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
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
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");
            ctx.Dispose();
            return PartialView("SaleExportListPartialView", model);
        }


        [HttpPost]
        public PartialViewResult SaleExportListPartialView(int? storeId, string storeName, int? exporterId, string exporterName, int? customerId,
            string customerName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
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

            var storeNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHO",
                Value = string.IsNullOrWhiteSpace(storeName) ? string.Empty : storeName
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
            var exportList = ctx.Database.SqlQuery<SaleExportListModel>("exec SP_GET_PHIEU_XUAT_KHO_BAN_LE @MA_KHO, @TEN_KHO, @MA_NHAN_VIEN_XUAT, @TEN_NHAN_VIEN_XUAT, @MA_KHACH_HANG, @TEN_KHACH_HANG, @FROM_DATE , @TO_DATE",
                storeIdParam,
                storeNameParam,
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
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");
            ctx.Dispose();
            return PartialView("SaleExportListPartialView", model);
        }

        [CustomActionFilter]
        public ActionResult ExportList()
        {
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public PartialViewResult ExportListPartialView()
        {
            return PartialView("ExportListPartialView");
        }

        [CustomActionFilter]
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
                ctx.Dispose();
                return RedirectToAction("Index", new { @message = "Không thể xuất kho hóa đơn này. Lý do: có thể hóa đơn đã được xuất kho, hay đã bị hủy." });
            }
            else if (returnVal == 0)
            {
                ViewBag.Message = "Không đủ số lượng để xuất kho";
            }else
            {
                ctx.Dispose();
                return RedirectToAction("Index", new { @messageInfor = "Xuất kho thành công" });
            }
            var infor = ctx.SP_GET_HOA_DON_INFO(model.Infor.MA_HOA_DON).FirstOrDefault();
            var detailList = ctx.SP_GET_HD_DETAIL_FOR_EXPORT(Convert.ToInt32(model.storeId), Convert.ToInt32(model.Infor.MA_HOA_DON)).ToList<SP_GET_HD_DETAIL_FOR_EXPORT_Result>();
            model.DetailList = detailList;
            model.Infor = infor;
            ctx.Dispose();
            return View(model);
        }

        [CustomActionFilter]
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
            ctx.Dispose();
            return View(model);
        }

        [CustomActionFilter]
        public ActionResult Index(string message, string messageInfor)
        {
            ViewBag.Message = message;
            ViewBag.MessageInfor = messageInfor;
            var ctx = new SmsContext();
            var storeList = ctx.SP_GET_STORES_BY_USR_ID(Convert.ToInt32(Session["UserId"])).ToList<SP_GET_STORES_BY_USR_ID_Result>();
            ExportModel model = new ExportModel();
            if (storeList != null && storeList.Count > 0)
            {
                model.storeId = (int)storeList.First().MA_KHO;
            }
            else
            {
                model.storeId = 0;
            }

            model.Stores = storeList;
            return View(model);
        }


        [HttpPost]
        public PartialViewResult PagingContent(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? storeId, string storeName, int? currentPageIndex)
        {
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
            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            var list = ctx.SP_GET_HOA_DON_CAN_XUAT_KHO(Convert.ToInt32(customerId), customerName, Convert.ToInt32(storeId), storeName, fromdate, todate).OrderByDescending(uh => uh.NGAY_BAN)
                .Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result>();
            ExportModel model = new ExportModel();
            model.WaitingList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.storeId = storeId;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = customerName;
            ViewBag.Todate = ((DateTime)todate).ToString("dd/MM/yyyy");
            ViewBag.Fromdate = ((DateTime)fromdate).ToString("dd/MM/yyyy");
            ctx.Dispose();
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
            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            var list = ctx.SP_GET_HOA_DON_CAN_XUAT_KHO(Convert.ToInt32(customerId), customerName, Convert.ToInt32(storeId), storeName, fromdate, todate).OrderByDescending(uh => uh.NGAY_BAN)
                .Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result>();
            ExportModel model = new ExportModel();
            model.WaitingList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = customerName;
            ViewBag.Todate = ((DateTime)todate).ToString("dd/MM/yyyy");
            ViewBag.Fromdate = ((DateTime)fromdate).ToString("dd/MM/yyyy");
            ctx.Dispose();
            return PartialView("IndexPartialView", model);
        }

        [CustomActionFilter]
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
                    
                    infor.NGAY_XUAT = model.Infor.NGAY_XUAT;
                    infor.MA_NHAN_VIEN_XUAT = Convert.ToInt32(Session["UserId"]);
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
                        if (detail.DEL_FLG != 1 && detail.MA_SAN_PHAM != null && !string.IsNullOrEmpty(detail.MA_SAN_PHAM.ToString()))
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
                    ctx.Dispose();
                    return RedirectToAction("ExportCancelList", new { @inforMessage = "Xuất hủy thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    ctx.Dispose();
                    return RedirectToAction("ExportCancelList", new { @message = "Xuất hủy thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        [CustomActionFilter]
        public ActionResult XuatHuy()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            ViewBag.Stores = stores;
            ExportModelXuatHuy model = new ExportModelXuatHuy();
            XUAT_KHO Infor = new XUAT_KHO();
            model.Infor = Infor;
            var storeList = ctx.SP_GET_STORES_BY_USR_ID(Convert.ToInt32(Session["UserId"])).ToList<SP_GET_STORES_BY_USR_ID_Result>();

            if (storeList != null && storeList.Count > 0)
            {
                model.Infor.MA_KHO_XUAT = storeList.First().MA_KHO;
            }
            model.Stores = stores;
            model.Units = units;
            model.StoreList = storeList;
            ViewBag.InputKind = -1;
            ctx.Dispose();
            return View(model);
        }
    }
}
