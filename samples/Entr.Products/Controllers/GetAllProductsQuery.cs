using Entr.CommandQuery;
using Entr.Data;
using Entr.Products.Domain;
using System.Threading.Tasks;

namespace Entr.Products.Controllers
{
    public class GetAllProductsQuery : IAsyncQuery<IPagedResult<Product>>
    {
        public PagingOptions PagingOptions { get; } = new PagingOptions();
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
