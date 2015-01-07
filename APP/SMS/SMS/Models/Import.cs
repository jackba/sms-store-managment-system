using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class ImportReportModel
    {
        public IPagedList<SP_GET_IMPORT_Result> ImportList { get; set; }
        public long PageCount { get; set; }
    }

    public class ImportCsvFile
    {
        public string No { get; set; }
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string UniName { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
    }

    public class ImportCsvModel
    {
        public List<ImportCsvFile> TheList { get; set; }
        public List<KHO> Stores { get; set; }

        [Required]
        public int StoreId { get; set; }
        [Required]
        public DateTime ImportDate { get; set; }
        public long Count { get; set; }
    }
    public class ImportModel
    {
        public int StoreId { get; set; }
        public int ProviderId { get; set; }
        public NHAP_KHO Infor { get; set; }
        public List<KHO> Stores { get; set; }
        public List<SP_GET_STORES_BY_USR_ID_Result> UserStore { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<NHA_CUNG_CAP> Providers { get; set; }
        public List<CHI_TIET_NHAP_KHO> Detail { get; set; }
    }

    public class ReturnNoBillModel
    {
        public TRA_HANG ReturnInfor { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<CHI_TIET_TRA_HANG> ReturnDetail { get; set; }
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
        public List<SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN_Result> ExportDetail { get; set; }
    }

    public class EditCancelTicketModel
    {
        public XUAT_KHO Infor { get; set; }
        public List<KHO> Stores { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN_Result> Detail { get; set; }
    }

    public class ImportTransferModel
    {
        public SP_GET_PHIEU_CHUYEN_KHO_INFO_BY_ID_Result Infor { get; set; }
        public NHAP_KHO ImportInfor { get; set; }
        public List<KHO> Stores { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN_Result> ExportDetail { get; set; }
    }

    public class ListExportTransferModel
    {
        public IPagedList<SP_GET_PHIEU_CHUYEN_KHO_Result> TheList { get; set; }
        public List<SP_GET_STORES_BY_USR_ID_Result> StoreList { get; set; }
        public int Count { get; set; }
    }

    public class EditImportModel
    {
        public SP_GET_IMPORT_INFOR_BY_ID_Result Infor { get; set; }
        public List<SP_GET_IMPORT_DETAIL_BY_ID_4_EDIT_Result> Detail { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<KHO> Stores { get; set; }
        public List<NHA_CUNG_CAP> Providers { get; set; }
    }

    public class ImportDetailModel
    {
        public SP_GET_IMPORT_INFOR_BY_ID_Result Infor { get; set; }
        public List<SP_GET_IMPORT_DETAIL_BY_ID_Result> Details { get; set; }
        public List<KHO> Stores { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
    }
}