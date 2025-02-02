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
    
    

    public class SanPhamDisplay {
        public SAN_PHAM SanPham { get; set; }
        public NGUOI_DUNG NguoiTao { get; set; }
        public NGUOI_DUNG NguoiCapNhat { get; set; }
        public DON_VI_TINH DonVi { get; set; }
        public NHOM_SAN_PHAM NhomSanPham { get; set; }
        public NHA_SAN_XUAT NhaSanXuat { get; set; }
    }
    public class ProductSA
    {
        public string TenSanPham { get; set; }
        public string KichThuoc { get; set; }
        public string TrongLuongFrom { get; set; }
        public string TrongLuongTo { get; set; }
        public string DonViTinh { get; set; }
        public string NhaSanXuat { get; set; }
        public string DacTa { get; set; }
        public string GiaBanFrom { get; set; }
        public string GiaBanTo { get; set; }
        public string ChietKhauFrom { get; set; }
        public string ChietKhauTo { get; set; }
        public string CoSoFrom { get; set; }
        public string CoSoTo { get; set; }
        public string NhomSanPham { get; set; }
    }
    public partial class SAN_PHAM
    {
        public SAN_PHAM()
        {
            this.CHI_TIET_HOA_DON = new HashSet<CHI_TIET_HOA_DON>();
            this.CHI_TIET_NHAP_KHO = new HashSet<CHI_TIET_NHAP_KHO>();
            this.CHI_TIET_TRA_HANG = new HashSet<CHI_TIET_TRA_HANG>();
            this.CHI_TIET_XUAT_KHO = new HashSet<CHI_TIET_XUAT_KHO>();
            this.CHUYEN_DOI_DON_VI_TINH = new HashSet<CHUYEN_DOI_DON_VI_TINH>();
            this.TRA_HANG_NCC_CHI_TIET = new HashSet<TRA_HANG_NCC_CHI_TIET>();
            this.TON_KHO_MIN_MAX_KHO = new HashSet<TON_KHO_MIN_MAX_KHO>();
        }
    
        public int MA_SAN_PHAM { get; set; }
        [Required(ErrorMessage = "Hãy nhập tên sản phẩm.")]
        [Display(Name = "Tên sản phẩm")]
        [StringLength(50)]
        public string TEN_SAN_PHAM { get; set; }
        [Display(Name = "Kích thước")]
        [StringLength(100)]
        public string KICH_THUOC { get; set; }
        

		[Display(Name = "Trọng lượng")]
		public Nullable<double> CAN_NANG { get; set; }
        [Display(Name = "Đặc tả")]
        [StringLength(1000)]
        public string DAC_TA { get; set; }
        [Display(Name = "Đơn vị tính")]
        public Nullable<int> MA_DON_VI { get; set; }
        [Display(Name = "Nhà sản xuất")]
        public Nullable<int> MA_NHA_SAN_XUAT { get; set; }
        [Display(Name = "Hình ảnh")]        
        public byte[] HINH_ANH { get; set; }
        [Display(Name = "Giá bán 1")]
        [DisplayFormat(DataFormatString = "#,###,###", ApplyFormatInEditMode = true)]
        public Nullable<double> GIA_BAN_1 { get; set; }
         [Display(Name = "Giá bán 2")]   
        public Nullable<double> GIA_BAN_2 { get; set; }
        [Display(Name = "Giá bán 3")]  
        public Nullable<double> GIA_BAN_3 { get; set; }
        [Display(Name = "Chiết khấu 1")]  
        public Nullable<double> CHIEC_KHAU_1 { get; set; }
        [Display(Name = "Chiết khấu 2")]  
        public Nullable<double> CHIEC_KHAU_2 { get; set; }
        [Display(Name = "Chiết khấu 3")]  
        public Nullable<double> CHIEC_KHAU_3 { get; set; }
         [Display(Name = "Cơ số tối thiểu")]  
        public Nullable<double> CO_SO_TOI_THIEU { get; set; }
          [Display(Name = "Cơ số tối đa")]  
        public Nullable<double> CO_SO_TOI_DA { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
         [Display(Name = "Code sản phẩm")]  
        public string CODE { get; set; }
          [Display(Name = "Phân nhóm")]
        public Nullable<int> MA_NHOM { get; set; }
    
        public virtual ICollection<CHI_TIET_HOA_DON> CHI_TIET_HOA_DON { get; set; }
        public virtual ICollection<CHI_TIET_NHAP_KHO> CHI_TIET_NHAP_KHO { get; set; }
        public virtual ICollection<CHI_TIET_TRA_HANG> CHI_TIET_TRA_HANG { get; set; }
        public virtual ICollection<CHI_TIET_XUAT_KHO> CHI_TIET_XUAT_KHO { get; set; }
        public virtual ICollection<CHUYEN_DOI_DON_VI_TINH> CHUYEN_DOI_DON_VI_TINH { get; set; }
        public virtual DON_VI_TINH DON_VI_TINH { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual NHA_SAN_XUAT NHA_SAN_XUAT { get; set; }
        public virtual ICollection<TRA_HANG_NCC_CHI_TIET> TRA_HANG_NCC_CHI_TIET { get; set; }
        public virtual NHOM_SAN_PHAM NHOM_SAN_PHAM { get; set; }
        public virtual ICollection<TON_KHO_MIN_MAX_KHO> TON_KHO_MIN_MAX_KHO { get; set; }
    }
}
