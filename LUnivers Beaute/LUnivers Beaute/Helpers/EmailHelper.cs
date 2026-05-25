using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace LUnivers_Beaute.Helpers
{
    public static class EmailHelper
    {
        public static void SendOtpEmail(string recipientEmail, string otpCode)
        {
            string senderEmail = "";
            string senderPassword = "";

            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                if (File.Exists(configPath))
                {
                    string jsonString = File.ReadAllText(configPath);
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        var emailConfig = doc.RootElement.GetProperty("Email");
                        senderEmail = emailConfig.GetProperty("SenderEmail").GetString() ?? "";
                        senderPassword = emailConfig.GetProperty("AppPassword").GetString() ?? "";
                    }
                }
            }
            catch { }

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
            {
                throw new Exception("Chưa cấu hình SenderEmail và AppPassword trong hệ thống! Vui lòng vào Cài đặt để thiết lập.");
            }

            try
            {
                var fromAddress = new MailAddress(senderEmail, "L'Univers Beauté");
                var toAddress = new MailAddress(recipientEmail);
                string subject = "Mã xác nhận quên mật khẩu - L'Univers Beauté";
                string body = $"Chào bạn,\n\nMã xác nhận (OTP) để đặt lại mật khẩu của bạn là: {otpCode}\n\nVui lòng không chia sẻ mã này cho bất kỳ ai.\n\nTrân trọng,\nĐội ngũ L'Univers Beauté";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, senderPassword)
                };
                
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi gửi email: " + ex.Message);
            }
        }
    }
}
