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
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TRA_HANG> TRA_HANG { get; set; }
        public virtual DbSet<XUAT_KHO> XUAT_KHO { get; set; }
    }
}