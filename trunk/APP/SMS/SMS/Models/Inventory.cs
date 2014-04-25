using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Inventory
    {
        public int MA_SAN_PHAM { get; set; }
        public string TEN_SAN_PHAM { get; set; }
        public string TEN_DON_VI { get; set; }
        public double SO_LUONG_TON { get; set; }
        public double VALUE { get; set; }
    }
    public class InventoryTotal
    {
        public double VALUE { get; set; }
    }
    public class GetInventoryModel
    {
        public IPagedList<Inventory> InventoryList { get; set; }
        public double VALUE { get; set; }
    }
}