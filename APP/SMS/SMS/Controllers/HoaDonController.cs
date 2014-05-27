﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.App_Start;
using SMS.Models;
using PagedList;

namespace SMS.Controllers
{
    [CustomActionFilter]
    public class HoaDonController : Controller
    {
        //
        // GET: /HoaDon/

        [HttpGet]

        public ActionResult Collection()
        {
            return View();
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
            /*model.HoaDonList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.customerName = customerName;
            ViewBag.salerName = salerName;
            ViewBag.accountantName = accountantName;*/
            return PartialView("CollectionPartialView", model);
        }


        public ActionResult Index(DateTime? fromdate, DateTime? todate, 
            int? customerId, string customerName, int? salerId, string salerName,
            int? accountantId, string accountantName, int? status, int? page)
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
            var list = ctx.SP_GET_HOA_DON_BH(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status)).OrderByDescending(uh => uh.NGAY_BAN).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_BH_Result>();
            var AllValue = ctx.SP_GET_VALUE_ALL_HOA_DON(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status)).FirstOrDefault();
            HoaDonBHModel model = new HoaDonBHModel();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            model.HoaDonList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            model.AllValue = AllValue;
            ViewBag.InputKind = Convert.ToInt32(status);
            ViewBag.customerName = customerName;
            ViewBag.salerName = salerName;
            ViewBag.accountantName = accountantName;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? salerId, string salerName,
            int? accountantId, string accountantName, int? status, int? page, bool? flg)
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
            var list = ctx.SP_GET_HOA_DON_BH(fromdate, todate, Convert.ToInt32(customerId), customerName,
                Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status)).OrderByDescending(uh => uh.NGAY_BAN).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_BH_Result>();
            var AllValue = ctx.SP_GET_VALUE_ALL_HOA_DON(fromdate, todate, Convert.ToInt32(customerId), customerName,
               Convert.ToInt32(salerId), salerName, Convert.ToInt32(accountantId), accountantName, Convert.ToInt32(status)).FirstOrDefault();
            HoaDonBHModel model = new HoaDonBHModel();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            model.HoaDonList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            model.AllValue = AllValue;
            ViewBag.InputKind = Convert.ToInt32(status);
            return View(model);
        }

        [HttpPost]
        public ActionResult Payment(InvoicesModel model)
        {
            var ctx = new SmsContext();
            var invoice = ctx.HOA_DON.Find(model.Infor.MA_HOA_DON);
            if (invoice != null && invoice.ACTIVE == "A")
            {
                invoice.SO_TIEN_KHACH_TRA = model.Infor.SO_TIEN_KHACH_TRA;
                if (model.Infor.SO_TIEN_KHACH_TRA <= (model.Infor.TONG_TIEN - model.Infor.CHIEC_KHAU))
                {
                    model.Infor.SO_TIEN_NO_GOI_DAU = (model.Infor.TONG_TIEN - model.Infor.CHIEC_KHAU) - model.Infor.SO_TIEN_KHACH_TRA;
                }
                else
                {
                    model.Infor.SO_TIEN_KHACH_TRA = (model.Infor.TONG_TIEN - model.Infor.CHIEC_KHAU);
                    model.Infor.SO_TIEN_NO_GOI_DAU = 0;
                }
                
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
            return View();
        }
        
        public ActionResult ShowDetail(int id)
        {
            var ctx = new SmsContext();
            var invoiceInfor = ctx.SP_GET_HOA_DON_INFO(id).FirstOrDefault();
            List<V_HOA_DON> detailList = ctx.V_HOA_DON.Where(dh => dh.MA_HOA_DON == id).ToList();
            InvoicesModel model = new InvoicesModel();
            model.Infor = invoiceInfor;
            model.detailList = detailList;
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
