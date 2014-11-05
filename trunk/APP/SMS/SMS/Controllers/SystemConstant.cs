using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMS.Models;
using System.Text;
using System.Net.Mail;

namespace SMS.Controllers
{
    public class SystemConstant
    {
        public const int ROWS = 10;
        public const int MAX_ROWS = 999;
        private static System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("vi-VN");
        public static DateTime MIN_DATE = getMinDate();
        public static DateTime MAX_DATE = DateTime.ParseExact("01/01/9999", "dd/MM/yyyy", cultureinfo);
        public static string SALT = "SMS2014";
        public static DateTime getMinDate()
        {            
            return DateTime.ParseExact("01/01/2014", "dd/MM/yyyy", cultureinfo);
        }

        public static string RandomString(int size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }        

        public static void sendEmail(string mailTo, string mailFrom, string mailSubject, string mailBody, string username, string password)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(mailTo);
                mail.From = new MailAddress(mailFrom);
                mail.Subject = mailSubject;
                string Body = mailBody;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                (username, password);// Enter seders User name and password  
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

    }
    
}