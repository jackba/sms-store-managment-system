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
    
    public partial class TON_KHO_MIN_MAX_KHO
    {
        public int ID { get; set; }
        public Nullable<int> MA_SAN_PHAM { get; set; }
        public Nullable<int> MA_KHO { get; set; }
        public Nullable<double> CO_SO_TOI_THIEU { get; set; }
        public Nullable<double> CO_SO_TOI_DA { get; set; }
        public string GHI_CHU { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
    
        public virtual KHO KHO { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual SAN_PHAM SAN_PHAM { get; set; }
    }
}
