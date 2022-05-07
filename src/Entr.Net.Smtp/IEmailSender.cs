using System.Net.Mail;

namespace Entr.Net.Smtp;

public interface IEmailSender
{
    Task SendMailAsync(MailMessage message);
}
