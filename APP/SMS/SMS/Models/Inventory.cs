using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Inventory
    {
        public int MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public string TEN_DON_VI { get; set; }
        public double SO_LUONG_TON { get; set; }
        public double VALUE { get; set; }
    }
    public class InventoryTotal
    {
        public double VALUE { get; set; }
    }
    public class GetInventoryModel
    {
        public IPagedList<Inventory> InventoryList { get; set; }
        public double VALUE { get; set; }
    }

    public class SpImportRepoter
    {
        public int MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public string TEN_DON_VI { get; set; }
        public double SO_LUONG { get; set; }
        public double GIA_VON { get; set; }
        public double VALUE { get; set; }
    }

    public class SpImportRepoterModel
    {
        public IPagedList<SpImportRepoter> ResultList { get; set; }
        public double VALUE { get; set; }
    }

    public class ExportRepot
    {
        public int MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public string TEN_DON_VI { get; set; }
        public double SO_LUONG { get; set; }
        public double GIA_XUAT { get; set; }
        public double VALUE { get; set; }
    }

    public class ExportRepotModel
    {
        public IPagedList<ExportRepot> ResultList { get; set; }
        public double VALUE { get; set; }
    }

    public class ExportReportDetail
    {
        public Nullable<System.DateTime> NGAY_XUAT { get; set; }
        public Nullable<int> MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public string TEN_DON_VI { get; set; }
        public double SO_LUONG { get; set; }
        public double GIA_XUAT { get; set; }
        public double VALUE { get; set; }
    }


    public class ExportReportDetailModel
    {
        public IPagedList<ExportReportDetail> ResultList { get; set; }
        public double VALUE { get; set; }
    }

    public class ImEx
    {
        public Nullable<System.DateTime> NGAY_NHAP_XUAT { get; set; }
        public Nullable<int> MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public string TEN_DON_VI { get; set; }
        public double SO_LUONG_NHAP { get; set; }
        public double SO_LUONG_XUAT { get; set; }
    }

    public class ImExModel
    {
        public IPagedList<ImEx> ResultList { get; set; }
    }
}