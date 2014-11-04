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
        static IDictionary<string, int> Slugs = new Dictionary<string, int>
          {
            {"this-is-a-slug", 100}, 
            {"another-slug", 101}, 
            {"and-another", 102}
          };

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                string currentController = filterContext.HttpContext.Request.RequestContext.RouteData.GetRequiredString("controller");
                string action = filterContext.HttpContext.Request.RequestContext.RouteData.GetRequiredString("action");
                var ctx = new SmsContext();
                bool IsMetadataManager = filterContext.HttpContext.Session["IsMetadataManager"] == null ? false :(bool)filterContext.HttpContext.Session["IsMetadataManager"];
                bool IsSaler = filterContext.HttpContext.Session["IsSaler"] == null ? false : (bool)filterContext.HttpContext.Session["IsSaler"];
                bool IsStoreManager =  filterContext.HttpContext.Session["IsStoreManager"] == null ? false :(bool)filterContext.HttpContext.Session["IsStoreManager"];
                bool IsAdmin = filterContext.HttpContext.Session["IsAdmin"] == null ? false : (bool)filterContext.HttpContext.Session["IsAdmin"];
                bool IsAccounting = filterContext.HttpContext.Session["IsAccounting"] == null ? false : (bool)filterContext.HttpContext.Session["IsAccounting"];
                if (filterContext.HttpContext.Session["UserId"] == null ||
                    Convert.ToInt32(filterContext.HttpContext.Session["UserId"]) <= 0)
                {
                    filterContext.HttpContext.Session.Abandon();
                    filterContext.HttpContext.Session.Clear();
                    filterContext.HttpContext.Session.RemoveAll();
                    FormsAuthentication.SignOut();
                    filterContext.HttpContext.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(-1);
                    throw new NotImplementedException("Bạn không có quyền thực thi tác vụ này. Login đã timeout, vui lòng đăng nhập lại.");
                }
                if (!IsAdmin)
                {
                    var permission = ctx.CONTROLLER_PERMISSION.FirstOrDefault(u => u.CONTROLLER_NAME == currentController
                        && u.ACTION_NAME == action
                        && u.ACTIVE == "A"
                        && (
                               (IsAdmin && u.IS_ADMIN == IsAdmin) ||
                               (IsAccounting && u.IS_ACCOUNTING == IsAccounting) ||
                               (IsStoreManager && u.IS_STORE_MANAGER == IsStoreManager) ||
                               (IsSaler && u.IS_ACCOUNTING == IsSaler) ||
                               (IsMetadataManager && u.IS_METEDATA_MANAGER == IsMetadataManager)
                            )
                        );
                    try
                    {
                        if (permission == null)
                        {
                            filterContext.HttpContext.Session.Abandon();
                            filterContext.HttpContext.Session.Clear();
                            filterContext.HttpContext.Session.RemoveAll();
                            FormsAuthentication.SignOut();
                            filterContext.HttpContext.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(-1);
                            throw new NotImplementedException("Bạn không có quyền thực thi tác vụ này. Vui lòng liên hệ admin để biết thêm phân quyền của bạn.");
                        }
                        else
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }
                }
                ctx.Dispose();
            }
        }

    }
}