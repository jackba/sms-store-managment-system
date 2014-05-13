using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace SMS.Models
{
    public class ImportReportModel
    {
        public IPagedList<SP_GET_IMPORT_Result> ImportList { get; set; }
        public long PageCount { get; set; }
    }

}