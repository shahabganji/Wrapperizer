using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Cqrs.Behaviours.ErrorHandler;
using Wrapperizer.Extensions.DependencyInjection.Cqrs.Abstractions;

namespace Wrapperizer.Extensions.DependencyInjection.Cqrs.Behaviours.ErrorHandler
{
    public static class CqrsContextBuilderServiceCollectionExtensions
    {
        public static CqrsContext AddErrorHandler(this CqrsContext context)
        {
            context.Services.Add(new ServiceDescriptor(
                typeof(IPipelineBehavior<,>), typeof(ErrorHandlerBehaviour<,>),
                context.ServiceLifetime));

            return context;
        }
    }
}
