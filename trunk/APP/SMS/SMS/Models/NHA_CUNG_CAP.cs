//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NHA_CUNG_CAP
    {
        public NHA_CUNG_CAP()
        {
            this.NHAP_KHO = new HashSet<NHAP_KHO>();
        }
    
        public int MA_NHA_CUNG_CAP { get; set; }
        public string TEN_NHA_CUNG_CAP { get; set; }
        public string DIA_CHI { get; set; }
        public string TEN_NGUOI_LIEN_HE { get; set; }
        public string SO_DIEN_THOAI { get; set; }
        public string EMAIL { get; set; }
        public string GHI_CHU { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
    
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual ICollection<NHAP_KHO> NHAP_KHO { get; set; }
    }
}