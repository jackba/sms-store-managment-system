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

        public ActionResult ReturnToProvider()
        {
            Return2Provider model = new Return2Provider();
            var ctx = new SmsContext();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var providers = ctx.NHA_CUNG_CAP.Where(u => u.ACTIVE == "A").ToList<NHA_CUNG_CAP>();
            model.Units = units;
            model.Stores = stores;
            model.Providers = providers;
            return View(model);
        }

        public ActionResult DeleteReturn2Provider(int id)
        {
            if (id < 0)
            {
                return RedirectToAction("ListOfToProvider", new { @message = "Không tồn tại phiếu trả hàng này." });
            }
            var ctx = new SmsContext();
            var infor = ctx.TRA_HANG_NCC.Find(id);
            if (infor == null || infor.ACTIVE != "A")
            {
                return RedirectToAction("ListOfToProvider", new { @message = "Không tồn tại phiếu trả hàng này." });
            }
            var export = ctx.XUAT_KHO.Where(u => u.MA_PHIEU_TRA_NCC == id && u.ACTIVE == "A").FirstOrDefault();
            if (export != null)
            {
                return RedirectToAction("ListOfToProvider", new { @message = "Phiếu trả này đã được xuất kho, bạn không thể hủy phiếu trả này." });
            }
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    infor.ACTIVE = "I";
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();
                    var details = ctx.TRA_HANG_NCC_CHI_TIET.Where(u => u.ACTIVE == "A" && u.MA_PHIEU_TRA_NCC == id).ToList<TRA_HANG_NCC_CHI_TIET>();
                    foreach (var detail in details)
                    {
                        detail.ACTIVE = "I";
                        detail.UPDATE_AT = DateTime.Now;
                        detail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                        ctx.SaveChanges();
                    }
                    transaction.Complete();
                    return RedirectToAction("ListOfToProvider", new { @inforMessage = "Hủy phiếu trả hàng nhà cung cấp thành công." });
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    Transaction.Current.Rollback();
                    return RedirectToAction("ListOfToProvider", new { @message = "Hủy phiếu trả thất bại, vui lòng liên hệ admin." });
                }
            }           
        }

        [HttpPost]
        public ActionResult ReturnToProvider(Return2Provider model)
        {
            var ctx = new SmsContext();
            var infor = model.Infor;
            var details = model.Details;

            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var returnInfor = ctx.TRA_HANG_NCC.Create();
                    returnInfor.ACTIVE = "A";
                    returnInfor.MA_NHA_CUNG_CAP = infor.MA_NHA_CUNG_CAP;
                    returnInfor.NGAY_LAP_PHIEU = infor.NGAY_LAP_PHIEU;
                    returnInfor.NGUOI_LAP_PHIEU = Convert.ToInt32(Session["UserId"]);
                    returnInfor.GHI_CHU = infor.GHI_CHU;
                    returnInfor.CREATE_AT = DateTime.Now;
                    returnInfor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    returnInfor.UPDATE_AT = DateTime.Now;
                    returnInfor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);

                    ctx.TRA_HANG_NCC.Add(returnInfor);
                    ctx.SaveChanges();

                    TRA_HANG_NCC_CHI_TIET ct;

                    foreach (var detail in details)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            ct = ctx.TRA_HANG_NCC_CHI_TIET.Create();
                            ct.ACTIVE = "A";
                            //ct = ctx.CHI_TIET_XUAT_KHO.Create();
                            //if (infor.SAVE_FLG == 1)
                            //{
                            //    ct.ACTIVE = "W";
                            //}
                            //else
                            //{
                            //    ct.ACTIVE = "A";
                            //}
                            ct.CREATE_AT = DateTime.Now;
                            ct.UPDATE_AT = DateTime.Now;
                            ct.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.MA_PHIEU_TRA_NCC = returnInfor.ID;
                            ct.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            ct.MA_DON_VI = detail.MA_DON_VI;
                            ct.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            ct.SO_LUONG = detail.SO_LUONG_TEMP * detail.HE_SO;
                            ct.MA_KHO_XUAT = detail.MA_KHO_XUAT;
                            ct.DON_GIA_TEMP = detail.DON_GIA_TEMP;
                            ct.DON_GIA = ct.DON_GIA_TEMP / detail.HE_SO;
                            ctx.TRA_HANG_NCC_CHI_TIET.Add(ct);
                            ctx.SaveChanges();
                        }

                    }
                    transaction.Complete();
                    return RedirectToAction("ListOfToProvider", new { @inforMessage = "Lưu thành công" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Transaction.Current.Rollback();
                    return RedirectToAction("ListOfToProvider", new { @message = "Lưu thất bại, vui lòng liên hệ admin." });
                }
            }
           
        }

        public ActionResult EditReturnToProvider(int id)
        {
            EditReturn2Provider model = new EditReturn2Provider();
            var ctx = new SmsContext();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var providers = ctx.NHA_CUNG_CAP.Where(u => u.ACTIVE == "A").ToList<NHA_CUNG_CAP>();
            var details = ctx.SP_GET_RE_DETAIL_BY_ID(id).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_RE_DETAIL_BY_ID_Result>();
            var infor = ctx.TRA_HANG_NCC.Include("NHA_CUNG_CAP").Where(u => u.ID == id && u.ACTIVE == "A").FirstOrDefault();
            if (infor == null)
            {
                return RedirectToAction("ListOfToProvider", new { @message = "Không tìm thấy phiếu trả hàng này." });
            }
            model.Units = units;
            model.Stores = stores;
            model.Providers = providers;
            model.Infor = infor;
            model.Details = details;
            model.Count = details.Count();
            return View(model);
        }

        [HttpPost]
        public ActionResult EditReturnToProvider(EditReturn2Provider model)
        {
            var ctx = new SmsContext();
            var infor = model.Infor;
            var details = model.Details;
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var returnInfor = ctx.TRA_HANG_NCC.Where(u => u.ID == model.Infor.ID && u.ACTIVE == "A").FirstOrDefault();
                    if (returnInfor == null)
                    {
                        return RedirectToAction("ListOfToProvider", new { @message = "Phiếu nhập hàng không tồn tại, hoặc đã bị xóa. Vui lòng kiểm tra lại" });
                    }
                    //returnInfor.ACTIVE = "A";
                    returnInfor.MA_NHA_CUNG_CAP = infor.MA_NHA_CUNG_CAP;
                    returnInfor.NGAY_LAP_PHIEU = infor.NGAY_LAP_PHIEU;
                    returnInfor.NGUOI_LAP_PHIEU = Convert.ToInt32(Session["UserId"]);
                    returnInfor.GHI_CHU = infor.GHI_CHU;
                    returnInfor.CREATE_AT = DateTime.Now;
                    returnInfor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    returnInfor.UPDATE_AT = DateTime.Now;
                    returnInfor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();

                    ctx.TRA_HANG_NCC_CHI_TIET.RemoveRange(ctx.TRA_HANG_NCC_CHI_TIET.Where(u => u.MA_PHIEU_TRA_NCC == model.Infor.ID));
                    TRA_HANG_NCC_CHI_TIET ct;

                    foreach (var detail in details)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            ct = ctx.TRA_HANG_NCC_CHI_TIET.Create();
                            ct.ACTIVE = "A";
                            //ct = ctx.CHI_TIET_XUAT_KHO.Create();
                            //if (infor.SAVE_FLG == 1)
                            //{
                            //    ct.ACTIVE = "W";
                            //}
                            //else
                            //{
                            //    ct.ACTIVE = "A";
                            //}
                            ct.CREATE_AT = DateTime.Now;
                            ct.UPDATE_AT = DateTime.Now;
                            ct.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.MA_PHIEU_TRA_NCC = returnInfor.ID;
                            ct.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            ct.MA_DON_VI = detail.MA_DON_VI;
                            ct.SO_LUONG_TEMP = detail.SO_LUONG;
                            ct.SO_LUONG = detail.SO_LUONG * detail.HE_SO;
                            ct.MA_KHO_XUAT = detail.MA_KHO_XUAT;
                            ct.DON_GIA_TEMP = detail.DON_GIA;
                            ct.DON_GIA = ct.DON_GIA_TEMP / detail.HE_SO;
                            ctx.TRA_HANG_NCC_CHI_TIET.Add(ct);
                            ctx.SaveChanges();
                        }

                    }
                    transaction.Complete();
                    return RedirectToAction("ListOfToProvider", new { @inforMessage = "Lưu thành công" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Transaction.Current.Rollback();
                    return RedirectToAction("ListOfToProvider", new { @message = "Lưu thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        public ActionResult ReturnNoBill()
        {
            var ctx = new SmsContext();
            ReturnNoBillModel model = new ReturnNoBillModel();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            model.Units = units;
            return View(model);
        }

        [HttpPost]
        public ActionResult ReturnNoBill(ReturnNoBillModel model)
        {
            var ctx = new SmsContext();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var returnInfor = ctx.TRA_HANG.Create();
                    returnInfor.GHI_CHU = model.ReturnInfor.GHI_CHU;
                    returnInfor.NGAY_TRA = model.ReturnInfor.NGAY_TRA;
                    returnInfor.TEN_KHACH_HANG = model.ReturnInfor.TEN_KHACH_HANG;
                    returnInfor.CREATE_AT = DateTime.Now;
                    returnInfor.UPDATE_AT = DateTime.Now;
                    returnInfor.NHAN_VIEN_NHAN = Convert.ToInt32(Session["UserId"]);
                    returnInfor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    returnInfor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    returnInfor.ACTIVE = "A";
                    ctx.TRA_HANG.Add(returnInfor);
                    ctx.SaveChanges();

                    foreach (var detail in model.ReturnDetail)
                    {
                        CHI_TIET_TRA_HANG ct;
                        if (detail.DEL_FLG != 1)
                        {
                            ct = ctx.CHI_TIET_TRA_HANG.Create();
                            ct.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            ct.MA_DON_VI = detail.MA_DON_VI;
                            ct.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            ct.SO_LUONG_TRA = detail.SO_LUONG_TEMP * detail.HE_SO;
                            ct.DON_GIA_TEMP = detail.GIA_VON;
                            ct.GIA_VON = detail.GIA_VON / detail.HE_SO;
                            ct.ACTIVE = "A";
                            ct.CREATE_AT = DateTime.Now;
                            ct.UPDATE_AT = DateTime.Now;
                            ct.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.MA_TRA_HANG = returnInfor.MA_TRA_HANG;
                            ctx.CHI_TIET_TRA_HANG.Add(ct);
                            ctx.SaveChanges();
                        }
                    }
                    transaction.Complete();
                    return RedirectToAction("ReturnPurchaseList", new { @inforMessage = "Nhận trả hàng thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("ReturnPurchaseList", new { @message = "Nhận trả hàng thất bại, vui lòng liên hệ admin." });
                }
            }
        }

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

        public ViewResult ShowReturnBill(int id)
        {
            var ctx = new SmsContext();
            ReturnBillModel model = new ReturnBillModel();
            var infor = ctx.SP_GET_RETURN_INFO_BY_ID(id).FirstOrDefault();
            var detail = ctx.SP_GET_RETURN_DETAIL_BY_ID(id).ToList<SP_GET_RETURN_DETAIL_BY_ID_Result>();
            model.Infor = infor;
            model.Detail = detail;
            return View(model);
        }

        public ViewResult EditGetReturn(int id)
        {
            var ctx = new SmsContext();
            ReturnBillModel model = new ReturnBillModel();
            var infor = ctx.SP_GET_RETURN_INFO_BY_ID(id).FirstOrDefault();
            var detail = ctx.SP_GET_RETURN_DETAIL_BY_ID(id).ToList<SP_GET_RETURN_DETAIL_BY_ID_Result>();
            model.Infor = infor;
            model.Detail = detail;
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            model.Units = units;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditGetReturn(ReturnBillModel model)
        {
            var ctx = new SmsContext();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var returnInfor = ctx.TRA_HANG.Find(model.Infor.MA_TRA_HANG);
                    returnInfor.GHI_CHU = model.Infor.GHI_CHU;
                    returnInfor.NGAY_TRA = model.Infor.NGAY_TRA;
                    returnInfor.TEN_KHACH_HANG = model.Infor.TEN_KHACH_HANG;
                    returnInfor.UPDATE_AT = DateTime.Now;
                    returnInfor.NHAN_VIEN_NHAN = Convert.ToInt32(Session["UserId"]);
                    returnInfor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    returnInfor.ACTIVE = "A";
                    ctx.SaveChanges();
                    ctx.CHI_TIET_TRA_HANG.RemoveRange(ctx.CHI_TIET_TRA_HANG.Where(u => u.MA_TRA_HANG == returnInfor.MA_TRA_HANG));
                    foreach (var detail in model.Detail)
                    {
                        CHI_TIET_TRA_HANG ct;
                        if (detail.DEL_FLG != 1)
                        {
                            ct = ctx.CHI_TIET_TRA_HANG.Create();
                            ct.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            ct.MA_DON_VI = detail.MA_DON_VI;
                            ct.SO_LUONG_TEMP = detail.SO_LUONG;
                            ct.SO_LUONG_TRA = detail.SO_LUONG * detail.HE_SO;
                            ct.GIA_VON = detail.DON_GIA / detail.HE_SO;
                            ct.DON_GIA_TEMP = detail.DON_GIA;
                            ct.ACTIVE = "A";
                            ct.CREATE_AT = DateTime.Now;
                            ct.UPDATE_AT = DateTime.Now;
                            ct.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.MA_TRA_HANG = returnInfor.MA_TRA_HANG;
                            ctx.CHI_TIET_TRA_HANG.Add(ct);
                            ctx.SaveChanges();
                        }
                    }
                    transaction.Complete();
                    return RedirectToAction("ReturnPurchaseList", new { @inforMessage = "Nhận trả hàng thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("ReturnPurchaseList", new { @message = "Nhận trả hàng thất bại, vui lòng liên hệ admin." });
                }
            }
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
                    return RedirectToAction("ReturnPurchaseList", new { @inforMessage = "Nhận trả hàng thành công." });
                }
                catch (Exception)
                {                    
                    Transaction.Current.Rollback();
                    return RedirectToAction("ReturnPurchaseList", new { @message = "Nhận trả hàng thất bại, vui lòng liên hệ admin." });
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
