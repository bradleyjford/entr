using System.Net.Mail;
using System.Threading.Tasks;
using Entr.CommandQuery;

namespace Entr.Net.Smtp;

public class SmtpContextAsyncCommandHandlerDecorator<TCommand, TResult> : IAsyncCommandHandler<TCommand, TResult>
    where TCommand : IAsyncCommand<TResult>
{
    readonly IAsyncCommandHandler<TCommand, TResult> _decorated;
    readonly SmtpClient _smtpClient;
    readonly ISmtpContext _smtpContext;

    public SmtpContextAsyncCommandHandlerDecorator(
        IAsyncCommandHandler<TCommand, TResult> decorated, 
        SmtpClient smtpClient,
        ISmtpContext smtpContext)
    {
        _decorated = decorated;
        _smtpClient = smtpClient;
        _smtpContext = smtpContext;
    }

    public async Task<TResult> Handle(TCommand command)
    {
        TResult result = await _decorated.Handle(command);

        await SendEmails();

        return result;
    }

    async Task SendEmails()
    {
        foreach (MailMessage message in _smtpContext.Messages)
        {
            await _smtpClient.SendMailAsync(message);
        }
    }
}
