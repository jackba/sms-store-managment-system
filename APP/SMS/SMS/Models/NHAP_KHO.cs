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
    using System.Collections.Generic;
    
    public partial class NHAP_KHO
    {
        public NHAP_KHO()
        {
            this.CHI_TIET_NHAP_KHO = new HashSet<CHI_TIET_NHAP_KHO>();
            this.KIEM_KHO_HISTORY = new HashSet<KIEM_KHO_HISTORY>();
        }
    
        public int MA_NHAP_KHO { get; set; }
        public string SO_HOA_DON { get; set; }
        public Nullable<System.DateTime> NGAY_NHAP { get; set; }
        public Nullable<int> NHAN_VIEN_NHAP { get; set; }
        public Nullable<int> MA_NHA_CUNG_CAP { get; set; }
        public Nullable<int> MA_KHO { get; set; }
        public Nullable<short> LY_DO_NHAP { get; set; }
        public Nullable<int> MA_PHIEU_TRA { get; set; }
        public string GHI_CHU { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
        public Nullable<int> MA_PHIEU_XUAT { get; set; }
    
        public virtual ICollection<CHI_TIET_NHAP_KHO> CHI_TIET_NHAP_KHO { get; set; }
        public virtual KHO KHO { get; set; }
        public virtual ICollection<KIEM_KHO_HISTORY> KIEM_KHO_HISTORY { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG2 { get; set; }
        public virtual NHA_CUNG_CAP NHA_CUNG_CAP { get; set; }
        public virtual TRA_HANG TRA_HANG { get; set; }
        public virtual XUAT_KHO XUAT_KHO { get; set; }
    }
}
