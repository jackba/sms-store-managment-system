using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using SMS.Models;
using System.Web.Security;
using System.Data.Entity.Validation;

namespace SMS.Controllers
{
    [Authorize]
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
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            else
                return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogIn(Models.NGUOI_DUNG userr)
        {
            //if (ModelState.IsValid)

            if (IsValid(userr.USER_NAME, userr.MAT_KHAU))
            {

                FormsAuthentication.SetAuthCookie(userr.USER_NAME, false);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Login details are wrong.");
            }
            return View(userr);
        }

        [HttpPost]
        public void LogOff()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            
            //return RedirectToAction("Index", "Home");
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
                        //var db = ctx.NGUOI_DUNG;
                        //var crypto = new SimpleCrypto.PBKDF2();
                        //var encrypPass = crypto.Compute(user.Password);
                        var newUser = ctx.NGUOI_DUNG.Create();

                        //TODO: fill all properties of NGUOI_DUNG
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
            //var crypto = new SimpleCrypto.PBKDF2();

            bool IsValid = false;
            using (var ctx = new SmsContext())
            {
                var user = ctx.NGUOI_DUNG.FirstOrDefault(u => u.USER_NAME == username);

                if (user != null)
                {
                    //if (user.MAT_KHAU == crypto.Compute(password, user.PasswordSalt))
                    if (user.MAT_KHAU.Equals(password))
                    {
                        IsValid = true;
                    }
                }
            }

            return IsValid;
        } 
    }
}
