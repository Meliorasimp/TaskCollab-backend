using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using app.Interface;
using Microsoft.Extensions.Options;

namespace app.Service
{
    public class EmailOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? From { get; set; }
    }
    public class DummyEmailService : IDummyEmailService
    {
        private readonly IOptions<EmailOptions> _option;
        public DummyEmailService(IOptions<EmailOptions> option)
        {
            _option = option;
        }
        public async Task SendDummyEmailAsync(string to, string subject, string body)
        {
            var client = new SmtpClient(_option.Value.Host, _option.Value.Port)
            {
                Credentials = new NetworkCredential(_option.Value.Username, _option.Value.Password),
                EnableSsl = true
            };
            var message = new MailMessage(
                from: _option.Value.From!,
                to: to,
                subject: subject,
                body: body
            );

            await client.SendMailAsync(message);
        }
    }
}