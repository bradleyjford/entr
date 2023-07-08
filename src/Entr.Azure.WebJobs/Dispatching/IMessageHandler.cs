using System;
using System.Threading.Tasks;

namespace Entr.Azure.WebJobs.Dispatching
{
    public interface IMessageHandler<in TRequest>
    {
        Task Handle(TRequest request);
    }
}
