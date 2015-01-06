using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class SmsMaster
    {
    }

    public class StoreAndUserList
    {
        public List<KHO> Stores { get; set; }
        public List<StoreUser> StoresUser { get; set; }
    }

    public class StoreUser
    {
        public int ID { get; set; }
        public int USR_ID { get; set; }
        public string USR_NAME { get; set; }
        public int KHO_ID_1 { get; set; }
        public int KHO_ID_2 { get; set; }
        public int KHO_ID_3 { get; set; }
        public int KHO_ID_4 { get; set; }
        public int KHO_ID_5 { get; set; }
        public int KHO_ID_6 { get; set; }
        public int KHO_ID_7 { get; set; }
        public int KHO_ID_8 { get; set; }
        public int KHO_ID_9 { get; set; }
    }

    public class SmsMasterModel
    {
        [Required]
        [Display(Name = "Tên công ty")]
        [StringLength(1000)]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Số điện thoại")]
        [StringLength(200)]
        public string PhoneNumber { get; set; }

        [Required]
        public string FaxNumber { get; set; }

        [Required]
        public string AdvertisementHeader { get; set; }

        [Required]
        public string AdvertisementFooter { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string EmailUserName { get; set; }

        [Required]
        public string EmailPassword { get; set; }
    }
}