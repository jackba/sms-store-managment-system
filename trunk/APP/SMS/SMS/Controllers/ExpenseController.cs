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

        public ActionResult AddExpense()
        {
            return View();
        }

        public ActionResult Index()
        {
            ExpenesModel model = new ExpenesModel();
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
