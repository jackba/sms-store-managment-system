using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;
using SMS.App_Start;
using System.Transactions;
using System.Data.SqlClient;
using System.Data;

namespace SMS.Controllers
{
    [CustomActionFilter]
    public class ImportController : Controller
    {

        [HttpPost]
        public JsonResult getInventory(string storeId, string productId)
        {
            var ctx = new SmsContext();
            var StoreIdParam = new SqlParameter
            {
                ParameterName = "STORE_ID",
                Value = Convert.ToInt32(storeId)
            };

            var ProductIdParam = new SqlParameter
            {
                ParameterName = "PRODUC_ID",
                Value = Convert.ToInt32(productId)
            };

            var returnValue = new SqlParameter
            {
                ParameterName = "RETURN_VAL",
                Value = Convert.ToInt32(0),
                Direction = ParameterDirection.Output
            };

            var tonkho = ctx.Database.SqlQuery<Object>("exec SP_GET_TON_KHO_BY_STORE_ID_AND_PRODUCT_ID @STORE_ID, @PRODUC_ID, @RETURN_VAL OUT ",
                StoreIdParam, ProductIdParam, returnValue).ToList<Object>().Take(SystemConstant.MAX_ROWS);
            var rv = returnValue.Value == DBNull.Value? 0: Convert.ToDouble(returnValue.Value);
            var result = Json(rv);
            return result;
        }

        //
        // GET: /Import/
        public ActionResult ListOfImportBill()
        {
            return View();
        }

        public ActionResult ListExportTransfer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExportTransfer(TransferModel model)
        {
            var ctx = new SmsContext();
            var infor = model.ExportInfor;
            var details = model.ExportDetail;
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var exInfor = ctx.XUAT_KHO.Create();
                    if (infor.SAVE_FLG == 1)
                    {
                        exInfor.ACTIVE = "W"; // waiting
                    }else
                    {
                        exInfor.ACTIVE = "A"; 
                    }
                    exInfor.MA_KHO_XUAT = infor.MA_KHO_XUAT;
                    exInfor.MA_KHO_NHAN = infor.MA_KHO_NHAN;
                    exInfor.GHI_CHU = infor.GHI_CHU;
                    exInfor.NGAY_XUAT = infor.NGAY_XUAT;
                    exInfor.LY_DO_XUAT = 3; // xuất chuyển kho
                    exInfor.CREATE_AT = DateTime.Now;
                    exInfor.UPDATE_AT = DateTime.Now;
                    exInfor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    exInfor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    exInfor.MA_NHAN_VIEN_XUAT = Convert.ToInt32(Session["UserId"]);
                    ctx.XUAT_KHO.Add(exInfor);
                    ctx.SaveChanges();

                    CHI_TIET_XUAT_KHO ct;

                    foreach (var detail in details)
                    {
                        if(detail.DEL_FLG != 1)
                        {
                            ct = ctx.CHI_TIET_XUAT_KHO.Create();
                            if(infor.SAVE_FLG == 1)
                            {
                                ct.ACTIVE = "W";
                            }else
                            {
                                ct.ACTIVE = "A";
                            }
                            ct.CREATE_AT = DateTime.Now;
                            ct.UPDATE_AT = DateTime.Now;
                            ct.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            ct.MA_DON_VI = detail.MA_DON_VI;
                            ct.MA_XUAT_KHO = exInfor.MA_XUAT_KHO;
                            ct.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            ct.SO_LUONG = detail.SO_LUONG_TEMP * detail.HE_SO;
                            ct.GIA_XUAT = 0;
                            ctx.CHI_TIET_XUAT_KHO.Add(ct);
                            ctx.SaveChanges();

                        }
                        
                    }
                    transaction.Complete();
                    return RedirectToAction("ListExportTransfer", new { @inforMessage = "Lưu thành công" });
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("ListExportTransfer", new { @message = "Lưu thất bại, vui lòng liên hệ admin." });
                }
            }
            return View();
        }
        public ActionResult ExportTransfer()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            TransferModel model = new TransferModel();
            if (!(bool)Session["IsAdmin"])
            {
                model.ExportInfor.MA_KHO_XUAT = Convert.ToInt32(Session["MyStore"]);
            }
            model.Stores = stores;
            model.Units = units;
            return View(model);
        }

        [HttpPost]
        public ActionResult Import(ImportModel model)
        {
            var ctx = new SmsContext();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var infor = ctx.NHAP_KHO.Create();
                    infor.MA_KHO = model.Infor.MA_KHO;
                    infor.MA_NHA_CUNG_CAP = model.Infor.MA_NHA_CUNG_CAP;
                    infor.NGAY_NHAP = model.Infor.NGAY_NHAP;
                    infor.NHAN_VIEN_NHAP = Convert.ToInt32(Session["UserId"]);
                    infor.SO_HOA_DON = model.Infor.SO_HOA_DON;
                    infor.CREATE_AT = DateTime.Now;
                    infor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.ACTIVE = "A";
                    infor.GHI_CHU = model.Infor.GHI_CHU;
                    infor.LY_DO_NHAP = 0; // nhập mua hàng
                    ctx.NHAP_KHO.Add(infor);
                    ctx.SaveChanges();

                    CHI_TIET_NHAP_KHO importDetail;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            importDetail = ctx.CHI_TIET_NHAP_KHO.Create();
                            importDetail.ACTIVE = "A";
                            importDetail.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            importDetail.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            importDetail.HE_SO = detail.HE_SO;
                            importDetail.SO_LUONG = detail.SO_LUONG_TEMP * detail.HE_SO;
                            importDetail.MA_DON_VI = detail.MA_DON_VI;
                            importDetail.MA_NHAP_KHO = infor.MA_NHAP_KHO;
                            importDetail.CREATE_AT = DateTime.Now;
                            importDetail.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.UPDATE_AT = DateTime.Now;
                            importDetail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.GIA_VON = detail.GIA_VON/detail.HE_SO;
                            importDetail.DON_GIA_TEMP = detail.GIA_VON;
                            ctx.CHI_TIET_NHAP_KHO.Add(importDetail);
                            ctx.SaveChanges();
                        }                        
                    }
                    transaction.Complete();
                    return RedirectToAction("Index", new { @inforMessage = "Nhận trả hàng thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("Index", new { @message = "Nhận trả hàng thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        public ActionResult Import()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var providers = ctx.NHA_CUNG_CAP.Where(u => u.ACTIVE == "A").ToList<NHA_CUNG_CAP>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            ViewBag.Stores = stores;
            ImportModel model = new ImportModel();
            if(!(bool)Session["IsAdmin"]){
                model.Infor.MA_KHO = Convert.ToInt32(Session["MyStore"]);
            }
            model.Stores = stores;
            model.Providers = providers;
            model.Units = units;
            ViewBag.InputKind = -1;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult IndexPartialView(DateTime? fromDate, DateTime? toDate, int? importerId, string importerName,
            int? reasonId, int? storeId, string storeName, int? providerId, string providerName, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            fromDate = fromDate == null ? SystemConstant.MIN_DATE : fromDate;
            toDate = toDate == null ? SystemConstant.MAX_DATE : toDate;
            ViewBag.InputKind = Convert.ToInt32(reasonId);

            if (string.IsNullOrEmpty(storeName))
            {
                storeName = string.Empty;
                storeId = 0;
            }

            if (string.IsNullOrEmpty(importerName))
            {
                importerName = string.Empty;
                importerId = 0;
            }

            if (string.IsNullOrEmpty(providerName))
            {
                providerName = string.Empty;
                providerId = 0;
            }

            if (!(bool)Session["IsAdmin"])
            {
                importerId = (int)Session["UserId"];
                storeId = (int)Session["MyStore"];
            }

            ViewBag.ProviderId = providerId;
            ViewBag.ProviderName = providerName;
            ViewBag.ImporterId = importerId;
            ViewBag.ImporterName = importerName;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            var resultList = ctx.SP_GET_IMPORT(fromDate, toDate, Convert.ToInt32(importerId), importerName, Convert.ToInt32(reasonId)
                , Convert.ToInt32(storeId), storeName, Convert.ToInt32(providerId), providerName).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_IMPORT_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ImportReportModel reportModel = new ImportReportModel();
            reportModel.ImportList = resultList.ToPagedList(pageIndex, pageSize);
            reportModel.PageCount = resultList.Count;
            return PartialView("IndexPartialView", reportModel);
        }

        [HttpGet]
        public ActionResult Index(int? reasonId)
        {
            ViewBag.InputKind = Convert.ToInt32(reasonId);
            return View();
        }
    }
}
