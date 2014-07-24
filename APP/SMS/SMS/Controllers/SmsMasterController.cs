using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.App_Start;

namespace SMS.Controllers
{
    [CustomActionFilter]
    public class SmsMasterController : Controller
    {
        //
        // GET: /SmsMaster/

        [HttpPost]
        public ActionResult Config(SmsMasterModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var companyName = db.SMS_MASTER.Where(u => u.NAME == "COMPANY_NAME" && u.ACTIVE == "A").FirstOrDefault();
                if (companyName == null)
                {
                    companyName = db.SMS_MASTER.Create();
                    companyName.NAME = "COMPANY_NAME";
                    companyName.VALUE = model.CompanyName;
                    companyName.UPDATE_AT = DateTime.Now;
                    companyName.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    companyName.CREATE_AT = DateTime.Now;
                    companyName.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    companyName.ACTIVE = "A";
                    db.SMS_MASTER.Add(companyName);
                    db.SaveChanges();
                }
                else
                {
                    companyName.VALUE = model.CompanyName;
                    companyName.UPDATE_AT = DateTime.Now;
                    companyName.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    db.SaveChanges();
                }
                

                var address = db.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADDRESS").FirstOrDefault();
                if (address == null)
                {
                    address = db.SMS_MASTER.Create();
                    address.NAME = "ADDRESS";
                    address.VALUE = model.Address;
                    address.UPDATE_AT = DateTime.Now;
                    address.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    address.CREATE_AT = DateTime.Now;
                    address.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    address.ACTIVE = "A";
                    db.SMS_MASTER.Add(address);
                    db.SaveChanges();
                }
                else
                {
                    address.VALUE = model.Address;
                    address.UPDATE_AT = DateTime.Now;
                    address.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    db.SaveChanges();
                }
                

                var phoneNumber = db.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "PHONE_NUMBER").FirstOrDefault();
                if (phoneNumber == null)
                {
                    phoneNumber = db.SMS_MASTER.Create();
                    phoneNumber.NAME = "PHONE_NUMBER";
                    phoneNumber.VALUE = model.PhoneNumber;
                    phoneNumber.UPDATE_AT = DateTime.Now;
                    phoneNumber.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    phoneNumber.CREATE_AT = DateTime.Now;
                    phoneNumber.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    phoneNumber.ACTIVE = "A";
                    db.SMS_MASTER.Add(phoneNumber);
                    db.SaveChanges();
                }
                else
                {
                    phoneNumber.VALUE = model.PhoneNumber;
                    phoneNumber.UPDATE_AT = DateTime.Now;
                    phoneNumber.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    db.SaveChanges();
                }
                

                var faxNumber = db.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "FAX_NUMBER").FirstOrDefault();
                if (faxNumber == null)
                {
                    faxNumber = db.SMS_MASTER.Create();
                    faxNumber.NAME = "FAX_NUMBER";
                    faxNumber.VALUE = model.FaxNumber;
                    faxNumber.UPDATE_AT = DateTime.Now;
                    faxNumber.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    faxNumber.CREATE_AT = DateTime.Now;
                    faxNumber.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    faxNumber.ACTIVE = "A";
                    db.SMS_MASTER.Add(faxNumber);
                    db.SaveChanges();
                }
                else
                {
                    faxNumber.VALUE = model.FaxNumber;
                    faxNumber.UPDATE_AT = DateTime.Now;
                    faxNumber.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    db.SaveChanges();
                }
               

                var advertisementHeader = db.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADVERTISEMENT_HEADER").FirstOrDefault();
                if (advertisementHeader == null)
                {
                    advertisementHeader = db.SMS_MASTER.Create();
                    advertisementHeader.NAME = "ADVERTISEMENT_HEADER";
                    advertisementHeader.VALUE = model.AdvertisementHeader;
                    advertisementHeader.CREATE_AT = DateTime.Now;
                    advertisementHeader.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    advertisementHeader.UPDATE_AT = DateTime.Now;
                    advertisementHeader.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    advertisementHeader.ACTIVE = "A";
                    db.SMS_MASTER.Add(advertisementHeader);
                    db.SaveChanges();
                }
                else
                {
                    advertisementHeader.VALUE = model.AdvertisementHeader;
                    advertisementHeader.UPDATE_AT = DateTime.Now;
                    advertisementHeader.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    db.SaveChanges();
                }
                

                var advertisementFooter = db.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADVERTISEMENT_FOOTER").FirstOrDefault();
                if (advertisementFooter == null)
                {
                    advertisementFooter = db.SMS_MASTER.Create();
                    advertisementFooter.NAME = "ADVERTISEMENT_FOOTER";
                    advertisementFooter.VALUE = model.AdvertisementFooter;
                    advertisementFooter.CREATE_AT = DateTime.Now;
                    advertisementFooter.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    advertisementFooter.UPDATE_AT = DateTime.Now;
                    advertisementFooter.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    advertisementFooter.ACTIVE = "A";
                    db.SMS_MASTER.Add(advertisementFooter);
                    db.SaveChanges();
                }
                else
                {
                    advertisementFooter.VALUE = model.AdvertisementFooter;
                    advertisementFooter.UPDATE_AT = DateTime.Now;
                    advertisementFooter.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    db.SaveChanges();
                }
                
            }
            return View(model);
        }
        public ActionResult Config()
        {
            SmsMasterModel model = new SmsMasterModel();
            var ctx = new SmsContext();
            var companyName = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "COMPANY_NAME").FirstOrDefault();
            var address = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADDRESS").FirstOrDefault();
            var phoneNumber = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "PHONE_NUMBER").FirstOrDefault();
            var faxNumber = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "FAX_NUMBER").FirstOrDefault();
            var advertisementHeader = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADVERTISEMENT_HEADER").FirstOrDefault();
            var advertisementFooter = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "ADVERTISEMENT_FOOTER").FirstOrDefault();
            model.CompanyName = companyName == null ? "" : companyName.VALUE;
            model.Address = address == null ? "" : address.VALUE;
            model.AdvertisementHeader = advertisementHeader == null ? "" : advertisementHeader.VALUE;
            model.AdvertisementFooter = advertisementFooter == null ? "" : advertisementFooter.VALUE ;
            model.PhoneNumber = phoneNumber == null ? "" : phoneNumber.VALUE;
            model.FaxNumber = faxNumber == null ? "" : faxNumber.VALUE;
            return View(model);
        }

        public ActionResult Index()
        {

            var ctx = new SmsContext();
            var theListContext = (from s in ctx.SMS_MASTER
                                      where s.ACTIVE.Equals("A")
                                  select new 
                                  { 
                                      s
                                  }).Take(SystemConstant.MAX_ROWS).ToList();
            foreach (var sms in theListContext)
            {
                switch (sms.s.NAME)
                {
                     case "MAX_DEBIT_KIND_1":
                        ViewBag.MaxDebit1 = string.IsNullOrEmpty(sms.s.VALUE)? "0": decimal.Parse(sms.s.VALUE).ToString("#,##0.00");
                        break;
                     case "MAX_DEBIT_KIND_2":
                        ViewBag.MaxDebit2 = string.IsNullOrEmpty(sms.s.VALUE) ? "0" : decimal.Parse(sms.s.VALUE).ToString("#,##0.00");
                        break;
                     case "MAX_DEBIT_KIND_3":
                        ViewBag.MaxDebit3 = string.IsNullOrEmpty(sms.s.VALUE) ? "0" : decimal.Parse(sms.s.VALUE).ToString("#,##0.00");
                        break;
                     case "MAX_MOUNTH_KIND_1":
                        ViewBag.MaxMonth1 = string.IsNullOrEmpty(sms.s.VALUE) ? "0" : int.Parse(sms.s.VALUE).ToString();
                        break;
                     case "MAX_MOUNTH_KIND_2":
                        ViewBag.MaxMonth2 = string.IsNullOrEmpty(sms.s.VALUE) ? "0" : int.Parse(sms.s.VALUE).ToString();
                        break;
                     case "MAX_MOUNTH_KIND_3":
                        ViewBag.MaxMonth3 = string.IsNullOrEmpty(sms.s.VALUE) ? "0" : int.Parse(sms.s.VALUE).ToString();
                        break;
                }
                   
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(string MaxDebit1, string MaxDebit2, 
            string MaxDebit3, string MaxMonth1, string MaxMonth2, string MaxMonth3)
        {
            if (string.IsNullOrEmpty(MaxDebit1) ||
                string.IsNullOrEmpty(MaxDebit2) ||
                string.IsNullOrEmpty(MaxDebit3) ||
                string.IsNullOrEmpty(MaxMonth1) ||
                string.IsNullOrEmpty(MaxMonth2) ||
                string.IsNullOrEmpty(MaxMonth3))
            {
                ViewBag.Message = "Vui lòng nhập tất cả các thông tin trên màn hình.";
            }
            else
            {
                MaxDebit1 = string.IsNullOrEmpty(MaxDebit1) ? "0" : MaxDebit1.Replace(",", "");
                MaxDebit2 = string.IsNullOrEmpty(MaxDebit2) ? "0" : MaxDebit2.Replace(",", "");
                MaxDebit3 = string.IsNullOrEmpty(MaxDebit3) ? "0" : MaxDebit3.Replace(",", "");
                var ctx = new SmsContext();
                var sms1 = ctx.SMS_MASTER.FirstOrDefault(u => u.NAME.Equals("MAX_DEBIT_KIND_1") && u.ACTIVE.Equals("A"));
                sms1.VALUE = decimal.Parse(MaxDebit1).ToString();

                var sms2 = ctx.SMS_MASTER.FirstOrDefault(u => u.NAME.Equals("MAX_DEBIT_KIND_2") && u.ACTIVE.Equals("A"));
                sms2.VALUE = decimal.Parse(MaxDebit2).ToString();

                var sms3 = ctx.SMS_MASTER.FirstOrDefault(u => u.NAME.Equals("MAX_DEBIT_KIND_3") && u.ACTIVE.Equals("A"));
                sms3.VALUE = decimal.Parse(MaxDebit3).ToString();

                var sms4 = ctx.SMS_MASTER.FirstOrDefault(u => u.NAME.Equals("MAX_MOUNTH_KIND_1") && u.ACTIVE.Equals("A"));
                sms4.VALUE = int.Parse(MaxMonth1).ToString();

                var sms5 = ctx.SMS_MASTER.FirstOrDefault(u => u.NAME.Equals("MAX_MOUNTH_KIND_2") && u.ACTIVE.Equals("A"));
                sms5.VALUE = int.Parse(MaxMonth2).ToString();

                var sms6 = ctx.SMS_MASTER.FirstOrDefault(u => u.NAME.Equals("MAX_MOUNTH_KIND_3") && u.ACTIVE.Equals("A"));
                sms6.VALUE = int.Parse(MaxMonth3).ToString();

                ctx.SaveChanges();
            }
            return View();
        }
    }
}
