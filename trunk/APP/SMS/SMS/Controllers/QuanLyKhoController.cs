using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data.SqlClient;
using PagedList;
using System.Data;
using SMS.App_Start;
using System.IO;
using CsvHelper;
using System.Transactions;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    public class QuanLyKhoController : Controller
    {
        private bool isFloat(string s)
        {
            float outNum =0;
            return float.TryParse(s, out outNum);
        }

        public ActionResult InventoryCompared(string message, string inforMessage)
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var productGroups = ctx.NHOM_SAN_PHAM.Where(u => u.ACTIVE == "A").ToList<NHOM_SAN_PHAM>();
            CompareModel model = new CompareModel();
            model.Stores = stores;
            model.ProductGroups = productGroups;
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            ctx.Dispose();
            return View(model);
        }

        public PartialViewResult InventoryComparedPtv(int? storeId, int? productGroupId, 
            int? productId, string productName, DateTime firstDate, DateTime secondDate, int? currentPageIndex)
        {
            if (storeId == null)
            {
                storeId = 0;
            }
            if (productGroupId == null)
            {
                productGroupId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();

            var tonkho = ctx.Database.SqlQuery<Compared>("exec SP_COMPARE_INVENTORY @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @MA_NHOM, @FIRST_DATE, @SECOND_DATE ",
                new SqlParameter("MA_KHO", Convert.ToInt32(storeId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(productId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(productName) ? "" : productName.Trim()),
                new SqlParameter("MA_NHOM", Convert.ToInt32(productGroupId)),
                new SqlParameter("FIRST_DATE", Convert.ToDateTime(firstDate)),
                new SqlParameter("SECOND_DATE", Convert.ToDateTime(secondDate))).ToList<Compared>();
            
            IPagedList<Compared> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);

            CompareModel model = new CompareModel();
            model.ComparedList = tk;
            ViewBag.StoreId = storeId;
            ViewBag.ProductGroupId = productGroupId;
            ViewBag.ProductName = productName;
            ViewBag.FirstDate = firstDate.ToString("dd/MM/yyyy");
            ViewBag.SecondDate = secondDate.ToString("dd/MM/yyyy");
            ViewBag.Count = tonkho.Count;
            ctx.Dispose();
            return PartialView("InventoryComparedPtv", model);
        }

        public FileContentResult downloadCompared(int? storeId, int? productGroupId,
            int? productId, string productName, DateTime firstDate, DateTime secondDate)
        {
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"Mã sản phẩm\",");
            fileStringBuilder.Append("\"Tên sản phẩm\",");
            fileStringBuilder.Append("\"Đơn vị tính\",");
            fileStringBuilder.Append("\"" + firstDate.ToString("dd/MM/yyyy") + "\",");
            fileStringBuilder.Append("\"" + secondDate.ToString("dd/MM/yyyy") + "\",");
            fileStringBuilder.Append("\"Nhập\",");
            fileStringBuilder.Append("\"Xuất\",");
            fileStringBuilder.Append("\"Thay đổi\"");
            if (storeId == null)
            {
                storeId = 0;
            }
            if (productGroupId == null)
            {
                productGroupId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();

            var tonkho = ctx.Database.SqlQuery<Compared>("exec SP_COMPARE_INVENTORY @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @MA_NHOM, @FIRST_DATE, @SECOND_DATE ",
                new SqlParameter("MA_KHO", Convert.ToInt32(storeId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(productId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(productName) ? "" : productName.Trim()),
                new SqlParameter("MA_NHOM", Convert.ToInt32(productGroupId)),
                new SqlParameter("FIRST_DATE", Convert.ToDateTime(firstDate)),
                new SqlParameter("SECOND_DATE", Convert.ToDateTime(secondDate))).ToList<Compared>();
            
            int i = 0;
            foreach (var detail in tonkho)
            {
                fileStringBuilder.Append("\n");
                i += 1;
                fileStringBuilder.Append("\"" + i + "\",");
                fileStringBuilder.Append("\"" + detail.MA_SAN_PHAM + "\",");
                fileStringBuilder.Append("\"" + detail.TEN_SAN_PHAM + "\",");
                fileStringBuilder.Append("\"" + detail.TEN_DON_VI + "\",");
                fileStringBuilder.Append("\"" + detail.INVEN_FIRST_DATE.ToString("#,##0.##") + "\",");
                fileStringBuilder.Append("\"" + detail.INVEN_SECOND_DATE.ToString("#,##0.##") + "\",");
                fileStringBuilder.Append("\"" + detail.IMPORT.ToString("#,##0.##") + "\",");
                fileStringBuilder.Append("\"" + detail.EXPORT.ToString("#,##0.##") + "\",");
                fileStringBuilder.Append("\"" + detail.COMPARED.ToString("#,##0.##") + "\",");
            }
            ctx.Dispose();
            return File(new System.Text.UTF8Encoding().GetBytes(fileStringBuilder.ToString()), "text/csv", fileName + ".csv");
        }



        [CustomActionFilter]
        [HttpPost]
        public FileContentResult downloadtWarningbyStoreId(int storeId, string productName)
        {
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"Mã sản phẩm\",");
            fileStringBuilder.Append("\"Tên sản phẩm\",");
            fileStringBuilder.Append("\"Đơn vị tính\",");
            fileStringBuilder.Append("\"Tồn tối thiểu\",");
            fileStringBuilder.Append("\"Tồn kho\"");
            var ctx = new SmsContext();
            var inventory = ctx.SP_GET_WARNING_BY_STORE(storeId, productName).ToList<SP_GET_WARNING_BY_STORE_Result>();
            int i = 0;
            foreach (var detail in inventory)
            {
                fileStringBuilder.Append("\n");
                i += 1;
                fileStringBuilder.Append("\"" + i + "\",");
                fileStringBuilder.Append("\"" + detail.MA_SAN_PHAM + "\",");
                fileStringBuilder.Append("\"" + detail.TEN_SAN_PHAM + "\",");
                fileStringBuilder.Append("\"" + detail.TEN_DON_VI + "\",");
                fileStringBuilder.Append("\"" + detail.INVENTORY_MIN.ToString("#,##0.##") + "\",");
                fileStringBuilder.Append("\"" + detail.INVENTORY.ToString("#,##0.##") + "\",");
            }
            ctx.Dispose();
            return File(new System.Text.UTF8Encoding().GetBytes(fileStringBuilder.ToString()), "text/csv", fileName + ".csv");
        }

        [CustomActionFilter]
        public ActionResult getWarningbyStoreId()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            ViewBag.Stores = stores;
            ctx.Dispose();
            return View();
        }

        [CustomActionFilter]
        public PartialViewResult getWarningbyStoreIdPtv(int storeId, string productName, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            var inventory = ctx.SP_GET_WARNING_BY_STORE(storeId, productName).ToList<SP_GET_WARNING_BY_STORE_Result>();
            ViewBag.CurrentPageIndex = currentPageIndex;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;           
            InventoryByStoreModel model = new InventoryByStoreModel();
            model.Count = inventory.Count();
            model.WarningList = inventory.ToPagedList(pageIndex, pageSize);
            ctx.Dispose();
            return PartialView("getWarningbyStoreIdPtv", model);
        }

        [CustomActionFilter]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Không tìm thấy cấu hình tương ứng.";
                return View("../Home/Error"); ;
            }
            var ctx = new SmsContext();
            var donvi = ctx.TON_KHO_MIN_MAX_KHO.Find(id);
            if (donvi != null && donvi.ACTIVE.Equals("A"))
            {
                donvi.ACTIVE = "I";
                donvi.UPDATE_AT = DateTime.Now;
                donvi.CREATE_BY = (int)Session["UserId"];
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("MinMaxOfProductByStore", new { @inforMessage = "Xóa cấu hình thành công." });
            }
            else
            {
                ctx.Dispose();
                return RedirectToAction("MinMaxOfProductByStore", new { @Message = "Không tồn tại cấu hình này. Vui lòng kiểm tra lại." });
            }
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult importMinMax(MinMax model, HttpPostedFileBase file)
        {
            var ctx = new SmsContext();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            model.Stores = stores;
            int storeId = 0;
            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }else
            {
                storeId =(int)model.Infor.MA_KHO;
            }
            if (file != null)
            {
                var fp = Path.Combine(HttpContext.Server.MapPath("~/ImportUploads"), Path.GetFileName(file.FileName));
                file.SaveAs(fp);

                if (Path.GetExtension(fp) == null || Path.GetExtension(fp).ToLower() != ".csv")
                {
                    ViewBag.Message = "Định dạng file không đúng. Vui lòng chọn lại file import.";
                    return View(model);
                }
                ICsvParser csvParser = new CsvParser(new StreamReader(file.InputStream));
                CsvReader csvReader = new CsvReader(csvParser);
                string[] headers = { };
                List<string[]> rows = new List<string[]>();
                string[] row;

                while (csvReader.Read())
                {
                    if (csvReader.FieldHeaders != null && csvReader.FieldHeaders.Length != 6)
                    {
                        ViewBag.Message = "Định dạng file CSV không đúng. Vui lòng kiểm tra lại.";
                        return View(model);
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
                        TON_KHO_MIN_MAX_KHO mm;
                        float min, max;
                        string smin, smax, sproductId;
                        int productId = 0;
                        foreach (var r in rows)
                        {
                            i++;
                            smin = r[4].ToString();
                            smax = r[5].ToString();
                            sproductId = r[1].ToString();
                            if (string.IsNullOrEmpty(smax))
                            {
                                smax = "0";
                            }
                            if (!float.TryParse(smin, out min) || !float.TryParse(smax, out max) || !int.TryParse(sproductId, out productId))
                            {
                                if (!flag)
                                {
                                    fileStringBuilder.Append("<br>");
                                }
                                flag = false;
                                fileStringBuilder.Append("Lỗi dữ liệu tại dòng tại dòng " + i + ". Vui lòng kiểm tra ký tự số.");
                            }
                            else
                            {
                                mm = ctx.TON_KHO_MIN_MAX_KHO.Where(u => u.ACTIVE == "A" && u.MA_KHO == storeId && u.MA_SAN_PHAM == productId).FirstOrDefault();
                                if (mm == null)
                                {
                                    mm = ctx.TON_KHO_MIN_MAX_KHO.Create();
                                }
                                mm.MA_SAN_PHAM = productId;
                                mm.CO_SO_TOI_THIEU =min;
                                mm.CO_SO_TOI_DA = max;
                                mm.MA_KHO = storeId;
                                mm.UPDATE_AT = DateTime.Now;
                                mm.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                                
                                mm.ACTIVE = "A";
                                if (mm.ID <= 0 || mm.ID == null)
                                {
                                    mm.CREATE_AT = DateTime.Now;
                                    mm.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                                    ctx.TON_KHO_MIN_MAX_KHO.Add(mm);
                                }
                                ctx.SaveChanges();
                            }
                        }
                        if (flag)
                        {
                            transaction.Complete();
                            ctx.Dispose();
                            return RedirectToAction("MinMaxOfProductByStore", new { @inforMessage = "Import danh sách thành công." });
                        }
                        else
                        {
                            Transaction.Current.Rollback();
                            ViewBag.Message = fileStringBuilder.ToString();
                            ctx.Dispose();
                            return View(model);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                        ViewBag.Message = "Lỗi dữ liệu tại dòng tại dòng " + i + ". Có lỗi xảy ra trong quá trình import. Vui lòng liên hệ admin.";
                        ctx.Dispose();
                        return View(model);
                    }
                }
                if (!(bool)Session["IsAdmin"])
                {
                    storeId = Convert.ToInt32(Session["MyStore"]);
                }

            }
            else
            {
                ViewBag.Message = "Bạn chưa chọn file. Vui lòng chọn file import.";
                return View(model);
            }
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult importMinMax()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            MinMax model = new MinMax();
            TON_KHO_MIN_MAX_KHO Infor = new TON_KHO_MIN_MAX_KHO();
            model.Infor = Infor;
            model.Infor.MA_KHO = Convert.ToInt32(Session["MyStore"]);
            model.Stores = stores;
            ctx.Dispose();
            return View(model);
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult EditMinmax(MinMax model)
        {
            var ctx = new SmsContext();
            var Infor = ctx.TON_KHO_MIN_MAX_KHO.Find(model.MinMaxInfor.ID);
            if (Infor == null || Infor.ACTIVE != "A")
            {
                ctx.Dispose();
                return RedirectToAction("MinMaxOfProductByStore", new { @message = "Không tồn tại khai báo này, có thể đã bị xóa bởi user khác. Vui lòng kiểm tra lại" });
            }
            try
            {
                Infor.CO_SO_TOI_THIEU = model.MinMaxInfor.CO_SO_TOI_THIEU;
                Infor.CO_SO_TOI_DA = model.MinMaxInfor.CO_SO_TOI_DA;
                Infor.UPDATE_AT = DateTime.Now;
                Infor.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("MinMaxOfProductByStore", new { @inforMessage = "Lưu thành công." });
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return RedirectToAction("MinMaxOfProductByStore", new { @Message = "Lưu thất bại. Vui lòng thử lại lần nữa." });
            }
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult EditMinmax(int id)
        {
            var ctx = new SmsContext();
            var Infor = ctx.SP_GET_MIN_MAX_BY_ID(id).FirstOrDefault();
            if (Infor == null)
            {
                ctx.Dispose();
                return RedirectToAction("MinMaxOfProductByStore", new { @message = "Không tồn tại khai báo này, có thể đã bị xóa bởi user khác. Vui lòng kiểm tra lại" });
            }
            MinMax model = new MinMax();
            model.MinMaxInfor = Infor;
            ctx.Dispose();
            return View(model);
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult AddMinMax(MinMax model)
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            string message = "";
            if (model.Infor.MA_SAN_PHAM == null)
            {
                message += "Bạn chưa nhập sản phẩm, vui lòng chọn sản phẩm.";
            }
            if (model.Infor.CO_SO_TOI_THIEU == null || model.Infor.CO_SO_TOI_THIEU == 0 )
            {
                message += "\n";
                message += "Hãy nhập cơ số tối thiểu";
            }
            if (string.IsNullOrEmpty(message))
            {
                int storeId = 0;
                if (!(bool)Session["IsAdmin"])
                {
                    storeId = Convert.ToInt32(Session["MyStore"]);
                }
                else
                {
                    storeId = (int)model.Infor.MA_KHO;
                }

                var mm = ctx.TON_KHO_MIN_MAX_KHO.Where(u => u.ACTIVE == "A" && u.MA_KHO == storeId && u.MA_SAN_PHAM == model.Infor.MA_SAN_PHAM).FirstOrDefault();
                if (mm == null)
                {
                    mm = ctx.TON_KHO_MIN_MAX_KHO.Create();
                }               
                
                if (!(bool)Session["IsAdmin"])
                {
                    mm.MA_KHO = Convert.ToInt32(Session["MyStore"]);
                }
                else
                {
                    mm.MA_KHO = model.Infor.MA_KHO;
                }
                mm.MA_SAN_PHAM = model.Infor.MA_SAN_PHAM;
                mm.CO_SO_TOI_THIEU = model.Infor.CO_SO_TOI_THIEU;
                mm.CO_SO_TOI_DA = model.Infor.CO_SO_TOI_DA;
               
                mm.UPDATE_AT = DateTime.Now;
                mm.UPDATE_BY = Convert.ToInt32(Session["UserId"]);
               
                mm.ACTIVE = "A";
                if (mm.ID <= 0 || mm.ID == null)
                {
                    mm.CREATE_AT = DateTime.Now;
                    mm.CREATE_BY = Convert.ToInt32(Session["UserId"]);
                    ctx.TON_KHO_MIN_MAX_KHO.Add(mm);
                }                
                ctx.SaveChanges();
                ctx.Dispose();
                return RedirectToAction("MinMaxOfProductByStore", new { @inforMessage = "Lưu thành công" });
            }            
            ViewBag.Message = message;
            model.Stores = stores;
            ctx.Dispose();
            return View(model);
        }

        [CustomActionFilter]
        [HttpGet]
        public ActionResult AddMinMax()
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            MinMax model = new MinMax();
            TON_KHO_MIN_MAX_KHO Infor = new TON_KHO_MIN_MAX_KHO();
            model.Infor = Infor;
            model.Infor.MA_KHO = Convert.ToInt32(Session["MyStore"]);
            model.Stores = stores;
            ctx.Dispose();
            return View(model);
        }

        [CustomActionFilter]
        [HttpPost]
        public FileContentResult downloadMinMaxByStore(int? storeId, int? productGroupId, string productName)
        {
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"Mã sản phẩm\",");
            fileStringBuilder.Append("\"Tên sản phẩm\",");
            fileStringBuilder.Append("\"Đơn vị tính\",");
            fileStringBuilder.Append("\"Cơ số tối thiểu\",");
            fileStringBuilder.Append("\"Cơ số tối đa\"");
            if (storeId == null)
            {
                storeId = 0;
            }
            if (productGroupId == null)
            {
                productGroupId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();
            var MinMax = ctx.SP_GET_MIN_MAX_BY_STORE(storeId, productGroupId, productName).ToList<SP_GET_MIN_MAX_BY_STORE_Result>();
            int i = 0;
            foreach (var detail in MinMax)
            {
                fileStringBuilder.Append("\n");
                i += 1;
                fileStringBuilder.Append("\"" + i + "\",");
                fileStringBuilder.Append("\"" + detail.MA_SAN_PHAM + "\",");
                fileStringBuilder.Append("\"" + detail.TEN_SAN_PHAM + "\",");
                fileStringBuilder.Append("\"" + detail.TEN_DON_VI + "\",");
                fileStringBuilder.Append("\"" + ((Double)detail.CO_SO_TOI_THIEU).ToString("#,###.##") + "\",");
                fileStringBuilder.Append("\"" + ((Double)detail.CO_SO_TOI_DA).ToString("#,###.##") + "\",");
            }
            ctx.Dispose();
            return File(new System.Text.UTF8Encoding().GetBytes(fileStringBuilder.ToString()), "text/csv", fileName + ".csv");
        }
        
        [CustomActionFilter]
        public ActionResult MinMaxOfProductByStore(string message, string inforMessage)
        {
            var ctx = new SmsContext();
            var stores = ctx.KHOes.Where(u => u.ACTIVE == "A").ToList<KHO>();
            var productGroups = ctx.NHOM_SAN_PHAM.Where(u => u.ACTIVE == "A").ToList<NHOM_SAN_PHAM>();
            MinMax model = new MinMax();
            model.Stores = stores;
            model.ProductGroups = productGroups;
            ViewBag.Message = message;
            ViewBag.InforMessage = inforMessage;
            ctx.Dispose();
            return View(model);
        }

        public PartialViewResult MinMaxOfProductByStorePartialView(int? storeId, int? productGroupId, string productName, int? currentPageIndex)
        {
            if (storeId == null)
            {
                storeId = 0;
            }
            if (productGroupId == null)
            {
                productGroupId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                storeId = Convert.ToInt32(Session["MyStore"]);
            }
            var ctx = new SmsContext();
            var MinMax = ctx.SP_GET_MIN_MAX_BY_STORE(storeId, productGroupId, productName).ToList<SP_GET_MIN_MAX_BY_STORE_Result>();
            IPagedList<SP_GET_MIN_MAX_BY_STORE_Result> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = MinMax.ToPagedList(pageIndex, pageSize);
            MinMax model = new MinMax();
            model.MinMaxList = tk;
            ViewBag.StoreId = storeId;
            ViewBag.ProductGroupId = productGroupId;
            ViewBag.ProductName = productName;
            ViewBag.Count = MinMax.Count;
            ctx.Dispose();
            return PartialView("MinMaxOfProductByStorePartialView", model);
        }


        [CustomActionFilter]
        public ActionResult Index()
        {
            return View();
        }


        [CustomActionFilter]
        [HttpGet]
        public ActionResult FifoReport()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult FifoReportPagingContent(int? StoreId, string StoreName, int? ProductId, string ProductName,  bool? flag, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }

            if (!(bool)Session["IsAdmin"])
            {
                StoreId = (int)Session["MyStore"];
            }

            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }

            var idStoreParam = new SqlParameter
            {
                ParameterName = "MA_KHO",
                Value = Convert.ToInt32(StoreId)
            };

            var StoreNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHO",
                Value = StoreName
            };


            var idProductParam = new SqlParameter
            {
                ParameterName = "MA_SAN_PHAM",
                Value = Convert.ToInt32(ProductId)
            };

            var ProductNameParam = new SqlParameter
            {
                ParameterName = "TEN_SAN_PHAM_PR",
                Value = ProductName
            };

            var totalExport = new SqlParameter
            {
                ParameterName = "GIA_VON_HANG_BAN_TOTAL",
                Value = Convert.ToDouble(0),
                Direction = ParameterDirection.Output
            };
            var totalLeft = new SqlParameter
            {
                ParameterName = "GIA_TRI_HANG_TON_TOTAL",
                Value = Convert.ToDouble(0),
                Direction = ParameterDirection.Output
            };
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var tonkho = ctx.Database.SqlQuery<Fifo>("exec STMA_GET_GIA_TRI_HANG_BAN_TON @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM_PR, @GIA_VON_HANG_BAN_TOTAL OUT, @GIA_TRI_HANG_TON_TOTAL OUT",
                idStoreParam,
                StoreNameParam,
                idProductParam,
                ProductNameParam,
                totalExport,
                totalLeft
                ).ToList<Fifo>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<Fifo> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            FifoModel model = new FifoModel();
            model.ResultList = tk;
            var exportValue = totalExport == null ? 0 : (double)totalExport.Value;
            var leftValue = totalLeft == null ? 0 : (double)totalLeft.Value;
            model.GIA_VON_HANG_BAN_TOTAL = exportValue;
            model.GIA_TRI_HANG_TON_TOTAL = leftValue;
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ViewBag.StoreId = StoreId;
            ViewBag.ProductId = ProductId;
            ctx.Dispose();
            return PartialView("FifoReportPartialView", model);
        }

        [HttpPost]
        public PartialViewResult FifoReportPartialView(int? StoreId, int? ProductId, bool? flag, string StoreName, string ProductName, int? currentPageIndex)
        {
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }

            if (!(bool)Session["IsAdmin"])
            {
                StoreId = (int)Session["MyStore"];
            }

            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }

            var idStoreParam = new SqlParameter
            {
                ParameterName = "MA_KHO",
                Value = Convert.ToInt32(StoreId)
            };

            var StoreNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHO",
                Value = StoreName
            };


            var idProductParam = new SqlParameter
            {
                ParameterName = "MA_SAN_PHAM",
                Value = Convert.ToInt32(ProductId)
            };

            var ProductNameParam = new SqlParameter
            {
                ParameterName = "TEN_SAN_PHAM_PR",
                Value = ProductName
            };

            var totalExport = new SqlParameter
            {
                ParameterName = "GIA_VON_HANG_BAN_TOTAL",
                Value = Convert.ToDouble(0),
                Direction = ParameterDirection.Output
            };
            var totalLeft = new SqlParameter
            {
                ParameterName = "GIA_TRI_HANG_TON_TOTAL",
                Value = Convert.ToDouble(0),
                Direction = ParameterDirection.Output
            };
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            var tonkho = ctx.Database.SqlQuery<Fifo>("exec STMA_GET_GIA_TRI_HANG_BAN_TON @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM_PR, @GIA_VON_HANG_BAN_TOTAL OUT, @GIA_TRI_HANG_TON_TOTAL OUT",
                idStoreParam,
                StoreNameParam,
                idProductParam,
                ProductNameParam,
                totalExport,
                totalLeft
                ).ToList<Fifo>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<Fifo> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            FifoModel model = new FifoModel();
            model.ResultList = tk;
            var exportValue = totalExport == null? 0: (double)totalExport.Value;
            var leftValue = totalLeft == null ? 0: (double)totalLeft.Value;
            model.GIA_VON_HANG_BAN_TOTAL = exportValue;
            model.GIA_TRI_HANG_TON_TOTAL = leftValue;
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ViewBag.StoreId = StoreId;
            ViewBag.ProductId = ProductId;
            ctx.Dispose();
            return PartialView("FifoReportPartialView", model);
        }

        
        [HttpPost]
        public PartialViewResult ImExDetailPartialViewResult(int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                StoreId = Convert.ToInt32(Session["MyStore"]);
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }

            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromDate.ToString()).ToString("dd/MM/yyyy");
            }

            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(toDate.ToString()).ToString("dd/MM/yyyy");
            }
            var tonkho = ctx.Database.SqlQuery<ImEx>("exec SP_GET_NHAP_XUAT @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<ImEx>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<ImEx> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ViewBag.StoreId = StoreId;
            ViewBag.ProductId = ProductId;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");;
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy"); ;
            ImExModel model = new ImExModel();
            model.ResultList = tk;
            ctx.Dispose();
            return PartialView("ImExDetailPartialViewResult", model);
        }

        /****************************************************************
         * 
         * 
         ****************************************************************/
        [CustomActionFilter]
        [HttpGet]
        public ActionResult ImExDetail()
        {
            return View();
        }

        /****************************************************************
         * 
         * 
         ****************************************************************/
        [HttpPost]
        public PartialViewResult ExportReportDetailPartialView(int? kind, int? StoreId, int? ProductId, 
             string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                StoreId = (int)Session["MyStore"];
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }

            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromDate.ToString()).ToString("dd/MM/yyyy");
            }

            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(toDate.ToString()).ToString("dd/MM/yyyy");
            }
            var tonkho = ctx.Database.SqlQuery<ExportReportDetail>("exec SP_EXPORT_REPORT_DETAIL @KIND, @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<ExportReportDetail>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<ExportReportDetail> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ViewBag.StoreId = StoreId;
            ViewBag.ProductId = ProductId;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");
            ExportReportDetailModel model = new ExportReportDetailModel();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_EXPORT_REPORT_SUM @KIND, @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<InventoryTotal>().First();
            model.VALUE = (double)total.VALUE;
            ctx.Dispose();
            return PartialView("ExportReportDetailPartialView", model);
        }

        /****************************************************************
         * 
         * 
         ****************************************************************/
        [CustomActionFilter]
        [HttpGet]
        public ActionResult ExportReportDetail(int? kind)
        {
            var ctx = new SmsContext();
            ctx.Database.CommandTimeout = 300;
            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;
            ctx.Dispose();
            return View();
        }

        [HttpPost]
        public PartialViewResult ExportReportPartialView(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }

            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromDate.ToString()).ToString("dd/MM/yyyy");
            }

            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(toDate.ToString()).ToString("dd/MM/yyyy");
            }
            if (!(bool)Session["IsAdmin"])
            {
                StoreId = (int)Session["MyStore"];
            }

            var tonkho = ctx.Database.SqlQuery<ExportRepot>("exec SP_EXPORT_REPORT @KIND, @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName)? string.Empty: StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<ExportRepot>().Take(SystemConstant.MAX_ROWS);
            ViewBag.Count = tonkho.Count();
            IPagedList<ExportRepot> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ViewBag.StoreId = StoreId;
            ViewBag.ProductId = ProductId;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");
            ExportRepotModel model = new ExportRepotModel();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_EXPORT_REPORT_SUM @KIND, @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<InventoryTotal>().First();
            model.VALUE = (double)total.VALUE;
            ctx.Dispose();
            return PartialView("ExportReportPartialView", model);
        }

        [CustomActionFilter]
        public ActionResult ExportReport(int? kind)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;
            ctx.Dispose();
            return View();
        }
       
        [HttpPost]
        public PartialViewResult ImportRepoterPartialView(int? kind, int? StoreId, string StoreName, int? ProductId,
            string ProductName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;

            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                StoreId = (int)Session["MyStore"];
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromDate.ToString()).ToString("dd/MM/yyyy");
            }

            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(toDate.ToString()).ToString("dd/MM/yyyy");
            }
            var tonkho = ctx.Database.SqlQuery<SpImportRepoter>("exec SP_IMPORT_REPORTER @KIND, @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<SpImportRepoter>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<SpImportRepoter> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ViewBag.StoreId = StoreId;
            ViewBag.ProductId = ProductId;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");
            SpImportRepoterModel model = new SpImportRepoterModel();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_IMPORT_REPORTER_SUM @KIND, @MA_KHO,@TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<InventoryTotal>().First();
            model.VALUE = (double)total.VALUE;
            ctx.Dispose();
            return PartialView("ImportRepoterPartialView", model);
        }

        [HttpGet]
        public ActionResult ImportRepoter(int? kind)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;
            ctx.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult ImportRepoter(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page, bool? flag)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductId = 0;
            }

            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromDate.ToString()).ToString("dd/MM/yyyy");
            }

            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(toDate.ToString()).ToString("dd/MM/yyyy");
            }
            var tonkho = ctx.Database.SqlQuery<SpImportRepoter>("exec SP_IMPORT_REPORTER @KIND, @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO",string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<SpImportRepoter>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<SpImportRepoter> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            SpImportRepoterModel model = new SpImportRepoterModel();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_IMPORT_REPORTER_SUM @KIND, @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<InventoryTotal>().First();
            model.VALUE = (double)total.VALUE;
            ctx.Dispose();
            return View(model);
        }

        [HttpPost]
        public PartialViewResult ImportRepoterDetailPartialView(int? kind, int? StoreId, int? ProductId, 
            string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }

            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromDate.ToString()).ToString("dd/MM/yyyy");
            }

            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(toDate.ToString()).ToString("dd/MM/yyyy");
            }
            var tonkho = ctx.Database.SqlQuery<SP_IMPORT_REPORTER_DETAIL_Result>("exec SP_IMPORT_REPORTER_DETAIL @KIND, @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<SP_IMPORT_REPORTER_DETAIL_Result>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<SP_IMPORT_REPORTER_DETAIL_Result> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);

            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ViewBag.StoreId = StoreId;
            ViewBag.ProductId = ProductId;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = ((DateTime)toDate).ToString("dd/MM/yyyy");

            ImportReportDetail model = new ImportReportDetail();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_IMPORT_REPORTER_SUM @KIND, @MA_KHO, @TEN_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName)? string.Empty: StoreName),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<InventoryTotal>().First();
            model.VALUE = (double)total.VALUE;
            ctx.Dispose();
            return PartialView("ImportRepoterDetailPartialView", model);
        }

        public ActionResult ImportRepoterDetail(int? kind)
        {
            var ctx = new SmsContext();
            if (kind == null){
                kind = -1;
            }
            ViewBag.InputKind = kind;
            ctx.Dispose();
            return View();
        }
        
        [HttpPost]
        public ActionResult ImportRepoterDetail(int? kind, int? StoreId, int? ProductId, string StoreName, string ProductName, DateTime? fromDate, DateTime? toDate, int? page, bool? flag)
        {
            var ctx = new SmsContext();
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductId = 0;
            }

            if (kind == null)
            {
                kind = -1;
            }
            ViewBag.InputKind = kind;
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            else
            {
                ViewBag.FromDate = DateTime.Parse(fromDate.ToString()).ToString("dd/MM/yyyy");
            }

            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            else
            {
                ViewBag.toDate = DateTime.Parse(toDate.ToString()).ToString("dd/MM/yyyy");
            }
            var tonkho = ctx.Database.SqlQuery<SP_IMPORT_REPORTER_DETAIL_Result>("exec SP_IMPORT_REPORTER_DETAIL @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<SP_IMPORT_REPORTER_DETAIL_Result>().Take(SystemConstant.MAX_ROWS);

            ViewBag.Count = tonkho.Count();
            IPagedList<SP_IMPORT_REPORTER_DETAIL_Result> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = page == null ? 1 : (int)page;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            ViewBag.StoreName = StoreName;
            ViewBag.ProductName = ProductName;
            ImportReportDetail model = new ImportReportDetail();
            model.ResultList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_IMPORT_REPORTER_SUM @KIND, @MA_KHO, @MA_SAN_PHAM, @TEN_SAN_PHAM, @FROM_DATE, @TO_DATE ",
                new SqlParameter("KIND", Convert.ToInt32(kind)),
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? string.Empty : ProductName.Trim()),
                new SqlParameter("FROM_DATE", fromDate),
                new SqlParameter("TO_DATE", toDate)
                ).ToList<InventoryTotal>().First();
            model.VALUE = (double)total.VALUE;
            ctx.Dispose();
            return View(model);
        }
        
        [CustomActionFilter]
        [HttpGet]
        public ActionResult Inventory()
        {
            var ctx = new SmsContext();
            var productGroups = ctx.NHOM_SAN_PHAM.Where(u => u.ACTIVE == "A").ToList<NHOM_SAN_PHAM>();
            ViewBag.ProductGroups = productGroups;
            ctx.Dispose();
            return View();
        }

        [HttpPost]
        public FileContentResult downloadCSV(int? StoreId, int? productGroupId, int? ProductId, string StoreName, string ProductName)
        {
            var ctx = new SmsContext();
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                StoreId = (int)Session["MyStore"];
            }
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("\"STT\",");
            fileStringBuilder.Append("\"Mã sản phẩm\",");
            fileStringBuilder.Append("\"CODE\",");
            fileStringBuilder.Append("\"Tên sản phẩm\",");
            fileStringBuilder.Append("\"Tên đơn vị tính\",");
            fileStringBuilder.Append("\"Số lượng tồn\",");
            fileStringBuilder.Append("\"Giá vốn\"");
            var tonkho = ctx.Database.SqlQuery<Inventory>("exec SP_GET_INVENTORY @MA_KHO, @TEN_KHO, @GROUP_ID, @MA_SAN_PHAM, @TEN_SAN_PHAM ",
               new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
               new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
               new SqlParameter("GROUP_ID", Convert.ToInt32(productGroupId)),
               new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
               new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<Inventory>();
             int i = 0;
             foreach (var product in tonkho)
             {
                 fileStringBuilder.Append("\n");
                 i += 1;
                 fileStringBuilder.Append("\"" + i + "\",");
                 fileStringBuilder.Append("\"" + product.MA_SAN_PHAM + "\",");
                 fileStringBuilder.Append("\"" + product.CODE + "\",");
                 fileStringBuilder.Append("\"" + product.TEN_SAN_PHAM + "\",");
                 fileStringBuilder.Append("\"" + product.TEN_DON_VI + "\",");
                 fileStringBuilder.Append("\"" + product.SO_LUONG_TON.ToString("#,###.##") + "\",");
                 fileStringBuilder.Append("\"" +  "" + "\",");
             }
             ctx.Dispose();
            return File(new System.Text.UTF8Encoding().GetBytes(fileStringBuilder.ToString()), "text/csv", fileName + ".csv");
        }


        [HttpPost]
        public PartialViewResult InventoryPagingConent(int? StoreId, int? productGroupId, int? ProductId, string StoreName, string ProductName, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                StoreId = (int)Session["MyStore"];
            }
            ViewBag.StoreId = StoreId;
            ViewBag.StoreName = StoreName;
            ViewBag.ProductId = ProductId;
            ViewBag.ProductName = ProductName;
            ViewBag.ProductGroupId = productGroupId;
            var tonkho = ctx.Database.SqlQuery<Inventory>("exec SP_GET_INVENTORY @MA_KHO, @TEN_KHO, @GROUP_ID, @MA_SAN_PHAM, @TEN_SAN_PHAM ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("GROUP_ID", Convert.ToInt32(productGroupId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<Inventory>().Take(SystemConstant.MAX_ROWS);
            ViewBag.Count = tonkho.Count();
            IPagedList<Inventory> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            GetInventoryModel model = new GetInventoryModel();
            model.InventoryList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_GET_VALUE_OF_INVENTORY @MA_KHO, @TEN_KHO,@GROUP_ID, @MA_SAN_PHAM, @TEN_SAN_PHAM ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("GROUP_ID", Convert.ToInt32(productGroupId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<InventoryTotal>().First();
            model.VALUE = total.VALUE;
            ctx.Dispose();
            return PartialView("InventoryPartialView", model);
        }

        [HttpPost]
        public PartialViewResult InventoryPartialView(int? StoreId, int? productGroupId, int? ProductId, string StoreName, string ProductName, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = string.Empty;
                StoreId = 0;
            }
            if (string.IsNullOrEmpty(ProductName))
            {
                ProductName = string.Empty;
                ProductId = 0;
            }
            if (!(bool)Session["IsAdmin"])
            {
                StoreId = (int)Session["MyStore"];
            }
            ViewBag.StoreId = StoreId;
            ViewBag.StoreName = StoreName;
            ViewBag.ProductId = ProductId;
            ViewBag.ProductName = ProductName;
            ViewBag.ProductGroupId = productGroupId;

            var tonkho = ctx.Database.SqlQuery<Inventory>("exec SP_GET_INVENTORY @MA_KHO, @TEN_KHO, @GROUP_ID, @MA_SAN_PHAM, @TEN_SAN_PHAM ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("GROUP_ID", Convert.ToInt32(productGroupId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<Inventory>().Take(SystemConstant.MAX_ROWS);
            ViewBag.Count = tonkho.Count();
            IPagedList<Inventory> tk = null;
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            tk = tonkho.ToPagedList(pageIndex, pageSize);
            GetInventoryModel model = new GetInventoryModel();
            model.InventoryList = tk;
            var total = ctx.Database.SqlQuery<InventoryTotal>("exec SP_GET_VALUE_OF_INVENTORY @MA_KHO, @TEN_KHO,@GROUP_ID, @MA_SAN_PHAM, @TEN_SAN_PHAM ",
                new SqlParameter("MA_KHO", Convert.ToInt32(StoreId)),
                new SqlParameter("TEN_KHO", string.IsNullOrWhiteSpace(StoreName) ? string.Empty : StoreName),
                new SqlParameter("GROUP_ID", Convert.ToInt32(productGroupId)),
                new SqlParameter("MA_SAN_PHAM", Convert.ToInt32(ProductId)),
                new SqlParameter("TEN_SAN_PHAM", string.IsNullOrEmpty(ProductName) ? "" : ProductName.Trim())).ToList<InventoryTotal>().First();
            model.VALUE = total.VALUE;
            ctx.Dispose();
            return PartialView("InventoryPartialView", model);
        }

    }
}
