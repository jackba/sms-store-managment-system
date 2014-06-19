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

    public class ImportModel
    {
        public int StoreId { get; set; }
        public int ProviderId { get; set; }
        public NHAP_KHO Infor { get; set; }
        public List<KHO> Stores { get; set; }
        public List<NHA_CUNG_CAP> Providers { get; set; }
        public List<CHI_TIET_NHAP_KHO> Detail { get; set; }
    }
}