using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using SMS.Models;
using PagedList;
using System.IO;
using SMS.App_Start;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Transactions;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    public class NguoiDungController : Controller
    {
        //
        // GET: /DonVi/

        [HttpGet]
        public ActionResult StoreUser()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SetQuestions()
        {
            SecurityQuestion model = new SecurityQuestion();
            var ctx = new SmsContext();
            var questions = ctx.SECURITY_QUESTIONS.Where(u => u.ACTIVE == "A").ToList<SECURITY_QUESTIONS>();
            int userId = 0;
            int.TryParse(Session["UserId"].ToString(), out userId);
            var persionalQuestions = ctx.PERSONAL_QUESTIONS.Where(u => u.ACTIVE == "A" && u.USR_ID == userId).Take(3).ToList < PERSONAL_QUESTIONS>();
            if (persionalQuestions != null && persionalQuestions.Count == 3)
            {
                model.QuestionId1 = (int)persionalQuestions[0].QUESTION_ID;
                model.QuestionId2 = (int)persionalQuestions[1].QUESTION_ID;
                model.QuestionId3 = (int)persionalQuestions[2].QUESTION_ID;
            }
            else
            {
                model.QuestionId1 = 0;
                model.QuestionId2 = 0;
                model.QuestionId3 = 0;
            }
            model.Questions = questions;
            ctx.Dispose();
            return View(model);
        }


        public ActionResult SetQuestions(SecurityQuestion model)
        {
            if (ModelState.IsValid)
            {
                var ctx = new SmsContext();
                var questions = ctx.SECURITY_QUESTIONS.Where(u => u.ACTIVE == "A").ToList<SECURITY_QUESTIONS>();
                model.Questions = questions;
                var crypto = new SimpleCrypto.PBKDF2();
                int userId = 0;
                int.TryParse(Session["UserId"].ToString(), out userId);
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        ctx.PERSONAL_QUESTIONS.RemoveRange(ctx.PERSONAL_QUESTIONS.Where(x => x.USR_ID == userId));
                        var question1 = ctx.PERSONAL_QUESTIONS.Create();
                        question1.USR_ID = userId;
                        question1.QUESTION_ID = model.QuestionId1;
                        question1.ANSWER = crypto.Compute(model.Answer1);
                        question1.ANSWER_SALT = crypto.Salt;
                        question1.CREATE_AT = DateTime.Now;
                        question1.UPDATE_AT = DateTime.Now;
                        question1.ACTIVE = "A";
                        ctx.PERSONAL_QUESTIONS.Add(question1);

                        var question2 = ctx.PERSONAL_QUESTIONS.Create();
                        question2.USR_ID = userId;
                        question2.QUESTION_ID = model.QuestionId2;
                        question2.ANSWER = crypto.Compute(model.Answer2);
                        question2.ANSWER_SALT = crypto.Salt;
                        question2.CREATE_AT = DateTime.Now;
                        question2.UPDATE_AT = DateTime.Now;
                        question2.ACTIVE = "A";
                        ctx.PERSONAL_QUESTIONS.Add(question2);

                        var question3 = ctx.PERSONAL_QUESTIONS.Create();
                        question3.USR_ID = userId;
                        question3.QUESTION_ID = model.QuestionId3;
                        question3.ANSWER = crypto.Compute(model.Answer3);
                        question3.ANSWER_SALT = crypto.Salt;
                        question3.CREATE_AT = DateTime.Now;
                        question3.UPDATE_AT = DateTime.Now;
                        question3.ACTIVE = "A";
                        ctx.PERSONAL_QUESTIONS.Add(question3);

                        ctx.SaveChanges();
                        transaction.Complete();
                        ViewBag.InforMessage = "Cập nhật câu hỏi bảo mật thành công.";
                        ctx.Dispose();
                        return View(model);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                        Transaction.Current.Rollback();
                        ctx.Dispose();
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình cập nhật câu hỏi bảo mật.";
                        return View(model);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Index(string searchString, string sortOrder, string currentFilter, int? page)
        {
            var ctx = new SmsContext();
            if (!String.IsNullOrEmpty(searchString) && (page == null || page == 0))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = sortOrder == "id_desc" ? "id" : "id_desc";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            var theListContext = (from u in ctx.NGUOI_DUNG
                                  join k in ctx.KHOes on u.MA_KHO equals k.MA_KHO into kh
                                  from kho in kh.DefaultIfEmpty()
                                  join g in ctx.NHOM_NGUOI_DUNG on u.MA_NHOM_NGUOI_DUNG equals g.MA_NHOM into gr
                                  from gro in gr.DefaultIfEmpty()
                                  join u1 in ctx.NGUOI_DUNG on u.CREATE_BY equals u1.MA_NGUOI_DUNG
                                  join u2 in ctx.NGUOI_DUNG on u.UPDATE_BY equals u2.MA_NGUOI_DUNG
                                  where (u.ACTIVE == "A" && (String.IsNullOrEmpty(searchString) || u.TEN_NGUOI_DUNG.ToUpper().Contains(searchString.ToUpper()) || u.USER_NAME.ToUpper().Contains(searchString.ToUpper())))
                                  select new NguoiDungObj
                                  {
                                      Kho = kho, 
                                      NhomNguoiDung = gro,
                                      NguoiDung = u,
                                      NguoiTao = u1,
                                      NguoiCapNhat = u2
                                  }).Take(SystemConstant.MAX_ROWS);
            ViewBag.CurrentFilter = searchString;

            IPagedList<NguoiDungObj> nguoiDungs = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
            switch (sortOrder)
            {
                case "id":
                    nguoiDungs = theListContext.OrderBy(nd => nd.NguoiDung.MA_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                case "id_desc":
                    nguoiDungs = theListContext.OrderByDescending(nd => nd.NguoiDung.MA_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    nguoiDungs = theListContext.OrderBy(nd => nd.NguoiDung.TEN_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    nguoiDungs = theListContext.OrderByDescending(nd => nd.NguoiDung.TEN_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    nguoiDungs = theListContext.OrderBy(nd => nd.NguoiDung.MA_NGUOI_DUNG).ToPagedList(pageIndex, pageSize);
                    break;
            }
            ctx.Dispose();
            return View(nguoiDungs);
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult AddNew()
        {
            //Ma Kho
            BindKho();

            //Ma Nhom
            BindNhomNguoiDung();

            return View();
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            NGUOI_DUNG nguoidung = ctx.NGUOI_DUNG.Find(id);
            if (nguoidung.ACTIVE.Equals("A"))
            {
                //Ma Kho
                BindKho();

                //Ma Nhom
                BindNhomNguoiDung();

                ViewBag.nguoiDung = nguoidung;
                ctx.Dispose();
                return View(nguoidung);
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                return View("../Home/Error"); ;
            }

        }
        
        [CustomActionFilter]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id == (int)Session["UserId"])
            {
                ViewBag.Message = "Bạn không thể xóa chính mình ra khỏi hệ thống.";
                return View("../Home/Error"); ;
            }
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var nguoidung = ctx.NGUOI_DUNG.Find(id);
            if (nguoidung.ACTIVE.Equals("A"))
            {
                nguoidung.ACTIVE = "I";
                nguoidung.UPDATE_AT = DateTime.Now;
                nguoidung.CREATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("Index").Success("Xóa thành công");
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult Edit(SMS.Models.NGUOI_DUNG nguoiDung)
        {
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var nguoidung = db.NGUOI_DUNG.Find((int)nguoiDung.MA_NGUOI_DUNG);
                nguoidung.TEN_NGUOI_DUNG = nguoiDung.TEN_NGUOI_DUNG;
                nguoidung.NGAY_SINH = nguoiDung.NGAY_SINH;
                nguoidung.SO_CHUNG_MINH = nguoiDung.SO_CHUNG_MINH;
                nguoidung.DIA_CHI = nguoiDung.DIA_CHI;
                nguoidung.SO_DIEN_THOAI = nguoiDung.SO_DIEN_THOAI;
                nguoidung.MA_KHO = nguoiDung.MA_KHO;
                nguoidung.NGAY_VAO_LAM = nguoiDung.NGAY_VAO_LAM;

                if (Request.Files[0].InputStream.Length != 0)
                {
                    Stream fileStream = Request.Files[0].InputStream;
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                    nguoidung.HINH_ANH = bytes;
                }
                nguoidung.MA_NHOM_NGUOI_DUNG = nguoiDung.MA_NHOM_NGUOI_DUNG;
                nguoidung.GHI_CHU = nguoiDung.GHI_CHU;
                nguoidung.EMAIL = nguoiDung.EMAIL;
                nguoidung.ACTIVE = "A";
                nguoidung.UPDATE_AT = DateTime.Now;
                nguoidung.UPDATE_BY = (int)Session["UserId"];
                db.SaveChanges();
                return RedirectToAction("Index").Success("Lưu thành công");
            }
            var ctx = new SmsContext();
            NGUOI_DUNG nguoidung1 = ctx.NGUOI_DUNG.Find((int)nguoiDung.MA_NGUOI_DUNG);
            if (nguoidung1.ACTIVE.Equals("A"))
            {
                //Ma Kho
                BindKho();

                //Ma Nhom
                BindNhomNguoiDung();

                ViewBag.nguoiDung = nguoidung1;
                ctx.Dispose();
                return View(nguoidung1);
            }
            ctx.Dispose();
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult AddNew(SMS.Models.NGUOI_DUNG nguoiDung)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                var db = new SmsContext();
                var crypto = new SimpleCrypto.PBKDF2();
                var nguoidung = db.NGUOI_DUNG.Create();
                nguoidung.TEN_NGUOI_DUNG = nguoiDung.TEN_NGUOI_DUNG;
                nguoidung.NGAY_SINH = nguoiDung.NGAY_SINH;
                nguoidung.SO_CHUNG_MINH = nguoiDung.SO_CHUNG_MINH;
                nguoidung.DIA_CHI = nguoiDung.DIA_CHI;
                nguoidung.SO_DIEN_THOAI = nguoiDung.SO_DIEN_THOAI;
                nguoidung.EMAIL = nguoiDung.EMAIL;
                nguoidung.MA_KHO = nguoiDung.MA_KHO;
                nguoidung.USER_NAME = nguoiDung.USER_NAME;
                nguoidung.MAT_KHAU = crypto.Compute(nguoiDung.MAT_KHAU);
                nguoidung.SALT = crypto.Salt;
                nguoidung.NGAY_VAO_LAM = nguoiDung.NGAY_VAO_LAM;

                if (Request.Files[0].InputStream.Length != 0)
                {
                    Stream fileStream = Request.Files[0].InputStream;
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                    nguoidung.HINH_ANH = bytes;
                }
                nguoidung.MA_NHOM_NGUOI_DUNG = nguoiDung.MA_NHOM_NGUOI_DUNG;
                nguoidung.GHI_CHU = nguoiDung.GHI_CHU;
                nguoidung.ACTIVE = "A";
                nguoidung.UPDATE_AT = DateTime.Now;
                nguoidung.CREATE_AT = DateTime.Now;
                nguoidung.UPDATE_BY = (int)Session["UserId"];
                nguoidung.CREATE_BY = (int)Session["UserId"];
                db.NGUOI_DUNG.Add(nguoidung);
                db.SaveChanges();
                return RedirectToAction("Index").Success("Lưu thành công.");
            }
            //Ma Kho
            BindKho();

            //Ma Nhom
            BindNhomNguoiDung();
            return View();
        }

        [HttpGet]
        public FileContentResult GetImage(int id)
        {
            using (var ctx = new SmsContext())
            {
                var nd = ctx.NGUOI_DUNG.FirstOrDefault(p => p.MA_NGUOI_DUNG == id);

                if (nd != null && nd.HINH_ANH != null)
                {
                    return File(nd.HINH_ANH, "image/png");
                }
            }
            return null;
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult changePass()
        {
            ViewData["errorcode"] = "";
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult changePass(FormCollection form)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            String username = form["username"];
            String oldpass = form["oldpass"];
            String newpass = form["newpass"];
            String confirmpass = form["confirmpass"];
            
            using (var ctx = new SmsContext())
            {
                ViewData["errorcode"] = "99";

                NGUOI_DUNG nd = ctx.NGUOI_DUNG.FirstOrDefault(n => n.USER_NAME == username);
                oldpass = crypto.Compute(oldpass, nd.SALT);
                if (nd != null)
                {
                    if (!String.IsNullOrEmpty(oldpass))
                    {
                        if (oldpass.Equals(nd.MAT_KHAU))
                        {
                            if (!String.IsNullOrEmpty(newpass))
                            {
                                if (!String.IsNullOrEmpty(confirmpass))
                                {
                                    if (newpass.Equals(confirmpass))
                                    {
                                        
                                        nd.MAT_KHAU = crypto.Compute(newpass);
                                        nd.SALT = crypto.Salt;
                                        ctx.SaveChanges();
                                        ViewData["errormsg"] = "Thay đổi mật khẩu thành công!";
                                    }
                                    else
                                    {
                                        ViewData["errormsg"] = "Mật khẩu mới & xác nhận không khớp.";
                                        ViewData["errorcode"] = "4";
                                    }
                                }
                                else
                                {
                                    ViewData["errormsg"] = "Vui lòng nhập mật khẩu xác nhận.";
                                    ViewData["errorcode"] = "3";
                                }
                            }
                            else
                            {
                                ViewData["errormsg"] = "Vui lòng nhập mật khẩu mới.";
                                ViewData["errorcode"] = "2";
                            }
                        }
                        else
                        {
                            ViewData["errormsg"] = "Mật khẩu hiện tại không đúng.";
                            ViewData["errorcode"] = "1";
                        }
                    }
                    else
                    {
                        ViewData["errormsg"] = "Vui lòng nhập mật khẩu hiện tại.";
                        ViewData["errorcode"] = "1";
                    }
                }
                else
                {
                    ViewData["errormsg"] = "Người dùng không tồn tại.";
                }
            }
            return View();
        }

        private void BindKho()
        {
            using (var ctx = new SmsContext())
            {
                List<KHO> kho = ctx.KHOes.Where(m => m.ACTIVE.Equals("A")).ToList();
                ViewBag.Kho = kho;
            }
        }

        private void BindNhomNguoiDung()
        {
            using (var ctx = new SmsContext())
            {
                List<NHOM_NGUOI_DUNG> nhomNguoiDung = ctx.NHOM_NGUOI_DUNG.Where(m => m.ACTIVE.Equals("A")).ToList();
                ViewBag.NhomNguoiDung = nhomNguoiDung;
            }
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Show(int id)
        {
            var ctx = new SmsContext();
            var User = ctx.NGUOI_DUNG.Include("KHO").Include("NHOM_NGUOI_DUNG").Single(us => us.MA_NGUOI_DUNG == id);
            ctx.Dispose();
            return View(User);
        }

        [HttpPost]
        public JsonResult Find(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedUsers = from x in ctx.NGUOI_DUNG
                                 where (x.TEN_NGUOI_DUNG.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                 select new
                                 {
                                     id = x.MA_NGUOI_DUNG,
                                     value = x.TEN_NGUOI_DUNG
                                 };
            var result = Json(suggestedUsers.Take(10).ToList());
            ctx.Dispose();
            return result; 
        }

        [CustomActionFilter]
        [HttpPost]
        public JsonResult ResetPassword(int id)
        {
            object yourOjbect = null;
            string data = "";
            if (id == (int)Session["UserId"])
            {
                data = "{ \"Message \" : \"Không được cập nhật lại mật khẩu chính mình.\"}";
                yourOjbect = new JavaScriptSerializer().DeserializeObject(data);
                return Json(yourOjbect);
            }
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                data = "{ \"Message \" : \"Không tìm thấy người dùng tương ứng.\"}";

                //Deserializing it into an object that will contain each of the keys and their values
                yourOjbect = new JavaScriptSerializer().DeserializeObject(data);
                return Json(yourOjbect);
            }
            var ctx = new SmsContext();
            var nguoidung = ctx.NGUOI_DUNG.Find(id);
            var crypto = new SimpleCrypto.PBKDF2();
            if (nguoidung.ACTIVE.Equals("A"))
            {
                nguoidung.MAT_KHAU = crypto.Compute(nguoidung.USER_NAME);
                nguoidung.SALT = crypto.Salt;
                nguoidung.UPDATE_AT = DateTime.Now;
                nguoidung.CREATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                //return Json("{\"Message \":\" Cập nhật thành công.\"}");

                data = "{ \"Message \" : \"Cập nhật thành công.\"}";

                //Deserializing it into an object that will contain each of the keys and their values
                yourOjbect = new JavaScriptSerializer().DeserializeObject(data);
                ctx.Dispose();
                return Json(yourOjbect);
            }
            else
            {
                ViewBag.Message = "Không tìm thấy người dùng tương ứng.";
                data = "{ \"Message \" : \"Không tìm thấy người dùng tương ứng.\"}";
                yourOjbect = new JavaScriptSerializer().DeserializeObject(data);
                ctx.Dispose();
                return Json(yourOjbect);
            }
        }
    }
}
