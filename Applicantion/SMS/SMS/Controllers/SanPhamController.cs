using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SMS.Controllers
{
    public class SanPhamController : Controller
    {
        //
        // GET: /SanPham/
        public ActionResult Index()
        {
            return View();
        }

          [HttpPost]
        public ActionResult Index(string searchString)
        {
            return View();
        }
         [HttpGet]
        public ActionResult AddNew()
        {
            var ctx = new SmsContext();

            BindListDV(ctx);
            BindListNSX(ctx);

            return View();
        }

         private void BindListDV(SmsContext ctx)
         {
             var listDV = new List<DON_VI_TINH>();
             listDV.Add(new DON_VI_TINH { MA_DON_VI = -1, TEN_DON_VI = "Chọn đơn vị tính" });
             var dsDonVi = (from s in ctx.DON_VI_TINH select s).ToList < DON_VI_TINH>();
             if(null != dsDonVi){
                 listDV.AddRange(dsDonVi);
             }
             ViewBag.dsDonVi = listDV;           
         }

         private void BindListNSX(SmsContext ctx)
         {             
             var listNSX = new List<NHA_SAN_XUAT>();
             listNSX.Add(new NHA_SAN_XUAT { MA_NHA_SAN_XUAT = -1, TEN_NHA_SAN_XUAT = "Chọn nhà sản xuất" });
             var dsNSX = from s in ctx.NHA_SAN_XUAT select s;
             if (dsNSX != null && dsNSX.Count() > 0)
             {
                 listNSX.AddRange(dsNSX);
             }
             ViewBag.dsNSX = listNSX;
    
         }

         [HttpGet]
         public ActionResult Edit(int id)
         {
             return View();
         }
         [HttpGet]
         public ActionResult Delete(int id)
         {
             return View();
         }
        [HttpPost]
        public ActionResult AddNew(SAN_PHAM product)
         {

             if (ModelState.IsValid)
             {
                 var db = new SmsContext();
                 var sp = db.SAN_PHAM.Create();
                 // input fields
                 sp.TEN_SAN_PHAM = product.TEN_SAN_PHAM;
                 sp.KICH_THUOC = product.KICH_THUOC;
                 sp.CAN_NANG = product.CAN_NANG;
                 if (-1 == product.MA_DON_VI)
                 {
                     product.MA_DON_VI = null;
                 }
                 if (-1 == product.MA_DON_VI)
                 {
                     product.MA_DON_VI = null;
                 }
                 sp.MA_DON_VI   = product.MA_DON_VI;
                 sp.MA_NHA_SAN_XUAT = product.MA_NHA_SAN_XUAT;
                 sp.DAC_TA = product.DAC_TA;
                 sp.GIA_BAN_1 = product.GIA_BAN_1;
                 sp.GIA_BAN_2 = product.GIA_BAN_2;
                 sp.GIA_BAN_3 = product.GIA_BAN_3;
                 sp.CHIEC_KHAU_1 = product.CHIEC_KHAU_1;
                 sp.CHIEC_KHAU_2 = product.CHIEC_KHAU_2;
                 sp.CHIEC_KHAU_3 = product.CHIEC_KHAU_3;
                 sp.CO_SO_TOI_THIEU = product.CO_SO_TOI_THIEU;
                 sp.CO_SO_TOI_DA = product.CO_SO_TOI_DA;
                 //common fields
                 sp.ACTIVE = "A";
                 sp.UPDATE_AT = DateTime.Now;
                 sp.CREATE_AT = DateTime.Now;
                 sp.UPDATE_BY = (int)Session["UserId"];
                 sp.CREATE_BY = (int)Session["UserId"];

                 db.SAN_PHAM.Add(sp);
                 db.SaveChanges();
                 return Redirect("Index");
             }
             var ctx = new SmsContext();
             BindListDV(ctx);
             BindListNSX(ctx);

             return View();
         }

        private object removeCommaInput(object value)
        {
            if (value != null && value.ToString() != null && value.ToString().Contains(",") == true)
            {
                value = value.ToString().Replace(",", "").Trim();
                return value;
            }
            return null;
        }
    }
     
}
