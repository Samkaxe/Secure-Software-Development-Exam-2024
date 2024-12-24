using System.Net;
using System.Net.Mail;
using Application.Interfaces;

namespace Application.Services;


public class EmailService : IEmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587; 
    private readonly string _fromEmail;
    private readonly string _emailPassword;

    public EmailService(string fromEmail, string emailPassword)
    {
        _fromEmail = fromEmail;
        _emailPassword = emailPassword;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using (var client = new SmtpClient(_smtpServer, _smtpPort))
        {
            client.Credentials = new NetworkCredential(_fromEmail, _emailPassword);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true 
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}