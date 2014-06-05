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
        public PartialViewResult IndexPartialView(DateTime? fromDate, DateTime? toDate, int? importerId, string importerName,
            int? reasonId, int? storeId, string storeName, int? providerId, string providerName, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            fromDate = fromDate == null ? SystemConstant.MIN_DATE : fromDate;
            toDate = toDate == null ? SystemConstant.MAX_DATE : toDate;
            ViewBag.InputKind = Convert.ToInt32(reasonId);

            if (string.IsNullOrEmpty(storeName))
            {
                storeName = string.Empty;
                storeId = 0;
            }

            if (string.IsNullOrEmpty(importerName))
            {
                importerName = string.Empty;
                importerId = 0;
            }

            if (string.IsNullOrEmpty(providerName))
            {
                providerName = string.Empty;
                providerId = 0;
            }

            if (!(bool)Session["IsAdmin"])
            {
                importerId = (int)Session["UserId"];
                storeId = (int)Session["MyStore"];
            }

            ViewBag.ProviderId = providerId;
            ViewBag.ProviderName = providerName;
            ViewBag.ImporterId = importerId;
            ViewBag.ImporterName = importerName;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            var resultList = ctx.SP_GET_IMPORT(fromDate, toDate, Convert.ToInt32(importerId), importerName, Convert.ToInt32(reasonId)
                , Convert.ToInt32(storeId), storeName, Convert.ToInt32(providerId), providerName).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_IMPORT_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ImportReportModel reportModel = new ImportReportModel();
            reportModel.ImportList = resultList.ToPagedList(pageIndex, pageSize);
            reportModel.PageCount = resultList.Count;
            return PartialView("IndexPartialView", reportModel);
        }

        [HttpGet]
        public ActionResult Index(int? reasonId)
        {
            ViewBag.InputKind = Convert.ToInt32(reasonId);
            return View();
        }
    }
}
