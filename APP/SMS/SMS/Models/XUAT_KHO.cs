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
    
    public partial class XUAT_KHO
    {
        public XUAT_KHO()
        {
            this.CHI_TIET_XUAT_KHO = new HashSet<CHI_TIET_XUAT_KHO>();
            this.TRA_HANG = new HashSet<TRA_HANG>();
            this.NHAP_KHO = new HashSet<NHAP_KHO>();
        }
    
        public int MA_XUAT_KHO { get; set; }
        public Nullable<int> MA_HOA_DON { get; set; }
        public Nullable<short> LY_DO_XUAT { get; set; }
        public Nullable<int> MA_KHO_XUAT { get; set; }
        public Nullable<int> MA_KHO_NHAN { get; set; }
        public Nullable<System.DateTime> NGAY_XUAT { get; set; }
        public Nullable<int> MA_NHAN_VIEN_XUAT { get; set; }
        public string TEN_NGUOI_NHAN_HANG { get; set; }
        public string GHI_CHU { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
        public Nullable<int> MA_PHIEU_TRA_NCC { get; set; }
    
        public virtual ICollection<CHI_TIET_XUAT_KHO> CHI_TIET_XUAT_KHO { get; set; }
        public virtual HOA_DON HOA_DON { get; set; }
        public virtual KHO KHO { get; set; }
        public virtual KHO KHO1 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG2 { get; set; }
        public virtual ICollection<TRA_HANG> TRA_HANG { get; set; }
        public virtual TRA_HANG_NCC TRA_HANG_NCC { get; set; }
        public virtual ICollection<NHAP_KHO> NHAP_KHO { get; set; }
        public int? SAVE_FLG { get; set; }
    }
}
