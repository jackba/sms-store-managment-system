﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class SmsMaster
    {
    }
    public class SmsMasterModel
    {
        [Required]
        [Display(Name = "Tên công ty")]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Số điện thoại")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public string FaxNumber { get; set; }

        [Required]
        public string AdvertisementHeader { get; set; }

        [Required]
        public string AdvertisementFooter { get; set; }

        [Required]
        public string Address { get; set; }
    }
}