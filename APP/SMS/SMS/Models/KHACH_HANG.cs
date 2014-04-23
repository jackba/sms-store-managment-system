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
    using System.Web.Mvc;
    using PagedList;

    public class KhachHangModel
    {
        public KHACH_HANG KhachHang { get; set; }
        public KHU_VUC KhuVuc { get; set; }
        public IPagedList<KhachHangDebitHistModel> KhachHangHists { get; set; }
        public IPagedList<GET_HOA_DON_Result> OrderHist { get; set; }
        public IPagedList<KHACH_HANG_RESULT> WarningList { get; set; }
        public NGUOI_DUNG NguoiTao { get; set; }
        public NGUOI_DUNG NguoiCapNhat { get; set; }
    }

    public partial class KHACH_HANG_RESULT
    {
        public int MA_KHACH_HANG  { get; set; }
        public string TEN_KHACH_HANG { get; set; }
		public string MA_THE_KHACH_HANG  { get; set; }
		public string DIA_CHI  { get; set; }
		public string SO_DIEN_THOAI  { get; set; }
		public string EMAIL  { get; set; }
		public int MA_KHU_VUC  { get; set; }
        public double DOANH_SO { get; set; }
		public double NO_GOI_DAU  { get; set; } 
		public DateTime NGAY_PHAT_SINH_NO  { get; set; }
		public int KIND  { get; set; }
		public string TEN_KHU_VUC  { get; set; }
		public string KIND_NAME { get; set; }
    }
    public partial class KHACH_HANG
    {
        public KHACH_HANG()
        {
            this.HOA_DON = new HashSet<HOA_DON>();
            this.KHACH_HANG_DEBIT_HIST = new HashSet<KHACH_HANG_DEBIT_HIST>();
        }
    
        public int MA_KHACH_HANG { get; set; }
        [Display(Name = "Mã thẻ khách hàng")]
        [StringLength(20)]
        public string MA_THE_KHACH_HANG { get; set; }

        [Required]
        [Display(Name = "Tên khách hàng")]
        [StringLength(50)]
        public string TEN_KHACH_HANG { get; set; }

        [Required]
        [Display(Name = "Địa chỉ")]
        [StringLength(200)]
        public string DIA_CHI { get; set; }

        [Required]
        [Display(Name = "Số điện thoại")]
        [StringLength(15, ErrorMessage = "Số CMND không hợp lệ", MinimumLength = 8)]
        public string SO_DIEN_THOAI { get; set; }

        [Display(Name = "Email")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string EMAIL { get; set; }

        [Required]
        [Display(Name = "Khu vực")]
        public Nullable<int> MA_KHU_VUC { get; set; }

        [Display(Name = "Doanh số")]
        //[DisplayFormat(DataFormatString = "0,0.n2", ApplyFormatInEditMode = true)]
        public decimal DOANH_SO { get; set; }

        [Display(Name = "Nợ gối đầu")]
        //[DisplayFormat(DataFormatString = "0,0.n2",  ApplyFormatInEditMode = true)]
        public decimal NO_GOI_DAU { get; set; }

        public byte[] HINH_ANH { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }

        [Display(Name = "Ngày phát sinh nợ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public Nullable<DateTime> NGAY_PHAT_SINH_NO { get; set; }

        [Display(Name = "Loại khách hàng")]
        public int KIND { get; set; }
    
        public virtual ICollection<HOA_DON> HOA_DON { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual KHU_VUC KHU_VUC { get; set; }
        public virtual ICollection<KHACH_HANG_DEBIT_HIST> KHACH_HANG_DEBIT_HIST { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
    }
}
