using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data;
using PagedList;
using System.Data.SqlClient;

namespace SMS.Controllers
{
    public class ExportController : Controller
    {
        //
        // GET: /Export/


        [HttpPost]
        public ActionResult Export(ExportModel model)
        {
            var ctx = new SmsContext();

            var storeId = new SqlParameter
            {
                ParameterName = "MA_KHO",
                Value = Convert.ToInt32(model.storeId)
            };

            var InvoiceId = new SqlParameter
            {
                ParameterName = "MA_HOA_DON",
                Value = Convert.ToInt32(model.Infor.MA_HOA_DON)
            };
            var UserId = new SqlParameter
            {
                ParameterName = "MA_NHAN_VIEN_THUC_HIEN",
                Value = Convert.ToInt32(Session["UserId"])
            };
            var customerName = new SqlParameter
            {
                ParameterName = "TEN_KHACH_HANG",
                Value = model.Infor.TEN_KHACH_HANG
            };
            var returnValue = new SqlParameter
            {
                ParameterName = "RETURN_VALUE",
                Value = Convert.ToInt32(0),
                Direction = ParameterDirection.Output
            };

            ctx.Database.CommandTimeout = 300;
            var export = ctx.Database.ExecuteSqlCommand("exec SP_SALE_EXPORT @MA_KHO, @MA_HOA_DON, @MA_NHAN_VIEN_THUC_HIEN, @TEN_KHACH_HANG , @RETURN_VALUE OUT",
                storeId,
                InvoiceId,
                UserId,
                customerName,
                returnValue
             );
            
            int returnVal = Convert.ToInt32(returnValue.Value);

            if(returnVal == -1)
            {
                return RedirectToAction("Index", new { @message = "Không thể xuất kho hóa đơn này. Lý do: có thể hóa đơn đã được xuất kho, hay đã bị hủy." });
            }
            else if (returnVal == 0)
            {
                ViewBag.Message = "Không đủ số lượng để xuất kho";
            }else
            {
                return RedirectToAction("Index", new { @messageInfor = "Xuất kho thành công" });
            }
            var infor = ctx.SP_GET_HOA_DON_INFO(model.storeId).FirstOrDefault();
            var detailList = ctx.SP_GET_HD_DETAIL_FOR_EXPORT(Convert.ToInt32(model.storeId), Convert.ToInt32(model.Infor.MA_HOA_DON)).ToList<SP_GET_HD_DETAIL_FOR_EXPORT_Result>();
            model.DetailList = detailList;
            model.Infor = infor;
            return View(model);
        }

        public ActionResult Export(int id, int? makho)
        {
            var ctx = new SmsContext();
            var infor =  ctx.SP_GET_HOA_DON_INFO(id).FirstOrDefault();
            if (makho == null)
            {
                makho = 0;
            }
            var detailList = ctx.SP_GET_HD_DETAIL_FOR_EXPORT(Convert.ToInt32(makho), Convert.ToInt32(id)).ToList<SP_GET_HD_DETAIL_FOR_EXPORT_Result>();
            ExportModel model = new ExportModel();
            model.DetailList = detailList;
            model.Infor = infor;
            model.storeId = Convert.ToInt32(makho);
            return View(model);
        }


        public ActionResult Index(string message, string messageInfor)
        {
            ViewBag.Message = message;
            ViewBag.MessageInfor = messageInfor;
            return View();
        }

        [HttpPost]
        public PartialViewResult PagingContent(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? storeId, string storeName, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(storeName))
            {
                storeId = 0;
            }
            if (fromdate == null)
            {
                fromdate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
            }

            if (todate == null)
            {
                todate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(todate.ToString()).ToString("dd/MM/yyyy");
            }
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var list = ctx.SP_GET_HOA_DON_CAN_XUAT_KHO(Convert.ToInt32(customerId), customerName, Convert.ToInt32(storeId), fromdate, todate).OrderByDescending(uh => uh.NGAY_BAN)
                .Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result>();
            ExportModel model = new ExportModel();
            model.WaitingList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.storeId = storeId;
            return PartialView("IndexPartialView", model);
        }

        [HttpPost]
        public PartialViewResult IndexPartialView(DateTime? fromdate, DateTime? todate,
            int? customerId, string customerName, int? storeId, string storeName, int? currentPageIndex)
        {

            if (string.IsNullOrEmpty(customerName))
            {
                customerId = 0;
            }
            if (string.IsNullOrEmpty(storeName))
            {
                storeId = 0;
            }
            if (fromdate == null)
            {
                fromdate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
            }

            if (todate == null)
            {
                todate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(todate.ToString()).ToString("dd/MM/yyyy");
            }
            if (!(bool)Session["IsAdmin"] && !(bool)Session["IsAccounting"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var list = ctx.SP_GET_HOA_DON_CAN_XUAT_KHO(Convert.ToInt32(customerId), customerName, Convert.ToInt32(storeId), fromdate, todate).OrderByDescending(uh => uh.NGAY_BAN)
                .Take(SystemConstant.MAX_ROWS).ToList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result>();
            ExportModel model = new ExportModel();
            model.WaitingList = list.ToPagedList(pageIndex, pageSize);
            model.PageCount = list.Count;
            ViewBag.storeId = storeId;
            return PartialView("IndexPartialView", model);
        }
    }
}
