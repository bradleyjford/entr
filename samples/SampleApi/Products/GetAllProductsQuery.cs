using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SampleApi.Data;

namespace SampleApi.Products;

public class GetAllProductsQuery : PagingOptions, IAsyncQuery<IPagedResult<ProductResponse>>
{
}

public class GetAllProductsQueryHandler : IAsyncQueryHandler<GetAllProductsQuery, IPagedResult<ProductResponse>>
{
    readonly SampleApiDbContext _dbContext;
    readonly IMapper _mapper;

    public GetAllProductsQueryHandler(
        SampleApiDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IPagedResult<ProductResponse>> Handle(GetAllProductsQuery command)
    {
        return await _dbContext.Products
            .ProjectTo<ProductResponse>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(command, new SortDescriptor(nameof(Product.Name)));
    }
}
