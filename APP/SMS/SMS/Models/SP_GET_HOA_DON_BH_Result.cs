//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMS.Models
{
    using System;
    using PagedList;

    public class HoaDonBHModel
    {
        public IPagedList<SP_GET_HOA_DON_BH_Result> HoaDonList { get; set; }
        public SP_GET_VALUE_ALL_HOA_DON_Result AllValue { get; set; }
        public long PageCount { get; set; }
    }
    public partial class SP_GET_HOA_DON_BH_Result
    {
        public int MA_HOA_DON { get; set; }
        public string SO_HOA_DON { get; set; }
        public Nullable<int> MA_KHACH_HANG { get; set; }
        public string TEN_KHACH_HANG { get; set; }
        public Nullable<int> MA_NHAN_VIEN_BAN { get; set; }
        public string TEN_NGUOI_BAN { get; set; }
        public Nullable<System.DateTime> NGAY_BAN { get; set; }
        public Nullable<System.DateTime> NGAY_GIAO { get; set; }
        public string DIA_CHI_GIAO_HANG { get; set; }
        public Nullable<short> STATUS { get; set; }
        public Nullable<double> SO_TIEN_KHACH_TRA { get; set; }
        public Nullable<double> SO_TIEN_NO_GOI_DAU { get; set; }
        public Nullable<int> MA_NHAN_VIEN_THU_TIEN { get; set; }
        public string TEN_NV_TT { get; set; }
    }
}