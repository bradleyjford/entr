using System.Net.Mail;
using System.Threading.Tasks;
using Entr.CommandQuery;
using Microsoft.Extensions.Logging;

namespace Entr.Net.Smtp;

public class EmailQueueSenderAsyncCommandHandlerDecorator<TCommand, TResult> : IAsyncCommandHandler<TCommand, TResult>
    where TCommand : IAsyncCommand<TResult>
{
    readonly IAsyncCommandHandler<TCommand, TResult> _decorated;
    readonly IEmailSender _emailSender;
    readonly EmailQueue _emailQueue;
    readonly ILogger _logger;

    public EmailQueueSenderAsyncCommandHandlerDecorator(
        IAsyncCommandHandler<TCommand, TResult> decorated, 
        IEmailSender emailSender,
        EmailQueue emailQueue,
        ILogger<EmailQueueSenderAsyncCommandHandlerDecorator<TCommand, TResult>> logger)
    {
        _decorated = decorated;
        _emailSender = emailSender;
        _emailQueue = emailQueue;
        _logger = logger;
    }

    public async Task<TResult> Handle(TCommand command)
    {
        TResult result = await _decorated.Handle(command);

        await SendEmails();

        return result;
    }

    async Task SendEmails()
    {
        foreach (var message in _emailQueue.Messages)
        {
            await _emailSender.SendMailAsync(message);

            _logger.LogEmail(message);
        }
    }

}
