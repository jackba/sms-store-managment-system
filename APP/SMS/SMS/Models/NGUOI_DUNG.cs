﻿//------------------------------------------------------------------------------
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
    using System.ComponentModel.DataAnnotations;
    
    public partial class NGUOI_DUNG
    {
        public NGUOI_DUNG()
        {
            this.CHI_TIET_HOA_DON = new HashSet<CHI_TIET_HOA_DON>();
            this.CHI_TIET_HOA_DON1 = new HashSet<CHI_TIET_HOA_DON>();
            this.CHI_TIET_NHAP_KHO = new HashSet<CHI_TIET_NHAP_KHO>();
            this.CHI_TIET_NHAP_KHO1 = new HashSet<CHI_TIET_NHAP_KHO>();
            this.CHI_TIET_TRA_HANG = new HashSet<CHI_TIET_TRA_HANG>();
            this.CHI_TIET_TRA_HANG1 = new HashSet<CHI_TIET_TRA_HANG>();
            this.CHI_TIET_XUAT_KHO = new HashSet<CHI_TIET_XUAT_KHO>();
            this.CHI_TIET_XUAT_KHO1 = new HashSet<CHI_TIET_XUAT_KHO>();
            this.CHUYEN_DOI_DON_VI_TINH = new HashSet<CHUYEN_DOI_DON_VI_TINH>();
            this.CHUYEN_DOI_DON_VI_TINH1 = new HashSet<CHUYEN_DOI_DON_VI_TINH>();
            this.DON_VI_TINH = new HashSet<DON_VI_TINH>();
            this.DON_VI_TINH1 = new HashSet<DON_VI_TINH>();
            this.HOA_DON = new HashSet<HOA_DON>();
            this.HOA_DON1 = new HashSet<HOA_DON>();
            this.HOA_DON2 = new HashSet<HOA_DON>();
            this.HOA_DON3 = new HashSet<HOA_DON>();
            this.KHACH_HANG = new HashSet<KHACH_HANG>();
            this.KHACH_HANG1 = new HashSet<KHACH_HANG>();
            this.KHOes = new HashSet<KHO>();
            this.KHOes1 = new HashSet<KHO>();
            this.KHOes2 = new HashSet<KHO>();
            this.KHU_VUC = new HashSet<KHU_VUC>();
            this.KHU_VUC1 = new HashSet<KHU_VUC>();
            this.KIEM_KHO_HISTORY = new HashSet<KIEM_KHO_HISTORY>();
            this.KIEM_KHO_HISTORY1 = new HashSet<KIEM_KHO_HISTORY>();
            this.NGUOI_DUNG1 = new HashSet<NGUOI_DUNG>();
            this.NGUOI_DUNG11 = new HashSet<NGUOI_DUNG>();
            this.NHA_CUNG_CAP = new HashSet<NHA_CUNG_CAP>();
            this.NHA_CUNG_CAP1 = new HashSet<NHA_CUNG_CAP>();
            this.NHA_SAN_XUAT = new HashSet<NHA_SAN_XUAT>();
            this.NHA_SAN_XUAT1 = new HashSet<NHA_SAN_XUAT>();
            this.NHAP_KHO = new HashSet<NHAP_KHO>();
            this.NHAP_KHO1 = new HashSet<NHAP_KHO>();
            this.NHAP_KHO2 = new HashSet<NHAP_KHO>();
            this.NHOM_NGUOI_DUNG1 = new HashSet<NHOM_NGUOI_DUNG>();
            this.NHOM_NGUOI_DUNG2 = new HashSet<NHOM_NGUOI_DUNG>();
            this.PHAN_QUYEN = new HashSet<PHAN_QUYEN>();
            this.PHAN_QUYEN1 = new HashSet<PHAN_QUYEN>();
            this.SAN_PHAM = new HashSet<SAN_PHAM>();
            this.SAN_PHAM1 = new HashSet<SAN_PHAM>();
            this.TRA_HANG = new HashSet<TRA_HANG>();
            this.TRA_HANG1 = new HashSet<TRA_HANG>();
            this.TRA_HANG2 = new HashSet<TRA_HANG>();
            this.XUAT_KHO = new HashSet<XUAT_KHO>();
            this.XUAT_KHO1 = new HashSet<XUAT_KHO>();
            this.XUAT_KHO2 = new HashSet<XUAT_KHO>();
        }
        [Key]
        public int MA_NGUOI_DUNG { get; set; }
        
        [Required(ErrorMessage="Vui lòng nhập tên của bạn")]
        [Display(Name = "Họ và tên")]
        public string TEN_NGUOI_DUNG { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [Display(Name = "Ngày sinh")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        //[RegularExpression("(0?[1-9]|[12][0-9]|3[01])/(0?[1-9]|1[012])/((19|20)\\d\\d)", ErrorMessage = "Ngày không hợp lệ")]
        public System.DateTime NGAY_SINH { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số CMND")]
        [StringLength(10, ErrorMessage = "Số CMND không hợp lệ", MinimumLength=9)]
        public string SO_CHUNG_MINH { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        [MaxLength(200)]
        public string DIA_CHI { get; set; }

        [Display(Name = "Số điện thoại")]
        public string SO_DIEN_THOAI { get; set; }

        [Display(Name = "Mã kho")]
        public Nullable<int> MA_KHO { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập User Name")]
        [Display(Name = "User Name")]
        [RegularExpression(@"(\S)+", ErrorMessage = "Không được dùng khoảng trắng")]
        [ScaffoldColumn(false)]
        public string USER_NAME { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string MAT_KHAU { get; set; }

        [Display(Name = "Ngày vào làm")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> NGAY_VAO_LAM { get; set; }

        [Display(Name = "Hình ảnh")]
        public byte[] HINH_ANH { get; set; }

        [Display(Name = "Ghi chú")]
        public string GHI_CHU { get; set; }

        [Display(Name = "Mã nhóm")]
        public Nullable<int> MA_NHOM_NGUOI_DUNG { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }

        [Display(Name = "Active")]
        public string ACTIVE { get; set; }
    
        public virtual ICollection<CHI_TIET_HOA_DON> CHI_TIET_HOA_DON { get; set; }
        public virtual ICollection<CHI_TIET_HOA_DON> CHI_TIET_HOA_DON1 { get; set; }
        public virtual ICollection<CHI_TIET_NHAP_KHO> CHI_TIET_NHAP_KHO { get; set; }
        public virtual ICollection<CHI_TIET_NHAP_KHO> CHI_TIET_NHAP_KHO1 { get; set; }
        public virtual ICollection<CHI_TIET_TRA_HANG> CHI_TIET_TRA_HANG { get; set; }
        public virtual ICollection<CHI_TIET_TRA_HANG> CHI_TIET_TRA_HANG1 { get; set; }
        public virtual ICollection<CHI_TIET_XUAT_KHO> CHI_TIET_XUAT_KHO { get; set; }
        public virtual ICollection<CHI_TIET_XUAT_KHO> CHI_TIET_XUAT_KHO1 { get; set; }
        public virtual ICollection<CHUYEN_DOI_DON_VI_TINH> CHUYEN_DOI_DON_VI_TINH { get; set; }
        public virtual ICollection<CHUYEN_DOI_DON_VI_TINH> CHUYEN_DOI_DON_VI_TINH1 { get; set; }
        public virtual ICollection<DON_VI_TINH> DON_VI_TINH { get; set; }
        public virtual ICollection<DON_VI_TINH> DON_VI_TINH1 { get; set; }
        public virtual ICollection<HOA_DON> HOA_DON { get; set; }
        public virtual ICollection<HOA_DON> HOA_DON1 { get; set; }
        public virtual ICollection<HOA_DON> HOA_DON2 { get; set; }
        public virtual ICollection<HOA_DON> HOA_DON3 { get; set; }
        public virtual ICollection<KHACH_HANG> KHACH_HANG { get; set; }
        public virtual ICollection<KHACH_HANG> KHACH_HANG1 { get; set; }
        public virtual ICollection<KHO> KHOes { get; set; }
        public virtual ICollection<KHO> KHOes1 { get; set; }
        public virtual ICollection<KHO> KHOes2 { get; set; }
        public virtual KHO KHO { get; set; }
        public virtual ICollection<KHU_VUC> KHU_VUC { get; set; }
        public virtual ICollection<KHU_VUC> KHU_VUC1 { get; set; }
        public virtual ICollection<KIEM_KHO_HISTORY> KIEM_KHO_HISTORY { get; set; }
        public virtual ICollection<KIEM_KHO_HISTORY> KIEM_KHO_HISTORY1 { get; set; }
        public virtual ICollection<NGUOI_DUNG> NGUOI_DUNG1 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG2 { get; set; }
        public virtual NHOM_NGUOI_DUNG NHOM_NGUOI_DUNG { get; set; }
        public virtual ICollection<NGUOI_DUNG> NGUOI_DUNG11 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG3 { get; set; }
        public virtual ICollection<NHA_CUNG_CAP> NHA_CUNG_CAP { get; set; }
        public virtual ICollection<NHA_CUNG_CAP> NHA_CUNG_CAP1 { get; set; }
        public virtual ICollection<NHA_SAN_XUAT> NHA_SAN_XUAT { get; set; }
        public virtual ICollection<NHA_SAN_XUAT> NHA_SAN_XUAT1 { get; set; }
        public virtual ICollection<NHAP_KHO> NHAP_KHO { get; set; }
        public virtual ICollection<NHAP_KHO> NHAP_KHO1 { get; set; }
        public virtual ICollection<NHAP_KHO> NHAP_KHO2 { get; set; }
        public virtual ICollection<NHOM_NGUOI_DUNG> NHOM_NGUOI_DUNG1 { get; set; }
        public virtual ICollection<NHOM_NGUOI_DUNG> NHOM_NGUOI_DUNG2 { get; set; }
        public virtual ICollection<PHAN_QUYEN> PHAN_QUYEN { get; set; }
        public virtual ICollection<PHAN_QUYEN> PHAN_QUYEN1 { get; set; }
        public virtual ICollection<SAN_PHAM> SAN_PHAM { get; set; }
        public virtual ICollection<SAN_PHAM> SAN_PHAM1 { get; set; }
        public virtual ICollection<TRA_HANG> TRA_HANG { get; set; }
        public virtual ICollection<TRA_HANG> TRA_HANG1 { get; set; }
        public virtual ICollection<TRA_HANG> TRA_HANG2 { get; set; }
        public virtual ICollection<XUAT_KHO> XUAT_KHO { get; set; }
        public virtual ICollection<XUAT_KHO> XUAT_KHO1 { get; set; }
        public virtual ICollection<XUAT_KHO> XUAT_KHO2 { get; set; }
    }
}
