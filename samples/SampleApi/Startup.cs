using Autofac;
using Entr.CommandQuery.Autofac;
using Entr.Net.Smtp;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SampleApi.Data;
using SampleApi.Products;
using SampleApi.Security;

namespace SampleApi;

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
        services.AddLogging();

        services.AddDbContext<SampleApiDbContext>(options =>
        {
            options.ReplaceService<IValueConverterSelector, EntrEntityIdValueConverterSelector>();
            options.UseSqlServer(Configuration.GetConnectionString("Default")!);
        });

        services.AddScoped<DbContext>(sp => sp.GetService<SampleApiDbContext>()!);
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddAutoMapper(AutomapperConfiguration.Configure);

        services.AddValidatorsFromAssemblyContaining<Startup>();
        services.AddControllers();
        services.AddHttpContextAccessor();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.MapType<ProductId>(() => new OpenApiSchema { Type = "string", Format = "uuid" });
        });

        services.AddScoped<EmailQueue>();
        services.AddScoped<IEmailSender, NullEmailSender>();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<UserContext>()
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        var thisAssembly = typeof(Startup).Assembly;

        builder.RegisterMediator();
        builder.RegisterMediatorAsyncQueryHandlers(thisAssembly);
        builder.RegisterMediatorAsyncCommandHandlers(thisAssembly,
            typeof(EmailQueueSenderAsyncCommandHandlerDecorator<,>),
            typeof(SampleApiUnitOfWorkCommandHandlerDecorator<,>));
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
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseEndpoints(c =>
        {
            c.MapControllers();
        });
    }
}
