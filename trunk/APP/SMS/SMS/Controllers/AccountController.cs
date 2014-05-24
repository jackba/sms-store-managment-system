using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Web.Security;
using System.Data.Entity.Validation;
using SMS.App_Start;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    [CustomActionFilter]
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogIn()
        {
            if (Session["UserId"] == null)
            {
                Session.Abandon();
                Session.Clear();
                Session.RemoveAll();
                FormsAuthentication.SignOut();

                Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(-1);
                return View();
            }
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
                return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogIn(Models.NGUOI_DUNG userr)
        {
            if (IsValid(userr.USER_NAME, userr.MAT_KHAU))
            {
                FormsAuthentication.SetAuthCookie(userr.USER_NAME, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Nhập sai user name hoặc mật khẩu");
            }
            return View(userr);
        }

        [HttpPost]
        public void LogOff()
        {

            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            FormsAuthentication.SignOut();

            Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(-1);
            FormsAuthentication.RedirectToLoginPage();
        }
        [HttpGet]
        public ActionResult Show()
        {
            int id = (int)Session["UserId"];
            var ctx = new SmsContext();
            var User = ctx.NGUOI_DUNG.Include("KHO").Include("NHOM_NGUOI_DUNG").Single(us => us.MA_NGUOI_DUNG == id);
            return View(User);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(Models.NGUOI_DUNG user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var ctx = new SmsContext())
                    {
                        var newUser = ctx.NGUOI_DUNG.Create();
                        newUser.TEN_NGUOI_DUNG = user.TEN_NGUOI_DUNG;
                        newUser.NGAY_SINH = user.NGAY_SINH;
                        newUser.SO_CHUNG_MINH = user.SO_CHUNG_MINH;
                        newUser.DIA_CHI = user.DIA_CHI;

                        newUser.USER_NAME = user.USER_NAME;

                        newUser.MAT_KHAU = user.MAT_KHAU;

                        ctx.NGUOI_DUNG.Add(newUser);

                        ctx.SaveChanges();

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",

                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (var ve in eve.ValidationErrors)
                    {

                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",

                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return View();
        }

        private bool IsValid(string username, string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            bool IsValid = false;
            using (var ctx = new SmsContext())
            {
                var user = ctx.NGUOI_DUNG.FirstOrDefault(u => u.USER_NAME == username && u.ACTIVE == "A");
                if (user != null)
                {
                    if (user.MAT_KHAU == crypto.Compute(password, user.SALT))
                    {
                        IsValid = true;
                        var usRole = ctx.SP_GET_ROLE_OF_USER(Convert.ToInt32(user.MA_NGUOI_DUNG)).FirstOrDefault<SP_GET_ROLE_OF_USER_Result>();
                        if (usRole != null)
                        {
                            Session["UserId"] = (int)user.MA_NGUOI_DUNG;
                            Session["GroupUserId"] = (int)user.MA_NHOM_NGUOI_DUNG;
                            Session["UserName"] = user.TEN_NGUOI_DUNG;
                            Session["IsAdmin"] = usRole.IS_ADMIN != null ? (bool)usRole.IS_ADMIN : false;
                            Session["IsAccounting"] = usRole.IS_ACCOUNTING != null ? (bool)usRole.IS_ACCOUNTING : false;
                            Session["IsSaler"] = usRole.IS_SALER != null ? (bool)usRole.IS_SALER : false;
                            Session["IsMetadataManager"] = usRole.IS_METADATA_MANAGER != null ? (bool)usRole.IS_METADATA_MANAGER : false;
                            Session["IsStoreManager"] = usRole.IS_STORE_MANAGER != null ? (bool)usRole.IS_STORE_MANAGER : false;
                            Session["MyStore"] = user.MA_KHO;
                        }
                    }
                }
            }

            return IsValid;
        }

        [HttpPost]
        public String changePassword(String uname, String oldpass, String newpass, String confirmpass)
        {
            String msg = null;
            try
            {
                using (var ctx = new SmsContext())
                {
                    NGUOI_DUNG user = ctx.NGUOI_DUNG.FirstOrDefault(u => u.USER_NAME == uname);
                    if (user != null)
                    {
                        //Check old password
                        if (oldpass.Equals(user.MAT_KHAU))
                        {
                            if (!String.IsNullOrEmpty(newpass))
                            {
                                if (!String.IsNullOrEmpty(confirmpass))
                                {
                                    //Check new & confirm password
                                    if (newpass.Equals(confirmpass))
                                    {
                                        user.MAT_KHAU = newpass;
                                        ctx.SaveChanges();
                                        msg = "3";
                                    }
                                    else
                                    {
                                        msg = "2";//confirmp's not same
                                    }
                                }
                                else
                                {
                                    msg = "6";//confirmp's empty
                                }
                            }
                            else
                            {
                                msg = "5";//newp's empty
                            }
                        }
                        else
                        {
                            msg = "1";//oldp's incorrect
                        }
                    }
                    else
                    {
                        msg = "4";//username's invalid
                    }
                }
            }
            catch (Exception e)
            {
                msg = "99";//other errors
            }
            return msg;

        }
    }
}
