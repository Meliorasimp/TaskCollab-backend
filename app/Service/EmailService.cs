using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Interface;
using MailKit.Net.Smtp;
using MimeKit;
namespace app.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendVerificationEmailAsync(string toEmail, string userName, string verificationToken)
        {
           string smtpHost = _configuration.GetValue<string>("Email:SmtpHost") ?? throw new InvalidOperationException("SMTP host is not configured.");
           int smtpPort = _configuration.GetValue<int>("Email:SmtpPort");
           string smtpUser = _configuration.GetValue<string>("Email:SmtpUser") ?? throw new InvalidOperationException("SMTP user is not configured.");
           string smtpPass = _configuration.GetValue<string>("Email:SmtpPassword") ?? throw new InvalidOperationException("SMTP password is not configured.");
           string frontendUrl = _configuration.GetValue<string>("Email:FrontendUrl") ?? throw new InvalidOperationException("Frontend URL is not configured.");

           string verificationUrl = $"{frontendUrl}/verify-email?token={verificationToken}";
           var message = new MimeMessage();
           message.From.Add(new MailboxAddress("TaskCollab", smtpUser));
           message.To.Add(new MailboxAddress(userName, toEmail));
           message.Subject = "Verify your email address";

           message.Body = new TextPart("html")
           {
               Text = $@"
                <h2>Hello {userName},</h2>
                <p>Thank you for registering at TaskCollab! Please verify your email address by clicking the link below:</p>
                <a href='{verificationUrl}'>Verify Email Address</a>
                <p>If you did not create an account, please ignore this email.</p>
                <br/>
               "
           };
            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}