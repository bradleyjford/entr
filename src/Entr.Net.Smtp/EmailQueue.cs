using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Entr.Net.Smtp;

public sealed class EmailQueue
{
    readonly Lazy<List<MailMessage>> _messages =
        new (() => new List<MailMessage>());

    public void QueueMessage(MailMessage message)
    {
        _messages.Value.Add(message);
    }

    public IEnumerable<MailMessage> Messages =>
        _messages.IsValueCreated ? _messages.Value : Enumerable.Empty<MailMessage>();
}
