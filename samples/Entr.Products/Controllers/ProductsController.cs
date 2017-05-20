using System.Threading.Tasks;
using Entr.CommandQuery;
using Microsoft.AspNetCore.Mvc;

namespace Entr.Products.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get(GetAllProductsQuery query)
        {
            return Ok(await _mediator.SendAsync(query));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        
    }
}
