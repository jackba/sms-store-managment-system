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

    public class PhanQuyenObj
    {
        public PHAN_QUYEN PhanQuyen { get; set; }
        public NGUOI_DUNG NguoiTao { get; set; }
        public NGUOI_DUNG NguoiCapNhat { get; set; }
    }

    public partial class PHAN_QUYEN
    {
        public int ID { get; set; }
        public Nullable<int> MA_NGUOI_DUNG { get; set; }
        public bool QUYEN_ADMIN { get; set; }
        public bool QUYEN_DANH_MUC_SAN_PHAM { get; set; }
        public bool QUYEN_BAN_HANG { get; set; }
        public bool QUYEN_THAU_NGAN { get; set; }
        public Nullable<int> MA_NHOM_NGUOI_DUNG { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
        public bool QUYEN_QUAN_LY_KHO { get; set; }
    
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual NHOM_NGUOI_DUNG NHOM_NGUOI_DUNG { get; set; }
        public virtual NHOM_NGUOI_DUNG NHOM_NGUOI_DUNG1 { get; set; }
    }
}