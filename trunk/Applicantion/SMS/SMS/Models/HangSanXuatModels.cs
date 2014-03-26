using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Models
{
    public class HangSanXuatModels
    {
        public int MaHangSanXuat { get; set; }

        [Required]
        [StringLength(100)]
        public string TenHangSanXuat { get; set; }

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
