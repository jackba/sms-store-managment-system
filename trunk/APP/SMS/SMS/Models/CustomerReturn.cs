using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace SMS.Models
{

    public class CustomerReturn
    {
        public IPagedList<SP_GET_RETURN_LIST_Result> detailReturnList { get; set; }
        public int Count { get; set; }
    }

    public class RefundModel
    {
        public TRA_HANG Infor { get; set; }
        public List<SP_GET_REFUND_DETAIL_Result> Detail { get; set; }
        public DateTime? NgayNhapKho { get; set; }
        public int? MaKho { get; set; }
    }

    public class ReturnToProviderModel
    {
        public TRA_HANG Infor { get; set; }
        public List<SP_GET_REFUND_DETAIL_Result> Detail { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? ProviderId { get; set; }
        public string Note { get; set; }
        public int price { get; set; }
    }

    public class List2ProviderModel
    {
        public IPagedList<SP_GET_LIST_RETURN_TO_PROVIDERS_Result> Detail { get; set; }
        public int Count { get; set; }
    }

}