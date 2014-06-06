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
            var ctx = new SmsContext();
            var nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(uh => uh.ACTIVE == "A").ToList<NHOM_NGUOI_DUNG>();
            ViewBag.GroupUserList = nhomNguoiDung;
            return View();
        }


        [HttpPost]
        public ActionResult AddNew(SMS_MESSAGES model)
        {
            if (ModelState.IsValid)
            {
                var ctx = new SmsContext();
                var smsMessage = ctx.SMS_MESSAGES.Create();
                smsMessage.ID_NHOM_NGUOI_NHAN = model.ID_NHOM_NGUOI_NHAN;
                smsMessage.NOI_DUNG = model.NOI_DUNG;
                smsMessage.NGAY_GUI = DateTime.Now;
                smsMessage.ID_NGUOI_GUI = Convert.ToInt32(Session["UserId"]);
                smsMessage.ACTIVE = "A";
                smsMessage.CREATE_AT = DateTime.Now;
                smsMessage.UPDATE_AT = DateTime.Now;
                smsMessage.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                smsMessage.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                ctx.SMS_MESSAGES.Add(smsMessage);
                ctx.SaveChanges();
                return RedirectToAction("Index", new { @inforMessage = "Lưu thành công." });
            }
            else
            {
                var ctx = new SmsContext();
                var nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(uh => uh.ACTIVE == "A").ToList<NHOM_NGUOI_DUNG>();
                ViewBag.GroupUserList = nhomNguoiDung;
                return View();
            }
        }
        
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy tin nhắn tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.SMS_MESSAGES.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {
                if (donvi.ID_NGUOI_GUI != (int)Session["UserId"])
                {
                    return RedirectToAction("Index", new { @message = "Bạn không phải là người tạo ra tin nhắn này, bạn không có quyền xóa nó." });
                }
                donvi.ACTIVE = "I";
                donvi.UPDATE_AT = DateTime.Now;
                donvi.CREATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                return RedirectToAction("Index", new { @inforMessage = "Xóa thành công." });
            }
            else
            {
                ViewBag.Message = "Không tìm thấy tin nhắn tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [HttpPost]
        public ActionResult Edit(SMS_MESSAGES model)
        {
            var ctx = new SmsContext();
            var nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(uh => uh.ACTIVE == "A").ToList<NHOM_NGUOI_DUNG>();
            if (ModelState.IsValid)
            {
                if (model.ID_NGUOI_GUI != (int)Session["UserId"])
                {
                    ViewBag.Message = "Bạn không phải là người tạo ra tin nhắn này, bạn không có quyền thay đổi nó.";
                    ViewBag.GroupUserList = nhomNguoiDung;
                    return View(model);
                }else
                {
                    
                    var sms = ctx.SMS_MESSAGES.Find(model.ID);
                    sms.ID_NHOM_NGUOI_NHAN = model.ID_NHOM_NGUOI_NHAN;
                    sms.NOI_DUNG = model.NOI_DUNG;
                    sms.UPDATE_AT = DateTime.Now;
                    sms.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();
                    return RedirectToAction("Index", new { @inforMessage = "Lưu thành công." });
                }
            }
            ViewBag.GroupUserList = nhomNguoiDung;
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy đơn vị tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            SMS_MESSAGES donvi = ctx.SMS_MESSAGES.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {
                ViewBag.donVi = donvi;
                var nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(uh => uh.ACTIVE == "A").ToList<NHOM_NGUOI_DUNG>();
                ViewBag.GroupUserList = nhomNguoiDung;
                return View(donvi);
            }
            else
            {
                ViewBag.Message = "Không tìm thấy đơn vị tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        public ActionResult Index(string inforMessage, string message)
        {
            ViewBag.Message = message;
            ViewBag.MessageInfor = inforMessage;
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
