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
    using System.ComponentModel.DataAnnotations;

    public class NhaCungCapModel
    {
        public NHA_CUNG_CAP NhaCungCap { get; set; }
        public NGUOI_DUNG NguoiTao { get; set; }
        public NGUOI_DUNG NguoiCapNhat { get; set; }
    }

    public partial class NHA_CUNG_CAP
    {
        public NHA_CUNG_CAP()
        {
            this.NHAP_KHO = new HashSet<NHAP_KHO>();
            this.TRA_HANG_NCC = new HashSet<TRA_HANG_NCC>();
        }
    
        public int MA_NHA_CUNG_CAP { get; set; }
        [Required]
        [Display(Name = "Tên nhà cung cấp")]
        [StringLength(100)]
        public string TEN_NHA_CUNG_CAP { get; set; }
        [Required]
        [Display(Name = "Địa chỉ")]
        [StringLength(300)]
        public string DIA_CHI { get; set; }
        [Required]
        [Display(Name = "Tên người liên hệ")]
        [StringLength(100)]
        public string TEN_NGUOI_LIEN_HE { get; set; }
        [Required]
        [Display(Name = "Số điện thoại")]
        [StringLength(15)]
        public string SO_DIEN_THOAI { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string EMAIL { get; set; }
        [Required]
        [Display(Name = "Ghi chú")]
        [StringLength(1000)]
        public string GHI_CHU { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
    
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual ICollection<NHAP_KHO> NHAP_KHO { get; set; }
        public virtual ICollection<TRA_HANG_NCC> TRA_HANG_NCC { get; set; }
    }
}