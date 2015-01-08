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
        public List<SP_GET_STORES_BY_USR_ID_Result> StoreList { get; set; }
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

    public class Return2Provider
    {
        public TRA_HANG_NCC Infor { get; set; }
        public List<TRA_HANG_NCC_CHI_TIET> Details { get; set; }
        public List<KHO> Stores { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<NHA_CUNG_CAP> Providers { get; set; }
    }


    public class EditReturn2Provider
    {
        public TRA_HANG_NCC Infor { get; set; }
        public List<SP_GET_RE_DETAIL_BY_ID_Result> Details { get; set; }
        public List<KHO> Stores { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<NHA_CUNG_CAP> Providers { get; set; }
        public int Count { get; set; }
    }
    
}