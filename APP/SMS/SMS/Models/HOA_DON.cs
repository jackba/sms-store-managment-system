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
    
    public partial class HOA_DON
    {
        public HOA_DON()
        {
            this.CHI_TIET_HOA_DON = new HashSet<CHI_TIET_HOA_DON>();
            this.KHACH_HANG_DEBIT_HIST = new HashSet<KHACH_HANG_DEBIT_HIST>();
            this.XUAT_KHO = new HashSet<XUAT_KHO>();
            this.TRA_HANG = new HashSet<TRA_HANG>();
        }
    
        public int MA_HOA_DON { get; set; }
        public string SO_HOA_DON { get; set; }
        public Nullable<int> MA_KHACH_HANG { get; set; }
        public string TEN_KHACH_HANG { get; set; }
        public Nullable<int> MA_NHAN_VIEN_BAN { get; set; }
        public Nullable<System.DateTime> NGAY_BAN { get; set; }
        public Nullable<System.DateTime> NGAY_GIAO { get; set; }
        public string DIA_CHI_GIAO_HANG { get; set; }
        public Nullable<short> STATUS { get; set; }
        public Nullable<int> EDIT_APPROVER { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
        public Nullable<double> SO_TIEN_KHACH_TRA { get; set; }
        public Nullable<double> SO_TIEN_NO_GOI_DAU { get; set; }
        public Nullable<int> MA_NHAN_VIEN_THU_TIEN { get; set; }
    
        public virtual ICollection<CHI_TIET_HOA_DON> CHI_TIET_HOA_DON { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual KHACH_HANG KHACH_HANG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG2 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG3 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG4 { get; set; }
        public virtual ICollection<KHACH_HANG_DEBIT_HIST> KHACH_HANG_DEBIT_HIST { get; set; }
        public virtual ICollection<XUAT_KHO> XUAT_KHO { get; set; }
        public virtual ICollection<TRA_HANG> TRA_HANG { get; set; }
    }
}
