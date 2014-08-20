using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;
using System.Transactions;

namespace SMS.Controllers
{
    public class CustomerRefundController : Controller
    {
        //
        // GET: /CustomerRefund/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReturnToProvider(ReturnToProviderModel model)
        {
            var ctx = new SmsContext();
            var refund = ctx.TRA_HANG.Find(model.Infor.MA_TRA_HANG);
            if (refund.RETURN_FLG != null && (bool)refund.RETURN_FLG)
            {
                return RedirectToAction("ReturnPurchaseList", new { @message = "Phiếu trả hàng này đã được lập phiếu trả cho nhà cung cấp, vui lòng liên hệ admin." });
            }
             using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var returnToProvider = ctx.TRA_HANG_NCC.Create();
                    returnToProvider.MA_PHIEU_TRA = refund.MA_TRA_HANG;
                    returnToProvider.NGUOI_LAP_PHIEU = Convert.ToInt32(Session["UserId"]);
                    returnToProvider.MA_NHA_CUNG_CAP = Convert.ToInt32(model.ProviderId);
                    returnToProvider.GHI_CHU = model.Note;
                    returnToProvider.NGAY_LAP_PHIEU = model.ReturnDate;
                    returnToProvider.CREATE_AT = DateTime.Now;
                    returnToProvider.UPDATE_AT = DateTime.Now;
                    returnToProvider.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    returnToProvider.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    returnToProvider.ACTIVE = "A";

                    ctx.TRA_HANG_NCC.Add(returnToProvider);
                    ctx.SaveChanges();

                    refund.RETURN_FLG = true;
                    TRA_HANG_NCC_CHI_TIET ct;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG == 1 || detail.SO_LUONG_TRA < detail.SO_LUONG_TON)
                        {
                            refund.RETURN_FLG = true;
                        }
                        if (detail.DEL_FLG != 1)
                        {
                            ct = ctx.TRA_HANG_NCC_CHI_TIET.Create();
                            ct.MA_PHIEU_TRA_NCC = returnToProvider.ID;
                            ct.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            ct.SO_LUONG = detail.SO_LUONG_TRA;
                            ct.UPDATE_AT = DateTime.Now;
                            ct.CREATE_AT = DateTime.Now;
                            ct.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.ACTIVE = "A";
                            //ct.DON_GIA = 
                            ct.DON_GIA = detail.GIA_VON;
                            ctx.TRA_HANG_NCC_CHI_TIET.Add(ct);
                            ctx.SaveChanges();
                        }
                    }
                    refund.UPDATE_AT = DateTime.Now;
                    refund.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();
                    transaction.Complete();
                    return RedirectToAction("ReturnPurchaseList", new { @inforMessage = "Lập phiếu trả hàng đến nhà cung cấp thành công." });
                }
                catch(Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("ReturnPurchaseList", new { @message = "Lập phiếu trả hàng đến nhà cung cấp thất bại, vui lòng liên hệ admin." });
                }
             }
        }


        [HttpPost]
        public ActionResult ImportRefund(RefundModel model)
        {
            var ctx = new SmsContext();
            var refund = ctx.TRA_HANG.Find(model.Infor.MA_TRA_HANG);
            if (refund.IMPORT_FLG != null && (bool)refund.IMPORT_FLG)
            {
                return RedirectToAction("ReturnPurchaseList", new { @message = "Phiếu trả hàng này đã được nhập kho, vui lòng liên hệ admin." });
            }
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                   

                    var import = ctx.NHAP_KHO.Create();
                    import.MA_KHO = model.MaKho;
                    import.NGAY_NHAP = model.NgayNhapKho;
                    import.LY_DO_NHAP = 1;
                    import.MA_PHIEU_TRA = model.Infor.MA_TRA_HANG;
                    import.NHAN_VIEN_NHAP = Convert.ToInt32(Session["UserId"]);
                    import.UPDATE_AT = DateTime.Now;
                    import.CREATE_AT = DateTime.Now;
                    import.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    import.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    import.ACTIVE = "A";

                    ctx.NHAP_KHO.Add(import);
                    ctx.SaveChanges();

                    refund.IMPORT_FLG = true; 
                    CHI_TIET_NHAP_KHO importDetail;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG == 1 || detail.SO_LUONG_TRA < detail.SO_LUONG_TON)
                        {
                            refund.IMPORT_FLG = false; 
                        }
                        if (detail.DEL_FLG != 1)
                        {
                            importDetail = ctx.CHI_TIET_NHAP_KHO.Create();
                            importDetail.MA_NHAP_KHO = import.MA_NHAP_KHO;
                            importDetail.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            importDetail.SO_LUONG = detail.SO_LUONG_TRA;
                            importDetail.GIA_VON = detail.GIA_VON;
                            importDetail.ACTIVE = "A";
                            importDetail.UPDATE_AT = DateTime.Now;
                            importDetail.CREATE_AT = DateTime.Now;
                            importDetail.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ctx.CHI_TIET_NHAP_KHO.Add(importDetail);
                            ctx.SaveChanges();
                        }
                    }

                   
                    refund.UPDATE_AT = DateTime.Now;
                    refund.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();

                    transaction.Complete();

                    return RedirectToAction("ReturnPurchaseList", new { @inforMessage = "Nhập kho - nhận trả hàng thành công." });
                }
                catch (Exception)
                {                    
                    Transaction.Current.Rollback();
                    return RedirectToAction("ReturnPurchaseList", new { @message = "Nhập kho thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        public ActionResult ImportRefund(int id)
        {
            var ctx = new SmsContext();
            var refund = ctx.TRA_HANG.Find(id);
            if (refund.ACTIVE != "A")
            {
                return RedirectToAction("ReturnPurchaseList", new { @inforMessage = "Không tìm thấy phiếu trả hàng nào tương ứng." });
            }
            var refundDetail = ctx.SP_GET_REFUND_DETAIL(id).ToList<SP_GET_REFUND_DETAIL_Result>();
            RefundModel model = new RefundModel();
            model.Infor = refund;
            model.Detail = refundDetail;
            if (!(bool)Session["IsAdmin"])
            {
                model.MaKho = Convert.ToInt32(Session["MyStore"]);
            }
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            ViewBag.Kho = stores;
            return View(model);
        }


        public ActionResult ReturnToProvider(int id)
        {
            var ctx = new SmsContext();
            var refund = ctx.TRA_HANG.Find(id);
            if (refund.ACTIVE != "A")
            {
                return RedirectToAction("ReturnPurchaseList", new { @inforMessage = "Không tìm thấy phiếu trả hàng nào tương ứng." });
            }
            var refundDetail = ctx.SP_GET_REFUND_DETAIL(id).ToList<SP_GET_REFUND_DETAIL_Result>();
            ReturnToProviderModel model = new ReturnToProviderModel();
            model.Infor = refund;
            model.Detail = refundDetail;
            var providers = ctx.NHA_CUNG_CAP.Where(u => u.ACTIVE == "A").ToList<NHA_CUNG_CAP>();
            ViewBag.Providers = providers;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult ReturnPurchaseListPartialView(string customerName, DateTime? fromDate,
            DateTime? toDate, int? userId, string userName, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = string.Empty;
                userId = 0;
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
            var resultList = ctx.SP_GET_RETURN_LIST(customerName, fromDate, toDate, userId, userName).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_RETURN_LIST_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            CustomerReturn model = new CustomerReturn();
            model.detailReturnList = resultList.ToPagedList(pageIndex, pageSize);
            model.Count = resultList.Count;
            ViewBag.CustomerName = customerName;
            ViewBag.UserId = userId;
            ViewBag.UserName = userName;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.Todate = ((DateTime)toDate).ToString("dd/MM/yyyy");
            return PartialView("ReturnPurchaseListPartialView", model);
        }

        public ActionResult ReturnPurchaseList(string message, string messageInfor)
        {
            ViewBag.Message = message;
            ViewBag.MessageInfor = messageInfor;
            return View();
        }

    }
}
