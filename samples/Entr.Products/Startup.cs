using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Entr.CommandQuery.Autofac;
using Entr.Data.EntityFramework;
using Entr.Products.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Entr.Products
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDbContext<ProductsDbContext>(options =>
            {
                options.ReplaceService<IValueConverterSelector, EntrEntityIdValueConverterSelector>();
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var thisAssembly = typeof(Startup).Assembly;

            builder.RegisterMediator();
            builder.RegisterMediatorAsyncQueryHandlers(thisAssembly);
            builder.RegisterMediatorAsyncCommandHandlers(thisAssembly,
                typeof(UnitOfWorkAsyncCommandHandlerDecorator<,>));
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostApplicationLifetime appLifetime,
            IWebHostEnvironment env, 
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseEndpoints(c =>
            {
                c.MapControllers();
            });
        }
    }
}
