using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data.SqlClient;
using PagedList;
using System.Web.Helpers;
using SMS.App_Start;

namespace SMS.Controllers
{
    [Authorize]
    [HandleError]
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        [CustomActionFilter]
        [HttpPost]
        public ActionResult DownloadDebitColecction(int? customerId, string customerName, DateTime? fromDate, DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var ctx = new SmsContext();
            var customerIdParam = new SqlParameter
            {
                ParameterName = "CUSTOMER_ID",
                Value = Convert.ToInt32(customerId)
            };
            var customerNameParam = new SqlParameter
            {
                ParameterName = "CUSTOMER_NAME",
                Value = customerName
            };
            var FromDateParam = new SqlParameter
            {
                ParameterName = "FROM_DATE",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "TO_DATE",
                Value = Convert.ToDateTime(toDate)
            };
            var deatils = ctx.Database.SqlQuery<ReportDebitColection>("exec SP_GET_REPORT_DEBT_COLLECTION @CUSTOMER_ID, @CUSTOMER_NAME, @FROM_DATE, @TO_DATE", customerIdParam, customerNameParam,
                FromDateParam, ToDateParam).ToList<ReportDebitColection>();
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename= " + fileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("<table border=2><tr><td bgcolor='#6495ED' style=\"font-size:18px; font-family:'Times New Roman'\" align='center' colspan='6'> BÁO CÁO THU NỢ </td></tr>");
            fileStringBuilder.Append("<tr>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> STT </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tên khách hàng </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Ngày</td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Trả bằng tiền mặt </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Trả hàng </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tổng cộng </td>");
            fileStringBuilder.Append("</tr>");

            int i = 0;
            double cash = 0;
            double byReturn = 0;
            double total = 0;

            foreach (var detail in deatils)
            {

                i++;
                cash += detail.PHAT_SINH;
                byReturn += detail.PHAT_SINH_BY_RETURN;
                total += detail.TOTAL;
                fileStringBuilder.Append("<tr>");
                fileStringBuilder.Append("<td align='center' style=\"font-size:18px; font-family:'Times New Roman'\"> " + i + " </td>");
                fileStringBuilder.Append("<td align='left' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.TEN_KHACH_HANG + " </td>");
                fileStringBuilder.Append("<td align='left' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.NGAY_PHAT_SINH.ToString("dd/MM/yyyy") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.PHAT_SINH.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.PHAT_SINH_BY_RETURN.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.TOTAL.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("</tr>");
            }

            fileStringBuilder.Append("<tr>");
            fileStringBuilder.Append("<td align='left' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman' \" colspan='3' > Tổng cộng </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + cash.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + byReturn.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + total.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("</tr>");

            fileStringBuilder.Append("</table>");

            Response.Output.Write(fileStringBuilder.ToString());
            Response.Flush();
            Response.End();
            return View("../Report/ReportDebitColecction");
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult downloadReportByArea(int? areaId, string areaName, DateTime? fromDate, DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(areaName))
            {
                areaName = string.Empty;
                areaId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var ctx = new SmsContext();
            var customerIdParam = new SqlParameter
            {
                ParameterName = "MA_KHU_VUC",
                Value = Convert.ToInt32(areaId)
            };
            var customerNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHU_VUC",
                Value = areaName
            };
            var FromDateParam = new SqlParameter
            {
                ParameterName = "START_TIME",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "END_TIME",
                Value = Convert.ToDateTime(toDate)
            };

            var details = ctx.Database.SqlQuery<Report>("exec SP_REPORT_BY_DAY_AND_AREA  @START_TIME, @END_TIME, @MA_KHU_VUC, @TEN_KHU_VUC ",
                FromDateParam, ToDateParam, customerIdParam,customerNameParam ).ToList<Report>();
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename= " + fileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("<table border=2><tr><td bgcolor='#6495ED' style=\"font-size:18px; font-family:'Times New Roman'\" align='center' colspan='5'> BÁO CÁO DOANH THU THEO KHU VỰC </td></tr>");
            fileStringBuilder.Append("<tr>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Ngày  </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tổng thực thu </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tổng nợ gối đầu</td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tổng trả hàng </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Doanh thu </td>");
            fileStringBuilder.Append("</tr>");
            int AreaId = 0;
            double reciptTotal = 0;
            double debitTotal = 0;
            double returnTotal = 0;
            double total = 0;
            double subreciptTotal = 0;
            double subdebitTotal = 0;
            double subreturnTotal = 0;
            double subtotal = 0;
            foreach (var detail in details)
            {
                if (AreaId != detail.MA_KHU_VUC)
                {
                    AreaId = detail.MA_KHU_VUC;
                    if (subtotal != 0 || subreciptTotal != 0 || subdebitTotal != 0 || subreturnTotal != 0)
                    {
                        fileStringBuilder.Append("<tr>");
                        fileStringBuilder.Append("<td align='center' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> </td>");
                        fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subreciptTotal.ToString("#,###.##") + " </td>");
                        fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subdebitTotal.ToString("#,###.##") + " </td>");
                        fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subreturnTotal.ToString("#,###.##") + " </td>");
                        fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subtotal.ToString("#,###.##") + " </td>");
                        fileStringBuilder.Append("</tr>");
                    }
                    fileStringBuilder.Append("<tr>");
                    fileStringBuilder.Append("<td align='left' colspan='5' bgcolor='#87CEFA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.TEN_KHU_VUC + "</td>");
                    fileStringBuilder.Append("</tr>");
                    subreciptTotal = 0;
                    subdebitTotal = 0;
                    subreturnTotal = 0;
                    subtotal = 0;
                }
                reciptTotal += detail.SO_TIEN_KHACH_TRA;
                debitTotal += detail.SO_TIEN_NO_GOI_DAU;
                returnTotal += detail.RETURN_TOTAL;
                total += detail.TOTAL;
                subreciptTotal += detail.SO_TIEN_KHACH_TRA;
                subdebitTotal += detail.SO_TIEN_NO_GOI_DAU;
                subreturnTotal += detail.RETURN_TOTAL;
                subtotal += detail.TOTAL;
                fileStringBuilder.Append("<tr>");
                fileStringBuilder.Append("<td align='left' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.DAY.ToString("dd/MM/yyyy") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.SO_TIEN_KHACH_TRA.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.SO_TIEN_NO_GOI_DAU.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.RETURN_TOTAL.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.TOTAL.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("</tr>");
            }
            if (subtotal != 0 || subreciptTotal != 0 || subdebitTotal != 0 || subreturnTotal != 0)
            {
                fileStringBuilder.Append("<tr>");
                fileStringBuilder.Append("<td align='left' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> </td>");
                fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subreciptTotal.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subdebitTotal.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subreturnTotal.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subtotal.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("</tr>");
            }

            fileStringBuilder.Append("<tr>");
            fileStringBuilder.Append("<td align='left' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> Tổng cộng </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + reciptTotal.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + debitTotal.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + returnTotal.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + total.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("</tr>");

            fileStringBuilder.Append("</table>");

            Response.Output.Write(fileStringBuilder.ToString());
            Response.Flush();
            Response.End();
            return View("../Report/ReportByCustomer");
        }

        [CustomActionFilter]
        [HttpPost]
        public ActionResult downloadReportBycustomer(int? customerId, string customerName, DateTime? fromDate, DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var ctx = new SmsContext();
            var customerIdParam = new SqlParameter
            {
                ParameterName = "MA_KHACH_HANG",
                Value = Convert.ToInt32(customerId)
            };
            var customerNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHACH_HANG",
                Value = customerName
            };
            var FromDateParam = new SqlParameter
            {
                ParameterName = "START_TIME",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "END_TIME",
                Value = Convert.ToDateTime(toDate)
            };
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + DateTime.Now.Millisecond.ToString();
            var details = ctx.Database.SqlQuery<ReportByCustomer>("exec SP_REPORT_BY_CUSTOMER  @START_TIME, @END_TIME, @MA_KHACH_HANG, @TEN_KHACH_HANG ",
               FromDateParam, ToDateParam, customerIdParam, customerNameParam).ToList<ReportByCustomer>();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename= "+ fileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            System.Text.StringBuilder fileStringBuilder = new System.Text.StringBuilder();
            fileStringBuilder.Append("<table border=2><tr><td bgcolor='#6495ED' style=\"font-size:18px; font-family:'Times New Roman'\" align='center' colspan='5'> BÁO CÁO DOANH THU THEO KHU VỰC - KHÁCH HÀNG </td></tr>");
            fileStringBuilder.Append("<tr>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tên khách hàng </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tổng thực thu </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tổng nợ gối đầu</td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Tổng trả hàng </td>");
            fileStringBuilder.Append("<td align='center' bgcolor='#E6E6FA' style=\"font-size:18px; font-family:'Times New Roman'\"> Doanh thu </td>");
            fileStringBuilder.Append("</tr>");
            int AreaId = 0;
            double reciptTotal = 0;
            double debitTotal = 0;
            double returnTotal = 0;
            double total = 0;
            double subreciptTotal = 0;
            double subdebitTotal = 0;
            double subreturnTotal = 0;
            double subtotal = 0;
            foreach (var detail in details)
            {
                if (AreaId != detail.MA_KHU_VUC)
                {
                    AreaId = detail.MA_KHU_VUC;
                    if (subtotal != 0 || subreciptTotal != 0 || subdebitTotal != 0 || subreturnTotal != 0)
                    {
                        fileStringBuilder.Append("<tr>");
                        fileStringBuilder.Append("<td align='center' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> </td>");
                        fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subreciptTotal.ToString("#,###.##") + " </td>");
                        fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subdebitTotal.ToString("#,###.##") + " </td>");
                        fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subreturnTotal.ToString("#,###.##") + " </td>");
                        fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subtotal.ToString("#,###.##") + " </td>");
                        fileStringBuilder.Append("</tr>");
                    }
                    fileStringBuilder.Append("<tr>");
                    fileStringBuilder.Append("<td align='left' colspan='5' bgcolor='#87CEFA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.TEN_KHU_VUC +"</td>");
                    fileStringBuilder.Append("</tr>");
                    subreciptTotal = 0;
                    subdebitTotal = 0;
                    subreturnTotal = 0;
                    subtotal = 0;
                }
                reciptTotal += detail.SO_TIEN_KHACH_TRA;
                debitTotal += detail.SO_TIEN_NO_GOI_DAU;
                returnTotal += detail.RETURN_TOTAL;
                total += detail.TOTAL;
                subreciptTotal += detail.SO_TIEN_KHACH_TRA;
                subdebitTotal += detail.SO_TIEN_NO_GOI_DAU;
                subreturnTotal += detail.RETURN_TOTAL;
                subtotal += detail.TOTAL;
                fileStringBuilder.Append("<tr>");
                fileStringBuilder.Append("<td align='left' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.TEN_KHACH_HANG +" </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.SO_TIEN_KHACH_TRA.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.SO_TIEN_NO_GOI_DAU.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.RETURN_TOTAL.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' style=\"font-size:18px; font-family:'Times New Roman'\"> " + detail.TOTAL.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("</tr>");
            }
            if (subtotal != 0 || subreciptTotal != 0 || subdebitTotal != 0 || subreturnTotal != 0)
            {
                fileStringBuilder.Append("<tr>");
                fileStringBuilder.Append("<td align='left' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> </td>");
                fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subreciptTotal.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subdebitTotal.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subreturnTotal.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("<td align='right' bgcolor='#20B2AA' style=\"font-size:18px; font-family:'Times New Roman'\"> " + subtotal.ToString("#,###.##") + " </td>");
                fileStringBuilder.Append("</tr>");
            }

            fileStringBuilder.Append("<tr>");
            fileStringBuilder.Append("<td align='left' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> Tổng cộng </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + reciptTotal.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + debitTotal.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + returnTotal.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("<td align='right' bgcolor='#C0C0C0' style=\"font-size:18px; font-family:'Times New Roman'\"> " + total.ToString("#,###.##") + " </td>");
            fileStringBuilder.Append("</tr>");

            fileStringBuilder.Append("</table>");

            Response.Output.Write(fileStringBuilder.ToString());
            Response.Flush();
            Response.End();
            return View("../Report/ReportByCustomer");
        }


        public ActionResult ReportByCustomer()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult ReportByCustomerPartialView(int? customerId, string customerName, DateTime? fromDate, DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var ctx = new SmsContext();
            var customerIdParam = new SqlParameter
            {
                ParameterName = "MA_KHACH_HANG",
                Value = Convert.ToInt32(customerId)
            };
            var customerNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHACH_HANG",
                Value = customerName
            };
            var FromDateParam = new SqlParameter
            {
                ParameterName = "START_TIME",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "END_TIME",
                Value = Convert.ToDateTime(toDate)
            };
            var tonkho = ctx.Database.SqlQuery<ReportByCustomer>("exec SP_REPORT_BY_CUSTOMER  @START_TIME, @END_TIME, @MA_KHACH_HANG, @TEN_KHACH_HANG ",
               FromDateParam, ToDateParam, customerIdParam, customerNameParam).Take(SystemConstant.MAX_ROWS).ToList<ReportByCustomer>();
            ReportByCustomerModel model = new ReportByCustomerModel();
            model.TheList = tonkho;

            return PartialView("ReportByCustomerPartialView", model);
        }

        [CustomActionFilter]
        public ActionResult ReportByArea()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult ReportByAreaPartialView(int? areaId, string areaName, DateTime? fromDate, DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(areaName))
            {
                areaName = string.Empty;
                areaId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var ctx = new SmsContext();
            var customerIdParam = new SqlParameter
            {
                ParameterName = "MA_KHU_VUC",
                Value = Convert.ToInt32(areaId)
            };
            var customerNameParam = new SqlParameter
            {
                ParameterName = "TEN_KHU_VUC",
                Value = areaName
            };
            var FromDateParam = new SqlParameter
            {
                ParameterName = "START_TIME",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "END_TIME",
                Value = Convert.ToDateTime(toDate)
            };

            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_REPORT_BY_DAY_AND_AREA  @START_TIME, @END_TIME, @MA_KHU_VUC, @TEN_KHU_VUC ",
                FromDateParam, ToDateParam, customerIdParam,customerNameParam ).Take(SystemConstant.MAX_ROWS).ToList<Report>();
            ReportModel model = new ReportModel();
            model.TheList = tonkho;

            return PartialView("ReportByAreaPartialView", model);
        }

        [CustomActionFilter]
        public ActionResult ReportDebitColecction()
        {
            return View();
        }



        [HttpPost]
        public PartialViewResult ReportDebitColecctionPartialView(int? customerId, string customerName, DateTime? fromDate, DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                customerName = string.Empty;
                customerId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var ctx = new SmsContext();
            var customerIdParam = new SqlParameter
            {
                ParameterName = "CUSTOMER_ID",
                Value = Convert.ToInt32(customerId)
            };
            var customerNameParam = new SqlParameter
            {
                ParameterName = "CUSTOMER_NAME",
                Value = customerName
            };
            var FromDateParam = new SqlParameter
            {
                ParameterName = "FROM_DATE",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "TO_DATE",
                Value = Convert.ToDateTime(toDate)
            };
            var tonkho = ctx.Database.SqlQuery<ReportDebitColection>("exec SP_GET_REPORT_DEBT_COLLECTION @CUSTOMER_ID, @CUSTOMER_NAME, @FROM_DATE, @TO_DATE", customerIdParam, customerNameParam,
                FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<ReportDebitColection>();
            ReportDebitColectionModel model = new ReportDebitColectionModel();
            model.Details = tonkho;
            return PartialView("ReportDebitColecctionPartialView", model);
        }

        [CustomActionFilter]
        public ActionResult Return2ProviderReport()
        {
            return View();
        }

        public ActionResult GetRainfallChart()
        {
            var ctx = new SmsContext();
            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_GET_STATISTICS_BY_MONTH").Take(SystemConstant.MAX_ROWS).ToList<Report>();
            var chart = new Chart(width:800, height: 400)
                .AddSeries(
                        xValue: tonkho.Select(x => x.MONTH).ToArray(),
                        yValues: tonkho.Select(x => x.TOTAL).ToArray()
                      ).AddTitle("Biểu đồ doanh thu sau khi trừ trả hàng").Write();
            return null;
        }

        [HttpPost]
        public PartialViewResult Return2ProviderReportPartialView(int? providerId, string providerName,
            DateTime? fromDate, DateTime? toDate, int? currentPageIndex)
        {
            var ctx = new SmsContext();
            if (string.IsNullOrWhiteSpace(providerName))
            {
                providerName = string.Empty;
                providerId = 0;
            }
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }

            var providerIdParam = new SqlParameter
            {
                ParameterName = "PROVIDER_ID",
                Value = Convert.ToInt32(providerId)
            };
            var providerNameParam = new SqlParameter
            {
                ParameterName = "PROVIDER_NAME",
                Value = providerName
            };
            var FromDateParam = new SqlParameter
            {
                ParameterName = "FROM_DATE",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "TO_DATE",
                Value = Convert.ToDateTime(toDate)
            };
            int pageSize = SystemConstant.ROWS;
            int pageIndex = currentPageIndex == null ? 1 : (int)currentPageIndex;
            var details = ctx.Database.SqlQuery<ReportReturn2Provider>("exec SP_GET_REPORT_RETURN_2_PROVIDER @PROVIDER_ID, @PROVIDER_NAME, @FROM_DATE, @TO_DATE ", 
                providerIdParam, providerNameParam, FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<ReportReturn2Provider>();
            ReportReturn2ProviderModel model = new ReportReturn2ProviderModel();
            model.Count = details.Count;
            model.Details = details.ToPagedList(pageIndex, pageSize);

            ViewBag.ProviderId = providerId;
            ViewBag.ProviderName = providerName;
            ViewBag.FromDate = ((DateTime)fromDate).ToString("dd/MM/yyyy");
            ViewBag.Todate = ((DateTime)toDate).ToString("dd/MM/yyyy");

            return PartialView("Return2ProviderReportPartialView", model);
        }

        [CustomActionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [CustomActionFilter]
        public ActionResult MonthlyReport()
        {
            var ctx = new SmsContext();
            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_GET_STATISTICS_BY_MONTH").Take(SystemConstant.MAX_ROWS).ToList<Report>();
            var total = ctx.Database.SqlQuery<Report>("exec SP_GET_STATISTICS_OF_YEAR").Take(SystemConstant.MAX_ROWS).FirstOrDefault();
            ReportModel model = new ReportModel();
            model.TheList = tonkho;
            model.Total = total;
            return View(model);
        }

        public ActionResult GetRainfallChartWeek()
        {
            var ctx = new SmsContext();
            var tonkho = ctx.Database.SqlQuery<ReportWeek>("exec SP_GET_STATISTICS_BY_WEEK_OF_YEAR").Take(SystemConstant.MAX_ROWS).ToList<ReportWeek>();
            var chart = new Chart(width: 800, height: 400)
                .AddSeries(
                        xValue: tonkho.Select(x => x.WEEK).ToArray(),
                        yValues: tonkho.Select(x => x.TOTAL).ToArray()
                      ).AddTitle("Biểu đồ doanh thu sau khi trừ trả hàng").Write();
            return null;
        }

        [CustomActionFilter]
        public ActionResult WeeklyReport()
        {
            var ctx = new SmsContext();
            var tonkho = ctx.Database.SqlQuery<ReportWeek>("exec SP_GET_STATISTICS_BY_WEEK_OF_YEAR").Take(SystemConstant.MAX_ROWS).ToList<ReportWeek>();
            var total = ctx.Database.SqlQuery<Report>("exec SP_GET_STATISTICS_OF_YEAR").Take(SystemConstant.MAX_ROWS).FirstOrDefault();
            ReportWeekModel model = new ReportWeekModel();
            model.TheList = tonkho;
            model.Total = total;
            return View(model);
        }

        public ActionResult WeReport()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult WeReportPartialView(DateTime? fromDate, DateTime? toDate)
        {
            var ctx = new SmsContext();
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var FromDateParam = new SqlParameter
            {
                ParameterName = "START_TIME",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "END_TIME",
                Value = Convert.ToDateTime(toDate)
            };
            var tonkho = ctx.Database.SqlQuery<ReportWeek>("exec SP_REPORT_BY_WEEK @START_TIME, @END_TIME",
               FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<ReportWeek>();
            ReportWeekModel model = new ReportWeekModel();
            model.TheList = tonkho;
            return PartialView("WeReportPartialView", model);
        }
        
        [CustomActionFilter]
        public ActionResult DayReport()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult DayReportPartialView(DateTime? fromDate, DateTime? toDate)
        {
            var ctx = new SmsContext();
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var FromDateParam = new SqlParameter
            {
                ParameterName = "START_TIME",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "END_TIME",
                Value = Convert.ToDateTime(toDate)
            };
            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_REPORT_BY_DAY @START_TIME, @END_TIME",
               FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<Report>();
            ReportModel model = new ReportModel();
            model.TheList = tonkho;
            return PartialView("DayReportPartialView", model);
        }

        [CustomActionFilter]
        public ActionResult MthReport()
        {
            return View();
        }


        [HttpPost]
        public PartialViewResult MthReportPartialView(DateTime? fromDate, DateTime? toDate)
        {
            var ctx = new SmsContext();
            if (fromDate == null)
            {
                fromDate = SystemConstant.MIN_DATE;
            }
            if (toDate == null)
            {
                toDate = SystemConstant.MAX_DATE;
            }
            var FromDateParam = new SqlParameter
            {
                ParameterName = "START_TIME",
                Value = Convert.ToDateTime(fromDate)
            };
            var ToDateParam = new SqlParameter
            {
                ParameterName = "END_TIME",
                Value = Convert.ToDateTime(toDate)
            };
            var tonkho = ctx.Database.SqlQuery<Report>("exec SP_REPORT_BY_MONTH @START_TIME, @END_TIME",
               FromDateParam, ToDateParam).Take(SystemConstant.MAX_ROWS).ToList<Report>();
            ReportModel model = new ReportModel();
            model.TheList = tonkho;
            return PartialView("MthReportPartialView", model);
        }
    }
}
