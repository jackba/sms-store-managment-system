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

        public ActionResult Index()
        {

            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"] )
            {
                ViewBag.Message = "Bạn không có quyền vào mục này.";
                return View("../Home/Index");
            }
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
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                ViewBag.Message = "Bạn không có quyền vào mục này.";
                return View("../Home/Index");
            }
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
