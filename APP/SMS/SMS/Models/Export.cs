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
    public class SaleExportListModel
    {
        public Nullable<int> MA_XUAT_KHO { get; set; }
        public Nullable<int> MA_HOA_DON { get; set; }
        public Nullable<System.DateTime> NGAY_XUAT { get; set; }
        public Nullable<int> MA_NHAN_VIEN_XUAT { get; set; }
        public string TEN_NHAN_VIEN_XK { get; set; }
        public string TEN_NGUOI_NHAN_HANG { get; set; }
        public string SO_HOA_DON { get; set; }
        public Nullable<int> MA_KHACH_HANG { get; set; }
        public string TEN_KHACH_HANG { get; set; }
        public string DIA_CHI_GIAO_HANG { get; set; }
        public string TEN_KHO { get; set; }
    }

    public class ExportModel
    {
        public IPagedList<SP_GET_HOA_DON_CAN_XUAT_KHO_Result> WaitingList { get; set; }
        public IPagedList<SaleExportListModel> SaleExportList { get; set; }
        public IPagedList<SP_GET_HOA_DON_EXPORTED_Result> ExportedList { get; set; }
        public SP_GET_HOA_DON_INFO_Result Infor { get; set; }
        public List<SP_GET_HD_DETAIL_FOR_EXPORT_Result> DetailList { get; set; }
        public List<SP_GET_STORES_BY_USR_ID_Result> Stores { get; set; }
        public int storeId { get; set; }
        public long PageCount { get; set; }
    }

    public class ExportedDetailModel
    {
        public List<SP_GET_EXPORT_DETAIL_BY_ID_Result> thelist { get; set; }

        public int flg { get; set; }
    }
    public class CancelExportModel
    {
        public IPagedList<SP_GET_EXPORT_4_CANCEL_Result> TheList { get; set; }
        public int Count { get; set; }
    }

    public class ExportModelXuatHuy
    {
        public int StoreId { get; set; }
        public int ProviderId { get; set; }
        public XUAT_KHO Infor { get; set; }
        public List<KHO> Stores { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<CHI_TIET_XUAT_KHO> Detail { get; set; }
    }

    public class WaitingExport2ProviderListModel
    {
        public IPagedList<SP_GET_WAITING_EX_2_PROVIDER_Result> TheList { get; set; }
        public int Count { get; set; }
    }

    public class Export2ProviderModel
    {
        public TRA_HANG_NCC Infor { get; set; }
        public int StoreId { get; set; }
        public DateTime exportDate { get; set; }
        public List<SP_GET_DE_OF_RE_2_PR_BY_ST_AND_INV_ID_Result> TheList { get; set; }
    }
}