using System;
using System.Threading.Tasks;
using Entr.CommandQuery;
using Entr.Data;
using Entr.Products.Data;
using Entr.Products.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Entr.Products.Controllers
{
    [ApiController]
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

            var response = await _mediator.SendAsync(query);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Product>> Get([FromRoute] Guid id)
        {
            var pid = new ProductId(Guid.Parse("7ac27466-2f86-4105-be90-60d48e356537"));
            
            var product = await _dbContext.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == pid);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }
    }
}
