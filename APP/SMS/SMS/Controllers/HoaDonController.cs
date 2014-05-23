using System;
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
            return View(model); 
        }
    }
}
