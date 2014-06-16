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
                    refund.IMPORT_FLG = true;
                    refund.UPDATE_AT = DateTime.Now;
                    refund.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();

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

                    CHI_TIET_NHAP_KHO importDetail;
                    foreach (var detail in model.Detail)
                    {
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

        [HttpPost]
        public PartialViewResult ReturnPurchaseListPartialView(string customerName, DateTime? fromDate,
            DateTime? toDate, int? userId, string userName, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = string.Empty;
                userId = 0;
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
            ViewBag.FromDate = fromDate;
            ViewBag.Todate = toDate;
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
