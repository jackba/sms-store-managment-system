using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Models
{
    public class DonViModels
    {
        public int MadonVi { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDonVi { get; set; }

         [StringLength(1000)]
        public string GhiChu { get; set; }

        public int MaNhanVienTao { get; set; }

        public int MaNhanVienCapNhat { get; set; }

        public DateTime NgayTaoMoi { get; set; }

        public DateTime NgayCapNhat { get; set; }
    }
}
