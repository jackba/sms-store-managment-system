﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SmsContext : DbContext
    {
        public SmsContext()
            : base("name=SmsContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CHI_TIET_HOA_DON> CHI_TIET_HOA_DON { get; set; }
        public virtual DbSet<CHI_TIET_NHAP_KHO> CHI_TIET_NHAP_KHO { get; set; }
        public virtual DbSet<CHI_TIET_TRA_HANG> CHI_TIET_TRA_HANG { get; set; }
        public virtual DbSet<CHI_TIET_XUAT_KHO> CHI_TIET_XUAT_KHO { get; set; }
        public virtual DbSet<CHUYEN_DOI_DON_VI_TINH> CHUYEN_DOI_DON_VI_TINH { get; set; }
        public virtual DbSet<CONTROLLER_PERMISSION> CONTROLLER_PERMISSION { get; set; }
        public virtual DbSet<DON_VI_TINH> DON_VI_TINH { get; set; }
        public virtual DbSet<HOA_DON> HOA_DON { get; set; }
        public virtual DbSet<KHACH_HANG> KHACH_HANG { get; set; }
        public virtual DbSet<KHACH_HANG_DEBIT_HIST> KHACH_HANG_DEBIT_HIST { get; set; }
        public virtual DbSet<KHO> KHOes { get; set; }
        public virtual DbSet<KHU_VUC> KHU_VUC { get; set; }
        public virtual DbSet<KIEM_KHO_HISTORY> KIEM_KHO_HISTORY { get; set; }
        public virtual DbSet<NGUOI_DUNG> NGUOI_DUNG { get; set; }
        public virtual DbSet<NHA_CUNG_CAP> NHA_CUNG_CAP { get; set; }
        public virtual DbSet<NHA_SAN_XUAT> NHA_SAN_XUAT { get; set; }
        public virtual DbSet<NHAP_KHO> NHAP_KHO { get; set; }
        public virtual DbSet<NHOM_NGUOI_DUNG> NHOM_NGUOI_DUNG { get; set; }
        public virtual DbSet<PHAN_QUYEN> PHAN_QUYEN { get; set; }
        public virtual DbSet<SAN_PHAM> SAN_PHAM { get; set; }
        public virtual DbSet<SMS_MASTER> SMS_MASTER { get; set; }
        public virtual DbSet<TRA_HANG> TRA_HANG { get; set; }
        public virtual DbSet<TRA_HANG_NCC> TRA_HANG_NCC { get; set; }
        public virtual DbSet<TRA_HANG_NCC_CHI_TIET> TRA_HANG_NCC_CHI_TIET { get; set; }
        public virtual DbSet<XUAT_KHO> XUAT_KHO { get; set; }
        public virtual DbSet<V_HOA_DON> V_HOA_DON { get; set; }
        public virtual DbSet<V_IMPORT_DETAIL> V_IMPORT_DETAIL { get; set; }
        public virtual DbSet<V_NHAP_KHO> V_NHAP_KHO { get; set; }
        public virtual DbSet<V_NHAP_XUAT_DETAIL> V_NHAP_XUAT_DETAIL { get; set; }
        public virtual DbSet<V_NHAP_XUAT_KHO> V_NHAP_XUAT_KHO { get; set; }
        public virtual DbSet<V_XUAT_KHO> V_XUAT_KHO { get; set; }
    
        public virtual ObjectResult<GET_HOA_DON_Result> GET_HOA_DON(Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE, Nullable<int> mA_KHACH_HANG)
        {
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            var mA_KHACH_HANGParameter = mA_KHACH_HANG.HasValue ?
                new ObjectParameter("MA_KHACH_HANG", mA_KHACH_HANG) :
                new ObjectParameter("MA_KHACH_HANG", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GET_HOA_DON_Result>("GET_HOA_DON", fROM_DATEParameter, tO_DATEParameter, mA_KHACH_HANGParameter);
        }
    
        public virtual int GET_KHACH_HANG_ALERT(string nAME)
        {
            var nAMEParameter = nAME != null ?
                new ObjectParameter("NAME", nAME) :
                new ObjectParameter("NAME", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_KHACH_HANG_ALERT", nAMEParameter);
        }
    
        public virtual ObjectResult<GET_SUM_HOA_DON_BY_CUS_ID_Result> GET_SUM_HOA_DON_BY_CUS_ID(Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE, Nullable<int> mA_KHACH_HANG)
        {
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            var mA_KHACH_HANGParameter = mA_KHACH_HANG.HasValue ?
                new ObjectParameter("MA_KHACH_HANG", mA_KHACH_HANG) :
                new ObjectParameter("MA_KHACH_HANG", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GET_SUM_HOA_DON_BY_CUS_ID_Result>("GET_SUM_HOA_DON_BY_CUS_ID", fROM_DATEParameter, tO_DATEParameter, mA_KHACH_HANGParameter);
        }
    
        public virtual ObjectResult<SP_EXPORT_REPORT_Result> SP_EXPORT_REPORT(Nullable<int> kIND, Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, string tEN_SAN_PHAM, Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE)
        {
            var kINDParameter = kIND.HasValue ?
                new ObjectParameter("KIND", kIND) :
                new ObjectParameter("KIND", typeof(int));
    
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            var tEN_SAN_PHAMParameter = tEN_SAN_PHAM != null ?
                new ObjectParameter("TEN_SAN_PHAM", tEN_SAN_PHAM) :
                new ObjectParameter("TEN_SAN_PHAM", typeof(string));
    
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_EXPORT_REPORT_Result>("SP_EXPORT_REPORT", kINDParameter, mA_KHOParameter, mA_SAN_PHAMParameter, tEN_SAN_PHAMParameter, fROM_DATEParameter, tO_DATEParameter);
        }
    
        public virtual ObjectResult<SP_EXPORT_REPORT_DETAIL_Result> SP_EXPORT_REPORT_DETAIL(Nullable<int> kIND, Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, string tEN_SAN_PHAM, Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE)
        {
            var kINDParameter = kIND.HasValue ?
                new ObjectParameter("KIND", kIND) :
                new ObjectParameter("KIND", typeof(int));
    
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            var tEN_SAN_PHAMParameter = tEN_SAN_PHAM != null ?
                new ObjectParameter("TEN_SAN_PHAM", tEN_SAN_PHAM) :
                new ObjectParameter("TEN_SAN_PHAM", typeof(string));
    
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_EXPORT_REPORT_DETAIL_Result>("SP_EXPORT_REPORT_DETAIL", kINDParameter, mA_KHOParameter, mA_SAN_PHAMParameter, tEN_SAN_PHAMParameter, fROM_DATEParameter, tO_DATEParameter);
        }
    
        public virtual ObjectResult<Nullable<double>> SP_EXPORT_REPORT_SUM(Nullable<int> kIND, Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, string tEN_SAN_PHAM, Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE)
        {
            var kINDParameter = kIND.HasValue ?
                new ObjectParameter("KIND", kIND) :
                new ObjectParameter("KIND", typeof(int));
    
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            var tEN_SAN_PHAMParameter = tEN_SAN_PHAM != null ?
                new ObjectParameter("TEN_SAN_PHAM", tEN_SAN_PHAM) :
                new ObjectParameter("TEN_SAN_PHAM", typeof(string));
    
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<double>>("SP_EXPORT_REPORT_SUM", kINDParameter, mA_KHOParameter, mA_SAN_PHAMParameter, tEN_SAN_PHAMParameter, fROM_DATEParameter, tO_DATEParameter);
        }
    
        public virtual ObjectResult<SP_GET_ALL_ROLE_Result> SP_GET_ALL_ROLE()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_ALL_ROLE_Result>("SP_GET_ALL_ROLE");
        }
    
        public virtual ObjectResult<SP_GET_DON_VI_TINH_Result> SP_GET_DON_VI_TINH(Nullable<int> mA_SAN_PHAM)
        {
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_DON_VI_TINH_Result>("SP_GET_DON_VI_TINH", mA_SAN_PHAMParameter);
        }
    
        public virtual ObjectResult<SP_GET_HOA_DON_Result> SP_GET_HOA_DON(Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE, Nullable<int> mA_NHAN_VIEN_BAN_HANG, Nullable<int> mA_NV_THU_TIEN)
        {
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            var mA_NHAN_VIEN_BAN_HANGParameter = mA_NHAN_VIEN_BAN_HANG.HasValue ?
                new ObjectParameter("MA_NHAN_VIEN_BAN_HANG", mA_NHAN_VIEN_BAN_HANG) :
                new ObjectParameter("MA_NHAN_VIEN_BAN_HANG", typeof(int));
    
            var mA_NV_THU_TIENParameter = mA_NV_THU_TIEN.HasValue ?
                new ObjectParameter("MA_NV_THU_TIEN", mA_NV_THU_TIEN) :
                new ObjectParameter("MA_NV_THU_TIEN", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_HOA_DON_Result>("SP_GET_HOA_DON", fROM_DATEParameter, tO_DATEParameter, mA_NHAN_VIEN_BAN_HANGParameter, mA_NV_THU_TIENParameter);
        }
    
        public virtual ObjectResult<SP_GET_IMPORT_Result> SP_GET_IMPORT(Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE, Nullable<int> mA_NHAN_VIEN_NHAP, Nullable<int> lY_DO_NHAP, Nullable<int> mA_KHO, Nullable<int> mA_NHA_CC)
        {
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            var mA_NHAN_VIEN_NHAPParameter = mA_NHAN_VIEN_NHAP.HasValue ?
                new ObjectParameter("MA_NHAN_VIEN_NHAP", mA_NHAN_VIEN_NHAP) :
                new ObjectParameter("MA_NHAN_VIEN_NHAP", typeof(int));
    
            var lY_DO_NHAPParameter = lY_DO_NHAP.HasValue ?
                new ObjectParameter("LY_DO_NHAP", lY_DO_NHAP) :
                new ObjectParameter("LY_DO_NHAP", typeof(int));
    
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_NHA_CCParameter = mA_NHA_CC.HasValue ?
                new ObjectParameter("MA_NHA_CC", mA_NHA_CC) :
                new ObjectParameter("MA_NHA_CC", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_IMPORT_Result>("SP_GET_IMPORT", fROM_DATEParameter, tO_DATEParameter, mA_NHAN_VIEN_NHAPParameter, lY_DO_NHAPParameter, mA_KHOParameter, mA_NHA_CCParameter);
        }
    
        public virtual ObjectResult<SP_GET_IMPORT_DETAIL_BY_ID_Result> SP_GET_IMPORT_DETAIL_BY_ID(Nullable<int> iMPORT_ID)
        {
            var iMPORT_IDParameter = iMPORT_ID.HasValue ?
                new ObjectParameter("IMPORT_ID", iMPORT_ID) :
                new ObjectParameter("IMPORT_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_IMPORT_DETAIL_BY_ID_Result>("SP_GET_IMPORT_DETAIL_BY_ID", iMPORT_IDParameter);
        }
    
        public virtual ObjectResult<SP_GET_INVENTORY_Result> SP_GET_INVENTORY(Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, string tEN_SAN_PHAM)
        {
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            var tEN_SAN_PHAMParameter = tEN_SAN_PHAM != null ?
                new ObjectParameter("TEN_SAN_PHAM", tEN_SAN_PHAM) :
                new ObjectParameter("TEN_SAN_PHAM", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_INVENTORY_Result>("SP_GET_INVENTORY", mA_KHOParameter, mA_SAN_PHAMParameter, tEN_SAN_PHAMParameter);
        }
    
        public virtual ObjectResult<SP_GET_INVENTORY_BY_STORE_ID_Result> SP_GET_INVENTORY_BY_STORE_ID(Nullable<int> mA_KHO)
        {
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_INVENTORY_BY_STORE_ID_Result>("SP_GET_INVENTORY_BY_STORE_ID", mA_KHOParameter);
        }
    
        public virtual ObjectResult<SP_GET_NHAP_XUAT_Result> SP_GET_NHAP_XUAT(Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, string tEN_SAN_PHAM, Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE)
        {
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            var tEN_SAN_PHAMParameter = tEN_SAN_PHAM != null ?
                new ObjectParameter("TEN_SAN_PHAM", tEN_SAN_PHAM) :
                new ObjectParameter("TEN_SAN_PHAM", typeof(string));
    
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_NHAP_XUAT_Result>("SP_GET_NHAP_XUAT", mA_KHOParameter, mA_SAN_PHAMParameter, tEN_SAN_PHAMParameter, fROM_DATEParameter, tO_DATEParameter);
        }
    
        public virtual ObjectResult<SP_GET_ROLE_OF_USER_Result> SP_GET_ROLE_OF_USER(Nullable<int> uSER_ID)
        {
            var uSER_IDParameter = uSER_ID.HasValue ?
                new ObjectParameter("USER_ID", uSER_ID) :
                new ObjectParameter("USER_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_ROLE_OF_USER_Result>("SP_GET_ROLE_OF_USER", uSER_IDParameter);
        }
    
        public virtual int SP_GET_TON_KHO()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GET_TON_KHO");
        }
    
        public virtual int SP_GET_TON_KHO_ALERT(string nAME)
        {
            var nAMEParameter = nAME != null ?
                new ObjectParameter("NAME", nAME) :
                new ObjectParameter("NAME", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GET_TON_KHO_ALERT", nAMEParameter);
        }
    
        public virtual int SP_GET_TON_KHO_ALL(string nAME)
        {
            var nAMEParameter = nAME != null ?
                new ObjectParameter("NAME", nAME) :
                new ObjectParameter("NAME", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GET_TON_KHO_ALL", nAMEParameter);
        }
    
        public virtual int SP_GET_TON_KHO_BY_ID(Nullable<int> iNPUT_ID)
        {
            var iNPUT_IDParameter = iNPUT_ID.HasValue ?
                new ObjectParameter("INPUT_ID", iNPUT_ID) :
                new ObjectParameter("INPUT_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GET_TON_KHO_BY_ID", iNPUT_IDParameter);
        }
    
        public virtual ObjectResult<Nullable<double>> SP_GET_VALUE_OF_INVENTORY(Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, string tEN_SAN_PHAM)
        {
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            var tEN_SAN_PHAMParameter = tEN_SAN_PHAM != null ?
                new ObjectParameter("TEN_SAN_PHAM", tEN_SAN_PHAM) :
                new ObjectParameter("TEN_SAN_PHAM", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<double>>("SP_GET_VALUE_OF_INVENTORY", mA_KHOParameter, mA_SAN_PHAMParameter, tEN_SAN_PHAMParameter);
        }
    
        public virtual ObjectResult<SP_IMPORT_REPORTER_Result> SP_IMPORT_REPORTER(Nullable<int> kIND, Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, string tEN_SAN_PHAM, Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE)
        {
            var kINDParameter = kIND.HasValue ?
                new ObjectParameter("KIND", kIND) :
                new ObjectParameter("KIND", typeof(int));
    
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            var tEN_SAN_PHAMParameter = tEN_SAN_PHAM != null ?
                new ObjectParameter("TEN_SAN_PHAM", tEN_SAN_PHAM) :
                new ObjectParameter("TEN_SAN_PHAM", typeof(string));
    
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_IMPORT_REPORTER_Result>("SP_IMPORT_REPORTER", kINDParameter, mA_KHOParameter, mA_SAN_PHAMParameter, tEN_SAN_PHAMParameter, fROM_DATEParameter, tO_DATEParameter);
        }
    
        public virtual ObjectResult<SP_IMPORT_REPORTER_DETAIL_Result> SP_IMPORT_REPORTER_DETAIL(Nullable<int> kIND, Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, string tEN_SAN_PHAM, Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE)
        {
            var kINDParameter = kIND.HasValue ?
                new ObjectParameter("KIND", kIND) :
                new ObjectParameter("KIND", typeof(int));
    
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            var tEN_SAN_PHAMParameter = tEN_SAN_PHAM != null ?
                new ObjectParameter("TEN_SAN_PHAM", tEN_SAN_PHAM) :
                new ObjectParameter("TEN_SAN_PHAM", typeof(string));
    
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_IMPORT_REPORTER_DETAIL_Result>("SP_IMPORT_REPORTER_DETAIL", kINDParameter, mA_KHOParameter, mA_SAN_PHAMParameter, tEN_SAN_PHAMParameter, fROM_DATEParameter, tO_DATEParameter);
        }
    
        public virtual ObjectResult<Nullable<double>> SP_IMPORT_REPORTER_SUM(Nullable<int> kIND, Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, string tEN_SAN_PHAM, Nullable<System.DateTime> fROM_DATE, Nullable<System.DateTime> tO_DATE)
        {
            var kINDParameter = kIND.HasValue ?
                new ObjectParameter("KIND", kIND) :
                new ObjectParameter("KIND", typeof(int));
    
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            var tEN_SAN_PHAMParameter = tEN_SAN_PHAM != null ?
                new ObjectParameter("TEN_SAN_PHAM", tEN_SAN_PHAM) :
                new ObjectParameter("TEN_SAN_PHAM", typeof(string));
    
            var fROM_DATEParameter = fROM_DATE.HasValue ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(System.DateTime));
    
            var tO_DATEParameter = tO_DATE.HasValue ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<double>>("SP_IMPORT_REPORTER_SUM", kINDParameter, mA_KHOParameter, mA_SAN_PHAMParameter, tEN_SAN_PHAMParameter, fROM_DATEParameter, tO_DATEParameter);
        }
    
        public virtual int STMA_GET_GIA_TRI_HANG_BAN_TON(Nullable<int> mA_KHO, Nullable<int> mA_SAN_PHAM, ObjectParameter gIA_VON_HANG_BAN_TOTAL, ObjectParameter gIA_TRI_HANG_TON_TOTAL)
        {
            var mA_KHOParameter = mA_KHO.HasValue ?
                new ObjectParameter("MA_KHO", mA_KHO) :
                new ObjectParameter("MA_KHO", typeof(int));
    
            var mA_SAN_PHAMParameter = mA_SAN_PHAM.HasValue ?
                new ObjectParameter("MA_SAN_PHAM", mA_SAN_PHAM) :
                new ObjectParameter("MA_SAN_PHAM", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("STMA_GET_GIA_TRI_HANG_BAN_TON", mA_KHOParameter, mA_SAN_PHAMParameter, gIA_VON_HANG_BAN_TOTAL, gIA_TRI_HANG_TON_TOTAL);
        }
    }
}
