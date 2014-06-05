using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;

namespace SMS.Controllers
{
    public class SmsMessageController : Controller
    {
        //
        // GET: /SmsMessage/

        public ActionResult AddNew()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult FindGroupUser(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.NHOM_NGUOI_DUNG
                                    where (x.TEN_NHOM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_NHOM,
                                        value = x.TEN_NHOM
                                    };
            var result = Json(suggestedProducts.Take(5).ToList());
            return result;
        }

        [HttpPost]
        public PartialViewResult IndexPartialView(int? groupUserId, string groupUserName, string searchString, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(groupUserName))
            {
                groupUserId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }

            var ctx = new SmsContext();
            var messageList = ctx.SP_GET_SMS_MESSAGES(Convert.ToInt32(groupUserId), groupUserName, searchString, 
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate)).ToList<SP_GET_SMS_MESSAGES_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            SmsMessageModel model = new SmsMessageModel();
            model.Count = messageList.Count;
            model.MessageList = messageList.ToPagedList(pageIndex, pageSize);
            ViewBag.GroupUserId = groupUserId;
            ViewBag.GroupUserName = groupUserName;
            ViewBag.SearchString = searchString;
            return PartialView("IndexPartialView", model);
        }
    }
}
