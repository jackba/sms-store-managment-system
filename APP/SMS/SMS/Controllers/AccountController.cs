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
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetPassword()
        {
            SecurityQuestion model = new SecurityQuestion();
            var ctx = new SmsContext();
            var questions = ctx.SECURITY_QUESTIONS.Where(u => u.ACTIVE == "A").ToList<SECURITY_QUESTIONS>();
            model.Questions = questions;
            model.QuestionId1 = 0;
            model.QuestionId2 = 0;
            model.QuestionId3 = 0;
            return View(model);
        }

        
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetPassword(SecurityQuestion model)
        {
            if (ModelState.IsValid)
            {
                var ctx = new SmsContext();
                var questions = ctx.SECURITY_QUESTIONS.Where(u => u.ACTIVE == "A").ToList<SECURITY_QUESTIONS>();
                model.Questions = questions;
                var crypto = new SimpleCrypto.PBKDF2();
                var usr = ctx.NGUOI_DUNG.Where(u => u.ACTIVE == "A" && u.USER_NAME.ToLower() == model.userName.ToLower()).FirstOrDefault();
                if (usr != null)
                {
                    if (string.IsNullOrEmpty(usr.EMAIL))
                    {
                        ViewBag.Message = "Account của bạn không có email để nhận mật khẩu mới. Vui lòng liên hệ admin để reset lại mật khẩu cho bạn.";
                        return View(model);
                    }
                    var question1 = ctx.PERSONAL_QUESTIONS.Where(u => u.QUESTION_ID == model.QuestionId1 && u.USR_ID == usr.MA_NGUOI_DUNG).FirstOrDefault();
                    if (question1 != null)
                    {
                        if (!(question1.ANSWER == crypto.Compute(model.Answer1, question1.ANSWER_SALT)))
                        {
                            model.QuestionId1 = 0;
                            model.QuestionId2 = 0;
                            model.QuestionId3 = 0;
                            ViewBag.Message = "Dữ liệu của bạn không trùng khớp với thông tin của hệ thống.";
                            return View(model);
                        }
                    }

                    var question2 = ctx.PERSONAL_QUESTIONS.Where(u => u.QUESTION_ID == model.QuestionId2 && u.USR_ID == usr.MA_NGUOI_DUNG).FirstOrDefault();
                    if (question2 != null)
                    {
                        if (!(question2.ANSWER == crypto.Compute(model.Answer2, question2.ANSWER_SALT)))
                        {
                            model.QuestionId1 = 0;
                            model.QuestionId2 = 0;
                            model.QuestionId3 = 0;
                            ViewBag.Message = "Dữ liệu của bạn không trùng khớp với thông tin của hệ thống.";
                            return View(model);
                        }
                    }

                    var question3 = ctx.PERSONAL_QUESTIONS.Where(u => u.QUESTION_ID == model.QuestionId3 && u.USR_ID == usr.MA_NGUOI_DUNG).FirstOrDefault();
                    if (question3 != null)
                    {
                        if (!(question3.ANSWER == crypto.Compute(model.Answer3, question3.ANSWER_SALT)))
                        {
                            model.QuestionId1 = 0;
                            model.QuestionId2 = 0;
                            model.QuestionId3 = 0;
                            ViewBag.Message = "Dữ liệu của bạn không trùng khớp với thông tin của hệ thống.";
                            return View(model);
                        }
                    }

                    string newPass = SystemConstant.RandomString(8);
                    var email = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "EMAIL").FirstOrDefault();
                    var emailUser = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "EMAIL_USR").FirstOrDefault();
                    var emailPass = ctx.SMS_MASTER.Where(u => u.ACTIVE == "A" && u.NAME == "EMAIL_PASS").FirstOrDefault();

                    if (email == null || string.IsNullOrEmpty(email.VALUE)
                        || emailUser == null || string.IsNullOrEmpty(emailUser.VALUE)
                        || emailPass == null || string.IsNullOrEmpty(emailPass.VALUE))
                    {
                        ViewBag.Message = "Hệ thống chưa được cấu hình để gửi email. Vui lòng liên hệ admin.";
                        return View(model);
                    };

                    try
                    {
                        
                        usr.MAT_KHAU = crypto.Compute(newPass);
                        usr.SALT = crypto.Salt;
                        ctx.SaveChanges();
                        SystemConstant.sendEmail(usr.EMAIL, email.VALUE, "[Vân Phước - SMS] Mật khẩu mới", "Mật khẩu mới của bạn để đăng nhập vào hệ thống là " + newPass + ". Bạn nên thay đôi mật khẩu ngay sau khi đăng nhập.",
                            emailUser.VALUE,emailPass.VALUE);
                        ViewBag.InforMessage = "Mật khẩu đã được gửi đến email của bạn. Vui lòng đăng nhập email để lấy mật khẩu mới.";
                        return View(model);
                    }
                    catch(Exception ex)
                    {
                        Console.Write(ex.ToString());
                        ViewBag.Message = "Không thể cập nhật được mật khẩu mới cho bạn.";
                        return View(model);
                    }
                }
                else
                {
                    model.QuestionId1 = 0;
                    model.QuestionId2 = 0;
                    model.QuestionId3 = 0;
                    ViewBag.Message = "Dữ liệu của bạn không trùng khớp với thông tin của hệ thống.";
                    return View(model);
                }
            }
            return View(model);
        }

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
        public JsonResult CheckingLogIn(string username, string password)
        {
            if (IsValid(username, password))
            {
                return Json(new { Status = "Success" });
            }
            else
            {
                //ModelState.AddModelError("", "Nhập sai User name hoặc mật khẩu");
                return Json(new { Status = "Error" });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogIn(Models.NGUOI_DUNG userr)
        {
            //if (IsValid(userr.USER_NAME, userr.MAT_KHAU))
            //{
                FormsAuthentication.SetAuthCookie(userr.USER_NAME, false);
                return RedirectToAction("Index", "Home");
            //}
            //else
            //{
                //ModelState.AddModelError("", "Nhập sai User name hoặc mật khẩu");
                
            //}
            //return View(userr);
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
        [CustomActionFilter]
        public ActionResult Register(Models.NGUOI_DUNG user)
        {
            var crypto = new SimpleCrypto.PBKDF2();
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

                        newUser.MAT_KHAU = crypto.Compute(user.MAT_KHAU);

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
                var user = ctx.NGUOI_DUNG.Where(u => u.USER_NAME.ToLower() == username.ToLower() && u.ACTIVE == "A").FirstOrDefault();
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
