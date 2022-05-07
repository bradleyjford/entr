using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SampleApi.Data;

namespace SampleApi.Products;

[ApiController]
[Route("api/products")]
public class ProductsController : Controller
{
    readonly SampleApiDbContext _dbContext;
    readonly IMediator _mediator;
    readonly IMapper _mapper;

    public ProductsController(
        SampleApiDbContext dbContext,
        IMediator mediator,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IPagedResult<ProductResponse>> Get([FromQuery] GetAllProductsQuery query)
    {
        return await _mediator.SendAsync(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ProductResponse>> Get([FromRoute] ProductId id)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .ProjectTo<ProductResponse>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductCommand command)
    {
        var product = await _mediator.SendAsync(command);

        var uri = Url.Action(nameof(Get), new { id = product.Id });

        var response = _mapper.Map<ProductResponse>(product);

        return Created(uri!, product);
    }
}
