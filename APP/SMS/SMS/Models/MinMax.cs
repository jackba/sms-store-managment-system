using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace SMS.Models
{
    public class MinMax
    {
        public List<KHO> Stores { get; set; }
        public List<NHOM_SAN_PHAM> ProductGroups { get; set; }
        public IPagedList<SP_GET_MIN_MAX_BY_STORE_Result> MinMaxList { get; set; } 
    }
}