using System.Net.Mail;

namespace fashion.Services
{
    public class MailService
    {
        public static void SendRegistrationEmail(string emailAddress, string body = "")
        {
            // Cấu hình thông tin email
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Cổng SMTP
            string userName = "duong012018@gmail.com";
            string password = "rkydvuebxitbtltd";

            // Tạo nội dung email
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.From = new MailAddress(userName);
            mail.To.Add(new MailAddress(emailAddress));
            mail.Subject = "Registration Confirmation";
            mail.Body = body;

            // Gửi email sử dụng SmtpClient
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(userName, password);
            smtpClient.EnableSsl = true;
            smtpClient.Send(mail);
        }
    }
}