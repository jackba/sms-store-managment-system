using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.App_Start;
using SMS.Models;
using PagedList;
using System.Data.SqlClient;
using System.Data;

namespace SMS.Controllers
{
    [CustomActionFilter]
    public class HoaDonController : Controller
    {
        //
        // GET: /HoaDon/


        [HttpPost]
        public JsonResult FindExported(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.HOA_DON
                                    where (x.SO_HOA_DON.StartsWith(prefixText) 
                                    && x.ACTIVE.Equals("A") && x.STATUS == 3)
                                    select new
                                    {
                                        id = x.MA_HOA_DON,
                                        value = x.SO_HOA_DON
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }


        [HttpGet]
        public ActionResult Collection(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public PartialViewResult PagingContent(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? salerId, string salerName,
            int? accountantId, string accountantName, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(salerName))
            {
                salerId = 0;
            }
            if (string.IsNullOrEmpty(accountantName))
            {
                accountantId = 0;
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
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                salerId = Convert.ToInt32(Session["UserId"]);
            }
            if (!(bool)Session["IsAdmin"] && (bool)Session["IsAccounting"])
            {
                accountantId = Convert.ToInt32(Session["UserId"]);
            }
            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var list = ctx.SP_GET_HOA_DON_CHUA_TT(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName).OrderByDescending(uh => uh.NGAY_BAN).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CHUA_TT_Result>();
            InvoicesNoReciveModel model = new InvoicesNoReciveModel();
            model.Invoices = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            return PartialView("CollectionPartialView", model);
        }

        [HttpPost]
        public PartialViewResult CollectionPartialView(DateTime? fromdate, DateTime? todate, 
            int? customerId, string customerName, int? salerId, string salerName,
            int? accountantId, string accountantName, int? currentPageIndex)
        {

            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(salerName))
            {
                salerId = 0;
            }
            if (string.IsNullOrEmpty(accountantName))
            {
                accountantId = 0;
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
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                salerId = Convert.ToInt32(Session["UserId"]);
            }
            if (!(bool)Session["IsAdmin"] && (bool)Session["IsAccounting"])
            {
                accountantId = Convert.ToInt32(Session["UserId"]);
            }
            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var list = ctx.SP_GET_HOA_DON_CHUA_TT(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName).OrderByDescending(uh => uh.NGAY_BAN).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CHUA_TT_Result>();
            InvoicesNoReciveModel model = new InvoicesNoReciveModel();
            model.Invoices = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            return PartialView("CollectionPartialView", model);
        }

         [HttpPost]
        public PartialViewResult IndexPagingContent(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? salerId, string salerName,
            int? accountantId, string accountantName, int? status, int? areaId, string areaName, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(areaName))
            {
                areaId = 0;
            }

            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(salerName))
            {
                salerId = 0;
            }
            if (string.IsNullOrEmpty(accountantName))
            {
                accountantId = 0;
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
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                salerId = Convert.ToInt32(Session["UserId"]);
            }
            if (!(bool)Session["IsAdmin"] && (bool)Session["IsAccounting"])
            {
                accountantId = Convert.ToInt32(Session["UserId"]);
            }
            var ctx = new SmsContext();
            var list = ctx.SP_GET_HOA_DON_BH(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status),
                Convert.ToInt32(areaId), areaName).OrderByDescending(uh => uh.NGAY_BAN).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_BH_Result>();
            var AllValue = ctx.SP_GET_VALUE_ALL_HOA_DON(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status),
                Convert.ToInt32(areaId), areaName).FirstOrDefault();
            HoaDonBHModel model = new HoaDonBHModel();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            model.HoaDonList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            model.AllValue = AllValue;
            ViewBag.InputKind = Convert.ToInt32(status);
            ViewBag.customerName = customerName;
            ViewBag.salerName = salerName;
            ViewBag.accountantName = accountantName;
            return PartialView("IndexPartialView", model);
        }

        [HttpPost]
        public PartialViewResult IndexPartialView(DateTime? fromdate, DateTime? todate, 
            int? customerId, string customerName, int? salerId, string salerName,
            int? accountantId, string accountantName, int? status, int? areaId, string areaName, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(areaName))
            {
                areaId = 0;
            }

            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(salerName))
            {
                salerId = 0;
            }
            if (string.IsNullOrEmpty(accountantName))
            {
                accountantId = 0;
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
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                salerId = Convert.ToInt32(Session["UserId"]);
            }
            if (!(bool)Session["IsAdmin"] && (bool)Session["IsAccounting"])
            {
                accountantId = Convert.ToInt32(Session["UserId"]);
            }
            var ctx = new SmsContext();
            var list = ctx.SP_GET_HOA_DON_BH(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status),
                Convert.ToInt32(areaId), areaName).OrderByDescending(uh => uh.NGAY_BAN).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_BH_Result>();
            var AllValue = ctx.SP_GET_VALUE_ALL_HOA_DON(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status),
                Convert.ToInt32(areaId), areaName).FirstOrDefault();
            HoaDonBHModel model = new HoaDonBHModel();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            model.HoaDonList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            model.AllValue = AllValue;
            
            ViewBag.customerName = customerName;
            ViewBag.salerName = salerName;
            ViewBag.accountantName = accountantName;
            return PartialView("IndexPartialView", model);
        }

        public ActionResult Index( string messsage, string inforMessage)
        {
            ViewBag.InputKind = Convert.ToInt32(0);
            ViewBag.Messasge = messsage;
            ViewBag.InforMessage = inforMessage;
            return View();
        }
        [HttpPost]
        public ActionResult PaymentAndExport(InvoicesModel model)
        {
            var ctx = new SmsContext();
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
            if (model.Infor.SO_TIEN_KHACH_TRA >= (model.Infor.TONG_TIEN - model.Infor.CHIEC_KHAU))
            {
                model.Infor.SO_TIEN_KHACH_TRA = model.Infor.TONG_TIEN - model.Infor.CHIEC_KHAU;
            }
            var TotalPay = new SqlParameter
            {
                ParameterName = "SO_TIEN_KHACH_TRA",
                Value = Convert.ToDouble(model.Infor.SO_TIEN_KHACH_TRA)
            };
            var returnValue = new SqlParameter
            {
                ParameterName = "RETURN_VALUE",
                Value = Convert.ToInt32(0),
                Direction = ParameterDirection.Output
            };

            ctx.Database.CommandTimeout = 300;
            var tonkho = ctx.Database.ExecuteSqlCommand("exec SP_THU_TIEN_XUAT_KHO @MA_HOA_DON, @MA_NHAN_VIEN_THUC_HIEN, @SO_TIEN_KHACH_TRA , @RETURN_VALUE OUT",
                InvoiceId,
                UserId,
                TotalPay,
                returnValue
                );
            int returnVal = Convert.ToInt32(returnValue.Value);
            int flg = Convert.ToInt32(Request.Form["flg"]);
            if (returnVal == 0)
            {
                ViewBag.Message = "Không đủ số lượng để xuất kho. Vui lòng kiểm tra lại hóa đơn.";
                ViewBag.Status = 0;
                return RedirectToAction("ShowDetail", new { @id = model.Infor.MA_HOA_DON, @flg = flg, @status = 0 });
            }
            else if (returnVal == -1)
            {
                ViewBag.Message = "Hóa đơn đã được thu tiền. Không thể thu tiền hóa đơn này";
                ViewBag.Status = -1;
                return RedirectToAction("ShowDetail", new { @id = model.Infor.MA_HOA_DON, @flg = flg, @status = -1 });
            }
            var invoiceInfor = ctx.SP_GET_HOA_DON_INFO(model.Infor.MA_HOA_DON).FirstOrDefault();
            List<V_HOA_DON> detailList = ctx.V_HOA_DON.Where(dh => dh.MA_HOA_DON == model.Infor.MA_HOA_DON).ToList();
            model.Infor = invoiceInfor;
            model.detailList = detailList;

            SmsMasterModel master = new SmsMasterModel();
            var companyName = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "COMPANY_NAME").FirstOrDefault();
            var address = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADDRESS").FirstOrDefault();
            var phoneNumber = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "PHONE_NUMBER").FirstOrDefault();
            var faxNumber = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "FAX_NUMBER").FirstOrDefault();
            var advertisementHeader = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADVERTISEMENT_HEADER").FirstOrDefault();
            var advertisementFooter = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADVERTISEMENT_FOOTER").FirstOrDefault();
            master.CompanyName = companyName == null ? "" : companyName.VALUE;
            master.Address = address == null ? "" : address.VALUE;
            master.AdvertisementHeader = advertisementHeader == null ? "" : advertisementHeader.VALUE;
            master.AdvertisementFooter = advertisementFooter == null ? "" : advertisementFooter.VALUE;
            master.PhoneNumber = phoneNumber == null ? "" : phoneNumber.VALUE;
            master.FaxNumber = faxNumber == null ? "" : faxNumber.VALUE;
            model.SmsMaster = master;
            return View(model);
        }
        [HttpPost]
        public ActionResult Payment(InvoicesModel model)
        {
            var ctx = new SmsContext();
            var invoice = ctx.HOA_DON.Find(model.Infor.MA_HOA_DON);
            if (invoice != null && invoice.STATUS >= 2)
            {
                return RedirectToAction("Collection", new {@messagae = "Hóa đơn đã được thu tiền."});
            }
            if (invoice != null && invoice.ACTIVE == "A")
            {
                if (model.Infor.SO_TIEN_KHACH_TRA == null)
                {
                    model.Infor.SO_TIEN_KHACH_TRA = 0;
                }
                
                if (model.Infor.SO_TIEN_KHACH_TRA <= (model.Infor.TONG_TIEN - model.Infor.CHIEC_KHAU))
                {
                    model.Infor.SO_TIEN_NO_GOI_DAU = (model.Infor.TONG_TIEN - model.Infor.CHIEC_KHAU) - model.Infor.SO_TIEN_KHACH_TRA;
                }
                else
                {
                    model.Infor.SO_TIEN_KHACH_TRA = (model.Infor.TONG_TIEN - model.Infor.CHIEC_KHAU);
                    model.Infor.SO_TIEN_NO_GOI_DAU = 0;
                }
                invoice.SO_TIEN_KHACH_TRA = model.Infor.SO_TIEN_KHACH_TRA;
                invoice.SO_TIEN_NO_GOI_DAU = model.Infor.SO_TIEN_NO_GOI_DAU;
                invoice.STATUS = 2;
                invoice.UPDATE_AT = DateTime.Now;
                invoice.UPDATE_BY = (int)Session["UserId"];
                invoice.MA_NHAN_VIEN_THU_TIEN = (int)Session["UserId"];
                if(Convert.ToInt32(model.Infor.MA_KHACH_HANG) >0)
                {
                    var customer = ctx.KHACH_HANG.Find(Convert.ToInt32(model.Infor.MA_KHACH_HANG));
                    if (customer != null && customer.ACTIVE == "A")
                    {
                        customer.DOANH_SO = customer.DOANH_SO + Convert.ToDecimal(model.Infor.TONG_TIEN) - Convert.ToDecimal(model.Infor.CHIEC_KHAU);
                        if (model.Infor.SO_TIEN_NO_GOI_DAU > 0)
                        {
                            if (customer.NGAY_PHAT_SINH_NO == null)
                            {
                                customer.NGAY_PHAT_SINH_NO = DateTime.Now;
                            }

                            var DebitHist = ctx.KHACH_HANG_DEBIT_HIST.Create();
                            DebitHist.NO_TRUOC = Convert.ToDouble(customer.NO_GOI_DAU);
                            DebitHist.NO_SAU = Convert.ToDouble(customer.NO_GOI_DAU) + model.Infor.SO_TIEN_NO_GOI_DAU;
                            DebitHist.NGAY_PHAT_SINH = DateTime.Now;
                            DebitHist.PHAT_SINH = -1*Convert.ToDouble(model.Infor.SO_TIEN_NO_GOI_DAU);
                            DebitHist.MA_HOA_DON = model.Infor.MA_HOA_DON;
                            DebitHist.MA_KHACH_HANG = customer.MA_KHACH_HANG;
                            DebitHist.MA_NHAN_VIEN_TH = (int)Session["UserId"];
                            DebitHist.ACTIVE = "A";
                            DebitHist.UPDATE_AT = DateTime.Now;
                            DebitHist.UPDATE_BY = (int)Session["UserId"];
                            DebitHist.CREATE_AT = DateTime.Now;
                            DebitHist.CREATE_BY = (int)Session["UserId"];
                            ctx.KHACH_HANG_DEBIT_HIST.Add(DebitHist);
                        }
                        customer.NO_GOI_DAU = customer.NO_GOI_DAU + Convert.ToDecimal(model.Infor.SO_TIEN_NO_GOI_DAU);
                        customer.UPDATE_AT = DateTime.Now;
                        customer.UPDATE_BY = (int)Session["UserId"];
                    }else
                    {
                        ViewBag.Message = "Không tìm thấy khách hàng tương ứng.";
                        return View("../Home/Error");
                    }
                }
                ctx.SaveChanges();
            }else
            {
                ViewBag.Message = "Không tìm thấy  hóa đơn tương ứng.";
                return View("../Home/Error");
            }
            var invoiceInfor = ctx.SP_GET_HOA_DON_INFO(model.Infor.MA_HOA_DON).FirstOrDefault();
            List<V_HOA_DON> detailList = ctx.V_HOA_DON.Where(dh => dh.MA_HOA_DON == model.Infor.MA_HOA_DON).ToList();
            model.Infor = invoiceInfor;
            model.detailList = detailList;

            SmsMasterModel master = new SmsMasterModel();
            var companyName = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "COMPANY_NAME").FirstOrDefault();
            var address = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADDRESS").FirstOrDefault();
            var phoneNumber = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "PHONE_NUMBER").FirstOrDefault();
            var faxNumber = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "FAX_NUMBER").FirstOrDefault();
            var advertisementHeader = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADVERTISEMENT_HEADER").FirstOrDefault();
            var advertisementFooter = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADVERTISEMENT_FOOTER").FirstOrDefault();
            master.CompanyName = companyName == null ? "" : companyName.VALUE;
            master.Address = address == null ? "" : address.VALUE;
            master.AdvertisementHeader = advertisementHeader == null ? "" : advertisementHeader.VALUE;
            master.AdvertisementFooter = advertisementFooter == null ? "" : advertisementFooter.VALUE;
            master.PhoneNumber = phoneNumber == null ? "" : phoneNumber.VALUE;
            master.FaxNumber = faxNumber == null ? "" : faxNumber.VALUE;
            model.SmsMaster = master;
            return View(model);
        }
        
        public ActionResult ShowDetail(int id, int? flg, int? status )
        {
            var ctx = new SmsContext();
            InvoicesModel model = new InvoicesModel();
            var invoiceInfor = ctx.SP_GET_HOA_DON_INFO(id).FirstOrDefault();
            if (invoiceInfor != null && invoiceInfor.MA_KHACH_HANG != null)
            {
                var customer = ctx.KHACH_HANG.Find(invoiceInfor.MA_KHACH_HANG);
                if(customer != null)
                {
                    var kind = customer.KIND;
                    model.customerDebit = customer.NO_GOI_DAU;
                    string key = "";
                    if(kind ==1)
                    {
                        key = "MAX_DEBIT_KIND_1";
                    }
                    else if(kind == 2)
                    {
                        key = "MAX_DEBIT_KIND_2";
                    }else
                    {
                        key = "MAX_DEBIT_KIND_3";
                    }
                    var maxdebit = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == key).FirstOrDefault();
                    if (maxdebit != null && maxdebit.VALUE != null)
                    {
                        decimal debit = decimal.Parse(maxdebit.VALUE.Replace(",", ""));
                        model.maxDebit = debit;
                    }
                }
            }
            List<V_HOA_DON> detailList = ctx.V_HOA_DON.Where(dh => dh.MA_HOA_DON == id).ToList();
            
            model.Infor = invoiceInfor;
            model.detailList = detailList;
            ViewBag.Flg = flg;
            if (status == 0)
            {
                ViewBag.Message = "Không đủ số lượng để xuất kho. Vui lòng kiểm tra lại hóa đơn.";
                ViewBag.Status = 0;
            }
            else if (status == -1)
            {
                ViewBag.Message = "Hóa đơn đã được thu tiền. Không thể thu tiền hóa đơn này";
                ViewBag.Status = -1;
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy  hóa đơn tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            int userid = (int)Session["UserId"];
            var invoice = ctx.HOA_DON.Find(id);
            if (invoice.ACTIVE.Equals("A"))
            {
                
                invoice.ACTIVE = "I";
                invoice.UPDATE_AT = DateTime.Now;
                invoice.CREATE_BY = (int)Session["UserId"];

                if(invoice.STATUS  >=2 && (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"]))
                {
                    ViewBag.Message = "Bạn không có quyền xóa hóa đơn này.";
                    return View("../Home/Error"); ;
                }
                if (invoice.STATUS >=2 && invoice.MA_KHACH_HANG != null && !string.IsNullOrEmpty(invoice.MA_KHACH_HANG.ToString()))
                {
                    var customer = ctx.KHACH_HANG.Single(uh => uh.MA_KHACH_HANG == invoice.MA_KHACH_HANG
                        && uh.ACTIVE == "A");
                    if (customer != null)
                    {
                        customer.DOANH_SO = Convert.ToDecimal(customer.DOANH_SO) -
                            Convert.ToDecimal(invoice.SO_TIEN_KHACH_TRA)
                            - Convert.ToDecimal(invoice.SO_TIEN_NO_GOI_DAU);
                        customer.NO_GOI_DAU = Convert.ToDecimal(customer.NO_GOI_DAU) - Convert.ToDecimal(invoice.SO_TIEN_NO_GOI_DAU);
                        if (customer.NO_GOI_DAU <= 0)
                        {
                            customer.NGAY_PHAT_SINH_NO = null;
                        }
                        customer.UPDATE_AT = DateTime.Now;
                        customer.UPDATE_BY = (int)Session["UserId"];
                    }
                    var debitHist = ctx.KHACH_HANG_DEBIT_HIST.OrderByDescending(uh => uh.ID).FirstOrDefault(uh => uh.MA_HOA_DON == invoice.MA_HOA_DON && uh.ACTIVE == "A");
                    if (debitHist != null)
                    {
                        debitHist.ACTIVE = "I";
                        debitHist.UPDATE_AT = DateTime.Now;
                        debitHist.UPDATE_BY = (int)Session["UserId"];
                    }
                    if(invoice.STATUS  > 2 )
                    {
                        var export = ctx.XUAT_KHO.OrderByDescending(uh => uh.MA_XUAT_KHO).FirstOrDefault(uh => uh.MA_HOA_DON == invoice.MA_HOA_DON && uh.ACTIVE == "A");
                        if(export != null)
                        {
                            export.ACTIVE = "I";
                            export.UPDATE_AT = DateTime.Now;
                            export.UPDATE_BY = (int)Session["UserId"];
                            var exportDetails = ctx.CHI_TIET_XUAT_KHO.Where(uh => uh.MA_XUAT_KHO == export.MA_XUAT_KHO && uh.ACTIVE == "A")
                                .ToList<CHI_TIET_XUAT_KHO>();
                            if (exportDetails != null)
                            {
                                foreach (CHI_TIET_XUAT_KHO detail in exportDetails)
                                {
                                    detail.ACTIVE = "I";
                                    detail.UPDATE_AT = DateTime.Now;
                                    detail.UPDATE_BY = (int)Session["UserId"];
                                }
                            }
                        }
                    }
                    
                }
                var details = ctx.CHI_TIET_HOA_DON.Where(uh => uh.MA_HOA_DON == invoice.MA_HOA_DON 
                    && uh.ACTIVE == "A").ToList<CHI_TIET_HOA_DON>();
                if (details != null)
                {
                    foreach (CHI_TIET_HOA_DON detail in details)
                    {
                        detail.ACTIVE = "I";
                        detail.UPDATE_AT = DateTime.Now;
                        detail.UPDATE_BY = (int)Session["UserId"];
                    }
                }
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Không tìm thấy hóa đơn tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [HttpPost]
        public ActionResult ShowDetail(InvoicesModel model)
        {
            var ctx = new SmsContext();
            var invoiceInfor = ctx.HOA_DON.Single(uh => uh.MA_HOA_DON == model.Infor.MA_HOA_DON && uh.ACTIVE == "A");
            if (invoiceInfor == null || invoiceInfor.STATUS != 2)
            {
                ViewBag.Message = "Không thể cấp quyền sửa hóa đơn cho hóa đơn này.";
            }else
            {
                if(invoiceInfor.MA_KHACH_HANG != null && !string.IsNullOrEmpty(invoiceInfor.MA_KHACH_HANG.ToString()))
                {
                    var customer = ctx.KHACH_HANG.Single(uh => uh.MA_KHACH_HANG == invoiceInfor.MA_KHACH_HANG
                        && uh.ACTIVE == "A");
                    if (customer != null)
                    {
                        customer.DOANH_SO = Convert.ToDecimal(customer.DOANH_SO) - 
                            Convert.ToDecimal(invoiceInfor.SO_TIEN_KHACH_TRA)
                            - Convert.ToDecimal(invoiceInfor.SO_TIEN_NO_GOI_DAU);
                        customer.NO_GOI_DAU = Convert.ToDecimal(customer.NO_GOI_DAU) - Convert.ToDecimal(invoiceInfor.SO_TIEN_NO_GOI_DAU);
                        if (customer.NO_GOI_DAU <= 0)
                        {
                            customer.NGAY_PHAT_SINH_NO = null;
                        }
                        customer.UPDATE_AT = DateTime.Now;
                        customer.UPDATE_BY = (int)Session["UserId"];
                    }
                    var debitHist = ctx.KHACH_HANG_DEBIT_HIST.OrderByDescending(uh=> uh.ID).FirstOrDefault(uh => uh.MA_HOA_DON == model.Infor.MA_HOA_DON && uh.ACTIVE == "A");
                    if (debitHist != null)
                    {
                        debitHist.ACTIVE = "I";
                        debitHist.UPDATE_AT = DateTime.Now;
                        debitHist.UPDATE_BY = (int)Session["UserId"];
                    }
                }
                invoiceInfor.STATUS = 1;
                invoiceInfor.SO_TIEN_KHACH_TRA = 0;
                invoiceInfor.SO_TIEN_NO_GOI_DAU = 0;
                invoiceInfor.MA_NHAN_VIEN_THU_TIEN = null;
                invoiceInfor.EDIT_APPROVER = (int)Session["UserId"];
                invoiceInfor.UPDATE_AT = DateTime.Now;
                invoiceInfor.UPDATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
            }
            List<V_HOA_DON> detailList = ctx.V_HOA_DON.Where(dh => dh.MA_HOA_DON == model.Infor.MA_HOA_DON).ToList();
            var infor = ctx.SP_GET_HOA_DON_INFO(model.Infor.MA_HOA_DON).FirstOrDefault();
            model.Infor = infor;
            model.detailList = detailList;
            return RedirectToAction("Index"); 
        }
    }
}
