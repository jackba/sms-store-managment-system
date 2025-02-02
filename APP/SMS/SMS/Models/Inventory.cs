﻿using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Compared
    {
        public int MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public int MA_DON_VI { get; set; }
        public string TEN_DON_VI { get; set; }
        public double INVEN_FIRST_DATE { get; set; }
        public double INVEN_SECOND_DATE { get; set; }
        public double IMPORT { get; set; }
        public double EXPORT { get; set; }
        public double COMPARED { get; set; }
    }

    public class CompareModel
    {
        public List<KHO> Stores { get; set; }
        public List<NHOM_SAN_PHAM> ProductGroups { get; set; }
        public List<SP_GET_STORES_BY_USR_ID_Result> StoreList { get; set; }
        public List<SAN_PHAM> Products { get; set; }
        public IPagedList<Compared> ComparedList { get; set; }
    }

    public class Inventory
    {
        public int MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public string CODE { get; set; }
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

    public class CheckingStoreModel
    {
        public Nullable<int> ID { get; set; }
        public Nullable<int> MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public string TEN_DON_VI { get; set; }
        public double GIA_BAN_1 { get; set; }
        public double GIA_BAN_2 { get; set; }
        public double GIA_BAN_3 { get; set; }
        public double CHIEC_KHAU_1 { get; set; }
        public double CHIEC_KHAU_2 { get; set; }
        public double CHIEC_KHAU_3 { get; set; }
        public double TON_KHO_1 { get; set; }
        public double TON_KHO_2 { get; set; }
        public double TON_KHO_3 { get; set; }
        public double TON_KHO_4 { get; set; }
        public double TON_KHO_5 { get; set; }
        public double TOTAL { get; set; }

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

    public class Fifo
    {
        public Nullable<int> ID { get; set; }
        public Nullable<int> MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public string TEN_DON_VI { get; set; }
        public double SO_LUONG_NHAP { get; set; }
        public double SO_LUONG_XUAT { get; set; }
        public double GIA_VON_HANG_BAN { get; set; }
        public double SO_LUONG_TON { get; set; }
        public double GIA_VON_HANG_TON { get; set; }
    }

    public class FifoModel
    {
        public IPagedList<Fifo> ResultList { get; set; }
        public double GIA_VON_HANG_BAN_TOTAL { get; set; }
        public double GIA_TRI_HANG_TON_TOTAL { get; set; }        
    }

    public class InventoryByStoreModel
    {
        public int Count { get; set; }
        public IPagedList<SP_GET_WARNING_BY_STORE_Result> WarningList { get; set; }
    }

}