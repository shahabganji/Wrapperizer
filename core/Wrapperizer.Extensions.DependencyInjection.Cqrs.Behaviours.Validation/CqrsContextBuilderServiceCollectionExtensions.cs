using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Cqrs.Behaviours.Validation;
using Wrapperizer.Extensions.DependencyInjection.Cqrs.Abstractions;

namespace Wrapperizer.Extensions.DependencyInjection.Cqrs.Behaviours.Validation
{
    public static class CqrsContextBuilderServiceCollectionExtensions
    {
        public static CqrsContext AddValidation(this CqrsContext context)
        {
            context.Services.Add(new ServiceDescriptor(
                typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>),
                context.ServiceLifetime));

            return context;
        }
    }
}
