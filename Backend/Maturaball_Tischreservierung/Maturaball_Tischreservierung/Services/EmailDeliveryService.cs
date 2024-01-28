using MimeKit;
using System.Net.Mail;
using System.Net;
using Maturaball_Tischreservierung.Models;

namespace Maturaball_Tischreservierung.Services
{
    public class EmailDeliveryService : UsesDatabase
    {
        public EmailDeliveryService(ApiConfig apiConfig) : base(apiConfig)
        {
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            // send email with local postfix server

            SmtpClient client = new(_apiConfig.SmtpAddress, _apiConfig.SmtpPort)
            {
                EnableSsl = false 
            };

            // Create the email
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("noreply@eliastraunbauer.com"),
                Subject = subject,
                Body = body
            };
            mail.To.Add(email);

            // Send the email
            try
            {
                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sending email: " + ex.Message);
            }
        }
    }
}
