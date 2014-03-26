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
            ViewBag.theList = theListContext;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var ctx = new SmsContext();
            var theList = (from s in ctx.DON_VI_TINH
                           where (s.TEN_DON_VI.ToUpper().Contains(searchString.ToUpper()) || s.GHI_CHU.ToUpper().Contains(searchString.ToUpper())) && s.ACTIVE == "A"
                select s).ToList<DON_VI_TINH>();
            return View(theList);
        }


        [HttpGet]
        public ActionResult AddNew()
        {
            
            return View();
        }


        [HttpPost]
        public ActionResult AddNew(SMS.Models.DON_VI_TINH donVi)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var donVitinh = db.DON_VI_TINH.Create();
                donVitinh.TEN_DON_VI = donVi.TEN_DON_VI;
                donVitinh.GHI_CHU = donVi.GHI_CHU;
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
