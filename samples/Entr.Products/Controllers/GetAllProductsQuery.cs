using System.Threading.Tasks;
using Entr.CommandQuery;
using Entr.Data;
using Entr.Products.Domain;

namespace Entr.Products.Controllers
{
    public class GetAllProductsQuery : IAsyncQuery<IPagedResult<Product>>
    {
        public GetAllProductsQuery(PagingOptions pagingOptions)
        {
            PagingOptions = pagingOptions;
        }

        public PagingOptions PagingOptions { get; }
    }

    public class GetAllProductsQueryHandler : IAsyncQueryHandler<GetAllProductsQuery, IPagedResult<Product>>
    {
        readonly ProductsDbContext _dbContext;

        public GetAllProductsQueryHandler(ProductsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IPagedResult<Product>> Handle(GetAllProductsQuery query)
        {
            return await _dbContext.Products.ToPagedResultAsync(query.PagingOptions, new SortDescriptor("Name"));
        }
    }
}
