using System.Threading.Tasks;

namespace Entr.CommandQuery
{
    public interface IAsyncRequestHandler<in TAsyncRequest, TResponse>
        where TAsyncRequest : IAsyncRequest<TResponse>
    {
        Task<TResponse> Handle(TAsyncRequest request);
    }
}
