using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Models
{
    public class NhaCungCapModels
    {
        public int MaNhaCungCap { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenNhaCungCap { get; set; }

        [Required]
        [MaxLength(300)]
        public string DiaChi { get; set; }

        [Required]
        [StringLength(15)]
        public string CoDienThoai { get; set; }

        [Required]
        public string Email { get; set; }

        
        [StringLength(1000)]
        public string GhiChu { get; set; }

        public int MaNhanVienTao { get; set; }

        public int MaNhanVienCapNhat { get; set; }

        public DateTime NgayTaoMoi { get; set; }

        public DateTime NgayCapNhat { get; set; }

        [StringLength(1)]
        public string active { get; set; }
    }
}
