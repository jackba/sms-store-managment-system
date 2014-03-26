using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using SMS.Models;

namespace SMS.Controllers
{
    public class DonViController : Controller
    {
        //
        // GET: /DonVi/

        public ActionResult Index()
        {
            var ctx = new SmsContext();
            var theListContext = (from s in ctx.DON_VI_TINH
                where (s.ACTIVE == "A")
                select s).ToList<DON_VI_TINH>();
            DonViModels donViTinh;
            List<DonViModels> theList = new List<DonViModels>();
            foreach (DON_VI_TINH donVi in theListContext)
            {
                donViTinh = new DonViModels();
                donViTinh.MadonVi = donVi.MA_DON_VI;
                donViTinh.TenDonVi = donVi.TEN_DON_VI;
                donViTinh.GhiChu = donVi.GHI_CHU;
                theList.Add(donViTinh);
            }
            ViewBag.theList = theList;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string searchKeyWord)
        {
            var ctx = new SmsContext();
            var theList = (from s in ctx.DON_VI_TINH
                where (s.TEN_DON_VI == searchKeyWord || s.GHI_CHU == searchKeyWord) && s.ACTIVE == "A"
                select s).ToList<DON_VI_TINH>();
            return View(theList);
        }


        [HttpGet]
        public ActionResult AddNew()
        {
            
            return View();
        }


        [HttpPost]
        public ActionResult AddNew(SMS.Models.DonViModels donVi)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var donVitinh = db.DON_VI_TINH.Create();
                donVitinh.TEN_DON_VI = donVi.TenDonVi;
                donVitinh.GHI_CHU = donVi.GhiChu;
                donVitinh.ACTIVE = "A";
                donVitinh.UPDATE_AT = DateTime.Now;
                donVitinh.CREATE_AT = DateTime.Now;
                donVitinh.UPDATE_BY =  (int)Session["UserId"];
                donVitinh.CREATE_BY = (int)Session["UserId"];
                db.DON_VI_TINH.Add(donVitinh);
                db.SaveChanges();
                return Redirect("Index");
            }
            return View();
        }
    }
}
