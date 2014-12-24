using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Text;
using SMS.App_Start;
using System.Data;
using CsvHelper;
using System.Transactions;

namespace SMS.Controllers
{

    [Authorize]
    [HandleError]
    public class SanPhamController : Controller
    {
        public const string SEARCH_ADVANCE = "SearchAdvanceCondition";
        IPagedList<SanPhamDisplay> listResult = null;

        [HttpGet]
        [CustomActionFilter]
        public ActionResult Index(string inforMessage, string message)
        {
            ViewBag.InforMessage = inforMessage;
            ViewBag.Message = message;
            Session[SEARCH_ADVANCE] = null;
            var ctx = new SmsContext();
            var productGroups = ctx.NHOM_SAN_PHAM.Where(u => u.ACTIVE == "A").ToList<NHOM_SAN_PHAM>();
            ViewBag.ProductGroups = productGroups;
            ctx.Dispose();
            return View();
        }

        [CustomActionFilter]
        [HttpGet]        
        public ActionResult ListPriceProducts()
        {          
            return View();
        }

        private bool checkProductExisted(string code)
        {
            var ctx = new SmsContext();
            var product = ctx.SAN_PHAM.Where(u => u.ACTIVE == "A" && u.CODE.ToLower() == code.ToLower()).FirstOrDefault();
            if (product != null)
            {
                ctx.Dispose();
                return true;
            }
            return false;
        }


        private bool checkPriceDiscountAndWeigh(string price1, string price2, string price3, string discount1, string discount2, string discount3, string weight)
        {
            try
            {
                if(string.IsNullOrEmpty(price1) ||
                    string.IsNullOrEmpty(price2) ||
                    string.IsNullOrEmpty(price3))
                {
                    return false;
                }

                discount1 = string.IsNullOrEmpty(discount1) ? "0" : discount1;
                discount2 = string.IsNullOrEmpty(discount2) ? "0" : discount2;
                discount3 = string.IsNullOrEmpty(discount3) ? "0" : discount3;
                weight = string.IsNullOrEmpty(weight) ? "0" : weight;

                double.Parse(price1);
                double.Parse(price2);
                double.Parse(price3);
                double.Parse(discount1);
                double.Parse(discount2);
                double.Parse(discount3);
                double.Parse(weight);
            }catch
            {
                return false;
            }
            return true;
        }


        private bool checkUnit(string unitid)
        {
            try
            {
                if (string.IsNullOrEmpty(unitid) || "0".Equals(unitid.Trim()))
                {
                    return false;
                }
                int.Parse(unitid);
            }catch
            {
                return false;
            }
            return true;
        }

        [CustomActionFilter]
        public ActionResult importConvertCsv()
        {
            return View();
        }
        
        private bool checkConverterRow(string productId, string unitId, 
            string factor,string price1, string price2, string price3, string discount1, string discount2, string discount3)
        {
            discount1 = string.IsNullOrWhiteSpace(discount1) ? "0" : discount1;
            discount2 = string.IsNullOrWhiteSpace(discount2) ? "0" : discount2;
            discount3 = string.IsNullOrWhiteSpace(discount3) ? "0" : discount3;
            int proId;
            int unId;
            float fac, p1, p2, p3, d1, d2, d3;

            if(string.IsNullOrWhiteSpace(price1) 
                || string.IsNullOrWhiteSpace(price2) 
                || string.IsNullOrWhiteSpace(price3)
                || string.IsNullOrWhiteSpace(productId)
                || string.IsNullOrWhiteSpace(unitId))
            {
                return false;
            }

            if(!int.TryParse(productId, out proId)
                || !int.TryParse(unitId, out unId)
                || !float.TryParse(factor, out fac)
                || !float.TryParse(price1, out p1)
                || !float.TryParse(price2, out p2)
                || !float.TryParse(price3, out p3)
                || !float.TryParse(discount1, out d1)
                || !float.TryParse(discount2, out d2)
                || !float.TryParse(discount3, out d3))
            {
                return false;
            }
            return true;
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult importConvertCsv(HttpPostedFileBase file)
        {
            var ctx = new SmsContext();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            if (file != null)
            {
                var fp = Path.Combine(HttpContext.Server.MapPath("~/ImportUploads"), Path.GetFileName(file.FileName));
                file.SaveAs(fp);
                
                if (Path.GetExtension(fp) == null || Path.GetExtension(fp).ToLower() != ".csv")
                {
                    ViewBag.Message = "Định dạng file không đúng. Vui lòng chọn lại file import.";
                    return View();
                }
                ICsvParser csvParser = new CsvParser(new StreamReader(file.InputStream));
                CsvReader csvReader = new CsvReader(csvParser);
                string[] headers = { };
                List<string[]> rows = new List<string[]>();
                string[] row;
                
                while (csvReader.Read())
                {
                    if (csvReader.FieldHeaders != null && csvReader.FieldHeaders.Length != 13)
                    {
                        ViewBag.Message = "Định dạng file CSV không đúng. Vui lòng kiểm tra lại.";
                        return View();
                    }
                    else
                    {
                        if (csvReader.FieldHeaders != null && csvReader.FieldHeaders.Length > 0 && !headers.Any())
                        {
                            headers = csvReader.FieldHeaders;
                        }
                        row = new string[headers.Count()];
                        for (int j = 0; j < headers.Count(); j++)
                        {
                            row[j] = csvReader.GetField(j);
                        }
                        rows.Add(row);
                    }
                }

                bool flag = true;
                int i = 1;
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        CHUYEN_DOI_DON_VI_TINH convert;
                        foreach (var r in rows)
                        {
                            i++;
                            if (string.IsNullOrWhiteSpace(r[1]))
                            {
                                if (checkConverterRow(r[2], r[4], r[6], r[7], r[8], r[9], r[10], r[11], r[12]))
                                {
                                    convert = ctx.CHUYEN_DOI_DON_VI_TINH.Create();
                                    convert.UPDATE_AT = DateTime.Now;
                                    convert.ACTIVE = "A";
                                    convert.CREATE_AT = DateTime.Now;
                                    convert.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                                    convert.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                                    convert.MA_SAN_PHAN = int.Parse(r[2]);
                                    convert.MA_DON_VI_VAO = int.Parse(r[4]);
                                    convert.HE_SO = float.Parse(r[6]);
                                    convert.GIA_BAN_1 = float.Parse(r[7]);
                                    convert.GIA_BAN_2 = float.Parse(r[8]);
                                    convert.GIA_BAN_3 = float.Parse(r[9]);
                                    convert.CHIEC_KHAU_1 = string.IsNullOrWhiteSpace(r[10])? 0 : float.Parse(r[10]);
                                    convert.CHIEC_KHAU_2 = string.IsNullOrWhiteSpace(r[11]) ? 0 : float.Parse(r[11]);
                                    convert.CHIEC_KHAU_3 = string.IsNullOrWhiteSpace(r[12]) ? 0 :  float.Parse(r[12]);
                                    ctx.CHUYEN_DOI_DON_VI_TINH.Add(convert);
                                    ctx.SaveChanges();
                                }
                                else
                                {
                                    if (!flag)
                                    {
                                        fileStringBuilder.Append("<br>");
                                    }
                                    flag = false;
                                    fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Vui lòng kiểm tra ký tự số.");
                                }
                            }
                            else
                            {
                                int convertId = 0;
                                if (!int.TryParse(r[1], out convertId))
                                {
                                    if (!flag)
                                    {
                                        fileStringBuilder.Append("<br>");
                                    }
                                    flag = false;
                                    fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". ID phải là ký tự sô. Vui lòng kiểm tra lại");
                                }
                                else
                                {
                                    convert = ctx.CHUYEN_DOI_DON_VI_TINH.Find(convertId);
                                    if (convert == null || convert.ACTIVE != "A")
                                    {
                                        if (!flag)
                                        {
                                            fileStringBuilder.Append("<br>");
                                        }
                                        flag = false;
                                        fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". ID phải là ký tự sô. Vui lòng kiểm tra lại");
                                    }
                                    else
                                    {
                                        if (checkConverterRow(r[2], r[4], r[6], r[7], r[8], r[9], r[10], r[11], r[12]))
                                        {
                                            convert.ACTIVE = "A";
                                            convert.UPDATE_AT = DateTime.Now;
                                            convert.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                                            convert.MA_SAN_PHAN = int.Parse(r[2]);
                                            convert.MA_DON_VI_VAO = int.Parse(r[4]);
                                            convert.HE_SO = float.Parse(r[6]);
                                            convert.GIA_BAN_1 = float.Parse(r[7]);
                                            convert.GIA_BAN_2 = float.Parse(r[8]);
                                            convert.GIA_BAN_3 = float.Parse(r[9]);
                                            convert.CHIEC_KHAU_1 = string.IsNullOrWhiteSpace(r[10]) ? 0 : float.Parse(r[10]);
                                            convert.CHIEC_KHAU_2 = string.IsNullOrWhiteSpace(r[11]) ? 0 : float.Parse(r[11]);
                                            convert.CHIEC_KHAU_3 = string.IsNullOrWhiteSpace(r[12]) ? 0 : float.Parse(r[12]);
                                            ctx.SaveChanges();
                                        }
                                        else
                                        {
                                            if (!flag)
                                            {
                                                fileStringBuilder.Append("<br>");
                                            }
                                            flag = false;
                                            fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Vui lòng kiểm tra ký tự số.");
                                        }
                                    }
                                }
                            }
                        }
                        if (flag)
                        {
                            transaction.Complete();
                            ctx.Dispose();
                            return RedirectToAction("ConvertUnitOfProducts", new { @inforMessage = "Import danh sách sản phẩm thành công." });
                        }
                        else
                        {
                            Transaction.Current.Rollback();
                            ctx.Dispose();
                            ViewBag.Message = fileStringBuilder.ToString();
                            return View();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                        ctx.Dispose();
                        ViewBag.Message = "Lỗi dữ liệu tại dòng tại dòng " + i + ". Có lỗi xảy ra trong quá trình import. Vui lòng liên hệ admin.";
                        return View();
                    }
                }
            }
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult importCsv(HttpPostedFileBase file)
        {
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            if (file != null)
            {
                var fp = Path.Combine(HttpContext.Server.MapPath("~/ImportUploads"), Path.GetFileName(file.FileName));
                file.SaveAs(fp);

                if (Path.GetExtension(fp)  == null || Path.GetExtension(fp).ToLower() != ".csv")
                {
                    ViewBag.Message = "Định dạng file không đúng. Vui lòng chọn lại file import.";
                    return View();
                }

                ICsvParser csvParser = new CsvParser(new StreamReader(file.InputStream));
                CsvReader csvReader = new CsvReader(csvParser);
                string[] headers = { };
                List<string[]> rows = new List<string[]>();
                string[] row;

                while (csvReader.Read())
                {
                    if (csvReader.FieldHeaders != null && csvReader.FieldHeaders.Length != 19)
                    {
                        ViewBag.Message = "Định dạng file CSV không đúng. Vui lòng kiểm tra lại.";
                        return View();
                    }
                    else
                    {
                        if (csvReader.FieldHeaders != null && csvReader.FieldHeaders.Length > 0 && !headers.Any())
                        {
                            headers = csvReader.FieldHeaders;
                        }
                        row = new string[headers.Count()];
                        for (int j = 0; j < headers.Count(); j++)
                        {
                            row[j] = csvReader.GetField(j);
                        }
                        rows.Add(row);
                    }


                }
                bool flag = true;
                int i = 1;
                TransactionOptions transOptions = new TransactionOptions();
                transOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                transOptions.Timeout = TransactionManager.MaximumTimeout;
                using (var transaction = new System.Transactions.TransactionScope(TransactionScopeOption.Required, transOptions))
                {
                    try
                    {
                        SAN_PHAM product;
                        foreach (var r in rows)
                        {
                            i++;
                            if (string.IsNullOrWhiteSpace(r[1]))
                            {
                                if (string.IsNullOrWhiteSpace(r[2]) ||
                                    string.IsNullOrWhiteSpace(r[3]) ||
                                    string.IsNullOrWhiteSpace(r[4]) ||
                                    string.IsNullOrWhiteSpace(r[5]) ||
                                    string.IsNullOrWhiteSpace(r[6]) ||
                                    string.IsNullOrWhiteSpace(r[7]) ||
                                    string.IsNullOrWhiteSpace(r[8]))
                                {
                                    if (!flag)
                                    {
                                        fileStringBuilder.Append("<br>");
                                    }
                                    flag = false;
                                    fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Khi thêm mới bạn phải điền đầy đủ thông tin");
                                }
                                else
                                {
                                    if (checkProductExisted(r[2]))
                                    {
                                        if (!flag)
                                        {
                                            fileStringBuilder.Append("<br>");
                                        }
                                        flag = false;
                                        fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Trùng CODE.");
                                    }
                                    else
                                    {
                                        if (!checkPriceDiscountAndWeigh(r[6], r[7], r[8], r[9], r[10], r[11], r[13]))
                                        {
                                            if (!flag)
                                            {
                                                fileStringBuilder.Append("<br>");
                                            }
                                            flag = false;
                                            fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Vui lòng kiểm trai lại đơn giá, chiếc khấu và trọng lượng, tất cả phải là ký tự số.");
                                        }
                                        else
                                        {
                                            if (!checkUnit(r[4]) || !checkUnit(r[16]))
                                            {
                                                if (!flag)
                                                {
                                                    fileStringBuilder.Append("<br>");
                                                }
                                                flag = false;
                                                fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Vui lòng kiểm trai lại mã đơn vị hoặc mã nhóm.");
                                            }
                                            else
                                            {
                                                product = ctx.SAN_PHAM.Create();
                                                product.CODE = r[2];
                                                product.TEN_SAN_PHAM = r[3];
                                                product.MA_DON_VI = Convert.ToInt32(r[4]);
                                                product.GIA_BAN_1 = Convert.ToDouble(r[6]);
                                                product.GIA_BAN_2 = Convert.ToDouble(r[7]);
                                                product.GIA_BAN_3 = Convert.ToDouble(r[8]);                                                        
                                                product.CHIEC_KHAU_1 = string.IsNullOrWhiteSpace(r[9])? 0 : Convert.ToDouble(r[9]);
                                                product.CHIEC_KHAU_2 = string.IsNullOrWhiteSpace(r[10]) ? 0 : Convert.ToDouble(r[10]);
                                                product.CHIEC_KHAU_3 = string.IsNullOrWhiteSpace(r[11]) ? 0 : Convert.ToDouble(r[11]);
                                                product.KICH_THUOC = r[12];
                                                product.CAN_NANG = string.IsNullOrWhiteSpace(r[13]) ? 0 : Convert.ToDouble(r[13]);
                                                product.CO_SO_TOI_THIEU = string.IsNullOrWhiteSpace(r[14]) ? 0 : Convert.ToDouble(r[14]);
                                                product.CO_SO_TOI_DA = string.IsNullOrWhiteSpace(r[15]) ? 0 : Convert.ToDouble(r[15]);
                                                product.MA_NHOM = Convert.ToInt32(r[16]);
                                                product.DAC_TA  = r[18];
                                                product.ACTIVE = "A";
                                                product.UPDATE_AT = DateTime.Now;
                                                product.CREATE_AT = DateTime.Now;
                                                product.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                                                product.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                                                ctx.SAN_PHAM.Add(product);
                                                ctx.SaveChanges();
                                            }
                                        }
                                                
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(r[2]) ||
                                    string.IsNullOrWhiteSpace(r[3]) ||
                                    string.IsNullOrWhiteSpace(r[4]) ||
                                    string.IsNullOrWhiteSpace(r[5]) ||
                                    string.IsNullOrWhiteSpace(r[6]) ||
                                    string.IsNullOrWhiteSpace(r[7]) ||
                                    string.IsNullOrWhiteSpace(r[8]))
                                {
                                    if (!flag)
                                    {
                                        fileStringBuilder.Append("<br>");
                                    }
                                    flag = false;
                                    fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Vui lòng điền đầy đủ các thông tin cần cập nhật.");
                                }
                                else
                                {
                                    int productId = 0;
                                    if (!int.TryParse(r[1], out productId))
                                    {
                                        if (!flag)
                                        {
                                            fileStringBuilder.Append("<br>");
                                        }
                                        flag = false;
                                        fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Mã sản phẩm phải là số nguyên.");
                                    }
                                    else
                                    {
                                        product = ctx.SAN_PHAM.Find(productId);
                                        if (product == null || product.ACTIVE != "A")
                                        {
                                            if (!flag)
                                            {
                                                fileStringBuilder.Append("<br>");
                                            }
                                            flag = false;
                                            fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Không tồn tại sản phẩm này trong hệ thống.");
                                        }
                                        else
                                        {
                                            string code = r[2].Trim();
                                            var product1 = ctx.SAN_PHAM.Where(u => u.ACTIVE == "A" && u.CODE.ToLower() == code).FirstOrDefault();
                                            if (product1 != null && product1.MA_SAN_PHAM != product.MA_SAN_PHAM)
                                            {
                                                if (!flag)
                                                {
                                                    fileStringBuilder.Append("<br>");
                                                }
                                                flag = false;
                                                fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Sản phẩm trùng CODE. Vui lòng chọn code khác");
                                            }
                                            else
                                            {
                                                if (!checkPriceDiscountAndWeigh(r[6], r[7], r[8], r[9], r[10], r[11], r[13]))
                                                {
                                                    if (!flag)
                                                    {
                                                        fileStringBuilder.Append("<br>");
                                                    }
                                                    flag = false;
                                                    fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Vui lòng kiểm trai lại đơn giá, chiếc khấu và trọng lượng, tất cả phải là ký tự số.");
                                                }
                                                else
                                                {
                                                    if (!checkUnit(r[4]) || !checkUnit(r[16]))
                                                    {
                                                        if (!flag)
                                                        {
                                                            fileStringBuilder.Append("<br>");
                                                        }
                                                        flag = false;
                                                        fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Vui lòng kiểm trai lại mã đơn vị hoặc mã nhóm.");
                                                    }
                                                    else
                                                    {
                                                        if(i>=350)
                                                        {
                                                            Console.Write(i);
                                                        }
                                                        product.CODE = r[2];
                                                        product.TEN_SAN_PHAM = r[3];
                                                        product.MA_DON_VI = Convert.ToInt32(r[4]);
                                                        product.GIA_BAN_1 = Convert.ToDouble(r[6]);
                                                        product.GIA_BAN_2 = Convert.ToDouble(r[7]);
                                                        product.GIA_BAN_3 = Convert.ToDouble(r[8]);
                                                        product.CHIEC_KHAU_1 = string.IsNullOrWhiteSpace(r[9]) ? 0 : Convert.ToDouble(r[9]);
                                                        product.CHIEC_KHAU_2 = string.IsNullOrWhiteSpace(r[10]) ? 0 : Convert.ToDouble(r[10]);
                                                        product.CHIEC_KHAU_3 = string.IsNullOrWhiteSpace(r[11]) ? 0 : Convert.ToDouble(r[11]);
                                                        product.KICH_THUOC = r[12];
                                                        product.CAN_NANG = string.IsNullOrWhiteSpace(r[13]) ? 0 : Convert.ToDouble(r[13]);
                                                        product.CO_SO_TOI_THIEU = string.IsNullOrWhiteSpace(r[14]) ? 0 : Convert.ToDouble(r[14]);
                                                        product.CO_SO_TOI_DA = string.IsNullOrWhiteSpace(r[15]) ? 0 : Convert.ToDouble(r[15]);
                                                        product.MA_NHOM = Convert.ToInt32(r[16]);
                                                        product.DAC_TA = r[18];
                                                        product.ACTIVE = "A";
                                                        product.UPDATE_AT = DateTime.Now;
                                                        product.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                                                        ctx.SaveChanges();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (flag)
                        {
                            transaction.Complete();
                            ctx.Dispose();
                            return RedirectToAction("Index", new { @inforMessage = "Import danh sách sản phẩm thành công." });
                        }
                        else
                        {
                            Transaction.Current.Rollback();
                            ctx.Dispose();
                            ViewBag.Message = fileStringBuilder.ToString();
                            return View();                                    
                        }
                    }
                    catch(Exception ex)
                    {
                        Transaction.Current.Rollback();
                        ctx.Dispose();
                        ViewBag.Message = "Lỗi dữ liệu tại dòng tại dòng " + i + ". Có lỗi xảy ra trong quá trình import (" + ex.ToString() +"). Vui lòng liên hệ admin.";
                        return View();
                    }
                }
            }
                
            
            else
            {
                ViewBag.Message = "Bạn chưa chọn file. Vui lòng chọn file import.";
                return View();
            }
        }

        [CustomActionFilter]
        public ActionResult importCsv(string message, string inforMessage)
        {
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            return View();
        }


        [HttpPost]
        public JsonResult FindProductByCode(string code)
        {
            var ctx = new SmsContext();
            var result = ctx.SAN_PHAM.Where(u => u.ACTIVE == "A" && u.CODE.ToLower() == code.ToLower()).FirstOrDefault();
            if (result == null)
            {
                return null;
            }
            var jresult = Json(new
            {
                ma_san_pham = result.MA_SAN_PHAM,
                ten_san_pham = result.TEN_SAN_PHAM,
                code = result.CODE,
                gia_ban_1 = result.GIA_BAN_1,
                gia_ban_2 = result.GIA_BAN_2,
                gia_ban_3 = result.GIA_BAN_3,
                chiec_khau_1 = result.CHIEC_KHAU_1,
                chiec_khau_2 = result.CHIEC_KHAU_2,
                chiec_khau_3 = result.CHIEC_KHAU_3,
            });
            ctx.Dispose();
            return jresult;
        }

        [CustomActionFilter]
        [HttpPost]
        public FileContentResult downloadConvertCSVTemplate(int? productGroupId, string CurrentFilter)
        {
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"ID\",");
            fileStringBuilder.Append("\"Mã sản phẩm\",");
            fileStringBuilder.Append("\"Tên sản phẩm\",");
            fileStringBuilder.Append("\"Mã đơn vị tính\",");
            fileStringBuilder.Append("\"Tên đơn vị tính\",");
            fileStringBuilder.Append("\"Hệ số\",");
            fileStringBuilder.Append("\"Đơn giá 1\",");
            fileStringBuilder.Append("\"Đơn giá 2\",");
            fileStringBuilder.Append("\"Đơn giá 3\",");
            fileStringBuilder.Append("\"Chiếc khấu 1\",");
            fileStringBuilder.Append("\"Chiếc khấu 2\",");
            fileStringBuilder.Append("\"Chiếc khấu 3\"");
            var ctx = new SmsContext();
            var converts = ctx.CHUYEN_DOI_DON_VI_TINH.Include("SAN_PHAM").Include("DON_VI_TINH").Where(u => u.ACTIVE == "A"
                && (productGroupId == null || productGroupId == 0 ||  u.SAN_PHAM.MA_NHOM == productGroupId)
                && (string.IsNullOrEmpty(CurrentFilter) || u.SAN_PHAM.TEN_SAN_PHAM.Contains(CurrentFilter)));
            int i = 0;
            foreach (var convert in converts)
            {
                fileStringBuilder.Append("\n");
                i += 1;
                fileStringBuilder.Append("\"" + i + "\",");
                fileStringBuilder.Append("\"" + convert.MA_CHUYEN_DOI + "\",");
                fileStringBuilder.Append("\"" + convert.MA_SAN_PHAN + "\",");
                fileStringBuilder.Append("\"" + convert.SAN_PHAM.TEN_SAN_PHAM + "\",");
                fileStringBuilder.Append("\"" + convert.MA_DON_VI_VAO + "\",");
                fileStringBuilder.Append("\"" + convert.DON_VI_TINH.TEN_DON_VI + "\",");
                fileStringBuilder.Append("\"" + (convert.HE_SO == null? "0": ((Double)convert.HE_SO).ToString("#,###.##").Replace("\"", "\"\"") + "\","));
                fileStringBuilder.Append("\"" + (convert.GIA_BAN_1 == null ? "0" : ((Double)convert.GIA_BAN_1).ToString("#,###.##").Replace("\"", "\"\"") + "\","));
                fileStringBuilder.Append("\"" + (convert.GIA_BAN_2 == null ? "0" : ((Double)convert.GIA_BAN_2).ToString("#,###.##").Replace("\"", "\"\"") + "\","));
                fileStringBuilder.Append("\"" + (convert.GIA_BAN_3 == null ? "0" : ((Double)convert.GIA_BAN_3).ToString("#,###.##").Replace("\"", "\"\"") + "\","));
                fileStringBuilder.Append("\"" + (convert.CHIEC_KHAU_1 == null ? "0" : ((Double)convert.CHIEC_KHAU_1).ToString("#,###.##").Replace("\"", "\"\"") + "\","));
                fileStringBuilder.Append("\"" + (convert.CHIEC_KHAU_2 == null ? "0" : ((Double)convert.CHIEC_KHAU_2).ToString("#,###.##").Replace("\"", "\"\"") + "\","));
                fileStringBuilder.Append("\"" + (convert.CHIEC_KHAU_3 == null ? "0" : ((Double)convert.CHIEC_KHAU_3).ToString("#,###.##").Replace("\"", "\"\"") + "\""));
            }
            ctx.Dispose();
            return File(new System.Text.UTF8Encoding().GetBytes(fileStringBuilder.ToString()), "text/csv", fileName + ".csv");
        }

        [HttpPost]
        public FileContentResult downloadCSVTemplate(int? productGroupId, string CurrentFilter)
        {
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"Mã sản phẩm\",");
            fileStringBuilder.Append("\"CODE\",");
            fileStringBuilder.Append("\"Tên sản phẩm\",");
            fileStringBuilder.Append("\"Mã đơn vị tính\",");
            fileStringBuilder.Append("\"Tên đơn vị tính\",");
            fileStringBuilder.Append("\"Đơn giá 1\",");
            fileStringBuilder.Append("\"Đơn giá 2\",");
            fileStringBuilder.Append("\"Đơn giá 3\",");
            fileStringBuilder.Append("\"Chiếc khấu 1\",");
            fileStringBuilder.Append("\"Chiếc khấu 2\",");
            fileStringBuilder.Append("\"Chiếc khấu 3\",");
            fileStringBuilder.Append("\"Kích thước\",");
            fileStringBuilder.Append("\"Trọng lượng\",");
            fileStringBuilder.Append("\"Tồn kho tối thiểu\",");
            fileStringBuilder.Append("\"Tồn kho tối đa\",");
            fileStringBuilder.Append("\"Mã nhóm\",");
            fileStringBuilder.Append("\"Tên nhóm\",");
            fileStringBuilder.Append("\"Đặc tả\"");
            var ctx = new SmsContext();
            var products = ctx.SAN_PHAM.Include("DON_VI_TINH").Include("NHOM_SAN_PHAM").Where(u => u.ACTIVE == "A" &&
                (productGroupId == null || productGroupId  == 0|| u.MA_NHOM == productGroupId) &&
                (string.IsNullOrEmpty(CurrentFilter) || u.TEN_SAN_PHAM.Contains(CurrentFilter))
            ).ToList<SAN_PHAM>();
            int i = 0;
            foreach (var product in products)
            {
                fileStringBuilder.Append("\n");
                i += 1;
                fileStringBuilder.Append("\"" + i + "\",");
                fileStringBuilder.Append("\"" + product.MA_SAN_PHAM + "\",");
                fileStringBuilder.Append("\"" + (string.IsNullOrEmpty(product.CODE) ? "" : product.CODE).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (string.IsNullOrEmpty(product.TEN_SAN_PHAM) ? "" : product.TEN_SAN_PHAM).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + product.MA_DON_VI + "\",");
                fileStringBuilder.Append("\"" + (string.IsNullOrEmpty(product.DON_VI_TINH.TEN_DON_VI) ? "" : product.DON_VI_TINH.TEN_DON_VI).Replace("\"", "\"\"") + "\",");                
                fileStringBuilder.Append("\"" + (product.GIA_BAN_1 == null ? "" : ((Double)product.GIA_BAN_1).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (product.GIA_BAN_2 == null ? "" : ((Double)product.GIA_BAN_2).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (product.GIA_BAN_3 == null ? "" : ((Double)product.GIA_BAN_3).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (product.CHIEC_KHAU_1 == null ? "" : ((Double)product.CHIEC_KHAU_1).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (product.CHIEC_KHAU_2 == null ? "" : ((Double)product.CHIEC_KHAU_2).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (product.CHIEC_KHAU_3 == null ? "" : ((Double)product.CHIEC_KHAU_3).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + product.KICH_THUOC + "\",");
                fileStringBuilder.Append("\"" + (product.CAN_NANG == null ? "0" : ((Double)product.CAN_NANG).ToString("#,###0.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (product.CO_SO_TOI_THIEU == null ? "" : ((Double)product.CO_SO_TOI_THIEU).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (product.CO_SO_TOI_DA == null ? "" : ((Double)product.CO_SO_TOI_DA).ToString("#,###.##")).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + product.MA_NHOM + "\",");
                fileStringBuilder.Append("\"" + (product.MA_NHOM == null ? "" : product.NHOM_SAN_PHAM.TEN_NHOM).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + product.DAC_TA + "\"");
            }
            ctx.Dispose();
            return File(new System.Text.UTF8Encoding().GetBytes(fileStringBuilder.ToString()), "text/csv", fileName + ".csv");
        }

        [HttpPost]
        public PartialViewResult PagingContent(string sortOrder, string CurrentFilter, int? currentPageIndex, FormCollection form)
        {
            if (Session[SEARCH_ADVANCE] == null)
            {
                listResult = GetListProductNotSearchAdvance(sortOrder, CurrentFilter, currentPageIndex , form );
            }
            else
            {
                ProductSA psa = (ProductSA)Session[SEARCH_ADVANCE];
                listResult = GetListProductSearchAdvance(sortOrder, psa, currentPageIndex);
            }

            return PartialView("SanPhamPV", listResult);
        }

        [HttpPost]
        public JsonResult FindSuggest(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                 where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                 select new
                                 {
                                     id = x.MA_SAN_PHAM,
                                     value = x.TEN_SAN_PHAM
                                 };
            var result = Json(suggestedProducts.Take(5).ToList());
            ctx.Dispose();
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestWithUnitName(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    join u in ctx.DON_VI_TINH on x.MA_DON_VI equals u.MA_DON_VI
                                    where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        value = x.TEN_SAN_PHAM,
                                        unitName = u.TEN_DON_VI
                                    };
            var result = Json(suggestedProducts.Take(10).ToList());
            ctx.Dispose();
            return result;
        }


        [HttpPost]
        public JsonResult FindSuggestName(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        value = x.TEN_SAN_PHAM, 
                                        code = x.CODE
                                    };
            var result = Json(suggestedProducts.Take(10).ToList());
            ctx.Dispose();
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestByCd(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    where (x.CODE.ToLower().StartsWith(prefixText.ToLower()) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        value = x.CODE,
                                        name = x.TEN_SAN_PHAM
                                    };
            var result = Json(suggestedProducts.Take(10).ToList());
            ctx.Dispose();
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestForReturn(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    where ((x.CODE.ToLower().StartsWith(prefixText.ToLower())
                                    || x.TEN_SAN_PHAM.ToLower().StartsWith(prefixText.ToLower())) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        code = x.CODE,
                                        name = x.TEN_SAN_PHAM,
                                        price = (x.GIA_BAN_3 == null ? 0 : x.GIA_BAN_3*90/100)
                                    };
            var result = Json(suggestedProducts.Take(10).ToList());
            ctx.Dispose();
            return result;
        }

        [HttpPost]
        public JsonResult FindInventory(int? storeId, int? productId)
        {
            var ctx = new SmsContext();
            var StoreIdParam = new SqlParameter
            {
                ParameterName = "STORE_ID",
                Value = Convert.ToInt32(storeId)
            };

            var ProductIdParam = new SqlParameter
            {
                ParameterName = "PRODUC_ID",
                Value = Convert.ToInt32(productId)
            };

            var returnValue = new SqlParameter
            {
                ParameterName = "RETURN_VAL",
                Value = Convert.ToInt32(0),
                Direction = ParameterDirection.Output
            };
            var tonkho = ctx.Database.SqlQuery<Object>("exec SP_GET_TON_KHO_BY_STORE_ID_AND_PRODUCT_ID @STORE_ID, @PRODUC_ID, @RETURN_VAL OUT ",
                StoreIdParam, ProductIdParam, returnValue).ToList<Object>().Take(SystemConstant.MAX_ROWS);
            var rv = returnValue.Value == DBNull.Value ? 0 : Convert.ToDouble(returnValue.Value);
            var jresult = Json(new
            {
                ton_kho = rv.ToString("#.##"),
            });
            ctx.Dispose();
            return jresult;
        }


        [HttpPost]
        public JsonResult FindSuggestByCode(string prefixText, string typeCustomer)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    join u in ctx.DON_VI_TINH on x.MA_DON_VI equals u.MA_DON_VI
                                    where (x.CODE.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        code = x.CODE,
                                        label = x.CODE,
                                        name = x.TEN_SAN_PHAM,
                                        unit = x.MA_DON_VI,
                                        unitNm = u.TEN_DON_VI,
                                        price = typeCustomer.Equals("1") ? x.GIA_BAN_1 ?? 0 :
                                                    (typeCustomer.Equals("2") ? x.GIA_BAN_2 ?? 0 : x.GIA_BAN_3 ?? 0),
                                        discount = typeCustomer.Equals("1") ? x.CHIEC_KHAU_1 ?? 0 :
                                                    (typeCustomer.Equals("2") ? x.CHIEC_KHAU_2 ?? 0 : x.CHIEC_KHAU_3 ?? 0)
                                    };
            var result = Json(suggestedProducts.Take(10).ToList());
            ctx.Dispose();
            return result;
        }

        [HttpPost]

        public JsonResult FindSuggestFirstbyCode(string prefixText, string typeCustomer)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    join u in ctx.DON_VI_TINH on x.MA_DON_VI equals u.MA_DON_VI
                                    where (x.CODE.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        code = x.CODE,
                                        label = x.CODE,
                                        name = x.TEN_SAN_PHAM,
                                        unit = x.MA_DON_VI,
                                        unitNm = u.TEN_DON_VI,
                                        price = typeCustomer.Equals("1") ? x.GIA_BAN_1 ?? 0 :
                                                    (typeCustomer.Equals("2") ? x.GIA_BAN_2 ?? 0 : x.GIA_BAN_3 ?? 0),
                                        discount = typeCustomer.Equals("1") ? x.CHIEC_KHAU_1 ?? 0 :
                                                    (typeCustomer.Equals("2") ? x.CHIEC_KHAU_2 ?? 0 : x.CHIEC_KHAU_3 ?? 0)
                                    };
            var result = Json(suggestedProducts.Take(1).ToList());
            ctx.Dispose();
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestByTypeCustomer(string prefixText, string typeCustomer)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    join u in ctx.DON_VI_TINH on x.MA_DON_VI equals u.MA_DON_VI
                                    where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        code = x.CODE,
                                        name = x.TEN_SAN_PHAM,
                                        label = x.TEN_SAN_PHAM,    
                                        unit = x.MA_DON_VI,
                                        unitNm = u.TEN_DON_VI,
                                        price = typeCustomer.Equals("1") ? x.GIA_BAN_1 ?? 0: 
                                                    (typeCustomer.Equals("2") ? x.GIA_BAN_2 ?? 0 : x.GIA_BAN_3 ?? 0),
                                        discount = typeCustomer.Equals("1") ? x.CHIEC_KHAU_1 ?? 0 : 
                                                    (typeCustomer.Equals("2") ? x.CHIEC_KHAU_2 ?? 0 : x.CHIEC_KHAU_3 ?? 0)
                                    };
            var result = Json(suggestedProducts.Take(10).ToList());
            ctx.Dispose();
            return result;
        }

        [HttpPost]
        public JsonResult FindSuggestConvert(string prefixText)
        {
            var ctx = new SmsContext();
            var suggestedProducts = from x in ctx.SAN_PHAM
                                    join u in ctx.DON_VI_TINH on x.MA_DON_VI equals u.MA_DON_VI
                                    where (x.TEN_SAN_PHAM.StartsWith(prefixText) && x.ACTIVE.Equals("A"))
                                    select new
                                    {
                                        id = x.MA_SAN_PHAM,
                                        value = x.TEN_SAN_PHAM,
                                        unitRoot = x.MA_DON_VI,
                                        unitName = u.TEN_DON_VI
                                    };
            var result = Json(suggestedProducts.Take(10).ToList());
            ctx.Dispose();
            return result;
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult ExportExcel(FormCollection collection)
        {
            string fileName = "BaoGiaKhachHang_";
            fileName += DateTime.Now.ToString("dd-MM-yyyy-HHmmss");
            fileName += ".xls";
            string listProductID = "";
            string[] arrRowNum= new string[]{};

            if (collection.AllKeys.Contains("ListProductID"))
            {
                listProductID = collection.Get("ListProductID");
                arrRowNum = listProductID.Split(new char[] { ',' });
            }

            var products = new System.Data.DataTable("Products");
            products.Columns.Add("Mã Sản Phẩm", typeof(string));
            products.Columns.Add("Tên Sản Phẩm", typeof(string));
            products.Columns.Add("Giá bán", typeof(string));
            products.Columns.Add("Chiết khấu", typeof(string));
            products.Columns.Add("Giá thực", typeof(string));
            foreach (string RowNum in arrRowNum)
            {

                products.Rows.Add(RowNum.Split(new char[]{'_'})[0], 
                    collection.Get("TenSanPham_"  + RowNum) , collection.Get("GiaBan_" + RowNum) , 
                                       collection.Get("ChietKhau_"  + RowNum) , collection.Get("GiaThuc_" + RowNum));
            }

            var grid = new GridView();

            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.ms-excel";
            //Response.Charset = "utf-8";
            //Response.HeaderEncoding = Encoding.UTF8;
            //Response.ContentEncoding = Encoding.UTF8;
            Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"/>");

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("ListPriceProducts");
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult AddNew()
        {
            var ctx = new SmsContext();
            BindListDV(ctx);
            BindListNhomSP(ctx);
            BindListNSX(ctx);
            SetModeTitle(false);
            SetDefaultValue();
            ctx.Dispose();
            return View();
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            string msg =  "Không tìm thấy sản phẩm tương ứng.";
            if (id <= 0)
            {
                ViewBag.Message = msg; 
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            SAN_PHAM sp = ctx.SAN_PHAM.Find(id);
            if (sp.ACTIVE.Equals("A"))
            {
                SetModeTitle(true);
                BindListDV(ctx);
                BindListNhomSP(ctx);
                BindListNSX(ctx);

                SetHiddenFields(sp);
                ctx.Dispose();
                return View("../SanPham/AddNew", sp);

            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = msg; 
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        [HttpPost]
        public PartialViewResult SearchAdvance(FormCollection collection)
        {
            ProductSA psa = new ProductSA();
            if (collection.AllKeys.Contains("TenSanPham"))
            {
                psa.TenSanPham = collection.Get("TenSanPham");
            }

            if (collection.AllKeys.Contains("KichThuoc"))
            {
                psa.KichThuoc = collection.Get("KichThuoc");
            }
            if (collection.AllKeys.Contains("TrongLuongFrom"))
            {
                psa.TrongLuongFrom = collection.Get("TrongLuongFrom");
            }
            if (collection.AllKeys.Contains("TrongLuongTo"))
            {
                psa.TrongLuongTo = collection.Get("TrongLuongTo");
            }
            if (collection.AllKeys.Contains("DonViTinh"))
            {
                psa.DonViTinh = collection.Get("DonViTinh");
            }
            if (collection.AllKeys.Contains("NhaSanXuat"))
            {
                psa.NhaSanXuat = collection.Get("NhaSanXuat");
            }
            if (collection.AllKeys.Contains("DacTa"))
            {
                psa.DacTa = collection.Get("DacTa");
            }
            if (collection.AllKeys.Contains("GiaBanFrom"))
            {
                psa.GiaBanFrom = collection.Get("GiaBanFrom");
            }
            if (collection.AllKeys.Contains("GiaBanTo"))
            {
                psa.GiaBanTo = collection.Get("GiaBanTo");
            }

            if (collection.AllKeys.Contains("ChietKhauFrom"))
            {
                psa.ChietKhauFrom = collection.Get("ChietKhauFrom");
            }
            if (collection.AllKeys.Contains("ChietKhauTo"))
            {
                psa.ChietKhauTo = collection.Get("ChietKhauTo");
            }

            if (collection.AllKeys.Contains("CoSoFrom"))
            {
                psa.CoSoFrom = collection.Get("CoSoFrom");
            }
            if (collection.AllKeys.Contains("CoSoTo"))
            {
                psa.CoSoTo = collection.Get("CoSoTo");
            }

            if (collection.AllKeys.Contains("NhomSanPham"))
            {
                psa.NhomSanPham = collection.Get("NhomSanPham");
            }

            Session[SEARCH_ADVANCE] = psa;

            IPagedList<SanPhamDisplay> listResult = GetListProductSearchAdvance(null, psa, null);
            return PartialView("SanPhamPV", listResult);
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult Edit(SAN_PHAM productUpdated)
        {
            var db = new SmsContext();
            var sp = db.SAN_PHAM.Find((int)productUpdated.MA_SAN_PHAM);

            sp.TEN_SAN_PHAM = productUpdated.TEN_SAN_PHAM;
            sp.KICH_THUOC = productUpdated.KICH_THUOC;
            sp.CAN_NANG = productUpdated.CAN_NANG;
            sp.MA_DON_VI = productUpdated.MA_DON_VI;
            sp.CODE = productUpdated.CODE;
            if (-1 == productUpdated.MA_NHA_SAN_XUAT)
            {
                productUpdated.MA_NHA_SAN_XUAT = null;
            }
            if (-1 == productUpdated.MA_NHOM)
            {
                productUpdated.MA_NHOM = null;
            }
            sp.MA_DON_VI = productUpdated.MA_DON_VI;
            sp.MA_NHA_SAN_XUAT = productUpdated.MA_NHA_SAN_XUAT;
            sp.MA_NHOM = productUpdated.MA_NHOM;
            sp.DAC_TA = productUpdated.DAC_TA;
            sp.GIA_BAN_1 = productUpdated.GIA_BAN_1;
            sp.GIA_BAN_2 = productUpdated.GIA_BAN_2;
            sp.GIA_BAN_3 = productUpdated.GIA_BAN_3;
            sp.CHIEC_KHAU_1 = productUpdated.CHIEC_KHAU_1;
            sp.CHIEC_KHAU_2 = productUpdated.CHIEC_KHAU_2;
            sp.CHIEC_KHAU_3 = productUpdated.CHIEC_KHAU_3;
            sp.CO_SO_TOI_THIEU = productUpdated.CO_SO_TOI_THIEU;
            sp.CO_SO_TOI_DA = productUpdated.CO_SO_TOI_DA;
            //common fields
            sp.ACTIVE = "A";
            sp.UPDATE_AT = DateTime.Now;
            sp.UPDATE_BY = (int)Session["UserId"];

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy đơn vị tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var sp = ctx.SAN_PHAM.Find(id);
            if (sp.ACTIVE.Equals("A"))
            {
                sp.ACTIVE = "I";
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("Index");
            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = "Không tìm thấy đơn vị tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult AddNew(SAN_PHAM productInsert)
        {
            var db = new SmsContext();
            var sp = db.SAN_PHAM.Create();
            // input fields
            sp.TEN_SAN_PHAM = productInsert.TEN_SAN_PHAM;
            sp.KICH_THUOC = productInsert.KICH_THUOC;
            sp.CAN_NANG = productInsert.CAN_NANG;
            sp.MA_DON_VI = productInsert.MA_DON_VI;
            sp.CODE = productInsert.CODE;
            if (-1 == productInsert.MA_NHA_SAN_XUAT)
            {
                productInsert.MA_NHA_SAN_XUAT = null;
            }
            if (-1 == productInsert.MA_NHOM)
            {
                productInsert.MA_NHOM = null;
            }
            sp.MA_DON_VI = productInsert.MA_DON_VI;
            sp.MA_NHA_SAN_XUAT = productInsert.MA_NHA_SAN_XUAT;
            sp.MA_NHOM = productInsert.MA_NHOM;
            sp.DAC_TA = productInsert.DAC_TA;
            sp.GIA_BAN_1 = productInsert.GIA_BAN_1;
            sp.GIA_BAN_2 = productInsert.GIA_BAN_2;
            sp.GIA_BAN_3 = productInsert.GIA_BAN_3;
            sp.CHIEC_KHAU_1 = productInsert.CHIEC_KHAU_1;
            sp.CHIEC_KHAU_2 = productInsert.CHIEC_KHAU_2;
            sp.CHIEC_KHAU_3 = productInsert.CHIEC_KHAU_3;
            sp.CO_SO_TOI_THIEU = productInsert.CO_SO_TOI_THIEU;
            sp.CO_SO_TOI_DA = productInsert.CO_SO_TOI_DA;
            //common fields
            sp.ACTIVE = "A";
            sp.UPDATE_AT = DateTime.Now;
            sp.CREATE_AT = DateTime.Now;
            sp.UPDATE_BY = (int)Session["UserId"];
            sp.CREATE_BY = (int)Session["UserId"];

            db.SAN_PHAM.Add(sp);
            db.SaveChanges();
            return Redirect("Index");
        }

        #region Common function


        private void BindListDV(SmsContext ctx)
        {
            var listDV = new List<DON_VI_TINH>();
            listDV.Add(new DON_VI_TINH { MA_DON_VI = -1, TEN_DON_VI = "Chọn đơn vị tính" });
            var dsDonVi = (from s in ctx.DON_VI_TINH select s).ToList<DON_VI_TINH>();
            if (null != dsDonVi)
            {
                listDV.AddRange(dsDonVi);
            }
            ViewBag.dsDonVi = listDV;

        }

        private void BindListNhomSP(SmsContext ctx)
        {
            var listNhomSP = new List<NHOM_SAN_PHAM>();
            listNhomSP.Add(new NHOM_SAN_PHAM { MA_NHOM = -1, TEN_NHOM = "Chọn nhóm sản phẩm" });
            var dsNhomSP = (from s in ctx.NHOM_SAN_PHAM select s).ToList<NHOM_SAN_PHAM>();
            if (null != dsNhomSP)
            {
                listNhomSP.AddRange(dsNhomSP);
            }
            ViewBag.dsNhomSP = listNhomSP;

        }

        private void BindListNSX(SmsContext ctx)
        {
            var listNSX = new List<NHA_SAN_XUAT>();
            listNSX.Add(new NHA_SAN_XUAT { MA_NHA_SAN_XUAT = -1, TEN_NHA_SAN_XUAT = "Chọn nhà sản xuất" });
            var dsNSX = from s in ctx.NHA_SAN_XUAT select s;
            if (dsNSX != null && dsNSX.Count() > 0)
            {
                listNSX.AddRange(dsNSX);
            }
            ViewBag.dsNSX = listNSX;

        }
        private void SetHiddenFields(SAN_PHAM sp)
        {
            if (sp != null)
            {
                ViewBag.CanNang = sp.CAN_NANG;
                ViewBag.DVSelected = sp.MA_DON_VI;
                ViewBag.NSXSelected = sp.MA_NHA_SAN_XUAT;
                ViewBag.GiaBan1 = sp.GIA_BAN_1;
                ViewBag.GiaBan2 = sp.GIA_BAN_2;
                ViewBag.GiaBan3 = sp.GIA_BAN_3;
                ViewBag.ChietKhau1 = sp.CHIEC_KHAU_1;
                ViewBag.ChietKhau2 = sp.CHIEC_KHAU_2;
                ViewBag.ChietKhau3 = sp.CHIEC_KHAU_3;
                ViewBag.CoSoMin = sp.CO_SO_TOI_THIEU;
                ViewBag.CoSoMax = sp.CO_SO_TOI_DA;
            }

        }
        private void SetModeTitle(bool isModeUpdate)
        {
            if (isModeUpdate)
            {
                ViewBag.Title = "Cập nhật sản phẩm";
                ViewBag.Mode = "UPDATE";
            }
            else
            {
                ViewBag.Title = "Thêm mới sản phẩm";
                ViewBag.Mode = "CREATE";
            }
        }

        private void SetDefaultValue()
        {
            ViewBag.CanNang = "0";
            ViewBag.GiaBan1 = "0";
            ViewBag.GiaBan2 = "0";
            ViewBag.GiaBan3 = "0";
            ViewBag.ChietKhau1 = "0";
            ViewBag.ChietKhau2 = "0";
            ViewBag.ChietKhau3 = "0";
            ViewBag.CoSoMin = "0";
            ViewBag.CoSoMax = "0";
        }
        private IPagedList<SanPhamDisplay> GetListProductNotSearchAdvance(string sortOrder, string CurrentFilter, int? currentPageIndex , FormCollection form)
        {

            int pageSize = SystemConstant.ROWS;

            ViewBag.CurrentSort = sortOrder;

            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            int maNhom = 0;
            if (form.AllKeys.Contains("productGroupId"))
            {
                int.TryParse(form.Get("productGroupId"), out maNhom);
            }

            var ctx = new SmsContext();

            var contentLst = (from s in ctx.SAN_PHAM
                              where (
                              s.ACTIVE == "A"
                              && (maNhom == 0 || s.MA_NHOM == maNhom)
                              && (String.IsNullOrEmpty(CurrentFilter)
                              || s.TEN_SAN_PHAM.ToUpper().Contains(CurrentFilter.ToUpper())
                              || s.DAC_TA.ToUpper().Contains(CurrentFilter.ToUpper())
                              || s.KICH_THUOC.ToUpper().Contains(CurrentFilter.ToUpper())))
                              join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                              join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG
                              join dv in ctx.DON_VI_TINH on s.MA_DON_VI equals dv.MA_DON_VI into dv_join
                              from dv in dv_join.DefaultIfEmpty()
                              join dv in ctx.NHA_SAN_XUAT on s.MA_NHA_SAN_XUAT equals dv.MA_NHA_SAN_XUAT into nsx_join
                              from nsx in nsx_join.DefaultIfEmpty()
                              join nsp in ctx.NHOM_SAN_PHAM on s.MA_NHOM equals nsp.MA_NHOM into nsp_join
                              from nsp in nsp_join.DefaultIfEmpty()
                              select new SanPhamDisplay
                              {
                                  SanPham = s,
                                  NguoiTao = u,
                                  NguoiCapNhat = u1,
                                  DonVi = dv,
                                  NhaSanXuat = nsx,
                                  NhomSanPham = nsp
                              }).Take(SystemConstant.MAX_ROWS);

            IPagedList<SanPhamDisplay> DisplayContentLst = null;

            int pageIndex = (currentPageIndex ?? 1);
            ViewBag.Count = contentLst.Count();
            switch (sortOrder)
            {
                case "id_desc":
                    DisplayContentLst = contentLst.OrderByDescending(u => u.SanPham.MA_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    DisplayContentLst = contentLst.OrderBy(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    DisplayContentLst = contentLst.OrderByDescending(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    DisplayContentLst = contentLst.OrderBy(u => u.SanPham.MA_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
            }
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.CurrentFilter = CurrentFilter;
            ViewBag.DisplayContentLst = DisplayContentLst;
            ctx.Dispose();
            return DisplayContentLst;
        }

        private IPagedList<SanPhamDisplay> GetListProductSearchAdvance(string sortOrder, ProductSA psa, int? currentPageIndex)
        {
            int pageSize = SystemConstant.ROWS;

            ViewBag.CurrentSort = sortOrder;

            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";

            double weightFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.TrongLuongFrom) ? "0" : psa.TrongLuongFrom.Replace(",", ""), out weightFrom);
            double weightTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.TrongLuongTo) ? "0" : psa.TrongLuongTo.Replace(",", ""), out weightTo);
            double priceFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.GiaBanFrom) ? "0" : psa.GiaBanFrom.Replace(",", ""), out priceFrom);
            double priceTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.GiaBanTo) ? "0" : psa.GiaBanTo.Replace(",", ""), out priceTo);
            double discountFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.ChietKhauFrom) ? "0" : psa.ChietKhauFrom.Replace(",", ""), out discountFrom);
            double discountTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.ChietKhauTo) ? "0" : psa.ChietKhauTo.Replace(",", ""), out discountTo);
            double amoutFrom = 0;
            double.TryParse(string.IsNullOrEmpty(psa.CoSoFrom) ? "0" : psa.CoSoFrom.Replace(",", ""), out amoutFrom);
            double amountTo = 0;
            double.TryParse(string.IsNullOrEmpty(psa.CoSoTo) ? "0" : psa.CoSoTo.Replace(",", ""), out amountTo);

            var ctx = new SmsContext();
            var contentLst = (from s in ctx.SAN_PHAM
                              where (s.ACTIVE == "A"
                                && (String.IsNullOrEmpty(psa.TenSanPham)
                                || s.TEN_SAN_PHAM.ToUpper().Contains(psa.TenSanPham.ToUpper()))
                                && (String.IsNullOrEmpty(psa.KichThuoc)
                                || s.KICH_THUOC.ToUpper().Contains(psa.KichThuoc.ToUpper()))
                               
                                && (String.IsNullOrEmpty(psa.TrongLuongFrom)
                                || s.CAN_NANG >= weightFrom)
                                && (String.IsNullOrEmpty(psa.TrongLuongTo)
                                || s.CAN_NANG <= weightTo)
                               
                                && (String.IsNullOrEmpty(psa.DonViTinh)
                                || s.DON_VI_TINH.TEN_DON_VI.ToUpper().Contains(psa.DonViTinh.ToUpper()))

                                  && (String.IsNullOrEmpty(psa.NhomSanPham)
                                || s.NHOM_SAN_PHAM.TEN_NHOM.ToUpper().Contains(psa.NhomSanPham.ToUpper()))
                               
                                && (String.IsNullOrEmpty(psa.NhaSanXuat)
                                || s.NHA_SAN_XUAT.TEN_NHA_SAN_XUAT.ToUpper().Contains(psa.NhaSanXuat.ToUpper()))
                               
                                && (String.IsNullOrEmpty(psa.DacTa)
                                || s.DAC_TA.ToUpper().Contains(psa.DacTa.ToUpper()))

                                && (String.IsNullOrEmpty(psa.GiaBanFrom)
                                || s.GIA_BAN_1 >= priceFrom || s.GIA_BAN_2 >= priceFrom || s.GIA_BAN_3 >= priceFrom)
                                && (String.IsNullOrEmpty(psa.GiaBanTo)
                                || s.GIA_BAN_1 <= priceTo || s.GIA_BAN_2 <= priceTo || s.GIA_BAN_3 <= priceTo)

                                && (String.IsNullOrEmpty(psa.ChietKhauFrom)
                                || s.CHIEC_KHAU_1 >= discountFrom || s.CHIEC_KHAU_2 >= discountFrom || s.CHIEC_KHAU_3 >= discountFrom)
                                && (String.IsNullOrEmpty(psa.ChietKhauTo)
                                || s.CHIEC_KHAU_1 <= discountTo || s.CHIEC_KHAU_2 <= discountTo || s.CHIEC_KHAU_3 <= discountTo)

                                && (String.IsNullOrEmpty(psa.CoSoFrom)
                                || s.CO_SO_TOI_THIEU >= amoutFrom)
                                && (String.IsNullOrEmpty(psa.CoSoTo)
                                || s.CO_SO_TOI_DA <= amountTo)
                              )
                              join u in ctx.NGUOI_DUNG on s.CREATE_BY equals u.MA_NGUOI_DUNG
                              join u1 in ctx.NGUOI_DUNG on s.CREATE_BY equals u1.MA_NGUOI_DUNG

                              join dv in ctx.DON_VI_TINH on s.MA_DON_VI equals dv.MA_DON_VI into dv_join
                              from dv in dv_join.DefaultIfEmpty()

                              join nsp in ctx.NHOM_SAN_PHAM on s.MA_NHOM equals nsp.MA_NHOM into nsp_join
                              from nsp in nsp_join.DefaultIfEmpty()

                              join dv in ctx.NHA_SAN_XUAT on s.MA_NHA_SAN_XUAT equals dv.MA_NHA_SAN_XUAT into nsx_join
                              from nsx in nsx_join.DefaultIfEmpty()
                              select new SanPhamDisplay
                              {
                                  SanPham = s,
                                  NguoiTao = u,
                                  NguoiCapNhat = u1,
                                  DonVi = dv,
                                  NhaSanXuat = nsx,
                                  NhomSanPham = nsp

                              }).Take(SystemConstant.MAX_ROWS);


            IPagedList<SanPhamDisplay> DisplayContentLst = null;

            int pageIndex = (currentPageIndex ?? 1);
            ViewBag.Count = contentLst.Count();
            switch (sortOrder)
            {
                case "id_desc":
                    DisplayContentLst = contentLst.OrderByDescending(u => u.SanPham.MA_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    DisplayContentLst = contentLst.OrderBy(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    DisplayContentLst = contentLst.OrderByDescending(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    DisplayContentLst = contentLst.OrderBy(u => u.SanPham.MA_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
            }

            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.CurrentFilterAdvance = psa;
            ViewBag.DisplayContentLst = DisplayContentLst;
            ctx.Dispose();
            return DisplayContentLst;
        }


        private object removeCommaInput(object value)
        {
            if (value != null && value.ToString() != null && value.ToString().Contains(",") == true)
            {
                value = value.ToString().Replace(",", "").Trim();
                return value;
            }
            return null;
        }

        #endregion

        [HttpGet]
        public ActionResult Warning(string SearchString, int? page, bool? flag)
        {
           
            /*IPagedList<SP_GET_TON_KHO_ALERT> tk = null;
            ViewBag.CurrentFilter = SearchString;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;

            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            var tonkho = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", 
                new SqlParameter("NAME", string.IsNullOrEmpty(SearchString)
                    ? "" : SearchString.Trim())).ToList<SP_GET_TON_KHO_ALERT>().Take(SystemConstant.MAX_ROWS); ;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = tonkho.Count();
            tk = tonkho.ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.KhoList = ListKho;
            ViewBag.tonKho = tk;
            GetTonKhoAlertModel model = new GetTonKhoAlertModel();
            model.WarningList = tk;*/
            return View();
        }

        [CustomActionFilter]
        public ActionResult downloadWarningList()
        {
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var details = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", 
                new SqlParameter("NAME", string.Empty)).ToList<SP_GET_TON_KHO_ALERT>();
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename= " + fileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("<table border=2><tr><td bgcolor='#6495ED' style=\"font-size:18px; font-family:'Times New Roman'\" align='center' colspan='5'> SẢN PHẨM CẦN NHẬP KHO </td></tr>");
            fileStringBuilder.Append("<tr>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> STT </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Mã sản phẩm </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tên sản phẩm </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tên đơn vị</td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Số lượng </td>");
            fileStringBuilder.Append("</tr>");
            int i = 0;
            foreach (var detail in details)
            {
                i++;
                fileStringBuilder.Append("<tr>");
                fileStringBuilder.Append("<td align='center' > " + i + " </td>");
                fileStringBuilder.Append("<td align='center' > " + detail.MA_SAN_PHAM + " </td>");
                fileStringBuilder.Append("<td align='center' > " + detail.TEN_SAN_PHAM + " </td>");
                fileStringBuilder.Append("<td align='center' > " + detail.TEN_DON_VI + " </td>");
                fileStringBuilder.Append("<td align='center' > </td>");
                fileStringBuilder.Append("</tr>");
            }
            fileStringBuilder.Append("</table>");

            Response.Output.Write(fileStringBuilder.ToString());
            Response.Flush();
            Response.End();
            ctx.Dispose();
            return View("../Sanpham/Warning");
        }

        [HttpPost]
        public PartialViewResult PagingContentWarning(string SearchString, int? currentPageIndex)
        {

            IPagedList<SP_GET_TON_KHO_ALERT> tk = null;

            ViewBag.SearchString = SearchString;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;

            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            // var watch = Stopwatch.StartNew();
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            //string s = ctx.Database.Connection.ToString();
            var tonkho = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", new SqlParameter("NAME", string.IsNullOrEmpty(SearchString) ? "" : SearchString.Trim())).ToList<SP_GET_TON_KHO_ALERT>().Take(SystemConstant.MAX_ROWS); ;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = tonkho.Count();
            tk = tonkho.ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.KhoList = ListKho;
            ViewBag.tonKho = tk;
            GetTonKhoAlertModel model = new GetTonKhoAlertModel();
            model.WarningList = tk;
            ctx.Dispose();
            return PartialView("WarningPartialView", model);
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult Warning(string SearchString, int? page)
        {
            IPagedList<SP_GET_TON_KHO_ALERT> tk = null;
            ViewBag.CurrentFilter = SearchString;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;

            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var ListKho = ctx.KHOes.Where(u => u.ACTIVE.Equals("A")).ToList();
            var tonkho = ctx.Database.SqlQuery<SP_GET_TON_KHO_ALERT>("exec SP_GET_TON_KHO_ALERT @NAME ", new SqlParameter("NAME", string.IsNullOrEmpty(SearchString) ? "" : SearchString.Trim())).ToList<SP_GET_TON_KHO_ALERT>().Take(SystemConstant.MAX_ROWS); ;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = tonkho.Count();
            tk = tonkho.ToList().ToPagedList(pageIndex, pageSize);
            ViewBag.KhoList = ListKho;
            ViewBag.tonKho = tk;
            GetTonKhoAlertModel model = new GetTonKhoAlertModel();
            model.WarningList = tk;
            ctx.Dispose();
            return View(model);
        }


        /*** CONVERT UNIT START **/
        [CustomActionFilter]
        [HttpGet]
        public ActionResult ConvertUnitOfProducts(string inforMessage, string message)
        {
            ViewBag.InforMessage = inforMessage;
            ViewBag.Message = message;
            var ctx = new SmsContext();
            var productGroups = ctx.NHOM_SAN_PHAM.Where(u => u.ACTIVE == "A").ToList<NHOM_SAN_PHAM>();
            ViewBag.ProductGroups = productGroups;
            ctx.Dispose();
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public PartialViewResult ConvertUnitOfProducts(int? productGroupId, string sortOrder, string CurrentFilter, int? currentPageIndex)
        {
             int pageSize = SystemConstant.ROWS;

            ViewBag.CurrentSort = sortOrder;

            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.ProductGroupId = productGroupId;
            var ctx = new SmsContext();
            var theListContext = (from cd in ctx.CHUYEN_DOI_DON_VI_TINH                                  
                                  join sp in ctx.SAN_PHAM on cd.MA_SAN_PHAN equals sp.MA_SAN_PHAM
                                  join gr in ctx.NHOM_SAN_PHAM on sp.MA_NHOM equals gr.MA_NHOM
                                  join dv1 in ctx.DON_VI_TINH on sp.MA_DON_VI equals dv1.MA_DON_VI
                                  join dv2 in ctx.DON_VI_TINH on cd.MA_DON_VI_VAO equals dv2.MA_DON_VI
                                  join u in ctx.NGUOI_DUNG on cd.CREATE_BY equals u.MA_NGUOI_DUNG
                                  join u1 in ctx.NGUOI_DUNG on cd.UPDATE_BY equals u1.MA_NGUOI_DUNG
                                  where
                                  (cd.ACTIVE == "A"
                                  && (productGroupId == null || productGroupId == 0 || gr.MA_NHOM == productGroupId)
                                  && (string.IsNullOrEmpty(CurrentFilter) 
                                    || sp.TEN_SAN_PHAM.ToLower().Contains(CurrentFilter.Trim().ToLower())
                                    || dv1.TEN_DON_VI.ToLower().Contains(CurrentFilter.Trim().ToLower())
                                    || dv2.TEN_DON_VI.ToLower().Contains(CurrentFilter.Trim().ToLower())))
                                  select new ChuyenDoiDonViTinhModel
                                  {
                                      ChuyenDoiDonVi = cd,
                                      SanPham = sp,
                                      DonViCuoi = dv1,
                                      DonViVao = dv2,
                                      NguoiCapNhat = u1,
                                      NguoiTao = u
                                  }).ToList<ChuyenDoiDonViTinhModel>();
            IPagedList<ChuyenDoiDonViTinhModel> khachHangs = null;
           
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ViewBag.CurrentPageIndex = pageIndex;
            ViewBag.Count = theListContext.Count();
       
            switch (sortOrder)
            {
                case "id_desc":
                    khachHangs = theListContext.OrderByDescending(u => u.ChuyenDoiDonVi.MA_CHUYEN_DOI).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    khachHangs = theListContext.OrderBy(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    khachHangs = theListContext.OrderByDescending(u => u.SanPham.TEN_SAN_PHAM).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    khachHangs = theListContext.OrderBy(u => u.ChuyenDoiDonVi.MA_CHUYEN_DOI).ToPagedList(pageIndex, pageSize);
                    break;
            }

            ViewBag.CurrentFilter = CurrentFilter;

            ctx.Dispose();
            return PartialView("ListConvertUnitPartialView", khachHangs);
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult AddNewConvertUnitOfProducts()
        {
            SetModeUnitTitle(false);
            ViewBag.HeSo = "2";
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult AddNewConvertUnitOfProducts(SMS.Models.CHUYEN_DOI_DON_VI_TINH convertUnitInsert)
        {
            var ctx = new SmsContext();
            var conUnit = ctx.CHUYEN_DOI_DON_VI_TINH.Create();
            conUnit.MA_SAN_PHAN = convertUnitInsert.MA_SAN_PHAN;
            conUnit.MA_DON_VI_VAO = convertUnitInsert.MA_DON_VI_VAO;
            conUnit.HE_SO = convertUnitInsert.HE_SO;
            conUnit.CHIEC_KHAU_1 = convertUnitInsert.CHIEC_KHAU_1;
            conUnit.GIA_BAN_1 = convertUnitInsert.GIA_BAN_1;
            conUnit.CHIEC_KHAU_2 = convertUnitInsert.CHIEC_KHAU_2;
            conUnit.GIA_BAN_2 = convertUnitInsert.GIA_BAN_2;
            conUnit.CHIEC_KHAU_3 = convertUnitInsert.CHIEC_KHAU_3;
            conUnit.GIA_BAN_3 = convertUnitInsert.GIA_BAN_3;
            conUnit.ACTIVE = "A";
            conUnit.UPDATE_AT = DateTime.Now;
            conUnit.CREATE_AT = DateTime.Now;
            conUnit.UPDATE_BY = (int)Session["UserId"];
            conUnit.CREATE_BY = (int)Session["UserId"];

            ctx.CHUYEN_DOI_DON_VI_TINH.Add(conUnit);
            ctx.SaveChanges();
            ctx.Dispose();
            return Redirect("ConvertUnitOfProducts");
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult EditConvertUnitOfProducts(int id)
        {
            string msg = "Không tìm thấy chuyển đổi đơn vị tương ứng.";
            if (id <= 0)
            {
                ViewBag.Message = msg;
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            CHUYEN_DOI_DON_VI_TINH cddv = ctx.CHUYEN_DOI_DON_VI_TINH.Find(id);
            if (cddv.ACTIVE.Equals("A"))
            {
                SetModeUnitTitle(true);

                SetHiddenConvertUnitFields(cddv);
                ctx.Dispose();
                return View("../SanPham/AddNewConvertUnitOfProducts", cddv);

            }
            else
            {
                ctx.Dispose();
                ViewBag.Message = msg;
                return View("../Home/Error"); ;
            }

        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult EditConvertUnitOfProducts(SMS.Models.CHUYEN_DOI_DON_VI_TINH convertUnitUpdated)
        {
            var db = new SmsContext();
            if (ModelState.IsValid)
            {
                var cddv = db.CHUYEN_DOI_DON_VI_TINH.Find((int)convertUnitUpdated.MA_CHUYEN_DOI);
                cddv.MA_SAN_PHAN = convertUnitUpdated.MA_SAN_PHAN;
                cddv.MA_DON_VI_VAO = convertUnitUpdated.MA_DON_VI_VAO;
                cddv.HE_SO = convertUnitUpdated.HE_SO;
                cddv.CHIEC_KHAU_1 = convertUnitUpdated.CHIEC_KHAU_1;
                cddv.CHIEC_KHAU_2 = convertUnitUpdated.CHIEC_KHAU_2;
                cddv.CHIEC_KHAU_3 = convertUnitUpdated.CHIEC_KHAU_3;
                cddv.GIA_BAN_1 = convertUnitUpdated.GIA_BAN_1;
                cddv.GIA_BAN_2 = convertUnitUpdated.GIA_BAN_2;
                cddv.GIA_BAN_3 = convertUnitUpdated.GIA_BAN_3;
                //common fields
                cddv.ACTIVE = "A";
                cddv.UPDATE_AT = DateTime.Now;
                cddv.UPDATE_BY = (int)Session["UserId"];

                db.SaveChanges();
                db.Dispose();
                return RedirectToAction("ConvertUnitOfProducts");
            }

            return View();
        }

        private void SetModeUnitTitle(bool isModeUpdate)
        {
            if (isModeUpdate)
            {
                ViewBag.Title = "Cập nhật chuyển đổi đơn vị";
                ViewBag.ModeUnit = "UPDATE";
            }
            else
            {
                ViewBag.Title = "Thêm mới chuyển đổi đơn vị";
                ViewBag.ModeUnit = "CREATE";
            }
        }
        private void SetHiddenConvertUnitFields(CHUYEN_DOI_DON_VI_TINH cddv)
        {
            if (cddv != null)
            {
                var ctx = new SmsContext();
                SAN_PHAM sp = ctx.SAN_PHAM.Find(cddv.MA_SAN_PHAN);
                ViewBag.ProductName = sp.TEN_SAN_PHAM ;
                DON_VI_TINH dv = ctx.DON_VI_TINH.Find(cddv.MA_DON_VI_VAO);
                ViewBag.InputUnitName = dv.TEN_DON_VI ;
                ViewBag.HeSo = cddv.HE_SO;
                DON_VI_TINH dvroot = ctx.DON_VI_TINH.Find(sp.MA_DON_VI);
                ctx.Dispose();
                ViewBag.UnitName = dvroot.TEN_DON_VI;
            }

        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult DeleteConvertUnit(int id)
        {
            string msg = "Không tìm thấy chuyển đổi đơn vị tương ứng.";
            if (id <= 0)
            {
                ViewBag.Message = msg;
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            CHUYEN_DOI_DON_VI_TINH cddv = ctx.CHUYEN_DOI_DON_VI_TINH.Find(id);
            if (cddv.ACTIVE.Equals("A"))
            {
                cddv.ACTIVE = "I";
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("ConvertUnitOfProducts");
            }
            else
            {
                ViewBag.Message = msg;
                ctx.Dispose();
                return View("../Home/Error"); ;
            }
        }
        /*** CONVERT UNIT END **/
    }

}