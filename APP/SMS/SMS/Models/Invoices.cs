using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class InvoicesModel
    {
        public SP_GET_HOA_DON_INFO_Result Infor { get; set; }
        public List<V_HOA_DON> detailList { get; set; }
    }
}