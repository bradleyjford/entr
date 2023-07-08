using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;

namespace Entr.CommandQuery.Autofac;

public static class ContainerBuilderExtensions
{
    public static void RegisterMediator(this ContainerBuilder builder)
    {
        builder.RegisterType<AutofacRequestHandlerResolver>().AsSelf().SingleInstance();

        builder.RegisterType<Mediator>().As<IMediator>()
            .WithParameter(
                (p, c) => p.ParameterType == typeof(IRequestHandlerResolver),
                (p, c) => c.Resolve<AutofacRequestHandlerResolver>())
            .SingleInstance();
    }

    public static void RegisterMediatorAsyncRequestHandlers(
        this ContainerBuilder builder,
        Assembly assembly,
        params Type[] decorators)
    {
        RegisterMediatorRequestHandlers(
            builder,
            typeof(IAsyncRequestHandler<,>),
            assembly,
            decorators);
    }

    public static void RegisterMediatorAsyncCommandHandlers(
        this ContainerBuilder builder,
        Assembly assembly,
        params Type[] decorators)
    {
        RegisterMediatorRequestHandlers(
            builder,
            typeof(IAsyncCommandHandler<,>),
            assembly,
            decorators);
    }

    public static void RegisterMediatorAsyncQueryHandlers(
        this ContainerBuilder builder,
        Assembly assembly,
        params Type[] decorators)
    {
        RegisterMediatorRequestHandlers(
            builder,
            typeof(IAsyncQueryHandler<,>),
            assembly,
            decorators);
    }

    static void RegisterMediatorRequestHandlers(
        ContainerBuilder builder,
        Type handlerType,
        Assembly assembly,
        params Type[] decorators)
    {
        builder.RegisterAssemblyTypes(assembly)
            .As(t => t.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(handlerType)));

        foreach (var decorator in decorators.Reverse())
        {
            builder.RegisterGenericDecorator(decorator, handlerType);
        }
    }
}
