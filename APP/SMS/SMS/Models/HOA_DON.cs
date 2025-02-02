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
    public class HoaDonBanHang
    {
        public KhachHangInfo KH_Info { get; set; }
        public List<KHO> Store { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<ChiTiet_HoaDon> lstChiTietHoaDon { get; set; }
        public string Message { get; set; }
        public string InforMessage { get; set; }

        public string printFlg { get; set; }
    }

    public class EditHoaDonBanHang
    {
        public SP_GET_BILL_INFOR_Result Infor { get; set; }
        public List<KHO> Store { get; set; }
        public List<DON_VI_TINH> Units { get; set; }
        public List<SP_GET_BILL_DETAIL_BY_ID_Result> Details { get; set; }
        public string Message { get; set; }
        public string InforMessage { get; set; }
    }

    public class KhachHangInfo{
        public int Ma_HD { get; set; }
        public int? Ma_KH {get;set;}
        public string Ten_KH {get;set;}
        public DateTime? Ngay_Ban {get;set;}
        public DateTime? Ngay_Giao {get;set;}
        public string Dien_Thoai { get; set; }
        public string Dia_Chi {get;set;}
    }
    public class ChiTiet_HoaDon{
        public string Code_SP { get; set; } 
        public int? Ma_SP {get;set;}
        public string Ten_SP { get; set; } 
        public double? Gia_Ban { get; set; }
        public double? Thanh_Tien { get; set; }
        public double? Phan_Tram_CK{get;set;}
        public double? Gia_Thuc{get;set;}
        public double? So_Luong{get;set;}
        public int? Ma_Kho_Xuat{get;set;}
        public string Ten_Kho_Xuat { get; set; }
        public int Ma_Don_Vi{ get; set; }
        public string Ten_Don_Vi { get; set; }
        public double? HE_SO { get; set; }
        public int? DEL_FLG { get; set; }
    }
    public partial class HOA_DON
    {
        public HOA_DON()
        {
            this.CHI_TIET_HOA_DON = new HashSet<CHI_TIET_HOA_DON>();
            this.KHACH_HANG_DEBIT_HIST = new HashSet<KHACH_HANG_DEBIT_HIST>();
            this.TRA_HANG = new HashSet<TRA_HANG>();
            this.XUAT_KHO = new HashSet<XUAT_KHO>();
        }
    
        public int MA_HOA_DON { get; set; }
        public string SO_HOA_DON { get; set; }
        public Nullable<int> MA_KHACH_HANG { get; set; }
        public string TEN_KHACH_HANG { get; set; }
        public Nullable<int> MA_NHAN_VIEN_BAN { get; set; }
        public Nullable<System.DateTime> NGAY_BAN { get; set; }
        public Nullable<System.DateTime> NGAY_GIAO { get; set; }
        public string DIA_CHI_GIAO_HANG { get; set; }
        public Nullable<short> STATUS { get; set; }
        public Nullable<int> EDIT_APPROVER { get; set; }
        public Nullable<int> CREATE_BY { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_AT { get; set; }
        public Nullable<System.DateTime> UPDATE_AT { get; set; }
        public string ACTIVE { get; set; }
        public Nullable<double> SO_TIEN_KHACH_TRA { get; set; }
        public Nullable<double> SO_TIEN_NO_GOI_DAU { get; set; }
        public Nullable<int> MA_NHAN_VIEN_THU_TIEN { get; set; }
        public string SO_DIEN_THOAI { get; set; }
    
        public virtual ICollection<CHI_TIET_HOA_DON> CHI_TIET_HOA_DON { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG1 { get; set; }
        public virtual KHACH_HANG KHACH_HANG { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG2 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG3 { get; set; }
        public virtual NGUOI_DUNG NGUOI_DUNG4 { get; set; }
        public virtual ICollection<KHACH_HANG_DEBIT_HIST> KHACH_HANG_DEBIT_HIST { get; set; }
        public virtual ICollection<TRA_HANG> TRA_HANG { get; set; }
        public virtual ICollection<XUAT_KHO> XUAT_KHO { get; set; }
    }
}
