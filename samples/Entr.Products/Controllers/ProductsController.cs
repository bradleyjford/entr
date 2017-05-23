using System;
using System.Threading.Tasks;
using Entr.CommandQuery;
using Entr.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Entr.Products.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        readonly ProductsDbContext _dbContext;
        readonly IMediator _mediator;

        public ProductsController(ProductsDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PagingOptions pagingOptions)
        {
            var query = new GetAllProductsQuery(pagingOptions);

            return Ok(await _mediator.SendAsync(query));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _dbContext.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
