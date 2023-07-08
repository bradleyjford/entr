using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Entr.Azure.WebJobs.Dispatching.ServiceBus
{
    public static class ServiceBusDispatcherServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceBusTopicMessageDispatcher(
            this IServiceCollection services,
            Action<ServiceBusDispatcherOptions> options,
            params Assembly[] handlerAssemblies)
        {
            var config = new ServiceBusDispatcherOptions();

            options(config);

            services.AddTransient<IHostedService>(serviceProvider =>
            {
                return new ServiceBusDispatcherHostedService(
                    config.ServiceBusConnectionString,
                    config.Topic,
                    config.Subscription,
                    config.MaxDeliveryCount,
                    config.MaxConcurrentCalls,
                    config.ContentTypeConverter,
                    serviceProvider.GetService<MessageDispatcher>(),
                    serviceProvider.GetService<ILogger<ServiceBusDispatcherHostedService>>());
            });

            return services;
        }
    }
}
