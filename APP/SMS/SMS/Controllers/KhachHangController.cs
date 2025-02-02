﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using SMS.App_Start;

namespace SMS.Controllers
{

        


    [Authorize]
    [HandleError]    
    public class KhachHangController : Controller
    {
        public FileContentResult downloadWarningCSV(string SearchString)
        {
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"Tên khách hàng\",");
            fileStringBuilder.Append("\"Địa chỉ\",");
            fileStringBuilder.Append("\"Doanh số\",");
            fileStringBuilder.Append("\"Công nợ\",");
            fileStringBuilder.Append("\"Ngày phát sinh nợ\"");
            var ctx = new SmsContext();
            var khList = ctx.Database.SqlQuery<KHACH_HANG_RESULT>(" exec GET_KHACH_HANG_ALERT @NAME ", new SqlParameter("NAME", string.IsNullOrEmpty(SearchString) ? "" : SearchString.Trim())).ToList<KHACH_HANG_RESULT>();
             int i = 0;
             foreach (var product in khList)
             {
                 fileStringBuilder.Append("\n");
                 i += 1;
                 fileStringBuilder.Append("\"" + i + "\",");
                 fileStringBuilder.Append("\"" + product.TEN_KHACH_HANG+ "\",");
                 fileStringBuilder.Append("\"" + product.DIA_CHI + "\",");
                 fileStringBuilder.Append("\"" + (product.DOANH_SO == null ? "" : ((Double)product.DOANH_SO).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                 fileStringBuilder.Append("\"" + (product.NO_GOI_DAU == null ? "" : ((Double)product.NO_GOI_DAU).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                 fileStringBuilder.Append("\"" + (product.NGAY_PHAT_SINH_NO == null ? "" : (DateTime.Parse(product.NGAY_PHAT_SINH_NO.ToString())).ToString("dd/MM/yyyy")).Replace("\"", "\"\"") + "\"");
             }
            ctx.Dispose();
            return File(new System.Text.UTF8Encoding().GetBytes(fileStringBuilder.ToString()), "text/csv", fileName + ".csv");

        }

        [HttpPost]
        public FileContentResult downloadCSV(
            string searchString, string customerName, string customerKind, string customerArea,
            string customerAmountFrom, string customerAmountTo, string customerDebitFrom,
            string customerDebitTo,string ShowFlag)
        {
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"Tên khách hàng\",");
            fileStringBuilder.Append("\"Doanh số\",");
            fileStringBuilder.Append("\"Công nợ\",");
            fileStringBuilder.Append("\"Ngày phát sinh nợ\",");
            fileStringBuilder.Append("\"Mã thẻ khách hàng\",");
            fileStringBuilder.Append("\"Loại khách hàng\",");
            fileStringBuilder.Append("\"Số điện thoại\",");
            fileStringBuilder.Append("\"Email\",");
            fileStringBuilder.Append("\"Địa chỉ\",");
            fileStringBuilder.Append("\"Khu vực\"");
            int kind = string.IsNullOrEmpty(customerKind) ? 0 : int.Parse(customerKind);
            int areaId = string.IsNullOrEmpty(customerArea) ? 0 : int.Parse(customerArea);
            decimal amountFrom = 0;
            decimal.TryParse(string.IsNullOrEmpty(customerAmountFrom) ? "0" : customerAmountFrom.Replace("'", ""), out amountFrom);
            decimal amountTo = 0;
            decimal.TryParse(string.IsNullOrEmpty(customerAmountTo) ? "0" : customerAmountTo.Replace("'", ""), out amountTo);
            decimal debitFrom = 0;
            decimal.TryParse(string.IsNullOrEmpty(customerDebitFrom) ? "0" : customerDebitFrom.Replace(",", ""), out debitFrom);
            decimal debitTo = 0;
            decimal.TryParse(string.IsNullOrEmpty(customerDebitFrom) ? "0" : customerDebitFrom.Replace(",", ""), out debitTo);
            var ctx = new SmsContext();

            var theListContext = (from KhachHang in ctx.KHACH_HANG
                                  join KhuVuc in ctx.KHU_VUC
                                  on KhachHang.MA_KHU_VUC equals KhuVuc.MA_KHU_VUC into kv
                                  from kVuc in kv.DefaultIfEmpty()
                                  join u in ctx.NGUOI_DUNG on KhachHang.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on KhachHang.UPDATE_BY equals u1.MA_NGUOI_DUNG
                                  where
                                  (KhachHang.ACTIVE == "A"
                                  && (String.IsNullOrEmpty(searchString)
                                  || KhachHang.TEN_KHACH_HANG.ToUpper().Contains(searchString.ToUpper())
                                  || KhachHang.MA_THE_KHACH_HANG.ToUpper().Contains(searchString.ToUpper())
                                  || KhachHang.SO_DIEN_THOAI.ToUpper().Contains(searchString.ToUpper())
                                  || kVuc.TEN_KHU_VUC.ToUpper().Contains(searchString.ToUpper()))
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerName) || KhachHang.TEN_KHACH_HANG.ToUpper().Contains(customerName.ToUpper()))
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerKind) || "0".Equals(customerKind) || KhachHang.KIND == kind)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerArea) || "0".Equals(customerArea) || KhachHang.MA_KHU_VUC == areaId)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerAmountFrom) || KhachHang.DOANH_SO >= amountFrom)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerAmountTo) || KhachHang.DOANH_SO <= amountTo)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerAmountTo) || KhachHang.DOANH_SO <= amountTo)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerDebitFrom) || KhachHang.NO_GOI_DAU >= debitFrom)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerDebitTo) || KhachHang.NO_GOI_DAU <= debitTo)
                                  )
                                  select new KhachHangModel
                                  {
                                      KhachHang = KhachHang,
                                      KhuVuc = kVuc,
                                      NguoiCapNhat = u1,
                                      NguoiTao = u
                                  });

            string ckind = "";
            int i = 0;
            foreach (var product in theListContext)
            {
                fileStringBuilder.Append("\n");
                i += 1;
                fileStringBuilder.Append("\"" + i + "\",");
                fileStringBuilder.Append("\"" + product.KhachHang.TEN_KHACH_HANG + "\",");
                fileStringBuilder.Append("\"" + (product.KhachHang.DOANH_SO == null ? "" : ((Double)product.KhachHang.DOANH_SO).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (product.KhachHang.NO_GOI_DAU == null ? "" : ((Double)product.KhachHang.NO_GOI_DAU).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (product.KhachHang.NGAY_PHAT_SINH_NO == null ? "" : ((DateTime)product.KhachHang.NGAY_PHAT_SINH_NO).ToString("dd/MM/yyyy")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + product.KhachHang.MA_THE_KHACH_HANG + "\",");
                if (product.KhachHang.KIND == 1)
                {
                    ckind = "Công ty, công trình";
                }
                else if (product.KhachHang.KIND == 2)
                {
                    ckind = "Thân thiết";
                }
                else
                {
                    ckind = "Thường";
                }
                fileStringBuilder.Append("\"" + ckind + "\",");
                fileStringBuilder.Append("\"" + product.KhachHang.SO_DIEN_THOAI + "\",");
                fileStringBuilder.Append("\"" + product.KhachHang.EMAIL + "\",");
                fileStringBuilder.Append("\"" + product.KhachHang.DIA_CHI + "\",");
                fileStringBuilder.Append("\"" + (product.KhuVuc == null ? "" : product.KhuVuc.TEN_KHU_VUC).Replace("\"", "\"\"") + "\"");
            }
            ctx.Dispose();
            return File(new System.Text.UTF8Encoding().GetBytes(fileStringBuilder.ToString()), "text/csv", fileName + ".csv");
        }

        
        [HttpPost]
        public PartialViewResult IndexPartialView(string searchString, string customerName, string customerKind, string customerArea,
            string customerAmountFrom, string customerAmountTo, string customerDebitFrom,
            string customerDebitTo, string ShowFlag, string sortOrder, string currentFilter, int? currentPageIndex)
        {
            ViewBag.CusomerKind = string.IsNullOrEmpty(customerKind) ? 0 : int.Parse(customerKind);
            ViewBag.CustomerArea = string.IsNullOrEmpty(customerArea) ? 0 : int.Parse(customerArea);
            ViewBag.ShowFlag = string.IsNullOrEmpty(ShowFlag) ? 0 : int.Parse(ShowFlag);
            ViewBag.customerAmountFrom = customerAmountFrom;
            ViewBag.customerAmountTo = customerAmountTo;
            ViewBag.customerDebitFrom = customerDebitFrom;
            ViewBag.customerDebitTo = customerDebitTo;
            ViewBag.customerName = customerName;

            int kind = string.IsNullOrEmpty(customerKind) ? 0 : int.Parse(customerKind);
            int areaId = string.IsNullOrEmpty(customerArea) ? 0 : int.Parse(customerArea);
            decimal amountFrom = 0;
            decimal.TryParse(string.IsNullOrEmpty(customerAmountFrom) ? "0" : customerAmountFrom.Replace("'", ""), out amountFrom);
            decimal amountTo = 0;
            decimal.TryParse(string.IsNullOrEmpty(customerAmountTo) ? "0" : customerAmountTo.Replace("'", ""), out amountTo);
            decimal debitFrom = 0;
            decimal.TryParse(string.IsNullOrEmpty(customerDebitFrom) ? "0" : customerDebitFrom.Replace(",", ""), out debitFrom);
            decimal debitTo = 0;
            decimal.TryParse(string.IsNullOrEmpty(customerDebitFrom) ? "0" : customerDebitFrom.Replace(",", ""), out debitTo);
            var ctx = new SmsContext();
            if (!String.IsNullOrEmpty(searchString) && (currentPageIndex == null || currentPageIndex == 0))
            {
                currentPageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "id";
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.AdminSortParm = sortOrder == "admin_name" ? "admin_name_desc" : "admin_name";
            var theListContext = (from KhachHang in ctx.KHACH_HANG
                                  join KhuVuc in ctx.KHU_VUC
                                  on KhachHang.MA_KHU_VUC equals KhuVuc.MA_KHU_VUC into kv
                                  from kVuc in kv.DefaultIfEmpty()
                                  join u in ctx.NGUOI_DUNG on KhachHang.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on KhachHang.UPDATE_BY equals u1.MA_NGUOI_DUNG
                                  where
                                  (KhachHang.ACTIVE == "A"
                                  && (String.IsNullOrEmpty(searchString)
                                  || KhachHang.TEN_KHACH_HANG.ToUpper().Contains(searchString.ToUpper())
                                  || KhachHang.MA_THE_KHACH_HANG.ToUpper().Contains(searchString.ToUpper())
                                  || KhachHang.SO_DIEN_THOAI.ToUpper().Contains(searchString.ToUpper())
                                  || kVuc.TEN_KHU_VUC.ToUpper().Contains(searchString.ToUpper()))
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerName) || KhachHang.TEN_KHACH_HANG.ToUpper().Contains(customerName.ToUpper()))
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerKind) || "0".Equals(customerKind) || KhachHang.KIND == kind)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerArea) || "0".Equals(customerArea) || KhachHang.MA_KHU_VUC == areaId)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerAmountFrom) || KhachHang.DOANH_SO >= amountFrom)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerAmountTo) || KhachHang.DOANH_SO <= amountTo)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerAmountTo) || KhachHang.DOANH_SO <= amountTo)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerDebitFrom) || KhachHang.NO_GOI_DAU >= debitFrom)
                                  && (string.IsNullOrEmpty(ShowFlag) || "0".Equals(ShowFlag) || string.IsNullOrEmpty(customerDebitTo) || KhachHang.NO_GOI_DAU <= debitTo)
                                  )
                                  select new KhachHangModel
                                  {
                                      KhachHang = KhachHang,
                                      KhuVuc = kVuc,
                                      NguoiCapNhat = u1,
                                      NguoiTao = u
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;
            IPagedList<KhachHangModel> khachHangs = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
            switch (sortOrder)
            {
                case "id":
                    khachHangs = theListContext.OrderBy(KhachHang => KhachHang.KhachHang.MA_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    khachHangs = theListContext.OrderByDescending(KhachHang => KhachHang.KhachHang.MA_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    khachHangs = theListContext.OrderBy(KhachHang => KhachHang.KhachHang.TEN_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    khachHangs = theListContext.OrderByDescending(KhachHang => KhachHang.KhachHang.TEN_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "admin_name":
                    khachHangs = theListContext.OrderBy(KhachHang => KhachHang.KhuVuc.TEN_KHU_VUC).ToPagedList(pageIndex, pageSize);
                    break;
                case "admin_name_desc":
                    khachHangs = theListContext.OrderByDescending(KhachHang => KhachHang.KhuVuc.TEN_KHU_VUC).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khachHangs = theListContext.OrderBy(KhachHang => KhachHang.KhachHang.MA_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
            }
            var khuVucList = (from s in ctx.KHU_VUC
                              where s.ACTIVE == "A"
                              select s).ToList<KHU_VUC>();
            ViewBag.khuVucList = khuVucList;
            ViewBag.CustomerAutocomplete = khuVucList.ToArray();
            ctx.Dispose();
            return PartialView("IndexPartialView", khachHangs);
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Index()
        {
           
            var ctx = new SmsContext();
            var khuVucList = (from s in ctx.KHU_VUC
                              where s.ACTIVE == "A"
                              select s).ToList<KHU_VUC>();
            ViewBag.khuVucList = khuVucList;
            ViewBag.CustomerAutocomplete = khuVucList.ToArray();
            ViewBag.CustomerArea = 0;
            ViewBag.CusomerKind = 0;
            ViewBag.ShowFlag = 0;
            ctx.Dispose();
            return View();
        }


        [HttpPost] 
        public JsonResult Find(string prefixText) 
        { 
            var ctx = new SmsContext();
            var suggestedUsers = from x in ctx.KHACH_HANG 
                                 where (x.TEN_KHACH_HANG.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                 select new
                                 {
                                     id = x.MA_KHACH_HANG,  
                                     value = x.TEN_KHACH_HANG, 
                                 debit = x.NO_GOI_DAU}; 
            var result = Json(suggestedUsers.Take(10).ToList());
            ctx.Dispose();
            return result; 
        }
        /*Tattt add 2014/05/10 start*/
        [HttpPost]
        public JsonResult FindDetailCustomer(string prefixText)
        {
            var ctx = new SmsContext();

            var suggestedUsers = from x in ctx.KHACH_HANG
                                 where (x.TEN_KHACH_HANG.StartsWith(prefixText)
                                        && x.ACTIVE.Equals("A"))
                                 select new
                                 {
                                     id = x.MA_KHACH_HANG,
                                     name = x.TEN_KHACH_HANG,
                                     label = x.TEN_KHACH_HANG,
                                     cardNo = x.MA_THE_KHACH_HANG,
                                     address = x.DIA_CHI,
                                     fone = x.SO_DIEN_THOAI,
                                     mail = x.EMAIL,
                                     kind = x.KIND,
                                     debit = x.NO_GOI_DAU
                                 };
            var result = Json(suggestedUsers.Take(10).ToList());
            ctx.Dispose();
            return result;
        }
        /*Tattt add 2014/05/10 end*/
        [CustomActionFilter]
        [HttpGet]
        public ActionResult AddNew()
        {
            var ctx = new SmsContext();
            var khuVucList = (from s in ctx.KHU_VUC
                              where s.ACTIVE == "A"
                              select s).ToList<KHU_VUC>();
            ViewBag.khuVucList = khuVucList;
            ctx.Dispose();
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult AddNew(Models.KHACH_HANG khachHang)
        {
            var ctx = new SmsContext();
            if (ModelState.IsValid)
            {
                try
                {
                    var db = new SmsContext();
                    var khuVucNew = db.KHACH_HANG.Create();
                    khuVucNew.TEN_KHACH_HANG = khachHang.TEN_KHACH_HANG;
                    khuVucNew.MA_THE_KHACH_HANG = khachHang.MA_THE_KHACH_HANG;
                    khuVucNew.DIA_CHI = khachHang.DIA_CHI;
                    khuVucNew.SO_DIEN_THOAI = khachHang.SO_DIEN_THOAI;
                    khuVucNew.EMAIL = khachHang.EMAIL;
                    khuVucNew.MA_KHU_VUC = khachHang.MA_KHU_VUC;
                    khuVucNew.DOANH_SO = (khachHang.DOANH_SO == null ? 0 : khachHang.DOANH_SO);
                    khuVucNew.NO_GOI_DAU = (khachHang.NO_GOI_DAU == null ? 0 : khachHang.NO_GOI_DAU);
                    khuVucNew.NGAY_PHAT_SINH_NO = khachHang.NGAY_PHAT_SINH_NO;
                    khuVucNew.KIND = khachHang.KIND;
                    khuVucNew.ACTIVE = "A";
                    khuVucNew.UPDATE_AT = DateTime.Now;
                    khuVucNew.CREATE_AT = DateTime.Now;
                    khuVucNew.UPDATE_BY = (int)Session["UserId"];
                    khuVucNew.CREATE_BY = (int)Session["UserId"];
                    db.KHACH_HANG.Add(khuVucNew);
                    db.SaveChanges();
                    ctx.Dispose();
                    return RedirectToAction("Index").Success("Lưu thành công");
                }
                catch (Exception ex)
                {
                    var khuVucList = (from s in ctx.KHU_VUC
                                      where s.ACTIVE == "A"
                                      select s).ToList<KHU_VUC>();
                    ViewBag.khuVucList = khuVucList;
                    return View(khachHang).Error("Không thể lưu thông tin khách hàng. Vui lòng kiểm tra lại CODE.");
                }
                
            }
            else
            {
                var khuVucList = (from s in ctx.KHU_VUC
                                  where s.ACTIVE == "A"
                                  select s).ToList<KHU_VUC>();
                ViewBag.khuVucList = khuVucList;
                return View();
            }
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult UpdateDebit(int id, int? flg)
        {
            ViewBag.flg = flg;
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"]){
                ViewBag.Message = "Bạn không có quyền thay đổi công nợ.";
                return RedirectToAction("Index");
            }
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy khách hàng tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            KHACH_HANG khuVuc = ctx.KHACH_HANG.Find(id);
            khuVuc.flg = flg;
            if (khuVuc.ACTIVE.Equals("A"))
            {
                var khuVucList = (from s in ctx.KHU_VUC
                                  where s.ACTIVE == "A"
                                  select s).ToList<KHU_VUC>();
                ViewBag.khuVucList = khuVucList;
                ctx.Dispose();
                return View(khuVuc);
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult UpdateDebit(Models.KHACH_HANG khachHang)
        {
           
            //string a = Request.Form["amount"];
            decimal amount = (decimal)khachHang.SoTienKhachTra;
            //Decimal.TryParse(a, out amount);

           // a = Request.Form["returnDate"];
            DateTime returnDate = khachHang.NgayTraNo;

            if (khachHang.NgayTraNo != null)
            {
                //returnDate = DateTime.ParseExact(a, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //ViewBag.returnDate = returnDate.ToString("dd/MM/yyyy");
            }
            else
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin.";                
                ViewBag.returnDate = "";
            }


            decimal oldDebit = khachHang.NO_GOI_DAU;
            decimal newDebit = khachHang.NO_GOI_DAU - amount;
            if (amount == 0)
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin.";
                ViewBag.amount = "";
                ViewBag.newDebit = "";

            }
            else
            {
                ViewBag.amount = amount;
                ViewBag.newDebit = newDebit;
            }

            int flg = Convert.ToInt32(Request.Form["flg"]);

            string note = khachHang.NTextNotes;
            ViewBag.note = note;
            
            if (amount != 0 && returnDate != DateTime.MinValue)
            {                
                var db = new SmsContext();
                var donVitinh = db.KHACH_HANG.Find((int)khachHang.MA_KHACH_HANG);
                donVitinh.NO_GOI_DAU = newDebit;
                if (newDebit <= 0)
                {
                    donVitinh.NGAY_PHAT_SINH_NO = null;
                }
                donVitinh.ACTIVE = "A";
                donVitinh.UPDATE_AT = DateTime.Now;
                donVitinh.UPDATE_BY = (int)Session["UserId"];

                var debitHist = db.KHACH_HANG_DEBIT_HIST.Create();
                debitHist.MA_KHACH_HANG = khachHang.MA_KHACH_HANG;
                debitHist.MA_NHAN_VIEN_TH = (int)Session["UserId"];
                debitHist.NGAY_PHAT_SINH = returnDate;
                debitHist.NO_TRUOC = (double)oldDebit;
                debitHist.NO_SAU = (double)newDebit;
                debitHist.PHAT_SINH = (double)amount;
                debitHist.GHI_CHU = string.IsNullOrWhiteSpace(note)? "":  note.Trim();
                debitHist.ACTIVE = "A";
                debitHist.UPDATE_AT = DateTime.Now;
                debitHist.CREATE_AT = DateTime.Now;
                debitHist.UPDATE_BY = (int)Session["UserId"];
                debitHist.CREATE_BY = (int)Session["UserId"];
                db.KHACH_HANG_DEBIT_HIST.Add(debitHist);
                db.SaveChanges();
                db.Dispose();
                if (flg == 1)
                {
                    return RedirectToAction("Index").Success("Lưu thành công");
                }
                else
                {

                    return RedirectToAction("Warning").Success("Lưu thành công");
                }
            }
            return View(khachHang);
        }

        [HttpPost]
        public PartialViewResult showDebitHistPartialView(int? customerId, DateTime? fromDate, DateTime? toDate, string sortOrder, int? currentPageIndex)
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
            fromDate = fromDate == null ? SystemConstant.MIN_DATE : fromDate;
            toDate = toDate == null ? SystemConstant.MAX_DATE : toDate;

            var debitHist = (from s in ctx.KHACH_HANG_DEBIT_HIST
                             join u1 in ctx.NGUOI_DUNG on s.MA_NHAN_VIEN_TH equals u1.MA_NGUOI_DUNG
                             where (s.ACTIVE.Equals("A")
                             && s.MA_KHACH_HANG == customerId
                             && (s.NGAY_PHAT_SINH >= fromDate)
                             && (s.NGAY_PHAT_SINH <= toDate)
                             )
                             select new KhachHangDebitHistModel
                             {
                                 KhachHangDebitHist = s,
                                 NhanVienThucHien = u1
                             }).Take(SystemConstant.MAX_ROWS);

            if (sortOrder == "date_desc")
                sortOrder = "date";
            else
                sortOrder = "date_desc";
            IPagedList<KhachHangDebitHistModel> khachHangHists = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ViewBag.CurrentPageIndex = pageIndex;
            switch (sortOrder)
            {
                case "date":
                    khachHangHists = debitHist.OrderBy(KhachHangHist => KhachHangHist.KhachHangDebitHist.NGAY_PHAT_SINH).ToPagedList(pageIndex, pageSize);
                    break;
                case "date_desc":
                    khachHangHists = debitHist.OrderByDescending(KhachHangHist => KhachHangHist.KhachHangDebitHist.NGAY_PHAT_SINH).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khachHangHists = debitHist.OrderBy(KhachHangHist => KhachHangHist.KhachHangDebitHist.NGAY_PHAT_SINH).ToPagedList(pageIndex, pageSize);
                    break;

            }
            KhachHangModel KhachHang = new KhachHangModel();
            KhachHang.KhachHangHists = khachHangHists;
            KhachHang.Count = debitHist == null ? 0 : debitHist.Count();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CustomerId = customerId;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");;
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");;
            ctx.Dispose();
            return PartialView("showDebitHistPartialView", KhachHang);
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult showDebitHist(int id, int? flg)
        {
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                ViewBag.Message = "Bạn không có quyền thay đổi công nợ.";
                return RedirectToAction("Index");
            }
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy khách hàng tương ứng.";
                return View("../Home/Error"); ;
            }
            ViewBag.flg = flg;
            var ctx = new SmsContext();
            KHACH_HANG khuVuc = ctx.KHACH_HANG.Find(id);
            if (khuVuc.ACTIVE.Equals("A"))
            {
                KhachHangModel model = new KhachHangModel();
                model.KhachHang = khuVuc;
                ctx.Dispose();
                return View(model);
            }
            else
            {
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                ctx.Dispose();
                return View("../Home/Error"); ;
            }
        }

        [HttpPost]
        public PartialViewResult showOrderHistPartialView(int? customerId, DateTime? fromDate, DateTime? toDate, string sortOrder, int? currentPageIndex)
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
            var orderList = ctx.GET_HOA_DON(fromDate, toDate, customerId).ToList<GET_HOA_DON_Result>();
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "date_desc";
            }
            else
            {
                sortOrder = "date";
            }
            ViewBag.SortOrder = sortOrder;
            IPagedList<GET_HOA_DON_Result> khachHangHists = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ViewBag.Count = orderList.Count();
            switch (sortOrder)
            {
                case "date":
                    khachHangHists = orderList.OrderBy(KhachHangHist => KhachHangHist.NGAY_BAN).ToList().ToPagedList(pageIndex, pageSize);
                    break;
                case "date_desc":
                    khachHangHists = orderList.OrderByDescending(KhachHangHist => KhachHangHist.NGAY_BAN).ToList().ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khachHangHists = orderList.OrderBy(KhachHangHist => KhachHangHist.NGAY_BAN).ToList().ToPagedList(pageIndex, pageSize);
                    break;

            }

            KhachHangModel KhachHang = new KhachHangModel();
            KhachHang.OrderHist = khachHangHists;
            var total = ctx.GET_SUM_HOA_DON_BY_CUS_ID(fromDate, toDate, customerId).ToList().First();
            KhachHang.Total = total;
            ViewBag.CustomerId = customerId;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");
            ViewBag.currentPageIndex = currentPageIndex;
            ctx.Dispose();
            return PartialView("showOrderHistPartialView", KhachHang);
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult showOrderHist(int id)
        {
            int flg = Convert.ToInt32(Request.Form["flg"]);
            ViewBag.flg = flg;
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                ViewBag.Message = "Bạn không có quyền thay đổi công nợ.";
                return RedirectToAction("Index");
            }
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy khách hàng tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            KHACH_HANG khuVuc = ctx.KHACH_HANG.Find(id);
            if (khuVuc.ACTIVE.Equals("A"))
            {
                KhachHangModel KhachHang = new KhachHangModel();
                KhachHang.KhachHang = khuVuc;
                ctx.Dispose();
                return View(KhachHang);
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy khách hàng tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            KHACH_HANG khuVuc = ctx.KHACH_HANG.Find(id);
            if (khuVuc.ACTIVE.Equals("A"))
            {
                var khuVucList = (from s in ctx.KHU_VUC
                                  where s.ACTIVE == "A"
                                  select s).ToList<KHU_VUC>();
                ViewBag.khuVucList = khuVucList;
                ctx.Dispose();
                return View(khuVuc);
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy khu vực tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy khách hàng tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var khachHang = ctx.KHACH_HANG.Find(id);
            if (khachHang.ACTIVE.Equals("A"))
            {
                khachHang.ACTIVE = "I";
                khachHang.UPDATE_AT = DateTime.Now;
                khachHang.CREATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("Index");
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy khách hàng tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult CancelHist(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy chứng từ tương ứng";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.KHACH_HANG_DEBIT_HIST.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {
                donvi.ACTIVE = "I";
                donvi.UPDATE_AT = DateTime.Now;
                donvi.CREATE_BY = (int)Session["UserId"];

                var khachHang = ctx.KHACH_HANG.Find(donvi.MA_KHACH_HANG);
                khachHang.NO_GOI_DAU = khachHang.NO_GOI_DAU + (decimal)donvi.PHAT_SINH;
                donvi.UPDATE_AT = DateTime.Now;
                donvi.CREATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("showDebitHist", new { id = donvi.MA_KHACH_HANG });
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy chứng từ tương ứng";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult Edit(Models.KHACH_HANG khachHang)
        {
            var ctx = new SmsContext();            
            if (ModelState.IsValid)
            {
                try
                {
                    var db = new SmsContext();
                    var khuVucNew = db.KHACH_HANG.Find(khachHang.MA_KHACH_HANG);
                    khuVucNew.TEN_KHACH_HANG = khachHang.TEN_KHACH_HANG;
                    khuVucNew.MA_THE_KHACH_HANG = khachHang.MA_THE_KHACH_HANG;
                    khuVucNew.DIA_CHI = khachHang.DIA_CHI;
                    khuVucNew.SO_DIEN_THOAI = khachHang.SO_DIEN_THOAI;
                    khuVucNew.EMAIL = khachHang.EMAIL;
                    khuVucNew.MA_KHU_VUC = khachHang.MA_KHU_VUC;
                    if ((bool)Session["IsAdmin"] || (bool)Session["IsAccounting"])
                    {
                        khuVucNew.DOANH_SO = (khachHang.DOANH_SO == null ? 0 : khachHang.DOANH_SO);
                        khuVucNew.NO_GOI_DAU = (khachHang.NO_GOI_DAU == null ? 0 : khachHang.NO_GOI_DAU);
                        khuVucNew.NGAY_PHAT_SINH_NO = khachHang.NGAY_PHAT_SINH_NO;
                    }
                    khuVucNew.KIND = khachHang.KIND;
                    khuVucNew.ACTIVE = "A";
                    khuVucNew.UPDATE_AT = DateTime.Now;
                    khuVucNew.UPDATE_BY = (int)Session["UserId"];
                    db.SaveChanges();
                    ctx.Dispose();
                    return RedirectToAction("Index").Success("Lưu thành công");
                }
                catch (Exception ex)
                {
                    var khuVucList = (from s in ctx.KHU_VUC
                                      where s.ACTIVE == "A"
                                      select s).ToList<KHU_VUC>();
                    ViewBag.khuVucList = khuVucList;
                    return View(khachHang).Error("Không thể lưu thông tin khách hàng. Vui lòng kiểm tra lại CODE.");
                }
            }
            else
            {
                var khuVucList = (from s in ctx.KHU_VUC
                                  where s.ACTIVE == "A"
                                  select s).ToList<KHU_VUC>();
                ViewBag.khuVucList = khuVucList;
                ctx.Dispose();
                return View();
            }
        }

        [HttpPost]
        public PartialViewResult WarningPartialView(string SearchString, string sortOrder, 
            string currentFilter, int? currentPageIndex)
        {
            if (!String.IsNullOrEmpty(SearchString) && (currentPageIndex == null || currentPageIndex == 0))
            {
                currentPageIndex = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            var ctx = new SmsContext();
            var khList = ctx.Database.SqlQuery<KHACH_HANG_RESULT>(" exec GET_KHACH_HANG_ALERT @NAME ", new SqlParameter("NAME", string.IsNullOrEmpty(SearchString) ? "" : SearchString.Trim())).ToList<KHACH_HANG_RESULT>();
            ViewBag.DateSortParam = sortOrder == "date_desc" ? "date" : "date_desc";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.DebitSortParm = sortOrder == "debit" ? "debit_desc" : "debit";

            IPagedList<KHACH_HANG_RESULT> khachHangHists = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = khList.Count();
            switch (sortOrder)
            {
                case "date":
                    khachHangHists = khList.OrderBy(KhachHangHist => KhachHangHist.NGAY_PHAT_SINH_NO).ToPagedList(pageIndex, pageSize);
                    break;
                case "date_desc":
                    khachHangHists = khList.OrderByDescending(KhachHangHist => KhachHangHist.NGAY_PHAT_SINH_NO).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    khachHangHists = khList.OrderBy(KhachHangHist => KhachHangHist.TEN_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    khachHangHists = khList.OrderByDescending(KhachHangHist => KhachHangHist.TEN_KHACH_HANG).ToPagedList(pageIndex, pageSize);
                    break;
                case "debit":
                    khachHangHists = khList.OrderBy(KhachHangHist => KhachHangHist.NO_GOI_DAU).ToPagedList(pageIndex, pageSize);
                    break;
                case "debit_desc":
                    khachHangHists = khList.OrderByDescending(KhachHangHist => KhachHangHist.NO_GOI_DAU).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khachHangHists = khList.OrderBy(KhachHangHist => KhachHangHist.NGAY_PHAT_SINH_NO).ToPagedList(pageIndex, pageSize);
                    break;

            }
            ViewBag.debitHist = khachHangHists;
            KhachHangModel khachHangModel = new KhachHangModel();
            khachHangModel.WarningList = khachHangHists;
            ViewBag.CurrentFilter = SearchString;
            ctx.Dispose();
            return PartialView("WarningPartialView", khachHangModel);
        }


        [CustomActionFilter]
        [HttpGet]
        public ActionResult Warning()
        {
            return View();
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Show(int id, int? flg)
        {
            ViewBag.flg = flg;
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy chứng từ tương ứng";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.KHACH_HANG.Include("KHU_VUC").Single(kh => kh.MA_KHACH_HANG == id);
            if (donvi.ACTIVE.Equals("A"))
            {
                ctx.Dispose();
                return View(donvi);
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy chứng từ tương ứng";
                return View("../Home/Error"); ;
            }
        }
    }
}
