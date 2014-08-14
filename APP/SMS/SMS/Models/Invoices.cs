using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace SMS.Models
{
    public class InvoicesModel
    {
        public SP_GET_HOA_DON_INFO_Result Infor { get; set; }
        public KHACH_HANG CustomerInformation { get; set; }
        public List<SP_GET_HOA_DON_DETAIL_FOR_RETURN_Result> detailReturnList { get; set; }
        public List<V_HOA_DON> detailList { get; set; }
    }

    public class InvoicesNoReciveModel
    {
        public IPagedList<SP_GET_HOA_DON_CHUA_TT_Result> Invoices { get; set; }
        public long PageCount { get; set; }
    }

    public class ReturnBillModel
    {
        public List<DON_VI_TINH> Units { get; set; }
        public SP_GET_RETURN_INFO_BY_ID_Result Infor { get; set; }
        public List<SP_GET_RETURN_DETAIL_BY_ID_Result> Detail { get; set; }
    }

}