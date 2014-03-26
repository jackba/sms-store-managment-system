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
        [Display(Name = "Tên đơn vị:")]
        public string TenDonVi { get; set; }

        [Display(Name = "Ghi chú:")]
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
