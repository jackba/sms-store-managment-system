using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using PagedList;
using SMS.App_Start;
using System.Transactions;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web.UI;
using CsvHelper;
using SMS.App_Start;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]  
    public class ImportController : Controller
    {
        [CustomActionFilter]
        public ActionResult showDetail(int id)
        {
            return View();
        }
     
        [HttpPost]
        public PartialViewResult showDetailPartialView(int id)
        {
            return PartialView();
        }

        
        public bool checkNumber(string str)
        {
            try
            {
                double outNumber;
                double.TryParse(str, out outNumber);
                if (outNumber == (double)0)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool checkInteger(string str)
        {
            try
            {
                int outNumber;
                int.TryParse(str, out outNumber);
                if (outNumber == (int)0)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult importCsv(HttpPostedFileBase file, ImportCsvModel model)
        {
            var ctx = new SmsContext();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var storeId = model.StoreId;
            string ImportNo = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            if (file != null)
            {
                var fp = Path.Combine(HttpContext.Server.MapPath("~/ImportUploads"), Path.GetFileName(file.FileName));
                file.SaveAs(fp);

                ICsvParser csvParser = new CsvParser(new StreamReader(file.InputStream));
                CsvReader csvReader = new CsvReader(csvParser);
                string[] headers = { };
                List<string[]> rows = new List<string[]>();
                string[] row;
                while (csvReader.Read())
                {
                    if (csvReader.FieldHeaders != null && csvReader.FieldHeaders.Length != 7)
                    {
                        ViewBag.Message = "Định dạng file CSV không đúng. Vui lòng kiểm tra lại.";
                        model.Stores = stores;
                        model.StoreId = storeId;
                        return View(model);
                    }
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
                ImportCsvFile record;
                List<ImportCsvFile> thelist = new List<ImportCsvFile>();
                bool flag = true;
                int i =1;
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        ctx.Database.CommandTimeout = 300;

                        var import = ctx.NHAP_KHO.Create();
                        import.NGAY_NHAP = model.ImportDate;
                        import.NHAN_VIEN_NHAP = Convert.ToInt32(Session["UserId"]);
                        import.MA_KHO = model.StoreId;
                        import.LY_DO_NHAP = 2;
                        import.SO_HOA_DON = ImportNo;
                        import.CREATE_AT = DateTime.Now;
                        import.UPDATE_AT = DateTime.Now;
                        import.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                        import.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                        import.ACTIVE = "A";
                        ctx.NHAP_KHO.Add(import);
                        ctx.SaveChanges();

                        var importHist = ctx.KIEM_KHO_HISTORY.Create();
                        importHist.MA_NHAP_KHO = import.MA_NHAP_KHO;
                        importHist.NGAY_KIEM_KHO = model.ImportDate;
                        importHist.CREATE_AT = DateTime.Now;
                        importHist.UPDATE_AT = DateTime.Now;
                        importHist.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                        importHist.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                        importHist.ACTIVE = "A";
                        ctx.KIEM_KHO_HISTORY.Add(importHist);
                        ctx.SaveChanges();

                        CHI_TIET_NHAP_KHO detail;
                        foreach (var r in rows)
                        {
                            i++;
                            if (string.IsNullOrWhiteSpace(r[1]) || string.IsNullOrWhiteSpace(r[5]) || string.IsNullOrWhiteSpace(r[6]))
                            {
                                if (!flag)
                                {
                                    fileStringBuilder.Append("<br>");
                                }
                                flag = false;
                                fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i);
                            }
                            else
                            {
                                if (!checkInteger(r[0].ToString())
                                    || !checkNumber(r[5].ToString())
                                    || !checkNumber(r[6].ToString()))
                                {
                                    if (!flag)
                                    {
                                        fileStringBuilder.Append("<br>");
                                    }
                                    flag = false;
                                    fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng, kiểm tra lại mã sản phẩm, số lượng và đơn giá." + i);
                                }
                                else
                                {
                                    record = new ImportCsvFile();
                                    record.No = r[0].ToString();
                                    record.Id = r[1].ToString();
                                    record.Code = r[2].ToString();
                                    record.Name = r[3].ToString();
                                    record.UniName = r[4].ToString();
                                    record.Quantity = r[5].ToString();
                                    record.Price = r[6].ToString();
                                    

                                    detail = ctx.CHI_TIET_NHAP_KHO.Create();
                                    detail.MA_NHAP_KHO = import.MA_NHAP_KHO;
                                    detail.MA_SAN_PHAM = Convert.ToInt32(record.Id);
                                    detail.SO_LUONG = Convert.ToDouble(record.Quantity);
                                    detail.GIA_VON = Convert.ToDouble(record.Price);
                                    detail.CREATE_AT = DateTime.Now;
                                    detail.UPDATE_AT = DateTime.Now;
                                    detail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                                    detail.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                                    detail.ACTIVE = "A";
                                    ctx.CHI_TIET_NHAP_KHO.Add(detail);
                                    ctx.SaveChanges();

                                    thelist.Add(record);
                                }
                            }
                        }
                        if (flag)
                        {
                            ViewBag.InforMessage = "Nhập kiểm kho thành công. Vui lòng xem chi tiếc bên dưới.";
                            transaction.Complete();
                        }
                        else
                        {
                            Transaction.Current.Rollback();
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Transaction.Current.Rollback();
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình nhập kho. Vui lòng thực hiện lại.";
                    }
                    
                }
                if (flag)
                {
                    model.TheList = thelist;
                    model.Count = thelist.Count();
                }
                else
                {
                    ViewBag.Message = fileStringBuilder.ToString();
                }                            
            }
            model.Stores = stores;
            model.StoreId = storeId;
            return View(model);
        }

        [CustomActionFilter]
        public ActionResult importCsv(string message, string inforMessage)
        {
            ImportCsvModel model = new ImportCsvModel();
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            if (!(bool)Session["IsAdmin"])
            {
                model.StoreId = Convert.ToInt32(Session["MyStore"]);
            }else
            {
                model.StoreId = 0;
            }
            model.Stores = stores;
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            return View(model);
        }

        [CustomActionFilter]
        public ActionResult downloadCSVTemplate()
        {
            Response.ClearContent();
            Response.Buffer = true;
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            Response.AddHeader("content-disposition", "attachment; filename= " + fileName + ".csv");
            Response.ContentType = "text/csv";
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"Mã sản phẩm\",");
            fileStringBuilder.Append("\"CODE\","); 
            fileStringBuilder.Append("\"Tên sản phẩm\",");
            fileStringBuilder.Append("\"Đơn vị tính\",");
            fileStringBuilder.Append("\"Số lượng\",");
            fileStringBuilder.Append("\"Giá vốn\"");            
            var ctx = new SmsContext();

            var products = ctx.SAN_PHAM.Include("DON_VI_TINH").Where(u => u.ACTIVE == "A").ToList<SAN_PHAM>();

            int i = 0;
            foreach (var product in products)
            {
                fileStringBuilder.Append("\n");
                i += 1;
                fileStringBuilder.Append("\"" + i + "\",");
                fileStringBuilder.Append("\"" + product.MA_SAN_PHAM + "\",");
                fileStringBuilder.Append("\"" + (string.IsNullOrEmpty(product.CODE) ? "" : product.CODE).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (string.IsNullOrEmpty(product.TEN_SAN_PHAM) ? "" : product.TEN_SAN_PHAM).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\"" + (string.IsNullOrEmpty(product.DON_VI_TINH.TEN_DON_VI) ? "" : product.DON_VI_TINH.TEN_DON_VI).Replace("\"", "\"\"") + "\",");
                fileStringBuilder.Append("\" \",");
                fileStringBuilder.Append("\" \"");   
            }
            Response.Output.Write(fileStringBuilder.ToString());
            Response.Flush();
            Response.End();
            return View("../Home/Import");
        }

        [CustomActionFilter]
        public ActionResult downloadTemplateExcel()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("<table border=2><tr><td bgcolor='#00FFFF' style=\"font-size:18px; font-family:'Times New Roman'\" align='center' colspan='7'> Kiểm kho định kỳ </td></tr>");
            fileStringBuilder.Append("<td align='center' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> STT </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> Mã sản phẩm </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> CODE </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> Tên sản phẩm </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> Đơn vị tính </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> Số lượng</td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> Giá vốn </td></tr>");
            
            var ctx = new SmsContext();
            var products = ctx.SAN_PHAM.Include("DON_VI_TINH").Where(u => u.ACTIVE == "A").ToList<SAN_PHAM>();
            int i = 0;
            foreach (var product in products)
            {
                i += 1;
                fileStringBuilder.Append("<tr>");
                fileStringBuilder.Append("<td align='center'  style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> " + i + " </td>");
                fileStringBuilder.Append("<td align='center'  style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> " + product.MA_SAN_PHAM + " </td>");
                fileStringBuilder.Append("<td align='center'  style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> " + product.CODE + " </td>");
                fileStringBuilder.Append("<td align='left'  style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> " + product.TEN_SAN_PHAM + " </td>");
                fileStringBuilder.Append("<td align='center'  style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'> " + product.DON_VI_TINH.TEN_DON_VI + " </td>");
                fileStringBuilder.Append("<td align='center'  style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'></td>");
                fileStringBuilder.Append("<td align='center'  style=\"font-size:18px; font-family:'Times New Roman'\" colspan='1'></td>");
                fileStringBuilder.Append("</tr>");
            }
            
            fileStringBuilder.Append("</table>");
            Response.Output.Write(fileStringBuilder.ToString());
            Response.Flush();
            Response.End();
            return View("../Home/Import");
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult EditAdjustment(EditImportModel model)
        {
            var ctx = new SmsContext();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var infor = ctx.NHAP_KHO.Find(model.Infor.MA_NHAP_KHO);
                    if (infor == null || infor.ACTIVE == "I")
                    {
                        return RedirectToAction("Index", new { @message = "Phiếu nhập hàng không tồn tại, hoặc đã bị xóa. Vui lòng kiểm tra lại" });
                    }
                    if ((bool)Session["IsAdmin"])
                    {
                        infor.MA_KHO = model.Infor.MA_KHO;
                    }                    
                    infor.MA_NHA_CUNG_CAP = model.Infor.MA_NHA_CUNG_CAP;
                    infor.NGAY_NHAP = model.Infor.NGAY_NHAP;
                    infor.NHAN_VIEN_NHAP = Convert.ToInt32(Session["UserId"]);
                    infor.SO_HOA_DON = model.Infor.SO_HOA_DON;
                    infor.CREATE_AT = DateTime.Now;
                    infor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.ACTIVE = "A";
                    infor.GHI_CHU = model.Infor.GHI_CHU;
                    infor.LY_DO_NHAP = 4; // Nhập điều chỉnh
                    ctx.SaveChanges();

                    ctx.CHI_TIET_NHAP_KHO.RemoveRange(ctx.CHI_TIET_NHAP_KHO.Where(u => u.MA_NHAP_KHO == model.Infor.MA_NHAP_KHO));
                    CHI_TIET_NHAP_KHO importDetail;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            importDetail = ctx.CHI_TIET_NHAP_KHO.Create();
                            importDetail.ACTIVE = "A";
                            importDetail.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            importDetail.SO_LUONG_TEMP = detail.SO_LUONG;
                            importDetail.HE_SO = detail.HE_SO;
                            importDetail.SO_LUONG = detail.SO_LUONG * detail.HE_SO;
                            importDetail.MA_DON_VI = detail.MA_DON_VI;
                            importDetail.MA_NHAP_KHO = infor.MA_NHAP_KHO;
                            importDetail.CREATE_AT = DateTime.Now;
                            importDetail.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.UPDATE_AT = DateTime.Now;
                            importDetail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.GIA_VON = detail.DON_GIA / detail.HE_SO;
                            importDetail.DON_GIA_TEMP = detail.DON_GIA;
                            ctx.CHI_TIET_NHAP_KHO.Add(importDetail);
                            ctx.SaveChanges();
                        }
                    }
                    transaction.Complete();
                    return RedirectToAction("Index", new { @messageInfor = "Sửa phiếu nhập điều chĩnh thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("Index", new { @message = "Sửa phiếu nhập điều chĩnh thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        [CustomActionFilter]
        public ActionResult EditAdjustment(int id)
        {
            EditImportModel model = new EditImportModel();
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var providers = ctx.NHA_CUNG_CAP.Where(u => u.ACTIVE == "A").ToList<NHA_CUNG_CAP>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            ViewBag.Stores = stores;
            //if (!(bool)Session["IsAdmin"])
            //{
            //    model.Infor.MA_KHO = Convert.ToInt32(Session["MyStore"]);
            //}
            model.Stores = stores;
            model.Providers = providers;
            model.Units = units;
            var infor = ctx.SP_GET_IMPORT_INFOR_BY_ID(id).FirstOrDefault();
            var detail = ctx.SP_GET_IMPORT_DETAIL_BY_ID_4_EDIT(id).ToList<SP_GET_IMPORT_DETAIL_BY_ID_4_EDIT_Result>();
            model.Infor = infor;
            model.Detail = detail;
            return View(model);
        }

        [CustomActionFilter]
        public ActionResult Adjustment()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var providers = ctx.NHA_CUNG_CAP.Where(u => u.ACTIVE == "A").ToList<NHA_CUNG_CAP>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            ViewBag.Stores = stores;
            ImportModel model = new ImportModel();
            NHAP_KHO Infor = new NHAP_KHO();
            model.Infor = Infor;
            if (!(bool)Session["IsAdmin"])
            {
                model.Infor.MA_KHO = Convert.ToInt32(Session["MyStore"]);
            }

            model.Stores = stores;
            model.Providers = providers;
            model.Units = units;
            ViewBag.InputKind = -1;
            return View(model);
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult Adjustment(ImportModel model)
        {
            var ctx = new SmsContext();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var infor = ctx.NHAP_KHO.Create();
                    if ((bool)Session["IsAdmin"])
                    {
                        infor.MA_KHO = model.Infor.MA_KHO;
                    }
                    else
                    {
                        infor.MA_KHO = Convert.ToInt32(Session["MyStore"]);
                    }
                    infor.NGAY_NHAP = model.Infor.NGAY_NHAP;
                    infor.NHAN_VIEN_NHAP = Convert.ToInt32(Session["UserId"]);
                    infor.CREATE_AT = DateTime.Now;
                    infor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.ACTIVE = "A";
                    infor.GHI_CHU = model.Infor.GHI_CHU;
                    infor.LY_DO_NHAP = 4; // nhập mua hàng
                    ctx.NHAP_KHO.Add(infor);
                    ctx.SaveChanges();

                    CHI_TIET_NHAP_KHO importDetail;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            importDetail = ctx.CHI_TIET_NHAP_KHO.Create();
                            importDetail.ACTIVE = "A";
                            importDetail.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            importDetail.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            importDetail.HE_SO = detail.HE_SO;
                            importDetail.SO_LUONG = detail.SO_LUONG_TEMP * detail.HE_SO;
                            importDetail.MA_DON_VI = detail.MA_DON_VI;
                            importDetail.MA_NHAP_KHO = infor.MA_NHAP_KHO;
                            importDetail.CREATE_AT = DateTime.Now;
                            importDetail.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.UPDATE_AT = DateTime.Now;
                            importDetail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ctx.CHI_TIET_NHAP_KHO.Add(importDetail);
                            ctx.SaveChanges();
                        }
                    }
                    transaction.Complete();
                    return RedirectToAction("Index", new { @messageInfor = "Nhập điều chỉnh kho thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("Index", new { @message = "Nhập điều chỉnh kho thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        [CustomActionFilter]
        public ActionResult deleteTransfer(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy phiếu xuất kho tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.XUAT_KHO.Find(id);
            if (donvi.ACTIVE.Equals("A") || donvi.ACTIVE.Equals("W"))
            {

                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        donvi.ACTIVE = "I";
                        donvi.UPDATE_AT = DateTime.Now;
                        donvi.CREATE_BY = (int)Session["UserId"];
                        ctx.SaveChanges();

                        var details = ctx.CHI_TIET_XUAT_KHO.Where(u => u.ACTIVE == "A" && u.MA_XUAT_KHO == id).ToList<CHI_TIET_XUAT_KHO>();
                        foreach (var detail in details)
                        {
                            detail.ACTIVE = "I";
                            detail.UPDATE_AT = DateTime.Now;
                            detail.CREATE_BY = (int)Session["UserId"];
                            ctx.SaveChanges();
                        }
                        transaction.Complete();
                        return RedirectToAction("ListExportTransfer", new { @inforMessage = "Xóa phiếu xuất chuyển kho thành công." });
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                        Transaction.Current.Rollback();
                        return RedirectToAction("ListExportTransfer", new { @message = "Xóa phiếu xuất chuyển kho  thất bại." });
                    }
                }

            }
            else
            {
                ViewBag.Message = "Không tìm thấy  phiếu xuất chuyển kho tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [CustomActionFilter]
        public ActionResult deleteImport(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy phiếu nhập kho tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.NHAP_KHO.Find(id);
            if (donvi.ACTIVE.Equals("A"))
            {
                
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        donvi.ACTIVE = "I";
                        donvi.UPDATE_AT = DateTime.Now;
                        donvi.CREATE_BY = (int)Session["UserId"];
                        ctx.SaveChanges();
                        if (donvi.LY_DO_NHAP == 2)
                        {
                            var importHist = ctx.KIEM_KHO_HISTORY.Where(u => u.MA_NHAP_KHO == donvi.MA_NHAP_KHO && u.ACTIVE == "A").FirstOrDefault();
                            importHist.ACTIVE = "I";
                            importHist.UPDATE_AT = DateTime.Now;
                            importHist.CREATE_BY = (int)Session["UserId"];
                            ctx.SaveChanges();
                        }
                        var details = ctx.CHI_TIET_NHAP_KHO.Where(u => u.ACTIVE == "A" && u.MA_NHAP_KHO == id).ToList<CHI_TIET_NHAP_KHO>();
                        foreach (var detail in details)
                        {
                            detail.ACTIVE = "I";
                            detail.UPDATE_AT = DateTime.Now;
                            detail.CREATE_BY = (int)Session["UserId"];
                            ctx.SaveChanges();
                        }
                        transaction.Complete();
                        return RedirectToAction("Index", new { @messageInfor = "Xóa phiếu nhập kho thành công." });
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                        Transaction.Current.Rollback();
                        return RedirectToAction("Index", new { @message = "Xóa phiếu nhập kho thất bại." });
                    }
                }
                
            }
            else
            {
                ViewBag.Message = "Không tìm thấy phiếu nhập kho tương ứng.";
                return View("../Home/Error"); ;
            }
        }

        [HttpPost]
        public JsonResult getInventory(string storeId, string productId)
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
            var rv = returnValue.Value == DBNull.Value? 0: Convert.ToDouble(returnValue.Value);
            var result = Json(rv);
            return result;
        }

        //
        // GET: /Import/
        [CustomActionFilter]
        public ActionResult ListOfImportBill()
        {
            return View();
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult ImportTransfer(ImportTransferModel model)
        {
            var ctx = new SmsContext();
            if (model.Infor.STATUS == 3)
            {
                return RedirectToAction("ListWaitingImport", new { @message = "Phiếu chuyển đã được nhập kho, vui lòng kiểm tra lại" });
            }
            var StoreIdParam = new SqlParameter
            {
                ParameterName = "MA_KHO",
                Value = Convert.ToInt32(model.Infor.MA_KHO_NHAN)
            };

            var ImportDateParam = new SqlParameter
            {
                ParameterName = "NGAY_NHAP",
                Value = Convert.ToDateTime(model.ImportInfor.NGAY_NHAP)
            };

            var NoteParam = new SqlParameter
            {
                ParameterName = "GHI_CHU",
                Value = string.IsNullOrEmpty(model.ImportInfor.GHI_CHU) ? string.Empty : model.ImportInfor.GHI_CHU
            };

            var UserParam = new SqlParameter
            {
                ParameterName = "MA_NHAN_VIEN",
                Value = Convert.ToInt32(Session["UserId"])
            };

            var IdParam = new SqlParameter
            {
                ParameterName = "MA_PHIEU_CHUYEN",
                Value = Convert.ToInt32(model.Infor.MA_XUAT_KHO)
            };

            var returnValue = new SqlParameter
            {
                ParameterName = "RETURN_VALUE",
                Value = Convert.ToInt32(0),
                Direction = ParameterDirection.Output
            };


            var tonkho = ctx.Database.SqlQuery<Object>("exec SP_IMPORT_TRANSFER @MA_PHIEU_CHUYEN, @NGAY_NHAP, @MA_NHAN_VIEN, @MA_KHO, @GHI_CHU , @RETURN_VALUE OUT ",
                IdParam, ImportDateParam, UserParam, StoreIdParam, NoteParam, returnValue).ToList<Object>().Take(SystemConstant.MAX_ROWS);
            var rv = returnValue.Value == DBNull.Value ? 0 : Convert.ToDouble(returnValue.Value);
            if (rv > 0)
            {
                return RedirectToAction("ListWaitingImport", new { @inforMessage = "Lưu thành công" });
            }else
            {
                return RedirectToAction("ListWaitingImport", new { @message = "Nhập kho thất bại, vui lòng kiểm tra lại" });
            }
        }
        
        [CustomActionFilter]
        [HttpPost]
        public ActionResult Edit(EditImportModel model)
        {
            var ctx = new SmsContext();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var infor = ctx.NHAP_KHO.Find(model.Infor.MA_NHAP_KHO);
                    if (infor == null || infor.ACTIVE == "I")
                    {
                        return RedirectToAction("Index", new { @message = "Phiếu nhập hàng không tồn tại, hoặc đã bị xóa. Vui lòng kiểm tra lại" });
                    }
                    if ((bool)Session["IsAdmin"])
                    {
                        infor.MA_KHO = model.Infor.MA_KHO;
                    }                    
                    infor.MA_NHA_CUNG_CAP = model.Infor.MA_NHA_CUNG_CAP;
                    infor.NGAY_NHAP = model.Infor.NGAY_NHAP;
                    infor.NHAN_VIEN_NHAP = Convert.ToInt32(Session["UserId"]);
                    infor.SO_HOA_DON = model.Infor.SO_HOA_DON;
                    infor.CREATE_AT = DateTime.Now;
                    infor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.ACTIVE = "A";
                    infor.GHI_CHU = model.Infor.GHI_CHU;
                    infor.LY_DO_NHAP = 0; // nhập mua hàng
                    ctx.SaveChanges();

                    ctx.CHI_TIET_NHAP_KHO.RemoveRange(ctx.CHI_TIET_NHAP_KHO.Where(u => u.MA_NHAP_KHO == model.Infor.MA_NHAP_KHO));
                    CHI_TIET_NHAP_KHO importDetail;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            importDetail = ctx.CHI_TIET_NHAP_KHO.Create();
                            importDetail.ACTIVE = "A";
                            importDetail.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            importDetail.SO_LUONG_TEMP = detail.SO_LUONG;
                            importDetail.HE_SO = detail.HE_SO;
                            importDetail.SO_LUONG = detail.SO_LUONG * detail.HE_SO;
                            importDetail.MA_DON_VI = detail.MA_DON_VI;
                            importDetail.MA_NHAP_KHO = infor.MA_NHAP_KHO;
                            importDetail.CREATE_AT = DateTime.Now;
                            importDetail.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.UPDATE_AT = DateTime.Now;
                            importDetail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.GIA_VON = detail.DON_GIA / detail.HE_SO;
                            importDetail.DON_GIA_TEMP = detail.DON_GIA;
                            ctx.CHI_TIET_NHAP_KHO.Add(importDetail);
                            ctx.SaveChanges();
                        }
                    }
                    transaction.Complete();
                    return RedirectToAction("Index", new { @messageInfor = "Sửa hóa đơn mua hàng thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("Index", new { @message = "Sửa hóa đơn mua hàng thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        [CustomActionFilter]
        public ActionResult Edit(int id)
        {
            EditImportModel model = new EditImportModel();
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var providers = ctx.NHA_CUNG_CAP.Where(u => u.ACTIVE == "A").ToList<NHA_CUNG_CAP>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            ViewBag.Stores = stores;
            //if (!(bool)Session["IsAdmin"])
            //{
            //    model.Infor.MA_KHO = Convert.ToInt32(Session["MyStore"]);
            //}
            model.Stores = stores;
            model.Providers = providers;
            model.Units = units;
            var infor = ctx.SP_GET_IMPORT_INFOR_BY_ID(id).FirstOrDefault();
            var detail = ctx.SP_GET_IMPORT_DETAIL_BY_ID_4_EDIT(id).ToList<SP_GET_IMPORT_DETAIL_BY_ID_4_EDIT_Result>();
            model.Infor = infor;
            model.Detail = detail;
            return View(model);
        }

        [CustomActionFilter]
        public ActionResult ImportTransfer(int id)
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            var infor = ctx.SP_GET_PHIEU_CHUYEN_KHO_INFO_BY_ID(Convert.ToInt32(id)).FirstOrDefault();
            ImportTransferModel model = new ImportTransferModel();
            model.Infor = infor;
            model.Stores = stores;
            model.Units = units;            
            var detail = ctx.SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN(Convert.ToInt32(id)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN_Result>();
            model.ExportDetail = detail;
            return View(model);
        }

        [CustomActionFilter]
        public ActionResult ListExportTransfer(string message, string inforMessage)
        {
            var ctx = new SmsContext();
            ViewBag.InputKind = -1;
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            if (!(bool)Session["IsAdmin"])
            {
                ViewBag.ExportStoreId = Session["MyStore"];
            }
            ViewBag.Stores = stores;
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            return View();
        }

        [CustomActionFilter]
        public ActionResult ListWaitingImport(string message, string inforMessage)
        {
            var ctx = new SmsContext();
            ViewBag.InputKind = 0;
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A");
            ViewBag.Stores = stores;
            if (!(bool)Session["IsAdmin"])
            {
                ViewBag.ImportStoreId = Session["MyStore"];
            }
            ViewBag.Stores = stores;
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            return View();
        }

        [HttpPost]
        public PartialViewResult ListWaitingImportPartialView(int? status, int? exportStoreId,
            int? importStoreId, DateTime? fromDate, DateTime? todate, int? currentPageIndex)
        {
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (todate == null)
            {
                todate = SystemConstant.MAX_DATE;
            }
            if (!(bool)Session["IsAdmin"])
            {
                importStoreId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();
            var theList = ctx.SP_GET_PHIEU_CHUYEN_KHO(Convert.ToInt32(status), Convert.ToInt32(exportStoreId),
                Convert.ToInt32(importStoreId), fromDate, todate, Convert.ToInt32(0), string.Empty).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_PHIEU_CHUYEN_KHO_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ListExportTransferModel model = new ListExportTransferModel();
            model.TheList = theList.ToPagedList(pageIndex, pageSize);
            model.Count = theList.Count;
            ViewBag.Status = status;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)todate).ToString("dd/MM/yyyy");
            ViewBag.ImportStoreId = importStoreId;
            ViewBag.ExportStoreId = exportStoreId;
            return PartialView("ListWaitingImportPartialView", model);
        }

        [CustomActionFilter]
        public ActionResult EditExportTransfer(int id)
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            var infor = ctx.SP_GET_PHIEU_CHUYEN_KHO_INFO_BY_ID(Convert.ToInt32(id)).FirstOrDefault();
            EditTransferModel model = new EditTransferModel();
            //if (!(bool)Session["IsAdmin"])
            //{
            //    model.Infor.MA_KHO_XUAT = Convert.ToInt32(Session["MyStore"]);
            //}
            model.Stores = stores;
            model.Units = units;
            model.Infor = infor;
            var detail = ctx.SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN(Convert.ToInt32(id)).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN_Result>();
            model.ExportDetail = detail;
            return View(model);
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult EditExportTransfer(EditTransferModel model)
        {
            var ctx = new SmsContext();
            var details = model.ExportDetail;
            var infor = model.Infor;
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try{
                    var exInfor = ctx.XUAT_KHO.Find(infor.MA_XUAT_KHO);
                    if (infor.SAVE_FLG == 1)
                    {
                        exInfor.ACTIVE = "W"; // waiting
                    }
                    else
                    {
                        exInfor.ACTIVE = "A";
                    }
                    if ((bool)Session["IsAdmin"])
                    {
                        exInfor.MA_KHO_XUAT = infor.MA_KHO_XUAT;
                    }                    
                    exInfor.MA_KHO_NHAN = infor.MA_KHO_NHAN;
                    exInfor.GHI_CHU = infor.GHI_CHU;
                    exInfor.NGAY_XUAT = infor.NGAY_XUAT;
                    exInfor.LY_DO_XUAT = 3; // xuất chuyển kho
                    exInfor.CREATE_AT = DateTime.Now;
                    exInfor.UPDATE_AT = DateTime.Now;
                    exInfor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    exInfor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    exInfor.MA_NHAN_VIEN_XUAT = Convert.ToInt32(Session["UserId"]);
                    ctx.SaveChanges();
                    var oldDetails = ctx.CHI_TIET_XUAT_KHO.Where(u => u.MA_XUAT_KHO == infor.MA_XUAT_KHO).ToList<CHI_TIET_XUAT_KHO>();
                    foreach (var oldDetail in oldDetails)
                    {
                        oldDetail.ACTIVE = "I";
                        oldDetail.UPDATE_AT = DateTime.Now;
                        oldDetail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                        ctx.SaveChanges();
                    }

                    CHI_TIET_XUAT_KHO ct;
                    foreach (var detail in details)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            ct = ctx.CHI_TIET_XUAT_KHO.Create();
                            if (infor.SAVE_FLG == 1)
                            {
                                ct.ACTIVE = "W";
                            }
                            else
                            {
                                ct.ACTIVE = "A";
                            }
                            ct.CREATE_AT = DateTime.Now;
                            ct.UPDATE_AT = DateTime.Now;
                            ct.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            ct.MA_DON_VI = detail.MA_DON_VI;
                            ct.MA_XUAT_KHO = exInfor.MA_XUAT_KHO;
                            ct.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            ct.SO_LUONG = detail.SO_LUONG_TEMP * detail.HE_SO;
                            ct.GIA_XUAT = 0;
                            ctx.CHI_TIET_XUAT_KHO.Add(ct);
                            ctx.SaveChanges();
                        }
                    }

                    transaction.Complete();
                    return RedirectToAction("ListExportTransfer", new { @messageInfor = "Lưu thành công" });
                }
                catch (Exception ex)
                {
                    
                    Transaction.Current.Rollback();
                    return RedirectToAction("ListExportTransfer", new { @message = "Lưu thất bại, vui lòng liên hệ admin." });
                }
            }
        }
        

        [HttpPost]
        public PartialViewResult ListExportTransferPartialView(int? status, int? exportStoreId,
            int? importStoreId, int? userId, string userFullName, DateTime? fromDate, DateTime? todate, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(userFullName))
            {
                userFullName = string.Empty;
                userId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                userId = Convert.ToInt32(Session["UserId"]);
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (todate == null)
            {
                todate = SystemConstant.MAX_DATE;
            }
            if (!(bool)Session["IsAdmin"])
            {
                exportStoreId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();
            var theList = ctx.SP_GET_PHIEU_CHUYEN_KHO(Convert.ToInt32(status), Convert.ToInt32(exportStoreId),
                Convert.ToInt32(importStoreId), fromDate, todate, Convert.ToInt32(userId), userFullName).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_PHIEU_CHUYEN_KHO_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ListExportTransferModel model = new ListExportTransferModel();
            model.TheList = theList.ToPagedList(pageIndex, pageSize);
            model.Count = theList.Count;
            ViewBag.Status = status;
            ViewBag.UserId = userId;
            ViewBag.UserFullName = userFullName;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)todate).ToString("dd/MM/yyyy");
            ViewBag.ImportStoreId = importStoreId;
            ViewBag.ExportStoreId = exportStoreId;
            return PartialView("ListExportTransferPartialView", model);
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult ExportTransfer(TransferModel model)
        {
            var ctx = new SmsContext();
            var infor = model.ExportInfor;
            var details = model.ExportDetail;
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var exInfor = ctx.XUAT_KHO.Create();
                    if (infor.SAVE_FLG == 1)
                    {
                        exInfor.ACTIVE = "W"; // waiting
                    }else
                    {
                        exInfor.ACTIVE = "A"; 
                    }
                    if ((bool)Session["IsAdmin"])
                    {
                        exInfor.MA_KHO_XUAT = infor.MA_KHO_XUAT;
                    }
                    else
                    {
                        exInfor.MA_KHO_XUAT = Convert.ToInt32(Session["MyStore"]);
                    }
                    exInfor.MA_KHO_NHAN = infor.MA_KHO_NHAN;
                    exInfor.GHI_CHU = infor.GHI_CHU;
                    exInfor.NGAY_XUAT = infor.NGAY_XUAT;
                    exInfor.LY_DO_XUAT = 3; // xuất chuyển kho
                    exInfor.CREATE_AT = DateTime.Now;
                    exInfor.UPDATE_AT = DateTime.Now;
                    exInfor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    exInfor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    exInfor.MA_NHAN_VIEN_XUAT = Convert.ToInt32(Session["UserId"]);
                    ctx.XUAT_KHO.Add(exInfor);
                    ctx.SaveChanges();

                    CHI_TIET_XUAT_KHO ct;

                    foreach (var detail in details)
                    {
                        if(detail.DEL_FLG != 1)
                        {
                            ct = ctx.CHI_TIET_XUAT_KHO.Create();
                            if(infor.SAVE_FLG == 1)
                            {
                                ct.ACTIVE = "W";
                            }else
                            {
                                ct.ACTIVE = "A";
                            }
                            ct.CREATE_AT = DateTime.Now;
                            ct.UPDATE_AT = DateTime.Now;
                            ct.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            ct.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            ct.MA_DON_VI = detail.MA_DON_VI;
                            ct.MA_XUAT_KHO = exInfor.MA_XUAT_KHO;
                            ct.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            ct.SO_LUONG = detail.SO_LUONG_TEMP * detail.HE_SO;
                            ct.GIA_XUAT = 0;
                            ctx.CHI_TIET_XUAT_KHO.Add(ct);
                            ctx.SaveChanges();

                        }
                        
                    }
                    transaction.Complete();
                    return RedirectToAction("ListExportTransfer", new { @inforMessage = "Lưu thành công" });
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("ListExportTransfer", new { @message = "Lưu thất bại, vui lòng liên hệ admin." });
                }
            }
        }
        public ActionResult ExportTransfer()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            TransferModel model = new TransferModel();
            XUAT_KHO ExportInfor = new XUAT_KHO();
            model.ExportInfor = ExportInfor;
            if (!(bool)Session["IsAdmin"])
            {
                model.ExportInfor.MA_KHO_XUAT = Convert.ToInt32(Session["MyStore"]);
            }
            model.Stores = stores;
            model.Units = units;
            return View(model);
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult Import(ImportModel model)
        {
            var ctx = new SmsContext();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var infor = ctx.NHAP_KHO.Create();
                    if ((bool)Session["IsAdmin"])
                    {
                        infor.MA_KHO = model.Infor.MA_KHO;
                    }
                    else
                    {
                        infor.MA_KHO = Convert.ToInt32(Session["MyStore"]);
                    }
                    infor.MA_NHA_CUNG_CAP = model.Infor.MA_NHA_CUNG_CAP;
                    infor.NGAY_NHAP = model.Infor.NGAY_NHAP;
                    infor.NHAN_VIEN_NHAP = Convert.ToInt32(Session["UserId"]);
                    infor.SO_HOA_DON = model.Infor.SO_HOA_DON;
                    infor.CREATE_AT = DateTime.Now;
                    infor.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.UPDATE_AT = DateTime.Now;
                    infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                    infor.ACTIVE = "A";
                    infor.GHI_CHU = model.Infor.GHI_CHU;
                    infor.LY_DO_NHAP = 0; // nhập mua hàng
                    ctx.NHAP_KHO.Add(infor);
                    ctx.SaveChanges();

                    CHI_TIET_NHAP_KHO importDetail;
                    foreach (var detail in model.Detail)
                    {
                        if (detail.DEL_FLG != 1)
                        {
                            importDetail = ctx.CHI_TIET_NHAP_KHO.Create();
                            importDetail.ACTIVE = "A";
                            importDetail.MA_SAN_PHAM = detail.MA_SAN_PHAM;
                            importDetail.SO_LUONG_TEMP = detail.SO_LUONG_TEMP;
                            importDetail.HE_SO = detail.HE_SO;
                            importDetail.SO_LUONG = detail.SO_LUONG_TEMP * detail.HE_SO;
                            importDetail.MA_DON_VI = detail.MA_DON_VI;
                            importDetail.MA_NHAP_KHO = infor.MA_NHAP_KHO;
                            importDetail.CREATE_AT = DateTime.Now;
                            importDetail.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.UPDATE_AT = DateTime.Now;
                            importDetail.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                            importDetail.GIA_VON = detail.GIA_VON/detail.HE_SO;
                            importDetail.DON_GIA_TEMP = detail.GIA_VON;
                            ctx.CHI_TIET_NHAP_KHO.Add(importDetail);
                            ctx.SaveChanges();
                        }                        
                    }
                    transaction.Complete();
                    return RedirectToAction("Index", new { @messageInfor = "Nhập kho thành công." });
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    return RedirectToAction("Index", new { @message = "Nhập kho thất bại, vui lòng liên hệ admin." });
                }
            }
        }

        [CustomActionFilter]
        public ActionResult Import()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var providers = ctx.NHA_CUNG_CAP.Where(u => u.ACTIVE == "A").ToList<NHA_CUNG_CAP>();
            var units = ctx.DON_VI_TINH.Where(u => u.ACTIVE == "A").ToList<DON_VI_TINH>();
            ViewBag.Stores = stores;
            ImportModel model = new ImportModel();
            NHAP_KHO Infor = new NHAP_KHO();
            model.Infor = Infor;
            if(!(bool)Session["IsAdmin"]){
                model.Infor.MA_KHO = Convert.ToInt32(Session["MyStore"]);
            }
            model.Stores = stores;
            model.Providers = providers;
            model.Units = units;
            ViewBag.InputKind = -1;
            return View(model);
        }


        [HttpPost]
        public PartialViewResult IndexPartialView(DateTime? fromDate, DateTime? toDate, int? importerId, string importerName,
            int? reasonId, int? storeId, string storeName, int? providerId, string providerName, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            fromDate = fromDate == null ? SystemConstant.MIN_DATE : fromDate;
            toDate = toDate == null ? SystemConstant.MAX_DATE : toDate;
            ViewBag.InputKind = Convert.ToInt32(reasonId);

            if (string.IsNullOrEmpty(storeName))
            {
                storeName = string.Empty;
                storeId = 0;
            }

            if (string.IsNullOrEmpty(importerName))
            {
                importerName = string.Empty;
                importerId = 0;
            }

            if (string.IsNullOrEmpty(providerName))
            {
                providerName = string.Empty;
                providerId = 0;
            }

            if (!(bool)Session["IsAdmin"])
            {
                importerId = (int)Session["UserId"];
                storeId = (int)Session["MyStore"];
            }

            ViewBag.ProviderId = providerId;
            ViewBag.ProviderName = providerName;
            ViewBag.ImporterId = importerId;
            ViewBag.ImporterName = importerName;
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = storeName;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy"); ;
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");
            ViewBag.InputKind = Convert.ToInt32(reasonId);
            var resultList = ctx.SP_GET_IMPORT(fromDate, toDate, Convert.ToInt32(importerId), importerName, Convert.ToInt32(reasonId)
                , Convert.ToInt32(storeId), storeName, Convert.ToInt32(providerId), providerName).Take(SystemConstant.MAX_ROWS).ToList<SP_GET_IMPORT_Result>();
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            ImportReportModel reportModel = new ImportReportModel();
            reportModel.ImportList = resultList.ToPagedList(pageIndex, pageSize);
            reportModel.PageCount = resultList.Count;
            return PartialView("IndexPartialView", reportModel);
        }

        [HttpGet]
        public ActionResult Index(string message, string messageInfor)
        {
            ViewBag.Message = message;
            ViewBag.MessageInfor = messageInfor;
            ViewBag.InputKind = Convert.ToInt32(-1);
            return View();
        }
    }
}
