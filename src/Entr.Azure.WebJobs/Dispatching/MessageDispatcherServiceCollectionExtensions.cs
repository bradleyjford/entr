using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Entr.Azure.WebJobs.Dispatching
{
    public static class MessageDispatcherServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageDispatcher(this IServiceCollection services, params Assembly[] handlerAssemblies)
        {
            services.TryAddScoped<MessageDispatcher>();

            AddMessageHandlers(services, handlerAssemblies.Distinct().ToArray());

            return services;
        }

        private static void AddMessageHandlers(IServiceCollection services, IEnumerable<Assembly> handlerAssemblies)
        {
            var concreteTypes = new HashSet<Type>();
            var interfaceTypes = new HashSet<Type>();

            foreach (var type in handlerAssemblies.SelectMany(a => a.DefinedTypes).Where(t => !t.IsOpenGeneric()))
            {
                var closingInterfaceTypes = type
                    .FindInterfacesThatClose(typeof(IMessageHandler<>))
                    .ToArray();

                if (!closingInterfaceTypes.Any())
                {
                    continue;
                }

                if (type.IsConcrete())
                {
                    concreteTypes.Add(type);
                }

                foreach (var interfaceType in closingInterfaceTypes)
                {
                    interfaceTypes.Add(interfaceType);
                }
            }

            foreach (var interfaceType in interfaceTypes)
            {
                var exactMatches = concreteTypes.Where(x => x.CanCastTo(interfaceType));

                foreach (var type in exactMatches)
                {
                    services.AddTransient(interfaceType, type);
                }
            }
        }
    }
}
