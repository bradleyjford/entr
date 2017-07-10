using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Entr.Net.Http
{
    /// <summary>
    /// <seealso cref="IAsyncActionFilter">Async Action Filter</seealso> for handling 
    /// ModelState validation.
    /// </summary>
    public class ValidateModelStateAsyncActionFilter : IAsyncActionFilter
    {
        private readonly Predicate<ActionExecutingContext> _predicate = c => true;

        /// <summary>
        /// Intantiates a new <see cref="ValidateModelStateAsyncActionFilter">ValidateModelStateAsyncActionFilter</see>.
        /// </summary>
        public ValidateModelStateAsyncActionFilter()
        {
        }

        /// <summary>
        /// Instantites a new <see cref="ValidateModelStateAsyncActionFilter">ValidateModelStateAsyncActionFilter</see> 
        /// specifying a predicate that will be evalated on each request to determine if the filter is applied.
        /// </summary>
        /// <param name="predicate">
        /// Predicate that is evaluated on each request to determine if the filter will be applied.
        /// </param>
        /// <example>
        ///     The following example configures a global action filter for all requests fot API endpoints 
        ///     with a route starting with /api.
        ///     <code>
        ///     services.AddMvc(c => {
        ///        var validationFilter = new ValidateModelStateAsyncActionFilter(
        ///            ctx => ctx.HttpContext.Request.Path.StartsWithSegments(new PathString("/api"))
        ///        );
        ///
        ///        c.Filters.Add(validationFilter);
        ///     });
        ///     </code>
        /// </example>
        public ValidateModelStateAsyncActionFilter(Predicate<ActionExecutingContext> predicate)
        {
            _predicate = predicate;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_predicate(context) && !context.ModelState.IsValid)
            {
                context.Result = new BadRequestResult();

                return;
            }

            await next();
        }
    }
}
