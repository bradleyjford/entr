using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Entr.Net.Smtp
{
    public interface ISmtpContext
    {
        void QueueMessage(MailMessage message);
        IEnumerable<MailMessage> Messages { get; }
    }

    public class SmtpContext : ISmtpContext
    {
        readonly Lazy<List<MailMessage>> _messages = new Lazy<List<MailMessage>>(() => new List<MailMessage>());

        public void QueueMessage(MailMessage message)
        {
            _messages.Value.Add(message);
        }

        public IEnumerable<MailMessage> Messages => _messages.IsValueCreated ? _messages.Value : Enumerable.Empty<MailMessage>();
    }
}
