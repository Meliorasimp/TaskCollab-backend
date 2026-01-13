using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Interface
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string userName, string verificationToken);
    }

    public interface IDummyEmailService
    {
        Task SendDummyEmailAsync(string to, string subject, string body);
    }
}