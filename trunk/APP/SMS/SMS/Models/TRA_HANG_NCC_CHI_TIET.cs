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
    
    public partial class TRA_HANG_NCC_CHI_TIET
    {
        public int ID { get; set; }
        public Nullable<int> MA_PHIEU_TRA_NCC { get; set; }
        public Nullable<int> MA_SAN_PHAM { get; set; }
        public Nullable<double> SO_LUONG { get; set; }
        public Nullable<double> DON_GIA { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
    
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual SAN_PHAM SAN_PHAM { get; set; }
        public virtual TRA_HANG_NCC TRA_HANG_NCC { get; set; }
    }
}