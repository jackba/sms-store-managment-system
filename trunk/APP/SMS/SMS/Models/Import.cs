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
        public List<DON_VI_TINH> Units { get; set; }
        public List<NHA_CUNG_CAP> Providers { get; set; }
        public List<CHI_TIET_NHAP_KHO> Detail { get; set; }
    }

    public class TransferModel
    {
        public NHAP_KHO ImportInfor { get; set; }
        public List<KHO> Stores { get; set; }
        public XUAT_KHO ExportInfor { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<CHI_TIET_XUAT_KHO> ExportDetail { get; set; }
        public List<CHI_TIET_NHAP_KHO> ImportDetail { get; set; }
    }


    public class EditTransferModel
    {
        public SP_GET_PHIEU_CHUYEN_KHO_INFO_BY_ID_Result Infor { get; set; }
        public List<KHO> Stores { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<CHI_TIET_XUAT_KHO> ExportDetail { get; set; }
    }
    public class ListExportTransferModel
    {
        public IPagedList<SP_GET_PHIEU_CHUYEN_KHO_Result> TheList { get; set; }
        public int Count { get; set; }
    }
}