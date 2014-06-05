using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;
using SMS.App_Start;

namespace SMS.Controllers
{
    [CustomActionFilter]
    public class ImportController : Controller
    {
        //
        // GET: /Import/

        [HttpPost]
        public PartialViewResult IndexPartialView(DateTime? fromDate, DateTime? toDate, int? importerId,
            int? reasonId, int? storeId, int? providerId, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            fromDate = fromDate == null ? SystemConstant.MIN_DATE : fromDate;
            toDate = toDate == null ? SystemConstant.MAX_DATE : toDate;
            ViewBag.InputKind = Convert.ToInt32(reasonId);
            if ((bool)Session["IsAdmin"])
            {
                if (importerId == null)
                {
                    importerId = 0;
                }
            }
            else
            {
                importerId = (int)Session["UserId"];
            }
            var resultList = ctx.SP_GET_IMPORT(fromDate, toDate, Convert.ToInt32(importerId), Convert.ToInt32(reasonId)
                , Convert.ToInt32(storeId), Convert.ToInt32(providerId)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_IMPORT_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ImportReportModel reportModel = new ImportReportModel();
            reportModel.ImportList = resultList.ToPagedList(pageIndex, pageSize);
            reportModel.PageCount = resultList.Count;
            return View(reportModel);
        }

        [HttpGet]
        public ActionResult Index(DateTime? fromDate, DateTime? toDate, int? importerId, int? reasonId, int? storeId, int? providerId, int? page )
        {
            var ctx = new SmsContext();
            fromDate = fromDate == null ? SystemConstant.MIN_DATE : fromDate;
            toDate = toDate == null ? SystemConstant.MAX_DATE : toDate;
            ViewBag.InputKind = Convert.ToInt32(reasonId);
            if ((bool)Session["IsAdmin"])
            {
                if (importerId == null)
                {
                    importerId = 0;
                }
            }
            else
            {
                importerId = (int)Session["UserId"];
            }
            var resultList = ctx.SP_GET_IMPORT(fromDate, toDate, Convert.ToInt32(importerId), Convert.ToInt32(reasonId)
                , Convert.ToInt32(storeId), Convert.ToInt32(providerId)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_IMPORT_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ImportReportModel reportModel = new ImportReportModel();
            reportModel.ImportList = resultList.ToPagedList(pageIndex, pageSize);
            reportModel.PageCount = resultList.Count;
            return View(reportModel);
        }

        [HttpPost]
        public ActionResult Index(DateTime? fromDate, DateTime? toDate, int? importerId, int? reasonId, int? storeId, int? providerId, int? page, bool?flg)
        {
            var ctx = new SmsContext();
            fromDate = fromDate == null ? SystemConstant.MIN_DATE : fromDate;
            toDate = toDate == null ? SystemConstant.MAX_DATE : toDate;
            ViewBag.InputKind = Convert.ToInt32(reasonId);
            if ((bool)Session["IsAdmin"])
            {
                if (importerId == null)
                {
                    importerId = 0;
                }
            }
            else
            {
                importerId = (int)Session["UserId"];
            }
            var resultList = ctx.SP_GET_IMPORT(fromDate, toDate, Convert.ToInt32(importerId), Convert.ToInt32(reasonId)
                , Convert.ToInt32(storeId), Convert.ToInt32(providerId)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_IMPORT_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ImportReportModel reportModel = new ImportReportModel();
            reportModel.ImportList = resultList.ToPagedList(pageIndex, pageSize);
            reportModel.PageCount = resultList.Count;
            return View(reportModel);
        }
    }
}
