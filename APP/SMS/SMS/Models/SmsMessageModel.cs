using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace SMS.Models
{
    public class SmsMessageModel
    {
        public IPagedList<SP_GET_SMS_MESSAGES_Result> MessageList { get; set; }
        public int Count { get; set; }
    }
}