using AutoPartesRazor.Interfaces;
using System.Net;
using System.Net.Mail;

namespace AutoPartesRazor.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    // Envio de correo
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtp = new SmtpClient(_config["SMTP:Host"])
        {
            Port = int.Parse(_config["SMTP:Port"]),
            Credentials = new NetworkCredential(_config["SMTP:User"], _config["SMTP:Pass"]),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(_config["SMTP:User"]),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        message.To.Add(email);

        await smtp.SendMailAsync(message);
    }
}

