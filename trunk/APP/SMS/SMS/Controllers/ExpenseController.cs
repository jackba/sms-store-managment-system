using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace SMS.Controllers
{
    public class ExpenseController : Controller
    {
        //
        // GET: /Expense/
        [HttpGet]
        public ActionResult AddExpense(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public ActionResult AddExpense(EXPENS model)
        {
            if (ModelState.IsValid)
            {
                var ctx = new SmsContext();
                var ex = ctx.EXPENSES.Create();
                ex.ACTIVE = "A";
                ex.GHI_CHU = model.GHI_CHU;
                ex.LOAI_CHI = model.LOAI_CHI;
                ex.NGAY_CHI = model.NGAY_CHI;
                ex.NGUOI_CHI = Convert.ToInt32(Session["UserId"]);
                ex.TEN_NGUOI_NHAN = model.TEN_NGUOI_NHAN;
                ex.UPDATE_AT = DateTime.Now;
                ex.CREATE_AT = DateTime.Now;
                ex.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                ex.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                ex.TONG_CHI = model.TONG_CHI;
                ctx.EXPENSES.Add(ex);
                ctx.SaveChanges();
                return RedirectToAction("Index", new { @inforMessage = "Lưu thành công." });
            }
            else
            {
                return View();
            }            
        }


        public ActionResult Index(string message, string inforMessage)
        {
            ExpenesModel model = new ExpenesModel();
            ViewBag.Message = message;
            ViewBag.InForMessage = inforMessage;
            model.Kind = 0;
            return View(model);
        }

        public PartialViewResult IndexPtv(int? kind, string reciever, int? userId, string userName, float? totalFrom, float? totalTo, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = string.Empty;
                userId = 0;
            }            
            var ctx = new SmsContext();
            var expenses = ctx.SP_GET_EXPENSES(Convert.ToInt32(kind), reciever, userId, userName, totalFrom, totalTo, fromDate,toDate).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_EXPENSES_Result>();
            ExpenesModel model = new ExpenesModel();
            model.Count = expenses.Count;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            model.ResultList = expenses.ToPagedList(pageIndex, pageSize);
            ViewBag.Kind = kind;
            ViewBag.Reciever = reciever;
            ViewBag.UserId = userId;
            ViewBag.UserName = userName;
            ViewBag.TotalFrom = totalFrom;
            ViewBag.ToTalTo = totalTo;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return PartialView("IndexPtv", model);
        }

    }
}
