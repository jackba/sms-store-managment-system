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
    
    public partial class TRA_HANG_NCC
    {
        public TRA_HANG_NCC()
        {
            this.TRA_HANG_NCC_CHI_TIET = new HashSet<TRA_HANG_NCC_CHI_TIET>();
            this.XUAT_KHO = new HashSet<XUAT_KHO>();
        }
    
        public int ID { get; set; }
        public Nullable<int> MA_NHA_CUNG_CAP { get; set; }
        public Nullable<int> MA_PHIEU_TRA { get; set; }
        public Nullable<System.DateTime> NGAY_LAP_PHIEU { get; set; }
        public Nullable<int> NGUOI_LAP_PHIEU { get; set; }
        public string GHI_CHU { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
    
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG2 { get; set; }
        public virtual NHA_CUNG_CAP NHA_CUNG_CAP { get; set; }
        public virtual TRA_HANG TRA_HANG { get; set; }
        public virtual ICollection<TRA_HANG_NCC_CHI_TIET> TRA_HANG_NCC_CHI_TIET { get; set; }
        public virtual ICollection<XUAT_KHO> XUAT_KHO { get; set; }
    }
}
