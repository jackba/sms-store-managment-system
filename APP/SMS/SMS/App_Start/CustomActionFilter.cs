using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Web.Security;

namespace SMS.App_Start
{
    public class CustomActionFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                string currentController = filterContext.HttpContext.Request.RequestContext.RouteData.GetRequiredString("controller");
                string action = filterContext.HttpContext.Request.RequestContext.RouteData.GetRequiredString("action");
                var ctx = new SmsContext();
                int level = 0;
                bool IsMetadataManager = (bool)filterContext.HttpContext.Session["IsMetadataManager"];
                bool IsSaler = (bool)filterContext.HttpContext.Session["IsSaler"];
                bool IsStoreManager = (bool)filterContext.HttpContext.Session["IsStoreManager"];
                bool IsAdmin = (bool)filterContext.HttpContext.Session["IsAdmin"];
                bool IsAccounting = (bool)filterContext.HttpContext.Session["IsAccounting"];
                if (IsAdmin)
                {
                    level = 31;
                }
                else if (IsAccounting)
                {
                    level = 15;
                }
                else if (IsStoreManager)
                {
                    level = 7;
                }
                else if (IsSaler)
                {
                    level = 3;
                }else if(IsMetadataManager)
                {
                    level = 1;
                }
                var permission = ctx.CONTROLLER_PERMISSION.FirstOrDefault(u => u.CONTROLLER_NAME == currentController && u.ACTION_NAME == action && u.ACTIVE == "A");
                if (permission != null && (int)permission.PERMISSION_LEVEL <= level)
                {

                }else
                {
                    throw new NotImplementedException("Bạn không có quyền thực thi tác vụ này");
                }                
            }
        }

    }
}