using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace Entr.Net.Smtp;

static partial class Logs
{
    public static void LogEmail(this ILogger logger, MailMessage message)
    {
        LogEmail(logger, message.To, message.Subject);
        LogEmail(logger, message.CC, message.Subject);
        LogEmail(logger, message.Bcc, message.Subject);
    }
    
    static void LogEmail(ILogger logger, MailAddressCollection recipients, string subject)
    {
        foreach (var recipient in recipients)
        {
            LogEmail(logger, recipient.Address, subject);
        }
    }
    
    [LoggerMessage(EventName = "EmailSent", Level = LogLevel.Information,
        Message = "Sending email to {EmailAddress} with subject {EmailSubject}")]
    static partial void LogEmail(ILogger logger, string emailAddress, string emailSubject);

}
