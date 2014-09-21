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
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id < 0)
            {
                return RedirectToAction("Index", new { @message = "Không tồn tại phiếu chi này" });
            }
            var ctx = new SmsContext();
            var Ex = ctx.EXPENSES.Find(id);
            if (Ex.ACTIVE != "A")
            {
                return RedirectToAction("Index", new { @message = "Không tồn tại phiếu chi này. Phiếu chi này đã bị xóa." });
            }
            else
            {
                Ex.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                Ex.UPDATE_AT = DateTime.Now;
                Ex.ACTIVE = "I";
                ctx.SaveChanges();
                return RedirectToAction("Index", new { @inforMessage = "Xóa thành công." });
            }
        }


        [HttpGet]
        public ActionResult EditExpense(int id)
        {
            if (id < 0)
            {
                return RedirectToAction("Index", new { @message = "Không tồn tại phiếu chi này" });
            }
            var ctx = new SmsContext();
            var Ex = ctx.EXPENSES.Find(id);
            if(Ex.ACTIVE != "A")
            {
                return RedirectToAction("Index", new { @message = "Không tồn tại phiếu chi này. Phiếu chi này đã bị xóa." });
            }
            return View(Ex);
        }

        [HttpPost]
        public ActionResult EditExpense(EXPENS model)
        {
            if (ModelState.IsValid)
            {
                var ctx = new SmsContext();
                var Ex = ctx.EXPENSES.Find(model.ID);
                if (Ex != null && Ex.ACTIVE == "A")
                {
                    Ex.LOAI_CHI = model.LOAI_CHI;
                    Ex.NGAY_CHI = model.NGAY_CHI;
                    Ex.GHI_CHU = model.GHI_CHU;
                    Ex.TEN_NGUOI_NHAN = model.TEN_NGUOI_NHAN;
                    Ex.TONG_CHI = model.TONG_CHI;
                    Ex.UPDATE_AT = DateTime.Now;
                    Ex.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();
                    return RedirectToAction("Index", new { @inforMessage = "Lưu thành công." });
                }
                else
                {
                    return RedirectToAction("Index", new { @message = "Không tồn tại phiếu chi này. Phiếu chi này đã bị xóa." });
                }
            }
            return View(model);
        }

        [HttpPost]
        public FileContentResult downloadExpenses(int? kind, string reciever, int? userId, string userName, 
            float? totalFrom, float? totalTo, DateTime? fromDate, DateTime? toDate)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = string.Empty;
                userId = 0;
            }  
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"Ngày chi tiền\",");
            fileStringBuilder.Append("\"Người chi tiền\",");
            fileStringBuilder.Append("\"Tên người nhận tiền\",");
            fileStringBuilder.Append("\"Mục đích chi tiền\",");
            fileStringBuilder.Append("\"Tổng tiền\"");
            var ctx = new SmsContext();
            var expenses = ctx.SP_GET_EXPENSES(Convert.ToInt32(kind), reciever, userId, userName, totalFrom, totalTo, fromDate, toDate).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_EXPENSES_Result>();
            int i = 0;
            string str = "";
            foreach (var detail in expenses)
            {
                fileStringBuilder.Append("\n");
                i += 1;
                fileStringBuilder.Append("\"" + i + "\",");
                fileStringBuilder.Append("\"" + ((DateTime)detail.NGAY_CHI).ToString("dd/MM/yyyy") + "\",");
                fileStringBuilder.Append("\"" + detail.TEN_NGUOI_DUNG + "\",");
                fileStringBuilder.Append("\"" + detail.TEN_NGUOI_NHAN + "\",");
                switch (detail.LOAI_CHI)
                {
                    case 1: 
                        str = "Chi mua hàng";
                        break;
                    case 2:
                        str = "Chi cho vận chuyển";
                        break;
                    case 3:
                        str = "Chi cho tiền lương nhân viên";
                        break;
                    case 4: 
                        str = "Chi vào mục đích khác";
                        break;
                }
                fileStringBuilder.Append("\"" + str + "\",");
                fileStringBuilder.Append("\"" + detail.TONG_CHI.ToString("#,###,##") + "\",");
            }
            return File(new System.Text.UTF8Encoding().GetBytes(fileStringBuilder.ToString()), "text/csv", fileName + ".csv");
        }

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
