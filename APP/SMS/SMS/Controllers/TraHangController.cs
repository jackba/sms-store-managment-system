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
    public class TraHangController : Controller
    {
        //
        // GET: /TraHang/

        public ActionResult ListOfToProvider(string message, string inforMessage)
        {
            var ctx = new SmsContext();
            ViewBag.Flag = 0;
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            return View();
        }

        [HttpPost]
        public PartialViewResult ListOfToProviderPartialView(int? providerId, string providerName, int? userId, 
            string userFullName, DateTime? fromDate, DateTime? toDate, int?flag, int?  currentPageIndex)
        {
            var ctx = new SmsContext();
            if (string.IsNullOrEmpty(providerName))
            {
                providerName = string.Empty;
                providerId = 0;
            }
            if (string.IsNullOrEmpty(userFullName))
            {
                userId = 0;
                userFullName = string.Empty;
            }
            if (!(bool)Session["IsAdmin"])
            {
                userId = Convert.ToInt32(Session["UserId"]);
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            ViewBag.Flag = Convert.ToInt32(flag);
            ViewBag.ProviderId = providerId;
            List2ProviderModel model = new List2ProviderModel();
            var detail = ctx.SP_GET_LIST_RETURN_TO_PROVIDERS(Convert.ToInt32(providerId),
                providerName, Convert.ToInt32(flag), fromDate, toDate, Convert.ToInt32(userId), userFullName).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_LIST_RETURN_TO_PROVIDERS_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            model.Detail = detail.ToPagedList(pageIndex, pageSize);
            model.Count = detail.Count;
            return PartialView("ListOfToProviderPartialView", model);
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
            if (!(bool)Session["IsAdmin"])
            {
                userId = Convert.ToInt32(Session["UserId"]);
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


        public ActionResult Show(int id, string message, string messageInfor)
        {
            var ctx = new SmsContext();
            var invoiceInfor = ctx.SP_GET_HOA_DON_INFO(id).FirstOrDefault();
            List<SP_GET_HOA_DON_DETAIL_FOR_RETURN_Result> detailList = 
                ctx.SP_GET_HOA_DON_DETAIL_FOR_RETURN(id).ToList<SP_GET_HOA_DON_DETAIL_FOR_RETURN_Result>();
            InvoicesModel model = new InvoicesModel();
            model.Infor = invoiceInfor;
            model.detailReturnList = detailList;
            ViewBag.Message = message;
            ViewBag.MessageInfor = messageInfor;
            return View(model);
        }

        [HttpPost]
        public ActionResult Show(InvoicesModel model)
        {
            var id = model.Infor.MA_HOA_DON;
            var returnList = model.detailReturnList;
            var ctx = new SmsContext();
            var hoaDon = ctx.V_HOA_DON.Where(u => u.MA_HOA_DON == id).FirstOrDefault();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var hoadon = ctx.HOA_DON.Find(id);
                    if (hoadon.STATUS == 4)
                    {
                        return RedirectToAction("Index", new { @message = "Hóa đơn này ." });
                    }
                    hoadon.STATUS = 4;
                    hoadon.UPDATE_AT = DateTime.Now;
                    hoadon.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();
                    var phieutra = ctx.TRA_HANG.Create();
                    phieutra.MA_HOA_DON = id;
                    phieutra.NGAY_TRA = DateTime.Now;
                    phieutra.NHAN_VIEN_NHAN = Convert.ToInt32(Session["UserId"]);
                    phieutra.STATUS = 1;
                    phieutra.TEN_KHACH_HANG = hoaDon.TEN_KHACH_HANG;
                    phieutra.CREATE_AT = DateTime.Now;
                    phieutra.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    phieutra.UPDATE_AT = DateTime.Now;
                    phieutra.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    phieutra.ACTIVE = "A";
                    ctx.TRA_HANG.Add(phieutra);
                    ctx.SaveChanges();
                    int maPhieuTra = phieutra.MA_TRA_HANG;
                    CHI_TIET_TRA_HANG chitiet = null;
                    foreach (var detail in model.detailReturnList)
                    {
                        if (Convert.ToInt32(detail.DEL_FLG) != 1)
                        {
                            chitiet = ctx.CHI_TIET_TRA_HANG.Create();
                            chitiet.MA_TRA_HANG = maPhieuTra;
                            chitiet.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            chitiet.SO_LUONG_TRA = detail.SO_LUONG;
                            chitiet.GIA_VON = detail.DON_GIA;
                            chitiet.ACTIVE = "A";
                            chitiet.CREATE_AT = DateTime.Now;
                            chitiet.UPDATE_AT = DateTime.Now;
                            chitiet.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            chitiet.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ctx.CHI_TIET_TRA_HANG.Add(chitiet);
                            ctx.SaveChanges();
                        }
                    }
                    transaction.Complete();
                    return RedirectToAction("Index", new { @inforMessage  = "Nhận trả hàng thành công."});
                }
                catch (Exception)
                {                    
                    Transaction.Current.Rollback();
                    return RedirectToAction("Index", new { @message = "Nhận trả hàng thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        public ActionResult Index(string message, string inforMessage)
        {
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            return View();
        }

        [HttpPost]
        public PartialViewResult IndexPartialView(int? billId, string billCode,
            int? customerId, string customerName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            if (string.IsNullOrEmpty(billCode))
            {
                billCode = string.Empty;
                billId = 0;
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
            var exportedList = ctx.SP_GET_HOA_DON_EXPORTED(Convert.ToInt32(billId), billCode,
                Convert.ToInt32(customerId), customerName, Convert.ToDateTime(fromDate),
                Convert.ToDateTime(toDate)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_EXPORTED_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ExportModel model = new ExportModel();
            model.ExportedList = exportedList.ToPagedList(pageIndex, pageSize);
            model.PageCount = exportedList.Count;
            ViewBag.BillId = billId;
            ViewBag.BillCode = billCode;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = customerName;
            return PartialView("IndexPartialView", model);
        }
    }
}
