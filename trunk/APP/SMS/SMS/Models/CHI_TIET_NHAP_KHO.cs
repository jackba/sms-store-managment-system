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
    
    public partial class CHI_TIET_NHAP_KHO
    {
        public Nullable<int> MA_NHAP_KHO { get; set; }
        public Nullable<int> MA_SAN_PHAM { get; set; }
        public Nullable<double> SO_LUONG { get; set; }
        public Nullable<double> GIA_VON { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
        public int ID { get; set; }
    
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NHAP_KHO NHAP_KHO { get; set; }
        public virtual SAN_PHAM SAN_PHAM { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
    }
}