using System.Net.Mail;

namespace Entr.Net.Smtp;

public class NullEmailSender : IEmailSender
{
    public Task SendMailAsync(MailMessage message)
    {
        return Task.CompletedTask;
    }
}
