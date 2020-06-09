using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Extensions.Cqrs.EfCore.Behaviors;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class WrapperizerCqrsEfCoreContextBuilderExtensions
    {
        public static WrapperizerCqrsContextBuilder AddTransactionalCommands(
            this WrapperizerCqrsContextBuilder context)
        {
            context.ServiceCollection.Add(
                new ServiceDescriptor(
                    typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>),
                    context.ServiceLifetime));
            return context;
        }
    }
}
