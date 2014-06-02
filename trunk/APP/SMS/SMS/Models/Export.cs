using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace SMS.Models
{
    public class Export
    {
    }
    public class ExportModel
    {
        public IPagedList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result> WaitingList { get; set; }
        public SP_GET_HOA_DON_INFO_Result Infor { get; set; }
        public List<SP_GET_HD_DETAIL_FOR_EXPORT_Result> DetailList { get; set; }
        public int storeId { get; set; }
        public long PageCount { get; set; }
    }
}